Public Class HarnessConnectivity

    Private _adjacencies As Dictionary(Of String, WireAdjacency)
    Private _vertices As Dictionary(Of String, CavityVertex)
    Private _harnessPartNumber As String = String.Empty

    Public Sub New()
        _adjacencies = New Dictionary(Of String, WireAdjacency)
        _vertices = New Dictionary(Of String, CavityVertex)
    End Sub

    Public Sub New(harnessPartnumber As String)
        MyClass.New()
        _harnessPartNumber = harnessPartnumber
    End Sub

    Public Sub AddCavityVertex(ByVal vertex As CavityVertex)
        If (Not _vertices.ContainsKey(vertex.Key)) Then _vertices.Add(vertex.Key, vertex)
    End Sub

    Public Sub AddWireAdjacency(ByVal adjacency As WireAdjacency)
        If (Not _adjacencies.ContainsKey(adjacency.Key)) Then _adjacencies.Add(adjacency.Key, adjacency)
    End Sub

    Public Sub LinkCavityVertexesViaWireAdjacency(ByVal sourceVertexKey As String, ByVal targetVertexKey As String, ByVal adjacencyKey As String)
        _vertices(sourceVertexKey).LinkToSuccessorCavityVertexViaWireAdjacency(_vertices(targetVertexKey), _adjacencies(adjacencyKey))
    End Sub

    Public ReadOnly Property HarnessPartnumber As String
        Get
            Return _harnessPartNumber
        End Get
    End Property
    Public ReadOnly Property CavityVertices As IEnumerable(Of CavityVertex)
        Get
            Return _vertices.Values
        End Get
    End Property

    Public ReadOnly Property WireAdjacencies As IEnumerable(Of WireAdjacency)
        Get
            Return _adjacencies.Values
        End Get
    End Property

    Public Function GetCavityVertex(ByVal key As String) As CavityVertex
        Return _vertices(key)
    End Function

    Public Function GetWireAdjacency(ByVal key As String) As WireAdjacency
        Return _adjacencies(key)
    End Function

    Public Function HasCavityVertex(ByVal key As String) As Boolean
        Return _vertices.ContainsKey(key)
    End Function

    Public Function HasWireAdjacency(ByVal key As String) As Boolean
        Return _adjacencies.ContainsKey(key)
    End Function

    Shared Function GetUniqueId(haness As String, kblid As String) As String
        Return String.Format("{0};{1}", haness, kblid)
    End Function
    Shared Function GetHarnessFromUniqueId(id As String) As String
        Return id.Split(";".ToCharArray, StringSplitOptions.RemoveEmptyEntries).First
    End Function
    Shared Function GetKblIdFromUniqueId(id As String) As String
        Return id.Split(";".ToCharArray, StringSplitOptions.RemoveEmptyEntries).Last
    End Function


End Class


Public Class CavityVertex

    Private _adjacenciesToSuccessors As AdjacenciesToVertex
    Private _key As String ' Cavity occurrence Id from KBL combined with harness

    Public Property CavityNumber As String
    Public Property ConnectorHousing As Connector_housing
    Public Property ConnectorOccurrence As Connector_occurrence
    Public Property HarnessDescription As String
    Public Property HarnessPartNumber As String
    Public Property IsInliner As Boolean
    Public Property PositionX As Double
    Public Property PositionY As Double

    Public Sub New(ByVal key As String)
        _adjacenciesToSuccessors = New AdjacenciesToVertex
        _key = key
    End Sub

    Public Function Clone() As CavityVertex
        Dim cavVertex As New CavityVertex(Me.Key)
        With cavVertex
            .CavityNumber = Me.CavityNumber
            .ConnectorHousing = Me.ConnectorHousing
            .ConnectorOccurrence = Me.ConnectorOccurrence
            .HarnessDescription = Me.HarnessDescription
            .HarnessPartNumber = Me.HarnessPartNumber
            .IsInliner = Me.IsInliner
            .PositionX = Me.PositionX
            .PositionY = Me.PositionY
        End With

        Return cavVertex
    End Function

    Public Sub LinkToSuccessorCavityVertexViaWireAdjacency(ByVal vertex As CavityVertex, ByVal adjacency As WireAdjacency)
        _adjacenciesToSuccessors.Add(New AdjacencyToVertex(vertex, adjacency))
    End Sub

    Public ReadOnly Property AdjacenciesToSuccessors As AdjacenciesToVertex
        Get
            Return _adjacenciesToSuccessors
        End Get
    End Property

    Public ReadOnly Property Key As String
        Get
            Return _key
        End Get
    End Property

End Class


Public Class WireAdjacency
    Private _key As String ' Core or wire occurrence Id from KBL or inliner ID combined with harness

    Public Property CoreWireOccurrence As Object
    Public Property CoreWirePart As Object
    Public Property SignalName As String
    Public Property WireNumber As String

    Public Sub New(ByVal key As String)
        _key = key
    End Sub

    Public Function Clone() As WireAdjacency
        Dim wireAdjacency As New WireAdjacency(Me.Key)
        With wireAdjacency
            .CoreWireOccurrence = Me.CoreWireOccurrence
            .CoreWirePart = Me.CoreWirePart
            .SignalName = Me.SignalName
            .WireNumber = Me.WireNumber
        End With

        Return wireAdjacency
    End Function


    Public ReadOnly Property Key As String
        Get
            Return _key
        End Get
    End Property

End Class


Public Class AdjacencyToVertex

    Private _adjacency As WireAdjacency
    Private _vertex As CavityVertex

    Public Sub New(ByVal toVertex As CavityVertex, ByVal viaAdjacency As WireAdjacency)
        _adjacency = viaAdjacency
        _vertex = toVertex
    End Sub

    Public ReadOnly Property WireAdjacency As WireAdjacency
        Get
            Return _adjacency
        End Get
    End Property

    Public ReadOnly Property Vertex As CavityVertex
        Get
            Return _vertex
        End Get
    End Property

End Class


Public Class AdjacenciesToVertex
    Inherits CollectionBase

    Public Sub Add(ByVal adjacencyToVertex As AdjacencyToVertex)
        List.Add(adjacencyToVertex)
    End Sub


End Class