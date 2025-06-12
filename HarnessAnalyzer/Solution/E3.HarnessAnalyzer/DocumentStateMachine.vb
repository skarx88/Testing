Imports System.ComponentModel
Imports System.IO
Imports System.IO.Compression
Imports System.Text.RegularExpressions
Imports Infragistics.Documents.Excel
Imports Infragistics.Win.UltraWinDock
Imports Infragistics.Win.UltraWinListView
Imports Infragistics.Win.UltraWinTabControl
Imports Infragistics.Win.UltraWinToolbars
Imports Zuken.E3.HarnessAnalyzer.QualityStamping
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.HarnessAnalyzer.Shared.Common
Imports Zuken.E3.Lib.Comparer.Topology.Designs
Imports Zuken.E3.Lib.Comparer.Topology.Documents
Imports Zuken.E3.Lib.Eyeshot.Model
Imports Zuken.E3.Lib.IO
Imports Zuken.E3.Lib.IO.Files.Hcv
Imports Zuken.E3.Lib.IO.KBL
Imports Zuken.E3.Lib.Packager.Circle

Public Class DocumentStateMachine

    Friend Event LogMessage(sender As DocumentForm, e As LogEventArgs)

    Private _logEventArgs As LogEventArgs

    Private WithEvents _activeDrawingCanvas As DrawingCanvas
    Private WithEvents _documentForm As DocumentForm
    Private WithEvents _editIssuesForm As IssueReporting.EditIssuesForm
    Private WithEvents _moduleConfigForm As ModuleConfigForm
    Private WithEvents _udmDocument As UltraDockManager
    Private WithEvents _utmDocument As UltraToolbarsManager
    Private WithEvents _topologyCompareForm As TopologyCompareForm

    Friend WithEvents _ultrasonicSpliceTerminalDistanceForm As UltrasonicSpliceTerminalDistanceForm
    Friend WithEvents _bomForm As BOMForm
    Friend WithEvents _validityCheckForm As ValidityCheckForm

    Private _semShowAndStartCompare As New Threading.SemaphoreSlim(1)
    Private _isAnalysisActive As Boolean
    Private _indicatorsListView As UltraListView

    Private Enum FileNameType
        QmStamps
        CavityChecks
        ExcelDocument
        Redlinings
        Memolist
        CompareResult
    End Enum

    Public Sub New(documentForm As DocumentForm)
        _activeDrawingCanvas = documentForm.ActiveDrawingCanvas
        _documentForm = documentForm
        _udmDocument = documentForm.udmDocument
        _utmDocument = documentForm.utmDocument
        _Tools = New UtDocumentTools(_utmDocument) ' HINT: initialize "tools-collection" for easy access on each TabTool/Button
        _logEventArgs = New LogEventArgs

        InitializeDocumentRibbon()
    End Sub

    Private Sub InitializeDocumentRibbon()
        _utmDocument.BeginUpdate()

        Try
            _utmDocument.Office2007UICompatibility = False
            _utmDocument.Style = ToolbarStyle.Office2010

            CreateChecksTabTools()
            CreateEditTabTools()
            CreateHomeTabTools()
            CreateSettingsTabTools()

            InitializeChecksTab()
            InitializeEditTab()
            InitializeHomeTab()
            InitializeSettingsTab()

        Catch ex As Exception
            ex.ShowMessageBox(String.Format(ErrorStrings.DocStatMachine_ErrorLoadMenu_Msg, vbCrLf, ex.Message))
        Finally
            _utmDocument.EndUpdate()
        End Try
    End Sub

    Private Sub CreateChecksTabTools()
        With _utmDocument
            Dim cavityAssignmentButton As New ButtonTool(ChecksTabToolKey.CavityAssignment.ToString)
            With cavityAssignmentButton.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.Checks_CavityAssignment.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.Checks_CavityAssignment.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.CavityAssignment
            End With

            Dim ultrasonicSpliceTerminalDistanceButton As New ButtonTool(ChecksTabToolKey.UltrasonicSpliceTerminalDistance.ToString)
            With ultrasonicSpliceTerminalDistanceButton.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.Checks_UltraSonicSpliceTerminalDistance.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.Checks_UltraSonicSpliceTerminalDistance.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.UltrasonicSpliceTerminalDistance
            End With

            Dim validityButton As New ButtonTool(ChecksTabToolKey.Validity.ToString)
            With validityButton.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.Checks_Validity.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.Checks_Validity.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.Validity
                .Visible = False
            End With

            Dim topologyCompareButton As New ButtonTool(ChecksTabToolKey.TopologyCompare.ToString)
            With topologyCompareButton.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.TopologyCompare
                .AppearancesSmall.Appearance.Image = My.Resources.TopologyCompare
                .Caption = My.Resources.MenuButtonStrings.TopologyCompare
                .Visible = _documentForm.HasTopoCompareFeature
            End With

            .Tools.Add(cavityAssignmentButton)
            .Tools.Add(ultrasonicSpliceTerminalDistanceButton)
            .Tools.Add(validityButton)
            .Tools.Add(topologyCompareButton)
        End With
    End Sub

    Private Sub CreateEditTabTools()
        With _utmDocument
            Dim addQMStampButton As New ButtonTool(EditTabToolKey.AddQMStamp.ToString)
            With addQMStampButton.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.EditQMStamp.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.EditQMStamp.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.Add
            End With

            Dim deleteQMStampButton As New ButtonTool(EditTabToolKey.DeleteQMStamp.ToString)
            With deleteQMStampButton.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.DeleteQMStamp.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.DeleteQMStamp.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.Delete
            End With

            Dim deleteRedliningButton As New ButtonTool(EditTabToolKey.DeleteRedlining.ToString)
            With deleteRedliningButton.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.DeleteRedlining.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.DeleteRedlining.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.Delete
            End With

            Dim editRedliningButton As New ButtonTool(EditTabToolKey.EditRedlining.ToString)
            With editRedliningButton.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.EditRedlining.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.EditRedlining.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.AddEdit
            End With

            Dim exportGraphicalDataCompareResultButton As New ButtonTool(EditTabToolKey.ExportGraphicalDataCompareResult.ToString)
            With exportGraphicalDataCompareResultButton.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.ExportGraphicalCompareResult.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.ExportGraphicalCompareResult.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.Export
            End With

            Dim exportMemolistButton As New ButtonTool(EditTabToolKey.ExportMemolist.ToString)
            With exportMemolistButton.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.ExportMemolist.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.ExportMemolist.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.Export
            End With

            Dim exportQMStampsButton As New ButtonTool(EditTabToolKey.ExportQMStamps.ToString)
            With exportQMStampsButton.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.ExportQMStamps.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.ExportQMStamps.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.Export
            End With

            Dim exportRedliningButton As New ButtonTool(EditTabToolKey.ExportRedlining.ToString)
            With exportRedliningButton.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.ExportRedlining.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.ExportRedlining.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.Export
            End With

            Dim exportTechnicalDataCompareResultButton As New ButtonTool(EditTabToolKey.ExportTechnicalDataCompareResult.ToString)
            With exportTechnicalDataCompareResultButton.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.ExportDataCompareResult.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.ExportDataCompareResult.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.Export
            End With

            Dim exportCavityAssignmentButton As New ButtonTool(EditTabToolKey.ExportCavityAssignment.ToString)
            With exportCavityAssignmentButton.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.ExportCavityAssignment.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.ExportCavityAssignment.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.Export
            End With

            Dim importGraphicalDataCompareResultButton As New ButtonTool(EditTabToolKey.ImportGraphicalDataCompareResult.ToString)
            With importGraphicalDataCompareResultButton.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.ImportGraphicalCompareResult.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.ImportGraphicalCompareResult.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.Import
            End With

            Dim importMemolistButton As New ButtonTool(EditTabToolKey.ImportMemolist.ToString)
            With importMemolistButton.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.ImportMemolist.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.ImportMemolist.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.Import
            End With

            Dim importQMStampsButton As New ButtonTool(EditTabToolKey.ImportQMStamps.ToString)
            With importQMStampsButton.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.ImportQMStamps.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.ImportQMStamps.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.Import
            End With

            Dim importRedliningButton As New ButtonTool(EditTabToolKey.ImportRedlining.ToString)
            With importRedliningButton.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.ImportRedlining.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.ImportRedlining.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.Import
            End With

            Dim importTechnicalDataCompareResultButton As New ButtonTool(EditTabToolKey.ImportTechnicalDataCompareResult.ToString)
            With importTechnicalDataCompareResultButton.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.ImportDataCompareResult.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.ImportDataCompareResult.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.Import
            End With

            Dim importValidityCheckResultsButton As New ButtonTool(EditTabToolKey.ImportValidityCheckResults.ToString)
            With importValidityCheckResultsButton.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.ImportValidityCheck.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.ImportValidityCheck.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.Import
            End With

            .Tools.Add(addQMStampButton)
            .Tools.Add(deleteQMStampButton)
            .Tools.Add(deleteRedliningButton)
            .Tools.Add(editRedliningButton)
            .Tools.Add(exportGraphicalDataCompareResultButton)
            .Tools.Add(exportMemolistButton)
            .Tools.Add(exportQMStampsButton)
            .Tools.Add(exportRedliningButton)
            .Tools.Add(exportTechnicalDataCompareResultButton)
            .Tools.Add(exportCavityAssignmentButton)
            .Tools.Add(importGraphicalDataCompareResultButton)
            .Tools.Add(importMemolistButton)
            .Tools.Add(importQMStampsButton)
            .Tools.Add(importRedliningButton)
            .Tools.Add(importTechnicalDataCompareResultButton)
            .Tools.Add(importValidityCheckResultsButton)
        End With
    End Sub

    Private Sub CreateHomeTabTools()
        With _utmDocument

            Dim bomActive As New ButtonTool(HomeTabToolKey.BOMActive.ToString)
            With bomActive.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.BOMActive.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.BOMActive.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.ActiveModules
            End With

            Dim bomAll As New ButtonTool(HomeTabToolKey.BOMAll.ToString)
            With bomAll.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.BOMAll.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.BOMAll.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.AllModules
            End With

            Dim deviceBom As New ButtonTool(HomeTabToolKey.DeviceBOM.ToString)
            With deviceBom.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.BOMActive.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.BOMActive.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.DeviceBOM
            End With

            Dim exportExcel As New ButtonTool(HomeTabToolKey.ExportExcel.ToString)
            With exportExcel.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.ExportExcel.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.ExportExcel.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.AllDataTables
            End With

            Dim exportExcelActiveDataTable As New ButtonTool(HomeTabToolKey.ExportExcelActiveDataTable.ToString)
            With exportExcelActiveDataTable.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.ExportExcelActiveDataTable.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.ExportExcelActiveDataTable.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.ActiveDataTable
            End With

            Dim manageModuleConfig As New ButtonTool(HomeTabToolKey.ModuleConfigManager.ToString)
            With manageModuleConfig.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.ModuleConfigManager.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.ModuleConfigManager.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.Manage
            End With

            Dim pan As New StateButtonTool(HomeTabToolKey.Pan.ToString)
            With pan.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.Pan.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.Pan.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.Pan
                .Enabled = Not _documentForm.IsTouchEnabled
            End With

            Dim pasteFromClipboard As New ButtonTool(HomeTabToolKey.PasteFromClipboard.ToString)
            With pasteFromClipboard.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.PasteFromClipboard.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.PasteFromClipboard.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.PasteFromClipboard
                .Enabled = Clipboard.ContainsText AndAlso Not Clipboard.GetText.IsNullOrEmpty AndAlso Clipboard.GetText <> " "
            End With

            Dim refresh As New ButtonTool(HomeTabToolKey.Refresh.ToString)
            With refresh.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.Refresh
                .AppearancesSmall.Appearance.Image = My.Resources.Refresh
                .Caption = My.Resources.MenuButtonStrings.Refresh
            End With

#If DEBUG Or CONFIG = "Debug" Then
            Dim openInternalModelViewer As New ButtonTool(HomeTabToolKey.OpenInternalModelViewer.ToString)
            With openInternalModelViewer.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.InternalModelViewer
                .AppearancesSmall.Appearance.Image = My.Resources.InternalModelViewer
                .Caption = My.Resources.MenuButtonStrings.InternalModelViewer
            End With
