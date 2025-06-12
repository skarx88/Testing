Imports Infragistics.Win.UltraWinToolbars
Imports Infragistics.Win.UltraWinTree

Namespace Compare.Table

    <Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
    Public Class DiffDefaultPropsWarningDialog

        Public Sub New()
            InitializeComponent()
        End Sub

        Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Close_Button.Click
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()
        End Sub

        Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Close()
        End Sub

        Private Sub UltraToolbarsManager1_ToolClick(sender As Object, e As Infragistics.Win.UltraWinToolbars.ToolClickEventArgs) Handles UltraToolbarsManager1.ToolClick
            Select Case e.Tool.Key.Parse(Of ToolKeys)
                Case ToolKeys.CollapseAll : Me.utDifferences.CollapseAll()
                Case ToolKeys.ExpandAll : Me.utDifferences.ExpandAll()
                Case ToolKeys.Copy
                    Dim txt As New System.Text.StringBuilder
                    For Each grpNode As IGrouping(Of UltraTreeNode, UltraTreeNode) In Me.utDifferences.SelectedNodes.Cast(Of UltraTreeNode).GroupBy(Function(n) n.Parent)
                        If grpNode.Key Is Nothing Then
                            For Each rootNode As UltraTreeNode In grpNode
                                txt.AppendLine(GetTextContent(rootNode))
                            Next
                        Else
                            txt.AppendLine(GetTextContent(grpNode.Key, grpNode.ToArray))
                        End If
                    Next
                    If txt.Length > 0 Then
                        E3.Lib.DotNet.Expansions.Devices.My.Computer.Clipboard.SetText(txt.ToString)
                    End If
                Case ToolKeys.SelectAll
                    utDifferences.BeginUpdate()
                    utDifferences.SelectedNodes.AddRange(utDifferences.Nodes.Cast(Of UltraTreeNode).ToArray, True)
                    utDifferences.EndUpdate()
            End Select
        End Sub

        Private Function GetTextContent(Node As UltraTreeNode, ParamArray selectNodes As UltraTreeNode()) As String
            If selectNodes.Length = 0 Then selectNodes = Node.Nodes.Cast(Of UltraTreeNode).ToArray

            Dim txt As New System.Text.StringBuilder
            txt.AppendLine(Node.Text)
            txt.AppendLine(vbTab & String.Join(vbTab, selectNodes(0).Cells.Cast(Of UltraTreeNodeCell).Select(Function(cell) cell.Column.TextResolved)))

            For Each subNode As UltraTreeNode In selectNodes
                txt.AppendLine(vbTab & String.Join(vbTab, subNode.Cells.Cast(Of UltraTreeNodeCell).Select(Function(cell) cell.Text)))
            Next
            Return txt.ToString
        End Function

        Private Sub UltraToolbarsManager1_BeforeToolDropdown(sender As Object, e As BeforeToolDropdownEventArgs) Handles UltraToolbarsManager1.BeforeToolDropdown
            Select Case e.Tool.Key.Parse(Of ToolKeys)
                Case ToolKeys.PopupMenuTool
                    With CType(e.Tool, PopupMenuTool)
                        .Tools(ToolKeys.CollapseAll.ToString).SharedProps.Enabled = Me.utDifferences.Nodes.Cast(Of UltraTreeNode).Any(Function(node) node.Expanded)
                        .Tools(ToolKeys.ExpandAll.ToString).SharedProps.Enabled = Me.utDifferences.Nodes.Cast(Of UltraTreeNode).Any(Function(node) Not node.Expanded)
                        .Tools(ToolKeys.SelectAll.ToString).SharedProps.Enabled = Me.utDifferences.Nodes.Cast(Of UltraTreeNode).Any(Function(node) Not node.Selected)
                        .Tools(ToolKeys.Copy.ToString).SharedProps.Enabled = Me.utDifferences.Nodes.Cast(Of UltraTreeNode).Any(Function(node) node.Selected OrElse node.Nodes.Cast(Of UltraTreeNode).Any(Function(subNode) subNode.Selected))
                    End With
            End Select
        End Sub

        Private Sub DiffDefaultPropsWarningDialog_Load(sender As Object, e As EventArgs) Handles Me.Load
            utDifferences.ExpandAll()
        End Sub

        Private Sub utDifferences_MouseDown(sender As Object, e As MouseEventArgs) Handles utDifferences.MouseDown
            If e.Button = MouseButtons.Right Then
                Dim node As UltraTreeNode = utDifferences.GetNodeFromPoint(e.Location)
                If node IsNot Nothing AndAlso Not node.Selected Then
                    utDifferences.SelectedNodes.AddRange(New UltraTreeNode() {node}, True)
                End If
            End If
        End Sub

        Private Enum ToolKeys
            PopupMenuTool
            CollapseAll
            ExpandAll
            SelectAll
            Copy
        End Enum

    End Class

End Namespace