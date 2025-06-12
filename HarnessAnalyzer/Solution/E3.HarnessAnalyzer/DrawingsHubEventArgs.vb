Imports Infragistics.Win.UltraWinTree

Public Class DrawingsHubEventArgs
    Inherits EventArgs

    Public Sub New()
    End Sub

    Public Sub New(selNode As UltraTreeNode, file As System.IO.IBaseFile)
        SelectedNode = selNode
        Me.File = file
    End Sub

    Public SelectedNode As UltraTreeNode
    Public File As System.IO.IBaseFile

End Class
