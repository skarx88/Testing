Public Class DescendingComparer(Of T As IComparable(Of T))
    Implements IComparer(Of T)

    Public Function Compare(x As T, y As T) As Integer Implements IComparer(Of T).Compare
        Return y.CompareTo(x)
    End Function

End Class