Imports System.ComponentModel
Imports System.Globalization
Imports System.IO
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Runtime.InteropServices.ComTypes
Imports System.Threading
Imports System.Xml.Serialization
Imports System.Drawing
Imports Infragistics.Shared
Imports Infragistics.Win
Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinGrid
Imports Infragistics.Win.UltraWinGrid.ExcelExport
Imports Infragistics.Win.UltraWinProgressBar
Imports Infragistics.Win.UltraWinTabControl
Imports Infragistics.Win.UltraWinToolbars
Imports VectorDraw.Professional.vdCollections
Imports Zuken.E3.HarnessAnalyzer.Project.Documents
Imports Zuken.E3.HarnessAnalyzer.QualityStamping
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.HarnessAnalyzer.Settings.QualityStamping.Specification
Imports Zuken.E3.HarnessAnalyzer.Shared
Imports Zuken.E3.HarnessAnalyzer.Shared.Common
Imports Zuken.E3.Interop
Imports Zuken.E3.Lib.Router.Topology
Imports Zuken.E3.Lib.Schema.Kbl.Properties

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class InformationHub

    Friend Event ActiveTabChanged(sender As Object, e As ActiveTabChangedEventArgs)
    Friend Event GridKeyDown(sender As Object, e As GridKeyDownEventArgs)
    Friend Event AfterRowActivate(sender As Object, e As RowEventArgs)
    Friend Event UnhandledCellDataRequested(sender As Object, e As CellDataRequestedEventArgs)
    Friend Event CellValueUpdated(sender As Object, e As CellDataUpdatedEventArgs)
    Friend Event ClickCell(sender As Object, e As ClickCellEventArgs)
    Friend Event ApplyModuleConfiguration(sender As InformationHub, e As InformationHubEventArgs)
    Friend Event HighlightEntireRoutingPath(sender As InformationHub, e As HighlightEntireRoutingPathEventArgs)
    Friend Event HubSelectionChanged(sender As InformationHub, e As InformationHubEventArgs)
    Friend Event LogMessage(sender As InformationHub, e As LogEventArgs)
    Friend Event MemolistChanged(sender As InformationHub, e As Dictionary(Of String, Object))
    Friend Event Message(sender As InformationHub, e As MessageEventArgs)
    Friend Event QMStampsChanged(sender As InformationHub, e As InformationHubEventArgs)
    Friend Event RedliningsChanged(sender As InformationHub, e As InformationHubEventArgs)
    Friend Event InitiateRedliningDialog(sender As Object, e As InformationHubEventArgs)
    Friend Event ShowEntireRoutingPath(sender As InformationHub, e As InformationHubEventArgs)
    Friend Event ShowOverallConnectivity(sender As InformationHub, e As InformationHubEventArgs)
    Friend Event ShowStartEndConnectors(sender As InformationHub, e As InformationHubEventArgs)

    Private _accessoryGridAppearance As AccessoryGridAppearance
    Private _approvalGridAppearance As ApprovalGridAppearance
    Private _assemblyPartGridAppearance As AssemblyPartGridAppearance
    Private _cableGridAppearance As CableGridAppearance
    Private _changeDescriptionGridAppearance As ChangeDescriptionGridAppearance
    Private _componentBoxGridAppearance As ComponentBoxGridAppearance
    Private _componentGridAppearance As ComponentGridAppearance
    Private _connectorGridAppearance As ConnectorGridAppearance
    Private _coPackGridAppearance As CoPackGridAppearance
    Private _defDimSpecGridAppearance As DefaultDimSpecGridAppearance
    Private _dimSpecGridAppearance As DimSpecGridAppearance
    Private _fixingGridAppearance As FixingGridAppearance
    Private _moduleGridAppearance As ModuleGridAppearance
    Private _netGridAppearance As NetGridAppearance
    Private _segmentGridAppearance As SegmentGridAppearance
    Private _vertexGridAppearance As VertexGridAppearance
    Private _wireGridAppearance As WireGridAppearance

    Private _clickedGridRow As UltraGridRow
    Private _comparisonMapperList As Dictionary(Of KblObjectType, ComparisonMapper)
    Private _connectorCavityContactPointMapper As Dictionary(Of String, List(Of Tuple(Of Object, Tuple(Of String, String))))
    Private _contextMenuGrid As PopupMenuTool
    Private _contextMenuTab As PopupMenuTool
    Private _currentHeaderRowIndex As Integer
    Private _diameterSettings As DiameterSettings
    Private _displayValueExceedsExcelCellLimitWarning As Boolean
    Private _generalSettings As GeneralSettingsBase
    Private _exportOnlyActiveSelectedRows As Boolean
    Private _exportRunning As Boolean
    Private _fromGridContextMenu As Boolean
    Private _grids As Dictionary(Of String, UltraGrid)
    Private _informationHubEventArgs As InformationHubEventArgs
    Private _kblMapperForCompare As KblMapper
    Private _messageEventArgs As MessageEventArgs
    Private _oldE3Wires As Dictionary(Of Integer, e3Pin)
    Private _parentForm As Form
    Private _pressedMouseButton As MouseButtons
    Private _pressedKey As Keys = Keys.None
    Private _previousGrid As UltraGrid
    Private _progressBar As UltraProgressBar
    Private _qmStamps As QMStamps
    Private _qmStampSpecifications As QMStampSpecifications
    Private _redliningInformation As RedliningInformation
    Private _selectedChildRows As HashSet(Of UltraGridRow)
    Private _skipDescendants As Boolean
    Private _wiresAndCores As List(Of IKblWireCoreOccurrence)
    Private _filledDataSources As New List(Of UltraDataSource)
    Private _logEventArgs As New LogEventArgs
    Private _progress As Action(Of Integer)
    Private _isHubSelecting As Boolean = False
    Private _selectedConnectorRow As UltraGridRow
    Private _syncCtxt As Threading.SynchronizationContext = Threading.SynchronizationContext.Current
    Private _currentRowFilterInfo As RowFilterInfo
    Private _rowFilters As RowFiltersCollection
    Private _kblMapper As KblMapper
    Private _kblIdRowCache As New KblIdRowCache

    Public Const CAVITY_COUNT_COLUMN_KEY As String = "CavityCount"
    Public Const WIRE_CLASS_COLUMN_KEY As String = "WireClass"
    Public Const COMPONENT_CLASS_COLUMN_KEY As String = "ComponentClass"
    Public Const SYSTEM_ID_COLUMN_KEY As String = "SystemId"
    Public Const PRIMARY_LENGTH_COLUMN_KEY As String = "PrimaryLength"
    Public Const PLACEMENT As String = "Placement" 'corresponds to the placement property on fixings , accessories and connectors

    Private WithEvents _bundleCrossSectionForm As BundleCrossSectionForm

    Public Sub New(diameterSettings As DiameterSettings, generalSettings As GeneralSettingsBase, parentForm As Form, qmStampSpecifications As QMStampSpecifications)
        InitializeComponent()
        _connectorCavityContactPointMapper = New Dictionary(Of String, List(Of Tuple(Of Object, Tuple(Of String, String))))
        _contextMenuGrid = New PopupMenuTool("GridContextMenu")
        _contextMenuTab = New PopupMenuTool("TabContextMenu")
        _diameterSettings = diameterSettings
        _generalSettings = generalSettings
        _informationHubEventArgs = New InformationHubEventArgs(String.Empty)
        _logEventArgs = New LogEventArgs
        _messageEventArgs = New MessageEventArgs(MessageEventArgs.MsgType.ShowMessage)
        _parentForm = parentForm
        _qmStampSpecifications = qmStampSpecifications
        _selectedChildRows = New HashSet(Of UltraGridRow)
        _wiresAndCores = New List(Of IKblWireCoreOccurrence)

        Me.BackColor = Color.White

        If (TypeOf _parentForm Is DocumentForm) Then
            Me.utchpInformationHub.Enabled = DirectCast(_parentForm, DocumentForm).IsTouchEnabled
        Else
            Me.utchpInformationHub.Enabled = False
        End If

        FillContextMenus()

        For Each tb As UltraTab In utcInformationHub.Tabs
            Try
                Dim grid As UltraGrid = tb.TabPage.Controls.OfType(Of UltraGrid).Single
                AddHandler grid.ClickCell, AddressOf Grid_ClickCell
                AddHandler grid.InitializeLayout, AddressOf Grid_InitializeLayout
                AddHandler grid.AfterRowActivate, AddressOf Grid_AfterRowActivate
                AddHandler grid.KeyUp, AddressOf grid_KeyUp

                grid.DisplayLayout.Override.AllowMultiCellOperations = grid.DisplayLayout.Override.AllowMultiCellOperations Or AllowMultiCellOperation.Copy
                grid.DisplayLayout.Override.FilterUIProvider = New SupportDialogs.FilterUIProvider.UltraGridExcelStyleFilterUIProvider
            Catch ex As Exception
                If TryCast(Me.DocumentForm, DocumentForm)?._logHub IsNot Nothing Then
                    TryCast(Me.DocumentForm, DocumentForm)?._logHub.WriteLogMessage(New LogEventArgs(LogEventArgs.LoggingLevel.Error, String.Format(ErrorStrings.InformationHub_ErrorSetFilter, ex.Message)))
                Else
                    Throw
                End If
            End Try
        Next
    End Sub

    Protected Overridable Sub AddNewRowFilters(grid As UltraGrid)
        Select Case grid.Name
            Case ugAccessories.Name, ugAssemblyParts.Name, ugComponentBoxes.Name, ugCoPacks.Name, ugDimSpecs.Name, ugFixings.Name
                _rowFilters.AddNewInactiveObjectsRowFilter(grid)
            Case ugConnectors.Name
                _rowFilters.AddNew(Of ConnectorsRowFilter)(grid)
                _rowFilters.AddNew(Of CavityChildRowFilter)(grid)
            Case ugCables.Name
                _rowFilters.AddNew(Of CablesRowFilter)(grid)
                _rowFilters.AddNew(Of CoresChildRowFilter)(grid)
            Case ugComponents.Name
                _rowFilters.AddNew(Of ComponentRowFilter)(grid)
            Case ugModules.Name
                _rowFilters.AddNew(Of ModulesRowFilter)(grid)
            Case ugNets.Name
                _rowFilters.AddNew(Of NetsRowFilter)(grid)
            Case ugSegments.Name
                _rowFilters.AddNew(Of SegmentsRowFilter)(grid)
                _rowFilters.AddNew(Of WireProtectionRowFilter)(grid)
            Case ugVertices.Name
                _rowFilters.AddNew(Of VerticesRowFilter)(grid)
                _rowFilters.AddNew(Of WireProtectionRowFilter)(grid)
            Case ugWires.Name
                _rowFilters.AddNew(Of WiresRowFilter)(ugWires)
            Case Else
                Throw New NotImplementedException($"Adding row filter for grid ""{grid.Name}"" not implemented!")
        End Select
    End Sub


    Protected Overridable Function GetDetailInformationForm(caption As String, displayData As Object, Optional inactiveObjects As IDictionary(Of String, IEnumerable(Of String)) = Nothing, Optional kblMapper As KblMapper = Nothing, Optional objectId As String = Nothing, Optional wireLengthType As String = Nothing, Optional objectType As String = Nothing) As DetailInformationForm
        Return New DetailInformationForm(caption, displayData, inactiveObjects, _kblMapper, objectId, wireLengthType, objectType)
    End Function

    Protected Overridable Sub OnAfterRowActivate(Optional grid As UltraGrid = Nothing)
        If (grid Is Nothing) Then
            grid = Me.ActiveGrid
        End If
        RaiseEvent AfterRowActivate(Me, New RowEventArgs(grid.ActiveRow))
    End Sub

    Protected Overridable Sub OnClickCell(e As ClickCellEventArgs)
        RaiseEvent ClickCell(Me, e)
    End Sub

    Protected Overridable Sub OnGridKeyDown(sender As Object, e As GridKeyDownEventArgs)
        _pressedKey = e.KeyEventArgs.KeyCode
        RaiseEvent GridKeyDown(sender, e)
    End Sub

    Protected Overridable Sub OnLogMessage(e As LogEventArgs)
        If _syncCtxt IsNot Nothing Then
            _syncCtxt.Send(Sub() RaiseEvent LogMessage(Me, e))
        Else
            RaiseEvent LogMessage(Me, e)
        End If
    End Sub

    Protected Overridable Sub OnPreviewGridKeyDown(sender As Object, e As PreviewKeyDownEventArgs)
    End Sub

    Private Sub InitializeRowFilterCollection(kbl As KblMapper)
        If _currentRowFilterInfo Is Nothing Then
            _currentRowFilterInfo = New RowFilterInfo(Me)
            If _rowFilters IsNot Nothing Then
                _rowFilters.Dispose() ' HINT: shoudn't be possible under normal circumstances
            End If

            _rowFilters = New RowFiltersCollection(_currentRowFilterInfo)
        Else
            _currentRowFilterInfo.InfoHub = Me
        End If
    End Sub

    'Private Sub InitializeRowSurrogatorsCollection(kbl As IKblContainer, kbl_compare As IKblContainer)
    '    _rowSurrogators?.Dispose()
    '    _rowSurrogators = Nothing
    '    _rowSurrogators = New KblRowSurrogatorsCollection(kbl, kbl_compare, TryCast(Me.DocumentForm, DocumentForm)?.MainForm)
    'End Sub

    Friend Function InitializeDataSources(kblMapper As KblMapper, qmStamps As QMStamps, redliningInformation As RedliningInformation, progress As Action(Of Integer), Optional comparisonMapperList As Dictionary(Of KblObjectType, ComparisonMapper) = Nothing, Optional kblMapperForCompare As KblMapper = Nothing, Optional displayLengthClassMsg As Boolean = True) As Boolean
        _filledDataSources.Clear()
        _comparisonMapperList = comparisonMapperList
        _kblMapper = kblMapper

        InitializeRowFilterCollection(kblMapper)
        'InitializeRowSurrogatorsCollection(kblMapper, kblMapperForCompare)

        _kblMapperForCompare = kblMapperForCompare
        _qmStamps = qmStamps
        _redliningInformation = redliningInformation
        _progress = progress

        If _comparisonMapperList Is Nothing Then
            Me.utcInformationHub.Tabs(TabNames.tabWires.ToString).Text = InformationHubStrings.WiresTab_Caption

            _logEventArgs.LogLevel = LogEventArgs.LoggingLevel.Information
            _logEventArgs.LogMessage = String.Format(InformationHubStrings.StartLoadKBL_LogMsg, Now)

            OnLogMessage(_logEventArgs)
        End If

        Using p_total As IProgressExProvider = ProgressEx.AttachNew(progress, 3, ProgressEx.ReportType.Direct)
            Try
                If _comparisonMapperList Is Nothing Then
                    Dim showCableLengthTypeMissing As Boolean = False
                    Dim showWireLengthTypeMissing As Boolean = False

                    With _kblMapper
                        Dim lastPercent As Integer = 0
                        Dim lock As New System.Threading.SemaphoreSingle
                        Using chunk As ProgressExChunk = p_total.ProgressEx.NewChunk()
                            _kblMapper.ReBuild(chunk.CreateProgressAction)
                        End Using

                        Using p_total.ProgressEx.NewChunk(3)

                            If _generalSettings IsNot Nothing Then
                                If (_generalSettings.DefaultCableLengthType Is Nothing OrElse _generalSettings.DefaultCableLengthType = String.Empty) AndAlso (.KBLCableLengthTypes.Count <> 0) Then
                                    _generalSettings.DefaultCableLengthType = .KBLCableLengthTypes(0)
                                ElseIf (_generalSettings.DefaultCableLengthType IsNot Nothing) AndAlso (_generalSettings.DefaultCableLengthType <> String.Empty) AndAlso (.KBLCableLengthTypes.Count <> 0) AndAlso (Not .KBLCableLengthTypes.Any(Function(lt) lt.ToLower = _generalSettings.DefaultCableLengthType.ToLower)) Then
                                    showCableLengthTypeMissing = displayLengthClassMsg
                                End If
                            End If

                            p_total.ProgressEx.ProgressChunkStep()

                            If _generalSettings IsNot Nothing Then
                                If (_generalSettings.DefaultWireLengthType Is Nothing OrElse _generalSettings.DefaultWireLengthType = String.Empty) AndAlso (.KBLWireLengthTypes.Count <> 0) Then
                                    _generalSettings.DefaultWireLengthType = _kblMapper.KBLWireLengthTypes(0)
                                ElseIf (_generalSettings.DefaultWireLengthType IsNot Nothing) AndAlso (_generalSettings.DefaultWireLengthType <> String.Empty) AndAlso (.KBLWireLengthTypes.Count <> 0) AndAlso (Not .KBLWireLengthTypes.Any(Function(lt) lt.ToLower = _generalSettings.DefaultWireLengthType.ToLower)) Then
                                    showWireLengthTypeMissing = displayLengthClassMsg
                                End If
                            End If

                            p_total.ProgressEx.ProgressChunkStep()

                            TryCast(Me.ParentForm, DocumentForm)?.MainForm?.InvokeOrDefault(
                                Sub()
                                    If showCableLengthTypeMissing AndAlso showWireLengthTypeMissing Then
                                        TryCast(Me.DocumentForm, DocumentForm)?._logHub.WriteLogMessage(New LogEventArgs(LogEventArgs.LoggingLevel.Warning, String.Format(InformationHubStrings.DefWireAndCableLengthTypeNotFound_Msg, _generalSettings.DefaultWireLengthType, _generalSettings.DefaultCableLengthType)))
                                    ElseIf showCableLengthTypeMissing Then
                                        TryCast(Me.DocumentForm, DocumentForm)?._logHub.WriteLogMessage(New LogEventArgs(LogEventArgs.LoggingLevel.Warning, String.Format(InformationHubStrings.DefCabLengthTypeNotFound_Msg, _generalSettings.DefaultCableLengthType)))
                                    ElseIf showWireLengthTypeMissing Then
                                        TryCast(Me.DocumentForm, DocumentForm)?._logHub.WriteLogMessage(New LogEventArgs(LogEventArgs.LoggingLevel.Warning, String.Format(InformationHubStrings.DefWirLengthTypeNotFound_Msg, _generalSettings.DefaultWireLengthType)))
                                    End If
                                End Sub)
                            p_total.ProgressEx.ProgressChunkStep()
                        End Using
                    End With
                End If
            Catch ex As Exception
#If DEBUG Or CONFIG = "Debug" Then
                Throw
#End If
                TryCast(Me.DocumentForm, DocumentForm)?.MainForm.AppInitManager.ErrorManager.ShowMsgBoxErrorOrWriteConsole(String.Format(InformationHubStrings.ErrorFillMapper_Msg, vbCrLf, ex.Message), ErrorCodes.ERROR_INVALID_DATA)

                _logEventArgs.LogLevel = LogEventArgs.LoggingLevel.Error
                _logEventArgs.LogMessage = String.Format(InformationHubStrings.ProblemLoadMapper_LogMsg, ex.Message)

                OnLogMessage(_logEventArgs)

                Return False
            End Try

            Try
                Using c As ProgressExChunk = p_total.ProgressEx.NewChunk(20)
                    InitializeHarness() '1
                    p_total.ProgressEx.ProgressChunkStep()
                    InitializeModules() '2
                    p_total.ProgressEx.ProgressChunkStep()
                    InitializeVertices() '3
                    p_total.ProgressEx.ProgressChunkStep()
                    InitializeSegments() '4
                    p_total.ProgressEx.ProgressChunkStep()
                    InitializeAccessories() '5
                    p_total.ProgressEx.ProgressChunkStep()
                    InitializeAssemblyParts() '6
                    p_total.ProgressEx.ProgressChunkStep()
                    InitializeFixings() '7
                    p_total.ProgressEx.ProgressChunkStep()
                    InitializeComponentBoxes() '8
                    p_total.ProgressEx.ProgressChunkStep()
                    InitializeComponents() '9
                    p_total.ProgressEx.ProgressChunkStep()
                    InitializeConnectors() '10
                    p_total.ProgressEx.ProgressChunkStep()
                    InitializeCables() '11
                    p_total.ProgressEx.ProgressChunkStep()
                    InitializeWires() '12
                    p_total.ProgressEx.ProgressChunkStep()
                    InitializeNets() '13
                    p_total.ProgressEx.ProgressChunkStep()
                    InitializeApprovals() '14
                    p_total.ProgressEx.ProgressChunkStep()
                    InitializeChangeDescriptions() '15
                    p_total.ProgressEx.ProgressChunkStep()
                    InitializeCoPacks() '16
                    p_total.ProgressEx.ProgressChunkStep()
                    InitializeDefDimSpecs() '17
                    p_total.ProgressEx.ProgressChunkStep()
                    InitializeDimSpecs() '18
                    p_total.ProgressEx.ProgressChunkStep()
                    InitializeRedlinings() '19
                    p_total.ProgressEx.ProgressChunkStep()
                    InitializeQMStamps() '20
                    p_total.ProgressEx.ProgressChunkStep()
                    Return True
                End Using
            Catch ex As Exception
#If DEBUG Or CONFIG = "Debug" Then
                Throw
#End If
                If (_comparisonMapperList Is Nothing) Then
                    TryCast(Me.DocumentForm, DocumentForm)?.MainForm.AppInitManager.ErrorManager.ShowMsgBoxErrorOrWriteConsole(String.Format(InformationHubStrings.ErrorInitGridData_Msg, vbCrLf, ex.Message), ErrorCodes.ERROR_INVALID_DATA)

                    _logEventArgs.LogLevel = LogEventArgs.LoggingLevel.Error
                    _logEventArgs.LogMessage = String.Format(InformationHubStrings.ProblemInitGridData_LogMsg, ex.Message)

                    OnLogMessage(_logEventArgs)
                Else
                    TryCast(Me.DocumentForm, DocumentForm)?.MainForm.AppInitManager.ErrorManager.ShowMsgBoxErrorOrWriteConsole(String.Format(InformationHubStrings.ErrorInitCompGridData_Msg, vbCrLf, ex.Message), ErrorCodes.ERROR_INVALID_DATA)
                End If

                Return False
            End Try
        End Using
    End Function

    Private Sub InitGridDefaults(grid As UltraGrid)
        grid.SyncWithCurrencyManager = False
    End Sub

    Friend Function InitializeGrids() As Boolean
        Try
            Me.ugHarness.CreationFilter = New ValidityCheckCreationFilter(_parentForm)
            InitGridDefaults(Me.ugHarness)
            Me.ugHarness.DataSource = Me.udsHarness

            Me.ugModules.CreationFilter = New ValidityCheckCreationFilter(_parentForm)
            InitGridDefaults(Me.ugModules)
            Me.ugModules.DataSource = Me.udsModules

            Me.ugVertices.CreationFilter = New ValidityCheckCreationFilter(_parentForm)
            InitGridDefaults(Me.ugVertices)
            Me.ugVertices.DataSource = Me.udsVertices

            Me.ugSegments.CreationFilter = New ValidityCheckCreationFilter(_parentForm)
            InitGridDefaults(Me.ugSegments)
            Me.ugSegments.DataSource = Me.udsSegments

            Me.ugAccessories.CreationFilter = New ValidityCheckCreationFilter(_parentForm)
            InitGridDefaults(Me.ugAccessories)
            Me.ugAccessories.DataSource = Me.udsAccessories

            Me.ugAssemblyParts.CreationFilter = New ValidityCheckCreationFilter(_parentForm)
            InitGridDefaults(Me.ugAssemblyParts)
            Me.ugAssemblyParts.DataSource = Me.udsAssemblyParts

            Me.ugFixings.CreationFilter = New ValidityCheckCreationFilter(_parentForm)
            InitGridDefaults(Me.ugFixings)
            Me.ugFixings.DataSource = Me.udsFixings

            Me.ugComponentBoxes.CreationFilter = New ValidityCheckCreationFilter(_parentForm)
            InitGridDefaults(Me.ugComponentBoxes)
            Me.ugComponentBoxes.DataSource = Me.udsComponentBoxes

            Me.ugComponents.CreationFilter = New ValidityCheckCreationFilter(_parentForm)
            InitGridDefaults(Me.ugComponents)
            Me.ugComponents.DataSource = Me.udsComponents

            Me.ugConnectors.CreationFilter = New ValidityCheckCreationFilter(_parentForm)
            InitGridDefaults(Me.ugConnectors)
            Me.ugConnectors.DataSource = Me.udsConnectors

            Me.ugCables.CreationFilter = New ValidityCheckCreationFilter(_parentForm)
            InitGridDefaults(Me.ugCables)
            Me.ugCables.DataSource = Me.udsCables

            Me.ugWires.CreationFilter = New ValidityCheckCreationFilter(_parentForm)
            InitGridDefaults(Me.ugWires)
            Me.ugWires.DataSource = Me.udsWires

            InitGridDefaults(Me.ugNets)
            Me.ugNets.DataSource = Me.udsNets

            InitGridDefaults(Me.ugApprovals)
            Me.ugApprovals.DataSource = Me.udsApprovals

            InitGridDefaults(Me.ugChangeDescriptions)
            Me.ugChangeDescriptions.DataSource = Me.udsChangeDescriptions

            InitGridDefaults(Me.ugCoPacks)
            Me.ugCoPacks.DataSource = Me.udsCoPacks

            InitGridDefaults(Me.ugDefDimSpecs)
            Me.ugDefDimSpecs.DataSource = Me.udsDefDimSpecs

            Me.ugDimSpecs.CreationFilter = New ValidityCheckCreationFilter(_parentForm)
            InitGridDefaults(Me.ugDimSpecs)
            Me.ugDimSpecs.DataSource = Me.udsDimSpecs

            InitGridDefaults(Me.ugRedlinings)
            Me.ugRedlinings.DataSource = Me.udsRedlinings

            InitGridDefaults(Me.ugQMStamps)
            Me.ugQMStamps.DataSource = Me.udsQMStamps

            _grids = New Dictionary(Of String, UltraGrid)

            With _grids
                .Add(KblObjectType.Harness.ToString, Me.ugHarness)
                .Add(KblObjectType.Module.ToString, Me.ugModules)
                .Add(KblObjectType.Node.ToString, Me.ugVertices)
                .Add(KblObjectType.Segment.ToString, Me.ugSegments)
                .Add(KblObjectType.Accessory_occurrence.ToString, Me.ugAccessories)
                .Add(KblObjectType.Assembly_part_occurrence.ToString, Me.ugAssemblyParts)
                .Add(KblObjectType.Fixing_occurrence.ToString, Me.ugFixings)
                .Add(KblObjectType.Component_box_occurrence.ToString, Me.ugComponentBoxes)
                .Add(KblObjectType.Component_occurrence.ToString, Me.ugComponents)
                .Add(KblObjectType.Connector_occurrence.ToString, Me.ugConnectors)
                .Add(KblObjectType.Special_wire_occurrence.ToString, Me.ugCables)
                .Add(KblObjectType.Wire_occurrence.ToString, Me.ugWires)
                .Add(KblObjectType.Net.ToString, Me.ugNets)
                .Add(KblObjectType.Approval.ToString, Me.ugApprovals)
                .Add(KblObjectType.Change_description.ToString, Me.ugChangeDescriptions)
                .Add(KblObjectType.Co_pack_occurrence.ToString, Me.ugCoPacks)
                .Add(KblObjectType.Default_dimension_specification.ToString, Me.ugDefDimSpecs)
                .Add(KblObjectType.Dimension_specification.ToString, Me.ugDimSpecs)
                .Add(KblObjectType.Redlining.ToString, Me.ugRedlinings)
                .Add(KblObjectType.QMStamp.ToString, Me.ugQMStamps)
            End With

            If (Me.utchpInformationHub.Enabled) Then
                For Each grid As UltraGrid In _grids.Values
                    AddHandler grid.GestureQueryStatus, AddressOf OnGestureQueryStatus
                Next
            End If
        Catch ex As Exception
#If DEBUG Or CONFIG = "Debug" Then
            Throw
#End If
            If (_comparisonMapperList Is Nothing) Then

                MessageBoxEx.ShowError(String.Format(InformationHubStrings.ErrorFillGridData_Msg, vbCrLf, ex.Message))

                _logEventArgs.LogLevel = LogEventArgs.LoggingLevel.Error
                _logEventArgs.LogMessage = String.Format(InformationHubStrings.ProblemFillGridData_LogMsg, ex.Message)

                OnLogMessage(_logEventArgs)
            Else
                MessageBoxEx.ShowError(String.Format(InformationHubStrings.ErrorFillCompGridData_Msg, vbCrLf, ex.Message))
            End If

            Return False
        End Try

        If (_comparisonMapperList Is Nothing) Then
            If (_generalSettings?.E3ApplicationHighlightHooksEnabled) Then
                Me.timerE3Application.Start()
            End If

            _logEventArgs.LogLevel = LogEventArgs.LoggingLevel.Information
            _logEventArgs.LogMessage = String.Format(InformationHubStrings.FinishedLoadKBL_LogMsg, Now)

            OnLogMessage(_logEventArgs)
        End If

        Return True
    End Function

    Private Class KblDeserializationWorkarounds

        Private _events As XmlDeserializationEvents

        Private _unknownElements As New List(Of String)
        Private _unknownNodes As New List(Of String)
        Private _UnknownAttributes As New List(Of String)

        Public Sub New()
            _events = New XmlDeserializationEvents()
            _events.OnUnknownAttribute = AddressOf OnUnknownAttribute
            _events.OnUnknownElement = AddressOf OnUnknownElement
            _events.OnUnknownNode = AddressOf OnUnknownNode
            _events.OnUnreferencedObject = AddressOf OnUnreferencedObject
        End Sub

        Private Sub OnUnreferencedObject(sender As Object, e As UnreferencedObjectEventArgs)
        End Sub

        Private Sub OnUnknownAttribute(sender As Object, e As XmlAttributeEventArgs)
            _UnknownAttributes.Add(e.Attr.Name)
        End Sub

        Private Sub OnUnknownNode(sender As Object, e As XmlNodeEventArgs)
            _unknownNodes.Add(e.Name)
        End Sub

        Private Sub OnUnknownElement(sender As Object, e As XmlElementEventArgs)
            _unknownElements.Add(e.Element.LocalName)
        End Sub

        Public ReadOnly Property Events As XmlDeserializationEvents
            Get
                Return _events
            End Get
        End Property
    End Class

    Friend Function LoadKBLData(s As System.IO.Stream) As KblMapper
        Using xmlReader As System.Xml.XmlReader = System.Xml.XmlReader.Create(s)
            Return LoadKBLData(xmlReader)
        End Using
    End Function

    Friend Function LoadKBLData(reader As System.Xml.XmlReader) As KblMapper
        Try
            Return New KblMapper(KBL_container.Read(reader))
        Catch ex As Exception
#If DEBUG Or CONFIG = "Debug" Then
            Throw
#Else
            DirectCast(Me.ParentForm, DocumentForm).MainForm.AppInitManager.ErrorManager.ShowMsgBoxExclamtionOrWriteConsole(String.Format("{0}{1}{2}", ex.Message, vbCrLf, If(ex.InnerException IsNot Nothing, ex.InnerException.Message, String.Empty)), ErrorCodes.ERROR_EA_FILE_CORRUPT)
#End If
            Return Nothing
        End Try
    End Function

    Friend Function LoadKBLData(kblXDocument As XDocument) As KblMapper
        Using xmlReader As System.Xml.XmlReader = kblXDocument.CreateReader()
            Return LoadKBLData(xmlReader)
        End Using
    End Function

    Friend Sub CanvasSelectionChangedSyncRowsAction(raiseSelectionChangedEventForModules As Boolean, selection As CanvasSelection)
        ClearSelectedRowsInGrids()
        BeginGridUpdate()

        If TypeOf _parentForm Is DocumentForm Then ' BEWARE: _parentForm can also be the CompareForm here -> checking the document-form-property would not correctly reflect the old trunk BL
            TryCast(Me.DocumentForm, DocumentForm)?._modulesHub.SelectionChangedFromDocumentForm(New List(Of String))
        End If

        Dim idsToSelectExtra As New List(Of String)

        'HINT: remove tapings moved into CanvasSelection-object-constructor

        If _generalSettings.ObjectTypeDependentSelection Then
            Select Case utcInformationHub.ActiveTab.Key
                Case TabNames.tabModules.ToString
                    idsToSelectExtra.AddRange(SelectModulesInGrid(selection))
                Case TabNames.tabComponents.ToString
                    idsToSelectExtra.AddRange(SelectComponentsInGrid(selection))
                Case TabNames.tabConnectors.ToString
                    If selection.HasWires Then
                        idsToSelectExtra.AddRange(SelectCavitiesInGrid(selection))
                    End If
                Case TabNames.tabCables.ToString
                    idsToSelectExtra.AddRange(SelectCablesInGrid(selection))
                Case TabNames.tabWires.ToString
                    idsToSelectExtra.AddRange(SelectWiresInGrid(selection))
                Case TabNames.tabNets.ToString
                    idsToSelectExtra.AddRange(SelectNetsInGrid(selection))
                Case TabNames.tabRedlinings.ToString
                    idsToSelectExtra.AddRange(SelectRedliningsInGrid(selection))
            End Select
        Else
            _informationHubEventArgs.ObjectIds.NewOrClear
            _informationHubEventArgs.ObjectType = KblObjectType.Undefined
            _informationHubEventArgs.KblMapperSourceId = _kblMapper.Id

            'HINT: see TypeoOf _parentForm is DocumentForm - explanation at the beginning of this method
            If (TypeOf _parentForm Is DocumentForm) AndAlso (TryCast(Me.DocumentForm, DocumentForm)?.ActiveDrawingCanvas?.StampSelected).GetValueOrDefault AndAlso (selection.StampIds.Length > 0) Then
                idsToSelectExtra.AddRange(SelectRowsInGrids(selection.StampIds, False, Not _generalSettings.ObjectTypeDependentSelection))
            Else
                idsToSelectExtra.AddRange(SelectRowsInGrids(selection.AllKblIds.ToList, False, Not _generalSettings.ObjectTypeDependentSelection))
            End If
        End If

        If TypeOf _parentForm Is DocumentForm Then
            For Each drawingTab As UltraTab In DirectCast(_parentForm, DocumentForm).utcDocument.Tabs
                SelectInDrawing(drawingTab, selection)
            Next
        End If

        Dim kblIds As String() = Array.Empty(Of String)
        If DocumentForm IsNot Nothing Then
            kblIds = selection.AllKblIds.Concat(idsToSelectExtra).Distinct.ToArray
            If selection.Source = CanvasSelection.CanvasSelectionSource.From2D Then
                TryCast(DocumentForm, DocumentForm)?.SelectIn3DView(kblIds)
            End If
        End If

        If TryCast(DocumentForm, DocumentForm)?.MainForm IsNot Nothing Then
            TryCast(DocumentForm, DocumentForm)?.MainForm?.SelectSchematicsEntities(CType(DocumentForm, DocumentForm).CreateEdbId(kblIds).ToArray)
        End If

        EndGridUpdate()

        If raiseSelectionChangedEventForModules AndAlso (_informationHubEventArgs.ObjectType = KblObjectType.Harness_module) Then
            OnHubSelectionChanged()
        End If
    End Sub

    Private Sub SelectInDrawing(drawingTab As UltraTab, selection As CanvasSelection)
        If (drawingTab.Visible) AndAlso (Not drawingTab.Active OrElse (TryCast(Me.DocumentForm, DocumentForm)?.MainForm?.D3D.Consolidated IsNot Nothing AndAlso CType(Me.DocumentForm, DocumentForm).MainForm.D3D.Consolidated.IsSelecting)) AndAlso (drawingTab.TabPage.Controls.Count <> 0) Then
            If TypeOf drawingTab.TabPage.Controls(0) Is DrawingCanvas Then
                'HINT: MR cumbersome deselect...
                DirectCast(drawingTab.TabPage.Controls(0), DrawingCanvas).InformationHubSelectionChanged(New List(Of String), KblObjectType.Undefined, True)

                For Each kblObjectTypeIdMapping As KeyValuePair(Of KblObjectType, HashSet(Of String)) In selection
                    DirectCast(drawingTab.TabPage.Controls(0), DrawingCanvas).InformationHubSelectionChanged(kblObjectTypeIdMapping.Value.ToList, kblObjectTypeIdMapping.Key, False, issueIds:=selection.IssueIds, stampIds:=selection.StampIds)
                Next
            End If
        End If
    End Sub

    Friend Sub CloseContextMenus()
        If (_contextMenuGrid?.IsOpen).GetValueOrDefault Then
            _contextMenuGrid.ClosePopup()
        End If

        If (_contextMenuTab?.IsOpen).GetValueOrDefault Then
            _contextMenuTab.ClosePopup()
        End If
    End Sub

    Friend Function ExportAllToExcel(fileName As String, ByRef progressBar As UltraWinProgressBar.UltraProgressBar, Optional diffPropsData As IEnumerable(Of Compare.Table.ColumnView) = Nothing) As Boolean
        Dim success As Boolean = False

        Try
            Dim activeTab As UltraTab = Me.utcInformationHub.ActiveTab
            Dim workbook As Infragistics.Documents.Excel.Workbook = Nothing
            _exportOnlyActiveSelectedRows = False
            _progressBar = progressBar

            Select Case KnownFile.GetFileType(fileName)
                Case KnownFile.Type.XLS
                    workbook = New Infragistics.Documents.Excel.Workbook(Infragistics.Documents.Excel.WorkbookFormat.Excel97To2003)
                Case KnownFile.Type.XLSX
                    workbook = New Infragistics.Documents.Excel.Workbook(Infragistics.Documents.Excel.WorkbookFormat.Excel2007)
                Case Else
                    Throw New NotSupportedException($"File format not supported ""{Path.GetExtension(fileName)}"" ")
            End Select

            Me.ugeeInformationHub.ExportFormattingOptions = ExcelExport.ExportFormattingOptions.ColumnHeader

            For Each tab As UltraTab In Me.utcInformationHub.Tabs
                If (tab.Text <> KblObjectType.Redlining.ToLocalizedPluralString) AndAlso (tab.Text <> KblObjectType.QMStamp.ToLocalizedPluralString) Then
                    Me.utcInformationHub.ActiveTab = tab

                    If tab.Visible Then
                        ToggleColumnVisibility(NameOf(InformationHubStrings.AddLengthInfo_ColumnCaption), DirectCast(tab.TabPage.Controls(0), UltraGrid), False)
                        ToggleColumnVisibility(NameOf(InformationHubStrings.AssignedModules_ColumnCaption), DirectCast(tab.TabPage.Controls(0), UltraGrid), False)

                        If _kblMapperForCompare IsNot Nothing Then
                            ToggleColumnVisibility(CompareCommentColumnKey, DirectCast(tab.TabPage.Controls(0), UltraGrid), hidden:=False)
                        End If

                        Me.ugeeInformationHub.Export(DirectCast(tab.TabPage.Controls(0), UltraGrid), workbook.Worksheets.Add(Replace(tab.Text, "/", "_")))
                        If _kblMapperForCompare IsNot Nothing Then
                            ToggleColumnVisibility(CompareCommentColumnKey, DirectCast(tab.TabPage.Controls(0), UltraGrid), hidden:=True)
                        End If

                        ToggleColumnVisibility(NameOf(InformationHubStrings.AddLengthInfo_ColumnCaption), DirectCast(tab.TabPage.Controls(0), UltraGrid), True)
                        ToggleColumnVisibility(NameOf(InformationHubStrings.AssignedModules_ColumnCaption), DirectCast(tab.TabPage.Controls(0), UltraGrid), True)
                    End If
                End If
            Next

            If Not (diffPropsData Is Nothing OrElse Not diffPropsData.Any()) Then
                Me.ugDifferences.DataSource = diffPropsData
                ugePropDifferences.Export(Me.ugDifferences, workbook.Worksheets.Add(InformationHubStrings.ComparableWorksheet_Name))
            End If

            workbook.Save(fileName)

            If _displayValueExceedsExcelCellLimitWarning Then
                MessageBoxEx.ShowWarning(InformationHubStrings.WarningExcelCellLimit_Msg)
                _displayValueExceedsExcelCellLimitWarning = False
            End If

            If MessageBoxEx.ShowQuestion(InformationHubStrings.ExportExcelFinished_Msg) = System.Windows.Forms.DialogResult.Yes Then
                ProcessEx.Start(fileName)
            End If

            Me.utcInformationHub.ActiveTab = activeTab

            success = True
        Catch ex As Exception
#If DEBUG Or CONFIG = "Debug" Then
            Throw
#Else
            ex.ShowMessageBox(String.Format(InformationHubStrings.ErrorExportExcel_Msg, vbCrLf, ex.Message))
#End If

            _logEventArgs.LogLevel = LogEventArgs.LoggingLevel.Error
            _logEventArgs.LogMessage = String.Format(InformationHubStrings.ProblemExportExcel_LogMsg, ex.Message)

            OnLogMessage(_logEventArgs)
        Finally
            _progressBar = Nothing
        End Try

        Return success
    End Function

    Friend Sub ExportSelectedGridDataToExcel()
        If (ActiveGrid.Selected.Rows.Count <> 0) Then
            If MessageBoxEx.ShowQuestion(InformationHubStrings.ExportSelRows_Msg) = MsgBoxResult.Yes Then
                _exportOnlyActiveSelectedRows = True
            Else
                _exportOnlyActiveSelectedRows = False
            End If
        End If

        Using sfdExcel As New SaveFileDialog
            With sfdExcel
                sfdExcel.DefaultExt = KnownFile.XLSX.Trim("."c)

                If (_kblMapper.GetChanges.Any) Then
                    sfdExcel.FileName = String.Format("{0}{1}{2}_{3}_{4}_{5}.xlsx", Now.Year, Format(Now.Month, "00"), Format(Now.Day, "00"), Replace(_kblMapper.HarnessPartNumber, " ", String.Empty), _kblMapper.GetChanges.Max(Function(change) change.Id), Replace(Me.utcInformationHub.ActiveTab.Text, "/", "_"))
                Else
                    sfdExcel.FileName = String.Format("{0}{1}{2}_{3}_{4}.xlsx", Now.Year, Format(Now.Month, "00"), Format(Now.Day, "00"), Replace(_kblMapper.HarnessPartNumber, " ", String.Empty), Replace(Me.utcInformationHub.ActiveTab.Text, "/", "_"))
                End If

                .Filter = "Excel files (*.xlsx)|*.xlsx|Excel files (97-2003) (*.xls)|*.xls"
                .Title = InformationHubStrings.ExportSelRowsToExcelFile_Title

                If sfdExcel.ShowDialog(Me) = DialogResult.OK Then
                    Try
                        Dim workbook As Infragistics.Documents.Excel.Workbook = Nothing

                        If KnownFile.IsXlsx(sfdExcel.FileName) Then
                            workbook = New Infragistics.Documents.Excel.Workbook(Infragistics.Documents.Excel.WorkbookFormat.Excel2007)
                        Else
                            workbook = New Infragistics.Documents.Excel.Workbook(Infragistics.Documents.Excel.WorkbookFormat.Excel97To2003)
                        End If

                        Using _parentForm.EnableWaitCursor

                            ToggleColumnVisibility(NameOf(InformationHubStrings.AddLengthInfo_ColumnCaption), ActiveGrid, False)
                            ToggleColumnVisibility(NameOf(InformationHubStrings.AssignedModules_ColumnCaption), ActiveGrid, False)

                            Dim nonSelectedParentRows As New List(Of UltraGridRow)

                            If _exportOnlyActiveSelectedRows Then
                                For Each row As UltraGridRow In ActiveGrid.Selected.Rows
                                    If (row.HasParent) AndAlso (row.ParentRow.Appearance.ForeColor <> Color.Gray) Then
                                        row.ParentRow.Appearance.ForeColor = Color.Gray
                                        nonSelectedParentRows.Add(row.ParentRow)
                                    End If
                                Next
                            End If

                            Me.ugeeInformationHub.ExportFormattingOptions = ExcelExport.ExportFormattingOptions.ColumnHeader
                            Me.ugeeInformationHub.Export(Me.ActiveGrid, workbook.Worksheets.Add(Replace(Me.utcInformationHub.ActiveTab.Text, "/", "_")))

                            workbook.Save(.FileName)

                            For Each row As UltraGridRow In nonSelectedParentRows
                                row.Appearance.ForeColor = Color.Black
                            Next

                            ToggleColumnVisibility(NameOf(InformationHubStrings.AddLengthInfo_ColumnCaption), ActiveGrid, True)
                            ToggleColumnVisibility(NameOf(InformationHubStrings.AssignedModules_ColumnCaption), ActiveGrid, True)

                            If _displayValueExceedsExcelCellLimitWarning Then
                                MessageBoxEx.ShowWarning(InformationHubStrings.WarningExcelCellLimit_Msg)
                                _displayValueExceedsExcelCellLimitWarning = False
                            End If
                        End Using

                        If MessageBoxEx.ShowQuestion(InformationHubStrings.ExportExcelFinished_Msg) = System.Windows.Forms.DialogResult.Yes Then
                            ProcessEx.Start(.FileName)
                        End If
                    Catch ex As Exception
                        ex.ShowMessageBox(String.Format(InformationHubStrings.ErrorExportExcel_Msg, vbCrLf, ex.Message))

                        _logEventArgs.LogLevel = LogEventArgs.LoggingLevel.Error
                        _logEventArgs.LogMessage = String.Format(InformationHubStrings.ProblemExportExcel_LogMsg, ex.Message)

                        OnLogMessage(_logEventArgs)
                    End Try
                End If
            End With
        End Using
    End Sub

    Friend ReadOnly Property CurrentRowFilterInfo As RowFilterInfo
        Get
            Return _currentRowFilterInfo
        End Get
    End Property

    Friend ReadOnly Property RowFiltes As RowFiltersCollection
        Get
            Return _rowFilters
        End Get
    End Property

    Friend Function FilterActiveObjects(inactiveModules As Dictionary(Of String, E3.Lib.Schema.Kbl.[Module]), hideEntitiesWithNoModules As Boolean) As Boolean
        ClearBoldTabAppearances()
        _rowFilters.Refresh(inactiveModules, hideEntitiesWithNoModules)
        Return True
    End Function

    Private Function GetSealsOnPlugReplacements() As Dictionary(Of String, List(Of String))
        'HINT id = plugId, list of seals ids which replace plug
        Dim sealsOnPlugReplacements As New Dictionary(Of String, List(Of String))
        For Each seal As Cavity_seal_occurrence In _kblMapper.GetCavitySealOccurrences
            If seal.Replacing IsNot Nothing AndAlso seal.Replacing.Length > 0 Then
                For Each plg As Part_substitution In seal.Replacing
                    If Not String.IsNullOrEmpty(plg.Replaced) Then
                        sealsOnPlugReplacements.GetOrAddNew(plg.Replaced).Add(seal.SystemId)
                    End If
                Next
            End If
        Next
        Return sealsOnPlugReplacements
    End Function

    Friend Function FilterAnalysisViewObjects(activeObjects As ICollection(Of String)) As String()
        'TODO: Change to RowFilters to enable original intended Business logic for OnDemandLoading -> iterating over all grids and rows makes this implementation (done under massive costs: GetCellValue-Event instead of using easy-DataBinding) obsolete
        Dim filteredKblIds As New HashSet(Of String)
        ClearMarkedRowsInGrids()
        ClearSelectedRowsInGrids()
        BeginGridUpdate()

        For Each grid As KeyValuePair(Of String, UltraGrid) In _grids
            If Not grid.Key = KblObjectType.Harness.ToString AndAlso Not grid.Key = KblObjectType.Module.ToString Then
                For Each row As UltraGridRow In grid.Value.Rows
                    row.Appearance.BackColor = Color.White
                    If row.HasChild Then
                        For Each childRow As UltraGridRow In row.ChildBands(0).Rows
                            childRow.Appearance.BackColor = Color.White
                        Next
                    End If
                Next
            End If
        Next

        If activeObjects IsNot Nothing Then
            Dim activeGrids As New List(Of String)
            For Each grid As KeyValuePair(Of String, UltraGrid) In _grids
                If (Not grid.Key = KblObjectType.Harness.ToString) AndAlso (Not grid.Key = KblObjectType.Module.ToString) Then
                    Dim activeRowCounter As Integer = 0

                    For Each row As UltraGridRow In grid.Value.Rows
                        Dim objectId As String = If(grid.Key = KblObjectType.Redlining.ToString, row.Cells("ObjectId").Value.ToString, row.Tag?.ToString)
                        If (grid.Key = KblObjectType.Redlining.ToString AndAlso Not activeObjects.Contains(objectId)) OrElse (grid.Key <> KblObjectType.Redlining.ToString AndAlso Not activeObjects.Contains(objectId)) Then
                            row.Appearance.BackColor = Color.FromArgb(190, 190, 190)
                            filteredKblIds.Add(objectId)
                        Else
                            grid.Value.Rows.Move(row, activeRowCounter)
                            activeRowCounter += 1
                            activeGrids.TryAdd(grid.Key)
                        End If

                        If row.HasChild Then
                            For Each childRow As UltraGridRow In row.ChildBands(0).Rows
                                If row.Appearance.BackColor = Color.FromArgb(190, 190, 190) AndAlso Not activeObjects.Contains(childRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString) Then
                                    childRow.Appearance.BackColor = Color.FromArgb(190, 190, 190)
                                    filteredKblIds.Add(childRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString)
                                End If
                            Next
                        End If
                    Next

                    If activeRowCounter <> 0 Then
                        grid.Value.ActiveRowScrollRegion.ScrollRowIntoView(grid.Value.Rows(0))
                    End If
                End If
            Next

            If activeGrids.Count > 0 Then
                BringTabIntoViewCore(CType(_grids(activeGrids.First).Parent, UltraTabPageControl).Tab.Key)
            End If
        End If

        EndGridUpdate()
        Return filteredKblIds.ToArray
    End Function

    Friend Sub ImportValidityCheckResults()
        BeginGridUpdate()

        For Each grid As UltraGrid In _grids.Values
            If grid.CreationFilter IsNot Nothing Then
                grid.CreationFilter = New ValidityCheckCreationFilter(_parentForm)
            End If
        Next

        EndGridUpdate()
    End Sub

    Friend Function SelectRowsInGrids(ids As ICollection(Of String), removePreviousSelection As Boolean, singleObjectType As Boolean, Optional selectIn3DView As Boolean = True, Optional highlightInConnectivityView As Boolean = True) As List(Of String)
        Dim selected As New List(Of String)

        If Not _isHubSelecting Then ' HINT: call from within hubSelection (f.e. user clicked informationHubRow directly is ignored to avoid recursive calls)
            If removePreviousSelection Then
                ClearSelectedRowsInGrids()
            End If

            SetInformationHubEventArgs(ids, Nothing, Nothing)

            Dim tabIsInView As Boolean = False
            For Each id As String In ids
                Dim kblOcc As IKblOccurrence = _kblMapper.GetOccurrenceObjectUntyped(id)
                If kblOcc IsNot Nothing Then
                    Select Case kblOcc.ObjectType
                        Case KblObjectType.Accessory_occurrence
                            If singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabAccessories.ToString) Then
                                tabIsInView = BringTabIntoView(TabNames.tabAccessories.ToString, singleObjectType, tabIsInView)
                                selected.AddRange(SelectKblIdRowsInGrid(_grids(KblObjectType.Accessory_occurrence.ToString), id))
                            End If
                        Case KblObjectType.Approval
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabApprovals.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabApprovals.ToString, singleObjectType, tabIsInView)
                                selected.AddRange(SelectKblIdRowsInGrid(_grids(KblObjectType.Approval.ToString), id))
                            End If
                        Case KblObjectType.Assembly_part_occurrence
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabAssemblyParts.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabAssemblyParts.ToString, singleObjectType, tabIsInView)
                                selected.AddRange(SelectKblIdRowsInGrid(_grids(KblObjectType.Assembly_part_occurrence.ToString), id))
                            End If
                        Case KblObjectType.Cavity_occurrence, KblObjectType.Contact_point
                            If singleObjectType OrElse Me.utcInformationHub.ActiveTab?.Key = TabNames.tabConnectors.ToString Then
                                tabIsInView = BringTabIntoView(TabNames.tabConnectors.ToString, singleObjectType, tabIsInView)
                                selected.AddRange(SelectKblIdRowsInGrid(_grids(KblObjectType.Connector_occurrence.ToString), id)) 'we get the connector back
                            End If
                        Case KblObjectType.Change
                            If (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabModules.ToString) Then
                                tabIsInView = BringTabIntoView(TabNames.tabModules.ToString, singleObjectType, tabIsInView)
                                selected.AddRange(SelectKblIdRowsInGrid(_grids(KblObjectType.Module.ToString), id))
                            End If
                        Case KblObjectType.Change_description
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabChangeDescriptions.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabChangeDescriptions.ToString, singleObjectType, tabIsInView)
                                selected.AddRange(SelectKblIdRowsInGrid(_grids(KblObjectType.Change_description.ToString), id))
                            End If
                        Case KblObjectType.Component_box_occurrence
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabComponentBoxes.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabComponentBoxes.ToString, singleObjectType, tabIsInView)
                                selected.AddRange(SelectKblIdRowsInGrid(_grids(KblObjectType.Component_box_occurrence.ToString), id))
                            End If
                        Case KblObjectType.Component_occurrence
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabNets.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabNets.ToString, singleObjectType, tabIsInView)
                                selected.AddRange(SelectKblIdRowsInGrid(_grids(KblObjectType.Component_occurrence.ToString), id))
                            End If
                        Case KblObjectType.Connection
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabNets.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabNets.ToString, singleObjectType, tabIsInView)
                                selected.AddRange(SelectKblIdRowsInGrid(_grids(KblObjectType.Net.ToString), DirectCast(_kblMapper.KBLOccurrenceMapper(id), Connection).Wire))
                            End If
                        Case KblObjectType.Connector_occurrence
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabConnectors.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabConnectors.ToString, singleObjectType, tabIsInView)
                                selected.AddRange(SelectKblIdRowsInGrid(_grids(KblObjectType.Connector_occurrence.ToString), id))
                            End If
                        Case KblObjectType.Co_pack_occurrence
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabCoPacks.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabCoPacks.ToString, singleObjectType, tabIsInView)
                                selected.AddRange(SelectKblIdRowsInGrid(_grids(KblObjectType.Co_pack_occurrence.ToString), id))
                            End If
                        Case KblObjectType.Core_occurrence
                            If (singleObjectType OrElse Me.utcInformationHub.ActiveTab?.Key = TabNames.tabWires.ToString) Then
                                tabIsInView = BringTabIntoView(TabNames.tabWires.ToString, singleObjectType, tabIsInView)
                                selected.AddRange(SelectKblIdRowsInGrid(_grids(KblObjectType.Wire_occurrence.ToString), id))
                            End If
                        Case KblObjectType.Default_dimension_specification
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabDefDimSpecs.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabDefDimSpecs.ToString, singleObjectType, tabIsInView)
                                selected.AddRange(SelectKblIdRowsInGrid(_grids(KblObjectType.Default_dimension_specification.ToString), id))
                            End If
                        Case KblObjectType.Dimension_specification
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabDimSpecs.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabDimSpecs.ToString, singleObjectType, tabIsInView)
                                selected.AddRange(SelectKblIdRowsInGrid(_grids(KblObjectType.Dimension_specification.ToString), id))
                            End If
                        Case KblObjectType.Fixing_occurrence
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabFixings.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabFixings.ToString, singleObjectType, tabIsInView)
                                selected.AddRange(SelectKblIdRowsInGrid(_grids(KblObjectType.Fixing_occurrence.ToString), id))
                            End If
                        Case KblObjectType.Module
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabModules.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabModules.ToString, singleObjectType, tabIsInView)
                                selected.AddRange(SelectKblIdRowsInGrid(_grids(KblObjectType.Module.ToString), id))
                            End If
                        Case KblObjectType.Node
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabVertices.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabVertices.ToString, singleObjectType, tabIsInView)
                                selected.AddRange(SelectKblIdRowsInGrid(_grids(KblObjectType.Node.ToString), id))
                            End If
                        Case KblObjectType.Segment
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabSegments.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabSegments.ToString, singleObjectType, tabIsInView)
                                selected.AddRange(SelectKblIdRowsInGrid(_grids(KblObjectType.Segment.ToString), id))
                            End If
                        Case KblObjectType.Special_wire_occurrence
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabCables.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabCables.ToString, singleObjectType, tabIsInView)
                                selected.AddRange(SelectKblIdRowsInGrid(_grids(KblObjectType.Special_wire_occurrence.ToString), id))
                            End If
                        Case KblObjectType.Wire_occurrence
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabWires.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabWires.ToString, singleObjectType, tabIsInView)
                                selected.AddRange(SelectKblIdRowsInGrid(_grids(KblObjectType.Wire_occurrence.ToString), id))
                            End If
                        Case KblObjectType.Wire_protection_occurrence
                            If (_kblMapper.GetSegments.Any(Function(segment) segment.Protection_area IsNot Nothing AndAlso segment.Protection_area.Any(Function(protectionArea) protectionArea.Associated_protection = id))) Then
                                If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabSegments.ToString)) Then
                                    tabIsInView = BringTabIntoView(TabNames.tabSegments.ToString, singleObjectType, tabIsInView)
                                    selected.AddRange(SelectKblIdRowsInGrid(_grids(KblObjectType.Segment.ToString), id))
                                End If
                            Else
                                If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabVertices.ToString)) Then
                                    tabIsInView = BringTabIntoView(TabNames.tabVertices.ToString, singleObjectType, tabIsInView)
                                    selected.AddRange(SelectKblIdRowsInGrid(_grids(KblObjectType.Node.ToString), id))
                                End If
                            End If
                        Case KblObjectType.Cavity_plug_occurrence, KblObjectType.Cavity_seal_occurrence, KblObjectType.Special_terminal_occurrence, KblObjectType.Terminal_occurrence
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabConnectors.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabConnectors.ToString, singleObjectType, tabIsInView)
                                selected.AddRange(SelectKblIdRowsInGrid(_grids(KblObjectType.Connector_occurrence.ToString), id))
                            End If
                    End Select
                ElseIf (_kblMapper.KBLNetList.Contains(id)) AndAlso (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabNets.ToString)) Then
                    tabIsInView = BringTabIntoView(TabNames.tabNets.ToString, singleObjectType, tabIsInView)
                    selected.AddRange(SelectKblIdRowsInGrid(_grids(KblObjectType.Net.ToString), id))
                ElseIf (ugQMStamps.Rows.Where(Function(row) row.Tag IsNot Nothing AndAlso row.Tag?.ToString = id).Any()) AndAlso (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabQMStamps.ToString)) Then
                    tabIsInView = BringTabIntoView(TabNames.tabQMStamps.ToString, singleObjectType, tabIsInView)
                    selected.AddRange(SelectKblIdRowsInGrid(_grids(KblObjectType.QMStamp.ToString), id))
                ElseIf (ugRedlinings.Rows.Where(Function(row) row.Tag IsNot Nothing AndAlso row.Tag?.ToString = id).Any()) AndAlso (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabRedlinings.ToString)) Then
                    tabIsInView = BringTabIntoView(TabNames.tabRedlinings.ToString, singleObjectType, tabIsInView)
                    selected.AddRange(SelectKblIdRowsInGrid(_grids(KblObjectType.Redlining.ToString), id))
                End If
            Next

            If (removePreviousSelection) AndAlso (_informationHubEventArgs.ObjectIds IsNot Nothing) Then
                _informationHubEventArgs.SelectIn3DView = selectIn3DView
                _informationHubEventArgs.HighlightInConnectivityView = highlightInConnectivityView
                OnHubSelectionChanged()
            End If
        End If

        Return selected.Where(Function(selRow) selRow IsNot Nothing).ToList
    End Function

    Private Function BringTabIntoView(bringTabIntoViewKey As String, singleObjectType As Boolean, tabIsInView As Boolean) As Boolean
        If Not String.IsNullOrEmpty(bringTabIntoViewKey) AndAlso (singleObjectType) AndAlso (Not tabIsInView) Then
            Return BringTabIntoViewCore(bringTabIntoViewKey)
        End If
        Return False
    End Function

    Friend Function SelectRowsInCompareHubGrids(ids As List(Of String), kblMapper As KblMapper, changeType As String, removePreviousSelection As Boolean, singleObjectType As Boolean, Optional selectIn3DView As Boolean = True, Optional highlightInConnectivityView As Boolean = True) As List(Of String)
        Dim selected As New List(Of String)

        If Not _isHubSelecting Then ' HINT: call from within hubSelection (f.e. user clicked informationHubRow directly is ignored to avoid recursive calls)
            If removePreviousSelection Then
                ClearSelectedRowsInGrids()
            End If

            SetInformationCompareHubEventArgs(ids, kblMapper, Nothing)

            Dim tabIsInView As Boolean = False
            For Each id As String In ids
                Dim occ As IKblOccurrence = kblMapper.GetOccurrenceObjectUntyped(id)
                If occ IsNot Nothing Then
                    Select Case occ.ObjectType
                        Case KblObjectType.Accessory_occurrence
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabAccessories.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabAccessories.ToString, singleObjectType, tabIsInView)
                                selected.Add(SelectRowInCompareHubGrid(_grids(KblObjectType.Accessory_occurrence.ToString), id, changeType))
                            End If
                        Case KblObjectType.Approval
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabApprovals.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabApprovals.ToString, singleObjectType, tabIsInView)
                                selected.Add(SelectRowInCompareHubGrid(_grids(KblObjectType.Approval.ToString), id, changeType))
                            End If
                        Case KblObjectType.Assembly_part_occurrence
                            If (singleObjectType OrElse (Me.utcInformationHub?.ActiveTab?.Key = TabNames.tabAssemblyParts.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabAssemblyParts.ToString, singleObjectType, tabIsInView)
                                selected.Add(SelectRowInCompareHubGrid(_grids(KblObjectType.Assembly_part_occurrence.ToString), id, changeType))
                            End If
                        Case KblObjectType.Cavity_occurrence
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabConnectors.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabConnectors.ToString, singleObjectType, tabIsInView)
                                selected.Add(SelectRowInCompareHubGrid(_grids(KblObjectType.Connector_occurrence.ToString), id, changeType)) 'we get the connector back
                            End If
                        Case KblObjectType.Contact_point
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabConnectors.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabConnectors.ToString, singleObjectType, tabIsInView)
                                selected.Add(SelectRowInCompareHubGrid(_grids(KblObjectType.Connector_occurrence.ToString), id, changeType)) 'we get the connector back
                            End If
                        Case KblObjectType.Cavity_plug_occurrence, KblObjectType.Cavity_seal_occurrence, KblObjectType.Special_terminal_occurrence, KblObjectType.Terminal_occurrence
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabConnectors.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabConnectors.ToString, singleObjectType, tabIsInView)
                                selected.Add(SelectRowInCompareHubGrid(_grids(KblObjectType.Connector_occurrence.ToString), id, changeType))
                            End If
                        Case KblObjectType.Change
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabModules.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabModules.ToString, singleObjectType, tabIsInView)
                                selected.Add(SelectRowInCompareHubGrid(_grids(KblObjectType.Module.ToString), id, changeType))
                            End If
                        Case KblObjectType.Change_description
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabChangeDescriptions.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabChangeDescriptions.ToString, singleObjectType, tabIsInView)
                                selected.Add(SelectRowInCompareHubGrid(_grids(KblObjectType.Change_description.ToString), id, changeType))
                            End If
                        Case KblObjectType.Component_box_occurrence
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabComponentBoxes.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabComponentBoxes.ToString, singleObjectType, tabIsInView)
                                selected.Add(SelectRowInCompareHubGrid(_grids(KblObjectType.Component_box_occurrence.ToString), id, changeType))
                            End If
                        Case KblObjectType.Component_occurrence
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabComponents.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabComponents.ToString, singleObjectType, tabIsInView)
                                selected.Add(SelectRowInCompareHubGrid(_grids(KblObjectType.Component_occurrence.ToString), id, changeType))
                            End If
                        Case KblObjectType.Connection
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabNets.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabNets.ToString, singleObjectType, tabIsInView)
                                selected.Add(SelectRowInCompareHubGrid(_grids(KblObjectType.Net.ToString), DirectCast(kblMapper.KBLOccurrenceMapper(id), Connection).Wire, changeType))
                            End If
                        Case KblObjectType.Connector_occurrence
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabConnectors.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabConnectors.ToString, singleObjectType, tabIsInView)
                                selected.Add(SelectRowInCompareHubGrid(_grids(KblObjectType.Connector_occurrence.ToString), id, changeType))
                            End If
                        Case KblObjectType.Co_pack_occurrence
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabCoPacks.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabCoPacks.ToString, singleObjectType, tabIsInView)
                                selected.Add(SelectRowInCompareHubGrid(_grids(KblObjectType.Co_pack_occurrence.ToString), id, changeType))
                            End If
                        Case KblObjectType.Core_occurrence
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabWires.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabWires.ToString, singleObjectType, tabIsInView)
                                selected.Add(SelectRowInCompareHubGrid(_grids(KblObjectType.Wire_occurrence.ToString), id, changeType))
                            End If
                        Case KblObjectType.Default_dimension_specification
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabDefDimSpecs.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabDefDimSpecs.ToString, singleObjectType, tabIsInView)
                                selected.Add(SelectRowInCompareHubGrid(_grids(KblObjectType.Default_dimension_specification.ToString), id, changeType))
                            End If
                        Case KblObjectType.Dimension_specification
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabDimSpecs.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabDimSpecs.ToString, singleObjectType, tabIsInView)
                                selected.Add(SelectRowInCompareHubGrid(_grids(KblObjectType.Dimension_specification.ToString), id, changeType))
                            End If
                        Case KblObjectType.Fixing_occurrence
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabFixings.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabFixings.ToString, singleObjectType, tabIsInView)
                                selected.Add(SelectRowInCompareHubGrid(_grids(KblObjectType.Fixing_occurrence.ToString), id, changeType))
                            End If
                        Case KblObjectType.Module
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabModules.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabModules.ToString, singleObjectType, tabIsInView)
                                selected.Add(SelectRowInCompareHubGrid(_grids(KblObjectType.Module.ToString), id, changeType))
                            End If
                        Case KblObjectType.Node
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabVertices.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabVertices.ToString, singleObjectType, tabIsInView)
                                selected.Add(SelectRowInCompareHubGrid(_grids(KblObjectType.Node.ToString), id, changeType))
                            End If
                        Case KblObjectType.Segment
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabSegments.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabSegments.ToString, singleObjectType, tabIsInView)
                                selected.Add(SelectRowInCompareHubGrid(_grids(KblObjectType.Segment.ToString), id, changeType))
                            End If
                        Case KblObjectType.Special_wire_occurrence
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabCables.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabCables.ToString, singleObjectType, tabIsInView)
                                selected.Add(SelectRowInCompareHubGrid(_grids(KblObjectType.Special_wire_occurrence.ToString), id, changeType))
                            End If
                        Case KblObjectType.Wire_occurrence
                            If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabWires.ToString)) Then
                                tabIsInView = BringTabIntoView(TabNames.tabWires.ToString, singleObjectType, tabIsInView)
                                selected.Add(SelectRowInCompareHubGrid(_grids(KblObjectType.Wire_occurrence.ToString), id, changeType))
                            End If
                        Case KblObjectType.Wire_protection_occurrence
                            If (kblMapper.GetSegments.Any(Function(segment) segment.Protection_area IsNot Nothing AndAlso segment.Protection_area.Any(Function(protectionArea) protectionArea.Associated_protection = id))) Then
                                If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabSegments.ToString)) Then
                                    tabIsInView = BringTabIntoView(TabNames.tabSegments.ToString, singleObjectType, tabIsInView)
                                    selected.Add(SelectRowInCompareHubGrid(_grids(KblObjectType.Segment.ToString), id, changeType))
                                End If
                            Else
                                If (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabVertices.ToString)) Then
                                    tabIsInView = BringTabIntoView(TabNames.tabVertices.ToString, singleObjectType, tabIsInView)
                                    selected.Add(SelectRowInCompareHubGrid(_grids(KblObjectType.Node.ToString), id, changeType))
                                End If
                            End If
                    End Select
                ElseIf (kblMapper.KBLNetList.Contains(id)) AndAlso (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabNets.ToString)) Then
                    tabIsInView = BringTabIntoView(TabNames.tabNets.ToString, singleObjectType, tabIsInView)
                    selected.Add(SelectRowInCompareHubGrid(_grids(KblObjectType.Net.ToString), id, changeType))
                ElseIf (ugQMStamps.Rows.Where(Function(row) row.Tag IsNot Nothing AndAlso row.Tag?.ToString = id).Any()) AndAlso (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabQMStamps.ToString)) Then
                    tabIsInView = BringTabIntoView(TabNames.tabQMStamps.ToString, singleObjectType, tabIsInView)
                    selected.Add(SelectRowInCompareHubGrid(_grids(KblObjectType.QMStamp.ToString), id, changeType))
                ElseIf (ugRedlinings.Rows.Where(Function(row) row.Tag IsNot Nothing AndAlso row.Tag?.ToString = id).Any()) AndAlso (singleObjectType OrElse (Me.utcInformationHub.ActiveTab?.Key = TabNames.tabRedlinings.ToString)) Then
                    tabIsInView = BringTabIntoView(TabNames.tabRedlinings.ToString, singleObjectType, tabIsInView)
                    selected.Add(SelectRowInCompareHubGrid(_grids(KblObjectType.Redlining.ToString), id, changeType))
                End If
            Next

            If removePreviousSelection AndAlso _informationHubEventArgs.ObjectIds IsNot Nothing Then
                _informationHubEventArgs.SelectIn3DView = selectIn3DView
                _informationHubEventArgs.HighlightInConnectivityView = highlightInConnectivityView

                OnHubSelectionChanged()
            End If
        End If

        Return selected.Where(Function(selRow) selRow IsNot Nothing).ToList
    End Function

    Private Sub AddCompareColumns(columns As UltraDataColumnsCollection, addCheckedColumns As Boolean)
        If addCheckedColumns Then
            With columns.TryAddOrGet(NameOf(InformationHubStrings.Checked_ColumnCaption))
                .DataType = GetType(Boolean)
                .ReadOnly = DefaultableBoolean.False
            End With
            With columns.TryAddOrGet(NameOf(InformationHubStrings.ToBeChanged_ColumnCaption))
                .DataType = GetType(Boolean)
                .ReadOnly = DefaultableBoolean.False
            End With
        End If

        With columns.TryAddOrGet(CompareCommentColumnKey)
            .DataType = GetType(String)
            .ReadOnly = DefaultableBoolean.False
        End With

        columns.TryAddOrGet(NameOf(InformationHubStrings.DiffType_ColumnCaption))
    End Sub

    Private Sub BeginGridUpdate()
        For Each grid As UltraGrid In _grids.Values
            grid.BeginUpdate()
        Next
    End Sub

    Private Function BringTabIntoViewCore(tabKey As String, Optional disableEventManager As Boolean = True) As Boolean
        Dim tabIsInView As Boolean = False

        Me.utcInformationHub.BeginUpdate()
        If disableEventManager Then
            Me.utcInformationHub.EventManager.AllEventsEnabled = False
        End If

        Dim previousTab As UltraTab = Me.utcInformationHub.SelectedTab

        If Me.utcInformationHub.Tabs(tabKey).Visible Then
            If (Me.utcInformationHub.SelectedTab Is Me.utcInformationHub.Tabs(tabKey)) Then
                Me.utcInformationHub.SelectedTab = Nothing
            End If

            Me.utcInformationHub.ActiveTab = Me.utcInformationHub.Tabs(tabKey)
            Me.utcInformationHub.SelectedTab = Me.utcInformationHub.Tabs(tabKey)
            Me.utcInformationHub.SelectedTab.EnsureTabInView()

            tabIsInView = True
        End If

        If disableEventManager Then
            Me.utcInformationHub.EventManager.AllEventsEnabled = True
            If tabIsInView AndAlso previousTab IsNot Me.utcInformationHub.SelectedTab Then
                OnActiveTabChanged(New ActiveTabChangedEventArgs(Me.utcInformationHub.SelectedTab, previousTab))
            End If
        End If

        Me.utcInformationHub.EndUpdate()

        Return tabIsInView
    End Function

    Private Sub CalculateDistanceBetweenSelectedObjects()
        Dim segmentTrace As New List(Of SegmentDistanceEntry)

        Dim selectedObjects As Dictionary(Of String, Object) = GetSelectedObjectsForDistanceCalculation()
        If selectedObjects.Count <> 2 Then
            Return
        End If

        Using _parentForm.EnableWaitCursor

            Dim firstNode As Node = If(TypeOf selectedObjects.Values.First Is Node, DirectCast(selectedObjects.Values.First, Node), Nothing)
            Dim firstSegment As Segment = If(TypeOf selectedObjects.Values.First Is Segment, DirectCast(selectedObjects.Values.First, Segment), Nothing)
            Dim secondNode As Node = If(TypeOf selectedObjects.Values.Last Is Node, DirectCast(selectedObjects.Values.Last, Node), Nothing)
            Dim secondSegment As Segment = If(TypeOf selectedObjects.Values.Last Is Segment, DirectCast(selectedObjects.Values.Last, Segment), Nothing)

            If (firstSegment IsNot Nothing AndAlso firstSegment Is secondSegment) Then
                'HINT: two fixings on the same segment
                Dim fixingAssignment1 As Fixing_assignment = firstSegment.Fixing_assignment.Where(Function(fixAssignment) fixAssignment.Fixing = selectedObjects.Keys.First).FirstOrDefault
                Dim fixingAssignment2 As Fixing_assignment = firstSegment.Fixing_assignment.Where(Function(fixAssignment) fixAssignment.Fixing = selectedObjects.Keys.Last).FirstOrDefault
                Dim entry As New SegmentDistanceEntry(firstSegment, fixingAssignment1.Location, fixingAssignment2.Location)
                entry.FixingIds.Add(selectedObjects.Keys.First)
                entry.FixingIds.Add(selectedObjects.Keys.Last)
                segmentTrace.Add(entry)
            Else
                Dim startNode As Node = If(firstNode, _kblMapper.GetOccurrenceObject(Of Node)(firstSegment.Start_node))
                Dim endNode As Node = If(secondNode, _kblMapper.GetOccurrenceObject(Of Node)(secondSegment.Start_node))

                If startNode IsNot Nothing AndAlso endNode IsNot Nothing Then
                    Dim nodeIds As New List(Of String)
                    For Each seg As Segment In GetSegmentsOnTheWay(startNode.SystemId, endNode.SystemId)
                        If seg IsNot firstSegment AndAlso seg IsNot secondSegment Then
                            segmentTrace.Add(New SegmentDistanceEntry(seg, 0.0, 1.0))
                            nodeIds.Add(seg.Start_node)
                            nodeIds.Add(seg.End_node)
                        End If
                    Next

                    If segmentTrace.Count = 0 Then
                        If firstSegment IsNot Nothing AndAlso secondSegment IsNot Nothing Then
                            If (firstSegment.Start_node = secondSegment.Start_node OrElse firstSegment.Start_node = secondSegment.End_node) Then
                                nodeIds.Add(firstSegment.Start_node)
                            ElseIf (firstSegment.End_node = secondSegment.Start_node OrElse firstSegment.End_node = secondSegment.End_node) Then
                                nodeIds.Add(firstSegment.End_node)
                            End If
                        ElseIf firstSegment IsNot Nothing AndAlso secondNode IsNot Nothing Then
                            nodeIds.Add(secondNode.SystemId)
                        ElseIf secondSegment IsNot Nothing AndAlso firstNode IsNot Nothing Then
                            nodeIds.Add(firstNode.SystemId)
                        End If
                    End If

                    If firstNode Is Nothing Then
                        Dim fixingAssignment As Fixing_assignment = firstSegment.Fixing_assignment.Where(Function(fixAssignment) fixAssignment.Fixing = selectedObjects.Keys.First).FirstOrDefault
                        Dim entry As New SegmentDistanceEntry(firstSegment, 0.0, 0.0)
                        entry.FixingIds.Add(fixingAssignment.Fixing)
                        segmentTrace.Add(entry)
                        If (nodeIds.Contains(firstSegment.Start_node)) Then
                            entry.Start = 0.0
                            entry.End = fixingAssignment.Location
                        Else
                            entry.Start = fixingAssignment.Location
                            entry.End = 1.0
                        End If
                    End If

                    If secondNode Is Nothing Then
                        Dim fixingAssignment As Fixing_assignment = secondSegment.Fixing_assignment.Where(Function(fixAssignment) fixAssignment.Fixing = selectedObjects.Keys.Last).FirstOrDefault
                        Dim entry As New SegmentDistanceEntry(secondSegment, 0.0, 0.0)
                        entry.FixingIds.Add(fixingAssignment.Fixing)
                        segmentTrace.Add(entry)

                        If nodeIds.Contains(secondSegment.Start_node) Then
                            entry.Start = 0.0
                            entry.End = fixingAssignment.Location
                        Else
                            entry.Start = fixingAssignment.Location
                            entry.End = 1.0
                        End If
                    End If
                Else
                    MessageBoxEx.ShowWarning(Me.ParentForm, String.Format(InformationHubStrings.Warning2CalcDist_Msg, vbCrLf))
                End If
            End If

            Dim physicalDistance As Double = 0.0
            Dim physicalDistanceUnitName As String = String.Empty
            Dim virtualDistance As Double = 0.0
            Dim virtualDistanceUnitName As String = String.Empty

            For Each entry As SegmentDistanceEntry In segmentTrace
                If (entry.Segment.Physical_length IsNot Nothing) Then
                    physicalDistance += entry.Segment.Physical_length.Value_component * entry.ActiveFraction
                    physicalDistanceUnitName = _kblMapper.KBLUnitMapper(entry.Segment.Physical_length.Unit_component).Unit_name
                End If

                If (entry.Segment.Virtual_length IsNot Nothing) Then
                    virtualDistance += entry.Segment.Virtual_length.Value_component * entry.ActiveFraction
                    virtualDistanceUnitName = _kblMapper.KBLUnitMapper(entry.Segment.Virtual_length.Unit_component).Unit_name
                End If
            Next

            Dim drawingCanvas As DrawingCanvas = Nothing
            If TypeOf _parentForm Is DocumentForm Then
                drawingCanvas = DirectCast(_parentForm, DocumentForm).ActiveDrawingCanvas
            End If

            If drawingCanvas IsNot Nothing Then
                drawingCanvas.HighlightDistanceTrace(segmentTrace)
            End If

            Dim temporalEntities As TemporalEntitiesResult = Nothing
            If TryCast(Me.DocumentForm.HcvDocument, HcvDocument)?.IsOpen Then
                temporalEntities = TryCast(Me.DocumentForm.HcvDocument, HcvDocument)?.ShowAsNewTemporalEntities(segmentTrace)
            End If

            Using temporalEntities
                MessageBoxEx.ShowInfo(Me.ParentForm, String.Format(InformationHubStrings.CalcDist_Msg, vbCrLf, Math.Round(physicalDistance, 1), physicalDistanceUnitName, Math.Round(virtualDistance, 1), virtualDistanceUnitName))

                If drawingCanvas IsNot Nothing Then
                    drawingCanvas.DeHighLightDistanceTrace(segmentTrace)
                End If
            End Using ' HINT: removes temp-entities on dispose + invalidate (Property: TemporalEntitiesResult.InvalidateAfterDispose)

        End Using
    End Sub

    Private Function GetSegmentsOnTheWay(startNodeId As String, endNodeId As String) As List(Of Segment)
        Dim segments As New List(Of Segment)
        Dim router As PathRouter(Of String) = SetUpRouterForDistanceCalculation()
        Dim pathes As PathRouter(Of String).RoutingPathes = router.Route(startNodeId, endNodeId)
        If pathes.Count = 1 Then
            For Each path As PathRouter(Of String).RoutingPath In pathes
                For Each segmentId As String In path.AdjacencyKeys
                    segments.Add(_kblMapper.GetSegments.Single(Function(seg) seg.SystemId = segmentId.ToString))
                Next
            Next
        Else
            MessageBoxEx.ShowWarning(Me.ParentForm, String.Format(InformationHubStrings.Warning1CalcDist_Msg, vbCrLf))
        End If
        Return segments
    End Function

    Private Function SetUpRouterForDistanceCalculation() As PathRouter(Of String)
        Dim router As New PathRouter(Of String)
        For Each node As Node In _kblMapper.GetNodes
            If (_currentRowFilterInfo.InactiveObjects IsNot Nothing) OrElse (Not (_currentRowFilterInfo.InactiveObjects.ContainsKey(KblObjectType.Node) AndAlso _currentRowFilterInfo.InactiveObjects(KblObjectType.Node).Contains(node.SystemId))) Then
                router.AddVertex(node.SystemId)
            End If
        Next

        For Each segment As Segment In _kblMapper.GetSegments
            If (_currentRowFilterInfo.InactiveObjects?.Count).GetValueOrDefault = 0 OrElse (_currentRowFilterInfo.InactiveObjects.ContainsValue(KblObjectType.Segment, segment.SystemId)) Then
                router.AddAdjacency(segment.SystemId, If(segment.Physical_length IsNot Nothing, CInt(segment.Physical_length.Value_component), If(segment.Virtual_length IsNot Nothing, CInt(segment.Virtual_length.Value_component), 0)))

                If (_currentRowFilterInfo.InactiveObjects?.Count).GetValueOrDefault = 0 OrElse Not (_currentRowFilterInfo.InactiveObjects.ContainsValue(KblObjectType.Node, segment.Start_node) OrElse _currentRowFilterInfo.InactiveObjects.ContainsValue(KblObjectType.Node, segment.End_node)) Then
                    router.LinkVertexesViaAdjacency(segment.Start_node, segment.End_node, segment.SystemId)
                    router.LinkVertexesViaAdjacency(segment.End_node, segment.Start_node, segment.SystemId)
                End If
            End If
        Next
        Return router
    End Function

    Private Function ChangeGridContextMenuVisibility(clickedRow As UltraGridRow) As Boolean
        If (clickedRow.Activated) OrElse (clickedRow.Selected) OrElse InformationHubUtils.HasMarkedRowColor(clickedRow) Then
            If (clickedRow.Activated) OrElse (clickedRow.Selected) Then
                _clickedGridRow = clickedRow

                Me.utmInformationHub.Tools(InfoHubMenuToolKey.ClearMarkedObjects.ToString(True)).SharedProps.Visible = False
                Me.utmInformationHub.Tools(InfoHubMenuToolKey.CalculateDistance.ToString(True)).SharedProps.Visible = GetSelectedObjectsForDistanceCalculation.Count = 2 AndAlso (_clickedGridRow.Band.Key = KblObjectType.Node.ToString(True) OrElse _clickedGridRow.Band.Key = KblObjectType.Accessory_occurrence.ToString(True) OrElse _clickedGridRow.Band.Key = KblObjectType.Fixing_occurrence.ToString(True) OrElse _clickedGridRow.Band.Key = KblObjectType.Connector_occurrence.ToString(True))

                If (_clickedGridRow.Band.Key = KblObjectType.Redlining.ToString(True)) Then
                    Me.utmInformationHub.Tools(InfoHubMenuToolKey.Edit.ToString(True)).SharedProps.Visible = True
                    Me.utmInformationHub.Tools(InfoHubMenuToolKey.EditRedlining.ToString(True)).SharedProps.Visible = False
                Else
                    Me.utmInformationHub.Tools(InfoHubMenuToolKey.Edit.ToString(True)).SharedProps.Visible = False
                    Me.utmInformationHub.Tools(InfoHubMenuToolKey.EditRedlining.ToString(True)).SharedProps.Visible = True
                End If

                Me.utmInformationHub.Tools(InfoHubMenuToolKey.MarkSelectedObjects.ToString(True)).SharedProps.Visible = True

                If (_clickedGridRow.Band.Key = PartPropertyName.Change) OrElse (_clickedGridRow.Band.Key = KblObjectType.Connection.ToString(True)) OrElse (_clickedGridRow.Band.Key = KblObjectType.Redlining.ToString(True)) Then
                    Me.utmInformationHub.Tools(InfoHubMenuToolKey.MemolistAddSelected.ToString(True)).SharedProps.Visible = False
                Else
                    Me.utmInformationHub.Tools(InfoHubMenuToolKey.MemolistAddSelected.ToString(True)).SharedProps.Visible = True
                End If

                Me.utmInformationHub.Tools(InfoHubMenuToolKey.SelectMarkedObjects.ToString(True)).SharedProps.Visible = False

                If (clickedRow.Band.Key = KblObjectType.Segment.ToString(True)) Then
                    Me.utmInformationHub.Tools(InfoHubMenuToolKey.CalculateTotalLength.ToString(True)).SharedProps.Visible = _grids(KblObjectType.Segment.ToString(True)).Selected.Rows.Count > 1
                    Me.utmInformationHub.Tools(InfoHubMenuToolKey.CalculateWireWeight.ToString(True)).SharedProps.Visible = True
                    Me.utmInformationHub.Tools(InfoHubMenuToolKey.ShowBundleCrossSection.ToString(True)).SharedProps.Visible = True
                Else
                    Me.utmInformationHub.Tools(InfoHubMenuToolKey.CalculateTotalLength.ToString(True)).SharedProps.Visible = False
                    Me.utmInformationHub.Tools(InfoHubMenuToolKey.CalculateWireWeight.ToString(True)).SharedProps.Visible = False
                    Me.utmInformationHub.Tools(InfoHubMenuToolKey.ShowBundleCrossSection.ToString(True)).SharedProps.Visible = False
                End If

                If (clickedRow.Band.Key = KblObjectType.Connector_occurrence.ToString(True)) Then
                    Me.utmInformationHub.Tools(InfoHubMenuToolKey.ShowConnectivity.ToString(True)).SharedProps.Visible = True
                    Me.utmInformationHub.Tools(InfoHubMenuToolKey.ShowContacting.ToString(True)).SharedProps.Visible = True
                Else
                    Me.utmInformationHub.Tools(InfoHubMenuToolKey.ShowConnectivity.ToString(True)).SharedProps.Visible = False
                    Me.utmInformationHub.Tools(InfoHubMenuToolKey.ShowContacting.ToString(True)).SharedProps.Visible = False
                End If

                Me.utmInformationHub.Tools(InfoHubMenuToolKey.ShowCorresponding.ToString(True)).SharedProps.Visible = False

                If (_clickedGridRow.Band.Key = KblObjectType.Approval.ToString(True)) OrElse (_clickedGridRow.Band.Key = KblObjectType.Default_dimension_specification.ToString(True)) OrElse (_clickedGridRow.Band.Key = KblObjectType.Dimension_specification.ToString(True)) OrElse (_clickedGridRow.Band.Key = KblObjectType.Harness.ToString(True)) OrElse (_clickedGridRow.Band.Key = KblObjectType.Module.ToString(True)) OrElse (_clickedGridRow.Band.Key = KblObjectType.Redlining.ToString(True)) Then
                    Me.utmInformationHub.Tools(InfoHubMenuToolKey.ShowModules.ToString(True)).SharedProps.Visible = False
                Else
                    Me.utmInformationHub.Tools(InfoHubMenuToolKey.ShowModules.ToString(True)).SharedProps.Visible = True
                End If

                Me.utmInformationHub.Tools(InfoHubMenuToolKey.ShowSchematicsView.ToString(True)).SharedProps.Visible = (Me.ParentForm IsNot Nothing AndAlso CType(Me.ParentForm, DocumentForm).HasSchematicsFeature) _
                AndAlso (
                    (_clickedGridRow.Band.Key = KblObjectType.Wire_occurrence.ToString(True)) OrElse
                    (_clickedGridRow.Band.Key = KblObjectType.Cavity_occurrence.ToString(True)) OrElse
                    (_clickedGridRow.Band.Key = KblObjectType.Special_wire_occurrence.ToString(True)) OrElse
                    (_clickedGridRow.Band.Key = KblObjectType.Connector_occurrence.ToString(True)) OrElse
                    (_clickedGridRow.Band.Key = KblObjectType.Module.ToString(True)) OrElse
                    (_clickedGridRow.Band.Key = KblObjectType.Core_occurrence.ToString(True))
                )

                If (_clickedGridRow.Band.Key = KblObjectType.Special_wire_occurrence.ToString(True)) OrElse (_clickedGridRow.Band.Key = KblObjectType.Core_occurrence.ToString(True)) OrElse (_clickedGridRow.Band.Key = KblObjectType.Wire_occurrence.ToString(True)) Then
                    Me.utmInformationHub.Tools(InfoHubMenuToolKey.HighlightEntireRoutingPath.ToString(True)).SharedProps.Visible = True

                    If (_clickedGridRow.Band.Key = KblObjectType.Core_occurrence.ToString(True)) OrElse (_clickedGridRow.Band.Key = KblObjectType.Wire_occurrence.ToString(True)) Then
                        Me.utmInformationHub.Tools(InfoHubMenuToolKey.ShowOverallConnectivity.ToString(True)).SharedProps.Visible = True
                        Me.utmInformationHub.Tools(InfoHubMenuToolKey.CalculateWireResistance.ToString(True)).SharedProps.Visible = True
                    Else
                        Me.utmInformationHub.Tools(InfoHubMenuToolKey.ShowOverallConnectivity.ToString(True)).SharedProps.Visible = False
                        Me.utmInformationHub.Tools(InfoHubMenuToolKey.CalculateWireResistance.ToString(True)).SharedProps.Visible = False
                    End If

                    Me.utmInformationHub.Tools(InfoHubMenuToolKey.ShowEntireRoutingPath.ToString(True)).SharedProps.Visible = True
                    Me.utmInformationHub.Tools(InfoHubMenuToolKey.ShowStartEndConnectors.ToString(True)).SharedProps.Visible = True
                    Me.utmInformationHub.Tools(InfoHubMenuToolKey.CalculateWireWeight.ToString(True)).SharedProps.Visible = True
                Else
                    Me.utmInformationHub.Tools(InfoHubMenuToolKey.HighlightEntireRoutingPath.ToString(True)).SharedProps.Visible = False
                    Me.utmInformationHub.Tools(InfoHubMenuToolKey.ShowEntireRoutingPath.ToString(True)).SharedProps.Visible = False
                    Me.utmInformationHub.Tools(InfoHubMenuToolKey.ShowOverallConnectivity.ToString(True)).SharedProps.Visible = clickedRow.Band.Key = KblObjectType.Cavity_occurrence.ToString(True) AndAlso (clickedRow.Cells(HarnessAnalyzer.[Shared].CORE_WIRE_NUMBER_KEY).Value.ToString <> String.Empty)
                    Me.utmInformationHub.Tools(InfoHubMenuToolKey.ShowStartEndConnectors.ToString(True)).SharedProps.Visible = False
                    Me.utmInformationHub.Tools(InfoHubMenuToolKey.CalculateWireWeight.ToString(True)).SharedProps.Visible = clickedRow.Band.Key = KblObjectType.Segment.ToString(True)
                    Me.utmInformationHub.Tools(InfoHubMenuToolKey.CalculateWireResistance.ToString(True)).SharedProps.Visible = False
                End If

                If (_clickedGridRow.Band.Key = KblObjectType.QMStamp.ToString(True)) Then
                    Me.utmInformationHub.Tools(InfoHubMenuToolKey.EditRedlining.ToString(True)).SharedProps.Visible = False
                    Me.utmInformationHub.Tools(InfoHubMenuToolKey.MemolistAddSelected.ToString(True)).SharedProps.Visible = False
                    Me.utmInformationHub.Tools(InfoHubMenuToolKey.ShowModules.ToString(True)).SharedProps.Visible = False
                End If

                Me.utmInformationHub.Tools(InfoHubMenuToolKey.CalculateModuleWeight.ToString(True)).SharedProps.Visible = _clickedGridRow.Band.Key = KblObjectType.Module.ToString(True)

                If (clickedRow.Band.Key = KblObjectType.Change.ToString(True)) OrElse (_clickedGridRow.Band.Key = KblObjectType.Default_dimension_specification.ToString(True)) Then
                    Me.utmInformationHub.Tools(InfoHubMenuToolKey.ShowCorresponding.ToString(True)).SharedProps.Visible = False
                Else
                    Me.utmInformationHub.Tools(InfoHubMenuToolKey.ShowCorresponding.ToString(True)).SharedProps.Visible = True

                    With DirectCast(Me.utmInformationHub.Tools(InfoHubMenuToolKey.ShowCorresponding.ToString(True)), PopupMenuTool)
                        For Each tool As ButtonTool In .Tools
                            tool.SharedProps.Visible = False
                        Next

                        Select Case clickedRow.Band.Key
                            Case KblObjectType.Accessory_occurrence.ToString(True)
                                .Tools(InfoHubMenuToolKey.CorrespondingAssemblyParts.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabAssemblyParts.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingChangeDescription.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabChangeDescriptions.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingComponentBoxes.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabComponentBoxes.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingConnectors.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabConnectors.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingCoPacks.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabCoPacks.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingDimSpecs.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabDimSpecs.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingModules.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabModules.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingFixings.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabFixings.ToString(True)).Visible
                            Case KblObjectType.Approval.ToString(True)
                                .Tools(InfoHubMenuToolKey.CorrespondingModules.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabModules.ToString(True)).Visible
                            Case KblObjectType.Assembly_part_occurrence.ToString(True)
                                .Tools(InfoHubMenuToolKey.CorrespondingAccessories.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabAccessories.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingAssemblyParts.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabAssemblyParts.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingCables.ToString(True)).SharedProps.Enabled = Me.utcInformationHub.Tabs(TabNames.tabCables.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingCables.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabCables.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingComponentBoxes.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabComponentBoxes.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingComponents.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabComponents.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingConnectors.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabConnectors.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingCoPacks.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabCoPacks.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingDimSpecs.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabDimSpecs.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingFixings.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabFixings.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingNets.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabNets.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingModules.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabModules.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingSegments.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabSegments.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingVertices.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabVertices.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingWires.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabWires.ToString(True)).Visible
                            Case KblObjectType.Special_wire_occurrence.ToString(True), KblObjectType.Core_occurrence.ToString(True)
                                .Tools(InfoHubMenuToolKey.CorrespondingAssemblyParts.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabAssemblyParts.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingChangeDescription.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabChangeDescriptions.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingComponentBoxes.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabComponentBoxes.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingConnectors.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabConnectors.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingModules.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabModules.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingNets.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabNets.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingSegments.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabSegments.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingVertices.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabVertices.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingWires.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabWires.ToString(True)).Visible
                            Case KblObjectType.Cavity_occurrence.ToString(True)
                                .Tools(InfoHubMenuToolKey.CorrespondingAssemblyParts.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabAssemblyParts.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingCables.ToString(True)).SharedProps.Enabled = Me.utcInformationHub.Tabs(TabNames.tabCables.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingCables.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabCables.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingChangeDescription.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabChangeDescriptions.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingModules.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabModules.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingNets.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabNets.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingVertices.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabVertices.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingWires.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabWires.ToString(True)).Visible
                            Case KblObjectType.Change_description.ToString(True)
                                .Tools(InfoHubMenuToolKey.CorrespondingAccessories.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabAccessories.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingAssemblyParts.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabAssemblyParts.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingCables.ToString(True)).SharedProps.Enabled = Me.utcInformationHub.Tabs(TabNames.tabCables.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingCables.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabCables.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingComponentBoxes.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabComponentBoxes.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingComponents.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabComponents.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingConnectors.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabConnectors.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingCoPacks.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabCoPacks.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingFixings.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabFixings.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingNets.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabNets.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingModules.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabModules.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingSegments.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabSegments.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingVertices.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabVertices.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingWires.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabWires.ToString(True)).Visible
                            Case KblObjectType.Component_box_occurrence.ToString(True)
                                .Tools(InfoHubMenuToolKey.CorrespondingAccessories.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabAccessories.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingAssemblyParts.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabAssemblyParts.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingCables.ToString(True)).SharedProps.Enabled = Me.utcInformationHub.Tabs(TabNames.tabCables.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingCables.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabCables.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingChangeDescription.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabChangeDescriptions.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingComponents.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabComponents.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingConnectors.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabConnectors.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingCoPacks.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabCoPacks.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingFixings.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabFixings.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingModules.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabModules.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingSegments.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabSegments.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingVertices.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabVertices.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingWires.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabWires.ToString(True)).Visible
                            Case KblObjectType.Component_occurrence.ToString(True)
                                .Tools(InfoHubMenuToolKey.CorrespondingAssemblyParts.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabAssemblyParts.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingChangeDescription.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabChangeDescriptions.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingComponentBoxes.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabComponentBoxes.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingConnectors.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabConnectors.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingModules.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabModules.ToString(True)).Visible
                            Case KblObjectType.Connection.ToString(True)
                                .Tools(InfoHubMenuToolKey.CorrespondingAssemblyParts.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabAssemblyParts.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingChangeDescription.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabChangeDescriptions.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingConnectors.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabConnectors.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingModules.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabModules.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingSegments.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabSegments.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingVertices.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabVertices.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingWires.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabWires.ToString(True)).Visible
                            Case KblObjectType.Connector_occurrence.ToString(True)
                                .Tools(InfoHubMenuToolKey.CorrespondingAccessories.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabAccessories.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingAssemblyParts.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabAssemblyParts.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingCables.ToString(True)).SharedProps.Enabled = Me.utcInformationHub.Tabs(TabNames.tabCables.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingCables.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabCables.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingChangeDescription.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabChangeDescriptions.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingComponentBoxes.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabComponentBoxes.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingComponents.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabComponents.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingCoPacks.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabCoPacks.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingDimSpecs.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabDimSpecs.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingModules.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabModules.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingVertices.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabVertices.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingWires.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabWires.ToString(True)).Visible
                            Case KblObjectType.Co_pack_occurrence.ToString(True)
                                .Tools(InfoHubMenuToolKey.CorrespondingAccessories.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabAccessories.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingAssemblyParts.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabAssemblyParts.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingChangeDescription.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabChangeDescriptions.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingComponentBoxes.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabComponentBoxes.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingConnectors.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabConnectors.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingModules.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabModules.ToString(True)).Visible
                            Case KblObjectType.Dimension_specification.ToString(True)
                                .Tools(InfoHubMenuToolKey.CorrespondingAccessories.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabAccessories.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingAssemblyParts.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabAssemblyParts.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingConnectors.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabConnectors.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingFixings.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabFixings.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingSegments.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabSegments.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingVertices.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabVertices.ToString(True)).Visible
                            Case KblObjectType.Fixing_occurrence.ToString(True)
                                .Tools(InfoHubMenuToolKey.CorrespondingAccessories.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabAccessories.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingAssemblyParts.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabAssemblyParts.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingChangeDescription.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabChangeDescriptions.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingComponentBoxes.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabComponentBoxes.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingDimSpecs.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabDimSpecs.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingModules.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabModules.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingSegments.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabSegments.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingVertices.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabVertices.ToString(True)).Visible
                            Case KblObjectType.Module.ToString(True), KblObjectType.Redlining.ToString(True)
                                .Tools(InfoHubMenuToolKey.CorrespondingAccessories.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabAccessories.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingApprovals.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabApprovals.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingAssemblyParts.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabAssemblyParts.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingCables.ToString(True)).SharedProps.Enabled = Me.utcInformationHub.Tabs(TabNames.tabCables.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingCables.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabCables.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingChangeDescription.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabChangeDescriptions.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingComponentBoxes.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabComponentBoxes.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingComponents.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabComponents.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingConnectors.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabConnectors.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingCoPacks.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabCoPacks.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingFixings.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabFixings.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingNets.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabNets.ToString(True)).Visible

                                If (clickedRow.Band.Key = KblObjectType.Redlining.ToString(True)) Then
                                    .Tools(InfoHubMenuToolKey.CorrespondingDimSpecs.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabDimSpecs.ToString(True)).Visible
                                    .Tools(InfoHubMenuToolKey.CorrespondingModules.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabModules.ToString(True)).Visible
                                End If

                                .Tools(InfoHubMenuToolKey.CorrespondingSegments.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabSegments.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingVertices.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabVertices.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingWires.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabWires.ToString(True)).Visible
                            Case KblObjectType.Net.ToString(True)
                                .Tools(InfoHubMenuToolKey.CorrespondingCables.ToString(True)).SharedProps.Enabled = Me.utcInformationHub.Tabs(TabNames.tabCables.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingCables.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabCables.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingChangeDescription.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabChangeDescriptions.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingConnectors.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabConnectors.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingModules.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabModules.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingSegments.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabSegments.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingVertices.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabVertices.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingWires.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabWires.ToString(True)).Visible
                            Case KblObjectType.Protection_on_segment.ToString(True)
                                .Tools(InfoHubMenuToolKey.CorrespondingAssemblyParts.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabAssemblyParts.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingCables.ToString(True)).SharedProps.Enabled = Me.utcInformationHub.Tabs(TabNames.tabCables.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingCables.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabCables.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingChangeDescription.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabChangeDescriptions.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingComponentBoxes.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabComponentBoxes.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingDimSpecs.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabDimSpecs.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingModules.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabModules.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingWires.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabWires.ToString(True)).Visible
                            Case KblObjectType.Protection_on_vertex.ToString(True)
                                .Tools(InfoHubMenuToolKey.CorrespondingAssemblyParts.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabAssemblyParts.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingChangeDescription.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabChangeDescriptions.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingComponentBoxes.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabComponentBoxes.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingDimSpecs.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabDimSpecs.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingModules.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabModules.ToString(True)).Visible
                            Case KblObjectType.QMStamp.ToString(True)
                                .Tools(InfoHubMenuToolKey.CorrespondingAccessories.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabAccessories.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingCables.ToString(True)).SharedProps.Enabled = Me.utcInformationHub.Tabs(TabNames.tabCables.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingCables.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabCables.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingComponentBoxes.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabComponentBoxes.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingComponents.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabComponents.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingConnectors.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabConnectors.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingFixings.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabFixings.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingSegments.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabSegments.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingVertices.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabVertices.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingWires.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabWires.ToString(True)).Visible
                            Case KblObjectType.Segment.ToString(True)
                                .Tools(InfoHubMenuToolKey.CorrespondingCables.ToString(True)).SharedProps.Enabled = Me.utcInformationHub.Tabs(TabNames.tabCables.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingCables.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabCables.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingChangeDescription.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabChangeDescriptions.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingDimSpecs.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabDimSpecs.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingFixings.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabFixings.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingVertices.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabVertices.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingWires.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabWires.ToString(True)).Visible
                            Case KblObjectType.Node.ToString(True)
                                .Tools(InfoHubMenuToolKey.CorrespondingCables.ToString(True)).SharedProps.Enabled = Me.utcInformationHub.Tabs(TabNames.tabCables.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingCables.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabCables.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingChangeDescription.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabChangeDescriptions.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingConnectors.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabConnectors.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingDimSpecs.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabDimSpecs.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingFixings.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabFixings.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingSegments.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabSegments.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingWires.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabWires.ToString(True)).Visible
                            Case KblObjectType.Wire_occurrence.ToString(True)
                                .Tools(InfoHubMenuToolKey.CorrespondingAssemblyParts.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabAssemblyParts.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingCables.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabCables.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingChangeDescription.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabChangeDescriptions.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingComponentBoxes.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabComponentBoxes.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingConnectors.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabConnectors.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingModules.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabModules.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingNets.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabNets.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingSegments.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabSegments.ToString(True)).Visible
                                .Tools(InfoHubMenuToolKey.CorrespondingVertices.ToString(True)).SharedProps.Visible = Me.utcInformationHub.Tabs(TabNames.tabVertices.ToString(True)).Visible

                                If (_kblMapper.KBLCoreCableMapper.ContainsKey(clickedRow.Tag?.ToString)) Then
                                    .Tools(InfoHubMenuToolKey.CorrespondingCables.ToString(True)).SharedProps.Enabled = Me.utcInformationHub.Tabs(TabNames.tabCables.ToString(True)).Visible
                                Else
                                    .Tools(InfoHubMenuToolKey.CorrespondingCables.ToString(True)).SharedProps.Enabled = False
                                End If
                        End Select
                    End With
                End If
            Else
                For Each tool As ToolBase In Me.utmInformationHub.Tools
                    tool.SharedProps.Visible = False
                Next
            End If

            If MarkedRowsNumericStringSortComparer.FindMarkingRows({clickedRow}, _currentRowFilterInfo).Count > 0 Then
                Me.utmInformationHub.Tools(InfoHubMenuToolKey.ClearMarkedObjects.ToString(True)).SharedProps.Visible = True
                Me.utmInformationHub.Tools(InfoHubMenuToolKey.MarkSelectedObjects.ToString(True)).SharedProps.Visible = False
                Me.utmInformationHub.Tools(InfoHubMenuToolKey.SelectMarkedObjects.ToString(True)).SharedProps.Visible = True
            End If

            Return True
        Else
            Return False
        End If
    End Function

    Private Sub ChangeModifiedCellAppearance(cell As UltraGridCell, compareValue As Object, Optional isAdded As Boolean = False, Optional isDeleted As Boolean = False)
        If (compareValue IsNot Nothing) AndAlso (TypeOf compareValue Is KeyValuePair(Of String, Object)) Then
            compareValue = DirectCast(compareValue, KeyValuePair(Of String, Object)).Value
        End If

        If (compareValue Is Nothing) Then
            compareValue = String.Empty
        End If

        With cell
            If (Not isAdded) AndAlso (Not isDeleted) Then
                .Appearance.ForeColor = CHANGED_MODIFIED_FORECOLOR.ToColor
            End If

            If .Value.ToString <> Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS Then
                If TypeOf compareValue Is String AndAlso String.IsNullOrEmpty(CType(compareValue, String)) Then
                    If isAdded Then
                        .Appearance.FontData.Bold = DefaultableBoolean.True
                    ElseIf isDeleted Then
                        .Appearance.FontData.Strikeout = DefaultableBoolean.True
                    Else
                        .ToolTipText = String.Format("{0}{1}{2}{3}{4}", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc2_TooltipPart)
                    End If
                ElseIf IsNumeric(compareValue) OrElse TypeOf compareValue Is String Then
                    If (.Value.ToString.EndsWith("%"c)) AndAlso (IsNumeric((Replace(.Value.ToString, "%", String.Empty)).Trim)) Then
                        .ToolTipText = String.Format("{0}{1}{2}{3}{4}{5} %", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart, CInt(CDbl(compareValue) * 100).ToString)
                    Else
                        .ToolTipText = String.Format("{0}{1}{2}{3}{4}{5}", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart, compareValue.ToString)
                    End If
                ElseIf (TypeOf compareValue Is AliasIdComparisonMapper) Then
                    .ToolTipText = String.Format("{0}{1}{2}{3}{4}", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart)

                    For Each aliasId As Alias_identification In DirectCast(compareValue, AliasIdComparisonMapper).Changes.NewItems
                        .ToolTipText &= String.Format(InformationHubStrings.Added_TooltipPart, vbLf, String.Format(InformationHubStrings.AliasId_TooltipPart, aliasId.Alias_id, If(aliasId.Scope Is Nothing OrElse aliasId.Scope = String.Empty, "-", aliasId.Scope), If(aliasId.Description Is Nothing OrElse aliasId.Description = String.Empty, "-", aliasId.Description)))
                    Next

                    For Each aliasId As Alias_identification In DirectCast(compareValue, AliasIdComparisonMapper).Changes.DeletedItems
                        .ToolTipText &= String.Format(InformationHubStrings.Deleted_TooltipPart, vbLf, String.Format(InformationHubStrings.AliasId_TooltipPart, aliasId.Alias_id, If(aliasId.Scope Is Nothing OrElse aliasId.Scope = String.Empty, "-", aliasId.Scope), If(aliasId.Description Is Nothing OrElse aliasId.Description = String.Empty, "-", aliasId.Description)))
                    Next
                ElseIf (TypeOf compareValue Is CommonComparisonMapper) Then
                    .ToolTipText = String.Format("{0}{1}{2}{3}{4}", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart)

                    For Each addedObject As Object In DirectCast(compareValue, CommonComparisonMapper).Changes.NewItems
                        If (TypeOf addedObject Is Change) Then
                            With DirectCast(addedObject, Change)
                                cell.ToolTipText &= String.Format(InformationHubStrings.Added_TooltipPart, vbLf, String.Format(InformationHubStrings.Change_TooltipPart, If(.Id, "-"), If(.Description, "-"), If(.Change_request, "-"), If(.Change_date, "-"), .Responsible_designer, .Designer_department, If(.Approver_name, "-"), If(.Approver_department, "-")))
                            End With
                        ElseIf (TypeOf addedObject Is External_reference) Then
                            With DirectCast(addedObject, External_reference)
                                cell.ToolTipText &= String.Format(InformationHubStrings.Added_TooltipPart, vbLf, String.Format(InformationHubStrings.ExtReference_TooltipPart, If(.Document_type <> String.Empty, .Document_type, "-"), If(.Document_number <> String.Empty, .Document_number, "-"), If(.Change_level <> String.Empty, .Change_level, "-"), If(.File_name, "-"), If(.Location, "-"), .Data_format, If(.Creating_system, "-")))
                            End With
                        ElseIf (TypeOf addedObject Is String) Then
                            .ToolTipText &= String.Format(InformationHubStrings.Added_TooltipPart, vbLf, addedObject.ToString)
                        End If
                    Next

                    For Each deletedObject As Object In DirectCast(compareValue, CommonComparisonMapper).Changes.DeletedItems
                        If (TypeOf deletedObject Is Change) Then
                            With DirectCast(deletedObject, Change)
                                cell.ToolTipText &= String.Format(InformationHubStrings.Deleted_TooltipPart, vbLf, String.Format(InformationHubStrings.Change_TooltipPart, If(.Id, "-"), If(.Description, "-"), If(.Change_request, "-"), If(.Change_date, "-"), .Responsible_designer, .Designer_department, If(.Approver_name, "-"), If(.Approver_department, "-")))
                            End With
                        ElseIf (TypeOf deletedObject Is External_reference) Then
                            With DirectCast(deletedObject, External_reference)
                                cell.ToolTipText &= String.Format(InformationHubStrings.Deleted_TooltipPart, vbLf, String.Format(InformationHubStrings.ExtReference_TooltipPart, If(.Document_type <> String.Empty, .Document_type, "-"), If(.Document_number <> String.Empty, .Document_number, "-"), If(.Change_level <> String.Empty, .Change_level, "-"), If(.File_name, "-"), If(.Location, "-"), .Data_format, If(.Creating_system, "-")))
                            End With
                        ElseIf (TypeOf deletedObject Is String) Then
                            .ToolTipText &= String.Format(InformationHubStrings.Deleted_TooltipPart, vbLf, deletedObject.ToString)
                        End If
                    Next
                ElseIf (TypeOf compareValue Is CrossSectionAreaComparisonMapper) Then
                    .ToolTipText = String.Format("{0}{1}{2}{3}{4}", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart)

                    For Each csaChangedProperties As ChangedProperty In DirectCast(compareValue, CrossSectionAreaComparisonMapper).Changes.ModifiedItems
                        If (csaChangedProperties.ChangedProperties.ContainsKey(CrossSectionAreaPropertyName.Area.ToString)) Then .ToolTipText &= String.Format("{0}{1} {2}", vbLf, DirectCast(csaChangedProperties.ChangedProperties(CrossSectionAreaPropertyName.Area.ToString), Numerical_value).Value_component.ToString("#0.##", CultureInfo.InvariantCulture), _kblMapperForCompare.KBLUnitMapper(DirectCast(csaChangedProperties.ChangedProperties(CrossSectionAreaPropertyName.Area.ToString), Numerical_value).Unit_component).Unit_name)
                    Next

                    For Each crossSectionArea As Cross_section_area In DirectCast(compareValue, CrossSectionAreaComparisonMapper).Changes.NewItems
                        .ToolTipText &= String.Format(InformationHubStrings.Added2_TooltipPart, vbLf, crossSectionArea.Area.Value_component.ToString("#0.##", CultureInfo.InvariantCulture), _kblMapperForCompare.KBLUnitMapper(crossSectionArea.Area.Unit_component).Unit_name)
                    Next

                    For Each crossSectionArea As Cross_section_area In DirectCast(compareValue, CrossSectionAreaComparisonMapper).Changes.DeletedItems
                        .ToolTipText &= String.Format(InformationHubStrings.Deleted2_TooltipPart, vbLf, crossSectionArea.Area.Value_component.ToString("#0.##", CultureInfo.InvariantCulture), _kblMapperForCompare.KBLUnitMapper(crossSectionArea.Area.Unit_component).Unit_name)
                    Next
                ElseIf (TypeOf compareValue Is FixingAssignmentComparisonMapper) Then
                    .ToolTipText = String.Format("{0}{1}{2}{3}{4}", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart)

                    For Each fixingAssignment As Fixing_assignment In DirectCast(compareValue, FixingAssignmentComparisonMapper).Changes.NewItems
                        If (_kblMapperForCompare.KBLOccurrenceMapper.ContainsKey(fixingAssignment.Fixing)) Then
                            If (TypeOf _kblMapperForCompare.KBLOccurrenceMapper(fixingAssignment.Fixing) Is Accessory_occurrence) Then
                                With DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(fixingAssignment.Fixing), Accessory_occurrence)
                                    cell.ToolTipText &= String.Format(InformationHubStrings.FixingAssignmentAdd_TooltipPart, vbLf, .Id, If(.Description, "-"), If(_kblMapperForCompare.KBLPartMapper.ContainsKey(If(.Part, String.Empty)), DirectCast(_kblMapperForCompare.KBLPartMapper(.Part), Accessory).Part_number, "-"), String.Format("{0} %", Math.Round(fixingAssignment.Location * 100, NOF_DIGITS_LOCATIONS)))
                                End With
                            Else
                                With DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(fixingAssignment.Fixing), Fixing_occurrence)
                                    cell.ToolTipText &= String.Format(InformationHubStrings.FixingAssignmentAdd_TooltipPart, vbLf, .Id, If(.Description, "-"), If(_kblMapperForCompare.KBLPartMapper.ContainsKey(If(.Part, String.Empty)), DirectCast(_kblMapperForCompare.KBLPartMapper(.Part), Fixing).Part_number, "-"), String.Format("{0} %", Math.Round(fixingAssignment.Location * 100, NOF_DIGITS_LOCATIONS)))
                                End With
                            End If
                        End If
                    Next

                    For Each fixingAssignment As Fixing_assignment In DirectCast(compareValue, FixingAssignmentComparisonMapper).Changes.DeletedItems
                        If (_kblMapper.KBLOccurrenceMapper.ContainsKey(fixingAssignment.Fixing)) Then
                            If (TypeOf _kblMapper.KBLOccurrenceMapper(fixingAssignment.Fixing) Is Accessory_occurrence) Then
                                With DirectCast(_kblMapper.KBLOccurrenceMapper(fixingAssignment.Fixing), Accessory_occurrence)
                                    cell.ToolTipText &= String.Format(InformationHubStrings.FixingAssignmentDel_TooltipPart, vbLf, .Id, If(.Description, "-"), If(_kblMapper.KBLPartMapper.ContainsKey(If(.Part, String.Empty)), DirectCast(_kblMapper.KBLPartMapper(.Part), Accessory).Part_number, "-"), String.Format("{0} %", Math.Round(fixingAssignment.Location * 100, NOF_DIGITS_LOCATIONS)))
                                End With
                            Else
                                With DirectCast(_kblMapper.KBLOccurrenceMapper(fixingAssignment.Fixing), Fixing_occurrence)
                                    cell.ToolTipText &= String.Format(InformationHubStrings.FixingAssignmentDel_TooltipPart, vbLf, .Id, If(.Description, "-"), If(_kblMapper.KBLPartMapper.ContainsKey(If(.Part, String.Empty)), DirectCast(_kblMapper.KBLPartMapper(.Part), Fixing).Part_number, "-"), String.Format("{0} %", Math.Round(fixingAssignment.Location * 100, NOF_DIGITS_LOCATIONS)))
                                End With
                            End If
                        End If
                    Next
                ElseIf (TypeOf compareValue Is InstallationInfoComparisonMapper) Then
                    .ToolTipText = String.Format("{0}{1}{2}{3}{4}", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart)

                    For Each installationInstruction As Installation_instruction In DirectCast(compareValue, InstallationInfoComparisonMapper).Changes.NewItems
                        .ToolTipText &= String.Format(InformationHubStrings.Added3_TooltipPart, vbLf, installationInstruction.Instruction_type, installationInstruction.Instruction_value)
                    Next

                    For Each installationInstruction As Installation_instruction In DirectCast(compareValue, InstallationInfoComparisonMapper).Changes.DeletedItems
                        .ToolTipText &= String.Format(InformationHubStrings.Deleted3_TooltipPart, vbLf, installationInstruction.Instruction_type, installationInstruction.Instruction_value)
                    Next
                ElseIf (TypeOf compareValue Is LocalizedDescriptionComparisonMapper) Then
                    .ToolTipText = String.Format("{0}{1}{2}{3}{4}", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart)

                    For Each locDescChangedProperties As ChangedProperty In DirectCast(compareValue, LocalizedDescriptionComparisonMapper).Changes.ModifiedItems
                        If (locDescChangedProperties.ChangedProperties.ContainsKey(LocalizedDescriptionPropertyName.Value.ToString)) Then
                            .ToolTipText &= String.Format("{0}", locDescChangedProperties.ChangedProperties(LocalizedDescriptionPropertyName.Value.ToString))
                        End If
                    Next

                    For Each locDecription As Localized_string In DirectCast(compareValue, LocalizedDescriptionComparisonMapper).Changes.NewItems
                        .ToolTipText &= String.Format(InformationHubStrings.Added3_TooltipPart, vbLf, locDecription.Language_code, locDecription.Value)
                    Next

                    For Each locDecription As Localized_string In DirectCast(compareValue, LocalizedDescriptionComparisonMapper).Changes.DeletedItems
                        .ToolTipText &= String.Format(InformationHubStrings.Deleted3_TooltipPart, vbLf, locDecription.Language_code, locDecription.Value)
                    Next
                ElseIf (TypeOf compareValue Is Numerical_value) Then
                    .ToolTipText = String.Format("{0}{1}{2}{3}{4}{5} {6}", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart, DirectCast(compareValue, Numerical_value).Value_component.ToString("#0.##", CultureInfo.InvariantCulture), _kblMapperForCompare.KBLUnitMapper(DirectCast(compareValue, Numerical_value).Unit_component).Unit_name)
                ElseIf (TypeOf compareValue Is Material) Then
                    .ToolTipText = String.Format("{0}{1}{2}{3}{4}{5}", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart, DirectCast(compareValue, Material).Material_key)
                ElseIf (TypeOf compareValue Is ProcessingInfoComparisonMapper) Then
                    .ToolTipText = String.Format("{0}{1}{2}{3}{4}", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart)

                    For Each processingInstruction As Processing_instruction In DirectCast(compareValue, ProcessingInfoComparisonMapper).Changes.NewItems
                        .ToolTipText &= String.Format(InformationHubStrings.Added3_TooltipPart, vbLf, processingInstruction.Instruction_type, processingInstruction.Instruction_value)
                    Next

                    For Each processingInstruction As Processing_instruction In DirectCast(compareValue, ProcessingInfoComparisonMapper).Changes.DeletedItems
                        .ToolTipText &= String.Format(InformationHubStrings.Deleted3_TooltipPart, vbLf, processingInstruction.Instruction_type, processingInstruction.Instruction_value)
                    Next
                ElseIf (TypeOf compareValue Is SingleObjectComparisonMapper) Then
                    .ToolTipText = String.Format("{0}{1}{2}{3}{4}", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart)

                    For Each singleObject As Object In DirectCast(compareValue, SingleObjectComparisonMapper).Changes.NewItems
                        If (TypeOf singleObject Is Part) Then
                            .ToolTipText &= String.Format(InformationHubStrings.Added_TooltipPart, vbLf, DirectCast(singleObject, Part).Part_number)
                        ElseIf (TypeOf singleObject Is Wiring_group) Then
                            .ToolTipText &= String.Format(InformationHubStrings.Added_TooltipPart, vbLf, DirectCast(singleObject, Wiring_group).Id)
                        End If
                    Next

                    For Each singleObject As Object In DirectCast(compareValue, SingleObjectComparisonMapper).Changes.DeletedItems
                        If (TypeOf singleObject Is Part) Then
                            .ToolTipText &= String.Format(InformationHubStrings.Deleted_TooltipPart, vbLf, DirectCast(singleObject, Part).Part_number)
                        ElseIf (TypeOf singleObject Is Wiring_group) Then
                            .ToolTipText &= String.Format(InformationHubStrings.Deleted_TooltipPart, vbLf, DirectCast(singleObject, Wiring_group).Id)
                        End If
                    Next
                ElseIf (TypeOf compareValue Is WireLengthComparisonMapper) Then
                    For Each wireChangedProperties As ChangedProperty In DirectCast(compareValue, WireLengthComparisonMapper).Changes.ModifiedItems
                        If (wireChangedProperties.ChangedProperties.ContainsKey(WireLengthPropertyName.Length_value.ToString)) Then
                            .ToolTipText = String.Format("{0}{1}{2}{3}{4}{5} {6}", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart, wireChangedProperties.ChangedProperties(WireLengthPropertyName.Length_value).ToString, "mm")
                        End If
                    Next

                    For Each wireLength As Wire_length In DirectCast(compareValue, WireLengthComparisonMapper).Changes.NewItems
                        .ToolTipText = String.Format(InformationHubStrings.Added2_TooltipPart, vbLf, Math.Round(wireLength.Length_value.Value_component, 2).ToString, "mm")
                    Next

                    For Each wireLength As Wire_length In DirectCast(compareValue, WireLengthComparisonMapper).Changes.DeletedItems
                        .ToolTipText = String.Format(InformationHubStrings.Deleted2_TooltipPart, vbLf, Math.Round(wireLength.Length_value.Value_component, 2).ToString, "mm")
                    Next

                    If (.ToolTipText.StartsWith(vbLf)) Then
                        .ToolTipText = .ToolTipText.Substring(1, .ToolTipText.Length - 1)
                    End If
                End If

                If (.Value.ToString = String.Empty) AndAlso (.ToolTipText <> String.Empty) Then
                    DirectCast(.Row.ListObject, UltraDataRow).SetCellValue(.Column.Key, "-")
                End If
            Else
                If isAdded Then
                    .Appearance.FontData.Bold = DefaultableBoolean.True
                ElseIf (isDeleted) Then
                    .Appearance.FontData.Strikeout = DefaultableBoolean.True
                Else
                    .Appearance.FontData.SizeInPoints = 14
                End If
            End If
        End With
    End Sub

    Private Sub ChangeModifiedRowAppearance(row As UltraGridRow, isDeleted As Boolean, isAdded As Boolean)
        With row
            If (isDeleted) Then
                For Each cell As UltraGridCell In row.Cells
                    If (row.ParentRow Is Nothing AndAlso cell.Column.Index > 0) OrElse (row.ParentRow IsNot Nothing AndAlso cell.Column.Index > 1) Then
                        cell.Appearance.FontData.Strikeout = DefaultableBoolean.True
                    End If
                Next
            ElseIf (isAdded) Then
                For Each cell As UltraGridCell In row.Cells
                    If (row.ParentRow Is Nothing AndAlso cell.Column.Index > 0) OrElse (row.ParentRow IsNot Nothing AndAlso cell.Column.Index > 1) Then
                        cell.Appearance.FontData.Bold = DefaultableBoolean.True
                    End If
                Next
            End If
        End With
    End Sub

    Private Function ClearMarkedRowsInGrids(Optional refreshRowFilters As Boolean = True) As Boolean
        Dim changed As Boolean = False
        If ClearBoldTabAppearances() Then
            changed = True
        End If

        If (_currentRowFilterInfo?.MarkedRows?.Count).GetValueOrDefault > 0 Then
            changed = True
            _currentRowFilterInfo?.MarkedRows?.Clear()
            If refreshRowFilters Then
                _rowFilters?.Refresh()
            End If
        End If

        Return changed
    End Function

    Private Function ClearBoldTabAppearances() As Boolean
        Dim changed As Boolean = False
        For Each tab As UltraTab In Me.utcInformationHub.Tabs
            If tab.Appearance.FontData.Bold <> DefaultableBoolean.Default Then
                tab.Appearance.FontData.Bold = DefaultableBoolean.Default
                If Not changed Then
                    utcInformationHub.BeginUpdate()
                End If
                changed = True
            End If
        Next

        If changed Then
            utcInformationHub.EndUpdate()
        End If
        Return changed
    End Function

    Private Sub ClearSelectedChildRowsInGrid(grid As UltraGrid, Optional update As Boolean = True)
        If update Then
            grid.BeginUpdate()
        End If

        Using grid.EventManager.ProtectProperty(NameOf(EventManagerBase.AllEventsEnabled), False)
            Dim removableChildRows As New List(Of UltraGridRow)
            For Each selectedChildRow As UltraGridRow In _selectedChildRows
                If selectedChildRow.Band.ParentBand.Key = grid.DisplayLayout.Bands(0).Key Then
                    selectedChildRow.Appearance.ForeColor = selectedChildRow.Band.Layout.Appearance.ForeColor
                    selectedChildRow.Appearance.BackColor = selectedChildRow.Band.Layout.Appearance.BackColor
                    removableChildRows.Add(selectedChildRow)
                End If
            Next

            For Each removableChildRow As UltraGridRow In removableChildRows
                _selectedChildRows.Remove(removableChildRow)
            Next
        End Using

        If update Then
            grid.EndUpdate()
        End If
    End Sub

    Private Sub ClearSelectedRowsInGrid(grid As UltraGrid, Optional clearActiveRow As Boolean = True, Optional update As Boolean = True)
        If grid.Selected.Rows.Count > 0 Then
            If update Then
                grid.BeginUpdate()
            End If

            Using grid.EventManager.ProtectProperty(NameOf(EventManagerBase.AllEventsEnabled), False)
                grid.Selected.Rows.Clear()
                If clearActiveRow Then
                    grid.ActiveRow = Nothing
                End If
            End Using

            If update Then
                grid.EndUpdate()
            End If
        End If

        ClearSelectedChildRowsInGrid(grid, update)
    End Sub

    Private Sub ClearSelectedRowsInGrids()
        Dim activeGrid As UltraGrid = Me.ActiveGrid
        For Each grid As UltraGrid In (_grids?.Values).OrEmpty.Except({activeGrid}) ' HINT: without activegrid because we want to update the active grid to update at least to retain the focus
            ClearSelectedRowsInGrid(grid)
        Next

        If activeGrid IsNot Nothing Then
            ClearSelectedRowsInGrid(activeGrid, True)
        End If
    End Sub


    Private Sub EndGridUpdate(Optional invalidate As Boolean = False)
        For Each grid As UltraGrid In _grids.Values
            grid.Update()
            grid.EndUpdate(invalidate)
        Next
    End Sub

    Private Sub FillContextMenus()
        With _contextMenuGrid
            .DropDownArrowStyle = DropDownArrowStyle.None

            Dim editButton As New ButtonTool(InfoHubMenuToolKey.Edit.ToString)
            editButton.SharedProps.Caption = InformationHubStrings.Edit_CtxtMnu_Caption
            editButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.Redlining

            Dim editRedliningButton As New ButtonTool(InfoHubMenuToolKey.EditRedlining.ToString)
            editRedliningButton.SharedProps.Caption = InformationHubStrings.EditRedlining_CtxtMnu_Caption
            editRedliningButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.Redlining

            Dim highlightEntireRoutingPathButton As New ButtonTool(InfoHubMenuToolKey.HighlightEntireRoutingPath.ToString)
            highlightEntireRoutingPathButton.SharedProps.Caption = InformationHubStrings.HighlightRoutingPath_CtxtMnu_Caption
            highlightEntireRoutingPathButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.HighlightEntireRoutingPath.ToBitmap

            Dim showBundleCrossSectionButton As New ButtonTool(InfoHubMenuToolKey.ShowBundleCrossSection.ToString)
            showBundleCrossSectionButton.SharedProps.Caption = InformationHubStrings.ShowBundleCS_CtxtMnu_Caption
            showBundleCrossSectionButton.SharedProps.Visible = False
            showBundleCrossSectionButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.BundleCrossSection.ToBitmap

            Dim showConnectivityButton As New ButtonTool(InfoHubMenuToolKey.ShowConnectivity.ToString)
            showConnectivityButton.SharedProps.Caption = InformationHubStrings.ShowConnectivity_CtxtMnu_Caption
            showConnectivityButton.SharedProps.Visible = False
            showConnectivityButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.Connectivity.ToBitmap

            Dim showContactingButton As New ButtonTool(InfoHubMenuToolKey.ShowContacting.ToString)
            showContactingButton.SharedProps.Caption = InformationHubStrings.ShowContacting_CtxtMnu_Caption
            showContactingButton.SharedProps.Visible = False
            showContactingButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.Connectivity.ToBitmap


            Dim showCorrespondingMenu As New PopupMenuTool(InfoHubMenuToolKey.ShowCorresponding.ToString)
            showCorrespondingMenu.SharedProps.Caption = InformationHubStrings.ShowCorresponding_CtxtMnu_Caption

            Dim showEntireRoutingPathButton As New ButtonTool(InfoHubMenuToolKey.ShowEntireRoutingPath.ToString)
            showEntireRoutingPathButton.SharedProps.Caption = InformationHubStrings.ShowRoutingPath_CtxtMnu_Caption
            showEntireRoutingPathButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.ShowEntireRoutingPath.ToBitmap

            Dim showModulesButton As New ButtonTool(InfoHubMenuToolKey.ShowModules.ToString)
            showModulesButton.SharedProps.Caption = InformationHubStrings.ShowMods_CtxtMnu_Caption
            showModulesButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.ModuleFamily.ToBitmap

            Dim showOverallConnectivityButton As New ButtonTool(InfoHubMenuToolKey.ShowOverallConnectivity.ToString)
            showOverallConnectivityButton.SharedProps.Caption = InformationHubStrings.ShowOverallConnectivity_CtxtMnu_Caption
            showOverallConnectivityButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.ShowEntireConnectivity.ToBitmap

            Dim showSchematicsView As New ButtonTool(InfoHubMenuToolKey.ShowSchematicsView.ToString)
            showSchematicsView.SharedProps.Caption = InformationHubStrings.ShowSchematics_CtxtMnu_Caption
            showSchematicsView.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.ShowEntireConnectivity.ToBitmap

            Dim showStartEndConnectors As New ButtonTool(InfoHubMenuToolKey.ShowStartEndConnectors.ToString)
            showStartEndConnectors.SharedProps.Caption = InformationHubStrings.ShowStartEndConns_CtxtMnu_Caption
            showStartEndConnectors.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.StartEndConnector.ToBitmap

            Dim correspondingAccessoriesButton As New ButtonTool(InfoHubMenuToolKey.CorrespondingAccessories.ToString)
            correspondingAccessoriesButton.SharedProps.Caption = KblObjectType.Accessory_occurrence.ToLocalizedPluralString

            Dim correspondingApprovalsButton As New ButtonTool(InfoHubMenuToolKey.CorrespondingApprovals.ToString)
            correspondingApprovalsButton.SharedProps.Caption = KblObjectType.Approval.ToLocalizedPluralString

            Dim correspondingAssemblyPartsButton As New ButtonTool(InfoHubMenuToolKey.CorrespondingAssemblyParts.ToString)
            correspondingAssemblyPartsButton.SharedProps.Caption = KblObjectType.Assembly_part_occurrence.ToLocalizedPluralString

            Dim correspondingCablesButton As New ButtonTool(InfoHubMenuToolKey.CorrespondingCables.ToString)
            correspondingCablesButton.SharedProps.Caption = KblObjectType.Special_wire_occurrence.ToLocalizedPluralString

            Dim correspondingChangeDescriptionsButton As New ButtonTool(InfoHubMenuToolKey.CorrespondingChangeDescription.ToString)
            correspondingChangeDescriptionsButton.SharedProps.Caption = KblObjectType.Change_description.ToLocalizedPluralString

            Dim correspondingComponentBoxesButton As New ButtonTool(InfoHubMenuToolKey.CorrespondingComponentBoxes.ToString)
            correspondingComponentBoxesButton.SharedProps.Caption = KblObjectType.Component_box.ToLocalizedPluralString

            Dim correspondingComponentsButton As New ButtonTool(InfoHubMenuToolKey.CorrespondingComponents.ToString)
            correspondingComponentsButton.SharedProps.Caption = KblObjectType.Component.ToLocalizedPluralString

            Dim correspondingConnectorsButton As New ButtonTool(InfoHubMenuToolKey.CorrespondingConnectors.ToString)
            correspondingConnectorsButton.SharedProps.Caption = KblObjectType.Connector_occurrence.ToLocalizedPluralString

            Dim correspondingCoPacksButton As New ButtonTool(InfoHubMenuToolKey.CorrespondingCoPacks.ToString)
            correspondingCoPacksButton.SharedProps.Caption = KblObjectType.Co_pack_occurrence.ToLocalizedPluralString

            Dim correspondingDimSpecsButton As New ButtonTool(InfoHubMenuToolKey.CorrespondingDimSpecs.ToString)
            correspondingDimSpecsButton.SharedProps.Caption = KblObjectType.Dimension_specification.ToLocalizedPluralString

            Dim correspondingFixingsButton As New ButtonTool(InfoHubMenuToolKey.CorrespondingFixings.ToString)
            correspondingFixingsButton.SharedProps.Caption = KblObjectType.Fixing_occurrence.ToLocalizedPluralString

            Dim correspondingModulesButton As New ButtonTool(InfoHubMenuToolKey.CorrespondingModules.ToString)
            correspondingModulesButton.SharedProps.Caption = KblObjectType.Harness_module.ToLocalizedPluralString

            Dim correspondingNetsButton As New ButtonTool(InfoHubMenuToolKey.CorrespondingNets.ToString)
            correspondingNetsButton.SharedProps.Caption = KblObjectType.Net.ToLocalizedPluralString

            Dim correspondingSegmentsButton As New ButtonTool(InfoHubMenuToolKey.CorrespondingSegments.ToString)
            correspondingSegmentsButton.SharedProps.Caption = KblObjectType.Segment.ToLocalizedPluralString

            Dim correspondingVerticesButton As New ButtonTool(InfoHubMenuToolKey.CorrespondingVertices.ToString)
            correspondingVerticesButton.SharedProps.Caption = KblObjectType.Node.ToLocalizedPluralString

            Dim correspondingWiresButton As New ButtonTool(InfoHubMenuToolKey.CorrespondingWires.ToString)
            correspondingWiresButton.SharedProps.Caption = String.Format("{0}/{1}", KblObjectType.Wire_occurrence.ToLocalizedPluralString, KblObjectType.Core_occurrence.ToLocalizedPluralString)

            Dim defaultCurrentThreadCultureName As String = (If(Globalization.CultureInfo.DefaultThreadCurrentCulture?.Name, Globalization.CultureInfo.CurrentCulture.Name))

            If defaultCurrentThreadCultureName = "en-US" Then
                showCorrespondingMenu.Tools.AddRange(New ToolBase() {correspondingAccessoriesButton, correspondingApprovalsButton, correspondingAssemblyPartsButton, correspondingCablesButton, correspondingChangeDescriptionsButton, correspondingComponentBoxesButton, correspondingComponentsButton, correspondingConnectorsButton, correspondingCoPacksButton, correspondingDimSpecsButton, correspondingFixingsButton, correspondingModulesButton, correspondingNetsButton, correspondingSegmentsButton, correspondingVerticesButton, correspondingWiresButton})
            Else
                showCorrespondingMenu.Tools.AddRange(New ToolBase() {correspondingChangeDescriptionsButton, correspondingFixingsButton, correspondingCoPacksButton, correspondingDimSpecsButton, correspondingWiresButton, correspondingApprovalsButton, correspondingCablesButton, correspondingVerticesButton, correspondingComponentsButton, correspondingComponentBoxesButton, correspondingModulesButton, correspondingNetsButton, correspondingSegmentsButton, correspondingConnectorsButton, correspondingAccessoriesButton, correspondingAssemblyPartsButton})
            End If

            Dim calculateDistanceButton As New ButtonTool(InfoHubMenuToolKey.CalculateDistance.ToString)
            calculateDistanceButton.SharedProps.Caption = InformationHubStrings.CalcDist_CtxtMnu_Caption
            calculateDistanceButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.Distance.ToBitmap

            Dim calculateModuleWeightButton As New ButtonTool(InfoHubMenuToolKey.CalculateModuleWeight.ToString)
            calculateModuleWeightButton.SharedProps.Caption = InformationHubStrings.CalcModWeight_CtxtMnu_Caption
            calculateModuleWeightButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.Calculator.ToBitmap

            Dim calculateTotalLengthButton As New ButtonTool(InfoHubMenuToolKey.CalculateTotalLength.ToString)
            calculateTotalLengthButton.SharedProps.Caption = InformationHubStrings.CalcTotLength_CtxtMnu_Caption
            calculateTotalLengthButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.Distance.ToBitmap

            Dim calculateWireWeightButton As New ButtonTool(InfoHubMenuToolKey.CalculateWireWeight.ToString)
            calculateWireWeightButton.SharedProps.Caption = InformationHubStrings.CalcWirWeight_CtxtMnu_Caption
            calculateWireWeightButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.Calculator.ToBitmap

            Dim calculateWireResistanceButton As New ButtonTool(InfoHubMenuToolKey.CalculateWireResistance.ToString)
            calculateWireResistanceButton.SharedProps.Caption = InformationHubStrings.CalcWirRes_CtxtMnu_Caption
            calculateWireResistanceButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.Calculator.ToBitmap

            Dim clearMarkedObjectsButton As New ButtonTool(InfoHubMenuToolKey.ClearMarkedObjects.ToString)
            clearMarkedObjectsButton.SharedProps.Caption = InformationHubStrings.ClearMarkedObjs_CtxtMnu_Caption
            clearMarkedObjectsButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.ClearMarkedObjects.ToBitmap

            Dim selectMarkedObjectsButton As New ButtonTool(InfoHubMenuToolKey.SelectMarkedObjects.ToString)
            selectMarkedObjectsButton.SharedProps.Caption = InformationHubStrings.SelMarkedObjs_CtxtMnu_Caption
            selectMarkedObjectsButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.SelectMarkedObjects.ToBitmap

            Dim markSelectedObjectsButton As New ButtonTool(InfoHubMenuToolKey.MarkSelectedObjects.ToString)
            markSelectedObjectsButton.SharedProps.Caption = InformationHubStrings.MarkSelObjs_CtxtMnu_Caption
            markSelectedObjectsButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.MarkSelectedObjects.ToBitmap

            Dim memolistAddSelectedButton As New ButtonTool(InfoHubMenuToolKey.MemolistAddSelected.ToString)
            memolistAddSelectedButton.SharedProps.Caption = InformationHubStrings.AddToMemList_CtxtMnu_Caption
            memolistAddSelectedButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.Memolist.ToBitmap


            Dim showSpliceProposals As New ButtonTool(InfoHubMenuToolKey.ShowSpliceProposals.ToString)
            showSpliceProposals.SharedProps.Caption = InformationHubStrings.ShowSpliceProposals
            showSpliceProposals.SharedProps.Visible = False

            Dim showRubberlines As New ButtonTool(InfoHubMenuToolKey.ShowRubberlinesToCorrespondingConnectors.ToString)
            showRubberlines.SharedProps.Caption = InformationHubStrings.ShowRubberlines
            showRubberlines.SharedProps.Visible = False


            Me.utmInformationHub.Tools.AddRange(New ToolBase() {_contextMenuGrid, calculateDistanceButton, calculateModuleWeightButton, calculateTotalLengthButton, calculateWireWeightButton, calculateWireResistanceButton, clearMarkedObjectsButton, editButton, editRedliningButton, highlightEntireRoutingPathButton, markSelectedObjectsButton, memolistAddSelectedButton, selectMarkedObjectsButton, showBundleCrossSectionButton, showConnectivityButton, showContactingButton, showCorrespondingMenu, showEntireRoutingPathButton, showModulesButton, showOverallConnectivityButton, showSchematicsView, showStartEndConnectors, showSpliceProposals, showRubberlines})

            .Tools.AddTool(editButton.Key)
            .Tools.AddTool(editRedliningButton.Key)

            .Tools.AddTool(highlightEntireRoutingPathButton.Key)
            .Tools.AddTool(showEntireRoutingPathButton.Key)
            .Tools.AddTool(showOverallConnectivityButton.Key)

            .Tools.AddTool(showCorrespondingMenu.Key)
            .Tools.AddTool(showBundleCrossSectionButton.Key)
            .Tools.AddTool(showConnectivityButton.Key)
            .Tools.AddTool(showContactingButton.Key)
            .Tools.AddTool(showModulesButton.Key)
            .Tools.AddTool(showSchematicsView.Key)
            .Tools.AddTool(showStartEndConnectors.Key)

            .Tools.AddTool(calculateDistanceButton.Key)
            .Tools.AddTool(calculateModuleWeightButton.Key)
            .Tools.AddTool(calculateTotalLengthButton.Key)
            .Tools.AddTool(calculateWireResistanceButton.Key)
            .Tools.AddTool(calculateWireWeightButton.Key)

            .Tools.AddTool(clearMarkedObjectsButton.Key)
            .Tools.AddTool(selectMarkedObjectsButton.Key)

            .Tools.AddTool(markSelectedObjectsButton.Key)

            .Tools.AddTool(memolistAddSelectedButton.Key)
            .Tools.AddTool(showSpliceProposals.Key)
            .Tools.AddTool(showRubberlines.Key)

            .Tools(calculateDistanceButton.Key).InstanceProps.IsFirstInGroup = True
            .Tools(calculateTotalLengthButton.Key).InstanceProps.IsFirstInGroup = True
            .Tools(calculateWireResistanceButton.Key).InstanceProps.IsFirstInGroup = True
            .Tools(clearMarkedObjectsButton.Key).InstanceProps.IsFirstInGroup = True
            .Tools(highlightEntireRoutingPathButton.Key).InstanceProps.IsFirstInGroup = True
            .Tools(markSelectedObjectsButton.Key).InstanceProps.IsFirstInGroup = True
            .Tools(memolistAddSelectedButton.Key).InstanceProps.IsFirstInGroup = True
            .Tools(showCorrespondingMenu.Key).InstanceProps.IsFirstInGroup = True

            .Tools(showSpliceProposals.Key).InstanceProps.IsFirstInGroup = True
            .Tools(showRubberlines.Key).InstanceProps.IsFirstInGroup = True

        End With

        With _contextMenuTab
            .DropDownArrowStyle = DropDownArrowStyle.None

            Dim collapseAllButton As New ButtonTool(InfoHubMenuToolKey.CollapseAll.ToString)
            collapseAllButton.SharedProps.Caption = InformationHubStrings.CollapseAll_CtxtMnu_Caption
            collapseAllButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.CollapseAll.ToBitmap

            Dim expandAllButton As New ButtonTool(InfoHubMenuToolKey.ExpandAll.ToString)
            expandAllButton.SharedProps.Caption = InformationHubStrings.ExpandAll_CtxtMnu_Caption
            expandAllButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.ExpandAll.ToBitmap

            Dim exportExcelButton As New ButtonTool(InfoHubMenuToolKey.ExportExcel.ToString)
            exportExcelButton.SharedProps.Caption = InformationHubStrings.ExportExcel_CtxtMnu_Caption
            exportExcelButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.ExportExcel.ToBitmap

            Dim clearFiltersButton As New ButtonTool(InfoHubMenuToolKey.ClearFilters.ToString)
            clearFiltersButton.SharedProps.Caption = InformationHubStrings.ClearFilters_CtxtMnu_Caption
            clearFiltersButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.ClearFilters.ToBitmap

            Dim clearAllFiltersButton As New ButtonTool(InfoHubMenuToolKey.ClearAllFilters.ToString)
            clearAllFiltersButton.SharedProps.Caption = InformationHubStrings.ClearAllFilters_CtxtMnu_Caption
            clearAllFiltersButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.ClearAllFilters.ToBitmap

            Me.utmInformationHub.Tools.AddRange(New ToolBase() {_contextMenuTab, collapseAllButton, expandAllButton, exportExcelButton, clearFiltersButton, clearAllFiltersButton})

            .Tools.AddTool(collapseAllButton.Key)
            .Tools.AddTool(expandAllButton.Key)
            .Tools.AddTool(exportExcelButton.Key)
            .Tools.AddTool(clearFiltersButton.Key)
            .Tools.AddTool(clearAllFiltersButton.Key)
        End With
    End Sub

    Private Sub FillDataSource(dataSource As UltraDataSource, gridAppearance As GridAppearance, rowCount As Integer, Optional comparisonMapper As ComparisonMapper = Nothing)
        If Not _filledDataSources.Contains(dataSource) Then
            _filledDataSources.Add(dataSource)
        End If

        If gridAppearance.GridTable Is Nothing Then
            If TryCast(DocumentForm, DocumentForm)?.MainForm IsNot Nothing AndAlso DebugEx.IsDebug Then 'HINT: when main form is null this means the parent documentForm run's in standalone-mode -> no appearances -> don't raise any exception because it's normal to have this behavior in this case
                Throw New NullReferenceException($"Please check GridAppearance ""{gridAppearance.GetType.Name}"" does Not have a GridTable (maybe there was an Error loading appearances before?).In Release we will create a Default table here an proceed normally!")
            Else ' in standalone and in release just create the default table when gridTable is nothing
                gridAppearance.CreateDefaultTable()
            End If
        End If

        Dim sortedGridColumns As New SortedDictionary(Of Integer, GridColumn)
        If gridAppearance?.GridTable IsNot Nothing Then
            For Each gridColumn As GridColumn In gridAppearance.GridTable.GridColumns
                sortedGridColumns.Add(gridColumn.Ordinal, gridColumn)
            Next
        End If

        With dataSource
            With dataSource.Band
                dataSource.Band.Key = (gridAppearance?.GridTable?.Type).GetValueOrDefault.ToString

                If (_kblMapperForCompare Is Nothing) AndAlso (TypeOf gridAppearance Is ComponentGridAppearance) Then
                    .Columns.TryAddOrGet(COMPONENT_CLASS_COLUMN_KEY)
                ElseIf (_kblMapperForCompare Is Nothing) AndAlso (TypeOf gridAppearance Is WireGridAppearance) Then
                    .Columns.TryAddOrGet(WIRE_CLASS_COLUMN_KEY)
                ElseIf (_kblMapperForCompare IsNot Nothing) Then
                    AddCompareColumns(.Columns, True)
                End If

                For Each gridColumn As GridColumn In sortedGridColumns.Values
                    If gridColumn.KBLPropertyName = WirePropertyName.Length_information.ToString Then
                        If _kblMapperForCompare IsNot Nothing Then
                            .Columns.Add(PRIMARY_LENGTH_COLUMN_KEY)
                        End If

                    End If
                    .Columns.TryAddOrGet(gridColumn.KBLPropertyName)
                    If (_kblMapperForCompare Is Nothing) AndAlso (TypeOf gridAppearance Is CableGridAppearance OrElse TypeOf gridAppearance Is WireGridAppearance) AndAlso (gridColumn.KBLPropertyName = CablePropertyName.Length_Information.ToString) Then
                        .Columns.TryAddOrGet(InformationHubStrings.AddLengthInfo_ColumnCaption)
                    End If
                Next

                If (_kblMapperForCompare Is Nothing) AndAlso (TypeOf gridAppearance IsNot ModuleGridAppearance) Then
                    .Columns.TryAddOrGet(InformationHubStrings.AssignedModules_ColumnCaption)
                End If

                If (_kblMapperForCompare IsNot Nothing) Then
                    If (TypeOf gridAppearance Is FixingGridAppearance OrElse TypeOf gridAppearance Is AccessoryGridAppearance OrElse TypeOf gridAppearance Is ConnectorGridAppearance) Then
                        .Columns.TryAddOrGet(PLACEMENT) 'HINT Column to keep the modification of the placement vector for 3D compare
                    End If

                    'HINT these are removed as they are shown in the fixing assignment changes on the segment
                    If (TypeOf gridAppearance Is FixingGridAppearance) AndAlso .Columns.Exists(FixingPropertyName.SegmentLocation) Then
                        .Columns.Remove(FixingPropertyName.SegmentLocation)
                    End If

                    If (TypeOf gridAppearance Is FixingGridAppearance) AndAlso .Columns.Exists(FixingPropertyName.SegmentAbsolute_location) Then
                        .Columns.Remove(FixingPropertyName.SegmentAbsolute_location)
                    End If

                    If (TypeOf gridAppearance Is AccessoryGridAppearance) AndAlso .Columns.Exists(AccessoryPropertyName.SegmentLocation) Then
                        .Columns.Remove(AccessoryPropertyName.SegmentLocation)
                    End If

                    If (TypeOf gridAppearance Is AccessoryGridAppearance) AndAlso .Columns.Exists(AccessoryPropertyName.SegmentAbsolute_location) Then
                        .Columns.Remove(AccessoryPropertyName.SegmentAbsolute_location)
                    End If

                    If (TypeOf gridAppearance Is WireGridAppearance) AndAlso .Columns.Exists(WirePropertyName.AdditionalExtremities) Then
                        .Columns.Remove(WirePropertyName.AdditionalExtremities)
                    End If
                End If

                If (gridAppearance?.GridTable?.GridSubTable IsNot Nothing) Then
                    .ChildBands.Add(gridAppearance.GridTable.GridSubTable.Type.ToString)

                    sortedGridColumns.Clear()

                    For Each gridColumn As GridColumn In gridAppearance.GridTable.GridSubTable.GridColumns
                        sortedGridColumns.Add(gridColumn.Ordinal, gridColumn)
                    Next

                    If .ChildBands?.Count > 0 Then 'hint: normally shouldn't be possible
                        .ChildBands(0).Columns.Add(SYSTEM_ID_COLUMN_KEY)

                        If (_kblMapperForCompare IsNot Nothing) Then
                            AddCompareColumns(.ChildBands(0).Columns, False)
                        End If

                        For Each gridColumn As GridColumn In sortedGridColumns.Values
                            .ChildBands(0).Columns.TryAddOrGet(gridColumn.KBLPropertyName)

                            If (_kblMapperForCompare Is Nothing) AndAlso (TypeOf gridAppearance Is CableGridAppearance) AndAlso (gridColumn.KBLPropertyName = CablePropertyName.Length_Information) Then
                                .ChildBands(0).Columns.TryAddOrGet(InformationHubStrings.AddLengthInfo_ColumnCaption)
                            End If
                        Next

                        If (_kblMapperForCompare Is Nothing) AndAlso (TypeOf gridAppearance IsNot ModuleGridAppearance) Then
                            .ChildBands(0).Columns.TryAddOrGet(InformationHubStrings.AssignedModules_ColumnCaption)
                        End If

                        If _kblMapperForCompare IsNot Nothing Then
                            If (TypeOf gridAppearance Is ConnectorGridAppearance) Then
                                .ChildBands(0).Columns.Remove(ConnectorPropertyName.Plating)
                            End If
                        End If
                    Else
                        Throw New Exception($"Unexpected behavior detected: {NameOf(dataSource)}.{NameOf(dataSource.Band)}.{NameOf(dataSource.Band.ChildBands)} are empty! Please check the BL of the {NameOf(FillDataSource)}-method.{vbCrLf}Expected: a child-band (with the key ""{SYSTEM_ID_COLUMN_KEY}"") should have been added before this point!")
                    End If
                End If
            End With

            If comparisonMapper Is Nothing Then
                .Rows.SetCount(rowCount)
            ElseIf comparisonMapper.HasChanges Then
                Dim compareObjects As New Dictionary(Of String, Object)
                For Each cItem As ChangedItem In comparisonMapper.Changes
                    compareObjects.Add(String.Format("{0}|{1}", cItem.Text, cItem.Key), cItem.Item)
                Next

                .Band.Tag = compareObjects
                .Rows.SetCount(compareObjects.Count)
            End If
        End With
    End Sub

    Private Function GetAssignedModules(kblId As String) As String
        Dim assignedModules As New System.Text.StringBuilder
        Dim modules As New HashSet(Of String)

        For Each m As [Lib].Schema.Kbl.Module In _kblMapper.GetModulesOfObject(kblId)
            If modules.Add(m.SystemId) Then
                assignedModules.AppendLine($"{m.Abbreviation } [{m.Part_number }]{2}")
            End If
        Next

        Return assignedModules.ToString
    End Function

    Private Sub GetAssignedModulesCellValueForCavity(ByVal e As CellDataRequestedEventArgs, cavityOccurrence As Cavity_occurrence, sealOccurrence As Cavity_seal_occurrence, specialTerminalOccurrence As Special_terminal_occurrence, terminalOccurrence As Terminal_occurrence)
        If (terminalOccurrence IsNot Nothing) Then
            If (e.Data IsNot Nothing) Then
                e.Data = String.Format("{0}{1}{3}{1}{2}", e.Data.ToString, vbCrLf, GetAssignedModules(terminalOccurrence.SystemId), KblObjectType.Terminal_occurrence.ToLocalizedPluralString)
            Else
                e.Data = String.Format("{2}{0}{1}", vbCrLf, GetAssignedModules(terminalOccurrence.SystemId), KblObjectType.Terminal_occurrence.ToLocalizedPluralString)
            End If
        End If

        If (specialTerminalOccurrence IsNot Nothing) Then
            If (e.Data IsNot Nothing) Then
                e.Data = String.Format("{0}{1}{3}{1}{2}", e.Data.ToString, vbCrLf, GetAssignedModules(specialTerminalOccurrence.SystemId), KblObjectType.Terminal_occurrence.ToLocalizedPluralString)
            Else
                e.Data = String.Format("{2}{0}{1}", vbCrLf, GetAssignedModules(specialTerminalOccurrence.SystemId), KblObjectType.Terminal_occurrence.ToLocalizedPluralString)
            End If
        End If

        If (sealOccurrence IsNot Nothing) Then
            If (e.Data IsNot Nothing) Then
                e.Data = String.Format("{0}{1}{3}{1}{2}", e.Data.ToString, vbCrLf, GetAssignedModules(sealOccurrence.SystemId), KblObjectType.Cavity_seal_occurrence.ToLocalizedPluralString)
            Else
                e.Data = String.Format("{2}{0}{1}", vbCrLf, GetAssignedModules(sealOccurrence.SystemId), KblObjectType.Cavity_seal_occurrence.ToLocalizedPluralString)
            End If
        End If

        If Not String.IsNullOrEmpty(cavityOccurrence.Associated_plug) Then
            Dim cav_occ As Cavity_plug_occurrence = _kblMapper.GetOccurrenceObject(Of Cavity_plug_occurrence)(cavityOccurrence.Associated_plug)
            If cav_occ IsNot Nothing Then
                If (e.Data IsNot Nothing) Then
                    e.Data = String.Format("{0}{1}{3}{1}{2}", e.Data.ToString, vbCrLf, GetAssignedModules(cav_occ.SystemId), KblObjectType.Cavity_plug_occurrence.ToLocalizedString)
                Else
                    e.Data = String.Format("{2}{0}{1}", vbCrLf, GetAssignedModules(cav_occ.SystemId), KblObjectType.Cavity_plug_occurrence.ToLocalizedString)
                End If
            End If
        End If
    End Sub

    Private Function GetChangedContactPointProperty(val As Object) As CommonChangedProperty
        'HINT this seems to be the purest nonsense!
        Dim changedProperty As New CommonChangedProperty(Nothing, _kblMapper, _kblMapperForCompare, _generalSettings)

        If (TypeOf val Is Contact_point) Then
            Return Nothing
        End If

        Dim val1 As Object = DirectCast(val, Runtime.CompilerServices.ITuple).Item(0)
        Dim val2 As Object = DirectCast(val, Runtime.CompilerServices.ITuple).Item(1)

        If TypeOf val1 Is SingleObjectComparisonMapper Then
            changedProperty.ChangedProperties.Add(ConnectorPropertyName.Terminal_part_information, val1)

            Dim partChangedProperty As PartChangedProperty = TryCast(DirectCast(val1, SingleObjectComparisonMapper).Changes.ModifiedItems.FirstOrDefault, HarnessAnalyzer.PartChangedProperty)
            If (partChangedProperty IsNot Nothing) AndAlso (partChangedProperty.ChangedProperties.ContainsKey(PartPropertyName.Part_description)) Then
                changedProperty.ChangedProperties.Add(ConnectorPropertyName.Description, partChangedProperty.ChangedProperties(PartPropertyName.Part_description))
            End If

        ElseIf TypeOf val1 Is String AndAlso val1.ToString <> "Original" Then
            changedProperty.ChangedProperties.Add(ConnectorPropertyName.Description, val1)
            changedProperty.ChangedProperties.Add(ConnectorPropertyName.Terminal_part_number, val1)
        End If

        If (TypeOf val2 Is SingleObjectComparisonMapper) Then
            changedProperty.ChangedProperties.Add(ConnectorPropertyName.Seal_part_information, val1)
        ElseIf (TypeOf val2 Is String) AndAlso (val2.ToString <> String.Empty) AndAlso (val2.ToString <> "Original") Then
            changedProperty.ChangedProperties.Add(ConnectorPropertyName.Seal_part_number, val2)
        End If

        Return changedProperty
    End Function

    Private Function GetExportCellValuesFromAliasIds(node As Node, part As Part, segment As Segment) As String
        Dim alias_id As Alias_identification() = Array.Empty(Of Alias_identification)

        If (node IsNot Nothing) Then
            alias_id = node.Alias_id
        End If

        If (part IsNot Nothing) Then
            alias_id = part.Alias_id
        End If

        If (segment IsNot Nothing) Then
            alias_id = segment.Alias_id
        End If

        Dim cellValueStr As New System.Text.StringBuilder
        For Each aliasId As Alias_identification In alias_id.OrEmpty
            cellValueStr.AppendLine(String.Format(InformationHubStrings.Id_ExcelComment, aliasId.Alias_id))
            If (aliasId.Scope IsNot Nothing) Then
                cellValueStr.Append(String.Format(InformationHubStrings.Scope_ExcelComment, aliasId.Scope))
            End If
            If (aliasId.Description IsNot Nothing) Then
                cellValueStr.Append(String.Format(InformationHubStrings.Description_ExcelComment, aliasId.Description))
            End If
        Next
        Return cellValueStr.ToString
    End Function

    Private Function GetExportCellValuesFromChanges(part As Part) As String
        Dim change_cell_values As New System.Text.StringBuilder
        If part IsNot Nothing Then
            For Each change As Change In part.Change
                Dim cell_value_list As New List(Of String)
                If (change.SystemId IsNot Nothing) Then
                    cell_value_list.Add(String.Format(InformationHubStrings.Id_ExcelComment, change.Id))
                End If

                If (change.Description IsNot Nothing) Then
                    cell_value_list.Add(String.Format(InformationHubStrings.Description_ExcelComment, change.Description))
                End If

                If (change.Change_request IsNot Nothing) Then
                    cell_value_list.Add(String.Format(InformationHubStrings.ChangeReq_ExcelComment, change.Change_request))
                End If

                If (change.Change_date IsNot Nothing) Then
                    cell_value_list.Add(String.Format(InformationHubStrings.ChangeDate_ExcelComment, change.Change_date))
                End If

                change_cell_values.AppendLine(String.Join(String.Empty, cell_value_list))
                change_cell_values.Append(String.Format(InformationHubStrings.RespDes_ExcelComment, change.Responsible_designer))
                change_cell_values.Append(String.Format(InformationHubStrings.DesDep_ExcelComment, change.Designer_department))

                If (change.Approver_name IsNot Nothing) Then
                    change_cell_values.Append(String.Format(InformationHubStrings.ApprName_ExcelComment, change.Approver_name))
                End If

                If (change.Approver_department IsNot Nothing) Then
                    change_cell_values.Append(String.Format(InformationHubStrings.ApprDep_ExcelComment, change.Approver_department))
                End If
            Next
        End If
        Return change_cell_values.ToString
    End Function

    Private Function GetExportCellValuesFromCrossSectionAreaInformation(segment As Segment) As String
        Dim cell_values As New System.Text.StringBuilder
        If segment IsNot Nothing Then
            For Each crossSectionArea As Cross_section_area In segment.Cross_section_area_information
                With crossSectionArea
                    cell_values.AppendLine(String.Format(InformationHubStrings.ValDet_ExcelComment, crossSectionArea.Value_determination))

                    If crossSectionArea.Area IsNot Nothing Then
                        If _kblMapper.KBLUnitMapper.ContainsKey(.Area.Unit_component) Then
                            cell_values.Append(String.Format(InformationHubStrings.Area_ExcelComment, String.Format("{0} {1}", Math.Round(.Area.Value_component, 2), _kblMapper.GetUnit(.Area.Unit_component).Unit_name)))
                        ElseIf (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLUnitMapper.ContainsKey(.Area.Unit_component)) Then
                            cell_values.Append(String.Format(InformationHubStrings.Area_ExcelComment, String.Format("{0} {1}", Math.Round(.Area.Value_component, 2), _kblMapperForCompare.GetUnit(.Area.Unit_component).Unit_name)))
                        Else
                            cell_values.Append(String.Format(InformationHubStrings.Area_ExcelComment, Math.Round(.Area.Value_component, 2)))
                        End If
                    End If
                End With
            Next
        End If
        Return cell_values.ToString
    End Function

    Private Function GetExportCellValuesFromExternalReferences(part As Part) As String
        Dim externalReferences As New List(Of External_reference)

        If part?.External_references IsNot Nothing Then
            For Each externalReference As String In part.External_references.SplitSpace
                Dim ext_ref As External_reference = _kblMapper.GetOccurrenceObject(Of External_reference)(externalReference)
                If ext_ref IsNot Nothing Then
                    externalReferences.Add(ext_ref)
                End If
            Next
        End If

        Dim cell_values As New System.Text.StringBuilder
        For Each externalReference As External_reference In externalReferences
            cell_values.AppendLine(String.Format(InformationHubStrings.DocTyp_ExcelComment, externalReference.Document_type))
            cell_values.Append(String.Format(InformationHubStrings.DocNum_ExcelComment, externalReference.Document_number))
            cell_values.Append(String.Format(InformationHubStrings.ChangeLvl_ExcelComment, externalReference.Change_level))

            If externalReference.File_name IsNot Nothing Then
                cell_values.Append(String.Format(InformationHubStrings.FileName_ExcelComment, externalReference.File_name))
            End If

            If externalReference.Location IsNot Nothing Then
                cell_values.Append(String.Format(InformationHubStrings.Loc_ExcelComment, externalReference.Location))
            End If

            cell_values.Append(String.Format(InformationHubStrings.DataFormat_ExcelComment, externalReference.Data_format))

            If externalReference.Creating_system IsNot Nothing Then
                cell_values.Append(String.Format(InformationHubStrings.CreatingSys_ExcelComment, externalReference.Creating_system))
            End If
        Next
        Return cell_values.ToString
    End Function

    Private Function GetExportCellValuesFromFixingAssignment(segment As Segment) As String
        Dim cell_values As New System.Text.StringBuilder
        If segment IsNot Nothing Then
            For Each fixingAssignment As Fixing_assignment In segment.Fixing_assignment
                Dim fixing_occ As IKblOccurrence = _kblMapper.GetOccurrenceObjectUntyped(fixingAssignment.Fixing)
                If fixing_occ IsNot Nothing Then
                    Dim id As String = If(fixingAssignment.Id, fixingAssignment.Fixing) 'HINT: Serializer is no longer filling in the id (sometimes?) but there is also no id in the kbl, in old trunk this id filled with the same value as the .Fixing-Property

                    Dim alias_id_values As New System.Text.StringBuilder
                    For Each aliasId As Alias_identification In fixingAssignment.Alias_id
                        alias_id_values.Append(String.Format(InformationHubStrings.AliasId_TooltipPart, aliasId.Alias_id, If(aliasId.Scope, "-"), If(aliasId.Description, "-")))
                    Next

                    If id IsNot Nothing Then
                        cell_values.AppendLine(String.Format(InformationHubStrings.Id_ExcelComment, id) + alias_id_values.ToString)
                    Else
                        cell_values.AppendLine(alias_id_values.ToString)
                    End If

                    cell_values.Append(String.Format(InformationHubStrings.Loc_ExcelComment, fixingAssignment.Location))

                    If _kblMapper.KBLOccurrenceMapper.ContainsKey(fixingAssignment.Fixing) Then
                        cell_values.Append(String.Format(InformationHubStrings.AccFixName_ExcelComment, E3.Lib.Schema.Kbl.Harness.GetReferenceElement(fixingAssignment.Fixing, _kblMapper)))
                    Else
                        cell_values.Append(String.Format(InformationHubStrings.AccFixName_ExcelComment, fixingAssignment.Fixing))
                    End If
                End If
            Next
        End If
        Return cell_values.ToString
    End Function

    Private Function GetExportCellValuesFromMounting(component As Component_occurrence) As String
        Dim cell_values As New System.Text.StringBuilder
        If component IsNot Nothing Then
            For Each mounting As String In component.Mounting?.SplitSpace.OrEmpty
                Dim mountOcc As IKblOccurrence = _kblMapper.GetOccurrenceObjectUntyped(mounting)
                If mountOcc IsNot Nothing Then
                    Select Case mountOcc.ObjectType
                        Case KblObjectType.Cavity_occurrence
                            Dim connectorOccurrence As Connector_occurrence = _kblMapper.GetOccurrenceObject(Of Connector_occurrence)(_kblMapper.KBLCavityConnectorMapper(mounting))
                            Dim cavityPart As Cavity = _kblMapper.GetPart(Of Cavity)(mountOcc.Part)
                            cell_values.AppendLine(String.Format(InformationHubStrings.MountingCav_ExcelComment, connectorOccurrence.Id, cavityPart.Cavity_number))
                        Case KblObjectType.Connector_occurrence
                            cell_values.AppendLine(String.Format(InformationHubStrings.MountingConn_ExcelComment, mountOcc.Id))
                        Case KblObjectType.Slot_occurrence
                            Dim connectorOccurrence As Connector_occurrence = _kblMapper.GetOccurrenceObject(Of Connector_occurrence)(_kblMapper.KBLSlotConnectorMapper(mounting))
                            Dim cavityOccurrence As Cavity_occurrence = DirectCast(mountOcc, Slot_occurrence).Cavities.FirstOrDefault
                            If cavityOccurrence IsNot Nothing Then
                                cell_values.AppendLine(String.Format(InformationHubStrings.MountingSlot_ExcelComment, connectorOccurrence.Id, _kblMapper.GetPart(Of Cavity)(cavityOccurrence.Part).Cavity_number))
                            End If
                    End Select
                End If
            Next
        End If
        Return cell_values.ToString
    End Function

    Private Function GetExportCellValuesFromProcessingInformation(node As Node, part As Part, assignment As Fixing_assignment) As String
        Dim processingInformation As Processing_instruction() = Array.Empty(Of Processing_instruction)
        If node IsNot Nothing Then
            processingInformation = node.Processing_information
        ElseIf part IsNot Nothing Then
            processingInformation = part.Processing_information
        ElseIf assignment IsNot Nothing Then
            processingInformation = assignment.Processing_information
        End If

        If processingInformation IsNot Nothing Then
            Dim cell_values As New System.Text.StringBuilder
            For Each processingInstruction As Processing_instruction In processingInformation
                cell_values.AppendLine(String.Format(InformationHubStrings.InstrType_ExcelComment, processingInstruction.Instruction_type))
                cell_values.Append(String.Format(InformationHubStrings.InstrVal_ExcelComment, processingInstruction.Instruction_value))
            Next
            Return cell_values.ToString
        End If
        Return String.Empty
    End Function

    Private Function GetExportCellValuesFromReferencedComponents(node As Node) As String
        Dim cell_values As New System.Text.StringBuilder
        If node IsNot Nothing AndAlso Not String.IsNullOrEmpty(node.Referenced_components) Then
            For Each referencedComponent As String In node.Referenced_components.SplitSpace
                Dim occ As IKblOccurrence = _kblMapper.GetOccurrenceObjectUntyped(referencedComponent)
                If occ IsNot Nothing Then
                    Select Case occ.ObjectType
                        Case KblObjectType.Accessory_occurrence,
                             KblObjectType.Assembly_part_occurrence,
                             KblObjectType.Connector_occurrence,
                             KblObjectType.Fixing_occurrence,
                             KblObjectType.Wire_protection_occurrence
                            cell_values.AppendLine(String.Format("{0} [{1}]", referencedComponent, occ.Id))
                    End Select
                End If
            Next
        End If
        Return cell_values.ToString
    End Function

    Private Function GetExportCellValuesFromRouting(tag As Object) As String
        Dim ids As New List(Of String)

        If tag IsNot Nothing Then
            Select Case tag.GetType.GetKblObjectType
                Case KblObjectType.Core_occurrence
                    ids.Add(DirectCast(tag, Core_occurrence).SystemId)
                Case KblObjectType.Special_wire_occurrence
                    For Each coreOccurrence As Core_occurrence In DirectCast(tag, Special_wire_occurrence).Core_occurrence
                        ids.Add(coreOccurrence.SystemId)
                    Next
                Case Else
                    ids.Add(DirectCast(tag, Wire_occurrence).SystemId)
            End Select
        End If

        Dim segmentIds As New List(Of String)

        For Each id As String In ids
            For Each segment As Segment In _kblMapper.KBLWireSegmentMapper.GetSegmentsOrEmpty(id)
                segmentIds.TryAdd(segment.Id)
            Next
        Next

        segmentIds.Sort()

        Dim cell_values As New System.Text.StringBuilder
        For Each segmentId As String In segmentIds
            cell_values.AppendLine(String.Format(InformationHubStrings.Seg_ExcelComment, segmentId))
        Next
        Return cell_values.ToString
    End Function

    Private Function GetExportCellValuesFromWireLengthes(columnKey As String, wireLengthList As IEnumerable(Of Wire_length)) As String
        Dim cell_values As New System.Text.StringBuilder
        For Each wireLength As Wire_length In wireLengthList
            If (columnKey <> NameOf(InformationHubStrings.AddLengthInfo_ColumnCaption)) OrElse (_generalSettings.DefaultWireLengthType.ToLower <> wireLength.Length_type.ToLower) Then
                cell_values.AppendLine(String.Format(InformationHubStrings.LengthType_ExcelComment, wireLength.Length_type))

                If _kblMapper.KBLUnitMapper.ContainsKey(wireLength.Length_value.Unit_component) Then
                    cell_values.Append(String.Format(InformationHubStrings.LengthValue_ExcelComment, String.Format("{0} {1}", Math.Round(wireLength.Length_value.Value_component, 2), _kblMapper.KBLUnitMapper(wireLength.Length_value.Unit_component).Unit_name)))
                ElseIf (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLUnitMapper.ContainsKey(wireLength.Length_value.Unit_component)) Then
                    cell_values.Append(String.Format(InformationHubStrings.LengthValue_ExcelComment, String.Format("{0} {1}", Math.Round(wireLength.Length_value.Value_component, 2), _kblMapperForCompare.KBLUnitMapper(wireLength.Length_value.Unit_component).Unit_name)))
                Else
                    cell_values.Append(String.Format(InformationHubStrings.LengthValue_ExcelComment, Math.Round(wireLength.Length_value.Value_component, 2)))
                End If
            End If
        Next
        Return cell_values.ToString
    End Function

    Private Function GetKblIdsFromRootGroup_Recursivley(childGroups As vdEntities) As List(Of String)
        For Each group As VdSVGGroup In childGroups
            If (group.KblId <> String.Empty) Then
                Dim kblIds As New List(Of String) From
                {
                    group.KblId
                }
                kblIds.AddRange(group.SecondaryKblIds)

                Return kblIds
            Else
                GetKblIdsFromRootGroup_Recursivley(group.ChildGroups)
            End If
        Next

        Return New List(Of String)
    End Function

    Private Function GetObjectReferences(selectedGridRows As List(Of UltraGridRow)) As List(Of ObjectReference)
        Dim objectReferences As New List(Of ObjectReference)

        For Each selectedGridRow As UltraGridRow In selectedGridRows
            If selectedGridRow.Tag Is Nothing Then
                Continue For
            End If

            Dim objectId As String = selectedGridRow.Tag?.ToString
            Dim objectName As String = String.Empty
            Dim objectType As KblObjectType = KblObjectType.Undefined

            If (selectedGridRow.Cells.Exists(SYSTEM_ID_COLUMN_KEY)) Then
                objectId = selectedGridRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString
            End If

            If [Enum].TryParse(Of KblObjectType)(selectedGridRow.Band.Key, objectType) Then
                If objectType = KblObjectType.Redlining Then
                    objectId = selectedGridRow.Cells("ObjectId").Value.ToString
                    objectName = selectedGridRow.Cells(NameOf(InformationHubStrings.ObjName_ColumnCaption)).Value.ToString
                    objectType = [Enum].Parse(Of KblObjectType)(selectedGridRow.Cells(NameOf(InformationHubStrings.ObjType_ColumnCaption)).Value.ToString)
                Else
                    Dim obj_ref_info As KblObjRefInfo = E3.Lib.Schema.Kbl.Utils.GetKblObjectRefInfo(objectId, objectType, _kblMapper)
                    objectId = obj_ref_info.Id
                    objectType = obj_ref_info.Type
                    objectName = obj_ref_info.Name
                End If
            Else
                objectType = KblObjectType.Undefined
            End If

            If Not String.IsNullOrEmpty(objectName) AndAlso (objectType <> KblObjectType.Undefined) Then
                objectReferences.Add(New ObjectReference(objectId, objectName, objectType))
            End If
        Next

        If (objectReferences.Any(Function(objRef) objRef.ObjectType = E3.Lib.Schema.Kbl.KblObjectType.Core_occurrence OrElse objRef.ObjectType = E3.Lib.Schema.Kbl.KblObjectType.Wire_occurrence)) Then
            If (Me.utcInformationHub.ActiveTab.Text = KblObjectType.Special_wire_occurrence.ToLocalizedPluralString) Then
                For Each row As UltraGridRow In Me.ugNets.Rows
                    If row.HasChild Then
                        For Each childRow As UltraGridRow In row.ChildBands(0).Rows
                            If (objectReferences.Any(Function(objRef) objRef.KblId = childRow.Tag?.ToString)) Then
                                selectedGridRows.Add(childRow)
                            End If
                        Next
                    End If
                Next

                For Each row As UltraGridRow In Me.ugWires.Rows
                    If (objectReferences.Any(Function(objRef) objRef.KblId = row.Tag?.ToString)) Then
                        selectedGridRows.Add(row)
                    End If
                Next
            ElseIf Me.utcInformationHub.ActiveTab.Text = String.Format("{0}/{1}", KblObjectType.Wire_occurrence.ToLocalizedPluralString, KblObjectType.Core_occurrence.ToLocalizedPluralString) Then
                For Each row As UltraGridRow In Me.ugCables.Rows
                    If row.HasChild Then
                        For Each childRow As UltraGridRow In row.ChildBands(0).Rows
                            If (objectReferences.Any(Function(objRef) objRef.KblId = childRow.Tag?.ToString)) Then
                                selectedGridRows.Add(childRow)
                            End If
                        Next
                    End If
                Next

                For Each row As UltraGridRow In Me.ugNets.Rows
                    If row.HasChild Then
                        For Each childRow As UltraGridRow In row.ChildBands(0).Rows
                            If (objectReferences.Any(Function(objRef) objRef.KblId = childRow.Tag?.ToString)) Then
                                selectedGridRows.Add(childRow)
                            End If
                        Next
                    End If
                Next
            Else
                For Each row As UltraGridRow In Me.ugCables.Rows
                    If row.HasChild Then
                        For Each childRow As UltraGridRow In row.ChildBands(0).Rows
                            If (objectReferences.Any(Function(objRef) objRef.KblId = childRow.Tag?.ToString)) Then
                                selectedGridRows.Add(childRow)
                            End If
                        Next
                    End If
                Next

                For Each row As UltraGridRow In Me.ugWires.Rows
                    If (objectReferences.Any(Function(objRef) objRef.KblId = row.Tag?.ToString)) Then
                        selectedGridRows.Add(row)
                    End If
                Next
            End If
        End If

        Return objectReferences
    End Function

    Private Function GetRunningE3Objects() As Dictionary(Of Integer, Object)
        Dim foundRunningE3Instances As New Dictionary(Of Integer, Object)()
        Dim bindCtx As IBindCtx = Nothing
        Dim runningObjectTable As IRunningObjectTable = Nothing

        Try
            Dim enumMoniker As IEnumMoniker = Nothing

            Dim hRes As HResult = System.Windows.Native.CreateBindCtx(0, bindCtx)
            If hRes = HResult.OutOfMemory Then
                Throw New OutOfMemoryException("Coudn't created IBindCtx!")
            End If

            Try
                bindCtx.GetRunningObjectTable(runningObjectTable)
                runningObjectTable.EnumRunning(enumMoniker)

                Dim monikers As IMoniker() = New IMoniker(0) {}
                Dim fetched As IntPtr = IntPtr.Zero

                While enumMoniker.[Next](1, monikers, fetched) = 0
                    Dim moniker As IMoniker = monikers(0)
                    If moniker IsNot Nothing Then
                        Dim displayName As String = String.Empty
                        moniker.GetDisplayName(bindCtx, Nothing, displayName)

                        If displayName.ToUpperInvariant().StartsWith(E3_APPLICATION_PREFIX) Then
                            Dim tempArray As String() = displayName.Split(":"c, StringSplitOptions.RemoveEmptyEntries)
                            If (tempArray.Length = 2) Then
                                Dim processId As Integer
                                If Integer.TryParse(tempArray(1), processId) Then
                                    Dim objRef As Object = Nothing
                                    runningObjectTable.GetObject(moniker, objRef)
                                    foundRunningE3Instances.Add(processId, objRef)
                                End If
                            End If
                        End If
                    End If
                End While
            Catch exception As Exception
                _logEventArgs.LogLevel = LogEventArgs.LoggingLevel.Warning
                _logEventArgs.LogMessage = InformationHubStrings.WarningGetE3App_LogMsg

                OnLogMessage(_logEventArgs)
            End Try
        Catch ex As Exception
            _logEventArgs.LogLevel = LogEventArgs.LoggingLevel.Error
            _logEventArgs.LogMessage = String.Format(InformationHubStrings.ErrorGetE3App_LogMsg, ex.Message)

            OnLogMessage(_logEventArgs)
        Finally
            If bindCtx IsNot Nothing Then
                Marshal.ReleaseComObject(bindCtx)
            End If
            If runningObjectTable IsNot Nothing Then
                Marshal.ReleaseComObject(runningObjectTable)
            End If
        End Try

        Return foundRunningE3Instances
    End Function

    Private Function GetSelectedObjectsForDistanceCalculation() As Dictionary(Of String, Object)
        Dim selectedObjects As New Dictionary(Of String, Object)

        For Each grid As KeyValuePair(Of String, UltraGrid) In _grids
            Dim grid_kbl_obj_type As KblObjectType
            If [Enum].TryParse(Of KblObjectType)(grid.Key, grid_kbl_obj_type) Then
                Select Case grid_kbl_obj_type
                    Case KblObjectType.Accessory_occurrence, KblObjectType.Fixing_occurrence
                        For Each row As UltraGridRow In grid.Value.Selected.Rows
                            Dim referenceNodes As IEnumerable(Of Node) = _kblMapper.GetNodes.Where(Function(node) node.Referenced_components IsNot Nothing AndAlso Not row.Tag IsNot Nothing AndAlso node.Referenced_components.SplitSpace.Contains(row.Tag?.ToString))
                            Dim referenceSegments As IEnumerable(Of Segment) = _kblMapper.GetSegments.Where(Function(segment) segment.Fixing_assignment IsNot Nothing AndAlso row.Tag IsNot Nothing AndAlso segment.Fixing_assignment.Any(Function(fixingAssignment) fixingAssignment.Fixing = row.Tag?.ToString))

                            If (referenceNodes.Count = 1 AndAlso Not referenceSegments.Any()) Then
                                selectedObjects.Add(row.Tag?.ToString, referenceNodes.FirstOrDefault)
                            ElseIf (Not referenceNodes.Any() AndAlso referenceSegments.Count = 1) Then
                                selectedObjects.Add(row.Tag?.ToString, referenceSegments.FirstOrDefault)
                            End If
                        Next
                    Case KblObjectType.Assembly_part_occurrence, KblObjectType.Connector_occurrence
                        For Each row As UltraGridRow In grid.Value.Selected.Rows
                            Dim referenceNodes As IEnumerable(Of Node) = _kblMapper.GetNodes.Where(Function(node) node.Referenced_components IsNot Nothing AndAlso row.Tag IsNot Nothing AndAlso node.Referenced_components.SplitSpace.Contains(row.Tag?.ToString))
                            If (referenceNodes.Count = 1) Then
                                selectedObjects.Add(row.Tag?.ToString, referenceNodes.FirstOrDefault)
                            End If
                        Next
                    Case KblObjectType.Node
                        For Each row As UltraGridRow In grid.Value.Selected.Rows
                            If row.Tag IsNot Nothing Then
                                selectedObjects.Add(row.Tag?.ToString, _kblMapper.GetOccurrenceObjectUntyped(row.Tag?.ToString))
                            End If
                        Next
                End Select
            End If
        Next

        Return selectedObjects
    End Function

    Private Function HasInactiveConnectorActiveParts(connector As Connector_occurrence, inactiveObjects As ITypeGroupedKblIds) As Boolean
        For Each accessory As Accessory_occurrence In _kblMapper.GetAccessoryOccurrences
            If accessory.Reference_element = connector.SystemId Then
                If Not inactiveObjects.GetValueOrEmpty(KblObjectType.Accessory_occurrence).Contains(accessory.SystemId) Then
                    Return True
                End If
            End If
        Next

        For Each component As Component_occurrence In _kblMapper.GetComponentOccurrences
            If Not String.IsNullOrWhiteSpace(component.Mounting) Then
                For Each mounting As String In component.Mounting?.SplitSpace.OrEmpty
                    If mounting = connector.SystemId Then
                        Dim kblObjType As KblObjectType = component.GetType.GetKblObjectType
                        Select Case kblObjType
                            Case KblObjectType.Component_occurrence, KblObjectType.Fuse_occurrence
                                If Not inactiveObjects.GetValueOrEmpty(kblObjType).Contains(component.SystemId) Then
                                    Return True
                                End If
                        End Select
                    End If
                Next
            End If
        Next

        For Each contactPoint As Contact_point In connector.Contact_points
            If Not String.IsNullOrWhiteSpace(contactPoint.Associated_parts) Then
                For Each associatedPart As String In contactPoint.Associated_parts?.SplitSpace.OrEmpty
                    Dim kblOcc As IKblOccurrence = _kblMapper.GetAnyOccurrenceObjectOfKblObjectTypes(associatedPart, KblObjectType.Cavity_seal_occurrence, KblObjectType.Special_terminal_occurrence)
                    If kblOcc IsNot Nothing Then
                        If inactiveObjects.GetValueOrEmpty(kblOcc.ObjectType).Contains(associatedPart) Then
                            Return True
                        End If
                    End If
                Next
            End If

            If _kblMapper.KBLContactPointWireMapper.ContainsKey(contactPoint.SystemId) Then
                For Each wireId As String In _kblMapper.KBLContactPointWireMapper(contactPoint.SystemId)
                    Dim wireCore As IKblWireCoreOccurrence = _kblMapper.GetWireOrCoreOccurrence(wireId)
                    If wireCore IsNot Nothing Then
                        Dim kblObjType As KblObjectType = wireCore.ObjectType
                        If kblObjType = KblObjectType.Core_occurrence Then
                            'HINT: map core to cable BL and proceed normally
                            kblObjType = KblObjectType.Special_wire_occurrence
                            wireId = _kblMapper.KBLCoreCableMapper(wireId)
                        End If

                        If inactiveObjects.ContainsValue(kblObjType, wireId) Then
                            Return True
                        End If
                    End If
                Next
            End If
        Next

        Return False
    End Function

    Private Sub InitializeGridColumn(ByVal gridAppearance As GridAppearance, ByVal layout As UltraGridLayout, ByVal band As UltraGridBand, ByVal column As UltraGridColumn)
        Dim gridColumn As GridColumn = Nothing

        If (gridAppearance?.GridTable IsNot Nothing) Then
            If band.ParentBand Is Nothing Then 'Toplevel appearance
                gridColumn = gridAppearance.GridTable.GridColumns.FindGridColumn(column.Key).SingleOrDefault
            Else 'Secondlevel appearance
                If (gridAppearance.GridTable.GridSubTable IsNot Nothing) Then
                    gridColumn = gridAppearance.GridTable.GridSubTable.GridColumns.FindGridColumn(column.Key).SingleOrDefault
                End If
            End If
        End If

        If (column?.Key = ModulePropertyName.Controlled_components) Then
            column.Header.Caption = InformationHubStrings.CtrlComps_ColumnCaption
        End If

        If column?.Key = NameOf(InformationHubStrings.DiffType_ColumnCaption) Then
            column.Header.Fixed = True
            column.MaxWidth = 90
        End If

        If (column.Key = SYSTEM_ID_COLUMN_KEY) OrElse (gridColumn IsNot Nothing AndAlso Not gridColumn.Visible) OrElse (_comparisonMapperList IsNot Nothing AndAlso (column.Key = CAVITY_COUNT_COLUMN_KEY OrElse column.Key = "KEM" OrElse column.Key = "ZGS")) Then
            column.Hidden = True
        End If
        If (_comparisonMapperList IsNot Nothing AndAlso TypeOf (gridAppearance) Is ConnectorGridAppearance AndAlso (column.Key = WirePropertyName.Wire_type.ToString OrElse column.Key = WirePropertyName.Cross_section_area.ToString OrElse column.Key = WirePropertyName.Core_Colour.ToString)) Then
            column.Hidden = True
        End If

        If column IsNot Nothing AndAlso Not column.Hidden Then
            With column
                column.CellAppearance.TextHAlign = HAlign.Center
                column.CellAppearance.TextVAlign = VAlign.Middle

                If (gridColumn IsNot Nothing) AndAlso (Not gridColumn.Comparable AndAlso Not gridColumn.HideComparable) Then
                    column.Header.Appearance.Image = My.Resources.NotComparable
                End If

                column.Header.Appearance.TextHAlign = HAlign.Center
                column.Header.Appearance.TextVAlign = VAlign.Middle

                If (gridAppearance IsNot Nothing) Then
                    If (column.Band.ParentBand Is Nothing) AndAlso (gridColumn IsNot Nothing) Then
                        column.Header.Caption = gridColumn.Name
                    ElseIf (gridAppearance?.GridTable?.GridSubTable IsNot Nothing) AndAlso (gridColumn IsNot Nothing) Then
                        column.Header.Caption = gridColumn.Name
                    End If
                End If

                If column?.Key IsNot Nothing Then
                    Select Case column.Key
                        Case CAVITY_COUNT_COLUMN_KEY, HarnessAnalyzer.[Shared].CORE_WIRE_NUMBER_KEY, CablePropertyName.Special_wire_id.ToString, CablePropertyName.Cross_section_area.ToString, CablePropertyName.Length_Information.ToString, CablePropertyName.Bend_radius.ToString, CablePropertyName.Outside_diameter.ToString, ConnectionPropertyName.Cavity_A.ToString, ConnectionPropertyName.Cavity_B.ToString, ConnectionPropertyName.Wire.ToString, ConnectorPropertyName.Cavity_number.ToString, PartPropertyName.Mass_information.ToString, ProtectionAreaPropertyName.End_location.ToString, ProtectionAreaPropertyName.Start_location.ToString, SegmentPropertyName.Cross_Section_Area_information.ToString, SegmentPropertyName.Physical_length.ToString, SegmentPropertyName.Virtual_length.ToString, VertexPropertyName.Cartesian_pointX.ToString, VertexPropertyName.Cartesian_pointY.ToString, VertexPropertyName.Cartesian_pointZ.ToString, WirePropertyName.Wire_number.ToString, WireProtectionPropertyName.Protection_length.ToString
                            column.FilterOperatorDropDownItems = FilterOperatorDropDownItems.Equals Or FilterOperatorDropDownItems.GreaterThan Or FilterOperatorDropDownItems.GreaterThanOrEqualTo Or FilterOperatorDropDownItems.LessThan Or FilterOperatorDropDownItems.LessThanOrEqualTo Or FilterOperatorDropDownItems.NotEquals
                            column.RowFilterComparer = New NumericStringFilterComparer
                            column.SortComparer = New MarkedRowsNumericStringSortComparer(CurrentRowFilterInfo) ' HINT: replace the stringSortComparer with MarkedRowsComparer to install the markSortcomparer over the NumericStringComparer (for BL explanation see comment on MarkedRowsSortComparer class) which is not sorting as the normal column sorters (based on cell values), it's sorting on row's. At this state of implementation it's unknown if we have a sortComparer on row-level: so we use a column sorter as a workaround to archive this behaviour
                        Case CommonPropertyName.Id, ModulePropertyName.Abbreviation
                            column.SortComparer = New MarkedRowsNumericStringSortComparer(CurrentRowFilterInfo) With {.NumericStringComparerEnabled = True}
                        Case Else
                            Dim hasMarkedRowsSortComparerColumn As Boolean = layout.Bands.Any(Function(b) b.Columns.Any(Function(r) TypeOf r.SortComparer Is MarkedRowsNumericStringSortComparer))
                            If Not hasMarkedRowsSortComparerColumn Then
                                If Not TypeOf column.SortComparer Is MarkedRowsNumericStringSortComparer Then
                                    column.SortComparer = New MarkedRowsNumericStringSortComparer(CurrentRowFilterInfo) With {.NumericStringComparerEnabled = False} ' HINT: this sorter is just for sorting the marked rows, no numeric string name sorting here
                                ElseIf TypeOf column.SortComparer Is NumericStringSortComparer Then
                                    column.SortComparer = New MarkedRowsNumericStringSortComparer(CurrentRowFilterInfo)
                                End If
                            End If
                    End Select
                End If

                If (_kblMapperForCompare Is Nothing) Then
                    If (band.ParentBand Is Nothing) AndAlso (column.Index = 0) AndAlso (layout.Grid.Name <> NameOf(ugHarness) AndAlso layout.Grid.Name <> NameOf(ugWires)) Then
                        column.SortIndicator = SortIndicator.Ascending
                    End If

                    If (band.ParentBand IsNot Nothing OrElse layout.Grid.Name = NameOf(ugWires)) AndAlso (column.Index = 1) Then
                        column.SortIndicator = SortIndicator.Ascending
                    End If
                Else
                    If (band.ParentBand Is Nothing) AndAlso (column.Index = 4) AndAlso (layout.Grid.Name <> NameOf(ugHarness)) Then
                        column.SortIndicator = SortIndicator.Ascending
                    End If
                End If

                If (column.Index = 6) AndAlso (layout.Grid.Name = NameOf(ugRedlinings)) Then
                    column.Style = ColumnStyle.CheckBox
                End If
            End With
        End If

        If column?.Key = NameOf(InformationHubStrings.AddLengthInfo_ColumnCaption) OrElse column?.Key = NameOf(InformationHubStrings.AssignedModules_ColumnCaption) Then
            column.Hidden = True
        End If
    End Sub

    Private Sub InitializeGridLayout(gridAppearance As GridAppearance, layout As UltraGridLayout)
        If layout.Grid.Name = NameOf(ugHarness) Then
            layout.AutoFitStyle = AutoFitStyle.ResizeAllColumns
        End If

        layout.CaptionVisible = DefaultableBoolean.False
        layout.GroupByBox.Hidden = True
        layout.LoadStyle = LoadStyle.LoadOnDemand

        With layout.Override
            .AllowColMoving = AllowColMoving.NotAllowed
            .AllowDelete = DefaultableBoolean.False

            If layout.Grid.Name <> NameOf(ugHarness) Then
                .AllowRowFiltering = DefaultableBoolean.True
            End If

            .AllowUpdate = DefaultableBoolean.False
            .ButtonStyle = UIElementButtonStyle.Button3D
            .CellClickAction = CellClickAction.RowSelect
            .FixedHeaderIndicator = FixedHeaderIndicator.None
            .RowSelectors = DefaultableBoolean.False
            .RowSizing = RowSizing.Fixed

            If _comparisonMapperList Is Nothing Then
                .SelectTypeRow = SelectType.Extended
            Else
                .ActiveCellAppearance.Reset()
                .ActiveRowAppearance.Reset()

                .SelectTypeCell = SelectType.None
                .SelectTypeRow = SelectType.Single
            End If
        End With

        layout.Scrollbars = Scrollbars.Automatic
        layout.UseFixedHeaders = True

        For Each band As UltraGridBand In layout.Bands
            For Each column As UltraGridColumn In band.Columns
                InitializeGridColumn(gridAppearance, layout, band, column)
                Dim str As String = InformationHubStrings.ResourceManager.GetString(column.Key)
                If Not String.IsNullOrEmpty(str) AndAlso column.Header.Caption <> str Then
                    column.Header.Caption = str
                End If
            Next
        Next
    End Sub

    Private Sub InitializeRowForCompare(compareObjects As Dictionary(Of String, Object), row As UltraGridRow)
        Dim coCount As Integer = -1
        Dim rowSystemId As String = GetSystemIdFromCompare(row)

        For Each compareObject As KeyValuePair(Of String, Object) In compareObjects
            coCount += 1
            Dim compareObjectSystemId As String = GetSystemIdFromCompare(compareObject)

            If (String.IsNullOrEmpty(compareObjectSystemId) OrElse String.IsNullOrEmpty(rowSystemId) AndAlso (coCount = row.Index)) OrElse (compareObjectSystemId = rowSystemId) Then
                If SetCompareTag(row, compareObject) Then
                    Exit For
                End If
            End If
        Next
    End Sub

    Private Function GetSystemIdFromCompare(row As UltraGridRow) As String
        If TypeOf row.Tag Is KeyValuePair(Of String, Object) Then
            Return GetSystemIdFromCompare(CType(row.Tag, KeyValuePair(Of String, Object)))
        End If
        Return String.Empty
    End Function

    Private Function GetSystemIdFromCompare(compareObject As KeyValuePair(Of String, Object)) As String
        If Not String.IsNullOrEmpty(compareObject.Key) Then
            Return compareObject.Key.Split("|"c).Last
        End If
        Return String.Empty
    End Function

    Private Function SetCompareTag(row As UltraGridRow, compareObject As KeyValuePair(Of String, Object)) As Boolean
        row.Tag = compareObject

        If (IsModifiedDiffType(compareObject.Key)) Then
            Dim changedProperty As ChangedProperty = TryCast(compareObject.Value, ChangedProperty)
            If changedProperty Is Nothing Then
                If compareObject.Key.Contains("$"c) Then
                    Dim changedCavityKey As String = compareObject.Key.Split("$"c, StringSplitOptions.RemoveEmptyEntries)(0)
                    Dim changedCavityValue As Object = DirectCast(compareObject.Value, Tuple(Of Object, Object)).Item1
                    Dim changedContactPointKey As String = compareObject.Key.Split("$"c, StringSplitOptions.RemoveEmptyEntries)(1)
                    Dim changedContactPointValue As Object = DirectCast(compareObject.Value, Tuple(Of Object, Object)).Item2

                    changedProperty = GetChangedContactPointProperty(changedContactPointValue)

                    If (changedProperty IsNot Nothing) AndAlso (TypeOf changedCavityValue Is CavityChangedProperty) Then
                        For Each changedProp As KeyValuePair(Of String, Object) In DirectCast(changedCavityValue, CavityChangedProperty).ChangedProperties
                            changedProperty.ChangedProperties.Add(changedProp.Key, changedProp.Value)
                        Next
                    End If
                ElseIf (compareObject.Key.Contains("#"c)) Then
                    changedProperty = GetChangedContactPointProperty(compareObject.Value)
                Else
                    Return False
                End If
            End If

            If (changedProperty IsNot Nothing) Then
                For Each changedProp As KeyValuePair(Of String, Object) In changedProperty.ChangedProperties
                    Dim propertyKey As String = changedProp.Key

                    If row.Cells.Exists(propertyKey) Then
                        If (TypeOf changedProp.Value Is String) AndAlso (ChangedItem.IsNewOrDeleted(changedProp.Value.ToString)) AndAlso (compareObject.Key.Contains("#"c)) Then
                            ChangeModifiedCellAppearance(row.Cells(propertyKey), Nothing, ChangedItem.IsNew(changedProp.Value.ToString), ChangedItem.IsDeleted(changedProp.Value.ToString))
                        Else
                            ChangeModifiedCellAppearance(row.Cells(propertyKey), changedProp.Value)
                        End If
                    ElseIf (propertyKey = WirePropertyName.Part) Then
                        If TypeOf changedProp.Value Is CoreChangedProperty Then
                            For Each changedPartProp As KeyValuePair(Of String, Object) In DirectCast(changedProp.Value, CoreChangedProperty).ChangedProperties
                                If (row.Cells.Exists(changedPartProp.Key)) Then
                                    ChangeModifiedCellAppearance(row.Cells(changedPartProp.Key), changedPartProp.Value)
                                End If
                            Next
                        ElseIf TypeOf changedProp.Value Is GeneralWireChangedProperty Then
                            For Each changedPartProp As KeyValuePair(Of String, Object) In DirectCast(changedProp.Value, GeneralWireChangedProperty).ChangedProperties
                                If (row.Cells.Exists(changedPartProp.Key)) Then
                                    ChangeModifiedCellAppearance(row.Cells(changedPartProp.Key), changedPartProp.Value)
                                End If
                            Next
                        ElseIf TypeOf changedProp.Value Is PartChangedProperty Then
                            For Each changedPartProp As KeyValuePair(Of String, Object) In DirectCast(changedProp.Value, PartChangedProperty).ChangedProperties
                                If (row.Cells.Exists(changedPartProp.Key)) Then
                                    ChangeModifiedCellAppearance(row.Cells(changedPartProp.Key), changedPartProp.Value)
                                End If
                            Next
                        End If
                    ElseIf (propertyKey = ProtectionAreaPropertyName.Associated_protection) Then
                        For Each wireProtectionOccurrenceChangedProperty As KeyValuePair(Of String, Object) In DirectCast(changedProp.Value, WireProtectionOccurrenceChangedProperty).ChangedProperties
                            For Each changedWireProtOccProp As KeyValuePair(Of String, Object) In DirectCast(changedProp.Value, WireProtectionOccurrenceChangedProperty).ChangedProperties
                                If (row.Cells.Exists(changedWireProtOccProp.Key)) Then
                                    ChangeModifiedCellAppearance(row.Cells(changedWireProtOccProp.Key), changedWireProtOccProp.Value)
                                ElseIf (changedWireProtOccProp.Key = WireProtectionPropertyName.Part) Then
                                    For Each partChangedProperty As KeyValuePair(Of String, Object) In DirectCast(changedWireProtOccProp.Value, PartChangedProperty).ChangedProperties
                                        If (row.Cells.Exists(partChangedProperty.Key)) Then
                                            ChangeModifiedCellAppearance(row.Cells(partChangedProperty.Key), partChangedProperty.Value)
                                        End If
                                    Next
                                End If
                            Next
                        Next
                    End If
                Next
            End If
        ElseIf IsDeletedDiffType(compareObject.Key) Then
            ChangeModifiedRowAppearance(row, True, False)
        ElseIf IsAddedDiffType(compareObject.Key) Then
            ChangeModifiedRowAppearance(row, False, True)
        End If

        Return True
    End Function

    Private Sub OnGestureQueryStatus(sender As Object, e As Touch.GestureQueryStatusEventArgs)
        e.PressAndHoldDelay = 1000
    End Sub

    Private Function RequestCellPartData(ByRef cellData As Object, fromReference As Boolean, kblPropertyName As String, part As Part) As Boolean
        If part Is Nothing Then
            Return False
        End If

        With part
            Select Case kblPropertyName
                Case PartPropertyName.Part_number
                    cellData = .Part_number
                Case PartPropertyName.Company_name
                    cellData = .Company_name
                Case PartPropertyName.Part_alias_ids
                    If part.Alias_id.Length > 0 Then
                        If (part.Alias_id.Length = 1) AndAlso Not String.IsNullOrEmpty(part.Alias_id.First.Description) AndAlso Not String.IsNullOrEmpty(part.Alias_id.First.Scope) Then
                            cellData = .Alias_id(0).Alias_id
                        Else
                            cellData = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                        End If
                    End If
                Case PartPropertyName.Version
                    cellData = .Version
                Case PartPropertyName.Abbreviation
                    cellData = .Abbreviation
                Case PartPropertyName.Part_description
                    cellData = .Description
                Case PartPropertyName.Predecessor_part_number
                    If (part.Predecessor_part_number IsNot Nothing) Then
                        cellData = part.Predecessor_part_number
                    End If
                Case PartPropertyName.Degree_of_maturity
                    If (part.Degree_of_maturity IsNot Nothing) Then
                        cellData = part.Degree_of_maturity
                    End If
                Case PartPropertyName.Copyright_note
                    If (.Copyright_note IsNot Nothing) Then
                        cellData = part.Copyright_note
                    End If
                Case PartPropertyName.Mass_information
                    If (part.Mass_information IsNot Nothing) Then
                        If (fromReference) AndAlso (_kblMapper.KBLUnitMapper.ContainsKey(.Mass_information.Unit_component)) Then
                            cellData = String.Format("{0} {1}", Math.Round(.Mass_information.Value_component, 3), _kblMapper.KBLUnitMapper(.Mass_information.Unit_component).Unit_name)
                        ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLUnitMapper.ContainsKey(.Mass_information.Unit_component)) Then
                            cellData = String.Format("{0} {1}", Math.Round(.Mass_information.Value_component, 3), _kblMapperForCompare.KBLUnitMapper(.Mass_information.Unit_component).Unit_name)
                        Else
                            cellData = Math.Round(.Mass_information.Value_component, 3)
                        End If
                    End If
                Case PartPropertyName.Part_number_type
                    If (part.Part_number_type IsNot Nothing) Then
                        cellData = .Part_number_type
                    End If
                Case PartPropertyName.External_references
                    If (part.External_references IsNot Nothing) AndAlso (.External_references.SplitSpace.Length <> 0) Then
                        cellData = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                    End If
                Case PartPropertyName.Change
                    If (.Change.Length <> 0) Then
                        cellData = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                    End If
                Case PartPropertyName.Material_information
                    If (part.Material_information IsNot Nothing) Then
                        If (.Material_information.Material_reference_system Is Nothing) Then
                            cellData = .Material_information.Material_key
                        Else
                            cellData = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                        End If
                    End If
                Case PartPropertyName.Part_processing_information
                    If (part.Processing_information.Length <> 0) Then
                        cellData = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                    End If
                Case PartPropertyName.Part_Localized_description
                    If (part.Localized_description.Length > 0) Then
                        If (part.Localized_description.Length = 1) Then
                            cellData = part.Localized_description(0).Value
                        Else
                            cellData = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                        End If
                    End If

                Case Else
                    Return False
            End Select
        End With
        Return True
    End Function

    Private Function RequestDialogCompareData(displayDialog As Boolean, objectId As String, tag As KeyValuePair(Of String, Object)) As String
        Dim detailInformationType As String = String.Empty
        Dim tagKey As String = tag.Key

        If (tagKey = NameOf(Change_description.Changed_elements)) Then
            detailInformationType = InformationHubStrings.ChangedElements_Caption
        ElseIf (tagKey = NameOf(Module_configuration.Controlled_components)) Then
            detailInformationType = InformationHubStrings.ControlledComponents_Caption
        ElseIf (tagKey = NameOf(Component_occurrence.Mounting)) Then
            detailInformationType = InformationHubStrings.MountingObjects_Caption
        ElseIf (tagKey = NameOf(Node.Referenced_components)) Then
            detailInformationType = InformationHubStrings.RefComps_Caption
        ElseIf (tagKey = NameOf(Accessory_occurrence.Reference_element)) Then
            detailInformationType = InformationHubStrings.RefElement_Caption
        ElseIf (tagKey = NameOf(Node.Referenced_cavities)) Then
            detailInformationType = InformationHubStrings.RefCavs_Caption
        ElseIf (TypeOf tag.Value Is AliasIdComparisonMapper) Then
            detailInformationType = InformationHubStrings.AliasIds_Caption
        ElseIf (TypeOf tag.Value Is LocalizedDescriptionComparisonMapper) Then
            detailInformationType = InformationHubStrings.LocDescription_Caption

        ElseIf (TypeOf tag.Value Is CommonComparisonMapper) Then
            If (tagKey = NameOf(Part.Change) OrElse (tagKey = NameOf(Change_description.Related_changes))) Then
                detailInformationType = KblObjectType.Change.ToLocalizedPluralString
            ElseIf (tagKey = NameOf(Part.External_references)) Then
                detailInformationType = InformationHubStrings.ExtReferences_Caption
            Else
                detailInformationType = InformationHubStrings.RoutingInfo_Caption
            End If
        ElseIf (TypeOf tag.Value Is CrossSectionAreaComparisonMapper) Then
            detailInformationType = InformationHubStrings.CSAInfo_Caption
        ElseIf (TypeOf tag.Value Is FixingAssignmentComparisonMapper) Then
            detailInformationType = InformationHubStrings.FixingAssignments_Caption
        ElseIf (TypeOf tag.Value Is InstallationInfoComparisonMapper) Then
            detailInformationType = InformationHubStrings.InstallInfo_Caption
        ElseIf (TypeOf tag.Value Is ProcessingInfoComparisonMapper) Then
            detailInformationType = InformationHubStrings.ProcInfo_Caption
        ElseIf (TypeOf tag.Value Is SingleObjectComparisonMapper) Then
            If (tagKey = ConnectorPropertyName.Terminal_part_information) Then
                detailInformationType = InformationHubStrings.TerminalPart_Caption
            ElseIf (tagKey = ConnectorPropertyName.Seal_part_information) Then
                detailInformationType = InformationHubStrings.SealPart_Caption
            ElseIf (tagKey = ConnectorPropertyName.Plug_part_information) Then
                detailInformationType = InformationHubStrings.PlugPart_Caption
            Else
                detailInformationType = KblObjectType.Wiring_group.ToLocalizedString
            End If
        ElseIf TypeOf tag.Value Is WireLengthComparisonMapper Then
            detailInformationType = InformationHubStrings.LengthInfo_Caption
        End If

        If displayDialog Then
            ShowDetailInformationForm(detailInformationType, tag, Nothing, _kblMapper, objectId)
        Else
            Dim rows_str As New System.Text.StringBuilder
            Using detailInformationForm As DetailInformationForm = GetDetailInformationForm(detailInformationType, tag, Nothing, _kblMapper, objectId)
                For Each row As UltraGridRow In detailInformationForm.ugDetailInformation.Rows
                    Dim changed As Boolean = False 'HINT quite cumbersome but here was no change information found and the export to Excel overflowed with useless data

                    If (row.Appearance.FontData.Bold = DefaultableBoolean.True) Then
                        rows_str.AppendLine(InformationHubStrings.Added_CellValue)
                        changed = True
                    ElseIf (row.Appearance.FontData.Strikeout = DefaultableBoolean.True) Then
                        rows_str.AppendLine(InformationHubStrings.Deleted_CellValue)
                        changed = True
                    Else
                        For Each cell As UltraGridCell In row.Cells
                            If cell.Appearance.ForeColor.Is(CHANGED_MODIFIED_FORECOLOR) Then
                                rows_str.AppendLine(InformationHubStrings.Modified_CellValue)
                                changed = True
                                Exit For
                            End If
                        Next
                    End If

                    If changed Then
                        Dim cells_str As New System.Text.StringBuilder
                        For Each cell As UltraGridCell In row.Cells
                            If cell.Value.ToString <> Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS Then
                                If cell.Column.Index = 0 Then
                                    'HINT this output is nonsense in several cases
                                    cells_str.Append(String.Format("{0}: {1}", cell.Column.Key, cell.Value.ToString))
                                Else
                                    cells_str.Append(String.Format("   {0}: {1}", cell.Column.Key, cell.Value.ToString))
                                End If

                                If Not String.IsNullOrEmpty(cell.ToolTipText) Then
                                    cells_str.Append(String.Format(" --> {0}", cell.ToolTipText.Split(New String() {InformationHubStrings.CompDoc1_TooltipPart}, StringSplitOptions.RemoveEmptyEntries).LastOrDefault.OrEmpty.Trim))
                                End If
                            End If
                        Next
                        rows_str.Append(cells_str)
                    End If
                Next
            End Using
            Return rows_str.ToString
        End If

        Return String.Empty
    End Function

    Private Sub RequestDialogPartData(kblMapper As KblMapper, kblPropertyName As String, part As Part)
        With part
            Select Case kblPropertyName
                Case PartPropertyName.Part_alias_ids, "Alias id", NameOf(InformationHubStrings.AliasIds_Caption)
                    ShowDetailInformationForm(InformationHubStrings.AliasIds_Caption, .Alias_id, Nothing, kblMapper)
                Case PartPropertyName.Change, "Change", KblObjectType.Change.ToLocalizedPluralString, NameOf(InformationHubStrings.Change_Caption) 'HINT the different string retrievals are not touched as this mess cannot be handled properly
                    ShowDetailInformationForm([Lib].Schema.Kbl.Utils.GetPluralLocalizedName(KblObjectType.Change), .Change, Nothing, kblMapper)
                Case PartPropertyName.External_references, "External references", NameOf(InformationHubStrings.ExtReferences_Caption)
                    Dim externalReferences As New List(Of External_reference)

                    If Not String.IsNullOrWhiteSpace(.External_references) Then
                        For Each externalReference As String In .External_references?.SplitSpace.OrEmpty
                            Dim ext_ref_occ As External_reference = kblMapper.GetOccurrenceObject(Of External_reference)(externalReference)
                            If ext_ref_occ IsNot Nothing Then
                                externalReferences.Add(ext_ref_occ)
                            End If
                        Next
                    End If

                    ShowDetailInformationForm(InformationHubStrings.ExtReferences_Caption, externalReferences, Nothing, kblMapper)
                Case PartPropertyName.Material_information, "Material information", NameOf(InformationHubStrings.MatInfo_Caption)
                    ShowDetailInformationForm(InformationHubStrings.MatInfo_Caption, .Material_information, Nothing, kblMapper)
                Case PartPropertyName.Part_processing_information, "Processing information", NameOf(InformationHubStrings.ProcInfo_Caption)
                    ShowDetailInformationForm(InformationHubStrings.ProcInfo_Caption, .Processing_information, Nothing, kblMapper)
                Case PartPropertyName.Part_Localized_description, NameOf(InformationHubStrings.LocDescription_Caption)
                    ShowDetailInformationForm(InformationHubStrings.LocDescription_Caption, .Localized_description, Nothing, kblMapper)
            End Select
        End With
    End Sub

    Private Function SelectAllRowsOfActiveGrid() As List(Of String)
        Dim selected As New List(Of String)
        If Me.utcInformationHub.ActiveTab?.Key <> TabNames.tabHarness.ToString Then
            ClearSelectedRowsInGrids()
            BeginGridUpdate()

            Dim rows_kblIds As String() = InformationHubUtils.GetKblIds(Me.ActiveGrid.Rows.Cast(Of UltraGridRow).Where(Function(row) InformationHubUtils.CanSetMarkedRowAppearance(row))).ToArray
            selected.AddRange(SelectKblIdRowsInGrid(Me.ActiveGrid, rows_kblIds))
            EndGridUpdate()

            If SetInformationHubEventArgs(rows_kblIds, Nothing, Nothing) Then
                OnHubSelectionChanged()
            End If
        End If
        Return selected
    End Function

    Private Function SelectCablesInGrid(selection As CanvasSelection) As List(Of String)
        Dim cableKblIds As New List(Of String)(selection.GetCableIds)
        selection.AddCableIds(cableKblIds.ToArray, True)
        Return SelectRowsInGrids(cableKblIds, False, True)
    End Function

    Private Function SelectCavitiesInGrid(ByVal selection As CanvasSelection) As List(Of String)
        selection.RemoveWires()
        Dim cavityPartIds As List(Of String) = selection.GetCavityPartIds
        selection.AddCavityParts(cavityPartIds)
        Return SelectRowsInGrids(cavityPartIds.ToList, False, True)
    End Function

    Private Function SelectComponentsInGrid(selection As CanvasSelection) As List(Of String)
        Dim componentKblIds As New List(Of String)(selection.GetComponentIds)
        selection.AddComponentIds(componentKblIds, True)
        Return SelectRowsInGrids(componentKblIds, False, True)
    End Function

    Private Function SelectModulesInGrid(ByVal selection As CanvasSelection) As List(Of String)
        Dim moduleKblIds As List(Of String) = selection.GetModuleIds.ToList
        selection.AddModuleIds(moduleKblIds, True)
        Return SelectRowsInGrids(moduleKblIds, False, True)
    End Function

    Private Function SelectNetsInGrid(ByVal selection As CanvasSelection) As List(Of String)
        Dim netNames As String() = selection.GetNetNames(removeSegments:=True, removeWires:=True)
        selection.AddNetNames(netNames, True)

        Return SelectRowsInGrids(netNames.ToList, False, True)
    End Function

    Private Function SelectRedliningsInGrid(ByVal selection As CanvasSelection) As List(Of String)
        Dim redliningIds As IEnumerable(Of String) = selection.GetRedliningsIds
        selection.AddRedlingsIds(redliningIds, True)
        Return SelectRowsInGrids(redliningIds.ToList, False, True)
    End Function

    Private Function SelectKblIdRowsInGrid(grid As UltraGrid, ParamArray kbl_ids() As String) As String()
        Return SelectKblIdRowsInGrid(grid, kbl_ids, True, True, True)
    End Function

    Private Function SelectRowInGridCore(row As RowMarkableResult, ensureRowsInView As Boolean) As Boolean
        If row IsNot Nothing Then
            Try
                If Not CurrentRowFilterInfo.IsActiveGridMarkingRow(row.Row) Then
                    row.Row.Selected = True
                    If row.HasChildren Then
                        For Each row_child As UltraGridRow In row.Children
                            row_child.Appearance.BackColor = row_child.Band.Layout.Grid.DisplayLayout.DefaultSelectedBackColor
                            row_child.Appearance.ForeColor = row_child.Band.Layout.Grid.DisplayLayout.DefaultSelectedForeColor
                        Next
                        _selectedConnectorRow = row.Row ' TODO: ???? why ? how ?
                    End If

                    Return True
                End If
            Finally
                If ensureRowsInView Then
                    InformationHubUtils.EnsureRowsVisible({row})
                End If
            End Try
        End If
        Return False
    End Function

    Private Function SelectKblIdRowsInGrid(grid As UltraGrid, ids As IEnumerable(Of String), disableEvents As Boolean) As String()
        Return SelectKblIdRowsInGrid(grid, ids, disableEvents, True)
    End Function

    Private Function SelectKblIdRowsInGrid(grid As UltraGrid, kbl_ids As IEnumerable(Of String), disableEvents As Boolean, ensureRowsVisible As Boolean, Optional forceSelectOnly As Boolean = False) As String()
        If disableEvents Then
            grid.EventManager.AllEventsEnabled = False
        End If

        Dim foundSelectableRows As New List(Of RowMarkableResult)
        Try
            Dim rows As List(Of RowMarkableResult) = _kblIdRowCache.FindRows(grid.Rows.Cast(Of UltraGridRow).Where(Function(r) Not r.Hidden AndAlso Not r.IsFilteredOut), kbl_ids)
            If rows.Count > 0 Then
                For Each row_result As RowMarkableResult In rows
                    If SelectRowInGridCore(row_result, False) Then
                        _selectedChildRows.AddRange(row_result.Children) ' HINT: hashset -> no contains check necessary
                        foundSelectableRows.Add(row_result)
                    End If
                Next

                If ensureRowsVisible Then
                    InformationHubUtils.EnsureRowsVisible(rows)
                End If
            End If
        Finally
            If disableEvents Then
                grid.EventManager.AllEventsEnabled = True
            End If
        End Try

        Return InformationHubUtils.GetKblIds(foundSelectableRows.Select(Function(r_res) r_res.Row)).ToArray
    End Function

    Private Function RowExists(id As String, row As UltraGridRow, change As String) As Boolean
        Dim found As Boolean = False
        Dim tagId As String = ""
        If row.Tag IsNot Nothing AndAlso TypeOf row.Tag Is KeyValuePair(Of String, Object) Then
            tagId = DirectCast(row.Tag, KeyValuePair(Of String, Object)).Key.SplitRemoveEmpty("|"c)(1)
            Dim changetype As String = DirectCast(row.Tag, KeyValuePair(Of String, Object)).Key.SplitRemoveEmpty("|"c)(0)
            If tagId = id AndAlso changetype = change Then
                found = True
            End If
        End If
        Return found
    End Function

    Private Function SelectRowInCompareHubGrid(grid As UltraGrid, id As String, changeType As String, Optional disableEvents As Boolean = True, Optional markRow As Boolean = False) As String
        Dim selectedRow As String = Nothing
        With grid
            For Each row As UltraGridRow In .Rows
                If (Not row.Hidden) Then
                    Dim found As Boolean = RowExists(id, row, changeType)

                    If found Then
                        If disableEvents Then
                            .EventManager.AllEventsEnabled = False
                        End If

                        If markRow Then
                            If row.Appearance.BackColor <> Color.FromArgb(190, 190, 190) Then
                                InformationHubUtils.SetMarkedRowAppearance(row)
                            End If
                        Else
                            row.Selected = True
                            selectedRow = row.Tag?.ToString
                        End If

                        If disableEvents Then
                            .EventManager.AllEventsEnabled = True
                        End If

                        .ActiveRowScrollRegion.ScrollRowIntoView(row)
                        _selectedConnectorRow = row
                        Exit For
                    ElseIf row.HasChild Then
                        Dim rowFound As Boolean = False

                        For Each childRow As UltraGridRow In row.ChildBands(0).Rows
                            If Not childRow.Hidden AndAlso childRow.Tag IsNot Nothing Then

                                found = RowExists(id, childRow, changeType)
                                If found Then
                                    If disableEvents Then
                                        .EventManager.AllEventsEnabled = False
                                    End If

                                    If markRow Then
                                        If (row.Appearance.BackColor <> Color.FromArgb(190, 190, 190)) Then
                                            InformationHubUtils.SetMarkedRowAppearance(row)
                                            InformationHubUtils.SetMarkedRowAppearance(childRow)
                                        End If
                                    Else
                                        childRow.Appearance.BackColor = .DisplayLayout.DefaultSelectedBackColor
                                        childRow.Appearance.ForeColor = .DisplayLayout.DefaultSelectedForeColor
                                        _selectedChildRows.Add(childRow) ' HINT: hashset -> no contains check necessary

                                        row.Selected = True
                                        selectedRow = row.Tag?.ToString
                                    End If

                                    If disableEvents Then
                                        .EventManager.AllEventsEnabled = True
                                    End If

                                    row.ExpandAll()

                                    .ActiveRowScrollRegion.ScrollRowIntoView(childRow)

                                    rowFound = True

                                    Exit For
                                ElseIf (TypeOf childRow.Tag Is List(Of String)) Then
                                    For Each kblIdInList As String In DirectCast(childRow.Tag, List(Of String))
                                        If kblIdInList = id Then
                                            If disableEvents Then
                                                .EventManager.AllEventsEnabled = False
                                            End If

                                            If markRow Then
                                                If InformationHubUtils.CanSetMarkedRowAppearance(row) Then
                                                    InformationHubUtils.SetMarkedRowAppearance(row)
                                                    InformationHubUtils.SetMarkedRowAppearance(childRow)
                                                End If
                                            Else
                                                childRow.Appearance.BackColor = .DisplayLayout.DefaultSelectedBackColor
                                                childRow.Appearance.ForeColor = .DisplayLayout.DefaultSelectedForeColor
                                                _selectedChildRows.Add(childRow) ' HINT: hashset -> no contains check necessary

                                                row.Selected = True
                                                selectedRow = row.Tag?.ToString
                                            End If

                                            If disableEvents Then
                                                .EventManager.AllEventsEnabled = True
                                            End If

                                            row.ExpandAll()

                                            .ActiveRowScrollRegion.ScrollRowIntoView(childRow)

                                            rowFound = True

                                            Exit For
                                        End If
                                    Next

                                    If (rowFound) Then Exit For
                                End If
                            End If
                        Next

                        If (rowFound) Then Exit For
                    End If
                End If
            Next
        End With
        Return selectedRow
    End Function

    Private Function SelectWiresInGrid(selection As CanvasSelection) As List(Of String)
        Dim wireKblIds As New List(Of String)(selection.GetWiresIdsFromSegments(removeSegments:=True))
        selection.AddWireIds(wireKblIds)
        Return SelectRowsInGrids(wireKblIds.ToList, False, True)
    End Function

    Private Function SetInformationHubEventArgs(ids As ICollection(Of String), row As UltraGridRow, rows As SelectedRowsCollection) As Boolean
        Dim tempArgs As InformationHubEventArgs = CType(_informationHubEventArgs.Clone, InformationHubEventArgs)

        _informationHubEventArgs.ObjectIds = New HashSet(Of String)
        _informationHubEventArgs.SelectIn3DView = True
        _informationHubEventArgs.HighlightInConnectivityView = True

        Dim isAdded As Boolean = False
        Dim isDeleted As Boolean = False

        If row IsNot Nothing Then
            ids = New List(Of String)

            Dim tag As Object = row.Tag
            If tag IsNot Nothing Then

                Dim key As String = Nothing
                Dim value As Object = Nothing
                Dim stringColl As IEnumerable(Of String) = Nothing

                If System.Collections.UtilsEx.TryGetKeyValuePair(Of String, Object)(tag, key, value) Then
                    For Each item As String In key?.Trim.Split("$"c, "|"c).OrEmpty
                        Select Case item
                            Case "Added"
                                isAdded = True
                            Case "Deleted"
                                isDeleted = True
                            Case "Modified"
                            Case Else
                                ids.Add(item)
                        End Select
                    Next
                ElseIf System.Collections.UtilsEx.TryGetEnumerable(Of String)(tag, stringColl) Then
                    _informationHubEventArgs.ObjectIds.AddRange(stringColl)
                    _informationHubEventArgs.ObjectType = E3.Lib.Schema.Kbl.KblObjectType.Cavity_occurrence
                ElseIf TypeOf tag Is NetRowInfoContainer Then
                    ids.Add(CType(tag, NetRowInfoContainer).NetName)
                ElseIf tag IsNot Nothing Then
                    ids.Add(tag.ToString.Trim)
                End If
            End If
        ElseIf (rows?.Count).GetValueOrDefault > 0 Then
            ids = New List(Of String)

            For Each r As UltraGridRow In rows
                row = r

                Dim tag As Object = r.Tag
                If tag IsNot Nothing Then
                    Dim key As String = Nothing
                    Dim value As Object = Nothing
                    Dim stringColl As IEnumerable(Of String) = Nothing

                    If System.Collections.UtilsEx.TryGetKeyValuePair(Of String, Object)(tag, key, value) Then
                        For Each item As String In key?.Trim.Split("$"c, "|"c).OrEmpty
                            Select Case item
                                Case "Added"
                                    isAdded = True
                                Case "Deleted"
                                    isDeleted = True
                                Case "Modified"
                                Case Else
                                    ids.Add(item)
                            End Select
                        Next
                    ElseIf System.Collections.UtilsEx.TryGetEnumerable(Of String)(tag, stringColl) Then
                        _informationHubEventArgs.ObjectIds.AddRange(stringColl)
                        _informationHubEventArgs.ObjectType = E3.Lib.Schema.Kbl.KblObjectType.Cavity_occurrence
                    ElseIf r.Band.Key = KblObjectType.Net.ToString Then
                        ids.Add(CType(tag, NetRowInfoContainer).NetName)
                    Else
                        ids.Add(tag.ToString)
                    End If
                End If
            Next
        End If

        For Each id As String In ids.OrEmpty
            Dim kblMapper As KblMapper = Nothing
            If isAdded OrElse isDeleted Then
                If isAdded Then
                    If _generalSettings.InverseCompare Then
                        If (_kblMapper?.KBLOccurrenceMapper?.ContainsKey(id)).GetValueOrDefault Then
                            kblMapper = _kblMapper
                        End If
                    Else
                        If (_kblMapperForCompare?.KBLOccurrenceMapper?.ContainsKey(id)).GetValueOrDefault Then
                            kblMapper = _kblMapperForCompare
                        End If
                    End If
                Else
                    If _generalSettings.InverseCompare Then
                        If (_kblMapperForCompare?.KBLOccurrenceMapper?.ContainsKey(id)).GetValueOrDefault Then
                            kblMapper = _kblMapperForCompare
                        End If
                    Else
                        If (_kblMapper?.KBLOccurrenceMapper?.ContainsKey(id)).GetValueOrDefault Then
                            kblMapper = _kblMapper
                        End If
                    End If
                End If
            Else
                If (_kblMapper?.KBLOccurrenceMapper?.ContainsKey(id)).GetValueOrDefault Then
                    kblMapper = _kblMapper
                End If
            End If

            If kblMapper IsNot Nothing Then
                _informationHubEventArgs.KblMapperSourceId = kblMapper.Id
                _informationHubEventArgs.ObjectType = kblMapper.KBLOccurrenceMapper(id).GetType.GetKblObjectType

                Select Case _informationHubEventArgs.ObjectType
                    Case KblObjectType.Accessory_occurrence
                        _informationHubEventArgs.ObjectIds.Add(id)
                    Case KblObjectType.Approval
                        _informationHubEventArgs.ObjectIds.Add(id)
                    Case KblObjectType.Assembly_part_occurrence
                        _informationHubEventArgs.ObjectIds.Add(id)
                    Case KblObjectType.Component_box_occurrence
                        _informationHubEventArgs.ObjectIds.Add(id)
                    Case KblObjectType.Component_occurrence
                        _informationHubEventArgs.ObjectIds.Add(id)
                        If Not String.IsNullOrWhiteSpace(TryCast(kblMapper.KBLOccurrenceMapper(id), Component_occurrence)?.Mounting) Then
                            For Each mounting As String In DirectCast(kblMapper.KBLOccurrenceMapper(id), Component_occurrence).Mounting?.Trim.SplitSpace
                                Dim mountOcc As IKblOccurrence = CType(kblMapper.KBLOccurrenceMapper(mounting), IKblOccurrence)
                                Select Case mountOcc.GetType.GetKblObjectType
                                    Case KblObjectType.Cavity_occurrence
                                        _informationHubEventArgs.ObjectIds.Add(kblMapper.KBLCavityConnectorMapper(mounting))
                                    Case KblObjectType.Connector_occurrence
                                        _informationHubEventArgs.ObjectIds.Add(mounting)
                                    Case KblObjectType.Slot_occurrence
                                        _informationHubEventArgs.ObjectIds.Add(kblMapper.KBLSlotConnectorMapper(mounting))
                                End Select
                            Next
                        End If

                        _informationHubEventArgs.ObjectType = E3.Lib.Schema.Kbl.KblObjectType.Connector_occurrence
                    Case KblObjectType.Connection
                        If Me.utcInformationHub.ActiveTab?.Key = TabNames.tabNets.ToString Then
                            id = DirectCast(kblMapper.KBLOccurrenceMapper(id), Connection).Wire
                            _informationHubEventArgs.ObjectIds.Add(id)
                            _informationHubEventArgs.ObjectIds.TryAddRange(kblMapper.KBLWireSegmentMapper.GetSegmentIdsOrEmpty(id, id))
                            _informationHubEventArgs.ObjectType = E3.Lib.Schema.Kbl.KblObjectType.Net
                        End If
                    Case KblObjectType.Connector_occurrence
                        _informationHubEventArgs.ObjectIds.Add(id)
                    Case KblObjectType.Co_pack_occurrence
                        _informationHubEventArgs.ObjectIds.Add(id)
                    Case KblObjectType.Core_occurrence
                        _informationHubEventArgs.ObjectIds.Add(id)
                        _informationHubEventArgs.ObjectIds.TryAddRange(kblMapper.KBLWireSegmentMapper.GetSegmentIdsOrEmpty(id, id))
                        _informationHubEventArgs.ObjectType = E3.Lib.Schema.Kbl.KblObjectType.Wire_occurrence
                    Case KblObjectType.Default_dimension_specification
                        _informationHubEventArgs.ObjectIds.Add(id)
                    Case KblObjectType.Dimension_specification
                        _informationHubEventArgs.ObjectIds.Add(id)
                    Case KblObjectType.Fixing_occurrence
                        _informationHubEventArgs.ObjectIds.Add(id)
                    Case KblObjectType.Module
                        _informationHubEventArgs.ObjectIds.Add(id)
                        _informationHubEventArgs.ObjectType = KblObjectType.Harness_module
                    Case KblObjectType.Node
                        _informationHubEventArgs.ObjectIds.Add(id)
                    Case KblObjectType.Segment
                        _informationHubEventArgs.ObjectIds.Add(id)
                    Case KblObjectType.Special_wire_occurrence
                        For Each core As Core_occurrence In kblMapper.GetOccurrenceObject(Of Special_wire_occurrence)(id).Core_occurrence
                            _informationHubEventArgs.ObjectIds.Add(core.SystemId)

                            For Each segment As Segment In kblMapper.KBLWireSegmentMapper.GetSegmentsOrEmpty(core.SystemId)
                                _informationHubEventArgs.ObjectIds.Add(segment.SystemId)
                            Next
                        Next
                    Case KblObjectType.Wire_occurrence
                        _informationHubEventArgs.ObjectIds.Add(id)
                        _informationHubEventArgs.ObjectIds.TryAddRange(kblMapper.KBLWireSegmentMapper.GetSegmentIdsOrEmpty(id))
                    Case KblObjectType.Wire_protection_occurrence
                        _informationHubEventArgs.ObjectIds.Add(id)
                        _informationHubEventArgs.ObjectType = If(kblMapper.GetSegments.Any(Function(segment) segment.Protection_area IsNot Nothing AndAlso segment.Protection_area.Any(Function(protectionArea) protectionArea.Associated_protection = id)), E3.Lib.Schema.Kbl.KblObjectType.Segment, E3.Lib.Schema.Kbl.KblObjectType.Node)
                    Case KblObjectType.Protection_area
                        Dim protId As String = CType(kblMapper.KBLOccurrenceMapper(id), Protection_area).Associated_protection
                        _informationHubEventArgs.ObjectIds.Add(protId)
                        _informationHubEventArgs.ObjectType = If(kblMapper.GetSegments.Any(Function(segment) segment.Protection_area IsNot Nothing AndAlso segment.Protection_area.Any(Function(protectionArea) protectionArea.Associated_protection = id)), E3.Lib.Schema.Kbl.KblObjectType.Segment, E3.Lib.Schema.Kbl.KblObjectType.Node)
                    Case KblObjectType.Change_description
                        _informationHubEventArgs.ObjectIds.Add(id)
                    Case Else
                        If _informationHubEventArgs.ObjectType = KblObjectType.Cavity_occurrence OrElse (TypeOf kblMapper.KBLOccurrenceMapper(id) Is Contact_point) Then
                            _informationHubEventArgs.ObjectIds.Add(id)

                            Dim contactPoints As Contact_point() = Array.Empty(Of Contact_point)
                            If (TypeOf kblMapper.KBLOccurrenceMapper(id) Is Contact_point) Then
                                contactPoints.Add(DirectCast(kblMapper.KBLOccurrenceMapper(id), Contact_point))
                            ElseIf (kblMapper.KBLCavityContactPointMapper.ContainsKey(id)) Then
                                contactPoints = kblMapper.KBLCavityContactPointMapper(id).ToArray
                            End If

                            For Each contactPoint As Contact_point In contactPoints
                                If (contactPoint.Associated_parts IsNot Nothing) AndAlso (contactPoint.Associated_parts <> String.Empty) Then
                                    For Each associatedPart As String In contactPoint.Associated_parts.SplitSpace
                                        _informationHubEventArgs.ObjectIds.Add(associatedPart)
                                    Next
                                End If
                            Next

                            _informationHubEventArgs.ObjectType = E3.Lib.Schema.Kbl.KblObjectType.Cavity_occurrence
                        Else
                            _informationHubEventArgs.ObjectType = KblObjectType.Undefined
                        End If
                End Select
            ElseIf (_kblMapper?.KBLNetMapper?.ContainsKey(id)).GetValueOrDefault OrElse (_kblMapperForCompare?.KBLNetMapper?.ContainsKey(id)).GetValueOrDefault Then
                'HINT unclear if inverse compare needs to be considered here? seems to be wrong as its just an id trial... MR
                kblMapper = If(_kblMapper.KBLNetMapper.ContainsKey(id), _kblMapper, _kblMapperForCompare)
                _informationHubEventArgs.KblMapperSourceId = kblMapper.Id

                For Each connection As Connection In kblMapper.KBLNetMapper(id)
                    If _currentRowFilterInfo.InactiveObjects.ContainsValue(KblObjectType.Core_occurrence, connection.Wire) OrElse (_currentRowFilterInfo.InactiveObjects.ContainsValue(KblObjectType.Wire_occurrence, connection.Wire)) Then
                        'HINT Wire is inactive in selected module configuration
                    Else
                        _informationHubEventArgs.ObjectIds.Add(connection.Wire)
                        _informationHubEventArgs.ObjectIds.TryAddRange(kblMapper.KBLWireSegmentMapper.GetSegmentIdsOrEmpty(connection.Wire))
                    End If
                Next

                _informationHubEventArgs.ObjectType = E3.Lib.Schema.Kbl.KblObjectType.Net
            ElseIf _kblMapper?.GetHarness?.SystemId = id OrElse _kblMapperForCompare?.GetHarness?.SystemId = id Then
                If _kblMapper?.GetHarness?.SystemId = id Then
                    _informationHubEventArgs.KblMapperSourceId = _kblMapper.Id
                Else
                    _informationHubEventArgs.KblMapperSourceId = _kblMapperForCompare?.Id
                End If

                _informationHubEventArgs.ObjectIds.Add(id)
                _informationHubEventArgs.ObjectType = E3.Lib.Schema.Kbl.KblObjectType.Harness
            ElseIf (TryCast(_parentForm, DocumentForm)?.ActiveDrawingCanvas?.StampMapper?.ContainsKey(id)).GetValueOrDefault Then
                _informationHubEventArgs.KblMapperSourceId = _kblMapper.Id
                _informationHubEventArgs.ObjectIds.Add(id)
                _informationHubEventArgs.ObjectType = E3.Lib.Schema.Kbl.KblObjectType.QMStamp
            End If
        Next

        Return Not tempArgs.AreEqual(_informationHubEventArgs)
    End Function

    Private Function SetInformationCompareHubEventArgs(ids As List(Of String), kblMapper As KblMapper, changeType As String) As Boolean
        Dim tempArgs As InformationHubEventArgs = CType(_informationHubEventArgs.Clone, InformationHubEventArgs)

        _informationHubEventArgs.ObjectIds = New HashSet(Of String)
        _informationHubEventArgs.SelectIn3DView = True
        _informationHubEventArgs.HighlightInConnectivityView = True

        Dim isAdded As Boolean = False
        Dim isDeleted As Boolean = False
        If changeType = "Added" Then isAdded = True
        If changeType = "Deleted" Then isDeleted = True

        For Each id As String In ids
            If isAdded Or isDeleted Then
                If (isAdded) Then
                    If _generalSettings.InverseCompare Then
                        If _kblMapper.KBLOccurrenceMapper.ContainsKey(id) Then
                            kblMapper = _kblMapper
                        End If
                    End If
                Else
                    If _generalSettings.InverseCompare Then
                        If _kblMapperForCompare IsNot Nothing AndAlso _kblMapperForCompare.KBLOccurrenceMapper.ContainsKey(id) Then
                            kblMapper = _kblMapperForCompare
                        End If
                    End If
                End If
            Else
                If (_kblMapper.KBLOccurrenceMapper.ContainsKey(id)) Then
                    kblMapper = _kblMapper
                End If
            End If

            If (kblMapper IsNot Nothing) Then
                _informationHubEventArgs.KblMapperSourceId = kblMapper.Id
                _informationHubEventArgs.ObjectType = kblMapper.KBLOccurrenceMapper(id).GetType.GetKblObjectType

                Select Case _informationHubEventArgs.ObjectType
                    Case KblObjectType.Accessory_occurrence
                        _informationHubEventArgs.ObjectIds.Add(id)
                    Case KblObjectType.Approval
                        _informationHubEventArgs.ObjectIds.Add(id)
                    Case KblObjectType.Assembly_part_occurrence
                        _informationHubEventArgs.ObjectIds.Add(id)
                    Case KblObjectType.Component_box_occurrence
                        _informationHubEventArgs.ObjectIds.Add(id)
                    Case KblObjectType.Component_occurrence
                        _informationHubEventArgs.ObjectIds.Add(id)
                        If Not String.IsNullOrWhiteSpace(TryCast(kblMapper.KBLOccurrenceMapper(id), Component_occurrence)?.Mounting) Then
                            For Each mounting As String In DirectCast(kblMapper.KBLOccurrenceMapper(id), Component_occurrence).Mounting?.SplitSpace.OrEmpty
                                Dim mountOcc As IKblOccurrence = CType(kblMapper.KBLOccurrenceMapper(mounting), IKblOccurrence)
                                Select Case mountOcc.GetType.GetKblObjectType
                                    Case KblObjectType.Cavity_occurrence
                                        _informationHubEventArgs.ObjectIds.Add(kblMapper.KBLCavityConnectorMapper(mounting))
                                    Case KblObjectType.Connector_occurrence
                                        _informationHubEventArgs.ObjectIds.Add(mounting)
                                    Case KblObjectType.Slot_occurrence
                                        _informationHubEventArgs.ObjectIds.Add(kblMapper.KBLSlotConnectorMapper(mounting))
                                End Select
                            Next
                        End If

                        _informationHubEventArgs.ObjectType = E3.Lib.Schema.Kbl.KblObjectType.Connector_occurrence
                    Case KblObjectType.Connection
                        If Me.utcInformationHub.ActiveTab?.Key = TabNames.tabNets.ToString Then
                            id = DirectCast(kblMapper.KBLOccurrenceMapper(id), Connection).Wire
                            _informationHubEventArgs.ObjectIds.Add(id)
                            _informationHubEventArgs.ObjectIds.TryAddRange(kblMapper.KBLWireSegmentMapper.GetSegmentIdsOrEmpty(id, id))
                            _informationHubEventArgs.ObjectType = E3.Lib.Schema.Kbl.KblObjectType.Net
                        End If
                    Case KblObjectType.Connector_occurrence
                        _informationHubEventArgs.ObjectIds.Add(id)
                    Case KblObjectType.Co_pack_occurrence
                        _informationHubEventArgs.ObjectIds.Add(id)
                    Case KblObjectType.Core_occurrence
                        _informationHubEventArgs.ObjectIds.Add(id)
                        _informationHubEventArgs.ObjectIds.TryAddRange(kblMapper.KBLWireSegmentMapper.GetSegmentIdsOrEmpty(id, id))
                        _informationHubEventArgs.ObjectType = E3.Lib.Schema.Kbl.KblObjectType.Wire_occurrence
                    Case KblObjectType.Default_dimension_specification
                        _informationHubEventArgs.ObjectIds.Add(id)
                    Case KblObjectType.Dimension_specification
                        _informationHubEventArgs.ObjectIds.Add(id)
                    Case KblObjectType.Fixing_occurrence
                        _informationHubEventArgs.ObjectIds.Add(id)
                    Case KblObjectType.Module
                        _informationHubEventArgs.ObjectIds.Add(id)
                        _informationHubEventArgs.ObjectType = KblObjectType.Harness_module
                    Case KblObjectType.Node
                        _informationHubEventArgs.ObjectIds.Add(id)
                    Case KblObjectType.Segment
                        _informationHubEventArgs.ObjectIds.Add(id)
                    Case KblObjectType.Special_wire_occurrence
                        For Each core As Core_occurrence In kblMapper.GetOccurrenceObject(Of Special_wire_occurrence)(id).Core_occurrence
                            _informationHubEventArgs.ObjectIds.Add(core.SystemId)

                            For Each segment As Segment In kblMapper.KBLWireSegmentMapper.GetSegmentsOrEmpty(core.SystemId)
                                _informationHubEventArgs.ObjectIds.Add(segment.SystemId)
                            Next
                        Next
                    Case KblObjectType.Wire_occurrence
                        _informationHubEventArgs.ObjectIds.Add(id)
                        _informationHubEventArgs.ObjectIds.TryAddRange(kblMapper.KBLWireSegmentMapper.GetSegmentIdsOrEmpty(id))
                    Case KblObjectType.Wire_protection_occurrence
                        _informationHubEventArgs.ObjectIds.Add(id)
                        _informationHubEventArgs.ObjectType = If(kblMapper.GetSegments.Any(Function(segment) segment.Protection_area IsNot Nothing AndAlso segment.Protection_area.Any(Function(protectionArea) protectionArea.Associated_protection = id)), E3.Lib.Schema.Kbl.KblObjectType.Segment, E3.Lib.Schema.Kbl.KblObjectType.Node)
                    Case KblObjectType.Protection_area
                        Dim protId As String = CType(kblMapper.KBLOccurrenceMapper(id), Protection_area).Associated_protection
                        _informationHubEventArgs.ObjectIds.Add(protId)
                        _informationHubEventArgs.ObjectType = If(kblMapper.GetSegments.Any(Function(segment) segment.Protection_area IsNot Nothing AndAlso segment.Protection_area.Any(Function(protectionArea) protectionArea.Associated_protection = id)), E3.Lib.Schema.Kbl.KblObjectType.Segment, E3.Lib.Schema.Kbl.KblObjectType.Node)
                    Case KblObjectType.Change_description
                        _informationHubEventArgs.ObjectIds.Add(id)
                    Case Else
                        If _informationHubEventArgs.ObjectType = KblObjectType.Cavity_occurrence OrElse (TypeOf kblMapper.KBLOccurrenceMapper(id) Is Contact_point) Then
                            _informationHubEventArgs.ObjectIds.Add(id)

                            Dim contactPoints As Contact_point() = Array.Empty(Of Contact_point)
                            If (TypeOf kblMapper.KBLOccurrenceMapper(id) Is Contact_point) Then
                                contactPoints.Add(DirectCast(kblMapper.KBLOccurrenceMapper(id), Contact_point))
                            ElseIf (kblMapper.KBLCavityContactPointMapper.ContainsKey(id)) Then
                                contactPoints = kblMapper.KBLCavityContactPointMapper(id).ToArray
                            End If

                            For Each contactPoint As Contact_point In contactPoints
                                If (contactPoint.Associated_parts IsNot Nothing) AndAlso (contactPoint.Associated_parts <> String.Empty) Then
                                    For Each associatedPart As String In contactPoint.Associated_parts.SplitSpace
                                        _informationHubEventArgs.ObjectIds.Add(associatedPart)
                                    Next
                                End If
                            Next

                            _informationHubEventArgs.ObjectType = E3.Lib.Schema.Kbl.KblObjectType.Cavity_occurrence
                        Else
                            _informationHubEventArgs.ObjectType = KblObjectType.Undefined
                        End If
                End Select
            ElseIf (_kblMapper?.KBLNetMapper?.ContainsKey(id)).GetValueOrDefault OrElse (_kblMapperForCompare?.KBLNetMapper?.ContainsKey(id)).GetValueOrDefault Then
                'HINT unclear if inverse compare needs to be considered here? seems to be wrong as its just an id trial... MR
                kblMapper = If(_kblMapper.KBLNetMapper.ContainsKey(id), _kblMapper, _kblMapperForCompare)
                _informationHubEventArgs.KblMapperSourceId = kblMapper.Id

                For Each connection As Connection In kblMapper.KBLNetMapper(id)
                    If _currentRowFilterInfo.InactiveObjects.ContainsValue(KblObjectType.Core_occurrence, connection.Wire) OrElse (_currentRowFilterInfo.InactiveObjects.ContainsValue(KblObjectType.Wire_occurrence, connection.Wire)) Then
                        'HINT Wire is inactive in selected module configuration
                    Else
                        _informationHubEventArgs.ObjectIds.Add(connection.Wire)
                        _informationHubEventArgs.ObjectIds.TryAddRange(kblMapper.KBLWireSegmentMapper.GetSegmentIdsOrEmpty(connection.Wire))
                    End If
                Next

                _informationHubEventArgs.ObjectType = E3.Lib.Schema.Kbl.KblObjectType.Net
            ElseIf _kblMapper?.GetHarness?.SystemId = id OrElse _kblMapperForCompare?.GetHarness?.SystemId = id Then
                If _kblMapper?.GetHarness?.SystemId = id Then
                    _informationHubEventArgs.KblMapperSourceId = _kblMapper.Id
                Else
                    _informationHubEventArgs.KblMapperSourceId = _kblMapperForCompare?.Id
                End If

                _informationHubEventArgs.ObjectIds.Add(id)
                _informationHubEventArgs.ObjectType = E3.Lib.Schema.Kbl.KblObjectType.Harness
            ElseIf (TryCast(_parentForm, DocumentForm)?.ActiveDrawingCanvas?.StampMapper?.ContainsKey(id)).GetValueOrDefault Then
                _informationHubEventArgs.KblMapperSourceId = _kblMapper.Id
                _informationHubEventArgs.ObjectIds.Add(id)
                _informationHubEventArgs.ObjectType = E3.Lib.Schema.Kbl.KblObjectType.QMStamp
            End If
        Next
        Return Not tempArgs.AreEqual(_informationHubEventArgs)
    End Function

    Private Sub SetSelectedAsMarkedOriginRows(grid As UltraGrid)
        ClearMarkedRowsInGrids(False)
        AddMarkedOriginRows(grid.Selected.Rows.Cast(Of UltraGridRow))
        _rowFilters.Refresh()
    End Sub

    Private Sub ToggleMarkedOriginRows(rows As IEnumerable(Of UltraGridRow))
        BeginUpdateOnEachRow(rows, Sub(row) ToggleMarkedOriginRow(row, False, False))
        _rowFilters.Refresh()
    End Sub

    Private Sub RemoveMarkedOriginRows(rows As IEnumerable(Of UltraGridRow))
        BeginUpdateOnEachRow(rows, Sub(row) RemoveMarkedOriginRow(row, False, False))
        _rowFilters.Refresh()
    End Sub

    Private Sub AddMarkedOriginRows(rows As IEnumerable(Of UltraGridRow))
        BeginUpdateOnEachRow(rows, Sub(row) AddMarkedOriginRow(row, False, False))
        _rowFilters.Refresh()
    End Sub

    Private Sub BeginUpdateOnEachRow(rows As IEnumerable(Of UltraGridRow), onEachRowAction As Action(Of UltraGridRow))
        Me.utcInformationHub.BeginUpdate()
        For Each rows_grid As IGrouping(Of UltraGridBase, UltraGridRow) In rows.GroupBy(Function(r) r.Band.Layout.Grid)
            rows_grid.Key.BeginUpdate()
            For Each row As UltraGridRow In rows_grid
                onEachRowAction.Invoke(row)
            Next
            rows_grid.Key.EndUpdate()
        Next
        Me.utcInformationHub.EndUpdate()
    End Sub

    Private Function RemoveMarkedOriginRow(row As UltraGridRow, Optional beginUpdate As Boolean = True, Optional refreshRowFilters As Boolean = True) As Boolean
        If BeginChangeMarkedOriginRow(row?.Band?.Layout?.Grid, False, beginUpdate) Then
            Dim removed As Boolean = Me.CurrentRowFilterInfo.MarkedRows.Remove(row)
            EndChangeMarkedOriginRow(row.Band.Layout.Grid, refreshRowFilters, beginUpdate) ' HINT: always because the endUpdate must be closed
            Return removed
        End If
        Return False
    End Function

    Private Function AddMarkedOriginRow(row As UltraGridRow, Optional beginUpdate As Boolean = True, Optional clearMarkedRows As Boolean = True, Optional refreshFilters As Nullable(Of Boolean) = Nothing) As Boolean
        If BeginChangeMarkedOriginRow(row?.Band?.Layout?.Grid, clearMarkedRows, beginUpdate) Then
            Dim added As Boolean = Me.CurrentRowFilterInfo.MarkedRows.Add(row)
            EndChangeMarkedOriginRow(row.Band.Layout.Grid, If(refreshFilters.HasValue, refreshFilters.Value, clearMarkedRows), beginUpdate) ' HINT: always because the endUpdate must be closed
            Return added
        End If
        Return False
    End Function

    Private Function BeginChangeMarkedOriginRow(grid As UltraGridBase, Optional clearMarkedRows As Boolean = True, Optional beginUpdate As Boolean = True) As Boolean
        If grid Is Nothing Then
            Return False
        End If

        If clearMarkedRows Then
            ClearMarkedRowsInGrids(refreshRowFilters:=False)
        End If

        If beginUpdate Then
            grid.BeginUpdate()
        End If

        Return True
    End Function

    Private Sub EndChangeMarkedOriginRow(grid As UltraGridBase, Optional refreshRowFilters As Boolean = True, Optional beginEndUpdate As Boolean = True)
        If beginEndUpdate Then
            grid.EndUpdate()
        End If

        If beginEndUpdate Then
            utcInformationHub.BeginUpdate()
        End If

        utcInformationHub.ActiveTab.Appearance.FontData.Bold = If(Me.CurrentRowFilterInfo.MarkedRows.Count > 0, DefaultableBoolean.True, DefaultableBoolean.Default)

        If beginEndUpdate Then
            utcInformationHub.EndUpdate()
        End If

        If refreshRowFilters Then
            _rowFilters.Refresh()
        End If
    End Sub

    Private Sub ToggleMarkedOriginRow(row As UltraGridRow, Optional clearMarkedRows As Boolean = True, Optional beginUpdate As Boolean = True)
        If Me.CurrentRowFilterInfo.MarkedRows.Contains(row) Then
            RemoveMarkedOriginRow(row, beginUpdate, clearMarkedRows)
        Else
            AddMarkedOriginRow(row, beginUpdate, clearMarkedRows)
        End If
    End Sub

    Private Function ShowDetailInformationForm(caption As String, displayData As Object, Optional inactiveObjects As IDictionary(Of String, IEnumerable(Of String)) = Nothing, Optional kblMapper As KblMapper = Nothing, Optional objectId As String = Nothing, Optional wireLengthType As String = Nothing, Optional objectType As String = Nothing) As DialogResult
        Using detailInformationForm As DetailInformationForm = GetDetailInformationForm(caption, displayData, inactiveObjects, kblMapper, objectId, wireLengthType, objectType)
            AddHandler detailInformationForm.SelectionChanged, AddressOf detailInformationForm_SelectionChanged
            Dim res As DialogResult = detailInformationForm.ShowDialog(Me)
            RemoveHandler detailInformationForm.SelectionChanged, AddressOf detailInformationForm_SelectionChanged
            Return res
        End Using
    End Function

    Private Sub detailInformationForm_SelectionChanged(sender As DetailInformationForm, e As DetailInformationEventArgs)
        Dim ids As New List(Of String) From {e.ObjectId}
        SelectRowsInGrids(ids, True, True, True)
    End Sub

    Private Sub ShowModulesOfSelectedObject()
        Dim modules As New Dictionary(Of [Lib].Schema.Kbl.[Module], List(Of String))

        If (TypeOf _clickedGridRow.Tag Is List(Of String)) Then
            Dim terminalPartNumber As String = _clickedGridRow.Cells(ConnectorPropertyName.Terminal_part_number).Value.ToString
            Dim sealPartNumber As String = _clickedGridRow.Cells(ConnectorPropertyName.Seal_part_number).Value.ToString
            For Each objectId As String In DirectCast(_clickedGridRow.Tag, List(Of String))
                If (_kblMapper.KBLOccurrenceMapper.ContainsKey(objectId)) AndAlso (TypeOf _kblMapper.KBLOccurrenceMapper(objectId) Is Terminal_occurrence) Then
                    Dim terminal As General_terminal = DirectCast(_kblMapper.KBLPartMapper(DirectCast(_kblMapper.KBLOccurrenceMapper(objectId), Terminal_occurrence).Part), General_terminal)
                    If (terminal.Part_number <> terminalPartNumber) Then
                        Continue For
                    End If
                ElseIf (_kblMapper.KBLOccurrenceMapper.ContainsKey(objectId)) AndAlso (TypeOf _kblMapper.KBLOccurrenceMapper(objectId) Is Cavity_seal_occurrence) Then
                    Dim seal As Cavity_seal = DirectCast(_kblMapper.KBLPartMapper(DirectCast(_kblMapper.KBLOccurrenceMapper(objectId), Cavity_seal_occurrence).Part), Cavity_seal)
                    If (seal.Part_number <> sealPartNumber) Then
                        Continue For
                    End If
                End If

                If (_kblMapper.KBLObjectModuleMapper.ContainsKey(objectId)) Then
                    For Each kblId As String In _kblMapper.KBLObjectModuleMapper(objectId)
                        If (_kblMapper.KBLOccurrenceMapper.ContainsKey(kblId)) Then
                            If (Not modules.ContainsKey(DirectCast(_kblMapper.KBLOccurrenceMapper(kblId), E3.[Lib].Schema.Kbl.[Module]))) Then
                                modules.Add(DirectCast(_kblMapper.KBLOccurrenceMapper(kblId), E3.[Lib].Schema.Kbl.[Module]), New List(Of String))
                            End If
                            If (Not modules(DirectCast(_kblMapper.KBLOccurrenceMapper(kblId), E3.[Lib].Schema.Kbl.[Module])).Contains(Me.utcInformationHub.ActiveTab.Text)) Then
                                modules(DirectCast(_kblMapper.KBLOccurrenceMapper(kblId), E3.[Lib].Schema.Kbl.[Module])).Add(Me.utcInformationHub.ActiveTab.Text)
                            End If
                        End If
                    Next
                End If
            Next
        Else
            Dim node As Node = If(_kblMapper.KBLOccurrenceMapper.ContainsKey(_clickedGridRow.Tag?.ToString), TryCast(_kblMapper.KBLOccurrenceMapper(_clickedGridRow.Tag?.ToString), Node), Nothing)
            Dim occurrences As New Dictionary(Of String, String)
            Dim segment As Segment = If(_kblMapper.KBLOccurrenceMapper.ContainsKey(_clickedGridRow.Tag?.ToString), TryCast(_kblMapper.KBLOccurrenceMapper(_clickedGridRow.Tag?.ToString), Segment), Nothing)

            If (node IsNot Nothing) AndAlso (node.Referenced_components IsNot Nothing) Then
                For Each refCompId As String In node.Referenced_components.SplitSpace
                    If (Not occurrences.ContainsKey(refCompId)) AndAlso (_kblMapper.KBLOccurrenceMapper.ContainsKey(refCompId)) Then
                        If (TypeOf _kblMapper.KBLOccurrenceMapper(refCompId) Is Accessory_occurrence) Then
                            occurrences.Add(refCompId, E3.Lib.Schema.Kbl.KblObjectType.Accessory_occurrence.ToString)
                        ElseIf (TypeOf _kblMapper.KBLOccurrenceMapper(refCompId) Is Connector_occurrence) Then
                            occurrences.Add(refCompId, E3.Lib.Schema.Kbl.KblObjectType.Connector_occurrence.ToString)
                        ElseIf (TypeOf _kblMapper.KBLOccurrenceMapper(refCompId) Is Fixing_occurrence) Then
                            occurrences.Add(refCompId, E3.Lib.Schema.Kbl.KblObjectType.Fixing_occurrence.ToString)
                        ElseIf (TypeOf _kblMapper.KBLOccurrenceMapper(refCompId) Is Wire_protection_occurrence) Then
                            occurrences.Add(refCompId, E3.Lib.Schema.Kbl.KblObjectType.Wire_protection_occurrence.ToString)
                        End If
                    End If
                Next
            ElseIf segment IsNot Nothing Then
                If segment.Fixing_assignment IsNot Nothing Then
                    For Each fixingAssignment As Fixing_assignment In segment.Fixing_assignment
                        If (_kblMapper.KBLOccurrenceMapper.ContainsKey(fixingAssignment.Fixing)) Then
                            occurrences.TryAdd(fixingAssignment.Fixing, KblObjectType.Fixing_occurrence.ToLocalizedPluralString)
                        End If
                    Next
                End If

                If (segment.Protection_area IsNot Nothing) Then
                    For Each protectionArea As Protection_area In segment.Protection_area
                        If (_kblMapper.KBLOccurrenceMapper.ContainsKey(protectionArea.Associated_protection)) Then
                            occurrences.TryAdd(protectionArea.Associated_protection, KblObjectType.Wire_protection_occurrence.ToLocalizedPluralString)
                        End If
                    Next
                End If

                If (_kblMapper.KBLSegmentWireMapper.ContainsKey(segment.SystemId)) Then
                    For Each wireId As String In _kblMapper.KBLSegmentWireMapper(segment.SystemId)
                        If (_kblMapper.KBLOccurrenceMapper.ContainsKey(wireId)) Then
                            occurrences.TryAdd(wireId, KblObjectType.Wire_occurrence.ToLocalizedPluralString)
                        End If
                    Next
                End If
            Else
                occurrences.Add(_clickedGridRow.Tag?.ToString, Me.utcInformationHub.ActiveTab.Text)
            End If

            For Each occurrence As KeyValuePair(Of String, String) In occurrences
                If (_kblMapper.KBLObjectModuleMapper.ContainsKey(occurrence.Key)) Then
                    For Each kblId As String In _kblMapper.KBLObjectModuleMapper(occurrence.Key)
                        If (_kblMapper.KBLOccurrenceMapper.ContainsKey(kblId)) Then
                            If (Not modules.ContainsKey(DirectCast(_kblMapper.KBLOccurrenceMapper(kblId), E3.[Lib].Schema.Kbl.[Module]))) Then
                                modules.Add(DirectCast(_kblMapper.KBLOccurrenceMapper(kblId), E3.[Lib].Schema.Kbl.[Module]), New List(Of String))
                            End If
                            If (Not modules(DirectCast(_kblMapper.KBLOccurrenceMapper(kblId), E3.[Lib].Schema.Kbl.[Module])).Contains(occurrence.Value)) Then
                                modules(DirectCast(_kblMapper.KBLOccurrenceMapper(kblId), E3.[Lib].Schema.Kbl.[Module])).Add(occurrence.Value)
                            End If
                        End If
                    Next
                End If
            Next
        End If

        If (modules.Count <> 0) Then
            If (_clickedGridRow.Band.Key = KblObjectType.Cavity_occurrence.ToString) Then
                Using modulesOnCavityForm As New ModulesOnCavityForm(_kblMapper, DirectCast(_clickedGridRow.Tag, List(Of String)))
                    If (modulesOnCavityForm.ShowDialog(Me) = DialogResult.Abort) Then
                        Dim kblIds As New List(Of String)

                        For Each row As UltraGridRow In modulesOnCavityForm.ugModulesOnPlug.Rows
                            kblIds.Add(row.Cells(DetailInformationFormStrings.ModId_PropName).Value.ToString)
                        Next

                        For Each row As UltraGridRow In modulesOnCavityForm.ugModulesOnSeal.Rows
                            kblIds.Add(row.Cells(DetailInformationFormStrings.ModId_PropName).Value.ToString)
                        Next

                        For Each row As UltraGridRow In modulesOnCavityForm.ugModulesOnTerminal.Rows
                            kblIds.Add(row.Cells(DetailInformationFormStrings.ModId_PropName).Value.ToString)
                        Next

                        SetInformationHubEventArgs(kblIds, Nothing, Nothing)

                        Dim harnessConfig As Harness_configuration = CType(DirectCast(_parentForm, DocumentForm)._harnessModuleConfigurations.Where(Function(harnessModuleConfig) harnessModuleConfig.ConfigurationType = HarnessModuleConfigurationType.Custom).FirstOrDefault.HarnessConfigurationObject, Harness_configuration)
                        harnessConfig.Modules = String.Empty

                        For Each moduleId As String In kblIds
                            If Not _kblMapper.InactiveModules.ContainsKey(moduleId) Then
                                If String.IsNullOrEmpty(harnessConfig.Modules) Then
                                    harnessConfig.Modules = moduleId
                                Else
                                    harnessConfig.Modules = String.Format("{0} {1}", harnessConfig.Modules, moduleId)
                                End If
                            End If
                        Next

                        RaiseEvent ApplyModuleConfiguration(Me, _informationHubEventArgs)
                    End If
                End Using
            Else
                Using detailInformationForm As DetailInformationForm = GetDetailInformationForm(InformationHubStrings.LinkedModules_Caption, modules, Nothing, _kblMapper, objectType:=Me.utcInformationHub.ActiveTab.Text)
                    If detailInformationForm.ShowDialog(Me) = DialogResult.Abort Then
                        Dim kblIds As New List(Of String)

                        For Each row As UltraGridRow In detailInformationForm.ugDetailInformation.Rows
                            If TypeOf row Is UltraGridGroupByRow Then
                                For Each childRow As UltraGridRow In DirectCast(row, UltraGridGroupByRow).Rows
                                    kblIds.TryAdd(childRow.Cells(DetailInformationFormStrings.ModId_PropName).Value.ToString)
                                Next
                            Else
                                kblIds.TryAdd(row.Cells(DetailInformationFormStrings.ModId_PropName).Value.ToString)
                            End If
                        Next

                        SetInformationHubEventArgs(kblIds, Nothing, Nothing)

                        Dim harnessConfig As Harness_configuration = CType(DirectCast(_parentForm, DocumentForm)._harnessModuleConfigurations.Where(Function(harnessModuleConfig) harnessModuleConfig.ConfigurationType = HarnessModuleConfigurationType.Custom).FirstOrDefault.HarnessConfigurationObject, Harness_configuration)
                        harnessConfig.Modules = String.Empty

                        For Each moduleId As String In kblIds
                            If (Not _kblMapper.InactiveModules.ContainsKey(moduleId)) Then
                                If (harnessConfig.Modules = String.Empty) Then
                                    harnessConfig.Modules = moduleId
                                Else
                                    harnessConfig.Modules = String.Format("{0} {1}", harnessConfig.Modules, moduleId)
                                End If
                            End If
                        Next

                        RaiseEvent ApplyModuleConfiguration(Me, _informationHubEventArgs)
                    End If
                End Using
            End If
        Else
            MessageBoxEx.ShowInfo(InformationHubStrings.NoLinkedModules_Msg)
        End If
    End Sub

    Private Sub ShowRedliningDialog(objectIds As ICollection(Of String), objectNames As List(Of String), objectType As KblObjectType, redliningData As Redlining, selectedGridRows As List(Of UltraGridRow), Optional redliningList As RedliningList = Nothing)
        Dim dialogResult As DialogResult = DialogResult.None
        Dim is3D As Boolean = False

        If (TypeOf _parentForm Is DocumentForm) Then
            is3D = CType(_parentForm, DocumentForm).IsDocument3DActive
        End If

        Using redliningForm As New RedliningForm(_kblMapper, _generalSettings.LastChangedByEditable, objectIds, objectNames, objectType, _redliningInformation, is3D, redliningList)
            redliningForm.IsRedliningForCavitiyNavigator = _IsRedliningForCavityNavigator
            dialogResult = redliningForm.ShowDialog(_parentForm)

            If (dialogResult = System.Windows.Forms.DialogResult.Retry) Then
                redliningList = redliningForm.RedliningList
            End If
        End Using

        If dialogResult = System.Windows.Forms.DialogResult.OK Then
            If Me.utcInformationHub.ActiveTab IsNot Nothing Then
                ClearSelectedRowsInGrid(ActiveGrid)
            End If

            If Me.utcInformationHub.Tabs(TabNames.tabRedlinings.ToString).Appearance.FontData.Bold = DefaultableBoolean.True Then
                ClearMarkedRowsInGrids()
            End If

            InitializeRedlinings()

            Me.ugRedlinings.UpdateData()

            If (redliningData IsNot Nothing) Then
                ResetRedliningIconStateInGridCells(redliningData.ObjectId, redliningData.ObjectType)

                _informationHubEventArgs.ObjectIds = New HashSet(Of String) From {
                    redliningData.ObjectId
                }
                _informationHubEventArgs.ObjectType = redliningData.ObjectType
            Else
                _informationHubEventArgs.ObjectIds = If(TypeOf objectIds Is HashSet(Of String), CType(objectIds, HashSet(Of String)), objectIds.ToHashSet)

                If (Me.utcInformationHub.ActiveTab.Text <> KblObjectType.Redlining.ToString) Then
                    For Each selectedGridRow As UltraGridRow In selectedGridRows
                        If (selectedGridRow.ParentRow Is Nothing) Then
                            If _redliningInformation IsNot Nothing AndAlso _redliningInformation.Redlinings.Where(Function(redlining) redlining.ObjectId = selectedGridRow.Tag?.ToString).Any() Then
                                If (selectedGridRow.Band.Key = KblObjectType.Wire_occurrence.ToString) Then
                                    selectedGridRow.Cells(1).Appearance.Image = My.Resources.Redlining
                                Else
                                    selectedGridRow.Cells(0).Appearance.Image = My.Resources.Redlining
                                End If
                            Else
                                If (selectedGridRow.Band.Key = KblObjectType.Wire_occurrence.ToString) Then
                                    selectedGridRow.Cells(1).Appearance.Image = Nothing
                                Else
                                    selectedGridRow.Cells(0).Appearance.Image = Nothing
                                End If
                            End If
                        Else
                            If _redliningInformation IsNot Nothing AndAlso (_redliningInformation.Redlinings.Where(Function(redlining) redlining.ObjectId = selectedGridRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString).Any() OrElse (selectedGridRow.Band.Key = KblObjectType.Connection.ToString AndAlso _redliningInformation.Redlinings.Where(Function(redlining) redlining.ObjectId = selectedGridRow.Tag?.ToString).Any())) Then
                                selectedGridRow.Cells(1).Appearance.Image = My.Resources.Redlining
                            Else
                                selectedGridRow.Cells(1).Appearance.Image = Nothing
                            End If
                        End If
                    Next
                End If

                _informationHubEventArgs.ObjectType = objectType
            End If

            RaiseEvent RedliningsChanged(Me, _informationHubEventArgs)
        ElseIf (dialogResult = System.Windows.Forms.DialogResult.Retry) Then
            Dim graphicalRedlining As Redlining = Nothing

            For Each redlining As Redlining In redliningList.Where(Function(rdlining) rdlining.Classification = RedliningClassification.GraphicalComment)
                graphicalRedlining = redlining
            Next

            If (graphicalRedlining IsNot Nothing) Then
                If (graphicalRedlining.Comment = String.Empty) Then
                    With DirectCast(_parentForm, DocumentForm)
                        If (.ActiveDrawingCanvas IsNot Nothing) Then
                            With .ActiveDrawingCanvas.vdCanvas.BaseControl
                                Dim boundingBox As VectorDraw.Geometry.Box = Nothing
                                Dim crossing As Boolean

                                If (.ActiveDocument.ActionUtility.getUserRectViewCS(Nothing, False, boundingBox, crossing) = VectorDraw.Actions.StatusCode.Success) Then
                                    Dim scaleFactor As Double = If(boundingBox.Width > boundingBox.Height, If(boundingBox.Width > 1000, 1, (1000 - boundingBox.Width) / 100), If(boundingBox.Height > 1000, 1, (1000 - boundingBox.Height) / 100))
                                    Dim bmp As New Bitmap(CInt(boundingBox.Width * scaleFactor), CInt(boundingBox.Height * scaleFactor))
                                    If (bmp IsNot Nothing) Then
                                        Dim graphics As Graphics = Graphics.FromImage(bmp)

                                        .ActiveDocument.ActiveLayOut.Label = "RenderGraphics"
                                        .ActiveDocument.ActiveLayOut.RenderToGraphics(graphics, boundingBox, bmp.Width, bmp.Height)
                                        .ActiveDocument.ActiveLayOut.Label = String.Empty

                                        graphics.Dispose()

                                        Using graphicalRedliningForm As New GraphicalRedliningForm(graphicalRedlining.Comment, graphicalRedlining.ObjectName, bmp, boundingBox)
                                            graphicalRedliningForm.ShowDialog(Me)
                                            graphicalRedlining.Comment = graphicalRedliningForm.DocumentStreamString
                                        End Using
                                    End If
                                End If
                            End With
                        End If
                    End With
                Else
                    Using graphicalRedliningForm As New GraphicalRedliningForm(graphicalRedlining.Comment, graphicalRedlining.ObjectName)
                        graphicalRedliningForm.ShowDialog(Me)
                        graphicalRedlining.Comment = graphicalRedliningForm.DocumentStreamString
                    End Using
                End If
            Else
                MessageBoxEx.ShowError(InformationHubStrings.ErrorLoadGraphRedlining_Msg)
            End If

            ShowRedliningDialog(objectIds, objectNames, objectType, redliningData, selectedGridRows, redliningList)
        End If
    End Sub

    Private Sub ShowWireWeightCalculationForm()
        Using clcForm As New CalculateWeightForm(ActiveGrid.Selected.Rows, _kblMapper)
            AddHandler clcForm.CalculateProgress, Sub(sender2 As Object, prg As System.ComponentModel.ProgressChangedEventArgs)
                                                      _messageEventArgs.ProgressPercentage = prg.ProgressPercentage
                                                      _messageEventArgs.StatusMessage = InformationHubStrings.WeightCalc_Calculating
                                                      RaiseEvent Message(Me, _messageEventArgs)
                                                  End Sub
            AddHandler clcForm.CalculationFinished, Sub()
                                                        _messageEventArgs.StatusMessage = String.Empty
                                                        _messageEventArgs.ProgressPercentage = 0
                                                        RaiseEvent Message(Me, _messageEventArgs)
                                                    End Sub
            clcForm.ShowDialog(Me)
        End Using
    End Sub

    Private Sub ToggleColumnVisibility(columnKey As String, grid As UltraGrid, hidden As Boolean)
        grid.BeginUpdate()

        If (grid.DisplayLayout.Bands(0).Columns.Exists(columnKey)) AndAlso (columnKey <> NameOf(InformationHubStrings.AddLengthInfo_ColumnCaption) OrElse (grid.DisplayLayout.Bands(0).Columns.Exists(CablePropertyName.Length_Information.ToString) AndAlso Not grid.DisplayLayout.Bands(0).Columns(CablePropertyName.Length_Information).Hidden)) Then
            grid.DisplayLayout.Bands(0).Columns(columnKey).Hidden = hidden
        End If

        If (grid.DisplayLayout.Bands.Count > 1) AndAlso (grid.DisplayLayout.Bands(1).Columns.Exists(columnKey)) AndAlso (columnKey <> NameOf(InformationHubStrings.AddLengthInfo_ColumnCaption) OrElse (grid.DisplayLayout.Bands(1).Columns.Exists(CablePropertyName.Length_Information) AndAlso Not grid.DisplayLayout.Bands(1).Columns(CablePropertyName.Length_Information).Hidden)) Then
            grid.DisplayLayout.Bands(1).Columns(columnKey).Hidden = hidden
        End If

        grid.EndUpdate()
    End Sub

    Private Function GetInformationHubChangeText(changeType As CompareChangeType) As String
        Select Case changeType
            Case CompareChangeType.Deleted
                Return InformationHubStrings.Deleted
            Case CompareChangeType.Modified
                Return InformationHubStrings.Modified
            Case CompareChangeType.New
                Return InformationHubStrings.Added
            Case Else
                Throw New NotImplementedException(String.Format("{0}", changeType.ToString))
        End Select
    End Function

    Private Sub SetDiffCellValueFromObjectKey(row As UltraDataRow, compareObjKey As String)
        SetDiffCellValue(row, GetInformationHubChangeText(ChangedItem.ParseFromText(ExtractChangeText(compareObjKey))))
    End Sub

    Private Sub SetDiffCellValue(row As UltraDataRow, value As String)
        row.SetCellValue(NameOf(InformationHubStrings.DiffType_ColumnCaption), value)
    End Sub

    Private Function ExtractChangeText(compareObjectKey As String) As String
        Return compareObjectKey.SplitRemoveEmpty("|"c)(0)
    End Function

    Private Function ExtractSystemId(compareObjectKey As String) As String
        Return compareObjectKey.SplitRemoveEmpty("|"c)(1)
    End Function

    Private Function IsDeletedDiffType(compareObjectKey As String) As Boolean
        Return ChangedItem.IsDeleted(ExtractChangeText(compareObjectKey))
    End Function

    Private Function IsModifiedDiffType(compareObjectKey As String) As Boolean
        Return ChangedItem.IsModified(ExtractChangeText(compareObjectKey))
    End Function

    Private Function IsAddedDiffType(compareObjectKey As String) As Boolean
        Return ChangedItem.IsNew(ExtractChangeText(compareObjectKey))
    End Function

    Private Function HasChangeTypeWithInverse(row As UltraDataRow, type As CompareChangeType) As Boolean
        If row.Band.Columns.Exists(NameOf(InformationHubStrings.DiffType_ColumnCaption)) Then
            Dim diffTypeCellValue As Object = row.GetCellValue(NameOf(InformationHubStrings.DiffType_ColumnCaption))
            Select Case type
                Case CompareChangeType.Modified
                    Return diffTypeCellValue.ToString = InformationHubStrings.Modified
                Case CompareChangeType.Deleted
                    If _generalSettings.InverseCompare Then
                        Return diffTypeCellValue.ToString = InformationHubStrings.Added
                    End If
                    Return diffTypeCellValue.ToString = InformationHubStrings.Deleted
                Case CompareChangeType.New
                    If _generalSettings.InverseCompare Then
                        Return diffTypeCellValue.ToString = InformationHubStrings.Deleted
                    End If
                    Return diffTypeCellValue.ToString = InformationHubStrings.Added
                Case Else
                    Throw New NotImplementedException(String.Format("ChangeType -> {0}", GetType(InformationHubStrings).Name))
            End Select
        End If
        Return False
    End Function

    Private Sub BundleCrossSectionForm_BundleGridSelectionChanged(sender As BundleCrossSectionForm, e As InformationHubEventArgs) Handles _bundleCrossSectionForm.BundleGridSelectionChanged
        SelectRowsInGrids(e.ObjectIds, True, True)
    End Sub

    Private Sub ConnectivityView_ConnectivityViewMouseClick(sender As Object, e As InformationHubEventArgs)
        SelectRowsInGrids(e.ObjectIds, True, True)
    End Sub

    Private Sub Grid_AfterRowActivate(sender As Object, e As EventArgs)
        OnAfterRowActivate(CType(sender, UltraGrid))
    End Sub

    Private Sub Grid_ClickCell(sender As Object, e As ClickCellEventArgs)
        OnClickCell(e)
    End Sub

    Private Sub Grid_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs)
        For Each band As UltraGridBand In e.Layout.Bands
            If band.Columns.Exists(CompareCommentColumnKey) Then
                band.Columns(CompareCommentColumnKey).Hidden = True
            End If
        Next
    End Sub

    Private Sub grid_KeyUp(sender As Object, e As KeyEventArgs)
        _pressedKey = Keys.None
    End Sub

    Private Sub grid_PreviewKeyDown(sender As Object, e As PreviewKeyDownEventArgs)
        OnPreviewGridKeyDown(sender, e)
    End Sub

    Private Sub TimerE3Application_Tick(sender As Object, e As EventArgs) Handles timerE3Application.Tick
        Dim e3Application As e3Application = Nothing
        Dim e3Job As e3Job = Nothing
        Dim e3Wire As e3Pin = Nothing

        Try
            If GetRunningE3Objects.Count <> 0 Then
                e3Application = New e3Application
                e3Job = CType(e3Application.CreateJobObject, e3Job)
                e3Wire = CType(e3Job.CreatePinObject, e3Pin)

                Dim e3Wires As New Dictionary(Of Integer, e3Pin)

                Dim wireIds_obj As Object = Nothing
                Dim wireCount As Integer = e3Job.GetTreeSelectedPinIds(wireIds_obj)
                Dim wireIds As IEnumerable = CType(wireIds_obj, IEnumerable)

                For wireIdIndex As Integer = 1 To wireCount
                    e3Wire.SetId(CInt(wireIds(wireIdIndex)))
                    e3Wires.Add(e3Wire.GetId, e3Wire)
                Next

                If (_oldE3Wires Is Nothing) OrElse (Not _oldE3Wires.Keys.Where(Function(key) e3Wires.ContainsKey(key) AndAlso e3Wires(key).GetId = _oldE3Wires(key).GetId).Any()) Then
                    _oldE3Wires = e3Wires

                    For Each e3Wire In e3Wires.Values
                        Dim coreWireIds As New List(Of String)

                        For Each coreOccurrence As Core_occurrence In _kblMapper.KBLCoreList.Where(Function(core) core.Wire_number = e3Wire.GetName)
                            coreWireIds.Add(coreOccurrence.SystemId)
                        Next

                        For Each wireOccurrence As Wire_occurrence In _kblMapper.KBLWireList.Where(Function(wire) wire.Wire_number = e3Wire.GetName)
                            coreWireIds.Add(wireOccurrence.SystemId)
                        Next

                        SelectRowsInGrids(coreWireIds, True, True)
                    Next
                End If
            End If
        Catch ex As Exception
            ' Error while highlighting selected wire from E3 application
        Finally
            e3Wire = Nothing
            e3Job = Nothing
            e3Application = Nothing
        End Try
    End Sub

    Private Sub UgeeInformationHub_CellExported(sender As Object, e As CellExportedEventArgs) Handles ugeeInformationHub.CellExported
        If (e.GridRow.Cells(e.GridColumn.Key).ToolTipText <> String.Empty) Then
            e.CurrentWorksheet.Rows(e.CurrentRowIndex).Cells(e.CurrentColumnIndex).Comment = New Infragistics.Documents.Excel.WorksheetCellComment With {
                .Text = New Infragistics.Documents.Excel.FormattedString(e.GridRow.Cells(e.GridColumn.Key).ToolTipText)
            }
            e.CurrentWorksheet.Rows(e.CurrentRowIndex).Cells(e.CurrentColumnIndex).Comment.SetBoundsInTwips(e.CurrentWorksheet, New Rectangle(e.CurrentWorksheet.Rows(e.CurrentRowIndex).Cells(e.CurrentColumnIndex).Comment.GetBoundsInTwips.Location, New Size(8000, 2000)))
        End If

        If e.GridRow.Cells(e.GridColumn.Key).Appearance.ForeColor.Is(CHANGED_MODIFIED_FORECOLOR) Then
            e.CurrentWorksheet.Rows(e.CurrentRowIndex).Cells(e.CurrentColumnIndex).CellFormat.Font.ColorInfo = New Infragistics.Documents.Excel.WorkbookColorInfo(CHANGED_MODIFIED_FORECOLOR.ToColor)
        End If

        If e.GridRow.Cells.Exists(NameOf(InformationHubStrings.DiffType_ColumnCaption)) AndAlso e.GridColumn.Key <> NameOf(InformationHubStrings.DiffType_ColumnCaption) Then
            If e.GridRow.Cells(NameOf(InformationHubStrings.DiffType_ColumnCaption)).Value.ToString = InformationHubStrings.Added Then
                e.CurrentWorksheet.Rows(e.CurrentRowIndex).Cells(e.CurrentColumnIndex).CellFormat.Font.Bold = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True
            ElseIf (e.GridRow.Cells(NameOf(InformationHubStrings.DiffType_ColumnCaption)).Value.ToString = InformationHubStrings.Deleted) Then
                e.CurrentWorksheet.Rows(e.CurrentRowIndex).Cells(e.CurrentColumnIndex).CellFormat.Font.Strikeout = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True
            End If
        End If

        If (e.GridRow.Tag IsNot Nothing) AndAlso (TypeOf e.GridRow.Tag Is String) AndAlso (_redliningInformation IsNot Nothing) AndAlso (_redliningInformation.Redlinings.Any(Function(redlining) redlining.ID = e.GridRow.Tag?.ToString)) AndAlso e.GridColumn.Key = NameOf(InformationHubStrings.Comment_ColumnCaption) Then
            Dim redlining As Redlining = _redliningInformation.Redlinings.Where(Function(rl) rl.ID = e.GridRow.Tag?.ToString).FirstOrDefault
            If redlining.Classification = RedliningClassification.GraphicalComment Then
                Using documentStream As New IO.MemoryStream(Convert.FromBase64String(redlining.Comment))
                    documentStream.Position = 0
                    Using vDraw As New VectorDraw.Professional.Control.VectorDrawBaseControl
                        vDraw.EnsureDocument()

                        If (vDraw.ActiveDocument.LoadFromMemory(documentStream, True)) Then
                            vDraw.ActiveDocument.GridMode = False

                            VdOpenSave.SaveAs(vDraw.ActiveDocument, IO.Path.Combine(DirectCast(_parentForm, DocumentForm).TemporaryJunkFolder.FullName, String.Format("{0}.png", redlining.ID)))
                        End If
                    End Using
                    documentStream.Close()
                End Using

                e.CurrentWorksheet.Columns(e.CurrentColumnIndex).Width = 20000
                e.CurrentWorksheet.Rows(e.CurrentRowIndex).Height = 5000

                Dim imageStream As New MemoryStream
                Using image As Drawing.Image = Image.FromFile(IO.Path.Combine(DirectCast(_parentForm, DocumentForm).TemporaryJunkFolder.FullName, String.Format("{0}.png", redlining.ID)))
                    image.Save(imageStream, Imaging.ImageFormat.Png)
                End Using

                IO.File.Delete(IO.Path.Combine(DirectCast(_parentForm, DocumentForm).TemporaryJunkFolder.FullName, String.Format("{0}.png", redlining.ID)))

                Dim wsImage As New Infragistics.Documents.Excel.WorksheetImage(Image.FromStream(imageStream))
                With wsImage
                    .TopLeftCornerCell = e.CurrentWorksheet.Rows(e.CurrentRowIndex).Cells(e.CurrentColumnIndex)
                    .TopLeftCornerPosition = New PointF(0.0F, 0.0F)
                    .BottomRightCornerCell = e.CurrentWorksheet.Rows(e.CurrentRowIndex).Cells(e.CurrentColumnIndex)
                    .BottomRightCornerPosition = New PointF(100.0F, 100.0F)
                End With

                e.CurrentWorksheet.Shapes.Add(wsImage)
            End If
        End If

        If e.Value.ToString = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS Then
            Dim cellValue As String = String.Empty
            Dim columnKey As String = e.GridColumn.Key
            Dim componentPinMaps As Component_pin_map() = Array.Empty(Of Component_pin_map)
            Dim installationInformationList As Installation_instruction() = Array.Empty(Of Installation_instruction)
            Dim node As Node = TryCast(e.GridRow.Cells(columnKey).Tag, Node)
            Dim part As Part = TryCast(e.GridRow.Cells(columnKey).Tag, Part)
            Dim segment As Segment = TryCast(e.GridRow.Cells(columnKey).Tag, Segment)
            Dim tag As Object = e.GridRow.Cells(columnKey).Tag
            Dim wireLengthList As Wire_length() = Array.Empty(Of Wire_length)
            Dim fixingAssignment As Fixing_assignment = Nothing

            If (node Is Nothing AndAlso part Is Nothing AndAlso segment Is Nothing) AndAlso columnKey = NameOf(InformationHubStrings.PropVal_ColumnCaption) Then
                part = _kblMapper.GetHarness
                columnKey = e.GridRow.Cells(0).Value.ToString
            ElseIf (TypeOf tag Is Accessory_occurrence) Then
                If (_kblMapper.KBLPartMapper.ContainsKey(DirectCast(tag, Accessory_occurrence).Part)) Then
                    part = DirectCast(_kblMapper.KBLPartMapper(DirectCast(tag, Accessory_occurrence).Part), Part)
                ElseIf (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLPartMapper.ContainsKey(DirectCast(tag, Accessory_occurrence).Part)) Then
                    part = DirectCast(_kblMapperForCompare.KBLPartMapper(DirectCast(tag, Accessory_occurrence).Part), Part)
                End If

                installationInformationList = DirectCast(tag, Accessory_occurrence).Installation_information
            ElseIf (TypeOf tag Is Assembly_part_occurrence) Then
                If (_kblMapper.KBLPartMapper.ContainsKey(DirectCast(tag, Assembly_part_occurrence).Part)) Then
                    part = DirectCast(_kblMapper.KBLPartMapper(DirectCast(tag, Assembly_part_occurrence).Part), Part)
                ElseIf (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLPartMapper.ContainsKey(DirectCast(tag, Assembly_part_occurrence).Part)) Then
                    part = DirectCast(_kblMapperForCompare.KBLPartMapper(DirectCast(tag, Assembly_part_occurrence).Part), Part)
                End If

                installationInformationList = DirectCast(tag, Assembly_part_occurrence).Installation_information
            ElseIf (TypeOf tag Is Component_box_occurrence) Then
                If (_kblMapper.KBLPartMapper.ContainsKey(DirectCast(tag, Component_box_occurrence).Part)) Then
                    part = DirectCast(_kblMapper.KBLPartMapper(DirectCast(tag, Component_box_occurrence).Part), Part)
                ElseIf (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLPartMapper.ContainsKey(DirectCast(tag, Component_box_occurrence).Part)) Then
                    part = DirectCast(_kblMapperForCompare.KBLPartMapper(DirectCast(tag, Component_box_occurrence).Part), Part)
                End If

                installationInformationList = DirectCast(tag, Component_box_occurrence).Installation_information
            ElseIf (TypeOf tag Is Component_occurrence) Then
                If (_kblMapper.KBLPartMapper.ContainsKey(DirectCast(tag, Component_occurrence).Part)) Then
                    part = DirectCast(_kblMapper.KBLPartMapper(DirectCast(tag, Component_occurrence).Part), Part)
                ElseIf (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLPartMapper.ContainsKey(DirectCast(tag, Component_occurrence).Part)) Then
                    part = DirectCast(_kblMapperForCompare.KBLPartMapper(DirectCast(tag, Component_occurrence).Part), Part)
                End If

                componentPinMaps = DirectCast(tag, Component_occurrence).Component_pin_maps
                installationInformationList = DirectCast(tag, Component_occurrence).Installation_information
            ElseIf (TypeOf tag Is Connector_occurrence) Then
                If (_kblMapper.KBLPartMapper.ContainsKey(DirectCast(tag, Connector_occurrence).Part)) Then
                    part = DirectCast(_kblMapper.KBLPartMapper(DirectCast(tag, Connector_occurrence).Part), Part)
                ElseIf (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLPartMapper.ContainsKey(DirectCast(tag, Connector_occurrence).Part)) Then
                    part = DirectCast(_kblMapperForCompare.KBLPartMapper(DirectCast(tag, Connector_occurrence).Part), Part)
                End If

                installationInformationList = DirectCast(tag, Connector_occurrence).Installation_information
            ElseIf (TypeOf tag Is Core_occurrence) Then
                If (_kblMapper.KBLCoreCableMapper.ContainsKey(DirectCast(tag, Core_occurrence).SystemId)) AndAlso (_kblMapper.KBLPartMapper.ContainsKey(DirectCast(_kblMapper.KBLOccurrenceMapper(_kblMapper.KBLCoreCableMapper(DirectCast(tag, Core_occurrence).SystemId)), Special_wire_occurrence).Part)) Then
                    part = DirectCast(_kblMapper.KBLPartMapper(DirectCast(_kblMapper.KBLOccurrenceMapper(_kblMapper.KBLCoreCableMapper(DirectCast(tag, Core_occurrence).SystemId)), Special_wire_occurrence).Part), Part)
                ElseIf (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLCoreCableMapper.ContainsKey(DirectCast(tag, Core_occurrence).SystemId)) AndAlso (_kblMapperForCompare.KBLPartMapper.ContainsKey(DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(_kblMapperForCompare.KBLCoreCableMapper(DirectCast(tag, Core_occurrence).SystemId)), Special_wire_occurrence).Part)) Then
                    part = DirectCast(_kblMapperForCompare.KBLPartMapper(DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(_kblMapperForCompare.KBLCoreCableMapper(DirectCast(tag, Core_occurrence).SystemId)), Special_wire_occurrence).Part), Part)
                End If

                If (_kblMapper.KBLCoreCableMapper.ContainsKey(DirectCast(tag, Core_occurrence).SystemId)) AndAlso (_kblMapper.KBLOccurrenceMapper.ContainsKey(_kblMapper.KBLCoreCableMapper(DirectCast(tag, Core_occurrence).SystemId))) Then
                    installationInformationList = DirectCast(_kblMapper.KBLOccurrenceMapper(_kblMapper.KBLCoreCableMapper(DirectCast(tag, Core_occurrence).SystemId)), Special_wire_occurrence).Installation_information
                ElseIf (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLCoreCableMapper.ContainsKey(DirectCast(tag, Core_occurrence).SystemId)) AndAlso (_kblMapperForCompare.KBLOccurrenceMapper.ContainsKey(_kblMapperForCompare.KBLCoreCableMapper(DirectCast(tag, Core_occurrence).SystemId))) Then
                    installationInformationList = DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(_kblMapperForCompare.KBLCoreCableMapper(DirectCast(tag, Core_occurrence).SystemId)), Special_wire_occurrence).Installation_information
                End If

                wireLengthList = DirectCast(tag, Core_occurrence).Length_information
            ElseIf (TypeOf tag Is Co_pack_occurrence) Then
                If (_kblMapper.KBLPartMapper.ContainsKey(DirectCast(tag, Co_pack_occurrence).Part)) Then
                    part = DirectCast(_kblMapper.KBLPartMapper(DirectCast(tag, Co_pack_occurrence).Part), Part)
                ElseIf (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLPartMapper.ContainsKey(DirectCast(tag, Co_pack_occurrence).Part)) Then
                    part = DirectCast(_kblMapperForCompare.KBLPartMapper(DirectCast(tag, Co_pack_occurrence).Part), Part)
                End If

                installationInformationList = DirectCast(tag, Co_pack_occurrence).Installation_information
            ElseIf (TypeOf tag Is Fixing_occurrence) Then
                If (_kblMapper.KBLPartMapper.ContainsKey(DirectCast(tag, Fixing_occurrence).Part)) Then
                    part = DirectCast(_kblMapper.KBLPartMapper(DirectCast(tag, Fixing_occurrence).Part), Part)
                ElseIf (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLPartMapper.ContainsKey(DirectCast(tag, Fixing_occurrence).Part)) Then
                    part = DirectCast(_kblMapperForCompare.KBLPartMapper(DirectCast(tag, Fixing_occurrence).Part), Part)
                End If
                fixingAssignment = GetFixingAssignment(DirectCast(tag, Fixing_occurrence).SystemId)
                installationInformationList = DirectCast(tag, Fixing_occurrence).Installation_information
            ElseIf (TypeOf tag Is General_wire_occurrence) Then
                If (_kblMapper.KBLPartMapper.ContainsKey(DirectCast(tag, General_wire_occurrence).Part)) Then
                    part = DirectCast(_kblMapper.KBLPartMapper(DirectCast(tag, General_wire_occurrence).Part), Part)
                ElseIf (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLPartMapper.ContainsKey(DirectCast(tag, General_wire_occurrence).Part)) Then
                    part = DirectCast(_kblMapperForCompare.KBLPartMapper(DirectCast(tag, General_wire_occurrence).Part), Part)
                End If

                installationInformationList = DirectCast(tag, General_wire_occurrence).Installation_information
                wireLengthList = DirectCast(tag, General_wire_occurrence).Length_information
            ElseIf (TypeOf tag Is Protection_area) AndAlso (_kblMapper.KBLOccurrenceMapper.ContainsKey(DirectCast(tag, Protection_area).Associated_protection)) Then
                If (_kblMapper.KBLPartMapper.ContainsKey(DirectCast(_kblMapper.KBLOccurrenceMapper(DirectCast(tag, Protection_area).Associated_protection), Wire_protection_occurrence).Part)) Then
                    part = DirectCast(_kblMapper.KBLPartMapper(DirectCast(_kblMapper.KBLOccurrenceMapper(DirectCast(tag, Protection_area).Associated_protection), Wire_protection_occurrence).Part), Part)
                ElseIf (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLPartMapper.ContainsKey(DirectCast(_kblMapper.KBLOccurrenceMapper(DirectCast(tag, Protection_area).Associated_protection), Wire_protection_occurrence).Part)) Then
                    part = DirectCast(_kblMapperForCompare.KBLPartMapper(DirectCast(_kblMapper.KBLOccurrenceMapper(DirectCast(tag, Protection_area).Associated_protection), Wire_protection_occurrence).Part), Part)
                End If

                installationInformationList = DirectCast(_kblMapper.KBLOccurrenceMapper(DirectCast(tag, Protection_area).Associated_protection), Wire_protection_occurrence).Installation_information
            ElseIf TypeOf tag Is KeyValuePair(Of String, Object) Then
                Dim key As String = DirectCast(e.GridRow.Tag, KeyValuePair(Of String, Object)).Key

                If key.Contains("$"c) Then
                    If (columnKey = ConnectorPropertyName.Plug_part_information) Then
                        cellValue = RequestDialogCompareData(False, key.SplitRemoveEmpty("$"c)(0).SplitRemoveEmpty("|"c)(1), DirectCast(tag, KeyValuePair(Of String, Object)))
                    Else
                        cellValue = RequestDialogCompareData(False, key.SplitRemoveEmpty("$"c)(1).SplitRemoveEmpty("|"c)(1), DirectCast(tag, KeyValuePair(Of String, Object)))
                    End If
                ElseIf key.Contains("|"c) Then
                    cellValue = RequestDialogCompareData(False, key.SplitRemoveEmpty("|"c)(1), DirectCast(tag, KeyValuePair(Of String, Object)))
                Else
                    cellValue = RequestDialogCompareData(False, key, DirectCast(tag, KeyValuePair(Of String, Object)))
                End If
            End If

            If (componentPinMaps IsNot Nothing) OrElse (installationInformationList IsNot Nothing) OrElse (node IsNot Nothing) OrElse (part IsNot Nothing) OrElse (segment IsNot Nothing) OrElse (wireLengthList IsNot Nothing) Then
                Select Case columnKey
                    Case ComponentPropertyName.Mounting
                        If (TryCast(tag, Component_occurrence)?.Mounting IsNot Nothing) Then
                            cellValue = GetExportCellValuesFromMounting(DirectCast(tag, Component_occurrence))
                        End If
                    Case PartPropertyName.Part_alias_ids, VertexPropertyName.Alias_id, ObjectPropertyNameStrings.AliasId
                        cellValue = GetExportCellValuesFromAliasIds(node, part, segment)
                    Case PartPropertyName.Change
                        cellValue = GetExportCellValuesFromChanges(part)
                    Case PartPropertyName.External_references, ObjectPropertyNameStrings.ExternalReferences
                        cellValue = GetExportCellValuesFromExternalReferences(part)
                    Case PartPropertyName.Material_information, ObjectPropertyNameStrings.MaterialInformation
                        Dim cell_values_str As New System.Text.StringBuilder
                        With part.Material_information
                            cell_values_str.Append(String.Format(InformationHubStrings.MatKey_ExcelComment, .Material_key))
                            If (.Material_reference_system IsNot Nothing) Then
                                cell_values_str.Append(String.Format(InformationHubStrings.MatRefSys_ExcelComment, .Material_reference_system))
                            End If
                        End With
                        cellValue = cell_values_str.ToString
                    Case PartPropertyName.Part_processing_information, "processing_Information", ObjectPropertyNameStrings.ProcessingInformation
                        cellValue = GetExportCellValuesFromProcessingInformation(node, part, fixingAssignment)
                    Case FixingPropertyName.AssignmentProcessingInfos
                        cellValue = GetExportCellValuesFromProcessingInformation(Nothing, Nothing, fixingAssignment)
                    Case VertexPropertyName.Referenced_components
                        cellValue = GetExportCellValuesFromReferencedComponents(node)
                    Case SegmentPropertyName.Cross_Section_Area_information
                        cellValue = GetExportCellValuesFromCrossSectionAreaInformation(segment)
                    Case SegmentPropertyName.Fixing_Assignment
                        cellValue = GetExportCellValuesFromFixingAssignment(segment)
                    Case WirePropertyName.Length_information, NameOf(InformationHubStrings.AddLengthInfo_ColumnCaption)
                        cellValue = GetExportCellValuesFromWireLengthes(columnKey, wireLengthList)
                    Case WirePropertyName.Installation_Information
                        Dim inst_infos_cell_values As New System.Text.StringBuilder
                        For Each installationInformation As Installation_instruction In installationInformationList
                            inst_infos_cell_values.AppendLine(String.Format(InformationHubStrings.InstrType2_ExcelComment, installationInformation.Instruction_type))
                            inst_infos_cell_values.Append(String.Format(InformationHubStrings.InstrVal2_ExcelComment, installationInformation.Instruction_value))
                        Next
                        cellValue = inst_infos_cell_values.ToString
                    Case WirePropertyName.Routing
                        cellValue = GetExportCellValuesFromRouting(tag)
                    Case ComponentPropertyName.ComponentPinMaps
                        Dim comp_pinMap_cell_values As New System.Text.StringBuilder
                        For Each componentPinMap As Component_pin_map In componentPinMaps
                            comp_pinMap_cell_values.AppendLine(String.Format(InformationHubStrings.CompPinMapPinNumber_ExcelComment, componentPinMap.Component_pin_number))
                            comp_pinMap_cell_values.Append(String.Format(InformationHubStrings.CompPinMapCavNumber_ExcelComment, componentPinMap.Cavity_number))
                            If componentPinMap.Connected_contact_points IsNot Nothing Then
                                comp_pinMap_cell_values.Append(String.Format(InformationHubStrings.CompPinMapContactPoint_ExcelComment, componentPinMap.GetConnectedContactPointInformation(If(_kblMapper.KBLPartMapper.ContainsKey(DirectCast(tag, Component_occurrence).Part), _kblMapper, _kblMapperForCompare))))
                            End If
                        Next
                        cellValue = comp_pinMap_cell_values.ToString
                End Select
            End If

            If (cellValue.Length > Int16.MaxValue) Then
                cellValue = String.Format("{0}~", cellValue.Substring(0, Int16.MaxValue))

                _displayValueExceedsExcelCellLimitWarning = True
            End If

            e.CurrentWorksheet.Rows(e.CurrentRowIndex).Cells(e.CurrentColumnIndex).CellFormat.Font.Height = 160
            e.CurrentWorksheet.Rows(e.CurrentRowIndex).Cells(e.CurrentColumnIndex).Value = cellValue
        End If
    End Sub

    Private Sub UgeeInformationHub_ExportEnded(ByVal sender As Object, ByVal e As ExportEndedEventArgs) Handles ugeeInformationHub.ExportEnded
        If (_progressBar Is Nothing) Then
            _messageEventArgs.ProgressPercentage = 0
            _messageEventArgs.StatusMessage = String.Empty

            RaiseEvent Message(Me, _messageEventArgs)
        End If

        _exportRunning = False
        _skipDescendants = True

        If (Not e.CurrentWorksheet.Name.EndsWith(InformationHubStrings.FlatSuffix1_ExcelWorksheet)) Then
            Select Case Me.utcInformationHub.ActiveTab.Text 'TODO: this is cumbersome, there must be a non localized way to map the active tab to KblObjectType
                Case KblObjectType.Special_wire_occurrence.ToLocalizedPluralString,
                     KblObjectType.Connector_occurrence.ToLocalizedPluralString,
                     KblObjectType.Harness_module.ToLocalizedPluralString,
                     KblObjectType.Net.ToLocalizedPluralString,
                     KblObjectType.Segment.ToLocalizedPluralString,
                     KblObjectType.Node.ToLocalizedPluralString
                    Me.ugeeInformationHub.Export(ActiveGrid, e.Workbook.Worksheets.Add(String.Format(InformationHubStrings.FlatSuffix2_ExcelWorksheet, Me.utcInformationHub.ActiveTab.Text)))
            End Select
        End If

        _skipDescendants = False
    End Sub

    Private Sub UgeeInformationHub_ExportStarted(sender As Object, e As ExportStartedEventArgs) Handles ugeeInformationHub.ExportStarted
        _exportRunning = True

        If (_progressBar Is Nothing) Then
            With _messageEventArgs
                .ProgressPercentage = 0
                .StatusMessage = String.Format(InformationHubStrings.ExportingToExcel_LogMsg, e.CurrentWorksheet.Name)
                .SetUseLastPosition()
            End With

            RaiseEvent Message(Me, _messageEventArgs)
        End If
    End Sub

    Private Sub UgeeInformationHub_HeaderRowExporting(sender As Object, e As HeaderRowExportingEventArgs) Handles ugeeInformationHub.HeaderRowExporting
        _currentHeaderRowIndex = If(e.CurrentOutlineLevel = 0, 0, e.CurrentRowIndex)

        If (e.CurrentRowIndex <> 0) AndAlso (e.CurrentOutlineLevel = 0) Then
            e.Cancel = True
        End If
    End Sub

    Private Sub UgeeInformationHub_InitializeRow(sender As Object, e As ExcelExportInitializeRowEventArgs) Handles ugeeInformationHub.InitializeRow
        'HINT Filtering out rows from the export if they are filtered out on the grid
        If (_skipDescendants) Then
            e.SkipDescendants = True
        End If

        With ActiveGrid
            Dim gridRow As UltraGridRow = .GetRowFromPrintRow(e.Row)
            If (gridRow.HiddenResolved) OrElse (_exportOnlyActiveSelectedRows AndAlso Not .Selected.Rows.Contains(gridRow)) Then
                Dim skipRow As Boolean = True

                If (gridRow.HasChild) Then
                    For Each childRow As UltraGridRow In gridRow.ChildBands(0).Rows
                        If (.Selected.Rows.Contains(childRow)) Then
                            skipRow = False
                            Exit For
                        End If
                    Next
                End If

                e.SkipDescendants = skipRow
                e.SkipRow = skipRow
            End If

            If (_skipDescendants) AndAlso (gridRow.Appearance.ForeColor = Color.Gray) AndAlso Me.utcInformationHub.ActiveTab.Text <> KblObjectType.Harness_module.ToLocalizedPluralString Then
                'HINT The check on modules tab is necessary as this is the only table with grayed out entries which get lost otherwise on flat export
                'implicit junk solution!
                e.SkipRow = True
            End If
        End With

        If (e.Row.ParentRow Is Nothing) Then
            If (_progressBar IsNot Nothing) Then
                _progressBar.Value = CInt((e.CurrentRowIndex * 100) / If(e.Row.ParentCollection.Count > e.CurrentRowIndex, e.Row.ParentCollection.Count, e.CurrentRowIndex))
                _progressBar.Update()
            Else
                _messageEventArgs.ProgressPercentage = CInt((e.CurrentRowIndex * 100) / If(e.Row.ParentCollection.Count > e.CurrentRowIndex, e.Row.ParentCollection.Count, e.CurrentRowIndex))

                RaiseEvent Message(Me, _messageEventArgs)
            End If
        End If
    End Sub

    Private Sub UgeeInformationHub_RowExported(sender As Object, e As RowExportedEventArgs) Handles ugeeInformationHub.RowExported
        Dim hasComments As Boolean = False

        For Each cell As Infragistics.Documents.Excel.WorksheetCell In e.CurrentWorksheet.Rows(e.CurrentRowIndex - 1).Cells
            If (cell.HasComment) Then
                e.CurrentWorksheet.Rows(e.CurrentRowIndex).Cells(cell.ColumnIndex).CellFormat.Font.ColorInfo = New Infragistics.Documents.Excel.WorkbookColorInfo(CHANGED_MODIFIED_FORECOLOR.ToColor)
                e.CurrentWorksheet.Rows(e.CurrentRowIndex).Cells(cell.ColumnIndex).Value = String.Format("{0} {1}", InformationHubStrings.Compare_CellValue, cell.Comment.Text.ToString.Split(New String() {InformationHubStrings.CompDoc1_TooltipPart}, StringSplitOptions.RemoveEmptyEntries).LastOrDefault.OrEmpty.Trim)

                cell.Comment = Nothing
                cell.Value = String.Format("{0} {1}", InformationHubStrings.Reference_CellValue, cell.Value)

                hasComments = True
            End If
        Next

        If hasComments Then
            For Each cell As Infragistics.Documents.Excel.WorksheetCell In e.CurrentWorksheet.Rows(e.CurrentRowIndex - 1).Cells
                If Not ((cell.Value.ToString.StartsWith(InformationHubStrings.Compare_CellValue)) OrElse (cell.Value.ToString.StartsWith(InformationHubStrings.Reference_CellValue))) Then
                    Dim mergedCells As Infragistics.Documents.Excel.WorksheetMergedCellsRegion = e.CurrentWorksheet.MergedCellsRegions.Add(e.CurrentRowIndex - 1, cell.ColumnIndex, e.CurrentRowIndex, cell.ColumnIndex)
                    mergedCells.CellFormat.VerticalAlignment = Infragistics.Documents.Excel.VerticalCellAlignment.Center
                End If
            Next

            e.CurrentRowIndex += 1
        End If
    End Sub

    Private Sub UtcInformationHub_ActiveTabChanged(sender As Object, e As ActiveTabChangedEventArgs) Handles utcInformationHub.ActiveTabChanged
        OnActiveTabChanged(e) ' HINT: should be fired before doing stuff with ActiveMarkedRows -> this will also update the ActiveMarkedRows within CurrentRowFilterInfo

        Me.ActiveGrid?.Focus()

        If Not _exportRunning AndAlso _kblMapperForCompare Is Nothing Then
            If e.Tab.Key <> TabNames.tabHarness.ToString Then
                Dim grid As UltraGrid = DirectCast(e.Tab.TabPage.Controls(0), UltraGrid)

                BeginGridUpdate()

                If _previousGrid IsNot Nothing Then
                    Dim selectedOriginKblIds As New List(Of String)
                    If _fromGridContextMenu Then
                        If _previousGrid.Name = ugQMStamps.Name Then
                            If _previousGrid.Selected.Rows.Count <> 0 Then
                                For Each selectedRow As UltraGridRow In _previousGrid.Selected.Rows
                                    selectedOriginKblIds.Add(selectedRow.Tag?.ToString)
                                Next
                            ElseIf (_previousGrid.ActiveCell IsNot Nothing) Then
                                selectedOriginKblIds.Add(_previousGrid.ActiveCell.Row.Tag?.ToString)
                            ElseIf (_previousGrid.ActiveRow IsNot Nothing) Then
                                selectedOriginKblIds.Add(_previousGrid.ActiveRow.Tag?.ToString)
                            End If
                        Else
                            If _previousGrid.Selected.Rows.Count = 0 AndAlso _previousGrid.ActiveRow IsNot Nothing Then
                                _previousGrid.Selected.Rows.Add(_previousGrid.ActiveRow)
                            End If

                            For Each selectedRow As UltraGridRow In _previousGrid.Selected.Rows
                                If selectedRow.Band.Key = KblObjectType.Net.ToString AndAlso selectedRow.HasChild Then
                                    For Each childRow As UltraGridRow In selectedRow.ChildBands(0).Rows
                                        If childRow.Tag IsNot Nothing Then
                                            selectedOriginKblIds.TryAdd(childRow.Tag?.ToString)
                                        End If
                                    Next
                                ElseIf (selectedRow.Band.Key = KblObjectType.Redlining.ToString) Then
                                    If (selectedRow.Cells("ObjectId").Value IsNot Nothing) AndAlso (Not selectedOriginKblIds.Contains(selectedRow.Cells("ObjectId").Value.ToString)) Then
                                        selectedOriginKblIds.Add(selectedRow.Cells("ObjectId").Value.ToString)
                                    End If
                                ElseIf (selectedRow.Tag IsNot Nothing) AndAlso (TypeOf selectedRow.Tag Is List(Of String)) AndAlso (DirectCast(selectedRow.Tag, List(Of String)).Count <> 0) Then
                                    For Each kblId As String In DirectCast(selectedRow.Tag, List(Of String))
                                        selectedOriginKblIds.TryAdd(kblId)
                                    Next
                                ElseIf (selectedRow.Tag IsNot Nothing) Then
                                    selectedOriginKblIds.TryAdd(selectedRow.Tag?.ToString)
                                End If
                            Next
                        End If
                    End If

                    Dim kblIds As String() = If(selectedOriginKblIds.Count > 0, selectedOriginKblIds, CurrentRowFilterInfo.MarkedRows.ActiveGridKblIds).ToArray 'HINT: selectedOriginKblIds has the higher priority here ! (this priority behavior was ported from old trunk logic from method InformationHub.UtcInformationHub_ActiveTabChanged)
                    If kblIds.Length > 0 Then
                        Using Me.EnableWaitCursor
                            If selectedOriginKblIds.Count > 0 Then
                                ClearSelectedRowsInGrid(grid)
                            End If

                            If _previousGrid.Name = ugQMStamps.Name Then
                                SelectKblIdRowsInGrid(grid, _qmStamps.Stamps.Where(Function(s) kblIds.Contains(s.Id)).SelectMany(Function(stamp) stamp.ObjectReferences.Select(Function(objRef) objRef.KblId)).ToArray)
                            ElseIf _previousGrid.Name = ugRedlinings.Name Then
                                SelectKblIdRowsInGrid(grid, kblIds)
                            Else
                                SelectKblIdRowsInGrid(grid, InformationHubUtils.GetCorrespondingKblIdsFrom(kblIds, Me), True)
                            End If
                        End Using
                    End If
                End If

                EndGridUpdate(invalidate:=True)
                SetInformationHubEventArgs(Nothing, Nothing, grid?.Selected?.Rows)
            Else
                _informationHubEventArgs.KblMapperSourceId = _kblMapper.Id
                _informationHubEventArgs.ObjectIds = New HashSet(Of String)
                _informationHubEventArgs.ObjectType = E3.Lib.Schema.Kbl.KblObjectType.Harness
            End If

            OnHubSelectionChanged()
        End If

        Me.Cursor = Cursors.Default
    End Sub

    Protected Overridable Sub OnActiveTabChanged(e As ActiveTabChangedEventArgs)
        RaiseEvent ActiveTabChanged(Me, e)
    End Sub

    Private Sub UtcInformationHub_ActiveTabChanging(sender As Object, e As ActiveTabChangingEventArgs) Handles utcInformationHub.ActiveTabChanging
        If _exportRunning Then
            e.Cancel = True
        ElseIf (Me.utcInformationHub.ActiveTab IsNot Nothing) Then
            Me.Cursor = Cursors.WaitCursor
            _previousGrid = ActiveGrid
        End If
    End Sub

    Private Sub UtcInformationHub_KeyDown(sender As Object, e As KeyEventArgs) Handles utcInformationHub.KeyDown
        If e.Control AndAlso e.KeyCode = Keys.A Then
            SelectAllRowsOfActiveGrid()
        End If
    End Sub

    Private Sub UtcInformationHub_MouseClick(sender As Object, e As MouseEventArgs) Handles utcInformationHub.MouseClick
        If Me.utcInformationHub.ActiveTab IsNot Nothing Then
            If _contextMenuTab.IsOpen Then
                _contextMenuTab.ClosePopup()
            End If

            Me.utcInformationHub.ActiveTab.TabPage.Controls(0).Focus()

            If e.Button = System.Windows.Forms.MouseButtons.Right Then
                Me.utmInformationHub.Tools(InfoHubMenuToolKey.ExportExcel.ToString).SharedProps.Caption = If(CultureInfo.DefaultThreadCurrentCulture.Name = "en-US", String.Format(InformationHubStrings.ExportTableToExcel_CtxtMnu_Caption, Me.utcInformationHub.ActiveTab.Text.ToLower), String.Format(InformationHubStrings.ExportTableToExcel_CtxtMnu_Caption, Me.utcInformationHub.ActiveTab.Text))
                Me.utmInformationHub.Tools(InfoHubMenuToolKey.ExportExcel.ToString).SharedProps.Visible = True

                Me.utmInformationHub.Tools(InfoHubMenuToolKey.ClearFilters.ToString).SharedProps.Visible = True
                Me.utmInformationHub.Tools(InfoHubMenuToolKey.ClearAllFilters.ToString).SharedProps.Visible = True

                Select Case Me.utcInformationHub.ActiveTab.Text
                    Case KblObjectType.Special_wire_occurrence.ToLocalizedPluralString, KblObjectType.Connector_occurrence.ToLocalizedPluralString, KblObjectType.Net.ToLocalizedPluralString, KblObjectType.Harness_module.ToLocalizedPluralString, KblObjectType.Segment.ToLocalizedPluralString, KblObjectType.Node.ToLocalizedPluralString
                        Me.utmInformationHub.Tools(InfoHubMenuToolKey.CollapseAll.ToString).SharedProps.Visible = True
                        Me.utmInformationHub.Tools(InfoHubMenuToolKey.ExpandAll.ToString).SharedProps.Visible = True
                    Case Else
                        Me.utmInformationHub.Tools(InfoHubMenuToolKey.CollapseAll.ToString).SharedProps.Visible = False
                        Me.utmInformationHub.Tools(InfoHubMenuToolKey.ExpandAll.ToString).SharedProps.Visible = False
                End Select

                _contextMenuTab.ShowPopup()
            End If
        End If
    End Sub

    Private Sub UtcInformationHub_SelectedTabChanged(sender As Object, e As SelectedTabChangedEventArgs) Handles utcInformationHub.SelectedTabChanged
        OnAfterRowActivate()
    End Sub

    Private Sub UtmInformationHub_ToolClick(sender As Object, e As ToolClickEventArgs) Handles utmInformationHub.ToolClick
        _fromGridContextMenu = True

        Select Case e.Tool.Key.TryParse(Of InfoHubMenuToolKey).GetValueOrDefault
            Case InfoHubMenuToolKey.CalculateDistance
                CalculateDistanceBetweenSelectedObjects()

            Case InfoHubMenuToolKey.CalculateModuleWeight
                ShowModulesWeightView()

            Case InfoHubMenuToolKey.CalculateTotalLength
                CalculateTotalLength()

            Case InfoHubMenuToolKey.CalculateWireWeight
                ShowWireWeightCalculationForm()

            Case InfoHubMenuToolKey.CalculateWireResistance
                CalculateWireResistance()

            Case InfoHubMenuToolKey.ClearMarkedObjects
                ClearMarkedRowsInGrids()

            Case InfoHubMenuToolKey.CollapseAll
                CollapseAll()

            Case InfoHubMenuToolKey.CorrespondingAccessories
                BringTabIntoViewCore(TabNames.tabAccessories.ToString(True), False)

            Case InfoHubMenuToolKey.CorrespondingApprovals
                BringTabIntoViewCore(TabNames.tabApprovals.ToString(True), False)

            Case InfoHubMenuToolKey.CorrespondingAssemblyParts
                BringTabIntoViewCore(TabNames.tabAssemblyParts.ToString(True), False)

            Case InfoHubMenuToolKey.CorrespondingCables
                BringTabIntoViewCore(TabNames.tabCables.ToString(True), False)

            Case InfoHubMenuToolKey.CorrespondingChangeDescription
                BringTabIntoViewCore(TabNames.tabChangeDescriptions.ToString(True), False)

            Case InfoHubMenuToolKey.CorrespondingComponentBoxes
                BringTabIntoViewCore(TabNames.tabComponentBoxes.ToString(True), False)

            Case InfoHubMenuToolKey.CorrespondingComponents
                BringTabIntoViewCore(TabNames.tabComponents.ToString(True), False)

            Case InfoHubMenuToolKey.CorrespondingConnectors
                BringTabIntoViewCore(TabNames.tabConnectors.ToString(True), False)

            Case InfoHubMenuToolKey.CorrespondingCoPacks
                BringTabIntoViewCore(TabNames.tabCoPacks.ToString(True), False)

            Case InfoHubMenuToolKey.CorrespondingDimSpecs
                BringTabIntoViewCore(TabNames.tabDimSpecs.ToString(True), False)

            Case InfoHubMenuToolKey.CorrespondingFixings
                BringTabIntoViewCore(TabNames.tabFixings.ToString(True), False)

            Case InfoHubMenuToolKey.CorrespondingModules
                BringTabIntoViewCore(TabNames.tabModules.ToString(True), False)

            Case InfoHubMenuToolKey.CorrespondingNets
                BringTabIntoViewCore(TabNames.tabNets.ToString(True), False)

            Case InfoHubMenuToolKey.CorrespondingSegments
                BringTabIntoViewCore(TabNames.tabSegments.ToString(True), False)

            Case InfoHubMenuToolKey.CorrespondingVertices
                BringTabIntoViewCore(TabNames.tabVertices.ToString(True), False)

            Case InfoHubMenuToolKey.CorrespondingWires
                BringTabIntoViewCore(TabNames.tabWires.ToString(True), False)

            Case InfoHubMenuToolKey.Edit
                EditRedlining()

            Case InfoHubMenuToolKey.EditRedlining
                AddOrEditRedlining(Nothing)

            Case InfoHubMenuToolKey.ExpandAll
                ExpandAll()

            Case InfoHubMenuToolKey.ExportExcel
                ExportSelectedGridDataToExcel()

            Case InfoHubMenuToolKey.HighlightEntireRoutingPath, InfoHubMenuToolKey.ShowEntireRoutingPath
                HighlightOrShowEntireRoutingPath(e)

            Case InfoHubMenuToolKey.MarkSelectedObjects
                MarkActiveSelectedObjects()

            Case InfoHubMenuToolKey.MemolistAddSelected
                AddSelectedToMemoList()

            Case InfoHubMenuToolKey.SelectMarkedObjects
                SelectMarkedObjects()

            Case InfoHubMenuToolKey.ShowBundleCrossSection
                ShowBundleCrossSectionView()

            Case InfoHubMenuToolKey.ShowConnectivity
                ShowConnectivityView()

            Case InfoHubMenuToolKey.ShowContacting
                ShowContactingView()

            Case InfoHubMenuToolKey.ShowModules
                ShowModulesOfSelectedObject()

            Case InfoHubMenuToolKey.ShowSchematicsView
                ShowSchematics()

            Case InfoHubMenuToolKey.ShowOverallConnectivity
                ShowOverallConnectivityView()

            Case InfoHubMenuToolKey.ShowStartEndConnectors
                ShowStartEndConnectorView()

            Case InfoHubMenuToolKey.ClearAllFilters
                ClearFiltersOnAllTabs()

            Case InfoHubMenuToolKey.ClearFilters
                ClearFiltersOnActiveTab()

            Case InfoHubMenuToolKey.ShowSpliceProposals
                ShowSpliceProposals()

            Case InfoHubMenuToolKey.ShowRubberlinesToCorrespondingConnectors
                ShowCorrespondingRubberlines()

        End Select

        _clickedGridRow = Nothing
        _fromGridContextMenu = False
    End Sub

    Private Sub ShowSchematics()
        CType(Me.ParentForm, DocumentForm).MainForm.ShowSchematicsEntities(CType(Me.ParentForm, DocumentForm), GetSelectedRowKblIds.ToArray)
    End Sub

    Private Function GetSelectedRowKblIds() As IEnumerable(Of String)
        Dim ids As New List(Of String)
        For Each row As UltraGridRow In Me.ActiveGrid.Selected.Rows
            If Not TypeOf row.Tag Is String AndAlso TypeOf row.Tag Is IEnumerable Then
                For Each id As String In CType(row.Tag, IEnumerable).OfType(Of String)
                    If (_kblMapper.KBLOccurrenceMapper.ContainsKey(id) AndAlso TypeOf (_kblMapper.KBLOccurrenceMapper(id)) Is Contact_point) Then
                        If (_kblMapper.KBLContactPointWireMapper.ContainsKey(id)) Then
                            For Each wid As String In _kblMapper.KBLContactPointWireMapper(id)
                                'HINT use first wire, later a real entry from the cavity should be implemented
                                ids.Add(wid)
                                Exit For
                            Next
                        End If
                    End If
                Next
            ElseIf TypeOf row.Tag Is String OrElse TypeOf row.Tag Is NetRowInfoContainer Then ' HINT: check for NetRowInfoContainer is normally not needed but to emulate the old busniess logic 100% when the net row information was a string we also check for NetRowInfoContainer -> to be re-evaluated if needed anymore
                ids.Add(row.Tag?.ToString)
            End If
        Next
        Return ids
    End Function

    Private Sub ShowCorrespondingRubberlines()
        Dim myIds As List(Of String)
        myIds = CType(ugConnectors.ActiveRow.Tag, List(Of String))
        Dim id As String = myIds(0)
        TryCast(Me.DocumentForm, DocumentForm)?.ShowCorrespondingRubberlines(id)
    End Sub

    Private Sub ShowSpliceProposals()
        If ugConnectors.Selected.Rows.Count > 0 AndAlso TypeOf ugConnectors.Selected.Rows(0).Tag Is String Then
            Dim tsk As Task(Of Boolean) = CType(_parentForm, DocumentForm)?.TryShowSpliceProposalIn3D(CType(ugConnectors.Selected.Rows(0).Tag, String))
        End If
    End Sub

    Private Sub HighlightOrShowEntireRoutingPath(e As ToolClickEventArgs)
        If (_kblMapper.KBLOccurrenceMapper.ContainsKey(_clickedGridRow.Tag?.ToString)) Then
            Using Me.EnableWaitCursor
                Dim coreWireIds As New List(Of String)
                Dim cable_occ As Special_wire_occurrence = _kblMapper.GetOccurrenceObject(Of Special_wire_occurrence)(_clickedGridRow.Tag?.ToString)
                If cable_occ IsNot Nothing Then
                    coreWireIds.AddRange(cable_occ.Core_occurrence.Select(Function(c_occ) c_occ.SystemId))
                Else
                    coreWireIds.Add(_clickedGridRow.Tag?.ToString)
                End If

                RaiseEvent HighlightEntireRoutingPath(Me, New HighlightEntireRoutingPathEventArgs(coreWireIds, TypeOf _kblMapper.KBLOccurrenceMapper(_clickedGridRow.Tag?.ToString) Is Special_wire_occurrence, e.Tool.Key = InfoHubMenuToolKey.HighlightEntireRoutingPath.ToString))
            End Using

            If (e.Tool.Key = InfoHubMenuToolKey.ShowEntireRoutingPath.ToString) Then
                _informationHubEventArgs.KblMapperSourceId = _kblMapper.Id
                _informationHubEventArgs.ObjectIds = New HashSet(Of String)
                _informationHubEventArgs.ObjectType = E3.Lib.Schema.Kbl.KblObjectType.Wire_occurrence

                RaiseEvent ShowEntireRoutingPath(Me, _informationHubEventArgs)
            End If
        End If
    End Sub

    Private Sub AddSelectedToMemoList()
        Dim selectedObjects As New Dictionary(Of String, Object)

        For Each row As UltraGridRow In ActiveGrid.Selected.Rows
            If (row.ParentRow Is Nothing) Then
                If (_kblMapper.KBLOccurrenceMapper.ContainsKey(row.Tag?.ToString)) Then
                    selectedObjects.TryAdd(row.Tag?.ToString, _kblMapper.KBLOccurrenceMapper(row.Tag?.ToString))
                ElseIf (ActiveGrid.Name = ugNets.Name) Then
                    selectedObjects.TryAdd(row.Tag?.ToString, row.Tag?.ToString)
                End If
            Else
                If (_kblMapper.KBLOccurrenceMapper.ContainsKey(row.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString)) Then
                    selectedObjects.TryAdd(row.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString, _kblMapper.KBLOccurrenceMapper(row.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString))
                End If
            End If
        Next

        RaiseEvent MemolistChanged(Me, selectedObjects)
    End Sub

    Protected Function GetFixingAssignment(fixingSystemId As String) As Fixing_assignment
        Return _kblMapper.GetSegments.SelectMany(Function(seg) seg.Fixing_assignment.OrEmpty.Where(Function(fix_ass) fix_ass.Fixing = fixingSystemId)).FirstOrDefault
    End Function

    Private Sub MarkActiveSelectedObjects()
        Dim active_grid As UltraGrid = Me.ActiveGrid
        SetSelectedAsMarkedOriginRows(active_grid)
    End Sub

    Private Sub SelectMarkedObjects()
        Dim activeGrid As UltraGrid = Me.ActiveGrid
        activeGrid.BeginUpdate()
        Using activeGrid.EventManager.ProtectProperty(NameOf(GridEventManager.AllEventsEnabled), False)
            Me.SetSelectedAsMarkedOriginRows(activeGrid)
        End Using
        activeGrid.EndUpdate()

        If SetInformationHubEventArgs(Nothing, Nothing, activeGrid.Selected.Rows) Then
            OnHubSelectionChanged()
        End If
    End Sub

    Private Sub EditRedlining()
        Dim redliningData As Redlining = _redliningInformation?.Redlinings.Single(Function(redlining) redlining.ID = _clickedGridRow.Tag?.ToString)

        Dim objectNames As New List(Of String) From {
            redliningData.ObjectName
        }

        Dim objectIds As New List(Of String) From {
            redliningData?.ObjectId
        }
        _IsRedliningForCavityNavigator = False

        ShowRedliningDialog(objectIds, objectNames, redliningData.ObjectType, redliningData, Nothing)

        Me.ugRedlinings.BeginUpdate()
        Me.ugRedlinings.DisplayLayout.Bands(0).Columns(NameOf(InformationHubStrings.Group_ColumnCaption)).ValueList = GetRedliningGroups()
        Me.ugRedlinings.EndUpdate()
    End Sub

    Private Sub ExpandAll()
        With ActiveGrid
            .BeginUpdate()
            .Rows.ExpandAll(True)
            .EndUpdate()
        End With
    End Sub

    Private Sub CollapseAll()
        With ActiveGrid
            .BeginUpdate()
            .Rows.CollapseAll(True)
            .EndUpdate()
        End With
    End Sub

    Private Sub CalculateWireResistance()
        If TypeOf _clickedGridRow.Tag Is String OrElse TypeOf _clickedGridRow.Tag Is NetRowInfoContainer AndAlso TypeOf Me.DocumentForm Is DocumentForm Then
            Dim combinedId As String = HarnessConnectivity.GetUniqueId(_kblMapper.HarnessPartNumber, _clickedGridRow.Tag?.ToString)
            WireResistanceCalculatorForm.ShowDialog(Me, CType(DocumentForm, DocumentForm).MainForm.GetAllDocuments.Values.Select(Function(dc) dc.KBL), {combinedId})
        End If
    End Sub

    Private Sub CalculateTotalLength()
        Dim physicalTotalLength As Double = 0.0
        Dim physicalTotalLengthUnitName As String = String.Empty
        Dim virtualTotalLength As Double = 0.0
        Dim virtualTotalLengthUnitName As String = String.Empty

        For Each selectedRow As UltraGridRow In _grids(KblObjectType.Segment.ToString).Selected.Rows
            Dim segment As Segment = _kblMapper.GetSegment(selectedRow.Tag?.ToString)
            If (segment IsNot Nothing) Then
                If (segment.Physical_length IsNot Nothing) Then
                    physicalTotalLength += segment.Physical_length.Value_component
                    physicalTotalLengthUnitName = _kblMapper.GetUnit(segment.Physical_length.Unit_component).Unit_name
                End If

                If (segment.Virtual_length IsNot Nothing) Then
                    virtualTotalLength += segment.Virtual_length.Value_component
                    virtualTotalLengthUnitName = _kblMapper.GetUnit(segment.Virtual_length.Unit_component).Unit_name
                End If
            End If
        Next

        MessageBoxEx.ShowInfo(Me.ParentForm, String.Format(InformationHubStrings.CalcTotLength_Msg, vbCrLf, Math.Round(physicalTotalLength, 2), physicalTotalLengthUnitName, Math.Round(virtualTotalLength, 2), virtualTotalLengthUnitName))
    End Sub

    Private Sub ShowModulesWeightView()
        Using modulesWeightForm As New ModulesWeightForm(_kblMapper, _grids(KblObjectType.Module.ToString).Selected.Rows)
            modulesWeightForm.ShowDialog(Me)
        End Using
    End Sub

    Private Sub ShowOverallConnectivityView()
        Using Me.EnableWaitCursor

            Dim coreWireIds As New HashSet(Of String)
            If (_kblMapper.KBLOccurrenceMapper.ContainsKey(_clickedGridRow.Tag?.ToString)) Then
                Dim special_wire_occ As Special_wire_occurrence = _kblMapper.GetOccurrenceObject(Of Special_wire_occurrence)(_clickedGridRow.Tag?.ToString)
                If special_wire_occ IsNot Nothing Then
                    For Each coreOcc As Core_occurrence In special_wire_occ.Core_occurrence
                        coreWireIds.Add(coreOcc.SystemId)
                    Next
                Else
                    coreWireIds.Add(_clickedGridRow.Tag?.ToString)
                End If
            ElseIf (_clickedGridRow.Band.Key = KblObjectType.Cavity_occurrence.ToString) Then
                For Each cableOcc As Special_wire_occurrence In _kblMapper.KBLCableList.Where(Function(cab) cab.Core_occurrence.Any(Function(cor) cor.Wire_number = _clickedGridRow.Cells(HarnessAnalyzer.[Shared].CORE_WIRE_NUMBER_KEY).Value.ToString))
                    For Each coreOcc As Core_occurrence In cableOcc.Core_occurrence.Where(Function(cor) cor.Wire_number = _clickedGridRow.Cells(HarnessAnalyzer.[Shared].CORE_WIRE_NUMBER_KEY).Value.ToString)
                        coreWireIds.Add(coreOcc.SystemId)
                    Next
                Next

                For Each wireOcc As Wire_occurrence In _kblMapper.KBLWireList.Where(Function(wir) wir.Wire_number = _clickedGridRow.Cells(HarnessAnalyzer.[Shared].CORE_WIRE_NUMBER_KEY).Value.ToString)
                    coreWireIds.Add(wireOcc.SystemId)
                Next
            End If

            _informationHubEventArgs.KblMapperSourceId = _kblMapper.Id
            _informationHubEventArgs.ObjectIds = coreWireIds
            _informationHubEventArgs.ObjectType = E3.Lib.Schema.Kbl.KblObjectType.Wire_occurrence
        End Using

        RaiseEvent ShowOverallConnectivity(Me, _informationHubEventArgs)
    End Sub

    Private Sub ShowStartEndConnectorView()
        Dim objectIds As New HashSet(Of String)
        If (_kblMapper.KBLWireNetMapper.ContainsKey(_clickedGridRow.Tag?.ToString)) Then
            objectIds.Add(_clickedGridRow.Tag?.ToString)

            If (_kblMapper.KBLWireNetMapper(_clickedGridRow.Tag?.ToString).Extremities.Length > 1) Then
                If (_kblMapper.KBLContactPointConnectorMapper.ContainsKey(_kblMapper.KBLWireNetMapper(_clickedGridRow.Tag?.ToString).GetStartContactPointId)) Then
                    objectIds.Add(_kblMapper.KBLContactPointConnectorMapper(_kblMapper.KBLWireNetMapper(_clickedGridRow.Tag?.ToString).GetStartContactPointId))
                End If

                If (_kblMapper.KBLContactPointConnectorMapper.ContainsKey(_kblMapper.KBLWireNetMapper(_clickedGridRow.Tag?.ToString).GetEndContactPointId)) Then
                    objectIds.Add(_kblMapper.KBLContactPointConnectorMapper(_kblMapper.KBLWireNetMapper(_clickedGridRow.Tag?.ToString).GetEndContactPointId))
                End If
            End If
        Else
            Dim special_wire_occ As Special_wire_occurrence = _kblMapper.GetOccurrenceObject(Of Special_wire_occurrence)(_clickedGridRow.Tag?.ToString)
            If special_wire_occ IsNot Nothing Then
                For Each core As Core_occurrence In special_wire_occ.Core_occurrence
                    objectIds.Add(core.SystemId)

                    If (_kblMapper.KBLWireNetMapper.ContainsKey(core.SystemId)) AndAlso (_kblMapper.KBLWireNetMapper(core.SystemId).Extremities.Length > 1) Then
                        If (_kblMapper.KBLContactPointConnectorMapper.ContainsKey(_kblMapper.KBLWireNetMapper(core.SystemId).GetStartContactPointId)) Then
                            objectIds.Add(_kblMapper.KBLContactPointConnectorMapper(_kblMapper.KBLWireNetMapper(core.SystemId).GetStartContactPointId))
                        End If

                        If (_kblMapper.KBLContactPointConnectorMapper.ContainsKey(_kblMapper.KBLWireNetMapper(core.SystemId).GetEndContactPointId)) Then
                            objectIds.Add(_kblMapper.KBLContactPointConnectorMapper(_kblMapper.KBLWireNetMapper(core.SystemId).GetEndContactPointId))
                        End If
                    End If

                    Exit For
                Next
            End If
        End If

        If objectIds.Count > 0 Then
            _informationHubEventArgs.KblMapperSourceId = _kblMapper.Id
            _informationHubEventArgs.ObjectIds = objectIds
            _informationHubEventArgs.ObjectType = E3.Lib.Schema.Kbl.KblObjectType.Wire_occurrence
            RaiseEvent ShowStartEndConnectors(Me, _informationHubEventArgs)
        End If
    End Sub

    Private Sub ShowBundleCrossSectionView()
        Dim dialogExists As Boolean = False

        For Each form As Form In My.Application.OpenForms
            If (TypeOf form Is BundleCrossSectionForm) AndAlso (DirectCast(form, BundleCrossSectionForm).Tag.Equals(DirectCast(_kblMapper.KBLOccurrenceMapper(_clickedGridRow.Tag?.ToString), Segment))) Then
                dialogExists = True

                form.BringToFront()
                form.Focus()

                Exit For
            End If
        Next

        If (Not dialogExists) Then
            _bundleCrossSectionForm = New BundleCrossSectionForm(DirectCast(_parentForm, DocumentForm)._calculatedSegmentDiameters, _diameterSettings, _parentForm.Text, DirectCast(_parentForm, DocumentForm)._harnessModuleConfigurations, _kblMapper, DirectCast(_kblMapper.KBLOccurrenceMapper(_clickedGridRow.Tag?.ToString), Segment), _generalSettings.WireColorCodes)

            If (_bundleCrossSectionForm.IsWireRoutingAvailable) Then
                _bundleCrossSectionForm.Show(Me)
            Else
#If DEBUG Or CONFIG = "Debug" Then
                MessageBoxEx.ShowInfo(Me, "Routing not availalbe")
#End If
            End If
        End If
    End Sub

    Private Sub ShowConnectivityView()
        Dim conn_occ As Connector_occurrence = _kblMapper.GetOccurrenceObject(Of Connector_occurrence)(_clickedGridRow.Tag?.ToString)
        If conn_occ IsNot Nothing Then
            Using connectivityView As New ConnectivityView(conn_occ, _currentRowFilterInfo.InactiveObjects, _kblMapper, _generalSettings.WireColorCodes)
                AddHandler connectivityView.ConnectivityViewMouseClick, AddressOf ConnectivityView_ConnectivityViewMouseClick
                connectivityView.ShowDialog(Me)
                RemoveHandler connectivityView.ConnectivityViewMouseClick, AddressOf ConnectivityView_ConnectivityViewMouseClick
            End Using
        Else
            MessageBoxEx.ShowWarning($"No connector occurrence found! ({_clickedGridRow.Tag?.ToString})")
        End If
    End Sub

    Private Sub ShowContactingView()
        Using contatingView As New ContactingView(_kblMapper.GetOccurrenceObject(Of Connector_occurrence)(_clickedGridRow.Tag?.ToString), _kblMapper)
            contatingView.ShowDialog(Me)
        End Using
    End Sub

    Private Sub ClearFiltersOnActiveTab()
        ActiveGrid.DisplayLayout.Bands(0).ColumnFilters.ClearAllFilters()
    End Sub

    Private Sub ClearFiltersOnAllTabs()
        For Each tab As UltraTab In Me.utcInformationHub.Tabs
            DirectCast(tab.TabPage.Controls(0), UltraGrid).DisplayLayout.Bands(0).ColumnFilters.ClearAllFilters()
        Next
    End Sub

    Public ReadOnly Property ActiveGrid As UltraGrid
        Get
            Return utcInformationHub?.ActiveTab?.TabPage?.Controls.OfType(Of UltraGrid).SingleOrDefault
        End Get
    End Property

    Friend Shared ReadOnly Property CompareCommentColumnKey As String
        Get
            Return String.Format("{0} (Compare)", GraphicalCompareFormStrings.Comment_ColumnCaption)
        End Get
    End Property

    Public ReadOnly Property DocumentForm As IDocumentForm
        Get
            If TypeOf Me.ParentForm Is CompareForm Then
                Return CType(Me.ParentForm, CompareForm).ActiveDocument
            Else
                Return TryCast(_parentForm, IDocumentForm)
            End If
        End Get
    End Property

    Public ReadOnly Property ExcelExportRunning As Boolean
        Get
            Return _exportRunning
        End Get
    End Property

    Public Shadows ReadOnly Property ParentForm As Form
        Get
            Return _parentForm
        End Get
    End Property

    Public ReadOnly Property Rows As IEnumerable(Of UltraDataRow)
        Get
            Return _filledDataSources.SelectMany(Function(ds) ds.Rows.Cast(Of UltraDataRow))
        End Get
    End Property

    Public ReadOnly Property Kbl As KblMapper
        Get
            Return _kblMapper
        End Get
    End Property

    Friend ReadOnly Property RedliningInfo As RedliningInformation
        Get
            Return _redliningInformation
        End Get
    End Property

    Friend ReadOnly Property KblIdRowCache As KblIdRowCache
        Get
            Return _kblIdRowCache
        End Get
    End Property

    Protected Overridable Sub OnHubSelectionChanged()
        _isHubSelecting = True
        Try
            RaiseEvent HubSelectionChanged(Me, _informationHubEventArgs)
        Finally
            _isHubSelecting = False
        End Try
    End Sub

End Class