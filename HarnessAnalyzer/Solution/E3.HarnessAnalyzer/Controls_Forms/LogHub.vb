Imports System.ComponentModel
Imports System.IO
Imports Infragistics.Win.UltraWinDock
Imports Infragistics.Win.UltraWinListView
Imports Zuken.E3.Lib.Toasts

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class LogHub

    Private WithEvents _connectedToastManager As LogHubToastManager

    Public Sub New()
        InitializeComponent()

        Me.BackColor = Color.White
    End Sub

    Property ConnectedToastManager As LogHubToastManager
        Get
            Return _connectedToastManager
        End Get
        Set(value As LogHubToastManager)
            If _connectedToastManager IsNot Nothing Then
                _connectedToastManager.LogHub = Nothing
            End If
            _connectedToastManager = value
            If _connectedToastManager IsNot Nothing Then
                _connectedToastManager.LogHub = Me
            End If
        End Set
    End Property

    Friend Sub WriteLogMessage(message As String, logLevel As LogEventArgs.LoggingLevel, Optional showToast As Boolean = True)
        WriteLogMessage(New LogEventArgs(logLevel, message), showToast)
    End Sub

    Friend Sub WriteLogWarning(message As String, Optional showToast As Boolean = True)
        Me.WriteLogMessage(New LogEventArgs(LogEventArgs.LoggingLevel.Warning, message))
    End Sub

    Friend Sub WriteLogMessage(e As LogEventArgs, Optional showToast As Boolean = True)
        With Me.ulvLog
            .InvokeOrDefault(
                Sub()
                    .BeginUpdate()
                    Dim item As UltraListViewItem
                    If showToast AndAlso ConnectedToastManager IsNot Nothing Then
                        item = ConnectedToastManager.AddLogMessageShowToast(e)
                    Else
                        item = GetLogItem(e)
                        .Items.Add(item)
                    End If

                    If (item.SubItems.All.Length > 0) Then
                        item.SubItems(0).Value = e.LogMessage
                    End If

                    .EndUpdate()
                End Sub)
        End With
    End Sub

    Friend Function GetLogItem(e As LogEventArgs) As UltraListViewItem
        Dim listItem As New UltraListViewItem

        Select Case e.LogLevel
            Case LogEventArgs.LoggingLevel.Error
                listItem.Appearance.Image = My.Resources.Error_Small
            Case LogEventArgs.LoggingLevel.Information
                listItem.Appearance.Image = My.Resources.About_Small
                   ' HINT: dont show toasts for information messages because this will spam the toast-manager: only the necessary informations
            Case LogEventArgs.LoggingLevel.Warning
                listItem.Appearance.Image = My.Resources.MismatchingConfig_Small
        End Select

        listItem.Value = e.LogLevel

        Return listItem
    End Function


    Private Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        Using sfdExport As New SaveFileDialog
            With sfdExport
                .Filter = "Log files (*.txt)|*.txt"
                .FileName = String.Format("{0}{1}{2}_Logtext.txt", Now.Year, Format(Now.Month, "00"), Format(Now.Day, "00"))
                .Title = "Export log messages to text file..."

                If (.ShowDialog(Me) = DialogResult.OK) Then
                    Using writer As New StreamWriter(.FileName)
                        For Each listItem As UltraListViewItem In Me.ulvLog.Items
                            writer.WriteLine(String.Format("[{0}]: {1}", listItem.Value, listItem.SubItems(0).Value))
                        Next

                        writer.Close()
                    End Using

                    MessageBox.Show(DialogStrings.ExportLogInformationSuccess_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End With
        End Using
    End Sub

    Private Sub _connectedToastManager_ToastMessageClick(sender As Object, e As ToastMessageClickEventArgs) Handles _connectedToastManager.ToastMessageClick
        Dim item As UltraListViewItem = TryCast(e.Toast.Data, UltraListViewItem)
        If item IsNot Nothing Then
            Dim manager As UltraDockManager = TryCast(TryCast(TryCast(item?.Control?.Parent, LogHub)?.Parent, DockableWindow)?.Parent, WindowDockingArea)?.Pane?.Manager
            If manager IsNot Nothing Then
                Dim pane As DockableControlPane = manager.PaneFromControl(item.Control.Parent)
                pane.Show()
                pane.Activate()
            End If

            item.Control.BeginUpdate()
            item.Control.SelectedItems.Clear()
            item.Control.SelectedItems.Add(item)
            item.BringIntoView()
            item.Activate()
            item.Control.EndUpdate()
            item.Control.Focus()
        End If
    End Sub

    Private Sub ShowToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowToolStripMenuItem.Click
        If ulvLog.ActiveItem IsNot Nothing OrElse ulvLog.SelectedItems.Count > 0 Then
            Dim showItem As UltraListViewItem = ulvLog.ActiveItem
            ShowItemMessage(showItem)
        End If
    End Sub

    Private Sub CopyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyToolStripMenuItem.Click
        If ulvLog.SelectedItems.Count > 0 Then
            Dim txt As New System.Text.StringBuilder
            For Each item As UltraListViewItem In ulvLog.SelectedItems
                txt.AppendLine(item.Text + "| " + vbTab + String.Join(";", item.SubItems.Cast(Of UltraListViewSubItem).Select(Function(child) child.Text)))
            Next
            Clipboard.SetText(txt.ToString)
        End If
    End Sub

    Private Sub ContextMenuStrip1_Opening(sender As Object, e As CancelEventArgs) Handles ContextMenuStrip1.Opening
        ShowToolStripMenuItem.Enabled = (ulvLog.SelectedItems.Count > 0) OrElse ulvLog.ActiveItem IsNot Nothing
        CopyToolStripMenuItem.Enabled = ulvLog.SelectedItems.Count > 0
    End Sub

    Private Sub ulvLog_ItemDoubleClick(sender As Object, e As ItemDoubleClickEventArgs) Handles ulvLog.ItemDoubleClick
        ShowItemMessage(e.Item)
    End Sub

    Private Sub ShowItemMessage(item As UltraListViewItem)
        If item Is Nothing Then
            item = ulvLog.SelectedItems.Cast(Of UltraListViewItem).First
        End If
        MessageBoxEx.Show(Me, item.SubItems("LogMessage").Text, MessageBoxIcon.Information)
    End Sub

    Private Sub ulvLog_MouseDown(sender As Object, e As MouseEventArgs) Handles ulvLog.MouseDown
        Dim item As UltraListViewItem = ulvLog.ItemFromPoint(e.Location)
        If item IsNot Nothing Then
            ulvLog.SelectedItems.Clear()
            ulvLog.SelectedItems.Add(item)
            ulvLog.ActiveItem = item
        End If
    End Sub

End Class
