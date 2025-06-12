Imports System.IO
Imports System.Text.RegularExpressions
Imports Infragistics.Win.UltraWinToolbars
Imports VectorDraw.Actions
Imports VectorDraw.Geometry
Imports VectorDraw.Professional.vdCollections
Imports VectorDraw.Professional.vdFigures
Imports VectorDraw.Professional.vdObjects
Imports VectorDraw.Professional.vdPrimaries

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class GraphicalRedliningForm

    Private _clickPoint As gPoint
    Private _currentCommandAction As String
    Private _currentCommandActionCount As Integer
    Private _cursors As New Dictionary(Of String, Cursor)
    Private _documentStreamString As String
    Private _hitEntity As vdFigure
    Private _hoveredEntity As vdFigure
    Private _isArrow As Boolean
    Private _isDirty As Boolean
    Private _movePoint As gPoint
    Private _objectName As String
    Private _selectedGripPointIndex As Integer
    Private _selection As vdSelection

    Public Sub New(documentStreamString As String, objectName As String, Optional background As Bitmap = Nothing, Optional boundingBox As Box = Nothing)
        InitializeComponent()

        _documentStreamString = documentStreamString
        _objectName = objectName

        Me.BackColor = Color.White
        Me.Icon = My.Resources.EditRedlining

        Me.uceFontSize.Items.Add(8, "8")
        Me.uceFontSize.Items.Add(9, "9")
        Me.uceFontSize.Items.Add(10, "10")
        Me.uceFontSize.Items.Add(11, "11")
        Me.uceFontSize.Items.Add(12, "12")
        Me.uceFontSize.Items.Add(14, "14")
        Me.uceFontSize.Items.Add(16, "16")
        Me.uceFontSize.Items.Add(18, "18")
        Me.uceFontSize.Items.Add(20, "20")
        Me.uceFontSize.Items.Add(22, "22")
        Me.uceFontSize.Items.Add(24, "24")
        Me.uceFontSize.Items.Add(26, "26")
        Me.uceFontSize.Items.Add(28, "28")
        Me.uceFontSize.Items.Add(36, "36")
        Me.uceFontSize.Items.Add(48, "48")
        Me.uceFontSize.Items.Add(72, "72")

        Me.uckShowBackground.Checked = True

        Me.utmGraphicalRedlining.Style = ToolbarStyle.Office2010
        Me.utmGraphicalRedlining.Toolbars(0).Tools("DrawArc").SharedProps.AppearancesSmall.Appearance.Image = My.Resources.DrawArc.ToBitmap
        Me.utmGraphicalRedlining.Toolbars(0).Tools("DrawArrow").SharedProps.AppearancesSmall.Appearance.Image = My.Resources.DrawArrow.ToBitmap
        Me.utmGraphicalRedlining.Toolbars(0).Tools("DrawCircle").SharedProps.AppearancesSmall.Appearance.Image = My.Resources.DrawCircle.ToBitmap
        Me.utmGraphicalRedlining.Toolbars(0).Tools("DrawDimension").SharedProps.AppearancesSmall.Appearance.Image = My.Resources.DrawDimension.ToBitmap
        Me.utmGraphicalRedlining.Toolbars(0).Tools("DrawLine").SharedProps.AppearancesSmall.Appearance.Image = My.Resources.DrawLine.ToBitmap
        Me.utmGraphicalRedlining.Toolbars(0).Tools("DrawPolyline").SharedProps.AppearancesSmall.Appearance.Image = My.Resources.DrawPolyline.ToBitmap
        Me.utmGraphicalRedlining.Toolbars(0).Tools("DrawRectangle").SharedProps.AppearancesSmall.Appearance.Image = My.Resources.DrawRectangle.ToBitmap
        Me.utmGraphicalRedlining.Toolbars(0).Tools("DrawText").SharedProps.AppearancesSmall.Appearance.Image = My.Resources.DrawText.ToBitmap

        DirectCast(Me.utmGraphicalRedlining.Toolbars(0).Tools("ActiveHatch"), ComboBoxTool).SelectedIndex = 0
        DirectCast(Me.utmGraphicalRedlining.Toolbars(0).Tools("ActiveLineStyle"), ComboBoxTool).SelectedIndex = 4
        DirectCast(Me.utmGraphicalRedlining.Toolbars(0).Tools("ActivePenColor"), PopupColorPickerTool).SelectedColor = Color.Black
        DirectCast(Me.utmGraphicalRedlining.Toolbars(0).Tools("ActivePenWidth"), ComboBoxTool).SelectedIndex = 0

        Me.utmGraphicalRedlining.Toolbars(0).Tools("ShowGrid").SharedProps.AppearancesSmall.Appearance.Image = My.Resources.ShowGrid.ToBitmap
        DirectCast(Me.utmGraphicalRedlining.Toolbars(0).Tools("ShowGrid"), StateButtonTool).Checked = True

        Me.utmGraphicalRedlining.Toolbars(0).Tools("SnapToGrid").SharedProps.AppearancesSmall.Appearance.Image = My.Resources.SnapToGrid.ToBitmap

        Me.utmGraphicalRedlining.Toolbars(0).Tools("ZoomExtends").SharedProps.AppearancesSmall.Appearance.Image = My.Resources.ZoomExtends.ToBitmap
        Me.utmGraphicalRedlining.Toolbars(0).Tools("ZoomWindow").SharedProps.AppearancesSmall.Appearance.Image = My.Resources.ZoomWindow.ToBitmap

        Me.utmGraphicalRedlining.Toolbars(0).Tools("Export").SharedProps.AppearancesSmall.Appearance.Image = My.Resources.ExportGraphic.ToBitmap
        Me.utmGraphicalRedlining.Toolbars(0).Tools("Print").SharedProps.AppearancesSmall.Appearance.Image = My.Resources.Print.ToBitmap
        Me.utmGraphicalRedlining.Toolbars(0).Tools("Save").SharedProps.AppearancesSmall.Appearance.Image = My.Resources.SaveDocument.ToBitmap

        InitializeCursors()
        InitializeVectorDrawBaseControl()

        If (_documentStreamString = String.Empty) AndAlso (background IsNot Nothing) Then InitializeBackground(background, boundingBox)

        _selection = Me.vDraw.ActiveDocument.Selections(0)

        Me.vDraw.ActiveDocument.ZoomExtents()
        Me.vDraw.ActiveDocument.Invalidate()
    End Sub


    Private Sub AddFigureToSelection(Optional figure As vdFigure = Nothing)
        Dim selFigure As vdFigure = figure

        If (selFigure Is Nothing) Then selFigure = _hitEntity
        If (selFigure IsNot Nothing) Then
            If (selFigure.XProperties.FindName("PenColor") Is Nothing) Then selFigure.XProperties.Add("PenColor")

            If (Not _selection.FindItem(selFigure)) Then
                selFigure.XProperties.FindName("PenColor").PropValue = selFigure.PenColor.SystemColor
                selFigure.PenColor.SystemColor = Color.Magenta

                _selection.AddItem(selFigure, True, vdSelection.AddItemCheck.Nochecking)
            ElseIf (E3.Lib.DotNet.Expansions.Devices.My.Computer.Keyboard.CtrlKeyDown) AndAlso (_selection.FindItem(selFigure)) Then
                selFigure.PenColor.SystemColor = DirectCast(selFigure.XProperties.FindName("PenColor").PropValue, Color)
                selFigure.ShowGrips = False

                _selection.RemoveItem(selFigure)
            End If

            selFigure.Update()
            selFigure.Invalidate()

            If (_selection.Count = 1) AndAlso (TypeOf _selection(0) Is vdDimension OrElse TypeOf _selection(0) Is vdText) Then
                Me.upnTextEdit.Visible = True
            Else
                Me.upnTextEdit.Visible = False
            End If
        End If
    End Sub

    Private Sub DeselectAll()
        If (_selection IsNot Nothing) AndAlso (_selection.Count <> 0) Then
            For Each selFigure As vdFigure In _selection
                selFigure.PenColor.SystemColor = DirectCast(selFigure.XProperties.FindName("PenColor").PropValue, Color)
                selFigure.ShowGrips = False

                selFigure.Update()
            Next

            _selection.RemoveAll()

            Me.upnTextEdit.Visible = False
            Me.vDraw.ActiveDocument.Invalidate()
        End If
    End Sub

    Private Sub Export()
        With sfdExport
            .DefaultExt = KnownFile.DXF.Trim("."c)
            .FileName = String.Format("Graphical_redlining_for_{0}", Regex.Replace(_objectName, "\W", "_"))
            .Filter = "Autodesk DXF file (*.dxf)|*.dxf|JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|Bitmap (*.bmp)|*.bmp"
            .Title = GraphicalRedliningFormStrings.ExportRedliningToFile_Title

            If (.ShowDialog(Me) = DialogResult.OK) Then
                Try
                    If (VdOpenSave.SaveAs(Me.vDraw.ActiveDocument, sfdExport.FileName)) Then
                        MessageBox.Show(GraphicalRedliningFormStrings.ExportRedliningSuccess_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Else
                        MessageBox.Show(GraphicalRedliningFormStrings.ProblemExportRedlining_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                Catch ex As Exception
                    MessageBox.Show(String.Format(GraphicalRedliningFormStrings.ErrorExportRedlining_Msg, vbCrLf, ex.Message), [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        End With
    End Sub

    Private Function GetGridSnapPoint(point As gPoint) As gPoint
        Dim snapX As Double = Me.vDraw.ActiveDocument.SnapSpaceX
        Dim snapY As Double = Me.vDraw.ActiveDocument.SnapSpaceY

        If (Me.vDraw.ActiveDocument.SnapMode) AndAlso (snapX <> 0.1 AndAlso snapY <> 0.1) Then
            Return New gPoint(CLng(point.x / snapX) * snapX, CLng(point.y / snapY) * snapY, 0)
        Else
            Return point
        End If
    End Function

    Private Sub InitializeCursors()
        Dim cursorResources As New Dictionary(Of String, Byte())
        cursorResources.Add("BasePoint", My.Resources.BasePoint)
        cursorResources.Add("DimOffset", My.Resources.DimOffset)
        cursorResources.Add("DimRefPoint", My.Resources.DimRefPoint)
        cursorResources.Add("EndAngle", My.Resources.EndAngle)
        cursorResources.Add("EndPoint", My.Resources.EndPoint)
        cursorResources.Add("Radius", My.Resources.Radius)
        cursorResources.Add("Rotation", My.Resources.Rotation)
        cursorResources.Add("StartAngle", My.Resources.StartAngle)
        cursorResources.Add("Vertex", My.Resources.Vertex)

        _cursors = New Dictionary(Of String, Cursor)

        For Each cursorResource As KeyValuePair(Of String, Byte()) In cursorResources
            Using cursorStream As New IO.MemoryStream(cursorResource.Value)
                _cursors.Add(cursorResource.Key, New Cursor(cursorStream))

                cursorStream.Close()
            End Using
        Next
    End Sub

    Private Sub InitializeBackground(background As Bitmap, boundingBox As Box)
        Dim layer As New vdLayer
        With layer
            .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
            .setDocumentDefaults()

            .Name = "Background"
        End With

        _isDirty = True

        With Me.vDraw.ActiveDocument
            .Layers.Add(layer)

            Dim imageDefinition As New vdImageDef(Me.vDraw.ActiveDocument, "Background")

            .Images.AddItem(imageDefinition)

            With imageDefinition
                .Image.SelectImage(background)
                .InternalSetBytes(New ByteArray(.Image.GetImageBytes(True)))
                .Update()
            End With

            Dim image As New vdImage
            With image
                .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                .setDocumentDefaults()

                .Height = boundingBox.Height
                .ImageDefinition = imageDefinition
                .InsertionPoint = boundingBox.Min
                .Layer = Me.vDraw.ActiveDocument.Layers.FindName("Background")
                .PenColor.SystemColor = Color.Transparent
                .Width = boundingBox.Width
            End With

            .ActiveLayOut.Entities.AddItem(image)

            .Layers.FindName("Background").Lock = True
            .Limits = image.BoundingBox
        End With
    End Sub

    Private Sub InitializeVectorDrawBaseControl()
        Me.vDraw.AllowDrop = False
        Me.vDraw.EnsureDocument()
        Me.vDraw.InputDevice_TouchGesture.Actions = VectorDraw.Professional.Actions.TouchGestureActions.Pan Or VectorDraw.Professional.Actions.TouchGestureActions.Zoom

        If (_documentStreamString <> String.Empty) Then
            Dim documentLoadedSuccessfully As Boolean

            Try
                Dim documentStream As New IO.MemoryStream(Convert.FromBase64String(_documentStreamString))
                documentStream.Position = 0

                documentLoadedSuccessfully = Me.vDraw.ActiveDocument.LoadFromMemory(documentStream, True)

                documentStream.Close()
            Catch ex As Exception
                documentLoadedSuccessfully = False
            End Try

            If (Not documentLoadedSuccessfully) Then
                MessageBox.Show(GraphicalRedliningFormStrings.FailedLoadRedlining_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
            End If
        End If

        With Me.vDraw.ActiveDocument
            .UndoHistory.PushEnable(False)

            .ActiveDimStyle.ArrowSize = 2.5
            .ActiveDimStyle.DecimalPrecision = 2
            .ActiveDimStyle.TextHeight = 2.5
            .ActiveHatchProperties = New vdHatchProperties(VectorDraw.Professional.Constants.VdConstFill.VdFillModeNone)
            .ActiveLineType = Me.vDraw.ActiveDocument.LineTypes.Solid
            .ActivePenColor.SystemColor = Color.Black
            .ActivePenWidth = 0
            .ActiveTextStyle.Height = 3

            .DisableZoomOnResize = True
            .EnableAutoGripOn = False
            .EnableUrls = False

            With .GlobalRenderProperties
                .AxisSize = 10
                .CrossSize = 8
                .GridColor = Color.Black
                .SelectingCrossColor = Color.Transparent
                .SelectingWindowColor = Color.Transparent
            End With

            .GridMode = True
            .GridSpaceX = 2
            .GridSpaceY = 2
            .Limits = .ActiveLayOut.Entities.GetBoundingBox(True, True)
            .OrbitActionKey = vdDocument.KeyWithMouseStroke.None
            .OsnapDialogKey = Keys.None
            .Background = Color.White
            .Palette.Background = Color.White
            .ShowUCSAxis = False
            .SnapSpaceX = .GridSpaceX
            .SnapSpaceY = .GridSpaceY
            .UrlActionKey = Keys.None
            .Selections.Add(New vdSelection)
        End With
    End Sub

    Private Sub Print()
        Dim printing As New HarnessAnalyzer.Printing.VdPrinting(Me.vDraw.ActiveDocument)
        printing.DocumentName = Me.Text

        Using printForm As New Printing.PrintForm(printing, Nothing, False)
            printForm.ShowDialog(Me)
        End Using
    End Sub

    Private Sub Save()
        DeselectAll()

        Dim byteArray As Byte()

        Dim documentStream As IO.MemoryStream = Me.vDraw.ActiveDocument.ToStream(True)
        If (documentStream IsNot Nothing) Then
            byteArray = documentStream.ToArray()

            documentStream.Close()

            _documentStreamString = Convert.ToBase64String(byteArray)
        End If

        _isDirty = False
    End Sub

    Private Sub GraphicalRedliningForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If (Me.vDraw.ActiveDocument.CommandAction.OpenLoops <> 0) Then Me.vDraw.ActiveDocument.CommandAction.Cancel()

        If (_isDirty) AndAlso (Me.DialogResult = System.Windows.Forms.DialogResult.OK OrElse (Me.DialogResult = System.Windows.Forms.DialogResult.Cancel AndAlso MessageBox.Show(GraphicalRedliningFormStrings.SaveChanges_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = System.Windows.Forms.DialogResult.Yes)) Then Save()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        If (Me.vDraw.ActiveDocument.CommandAction.OpenLoops <> 0) Then Me.vDraw.ActiveDocument.CommandAction.Cancel()

        Me.DialogResult = System.Windows.Forms.DialogResult.Abort
    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        If (Me.vDraw.ActiveDocument.CommandAction.OpenLoops <> 0) Then Me.vDraw.ActiveDocument.CommandAction.Cancel()

        Me.DialogResult = System.Windows.Forms.DialogResult.OK
    End Sub

    Private Sub txtText_TextChanged(sender As Object, e As EventArgs) Handles txtText.TextChanged
        If (_selection IsNot Nothing) AndAlso (_selection.Count <> 0) AndAlso (TypeOf _selection(0) Is vdDimension OrElse TypeOf _selection(0) Is vdText) Then
            _isDirty = True

            If (TypeOf _selection(0) Is vdDimension) Then
                DirectCast(_selection(0), vdDimension).dimText = Me.txtText.Text
            Else
                DirectCast(_selection(0), vdText).TextString = Me.txtText.Text
            End If

            _selection(0).Update()

            Me.vDraw.ActiveDocument.Invalidate()
        End If
    End Sub

    Private Sub uceFontSize_TextChanged(sender As Object, e As EventArgs) Handles uceFontSize.TextChanged
        If (IsNumeric(Me.uceFontSize.Text)) AndAlso (_selection IsNot Nothing) AndAlso (_selection.Count <> 0) AndAlso (TypeOf _selection(0) Is vdDimension OrElse TypeOf _selection(0) Is vdText) Then
            _isDirty = True

            If (TypeOf _selection(0) Is vdDimension) Then
                DirectCast(_selection(0), vdDimension).TextHeight = CDbl(Me.uceFontSize.Text) / 3
            Else
                DirectCast(_selection(0), vdText).Height = CDbl(Me.uceFontSize.Text) / 3
            End If

            _selection(0).Update()

            Me.vDraw.ActiveDocument.Invalidate()
        End If
    End Sub

    Private Sub uckBold_CheckedValueChanged(sender As Object, e As EventArgs) Handles uckBold.CheckedValueChanged
        If (_selection IsNot Nothing) AndAlso (_selection.Count <> 0) AndAlso (TypeOf _selection(0) Is vdDimension OrElse TypeOf _selection(0) Is vdText) Then
            _isDirty = True

            If (TypeOf _selection(0) Is vdDimension) Then
                DirectCast(_selection(0), vdDimension).TextStyle.Bold = Me.uckBold.Checked
            Else
                DirectCast(_selection(0), vdText).Bold = Me.uckBold.Checked
            End If

            _selection(0).Update()

            Me.vDraw.ActiveDocument.Invalidate()
        End If
    End Sub

    Private Sub uckItalic_CheckedValueChanged(sender As Object, e As EventArgs) Handles uckItalic.CheckedValueChanged
        If (_selection IsNot Nothing) AndAlso (_selection.Count <> 0) AndAlso (TypeOf _selection(0) Is vdDimension OrElse TypeOf _selection(0) Is vdText) Then
            _isDirty = True

            If (TypeOf _selection(0) Is vdDimension) Then
                DirectCast(_selection(0), vdDimension).TextStyle.IsItalic = Me.uckItalic.Checked
            Else
                DirectCast(_selection(0), vdText).Style.IsItalic = Me.uckItalic.Checked
            End If

            _selection(0).Update()

            Me.vDraw.ActiveDocument.Invalidate()
        End If
    End Sub

    Private Sub uckShowBackground_CheckedValueChanged(sender As Object, e As EventArgs) Handles uckShowBackground.CheckedValueChanged
        If (Me.vDraw.ActiveDocument IsNot Nothing) AndAlso (Me.vDraw.ActiveDocument.Layers.FindName("Background") IsNot Nothing) Then
            Me.vDraw.ActiveDocument.Layers.FindName("Background").Frozen = Not Me.uckShowBackground.Checked
            Me.vDraw.ActiveDocument.Invalidate()
        End If
    End Sub

    Private Sub uckUnderline_CheckedValueChanged(sender As Object, e As EventArgs) Handles uckUnderline.CheckedValueChanged
        If (_selection IsNot Nothing) AndAlso (_selection.Count <> 0) AndAlso (TypeOf _selection(0) Is vdDimension OrElse TypeOf _selection(0) Is vdText) Then
            _isDirty = True

            If (TypeOf _selection(0) Is vdDimension) Then
                DirectCast(_selection(0), vdDimension).TextStyle.IsUnderLine = Me.uckUnderline.Checked
            Else
                DirectCast(_selection(0), vdText).Style.IsUnderLine = Me.uckUnderline.Checked
            End If

            _selection(0).Update()

            Me.vDraw.ActiveDocument.Invalidate()
        End If
    End Sub

    Private Sub upnTextEdit_VisibleChanged(sender As Object, e As EventArgs) Handles upnTextEdit.VisibleChanged
        If (_selection IsNot Nothing) AndAlso (_selection.Count <> 0) Then
            If (TypeOf _selection(0) Is vdDimension) Then
                With DirectCast(_selection(0), vdDimension)
                    If (.dimText = String.Empty) Then
                        Me.txtText.Text = Math.Round(.Measurement, 2).ToString
                    Else
                        Me.txtText.Text = .dimText
                    End If

                    Me.uceFontSize.Text = (.TextHeight * 3).ToString
                    Me.uckBold.Checked = .TextStyle.Bold
                    Me.uckItalic.Checked = .TextStyle.IsItalic
                    Me.uckUnderline.Checked = .TextStyle.IsUnderLine
                End With
            ElseIf (TypeOf _selection(0) Is vdText) Then
                With DirectCast(_selection(0), vdText)
                    Me.txtText.Text = .TextString
                    Me.uceFontSize.Text = (.Height * 3).ToString
                    Me.uckBold.Checked = .Bold
                    Me.uckItalic.Checked = .Style.IsItalic
                    Me.uckUnderline.Checked = .Style.IsUnderLine
                End With
            End If
        End If
    End Sub

    Private Sub utmGraphicalRedlining_AfterToolCloseup(sender As Object, e As ToolDropdownEventArgs) Handles utmGraphicalRedlining.AfterToolCloseup
        Dim modifySelection As New vdSelection
        modifySelection.AddRange(_selection, vdSelection.AddItemCheck.Nochecking)

        DeselectAll()

        Select Case e.Tool.Key
            Case "ActiveHatch"
                Dim hatchProperties As New vdHatchProperties
                With hatchProperties
                    .SetUnRegisterDocument(Me.vDraw.ActiveDocument)

                    .FillColor = Me.vDraw.ActiveDocument.ActivePenColor
                    .FillMode = [Enum].Parse(Of VectorDraw.Professional.Constants.VdConstFill)(DirectCast(DirectCast(e.Tool, ComboBoxTool).SelectedItem, Infragistics.Win.ValueListItem).DataValue.ToString)

                    If (.FillMode = VectorDraw.Professional.Constants.VdConstFill.VdFillModeSolid) Then
                        .FillBkColor = Me.vDraw.ActiveDocument.ActivePenColor
                    Else
                        .FillBkColor.SystemColor = Color.Transparent
                    End If
                End With

                Me.vDraw.ActiveDocument.ActiveHatchProperties = hatchProperties

                For Each selFigure As vdFigure In modifySelection
                    _isDirty = True

                    If (TypeOf selFigure Is vdArc) Then
                        DirectCast(selFigure, vdArc).HatchProperties = Me.vDraw.ActiveDocument.ActiveHatchProperties
                    ElseIf (TypeOf selFigure Is vdCircle) Then
                        DirectCast(selFigure, vdCircle).HatchProperties = Me.vDraw.ActiveDocument.ActiveHatchProperties
                    ElseIf (TypeOf selFigure Is vdPolyline) AndAlso (selFigure.Label = String.Empty) Then
                        DirectCast(selFigure, vdPolyline).HatchProperties = Me.vDraw.ActiveDocument.ActiveHatchProperties
                    ElseIf (TypeOf selFigure Is vdRect) Then
                        DirectCast(selFigure, vdRect).HatchProperties = Me.vDraw.ActiveDocument.ActiveHatchProperties
                    End If

                    AddFigureToSelection(selFigure)
                Next
            Case "ActiveLineStyle"
                Me.vDraw.ActiveDocument.ActiveLineType = Me.vDraw.ActiveDocument.LineTypes.FindName(DirectCast(DirectCast(e.Tool, ComboBoxTool).SelectedItem, Infragistics.Win.ValueListItem).DataValue.ToString)

                For Each selFigure As vdFigure In modifySelection
                    _isDirty = True

                    selFigure.LineType = Me.vDraw.ActiveDocument.ActiveLineType

                    AddFigureToSelection(selFigure)
                Next
            Case "ActivePenColor"
                Me.vDraw.ActiveDocument.ActiveHatchProperties.FillColor.SystemColor = DirectCast(e.Tool, PopupColorPickerTool).SelectedColor
                Me.vDraw.ActiveDocument.ActivePenColor.SystemColor = DirectCast(e.Tool, PopupColorPickerTool).SelectedColor

                If (Me.vDraw.ActiveDocument.ActiveHatchProperties.FillMode = VectorDraw.Professional.Constants.VdConstFill.VdFillModeSolid) Then
                    Me.vDraw.ActiveDocument.ActiveHatchProperties.FillBkColor = Me.vDraw.ActiveDocument.ActivePenColor
                Else
                    Me.vDraw.ActiveDocument.ActiveHatchProperties.FillBkColor.SystemColor = Color.Transparent
                End If

                For Each selFigure As vdFigure In modifySelection
                    _isDirty = True

                    selFigure.PenColor = Me.vDraw.ActiveDocument.ActivePenColor

                    If (TypeOf selFigure Is vdArc) Then
                        DirectCast(selFigure, vdArc).HatchProperties = Me.vDraw.ActiveDocument.ActiveHatchProperties
                    ElseIf (TypeOf selFigure Is vdCircle) Then
                        DirectCast(selFigure, vdCircle).HatchProperties = Me.vDraw.ActiveDocument.ActiveHatchProperties
                    ElseIf (TypeOf selFigure Is vdPolyline) Then
                        If (selFigure.Label = String.Empty) Then
                            DirectCast(selFigure, vdPolyline).HatchProperties = Me.vDraw.ActiveDocument.ActiveHatchProperties
                        Else
                            DirectCast(selFigure, vdPolyline).HatchProperties.FillColor = Me.vDraw.ActiveDocument.ActivePenColor
                        End If
                    ElseIf (TypeOf selFigure Is vdRect) Then
                        DirectCast(selFigure, vdRect).HatchProperties = Me.vDraw.ActiveDocument.ActiveHatchProperties
                    End If

                    AddFigureToSelection(selFigure)
                Next
            Case "ActivePenWidth"
                Me.vDraw.ActiveDocument.ActivePenWidth = CDbl(DirectCast(DirectCast(e.Tool, ComboBoxTool).SelectedItem, Infragistics.Win.ValueListItem).DataValue)

                For Each selFigure As vdFigure In modifySelection
                    _isDirty = True

                    selFigure.PenWidth = Me.vDraw.ActiveDocument.ActivePenWidth

                    AddFigureToSelection(selFigure)
                Next
        End Select

        modifySelection.RemoveAll()
        modifySelection.Dispose()

        Me.vDraw.ActiveDocument.Invalidate()
    End Sub

    Private Sub utmGraphicalRedlining_ToolClick(sender As Object, e As ToolClickEventArgs) Handles utmGraphicalRedlining.ToolClick
        If (Me.vDraw.ActiveDocument IsNot Nothing) Then
            _isArrow = e.Tool.Key = "DrawArrow"

            Select Case e.Tool.Key
                Case "DrawArc"
                    Me.vDraw.ActiveDocument.CommandAction.CmdArc(Nothing, Nothing, Nothing, Nothing)
                Case "DrawArrow", "DrawLine"
                    Me.vDraw.ActiveDocument.CommandAction.CmdLine(Nothing)
                Case "DrawCircle"
                    Me.vDraw.ActiveDocument.CommandAction.CmdCircle(Nothing, Nothing)
                Case "DrawDimension"
                    Me.vDraw.ActiveDocument.CommandAction.CmdDim(VectorDraw.Professional.Constants.VdConstDimType.dim_Aligned, Nothing, Nothing, Nothing)
                Case "DrawPolyline"
                    Me.vDraw.ActiveDocument.CommandAction.CmdPolyLine(Nothing)
                Case "DrawRectangle"
                    Me.vDraw.ActiveDocument.CommandAction.CmdRect(Nothing, Nothing)
                Case "DrawText"
                    Me.vDraw.ActiveDocument.CommandAction.CmdText(Nothing, Nothing, Nothing)
                Case "ShowGrid"
                    Me.vDraw.ActiveDocument.GridMode = DirectCast(e.Tool, StateButtonTool).Checked
                    Me.vDraw.ActiveDocument.Invalidate()
                Case "SnapToGrid"
                    Me.vDraw.ActiveDocument.SnapMode = DirectCast(e.Tool, StateButtonTool).Checked
                    Me.vDraw.ActiveDocument.Invalidate()
                Case "ZoomExtends"
                    Me.vDraw.ActiveDocument.CommandAction.Zoom("E", 0, 0)
                Case "ZoomWindow"
                    Me.vDraw.ActiveDocument.CommandAction.Zoom("W", "USER", "USER")
                Case "Export"
                    Export()
                Case "Print"
                    Print()
                Case "Save"
                    Save()
            End Select
        End If
    End Sub

    Private Sub vDraw_ActionEnd(sender As Object, actionName As String) Handles vDraw.ActionEnd
        If (_currentCommandAction IsNot Nothing) AndAlso (_currentCommandAction.StartsWith("Cmd")) Then _isDirty = True

        Me.lblDrawCommand.Visible = False
        Me.utmGraphicalRedlining.ToolTipDisplayStyle = ToolTipDisplayStyle.Standard
        Me.vDraw.SetCustomMousePointer(Nothing)
    End Sub

    Private Sub vDraw_ActionError(sender As Object, actionName As String) Handles vDraw.ActionError
        Me.lblDrawCommand.Visible = False
        Me.utmGraphicalRedlining.ToolTipDisplayStyle = ToolTipDisplayStyle.Standard
        Me.vDraw.SetCustomMousePointer(Nothing)
    End Sub

    Private Sub vDraw_ActionJobLoop(sender As Object, action As Object, ByRef cancel As Boolean) Handles vDraw.ActionJobLoop
        If (_currentCommandAction = "CmdLine") AndAlso (_currentCommandActionCount > 3) Then
            _currentCommandAction = String.Empty

            cancel = True
        End If
    End Sub

    Private Sub vDraw_ActionStart(sender As Object, actionName As String, ByRef cancel As Boolean) Handles vDraw.ActionStart
        If (actionName.StartsWith("Cmd")) OrElse (actionName = "Zoom") Then
            _currentCommandAction = actionName
            _currentCommandActionCount = 0
        End If

        Me.utmGraphicalRedlining.ToolTipDisplayStyle = ToolTipDisplayStyle.None

        Select Case actionName
            Case "BaseAction_ActionGetPoint"
                Me.vDraw.SetCustomMousePointer(_cursors("BasePoint"))

                Select Case _currentCommandAction
                    Case "CmdArc"
                        Me.lblDrawCommand.Text = GraphicalRedliningFormStrings.DefCPArc_Label
                    Case "CmdCircle"
                        Me.lblDrawCommand.Text = GraphicalRedliningFormStrings.DefCPCircle_Label
                    Case "CmdDim"
                        Me.lblDrawCommand.Text = GraphicalRedliningFormStrings.DefBPDim_Label
                    Case "CmdLine", "CmdPolyLine"
                        If (_isArrow) Then
                            Me.lblDrawCommand.Text = GraphicalRedliningFormStrings.DefSPArrow_Label
                        Else
                            Me.lblDrawCommand.Text = GraphicalRedliningFormStrings.DefSPLine_Label
                        End If
                    Case "CmdRect"
                        Me.lblDrawCommand.Text = GraphicalRedliningFormStrings.DefSCRect_Label
                    Case "CmdText"
                        Me.lblDrawCommand.Text = GraphicalRedliningFormStrings.DefBPText_Label
                    Case "Zoom"
                        Me.lblDrawCommand.Text = String.Empty
                        Me.vDraw.SetCustomMousePointer(Nothing)
                End Select
            Case "BaseAction_ActionGetRefPoint"
                Select Case _currentCommandAction
                    Case "CmdArc"
                        If (_currentCommandActionCount = 2) Then
                            Me.lblDrawCommand.Text = GraphicalRedliningFormStrings.DefRadArc_Label
                            Me.vDraw.SetCustomMousePointer(_cursors("Radius"))
                        Else
                            Me.lblDrawCommand.Text = GraphicalRedliningFormStrings.DefSAngArc_Label
                            Me.vDraw.SetCustomMousePointer(_cursors("StartAngle"))
                        End If
                    Case "CmdDim"
                        Me.lblDrawCommand.Text = GraphicalRedliningFormStrings.DefRPDim_Label
                        Me.vDraw.SetCustomMousePointer(_cursors("DimRefPoint"))
                    Case "CmdText"
                        Me.lblDrawCommand.Text = GraphicalRedliningFormStrings.DefRotText_Label
                        Me.vDraw.SetCustomMousePointer(_cursors("Rotation"))
                End Select
            Case "BaseAction_ActionArc"
                Me.lblDrawCommand.Text = GraphicalRedliningFormStrings.DefEAngArc_Label
                Me.vDraw.SetCustomMousePointer(_cursors("EndAngle"))
            Case "BaseAction_ActionCircle"
                Me.lblDrawCommand.Text = GraphicalRedliningFormStrings.DefRadCircle_Label
                Me.vDraw.SetCustomMousePointer(_cursors("Radius"))
            Case "BaseAction_ActionDimension"
                Me.lblDrawCommand.Text = GraphicalRedliningFormStrings.DefOffsetDim_Label
                Me.vDraw.SetCustomMousePointer(_cursors("DimOffset"))
            Case "BaseAction_ActionLine"
                If (_isArrow) Then
                    Me.lblDrawCommand.Text = GraphicalRedliningFormStrings.DefEPArrow_Label
                Else
                    Me.lblDrawCommand.Text = GraphicalRedliningFormStrings.DefEPLine_Label
                End If

                Me.vDraw.SetCustomMousePointer(_cursors("EndPoint"))
            Case "BaseAction_ActionPolyLine"
                Me.lblDrawCommand.Text = GraphicalRedliningFormStrings.DefNxtVertexPntPolyline_Label
                Me.vDraw.SetCustomMousePointer(_cursors("Vertex"))
            Case "BaseAction_ActionRect"
                Me.lblDrawCommand.Text = GraphicalRedliningFormStrings.DefECRect_Label
                Me.vDraw.SetCustomMousePointer(_cursors("EndPoint"))
            Case "BaseAction_ActionText"
                Me.lblDrawCommand.Text = GraphicalRedliningFormStrings.DefTxtString_Label
        End Select

        _currentCommandActionCount += 1

        If (actionName <> "BaseAction_ActionGetRectFromPointSelectDCS") AndAlso (actionName <> "BaseAction_ActionManager") AndAlso (actionName <> "BaseAction_ActionPan") AndAlso (actionName <> "CmdPrintDialog") Then Me.lblDrawCommand.Visible = True
    End Sub

    Private Sub vDraw_AddItem(obj As Object, ByRef Cancel As Boolean) Handles vDraw.AddItem
        If (_isArrow) AndAlso (TypeOf obj Is vdLine) Then
            Cancel = True

            Dim line As vdLine = DirectCast(obj, vdLine)
            Dim lineAngle As Double = line.StartPoint.GetAngle(line.EndPoint)
            Dim lineDistance As Double = line.StartPoint.Distance2D(line.EndPoint)

            Dim pointAtLine As gPoint = line.getPointAtDist(lineDistance - (lineDistance / 6))
            Dim firstArrowPoint As gPoint = line.getPointAtDist(lineDistance - (lineDistance / 9))
            Dim secondArrowPoint As gPoint = line.getPointAtDist(lineDistance - (lineDistance / 9))

            Dim matrix As New Matrix
            matrix.TranslateMatrix(-1D * pointAtLine)
            matrix.RotateZMatrix(Globals.DegreesToRadians(90D))
            matrix.TranslateMatrix(pointAtLine)

            firstArrowPoint = matrix.Transform(firstArrowPoint)

            matrix = New Matrix
            matrix.TranslateMatrix(-1D * pointAtLine)
            matrix.RotateZMatrix(-Globals.DegreesToRadians(90D))
            matrix.TranslateMatrix(pointAtLine)

            secondArrowPoint = matrix.Transform(secondArrowPoint)

            Dim polyline As New vdPolyline
            With polyline
                .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                .setDocumentDefaults()

                .Flag = VectorDraw.Professional.Constants.VdConstPlineFlag.PlFlagCLOSE

                Dim hatchProperties As New vdHatchProperties(VectorDraw.Professional.Constants.VdConstFill.VdFillModeSolid)
                hatchProperties.SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                hatchProperties.FillColor = Me.vDraw.ActiveDocument.ActivePenColor

                .HatchProperties = hatchProperties
                .Label = "Arrow"

                With .VertexList
                    .Add(line.StartPoint)
                    .Add(line.EndPoint)
                    .Add(firstArrowPoint)
                    .Add(secondArrowPoint)
                    .Add(line.EndPoint)
                End With
            End With

            Me.vDraw.ActiveDocument.ActiveLayOut.Entities.AddItem(polyline)
            Me.vDraw.ActiveDocument.Invalidate()
        ElseIf (TypeOf obj Is vdDimension) Then
            DirectCast(obj, vdDimension).TextStyle = DirectCast(Me.vDraw.ActiveDocument.TextStyles.FindName("STANDARD").Clone(Me.vDraw.ActiveDocument), vdTextstyle)
        ElseIf (TypeOf obj Is vdText) Then
            DirectCast(obj, vdText).Style = DirectCast(Me.vDraw.ActiveDocument.TextStyles.FindName("STANDARD").Clone(Me.vDraw.ActiveDocument), vdTextstyle)
        End If
    End Sub

    Private Sub vDraw_KeyDown(sender As Object, e As KeyEventArgs) Handles vDraw.KeyDown
        If (e.Control) Then
            If (e.KeyCode = Keys.A) Then
                For Each figure As vdFigure In Me.vDraw.ActiveDocument.ActiveLayOut.Entities
                    If (Not figure.Layer.Lock) Then AddFigureToSelection(figure)
                Next
            ElseIf (e.KeyCode = Keys.Add) OrElse (e.KeyCode = Keys.Oemplus) Then
                Me.vDraw.ActiveDocument.CommandAction.Zoom("S", 1.25, 0)
            ElseIf (e.KeyCode = Keys.E) Then
                Me.vDraw.ActiveDocument.CommandAction.Zoom("E", 0, 0)
            ElseIf (e.KeyCode = Keys.H) Then
                Me.vDraw.ActiveDocument.CommandAction.Pan()
            ElseIf (e.KeyCode = Keys.Subtract) OrElse (e.KeyCode = Keys.OemMinus) Then
                Me.vDraw.ActiveDocument.CommandAction.Zoom("S", (1 / 1.25), 0)
            ElseIf (e.KeyCode = Keys.W) Then
                Me.vDraw.ActiveDocument.CommandAction.Zoom("W", "USER", "USER")
            End If
        ElseIf (e.KeyCode = Keys.Delete) Then
            For Each selFigure As vdFigure In _selection
                _isDirty = True

                Me.vDraw.ActiveDocument.ActiveLayOut.Entities.RemoveItem(selFigure)
            Next

            _selection.RemoveAll()

            Me.upnTextEdit.Visible = False
            Me.vDraw.ActiveDocument.Invalidate()
        ElseIf (e.KeyCode = Keys.Escape) Then
            Me.vDraw.ActiveDocument.CommandAction.Cancel()

            DeselectAll()
        End If
    End Sub

    Private Sub vDraw_MouseClick(sender As Object, e As MouseEventArgs) Handles vDraw.MouseClick
        If (Me.vDraw.ActiveDocument.CommandAction.OpenLoops = 0) Then
            If (e.Button = System.Windows.Forms.MouseButtons.Left) AndAlso (_clickPoint IsNot Nothing) Then
                If (Not E3.Lib.DotNet.Expansions.Devices.My.Computer.Keyboard.CtrlKeyDown) OrElse (e.Button = System.Windows.Forms.MouseButtons.Right) Then DeselectAll()

                _hitEntity = Me.vDraw.ActiveDocument.ActiveLayOut.GetEntityFromPoint(Me.vDraw.ActiveDocument.ActiveRender.View2PixelI(_clickPoint), 6, False, vdDocument.LockLayerMethodEnum.DisableAll)

                If (_hitEntity IsNot Nothing) Then AddFigureToSelection()
            ElseIf (e.Button = System.Windows.Forms.MouseButtons.Right) Then
                If (_hoveredEntity IsNot Nothing) AndAlso (_selection.FindItem(_hoveredEntity)) Then
                    _hoveredEntity.ShowGrips = True
                    _hoveredEntity.Update()

                    Me.vDraw.ActiveDocument.Invalidate()
                Else
                    DeselectAll()
                End If
            End If
        End If
    End Sub

    Private Sub vDraw_MouseDown(sender As Object, e As MouseEventArgs) Handles vDraw.MouseDown
        If (Me.vDraw.ActiveDocument.CommandAction.OpenLoops = 0) AndAlso (e.Button = System.Windows.Forms.MouseButtons.Left) Then
            _clickPoint = GetGridSnapPoint(Me.vDraw.ActiveDocument.CCS_CursorPos)

            For Each selFigure As vdFigure In _selection
                If (selFigure.ShowGrips) Then
                    For gripPointIndex As Integer = 0 To selFigure.GetGripPoints.Count - 1
                        Dim gripPoint As gPoint = GetGridSnapPoint(selFigure.GetGripPoints(gripPointIndex))

                        If (Math.Round(gripPoint.x, 0) = Math.Round(_clickPoint.x, 0)) AndAlso (Math.Round(gripPoint.y, 0) = Math.Round(_clickPoint.y, 0)) Then
                            _selectedGripPointIndex = gripPointIndex

                            Exit For
                        End If
                    Next
                End If
            Next
        Else
            _clickPoint = Nothing
            _movePoint = Nothing
        End If
    End Sub

    Private Sub vDraw_MouseMove(sender As Object, e As MouseEventArgs) Handles vDraw.MouseMove
        If (Me.vDraw.ActiveDocument.CommandAction.OpenLoops = 0) Then
            _hoveredEntity = Me.vDraw.ActiveDocument.ActiveLayOut.GetEntityFromPoint(Me.vDraw.ActiveDocument.ActiveRender.View2PixelI(GetGridSnapPoint(Me.vDraw.ActiveDocument.CCS_CursorPos)), 6, False, vdDocument.LockLayerMethodEnum.DisableAll)
            If (_hoveredEntity IsNot Nothing) AndAlso (_movePoint Is Nothing) Then
                If (_selection.FindItem(_hoveredEntity)) Then
                    If (Me.vDraw.GetCustomMousePointer <> Cursors.SizeAll) Then
                        Me.vDraw.SetCustomMousePointer(Cursors.SizeAll)
                        Me.vDraw.ActiveDocument.Invalidate()
                    End If
                Else
                    If (Me.vDraw.GetCustomMousePointer <> Cursors.Hand) Then
                        Me.vDraw.SetCustomMousePointer(Cursors.Hand)
                        Me.vDraw.ActiveDocument.Invalidate()
                    End If
                End If
            ElseIf (Me.vDraw.GetCustomMousePointer IsNot Nothing) AndAlso (_movePoint Is Nothing) Then
                Me.vDraw.SetCustomMousePointer(Nothing)
                Me.vDraw.ActiveDocument.Invalidate()
            End If

            If (e.Button = System.Windows.Forms.MouseButtons.Left) AndAlso (Not E3.Lib.DotNet.Expansions.Devices.My.Computer.Keyboard.CtrlKeyDown) Then
                Dim currentCursorPos As gPoint = GetGridSnapPoint(Me.vDraw.ActiveDocument.ActiveRender.Pixel2View(New POINT(e.X, e.Y)))

                If (_selectedGripPointIndex <> Integer.MaxValue) Then
                    Dim figureWithGrips As vdFigure = Nothing

                    _isDirty = True

                    For Each selFigure As vdFigure In _selection
                        If (selFigure.ShowGrips) Then
                            figureWithGrips = selFigure

                            Exit For
                        End If
                    Next

                    If (figureWithGrips IsNot Nothing) Then
                        If (TypeOf figureWithGrips Is vdArc) Then
                            Select Case _selectedGripPointIndex
                                Case 0
                                    DirectCast(figureWithGrips, vdArc).StartAngle = DirectCast(figureWithGrips, vdArc).Center.GetAngle(currentCursorPos)
                                Case 1
                                    DirectCast(figureWithGrips, vdArc).EndAngle = DirectCast(figureWithGrips, vdArc).Center.GetAngle(currentCursorPos)
                                Case 2
                                    DirectCast(figureWithGrips, vdArc).Radius = DirectCast(figureWithGrips, vdArc).Center.Distance2D(currentCursorPos)
                                Case 3
                                    DirectCast(figureWithGrips, vdArc).Center = currentCursorPos
                            End Select
                        ElseIf (TypeOf figureWithGrips Is vdCircle) Then
                            Select Case _selectedGripPointIndex
                                Case 0
                                    DirectCast(figureWithGrips, vdCircle).Center = currentCursorPos
                                Case Else
                                    DirectCast(figureWithGrips, vdCircle).Radius = DirectCast(figureWithGrips, vdCircle).Center.Distance2D(currentCursorPos)
                            End Select
                        ElseIf (TypeOf figureWithGrips Is vdDimension) Then
                            Select Case _selectedGripPointIndex
                                Case 0, 1
                                    DirectCast(figureWithGrips, vdDimension).LinePosition = currentCursorPos
                                Case 2
                                    DirectCast(figureWithGrips, vdDimension).DefPoint1 = currentCursorPos
                                Case 3
                                    DirectCast(figureWithGrips, vdDimension).DefPoint2 = currentCursorPos
                            End Select
                        ElseIf (TypeOf figureWithGrips Is vdInsert) Then
                            DirectCast(figureWithGrips, vdInsert).InsertionPoint = currentCursorPos
                        ElseIf (TypeOf figureWithGrips Is vdLine) Then
                            Select Case _selectedGripPointIndex
                                Case 0
                                    DirectCast(figureWithGrips, vdLine).StartPoint = currentCursorPos
                                Case 1
                                    DirectCast(figureWithGrips, vdLine).EndPoint = currentCursorPos
                            End Select
                        ElseIf (TypeOf figureWithGrips Is vdPolyline) Then
                            DirectCast(figureWithGrips, vdPolyline).VertexList(_selectedGripPointIndex) = New Vertex(currentCursorPos)
                        ElseIf (TypeOf figureWithGrips Is vdRect) Then
                            Select Case _selectedGripPointIndex
                                Case 0
                                    DirectCast(figureWithGrips, vdRect).InsertionPoint = currentCursorPos
                                Case 1
                                    DirectCast(figureWithGrips, vdRect).Height = -DirectCast(figureWithGrips, vdRect).InsertionPoint.y + currentCursorPos.y
                                    DirectCast(figureWithGrips, vdRect).Width = -DirectCast(figureWithGrips, vdRect).InsertionPoint.x + currentCursorPos.x
                            End Select
                        ElseIf (TypeOf figureWithGrips Is vdText) Then
                            DirectCast(figureWithGrips, vdText).InsertionPoint = currentCursorPos
                        End If

                        figureWithGrips.Update()

                        Me.vDraw.ActiveDocument.Invalidate()
                    End If
                ElseIf (_clickPoint IsNot Nothing) AndAlso (_hitEntity Is Nothing) AndAlso (_hoveredEntity Is Nothing) AndAlso (_movePoint Is Nothing) Then
                    Dim selection As New vdSelection

                    If (Me.vDraw.ActiveDocument.ActionUtility.getUserStartWindowSelection(_clickPoint, selection) = StatusCode.Success) Then
                        DeselectAll()

                        For Each figure As vdFigure In selection
                            AddFigureToSelection(figure)
                        Next
                    End If

                    If (selection IsNot Nothing) Then selection.Dispose()

                    _clickPoint = Nothing
                ElseIf (_hoveredEntity IsNot Nothing AndAlso _selection.FindItem(_hoveredEntity)) OrElse (_movePoint IsNot Nothing) Then
                    If (_movePoint Is Nothing) Then
                        _isDirty = True
                        _movePoint = _clickPoint

                        Me.vDraw.ActiveDocument.FreezeActions = True
                    End If

                    Me.vDraw.ActiveDocument.CommandAction.CmdMove(_selection, _movePoint, currentCursorPos)

                    _movePoint = currentCursorPos
                End If
            End If
        End If

        Me.lblCoordinates.Text = String.Format("X: {0}, Y: {1}", Math.Round(Me.vDraw.ActiveDocument.CCS_CursorPos.x, 2), Math.Round(Me.vDraw.ActiveDocument.CCS_CursorPos.y, 2))
    End Sub

    Private Sub vDraw_MouseUp(sender As Object, e As MouseEventArgs) Handles vDraw.MouseUp
        If (_movePoint IsNot Nothing) Then _movePoint = Nothing

        Me.vDraw.ActiveDocument.FreezeActions = False

        _selectedGripPointIndex = Integer.MaxValue
    End Sub


    Friend ReadOnly Property DocumentStreamString() As String
        Get
            Return _documentStreamString
        End Get
    End Property

End Class