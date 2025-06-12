<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class WireResistanceCalculatorForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(WireResistanceCalculatorForm))
        Dim UltraToolTipInfo1 As Infragistics.Win.UltraWinToolTip.UltraToolTipInfo = New Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Length of all selected wires is zero (maybe check selected length-type in setting" &
        "s)", Infragistics.Win.ToolTipImage.Warning, "Warning", Infragistics.Win.DefaultableBoolean.[Default])
        Dim Appearance1 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance2 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Me.btnClose = New Infragistics.Win.Misc.UltraButton()
        Me.lblCaption = New Infragistics.Win.Misc.UltraLabel()
        Me.ugbConductor = New Infragistics.Win.Misc.UltraGroupBox()
        Me.uneTemperature = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
        Me.lblTemperature = New Infragistics.Win.Misc.UltraLabel()
        Me.ufllResistivityUnit = New Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel()
        Me.uneResistivity = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
        Me.lblResistivity = New Infragistics.Win.Misc.UltraLabel()
        Me.uceConductorMaterial = New Infragistics.Win.UltraWinEditors.UltraComboEditor()
        Me.lblConductorMaterial = New Infragistics.Win.Misc.UltraLabel()
        Me.lblWireLength = New Infragistics.Win.Misc.UltraLabel()
        Me.uneWireLength = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
        Me.lblWireCSA = New Infragistics.Win.Misc.UltraLabel()
        Me.uneWireCSA = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
        Me.lblWireResistance = New Infragistics.Win.Misc.UltraLabel()
        Me.uneWireResistance = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
        Me.txtMultiple = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.txtResistanceMultiplier = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
        Me.lblAddMultiply = New Infragistics.Win.Misc.UltraLabel()
        Me.lblAdditionalResistance = New Infragistics.Win.Misc.UltraLabel()
        Me.UltraToolTipManager1 = New Infragistics.Win.UltraWinToolTip.UltraToolTipManager(Me.components)
        CType(Me.ugbConductor, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ugbConductor.SuspendLayout()
        CType(Me.uneTemperature, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.uneResistivity, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.uceConductorMaterial, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.uneWireLength, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.uneWireCSA, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.uneWireResistance, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtMultiple, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.Panel1.SuspendLayout()
        CType(Me.txtResistanceMultiplier, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnClose
        '
        resources.ApplyResources(Me.btnClose, "btnClose")
        Me.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnClose.Name = "btnClose"
        '
        'lblCaption
        '
        resources.ApplyResources(Me.lblCaption, "lblCaption")
        Me.lblCaption.Name = "lblCaption"
        '
        'ugbConductor
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.ugbConductor, 3)
        Me.ugbConductor.Controls.Add(Me.uneTemperature)
        Me.ugbConductor.Controls.Add(Me.lblTemperature)
        Me.ugbConductor.Controls.Add(Me.ufllResistivityUnit)
        Me.ugbConductor.Controls.Add(Me.uneResistivity)
        Me.ugbConductor.Controls.Add(Me.lblResistivity)
        Me.ugbConductor.Controls.Add(Me.uceConductorMaterial)
        Me.ugbConductor.Controls.Add(Me.lblConductorMaterial)
        resources.ApplyResources(Me.ugbConductor, "ugbConductor")
        Me.ugbConductor.Name = "ugbConductor"
        '
        'uneTemperature
        '
        resources.ApplyResources(Me.uneTemperature, "uneTemperature")
        Me.uneTemperature.FormatString = "##0 °C"
        Me.uneTemperature.MaxValue = 140
        Me.uneTemperature.MinValue = -40
        Me.uneTemperature.Name = "uneTemperature"
        Me.uneTemperature.PromptChar = Global.Microsoft.VisualBasic.ChrW(32)
        '
        'lblTemperature
        '
        resources.ApplyResources(Me.lblTemperature, "lblTemperature")
        Me.lblTemperature.Name = "lblTemperature"
        '
        'ufllResistivityUnit
        '
        resources.ApplyResources(Me.ufllResistivityUnit, "ufllResistivityUnit")
        Me.ufllResistivityUnit.Name = "ufllResistivityUnit"
        Me.ufllResistivityUnit.TabStop = True
        '
        'uneResistivity
        '
        resources.ApplyResources(Me.uneResistivity, "uneResistivity")
        Me.uneResistivity.FormatString = "##0.0###"
        Me.uneResistivity.MaskInput = "{double:3.4}"
        Me.uneResistivity.MaxValue = 999.9999R
        Me.uneResistivity.MinValue = 0
        Me.uneResistivity.Name = "uneResistivity"
        Me.uneResistivity.NumericType = Infragistics.Win.UltraWinEditors.NumericType.[Double]
        Me.uneResistivity.PromptChar = Global.Microsoft.VisualBasic.ChrW(32)
        Me.uneResistivity.ReadOnly = True
        '
        'lblResistivity
        '
        resources.ApplyResources(Me.lblResistivity, "lblResistivity")
        Me.lblResistivity.Name = "lblResistivity"
        '
        'uceConductorMaterial
        '
        resources.ApplyResources(Me.uceConductorMaterial, "uceConductorMaterial")
        Me.uceConductorMaterial.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList
        Me.uceConductorMaterial.Name = "uceConductorMaterial"
        '
        'lblConductorMaterial
        '
        resources.ApplyResources(Me.lblConductorMaterial, "lblConductorMaterial")
        Me.lblConductorMaterial.Name = "lblConductorMaterial"
        '
        'lblWireLength
        '
        resources.ApplyResources(Me.lblWireLength, "lblWireLength")
        Me.lblWireLength.Name = "lblWireLength"
        '
        'uneWireLength
        '
        resources.ApplyResources(Me.uneWireLength, "uneWireLength")
        Me.uneWireLength.FormatString = "#####0.0"
        Me.uneWireLength.MaskInput = "{double:6.1}"
        Me.uneWireLength.MaxValue = 999999.9R
        Me.uneWireLength.MinValue = 0
        Me.uneWireLength.Name = "uneWireLength"
        Me.uneWireLength.Nullable = True
        Me.uneWireLength.NumericType = Infragistics.Win.UltraWinEditors.NumericType.[Double]
        Me.uneWireLength.PromptChar = Global.Microsoft.VisualBasic.ChrW(32)
        UltraToolTipInfo1.ToolTipImage = Infragistics.Win.ToolTipImage.Warning
        resources.ApplyResources(UltraToolTipInfo1, "UltraToolTipInfo1")
        Me.UltraToolTipManager1.SetUltraToolTip(Me.uneWireLength, UltraToolTipInfo1)
        '
        'lblWireCSA
        '
        resources.ApplyResources(Me.lblWireCSA, "lblWireCSA")
        Me.lblWireCSA.Name = "lblWireCSA"
        '
        'uneWireCSA
        '
        resources.ApplyResources(Me.uneWireCSA, "uneWireCSA")
        Me.uneWireCSA.FormatString = "#####0.0#"
        Me.uneWireCSA.MaskInput = "{double:6.2}"
        Me.uneWireCSA.MaxValue = 999999.99R
        Me.uneWireCSA.MinValue = 0
        Me.uneWireCSA.Name = "uneWireCSA"
        Me.uneWireCSA.Nullable = True
        Me.uneWireCSA.NumericType = Infragistics.Win.UltraWinEditors.NumericType.[Double]
        Me.uneWireCSA.PromptChar = Global.Microsoft.VisualBasic.ChrW(32)
        '
        'lblWireResistance
        '
        resources.ApplyResources(Me.lblWireResistance, "lblWireResistance")
        Me.lblWireResistance.Name = "lblWireResistance"
        '
        'uneWireResistance
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.uneWireResistance, 2)
        resources.ApplyResources(Me.uneWireResistance, "uneWireResistance")
        Me.uneWireResistance.FormatString = "#####0.0###"
        Me.uneWireResistance.MaskInput = "{double:3.4}"
        Me.uneWireResistance.MaxValue = 999.9999R
        Me.uneWireResistance.MinValue = 0
        Me.uneWireResistance.Name = "uneWireResistance"
        Me.uneWireResistance.Nullable = True
        Me.uneWireResistance.NullText = "NaN"
        Me.uneWireResistance.NumericType = Infragistics.Win.UltraWinEditors.NumericType.[Double]
        Me.uneWireResistance.PromptChar = Global.Microsoft.VisualBasic.ChrW(32)
        Me.uneWireResistance.ReadOnly = True
        '
        'txtMultiple
        '
        Appearance1.ForeColor = System.Drawing.Color.Red
        resources.ApplyResources(Appearance1, "Appearance1")
        Me.txtMultiple.Appearance = Appearance1
        resources.ApplyResources(Me.txtMultiple, "txtMultiple")
        Me.txtMultiple.Name = "txtMultiple"
        Me.txtMultiple.ReadOnly = True
        '
        'TableLayoutPanel1
        '
        resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
        Me.TableLayoutPanel1.Controls.Add(Me.ugbConductor, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.lblWireLength, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.lblWireCSA, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.Panel1, 1, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.lblWireResistance, 0, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.uneWireResistance, 1, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.uneWireLength, 1, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.txtResistanceMultiplier, 1, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.lblAddMultiply, 2, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.lblAdditionalResistance, 0, 3)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.uneWireCSA)
        Me.Panel1.Controls.Add(Me.txtMultiple)
        resources.ApplyResources(Me.Panel1, "Panel1")
        Me.Panel1.Name = "Panel1"
        '
        'txtResistanceMultiplier
        '
        resources.ApplyResources(Me.txtResistanceMultiplier, "txtResistanceMultiplier")
        Me.txtResistanceMultiplier.FormatString = "#####0.0###"
        Me.txtResistanceMultiplier.MaskInput = "{double:3.4}"
        Me.txtResistanceMultiplier.MaxValue = 999.9999R
        Me.txtResistanceMultiplier.MinValue = 0R
        Me.txtResistanceMultiplier.Name = "txtResistanceMultiplier"
        Me.txtResistanceMultiplier.Nullable = True
        Me.txtResistanceMultiplier.NullText = "0"
        Me.txtResistanceMultiplier.NumericType = Infragistics.Win.UltraWinEditors.NumericType.[Double]
        Me.txtResistanceMultiplier.PromptChar = Global.Microsoft.VisualBasic.ChrW(32)
        '
        'lblAddMultiply
        '
        resources.ApplyResources(Appearance2, "Appearance2")
        Me.lblAddMultiply.Appearance = Appearance2
        resources.ApplyResources(Me.lblAddMultiply, "lblAddMultiply")
        Me.lblAddMultiply.Name = "lblAddMultiply"
        '
        'lblAdditionalResistance
        '
        resources.ApplyResources(Me.lblAdditionalResistance, "lblAdditionalResistance")
        Me.lblAdditionalResistance.Name = "lblAdditionalResistance"
        '
        'UltraToolTipManager1
        '
        Me.UltraToolTipManager1.ContainingControl = Me
        '
        'WireResistanceCalculatorForm
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.CancelButton = Me.btnClose
        resources.ApplyResources(Me, "$this")
        Me.Controls.Add(Me.lblCaption)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Controls.Add(Me.btnClose)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "WireResistanceCalculatorForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        CType(Me.ugbConductor, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ugbConductor.ResumeLayout(False)
        Me.ugbConductor.PerformLayout()
        CType(Me.uneTemperature, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.uneResistivity, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.uceConductorMaterial, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.uneWireLength, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.uneWireCSA, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.uneWireResistance, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtMultiple, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        CType(Me.txtResistanceMultiplier, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnClose As Infragistics.Win.Misc.UltraButton
    Friend WithEvents lblCaption As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents ugbConductor As Infragistics.Win.Misc.UltraGroupBox
    Friend WithEvents uneResistivity As Infragistics.Win.UltraWinEditors.UltraNumericEditor
    Friend WithEvents lblResistivity As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents uceConductorMaterial As Infragistics.Win.UltraWinEditors.UltraComboEditor
    Friend WithEvents lblConductorMaterial As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents lblWireLength As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents uneWireLength As Infragistics.Win.UltraWinEditors.UltraNumericEditor
    Friend WithEvents lblWireCSA As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents uneWireCSA As Infragistics.Win.UltraWinEditors.UltraNumericEditor
    Friend WithEvents lblWireResistance As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents uneWireResistance As Infragistics.Win.UltraWinEditors.UltraNumericEditor
    Friend WithEvents ufllResistivityUnit As Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel
    Friend WithEvents uneTemperature As Infragistics.Win.UltraWinEditors.UltraNumericEditor
    Friend WithEvents lblTemperature As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents txtMultiple As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents Panel1 As Panel
    Friend WithEvents txtResistanceMultiplier As Infragistics.Win.UltraWinEditors.UltraNumericEditor
    Friend WithEvents lblAddMultiply As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents lblAdditionalResistance As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents UltraToolTipManager1 As Infragistics.Win.UltraWinToolTip.UltraToolTipManager
End Class
