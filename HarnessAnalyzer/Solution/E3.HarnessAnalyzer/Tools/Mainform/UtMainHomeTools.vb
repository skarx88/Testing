Imports Infragistics.Win.UltraWinToolbars
Imports Zuken.E3.HarnessAnalyzer.Shared.Common

Public Class UtMainHomeTools
    Inherits UtBaseTools(Of HomeTabToolKey)

    Public Sub New(manager As UltraToolbarsManager)
        MyBase.New(manager)
    End Sub

    ReadOnly Property CompareData As ButtonTool
        Get
            Return GetBaseTool(Of ButtonTool)(HomeTabToolKey.CompareData)
        End Get
    End Property

    ReadOnly Property CompareGraphic As ButtonTool
        Get
            Return GetBaseTool(Of ButtonTool)(HomeTabToolKey.CompareGraphic)
        End Get
    End Property

    ReadOnly Property LoadCarTransformation As ButtonTool
        Get
            Return GetBaseTool(Of ButtonTool)(HomeTabToolKey.LoadCarTransformation)
        End Get
    End Property

    ReadOnly Property SaveCarTransformation As ButtonTool
        Get
            Return GetBaseTool(Of ButtonTool)(HomeTabToolKey.SaveCarTransformation)
        End Get
    End Property

    ReadOnly Property HideNoModuleEntities As StateButtonTool
        Get
            Return GetBaseTool(Of StateButtonTool)(HomeTabToolKey.HideNoModuleEntities)
        End Get
    End Property

    ReadOnly Property BundleCalculator As ButtonTool
        Get
            Return GetBaseTool(Of ButtonTool)(HomeTabToolKey.BundleCalculator)
        End Get
    End Property

    Public ReadOnly Property Inliners As ButtonTool
        Get
            Return GetBaseTool(Of ButtonTool)(HomeTabToolKey.Inliners)
        End Get
    End Property

    Public ReadOnly Property Search As ButtonTool
        Get
            Return GetBaseTool(Of ButtonTool)(HomeTabToolKey.Search)
        End Get
    End Property

End Class