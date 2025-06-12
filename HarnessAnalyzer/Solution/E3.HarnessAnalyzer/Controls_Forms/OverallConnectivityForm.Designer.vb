<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class OverallConnectivityForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(OverallConnectivityForm))
        Dim UltraToolTipInfo2 As Infragistics.Win.UltraWinToolTip.UltraToolTipInfo = New Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Select all wires/cores of this connectivity (click on a wire/core with the  left " &
        "mouse for single selection)", Infragistics.Win.ToolTipImage.[Default], "", Infragistics.Win.DefaultableBoolean.[Default])
        Dim UltraToolTipInfo1 As Infragistics.Win.UltraWinToolTip.UltraToolTipInfo = New Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Select all wires/cores of this connectivity (click on a wire/core with the  left " &
        "mouse for single selection)", Infragistics.Win.ToolTipImage.[Default], "", Infragistics.Win.DefaultableBoolean.[Default])
        Me.btnClose = New Infragistics.Win.Misc.UltraButton()
        Me.vDraw = New VectorDraw.Professional.Control.VectorDrawBaseControl()
        Me.btnRedraw = New Infragistics.Win.Misc.UltraButton()
        Me.btnExport = New Infragistics.Win.Misc.UltraButton()
        Me.btnPrint = New Infragistics.Win.Misc.UltraButton()
        Me.uttmOverallConnectivity = New Infragistics.Win.UltraWinToolTip.UltraToolTipManager(Me.components)
        Me.btnSelectAll = New Infragistics.Win.Misc.UltraButton()
        Me.btnCalcWireResistance = New Infragistics.Win.Misc.UltraButton()
        Me.btnCalcVoltageDrop = New Infragistics.Win.Misc.UltraButton()
        Me.SuspendLayout()
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
        Me.vDraw.Name = "vDraw"
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
        'uttmOverallConnectivity
        '
        Me.uttmOverallConnectivity.ContainingControl = Me
        Me.uttmOverallConnectivity.DisplayStyle = Infragistics.Win.ToolTipDisplayStyle.Standard
        '
        'btnSelectAll
        '
        resources.ApplyResources(Me.btnSelectAll, "btnSelectAll")
        Me.btnSelectAll.Name = "btnSelectAll"
        resources.ApplyResources(UltraToolTipInfo2, "UltraToolTipInfo2")
        Me.uttmOverallConnectivity.SetUltraToolTip(Me.btnSelectAll, UltraToolTipInfo2)
        '
        'btnCalcWireResistance
        '
        resources.ApplyResources(Me.btnCalcWireResistance, "btnCalcWireResistance")
        Me.btnCalcWireResistance.Name = "btnCalcWireResistance"
        resources.ApplyResources(UltraToolTipInfo1, "UltraToolTipInfo1")
        Me.uttmOverallConnectivity.SetUltraToolTip(Me.btnCalcWireResistance, UltraToolTipInfo1)
        '
        'btnCalcVoltageDrop
        '
        resources.ApplyResources(Me.btnCalcVoltageDrop, "btnCalcVoltageDrop")
        Me.btnCalcVoltageDrop.Name = "btnCalcVoltageDrop"
        resources.ApplyResources(UltraToolTipInfo1, "UltraToolTipInfo1")
        Me.uttmOverallConnectivity.SetUltraToolTip(Me.btnCalcVoltageDrop, UltraToolTipInfo1)
        '
        'OverallConnectivityForm
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.CancelButton = Me.btnClose
        resources.ApplyResources(Me, "$this")
        Me.Controls.Add(Me.btnCalcVoltageDrop)
        Me.Controls.Add(Me.btnCalcWireResistance)
        Me.Controls.Add(Me.btnSelectAll)
        Me.Controls.Add(Me.btnRedraw)
        Me.Controls.Add(Me.btnExport)
        Me.Controls.Add(Me.btnPrint)
        Me.Controls.Add(Me.vDraw)
        Me.Controls.Add(Me.btnClose)
        Me.MinimizeBox = False
        Me.Name = "OverallConnectivityForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnClose As Infragistics.Win.Misc.UltraButton
    Friend WithEvents vDraw As VectorDraw.Professional.Control.VectorDrawBaseControl
    Friend WithEvents btnRedraw As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnExport As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnPrint As Infragistics.Win.Misc.UltraButton
    Friend WithEvents uttmOverallConnectivity As Infragistics.Win.UltraWinToolTip.UltraToolTipManager
    Friend WithEvents btnSelectAll As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnCalcWireResistance As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnCalcVoltageDrop As Infragistics.Win.Misc.UltraButton
End Class
