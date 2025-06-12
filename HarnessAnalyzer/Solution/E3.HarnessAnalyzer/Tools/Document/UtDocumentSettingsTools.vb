Imports Infragistics.Win.UltraWinToolbars
Imports Zuken.E3.HarnessAnalyzer.Shared.Common

Public Class UtDocumentSettingsTools
    Inherits UtBaseTools(Of SettingsTabToolKey)

    Public Sub New(manager As UltraToolbarsManager)
        MyBase.New(manager)
    End Sub

    ReadOnly Property DisplayLogHubBtn As StateButtonTool
        Get
            Return GetSettingsTool(Of StateButtonTool)(SettingsTabToolKey.DisplayLogHub)
        End Get
    End Property

    ReadOnly Property DisplayDrawingsHubBtn As StateButtonTool
        Get
            Return GetSettingsTool(Of StateButtonTool)(SettingsTabToolKey.DisplayDrawingsHub)
        End Get
    End Property

    ReadOnly Property DisplayModulesHubBtn As StateButtonTool
        Get
            Return GetSettingsTool(Of StateButtonTool)(SettingsTabToolKey.DisplayModulesHub)
        End Get
    End Property

    ReadOnly Property DisplayInformationHubBtn As StateButtonTool
        Get
            Return GetSettingsTool(Of StateButtonTool)(SettingsTabToolKey.DisplayInformationHub)
        End Get
    End Property

    ReadOnly Property DisplayNavigatorHub As StateButtonTool
        Get
            Return GetSettingsTool(Of StateButtonTool)(SettingsTabToolKey.DisplayNavigatorHub)
        End Get
    End Property

    ReadOnly Property DisplayMemolistHub As StateButtonTool
        Get
            Return GetSettingsTool(Of StateButtonTool)(SettingsTabToolKey.DisplayMemolistHub)
        End Get
    End Property

    ReadOnly Property DisplayRedliningsBtn As StateButtonTool
        Get
            Return GetSettingsTool(Of StateButtonTool)(SettingsTabToolKey.DisplayRedlinings)
        End Get
    End Property

    ReadOnly Property DisplayQMStampsBtn As StateButtonTool
        Get
            Return GetSettingsTool(Of StateButtonTool)(SettingsTabToolKey.DisplayQMStamps)
        End Get
    End Property

    ReadOnly Property Indicators As ToolBase
        Get
            Return GetSettingsTool(Of ToolBase)(SettingsTabToolKey.Indicators)
        End Get
    End Property

    Protected Function GetSettingsTool(Of T As ToolBase)(key As SettingsTabToolKey) As T
        Return TryGetTool(Of T)(key.ToString)
    End Function

End Class