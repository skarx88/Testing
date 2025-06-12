
<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class ErrorMsgWithStackTrace

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub


    Public Shared Sub ShowMessage(txt As String, stacktrace As String)
        Dim dlg As New ErrorMsgWithStackTrace
        dlg.lblMessage.Text = txt
        dlg.txtStacktrace.Text = stacktrace
        dlg.ShowDialog()
    End Sub

    Private Sub CopyAllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyAllToolStripMenuItem.Click
        If Not String.IsNullOrEmpty(Me.txtStacktrace.Text) Then
            Clipboard.SetText(Me.txtStacktrace.Text)
        End If
    End Sub
End Class
