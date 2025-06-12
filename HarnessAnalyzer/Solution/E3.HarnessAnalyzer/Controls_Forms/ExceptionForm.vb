<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class ExceptionForm

    Public Overloads Function ShowDialog(owner As IWin32Window, message As String, debugInformation As String) As DialogResult
        Me.lblError.Text = message
        Me.txtDebugInfo.Text = debugInformation

        Return Me.ShowDialog(owner)
    End Function

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Dim td As New Threading.Thread(New Threading.ThreadStart(Sub()
                                                                     If sfdException.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then
                                                                         Try
                                                                             E3.Lib.DotNet.Expansions.Devices.My.Computer.FileSystem.WriteAllText(sfdException.FileName, lblError.Text & vbCrLf & vbCrLf & Me.txtDebugInfo.Text, False)
                                                                         Catch ex As Exception
                                                                             If MsgBox(String.Format(ErrorStrings.WritingToFileError, sfdException.FileName, ex.Message), MsgBoxStyle.RetryCancel, [Shared].MSG_BOX_TITLE) = MsgBoxResult.Retry Then
                                                                                 btnSave_Click(sender, e)
                                                                             End If
                                                                         End Try
                                                                     End If

                                                                 End Sub))
        td.SetApartmentState(Threading.ApartmentState.STA)
        td.Start()
    End Sub

End Class