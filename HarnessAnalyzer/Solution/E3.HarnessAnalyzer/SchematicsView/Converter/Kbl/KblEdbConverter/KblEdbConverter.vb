Imports System.ComponentModel
Imports Zuken.E3.App.Controls
Imports Zuken.E3.HarnessAnalyzer.Schematics.Controls
Imports Zuken.E3.Lib.Converter.Unit

Namespace Schematics.Converter.Kbl

    Public Class KblEdbConverter
        Implements IDisposable
        Implements IKblConverter

        Private Event IKblConverter_ConnectorResolving(sender As Object, e As ConverterEventArgs) Implements IKblConverter.ConnectorResolving
        Private Event IKblConverter_BeforeConversionStart(sender As Object, e As CancelEventArgs) Implements IKblConverter.IKblConverter_BeforeConversionStart
        Private Event IKblConverter_AfterConversionFinished(sender As Object, e As FinishedEventArgs) Implements IKblConverter.IKblConverter_AfterConversionFinished

        Public Event ResolveEntityId(sender As Object, e As IdEventArgs) Implements IKblConverter.ResolveEntityId
        Public Event AfterEntityCreated(sender As Object, e As EntityEventArgs) Implements IKblConverter.AfterEdbEntityCreated

        Public Event ErrorInitialize(sender As Object, e As InitializeErrorEventArgs)
        Public Event ConnectorResolving(sender As Object, e As ConnectorResolvingEventArgs)
        Public Event ResolveComponentType(sender As Object, e As ComponentTypeResolveEventArgs)

        Public Event AfterModulesSet(sender As Object, e As ModuleSetEventArgs)
        Public Event AfterFunctionSet(sender As Object, e As FunctionSetEventArgs)

        Public Event ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs)

        Private _combinedMappers As New CombinedKblData
        Private _edbConnectorInfoMapper As New ConnectorInfoMapperCollection
        Private _currentConvertedItems As New Schematics.Converter.DocumentsCollection

        Private _inlinerCavityMap As New Dictionary(Of String, EdbConversionCavityEntity)
        Private _inlinerConnectorMap As New Dictionary(Of String, EdbConversionConnectorEntity)
        Private _edbIdsResolved As New Dictionary(Of String, Dictionary(Of String, String))
        Private _componentTypesResolved As New Dictionary(Of String, Connectivity.Model.ComponentType)
        Private _edbModel As Connectivity.Model.EdbModel

        Private _addNewOrGetObjectGetter As New AttributeMethodGetter(Of KblEdbConverter, UntypedAddObjectMethodAttribute)()
        Private _getEdbIdInternalGetter As New AttributeMethodGetter(Of KblEdbConverter, GetEdbSystemIdInternalMethodAttribute)()

        Private _dummyComponentCount As Integer = 0
        Private _cavityPartsToOccurrencesMapper As ConnectorCavityBlocksInternal

        Public Sub New()
        End Sub

        Public Sub Init(data() As KblDocumentData, Optional cancelToken As Nullable(Of System.Threading.CancellationToken) = Nothing)
            _combinedMappers.Init(data, AddressOf OnInitializeError, cancelToken)
            InitCavityPartsToOccurrencesMapper(cancelToken)
        End Sub

        Private Sub InitCavityPartsToOccurrencesMapper(Optional cancelToken As Nullable(Of System.Threading.CancellationToken) = Nothing)
            If Not (cancelToken?.IsCancellationRequested).GetValueOrDefault Then
                'HINT: merge all connector_occurrences by it's part (for each connector) together -> this is needed for conversion when there are f.e. different occurrences mapped to same part -> the conversion will take care of this for schematics to also merge to occurrences to this shared-cavty-part of the connector together
                TryCast(_cavityPartsToOccurrencesMapper, IDisposable)?.Dispose()
                _cavityPartsToOccurrencesMapper = New ConnectorCavityBlocksInternal(_combinedMappers)

                For Each cavOccDocsGroup As IObjectGroup In _combinedMappers.Occurrences.OfObjectType(Of Cavity_occurrence) ' HINT: filter all "object-to-documents-groups" down to contain only cavities
                    If _combinedMappers.CavityToComponentBoxConnector.Contains(cavOccDocsGroup.ObjectId) Then
                        Continue For 'HINT this is a preliminary fix, as the cavity occurences of a component box have no relation to the normal connector occurences.
                    End If

                    For Each kv As DictionaryEntry In cavOccDocsGroup ' for each document
                        If cancelToken?.IsCancellationRequested Then
                            Return
                        End If

                        Dim cavityBlock As ConnectorCavityDocumentBlock = _cavityPartsToOccurrencesMapper.AddOrGet(CStr(kv.Key))
                        Dim cavityOcc As Cavity_occurrence = CType(kv.Value, Cavity_occurrence)
                        Dim partGroup As InternalCavityPartGroup = cavityBlock.AddOrUpdatePart(cavityOcc)
                    Next
                Next
            End If
        End Sub

        Private Sub OnInitializeError(e As InitializeError)
            Select Case e.ErrorType
                Case InitializeError.Type.AddErrorCavityAtWireMapper
                    OnInitializeError(New InitializeErrorEventArgs(InitializeErrorEventArgs.ErrorType.EntityAlreadyExists, e.DocumentId, e.Object, KblEdbConverterErrorStrings.EqualCavityInDifferentWiresError))
                Case InitializeError.Type.AddErrorConnectorAtCavityMapper
                    OnInitializeError(New InitializeErrorEventArgs(InitializeErrorEventArgs.ErrorType.EntityAlreadyExists, e.DocumentId, e.Object, KblEdbConverterErrorStrings.EqualConnectionAtDifferentCavitiesError))
                Case InitializeError.Type.AddErrorObjectAtModuleMapper
                    OnInitializeError(New InitializeErrorEventArgs(InitializeErrorEventArgs.ErrorType.EntityAlreadyExists, e.DocumentId, e.Object, KblEdbConverterErrorStrings.ModuleMapError))
                Case InitializeError.Type.AddErrorPartMapper
                    OnInitializeError(New InitializeErrorEventArgs(InitializeErrorEventArgs.ErrorType.EntityAlreadyExists, e.DocumentId, e.Object, KblEdbConverterErrorStrings.EqualPartsInDocumentError))
                Case InitializeError.Type.AddErrorUnitsMapper
                    OnInitializeError(New InitializeErrorEventArgs(InitializeErrorEventArgs.ErrorType.EntityAlreadyExists, e.DocumentId, e.Object, KblEdbConverterErrorStrings.EqualUnitsInDocumentError))
                Case InitializeError.Type.AddErrorWireAtCavityMapper
                    OnInitializeError(New InitializeErrorEventArgs(InitializeErrorEventArgs.ErrorType.EntityAlreadyExists, e.DocumentId, e.Object, KblEdbConverterErrorStrings.EqualWireAtDifferentCavitiesError))
                Case InitializeError.Type.AddErrorWireAtWireGroupMapper
                    OnInitializeError(New InitializeErrorEventArgs(InitializeErrorEventArgs.ErrorType.EntityAlreadyExists, e.DocumentId, e.Object, KblEdbConverterErrorStrings.EqualWireInDifferentWireGroupsError))
                Case InitializeError.Type.AddErrorWireConenctions
                    OnInitializeError(New InitializeErrorEventArgs(InitializeErrorEventArgs.ErrorType.EntityAlreadyExists, e.DocumentId, e.Object, KblEdbConverterErrorStrings.EqualWireConnectionMappingError))
                Case Else
                    OnInitializeError(New InitializeErrorEventArgs(InitializeErrorEventArgs.ErrorType.Unknown, e.DocumentId, e.Object, KblEdbConverterErrorStrings.Unknown_Error))
            End Select
        End Sub

        Protected Overridable Sub OnInitializeError(e As InitializeErrorEventArgs)
            RaiseEvent ErrorInitialize(Me, e)
        End Sub

        Public Function CreateEdb(Optional cancel As Threading.CancellationToken = Nothing) As KblConversionResult
            Dim success As Boolean = False
            Dim startArgs As New CancelEventArgs()

            Using resultAdapter As KblConversionResult.KblConversionAdapter = KblConversionResult.KblConversionAdapter.AttachNewResultTo(Me)
                Try
                    OnBeforeConversionStart(startArgs)
                    If Not startArgs.Cancel Then
                        Init()

                        CreateConnectorInfoAndConnectorComponentMapper(cancel) 'HINT: always at first because the next step needs the ComponentConnectorMapper to resolve the shortnames, connector-names/inliners
                        CreateEdbObjects(cancel)
                        _edbModel.CompleteModelCreation()
                        success = True
                    End If
                Finally
                    OnAfterConversion(New FinishedEventArgs(Id, Not success, startArgs.Cancel, _edbModel))
                End Try
                Return CType(resultAdapter.CurrentResult, KblConversionResult)
            End Using
        End Function

        Private Sub CreateEdbObjects(cancel As System.Threading.CancellationToken)
            'HINT: we do not need to create the other objects because the wire/core-creator itself creates all connected objects by itself
            Dim createWiresEntries As List(Of DictionaryEntry) = GetObjectsToCreate(Of Wire_occurrence)()
            Dim createCoresEntries As List(Of DictionaryEntry) = GetObjectsToCreate(Of Core_occurrence)()
            Dim createCablesEntries As List(Of DictionaryEntry) = GetObjectsToCreate(Of Special_wire_occurrence)()

            Dim totalCreateCount As Integer = createCoresEntries.Count + createWiresEntries.Count + createCoresEntries.Count + createCablesEntries.Count
            Dim totalCurrent As Integer = 0
            Dim currentPercent As Integer

            Dim afterEntityAdded As Action(Of List(Of EdbConversionEntity)) =
                Sub(currentList As List(Of EdbConversionEntity))
                    totalCurrent += 1
                    Dim percent As Integer = CInt((totalCurrent * 100) / totalCreateCount)
                    If currentPercent <> percent Then
                        currentPercent = percent
                        OnPercentChanged(currentPercent)
                    End If
                End Sub

            CreateEdbObjectsFromBlockEntries(createWiresEntries, afterEntityAdded, cancel)
            CreateEdbObjectsFromBlockEntries(createCoresEntries, afterEntityAdded, cancel)
            CreateEdbObjectsFromBlockEntries(createCablesEntries, afterEntityAdded, cancel)
        End Sub

        Private Sub Init()
            'HINT: created before we clear the old stuff (generation was re-started)
            _inlinerCavityMap.Clear()
            _inlinerConnectorMap.Clear()

            _componentTypesResolved.Clear()
            _currentConvertedItems.Clear()
            _edbIdsResolved.Clear()

            If _edbModel IsNot Nothing Then
                _edbModel.Dispose() 'HINT dispose is allowed here because we have created this model only internally
            End If

            _edbModel = New Connectivity.Model.EdbModel
        End Sub

        Public Property DefaultComponentType As Connectivity.Model.ComponentType

        ''' <summary>
        ''' This gived the unique id of this converter to distinct between multiple KblEdbConverters
        ''' </summary>
        ''' <returns></returns>
        Protected Friend ReadOnly Property Id As Guid = Guid.NewGuid Implements IKblConverter.Id

        Private Sub CreateConnectorInfoAndConnectorComponentMapper(Optional cancel As Threading.CancellationToken = Nothing)
            _edbConnectorInfoMapper.Clear()

            Dim componentsByNames As New Dictionary(Of String, ComponentInfo)
            For Each grp As ObjectGroup(Of IKblOccurrence) In _combinedMappers.Occurrences
                If cancel <> Threading.CancellationToken.None AndAlso cancel.CanBeCanceled Then
                    cancel.ThrowIfCancellationRequested()
                End If

                For Each documentConnEntry As KeyValuePair(Of String, IKblOccurrence) In grp
                    If cancel <> Threading.CancellationToken.None AndAlso cancel.CanBeCanceled Then
                        cancel.ThrowIfCancellationRequested()
                    End If

                    If TypeOf documentConnEntry.Value Is Connector_occurrence OrElse TypeOf documentConnEntry.Value Is Component_box_connector_occurrence Then
                        Dim connector As ConnectorInfo
                        If TypeOf documentConnEntry.Value Is Connector_occurrence Then
                            connector = OnConnectorResolving(CreateNewConnectorInfo(documentConnEntry.Key, CType(documentConnEntry.Value, Connector_occurrence)))
                        Else
                            connector = CreateNewConnectorInfo(documentConnEntry.Key, CType(documentConnEntry.Value, Component_box_connector_occurrence))
                        End If

                        With connector
                            If String.IsNullOrEmpty(.Component.ShortName) Then
                                .Component.ShortName = GetNewComponentShortName()
                            End If
                            .Component = componentsByNames.AddOrGet(.Component.ShortName, .Component) 'HINT: override with existing componentInfo when the component with the same name already was created (adds also the connector to it)
                            TryAddToResolveBuffer(.Component.ShortName, If(.Component.Type.HasValue, .Component.Type.Value, ResolveComponentTypeInternal(.Component.ShortName)))
                            _edbConnectorInfoMapper.Add(connector)
                        End With
                    End If
                Next
            Next
        End Sub

        Private Function CreateEdbObjects(Of T As IKblOccurrence)(Optional filterObject As Func(Of String, T, Boolean) = Nothing, Optional afterEntityAdded As Action(Of List(Of EdbConversionEntity)) = Nothing, Optional cancel As Threading.CancellationToken = Nothing) As List(Of EdbConversionEntity)
            Return CreateEdbObjectsFromBlockEntries(GetObjectsToCreate(Of T)(filterObject), afterEntityAdded, cancel)
        End Function

        Private Function CreateEdbObjectsFromBlockEntries(blockEntries As IEnumerable(Of DictionaryEntry), Optional afterEntitiesAdded As Action(Of List(Of EdbConversionEntity)) = Nothing, Optional cancel As Threading.CancellationToken = Nothing) As List(Of EdbConversionEntity)
            Dim list As New List(Of EdbConversionEntity)
            For Each blockandObj As DictionaryEntry In blockEntries
                If cancel <> Threading.CancellationToken.None AndAlso cancel.CanBeCanceled Then
                    cancel.ThrowIfCancellationRequested()
                End If
                Dim items As EdbConversionEntity = AddNewOrGetEdbObjectUntyped(blockandObj.Key.ToString, blockandObj.Value)
                If items IsNot Nothing Then
                    list.Add(items)
                    If afterEntitiesAdded IsNot Nothing Then
                        afterEntitiesAdded.Invoke(list)
                    End If
                End If
            Next
            Return list
        End Function

        Private Function GetObjectsToCreate(Of T As IKblOccurrence)(Optional filterObject As Func(Of String, T, Boolean) = Nothing) As List(Of DictionaryEntry)
            Dim list As New List(Of DictionaryEntry)
            For Each occBlockGroup As IObjectGroup In _combinedMappers.Occurrences.OfObjectType(Of T)()
                For Each blockAndObj As DictionaryEntry In occBlockGroup
                    If filterObject IsNot Nothing AndAlso filterObject.Invoke(blockAndObj.Key.ToString, CType(blockAndObj.Value, T)) Then
                        Continue For
                    Else
                        list.Add(blockAndObj)
                    End If
                Next
            Next
            Return list
        End Function

