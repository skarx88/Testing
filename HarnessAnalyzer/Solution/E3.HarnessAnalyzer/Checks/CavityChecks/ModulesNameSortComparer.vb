Namespace Checks.Cavities

    Public Class ModulesNameSortComparer
        Inherits NumericStringSortComparer

        Public Sub New()
            MyBase.New
        End Sub

        Public Overrides Function Compare(x As Object, y As Object) As Integer
            Dim itemX As Infragistics.Win.UltraWinListView.UltraListViewItem = TryCast(x, Infragistics.Win.UltraWinListView.UltraListViewItem)
            Dim itemY As Infragistics.Win.UltraWinListView.UltraListViewItem = TryCast(y, Infragistics.Win.UltraWinListView.UltraListViewItem)
            Return MyBase.Compare(CStr(itemX.Value), CStr(itemY.Value))
        End Function

    End Class

End Namespace