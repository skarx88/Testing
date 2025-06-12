Imports Zuken.E3.App.Windows.Controls.Comparer.Topology
Imports Zuken.E3.Lib.Comparer.Topology.Wizard

Public Class TopologyCompareStartVerticesStateMachine
    Inherits VertexPairsStateMachine

    Public Sub New(control As DocumentControl)
        MyBase.New(control)
    End Sub

    Public Overrides ReadOnly Property Text As String
        Get
            Return My.Resources.TopologyCompareTexts.StartVerticesWizardTabText
        End Get
    End Property

End Class