#Region "Inliner"

        Private Function TryGetInlinerId(documentId As String, cavityShortName As String, connector As Connector_occurrence, ByRef inlinerId As String) As Boolean
            Dim connInlinerId As String = Nothing
            If TryGetInlinerId(documentId, connector, connInlinerId) Then
                inlinerId = IdConverter.GetCombined(connInlinerId, cavityShortName)
                Return True
            End If
            Return False
        End Function

        Private Function TryGetInlinerId(documentId As String, connector As Connector_occurrence, ByRef inlinerId As String) As Boolean
            Dim comp As EdbConversionComponentEntity = Me.GetOrCreateComponent(documentId, connector)
            If comp IsNot Nothing Then
                Return TryGetInlinerId(documentId, GetEdbSystemIdInternal(documentId, connector), GetSystemId(connector), ResolveShortName(connector, documentId), comp.ComponentType, comp.Id, inlinerId)
            End If
            Return False
        End Function

        Private Function TryGetInlinerId(documentId As String, connectorEdbSysId As String, originalId As String, shortName As String, componentType As Connectivity.Model.ComponentType, componentEdbId As String, ByRef inlinerId As String) As Boolean
            Select Case componentType
                Case Connectivity.Model.ComponentType.Inliner
                    Dim connInfo As ConnectorInfo = TryGetConnectorInfo(connectorEdbSysId)
                    If connInfo Is Nothing Then
                        connInfo = New ConnectorInfo(documentId, connectorEdbSysId, originalId, shortName, String.Empty)
                    End If
                    With connInfo
                        inlinerId = IdConverter.GetCombined(componentEdbId, If(Not String.IsNullOrEmpty(.InlinerName), .InlinerName, .ShortName))
                    End With
                    Return True
            End Select
            Return False
        End Function

