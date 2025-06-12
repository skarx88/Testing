Imports Infragistics.Win.UltraWinGrid

Friend Class ComponentRowFilter
    Inherits BaseRowFilter

    Public Sub New(info As RowFilterInfo, grid As UltraGrid, Optional disableFilteredRowsOnly As Boolean = False)
        MyBase.New(info, grid, disableFilteredRowsOnly)
    End Sub

    Protected Sub New()
        MyBase.New
    End Sub

    Public Overrides Property KblObjectTypes As KblObjectType()
        Get
            Return {KblObjectType.Component_occurrence, KblObjectType.Fuse_occurrence}
        End Get
        Protected Set(value As KblObjectType())
            Throw New NotSupportedException("Setting object types to ModulesRowFilter not supported!")
        End Set
    End Property

End Class
