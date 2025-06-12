Public Class ModulesHubEventArgs
    Inherits EventArgs

    Public HarnessConfig As Harness_configuration
    Public IsDirty As Boolean
    Public ModuleIds As List(Of String)

    Public Sub New()

    End Sub

    Public Sub New(harnessCfg As Harness_configuration, isDrty As Boolean, modIds As List(Of String))
        HarnessConfig = harnessCfg
        IsDirty = isDrty
        ModuleIds = modIds
    End Sub

End Class