#End Region

#Region "ResolveShortName"

        Private Function ResolveShortName(cmpBox As Component_box_occurrence) As String
            Return cmpBox.Id
        End Function

        Private Function ResolveShortName(conn As Connection) As String
            Return conn.Signal_name
        End Function

        Private Function ResolveShortName([module] As [Module]) As String
            Return [module].Abbreviation
        End Function

        Private Function ResolveShortName(wireGroup As Wiring_group) As String
            Return wireGroup.Id
        End Function

        Private Function ResolveShortName(core As Core_occurrence) As String
            Return core.Wire_number
        End Function

        Private Function ResolveShortName(wire As Wire_occurrence) As String
            Return wire.Wire_number
        End Function

        Private Function ResolveShortName(cable As Special_wire_occurrence) As String
            Return cable.Special_wire_id
        End Function

        Private Function ResolveShortName(component As Component_occurrence, documentId As String, Optional connector As Connector_occurrence = Nothing) As String
            Dim componentShortName As String = component.Id
            If connector IsNot Nothing Then
                Dim componentType As Connectivity.Model.ComponentType = ResolveComponentTypeInternal(componentShortName)
                Select Case componentType
                    Case Connectivity.Model.ComponentType.Inliner
                        TryGetInlinerId(documentId, connector, componentShortName)
                    Case Connectivity.Model.ComponentType.Eyelet, Connectivity.Model.ComponentType.Splice
                        componentShortName = ResolveShortName(connector, documentId)
                End Select
            End If

            Return componentShortName
        End Function

        Private Function ResolveShortName(cavity As Cavity_occurrence, documentId As String) As String
            Dim cavityShortName As String = String.Empty
            If Not IsSpliceOrEyelet(documentId, cavity) Then
                Dim cavPart As Cavity = TryGetPart(Of Cavity)(documentId, cavity.Part)
                cavityShortName = cavPart.Cavity_number
            End If
            Return cavityShortName
        End Function

        Private Function ResolveShortName(connector As Component_box_connector_occurrence, documentId As String) As String
            Dim connInfo As ConnectorInfo = TryGetConnectorInfo(documentId, connector)
            If connInfo IsNot Nothing Then
                Return connInfo.ShortName
            End If

            Dim cmpBoxConn As Component_box_connector = TryGetPart(Of Component_box_connector)(documentId, connector.Part)
            If cmpBoxConn IsNot Nothing Then
                Return cmpBoxConn.Id
            End If
            Return String.Empty
        End Function

        Private Function ResolveShortName(connector As Connector_occurrence, documentId As String) As String
            Dim connInfo As ConnectorInfo = TryGetConnectorInfo(documentId, connector)
            If connInfo IsNot Nothing Then
                Return connInfo.ShortName
            End If
            Return connector.Id
        End Function

