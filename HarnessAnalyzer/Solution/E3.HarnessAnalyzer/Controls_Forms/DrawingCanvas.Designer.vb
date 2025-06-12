Imports System.Windows.Controls
Imports VectorDraw.Professional.vdPrimaries

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class DrawingCanvas
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing Then
                If _vdBaseControl?.ActiveDocument?.Model?.Entities IsNot Nothing Then
                    For Each entity As vdFigure In _vdBaseControl.ActiveDocument.Model.Entities
                        entity.Dispose()
                    Next
                End If

                _vdBaseControl?.ActiveDocument?.Model?.Entities?.Dispose()
                _vdBaseControl?.ActiveDocument?.ClearAll()
                _highlightRects?.Clear()
                _activeAnalysisObjects?.Clear()
                _issueMapper?.Clear()
                _redliningMapper?.Clear()
                _stampMapper?.Clear()
                _groupMapper?.Clear()
                _svgConverter?.Dispose()
                _tooltipForm?.Dispose()
                _vdBaseControl?.Dispose()
                _lazyDrawingEntitiesProgressTimer?.Dispose()
                _factory?.Dispose()
                components?.Dispose()
            End If

            NavigatorImage = Nothing
            _factory = Nothing
            _initialSvgBoundingBox = Nothing
            _lazyDrawingEntitiesProgressTimer = Nothing
            _activeDocument = Nothing
            _vdBaseControl = Nothing
            _tooltipForm = Nothing
            _svgConverter = Nothing
            _draftConverter = Nothing
            _documentForm = Nothing
            _logEventArgs = Nothing
            _groupMapper = Nothing
            _workState = Nothing
            _selection = Nothing
            _redliningInformation = Nothing
            _qmStamps = Nothing
            _messageEventArgs = Nothing
            _issueMapper = Nothing
            _hoverEntity = Nothing
            _hitPoint = Nothing
            _generalSettings = Nothing
            _activeAnalysisObjects = Nothing
            _gripEntity = Nothing
            _hitEntity = Nothing
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'HINT Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DrawingCanvas))
        tooltipTimer = New Timer(components)
        vdCanvas = New vdScrollableControl.vdScrollableControl()
        uttmCanvas = New Infragistics.Win.UltraWinToolTip.UltraToolTipManager(components)
        SuspendLayout()
        ' 
        ' tooltipTimer
        ' 
        tooltipTimer.Interval = 1000
        ' 
        ' vdCanvas
        ' 
        vdCanvas.BackColor = Color.White
        resources.ApplyResources(vdCanvas, "vdCanvas")
        vdCanvas.Name = "vdCanvas"
        vdCanvas.ShowLayoutPopupMenu = True
        ' 
        ' uttmCanvas
        ' 
        uttmCanvas.ContainingControl = Me
        ' 
        ' DrawingCanvas
        ' 
        resources.ApplyResources(Me, "$this")
        AutoScaleMode = AutoScaleMode.Font
        Controls.Add(vdCanvas)
        DoubleBuffered = True
        Name = "DrawingCanvas"
        ResumeLayout(False)

    End Sub

    Friend WithEvents tooltipTimer As System.Windows.Forms.Timer
    Friend WithEvents vdCanvas As vdScrollableControl.vdScrollableControl
    Friend WithEvents uttmCanvas As Infragistics.Win.UltraWinToolTip.UltraToolTipManager

End Class
