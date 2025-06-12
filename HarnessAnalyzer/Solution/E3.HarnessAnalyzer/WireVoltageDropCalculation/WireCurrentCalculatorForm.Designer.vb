<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class WireCurrentCalculatorForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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
        Dim Appearance1 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Me.ExportUltraBtn = New Infragistics.Win.Misc.UltraButton()
        Me.CancelUltraBtn = New Infragistics.Win.Misc.UltraButton()
        Me.OutputTableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
        Me.RgesLbl = New Infragistics.Win.Misc.UltraLabel()
        Me.DeltaULbl = New Infragistics.Win.Misc.UltraLabel()
        Me.PvWattLbl = New Infragistics.Win.Misc.UltraLabel()
        Me.TotalConductorWeightLbl = New Infragistics.Win.Misc.UltraLabel()
        Me.TotalWeightLbl = New Infragistics.Win.Misc.UltraLabel()
        Me.RgesUltraFTE = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.DeltaUUltaFTE = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.PvWattUltraFTE = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.TotalConductorWeightUltraFTE = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.TotalWeightUltraFTE = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.Outputlbl = New Infragistics.Win.Misc.UltraLabel()
        Me.InputCurrentUltralbl = New Infragistics.Win.Misc.UltraLabel()
        Me.InputCurrentUNE = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
        Me.VectorDrawControlPanel = New Infragistics.Win.Misc.UltraPanel()
        Me.OutputTableLayoutPanel.SuspendLayout()
        CType(Me.RgesUltraFTE, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DeltaUUltaFTE, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PvWattUltraFTE, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TotalConductorWeightUltraFTE, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TotalWeightUltraFTE, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.InputCurrentUNE, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.VectorDrawControlPanel.SuspendLayout()
        Me.SuspendLayout()
        '
        'ExportUltraBtn
        '
        Me.ExportUltraBtn.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ExportUltraBtn.Location = New System.Drawing.Point(1660, 1620)
        Me.ExportUltraBtn.Name = "ExportUltraBtn"
        Me.ExportUltraBtn.Size = New System.Drawing.Size(237, 70)
        Me.ExportUltraBtn.TabIndex = 0
        Me.ExportUltraBtn.Text = "Export "
        '
        'CancelUltraBtn
        '
        Me.CancelUltraBtn.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CancelUltraBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.CancelUltraBtn.Location = New System.Drawing.Point(1938, 1620)
        Me.CancelUltraBtn.Name = "CancelUltraBtn"
        Me.CancelUltraBtn.Size = New System.Drawing.Size(257, 70)
        Me.CancelUltraBtn.TabIndex = 1
        Me.CancelUltraBtn.Text = "Cancel"
        '
        'OutputTableLayoutPanel
        '
        Me.OutputTableLayoutPanel.ColumnCount = 2
        Me.OutputTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 55.45707!))
        Me.OutputTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 44.54292!))
        Me.OutputTableLayoutPanel.Controls.Add(Me.RgesLbl, 0, 0)
        Me.OutputTableLayoutPanel.Controls.Add(Me.DeltaULbl, 0, 1)
        Me.OutputTableLayoutPanel.Controls.Add(Me.PvWattLbl, 0, 2)
        Me.OutputTableLayoutPanel.Controls.Add(Me.TotalConductorWeightLbl, 0, 3)
        Me.OutputTableLayoutPanel.Controls.Add(Me.TotalWeightLbl, 0, 4)
        Me.OutputTableLayoutPanel.Controls.Add(Me.RgesUltraFTE, 1, 0)
        Me.OutputTableLayoutPanel.Controls.Add(Me.DeltaUUltaFTE, 1, 1)
        Me.OutputTableLayoutPanel.Controls.Add(Me.PvWattUltraFTE, 1, 2)
        Me.OutputTableLayoutPanel.Controls.Add(Me.TotalConductorWeightUltraFTE, 1, 3)
        Me.OutputTableLayoutPanel.Controls.Add(Me.TotalWeightUltraFTE, 1, 4)
        Me.OutputTableLayoutPanel.Location = New System.Drawing.Point(36, 1348)
        Me.OutputTableLayoutPanel.Margin = New System.Windows.Forms.Padding(10)
        Me.OutputTableLayoutPanel.Name = "OutputTableLayoutPanel"
        Me.OutputTableLayoutPanel.RowCount = 5
        Me.OutputTableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 67.0!))
        Me.OutputTableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 67.0!))
        Me.OutputTableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 67.0!))
        Me.OutputTableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 67.0!))
        Me.OutputTableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 67.0!))
        Me.OutputTableLayoutPanel.Size = New System.Drawing.Size(647, 342)
        Me.OutputTableLayoutPanel.TabIndex = 3
        '
        'RgesLbl
        '
        Me.RgesLbl.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RgesLbl.Name = "RgesLbl"
        Me.RgesLbl.TabIndex = 0
        '
        'DeltaULbl
        '
        Me.DeltaULbl.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DeltaULbl.Name = "DeltaULbl"
        Me.DeltaULbl.TabIndex = 1
        '
        'PvWattLbl
        '
        Me.PvWattLbl.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PvWattLbl.Name = "PvWattLbl"
        Me.PvWattLbl.TabIndex = 0
        '
        'TotalConductorWeightLbl
        '
        Me.TotalConductorWeightLbl.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TotalConductorWeightLbl.Name = "TotalConductorWeightLbl"
        Me.TotalConductorWeightLbl.TabIndex = 0
        '
        'TotalWeightLbl
        '
        Me.TotalWeightLbl.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TotalWeightLbl.Name = "TotalWeightLbl"
        Me.TotalWeightLbl.TabIndex = 0
        '
        'RgesUltraFTE
        '
        Me.RgesUltraFTE.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RgesUltraFTE.Name = "RgesUltraFTE"
        Me.RgesUltraFTE.TabIndex = 2
        '
        'DeltaUUltaFTE
        '
        Me.DeltaUUltaFTE.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DeltaUUltaFTE.Name = "DeltaUUltaFTE"
        Me.DeltaUUltaFTE.TabIndex = 3
        '
        'PvWattUltraFTE
        '
        Me.PvWattUltraFTE.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PvWattUltraFTE.Name = "PvWattUltraFTE"
        Me.PvWattUltraFTE.Size = New System.Drawing.Size(283, 41)
        Me.PvWattUltraFTE.TabIndex = 4
        '
        'TotalConductorWeightUltraFTE
        '
        Me.TotalConductorWeightUltraFTE.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TotalConductorWeightUltraFTE.Name = "TotalConductorWeightUltraFTE"
        Me.TotalConductorWeightUltraFTE.TabIndex = 5
        '
        'TotalWeightUltraFTE
        '
        Me.TotalWeightUltraFTE.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TotalWeightUltraFTE.Name = "TotalWeightUltraFTE"
        Me.TotalWeightUltraFTE.TabIndex = 5
        '
        'Outputlbl
        '
        Appearance1.TextHAlignAsString = "Left"
        Appearance1.TextVAlignAsString = "Middle"
        Me.Outputlbl.Appearance = Appearance1
        Me.Outputlbl.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.900001!, System.Drawing.FontStyle.Bold)
        Me.Outputlbl.Location = New System.Drawing.Point(36, 1272)
        Me.Outputlbl.Margin = New System.Windows.Forms.Padding(5)
        Me.Outputlbl.Name = "Outputlbl"
        Me.Outputlbl.Size = New System.Drawing.Size(273, 55)
        Me.Outputlbl.TabIndex = 4
        '
        'InputCurrentUltralbl
        '
        Me.InputCurrentUltralbl.Name = "InputCurrentUltralbl"
        Me.InputCurrentUltralbl.Size = New System.Drawing.Size(264, 41)
        Me.InputCurrentUltralbl.TabIndex = 5
        '
        'InputCurrentUNE
        '
        Me.InputCurrentUNE.FormatString = "###0.0# A"
        Me.InputCurrentUNE.MaskInput = "{double:3.2}"
        Me.InputCurrentUNE.MaxValue = 999.9999R
        Me.InputCurrentUNE.MinValue = 0
        Me.InputCurrentUNE.Name = "InputCurrentUNE"
        Me.InputCurrentUNE.Nullable = True
        Me.InputCurrentUNE.NullText = "1"
        Me.InputCurrentUNE.NumericType = Infragistics.Win.UltraWinEditors.NumericType.[Double]
        Me.InputCurrentUNE.PromptChar = Global.Microsoft.VisualBasic.ChrW(32)
        Me.InputCurrentUNE.Size = New System.Drawing.Size(264, 41)
        Me.InputCurrentUNE.TabIndex = 6
        Me.InputCurrentUNE.Value = 1.0R
        '
        'VectorDrawControlPanel
        '
        Me.VectorDrawControlPanel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.VectorDrawControlPanel.Location = New System.Drawing.Point(0, 12)
        Me.VectorDrawControlPanel.Name = "VectorDrawControlPanel"
        Me.VectorDrawControlPanel.Size = New System.Drawing.Size(2244, 400)
        Me.VectorDrawControlPanel.TabIndex = 7
        '
        'WireCurrentCalculatorForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(240.0!, 240.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.CancelButton = Me.CancelUltraBtn
        Me.ClientSize = New System.Drawing.Size(2255, 1730)
        Me.Controls.Add(Me.VectorDrawControlPanel)
        Me.Controls.Add(Me.Outputlbl)
        Me.Controls.Add(Me.OutputTableLayoutPanel)
        Me.Controls.Add(Me.CancelUltraBtn)
        Me.Controls.Add(Me.ExportUltraBtn)
        Me.MinimizeBox = False
        Me.Name = "WireCurrentCalculatorForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.Text = "Voltage drop"
        Me.OutputTableLayoutPanel.ResumeLayout(False)
        Me.OutputTableLayoutPanel.PerformLayout()
        CType(Me.RgesUltraFTE, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DeltaUUltaFTE, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PvWattUltraFTE, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TotalConductorWeightUltraFTE, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TotalWeightUltraFTE, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.InputCurrentUNE, System.ComponentModel.ISupportInitialize).EndInit()
        Me.VectorDrawControlPanel.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents ExportUltraBtn As Infragistics.Win.Misc.UltraButton
    Friend WithEvents CancelUltraBtn As Infragistics.Win.Misc.UltraButton
    Friend WithEvents OutputTableLayoutPanel As TableLayoutPanel
    Friend WithEvents RgesLbl As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents DeltaULbl As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents PvWattLbl As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents TotalWeightLbl As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents TotalConductorWeightLbl As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents RgesUltraFTE As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents DeltaUUltaFTE As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents PvWattUltraFTE As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents TotalWeightUltraFTE As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents TotalConductorWeightUltraFTE As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents Outputlbl As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents InputCurrentUltralbl As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents InputCurrentUNE As Infragistics.Win.UltraWinEditors.UltraNumericEditor
    Friend WithEvents VectorDrawControlPanel As Infragistics.Win.Misc.UltraPanel

End Class
