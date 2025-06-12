<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class ComparingProgressForm

    Public Event Cancelling As EventHandler

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
        btnCancel.Enabled = False
        RaiseEvent Cancelling(Me, New EventArgs())
    End Sub

End Class