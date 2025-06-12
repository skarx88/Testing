Imports Infragistics.Win.UltraWinToolTip
Imports VectorDraw.Geometry
Imports VectorDraw.Professional.Constants
Imports VectorDraw.Professional.vdCollections
Imports VectorDraw.Professional.vdFigures
Imports VectorDraw.Professional.vdObjects
Imports VectorDraw.Professional.vdPrimaries

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class SelectedWireOrCoreVdDrawUserControl

    Private _kblMappers As IEnumerable(Of KblMapper)
    Private _connectivity As HarnessConnectivity ' finally to build 
    Private _selectedConnectivityOrderedInSequence As New HarnessConnectivity 'to make in correct Sequence
    Private _overallConnOriginal As HarnessConnectivity
    Private _overallConnectivity As OverallConnectivity

    Private _wireOrCoreIds() As String
    Private _occupiedGridPositions As List(Of Tuple(Of Double, Double))
    Private _startingPoint As Tuple(Of String, String)
    Private _originWireNumber As String
    Private _defaultWireLengthType As String

    Private _harnessesWithInactiveCoresWires As Dictionary(Of String, List(Of String))
    Private _harnessGroups As New Dictionary(Of String, Dictionary(Of Integer, List(Of CavityVertex)))
    Private _dicOfSytemIdAndWireOrcore As New Dictionary(Of String, Object)
    Private _dicOfCoreOrWireNumberAndCorOrWireId As New Dictionary(Of String, String)
    Private _InlinerVertexAndWireAdj As New Dictionary(Of String, WireAdjacency)
    Private _dicOfVertexAndWireAdj As New Dictionary(Of String, WireAdjacency)
    Private _dicOfVertexAndWireAdjlist As New Dictionary(Of String, List(Of WireAdjacency))
    Private _dicOfWireAdjAndVertices As New Dictionary(Of String, Dictionary(Of String, CavityVertex))
    Private _dicOfSequentialWireAdjAndVertices As New Dictionary(Of String, Dictionary(Of String, CavityVertex))
    Private _dicOfVertices As New Dictionary(Of String, CavityVertex)
    Private _vertPairWithVirtualAdjacency As New Dictionary(Of String, String)
    Private _harnessDescriptionAndColor As New Dictionary(Of String, Color)
    Private _wireAdjacenciesSequence As New List(Of WireAdjacency)
    Private _harnessIds As New List(Of String)

    Private WithEvents _uttmVoltageDrop As Infragistics.Win.UltraWinToolTip.UltraToolTipManager

    Private Const GRID_X_OFFSET As Double = 50
    Private Const GRID_Y_OFFSET As Double = 25

    Public Sub New(kblmappers As IEnumerable(Of KblMapper), harnessDescriptionAndColor As Dictionary(Of String, Color), wireAdjacencies As List(Of WireAdjacency), defaultWireLengthType As String, OverallConnectivityOriginal As HarnessConnectivity, dicOfWireAdjAndVertices As Dictionary(Of String, Dictionary(Of String, CavityVertex)), OverallConnectivity As OverallConnectivity, originHarnessConnectivity As HarnessConnectivity, harnessesWithInactiveCoresWires As Dictionary(Of String, List(Of String)))
        _kblMappers = kblmappers
        _harnessesWithInactiveCoresWires = harnessesWithInactiveCoresWires
        _overallConnectivity = OverallConnectivity
        _overallConnOriginal = OverallConnectivityOriginal
        _occupiedGridPositions = New List(Of Tuple(Of Double, Double))
        _defaultWireLengthType = defaultWireLengthType
        _dicOfWireAdjAndVertices = dicOfWireAdjAndVertices
        _harnessDescriptionAndColor = harnessDescriptionAndColor

        Init(AnalyzeAndGetTheSequenceOfWireAdjacencies(wireAdjacencies))
        InitializeComponent()
        Me.vDraw.Dock = DockStyle.Fill
        Me.vDraw.DisableVdrawDxf = False

        _uttmVoltageDrop = New UltraToolTipManager With {
            .ContainingControl = Me,
            .DisplayStyle = Infragistics.Win.ToolTipDisplayStyle.Standard
        }

        InitializeView()
    End Sub

    Public Property IsSelectionInvalidErrorOccurred As Boolean = False
    Public ReadOnly Property GetWireAdjacenciesSequence As List(Of WireAdjacency)
        Get
            Return _connectivity.WireAdjacencies.ToList
        End Get
    End Property
    ''' <summary>
    ''' Wire adjacencies in sequence along with vertices for wire adjacency
    ''' </summary>
    ''' <returns>dictionary of Wire adjacencies</returns>
    Public ReadOnly Property SequentialWireAdjacenciesWithVertices As Dictionary(Of String, Dictionary(Of String, CavityVertex))
        Get
            Return _dicOfSequentialWireAdjAndVertices
        End Get
    End Property

    ''' <summary>
    ''' Start vertex is found in order to sequenctially arrange the wire adjacencies  
    ''' </summary>
    ''' <param name="wireAdjacencies"></param>
    ''' <returns>wire adjacencies in sequence</returns>
    Private Function AnalyzeAndGetTheSequenceOfWireAdjacencies(wireAdjacencies As List(Of WireAdjacency)) As List(Of WireAdjacency)
        Dim wireAdjenciesInSequence As New List(Of WireAdjacency)
        Dim dicOfWirAdj As New Dictionary(Of String, WireAdjacency)
        Dim startVert As CavityVertex = Nothing

        'Hint: vertex pair ,which have virtual wire adjacency
        For Each wireKvp As KeyValuePair(Of String, Dictionary(Of String, CavityVertex)) In _dicOfWireAdjAndVertices
            For Each vertKvp As KeyValuePair(Of String, CavityVertex) In wireKvp.Value
                If vertKvp.Value.IsInliner Then
                    For Each adjVert As AdjacencyToVertex In vertKvp.Value.AdjacenciesToSuccessors
                        If adjVert.Vertex.IsInliner AndAlso adjVert.WireAdjacency.CoreWireOccurrence Is Nothing Then
                            If Not _vertPairWithVirtualAdjacency.ContainsKey(vertKvp.Value.Key) Then
                                _vertPairWithVirtualAdjacency.Add(vertKvp.Value.Key, adjVert.Vertex.Key)
                            End If
                            If Not _vertPairWithVirtualAdjacency.ContainsKey(adjVert.Vertex.Key) Then
                                _vertPairWithVirtualAdjacency.Add(adjVert.Vertex.Key, vertKvp.Value.Key)
                            End If
                        End If
                    Next
                End If
            Next
        Next

        If _overallConnOriginal IsNot Nothing Then
            For Each wireAdj As WireAdjacency In _overallConnOriginal.WireAdjacencies
                If startVert Is Nothing Then
                    If _dicOfWireAdjAndVertices.ContainsKey(wireAdj.Key) AndAlso _dicOfWireAdjAndVertices(wireAdj.Key).Count > 0 Then
                        For Each vertKvp As KeyValuePair(Of String, CavityVertex) In _dicOfWireAdjAndVertices(wireAdj.Key)
                            If CheckIfThisVertexHaveOneNeighbouringVertex(vertKvp.Value, wireAdj.Key) Then
                                startVert = vertKvp.Value
                                Exit For
                            End If
                        Next

                    End If
                End If
            Next
        End If

        For Each wireAdj As WireAdjacency In _overallConnOriginal.WireAdjacencies
            dicOfWirAdj.TryAdd(wireAdj.Key, wireAdj)
        Next

        If _dicOfWireAdjAndVertices.Count > 0 Then
            Dim dicOfVertAndWirAdj As New Dictionary(Of String, List(Of String))

            For Each wireAdjKvp As KeyValuePair(Of String, Dictionary(Of String, CavityVertex)) In _dicOfWireAdjAndVertices
                For Each vertexKvp As KeyValuePair(Of String, CavityVertex) In wireAdjKvp.Value
                    If Not dicOfVertAndWirAdj.ContainsKey(vertexKvp.Key) Then
                        Dim wirAdjList As New List(Of String)
                        If Not wirAdjList.Contains(wireAdjKvp.Key) Then
                            wirAdjList.Add(wireAdjKvp.Key)
                        End If
                        dicOfVertAndWirAdj.Add(vertexKvp.Key, wirAdjList)
                    Else
                        dicOfVertAndWirAdj(vertexKvp.Key).Add(wireAdjKvp.Key)
                    End If
                Next
            Next
            'Hint : For twisted pair, start vertex will be nothing, if both selected wires have same endings.
            RecursivelyAddWireAdjenciesInSequence(startVert, dicOfWirAdj, dicOfVertAndWirAdj, wireAdjenciesInSequence)
        End If

        Return wireAdjenciesInSequence
    End Function

    Private Function CheckIfThisVertexHaveOneNeighbouringVertex(vert As CavityVertex, wireAdjKey As String) As Boolean
        Dim wireAdjCount As Integer = 0
        For Each adjVert As AdjacencyToVertex In vert.AdjacenciesToSuccessors

            If adjVert.WireAdjacency IsNot Nothing AndAlso _overallConnOriginal.WireAdjacencies.Contains(adjVert.WireAdjacency) Then
                wireAdjCount += 1
            End If

            'Hint : avoid inliner as real wire adjacency
            If vert.IsInliner Then
                For Each wireadjKvp As KeyValuePair(Of String, Dictionary(Of String, CavityVertex)) In _dicOfWireAdjAndVertices
                    For Each vertKvp As KeyValuePair(Of String, CavityVertex) In wireadjKvp.Value
                        If _dicOfWireAdjAndVertices(wireAdjKey).ContainsKey(vert.Key) AndAlso _dicOfWireAdjAndVertices(wireadjKvp.Key).ContainsKey(vertKvp.Key) AndAlso vertKvp.Value.IsInliner Then
                            If _vertPairWithVirtualAdjacency.ContainsKey(vert.Key) AndAlso _vertPairWithVirtualAdjacency.ContainsKey(vertKvp.Key) Then
                                If _vertPairWithVirtualAdjacency(vert.Key) = vertKvp.Key AndAlso _vertPairWithVirtualAdjacency(vertKvp.Key) = vert.Key Then
                                    wireAdjCount += 1
                                    If wireAdjCount >= 2 Then
                                        Return If(wireAdjCount = 1, True, False)
                                    End If
                                End If
                            End If
                        End If
                    Next
                Next
            End If
        Next
        Return If(wireAdjCount = 1, True, False)
    End Function

    Private Sub RecursivelyAddWireAdjenciesInSequence(startVert As CavityVertex, dicOfWirAdj As Dictionary(Of String, WireAdjacency), dicOfVertAndWirAdj As Dictionary(Of String, List(Of String)), wireAdjenciesInSequence As List(Of WireAdjacency))
        Dim wireAdj As WireAdjacency = Nothing

        If startVert IsNot Nothing AndAlso dicOfVertAndWirAdj.ContainsKey(startVert.Key) Then

            If Not _selectedConnectivityOrderedInSequence.HasCavityVertex(startVert.Key) Then
                _selectedConnectivityOrderedInSequence.AddCavityVertex(startVert)
            End If

            For Each adjVert As AdjacencyToVertex In startVert.AdjacenciesToSuccessors
                If _dicOfWireAdjAndVertices.ContainsKey(adjVert.WireAdjacency.Key) Then
                    If Not _selectedConnectivityOrderedInSequence.HasCavityVertex(adjVert.Vertex.Key) Then
                        _selectedConnectivityOrderedInSequence.AddCavityVertex(adjVert.Vertex)
                    End If
                End If
            Next

            For Each wireAdjkey As String In dicOfVertAndWirAdj(startVert.Key)
                If dicOfWirAdj.ContainsKey(wireAdjkey) Then
                    If Not wireAdjenciesInSequence.Contains(dicOfWirAdj(wireAdjkey)) Then
                        wireAdjenciesInSequence.Add(dicOfWirAdj(wireAdjkey))
                        _selectedConnectivityOrderedInSequence.AddWireAdjacency(dicOfWirAdj(wireAdjkey))
                        wireAdj = dicOfWirAdj(wireAdjkey)
                        Exit For
                    End If
                End If
            Next
        End If

        If wireAdjenciesInSequence.Count <> _dicOfWireAdjAndVertices.Count Then
            If wireAdj IsNot Nothing AndAlso _dicOfWireAdjAndVertices.ContainsKey(wireAdj.Key) Then
                If _dicOfWireAdjAndVertices.ContainsKey(wireAdj.Key) Then
                    For Each vertKvp As KeyValuePair(Of String, CavityVertex) In _dicOfWireAdjAndVertices(wireAdj.Key)
                        RecursivelyAddWireAdjenciesInSequence(vertKvp.Value, dicOfWirAdj, dicOfVertAndWirAdj, wireAdjenciesInSequence)
                    Next
                End If
            End If

        End If
    End Sub

    Private Sub Init(wireAdjacencies As List(Of WireAdjacency))
        If wireAdjacencies.Count > 0 Then

            Dim dicOfselectedWireOrCore As New Dictionary(Of String, String)
            Dim coreOrWireIds() As String = wireAdjacencies.Select(Function(wire) If(wire.CoreWireOccurrence IsNot Nothing, wire.Key, Nothing)).ToArray

            For Each coreOrWireId As String In coreOrWireIds
                If coreOrWireId IsNot Nothing Then
                    Dim harnessId As String = HarnessConnectivity.GetHarnessFromUniqueId(coreOrWireId)
                    Dim resolvedCoreOrWireId As String = HarnessConnectivity.GetKblIdFromUniqueId(coreOrWireId)
                    Dim harnessMapper As KblMapper = _kblMappers.Where(Function(m) m.HarnessPartNumber = harnessId).FirstOrDefault

                    If (harnessMapper IsNot Nothing) Then
                        Dim coreOccurrence As Core_occurrence = TryCast(harnessMapper.KBLOccurrenceMapper(resolvedCoreOrWireId), Core_occurrence)
                        Dim wireOccurrence As Wire_occurrence = TryCast(harnessMapper.KBLOccurrenceMapper(resolvedCoreOrWireId), Wire_occurrence)
                        If coreOccurrence IsNot Nothing Then
                            If Not dicOfselectedWireOrCore.ContainsKey(coreOccurrence.SystemId) Then
                                dicOfselectedWireOrCore.Add(coreOccurrence.SystemId, coreOccurrence.Wire_number)
                            End If
                            If Not _dicOfSytemIdAndWireOrcore.ContainsKey(harnessId) Then
                                _dicOfSytemIdAndWireOrcore.Add(harnessId, coreOccurrence)
                            End If
                            If Not _dicOfCoreOrWireNumberAndCorOrWireId.ContainsKey(coreOccurrence.Wire_number) Then
                                _dicOfCoreOrWireNumberAndCorOrWireId.Add(coreOccurrence.SystemId, coreOrWireId)
                            End If
                        Else
                            If Not dicOfselectedWireOrCore.ContainsKey(wireOccurrence.SystemId) Then
                                dicOfselectedWireOrCore.Add(wireOccurrence.SystemId, wireOccurrence.Wire_number)
                            End If
                            If Not _dicOfSytemIdAndWireOrcore.ContainsKey(harnessId) Then
                                _dicOfSytemIdAndWireOrcore.Add(harnessId, wireOccurrence)
                            End If
                            If Not _dicOfCoreOrWireNumberAndCorOrWireId.ContainsKey(wireOccurrence.Wire_number) Then
                                _dicOfCoreOrWireNumberAndCorOrWireId.Add(wireOccurrence.SystemId, coreOrWireId)
                            End If
                        End If

                    End If
                End If

            Next

            For Each wireAdj As WireAdjacency In _selectedConnectivityOrderedInSequence.WireAdjacencies
                If coreOrWireIds.Length > 0 And wireAdj.CoreWireOccurrence IsNot Nothing Then
                    For Each id As String In coreOrWireIds
                        If wireAdj.Key = id Then
                            _wireAdjacenciesSequence.Add(wireAdj)
                        End If
                    Next
                End If
            Next

            coreOrWireIds = _wireAdjacenciesSequence.Select(Function(w) w.Key).ToArray
            Dim dicOfWireAdj As New Dictionary(Of String, WireAdjacency)
            _connectivity = New HarnessConnectivity

            For Each wireadj As WireAdjacency In wireAdjacencies
                If Not dicOfWireAdj.ContainsKey(wireadj.Key) AndAlso wireadj.CoreWireOccurrence IsNot Nothing Then
                    dicOfWireAdj.Add(wireadj.Key, wireadj)
                End If
            Next

            For Each v As CavityVertex In _selectedConnectivityOrderedInSequence.CavityVertices
                For Each adjacencyToVertex As AdjacencyToVertex In v.AdjacenciesToSuccessors

                    If (adjacencyToVertex.Vertex IsNot Nothing) Then
                        If adjacencyToVertex.WireAdjacency.CoreWireOccurrence Is Nothing AndAlso adjacencyToVertex.Vertex.IsInliner Then

                            If adjacencyToVertex.Vertex.IsInliner AndAlso Not _InlinerVertexAndWireAdj.ContainsKey(adjacencyToVertex.Vertex.Key) Then
                                _InlinerVertexAndWireAdj.Add(adjacencyToVertex.Vertex.Key, adjacencyToVertex.WireAdjacency)
                            End If
                            If v.IsInliner AndAlso Not _InlinerVertexAndWireAdj.ContainsKey(v.Key) Then
                                _InlinerVertexAndWireAdj.Add(v.Key, adjacencyToVertex.WireAdjacency)
                            End If

                            AddVertexAndWireAdjaciesToDictionary(v, adjacencyToVertex.WireAdjacency)
                            AddVertexAndWireAdjaciesToDictionary(adjacencyToVertex.Vertex, adjacencyToVertex.WireAdjacency)
                        Else
                            If wireAdjacencies.Where(Function(x) x.Key = adjacencyToVertex.WireAdjacency.Key).Any Then
                                AddVertexAndWireAdjaciesToDictionary(v, adjacencyToVertex.WireAdjacency)
                                AddVertexAndWireAdjaciesToDictionary(adjacencyToVertex.Vertex, adjacencyToVertex.WireAdjacency)
                            End If
                        End If
                    End If
                Next
            Next

            For Each wireadj As KeyValuePair(Of String, Dictionary(Of String, CavityVertex)) In _dicOfSequentialWireAdjAndVertices
                If dicOfWireAdj.ContainsKey(wireadj.Key) AndAlso Not _connectivity.HasWireAdjacency(wireadj.Key) Then
                    _connectivity.AddWireAdjacency(dicOfWireAdj(wireadj.Key).Clone)
                    If _dicOfSequentialWireAdjAndVertices(wireadj.Key).Count = 2 Then

                        Dim cavVer As CavityVertex = _dicOfSequentialWireAdjAndVertices(wireadj.Key).FirstOrDefault.Value
                        Dim cavVer2 As CavityVertex = _dicOfSequentialWireAdjAndVertices(wireadj.Key).LastOrDefault.Value

                        If _dicOfVertexAndWireAdjlist(cavVer.Key).Count = 1 Then
                            AddCavityVertexToconnectivity(cavVer, dicOfWireAdj(wireadj.Key).Clone)
                            AddCavityVertexToconnectivity(cavVer2, dicOfWireAdj(wireadj.Key).Clone)
                        ElseIf _dicOfVertexAndWireAdjlist(cavVer2.Key).Count = 1 Then
                            AddCavityVertexToconnectivity(cavVer2, dicOfWireAdj(wireadj.Key).Clone)
                            AddCavityVertexToconnectivity(cavVer, dicOfWireAdj(wireadj.Key).Clone)
                        Else
                            AddCavityVertexToconnectivity(cavVer, dicOfWireAdj(wireadj.Key).Clone)
                            AddCavityVertexToconnectivity(cavVer2, dicOfWireAdj(wireadj.Key).Clone)
                        End If

                        For Each v As AdjacencyToVertex In cavVer.AdjacenciesToSuccessors
                            If v.WireAdjacency.CoreWireOccurrence Is Nothing Then
                                If Not _connectivity.HasCavityVertex(cavVer.Key) Then
                                    _connectivity.AddCavityVertex(cavVer)

                                    If Not _dicOfVertexAndWireAdj.ContainsKey(cavVer.Key) Then
                                        _dicOfVertexAndWireAdj.Add(cavVer.Key, dicOfWireAdj(wireadj.Key).Clone)
                                    End If

                                End If
                            End If
                        Next

                        For Each v As AdjacencyToVertex In cavVer2.AdjacenciesToSuccessors
                            If v.WireAdjacency.CoreWireOccurrence Is Nothing Then
                                If Not _connectivity.HasCavityVertex(cavVer2.Key) Then
                                    _connectivity.AddCavityVertex(cavVer2)

                                    If Not _dicOfVertexAndWireAdj.ContainsKey(cavVer2.Key) Then
                                        _dicOfVertexAndWireAdj.Add(cavVer2.Key, dicOfWireAdj(wireadj.Key).Clone)
                                    End If

                                End If
                            End If
                        Next
                    End If
                End If
            Next

            Dim startingPoint As Tuple(Of String, String) = Nothing

            If _connectivity IsNot Nothing Then
                For Each kblmapper As KblMapper In _kblMappers
                    For Each coreOrWireId As String In coreOrWireIds
                        Dim resolvedCoreOrWireId As String = HarnessConnectivity.GetKblIdFromUniqueId(coreOrWireId)
                        Dim connection As Connection = kblmapper.GetConnections.Where(Function(con) con.Wire = resolvedCoreOrWireId).FirstOrDefault
                        If connection IsNot Nothing AndAlso startingPoint Is Nothing Then
                            Dim startingCoreWireId As String = HarnessConnectivity.GetUniqueId(kblmapper.HarnessPartNumber, resolvedCoreOrWireId)
                            Dim startCavityId As String = If(_connectivity.CavityVertices.Any(), CType(_connectivity.CavityVertices(0), CavityVertex).Key, Nothing)

                            startingPoint = New Tuple(Of String, String)(startCavityId, startingCoreWireId)
                            Exit For
                        End If
                    Next
                Next
            End If
            _wireOrCoreIds = coreOrWireIds
            _startingPoint = startingPoint
        End If
    End Sub

    Private Sub AddVertexAndWireAdjaciesToDictionary(vertex As CavityVertex, wireadj As WireAdjacency)
        vertex.PositionX = 0
        vertex.PositionY = 0

        If Not _dicOfSequentialWireAdjAndVertices.ContainsKey(wireadj.Key) Then
            Dim vertices As New Dictionary(Of String, CavityVertex)
            vertices.TryAdd(vertex.Key, vertex)
            _dicOfSequentialWireAdjAndVertices.Add(wireadj.Key, vertices)
        Else
            _dicOfSequentialWireAdjAndVertices(wireadj.Key).TryAdd(vertex.Key, vertex)
        End If
        If Not _dicOfVertexAndWireAdjlist.ContainsKey(vertex.Key) Then
            Dim wireAdjList As New List(Of WireAdjacency)

            wireAdjList.Add(wireadj)
            _dicOfVertexAndWireAdjlist.Add(vertex.Key, wireAdjList)
        Else
            If Not _dicOfVertexAndWireAdjlist(vertex.Key).Contains(wireadj) Then
                _dicOfVertexAndWireAdjlist(vertex.Key).Add(wireadj)
            End If

        End If
    End Sub
    Private Sub AddCavityVertexToconnectivity(cavVer As CavityVertex, wireadj As WireAdjacency)
        If Not _connectivity.HasCavityVertex(cavVer.Key) Then
            _connectivity.AddCavityVertex(cavVer)

            _dicOfVertexAndWireAdj.TryAdd(cavVer.Key, wireadj)
            _dicOfVertices.TryAdd(cavVer.Key, cavVer)
        End If
    End Sub

    Private Sub vDraw_Load(sender As Object, e As EventArgs) Handles vDraw.Load
        InitializeVectorDrawBaseControl()
    End Sub

    Private Sub InitializeVectorDrawBaseControl()
        Me.vDraw.AllowDrop = False
        Me.vDraw.EnsureDocument()

        With Me.vDraw.ActiveDocument
            .UndoHistory.PushEnable(False)
            .Selections.Add(New vdSelection())
            .EnableAutoFocus = True
            .EnableAutoGripOn = False
            .EnableUrls = False
            .EnableToolTips = False
            .DisableRedraw = False

            With .GlobalRenderProperties
                .AxisSize = 10
                .CrossSize = 8
                .CurveResolution = 1000
                .GridColor = Color.Black
                .SelectingCrossColor = Color.Transparent
                .SelectingWindowColor = Color.Transparent
                .TimerBreakForDraw = 500
                .OpenGlAntializing = 8
                .RenderingQuality = VectorDraw.Render.vdRender.RenderingQualityMode.HighQuality
            End With

            .GridMode = False
            .OrbitActionKey = vdDocument.KeyWithMouseStroke.None
            .OsnapDialogKey = Keys.None
            .Palette.Background = Color.White
            .ShowUCSAxis = False
            .ToolTipDispProps.UseReverseOrder = False
            .UrlActionKey = Keys.None
        End With

    End Sub
    Private Sub InitializeView()
        Me.vDraw.AllowDrop = False
        Me.vDraw.EnsureDocument()

        With Me.vDraw.ActiveDocument
            .UndoHistory.PushEnable(False)

            .DisableZoomOnResize = True
            .EnableAutoGripOn = False
            .EnableUrls = False

            With .GlobalRenderProperties
                .AxisSize = 10
                .CrossSize = 8
                .GridColor = Color.White
                .SelectingCrossColor = Color.Transparent
                .SelectingWindowColor = Color.Transparent
            End With

            .GridMode = False
            .OrbitActionKey = vdDocument.KeyWithMouseStroke.None
            .OsnapDialogKey = Keys.None
            .Palette.Background = Color.White
            .ShowUCSAxis = False
            .ToolTipDispProps.UseReverseOrder = False
            .UrlActionKey = Keys.None
        End With

        If Not CheckIfAnyErrorOccured() AndAlso _wireOrCoreIds IsNot Nothing Then
            For Each coreOrWireId As String In _wireOrCoreIds
                Dim harnessId As String = HarnessConnectivity.GetHarnessFromUniqueId(coreOrWireId)
                If Not _harnessIds.Contains(harnessId) Then
                    _harnessIds.Add(harnessId)
                End If
            Next

            If (_overallConnOriginal.HasCavityVertex(_startingPoint.Item1) AndAlso _overallConnOriginal.HasWireAdjacency(_startingPoint.Item2)) Then
                Dim wireAdjacency As WireAdjacency = _overallConnOriginal.GetWireAdjacency(_startingPoint.Item2)
                Dim startCavityVertex As CavityVertex = _overallConnOriginal.GetCavityVertex(_startingPoint.Item1)
                _originWireNumber = wireAdjacency.WireNumber

                CreateConnectivityGraph(startCavityVertex)

                Me.vDraw.ActiveDocument.ZoomExtents()
                Me.vDraw.ActiveDocument.Invalidate()
            End If
        End If
    End Sub


    Public Sub FitToPanel()
        Me.vDraw.ActiveDocument.ZoomExtents()
        Me.vDraw.ActiveDocument.Invalidate()
    End Sub

    Private Sub vDraw_MouseMove(sender As Object, e As MouseEventArgs) Handles vDraw.MouseMove
        Static mpos As System.Drawing.Point
        If Not (mpos.X - e.Location.X = 0 AndAlso mpos.Y - e.Location.Y = 0) Then
            Dim currentCursorPoint As gPoint = Me.vDraw.ActiveDocument.CCS_CursorPos
            Dim hitEntity As VectorDraw.Professional.vdPrimaries.vdFigure = Me.vDraw.ActiveDocument.ActiveLayOut.GetEntityFromPoint(Me.vDraw.ActiveDocument.ActiveRender.View2PixelI(currentCursorPoint), 6, False, VectorDraw.Professional.vdObjects.vdDocument.LockLayerMethodEnum.DisableAll)

            _uttmVoltageDrop.HideToolTip()

            If (hitEntity IsNot Nothing) AndAlso (hitEntity.XProperties.FindName("Tooltip") IsNot Nothing) AndAlso (hitEntity.XProperties.FindName("Tooltip").PropValue IsNot Nothing) Then
                Dim ttInfo As UltraToolTipInfo = _uttmVoltageDrop.GetUltraToolTip(Me.vDraw)
                ttInfo.ToolTipText = hitEntity.XProperties.FindName("Tooltip").PropValue.ToString
                ttInfo.ToolTipImage = Infragistics.Win.ToolTipImage.Info
                ttInfo.ToolTipTitle = OverallConnectivityFormStrings.Caption_Tooltip
                ttInfo.Enabled = Infragistics.Win.DefaultableBoolean.False

                _uttmVoltageDrop.ShowToolTip(Me.vDraw)
            End If
        End If
        mpos = e.Location
    End Sub
    Private Sub vDraw_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles vDraw.MouseDoubleClick
        FitToPanel()
    End Sub

    Private Sub CreateConnectivityGraph(startCavityVertex As CavityVertex)
        Dim processedVertices As New List(Of CavityVertex)

        _harnessGroups = New Dictionary(Of String, Dictionary(Of Integer, List(Of CavityVertex)))
        processedVertices.Clear()

        SetVerticesGridPositions(_connectivity.GetCavityVertex(startCavityVertex.Key), processedVertices)
        DrawConnectivityGraph()
    End Sub

    Private Function CheckIfAnyErrorOccured() As Boolean
        Dim numberOfWireAdj As Integer = 0
        For Each vert As CavityVertex In _overallConnOriginal.CavityVertices
            If _dicOfVertexAndWireAdjlist.ContainsKey(vert.Key) Then

                If _dicOfVertexAndWireAdjlist(vert.Key).Count > 2 Then
                    If vert.IsInliner Then
                        For Each wireAdj As WireAdjacency In _dicOfVertexAndWireAdjlist(vert.Key)
                            'hint : Checking only for real wireAdjacencies 
                            If wireAdj.CoreWireOccurrence IsNot Nothing Then
                                numberOfWireAdj += 1
                            End If
                        Next

                        For Each inlineVer As KeyValuePair(Of String, WireAdjacency) In _InlinerVertexAndWireAdj
                            If _dicOfVertices.ContainsKey(inlineVer.Key) Then
                                If _vertPairWithVirtualAdjacency.ContainsKey(inlineVer.Key) AndAlso _vertPairWithVirtualAdjacency.ContainsKey(vert.Key) Then
                                    If _vertPairWithVirtualAdjacency(inlineVer.Key) = vert.Key AndAlso _vertPairWithVirtualAdjacency(vert.Key) = inlineVer.Key Then
                                        numberOfWireAdj += 1
                                    End If
                                End If
                            End If
                        Next
                    Else

                        IsSelectionInvalidErrorOccurred = True
                        Exit For
                    End If
                End If
            End If
        Next
        If numberOfWireAdj > 2 Or IsSelectionInvalidErrorOccurred Then
            IsSelectionInvalidErrorOccurred = True
            MessageBox.Show(ErrorStrings.SelectedWireOrCoreVDrawUserControl_SelectionError, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
        'Hint : n number wireAdjacency(s) will have n+1 vertices , but when vertex is inliner then it will have n+2 vertices for n number of wireAdjacency(s) but there are some exceptions. 
        If Not IsSelectionInvalidErrorOccurred AndAlso _overallConnOriginal.WireAdjacencies.Count <> _overallConnOriginal.CavityVertices.Count - 1 Then
            Dim isErrorOccurred As Boolean = False
            Dim IsInlinerVertex As Boolean = False

            For Each kvp As KeyValuePair(Of String, WireAdjacency) In _InlinerVertexAndWireAdj
                If Not IsInlinerVertex Then
                    If _dicOfVertices.ContainsKey(kvp.Key) AndAlso _dicOfVertices(kvp.Key).IsInliner Then
                        For Each adjacencyToVertex As AdjacencyToVertex In _dicOfVertices(kvp.Key).AdjacenciesToSuccessors
                            If _dicOfVertices.ContainsKey(adjacencyToVertex.Vertex.Key) AndAlso adjacencyToVertex.Vertex.IsInliner Then
                                IsInlinerVertex = True
                                Exit For
                            End If
                        Next
                    End If
                End If
            Next

            If _InlinerVertexAndWireAdj.Count > 0 AndAlso IsInlinerVertex Then
                For Each wireAdj As WireAdjacency In _overallConnOriginal.WireAdjacencies
                    If Not CheckIfThisWireIsAdjacentToAnyOtherWire(wireAdj) Then
                        isErrorOccurred = True
                    End If
                Next
            Else
                isErrorOccurred = True
            End If

            If isErrorOccurred Then
                MessageBox.Show(ErrorStrings.AdjacentWireVDrawUserControl_SelectionError, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)
                IsSelectionInvalidErrorOccurred = True
            End If
        End If

        Return IsSelectionInvalidErrorOccurred
    End Function

    Private Function CheckIfThisWireIsAdjacentToAnyOtherWire(wireAdj As WireAdjacency) As Boolean
        For Each wireAdjkvp As KeyValuePair(Of String, Dictionary(Of String, CavityVertex)) In _dicOfWireAdjAndVertices
            If wireAdj.Key <> wireAdjkvp.Key Then
                For Each vertexKvp As KeyValuePair(Of String, CavityVertex) In wireAdjkvp.Value
                    If _dicOfWireAdjAndVertices.ContainsKey(wireAdj.Key) AndAlso _dicOfWireAdjAndVertices(wireAdj.Key).ContainsKey(vertexKvp.Key) Then
                        Return True
                    End If
                Next
            End If
        Next
        Return False
    End Function

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
    Private Sub SetVerticesGridPositions(cavityVertex As CavityVertex, ByRef processedVertices As List(Of CavityVertex))
        Dim gridXPosition As Double = cavityVertex.PositionX + GRID_X_OFFSET
        Dim positiveXOffset As Double = 0
        Dim IsFirstSet As Boolean = False
        Dim harnessIndex As Integer = 0
        Dim harnessUniqueId As String = String.Empty
        Dim harnessGrpId As String

        For Each wireadjKvp As KeyValuePair(Of String, Dictionary(Of String, CavityVertex)) In _dicOfSequentialWireAdjAndVertices
            Dim isVirtualAdj As Boolean = False

            For Each vertKvp As KeyValuePair(Of String, CavityVertex) In wireadjKvp.Value
                If _dicOfVertexAndWireAdj.ContainsKey(vertKvp.Key) Then
                    If _dicOfVertexAndWireAdj(vertKvp.Key).Key = wireadjKvp.Key Then

                        harnessGrpId = String.Format("{0}|{1}", vertKvp.Value.HarnessPartNumber, vertKvp.Value.HarnessDescription)

                        If String.IsNullOrEmpty(harnessUniqueId) Then
                            harnessUniqueId = harnessGrpId
                        End If

                        If harnessUniqueId <> harnessGrpId Then
                            harnessIndex += 1
                            harnessUniqueId = String.Empty
                        End If

                        If (Not _harnessGroups.ContainsKey(harnessGrpId)) Then
                            _harnessGroups.Add(harnessGrpId, New Dictionary(Of Integer, List(Of CavityVertex)))
                        End If

                        If Not _harnessGroups(harnessGrpId).ContainsKey(harnessIndex) Then
                            _harnessGroups(harnessGrpId).Add(harnessIndex, New List(Of CavityVertex))
                        End If

                        If Not _harnessGroups(harnessGrpId)(harnessIndex).Contains(vertKvp.Value) Then
                            _harnessGroups(harnessGrpId)(harnessIndex).Add(vertKvp.Value)
                        Else
                            For Each v As CavityVertex In _harnessGroups(harnessGrpId)(harnessIndex)
                                If v.Key <> vertKvp.Key Then
                                    _harnessGroups(harnessGrpId)(harnessIndex).Add(vertKvp.Value)
                                End If
                            Next
                        End If
                    End If
                End If
            Next
        Next

        For Each wireadjKvp As KeyValuePair(Of String, Dictionary(Of String, CavityVertex)) In _dicOfSequentialWireAdjAndVertices
            For Each vertKvp As KeyValuePair(Of String, CavityVertex) In wireadjKvp.Value

                If Not processedVertices.Contains(vertKvp.Value) Then

                    If (vertKvp.Value.PositionX = 0) AndAlso Not IsFirstSet Then
                        vertKvp.Value.PositionX = 0
                        IsFirstSet = True
                    Else

                        If vertKvp.Value.IsInliner Then
                            If _dicOfWireAdjAndVertices.Count > 0 AndAlso _dicOfWireAdjAndVertices.ContainsKey(wireadjKvp.Key) Then
                                If _dicOfWireAdjAndVertices(wireadjKvp.Key).ContainsKey(vertKvp.Value.Key) Then
                                    For Each vert As KeyValuePair(Of String, CavityVertex) In _dicOfWireAdjAndVertices(wireadjKvp.Key)
                                        If vertKvp.Value.PositionX = 0 Then
                                            If _vertPairWithVirtualAdjacency.ContainsKey(vert.Key) AndAlso _vertPairWithVirtualAdjacency.ContainsKey(vertKvp.Key) Then
                                                If _vertPairWithVirtualAdjacency(vert.Key) = vertKvp.Key AndAlso _vertPairWithVirtualAdjacency(vertKvp.Key) = vert.Key Then
                                                    positiveXOffset += 20
                                                    vertKvp.Value.PositionX += positiveXOffset
                                                End If
                                            End If
                                        End If
                                    Next

                                    If vertKvp.Value.PositionX = 0 Then
                                        positiveXOffset += GRID_X_OFFSET
                                        vertKvp.Value.PositionX += positiveXOffset
                                    End If

                                End If
                            End If
                        Else
                            positiveXOffset += GRID_X_OFFSET
                            vertKvp.Value.PositionX += positiveXOffset
                        End If

                    End If

                    processedVertices.Add(vertKvp.Value)
                End If
            Next
        Next
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
            Dim listOfVertices As New List(Of CavityVertex)

            For Each vertex As CavityVertex In _connectivity.CavityVertices

                For Each adjacencyToVertex As AdjacencyToVertex In vertex.AdjacenciesToSuccessors

                    If (Not drawnWireAdjacencies.Contains(adjacencyToVertex.WireAdjacency.Key)) Then
                        drawnWireAdjacencies.Add(adjacencyToVertex.WireAdjacency.Key)
                        Dim tpl As New Tuple(Of CavityVertex, CavityVertex)(vertex, adjacencyToVertex.Vertex)

                        If (adjacencyToVertex.WireAdjacency.CoreWireOccurrence Is Nothing) AndAlso _dicOfVertexAndWireAdj.ContainsKey(vertex.Key) AndAlso _dicOfVertexAndWireAdj.ContainsKey(adjacencyToVertex.Vertex.Key) Then
                            If _harnessIds.Contains(vertex.HarnessPartNumber) Then
                                virtualAdjacencies.Add(tpl)
                            End If
                        Else
                            realAdjacencies.Add(tpl)
                            If (_startingPoint.Item2 = adjacencyToVertex.WireAdjacency.Key) Then
                                startAdjacency = adjacencyToVertex.WireAdjacency
                                startAdjacencyVertices = tpl
                            Else
                                If _wireAdjacenciesSequence.Contains(adjacencyToVertex.WireAdjacency) Then
                                    DrawWireAdjacency(vertex, adjacencyToVertex.Vertex, adjacencyToVertex.WireAdjacency, adjacencyOffsets)
                                End If

                            End If
                            If Not listOfVertices.Contains(vertex) Then
                                listOfVertices.Add(vertex)
                            End If
                            If Not listOfVertices.Contains(adjacencyToVertex.Vertex) Then
                                listOfVertices.Add(adjacencyToVertex.Vertex)
                            End If
                        End If

                    End If
                Next
            Next
            'HINT Draw red start adjacency on top so that it is visible even if there are multiple wire adjacencies overlaying
            If (startAdjacency IsNot Nothing) AndAlso _wireAdjacenciesSequence.Contains(startAdjacency) Then
                DrawWireAdjacency(startAdjacencyVertices.Item1, startAdjacencyVertices.Item2, startAdjacency, adjacencyOffsets)
            End If

            virtualAdjacencies.Sort(New VertexSortComparer)
            'HINT start with shortest and detect overlay
            For Each entry As Tuple(Of CavityVertex, CavityVertex) In virtualAdjacencies
                DrawVirtualAdjacency(entry.Item1, entry.Item2, False)
            Next

            For Each vertex As CavityVertex In _connectivity.CavityVertices
                If listOfVertices.Contains(vertex) Then
                    DrawCavityVertex(vertex)
                End If

            Next
        End If
    End Sub

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
                    hatchProperties.FillColor.SystemColor = OverallConnectivityForm.Colors.VertexInliner
                Else
                    hatchProperties.FillColor.SystemColor = OverallConnectivityForm.Colors.Vertex
                End If

                .HatchProperties = hatchProperties
                .PenColor.SystemColor = OverallConnectivityForm.Colors.VertexBorder
                .Radius = 2.5
                .XProperties.Add("Tooltip").PropValue = GetCavityVertexTooltipInformation(cavityVertex)
            End With

            Me.vDraw.ActiveDocument.Model.Entities.AddItem(vertexCircle)
        ElseIf cavityVertex.ConnectorOccurrence.UsageSpecified Then
            Select Case cavityVertex.ConnectorOccurrence.Usage
                Case Connector_usage.ringterminal
                    Dim vertexTriangle As New vdPolyline
                    With vertexTriangle
                        .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                        .setDocumentDefaults()

                        .Flag = VdConstPlineFlag.PlFlagCLOSE

                        Dim hatchProperties As New vdHatchProperties(VdConstFill.VdFillModeSolid)
                        hatchProperties.SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                        hatchProperties.FillColor.SystemColor = OverallConnectivityForm.Colors.RingTerminal

                        .HatchProperties = hatchProperties
                        .PenColor.SystemColor = OverallConnectivityForm.Colors.RingTerminalBorder
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
                        hatchProperties.FillColor.SystemColor = OverallConnectivityForm.Colors.Splice

                        .HatchProperties = hatchProperties
                        .Height = 5
                        .InsertionPoint = New gPoint(cavityVertex.PositionX - 2.5, cavityVertex.PositionY - 2.5)
                        .PenColor.SystemColor = OverallConnectivityForm.Colors.SpliceBorder
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
            .PenColor.SystemColor = OverallConnectivityForm.Colors.CavityText
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
            .PenColor.SystemColor = OverallConnectivityForm.Colors.ConnectorText
            .TextString = cavityVertex.ConnectorOccurrence.Id
            .VerJustify = VdConstVerJust.VdTextVerCen
        End With

        Dim connectorTextRect As New vdRect
        With connectorTextRect
            .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
            .setDocumentDefaults()

            Dim hatchProperties As New vdHatchProperties(VdConstFill.VdFillModeSolid)
            hatchProperties.SetUnRegisterDocument(Me.vDraw.ActiveDocument)
            hatchProperties.FillColor.SystemColor = OverallConnectivityForm.Colors.ConnectorTextRect

            .HatchProperties = hatchProperties
            .Height = connectorText.BoundingBox.Height + 0.5
            .InsertionPoint = New gPoint(connectorText.BoundingBox.Min.x - 0.25, connectorText.BoundingBox.Min.y - 0.25)
            .PenColor.SystemColor = OverallConnectivityForm.Colors.ConnectorTextRect
            .Width = connectorText.BoundingBox.Width + 0.5
        End With

        Me.vDraw.ActiveDocument.Model.Entities.AddItem(connectorTextRect)
        Me.vDraw.ActiveDocument.Model.Entities.AddItem(connectorText)
    End Sub
    Private Sub DrawHarnessBoxes()
        Dim colorCount As Integer = 0
        Dim harnessColors As New List(Of Color)(OverallConnectivityForm.Colors.HarnessColors)
        Dim sortedHarnessBoxes As New SortedDictionary(Of Double, vdRect)
        Const OFFSET As Integer = 4

        For Each harnessWithGroups As KeyValuePair(Of String, Dictionary(Of Integer, List(Of CavityVertex))) In _harnessGroups
            Dim harnessBoxes As New List(Of Tuple(Of Integer, Box))

            For Each harnessGroup As KeyValuePair(Of Integer, List(Of CavityVertex)) In harnessWithGroups.Value
                Dim harnessBox As New Box

                For Each cavityVertex As CavityVertex In harnessGroup.Value
                    If _harnessIds.Contains(cavityVertex.HarnessPartNumber) Then
                        harnessBox.AddPoint(New gPoint(cavityVertex.PositionX, cavityVertex.PositionY))
                    End If

                Next
                If harnessBox.GetPoints.Count > 0 Then
                    harnessBoxes.Add(New Tuple(Of Integer, Box)(harnessGroup.Key, harnessBox))
                End If

            Next

            For Each harnessBox As Tuple(Of Integer, Box) In harnessBoxes
                Dim harnessRect As New vdRect
                With harnessRect
                    .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                    .setDocumentDefaults()

                    Dim hatchProperties As New vdHatchProperties(VdConstFill.VdFillModeSolid)
                    hatchProperties.SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                    hatchProperties.FillColor.AlphaBlending = 25

                    If _harnessDescriptionAndColor.ContainsKey(harnessWithGroups.Key) Then
                        hatchProperties.FillColor.SystemColor = _harnessDescriptionAndColor(harnessWithGroups.Key)
                    Else
                        hatchProperties.FillColor.SystemColor = harnessColors(colorCount)
                    End If


                    .HatchProperties = hatchProperties
                    .Height = harnessBox.Item2.Height + 30
                    .InsertionPoint = New gPoint(harnessBox.Item2.Min.x - 15, harnessBox.Item2.Min.y - 15)
                    .PenColor.SystemColor = OverallConnectivityForm.Colors.HarnessBoxBorder
                    .Width = harnessBox.Item2.Width + 30
                    .XProperties.Add("GroupIndex").PropValue = harnessBox.Item1
                    .XProperties.Add("Harness").PropValue = harnessWithGroups.Key
                End With

                Me.vDraw.ActiveDocument.Model.Entities.AddItem(harnessRect)

                Do While sortedHarnessBoxes.ContainsKey(harnessRect.InsertionPoint.x)
                    Me.vDraw.ActiveDocument.CommandAction.CmdMove(harnessRect, harnessRect.InsertionPoint, New gPoint(harnessRect.InsertionPoint.x + (GRID_X_OFFSET / 2), harnessRect.InsertionPoint.y))
                Loop

                sortedHarnessBoxes.Add(harnessRect.InsertionPoint.x, harnessRect)
            Next

            colorCount += 1

            If (colorCount > harnessColors.Count - 1) Then colorCount = 0
        Next

        For Each harnessRect As KeyValuePair(Of Double, vdRect) In sortedHarnessBoxes
            For Each rect As KeyValuePair(Of Double, vdRect) In sortedHarnessBoxes.Where(Function(rt) rt.Key > harnessRect.Key)
                If (harnessRect.Value.BoundingBox.Max.x >= rect.Value.BoundingBox.Min.x) Then
                    Dim groupIndex As Integer = CInt(rect.Value.XProperties.FindName("GroupIndex").PropValue)
                    Dim harness As String = rect.Value.XProperties.FindName("Harness").PropValue.ToString
                    Dim offsetX As Double = (harnessRect.Value.BoundingBox.Max.x + (GRID_X_OFFSET / 2)) - rect.Value.BoundingBox.Min.x

                    Me.vDraw.ActiveDocument.CommandAction.CmdMove(rect.Value, rect.Value.InsertionPoint, New gPoint(harnessRect.Value.BoundingBox.Max.x + (GRID_X_OFFSET / 2), rect.Value.InsertionPoint.y))

                    For Each harnessWithGroups As KeyValuePair(Of String, Dictionary(Of Integer, List(Of CavityVertex))) In _harnessGroups
                        If (harnessWithGroups.Key = harness) Then
                            For Each harnessGroup As KeyValuePair(Of Integer, List(Of CavityVertex)) In harnessWithGroups.Value
                                If (harnessGroup.Key = groupIndex) Then
                                    For Each cavityVertex As CavityVertex In harnessGroup.Value
                                        cavityVertex.PositionX += offsetX
                                    Next
                                End If
                            Next
                        End If
                    Next
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
                .PenColor.SystemColor = OverallConnectivityForm.Colors.HarnessText
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
                .PenColor.SystemColor = OverallConnectivityForm.Colors.HarnessText
                .TextString = harnessRect.XProperties.FindName("Harness").PropValue.ToString.SplitRemoveEmpty("|"c)(1)
                .VerJustify = VdConstVerJust.VdTextVerCen
            End With
            If (harnessRect.BoundingBox.Width > (2 * OFFSET) AndAlso harnessText.BoundingBox.Width > (harnessRect.BoundingBox.Width - (2 * OFFSET))) Then
                Dim ratio As Double = harnessText.BoundingBox.Width / (harnessRect.BoundingBox.Width - (2 * OFFSET))
                harnessText.Height /= ratio
            End If

            Me.vDraw.ActiveDocument.Model.Entities.AddItem(harnessText)
        Next

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
                    .PenColor.SystemColor = If(IsSelected(entity), GetAdjencyActiveOrDefaultColor(wireAdjacency), OverallConnectivityForm.Colors.WireSelected)
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
            Return .PenColor.SystemColor.ToArgb = OverallConnectivityForm.Colors.WireSelected.ToArgb
        End With
    End Function

    Private Function GetAdjencyActiveOrDefaultColor(wireAdjacency As WireAdjacency) As System.Drawing.Color
        If (_startingPoint.Item2 = wireAdjacency.Key) Then
            Return OverallConnectivityForm.Colors.WireActive
        Else
            Return OverallConnectivityForm.Colors.WireDefault
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
                    entity.PenColor.SystemColor = OverallConnectivityForm.Colors.AdjancencyTextRect
                ElseIf TypeOf entity Is vdText Then
                    entity.PenColor.SystemColor = OverallConnectivityForm.Colors.AdjancencyText
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
                .SPlineFlag = VdConstSplineFlag.SFlagSTANDARD

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
            .PenColor.SystemColor = OverallConnectivityForm.Colors.AdjancencyText
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
            hatchProperties.FillColor.SystemColor = OverallConnectivityForm.Colors.AdjancencyTextRect

            .HatchProperties = hatchProperties
            .Height = adjacencyText.BoundingBox.Height + 0.5
            .InsertionPoint = New gPoint(adjacencyText.BoundingBox.Min.x - 0.25, adjacencyText.BoundingBox.Min.y - 0.25)
            .Label = wireAdjacency.Key
            .PenColor.SystemColor = OverallConnectivityForm.Colors.AdjancencyTextRect
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
    Private Sub DrawVirtualAdjacency(cavityVertex1 As CavityVertex, cavityVertex2 As CavityVertex, addBend As Boolean)
        Dim adjacencyLine As New vdPolyline
        Dim intermediateYPosition As Double = cavityVertex1.PositionY + GRID_Y_OFFSET

        With adjacencyLine
            .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
            .setDocumentDefaults()

            .LineType = Me.vDraw.ActiveDocument.LineTypes.DPIDash
            .VertexList.Add(New gPoint(cavityVertex1.PositionX, cavityVertex1.PositionY))

            If (addBend) Then
                .SPlineFlag = VdConstSplineFlag.SFlagQUADRATIC
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

                tooltip = String.Format(OverallConnectivityFormStrings.CavVertex_Tooltip1, vbCrLf, .Id, .Description, .Usage, cavityCount)
            End With

            If (cavityVertex.ConnectorHousing IsNot Nothing) Then
                With cavityVertex.ConnectorHousing
                    If (.Housing_code IsNot Nothing) Then housingCode = .Housing_code
                    If (.Housing_colour IsNot Nothing) Then housingColor = .Housing_colour
                    If (.Housing_type IsNot Nothing) Then housingType = .Housing_type

                    tooltip &= String.Format(OverallConnectivityFormStrings.CavVertex_Tooltip2, vbCrLf, housingColor, housingCode, housingType)
                End With
            End If
        End If

        Return tooltip
    End Function


End Class
