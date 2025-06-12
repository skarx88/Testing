Imports System.IO
Imports System.Text.RegularExpressions
Imports Infragistics.Win.UltraWinToolbars
Imports VectorDraw.Actions
Imports VectorDraw.Geometry
Imports VectorDraw.Professional.Constants
Imports VectorDraw.Professional.vdCollections
Imports VectorDraw.Professional.vdFigures
Imports VectorDraw.Professional.vdObjects
Imports VectorDraw.Professional.vdPrimaries

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class TopologyEditorForm

    Private _addVertexToPolygon As Boolean
    Private _clickPoint As gPoint
    Private _currentCommandAction As String
    Private _currentTopologyViewFile As String
    Private _cursors As New Dictionary(Of String, Cursor)
    Private _harnessPartNumbers As List(Of String)
    Private _hitEntity As vdFigure
    Private _hoveredEntity As vdFigure
    Private _isDirty As Boolean
    Private _movePoint As gPoint
    Private _selectedGripPointIndex As Integer
    Private _selection As vdSelection

    Public Sub New()
        InitializeComponent()

        Me.BackColor = Color.White
        Me.Icon = My.Resources.TopologyEditor
        Me.Text = TopologyEditorFormStrings.Caption

        _harnessPartNumbers = New List(Of String)

        InitializeCursors()
        InitializeToolbar()
        InitializeVectorDrawBaseControl()
    End Sub

    Private Sub AddCompartment(obj As vdFigure)
        If (TypeOf obj Is vdPolyline) AndAlso (DirectCast(obj, vdPolyline).VertexList.Count <= 2) Then
            Me.vDraw.ActiveDocument.ActiveLayOut.Entities.RemoveItem(obj)

            Exit Sub
        End If

        Using compartmentForm As New CompartmentForm(_harnessPartNumbers, String.Empty, String.Empty)
            If (compartmentForm.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then
                If (TypeOf obj Is vdPolyline) Then
                    With DirectCast(obj, vdPolyline)
                        .Flag = VdConstPlineFlag.PlFlagCLOSE

                        Dim hatchProperties As New vdHatchProperties(VdConstFill.VdFillModeSolid)
                        hatchProperties.SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                        hatchProperties.FillColor.SystemColor = Color.LightBlue
                        hatchProperties.FillColor.AlphaBlending = 128

                        .HatchProperties = hatchProperties
                        .PenColor.SystemColor = Color.LightBlue
                        .PenWidth = 2
                        .XProperties.Add("HarnessPartNumber").PropValue = compartmentForm.txtHarnessPartNumber.Text
                        .XProperties.Add("HarnessDescription").PropValue = compartmentForm.txtHarnessDescription.Text
                        .XProperties.Add("PenColor")
                        .XProperties.Add("RefFigureId")
                    End With
                Else
                    With DirectCast(obj, vdRect)
                        Dim hatchProperties As New vdHatchProperties(VdConstFill.VdFillModeSolid)
                        hatchProperties.SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                        hatchProperties.FillColor.SystemColor = Color.LightBlue
                        hatchProperties.FillColor.AlphaBlending = 128

                        .HatchProperties = hatchProperties
                        .PenColor.SystemColor = Color.LightBlue
                        .PenWidth = 2
                        .XProperties.Add("HarnessPartNumber").PropValue = compartmentForm.txtHarnessPartNumber.Text
                        .XProperties.Add("HarnessDescription").PropValue = compartmentForm.txtHarnessDescription.Text
                        .XProperties.Add("PenColor")
                        .XProperties.Add("RefFigureId")
                    End With
                End If

                obj.Update()

                Dim text As New vdText
                With text
                    .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                    .setDocumentDefaults()

                    .Bold = True
                    .Height = 4
                    .HorJustify = VdConstHorJust.VdTextHorCenter
                    .InsertionPoint = DirectCast(obj, vdFigure).BoundingBox.MidPoint
                    .PenColor.SystemColor = Color.Black

                    If (Math.Abs(DirectCast(obj, vdFigure).BoundingBox.Height) > Math.Abs(DirectCast(obj, vdFigure).BoundingBox.Width)) Then .Rotation = Math.PI / 2

                    If (compartmentForm.txtHarnessDescription.Text <> String.Empty) Then
                        .TextString = compartmentForm.txtHarnessDescription.Text
                    Else
                        .TextString = String.Format("[{0}]", compartmentForm.txtHarnessPartNumber.Text)
                    End If

                    .VerJustify = VdConstVerJust.VdTextVerCen

                    .XProperties.Add("PenColor")
                    .XProperties.Add("RefFigureId").PropValue = DirectCast(obj, vdFigure).HandleId
                End With

                Me.vDraw.ActiveDocument.ActiveLayOut.Entities.AddItem(text)

                DirectCast(obj, vdFigure).XProperties.FindName("RefFigureId").PropValue = text.HandleId

                _harnessPartNumbers.Add(compartmentForm.txtHarnessPartNumber.Text)
            Else
                Me.vDraw.ActiveDocument.ActiveLayOut.Entities.RemoveItem(obj)
            End If

            Me.vDraw.ActiveDocument.Invalidate()
        End Using
    End Sub

    Private Sub AddFigureToSelection(Optional figure As vdFigure = Nothing)
        Dim selFigure As vdFigure = figure

        If (selFigure Is Nothing) Then selFigure = _hitEntity
        If (selFigure IsNot Nothing) Then
            If (Not _selection.FindItem(selFigure)) Then
                selFigure.XProperties.FindName("PenColor").PropValue = selFigure.PenColor.SystemColor
                selFigure.PenColor.SystemColor = Color.Magenta

                _selection.AddItem(selFigure, True, vdSelection.AddItemCheck.Nochecking)

                If (selFigure.XProperties.FindName("RefFigureId") IsNot Nothing) AndAlso (TypeOf selFigure Is vdPolyline OrElse TypeOf selFigure Is vdRect) Then
                    Dim fig As vdFigure = FindFromId(CInt(selFigure.XProperties.FindName("RefFigureId").PropValue))
                    If (fig IsNot Nothing) AndAlso (Not fig.Equals(selFigure)) AndAlso (Not _selection.FindItem(fig)) Then
                        fig.XProperties.FindName("PenColor").PropValue = fig.PenColor.SystemColor
                        fig.PenColor.SystemColor = Color.Magenta

                        _selection.AddItem(fig, True, vdSelection.AddItemCheck.Nochecking)

                        fig.Update()
                        fig.Invalidate()
                    End If
                End If
            ElseIf (E3.Lib.DotNet.Expansions.Devices.My.Computer.Keyboard.CtrlKeyDown) AndAlso (_selection.FindItem(selFigure)) Then
                selFigure.PenColor.SystemColor = DirectCast(selFigure.XProperties.FindName("PenColor").PropValue, Color)
                selFigure.ShowGrips = False

                _selection.RemoveItem(selFigure)

                If (selFigure.XProperties.FindName("RefFigureId") IsNot Nothing) AndAlso (TypeOf selFigure Is vdPolyline OrElse TypeOf selFigure Is vdRect) Then
                    Dim fig As vdFigure = FindFromId(CInt(selFigure.XProperties.FindName("RefFigureId").PropValue))
                    If (fig IsNot Nothing) AndAlso (Not fig.Equals(selFigure)) AndAlso (_selection.FindItem(fig)) Then
                        fig.PenColor.SystemColor = DirectCast(fig.XProperties.FindName("PenColor").PropValue, Color)

                        _selection.RemoveItem(fig)

                        fig.Update()
                        fig.Invalidate()
                    End If
                End If
            End If

            selFigure.Update()
            selFigure.Invalidate()
        End If
    End Sub

    Private Sub DeselectAll()
        If (_selection IsNot Nothing) AndAlso (_selection.Count <> 0) Then
            For Each selFigure As vdFigure In _selection
                If (selFigure.XProperties.FindName("PenColor") IsNot Nothing) Then selFigure.PenColor.SystemColor = DirectCast(selFigure.XProperties.FindName("PenColor").PropValue, Color)

                selFigure.ShowGrips = False
                selFigure.Update()
            Next

            _selection.RemoveAll()
        End If

        Me.vDraw.ActiveDocument.Invalidate()
    End Sub

    Private Sub Export()
        With Me.sfdTopologyEditor
            .DefaultExt = KnownFile.DXF.Trim("."c)

            If (_currentTopologyViewFile IsNot Nothing) Then
                .FileName = String.Format("Car_topology_{0}", Regex.Replace(_currentTopologyViewFile, "\W", "_"))
            Else
                .FileName = "Car_topology"
            End If

            .Filter = "Autodesk DXF file (*.dxf)|*.dxf|JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|Bitmap (*.bmp)|*.bmp"
            .Title = TopologyEditorFormStrings.ExportTopoToFile_Title

            If (.ShowDialog(Me) = DialogResult.OK) Then
                Try
                    If (VdOpenSave.SaveAs(Me.vDraw.ActiveDocument, Me.sfdTopologyEditor.FileName)) Then
                        MessageBox.Show(TopologyEditorFormStrings.ExportTopoSuccess_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Else
                        MessageBox.Show(TopologyEditorFormStrings.ProblemExportTopo_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                Catch ex As Exception
                    MessageBox.Show(String.Format(TopologyEditorFormStrings.ErrorExportTopo_Msg, vbCrLf, ex.Message), [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        End With
    End Sub

    Private Function FindFromId(id As Integer) As vdFigure
        For Each figure As vdFigure In Me.vDraw.ActiveDocument.ActiveLayOut.Entities
            If (figure.HandleId = id) Then Return figure
        Next

        Return Nothing
    End Function

    Private Function GetGridSnapPoint(point As gPoint) As gPoint
        Dim snapX As Double = Me.vDraw.ActiveDocument.SnapSpaceX
        Dim snapY As Double = Me.vDraw.ActiveDocument.SnapSpaceY

        If (Me.vDraw.ActiveDocument.SnapMode) AndAlso (snapX <> 0.1 AndAlso snapY <> 0.1) Then
            Return New gPoint(CLng(point.x / snapX) * snapX, CLng(point.y / snapY) * snapY, 0)
        Else
            Return point
        End If
    End Function

    Private Sub HandleMouseMoveWhileLeftButtonDown(ByVal e As MouseEventArgs)
        Dim currentCursorPos As gPoint = GetGridSnapPoint(Me.vDraw.ActiveDocument.ActiveRender.Pixel2View(New POINT(e.X, e.Y)))

        If (_selectedGripPointIndex <> Integer.MaxValue) Then
            Dim figureWithGrips As vdFigure = Nothing

            For Each selFigure As vdFigure In _selection
                If (selFigure.ShowGrips) Then
                    figureWithGrips = selFigure

                    Exit For
                End If
            Next

            If (figureWithGrips IsNot Nothing) Then
                _isDirty = True

                If (TypeOf figureWithGrips Is vdPolyline) Then
                    DirectCast(figureWithGrips, vdPolyline).VertexList(_selectedGripPointIndex).x = currentCursorPos.x
                    DirectCast(figureWithGrips, vdPolyline).VertexList(_selectedGripPointIndex).y = currentCursorPos.y
                ElseIf (TypeOf figureWithGrips Is vdRect) Then
                    Select Case _selectedGripPointIndex
                        Case 0
                            DirectCast(figureWithGrips, vdRect).InsertionPoint = currentCursorPos
                        Case 1
                            DirectCast(figureWithGrips, vdRect).Height = -DirectCast(figureWithGrips, vdRect).InsertionPoint.y + currentCursorPos.y
                            DirectCast(figureWithGrips, vdRect).Width = -DirectCast(figureWithGrips, vdRect).InsertionPoint.x + currentCursorPos.x
                    End Select
                End If

                Dim text As vdText = DirectCast(FindFromId(CInt(figureWithGrips.XProperties.FindName("RefFigureId").PropValue)), vdText)
                text.InsertionPoint = DirectCast(figureWithGrips, vdFigure).BoundingBox.MidPoint

                If (Math.Abs(DirectCast(figureWithGrips, vdFigure).BoundingBox.Height) > Math.Abs(DirectCast(figureWithGrips, vdFigure).BoundingBox.Width)) Then
                    text.Rotation = Math.PI / 2
                Else
                    text.Rotation = 0
                End If

                text.Update()

                figureWithGrips.Update()

                Me.vDraw.ActiveDocument.Invalidate()
            End If
        ElseIf (_hoveredEntity IsNot Nothing) AndAlso (TypeOf _hoveredEntity Is vdPolyline) AndAlso (_hoveredEntity.ShowGrips) AndAlso (_addVertexToPolygon) Then
            Dim distToLine As Double = DirectCast(_hoveredEntity, vdPolyline).getClosestPointTo(currentCursorPos).Distance2D(currentCursorPos)

            If (distToLine <= 4) Then
                Dim segmentIndex As Integer = DirectCast(_hoveredEntity, vdPolyline).SegmentIndexFromPoint(currentCursorPos, 0.1)

                DirectCast(_hoveredEntity, vdPolyline).InsertVertex(segmentIndex, New Vertex(currentCursorPos))
                DirectCast(_hoveredEntity, vdPolyline).Update()

                _selectedGripPointIndex = segmentIndex + 1
            End If

            _addVertexToPolygon = False
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
            End If

            Dim deltaX As Double = _movePoint.x - currentCursorPos.x
            Dim deltaY As Double = _movePoint.y - currentCursorPos.y

            For Each selFigure As vdFigure In _selection
                If (TypeOf selFigure Is vdPolyline) Then
                    Dim oldVertexList As Vertexes = DirectCast(DirectCast(selFigure, vdPolyline).VertexList.Clone(), Vertexes)

                    DirectCast(selFigure, vdPolyline).VertexList.RemoveAll()

                    For Each vertex As gPoint In oldVertexList
                        DirectCast(selFigure, vdPolyline).VertexList.Add(New gPoint(vertex.x - deltaX, vertex.y - deltaY))
                    Next
                ElseIf (TypeOf selFigure Is vdRect) Then
                    DirectCast(selFigure, vdRect).InsertionPoint = New gPoint(DirectCast(selFigure, vdRect).InsertionPoint.x - deltaX, DirectCast(selFigure, vdRect).InsertionPoint.y - deltaY)
                ElseIf (TypeOf selFigure Is vdText) Then
                    DirectCast(selFigure, vdText).InsertionPoint = New gPoint(DirectCast(selFigure, vdText).InsertionPoint.x - deltaX, DirectCast(selFigure, vdText).InsertionPoint.y - deltaY)
                End If

                selFigure.Update()
            Next

            Me.vDraw.ActiveDocument.Invalidate()

            _movePoint = currentCursorPos
        End If
    End Sub

    Private Sub InitializeCursors()
        Dim cursorResources As New Dictionary(Of String, Byte())
        cursorResources.Add("BasePoint", My.Resources.BasePoint)
        cursorResources.Add("EndPoint", My.Resources.EndPoint)
        cursorResources.Add("Vertex", My.Resources.Vertex)

        _cursors = New Dictionary(Of String, Cursor)

        For Each cursorResource As KeyValuePair(Of String, Byte()) In cursorResources
            Using cursorStream As New IO.MemoryStream(cursorResource.Value)
                _cursors.Add(cursorResource.Key, New Cursor(cursorStream))

                cursorStream.Close()
            End Using
        Next
    End Sub

    Private Sub InitializeToolbar()
        Me.utmTopologyEditor.Style = ToolbarStyle.Office2010

        Dim mainToolbar As UltraToolbar = Me.utmTopologyEditor.Toolbars.AddToolbar("MainToolbar")
        mainToolbar.IsMainMenuBar = True

        With Me.utmTopologyEditor
            Dim backgroundPictureButton As New ButtonTool("BackgroundPicture")
            With backgroundPictureButton.SharedProps
                .AppearancesSmall.Appearance.Image = My.Resources.BackgroundPicture.ToBitmap
                .Caption = TopologyEditorFormStrings.BackPic_MnuBtn_Caption
            End With

            Dim exportButton As New ButtonTool("Export")
            With exportButton.SharedProps
                .AppearancesSmall.Appearance.Image = My.Resources.ExportGraphic.ToBitmap
                .Caption = TopologyEditorFormStrings.Export_MnuBtn_Caption
            End With

            Dim openButton As New ButtonTool("Open")
            With openButton.SharedProps
                .AppearancesSmall.Appearance.Image = My.Resources.OpenDocument.ToBitmap
                .Caption = TopologyEditorFormStrings.Open_MnuBtn_Caption
            End With

            Dim placePolygonCompartmentButton As New ButtonTool("PlacePolygonCompartment")
            With placePolygonCompartmentButton.SharedProps
                .AppearancesSmall.Appearance.Image = My.Resources.PlacePolygonCompartment.ToBitmap
                .Caption = TopologyEditorFormStrings.PlacePolyComp_MnuBtn_Caption
            End With

            Dim placeRectCompartmentButton As New ButtonTool("PlaceRectCompartment")
            With placeRectCompartmentButton.SharedProps
                .AppearancesSmall.Appearance.Image = My.Resources.PlaceRectCompartment.ToBitmap
                .Caption = TopologyEditorFormStrings.PlaceRectComp_MnuBtn_Caption
            End With

            Dim printButton As New ButtonTool("Print")
            With printButton.SharedProps
                .AppearancesSmall.Appearance.Image = My.Resources.Print.ToBitmap
                .Caption = TopologyEditorFormStrings.Print_MnuBtn_Caption
            End With

            Dim saveButton As New ButtonTool("Save")
            With saveButton.SharedProps
                .AppearancesSmall.Appearance.Image = My.Resources.SaveDocument.ToBitmap
                .Caption = TopologyEditorFormStrings.Save_MnuBtn_Caption
            End With

            Dim saveAsButton As New ButtonTool("SaveAs")
            With saveAsButton.SharedProps
                .AppearancesSmall.Appearance.Image = My.Resources.SaveDocumentAs.ToBitmap
                .Caption = TopologyEditorFormStrings.SaveAs_MnuBtn_Caption
            End With

            Dim showGridButton As New StateButtonTool("ShowGrid")
            With showGridButton.SharedProps
                .AppearancesSmall.Appearance.Image = My.Resources.ShowGrid.ToBitmap
                .Caption = TopologyEditorFormStrings.ShowGrid_MnuBtn_Caption
            End With

            showGridButton.Checked = True

            Dim snapToGridButton As New StateButtonTool("SnapToGrid")
            With snapToGridButton.SharedProps
                .AppearancesSmall.Appearance.Image = My.Resources.SnapToGrid.ToBitmap
                .Caption = TopologyEditorFormStrings.SnapGrid_MnuBtn_Caption
            End With

            snapToGridButton.Checked = True

            Dim zoomExtendsButton As New ButtonTool("ZoomExtends")
            With zoomExtendsButton.SharedProps
                .AppearancesSmall.Appearance.Image = My.Resources.ZoomExtends.ToBitmap
                .Caption = TopologyEditorFormStrings.ZoomExt_MnuBtn_Caption
            End With

            Dim zoomWindow As New ButtonTool("ZoomWindow")
            With zoomWindow.SharedProps
                .AppearancesSmall.Appearance.Image = My.Resources.ZoomWindow.ToBitmap
                .Caption = TopologyEditorFormStrings.ZoomWndw_MnuBtn_Caption
            End With

            .Tools.Add(backgroundPictureButton)
            .Tools.Add(exportButton)
            .Tools.Add(openButton)
            .Tools.Add(placePolygonCompartmentButton)
            .Tools.Add(placeRectCompartmentButton)
            .Tools.Add(printButton)
            .Tools.Add(saveButton)
            .Tools.Add(saveAsButton)
            .Tools.Add(showGridButton)
            .Tools.Add(snapToGridButton)
            .Tools.Add(zoomExtendsButton)
            .Tools.Add(zoomWindow)
        End With

        mainToolbar.Tools.AddTool("Open")
        mainToolbar.Tools.AddTool("Save")
        mainToolbar.Tools.AddTool("SaveAs")
        mainToolbar.Tools.AddTool("Export")
        mainToolbar.Tools.AddTool("Print")
        mainToolbar.Tools.AddTool("BackgroundPicture")
        mainToolbar.Tools("BackgroundPicture").InstanceProps.IsFirstInGroup = True
        mainToolbar.Tools.AddTool("PlacePolygonCompartment")
        mainToolbar.Tools.AddTool("PlaceRectCompartment")
        mainToolbar.Tools.AddTool("ShowGrid")
        mainToolbar.Tools("ShowGrid").InstanceProps.IsFirstInGroup = True
        mainToolbar.Tools.AddTool("SnapToGrid")
        mainToolbar.Tools.AddTool("ZoomExtends")
        mainToolbar.Tools("ZoomExtends").InstanceProps.IsFirstInGroup = True
        mainToolbar.Tools.AddTool("ZoomWindow")
    End Sub

    Private Sub InitializeVectorDrawBaseControl()
        Me.vDraw.AllowDrop = False
        Me.vDraw.EnsureDocument()
        Me.vDraw.InputDevice_TouchGesture.Actions = VectorDraw.Professional.Actions.TouchGestureActions.Pan Or VectorDraw.Professional.Actions.TouchGestureActions.Zoom
        With Me.vDraw.ActiveDocument
            .UndoHistory.PushEnable(False)

            .ActiveActionRender.PenStyle.color = Color.Magenta
            .ActiveActionRender.PenStyle.SetStdWidth(2)

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
            .Limits = New Box(New gPoint(0, 0), New gPoint(1000, 606))
            .OrbitActionKey = vdDocument.KeyWithMouseStroke.None
            .OsnapDialogKey = Keys.None
            .Palette.Background = Color.White
            .ShowUCSAxis = False
            .SnapMode = True
            .SnapSpaceX = .GridSpaceX
            .SnapSpaceY = .GridSpaceY
            .UrlActionKey = Keys.None

            Me.vDraw.ActiveDocument.ZoomExtents()
            .EnableToolTips = False
            .Selections.Add(New vdSelection)
        End With

        _selection = Me.vDraw.ActiveDocument.Selections(0)
    End Sub

    Private Sub LoadBackgroundPicture()
        With Me.ofdTopologyEditor
            .DefaultExt = KnownFile.DWG.Trim("."c)
            .Filter = "Autodesk DWG file (*.dwg)|*.dwg|Autodesk DXF file (*.dxf)|*.dxf|JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|Bitmap (*.bmp)|*.bmp"
            .Title = TopologyEditorFormStrings.LoadBackPicFile_Title

            If (.ShowDialog(Me) = DialogResult.OK) Then
                If (Me.vDraw.ActiveDocument.Layers.FindName("Background") Is Nothing) Then
                    Dim layer As New vdLayer
                    With layer
                        .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                        .setDocumentDefaults()

                        .Name = "Background"
                    End With

                    Me.vDraw.ActiveDocument.Layers.Add(layer)
                Else
                    Me.vDraw.ActiveDocument.Layers.FindName("Background").Lock = False

                    Dim figuresForDeletion As New List(Of vdFigure)

                    For Each figure As vdFigure In Me.vDraw.ActiveDocument.ActiveLayOut.Entities
                        If (figure.Layer.Name = "Background") Then
                            figuresForDeletion.Add(figure)
                        End If
                    Next

                    For Each figure As vdFigure In figuresForDeletion
                        Me.vDraw.ActiveDocument.ActiveLayOut.Entities.RemoveItem(figure)
                    Next
                End If

                Me.vDraw.ActiveDocument.ActiveLayer = Me.vDraw.ActiveDocument.Layers.FindName("Background")

                Dim block As vdBlock = VdOpenSave.AddFromFile(Me.vDraw.ActiveDocument.Blocks, .FileName, True)
                If (block IsNot Nothing) Then
                    Dim insert As New vdInsert
                    With insert
                        .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                        .setDocumentDefaults()

                        .Block = block
                    End With

                    Me.vDraw.ActiveDocument.Model.Entities.AddItem(insert)

                    With Me.vDraw.ActiveDocument
                        .CommandAction.CmdScale(insert, insert.InsertionPoint, 500 / insert.BoundingBox.Width)
                        .Limits = insert.BoundingBox()
                        .ActiveLayOut.Entities.ChangeOrder(insert, True)
                        .Layers.FindName("Background").Lock = True
                        .ActiveLayer = Me.vDraw.ActiveDocument.Layers.FindName("0")
                        Me.vDraw.ActiveDocument.ZoomExtents()
                        .Invalidate()
                    End With

                    _isDirty = True

                    DeselectAll()

                    Me.uckShowBackground.Checked = True
                End If
            End If
        End With
    End Sub

    Private Sub Open()
        With Me.ofdTopologyEditor
            .DefaultExt = KnownFile.TOPV.Trim("."c)
            .FileName = String.Empty
            .Filter = "Topology view files for E³.HarnessAnalyzer (*.topv)|*.topv"
            .Title = TopologyEditorFormStrings.OpenTopoFile_Title

            If (.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then
                Dim topologyViewFile As String = IO.Path.Combine(IO.Path.GetDirectoryName(.FileName), String.Format("{0}.vdcl", IO.Path.GetFileNameWithoutExtension(.FileName)))

                IO.File.Copy(.FileName, topologyViewFile, True)

                If (Me.vDraw.ActiveDocument.Open(topologyViewFile)) Then
                    Me.vDraw.ActiveDocument.ActiveActionRender.PenStyle.color = Color.Magenta
                    Me.vDraw.ActiveDocument.ActiveActionRender.PenStyle.SetStdWidth(2)

                    Me.vDraw.ActiveDocument.ZoomExtents()
                    Me.vDraw.ActiveDocument.Invalidate()

                    DeselectAll()

                    _currentTopologyViewFile = .FileName
                    _harnessPartNumbers = New List(Of String)

                    For Each figure As vdFigure In Me.vDraw.ActiveDocument.ActiveLayOut.Entities
                        If (figure.XProperties.FindName("HarnessPartNumber") IsNot Nothing) Then
                            _harnessPartNumbers.Add(figure.XProperties.FindName("HarnessPartNumber").PropValue.ToString)
                        End If
                    Next

                    Me.uckShowBackground.Checked = True

                    Me.Text = String.Format(TopologyEditorFormStrings.Caption2, .FileName)
                Else
                    MessageBox.Show(TopologyEditorFormStrings.ErrorLoadTopo_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If

                IO.File.Delete(topologyViewFile)
            End If
        End With
    End Sub

    Private Sub Save(saveAs As Boolean)
        DeselectAll()

        If (_currentTopologyViewFile Is Nothing) OrElse (_currentTopologyViewFile = String.Empty) OrElse (saveAs) Then
            With Me.sfdTopologyEditor
                .DefaultExt = KnownFile.TOPV.Trim("."c)
                .FileName = String.Empty
                .Filter = "Topology view files for E³.HarnessAnalyzer (*.topv)|*.topv"
                .OverwritePrompt = True
                .Title = TopologyEditorFormStrings.SaveTopoFile_Title

                If (.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then
                    _currentTopologyViewFile = .FileName

                    Me.Text = String.Format(TopologyEditorFormStrings.Caption2, .FileName)
                Else
                    Exit Sub
                End If
            End With
        End If

        If (Me.vDraw.ActiveDocument.SaveAs(IO.Path.Combine(IO.Path.GetDirectoryName(_currentTopologyViewFile), String.Format("{0}.vdcl", IO.Path.GetFileNameWithoutExtension(_currentTopologyViewFile))))) Then
            IO.File.Copy(IO.Path.Combine(IO.Path.GetDirectoryName(_currentTopologyViewFile), String.Format("{0}.vdcl", IO.Path.GetFileNameWithoutExtension(_currentTopologyViewFile))), _currentTopologyViewFile, True)
            IO.File.Delete(IO.Path.Combine(IO.Path.GetDirectoryName(_currentTopologyViewFile), String.Format("{0}.vdcl", IO.Path.GetFileNameWithoutExtension(_currentTopologyViewFile))))

            _isDirty = False
        Else
            MessageBox.Show(TopologyEditorFormStrings.ErrorSaveTopo_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub


    Private Sub TopologyEditorForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If (Me.vDraw.ActiveDocument.CommandAction.OpenLoops <> 0) Then Me.vDraw.ActiveDocument.CommandAction.Cancel()

        If (_isDirty) AndAlso (Me.DialogResult = System.Windows.Forms.DialogResult.OK OrElse (Me.DialogResult = System.Windows.Forms.DialogResult.Cancel AndAlso MessageBox.Show(TopologyEditorFormStrings.SaveChanges_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = System.Windows.Forms.DialogResult.Yes)) Then Save(False)
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
    End Sub

    Private Sub uckShowBackground_CheckedValueChanged(sender As Object, e As EventArgs) Handles uckShowBackground.CheckedValueChanged
        If (Me.vDraw.ActiveDocument IsNot Nothing) AndAlso (Me.vDraw.ActiveDocument.Layers.FindName("Background") IsNot Nothing) Then
            Me.vDraw.ActiveDocument.Layers.FindName("Background").Frozen = Not Me.uckShowBackground.Checked
            Me.vDraw.ActiveDocument.Invalidate()
        End If
    End Sub

    Private Sub utmTopologyEditor_ToolClick(sender As Object, e As ToolClickEventArgs) Handles utmTopologyEditor.ToolClick
        If (Me.vDraw.ActiveDocument IsNot Nothing) Then
            Select Case e.Tool.Key
                Case "Open"
                    Open()
                Case "Save"
                    Save(False)
                Case "SaveAs"
                    Save(True)
                Case "Export"
                    Export()
                Case "Print"
                    Dim printing As New HarnessAnalyzer.Printing.VdPrinting(Me.vDraw.ActiveDocument)
                    printing.DocumentName = Me.Text

                    Using printForm As New Printing.PrintForm(printing, Nothing, False)
                        printForm.ShowDialog(Me)
                    End Using
                Case "BackgroundPicture"
                    LoadBackgroundPicture()
                Case "PlacePolygonCompartment"
                    Me.vDraw.ActiveDocument.CommandAction.CmdPolyLine(Nothing)
                Case "PlaceRectCompartment"
                    Me.vDraw.ActiveDocument.CommandAction.CmdRect(Nothing, Nothing)
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
            End Select
        End If
    End Sub

    Private Sub vDraw_ActionEnd(sender As Object, actionName As String) Handles vDraw.ActionEnd
        If (_currentCommandAction IsNot Nothing) AndAlso (_currentCommandAction.StartsWith("Cmd")) Then _isDirty = True
        If (actionName = "CmdPolyLine") Then AddCompartment(Me.vDraw.ActiveDocument.Model.Entities.Last)

        Me.lblDrawCommand.Visible = False
        Me.utmTopologyEditor.ToolTipDisplayStyle = ToolTipDisplayStyle.Standard
        Me.vDraw.ActiveDocument.ActiveActionRender.PenStyle.color = Color.Magenta
        Me.vDraw.ActiveDocument.ActiveActionRender.PenStyle.SetStdWidth(2)
        Me.vDraw.SetCustomMousePointer(Nothing)
    End Sub

    Private Sub vDraw_ActionError(sender As Object, actionName As String) Handles vDraw.ActionError
        Me.lblDrawCommand.Visible = False
        Me.utmTopologyEditor.ToolTipDisplayStyle = ToolTipDisplayStyle.Standard
        Me.vDraw.SetCustomMousePointer(Nothing)
    End Sub

    'Private Sub vDraw_ActionJobLoop(sender As Object, action As Object, ByRef cancel As Boolean) Handles vDraw.ActionJobLoop
    '    If (TypeOf action Is VectorDraw.Professional.CommandActions.ActionPolyLine) AndAlso (DirectCast(DirectCast(action, VectorDraw.Professional.CommandActions.ActionPolyLine).Entity, vdPolyline).VertexList.Count >= 3) Then
    '        With DirectCast(DirectCast(action, VectorDraw.Professional.CommandActions.ActionPolyLine).Entity, vdPolyline)
    '            Me.vDraw.ActiveDocument.ActiveActionRender.DrawLine(sender, .VertexList(.VertexList.Count - 1), .VertexList(0))
    '        End With
    '    End If
    'End Sub
    Private Sub vDraw_ActionDraw(sender As Object, action As Object, isHideMode As Boolean, ByRef cancel As Boolean) Handles vDraw.ActionDraw
        If (action IsNot Nothing AndAlso TypeOf (action) Is VectorDraw.Professional.CommandActions.ActionPolyLine) Then
            Dim polyLineAction As VectorDraw.Professional.CommandActions.ActionPolyLine = CType(action, VectorDraw.Professional.CommandActions.ActionPolyLine)
            If (polyLineAction IsNot Nothing AndAlso polyLineAction.Entity IsNot Nothing AndAlso TypeOf (polyLineAction.Entity) Is vdPolyline) Then
                Dim withBlock As vdPolyline = CType(polyLineAction.Entity, vdPolyline)
                If withBlock.VertexList.Count >= 3 Then
                    polyLineAction.Render.DrawLine(sender, withBlock.VertexList(withBlock.VertexList.Count - 1), withBlock.VertexList(0))
                End If
            End If
        End If
    End Sub

    Private Sub vDraw_ActionStart(sender As Object, actionName As String, ByRef cancel As Boolean) Handles vDraw.ActionStart
        If (actionName.StartsWith("Cmd")) OrElse (actionName = "Zoom") Then _currentCommandAction = actionName
        If (actionName <> "BaseAction_ActionPan") AndAlso (_currentCommandAction = "CmdPolyLine" OrElse _currentCommandAction = "CmdRect") Then Me.lblDrawCommand.Visible = True

        Me.utmTopologyEditor.ToolTipDisplayStyle = ToolTipDisplayStyle.None

        Select Case actionName
            Case "BaseAction_ActionGetPoint"
                If (_currentCommandAction <> "Zoom") Then
                    If (_currentCommandAction = "CmdPolyLine") Then
                        Me.lblDrawCommand.Text = TopologyEditorFormStrings.DefSPOfComp_Label
                    Else
                        Me.lblDrawCommand.Text = TopologyEditorFormStrings.DefSCOfComp_Label
                    End If

                    Me.vDraw.SetCustomMousePointer(_cursors("BasePoint"))
                Else
                    Me.lblDrawCommand.Text = String.Empty
                    Me.vDraw.SetCustomMousePointer(Nothing)
                End If
            Case "BaseAction_ActionPolyLine"
                Me.lblDrawCommand.Text = TopologyEditorFormStrings.DefNxtPOfComp_Label
                Me.vDraw.ActiveDocument.Invalidate()
                Me.vDraw.SetCustomMousePointer(_cursors("Vertex"))
            Case "BaseAction_ActionRect"
                Me.lblDrawCommand.Text = TopologyEditorFormStrings.DefEPOfComp_Label
                Me.vDraw.SetCustomMousePointer(_cursors("EndPoint"))
        End Select
    End Sub

    Private Sub vDraw_AfterAddItem(obj As Object) Handles vDraw.AfterAddItem
        If (_currentCommandAction IsNot Nothing) AndAlso (_currentCommandAction = "CmdRect") AndAlso (TypeOf obj Is vdRect) Then AddCompartment(DirectCast(obj, vdFigure))
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
            Dim figuresForDeletion As New List(Of vdFigure)

            For Each selFigure As vdFigure In _selection
                _isDirty = True

                If (selFigure.XProperties.FindName("HarnessPartNumber") IsNot Nothing) AndAlso (_harnessPartNumbers.Contains(selFigure.XProperties.FindName("HarnessPartNumber").PropValue.ToString)) Then
                    _harnessPartNumbers.Remove(selFigure.XProperties.FindName("HarnessPartNumber").PropValue.ToString)

                    For Each figure As vdFigure In Me.vDraw.ActiveDocument.ActiveLayOut.Entities
                        If (figure IsNot Nothing) AndAlso (Not _selection.FindItem(figure)) AndAlso ((figure.XProperties.FindName("StartCompartmentId") IsNot Nothing AndAlso CInt(figure.XProperties.FindName("StartCompartmentId").PropValue) = selFigure.HandleId) OrElse (figure.XProperties.FindName("EndCompartmentId") IsNot Nothing AndAlso CInt(figure.XProperties.FindName("EndCompartmentId").PropValue) = selFigure.HandleId)) Then figuresForDeletion.Add(figure)
                    Next
                End If

                figuresForDeletion.Add(selFigure)
            Next

            For Each figure As vdFigure In figuresForDeletion
                Me.vDraw.ActiveDocument.ActiveLayOut.Entities.RemoveItem(figure)
            Next

            _selection.RemoveAll()

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
                If (_hoveredEntity IsNot Nothing) AndAlso (_selection.FindItem(_hoveredEntity)) AndAlso (Not TypeOf _hoveredEntity Is vdText) Then
                    _hoveredEntity.ShowGrips = True
                    _hoveredEntity.Update()

                    Me.vDraw.ActiveDocument.Invalidate()
                Else
                    DeselectAll()
                End If
            End If
        End If
    End Sub

    Private Sub vDraw_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles vDraw.MouseDoubleClick
        If (Me.vDraw.ActiveDocument.CommandAction.OpenLoops = 0) AndAlso (_hitEntity IsNot Nothing) AndAlso (_hitEntity.XProperties.FindName("HarnessPartNumber") IsNot Nothing) Then
            Using compartmentForm As New CompartmentForm(_harnessPartNumbers, _hitEntity.XProperties.FindName("HarnessPartNumber").PropValue.ToString, _hitEntity.XProperties.FindName("HarnessDescription").PropValue.ToString)
                If (compartmentForm.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then
                    With _hitEntity
                        _harnessPartNumbers.Remove(.XProperties.FindName("HarnessPartNumber").PropValue.ToString)
                        _harnessPartNumbers.Add(compartmentForm.txtHarnessPartNumber.Text)

                        .XProperties.FindName("HarnessPartNumber").PropValue = compartmentForm.txtHarnessPartNumber.Text
                        .XProperties.FindName("HarnessDescription").PropValue = compartmentForm.txtHarnessDescription.Text

                        .Update()
                    End With

                    With DirectCast(FindFromId(CInt(_hitEntity.XProperties.FindName("RefFigureId").PropValue)), vdText)
                        If (compartmentForm.txtHarnessDescription.Text <> String.Empty) Then
                            .TextString = compartmentForm.txtHarnessDescription.Text
                        Else
                            .TextString = String.Format("[{0}]", compartmentForm.txtHarnessPartNumber.Text)
                        End If

                        .Update()
                    End With

                    _isDirty = True

                    Me.vDraw.ActiveDocument.Invalidate()
                End If
            End Using
        End If
    End Sub

    Private Sub vDraw_MouseDown(sender As Object, e As MouseEventArgs) Handles vDraw.MouseDown
        If (Me.vDraw.ActiveDocument.CommandAction.OpenLoops = 0) AndAlso (e.Button = System.Windows.Forms.MouseButtons.Left) Then
            _addVertexToPolygon = True
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
        ElseIf (Me.vDraw.ActiveDocument.CommandAction.OpenLoops = 0) AndAlso (e.Button = System.Windows.Forms.MouseButtons.Right) AndAlso (_hoveredEntity IsNot Nothing) AndAlso (TypeOf _hoveredEntity Is vdPolyline) AndAlso (_hoveredEntity.ShowGrips) Then
            _clickPoint = GetGridSnapPoint(Me.vDraw.ActiveDocument.CCS_CursorPos)

            For gripPointIndex As Integer = 0 To _hoveredEntity.GetGripPoints.Count - 1
                Dim gripPoint As gPoint = GetGridSnapPoint(_hoveredEntity.GetGripPoints(gripPointIndex))

                If (Math.Round(gripPoint.x, 0) = Math.Round(_clickPoint.x, 0)) AndAlso (Math.Round(gripPoint.y, 0) = Math.Round(_clickPoint.y, 0)) Then
                    DirectCast(_hoveredEntity, vdPolyline).VertexList.RemoveAt(gripPointIndex)
                    DirectCast(_hoveredEntity, vdPolyline).Update()

                    Exit For
                End If
            Next
        Else
            _clickPoint = Nothing
            _movePoint = Nothing
        End If
    End Sub

    Private Sub vDraw_MouseMove(sender As Object, e As MouseEventArgs) Handles vDraw.MouseMove
        _hoveredEntity = Me.vDraw.ActiveDocument.ActiveLayOut.GetEntityFromPoint(Me.vDraw.ActiveDocument.ActiveRender.View2PixelI(GetGridSnapPoint(Me.vDraw.ActiveDocument.CCS_CursorPos)), 6, False, vdDocument.LockLayerMethodEnum.DisableAll)

        If (Me.vDraw.ActiveDocument.CommandAction.OpenLoops = 0) Then
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

            If (e.Button = System.Windows.Forms.MouseButtons.Left) AndAlso (Not E3.Lib.DotNet.Expansions.Devices.My.Computer.Keyboard.CtrlKeyDown) Then HandleMouseMoveWhileLeftButtonDown(e)
        End If

        Me.lblCoordinates.Text = String.Format("X: {0}, Y: {1}", Math.Round(Me.vDraw.ActiveDocument.CCS_CursorPos.x, 2), Math.Round(Me.vDraw.ActiveDocument.CCS_CursorPos.y, 2))
    End Sub

    Private Sub vDraw_MouseUp(sender As Object, e As MouseEventArgs) Handles vDraw.MouseUp
        _addVertexToPolygon = True
        _movePoint = Nothing
        _selectedGripPointIndex = Integer.MaxValue
    End Sub

End Class