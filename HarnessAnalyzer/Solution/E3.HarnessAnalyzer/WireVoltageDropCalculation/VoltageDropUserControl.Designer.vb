<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class VoltageDropUserControl
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.StartResitanceUNE = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
        Me.WireOrCoreNumUTE = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.FirstContactPointUTE = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.LastContactPointUTE = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.TypeOfWireUCE = New Infragistics.Win.UltraWinEditors.UltraComboEditor()
        Me.ConductorMaterialUCE = New Infragistics.Win.UltraWinEditors.UltraComboEditor()
        Me.MaterialDensityUNE = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
        Me.ResistivityCustomUNE = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
        Me.CsaUNE = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
        Me.TemperatureUNE = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
        Me.LengthUNE = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
        Me.ConductorWeightUNE = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
        Me.TotalWeightUNE = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
        Me.EndResistanceUNE = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
        Me.ContentTableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
        CType(Me.StartResitanceUNE, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WireOrCoreNumUTE, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.FirstContactPointUTE, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TypeOfWireUCE, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ConductorMaterialUCE, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.MaterialDensityUNE, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ResistivityCustomUNE, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CsaUNE, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TemperatureUNE, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LengthUNE, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ConductorWeightUNE, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TotalWeightUNE, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LastContactPointUTE, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.EndResistanceUNE, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ContentTableLayoutPanel.SuspendLayout()
        Me.SuspendLayout()
        '
        'ResitivityUNE
        '
        Me.StartResitanceUNE.Dock = System.Windows.Forms.DockStyle.Fill
        Me.StartResitanceUNE.FormatString = "###0.0# Ω"
        Me.StartResitanceUNE.MaskInput = "{double:3.2}"
        Me.StartResitanceUNE.MaxValue = 999.9999R
        Me.StartResitanceUNE.MinValue = 0
        Me.StartResitanceUNE.Name = "ResitivityUNE"
        Me.StartResitanceUNE.Nullable = True
        Me.StartResitanceUNE.NullText = "0.0"
        Me.StartResitanceUNE.NumericType = Infragistics.Win.UltraWinEditors.NumericType.[Double]
        Me.StartResitanceUNE.PromptChar = Global.Microsoft.VisualBasic.ChrW(32)
        Me.StartResitanceUNE.TabIndex = 1
        Me.StartResitanceUNE.Value = 0.1R
        '
        'WireOrCoreNumUTE
        '
        Me.WireOrCoreNumUTE.Dock = System.Windows.Forms.DockStyle.Fill
        Me.WireOrCoreNumUTE.Name = "WireOrCoreNumUTE"
        Me.WireOrCoreNumUTE.TabIndex = 2

        '
        'FirstContactPointUTE
        '
        Me.FirstContactPointUTE.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FirstContactPointUTE.Name = "FirstContactPointUTE"
        Me.FirstContactPointUTE.ReadOnly = True
        Me.FirstContactPointUTE.TabIndex = 2
        '
        'LastContactPointUTE
        '
        Me.LastContactPointUTE.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LastContactPointUTE.Name = "LastContactPointUTE"
        Me.LastContactPointUTE.ReadOnly = True
        Me.LastContactPointUTE.TabIndex = 2
        '
        'TypeOfWireUCE
        '
        Me.TypeOfWireUCE.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TypeOfWireUCE.Name = "TypeOfWireUCE"
        Me.TypeOfWireUCE.TabIndex = 3
        '
        'ConductorMaterialUCE
        '
        Me.ConductorMaterialUCE.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ConductorMaterialUCE.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList
        Me.ConductorMaterialUCE.Name = "ConductorMaterialUCE"
        Me.ConductorMaterialUCE.TabIndex = 4
        '
        'MaterialResitivityUNE
        '
        Me.MaterialDensityUNE.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MaterialDensityUNE.FormatString = "###0.0# g/cm³"
        Me.MaterialDensityUNE.MaskInput = "{double:3.2}"
        Me.MaterialDensityUNE.MaxValue = 999.9999R
        Me.MaterialDensityUNE.MinValue = 0
        Me.MaterialDensityUNE.Name = "MaterialResitivityUNE"
        Me.MaterialDensityUNE.Nullable = True
        Me.MaterialDensityUNE.NullText = "0.0"
        Me.MaterialDensityUNE.NumericType = Infragistics.Win.UltraWinEditors.NumericType.[Double]
        Me.MaterialDensityUNE.PromptChar = Global.Microsoft.VisualBasic.ChrW(32)
        Me.MaterialDensityUNE.TabIndex = 1
        Me.MaterialDensityUNE.Value = 0.0R
        '
        'ResistivityCustomUNE
        '
        Me.ResistivityCustomUNE.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ResistivityCustomUNE.FormatString = "#####0.0### x10⁻⁸ Ωm"
        Me.ResistivityCustomUNE.MaskInput = "{double:4.3}"
        Me.ResistivityCustomUNE.MaxValue = 999.9999R
        Me.ResistivityCustomUNE.MinValue = 0
        Me.ResistivityCustomUNE.Name = "ResistivityCustomUNE"
        Me.ResistivityCustomUNE.Nullable = True
        Me.ResistivityCustomUNE.NullText = "0.0"
        Me.ResistivityCustomUNE.NumericType = Infragistics.Win.UltraWinEditors.NumericType.[Double]
        Me.ResistivityCustomUNE.PromptChar = Global.Microsoft.VisualBasic.ChrW(32)
        Me.ResistivityCustomUNE.TabIndex = 1
        Me.ResistivityCustomUNE.Value = 0.0R


        '
        'CsaUNE
        '
        Me.CsaUNE.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CsaUNE.FormatString = "#####0.0# mm²"
        Me.CsaUNE.MaskInput = "{double:6.2}"
        Me.CsaUNE.MaxValue = 999.9999R
        Me.CsaUNE.MinValue = 0
        Me.CsaUNE.Name = "CsaUNE"
        Me.CsaUNE.Nullable = True
        Me.CsaUNE.NumericType = Infragistics.Win.UltraWinEditors.NumericType.[Double]
        Me.CsaUNE.PromptChar = Global.Microsoft.VisualBasic.ChrW(32)
        Me.CsaUNE.TabIndex = 5
        '
        'TemperatureUNE
        '
        Me.TemperatureUNE.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TemperatureUNE.FormatString = "##0 °C"
        Me.TemperatureUNE.MaxValue = 200
        Me.TemperatureUNE.MinValue = -200
        Me.TemperatureUNE.Name = "TemperatureUNE"
        Me.TemperatureUNE.Nullable = True
        Me.TemperatureUNE.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Integer
        Me.TemperatureUNE.PromptChar = Global.Microsoft.VisualBasic.ChrW(32)
        Me.TemperatureUNE.TabIndex = 6
        '
        'LengthUNE
        '
        Me.LengthUNE.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LengthUNE.FormatString = "#####0.0 mm"
        Me.LengthUNE.MaskInput = "{double:6.1}"
        Me.LengthUNE.MaxValue = 999999.99R
        Me.LengthUNE.MinValue = 0
        Me.LengthUNE.Name = "LengthUNE"
        Me.LengthUNE.Nullable = True
        Me.LengthUNE.NullText = "0"
        Me.LengthUNE.NumericType = Infragistics.Win.UltraWinEditors.NumericType.[Double]
        Me.LengthUNE.PromptChar = Global.Microsoft.VisualBasic.ChrW(32)
        Me.LengthUNE.TabIndex = 7
        '
        'ConductorWeightUNE
        '
        Me.ConductorWeightUNE.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ConductorWeightUNE.FormatString = "#####0.0 g"
        Me.ConductorWeightUNE.MaxValue = 999999.99R
        Me.ConductorWeightUNE.MinValue = 0
        Me.ConductorWeightUNE.Name = "ConductorWeightUNE"
        Me.ConductorWeightUNE.Nullable = True
        Me.ConductorWeightUNE.NullText = "0.0 g"
        Me.ConductorWeightUNE.NumericType = Infragistics.Win.UltraWinEditors.NumericType.[Double]
        Me.ConductorWeightUNE.PromptChar = Global.Microsoft.VisualBasic.ChrW(32)
        Me.ConductorWeightUNE.ReadOnly = True
        Me.ConductorWeightUNE.TabIndex = 8
        '
        'TotalWeightUNE
        '
        Me.TotalWeightUNE.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TotalWeightUNE.FormatString = "#####0.0 g"
        Me.TotalWeightUNE.MaxValue = 999999.99R
        Me.TotalWeightUNE.MinValue = 0
        Me.TotalWeightUNE.Name = "TotalWeightUNE"
        Me.TotalWeightUNE.Nullable = True
        Me.TotalWeightUNE.NullText = "0.0 g"
        Me.TotalWeightUNE.NumericType = Infragistics.Win.UltraWinEditors.NumericType.[Double]
        Me.TotalWeightUNE.PromptChar = Global.Microsoft.VisualBasic.ChrW(32)
        Me.TotalWeightUNE.ReadOnly = True
        Me.TotalWeightUNE.TabIndex = 9
        '
        'LastVoltUNE
        '
        Me.EndResistanceUNE.Dock = System.Windows.Forms.DockStyle.Fill
        Me.EndResistanceUNE.FormatString = "###0.0# Ω"
        Me.EndResistanceUNE.MaskInput = "{double:3.2}"
        Me.EndResistanceUNE.MaxValue = 999.9999R
        Me.EndResistanceUNE.MinValue = 0
        Me.EndResistanceUNE.Name = "LastVoltUNE"
        Me.EndResistanceUNE.Nullable = True
        Me.EndResistanceUNE.NullText = "0.0"
        Me.EndResistanceUNE.NumericType = Infragistics.Win.UltraWinEditors.NumericType.[Double]
        Me.EndResistanceUNE.PromptChar = Global.Microsoft.VisualBasic.ChrW(32)
        Me.EndResistanceUNE.TabIndex = 10
        Me.EndResistanceUNE.Value = 0.1R
        '
        'ContentTableLayoutPanel
        '
        Me.ContentTableLayoutPanel.ColumnCount = 14
        Me.ContentTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5!))
        Me.ContentTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.5!))
        Me.ContentTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.5!))
        Me.ContentTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 23.0!))
        Me.ContentTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15.5!))
        Me.ContentTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.0!))
        Me.ContentTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22.5!))
        Me.ContentTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15.5!))
        Me.ContentTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15.5!))
        Me.ContentTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 21.5!))
        Me.ContentTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15.5!))
        Me.ContentTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18.0!))
        Me.ContentTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.5!))
        Me.ContentTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15.5!))

        Me.ContentTableLayoutPanel.Controls.Add(Me.StartResitanceUNE, 0, 0)
        Me.ContentTableLayoutPanel.Controls.Add(Me.WireOrCoreNumUTE, 1, 0)
        Me.ContentTableLayoutPanel.Controls.Add(Me.FirstContactPointUTE, 2, 0)
        Me.ContentTableLayoutPanel.Controls.Add(Me.TypeOfWireUCE, 3, 0)
        Me.ContentTableLayoutPanel.Controls.Add(Me.ConductorMaterialUCE, 4, 0)
        Me.ContentTableLayoutPanel.Controls.Add(Me.MaterialDensityUNE, 5, 0)
        Me.ContentTableLayoutPanel.Controls.Add(Me.ResistivityCustomUNE, 6, 0)
        Me.ContentTableLayoutPanel.Controls.Add(Me.CsaUNE, 7, 0)
        Me.ContentTableLayoutPanel.Controls.Add(Me.TemperatureUNE, 8, 0)
        Me.ContentTableLayoutPanel.Controls.Add(Me.LengthUNE, 9, 0)
        Me.ContentTableLayoutPanel.Controls.Add(Me.ConductorWeightUNE, 10, 0)
        Me.ContentTableLayoutPanel.Controls.Add(Me.TotalWeightUNE, 11, 0)
        Me.ContentTableLayoutPanel.Controls.Add(Me.LastContactPointUTE, 12, 0)
        Me.ContentTableLayoutPanel.Controls.Add(Me.EndResistanceUNE, 13, 0)
        Me.ContentTableLayoutPanel.Location = New System.Drawing.Point(0, 85)
        Me.ContentTableLayoutPanel.Name = "ContentTableLayoutPanel"
        Me.ContentTableLayoutPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.None
        Me.ContentTableLayoutPanel.Margin = New Padding(5)
        Me.ContentTableLayoutPanel.RowCount = 1
        Me.ContentTableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.ContentTableLayoutPanel.Size = New System.Drawing.Size(1500, 41)
        Me.ContentTableLayoutPanel.TabIndex = 0
        '
        'CurrentUserControl
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.AutoSize = True
        'Me.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Me.Controls.Add(Me.ContentTableLayoutPanel)
        Me.Name = "CurrentUserControl"
        Me.Size = New System.Drawing.Size(1400, 41)
        CType(Me.StartResitanceUNE, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WireOrCoreNumUTE, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.FirstContactPointUTE, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TypeOfWireUCE, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ConductorMaterialUCE, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.MaterialDensityUNE, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ResistivityCustomUNE, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CsaUNE, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TemperatureUNE, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LengthUNE, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ConductorWeightUNE, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TotalWeightUNE, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LastContactPointUTE, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.EndResistanceUNE, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ContentTableLayoutPanel.ResumeLayout(False)
        Me.ContentTableLayoutPanel.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents StartResitanceUNE As Infragistics.Win.UltraWinEditors.UltraNumericEditor
    Friend WithEvents CsaUNE As Infragistics.Win.UltraWinEditors.UltraNumericEditor
    Friend WithEvents TemperatureUNE As Infragistics.Win.UltraWinEditors.UltraNumericEditor
    Friend WithEvents LengthUNE As Infragistics.Win.UltraWinEditors.UltraNumericEditor
    Friend WithEvents ConductorWeightUNE As Infragistics.Win.UltraWinEditors.UltraNumericEditor
    Friend WithEvents TotalWeightUNE As Infragistics.Win.UltraWinEditors.UltraNumericEditor
    Friend WithEvents EndResistanceUNE As Infragistics.Win.UltraWinEditors.UltraNumericEditor
    Friend WithEvents MaterialDensityUNE As Infragistics.Win.UltraWinEditors.UltraNumericEditor
    Friend WithEvents ResistivityCustomUNE As Infragistics.Win.UltraWinEditors.UltraNumericEditor
    Friend WithEvents WireOrCoreNumUTE As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents FirstContactPointUTE As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents LastContactPointUTE As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents TypeOfWireUCE As Infragistics.Win.UltraWinEditors.UltraComboEditor
    Friend WithEvents ConductorMaterialUCE As Infragistics.Win.UltraWinEditors.UltraComboEditor
    Friend WithEvents ContentTableLayoutPanel As System.Windows.Forms.TableLayoutPanel
End Class
