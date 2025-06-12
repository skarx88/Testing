<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class StartEndConnectorsForm
    Inherits System.Windows.Forms.Form

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(StartEndConnectorsForm))
        Me.vDraw = New VectorDraw.Professional.Control.VectorDrawBaseControl()
        Me.btnClose = New Infragistics.Win.Misc.UltraButton()
        Me.btnExport = New Infragistics.Win.Misc.UltraButton()
        Me.btnPrint = New Infragistics.Win.Misc.UltraButton()
        Me.btnRedraw = New Infragistics.Win.Misc.UltraButton()
        Me.SuspendLayout()
        '
        'vDraw
        '
        resources.ApplyResources(Me.vDraw, "vDraw")
        Me.vDraw.AccessibleRole = System.Windows.Forms.AccessibleRole.Window
        Me.vDraw.AllowDrop = True
        Me.vDraw.Cursor = System.Windows.Forms.Cursors.Default
        Me.vDraw.DisableVdrawDxf = False
        Me.vDraw.EnableAutoGripOn = True
        Me.vDraw.Name = "vDraw"
        '
        'btnClose
        '
        resources.ApplyResources(Me.btnClose, "btnClose")
        Me.btnClose.Name = "btnClose"
        '
        'btnExport
        '
        resources.ApplyResources(Me.btnExport, "btnExport")
        Me.btnExport.Name = "btnExport"
        '
        'btnPrint
        '
        resources.ApplyResources(Me.btnPrint, "btnPrint")
        Me.btnPrint.Name = "btnPrint"
        '
        'btnRedraw
        '
        resources.ApplyResources(Me.btnRedraw, "btnRedraw")
        Me.btnRedraw.Name = "btnRedraw"
        '
        'StartEndConnectorsForm
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.Controls.Add(Me.btnRedraw)
        Me.Controls.Add(Me.btnExport)
        Me.Controls.Add(Me.btnPrint)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.vDraw)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
        Me.MinimizeBox = False
        Me.Name = "StartEndConnectorsForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents vDraw As VectorDraw.Professional.Control.VectorDrawBaseControl
    Private WithEvents btnClose As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnExport As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnPrint As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnRedraw As Infragistics.Win.Misc.UltraButton

End Class
