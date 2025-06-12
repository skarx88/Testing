Imports System.ComponentModel
Imports System.IO
Imports System.Reflection
Imports Infragistics.Win.UltraWinDock
Imports Infragistics.Win.UltraWinGrid
Imports Infragistics.Win.UltraWinTabbedMdi
Imports Infragistics.Win.UltraWinTabControl
Imports Infragistics.Win.UltraWinToolbars
Imports VectorDraw.Professional.vdObjects
Imports Zuken.E3.HarnessAnalyzer.Project.Documents
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.HarnessAnalyzer.Shared
Imports Zuken.E3.Lib.IO
Imports Zuken.E3.Lib.IO.E3XML
Imports Zuken.E3.Lib.IO.Files
Imports Zuken.E3.Lib.IO.Files.Dsi
Imports Zuken.E3.Lib.IO.Files.Hcv
Imports Zuken.E3.Lib.IO.KBL
Imports Zuken.E3.Lib.IO.PlmXml
Imports Zuken.E3.Lib.Model
Imports Zuken.E3.Lib.Packager.Circle
Imports Zuken.E3.Lib.Schema.Kbl.Properties

<ObfuscationAttribute(Feature:="renaming", ApplyToMembers:=False)>
Public Class MainStateMachine

    Public Event DocumentOpenFinished(sender As Object, e As DocumentOpenFinshedEventArgs)
    Public Event DocumentFilterItemCheckedChanged(sender As Object, e As DocumentFilterControl.ItemCheckedEventArgs)

    Private _entireCarSettings As EntireCarSettings
    Private _xHcvFile As E3.Lib.IO.Files.Hcv.XhcvFile
    Private _overallConnectivity As OverallConnectivity
    Private _selectionChangedBySystem As Boolean
    Private _virtualInlinerPairs As New List(Of VirtualInlinerPair)

    Private WithEvents _inlinersForm As InlinersForm
    Private WithEvents _mainForm As MainForm

    Friend WithEvents _compareForm As CompareForm
    Friend WithEvents _graphicalCompareForm As GraphicalCompareForm

    Private WithEvents _udmMain As UltraDockManager
    Private WithEvents _utmMain As UltraToolbarsManager
    Private WithEvents _utmmMain As UltraTabbedMdiManager

    Private WithEvents _documentFilterControl As DocumentFilterControl

    Private _closingDocument As DocumentForm
    Private _isLoadingXhcv As Boolean = False
    Private _IsPickUpWindowClicked As Boolean
    Private _isPrintingAreaFromModel3DSelected As Boolean
    Private _openDocLock As New System.Threading.SemaphoreSlim(1)
    Private _openxhcvLock As New System.Threading.SemaphoreSingle

    Public Sub New(mainForm As MainForm)
        _mainForm = mainForm
        _udmMain = mainForm.udmMain
        _utmMain = mainForm.utmMain
        _utmmMain = mainForm.utmmMain

        InitializeBrowserRibbon()
    End Sub

    Friend Function SaveXhcV() As String
        Try
            _mainForm.MainStateMachine.XHcvFile.Save()
            Return String.Empty
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Private Sub InitializeBrowserRibbon()
        _utmMain.BeginUpdate()
        Try
            _utmMain.Office2007UICompatibility = False
            _utmMain.Ribbon.FileMenuStyle = FileMenuStyle.ApplicationMenu2010
            _utmMain.Ribbon.FileMenuButtonCaption = My.Resources.MenuButtonStrings.File
            _utmMain.Ribbon.Visible = True
            _utmMain.Style = ToolbarStyle.Office2010

            Try
                CreateApplicationMenuTools()
            Catch ex As Exception
                ex.ShowMessageBox(String.Format(ErrorStrings.MainStatMachine_ErrorCreatingApplicationMenuTools, vbCrLf, ex.Message))
                Return
            End Try

            Try
                CreateHomeTabTools()
            Catch ex As Exception
                ex.ShowMessageBox(String.Format(ErrorStrings.MainStatMachine_ErrorCreatingHomeTabTools, vbCrLf, ex.Message))
                Return
            End Try

            Try
                CreateSettingsTabTools()
            Catch ex As Exception
                ex.ShowMessageBox(String.Format(ErrorStrings.MainStatMachine_ErrorCreatingSettingsTabTools, vbCrLf, ex.Message))
                Return
            End Try

            Try
                InitializeApplicationMenu()
            Catch ex As Exception
                ex.ShowMessageBox(String.Format(ErrorStrings.MainStatMachine_ErrorLoadMenu_Msg, vbCrLf, ex.Message))
                Return
            End Try

            Try
                InitializeHomeTab()
            Catch ex As Exception
                ex.ShowMessageBox(String.Format(ErrorStrings.MainStatMachine_ErrorCreatingHomeTabs, vbCrLf, ex.Message))
                Return
            End Try

            Try
                InitializeQuickAccessToolbar()
            Catch ex As Exception
                ex.ShowMessageBox(String.Format(ErrorStrings.MainStatMachine_ErrorCreatingQuickAccessToolbar, vbCrLf, ex.Message))
                Return
            End Try

            Try
                InitializeSettingsTab()
            Catch ex As Exception
                ex.ShowMessageBox(String.Format(ErrorStrings.MainStatMachine_ErrorInitializeSettingsTab, vbCrLf, ex.Message))
                Return
            End Try

            Try
                InitializeTutorials()
            Catch ex As Exception
                ex.ShowMessageBox(String.Format(ErrorStrings.MainStatMachine_ErrorCreatingTutorials, vbCrLf, ex.Message))
                Return
            End Try
        Finally
            _utmMain.EndUpdate()
        End Try
    End Sub

    Public Function GetOpenedXhcv() As XhcvFile
        For Each hcv As HcvFile In GetOpenHcvs()
            If TypeOf hcv?.Owner Is XhcvFile Then
                Return CType(hcv.Owner, XhcvFile)
            End If
        Next
        Return Nothing
    End Function

    Public Function GetOpenHcvs() As IEnumerable(Of HcvFile)
        Dim list As New List(Of HcvFile)
        For Each kv As KeyValuePair(Of String, DocumentForm) In Me._mainForm.GetAllDocuments
            list.Add(kv.Value.File)
        Next
        Return list
    End Function

    Private Sub CreateApplicationMenuTools()
        Dim openDocumentLabelTool As New LabelTool(ToolKeys.OpenDocumentLabel.ToString)
        openDocumentLabelTool.SharedProps.Caption = My.Resources.MenuButtonStrings.OpenDocument

        Dim openDocumentListTool As New ListTool(ToolKeys.OpenDocumentList.ToString)
        With openDocumentListTool
            .DisplayCheckmark = False
            .ListToolItems.Add("HCV", "HCV")
            .ListToolItems("HCV").Appearance.Image = My.Resources.OpenHCVDocument.ToBitmap
            .ListToolItems("HCV").DescriptionOnMenu = "Harness Container for Vehicles"
            .ListToolItems.Add("KBL", "KBL")
            .ListToolItems("KBL").Appearance.Image = My.Resources.OpenKBLDocument.ToBitmap
            .ListToolItems("KBL").DescriptionOnMenu = "VDA KBL - Harness Description List (KBL) (V2.2/V2.3/V2.4/2.5)"
            .ListToolItems.Add("XHCV", "xHCV")
            .ListToolItems("XHCV").Appearance.Image = My.Resources.OpenXHCVDocument.ToBitmap
            .ListToolItems("XHCV").DescriptionOnMenu = "Extended Harness Container for Vehicles"
            .ListToolItems.Add("DSI", "DSI")
            .ListToolItems("DSI").Appearance.Image = My.Resources.OpenDSIDocument.ToBitmap
            .ListToolItems("DSI").DescriptionOnMenu = "Harness Design System Interface (DSI)"
            .ListToolItems("DSI").Enabled = _mainForm.AppInitManager.LicensedFeatures.HasFlag(ApplicationFeature.E3HarnessAnalyzerDSI)
            .ListToolItems.Add("PLMXML", "PLMXML")
            .ListToolItems("PLMXML").Appearance.Image = My.Resources.OpenPlmXmlDocument.ToBitmap
            .ListToolItems("PLMXML").DescriptionOnMenu = "Siemens Product-Lifecycle-Management XML"
            .ListToolItems("PLMXML").Enabled = _mainForm.AppInitManager.LicensedFeatures.HasFlag(ApplicationFeature.E3HarnessAnalyzerPlmXml)
            .ListToolItems.Add("XML", "XML")
            .ListToolItems("XML").Appearance.Image = My.Resources.OpenE3XMLDocument.ToBitmap
            .ListToolItems("XML").DescriptionOnMenu = "E3 XML"

            ' HINT: Currently not available as long as there is no VEC importer existing!
            '.ListToolItems.Add("VEC", "VEC")
            '.ListToolItems("VEC").Appearance.Image = My.Resources.OpenVECDocument.ToBitmap
            '.ListToolItems("VEC").DescriptionOnMenu = "Vehicle Electric Container"
            '.ListToolItems("VEC").Enabled = MainForm.AppInitManager.LicenseManager.LicensedFeatures.HasFlag(ApplicationFeature.E3HarnessAnalyzerVEC)
        End With

        Dim openDocumentMenuTool As New PopupMenuTool(ToolKeys.Open.ToString)

        _utmMain.Tools.Add(openDocumentLabelTool)
        _utmMain.Tools.Add(openDocumentListTool)
        _utmMain.Tools.Add(openDocumentMenuTool)

        With openDocumentMenuTool
            .Settings.PopupStyle = PopupStyle.Menu
            .SharedProps.Caption = My.Resources.MenuButtonStrings.Open2
            .Tools.AddTool(ToolKeys.OpenDocumentLabel.ToString)
            .Tools.AddTool(ToolKeys.OpenDocumentList.ToString)
        End With

        Dim openDocumentButton As New ButtonTool(ApplicationMenuToolKey.OpenDocument.ToString)
        With openDocumentButton.SharedProps
            .AppearancesLarge.Appearance.Image = My.Resources.OpenDocument.ToBitmap
            .AppearancesSmall.Appearance.Image = My.Resources.OpenDocument.ToBitmap
            .Caption = My.Resources.MenuButtonStrings.Open
        End With

        Dim saveDocumentButton As New ButtonTool(ApplicationMenuToolKey.SaveDocument.ToString)
        With saveDocumentButton.SharedProps
            .AppearancesLarge.Appearance.Image = My.Resources.SaveDocument.ToBitmap
            .AppearancesSmall.Appearance.Image = My.Resources.SaveDocument.ToBitmap
            .Caption = My.Resources.MenuButtonStrings.Save
            .Enabled = False
        End With

        Dim saveDocumentsButton As New ButtonTool(ApplicationMenuToolKey.SaveDocuments.ToString)
        With saveDocumentsButton.SharedProps
            .AppearancesLarge.Appearance.Image = My.Resources.SaveDocuments.ToBitmap
            .AppearancesSmall.Appearance.Image = My.Resources.SaveDocuments.ToBitmap
            .Caption = My.Resources.MenuButtonStrings.SaveAll
            .Enabled = False
        End With

        Dim saveDocumentAsButton As New ButtonTool(ApplicationMenuToolKey.SaveDocumentAs.ToString)
        With saveDocumentAsButton.SharedProps
            .AppearancesLarge.Appearance.Image = My.Resources.SaveDocumentAs.ToBitmap
            .AppearancesSmall.Appearance.Image = My.Resources.SaveDocumentAs.ToBitmap
            .Caption = My.Resources.MenuButtonStrings.SaveAs
            .Enabled = False
        End With

        Dim closeDocumentButton As New ButtonTool(ApplicationMenuToolKey.CloseDocument.ToString)
        With closeDocumentButton.SharedProps
            .AppearancesLarge.Appearance.Image = My.Resources.CloseDocument.ToBitmap
            .AppearancesSmall.Appearance.Image = My.Resources.CloseDocument.ToBitmap
            .Caption = My.Resources.MenuButtonStrings.Close
            .Enabled = False
        End With

        Dim closeDocumentsButton As New ButtonTool(ApplicationMenuToolKey.CloseDocuments.ToString)
        With closeDocumentsButton.SharedProps
            .AppearancesLarge.Appearance.Image = My.Resources.CloseDocuments.ToBitmap
            .AppearancesSmall.Appearance.Image = My.Resources.CloseDocuments.ToBitmap
            .Caption = My.Resources.MenuButtonStrings.CloseAll
            .Enabled = False
        End With

        Dim exportLabelTool As New LabelTool(ToolKeys.ExportLabel.ToString)
        exportLabelTool.SharedProps.Caption = My.Resources.MenuButtonStrings.ExportDocument

        Dim exportListTool As New ListTool(ToolKeys.ExportList.ToString)
        With exportListTool
            .DisplayCheckmark = False

            .ListToolItems.Add(ApplicationMenuToolKey.ExportExcel.ToString, "Excel").Appearance.Image = My.Resources.ExportExcel.ToBitmap
            .ListToolItems.Add(ApplicationMenuToolKey.ExportGraphic.ToString, My.Resources.MenuButtonStrings.Graphic)
            .ListToolItems(ApplicationMenuToolKey.ExportGraphic.ToString).Appearance.Image = My.Resources.ExportGraphic.ToBitmap
            .ListToolItems.Add(ApplicationMenuToolKey.ExportPDF.ToString, "PDF").Appearance.Image = My.Resources.ExportPDF.ToBitmap

            .ListToolItems.Add(ApplicationMenuToolKey.ExportKbl.ToString, "Kbl")
            .ListToolItems(ApplicationMenuToolKey.ExportKbl.ToString).Appearance.Image = My.Resources.ExportKbl.ToBitmap
            .ListToolItems(ApplicationMenuToolKey.ExportKbl.ToString).Enabled = False
        End With

        _utmMain.Tools.Add(exportLabelTool)
        _utmMain.Tools.Add(exportListTool)

        Dim exportMenuTool As New PopupMenuTool(ToolKeys.Export.ToString)
        _utmMain.Tools.Add(exportMenuTool) ' HINT: must be added before tools can be added to it's child collection

        With exportMenuTool
            .Settings.PopupStyle = PopupStyle.Menu
            .SharedProps.Caption = My.Resources.MenuButtonStrings.Export
            .SharedProps.Enabled = False

            .Tools.AddTool(ToolKeys.ExportLabel.ToString)
            .Tools.AddTool(ToolKeys.ExportList.ToString)
        End With

        Dim recentLabelTool As New LabelTool(ToolKeys.RecentLabel.ToString)
        recentLabelTool.SharedProps.Caption = My.Resources.MenuButtonStrings.RecentlyUsedDocuments

        Dim recentListTool As New ListTool(ToolKeys.RecentList.ToString)
        recentListTool.DisplayCheckmark = False

        _utmMain.Tools.Add(recentLabelTool)
        _utmMain.Tools.Add(recentListTool)

        UpdateRecentFileList(recentListTool)

        Dim recentMenuTool As New PopupMenuTool(ApplicationMenuToolKey.RecentDocuments.ToString)
        _utmMain.Tools.Add(recentMenuTool) ' HINT: must be added before tools can be added to it's child collection

        With recentMenuTool
            .Settings.PopupStyle = PopupStyle.Menu
            .SharedProps.Caption = My.Resources.MenuButtonStrings.Recent
            .Tools.AddTool(ToolKeys.RecentLabel.ToString)
            .Tools.AddTool(ToolKeys.RecentList.ToString)
        End With

        Dim printButton As New ButtonTool(ApplicationMenuToolKey.Print.ToString)
        With printButton.SharedProps
            .AppearancesLarge.Appearance.Image = My.Resources.Print.ToBitmap
            .AppearancesSmall.Appearance.Image = My.Resources.Print.ToBitmap
            .Caption = My.Resources.MenuButtonStrings.Print
        End With

        Dim exitE3HarnessAnalyzerButton As New ButtonTool(ApplicationMenuToolKey.ExitE3HarnessAnalyzer.ToString)
        With exitE3HarnessAnalyzerButton.SharedProps
            .AppearancesLarge.Appearance.Image = My.Resources.ExitE3HarnessAnalyzer.ToBitmap
            .AppearancesSmall.Appearance.Image = My.Resources.ExitE3HarnessAnalyzer.ToBitmap
            .Caption = My.Resources.MenuButtonStrings.ExitApplication
        End With

        _utmMain.Tools.Add(openDocumentButton)
        _utmMain.Tools.Add(saveDocumentButton)
        _utmMain.Tools.Add(saveDocumentsButton)
        _utmMain.Tools.Add(saveDocumentAsButton)
        _utmMain.Tools.Add(closeDocumentButton)
        _utmMain.Tools.Add(closeDocumentsButton)
        _utmMain.Tools.Add(printButton)
        _utmMain.Tools.Add(exitE3HarnessAnalyzerButton)
    End Sub

    Private Sub CreateHomeTabTools()
        With _utmMain
            Dim compareDataButton As New ButtonTool(HomeTabToolKey.CompareData.ToString)
            With compareDataButton.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.CompareData.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.CompareData.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.CompareData
                .Enabled = False
            End With

            Dim compareGraphicButton As New ButtonTool(HomeTabToolKey.CompareGraphic.ToString)
            With compareGraphicButton.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.CompareGraphic.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.CompareGraphic.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.CompareGraphic
                .Enabled = False
            End With

            Dim display3DView As New StateButtonTool(HomeTabToolKey.Display3DView.ToString)
            With display3DView.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.View3D
                .AppearancesSmall.Appearance.Image = My.Resources.View3D
                .Caption = My.Resources.MenuButtonStrings.View3D
            End With

            _documentFilterControl = New DocumentFilterControl(_mainForm.Project)

            Dim document3DFilter As New PopupControlContainerTool(HomeTabToolKey.Document3DFilter.ToString)
            document3DFilter.Control = _documentFilterControl

            With document3DFilter.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.Drawing.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.Drawing.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.Document3DFilter
                .Enabled = False
                .Visible = _mainForm.HasView3DFeature
            End With

            Dim loadCarTransformationButton As New ButtonTool(HomeTabToolKey.LoadCarTransformation.ToString)
            With loadCarTransformationButton.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.LoadCarTransformation
                .AppearancesSmall.Appearance.Image = My.Resources.LoadCarTransformation
                .Caption = My.Resources.MenuButtonStrings.LoadCarTransformation
                .Visible = False
                .Enabled = False
            End With

            Dim inliners As New ButtonTool(HomeTabToolKey.Inliners.ToString)
            With inliners.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.Inliners.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.Inliners.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.Inliners
                .Enabled = False
            End With

            Dim bdlCalc As New ButtonTool(HomeTabToolKey.BundleCalculator.ToString)
            With bdlCalc.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.BundleCrossSection.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.BundleCrossSection.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.BundleCalculator
                .Enabled = True
            End With

            Dim saveCarTransformationButton As New ButtonTool(HomeTabToolKey.SaveCarTransformation.ToString)
            With saveCarTransformationButton.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.SaveCarTransformation
                .AppearancesSmall.Appearance.Image = My.Resources.SaveCarTransformation
                .Caption = My.Resources.MenuButtonStrings.SaveCarTransformation
                .Visible = False
                .Enabled = False
            End With

            Dim export3DModel As New ButtonTool(HomeTabToolKey.Export3DModel.ToString)
            With export3DModel.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.Export3dModel
                .AppearancesSmall.Appearance.Image = My.Resources.Export3dModel
                .Caption = My.Resources.MenuButtonStrings.Export3DModel
                .Visible = True
                .Enabled = False ' HINT: always disable export to 3d, because only if 3d is available (on load) the button is enabled
            End With

            Dim searchButton As New ButtonTool(HomeTabToolKey.Search.ToString)
            With searchButton.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.Search.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.Search.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.Find
                .Enabled = False
            End With

            Dim topologyEditorButton As New ButtonTool(HomeTabToolKey.TopologyEditor.ToString)
            With topologyEditorButton.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.TopologyEditor.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.TopologyEditor.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.TopologyEditor
            End With

            Dim hideNoModuleEntities As New StateButtonTool(HomeTabToolKey.HideNoModuleEntities.ToString)
            With hideNoModuleEntities.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.hide
                .AppearancesSmall.Appearance.Image = My.Resources.hide
                .Caption = My.Resources.MenuButtonStrings.HideNoModuleEntities
                .Enabled = False
            End With

            .Tools.Add(hideNoModuleEntities)
            .Tools.Add(compareDataButton)
            .Tools.Add(compareGraphicButton)
            .Tools.Add(display3DView)
            .Tools.Add(document3DFilter)
            .Tools.Add(loadCarTransformationButton)
            .Tools.Add(inliners)
            .Tools.Add(saveCarTransformationButton)
            .Tools.Add(searchButton)
            .Tools.Add(topologyEditorButton)
            .Tools.Add(export3DModel)
            .Tools.Add(bdlCalc)
        End With
    End Sub

    Private Sub CreateSettingsTabTools()
        With _utmMain
            Dim aboutButton As New ButtonTool(SettingsTabToolKey.About.ToString)
            With aboutButton.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.About.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.About.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.About
            End With

            Dim dataTableSettingsButton As New ButtonTool(SettingsTabToolKey.DataTableSettings.ToString)
            With dataTableSettingsButton.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.DataTableSettings.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.DataTableSettings.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.DataTables2
            End With

            Dim displayTopologyHub As New StateButtonTool(SettingsTabToolKey.DisplayTopologyHub.ToString)
            With displayTopologyHub.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.DisplayTopologyHub.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.DisplayTopologyHub.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.CarTopology
                .Visible = False
            End With

            Dim generalSettingsButton As New ButtonTool(SettingsTabToolKey.GeneralSettings.ToString)
            With generalSettingsButton.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.GeneralSettings.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.GeneralSettings.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.General
            End With

            Dim helpButton As New ButtonTool(SettingsTabToolKey.Help.ToString)
            With helpButton.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.Help.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.Help.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.Help
            End With

            Dim layoutSettingsButton As New ButtonTool(SettingsTabToolKey.LayoutSettings.ToString)
            With layoutSettingsButton.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.LayoutSettings.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.LayoutSettings.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.ResetLayout
            End With

            Dim wireTypeDiametersButton As New ButtonTool(SettingsTabToolKey.WireTypeDiameters.ToString)
            With wireTypeDiametersButton.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.WireTypeDiameters.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.WireTypeDiameters.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.WireTypeDiameters
            End With

            Dim wireTypeWeightsButton As New ButtonTool(SettingsTabToolKey.WireTypeWeights.ToString)
            With wireTypeWeightsButton.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.WireTypeWeights.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.WireTypeWeights.ToBitmap
                .Caption = My.Resources.MenuButtonStrings.WireTypeWeights
            End With

            .Tools.Add(aboutButton)
            .Tools.Add(dataTableSettingsButton)
            .Tools.Add(displayTopologyHub)
            .Tools.Add(generalSettingsButton)
            .Tools.Add(helpButton)
            .Tools.Add(layoutSettingsButton)
            .Tools.Add(wireTypeDiametersButton)
            .Tools.Add(wireTypeWeightsButton)
        End With
    End Sub

    Private Sub InitializeApplicationMenu()
        With _utmMain.Ribbon.ApplicationMenu2010.NavigationMenu
            .Tools.AddTool(ToolKeys.Open.ToString)
            .Tools.AddTool(ApplicationMenuToolKey.SaveDocument.ToString)
            .Tools.AddTool(ApplicationMenuToolKey.SaveDocuments.ToString)
            .Tools.AddTool(ApplicationMenuToolKey.SaveDocumentAs.ToString)
            .Tools.AddTool(ApplicationMenuToolKey.CloseDocument.ToString)
            .Tools.AddTool(ApplicationMenuToolKey.CloseDocuments.ToString)
            .Tools.AddTool(ToolKeys.Export.ToString)
            .Tools.AddTool(ApplicationMenuToolKey.RecentDocuments.ToString)
            .Tools.AddTool(ApplicationMenuToolKey.Print.ToString)
            .Tools.AddTool(ApplicationMenuToolKey.ExitE3HarnessAnalyzer.ToString)
        End With
    End Sub

    Private Sub InitializeHomeTab()
        With _utmMain.Ribbon
            Dim homeTab As RibbonTab = .Tabs.Add(NameOf(My.Resources.MenuButtonStrings.Home))
            With homeTab
                .Caption = My.Resources.MenuButtonStrings.Home

                Dim toolsGroup As RibbonGroup = .Groups.Add(NameOf(My.Resources.MenuButtonStrings.Tools))
                With toolsGroup
                    .Caption = My.Resources.MenuButtonStrings.Tools
                    .MergeOrder = 2
                    .Tools.AddTool(HomeTabToolKey.CompareData.ToString)
                    .Tools(HomeTabToolKey.CompareData.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(HomeTabToolKey.CompareGraphic.ToString)
                    .Tools(HomeTabToolKey.CompareGraphic.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(HomeTabToolKey.Search.ToString)
                    .Tools(HomeTabToolKey.Search.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(HomeTabToolKey.Inliners.ToString)
                    .Tools(HomeTabToolKey.Inliners.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(HomeTabToolKey.TopologyEditor.ToString)
                    .Tools(HomeTabToolKey.TopologyEditor.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(HomeTabToolKey.HideNoModuleEntities.ToString)
                    .Tools(HomeTabToolKey.HideNoModuleEntities.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(HomeTabToolKey.BundleCalculator.ToString)
                    .Tools(HomeTabToolKey.BundleCalculator.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                End With

                Dim view3DGroup As RibbonGroup = .Groups.Add(NameOf(My.Resources.MenuButtonStrings._3D))
                With view3DGroup
                    .MergeOrder = 8
                    .Caption = My.Resources.MenuButtonStrings._3D
                    .Tools.AddTool(HomeTabToolKey.Display3DView.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(HomeTabToolKey.Document3DFilter.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(HomeTabToolKey.LoadCarTransformation.ToString)
                    .Tools.AddTool(HomeTabToolKey.SaveCarTransformation.ToString)
                    .Tools.AddTool(HomeTabToolKey.Export3DModel.ToString)
                    .Visible = False
                End With
            End With
        End With
    End Sub

    Private Sub InitializeQuickAccessToolbar()
        With _utmMain.Ribbon.QuickAccessToolbar
            .Location = QuickAccessToolbarLocation.AboveRibbon

            .Tools.AddTool(ApplicationMenuToolKey.OpenDocument.ToString)
            .Tools.AddTool(ApplicationMenuToolKey.SaveDocument.ToString)

            .ContextMenuTools.AddTool(ApplicationMenuToolKey.OpenDocument.ToString)
            .ContextMenuTools.AddTool(ApplicationMenuToolKey.SaveDocument.ToString)
        End With
    End Sub

    Private Sub InitializeSettingsTab()
        With _utmMain.Ribbon
            Dim settingsTab As RibbonTab = .Tabs.Add(NameOf(My.Resources.MenuButtonStrings.Settings))
            With settingsTab
                .MergeOrder = 3
                .Caption = My.Resources.MenuButtonStrings.Settings

                Dim applicationGroup As RibbonGroup = .Groups.Add(NameOf(My.Resources.MenuButtonStrings.Application))
                With applicationGroup
                    .Caption = My.Resources.MenuButtonStrings.Application
                    .Tools.AddTool(SettingsTabToolKey.GeneralSettings.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(SettingsTabToolKey.LayoutSettings.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(SettingsTabToolKey.DataTableSettings.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(SettingsTabToolKey.WireTypeDiameters.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(SettingsTabToolKey.WireTypeWeights.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                End With

                Dim panesGroup As RibbonGroup = settingsTab.Groups.Add(NameOf(My.Resources.MenuButtonStrings.Panes))
                panesGroup.Caption = My.Resources.MenuButtonStrings.Panes
                panesGroup.Tools.AddTool(SettingsTabToolKey.DisplayTopologyHub.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large

                Dim infoGroup As RibbonGroup = settingsTab.Groups.Add(NameOf(My.Resources.MenuButtonStrings.Info))
                With infoGroup
                    .Caption = My.Resources.MenuButtonStrings.Info
                    .MergeOrder = 2
                    .Tools.AddTool(SettingsTabToolKey.About.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(SettingsTabToolKey.Help.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                End With
            End With
        End With
    End Sub

    Private Sub InitializeTutorials()
        If (_mainForm.GeneralSettings.Tutorials?.Count).GetValueOrDefault > 0 Then
            Dim PopupMenuTool1 As PopupMenuTool = New PopupMenuTool("PopupMenuTool1")

            PopupMenuTool1.SharedPropsInternal.Caption = My.Resources.MenuButtonStrings.Tutorials
            PopupMenuTool1.SharedPropsInternal.DisplayStyle = ToolDisplayStyle.ImageAndText
            PopupMenuTool1.SharedPropsInternal.AppearancesSmall.Appearance.Image = My.Resources.Tutorials
            PopupMenuTool1.SharedPropsInternal.AppearancesSmall.Appearance.BackColor = Color.LightGreen

            _utmMain.Tools.Add(PopupMenuTool1)

            Dim cntr As Integer = 0
            For Each tut As Tutorial In _mainForm.GeneralSettings.Tutorials.Where(Function(t) t.UILanguage = _mainForm.GeneralSettings.UILanguage)
                Dim btn As ButtonTool = New ButtonTool(String.Format("Tutorial{0}", cntr.ToString))
                btn.SharedPropsInternal.Caption = tut.Title
                btn.SharedPropsInternal.AppearancesSmall.Appearance.Image = My.Resources.Tutorials
                _utmMain.Tools.Add(btn)
                PopupMenuTool1.Tools.Add(btn)
                cntr += 1
            Next

            _utmMain.Ribbon.TabItemToolbar.NonInheritedTools.AddRange({PopupMenuTool1})
        End If
    End Sub

    Private Sub InitializeVirtualInlinerPairs(virtualInliners As IEnumerable(Of Component_box))
        Dim documentForms As List(Of DocumentForm) = _mainForm.GetAllDocuments.Values.ToList

        _virtualInlinerPairs = New List(Of VirtualInlinerPair)

        For Each virtualInliner As Component_box In virtualInliners.OrderBy(Function(vi) vi.Part_number)
            Dim virtualInlinerPair As VirtualInlinerPair = If(_virtualInlinerPairs.Any(Function(vip) vip.Id = virtualInliner.Part_number), _virtualInlinerPairs.Where(Function(vip) vip.Id = virtualInliner.Part_number).FirstOrDefault, New VirtualInlinerPair(virtualInliner))
            For Each boxConnector As Component_box_connector In virtualInliner.Component_box_connectors.OrderBy(Function(c) c.Id)
                For Each docFrm As DocumentForm In documentForms
                    Dim counterInliner As Connector_occurrence = docFrm.GetCounterInlinerToBoxConnector(boxConnector)
                    If counterInliner IsNot Nothing Then
                        Dim connectorFace As VdSVGGroup = If(docFrm.ActiveDrawingCanvas IsNot Nothing AndAlso docFrm.ActiveDrawingCanvas.GroupMapper.ContainsKey(counterInliner.SystemId), docFrm.ActiveDrawingCanvas.GroupMapper(counterInliner.SystemId).Values.Where(Function(g) g.SVGType = SvgType.Undefined.ToString).FirstOrDefault, Nothing)

                        For Each inlinerIdentifier As InlinerIdentifier In _mainForm.GeneralSettings.InlinerIdentifiers
                            Dim pairRecognizer As String = inlinerIdentifier.GetPairRecognizer(counterInliner.Id)
                            If Not String.IsNullOrEmpty(pairRecognizer) Then
                                If pairRecognizer = inlinerIdentifier.PairIdentifiers.Item1 Then
                                    virtualInlinerPair.ConnectorOccsWithKblMapperA.Add(counterInliner, docFrm.KBL)
                                    virtualInlinerPair.InlinerIdA = If(virtualInlinerPair.InlinerIdA = String.Empty, counterInliner.Id, String.Format("{0}, {1}", virtualInlinerPair.InlinerIdA, counterInliner.Id))
                                    virtualInlinerPair.DocumentA = docFrm.Document
                                    virtualInlinerPair.ConnectorFacesA.Add(counterInliner.Id, counterInliner.SystemId, connectorFace)

                                Else
                                    virtualInlinerPair.ConnectorOccsWithKblMapperB.Add(counterInliner, docFrm.KBL)
                                    virtualInlinerPair.InlinerIdB = If(virtualInlinerPair.InlinerIdB = String.Empty, counterInliner.Id, String.Format("{0}, {1}", virtualInlinerPair.InlinerIdB, counterInliner.Id))
                                    virtualInlinerPair.DocumentB = docFrm.Document
                                    virtualInlinerPair.ConnectorFacesB.Add(counterInliner.Id, counterInliner.SystemId, connectorFace)
                                End If
                                virtualInlinerPair.RemapPinning(counterInliner, boxConnector)
                                Exit For
                            End If
                        Next
                    End If
                Next
            Next

            If (Not _virtualInlinerPairs.Any(Function(vip) vip.Id = virtualInliner.Part_number)) Then
                _virtualInlinerPairs.Add(virtualInlinerPair)
            End If
        Next

        For Each inlPair As VirtualInlinerPair In _virtualInlinerPairs
            For Each entr As Tuple(Of String, String) In inlPair.GetCavityOccurenceMapping
                _mainForm.MainStateMachine.OverallConnectivity.InlinerCavityPairs.AddOrUpdate(entr.Item1, Function() entr.Item2, Function() entr.Item2)
            Next
        Next
    End Sub

    Private Function InitializeVirtualInliners(hcv As BaseHcvFile) As List(Of Component_box)
        If hcv.OfType(Of VirtualInlinersContainerFile).Any() Then
            Dim virtualInliners As New List(Of Component_box)
            For Each virtualInlinerFile As VirtualInlinersContainerFile In hcv.OfType(Of Hcv.VirtualInlinersContainerFile)
                Try
                    Using s As System.IO.Stream = virtualInlinerFile.GetDataAsStream
                        Dim kblContainer As KBL_container = KBL_container.Read(s)
                        virtualInliners.AddRange(kblContainer.Component_box)
                    End Using
                Catch ex As Exception
#If DEBUG Or CONFIG = "Debug" Then
                    Throw
#Else
                MessageBox.Show(String.Format(ErrorStrings.MainStatMachine_ErrorWhileLoadingVirtualInlinerDocument_Msg, IO.Path.GetFileName(virtualInlinerFile.FullName), vbCrLf, ex.Message), [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)
#End If
                End Try
            Next
            Return virtualInliners
        End If
        Return Nothing
    End Function

    Private Sub CloseDocument() ' TODO: replace with document.close after BL was migrated from 2D
        _mainForm?.utmmMain?.ActiveTab?.Close()
    End Sub

    Private Sub CloseDocuments() ' TODO: replace with_ project.close after BL was migrated from 2D
        Dim childForms() As Form = _mainForm.utmmMain.MdiParent.MdiChildren

        For Each childForm As Form In childForms
            childForm.Close()
        Next

        Me.XHcvFile?.Close()
        Me.XHcvFile = Nothing
    End Sub

    Private Sub CompareDocuments(compareData As Boolean)
        Dim compareDocuments As New List(Of DocumentForm)
        Dim referenceDocument As DocumentForm = Nothing

        'TODO: It would be better to migrate over to Project.Documents (read data from there). But the model-objects are not prepared for this because current 2D-Business-logic migration needed, this would cause a lot of changes to be made but for future and better handling of the code, this changes needed to be done! Also for avoiding misunderstanding handling of 2D-Document and 3D-Document correctly: 2D = DrawingCanvase, 3D = Drawing3DViewer. Both BL's are in DocumentForm (GUI-element which is the incorrect place) 
        For Each tabGroup As MdiTabGroup In _utmmMain.TabGroups ' TODO: replace this with project.documents and iterate over that model object after BL in Document was migrated from 2D-part
            For Each documentTab As MdiTab In tabGroup.Tabs
                Dim documentFrm As DocumentForm = DirectCast(documentTab.Form, DocumentForm)
                If Not documentFrm.IsBusy Then
                    If Not documentTab.IsFormActive Then
                        compareDocuments.Add(documentFrm)
                    ElseIf documentFrm.Is2DOr3DCanvasActive Then
                        referenceDocument = documentFrm
                    End If
                End If
            Next
        Next

        If compareData Then
            If (_compareForm Is Nothing) OrElse (_compareForm.IsDisposed) OrElse (Not _compareForm.Visible) Then
                _compareForm = New CompareForm(_mainForm.ActiveDocument, compareDocuments.ToArray)
                _compareForm.Show(_mainForm)
            End If

            _compareForm.BringToFront()
        Else
            If referenceDocument IsNot Nothing Then
                If (_graphicalCompareForm Is Nothing) OrElse (_graphicalCompareForm.IsDisposed) OrElse (Not _graphicalCompareForm.Visible) Then
                    _graphicalCompareForm = New GraphicalCompareForm(_mainForm.ActiveDocument, compareDocuments, _mainForm.GeneralSettings.MoveDistanceToleranceForGraphicalCompare, _mainForm.GeneralSettings.ThresholdNofInstancesWithIdenticalOffsetValue)
                    _graphicalCompareForm.Show(_mainForm)
                End If

                _graphicalCompareForm.BringToFront()
            End If
        End If
    End Sub

    Private Sub ExportDSI()
        If _mainForm.ActiveDocument IsNot Nothing Then
            Using sfdDSI As New SaveFileDialog
                With sfdDSI
                    If _mainForm.ActiveDocument.KBL.GetChanges.Any() Then
                        .FileName = String.Format("{0}{1}{2}_{3}_{4}_{5}{6}", Now.Year, Format(Now.Month, "00"), Format(Now.Day, "00"), Replace(_mainForm.ActiveDocument.KBL.HarnessPartNumber, " ", String.Empty), _mainForm.ActiveDocument.KBL.GetChanges.Max(Function(change) change.Id), _mainForm.ActiveDocument.Text, KnownFile.DSI)
                    Else
                        .FileName = String.Format("{0}{1}{2}_{3}_{4}{5}", Now.Year, Format(Now.Month, "00"), Format(Now.Day, "00"), Replace(_mainForm.ActiveDocument.KBL.HarnessPartNumber, " ", String.Empty), _mainForm.ActiveDocument.Text, KnownFile.DSI)
                    End If

                    .Filter = $"DSI files (*{KnownFile.DSI})|*{KnownFile.DSI}"
                    .Title = DialogStrings.MainStatMachine_ExportToDSIBaseFile_Title
                End With

                If sfdDSI.ShowDialog(_mainForm) = DialogResult.OK Then
                    Using _mainForm.EnableWaitCursor
                        Try
                            Dim dsiInterface As New E3.Lib.IO.Files.Dsi.DSIInterface(_mainForm.ActiveDocument.KBL)
                            dsiInterface.ExportDSI(sfdDSI.FileName, _mainForm.ActiveDocument.Text)
                            MessageBoxEx.ShowInfo(DialogStrings.MainStatMachine_ExportDSISuccess_Msg)
                        Catch ex As Exception
                            ex.ShowMessageBox(String.Format(ErrorStrings.MainStatMachine_ErrorExportDSI_Msg, vbCrLf, ex.Message))
                        End Try
                    End Using
                End If
            End Using
        Else
            MessageBoxEx.ShowInfo(DialogStrings.MainStatMachine_NoDocLoaded_Msg)
        End If
    End Sub

    Private Sub ExportExcel()
        If _mainForm.ActiveDocument IsNot Nothing Then
            Using sfdDSI As New SaveFileDialog
                With sfdDSI
                    sfdDSI.DefaultExt = KnownFile.XLSX.Trim("."c)

                    If _mainForm.ActiveDocument.KBL.GetChanges.Any() Then
                        .FileName = String.Format("{0}{1}{2}_{3}_{4}_{5}{6}", Now.Year, Format(Now.Month, "00"), Format(Now.Day, "00"), Replace(_mainForm.ActiveDocument.KBL.HarnessPartNumber, " ", String.Empty), _mainForm.ActiveDocument.KBL.GetChanges.Max(Function(change) change.Id), _mainForm.ActiveDocument.Text, KnownFile.XLSX)
                    Else
                        .FileName = String.Format("{0}{1}{2}_{3}_{4}{5}", Now.Year, Format(Now.Month, "00"), Format(Now.Day, "00"), Replace(_mainForm.ActiveDocument.KBL.HarnessPartNumber, " ", String.Empty), _mainForm.ActiveDocument.Text, KnownFile.XLSX)
                    End If

                    .Filter = $"Excel files (*{KnownFile.XLSX})|*{KnownFile.XLSX}|Excel files (97-2003) (*{KnownFile.XLS})|*{KnownFile.XLS}"
                    .Title = DialogStrings.MainStatMachine_ExportToExcelFile_Title
                End With

                If sfdDSI.ShowDialog(_mainForm) = DialogResult.OK Then
                    Using _mainForm.ActiveDocument.EnableWaitCursor
                        Try
                            _mainForm.ActiveDocument._informationHub.ExportAllToExcel(sfdDSI.FileName, Nothing)
                        Catch ex As Exception
#If DEBUG Or CONFIG = "Debug" Then
                            Throw
#Else
                            MessageBoxEx.ShowError(String.Format(ErrorStrings.MainStatMachine_ErrorExportExcel_Msg, vbCrLf, ex.Message))
#End If
                        End Try
                    End Using
                End If
            End Using
        Else
            MessageBoxEx.ShowInfo(DialogStrings.MainStatMachine_NoDocLoaded_Msg)
        End If
    End Sub

    Private Sub ExportGraphic()
        If _mainForm.ActiveDocument IsNot Nothing Then
            If _mainForm.ActiveDocument.ActiveDrawingCanvas IsNot Nothing Then
                Using sfdGraphic As New SaveFileDialog
                    With sfdGraphic
                        .DefaultExt = KnownFile.DXF.Trim("."c)
                        .FileName = _mainForm.ActiveDocument.Text
                        .Filter = $"Autodesk DXF file (*{KnownFile.DXF})|*{KnownFile.DXF}|JPEG (*{KnownFile.JPG})|*{KnownFile.JPG}|PNG (*.png)|*.png|Bitmap (*.bmp)|*.bmp"
                        .Title = DialogStrings.MainStatMachine_ExportToGraphicFile_Title
                    End With

                    If sfdGraphic.ShowDialog(_mainForm) = DialogResult.OK Then
                        Using _mainForm.EnableWaitCursor
                            Try
                                If VdOpenSave.SaveAs(_mainForm.ActiveDocument.ActiveDrawingCanvas.vdCanvas.BaseControl.ActiveDocument, sfdGraphic.FileName) Then
                                    MessageBoxEx.ShowInfo(DialogStrings.MainStatMachine_ExportGraphicSuccess_Msg)
                                Else
                                    MessageBoxEx.ShowError(ErrorStrings.MainStatMachine_ProblemExportGraphic_Msg)
                                End If
                            Catch ex As Exception
#If DEBUG Or CONFIG = "Debug" Then
                                Throw
#Else
                                MessageBoxEx.ShowError(String.Format(ErrorStrings.MainStatMachine_ErrorExportGraphic_Msg, vbCrLf, ex.Message))
#End If
                            End Try
                        End Using

                        _mainForm.ActiveDocument.OnMessage(New MessageEventArgs(MessageEventArgs.MsgType.HideProgress))
                    End If
                End Using
            Else
                MessageBoxEx.ShowWarning(DialogStrings.MainStatMachine_NoDrwVisible_Msg)
            End If
        End If
    End Sub

    Private Sub ExportPDF()
        If _mainForm.ActiveDocument?.ActiveDrawingCanvas IsNot Nothing Then
            Using pdfSaveForm As New PdfSaveForm(_mainForm.ActiveDocument.ActiveDrawingCanvas.vdCanvas.BaseControl.ActiveDocument, _mainForm.ActiveDocument.Text)
                pdfSaveForm.SchematicsExportVisible = _mainForm.HasSchematicsFeature
                pdfSaveForm.SchematicsExportEnabled = _mainForm.SchematicsView IsNot Nothing AndAlso _mainForm.SchematicsView.ActiveEntities.Count > 0

                If pdfSaveForm.ShowDialog(_mainForm) = DialogResult.OK Then
                    pdfSaveForm.Refresh()

                    Using _mainForm.EnableWaitCursor
                        Select Case pdfSaveForm.Type
                            Case HarnessAnalyzer.PdfSaveForm.PdfExportType.Drawing
                                If pdfSaveForm.UseTTFText Then
                                    _mainForm.ActiveDocument.ActiveDrawingCanvas.vdCanvas.BaseControl.ActiveDocument.FileProperties.PDFExportProperties = vdFileProperties.PDFExportPropertiesFlags.DrawTextAsSelected
                                Else
                                    _mainForm.ActiveDocument.ActiveDrawingCanvas.vdCanvas.BaseControl.ActiveDocument.FileProperties.PDFExportProperties = vdFileProperties.PDFExportPropertiesFlags.UsePrinterPropertiesWithOutlineText
                                End If

                                Dim printer As vdPrint = _mainForm.ActiveDocument.ActiveDrawingCanvas.vdCanvas.BaseControl.ActiveDocument.ActiveLayOut.Printer
                                If printer IsNot Nothing Then
                                    Dim tempPrintName As String = printer.PrinterName

                                    _mainForm.ActiveDocument.ActiveDrawingCanvas.vdCanvas.BaseControl.ActiveDocument.ActiveLayOut.Label = "Printing"

                                    Try
                                        With printer
                                            With .margins
                                                .Bottom = 0
                                                .Left = 0
                                                .Right = 0
                                                .Top = 0
                                            End With

                                            .paperSize = New Rectangle(0, 0, CInt(pdfSaveForm.PDFExportBounds.Width / 25.4 * 100), CInt(pdfSaveForm.PDFExportBounds.Height / 25.4 * 100))
                                            .PrinterName = pdfSaveForm.FilePath
                                            .PrintExtents()
                                            .PrintScaleToFit()
                                            .CenterDrawingToPaper()
                                            CheckFileWritableIfExists(pdfSaveForm.FilePath) 'HINT: check file open-write because vectorDraw does not and will ignore writing to file when open.
                                            .PrintOut()
                                        End With
                                    Catch ex As Exception
#If DEBUG Or CONFIG = "Debug" Then
                                        Throw
#Else
                                    MessageBoxEx.ShowError(String.Format(ErrorStrings.MainStatMachine_ErrorExportPDF_Msg, vbCrLf, ex.Message))
#End If
                                    Finally
                                        printer.PrinterName = tempPrintName
                                    End Try

                                    _mainForm.ActiveDocument.ActiveDrawingCanvas.vdCanvas.BaseControl.ActiveDocument.ActiveLayOut.Label = String.Empty
                                End If
                            Case HarnessAnalyzer.PdfSaveForm.PdfExportType.Schematics
                                Me._mainForm.SchematicsView.ExportToPDF(pdfSaveForm.FilePath)
                        End Select
                    End Using
                End If
            End Using
        Else
            MessageBoxEx.ShowWarning(DialogStrings.MainStatMachine_NoDrwVisible_Msg)
        End If
    End Sub

    Private Sub ExportKbl()
        If _mainForm?.ActiveDocument?.ActiveDrawingCanvas IsNot Nothing Then
            Using sfdDSI As New SaveFileDialog
                With sfdDSI
                    .DefaultExt = KnownFile.KBL.TrimStart("."c)
                    .FileName = String.Format("{0}{1}", _mainForm.ActiveDocument.Text, KnownFile.KBL)
                    .Filter = $"Kbl files (*{KnownFile.KBL})|*{KnownFile.KBL}"

                    .Title = DialogStrings.MainStatMachine_ExportToKblFile_Title
                End With

                If sfdDSI.ShowDialog(_mainForm) = DialogResult.OK Then
                    Using _mainForm.ActiveDocument.EnableWaitCursor
                        If _mainForm.ActiveDocument.File?.KblDocument IsNot Nothing Then
                            Try
                                _mainForm.ActiveDocument.File.KblDocument.WriteAllBytes(sfdDSI.FileName)
                                MessageBoxEx.ShowInfo(String.Format(MainFormStrings.SuccessfullyExportedKBL, sfdDSI.FileName))
                            Catch ex As Exception
                                ex.ShowMessageBox(String.Format(ErrorStrings.MainStatMachine_ErrorExportKbl_Msg, vbCrLf, ex.Message))
                            End Try
                        End If
                    End Using
                End If
            End Using
        Else
            MessageBoxEx.ShowWarning(DialogStrings.MainStatMachine_NoDrwVisible_Msg)
        End If
    End Sub

    Private Sub CheckFileWritableIfExists(filePath As String)
        If E3.Lib.DotNet.Expansions.Devices.My.Computer.FileSystem.FileExists(filePath) Then
            Using fs As New FileStream(filePath, FileMode.Open, FileAccess.Write)
            End Using
        End If
    End Sub

    Private Function GetInlinerConnections() As List(Of Tuple(Of Tuple(Of String, String), Tuple(Of String, String)))
        Dim inlinerConnections As New List(Of Tuple(Of Tuple(Of String, String), Tuple(Of String, String)))
        Dim inlinerLists As New Dictionary(Of String, Dictionary(Of Connector_occurrence, Connector_occurrence))

        For Each tabGroup As MdiTabGroup In _utmmMain.TabGroups
            For Each documentTab As MdiTab In tabGroup.Tabs
                Dim docForm As DocumentForm = DirectCast(documentTab.Form, DocumentForm)
                If docForm.IsExtendedHCV AndAlso Not inlinerLists.ContainsKey(docForm.KBL.HarnessPartNumber) Then
                    inlinerLists.Add(docForm.KBL.HarnessPartNumber, docForm.Inliners)
                End If
            Next
        Next

        For Each inlinerList As KeyValuePair(Of String, Dictionary(Of Connector_occurrence, Connector_occurrence)) In inlinerLists
            For Each inlinerPair As KeyValuePair(Of Connector_occurrence, Connector_occurrence) In inlinerList.Value.Where(Function(inlPair) inlPair.Value IsNot Nothing)
                inlinerConnections.Add(New Tuple(Of Tuple(Of String, String), Tuple(Of String, String))(New Tuple(Of String, String)(inlinerList.Key, inlinerPair.Key.Id), New Tuple(Of String, String)(inlinerLists.Where(Function(inlList) inlList.Value.ContainsKey(inlinerPair.Value))(0).Key, inlinerPair.Value.Id)))
            Next
        Next

        Return inlinerConnections
    End Function

    Private Function GetModulePartNumbersFromFileName(fileName As String) As List(Of String)
        Dim squaredBracketStart As Integer = fileName.IndexOf("["c)
        Dim squaredBracketEnd As Integer = fileName.IndexOf("]"c)

        If (squaredBracketEnd - squaredBracketStart) > 1 Then
            Dim modulePartNumbers As New List(Of String)

            For Each modulePartNumber As String In fileName.Substring(squaredBracketStart + 1, squaredBracketEnd - squaredBracketStart - 1).SplitRemoveEmpty(";"c)
                modulePartNumbers.Add(modulePartNumber.Trim.Replace(" ", String.Empty))
            Next

            Return modulePartNumbers
        Else
            Return Nothing
        End If
    End Function

    Private Sub OnOpenDocumentFinished(doc As DocumentForm)
        RaiseEvent DocumentOpenFinished(Me, New DocumentOpenFinshedEventArgs(doc))
    End Sub

    Private Sub OnXhcvOpenedFinished(xHcv As XhcvFile)
        If _mainForm.ShowTopologyHub(xHcv) Then
            _mainForm.TopologyHub?.InitializeInlinerConnections(GetInlinerConnections, _mainForm.GeneralSettings.InlinerIdentifiers)
            'HINT: not neccessary because it's always shown when document (hcv of xhcv) was opened
            'DirectCast(Me._utmMain.Tools(SettingsTabToolKey.DisplayTopologyHub.ToString), StateButtonTool).Checked = True
            If _mainForm.TopologyHub IsNot Nothing AndAlso _mainForm.Tools.Settings.DisplayTopologyHub IsNot Nothing Then
                _mainForm.Tools.Settings.DisplayTopologyHub.SharedProps.Visible = True
            End If
        End If

        Dim vInliners As List(Of Component_box) = InitializeVirtualInliners(xHcv)
        If vInliners IsNot Nothing Then
            InitializeVirtualInlinerPairs(vInliners)
        End If
    End Sub

    Private Function ShowIsAnyDocumentXhcvWarning() As Boolean
        For Each tabGroup As MdiTabGroup In _mainForm.utmmMain.TabGroups
            For Each tab As MdiTab In tabGroup.Tabs
                Dim documentForm As DocumentForm = DirectCast(tab.Form, DocumentForm)
                If documentForm.IsExtendedHCV Then
                    MessageBoxEx.ShowWarning(DialogStrings.MainStatMachine_OpenDocWarning2_Msg)
                    Return True
                End If
            Next
        Next
        Return False
    End Function

    Private Function GetOpenFileDialog(fileType As KnownFile.Type) As System.Windows.Forms.OpenFileDialog
        Dim filters As New List(Of String)
        Dim titles As New List(Of String)
        Dim multiselect As Boolean = False

        If fileType.HasFlag(KnownFile.Type.KBL) Then
            filters.Add("VDA KBL - Harness Description List (*.kbl)|*.kbl")
            titles.Add(DialogStrings.MainStatMachine_SelKBLFile_Title)
            multiselect = True
        End If

        If fileType.HasFlag(KnownFile.Type.HCV) Then
            filters.Add("Harness Container for Vehicles files (*.hcv)|*.hcv")
            titles.Add(DialogStrings.MainStatMachine_SelHCVFile_Title)
            multiselect = True
        End If

        If fileType.HasFlag(KnownFile.Type.VEC) Then
            filters.Add("Vehicle Electric Container (*.vec)|*.vec")
            titles.Add(DialogStrings.MainStatMachine_SelVECFile_Title)
        End If

        If fileType.HasFlag(KnownFile.Type.xHCV) Then
            filters.Add("Extended Harness Container for Vehicles files (*.xhcv)|*.xhcv")
            titles.Add(DialogStrings.MainStatMachine_SelXHCVFile_Title)
        End If

        If fileType.HasFlag(KnownFile.Type.DSI) Then
            filters.Add("Design System Interface files (*.dsi)|*.dsi")
            titles.Add(DialogStrings.MainStatMachine_SelDSIBaseFile_Title)
        End If

        If fileType.HasFlag(KnownFile.Type.PLMXML) Then
            filters.Add("PLM XML files (*.plmxml)|*.plmxml")
            titles.Add(DialogStrings.MainStatMachine_SelPlmxmlFile_Title)
        End If

        If fileType.HasFlag(KnownFile.Type.XML) Then
            filters.Add("E3XML files (*.xml)|*.xml")
            titles.Add(DialogStrings.MainStatMachine_SelE3xmlFile_Title)
        End If

        Dim title As String = DialogStrings.MainStatMachine_SelCommonFile_Title
        If titles.Count = 1 Then
            title = titles.Single
        End If

        Dim ofdDocument As New System.Windows.Forms.OpenFileDialog
        With ofdDocument
            .Title = title

            If filters.Count > 1 Then
                filters.Insert(0, "All supported formats|" + String.Join(";"c, filters.Select(Function(f) f.Split("|"c).Last)))
                multiselect = True
            Else
                .DefaultExt = filters.Single.Split("|"c).Last.TrimStart("*.".ToCharArray)
            End If

            .Filter = String.Join("|", filters)
            .Multiselect = multiselect

            ' Special sandbox service here, directly change the open dialog to the test-data directory... (if available)
            If Environment.GetEnvironmentVariable("USERNAME") = SANDBOX_USER_NAME Then
                Dim dir As New DirectoryInfo(SANDBOX_TEST_DATA_DIR_PATH)
                If dir.Exists Then
                    .InitialDirectory = SANDBOX_TEST_DATA_DIR_PATH
                End If
            End If
        End With



        Return ofdDocument
    End Function

    Friend Async Function OpenDocument(Optional item As ListToolItem = Nothing) As Task
        Await _openDocLock.WaitAsync
        Try
            If item IsNot Nothing Then
                Dim ext As String = System.IO.Path.GetExtension(item.Key)
                If String.IsNullOrWhiteSpace(ext) Then
                    ext = item.Key
                    item = Nothing
                End If

                Dim type As KnownFile.Type = KnownFile.GetFileType(ext)
                If type <> KnownFile.Type.Unspecified Then
                    Await OpenDocument(type, item)
                    Return
                Else
                    Throw New NotSupportedException("not supported file extension: " + ext)
                End If
            Else
                Await OpenDocument(KnownFile.Type.xHCV Or
                         KnownFile.Type.HCV Or
                         KnownFile.Type.KBL Or
                         KnownFile.Type.DSI Or
                         KnownFile.Type.PLMXML)
            End If
        Finally
            _openDocLock.Release()
        End Try
    End Function

    Private Async Function OpenDocument(fileType As KnownFile.Type, Optional selectedListTool As ListToolItem = Nothing) As Task
        ' Display file open dialog for selecting a valid HCV file
        Dim fileNames() As String = Nothing

        If _mainForm.utmmMain.TabGroups.Count > 0 Then
            If fileType.HasFlag(KnownFile.Type.xHCV) Then
                MessageBoxEx.ShowWarning(DialogStrings.MainStatMachine_OpenDocWarning1_Msg)
                Return
            ElseIf ShowIsAnyDocumentXhcvWarning() Then
                Return
            End If
        End If

        If selectedListTool Is Nothing Then
            Using ofdDocument As OpenFileDialog = GetOpenFileDialog(fileType)

                If Not _mainForm.GeneralSettings.LastOpenedDirectory.IsNullOrEmpty Then
                    ofdDocument.InitialDirectory = _mainForm.GeneralSettings.LastOpenedDirectory
                End If

                If ofdDocument.ShowDialog(_mainForm) = DialogResult.OK Then
                    fileNames = ofdDocument.FileNames

                    If _mainForm.utmmMain.TabGroups.Count > 0 AndAlso KnownFile.IsXHcv(fileNames.FirstOrDefault) Then
                        MessageBoxEx.ShowWarning(DialogStrings.MainStatMachine_OpenDocWarning1_Msg)
                        fileNames = Nothing
                    End If
                End If
            End Using
        ElseIf Not IO.File.Exists(selectedListTool.Key) Then
            MessageBoxEx.ShowInfo(String.Format(ErrorStrings.MainStatMachine_RecentFileNotFound_Msg, selectedListTool.Key))

            _mainForm.GeneralSettings.RecentFiles.DeleteRecentFile(selectedListTool.Key)

            selectedListTool.ParentCollection.Remove(selectedListTool)
        Else
            fileNames = {selectedListTool.Key}
        End If

        If fileNames IsNot Nothing Then
            Using _mainForm.EnableWaitCursor

                _mainForm.GeneralSettings.LastOpenedDirectory = IO.Path.GetDirectoryName(fileNames.LastOrDefault)

                For Each fileName As String In fileNames
                    If _mainForm.GeneralSettings.RecentFiles.Count = 5 Then
                        _mainForm.GeneralSettings.RecentFiles.RemoveAt(0)
                    End If

                    If _mainForm.GeneralSettings.RecentFiles.FindRecentFile(fileName) Is Nothing Then
                        _mainForm.GeneralSettings.RecentFiles.AddRecentFile(New RecentFile(IO.Path.GetDirectoryName(fileName), IO.Path.GetFileName(fileName)))
                    End If
                Next

                _utmMain.BeginUpdate()

                With DirectCast(_utmMain.Tools(ToolKeys.RecentList.ToString), ListTool)
                    .ListToolItems.Clear()
                    For Each recentFile As RecentFile In _mainForm.GeneralSettings.RecentFiles
                        .ListToolItems.Add(IO.Path.Combine(recentFile.DirectoryName, recentFile.FileName), recentFile.FileName)
                        .ListToolItems(IO.Path.Combine(recentFile.DirectoryName, recentFile.FileName)).Appearance.Image = My.Resources.RecentDocuments.ToBitmap
                        .ListToolItems(IO.Path.Combine(recentFile.DirectoryName, recentFile.FileName)).DescriptionOnMenu = Infragistics.Win.FormattedLinkLabel.ParsedFormattedTextValue.EscapeXML(IO.Path.Combine(recentFile.DirectoryName, recentFile.FileName))
                    Next
                End With

                _utmMain.EndUpdate()

                Dim lastHcv As Hcv.BaseHcvFile = Nothing
                _CurrentLoadingDocumentsCount = fileNames.Length

                Try
                    For Each fileName As String In fileNames
                        Dim extension As KnownFile.Type = KnownFile.GetFileType(fileName)
                        Try
                            Select Case extension
                                Case KnownFile.Type.xHCV
                                    If _xHcvFile Is Nothing Then
                                        If fileNames.Length > 1 Then
                                            MessageBoxEx.ShowInfo(DialogStrings.MainStatMachine_MultipleHCVsOpeningNotSupported_Msg)
                                        End If

                                        Dim xhcv As XhcvFile = Nothing
                                        Try
                                            xhcv = XhcvFile.Create(fileName, Project.Common.ZIP_CACHING_METHOD)
                                        Catch ex As Exception
                                            Me.XHcvFile = Nothing
                                            Throw
                                        End Try

                                        If Await OpenXhcvDocument(xhcv) Then
                                            OnXhcvOpenedFinished(xhcv)
                                        Else
                                            ResetHcvAndKblFiles(xhcv)
                                            Me.XHcvFile = Nothing
                                        End If
                                    Else
                                        MessageBoxEx.ShowWarning(String.Format(DialogStrings.MainStatMachine_XHCVAlreadyOpened_Msg, vbCrLf))
                                    End If
                                Case Else
                                    Select Case extension
                                        Case KnownFile.Type.HCV
                                            lastHcv = HcvFile.Create(fileName, Project.Common.ZIP_CACHING_METHOD)
                                            Await OpenSingleHCVDocument(CType(lastHcv, HcvFile), False)
                                        Case KnownFile.Type.KBL
                                            lastHcv = KblFile.Create(fileName, Project.Common.ZIP_CACHING_METHOD)
                                            Await OpenKBLDocument(CType(lastHcv, KblFile))
                                        Case KnownFile.Type.DSI
                                            If _mainForm.AppInitManager.LicensedFeatures.HasFlag(ApplicationFeature.E3HarnessAnalyzerDSI) Then
                                                Await OpenDSIDocument(fileName)
                                            Else
                                                MessageBoxEx.ShowError(ErrorStrings.MainStatMachine_LicFeatMissingDSI_Msg)
                                            End If
                                        Case KnownFile.Type.PLMXML
                                            If _mainForm.AppInitManager.LicensedFeatures.HasFlag(ApplicationFeature.E3HarnessAnalyzerPlmXml) Then
                                                Await OpenPlmXmlDocument(fileName)
                                            Else
                                                MessageBoxEx.ShowError(ErrorStrings.MainStatMachine_LicFeatMissingPlmXml_Msg)
                                            End If
                                        Case KnownFile.Type.XML
                                            OpenE3XmlDocument(fileName)
                                        Case KnownFile.Type.VEC
                                            ' TODO: Open VEC file and convert it to KBL
                                    End Select
                            End Select
                        Catch ex As IO.IOException
                            ex.ShowMessageBox()
                        Finally
                            If CurrentLoadingDocumentsCount > 0 Then ' HINT: when coming from xhcv the currentLoadingDocumentsCount is already zero and we want to avoid to get lower than zero
                                _CurrentLoadingDocumentsCount -= 1
                            End If
                        End Try
                    Next
                Finally
                    _CurrentLoadingDocumentsCount = 0
                End Try

                If _mainForm.AppInitManager.IsInTestMode Then
                    _mainForm.ActiveDocument?.Close()
                    Environment.Exit(0)
                End If
            End Using
        End If
    End Function

    Private Async Function OpenDSIDocument(filePath As String) As Task
        Dim dsiInterface As E3.Lib.IO.Files.Dsi.DSIInterface = Nothing
        Try
            Dim importResult As KblContainerResult = Nothing
            dsiInterface = New E3.Lib.IO.Files.Dsi.DSIInterface(filePath)
            importResult = dsiInterface.ImportDSI()

            For Each dsiImportLogMessage As LogEventArgs In importResult.ToLogEventArgs(filePath)
                _mainForm.ActiveDocument._logHub.WriteLogMessage(dsiImportLogMessage)
            Next

            If importResult.IsSuccess OrElse importResult.HasOnlyWarnings Then
                Using temp_kblFile As TempFile = TempFile.CreateNew(KnownFile.KBL)
                    Using fs As New FileStream(temp_kblFile.Name, FileMode.Create)
                        KBL_container.Write(fs, importResult.KblContainer)
                    End Using

                    Dim kbl As KblFile = CType(KblFile.Create(temp_kblFile.Name, Project.Common.ZIP_CACHING_METHOD), KblFile)
                    If IO.File.Exists(KnownFile.Change(filePath, KnownFile.Type.DWG)) Then
                        Await OpenKBLDocument(kbl, drawingFilePath:=KnownFile.Change(filePath, KnownFile.Type.DWG))
                    ElseIf IO.File.Exists(KnownFile.Change(filePath, KnownFile.Type.DXF)) Then
                        Await OpenKBLDocument(kbl, drawingFilePath:=KnownFile.Change(filePath, KnownFile.Type.DXF))
                    Else
                        Await OpenKBLDocument(kbl)
                    End If
                End Using
            End If
        Catch ex As Exception
#If DEBUG Or CONFIG = "Debug" Then
            Throw
#Else
            ex.ShowMessageBox(String.Format(ErrorStrings.MainStatMachine_ErrorImportDSI_Msg, vbCrLf, ex.Message))
#End If
        End Try
    End Function

    Private Async Function OpenXhcvDocument(xhcv As XhcvFile) As Task(Of Boolean)
        Using Await _openxhcvLock.BeginWaitAsync
            Me.XHcvFile = xhcv

            Dim xhcv_open_cancel As New System.Threading.CancellationTokenSource
            Dim t As Task = Task.Factory.StartNew(Sub() _xHcvFile.Open(cancel:=xhcv_open_cancel.Token), TaskCreationOptions.LongRunning) ' HINT: just start pre-opening the xhcv file even before the user has decided what to do, if opening is not possible this will be cancelled and reverted

            _isLoadingXhcv = True
            _overallConnectivity = New OverallConnectivity
            _mainForm.OnStartLoadingXhcv()

            CType(_mainForm.D3D, D3D.MainFormController).OnBeforeOpenDocument(xhcv.FullName, True)
            Dim success_opened As Boolean = False
            Try
                success_opened = OpenXhcvDocumentCore(xhcv)
            Catch ex As Exception
                success_opened = False
                xhcv_open_cancel.Cancel()
                _isLoadingXhcv = False
                ex.ShowMessageBox(String.Format(ErrorStrings.MainStatMachine_ErrorExtractHCV_Msg, vbCrLf, ex.Message))
            End Try

            If Not success_opened Then
                xhcv_open_cancel.Cancel()
                If Not success_opened OrElse xhcv.IsOpening Then
                    Await TaskEx.WaitUntil(Function() Not xhcv.IsOpening)
                End If
            End If

            Return success_opened
        End Using
    End Function

    Private Function OpenXhcvDocumentCore(xhcv As XhcvFile) As Boolean
        If xhcv IsNot Nothing Then
            If xhcv.EntireCarSettings IsNot Nothing AndAlso xhcv.EntireCarSettings.HasData Then
                _entireCarSettings = EntireCarSettings.LoadFromContainerFile(xhcv.EntireCarSettings)
                If _entireCarSettings Is Nothing Then
                    _mainForm.AppInitManager.ErrorManager.ShowMsgBoxErrorOrWriteConsole(String.Format(ErrorStrings.MainStatMachine_ErrorLoadEntireCarSettings_Msg, vbCrLf, EntireCarSettings.LoadError?.Message), ErrorCodes.ERROR_UNKNOWN)
                End If
            End If

            If _mainForm.HasView3DFeature AndAlso xhcv.CarTransformation IsNot Nothing Then
                Try
                    CType(_mainForm.D3D, D3D.MainFormController).LoadCarTransformationSettingsFromContainer(xhcv.CarTransformation)
                Catch ex As System.Runtime.Serialization.SerializationException
                    'HINT: only show warning when deserialization of car transformation setting failed, but enables the user to proceed loading the xhcv without car transformation settings
                    If MessageBoxEx.ShowWarning(Me._mainForm, String.Format(ErrorStrings.MainStatMachine_WarningDeserializationCarTransformationSettingsFailed_Msg, ex.Message), MessageBoxButtons.YesNo) = DialogResult.Yes Then
                        xhcv.CarTransformation = Nothing
                    End If
                End Try
            End If
        End If

        Dim hcvFilesToLoadResult As LoadDocumentsResult = GetDocumentsToBeLoaded(xhcv)
        If hcvFilesToLoadResult.IsSuccess AndAlso hcvFilesToLoadResult.HcvFiles.Any() Then
            'HINT: do checks outside of async task (within tasks no checks are executed to avoid shown parallel messageboxes)
            For Each hcv As HcvFile In hcvFilesToLoadResult.HcvFiles
                If Not Me.IsOpenHcvDocumentAllowed(hcv) Then
                    Return False
                End If
            Next

            Try
                Using showProgressForm As New ProgressLoadingXhcvForm(Me)
                    Dim tasks As New List(Of Task)
                    showProgressForm.OnFormShown(
                    Async Sub(s, e)
                        Dim hcvFiles As HcvFile() = hcvFilesToLoadResult.HcvFiles.OrderBy(Function(f) f.FullName).ToArray
                        _CurrentLoadingDocumentsCount = hcvFiles.Length
                        For Each hcv As HcvFile In hcvFiles
                            Await OpenSingleHCVDocument(hcv, True, hcvFilesToLoadResult.LoadTopo, False) ' HINT: re-active out-commented-code to active parallelize-opening: this was tested basically and works but not fully clear if consolidated data is all clear -> for the first step it's now deactivated
                            'tasks.Add(OpenSingleHCVDocument(hcv, True, hcvFilesToLoadResult.LoadTopo, False))
                            _CurrentLoadingDocumentsCount -= 1
                        Next
                        'Await Task.WhenAll(tasks)
                        showProgressForm.Close()
                    End Sub)
                    showProgressForm.ShowDialog(_mainForm, xhcv.FullName, hcvFilesToLoadResult.HcvFiles.Count)
                End Using
                Return True
            Finally
                _CurrentLoadingDocumentsCount = 0
            End Try
        Else
            Return False
        End If

    End Function

    Private Function GetDocumentsToBeLoaded(xhcv As XhcvFile) As LoadDocumentsResult
        Static preferReleaseCodeFile As String = IO.Path.Combine(My.Application.Info.DirectoryPath, "PreferReleaseCode.debug")
        Dim loadTopo As Boolean = _mainForm.AppInitManager.IsInTestMode
        If Not loadTopo Then
            Dim owner As IWin32Window = If(My.Application.SplashScreen IsNot Nothing AndAlso My.Application.SplashScreen.Visible, My.Application.SplashScreen, _mainForm)
            If DebugEx.IsDebug AndAlso Not IO.File.Exists(preferReleaseCodeFile) Then
                Using selectDlg As New XhcvSelectDocumentsDialog()
                    Select Case selectDlg.ShowDialog(owner, xhcv)
                        Case DialogResult.Cancel
                            Return New LoadDocumentsResult(ResultState.Cancelled)
                        Case DialogResult.Continue
                            Using System.IO.File.Create(preferReleaseCodeFile)
                            End Using
                            Return GetDocumentsToBeLoaded(xhcv)
                        Case Else
                            Return New LoadDocumentsResult(ResultState.Success, selectDlg.GetSelectedHcvFiles.ToArray, selectDlg.OpenDrawings)
                    End Select
                End Using
            Else
                Dim showToporesult As Nullable(Of Boolean) = ShowLoadTopoQuestion(owner)
                If Not showToporesult.HasValue Then
                    Return New LoadDocumentsResult(ResultState.Cancelled)
                Else
                    loadTopo = showToporesult.Value
                End If
            End If
        End If
        Return New LoadDocumentsResult(ResultState.Success, xhcv.OfType(Of HcvFile).ToList, loadTopo)
    End Function

    Private Class LoadDocumentsResult
        Inherits Result(Of IEnumerable(Of HcvFile))

        Public Sub New(result As ResultState, Optional data As IEnumerable(Of HcvFile) = Nothing, Optional loadTopo As Boolean = False)
            MyBase.New(result, data)
            Me.LoadTopo = loadTopo
        End Sub

        Public ReadOnly Property LoadTopo As Boolean

        Public ReadOnly Property HcvFiles As IEnumerable(Of HcvFile)
            Get
                Return MyBase.Data
            End Get
        End Property
    End Class

    Private Function ShowLoadTopoQuestion(owner As IWin32Window) As Nullable(Of Boolean)
        Select Case MessageBoxEx.ShowQuestion(owner, DialogStrings.MainStatMachine_LoadDrwsForEachHCV_Msg, MessageBoxButtons.YesNoCancel, Nothing)
            Case DialogResult.Yes
                Return True
            Case DialogResult.Cancel
                Return Nothing
        End Select
        Return False
    End Function

    Private Sub ResetHcvAndKblFiles(hcv As BaseHcvFile)
        hcv?.Dispose()
    End Sub

    Private Async Function OpenKBLDocument(kbl As [Lib].IO.Files.Hcv.KblFile, Optional fromTopologyContainer As Boolean = False, Optional drawingFilePath As String = "", Optional doNotMirror As Boolean = False) As Task
        CType(_mainForm.D3D, D3D.MainFormController).OnBeforeOpenDocument(kbl.FullName, True)
        Using _mainForm.EnableWaitCursor()
            If _utmmMain.TabGroups.Count <> 0 Then
                For Each tabGroup As MdiTabGroup In _utmmMain.TabGroups
                    If tabGroup.Tabs.OfType(Of MdiTab).Any(Function(docTab) docTab.Key = kbl.FullName) Then
                        MessageBoxEx.ShowError(ErrorStrings.MainStatMachine_DocAlreadyOpened_Msg)
                        Return
                    End If
                Next
            End If

            If (_compareForm?.Visible).GetValueOrDefault OrElse (_graphicalCompareForm?.Visible).GetValueOrDefault Then
                If MessageBoxEx.ShowQuestion(DialogStrings.MainStatMachine_CloseCompDlgWhenOpeningKBL_Msg) = DialogResult.No Then
                    Return
                End If
            End If

            If (_compareForm?.Visible).GetValueOrDefault Then
                _compareForm.Close()
                _compareForm.Dispose()
            End If

            If (_graphicalCompareForm?.Visible).GetValueOrDefault Then
                _graphicalCompareForm.Close()

                If Not _graphicalCompareForm.Visible Then
                    _graphicalCompareForm.Dispose()
                End If
            End If

            _mainForm.SearchMachine.VisibleSearchForm(False)

            ' Identify module part numbers for activation inside file name
            Dim modulePartNumbersForActivation As List(Of String) = GetModulePartNumbersFromFileName(IO.Path.GetFileName(kbl.FullName))

            ' Create new tab with a new instance of a DocumentForm class and open the HCV file
            Dim documentForm As New DocumentForm(_mainForm, kbl, If(IO.File.Exists(drawingFilePath), New IO.FileInfo(drawingFilePath), Nothing), doNotMirror)
            documentForm.Show()

            Dim res As IResult = Await documentForm.OpenDocument(modulePartNumbersForActivation, loadDrawings:=True)
            Await TaskEx.WaitUntil(Function() Not documentForm.IsBusy)
            OnOpenDocumentFinished(documentForm)

            If res.IsSuccess Then
                Dim mdiTab As MdiTab = _utmmMain.TabFromForm(documentForm)
                mdiTab.Key = kbl.FullName

                If fromTopologyContainer Then
                    mdiTab.Settings.TabAppearance.BackColor = Color.LightSkyBlue
                    mdiTab.Settings.TabAppearance.Image = My.Resources.EntireCarContainer.ToBitmap
                End If
            Else
                documentForm.Close()
            End If
        End Using
    End Function

    Private Async Function OpenPlmXmlDocument(fullName As String) As Task
        Try
            Dim plmXmlInterface As New Files.PlmXml.PlmXmlInterface(fullName)
            Dim result As Files.PlmXml.ImportPlmXmlResult = plmXmlInterface.ImportPlmXml()
            If result.IsSuccess Then
                Using temp_kbl_file As TempFile = TempFile.Create(fullName, KnownFile.KBL)
                    Using fs As New FileStream(temp_kbl_file.Name, FileMode.Create)
                        result.KBl.WriteTo(fs)
                    End Using

                    Dim kbl As KblFile = KblFile.Create(temp_kbl_file.Name, Project.Common.ZIP_CACHING_METHOD)
                    Await OpenKBLDocument(kbl)
                End Using
            Else
                Select Case result.ErrorCode
                    Case Files.PlmXml.PlmXmlErrorCode.UnexpectedErrorPlmXml, Files.PlmXml.PlmXmlErrorCode.Undefined
                        MessageBoxEx.ShowError(DialogStrings.UnexpectedErrorPlmXml_Msg + vbCrLf + result.Message)
                    Case Files.PlmXml.PlmXmlErrorCode.UnsupportedPlmXmlVersion
                        MessageBoxEx.ShowError(DialogStrings.UnsupportedPlmXmlVersion_Msg)
                    Case Else
                        Throw New NotImplementedException($"Errorcode ""{result.ErrorCode}"" is not implemented!")
                End Select
            End If
        Catch ex As Exception
            ex.ShowMessageBox(String.Format("Error while importing PLM XML file!{0}{1}.", vbCrLf, ex.Message))
        End Try
    End Function

    Private Async Sub OpenE3XmlDocument(filePath As String)
        Dim m As New E3.Lib.Model.EESystemModel
        Dim messsages As New List(Of MessageInfo)
        Dim importResult As ImportE3XmlResult = m.ImportE3Xml(filePath, GetOrAddNewHarness(m))

        If importResult.IsFaulted Then
            messsages.Add(New MessageInfo(NameOf(PlmXmlTopologyImporter), importResult.Message, MessageType.Error))
        Else
            Dim resExport As KblContainerResult = importResult.ExportToKbl()
            If resExport.IsSuccess Then
                Using kbl_temp_file As TempFile = TempFile.Create(filePath, KnownFile.KBL, True) ' do not delete on dispose because the kbl_file will do that after it doesn't need it anymore
                    KBL_container.SaveTo(kbl_temp_file.Name, CType(resExport.KblContainer, KBL_container))
                    Dim kbl As KblFile = KblFile.Create(kbl_temp_file.Name, Project.Common.ZIP_CACHING_METHOD, False) ' HINT: delete file on dispose (normally when closed) because the kbl was generated from a temp-file
                    Await OpenKBLDocument(kbl, False, String.Empty, True)
                End Using
            End If
        End If

        Try
            If Me._mainForm.ActiveDocument IsNot Nothing AndAlso messsages IsNot Nothing Then
                For Each msg As MessageInfo In messsages
                    Me._mainForm.ActiveDocument._logHub.WriteLogMessage(New LogEventArgs(LogEventArgs.LoggingLevel.Error, msg.Text))
                Next
            Else
                MessageBoxEx.ShowError(String.Join(vbCrLf, messsages.Select(Function(e) e.Text)))
            End If

            _mainForm.Tools.Application.ExportKbl.Enabled = True
        Catch ex As Exception
#If DEBUG Or CONFIG = "Debug" Then
            Throw
#Else
            If Me._mainForm?.ActiveDocument IsNot Nothing Then
                Me._mainForm.ActiveDocument._logHub.WriteLogMessage(New LogEventArgs(LogEventArgs.LoggingLevel.Error, ex.Message))
            Else
                MessageBoxEx.Show(String.Format("Error while importing E3 XML file!{0}{1}.", vbCrLf, ex.Message))
            End If
#End If
        End Try
    End Sub

    Private Shared Function GetOrAddNewHarness(m As E3.Lib.Model.EESystemModel) As E3.Lib.Model.Harness
        SyncLock m
            If m.Harnesses.Count = 0 Then
                Return m.Harnesses.AddNew
            End If
            Return m.Harnesses.First
        End SyncLock
    End Function

    Public Function IsOpenHcvDocumentAllowed(hcv As HcvFile) As Boolean
        If _utmmMain.TabGroups.Count > 0 Then
            For Each tabGroup As MdiTabGroup In _utmmMain.TabGroups
                For Each documentTab As MdiTab In tabGroup.Tabs
                    If documentTab.Key = hcv.FullName Then
                        MessageBoxEx.ShowError(ErrorStrings.MainStatMachine_HCVAlreadyOpened_Msg)
                        _mainForm.UseWaitCursor = False
                        Return False
                    End If
                Next
            Next
        End If

        If (_compareForm?.Visible).GetValueOrDefault OrElse (_graphicalCompareForm?.Visible).GetValueOrDefault Then
            If MessageBoxEx.ShowQuestion(DialogStrings.MainStatMachine_CloseCompDlgWhenOpeningHCV_Msg) = DialogResult.No Then
                _mainForm.UseWaitCursor = False
                Return False
            End If
        End If

        If (_compareForm?.Visible).GetValueOrDefault Then
            _compareForm.Close()
            _compareForm.Dispose()
        End If

        If (_graphicalCompareForm?.Visible).GetValueOrDefault Then
            _graphicalCompareForm.Close()

            If Not _graphicalCompareForm.Visible Then
                _graphicalCompareForm.Dispose()
            End If
        End If

        _mainForm.SearchMachine.VisibleSearchForm(False)
        Return True
    End Function

    Private Async Function OpenSingleHCVDocument(hcv As HcvFile, fromTopologyContainer As Boolean, Optional loadDrawings As Boolean = True, Optional doPreChecks As Boolean = True) As Task(Of DocumentForm)
        If Not doPreChecks OrElse IsOpenHcvDocumentAllowed(hcv) Then

            ' Identify module part numbers for activation inside file name
            Dim modulePartNumbersForActivation As List(Of String) = GetModulePartNumbersFromFileName(IO.Path.GetFileName(hcv.FullName))

            ' Create new tab with a new instance of a DocumentForm class and open the HCV file
            Dim documentForm As New DocumentForm(_mainForm, hcv)
            documentForm.Show()

            Dim res As IResult = Await documentForm.OpenDocument(modulePartNumbersForActivation, loadDrawings)
            Await TaskEx.WaitUntil(Function() Not documentForm.IsBusy)
            OnOpenDocumentFinished(documentForm)

            If (res?.IsSuccess).GetValueOrDefault Then
                Dim mdiTab As MdiTab = _utmmMain.TabFromForm(documentForm)
                If mdiTab IsNot Nothing Then
                    mdiTab.Key = hcv.FullName

                    If fromTopologyContainer Then
                        mdiTab.Settings.TabAppearance.BackColor = Color.LightSkyBlue
                        mdiTab.Settings.TabAppearance.Image = My.Resources.EntireCarContainer.ToBitmap
                    End If
                End If
                Return documentForm
            Else
                documentForm.Close()
            End If
        End If
        Return Nothing
    End Function

    Friend Sub PrintDocument(Optional printing As Printing.VdPrinting = Nothing)
        If _mainForm.ActiveDocument IsNot Nothing Then
            If _mainForm.ActiveDocument.ActiveDrawingCanvas IsNot Nothing Then
                _mainForm.ActiveDocument.ActiveDrawingCanvas.vdCanvas.BaseControl.ActiveDocument.ActiveLayOut.Label = "Printing"

                If printing Is Nothing Then
                    printing = New HarnessAnalyzer.Printing.VdPrinting(_mainForm.ActiveDocument.ActiveDrawingCanvas.vdCanvas.BaseControl.ActiveDocument)
                    printing.DocumentName = _mainForm.ActiveDocument.Text
                End If

                Dim printSel As VectorDraw.Geometry.Box = Nothing
                Using printForm As New Printing.PrintForm(printing, _mainForm.SchematicsView)
                    If printForm.ShowDialog(_mainForm) = DialogResult.Ignore Then
                        _mainForm.ActiveDocument.ActiveDrawingCanvas.vdCanvas.BaseControl.ActiveDocument.ActionUtility.getUserRectViewCS(Nothing, printSel)
                    End If
                End Using

                If _mainForm.ActiveDocument IsNot Nothing AndAlso _mainForm.ActiveDocument.ActiveDrawingCanvas.vdCanvas IsNot Nothing AndAlso _mainForm.ActiveDocument.ActiveDrawingCanvas.vdCanvas.BaseControl IsNot Nothing AndAlso _mainForm.ActiveDocument.ActiveDrawingCanvas.vdCanvas.BaseControl.ActiveDocument IsNot Nothing Then
                    _mainForm.ActiveDocument.ActiveDrawingCanvas.vdCanvas.BaseControl.ActiveDocument.ActiveLayOut.Label = String.Empty
                End If

                If printSel IsNot Nothing Then
                    printing.PrintSelection = printSel
                    PrintDocument(printing)
                End If
            Else
                If _mainForm.ActiveDocument.IsDocument3DActive Then
                    _IsPickUpWindowClicked = False

                    Dim eyeshotCntrl As New EyeshotPrinting(_mainForm.ActiveDocument._D3DControl.Model3DControl1.Design, _isPrintingAreaFromModel3DSelected)
                    eyeshotCntrl.Model3d.ActionMode = devDept.Eyeshot.actionType.None

                    RemoveHandler eyeshotCntrl.Model3d.AfterActionModeChanged, AddressOf Model3d_AfterActionModeChanged
                    AddHandler eyeshotCntrl.Model3d.AfterActionModeChanged, AddressOf Model3d_AfterActionModeChanged

                    Using printForm As New Printing.PrintForm(eyeshotCntrl)
                        If printForm.ShowDialog(_mainForm) = DialogResult.Ignore Then
                            _isPrintingAreaFromModel3DSelected = eyeshotCntrl.IsPrintWindowSelected
                            _mainForm.ActiveDocument._D3DControl.CloseAllToolTips(forcePinned:=True)
                            _mainForm.ActiveDocument._D3DControl.Model3DControl1.Design.SetButtonActionMode(Of devDept.Eyeshot.ZoomWindowToolBarButton)(True) ' Hint : similar to document3DView.SetZoomWindowActionMode()
                        Else
                            _isPrintingAreaFromModel3DSelected = False
                        End If
                    End Using
                Else
                    MessageBoxEx.ShowInfo(DialogStrings.MainStatMachine_NoDrwOpened_Msg2)
                End If
            End If
        Else
            MessageBoxEx.ShowInfo(DialogStrings.MainStatMachine_NoDrwOpened_Msg)
        End If
    End Sub

    Private Function CheckIfItsZoomWindowOfPrintForm() As Boolean
        Return If(_mainForm.ActiveDocument.utmDocument.ActiveTool IsNot Nothing, False, True)
    End Function

    Private Sub Model3d_AfterActionModeChanged(ByVal sender As Object, ByVal e As devDept.Eyeshot.ActionModeEventArgs)
        If CheckIfItsZoomWindowOfPrintForm() Then
            If _IsPickUpWindowClicked AndAlso _isPrintingAreaFromModel3DSelected AndAlso e.Button Is Nothing AndAlso e.Action = devDept.Eyeshot.actionType.SelectVisibleByPickDynamic Then
                _mainForm.ActiveDocument._D3DControl.Model3DControl1.Design.Cursor = Cursors.Default
                _mainForm.ActiveDocument._D3DControl.Model3DControl1.Design.Refresh()
                _mainForm.ActiveDocument._D3DControl.Refresh()

                PrintDocument() 'After area selection finished
            End If

            If e.Button IsNot Nothing AndAlso e.Action = devDept.Eyeshot.actionType.ZoomWindow Then
                If e.Button.Name = devDept.Eyeshot.actionType.ZoomWindow.ToString Then
                    _IsPickUpWindowClicked = True ' selecting window or area
                    _mainForm.ActiveDocument._D3DControl.Model3DControl1.Design.Cursor = Cursors.Cross
                    _mainForm.ActiveDocument._D3DControl.CloseAllToolTips(forcePinned:=True)
                End If
            End If
        End If
    End Sub

    Friend Sub SaveDocument()
        Using _mainForm.EnableWaitCursor
            If _mainForm.ActiveDocument IsNot Nothing Then
                If Not _mainForm.ActiveDocument.SaveDocument() Then
                    Return ' abort when failed
                End If
            End If

            _mainForm.Tools.Application.SaveDocument.SharedProps.Enabled = False

            Dim documentDirty As Boolean = False
            If _mainForm.utmmMain.TabGroups.Count > 0 Then
                For Each tabGroup As MdiTabGroup In _mainForm.utmmMain.TabGroups
                    For Each tab As MdiTab In tabGroup.Tabs
                        Dim documentForm As DocumentForm = DirectCast(tab.Form, DocumentForm)
                        If documentForm.IsDirty Then
                            documentDirty = True
                            Exit For
                        End If
                    Next
                Next
            End If

            If Not documentDirty Then
                _mainForm.Tools.Application.SaveDocuments.SharedProps.Enabled = False
            End If
        End Using
    End Sub

    Private Sub SaveDocumentAs()
        If _mainForm.ActiveDocument IsNot Nothing Then
            Using sfdDocument As New SaveFileDialog
                With sfdDocument
                    .DefaultExt = KnownFile.HCV.TrimStart("."c)
                    .FileName = _mainForm.ActiveDocument.Text
                    .Filter = "Harness Container for Vehicles files (*.hcv)|*.hcv" 'TODO?: All the file Filters need to be Localized
                    .Title = DialogStrings.MainStatMachine_SaveHCVToFile_Title
                End With

                If sfdDocument.ShowDialog(_mainForm) = DialogResult.OK Then
                    Using _mainForm.EnableWaitCursor
                        Dim selectedFileAlreadyOpened As Boolean = False

                        For Each tabGroup As MdiTabGroup In _mainForm.utmmMain.TabGroups
                            For Each tab As MdiTab In tabGroup.Tabs
                                If tab.Key = sfdDocument.FileName Then
                                    selectedFileAlreadyOpened = True
                                    Exit For
                                End If
                            Next
                        Next

                        If Not selectedFileAlreadyOpened Then
                            Try
                                If _mainForm.ActiveDocument.SaveDocument(sfdDocument.FileName) Then
                                    If Not _mainForm.ActiveDocument.IsExtendedHCV Then
                                        _mainForm.ActiveDocument.Text = IO.Path.GetFileNameWithoutExtension(sfdDocument.FileName)

                                        Dim mdiTab As MdiTab = _mainForm.utmmMain.TabFromForm(_mainForm.ActiveDocument)
                                        mdiTab.Key = sfdDocument.FileName
                                    End If

                                    MessageBoxEx.ShowInfo(DialogStrings.MainStatMachine_SaveHCVSuccess_Msg)
                                Else
                                    MessageBoxEx.ShowError(String.Format(ErrorStrings.MainStatMachine_ErrorSaveHCV_Msg, vbCrLf, "<Unspecified>"))
                                End If
                            Catch ex As Exception
#If DEBUG Or CONFIG = "Debug" Then
                                Throw
#Else
                                MessageBoxex.ShowError(String.Format(ErrorStrings.MainStatMachine_ErrorSaveHCV_Msg, vbCrLf, ex.Message))
#End If

                            End Try

                            _mainForm.Tools.Application.SaveDocument.SharedProps.Enabled = False

                            Dim documentDirty As Boolean = False

                            If _mainForm.utmmMain.TabGroups.Count > 0 Then
                                For Each tabGroup As MdiTabGroup In _mainForm.utmmMain.TabGroups
                                    For Each tab As MdiTab In tabGroup.Tabs
                                        Dim documentForm As DocumentForm = DirectCast(tab.Form, DocumentForm)
                                        If documentForm.IsDirty Then
                                            documentDirty = True
                                            Exit For
                                        End If
                                    Next
                                Next
                            End If

                            If Not documentDirty Then
                                _mainForm.Tools.Application.SaveDocuments.SharedProps.Enabled = False
                            End If
                        Else
                            MessageBoxEx.ShowInfo(DialogStrings.MainStatMachine_CannotOvwrHCVFile_Msg)
                        End If
                    End Using
                End If
            End Using
        End If
    End Sub

    Private Sub SaveDocuments()
        Using _mainForm.EnableWaitCursor
            If _mainForm.utmmMain.TabGroups.Count > 0 Then
                For Each tabGroup As MdiTabGroup In _mainForm.utmmMain.TabGroups
                    For Each tab As MdiTab In tabGroup.Tabs
                        Dim documentForm As DocumentForm = DirectCast(tab.Form, DocumentForm)
                        If documentForm.IsDirty Then
                            documentForm.SaveDocument()
                        End If
                    Next
                Next
            End If

            _mainForm.Tools.Application.SaveDocument.SharedProps.Enabled = False
            _mainForm.Tools.Application.SaveDocuments.SharedProps.Enabled = False
        End Using
    End Sub

    Private Sub SearchDocuments()
        _mainForm.SearchMachine.CreateSearchFormWithPredefinedSearchString(_mainForm.SearchMachine.BeginsWithEnabled, _mainForm.SearchMachine.CaseSensitiveEnabled, False, _mainForm.SearchMachine.CurrentSearchString)
    End Sub

    Private Sub ShowDataTableSettings()
        Using gridAppearanceForm As New GridAppearanceForm
            If gridAppearanceForm.ShowDialog(_mainForm) = DialogResult.OK Then
                MessageBoxEx.ShowInfo(DialogStrings.MainStatMachine_CloseAppFirst_Msg)
            End If
        End Using
    End Sub

    Friend Sub ShowGeneralSettings(Optional activeTab As String = Nothing)
        Dim showFullScreenAxisCursor As Boolean = _mainForm.GeneralSettings.ShowFullScreenAxisCursor
        Dim cableLengthTypes As List(Of String) = Nothing
        Dim wireLengthTypes As List(Of String) = Nothing

        Using _mainForm.EnableWaitCursor
            If _mainForm.utmmMain.TabGroups.Count > 0 Then
                cableLengthTypes.NewOrClear
                wireLengthTypes.NewOrClear

                For Each tabGroup As MdiTabGroup In _mainForm.utmmMain.TabGroups
                    For Each tab As MdiTab In tabGroup.Tabs
                        Dim documentForm As DocumentForm = DirectCast(tab.Form, DocumentForm)

#If DEBUG Or CONFIG = "Debug" Then
                        'TODO: KblCableLengthTypes was nothing after xhcv was closed after some tests, coudn't reproduce it so added safety nothing check for release here but in debug exception is thrown to insetigate the error because normally this shoudn't happen when everything works flawlessly
                        If documentForm?.KBL?.KBLCableLengthTypes Is Nothing Then
                            Throw New NullReferenceException("KBLCableLengthTypes in KBlMapper seems to be nothing (check also document and KblMapper), this error shoudn't happen here but will be skipped in release-mode")
                        End If
#End If
                        If documentForm?.KBL?.KBLCableLengthTypes IsNot Nothing Then
                            For Each cableLengthType As String In documentForm.KBL.KBLCableLengthTypes
                                cableLengthTypes.TryAdd(cableLengthType)
                            Next
                        End If

#If DEBUG Or CONFIG = "Debug" Then
                        'TODO: KBLWireLengthTypes could be also nothing (due to exception happened before i coudn't find this really out, only assuming here) after xhcv was closed after some tests, coudn't reproduce it so added safety nothing check for release here but in debug exception is thrown to insetigate the error because normally this shoudn't happen when everything works flawlessly
                        If documentForm?.KBL?.KBLWireLengthTypes Is Nothing Then
                            Throw New NullReferenceException("KBLWireLengthTypes in KBlMapper seems to be nothing (check also document and KblMapper), this error shoudn't happen here but will be skipped in release-mode")
                        End If
#End If
                        If documentForm?.KBL?.KBLWireLengthTypes IsNot Nothing Then
                            For Each wireLengthType As String In documentForm.KBL?.KBLWireLengthTypes
                                wireLengthTypes.TryAdd(wireLengthType)
                            Next
                        End If
                    Next
                Next
            End If

            Using generalSettingsForm As New GeneralSettingsForm(_mainForm.GeneralSettings, cableLengthTypes, wireLengthTypes)
                generalSettingsForm.ShowDialog(_mainForm, activeTab)

                If generalSettingsForm.DefaultLengthTypeChanged AndAlso _mainForm.utmmMain.TabGroups.Count > 0 Then
                    UpdateCableWireLengthsInDataTables(cableLengthTypes, wireLengthTypes)
                End If

                If generalSettingsForm.RecentFileListCleared Then
                    UpdateRecentFileList(_mainForm.Tools.Application.RecentList)
                End If

#If CONFIG = "Debug" Or DEBUG Then
                If generalSettingsForm.FilePreviewAddonsChecked Then
                    If Not [Shared].Utilities.IsFilePreviewerRegistered Then
                        If Not [Shared].Utilities.RegisterFilePreviewer() Then
                            Debug.WriteLine("WARNING for experimental feature: could not register File-Preview-Handler! (only in debug mode)")
                        End If
                    End If
                ElseIf [Shared].Utilities.IsFilePreviewerRegistered Then
                    If Not [Shared].Utilities.UnRegisterFilePreviewer() Then
                        Debug.WriteLine("WARNING for experimental feature: could not un-register File-Preview-Handler! (only in debug mode)")
                    End If
                End If
#End If

                CType(_mainForm.D3D, D3D.MainFormController).UpdateFromGeneralSettings()

                If _mainForm.SchematicsView IsNot Nothing Then
                    _mainForm.SchematicsView.AutoZoomSelection = _mainForm.GeneralSettings.AutoZoomSelectionSchematics
                End If
            End Using

            If (showFullScreenAxisCursor <> _mainForm.GeneralSettings.ShowFullScreenAxisCursor) AndAlso (_mainForm.utmmMain.TabGroups.Count > 0) Then
                _mainForm.utmmMain.BeginUpdate()

                For Each tabGroup As MdiTabGroup In _mainForm.utmmMain.TabGroups
                    For Each tab As MdiTab In tabGroup.Tabs
                        For Each childTab As UltraTab In DirectCast(tab.Form, DocumentForm).utcDocument.Tabs
                            Dim child_drawing_canvas As DrawingCanvas = TryCast(childTab.TabPage.Controls(0), DrawingCanvas)
                            If child_drawing_canvas IsNot Nothing Then
                                If _mainForm.GeneralSettings.ShowFullScreenAxisCursor Then
                                    child_drawing_canvas.vdCanvas.BaseControl.ActiveDocument.GlobalRenderProperties.AxisSize = E3.Lib.Converter.Svg.Defaults.VDRAW_LIMIT
                                Else
                                    child_drawing_canvas.vdCanvas.BaseControl.ActiveDocument.GlobalRenderProperties.AxisSize = 10
                                End If
                            End If
                        Next
                    Next
                Next
                _mainForm.utmmMain.EndUpdate()
            End If
        End Using
    End Sub

    Private Sub ShowInliners()
        Dim documentForms As New List(Of DocumentForm)
        Dim inactiveInlinerPairs As Integer = 0
        Dim trivialInlinerPairs As New List(Of TrivialInlinerPair)
        Dim virtualInlinerPairs As New List(Of VirtualInlinerPair)
        Dim d As New Dictionary(Of HcvDocument, DocumentForm)

        For Each tabGroup As MdiTabGroup In _utmmMain.TabGroups
            For Each documentTab As MdiTab In tabGroup.Tabs
                Dim document As DocumentForm = CType(documentTab.Form, DocumentForm)
                d.Add(document.Document, DirectCast(documentTab.Form, DocumentForm))

                If Not document.IsExtendedHCV Then
                    Continue For
                End If

                Dim kblMapper As KblMapper = document.KBL

                For Each inlinerPair As KeyValuePair(Of Connector_occurrence, Connector_occurrence) In document.Inliners
                    If (inlinerPair.Key IsNot Nothing) AndAlso (inlinerPair.Value IsNot Nothing) Then
                        If (_virtualInlinerPairs Is Nothing OrElse Not _virtualInlinerPairs.Any(Function(vip) vip.ConnectorOccsWithKblMapperA.ContainsKey(inlinerPair.Key) OrElse vip.ConnectorOccsWithKblMapperA.ContainsKey(inlinerPair.Value) OrElse vip.ConnectorOccsWithKblMapperB.ContainsKey(inlinerPair.Key) OrElse vip.ConnectorOccsWithKblMapperB.ContainsKey(inlinerPair.Value))) Then
                            Dim connectorFace As VdSVGGroup = If(document.ActiveDrawingCanvas IsNot Nothing AndAlso document.ActiveDrawingCanvas.GroupMapper.ContainsKey(inlinerPair.Key.SystemId), document.ActiveDrawingCanvas.GroupMapper(inlinerPair.Key.SystemId).Values.Where(Function(g) g.SVGType = SvgType.Undefined.ToString).FirstOrDefault, Nothing)
                            If Not document.InactiveObjects.ContainsValue(KblObjectType.Connector_occurrence, inlinerPair.Key.SystemId) Then
                                If trivialInlinerPairs.Any(Function(p) p.InlinerIdA = inlinerPair.Value.Id) Then
                                    With trivialInlinerPairs.SingleOrDefault(Function(p) p.InlinerIdA = inlinerPair.Value.Id)
                                        If Not .ConnectorFacesB.ContainsKey(inlinerPair.Key.Id) Then
                                            .ConnectorFacesB.Add(inlinerPair.Key.Id, inlinerPair.Key.SystemId, connectorFace)
                                        End If

                                        .ConnectorOccB = inlinerPair.Key
                                        .Id = String.Format("{0}|{1}", .InlinerIdA, inlinerPair.Value.Id)
                                        .InactiveObjectsB = document.InactiveObjects
                                        .InlinerIdB = inlinerPair.Key.Id
                                        .KblMapperB = kblMapper
                                        .DocumentB = document.Document
                                    End With
                                ElseIf (Not trivialInlinerPairs.Any(Function(p) p.InlinerIdA = inlinerPair.Key.Id)) Then
                                    trivialInlinerPairs.Add(New TrivialInlinerPair(connectorFace, inlinerPair.Key, document.InactiveObjects, inlinerPair.Key.Id, kblMapper))
                                    trivialInlinerPairs.Last.DocumentA = document.Document
                                End If
                            Else
                                inactiveInlinerPairs += 1
                            End If
                        End If
                    End If
                Next

                documentForms.Add(document)
            Next
        Next

        For Each inlp As VirtualInlinerPair In _virtualInlinerPairs
            If inlp.DocumentA IsNot Nothing AndAlso d.ContainsKey(inlp.DocumentA) Then
                inlp.InactiveObjectsA = d(inlp.DocumentA).InactiveObjects
            End If

            If inlp.DocumentB IsNot Nothing AndAlso d.ContainsKey(inlp.DocumentB) Then
                inlp.InactiveObjectsB = d(inlp.DocumentB).InactiveObjects
            End If

            If inlp.DocumentA IsNot Nothing AndAlso inlp.DocumentB IsNot Nothing Then
                virtualInlinerPairs.Add(inlp)
            End If

            inactiveInlinerPairs += inlp.InactiveInliners()
        Next

        If trivialInlinerPairs.Count > 0 OrElse virtualInlinerPairs.Count > 0 Then
            _inlinersForm = New InlinersForm(virtualInlinerPairs, trivialInlinerPairs, _mainForm.GeneralSettings.InlinerPairCheckClassifications, inactiveInlinerPairs)
            _inlinersForm.ShowDialog(_mainForm)
        Else
            MessageBoxEx.ShowInfo(DialogStrings.MainStatMachine_NoInlPairFound_Msg)
        End If
    End Sub

    Private Sub UpdateCableWireLengthsInDataTables(cableLengthTypes As List(Of String), wireLengthTypes As List(Of String))
        Dim cableGridAppearance As CableGridAppearance = GridAppearance.All.OfType(Of CableGridAppearance).Single
        Dim wireGridAppearance As WireGridAppearance = GridAppearance.All.OfType(Of WireGridAppearance).Single

        For Each tabGroup As MdiTabGroup In _mainForm.utmmMain.TabGroups
            For Each tab As MdiTab In tabGroup.Tabs
                Dim documentForm As DocumentForm = DirectCast(tab.Form, DocumentForm)
                documentForm._informationHub.ugCables.BeginUpdate()

                With documentForm._informationHub.ugCables
                    If (.DisplayLayout.Bands.Count > 0) AndAlso (.DisplayLayout.Bands(0).Columns.Exists(CablePropertyName.Length_Information.ToString)) AndAlso (cableLengthTypes.Count > 1) AndAlso (_mainForm.GeneralSettings.DefaultCableLengthType IsNot Nothing AndAlso _mainForm.GeneralSettings.DefaultCableLengthType.IsNotNullOrEmpty) Then
                        .DisplayLayout.Bands(0).Columns(CablePropertyName.Length_Information).Header.Caption = String.Format("{0} [{1}]", cableGridAppearance.GridTable.GridColumns.FindGridColumn(CablePropertyName.Length_Information).Single.Name, _mainForm.GeneralSettings.DefaultCableLengthType.SplitSpace(0))
                    End If

                    If (.DisplayLayout.Bands.Count > 1) AndAlso (.DisplayLayout.Bands(1).Columns.Exists(WirePropertyName.Length_information)) AndAlso (wireLengthTypes.Count > 1) AndAlso (_mainForm.GeneralSettings.DefaultWireLengthType IsNot Nothing AndAlso _mainForm.GeneralSettings.DefaultWireLengthType.IsNotNullOrEmpty) Then
                        .DisplayLayout.Bands(1).Columns(WirePropertyName.Length_information).Header.Caption = String.Format("{0} [{1}]", cableGridAppearance.GridTable.GridSubTable.GridColumns.FindGridColumn(WirePropertyName.Length_information).Single.Name, _mainForm.GeneralSettings.DefaultWireLengthType.SplitSpace(0))
                    End If

                    For Each row As UltraGridRow In .Rows

                        If row.Cells.Exists(CablePropertyName.Length_Information) Then
                            Dim cable_occ As Special_wire_occurrence = documentForm.KBL.GetOccurrenceObject(Of Special_wire_occurrence)(row.Tag?.ToString)
                            If cable_occ.Length_information.Length > 0 Then
                                With row.Cells(CablePropertyName.Length_Information)
                                    Dim lengthTypesOnCable As New List(Of String)

                                    For Each wireLength As Wire_length In cable_occ.Length_information
                                        lengthTypesOnCable.Add(wireLength.Length_type)
                                    Next

                                    If lengthTypesOnCable.Count > 1 OrElse (Not lengthTypesOnCable.Any(Function(lt) lt.ToLower = _mainForm.GeneralSettings.DefaultCableLengthType.ToLower)) Then
                                        .Appearance.Image = My.Resources.About.ToBitmap
                                        .Style = ColumnStyle.Button
                                        .Tag = documentForm.KBL.KBLOccurrenceMapper(row.Tag?.ToString)
                                    Else
                                        .Appearance.Image = Nothing
                                        .Style = ColumnStyle.Default
                                        .Tag = Nothing
                                    End If
                                End With
                            End If
                        End If
                        If row.HasChild Then
                            For Each childRow As UltraGridRow In row.ChildBands(0).Rows
                                If childRow.Cells.Exists(WirePropertyName.Length_information) Then
                                    Dim core_occ As Core_occurrence = documentForm.KBL.GetOccurrenceObject(Of Core_occurrence)(childRow.Cells(Zuken.E3.HarnessAnalyzer.Shared.SYSTEM_ID).Value.ToString)
                                    If core_occ.Length_information.Length > 0 Then
                                        With childRow.Cells(WirePropertyName.Length_information)
                                            Dim lengthTypesOnWire As New List(Of String)

                                            For Each wireLength As Wire_length In core_occ.Length_information
                                                lengthTypesOnWire.Add(wireLength.Length_type)
                                            Next

                                            If lengthTypesOnWire.Count > 1 OrElse Not lengthTypesOnWire.Any(Function(lt) lt.ToLower = _mainForm.GeneralSettings.DefaultWireLengthType.ToLower) Then
                                                .Appearance.Image = My.Resources.About.ToBitmap
                                                .Style = ColumnStyle.Button
                                                .Tag = documentForm.KBL.GetOccurrenceObjectUntyped(childRow.Tag?.ToString)
                                            Else
                                                .Appearance.Image = Nothing
                                                .Style = ColumnStyle.Default
                                                .Tag = Nothing
                                            End If
                                        End With
                                    End If
                                End If
                            Next
                        End If
                    Next
                End With

                documentForm._informationHub.ugCables.EndUpdate()
                documentForm._informationHub.ugWires.BeginUpdate()

                With documentForm._informationHub.ugWires
                    If (.DisplayLayout.Bands.Count > 0) AndAlso (.DisplayLayout.Bands(0).Columns.Exists(WirePropertyName.Length_information.ToString)) AndAlso (wireLengthTypes.Count > 1) AndAlso (_mainForm.GeneralSettings.DefaultWireLengthType.IsNotNullOrEmpty) Then
                        .DisplayLayout.Bands(0).Columns(WirePropertyName.Length_information.ToString).Header.Caption = String.Format("{0} [{1}]", wireGridAppearance.GridTable.GridColumns.FindGridColumn(WirePropertyName.Length_information).Single.Name, _mainForm.GeneralSettings.DefaultWireLengthType.SplitSpace(0))
                    End If

                    For Each row As UltraGridRow In .Rows
                        Dim wireLengthes As List(Of Wire_length) = Nothing
                        Dim kbl_wire_core As IKblWireCoreOccurrence = documentForm.KBL.GetWireOrCoreOccurrence(row.Tag?.ToString)
                        wireLengthes = kbl_wire_core.Length_information.ToList

                        If row.Cells.Exists(WirePropertyName.Length_information.ToString) AndAlso wireLengthes.Count > 0 Then
                            With row.Cells(WirePropertyName.Length_information.ToString)
                                Dim lengthTypesOnWire As New List(Of String)

                                For Each wireLength As Wire_length In wireLengthes
                                    lengthTypesOnWire.Add(wireLength.Length_type)
                                Next

                                If lengthTypesOnWire.Count > 1 OrElse Not lengthTypesOnWire.Any(Function(lt) lt.ToLower = _mainForm.GeneralSettings.DefaultWireLengthType.ToLower) Then
                                    If (.Appearance.Image Is Nothing) Then
                                        .Appearance.Image = My.Resources.About.ToBitmap
                                    End If

                                    .Style = ColumnStyle.Button
                                    .Tag = documentForm.KBL.KBLOccurrenceMapper(row.Tag?.ToString)
                                Else
                                    .Appearance.Image = Nothing
                                    .Style = ColumnStyle.Default
                                    .Tag = Nothing
                                End If
                            End With
                        End If
                    Next
                End With

                documentForm._informationHub.ugWires.EndUpdate()
            Next
        Next
    End Sub

    Private Sub UpdateRecentFileList(recentListTool As ListTool)
        recentListTool.ListToolItems.Clear()
        If _mainForm.GeneralSettings.RecentFiles IsNot Nothing Then
            For Each recentFile As RecentFile In _mainForm.GeneralSettings.RecentFiles
                recentListTool.ListToolItems.Add(IO.Path.Combine(recentFile.DirectoryName, recentFile.FileName), recentFile.FileName)
                recentListTool.ListToolItems(IO.Path.Combine(recentFile.DirectoryName, recentFile.FileName)).Appearance.Image = My.Resources.RecentDocuments.ToBitmap
                recentListTool.ListToolItems(IO.Path.Combine(recentFile.DirectoryName, recentFile.FileName)).DescriptionOnMenu = Infragistics.Win.FormattedLinkLabel.ParsedFormattedTextValue.EscapeXML(IO.Path.Combine(recentFile.DirectoryName, recentFile.FileName))
            Next
        End If
    End Sub

    Public Sub HighlightCompareHubSelection(sender As InformationHub, e As InformationHubEventArgs)
        _compareForm_CompareHubSelectionChanged(sender, e)
    End Sub

    Private Sub _compareForm_CompareHubSelectionChanged(sender As InformationHub, e As InformationHubEventArgs) Handles _compareForm.CompareHubSelectionChanged
        If Not String.IsNullOrEmpty(e.KblMapperSourceId) Then
            For Each tabGroup As MdiTabGroup In _utmmMain.TabGroups
                For Each documentTab As MdiTab In tabGroup.Tabs
                    Dim documentFrm As DocumentForm = DirectCast(documentTab.Form, DocumentForm)
                    Using documentFrm.EnableWaitCursor
                        If (documentFrm._informationHub IsNot Nothing) AndAlso e.KblMapperSourceId.IsNotNullOrEmpty AndAlso (documentFrm.KBL.Id = e.KblMapperSourceId) Then
                            documentFrm.SelectRowsInGrids(e.ObjectIds, True, True)
                            If (e.ObjectIds.Count <> 0) AndAlso (documentFrm.KBL.KBLOccurrenceMapper.ContainsKey(e.ObjectIds.FirstOrDefault)) AndAlso (Not documentTab.IsFormActive) Then
                                _selectionChangedBySystem = True
                                documentTab.Activate()
                                _selectionChangedBySystem = False
                            End If
                        End If
                    End Using
                Next
            Next
        End If
    End Sub

    Private Sub _compareForm_FormClosed(sender As Object, e As FormClosedEventArgs) Handles _compareForm.FormClosed
        For Each tabGroup As MdiTabGroup In _utmmMain.TabGroups
            For Each documentTab As MdiTab In tabGroup.Tabs
                If documentTab.Form.Text.EndsWith(UIStrings.ReferenceSuffix_Caption) AndAlso Not documentTab.IsFormActive Then
                    _selectionChangedBySystem = True
                    documentTab.Activate()
                    _selectionChangedBySystem = False
                End If

                documentTab.Form.Text = IO.Path.GetFileNameWithoutExtension(documentTab.Key)
                documentTab.Settings.ActiveTabAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.False
            Next
        Next
    End Sub

    Private Sub _compareForm_Load(sender As Object, e As EventArgs) Handles _compareForm.Load
        _utmmMain.ActiveTab.Form.Text = String.Format("{0} {1}", _utmmMain.ActiveTab.Form.Text, UIStrings.ReferenceSuffix_Caption)
        _utmmMain.ActiveTab.Settings.ActiveTabAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True
    End Sub

    Private Sub _compareForm_LogMessage(sender As CompareForm, e As LogEventArgs) Handles _compareForm.LogMessage
        _mainForm?.ActiveDocument?._logHub.WriteLogMessage(e)
    End Sub

    Private Sub _documentFilterControl_ItemCheckedChanged(sender As Object, e As DocumentFilterControl.ItemCheckedEventArgs) Handles _documentFilterControl.ItemCheckedChanged
        RaiseEvent DocumentFilterItemCheckedChanged(Me, e)
    End Sub

    Private Sub _graphicalCompareForm_FormClosed(sender As Object, e As FormClosedEventArgs) Handles _graphicalCompareForm.FormClosed
        For Each tabGroup As MdiTabGroup In _utmmMain.TabGroups
            For Each documentTab As MdiTab In tabGroup.Tabs
                documentTab.Form.Text = IO.Path.GetFileNameWithoutExtension(documentTab.Key)
                documentTab.Settings.ActiveTabAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.False
            Next
        Next
    End Sub

    Private Sub _graphicalCompareForm_Load(sender As Object, e As EventArgs) Handles _graphicalCompareForm.Load
        _utmmMain.ActiveTab.Form.Text = String.Format("{0} {1}", _utmmMain.ActiveTab.Form.Text, UIStrings.ReferenceSuffix_Caption)
        _utmmMain.ActiveTab.Settings.ActiveTabAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True
    End Sub

    Private Sub _inlinersForm_InlinerSelectionChanged(sender As Object, e As InformationHubEventArgs) Handles _inlinersForm.InlinerSelectionChanged
        For Each tabGroup As MdiTabGroup In _utmmMain.TabGroups
            For Each documentTab As MdiTab In tabGroup.Tabs
                Dim documentFrm As DocumentForm = DirectCast(documentTab.Form, DocumentForm)
                Using documentFrm.EnableWaitCursor
                    If (documentFrm._informationHub IsNot Nothing) AndAlso (documentFrm.KBL.Id = e.KblMapperSourceId) Then
                        documentFrm.SelectRowsInGrids(e.ObjectIds, True, True)
                        If documentFrm.KBL.KBLOccurrenceMapper.ContainsKey(e.ObjectIds.FirstOrDefault) AndAlso Not documentTab.IsFormActive Then
                            _selectionChangedBySystem = True
                            documentTab.Activate()
                            _selectionChangedBySystem = False
                        End If
                    End If
                End Using
            Next
        Next
    End Sub

    Private Sub _udmMain_PaneHidden(sender As Object, e As PaneHiddenEventArgs) Handles _udmMain.PaneHidden
        _utmMain.BeginUpdate()
        Using _utmMain.EventManager.ProtectProperty(NameOf(ToolbarsEventManager.AllEventsEnabled), False)
            If e.Pane.Key = PaneKeys.TopologyHub.ToString Then
                DirectCast(_utmMain.Tools(SettingsTabToolKey.DisplayTopologyHub.ToString), StateButtonTool).Checked = False
            ElseIf e.Pane.Key = HomeTabToolKey.Display3DView.ToString Then
                DirectCast(_utmMain.Tools(HomeTabToolKey.Display3DView.ToString), StateButtonTool).Checked = False
                _mainForm.SaveConsolidated3DSettings()
            ElseIf e.Pane.Key = HomeTabToolKey.DisplaySchematicsView.ToString Then
                _mainForm.SaveAdvConnectivitySettings()
            End If
        End Using
        _utmMain.EndUpdate()
    End Sub

    Private Async Sub _utmMain_ToolClick(sender As Object, e As ToolClickEventArgs) Handles _utmMain.ToolClick
        If e.Tool.EnabledResolved Then ' additional check for double clicking -> skip this click/execution of BL if tool-click - [ to avoid stacking calls -> would block the UI when a lot of clicks where done ] - was executed again while other tool-click is still in progress
            Select Case e.Tool.Key
                Case ApplicationMenuToolKey.CloseDocument.ToString
                    SyncLock Me
                        e.Tool.SharedProps.Enabled = False
                        CloseDocument()
                        e.Tool.SharedProps.Enabled = True
                    End SyncLock
                Case ApplicationMenuToolKey.CloseDocuments.ToString
                    SyncLock Me
                        e.Tool.SharedProps.Enabled = False
                        CloseDocuments()
                        e.Tool.SharedProps.Enabled = True
                    End SyncLock
                Case ApplicationMenuToolKey.ExitE3HarnessAnalyzer.ToString
                    _mainForm.Close()
                Case ApplicationMenuToolKey.OpenDocument.ToString
                    Await OpenDocument()
                Case ApplicationMenuToolKey.Print.ToString
                    PrintDocument()
                Case ApplicationMenuToolKey.SaveDocument.ToString
                    SaveDocument()
                Case ApplicationMenuToolKey.SaveDocumentAs.ToString
                    SaveDocumentAs()
                Case ApplicationMenuToolKey.SaveDocuments.ToString
                    SaveDocuments()
                Case HomeTabToolKey.HideNoModuleEntities.ToString
                    SyncLock Me
                        e.Tool.SharedProps.Enabled = False
                        _mainForm.UpdateAllEntitiesVisibleWithoutModules()
                        e.Tool.SharedProps.Enabled = True
                    End SyncLock
                Case ToolKeys.ExportList.ToString
                    If (e.ListToolItem.Key = ApplicationMenuToolKey.ExportExcel.ToString) Then
                        ExportExcel()
                    ElseIf (e.ListToolItem.Key = ApplicationMenuToolKey.ExportGraphic.ToString) Then
                        ExportGraphic()
                    ElseIf (e.ListToolItem.Key = ApplicationMenuToolKey.ExportPDF.ToString) Then
                        ExportPDF()
                    ElseIf (e.ListToolItem.Key = ApplicationMenuToolKey.ExportKbl.ToString) Then
                        ExportKbl()
                    End If
                Case HomeTabToolKey.CompareData.ToString
                    SyncLock Me
                        e.Tool.SharedProps.Enabled = False
                        CompareDocuments(True)
                        e.Tool.SharedProps.Enabled = True
                    End SyncLock
                Case HomeTabToolKey.CompareGraphic.ToString
                    SyncLock Me
                        e.Tool.SharedProps.Enabled = False
                        CompareDocuments(False)
                        e.Tool.SharedProps.Enabled = True
                    End SyncLock
                Case HomeTabToolKey.Display3DView.ToString
                    CType(_mainForm.D3D, D3D.MainFormController).DisplayConsolidatedViewAction(CType(e.Tool, StateButtonTool))
                Case HomeTabToolKey.LoadCarTransformation.ToString
                    CType(_mainForm.D3D, D3D.MainFormController).LoadCarTransformationByUserAction()
                Case HomeTabToolKey.SaveCarTransformation.ToString
                    CType(_mainForm.D3D, D3D.MainFormController).SaveCarTransformationAction(apply:=True)
                Case HomeTabToolKey.Export3DModel.ToString
                    Dim tsk As Task = CType(_mainForm.D3D, D3D.MainFormController).Export3DModelAction()
                Case HomeTabToolKey.Search.ToString
                    SyncLock Me
                        SearchDocuments()
                    End SyncLock
                Case HomeTabToolKey.Inliners.ToString
                    ShowInliners()
                Case HomeTabToolKey.BundleCalculator.ToString
                    ShowStandaloneBundleCalculator()
                Case HomeTabToolKey.TopologyEditor.ToString
                    Using topologyEditorForm As New TopologyEditorForm
                        topologyEditorForm.ShowDialog(_mainForm)
                    End Using
                Case ToolKeys.OpenDocumentList.ToString
                    Await OpenDocument(e.ListToolItem)
                Case SettingsTabToolKey.About.ToString
                    Using splashScreen As New SplashScreen(True)
                        _mainForm.AppInitManager.SetBorrowedExpireDate(splashScreen)
                        splashScreen.ShowDialog(_mainForm)
                    End Using
                Case SettingsTabToolKey.DataTableSettings.ToString
                    ShowDataTableSettings()
                Case SettingsTabToolKey.DisplayTopologyHub.ToString
                    If (DirectCast(e.Tool, StateButtonTool).Checked) Then
                        _mainForm.udmMain.PaneFromKey(PaneKeys.TopologyHub.ToString).Show()
                    Else
                        _mainForm.udmMain.PaneFromKey(PaneKeys.TopologyHub.ToString).Close()
                    End If
                Case SettingsTabToolKey.Help.ToString
                    Try
                        System.Diagnostics.ProcessEx.Start(GetHelpFileName)
                    Catch ex As Exception
                        ex.ShowMessageBox(ErrorStrings.MainStatMachine_HelpNotFound_Msg)
                    End Try
                Case SettingsTabToolKey.GeneralSettings.ToString
                    ShowGeneralSettings()
                Case SettingsTabToolKey.LayoutSettings.ToString
                    SyncLock Me
                        e.Tool.SharedProps.Enabled = False
                        _mainForm?.ActiveDocument?.ResetDockableControls()
                        e.Tool.SharedProps.Enabled = True
                    End SyncLock
                Case SettingsTabToolKey.WireTypeDiameters.ToString
                    Using wireTypeDiametersWeightsForm As New WireTypeDiametersWeightsForm(_mainForm.DiameterSettings)
                        wireTypeDiametersWeightsForm.ShowDialog(_mainForm)
                    End Using
                Case SettingsTabToolKey.WireTypeWeights.ToString
                    Using wireTypeDiametersWeightsForm As New WireTypeDiametersWeightsForm(_mainForm.WeightSettings)
                        wireTypeDiametersWeightsForm.ShowDialog(_mainForm)
                    End Using
                Case ToolKeys.RecentList.ToString
                    Await OpenDocument(e.ListToolItem) ' openDocument is already sync-locked by itself
                Case Else
                    If e.Tool.Key.StartsWith("Tutorial") Then
                        PlayVideo(e.Tool.Key)
                    End If
            End Select
        End If
    End Sub

    Private Sub ShowStandaloneBundleCalculator()
        Dim calculatedSegmentDiameters As New Dictionary(Of String, Dictionary(Of HarnessModuleConfiguration, PackagingCircle))
        Dim harnessModuleConfigurations As New HarnessModuleConfigurationCollection
        Dim kblSegment As New E3.Lib.Schema.Kbl.Segment
        Dim kblMapper As [Lib].Schema.Kbl.KblMapper = CreateInitialKblMapperForBdlCalc(kblSegment)
        Using bdlcalc As New BundleCrossSectionForm(calculatedSegmentDiameters, _mainForm.DiameterSettings, "", harnessModuleConfigurations, kblMapper, kblSegment, _mainForm.GeneralSettings.WireColorCodes)
            bdlcalc.ShowDialog(_mainForm)
        End Using
    End Sub

    Private Function CreateInitialKblMapperForBdlCalc(seg As E3.Lib.Schema.Kbl.Segment) As KblMapper
        Dim kbl As New KBL_container
        seg.SystemId = "id_seg"
        seg.Id = "Temporary"
        seg.Fixing_assignment = Array.Empty(Of Fixing_assignment)
        seg.Protection_area = Array.Empty(Of Protection_area)
        seg.Start_node = "id_nodeA"
        seg.End_node = "id_nodeB"
        seg.Cross_section_area_information = Array.Empty(Of Cross_section_area)

        kbl.General_wire = Array.Empty(Of General_wire)
        Dim gw As New General_wire
        gw.SystemId = "id_generalWire"
        gw.Core = Array.Empty(Of Core)
        gw.Part_number = ""
        gw.Cover_colour = Array.Empty(Of Wire_colour)
        kbl.General_wire.Add(gw)

        kbl.Segment = Array.Empty(Of E3.Lib.Schema.Kbl.Segment)
        kbl.Segment.Add(seg)
        kbl.Harness = New E3.Lib.Schema.Kbl.Harness
        kbl.Harness.SystemId = "id_harness"
        kbl.Harness.Part_number = ""

        kbl.Harness.General_wire_occurrence = Array.Empty(Of General_wire_occurrence)
        Dim w As New Wire_occurrence
        w.SystemId = BundleCrossSectionForm.DummyWireId.ToString
        'HINT DummyId this specifies the initial wire in a dummy segment to be removed later in standalone mode
        'it is necessary, as the databinding and the way the adding of user wires work, does not allow for empty childbands
        'there are omitted otherwise and this also implies that we need always one User wire in our calculator 
        w.Wire_number = ""
        w.Length_information = Array.Empty(Of Wire_length)
        w.Part = gw.SystemId

        kbl.Harness.General_wire_occurrence.Add(w)

        kbl.Harness.Module = Array.Empty(Of [Lib].Schema.Kbl.[Module])
        Dim m As New [Lib].Schema.Kbl.[Module]
        m.SystemId = "id_module"
        m.Part_number = ""
        m.Change = Array.Empty(Of Change)
        kbl.Harness.Module.Add(m)

        Dim mcnf As New Module_configuration
        mcnf.SystemId = "id_modConfig"
        m.Module_configuration = mcnf
        mcnf.Controlled_components = w.SystemId

        kbl.Harness.Connection = Array.Empty(Of Connection)
        Dim cnct As New Connection
        cnct.SystemId = "id_connection"
        cnct.Id = ""
        cnct.Wire = w.SystemId
        cnct.Extremities = Array.Empty(Of Extremity)
        kbl.Harness.Connection.Add(cnct)

        kbl.Harness.Wiring_group = Array.Empty(Of Wiring_group)
        kbl.Routing = Array.Empty(Of Routing)

        Dim rt As New Routing
        rt.SystemId = "id_routing"
        rt.Routed_wire = cnct.SystemId
        rt.Segments = seg.SystemId
        kbl.Routing.Add(rt)

        Dim kblMapper As New KblMapper(kbl)
        kblMapper.ReBuild()
        Return kblMapper
    End Function

    Private Sub _utmmMain_TabClosed(sender As Object, e As MdiTabEventArgs) Handles _utmmMain.TabClosed
        _mainForm?.TopologyHub?.InitializeInlinerConnections(GetInlinerConnections, _mainForm.GeneralSettings.InlinerIdentifiers)
        _mainForm?.TopologyHub?.ToggleActiveHarnesses(_mainForm.utmmMain)

        If _utmmMain.ActiveTab Is Nothing Then
            _mainForm.Tools.Application.CloseDocument.SharedProps.Enabled = False
            _mainForm.Tools.Application.CloseDocument.SharedProps.Enabled = False
            _mainForm.Tools.Application.CloseDocuments.SharedProps.Enabled = False
            _mainForm.Tools.Application.SaveDocument.SharedProps.Enabled = False
            _mainForm.Tools.Application.SaveDocumentAs.SharedProps.Enabled = False
            _mainForm.Tools.Application.SaveDocuments.SharedProps.Enabled = False
            _mainForm.Tools.Application.Export.SharedProps.Enabled = False

            _mainForm.Tools.Application.ExportKbl.Enabled = False

            _mainForm.Tools.Home.CompareData.SharedProps.Enabled = False
            _mainForm.Tools.Home.CompareGraphic.SharedProps.Enabled = False
            _mainForm.Tools.Home.Inliners.SharedProps.Enabled = False
            _mainForm.Tools.Home.Search.SharedProps.Enabled = False
            _mainForm.Tools.Home.HideNoModuleEntities.SharedProps.Enabled = False
        Else
            _mainForm.Tools.Application.CloseDocuments.SharedProps.Enabled = _utmmMain.TabGroups.Count <> 0 AndAlso _utmmMain.TabGroups(0).Tabs.Count > 1
        End If

        If _inlinersForm IsNot Nothing Then
            _inlinersForm.Dispose()
        End If

        If Not _mainForm.IsDisposed AndAlso _mainForm.D3D IsNot Nothing Then ' HINT: event can come after main form is already disposed (access hard to property to trigger exception, which shoudn't be possible under normal conditions)
            CType(_mainForm.D3D, D3D.MainFormController).OnAfterDocumentClosed(_closingDocument)
            If _utmmMain.TabGroups.Count = 0 Then
                If _mainForm.SchematicsView IsNot Nothing AndAlso Not _mainForm.SchematicsView.IsDisposed Then
                    If _mainForm.SchematicsView.Visible Then
                        _mainForm?.Panes?.SchematicsPane?.Close()
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub _utmmMain_TabClosing(sender As Object, e As CancelableMdiTabEventArgs) Handles _utmmMain.TabClosing
        If TypeOf e.Tab.Form Is DocumentForm Then
            _closingDocument = CType(e.Tab.Form, DocumentForm)
        End If
    End Sub

    Private Sub _utmmMain_TabSelected(sender As Object, e As MdiTabEventArgs) Handles _utmmMain.TabSelected
        'HINT: Would be better when BL only at one place: TabActivated OR TabSelected: currently it's split half/half (activated and selected) -> but one of the event's will be raised alway's. I don't know currently who, but a merge should theoretically be possible. This merge will reduce the maintaince diffuculty.
        '     --> so better check this two event's and merge it some day's
        If Me.CurrentLoadingDocumentsCount = 0 Then
            TryCast(e.Tab.Form, DocumentForm)?.DisplayDrawing()
        End If

        If (_compareForm?.Visible).GetValueOrDefault AndAlso Not _selectionChangedBySystem Then
            _compareForm.Close()
            _compareForm.Dispose()
        End If

        If (_graphicalCompareForm?.Visible).GetValueOrDefault AndAlso Not _selectionChangedBySystem Then
            _graphicalCompareForm.Close()

            If Not _graphicalCompareForm.Visible Then
                _graphicalCompareForm.Dispose()
            End If
        End If
    End Sub

    Private Sub _utmmMain_TabSelecting(sender As Object, e As CancelableMdiTabEventArgs) Handles _utmmMain.TabSelecting
        If (_compareForm?.Visible).GetValueOrDefault AndAlso Not _selectionChangedBySystem Then
            If MessageBoxEx.ShowQuestion(DialogStrings.MainStatMachine_CloseCompDlgWhenChangingActiveHCV_Msg) = DialogResult.No Then
                e.Cancel = True
                Exit Sub
            End If
        End If

        If (_graphicalCompareForm?.Visible).GetValueOrDefault AndAlso Not _selectionChangedBySystem Then
            If MessageBoxEx.ShowQuestion(DialogStrings.MainStatMachine_CloseGraphCompDlgWhenChangingActiveHCV_Msg) = DialogResult.No Then
                e.Cancel = True
            End If
        End If
    End Sub

    Friend Property EntireCarSettings() As EntireCarSettings
        Get
            Return _entireCarSettings
        End Get
        Set(value As EntireCarSettings)
            _entireCarSettings = value
        End Set
    End Property

    Friend Property XHcvFile() As E3.Lib.IO.Files.Hcv.XhcvFile
        Get
            Return _xHcvFile
        End Get
        Set(value As E3.Lib.IO.Files.Hcv.XhcvFile)
            _xHcvFile = value
        End Set
    End Property

    Friend Property OverallConnectivity() As OverallConnectivity
        Get
            Return _overallConnectivity
        End Get
        Set(value As OverallConnectivity)
            _overallConnectivity = value
        End Set
    End Property

    Public ReadOnly Property IsXhcv As Boolean
        Get
            Return _xHcvFile IsNot Nothing
        End Get
    End Property

    Public ReadOnly Property IsLoadingXhcv As Boolean
        Get
            Return _isLoadingXhcv
        End Get
    End Property

    Public ReadOnly Property CurrentLoadingDocumentsCount As Integer = 0

    Private Sub PlayVideo(key As String)
        Try
            Dim idx As Integer = CInt(key.Replace("Tutorial", String.Empty))
            Dim l As List(Of Tutorial) = _mainForm.GeneralSettings.Tutorials.Where(Function(t) t.UILanguage = _mainForm.GeneralSettings.UILanguage).ToList
            System.Diagnostics.ProcessEx.Start(l(idx).Url)
        Catch ex As Exception
            ex.ShowMessageBox("Could not process video link!")
        End Try
    End Sub

    Private Function GetHelpFileName() As String
        Dim hlpFile As String = IO.Path.Combine(Application.StartupPath, String.Format("{0}.chm", HarnessAnalyzer.[Shared].PRODUCT_FOLDER + "_u"))
        Dim hlpFileDE As String = IO.Path.Combine(Application.StartupPath, String.Format("{0}.chm", HarnessAnalyzer.[Shared].PRODUCT_FOLDER + "_d"))

        If _mainForm.GeneralSettings.UILanguage.ToLower = "de-de" Then
            Dim finf As New IO.FileInfo(hlpFileDE)
            If finf.Exists Then
                Return hlpFileDE
            Else
                Return hlpFile
            End If
        Else
            Return hlpFile
        End If

        Return hlpFile
    End Function

End Class