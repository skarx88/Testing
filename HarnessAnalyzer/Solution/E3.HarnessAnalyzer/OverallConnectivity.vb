Public Class OverallConnectivity

    Private _harnessConnectivities As Concurrent.ConcurrentDictionary(Of String, HarnessConnectivity)
    Private _inlinerCavityPairs As Concurrent.ConcurrentDictionary(Of String, String)

    Public Sub New()
        _harnessConnectivities = New Concurrent.ConcurrentDictionary(Of String, HarnessConnectivity)
        _inlinerCavityPairs = New Concurrent.ConcurrentDictionary(Of String, String)
    End Sub

    Public ReadOnly Property HarnessConnectivities() As Concurrent.ConcurrentDictionary(Of String, HarnessConnectivity)
        Get
            Return _harnessConnectivities
        End Get
    End Property

    Public ReadOnly Property InlinerCavityPairs() As Concurrent.ConcurrentDictionary(Of String, String)
        Get
            Return _inlinerCavityPairs
        End Get
    End Property

End Class