#End Region

        Private Function TryGetConnectorInfo(documentId As String, connector As Component_box_connector_occurrence) As ConnectorInfo
            Return TryGetConnectorInfo(GetEdbSystemIdInternal(documentId, connector))
        End Function

        Private Function TryGetConnectorInfo(documentId As String, connector As Connector_occurrence) As ConnectorInfo
            Return TryGetConnectorInfo(GetEdbSystemIdInternal(documentId, connector))
        End Function

        Private Function TryGetConnectorInfo(edbConnectorSysId As String) As ConnectorInfo
            Dim connInfo As ConnectorInfo = Nothing
            Me._edbConnectorInfoMapper.TryGetValue(edbConnectorSysId, connInfo)
            Return connInfo
        End Function

#Region "ResolveComponentType"

        Private Function ResolveComponentTypeInternal(edbSysId As String, componentShortName As String, documentId As String) As Connectivity.Model.ComponentType
            Dim convComp As EdbConversionComponentEntity = _currentConvertedItems.TryGetDocumentEntity(Of EdbConversionComponentEntity)(documentId, edbSysId)
            If convComp IsNot Nothing Then
                Return convComp.ComponentType
            Else
                Return ResolveComponentTypeInternal(componentShortName)
            End If
        End Function

        Private Function ResolveComponentTypeInternal(kblComponent As Component_box_occurrence, documentId As String) As Connectivity.Model.ComponentType
            Return ResolveComponentTypeInternal(GetEdbSystemIdInternal(documentId, kblComponent), ResolveShortName(kblComponent), documentId)
        End Function

        Private Function ResolveComponentTypeInternal(kblComponent As Component_occurrence, documentId As String) As Connectivity.Model.ComponentType
            Return ResolveComponentTypeInternal(GetEdbSystemIdInternal(documentId, kblComponent), ResolveShortName(kblComponent, documentId), documentId)
        End Function

        Private Function ResolveComponentTypeInternal(componentShortName As String) As Connectivity.Model.ComponentType
            Dim type As Connectivity.Model.ComponentType
            If Not _componentTypesResolved.TryGetValue(componentShortName, type) Then
                Dim args As New ComponentTypeResolveEventArgs(componentShortName, Me.Id)
                OnResolveComponentType(args)
                type = args.ComponentType.GetValueOrDefault(Me.DefaultComponentType)
                TryAddToResolveBuffer(componentShortName, type)
            End If
            Return type
        End Function

#End Region

        Protected Overridable Sub OnResolveComponentType(e As ComponentTypeResolveEventArgs)
            RaiseEvent ResolveComponentType(Me, e)
        End Sub

        Private Function TryAddToResolveBuffer(componentShortName As String, type As Connectivity.Model.ComponentType) As Boolean
            Return _componentTypesResolved.TryAdd(componentShortName, type)
        End Function

        Private Sub SetModules(edbWire As EdbConversionWireEntity, moduleIds As IEnumerable(Of Tuple(Of String, String)))
            edbWire.Modules.Clear()
            edbWire.Modules.AddRange(moduleIds.Select(Function(tpl) tpl.Item1))
            OnAfterModuleSet(New ModuleSetEventArgs(moduleIds.Select(Function(tpl) New ModuleInfo(tpl.Item1, tpl.Item2)), edbWire.EdbItem.SysId))
        End Sub

        Protected Overridable Sub OnAfterModuleSet(e As ModuleSetEventArgs)
            RaiseEvent AfterModulesSet(Me, e)
        End Sub

        Private Sub SetFunction(edbWire As EdbConversionWireEntity, functionIdAndName As Tuple(Of String, String))
            edbWire.Function = functionIdAndName.Item1
            OnAfterFunctionSet(New FunctionSetEventArgs(functionIdAndName.Item1, edbWire.EdbItem.SysId, functionIdAndName.Item2))
        End Sub

        Protected Overridable Sub OnAfterFunctionSet(e As FunctionSetEventArgs)
            RaiseEvent AfterFunctionSet(Me, e)
        End Sub

        Private Function GetCableType(documentId As String, wireGroup As Wiring_group) As Connectivity.Model.CableType
            Return Connectivity.Model.CableType.Twisted
        End Function

        Private Function GetCableType(documentId As String, cable As Special_wire_occurrence) As Connectivity.Model.CableType
            Return Connectivity.Model.CableType.Undefined
        End Function

#Region "GetSpecifier"

        Private Function GetSpecifier(documentId As String, wire As Wire_occurrence) As String
            Return GetSpecifier(TryGetPart(Of General_wire)(documentId, wire.Part))
        End Function

        Private Function GetSpecifier(documentId As String, core As Core_occurrence) As String
            Return GetSpecifier(TryGetPart(Of Core)(documentId, core.Part))
        End Function

        'HINT: the specifier is currently the PartNumber (maybe changed later)
        Private Function GetSpecifier(genWire As General_wire) As String
            Return If(genWire IsNot Nothing, genWire.Part_number, String.Empty)
        End Function

        'HINT: the specifier is currently the id (maybe changed later)
        Private Function GetSpecifier(core As Core) As String
            Return If(core IsNot Nothing, core.Id, String.Empty)
        End Function

#End Region

        Private Function TryGetPart(Of T)(documentId As String, partId As String) As T
            Dim partObj As Object = _combinedMappers.Parts.TryGetDocumentObject(partId, documentId)
            If partObj IsNot Nothing Then
                Return CType(partObj, T)
            End If
            Return Nothing
        End Function

#Region "GetSqMmCsa"

        Private Function GetSqMmCsa(documentId As String, wire As Wire_occurrence) As Double
            Return GetSqMmCsa(documentId, TryGetPart(Of General_wire)(documentId, wire.Part))
        End Function

        Private Function GetSqMmCsa(documentId As String, core As Core_occurrence) As Double
            Return GetSqMmCsa(documentId, TryGetPart(Of Core)(documentId, core.Part))
        End Function

        Private Function GetSqMmCsa(documentId As String, generalWire As General_wire) As Double
            Return If(generalWire IsNot Nothing, GetSqMm(documentId, generalWire.Cross_section_area), 0)
        End Function

        Private Function GetSqMmCsa(documentId As String, core As Core) As Double
            Return If(core IsNot Nothing, GetSqMm(documentId, core.Cross_section_area), 0)
        End Function

#End Region

        Private Function GetSqMm(documentId As String, nv As Numerical_value) As Double
            Dim unit As Unit = Nothing
            If nv IsNot Nothing AndAlso _combinedMappers.Units.TryGetDocumentObject(nv.Unit_component, documentId, unit) Then
                Dim unitInfo As Units.KblPlainUnit = KblUnitConverter.GetUnitInfo(unit)
                Dim calcUnit As CalcUnit = CalcUnit.CreateFrom(unitInfo)
                If Not calcUnit.IsEmpty Then
                    Dim value As Nullable(Of Double) = UnitConverter.Convert(nv.Value_component, calcUnit, CalcUnit.SquareMMetre)
                    If value.HasValue Then
                        Return value.Value
                    End If
                End If
            End If
            Return 0
        End Function

