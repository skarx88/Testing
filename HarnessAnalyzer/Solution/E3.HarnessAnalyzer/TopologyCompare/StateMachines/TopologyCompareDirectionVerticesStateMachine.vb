
Public Class TopologyCompareDirectionVerticesStateMachine
    Inherits [Lib].Comparer.Topology.Wizard.DirectionVertexStateMachine

    Public Sub New(control As App.Windows.Controls.Comparer.Topology.DocumentControl)
        MyBase.New(control)
    End Sub

    Public Overrides ReadOnly Property Text As String
        Get
            Return My.Resources.TopologyCompareTexts.DirectionVerticesWizardTabText
        End Get
    End Property

End Class
