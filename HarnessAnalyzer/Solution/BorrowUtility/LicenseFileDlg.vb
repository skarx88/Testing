Imports Zuken.E3.HarnessAnalyzer.Shared
Imports Zuken.E3.Lib.Licensing

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class LicenseFileDlg

    Property Manager As [Shared].LicenseManager

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
        Me.TextBox1.Text = GetLicenseFilePath()
        Me.Info_EnvVars_Button.Visible = EnvVarsDialog.GetEnvVariables(Me.Manager).Count > 1
    End Sub

    Public Function GetLicenseFilePath() As String
        Dim dic As Dictionary(Of String, String) = EnvVarsDialog.GetEnvVariables(Me.Manager)
        If dic.ContainsKey(Manager.EnvironmentVariables.First) Then
            Return dic(Manager.EnvironmentVariables.First)
        End If

        If dic.Count > 0 Then
            Return dic.Values.First
        End If
        Return Nothing
    End Function

    Protected Overrides Sub OnVisibleChanged(e As EventArgs)
        MyBase.OnVisibleChanged(e)
        If Visible Then
            Try
                If ShowOpenFileOnOpen Then
                    ShowOpenFileDialog()
                End If
            Finally
                ShowOpenFileOnOpen = False
            End Try
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Open_Button.Click
        ShowOpenFileDialog()
    End Sub

    Private Sub ShowOpenFileDialog()
        If OpenFileDialog1.ShowDialog(Me) = DialogResult.OK Then
            Dim txt As List(Of String) = IO.File.ReadAllText(OpenFileDialog1.FileName).ToLines
            If txt.Any(Function(line) line.StartsWith("SERVER")) Then
                Me.TextBox1.Text = OpenFileDialog1.FileName
            Else
                MessageBox.Show("Incompatible license file: no SERVER line found!")
            End If
        End If
    End Sub

    Property ShowOpenFileOnOpen As Boolean = False

    Public Function IsValid() As Boolean
        Return Not String.IsNullOrWhiteSpace(Me.TextBox1.Text) OrElse IO.Path.GetExtension(Me.TextBox1.Text).ToLower = ".dat"
    End Function

    Public Sub ApplyTextToLicenseFilePath(Optional logger As Logger = Nothing)
        Environment.SetEnvironmentVariable(Manager.EnvironmentVariables.First, Me.TextBox1.Text)
        For Each zVarName As String In Me.Manager.EnvironmentVariables
            Dim value As String = Environment.GetEnvironmentVariable(zVarName, EnvironmentVariableTarget.User)
            If value Is Nothing Then
                Dim value2 As String = Environment.GetEnvironmentVariable(zVarName)
                value = value2
            End If

            If value IsNot Nothing Then
                Environment.SetEnvironmentVariable(zVarName, Me.TextBox1.Text, EnvironmentVariableTarget.User)
                If logger IsNot Nothing Then
                    logger.AddLog($"Set environment variable {zVarName}: {Me.TextBox1.Text}")
                End If
            End If
        Next
    End Sub

    Private Sub Info_EnvVars_Button_Click(sender As Object, e As EventArgs) Handles Info_EnvVars_Button.Click
        Using dlg As New EnvVarsDialog
            dlg.Manager = Me.Manager
            dlg.Populate()
            dlg.ShowDialog(Me)
        End Using
    End Sub

End Class
