Imports Infragistics.Win.UltraWinGrid

Friend Class WireProtectionRowFilter
    Inherits BaseChildRowFilter

    Public Sub New()
        MyBase.New
    End Sub

    Public Sub New(info As RowFilterInfo, grid As UltraGrid)
        MyBase.New(info, grid)
    End Sub

    Public Overrides Property KblObjectTypes As KblObjectType()
        Get
            Return {KblObjectType.Wire_protection_occurrence}
        End Get
        Protected Set(value As KblObjectType())
            Throw New NotSupportedException("Setting object types to WireProtectionRowFilter not supported!")
        End Set
    End Property

    Public Overrides Function DoRowFilter(row As UltraGridRow, Optional overrideKblObjectType As KblObjectType? = Nothing) As KblObjectFilterType
        If row.HasParent Then
            Return MyBase.DoRowFilter(row, overrideKblObjectType)
        End If
        Return KblObjectFilterType.Unfiltered
    End Function

End Class
