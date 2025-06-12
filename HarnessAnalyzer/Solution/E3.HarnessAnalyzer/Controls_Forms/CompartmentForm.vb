<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class CompartmentForm

    Private _harnessPartNumbers As List(Of String)
    Private _initialHarnessPartNumber As String

    Public Sub New(harnessPartNumbers As List(Of String), harnessPartNumber As String, harnessDescription As String)
        InitializeComponent()

        _initialHarnessPartNumber = harnessPartNumber
        _harnessPartNumbers = harnessPartNumbers

        If (harnessPartNumber = String.Empty) Then
            Me.txtHarnessPartNumber.Text = CompartmentFormStrings.HarnessPartNumber_Text
        Else
            Me.txtHarnessPartNumber.Text = harnessPartNumber
        End If

        Me.txtHarnessDescription.Text = harnessDescription
    End Sub

    Private Sub CompartmentForm_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        Me.txtHarnessPartNumber.SelectAll()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        If (Me.txtHarnessPartNumber.Text = CompartmentFormStrings.HarnessPartNumber_Text) OrElse (Me.txtHarnessPartNumber.Text = String.Empty) Then
            MessageBox.Show(CompartmentFormStrings.EnterValidPartNumber_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        ElseIf (_initialHarnessPartNumber <> Me.txtHarnessPartNumber.Text) AndAlso (_harnessPartNumbers.Contains(Me.txtHarnessPartNumber.Text)) Then
            MessageBox.Show(CompartmentFormStrings.PartNumberExists_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Else
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
        End If
    End Sub

End Class