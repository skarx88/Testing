Namespace Checks.Cavities.Views.Model
    Public Class CavityNumberWireViewSortComparer
        Implements IComparer(Of CavityWireView)

        Private _cavNumComparer As New CavityNumberSortComparer()
        Public Function Compare(x As CavityWireView, y As CavityWireView) As Integer Implements IComparer(Of CavityWireView).Compare
            Return _cavNumComparer.Compare(x.CavityName, y.CavityName)
        End Function
    End Class
End Namespace

