Imports System.Drawing
Imports System.IO

Friend Class PreviewControl

    Private _xhcvControl As New XhcvContentViewControl
    Private _swapViewer As New SvgViewerSwappingControl
    Private _lock As New System.Threading.SemaphoreSlim(1)

    Friend Sub New()
        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        _xhcvControl.Dock = DockStyle.Fill
        _swapViewer.Dock = DockStyle.Fill
    End Sub

    Friend Sub SetImage(img As System.Drawing.Image, Optional showDetails As Boolean = True)
        Me.PanelTop.Controls.Clear()
        Me.PanelTop.Controls.Add(_swapViewer)

        Me.GroupBox1.Visible = showDetails
        _swapViewer.SetImage(img)
    End Sub

    Friend Async Function SetSvgAsync(svgFilePath As String, cancel As System.Threading.CancellationToken) As Task
        Me.PanelTop.Controls.Clear()
        Me.PanelTop.Controls.Add(_swapViewer)

        Await _lock.WaitAsync
        Try
            If Not cancel.IsCancellationRequested Then
                Await _swapViewer.SetSvgFileAsync(svgFilePath, cancel)
                Me.GroupBox1.Visible = True
            Else
                _swapViewer.Reset()
            End If
        Finally
            _lock.Release()
        End Try
    End Function

    Friend Sub SetxHcv(stream As System.IO.Stream, name As String)
        Me.PanelTop.Controls.Clear()
        Me.PanelTop.Controls.Add(_xhcvControl)
        Me.GroupBox1.Visible = False
        _xhcvControl.LoadxHcvContent(stream, name)
    End Sub

    Friend Sub Reset(style As ResetStyle)
        PanelTop.Controls.Clear()
        Select Case style
            Case ResetStyle.ResetToSwapViewer
                PanelTop.Controls.Add(_swapViewer)
            Case ResetStyle.ResetToxhcvTree
                PanelTop.Controls.Add(_xhcvControl)
        End Select

        chkJT.Checked = False
        chkTopo2D.Checked = False
        chkTopo3D.Checked = False

        _swapViewer.Reset()
        _xhcvControl.Reset()
    End Sub

    Friend Enum ResetStyle
        ResetToSwapViewer
        ResetToxhcvTree
    End Enum

    Friend Sub SetErrorText(txt As String)
        Me.PanelTop.Controls.Clear()
        Me.PanelTop.Controls.Add(_swapViewer)

        _swapViewer.SetErrorText(txt)
    End Sub

End Class
