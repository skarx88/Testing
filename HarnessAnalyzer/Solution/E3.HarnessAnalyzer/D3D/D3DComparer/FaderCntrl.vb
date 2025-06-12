Namespace D3D

    <Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
    Public Class FaderCntrl

        Public Event TrackbarValueChanged(sender As Object, e As EventArgs)

        Public Sub New()
            ' Dieser Aufruf ist für den Designer erforderlich.
            InitializeComponent()
            Me.TrackBar1.Value = 1
            Me.lblCompare.Text = Document3DStrings.CompareDoc
            Me.lblReference.Text = Document3DStrings.RefDoc
            Me.BackColor = Color.Transparent
            Me.lblCompare.BackColor = Color.Transparent
            Me.lblReference.BackColor = Color.Transparent
        End Sub

        Private Sub TrackBar1_ValueChanged(sender As Object, e As EventArgs) Handles TrackBar1.ValueChanged
            If TrackBar1.Value <= 0 Then
                Me.lblReference.ForeColor = Color.Black
                Me.lblCompare.ForeColor = Color.Gray
                Me.lblReference.Font = New Font(Me.lblReference.Font, FontStyle.Bold)
                Me.lblCompare.Font = New Font(Me.lblCompare.Font, FontStyle.Regular)
            ElseIf TrackBar1.Value > 0 Then
                Me.lblReference.ForeColor = Color.Gray
                Me.lblCompare.ForeColor = Color.Black
                Me.lblReference.Font = New Font(Me.lblReference.Font, FontStyle.Regular)
                Me.lblCompare.Font = New Font(Me.lblCompare.Font, FontStyle.Bold)
            End If
            RaiseEvent TrackbarValueChanged(sender, e)
        End Sub
    End Class

End Namespace