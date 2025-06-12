Namespace D3D.Document.Controls.ToolTips

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class EntityToolTipControl
        Inherits System.Windows.Forms.UserControl

        'UserControl überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing Then
                    If components IsNot Nothing Then
                        components.Dispose()
                    End If
                    If Me.Properties IsNot Nothing Then
                        Me.Properties.Clear()
                    End If
                End If
                _entity = Nothing
                _Properties = Nothing
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
            Me.components = New System.ComponentModel.Container()
            Dim Appearance1 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim Appearance2 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim Appearance3 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim Appearance4 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim Appearance5 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
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
            Dim Appearance17 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim Appearance18 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim Appearance19 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim Appearance20 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim Appearance21 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim Appearance22 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim Appearance23 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim Appearance24 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim UltraListViewSubItemColumn1 As Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn = New Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("Value")
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(EntityToolTipControl))
            Me.UltraPanel1 = New Infragistics.Win.Misc.UltraPanel()
            Me.ugSupplements = New Infragistics.Win.UltraWinGrid.UltraGrid()
            Me.ContextMenuStripConnector = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.UltraSplitter2 = New Infragistics.Win.Misc.UltraSplitter()
            Me.ugConnector = New Infragistics.Win.UltraWinGrid.UltraGrid()
            Me.UltraSplitter1 = New Infragistics.Win.Misc.UltraSplitter()
            Me.GroupBox1 = New System.Windows.Forms.GroupBox()
            Me.PropertiesListView = New Infragistics.Win.UltraWinListView.UltraListView()
            Me.ContextMenuStripProperties = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
            Me.Label2 = New System.Windows.Forms.Label()
            Me.PartNumberTextBox = New Infragistics.Win.UltraWinEditors.UltraTextEditor
            Me.ContextMenuStripTextBox = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.CopyConnectorDataMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.CopyPropertiesMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.TextBoxToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.UltraPanel1.ClientArea.SuspendLayout()
            Me.UltraPanel1.SuspendLayout()
            CType(Me.ugSupplements, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.ContextMenuStripConnector.SuspendLayout()
            CType(Me.ugConnector, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.GroupBox1.SuspendLayout()
            CType(Me.PropertiesListView, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.ContextMenuStripProperties.SuspendLayout()
            Me.TableLayoutPanel1.SuspendLayout()
            Me.ContextMenuStripTextBox.SuspendLayout()
            Me.SuspendLayout()
            '
            'UltraPanel1
            '
            '
            'UltraPanel1.ClientArea
            '
            Me.UltraPanel1.ClientArea.Controls.Add(Me.ugSupplements)
            Me.UltraPanel1.ClientArea.Controls.Add(Me.UltraSplitter2)
            Me.UltraPanel1.ClientArea.Controls.Add(Me.ugConnector)
            Me.UltraPanel1.ClientArea.Controls.Add(Me.UltraSplitter1)
            Me.UltraPanel1.ClientArea.Controls.Add(Me.GroupBox1)
            Me.TableLayoutPanel1.SetColumnSpan(Me.UltraPanel1, 2)
            resources.ApplyResources(Me.UltraPanel1, "UltraPanel1")
            Me.UltraPanel1.Name = "UltraPanel1"
            '
            'ugSupplements
            '
            Me.ugSupplements.ContextMenuStrip = Me.ContextMenuStripConnector
            Appearance1.BackColor = System.Drawing.SystemColors.Window
            Appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption
            Me.ugSupplements.DisplayLayout.Appearance = Appearance1
            Me.ugSupplements.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns
            Me.ugSupplements.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
            Me.ugSupplements.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.[False]
            Appearance2.BackColor = System.Drawing.SystemColors.ActiveBorder
            Appearance2.BackColor2 = System.Drawing.SystemColors.ControlDark
            Appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical
            Appearance2.BorderColor = System.Drawing.SystemColors.Window
            Me.ugSupplements.DisplayLayout.GroupByBox.Appearance = Appearance2
            Appearance3.ForeColor = System.Drawing.SystemColors.GrayText
            Me.ugSupplements.DisplayLayout.GroupByBox.BandLabelAppearance = Appearance3
            Me.ugSupplements.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
            Appearance4.BackColor = System.Drawing.SystemColors.ControlLightLight
            Appearance4.BackColor2 = System.Drawing.SystemColors.Control
            Appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
            Appearance4.ForeColor = System.Drawing.SystemColors.GrayText
            Me.ugSupplements.DisplayLayout.GroupByBox.PromptAppearance = Appearance4
            Me.ugSupplements.DisplayLayout.MaxColScrollRegions = 1
            Me.ugSupplements.DisplayLayout.MaxRowScrollRegions = 1
            Appearance5.BackColor = System.Drawing.SystemColors.Window
            Appearance5.ForeColor = System.Drawing.SystemColors.ControlText
            Me.ugSupplements.DisplayLayout.Override.ActiveCellAppearance = Appearance5
            Appearance6.BackColor = System.Drawing.SystemColors.Highlight
            Appearance6.ForeColor = System.Drawing.SystemColors.HighlightText
            Me.ugSupplements.DisplayLayout.Override.ActiveRowAppearance = Appearance6
            Me.ugSupplements.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No
            Me.ugSupplements.DisplayLayout.Override.AllowColMoving = Infragistics.Win.UltraWinGrid.AllowColMoving.NotAllowed
            Me.ugSupplements.DisplayLayout.Override.AllowColSizing = Infragistics.Win.UltraWinGrid.AllowColSizing.Free
            Me.ugSupplements.DisplayLayout.Override.AllowColSwapping = Infragistics.Win.UltraWinGrid.AllowColSwapping.NotAllowed
            Me.ugSupplements.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.[False]
            Me.ugSupplements.DisplayLayout.Override.AllowMultiCellOperations = Infragistics.Win.UltraWinGrid.AllowMultiCellOperation.Copy
            Me.ugSupplements.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.[False]
            Me.ugSupplements.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted
            Me.ugSupplements.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted
            Appearance7.BackColor = System.Drawing.SystemColors.Window
            Me.ugSupplements.DisplayLayout.Override.CardAreaAppearance = Appearance7
            Appearance8.BorderColor = System.Drawing.Color.Silver
            Appearance8.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter
            Me.ugSupplements.DisplayLayout.Override.CellAppearance = Appearance8
            Me.ugSupplements.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect
            Me.ugSupplements.DisplayLayout.Override.CellPadding = 0
            Me.ugSupplements.DisplayLayout.Override.ColumnAutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.AllRowsInBand
            Appearance9.BackColor = System.Drawing.SystemColors.Control
            Appearance9.BackColor2 = System.Drawing.SystemColors.ControlDark
            Appearance9.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element
            Appearance9.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
            Appearance9.BorderColor = System.Drawing.SystemColors.Window
            Me.ugSupplements.DisplayLayout.Override.GroupByRowAppearance = Appearance9
            resources.ApplyResources(Appearance10, "Appearance10")
            Me.ugSupplements.DisplayLayout.Override.HeaderAppearance = Appearance10
            Me.ugSupplements.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti
            Me.ugSupplements.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand
            Appearance11.BackColor = System.Drawing.SystemColors.Window
            Appearance11.BorderColor = System.Drawing.Color.Silver
            Me.ugSupplements.DisplayLayout.Override.RowAppearance = Appearance11
            Me.ugSupplements.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.[False]
            Me.ugSupplements.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None
            Me.ugSupplements.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None
            Me.ugSupplements.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Extended
            Appearance12.BackColor = System.Drawing.SystemColors.ControlLight
            Me.ugSupplements.DisplayLayout.Override.TemplateAddRowAppearance = Appearance12
            Me.ugSupplements.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill
            Me.ugSupplements.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate
            Me.ugSupplements.DisplayLayout.TabNavigation = Infragistics.Win.UltraWinGrid.TabNavigation.NextControl
            Me.ugSupplements.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand
            resources.ApplyResources(Me.ugSupplements, "ugSupplements")
            Me.ugSupplements.Name = "ugSupplements"
            Me.ugSupplements.SyncWithCurrencyManager = False
            '
            'ContextMenuStripConnector
            '
            Me.ContextMenuStripConnector.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CopyConnectorDataMenuItem})
            Me.ContextMenuStripConnector.Name = "ContextMenuStrip1"
            resources.ApplyResources(Me.ContextMenuStripConnector, "ContextMenuStripConnector")
            '
            'CopyConnectorDataMenuItem
            '
            Me.UltraSplitter2.BackColor = System.Drawing.SystemColors.Control
            resources.ApplyResources(Me.UltraSplitter2, "UltraSplitter2")
            Me.UltraSplitter2.Name = "UltraSplitter2"
            Me.UltraSplitter2.RestoreExtent = 186
            '
            'ugConnector
            '
            Me.ugConnector.ContextMenuStrip = Me.ContextMenuStripConnector
            Appearance13.BackColor = System.Drawing.SystemColors.Window
            Appearance13.BorderColor = System.Drawing.SystemColors.InactiveCaption
            Me.ugConnector.DisplayLayout.Appearance = Appearance13
            Me.ugConnector.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns
            Me.ugConnector.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
            Me.ugConnector.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.[False]
            Appearance14.BackColor = System.Drawing.SystemColors.ActiveBorder
            Appearance14.BackColor2 = System.Drawing.SystemColors.ControlDark
            Appearance14.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical
            Appearance14.BorderColor = System.Drawing.SystemColors.Window
            Me.ugConnector.DisplayLayout.GroupByBox.Appearance = Appearance14
            Appearance15.ForeColor = System.Drawing.SystemColors.GrayText
            Me.ugConnector.DisplayLayout.GroupByBox.BandLabelAppearance = Appearance15
            Me.ugConnector.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
            Appearance16.BackColor = System.Drawing.SystemColors.ControlLightLight
            Appearance16.BackColor2 = System.Drawing.SystemColors.Control
            Appearance16.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
            Appearance16.ForeColor = System.Drawing.SystemColors.GrayText
            Me.ugConnector.DisplayLayout.GroupByBox.PromptAppearance = Appearance16
            Me.ugConnector.DisplayLayout.MaxColScrollRegions = 1
            Me.ugConnector.DisplayLayout.MaxRowScrollRegions = 1
            Appearance17.BackColor = System.Drawing.SystemColors.Window
            Appearance17.ForeColor = System.Drawing.SystemColors.ControlText
            Me.ugConnector.DisplayLayout.Override.ActiveCellAppearance = Appearance17
            Appearance18.BackColor = System.Drawing.SystemColors.Highlight
            Appearance18.ForeColor = System.Drawing.SystemColors.HighlightText
            Me.ugConnector.DisplayLayout.Override.ActiveRowAppearance = Appearance18
            Me.ugConnector.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No
            Me.ugConnector.DisplayLayout.Override.AllowColMoving = Infragistics.Win.UltraWinGrid.AllowColMoving.NotAllowed
            Me.ugConnector.DisplayLayout.Override.AllowColSizing = Infragistics.Win.UltraWinGrid.AllowColSizing.Free
            Me.ugConnector.DisplayLayout.Override.AllowColSwapping = Infragistics.Win.UltraWinGrid.AllowColSwapping.NotAllowed
            Me.ugConnector.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.[False]
            Me.ugConnector.DisplayLayout.Override.AllowMultiCellOperations = Infragistics.Win.UltraWinGrid.AllowMultiCellOperation.Copy
            Me.ugConnector.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.[False]
            Me.ugConnector.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted
            Me.ugConnector.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted
            Appearance19.BackColor = System.Drawing.SystemColors.Window
            Me.ugConnector.DisplayLayout.Override.CardAreaAppearance = Appearance19
            Appearance20.BorderColor = System.Drawing.Color.Silver
            Appearance20.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter
            Me.ugConnector.DisplayLayout.Override.CellAppearance = Appearance20
            Me.ugConnector.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect
            Me.ugConnector.DisplayLayout.Override.CellPadding = 0
            Me.ugConnector.DisplayLayout.Override.ColumnAutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.AllRowsInBand
            Appearance21.BackColor = System.Drawing.SystemColors.Control
            Appearance21.BackColor2 = System.Drawing.SystemColors.ControlDark
            Appearance21.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element
            Appearance21.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
            Appearance21.BorderColor = System.Drawing.SystemColors.Window
            Me.ugConnector.DisplayLayout.Override.GroupByRowAppearance = Appearance21
            resources.ApplyResources(Appearance22, "Appearance22")
            Me.ugConnector.DisplayLayout.Override.HeaderAppearance = Appearance22
            Me.ugConnector.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti
            Me.ugConnector.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand
            Appearance23.BackColor = System.Drawing.SystemColors.Window
            Appearance23.BorderColor = System.Drawing.Color.Silver
            Me.ugConnector.DisplayLayout.Override.RowAppearance = Appearance23
            Me.ugConnector.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.[False]
            Me.ugConnector.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None
            Me.ugConnector.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None
            Me.ugConnector.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Extended
            Appearance24.BackColor = System.Drawing.SystemColors.ControlLight
            Me.ugConnector.DisplayLayout.Override.TemplateAddRowAppearance = Appearance24
            Me.ugConnector.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill
            Me.ugConnector.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate
            Me.ugConnector.DisplayLayout.TabNavigation = Infragistics.Win.UltraWinGrid.TabNavigation.NextControl
            Me.ugConnector.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand
            resources.ApplyResources(Me.ugConnector, "ugConnector")
            Me.ugConnector.Name = "ugConnector"
            Me.ugConnector.SyncWithCurrencyManager = False
            '
            'UltraSplitter1
            '
            Me.UltraSplitter1.BackColor = System.Drawing.SystemColors.Control
            resources.ApplyResources(Me.UltraSplitter1, "UltraSplitter1")
            Me.UltraSplitter1.Name = "UltraSplitter1"
            Me.UltraSplitter1.RestoreExtent = 86
            '
            'GroupBox1
            '
            Me.GroupBox1.Controls.Add(Me.PropertiesListView)
            resources.ApplyResources(Me.GroupBox1, "GroupBox1")
            Me.GroupBox1.Name = "GroupBox1"
            Me.GroupBox1.TabStop = False
            '
            'PropertiesListView
            '
            resources.ApplyResources(Me.PropertiesListView, "PropertiesListView")
            Me.PropertiesListView.ContextMenuStrip = Me.ContextMenuStripProperties
            Me.PropertiesListView.ItemSettings.SelectionType = Infragistics.Win.UltraWinListView.SelectionType.[Single]
            Me.PropertiesListView.MainColumn.AutoSizeMode = Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.Header
            Me.PropertiesListView.MainColumn.Text = resources.GetString("PropertiesListView.MainColumn.Text")
            Me.PropertiesListView.Name = "PropertiesListView"
            Me.PropertiesListView.ShowGroups = False
            UltraListViewSubItemColumn1.AutoSizeMode = CType((Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.Header Or Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.VisibleItems), Infragistics.Win.UltraWinListView.ColumnAutoSizeMode)
            UltraListViewSubItemColumn1.Key = "Value"
            resources.ApplyResources(UltraListViewSubItemColumn1, "UltraListViewSubItemColumn1")
            Me.PropertiesListView.SubItemColumns.AddRange(New Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn() {UltraListViewSubItemColumn1})
            Me.PropertiesListView.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.Details
            Me.PropertiesListView.ViewSettingsDetails.AutoFitColumns = Infragistics.Win.UltraWinListView.AutoFitColumns.ResizeAllColumns
            Me.PropertiesListView.ViewSettingsDetails.ColumnHeadersVisible = False
            Me.PropertiesListView.ViewSettingsDetails.ColumnsShowSortIndicators = False
            Me.PropertiesListView.ViewSettingsDetails.FullRowSelect = True
            Me.PropertiesListView.ViewSettingsDetails.ImageSize = New System.Drawing.Size(0, 0)
            '
            'ContextMenuStripProperties
            '
            Me.ContextMenuStripProperties.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CopyPropertiesMenuItem})
            Me.ContextMenuStripProperties.Name = "ContextMenuStrip1"
            resources.ApplyResources(Me.ContextMenuStripProperties, "ContextMenuStripProperties")
            '
            'TableLayoutPanel1
            '
            resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
            Me.TableLayoutPanel1.Controls.Add(Me.Label2, 0, 1)
            Me.TableLayoutPanel1.Controls.Add(Me.UltraPanel1, 0, 2)
            Me.TableLayoutPanel1.Controls.Add(Me.PartNumberTextBox, 1, 1)
            Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
            '
            'Label2
            '
            resources.ApplyResources(Me.Label2, "Label2")
            Me.Label2.Name = "Label2"
            '
            'PartNumberTextBox
            '
            Me.PartNumberTextBox.ContextMenuStrip = Me.ContextMenuStripTextBox
            resources.ApplyResources(Me.PartNumberTextBox, "PartNumberTextBox")
            Me.PartNumberTextBox.Name = "PartNumberTextBox"
            Me.PartNumberTextBox.ReadOnly = True
            '
            'ContextMenuStripTextBox
            '
            Me.ContextMenuStripTextBox.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TextBoxToolStripMenuItem})
            Me.ContextMenuStripTextBox.Name = "ContextMenuStrip1"
            resources.ApplyResources(Me.ContextMenuStripTextBox, "ContextMenuStripTextBox")
            '
            'CopyConnectorDataMenuItem
            '
            Me.CopyConnectorDataMenuItem.Image = Global.Zuken.E3.HarnessAnalyzer.My.Resources.Resources.copy
            Me.CopyConnectorDataMenuItem.Name = "CopyConnectorDataMenuItem"
            resources.ApplyResources(Me.CopyConnectorDataMenuItem, "CopyConnectorDataMenuItem")
            '
            'CopyPropertiesMenuItem
            '
            Me.CopyPropertiesMenuItem.Image = Global.Zuken.E3.HarnessAnalyzer.My.Resources.Resources.copy
            Me.CopyPropertiesMenuItem.Name = "CopyPropertiesMenuItem"
            resources.ApplyResources(Me.CopyPropertiesMenuItem, "CopyPropertiesMenuItem")
            '
            'TextBoxToolStripMenuItem
            '
            Me.TextBoxToolStripMenuItem.Image = Global.Zuken.E3.HarnessAnalyzer.My.Resources.Resources.copy
            Me.TextBoxToolStripMenuItem.Name = "TextBoxToolStripMenuItem"
            resources.ApplyResources(Me.TextBoxToolStripMenuItem, "TextBoxToolStripMenuItem")
            '
            'ConnectorToolTipControl
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.TableLayoutPanel1)
            Me.Name = "ConnectorToolTipControl"
            Me.UltraPanel1.ClientArea.ResumeLayout(False)
            Me.UltraPanel1.ResumeLayout(False)
            CType(Me.ugSupplements, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ContextMenuStripConnector.ResumeLayout(False)
            CType(Me.ugConnector, System.ComponentModel.ISupportInitialize).EndInit()
            Me.GroupBox1.ResumeLayout(False)
            CType(Me.PropertiesListView, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ContextMenuStripProperties.ResumeLayout(False)
            Me.TableLayoutPanel1.ResumeLayout(False)
            Me.TableLayoutPanel1.PerformLayout()
            Me.ContextMenuStripTextBox.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents GroupBox1 As GroupBox
        Friend WithEvents PropertiesListView As Infragistics.Win.UltraWinListView.UltraListView
        Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
        Friend WithEvents PartNumberTextBox As Infragistics.Win.UltraWinEditors.UltraTextEditor
        Friend WithEvents Label2 As Label
        Friend WithEvents UltraPanel1 As Infragistics.Win.Misc.UltraPanel
        Friend WithEvents ugConnector As Infragistics.Win.UltraWinGrid.UltraGrid
        Friend WithEvents UltraSplitter1 As Infragistics.Win.Misc.UltraSplitter
        Friend WithEvents ContextMenuStripConnector As ContextMenuStrip
        Friend WithEvents CopyConnectorDataMenuItem As ToolStripMenuItem
        Friend WithEvents ContextMenuStripProperties As ContextMenuStrip
        Friend WithEvents CopyPropertiesMenuItem As ToolStripMenuItem
        Friend WithEvents ContextMenuStripTextBox As ContextMenuStrip
        Friend WithEvents TextBoxToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents ugSupplements As Infragistics.Win.UltraWinGrid.UltraGrid
        Friend WithEvents UltraSplitter2 As Infragistics.Win.Misc.UltraSplitter
    End Class

End Namespace