Imports Infragistics.Win.UltraWinToolbars
Imports Zuken.E3.HarnessAnalyzer.Shared.Common

Public Class UtDocumentHomeTools
    Inherits UtBaseTools(Of HomeTabToolKey)

    Public Sub New(manager As UltraToolbarsManager)
        MyBase.New(manager)
    End Sub

    ReadOnly Property ZoomMagnifierBtn As StateButtonTool
        Get
            Return GetHomeTool(Of StateButtonTool)(HomeTabToolKey.ZoomMagnifier)
        End Get
    End Property

    ReadOnly Property PanBtn As StateButtonTool
        Get
            Return GetHomeTool(Of StateButtonTool)(HomeTabToolKey.Pan)
        End Get
    End Property

    ReadOnly Property AnalysisShowDryWetBtn As StateButtonTool
        Get
            Return GetHomeTool(Of StateButtonTool)(HomeTabToolKey.AnalysisShowDryWet)
        End Get
    End Property

    ReadOnly Property AnalysisShowEyeletsBtn As StateButtonTool
        Get
            Return GetHomeTool(Of StateButtonTool)(HomeTabToolKey.AnalysisShowEyelets)
        End Get
    End Property

    ReadOnly Property AnalysisShowPlatingMatBtn As StateButtonTool
        Get
            Return GetHomeTool(Of StateButtonTool)(HomeTabToolKey.AnalysisShowPlatingMat)
        End Get
    End Property

    ReadOnly Property AnalysisShowProtectionsBtn As StateButtonTool
        Get
            Return GetHomeTool(Of StateButtonTool)(HomeTabToolKey.AnalysisShowProtections)
        End Get
    End Property

    ReadOnly Property AnalysisShowPartnumbersBtn As StateButtonTool
        Get
            Return GetHomeTool(Of StateButtonTool)(HomeTabToolKey.AnalysisShowPartnumbers)
        End Get
    End Property

    ReadOnly Property AnalysisShowQMIssuesBtn As StateButtonTool
        Get
            Return GetHomeTool(Of StateButtonTool)(HomeTabToolKey.AnalysisShowQMIssues)
        End Get
    End Property

    ReadOnly Property AnalysisShowSplicesBtn As StateButtonTool
        Get
            Return GetHomeTool(Of StateButtonTool)(HomeTabToolKey.AnalysisShowSplices)
        End Get
    End Property

    ReadOnly Property ZoomInBtn As ButtonTool
        Get
            Return GetHomeTool(Of ButtonTool)(HomeTabToolKey.ZoomIn)
        End Get
    End Property

    ReadOnly Property ZoomOutBtn As ButtonTool
        Get
            Return GetHomeTool(Of ButtonTool)(HomeTabToolKey.ZoomOut)
        End Get
    End Property

    ReadOnly Property ValidateDataBtn As ButtonTool
        Get
            Return GetHomeTool(Of ButtonTool)(HomeTabToolKey.ValidateData)
        End Get
    End Property

    ReadOnly Property ModuleConfigManagerBtn As ButtonTool
        Get
            Return GetHomeTool(Of ButtonTool)(HomeTabToolKey.ModuleConfigManager)
        End Get
    End Property

    ReadOnly Property PasteFromClipboardBtn As ButtonTool
        Get
            Return GetHomeTool(Of ButtonTool)(HomeTabToolKey.PasteFromClipboard)
        End Get
    End Property

    ReadOnly Property BOMActive As ButtonTool
        Get
            Return GetHomeTool(Of ButtonTool)(HomeTabToolKey.BOMActive)
        End Get
    End Property

    ReadOnly Property ExportExcelActiveDataTableBtn As ButtonTool
        Get
            Return GetHomeTool(Of ButtonTool)(HomeTabToolKey.ExportExcelActiveDataTable)
        End Get
    End Property

    ReadOnly Property ExportExcelBtn As ButtonTool
        Get
            Return GetHomeTool(Of ButtonTool)(HomeTabToolKey.ExportExcel)
        End Get
    End Property

    ReadOnly Property OpenInternalModelViewerBtn() As ButtonTool
        Get
            Return GetHomeTool(Of ButtonTool)(HomeTabToolKey.OpenInternalModelViewer)
        End Get
    End Property

    ReadOnly Property RefreshBtn() As ButtonTool
        Get
            Return GetHomeTool(Of ButtonTool)(HomeTabToolKey.Refresh)
        End Get
    End Property

    Public ReadOnly Property BOMAll As ButtonTool
        Get
            Return GetHomeTool(Of ButtonTool)(HomeTabToolKey.BOMAll)
        End Get
    End Property

    Protected Function GetHomeTool(Of T As ToolBase)(key As HomeTabToolKey) As T
        Return TryGetTool(Of T)(key.ToString)
    End Function

End Class