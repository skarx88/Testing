Namespace Checks.Cavities.Views.Document

    Public Class ActiveModulesChangedEventArgs
        Inherits EventArgs

        Public Sub New(activeHarnessConfigurationId As String)
            MyBase.New
            Me.ActiveHarnessConfigurationId = activeHarnessConfigurationId
        End Sub

        ReadOnly Property ActiveHarnessConfigurationId As String

    End Class

End Namespace
