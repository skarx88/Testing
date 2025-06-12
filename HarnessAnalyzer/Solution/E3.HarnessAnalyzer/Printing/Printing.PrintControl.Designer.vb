Namespace Printing

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class PrintControl
        Inherits System.Windows.Forms.UserControl

        'Form overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing Then
                    If components IsNot Nothing Then
                        components.Dispose()
                    End If
                End If
            Finally
                Application.RemoveMessageFilter(Me)
                MyBase.Dispose(disposing)
            End Try
        End Sub

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()>
        Friend Sub InitializeComponent()
            components = New ComponentModel.Container()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(PrintControl))
            Dim ValueListItem1 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
            Dim ValueListItem2 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
            Dim ValueListItem9 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
            Dim ValueListItem10 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
            Dim Appearance1 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim ValueListItem5 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
            Dim ValueListItem6 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
            Dim Appearance2 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim ValueListItem16 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
            Dim ValueListItem17 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
            Dim ValueListItem18 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
            Dim ValueListItem19 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
            Dim ValueListItem20 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
            Dim ValueListItem21 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
            Dim ValueListItem22 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
            Dim ButtonTool1 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("Reset")
            Dim PopupMenuTool1 As Infragistics.Win.UltraWinToolbars.PopupMenuTool = New Infragistics.Win.UltraWinToolbars.PopupMenuTool("PreviewContextMenu")
            Dim ButtonTool3 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("Update")
            Dim ButtonTool2 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("Reset")
            Dim ButtonTool4 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("Update")
            Dim Appearance3 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim Appearance4 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim Appearance5 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            upbPreview = New Infragistics.Win.Misc.UltraGroupBox()
            lblLoadingPreview = New Label()
            uckOnlyMargins = New Infragistics.Win.UltraWinEditors.UltraCheckEditor()
            ctrlPrintPreview = New Infragistics.Win.Printing.UltraPrintPreviewControl()
            btnClose = New Infragistics.Win.Misc.UltraButton()
            btnPrint = New Infragistics.Win.Misc.UltraButton()
            uceInstalledPrinters = New Infragistics.Win.UltraWinEditors.UltraComboEditor()
            lblSelectedPrinter = New Infragistics.Win.Misc.UltraLabel()
            btnPrinterSettings = New Infragistics.Win.Misc.UltraButton()
            ugbMargins = New Infragistics.Win.Misc.UltraGroupBox()
            uneRightMargin = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
            uneLeftMargin = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
            uneBottomMargin = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
            uneTopMargin = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
            lblLeft = New Infragistics.Win.Misc.UltraLabel()
            lblRight = New Infragistics.Win.Misc.UltraLabel()
            lblBottom = New Infragistics.Win.Misc.UltraLabel()
            lblTop = New Infragistics.Win.Misc.UltraLabel()
            ugbOrientation = New Infragistics.Win.Misc.UltraGroupBox()
            uosOrientation = New Infragistics.Win.UltraWinEditors.UltraOptionSet()
            ugbPaper = New Infragistics.Win.Misc.UltraGroupBox()
            TableLayoutPanel2 = New TableLayoutPanel()
            lblHeight = New Infragistics.Win.Misc.UltraLabel()
            uneWidth = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
            lblWidth = New Infragistics.Win.Misc.UltraLabel()
            uneHeight = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
            uckCustomPaperSize = New Infragistics.Win.UltraWinEditors.UltraCheckEditor()
            ucePaperSizes = New Infragistics.Win.UltraWinEditors.UltraComboEditor()
            lblSelectedPaperSize = New Infragistics.Win.Misc.UltraLabel()
            ugbScale = New Infragistics.Win.Misc.UltraGroupBox()
            uckCenterToPaper = New Infragistics.Win.UltraWinEditors.UltraCheckEditor()
            uckScaleToFit = New Infragistics.Win.UltraWinEditors.UltraCheckEditor()
            uneDrawingUnits = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
            lblDrawingUnits = New Infragistics.Win.Misc.UltraLabel()
            unePrinterUnits = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
            lblPrinterUnits = New Infragistics.Win.Misc.UltraLabel()
            nudNumberOfCopies = New NumericUpDown()
            lblNumberOfCopies = New Infragistics.Win.Misc.UltraLabel()
            ugbPrintArea = New Infragistics.Win.Misc.UltraGroupBox()
            uosPrintArea = New Infragistics.Win.UltraWinEditors.UltraOptionSet()
            btnPick = New Infragistics.Win.Misc.UltraButton()
            uosUnits = New Infragistics.Win.UltraWinEditors.UltraOptionSet()
            lblUnits = New Infragistics.Win.Misc.UltraLabel()
            btnUpdatePreview = New Infragistics.Win.Misc.UltraButton()
            tblSettings = New TableLayoutPanel()
            copiesPanel = New Panel()
            ugbViews = New Infragistics.Win.Misc.UltraGroupBox()
            uceViews = New Infragistics.Win.UltraWinEditors.UltraComboEditor()
            _PrintControl_Toolbars_Dock_Area_Left = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
            UltraToolbarsManager1 = New Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(components)
            _PrintControl_Toolbars_Dock_Area_Right = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
            _PrintControl_Toolbars_Dock_Area_Top = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
            _PrintControl_Toolbars_Dock_Area_Bottom = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
            UltraToolTipManager1 = New Infragistics.Win.UltraWinToolTip.UltraToolTipManager(components)
            btnRefreshPrinterCollection = New Infragistics.Win.Misc.UltraButton()
            CType(upbPreview, ComponentModel.ISupportInitialize).BeginInit()
            upbPreview.SuspendLayout()
            CType(uckOnlyMargins, ComponentModel.ISupportInitialize).BeginInit()
            CType(ctrlPrintPreview, ComponentModel.ISupportInitialize).BeginInit()
            CType(uceInstalledPrinters, ComponentModel.ISupportInitialize).BeginInit()
            CType(ugbMargins, ComponentModel.ISupportInitialize).BeginInit()
            ugbMargins.SuspendLayout()
            CType(uneRightMargin, ComponentModel.ISupportInitialize).BeginInit()
            CType(uneLeftMargin, ComponentModel.ISupportInitialize).BeginInit()
            CType(uneBottomMargin, ComponentModel.ISupportInitialize).BeginInit()
            CType(uneTopMargin, ComponentModel.ISupportInitialize).BeginInit()
            CType(ugbOrientation, ComponentModel.ISupportInitialize).BeginInit()
            ugbOrientation.SuspendLayout()
            CType(uosOrientation, ComponentModel.ISupportInitialize).BeginInit()
            CType(ugbPaper, ComponentModel.ISupportInitialize).BeginInit()
            ugbPaper.SuspendLayout()
            TableLayoutPanel2.SuspendLayout()
            CType(uneWidth, ComponentModel.ISupportInitialize).BeginInit()
            CType(uneHeight, ComponentModel.ISupportInitialize).BeginInit()
            CType(uckCustomPaperSize, ComponentModel.ISupportInitialize).BeginInit()
            CType(ucePaperSizes, ComponentModel.ISupportInitialize).BeginInit()
            CType(ugbScale, ComponentModel.ISupportInitialize).BeginInit()
            ugbScale.SuspendLayout()
            CType(uckCenterToPaper, ComponentModel.ISupportInitialize).BeginInit()
            CType(uckScaleToFit, ComponentModel.ISupportInitialize).BeginInit()
            CType(uneDrawingUnits, ComponentModel.ISupportInitialize).BeginInit()
            CType(unePrinterUnits, ComponentModel.ISupportInitialize).BeginInit()
            CType(nudNumberOfCopies, ComponentModel.ISupportInitialize).BeginInit()
            CType(ugbPrintArea, ComponentModel.ISupportInitialize).BeginInit()
            ugbPrintArea.SuspendLayout()
            CType(uosPrintArea, ComponentModel.ISupportInitialize).BeginInit()
            CType(uosUnits, ComponentModel.ISupportInitialize).BeginInit()
            tblSettings.SuspendLayout()
            copiesPanel.SuspendLayout()
            CType(ugbViews, ComponentModel.ISupportInitialize).BeginInit()
            ugbViews.SuspendLayout()
            CType(uceViews, ComponentModel.ISupportInitialize).BeginInit()
            CType(UltraToolbarsManager1, ComponentModel.ISupportInitialize).BeginInit()
            SuspendLayout()
            ' 
            ' upbPreview
            ' 
            resources.ApplyResources(upbPreview, "upbPreview")
            upbPreview.Controls.Add(lblLoadingPreview)
            upbPreview.Controls.Add(uckOnlyMargins)
            upbPreview.Controls.Add(ctrlPrintPreview)
            upbPreview.Name = "upbPreview"
            ' 
            ' lblLoadingPreview
            ' 
            resources.ApplyResources(lblLoadingPreview, "lblLoadingPreview")
            lblLoadingPreview.Name = "lblLoadingPreview"
            ' 
            ' uckOnlyMargins
            ' 
            uckOnlyMargins.BackColor = SystemColors.ButtonShadow
            uckOnlyMargins.BackColorInternal = SystemColors.ButtonShadow
            resources.ApplyResources(uckOnlyMargins, "uckOnlyMargins")
            uckOnlyMargins.Name = "uckOnlyMargins"
            ' 
            ' ctrlPrintPreview
            ' 
            resources.ApplyResources(ctrlPrintPreview, "ctrlPrintPreview")
            ctrlPrintPreview.AutoGeneratePreview = False
            ctrlPrintPreview.BackColorInternal = SystemColors.ControlDark
            UltraToolbarsManager1.SetContextMenuUltra(ctrlPrintPreview, "PreviewContextMenu")
            ctrlPrintPreview.DisplayPreviewStatus = False
            ctrlPrintPreview.ForeColor = SystemColors.ControlLightLight
            ctrlPrintPreview.Name = "ctrlPrintPreview"
            ctrlPrintPreview.Settings.MousePanning = Infragistics.Win.MousePanning.Both
            ctrlPrintPreview.Settings.ZoomMode = Infragistics.Win.Printing.ZoomMode.WholePage
            ctrlPrintPreview.UseAntiAlias = True
            ' 
            ' btnClose
            ' 
            resources.ApplyResources(btnClose, "btnClose")
            btnClose.Name = "btnClose"
            ' 
            ' btnPrint
            ' 
            resources.ApplyResources(btnPrint, "btnPrint")
            btnPrint.Name = "btnPrint"
            ' 
            ' uceInstalledPrinters
            ' 
            resources.ApplyResources(uceInstalledPrinters, "uceInstalledPrinters")
            uceInstalledPrinters.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList
            uceInstalledPrinters.Name = "uceInstalledPrinters"
            ' 
            ' lblSelectedPrinter
            ' 
            resources.ApplyResources(lblSelectedPrinter, "lblSelectedPrinter")
            lblSelectedPrinter.Name = "lblSelectedPrinter"
            ' 
            ' btnPrinterSettings
            ' 
            resources.ApplyResources(btnPrinterSettings, "btnPrinterSettings")
            btnPrinterSettings.Name = "btnPrinterSettings"
            ' 
            ' ugbMargins
            ' 
            ugbMargins.Controls.Add(uneRightMargin)
            ugbMargins.Controls.Add(uneLeftMargin)
            ugbMargins.Controls.Add(uneBottomMargin)
            ugbMargins.Controls.Add(uneTopMargin)
            ugbMargins.Controls.Add(lblLeft)
            ugbMargins.Controls.Add(lblRight)
            ugbMargins.Controls.Add(lblBottom)
            ugbMargins.Controls.Add(lblTop)
            resources.ApplyResources(ugbMargins, "ugbMargins")
            ugbMargins.Name = "ugbMargins"
            ' 
            ' uneRightMargin
            ' 
            resources.ApplyResources(uneRightMargin, "uneRightMargin")
            uneRightMargin.FormatString = "##0.0"
            uneRightMargin.MaskInput = "{double:3.1}"
            uneRightMargin.MaxValue = 999R
            uneRightMargin.MinValue = 0R
            uneRightMargin.Name = "uneRightMargin"
            uneRightMargin.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double
            uneRightMargin.PromptChar = " "c
            uneRightMargin.Value = 25.4R
            ' 
            ' uneLeftMargin
            ' 
            resources.ApplyResources(uneLeftMargin, "uneLeftMargin")
            uneLeftMargin.FormatString = "##0.0"
            uneLeftMargin.MaskInput = "{double:3.1}"
            uneLeftMargin.MaxValue = 999R
            uneLeftMargin.MinValue = 0R
            uneLeftMargin.Name = "uneLeftMargin"
            uneLeftMargin.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double
            uneLeftMargin.PromptChar = " "c
            uneLeftMargin.Value = 25.4R
            ' 
            ' uneBottomMargin
            ' 
            resources.ApplyResources(uneBottomMargin, "uneBottomMargin")
            uneBottomMargin.FormatString = "##0.0"
            uneBottomMargin.MaskInput = "{double:3.1}"
            uneBottomMargin.MaxValue = 999R
            uneBottomMargin.MinValue = 0R
            uneBottomMargin.Name = "uneBottomMargin"
            uneBottomMargin.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double
            uneBottomMargin.PromptChar = " "c
            uneBottomMargin.Value = 25.4R
            ' 
            ' uneTopMargin
            ' 
            resources.ApplyResources(uneTopMargin, "uneTopMargin")
            uneTopMargin.FormatString = "##0.0"
            uneTopMargin.MaskInput = "{double:3.1}"
            uneTopMargin.MaxValue = 999R
            uneTopMargin.MinValue = 0R
            uneTopMargin.Name = "uneTopMargin"
            uneTopMargin.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double
            uneTopMargin.PromptChar = " "c
            uneTopMargin.Value = 25.4R
            ' 
            ' lblLeft
            ' 
            resources.ApplyResources(lblLeft, "lblLeft")
            lblLeft.Name = "lblLeft"
            ' 
            ' lblRight
            ' 
            resources.ApplyResources(lblRight, "lblRight")
            lblRight.Name = "lblRight"
            ' 
            ' lblBottom
            ' 
            resources.ApplyResources(lblBottom, "lblBottom")
            lblBottom.Name = "lblBottom"
            ' 
            ' lblTop
            ' 
            resources.ApplyResources(lblTop, "lblTop")
            lblTop.Name = "lblTop"
            ' 
            ' ugbOrientation
            ' 
            ugbOrientation.Controls.Add(uosOrientation)
            resources.ApplyResources(ugbOrientation, "ugbOrientation")
            ugbOrientation.Name = "ugbOrientation"
            ' 
            ' uosOrientation
            ' 
            resources.ApplyResources(uosOrientation, "uosOrientation")
            uosOrientation.BorderStyle = Infragistics.Win.UIElementBorderStyle.None
            ValueListItem1.CheckState = CheckState.Checked
            ValueListItem1.DataValue = False
            resources.ApplyResources(ValueListItem1, "ValueListItem1")
            ValueListItem1.ForceApplyResources = ""
            ValueListItem2.DataValue = True
            resources.ApplyResources(ValueListItem2, "ValueListItem2")
            ValueListItem2.ForceApplyResources = ""
            uosOrientation.Items.AddRange(New Infragistics.Win.ValueListItem() {ValueListItem1, ValueListItem2})
            uosOrientation.ItemSpacingVertical = 20
            uosOrientation.Name = "uosOrientation"
            ' 
            ' ugbPaper
            ' 
            resources.ApplyResources(ugbPaper, "ugbPaper")
            ugbPaper.Controls.Add(TableLayoutPanel2)
            ugbPaper.Controls.Add(uckCustomPaperSize)
            ugbPaper.Controls.Add(ucePaperSizes)
            ugbPaper.Controls.Add(lblSelectedPaperSize)
            ugbPaper.Name = "ugbPaper"
            ' 
            ' TableLayoutPanel2
            ' 
            resources.ApplyResources(TableLayoutPanel2, "TableLayoutPanel2")
            TableLayoutPanel2.Controls.Add(lblHeight, 0, 0)
            TableLayoutPanel2.Controls.Add(uneWidth, 3, 0)
            TableLayoutPanel2.Controls.Add(lblWidth, 2, 0)
            TableLayoutPanel2.Controls.Add(uneHeight, 1, 0)
            TableLayoutPanel2.Name = "TableLayoutPanel2"
            ' 
            ' lblHeight
            ' 
            resources.ApplyResources(lblHeight, "lblHeight")
            lblHeight.Name = "lblHeight"
            ' 
            ' uneWidth
            ' 
            resources.ApplyResources(uneWidth, "uneWidth")
            uneWidth.FormatString = "####0.0"
            uneWidth.MaskInput = "{double:5.1}"
            uneWidth.MaxValue = 99999R
            uneWidth.MinValue = 0.1R
            uneWidth.Name = "uneWidth"
            uneWidth.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double
            uneWidth.PromptChar = " "c
            uneWidth.Value = 100R
            ' 
            ' lblWidth
            ' 
            resources.ApplyResources(lblWidth, "lblWidth")
            lblWidth.Name = "lblWidth"
            ' 
            ' uneHeight
            ' 
            resources.ApplyResources(uneHeight, "uneHeight")
            uneHeight.FormatString = "####0.0"
            uneHeight.MaskInput = "{double:5.1}"
            uneHeight.MaxValue = 99999R
            uneHeight.MinValue = 0.1R
            uneHeight.Name = "uneHeight"
            uneHeight.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double
            uneHeight.PromptChar = " "c
            uneHeight.Value = 100R
            ' 
            ' uckCustomPaperSize
            ' 
            resources.ApplyResources(uckCustomPaperSize, "uckCustomPaperSize")
            uckCustomPaperSize.Name = "uckCustomPaperSize"
            ' 
            ' ucePaperSizes
            ' 
            resources.ApplyResources(ucePaperSizes, "ucePaperSizes")
            ucePaperSizes.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList
            ucePaperSizes.Name = "ucePaperSizes"
            ' 
            ' lblSelectedPaperSize
            ' 
            resources.ApplyResources(lblSelectedPaperSize, "lblSelectedPaperSize")
            lblSelectedPaperSize.Name = "lblSelectedPaperSize"
            ' 
            ' ugbScale
            ' 
            resources.ApplyResources(ugbScale, "ugbScale")
            ugbScale.Controls.Add(uckCenterToPaper)
            ugbScale.Controls.Add(uckScaleToFit)
            ugbScale.Controls.Add(uneDrawingUnits)
            ugbScale.Controls.Add(lblDrawingUnits)
            ugbScale.Controls.Add(unePrinterUnits)
            ugbScale.Controls.Add(lblPrinterUnits)
            ugbScale.Name = "ugbScale"
            tblSettings.SetRowSpan(ugbScale, 2)
            ' 
            ' uckCenterToPaper
            ' 
            resources.ApplyResources(uckCenterToPaper, "uckCenterToPaper")
            uckCenterToPaper.Name = "uckCenterToPaper"
            ' 
            ' uckScaleToFit
            ' 
            resources.ApplyResources(uckScaleToFit, "uckScaleToFit")
            uckScaleToFit.Name = "uckScaleToFit"
            ' 
            ' uneDrawingUnits
            ' 
            resources.ApplyResources(uneDrawingUnits, "uneDrawingUnits")
            uneDrawingUnits.FormatString = "###0.0#"
            uneDrawingUnits.MaskInput = "{double:4.2}"
            uneDrawingUnits.MaxValue = 9999.99R
            uneDrawingUnits.MinValue = 0.01R
            uneDrawingUnits.Name = "uneDrawingUnits"
            uneDrawingUnits.NullText = "<Value missing>"
            uneDrawingUnits.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double
            uneDrawingUnits.PromptChar = " "c
            uneDrawingUnits.Value = 1R
            ' 
            ' lblDrawingUnits
            ' 
            resources.ApplyResources(lblDrawingUnits, "lblDrawingUnits")
            lblDrawingUnits.Name = "lblDrawingUnits"
            ' 
            ' unePrinterUnits
            ' 
            resources.ApplyResources(unePrinterUnits, "unePrinterUnits")
            unePrinterUnits.FormatString = "###0.0#"
            unePrinterUnits.MaskInput = "{double:4.2}"
            unePrinterUnits.MaxValue = 9999.99R
            unePrinterUnits.MinValue = 0.01R
            unePrinterUnits.Name = "unePrinterUnits"
            unePrinterUnits.NullText = "<Value missing>"
            unePrinterUnits.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double
            unePrinterUnits.PromptChar = " "c
            unePrinterUnits.Value = 1R
            ' 
            ' lblPrinterUnits
            ' 
            resources.ApplyResources(lblPrinterUnits, "lblPrinterUnits")
            lblPrinterUnits.Name = "lblPrinterUnits"
            ' 
            ' nudNumberOfCopies
            ' 
            resources.ApplyResources(nudNumberOfCopies, "nudNumberOfCopies")
            nudNumberOfCopies.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
            nudNumberOfCopies.Name = "nudNumberOfCopies"
            nudNumberOfCopies.Value = New Decimal(New Integer() {1, 0, 0, 0})
            ' 
            ' lblNumberOfCopies
            ' 
            resources.ApplyResources(lblNumberOfCopies, "lblNumberOfCopies")
            lblNumberOfCopies.Name = "lblNumberOfCopies"
            ' 
            ' ugbPrintArea
            ' 
            ugbPrintArea.Controls.Add(uosPrintArea)
            ugbPrintArea.Controls.Add(btnPick)
            resources.ApplyResources(ugbPrintArea, "ugbPrintArea")
            ugbPrintArea.Name = "ugbPrintArea"
            ' 
            ' uosPrintArea
            ' 
            resources.ApplyResources(uosPrintArea, "uosPrintArea")
            uosPrintArea.BorderStyle = Infragistics.Win.UIElementBorderStyle.None
            uosPrintArea.CheckedIndex = 0
            ValueListItem9.CheckState = CheckState.Checked
            ValueListItem9.DataValue = "Extends"
            resources.ApplyResources(ValueListItem9, "ValueListItem9")
            ValueListItem9.ForceApplyResources = ""
            ValueListItem10.DataValue = "Window"
            resources.ApplyResources(ValueListItem10, "ValueListItem10")
            ValueListItem10.ForceApplyResources = ""
            uosPrintArea.Items.AddRange(New Infragistics.Win.ValueListItem() {ValueListItem9, ValueListItem10})
            uosPrintArea.ItemSpacingHorizontal = 10
            uosPrintArea.ItemSpacingVertical = 10
            uosPrintArea.Name = "uosPrintArea"
            ' 
            ' btnPick
            ' 
            resources.ApplyResources(btnPick, "btnPick")
            Appearance1.Image = My.Resources.document_pick
            resources.ApplyResources(Appearance1.FontData, "Appearance1.FontData")
            resources.ApplyResources(Appearance1, "Appearance1")
            Appearance1.ForceApplyResources = "FontData|"
            btnPick.Appearance = Appearance1
            btnPick.Name = "btnPick"
            ' 
            ' uosUnits
            ' 
            resources.ApplyResources(uosUnits, "uosUnits")
            uosUnits.BorderStyle = Infragistics.Win.UIElementBorderStyle.None
            uosUnits.CheckedIndex = 1
            ValueListItem5.DataValue = "Inches"
            resources.ApplyResources(ValueListItem5, "ValueListItem5")
            ValueListItem5.ForceApplyResources = ""
            ValueListItem6.CheckState = CheckState.Checked
            ValueListItem6.DataValue = "Millimeter"
            resources.ApplyResources(ValueListItem6, "ValueListItem6")
            ValueListItem6.ForceApplyResources = ""
            uosUnits.Items.AddRange(New Infragistics.Win.ValueListItem() {ValueListItem5, ValueListItem6})
            uosUnits.ItemSpacingHorizontal = 10
            uosUnits.ItemSpacingVertical = 10
            uosUnits.Name = "uosUnits"
            ' 
            ' lblUnits
            ' 
            resources.ApplyResources(lblUnits, "lblUnits")
            lblUnits.Name = "lblUnits"
            ' 
            ' btnUpdatePreview
            ' 
            resources.ApplyResources(btnUpdatePreview, "btnUpdatePreview")
            Appearance2.Image = My.Resources.Refresh
            resources.ApplyResources(Appearance2.FontData, "Appearance2.FontData")
            resources.ApplyResources(Appearance2, "Appearance2")
            Appearance2.ForceApplyResources = "FontData|"
            btnUpdatePreview.Appearance = Appearance2
            btnUpdatePreview.Name = "btnUpdatePreview"
            ' 
            ' tblSettings
            ' 
            resources.ApplyResources(tblSettings, "tblSettings")
            tblSettings.Controls.Add(ugbMargins, 0, 0)
            tblSettings.Controls.Add(ugbOrientation, 1, 0)
            tblSettings.Controls.Add(ugbPrintArea, 2, 1)
            tblSettings.Controls.Add(ugbPaper, 2, 0)
            tblSettings.Controls.Add(ugbScale, 3, 0)
            tblSettings.Controls.Add(copiesPanel, 0, 1)
            tblSettings.Controls.Add(ugbViews, 4, 0)
            tblSettings.Name = "tblSettings"
            ' 
            ' copiesPanel
            ' 
            tblSettings.SetColumnSpan(copiesPanel, 2)
            copiesPanel.Controls.Add(lblNumberOfCopies)
            copiesPanel.Controls.Add(nudNumberOfCopies)
            copiesPanel.Controls.Add(lblUnits)
            copiesPanel.Controls.Add(uosUnits)
            resources.ApplyResources(copiesPanel, "copiesPanel")
            copiesPanel.Name = "copiesPanel"
            ' 
            ' ugbViews
            ' 
            ugbViews.Controls.Add(uceViews)
            resources.ApplyResources(ugbViews, "ugbViews")
            ugbViews.Name = "ugbViews"
            ' 
            ' uceViews
            ' 
            resources.ApplyResources(uceViews, "uceViews")
            uceViews.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList
            ValueListItem16.DataValue = "Front"
            resources.ApplyResources(ValueListItem16, "ValueListItem16")
            ValueListItem16.ForceApplyResources = ""
            ValueListItem17.DataValue = "Back"
            resources.ApplyResources(ValueListItem17, "ValueListItem17")
            ValueListItem17.ForceApplyResources = ""
            ValueListItem18.DataValue = "Left"
            resources.ApplyResources(ValueListItem18, "ValueListItem18")
            ValueListItem18.ForceApplyResources = ""
            ValueListItem19.DataValue = "Right"
            resources.ApplyResources(ValueListItem19, "ValueListItem19")
            ValueListItem19.ForceApplyResources = ""
            ValueListItem20.DataValue = "Top"
            resources.ApplyResources(ValueListItem20, "ValueListItem20")
            ValueListItem20.ForceApplyResources = ""
            ValueListItem21.DataValue = "Bottom"
            resources.ApplyResources(ValueListItem21, "ValueListItem21")
            ValueListItem21.ForceApplyResources = ""
            ValueListItem22.DataValue = "Isometric"
            resources.ApplyResources(ValueListItem22, "ValueListItem22")
            ValueListItem22.ForceApplyResources = ""
            uceViews.Items.AddRange(New Infragistics.Win.ValueListItem() {ValueListItem16, ValueListItem17, ValueListItem18, ValueListItem19, ValueListItem20, ValueListItem21, ValueListItem22})
            uceViews.Name = "uceViews"
            ' 
            ' _PrintControl_Toolbars_Dock_Area_Left
            ' 
            _PrintControl_Toolbars_Dock_Area_Left.AccessibleRole = AccessibleRole.Grouping
            _PrintControl_Toolbars_Dock_Area_Left.BackColor = SystemColors.Control
            _PrintControl_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left
            _PrintControl_Toolbars_Dock_Area_Left.ForeColor = SystemColors.ControlText
            resources.ApplyResources(_PrintControl_Toolbars_Dock_Area_Left, "_PrintControl_Toolbars_Dock_Area_Left")
            _PrintControl_Toolbars_Dock_Area_Left.Name = "_PrintControl_Toolbars_Dock_Area_Left"
            _PrintControl_Toolbars_Dock_Area_Left.ToolbarsManager = UltraToolbarsManager1
            ' 
            ' UltraToolbarsManager1
            ' 
            UltraToolbarsManager1.DesignerFlags = 1
            UltraToolbarsManager1.DockWithinContainer = Me
            UltraToolbarsManager1.SettingsKey = "PrintControl.UltraToolbarsManager1"
            UltraToolbarsManager1.ShowFullMenusDelay = 500
            ButtonTool1.SharedPropsInternal.Caption = resources.GetString("resource.Caption")
            ButtonTool1.SharedPropsInternal.Category = resources.GetString("resource.Category")
            PopupMenuTool1.SharedPropsInternal.Caption = resources.GetString("resource.Caption1")
            PopupMenuTool1.SharedPropsInternal.Category = resources.GetString("resource.Category1")
            PopupMenuTool1.Tools.AddRange(New Infragistics.Win.UltraWinToolbars.ToolBase() {ButtonTool3, ButtonTool2})
            Appearance3.Image = My.Resources.Refresh
            resources.ApplyResources(Appearance3.FontData, "Appearance3.FontData")
            resources.ApplyResources(Appearance3, "Appearance3")
            Appearance3.ForceApplyResources = "FontData|"
            ButtonTool4.SharedPropsInternal.AppearancesLarge.Appearance = Appearance3
            Appearance4.Image = My.Resources.Refresh
            resources.ApplyResources(Appearance4.FontData, "Appearance4.FontData")
            resources.ApplyResources(Appearance4, "Appearance4")
            Appearance4.ForceApplyResources = "FontData|"
            ButtonTool4.SharedPropsInternal.AppearancesSmall.Appearance = Appearance4
            resources.ApplyResources(ButtonTool4.SharedPropsInternal, "ButtonTool4.SharedPropsInternal")
            ButtonTool4.ForceApplyResources = "SharedPropsInternal"
            UltraToolbarsManager1.Tools.AddRange(New Infragistics.Win.UltraWinToolbars.ToolBase() {ButtonTool1, PopupMenuTool1, ButtonTool4})
            ' 
            ' _PrintControl_Toolbars_Dock_Area_Right
            ' 
            _PrintControl_Toolbars_Dock_Area_Right.AccessibleRole = AccessibleRole.Grouping
            _PrintControl_Toolbars_Dock_Area_Right.BackColor = SystemColors.Control
            _PrintControl_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right
            _PrintControl_Toolbars_Dock_Area_Right.ForeColor = SystemColors.ControlText
            resources.ApplyResources(_PrintControl_Toolbars_Dock_Area_Right, "_PrintControl_Toolbars_Dock_Area_Right")
            _PrintControl_Toolbars_Dock_Area_Right.Name = "_PrintControl_Toolbars_Dock_Area_Right"
            _PrintControl_Toolbars_Dock_Area_Right.ToolbarsManager = UltraToolbarsManager1
            ' 
            ' _PrintControl_Toolbars_Dock_Area_Top
            ' 
            _PrintControl_Toolbars_Dock_Area_Top.AccessibleRole = AccessibleRole.Grouping
            _PrintControl_Toolbars_Dock_Area_Top.BackColor = SystemColors.Control
            _PrintControl_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top
            _PrintControl_Toolbars_Dock_Area_Top.ForeColor = SystemColors.ControlText
            resources.ApplyResources(_PrintControl_Toolbars_Dock_Area_Top, "_PrintControl_Toolbars_Dock_Area_Top")
            _PrintControl_Toolbars_Dock_Area_Top.Name = "_PrintControl_Toolbars_Dock_Area_Top"
            _PrintControl_Toolbars_Dock_Area_Top.ToolbarsManager = UltraToolbarsManager1
            ' 
            ' _PrintControl_Toolbars_Dock_Area_Bottom
            ' 
            _PrintControl_Toolbars_Dock_Area_Bottom.AccessibleRole = AccessibleRole.Grouping
            _PrintControl_Toolbars_Dock_Area_Bottom.BackColor = SystemColors.Control
            _PrintControl_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom
            _PrintControl_Toolbars_Dock_Area_Bottom.ForeColor = SystemColors.ControlText
            resources.ApplyResources(_PrintControl_Toolbars_Dock_Area_Bottom, "_PrintControl_Toolbars_Dock_Area_Bottom")
            _PrintControl_Toolbars_Dock_Area_Bottom.Name = "_PrintControl_Toolbars_Dock_Area_Bottom"
            _PrintControl_Toolbars_Dock_Area_Bottom.ToolbarsManager = UltraToolbarsManager1
            ' 
            ' UltraToolTipManager1
            ' 
            UltraToolTipManager1.ContainingControl = Me
            ' 
            ' btnRefreshPrinterCollection
            ' 
            resources.ApplyResources(btnRefreshPrinterCollection, "btnRefreshPrinterCollection")
            Appearance5.Image = My.Resources.Refresh
            Appearance5.ImageHAlign = Infragistics.Win.HAlign.Center
            Appearance5.ImageVAlign = Infragistics.Win.VAlign.Middle
            resources.ApplyResources(Appearance5.FontData, "Appearance5.FontData")
            resources.ApplyResources(Appearance5, "Appearance5")
            Appearance5.ForceApplyResources = "FontData|"
            btnRefreshPrinterCollection.Appearance = Appearance5
            btnRefreshPrinterCollection.Name = "btnRefreshPrinterCollection"
            ' 
            ' PrintControl
            ' 
            AutoScaleMode = AutoScaleMode.Inherit
            Controls.Add(btnRefreshPrinterCollection)
            Controls.Add(tblSettings)
            Controls.Add(btnUpdatePreview)
            Controls.Add(btnPrinterSettings)
            Controls.Add(uceInstalledPrinters)
            Controls.Add(lblSelectedPrinter)
            Controls.Add(btnPrint)
            Controls.Add(btnClose)
            Controls.Add(upbPreview)
            Controls.Add(_PrintControl_Toolbars_Dock_Area_Left)
            Controls.Add(_PrintControl_Toolbars_Dock_Area_Right)
            Controls.Add(_PrintControl_Toolbars_Dock_Area_Bottom)
            Controls.Add(_PrintControl_Toolbars_Dock_Area_Top)
            resources.ApplyResources(Me, "$this")
            Name = "PrintControl"
            CType(upbPreview, ComponentModel.ISupportInitialize).EndInit()
            upbPreview.ResumeLayout(False)
            upbPreview.PerformLayout()
            CType(uckOnlyMargins, ComponentModel.ISupportInitialize).EndInit()
            CType(ctrlPrintPreview, ComponentModel.ISupportInitialize).EndInit()
            CType(uceInstalledPrinters, ComponentModel.ISupportInitialize).EndInit()
            CType(ugbMargins, ComponentModel.ISupportInitialize).EndInit()
            ugbMargins.ResumeLayout(False)
            ugbMargins.PerformLayout()
            CType(uneRightMargin, ComponentModel.ISupportInitialize).EndInit()
            CType(uneLeftMargin, ComponentModel.ISupportInitialize).EndInit()
            CType(uneBottomMargin, ComponentModel.ISupportInitialize).EndInit()
            CType(uneTopMargin, ComponentModel.ISupportInitialize).EndInit()
            CType(ugbOrientation, ComponentModel.ISupportInitialize).EndInit()
            ugbOrientation.ResumeLayout(False)
            CType(uosOrientation, ComponentModel.ISupportInitialize).EndInit()
            CType(ugbPaper, ComponentModel.ISupportInitialize).EndInit()
            ugbPaper.ResumeLayout(False)
            ugbPaper.PerformLayout()
            TableLayoutPanel2.ResumeLayout(False)
            TableLayoutPanel2.PerformLayout()
            CType(uneWidth, ComponentModel.ISupportInitialize).EndInit()
            CType(uneHeight, ComponentModel.ISupportInitialize).EndInit()
            CType(uckCustomPaperSize, ComponentModel.ISupportInitialize).EndInit()
            CType(ucePaperSizes, ComponentModel.ISupportInitialize).EndInit()
            CType(ugbScale, ComponentModel.ISupportInitialize).EndInit()
            ugbScale.ResumeLayout(False)
            ugbScale.PerformLayout()
            CType(uckCenterToPaper, ComponentModel.ISupportInitialize).EndInit()
            CType(uckScaleToFit, ComponentModel.ISupportInitialize).EndInit()
            CType(uneDrawingUnits, ComponentModel.ISupportInitialize).EndInit()
            CType(unePrinterUnits, ComponentModel.ISupportInitialize).EndInit()
            CType(nudNumberOfCopies, ComponentModel.ISupportInitialize).EndInit()
            CType(ugbPrintArea, ComponentModel.ISupportInitialize).EndInit()
            ugbPrintArea.ResumeLayout(False)
            CType(uosPrintArea, ComponentModel.ISupportInitialize).EndInit()
            CType(uosUnits, ComponentModel.ISupportInitialize).EndInit()
            tblSettings.ResumeLayout(False)
            copiesPanel.ResumeLayout(False)
            CType(ugbViews, ComponentModel.ISupportInitialize).EndInit()
            ugbViews.ResumeLayout(False)
            ugbViews.PerformLayout()
            CType(uceViews, ComponentModel.ISupportInitialize).EndInit()
            CType(UltraToolbarsManager1, ComponentModel.ISupportInitialize).EndInit()
            ResumeLayout(False)
            PerformLayout()

        End Sub
        Protected Friend WithEvents nudNumberOfCopies As System.Windows.Forms.NumericUpDown
        Friend WithEvents tblSettings As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents copiesPanel As System.Windows.Forms.Panel
        Friend WithEvents TableLayoutPanel2 As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents lblLoadingPreview As System.Windows.Forms.Label
        Private components As System.ComponentModel.IContainer
        Friend WithEvents upbPreview As Infragistics.Win.Misc.UltraGroupBox
        Friend WithEvents btnClose As Infragistics.Win.Misc.UltraButton
        Friend WithEvents btnPrint As Infragistics.Win.Misc.UltraButton
        Friend WithEvents ctrlPrintPreview As Infragistics.Win.Printing.UltraPrintPreviewControl
        Friend WithEvents uceInstalledPrinters As Infragistics.Win.UltraWinEditors.UltraComboEditor
        Friend WithEvents lblSelectedPrinter As Infragistics.Win.Misc.UltraLabel
        Friend WithEvents btnPrinterSettings As Infragistics.Win.Misc.UltraButton
        Friend WithEvents ugbMargins As Infragistics.Win.Misc.UltraGroupBox
        Friend WithEvents uneRightMargin As Infragistics.Win.UltraWinEditors.UltraNumericEditor
        Friend WithEvents uneLeftMargin As Infragistics.Win.UltraWinEditors.UltraNumericEditor
        Friend WithEvents uneBottomMargin As Infragistics.Win.UltraWinEditors.UltraNumericEditor
        Friend WithEvents uneTopMargin As Infragistics.Win.UltraWinEditors.UltraNumericEditor
        Friend WithEvents lblLeft As Infragistics.Win.Misc.UltraLabel
        Friend WithEvents lblRight As Infragistics.Win.Misc.UltraLabel
        Friend WithEvents lblBottom As Infragistics.Win.Misc.UltraLabel
        Friend WithEvents lblTop As Infragistics.Win.Misc.UltraLabel
        Friend WithEvents ugbOrientation As Infragistics.Win.Misc.UltraGroupBox
        Friend WithEvents uosOrientation As Infragistics.Win.UltraWinEditors.UltraOptionSet
        Friend WithEvents ugbPaper As Infragistics.Win.Misc.UltraGroupBox
        Friend WithEvents ucePaperSizes As Infragistics.Win.UltraWinEditors.UltraComboEditor
        Friend WithEvents lblSelectedPaperSize As Infragistics.Win.Misc.UltraLabel
        Friend WithEvents lblWidth As Infragistics.Win.Misc.UltraLabel
        Friend WithEvents uneWidth As Infragistics.Win.UltraWinEditors.UltraNumericEditor
        Friend WithEvents lblHeight As Infragistics.Win.Misc.UltraLabel
        Friend WithEvents uneHeight As Infragistics.Win.UltraWinEditors.UltraNumericEditor
        Friend WithEvents uckCustomPaperSize As Infragistics.Win.UltraWinEditors.UltraCheckEditor
        Friend WithEvents ugbScale As Infragistics.Win.Misc.UltraGroupBox
        Friend WithEvents uckScaleToFit As Infragistics.Win.UltraWinEditors.UltraCheckEditor
        Friend WithEvents uneDrawingUnits As Infragistics.Win.UltraWinEditors.UltraNumericEditor
        Friend WithEvents lblDrawingUnits As Infragistics.Win.Misc.UltraLabel
        Friend WithEvents unePrinterUnits As Infragistics.Win.UltraWinEditors.UltraNumericEditor
        Friend WithEvents lblPrinterUnits As Infragistics.Win.Misc.UltraLabel
        Friend WithEvents lblNumberOfCopies As Infragistics.Win.Misc.UltraLabel
        Friend WithEvents ugbPrintArea As Infragistics.Win.Misc.UltraGroupBox
        Friend WithEvents btnPick As Infragistics.Win.Misc.UltraButton
        Friend WithEvents uosPrintArea As Infragistics.Win.UltraWinEditors.UltraOptionSet
        Friend WithEvents uosUnits As Infragistics.Win.UltraWinEditors.UltraOptionSet
        Friend WithEvents lblUnits As Infragistics.Win.Misc.UltraLabel
        Friend WithEvents uckOnlyMargins As Infragistics.Win.UltraWinEditors.UltraCheckEditor
        Friend WithEvents btnUpdatePreview As Infragistics.Win.Misc.UltraButton
        Friend WithEvents UltraToolbarsManager1 As Infragistics.Win.UltraWinToolbars.UltraToolbarsManager
        Friend WithEvents _PrintControl_Toolbars_Dock_Area_Left As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
        Friend WithEvents _PrintControl_Toolbars_Dock_Area_Right As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
        Friend WithEvents _PrintControl_Toolbars_Dock_Area_Bottom As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
        Friend WithEvents _PrintControl_Toolbars_Dock_Area_Top As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
        Friend WithEvents uckCenterToPaper As Infragistics.Win.UltraWinEditors.UltraCheckEditor
        Friend WithEvents UltraToolTipManager1 As Infragistics.Win.UltraWinToolTip.UltraToolTipManager
        Friend WithEvents ugbViews As Infragistics.Win.Misc.UltraGroupBox
        Friend WithEvents uceViews As Infragistics.Win.UltraWinEditors.UltraComboEditor
        Friend WithEvents btnRefreshPrinterCollection As Infragistics.Win.Misc.UltraButton
    End Class

End Namespace