Imports System.ComponentModel
Imports Zuken.E3.Lib.Licensing

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class LogViewFrm

    Private WithEvents _logger As Logger

    Public Sub New()
        InitializeComponent()
    End Sub

    Property Logger As Logger
        Get
            Return _logger
        End Get
        Set
            _logger = Value
            If _logger IsNot Nothing Then
                Me.TextBox1.Text = _logger.ToString
            Else
                Me.TextBox1.Text = String.Empty
            End If
        End Set
    End Property

    Private Sub _logger_LogChanged(sender As Object, e As LogChangedEventArgs) Handles _logger.LogChanged
        If Me.TextBox1.InvokeRequired Then
            Me.TextBox1.Invoke(Sub() DoLogChanged(e))
        Else
            DoLogChanged(e)
        End If
    End Sub

    Private Sub DoLogChanged(e As LogChangedEventArgs)
        Select Case e.ChangeType
            Case LogChangeType.Added
                Me.TextBox1.AppendText(e.NewEntry + vbCrLf)
            Case LogChangeType.Reset
                Me.TextBox1.Text = String.Empty
        End Select
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If _logger IsNot Nothing Then
            _logger.Clear()
        Else
            Me.TextBox1.Text = String.Empty
        End If
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)
        MyBase.OnClosing(e)
        If e.CloseReason = CloseReason.UserClosing Then
            e.Cancel = True
            Me.Hide()
        End If
    End Sub

End Class