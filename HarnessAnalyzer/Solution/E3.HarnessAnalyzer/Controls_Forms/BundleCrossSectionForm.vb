Imports System.ComponentModel
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Threading
Imports Infragistics.Documents.Excel
Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinGrid
Imports Infragistics.Win.UltraWinToolbars
Imports Infragistics.Win.UltraWinToolTip
Imports VectorDraw.Geometry
Imports VectorDraw.Professional.Constants
Imports VectorDraw.Professional.vdCollections
Imports VectorDraw.Professional.vdFigures
Imports VectorDraw.Professional.vdObjects
Imports VectorDraw.Professional.vdPrimaries
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.HarnessAnalyzer.Shared.Common
Imports Zuken.E3.Lib.Packager.Circle

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class BundleCrossSectionForm

    Friend Event BundleGridSelectionChanged(sender As BundleCrossSectionForm, e As InformationHubEventArgs)

    Private _bundleAddOnInstallationTolerance As Double = 0
    Private _bundleAddOnProvisioningTolerance As Double = 0
    Private _prevBundleAddOnInstallationTolerance As Double = 0
    Private _prevBundleAddOnProvisioningTolerance As Double = 0

    Private _calculatedSegmentDiameters As Dictionary(Of String, Dictionary(Of HarnessModuleConfiguration, PackagingCircle))
    Private _clickedRow As UltraGridRow
    Private _contextMenu As PopupMenuTool
    Private _copiedUserWires As List(Of UltraGridRow)
    Private _diameterSettings As DiameterSettings
    Private _docName As String
    Private _dropDownRow As UltraGridRow
    Private _droppedRows As List(Of UltraGridRow)
    Private _exportCancelled As Boolean
    Private _exportFileName As String
    Private _exportRunning As Boolean
    Private _harnessModuleConfigurations As HarnessModuleConfigurationCollection
    Private _highlightedObjects As List(Of vdCircle)
    Private _hoveredRow As UltraGridRow
    Private _initializing As Boolean
    Private _kblMapper As KblMapper
    Private _messageBoxDisplayed As Boolean
    Private _moduleConfigurationsWithCablesWires As MappingDictionary(Of HarnessModuleConfiguration, IKblWireCoreOccurrence)
    Private _segment As Segment
    Private _selectedModuleConfig As HarnessModuleConfiguration
    Private _userWireNumber As New Random(Now.Millisecond)
    Private _wireColorCodes As WireColorCodeList
    Private _workingModuleConfig As HarnessModuleConfiguration
    Private _deleteInitialRow As Boolean = False
    Private _isStandAlone As Boolean = False
    Private _lockCalculate As New System.Threading.SemaphoreSingle
    Friend Shared DummyWireId As Guid = Guid.NewGuid
    Private Const CABLE_WIRE_BANDHEADER As String = "CablesAndWires"
    Private Const MODUL_CONFIG_BANDHEADER As String = "ModuleConfigs"

    Private WithEvents _circlePackager As CirclePackager

    Private Enum ContextMenuToolKey
        AddCableWire
        AddUserWire
        CloneAsUserWires
        CollapseAll
        CopyUserWires
        CopySelectedUserWires
        ExpandAll
        PasteUserWires
        RemoveCableWireTemporarily
        RemoveUserWire
        ResetPartnumberModification
        BulkChangePartnumber
    End Enum

    Friend Enum ColumnKeys
        ModulConfiguration
        OutsideDiaCalc
        OutsideDiaValCalc
        OutsideDiaPack
        OutsideDiaValPack
        OutsideDiaKBL
        OutsideDiaValKBL
        CSACalc
        CSAValCalc
        CSAPack
        CSAValPack
        CSAKBL
        CSAValKBL

        Id
        [Class]
        CabWirNum
        WiringGroup
        NetName
        HarnessModules
        OutsideDia
        OutsideDiaVal
        DiaSource
        PartNumber

        InitialPartnumber
        InitialGeneralWire

        TemporaryAdded
        TemporaryRemoved
        TemporaryModified

        Source
        GeneralWire
    End Enum

    Public Sub New(ByRef calculatedSegmentDiameters As Dictionary(Of String, Dictionary(Of HarnessModuleConfiguration, PackagingCircle)), diameterSettings As DiameterSettings, docName As String, ByRef harnessModuleConfigurations As HarnessModuleConfigurationCollection, kblMapper As KblMapper, segment As Segment, wireColorCodes As WireColorCodeList)
        InitializeComponent()

        _calculatedSegmentDiameters = calculatedSegmentDiameters
        _contextMenu = New PopupMenuTool("GridContextMenu")
        _copiedUserWires = New List(Of UltraGridRow)
        _diameterSettings = diameterSettings
        _docName = docName
        _harnessModuleConfigurations = harnessModuleConfigurations
        _highlightedObjects = New List(Of vdCircle)
        _initializing = True
        _kblMapper = kblMapper
        _segment = segment
        _wireColorCodes = wireColorCodes

        Initialize()
    End Sub

    Friend Shared Function GetCalculatedOutsideDiameter(segmentCircle As PackagingCircle, diameterSettings As DiameterSettings, addOnTolerance As Double, isToleranceOnArea As Boolean) As Double
        Dim accDiameters As Double = segmentCircle.InnerCircles.Sum(Function(c) c.Diameter)

        If (accDiameters <> 0) Then
            Dim outsideDiameter As Double = CDbl(Math.Round((diameterSettings.GenericDiameterFormulaParameters.BDL_Coeff1 / (segmentCircle.InnerCircles.Count ^ diameterSettings.GenericDiameterFormulaParameters.BDL_Exp)) * accDiameters * diameterSettings.GenericDiameterFormulaParameters.BDL_Corr, 1))
            If (addOnTolerance < 0) Then
                addOnTolerance = 0
            End If

            If (addOnTolerance > 10) Then
                addOnTolerance = 10
            End If

            If (isToleranceOnArea) Then
                outsideDiameter = 2 * Math.Sqrt(GetCalculatedCrossSectionArea(outsideDiameter) * (1 + addOnTolerance) / Math.PI)
            Else
                outsideDiameter *= 1 + addOnTolerance
            End If

            Return outsideDiameter
        Else
            Return 0
        End If
    End Function

    Friend Shared Function GetGenericCoreOrWireDiameter(wire As Zuken.E3.Lib.Model.Wire, diameterSettings As DiameterSettings) As Single
        Return GetGenericCoreOrWireDiameter(wire.CSANom, diameterSettings)
    End Function

    Friend Shared Function GetGenericCoreOrWireDiameter(coreOrGeneralWire As Object, diameterSettings As DiameterSettings) As Single
        Dim csa As Double = 0

        If (TypeOf coreOrGeneralWire Is Core) Then
            Dim core As Core = DirectCast(coreOrGeneralWire, Core)
            If (core.Cross_section_area IsNot Nothing) Then
                csa = core.Cross_section_area.Value_component
            End If
        Else
            Dim generalWire As General_wire = DirectCast(coreOrGeneralWire, General_wire)
            If (generalWire.Cross_section_area IsNot Nothing) Then
                csa = generalWire.Cross_section_area.Value_component
            End If
        End If

        Return GetGenericCoreOrWireDiameter(csa, diameterSettings)
    End Function

    Friend Shared Function GetGenericCoreOrWireDiameter(csa As Double, diameterSettings As DiameterSettings) As Single
        If (csa <> 0) Then
            Return CSng(Math.Round(diameterSettings.GenericDiameterFormulaParameters.WD_Coeff1 + diameterSettings.GenericDiameterFormulaParameters.WD_Coeff2 * csa + diameterSettings.GenericDiameterFormulaParameters.WD_Coeff3 * csa ^ diameterSettings.GenericDiameterFormulaParameters.WD_Exp, 1))
        Else
            Return 0
        End If
    End Function

    Friend Shared Function GetGenericMulticoreDiameter(generalWire As General_wire, diameterSettings As DiameterSettings) As Single
        Dim accDiameters As Double = 0
        Dim diameterCount As Integer = 0

        For Each core As Core In generalWire.Core
            If (core.Outside_diameter IsNot Nothing) Then
                accDiameters += core.Outside_diameter.Value_component
                diameterCount += 1
            End If
        Next

        Return GetGenericMulticoreDiameter(accDiameters, diameterCount, diameterSettings)
    End Function

    Friend Shared Function GetGenericMulticoreDiameter(cable As E3.Lib.Model.Cable, diameterSettings As DiameterSettings) As Single
        Dim accDiameters As Double = 0
        Dim diameterCount As Integer = 0

        For Each wire As E3.Lib.Model.Wire In cable.GetWires.Entries
            If Not Single.IsNaN(wire.OuterDiameter) Then
                accDiameters += wire.OuterDiameter
                diameterCount += 1
            End If
        Next

        Return GetGenericMulticoreDiameter(accDiameters, diameterCount, diameterSettings)
    End Function

    Friend Shared Function GetGenericMulticoreDiameter(accumulatedCoreDiameters As Double, coreDiamCount As Integer, diameterSettings As DiameterSettings) As Single
        If (accumulatedCoreDiameters <> 0) Then
            Return CSng(Math.Round((diameterSettings.GenericDiameterFormulaParameters.MCD_Coeff1 / (coreDiamCount ^ diameterSettings.GenericDiameterFormulaParameters.MCD_Exp)) * accumulatedCoreDiameters * diameterSettings.GenericDiameterFormulaParameters.MCD_Corr, 1))
        Else
            Return 0
        End If
    End Function

    Private Sub Initialize()
        Me.BackColor = Color.White
        Me.Icon = My.Resources.BundleCrossSection
        Me.Tag = _segment

        Me.Text = String.Format(BundleCrossSectionFormStrings.Caption, _segment.Id, _docName)

        Me.uneCstBundleInstallationAddOnTol.DataFilter = New PercentageDataFilter
        Me.uneCstBundleProvisioningAddOnTol.DataFilter = New PercentageDataFilter

        Me.uneCstBundleInstallationAddOnTol.Value = _diameterSettings.RawBundleInstallationAddOnTolerance
        Me.uneCstBundleProvisioningAddOnTol.Value = _diameterSettings.RawBundleProvisioningAddOnTolerance

        Me.upnMain.Appearance.BackColor = Color.White
        Me.upnMain.Appearance.BorderColor = Color.Black
        Me.upnMain.BorderStyle = Infragistics.Win.UIElementBorderStyle.Inset
        Me.upnBottom.Appearance.BackColor = Color.White

        _bundleAddOnInstallationTolerance = _diameterSettings.RawBundleInstallationAddOnTolerance
        _bundleAddOnProvisioningTolerance = _diameterSettings.RawBundleProvisioningAddOnTolerance
        _prevBundleAddOnInstallationTolerance = _bundleAddOnInstallationTolerance
        _prevBundleAddOnProvisioningTolerance = _bundleAddOnProvisioningTolerance

        If _diameterSettings.IsAddOnToleranceOnArea Then
            ulblToleranceMode1.Text = BundleCrossSectionFormStrings.ToleranceOnArea
            ulblToleranceMode2.Text = BundleCrossSectionFormStrings.ToleranceOnArea
        Else
            ulblToleranceMode1.Text = BundleCrossSectionFormStrings.ToleranceOnDiameter
            ulblToleranceMode2.Text = BundleCrossSectionFormStrings.ToleranceOnDiameter
        End If

        InitializeContextMenu()
        InitializeGrid()
        InitializePartNumbers()
        InitializeVectorDrawBaseControl()
    End Sub

    Private Sub InitializeContextMenu()
        With _contextMenu
            .DropDownArrowStyle = DropDownArrowStyle.None

            Dim addCableWire As New ButtonTool(ContextMenuToolKey.AddCableWire.ToString)
            addCableWire.SharedProps.Caption = BundleCrossSectionFormStrings.AddCabWir_CtxtMnu_Caption
            addCableWire.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.AddCableWire.ToBitmap

            Dim addUserWire As New ButtonTool(ContextMenuToolKey.AddUserWire.ToString)
            addUserWire.SharedProps.Caption = BundleCrossSectionFormStrings.AddUserWir_CtxtMnu_Caption
            addUserWire.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.AddUserWire.ToBitmap

            Dim cloneAsUserWires As New ButtonTool(ContextMenuToolKey.CloneAsUserWires.ToString)
            cloneAsUserWires.SharedProps.Caption = BundleCrossSectionFormStrings.CloneAsUserWir_CtxtMnu_Caption
            cloneAsUserWires.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.Clone.ToBitmap

            Dim collapseAllButton As New ButtonTool(ContextMenuToolKey.CollapseAll.ToString)
            collapseAllButton.SharedProps.Caption = InformationHubStrings.CollapseAll_CtxtMnu_Caption
            collapseAllButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.CollapseAll.ToBitmap

            Dim copyUserWires As New ButtonTool(ContextMenuToolKey.CopyUserWires.ToString)
            copyUserWires.SharedProps.Caption = BundleCrossSectionFormStrings.CopyUserWir_CtxtMnu_Caption
            copyUserWires.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.CopyModConfig.ToBitmap

            Dim copySelectedUserWires As New ButtonTool(ContextMenuToolKey.CopySelectedUserWires.ToString)
            copySelectedUserWires.SharedProps.Caption = BundleCrossSectionFormStrings.CopySelUserWir_CtxtMnu_Caption
            copySelectedUserWires.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.CopyModConfig.ToBitmap

            Dim expandAllButton As New ButtonTool(ContextMenuToolKey.ExpandAll.ToString)
            expandAllButton.SharedProps.Caption = InformationHubStrings.ExpandAll_CtxtMnu_Caption
            expandAllButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.ExpandAll.ToBitmap

            Dim pasteUserWires As New ButtonTool(ContextMenuToolKey.PasteUserWires.ToString)
            pasteUserWires.SharedProps.Caption = BundleCrossSectionFormStrings.PasteUserWir_CtxtMnu_Caption
            pasteUserWires.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.PasteFromClipboard.ToBitmap

            Dim removeCableWireTemporary As New ButtonTool(ContextMenuToolKey.RemoveCableWireTemporarily.ToString)
            removeCableWireTemporary.SharedProps.Caption = BundleCrossSectionFormStrings.RemoveCabWir_CtxtMnu_Caption
            removeCableWireTemporary.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.RemoveCableWireTemporarily.ToBitmap

            Dim removeUserWire As New ButtonTool(ContextMenuToolKey.RemoveUserWire.ToString)
            removeUserWire.SharedProps.Caption = BundleCrossSectionFormStrings.RemoveUserWir_CtxtMnu_Caption
            removeUserWire.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.RemoveUserWire.ToBitmap


            Dim resetPartnumber As New ButtonTool(ContextMenuToolKey.ResetPartnumberModification.ToString)
            resetPartnumber.SharedProps.Caption = BundleCrossSectionFormStrings.ResetPartnumberMod_CtxtMnu_Caption
            resetPartnumber.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.ResetPartnumberModification.ToBitmap

            Dim bulkChangePartnumber As New ButtonTool(ContextMenuToolKey.BulkChangePartnumber.ToString)
            bulkChangePartnumber.SharedProps.Caption = BundleCrossSectionFormStrings.BulkChangePartnumber_CtxtMnu_Caption
            bulkChangePartnumber.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.BulkChangePartnumber.ToBitmap

            Me.utmBundleCSAs.Tools.AddRange(New ToolBase() {_contextMenu, addCableWire, addUserWire, cloneAsUserWires, collapseAllButton, copyUserWires, copySelectedUserWires, expandAllButton, pasteUserWires, removeCableWireTemporary, removeUserWire, resetPartnumber, bulkChangePartnumber})

            .Tools.AddTool(addCableWire.Key)
            .Tools.AddTool(addUserWire.Key)
            .Tools.AddTool(cloneAsUserWires.Key)
            .Tools.AddTool(collapseAllButton.Key)
            .Tools.AddTool(copyUserWires.Key)
            .Tools.AddTool(copySelectedUserWires.Key)
            .Tools.AddTool(expandAllButton.Key)
            .Tools.AddTool(pasteUserWires.Key)
            .Tools.AddTool(removeCableWireTemporary.Key)
            .Tools.AddTool(removeUserWire.Key)
            .Tools.AddTool(resetPartnumber.Key)
            .Tools.AddTool(bulkChangePartnumber.Key)

        End With
    End Sub

    Private Sub InitializeGrid()
        Me.ugBundleCSAs.SyncWithCurrencyManager = False

        With Me.udsBundleCSAs
            .Band.Key = MODUL_CONFIG_BANDHEADER

            With .Band
                ColumnKeys.ModulConfiguration.ToString()

                .Columns.Add(ColumnKeys.ModulConfiguration.ToString)
                .Columns.Add(ColumnKeys.OutsideDiaCalc.ToString)
                .Columns.Add(ColumnKeys.OutsideDiaValCalc.ToString, GetType(Double))
                .Columns.Add(ColumnKeys.OutsideDiaPack.ToString)
                .Columns.Add(ColumnKeys.OutsideDiaValPack.ToString, GetType(Double))
                .Columns.Add(ColumnKeys.OutsideDiaKBL.ToString)
                .Columns.Add(ColumnKeys.OutsideDiaValKBL.ToString, GetType(Double))
                .Columns.Add(ColumnKeys.CSACalc.ToString)
                .Columns.Add(ColumnKeys.CSAValCalc.ToString, GetType(Double))
                .Columns.Add(ColumnKeys.CSAPack.ToString)
                .Columns.Add(ColumnKeys.CSAValPack.ToString, GetType(Double))
                .Columns.Add(ColumnKeys.CSAKBL.ToString)
                .Columns.Add(ColumnKeys.CSAValKBL.ToString, GetType(Double))
            End With

            .Band.ChildBands.Add(CABLE_WIRE_BANDHEADER)

            With .Band.ChildBands(0)
                .Columns.Add(ColumnKeys.Id.ToString, GetType(String)).DefaultValue = String.Empty
                .Columns.Add(ColumnKeys.Class.ToString, GetType(String))
                .Columns.Add(ColumnKeys.CabWirNum.ToString, GetType(String))
                .Columns.Add(ColumnKeys.PartNumber.ToString)
                .Columns.Add(ColumnKeys.WiringGroup.ToString)
                .Columns.Add(ColumnKeys.NetName.ToString, GetType(String))
                .Columns.Add(ColumnKeys.HarnessModules.ToString)
                .Columns.Add(ColumnKeys.OutsideDia.ToString)
                .Columns.Add(ColumnKeys.OutsideDiaVal.ToString, GetType(Double))
                .Columns.Add(ColumnKeys.DiaSource.ToString, GetType(String))

                .Columns.Add(ColumnKeys.TemporaryAdded.ToString, GetType(Boolean)).DefaultValue = False
                .Columns.Add(ColumnKeys.TemporaryRemoved.ToString, GetType(Boolean)).DefaultValue = False
                .Columns.Add(ColumnKeys.TemporaryModified.ToString, GetType(Boolean)).DefaultValue = False

                .Columns.Add(ColumnKeys.InitialPartnumber.ToString)
                .Columns.Add(ColumnKeys.InitialGeneralWire.ToString, GetType(Object)).DefaultValue = Nothing
                .Columns.Add(ColumnKeys.GeneralWire.ToString, GetType(Object)).DefaultValue = Nothing

            End With
        End With

        InitializeGridData()
    End Sub

    Private Sub InitializeGridData()
        Me.udsBundleCSAs.Rows.Clear()

        Dim routing As List(Of IKblWireCoreOccurrence) = Nothing

        If (_kblMapper.KBLSegmentWireMapper.ContainsKey(_segment.SystemId)) Then
            routing = _kblMapper.GetWireOrCoresOfSegment(_segment.SystemId)
        Else
            MessageBoxEx.ShowWarning(BundleCrossSectionFormStrings.WarningNoWireRouting_Msg)
            Return
        End If

        _moduleConfigurationsWithCablesWires = New MappingDictionary(Of HarnessModuleConfiguration, IKblWireCoreOccurrence)

        If (_harnessModuleConfigurations.Count = 0) Then
            Dim harnessConfig As New Harness_configuration
            With harnessConfig
                .SystemId = Guid.NewGuid.ToString

                .Abbreviation = KblObjectType.Custom.ToLocalizedString
                .Description = HarnessModuleConfigurationType.Custom.ToString
                .Part_number = _kblMapper.HarnessPartNumber
                .Version = "1"
            End With

            Dim moduleConfig As New HarnessModuleConfiguration
            With moduleConfig
                .ConfigurationType = HarnessModuleConfigurationType.Custom
                .HarnessConfiguration = harnessConfig
                .IsActive = True
            End With

            For Each coreWire As IKblWireCoreOccurrence In routing
                _moduleConfigurationsWithCablesWires.TryAddValue(moduleConfig, coreWire)
            Next
        Else
            For Each moduleConfig As HarnessModuleConfiguration In _harnessModuleConfigurations
                _moduleConfigurationsWithCablesWires.TryGetAddNew(moduleConfig) ' HINT: always add a module configuration to the dic because in the trunk state this was intended -> the bwCSACalculation_DoWork - routine is always needing all module configurations regardless if there are wire/cores or not
                For Each moduleId As String In moduleConfig.HarnessConfiguration.Modules.SplitSpace
                    For Each coreWire As IKblWireCoreOccurrence In routing
                        If (_kblMapper.KBLModuleObjectMapper.ContainsKey(moduleId)) AndAlso (_kblMapper.KBLModuleObjectMapper(moduleId).Contains(coreWire)) Then
                            _moduleConfigurationsWithCablesWires.TryAddValue(moduleConfig, coreWire)
                        End If
                    Next
                Next
            Next
        End If

        With Me.udsBundleCSAs

            For Each moduleConfigWithCablesWires As KeyValuePair(Of HarnessModuleConfiguration, IEnumerable(Of IKblWireCoreOccurrence)) In _moduleConfigurationsWithCablesWires
                Dim addedCables As New Dictionary(Of String, UltraDataRow)
                Dim moduleConfigRow As UltraDataRow = .Rows.Add
                Dim segmentCircle As PackagingCircle = Nothing

                If (Regex.Replace(moduleConfigWithCablesWires.Key.HarnessConfiguration.Part_number, "\s", String.Empty) <> String.Empty) Then
                    moduleConfigRow.SetCellValue(ColumnKeys.ModulConfiguration.ToString, moduleConfigWithCablesWires.Key.HarnessConfiguration.Part_number)
                ElseIf (Regex.Replace(moduleConfigWithCablesWires.Key.HarnessConfiguration.Abbreviation, "\s", String.Empty) <> String.Empty) Then
                    moduleConfigRow.SetCellValue(ColumnKeys.ModulConfiguration.ToString, moduleConfigWithCablesWires.Key.HarnessConfiguration.Abbreviation)
                ElseIf (Regex.Replace(moduleConfigWithCablesWires.Key.HarnessConfiguration.Description, "\s", String.Empty) <> String.Empty) Then
                    moduleConfigRow.SetCellValue(ColumnKeys.ModulConfiguration.ToString, moduleConfigWithCablesWires.Key.HarnessConfiguration.Description)
                End If

                If (_calculatedSegmentDiameters.ContainsKey(_segment.SystemId)) AndAlso (_calculatedSegmentDiameters(_segment.SystemId).ContainsKey(moduleConfigWithCablesWires.Key)) AndAlso (_calculatedSegmentDiameters(_segment.SystemId)(moduleConfigWithCablesWires.Key) IsNot Nothing) Then
                    segmentCircle = _calculatedSegmentDiameters(_segment.SystemId)(moduleConfigWithCablesWires.Key)

                    moduleConfigRow.SetCellValue(ColumnKeys.OutsideDiaCalc.ToString, String.Format("{0} mm", Math.Round(GetCalculatedOutsideDiameter(segmentCircle), 1)))
                    moduleConfigRow.SetCellValue(ColumnKeys.OutsideDiaValCalc.ToString, GetCalculatedOutsideDiameter(segmentCircle))
                    moduleConfigRow.SetCellValue(ColumnKeys.OutsideDiaPack.ToString, String.Format("{0} mm", Math.Round(GetPackagedOutsideDiameter(segmentCircle), 1)))
                    moduleConfigRow.SetCellValue(ColumnKeys.OutsideDiaValPack.ToString, GetPackagedOutsideDiameter(segmentCircle))
                    moduleConfigRow.SetCellValue(ColumnKeys.CSACalc.ToString, String.Format("{0} mm²", Math.Round(GetCalculatedCrossSectionArea(CDbl(moduleConfigRow.GetCellValue(ColumnKeys.OutsideDiaValCalc.ToString))), 1)))
                    moduleConfigRow.SetCellValue(ColumnKeys.CSAValCalc.ToString, GetCalculatedCrossSectionArea(GetCalculatedCrossSectionArea(CDbl(moduleConfigRow.GetCellValue(ColumnKeys.OutsideDiaValCalc.ToString)))))

                    moduleConfigRow.SetCellValue(ColumnKeys.CSAPack.ToString, String.Format("{0} mm²", Math.Round(GetPackagedCrossSectionArea(segmentCircle), 1)))
                    moduleConfigRow.SetCellValue(ColumnKeys.CSAValPack.ToString, GetPackagedCrossSectionArea(segmentCircle))
                Else
                    moduleConfigRow.SetCellValue(ColumnKeys.OutsideDiaCalc.ToString, BundleCrossSectionFormStrings.NotAvailable_CellVal)
                    moduleConfigRow.SetCellValue(ColumnKeys.OutsideDiaValCalc.ToString, 0)
                    moduleConfigRow.SetCellValue(ColumnKeys.OutsideDiaPack.ToString, BundleCrossSectionFormStrings.NotAvailable_CellVal)
                    moduleConfigRow.SetCellValue(ColumnKeys.OutsideDiaValPack.ToString, 0)
                    moduleConfigRow.SetCellValue(ColumnKeys.CSACalc.ToString, BundleCrossSectionFormStrings.NotAvailable_CellVal)
                    moduleConfigRow.SetCellValue(ColumnKeys.CSAValCalc.ToString, 0)
                    moduleConfigRow.SetCellValue(ColumnKeys.CSAPack.ToString, BundleCrossSectionFormStrings.NotAvailable_CellVal)
                    moduleConfigRow.SetCellValue(ColumnKeys.CSAValPack.ToString, 0)
                End If

                If (_segment.Cross_section_area_information.Length = 1) AndAlso (_segment.Cross_section_area_information(0).Area IsNot Nothing) AndAlso (_segment.Cross_section_area_information(0).Area.Value_component <> 0) Then
                    moduleConfigRow.SetCellValue(ColumnKeys.OutsideDiaKBL.ToString, String.Format("{0} mm", Math.Round(2 * Math.Sqrt(_segment.Cross_section_area_information(0).Area.Value_component / Math.PI), 1)))
                    moduleConfigRow.SetCellValue(ColumnKeys.OutsideDiaValKBL.ToString, 2 * Math.Sqrt(_segment.Cross_section_area_information(0).Area.Value_component / Math.PI))
                    Dim unit As Unit = _kblMapper.GetUnit(_segment.Cross_section_area_information(0).Area.Unit_component)
                    If unit IsNot Nothing Then
                        moduleConfigRow.SetCellValue(ColumnKeys.CSAKBL.ToString, String.Format("{0} {1}", Math.Round(_segment.Cross_section_area_information(0).Area.Value_component, 1), unit.Unit_name))
                    Else
                        moduleConfigRow.SetCellValue(ColumnKeys.CSAKBL.ToString, Math.Round(_segment.Cross_section_area_information(0).Area.Value_component, 1))
                    End If

                    moduleConfigRow.SetCellValue(ColumnKeys.CSAValKBL.ToString, _segment.Cross_section_area_information(0).Area.Value_component)
                Else
                    moduleConfigRow.SetCellValue(ColumnKeys.OutsideDiaKBL.ToString, BundleCrossSectionFormStrings.NotAvailable_CellVal)
                    moduleConfigRow.SetCellValue(ColumnKeys.OutsideDiaValKBL.ToString, 0)
                    moduleConfigRow.SetCellValue(ColumnKeys.CSAKBL.ToString, BundleCrossSectionFormStrings.NotAvailable_CellVal)
                    moduleConfigRow.SetCellValue(ColumnKeys.CSAValKBL.ToString, 0)
                End If

                moduleConfigRow.Tag = moduleConfigWithCablesWires.Key

                For Each wireCore As IKblWireCoreOccurrence In moduleConfigWithCablesWires.Value
                    Dim cableWireRow As UltraDataRow = moduleConfigRow.GetChildRows(.Band.ChildBands(0)).Add
                    Dim connection As Connection = Nothing
                    Dim generalWire As General_wire = Nothing

                    If (_kblMapper.KBLWireNetMapper.ContainsKey(wireCore.SystemId)) Then
                        connection = _kblMapper.KBLWireNetMapper(wireCore.SystemId)
                    End If

                    If wireCore.ObjectType = KblObjectType.Core_occurrence Then
                        Dim cable As Special_wire_occurrence = _kblMapper.GetCableOfWireOrCore(wireCore.SystemId)

                        generalWire = _kblMapper.GetPart(Of General_wire)(cable.Part)

                        If (Not addedCables.ContainsKey(cable.SystemId)) Then
                            cableWireRow.SetCellValue(ColumnKeys.Id.ToString, cable.SystemId)
                            cableWireRow.SetCellValue(ColumnKeys.Class.ToString, KblObjectType.Special_wire_occurrence.ToLocalizedPluralString)
                            cableWireRow.SetCellValue(ColumnKeys.CabWirNum.ToString, cable.Special_wire_id)
                            cableWireRow.Tag = cable

                            If (connection IsNot Nothing) AndAlso (connection.Signal_name IsNot Nothing) Then
                                cableWireRow.SetCellValue(ColumnKeys.NetName.ToString, connection.Signal_name)
                            End If

                            If (_kblMapper.KBLObjectModuleMapper.ContainsKey(cable.SystemId)) Then
                                Dim modules As String = String.Empty

                                For Each [module] As [Module] In _kblMapper.GetModulesOfObject(cable.SystemId)
                                    Dim moduleInfo As String = If([module].Abbreviation = String.Empty, [module].Part_number, String.Format("{0} [{1}]", [module].Abbreviation, [module].Part_number))

                                    If (modules = String.Empty) Then
                                        modules = moduleInfo
                                    Else
                                        modules = String.Format("{0}, {1}", modules, moduleInfo)
                                    End If
                                Next

                                cableWireRow.SetCellValue(ColumnKeys.HarnessModules.ToString, modules)
                            End If

                            addedCables.Add(cable.SystemId, cableWireRow)
                        Else
                            moduleConfigRow.GetChildRows(.Band.ChildBands(0)).Remove(cableWireRow)

                            If Not String.IsNullOrEmpty(connection?.Signal_name) Then
                                Dim rw As UltraDataRow = addedCables(cable.SystemId)
                                rw.SetCellValue(ColumnKeys.NetName.ToString, String.Format("{0};{1}", rw.GetCellValue(ColumnKeys.NetName.ToString).ToString, connection.Signal_name))
                            End If

                        End If
                    Else
                        generalWire = _kblMapper.GetPart(Of General_wire)(wireCore.Part)

                        cableWireRow.SetCellValue(ColumnKeys.Id.ToString, wireCore.SystemId)
                        cableWireRow.SetCellValue(ColumnKeys.Class.ToString, KblObjectType.Wire_occurrence.ToLocalizedString)
                        cableWireRow.SetCellValue(ColumnKeys.CabWirNum.ToString, wireCore.Wire_number)
                        cableWireRow.Tag = wireCore

                        Dim wiringGroup As Wiring_group = CType(wireCore, Wire_occurrence).GetWiringGroup(_kblMapper.GetWiringGroups)
                        If (wiringGroup IsNot Nothing) Then
                            cableWireRow.SetCellValue(ColumnKeys.WiringGroup.ToString, wiringGroup.Id)
                        End If

                        If (connection IsNot Nothing) AndAlso (connection.Signal_name IsNot Nothing) Then
                            cableWireRow.SetCellValue(ColumnKeys.NetName.ToString, connection.Signal_name)
                        End If

                        Dim modulesOfObject As List(Of [Module]) = _kblMapper.GetModulesOfObject(wireCore.SystemId).ToList
                        If modulesOfObject.Count > 0 Then
                            Dim modules As String = String.Empty
                            For Each [module] As E3.Lib.Schema.Kbl.Module In modulesOfObject
                                Dim moduleInfo As String = If(String.IsNullOrEmpty([module].Abbreviation), [module].Part_number, String.Format("{0} [{1}]", [module].Abbreviation, [module].Part_number))
                                If String.IsNullOrEmpty(modules) Then
                                    modules = moduleInfo
                                Else
                                    modules = String.Format("{0}, {1}", modules, moduleInfo)
                                End If
                            Next

                            cableWireRow.SetCellValue(ColumnKeys.HarnessModules.ToString, modules)
                        End If
                    End If

                    If (generalWire IsNot Nothing) Then
                        cableWireRow.SetCellValue(ColumnKeys.PartNumber.ToString, generalWire.Part_number)
                        cableWireRow.SetCellValue(ColumnKeys.InitialPartnumber.ToString, generalWire.Part_number)
                    End If

                    cableWireRow.SetCellValue(ColumnKeys.TemporaryAdded.ToString, False)
                    cableWireRow.SetCellValue(ColumnKeys.TemporaryRemoved.ToString, False)
                    cableWireRow.SetCellValue(ColumnKeys.TemporaryModified.ToString, False)
                    cableWireRow.SetCellValue(ColumnKeys.GeneralWire.ToString, generalWire)
                    cableWireRow.SetCellValue(ColumnKeys.InitialGeneralWire.ToString, generalWire)

                    GetOutsideDiameter(cableWireRow, generalWire)

                    If (segmentCircle IsNot Nothing) Then
                        For Each circle As PackagingCircle In segmentCircle.InnerCircles.Where(Function(innerCircle) innerCircle.Id = wireCore.SystemId AndAlso innerCircle.AllowEnlargement)
                            cableWireRow.SetCellValue(ColumnKeys.OutsideDia.ToString, String.Format("{0} mm", Math.Round(circle.Diameter, 1)))
                            cableWireRow.SetCellValue(ColumnKeys.OutsideDiaVal.ToString, circle.Diameter)
                            cableWireRow.SetCellValue(ColumnKeys.DiaSource.ToString, BundleCrossSectionFormStrings.Packaged_CellValue)
                        Next
                    End If

                    cableWireRow.SetCellValue(ColumnKeys.TemporaryRemoved.ToString, False)
                Next
            Next
        End With

        Me.ugBundleCSAs.DataSource = Me.udsBundleCSAs
    End Sub

    Private Sub InitializePartNumbers()
        Dim partNumberTable As New Data.DataTable("Partnumbers")

        With partNumberTable
            .Columns.Add(ColumnKeys.PartNumber.ToString, GetType(String))
            .Columns.Add(ColumnKeys.OutsideDia.ToString, GetType(String))
            .Columns.Add(ColumnKeys.OutsideDiaVal.ToString, GetType(Double))
            .Columns.Add(ColumnKeys.Source.ToString, GetType(String))
            .Columns.Add(ColumnKeys.GeneralWire.ToString, GetType(General_wire))

            Dim customRow As Data.DataRow = .NewRow
            customRow(ColumnKeys.PartNumber.ToString) = KblObjectType.Custom.ToLocalizedString
            customRow(ColumnKeys.OutsideDia.ToString) = "0.5 mm"
            customRow(ColumnKeys.OutsideDiaVal.ToString) = 0.5
            customRow(ColumnKeys.Source.ToString) = BundleCrossSectionFormStrings.User_CellValue

            partNumberTable.Rows.Add(customRow)

            Dim generalWires As New SortedDictionary(Of String, General_wire)

            For Each generalWire As General_wire In _kblMapper.GetGeneralWires
                If (Not generalWires.ContainsKey(generalWire.Part_number)) Then
                    generalWires.Add(generalWire.Part_number, generalWire)
                End If
            Next

            For Each generalWire As General_wire In generalWires.Values
                If (generalWire.Outside_diameter IsNot Nothing) AndAlso (generalWire.Outside_diameter.Value_component <> 0) Then
                    Dim row As Data.DataRow = .NewRow
                    row(ColumnKeys.PartNumber.ToString) = generalWire.Part_number
                    row(ColumnKeys.OutsideDia.ToString) = String.Format("{0} mm", Math.Round(generalWire.Outside_diameter.Value_component, 1))
                    row(ColumnKeys.OutsideDiaVal.ToString) = generalWire.Outside_diameter.Value_component
                    row(ColumnKeys.Source.ToString) = BundleCrossSectionFormStrings.FromKBL_CellValue
                    row(ColumnKeys.GeneralWire.ToString) = generalWire

                    .Rows.Add(row)
                End If
            Next

            For Each diameter As Diameter In _diameterSettings.Diameters
                If (Not String.IsNullOrEmpty(diameter.PartNumber) AndAlso Not generalWires.ContainsKey(diameter.PartNumber)) OrElse (diameter.WireType <> String.Empty AndAlso Not generalWires.ContainsKey(diameter.WireType)) Then
                    Dim row As Data.DataRow = .NewRow

                    If (diameter.PartNumber <> String.Empty) Then
                        row(ColumnKeys.PartNumber.ToString) = diameter.PartNumber

                        For Each generalWire As General_wire In _kblMapper.GetGeneralWires.Where(Function(genWire) genWire.Part_number = diameter.PartNumber)
                            row(ColumnKeys.GeneralWire.ToString) = generalWire
                            Exit For
                        Next
                    Else
                        row(ColumnKeys.PartNumber.ToString) = diameter.WireType

                        For Each generalWire As General_wire In _kblMapper.GetGeneralWires.Where(Function(genWire) genWire.Wire_type = diameter.WireType)
                            row(ColumnKeys.GeneralWire.ToString) = generalWire
                            Exit For
                        Next
                    End If

                    row(ColumnKeys.OutsideDia.ToString) = String.Format("{0} mm", Math.Round(diameter.Value, 1))
                    row(ColumnKeys.OutsideDiaVal.ToString) = diameter.Value
                    row(ColumnKeys.Source.ToString) = BundleCrossSectionFormStrings.ConfigFile_CellValue

                    .Rows.Add(row)
                End If
            Next
        End With

        Me.uddPartNumbers.DataSource = partNumberTable

        With Me.uddPartNumbers.DisplayLayout
            With .Bands(0)
                .Columns(ColumnKeys.OutsideDiaVal.ToString).Hidden = True
                .Columns(ColumnKeys.GeneralWire.ToString).Hidden = True

                With .Columns(ColumnKeys.PartNumber.ToString)
                    .SortIndicator = SortIndicator.Ascending
                    .Width = 150
                    .Header.Caption = BundleCrossSectionFormStrings.PartNumber_ColumnCaption
                End With
                With .Columns(ColumnKeys.OutsideDia.ToString)
                    .Width = 75
                    .Header.Caption = BundleCrossSectionFormStrings.OutsideDia_ColumnCaption
                End With
                With .Columns(ColumnKeys.Source.ToString)
                    .Width = 100
                    .Header.Caption = BundleCrossSectionFormStrings.Source_ColumnCaption
                End With

            End With
        End With

        Me.uddPartNumbers.ValueMember = ColumnKeys.PartNumber.ToString
        Me.uddPartNumbers.DisplayMember = ColumnKeys.PartNumber.ToString

        If (Me.ugBundleCSAs.DisplayLayout.Bands.Count >= 2) Then
            Me.ugBundleCSAs.DisplayLayout.Bands(1).Columns(ColumnKeys.PartNumber.ToString).ValueList = Me.uddPartNumbers
        End If
    End Sub

    Private Sub InitializeVectorDrawBaseControl()
        Me.vDraw.AllowDrop = False
        Me.vDraw.EnsureDocument()
        Me.vDraw.InputDevice_TouchGesture.Actions = VectorDraw.Professional.Actions.TouchGestureActions.Pan Or VectorDraw.Professional.Actions.TouchGestureActions.Zoom

        With Me.vDraw.ActiveDocument
            .UndoHistory.PushEnable(False)
            .Selections.Add(New vdSelection())
            .EnableAutoFocus = True
            .EnableAutoGripOn = False
            .EnableUrls = False
            .EnableToolTips = False
            .DisableRedraw = False

            With .GlobalRenderProperties
                .AxisSize = 10
                .CrossSize = 8
                .CurveResolution = 1000
                .GridColor = Color.Black
                .SelectingCrossColor = Color.Transparent
                .SelectingWindowColor = Color.Transparent
                .TimerBreakForDraw = 500
                .OpenGlAntializing = 8
                .RenderingQuality = VectorDraw.Render.vdRender.RenderingQualityMode.HighQuality
            End With

            .GridMode = False
            .OrbitActionKey = vdDocument.KeyWithMouseStroke.None
            .OsnapDialogKey = Keys.None
            .Palette.Background = Color.White
            .ShowUCSAxis = False
            .ToolTipDispProps.UseReverseOrder = False
            .UrlActionKey = Keys.None
        End With

    End Sub

    Private Sub AddDraggedCableWire(cableWireRow As UltraDataRow, parentBand As UltraGridBand, parentCollection As RowsCollection, doRowClone As Boolean)
        If (doRowClone) Then
            Dim existingRow As UltraGridRow = parentCollection.Where(Function(row) row.Band.Key = CABLE_WIRE_BANDHEADER AndAlso row.Cells(ColumnKeys.CabWirNum.ToString).Value.ToString = BuildUserWireId(cableWireRow.GetCellValue(ColumnKeys.CabWirNum.ToString).ToString)).FirstOrDefault
            If (existingRow Is Nothing) Then
                'build user wire 
                _droppedRows.Add(CreateCableWireRowClone(cableWireRow, parentBand))
            Else
                UpdateCableWireRow(cableWireRow, existingRow)
            End If
        Else

            Dim existingRow As UltraGridRow = parentCollection.Where(Function(row) row.Band.Key = CABLE_WIRE_BANDHEADER AndAlso row.Cells(ColumnKeys.CabWirNum.ToString).Value.ToString = cableWireRow.GetCellValue(ColumnKeys.CabWirNum.ToString).ToString).FirstOrDefault
            If (existingRow Is Nothing) Then
                'Try to find user row
                existingRow = parentCollection.Where(Function(row) row.Band.Key = CABLE_WIRE_BANDHEADER AndAlso row.Cells(ColumnKeys.CabWirNum.ToString).Value.ToString = BuildUserWireId(cableWireRow.GetCellValue(ColumnKeys.CabWirNum.ToString).ToString)).FirstOrDefault
                If (existingRow Is Nothing) Then
                    'build user wire
                    _droppedRows.Add(CreateCableWireRowClone(cableWireRow, parentBand))
                Else
                    UpdateCableWireRow(cableWireRow, existingRow)
                End If

            Else
                UpdateCableWireRow(cableWireRow, existingRow)
            End If
        End If
    End Sub

    Private Sub AddUserWire()
        If (_clickedRow Is Nothing) Then
            Exit Sub
        End If

        Me.ugBundleCSAs.BeginUpdate()
        Me.ugBundleCSAs.ActiveRow = _clickedRow

        _clickedRow = _clickedRow.Band.AddNew

        With _clickedRow
            .Cells(ColumnKeys.Id.ToString).Value = Guid.NewGuid.ToString
            .Cells(ColumnKeys.CabWirNum.ToString).Value = BuildUserWireId(_userWireNumber.Next(1000, 9999).ToString)
            .Cells(ColumnKeys.Class.ToString).Value = BundleCrossSectionFormStrings.User_CellValue
            .Cells(ColumnKeys.PartNumber.ToString).Value = DirectCast(Me.uddPartNumbers.DataSource, Data.DataTable).Rows(0)(ColumnKeys.PartNumber.ToString)
            .Cells(ColumnKeys.OutsideDia.ToString).Value = DirectCast(Me.uddPartNumbers.DataSource, Data.DataTable).Rows(0)(ColumnKeys.OutsideDia.ToString)
            .Cells(ColumnKeys.OutsideDiaVal.ToString).Value = DirectCast(Me.uddPartNumbers.DataSource, Data.DataTable).Rows(0)(ColumnKeys.OutsideDiaVal.ToString)
            .Cells(ColumnKeys.DiaSource.ToString).Value = BundleCrossSectionFormStrings.User_CellValue
            .Cells(ColumnKeys.TemporaryAdded.ToString).Value = True
        End With

        With Me.ugBundleCSAs
            .EndUpdate()

            .ActiveRow = _clickedRow
            .Selected.Rows.Clear()
            .Selected.Rows.Add(_clickedRow)
        End With
    End Sub

    Private Function BuildUserWireId(name As String) As String
        Return String.Format("UW_{0}", name)
    End Function

    Private Sub BulkChangePartnumber()
        If (Me.ugBundleCSAs.Selected.Rows.Count > 0) Then
            Using partNumberPicker As New PartNumberPicker
                partNumberPicker.DataSource = uddPartNumbers.DataSource
                If (partNumberPicker.ShowDialog(Me) = DialogResult.OK) Then
                    Dim dropDownRow As UltraGridRow = partNumberPicker.SelectedRow
                    For Each selectedRow As UltraGridRow In Me.ugBundleCSAs.Selected.Rows
                        With selectedRow
                            If (.Band.Key = CABLE_WIRE_BANDHEADER AndAlso Not CBool(.Cells(ColumnKeys.TemporaryRemoved.ToString).Value)) Then
                                With DirectCast(selectedRow.ListObject, UltraDataRow)
                                    .SetCellValue(ColumnKeys.PartNumber.ToString, dropDownRow.Cells(ColumnKeys.PartNumber.ToString).Value)
                                    .SetCellValue(ColumnKeys.GeneralWire.ToString, dropDownRow.Cells(ColumnKeys.GeneralWire.ToString).Value)

                                    If (dropDownRow.Cells(ColumnKeys.PartNumber.ToString).Value.ToString = KblObjectType.Custom.ToLocalizedString) Then
                                        .SetCellValue(ColumnKeys.OutsideDia.ToString, dropDownRow.Cells(ColumnKeys.OutsideDia.ToString).Value)
                                        .SetCellValue(ColumnKeys.OutsideDiaVal.ToString, 0)
                                        .SetCellValue(ColumnKeys.DiaSource.ToString, BundleCrossSectionFormStrings.User_CellValue)
                                        .SetCellValue(ColumnKeys.GeneralWire.ToString, Nothing)

                                        If (Not (CBool(.GetCellValue(ColumnKeys.TemporaryAdded.ToString)) OrElse CBool(.GetCellValue(ColumnKeys.TemporaryRemoved.ToString)))) Then
                                            .SetCellValue(ColumnKeys.TemporaryModified.ToString, True)
                                        End If
                                    Else
                                        .SetCellValue(ColumnKeys.GeneralWire.ToString, dropDownRow.Cells(ColumnKeys.GeneralWire.ToString).Value)
                                        If (CBool(.GetCellValue(ColumnKeys.TemporaryModified.ToString))) Then
                                            If (CStr(.GetCellValue(ColumnKeys.PartNumber.ToString)) = CStr(.GetCellValue(ColumnKeys.InitialPartnumber.ToString))) Then
                                                .SetCellValue(ColumnKeys.TemporaryModified.ToString, False)
                                            End If
                                        Else
                                            If (Not (CBool(.GetCellValue(ColumnKeys.TemporaryAdded.ToString)) OrElse CBool(.GetCellValue(ColumnKeys.TemporaryRemoved.ToString)))) Then
                                                .SetCellValue(ColumnKeys.TemporaryModified.ToString, True)
                                            End If
                                        End If
                                        GetOutsideDiameter(DirectCast(selectedRow.ListObject, UltraDataRow), TryCast(dropDownRow.Cells(ColumnKeys.GeneralWire.ToString).Value, General_wire))
                                    End If
                                End With
                            End If
                        End With
                    Next
                End If
            End Using
        End If
    End Sub

    Private Sub CalculateDiameter()
        If (Not Me.bwCSACalculation.IsBusy) Then
            ToggleControlAccessibility(False)

            Me.vDraw.ActiveDocument.ActiveLayOut.Entities.RemoveAll()
            Me.vDraw.ActiveDocument.Invalidate()

            Me.bwCSACalculation.RunWorkerAsync()
        End If
    End Sub

    Private Sub CheckForCablesWiresWithoutModuleAssignment()
        Dim coresWiresWithoutModuleAssignment As String = String.Empty
        Dim counter As Integer = 1

        For Each coreWire As IKblWireCoreOccurrence In _kblMapper.GetWireOrCoresOfSegment(_segment.SystemId)
            If Not _kblMapper.ObjectIsInAnyModule(coreWire.SystemId) Then
                Dim specWireOcc As Special_wire_occurrence = _kblMapper.GetGeneralWireOccurrences.OfType(Of Special_wire_occurrence).Where(Function(s) s.Core_occurrence IsNot Nothing AndAlso s.Core_occurrence.Any(Function(c) c.SystemId = coreWire.SystemId)).FirstOrDefault
                Dim coreOcc As Core_occurrence = If(specWireOcc IsNot Nothing, specWireOcc.Core_occurrence.Where(Function(c) c.SystemId = coreWire.SystemId).FirstOrDefault, Nothing)
                Dim wireOcc As Wire_occurrence = _kblMapper.GetGeneralWireOccurrences.OfType(Of Wire_occurrence).Where(Function(w) w.SystemId = coreWire.SystemId).FirstOrDefault
                Dim genWire As General_wire = If(coreOcc IsNot Nothing, _kblMapper.GetGeneralWires.Where(Function(genW) genW.Core IsNot Nothing AndAlso genW.Core.Any(Function(c) c.SystemId = coreOcc.Part)).FirstOrDefault, _kblMapper.GetGeneralWires.Where(Function(genW) genW.SystemId = wireOcc.Part).FirstOrDefault)

                Dim coreWireInformation As String = String.Empty

                If (coreOcc IsNot Nothing) Then
                    Dim core As Core = genWire.Core.Where(Function(c) c.SystemId = coreOcc.Part).FirstOrDefault

                    coreWireInformation = String.Format("{0}: '{1}' [{2}]{3}", KblObjectType.Core_occurrence.ToLocalizedString, coreOcc.Wire_number, If(core.Wire_type IsNot Nothing, core.Wire_type, core.Id), vbCrLf)
                Else
                    coreWireInformation = String.Format("{0}: '{1}' [{2}]{3}", KblObjectType.Wire_occurrence.ToLocalizedString, wireOcc.Wire_number, If(genWire.Wire_type IsNot Nothing, genWire.Wire_type, genWire.Part_number), vbCrLf)
                End If

                If (coresWiresWithoutModuleAssignment = String.Empty) Then
                    coresWiresWithoutModuleAssignment = String.Format("{0}{1}", String.Format("{0}: {1}{2}", BundleCrossSectionFormStrings.WiresWithoutModuleAssignment_Msg, vbCrLf, vbCrLf), coreWireInformation)
                Else
                    coresWiresWithoutModuleAssignment = String.Format("{0}{1}", coresWiresWithoutModuleAssignment, coreWireInformation)
                End If

                counter += 1

                If (counter > 20) Then
                    coresWiresWithoutModuleAssignment = String.Format("{0}!", BundleCrossSectionFormStrings.WiresWithoutModuleAssignment_Msg)

                    Exit For
                End If
            End If
        Next

        If Not String.IsNullOrEmpty(coresWiresWithoutModuleAssignment) Then
            _messageBoxDisplayed = True

            MessageBoxEx.ShowWarning(Me, coresWiresWithoutModuleAssignment)
        End If
    End Sub

    Private Sub CopySelectedUserWires()
        _copiedUserWires.Clear()
        If (Me.ugBundleCSAs.Selected.Rows.Count > 0) Then
            For Each row As UltraGridRow In Me.ugBundleCSAs.Selected.Rows
                If (CBool(row.Cells(ColumnKeys.TemporaryAdded.ToString).Value) = True) Then
                    _copiedUserWires.Add(row)
                End If
            Next
        End If
    End Sub

    Private Sub CopyAllUserWires()
        _copiedUserWires.Clear()

        For Each row As UltraGridRow In _clickedRow.ParentRow.ChildBands(0).Rows.Where(Function(childRow) CBool(childRow.Cells(ColumnKeys.TemporaryAdded.ToString).Value) = True)
            _copiedUserWires.Add(row)
        Next
    End Sub

    Private Function CreateCableWireRowClone(sourceCableWireRow As UltraDataRow, parentBand As UltraGridBand) As UltraGridRow
        Dim addedRow As UltraGridRow = parentBand.AddNew
        With addedRow
            .Cells(ColumnKeys.Id.ToString).Value = Guid.NewGuid.ToString
            .Cells(ColumnKeys.CabWirNum.ToString).Value = BuildUserWireId(sourceCableWireRow.GetCellValue(ColumnKeys.CabWirNum.ToString).ToString)
            .Cells(ColumnKeys.Class.ToString).Value = BundleCrossSectionFormStrings.User_CellValue
            .Cells(ColumnKeys.PartNumber.ToString).Value = sourceCableWireRow.GetCellValue(ColumnKeys.PartNumber.ToString)
            .Cells(ColumnKeys.OutsideDia.ToString).Value = sourceCableWireRow.GetCellValue(ColumnKeys.OutsideDia.ToString)
            .Cells(ColumnKeys.OutsideDiaVal.ToString).Value = sourceCableWireRow.GetCellValue(ColumnKeys.OutsideDiaVal.ToString)
            .Cells(ColumnKeys.DiaSource.ToString).Value = sourceCableWireRow.GetCellValue(ColumnKeys.DiaSource.ToString)
            .Cells(ColumnKeys.GeneralWire.ToString).Value = sourceCableWireRow.GetCellValue(ColumnKeys.GeneralWire.ToString)
            .Cells(ColumnKeys.TemporaryAdded.ToString).Value = True
        End With

        Return addedRow
    End Function

    Private Sub DrawBundlePicture()
        If (_calculatedSegmentDiameters(_segment.SystemId).ContainsKey(_selectedModuleConfig)) AndAlso (_calculatedSegmentDiameters(_segment.SystemId)(_selectedModuleConfig) IsNot Nothing) AndAlso (_calculatedSegmentDiameters(_segment.SystemId)(_selectedModuleConfig).InnerCircles.Count <> 0) Then
            Dim segmentCircle As PackagingCircle = _calculatedSegmentDiameters(_segment.SystemId)(_selectedModuleConfig)

            If (_bundleAddOnInstallationTolerance > 0) OrElse (_bundleAddOnProvisioningTolerance > 0) Then
                Dim toleranceCircle As New PackagingCircle("Tolerance circle") With {.Radius = GetPackagedOutsideDiameter(segmentCircle) / 2}

                If (segmentCircle.HasInvalidOutsideDiameter) Then
                    DrawInvalidSegmentText()
                Else
                    DrawCircle(toleranceCircle, Me.vDraw.ActiveDocument.LineTypes.Solid, Color.Black, True)
                    DrawCircleInCircle_Recursively(segmentCircle)
                End If

                DrawCaptionInformation(toleranceCircle)

                If (Not segmentCircle.HasInvalidOutsideDiameter) Then
                    DrawDimensions(segmentCircle, toleranceCircle)
                End If
            Else
                If (segmentCircle.HasInvalidOutsideDiameter) Then
                    DrawInvalidSegmentText()
                Else
                    DrawCircleInCircle_Recursively(segmentCircle)
                End If

                DrawCaptionInformation(segmentCircle)

                If (Not segmentCircle.HasInvalidOutsideDiameter) Then
                    DrawDimensions(segmentCircle)
                End If
            End If
        End If

        ZoomDrawing()
    End Sub

    Private Sub DrawCaptionInformation(circle As PackagingCircle)
        Dim moduleConfigName As String = String.Empty

        If (Regex.Replace(_selectedModuleConfig.HarnessConfiguration.Part_number, "\s", String.Empty) <> String.Empty) Then
            moduleConfigName = _selectedModuleConfig.HarnessConfiguration.Part_number
        ElseIf (Regex.Replace(_selectedModuleConfig.HarnessConfiguration.Abbreviation, "\s", String.Empty) <> String.Empty) Then
            moduleConfigName = _selectedModuleConfig.HarnessConfiguration.Abbreviation
        ElseIf (Regex.Replace(_selectedModuleConfig.HarnessConfiguration.Description, "\s", String.Empty) <> String.Empty) Then
            moduleConfigName = _selectedModuleConfig.HarnessConfiguration.Description
        End If

        Dim captionText As New vdMText
        With captionText
            .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
            .setDocumentDefaults()
            .VerJustify = VdConstVerJust.VdTextVerBottom
            .HorJustify = VdConstHorJust.VdTextHorCenter
            .Height = circle.Radius / 10
            .InsertionPoint = New gPoint(circle.Center.X, circle.Radius + (circle.Radius / 2.5))
            .PenColor.SystemColor = Color.Black

            If (_bundleAddOnInstallationTolerance > 0 OrElse _bundleAddOnProvisioningTolerance > 0) Then
                Dim tolerance As Double = _bundleAddOnInstallationTolerance + _bundleAddOnProvisioningTolerance

                If (_diameterSettings.IsAddOnToleranceOnArea) Then
                    .TextString = String.Format(BundleCrossSectionFormStrings.BundlePic_Caption2, _segment.Id, vbCrLf, moduleConfigName, CInt(tolerance * 100))
                Else
                    .TextString = String.Format(BundleCrossSectionFormStrings.BundlePic_Caption1, _segment.Id, vbCrLf, moduleConfigName, CInt(tolerance * 100))
                End If

            Else
                .TextString = String.Format(BundleCrossSectionFormStrings.BundlePic_Caption3, _segment.Id, vbCrLf, moduleConfigName)
            End If
        End With

        Me.vDraw.ActiveDocument.ActiveLayOut.Entities.AddItem(captionText)
    End Sub

    Private Function DrawCircle(circle As PackagingCircle, lineType As vdLineType, penColor As Color, withHatch As Boolean) As vdCircle
        Dim vdCircle As New vdCircle
        With vdCircle
            .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
            .setDocumentDefaults()

            .Center = New gPoint(circle.Center.X, circle.Center.Y)
            .XProperties.Add("Circle").PropValue = circle

            If (withHatch) Then
                Dim hatchProperties As New vdHatchProperties
                hatchProperties.SetUnRegisterDocument(Me.vDraw.ActiveDocument)

                If (circle.Id = "Tolerance circle") Then
                    hatchProperties.FillMode = VdConstFill.VdFillModeHatchBDiagonal
                    hatchProperties.FillColor.SystemColor = Color.DarkGray
                ElseIf (TypeOf circle.Tag Is Wiring_group) Then
                    hatchProperties.FillMode = VdConstFill.VdFillModeHatchDiagCross
                    hatchProperties.FillColor.SystemColor = Color.Black
                Else
                    hatchProperties.FillMode = VdConstFill.VdFillModeSolid

                    If (TypeOf circle.Tag Is Segment) Then
                        hatchProperties.FillColor.SystemColor = Color.DarkGray
                    ElseIf (circle.HasModification) Then
                        hatchProperties.FillColor.AlphaBlending = 25
                        hatchProperties.FillColor.SystemColor = Color.Red
                    Else
                        hatchProperties.FillColor.SystemColor = Color.LightGray
                    End If
                End If

                .HatchProperties = hatchProperties
            End If

            .Label = circle.Id
            .LineType = lineType
            .PenColor.SystemColor = penColor
            .Radius = circle.Radius

            If (Not TypeOf circle.Tag Is Segment) Then
                .XProperties.Add("Tooltip").PropValue = circle.Description
            End If
        End With

        Me.vDraw.ActiveDocument.ActiveLayOut.Entities.AddItem(vdCircle)

        Return vdCircle
    End Function

    Private Sub DrawCircleInCircle_Recursively(circle As PackagingCircle)
        Dim vdCircle As vdCircle = DrawCircle(circle, Me.vDraw.ActiveDocument.LineTypes.Solid, Color.Black, True)

        If (circle.CalculatedRadius <> 0) AndAlso (circle.CalculatedRadius < circle.Radius) Then
            DrawCircle(New PackagingCircle(circle.Id) With {.Center = circle.Center, .Description = circle.Description, .Radius = circle.CalculatedRadius}, Me.vDraw.ActiveDocument.LineTypes.DPIDash, Color.Black, False)
        End If

        If (TypeOf circle.Tag Is Core) Then
            Dim core As Core = DirectCast(circle.Tag, Core)

            DrawColorInformationForCircle(vdCircle, core.GetColours.Split("/".ToCharArray, StringSplitOptions.RemoveEmptyEntries).ToList)
        ElseIf (TypeOf circle.Tag Is General_wire) AndAlso (circle.InnerCircles.Count = 0) Then
            Dim generalWire As General_wire = DirectCast(circle.Tag, General_wire)

            DrawColorInformationForCircle(vdCircle, generalWire.GetColours.Split("/".ToCharArray, StringSplitOptions.RemoveEmptyEntries).ToList)
        End If

        For Each innerCircle As PackagingCircle In circle.InnerCircles
            DrawCircleInCircle_Recursively(innerCircle)
        Next

        If (circle.HasInvalidOutsideDiameter) Then
            DrawCircle(New PackagingCircle(circle.Id) With {.Center = circle.Center, .Description = circle.Description, .Radius = circle.CalculatedRadius}, Me.vDraw.ActiveDocument.LineTypes.DPIDash, Color.Red, False)
        End If
    End Sub

    Private Sub DrawColorInformationForCircle(vdCircle As vdCircle, colors As List(Of String))
        Dim colorCode1 As String = If(colors.Count >= 1, colors(0), String.Empty)
        Dim colorCode2 As String = If(colors.Count >= 2, colors(1), String.Empty)

        Dim color1 As Color = Color.Transparent
        Dim color2 As Color = Color.Transparent

        For Each wireColorCode As WireColorCode In _wireColorCodes.Where(Function(colorCode) colorCode.ColorCode = colorCode1)
            color1 = wireColorCode.Color
        Next

        If (colorCode2 <> String.Empty) Then
            For Each wireColorCode As WireColorCode In _wireColorCodes.Where(Function(colorCode) colorCode.ColorCode = colorCode2)
                color2 = wireColorCode.Color
            Next
        End If

        If (color1 <> Color.Transparent) AndAlso (color2 <> Color.Transparent) Then
            Dim vertexes As New Vertexes
            vertexes.Add(New gPoint(vdCircle.Center.x - vdCircle.Radius / 3, vdCircle.Center.y - vdCircle.Radius / 3))
            vertexes.Add(New gPoint(vdCircle.Center.x - vdCircle.Radius / 3, vdCircle.Center.y + vdCircle.Radius / 3))
            vertexes.Add(New gPoint(vdCircle.Center.x + vdCircle.Radius / 3, vdCircle.Center.y - vdCircle.Radius / 3))

            Me.vDraw.ActiveDocument.ActiveLayOut.Entities.AddItem(DrawColorPolyline(color1, vdCircle.Label, colorCode1, vertexes))

            vertexes = New Vertexes
            vertexes.Add(New gPoint(vdCircle.Center.x - vdCircle.Radius / 3, vdCircle.Center.y + vdCircle.Radius / 3))
            vertexes.Add(New gPoint(vdCircle.Center.x + vdCircle.Radius / 3, vdCircle.Center.y + vdCircle.Radius / 3))
            vertexes.Add(New gPoint(vdCircle.Center.x + vdCircle.Radius / 3, vdCircle.Center.y - vdCircle.Radius / 3))

            Me.vDraw.ActiveDocument.ActiveLayOut.Entities.AddItem(DrawColorPolyline(color2, vdCircle.Label, colorCode2, vertexes))
        ElseIf (color1 <> Color.Transparent) Then
            Dim vertexes As New Vertexes
            vertexes.Add(New gPoint(vdCircle.Center.x - vdCircle.Radius / 3, vdCircle.Center.y - vdCircle.Radius / 3))
            vertexes.Add(New gPoint(vdCircle.Center.x - vdCircle.Radius / 3, vdCircle.Center.y + vdCircle.Radius / 3))
            vertexes.Add(New gPoint(vdCircle.Center.x + vdCircle.Radius / 3, vdCircle.Center.y + vdCircle.Radius / 3))
            vertexes.Add(New gPoint(vdCircle.Center.x + vdCircle.Radius / 3, vdCircle.Center.y - vdCircle.Radius / 3))

            Me.vDraw.ActiveDocument.ActiveLayOut.Entities.AddItem(DrawColorPolyline(color1, vdCircle.Label, colorCode1, vertexes))
        End If
    End Sub

    Private Function DrawColorPolyline(color As Color, label As String, tooltip As String, vertexes As Vertexes) As vdPolyline
        Dim vdPolyline As New vdPolyline
        With vdPolyline
            .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
            .setDocumentDefaults()

            .Flag = VdConstPlineFlag.PlFlagCLOSE

            Dim hatchProperties As New vdHatchProperties(VdConstFill.VdFillModeSolid)
            hatchProperties.SetUnRegisterDocument(Me.vDraw.ActiveDocument)
            hatchProperties.FillColor.SystemColor = color

            .HatchProperties = hatchProperties
            .Label = label
            .VertexList = vertexes

            If (Me.uckSimpleBundleView.Checked) Then
                .visibility = vdFigure.VisibilityEnum.Invisible
            Else
                .visibility = vdFigure.VisibilityEnum.Visible
            End If

            .XProperties.Add("Tooltip").PropValue = String.Format(BundleCrossSectionFormStrings.Color_Text, tooltip)
        End With

        Return vdPolyline
    End Function

    Private Sub DrawDimensions(circle As PackagingCircle, Optional toleranceCircle As PackagingCircle = Nothing)
        Dim dimension As New vdDimension
        With dimension
            .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
            .setDocumentDefaults()

            .ArrowSize = circle.Radius / 12
            .DecimalPrecision = 2
            .DefPoint1 = New gPoint(-circle.Radius, 0)
            .DefPoint2 = New gPoint(circle.Radius, 0)
            .dimText = String.Format(BundleCrossSectionFormStrings.SegmentBundlePicDim_Label, Math.Round(GetCalculatedOutsideDiameter(circle, True), 1), Math.Round(circle.Diameter, 1))

            If (toleranceCircle IsNot Nothing) Then
                .LinePosition = New gPoint(0, -toleranceCircle.Radius - (toleranceCircle.Radius / 3))
            Else
                .LinePosition = New gPoint(0, -circle.Radius - (circle.Radius / 3))
            End If

            .TextDist = 0
            .TextHeight = .ArrowSize / 1.25
        End With

        Me.vDraw.ActiveDocument.ActiveLayOut.Entities.AddItem(dimension)

        If (toleranceCircle IsNot Nothing) Then
            dimension = New vdDimension
            With dimension
                .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                .setDocumentDefaults()

                .ArrowSize = circle.Radius / 8
                .DecimalPrecision = 2
                .DefPoint1 = New gPoint(-toleranceCircle.Radius, 0)
                .DefPoint2 = New gPoint(toleranceCircle.Radius, 0)
                .dimText = String.Format(BundleCrossSectionFormStrings.SegmentBundlePicDim_Label, Math.Round(GetCalculatedOutsideDiameter(circle), 1), Math.Round(toleranceCircle.Diameter, 1))
                .LinePosition = New gPoint(0, (-toleranceCircle.Radius - (toleranceCircle.Radius / 3)) * 1.2)
                .TextDist = 0
                .TextHeight = .ArrowSize / 1.5
            End With

            Me.vDraw.ActiveDocument.ActiveLayOut.Entities.AddItem(dimension)
        End If

        DrawLegend(dimension)
    End Sub

    Private Sub DrawInvalidSegmentText()
        Dim invalidSegmentText As New vdText
        With invalidSegmentText
            .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
            .setDocumentDefaults()

            .Height = 2.5
            .HorJustify = VdConstHorJust.VdTextHorCenter
            .InsertionPoint = New gPoint(20, -15)
            .PenColor.SystemColor = Color.Red
            .Rotation = 0.45
            .TextString = BundleCrossSectionFormStrings.InvalidSegment_Text
            .VerJustify = VdConstVerJust.VdTextVerCen
        End With

        Me.vDraw.ActiveDocument.ActiveLayOut.Entities.AddItem(invalidSegmentText)
    End Sub

    Private Sub DrawLegend(dimension As vdDimension)
        Dim legendText As New vdText
        With legendText
            .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
            .setDocumentDefaults()

            .Height = dimension.TextHeight * 0.75
            .InsertionPoint = New gPoint(dimension.LinePosition.x + dimension.DefPoint1.x, dimension.LinePosition.y - (dimension.TextHeight * 4))
            .TextString = BundleCrossSectionFormStrings.DrwLegendCalc_Text + " / " + BundleCrossSectionFormStrings.DrwLegendPack_Text
        End With

        Me.vDraw.ActiveDocument.ActiveLayOut.Entities.AddItem(legendText)
    End Sub

    Private Sub ExportBundleStructure_Recursively(circle As PackagingCircle, worksheet As Worksheet, Optional resetCounter As Boolean = False)
        Static Dim rowCounter As Integer = 1

        If (resetCounter) Then
            rowCounter = 1
        End If

        Dim csa As Numerical_value = Nothing
        Dim circle_cable As Special_wire_occurrence = _kblMapper.GetOccurrenceObject(Of Special_wire_occurrence)(circle.Id)
        Dim circle_wire_core As IKblWireCoreOccurrence = _kblMapper.GetOccurrenceObject(Of Wire_occurrence)(circle.Id)
        If circle_wire_core Is Nothing Then
            circle_wire_core = _kblMapper.GetOccurrenceObject(Of Core_occurrence)(circle.Id)
        End If

        With worksheet.Rows(rowCounter)
            If (TypeOf circle.Tag Is Core) Then
                If circle_wire_core IsNot Nothing Then
                    .Cells(0).Value = String.Format("{0}:{1} OD", _kblMapper.GetCableNameOfWireOrCore(circle.Id), circle_wire_core.Wire_number)
                End If

                csa = DirectCast(circle.Tag, Core).Cross_section_area
            ElseIf (TypeOf circle.Tag Is General_wire) Then
                If circle_cable IsNot Nothing Then
                    .Cells(0).Value = circle_cable.Special_wire_id
                ElseIf circle_wire_core IsNot Nothing Then
                    .Cells(0).Value = String.Format("{0} OD", circle_wire_core.Wire_number)

                    csa = DirectCast(circle.Tag, General_wire).Cross_section_area
                End If
            ElseIf (TypeOf circle.Tag Is Segment) Then
                .Cells(0).Value = DirectCast(circle.Tag, Segment).Id
            ElseIf (TypeOf circle.Tag Is Wiring_group) Then
                .Cells(0).Value = DirectCast(circle.Tag, Wiring_group).Id
            End If

            .Cells(1).Value = Math.Round(circle.Radius, 4)
            .Cells(2).Value = Math.Round(circle.Center.X, 4)
            .Cells(3).Value = Math.Round(circle.Center.Y, 4)

            If (TypeOf circle.Tag Is Core) OrElse (TypeOf circle.Tag Is General_wire) Then
                If circle_wire_core IsNot Nothing OrElse (circle_cable IsNot Nothing AndAlso (_kblMapper.KBLWireNetMapper.ContainsKey(circle.Id))) Then
                    .Cells(4).Value = _kblMapper.KBLWireNetMapper(circle.Id).Signal_name
                Else
                    .Cells(4).Value = "-"
                End If
            ElseIf (TypeOf circle.Tag Is Segment) OrElse (TypeOf circle.Tag Is Wiring_group) Then
                .Cells(4).Value = "-"
            End If
        End With

        rowCounter += 1

        If (csa IsNot Nothing) Then
            With worksheet.Rows(rowCounter)
                .Cells(0).Value = Replace(worksheet.Rows(rowCounter - 1).Cells(0).Value.ToString, " OD", " ID")
                .Cells(1).Value = Math.Round((2 * Math.Sqrt(csa.Value_component / Math.PI)) / 2, 4)
                .Cells(2).Value = Math.Round(circle.Center.X, 4)
                .Cells(3).Value = Math.Round(circle.Center.Y, 4)
                .Cells(4).Value = worksheet.Rows(rowCounter - 1).Cells(4).Value
            End With

            rowCounter += 1
        End If

        For Each innerCircle As PackagingCircle In circle.InnerCircles
            ExportBundleStructure_Recursively(innerCircle, worksheet)
        Next
    End Sub

    Private Function GetBundleWithCableAndWireCircles(moduleConfigRow As UltraDataRow) As PackagingCircle
        Dim segmentCircle As New PackagingCircle(_segment.SystemId)
        With segmentCircle
            .AllowEnlargement = True
            .Description = String.Format(BundleCrossSectionFormStrings.Segment_Text, _segment.Id)
            .Tag = _segment
        End With

        If (moduleConfigRow.GetChildRows(CABLE_WIRE_BANDHEADER).Count <> 0) Then
            Dim wiringGroupCircles As New Dictionary(Of String, PackagingCircle)

            For Each childRow As UltraDataRow In moduleConfigRow.GetChildRows(CABLE_WIRE_BANDHEADER)
                If (Not CBool(childRow.GetCellValue(ColumnKeys.TemporaryRemoved.ToString))) Then
                    Dim generalWire As General_wire = TryCast(childRow.GetCellValue(ColumnKeys.GeneralWire.ToString), General_wire)

                    Dim isModified As Boolean = CBool(childRow.GetCellValue(ColumnKeys.TemporaryModified.ToString))
                    Dim isAdded As Boolean = CBool(childRow.GetCellValue(ColumnKeys.TemporaryAdded.ToString))

                    Dim cableWireCircle As New PackagingCircle(childRow.GetCellValue(ColumnKeys.Id.ToString).ToString())
                    cableWireCircle.HasModification = isModified Or isAdded

                    If (TypeOf childRow.Tag Is Special_wire_occurrence) Then
                        Dim accCoreDiameters As Double = 0
                        Dim cable As Special_wire_occurrence = DirectCast(childRow.Tag, Special_wire_occurrence)

                        With cableWireCircle
                            .Description = String.Format(BundleCrossSectionFormStrings.Cable_Text, childRow.GetCellValue(ColumnKeys.CabWirNum.ToString))
                            .Parent = segmentCircle
                            If (childRow.GetCellValue(ColumnKeys.DiaSource.ToString).ToString = BundleCrossSectionFormStrings.GenFormula_CellValue) Then
                                .AllowEnlargement = True
                                .Radius = 0
                            Else
                                .Radius = CDbl(childRow.GetCellValue(ColumnKeys.OutsideDiaVal.ToString)) / 2
                            End If

                            .Tag = generalWire
                        End With

                        If (cable.Core_occurrence IsNot Nothing) AndAlso (cable.Core_occurrence.Length > 0) Then
                            For Each coreOcc As Core_occurrence In cable.Core_occurrence
                                Dim wiringGroup As Wiring_group = coreOcc.GetWiringGroup(_kblMapper.GetWiringGroups)
                                Dim wiringGroupCircle As PackagingCircle = Nothing

                                If (wiringGroup IsNot Nothing) AndAlso (wiringGroup.GetConsistencyState(_kblMapper) = [Lib].Schema.Kbl.WiringGroupConsistencyState.Valid) Then
                                    If (wiringGroupCircles.ContainsKey(wiringGroup.SystemId)) Then
                                        wiringGroupCircle = wiringGroupCircles(wiringGroup.SystemId)
                                    End If

                                    If (wiringGroupCircle Is Nothing) Then
                                        wiringGroupCircle = New PackagingCircle(wiringGroup.SystemId)

                                        With wiringGroupCircle
                                            .AllowEnlargement = True
                                            .Description = String.Format(BundleCrossSectionFormStrings.WiringGroup_Text, wiringGroup.Id)
                                            .Parent = cableWireCircle
                                            .Radius = 0
                                            .Tag = wiringGroup
                                        End With

                                        cableWireCircle.InnerCircles.Add(wiringGroupCircle)

                                        wiringGroupCircles.Add(wiringGroupCircle.Id, wiringGroupCircle)
                                    End If
                                End If

                                Dim core As Core = _kblMapper.GetPart(Of Core)(coreOcc.Part)
                                Dim coreCircle As New PackagingCircle(coreOcc.SystemId)
                                With coreCircle
                                    .Description = String.Format(BundleCrossSectionFormStrings.Core_Text, coreOcc.Wire_number)

                                    If (wiringGroupCircle IsNot Nothing) Then
                                        .Parent = wiringGroupCircle
                                    Else
                                        .Parent = cableWireCircle
                                    End If

                                    If (core.Outside_diameter IsNot Nothing) AndAlso (core.Outside_diameter.Value_component <> 0) Then
                                        .Radius = core.Outside_diameter.Value_component / 2
                                    Else
                                        .Radius = GetGenericCoreOrWireDiameter(core, _diameterSettings) / 2
                                    End If

                                    .Tag = core
                                End With

                                If (wiringGroupCircle IsNot Nothing) Then
                                    wiringGroupCircle.InnerCircles.Add(coreCircle)
                                Else
                                    cableWireCircle.InnerCircles.Add(coreCircle)
                                End If

                                accCoreDiameters += coreCircle.Radius
                            Next
                        End If

                        If (cableWireCircle.AllowEnlargement) AndAlso (accCoreDiameters = 0) Then
                            cableWireCircle.Radius = CDbl(childRow.GetCellValue(ColumnKeys.OutsideDiaVal.ToString)) / 2
                        End If

                        segmentCircle.InnerCircles.Add(cableWireCircle)
                    ElseIf (TypeOf childRow.Tag Is Wire_occurrence) Then
                        Dim wire As Wire_occurrence = DirectCast(childRow.Tag, Wire_occurrence)
                        Dim wiringGroup As Wiring_group = wire.GetWiringGroup(_kblMapper.GetWiringGroups)
                        Dim wiringGroupCircle As PackagingCircle = Nothing

                        If (wiringGroup IsNot Nothing) AndAlso (wiringGroup.GetConsistencyState(_kblMapper) = [Lib].Schema.Kbl.WiringGroupConsistencyState.Valid) Then
                            If (wiringGroupCircles.ContainsKey(wiringGroup.SystemId)) Then
                                wiringGroupCircle = wiringGroupCircles(wiringGroup.SystemId)
                            End If

                            If (wiringGroupCircle Is Nothing) Then
                                wiringGroupCircle = New PackagingCircle(wiringGroup.SystemId)

                                With wiringGroupCircle
                                    .AllowEnlargement = True
                                    .Description = String.Format(BundleCrossSectionFormStrings.WiringGroup_Text, wiringGroup.Id)
                                    .Parent = segmentCircle
                                    .Radius = 0
                                    .Tag = wiringGroup
                                End With

                                segmentCircle.InnerCircles.Add(wiringGroupCircle)

                                wiringGroupCircles.Add(wiringGroupCircle.Id, wiringGroupCircle)
                            End If
                        End If

                        With cableWireCircle
                            .Description = String.Format(BundleCrossSectionFormStrings.Wire_Text, childRow.GetCellValue(ColumnKeys.CabWirNum.ToString))
                            If (wiringGroupCircle IsNot Nothing) Then
                                .Parent = wiringGroupCircle
                            Else
                                .Parent = segmentCircle
                            End If

                            .Radius = CDbl(childRow.GetCellValue(ColumnKeys.OutsideDiaVal.ToString)) / 2
                            .Tag = generalWire
                        End With

                        If (wiringGroupCircle IsNot Nothing) Then
                            wiringGroupCircle.InnerCircles.Add(cableWireCircle)
                        Else
                            segmentCircle.InnerCircles.Add(cableWireCircle)
                        End If
                    Else
                        With cableWireCircle
                            .Description = String.Format(BundleCrossSectionFormStrings.UserWire_Text, childRow.GetCellValue(ColumnKeys.CabWirNum.ToString))
                            .Parent = segmentCircle
                            .Radius = CDbl(childRow.GetCellValue(ColumnKeys.OutsideDiaVal.ToString)) / 2
                            .Tag = generalWire
                        End With

                        segmentCircle.InnerCircles.Add(cableWireCircle)
                    End If
                End If
            Next
        End If

        Return segmentCircle
    End Function

    Private Shared Function GetCalculatedCrossSectionArea(calcSegmentOutsideDiameter As Double) As Double
        Return (calcSegmentOutsideDiameter ^ 2 * Math.PI) / 4
    End Function

    Private Function GetCalculatedOutsideDiameter(segmentCircle As PackagingCircle, Optional ignoreTolerances As Boolean = False) As Double
        If (ignoreTolerances) Then
            Return GetCalculatedOutsideDiameter(segmentCircle, _diameterSettings, 0.0, _diameterSettings.IsAddOnToleranceOnArea)
        Else
            Return GetCalculatedOutsideDiameter(segmentCircle, _diameterSettings, _bundleAddOnInstallationTolerance + _bundleAddOnProvisioningTolerance, _diameterSettings.IsAddOnToleranceOnArea)
        End If

    End Function

    Private Sub GetCircleById_Recursively(circle As PackagingCircle, id As String, ByRef foundedCircle As PackagingCircle)
        If (circle Is Nothing) Then
            Return
        End If

        If (circle.Id = id) Then
            foundedCircle = circle
        End If

        If (foundedCircle IsNot Nothing) Then
            Return
        End If

        For Each innerCirlce As PackagingCircle In circle.InnerCircles
            GetCircleById_Recursively(innerCirlce, id, foundedCircle)
        Next
    End Sub

    Private Function GetModuleConfigurations() As Dictionary(Of HarnessModuleConfiguration, PackagingCircle)
        Dim moduleConfigsWithCirles As New Dictionary(Of HarnessModuleConfiguration, PackagingCircle)

        For Each row As UltraDataRow In Me.udsBundleCSAs.Rows
            moduleConfigsWithCirles.Add(DirectCast(row.Tag, HarnessModuleConfiguration), Nothing)
        Next

        Return moduleConfigsWithCirles
    End Function

    Private Sub GetOutsideDiameter(cableWireRow As UltraDataRow, generalWire As General_wire)
        Dim diameterSource As String
        Dim partNumber As String = cableWireRow.GetCellValue(ColumnKeys.PartNumber.ToString).ToString

        Dim diameter As Diameter = _diameterSettings.Diameters.FindDiameterFromPartNumber(partNumber)
        diameterSource = BundleCrossSectionFormStrings.ConfigFile_CellValue

        If (diameter Is Nothing) Then
            diameter = _diameterSettings.Diameters.FindDiameterFromWireType(partNumber)
        End If

        If (diameter Is Nothing) AndAlso (generalWire IsNot Nothing) Then
            diameter = _diameterSettings.Diameters.FindDiameterFromWireType(generalWire.Wire_type)
        End If

        If (diameter Is Nothing) AndAlso (generalWire IsNot Nothing) AndAlso (generalWire.Outside_diameter IsNot Nothing) AndAlso (generalWire.Outside_diameter.Value_component <> 0) Then
            diameter = New Diameter(generalWire.Part_number, generalWire.Wire_type, CSng(Math.Round(generalWire.Outside_diameter.Value_component, 1)))
            diameterSource = BundleCrossSectionFormStrings.FromKBL_CellValue
        End If

        If (diameter Is Nothing) AndAlso (generalWire IsNot Nothing) Then
            If (generalWire.Core IsNot Nothing) AndAlso (generalWire.Core.Length > 0) Then
                diameter = New Diameter(generalWire.Part_number, generalWire.Wire_type, GetGenericMulticoreDiameter(generalWire, _diameterSettings))
            Else
                diameter = New Diameter(generalWire.Part_number, generalWire.Wire_type, GetGenericCoreOrWireDiameter(generalWire, _diameterSettings))
            End If

            diameterSource = BundleCrossSectionFormStrings.GenFormula_CellValue
        End If

        If (diameter Is Nothing) Then
            cableWireRow.SetCellValue(ColumnKeys.OutsideDia.ToString, BundleCrossSectionFormStrings.NotAvailable_CellVal)
            cableWireRow.SetCellValue(ColumnKeys.OutsideDiaVal.ToString, 0)
            cableWireRow.SetCellValue(ColumnKeys.DiaSource.ToString, BundleCrossSectionFormStrings.NotDeterminable_CellValue)
        Else
            cableWireRow.SetCellValue(ColumnKeys.OutsideDia.ToString, String.Format("{0} mm", diameter.Value))
            cableWireRow.SetCellValue(ColumnKeys.OutsideDiaVal.ToString, diameter.Value)
            cableWireRow.SetCellValue(ColumnKeys.DiaSource.ToString, diameterSource)
        End If
    End Sub

    Private Function GetPackagedCrossSectionArea(segmentCircle As PackagingCircle) As Double
        Dim csa As Double = segmentCircle.CrossSectionArea

        If (_diameterSettings.IsAddOnToleranceOnArea) Then
            csa *= 1 + (_bundleAddOnInstallationTolerance + _bundleAddOnProvisioningTolerance)
        Else
            csa = ((segmentCircle.Diameter * (1 + (_bundleAddOnInstallationTolerance + _bundleAddOnProvisioningTolerance))) ^ 2 * Math.PI) / 4
        End If

        Return csa
    End Function

    Private Function GetPackagedOutsideDiameter(segmentCircle As PackagingCircle) As Double
        Dim outsideDiameter As Double = segmentCircle.Diameter

        If (_diameterSettings.IsAddOnToleranceOnArea) Then
            outsideDiameter = 2 * Math.Sqrt(segmentCircle.CrossSectionArea * (1 + (_bundleAddOnInstallationTolerance + _bundleAddOnProvisioningTolerance)) / Math.PI)
        Else
            outsideDiameter *= 1 + (_bundleAddOnInstallationTolerance + _bundleAddOnProvisioningTolerance)
        End If

        Return outsideDiameter
    End Function

    Private Sub HighlightCableOrWireInDrawing(circle As vdCircle, withZoom As Boolean)
        circle.HatchProperties.FillColor.AlphaBlending = 255
        circle.HatchProperties.FillColor.SystemColor = Color.Magenta
        circle.Update()
        circle.Invalidate()

        If (Not _highlightedObjects.Contains(circle)) Then
            _highlightedObjects.Add(circle)
        End If

        If (withZoom) Then
            ZoomDrawing()
        Else
            Me.vDraw.ActiveDocument.Invalidate()
        End If

    End Sub

    Private Sub PasteUserWires()
        Dim pastedRow As UltraGridRow = Nothing

        Me.ugBundleCSAs.BeginUpdate()

        For Each row As UltraGridRow In _copiedUserWires
            If (Not (row.Disposed OrElse row.IsDeleted)) Then 'HINT:could happen if removed by user after a copy
                Me.ugBundleCSAs.ActiveRow = _clickedRow

                pastedRow = Me.ugBundleCSAs.ActiveRow.Band.AddNew()

                With pastedRow
                    .Cells(ColumnKeys.Id.ToString).Value = Guid.NewGuid.ToString
                    .Cells(ColumnKeys.CabWirNum.ToString).Value = BuildUserWireId(_userWireNumber.Next(1000, 9999).ToString)
                    .Cells(ColumnKeys.Class.ToString).Value = BundleCrossSectionFormStrings.User_CellValue
                    .Cells(ColumnKeys.PartNumber.ToString).Value = row.Cells(ColumnKeys.PartNumber.ToString).Text
                    .Cells(ColumnKeys.OutsideDia.ToString).Value = row.Cells(ColumnKeys.OutsideDia.ToString).Value
                    .Cells(ColumnKeys.OutsideDiaVal.ToString).Value = row.Cells(ColumnKeys.OutsideDiaVal.ToString).Value
                    .Cells(ColumnKeys.DiaSource.ToString).Value = row.Cells(ColumnKeys.DiaSource.ToString).Value
                    .Cells(ColumnKeys.TemporaryAdded.ToString).Value = True
                End With
            End If
        Next
        If pastedRow IsNot Nothing Then
            With Me.ugBundleCSAs
                .ActiveRow = pastedRow
                .Selected.Rows.Clear()
                .Selected.Rows.Add(pastedRow)
            End With
        End If
        Me.ugBundleCSAs.EndUpdate()
    End Sub

    Private Sub ReActivateCableWire()
        If (Me.ugBundleCSAs.Selected.Rows.Count <> 0) Then
            For Each selectedRow As UltraGridRow In Me.ugBundleCSAs.Selected.Rows
                If (selectedRow.Band.Key = CABLE_WIRE_BANDHEADER) Then
                    selectedRow.Cells(ColumnKeys.TemporaryRemoved.ToString).Value = False
                End If
            Next
        ElseIf (_clickedRow IsNot Nothing) Then
            _clickedRow.Cells(ColumnKeys.TemporaryRemoved.ToString).Value = False
        End If
    End Sub

    Private Sub RemoveCableWireTemporarily()
        If (Me.ugBundleCSAs.Selected.Rows.Count > 0) Then
            For Each selectedRow As UltraGridRow In Me.ugBundleCSAs.Selected.Rows
                With selectedRow
                    If (.Band.Key = CABLE_WIRE_BANDHEADER AndAlso Not CBool(.Cells(ColumnKeys.TemporaryRemoved.ToString).Value)) Then
                        .Cells(ColumnKeys.TemporaryRemoved.ToString).Value = True
                    End If
                End With
            Next
        ElseIf (_clickedRow IsNot Nothing) Then
            _clickedRow.Cells(ColumnKeys.TemporaryRemoved.ToString).Value = True
        End If
    End Sub

    Private Sub RemoveHighlightOfCablesOrWiresInDrawing()
        For Each highlightedCircle As vdCircle In _highlightedObjects
            Dim circ As PackagingCircle = TryCast(highlightedCircle.XProperties.FindName("Circle").PropValue, PackagingCircle)
            Dim wiring_group_occ As IKblOccurrence = _kblMapper.GetOccurrenceObject(Of Wiring_group)(highlightedCircle.Label)
            If wiring_group_occ IsNot Nothing Then
                highlightedCircle.HatchProperties.FillColor.SystemColor = Color.Black
            ElseIf (circ IsNot Nothing AndAlso circ.HasModification) Then
                highlightedCircle.HatchProperties.FillColor.AlphaBlending = 25
                highlightedCircle.HatchProperties.FillColor.SystemColor = Color.Red
            Else
                highlightedCircle.HatchProperties.FillColor.SystemColor = Color.LightGray
            End If

            highlightedCircle.Update()
            highlightedCircle.Invalidate()
        Next

        _highlightedObjects.Clear()

        Me.vDraw.ActiveDocument.Invalidate()
    End Sub

    Private Sub RemoveUserWire()
        If (Me.ugBundleCSAs.Selected.Rows.Count > 0) Then
            For Each selectedRow As UltraGridRow In Me.ugBundleCSAs.Selected.Rows
                If (Not (selectedRow.Band.Key = CABLE_WIRE_BANDHEADER) AndAlso (CBool(selectedRow.Cells(ColumnKeys.TemporaryAdded.ToString).Value))) Then
                    selectedRow.Selected = False
                End If
            Next
            Me.ugBundleCSAs.DeleteSelectedRows() 'bulk delete to start the worker only once
        Else
            _clickedRow.Delete()
            _clickedRow = Nothing
        End If
    End Sub

    Private Sub ResetPartNumberModification()
        If (Me.ugBundleCSAs.Selected.Rows.Count > 0) Then
            For Each selectedRow As UltraGridRow In Me.ugBundleCSAs.Selected.Rows
                With selectedRow
                    If (.Band.Key = CABLE_WIRE_BANDHEADER AndAlso CBool(.Cells(ColumnKeys.TemporaryModified.ToString).Value)) Then
                        .Cells(ColumnKeys.PartNumber.ToString).Value = .Cells(ColumnKeys.InitialPartnumber.ToString).Value
                        .Cells(ColumnKeys.GeneralWire.ToString).Value = .Cells(ColumnKeys.InitialGeneralWire.ToString).Value

                        .Cells(ColumnKeys.TemporaryModified.ToString).Value = False

                        GetOutsideDiameter(DirectCast(selectedRow.ListObject, UltraDataRow), TryCast(.Cells(ColumnKeys.GeneralWire.ToString).Value, General_wire))
                    End If
                End With
            Next
        End If
    End Sub

    Private Sub SelectInGrid(ids As List(Of String))
        Dim rowSelected As Boolean = False

        With Me.ugBundleCSAs
            .BeginUpdate()
            .ActiveRow = Nothing

            .EventManager.AllEventsEnabled = False
            .Selected.Rows.Clear()
            .EventManager.AllEventsEnabled = True

            For Each row As UltraGridRow In .Rows
                If (row.HasChild) AndAlso (DirectCast(row.ListObject, UltraDataRow).Tag.Equals(_selectedModuleConfig)) Then
                    For Each childRow As UltraGridRow In row.ChildBands(0).Rows
                        If (ids.Contains(childRow.GetCellValue(ColumnKeys.Id.ToString).ToString)) Then
                            .EventManager.AllEventsEnabled = False
                            childRow.Selected = True
                            .EventManager.AllEventsEnabled = True

                            row.ExpandAll()

                            .ActiveRowScrollRegion.ScrollRowIntoView(childRow)

                            rowSelected = True
                        End If
                    Next
                End If

                If (rowSelected) Then Exit For
            Next

            .EndUpdate()
        End With
    End Sub

    Private Sub ShowOrHideColorInformation(hide As Boolean)
        For Each figure As vdFigure In Me.vDraw.ActiveDocument.ActiveLayOut.Entities
            If (TypeOf figure Is vdPolyline) Then
                If (hide) Then
                    figure.visibility = vdFigure.VisibilityEnum.Invisible
                Else
                    figure.visibility = vdFigure.VisibilityEnum.Visible
                End If
            End If
        Next

        ZoomDrawing()
    End Sub

    Private Sub ToggleControlAccessibility(accessible As Boolean)
        Me.btnCancel.Visible = Not accessible
        Me.btnExportPicture.Enabled = accessible
        Me.btnExportTable.Enabled = accessible
        Me.btnPrint.Enabled = accessible

        Me.lblCstBundleInstallationAddOnTol.Visible = accessible
        Me.lblCstBundleProvisioningAddOnTol.Visible = accessible

        Me.uneCstBundleInstallationAddOnTol.Visible = accessible
        Me.uneCstBundleProvisioningAddOnTol.Visible = accessible

        Me.upbMain.Visible = Not accessible
        Me.uckSimpleBundleView.Visible = accessible
    End Sub

    Private Sub UpdateCableWireRow(sourceCableWireRow As UltraDataRow, rowToUpdate As UltraGridRow)
        With rowToUpdate
            .Cells(ColumnKeys.PartNumber.ToString).Value = sourceCableWireRow.GetCellValue(ColumnKeys.PartNumber.ToString)
            .Cells(ColumnKeys.OutsideDia.ToString).Value = sourceCableWireRow.GetCellValue(ColumnKeys.OutsideDia.ToString)
            .Cells(ColumnKeys.OutsideDiaVal.ToString).Value = sourceCableWireRow.GetCellValue(ColumnKeys.OutsideDiaVal.ToString)
            .Cells(ColumnKeys.DiaSource.ToString).Value = sourceCableWireRow.GetCellValue(ColumnKeys.DiaSource.ToString)
            .Cells(ColumnKeys.GeneralWire.ToString).Value = sourceCableWireRow.GetCellValue(ColumnKeys.GeneralWire.ToString)
            .Cells(ColumnKeys.TemporaryModified.ToString).Value = sourceCableWireRow.GetCellValue(ColumnKeys.TemporaryModified.ToString)
            .Cells(ColumnKeys.TemporaryRemoved.ToString).Value = sourceCableWireRow.GetCellValue(ColumnKeys.TemporaryRemoved.ToString)
        End With
    End Sub

    Private Sub UpdateCalculatedValuesInGrid()
        With Me.ugBundleCSAs
            .BeginUpdate()
            If .Rows IsNot Nothing Then
                For Each row As UltraGridRow In .Rows
                    If (DirectCast(row.ListObject, UltraDataRow).Tag.Equals(_workingModuleConfig)) Then
                        Dim segmentCircle As PackagingCircle = _calculatedSegmentDiameters(_segment.SystemId)(_workingModuleConfig)
                        If (segmentCircle IsNot Nothing) Then
                            row.Cells(ColumnKeys.OutsideDiaCalc.ToString).Value = String.Format("{0} mm", Math.Round(GetCalculatedOutsideDiameter(segmentCircle), 1))
                            row.Cells(ColumnKeys.OutsideDiaValCalc.ToString).Value = GetCalculatedOutsideDiameter(segmentCircle)
                            row.Cells(ColumnKeys.OutsideDiaPack.ToString).Value = String.Format("{0} mm", Math.Round(GetPackagedOutsideDiameter(segmentCircle), 1))
                            row.Cells(ColumnKeys.OutsideDiaValPack.ToString).Value = GetPackagedOutsideDiameter(segmentCircle)

                            row.Cells(ColumnKeys.CSACalc.ToString).Value = String.Format("{0} mm²", Math.Round(GetCalculatedCrossSectionArea(CDbl(row.Cells(ColumnKeys.OutsideDiaValCalc.ToString).Value)), 1))
                            row.Cells(ColumnKeys.CSAValCalc.ToString).Value = GetCalculatedCrossSectionArea(CDbl(row.Cells(ColumnKeys.OutsideDiaValCalc.ToString).Value))

                            row.Cells(ColumnKeys.CSAPack.ToString).Value = String.Format("{0} mm²", Math.Round(GetPackagedCrossSectionArea(segmentCircle), 1))
                            row.Cells(ColumnKeys.CSAValPack.ToString).Value = GetPackagedCrossSectionArea(segmentCircle)

                            If (row.HasChild) Then
                                For Each childRow As UltraGridRow In row.ChildBands(0).Rows
                                    Dim cable As Special_wire_occurrence = TryCast(DirectCast(childRow.ListObject, UltraDataRow).Tag, Special_wire_occurrence)
                                    If (cable IsNot Nothing) Then
                                        For Each cableCircle As PackagingCircle In segmentCircle.InnerCircles.Where(Function(innerCircle) innerCircle.Id = cable.SystemId)
                                            If (cableCircle.AllowEnlargement) Then
                                                childRow.Cells(ColumnKeys.OutsideDia.ToString).Value = String.Format("{0} mm", Math.Round(cableCircle.Diameter, 1))
                                                childRow.Cells(ColumnKeys.OutsideDiaVal.ToString).Value = cableCircle.Diameter
                                                childRow.Cells(ColumnKeys.DiaSource.ToString).Value = BundleCrossSectionFormStrings.Packaged_CellValue
                                            End If

                                            If (cableCircle.HasInvalidOutsideDiameter) Then
                                                childRow.Cells(ColumnKeys.Class.ToString).Appearance.Image = My.Resources.MismatchingConfig.ToBitmap
                                                childRow.Cells(ColumnKeys.Class.ToString).ToolTipText = BundleCrossSectionFormStrings.CabHasInvalidOutsideDia_Tooltip
                                            Else
                                                childRow.Cells(ColumnKeys.Class.ToString).Appearance.Image = Nothing
                                                childRow.Cells(ColumnKeys.Class.ToString).ToolTipText = String.Empty
                                            End If
                                        Next
                                    End If
                                Next
                            End If
                        End If

                        Exit For
                    End If
                Next

                If (_clickedRow IsNot Nothing) AndAlso (_clickedRow.ParentRow IsNot Nothing) Then
                    .ActiveRowScrollRegion.ScrollRowIntoView(_clickedRow)

                    _clickedRow = Nothing
                End If
            End If

            .EndUpdate()
        End With
    End Sub

    Private Sub ZoomDrawing()
        If Me.vDraw.ActiveDocument IsNot Nothing Then
            Me.vDraw.ActiveDocument.ViewCenter = New gPoint(0, 0)

            If (Me.vDraw.ActiveDocument.ActiveLayOut.Entities.Count <> 0) AndAlso (TypeOf Me.vDraw.ActiveDocument.ActiveLayOut.Entities(0) Is vdCircle) Then
                Me.vDraw.ActiveDocument.ViewSize = DirectCast(Me.vDraw.ActiveDocument.ActiveLayOut.Entities(0), vdCircle).Radius * 4
            ElseIf (Me.vDraw.ActiveDocument.ActiveLayOut.Entities.Count <> 0) AndAlso (Not TypeOf Me.vDraw.ActiveDocument.ActiveLayOut.Entities(0) Is vdCircle) Then
                Me.vDraw.ActiveDocument.ZoomExtents()
            Else
                Me.vDraw.ActiveDocument.ViewSize = 10
            End If

            Me.vDraw.ActiveDocument.Invalidate()
        End If
    End Sub

    Private Sub BundleCrossSectionForm_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        If (_messageBoxDisplayed) Then
            _messageBoxDisplayed = False
            Return
        End If

        Dim calcDiameter As Boolean = True

        If (_calculatedSegmentDiameters.ContainsKey(_segment.SystemId)) Then
            For Each moduleConfigWithCircle As KeyValuePair(Of HarnessModuleConfiguration, PackagingCircle) In _calculatedSegmentDiameters(_segment.SystemId)
                calcDiameter = False
            Next

            If (calcDiameter) Then
                If (_initializing) Then
                    _calculatedSegmentDiameters(_segment.SystemId) = GetModuleConfigurations()
                Else
                    _messageBoxDisplayed = True

                    If (MessageBoxEx.ShowInfo(String.Format(BundleCrossSectionFormStrings.RecalculateNecessary_Msg, vbCrLf), MessageBoxButtons.YesNo) = System.Windows.Forms.DialogResult.Yes) Then
                        InitializeGridData()

                        _calculatedSegmentDiameters(_segment.SystemId) = GetModuleConfigurations()
                    Else
                        calcDiameter = False
                    End If
                End If
            ElseIf (_initializing) Then
                If (_selectedModuleConfig Is Nothing) Then
                    For Each moduleConfig As HarnessModuleConfiguration In _moduleConfigurationsWithCablesWires.Keys
                        If (moduleConfig.IsActive) Then
                            _selectedModuleConfig = moduleConfig

                            Exit For
                        End If
                    Next
                End If

                'HINT: temporary HACK- there is no update of the grids and some gui elements if the the values come from the cache and are not recalculted by the worker
                'we should rework the whole structure one day
                _workingModuleConfig = _selectedModuleConfig
                UpdateCalculatedValuesInGrid()
                _workingModuleConfig = Nothing
                DrawBundlePicture()
                ToggleControlAccessibility(True)

            End If
        Else
            _calculatedSegmentDiameters.Add(_segment.SystemId, GetModuleConfigurations)
        End If

        If (_initializing) Then
            CheckForCablesWiresWithoutModuleAssignment()
        End If

        _initializing = False

        If (calcDiameter) Then CalculateDiameter()
    End Sub

    Private Sub BundleCrossSectionForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If (Me.bwCSACalculation.IsBusy) OrElse (_exportRunning) Then
            Me.bwCSACalculation.CancelAsync()

            e.Cancel = True
        ElseIf (Me.uneCstBundleInstallationAddOnTol.IsInEditMode) Then
            e.Cancel = True
        ElseIf (Me.uneCstBundleProvisioningAddOnTol.IsInEditMode) Then
            e.Cancel = True
        Else
            Dim userModificationExists As Boolean = False
            For Each row As UltraGridRow In Me.ugBundleCSAs.Rows
                If Not _isStandAlone AndAlso (row.ChildBands.Count > 0) AndAlso (row.ChildBands(0).Rows.Where(Function(childRow) (CBool(childRow.Cells(ColumnKeys.TemporaryAdded.ToString).Value) = True)).Any() OrElse row.ChildBands(0).Rows.Where(Function(childRow) (CBool(childRow.Cells(ColumnKeys.TemporaryRemoved.ToString).Value) = True)).Any() OrElse row.ChildBands(0).Rows.Where(Function(childRow) (CBool(childRow.Cells(ColumnKeys.TemporaryModified.ToString).Value) = True)).Any) Then
                    userModificationExists = True

                    Exit For
                End If
            Next

            If (userModificationExists) Then
                _messageBoxDisplayed = True

                If MessageBoxEx.ShowQuestion(BundleCrossSectionFormStrings.AddedWiresWillBeRemoved_Msg) = System.Windows.Forms.DialogResult.No Then
                    e.Cancel = True
                Else
                    _calculatedSegmentDiameters(_segment.SystemId).Clear()
                End If
            End If
        End If
    End Sub

    Private Sub BundleCrossSectionForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If (e.KeyCode = Keys.Escape) Then
            If Not (ugBundleCSAs.ActiveCell IsNot Nothing AndAlso ugBundleCSAs.ActiveCell.IsInEditMode) Then
                Me.Close()
            End If
        End If
    End Sub

    Private Async Sub btnCalculateAll_Click(sender As Object, e As EventArgs) Handles btnCalculateAll.Click
        Await _lockCalculate.WaitAsync
        Try
            Dim cts As New CancellationTokenSource
            Dim moduleConfigs As New List(Of HarnessModuleConfiguration)
            Dim tasks As New List(Of Task)
            Dim token As CancellationToken = cts.Token

            Me.Cursor = Cursors.WaitCursor

            ToggleControlAccessibility(False)

            Me.vDraw.ActiveDocument.ActiveLayOut.Entities.RemoveAll()
            Me.vDraw.ActiveDocument.Invalidate()

            With Me.ugBundleCSAs
                .BeginUpdate()
                .EventManager.AllEventsEnabled = False

                For Each row As UltraGridRow In .Rows
                    tasks.Add(Task.Factory.StartNew(
                              Sub()
                                  Dim moduleConfig As HarnessModuleConfiguration = DirectCast(DirectCast(row.ListObject, UltraDataRow).Tag, HarnessModuleConfiguration)
                                  If (_calculatedSegmentDiameters(_segment.SystemId)(moduleConfig) Is Nothing) Then
                                      If (moduleConfig Is Nothing) Then
                                          For Each moduleConfigWithCircle As KeyValuePair(Of HarnessModuleConfiguration, PackagingCircle) In _calculatedSegmentDiameters(_segment.SystemId)
                                              If (moduleConfigWithCircle.Key.IsActive) Then
                                                  moduleConfig = moduleConfigWithCircle.Key

                                                  Exit For
                                              End If
                                          Next
                                      End If

                                      If (moduleConfig Is Nothing) Then
                                          cts.Cancel()
                                      Else
                                          For Each dataRow As UltraDataRow In Me.udsBundleCSAs.Rows
                                              If (dataRow.Tag.Equals(moduleConfig)) Then
                                                  _calculatedSegmentDiameters(_segment.SystemId)(moduleConfig) = GetBundleWithCableAndWireCircles(dataRow)
                                                  Exit For
                                              End If
                                          Next

                                          Dim segmentCircle As PackagingCircle = _calculatedSegmentDiameters(_segment.SystemId)(moduleConfig)

                                          Try
                                              _circlePackager = New CirclePackager()
                                              _circlePackager.Package(segmentCircle)
                                              If segmentCircle.Radius = 0 Then
                                                  segmentCircle.HasInvalidOutsideDiameter = True
                                              End If
                                          Catch ex As Exception
                                              segmentCircle.HasInvalidOutsideDiameter = True
                                          End Try

                                          moduleConfigs.Add(moduleConfig)

                                          token.ThrowIfCancellationRequested()
                                      End If
                                  End If
                              End Sub, token))
                Next
                If Not _isStandAlone Then
                    .Rows.CollapseAll(True)
                End If
                .EventManager.AllEventsEnabled = True
                .EndUpdate()
            End With

            Try
                Task.WaitAll(tasks.ToArray())
            Catch ex As AggregateException
                For Each ie As Exception In ex.InnerExceptions
                    If (TypeOf ie Is OperationCanceledException) Then
                        MessageBoxEx.ShowWarning(Me, BundleCrossSectionFormStrings.CalculationCancelled_Msg)

                        Exit For
                    Else
                        MessageBoxEx.ShowError(Me, String.Format(BundleCrossSectionFormStrings.UnexpectedExceptionOccurred_Msg, ie.GetType().Name, ie.Message))
                    End If
                Next ie
            Finally
                cts.Dispose()
            End Try

            For Each moduleConfig As HarnessModuleConfiguration In moduleConfigs
                _workingModuleConfig = moduleConfig
                UpdateCalculatedValuesInGrid()
                _workingModuleConfig = Nothing
            Next

            With Me.ugBundleCSAs
                .BeginUpdate()

                .DisplayLayout.Bands(0).SortedColumns.Clear()
                .DisplayLayout.Bands(0).SortedColumns.Add(.DisplayLayout.Bands(0).Columns(ColumnKeys.OutsideDiaCalc.ToString), True)

                For Each column As UltraGridColumn In .DisplayLayout.Bands(0).Columns
                    If (column.Key = ColumnKeys.OutsideDiaCalc.ToString) Then
                        column.SortIndicator = SortIndicator.Descending
                    Else
                        column.SortIndicator = SortIndicator.None
                    End If
                Next

                .ActiveRow = .DisplayLayout.Rows(0)
                .ActiveRowScrollRegion.ScrollRowIntoView(.DisplayLayout.Rows(0))
                .Selected.Rows.Clear()
                .Selected.Rows.Add(.DisplayLayout.Rows(0))

                .EndUpdate()
            End With

            DrawBundlePicture()
            ToggleControlAccessibility(True)

        Finally
            Me.Cursor = Cursors.Default
            Me.btnCalculateAll.Enabled = False
            _lockCalculate.Release()
        End Try
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        If (Me.bwCSACalculation.IsBusy) Then
            Me.bwCSACalculation.CancelAsync()
        ElseIf (_exportRunning) Then
            _exportCancelled = True
        End If
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub btnExportPicture_Click(sender As Object, e As EventArgs) Handles btnExportPicture.Click
        Using sfdExport As New SaveFileDialog
            With sfdExport
                .DefaultExt = KnownFile.DXF.Trim("."c)
                .FileName = String.Format("Raw_bundle_of_segment_{0}", Regex.Replace(_segment.Id, "\W", "_"))
                .Filter = "Autodesk DXF file (*.dxf)|*.dxf|JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|Bitmap (*.bmp)|*.bmp"
                .Title = BundleCrossSectionFormStrings.ExportBundlePicFile_Title

                If sfdExport.ShowDialog(Me) = DialogResult.OK Then
                    Try
                        If (VdOpenSave.SaveAs(Me.vDraw.ActiveDocument, sfdExport.FileName)) Then
                            MessageBoxEx.ShowInfo(BundleCrossSectionFormStrings.ExportBundlePicSuccess_Msg)
                        Else
                            MessageBoxEx.ShowError(BundleCrossSectionFormStrings.ExportProblemBundlePic_Msg)
                        End If
                    Catch ex As Exception
                        ex.ShowMessageBox(String.Format(BundleCrossSectionFormStrings.ExportErrorBundlePic_Msg, vbCrLf, ex.Message))
                    End Try
                End If
            End With
        End Using
    End Sub

    Private Sub btnExportStructure_Click(sender As Object, e As EventArgs) Handles btnExportStructure.Click
        Using sfdExcel As New SaveFileDialog
            With sfdExcel
                .DefaultExt = KnownFile.XLSX.Trim("."c)

                .FileName = String.Format("{0}{1}{2}-harness_{3}-segment_{4}.xlsx", Now.Year, Format(Now.Month, "00"), Format(Now.Day, "00"), _kblMapper.HarnessPartNumber, Regex.Replace(_segment.Id, "\W", "_"))
                .Filter = "Excel files (*.xlsx)|*.xlsx|Excel files (97-2003) (*.xls)|*.xls"
                .Title = BundleCrossSectionFormStrings.ExportStructureFile_Title

                If (sfdExcel.ShowDialog(Me) = DialogResult.OK) Then
                    Try
                        _exportRunning = True

                        Dim workbook As New Workbook

                        If (IO.Path.GetExtension(.FileName).ToLower = ".xlsx") Then
                            workbook.SetCurrentFormat(WorkbookFormat.Excel2007)
                        Else
                            workbook.SetCurrentFormat(WorkbookFormat.Excel97To2003)
                        End If

                        Dim worksheet As Worksheet = workbook.Worksheets.Add("Bundle structure")

                        With worksheet.Rows(0)
                            .CellFormat.Font.Bold = ExcelDefaultableBoolean.True
                            .Cells(0).Value = "Object ID"
                            .Cells(1).Value = "Radius"
                            .Cells(2).Value = "Center-X"
                            .Cells(3).Value = "Center-Y"
                            .Cells(4).Value = "Signal Name"
                        End With

                        If (_calculatedSegmentDiameters(_segment.SystemId).ContainsKey(_selectedModuleConfig)) AndAlso (_calculatedSegmentDiameters(_segment.SystemId)(_selectedModuleConfig) IsNot Nothing) AndAlso (_calculatedSegmentDiameters(_segment.SystemId)(_selectedModuleConfig).InnerCircles.Count <> 0) Then
                            Dim segmentCircle As PackagingCircle = _calculatedSegmentDiameters(_segment.SystemId)(_selectedModuleConfig)

                            ExportBundleStructure_Recursively(segmentCircle, worksheet, True)
                        End If

                        workbook.Save(.FileName)

                        If (MessageBoxEx.ShowQuestion(BundleCrossSectionFormStrings.ExportStructureSuccess_Msg) = System.Windows.Forms.DialogResult.Yes) Then
                            ProcessEx.Start(.FileName)
                        End If
                    Catch ex As Exception
                        ex.ShowMessageBox(String.Format(BundleCrossSectionFormStrings.ExportStructureError_Msg, vbCrLf, ex.Message))
                    Finally
                        _exportRunning = False
                    End Try
                End If
            End With
        End Using
    End Sub

    Private Sub btnExportTable_Click(sender As Object, e As EventArgs) Handles btnExportTable.Click
        Using sfdExcel As New SaveFileDialog
            With sfdExcel
                .DefaultExt = KnownFile.XLSX.Trim("."c)

                .FileName = String.Format("{0}{1}{2}_{3}_Raw_Bundles.xlsx", Now.Year, Format(Now.Month, "00"), Format(Now.Day, "00"), Regex.Replace(_segment.Id, "\W", "_"))
                .Filter = "Excel files (*.xlsx)|*.xlsx|Excel files (97-2003) (*.xls)|*.xls"
                .Title = BundleCrossSectionFormStrings.ExportExcelFile_Title

                If (sfdExcel.ShowDialog(Me) = DialogResult.OK) Then
                    Try
                        _exportFileName = .FileName
                        _exportRunning = True

                        Dim moduleConfigsForCalculation As New List(Of HarnessModuleConfiguration)

                        For Each moduleConfigWithCircle As KeyValuePair(Of HarnessModuleConfiguration, PackagingCircle) In _calculatedSegmentDiameters(_segment.SystemId)
                            If (moduleConfigWithCircle.Value Is Nothing) Then
                                moduleConfigsForCalculation.Add(moduleConfigWithCircle.Key)
                            End If
                        Next

                        If (moduleConfigsForCalculation.Count <> 0) Then
                            ToggleControlAccessibility(False)

                            Dim moduleConfigCounter As Integer = 1

                            For Each moduleConfig As HarnessModuleConfiguration In moduleConfigsForCalculation
                                Me.upbMain.Value = CInt((moduleConfigCounter * 100) / moduleConfigsForCalculation.Count)

                                Application.DoEvents()

                                _workingModuleConfig = moduleConfig

                                For Each row As UltraDataRow In Me.udsBundleCSAs.Rows
                                    If (row.Tag.Equals(_workingModuleConfig)) Then
                                        _calculatedSegmentDiameters(_segment.SystemId)(_workingModuleConfig) = GetBundleWithCableAndWireCircles(row)

                                        Exit For
                                    End If
                                Next

                                _circlePackager = New CirclePackager()
                                _circlePackager.Package(_calculatedSegmentDiameters(_segment.SystemId)(_workingModuleConfig))

                                UpdateCalculatedValuesInGrid()

                                moduleConfigCounter += 1

                                If (_exportCancelled) Then
                                    Exit For
                                End If
                            Next

                            _workingModuleConfig = Nothing

                            ToggleControlAccessibility(True)
                        End If

                        If (Not _exportCancelled) Then
                            Dim workbook As New Workbook

                            If (IO.Path.GetExtension(_exportFileName).ToLower = ".xlsx") Then
                                workbook.SetCurrentFormat(WorkbookFormat.Excel2007)
                            Else
                                workbook.SetCurrentFormat(WorkbookFormat.Excel97To2003)
                            End If

                            workbook.Worksheets.Add("Raw bundles on segment")

                            If (_bundleAddOnInstallationTolerance > 0 OrElse _bundleAddOnProvisioningTolerance > 0) Then
                                If (_diameterSettings.IsAddOnToleranceOnArea) Then
                                    workbook.Worksheets(0).Rows(0).Cells(0).Value = String.Format("Segment: '{0}'    Cross section area tolerance: {1}%", _segment.Id, CInt((_bundleAddOnInstallationTolerance + _bundleAddOnProvisioningTolerance) * 100))
                                Else
                                    workbook.Worksheets(0).Rows(0).Cells(0).Value = String.Format("Segment: '{0}'    Outside diameter tolerance: {1}%", _segment.Id, CInt((_bundleAddOnInstallationTolerance + _bundleAddOnProvisioningTolerance) * 100))
                                End If
                            Else
                                workbook.Worksheets(0).Rows(0).Cells(0).Value = String.Format("Segment: '{0}'", _segment.Id)
                            End If

                            Me.ugeeBundleCSAs.ExportAsync(Me.ugBundleCSAs, workbook, 1, 0)
                        Else
                            _exportCancelled = False
                        End If
                    Catch ex As Exception
                        ShowExportTableErrorMessage(ex.Message)
                        _exportRunning = False
                    End Try
                End If
            End With
        End Using
    End Sub

    Private Sub ShowExportTableErrorMessage(msg As String)
        MessageBoxEx.ShowError(String.Format(BundleCrossSectionFormStrings.ExportExcelError_Msg, vbCrLf, msg))
    End Sub

    Private Sub btnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click
        Dim printing As New Printing.VdPrinting(Me.vDraw.ActiveDocument)
        printing.DocumentName = Me.Text

        Using printForm As New Printing.PrintForm(printing, Nothing, False)
            printForm.ShowDialog(Me)
        End Using
    End Sub

    Private Sub bwCSACalculation_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles bwCSACalculation.DoWork
        _lockCalculate.Wait()
        Try
            _workingModuleConfig = _selectedModuleConfig

            If (_workingModuleConfig Is Nothing) Then
                For Each moduleConfigWithCircle As KeyValuePair(Of HarnessModuleConfiguration, PackagingCircle) In _calculatedSegmentDiameters(_segment.SystemId)
                    If (moduleConfigWithCircle.Key.IsActive) Then
                        _workingModuleConfig = moduleConfigWithCircle.Key

                        Exit For
                    End If
                Next
            End If

            If (_workingModuleConfig Is Nothing) Then
                e.Cancel = True
            Else
                For Each row As UltraDataRow In Me.udsBundleCSAs.Rows
                    If (row.Tag.Equals(_workingModuleConfig)) Then
                        _calculatedSegmentDiameters(_segment.SystemId)(_workingModuleConfig) = GetBundleWithCableAndWireCircles(row)
                        Exit For
                    End If
                Next

                Dim segmentCircle As PackagingCircle = _calculatedSegmentDiameters(_segment.SystemId)(_workingModuleConfig)
                Try
                    _circlePackager = New CirclePackager()
                    _circlePackager.Package(segmentCircle)

                    If segmentCircle.Radius = 0 Then
                        segmentCircle.HasInvalidOutsideDiameter = True
                    End If
                Catch ex As Exception
                    segmentCircle.HasInvalidOutsideDiameter = True
                End Try

                If (Me.bwCSACalculation.CancellationPending) Then
                    e.Cancel = True
                End If
            End If
        Finally
            _lockCalculate.Release()
        End Try
    End Sub

    Private Sub bwCSACalculation_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles bwCSACalculation.ProgressChanged
        Me.upbMain.Value = e.ProgressPercentage
    End Sub

    Private Sub bwCSACalculation_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles bwCSACalculation.RunWorkerCompleted
        If (Not e.Cancelled AndAlso e.Error Is Nothing) Then
            UpdateCalculatedValuesInGrid()
            DrawBundlePicture()
            ToggleControlAccessibility(True)

            _workingModuleConfig = Nothing
        Else
            _calculatedSegmentDiameters(_segment.SystemId)(_workingModuleConfig) = Nothing

            If (Not _selectedModuleConfig.Equals(_workingModuleConfig)) AndAlso (_calculatedSegmentDiameters(_segment.SystemId)(_selectedModuleConfig) Is Nothing) Then
                Me.bwCSACalculation.RunWorkerAsync()
            Else
                ToggleControlAccessibility(True)

                _workingModuleConfig = Nothing
            End If
        End If
    End Sub

    Private Sub circlePackager_Progress(percentage As Integer, ByRef cancel As Boolean) Handles _circlePackager.Progress
        If (Me.bwCSACalculation.CancellationPending) Then
            cancel = True
        ElseIf (Me.bwCSACalculation.IsBusy) Then
            Me.bwCSACalculation.ReportProgress(percentage)
        End If
    End Sub

    Private Sub uddPartNumbers_RowSelected(sender As Object, e As RowSelectedEventArgs) Handles uddPartNumbers.RowSelected
        _dropDownRow = e.Row
    End Sub

    Private Sub ugBundleCSAs_AfterCellListCloseUp(sender As Object, e As CellEventArgs) Handles ugBundleCSAs.AfterCellListCloseUp
        If (_dropDownRow Is Nothing) Then Exit Sub

        With DirectCast(e.Cell.Row.ListObject, UltraDataRow)
            If (.GetCellValue(ColumnKeys.PartNumber.ToString).ToString <> _dropDownRow.Cells(ColumnKeys.PartNumber.ToString).Value.ToString) Then
                .SetCellValue(ColumnKeys.PartNumber.ToString, _dropDownRow.Cells(ColumnKeys.PartNumber.ToString).Value.ToString)
                .SetCellValue(ColumnKeys.GeneralWire.ToString, _dropDownRow.Cells(ColumnKeys.GeneralWire.ToString).Value)


                If (_dropDownRow.Cells(ColumnKeys.PartNumber.ToString).Value.ToString = KblObjectType.Custom.ToLocalizedString) Then
                    .SetCellValue(ColumnKeys.OutsideDia.ToString, _dropDownRow.Cells(ColumnKeys.OutsideDia.ToString).Value)
                    .SetCellValue(ColumnKeys.OutsideDiaVal.ToString, 0)
                    .SetCellValue(ColumnKeys.DiaSource.ToString, BundleCrossSectionFormStrings.User_CellValue)
                    If (Not (CBool(.GetCellValue(ColumnKeys.TemporaryAdded.ToString)) OrElse CBool(.GetCellValue(ColumnKeys.TemporaryRemoved.ToString)))) Then
                        .SetCellValue(ColumnKeys.TemporaryModified.ToString, True)
                    End If

                Else
                    If (CBool(.GetCellValue(ColumnKeys.TemporaryModified.ToString))) Then
                        If (CStr(.GetCellValue(ColumnKeys.PartNumber.ToString)) = CStr(.GetCellValue(ColumnKeys.InitialPartnumber.ToString))) Then
                            .SetCellValue(ColumnKeys.TemporaryModified.ToString, False)

                        End If
                    Else
                        If (Not (CBool(.GetCellValue(ColumnKeys.TemporaryAdded.ToString)) OrElse CBool(.GetCellValue(ColumnKeys.TemporaryRemoved.ToString)))) Then
                            .SetCellValue(ColumnKeys.TemporaryModified.ToString, True)
                        End If

                    End If
                    GetOutsideDiameter(DirectCast(e.Cell.Row.ListObject, UltraDataRow), TryCast(_dropDownRow.Cells(ColumnKeys.GeneralWire.ToString).Value, General_wire))
                End If
                CalculateDiameter()
            End If
        End With

        _clickedRow = e.Cell.Row
    End Sub

    Private Sub ugBundleCSAs_AfterEnterEditMode(sender As Object, e As EventArgs) Handles ugBundleCSAs.AfterEnterEditMode
        If (Me.ugBundleCSAs.ActiveCell.Column.Key = ColumnKeys.OutsideDia.ToString) Then
            Me.ugBundleCSAs.ActiveCell.Value = Me.ugBundleCSAs.ActiveRow.Cells(ColumnKeys.OutsideDiaVal.ToString).Value
            Me.ugBundleCSAs.ActiveCell.SelectAll()

            Me.vDraw.ActiveDocument.EnableAutoFocus = False
        End If
    End Sub

    Private Sub ugBundleCSAs_AfterExitEditMode(sender As Object, e As EventArgs) Handles ugBundleCSAs.AfterExitEditMode
        If (Me.ugBundleCSAs.ActiveCell.Column.Key = ColumnKeys.OutsideDia.ToString) Then
            Me.ugBundleCSAs.ActiveRow.Cells(ColumnKeys.OutsideDiaVal.ToString).Value = Me.ugBundleCSAs.ActiveCell.Value
            Me.ugBundleCSAs.ActiveRow.Cells(ColumnKeys.OutsideDia.ToString).Value = String.Format("{0} mm", Me.ugBundleCSAs.ActiveRow.Cells(ColumnKeys.OutsideDiaVal.ToString).Value)

            Me.vDraw.ActiveDocument.EnableAutoFocus = True

            _clickedRow = Me.ugBundleCSAs.ActiveRow

            CalculateDiameter()
        End If
    End Sub

    Private Sub ugBundleCSAs_AfterSelectChange(sender As Object, e As AfterSelectChangeEventArgs) Handles ugBundleCSAs.AfterSelectChange
        RemoveHighlightOfCablesOrWiresInDrawing()

        If (Me.ugBundleCSAs.Selected.Rows.Count <> 0) Then
            Dim newSelectedModuleConfig As HarnessModuleConfiguration

            If (Me.ugBundleCSAs.Selected.Rows(0).ParentRow Is Nothing) Then
                newSelectedModuleConfig = DirectCast(DirectCast(Me.ugBundleCSAs.Selected.Rows(0).ListObject, UltraDataRow).Tag, HarnessModuleConfiguration)
            Else
                newSelectedModuleConfig = DirectCast(DirectCast(Me.ugBundleCSAs.Selected.Rows(0).ParentRow.ListObject, UltraDataRow).Tag, HarnessModuleConfiguration)
            End If

            If (_selectedModuleConfig Is Nothing) OrElse (Not _selectedModuleConfig.Equals(newSelectedModuleConfig)) Then
                _selectedModuleConfig = newSelectedModuleConfig

                If (Me.bwCSACalculation.IsBusy) AndAlso (_calculatedSegmentDiameters(_segment.SystemId)(_selectedModuleConfig) Is Nothing) Then
                    Me.bwCSACalculation.CancelAsync()
                Else
                    Me.vDraw.ActiveDocument.ActiveLayOut.Entities.RemoveAll()
                    Me.vDraw.ActiveDocument.Invalidate()

                    If (_calculatedSegmentDiameters(_segment.SystemId)(_selectedModuleConfig) Is Nothing) Then
                        CalculateDiameter()
                    Else
                        DrawBundlePicture()
                    End If
                End If
            End If

            If (Me.ugBundleCSAs.Selected.Rows(0).ParentRow IsNot Nothing) AndAlso (Not Me.bwCSACalculation.IsBusy) Then
                For Each selectedRow As UltraGridRow In Me.ugBundleCSAs.Selected.Rows
                    Dim id As String = selectedRow.GetCellValue(ColumnKeys.Id.ToString).ToString
                    Dim circle As PackagingCircle = Nothing

                    If _calculatedSegmentDiameters.ContainsKey(_segment.SystemId) AndAlso _calculatedSegmentDiameters(_segment.SystemId).ContainsKey(_selectedModuleConfig) Then
                        GetCircleById_Recursively(_calculatedSegmentDiameters(_segment.SystemId)(_selectedModuleConfig), id, circle)
                    End If
                    If (circle IsNot Nothing) Then
                        If (circle.InnerCircles.Count = 1) AndAlso (circle.Radius < circle.InnerCircles(0).Radius) Then circle = circle.InnerCircles(0)

                        For Each figure As vdFigure In Me.vDraw.ActiveDocument.ActiveLayOut.Entities
                            If (TypeOf figure Is vdCircle) AndAlso (DirectCast(figure, vdCircle).HatchProperties IsNot Nothing) AndAlso (figure.Label = circle.Id) Then
                                HighlightCableOrWireInDrawing(DirectCast(figure, vdCircle), True)

                                Exit For
                            End If
                        Next
                    End If
                Next
            End If
        End If
    End Sub

    Private Sub ugBundleCSAs_BeforeCellListDropDown(sender As Object, e As CancelableCellEventArgs) Handles ugBundleCSAs.BeforeCellListDropDown
        If (CBool(e.Cell.Row.Cells(ColumnKeys.TemporaryRemoved.ToString).Value)) Then e.Cancel = True
    End Sub

    Private Sub ugBundleCSAs_BeforeEnterEditMode(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles ugBundleCSAs.BeforeEnterEditMode
        If (Me.ugBundleCSAs.ActiveCell.Column.Key = ColumnKeys.OutsideDia.ToString AndAlso Not Me.ugBundleCSAs.ActiveRow.Cells(ColumnKeys.DiaSource.ToString).Value.ToString = BundleCrossSectionFormStrings.User_CellValue) Then
            e.Cancel = True 'OrElse (Me.ugBundleCSAs.ActiveRow.Cells(BundleCrossSectionFormStrings.Class_ColumnCaption)).Value.ToString <> BundleCrossSectionFormStrings.User_CellValue)
        End If
    End Sub

    Private Sub ugBundleCSAs_BeforeExitEditMode(sender As Object, e As BeforeExitEditModeEventArgs) Handles ugBundleCSAs.BeforeExitEditMode
        If (Me.ugBundleCSAs.ActiveCell.Column.Key = ColumnKeys.OutsideDia.ToString) Then
            If (Not IsNumeric(Me.ugBundleCSAs.ActiveCell.Text)) OrElse (CDbl(Me.ugBundleCSAs.ActiveCell.Text) < 0) OrElse (CDbl(Me.ugBundleCSAs.ActiveCell.Text) > 1000) Then
                MessageBoxEx.ShowError(BundleCrossSectionFormStrings.EnterValidOutsideDia_Msg)

                Me.ugBundleCSAs.ActiveCell.Value = 0

                e.Cancel = True
            End If
        End If
    End Sub

    Private Sub ugBundleCSAs_BeforeRowsDeleted(sender As Object, e As BeforeRowsDeletedEventArgs) Handles ugBundleCSAs.BeforeRowsDeleted
        e.DisplayPromptMsg = False
        If Not _deleteInitialRow Then
            If e.Rows IsNot Nothing Then
                If _isStandAlone Then
                    If (e.Rows.First.Band.Key = CABLE_WIRE_BANDHEADER) Then
                        If e.Rows.Length >= e.Rows.First.ParentRow.ChildBands(0).Rows.Count Then
                            'Msg that one user wire must be existent ?
                            e.Cancel = True
                        End If
                    Else
                        e.Cancel = True
                    End If
                Else
                    If (e.Rows.First.Band.Key <> CABLE_WIRE_BANDHEADER) OrElse (e.Rows.Any(Function(row) row.Cells(ColumnKeys.Class.ToString).Value.ToString <> BundleCrossSectionFormStrings.User_CellValue)) Then
                        e.Cancel = True
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub ugBundleCSAs_AfterRowsDeleted(sender As Object, e As EventArgs) Handles ugBundleCSAs.AfterRowsDeleted
        If _deleteInitialRow Then
            Return
        End If
        CalculateDiameter()
    End Sub

    Private Sub ugBundleCSAs_BeforeSelectChange(sender As Object, e As BeforeSelectChangeEventArgs) Handles ugBundleCSAs.BeforeSelectChange
        If (Me.ugBundleCSAs.Selected.Rows.Count <> 0) Then
            If (Me.ugBundleCSAs.Selected.Rows(0).ParentRow Is Nothing) Then
                _selectedModuleConfig = DirectCast(DirectCast(Me.ugBundleCSAs.Selected.Rows(0).ListObject, UltraDataRow).Tag, HarnessModuleConfiguration)
            Else
                _selectedModuleConfig = DirectCast(DirectCast(Me.ugBundleCSAs.Selected.Rows(0).ParentRow.ListObject, UltraDataRow).Tag, HarnessModuleConfiguration)
            End If
        End If
    End Sub

    Private Sub ugBundleCSAs_CellDataError(sender As Object, e As CellDataErrorEventArgs) Handles ugBundleCSAs.CellDataError
        e.RaiseErrorEvent = False
        e.RestoreOriginalValue = True
    End Sub

    Private Sub ugBundleCSAs_DragDrop(sender As Object, e As DragEventArgs) Handles ugBundleCSAs.DragDrop
        If (_hoveredRow IsNot Nothing) Then
            Dim draggedObjects As List(Of Object) = TryCast(e.Data.GetData(GetType(List(Of Object))), List(Of Object))
            If (draggedObjects IsNot Nothing) AndAlso (draggedObjects.Count > 1) Then
                draggedObjects.RemoveAt(0)

                With Me.ugBundleCSAs
                    .BeginUpdate()

                    _droppedRows = New List(Of UltraGridRow)

                    For Each cableWireRow As UltraDataRow In draggedObjects
                        .ActiveRow = _hoveredRow

                        AddDraggedCableWire(cableWireRow, _hoveredRow.Band, _hoveredRow.ParentCollection, False)
                    Next

                    .EndUpdate()

                    .ActiveRow = _droppedRows.LastOrDefault
                    .Selected.Rows.Clear()

                    For Each row As UltraGridRow In _droppedRows
                        .Selected.Rows.Add(row)
                    Next

                    If (_droppedRows.Count <> 0) Then
                        .ActiveRowScrollRegion.ScrollRowIntoView(_droppedRows.LastOrDefault)
                    End If

                    _droppedRows = Nothing
                End With

                CalculateDiameter()
            End If

            _clickedRow = Nothing
            _hoveredRow = Nothing
        End If
    End Sub

    Private Sub ugBundleCSAs_DragOver(sender As Object, e As DragEventArgs) Handles ugBundleCSAs.DragOver
        Dim draggedObjects As List(Of Object) = TryCast(e.Data.GetData(GetType(List(Of Object))), List(Of Object))

        _hoveredRow = Nothing

        e.Effect = DragDropEffects.None

        If (draggedObjects IsNot Nothing) AndAlso (draggedObjects.Count <> 0) AndAlso (TypeOf draggedObjects.First Is Segment) AndAlso (DirectCast(draggedObjects.First, Segment).Id <> _segment.Id) Then
            Dim element As Infragistics.Win.UIElement = Me.ugBundleCSAs.DisplayLayout.UIElement.ElementFromPoint(Me.ugBundleCSAs.PointToClient(New Drawing.Point(e.X, e.Y)))
            If (element IsNot Nothing) Then
                Dim row As UltraGridRow = TryCast(element.GetContext(GetType(UltraGridRow)), UltraGridRow)
                If (row IsNot Nothing) AndAlso (row.Band.Key = CABLE_WIRE_BANDHEADER) Then
                    _hoveredRow = row

                    e.Effect = DragDropEffects.Move
                End If
            End If
        End If
    End Sub

    Private Sub ugBundleCSAs_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugBundleCSAs.InitializeLayout
        Me.ugBundleCSAs.BeginUpdate()

        With e.Layout
            .AutoFitStyle = AutoFitStyle.ResizeAllColumns
            .CaptionVisible = Infragistics.Win.DefaultableBoolean.False
            .GroupByBox.Hidden = True
            .LoadStyle = LoadStyle.LoadOnDemand

            With .Override
                .AllowColMoving = AllowColMoving.NotAllowed
                .ButtonStyle = Infragistics.Win.UIElementButtonStyle.Button3D
                .CellClickAction = CellClickAction.RowSelect
                .RowSelectors = Infragistics.Win.DefaultableBoolean.False
                .SelectTypeRow = SelectType.Extended

            End With

            With .Bands(MODUL_CONFIG_BANDHEADER)
                With .Columns(ColumnKeys.ModulConfiguration.ToString)
                    .CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center
                    .CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle
                    .Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center
                    .Header.Appearance.TextVAlign = Infragistics.Win.VAlign.Middle
                    .Width = 150
                    .SortIndicator = SortIndicator.Ascending
                    .Header.Caption = BundleCrossSectionFormStrings.ModConf_ColumnCaption
                End With

                With .Columns(ColumnKeys.OutsideDiaCalc.ToString)
                    .CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center
                    .CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle
                    .Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center
                    .Header.Appearance.TextVAlign = Infragistics.Win.VAlign.Middle
                    .Header.ToolTipText = BundleCrossSectionFormStrings.ValCalcByFormula_Tooltip
                    .SortComparer = New NumericStringSortComparer
                    .Header.Caption = BundleCrossSectionFormStrings.OutsideDiaCalc_ColumnCaption
                End With

                With .Columns(ColumnKeys.OutsideDiaValCalc.ToString)
                    .Hidden = True
                End With

                With .Columns(ColumnKeys.OutsideDiaPack.ToString)
                    .CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center
                    .CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle
                    .Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center
                    .Header.Appearance.TextVAlign = Infragistics.Win.VAlign.Middle
                    .Header.ToolTipText = BundleCrossSectionFormStrings.ValDetByPack_Tooltip
                    .SortComparer = New NumericStringSortComparer
                    .Header.Caption = BundleCrossSectionFormStrings.OutsideDiaPack_ColumnCaption
                End With
                With .Columns(ColumnKeys.OutsideDiaValPack.ToString)
                    .Hidden = True
                End With
                With .Columns(ColumnKeys.OutsideDiaKBL.ToString)
                    .CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center
                    .CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle
                    .Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center
                    .Header.Appearance.TextVAlign = Infragistics.Win.VAlign.Middle
                    .Header.ToolTipText = BundleCrossSectionFormStrings.ValFromKBL_Tooltip
                    .SortComparer = New NumericStringSortComparer
                    .Header.Caption = BundleCrossSectionFormStrings.OutsideDiaKBL_ColumnCaption
                End With
                With .Columns(ColumnKeys.OutsideDiaValKBL.ToString)
                    .Hidden = True
                End With
                With .Columns(ColumnKeys.CSACalc.ToString)
                    .CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center
                    .CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle
                    .Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center
                    .Header.Appearance.TextVAlign = Infragistics.Win.VAlign.Middle
                    .Header.ToolTipText = BundleCrossSectionFormStrings.ValCalcByFormula_Tooltip
                    .SortComparer = New NumericStringSortComparer
                    .Header.Caption = BundleCrossSectionFormStrings.CSACalc_ColumnCaption
                End With
                With .Columns(ColumnKeys.CSAValCalc.ToString)
                    .Hidden = True
                End With
                With .Columns(ColumnKeys.CSAPack.ToString)
                    .CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center
                    .CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle
                    .Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center
                    .Header.Appearance.TextVAlign = Infragistics.Win.VAlign.Middle
                    .Header.ToolTipText = BundleCrossSectionFormStrings.ValDetByPack_Tooltip
                    .SortComparer = New NumericStringSortComparer
                    .Header.Caption = BundleCrossSectionFormStrings.CSAPack_ColumnCaption
                End With
                With .Columns(ColumnKeys.CSAValPack.ToString)
                    .Hidden = True
                End With
                With .Columns(ColumnKeys.CSAKBL.ToString)
                    .CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center
                    .CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle
                    .Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center
                    .Header.Appearance.TextVAlign = Infragistics.Win.VAlign.Middle
                    .Header.ToolTipText = BundleCrossSectionFormStrings.ValFromKBL_Tooltip
                    .SortComparer = New NumericStringSortComparer
                    .Header.Caption = BundleCrossSectionFormStrings.CSAKBL_ColumnCaption
                End With

                With .Columns(ColumnKeys.CSAValKBL.ToString)
                    .Hidden = True
                End With

            End With


            With .Bands(CABLE_WIRE_BANDHEADER)
                .ColHeadersVisible = True
                With .Columns(ColumnKeys.Class.ToString)
                    .CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center
                    .CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle
                    .Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center
                    .Header.Appearance.TextVAlign = Infragistics.Win.VAlign.Middle
                    .Width = 75
                    .Header.Caption = BundleCrossSectionFormStrings.Class_ColumnCaption
                End With

                With .Columns(ColumnKeys.CabWirNum.ToString)
                    .CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center
                    .CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle
                    .Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center
                    .Header.Appearance.TextVAlign = Infragistics.Win.VAlign.Middle
                    .Width = 150
                    .SortIndicator = SortIndicator.Ascending
                    .Header.Caption = BundleCrossSectionFormStrings.CabWirNum_ColumnCaption
                End With

                With .Columns(ColumnKeys.PartNumber.ToString)
                    .CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center
                    .CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle
                    .Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center
                    .Header.Appearance.TextVAlign = Infragistics.Win.VAlign.Middle
                    .Style = ColumnStyle.DropDownList
                    .Header.Caption = BundleCrossSectionFormStrings.PartNumber_ColumnCaption
                End With
                With .Columns(ColumnKeys.WiringGroup.ToString)
                    .CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center
                    .CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle
                    .Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center
                    .Header.Appearance.TextVAlign = Infragistics.Win.VAlign.Middle
                    .Header.Caption = BundleCrossSectionFormStrings.WiringGroup_ColumnCaption
                End With
                With .Columns(ColumnKeys.NetName.ToString)
                    .CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center
                    .CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle
                    .Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center
                    .Header.Appearance.TextVAlign = Infragistics.Win.VAlign.Middle
                    .SortComparer = New NumericStringSortComparer
                    .Header.Caption = BundleCrossSectionFormStrings.NetName_ColumnCaption
                End With
                With .Columns(ColumnKeys.HarnessModules.ToString)
                    .CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center
                    .CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle
                    .Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center
                    .Header.Appearance.TextVAlign = Infragistics.Win.VAlign.Middle
                    .Header.Caption = KblObjectType.Harness_module.ToLocalizedString
                End With
                With .Columns(ColumnKeys.OutsideDia.ToString)
                    .CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center
                    .CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle
                    .Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center
                    .Header.Appearance.TextVAlign = Infragistics.Win.VAlign.Middle
                    .CellClickAction = CellClickAction.Edit
                    .SortComparer = New NumericStringSortComparer
                    .Header.Caption = BundleCrossSectionFormStrings.OutsideDia_ColumnCaption
                End With

                With .Columns(ColumnKeys.OutsideDiaVal.ToString)
                    .Hidden = True
                    .SortComparer = New NumericStringSortComparer
                End With

                With .Columns(ColumnKeys.DiaSource.ToString)
                    .CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center
                    .CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle
                    .Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center
                    .Header.Appearance.TextVAlign = Infragistics.Win.VAlign.Middle
                    .Header.Caption = BundleCrossSectionFormStrings.DiaSource_ColumnCaption
                End With

                With .Columns(ColumnKeys.TemporaryAdded.ToString)
                    .Hidden = True
                End With
                With .Columns(ColumnKeys.TemporaryRemoved.ToString)
                    .Hidden = True
                End With
                With .Columns(ColumnKeys.TemporaryModified.ToString)
                    .Hidden = True
                End With
                With .Columns(ColumnKeys.InitialPartnumber.ToString)
                    .Hidden = True
                End With
                With .Columns(ColumnKeys.InitialGeneralWire.ToString)
                    .Hidden = True
                End With
                With .Columns(ColumnKeys.GeneralWire.ToString)
                    .Hidden = True
                End With
                With .Columns(ColumnKeys.Id.ToString)
                    .Hidden = True
                End With

            End With
        End With

        Me.ugBundleCSAs.EndUpdate()
    End Sub

    Private Sub ugBundleCSAs_InitializeRow(sender As Object, e As InitializeRowEventArgs) Handles ugBundleCSAs.InitializeRow
        If (Not _exportRunning) Then
            For Each moduleConfig As HarnessModuleConfiguration In _moduleConfigurationsWithCablesWires.Keys
                If (moduleConfig.IsActive) AndAlso (DirectCast(e.Row.ListObject, UltraDataRow).Tag IsNot Nothing) AndAlso (DirectCast(e.Row.ListObject, UltraDataRow).Tag.Equals(moduleConfig)) Then
                    If (Me.ugBundleCSAs.Selected.Rows.Count = 0) Then
                        Me.ugBundleCSAs.EventManager.AllEventsEnabled = False

                        e.Row.Selected = True
                        e.Row.ExpandAll()

                        Me.ugBundleCSAs.ActiveRowScrollRegion.ScrollRowIntoView(e.Row)
                        Me.ugBundleCSAs.EventManager.AllEventsEnabled = True
                    End If

                    _selectedModuleConfig = moduleConfig

                    Exit For
                End If
            Next

            If (e.Row.HasChild) Then
                For Each childRow As UltraGridRow In e.Row.ChildBands(0).Rows
                    If childRow.Cells(0).Value.ToString = DummyWireId.ToString Then
                        _clickedRow = childRow
                        AddUserWire()
                        _deleteInitialRow = True
                        childRow.Delete()
                        _deleteInitialRow = False
                        _clickedRow = Nothing
                        _isStandAlone = True
                        Me.Text = String.Format(BundleCrossSectionFormStrings.Caption_Standalone)
                    End If

                    For Each wiringGroup As Wiring_group In _kblMapper.GetWiringGroups.Where(Function(wirGroup) wirGroup.Id = childRow.Cells(ColumnKeys.WiringGroup.ToString).Value.ToString)
                        If (wiringGroup.GetConsistencyState(_kblMapper) <> [Lib].Schema.Kbl.WiringGroupConsistencyState.Valid) Then
                            childRow.Cells(ColumnKeys.WiringGroup.ToString).Appearance.Image = My.Resources.MismatchingConfig.ToBitmap

                            Select Case wiringGroup.GetConsistencyState(_kblMapper)
                                Case [Lib].Schema.Kbl.WiringGroupConsistencyState.HasSingleCoreOrWireAssigned
                                    childRow.Cells(ColumnKeys.WiringGroup.ToString).ToolTipText = BundleCrossSectionFormStrings.WiringGroup_Tooltip1
                                Case [Lib].Schema.Kbl.WiringGroupConsistencyState.HasCoresAndWiresAssigned
                                    childRow.Cells(ColumnKeys.WiringGroup.ToString).ToolTipText = BundleCrossSectionFormStrings.WiringGroup_Tooltip2
                                Case [Lib].Schema.Kbl.WiringGroupConsistencyState.HasCoresOfDifferentCablesAssigned
                                    childRow.Cells(ColumnKeys.WiringGroup.ToString).ToolTipText = BundleCrossSectionFormStrings.WiringGroup_Tooltip3
                            End Select
                        End If
                    Next
                Next
            End If

            If (e.Row.Band.Key = CABLE_WIRE_BANDHEADER) Then
                If (CBool(e.Row.Cells(ColumnKeys.TemporaryAdded.ToString).Value) OrElse CBool(e.Row.Cells(ColumnKeys.TemporaryModified.ToString).Value)) Then
                    e.Row.Appearance.BackColor = Color.FromArgb(25, Color.Red)
                Else
                    e.Row.Appearance.BackColor = Color.White
                End If
                If (CBool(e.Row.Cells(ColumnKeys.TemporaryModified.ToString).Value)) Then
                    e.Row.Cells(ColumnKeys.PartNumber.ToString).Appearance.FontData.Italic = Infragistics.Win.DefaultableBoolean.True
                Else
                    e.Row.Cells(ColumnKeys.PartNumber.ToString).Appearance.FontData.Italic = Infragistics.Win.DefaultableBoolean.False
                End If
                If (CBool(e.Row.Cells(ColumnKeys.TemporaryRemoved.ToString).Value)) Then
                    e.Row.Appearance.FontData.Strikeout = Infragistics.Win.DefaultableBoolean.True
                Else
                    e.Row.Appearance.FontData.Strikeout = Infragistics.Win.DefaultableBoolean.False
                End If
            End If
        End If
    End Sub

    Private Sub ugBundleCSAs_MouseClick(sender As Object, e As MouseEventArgs) Handles ugBundleCSAs.MouseClick
        If (e.Button = System.Windows.Forms.MouseButtons.Right) Then
            Dim element As Infragistics.Win.UIElement = Me.ugBundleCSAs.DisplayLayout.UIElement.LastElementEntered
            Dim header As ColumnHeader = TryCast(element.GetContext(GetType(ColumnHeader)), ColumnHeader)
            Dim row As UltraGridRow = TryCast(element.GetContext(GetType(UltraGridRow)), UltraGridRow)

            If (header IsNot Nothing) AndAlso (header.Band.ParentBand Is Nothing) Then
                With _contextMenu
                    .Tools(ContextMenuToolKey.AddCableWire.ToString).SharedProps.Visible = False
                    .Tools(ContextMenuToolKey.AddUserWire.ToString).SharedProps.Visible = False
                    .Tools(ContextMenuToolKey.CloneAsUserWires.ToString).SharedProps.Visible = False
                    .Tools(ContextMenuToolKey.CollapseAll.ToString).SharedProps.Visible = True
                    .Tools(ContextMenuToolKey.CopyUserWires.ToString).SharedProps.Visible = False
                    .Tools(ContextMenuToolKey.CopySelectedUserWires.ToString).SharedProps.Visible = False
                    .Tools(ContextMenuToolKey.ExpandAll.ToString).SharedProps.Visible = True
                    .Tools(ContextMenuToolKey.PasteUserWires.ToString).SharedProps.Visible = False
                    .Tools(ContextMenuToolKey.RemoveCableWireTemporarily.ToString).SharedProps.Visible = False
                    .Tools(ContextMenuToolKey.RemoveUserWire.ToString).SharedProps.Visible = False
                    .Tools(ContextMenuToolKey.BulkChangePartnumber.ToString).SharedProps.Visible = False
                    .Tools(ContextMenuToolKey.ResetPartnumberModification.ToString).SharedProps.Visible = False

                    .ShowPopup()
                End With
            ElseIf (header Is Nothing) AndAlso (row IsNot Nothing) AndAlso (row.Band.Key = CABLE_WIRE_BANDHEADER) Then
                _clickedRow = row

                Dim nofRemovedWires As Integer = 0
                Dim nofAddedWires As Integer = 0
                Dim nofModifiedWires As Integer = 0
                Dim nofSelectedWires As Integer = 0
                If (Me.ugBundleCSAs.Selected.Rows.Count > 0) Then
                    For Each selectedRow As UltraGridRow In Me.ugBundleCSAs.Selected.Rows
                        With selectedRow
                            If (.Band.Key = CABLE_WIRE_BANDHEADER) Then
                                nofSelectedWires += 1
                                If (CBool(.Cells(ColumnKeys.TemporaryRemoved.ToString).Value)) Then
                                    nofRemovedWires += 1
                                End If
                                If (CBool(.Cells(ColumnKeys.TemporaryAdded.ToString).Value)) Then
                                    nofAddedWires += 1
                                End If
                                If (CBool(.Cells(ColumnKeys.TemporaryModified.ToString).Value)) Then
                                    nofModifiedWires += 1
                                End If
                            End If
                        End With
                    Next

                    With _contextMenu
                        .Tools(ContextMenuToolKey.AddCableWire.ToString).SharedProps.Visible = CBool(nofRemovedWires = nofSelectedWires AndAlso nofSelectedWires > 0)
                        .Tools(ContextMenuToolKey.CloneAsUserWires.ToString).SharedProps.Visible = CBool(nofRemovedWires = 0 AndAlso nofAddedWires = 0 AndAlso nofSelectedWires > 0)
                        .Tools(ContextMenuToolKey.CopyUserWires.ToString).SharedProps.Visible = CBool(nofAddedWires > 0)
                        .Tools(ContextMenuToolKey.CopySelectedUserWires.ToString).SharedProps.Visible = CBool(nofAddedWires > 0 AndAlso nofSelectedWires > 0)
                        .Tools(ContextMenuToolKey.PasteUserWires.ToString).SharedProps.Visible = CBool(_copiedUserWires.Count <> 0)
                        .Tools(ContextMenuToolKey.RemoveCableWireTemporarily.ToString).SharedProps.Visible = CBool(nofAddedWires = 0 AndAlso nofRemovedWires = 0 AndAlso nofSelectedWires > 0)

                        If _isStandAlone Then
                            .Tools(ContextMenuToolKey.RemoveUserWire.ToString).SharedProps.Visible = CBool(nofAddedWires = nofSelectedWires AndAlso nofSelectedWires > 0 AndAlso nofSelectedWires < row.ParentRow.ChildBands(0).Rows.Count)
                        Else
                            .Tools(ContextMenuToolKey.RemoveUserWire.ToString).SharedProps.Visible = CBool(nofAddedWires = nofSelectedWires AndAlso nofSelectedWires > 0)
                        End If

                        .Tools(ContextMenuToolKey.ResetPartnumberModification.ToString).SharedProps.Visible = CBool(nofModifiedWires = nofSelectedWires AndAlso nofSelectedWires > 0)
                        .Tools(ContextMenuToolKey.BulkChangePartnumber.ToString).SharedProps.Visible = CBool(nofRemovedWires = 0 AndAlso nofAddedWires = 0 AndAlso nofSelectedWires > 1)

                    End With
                Else
                    With _contextMenu
                        .Tools(ContextMenuToolKey.AddCableWire.ToString).SharedProps.Visible = False
                        .Tools(ContextMenuToolKey.CloneAsUserWires.ToString).SharedProps.Visible = False
                        .Tools(ContextMenuToolKey.CopyUserWires.ToString).SharedProps.Visible = False
                        .Tools(ContextMenuToolKey.CopySelectedUserWires.ToString).SharedProps.Visible = False
                        .Tools(ContextMenuToolKey.PasteUserWires.ToString).SharedProps.Visible = False
                        .Tools(ContextMenuToolKey.RemoveCableWireTemporarily.ToString).SharedProps.Visible = False
                        .Tools(ContextMenuToolKey.RemoveUserWire.ToString).SharedProps.Visible = False
                        .Tools(ContextMenuToolKey.ResetPartnumberModification.ToString).SharedProps.Visible = False
                        .Tools(ContextMenuToolKey.BulkChangePartnumber.ToString).SharedProps.Visible = False
                    End With
                End If

                With _contextMenu
                    .Tools(ContextMenuToolKey.AddUserWire.ToString).SharedProps.Visible = True
                    .Tools(ContextMenuToolKey.CollapseAll.ToString).SharedProps.Visible = False
                    .Tools(ContextMenuToolKey.ExpandAll.ToString).SharedProps.Visible = False

                    .ShowPopup()
                End With
            Else

            End If
        End If
    End Sub

    Private Sub ugBundleCSAs_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles ugBundleCSAs.MouseDoubleClick
        If (e.Button = System.Windows.Forms.MouseButtons.Left) Then
            For Each selectedRow As UltraGridRow In Me.ugBundleCSAs.Selected.Rows
                If (TypeOf DirectCast(selectedRow.ListObject, UltraDataRow).Tag Is Special_wire_occurrence) Then
                    Dim objectIds As New List(Of String)
                    objectIds.Add(DirectCast(DirectCast(selectedRow.ListObject, UltraDataRow).Tag, Special_wire_occurrence).SystemId)

                    RaiseEvent BundleGridSelectionChanged(Me, New InformationHubEventArgs(_kblMapper.Id, objectIds, KblObjectType.Special_wire_occurrence))
                ElseIf (TypeOf DirectCast(selectedRow.ListObject, UltraDataRow).Tag Is Wire_occurrence) Then
                    Dim objectIds As New List(Of String)
                    objectIds.Add(DirectCast(DirectCast(selectedRow.ListObject, UltraDataRow).Tag, Wire_occurrence).SystemId)

                    RaiseEvent BundleGridSelectionChanged(Me, New InformationHubEventArgs(_kblMapper.Id, objectIds, KblObjectType.Wire_occurrence))
                End If
            Next
        End If
    End Sub

    Private Sub ugBundleCSAs_QueryContinueDrag(sender As Object, e As QueryContinueDragEventArgs) Handles ugBundleCSAs.QueryContinueDrag
        If (e.EscapePressed) Then
            e.Action = DragAction.Cancel
        End If
    End Sub

    Private Sub ugBundleCSAs_SelectionDrag(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles ugBundleCSAs.SelectionDrag
        Dim draggedObjects As New List(Of Object)
        draggedObjects.Add(_segment)

        For Each selectedRow As UltraGridRow In Me.ugBundleCSAs.Selected.Rows
            If (selectedRow.Band.Key <> CABLE_WIRE_BANDHEADER) Then
                Return
            End If

            draggedObjects.Add(selectedRow.ListObject)
        Next

        Me.ugBundleCSAs.DoDragDrop(draggedObjects, DragDropEffects.Move)
    End Sub

    Private Sub ugeeBundleCSAs_ExportEnded(sender As Object, e As ExcelExport.ExportEndedEventArgs) Handles ugeeBundleCSAs.ExportEnded
        _exportRunning = False

        If Not e.Canceled Then
            e.Workbook.Worksheets(0).Rows(0).Cells(0).CellFormat.Font.Bold = ExcelDefaultableBoolean.True
            Try
                e.Workbook.Save(_exportFileName)
            Catch ex As Exception
                ShowExportTableErrorMessage(ex.Message)
                Return
            End Try

            If (MessageBoxEx.ShowQuestion(BundleCrossSectionFormStrings.ExportExcelSuccess_Msg) = System.Windows.Forms.DialogResult.Yes) Then
                ProcessEx.Start(_exportFileName)
            End If
        End If
    End Sub

    Private Sub ugeeBundleCSAs_HeaderRowExporting(sender As Object, e As ExcelExport.HeaderRowExportingEventArgs) Handles ugeeBundleCSAs.HeaderRowExporting
        If (e.CurrentRowIndex > 1) AndAlso (e.CurrentOutlineLevel = 0) Then
            e.Cancel = True
        End If
    End Sub

    Private Sub uckSimpleBundleView_CheckedChanged(sender As Object, e As EventArgs) Handles uckSimpleBundleView.CheckedChanged
        ShowOrHideColorInformation(Me.uckSimpleBundleView.Checked)
    End Sub

    Private Sub uneCstBundleInstallationAddOnTol_BeforeEnterEditMode(sender As Object, e As CancelEventArgs) Handles uneCstBundleInstallationAddOnTol.BeforeEnterEditMode
        _prevBundleAddOnInstallationTolerance = CDbl(uneCstBundleInstallationAddOnTol.Value)
    End Sub

    Private Sub uneCstBundleProvisioningAddOnTol_BeforeEnterEditMode(sender As Object, e As CancelEventArgs) Handles uneCstBundleProvisioningAddOnTol.BeforeEnterEditMode
        _prevBundleAddOnProvisioningTolerance = CDbl(uneCstBundleProvisioningAddOnTol.Value)
    End Sub

    Private Sub uneCstBundleInstallationAddOnTol_BeforeExitEditMode(sender As Object, e As Infragistics.Win.BeforeExitEditModeEventArgs) Handles uneCstBundleInstallationAddOnTol.BeforeExitEditMode
        If e.ApplyChanges Then
            _bundleAddOnInstallationTolerance = CDbl(Me.uneCstBundleInstallationAddOnTol.Value)
            If (_bundleAddOnInstallationTolerance <> _prevBundleAddOnInstallationTolerance) Then
                _prevBundleAddOnInstallationTolerance = _bundleAddOnInstallationTolerance
                CalculateDiameter()
            End If

        Else
            uneCstBundleInstallationAddOnTol.Value = _prevBundleAddOnInstallationTolerance
        End If
    End Sub

    Private Sub uneCstBundleProvisioningAddOnTol_BeforeExitEditMode(sender As Object, e As Infragistics.Win.BeforeExitEditModeEventArgs) Handles uneCstBundleProvisioningAddOnTol.BeforeExitEditMode
        If e.ApplyChanges Then
            _bundleAddOnProvisioningTolerance = CDbl(Me.uneCstBundleProvisioningAddOnTol.Value)
            If (_prevBundleAddOnProvisioningTolerance <> _bundleAddOnProvisioningTolerance) Then
                _prevBundleAddOnProvisioningTolerance = _bundleAddOnProvisioningTolerance
                CalculateDiameter()
            End If

        Else
            uneCstBundleProvisioningAddOnTol.Value = _prevBundleAddOnProvisioningTolerance
        End If
    End Sub

    Private Sub uneCstBundleInstallationAddOnTol_KeyDown(sender As Object, e As KeyEventArgs) Handles uneCstBundleInstallationAddOnTol.KeyDown
        If uneCstBundleInstallationAddOnTol.IsInEditMode Then
            If (e.KeyCode = Keys.Enter) Then
                Me.uneCstBundleInstallationAddOnTol.Editor.ExitEditMode(False, True)
                Me.SelectNextControl(uneCstBundleInstallationAddOnTol, True, True, True, False)
            End If

            If (e.KeyCode = Keys.Escape) Then
                Me.uneCstBundleInstallationAddOnTol.Editor.ExitEditMode(True, False)
                Me.SelectNextControl(uneCstBundleInstallationAddOnTol, True, True, True, False)
            End If
        End If
    End Sub

    Private Sub uneCstBundleProvisioningAddOnTol_KeyDown(sender As Object, e As KeyEventArgs) Handles uneCstBundleProvisioningAddOnTol.KeyDown
        If uneCstBundleProvisioningAddOnTol.IsInEditMode Then
            If (e.KeyCode = Keys.Enter) Then
                Me.uneCstBundleProvisioningAddOnTol.Editor.ExitEditMode(False, True)
                Me.SelectNextControl(uneCstBundleProvisioningAddOnTol, True, True, True, False)
            End If

            If (e.KeyCode = Keys.Escape) Then
                Me.uneCstBundleProvisioningAddOnTol.Editor.ExitEditMode(True, False)
                Me.SelectNextControl(uneCstBundleProvisioningAddOnTol, True, True, True, False)
            End If
        End If
    End Sub

    Private Sub utmBundleCSAs_ToolClick(sender As Object, e As ToolClickEventArgs) Handles utmBundleCSAs.ToolClick
        Select Case e.Tool.Key
            Case ContextMenuToolKey.AddCableWire.ToString
                ReActivateCableWire()
                CalculateDiameter()
            Case ContextMenuToolKey.AddUserWire.ToString
                AddUserWire()
                CalculateDiameter()
            Case ContextMenuToolKey.CloneAsUserWires.ToString
                CloneAsUserWires()
                CalculateDiameter()
            Case ContextMenuToolKey.CollapseAll.ToString
                With Me.ugBundleCSAs
                    .BeginUpdate()
                    .Rows.CollapseAll(True)
                    .EndUpdate()
                End With
            Case ContextMenuToolKey.CopyUserWires.ToString
                CopyAllUserWires()
            Case ContextMenuToolKey.CopySelectedUserWires.ToString
                CopySelectedUserWires()
            Case ContextMenuToolKey.ExpandAll.ToString
                With Me.ugBundleCSAs
                    .BeginUpdate()
                    .Rows.ExpandAll(True)
                    .EndUpdate()
                End With
            Case ContextMenuToolKey.PasteUserWires.ToString
                PasteUserWires()
                CalculateDiameter()
            Case ContextMenuToolKey.RemoveCableWireTemporarily.ToString
                RemoveCableWireTemporarily()
                CalculateDiameter()
            Case ContextMenuToolKey.RemoveUserWire.ToString
                RemoveUserWire()
                'CalculateDiameter() now moved to AfterRowsDelete for Del key usage in grid
            Case ContextMenuToolKey.ResetPartnumberModification.ToString
                ResetPartNumberModification()
                CalculateDiameter()
            Case ContextMenuToolKey.BulkChangePartnumber.ToString
                BulkChangePartnumber()
                CalculateDiameter()
        End Select
    End Sub

    Private Sub CloneAsUserWires()
        With Me.ugBundleCSAs
            .BeginUpdate()

            _droppedRows = New List(Of UltraGridRow)

            For Each row As UltraGridRow In Me.ugBundleCSAs.Selected.Rows
                If (Not (CBool(row.Cells(ColumnKeys.TemporaryAdded.ToString).Value) OrElse CBool(row.Cells(ColumnKeys.TemporaryRemoved.ToString).Value))) Then
                    .ActiveRow = row

                    AddDraggedCableWire(DirectCast(row.ListObject, UltraDataRow), row.Band, row.ParentCollection, True)
                End If
            Next

            .EndUpdate()

            .ActiveRow = _droppedRows.LastOrDefault
            .Selected.Rows.Clear()

            For Each row As UltraGridRow In _droppedRows
                .Selected.Rows.Add(row)
            Next

            If (_droppedRows.Count <> 0) Then
                .ActiveRowScrollRegion.ScrollRowIntoView(_droppedRows.LastOrDefault)
            End If

            _droppedRows = Nothing
        End With

    End Sub


    Private Sub vDraw_MouseClick(sender As Object, e As MouseEventArgs) Handles vDraw.MouseClick
        If (e.Button = System.Windows.Forms.MouseButtons.Left) Then
            Dim clickPoint As gPoint = Me.vDraw.ActiveDocument.CCS_CursorPos
            Dim hitEntity As vdFigure = Me.vDraw.ActiveDocument.ActiveLayOut.GetEntityFromPoint(Me.vDraw.ActiveDocument.ActiveRender.View2PixelI(clickPoint), 4, False, vdDocument.LockLayerMethodEnum.DisableAll)

            RemoveHighlightOfCablesOrWiresInDrawing()


            If Not String.IsNullOrEmpty(hitEntity?.Label) Then
                Dim occ As IKblOccurrence = _kblMapper.GetOccurrenceObjectUntyped(hitEntity.Label)
                If occ Is Nothing OrElse TypeOf occ IsNot Segment Then
                    Dim ids As New List(Of String)
                    ids.Add(hitEntity.Label)

                    If occ IsNot Nothing Then
                        Select Case occ.ObjectType
                            Case KblObjectType.Core_occurrence
                                Dim highlightCable As Boolean = True

                                For Each circle As PackagingCircle In _calculatedSegmentDiameters(_segment.SystemId)(_selectedModuleConfig).InnerCircles.Where(Function(cir) cir.InnerCircles.Count = 1 AndAlso cir.InnerCircles(0).Id = hitEntity.Label AndAlso cir.Radius < cir.InnerCircles(0).Radius)
                                    highlightCable = False
                                    ids(0) = circle.Id

                                    Exit For
                                Next

                                If (highlightCable) Then
                                    For Each figure As vdFigure In Me.vDraw.ActiveDocument.ActiveLayOut.Entities
                                        If (TypeOf figure Is vdCircle) AndAlso (DirectCast(figure, vdCircle).HatchProperties IsNot Nothing) AndAlso (figure.Label = _kblMapper.KBLCoreCableMapper(hitEntity.Label)) Then
                                            hitEntity = figure
                                            ids(0) = hitEntity.Label

                                            Exit For
                                        End If
                                    Next
                                End If
                            Case KblObjectType.Wiring_group
                                Dim circle As PackagingCircle = Nothing

                                GetCircleById_Recursively(_calculatedSegmentDiameters(_segment.SystemId)(_selectedModuleConfig), ids(0), circle)

                                If (circle IsNot Nothing) AndAlso (circle.InnerCircles.Count <> 0) Then
                                    If (TypeOf circle.Parent.Tag Is Segment) Then
                                        ids.Clear()

                                        For Each innerCircle As PackagingCircle In circle.InnerCircles
                                            ids.Add(innerCircle.Id)
                                        Next
                                    Else
                                        For Each figure As vdFigure In Me.vDraw.ActiveDocument.ActiveLayOut.Entities
                                            If (TypeOf figure Is vdCircle) AndAlso (DirectCast(figure, vdCircle).HatchProperties IsNot Nothing) AndAlso (figure.Label = circle.Parent.Id) Then
                                                hitEntity = figure
                                                ids(0) = hitEntity.Label

                                                Exit For
                                            End If
                                        Next
                                    End If
                                End If
                        End Select
                    End If

                    If (TypeOf hitEntity Is vdPolyline) Then
                        For Each figure As vdFigure In Me.vDraw.ActiveDocument.ActiveLayOut.Entities
                            If (TypeOf figure Is vdCircle) AndAlso (DirectCast(figure, vdCircle).HatchProperties IsNot Nothing) AndAlso (figure.Label = hitEntity.Label) Then
                                hitEntity = figure
                                ids(0) = hitEntity.Label

                                Exit For
                            End If
                        Next
                    End If

                    If (DirectCast(hitEntity, vdCircle).HatchProperties IsNot Nothing) Then
                        HighlightCableOrWireInDrawing(DirectCast(hitEntity, vdCircle), False)
                    End If

                    SelectInGrid(ids)
                End If
            End If
        End If
    End Sub

    Private Sub vDraw_MouseMove(sender As Object, e As MouseEventArgs) Handles vDraw.MouseMove
        Static mpos As System.Drawing.Point

        If Not (mpos.X - e.Location.X = 0 AndAlso mpos.Y - e.Location.Y = 0) Then
            Dim currentCursorPoint As gPoint = Me.vDraw.ActiveDocument.CCS_CursorPos
            Dim hitEntity As VectorDraw.Professional.vdPrimaries.vdFigure = Me.vDraw.ActiveDocument.ActiveLayOut.GetEntityFromPoint(Me.vDraw.ActiveDocument.ActiveRender.View2PixelI(currentCursorPoint), 6, False, VectorDraw.Professional.vdObjects.vdDocument.LockLayerMethodEnum.DisableAll)

            Me.uttmBundleView.HideToolTip()

            If (hitEntity IsNot Nothing) AndAlso (hitEntity.XProperties.FindName("Tooltip") IsNot Nothing) AndAlso (hitEntity.XProperties.FindName("Tooltip").PropValue IsNot Nothing) Then
                Dim ttInfo As UltraToolTipInfo = Me.uttmBundleView.GetUltraToolTip(Me.vDraw)
                ttInfo.ToolTipText = hitEntity.XProperties.FindName("Tooltip").PropValue.ToString
                ttInfo.ToolTipImage = Infragistics.Win.ToolTipImage.Info
                ttInfo.ToolTipTitle = BundleCrossSectionFormStrings.BundlePic_Tooltip
                ttInfo.Enabled = Infragistics.Win.DefaultableBoolean.False
                Me.uttmBundleView.ShowToolTip(Me.vDraw)
            End If
        End If
        mpos = e.Location
    End Sub

    Private Sub ugBundleCSAs_KeyDown(sender As Object, e As KeyEventArgs) Handles ugBundleCSAs.KeyDown
        If e.KeyCode = Keys.Enter Then
            ugBundleCSAs.PerformAction(UltraGridAction.ExitEditMode)
        End If
    End Sub

    Friend ReadOnly Property IsWireRoutingAvailable() As Boolean
        Get
            Return Me.udsBundleCSAs.Rows.Count <> 0
        End Get
    End Property

    Private Class PercentageDataFilter
        Implements Infragistics.Win.IEditorDataFilter
        Public Function Convert(conversionArgs As Infragistics.Win.EditorDataFilterConvertArgs) As Object Implements Infragistics.Win.IEditorDataFilter.Convert
            Select Case conversionArgs.Direction
                Case Infragistics.Win.ConversionDirection.EditorToOwner
                    Dim value As Decimal = System.Convert.ToDecimal(conversionArgs.Value)
                    conversionArgs.Handled = True
                    Return CDbl(value / 100)
                Case Infragistics.Win.ConversionDirection.OwnerToEditor
                    Dim value As Decimal = System.Convert.ToDecimal(conversionArgs.Value)
                    conversionArgs.Handled = True
                    Return CInt(value * 100)
            End Select
            Return 0
        End Function
    End Class

End Class