#End If

            Dim validateData As New ButtonTool(HomeTabToolKey.ValidateData.ToString)
            With validateData.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.ValidateData.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.ValidateData.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.ValidateData
            End With

            Dim zoomExtends As New ButtonTool(HomeTabToolKey.ZoomExtends.ToString)
            With zoomExtends.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.ZoomExtends.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.ZoomExtends.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.ZoomExtends
            End With

            Dim zoomIn As New ButtonTool(HomeTabToolKey.ZoomIn.ToString)
            With zoomIn.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.ZoomIn.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.ZoomIn.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.ZoomIn
            End With

            Dim zoomOut As New ButtonTool(HomeTabToolKey.ZoomOut.ToString)
            With zoomOut.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.ZoomOut.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.ZoomOut.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.ZoomOut
            End With

            Dim zoomPrevious As New ButtonTool(HomeTabToolKey.ZoomPrevious.ToString)
            With zoomPrevious.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.ZoomPrevious.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.ZoomPrevious.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.ZoomPrevious
            End With

            Dim zoomWindow As New ButtonTool(HomeTabToolKey.ZoomWindow.ToString)
            With zoomWindow.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.ZoomWindow.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.ZoomWindow.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.ZoomWindow
            End With

            Dim zoomMagnifier As New StateButtonTool(HomeTabToolKey.ZoomMagnifier.ToString)
            With zoomMagnifier.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.ZoomMagnifier.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.ZoomMagnifier.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.ZoomMagnifier
            End With

            .Tools.Add(bomActive)
            .Tools.Add(bomAll)
            .Tools.Add(deviceBom)
            .Tools.Add(exportExcel)
            .Tools.Add(exportExcelActiveDataTable)
            .Tools.Add(manageModuleConfig)
            .Tools.Add(pan)
            .Tools.Add(pasteFromClipboard)
            .Tools.Add(refresh)
            .Tools.Add(validateData)
            .Tools.Add(zoomExtends)
            .Tools.Add(zoomIn)
            .Tools.Add(zoomOut)
            .Tools.Add(zoomPrevious)
            .Tools.Add(zoomWindow)
            .Tools.Add(zoomMagnifier)
#If DEBUG Or CONFIG = "Debug" Then
            .Tools.Add(openInternalModelViewer)
