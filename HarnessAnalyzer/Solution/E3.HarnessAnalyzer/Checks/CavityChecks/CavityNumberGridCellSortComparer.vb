Imports Infragistics.Win.UltraWinGrid

Namespace Checks.Cavities.Views.Model
    Public Class CavityNumberGridCellSortComparer
        Implements IComparer

        Private _cavNumComparer As New CavityNumberSortComparer()

        Public Function Compare(x As Object, y As Object) As Integer Implements IComparer.Compare
            If TypeOf x Is UltraGridCell AndAlso TypeOf y Is UltraGridCell Then
                Return _cavNumComparer.Compare(CStr(CType(x, UltraGridCell).Value), CStr(CType(y, UltraGridCell).Value))
            End If

            Return Comparer.Default.Compare(x, y)
        End Function

    End Class

End Namespace
