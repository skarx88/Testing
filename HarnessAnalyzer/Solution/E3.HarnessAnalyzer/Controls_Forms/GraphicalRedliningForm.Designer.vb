<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class GraphicalRedliningForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(GraphicalRedliningForm))
        Dim Appearance1 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance2 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance3 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance4 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance5 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim UltraToolbar1 As Infragistics.Win.UltraWinToolbars.UltraToolbar = New Infragistics.Win.UltraWinToolbars.UltraToolbar("utbMain")
        Dim ButtonTool10 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("DrawArc")
        Dim ButtonTool11 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("DrawArrow")
        Dim ButtonTool12 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("DrawCircle")
        Dim ButtonTool18 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("DrawDimension")
        Dim ButtonTool13 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("DrawLine")
        Dim ButtonTool14 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("DrawPolyline")
        Dim ButtonTool15 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("DrawRectangle")
        Dim ButtonTool16 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("DrawText")
        Dim ComboBoxTool6 As Infragistics.Win.UltraWinToolbars.ComboBoxTool = New Infragistics.Win.UltraWinToolbars.ComboBoxTool("ActiveHatch")
        Dim ComboBoxTool3 As Infragistics.Win.UltraWinToolbars.ComboBoxTool = New Infragistics.Win.UltraWinToolbars.ComboBoxTool("ActiveLineStyle")
        Dim PopupColorPickerTool2 As Infragistics.Win.UltraWinToolbars.PopupColorPickerTool = New Infragistics.Win.UltraWinToolbars.PopupColorPickerTool("ActivePenColor")
        Dim ComboBoxTool4 As Infragistics.Win.UltraWinToolbars.ComboBoxTool = New Infragistics.Win.UltraWinToolbars.ComboBoxTool("ActivePenWidth")
        Dim StateButtonTool1 As Infragistics.Win.UltraWinToolbars.StateButtonTool = New Infragistics.Win.UltraWinToolbars.StateButtonTool("ShowGrid", "")
        Dim StateButtonTool3 As Infragistics.Win.UltraWinToolbars.StateButtonTool = New Infragistics.Win.UltraWinToolbars.StateButtonTool("SnapToGrid", "")
        Dim ButtonTool26 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("ZoomExtends")
        Dim ButtonTool27 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("ZoomWindow")
        Dim ButtonTool28 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("Export")
        Dim ButtonTool29 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("Print")
        Dim ButtonTool30 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("Save")
        Dim ButtonTool3 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("DrawCircle")
        Dim ButtonTool4 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("DrawLine")
        Dim ButtonTool5 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("DrawArc")
        Dim ButtonTool6 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("DrawArrow")
        Dim ButtonTool7 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("DrawPolyline")
        Dim ButtonTool8 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("DrawRectangle")
        Dim ButtonTool9 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("DrawText")
        Dim ButtonTool17 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("DrawDimension")
        Dim PopupColorPickerTool1 As Infragistics.Win.UltraWinToolbars.PopupColorPickerTool = New Infragistics.Win.UltraWinToolbars.PopupColorPickerTool("ActivePenColor")
        Dim ComboBoxTool1 As Infragistics.Win.UltraWinToolbars.ComboBoxTool = New Infragistics.Win.UltraWinToolbars.ComboBoxTool("ActiveLineStyle")
        Dim ValueList1 As Infragistics.Win.ValueList = New Infragistics.Win.ValueList(0)
        Dim ValueListItem8 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
        Dim ValueListItem9 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
        Dim ValueListItem11 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
        Dim ValueListItem12 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
        Dim ValueListItem1 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
        Dim ComboBoxTool2 As Infragistics.Win.UltraWinToolbars.ComboBoxTool = New Infragistics.Win.UltraWinToolbars.ComboBoxTool("ActivePenWidth")
        Dim ValueList2 As Infragistics.Win.ValueList = New Infragistics.Win.ValueList(0)
        Dim ValueListItem13 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
        Dim ValueListItem14 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
        Dim ValueListItem15 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
        Dim ValueListItem16 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
        Dim ValueListItem17 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
        Dim ValueListItem18 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
        Dim ValueListItem19 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
        Dim ValueListItem20 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
        Dim ValueListItem21 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
        Dim ValueListItem22 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
        Dim ValueListItem23 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
        Dim ButtonTool19 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("ZoomExtends")
        Dim ButtonTool20 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("ZoomWindow")
        Dim ButtonTool21 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("Save")
        Dim ButtonTool22 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("Export")
        Dim ButtonTool23 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("Print")
        Dim ComboBoxTool5 As Infragistics.Win.UltraWinToolbars.ComboBoxTool = New Infragistics.Win.UltraWinToolbars.ComboBoxTool("ActiveHatch")
        Dim ValueList3 As Infragistics.Win.ValueList = New Infragistics.Win.ValueList(0)
        Dim ValueListItem10 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
        Dim ValueListItem7 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
        Dim ValueListItem3 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
        Dim ValueListItem2 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
        Dim ValueListItem4 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
        Dim ValueListItem5 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
        Dim ValueListItem6 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
        Dim StateButtonTool2 As Infragistics.Win.UltraWinToolbars.StateButtonTool = New Infragistics.Win.UltraWinToolbars.StateButtonTool("ShowGrid", "")
        Dim StateButtonTool4 As Infragistics.Win.UltraWinToolbars.StateButtonTool = New Infragistics.Win.UltraWinToolbars.StateButtonTool("SnapToGrid", "")
        Me.upnGraphicalRedlining = New Infragistics.Win.Misc.UltraPanel()
        Me.upnTextEdit = New Infragistics.Win.Misc.UltraPanel()
        Me.uckUnderline = New Infragistics.Win.UltraWinEditors.UltraCheckEditor()
        Me.uckItalic = New Infragistics.Win.UltraWinEditors.UltraCheckEditor()
        Me.uckBold = New Infragistics.Win.UltraWinEditors.UltraCheckEditor()
        Me.uceFontSize = New Infragistics.Win.UltraWinEditors.UltraComboEditor()
        Me.lblFontSize = New Infragistics.Win.Misc.UltraLabel()
        Me.txtText = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.lblText = New Infragistics.Win.Misc.UltraLabel()
        Me.lblDrawCommand = New Infragistics.Win.Misc.UltraLabel()
        Me.uckShowBackground = New Infragistics.Win.UltraWinEditors.UltraCheckEditor()
        Me.lblCoordinates = New Infragistics.Win.Misc.UltraLabel()
        Me.btnCancel = New Infragistics.Win.Misc.UltraButton()
        Me.btnOK = New Infragistics.Win.Misc.UltraButton()
        Me.vDraw = New VectorDraw.Professional.Control.VectorDrawBaseControl()
        Me.utmGraphicalRedlining = New Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(Me.components)
        Me._GraphicalRedliningForm_Toolbars_Dock_Area_Left = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        Me._GraphicalRedliningForm_Toolbars_Dock_Area_Right = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        Me._GraphicalRedliningForm_Toolbars_Dock_Area_Top = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        Me._GraphicalRedliningForm_Toolbars_Dock_Area_Bottom = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        Me.sfdExport = New System.Windows.Forms.SaveFileDialog()
        Me.upnGraphicalRedlining.ClientArea.SuspendLayout()
        Me.upnGraphicalRedlining.SuspendLayout()
        Me.upnTextEdit.ClientArea.SuspendLayout()
        Me.upnTextEdit.SuspendLayout()
        CType(Me.uckUnderline, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.uckItalic, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.uckBold, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.uceFontSize, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtText, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.uckShowBackground, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.utmGraphicalRedlining, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'upnGraphicalRedlining
        '
        resources.ApplyResources(Me.upnGraphicalRedlining, "upnGraphicalRedlining")
        '
        'upnGraphicalRedlining.ClientArea
        '
        resources.ApplyResources(Me.upnGraphicalRedlining.ClientArea, "upnGraphicalRedlining.ClientArea")
        Me.upnGraphicalRedlining.ClientArea.Controls.Add(Me.upnTextEdit)
        Me.upnGraphicalRedlining.ClientArea.Controls.Add(Me.lblDrawCommand)
        Me.upnGraphicalRedlining.ClientArea.Controls.Add(Me.uckShowBackground)
        Me.upnGraphicalRedlining.ClientArea.Controls.Add(Me.lblCoordinates)
        Me.upnGraphicalRedlining.ClientArea.Controls.Add(Me.btnCancel)
        Me.upnGraphicalRedlining.ClientArea.Controls.Add(Me.btnOK)
        Me.upnGraphicalRedlining.ClientArea.Controls.Add(Me.vDraw)
        Me.upnGraphicalRedlining.Cursor = System.Windows.Forms.Cursors.Default
        Me.upnGraphicalRedlining.Name = "upnGraphicalRedlining"
        '
        'upnTextEdit
        '
        resources.ApplyResources(Me.upnTextEdit, "upnTextEdit")
        Appearance1.BackColor = System.Drawing.Color.Transparent
        resources.ApplyResources(Appearance1, "Appearance1")
        Me.upnTextEdit.Appearance = Appearance1
        '
        'upnTextEdit.ClientArea
        '
        resources.ApplyResources(Me.upnTextEdit.ClientArea, "upnTextEdit.ClientArea")
        Me.upnTextEdit.ClientArea.Controls.Add(Me.uckUnderline)
        Me.upnTextEdit.ClientArea.Controls.Add(Me.uckItalic)
        Me.upnTextEdit.ClientArea.Controls.Add(Me.uckBold)
        Me.upnTextEdit.ClientArea.Controls.Add(Me.uceFontSize)
        Me.upnTextEdit.ClientArea.Controls.Add(Me.lblFontSize)
        Me.upnTextEdit.ClientArea.Controls.Add(Me.txtText)
        Me.upnTextEdit.ClientArea.Controls.Add(Me.lblText)
        Me.upnTextEdit.Name = "upnTextEdit"
        '
        'uckUnderline
        '
        resources.ApplyResources(Me.uckUnderline, "uckUnderline")
        Me.uckUnderline.Name = "uckUnderline"
        '
        'uckItalic
        '
        resources.ApplyResources(Me.uckItalic, "uckItalic")
        Me.uckItalic.Name = "uckItalic"
        '
        'uckBold
        '
        resources.ApplyResources(Me.uckBold, "uckBold")
        Me.uckBold.Name = "uckBold"
        '
        'uceFontSize
        '
        resources.ApplyResources(Me.uceFontSize, "uceFontSize")
        Me.uceFontSize.Name = "uceFontSize"
        '
        'lblFontSize
        '
        resources.ApplyResources(Me.lblFontSize, "lblFontSize")
        resources.ApplyResources(Appearance2, "Appearance2")
        Me.lblFontSize.Appearance = Appearance2
        Me.lblFontSize.Name = "lblFontSize"
        '
        'txtText
        '
        resources.ApplyResources(Me.txtText, "txtText")
        Me.txtText.Name = "txtText"
        '
        'lblText
        '
        resources.ApplyResources(Me.lblText, "lblText")
        resources.ApplyResources(Appearance3, "Appearance3")
        Me.lblText.Appearance = Appearance3
        Me.lblText.Name = "lblText"
        '
        'lblDrawCommand
        '
        resources.ApplyResources(Me.lblDrawCommand, "lblDrawCommand")
        Appearance4.BackColor = System.Drawing.Color.Transparent
        Appearance4.FontData.BoldAsString = resources.GetString("resource.BoldAsString")
        Appearance4.FontData.ItalicAsString = resources.GetString("resource.ItalicAsString")
        Appearance4.FontData.StrikeoutAsString = resources.GetString("resource.StrikeoutAsString")
        Appearance4.FontData.UnderlineAsString = resources.GetString("resource.UnderlineAsString")
        Appearance4.ForeColor = System.Drawing.Color.Red
        resources.ApplyResources(Appearance4, "Appearance4")
        Me.lblDrawCommand.Appearance = Appearance4
        Me.lblDrawCommand.Name = "lblDrawCommand"
        '
        'uckShowBackground
        '
        resources.ApplyResources(Me.uckShowBackground, "uckShowBackground")
        Me.uckShowBackground.Name = "uckShowBackground"
        '
        'lblCoordinates
        '
        resources.ApplyResources(Me.lblCoordinates, "lblCoordinates")
        resources.ApplyResources(Appearance5, "Appearance5")
        Me.lblCoordinates.Appearance = Appearance5
        Me.lblCoordinates.Name = "lblCoordinates"
        '
        'btnCancel
        '
        resources.ApplyResources(Me.btnCancel, "btnCancel")
        Me.btnCancel.Name = "btnCancel"
        '
        'btnOK
        '
        resources.ApplyResources(Me.btnOK, "btnOK")
        Me.btnOK.Name = "btnOK"
        '
        'vDraw
        '
        resources.ApplyResources(Me.vDraw, "vDraw")
        Me.vDraw.AccessibleRole = System.Windows.Forms.AccessibleRole.Window
        Me.vDraw.AllowDrop = True
        Me.vDraw.Cursor = System.Windows.Forms.Cursors.Default
        Me.vDraw.DisableVdrawDxf = False
        Me.vDraw.EnableAutoGripOn = True
        Me.vDraw.Name = "vDraw"
        '
        'utmGraphicalRedlining
        '
        Me.utmGraphicalRedlining.DesignerFlags = 1
        Me.utmGraphicalRedlining.DockWithinContainer = Me
        Me.utmGraphicalRedlining.DockWithinContainerBaseType = GetType(System.Windows.Forms.Form)
        Me.utmGraphicalRedlining.ShowFullMenusDelay = 500
        UltraToolbar1.DockedColumn = 0
        UltraToolbar1.DockedRow = 0
        UltraToolbar1.IsMainMenuBar = True
        resources.ApplyResources(UltraToolbar1, "UltraToolbar1")
        ComboBoxTool6.InstanceProps.IsFirstInGroup = True
        StateButtonTool1.InstanceProps.IsFirstInGroup = True
        ButtonTool26.InstanceProps.IsFirstInGroup = True
        ButtonTool28.InstanceProps.IsFirstInGroup = True
        UltraToolbar1.NonInheritedTools.AddRange(New Infragistics.Win.UltraWinToolbars.ToolBase() {ButtonTool10, ButtonTool11, ButtonTool12, ButtonTool18, ButtonTool13, ButtonTool14, ButtonTool15, ButtonTool16, ComboBoxTool6, ComboBoxTool3, PopupColorPickerTool2, ComboBoxTool4, StateButtonTool1, StateButtonTool3, ButtonTool26, ButtonTool27, ButtonTool28, ButtonTool29, ButtonTool30})
        Me.utmGraphicalRedlining.Toolbars.AddRange(New Infragistics.Win.UltraWinToolbars.UltraToolbar() {UltraToolbar1})
        resources.ApplyResources(ButtonTool3.SharedPropsInternal, "ButtonTool3.SharedPropsInternal")
        ButtonTool3.ForceApplyResources = "SharedPropsInternal"
        resources.ApplyResources(ButtonTool4.SharedPropsInternal, "ButtonTool4.SharedPropsInternal")
        ButtonTool4.ForceApplyResources = "SharedPropsInternal"
        resources.ApplyResources(ButtonTool5.SharedPropsInternal, "ButtonTool5.SharedPropsInternal")
        ButtonTool5.ForceApplyResources = "SharedPropsInternal"
        resources.ApplyResources(ButtonTool6.SharedPropsInternal, "ButtonTool6.SharedPropsInternal")
        ButtonTool6.ForceApplyResources = "SharedPropsInternal"
        resources.ApplyResources(ButtonTool7.SharedPropsInternal, "ButtonTool7.SharedPropsInternal")
        ButtonTool7.ForceApplyResources = "SharedPropsInternal"
        resources.ApplyResources(ButtonTool8.SharedPropsInternal, "ButtonTool8.SharedPropsInternal")
        ButtonTool8.ForceApplyResources = "SharedPropsInternal"
        resources.ApplyResources(ButtonTool9.SharedPropsInternal, "ButtonTool9.SharedPropsInternal")
        ButtonTool9.ForceApplyResources = "SharedPropsInternal"
        resources.ApplyResources(ButtonTool17.SharedPropsInternal, "ButtonTool17.SharedPropsInternal")
        ButtonTool17.ForceApplyResources = "SharedPropsInternal"
        resources.ApplyResources(PopupColorPickerTool1.SharedPropsInternal, "PopupColorPickerTool1.SharedPropsInternal")
        PopupColorPickerTool1.ForceApplyResources = "SharedPropsInternal"
        resources.ApplyResources(ComboBoxTool1.SharedPropsInternal, "ComboBoxTool1.SharedPropsInternal")
        ValueListItem8.DataValue = "CENTER"
        resources.ApplyResources(ValueListItem8, "ValueListItem8")
        ValueListItem8.ForceApplyResources = ""
        ValueListItem9.DataValue = "DASHDOT"
        resources.ApplyResources(ValueListItem9, "ValueListItem9")
        ValueListItem9.ForceApplyResources = ""
        ValueListItem11.DataValue = "DASHED"
        resources.ApplyResources(ValueListItem11, "ValueListItem11")
        ValueListItem11.ForceApplyResources = ""
        ValueListItem12.DataValue = "DOT"
        resources.ApplyResources(ValueListItem12, "ValueListItem12")
        ValueListItem12.ForceApplyResources = ""
        ValueListItem1.DataValue = "SOLID"
        resources.ApplyResources(ValueListItem1, "ValueListItem1")
        ValueListItem1.ForceApplyResources = ""
        ValueList1.ValueListItems.AddRange(New Infragistics.Win.ValueListItem() {ValueListItem8, ValueListItem9, ValueListItem11, ValueListItem12, ValueListItem1})
        ComboBoxTool1.ValueList = ValueList1
        ComboBoxTool1.ForceApplyResources = "SharedPropsInternal"
        resources.ApplyResources(ComboBoxTool2.SharedPropsInternal, "ComboBoxTool2.SharedPropsInternal")
        ValueListItem13.DataValue = 0.0R
        resources.ApplyResources(ValueListItem13, "ValueListItem13")
        ValueListItem13.ForceApplyResources = ""
        ValueListItem14.DataValue = 0.1R
        resources.ApplyResources(ValueListItem14, "ValueListItem14")
        ValueListItem14.ForceApplyResources = ""
        ValueListItem15.DataValue = 0.25R
        resources.ApplyResources(ValueListItem15, "ValueListItem15")
        ValueListItem15.ForceApplyResources = ""
        ValueListItem16.DataValue = 0.5R
        resources.ApplyResources(ValueListItem16, "ValueListItem16")
        ValueListItem16.ForceApplyResources = ""
        ValueListItem17.DataValue = 0.75R
        resources.ApplyResources(ValueListItem17, "ValueListItem17")
        ValueListItem17.ForceApplyResources = ""
        ValueListItem18.DataValue = 1.0R
        resources.ApplyResources(ValueListItem18, "ValueListItem18")
        ValueListItem18.ForceApplyResources = ""
        ValueListItem19.DataValue = 1.5R
        resources.ApplyResources(ValueListItem19, "ValueListItem19")
        ValueListItem19.ForceApplyResources = ""
        ValueListItem20.DataValue = 2.0R
        resources.ApplyResources(ValueListItem20, "ValueListItem20")
        ValueListItem20.ForceApplyResources = ""
        ValueListItem21.DataValue = 4.0R
        resources.ApplyResources(ValueListItem21, "ValueListItem21")
        ValueListItem21.ForceApplyResources = ""
        ValueListItem22.DataValue = 8.0R
        resources.ApplyResources(ValueListItem22, "ValueListItem22")
        ValueListItem22.ForceApplyResources = ""
        ValueListItem23.DataValue = 16.0R
        resources.ApplyResources(ValueListItem23, "ValueListItem23")
        ValueListItem23.ForceApplyResources = ""
        ValueList2.ValueListItems.AddRange(New Infragistics.Win.ValueListItem() {ValueListItem13, ValueListItem14, ValueListItem15, ValueListItem16, ValueListItem17, ValueListItem18, ValueListItem19, ValueListItem20, ValueListItem21, ValueListItem22, ValueListItem23})
        ComboBoxTool2.ValueList = ValueList2
        ComboBoxTool2.ForceApplyResources = "SharedPropsInternal"
        resources.ApplyResources(ButtonTool19.SharedPropsInternal, "ButtonTool19.SharedPropsInternal")
        ButtonTool19.ForceApplyResources = "SharedPropsInternal"
        resources.ApplyResources(ButtonTool20.SharedPropsInternal, "ButtonTool20.SharedPropsInternal")
        ButtonTool20.ForceApplyResources = "SharedPropsInternal"
        resources.ApplyResources(ButtonTool21.SharedPropsInternal, "ButtonTool21.SharedPropsInternal")
        ButtonTool21.ForceApplyResources = "SharedPropsInternal"
        resources.ApplyResources(ButtonTool22.SharedPropsInternal, "ButtonTool22.SharedPropsInternal")
        ButtonTool22.ForceApplyResources = "SharedPropsInternal"
        resources.ApplyResources(ButtonTool23.SharedPropsInternal, "ButtonTool23.SharedPropsInternal")
        ButtonTool23.ForceApplyResources = "SharedPropsInternal"
        resources.ApplyResources(ComboBoxTool5.SharedPropsInternal, "ComboBoxTool5.SharedPropsInternal")
        ValueListItem10.DataValue = "VdFillModeNone"
        resources.ApplyResources(ValueListItem10, "ValueListItem10")
        ValueListItem10.ForceApplyResources = ""
        ValueListItem7.DataValue = "VdFillModeSolid"
        resources.ApplyResources(ValueListItem7, "ValueListItem7")
        ValueListItem7.ForceApplyResources = ""
        ValueListItem3.DataValue = "VdFillModeHatchCross"
        resources.ApplyResources(ValueListItem3, "ValueListItem3")
        ValueListItem3.ForceApplyResources = ""
        ValueListItem2.DataValue = "VdFillModeHatchBDiagonal"
        resources.ApplyResources(ValueListItem2, "ValueListItem2")
        ValueListItem2.ForceApplyResources = ""
        ValueListItem4.DataValue = "VdFillModeHatchDiagCross"
        resources.ApplyResources(ValueListItem4, "ValueListItem4")
        ValueListItem4.ForceApplyResources = ""
        ValueListItem5.DataValue = "VdFillModeHatchHorizontal"
        resources.ApplyResources(ValueListItem5, "ValueListItem5")
        ValueListItem5.ForceApplyResources = ""
        ValueListItem6.DataValue = "VdFillModeHatchVertical"
        resources.ApplyResources(ValueListItem6, "ValueListItem6")
        ValueListItem6.ForceApplyResources = ""
        ValueList3.ValueListItems.AddRange(New Infragistics.Win.ValueListItem() {ValueListItem10, ValueListItem7, ValueListItem3, ValueListItem2, ValueListItem4, ValueListItem5, ValueListItem6})
        ComboBoxTool5.ValueList = ValueList3
        ComboBoxTool5.ForceApplyResources = "SharedPropsInternal"
        resources.ApplyResources(StateButtonTool2.SharedPropsInternal, "StateButtonTool2.SharedPropsInternal")
        StateButtonTool2.ForceApplyResources = "SharedPropsInternal"
        resources.ApplyResources(StateButtonTool4.SharedPropsInternal, "StateButtonTool4.SharedPropsInternal")
        StateButtonTool4.ForceApplyResources = "SharedPropsInternal"
        Me.utmGraphicalRedlining.Tools.AddRange(New Infragistics.Win.UltraWinToolbars.ToolBase() {ButtonTool3, ButtonTool4, ButtonTool5, ButtonTool6, ButtonTool7, ButtonTool8, ButtonTool9, ButtonTool17, PopupColorPickerTool1, ComboBoxTool1, ComboBoxTool2, ButtonTool19, ButtonTool20, ButtonTool21, ButtonTool22, ButtonTool23, ComboBoxTool5, StateButtonTool2, StateButtonTool4})
        '
        '_GraphicalRedliningForm_Toolbars_Dock_Area_Left
        '
        resources.ApplyResources(Me._GraphicalRedliningForm_Toolbars_Dock_Area_Left, "_GraphicalRedliningForm_Toolbars_Dock_Area_Left")
        Me._GraphicalRedliningForm_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me._GraphicalRedliningForm_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control
        Me._GraphicalRedliningForm_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left
        Me._GraphicalRedliningForm_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText
        Me._GraphicalRedliningForm_Toolbars_Dock_Area_Left.Name = "_GraphicalRedliningForm_Toolbars_Dock_Area_Left"
        Me._GraphicalRedliningForm_Toolbars_Dock_Area_Left.ToolbarsManager = Me.utmGraphicalRedlining
        '
        '_GraphicalRedliningForm_Toolbars_Dock_Area_Right
        '
        resources.ApplyResources(Me._GraphicalRedliningForm_Toolbars_Dock_Area_Right, "_GraphicalRedliningForm_Toolbars_Dock_Area_Right")
        Me._GraphicalRedliningForm_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me._GraphicalRedliningForm_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control
        Me._GraphicalRedliningForm_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right
        Me._GraphicalRedliningForm_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText
        Me._GraphicalRedliningForm_Toolbars_Dock_Area_Right.Name = "_GraphicalRedliningForm_Toolbars_Dock_Area_Right"
        Me._GraphicalRedliningForm_Toolbars_Dock_Area_Right.ToolbarsManager = Me.utmGraphicalRedlining
        '
        '_GraphicalRedliningForm_Toolbars_Dock_Area_Top
        '
        resources.ApplyResources(Me._GraphicalRedliningForm_Toolbars_Dock_Area_Top, "_GraphicalRedliningForm_Toolbars_Dock_Area_Top")
        Me._GraphicalRedliningForm_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me._GraphicalRedliningForm_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control
        Me._GraphicalRedliningForm_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top
        Me._GraphicalRedliningForm_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText
        Me._GraphicalRedliningForm_Toolbars_Dock_Area_Top.Name = "_GraphicalRedliningForm_Toolbars_Dock_Area_Top"
        Me._GraphicalRedliningForm_Toolbars_Dock_Area_Top.ToolbarsManager = Me.utmGraphicalRedlining
        '
        '_GraphicalRedliningForm_Toolbars_Dock_Area_Bottom
        '
        resources.ApplyResources(Me._GraphicalRedliningForm_Toolbars_Dock_Area_Bottom, "_GraphicalRedliningForm_Toolbars_Dock_Area_Bottom")
        Me._GraphicalRedliningForm_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me._GraphicalRedliningForm_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control
        Me._GraphicalRedliningForm_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom
        Me._GraphicalRedliningForm_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText
        Me._GraphicalRedliningForm_Toolbars_Dock_Area_Bottom.Name = "_GraphicalRedliningForm_Toolbars_Dock_Area_Bottom"
        Me._GraphicalRedliningForm_Toolbars_Dock_Area_Bottom.ToolbarsManager = Me.utmGraphicalRedlining
        '
        'sfdExport
        '
        resources.ApplyResources(Me.sfdExport, "sfdExport")
        '
        'GraphicalRedliningForm
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.Controls.Add(Me.upnGraphicalRedlining)
        Me.Controls.Add(Me._GraphicalRedliningForm_Toolbars_Dock_Area_Left)
        Me.Controls.Add(Me._GraphicalRedliningForm_Toolbars_Dock_Area_Right)
        Me.Controls.Add(Me._GraphicalRedliningForm_Toolbars_Dock_Area_Bottom)
        Me.Controls.Add(Me._GraphicalRedliningForm_Toolbars_Dock_Area_Top)
        Me.Name = "GraphicalRedliningForm"
        Me.upnGraphicalRedlining.ClientArea.ResumeLayout(False)
        Me.upnGraphicalRedlining.ResumeLayout(False)
        Me.upnTextEdit.ClientArea.ResumeLayout(False)
        Me.upnTextEdit.ClientArea.PerformLayout()
        Me.upnTextEdit.ResumeLayout(False)
        CType(Me.uckUnderline, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.uckItalic, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.uckBold, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.uceFontSize, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtText, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.uckShowBackground, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.utmGraphicalRedlining, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents vDraw As VectorDraw.Professional.Control.VectorDrawBaseControl
    Friend WithEvents btnOK As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnCancel As Infragistics.Win.Misc.UltraButton
    Friend WithEvents utmGraphicalRedlining As Infragistics.Win.UltraWinToolbars.UltraToolbarsManager
    Friend WithEvents upnGraphicalRedlining As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents _GraphicalRedliningForm_Toolbars_Dock_Area_Left As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents _GraphicalRedliningForm_Toolbars_Dock_Area_Right As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents _GraphicalRedliningForm_Toolbars_Dock_Area_Bottom As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents _GraphicalRedliningForm_Toolbars_Dock_Area_Top As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents lblCoordinates As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents uckShowBackground As Infragistics.Win.UltraWinEditors.UltraCheckEditor
    Friend WithEvents lblDrawCommand As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents sfdExport As System.Windows.Forms.SaveFileDialog
    Friend WithEvents upnTextEdit As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents uckUnderline As Infragistics.Win.UltraWinEditors.UltraCheckEditor
    Friend WithEvents uckItalic As Infragistics.Win.UltraWinEditors.UltraCheckEditor
    Friend WithEvents uckBold As Infragistics.Win.UltraWinEditors.UltraCheckEditor
    Friend WithEvents uceFontSize As Infragistics.Win.UltraWinEditors.UltraComboEditor
    Friend WithEvents lblFontSize As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents txtText As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents lblText As Infragistics.Win.Misc.UltraLabel
End Class
