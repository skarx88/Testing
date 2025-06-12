Imports System.Drawing
Imports System.IO
Imports System.Net.NetworkInformation
Imports System.Text
Imports Microsoft.VisualBasic.Devices
Imports Microsoft.Web.WebView2.Core

Friend Class SvgViewerSwappingControl

    'Private WithEvents WebBrowser1 As New OffScreenWebbrowser
    Private _orgText As String
    Private _lock As New System.Threading.SemaphoreSlim(1)
    Private _isCompleted As Boolean
    Private WithEvents _webView As New Microsoft.Web.WebView2.WinForms.WebView2
    Private WithEvents _pictureBox As New ImageBox

    Friend Sub New()
        InitializeComponent()
        _orgText = lblInfoText.Text

        _pictureBox.Dock = DockStyle.Fill
        _pictureBox.SizeMode = ImageBoxSizeMode.Normal
        _pictureBox.AllowUnfocusedMouseWheel = True
        _pictureBox.AllowZoom = True

        _webView.Dock = DockStyle.Fill
    End Sub

    Friend Async Function SetSvgFileAsync(svgFilePath As String, cancel As System.Threading.CancellationToken) As Task
        Await _lock.WaitAsync

        Try
            Me.lblInfoText.Text = _orgText
            Me.lblInfoText.Visible = True
            Me.ProgressBar1.Visible = True
            Me.ProgressBar1.Value = 0
            Me.Controls.Remove(_pictureBox)
            Me.Controls.Remove(_webView)

            If _webView.CoreWebView2 Is Nothing Then
                Dim env As CoreWebView2Environment = Await CoreWebView2Environment.CreateAsync(Nothing, IO.Path.GetDirectoryName(Utilities.GetTempFilePath), Nothing)
                Await _webView.EnsureCoreWebView2Async(env)
            End If
            Me.Controls.Add(_webView)
            _isCompleted = False

            Me.HideProgress()
            _webView.CoreWebView2.Navigate(New Uri(svgFilePath).ToString)
            Await Task.Run(Sub() System.Threading.SpinWait.SpinUntil(Function() _isCompleted OrElse cancel.IsCancellationRequested))
        Catch ex As Exception
#If DEBUG Then
            Throw
#End If
        Finally
            _lock.Release()
        End Try
    End Function

    Friend Sub SetImage(img As System.Drawing.Image)
        Me.lblInfoText.Visible = False
        Me.ProgressBar1.Visible = False
        _pictureBox.Visible = True
        If _pictureBox.Image IsNot Nothing Then
            _pictureBox.Image.Dispose()
        End If

        Me.Controls.Remove(_webView)
        Me.Controls.Add(_pictureBox)
        _pictureBox.Image = img
        _pictureBox.ZoomToFit()
    End Sub

    Friend Sub Reset()
        _webView.Stop()
        _pictureBox.Image = Nothing
        Me.Controls.Remove(_webView)
        Me.Controls.Remove(_pictureBox)
        Me.lblInfoText.Text = _orgText
        Me.lblMessage.Visible = False
        Me.lblInfoText.Visible = True
        Me.ProgressBar1.Visible = True
        Me.ProgressBar1.Style = ProgressBarStyle.Marquee
        Me.Refresh()
    End Sub

    Friend Sub SetErrorText(txt As String)
        Me.Reset()
        Me.HideProgress()
        Me.lblMessage.Visible = True
        Me.lblMessage.Text = txt
    End Sub

    Private Sub _webView_NavigationCompleted(sender As Object, e As CoreWebView2NavigationCompletedEventArgs) Handles _webView.NavigationCompleted
        HideProgress()
        _isCompleted = True
    End Sub

    Private Sub HideProgress()
        Me.ProgressBar1.Visible = False
        Me.lblInfoText.Visible = False
        Me.lblMessage.Visible = False
    End Sub

    Private Async Sub _webView_NavigationStarting(sender As Object, e As CoreWebView2NavigationStartingEventArgs) Handles _webView.NavigationStarting
        Dim script As String =
        $"const svgs = document.getElementsByTagName(""svg"")
        svgs[0].style = ""height:100vh;width:100vw; position:fixed; vertical-align: middle"""
        Dim res = Await _webView.CoreWebView2.ExecuteScriptWithResultAsync(script)
    End Sub

End Class
