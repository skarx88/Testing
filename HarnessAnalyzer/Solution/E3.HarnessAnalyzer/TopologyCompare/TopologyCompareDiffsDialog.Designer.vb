<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TopologyCompareDiffsDialog
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
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

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(TopologyCompareDiffsDialog))
        Dim Appearance1 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim UltraGridBand1 As Infragistics.Win.UltraWinGrid.UltraGridBand = New Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1)
        Dim UltraGridColumn1 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("D1_Segments", -1, Nothing, 6966110, 0, 0)
        Dim Appearance2 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim UltraGridColumn2 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("D1_Length", -1, Nothing, 6966110, 1, 0)
        Dim Appearance3 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim UltraGridColumn3 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("D1_FixingID", -1, Nothing, 6966110, 2, 0)
        Dim UltraGridColumn4 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("D1_Fixing_PartNumber", -1, Nothing, 6966110, 3, 0)
        Dim UltraGridColumn5 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("D1_FixingPos", -1, Nothing, 6966110, 4, 0, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, False)
        Dim UltraGridColumn6 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("D2_Segments", -1, Nothing, 6966111, 0, 0)
        Dim Appearance4 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim UltraGridColumn7 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("D2_Length", -1, Nothing, 6966111, 1, 0)
        Dim Appearance5 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim UltraGridColumn8 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("D2_FixingID", -1, Nothing, 6966111, 2, 0)
        Dim UltraGridColumn9 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("D2_Fixing_PartNumber", -1, Nothing, 6966111, 3, 0)
        Dim UltraGridColumn10 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("D2_FixingPos", -1, Nothing, 6966111, 4, 0)
        Dim UltraGridGroup1 As Infragistics.Win.UltraWinGrid.UltraGridGroup = New Infragistics.Win.UltraWinGrid.UltraGridGroup("NewGroup0", 6966110)
        Dim UltraGridGroup2 As Infragistics.Win.UltraWinGrid.UltraGridGroup = New Infragistics.Win.UltraWinGrid.UltraGridGroup("RightModel", 6966111)
        Dim Appearance6 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance7 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance8 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance9 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance10 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance11 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance12 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance13 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance14 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance15 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance16 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim UltraDataColumn1 As Infragistics.Win.UltraWinDataSource.UltraDataColumn = New Infragistics.Win.UltraWinDataSource.UltraDataColumn("D1_Segments")
        Dim UltraDataColumn2 As Infragistics.Win.UltraWinDataSource.UltraDataColumn = New Infragistics.Win.UltraWinDataSource.UltraDataColumn("D1_Length")
        Dim UltraDataColumn3 As Infragistics.Win.UltraWinDataSource.UltraDataColumn = New Infragistics.Win.UltraWinDataSource.UltraDataColumn("D1_FixingID")
        Dim UltraDataColumn4 As Infragistics.Win.UltraWinDataSource.UltraDataColumn = New Infragistics.Win.UltraWinDataSource.UltraDataColumn("D1_Fixing_PartNumber")
        Dim UltraDataColumn5 As Infragistics.Win.UltraWinDataSource.UltraDataColumn = New Infragistics.Win.UltraWinDataSource.UltraDataColumn("D1_FixingPos")
        Dim UltraDataColumn6 As Infragistics.Win.UltraWinDataSource.UltraDataColumn = New Infragistics.Win.UltraWinDataSource.UltraDataColumn("D2_Segments")
        Dim UltraDataColumn7 As Infragistics.Win.UltraWinDataSource.UltraDataColumn = New Infragistics.Win.UltraWinDataSource.UltraDataColumn("D2_Length")
        Dim UltraDataColumn8 As Infragistics.Win.UltraWinDataSource.UltraDataColumn = New Infragistics.Win.UltraWinDataSource.UltraDataColumn("D2_FixingID")
        Dim UltraDataColumn9 As Infragistics.Win.UltraWinDataSource.UltraDataColumn = New Infragistics.Win.UltraWinDataSource.UltraDataColumn("D2_Fixing_PartNumber")
        Dim UltraDataColumn10 As Infragistics.Win.UltraWinDataSource.UltraDataColumn = New Infragistics.Win.UltraWinDataSource.UltraDataColumn("D2_FixingPos")
        Dim UltraToolTipInfo1 As Infragistics.Win.UltraWinToolTip.UltraToolTipInfo = New Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Fit excel columns to size that is required to make all content visible", Infragistics.Win.ToolTipImage.[Default], Nothing, Infragistics.Win.DefaultableBoolean.[Default])
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.btnExport = New System.Windows.Forms.Button()
        Me.ugCompareDifferences = New Infragistics.Win.UltraWinGrid.UltraGrid()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.CopyToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.AutoSizeColumnsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.UltraDataSource1 = New Infragistics.Win.UltraWinDataSource.UltraDataSource(Me.components)
        Me.UltraGridExcelExporter1 = New Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter(Me.components)
        Me.SaveExcelFileDialog = New System.Windows.Forms.SaveFileDialog()
        Me.chkIgnoreColumnSizes = New System.Windows.Forms.CheckBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.UltraToolTipManager1 = New Infragistics.Win.UltraWinToolTip.UltraToolTipManager(Me.components)
        Me.TableLayoutPanel1.SuspendLayout()
        CType(Me.ugCompareDifferences, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ContextMenuStrip1.SuspendLayout()
        CType(Me.UltraDataSource1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
        Me.TableLayoutPanel1.Controls.Add(Me.btnClose, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.btnExport, 0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        '
        'btnClose
        '
        Me.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel
        resources.ApplyResources(Me.btnClose, "btnClose")
        Me.btnClose.Name = "btnClose"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'btnExport
        '
        resources.ApplyResources(Me.btnExport, "btnExport")
        Me.btnExport.Name = "btnExport"
        '
        'ugCompareDifferences
        '
        resources.ApplyResources(Me.ugCompareDifferences, "ugCompareDifferences")
        Me.ugCompareDifferences.ContextMenuStrip = Me.ContextMenuStrip1
        Me.ugCompareDifferences.DataSource = Me.UltraDataSource1
        Appearance1.BackColor = System.Drawing.SystemColors.Window
        Appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption
        Me.ugCompareDifferences.DisplayLayout.Appearance = Appearance1
        Me.ugCompareDifferences.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns
        UltraGridColumn1.GroupByMode = Infragistics.Win.UltraWinGrid.GroupByMode.First2Characters
        resources.ApplyResources(UltraGridColumn1.Header, "UltraGridColumn1.Header")
        UltraGridColumn1.Header.Editor = Nothing
        resources.ApplyResources(Appearance2, "Appearance2")
        UltraGridColumn1.MergedCellAppearance = Appearance2
        UltraGridColumn1.MergedCellEvaluationType = Infragistics.Win.UltraWinGrid.MergedCellEvaluationType.MergeSameValue
        UltraGridColumn1.MergedCellStyle = Infragistics.Win.UltraWinGrid.MergedCellStyle.Always
        UltraGridColumn1.Width = 137
        UltraGridColumn1.ForceApplyResources = "Header"
        resources.ApplyResources(UltraGridColumn2, "UltraGridColumn2")
        UltraGridColumn2.GroupByMode = Infragistics.Win.UltraWinGrid.GroupByMode.First2Characters
        resources.ApplyResources(UltraGridColumn2.Header, "UltraGridColumn2.Header")
        UltraGridColumn2.Header.Editor = Nothing
        resources.ApplyResources(Appearance3, "Appearance3")
        UltraGridColumn2.MergedCellAppearance = Appearance3
        UltraGridColumn2.Nullable = Infragistics.Win.UltraWinGrid.Nullable.[Nothing]
        UltraGridColumn2.Width = 160
        UltraGridColumn2.ForceApplyResources = "Header|"
        UltraGridColumn3.GroupByMode = Infragistics.Win.UltraWinGrid.GroupByMode.First2Characters
        resources.ApplyResources(UltraGridColumn3.Header, "UltraGridColumn3.Header")
        UltraGridColumn3.Header.Editor = Nothing
        UltraGridColumn3.Width = 160
        UltraGridColumn3.ForceApplyResources = "Header"
        UltraGridColumn4.GroupByMode = Infragistics.Win.UltraWinGrid.GroupByMode.First2Characters
        resources.ApplyResources(UltraGridColumn4.Header, "UltraGridColumn4.Header")
        UltraGridColumn4.Header.Editor = Nothing
        UltraGridColumn4.Width = 132
        UltraGridColumn4.ForceApplyResources = "Header"
        UltraGridColumn5.GroupByMode = Infragistics.Win.UltraWinGrid.GroupByMode.First2Characters
        resources.ApplyResources(UltraGridColumn5.Header, "UltraGridColumn5.Header")
        UltraGridColumn5.Header.Editor = Nothing
        UltraGridColumn5.Width = 37
        UltraGridColumn5.ForceApplyResources = "Header"
        UltraGridColumn6.GroupByMode = Infragistics.Win.UltraWinGrid.GroupByMode.First2Characters
        resources.ApplyResources(UltraGridColumn6.Header, "UltraGridColumn6.Header")
        UltraGridColumn6.Header.Editor = Nothing
        resources.ApplyResources(Appearance4, "Appearance4")
        UltraGridColumn6.MergedCellAppearance = Appearance4
        UltraGridColumn6.MergedCellEvaluationType = Infragistics.Win.UltraWinGrid.MergedCellEvaluationType.MergeSameValue
        UltraGridColumn6.MergedCellStyle = Infragistics.Win.UltraWinGrid.MergedCellStyle.Always
        UltraGridColumn6.Width = 165
        UltraGridColumn6.ForceApplyResources = "Header"
        resources.ApplyResources(UltraGridColumn7, "UltraGridColumn7")
        UltraGridColumn7.GroupByMode = Infragistics.Win.UltraWinGrid.GroupByMode.First2Characters
        resources.ApplyResources(UltraGridColumn7.Header, "UltraGridColumn7.Header")
        UltraGridColumn7.Header.Editor = Nothing
        resources.ApplyResources(Appearance5, "Appearance5")
        UltraGridColumn7.MergedCellAppearance = Appearance5
        UltraGridColumn7.Nullable = Infragistics.Win.UltraWinGrid.Nullable.[Nothing]
        UltraGridColumn7.Width = 165
        UltraGridColumn7.ForceApplyResources = "Header|"
        UltraGridColumn8.GroupByMode = Infragistics.Win.UltraWinGrid.GroupByMode.First2Characters
        resources.ApplyResources(UltraGridColumn8.Header, "UltraGridColumn8.Header")
        UltraGridColumn8.Header.Editor = Nothing
        UltraGridColumn8.Width = 165
        UltraGridColumn8.ForceApplyResources = "Header"
        UltraGridColumn9.GroupByMode = Infragistics.Win.UltraWinGrid.GroupByMode.First2Characters
        resources.ApplyResources(UltraGridColumn9.Header, "UltraGridColumn9.Header")
        UltraGridColumn9.Header.Editor = Nothing
        UltraGridColumn9.Width = 109
        UltraGridColumn9.ForceApplyResources = "Header"
        UltraGridColumn10.GroupByMode = Infragistics.Win.UltraWinGrid.GroupByMode.First2Characters
        resources.ApplyResources(UltraGridColumn10.Header, "UltraGridColumn10.Header")
        UltraGridColumn10.Header.Editor = Nothing
        UltraGridColumn10.Width = 60
        UltraGridColumn10.ForceApplyResources = "Header"
        UltraGridBand1.Columns.AddRange(New Object() {UltraGridColumn1, UltraGridColumn2, UltraGridColumn3, UltraGridColumn4, UltraGridColumn5, UltraGridColumn6, UltraGridColumn7, UltraGridColumn8, UltraGridColumn9, UltraGridColumn10})
        UltraGridGroup1.Header.Caption = resources.GetString("resource.Caption")
        UltraGridGroup1.Header.Editor = Nothing
        UltraGridGroup1.Key = "NewGroup0"
        UltraGridGroup1.RowLayoutGroupInfo.LabelSpan = 1
        UltraGridGroup2.Key = "RightModel"
        UltraGridGroup2.RowLayoutGroupInfo.LabelSpan = 1
        UltraGridBand1.Groups.AddRange(New Infragistics.Win.UltraWinGrid.UltraGridGroup() {UltraGridGroup1, UltraGridGroup2})
        UltraGridBand1.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.[Select]
        Me.ugCompareDifferences.DisplayLayout.BandsSerializer.Add(UltraGridBand1)
        Me.ugCompareDifferences.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        Me.ugCompareDifferences.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.[False]
        Appearance6.BackColor = System.Drawing.SystemColors.ActiveBorder
        Appearance6.BackColor2 = System.Drawing.SystemColors.ControlDark
        Appearance6.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical
        Appearance6.BorderColor = System.Drawing.SystemColors.Window
        Me.ugCompareDifferences.DisplayLayout.GroupByBox.Appearance = Appearance6
        Appearance7.ForeColor = System.Drawing.SystemColors.GrayText
        Me.ugCompareDifferences.DisplayLayout.GroupByBox.BandLabelAppearance = Appearance7
        Me.ugCompareDifferences.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        Appearance8.BackColor = System.Drawing.SystemColors.ControlLightLight
        Appearance8.BackColor2 = System.Drawing.SystemColors.Control
        Appearance8.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance8.ForeColor = System.Drawing.SystemColors.GrayText
        Me.ugCompareDifferences.DisplayLayout.GroupByBox.PromptAppearance = Appearance8
        Me.ugCompareDifferences.DisplayLayout.MaxColScrollRegions = 1
        Me.ugCompareDifferences.DisplayLayout.MaxRowScrollRegions = 1
        Appearance9.BackColor = System.Drawing.SystemColors.Window
        Appearance9.ForeColor = System.Drawing.SystemColors.ControlText
        Me.ugCompareDifferences.DisplayLayout.Override.ActiveCellAppearance = Appearance9
        Appearance10.BackColor = System.Drawing.SystemColors.Highlight
        Appearance10.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.ugCompareDifferences.DisplayLayout.Override.ActiveRowAppearance = Appearance10
        Me.ugCompareDifferences.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No
        Me.ugCompareDifferences.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.[False]
        Me.ugCompareDifferences.DisplayLayout.Override.AllowMultiCellOperations = Infragistics.Win.UltraWinGrid.AllowMultiCellOperation.Copy
        Me.ugCompareDifferences.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted
        Me.ugCompareDifferences.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted
        Appearance11.BackColor = System.Drawing.SystemColors.Window
        Me.ugCompareDifferences.DisplayLayout.Override.CardAreaAppearance = Appearance11
        Appearance12.BorderColor = System.Drawing.Color.Silver
        Appearance12.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter
        Me.ugCompareDifferences.DisplayLayout.Override.CellAppearance = Appearance12
        Me.ugCompareDifferences.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect
        Me.ugCompareDifferences.DisplayLayout.Override.CellPadding = 0
        Appearance13.BackColor = System.Drawing.SystemColors.Control
        Appearance13.BackColor2 = System.Drawing.SystemColors.ControlDark
        Appearance13.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element
        Appearance13.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance13.BorderColor = System.Drawing.SystemColors.Window
        Me.ugCompareDifferences.DisplayLayout.Override.GroupByRowAppearance = Appearance13
        resources.ApplyResources(Appearance14, "Appearance14")
        Me.ugCompareDifferences.DisplayLayout.Override.HeaderAppearance = Appearance14
        Me.ugCompareDifferences.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti
        Me.ugCompareDifferences.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand
        Appearance15.BackColor = System.Drawing.SystemColors.Window
        Appearance15.BorderColor = System.Drawing.Color.Silver
        Me.ugCompareDifferences.DisplayLayout.Override.RowAppearance = Appearance15
        Me.ugCompareDifferences.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.[False]
        Me.ugCompareDifferences.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None
        Me.ugCompareDifferences.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None
        Me.ugCompareDifferences.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Extended
        Appearance16.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ugCompareDifferences.DisplayLayout.Override.TemplateAddRowAppearance = Appearance16
        Me.ugCompareDifferences.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill
        Me.ugCompareDifferences.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate
        Me.ugCompareDifferences.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand
        Me.ugCompareDifferences.Name = "ugCompareDifferences"
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CopyToolStripMenuItem, Me.ToolStripSeparator1, Me.AutoSizeColumnsToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        resources.ApplyResources(Me.ContextMenuStrip1, "ContextMenuStrip1")
        '
        'CopyToolStripMenuItem
        '
        Me.CopyToolStripMenuItem.Name = "CopyToolStripMenuItem"
        resources.ApplyResources(Me.CopyToolStripMenuItem, "CopyToolStripMenuItem")
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        resources.ApplyResources(Me.ToolStripSeparator1, "ToolStripSeparator1")
        '
        'AutoSizeColumnsToolStripMenuItem
        '
        Me.AutoSizeColumnsToolStripMenuItem.Name = "AutoSizeColumnsToolStripMenuItem"
        resources.ApplyResources(Me.AutoSizeColumnsToolStripMenuItem, "AutoSizeColumnsToolStripMenuItem")
        '
        'UltraDataSource1
        '
        UltraDataColumn2.DataType = GetType(Integer)
        UltraDataColumn7.DataType = GetType(Integer)
        Me.UltraDataSource1.Band.Columns.AddRange(New Object() {UltraDataColumn1, UltraDataColumn2, UltraDataColumn3, UltraDataColumn4, UltraDataColumn5, UltraDataColumn6, UltraDataColumn7, UltraDataColumn8, UltraDataColumn9, UltraDataColumn10})
        '
        'UltraGridExcelExporter1
        '
        '
        'SaveExcelFileDialog
        '
        Me.SaveExcelFileDialog.DefaultExt = IO.KnownFile.XLSX.Trim("."c)
        resources.ApplyResources(Me.SaveExcelFileDialog, "SaveExcelFileDialog")
        '
        'chkIgnoreColumnSizes
        '
        resources.ApplyResources(Me.chkIgnoreColumnSizes, "chkIgnoreColumnSizes")
        Me.chkIgnoreColumnSizes.Name = "chkIgnoreColumnSizes"
        resources.ApplyResources(UltraToolTipInfo1, "UltraToolTipInfo1")
        Me.UltraToolTipManager1.SetUltraToolTip(Me.chkIgnoreColumnSizes, UltraToolTipInfo1)
        Me.chkIgnoreColumnSizes.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        resources.ApplyResources(Me.GroupBox1, "GroupBox1")
        Me.GroupBox1.Controls.Add(Me.chkIgnoreColumnSizes)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.TabStop = False
        '
        'UltraToolTipManager1
        '
        Me.UltraToolTipManager1.ContainingControl = Me
        '
        'TopologyCompareDiffsDialog
        '
        Me.AcceptButton = Me.btnExport
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnClose
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.ugCompareDifferences)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.MinimizeBox = False
        Me.Name = "TopologyCompareDiffsDialog"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.TableLayoutPanel1.ResumeLayout(False)
        CType(Me.ugCompareDifferences, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ContextMenuStrip1.ResumeLayout(False)
        CType(Me.UltraDataSource1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents btnExport As System.Windows.Forms.Button
    Friend WithEvents ugCompareDifferences As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents btnClose As Button
    Friend WithEvents UltraDataSource1 As Infragistics.Win.UltraWinDataSource.UltraDataSource
    Friend WithEvents UltraGridExcelExporter1 As Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter
    Friend WithEvents SaveExcelFileDialog As SaveFileDialog
    Friend WithEvents chkIgnoreColumnSizes As CheckBox
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents CopyToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents AutoSizeColumnsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents UltraToolTipManager1 As Infragistics.Win.UltraWinToolTip.UltraToolTipManager
End Class
