Imports System.ComponentModel
Imports System.IO
Imports Zuken.E3.Lib.IO.Files.Hcv

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class XhcvSelectDocumentsDialog

    Private _xhcv As [Lib].IO.Files.Hcv.XhcvFile

    Private _itemColumnSorter As New ItemColumnSorter

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Public Shadows Function ShowDialog(owner As IWin32Window, xhcv As [Lib].IO.Files.Hcv.XhcvFile) As DialogResult
        _xhcv = xhcv
        Return MyBase.ShowDialog(owner)
    End Function

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
        Me.HcvDocumentsListView.ListViewItemSorter = _itemColumnSorter
        Me.Text = String.Format(Me.Text, IO.Path.GetFileName(_xhcv.FullName))
        Me.chkOpenTopology.Checked = My.Settings.XhcvDebugOpenTopoInDocument

        For Each hcv As [Lib].IO.Files.Hcv.HcvFile In _xhcv.OfType(Of [Lib].IO.Files.Hcv.HcvFile)
            Dim item As ListViewItem = New ListViewItem({IO.Path.GetFileName(hcv.FullName), Math.Round(hcv.Length / 1024 / 1024, 2).ToString})
            Me.HcvDocumentsListView.Items.Add(item)
            item.Checked = True
            item.Tag = hcv
        Next

        Me.HcvDocumentsListView.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent)
        Me.HcvDocumentsListView.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.HeaderSize)
    End Sub

    Public Function GetSelectedHcvFiles() As IEnumerable(Of [Lib].IO.Files.Hcv.HcvFile)
        Return Me.HcvDocumentsListView.CheckedItems.OfType(Of ListViewItem).Select(Function(item) CType(item.Tag, [Lib].IO.Files.Hcv.HcvFile))
    End Function

    Private Sub SelectAllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SelectAllToolStripMenuItem.Click
        SelectAllAction()
    End Sub

    Private Sub DeselectAllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeselectAllToolStripMenuItem.Click
        UncheckAllAction()
    End Sub

    Private Sub SelectToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CheckSelectedToolStripMenuItem.Click
        CheckSelectionOnlyAction()
    End Sub

    Private Sub HcvDocumentsListView_KeyDown(sender As Object, e As KeyEventArgs) Handles HcvDocumentsListView.KeyDown
        If e.Control Then
            If e.KeyCode = Keys.A Then
                SelectAllAction()
            End If
        End If
    End Sub

    Private Sub CheckSelectionOnlyAction()
        For Each item As ListViewItem In Me.HcvDocumentsListView.Items
            item.Checked = item.Selected
        Next
    End Sub

    Private Sub UncheckAllAction()
        For Each item As ListViewItem In Me.HcvDocumentsListView.Items
            item.Checked = False
        Next
    End Sub

    Private Sub SelectAllAction()
        For Each item As ListViewItem In Me.HcvDocumentsListView.Items
            item.Selected = True
        Next
    End Sub

    Private Sub btnRemoveFromDebug_Click(sender As Object, e As EventArgs) Handles btnRemoveFromDebug.Click
        If MessageBoxEx.ShowQuestion("Do you really want to remove this dialog permanently and execute the standard release code from now on?") = DialogResult.Yes Then
            Me.DialogResult = DialogResult.Continue
            Me.Close()
        End If
    End Sub

    Private Sub OpenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenToolStripMenuItem.Click
        For Each item As ListViewItem In Me.HcvDocumentsListView.SelectedItems
            Dim hcv As HcvFile = CType(item.Tag, HcvFile)
            Dim tempFile As String = IO.Path.Combine(IO.Path.GetTempPath, IO.Path.GetFileName(hcv.FullName))
            Try
                Using fs As New FileStream(tempFile, FileMode.Create)
                    hcv.SaveTo(fs)
                End Using
            Catch ex As Exception
#If DEBUG Or CONFIG = "Debug" Then
                Throw
#Else
                ex.ShowMessageBox
#End If
                Return
            End Try

            ProcessEx.Start(tempFile)
        Next
    End Sub

    Private Sub ContextMenuStrip1_Opening(sender As Object, e As CancelEventArgs) Handles ContextMenuStrip1.Opening
        Me.OpenToolStripMenuItem.Enabled = Me.HcvDocumentsListView.SelectedItems.Count > 0
    End Sub

    Private Sub HcvDocumentsListView_ColumnClick(sender As Object, e As ColumnClickEventArgs) Handles HcvDocumentsListView.ColumnClick
        _itemColumnSorter.SortColumn = e.Column
        If _itemColumnSorter.Sorting = SortOrder.Ascending Then
            _itemColumnSorter.Sorting = SortOrder.Descending
        Else
            _itemColumnSorter.Sorting = SortOrder.Ascending
        End If
        Me.HcvDocumentsListView.Sort()
    End Sub

    ReadOnly Property OpenDrawings As Boolean
        Get
            Return chkOpenTopology.Checked
        End Get
    End Property

    Protected Overrides Sub OnClosing(e As CancelEventArgs)
        MyBase.OnClosing(e)
        If Not e.Cancel Then
            My.Settings.XhcvDebugOpenTopoInDocument = Me.chkOpenTopology.Checked
        End If
    End Sub

    Private Class ItemColumnSorter
        Inherits Comparer(Of ListViewItem)

        Property SortColumn As Integer
        Property Sorting As SortOrder

        Public Overrides Function Compare(x As ListViewItem, y As ListViewItem) As Integer
            If SortColumn > -1 Then
                Dim sx As String = x.SubItems(SortColumn).Text
                Dim sy As String = y.SubItems(SortColumn).Text
                If IsNumeric(sx) AndAlso IsNumeric(sy) Then
                    Dim dx As Double = Double.Parse(sx)
                    Dim dy As Double = Double.Parse(sy)
                    If Sorting = SortOrder.Descending Then
                        Return dx.CompareTo(dy)
                    Else
                        Return dy.CompareTo(dx)
                    End If
                Else
                    If Sorting = SortOrder.Descending Then
                        Return sx.CompareTo(sy)
                    Else
                        Return sy.CompareTo(sx)
                    End If
                End If
            End If
            Return String.Compare(x.Text, y.Text)
        End Function
    End Class

End Class
