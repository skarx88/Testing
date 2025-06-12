Imports Infragistics.Win.UltraWinToolbars
Imports Zuken.E3.HarnessAnalyzer.Shared.Common

Public Class UtDocumentTools
    Inherits UtBaseTools(Of ToolKeys)

    Public Sub New(manager As UltraToolbarsManager)
        MyBase.New(manager)

        Me.Settings = New UtDocumentSettingsTools(manager)
        Me.Home = New UtDocumentHomeTools(manager)
        Me.Edit = New UtDocumentEditTools(manager)
    End Sub

    ReadOnly Property Home As UtDocumentHomeTools
    ReadOnly Property Settings As UtDocumentSettingsTools
    ReadOnly Property Edit As UtDocumentEditTools
End Class
