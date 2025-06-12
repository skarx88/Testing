Public Class WindowStateChangedDetector

    Private WithEvents _frm As Form
    Private _previousState As Nullable(Of FormWindowState) = Nothing

    Public Event WindowStateChanged(sender As Object, e As EventArgs)

    Public Sub New()
    End Sub

    Public Sub New(frm As Form)
        Me.Form = frm
    End Sub

    Property Form As System.Windows.Forms.Form
        Get
            Return _frm
        End Get
        Set(value As System.Windows.Forms.Form)
            _frm = value
        End Set
    End Property

    Private Sub _frm_Shown(sender As Object, e As EventArgs) Handles _frm.Shown
        _previousState = Me._frm.WindowState
    End Sub

    Private Sub _frm_Resize(sender As Object, e As EventArgs) Handles _frm.Resize
        If _previousState IsNot Nothing AndAlso _previousState.Value <> _frm.WindowState Then
            _previousState = Me._frm.WindowState
            RaiseEvent WindowStateChanged(_frm, New EventArgs)
        End If
    End Sub
End Class
