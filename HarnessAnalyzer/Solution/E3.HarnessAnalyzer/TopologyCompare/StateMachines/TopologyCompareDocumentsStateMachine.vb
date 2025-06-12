Imports Zuken.E3.App.Windows.Controls.Comparer.Topology
Imports Zuken.E3.Lib.Comparer.Topology.Wizard

Public Class TopologyCompareDocumentsStateMachine
    Inherits SelectFilesStateMachine

    Public Sub New(fileSelector As FileSelectorControl, control As DocumentControl)
        MyBase.New(fileSelector, control)
    End Sub

    Public Overrides ReadOnly Property Text As String
        Get
            Return My.Resources.TopologyCompareTexts.DocumentsWizardTabText
        End Get
    End Property

End Class