#Region "GetFunctionId"

        Private Function GetFunctionIdAndName(documentId As String, wire As Wire_occurrence) As Tuple(Of String, String)
            Dim conn As Connection = TryGetConnection(documentId, GetSystemId(wire))
            If conn IsNot Nothing Then
                Return New Tuple(Of String, String)(GetEdbIdInternal(documentId, conn), conn.Signal_name)
            End If
            Return New Tuple(Of String, String)(String.Empty, String.Empty)
        End Function

        Private Function GetFunctionIdAndName(documentId As String, core As Core_occurrence) As Tuple(Of String, String)
            Dim conn As Connection = TryGetConnection(documentId, GetSystemId(core))
            If conn IsNot Nothing Then
                Return New Tuple(Of String, String)(GetEdbIdInternal(documentId, conn), ResolveShortName(conn))
            End If
            Return New Tuple(Of String, String)(String.Empty, String.Empty)
        End Function

        Private Function TryGetConnection(documentId As String, systemId As String) As [Lib].Schema.Kbl.Connection
            Dim wireConnection As [Lib].Schema.Kbl.Connection = _combinedMappers.WireConnections.TryGetDocumentObject(systemId, documentId)
            If wireConnection IsNot Nothing Then
                Return wireConnection
            End If
            Return Nothing
        End Function

#End Region

#Region "GetEdbModuleIds"

        Private Function GetEdbModuleIdsAndName(documentId As String, wire As Wire_occurrence) As List(Of Tuple(Of String, String))
            Return TryGetModuleEdbIdsAndName(documentId, GetSystemId(wire))
        End Function

        Private Function GetEdbModuleIdsAndName(documentId As String, core As Core_occurrence) As List(Of Tuple(Of String, String))
            Return TryGetModuleEdbIdsAndName(documentId, GetSystemId(core))
        End Function

        Private Function TryGetModuleEdbIdsAndName(documentId As String, systemId As String) As List(Of Tuple(Of String, String))
            Dim resModules As List(Of [Lib].Schema.Kbl.Module) = _combinedMappers.ObjectToModuleMapper.TryGetDocumentObject(systemId, documentId)
            If resModules IsNot Nothing Then
                Return resModules.Select(Function(md) New Tuple(Of String, String)(GetEdbIdInternal(documentId, GetSystemId(md)), ResolveShortName(md))).ToList
            End If
            Return New List(Of Tuple(Of String, String))
        End Function

#End Region

