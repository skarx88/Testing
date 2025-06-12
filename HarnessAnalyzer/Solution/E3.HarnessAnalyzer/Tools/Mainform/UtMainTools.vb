Imports Infragistics.Win.UltraWinToolbars
Imports Zuken.E3.HarnessAnalyzer.Shared.Common

Public Class UtMainTools
    Inherits UtBaseTools(Of ToolKeys)

    Public Sub New(manager As UltraToolbarsManager)
        MyBase.New(manager)

        Me.Settings = New UtMainSettingsTools(manager)
        Me.Home = New UtMainHomeTools(manager)
        Me.Edit = New UtMainEditTools(manager)
        Me.Application = New UtApplicationMenuTools(manager)
    End Sub

    ReadOnly Property Application As UtApplicationMenuTools
    ReadOnly Property Home As UtMainHomeTools
    ReadOnly Property Settings As UtMainSettingsTools
    ReadOnly Property Edit As UtMainEditTools

End Class