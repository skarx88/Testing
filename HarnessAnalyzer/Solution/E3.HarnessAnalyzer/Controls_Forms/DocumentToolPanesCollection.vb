Imports Infragistics.Win.UltraWinDock
Imports Zuken.E3.HarnessAnalyzer.Shared.Common

Public Class DocumentToolPanesCollection
    Inherits ToolPanesCollection

    Public Sub New(udmMain As UltraDockManager)
        MyBase.New(udmMain)
    End Sub

    Public ReadOnly Property MemoListHubPane As DockableControlPane
        Get
            Return GetCommonPane(PaneKeys.MemolistHub)
        End Get
    End Property

    Public ReadOnly Property NavigatorHub As DockableControlPane
        Get
            Return GetCommonPane(PaneKeys.NavigatorHub)
        End Get
    End Property

End Class
