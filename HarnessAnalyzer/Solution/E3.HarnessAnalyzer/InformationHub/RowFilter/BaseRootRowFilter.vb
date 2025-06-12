Imports Infragistics.Win.UltraWinGrid

Friend Class BaseRootRowFilter
    Inherits BaseRowFilter

    Public Sub New(info As RowFilterInfo, grid As UltraGrid, Optional disableFilteredRowsOnly As Boolean = False)
        MyBase.New(info, grid, disableFilteredRowsOnly)
    End Sub

    Protected Sub New()
    End Sub

    Protected Overrides Function CanFilterRow(row As UltraGridRow) As Boolean
        Return Not row.HasParent
    End Function

End Class
