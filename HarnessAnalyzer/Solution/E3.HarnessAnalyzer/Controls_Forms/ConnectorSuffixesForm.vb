Imports Infragistics.Win.UltraWinListView

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class ConnectorSuffixesForm

    Private _originalSuffixes As New Dictionary(Of String, String)
    Private _count As Integer = -1

    Public Sub New()
        InitializeComponent()
    End Sub

    Public Shadows Function ShowDialog(owner As IWin32Window, suffixes As IEnumerable(Of String)) As DialogResult
        Init(suffixes)
        Return MyBase.ShowDialog(owner)
    End Function

    Public Shadows Sub Show(owner As IWin32Window, suffixes As IEnumerable(Of String))
        Init(suffixes)
        MyBase.ShowDialog(owner)
    End Sub

    Public Shadows Sub Show(suffixes As IEnumerable(Of String))
        Init(suffixes)
        MyBase.Show()
    End Sub

    Public Shadows Function ShowDialog(suffixes As IEnumerable(Of String)) As DialogResult
        Init(suffixes)
        Return MyBase.ShowDialog()
    End Function

    Private Sub Init(suffixes As IEnumerable(Of String))
        _count = -1
        For Each suffix As String In suffixes
            AddNewSuffix(suffix)
            _originalSuffixes.Add(_count.ToString, suffix)
        Next
    End Sub

    Public Function GetSuffixes() As IEnumerable(Of String)
        Select Case Me.DialogResult
            Case System.Windows.Forms.DialogResult.OK
                Return Me.ulvSuffixes.Items.Cast(Of UltraListViewItem).Select(Function(item) item.Value.ToString)
            Case Else
                Return _originalSuffixes.Values
        End Select
    End Function

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub btnAddNew_Click(sender As Object, e As EventArgs) Handles btnAddNew.Click
        Dim newSuffix As String = InputBox("Suffix:", String.Empty, String.Empty)
        If Not String.IsNullOrEmpty(newSuffix) Then
            ulvSuffixes.BeginUpdate()
            Dim item As UltraListViewItem = AddNewSuffix(newSuffix)
            ulvSuffixes.SelectedItems.Clear()
            ulvSuffixes.SelectedItems.Add(item)
            item.Activate()
            item.BringIntoView()
            ulvSuffixes.EndUpdate()
        End If
    End Sub

    Private Function AddNewSuffix(suffix As String) As UltraListViewItem
        _count += 1
        Return Me.ulvSuffixes.Items.Add(_count.ToString, suffix)
    End Function

    Private Sub UltraListView1_KeyDown(sender As Object, e As KeyEventArgs) Handles ulvSuffixes.KeyDown
        Select Case e.KeyCode
            Case Keys.Delete
                RemoveSelected()
        End Select
    End Sub

    Private Sub btnRemove_Click(sender As Object, e As EventArgs) Handles btnRemove.Click
        RemoveSelected()
    End Sub

    Private Sub RemoveSelected()
        For Each item As UltraListViewItem In Me.ulvSuffixes.SelectedItems.ToList
            Me.ulvSuffixes.Items.Remove(item)
        Next
    End Sub

    Private Sub ulvSuffixes_ItemSelectionChanged(sender As Object, e As ItemSelectionChangedEventArgs) Handles ulvSuffixes.ItemSelectionChanged
        btnRemove.Enabled = Me.ulvSuffixes.SelectedItems.Count > 0
    End Sub

    Private Sub RemoveToolStripMenuItem_Click(sender As Object, e As EventArgs)
        RemoveSelected()
    End Sub

    Private Sub ulvSuffixes_MouseDown(sender As Object, e As MouseEventArgs) Handles ulvSuffixes.MouseDown
        If e.Button = System.Windows.Forms.MouseButtons.Right Then
            ulvSuffixes.BeginUpdate()
            Me.ulvSuffixes.SelectedItems.Clear()
            Me.ulvSuffixes.ActiveItem = Nothing

            Dim item As UltraListViewItem = ulvSuffixes.ItemFromPoint(e.Location, True)
            If item IsNot Nothing Then
                Me.ulvSuffixes.SelectedItems.Add(item)
                item.Activate()
            End If
            ulvSuffixes.EndUpdate()
        End If
    End Sub

    Private Sub UltraToolbarsManager1_BeforeToolDropdown(sender As Object, e As Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventArgs) Handles UltraToolbarsManager1.BeforeToolDropdown
        If e.Tool.Key = "ContextMenu" Then
            Me.UltraToolbarsManager1.Tools("RemoveItemContextMenuItem").SharedProps.Enabled = Me.ulvSuffixes.SelectedItems.Count > 0
        End If
    End Sub
End Class
