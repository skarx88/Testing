Imports Infragistics.Win.UltraWinToolbars
Imports Zuken.E3.HarnessAnalyzer.Shared.Common

Public Class UtMainSettingsTools
    Inherits UtBaseTools(Of SettingsTabToolKey)

    Public Sub New(manager As UltraToolbarsManager)
        MyBase.New(manager)
    End Sub

    ReadOnly Property DisplayTopologyHub As ButtonTool
        Get
            Return GetBaseTool(Of ButtonTool)(SettingsTabToolKey.DisplayTopologyHub)
        End Get
    End Property

    'Public ReadOnly Property DisplayNavigatorHub As StateButtonTool
    '    Get
    '        Return GetBaseTool(Of StateButtonTool)(SettingsTabToolKey.DisplayNavigatorHub)
    '    End Get
    'End Property

    'Public ReadOnly Property DisplayMemoListHub As StateButtonTool
    '    Get
    '        Return GetBaseTool(Of StateButtonTool)(SettingsTabToolKey.DisplayMemolistHub)
    '    End Get
    'End Property

End Class