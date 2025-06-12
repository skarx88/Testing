<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class EntireRoutingPathForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(EntireRoutingPathForm))
        Dim Appearance1 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim UltraToolTipInfo1 As Infragistics.Win.UltraWinToolTip.UltraToolTipInfo = New Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Select all cables/wires of this routing path (click on the cable/wire with the le" &
        "ft mouse to single select)", Infragistics.Win.ToolTipImage.[Default], "", Infragistics.Win.DefaultableBoolean.[Default])
        Me.btnRedraw = New Infragistics.Win.Misc.UltraButton()
        Me.btnExport = New Infragistics.Win.Misc.UltraButton()
        Me.btnPrint = New Infragistics.Win.Misc.UltraButton()
        Me.btnClose = New Infragistics.Win.Misc.UltraButton()
        Me.vDraw = New VectorDraw.Professional.Control.VectorDrawBaseControl()
        Me.lblInfo = New Infragistics.Win.Misc.UltraLabel()
        Me.btnSelectAll = New Infragistics.Win.Misc.UltraButton()
        Me.uttmEntireRoutingPath = New Infragistics.Win.UltraWinToolTip.UltraToolTipManager(Me.components)
        Me.SuspendLayout()
        '
        'btnRedraw
        '
        resources.ApplyResources(Me.btnRedraw, "btnRedraw")
        Me.btnRedraw.Name = "btnRedraw"
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
        'btnClose
        '
        resources.ApplyResources(Me.btnClose, "btnClose")
        Me.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnClose.Name = "btnClose"
        '
        'vDraw
        '
        Me.vDraw.AccessibleRole = System.Windows.Forms.AccessibleRole.Window
        Me.vDraw.AllowDrop = True
        resources.ApplyResources(Me.vDraw, "vDraw")
        Me.vDraw.Cursor = System.Windows.Forms.Cursors.Default
        Me.vDraw.DisableVdrawDxf = False
        Me.vDraw.EnableAutoGripOn = True
        Me.vDraw.Name = "vDraw"
        '
        'lblInfo
        '
        resources.ApplyResources(Me.lblInfo, "lblInfo")
        resources.ApplyResources(Appearance1, "Appearance1")
        Me.lblInfo.Appearance = Appearance1
        Me.lblInfo.Name = "lblInfo"
        '
        'btnSelectAll
        '
        resources.ApplyResources(Me.btnSelectAll, "btnSelectAll")
        Me.btnSelectAll.Name = "btnSelectAll"
        resources.ApplyResources(UltraToolTipInfo1, "UltraToolTipInfo1")
        Me.uttmEntireRoutingPath.SetUltraToolTip(Me.btnSelectAll, UltraToolTipInfo1)
        '
        'uttmEntireRoutingPath
        '
        Me.uttmEntireRoutingPath.ContainingControl = Me
        Me.uttmEntireRoutingPath.DisplayStyle = Infragistics.Win.ToolTipDisplayStyle.Standard
        '
        'EntireRoutingPathForm
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.CancelButton = Me.btnClose
        resources.ApplyResources(Me, "$this")
        Me.Controls.Add(Me.btnSelectAll)
        Me.Controls.Add(Me.lblInfo)
        Me.Controls.Add(Me.btnRedraw)
        Me.Controls.Add(Me.btnExport)
        Me.Controls.Add(Me.btnPrint)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.vDraw)
        Me.MinimizeBox = False
        Me.Name = "EntireRoutingPathForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.TopMost = False
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnRedraw As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnExport As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnPrint As Infragistics.Win.Misc.UltraButton
    Private WithEvents btnClose As Infragistics.Win.Misc.UltraButton
    Friend WithEvents vDraw As VectorDraw.Professional.Control.VectorDrawBaseControl
    Friend WithEvents lblInfo As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents btnSelectAll As Infragistics.Win.Misc.UltraButton
    Friend WithEvents uttmEntireRoutingPath As Infragistics.Win.UltraWinToolTip.UltraToolTipManager
End Class
