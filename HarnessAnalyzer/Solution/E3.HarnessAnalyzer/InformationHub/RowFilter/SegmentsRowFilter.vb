Imports Infragistics.Win.UltraWinGrid

Friend Class SegmentsRowFilter
    Inherits BaseRootRowFilter

    Public Sub New()
        MyBase.New
    End Sub

    Public Sub New(info As RowFilterInfo, grid As UltraGrid)
        MyBase.New(info, grid)
    End Sub

    Public Overrides Property KblObjectTypes As KblObjectType()
        Get
            Return {KblObjectType.Segment}
        End Get
        Protected Set(value As KblObjectType())
            Throw New NotSupportedException("Setting object types to SegmentsRowFilter not supported!")
        End Set
    End Property

    Protected Overrides Function FilterKblObjectCore(kblObjectType As KblObjectType, systemId As String) As KblObjectFilterType
        Dim result As KblObjectFilterType = MyBase.FilterKblObjectCore(kblObjectType, systemId)
        If result = KblObjectFilterType.Unfiltered Then
            If Not Info.ActiveSegments.Contains(systemId) AndAlso Info.SegmentsWithInactiveProtections.Contains(systemId) Then
                Return KblObjectFilterType.FilteredAndGrayOut
            End If
        End If
        Return result
    End Function

End Class