#End If
        End With
    End Sub

    Private Sub CreateSettingsTabTools()
        With _utmDocument
            Dim displayDrawingsHub As New StateButtonTool(SettingsTabToolKey.DisplayDrawingsHub.ToString)
            With displayDrawingsHub.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.DisplayDrawingsHub.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.DisplayDrawingsHub.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.Drawings
            End With

            displayDrawingsHub.Checked = True

            Dim displayInformationHub As New StateButtonTool(SettingsTabToolKey.DisplayInformationHub.ToString)
            With displayInformationHub.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.DisplayInformationHub.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.DisplayInformationHub.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.DataTables
            End With

            displayInformationHub.Checked = True

            Dim displayLogHub As New StateButtonTool(SettingsTabToolKey.DisplayLogHub.ToString)
            With displayLogHub.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.DisplayLogHub.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.DisplayLogHub.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.Log
            End With

            Dim displayMemolistHub As New StateButtonTool(SettingsTabToolKey.DisplayMemolistHub.ToString)
            With displayMemolistHub.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.DisplayMemolistHub.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.DisplayMemolistHub.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.Memolist
            End With

            Dim displayModulesHub As New StateButtonTool(SettingsTabToolKey.DisplayModulesHub.ToString)
            With displayModulesHub.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.DisplayModulesHub.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.DisplayModulesHub.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.Modules
            End With

            displayModulesHub.Checked = True

            Dim displayNavigatorHub As New StateButtonTool(SettingsTabToolKey.DisplayNavigatorHub.ToString)
            With displayNavigatorHub.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.DisplayNavigatorHub.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.DisplayNavigatorHub.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.Navigator
            End With

            displayNavigatorHub.Checked = Not (TryCast(_documentForm.GeneralSettings, GeneralSettings)?.HideNavigatorHubOnLoad).GetValueOrDefault

            Dim displayQMStamps As New StateButtonTool(SettingsTabToolKey.DisplayQMStamps.ToString)
            With displayQMStamps.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.DisplayQMStamps.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.DisplayQMStamps.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.DisplayQMStamps
            End With

            displayQMStamps.Checked = True

            Dim displayRedlinings As New StateButtonTool(SettingsTabToolKey.DisplayRedlinings.ToString)
            With displayRedlinings.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.DisplayRedlinings.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.DisplayRedlinings.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.DisplayRedlinings
            End With

            displayRedlinings.Checked = True


            '******************Indicators*******************************


            _indicatorsListView = New UltraListView()
            _indicatorsListView.BeginUpdate()
            _indicatorsListView.View = UltraListViewStyle.List

            _indicatorsListView.ViewSettingsList.CheckBoxStyle = CheckBoxStyle.CheckBox
            _indicatorsListView.ViewSettingsList.ImageSize = Size.Empty
            _indicatorsListView.Text = My.Resources.MenuButtonStrings.Indicators


            Dim ItemShowSectionDimensions As New UltraListViewItem(SettingsTabToolKey.SectionDimensions.ToString)
            With ItemShowSectionDimensions
                .Value = My.Resources.MenuButtonStrings.SectionDimensions
            End With

            Dim ItemShowClockSymbols As New UltraListViewItem(SettingsTabToolKey.ClockSymbols.ToString)
            With ItemShowClockSymbols
                .Value = My.Resources.MenuButtonStrings.ClockSymbols
            End With

            Dim ItemShowDimensions As New UltraListViewItem(SettingsTabToolKey.Dimensions.ToString)
            With ItemShowDimensions
                .Value = My.Resources.MenuButtonStrings.Dimensions
            End With

            Dim ItemShowViewDirections As New UltraListViewItem(SettingsTabToolKey.ViewDirections.ToString)
            With ItemShowViewDirections
                .Value = My.Resources.MenuButtonStrings.ViewDirections
            End With

            Dim ItemShowImplicitDimensions As New UltraListViewItem(SettingsTabToolKey.ImplicitDimensions.ToString)
            With ItemShowImplicitDimensions
                .Value = My.Resources.MenuButtonStrings.ImpliciteDimensions
            End With

            Dim ItemShowHints As New UltraListViewItem(SettingsTabToolKey.Hints.ToString)
            With ItemShowHints
                .Value = My.Resources.MenuButtonStrings.Hints
            End With

            _indicatorsListView.Items.Add(ItemShowClockSymbols)
            _indicatorsListView.Items.Add(ItemShowDimensions)
            _indicatorsListView.Items.Add(ItemShowImplicitDimensions)
            _indicatorsListView.Items.Add(ItemShowViewDirections)
            _indicatorsListView.Items.Add(ItemShowHints)
            _indicatorsListView.Items.Add(ItemShowSectionDimensions)


            AddHandler _indicatorsListView.ItemCheckStateChanged, AddressOf lViewIndicatorsValueChanged

            Dim cntrlIndicators As New PopupControlContainerTool(SettingsTabToolKey.Indicators.ToString)
            With cntrlIndicators.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.DisplayIndicators
                .AppearancesSmall.Appearance.Image = My.Resources.DisplayIndicators
                .Caption = My.Resources.MenuButtonStrings.Indicators
                .Enabled = True
            End With

            _indicatorsListView.Height = (_indicatorsListView.ItemSizeResolved.Height + 4) * (_indicatorsListView.Items.Count + 1)
            _indicatorsListView.Width = _indicatorsListView.ItemSizeResolved.Width
            cntrlIndicators.Control = _indicatorsListView
            cntrlIndicators.SharedProps.Visible = _documentForm.HasView3DFeature
            _indicatorsListView.EndUpdate()

            '*******************************************************************************

            .Tools.Add(displayDrawingsHub)
            .Tools.Add(displayInformationHub)
            .Tools.Add(displayLogHub)
            .Tools.Add(displayMemolistHub)
            .Tools.Add(displayModulesHub)
            .Tools.Add(displayNavigatorHub)
            .Tools.Add(displayQMStamps)
            .Tools.Add(displayRedlinings)
            .Tools.Add(cntrlIndicators)
        End With
    End Sub

    Private Sub lViewIndicatorsValueChanged(sender As Object, e As EventArgs)
        Dim ShowItem As String = String.Empty
        Dim RemoveItem As String = String.Empty

        Dim lView As UltraListView = CType(sender, UltraListView)
        Dim oldSelection As New List(Of String)
        If lView.Tag IsNot Nothing AndAlso TypeOf lView.Tag Is List(Of String) Then
            Dim myList As List(Of String) = CType(lView.Tag, List(Of String))
            For Each entry As String In myList
                oldSelection.Add(entry)
            Next

        End If

        Dim selection As UltraListViewCheckedItemsCollection = lView.CheckedItems
        Dim currentSelection As New List(Of String)

        For Each item As UltraListViewItem In lView.CheckedItems
            currentSelection.Add(item.Key)
            If Not oldSelection.Contains(item.Key) Then
                ShowItem = item.Key
            End If
        Next

        If String.IsNullOrEmpty(ShowItem) Then
            For Each item As String In oldSelection
                If Not currentSelection.Contains(item) Then
                    RemoveItem = item
                    Exit For
                End If
            Next
        End If

        lView.Tag = currentSelection

        If Not String.IsNullOrEmpty(ShowItem) Then
            ShowIndicators(ShowItem)
        ElseIf Not String.IsNullOrEmpty(RemoveItem) Then
            RemoveIndicators(RemoveItem)
        End If
    End Sub

    Private Sub ShowIndicators(IndicatorType As String)
        Select Case IndicatorType
            Case SettingsTabToolKey.ClockSymbols.ToString
                _documentForm.ShowClockSymbols()
            Case SettingsTabToolKey.Dimensions.ToString
                _documentForm.ShowDimensions()
            Case SettingsTabToolKey.ViewDirections.ToString
                _documentForm.ShowVertexViewDirections()
            Case SettingsTabToolKey.ImplicitDimensions.ToString
                _documentForm.ShowImplicitDimensions()
            Case SettingsTabToolKey.Hints.ToString
                _documentForm.ShowHints()
            Case SettingsTabToolKey.SectionDimensions.ToString
                _documentForm.ShowSectionDimensions()
        End Select
    End Sub

    Private Sub RemoveIndicators(IndicatorType As String)
        Select Case IndicatorType
            Case SettingsTabToolKey.ClockSymbols.ToString
                _documentForm.ClearClockSymbols()
            Case SettingsTabToolKey.Dimensions.ToString
                _documentForm.ClearDimensions()
            Case SettingsTabToolKey.ViewDirections.ToString
                _documentForm.ClearStartViewVertex()
            Case SettingsTabToolKey.ImplicitDimensions.ToString
                _documentForm.ClearImplicitDimensions()
            Case SettingsTabToolKey.Hints.ToString
                _documentForm.ClearHints()
            Case SettingsTabToolKey.SectionDimensions.ToString
                _documentForm.ClearSectionDimensions()
        End Select
    End Sub

    Private Sub DoExportCavities()
        If _documentForm.MainForm.DocumentViews(_documentForm.Id).Model.AnyChanged Then
            Using sdExportCavityCheckInformation As New SaveFileDialog
                With sdExportCavityCheckInformation
                    .DefaultExt = Checks.Cavities.Files.CavityChecksFile.FileExtension
                    .Filter = Checks.Cavities.Files.CavityChecksFile.FileFilter
                    .Title = DialogStrings.DocStatMachine_SaveCavityChecksToFile_Title
                    .FileName = GetDefaultFileName(FileNameType.CavityChecks)
                End With

                If sdExportCavityCheckInformation.ShowDialog(_documentForm) = System.Windows.Forms.DialogResult.OK Then
                    _documentForm.MemoList.Name = IO.Path.GetFileNameWithoutExtension(sdExportCavityCheckInformation.FileName)
                    Try
                        _documentForm.MainForm.DocumentViews(_documentForm.Id).Export(sdExportCavityCheckInformation.FileName, Checks.Cavities.Views.Document.DocumentView.ExportFileFormat.CavityChecksInformation)
                        If MessageBox.Show(DialogStrings.DocStatMachine_CavityChecksExportSuccess_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                            ProcessEx.Start(sdExportCavityCheckInformation.FileName)
                        End If
                    Catch ex As Exception
                        ex.ShowMessageBox(String.Format(ErrorStrings.DocStatMachine_ErrorExportMsg, ex.Message))
                    End Try
                End If
            End Using
        Else
            MessageBox.Show(DialogStrings.DocStatMachine_NoCavityChecks, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub ExportRedlinings(fileName As String)
        Try
            Dim temporaryFolder As IO.DirectoryInfo = IO.Directory.CreateDirectory(IO.Path.Combine(IO.Path.GetDirectoryName(fileName), IO.Path.GetFileNameWithoutExtension(fileName)))
            IO.FileEx.TryDelete(fileName)

            _documentForm.redliningInformation.Name = IO.Path.GetFileNameWithoutExtension(fileName)
            _documentForm.redliningInformation.HarnessPartNumber = _documentForm.KBL.HarnessPartNumber
            _documentForm.redliningInformation.HarnessVersion = _documentForm.KBL.GetHarness.Version
            _documentForm.redliningInformation.Save(IO.Path.Combine(temporaryFolder.FullName, String.Format("{0}.xml", IO.Path.GetFileNameWithoutExtension(fileName))))

            For Each redlining As Redlining In _documentForm.redliningInformation.Redlinings
                If (redlining.Classification = RedliningClassification.GraphicalComment) AndAlso (redlining.Comment <> String.Empty) Then
                    Using documentStream As New IO.MemoryStream(Convert.FromBase64String(redlining.Comment))
                        documentStream.Position = 0

                        Using vDraw As New VectorDraw.Professional.Control.VectorDrawBaseControl
                            vDraw.EnsureDocument()

                            If (vDraw.ActiveDocument.LoadFromMemory(documentStream, True)) Then
                                vDraw.ActiveDocument.GridMode = False

                                VdOpenSave.SaveAs(vDraw.ActiveDocument, IO.Path.Combine(temporaryFolder.FullName, String.Format("{0}.png", redlining.ID)))
                            End If
                        End Using

                        documentStream.Close()
                    End Using
                End If
            Next

            ZipFile.CreateFromDirectory(temporaryFolder.FullName, fileName)

            IO.Directory.Delete(temporaryFolder.FullName, True)

            Select Case _documentForm.File.Type
                Case KnownContainerFileFlags.KBL, KnownContainerFileFlags.VEC
                    _documentForm.IsDirty = False
            End Select

            MessageBoxEx.ShowInfo(DialogStrings.DocStatMachine_RedliningExportSuccess_Msg)
        Catch ex As Exception
            ex.ShowMessageBox(String.Format(DialogStrings.DocStatMachine_RedliningExportError_Msg, ex.Message))
        End Try
    End Sub

    Private Sub ImportCompareResultInformation(fileName As String)
        Dim ccResultInfo As CheckedCompareResultInformation = Nothing
        Try
            ccResultInfo = CheckedCompareResultInformation.LoadFromFile(fileName)
            If (ccResultInfo IsNot Nothing) AndAlso (ccResultInfo.HarnessPartNumber = _documentForm.KBL.HarnessPartNumber) AndAlso (ccResultInfo.HarnessVersion = _documentForm.KBL.GetHarness.Version) Then
                If (Not _documentForm.GetCompareResultInfo(ccResultInfo.Type).CheckedCompareResults.Any) OrElse (MessageBoxEx.ShowQuestion(If(ccResultInfo.Type = KnownContainerFileFlags.TCRI, DialogStrings.DocStatMachine_OvrwrGraphCompResultInfo_Msg, DialogStrings.DocStatMachine_OvrwrTechCompResultInfo_Msg)) = DialogResult.Yes) Then
                    _documentForm.SetCompareResultInfo(ccResultInfo)
                    Select Case ccResultInfo.Type
                        Case [Lib].IO.Files.Hcv.KnownContainerFileFlags.GCRI
                            _documentForm.File.GCRI = New Files.Hcv.GraphicalCheckedCompareResultInfoContainerFile(ccResultInfo.AsStream.ToArray)
                        Case [Lib].IO.Files.Hcv.KnownContainerFileFlags.TCRI
                            _documentForm.File.TCRI = New Files.Hcv.TechnicalCheckedCompareResultInfoContainerFile(ccResultInfo.AsStream.ToArray)
                    End Select

                    If Not _documentForm.File.IsXhcvChild Then
                        _documentForm.File.Save(useTempIntermediateFile:=True) ' hint use temp intermediate to avoid corrupting zip archives while saving them -> when temp files is used the current file will only be overwritten when saving was successfull
                    End If

                    MessageBox.Show(If(ccResultInfo.Type = [Lib].IO.Files.Hcv.KnownContainerFileFlags.GCRI, DialogStrings.DocStatMachine_GraphCompResultInfoImportSuccess_Msg, DialogStrings.DocStatMachine_TechCompResultInfoImportSuccess_Msg), [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            Else
                MessageBoxEx.ShowError(If((ccResultInfo?.Type).GetValueOrDefault = [Lib].IO.Files.Hcv.KnownContainerFileFlags.GCRI, ErrorStrings.DocStatMachine_ErrorLoadGraphCompResultInfo_Msg, ErrorStrings.DocStatMachine_ErrorLoadTechCompResultInfo_Msg))
            End If
        Catch ex As Exception
            ex.ShowMessageBox(String.Format(If((ccResultInfo?.Type).GetValueOrDefault = [Lib].IO.Files.Hcv.KnownContainerFileFlags.GCRI, ErrorStrings.DocStatMachine_ErrorImportGraphCompResultInfo_Msg, ErrorStrings.DocStatMachine_ErrorImportTechCompResultInfo_Msg), vbCrLf, ex.Message))
        End Try
    End Sub

    Private Sub ImportMemolist(fileName As String)
        Try
            Dim mList As Memolist = Memolist.LoadFromFile(fileName)
            If (mList IsNot Nothing) AndAlso (mList.HarnessPartNumber = _documentForm.KBL.HarnessPartNumber AndAlso mList.HarnessVersion = _documentForm.KBL.HarnessVersion) OrElse (MessageBoxEx.ShowWarning(DialogStrings.ImportMemolist_Msg) = DialogResult.Yes) Then
                Dim identicalItemsExisting As Boolean = False
                Dim importErrors As Boolean = False
                Dim overrideExistingItems As Boolean = False

                _documentForm._memolistHub.UpdateData(_documentForm.MemoList.MemoItems)
                identicalItemsExisting = _documentForm.MemoList.MemoItems.GetIds.ContainsAnyOf(mList.MemoItems.GetIds)

                If (identicalItemsExisting) AndAlso (MessageBoxEx.ShowQuestion(DialogStrings.DocStatMachine_OvrIdentMemListItmsInDoc_Msg) = DialogResult.Yes) Then
                    overrideExistingItems = True
                End If

                For Each memoItem As MemoItem In mList.MemoItems
                    If (Not _documentForm.MemoList.MemoItems.Where(Function(memItem) memItem.Id = memoItem.Id).Any()) Then
                        If (_documentForm.KBL.KBLOccurrenceMapper.ContainsKey(memoItem.Id)) OrElse (memoItem.Type = "Nets" AndAlso _documentForm.KBL.KBLNetList.Contains(memoItem.Id)) Then
                            _documentForm.MemoList.MemoItems.Add(memoItem)
                        Else
                            importErrors = True

                            _logEventArgs.LogLevel = LogEventArgs.LoggingLevel.Warning
                            _logEventArgs.LogMessage = String.Format(My.Resources.LoggingStrings.DocStatMachine_ImportMemolistFailed, memoItem.Comment, memoItem.Type, memoItem.Name)

                            RaiseEvent LogMessage(_documentForm, _logEventArgs)
                        End If
                    ElseIf (overrideExistingItems) Then
                        For Each memoItemInDocument As MemoItem In _documentForm.MemoList.MemoItems.Where(Function(memItem) memItem.Id = memoItem.Id)
                            With memoItemInDocument
                                .Comment = memoItem.Comment
                                .IsSelected = memoItem.IsSelected
                            End With
                        Next
                    End If
                Next

                If (importErrors) Then
                    MessageBoxEx.ShowWarning(ErrorStrings.DocStatMachine_ErrorImportMemolist_Msg)
                Else
                    MessageBox.Show(DialogStrings.DocStatMachine_MemolistImportSuccess_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If

                _documentForm._memolistHub.InitializeData(_documentForm.MemoList.MemoItems, True)
                _udmDocument.PaneFromKey("MemolistHub").Show()

                Me.Tools.Settings.DisplayMemolistHub.Checked = True
            ElseIf (mList Is Nothing) Then
                MessageBoxEx.ShowError(String.Format(ErrorStrings.DocStatMachine_ErrorLoadMemolistFile_Msg, vbCrLf, Memolist.LoadError.Message))
            End If
        Catch ex As Exception
            ex.ShowMessageBox(String.Format(ErrorStrings.DocStatMachine_ErrorImportMemolistFile_Msg, vbCrLf, ex.Message))
        End Try
    End Sub

    Private Sub ImportQMStamps(fileName As String)
        Try
            Dim qmStamps As QMStamps = QMStamps.LoadFromFile(fileName)
            If (qmStamps IsNot Nothing) AndAlso ((qmStamps.HarnessPartNumber = _documentForm.KBL.HarnessPartNumber AndAlso qmStamps.HarnessVersion = _documentForm.KBL.GetHarness.Version) OrElse (MessageBox.Show(DialogStrings.ImportQMStamps_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.Yes)) Then
                Dim identicalQMStampsExisting As Boolean = False
                Dim importErrors As Boolean = False
                Dim overrideExistingQMStamps As Boolean = False

                For Each qmStamp As QMStamp In qmStamps.Stamps
                    If _documentForm.QmStamps.Stamps.Where(Function(stamp) stamp.Id = qmStamp.Id).Any Then
                        identicalQMStampsExisting = True
                        Exit For
                    End If
                Next

                If (identicalQMStampsExisting) AndAlso (MessageBox.Show(DialogStrings.DocStatMachine_OvrIdentQMStampsInDoc_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes) Then overrideExistingQMStamps = True

                For Each qmStamp As QMStamp In qmStamps.Stamps
                    If (Not _documentForm.QmStamps.Stamps.Where(Function(stamp) stamp.Id = qmStamp.Id).Any()) Then
                        Dim noObjRefFound As Boolean = True

                        For Each objRef As ObjectReference In qmStamp.ObjectReferences
                            If (_documentForm.KBL.KBLOccurrenceMapper.ContainsKey(objRef.KblId)) _
                                OrElse (objRef.ObjectType = E3.Lib.Schema.Kbl.KblObjectType.Net AndAlso _documentForm.KBL.KBLNetList.Contains(objRef.KblId)) _
                                OrElse (objRef.ObjectType = E3.Lib.Schema.Kbl.KblObjectType.Harness AndAlso objRef.KblId = _documentForm.KBL.GetHarness.SystemId) Then
                                noObjRefFound = False

                                Exit For
                            End If
                        Next

                        If Not noObjRefFound Then
                            _documentForm.QmStamps.Stamps.Add(qmStamp)
                        Else
                            importErrors = True

                            _logEventArgs.LogLevel = LogEventArgs.LoggingLevel.Warning
                            _logEventArgs.LogMessage = String.Format(My.Resources.LoggingStrings.DocStatMachine_ImportQMStampFailed, qmStamp.RefNo)

                            RaiseEvent LogMessage(_documentForm, _logEventArgs)
                        End If
                    ElseIf (overrideExistingQMStamps) Then
                        For Each qmStampInDocument As QMStamp In _documentForm.QmStamps.Stamps.Where(Function(stamp) stamp.Id = qmStamp.Id)
                            If (qmStamp.DateOfCheck > qmStampInDocument.DateOfCheck) Then
                                With qmStampInDocument
                                    .CheckComment = qmStamp.CheckComment
                                    .CheckedBy = qmStamp.CheckedBy
                                    .DateOfCheck = qmStamp.DateOfCheck
                                    .Passed = qmStamp.Passed
                                    .RefNo = qmStamp.RefNo
                                    .Specification = qmStamp.Specification

                                    .ObjectReferences = New ObjectReferenceList
                                    .ObjectReferences.AddRange(qmStamp.ObjectReferences)
                                End With
                            End If
                        Next
                    End If
                Next

                If (importErrors) Then
                    MessageBoxEx.ShowWarning(ErrorStrings.DocStatMachine_ErrorImportQMStamps_Msg)
                Else
                    MessageBox.Show(DialogStrings.DocStatMachine_QMStampsImportSuccess_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If

                _documentForm._informationHub.ImportQMStamps()
            ElseIf (qmStamps Is Nothing) Then
                MessageBoxEx.ShowError(String.Format(ErrorStrings.DocStatMachine_ErrorLoadQMStampsFile_Msg, vbCrLf, QMStamps.LoadError.Message))
            End If
        Catch ex As Exception
            ex.ShowMessageBox(String.Format(ErrorStrings.DocStatMachine_ErrorImportQMStampsFile_Msg, vbCrLf, ex.Message))
        End Try
    End Sub

    Private Sub ImportRedlinings(fileName As String)
        Try
            Dim redliningInfos As RedliningInformation = Nothing
            Try
                ' HINT: This is our new compressed RDL data format --> Extract information first
                Using zipArchive As ZipArchive = ZipFile.OpenRead(fileName)
                    Dim files As New List(Of String)
                    Dim temporaryFolder As IO.DirectoryInfo = IO.Directory.CreateDirectory(IO.Path.Combine(IO.Path.GetTempPath, Guid.NewGuid.ToString))

                    For Each zipArchiveEntry As ZipArchiveEntry In zipArchive.Entries
                        zipArchiveEntry.ExtractToFile(IO.Path.Combine(temporaryFolder.FullName, zipArchiveEntry.FullName))
                        files.Add(IO.Path.Combine(temporaryFolder.FullName, zipArchiveEntry.FullName))
                    Next

                    For Each file As String In files.Where(Function(f) KnownFile.IsType(KnownFile.Type.XML, f))
                        redliningInfos = RedliningInformation.LoadFromFile(file)
                    Next

                    IO.Directory.Delete(temporaryFolder.FullName, True)
                End Using
            Catch ex As System.IO.InvalidDataException
                'HINT this might be an old pure xml redlining file
                redliningInfos = RedliningInformation.LoadFromFile(fileName)
            End Try


            If (redliningInfos IsNot Nothing) AndAlso ((redliningInfos.HarnessPartNumber = _documentForm.KBL.HarnessPartNumber AndAlso redliningInfos.HarnessVersion = _documentForm.KBL.GetHarness.Version) OrElse (MessageBox.Show(Me._documentForm, DialogStrings.ImportRedlinings_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.Yes)) Then
                Dim identicalRedliningGroupsExisting As Boolean = False
                Dim identicalRedliningsExisting As Boolean = False

                Dim importErrors As Boolean = False

                Dim overrideExistingRedliningGroups As Boolean = False
                Dim overrideExistingRedlinings As Boolean = False

                For Each redliningGroup As RedliningGroup In redliningInfos.RedliningGroups
                    If (_documentForm.redliningInformation.RedliningGroups.Where(Function(rdliningGrp) rdliningGrp.ID = redliningGroup.ID).Any()) Then
                        identicalRedliningGroupsExisting = True

                        Exit For
                    End If
                Next

                For Each redlining As Redlining In redliningInfos.Redlinings
                    If (_documentForm.redliningInformation.Redlinings.Where(Function(rdlining) rdlining.ID = redlining.ID).Any()) Then
                        identicalRedliningsExisting = True

                        Exit For
                    End If
                Next

                If (identicalRedliningsExisting) AndAlso (MessageBoxEx.ShowQuestion(Me._documentForm, DialogStrings.DocStatMachine_OvrIdentRedlsInDoc_Msg) = DialogResult.Yes) Then
                    overrideExistingRedlinings = True
                End If

                If (identicalRedliningGroupsExisting) AndAlso (MessageBoxEx.ShowQuestion(Me._documentForm, DialogStrings.DocStatMachine_OvrIdentRedlGroupsInDoc_Msg) = DialogResult.Yes) Then
                    overrideExistingRedliningGroups = True
                End If

                For Each redliningGroup As RedliningGroup In redliningInfos.RedliningGroups
                    If Not _documentForm.redliningInformation.RedliningGroups.Any(Function(rdliningGrp) rdliningGrp.ID = redliningGroup.ID) Then
                        _documentForm.redliningInformation.RedliningGroups.Add(redliningGroup)
                    ElseIf (overrideExistingRedlinings) Then
                        For Each redliningGroupInDocument As RedliningGroup In _documentForm.redliningInformation.RedliningGroups.Where(Function(rdliningGrp) rdliningGrp.ID = redliningGroup.ID)
                            If (redliningGroup.LastChangedOn > redliningGroupInDocument.LastChangedOn) Then
                                With redliningGroupInDocument
                                    .ChangeTag = redliningGroup.ChangeTag
                                    .Comment = redliningGroup.Comment
                                    .LastChangedBy = redliningGroup.LastChangedBy
                                    .LastChangedOn = redliningGroup.LastChangedOn
                                End With
                            End If
                        Next
                    End If
                Next

                For Each redlining As Redlining In redliningInfos.Redlinings
                    If Not _documentForm.redliningInformation.Redlinings.Any(Function(rdlining) rdlining.ID = redlining.ID) Then
                        Dim redling_occ As IKblOccurrence = _documentForm.KBL.GetOccurrenceObjectUntyped(redlining.ObjectId)
                        If redling_occ IsNot Nothing OrElse (redlining.ObjectType = KblObjectType.Net AndAlso _documentForm.KBL.KBLNetList.Contains(redlining.ObjectId)) Then
                            _documentForm.redliningInformation.Redlinings.Add(redlining)
                        Else
                            importErrors = True

                            _logEventArgs.LogLevel = LogEventArgs.LoggingLevel.Warning
                            _logEventArgs.LogMessage = String.Format(My.Resources.LoggingStrings.DocStatMachine_ImportRedliningFailed, redlining.Comment, redlining.ObjectType, redlining.ObjectName)

                            RaiseEvent LogMessage(_documentForm, _logEventArgs)
                        End If
                    ElseIf (overrideExistingRedlinings) Then
                        For Each redliningInDocument As Redlining In _documentForm.redliningInformation.Redlinings.GetById(redlining.ID)
                            If (redlining.LastChangedOn > redliningInDocument.LastChangedOn) Then
                                With redliningInDocument
                                    .Classification = redlining.Classification
                                    .Comment = redlining.Comment
                                    .IsVisible = redlining.IsVisible
                                    .LastChangedBy = redlining.LastChangedBy
                                    .LastChangedOn = redlining.LastChangedOn
                                    .AssignedModules = redlining.AssignedModules
                                End With
                            End If
                        Next
                    End If
                Next

                If (importErrors) Then
                    MessageBoxEx.ShowWarning(Me._documentForm, ErrorStrings.DocStatMachine_ErrorImportRedlining_Msg)
                Else
                    MessageBox.Show(DialogStrings.DocStatMachine_RedliningImportSuccess_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If

                _documentForm._informationHub.ImportRedlinings()
            ElseIf (redliningInfos Is Nothing) Then
                MessageBoxEx.ShowError(Me._documentForm, String.Format(ErrorStrings.DocStatMachine_ErrorLoadRedliningFile_Msg, vbCrLf, RedliningInformation.LoadError.Message))
            End If
        Catch ex As Exception
            ex.ShowMessageBox(Me._documentForm, String.Format(ErrorStrings.DocStatMachine_ErrorImportRedliningFile_Msg, vbCrLf, ex.Message))
        End Try
    End Sub

    Private Function ImportUltrasonicSpliceTerminalDistanceMapping(fileName As String) As UltrasonicSpliceTerminalDistanceMapping
        Try
            Dim ultrasonicSpliceTerminalDistanceMapping As UltrasonicSpliceTerminalDistanceMapping = Nothing

            If KnownFile.IsXls(fileName) OrElse KnownFile.IsXlsx(fileName) Then
                Dim workbook As Workbook = Workbook.Load(fileName)
                If (workbook IsNot Nothing) Then
                    For Each worksheet As Worksheet In workbook.Worksheets
                        Dim cell As Infragistics.Documents.Excel.WorksheetCell = worksheet.GetCell("A1")
                        If cell IsNot Nothing AndAlso (cell.Value.ToString = "TerminalPartNumber") AndAlso (worksheet.GetCell("B1").Value.ToString = "MinDistance") Then
                            ultrasonicSpliceTerminalDistanceMapping = New UltrasonicSpliceTerminalDistanceMapping

                            For Each row As WorksheetRow In worksheet.Rows
                                If (row.Index >= 1) Then
                                    ultrasonicSpliceTerminalDistanceMapping.UltrasonicSpliceTerminalDistanceMaps.AddUltrasonicSpliceTerminalDistanceMap(New UltrasonicSpliceTerminalDistanceMap(CSng(row.Cells(1).Value), row.Cells(0).Value.ToString))
                                End If
                            Next

                            Exit For
                        End If
                    Next

                    If (ultrasonicSpliceTerminalDistanceMapping Is Nothing) Then
                        MessageBoxEx.ShowError(String.Format(ErrorStrings.DocStatMachine_ErrorLoadUltrasonicSpliceTerminalDistanceMappingFile_Msg, vbCrLf, ErrorStrings.DocStatMachine_ErrorLoadUltrasonicSpliceTerminalDistanceMappingFile_Excel))
                    End If
                End If
            Else
                ultrasonicSpliceTerminalDistanceMapping = UltrasonicSpliceTerminalDistanceMapping.LoadFromFile(fileName)
            End If

            If (ultrasonicSpliceTerminalDistanceMapping IsNot Nothing) AndAlso (ultrasonicSpliceTerminalDistanceMapping.UltrasonicSpliceTerminalDistanceMaps.Any) Then
                Return ultrasonicSpliceTerminalDistanceMapping
            ElseIf (ultrasonicSpliceTerminalDistanceMapping Is Nothing) AndAlso (UltrasonicSpliceTerminalDistanceMapping.LoadError IsNot Nothing) Then
                MessageBoxEx.ShowError(String.Format(ErrorStrings.DocStatMachine_ErrorLoadUltrasonicSpliceTerminalDistanceMappingFile_Msg, vbCrLf, UltrasonicSpliceTerminalDistanceMapping.LoadError.Message))
            End If
        Catch ex As Exception
            ex.ShowMessageBox(String.Format(ErrorStrings.DocStatMachine_ErrorImportUltrasonicSpliceTerminalDistanceMappingFile_Msg, vbCrLf, ex.Message))
        End Try

        Return Nothing
    End Function

    Private Sub ImportValidityCheckResults(fileName As String)
        Dim validityCheckContainer As ValidityCheckContainer = ValidityCheckContainer.LoadFromFile(fileName)
        If (validityCheckContainer?.PartNumber.ToLower.Trim = _documentForm.KBL.HarnessPartNumber.ToLower.Trim) OrElse (MessageBoxEx.ShowWarning(DialogStrings.ImportValidityCheckResults_Msg) = DialogResult.Yes) Then
            _utmDocument.Tools(ChecksTabToolKey.Validity.ToString).SharedProps.Visible = True
            _documentForm._validityCheckContainer = validityCheckContainer
            MessageBox.Show(DialogStrings.DocStatMachine_ValidityCheckResultsImportSuccess_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
            _documentForm._informationHub.ImportValidityCheckResults()
        End If
    End Sub

    Private Sub InitializeChecksTab()
        With _utmDocument.Ribbon
            Dim checksTab As RibbonTab = .Tabs.Add(NameOf(My.Resources.MenuButtonStrings.Checks))
            With checksTab
                .Caption = My.Resources.MenuButtonStrings.Checks
                .MergeOrder = 2

                Dim technicalGroup As RibbonGroup = .Groups.Add(NameOf(My.Resources.MenuButtonStrings.Technical))
                With technicalGroup
                    .Caption = My.Resources.MenuButtonStrings.Technical
                    .Tools.AddTool(ChecksTabToolKey.CavityAssignment.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(ChecksTabToolKey.UltrasonicSpliceTerminalDistance.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(ChecksTabToolKey.Validity.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(ChecksTabToolKey.TopologyCompare.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                End With
            End With
        End With
    End Sub

    Private Sub InitializeEditTab()
        With _utmDocument.Ribbon
            Dim editTab As RibbonTab = .Tabs.Add(NameOf(My.Resources.MenuButtonStrings.Edit2))
            With editTab
                .Caption = My.Resources.MenuButtonStrings.Edit2

                Dim redliningGroup As RibbonGroup = .Groups.Add(NameOf(My.Resources.MenuButtonStrings.Redlining))
                With redliningGroup
                    .Caption = My.Resources.MenuButtonStrings.Redlining
                    .Tools.AddTool(EditTabToolKey.EditRedlining.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(EditTabToolKey.DeleteRedlining.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(EditTabToolKey.ExportRedlining.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(EditTabToolKey.ImportRedlining.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                End With

                Dim qmStampGroup As RibbonGroup = .Groups.Add(NameOf(My.Resources.MenuButtonStrings.QMStampInformation))
                With qmStampGroup
                    .Caption = My.Resources.MenuButtonStrings.QMStampInformation
                    .Tools.AddTool(EditTabToolKey.AddQMStamp.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(EditTabToolKey.DeleteQMStamp.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(EditTabToolKey.ExportQMStamps.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(EditTabToolKey.ImportQMStamps.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                End With

                Dim memo_list_Group As RibbonGroup = .Groups.Add(NameOf(My.Resources.MenuButtonStrings.Memolist))
                With memo_list_Group
                    .Caption = My.Resources.MenuButtonStrings.Memolist
                    .Tools.AddTool(EditTabToolKey.ExportMemolist.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(EditTabToolKey.ImportMemolist.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                End With

                Dim graphicalDataCompareResultGroup As RibbonGroup = .Groups.Add(NameOf(My.Resources.MenuButtonStrings.GraphicalDataCompareResultInformation))
                With graphicalDataCompareResultGroup
                    .Caption = My.Resources.MenuButtonStrings.GraphicalDataCompareResultInformation
                    .Tools.AddTool(EditTabToolKey.ExportGraphicalDataCompareResult.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(EditTabToolKey.ImportGraphicalDataCompareResult.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                End With

                Dim technicalDataCompareResultGroup As RibbonGroup = .Groups.Add(NameOf(My.Resources.MenuButtonStrings.TechnicalDataCompareResultInformation))
                With technicalDataCompareResultGroup
                    .Caption = My.Resources.MenuButtonStrings.TechnicalDataCompareResultInformation
                    .Tools.AddTool(EditTabToolKey.ExportTechnicalDataCompareResult.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(EditTabToolKey.ImportTechnicalDataCompareResult.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                End With

                Dim cavityAssignmentGroup As RibbonGroup = .Groups.Add(NameOf(My.Resources.MenuButtonStrings.CavityAssignment))
                With cavityAssignmentGroup
                    .Caption = My.Resources.MenuButtonStrings.CavityAssignment
                    .Tools.AddTool(EditTabToolKey.ExportCavityAssignment.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                End With

                Dim validityCheckResultsGroup As RibbonGroup = .Groups.Add(NameOf(My.Resources.MenuButtonStrings.ValidityCheckResults))
                With validityCheckResultsGroup
                    .Caption = My.Resources.MenuButtonStrings.ValidityCheckResults
                    .Tools.AddTool(EditTabToolKey.ImportValidityCheckResults.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                End With
            End With
        End With
    End Sub

    Private Sub InitializeHomeTab()
        With _utmDocument.Ribbon
            Dim homeTab As RibbonTab = .Tabs.Add(NameOf(My.Resources.MenuButtonStrings.Home))
            With homeTab
                .Caption = My.Resources.MenuButtonStrings.Home

                Dim drawingGroup As RibbonGroup = .Groups.Add(NameOf(My.Resources.MenuButtonStrings.Drawing))
                With drawingGroup
                    .Caption = My.Resources.MenuButtonStrings.Drawing
                    .MergeOrder = 0
                    .Tools.AddTool(HomeTabToolKey.Pan.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(HomeTabToolKey.Refresh.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
#If DEBUG Or CONFIG = "Debug" Then
                    .Tools.AddTool(HomeTabToolKey.OpenInternalModelViewer.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
#End If
                End With

                Dim zoomGroup As RibbonGroup = .Groups.Add(NameOf(My.Resources.MenuButtonStrings.Zoom))
                With zoomGroup
                    .Caption = My.Resources.MenuButtonStrings.Zoom
                    .MergeOrder = 1
                    .Tools.AddTool(HomeTabToolKey.ZoomIn.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(HomeTabToolKey.ZoomOut.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(HomeTabToolKey.ZoomPrevious.ToString)
                    .Tools.AddTool(HomeTabToolKey.ZoomWindow.ToString)
                    .Tools.AddTool(HomeTabToolKey.ZoomExtends.ToString)
                    .Tools.AddTool(HomeTabToolKey.ZoomMagnifier.ToString)
                End With

                Dim toolsGroup As RibbonGroup = .Groups.Add(NameOf(My.Resources.MenuButtonStrings.Tools))
                With toolsGroup
                    .Caption = My.Resources.MenuButtonStrings.Tools
                    .MergeOrder = 2
                    .Tools.AddTool(HomeTabToolKey.ValidateData.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                End With

                Dim moduleConfigGroup As RibbonGroup = .Groups.Add(NameOf(My.Resources.MenuButtonStrings.ModuleConfiguration))
                With moduleConfigGroup
                    .Caption = My.Resources.MenuButtonStrings.ModuleConfiguration
                    .MergeOrder = 4
                    .Tools.AddTool(HomeTabToolKey.ModuleConfigManager.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(HomeTabToolKey.PasteFromClipboard.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                End With

                Dim bomGroup As RibbonGroup = .Groups.Add(NameOf(My.Resources.MenuButtonStrings.BOM))
                With bomGroup
                    .Caption = My.Resources.MenuButtonStrings.BOM
                    .MergeOrder = 5
                    .Tools.AddTool(HomeTabToolKey.BOMActive.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(HomeTabToolKey.BOMAll.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(HomeTabToolKey.DeviceBOM.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                End With

                Dim exportGroup As RibbonGroup = .Groups.Add(NameOf(My.Resources.MenuButtonStrings.ExcelExport))
                With exportGroup
                    .Caption = My.Resources.MenuButtonStrings.ExcelExport
                    .MergeOrder = 6
                    .Tools.AddTool(HomeTabToolKey.ExportExcelActiveDataTable.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(HomeTabToolKey.ExportExcel.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                End With
            End With
        End With
    End Sub

    Private Sub InitializeSettingsTab()
        With _utmDocument.Ribbon
            Dim settingsTab As RibbonTab = .Tabs.Add(NameOf(My.Resources.MenuButtonStrings.Settings))
            With settingsTab
                .Caption = My.Resources.MenuButtonStrings.Settings

                Dim displayGroup As RibbonGroup = .Groups.Add(NameOf(My.Resources.MenuButtonStrings.Display))
                With displayGroup
                    .Caption = My.Resources.MenuButtonStrings.Display
                    .Tools.AddTool(SettingsTabToolKey.DisplayRedlinings.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(SettingsTabToolKey.DisplayQMStamps.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(SettingsTabToolKey.Indicators.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                End With

                Dim panesGroup As RibbonGroup = .Groups.Add(NameOf(My.Resources.MenuButtonStrings.Panes))
                With panesGroup
                    .Caption = My.Resources.MenuButtonStrings.Panes
                    .Tools.AddTool(SettingsTabToolKey.DisplayDrawingsHub.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(SettingsTabToolKey.DisplayModulesHub.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(SettingsTabToolKey.DisplayInformationHub.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(SettingsTabToolKey.DisplayNavigatorHub.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(SettingsTabToolKey.DisplayMemolistHub.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(SettingsTabToolKey.DisplayLogHub.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                End With

            End With
        End With
    End Sub

    Friend Sub PasteModulePartNumbersFromClipboard()
        _documentForm.Cursor = Cursors.WaitCursor
        _documentForm.KBL.InactiveModules.Clear()

        Dim clipboardRegex As Regex = Nothing
        Try
            clipboardRegex = New Regex(_documentForm.MainForm.GeneralSettings.ClipboardModulePartNumberRegEx)
        Catch ex As Exception
#If DEBUG Or CONFIG = "Debug" Then
            Throw
#Else
            MessageBoxEx.ShowError(ErrorStrings.DocStatMachine_RegExForGenSettingsConfigInvalid_Msg)
#End If
            clipboardRegex = New Regex("\t|\r|\v|\n|[.,;|]")
        End Try

        Dim pastedStrings As New List(Of String)
        Dim pastedText As String = Clipboard.GetText()

        For Each subString As String In clipboardRegex.Split(pastedText)
            subString = subString.RegExEmptyReplace("\s")

            If (Not subString.IsNullOrEmpty) AndAlso (Not pastedStrings.Contains(subString)) Then
                pastedStrings.Add(subString)
            End If
        Next

        Dim modules As List(Of [Module]) = _documentForm.KBL.GetModules.ToList
        Dim notMatchingStrings As New List(Of String)

        For Each pastedString As String In pastedStrings
            If Not (modules.Where(Function(obj) DirectCast(obj, [Module]).Part_number.RegExEmptyReplace("\s") = pastedString).Any) AndAlso (Not notMatchingStrings.Contains(pastedString)) Then
                notMatchingStrings.Add(pastedString)
            End If
        Next

        Dim dialogResult As DialogResult = DialogResult.OK
        If notMatchingStrings.Count <> 0 Then
            Using pasteResultForm As New PasteResultForm(notMatchingStrings)
                dialogResult = pasteResultForm.ShowDialog(_documentForm)
            End Using
        End If

        If (dialogResult = System.Windows.Forms.DialogResult.OK) Then
            Dim moduleIds As New List(Of String)

            For Each [module] As [Module] In modules
                If (Not pastedStrings.Contains([module].Part_number.RegExEmptyReplace("\s"))) Then
                    _documentForm.KBL.InactiveModules.Add([module].SystemId, [module])
                Else
                    moduleIds.Add([module].SystemId)
                End If
            Next

            Dim custom_har_config As Harness_configuration = CType(_documentForm._harnessModuleConfigurations.Single(Function(harnessModuleConfig) harnessModuleConfig.ConfigurationType = HarnessModuleConfigurationType.Custom).HarnessConfigurationObject, Harness_configuration)
            custom_har_config.Modules = String.Join(" ", moduleIds)

            _documentForm.ApplyModuleConfiguration(True, True)
        End If

        ' HINT: There is a bug in Windows 7 (x64) systems when Clipboard.Clear() method will be used to delete all clipboard content.
        '       This leads to some random crashes in other currently opened applications!
        Clipboard.SetText(" ")

        _documentForm.Cursor = Cursors.Default
    End Sub

    Private Function GetDefaultFileName(type As FileNameType) As String
        Dim fileNameAndExt As String
        Select Case type
            Case FileNameType.QmStamps
                fileNameAndExt = "QMStamps.qms"
            Case FileNameType.CavityChecks
                fileNameAndExt = IO.Path.ChangeExtension("CavityAssignments", Checks.Cavities.Files.CavityChecksFile.FileExtension)
            Case FileNameType.ExcelDocument
                fileNameAndExt = IO.Path.ChangeExtension(_documentForm.Text, "xlsx")
            Case FileNameType.Redlinings
                fileNameAndExt = "Redlinings.rdl"
            Case FileNameType.Memolist
                fileNameAndExt = "Memolist.mlst"
            Case FileNameType.CompareResult
                fileNameAndExt = "Compare_result_information"
            Case Else
                Throw New NotImplementedException($"Unimplemented file name type: {type.ToString}")
        End Select

        If (_documentForm.KBL.GetChanges.Any()) Then
            Return String.Format("{0}{1}{2}_{3}_{4}_{5}", Now.Year, Format(Now.Month, "00"), Format(Now.Day, "00"), Replace(_documentForm.KBL.HarnessPartNumber, " ", String.Empty), _documentForm.KBL.GetChanges.Max(Function(change) change.Id), fileNameAndExt)
        Else
            Return String.Format("{0}{1}{2}_{3}_{4}", Now.Year, Format(Now.Month, "00"), Format(Now.Day, "00"), Replace(_documentForm.KBL.HarnessPartNumber, " ", String.Empty), fileNameAndExt)
        End If
    End Function

    Private Sub HideAnalysisView()
        _documentForm.FilterAnalysisViewObjects(Nothing)
        _documentForm.ApplyModuleConfiguration(False, False)
        _documentForm._informationHub?.FilterAnalysisViewObjects(Nothing)

        If (_editIssuesForm IsNot Nothing) AndAlso (_editIssuesForm.Visible) Then
            _editIssuesForm.Close()
        End If

        Me.Tools.Home.AnalysisShowDryWetBtn.Checked = False
        Me.Tools.Home.AnalysisShowEyeletsBtn.Checked = False
        Me.Tools.Home.AnalysisShowPlatingMatBtn.Checked = False
        Me.Tools.Home.AnalysisShowProtectionsBtn.Checked = False
        Me.Tools.Home.AnalysisShowPartnumbersBtn.Checked = False
        Me.Tools.Home.AnalysisShowQMIssuesBtn.Checked = False
        Me.Tools.Home.AnalysisShowSplicesBtn.Checked = False

        _isAnalysisActive = False
    End Sub

    Friend Sub ShowDeviceBomInformation()
        Using frm As New DeviceBOM()
            frm.ShowDialog(_documentForm.MainForm)
        End Using
    End Sub

    Friend Sub ShowBOMInformation(onlyActiveModules As Boolean)
        If (_bomForm Is Nothing) OrElse (_bomForm.IsDisposed) Then
            _bomForm = New BOMForm(_documentForm)
        End If

        If (Not onlyActiveModules) Then
            _bomForm.ActiveModulePartNumbers = Nothing
        Else
            Dim activeModulePartNumbers As New List(Of String)

            For Each [module] As [Module] In _documentForm.KBL.GetModules
                If (Not _documentForm.KBL.InactiveModules.ContainsKey([module].SystemId)) Then
                    activeModulePartNumbers.Add(BOMForm.AlignPartnumber([module].Part_number))
                End If
            Next

            _bomForm.ActiveModulePartNumbers = activeModulePartNumbers
        End If

        If _bomForm.Visible Then
            _bomForm.Close()
        End If

        _bomForm.Show(_documentForm)
    End Sub

    Private Sub ShowModuleConfigurationManager()
        With DirectCast(_documentForm.ParentForm, MainForm)
            If (.MainStateMachine._compareForm IsNot Nothing AndAlso .MainStateMachine._compareForm.Visible) OrElse (.MainStateMachine._graphicalCompareForm IsNot Nothing AndAlso .MainStateMachine._graphicalCompareForm.Visible) Then
                If MessageBoxEx.ShowQuestion(DialogStrings.DocStatMachine_OpenModConfig_Msg) = DialogResult.No Then
                    Exit Sub
                End If
            End If

            If (.MainStateMachine._compareForm IsNot Nothing) AndAlso (.MainStateMachine._compareForm.Visible) Then
                .MainStateMachine._compareForm.Close()
                .MainStateMachine._compareForm.Dispose()
            End If

            If (.MainStateMachine._graphicalCompareForm IsNot Nothing) AndAlso (.MainStateMachine._graphicalCompareForm.Visible) Then
                .MainStateMachine._graphicalCompareForm.Close()

                If (Not .MainStateMachine._graphicalCompareForm.Visible) Then
                    .MainStateMachine._graphicalCompareForm.Dispose()
                End If
            End If
        End With

        If _documentForm._harnessModuleConfigurations.Count > 0 Then
            _moduleConfigForm = New ModuleConfigForm(DirectCast(_documentForm.ParentForm, MainForm).GeneralSettings.ClipboardModulePartNumberRegEx, _documentForm._harnessModuleConfigurations, _documentForm.KBL)

            If (_moduleConfigForm.ShowDialog(_documentForm) = DialogResult.OK) AndAlso (_moduleConfigForm.IsDirty) Then
                Dim activeHarModuleConfig As HarnessModuleConfiguration = CType(_documentForm._harnessModuleConfigurations.GetActiveConfiguration, HarnessModuleConfiguration)

                _documentForm.KBL.InactiveModules.Clear()

                Dim moduleIds As List(Of String) = activeHarModuleConfig.HarnessConfiguration.Modules.SplitSpace.ToList

                For Each [module] As [Module] In _documentForm.KBL.GetModules
                    If (Not moduleIds.Contains([module].SystemId)) Then
                        _documentForm.KBL.InactiveModules.Add([module].SystemId, [module])
                    End If
                Next

                Using _documentForm.EnableWaitCursor
                    If Not activeHarModuleConfig.HarnessConfiguration.Part_number.RegExReplace("\s", String.Empty).IsNullOrEmpty Then
                        _documentForm._modulesHub.InitializeConfigurations(_documentForm._harnessModuleConfigurations, activeHarModuleConfig.HarnessConfiguration.Part_number)
                        _documentForm.ApplyModuleConfiguration(True, True, activeHarModuleConfig.HarnessConfiguration)
                    Else
                        If Not activeHarModuleConfig.HarnessConfiguration.Abbreviation.RegExReplace("\s", String.Empty).IsNullOrEmpty Then
                            _documentForm._modulesHub.InitializeConfigurations(_documentForm._harnessModuleConfigurations, activeHarModuleConfig.HarnessConfiguration.Abbreviation)
                            _documentForm.ApplyModuleConfiguration(True, True, activeHarModuleConfig.HarnessConfiguration)
                        Else
                            If Not activeHarModuleConfig.HarnessConfiguration.Description.RegExReplace("\s", String.Empty).IsNullOrEmpty Then
                                _documentForm._modulesHub.InitializeConfigurations(_documentForm._harnessModuleConfigurations, activeHarModuleConfig.HarnessConfiguration.Description)
                                _documentForm.ApplyModuleConfiguration(True, True, activeHarModuleConfig.HarnessConfiguration)
                            End If
                        End If
                    End If
                End Using

                For Each moduleConfigWithCircles As Dictionary(Of HarnessModuleConfiguration, PackagingCircle) In _documentForm._calculatedSegmentDiameters.Values
                    moduleConfigWithCircles.Clear()
                Next
            End If
        Else
            MessageBoxEx.ShowWarning(DialogStrings.DocStatMachine_NoModConfig_Msg)
        End If

        _moduleConfigForm?.Dispose()
    End Sub

    Private Sub _activeDrawingCanvas_CanvasMouseClick(sender As Object, e As MouseEventArgs) Handles _activeDrawingCanvas.CanvasMouseClick
        If (e.Button = MouseButtons.Left) Then
            TryCast(_documentForm.MdiParent, MainForm)?.TopologyHub?.SelectCompartments({_documentForm.KBL.HarnessPartNumber}, Nothing)

            Using _documentForm.EnableWaitCursor
                _documentForm.OnCanvasSelectionChanged(False, _documentForm.ActiveDrawingCanvas.CanvasSelection)
            End Using
        End If
    End Sub

    Private Sub _activeDrawingCanvas_CanvasSelectionChanged(sender As Object) Handles _activeDrawingCanvas.CanvasSelectionChanged
        TryCast(_documentForm.MdiParent, MainForm)?.TopologyHub?.SelectCompartments({_documentForm.KBL.HarnessPartNumber}, Nothing)

        Using _documentForm.EnableWaitCursor
            _documentForm.OnCanvasSelectionChanged(True, _documentForm.ActiveDrawingCanvas.CanvasSelection)
        End Using
    End Sub

    Private Sub _bomForm_SearchPartNo(partNo As String) Handles _bomForm.SearchPartNo
        With DirectCast(_documentForm.MdiParent, MainForm).SearchMachine
            .CreateSearchFormWithPredefinedSearchString(.BeginsWithEnabled, .CaseSensitiveEnabled, True, partNo)
        End With
    End Sub

    Private Sub _moduleConfigForm_LogMessage(sender As ModuleConfigForm, e As LogEventArgs) Handles _moduleConfigForm.LogMessage
        RaiseEvent LogMessage(_documentForm, e)
    End Sub

    Private Sub _udmDocument_PaneHidden(sender As Object, e As PaneHiddenEventArgs) Handles _udmDocument.PaneHidden
        _utmDocument.BeginUpdate()
        Using _utmDocument.EventManager.ProtectProperty(NameOf(ToolbarsEventManager.AllEventsEnabled), False)
            DirectCast(_utmDocument.Tools(String.Format("Display{0}", e.Pane.Key)), StateButtonTool).Checked = False
        End Using
        _utmDocument.EndUpdate()
    End Sub

    Private Sub _ultrasonicSpliceTerminalDistanceForm_UltrasonicSpliceSelectionChanged(sender As Object, e As InformationHubEventArgs) Handles _ultrasonicSpliceTerminalDistanceForm.UltrasonicSpliceSelectionChanged
        With _documentForm
            Using .EnableWaitCursor()
                If (._informationHub IsNot Nothing) AndAlso (.KBL.Id = e.KblMapperSourceId) Then
                    .SelectRowsInGrids(e.ObjectIds, True, True)
                End If
            End Using
        End With
    End Sub

    ' Dim activeAnalysisButton As StateButtonTool
    Private Sub _utmDocument_ToolClick(sender As Object, e As ToolClickEventArgs) Handles _utmDocument.ToolClick
        Select Case e.Tool.Key
            Case ChecksTabToolKey.CavityAssignment.ToString
                If (_documentForm IsNot Nothing) Then
                    If (_documentForm.MainForm IsNot Nothing AndAlso Not _documentForm.MainForm.IsDisposed) Then
                        _documentForm.MainForm.ShowActiveCavityChecker()
                    Else
                        Debug.Fail("Can't show cavity content checker because main form is empty or disposed!")
                    End If
                Else
                    Debug.Fail("Can't show cavity content checker because document is empty!")
                End If
            Case ChecksTabToolKey.UltrasonicSpliceTerminalDistance.ToString
                Dim ultrasonicSpliceTerminalDistanceMapping As UltrasonicSpliceTerminalDistanceMapping = Nothing
                Using ofdUltrasonicSpliceTerminalDistanceMapping As New OpenFileDialog
                    With ofdUltrasonicSpliceTerminalDistanceMapping
                        .DefaultExt = KnownFile.XML
                        .FileName = String.Empty
                        .Filter = "E³.HarnessAnalyzer ultrasonic splice - terminal distance mapping files (*.xml;*.xls;*.xlsx)|*.xml;*.xls;*.xlsx"
                        .Title = DialogStrings.DocStatMachine_ImportUSSpliceTerminalDistanceMappingFile_Title

                        If (ofdUltrasonicSpliceTerminalDistanceMapping.ShowDialog(_documentForm) = System.Windows.Forms.DialogResult.OK) Then
                            ultrasonicSpliceTerminalDistanceMapping = ImportUltrasonicSpliceTerminalDistanceMapping(.FileName)
                        End If
                    End With
                End Using

                If (ultrasonicSpliceTerminalDistanceMapping IsNot Nothing) Then
                    If (_ultrasonicSpliceTerminalDistanceForm IsNot Nothing) AndAlso (_ultrasonicSpliceTerminalDistanceForm.Visible) Then
                        _ultrasonicSpliceTerminalDistanceForm.Close()
                    End If

                    If (_ultrasonicSpliceTerminalDistanceForm Is Nothing) OrElse (_ultrasonicSpliceTerminalDistanceForm.IsDisposed) Then
                        _ultrasonicSpliceTerminalDistanceForm = New UltrasonicSpliceTerminalDistanceForm(_documentForm.MainForm.GeneralSettings.DefaultWireLengthType, _documentForm.KBL, _documentForm.MainForm.GeneralSettings.UltrasonicSpliceIdentifiers, ultrasonicSpliceTerminalDistanceMapping)
                    End If

                    _ultrasonicSpliceTerminalDistanceForm.Show(_documentForm)
                End If
            Case ChecksTabToolKey.Validity.ToString
                If (_documentForm IsNot Nothing) AndAlso (_documentForm._validityCheckContainer IsNot Nothing) Then
                    If (_validityCheckForm Is Nothing) OrElse (_validityCheckForm.IsDisposed) Then
                        _validityCheckForm = New ValidityCheckForm(_documentForm.KBL, _documentForm._validityCheckContainer)
                    End If

                    If (_validityCheckForm.Visible) Then
                        _validityCheckForm.Hide()
                    End If

                    _validityCheckForm.Show(_documentForm)
                End If
            Case ChecksTabToolKey.TopologyCompare.ToString
                TopologyCompareButtonAction()
            Case EditTabToolKey.DeleteQMStamp.ToString
                _documentForm._informationHub.DeleteQMStamp(True)
            Case EditTabToolKey.DeleteRedlining.ToString
                _documentForm._informationHub.DeleteRedlining()
            Case EditTabToolKey.AddQMStamp.ToString
                FinishPanAction()
                FinishMagnifierAction()
                Me.Tools.Settings.DisplayQMStampsBtn.Checked = True
                _documentForm._informationHub.AddOrEditQMStamp()
            Case EditTabToolKey.EditRedlining.ToString
                FinishPanAction()
                FinishMagnifierAction()
                Me.Tools.Settings.DisplayRedliningsBtn.Checked = True
                _documentForm._informationHub.AddOrEditRedlining(Nothing)
            Case EditTabToolKey.ExportGraphicalDataCompareResult.ToString, EditTabToolKey.ExportTechnicalDataCompareResult.ToString
                If (_documentForm.GetCompareResultInfo(If(e.Tool.Key = EditTabToolKey.ExportGraphicalDataCompareResult.ToString, [Lib].IO.Files.Hcv.KnownContainerFileFlags.GCRI, [Lib].IO.Files.Hcv.KnownContainerFileFlags.TCRI)).CheckedCompareResults.Count <> 0) Then
                    Using sfdCompareResultInformation As New SaveFileDialog
                        With sfdCompareResultInformation
                            .DefaultExt = If(e.Tool.Key = EditTabToolKey.ExportGraphicalDataCompareResult.ToString, ".gcri", ".tcri")
                            .FileName = GetDefaultFileName(FileNameType.CompareResult)
                            .Filter = If(e.Tool.Key = EditTabToolKey.ExportGraphicalDataCompareResult.ToString, "E³.HarnessAnalyzer graphical data compare result information files (*.gcri)|*.gcri", "E³.HarnessAnalyzer technical data compare result information files (*.tcri)|*.tcri")
                            .Title = If(e.Tool.Key = EditTabToolKey.ExportGraphicalDataCompareResult.ToString, DialogStrings.DocStatMachine_SaveGraphCompResultInfoFile_Title, DialogStrings.DocStatMachine_SaveTechCompResultInfoFile_Title)
                        End With

                        If sfdCompareResultInformation.ShowDialog(_documentForm) = System.Windows.Forms.DialogResult.OK Then
                            Dim resultInfo As CheckedCompareResultInformation = _documentForm.GetCompareResultInfo(If(e.Tool.Key = EditTabToolKey.ExportGraphicalDataCompareResult.ToString, [Lib].IO.Files.Hcv.KnownContainerFileFlags.GCRI, [Lib].IO.Files.Hcv.KnownContainerFileFlags.TCRI))
                            resultInfo.Name = IO.Path.GetFileNameWithoutExtension(sfdCompareResultInformation.FileName)
                            resultInfo.Save(sfdCompareResultInformation.FileName)

                            Select Case _documentForm.File.Type
                                Case KnownContainerFileFlags.KBL, KnownContainerFileFlags.VEC
                                    _documentForm.IsDirty = False
                            End Select

                            MessageBoxEx.ShowInfo(If(e.Tool.Key = EditTabToolKey.ExportGraphicalDataCompareResult.ToString, DialogStrings.DocStatMachine_GraphCompResultInfoExportSuccess_Msg, DialogStrings.DocStatMachine_TechCompResultInfoExportSuccess_Msg))
                        End If
                    End Using
                    _documentForm.OnMessage(New MessageEventArgs(MessageEventArgs.MsgType.HideProgress))
                Else
                    MessageBoxEx.ShowInfo(If(e.Tool.Key = EditTabToolKey.ExportGraphicalDataCompareResult.ToString, DialogStrings.DocStatMachine_NoGraphCompResultInfoInDoc_Msg, DialogStrings.DocStatMachine_NoTechCompResultInfoInDoc_Msg))
                End If
            Case EditTabToolKey.ExportMemolist.ToString
                _documentForm._memolistHub.UpdateData(_documentForm.MemoList.MemoItems)

                If (_documentForm.MemoList.MemoItems.Count <> 0) Then
                    Using sfdMemolist As New SaveFileDialog
                        With sfdMemolist
                            .DefaultExt = ".mlst"
                            .FileName = GetDefaultFileName(FileNameType.Memolist)
                            .Filter = "E³.HarnessAnalyzer memolist files (*.mlst)|*.mlst"
                            .Title = DialogStrings.DocStatMachine_SaveMemolistToFile_Title
                        End With

                        If sfdMemolist.ShowDialog(_documentForm) = System.Windows.Forms.DialogResult.OK Then
                            _documentForm.MemoList.Name = IO.Path.GetFileNameWithoutExtension(sfdMemolist.FileName)
                            _documentForm.MemoList.Save(sfdMemolist.FileName)

                            Select Case _documentForm.File.Type
                                Case KnownContainerFileFlags.KBL, KnownContainerFileFlags.VEC
                                    _documentForm.IsDirty = False
                            End Select

                            MessageBoxEx.ShowInfo(DialogStrings.DocStatMachine_MemolistExportSuccess_Msg)
                        End If
                    End Using
                Else
                    MessageBoxEx.ShowInfo(DialogStrings.DocStatMachine_NoMemolistInDoc_Msg)
                End If
            Case EditTabToolKey.ExportQMStamps.ToString
                If (_documentForm.QmStamps.Stamps.Count <> 0) Then
                    Using sfdQMStamps As New SaveFileDialog
                        With sfdQMStamps
                            .DefaultExt = ".mlst"
                            .FileName = GetDefaultFileName(FileNameType.QmStamps)
                            .Filter = "E³.HarnessAnalyzer QM stamp information (*.qms)|*.qms"
                            .Title = DialogStrings.DocStatMachine_SaveQMStampsToFile_Title
                        End With

                        If (sfdQMStamps.ShowDialog(_documentForm) = System.Windows.Forms.DialogResult.OK) Then
                            _documentForm.QmStamps.Name = IO.Path.GetFileNameWithoutExtension(sfdQMStamps.FileName)
                            _documentForm.QmStamps.Save(sfdQMStamps.FileName)

                            Select Case _documentForm.File.Type
                                Case KnownContainerFileFlags.KBL, KnownContainerFileFlags.VEC
                                    _documentForm.IsDirty = False
                            End Select

                            MessageBoxEx.ShowInfo(DialogStrings.DocStatMachine_QMStampsExportSuccess_Msg)
                        End If
                    End Using
                Else
                    MessageBoxEx.ShowInfo(DialogStrings.DocStatMachine_NoQMStampInDoc_Msg)
                End If
            Case EditTabToolKey.ExportCavityAssignment.ToString
                DoExportCavities()
            Case EditTabToolKey.ExportRedlining.ToString
                If (_documentForm.redliningInformation.Redlinings.Count > 0) Then
                    Using sfdRedliningInfo As New SaveFileDialog
                        With sfdRedliningInfo
                            .DefaultExt = ".rdl"
                            .FileName = GetDefaultFileName(FileNameType.Redlinings)
                            .Filter = "E³.HarnessAnalyzer redlining information files (*.rdl)|*.rdl"
                            .Title = DialogStrings.DocStatMachine_SaveRedliningToFile_Title
                        End With

                        If (sfdRedliningInfo.ShowDialog(_documentForm) = System.Windows.Forms.DialogResult.OK) Then
                            ExportRedlinings(sfdRedliningInfo.FileName)
                        End If
                    End Using
                Else
                    MessageBox.Show(DialogStrings.DocStatMachine_NoRedliningInDoc_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            Case EditTabToolKey.ImportGraphicalDataCompareResult.ToString, EditTabToolKey.ImportTechnicalDataCompareResult.ToString
                Using ofdCompareResultInformation As New OpenFileDialog
                    With ofdCompareResultInformation
                        .FileName = String.Empty
                        .DefaultExt = If(e.Tool.Key = Me.Tools.Edit.ImportGraphicalDataCompareResult.Key, ".gcri", ".tcri")
                        .Filter = If(e.Tool.Key = Me.Tools.Edit.ImportGraphicalDataCompareResult.Key, "E³.HarnessAnalyzer graphical data compare result information files (*.gcri)|*.gcri", "E³.HarnessAnalyzer technical data compare result information files (*.tcri)|*.tcri")
                        .Title = If(e.Tool.Key = Me.Tools.Edit.ImportGraphicalDataCompareResult.Key, DialogStrings.DocStatMachine_ImportGraphCompResultInfoFile_Title, DialogStrings.DocStatMachine_ImportTechCompResultInfoFile_Title)
                    End With

                    If ofdCompareResultInformation.ShowDialog(_documentForm) = System.Windows.Forms.DialogResult.OK Then
                        ImportCompareResultInformation(ofdCompareResultInformation.FileName)
                    End If
                End Using
            Case EditTabToolKey.ImportMemolist.ToString
                Using ofdMemolist As New OpenFileDialog
                    With ofdMemolist
                        .FileName = String.Empty
                        .DefaultExt = ".mlst"
                        .Filter = "E³.HarnessAnalyzer memolist files (*.mlst)|*.mlst"
                        .Title = DialogStrings.DocStatMachine_ImportMemolistFile_Title
                    End With

                    If (ofdMemolist.ShowDialog(_documentForm) = System.Windows.Forms.DialogResult.OK) Then
                        ImportMemolist(ofdMemolist.FileName)
                    End If
                End Using
            Case EditTabToolKey.ImportQMStamps.ToString
                Using ofdQMStamps As New OpenFileDialog
                    With ofdQMStamps
                        .FileName = String.Empty
                        .DefaultExt = ".qms"
                        .Filter = "E³.HarnessAnalyzer QM stamp information files (*.qmps)|*.qms"
                        .Title = DialogStrings.DocStatMachine_ImportQMStampsFile_Title
                    End With

                    If (ofdQMStamps.ShowDialog(_documentForm) = System.Windows.Forms.DialogResult.OK) Then
                        ImportQMStamps(ofdQMStamps.FileName)
                    End If
                End Using
            Case EditTabToolKey.ImportRedlining.ToString
                Using ofdRedliningInfo As New OpenFileDialog
                    With ofdRedliningInfo
                        .FileName = String.Empty
                        .DefaultExt = ".rdl"
                        .Filter = "E³.HarnessAnalyzer redlining information files (*.rdl)|*.rdl"
                        .Title = DialogStrings.DocStatMachine_ImportRedliningFile_Title
                    End With

                    If (ofdRedliningInfo.ShowDialog(_documentForm) = System.Windows.Forms.DialogResult.OK) Then
                        ImportRedlinings(ofdRedliningInfo.FileName)
                    End If
                End Using
            Case EditTabToolKey.ImportValidityCheckResults.ToString
                Using ofdValidityCheckResults As New OpenFileDialog
                    With ofdValidityCheckResults
                        .FileName = String.Empty
                        .DefaultExt = KnownFile.VCC.Trim("."c)
                        .Filter = "E³.HarnessAnalyzer validation check result files (*.vcc)|*.vcc"
                        .Title = DialogStrings.DocStatMachine_ImportValidityCheckResultsFile_Title
                    End With

                    If (ofdValidityCheckResults.ShowDialog(_documentForm) = System.Windows.Forms.DialogResult.OK) Then
                        ImportValidityCheckResults(ofdValidityCheckResults.FileName)
                    End If
                End Using

            Case HomeTabToolKey.BOMActive.ToString
                ShowBOMInformation(True)
            Case HomeTabToolKey.DeviceBOM.ToString
                ShowDeviceBomInformation()
            Case HomeTabToolKey.BOMAll.ToString
                ShowBOMInformation(False)
            Case HomeTabToolKey.ExportExcel.ToString
                Using sfdExcel As New SaveFileDialog
                    With sfdExcel
                        .FileName = GetDefaultFileName(FileNameType.ExcelDocument)
                        .DefaultExt = KnownFile.XLSX.Trim("."c)
                        .Filter = "Excel files (*.xlsx)|*.xlsx|Excel files (97-2003) (*.xls)|*.xls"
                        .Title = DialogStrings.DocStatMachine_ExportToExcelFile_Title
                        If .ShowDialog(_documentForm) = DialogResult.OK Then
                            Try
                                Using _documentForm.EnableWaitCursor
                                    _documentForm._informationHub.ExportAllToExcel(.FileName, Nothing)
                                End Using
                            Catch ex As Exception
                                ex.ShowMessageBox(String.Format(ErrorStrings.DocStatMachine_ErrorExportExcel_Msg, vbCrLf, ex.Message))
                            End Try
                        End If
                    End With
                End Using
            Case HomeTabToolKey.ExportExcelActiveDataTable.ToString
                _documentForm._informationHub.ExportSelectedGridDataToExcel()
            Case HomeTabToolKey.ModuleConfigManager.ToString
                ShowModuleConfigurationManager()
            Case HomeTabToolKey.Pan.ToString
                PanActionClicked(e)
#If DEBUG Or CONFIG = "Debug" Then
            Case HomeTabToolKey.OpenInternalModelViewer.ToString
                OpenDocumentInInternalModelViewerApp()
#End If
            Case HomeTabToolKey.PasteFromClipboard.ToString
                PasteModulePartNumbersFromClipboard()
            Case HomeTabToolKey.Refresh.ToString
                If (_documentForm.ActiveDrawingCanvas IsNot Nothing) Then
                    _documentForm.ActiveDrawingCanvas.vdCanvas.BaseControl.ActiveDocument.Redraw(True)
                End If
            Case HomeTabToolKey.ValidateData.ToString
                _documentForm.ValidateKBL(_documentForm.File.KblDocument, True)
            Case HomeTabToolKey.ZoomExtends.ToString
                If _documentForm?.IsDocument3DActive Then
                    _documentForm._D3DControl.ZoomFit()
                ElseIf (_documentForm.ActiveDrawingCanvas IsNot Nothing) Then
                    _documentForm.ActiveDrawingCanvas.vdCanvas.BaseControl.ActiveDocument.CommandAction.Zoom("E", 0, 0)
                End If
            Case HomeTabToolKey.ZoomIn.ToString
                _documentForm.ZoomInAction()
            Case HomeTabToolKey.ZoomOut.ToString
                _documentForm.ZoomOutAction()
            Case HomeTabToolKey.ZoomPrevious.ToString
                If (_documentForm.ActiveDrawingCanvas IsNot Nothing) Then
                    _documentForm.ActiveDrawingCanvas.vdCanvas.BaseControl.ActiveDocument.CommandAction.Zoom("P", 0, 0)
                End If
            Case HomeTabToolKey.ZoomWindow.ToString
                _documentForm.ZoomWindowAction()
            Case HomeTabToolKey.ZoomMagnifier.ToString
                MagnifierActionClicked(e)
            Case SettingsTabToolKey.DisplayDrawingsHub.ToString, SettingsTabToolKey.DisplayInformationHub.ToString, SettingsTabToolKey.DisplayLogHub.ToString, SettingsTabToolKey.DisplayMemolistHub.ToString, SettingsTabToolKey.DisplayModulesHub.ToString, SettingsTabToolKey.DisplayNavigatorHub.ToString
                If (DirectCast(e.Tool, StateButtonTool).Checked) Then
                    If e.Tool.Key = Me.Tools.Settings.DisplayNavigatorHub?.Key Then
                        If (_documentForm.Panes.NavigatorHub.Control Is Nothing) Then
                            _documentForm.Panes.NavigatorHub.Control = _documentForm._navigatorHub
                        End If

                        If _documentForm.ActiveDrawingCanvas IsNot Nothing Then
                            _documentForm._navigatorHub.LoadView()
                        End If
                    End If

                    _udmDocument.PaneFromKey(Replace(e.Tool.Key, "Display", String.Empty)).Show()
                Else
                    _udmDocument.PaneFromKey(Replace(e.Tool.Key, "Display", String.Empty)).Close()
                End If
            Case SettingsTabToolKey.DisplayQMStamps.ToString
                For Each drawingTab As UltraTab In _documentForm.utcDocument.Tabs
                    If (TypeOf drawingTab.TabPage.Controls(0) Is DrawingCanvas) Then
                        Dim document As VectorDraw.Professional.vdObjects.vdDocument = DirectCast(drawingTab.TabPage.Controls(0), DrawingCanvas).vdCanvas.BaseControl.ActiveDocument
                        Dim stampLayer As VectorDraw.Professional.vdPrimaries.vdLayer = document.Layers.FindName(VDRAW_LAYER_QMSTAMPS_NAME)
                        If (stampLayer IsNot Nothing) Then
                            stampLayer.Frozen = Not DirectCast(e.Tool, StateButtonTool).Checked

                            For Each vdStamp As VdStamp In stampLayer.GetReferenceObjects
                                vdStamp.visibility = CType(Not DirectCast(e.Tool, StateButtonTool).Checked, VectorDraw.Professional.vdPrimaries.vdFigure.VisibilityEnum)
                            Next

                            stampLayer.Document.Invalidate()
                        End If
                    End If
                Next
            Case SettingsTabToolKey.DisplayRedlinings.ToString
                For Each drawingTab As UltraTab In _documentForm.utcDocument.Tabs
                    If (TypeOf drawingTab.TabPage.Controls(0) Is DrawingCanvas) Then
                        If (DirectCast(drawingTab.TabPage.Controls(0), DrawingCanvas).vdCanvas.BaseControl.ActiveDocument.Layers.FindName(VDRAW_LAYER_REDLININGS_NAME) IsNot Nothing) Then
                            DirectCast(drawingTab.TabPage.Controls(0), DrawingCanvas).vdCanvas.BaseControl.ActiveDocument.Layers.FindName(VDRAW_LAYER_REDLININGS_NAME).Frozen = Not DirectCast(e.Tool, StateButtonTool).Checked
                        End If

                        If (DirectCast(drawingTab.TabPage.Controls(0), DrawingCanvas).vdCanvas.BaseControl.ActiveDocument.Layers.FindName(VDRAW_LAYER_REDLININGS_BACKGROUND_NAME) IsNot Nothing) Then
                            DirectCast(drawingTab.TabPage.Controls(0), DrawingCanvas).vdCanvas.BaseControl.ActiveDocument.Layers.FindName(VDRAW_LAYER_REDLININGS_BACKGROUND_NAME).Frozen = Not DirectCast(e.Tool, StateButtonTool).Checked
                        End If

                        DirectCast(drawingTab.TabPage.Controls(0), DrawingCanvas).vdCanvas.BaseControl.ActiveDocument.Invalidate()
                    Else
                        If CType(e.Tool, StateButtonTool).Checked Then
                            _documentForm.CreateRedlinings()
                        Else
                            _documentForm.HideRedlinings()
                        End If
                    End If
                Next
            Case SettingsTabToolKey.Indicators.ToString
        End Select
    End Sub

#If DEBUG Or CONFIG = "Debug" Then
    Private Sub OpenDocumentInInternalModelViewerApp()
        Static modelViewerPath As String = "C:\Program Files\Zuken\E3.ModelViewer\E3.ModelViewer.exe"
        If IO.File.Exists(modelViewerPath) Then
            Try
                ProcessEx.Start(modelViewerPath, $"/Open ""{_documentForm.File.FullName}""", IO.Path.GetDirectoryName(modelViewerPath))
            Catch ex As Exception
                ex.ShowMessageBox(Me._documentForm, "Error opening modelViewer: " + ex.GetInnerOrDefaultMessage)
            End Try
        Else
            MessageBoxEx.ShowError(Me._documentForm, $"ModelViewer not found in default path! ({modelViewerPath})")
        End If
    End Sub
#End If

    Private Sub PanActionClicked(e As ToolClickEventArgs)
        If Me.Tools.Home.ZoomMagnifierBtn?.Checked AndAlso _documentForm.MagnifierAction IsNot Nothing Then
            _documentForm.MagnifierAction.CancelAction(_documentForm.MagnifierAction)
        End If
        _documentForm.CancelMagnifierIn3D()
        _documentForm.OnMagnifierActionDisabled(New EventArgs)
        _documentForm.HandlePanAction(DirectCast(e.Tool, StateButtonTool).Checked)
    End Sub

    Private Sub MagnifierActionClicked(e As ToolClickEventArgs)
        If Me.Tools.Home.PanBtn?.Checked AndAlso _documentForm.PanAction IsNot Nothing Then
            _documentForm.PanAction.CancelAction(_documentForm.PanAction)
        End If

        _documentForm.OnPanActionDisabled(New EventArgs)
        _documentForm.CancelPanIn3D()
        _documentForm.HandleMagnifierAction(Me.Tools.Home.ZoomMagnifierBtn.Checked)
    End Sub

    Private Sub FinishPanAction()
        If _documentForm.PanAction IsNot Nothing Then
            If Me.Tools.Home.PanBtn?.Checked Then
                Me.Tools.Home.PanBtn.Checked = False
            End If
        End If
    End Sub

    Private Sub FinishMagnifierAction()
        If _documentForm.MagnifierAction IsNot Nothing Then
            _documentForm.MagnifierAction.FinishAction(_documentForm.MagnifierAction)
        End If
    End Sub

    Private Sub TopologyCompareButtonAction()
        If (_documentForm IsNot Nothing) Then
            If _topologyCompareForm IsNot Nothing AndAlso _topologyCompareForm.Visible Then
                If MessageBoxEx.ShowQuestion(_documentForm.MainForm, My.Resources.TopologyCompareTexts.StartNewCompareQuestion) = DialogResult.No Then
                    Return
                End If
                _topologyCompareForm.Close()
            End If

            Using topoCompareWizardForm As New TopologyCompareWizardForm()
                Try
                    topoCompareWizardForm.UseSwapDetection = My.Settings.UseSwapDetection
                    topoCompareWizardForm.LengthTolerance = My.Settings.LengthToleranceForCompare
                    topoCompareWizardForm.HcvLeft = _documentForm.File
                    topoCompareWizardForm.SelectedCompareLengthClass = CType(My.Settings.TopoCompareCompLengthClass, E3.Lib.Model.LengthClass)
                    topoCompareWizardForm.SelectedRefLengthClass = CType(My.Settings.TopoCompareRefLengthClass, E3.Lib.Model.LengthClass)

                    Dim result As DialogResult = topoCompareWizardForm.ShowDialog(Me._documentForm.MainForm)

                    My.Settings.TopoCompareCompLengthClass = topoCompareWizardForm.SelectedCompareLengthClass ' always save the length class selection because this seams to be more convinient for the user
                    My.Settings.TopoCompareRefLengthClass = topoCompareWizardForm.SelectedRefLengthClass

                    If result = DialogResult.OK Then
                        My.Settings.UseSwapDetection = topoCompareWizardForm.UseSwapDetection
                        My.Settings.LengthToleranceForCompare = topoCompareWizardForm.LengthTolerance

                        Dim document As CompareDocument = topoCompareWizardForm.GetDocument()
                        ShowAndStartCompare(document, topoCompareWizardForm.UseSwapDetection)
                    End If

                Catch ex As Exception
                    ex.ShowMessageBox(Me._documentForm.MainForm, String.Format(ErrorStrings.TopologyCompare_ErrorShowingWizard, ex.GetInnerOrDefaultMessage))
                End Try
            End Using
        End If
    End Sub

    Private Async Sub ShowAndStartCompare(compareDocument As CompareDocument, useSwapDetection As Boolean)
        Await _semShowAndStartCompare.WaitAsync
        Try
            Using comparingForm As ComparingProgressForm = New ComparingProgressForm()
                Dim cancellationRequested As Boolean = False
                Dim comparingForm_Cancelling As EventHandler =
                    Sub()
                        cancellationRequested = True
                        compareDocument.Cancel()
                    End Sub

                AddHandler comparingForm.Cancelling, comparingForm_Cancelling

                Dim compareFailed As Exception = Nothing
                Dim openDocumentResult As WorkResult = Nothing

                If (Not compareDocument?.IsOpen) Then
                    'compareDocument.ProgressChanged += progressChanged;
                    ' _cancellationTimer.Stop();
                    'progressForm.Continuous = true;
                    Dim tsk1 As Task(Of WorkResult)
                    If (compareDocument.IsBusy) Then
                        tsk1 = Task.Factory.StartNew(Of WorkResult)(
                            Function()
                                System.Threading.SpinWait.SpinUntil(Function() Not compareDocument.IsBusy)
                                Return Nothing
                            End Function)
                    Else
                        tsk1 = compareDocument.OpenAsync()
                    End If

                    Await tsk1.ShowDialogDelay(comparingForm, _documentForm.MainForm, TimeSpan.FromSeconds(1))
                    If tsk1.Result IsNot Nothing Then
                        openDocumentResult = tsk1.Result
                    End If

                    'compareDocument.ProgressChanged -= progressChanged;
                End If

                RemoveHandler comparingForm.Cancelling, comparingForm_Cancelling

                If (Not cancellationRequested) Then
                    If (compareFailed Is Nothing AndAlso (openDocumentResult Is Nothing OrElse openDocumentResult.Success)) Then
                        ' toolStripProgressBar1.Visible = True
                        _topologyCompareForm = New TopologyCompareForm
                        With _topologyCompareForm
                            'AddHandler compareView.ProgressChanged, AddressOf _compareView_progressChanged
                            'AddHandler compareView.WorkFinished, AddressOf _compareView_WorkFinished

                            .RestoreAnnotationState() ' HINT: Do this before assignment of document to provide the correct annotation state when adding entities from document
                            .Text = String.Format(_topologyCompareForm.Text, $"{compareDocument.LeftFile}<->{compareDocument.RightFile}")
                            Await .SetDocumentAsync(compareDocument)
                            .Show(_documentForm.MainForm)
                        End With
                        'RemoveHandler compareView.ProgressChanged, AddressOf _compareView_progressChanged
                        'RemoveHandler compareView.WorkFinished, AddressOf _compareView_WorkFinished
                    Else
                        If (compareFailed IsNot Nothing) Then
                            MessageBoxEx.ShowError(_documentForm.MainForm, String.Format(My.Resources.TopologyCompareTexts.CompareFailedMessage, compareFailed.Message))
                        ElseIf openDocumentResult.HasError Then
                            MessageBoxEx.ShowError(_documentForm.MainForm, String.Format(My.Resources.TopologyCompareTexts.ConversionFailedMessage, openDocumentResult.ErrorMessage))
                        ElseIf openDocumentResult.Cancelled Then
                            MessageBoxEx.ShowError(_documentForm.MainForm, My.Resources.TopologyCompareTexts.ConversionCancelledMessage)
                        End If
                    End If
                Else
                    MessageBox.Show(_documentForm.MainForm, My.Resources.TopologyCompareTexts.CompareCancelledMessage, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End Using
        Finally
            _semShowAndStartCompare.Release()
        End Try
    End Sub

    Private Sub _validityCheckForm_ValidityCheckEntrySelectionChanged(sender As Object, e As InformationHubEventArgs) Handles _validityCheckForm.ValidityCheckEntrySelectionChanged
        With _documentForm
            Using .EnableWaitCursor
                If (._informationHub IsNot Nothing) AndAlso (.KBL.Id = e.KblMapperSourceId) Then
                    .SelectRowsInGrids(e.ObjectIds, True, True)
                End If
            End Using
        End With
    End Sub

    Private Sub _documentForm_Closing(sender As Object, e As CancelEventArgs) Handles _documentForm.Closing
        If _topologyCompareForm?.Visible Then
            _topologyCompareForm.Close()
            e.Cancel = _topologyCompareForm.Visible
        End If
    End Sub

    Private Sub _documentForm_Disposed(sender As Object, e As EventArgs) Handles _documentForm.Disposed
        If _topologyCompareForm IsNot Nothing Then
            _topologyCompareForm.Dispose()
        End If
    End Sub

    Private Sub _documentForm_PanActionDisabled(sender As Object, e As EventArgs) Handles _documentForm.PanActionDisabled
        Me.Tools.Home.PanBtn?.SetChecked(False, False)
    End Sub

    Private Sub _documentForm_MagnifierActionDisabled(sender As Object, e As EventArgs) Handles _documentForm.MagnifierActionDisabled
        Me.Tools.Home.PanBtn?.SetChecked(False, False)
    End Sub

    Private Sub _udmDocument_PaneDisplayed(sender As Object, e As PaneDisplayedEventArgs) Handles _udmDocument.PaneDisplayed
        If e.Pane.Control IsNot Nothing AndAlso e.Pane.Key = Me.Tools.Settings.DisplayLogHubBtn?.Key Then
            Me.Tools.Settings.DisplayLogHubBtn?.SetChecked(True)
        End If
    End Sub

    Friend WriteOnly Property ActiveDrawingCanvas() As DrawingCanvas
        Set(value As DrawingCanvas)
            _activeDrawingCanvas = value
        End Set
    End Property

    Public ReadOnly Property Panes As DocumentToolPanesCollection
        Get
            Return _documentForm.Panes
        End Get
    End Property

    Public ReadOnly Property Tools As UtDocumentTools

    Private Sub _topologyCompareForm_SelectionChanged(sender As Object, e As ModelSelectionChangedEventArgs) Handles _topologyCompareForm.SelectionChanged
        Dim document As DocumentForm = Nothing
        Dim kblIds As New List(Of String)

        Select Case e.Side
            Case DocumentSide.Left
                document = _documentForm.MainForm.GetAllDocuments.Where(Function(doc) doc.Value.File.FullName = _topologyCompareForm.Document.LeftFile).Select(Function(kv) kv.Value).SingleOrDefault
            Case DocumentSide.Right
                document = _documentForm.MainForm.GetAllDocuments.Where(Function(doc) doc.Value.File.FullName = _topologyCompareForm.Document.RightFile).Select(Function(kv) kv.Value).SingleOrDefault
            Case Else
                Throw New NotImplementedException($"Side ""{e.Side.ToString}"" not implemented!")
        End Select

        If e.Added.Any() Then
            For Each entity As IBaseModelEntityEx In e.Added.OfType(Of IBaseModelEntityEx)
                For Each id As Guid In entity.GetEEObjectIds
                    Dim obj As E3.Lib.Model.ObjectBaseNaming = Nothing
                    Select Case e.Side
                        Case DocumentSide.Left
                            obj = CType(_topologyCompareForm.Document.ModelLeft(id), E3.Lib.Model.ObjectBaseNaming)
                        Case DocumentSide.Right
                            obj = CType(_topologyCompareForm.Document.ModelRight(id), E3.Lib.Model.ObjectBaseNaming)
                        Case Else
                            Throw New NotImplementedException($"Side ""{e.Side.ToString}"" not implemented!")
                    End Select

                    If obj IsNot Nothing Then
                        kblIds.AddRange(obj.GetKblIds)
                    End If
                Next
            Next
        End If

        If document IsNot Nothing Then
            document.Activate()
            document.SelectRowsInGrids(kblIds, True, True)
        End If
    End Sub

    Public Sub ResetIndicators()
        If _documentForm._D3DControl IsNot Nothing Then
            For Each h As D3D.HiddenEntity In _documentForm._D3DControl._d3d.HiddenEntities
                h.Reset()
            Next

            For Each item As UltraListViewItem In _indicatorsListView.Items
                RemoveIndicators(item.Key)
            Next
            For Each item As UltraListViewItem In _indicatorsListView.Items
                If item.CheckState = CheckState.Checked Then
                    ShowIndicators(item.Key)
                End If
            Next
        End If
    End Sub

End Class