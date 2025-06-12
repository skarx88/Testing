Imports Infragistics.Win.UltraWinTree
Imports Zuken.E3.Lib.IO.Files.Hcv

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class DrawingsHub

    Friend Event HubCheckStateChanged(ByVal sender As DrawingsHub, ByVal e As DrawingsHubEventArgs)
    Friend Event CancelLoad(ByVal sender As DrawingsHub, ByVal e As DrawingsHubEventArgs)

    Private _hcv As [Lib].IO.Files.Hcv.HcvFile

    Public Sub New()
        InitializeComponent()
        Me.BackColor = Color.White
    End Sub

    Friend Sub TryAddDocument3DNode(Optional drawingFile As String = "")
        Dim fileName As String = If(String.IsNullOrEmpty(drawingFile), UIStrings.Document3DPane_Caption, IO.Path.GetFileNameWithoutExtension(drawingFile)) & $" ({UIStrings.Document3DPane_Caption})"
        Dim drawing3DNode As New UltraTreeNode(DocumentForm.TAB_DOC3D_KEY, fileName)
        With drawing3DNode
            .LeftImages.Add(My.Resources.Drawing.ToBitmap)
            .Tag = New Document3DContainerFileDummy(DocumentForm.TAB_DOC3D_KEY)

            If String.IsNullOrEmpty(drawingFile) Then
                .Override.NodeAppearance.FontData.Italic = Infragistics.Win.DefaultableBoolean.True
            End If

            .Override.NodeStyle = NodeStyle.CheckBox
            .CheckedState = CheckState.Checked
        End With

        Me.utDrawings.Nodes.Add(drawing3DNode)
    End Sub

    Friend Function InitializeTree(hcv As [Lib].IO.Files.Hcv.HcvFile, Optional loadDrawings As Boolean = True) As Boolean
        Try
            _hcv = hcv
            Me.utDrawings.BeginUpdate()

            Dim drawingChecked As Boolean = False

            For Each svgFile As SvgContainerFile In hcv.OfType(Of [Lib].IO.Files.Hcv.SvgContainerFile)
                If Not TypeOf svgFile Is [Lib].IO.Files.Hcv.DraftContainerFile Then
                    Dim drawingNode As UltraTreeNode
                    If TypeOf svgFile Is [Lib].IO.Files.Hcv.DraftContainerFile Then
                        drawingNode = New UltraTreeNode(svgFile.FullName, UIStrings.DraftDrawing_Caption)
                    Else
                        drawingNode = New UltraTreeNode(svgFile.FullName, IO.Path.GetFileNameWithoutExtension(svgFile.FullName))
                    End If

                    With drawingNode
                        If Not drawingChecked AndAlso svgFile.Type = KnownContainerFileFlags.Topology AndAlso (loadDrawings) Then
                            .CheckedState = CheckState.Checked
                            drawingChecked = True
                        Else
                            .CheckedState = CheckState.Unchecked
                        End If

                        .LeftImages.Add(My.Resources.Drawing.ToBitmap)
                        .Override.NodeStyle = NodeStyle.CheckBox
                    End With

#If CONFIG = "Debug" Or DEBUG Then
                    Me.utDrawings.Nodes.Add(drawingNode)
#Else
                    If Not utDrawings.Nodes.Exists(drawingNode.Key) Then
                        Me.utDrawings.Nodes.Add(drawingNode)
                    End If
#End If
                End If
            Next

            If _hcv.OfType(Of DraftContainerFile).Any() Then
                For Each draft As [Lib].IO.Files.Hcv.DraftContainerFile In _hcv.OfType(Of [Lib].IO.Files.Hcv.DraftContainerFile)
                    AddDraftNode(draft)
                Next
            Else
                Dim draftEmpty As New DraftContainerFile()
                With AddDraftNode(draftEmpty)
                    .Override.NodeAppearance.FontData.Italic = Infragistics.Win.DefaultableBoolean.True
                End With
            End If

            Me.utDrawings.Override.SortComparer = New NumericStringSortComparer
            Me.utDrawings.Override.Sort = SortType.Ascending
            Me.utDrawings.EndUpdate()
        Catch ex As Exception
#If DEBUG Or CONFIG = "Debug" Then
            Throw
#Else
            MessageBox.Show(String.Format(DrawingsHubStrings.LoadDrawingTreeFailed_Msg, vbCrLf, ex.Message), [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)
#End If

            Me.utDrawings.EndUpdate()

            Return False
        End Try

        Return True
    End Function

    Private Function AddDraftNode(draft As DraftContainerFile) As UltraTreeNode
        Dim draftNode As New UltraTreeNode(draft.FullName, UIStrings.DraftDrawing_Caption)
        With draftNode
            .LeftImages.Add(My.Resources.Drawing.ToBitmap)
            .Override.NodeStyle = NodeStyle.CheckBox
            .Tag = draft
        End With
        Me.utDrawings.Nodes.Add(draftNode)
        Return draftNode
    End Function

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        RaiseEvent CancelLoad(Me, New DrawingsHubEventArgs())
    End Sub

    Private Sub utDrawings_AfterCheck(sender As Object, e As NodeEventArgs) Handles utDrawings.AfterCheck
        Dim file As System.IO.IBaseFile = Nothing
        If Not TypeOf e.TreeNode.Tag Is IContainerFile Then
            For Each c As SvgContainerFile In _hcv.OfType(Of SvgContainerFile)
                If c.FullName = e.TreeNode.Key Then
                    file = c
                    Exit For
                End If
            Next
        ElseIf TypeOf e.TreeNode.Tag Is SvgContainerFile Then
            file = CType(e.TreeNode.Tag, SvgContainerFile)
        ElseIf TypeOf e.TreeNode.Tag Is IContainerFile Then
            file = CType(e.TreeNode.Tag, IContainerFile)
        End If

        RaiseEvent HubCheckStateChanged(Me, New DrawingsHubEventArgs(e.TreeNode, file))
    End Sub

    Private Sub utDrawings_BeforeCheck(sender As Object, e As BeforeCheckEventArgs) Handles utDrawings.BeforeCheck
        If (e.TreeNode.LeftImages.Count > 1) Then
            e.Cancel = True
        End If
    End Sub

    Private Sub utDrawings_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles utDrawings.MouseDoubleClick
        Dim clickedNode As UltraTreeNode = Me.utDrawings.GetNodeFromPoint(e.X, e.Y)

        If (clickedNode IsNot Nothing) Then
            If (clickedNode.CheckedState = CheckState.Checked) Then
                clickedNode.CheckedState = CheckState.Unchecked
            Else
                clickedNode.CheckedState = CheckState.Checked
            End If
        End If
    End Sub


End Class
