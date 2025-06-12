Imports Infragistics.Win.UltraWinToolbars

Public Class UtBaseTools(Of TKeyEnum As Structure)

    Private _manager As UltraToolbarsManager

    Public Sub New(manager As UltraToolbarsManager)
        _manager = manager
    End Sub

    Protected Function GetBaseTool(Of TTool As ToolBase)(toolkey As TKeyEnum) As TTool
        Return TryGetTool(Of TTool)(toolkey.ToString)
    End Function

    Protected Function TryGetTool(Of TTool As ToolBase)(key As String) As TTool
        If Manager.Tools.Exists(key) Then
            Return TryCast(Manager.Tools.Item(key), TTool)
        End If
        Return Nothing
    End Function

    ReadOnly Property Manager As UltraToolbarsManager
        Get
            Return _manager
        End Get
    End Property

End Class