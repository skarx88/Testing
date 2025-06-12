Public Class VertexSortComparer
    Implements IComparer(Of Tuple(Of CavityVertex, CavityVertex))

    Public Sub New()

    End Sub

    Public Function Compare(x As Tuple(Of CavityVertex, CavityVertex), y As Tuple(Of CavityVertex, CavityVertex)) As Integer Implements IComparer(Of Tuple(Of CavityVertex, CavityVertex)).Compare
        Dim xPair As Tuple(Of CavityVertex, CavityVertex) = DirectCast(x, Tuple(Of CavityVertex, CavityVertex))
        Dim yPair As Tuple(Of CavityVertex, CavityVertex) = DirectCast(y, Tuple(Of CavityVertex, CavityVertex))

        Return GetDistance(xPair).CompareTo(GetDistance(yPair))
    End Function

    Private Function GetDistance(vtPair As Tuple(Of CavityVertex, CavityVertex)) As Double
        Dim dx As Double = vtPair.Item1.PositionX - vtPair.Item2.PositionX
        Dim dy As Double = vtPair.Item1.PositionY - vtPair.Item2.PositionY
        Return Math.Sqrt(dx * dx + dy * dy)
    End Function
End Class