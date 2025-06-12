Imports System.IO
Imports System.Text.RegularExpressions
Imports VectorDraw.Geometry
Imports VectorDraw.Professional.vdFigures
Imports VectorDraw.Professional.vdObjects
Imports VectorDraw.Professional.vdPrimaries
Imports Zuken.E3.HarnessAnalyzer.Shared.Common

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class StartEndConnectorsForm

    Private _wireId As String
    Private _wireShortName As String
    Private _lock As New System.Threading.SemaphoreSlim(1)

    Public Sub New()
        InitializeComponent()

        Me.BackColor = Color.White

        Me.vDraw.AllowDrop = False
        Me.vDraw.EnsureDocument()
        Me.vDraw.InputDevice_TouchGesture.Actions = VectorDraw.Professional.Actions.TouchGestureActions.Pan Or VectorDraw.Professional.Actions.TouchGestureActions.Zoom

        With Me.vDraw.ActiveDocument
            .UndoHistory.PushEnable(False)

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

            .GridMode = False
            .OrbitActionKey = vdDocument.KeyWithMouseStroke.None
            .OsnapDialogKey = Keys.None
            .Palette.Background = Color.White
            .ShowUCSAxis = False
            .UrlActionKey = Keys.None
            .EnableToolTips = False
        End With
    End Sub

    Public Sub AddFigure(figure As vdFigure, key As String)
        Dim clone As vdFigure = DirectCast(figure.Clone(Me.vDraw.ActiveDocument), vdFigure)
        clone.XProperties.Add(PROPERTY_CONNECTORSHORTNAME).PropValue = key

        Me.vDraw.ActiveDocument.Model.Entities.AddItem(clone)
    End Sub


    Private Sub HighlightWire(groups As VectorDraw.Professional.vdCollections.vdEntities)
        For Each group As VdSVGGroup In groups
            If (group.KblId = _wireId OrElse group.SecondaryKblIds.Contains(_wireId)) Then
                group.Lighting = Lighting.Highlight
            End If

            If (group.ChildGroups.Count <> 0) Then
                HighlightWire(group.ChildGroups)
            End If
        Next
    End Sub

    Private Sub StartEndConnectorsForm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.vDraw.ActiveDocument.Layers.FindName("0").Lock = False
        'left and right connector pane: table and face left:0 right:1
        Dim connPanes As ConnectorPaneSize() = {New ConnectorPaneSize, New ConnectorPaneSize}
        Dim tableAndFaces As New Dictionary(Of String, TableFacesMap)

        For Each group As VdSVGGroup In Me.vDraw.ActiveDocument.Model.Entities
            If Not group.Deleted Then
                Dim Id As String = group.XProperties.FindName(PROPERTY_CONNECTORSHORTNAME).PropValue.ToString
                If Not String.IsNullOrEmpty(Id) Then
                    If Not tableAndFaces.ContainsKey(Id) Then
                        tableAndFaces.Add(Id, New TableFacesMap)
                    End If

                    If (group.SVGType = SvgType.table.ToString) Then
                        tableAndFaces(Id).Table = group
                    Else
                        Dim matrix As New Matrix
                        matrix.A11 = -matrix.A11
                        If (group.ECSMatrix.IsEqualMatrix(New Matrix, 0.1)) Then
                            group.Transformby(matrix)
                        Else
                            Me.vDraw.ActiveDocument.CommandAction.CmdMirror(group, group.BoundingBox.MidPoint, New gPoint(group.BoundingBox.Min.x, group.BoundingBox.MidPoint.y), "no")
                        End If

                        Dim radius As Double = 0
                        For Each figure As vdFigure In group.ChildGroups
                            If (TryCast(figure, VdSVGGroup) IsNot Nothing) Then
                                radius = DirectCast(figure, VdSVGGroup).Rotation

                                Exit For
                            End If
                        Next
                        group.Rotation = radius

                        tableAndFaces(Id).Faces.Add(group)
                    End If
                End If
            End If
        Next

        Dim cntr As Integer = 0
        For Each entry As KeyValuePair(Of String, TableFacesMap) In tableAndFaces

            Dim pos As Double = connPanes(0).ConnectorWidth + (cntr + 1) * 10
            Dim pt As gPoint
            If entry.Value.Table IsNot Nothing Then
                Me.vDraw.ActiveDocument.CommandAction.CmdMove(entry.Value.Table, entry.Value.Table.BoundingBox.Min, New gPoint(pos, 10, 0))
                HighlightWire(entry.Value.Table.ChildGroups)
                connPanes(cntr).ConnectorShortName = entry.Key
                connPanes(cntr).ConnectorWidth = entry.Value.Table.BoundingBox.Width
                pt = New gPoint(entry.Value.Table.BoundingBox.Left, entry.Value.Table.BoundingBox.Top + 10, 0)
            Else
                pt = New gPoint(pos, 10, 0)
            End If

            For Each face As VdSVGGroup In entry.Value.Faces
                Me.vDraw.ActiveDocument.CommandAction.CmdMove(face, face.BoundingBox.Min, pt)

                If (face.BoundingBox.Width > connPanes(cntr).ConnectorWidth) Then
                    connPanes(cntr).ConnectorWidth = face.BoundingBox.Width
                End If
                connPanes(cntr).ConnectorHeight = face.BoundingBox.Top
            Next
            connPanes(cntr).ConnectorWidth = AddLabel(connPanes(cntr).ConnectorShortName, connPanes(cntr).ConnectorWidth, pos)

            cntr += 1
            If cntr > 1 Then Exit For
        Next

        If tableAndFaces.Count > 1 Then
            Dim line As New vdLine
            With line
                .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                .setDocumentDefaults()

                If (connPanes(0).ConnectorHeight > connPanes(1).ConnectorHeight) Then
                    .EndPoint = New gPoint(connPanes(0).ConnectorWidth + 15, connPanes(0).ConnectorHeight + 10)
                Else
                    .EndPoint = New gPoint(connPanes(0).ConnectorWidth + 15, connPanes(1).ConnectorHeight + 10)
                End If

                .PenWidth = 1
                .StartPoint = New gPoint(connPanes(0).ConnectorWidth + 15, 0)
            End With

            Me.vDraw.ActiveDocument.Model.Entities.AddItem(line)
        End If

        If tableAndFaces.Count >= 1 Then
            Dim rect As New vdRect
            With rect
                .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                .setDocumentDefaults()

                If (connPanes(0).ConnectorHeight > connPanes(1).ConnectorHeight) Then
                    .Height = connPanes(0).ConnectorHeight + 10
                Else
                    .Height = connPanes(1).ConnectorHeight + 10
                End If

                .InsertionPoint = New gPoint(0, 0, 0)
                .PenWidth = 1
                .Width = connPanes(0).ConnectorWidth + 20 + connPanes(1).ConnectorWidth + 10
            End With
            Me.vDraw.ActiveDocument.Model.Entities.AddItem(rect)
        End If

        Me.vDraw.ActiveDocument.Layers.FindName("0").Lock = True
        vDraw.ActiveDocument.MeterProgress.RaiseOnPaint = True
        Me.vDraw.ActiveDocument.ZoomExtents()
        Me.vDraw.ActiveDocument.Invalidate()
    End Sub

    Private Function AddLabel(label As String, connectorWidth As Double, xInsertion As Double) As Double
        Dim text As New vdText
        With text
            .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
            .setDocumentDefaults()

            .Height = 4
            .InsertionPoint = New gPoint(xInsertion, 4, 0)
            .Style.Bold = True
            .TextString = label
        End With

        Me.vDraw.ActiveDocument.Model.Entities.AddItem(text)

        If (text.BoundingBox.Width > connectorWidth) Then
            Return text.BoundingBox.Width
        End If
        Return connectorWidth
    End Function

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        Using sfdExport As New SaveFileDialog
            With sfdExport
                .DefaultExt = KnownFile.DXF.Trim("."c)
                .FileName = String.Format("Start_end_connectors_for_wire_{0}", Regex.Replace(_wireShortName, "\W", "_"))
                .Filter = "Autodesk DXF file(*.dxf)|*.dxf|JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|Bitmap (*.bmp)|*.bmp"
                .Title = StartEndConnectorsFormStrings.ExportViewToFile_Title

                If (.ShowDialog(Me) = DialogResult.OK) Then
                    Try
                        If (VdOpenSave.SaveAs(Me.vDraw.ActiveDocument, sfdExport.FileName)) Then
                            MessageBox.Show(StartEndConnectorsFormStrings.ExportViewSuccess_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Else
                            MessageBox.Show(StartEndConnectorsFormStrings.ProblemExportView_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End If
                    Catch ex As Exception
                        MessageBox.Show(String.Format(StartEndConnectorsFormStrings.ErrorExportView_Msg, vbCrLf, ex.Message), [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Try
                End If
            End With
        End Using
    End Sub

    Private Sub btnPrint_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPrint.Click
        Dim printing As Printing.VdPrinting = Nothing
        Try
            printing = New Printing.VdPrinting(Me.vDraw.ActiveDocument)
            printing.DocumentName = Me.Text
        Catch ex As Exception
            MessageBoxEx.Show(Me, String.Format(StartEndConnectorsFormStrings.PrintDlgFailed_Msg, ex.Message), [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try

        If printing IsNot Nothing Then
            Using printForm As New Printing.PrintForm(printing, Nothing, False)
                printForm.ShowDialog(Me)
            End Using
        End If
    End Sub

    Private Sub btnRedraw_Click(sender As Object, e As EventArgs) Handles btnRedraw.Click


        If Me.vDraw.ActiveDocument IsNot Nothing Then
            Me.vDraw.ActiveDocument.ZoomExtents()
            Me.vDraw.ActiveDocument.Invalidate()
        End If
        Me.vDraw.ActiveDocument.ZoomExtents()
        Me.vDraw.ActiveDocument.Redraw(True)
    End Sub

    Private Sub vDraw_AfterOpenDocument(ByVal sender As Object) Handles vDraw.AfterOpenDocument
        With Me.vDraw.ActiveDocument
            .UndoHistory.PushEnable(False)

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

            .GridMode = False
            .OrbitActionKey = vdDocument.KeyWithMouseStroke.None
            .OsnapDialogKey = Keys.None
            .Palette.Background = Color.White
            .ShowUCSAxis = False
            .UrlActionKey = Keys.None
        End With
    End Sub

    'Private Async Sub vDraw_ProgressStop(sender As Object, jobDescription As String)
    '    If jobDescription = "Drawing Entities" Then
    '        Await _lock.WaitAsync
    '        Try
    '            'HINT: this is some sort of workaround solution, because currently it's not clear when EXCATLY the vectorDraw drawing is fully ready so that the zoomExtends is "fully" working
    '            Await Task.Delay(100) ' the event is not clear after we have to call the zoomExtendes so that it's working (it's working a at my first test's a little bit later after all entities have been drawn...)
    '            vDraw.ActiveDocument.CommandAction.Zoom("E", 0, 0)
    '        Finally
    '            _lock.Release()
    '        End Try
    '        RemoveHandler vDraw.ActiveDocument.OnProgressStop, AddressOf vDraw_ProgressStop
    '    End If
    'End Sub

    Public Property WireIdToHighLight As String
        Get
            Return _wireId
        End Get
        Set(ByVal value As String)
            _wireId = value
        End Set
    End Property

    Public Property WireShortName As String
        Get
            Return _wireShortName
        End Get
        Set(value As String)
            _wireShortName = value
        End Set
    End Property

    Protected Overrides Sub Dispose(disposing As Boolean)
        'If disposing AndAlso vDraw IsNot Nothing AndAlso vDraw.ActiveDocument IsNot Nothing Then
        '    ' RemoveHandler vDraw.ActiveDocument.OnProgressStop, AddressOf vDraw_ProgressStop
        'End If
        MyBase.Dispose(disposing)
    End Sub


    Private Class TableFacesMap
        Property Table As VdSVGGroup = Nothing
        Property Faces As List(Of VdSVGGroup) = New List(Of VdSVGGroup)
    End Class

    Private Class ConnectorPaneSize
        Property ConnectorHeight As Double = 0.0
        Property ConnectorShortName As String = String.Empty
        Property ConnectorWidth As Double = 0.0
    End Class
End Class