Imports System.ComponentModel
Imports System.Reflection

Public Module Common

    ' Extra model variant in which the objects are being activated or deactivated by the document BL - leave empty to retain the MasterVariant as for activation&deactivation (the variant is used to control the disabled/enabled-state for each entity)
    Public Const ISACTIVE_VARIANT_NAME As String = "DocumentActivatedVariant"

    Public Const CHANGED_MODIFIED_FORECOLOR As Drawing.KnownColor = Drawing.KnownColor.DarkOrange
    Public Const CAR_TRANSFORMATION_SETTING_xHCVFILE As String = "CarTransformationSetting.bin"
    Public Const COMPANY_FOLDER As String = "Zuken"
    Public Const PRODUCT_FOLDER As String = "E3.HarnessAnalyzer"
    Public Const MSG_BOX_TITLE As String = "E³.HarnessAnalyzer"
    Public Const SELECTING_SIZE As Integer = 6
    Public Const SYSTEM_ID As String = "SystemId"
    Public Const PROPERTY_CONNECTORSHORTNAME As String = "ConnectorShortName"
    Public Const DEFAULT_WEIGHT_SETTINGS_FILE As String = "WeightSettings_default.xml"
    Public Const DEFAULT_E3XML_SETTINGS_FILE As String = "E3XmlSettings_default.xml"
    Public Const DEFAULT_GENERAL_SETTINGS_FILE As String = "GeneralSettings_default.xml"
    Public Const DEFAULT_DIAMETER_SETTINGS_FILE As String = "DiameterSettings_default.xml"
    Public Const E3HA_PROCESS_NAME As String = "E3.HarnessAnalyzer"
    Public Const SANDBOX_TEST_DATA_DIR_PATH As String = "C:\Solution\.TestData"
    Public Const SANDBOX_USER_NAME As String = "WDAGUtilityAccount"
    Public ReadOnly Property VDRAW_LAYER_REDLININGS_NAME As String = [Lib].Schema.KBL.KblObjectType.Redlining.ToString(True)
    Public Const VDRAW_LAYER_REDLININGS_BACKGROUND_NAME As String = "RedliningBackgrounds"
    Public Const VDRAW_LAYER_QMSTAMPS_NAME As String = "QMStamps"

    Friend Const VECTORDRAW_EVALUATION_EXPIRED_EXCEPTION_MESSAGE As String = "License error Your 90 days evaluation period has expired,-159"

    Public Const E3_APPLICATION_PREFIX As String = "!E3APPLICATION"

    Public Const CORE_WIRE_NUMBER_KEY As String = "CoreWireNumber"
    Public Const MODULE_KEY As String = "Module"
    Public Const ELLIPSIS As String = "..."
    Public Const NOT_AVAILABLE As String = "<n/a>"

    Public Const NOF_DIGITS_LOCATIONS As Integer = 1

    ''' <summary>
    ''' Specific weight of copper in g/cm³ (density)
    ''' </summary>
    ''' <remarks></remarks>
    Public Const SPECIFIC_COPPER_WEIGHT As Double = 8.92
    ''' <summary>
    ''' Specific weight of Aluminium in g/cm³ (density)
    ''' </summary>
    ''' <remarks></remarks>
    Public Const SPECIFIC_ALUMINIUM_WEIGHT As Double = 2.7
    ''' <summary>
    ''' DIN A0 paper size height
    ''' </summary>
    Public Const DEFAULT_DOC_HEIGHT As Integer = 841

    Public Delegate Sub JobDelegate(ByVal worker As BackgroundWorker, ByVal e As DoWorkEventArgs)
    Public Delegate Sub JobFinishedDelegate(ByVal worker As BackgroundWorker, ByVal e As RunWorkerCompletedEventArgs)
    Public Delegate Sub JobProgressDelegate(ByVal worker As BackgroundWorker, ByVal e As ProgressChangedEventArgs)

    <ObfuscationAttribute(Feature:="renaming", ApplyToMembers:=True)>
    Public Enum ApplicationMenuToolKey
        CloseDocument
        CloseDocuments
        ExitE3HarnessAnalyzer
        ExportExcel
        ExportGraphic
        ExportPDF
        ExportKbl
        OpenDocument
        Print
        RecentDocuments
        SaveDocument
        SaveDocumentAs
        SaveDocuments
    End Enum

    Public Enum ChecksTabToolKey
        CavityAssignment
        UltrasonicSpliceTerminalDistance
        Validity
        TopologyCompare
    End Enum

    Public Enum EditTabToolKey
        AddQMStamp
        DeleteQMStamp
        DeleteRedlining
        EditRedlining
        ExportCavityAssignment
        ExportGraphicalDataCompareResult
        ExportMemolist
        ExportQMStamps
        ExportRedlining
        ExportTechnicalDataCompareResult
        ImportGraphicalDataCompareResult
        ImportMemolist
        ImportQMStamps
        ImportRedlining
        ImportTechnicalDataCompareResult
        ImportValidityCheckResults
    End Enum

    Public Enum HarnessModuleConfigurationType
        Custom
        FromKBL
        UserDefined
    End Enum

    <ObfuscationAttribute(Feature:="renaming", ApplyToMembers:=True)>
    Public Enum HomeTabToolKey
        AnalysisShowDryWet
        AnalysisShowEyelets
        AnalysisShowPlatingMat
        AnalysisShowProtections
        AnalysisShowQMIssues
        AnalysisShowSplices
        AnalysisShowPartnumbers
        BOMActive
        BOMAll
        DeviceBOM
        BundleCalculator
        CompareData
        CompareGraphic
        Display3DView
        DisplaySchematicsView
        Document3DFilter
        ExportExcel
        ExportExcelActiveDataTable
        Inliners
        LoadCarTransformation
        Export3DModel
        ModuleConfigManager
        Pan
        PasteFromClipboard
        Refresh
        OpenInternalModelViewer ' HINT: this menu button is only intended for debug-mode and only for internal use´!
        SaveCarTransformation
        Search
        SelectAdditionalInfo
        TimeLeaders
        Dimensions
        ReferenceVertex
        TopologyEditor
        HideNoModuleEntities
        ValidateData
        ZoomExtends
        ZoomIn
        ZoomOut
        ZoomPrevious
        ZoomWindow
        ZoomMagnifier
    End Enum

    Public Enum RedliningClassification
        None
        GraphicalComment
        Information
        LengthComment
        Question
        Confirmation
        [Error]
    End Enum

    Public Enum SettingsTabToolKey
        About
        DataTableSettings
        DisplayDrawingsHub
        DisplayInformationHub
        DisplayLogHub
        DisplayMemolistHub
        DisplayModulesHub
        DisplayNavigatorHub
        DisplayQMStamps
        DisplayRedlinings
        DisplayTopologyHub
        GeneralSettings
        Help
        Indicators
        ClockSymbols
        Dimensions
        ImplicitDimensions
        ViewDirections
        LayoutSettings
        WireTypeDiameters
        WireTypeWeights
        Hints
        SectionDimensions
    End Enum

    Public Enum WiringGroupConsistencyState
        Valid
        HasSingleCoreOrWireAssigned
        HasCoresAndWiresAssigned
        HasCoresOfDifferentCablesAssigned
    End Enum

    Friend Enum AddOrUpdateResult
        Added
        OnlyUpdate
    End Enum

    Public Enum ToolKeys
        Open
        Export
        ExportList
        ExportLabel
        RecentLabel
        RecentList
        OpenDocumentLabel
        OpenDocumentList
    End Enum

    Public Enum PaneKeys
        TopologyHub
        DrawingsHub
        ModulesHub
        NavigatorHub
        MemolistHub
        InformationHub
        LogHub
    End Enum

End Module