#Region "AddNewOrGetEdbObject"

        Private Function AddNewOrGetEdbObject(documentId As String, connector As Component_box_connector_occurrence) As EdbConversionConnectorEntity
            Return CType(AddNewOrGetEdbObjectUntyped(documentId, connector), EdbConversionConnectorEntity)
        End Function

        Private Function AddNewOrGetEdbObject(documentId As String, connector As Connector_occurrence) As EdbConversionConnectorEntity
            Return CType(AddNewOrGetEdbObjectUntyped(documentId, connector), EdbConversionConnectorEntity)
        End Function

        Private Function AddNewOrGetEdbObject(documentId As String, comp As Component_occurrence) As EdbConversionComponentEntity
            Return CType(AddNewOrGetEdbObjectUntyped(documentId, comp), EdbConversionComponentEntity)
        End Function

        Private Function AddNewOrGetEdbObject(documentId As String, comp As Component_box_occurrence) As EdbConversionComponentEntity
            Return CType(AddNewOrGetEdbObjectUntyped(documentId, comp), EdbConversionComponentEntity)
        End Function

        Private Function AddNewOrGetEdbObject(documentId As String, wire As Wiring_group) As EdbConversionCableEntity
            Return CType(AddNewOrGetEdbObjectUntyped(documentId, wire), EdbConversionCableEntity)
        End Function

        ''' <summary>
        ''' Creates or returns a new ConnectivityView Wire or cable if the wire belongs to a wiring_group (Twisting cable)
        ''' </summary>
        ''' <param name="documentId"></param>
        ''' <param name="wire"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AddNewOrGetEdbObject(documentId As String, wire As Wire_occurrence) As EdbConversionEntity
            Return AddNewOrGetEdbObjectUntyped(documentId, wire)
        End Function

        Private Function AddNewOrGetEdbObject(documentId As String, core As Core_occurrence) As EdbConversionWireEntity
            Return CType(AddNewOrGetEdbObjectUntyped(documentId, core), EdbConversionWireEntity)
        End Function

        Private Function AddNewOrGetEdbObject(documentId As String, cav As Cavity_occurrence) As EdbConversionCavityEntity
            Return CType(AddNewOrGetEdbObjectUntyped(documentId, cav), EdbConversionCavityEntity)
        End Function

        Private Function AddNewOrGetEdbObject(documentId As String, wire As Special_wire_occurrence) As EdbConversionCableEntity
            Return CType(AddNewOrGetEdbObjectUntyped(documentId, wire), EdbConversionCableEntity)
        End Function

        Private Function AddNewOrGetEdbObjectUntyped(documentId As String, obj As Object) As EdbConversionEntity
            Dim newEntity As EdbConversionEntity = _currentConvertedItems.TryGetDocumentEntity(documentId, GetEdbSystemIdInternalUntyped(documentId, obj))
            If newEntity Is Nothing Then
                newEntity = _addNewOrGetObjectGetter.GetAndInvokeMethod(Of EdbConversionEntity)(Me, documentId, obj)
                If newEntity IsNot Nothing Then
                    TryRegisterEntityAndRaiseCreated(documentId, newEntity)
                End If
            End If
            Return newEntity
        End Function

#End Region

        Private Function TryRegisterEntityAndRaiseCreated(documentId As String, newEntity As EdbConversionEntity) As Boolean
            If RegisterConvertedEntity(documentId, newEntity) Then
                Me.OnAfterEntityCreated(newEntity)
                Return True
            End If
            Return False
        End Function

        Private Function RegisterConvertedEntity(documentId As String, newEntity As EdbConversionEntity) As Boolean
            If newEntity IsNot Nothing Then
                Dim entitiesOfBlock As Converter.EdbConversionDocument = _currentConvertedItems.AddNewOrGet(documentId)
                Return entitiesOfBlock.TryAdd(newEntity)
            End If
            Return False
        End Function

#Region "GetOrCreateCavities"

        Private Function GetOrCreateCavitites(documentId As String, core As Core_occurrence) As List(Of EdbConversionCavityEntity) ' circular POSSIBLE when using within cavity creation
            Return GetOrCreateCavitiesOfWire(documentId, GetSystemId(core))
        End Function

        Private Function GetOrCreateCavitites(documentId As String, wire As Wire_occurrence) As List(Of EdbConversionCavityEntity) ' circular POSSIBLE when using within cavity creation
            Return GetOrCreateCavitiesOfWire(documentId, GetSystemId(wire))
        End Function

        Private Function GetOrCreateCavitiesOfWire(documentId As String, wireOriginalId As String) As List(Of EdbConversionCavityEntity)
            Dim edbCavities As New HashSet(Of EdbConversionCavityEntity) ' block double cavity entities (the same objects but with a different occurrence) (can be now happening that we have the samen entity for the wire because of the partId-grouping-together-mapping)
            Dim kblCavities As List(Of Cavity_occurrence) = _combinedMappers.CavitiesOfWire.TryGetDocumentObject(wireOriginalId, documentId)
            If kblCavities IsNot Nothing Then
                For Each cav As Cavity_occurrence In kblCavities
                    Dim newEdbCav As EdbConversionCavityEntity = AddNewOrGetEdbObject(documentId, cav)
                    If newEdbCav IsNot Nothing Then
                        edbCavities.Add(newEdbCav)
                    End If
                Next
            End If
            Return edbCavities.ToList
        End Function

#End Region

        Private Function IsSpliceOrEyelet(documentId As String, cav As Cavity_occurrence) As Boolean
            Dim comp As EdbConversionComponentEntity = TryGetOrCreateComponent(documentId, cav)
            If comp IsNot Nothing Then
                Return comp.ComponentType = Connectivity.Model.ComponentType.Splice Or comp.ComponentType = Connectivity.Model.ComponentType.Eyelet
            End If
            Return False
        End Function

#Region "GetOrCreate"

        Private Function TryGetOrCreateConnector(documentId As String, cavity As Cavity_occurrence, Optional ByRef connOccFound As Connector_occurrence = Nothing) As EdbConversionConnectorEntity
            If _combinedMappers.ConnectorsOfCavity.TryGetDocumentObject(GetSystemId(cavity), documentId, connOccFound) Then
                Return AddNewOrGetEdbObject(documentId, connOccFound)
            End If
            Return Nothing
        End Function

        Private Function GetOrCreateConnector(documentId As String, cavity As Cavity_occurrence, Optional ByRef connOccFound As Connector_occurrence = Nothing) As EdbConversionConnectorEntity
            Dim connEntity As EdbConversionConnectorEntity = TryGetOrCreateConnector(documentId, cavity, connOccFound)
            If connEntity Is Nothing Then
                Dim cmpBoxConn As Component_box_connector_occurrence = _combinedMappers.CavityToComponentBoxConnector.TryGetDocumentObject(GetSystemId(cavity), documentId)
                If cmpBoxConn IsNot Nothing Then
                    Return AddNewOrGetEdbObject(documentId, cmpBoxConn)
                End If
                Throw New NotSupportedException(String.Format("Cavity ({0}) without a mapped connector is not supported!", GetSystemId(cavity)))
            End If
            Return connEntity
        End Function

        Private Function TryGetOrCreateComponent(documentId As String, cavity As Cavity_occurrence, Optional ByRef connResolved As Connector_occurrence = Nothing) As EdbConversionComponentEntity
            TryGetOrCreateConnector(documentId, cavity, connResolved)
            If connResolved IsNot Nothing Then
                Return GetOrCreateComponent(documentId, connResolved)
            End If
            Return Nothing
        End Function

        Private Function GetOrCreateComponent(documentId As String, connEdbSysId As String, ByRef componentInfo As ComponentInfo) As EdbConversionComponentEntity
            Dim compEntity As EdbConversionEntity = GetComponentFromConnector(connEdbSysId, componentInfo)
            If componentInfo IsNot Nothing AndAlso compEntity Is Nothing Then
                With componentInfo
                    Dim compOcc As Object = _combinedMappers.Occurrences.TryGetDocumentObject(.OriginalIds.SingleOrDefault, .DocumentId)
                    If compOcc IsNot Nothing Then
                        If TypeOf compOcc Is Component_occurrence Then
                            compEntity = _currentConvertedItems(.DocumentId).Item(AddNewOrGetEdbObject(.DocumentId, CType(compOcc, Component_occurrence)).Id)
                        Else
                            compEntity = _currentConvertedItems(.DocumentId).Item(AddNewOrGetEdbObject(.DocumentId, CType(compOcc, Component_box_occurrence)).Id)
                        End If
                    Else
                        compEntity = AddNewEdbComponent(componentInfo)
                        CType(compEntity, EdbConversionComponentEntity).IsVirtual = True
                    End If
                End With
            End If
            Return CType(compEntity, EdbConversionComponentEntity)
        End Function

        Private Function GetOrCreateComponent(documentId As String, conn As Component_box_connector_occurrence) As EdbConversionComponentEntity
            Return GetOrCreateComponent(documentId, GetEdbSystemIdInternal(documentId, conn), Nothing)
        End Function

        Private Function GetOrCreateComponent(documentId As String, conn As Connector_occurrence) As EdbConversionComponentEntity
            Return GetOrCreateComponent(documentId, GetEdbSystemIdInternal(documentId, conn), Nothing)
        End Function

        Private Function GetOrCreateWiresCables(documentId As String, cavity As Cavity_occurrence) As List(Of EdbConversionEntity)
            Dim edbWiresCables As New List(Of EdbConversionEntity)
            For Each wire As Object In GetWiresCablesOccurrences(documentId, cavity)
                If wire IsNot Nothing Then
                    If TypeOf wire Is Wire_occurrence Then
                        edbWiresCables.Add(AddNewOrGetEdbObject(documentId, CType(wire, Wire_occurrence)))
                    ElseIf TypeOf wire Is Core_occurrence Then
                        edbWiresCables.Add(AddNewOrGetEdbObject(documentId, CType(wire, Core_occurrence)))
                    ElseIf TypeOf wire Is Special_wire_occurrence Then
                        edbWiresCables.Add(AddNewOrGetEdbObject(documentId, CType(wire, Special_wire_occurrence)))
                    Else
                        Throw New NotImplementedException(String.Format("Unimplemented wire type ""{0}""", wire.GetType.Name))
                    End If
                End If
            Next
            Return edbWiresCables
        End Function

        Private Function GetOrCreateCavityGroup(documentId As String, cavity As Cavity_occurrence) As EdbConversionCavityGroupEntity
            Dim connector As EdbConversionConnectorEntity = Me.GetOrCreateConnector(documentId, cavity)
            If connector IsNot Nothing Then
                Dim edbCavGroupEdbId As String = GetEdbIdInternal(documentId, GetSystemId(cavity))
                Dim groupEntity As EdbConversionCavityGroupEntity = _currentConvertedItems.TryGetDocumentEntity(Of EdbConversionCavityGroupEntity)(documentId, edbCavGroupEdbId)
                If groupEntity Is Nothing Then
                    Dim cavityOcc As Cavity_occurrence = _combinedMappers.Occurrences.TryGetObject(Of Cavity_occurrence)(GetSystemId(cavity), documentId)
                    groupEntity = AddNewEdbCavityInternal(Of EdbConversionCavityGroupEntity)(documentId, edbCavGroupEdbId, GetSystemIdOrGroupedByPart(documentId, cavity), ResolveShortName(cavityOcc, documentId), connector, False)   'HINT: create a new cavity with a new id (Id + [current cavityNr + 1]) from the current cavity but it shall not have a edb cavity in the model (this is only some sort of space holder cavity that points to the virtual cavities)
                End If
                Return groupEntity
            End If
            Return Nothing
        End Function

#End Region

        Private Function GetWiresCablesOccurrences(documentId As String, cavity As Cavity_occurrence) As List(Of IKblOccurrence)
            Dim wiresCablesOccs As List(Of IKblOccurrence) = _combinedMappers.WiresOfCavityMapper.TryGetDocumentObject(GetSystemId(cavity), documentId)
            If wiresCablesOccs IsNot Nothing Then
                Return wiresCablesOccs
            End If
            Return New List(Of IKblOccurrence)
        End Function

        Private Function GetComponentFromConnector(connectorEdbSysId As String, Optional ByRef componentInfo As ComponentInfo = Nothing) As EdbConversionComponentEntity
            Dim connInfo As ConnectorInfo = Nothing
            If _edbConnectorInfoMapper.TryGetValue(connectorEdbSysId, connInfo) Then
                componentInfo = connInfo.Component
                With componentInfo
                    Return _currentConvertedItems.TryGetDocumentEntity(Of EdbConversionComponentEntity)(.DocumentId, .Id)
                End With
            End If
            Return Nothing
        End Function

        Private Function GetNewComponentShortName() As String
            _dummyComponentCount += 1
            Return String.Format("Component_{0}", _dummyComponentCount)
        End Function

        Private Function CreateNewConnectorInfo(blockId As String, connector As Connector_occurrence, Optional inlinerName As String = "") As ConnectorInfo
            Dim connInfo As New ConnectorInfo(blockId, GetEdbSystemIdInternal(blockId, connector), GetSystemId(connector), ResolveShortName(connector, blockId), inlinerName)

            If connector.UsageSpecified Then
                Select Case connector.Usage
                    Case Connector_usage.ringterminal
                        connInfo.IsEyelet = True
                    Case Connector_usage.splice
                        connInfo.IsSplice = True
                End Select

            End If
            connInfo.Description = connector.Description
            If connector.Alias_id IsNot Nothing Then
                For Each aliasIdent As Alias_identification In connector.Alias_id
                    If Not String.IsNullOrEmpty(aliasIdent.Alias_id) Then
                        connInfo.AliasIds.Add(aliasIdent.Alias_id)
                    End If
                Next
            End If
            If connector.Installation_information IsNot Nothing Then
                For Each installationIntr As Installation_instruction In connector.Installation_information
                    If Not String.IsNullOrEmpty(installationIntr.Instruction_value) Then
                        connInfo.InstallationInstructions.Add(installationIntr.Instruction_value)
                    End If
                Next
            End If
            Return connInfo
        End Function

        Private Function CreateNewConnectorInfo(documentId As String, connector As Component_box_connector_occurrence, Optional inlinerName As String = "") As ConnectorInfo
            Dim connInfo As New ConnectorInfo(documentId, GetEdbSystemIdInternal(documentId, connector), GetSystemId(connector), ResolveShortName(connector, documentId), inlinerName)
            Dim cmpBox As Component_box_occurrence = _combinedMappers.ComponentBoxConnectorsToComponentBox.TryGetDocumentObject(connInfo.OriginalIds.SingleOrDefault, documentId)
            If cmpBox IsNot Nothing Then
                Dim boxSysId As String = GetSystemId(cmpBox)
                connInfo.Component = New ComponentInfo(GetEdbIdInternal(documentId, boxSysId), boxSysId, ResolveShortName(cmpBox), ResolveComponentTypeInternal(cmpBox, documentId))
            End If
            Return connInfo
        End Function

        Private Function OnConnectorResolving(connectorInfo As ConnectorInfo) As ConnectorInfo
            Dim args As New ConnectorResolvingEventArgs(Me.Id, connectorInfo)
            OnConnectorResolving(args)
            Return args.Connector
        End Function

        Protected Overridable Sub OnConnectorResolving(e As ConnectorResolvingEventArgs)
            RaiseEvent ConnectorResolving(Me, e)
            RaiseEvent IKblConverter_ConnectorResolving(Me, e)
        End Sub

        Private Sub OnAfterEntityCreated(convertedEntity As EdbConversionEntity)
            RaiseEvent AfterEntityCreated(Me, New EntityEventArgs(Me.Id, convertedEntity))
        End Sub

        Protected Overridable Sub OnBeforeConversionStart(e As CancelEventArgs)
            RaiseEvent IKblConverter_BeforeConversionStart(Me, e)
        End Sub

        Protected Overridable Sub OnAfterConversion(e As FinishedEventArgs)
            RaiseEvent IKblConverter_AfterConversionFinished(Me, e)
        End Sub

        Protected Overridable Sub OnPercentChanged(e As System.ComponentModel.ProgressChangedEventArgs)
            RaiseEvent ProgressChanged(Me, e)
        End Sub

        Private Sub OnPercentChanged(percent As Integer)
            OnPercentChanged(New System.ComponentModel.ProgressChangedEventArgs(percent, Nothing))
        End Sub

#Region "GetEdbIdInternals"

        Private Function GetEdbIdInternal(documentId As String, objectId As String) As String
            Dim objectToEdbIdMap As Dictionary(Of String, String) = _edbIdsResolved.AddOrGet(documentId, New Dictionary(Of String, String))
            Dim edbId As String = Nothing
            If Not objectToEdbIdMap.TryGetValue(objectId, edbId) Then
                Dim args As New Converter.IdEventArgs(Id, documentId, objectId)
                RaiseEvent ResolveEntityId(Me, args)
                If String.IsNullOrEmpty(args.EdbId) Then
                    edbId = IdConverter.GetDefaultEdbId(documentId, objectId)
                Else
                    edbId = args.EdbId
                End If
                objectToEdbIdMap.Add(objectId, edbId)
            End If
            Return edbId
        End Function

        Private Function GetEdbIdInternal(documentId As String, connection As Connection) As String
            If connection IsNot Nothing Then
                Return GetEdbIdInternal(documentId, If(Not String.IsNullOrEmpty(connection.Signal_name), connection.Signal_name, connection.Wire))
            End If
            Return String.Empty
        End Function

        <GetEdbSystemIdInternalMethodAttribute>
        Private Function GetEdbSystemIdInternal(documentId As String, conn As Component_box_connector_occurrence) As String
            If conn IsNot Nothing Then
                Return GetEdbIdInternal(documentId, GetSystemId(conn))
            End If
            Return String.Empty
        End Function

        <GetEdbSystemIdInternalMethodAttribute>
        Private Function GetEdbSystemIdInternal(documentId As String, cmp As Component_box_occurrence) As String
            If cmp IsNot Nothing Then
                Return GetEdbIdInternal(documentId, GetSystemId(cmp))
            End If
            Return String.Empty
        End Function

        <GetEdbSystemIdInternalMethodAttribute>
        Private Function GetEdbSystemIdInternal(documentId As String, conn As Connector_occurrence) As String
            If conn IsNot Nothing Then
                Return GetEdbIdInternal(documentId, GetSystemId(conn))
            End If
            Return String.Empty
        End Function

        <GetEdbSystemIdInternalMethodAttribute>
        Private Function GetEdbSystemIdInternal(documentId As String, cav As Cavity_occurrence) As String
            If IsSpliceOrEyelet(documentId, cav) Then
                Dim cavGroup As EdbConversionCavityGroupEntity = GetOrCreateCavityGroup(documentId, cav)
                Return IdConverter.GetCombined(cavGroup.Id, (cavGroup.Count + 1).ToString)
            Else
                Dim cavCollection As InternalCavityPartGroup = _cavityPartsToOccurrencesMapper.GetCavitiesOfPart(documentId, cav)
                If cavCollection IsNot Nothing AndAlso cavCollection.Count > 1 Then
                    Return GetEdbIdInternal(documentId, IdConverter.GetCombined(cavCollection.Select(Function(cv) GetSystemId(cv)).ToArray))
                End If
                Return GetEdbIdInternal(documentId, GetSystemId(cav))
            End If
        End Function

        <GetEdbSystemIdInternalMethodAttribute>
        Private Function GetEdbSystemIdInternal(documentId As String, cmp As Component_occurrence) As String
            Return GetEdbIdInternal(documentId, GetSystemId(cmp))
        End Function

        <GetEdbSystemIdInternalMethodAttribute>
        Private Function GetEdbSystemIdInternal(documentId As String, wire As Wire_occurrence) As String
            Return GetEdbIdInternal(documentId, GetSystemId(wire))
        End Function

        <GetEdbSystemIdInternalMethodAttribute>
        Private Function GetEdbSystemIdInternal(documentId As String, wireGroup As Wiring_group) As String
            Return GetEdbIdInternal(documentId, GetSystemId(wireGroup))
        End Function

        <GetEdbSystemIdInternalMethodAttribute>
        Private Function GetEdbSystemIdInternal(documentId As String, cable As Special_wire_occurrence) As String
            Return GetEdbIdInternal(documentId, GetSystemId(cable))
        End Function

        <GetEdbSystemIdInternalMethodAttribute>
        Private Function GetEdbSystemIdInternal(documentId As String, core As Core_occurrence) As String
            Return GetEdbIdInternal(documentId, GetSystemId(core))
        End Function

        Private Function GetEdbSystemIdInternalUntyped(documentId As String, [object] As Object) As String
            Return CStr(_getEdbIdInternalGetter.GetMethod(documentId, [object]).Invoke(Me, New Object() {documentId, [object]}))
        End Function

#End Region

#Region "GetSystemID"

        Private Shared Function GetWireOrCoreSystemId(wireOrCore As Object) As String
            Dim wireOcc As Wire_occurrence = TryCast(wireOrCore, Wire_occurrence)
            Dim coreOcc As Core_occurrence = TryCast(wireOrCore, Core_occurrence)
            If wireOcc IsNot Nothing Then
                Return GetSystemId(wireOcc)
            ElseIf coreOcc IsNot Nothing Then
                Return GetSystemId(coreOcc)
            Else
                Throw New ArgumentException(String.Format("Given parameter object ""{0}"" is not of type ""{1}"" or ""{2}""!", wireOrCore.GetType.Name, GetType(Wire_occurrence).Name, GetType(Core_occurrence).Name), NameOf(wireOrCore))
            End If
        End Function

        Private Shared Function GetSystemId(cmp As Component_box_occurrence) As String
            Return cmp.SystemId
        End Function

        Private Shared Function GetSystemId(cmp As Component_occurrence) As String
            Return cmp.SystemId
        End Function

        Private Shared Function GetSystemId(wire As Wire_occurrence) As String
            Return wire.SystemId
        End Function

        Private Shared Function GetSystemId(wireGroup As Wiring_group) As String
            Return wireGroup.SystemId
        End Function

        Private Shared Function GetSystemId(cable As Special_wire_occurrence) As String
            Return cable.SystemId
        End Function

        Private Shared Function GetSystemId(core As Core_occurrence) As String
            Return core.SystemId
        End Function

        Private Shared Function GetSystemId(cavity As Cavity_occurrence) As String
            Return cavity.SystemId
        End Function

        ''' <summary>
        ''' Resolve cavity systemId normally or multiple system ids when cavities share the same part-cavity
        ''' </summary>
        ''' <param name="documentId"></param>
        ''' <param name="cavity"></param>
        ''' <returns></returns>
        Private Function GetSystemIdOrGroupedByPart(documentId As String, cavity As Cavity_occurrence) As String()
            Dim cavCollection As InternalCavityPartGroup = _cavityPartsToOccurrencesMapper.GetCavitiesOfPart(documentId, cavity)
            If cavCollection IsNot Nothing AndAlso cavCollection.Count > 1 Then
                Return cavCollection.Select(Function(cv) GetSystemId(cv)).ToArray
            End If

            Return New String() {GetSystemId(cavity)}
        End Function

        Private Shared Function GetSystemId(conn As Component_box_connector_occurrence) As String
            Return conn.SystemId
        End Function

        Private Shared Function GetSystemId(conn As Connector_occurrence) As String
            Return conn.SystemId
        End Function

        Private Shared Function GetSystemId([module] As [Module]) As String
            Return [module].SystemId
        End Function
#End Region

    End Class

End Namespace