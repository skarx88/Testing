Imports Infragistics.Win.UltraWinGrid

Friend Class CoresChildRowFilter
    Inherits BaseChildRowFilter

    Friend Sub New()
        Me.New(Nothing, Nothing)
    End Sub

    Protected Sub New(info As RowFilterInfo, grid As UltraGrid, Optional disableFilteredRowsOnly As Boolean = False)
        MyBase.New(info, grid, disableFilteredRowsOnly)
    End Sub

    Public Overrides Property KblObjectTypes As KblObjectType()
        Get
            Return {KblObjectType.Core_occurrence}
        End Get
        Protected Set(value As KblObjectType())
            Throw New NotSupportedException("Setting object types to ModulesRowFilter not supported!")
        End Set
    End Property

End Class
