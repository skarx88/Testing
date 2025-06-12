<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class AvailableFeaturesDlg

    Property Manager As E3.Lib.Licensing.LicenseManagerBase

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub AvailableLicensesDlg_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.AvailableLicensesList.Items.Clear()
        For Each ft As String In EnumEx(Of [Shared].ApplicationFeature).GetNames
            Me.AvailableLicensesList.Items.Add(ft, Manager.AvailableFeatures.Contains(ft))
        Next
    End Sub

    ReadOnly Property CheckedFeatures As String()
        Get
            Return Me.AvailableLicensesList.CheckedItems.Cast(Of String).ToArray
        End Get
    End Property

End Class
