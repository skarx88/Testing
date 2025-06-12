Namespace Compare.Table

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class DiffDefaultPropsWarningDialog
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
        '  <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DiffDefaultPropsWarningDialog))
            Dim UltraTreeColumnSet1 As Infragistics.Win.UltraWinTree.UltraTreeColumnSet = New Infragistics.Win.UltraWinTree.UltraTreeColumnSet()
            Dim UltraTreeNodeColumn1 As Infragistics.Win.UltraWinTree.UltraTreeNodeColumn = New Infragistics.Win.UltraWinTree.UltraTreeNodeColumn()
            Dim UltraTreeNodeColumn2 As Infragistics.Win.UltraWinTree.UltraTreeNodeColumn = New Infragistics.Win.UltraWinTree.UltraTreeNodeColumn()
            Dim UltraTreeColumnSet2 As Infragistics.Win.UltraWinTree.UltraTreeColumnSet = New Infragistics.Win.UltraWinTree.UltraTreeColumnSet()
            Dim UltraTreeNodeColumn3 As Infragistics.Win.UltraWinTree.UltraTreeNodeColumn = New Infragistics.Win.UltraWinTree.UltraTreeNodeColumn()
            Dim UltraTreeNodeColumn4 As Infragistics.Win.UltraWinTree.UltraTreeNodeColumn = New Infragistics.Win.UltraWinTree.UltraTreeNodeColumn()
            Dim UltraTreeNodeColumn5 As Infragistics.Win.UltraWinTree.UltraTreeNodeColumn = New Infragistics.Win.UltraWinTree.UltraTreeNodeColumn()
            Dim Override1 As Infragistics.Win.UltraWinTree.Override = New Infragistics.Win.UltraWinTree.Override()
            Dim PopupMenuTool1 As Infragistics.Win.UltraWinToolbars.PopupMenuTool = New Infragistics.Win.UltraWinToolbars.PopupMenuTool("PopupMenuTool")
            Dim ButtonTool3 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("CollapseAll")
            Dim ButtonTool4 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("ExpandAll")
            Dim ButtonTool6 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("Copy")
            Dim ButtonTool8 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("SelectAll")
            Dim ButtonTool1 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("ExpandAll")
            Dim ButtonTool2 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("CollapseAll")
            Dim ButtonTool5 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("Copy")
            Dim ButtonTool7 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("SelectAll")
            Me.UltraCheckEditor1 = New Infragistics.Win.UltraWinEditors.UltraCheckEditor()
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
            Me.Close_Button = New System.Windows.Forms.Button()
            Me.UltraToolTipManager1 = New Infragistics.Win.UltraWinToolTip.UltraToolTipManager(Me.components)
            Me.utDifferences = New Infragistics.Win.UltraWinTree.UltraTree()
            Me.ColumnViewBindingSource = New System.Windows.Forms.BindingSource(Me.components)
            Me._DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Left = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
            Me.UltraToolbarsManager1 = New Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(Me.components)
            Me._DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Right = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
            Me._DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Top = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
            Me._DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Bottom = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
            CType(Me.UltraCheckEditor1, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.TableLayoutPanel1.SuspendLayout()
            CType(Me.utDifferences, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.ColumnViewBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.UltraToolbarsManager1, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'UltraCheckEditor1
            '
            resources.ApplyResources(Me.UltraCheckEditor1, "UltraCheckEditor1")
            Me.UltraCheckEditor1.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter
            Me.UltraCheckEditor1.Name = "UltraCheckEditor1"
            '
            'TableLayoutPanel1
            '
            resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
            Me.TableLayoutPanel1.Controls.Add(Me.Close_Button, 1, 0)
            Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
            '
            'Close_Button
            '
            resources.ApplyResources(Me.Close_Button, "Close_Button")
            Me.Close_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Close_Button.Name = "Close_Button"
            '
            'UltraToolTipManager1
            '
            Me.UltraToolTipManager1.ContainingControl = Me
            '
            'utDifferences
            '
            resources.ApplyResources(Me.utDifferences, "utDifferences")
            Me.utDifferences.ColumnSettings.AutoFitColumns = Infragistics.Win.UltraWinTree.AutoFitColumns.ResizeAllColumns
            UltraTreeColumnSet1.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.None
            UltraTreeColumnSet1.ColumnAutoSizeMode = Infragistics.Win.UltraWinTree.ColumnAutoSizeMode.AllNodes
            UltraTreeNodeColumn1.ButtonDisplayStyle = Infragistics.Win.UltraWinTree.ButtonDisplayStyle.Always
            UltraTreeNodeColumn1.DataType = GetType(String)
            UltraTreeNodeColumn1.Key = "ColumnName"
            UltraTreeNodeColumn1.LayoutInfo.PreferredCellSize = New System.Drawing.Size(565, 22)
            UltraTreeNodeColumn1.LayoutInfo.PreferredLabelSize = New System.Drawing.Size(565, 26)
            UltraTreeNodeColumn1.LayoutInfo.SpanX = 2
            UltraTreeNodeColumn1.LayoutInfo.SpanY = 2
            UltraTreeNodeColumn2.ButtonDisplayStyle = Infragistics.Win.UltraWinTree.ButtonDisplayStyle.Always
            UltraTreeNodeColumn2.IsChaptered = True
            UltraTreeNodeColumn2.Key = "Properties"
            UltraTreeNodeColumn2.LayoutInfo.SpanX = 2
            UltraTreeNodeColumn2.LayoutInfo.SpanY = 2
            UltraTreeColumnSet1.Columns.Add(UltraTreeNodeColumn1)
            UltraTreeColumnSet1.Columns.Add(UltraTreeNodeColumn2)
            UltraTreeColumnSet1.IsAutoGenerated = True
            UltraTreeColumnSet1.LabelPosition = Infragistics.Win.UltraWinTree.NodeLayoutLabelPosition.None
            UltraTreeNodeColumn3.ButtonDisplayStyle = Infragistics.Win.UltraWinTree.ButtonDisplayStyle.Always
            UltraTreeNodeColumn3.DataType = GetType(String)
            UltraTreeNodeColumn3.Key = "Name"
            UltraTreeNodeColumn3.LayoutInfo.PreferredCellSize = New System.Drawing.Size(220, 22)
            UltraTreeNodeColumn3.LayoutInfo.PreferredLabelSize = New System.Drawing.Size(220, 0)
            UltraTreeNodeColumn3.LayoutInfo.SpanX = 2
            UltraTreeNodeColumn3.LayoutInfo.SpanY = 2
            resources.ApplyResources(UltraTreeNodeColumn3, "UltraTreeNodeColumn3")
            UltraTreeNodeColumn4.ButtonDisplayStyle = Infragistics.Win.UltraWinTree.ButtonDisplayStyle.Always
            UltraTreeNodeColumn4.DataType = GetType(Boolean)
            UltraTreeNodeColumn4.EditorComponent = Me.UltraCheckEditor1
            UltraTreeNodeColumn4.Key = "Current"
            UltraTreeNodeColumn4.LayoutInfo.PreferredCellSize = New System.Drawing.Size(165, 18)
            UltraTreeNodeColumn4.LayoutInfo.PreferredLabelSize = New System.Drawing.Size(165, 0)
            UltraTreeNodeColumn4.LayoutInfo.SpanX = 2
            UltraTreeNodeColumn4.LayoutInfo.SpanY = 2
            resources.ApplyResources(UltraTreeNodeColumn4, "UltraTreeNodeColumn4")
            UltraTreeNodeColumn5.ButtonDisplayStyle = Infragistics.Win.UltraWinTree.ButtonDisplayStyle.Always
            UltraTreeNodeColumn5.DataType = GetType(Boolean)
            UltraTreeNodeColumn5.EditorComponent = Me.UltraCheckEditor1
            UltraTreeNodeColumn5.Key = "Default"
            UltraTreeNodeColumn5.LayoutInfo.PreferredCellSize = New System.Drawing.Size(208, 18)
            UltraTreeNodeColumn5.LayoutInfo.PreferredLabelSize = New System.Drawing.Size(208, 0)
            UltraTreeNodeColumn5.LayoutInfo.SpanX = 2
            UltraTreeNodeColumn5.LayoutInfo.SpanY = 2
            resources.ApplyResources(UltraTreeNodeColumn5, "UltraTreeNodeColumn5")
            UltraTreeColumnSet2.Columns.Add(UltraTreeNodeColumn3)
            UltraTreeColumnSet2.Columns.Add(UltraTreeNodeColumn4)
            UltraTreeColumnSet2.Columns.Add(UltraTreeNodeColumn5)
            UltraTreeColumnSet2.IsAutoGenerated = True
            UltraTreeColumnSet2.Key = "Properties"
            Me.utDifferences.ColumnSettings.ColumnSets.Add(UltraTreeColumnSet1)
            Me.utDifferences.ColumnSettings.ColumnSets.Add(UltraTreeColumnSet2)
            Me.UltraToolbarsManager1.SetContextMenuUltra(Me.utDifferences, "PopupMenuTool")
            Me.utDifferences.DataSource = Me.ColumnViewBindingSource
            Me.utDifferences.FullRowSelect = True
            Me.utDifferences.Name = "utDifferences"
            Override1.SelectionType = Infragistics.Win.UltraWinTree.SelectType.Extended
            Override1.ShowExpansionIndicator = Infragistics.Win.UltraWinTree.ShowExpansionIndicator.CheckOnDisplay
            Me.utDifferences.Override = Override1
            Me.utDifferences.ViewStyle = Infragistics.Win.UltraWinTree.ViewStyle.Grid
            '
            'ColumnViewBindingSource
            '
            Me.ColumnViewBindingSource.DataSource = GetType(Zuken.E3.HarnessAnalyzer.Compare.Table.ColumnView)
            '
            '_DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Left
            '
            resources.ApplyResources(Me._DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Left, "_DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Left")
            Me._DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
            Me._DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control
            Me._DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left
            Me._DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText
            Me._DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Left.Name = "_DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Left"
            Me._DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Left.ToolbarsManager = Me.UltraToolbarsManager1
            '
            'UltraToolbarsManager1
            '
            Me.UltraToolbarsManager1.DesignerFlags = 1
            Me.UltraToolbarsManager1.DockWithinContainer = Me
            Me.UltraToolbarsManager1.DockWithinContainerBaseType = GetType(System.Windows.Forms.Form)
            Me.UltraToolbarsManager1.ShowFullMenusDelay = 500
            resources.ApplyResources(PopupMenuTool1.SharedPropsInternal, "PopupMenuTool1.SharedPropsInternal")
            PopupMenuTool1.Tools.AddRange(New Infragistics.Win.UltraWinToolbars.ToolBase() {ButtonTool3, ButtonTool4, ButtonTool6, ButtonTool8})
            PopupMenuTool1.ForceApplyResources = "SharedPropsInternal"
            resources.ApplyResources(ButtonTool1.SharedPropsInternal, "ButtonTool1.SharedPropsInternal")
            ButtonTool1.ForceApplyResources = "SharedPropsInternal"
            resources.ApplyResources(ButtonTool2.SharedPropsInternal, "ButtonTool2.SharedPropsInternal")
            ButtonTool2.ForceApplyResources = "SharedPropsInternal"
            resources.ApplyResources(ButtonTool5.SharedPropsInternal, "ButtonTool5.SharedPropsInternal")
            ButtonTool5.SharedPropsInternal.Shortcut = System.Windows.Forms.Shortcut.CtrlC
            ButtonTool5.ForceApplyResources = "SharedPropsInternal"
            resources.ApplyResources(ButtonTool7.SharedPropsInternal, "ButtonTool7.SharedPropsInternal")
            ButtonTool7.SharedPropsInternal.Shortcut = System.Windows.Forms.Shortcut.CtrlA
            ButtonTool7.ForceApplyResources = "SharedPropsInternal"
            Me.UltraToolbarsManager1.Tools.AddRange(New Infragistics.Win.UltraWinToolbars.ToolBase() {PopupMenuTool1, ButtonTool1, ButtonTool2, ButtonTool5, ButtonTool7})
            '
            '_DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Right
            '
            resources.ApplyResources(Me._DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Right, "_DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Right")
            Me._DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
            Me._DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control
            Me._DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right
            Me._DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText
            Me._DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Right.Name = "_DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Right"
            Me._DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Right.ToolbarsManager = Me.UltraToolbarsManager1
            '
            '_DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Top
            '
            resources.ApplyResources(Me._DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Top, "_DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Top")
            Me._DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
            Me._DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control
            Me._DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top
            Me._DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText
            Me._DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Top.Name = "_DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Top"
            Me._DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Top.ToolbarsManager = Me.UltraToolbarsManager1
            '
            '_DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Bottom
            '
            resources.ApplyResources(Me._DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Bottom, "_DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Bottom")
            Me._DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
            Me._DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control
            Me._DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom
            Me._DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText
            Me._DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Bottom.Name = "_DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Bottom"
            Me._DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Bottom.ToolbarsManager = Me.UltraToolbarsManager1
            '
            'DiffDefaultPropsWarningDialog
            '
            Me.AcceptButton = Me.Close_Button
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.CancelButton = Me.Close_Button
            Me.Controls.Add(Me.UltraCheckEditor1)
            Me.Controls.Add(Me.utDifferences)
            Me.Controls.Add(Me.TableLayoutPanel1)
            Me.Controls.Add(Me._DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Left)
            Me.Controls.Add(Me._DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Right)
            Me.Controls.Add(Me._DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Bottom)
            Me.Controls.Add(Me._DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Top)
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "DiffDefaultPropsWarningDialog"
            Me.ShowInTaskbar = False
            CType(Me.UltraCheckEditor1, System.ComponentModel.ISupportInitialize).EndInit()
            Me.TableLayoutPanel1.ResumeLayout(False)
            CType(Me.utDifferences, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.ColumnViewBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.UltraToolbarsManager1, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents Close_Button As System.Windows.Forms.Button
        Friend WithEvents UltraToolTipManager1 As Infragistics.Win.UltraWinToolTip.UltraToolTipManager
        Friend WithEvents ColumnViewBindingSource As BindingSource
        Friend WithEvents utDifferences As Infragistics.Win.UltraWinTree.UltraTree
        Friend WithEvents UltraCheckEditor1 As Infragistics.Win.UltraWinEditors.UltraCheckEditor
        Friend WithEvents UltraToolbarsManager1 As Infragistics.Win.UltraWinToolbars.UltraToolbarsManager
        Friend WithEvents _DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Left As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
        Friend WithEvents _DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Right As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
        Friend WithEvents _DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Bottom As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
        Friend WithEvents _DiffDefaultPropsWarningDialog_Toolbars_Dock_Area_Top As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    End Class

End Namespace