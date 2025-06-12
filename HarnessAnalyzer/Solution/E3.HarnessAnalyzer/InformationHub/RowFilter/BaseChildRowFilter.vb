Imports Infragistics.Win.UltraWinGrid

Friend MustInherit Class BaseChildRowFilter
    Inherits BaseRowFilter

    Protected Sub New()
        MyBase.New
    End Sub

    Protected Sub New(info As RowFilterInfo, grid As UltraGrid, Optional disableFilteredRowsOnly As Boolean = False)
        MyBase.New(info, grid, disableFilteredRowsOnly)
    End Sub

    Protected Overrides Function CanFilterRow(row As UltraGridRow) As Boolean
        Return row.HasParent
    End Function

End Class
