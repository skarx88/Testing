<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class BOMForm
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
        components = New ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(BOMForm))
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
        btnClose = New Infragistics.Win.Misc.UltraButton()
        btnExport = New Infragistics.Win.Misc.UltraButton()
        udsBOM = New Infragistics.Win.UltraWinDataSource.UltraDataSource(components)
        ugeeBOM = New Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter(components)
        ugBOM = New Infragistics.Win.UltraWinGrid.UltraGrid()
        utmBOM = New Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(components)
        _BOMForm_Toolbars_Dock_Area_Left = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        _BOMForm_Toolbars_Dock_Area_Right = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        _BOMForm_Toolbars_Dock_Area_Top = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        _BOMForm_Toolbars_Dock_Area_Bottom = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        uttmReplacings = New Infragistics.Win.UltraWinToolTip.UltraToolTipManager(components)
        CType(udsBOM, ComponentModel.ISupportInitialize).BeginInit()
        CType(ugBOM, ComponentModel.ISupportInitialize).BeginInit()
        CType(utmBOM, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' btnClose
        ' 
        resources.ApplyResources(btnClose, "btnClose")
        btnClose.Name = "btnClose"
        ' 
        ' btnExport
        ' 
        resources.ApplyResources(btnExport, "btnExport")
        btnExport.Name = "btnExport"
        ' 
        ' udsBOM
        ' 
        udsBOM.AllowAdd = False
        udsBOM.AllowDelete = False
        udsBOM.ReadOnly = True
        ' 
        ' ugeeBOM
        ' 
        ' 
        ' ugBOM
        ' 
        resources.ApplyResources(ugBOM, "ugBOM")
        Appearance1.BackColor = SystemColors.Window
        Appearance1.BorderColor = SystemColors.InactiveCaption
        resources.ApplyResources(Appearance1.FontData, "Appearance1.FontData")
        resources.ApplyResources(Appearance1, "Appearance1")
        Appearance1.ForceApplyResources = "FontData|"
        ugBOM.DisplayLayout.Appearance = Appearance1
        ugBOM.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        ugBOM.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False
        Appearance2.BackColor = SystemColors.ActiveBorder
        Appearance2.BackColor2 = SystemColors.ControlDark
        Appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical
        Appearance2.BorderColor = SystemColors.Window
        resources.ApplyResources(Appearance2.FontData, "Appearance2.FontData")
        resources.ApplyResources(Appearance2, "Appearance2")
        Appearance2.ForceApplyResources = "FontData|"
        ugBOM.DisplayLayout.GroupByBox.Appearance = Appearance2
        Appearance3.ForeColor = SystemColors.GrayText
        resources.ApplyResources(Appearance3.FontData, "Appearance3.FontData")
        resources.ApplyResources(Appearance3, "Appearance3")
        Appearance3.ForceApplyResources = "FontData|"
        ugBOM.DisplayLayout.GroupByBox.BandLabelAppearance = Appearance3
        ugBOM.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        Appearance4.BackColor = SystemColors.ControlLightLight
        Appearance4.BackColor2 = SystemColors.Control
        Appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance4.ForeColor = SystemColors.GrayText
        resources.ApplyResources(Appearance4.FontData, "Appearance4.FontData")
        resources.ApplyResources(Appearance4, "Appearance4")
        Appearance4.ForceApplyResources = "FontData|"
        ugBOM.DisplayLayout.GroupByBox.PromptAppearance = Appearance4
        ugBOM.DisplayLayout.MaxColScrollRegions = 1
        ugBOM.DisplayLayout.MaxRowScrollRegions = 1
        Appearance5.BackColor = SystemColors.Window
        Appearance5.ForeColor = SystemColors.ControlText
        resources.ApplyResources(Appearance5.FontData, "Appearance5.FontData")
        resources.ApplyResources(Appearance5, "Appearance5")
        Appearance5.ForceApplyResources = "FontData|"
        ugBOM.DisplayLayout.Override.ActiveCellAppearance = Appearance5
        Appearance6.BackColor = SystemColors.Highlight
        Appearance6.ForeColor = SystemColors.HighlightText
        resources.ApplyResources(Appearance6.FontData, "Appearance6.FontData")
        resources.ApplyResources(Appearance6, "Appearance6")
        Appearance6.ForceApplyResources = "FontData|"
        ugBOM.DisplayLayout.Override.ActiveRowAppearance = Appearance6
        ugBOM.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted
        ugBOM.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted
        Appearance7.BackColor = SystemColors.Window
        resources.ApplyResources(Appearance7.FontData, "Appearance7.FontData")
        resources.ApplyResources(Appearance7, "Appearance7")
        Appearance7.ForceApplyResources = "FontData|"
        ugBOM.DisplayLayout.Override.CardAreaAppearance = Appearance7
        Appearance8.BorderColor = Color.Silver
        Appearance8.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter
        resources.ApplyResources(Appearance8.FontData, "Appearance8.FontData")
        resources.ApplyResources(Appearance8, "Appearance8")
        Appearance8.ForceApplyResources = "FontData|"
        ugBOM.DisplayLayout.Override.CellAppearance = Appearance8
        ugBOM.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText
        ugBOM.DisplayLayout.Override.CellPadding = 0
        Appearance9.BackColor = SystemColors.Control
        Appearance9.BackColor2 = SystemColors.ControlDark
        Appearance9.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element
        Appearance9.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance9.BorderColor = SystemColors.Window
        resources.ApplyResources(Appearance9.FontData, "Appearance9.FontData")
        resources.ApplyResources(Appearance9, "Appearance9")
        Appearance9.ForceApplyResources = "FontData|"
        ugBOM.DisplayLayout.Override.GroupByRowAppearance = Appearance9
        resources.ApplyResources(Appearance10, "Appearance10")
        resources.ApplyResources(Appearance10.FontData, "Appearance10.FontData")
        Appearance10.ForceApplyResources = "FontData|"
        ugBOM.DisplayLayout.Override.HeaderAppearance = Appearance10
        ugBOM.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti
        ugBOM.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand
        Appearance11.BackColor = SystemColors.Window
        Appearance11.BorderColor = Color.Silver
        resources.ApplyResources(Appearance11.FontData, "Appearance11.FontData")
        resources.ApplyResources(Appearance11, "Appearance11")
        Appearance11.ForceApplyResources = "FontData|"
        ugBOM.DisplayLayout.Override.RowAppearance = Appearance11
        ugBOM.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False
        Appearance12.BackColor = SystemColors.ControlLight
        resources.ApplyResources(Appearance12.FontData, "Appearance12.FontData")
        resources.ApplyResources(Appearance12, "Appearance12")
        Appearance12.ForceApplyResources = "FontData|"
        ugBOM.DisplayLayout.Override.TemplateAddRowAppearance = Appearance12
        ugBOM.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill
        ugBOM.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate
        ugBOM.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy
        ugBOM.Name = "ugBOM"
        ' 
        ' utmBOM
        ' 
        utmBOM.DesignerFlags = 1
        utmBOM.DockWithinContainer = Me
        utmBOM.DockWithinContainerBaseType = GetType(Form)
        ' 
        ' _BOMForm_Toolbars_Dock_Area_Left
        ' 
        _BOMForm_Toolbars_Dock_Area_Left.AccessibleRole = AccessibleRole.Grouping
        _BOMForm_Toolbars_Dock_Area_Left.BackColor = SystemColors.Control
        _BOMForm_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left
        _BOMForm_Toolbars_Dock_Area_Left.ForeColor = SystemColors.ControlText
        resources.ApplyResources(_BOMForm_Toolbars_Dock_Area_Left, "_BOMForm_Toolbars_Dock_Area_Left")
        _BOMForm_Toolbars_Dock_Area_Left.Name = "_BOMForm_Toolbars_Dock_Area_Left"
        _BOMForm_Toolbars_Dock_Area_Left.ToolbarsManager = utmBOM
        ' 
        ' _BOMForm_Toolbars_Dock_Area_Right
        ' 
        _BOMForm_Toolbars_Dock_Area_Right.AccessibleRole = AccessibleRole.Grouping
        _BOMForm_Toolbars_Dock_Area_Right.BackColor = SystemColors.Control
        _BOMForm_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right
        _BOMForm_Toolbars_Dock_Area_Right.ForeColor = SystemColors.ControlText
        resources.ApplyResources(_BOMForm_Toolbars_Dock_Area_Right, "_BOMForm_Toolbars_Dock_Area_Right")
        _BOMForm_Toolbars_Dock_Area_Right.Name = "_BOMForm_Toolbars_Dock_Area_Right"
        _BOMForm_Toolbars_Dock_Area_Right.ToolbarsManager = utmBOM
        ' 
        ' _BOMForm_Toolbars_Dock_Area_Top
        ' 
        _BOMForm_Toolbars_Dock_Area_Top.AccessibleRole = AccessibleRole.Grouping
        _BOMForm_Toolbars_Dock_Area_Top.BackColor = SystemColors.Control
        _BOMForm_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top
        _BOMForm_Toolbars_Dock_Area_Top.ForeColor = SystemColors.ControlText
        resources.ApplyResources(_BOMForm_Toolbars_Dock_Area_Top, "_BOMForm_Toolbars_Dock_Area_Top")
        _BOMForm_Toolbars_Dock_Area_Top.Name = "_BOMForm_Toolbars_Dock_Area_Top"
        _BOMForm_Toolbars_Dock_Area_Top.ToolbarsManager = utmBOM
        ' 
        ' _BOMForm_Toolbars_Dock_Area_Bottom
        ' 
        _BOMForm_Toolbars_Dock_Area_Bottom.AccessibleRole = AccessibleRole.Grouping
        _BOMForm_Toolbars_Dock_Area_Bottom.BackColor = SystemColors.Control
        _BOMForm_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom
        _BOMForm_Toolbars_Dock_Area_Bottom.ForeColor = SystemColors.ControlText
        resources.ApplyResources(_BOMForm_Toolbars_Dock_Area_Bottom, "_BOMForm_Toolbars_Dock_Area_Bottom")
        _BOMForm_Toolbars_Dock_Area_Bottom.Name = "_BOMForm_Toolbars_Dock_Area_Bottom"
        _BOMForm_Toolbars_Dock_Area_Bottom.ToolbarsManager = utmBOM
        ' 
        ' uttmReplacings
        ' 
        uttmReplacings.ContainingControl = Me
        ' 
        ' BOMForm
        ' 
        AutoScaleMode = AutoScaleMode.Inherit
        resources.ApplyResources(Me, "$this")
        Controls.Add(ugBOM)
        Controls.Add(btnExport)
        Controls.Add(btnClose)
        Controls.Add(_BOMForm_Toolbars_Dock_Area_Left)
        Controls.Add(_BOMForm_Toolbars_Dock_Area_Right)
        Controls.Add(_BOMForm_Toolbars_Dock_Area_Bottom)
        Controls.Add(_BOMForm_Toolbars_Dock_Area_Top)
        KeyPreview = True
        Name = "BOMForm"
        CType(udsBOM, ComponentModel.ISupportInitialize).EndInit()
        CType(ugBOM, ComponentModel.ISupportInitialize).EndInit()
        CType(utmBOM, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)

    End Sub
    Friend WithEvents btnClose As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnExport As Infragistics.Win.Misc.UltraButton
    Friend WithEvents udsBOM As Infragistics.Win.UltraWinDataSource.UltraDataSource
    Friend WithEvents ugeeBOM As Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter
    Friend WithEvents ugBOM As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents utmBOM As Infragistics.Win.UltraWinToolbars.UltraToolbarsManager
    Friend WithEvents _BOMForm_Toolbars_Dock_Area_Left As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents _BOMForm_Toolbars_Dock_Area_Right As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents _BOMForm_Toolbars_Dock_Area_Bottom As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents _BOMForm_Toolbars_Dock_Area_Top As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents uttmReplacings As Infragistics.Win.UltraWinToolTip.UltraToolTipManager
End Class
