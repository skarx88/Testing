Imports System.IO.Compression

Friend Class XhcvContentViewControl

    Private _root As TreeNode

    Friend Sub New()
        InitializeComponent()
    End Sub

    Friend Sub LoadxHcvContent(xhcvStream As System.IO.Stream, name As String)
        Me.TreeView1.Nodes.Clear()
        _root = Me.TreeView1.Nodes.Add(name)

        Using za As New ZipArchive(xhcvStream, ZipArchiveMode.Read, True)
            For Each zipEntry As ZipArchiveEntry In za.Entries
                If IO.Path.GetExtension(zipEntry.Name) = ".hcv" Then
                    _root.Nodes.Add(CreateHcvNode(zipEntry.Open, zipEntry.Name))
                End If
            Next
        End Using

        _root.Expand()
    End Sub

    Friend Sub Reset()
        _root = Nothing
        Me.TreeView1.Nodes.Clear()
    End Sub

    Private Function CreateHcvNode(hcvStream As System.IO.Stream, name As String) As TreeNode
        Dim node As New TreeNode(name)
        Using za As New ZipArchive(hcvStream, ZipArchiveMode.Read, True)
            For Each entry As ZipArchiveEntry In za.Entries
                node.Nodes.Add(entry.Name)
            Next
        End Using
        Return node
    End Function

End Class
