<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ConnectivityView
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ConnectivityView))
        Me.sfdExport = New System.Windows.Forms.SaveFileDialog()
        Me.btnRedraw = New Infragistics.Win.Misc.UltraButton()
        Me.uceConnectors = New Infragistics.Win.UltraWinEditors.UltraComboEditor()
        Me.uckSimpleWireView = New Infragistics.Win.UltraWinEditors.UltraCheckEditor()
        Me.btnPrint = New Infragistics.Win.Misc.UltraButton()
        Me.btnExport = New Infragistics.Win.Misc.UltraButton()
        Me.btnClose = New Infragistics.Win.Misc.UltraButton()
        Me.vDraw = New VectorDraw.Professional.Control.VectorDrawBaseControl()
        Me.uttmConnectivityView = New Infragistics.Win.UltraWinToolTip.UltraToolTipManager(Me.components)
        CType(Me.uceConnectors, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.uckSimpleWireView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnRedraw
        '
        resources.ApplyResources(Me.btnRedraw, "btnRedraw")
        Me.btnRedraw.Name = "btnRedraw"
        '
        'uceConnectors
        '
        resources.ApplyResources(Me.uceConnectors, "uceConnectors")
        Me.uceConnectors.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList
        Me.uceConnectors.Name = "uceConnectors"
        '
        'uckSimpleWireView
        '
        resources.ApplyResources(Me.uckSimpleWireView, "uckSimpleWireView")
        Me.uckSimpleWireView.Name = "uckSimpleWireView"
        '
        'btnPrint
        '
        resources.ApplyResources(Me.btnPrint, "btnPrint")
        Me.btnPrint.Name = "btnPrint"
        '
        'btnExport
        '
        resources.ApplyResources(Me.btnExport, "btnExport")
        Me.btnExport.Name = "btnExport"
        '
        'btnClose
        '
        resources.ApplyResources(Me.btnClose, "btnClose")
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
        'uttmConnectivityView
        '
        Me.uttmConnectivityView.ContainingControl = Me
        '
        'ConnectivityView
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        resources.ApplyResources(Me, "$this")
        Me.Controls.Add(Me.vDraw)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.btnExport)
        Me.Controls.Add(Me.btnPrint)
        Me.Controls.Add(Me.uckSimpleWireView)
        Me.Controls.Add(Me.uceConnectors)
        Me.Controls.Add(Me.btnRedraw)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
        Me.MinimizeBox = False
        Me.Name = "ConnectivityView"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        CType(Me.uceConnectors, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.uckSimpleWireView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents sfdExport As System.Windows.Forms.SaveFileDialog
    Friend WithEvents btnRedraw As Infragistics.Win.Misc.UltraButton
    Friend WithEvents uceConnectors As Infragistics.Win.UltraWinEditors.UltraComboEditor
    Friend WithEvents uckSimpleWireView As Infragistics.Win.UltraWinEditors.UltraCheckEditor
    Friend WithEvents btnPrint As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnExport As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnClose As Infragistics.Win.Misc.UltraButton
    Friend WithEvents vDraw As VectorDraw.Professional.Control.VectorDrawBaseControl
    Friend WithEvents uttmConnectivityView As Infragistics.Win.UltraWinToolTip.UltraToolTipManager

End Class
