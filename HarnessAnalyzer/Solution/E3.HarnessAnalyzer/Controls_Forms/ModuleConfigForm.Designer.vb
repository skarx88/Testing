<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ModuleConfigForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ModuleConfigForm))
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
        Me.btnOK = New Infragistics.Win.Misc.UltraButton()
        Me.btnCancel = New Infragistics.Win.Misc.UltraButton()
        Me.btnImport = New Infragistics.Win.Misc.UltraButton()
        Me.uddbEditActiveConfig = New Infragistics.Win.Misc.UltraDropDownButton()
        Me.uceActiveConfig = New Infragistics.Win.UltraWinEditors.UltraComboEditor()
        Me.lblActiveConfig = New Infragistics.Win.Misc.UltraLabel()
        Me.ugModuleConfig = New Infragistics.Win.UltraWinGrid.UltraGrid()
        Me.udsModuleConfig = New Infragistics.Win.UltraWinDataSource.UltraDataSource(Me.components)
        Me.utmModuleConfig = New Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(Me.components)
        Me._ModuleConfigForm_Toolbars_Dock_Area_Left = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        Me._ModuleConfigForm_Toolbars_Dock_Area_Right = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        Me._ModuleConfigForm_Toolbars_Dock_Area_Top = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        Me._ModuleConfigForm_Toolbars_Dock_Area_Bottom = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        Me.uddbExport = New Infragistics.Win.Misc.UltraDropDownButton()
        CType(Me.uceActiveConfig, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ugModuleConfig, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.udsModuleConfig, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.utmModuleConfig, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnOK
        '
        resources.ApplyResources(Me.btnOK, "btnOK")
        Me.btnOK.Name = "btnOK"
        '
        'btnCancel
        '
        resources.ApplyResources(Me.btnCancel, "btnCancel")
        Me.btnCancel.Name = "btnCancel"
        '
        'btnImport
        '
        resources.ApplyResources(Me.btnImport, "btnImport")
        Me.btnImport.Name = "btnImport"
        '
        'uddbEditActiveConfig
        '
        resources.ApplyResources(Me.uddbEditActiveConfig, "uddbEditActiveConfig")
        Me.uddbEditActiveConfig.Name = "uddbEditActiveConfig"
        '
        'uceActiveConfig
        '
        resources.ApplyResources(Me.uceActiveConfig, "uceActiveConfig")
        Me.uceActiveConfig.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList
        Me.uceActiveConfig.Name = "uceActiveConfig"
        '
        'lblActiveConfig
        '
        resources.ApplyResources(Me.lblActiveConfig, "lblActiveConfig")
        resources.ApplyResources(Appearance1, "Appearance1")
        Me.lblActiveConfig.Appearance = Appearance1
        Me.lblActiveConfig.Name = "lblActiveConfig"
        '
        'ugModuleConfig
        '
        resources.ApplyResources(Me.ugModuleConfig, "ugModuleConfig")
        Appearance2.BackColor = System.Drawing.SystemColors.Window
        Appearance2.BorderColor = System.Drawing.SystemColors.InactiveCaption
        resources.ApplyResources(Appearance2, "Appearance2")
        Me.ugModuleConfig.DisplayLayout.Appearance = Appearance2
        Me.ugModuleConfig.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        Me.ugModuleConfig.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.[False]
        Appearance3.BackColor = System.Drawing.SystemColors.ActiveBorder
        Appearance3.BackColor2 = System.Drawing.SystemColors.ControlDark
        Appearance3.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical
        Appearance3.BorderColor = System.Drawing.SystemColors.Window
        resources.ApplyResources(Appearance3, "Appearance3")
        Me.ugModuleConfig.DisplayLayout.GroupByBox.Appearance = Appearance3
        Appearance4.ForeColor = System.Drawing.SystemColors.GrayText
        resources.ApplyResources(Appearance4, "Appearance4")
        Me.ugModuleConfig.DisplayLayout.GroupByBox.BandLabelAppearance = Appearance4
        Me.ugModuleConfig.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        Appearance5.BackColor = System.Drawing.SystemColors.ControlLightLight
        Appearance5.BackColor2 = System.Drawing.SystemColors.Control
        Appearance5.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance5.ForeColor = System.Drawing.SystemColors.GrayText
        resources.ApplyResources(Appearance5, "Appearance5")
        Me.ugModuleConfig.DisplayLayout.GroupByBox.PromptAppearance = Appearance5
        Me.ugModuleConfig.DisplayLayout.MaxColScrollRegions = 1
        Me.ugModuleConfig.DisplayLayout.MaxRowScrollRegions = 1
        Appearance6.BackColor = System.Drawing.SystemColors.Window
        Appearance6.ForeColor = System.Drawing.SystemColors.ControlText
        resources.ApplyResources(Appearance6, "Appearance6")
        Me.ugModuleConfig.DisplayLayout.Override.ActiveCellAppearance = Appearance6
        Appearance7.BackColor = System.Drawing.SystemColors.Highlight
        Appearance7.ForeColor = System.Drawing.SystemColors.HighlightText
        resources.ApplyResources(Appearance7, "Appearance7")
        Me.ugModuleConfig.DisplayLayout.Override.ActiveRowAppearance = Appearance7
        Me.ugModuleConfig.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted
        Me.ugModuleConfig.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted
        Appearance8.BackColor = System.Drawing.SystemColors.Window
        resources.ApplyResources(Appearance8, "Appearance8")
        Me.ugModuleConfig.DisplayLayout.Override.CardAreaAppearance = Appearance8
        Appearance9.BorderColor = System.Drawing.Color.Silver
        resources.ApplyResources(Appearance9, "Appearance9")
        Appearance9.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter
        Me.ugModuleConfig.DisplayLayout.Override.CellAppearance = Appearance9
        Me.ugModuleConfig.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText
        Me.ugModuleConfig.DisplayLayout.Override.CellPadding = 0
        Appearance10.BackColor = System.Drawing.SystemColors.Control
        Appearance10.BackColor2 = System.Drawing.SystemColors.ControlDark
        Appearance10.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element
        Appearance10.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance10.BorderColor = System.Drawing.SystemColors.Window
        resources.ApplyResources(Appearance10, "Appearance10")
        Me.ugModuleConfig.DisplayLayout.Override.GroupByRowAppearance = Appearance10
        resources.ApplyResources(Appearance11, "Appearance11")
        Me.ugModuleConfig.DisplayLayout.Override.HeaderAppearance = Appearance11
        Me.ugModuleConfig.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti
        Me.ugModuleConfig.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand
        Appearance12.BackColor = System.Drawing.SystemColors.Window
        Appearance12.BorderColor = System.Drawing.Color.Silver
        resources.ApplyResources(Appearance12, "Appearance12")
        Me.ugModuleConfig.DisplayLayout.Override.RowAppearance = Appearance12
        Me.ugModuleConfig.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.[False]
        Appearance13.BackColor = System.Drawing.SystemColors.ControlLight
        resources.ApplyResources(Appearance13, "Appearance13")
        Me.ugModuleConfig.DisplayLayout.Override.TemplateAddRowAppearance = Appearance13
        Me.ugModuleConfig.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill
        Me.ugModuleConfig.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate
        Me.ugModuleConfig.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy
        Me.ugModuleConfig.Name = "ugModuleConfig"
        '
        'udsModuleConfig
        '
        Me.udsModuleConfig.AllowAdd = False
        Me.udsModuleConfig.AllowDelete = False
        '
        'utmModuleConfig
        '
        Me.utmModuleConfig.DesignerFlags = 1
        Me.utmModuleConfig.DockWithinContainer = Me
        Me.utmModuleConfig.DockWithinContainerBaseType = GetType(System.Windows.Forms.Form)
        '
        '_ModuleConfigForm_Toolbars_Dock_Area_Left
        '
        resources.ApplyResources(Me._ModuleConfigForm_Toolbars_Dock_Area_Left, "_ModuleConfigForm_Toolbars_Dock_Area_Left")
        Me._ModuleConfigForm_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me._ModuleConfigForm_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control
        Me._ModuleConfigForm_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left
        Me._ModuleConfigForm_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText
        Me._ModuleConfigForm_Toolbars_Dock_Area_Left.Name = "_ModuleConfigForm_Toolbars_Dock_Area_Left"
        Me._ModuleConfigForm_Toolbars_Dock_Area_Left.ToolbarsManager = Me.utmModuleConfig
        '
        '_ModuleConfigForm_Toolbars_Dock_Area_Right
        '
        resources.ApplyResources(Me._ModuleConfigForm_Toolbars_Dock_Area_Right, "_ModuleConfigForm_Toolbars_Dock_Area_Right")
        Me._ModuleConfigForm_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me._ModuleConfigForm_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control
        Me._ModuleConfigForm_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right
        Me._ModuleConfigForm_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText
        Me._ModuleConfigForm_Toolbars_Dock_Area_Right.Name = "_ModuleConfigForm_Toolbars_Dock_Area_Right"
        Me._ModuleConfigForm_Toolbars_Dock_Area_Right.ToolbarsManager = Me.utmModuleConfig
        '
        '_ModuleConfigForm_Toolbars_Dock_Area_Top
        '
        resources.ApplyResources(Me._ModuleConfigForm_Toolbars_Dock_Area_Top, "_ModuleConfigForm_Toolbars_Dock_Area_Top")
        Me._ModuleConfigForm_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me._ModuleConfigForm_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control
        Me._ModuleConfigForm_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top
        Me._ModuleConfigForm_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText
        Me._ModuleConfigForm_Toolbars_Dock_Area_Top.Name = "_ModuleConfigForm_Toolbars_Dock_Area_Top"
        Me._ModuleConfigForm_Toolbars_Dock_Area_Top.ToolbarsManager = Me.utmModuleConfig
        '
        '_ModuleConfigForm_Toolbars_Dock_Area_Bottom
        '
        resources.ApplyResources(Me._ModuleConfigForm_Toolbars_Dock_Area_Bottom, "_ModuleConfigForm_Toolbars_Dock_Area_Bottom")
        Me._ModuleConfigForm_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me._ModuleConfigForm_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control
        Me._ModuleConfigForm_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom
        Me._ModuleConfigForm_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText
        Me._ModuleConfigForm_Toolbars_Dock_Area_Bottom.Name = "_ModuleConfigForm_Toolbars_Dock_Area_Bottom"
        Me._ModuleConfigForm_Toolbars_Dock_Area_Bottom.ToolbarsManager = Me.utmModuleConfig
        '
        'uddbExport
        '
        resources.ApplyResources(Me.uddbExport, "uddbExport")
        Me.uddbExport.Name = "uddbExport"
        '
        'ModuleConfigForm
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.Controls.Add(Me.uddbExport)
        Me.Controls.Add(Me.ugModuleConfig)
        Me.Controls.Add(Me.lblActiveConfig)
        Me.Controls.Add(Me.uceActiveConfig)
        Me.Controls.Add(Me.uddbEditActiveConfig)
        Me.Controls.Add(Me.btnImport)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me._ModuleConfigForm_Toolbars_Dock_Area_Left)
        Me.Controls.Add(Me._ModuleConfigForm_Toolbars_Dock_Area_Right)
        Me.Controls.Add(Me._ModuleConfigForm_Toolbars_Dock_Area_Bottom)
        Me.Controls.Add(Me._ModuleConfigForm_Toolbars_Dock_Area_Top)
        Me.KeyPreview = True
        Me.MinimizeBox = False
        Me.Name = "ModuleConfigForm"
        Me.ShowInTaskbar = False
        CType(Me.uceActiveConfig, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ugModuleConfig, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.udsModuleConfig, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.utmModuleConfig, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents btnOK As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnCancel As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnImport As Infragistics.Win.Misc.UltraButton
    Friend WithEvents uddbEditActiveConfig As Infragistics.Win.Misc.UltraDropDownButton
    Friend WithEvents uceActiveConfig As Infragistics.Win.UltraWinEditors.UltraComboEditor
    Friend WithEvents lblActiveConfig As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents ugModuleConfig As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents udsModuleConfig As Infragistics.Win.UltraWinDataSource.UltraDataSource
    Friend WithEvents utmModuleConfig As Infragistics.Win.UltraWinToolbars.UltraToolbarsManager
    Friend WithEvents _ModuleConfigForm_Toolbars_Dock_Area_Left As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents _ModuleConfigForm_Toolbars_Dock_Area_Right As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents _ModuleConfigForm_Toolbars_Dock_Area_Bottom As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents _ModuleConfigForm_Toolbars_Dock_Area_Top As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents uddbExport As Infragistics.Win.Misc.UltraDropDownButton
End Class
