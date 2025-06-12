Imports System.ComponentModel
Imports Zuken.E3.Lib.Licensing

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class ServerLicensesDialog

    Private _lock As New System.Threading.SemaphoreSlim(1)
    Private _isBusy As Boolean = False
    Private _internal As Boolean

    Property Logger As E3.Lib.Licensing.Logger
    Property Manager As E3.Lib.Licensing.LicenseManagerBase

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
        RefreshFeatures()
    End Sub

    Private Async Sub RefreshFeatures()
        Await _lock.WaitAsync()
        Me.UltraActivityIndicator1.Visible = True
        Me.ListView1.Visible = False
        _isBusy = True
        _internal = True
        Try
            Me.ListView1.Items.Clear()

            Dim borrowedLst As New HashSet(Of String)
            Dim resBorrowed As ApplicationFeatureInfosResult = Await Task.Run(Function() Manager.Borrow.GetBorrowedLicenses(Logger))
            If resBorrowed.IsSuccess Then
                borrowedLst.AddRange(resBorrowed.FeatureInfos.Select(Function(entry) entry.Name))
            End If

            Dim res As ApplicationFeatureInfosResult = Await Task.Run(Function() Manager.Status.GetAvailableFeaturesFromServer(Logger))
            If res.IsSuccess Then
                For Each ft As ApplicationFeatureInfo In res.FeatureInfos
                    Dim item As ListViewItem = Me.ListView1.Items.Add(New ListViewItem(New String() {ft.Name, ft.Version, ft.Licenses, ft.Vendor, ft.Expires}))
                    item.Tag = ft
                    item.Checked = borrowedLst.Contains(ft.Name)
                Next
                Me.ListView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent)
            End If
        Finally
            _internal = False
            Me.UltraActivityIndicator1.Visible = False
            Me.ListView1.Visible = True
            _isBusy = False
            _lock.Release()
        End Try
    End Sub

    Protected Overrides Sub OnClosing(e As CancelEventArgs)
        MyBase.OnClosing(e)
        e.Cancel = _isBusy
    End Sub

    Private Sub ContextMenuStrip1_Opening(sender As Object, e As CancelEventArgs) Handles ContextMenuStrip1.Opening
        Dim pos As System.Drawing.Point = Me.ListView1.PointToClient(MousePosition)
        Dim item As ListViewItem = Me.ListView1.GetItemAt(pos.X, pos.Y)
        If item IsNot Nothing Then
            If Not item.Selected Then
                Me.ListView1.SelectedItems.Clear()
                If item IsNot Nothing Then
                    item.Selected = True
                    item.Focused = True
                End If
            End If
        Else
            Me.ListView1.SelectedItems.Clear()
        End If
    End Sub

    Private Async Sub RemoveLicensedFeatureToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RemoveLicensedFeatureToolStripMenuItem.Click
        Await _lock.WaitAsync()
        Try
            Me.UseWaitCursor = True
            For Each item As ListViewItem In Me.ListView1.SelectedItems.Cast(Of ListViewItem).ToArray
                Await ReturnLicenseItem(item)
            Next
            Me.ListView1.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent)
        Finally
            Me.UseWaitCursor = False
            _lock.Release()
        End Try
    End Sub

    Private Async Function ReturnLicenseItem(item As ListViewItem) As Task(Of Boolean)
        Dim info As ApplicationFeatureInfo = CType(item.Tag, ApplicationFeatureInfo)
        item.Text = $"Returning ""{info.Name}"""
        If info IsNot Nothing Then
            Dim res As Result = Await Task.Run(Function() Manager.Borrow.ReturnFeatureFromAllServers(info.Name, Logger))
            If res.IsSuccess Then
                item.Text = info.Name
                item.Checked = False
                Return True
            Else
                item.Text = $"Error: {res.Message}"
            End If
        End If
        Return False
    End Function

    Private Async Sub BorrowToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BorrowToolStripMenuItem.Click
        Dim expDate As Date? = GetExpirationDateFromUser()
        If expDate.HasValue Then
            Await _lock.WaitAsync()
            Try
                Me.UseWaitCursor = True
                For Each item As ListViewItem In Me.ListView1.SelectedItems.Cast(Of ListViewItem).ToArray
                    Await BorrowItem(item, expDate.Value)
                Next
                Me.ListView1.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent)
            Finally
                Me.UseWaitCursor = False
                _lock.Release()
            End Try
        End If
    End Sub

    Private Function GetExpirationDateFromUser() As Nullable(Of Date)

        Using dlg As New SelectExpirationDateDialog
            If dlg.ShowDialog(Me) = DialogResult.Cancel Then
                Return Nothing
            End If
            Return CDate(dlg.ucalBorrow.Value)
        End Using
    End Function

    Private Async Function BorrowItem(item As ListViewItem, expDate As Date) As Task(Of Boolean)
        Dim info As ApplicationFeatureInfo = CType(item.Tag, ApplicationFeatureInfo)
        item.Text = $"Borrowing ""{info.Name}"""
        If info IsNot Nothing Then
            Dim res As Result = Await Task.Run(Function() Manager.Borrow.BorrowFeaturesOrAvailable(New String() {info.Name}, expDate, Logger))
            If res.IsSuccess Then
                item.Text = info.Name
                item.Checked = True
                Return True
            Else
                item.Text = $"Error: {res.Message}"
                item.Checked = False
            End If
        End If
        Return False
    End Function

    Private Sub Refresh_Button_Click(sender As Object, e As EventArgs) Handles Refresh_Button.Click
        RefreshFeatures()
    End Sub

    Private Sub ListView1_DoubleClick(sender As Object, e As EventArgs) Handles ListView1.DoubleClick
        Dim pos As System.Drawing.Point = Me.ListView1.PointToClient(MousePosition)
        Dim item As ListViewItem = Me.ListView1.GetItemAt(pos.X, pos.Y)
        If item IsNot Nothing Then
            My.Forms.BorrowUtility.LogView.Show(My.Forms.BorrowUtility)
        End If
    End Sub

    Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)
        MyBase.OnFormClosing(e)
        If e.CloseReason = CloseReason.UserClosing Then
            e.Cancel = True
            Me.Hide()
        End If
    End Sub

    Private Async Sub ListView1_ItemCheck(sender As Object, e As ItemCheckEventArgs) Handles ListView1.ItemCheck
        If Not _internal Then
            _internal = True
            Dim item As ListViewItem = Me.ListView1.Items(e.Index)
            Await _lock.WaitAsync()
            Try
                Me.UseWaitCursor = True
                If e.NewValue = CheckState.Unchecked Then
                    If Not Await ReturnLicenseItem(item) Then
                        If e.NewValue = CheckState.Checked Then
                            item.Checked = True
                        Else
                            item.Checked = False
                        End If
                    End If
                ElseIf e.NewValue = CheckState.Checked Then
                    Dim expDate As Date? = GetExpirationDateFromUser()
                    If expDate.HasValue Then
                        If Not Await BorrowItem(item, expDate.Value) Then
                            If e.CurrentValue = CheckState.Checked Then
                                item.Checked = True
                            Else
                                item.Checked = False
                            End If
                            e.NewValue = e.CurrentValue
                        End If
                    Else
                        If e.CurrentValue = CheckState.Checked Then
                            item.Checked = True
                        Else
                            item.Checked = False
                        End If
                        e.NewValue = e.CurrentValue
                    End If
                End If
            Finally
                Me.UseWaitCursor = False
                _lock.Release()
                _internal = False
            End Try
        End If
    End Sub

End Class
