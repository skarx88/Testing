Imports Infragistics.Win.UltraWinToolbars
Imports Zuken.E3.HarnessAnalyzer.Shared.Common

Public Class UtApplicationMenuTools
    Inherits UtBaseTools(Of ApplicationMenuToolKey)

    Public Sub New(manager As UltraToolbarsManager)
        MyBase.New(manager)
    End Sub

    ReadOnly Property ExportLabel As LabelTool
        Get
            Return TryGetTool(Of LabelTool)(ToolKeys.ExportLabel.ToString)  ' TODO: move toolkey to ApplicationMenuTools ??
        End Get
    End Property

    ReadOnly Property ExportList As ListTool
        Get
            Return TryGetTool(Of ListTool)(ToolKeys.ExportList.ToString)  ' TODO: move toolkey to ApplicationMenuTools ??
        End Get
    End Property

    ReadOnly Property OpenDocumentMenuTool As PopupMenuTool
        Get
            Return TryGetTool(Of PopupMenuTool)(ToolKeys.Open.ToString)  ' TODO: move toolkey to ApplicationMenuTools ??
        End Get
    End Property

    ReadOnly Property RecentList As ListTool
        Get
            Return TryGetTool(Of ListTool)(ToolKeys.RecentList.ToString)  ' TODO: move toolkey to ApplicationMenuTools ??
        End Get
    End Property

    ReadOnly Property Print As ButtonTool
        Get
            Return Me.GetBaseTool(Of ButtonTool)(ApplicationMenuToolKey.Print)
        End Get
    End Property

    ReadOnly Property ExportKbl As ListToolItem
        Get
            If (Me.ExportList?.ListToolItems.Exists(ApplicationMenuToolKey.ExportKbl.ToString)).GetValueOrDefault Then
                Return Me.ExportList.ListToolItems(ApplicationMenuToolKey.ExportKbl.ToString)
            End If
            Return Nothing
        End Get
    End Property

    ReadOnly Property Export As PopupMenuTool
        Get
            Return Me.TryGetTool(Of PopupMenuTool)(ToolKeys.Export.ToString) ' TODO: move toolkey to ApplicationMenuTools ??
        End Get
    End Property

    ReadOnly Property CloseDocument As ButtonTool
        Get
            Return Me.GetBaseTool(Of ButtonTool)(ApplicationMenuToolKey.CloseDocument)
        End Get
    End Property

    ReadOnly Property CloseDocuments As ButtonTool
        Get
            Return Me.GetBaseTool(Of ButtonTool)(ApplicationMenuToolKey.CloseDocuments)
        End Get
    End Property

    ReadOnly Property RecentDocuments As PopupMenuTool
        Get
            Return DirectCast(Me.Manager.Ribbon.ApplicationMenu2010.NavigationMenu.Tools(ApplicationMenuToolKey.RecentDocuments.ToString), Infragistics.Win.UltraWinToolbars.PopupMenuTool)
        End Get
    End Property

    Public ReadOnly Property SaveDocumentAs As ButtonTool
        Get
            Return Me.GetBaseTool(Of ButtonTool)(ApplicationMenuToolKey.SaveDocumentAs)
        End Get
    End Property

    Public ReadOnly Property SaveDocument As ButtonTool
        Get
            Return Me.GetBaseTool(Of ButtonTool)(ApplicationMenuToolKey.SaveDocument)
        End Get
    End Property

    ReadOnly Property SaveDocuments() As ButtonTool
        Get
            Return Me.GetBaseTool(Of ButtonTool)(ApplicationMenuToolKey.SaveDocuments)
        End Get
    End Property

End Class
