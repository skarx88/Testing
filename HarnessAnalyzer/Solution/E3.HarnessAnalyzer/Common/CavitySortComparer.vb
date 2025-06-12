Public Class CavitySortComparer
    Implements IComparer(Of Cavity_occurrence)

    Private _cavityNumberComparer As New CavityNumberSortComparer
    Private _PartMapper As Dictionary(Of String, Object)

    Public Sub New(PartMapper As Dictionary(Of String, Object))
        _PartMapper = PartMapper
    End Sub

    Public Function Compare(x As Cavity_occurrence, y As Cavity_occurrence) As Integer Implements IComparer(Of Cavity_occurrence).Compare
        Dim cavX As Cavity = If(_PartMapper.ContainsKey(x.Part), DirectCast(_PartMapper(x.Part), Cavity), Nothing)
        Dim cavY As Cavity = If(_PartMapper.ContainsKey(y.Part), DirectCast(_PartMapper(y.Part), Cavity), Nothing)

        Return _cavityNumberComparer.Compare(cavX?.Cavity_number, cavY?.Cavity_number)
    End Function

End Class
