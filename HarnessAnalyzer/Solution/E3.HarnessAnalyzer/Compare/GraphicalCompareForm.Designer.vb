<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class GraphicalCompareForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            If disposing Then
                _factory?.Dispose()
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
        components = New ComponentModel.Container()
        Dim Resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(GraphicalCompareForm))
        Dim Appearance1 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance2 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance3 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim EditorButton1 As Infragistics.Win.UltraWinEditors.EditorButton = New Infragistics.Win.UltraWinEditors.EditorButton("btnEdit")
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
        Dim Appearance21 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance23 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance22 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance20 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance19 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        upnResult = New Infragistics.Win.Misc.UltraPanel()
        upnEntry = New Infragistics.Win.Misc.UltraPanel()
        lblComment = New Infragistics.Win.Misc.UltraLabel()
        upnGraphic = New Infragistics.Win.Misc.UltraPanel()
        lblCompare = New Infragistics.Win.Misc.UltraLabel()
        lblReference = New Infragistics.Win.Misc.UltraLabel()
        lblNoGraphicsToDisplay = New Infragistics.Win.Misc.UltraLabel()
        btnSync = New Infragistics.Win.Misc.UltraButton()
        picCompResult = New PictureBox()
        picRefResult = New PictureBox()
        btnNext = New Infragistics.Win.Misc.UltraButton()
        btnPrevious = New Infragistics.Win.Misc.UltraButton()
        txtComment = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        uspResult = New Infragistics.Win.Misc.UltraSplitter()
        ugResults = New Infragistics.Win.UltraWinGrid.UltraGrid()
        ugbCompareSettings = New Infragistics.Win.Misc.UltraGroupBox()
        chkConsiderText = New Infragistics.Win.UltraWinEditors.UltraCheckEditor()
        uckConsiderOffset = New Infragistics.Win.UltraWinEditors.UltraCheckEditor()
        uneOffsetVectorY = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
        lblOffsetVector = New Infragistics.Win.Misc.UltraLabel()
        uneOffsetVectorX = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
        btnOffsetPreview = New Infragistics.Win.Misc.UltraButton()
        uceCompareDrw = New Infragistics.Win.UltraWinEditors.UltraComboEditor()
        lblCompareDrw = New Infragistics.Win.Misc.UltraLabel()
        uceReferenceDrw = New Infragistics.Win.UltraWinEditors.UltraComboEditor()
        lblReferenceDrw = New Infragistics.Win.Misc.UltraLabel()
        uceCompareDocument = New Infragistics.Win.UltraWinEditors.UltraComboEditor()
        txtReferenceDocument = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        lblCompareDocument = New Infragistics.Win.Misc.UltraLabel()
        lblReferenceDocument = New Infragistics.Win.Misc.UltraLabel()
        ugbCompareResults = New Infragistics.Win.Misc.UltraGroupBox()
        btnClose = New Infragistics.Win.Misc.UltraButton()
        btnExport = New Infragistics.Win.Misc.UltraButton()
        btnCompare = New Infragistics.Win.Misc.UltraButton()
        bwCompare = New ComponentModel.BackgroundWorker()
        upbCompare = New Infragistics.Win.UltraWinProgressBar.UltraProgressBar()
        uttmCompare = New Infragistics.Win.UltraWinToolTip.UltraToolTipManager(components)
        lblLegend1 = New Infragistics.Win.Misc.UltraLabel()
        lblLegend2 = New Infragistics.Win.Misc.UltraLabel()
        lblLegend3 = New Infragistics.Win.Misc.UltraLabel()
        udsResults = New Infragistics.Win.UltraWinDataSource.UltraDataSource(components)
        lblRowCountInfo = New Infragistics.Win.Misc.UltraLabel()
        ugeeResults = New Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter(components)
        utmCompare = New Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(components)
        _GraphicalCompareForm_Toolbars_Dock_Area_Left = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        _GraphicalCompareForm_Toolbars_Dock_Area_Right = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        _GraphicalCompareForm_Toolbars_Dock_Area_Top = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        _GraphicalCompareForm_Toolbars_Dock_Area_Bottom = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        btnResetAreaFilter = New Infragistics.Win.Misc.UltraButton()
        uddbSetAreaFilter = New Infragistics.Win.Misc.UltraDropDownButton()
        uckDisplayIndicators = New Infragistics.Win.UltraWinEditors.UltraCheckEditor()
        uckFilterDynamically = New Infragistics.Win.UltraWinEditors.UltraCheckEditor()
        btnSave = New Infragistics.Win.Misc.UltraButton()
        ProgressBar1 = New ProgressBar()
        upnResult.ClientArea.SuspendLayout()
        upnResult.SuspendLayout()
        upnEntry.ClientArea.SuspendLayout()
        upnEntry.SuspendLayout()
        upnGraphic.ClientArea.SuspendLayout()
        upnGraphic.SuspendLayout()
        CType(picCompResult, ComponentModel.ISupportInitialize).BeginInit()
        CType(picRefResult, ComponentModel.ISupportInitialize).BeginInit()
        CType(txtComment, ComponentModel.ISupportInitialize).BeginInit()
        CType(ugResults, ComponentModel.ISupportInitialize).BeginInit()
        CType(ugbCompareSettings, ComponentModel.ISupportInitialize).BeginInit()
        ugbCompareSettings.SuspendLayout()
        CType(chkConsiderText, ComponentModel.ISupportInitialize).BeginInit()
        CType(uckConsiderOffset, ComponentModel.ISupportInitialize).BeginInit()
        CType(uneOffsetVectorY, ComponentModel.ISupportInitialize).BeginInit()
        CType(uneOffsetVectorX, ComponentModel.ISupportInitialize).BeginInit()
        CType(uceCompareDrw, ComponentModel.ISupportInitialize).BeginInit()
        CType(uceReferenceDrw, ComponentModel.ISupportInitialize).BeginInit()
        CType(uceCompareDocument, ComponentModel.ISupportInitialize).BeginInit()
        CType(txtReferenceDocument, ComponentModel.ISupportInitialize).BeginInit()
        CType(ugbCompareResults, ComponentModel.ISupportInitialize).BeginInit()
        ugbCompareResults.SuspendLayout()
        CType(udsResults, ComponentModel.ISupportInitialize).BeginInit()
        CType(utmCompare, ComponentModel.ISupportInitialize).BeginInit()
        CType(uckDisplayIndicators, ComponentModel.ISupportInitialize).BeginInit()
        CType(uckFilterDynamically, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' upnResult
        ' 
        resources.ApplyResources(upnResult, "upnResult")
        ' 
        ' upnResult.ClientArea
        ' 
        upnResult.ClientArea.Controls.Add(upnEntry)
        upnResult.ClientArea.Controls.Add(uspResult)
        upnResult.ClientArea.Controls.Add(ugResults)
        upnResult.Name = "upnResult"
        ' 
        ' upnEntry
        ' 
        ' 
        ' upnEntry.ClientArea
        ' 
        upnEntry.ClientArea.Controls.Add(lblComment)
        upnEntry.ClientArea.Controls.Add(upnGraphic)
        upnEntry.ClientArea.Controls.Add(btnNext)
        upnEntry.ClientArea.Controls.Add(btnPrevious)
        upnEntry.ClientArea.Controls.Add(txtComment)
        resources.ApplyResources(upnEntry, "upnEntry")
        upnEntry.Name = "upnEntry"
        ' 
        ' lblComment
        ' 
        resources.ApplyResources(lblComment, "lblComment")
        lblComment.Name = "lblComment"
        ' 
        ' upnGraphic
        ' 
        resources.ApplyResources(upnGraphic, "upnGraphic")
        ' 
        ' upnGraphic.ClientArea
        ' 
        upnGraphic.ClientArea.Controls.Add(lblCompare)
        upnGraphic.ClientArea.Controls.Add(lblReference)
        upnGraphic.ClientArea.Controls.Add(lblNoGraphicsToDisplay)
        upnGraphic.ClientArea.Controls.Add(btnSync)
        upnGraphic.ClientArea.Controls.Add(picCompResult)
        upnGraphic.ClientArea.Controls.Add(picRefResult)
        upnGraphic.Name = "upnGraphic"
        ' 
        ' lblCompare
        ' 
        resources.ApplyResources(lblCompare, "lblCompare")
        lblCompare.Name = "lblCompare"
        ' 
        ' lblReference
        ' 
        resources.ApplyResources(lblReference, "lblReference")
        lblReference.Name = "lblReference"
        ' 
        ' lblNoGraphicsToDisplay
        ' 
        resources.ApplyResources(lblNoGraphicsToDisplay, "lblNoGraphicsToDisplay")
        Appearance1.BackColor = Color.Transparent
        Appearance1.ForeColor = Color.Red
        lblNoGraphicsToDisplay.Appearance = Appearance1
        lblNoGraphicsToDisplay.Name = "lblNoGraphicsToDisplay"
        ' 
        ' btnSync
        ' 
        resources.ApplyResources(btnSync, "btnSync")
        btnSync.Name = "btnSync"
        ' 
        ' picCompResult
        ' 
        picCompResult.BorderStyle = BorderStyle.FixedSingle
        resources.ApplyResources(picCompResult, "picCompResult")
        picCompResult.Name = "picCompResult"
        picCompResult.TabStop = False
        ' 
        ' picRefResult
        ' 
        picRefResult.BorderStyle = BorderStyle.FixedSingle
        resources.ApplyResources(picRefResult, "picRefResult")
        picRefResult.Name = "picRefResult"
        picRefResult.TabStop = False
        ' 
        ' btnNext
        ' 
        resources.ApplyResources(btnNext, "btnNext")
        Appearance2.FontData.BoldAsString = resources.GetString("resource.BoldAsString")
        btnNext.Appearance = Appearance2
        btnNext.Name = "btnNext"
        ' 
        ' btnPrevious
        ' 
        resources.ApplyResources(btnPrevious, "btnPrevious")
        Appearance3.FontData.BoldAsString = resources.GetString("resource.BoldAsString1")
        btnPrevious.Appearance = Appearance3
        btnPrevious.Name = "btnPrevious"
        ' 
        ' txtComment
        ' 
        resources.ApplyResources(txtComment, "txtComment")
        EditorButton1.Key = "btnEdit"
        txtComment.ButtonsRight.Add(EditorButton1)
        txtComment.Name = "txtComment"
        ' 
        ' uspResult
        ' 
        uspResult.BackColor = SystemColors.Control
        resources.ApplyResources(uspResult, "uspResult")
        uspResult.Name = "uspResult"
        uspResult.RestoreExtent = 120
        ' 
        ' ugResults
        ' 
        Appearance4.BackColor = SystemColors.Window
        Appearance4.BorderColor = SystemColors.InactiveCaption
        ugResults.DisplayLayout.Appearance = Appearance4
        ugResults.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        ugResults.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False
        Appearance5.BackColor = SystemColors.ActiveBorder
        Appearance5.BackColor2 = SystemColors.ControlDark
        Appearance5.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical
        Appearance5.BorderColor = SystemColors.Window
        ugResults.DisplayLayout.GroupByBox.Appearance = Appearance5
        Appearance6.ForeColor = SystemColors.GrayText
        ugResults.DisplayLayout.GroupByBox.BandLabelAppearance = Appearance6
        ugResults.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        Appearance7.BackColor = SystemColors.ControlLightLight
        Appearance7.BackColor2 = SystemColors.Control
        Appearance7.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance7.ForeColor = SystemColors.GrayText
        ugResults.DisplayLayout.GroupByBox.PromptAppearance = Appearance7
        ugResults.DisplayLayout.MaxColScrollRegions = 1
        ugResults.DisplayLayout.MaxRowScrollRegions = 1
        Appearance8.BackColor = SystemColors.Window
        Appearance8.ForeColor = SystemColors.ControlText
        ugResults.DisplayLayout.Override.ActiveCellAppearance = Appearance8
        Appearance9.BackColor = SystemColors.Highlight
        Appearance9.ForeColor = SystemColors.HighlightText
        ugResults.DisplayLayout.Override.ActiveRowAppearance = Appearance9
        ugResults.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted
        ugResults.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted
        Appearance10.BackColor = SystemColors.Window
        ugResults.DisplayLayout.Override.CardAreaAppearance = Appearance10
        Appearance11.BorderColor = Color.Silver
        Appearance11.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter
        ugResults.DisplayLayout.Override.CellAppearance = Appearance11
        ugResults.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText
        ugResults.DisplayLayout.Override.CellPadding = 0
        Appearance12.BackColor = SystemColors.Control
        Appearance12.BackColor2 = SystemColors.ControlDark
        Appearance12.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element
        Appearance12.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance12.BorderColor = SystemColors.Window
        ugResults.DisplayLayout.Override.GroupByRowAppearance = Appearance12
        ugResults.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti
        ugResults.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand
        Appearance13.BackColor = SystemColors.Window
        Appearance13.BorderColor = Color.Silver
        resources.ApplyResources(Appearance13, "Appearance13")
        Appearance13.ForceApplyResources = ""
        ugResults.DisplayLayout.Override.RowAppearance = Appearance13
        ugResults.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False
        Appearance14.BackColor = SystemColors.ControlLight
        ugResults.DisplayLayout.Override.TemplateAddRowAppearance = Appearance14
        ugResults.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill
        ugResults.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate
        ugResults.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy
        resources.ApplyResources(ugResults, "ugResults")
        ugResults.Name = "ugResults"
        ' 
        ' ugbCompareSettings
        ' 
        resources.ApplyResources(ugbCompareSettings, "ugbCompareSettings")
        ugbCompareSettings.Controls.Add(chkConsiderText)
        ugbCompareSettings.Controls.Add(uckConsiderOffset)
        ugbCompareSettings.Controls.Add(uneOffsetVectorY)
        ugbCompareSettings.Controls.Add(lblOffsetVector)
        ugbCompareSettings.Controls.Add(uneOffsetVectorX)
        ugbCompareSettings.Controls.Add(btnOffsetPreview)
        ugbCompareSettings.Controls.Add(uceCompareDrw)
        ugbCompareSettings.Controls.Add(lblCompareDrw)
        ugbCompareSettings.Controls.Add(uceReferenceDrw)
        ugbCompareSettings.Controls.Add(lblReferenceDrw)
        ugbCompareSettings.Controls.Add(uceCompareDocument)
        ugbCompareSettings.Controls.Add(txtReferenceDocument)
        ugbCompareSettings.Controls.Add(lblCompareDocument)
        ugbCompareSettings.Controls.Add(lblReferenceDocument)
        ugbCompareSettings.Name = "ugbCompareSettings"
        ' 
        ' chkConsiderText
        ' 
        resources.ApplyResources(chkConsiderText, "chkConsiderText")
        chkConsiderText.Checked = True
        chkConsiderText.CheckState = CheckState.Checked
        chkConsiderText.Name = "chkConsiderText"
        ' 
        ' uckConsiderOffset
        ' 
        resources.ApplyResources(uckConsiderOffset, "uckConsiderOffset")
        uckConsiderOffset.Name = "uckConsiderOffset"
        ' 
        ' uneOffsetVectorY
        ' 
        Appearance15.BackColor = Color.White
        uneOffsetVectorY.Appearance = Appearance15
        uneOffsetVectorY.BackColor = Color.White
        uneOffsetVectorY.FormatString = "#0.##"
        resources.ApplyResources(uneOffsetVectorY, "uneOffsetVectorY")
        uneOffsetVectorY.MaxValue = 10000R
        uneOffsetVectorY.MinValue = -10000R
        uneOffsetVectorY.Name = "uneOffsetVectorY"
        uneOffsetVectorY.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double
        uneOffsetVectorY.PromptChar = " "c
        uneOffsetVectorY.ReadOnly = True
        ' 
        ' lblOffsetVector
        ' 
        resources.ApplyResources(lblOffsetVector, "lblOffsetVector")
        lblOffsetVector.Name = "lblOffsetVector"
        ' 
        ' uneOffsetVectorX
        ' 
        Appearance16.BackColor = Color.White
        uneOffsetVectorX.Appearance = Appearance16
        uneOffsetVectorX.BackColor = Color.White
        uneOffsetVectorX.FormatString = "#0.##"
        resources.ApplyResources(uneOffsetVectorX, "uneOffsetVectorX")
        uneOffsetVectorX.MaxValue = 10000R
        uneOffsetVectorX.MinValue = -10000R
        uneOffsetVectorX.Name = "uneOffsetVectorX"
        uneOffsetVectorX.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double
        uneOffsetVectorX.PromptChar = " "c
        uneOffsetVectorX.ReadOnly = True
        ' 
        ' btnOffsetPreview
        ' 
        resources.ApplyResources(btnOffsetPreview, "btnOffsetPreview")
        btnOffsetPreview.Name = "btnOffsetPreview"
        ' 
        ' uceCompareDrw
        ' 
        resources.ApplyResources(uceCompareDrw, "uceCompareDrw")
        uceCompareDrw.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList
        uceCompareDrw.Name = "uceCompareDrw"
        ' 
        ' lblCompareDrw
        ' 
        resources.ApplyResources(lblCompareDrw, "lblCompareDrw")
        lblCompareDrw.Name = "lblCompareDrw"
        ' 
        ' uceReferenceDrw
        ' 
        uceReferenceDrw.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList
        resources.ApplyResources(uceReferenceDrw, "uceReferenceDrw")
        uceReferenceDrw.Name = "uceReferenceDrw"
        ' 
        ' lblReferenceDrw
        ' 
        resources.ApplyResources(lblReferenceDrw, "lblReferenceDrw")
        lblReferenceDrw.Name = "lblReferenceDrw"
        ' 
        ' uceCompareDocument
        ' 
        resources.ApplyResources(uceCompareDocument, "uceCompareDocument")
        uceCompareDocument.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList
        uceCompareDocument.Name = "uceCompareDocument"
        ' 
        ' txtReferenceDocument
        ' 
        Appearance17.BackColor = Color.White
        Appearance17.FontData.BoldAsString = resources.GetString("resource.BoldAsString2")
        txtReferenceDocument.Appearance = Appearance17
        txtReferenceDocument.BackColor = Color.White
        resources.ApplyResources(txtReferenceDocument, "txtReferenceDocument")
        txtReferenceDocument.Name = "txtReferenceDocument"
        txtReferenceDocument.ReadOnly = True
        ' 
        ' lblCompareDocument
        ' 
        resources.ApplyResources(lblCompareDocument, "lblCompareDocument")
        lblCompareDocument.Name = "lblCompareDocument"
        ' 
        ' lblReferenceDocument
        ' 
        resources.ApplyResources(lblReferenceDocument, "lblReferenceDocument")
        lblReferenceDocument.Name = "lblReferenceDocument"
        ' 
        ' ugbCompareResults
        ' 
        resources.ApplyResources(ugbCompareResults, "ugbCompareResults")
        ugbCompareResults.Controls.Add(upnResult)
        ugbCompareResults.Name = "ugbCompareResults"
        ' 
        ' btnClose
        ' 
        resources.ApplyResources(btnClose, "btnClose")
        btnClose.Name = "btnClose"
        ' 
        ' btnExport
        ' 
        resources.ApplyResources(btnExport, "btnExport")
        Appearance18.Image = Global.Zuken.E3.HarnessAnalyzer.My.Resources.Resources.ExportCompareResult
        btnExport.Appearance = Appearance18
        btnExport.Name = "btnExport"
        ' 
        ' btnCompare
        ' 
        resources.ApplyResources(btnCompare, "btnCompare")
        btnCompare.Name = "btnCompare"
        ' 
        ' bwCompare
        ' 
        bwCompare.WorkerReportsProgress = True
        bwCompare.WorkerSupportsCancellation = True
        ' 
        ' upbCompare
        ' 
        resources.ApplyResources(upbCompare, "upbCompare")
        upbCompare.Name = "upbCompare"
        ' 
        ' uttmCompare
        ' 
        uttmCompare.ContainingControl = Me
        ' 
        ' lblLegend1
        ' 
        resources.ApplyResources(lblLegend1, "lblLegend1")
        resources.ApplyResources(Appearance21, "Appearance21")
        Appearance21.ForceApplyResources = ""
        lblLegend1.Appearance = Appearance21
        lblLegend1.Name = "lblLegend1"
        ' 
        ' lblLegend2
        ' 
        resources.ApplyResources(lblLegend2, "lblLegend2")
        resources.ApplyResources(Appearance23, "Appearance23")
        Appearance23.ForceApplyResources = ""
        lblLegend2.Appearance = Appearance23
        lblLegend2.Name = "lblLegend2"
        ' 
        ' lblLegend3
        ' 
        resources.ApplyResources(lblLegend3, "lblLegend3")
        Appearance22.ForeColor = Color.DarkOrange
        resources.ApplyResources(Appearance22, "Appearance22")
        Appearance22.ForceApplyResources = ""
        lblLegend3.Appearance = Appearance22
        lblLegend3.Name = "lblLegend3"
        ' 
        ' udsResults
        ' 
        udsResults.AllowAdd = False
        udsResults.AllowDelete = False
        ' 
        ' lblRowCountInfo
        ' 
        resources.ApplyResources(lblRowCountInfo, "lblRowCountInfo")
        resources.ApplyResources(Appearance20, "Appearance20")
        Appearance20.ForceApplyResources = ""
        lblRowCountInfo.Appearance = Appearance20
        lblRowCountInfo.Name = "lblRowCountInfo"
        ' 
        ' ugeeResults
        ' 
        ' 
        ' utmCompare
        ' 
        utmCompare.DesignerFlags = 1
        utmCompare.DockWithinContainer = Me
        utmCompare.DockWithinContainerBaseType = GetType(Form)
        ' 
        ' _GraphicalCompareForm_Toolbars_Dock_Area_Left
        ' 
        _GraphicalCompareForm_Toolbars_Dock_Area_Left.AccessibleRole = AccessibleRole.Grouping
        _GraphicalCompareForm_Toolbars_Dock_Area_Left.BackColor = SystemColors.Control
        _GraphicalCompareForm_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left
        _GraphicalCompareForm_Toolbars_Dock_Area_Left.ForeColor = SystemColors.ControlText
        resources.ApplyResources(_GraphicalCompareForm_Toolbars_Dock_Area_Left, "_GraphicalCompareForm_Toolbars_Dock_Area_Left")
        _GraphicalCompareForm_Toolbars_Dock_Area_Left.Name = "_GraphicalCompareForm_Toolbars_Dock_Area_Left"
        _GraphicalCompareForm_Toolbars_Dock_Area_Left.ToolbarsManager = utmCompare
        ' 
        ' _GraphicalCompareForm_Toolbars_Dock_Area_Right
        ' 
        _GraphicalCompareForm_Toolbars_Dock_Area_Right.AccessibleRole = AccessibleRole.Grouping
        _GraphicalCompareForm_Toolbars_Dock_Area_Right.BackColor = SystemColors.Control
        _GraphicalCompareForm_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right
        _GraphicalCompareForm_Toolbars_Dock_Area_Right.ForeColor = SystemColors.ControlText
        resources.ApplyResources(_GraphicalCompareForm_Toolbars_Dock_Area_Right, "_GraphicalCompareForm_Toolbars_Dock_Area_Right")
        _GraphicalCompareForm_Toolbars_Dock_Area_Right.Name = "_GraphicalCompareForm_Toolbars_Dock_Area_Right"
        _GraphicalCompareForm_Toolbars_Dock_Area_Right.ToolbarsManager = utmCompare
        ' 
        ' _GraphicalCompareForm_Toolbars_Dock_Area_Top
        ' 
        _GraphicalCompareForm_Toolbars_Dock_Area_Top.AccessibleRole = AccessibleRole.Grouping
        _GraphicalCompareForm_Toolbars_Dock_Area_Top.BackColor = SystemColors.Control
        _GraphicalCompareForm_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top
        _GraphicalCompareForm_Toolbars_Dock_Area_Top.ForeColor = SystemColors.ControlText
        resources.ApplyResources(_GraphicalCompareForm_Toolbars_Dock_Area_Top, "_GraphicalCompareForm_Toolbars_Dock_Area_Top")
        _GraphicalCompareForm_Toolbars_Dock_Area_Top.Name = "_GraphicalCompareForm_Toolbars_Dock_Area_Top"
        _GraphicalCompareForm_Toolbars_Dock_Area_Top.ToolbarsManager = utmCompare
        ' 
        ' _GraphicalCompareForm_Toolbars_Dock_Area_Bottom
        ' 
        _GraphicalCompareForm_Toolbars_Dock_Area_Bottom.AccessibleRole = AccessibleRole.Grouping
        _GraphicalCompareForm_Toolbars_Dock_Area_Bottom.BackColor = SystemColors.Control
        _GraphicalCompareForm_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom
        _GraphicalCompareForm_Toolbars_Dock_Area_Bottom.ForeColor = SystemColors.ControlText
        resources.ApplyResources(_GraphicalCompareForm_Toolbars_Dock_Area_Bottom, "_GraphicalCompareForm_Toolbars_Dock_Area_Bottom")
        _GraphicalCompareForm_Toolbars_Dock_Area_Bottom.Name = "_GraphicalCompareForm_Toolbars_Dock_Area_Bottom"
        _GraphicalCompareForm_Toolbars_Dock_Area_Bottom.ToolbarsManager = utmCompare
        ' 
        ' btnResetAreaFilter
        ' 
        resources.ApplyResources(btnResetAreaFilter, "btnResetAreaFilter")
        btnResetAreaFilter.Name = "btnResetAreaFilter"
        ' 
        ' uddbSetAreaFilter
        ' 
        resources.ApplyResources(uddbSetAreaFilter, "uddbSetAreaFilter")
        uddbSetAreaFilter.Name = "uddbSetAreaFilter"
        ' 
        ' uckDisplayIndicators
        ' 
        resources.ApplyResources(uckDisplayIndicators, "uckDisplayIndicators")
        uckDisplayIndicators.Name = "uckDisplayIndicators"
        ' 
        ' uckFilterDynamically
        ' 
        resources.ApplyResources(uckFilterDynamically, "uckFilterDynamically")
        uckFilterDynamically.Name = "uckFilterDynamically"
        ' 
        ' btnSave
        ' 
        resources.ApplyResources(btnSave, "btnSave")
        Appearance19.Image = Global.Zuken.E3.HarnessAnalyzer.My.Resources.Resources.SaveCompareResult
        btnSave.Appearance = Appearance19
        btnSave.Name = "btnSave"
        ' 
        ' ProgressBar1
        ' 
        resources.ApplyResources(ProgressBar1, "ProgressBar1")
        ProgressBar1.MarqueeAnimationSpeed = 20
        ProgressBar1.Name = "ProgressBar1"
        ProgressBar1.Style = ProgressBarStyle.Marquee
        ' 
        ' GraphicalCompareForm
        ' 
        AutoScaleMode = AutoScaleMode.Inherit
        resources.ApplyResources(Me, "$this")
        Controls.Add(btnSave)
        Controls.Add(ProgressBar1)
        Controls.Add(uckFilterDynamically)
        Controls.Add(uckDisplayIndicators)
        Controls.Add(uddbSetAreaFilter)
        Controls.Add(btnResetAreaFilter)
        Controls.Add(lblRowCountInfo)
        Controls.Add(lblLegend1)
        Controls.Add(btnCompare)
        Controls.Add(lblLegend3)
        Controls.Add(btnExport)
        Controls.Add(lblLegend2)
        Controls.Add(btnClose)
        Controls.Add(ugbCompareResults)
        Controls.Add(ugbCompareSettings)
        Controls.Add(upbCompare)
        Controls.Add(_GraphicalCompareForm_Toolbars_Dock_Area_Left)
        Controls.Add(_GraphicalCompareForm_Toolbars_Dock_Area_Right)
        Controls.Add(_GraphicalCompareForm_Toolbars_Dock_Area_Bottom)
        Controls.Add(_GraphicalCompareForm_Toolbars_Dock_Area_Top)
        KeyPreview = True
        Name = "GraphicalCompareForm"
        upnResult.ClientArea.ResumeLayout(False)
        upnResult.ResumeLayout(False)
        upnEntry.ClientArea.ResumeLayout(False)
        upnEntry.ClientArea.PerformLayout()
        upnEntry.ResumeLayout(False)
        upnGraphic.ClientArea.ResumeLayout(False)
        upnGraphic.ResumeLayout(False)
        CType(picCompResult, ComponentModel.ISupportInitialize).EndInit()
        CType(picRefResult, ComponentModel.ISupportInitialize).EndInit()
        CType(txtComment, ComponentModel.ISupportInitialize).EndInit()
        CType(ugResults, ComponentModel.ISupportInitialize).EndInit()
        CType(ugbCompareSettings, ComponentModel.ISupportInitialize).EndInit()
        ugbCompareSettings.ResumeLayout(False)
        ugbCompareSettings.PerformLayout()
        CType(chkConsiderText, ComponentModel.ISupportInitialize).EndInit()
        CType(uckConsiderOffset, ComponentModel.ISupportInitialize).EndInit()
        CType(uneOffsetVectorY, ComponentModel.ISupportInitialize).EndInit()
        CType(uneOffsetVectorX, ComponentModel.ISupportInitialize).EndInit()
        CType(uceCompareDrw, ComponentModel.ISupportInitialize).EndInit()
        CType(uceReferenceDrw, ComponentModel.ISupportInitialize).EndInit()
        CType(uceCompareDocument, ComponentModel.ISupportInitialize).EndInit()
        CType(txtReferenceDocument, ComponentModel.ISupportInitialize).EndInit()
        CType(ugbCompareResults, ComponentModel.ISupportInitialize).EndInit()
        ugbCompareResults.ResumeLayout(False)
        CType(udsResults, ComponentModel.ISupportInitialize).EndInit()
        CType(utmCompare, ComponentModel.ISupportInitialize).EndInit()
        CType(uckDisplayIndicators, ComponentModel.ISupportInitialize).EndInit()
        CType(uckFilterDynamically, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)

    End Sub
    Friend WithEvents bwCompare As System.ComponentModel.BackgroundWorker
    Private WithEvents ugbCompareSettings As Infragistics.Win.Misc.UltraGroupBox
    Private WithEvents uceCompareDocument As Infragistics.Win.UltraWinEditors.UltraComboEditor
    Private WithEvents txtReferenceDocument As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Private WithEvents lblCompareDocument As Infragistics.Win.Misc.UltraLabel
    Private WithEvents lblReferenceDocument As Infragistics.Win.Misc.UltraLabel
    Private WithEvents ugbCompareResults As Infragistics.Win.Misc.UltraGroupBox
    Private WithEvents btnClose As Infragistics.Win.Misc.UltraButton
    Private WithEvents btnExport As Infragistics.Win.Misc.UltraButton
    Private WithEvents btnCompare As Infragistics.Win.Misc.UltraButton
    Private WithEvents upbCompare As Infragistics.Win.UltraWinProgressBar.UltraProgressBar
    Friend WithEvents uttmCompare As Infragistics.Win.UltraWinToolTip.UltraToolTipManager
    Friend WithEvents lblLegend1 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents lblLegend3 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents lblLegend2 As Infragistics.Win.Misc.UltraLabel
    Private WithEvents uceCompareDrw As Infragistics.Win.UltraWinEditors.UltraComboEditor
    Private WithEvents lblCompareDrw As Infragistics.Win.Misc.UltraLabel
    Private WithEvents uceReferenceDrw As Infragistics.Win.UltraWinEditors.UltraComboEditor
    Private WithEvents lblReferenceDrw As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents udsResults As Infragistics.Win.UltraWinDataSource.UltraDataSource
    Friend WithEvents upnResult As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents uspResult As Infragistics.Win.Misc.UltraSplitter
    Friend WithEvents ugResults As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents upnEntry As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents lblRowCountInfo As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents ugeeResults As Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter
    Friend WithEvents _GraphicalCompareForm_Toolbars_Dock_Area_Left As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents utmCompare As Infragistics.Win.UltraWinToolbars.UltraToolbarsManager
    Friend WithEvents _GraphicalCompareForm_Toolbars_Dock_Area_Right As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents _GraphicalCompareForm_Toolbars_Dock_Area_Bottom As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents _GraphicalCompareForm_Toolbars_Dock_Area_Top As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents btnNext As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnPrevious As Infragistics.Win.Misc.UltraButton
    Friend WithEvents txtComment As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents upnGraphic As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents picCompResult As System.Windows.Forms.PictureBox
    Friend WithEvents picRefResult As System.Windows.Forms.PictureBox
    Friend WithEvents lblComment As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents btnResetAreaFilter As Infragistics.Win.Misc.UltraButton
    Friend WithEvents uddbSetAreaFilter As Infragistics.Win.Misc.UltraDropDownButton
    Friend WithEvents lblCompare As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents lblReference As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents uckDisplayIndicators As Infragistics.Win.UltraWinEditors.UltraCheckEditor
    Friend WithEvents btnSync As Infragistics.Win.Misc.UltraButton
    Friend WithEvents uckFilterDynamically As Infragistics.Win.UltraWinEditors.UltraCheckEditor
    Friend WithEvents btnSave As Infragistics.Win.Misc.UltraButton
    Friend WithEvents lblNoGraphicsToDisplay As Infragistics.Win.Misc.UltraLabel
    Private WithEvents lblOffsetVector As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents uneOffsetVectorX As Infragistics.Win.UltraWinEditors.UltraNumericEditor
    Friend WithEvents btnOffsetPreview As Infragistics.Win.Misc.UltraButton
    Friend WithEvents uneOffsetVectorY As Infragistics.Win.UltraWinEditors.UltraNumericEditor
    Friend WithEvents uckConsiderOffset As Infragistics.Win.UltraWinEditors.UltraCheckEditor
    Friend WithEvents chkConsiderText As Infragistics.Win.UltraWinEditors.UltraCheckEditor
    Friend WithEvents ProgressBar1 As ProgressBar
End Class
