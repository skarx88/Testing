Imports System.IO
Imports VectorDraw.Geometry
Imports VectorDraw.Professional.Constants
Imports VectorDraw.Professional.vdCollections
Imports VectorDraw.Professional.vdFigures
Imports VectorDraw.Professional.vdObjects
Imports VectorDraw.Professional.vdPrimaries
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.HarnessAnalyzer.Shared
Imports Zuken.E3.Lib.Schema.Kbl.Properties

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class TopologyHub

    Friend Event CompartmentClicked(ByVal sender As TopologyHub, ByVal e As String)

    Private _activeCompartments As List(Of String)
    Private _carTopologyViewFile As [Lib].IO.Files.Hcv.TopologyViewContainerFile
    Private _inlinerConnections As List(Of Tuple(Of Tuple(Of String, String), Tuple(Of String, String)))
    Private _inlinerIdentifiers As InlinerIdentifierCollection
    Private _selectedCompartments As List(Of String)
    Private _hoveredEntity As vdFigure
    Private _selection As vdSelection

    Public Sub New()
        Try
            InitializeComponent()
        Catch ex As Exception
            If Not ex.OnDebugCheckForVectorDrawEvaluationError Then
                Throw
            End If
        End Try

        _activeCompartments = New List(Of String)
        _selectedCompartments = New List(Of String)

        If Me.vDraw IsNot Nothing Then
            Me.vDraw.AllowDrop = False
            Me.vDraw.EnsureDocument()
            Me.vDraw.InputDevice_TouchGesture.Actions = VectorDraw.Professional.Actions.TouchGestureActions.Pan Or VectorDraw.Professional.Actions.TouchGestureActions.Zoom
        End If
    End Sub

    Private Sub AddFigureToSelection(selFigure As vdFigure, Optional removeFromSelection As Boolean = True)
        If (Not _selection.FindItem(selFigure)) Then
            selFigure.XProperties.FindName("PenColor").PropValue = selFigure.PenColor.SystemColor
            selFigure.PenColor.SystemColor = Color.Magenta

            _selection.AddItem(selFigure, True, vdSelection.AddItemCheck.Nochecking)

            If (selFigure.XProperties.FindName("RefFigureId") IsNot Nothing) Then
                For Each figure As vdFigure In Me.vDraw.ActiveDocument.Model.Entities
                    If (figure.XProperties.FindName("RefFigureId") IsNot Nothing) AndAlso (CInt(figure.XProperties.FindName("RefFigureId").PropValue) = selFigure.HandleId) AndAlso (Not figure.Equals(selFigure)) AndAlso (Not _selection.FindItem(figure)) Then
                        figure.XProperties.FindName("PenColor").PropValue = figure.PenColor.SystemColor
                        figure.PenColor.SystemColor = Color.Magenta

                        _selection.AddItem(figure, True, vdSelection.AddItemCheck.Nochecking)

                        figure.Update()
                        figure.Invalidate()
                    End If
                Next
            End If
        ElseIf (_selection.FindItem(selFigure)) AndAlso (removeFromSelection) Then
            selFigure.PenColor.SystemColor = DirectCast(selFigure.XProperties.FindName("PenColor").PropValue, Color)
            selFigure.ShowGrips = False

            _selection.RemoveItem(selFigure)

            If (selFigure.XProperties.FindName("RefFigureId") IsNot Nothing) Then
                For Each figure As vdFigure In Me.vDraw.ActiveDocument.Model.Entities
                    If (figure.XProperties.FindName("RefFigureId") IsNot Nothing) AndAlso (CInt(figure.XProperties.FindName("RefFigureId").PropValue) = selFigure.HandleId) AndAlso (Not figure.Equals(selFigure)) AndAlso (_selection.FindItem(figure)) Then
                        figure.PenColor.SystemColor = DirectCast(figure.XProperties.FindName("PenColor").PropValue, Color)

                        _selection.RemoveItem(figure)

                        figure.Update()
                        figure.Invalidate()
                    End If
                Next
            End If
        End If

        selFigure.Update()
        selFigure.Invalidate()
    End Sub

    Private Sub DeselectAll()
        If (_selection?.Count > 0) Then
            For Each selFigure As vdFigure In _selection
                If (selFigure.XProperties.FindName("PenColor") IsNot Nothing) Then
                    selFigure.PenColor.SystemColor = DirectCast(selFigure.XProperties.FindName("PenColor").PropValue, Color)
                End If

                selFigure.ShowGrips = False
                selFigure.Update()
            Next

            _selection.RemoveAll()
        End If

        Me.vDraw.ActiveDocument?.Invalidate()
    End Sub

    Private Sub DrawInlinerConnection(ByRef compartmentsWithConnection As Dictionary(Of String, vdFigure), ByRef connection As vdFigure, ByRef endCompartment As vdFigure, inlinerConnection As Tuple(Of Tuple(Of String, String), Tuple(Of String, String)), ByRef startCompartment As vdFigure)
        For Each figure As vdFigure In Me.vDraw.ActiveDocument.Model.Entities
            If (figure.GetXPropertyValue("HarnessPartNumber") = inlinerConnection.Item1.Item1) AndAlso (startCompartment Is Nothing) Then
                startCompartment = figure
            End If

            If (figure.GetXPropertyValue("HarnessPartNumber") = inlinerConnection.Item2.Item1) AndAlso (endCompartment Is Nothing) Then
                endCompartment = figure
            End If
        Next

        If (startCompartment IsNot Nothing) AndAlso (endCompartment IsNot Nothing) Then
            Dim startPoint As gPoint = GetConnectionPointOfCompartment(startCompartment)
            Dim endPoint As gPoint = GetConnectionPointOfCompartment(endCompartment)

            Dim angle As Double = startPoint.GetAngle(endPoint)
            Dim polyline As New vdPolyline
            With polyline
                .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                .setDocumentDefaults()

                .Label = "Connection"
                .PenWidth = 2
                .SPlineFlag = VectorDraw.Professional.Constants.VdConstSplineFlag.SFlagQUADRATIC
                .VertexList.Add(startPoint)

                If (angle = 0) OrElse (Math.Round(angle, 2) = Math.Round(Math.PI / 2, 2)) Then
                    ' No intermediate vertex necessary
                Else
                    .VertexList.Add(endPoint.x, startPoint.y, 0, 0)
                End If

                .VertexList.Add(endPoint)

                .XProperties.Add("EndCompartment").PropValue = endCompartment.GetXPropertyValue("HarnessPartNumber")
                .XProperties.Add("PenColor")
                .XProperties.Add("StartCompartment").PropValue = startCompartment.GetXPropertyValue("HarnessPartNumber")
            End With

            Me.vDraw.ActiveDocument.Model.Entities.AddItem(polyline)

            connection = polyline

            If (Not compartmentsWithConnection.ContainsKey(startCompartment.GetXPropertyValue("HarnessPartNumber"))) Then
                compartmentsWithConnection.Add(startCompartment.GetXPropertyValue("HarnessPartNumber"), startCompartment)
            End If

            If (Not compartmentsWithConnection.ContainsKey(endCompartment.GetXPropertyValue("HarnessPartNumber"))) Then
                compartmentsWithConnection.Add(endCompartment.GetXPropertyValue("HarnessPartNumber"), endCompartment)
            End If
        End If
    End Sub

    Private Function GetConnectionPointOfCompartment(compartment As vdFigure) As gPoint
        If (TypeOf compartment Is vdPolyline) Then
            Dim polyline As vdPolyline = DirectCast(compartment, vdPolyline)
            Dim vertexPoints As New gPoints

            For Each vertex As Vertex In polyline.VertexList
                vertexPoints.Add(vertex.x, vertex.y, 0)
            Next

            Return vertexPoints.Centroid
        Else
            Return compartment.BoundingBox.MidPoint
        End If
    End Function

    Friend Sub Initialize(Optional carTopologyViewFile As [Lib].IO.Files.Hcv.TopologyViewContainerFile = Nothing)
        If carTopologyViewFile IsNot Nothing Then
            _carTopologyViewFile = carTopologyViewFile
        End If

        If _carTopologyViewFile IsNot Nothing Then
            Using tempFile As TempFile = TempFile.CreateNewWithFileContent(_carTopologyViewFile, ".vdcl") ' HINT: extension is mandatory or load will fail by vectorDraw
                Try
                    Dim success As Boolean = vDraw.ActiveDocument.Open(tempFile.Name)
                    If Not success Then
                        MessageBoxEx.ShowWarning(TopologyHubStrings.ProblemLoadTopo_Msg)
                        Return
                    End If

                    With Me.vDraw.ActiveDocument
                        .UndoHistory.PushEnable(False)

                        .ActiveActionRender.PenStyle.color = Color.Magenta
                        .ActiveActionRender.PenStyle.SetStdWidth(2)

                        .EnableAutoGripOn = False
                        .EnableUrls = False
                        .FreezeGridEvents = True
                        .FreezeModifyEvents.Push(True)

                        With .GlobalRenderProperties
                            .AxisSize = 10
                            .CrossSize = 8
                            .GridColor = Color.Black
                            .SelectingCrossColor = Color.Transparent
                            .SelectingWindowColor = Color.Transparent
                        End With

                        .GridMode = False
                        .Limits = .ActiveLayOut.Entities.GetBoundingBox(True, True)
                        .MouseWheelZoomScale = 1.0
                        .OrbitActionKey = vdDocument.KeyWithMouseStroke.None
                        .OsnapDialogKey = Keys.None
                        .Palette.Background = Color.White
                        .ShowUCSAxis = False
                        .SnapMode = False
                        .UrlActionKey = Keys.None
                        .Selections.Add(New vdSelection)
                    End With
                Catch ex As Exception
                    ex.ShowMessageBox(TopologyHubStrings.ProblemLoadTopo_Msg)
                    Return
                End Try
            End Using
        End If
        Me.vDraw.ActiveDocument.ZoomExtents()
        Me.vDraw.ActiveDocument.Invalidate()
        _selection = Me.vDraw.ActiveDocument.Selections(0)
    End Sub

    Friend Sub InitializeInlinerConnections(inlinerConnections As List(Of Tuple(Of Tuple(Of String, String), Tuple(Of String, String))), inlinerIdentifiers As InlinerIdentifierCollection)
        Dim compartmentsWithConnection As New Dictionary(Of String, vdFigure)
        Dim connectionIdsWithInlinerNames As New Dictionary(Of ULong, List(Of String))
        Dim figuresForDeletion As New List(Of vdFigure)

        _inlinerConnections = inlinerConnections
        _inlinerIdentifiers = inlinerIdentifiers

        If (Me.vDraw.ActiveDocument Is Nothing) Then
            Exit Sub
        End If

        For Each figure As vdFigure In Me.vDraw.ActiveDocument.Model.Entities
            If (figure.Label = "Connection") Then
                figuresForDeletion.Add(figure)
            End If
        Next

        For Each figure As vdFigure In figuresForDeletion
            Me.vDraw.ActiveDocument.Model.Entities.RemoveItem(figure)
        Next

        For Each inlinerConnection As Tuple(Of Tuple(Of String, String), Tuple(Of String, String)) In inlinerConnections
            Dim connection As vdFigure = Nothing
            Dim startCompartment As vdFigure = Nothing
            Dim endCompartment As vdFigure = Nothing

            For Each figure As vdFigure In Me.vDraw.ActiveDocument.Model.Entities
                If (TypeOf figure Is vdPolyline) AndAlso ((figure.GetXPropertyValue("StartCompartment") = inlinerConnection.Item1.Item1 AndAlso figure.GetXPropertyValue("EndCompartment") = inlinerConnection.Item2.Item1) OrElse (figure.GetXPropertyValue("StartCompartment") = inlinerConnection.Item2.Item1 AndAlso figure.GetXPropertyValue("EndCompartment") = inlinerConnection.Item1.Item1)) Then
                    connection = figure

                    Exit For
                End If
            Next

            If (connection Is Nothing) Then
                DrawInlinerConnection(compartmentsWithConnection, connection, endCompartment, inlinerConnection, startCompartment)
            End If
            If (connection IsNot Nothing) AndAlso (Not connectionIdsWithInlinerNames.ContainsKey(connection.HandleId)) Then
                connectionIdsWithInlinerNames.Add(connection.HandleId, New List(Of String))
            End If

            Dim inlinerName1 As String = inlinerConnection.Item1.Item2
            Dim inlinerName2 As String = inlinerConnection.Item2.Item2

            For Each inlinerIdentifier As InlinerIdentifier In inlinerIdentifiers
                If (inlinerIdentifier.KBLPropertyName = ConnectorPropertyName.Id.ToString) Then
                    If inlinerIdentifier.IsMatch(inlinerName1) Then
                        inlinerName1 = inlinerIdentifier.GetConnectorRecognizer(inlinerName1)
                    End If
                    If inlinerIdentifier.IsMatch(inlinerName2) Then
                        inlinerName2 = inlinerIdentifier.GetConnectorRecognizer(inlinerName2)
                    End If
                End If
            Next

            If (connection IsNot Nothing) AndAlso (Not connectionIdsWithInlinerNames(connection.HandleId).Contains(inlinerName1)) Then
                connectionIdsWithInlinerNames(connection.HandleId).Add(inlinerName1)
            End If
            If (connection IsNot Nothing) AndAlso (inlinerName1 <> inlinerName2) AndAlso (Not connectionIdsWithInlinerNames(connection.HandleId).Contains(inlinerName2)) Then
                connectionIdsWithInlinerNames(connection.HandleId).Add(inlinerName2)
            End If
        Next

        For Each connectionIdWithInlinerNames As KeyValuePair(Of ULong, List(Of String)) In connectionIdsWithInlinerNames
            connectionIdWithInlinerNames.Value.Sort()

            Dim angle As Double = 0
            Dim insertionPoint As gPoint = Nothing

            For Each figure As vdFigure In Me.vDraw.ActiveDocument.Model.Entities
                If (figure.HandleId = connectionIdWithInlinerNames.Key) Then
                    Dim polyLine As vdPolyline = DirectCast(figure, vdPolyline)
                    angle = polyLine.VertexList.First.GetAngle(polyLine.VertexList.Last)
                    insertionPoint = polyLine.getPointAtDist(polyLine.Length / 2)

                    If (angle > 0) AndAlso (Math.Round(angle, 2) <= Math.Round(Math.PI / 2, 2)) Then
                        insertionPoint.x += 5
                    End If

                    Exit For
                End If
            Next

            For Each inlinerName As String In connectionIdWithInlinerNames.Value
                insertionPoint.y -= 5

                Dim text As New vdText
                With text
                    .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                    .setDocumentDefaults()

                    .Bold = True
                    .Height = 4

                    If (angle > 0) AndAlso (Math.Round(angle, 2) <= Math.Round(Math.PI / 2, 2)) Then
                        .HorJustify = VdConstHorJust.VdTextHorLeft
                    Else
                        .HorJustify = VdConstHorJust.VdTextHorCenter
                    End If

                    .InsertionPoint = insertionPoint
                    .Label = "Connection"
                    .PenColor.SystemColor = Color.Black
                    .TextString = inlinerName
                    .VerJustify = VdConstVerJust.VdTextVerCen

                    .XProperties.Add("PenColor")
                End With

                Me.vDraw.ActiveDocument.Model.Entities.AddItem(text)
            Next
        Next

        For Each compartment As vdFigure In compartmentsWithConnection.Values
            Dim connectionPoint As gPoint = GetConnectionPointOfCompartment(compartment)
            Dim rect As New vdRect
            With rect
                .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                .setDocumentDefaults()

                Dim hatchProperties As New vdHatchProperties(VdConstFill.VdFillModeSolid)
                hatchProperties.SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                hatchProperties.FillColor.SystemColor = Color.Black

                .HatchProperties = hatchProperties
                .Height = 4
                .InsertionPoint = New gPoint(connectionPoint.x - 2, connectionPoint.y - 2)
                .Label = "Connection"
                .PenWidth = 1
                .Width = 4

                .XProperties.Add("PenColor")
                .XProperties.Add("RefFigureId").PropValue = compartment.HandleId
            End With

            Me.vDraw.ActiveDocument.Model.Entities.AddItem(rect)
        Next

        Me.vDraw.ActiveDocument.Invalidate()
    End Sub

    Friend Sub SelectActiveCompartments()
        Dim doc As DocumentForm = My.Application.MainForm?.ActiveDocument
        If doc?.KBL IsNot Nothing Then
            SelectCompartments(New List(Of String)(New String() {doc.KBL.HarnessPartNumber}), Nothing)
        End If
    End Sub

    Friend Sub SelectCompartments(harnessPartNumbers As IEnumerable(Of String), inlinerNames As List(Of String))
        DeselectAll()

        If (Me.vDraw.ActiveDocument IsNot Nothing) AndAlso (_selection IsNot Nothing) Then
            Dim harPrartNrSet As HashSet(Of String) = harnessPartNumbers.ToHashSet

            For Each figure As vdFigure In Me.vDraw.ActiveDocument.ActiveLayOut.Entities
                Dim value As String = figure.GetXPropertyValue("HarnessPartNumber")
                If Not String.IsNullOrEmpty(value) AndAlso harPrartNrSet.Contains(value) Then
                    AddFigureToSelection(figure)
                End If
            Next

            If (_selection.Count > 0) Then
                Dim connectionsForSelection As New List(Of vdFigure)
                Dim prevFigure As vdFigure = _selection(0)

                For selCounter As Integer = 1 To _selection.Count - 1
                    Dim selFigure As vdFigure = _selection(selCounter)

                    If (selFigure.GetXPropertyValue("HarnessPartNumber") IsNot Nothing) Then
                        For Each figure As vdFigure In Me.vDraw.ActiveDocument.ActiveLayOut.Entities
                            If (figure.GetXPropertyValue("StartCompartment") = prevFigure.GetXPropertyValue("HarnessPartNumber")) AndAlso (figure.GetXPropertyValue("EndCompartment") = selFigure.GetXPropertyValue("HarnessPartNumber")) OrElse (figure.GetXPropertyValue("StartCompartment") = selFigure.GetXPropertyValue("HarnessPartNumber")) AndAlso (figure.GetXPropertyValue("EndCompartment") = prevFigure.GetXPropertyValue("HarnessPartNumber")) Then
                                connectionsForSelection.Add(figure)

                                Exit For
                            End If
                        Next

                        prevFigure = selFigure
                    End If
                Next

                For Each figure As vdFigure In connectionsForSelection
                    AddFigureToSelection(figure)
                Next

                If (inlinerNames IsNot Nothing) Then
                    For Each inlinerName As String In inlinerNames
                        For Each figure As vdFigure In Me.vDraw.ActiveDocument.Model.Entities
                            If (TypeOf figure Is vdText) AndAlso (inlinerName.StartsWith(DirectCast(figure, vdText).TextString)) Then
                                AddFigureToSelection(figure, False)

                                Exit For
                            End If
                        Next
                    Next
                End If
            End If

            _selectedCompartments = harnessPartNumbers.ToList
        End If
    End Sub

    Friend Sub ToggleActiveHarnesses(utmmBrowser As Infragistics.Win.UltraWinTabbedMdi.UltraTabbedMdiManager)
        If (utmmBrowser IsNot Nothing) Then
            _activeCompartments = New List(Of String)

            If (utmmBrowser.TabGroups.Count <> 0) Then
                For Each tabGroup As Infragistics.Win.UltraWinTabbedMdi.MdiTabGroup In utmmBrowser.TabGroups
                    For Each tab As Infragistics.Win.UltraWinTabbedMdi.MdiTab In tabGroup.Tabs
                        Dim pn As String = DirectCast(tab.Form, DocumentForm).KBL?.HarnessPartNumber
                        If Not String.IsNullOrEmpty(pn) Then
                            _activeCompartments.Add(pn)
                        End If
                    Next
                Next
            End If
        End If

        If (Me.vDraw.ActiveDocument IsNot Nothing) Then
            For Each figure As vdFigure In Me.vDraw.ActiveDocument.ActiveLayOut.Entities
                If (TypeOf figure Is vdPolyline) AndAlso (figure.GetXPropertyValue("HarnessPartNumber") IsNot Nothing) Then
                    With DirectCast(figure, vdPolyline)
                        .HatchProperties.FillColor.SystemColor = Color.LightGray
                        .HatchProperties.FillColor.AlphaBlending = 128
                        .PenColor.SystemColor = Color.LightGray

                        .Update()
                    End With
                ElseIf (TypeOf figure Is vdRect) AndAlso (figure.GetXPropertyValue("HarnessPartNumber") IsNot Nothing) Then
                    With DirectCast(figure, vdRect)
                        .HatchProperties.FillColor.SystemColor = Color.LightGray
                        .HatchProperties.FillColor.AlphaBlending = 128
                        .PenColor.SystemColor = Color.LightGray

                        .Update()
                    End With
                End If
            Next

            For Each figure As vdFigure In Me.vDraw.ActiveDocument.ActiveLayOut.Entities
                If (TypeOf figure Is vdPolyline OrElse TypeOf figure Is vdRect) AndAlso (figure.XProperties.FindName("HarnessPartNumber") IsNot Nothing) AndAlso (_activeCompartments.Contains(figure.XProperties.FindName("HarnessPartNumber").PropValue.ToString)) Then
                    If (TypeOf figure Is vdPolyline) Then
                        With DirectCast(figure, vdPolyline)
                            .HatchProperties.FillColor.SystemColor = Color.LightBlue
                            .HatchProperties.FillColor.AlphaBlending = 128
                            .PenColor.SystemColor = Color.LightBlue

                            .Update()
                        End With
                    Else
                        With DirectCast(figure, vdRect)
                            .HatchProperties.FillColor.SystemColor = Color.LightBlue
                            .HatchProperties.FillColor.AlphaBlending = 128
                            .PenColor.SystemColor = Color.LightBlue

                            .Update()
                        End With
                    End If
                End If
            Next

            Me.vDraw.ActiveDocument.Invalidate()
        End If
    End Sub

    Private Sub TopologyHub_SizeChanged(sender As Object, e As EventArgs) Handles Me.SizeChanged
        If Me.vDraw?.ActiveDocument IsNot Nothing Then
            Me.vDraw.ActiveDocument.ZoomExtents()
            Me.vDraw.ActiveDocument.Invalidate()
        End If
    End Sub

    Private Sub btnReload_Click(sender As Object, e As EventArgs) Handles btnReload.Click
        Initialize()

        If (_inlinerConnections IsNot Nothing) Then
            InitializeInlinerConnections(_inlinerConnections, _inlinerIdentifiers)
        End If

        ToggleActiveHarnesses(Nothing)
        SelectCompartments(_selectedCompartments, Nothing)
    End Sub

    Private Sub vDraw_ActionStart(sender As Object, actionName As String, ByRef cancel As Boolean) Handles vDraw.ActionStart
        If (actionName = "BaseAction_ActionPan") Then
            cancel = True
        End If
    End Sub

    Private Sub vDraw_MouseClick(sender As Object, e As MouseEventArgs) Handles vDraw.MouseClick
        If (e.Button = System.Windows.Forms.MouseButtons.Left) AndAlso (_hoveredEntity IsNot Nothing) AndAlso (_hoveredEntity.GetXPropertyValue("HarnessPartNumber") IsNot Nothing) Then
            RaiseEvent CompartmentClicked(Me, _hoveredEntity.GetXPropertyValue("HarnessPartNumber"))
        End If
    End Sub

    Private Sub vDraw_MouseMove(sender As Object, e As MouseEventArgs) Handles vDraw.MouseMove
        _hoveredEntity = Me.vDraw.ActiveDocument.ActiveLayOut.GetEntityFromPoint(Me.vDraw.ActiveDocument.ActiveRender.View2PixelI(Me.vDraw.ActiveDocument.CCS_CursorPos), 4, False, vdDocument.LockLayerMethodEnum.DisableAll)
        If (_hoveredEntity IsNot Nothing) AndAlso (_hoveredEntity.GetXPropertyValue("HarnessPartNumber") IsNot Nothing) Then
            Me.vDraw.SetCustomMousePointer(Cursors.Hand)
            Me.vDraw.ActiveDocument.Invalidate()
        ElseIf (Me.vDraw.GetCustomMousePointer IsNot Nothing) Then
            Me.vDraw.SetCustomMousePointer(Nothing)
            Me.vDraw.ActiveDocument.Invalidate()
        End If
    End Sub

End Class
