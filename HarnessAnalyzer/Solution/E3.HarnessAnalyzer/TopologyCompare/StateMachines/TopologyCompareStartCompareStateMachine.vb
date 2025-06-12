Imports Zuken.E3.App.Windows.Controls.Comparer.Topology
Imports Zuken.E3.Lib.Comparer.Topology.Wizard

Public Class TopologyCompareStartCompareStateMachine
    Inherits StartCompareStateMachine

    Public Sub New(control As DocumentControl)
        MyBase.New(control)
    End Sub

    Public Overrides ReadOnly Property Text As String
        Get
            Return My.Resources.TopologyCompareTexts.StartCompareWizardTabText
        End Get
    End Property

End Class
