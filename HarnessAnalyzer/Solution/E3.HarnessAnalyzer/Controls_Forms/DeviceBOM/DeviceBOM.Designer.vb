Imports System.ComponentModel

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class DeviceBOM
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
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

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New Container()
        Dim Appearance11 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim UltraGridBand1 As Infragistics.Win.UltraWinGrid.UltraGridBand = New Infragistics.Win.UltraWinGrid.UltraGridBand("DeviceBomCompareObjectModuleGroup", -1)
        Dim UltraGridColumn11 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("Module")
        Dim UltraGridColumn12 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("CompareObjects")
        Dim UltraGridBand2 As Infragistics.Win.UltraWinGrid.UltraGridBand = New Infragistics.Win.UltraWinGrid.UltraGridBand("CompareObjects", 0)
        Dim UltraGridColumn13 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("Key")
        Dim UltraGridColumn14 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("PartNumberLeft")
        Dim UltraGridColumn15 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("PartTypeLeft")
        Dim UltraGridColumn16 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("PartNumberRight")
        Dim UltraGridColumn17 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("PartTypeRight")
        Dim UltraGridColumn18 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("Module")
        Dim UltraGridColumn19 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("CurrentLength")
        Dim UltraGridColumn22 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("StatusLeft")
        Dim UltraGridColumn23 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("StatusRight")
        Dim UltraGridColumn24 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("Visible")
        Dim Appearance12 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance13 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance14 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        UltraGroupBox1 = New Infragistics.Win.Misc.UltraGroupBox()
        LeftDocumentComboBox = New Infragistics.Win.UltraWinEditors.UltraComboEditor()
        UltraGroupBox2 = New Infragistics.Win.Misc.UltraGroupBox()
        RightDocumentComboBox = New Infragistics.Win.UltraWinEditors.UltraComboEditor()
        UltraGroupBox3 = New Infragistics.Win.Misc.UltraGroupBox()
        UltraSplitter1 = New Infragistics.Win.Misc.UltraSplitter()
        UltraGrid1 = New Infragistics.Win.UltraWinGrid.UltraGrid()
        ContextMenuStrip3 = New ContextMenuStrip(components)
        CollapseAllToolStripMenuItem = New ToolStripMenuItem()
        ExpandAllModulesToolStripMenuItem = New ToolStripMenuItem()
        BindingSource1 = New BindingSource(components)
        UltraListView1 = New Infragistics.Win.UltraWinListView.UltraListView()
        ContextMenuStrip1 = New ContextMenuStrip(components)
        ResetSelectionToolStripMenuItem = New ToolStripMenuItem()
        Compare_Button = New Infragistics.Win.Misc.UltraButton()
        Close_Button = New Infragistics.Win.Misc.UltraButton()
        CType(UltraGroupBox1, ISupportInitialize).BeginInit()
        UltraGroupBox1.SuspendLayout()
        CType(LeftDocumentComboBox, ISupportInitialize).BeginInit()
        CType(UltraGroupBox2, ISupportInitialize).BeginInit()
        UltraGroupBox2.SuspendLayout()
        CType(RightDocumentComboBox, ISupportInitialize).BeginInit()
        CType(UltraGroupBox3, ISupportInitialize).BeginInit()
        UltraGroupBox3.SuspendLayout()
        CType(UltraGrid1, ISupportInitialize).BeginInit()
        ContextMenuStrip3.SuspendLayout()
        CType(BindingSource1, ISupportInitialize).BeginInit()
        CType(UltraListView1, ISupportInitialize).BeginInit()
        ContextMenuStrip1.SuspendLayout()
        SuspendLayout()
        ' 
        ' UltraGroupBox1
        ' 
        UltraGroupBox1.Controls.Add(LeftDocumentComboBox)
        UltraGroupBox1.Location = New Point(12, 12)
        UltraGroupBox1.Name = "UltraGroupBox1"
        UltraGroupBox1.Size = New Size(390, 66)
        UltraGroupBox1.TabIndex = 0
        UltraGroupBox1.Text = "Left"
        ' 
        ' LeftDocumentComboBox
        ' 
        LeftDocumentComboBox.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        LeftDocumentComboBox.Location = New Point(6, 23)
        LeftDocumentComboBox.Name = "LeftDocumentComboBox"
        LeftDocumentComboBox.Size = New Size(378, 25)
        LeftDocumentComboBox.TabIndex = 0
        ' 
        ' UltraGroupBox2
        ' 
        UltraGroupBox2.Controls.Add(RightDocumentComboBox)
        UltraGroupBox2.Location = New Point(408, 12)
        UltraGroupBox2.Name = "UltraGroupBox2"
        UltraGroupBox2.Size = New Size(380, 66)
        UltraGroupBox2.TabIndex = 1
        UltraGroupBox2.Text = "Right"
        ' 
        ' RightDocumentComboBox
        ' 
        RightDocumentComboBox.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        RightDocumentComboBox.Location = New Point(6, 23)
        RightDocumentComboBox.Name = "RightDocumentComboBox"
        RightDocumentComboBox.Size = New Size(368, 25)
        RightDocumentComboBox.TabIndex = 0
        ' 
        ' UltraGroupBox3
        ' 
        UltraGroupBox3.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        UltraGroupBox3.Controls.Add(UltraSplitter1)
        UltraGroupBox3.Controls.Add(UltraGrid1)
        UltraGroupBox3.Controls.Add(UltraListView1)
        UltraGroupBox3.Location = New Point(12, 84)
        UltraGroupBox3.Name = "UltraGroupBox3"
        UltraGroupBox3.Size = New Size(776, 321)
        UltraGroupBox3.TabIndex = 2
        UltraGroupBox3.Text = "Compare"
        ' 
        ' UltraSplitter1
        ' 
        UltraSplitter1.Location = New Point(279, 20)
        UltraSplitter1.Name = "UltraSplitter1"
        UltraSplitter1.RestoreExtent = 0
        UltraSplitter1.Size = New Size(6, 298)
        UltraSplitter1.TabIndex = 2
        ' 
        ' UltraGrid1
        ' 
        UltraGrid1.ContextMenuStrip = ContextMenuStrip3
        UltraGrid1.DataSource = BindingSource1
        Appearance11.BackColor = Color.Silver
        UltraGrid1.DisplayLayout.Appearance = Appearance11
        UltraGrid1.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns
        UltraGridColumn11.Header.VisiblePosition = 0
        UltraGridColumn11.Width = 454
        UltraGridColumn12.Header.VisiblePosition = 1
        UltraGridBand1.Columns.AddRange(New Object() {UltraGridColumn11, UltraGridColumn12})
        UltraGridColumn13.Header.VisiblePosition = 0
        UltraGridColumn13.Width = 40
        UltraGridColumn14.Header.VisiblePosition = 1
        UltraGridColumn14.Width = 48
        UltraGridColumn15.Header.VisiblePosition = 3
        UltraGridColumn15.Width = 45
        UltraGridColumn16.Header.VisiblePosition = 2
        UltraGridColumn16.Width = 55
        UltraGridColumn17.Header.VisiblePosition = 4
        UltraGridColumn17.Width = 44
        UltraGridColumn18.Header.VisiblePosition = 6
        UltraGridColumn18.Width = 60
        UltraGridColumn19.Header.VisiblePosition = 5
        UltraGridColumn19.Width = 43
        UltraGridColumn22.Header.VisiblePosition = 7
        UltraGridColumn22.Width = 58
        UltraGridColumn23.Header.VisiblePosition = 8
        UltraGridColumn23.Width = 42
        UltraGridColumn24.Header.VisiblePosition = 9
        UltraGridColumn24.Hidden = True
        UltraGridColumn24.Width = 67
        UltraGridBand2.Columns.AddRange(New Object() {UltraGridColumn13, UltraGridColumn14, UltraGridColumn15, UltraGridColumn16, UltraGridColumn17, UltraGridColumn18, UltraGridColumn19, UltraGridColumn22, UltraGridColumn23, UltraGridColumn24})
        UltraGrid1.DisplayLayout.BandsSerializer.Add(UltraGridBand1)
        UltraGrid1.DisplayLayout.BandsSerializer.Add(UltraGridBand2)
        UltraGrid1.DisplayLayout.Override.BorderStyleHeader = Infragistics.Win.UIElementBorderStyle.Raised
        Appearance12.BackColor = Color.Transparent
        UltraGrid1.DisplayLayout.Override.CardAreaAppearance = Appearance12
        Appearance13.BorderColor = Color.LightGray
        UltraGrid1.DisplayLayout.Override.RowAppearance = Appearance13
        Appearance14.BackColor = Color.Navy
        Appearance14.ForeColor = Color.White
        UltraGrid1.DisplayLayout.Override.SelectedRowAppearance = Appearance14
        UltraGrid1.DisplayLayout.UseFixedHeaders = True
        UltraGrid1.Dock = DockStyle.Fill
        UltraGrid1.Font = New Font("Segoe UI", 9F)
        UltraGrid1.Location = New Point(279, 20)
        UltraGrid1.Name = "UltraGrid1"
        UltraGrid1.Size = New Size(494, 298)
        UltraGrid1.TabIndex = 0
        UltraGrid1.UseOsThemes = Infragistics.Win.DefaultableBoolean.False
        ' 
        ' ContextMenuStrip3
        ' 
        ContextMenuStrip3.Items.AddRange(New ToolStripItem() {CollapseAllToolStripMenuItem, ExpandAllModulesToolStripMenuItem})
        ContextMenuStrip3.Name = "ContextMenuStrip3"
        ContextMenuStrip3.Size = New Size(184, 48)
        ' 
        ' CollapseAllToolStripMenuItem
        ' 
        CollapseAllToolStripMenuItem.Name = "CollapseAllToolStripMenuItem"
        CollapseAllToolStripMenuItem.Size = New Size(183, 22)
        CollapseAllToolStripMenuItem.Text = "Collapse all modules"
        ' 
        ' ExpandAllModulesToolStripMenuItem
        ' 
        ExpandAllModulesToolStripMenuItem.Name = "ExpandAllModulesToolStripMenuItem"
        ExpandAllModulesToolStripMenuItem.Size = New Size(183, 22)
        ExpandAllModulesToolStripMenuItem.Text = "Expand all modules"
        ' 
        ' BindingSource1
        ' 
        BindingSource1.DataSource = GetType(DeviceBomCompareObjectModuleGroup)
        ' 
        ' UltraListView1
        ' 
        UltraListView1.ContextMenuStrip = ContextMenuStrip1
        UltraListView1.Dock = DockStyle.Left
        UltraListView1.ImageTransparentColor = Color.Transparent
        UltraListView1.ItemSettings.HideSelection = False
        UltraListView1.Location = New Point(3, 20)
        UltraListView1.MainColumn.DataType = GetType(String)
        UltraListView1.MainColumn.Text = "Module"
        UltraListView1.Name = "UltraListView1"
        UltraListView1.Size = New Size(276, 298)
        UltraListView1.TabIndex = 1
        UltraListView1.Text = "UltraListView1"
        UltraListView1.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.Details
        UltraListView1.ViewSettingsDetails.AutoFitColumns = Infragistics.Win.UltraWinListView.AutoFitColumns.ResizeAllColumns
        UltraListView1.ViewSettingsDetails.FullRowSelect = True
        ' 
        ' ContextMenuStrip1
        ' 
        ContextMenuStrip1.Items.AddRange(New ToolStripItem() {ResetSelectionToolStripMenuItem})
        ContextMenuStrip1.Name = "ContextMenuStrip1"
        ContextMenuStrip1.Size = New Size(153, 26)
        ' 
        ' ResetSelectionToolStripMenuItem
        ' 
        ResetSelectionToolStripMenuItem.Name = "ResetSelectionToolStripMenuItem"
        ResetSelectionToolStripMenuItem.Size = New Size(152, 22)
        ResetSelectionToolStripMenuItem.Text = "Reset selection"
        ' 
        ' Compare_Button
        ' 
        Compare_Button.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        Compare_Button.Location = New Point(632, 411)
        Compare_Button.Name = "Compare_Button"
        Compare_Button.Size = New Size(75, 30)
        Compare_Button.TabIndex = 3
        Compare_Button.Text = "Compare"
        ' 
        ' Close_Button
        ' 
        Close_Button.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        Close_Button.Location = New Point(713, 411)
        Close_Button.Name = "Close_Button"
        Close_Button.Size = New Size(75, 30)
        Close_Button.TabIndex = 4
        Close_Button.Text = "Close"
        ' 
        ' DeviceBOM
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(800, 450)
        Controls.Add(Close_Button)
        Controls.Add(Compare_Button)
        Controls.Add(UltraGroupBox3)
        Controls.Add(UltraGroupBox2)
        Controls.Add(UltraGroupBox1)
        Name = "DeviceBOM"
        Text = "DeviceBOM"
        CType(UltraGroupBox1, ISupportInitialize).EndInit()
        UltraGroupBox1.ResumeLayout(False)
        UltraGroupBox1.PerformLayout()
        CType(LeftDocumentComboBox, ISupportInitialize).EndInit()
        CType(UltraGroupBox2, ISupportInitialize).EndInit()
        UltraGroupBox2.ResumeLayout(False)
        UltraGroupBox2.PerformLayout()
        CType(RightDocumentComboBox, ISupportInitialize).EndInit()
        CType(UltraGroupBox3, ISupportInitialize).EndInit()
        UltraGroupBox3.ResumeLayout(False)
        CType(UltraGrid1, ISupportInitialize).EndInit()
        ContextMenuStrip3.ResumeLayout(False)
        CType(BindingSource1, ISupportInitialize).EndInit()
        CType(UltraListView1, ISupportInitialize).EndInit()
        ContextMenuStrip1.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents UltraGroupBox1 As Infragistics.Win.Misc.UltraGroupBox
    Friend WithEvents LeftDocumentComboBox As Infragistics.Win.UltraWinEditors.UltraComboEditor
    Friend WithEvents UltraGroupBox2 As Infragistics.Win.Misc.UltraGroupBox
    Friend WithEvents RightDocumentComboBox As Infragistics.Win.UltraWinEditors.UltraComboEditor
    Friend WithEvents UltraGroupBox3 As Infragistics.Win.Misc.UltraGroupBox
    Friend WithEvents Compare_Button As Infragistics.Win.Misc.UltraButton
    Friend WithEvents Close_Button As Infragistics.Win.Misc.UltraButton
    Friend WithEvents UltraGrid1 As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents BindingSource1 As BindingSource
    Friend WithEvents UltraListView1 As Infragistics.Win.UltraWinListView.UltraListView
    Friend WithEvents UltraSplitter1 As Infragistics.Win.Misc.UltraSplitter
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents ResetSelectionToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ContextMenuStrip3 As ContextMenuStrip
    Friend WithEvents CollapseAllToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ExpandAllModulesToolStripMenuItem As ToolStripMenuItem
End Class
