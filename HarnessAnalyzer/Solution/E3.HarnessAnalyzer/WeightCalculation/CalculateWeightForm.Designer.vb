Imports Zuken.E3.App.Windows.Controls.Infragistics.WinGrid

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class CalculateWeightForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(CalculateWeightForm))
        Dim UltraToolTipInfo1 As Infragistics.Win.UltraWinToolTip.UltraToolTipInfo = New Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Edit, to add length delta to all wires/cores", Infragistics.Win.ToolTipImage.[Default], Nothing, Infragistics.Win.DefaultableBoolean.[Default])
        Dim Appearance1 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim UltraGridBand1 As Infragistics.Win.UltraWinGrid.UltraGridBand = New Infragistics.Win.UltraWinGrid.UltraGridBand("CalculatedWeightRow", -1)
        Dim UltraGridColumn33 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("Cores")
        Dim UltraGridColumn34 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("Id", -1, Nothing, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Descending, False)
        Dim Appearance2 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim UltraGridColumn35 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("SourceString")
        Dim Appearance3 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim UltraGridColumn36 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("CsaSqMm")
        Dim Appearance4 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim UltraGridColumn37 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("Conductor")
        Dim Appearance5 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim UltraGridColumn38 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("Total")
        Dim Appearance6 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim UltraGridColumn1 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("ResultingLengthValue")
        Dim Appearance7 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim UltraGridColumn39 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("LengthValue")
        Dim Appearance8 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim UltraGridColumn40 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("KblMass")
        Dim Appearance9 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim SummarySettings1 As Infragistics.Win.UltraWinGrid.SummarySettings = New Infragistics.Win.UltraWinGrid.SummarySettings("", Infragistics.Win.UltraWinGrid.SummaryType.Sum, Nothing, "KblMass", 8, True, "CalculatedWeightRow", 0, Infragistics.Win.UltraWinGrid.SummaryPosition.UseSummaryPositionColumn, "KblMass", 8, True)
        Dim Appearance10 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim SummarySettings2 As Infragistics.Win.UltraWinGrid.SummarySettings = New Infragistics.Win.UltraWinGrid.SummarySettings("", Infragistics.Win.UltraWinGrid.SummaryType.Sum, Nothing, "Total", 5, True, "CalculatedWeightRow", 0, Infragistics.Win.UltraWinGrid.SummaryPosition.UseSummaryPositionColumn, "Total", 5, True)
        Dim SummarySettings3 As Infragistics.Win.UltraWinGrid.SummarySettings = New Infragistics.Win.UltraWinGrid.SummarySettings("", Infragistics.Win.UltraWinGrid.SummaryType.Sum, Nothing, "LengthValue", 7, True, "CalculatedWeightRow", 0, Infragistics.Win.UltraWinGrid.SummaryPosition.UseSummaryPositionColumn, "LengthValue", 7, True)
        Dim SummarySettings4 As Infragistics.Win.UltraWinGrid.SummarySettings = New Infragistics.Win.UltraWinGrid.SummarySettings("", Infragistics.Win.UltraWinGrid.SummaryType.Sum, Nothing, "Conductor", 4, True, "CalculatedWeightRow", 0, Infragistics.Win.UltraWinGrid.SummaryPosition.UseSummaryPositionColumn, "Conductor", 4, True)
        Dim UltraGridBand2 As Infragistics.Win.UltraWinGrid.UltraGridBand = New Infragistics.Win.UltraWinGrid.UltraGridBand("Cores", 0)
        Dim UltraGridColumn41 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("Cores")
        Dim UltraGridColumn42 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("Id")
        Dim Appearance11 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim UltraGridColumn43 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("SourceString")
        Dim Appearance12 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim UltraGridColumn44 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("CsaSqMm")
        Dim Appearance13 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim UltraGridColumn45 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("Conductor")
        Dim Appearance14 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim UltraGridColumn46 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("Total")
        Dim Appearance15 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim UltraGridColumn2 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("ResultingLengthValue")
        Dim UltraGridColumn47 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("LengthValue")
        Dim Appearance16 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim UltraGridColumn48 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("KblMass")
        Dim Appearance17 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance18 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance19 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.CopyToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BindingSource1 = New System.Windows.Forms.BindingSource(Me.components)
        Me.btnClose = New Infragistics.Win.Misc.UltraButton()
        Me.uneDeltaLength = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
        Me.Label1 = New Infragistics.Win.Misc.UltraLabel()
        Me.UltraToolTipManager1 = New Infragistics.Win.UltraWinToolTip.UltraToolTipManager(Me.components)
        Me.UltraValidator1 = New Infragistics.Win.Misc.UltraValidator(Me.components)
        Me.ugCalculatedWeights = New App.Windows.Controls.Infragistics.WinGrid.WinGridExtended()
        Me.CalculatedWeightRowBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.UltraProgressBar1 = New Infragistics.Win.UltraWinProgressBar.UltraProgressBar()
        Me.ContextMenuStrip1.SuspendLayout()
        CType(Me.BindingSource1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.uneDeltaLength, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.UltraValidator1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ugCalculatedWeights, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CalculatedWeightRowBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CopyToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        resources.ApplyResources(Me.ContextMenuStrip1, "ContextMenuStrip1")
        '
        'CopyToolStripMenuItem
        '
        Me.CopyToolStripMenuItem.Name = "CopyToolStripMenuItem"
        resources.ApplyResources(Me.CopyToolStripMenuItem, "CopyToolStripMenuItem")
        '
        'btnClose
        '
        resources.ApplyResources(Me.btnClose, "btnClose")
        Me.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.btnClose.Name = "btnClose"
        '
        'uneDeltaLength
        '
        resources.ApplyResources(Me.uneDeltaLength, "uneDeltaLength")
        Me.uneDeltaLength.MaskInput = "{LOC}nnnnn \mm"
        Me.uneDeltaLength.MinValue = -2147483647.0R
        Me.uneDeltaLength.Name = "uneDeltaLength"
        Me.uneDeltaLength.Nullable = True
        Me.uneDeltaLength.NumericType = Infragistics.Win.UltraWinEditors.NumericType.[Double]
        Me.uneDeltaLength.PromptChar = Global.Microsoft.VisualBasic.ChrW(32)
        resources.ApplyResources(UltraToolTipInfo1, "UltraToolTipInfo1")
        Me.UltraToolTipManager1.SetUltraToolTip(Me.uneDeltaLength, UltraToolTipInfo1)
        Me.UltraValidator1.GetValidationSettings(Me.uneDeltaLength).Condition = New Infragistics.Win.RangeCondition(-2147483647.0R, 2147483647.0R, Nothing)
        Me.UltraValidator1.GetValidationSettings(Me.uneDeltaLength).DataType = GetType(Double)
        Me.UltraValidator1.GetValidationSettings(Me.uneDeltaLength).ValidationPropertyName = "Value"
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        resources.ApplyResources(Appearance1, "Appearance1")
        Me.Label1.Appearance = Appearance1
        Me.Label1.Name = "Label1"
        '
        'UltraToolTipManager1
        '
        Me.UltraToolTipManager1.ContainingControl = Me
        Me.UltraToolTipManager1.DisplayStyle = Infragistics.Win.ToolTipDisplayStyle.Standard
        '
        'UltraValidator1
        '
        Me.UltraValidator1.NotificationSettings.Action = Infragistics.Win.Misc.NotificationAction.BalloonTip
        Me.UltraValidator1.ValidationTrigger = Infragistics.Win.Misc.ValidationTrigger.Programmatic
        '
        'ugCalculatedWeights
        '
        resources.ApplyResources(Me.ugCalculatedWeights, "ugCalculatedWeights")
        Me.ugCalculatedWeights.ContextMenuStrip = Me.ContextMenuStrip1
        Me.ugCalculatedWeights.DataSource = Me.CalculatedWeightRowBindingSource
        Me.ugCalculatedWeights.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns
        UltraGridColumn33.Header.Editor = Nothing
        UltraGridColumn33.Header.VisiblePosition = 8
        UltraGridColumn34.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.AllRowsInBand
        UltraGridColumn34.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly
        resources.ApplyResources(Appearance2, "Appearance2")
        UltraGridColumn34.CellAppearance = Appearance2
        UltraGridColumn34.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.CellSelect
        resources.ApplyResources(UltraGridColumn34.Header, "UltraGridColumn34.Header")
        UltraGridColumn34.Header.Editor = Nothing
        UltraGridColumn34.Header.VisiblePosition = 0
        resources.ApplyResources(UltraGridColumn34, "UltraGridColumn34")
        UltraGridColumn34.Nullable = Infragistics.Win.UltraWinGrid.Nullable.[Nothing]
        UltraGridColumn34.PromptChar = Global.Microsoft.VisualBasic.ChrW(32)
        UltraGridColumn34.Width = 72
        UltraGridColumn34.ForceApplyResources = "Header|"
        UltraGridColumn35.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.AllRowsInBand
        UltraGridColumn35.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly
        resources.ApplyResources(Appearance3, "Appearance3")
        UltraGridColumn35.CellAppearance = Appearance3
        UltraGridColumn35.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.CellSelect
        resources.ApplyResources(UltraGridColumn35.Header, "UltraGridColumn35.Header")
        UltraGridColumn35.Header.Editor = Nothing
        UltraGridColumn35.Header.VisiblePosition = 1
        resources.ApplyResources(UltraGridColumn35, "UltraGridColumn35")
        UltraGridColumn35.PromptChar = Global.Microsoft.VisualBasic.ChrW(32)
        UltraGridColumn35.Width = 113
        UltraGridColumn35.ForceApplyResources = "Header|"
        resources.ApplyResources(Appearance4, "Appearance4")
        UltraGridColumn36.CellAppearance = Appearance4
        resources.ApplyResources(UltraGridColumn36.Header, "UltraGridColumn36.Header")
        UltraGridColumn36.Header.Editor = Nothing
        UltraGridColumn36.Header.VisiblePosition = 2
        UltraGridColumn36.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals
        resources.ApplyResources(UltraGridColumn36, "UltraGridColumn36")
        UltraGridColumn36.Nullable = Infragistics.Win.UltraWinGrid.Nullable.[Nothing]
        UltraGridColumn36.PromptChar = Global.Microsoft.VisualBasic.ChrW(32)
        UltraGridColumn36.Width = 76
        UltraGridColumn36.ForceApplyResources = "Header|"
        resources.ApplyResources(Appearance5, "Appearance5")
        UltraGridColumn37.CellAppearance = Appearance5
        UltraGridColumn37.Header.Editor = Nothing
        UltraGridColumn37.Header.VisiblePosition = 4
        UltraGridColumn37.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals
        resources.ApplyResources(UltraGridColumn37, "UltraGridColumn37")
        UltraGridColumn37.PromptChar = Global.Microsoft.VisualBasic.ChrW(32)
        UltraGridColumn37.Width = 68
        UltraGridColumn37.ForceApplyResources = ""
        resources.ApplyResources(Appearance6, "Appearance6")
        UltraGridColumn38.CellAppearance = Appearance6
        UltraGridColumn38.Header.Editor = Nothing
        UltraGridColumn38.Header.VisiblePosition = 7
        UltraGridColumn38.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals
        resources.ApplyResources(UltraGridColumn38, "UltraGridColumn38")
        UltraGridColumn38.MinWidth = 15
        UltraGridColumn38.PromptChar = Global.Microsoft.VisualBasic.ChrW(32)
        UltraGridColumn38.Width = 68
        UltraGridColumn38.ForceApplyResources = ""
        Appearance7.ImageHAlign = Infragistics.Win.HAlign.Center
        resources.ApplyResources(Appearance7, "Appearance7")
        UltraGridColumn1.CellAppearance = Appearance7
        resources.ApplyResources(UltraGridColumn1.Header, "UltraGridColumn1.Header")
        UltraGridColumn1.Header.Editor = Nothing
        UltraGridColumn1.Header.VisiblePosition = 6
        UltraGridColumn1.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals
        resources.ApplyResources(UltraGridColumn1, "UltraGridColumn1")
        UltraGridColumn1.Nullable = Infragistics.Win.UltraWinGrid.Nullable.[Nothing]
        UltraGridColumn1.PromptChar = Global.Microsoft.VisualBasic.ChrW(32)
        UltraGridColumn1.Width = 107
        UltraGridColumn1.ForceApplyResources = "Header|"
        Appearance8.ImageHAlign = Infragistics.Win.HAlign.Center
        resources.ApplyResources(Appearance8, "Appearance8")
        UltraGridColumn39.CellAppearance = Appearance8
        UltraGridColumn39.DefaultCellValue = "0"
        resources.ApplyResources(UltraGridColumn39.Header, "UltraGridColumn39.Header")
        UltraGridColumn39.Header.Editor = Nothing
        UltraGridColumn39.Header.VisiblePosition = 5
        UltraGridColumn39.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals
        resources.ApplyResources(UltraGridColumn39, "UltraGridColumn39")
        UltraGridColumn39.Nullable = Infragistics.Win.UltraWinGrid.Nullable.[Nothing]
        UltraGridColumn39.PromptChar = Global.Microsoft.VisualBasic.ChrW(32)
        UltraGridColumn39.Width = 80
        UltraGridColumn39.ForceApplyResources = "Header|"
        UltraGridColumn40.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.AllRowsInBand
        UltraGridColumn40.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly
        resources.ApplyResources(Appearance9, "Appearance9")
        UltraGridColumn40.CellAppearance = Appearance9
        UltraGridColumn40.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.CellSelect
        UltraGridColumn40.Header.Editor = Nothing
        UltraGridColumn40.Header.VisiblePosition = 3
        UltraGridColumn40.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals
        resources.ApplyResources(UltraGridColumn40, "UltraGridColumn40")
        UltraGridColumn40.PromptChar = Global.Microsoft.VisualBasic.ChrW(32)
        UltraGridColumn40.Width = 71
        UltraGridColumn40.ForceApplyResources = ""
        UltraGridBand1.Columns.AddRange(New Object() {UltraGridColumn33, UltraGridColumn34, UltraGridColumn35, UltraGridColumn36, UltraGridColumn37, UltraGridColumn38, UltraGridColumn1, UltraGridColumn39, UltraGridColumn40})
        resources.ApplyResources(Appearance10, "Appearance10")
        SummarySettings1.Appearance = Appearance10
        SummarySettings1.DisplayFormat = "{0:###0.00} g"
        SummarySettings1.SummaryDisplayArea = Infragistics.Win.UltraWinGrid.SummaryDisplayAreas.BottomFixed
        SummarySettings2.DisplayFormat = "{0:###0.00} g"
        SummarySettings2.SummaryDisplayArea = Infragistics.Win.UltraWinGrid.SummaryDisplayAreas.BottomFixed
        SummarySettings3.DisplayFormat = "{0:###0.00} mm"
        SummarySettings3.SummaryDisplayArea = Infragistics.Win.UltraWinGrid.SummaryDisplayAreas.BottomFixed
        SummarySettings4.DisplayFormat = "{0:###0.00} g"
        SummarySettings4.SummaryDisplayArea = Infragistics.Win.UltraWinGrid.SummaryDisplayAreas.BottomFixed
        UltraGridBand1.Summaries.AddRange(New Infragistics.Win.UltraWinGrid.SummarySettings() {SummarySettings1, SummarySettings2, SummarySettings3, SummarySettings4})
        resources.ApplyResources(UltraGridBand1, "UltraGridBand1")
        UltraGridColumn41.Header.Editor = Nothing
        UltraGridColumn41.Header.VisiblePosition = 8
        resources.ApplyResources(Appearance11, "Appearance11")
        UltraGridColumn42.CellAppearance = Appearance11
        resources.ApplyResources(UltraGridColumn42.Header, "UltraGridColumn42.Header")
        UltraGridColumn42.Header.Editor = Nothing
        UltraGridColumn42.Header.VisiblePosition = 0
        UltraGridColumn42.Width = 86
        UltraGridColumn42.ForceApplyResources = "Header"
        resources.ApplyResources(Appearance12, "Appearance12")
        UltraGridColumn43.CellAppearance = Appearance12
        resources.ApplyResources(UltraGridColumn43.Header, "UltraGridColumn43.Header")
        UltraGridColumn43.Header.Editor = Nothing
        UltraGridColumn43.Header.VisiblePosition = 1
        UltraGridColumn43.PromptChar = Global.Microsoft.VisualBasic.ChrW(32)
        UltraGridColumn43.Width = 80
        UltraGridColumn43.ForceApplyResources = "Header"
        resources.ApplyResources(Appearance13, "Appearance13")
        UltraGridColumn44.CellAppearance = Appearance13
        resources.ApplyResources(UltraGridColumn44.Header, "UltraGridColumn44.Header")
        UltraGridColumn44.Header.Editor = Nothing
        UltraGridColumn44.Header.VisiblePosition = 2
        UltraGridColumn44.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals
        resources.ApplyResources(UltraGridColumn44, "UltraGridColumn44")
        UltraGridColumn44.Nullable = Infragistics.Win.UltraWinGrid.Nullable.[Nothing]
        UltraGridColumn44.PromptChar = Global.Microsoft.VisualBasic.ChrW(32)
        UltraGridColumn44.Width = 71
        UltraGridColumn44.ForceApplyResources = "Header|"
        resources.ApplyResources(Appearance14, "Appearance14")
        UltraGridColumn45.CellAppearance = Appearance14
        UltraGridColumn45.Header.Editor = Nothing
        UltraGridColumn45.Header.VisiblePosition = 4
        UltraGridColumn45.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals
        resources.ApplyResources(UltraGridColumn45, "UltraGridColumn45")
        UltraGridColumn45.PromptChar = Global.Microsoft.VisualBasic.ChrW(32)
        UltraGridColumn45.Width = 76
        UltraGridColumn45.ForceApplyResources = ""
        resources.ApplyResources(Appearance15, "Appearance15")
        UltraGridColumn46.CellAppearance = Appearance15
        UltraGridColumn46.Header.Editor = Nothing
        UltraGridColumn46.Header.VisiblePosition = 7
        UltraGridColumn46.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals
        resources.ApplyResources(UltraGridColumn46, "UltraGridColumn46")
        UltraGridColumn46.PromptChar = Global.Microsoft.VisualBasic.ChrW(32)
        UltraGridColumn46.Width = 75
        UltraGridColumn46.ForceApplyResources = ""
        resources.ApplyResources(UltraGridColumn2.Header, "UltraGridColumn2.Header")
        UltraGridColumn2.Header.Editor = Nothing
        UltraGridColumn2.Header.VisiblePosition = 6
        UltraGridColumn2.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals
        resources.ApplyResources(UltraGridColumn2, "UltraGridColumn2")
        UltraGridColumn2.Nullable = Infragistics.Win.UltraWinGrid.Nullable.[Nothing]
        UltraGridColumn2.PromptChar = Global.Microsoft.VisualBasic.ChrW(32)
        UltraGridColumn2.Width = 106
        UltraGridColumn2.ForceApplyResources = "Header|"
        resources.ApplyResources(Appearance16, "Appearance16")
        UltraGridColumn47.CellAppearance = Appearance16
        resources.ApplyResources(UltraGridColumn47.Header, "UltraGridColumn47.Header")
        UltraGridColumn47.Header.Editor = Nothing
        UltraGridColumn47.Header.VisiblePosition = 5
        UltraGridColumn47.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals
        resources.ApplyResources(UltraGridColumn47, "UltraGridColumn47")
        UltraGridColumn47.Nullable = Infragistics.Win.UltraWinGrid.Nullable.[Nothing]
        UltraGridColumn47.PromptChar = Global.Microsoft.VisualBasic.ChrW(32)
        UltraGridColumn47.Width = 68
        UltraGridColumn47.ForceApplyResources = "Header|"
        resources.ApplyResources(Appearance17, "Appearance17")
        UltraGridColumn48.CellAppearance = Appearance17
        UltraGridColumn48.Header.Editor = Nothing
        UltraGridColumn48.Header.VisiblePosition = 3
        UltraGridColumn48.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals
        resources.ApplyResources(UltraGridColumn48, "UltraGridColumn48")
        UltraGridColumn48.PromptChar = Global.Microsoft.VisualBasic.ChrW(32)
        UltraGridColumn48.Width = 74
        UltraGridColumn48.ForceApplyResources = ""
        UltraGridBand2.Columns.AddRange(New Object() {UltraGridColumn41, UltraGridColumn42, UltraGridColumn43, UltraGridColumn44, UltraGridColumn45, UltraGridColumn46, UltraGridColumn2, UltraGridColumn47, UltraGridColumn48})
        Me.ugCalculatedWeights.DisplayLayout.BandsSerializer.Add(UltraGridBand1)
        Me.ugCalculatedWeights.DisplayLayout.BandsSerializer.Add(UltraGridBand2)
        Me.ugCalculatedWeights.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.[False]
        Me.ugCalculatedWeights.DisplayLayout.MaxBandDepth = 2
        Me.ugCalculatedWeights.DisplayLayout.MaxColScrollRegions = 1
        Me.ugCalculatedWeights.DisplayLayout.MaxRowScrollRegions = 1
        Me.ugCalculatedWeights.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.[False]
        Me.ugCalculatedWeights.DisplayLayout.Override.AllowMultiCellOperations = CType((Infragistics.Win.UltraWinGrid.AllowMultiCellOperation.Copy Or Infragistics.Win.UltraWinGrid.AllowMultiCellOperation.CopyWithHeaders), Infragistics.Win.UltraWinGrid.AllowMultiCellOperation)
        Me.ugCalculatedWeights.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.CellSelect
        Me.ugCalculatedWeights.DisplayLayout.Override.ExpansionIndicator = Infragistics.Win.UltraWinGrid.ShowExpansionIndicator.CheckOnDisplay
        Me.ugCalculatedWeights.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti
        Appearance18.FontData.BoldAsString = resources.GetString("resource.BoldAsString")
        resources.ApplyResources(Appearance18, "Appearance18")
        Me.ugCalculatedWeights.DisplayLayout.Override.SummaryFooterCaptionAppearance = Appearance18
        Appearance19.FontData.BoldAsString = resources.GetString("resource.BoldAsString1")
        Me.ugCalculatedWeights.DisplayLayout.Override.SummaryValueAppearance = Appearance19
        Me.ugCalculatedWeights.Name = "ugCalculatedWeights"
        '
        'CalculatedWeightRowBindingSource
        '
        Me.CalculatedWeightRowBindingSource.DataSource = GetType(Zuken.E3.HarnessAnalyzer.CalculatedWeightRow)
        '
        'UltraProgressBar1
        '
        resources.ApplyResources(Me.UltraProgressBar1, "UltraProgressBar1")
        Me.UltraProgressBar1.Name = "UltraProgressBar1"
        '
        'CalculateWeightForm
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.CancelButton = Me.btnClose
        resources.ApplyResources(Me, "$this")
        Me.Controls.Add(Me.UltraProgressBar1)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.uneDeltaLength)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.ugCalculatedWeights)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
        Me.Name = "CalculateWeightForm"
        Me.ShowIcon = False
        Me.ContextMenuStrip1.ResumeLayout(False)
        CType(Me.BindingSource1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.uneDeltaLength, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.UltraValidator1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ugCalculatedWeights, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CalculatedWeightRowBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ugCalculatedWeights As App.Windows.Controls.Infragistics.WinGrid.WinGridExtended
    Friend WithEvents BindingSource1 As System.Windows.Forms.BindingSource
    Friend WithEvents CalculatedWeightRowBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents btnClose As Infragistics.Win.Misc.UltraButton
    Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents CopyToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents uneDeltaLength As Infragistics.Win.UltraWinEditors.UltraNumericEditor
    Friend WithEvents Label1 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents UltraToolTipManager1 As Infragistics.Win.UltraWinToolTip.UltraToolTipManager
    Friend WithEvents UltraValidator1 As Infragistics.Win.Misc.UltraValidator
    Friend WithEvents UltraProgressBar1 As Infragistics.Win.UltraWinProgressBar.UltraProgressBar
End Class
