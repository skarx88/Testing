Imports Infragistics.Win.UltraWinToolbars
Imports Zuken.E3.HarnessAnalyzer.Shared.Common

Public Class UtDocumentEditTools
    Inherits UtBaseTools(Of EditTabToolKey)

    Public Sub New(manager As UltraToolbarsManager)
        MyBase.New(manager)
    End Sub

    ReadOnly Property ImportTechnicalDataCompareResult As ButtonTool
        Get
            Return GetEditTool(Of ButtonTool)(EditTabToolKey.ImportTechnicalDataCompareResult)
        End Get
    End Property

    ReadOnly Property ImportGraphicalDataCompareResult As ButtonTool
        Get
            Return GetEditTool(Of ButtonTool)(EditTabToolKey.ImportGraphicalDataCompareResult)
        End Get
    End Property

    Protected Function GetEditTool(Of T As ToolBase)(key As EditTabToolKey) As T
        Return TryGetTool(Of T)(key.ToString)
    End Function

End Class