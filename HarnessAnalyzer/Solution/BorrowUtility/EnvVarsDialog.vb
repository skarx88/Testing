Imports Microsoft.Win32

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class EnvVarsDialog

    Property Manager As [Shared].LicenseManager

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
        Me.ListView1.Items.Clear()
        Me.ListView1.Groups.Clear()

        For Each grp As IGrouping(Of EnvironmentVariableTarget, EnvVariableInfo) In Variables.GroupBy(Function(v) v.Target)
            Dim group As New ListViewGroup(grp.Key.ToString)
            Me.ListView1.Groups.Add(group)
            For Each info As EnvVariableInfo In grp
                Me.ListView1.Items.Add(New ListViewItem(New String() {info.Name, info.Value})).Group = group
            Next
        Next
        Me.ListView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent)
    End Sub

    ReadOnly Property Variables As New List(Of EnvVariableInfo)

    Public Shared Function GetEnvVariables(manager As [Shared].LicenseManager) As Dictionary(Of String, String)
        Dim dic As New Dictionary(Of String, String)
        For Each zVarName As String In GetEnvironmentVariables(manager)
            Dim value As String = Environment.GetEnvironmentVariable(zVarName, EnvironmentVariableTarget.User)

            If value Is Nothing Then
                Dim value2 As String = Environment.GetEnvironmentVariable(zVarName)
                value = value2
            End If

            If value IsNot Nothing Then
                dic.Add(zVarName, value)
            End If
        Next
        Return dic
    End Function

    Private Shared Function GetEnvironmentVariables(Manager As [Shared].LicenseManager) As String()
        Return Manager.EnvironmentVariables.Concat({"LM_BORROW"}).ToArray
    End Function

    Public Function GetEnvVariablesByTarget() As Dictionary(Of EnvironmentVariableTarget, Dictionary(Of String, String))
        Dim allDic As New Dictionary(Of EnvironmentVariableTarget, Dictionary(Of String, String))
        Dim dic1 As Dictionary(Of String, String) = GetEnvVariablesByTarget(EnvironmentVariableTarget.Process)
        If dic1.Count > 0 Then
            allDic.Add(EnvironmentVariableTarget.Process, dic1)
        End If

        Dim dic2 As Dictionary(Of String, String) = GetEnvVariablesByTarget(EnvironmentVariableTarget.User)
        If dic2.Count > 0 Then
            allDic.Add(EnvironmentVariableTarget.User, dic2)
        End If

        Dim dic3 As Dictionary(Of String, String) = GetEnvVariablesByTarget(EnvironmentVariableTarget.Machine)
        If dic1.Count > 0 Then
            allDic.Add(EnvironmentVariableTarget.Machine, dic3)
        End If

        Return allDic
    End Function

    Private Function GetEnvVariablesByTarget(target As EnvironmentVariableTarget) As Dictionary(Of String, String)
        Dim dic As New Dictionary(Of String, String)
        For Each zVarName As String In GetEnvironmentVariables(Manager)
            Dim value As String = Environment.GetEnvironmentVariable(zVarName, target)
            If value IsNot Nothing Then
                dic.Add(zVarName, If(value, ""))
            End If
        Next
        Return dic
    End Function

    Public Sub Populate()
        Me.Variables.Clear()
        For Each kv As KeyValuePair(Of EnvironmentVariableTarget, Dictionary(Of String, String)) In GetEnvVariablesByTarget()
            For Each grp As KeyValuePair(Of String, String) In kv.Value
                Dim info As New EnvVariableInfo(kv.Key, grp.Key, grp.Value)
                Variables.Add(info)
            Next
        Next
    End Sub

End Class

Public Class EnvVariableInfo
    Public Sub New(target As EnvironmentVariableTarget, name As String, value As String)
        Me.Target = target
        Me.Name = name
        Me.Value = value
    End Sub

    ReadOnly Property Target As EnvironmentVariableTarget

    Property Name As String
    Property Value As String

End Class
