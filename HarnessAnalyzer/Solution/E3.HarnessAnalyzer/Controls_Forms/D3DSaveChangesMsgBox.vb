<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class D3DSaveChangesMsgBox

    Public Shadows Function ShowDialog() As DialogResult
        SetTitle()
        Return MyBase.ShowDialog()
    End Function

    Public Shadows Function ShowDialog(owner As IWin32Window) As DialogResult
        SetTitle()
        Return MyBase.ShowDialog(owner)
    End Function

    Private Sub SetTitle()
        Me.Text = [Shared].MSG_BOX_TITLE
    End Sub

End Class