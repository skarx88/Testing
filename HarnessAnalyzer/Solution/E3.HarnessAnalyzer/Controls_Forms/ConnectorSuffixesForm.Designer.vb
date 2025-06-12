<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ConnectorSuffixesForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ConnectorSuffixesForm))
        Dim PopupMenuTool1 As Infragistics.Win.UltraWinToolbars.PopupMenuTool = New Infragistics.Win.UltraWinToolbars.PopupMenuTool("ContextMenu")
        Dim ButtonTool1 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("RemoveItemContextMenuItem")
        Dim RemoveButtonTool As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("RemoveItemContextMenuItem")
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.OK_Button = New System.Windows.Forms.Button()
        Me.Cancel_Button = New System.Windows.Forms.Button()
        Me.ulvSuffixes = New Infragistics.Win.UltraWinListView.UltraListView()
        Me.btnAddNew = New Infragistics.Win.Misc.UltraButton()
        Me.btnRemove = New Infragistics.Win.Misc.UltraButton()
        Me.UltraToolbarsManager1 = New Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(Me.components)
        Me._frmConnectorSuffixes_Toolbars_Dock_Area_Left = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        Me._frmConnectorSuffixes_Toolbars_Dock_Area_Right = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        Me._frmConnectorSuffixes_Toolbars_Dock_Area_Top = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        Me._frmConnectorSuffixes_Toolbars_Dock_Area_Bottom = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        Me.TableLayoutPanel1.SuspendLayout()
        CType(Me.ulvSuffixes, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.UltraToolbarsManager1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
        Me.TableLayoutPanel1.Controls.Add(Me.OK_Button, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Cancel_Button, 1, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        '
        'OK_Button
        '
        resources.ApplyResources(Me.OK_Button, "OK_Button")
        Me.OK_Button.Name = "OK_Button"
        '
        'Cancel_Button
        '
        resources.ApplyResources(Me.Cancel_Button, "Cancel_Button")
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Name = "Cancel_Button"
        '
        'ulvSuffixes
        '
        resources.ApplyResources(Me.ulvSuffixes, "ulvSuffixes")
        Me.UltraToolbarsManager1.SetContextMenuUltra(Me.ulvSuffixes, "ContextMenu")
        Me.ulvSuffixes.ItemSettings.HideSelection = False
        Me.ulvSuffixes.Name = "ulvSuffixes"
        Me.ulvSuffixes.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.Details
        Me.ulvSuffixes.ViewSettingsDetails.AutoFitColumns = Infragistics.Win.UltraWinListView.AutoFitColumns.None
        Me.ulvSuffixes.ViewSettingsDetails.ColumnHeadersVisible = False
        Me.ulvSuffixes.ViewSettingsDetails.ColumnsShowSortIndicators = False
        Me.ulvSuffixes.ViewSettingsDetails.FullRowSelect = True
        Me.ulvSuffixes.ViewSettingsDetails.ImageSize = New System.Drawing.Size(0, 0)
        '
        'btnAddNew
        '
        resources.ApplyResources(Me.btnAddNew, "btnAddNew")
        Me.btnAddNew.Name = "btnAddNew"
        '
        'btnRemove
        '
        resources.ApplyResources(Me.btnRemove, "btnRemove")
        Me.btnRemove.Name = "btnRemove"
        '
        'UltraToolbarsManager1
        '
        Me.UltraToolbarsManager1.DesignerFlags = 1
        Me.UltraToolbarsManager1.DockWithinContainer = Me
        Me.UltraToolbarsManager1.DockWithinContainerBaseType = GetType(System.Windows.Forms.Form)
        Me.UltraToolbarsManager1.ShowFullMenusDelay = 500
        resources.ApplyResources(PopupMenuTool1.SharedPropsInternal, "PopupMenuTool1.SharedPropsInternal")
        PopupMenuTool1.Tools.AddRange(New Infragistics.Win.UltraWinToolbars.ToolBase() {ButtonTool1})
        PopupMenuTool1.ForceApplyResources = "SharedPropsInternal"
        resources.ApplyResources(RemoveButtonTool.SharedPropsInternal, "RemoveButtonTool.SharedPropsInternal")
        RemoveButtonTool.SharedPropsInternal.Shortcut = System.Windows.Forms.Shortcut.Del
        RemoveButtonTool.ForceApplyResources = "SharedPropsInternal"
        Me.UltraToolbarsManager1.Tools.AddRange(New Infragistics.Win.UltraWinToolbars.ToolBase() {PopupMenuTool1, RemoveButtonTool})
        '
        '_frmConnectorSuffixes_Toolbars_Dock_Area_Left
        '
        Me._frmConnectorSuffixes_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me._frmConnectorSuffixes_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control
        Me._frmConnectorSuffixes_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left
        Me._frmConnectorSuffixes_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText
        resources.ApplyResources(Me._frmConnectorSuffixes_Toolbars_Dock_Area_Left, "_frmConnectorSuffixes_Toolbars_Dock_Area_Left")
        Me._frmConnectorSuffixes_Toolbars_Dock_Area_Left.Name = "_frmConnectorSuffixes_Toolbars_Dock_Area_Left"
        Me._frmConnectorSuffixes_Toolbars_Dock_Area_Left.ToolbarsManager = Me.UltraToolbarsManager1
        '
        '_frmConnectorSuffixes_Toolbars_Dock_Area_Right
        '
        Me._frmConnectorSuffixes_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me._frmConnectorSuffixes_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control
        Me._frmConnectorSuffixes_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right
        Me._frmConnectorSuffixes_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText
        resources.ApplyResources(Me._frmConnectorSuffixes_Toolbars_Dock_Area_Right, "_frmConnectorSuffixes_Toolbars_Dock_Area_Right")
        Me._frmConnectorSuffixes_Toolbars_Dock_Area_Right.Name = "_frmConnectorSuffixes_Toolbars_Dock_Area_Right"
        Me._frmConnectorSuffixes_Toolbars_Dock_Area_Right.ToolbarsManager = Me.UltraToolbarsManager1
        '
        '_frmConnectorSuffixes_Toolbars_Dock_Area_Top
        '
        Me._frmConnectorSuffixes_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me._frmConnectorSuffixes_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control
        Me._frmConnectorSuffixes_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top
        Me._frmConnectorSuffixes_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText
        resources.ApplyResources(Me._frmConnectorSuffixes_Toolbars_Dock_Area_Top, "_frmConnectorSuffixes_Toolbars_Dock_Area_Top")
        Me._frmConnectorSuffixes_Toolbars_Dock_Area_Top.Name = "_frmConnectorSuffixes_Toolbars_Dock_Area_Top"
        Me._frmConnectorSuffixes_Toolbars_Dock_Area_Top.ToolbarsManager = Me.UltraToolbarsManager1
        '
        '_frmConnectorSuffixes_Toolbars_Dock_Area_Bottom
        '
        Me._frmConnectorSuffixes_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me._frmConnectorSuffixes_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control
        Me._frmConnectorSuffixes_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom
        Me._frmConnectorSuffixes_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText
        resources.ApplyResources(Me._frmConnectorSuffixes_Toolbars_Dock_Area_Bottom, "_frmConnectorSuffixes_Toolbars_Dock_Area_Bottom")
        Me._frmConnectorSuffixes_Toolbars_Dock_Area_Bottom.Name = "_frmConnectorSuffixes_Toolbars_Dock_Area_Bottom"
        Me._frmConnectorSuffixes_Toolbars_Dock_Area_Bottom.ToolbarsManager = Me.UltraToolbarsManager1
        '
        'frmConnectorSuffixes
        '
        Me.AcceptButton = Me.OK_Button
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.Cancel_Button
        Me.Controls.Add(Me.btnRemove)
        Me.Controls.Add(Me.btnAddNew)
        Me.Controls.Add(Me.ulvSuffixes)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Controls.Add(Me._frmConnectorSuffixes_Toolbars_Dock_Area_Left)
        Me.Controls.Add(Me._frmConnectorSuffixes_Toolbars_Dock_Area_Right)
        Me.Controls.Add(Me._frmConnectorSuffixes_Toolbars_Dock_Area_Bottom)
        Me.Controls.Add(Me._frmConnectorSuffixes_Toolbars_Dock_Area_Top)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmConnectorSuffixes"
        Me.ShowInTaskbar = False
        Me.TableLayoutPanel1.ResumeLayout(False)
        CType(Me.ulvSuffixes, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.UltraToolbarsManager1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents OK_Button As System.Windows.Forms.Button
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents ulvSuffixes As Infragistics.Win.UltraWinListView.UltraListView
    Friend WithEvents btnAddNew As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnRemove As Infragistics.Win.Misc.UltraButton
    Friend WithEvents UltraToolbarsManager1 As Infragistics.Win.UltraWinToolbars.UltraToolbarsManager
    Friend WithEvents _frmConnectorSuffixes_Toolbars_Dock_Area_Left As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents _frmConnectorSuffixes_Toolbars_Dock_Area_Right As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents _frmConnectorSuffixes_Toolbars_Dock_Area_Bottom As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents _frmConnectorSuffixes_Toolbars_Dock_Area_Top As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea

End Class
