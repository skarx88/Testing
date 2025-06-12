Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Windows.Documents
Imports System.Xml.Serialization
Imports Infragistics.Win.UltraWinToolTip
Imports VectorDraw.Geometry
Imports VectorDraw.Professional.Constants
Imports VectorDraw.Professional.vdFigures
Imports VectorDraw.Professional.vdObjects
Imports VectorDraw.Professional.vdPrimaries

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class OverallConnectivityForm

    Friend Event OverallConnectivityViewMouseClick(sender As Object, e As InformationHubEventArgs)

    Private _connectivity As HarnessConnectivity
    Private _startingPoint As Tuple(Of String, String)
    Private _defaultWireLengthType As String
    Private _harnessesWithInactiveCoresWires As Dictionary(Of String, List(Of String))
    Private _harnessGroups As New Dictionary(Of String, Dictionary(Of Integer, List(Of CavityVertex)))
    Private _harnessDescriptionAndColor As New Dictionary(Of String, Color)
    Private _occupiedGridPositions As List(Of Tuple(Of Double, Double))
    Private _overallConnectivity As OverallConnectivity
    Private _originHarnessConnectivity As HarnessConnectivity
    Private _originWireNumber As String
    Private _kblMappers As IEnumerable(Of KblMapper)
    Private WithEvents _wStateDetector As New WindowStateChangedDetector(Me)
    Private _verticesOfAdjacencies As New Dictionary(Of String, List(Of String))
    Private _adjacenciesOfVertices As New Dictionary(Of String, List(Of String))
    Private _allEmptyAdjacencies As New List(Of String)

    Private Const GRID_X_OFFSET As Double = 50
    Private Const GRID_Y_OFFSET As Double = 25

    Public Sub New(startingPoint As Tuple(Of String, String), defaultWireLengthType As String, harnessesWithInactiveCoresWires As Dictionary(Of String, List(Of String)), overallConnectivity As OverallConnectivity, originHarnessConnectivity As HarnessConnectivity, Optional kblMappersWireResCalc As IEnumerable(Of KblMapper) = Nothing)
        InitializeComponent()

        _startingPoint = startingPoint 'HINT this list needs to have combined Ids with harness and cavity, respectively wireId
        _defaultWireLengthType = defaultWireLengthType
        _harnessesWithInactiveCoresWires = harnessesWithInactiveCoresWires
        _occupiedGridPositions = New List(Of Tuple(Of Double, Double))
        _overallConnectivity = overallConnectivity
        _originHarnessConnectivity = originHarnessConnectivity
        _kblMappers = kblMappersWireResCalc

        Me.btnCalcWireResistance.Visible = kblMappersWireResCalc IsNot Nothing
        Me.btnCalcVoltageDrop.Visible = kblMappersWireResCalc IsNot Nothing
        Me.BackColor = Colors.Background

        If (originHarnessConnectivity.HasWireAdjacency(_startingPoint.Item2)) Then
            Me.Text = String.Format(OverallConnectivityFormStrings.Caption1, originHarnessConnectivity.GetWireAdjacency(_startingPoint.Item2).WireNumber)
        Else
            Me.Text = OverallConnectivityFormStrings.Caption2
        End If

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
                .GridColor = Colors.GridColor
                .SelectingCrossColor = Color.Transparent
                .SelectingWindowColor = Color.Transparent
            End With

            .GridMode = False
            .OrbitActionKey = vdDocument.KeyWithMouseStroke.None
            .OsnapDialogKey = Keys.None
            .Palette.Background = Colors.Background
            .ShowUCSAxis = False
            .ToolTipDispProps.UseReverseOrder = False
            .UrlActionKey = Keys.None
        End With

        InitializeView()
        Me.MinimumSize = Me.ClientSize
    End Sub

    Private Sub CreateConnectivityGraph(startCavityVertex As CavityVertex)
        _connectivity = New HarnessConnectivity 'HINT this graph contains clones of the original graphs!

        Dim processedVertices As New List(Of CavityVertex)
        BuildGraph_Recursively(startCavityVertex, processedVertices)

        _harnessGroups = New Dictionary(Of String, Dictionary(Of Integer, List(Of CavityVertex)))

        processedVertices.Clear()
        SetVerticesGridPositions_Recursively(_connectivity.GetCavityVertex(startCavityVertex.Key), 0, processedVertices)
    End Sub

    Private Sub BuildGraph_Recursively(startVertex As CavityVertex, ByRef processedVertices As List(Of CavityVertex))
        'HINT the traversal needs to be done on the real vertices from the individual connectivities, not the clones!
        If (startVertex IsNot Nothing) Then
            If (Not processedVertices.Contains(startVertex)) Then
                processedVertices.Add(startVertex)

                If (Not _connectivity.HasCavityVertex(startVertex.Key)) Then
                    _connectivity.AddCavityVertex(startVertex.Clone)
                End If
                Dim localStartVertex As CavityVertex = _connectivity.GetCavityVertex(startVertex.Key)
                If (startVertex.IsInliner) Then
                    If (_overallConnectivity IsNot Nothing) Then
                        If (_overallConnectivity.InlinerCavityPairs.ContainsKey(startVertex.Key)) Then
                            Dim counterId As String = _overallConnectivity.InlinerCavityPairs(startVertex.Key)

                            Dim harnessConnectivity As HarnessConnectivity = _overallConnectivity.HarnessConnectivities(HarnessConnectivity.GetHarnessFromUniqueId(counterId))

                            If (harnessConnectivity IsNot Nothing) AndAlso (harnessConnectivity.HasCavityVertex(counterId)) Then

                                If (Not _connectivity.HasCavityVertex(counterId)) Then
                                    Dim counterInlinerVertex As CavityVertex = harnessConnectivity.GetCavityVertex(counterId)
                                    If (Not _connectivity.HasCavityVertex(counterInlinerVertex.Key)) Then
                                        _connectivity.AddCavityVertex(counterInlinerVertex.Clone)
                                    End If
                                    Dim localEndVertex As CavityVertex = _connectivity.GetCavityVertex(counterInlinerVertex.Key)

                                    Dim inlinerAdjacency As New WireAdjacency(String.Format("{0}|{1}", startVertex.Key, counterInlinerVertex.Key))

                                    If (Not _connectivity.HasWireAdjacency(inlinerAdjacency.Key)) Then
                                        _connectivity.AddWireAdjacency(inlinerAdjacency)
                                    End If
                                    'HINT these virtual adjacencies are directed, the others undirected!
                                    localStartVertex.LinkToSuccessorCavityVertexViaWireAdjacency(localEndVertex, inlinerAdjacency)
                                    BuildGraph_Recursively(counterInlinerVertex, processedVertices)
                                End If
                            End If
                        End If
                    End If
                End If

                For Each adjacencyToVertex As AdjacencyToVertex In startVertex.AdjacenciesToSuccessors
                    If (adjacencyToVertex.Vertex IsNot Nothing AndAlso adjacencyToVertex.WireAdjacency.CoreWireOccurrence IsNot Nothing) Then
                        If (IsCoreWireOccurenceActive(startVertex, adjacencyToVertex.WireAdjacency.CoreWireOccurrence)) Then
                            If (Not _connectivity.HasWireAdjacency(adjacencyToVertex.WireAdjacency.Key)) Then
                                _connectivity.AddWireAdjacency(adjacencyToVertex.WireAdjacency.Clone)
                            End If
                            Dim localwireAdjacency As WireAdjacency = _connectivity.GetWireAdjacency(adjacencyToVertex.WireAdjacency.Key)

                            If (Not _connectivity.HasCavityVertex(adjacencyToVertex.Vertex.Key)) Then
                                _connectivity.AddCavityVertex(adjacencyToVertex.Vertex.Clone)
                            End If
                            Dim localEndVertex As CavityVertex = _connectivity.GetCavityVertex(adjacencyToVertex.Vertex.Key)

                            localStartVertex.LinkToSuccessorCavityVertexViaWireAdjacency(localEndVertex, localwireAdjacency)

                            BuildGraph_Recursively(adjacencyToVertex.Vertex(), processedVertices)
                        End If
                    End If
                Next
            End If
        End If
    End Sub

    Private Function IsCoreWireOccurenceActive(startVertex As CavityVertex, coreWireOccurrence As Object) As Boolean
        If (_harnessesWithInactiveCoresWires.ContainsKey(startVertex.HarnessPartNumber)) Then
            Dim coreOcc As Core_occurrence = TryCast(coreWireOccurrence, Core_occurrence)
            If (coreOcc IsNot Nothing) Then
                Return Not _harnessesWithInactiveCoresWires(startVertex.HarnessPartNumber).Contains(coreOcc.SystemId)
            Else
                Dim wireOcc As Wire_occurrence = TryCast(coreWireOccurrence, Wire_occurrence)
                If (wireOcc IsNot Nothing) Then
                    Return Not _harnessesWithInactiveCoresWires(startVertex.HarnessPartNumber).Contains(wireOcc.SystemId)
                End If
            End If
            Return False
        End If
        Return True
    End Function

    Private Sub DrawCavityVertex(cavityVertex As CavityVertex)
        If (Not cavityVertex.ConnectorOccurrence.UsageSpecified) Then
            Dim vertexCircle As New vdCircle
            With vertexCircle
                .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                .setDocumentDefaults()

                .Center = New gPoint(cavityVertex.PositionX, cavityVertex.PositionY)

                Dim hatchProperties As New vdHatchProperties(VdConstFill.VdFillModeSolid)
                hatchProperties.SetUnRegisterDocument(Me.vDraw.ActiveDocument)

                If (cavityVertex.IsInliner) Then
                    hatchProperties.FillColor.SystemColor = Colors.VertexInliner
                Else
                    hatchProperties.FillColor.SystemColor = Colors.Vertex
                End If

                .HatchProperties = hatchProperties
                .PenColor.SystemColor = Colors.VertexBorder
                .Radius = 2.5
                .XProperties.Add("Tooltip").PropValue = GetCavityVertexTooltipInformation(cavityVertex)
            End With

            Me.vDraw.ActiveDocument.Model.Entities.AddItem(vertexCircle)
        Else
            Select Case cavityVertex.ConnectorOccurrence.Usage
                Case Connector_usage.ringterminal
                    Dim vertexTriangle As New vdPolyline
                    With vertexTriangle
                        .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                        .setDocumentDefaults()

                        .Flag = VdConstPlineFlag.PlFlagCLOSE

                        Dim hatchProperties As New vdHatchProperties(VdConstFill.VdFillModeSolid)
                        hatchProperties.SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                        hatchProperties.FillColor.SystemColor = Colors.RingTerminal

                        .HatchProperties = hatchProperties
                        .PenColor.SystemColor = Colors.RingTerminalBorder
                        .VertexList.Add(New gPoint(cavityVertex.PositionX - 2.5, cavityVertex.PositionY - 2.5))
                        .VertexList.Add(New gPoint(cavityVertex.PositionX + 2.5, cavityVertex.PositionY - 2.5))
                        .VertexList.Add(New gPoint(cavityVertex.PositionX, cavityVertex.PositionY + 2.5))
                        .VertexList.Add(New gPoint(cavityVertex.PositionX - 2.5, cavityVertex.PositionY - 2.5))
                        .XProperties.Add("Tooltip").PropValue = GetCavityVertexTooltipInformation(cavityVertex)
                    End With

                    Me.vDraw.ActiveDocument.Model.Entities.AddItem(vertexTriangle)
                Case Connector_usage.splice
                    Dim vertexRect As New vdRect
                    With vertexRect
                        .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                        .setDocumentDefaults()

                        Dim hatchProperties As New vdHatchProperties(VdConstFill.VdFillModeSolid)
                        hatchProperties.SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                        hatchProperties.FillColor.SystemColor = Colors.Splice

                        .HatchProperties = hatchProperties
                        .Height = 5
                        .InsertionPoint = New gPoint(cavityVertex.PositionX - 2.5, cavityVertex.PositionY - 2.5)
                        .PenColor.SystemColor = Colors.SpliceBorder
                        .Width = 5
                        .XProperties.Add("Tooltip").PropValue = GetCavityVertexTooltipInformation(cavityVertex)
                    End With

                    Me.vDraw.ActiveDocument.Model.Entities.AddItem(vertexRect)
            End Select
        End If

        Dim cavityText As New vdText
        With cavityText
            .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
            .setDocumentDefaults()

            .Bold = True
            .Height = 2
            .HorJustify = VdConstHorJust.VdTextHorCenter
            .InsertionPoint = New gPoint(cavityVertex.PositionX, cavityVertex.PositionY)
            .PenColor.SystemColor = Colors.CavityText
            .TextString = cavityVertex.CavityNumber
            .VerJustify = VdConstVerJust.VdTextVerCen
            .XProperties.Add("Tooltip").PropValue = GetCavityVertexTooltipInformation(cavityVertex)
        End With

        Me.vDraw.ActiveDocument.Model.Entities.AddItem(cavityText)

        Dim connectorText As New vdText
        With connectorText
            .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
            .setDocumentDefaults()

            .Height = 2
            .HorJustify = VdConstHorJust.VdTextHorCenter
            .InsertionPoint = New gPoint(cavityVertex.PositionX, cavityVertex.PositionY - 5)
            .PenColor.SystemColor = Colors.ConnectorText
            .TextString = cavityVertex.ConnectorOccurrence.Id
            .VerJustify = VdConstVerJust.VdTextVerCen
        End With

        Dim connectorTextRect As New vdRect
        With connectorTextRect
            .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
            .setDocumentDefaults()

            Dim hatchProperties As New vdHatchProperties(VdConstFill.VdFillModeSolid)
            hatchProperties.SetUnRegisterDocument(Me.vDraw.ActiveDocument)
            hatchProperties.FillColor.SystemColor = Colors.ConnectorTextRect

            .HatchProperties = hatchProperties
            .Height = connectorText.BoundingBox.Height + 0.5
            .InsertionPoint = New gPoint(connectorText.BoundingBox.Min.x - 0.25, connectorText.BoundingBox.Min.y - 0.25)
            .PenColor.SystemColor = Colors.ConnectorTextRect
            .Width = connectorText.BoundingBox.Width + 0.5
        End With

        Me.vDraw.ActiveDocument.Model.Entities.AddItem(connectorTextRect)
        Me.vDraw.ActiveDocument.Model.Entities.AddItem(connectorText)
    End Sub

    Private Sub DrawConnectivityGraph()
        If _connectivity IsNot Nothing Then
            DrawHarnessBoxes()

            Dim adjacencyOffsets As New Dictionary(Of CavityVertex, Dictionary(Of CavityVertex, Integer))
            For Each vertex As CavityVertex In _connectivity.CavityVertices
                adjacencyOffsets.Add(vertex, New Dictionary(Of CavityVertex, Integer))
                For Each adjacencyToVertex As AdjacencyToVertex In vertex.AdjacenciesToSuccessors
                    If (adjacencyOffsets(vertex).ContainsKey(adjacencyToVertex.Vertex)) Then
                        adjacencyOffsets(vertex)(adjacencyToVertex.Vertex) += 1
                    Else
                        adjacencyOffsets(vertex).Add(adjacencyToVertex.Vertex, 1)
                    End If
                Next
            Next

            Dim drawnWireAdjacencies As New List(Of String)
            Dim realAdjacencies As New List(Of Tuple(Of CavityVertex, CavityVertex))
            Dim virtualAdjacencies As New List(Of Tuple(Of CavityVertex, CavityVertex))
            Dim startAdjacency As WireAdjacency = Nothing
            Dim startAdjacencyVertices As Tuple(Of CavityVertex, CavityVertex) = Nothing
            For Each vertex As CavityVertex In _connectivity.CavityVertices
                For Each adjacencyToVertex As AdjacencyToVertex In vertex.AdjacenciesToSuccessors
                    If (Not drawnWireAdjacencies.Contains(adjacencyToVertex.WireAdjacency.Key)) Then
                        drawnWireAdjacencies.Add(adjacencyToVertex.WireAdjacency.Key)
                        Dim tpl As New Tuple(Of CavityVertex, CavityVertex)(vertex, adjacencyToVertex.Vertex)
                        If (adjacencyToVertex.WireAdjacency.CoreWireOccurrence Is Nothing) Then
                            virtualAdjacencies.Add(tpl)
                        Else
                            realAdjacencies.Add(tpl)
                            If (_startingPoint.Item2 = adjacencyToVertex.WireAdjacency.Key) Then
                                startAdjacency = adjacencyToVertex.WireAdjacency
                                startAdjacencyVertices = tpl
                            Else
                                DrawWireAdjacency(vertex, adjacencyToVertex.Vertex, adjacencyToVertex.WireAdjacency, adjacencyOffsets)
                            End If
                        End If
                    End If
                Next
            Next
            'HINT Draw red start adjacency on top so that it is visible even if there are multiple wire adjacencies overlaying
            If (startAdjacency IsNot Nothing) Then
                DrawWireAdjacency(startAdjacencyVertices.Item1, startAdjacencyVertices.Item2, startAdjacency, adjacencyOffsets)
            End If


            virtualAdjacencies.Sort(New VertexSortComparer)
            'HINT start with shortest and detect overlay
            Dim shrinkBend As Boolean = False
            For Each entry As Tuple(Of CavityVertex, CavityVertex) In virtualAdjacencies
                Dim addBend As Boolean = False
                Dim pv1 As New gPoint(entry.Item1.PositionX, entry.Item1.PositionY)
                Dim pv2 As New gPoint(entry.Item2.PositionX, entry.Item2.PositionY)

                For Each entry1 As Tuple(Of CavityVertex, CavityVertex) In realAdjacencies
                    Dim pr1 As New gPoint(entry1.Item1.PositionX, entry1.Item1.PositionY)
                    Dim pr2 As New gPoint(entry1.Item2.PositionX, entry1.Item2.PositionY)

                    If (Globals.PointOnLine(pv1, pv2, pr1)) Then
                        If (Globals.PointOnLine(pv1, pv2, pr2)) Then
                            addBend = True
                            shrinkBend = False
                            Exit For
                        End If
                    ElseIf (Globals.PointOnLine(pr1, pr2, pv1)) Then
                        If (Globals.PointOnLine(pr1, pr2, pv2)) Then
                            addBend = True
                            shrinkBend = False
                            Exit For
                        End If
                    End If
                Next
                If Not addBend Then
                    For Each vertex As CavityVertex In _connectivity.CavityVertices
                        If entry.Item1 IsNot vertex AndAlso entry.Item2 IsNot vertex Then
                            Dim p As New gPoint(vertex.PositionX, vertex.PositionY)
                            If (Globals.PointOnLine(pv1, pv2, p)) Then
                                addBend = True
                                shrinkBend = True
                                Exit For
                            End If
                        End If

                    Next

                End If
                DrawVirtualAdjacency(entry.Item1, entry.Item2, addBend, shrinkBend)
            Next

            For Each vertex As CavityVertex In _connectivity.CavityVertices
                DrawCavityVertex(vertex)
            Next
        End If
    End Sub

    Private Sub DrawHarnessBoxes()
        Dim colorCount As Integer = 0
        Dim harnessColors As New List(Of Color)(Colors.HarnessColors)
        Dim sortedHarnessBoxes As New SortedDictionary(Of Double, vdRect)
        Const OFFSET As Integer = 4

        For Each harnessWithGroups As KeyValuePair(Of String, Dictionary(Of Integer, List(Of CavityVertex))) In _harnessGroups
            Dim harnessBoxes As New List(Of Tuple(Of Integer, Box))

            For Each harnessGroup As KeyValuePair(Of Integer, List(Of CavityVertex)) In harnessWithGroups.Value
                Dim harnessBox As New Box

                For Each cavityVertex As CavityVertex In harnessGroup.Value
                    harnessBox.AddPoint(New gPoint(cavityVertex.PositionX, cavityVertex.PositionY))
                Next

                harnessBoxes.Add(New Tuple(Of Integer, Box)(harnessGroup.Key, harnessBox))
            Next

            For Each harnessBox As Tuple(Of Integer, Box) In harnessBoxes
                Dim harnessRect As New vdRect
                With harnessRect
                    .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                    .setDocumentDefaults()

                    Dim hatchProperties As New vdHatchProperties(VdConstFill.VdFillModeSolid)
                    hatchProperties.SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                    hatchProperties.FillColor.AlphaBlending = 25
                    hatchProperties.FillColor.SystemColor = harnessColors(colorCount)

                    If Not _harnessDescriptionAndColor.ContainsKey(harnessWithGroups.Key) Then
                        _harnessDescriptionAndColor.Add(harnessWithGroups.Key, harnessColors(colorCount))
                    End If

                    .HatchProperties = hatchProperties
                    .Height = harnessBox.Item2.Height + 30
                    .InsertionPoint = New gPoint(harnessBox.Item2.Min.x - 15, harnessBox.Item2.Min.y - 15)
                    .PenColor.SystemColor = Colors.HarnessBoxBorder
                    .Width = harnessBox.Item2.Width + 30
                    .XProperties.Add("GroupIndex").PropValue = harnessBox.Item1
                    .XProperties.Add("Harness").PropValue = harnessWithGroups.Key
                End With

                Me.vDraw.ActiveDocument.Model.Entities.AddItem(harnessRect)

                Do While sortedHarnessBoxes.ContainsKey(harnessRect.InsertionPoint.x)
                    Me.vDraw.ActiveDocument.CommandAction.CmdMove(harnessRect, harnessRect.InsertionPoint, New gPoint(harnessRect.InsertionPoint.x + (GRID_X_OFFSET / 2), harnessRect.InsertionPoint.y))
                    MoveCavityVertices(harnessRect, GRID_X_OFFSET / 2)
                Loop

                sortedHarnessBoxes.Add(harnessRect.InsertionPoint.x, harnessRect)
            Next

            colorCount += 1

            If (colorCount > harnessColors.Count - 1) Then colorCount = 0
        Next

        For Each harnessRect As KeyValuePair(Of Double, vdRect) In sortedHarnessBoxes
            For Each rect As KeyValuePair(Of Double, vdRect) In sortedHarnessBoxes.Where(Function(rt) rt.Key > harnessRect.Key)
                If (harnessRect.Value.BoundingBox.Max.x >= rect.Value.BoundingBox.Min.x) Then
                    Dim offsetX As Double = (harnessRect.Value.BoundingBox.Max.x + (GRID_X_OFFSET / 2)) - rect.Value.InsertionPoint.x
                    Me.vDraw.ActiveDocument.CommandAction.CmdMove(rect.Value, rect.Value.InsertionPoint, New gPoint(harnessRect.Value.BoundingBox.Max.x + (GRID_X_OFFSET / 2), rect.Value.InsertionPoint.y))
                    MoveCavityVertices(rect.Value, offsetX)
                End If
            Next
        Next

        For Each harnessRect As vdRect In sortedHarnessBoxes.Values
            Dim harnessText As New vdText
            With harnessText
                .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                .setDocumentDefaults()

                .Height = 3
                .InsertionPoint = New gPoint(harnessRect.BoundingBox.UpperLeft.x + OFFSET, harnessRect.BoundingBox.UpperLeft.y - OFFSET)
                .PenColor.SystemColor = Colors.HarnessText
                .TextString = harnessRect.XProperties.FindName("Harness").PropValue.ToString.SplitRemoveEmpty("|"c)(0)
                .VerJustify = VdConstVerJust.VdTextVerCen
            End With

            Me.vDraw.ActiveDocument.Model.Entities.AddItem(harnessText)

            harnessText = New vdText
            With harnessText
                .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                .setDocumentDefaults()

                .Height = 2
                .InsertionPoint = New gPoint(harnessRect.BoundingBox.UpperLeft.x + OFFSET, harnessRect.BoundingBox.UpperLeft.y - 2 * OFFSET)
                .PenColor.SystemColor = Colors.HarnessText
                .TextString = harnessRect.XProperties.FindName("Harness").PropValue.ToString.SplitRemoveEmpty("|"c)(1)
                .VerJustify = VdConstVerJust.VdTextVerCen
            End With
            If (harnessRect.BoundingBox.Width > (2 * OFFSET) AndAlso harnessText.BoundingBox.Width > (harnessRect.BoundingBox.Width - (2 * OFFSET))) Then
                Dim ratio As Double = harnessText.BoundingBox.Width / (harnessRect.BoundingBox.Width - (2 * OFFSET))
                harnessText.Height /= ratio
            End If

            Me.vDraw.ActiveDocument.Model.Entities.AddItem(harnessText)
        Next
        If sortedHarnessBoxes.Count > 0 Then
            With sortedHarnessBoxes.First.Value
                DrawLegend(New gPoint(.InsertionPoint.x + 2.5, .InsertionPoint.y - 10), False, False, False)
                DrawLegend(New gPoint(.InsertionPoint.x + 2.5, .InsertionPoint.y - 16), True, False, False)
                DrawLegend(New gPoint(.InsertionPoint.x + 2.5, .InsertionPoint.y - 22), False, True, False)
                DrawLegend(New gPoint(.InsertionPoint.x + 2.5, .InsertionPoint.y - 28), False, False, True)
            End With
        End If
    End Sub

    Private Sub MoveCavityVertices(rect As vdRect, offset As Double)
        Dim groupIndex As Integer = CInt(rect.XProperties.FindName("GroupIndex").PropValue)
        Dim harness As String = rect.XProperties.FindName("Harness").PropValue.ToString
        For Each harnessWithGroups As KeyValuePair(Of String, Dictionary(Of Integer, List(Of CavityVertex))) In _harnessGroups
            If (harnessWithGroups.Key = harness) Then
                For Each harnessGroup As KeyValuePair(Of Integer, List(Of CavityVertex)) In harnessWithGroups.Value
                    If (harnessGroup.Key = groupIndex) Then
                        For Each cavityVertex As CavityVertex In harnessGroup.Value
                            cavityVertex.PositionX += offset
                        Next
                    End If
                Next
            End If
        Next
    End Sub

    Private Sub DrawLegend(center As gPoint, isInliner As Boolean, isRingTerminal As Boolean, isSplice As Boolean)
        If (isInliner) OrElse (Not isRingTerminal AndAlso Not isSplice) Then
            Dim vertexCircle As New vdCircle
            With vertexCircle
                .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                .setDocumentDefaults()

                .Center = center

                Dim hatchProperties As New vdHatchProperties(VdConstFill.VdFillModeSolid)
                hatchProperties.SetUnRegisterDocument(Me.vDraw.ActiveDocument)

                If (isInliner) Then
                    hatchProperties.FillColor.SystemColor = Colors.VertexInliner
                Else
                    hatchProperties.FillColor.SystemColor = Colors.Vertex
                End If

                .HatchProperties = hatchProperties
                .PenColor.SystemColor = Colors.VertexBorder
                .Radius = 2.5
            End With

            Me.vDraw.ActiveDocument.Model.Entities.AddItem(vertexCircle)
        ElseIf (isRingTerminal) Then
            Dim vertexTriangle As New vdPolyline
            With vertexTriangle
                .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                .setDocumentDefaults()

                .Flag = VdConstPlineFlag.PlFlagCLOSE

                Dim hatchProperties As New vdHatchProperties(VdConstFill.VdFillModeSolid)
                hatchProperties.SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                hatchProperties.FillColor.SystemColor = Colors.Vertex

                .HatchProperties = hatchProperties
                .PenColor.SystemColor = Colors.VertexBorder
                .VertexList.Add(New gPoint(center.x - 2.5, center.y - 2.5))
                .VertexList.Add(New gPoint(center.x + 2.5, center.y - 2.5))
                .VertexList.Add(New gPoint(center.x, center.y + 2.5))
                .VertexList.Add(New gPoint(center.x - 2.5, center.y - 2.5))
            End With

            Me.vDraw.ActiveDocument.Model.Entities.AddItem(vertexTriangle)
        ElseIf (isSplice) Then
            Dim vertexRect As New vdRect
            With vertexRect
                .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                .setDocumentDefaults()

                Dim hatchProperties As New vdHatchProperties(VdConstFill.VdFillModeSolid)
                hatchProperties.SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                hatchProperties.FillColor.SystemColor = Colors.Splice

                .HatchProperties = hatchProperties
                .Height = 5
                .InsertionPoint = New gPoint(center.x - 2.5, center.y - 2.5)
                .PenColor.SystemColor = Colors.SpliceBorder
                .Width = 5
            End With

            Me.vDraw.ActiveDocument.Model.Entities.AddItem(vertexRect)
        End If

        Dim vertexText As New vdText
        With vertexText
            .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
            .setDocumentDefaults()

            .Bold = True
            .Height = 2
            .HorJustify = VdConstHorJust.VdTextHorCenter
            .InsertionPoint = center
            .PenColor.SystemColor = Colors.CavityText
            .TextString = "#"
            .VerJustify = VdConstVerJust.VdTextVerCen
        End With

        Me.vDraw.ActiveDocument.Model.Entities.AddItem(vertexText)

        vertexText = New vdText
        With vertexText
            .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
            .setDocumentDefaults()

            .Height = 1.75
            .InsertionPoint = New gPoint(center.x + 5, center.y)
            .PenColor.SystemColor = Colors.ConnectorText

            If (isInliner) Then
                .TextString = OverallConnectivityFormStrings.InlinerLegend_Caption
            ElseIf (isRingTerminal) Then
                .TextString = OverallConnectivityFormStrings.EyeletLegend_Caption
            ElseIf (isSplice) Then
                .TextString = OverallConnectivityFormStrings.SpliceLegend_Caption
            Else
                .TextString = OverallConnectivityFormStrings.ConnectorLegend_Caption
            End If

            .VerJustify = VdConstVerJust.VdTextVerCen
        End With

        Me.vDraw.ActiveDocument.Model.Entities.AddItem(vertexText)
    End Sub

    Private Function TryUpdateWireAdjacencyColor(wireFigure As vdFigure, Optional highlightToggle As Boolean = False) As Boolean
        Dim wireAdj As WireAdjacency = GetWireAdjacency(wireFigure)
        If wireAdj IsNot Nothing Then
            Return TryUpdateWireAdjacencyColor(wireAdj, highlightToggle, If(highlightToggle, Nothing, wireFigure)) ' HINT: when highlight toggling is enabled , all corresponding entities to adjacency are used by setting the parameter to nothing
        End If
        Return False
    End Function

    Private Function TryUpdateWireAdjacencyColor(wireAdjacency As WireAdjacency, Optional highlightToggle As Boolean = False, Optional wireFigure As vdFigure = Nothing) As Boolean
        'HINT: when nothing all relevant entities to this wire adjacency are found here (f.e. a vdPolyling and the corresponding vdText)
        Dim adjacencyEntities As IEnumerable(Of vdFigure) = If(wireFigure IsNot Nothing, New vdFigure() {wireFigure}, Me.vDraw.ActiveDocument.Model.Entities.Cast(Of vdFigure).Where(Function(vdf) vdf.Label = wireAdjacency.Key))
        Dim res As Boolean

        For Each entity As vdFigure In adjacencyEntities
            entity.visibility = vdFigure.VisibilityEnum.Visible

            For pos As Integer = Me.vDraw.ActiveDocument.Model.Entities.GetObjectRealPosition(entity) + 1 To Me.vDraw.ActiveDocument.Model.Entities.Count - 1
                Dim fig As vdFigure = Me.vDraw.ActiveDocument.Model.Entities(pos)
                If TypeOf fig Is vdPolyline AndAlso TypeOf entity Is vdPolyline Then
                    If (ArePolylinesEqual(DirectCast(fig, vdPolyline), DirectCast(entity, vdPolyline))) Then
                        fig.visibility = vdFigure.VisibilityEnum.Invisible
                        fig.Invalidate()
                    End If
                End If
            Next

            With entity
                If highlightToggle Then
                    .PenColor.SystemColor = If(IsSelected(entity), GetAdjencyActiveOrDefaultColor(wireAdjacency), Colors.WireSelected)
                Else
                    .PenColor.SystemColor = GetAdjencyActiveOrDefaultColor(wireAdjacency)
                End If
                res = True
                .Invalidate()
            End With
        Next

        Return res
    End Function

    Private Function ArePolylinesEqual(l1 As vdPolyline, l2 As vdPolyline) As Boolean
        If (l1.VertexList.Count = l2.VertexList.Count AndAlso l1.VertexList.Count > 1) Then
            Dim cnt As Integer = l1.VertexList.Count - 1
            Dim equalPoints As Integer = 0
            For idx As Integer = 0 To cnt
                If (l1.VertexList.Item(idx).AreEqual(l2.VertexList.Item(idx)) OrElse l1.VertexList.Item(idx).AreEqual(l2.VertexList.Item(cnt - idx))) Then
                    equalPoints += 1
                End If
            Next
            Return CBool(equalPoints = l1.VertexList.Count)
        End If
        Return False
    End Function

    Private Function IsSelected(entity As vdFigure) As Boolean
        With entity
            Return .PenColor.SystemColor.ToArgb = Colors.WireSelected.ToArgb
        End With
    End Function

    Private Function GetAdjencyActiveOrDefaultColor(wireAdjacency As WireAdjacency) As System.Drawing.Color
        If (_startingPoint.Item2 = wireAdjacency.Key) Then
            Return Colors.WireActive
        Else
            Return Colors.WireDefault
        End If
    End Function

    Private Sub ResetAllWireAdjcencyColors()
        For Each entity As vdFigure In Me.vDraw.ActiveDocument.Model.Entities
            entity.visibility = vdFigure.VisibilityEnum.Visible
            Dim wireAdjacency As WireAdjacency = GetWireAdjacency(entity)
            If wireAdjacency IsNot Nothing Then
                If TypeOf entity Is vdPolyline Then
                    entity.PenColor.SystemColor = GetAdjencyActiveOrDefaultColor(wireAdjacency)
                ElseIf TypeOf entity Is vdRect Then
                    entity.PenColor.SystemColor = Colors.AdjancencyTextRect
                ElseIf TypeOf entity Is vdText Then
                    entity.PenColor.SystemColor = Colors.AdjancencyText
                End If
            End If
            entity.Invalidate()
        Next
    End Sub

    Private Function IsWireAdjacency(vdFigure As vdFigure) As Boolean
        Dim wireAdjacencykey As String = vdFigure.Label
        If Not String.IsNullOrEmpty(wireAdjacencykey) Then
            Return _connectivity.HasWireAdjacency(wireAdjacencykey)
        End If
        Return False
    End Function

    Private Function GetRealWireAdjacencyEntities(entities As IEnumerable(Of vdFigure)) As IEnumerable(Of vdFigure)
        Return entities.Select(Function(ent)
                                   Dim adj As WireAdjacency = GetWireAdjacency(ent)
                                   If adj IsNot Nothing AndAlso Not String.IsNullOrEmpty(adj.WireNumber) Then
                                       Return ent
                                   End If
                                   Return Nothing
                               End Function).Where(Function(ent) ent IsNot Nothing)
    End Function

    Private Function GetWireAdjacency(vdFigure As vdFigure) As WireAdjacency
        If IsWireAdjacency(vdFigure) Then
            Return _connectivity.GetWireAdjacency(vdFigure.Label)
        End If
        Return Nothing
    End Function

    Private Sub DrawWireAdjacency(cavityVertex1 As CavityVertex, cavityVertex2 As CavityVertex, wireAdjacency As WireAdjacency, offsetDict As Dictionary(Of CavityVertex, Dictionary(Of CavityVertex, Integer)))
        Dim adjacencyLine As New vdPolyline
        Dim intermediateYPosition As Double = cavityVertex1.PositionY + GRID_Y_OFFSET

        With adjacencyLine
            .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
            .setDocumentDefaults()

            .Label = wireAdjacency.Key
            .PenWidth = 1

            .VertexList.Add(New gPoint(cavityVertex1.PositionX, cavityVertex1.PositionY))

            If (cavityVertex1.PositionX = cavityVertex2.PositionX) Then
                .SPlineFlag = VdConstSplineFlag.SFlagQUADRATIC

                If (cavityVertex1.PositionY < cavityVertex2.PositionY) Then
                    .VertexList.Add(New gPoint(cavityVertex1.PositionX + GRID_X_OFFSET / 2, cavityVertex1.PositionY + (cavityVertex2.PositionY - cavityVertex1.PositionY) / 2))
                Else
                    .VertexList.Add(New gPoint(cavityVertex1.PositionX + GRID_X_OFFSET / 2, cavityVertex1.PositionY - (cavityVertex1.PositionY - cavityVertex2.PositionY) / 2))
                End If
            End If

            .VertexList.Add(New gPoint(cavityVertex2.PositionX, cavityVertex2.PositionY))
            .XProperties.Add("Harness").PropValue = cavityVertex1.HarnessPartNumber

        End With

        Me.vDraw.ActiveDocument.Model.Entities.AddItem(adjacencyLine)
        TryUpdateWireAdjacencyColor(adjacencyLine)

        Dim locOffset As Double = 0
        If (offsetDict(cavityVertex1)(cavityVertex2) > 0) Then
            locOffset = offsetDict(cavityVertex1)(cavityVertex2) \ 2
            If (offsetDict(cavityVertex1)(cavityVertex2) Mod 2 <> 0) Then
                locOffset *= -1
            End If
            locOffset *= 3.5
            offsetDict(cavityVertex1)(cavityVertex2) -= 1
        End If

        Dim adjacencyText As New vdText
        With adjacencyText
            .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
            .setDocumentDefaults()

            .Height = 2
            .HorJustify = VdConstHorJust.VdTextHorCenter
            .InsertionPoint = adjacencyLine.getPointAtDist(adjacencyLine.Length / 2)
            .InsertionPoint.y += locOffset
            .Label = wireAdjacency.Key
            .PenColor.SystemColor = Colors.AdjancencyText
            .TextString = wireAdjacency.WireNumber
            .VerJustify = VdConstVerJust.VdTextVerCen
            .XProperties.Add("Harness").PropValue = cavityVertex1.HarnessPartNumber
            If Not String.IsNullOrEmpty(wireAdjacency.WireNumber) Then
                .XProperties.Add("Tooltip").PropValue = GetWireAdjacencyTooltipInformation(wireAdjacency)
            End If
        End With

        Dim adjacencyTextRect As New vdRect
        With adjacencyTextRect
            .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
            .setDocumentDefaults()

            Dim hatchProperties As New vdHatchProperties(VdConstFill.VdFillModeSolid)
            hatchProperties.SetUnRegisterDocument(Me.vDraw.ActiveDocument)
            hatchProperties.FillColor.SystemColor = Colors.AdjancencyTextRect

            .HatchProperties = hatchProperties
            .Height = adjacencyText.BoundingBox.Height + 0.5
            .InsertionPoint = New gPoint(adjacencyText.BoundingBox.Min.x - 0.25, adjacencyText.BoundingBox.Min.y - 0.25)
            .Label = wireAdjacency.Key
            .PenColor.SystemColor = Colors.AdjancencyTextRect
            .Width = adjacencyText.BoundingBox.Width + 0.5
            .XProperties.Add("Harness").PropValue = cavityVertex1.HarnessPartNumber
            If Not String.IsNullOrEmpty(wireAdjacency.WireNumber) Then
                .XProperties.Add("Tooltip").PropValue = GetWireAdjacencyTooltipInformation(wireAdjacency)
            End If
        End With


        Me.vDraw.ActiveDocument.Model.Entities.AddItem(adjacencyTextRect)
        Me.vDraw.ActiveDocument.Model.Entities.AddItem(adjacencyText)

        If (adjacencyLine.VertexList(0).x <= adjacencyLine.VertexList(adjacencyLine.VertexList.Count - 1).x) Then
            adjacencyText.Rotation = adjacencyLine.VertexList(0).GetAngle(adjacencyLine.VertexList(adjacencyLine.VertexList.Count - 1))

            Me.vDraw.ActiveDocument.CommandAction.CmdRotate(adjacencyTextRect, adjacencyTextRect.BoundingBox.MidPoint, adjacencyText.Rotation)
        Else
            adjacencyText.Rotation = adjacencyLine.VertexList(adjacencyLine.VertexList.Count - 1).GetAngle(adjacencyLine.VertexList(0))

            Me.vDraw.ActiveDocument.CommandAction.CmdRotate(adjacencyTextRect, adjacencyTextRect.BoundingBox.MidPoint, adjacencyText.Rotation)
        End If

        If (Math.Round(adjacencyText.Rotation, 2) = Math.Round(Math.PI, 2)) Then
            adjacencyTextRect.Rotation = 0
            adjacencyText.Rotation = 0
        End If

    End Sub
    Private Sub DrawVirtualAdjacency(cavityVertex1 As CavityVertex, cavityVertex2 As CavityVertex, addBend As Boolean, shrinkBend As Boolean)
        Dim adjacencyLine As New vdPolyline
        Dim intermediateYPosition As Double = cavityVertex1.PositionY + GRID_Y_OFFSET
        Const HEIGHT As Double = 80

        Dim offset As Double = HEIGHT
        If shrinkBend Then
            offset /= 2
        End If
        With adjacencyLine
            .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
            .setDocumentDefaults()

            .LineType = Me.vDraw.ActiveDocument.LineTypes.DPIDash
            .VertexList.Add(New gPoint(cavityVertex1.PositionX, cavityVertex1.PositionY))

            If (addBend) Then
                .SPlineFlag = VdConstSplineFlag.SFlagQUADRATIC
                If (cavityVertex1.PositionY = cavityVertex2.PositionY) Then
                    If (cavityVertex1.PositionX > cavityVertex2.PositionX) Then
                        .VertexList.Add(New gPoint(cavityVertex2.PositionX + (cavityVertex1.PositionX - cavityVertex2.PositionX) / 2, cavityVertex1.PositionY + offset))
                    Else
                        .VertexList.Add(New gPoint(cavityVertex1.PositionX + (cavityVertex2.PositionX - cavityVertex1.PositionX) / 2, cavityVertex1.PositionY + offset))
                    End If
                End If
            End If

            .VertexList.Add(New gPoint(cavityVertex2.PositionX, cavityVertex2.PositionY))
            .XProperties.Add("Harness").PropValue = cavityVertex1.HarnessPartNumber

        End With

        Me.vDraw.ActiveDocument.Model.Entities.AddItem(adjacencyLine)
    End Sub

    Private Function GetCavityVertexTooltipInformation(cavityVertex As CavityVertex) As String
        Dim cavityCount As Integer = 0
        Dim housingCode As String = String.Empty
        Dim housingColor As String = String.Empty
        Dim housingType As String = String.Empty
        Dim tooltip As String = String.Empty

        If (cavityVertex.ConnectorOccurrence IsNot Nothing) Then
            With cavityVertex.ConnectorOccurrence
                If (.Slots IsNot Nothing) AndAlso (.Slots.Length <> 0) Then
                    cavityCount = .Slots(0).Cavities.Length
                End If

                tooltip = String.Format(OverallConnectivityFormStrings.CavVertex_Tooltip1, vbCrLf, .Id, .Description, If(.UsageSpecified, .Usage.ToStringOrXmlName, String.Empty), cavityCount)
            End With

            If (cavityVertex.ConnectorHousing IsNot Nothing) Then
                With cavityVertex.ConnectorHousing
                    If (.Housing_code IsNot Nothing) Then
                        housingCode = .Housing_code
                    End If
                    If (.Housing_colour IsNot Nothing) Then
                        housingColor = .Housing_colour
                    End If
                    If (.Housing_type IsNot Nothing) Then
                        housingType = .Housing_type
                    End If

                    tooltip &= String.Format(OverallConnectivityFormStrings.CavVertex_Tooltip2, vbCrLf, housingColor, housingCode, housingType)
                End With
            End If
        End If

        Return tooltip
    End Function

    Private Function GetInformationEventArgs(ParamArray entities() As vdFigure) As InformationHubEventArgs
        Dim informationHubEventArgs As New InformationHubEventArgs(String.Empty)
        informationHubEventArgs.ObjectIds = New HashSet(Of String)

        If (entities IsNot Nothing AndAlso entities.Length > 0) Then
            Dim idPairs As New List(Of KeyValuePair(Of String, String))
            For Each entity As vdFigure In entities
                If entity IsNot Nothing AndAlso entity.Label <> String.Empty Then
                    idPairs.Add(New KeyValuePair(Of String, String)(entity.XProperties.FindName("Harness").PropValue.ToString, HarnessConnectivity.GetKblIdFromUniqueId(entity.Label)))
                End If
            Next

            For Each grp_pair As KeyValuePair(Of String, List(Of String)) In idPairs.GroupBy(Function(kv) kv.Key).ToDictionary(Function(grp) grp.Key, Function(grp) grp.Select(Function(kv) kv.Value).Distinct.ToList)
                For Each value As String In grp_pair.Value
                    informationHubEventArgs.ObjectIds.Add(grp_pair.Key)
                    informationHubEventArgs.ObjectIds.Add(value)
                Next
            Next

            informationHubEventArgs.ObjectType = KblObjectType.Wire_occurrence
        End If

        Return informationHubEventArgs
    End Function

    Private Function GetWireAdjacencyTooltipInformation(wireAdjacency As WireAdjacency) As String
        Dim color As String = String.Empty
        Dim csa As String = String.Empty
        Dim length As String = String.Empty
        Dim outsideDiameter As String = String.Empty
        Dim tooltip As String = String.Empty
        Dim wireType As String = String.Empty

        If (TypeOf wireAdjacency.CoreWireOccurrence Is Core_occurrence) Then
            With DirectCast(wireAdjacency.CoreWireOccurrence, Core_occurrence)
                If (.Length_information.Length <> 0) Then
                    For Each wireLength As Wire_length In .Length_information
                        If (wireLength.Length_type.ToLower = _defaultWireLengthType.ToLower) Then
                            length = Math.Round(wireLength.Length_value.Value_component, 2).ToString

                            Exit For
                        End If
                    Next
                End If

                tooltip = String.Format(OverallConnectivityFormStrings.WireAdja_Tooltip1, vbCrLf, .Wire_number, wireAdjacency.SignalName, length)
            End With

            If (wireAdjacency.CoreWirePart IsNot Nothing) Then
                With DirectCast(wireAdjacency.CoreWirePart, Core)
                    color = .GetColours

                    If (.Cross_section_area IsNot Nothing) Then csa = Math.Round(.Cross_section_area.Value_component, 2).ToString
                    If (.Outside_diameter IsNot Nothing) Then outsideDiameter = Math.Round(.Outside_diameter.Value_component, 2).ToString
                    If (.Wire_type IsNot Nothing) Then wireType = .Wire_type
                End With
            End If
        ElseIf (TypeOf wireAdjacency.CoreWireOccurrence Is Wire_occurrence) Then
            With DirectCast(wireAdjacency.CoreWireOccurrence, Wire_occurrence)
                If (.Length_information.Length <> 0) Then
                    For Each wireLength As Wire_length In .Length_information
                        If (wireLength.Length_type.ToLower = _defaultWireLengthType.ToLower) Then
                            length = Math.Round(wireLength.Length_value.Value_component, 2).ToString

                            Exit For
                        End If
                    Next
                End If

                tooltip = String.Format(OverallConnectivityFormStrings.WireAdja_Tooltip2, vbCrLf, .Wire_number, wireAdjacency.SignalName, length)
            End With

            If (wireAdjacency.CoreWirePart IsNot Nothing) Then
                With DirectCast(wireAdjacency.CoreWirePart, General_wire)
                    color = .GetColours

                    If (.Cross_section_area IsNot Nothing) Then csa = Math.Round(.Cross_section_area.Value_component, 2).ToString
                    If (.Outside_diameter IsNot Nothing) Then outsideDiameter = Math.Round(.Outside_diameter.Value_component, 2).ToString
                    If (.Wire_type IsNot Nothing) Then wireType = .Wire_type
                End With
            End If
        End If

        If (wireAdjacency.CoreWirePart IsNot Nothing) Then tooltip &= String.Format(OverallConnectivityFormStrings.WireAdja_Tooltip3, vbCrLf, wireType, csa, outsideDiameter, color)

        Return tooltip
    End Function

    Private Sub InitializeView()

        If (_originHarnessConnectivity.HasCavityVertex(_startingPoint.Item1) AndAlso _originHarnessConnectivity.HasWireAdjacency(_startingPoint.Item2)) Then
            Dim wireAdjacency As WireAdjacency = _originHarnessConnectivity.GetWireAdjacency(_startingPoint.Item2)
            Dim startCavityVertex As CavityVertex = _originHarnessConnectivity.GetCavityVertex(_startingPoint.Item1)
            _originWireNumber = wireAdjacency.WireNumber

            CreateConnectivityGraph(startCavityVertex)
            DrawConnectivityGraph()

            Me.vDraw.ActiveDocument.ZoomExtents()
            Me.vDraw.ActiveDocument.Invalidate()
        End If
    End Sub

    Private Sub SetVerticesGridPositions_Recursively(cavityVertex As CavityVertex, harnessIndex As Integer, ByRef processedVertices As List(Of CavityVertex))
        Dim allGridYPositionsAccepted As Boolean = False
        Dim gridXPosition As Double = cavityVertex.PositionX + GRID_X_OFFSET
        Dim isPositive As Boolean = True
        Dim negativeYOffset As Double = 0
        Dim positiveYOffset As Double = 0
        Dim sortedVertices As New SortedDictionary(Of Integer, List(Of CavityVertex))(New DescendingComparer(Of Integer))
        Dim verticesWithGridYPositions As New Dictionary(Of CavityVertex, Double)

        If (processedVertices.Count <> 0) AndAlso (processedVertices.Last.IsInliner) AndAlso (cavityVertex.IsInliner) AndAlso (processedVertices.Last.HarnessPartNumber <> cavityVertex.HarnessPartNumber) Then
            For Each adj As AdjacencyToVertex In processedVertices.Last.AdjacenciesToSuccessors
                If (adj.WireAdjacency.CoreWireOccurrence Is Nothing) Then
                    If (adj.Vertex Is cavityVertex) Then
                        harnessIndex += 1
                        Exit For
                    End If
                End If
            Next
            'harnessIndex += 1
        End If
        If (Not _harnessGroups.ContainsKey(String.Format("{0}|{1}", cavityVertex.HarnessPartNumber, cavityVertex.HarnessDescription))) Then _harnessGroups.Add(String.Format("{0}|{1}", cavityVertex.HarnessPartNumber, cavityVertex.HarnessDescription), New Dictionary(Of Integer, List(Of CavityVertex)))
        If (Not _harnessGroups(String.Format("{0}|{1}", cavityVertex.HarnessPartNumber, cavityVertex.HarnessDescription)).ContainsKey(harnessIndex)) Then _harnessGroups(String.Format("{0}|{1}", cavityVertex.HarnessPartNumber, cavityVertex.HarnessDescription)).Add(harnessIndex, New List(Of CavityVertex))

        _harnessGroups(String.Format("{0}|{1}", cavityVertex.HarnessPartNumber, cavityVertex.HarnessDescription))(harnessIndex).Add(cavityVertex)

        processedVertices.Add(cavityVertex)

        For Each wireAdjacencyToVertex As AdjacencyToVertex In cavityVertex.AdjacenciesToSuccessors
            If (Not processedVertices.Contains(wireAdjacencyToVertex.Vertex)) Then
                If (Not sortedVertices.ContainsKey(wireAdjacencyToVertex.Vertex.AdjacenciesToSuccessors.Count - 1)) Then sortedVertices.Add(wireAdjacencyToVertex.Vertex.AdjacenciesToSuccessors.Count - 1, New List(Of CavityVertex))

                sortedVertices(wireAdjacencyToVertex.Vertex.AdjacenciesToSuccessors.Count - 1).Add(wireAdjacencyToVertex.Vertex)
            End If
        Next

        For Each cavityVertexList As List(Of CavityVertex) In sortedVertices.Values
            For Each cavVertex As CavityVertex In cavityVertexList
                If (Not verticesWithGridYPositions.ContainsKey(cavVertex)) Then
                    If (cavVertex.PositionX = 0) AndAlso (cavVertex.PositionY = 0) Then
                        If (verticesWithGridYPositions.Count = 0) Then
                            verticesWithGridYPositions.Add(cavVertex, cavityVertex.PositionY)
                        Else
                            If (isPositive) Then
                                isPositive = False
                                positiveYOffset += GRID_Y_OFFSET

                                verticesWithGridYPositions.Add(cavVertex, cavityVertex.PositionY + positiveYOffset)
                            Else
                                isPositive = True
                                negativeYOffset -= GRID_Y_OFFSET

                                verticesWithGridYPositions.Add(cavVertex, cavityVertex.PositionY + negativeYOffset)
                            End If
                        End If
                    End If
                End If
            Next
        Next

        isPositive = True
        negativeYOffset = 0
        positiveYOffset = 0

        Dim verticesWithAcceptedGridYPositions As New Dictionary(Of CavityVertex, Double)

        For Each vertexWithGridYPosition As KeyValuePair(Of CavityVertex, Double) In verticesWithGridYPositions
            verticesWithAcceptedGridYPositions.Add(vertexWithGridYPosition.Key, vertexWithGridYPosition.Value)
        Next

        Do
            For Each vertexWithGridYPosition As KeyValuePair(Of CavityVertex, Double) In verticesWithAcceptedGridYPositions
                If (_occupiedGridPositions.Contains(New Tuple(Of Double, Double)(gridXPosition, vertexWithGridYPosition.Value))) Then
                    verticesWithAcceptedGridYPositions.Clear()

                    If (isPositive) Then
                        isPositive = False
                        positiveYOffset += GRID_Y_OFFSET

                        For Each vtWithGridYPosition As KeyValuePair(Of CavityVertex, Double) In verticesWithGridYPositions
                            verticesWithAcceptedGridYPositions.Add(vtWithGridYPosition.Key, vtWithGridYPosition.Value + positiveYOffset)
                        Next
                    Else
                        isPositive = True
                        negativeYOffset -= GRID_Y_OFFSET

                        For Each vtWithGridYPosition As KeyValuePair(Of CavityVertex, Double) In verticesWithGridYPositions
                            verticesWithAcceptedGridYPositions.Add(vtWithGridYPosition.Key, vtWithGridYPosition.Value + negativeYOffset)
                        Next
                    End If

                    Continue Do
                End If
            Next

            allGridYPositionsAccepted = True
        Loop While (Not allGridYPositionsAccepted)

        For Each vertexWithGridYPosition As KeyValuePair(Of CavityVertex, Double) In verticesWithAcceptedGridYPositions
            _occupiedGridPositions.Add(New Tuple(Of Double, Double)(gridXPosition, vertexWithGridYPosition.Value))

            vertexWithGridYPosition.Key.PositionX = gridXPosition
            vertexWithGridYPosition.Key.PositionY = vertexWithGridYPosition.Value
        Next

        For Each cavVertex As CavityVertex In verticesWithAcceptedGridYPositions.Keys
            SetVerticesGridPositions_Recursively(cavVertex, harnessIndex, processedVertices)
        Next
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        Using sfdExport As New SaveFileDialog
            With sfdExport
                .DefaultExt = KnownFile.DXF.Trim("."c)
                .FileName = String.Format("Overall_connectivity_for_wire_{0}", Regex.Replace(_originWireNumber, "\W", "_"))
                .Filter = "Autodesk DXF file(*.dxf)|*.dxf|JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|Bitmap (*.bmp)|*.bmp"
                .Title = OverallConnectivityFormStrings.ExportConnectivityToFile_Title

                If (.ShowDialog(Me) = DialogResult.OK) Then
                    Try
                        If (VdOpenSave.SaveAs(Me.vDraw.ActiveDocument, sfdExport.FileName)) Then
                            MessageBox.Show(OverallConnectivityFormStrings.ExportConnSuccess_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Else
                            MessageBox.Show(OverallConnectivityFormStrings.ProblemExportConn_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End If
                    Catch ex As Exception
                        MessageBox.Show(String.Format(OverallConnectivityFormStrings.ErrorExportConn_Msg, vbCrLf, ex.Message), [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Try
                End If
            End With
        End Using
    End Sub

    Private Sub btnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click
        Dim printing As Printing.VdPrinting = Nothing
        Try
            printing = New Printing.VdPrinting(Me.vDraw.ActiveDocument)
            printing.DocumentName = Me.Text
        Catch ex As Exception
            MessageBoxEx.Show(Me, String.Format(OverallConnectivityFormStrings.PrintDlgFailed_Msg, ex.Message), [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try

        If printing IsNot Nothing Then
            Using printForm As New Printing.PrintForm(printing, Nothing, False)
                printForm.ShowDialog(Me)
            End Using
        End If
    End Sub

    Private Sub btnRedraw_Click(sender As Object, e As EventArgs) Handles btnRedraw.Click
        If (Me.vDraw.ActiveDocument IsNot Nothing) Then
            Me.vDraw.ActiveDocument.ZoomExtents()
            Me.vDraw.ActiveDocument.Redraw(True)
        End If

    End Sub

    Private Sub btnSelectAll_Click(sender As Object, e As EventArgs) Handles btnSelectAll.Click
        SetEntitiySelection(GetAllRealWireEntities.ToArray)
    End Sub

    Private Sub vDraw_MouseClick(sender As Object, e As MouseEventArgs) Handles vDraw.MouseClick
        If (e.Button = System.Windows.Forms.MouseButtons.Left) Then
            Dim clickPoint As gPoint = Me.vDraw.ActiveDocument.CCS_CursorPos
            Dim hitEntity As vdFigure = Me.vDraw.ActiveDocument.ActiveLayOut.GetEntityFromPoint(Me.vDraw.ActiveDocument.ActiveRender.View2PixelI(clickPoint), 4, False, vdDocument.LockLayerMethodEnum.DisableAll)
            If hitEntity IsNot Nothing Then
                Dim adj As WireAdjacency = GetWireAdjacency(hitEntity)
                If adj Is Nothing OrElse String.IsNullOrEmpty(adj.WireNumber) Then
                    hitEntity = Nothing
                End If
            End If
            SetEntitiySelection(hitEntity)
        End If
    End Sub

    Private Function GetSelectedEntites() As vdFigure()
        Dim list As New List(Of vdFigure)
        For Each entity As vdFigure In Me.vDraw.ActiveDocument.Model.Entities
            If IsSelected(entity) Then
                list.Add(entity)
            End If
        Next
        Return list.ToArray
    End Function

    Private Function GetAllRealWireEntities() As IEnumerable(Of vdFigure)
        Return Me.GetRealWireAdjacencyEntities(Me.vDraw.ActiveDocument.ActiveLayOut.Entities.Cast(Of vdFigure))
    End Function

    Private Sub SetEntitiySelection(ParamArray entities() As vdFigure)
        Dim updateNeeded As Boolean = False

        If Not E3.Lib.DotNet.Expansions.Devices.My.Computer.Keyboard.CtrlKeyDown Then  ' HINT: clear only selection when not ctrl-key is pressed to allow selection-toggling in this mode
            Me.ResetAllWireAdjcencyColors()
            updateNeeded = True
        End If

        If entities IsNot Nothing Then
            For Each entity As vdFigure In entities
                If entity IsNot Nothing Then
                    If TryUpdateWireAdjacencyColor(entity, True) Then
                        updateNeeded = True
                    End If
                End If
            Next
        End If

        If updateNeeded Then Me.vDraw.ActiveDocument.Update()
        Me.btnCalcWireResistance.Enabled = vDraw.ActiveDocument.Model.Entities.Cast(Of vdFigure).Any(Function(vdFig) IsSelected(vdFig))
        Me.btnCalcVoltageDrop.Enabled = vDraw.ActiveDocument.Model.Entities.Cast(Of vdFigure).Any(Function(vdFig) IsSelected(vdFig))
        Dim args As InformationHubEventArgs = GetInformationEventArgs(entities)
        If (args.ObjectIds.Count > 0) Then OnOverallConnectivityViewMouseClick(args)
    End Sub

    Protected Overridable Sub OnOverallConnectivityViewMouseClick(e As InformationHubEventArgs)
        RaiseEvent OverallConnectivityViewMouseClick(Me, e)
    End Sub

    Private Sub vDraw_MouseMove(sender As Object, e As MouseEventArgs) Handles vDraw.MouseMove
        Static mpos As System.Drawing.Point
        If Not (mpos.X - e.Location.X = 0 AndAlso mpos.Y - e.Location.Y = 0) Then
            Dim currentCursorPoint As gPoint = Me.vDraw.ActiveDocument.CCS_CursorPos
            Dim hitEntity As VectorDraw.Professional.vdPrimaries.vdFigure = Me.vDraw.ActiveDocument.ActiveLayOut.GetEntityFromPoint(Me.vDraw.ActiveDocument.ActiveRender.View2PixelI(currentCursorPoint), 6, False, VectorDraw.Professional.vdObjects.vdDocument.LockLayerMethodEnum.DisableAll)

            Me.uttmOverallConnectivity.HideToolTip()

            If (hitEntity IsNot Nothing) AndAlso (hitEntity.XProperties.FindName("Tooltip") IsNot Nothing) AndAlso (hitEntity.XProperties.FindName("Tooltip").PropValue IsNot Nothing) Then
                Dim ttInfo As UltraToolTipInfo = Me.uttmOverallConnectivity.GetUltraToolTip(Me.vDraw)
                ttInfo.ToolTipText = hitEntity.XProperties.FindName("Tooltip").PropValue.ToString
                ttInfo.ToolTipImage = Infragistics.Win.ToolTipImage.Info
                ttInfo.ToolTipTitle = OverallConnectivityFormStrings.Caption_Tooltip
                ttInfo.Enabled = Infragistics.Win.DefaultableBoolean.False

                Me.uttmOverallConnectivity.ShowToolTip(Me.vDraw)
            End If
        End If
        mpos = e.Location
    End Sub
    Private Sub btnCalcVoltageDrop_Click(sender As Object, e As EventArgs) Handles btnCalcVoltageDrop.Click
        If _kblMappers Is Nothing Then
            Throw New Exception("KblMappers were not set for wire resistance calculation!")
        End If

        Dim selectedEnts As vdFigure() = GetSelectedEntites()
        Dim wireAdjacencies As List(Of WireAdjacency) = selectedEnts.Select(Function(ent) GetWireAdjacency(ent)).Where(Function(ent) ent IsNot Nothing).Distinct.ToList
        If wireAdjacencies.Count = 0 Then
            MessageBoxEx.ShowError(Me, ErrorStrings.OverallConn_NoWiresSelected)
            Return
        End If

        Dim emptyCount As UInt32 = GetEmptyConnectedAdjacenciesOf(wireAdjacencies)

        Dim overallConn As New OverallConnectivity()
        If _overallConnectivity IsNot Nothing Then
            For Each harnessCon As KeyValuePair(Of String, HarnessConnectivity) In _overallConnectivity.HarnessConnectivities
                overallConn.HarnessConnectivities.TryAdd(harnessCon.Key, harnessCon.Value)
            Next
            For Each inlineCavPair As KeyValuePair(Of String, String) In _overallConnectivity.InlinerCavityPairs
                overallConn.InlinerCavityPairs.TryAdd(inlineCavPair.Key, inlineCavPair.Value)
            Next
        End If

        Dim harnessConn As New HarnessConnectivity(_originHarnessConnectivity.HarnessPartnumber)
        For Each conn As HarnessConnectivity In overallConn.HarnessConnectivities.Values
            If conn IsNot Nothing Then
                For Each cavVert As CavityVertex In conn.CavityVertices
                    If Not harnessConn.HasCavityVertex(cavVert.Key) Then
                        harnessConn.AddCavityVertex(cavVert)
                    End If

                Next
                For Each wireAdj As WireAdjacency In conn.WireAdjacencies
                    If Not harnessConn.HasWireAdjacency(wireAdj.Key) Then
                        harnessConn.AddWireAdjacency(wireAdj)
                    End If

                Next
            End If
        Next

        If WireCurrentCalculatorForm.ShowDialog(Me, _harnessDescriptionAndColor, _originWireNumber, _kblMappers, wireAdjacencies, _defaultWireLengthType, _connectivity, _harnessesWithInactiveCoresWires, overallConn, harnessConn, emptyCount) = DialogResult.Abort Then
            MessageBoxEx.ShowError(Me, ErrorStrings.CalculationWasAbortedInternallyNotPossibleByUnknonwnReason)
        End If
    End Sub
    Private Sub btnCalcWireResistance_Click(sender As Object, e As EventArgs) Handles btnCalcWireResistance.Click
        If _kblMappers Is Nothing Then
            Throw New Exception("KblMappers were not set for wire resistance calculation!")
        End If

        Dim selectedEnts As vdFigure() = GetSelectedEntites()
        Dim wireAdjacencies As List(Of WireAdjacency) = selectedEnts.Select(Function(ent) GetWireAdjacency(ent)).Where(Function(ent) ent IsNot Nothing).Distinct.ToList
        If wireAdjacencies.Count = 0 Then
            MessageBoxEx.ShowError(Me, ErrorStrings.OverallConn_NoWiresSelected)
            Return
        End If

        Dim emptyCount As UInt32 = GetEmptyConnectedAdjacenciesOf(wireAdjacencies)
        If WireResistanceCalculatorForm.ShowDialog(Me, _kblMappers, wireAdjacencies.Select(Function(wireAdj) wireAdj.Key).ToArray, emptyCount) = DialogResult.Abort Then
            MessageBoxEx.ShowError(Me, ErrorStrings.CalculationWasAbortedInternallyNotPossibleByUnknonwnReason)
        End If
    End Sub

    Private Function GetEmptyConnectedAdjacenciesOf(selectedAdjacencies As IEnumerable(Of WireAdjacency)) As UInt32
        Dim countEmptyConnected As UInt32 = 0

        For Each emptyAdjKey As String In _allEmptyAdjacencies
            If AnyAdjacencyInAllVertices(_verticesOfAdjacencies(emptyAdjKey), selectedAdjacencies) Then
                countEmptyConnected += 1UI
            End If
        Next

        Return countEmptyConnected
    End Function

    Private Function AnyAdjacencyInAllVertices(verticesKeys As List(Of String), adjacenciesToCheckAll As IEnumerable(Of WireAdjacency)) As Boolean
        Dim foundAll As Boolean = True
        For Each vertKey As String In verticesKeys
            Dim adjOfVert As List(Of String) = _adjacenciesOfVertices(vertKey)
            If foundAll Then
                foundAll = adjacenciesToCheckAll.Any(Function(checkAdj) adjOfVert.Contains(checkAdj.Key))
            End If
            If Not foundAll Then Exit For
        Next
        Return foundAll
    End Function

    Private Sub _wStateDetector_WindowStateChanged(sender As Object, e As EventArgs) Handles _wStateDetector.WindowStateChanged
        Me.vDraw.ActiveDocument.CommandAction.Zoom("E", 0, 0)
    End Sub

    Private Sub btnCalcVoltageDrop_ChangeUICues(sender As Object, e As UICuesEventArgs) Handles btnCalcVoltageDrop.ChangeUICues

    End Sub
End Class