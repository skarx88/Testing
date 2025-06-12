Imports Zuken.E3.App.Controls

Namespace Schematics.Converter.Kbl

    Partial Public Class KblEdbConverter

        <UntypedAddObjectMethod>
        Private Function AddNewEdbObject(documentId As String, wireGroup As Wiring_group) As EdbConversionCableEntity
            Return New EdbConversionCableEntity(documentId, GetSystemId(wireGroup), _edbModel.AddNewCable(GetEdbSystemIdInternal(documentId, wireGroup), ResolveShortName(wireGroup), GetCableType(documentId, wireGroup)))
        End Function

        Private Function AddNewEdbConnector(documentId As String, edbId As String, originalId As String, shortName As String, component As EdbConversionComponentEntity) As EdbConversionConnectorEntity
            ArgumentNullException.ThrowIfNull(component)
            Dim newConn As EdbConversionConnectorEntity = Nothing
            Dim addNewConnector As Func(Of Connectivity.Model.ConnectorType, EdbConversionConnectorEntity) = Function(connectorType2 As Connectivity.Model.ConnectorType) component.Connectors.AddNew(documentId, originalId, edbId, shortName, connectorType2)
            Dim inlinerId As String = Nothing
            If TryGetInlinerId(documentId, edbId, originalId, shortName, component.ComponentType, component.Id, inlinerId) Then
                If _inlinerConnectorMap.ContainsKey(inlinerId) Then
                    newConn = addNewConnector.Invoke(Connectivity.Model.ConnectorType.Counter)
                    _inlinerConnectorMap(inlinerId).AddMatingConnector(newConn)
                Else
                    newConn = addNewConnector.Invoke(Connectivity.Model.ConnectorType.Undefined)
                    _inlinerConnectorMap.Add(inlinerId, newConn)
                End If
            Else
                newConn = addNewConnector.Invoke(Connectivity.Model.ConnectorType.Undefined)
            End If

            TryRegisterEntityAndRaiseCreated(documentId, newConn) ' before cavities of connector (so that the methods can resolve this and do not create new)

            Return newConn
        End Function

        <UntypedAddObjectMethod>
        Private Function AddNewEdbObject(documentId As String, connector As Connector_occurrence) As EdbConversionConnectorEntity ' circular reference POSSIBLE when using within component creation
            Return AddNewEdbConnector(documentId, GetEdbSystemIdInternal(documentId, connector), GetSystemId(connector), ResolveShortName(connector, documentId), GetOrCreateComponent(documentId, connector))
        End Function

        <UntypedAddObjectMethod>
        Private Function AddNewEdbObject(documentId As String, cmpBoxConnOcc As Component_box_connector_occurrence) As EdbConversionConnectorEntity
            Return AddNewEdbConnector(documentId, GetEdbSystemIdInternal(documentId, cmpBoxConnOcc), GetSystemId(cmpBoxConnOcc), ResolveShortName(cmpBoxConnOcc, documentId), GetOrCreateComponent(documentId, cmpBoxConnOcc))
        End Function

        <UntypedAddObjectMethod>
        Private Function AddNewEdbObject(documentId As String, cavity As Cavity_occurrence) As EdbConversionCavityEntity ' circular POSSIBLE when using within connector creation
            Dim connector As EdbConversionConnectorEntity = GetOrCreateConnector(documentId, cavity)
            If connector IsNot Nothing Then
                Dim cavityShortName As String = ResolveShortName(cavity, documentId)
                If IsSpliceOrEyelet(documentId, cavity) Then
                    Dim cavGroup As EdbConversionCavityGroupEntity = GetOrCreateCavityGroup(documentId, cavity)
                    Dim newVirtCav As EdbConversionCavityEntity = AddNewEdbCavityInternal(Of EdbConversionCavityEntity)(documentId, GetEdbSystemIdInternal(documentId, cavity), GetSystemIdOrGroupedByPart(documentId, cavity), cavityShortName, connector)
                    newVirtCav.IsVirtual = True
                    cavGroup.AddEdbId(newVirtCav.Id)
                    Return newVirtCav
                Else
                    Return AddNewEdbCavityInternal(Of EdbConversionCavityEntity)(documentId, GetEdbSystemIdInternal(documentId, cavity), GetSystemIdOrGroupedByPart(documentId, cavity), cavityShortName, connector)
                End If
            End If
            Return Nothing
        End Function

        Private Function AddNewEdbCavityInternal(Of T As EdbConversionCavityEntity)(documentId As String, edbSysId As String, originalIds() As String, cavityShortName As String, connector As EdbConversionConnectorEntity, Optional addToModel As Boolean = True) As T
            Dim convEdbCav As T = connector.AddNewCavity(Of T)(documentId, edbSysId, originalIds, cavityShortName, addToModel)
            Dim connOcc As Connector_occurrence = _combinedMappers.ConnectorsOfCavity.TryGetDocumentObject(originalIds(0), documentId)
            If connOcc IsNot Nothing Then
                Dim connectorInlinerId As String = Nothing
                If TryGetInlinerId(documentId, connOcc, connectorInlinerId) Then
                    Dim cavityInlinerId As String = IdConverter.GetCombined(connectorInlinerId, cavityShortName)
                    If Not _inlinerCavityMap.TryAdd(cavityInlinerId, convEdbCav) Then
                        _inlinerCavityMap(cavityInlinerId).MatingCavities.TryAdd(convEdbCav)
                    End If
                End If
            End If

            TryRegisterEntityAndRaiseCreated(documentId, convEdbCav)
            Return convEdbCav
        End Function

        <UntypedAddObjectMethod>
        Private Function AddNewEdbObject(documentId As String, comp As Component_occurrence) As EdbConversionComponentEntity ' no circular reference possible
            Return AddnewEdbComponent(documentId, GetSystemId(comp), ResolveShortName(comp, documentId), GetEdbSystemIdInternal(documentId, comp), ResolveComponentTypeInternal(comp, documentId))
        End Function

        <UntypedAddObjectMethod>
        Private Function AddNewEdbObject(documentId As String, comp As Component_box_occurrence) As EdbConversionComponentEntity ' no circular reference possible
            Return AddnewEdbComponent(documentId, GetSystemId(comp), ResolveShortName(comp), GetEdbSystemIdInternal(documentId, comp), ResolveComponentTypeInternal(comp, documentId))
        End Function

        Private Function AddnewEdbComponent(documentId As String, componentKblId As String, componentEdbId As String, componentShortName As String, Optional componentType As Connectivity.Model.ComponentType = Connectivity.Model.ComponentType.Undefined) As EdbConversionComponentEntity
            Dim newCompEntity As New EdbConversionComponentEntity(documentId, componentKblId, _edbModel.AddNewComponent(componentEdbId, componentShortName, componentType), componentType)
            If RegisterConvertedEntity(documentId, newCompEntity) Then
                OnAfterEntityCreated(newCompEntity)
            End If
            Return newCompEntity
        End Function

        Private Function AddNewEdbComponent(compInfo As ComponentInfo) As EdbConversionComponentEntity
            With compInfo
                Return AddnewEdbComponent(.DocumentId, .OriginalIds.SingleOrDefault, .Id, .ShortName, .Type.GetValueOrDefault(DefaultComponentType))
            End With
        End Function

        ''' <summary>
        ''' Creates a new ConnectivityView Wire or cable (or returns the cable with added wire if the wire belongs to a wiring_group) (Twisting cable)
        ''' </summary>
        ''' <param name="documentId"></param>
        ''' <param name="wire"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <UntypedAddObjectMethod>
        Private Function AddNewEdbObject(documentId As String, wire As Wire_occurrence) As EdbConversionEntity ' circular POSSIBLE when using within cavity creation
            Static edbWireOnlyInternal As Boolean = False
            Dim edbWire As EdbConversionWireEntity = Nothing
            If Not edbWireOnlyInternal Then
                edbWireOnlyInternal = True
                Try
                    edbWire = CType(AddNewOrGetEdbObject(documentId, wire), EdbConversionWireEntity)
                Catch ex As Exception
#If DEBUG Or CONFIG = "Debug" Then
                    Throw
#End If
                Finally
                    edbWireOnlyInternal = False
                End Try
            Else
                edbWire = CreateNewWireEntity(documentId,
                          _edbModel.AddNewWire(GetEdbSystemIdInternal(documentId, wire), ResolveShortName(wire)),
                          GetSystemId(wire),
                          GetOrCreateCavitites(documentId, wire),
                          GetSpecifier(documentId, wire),
                          GetSqMmCsa(documentId, wire),
                          GetFunctionIdAndName(documentId, wire),
                          GetEdbModuleIdsAndName(documentId, wire))
                Return edbWire
            End If

            Dim wg As Wiring_group = _combinedMappers.WireToGroupMapper.TryGetDocumentObject(GetSystemId(wire), documentId)
            If wg IsNot Nothing Then
                Dim cable As EdbConversionCableEntity = AddNewOrGetEdbObject(documentId, wg)
                If cable IsNot Nothing Then
                    If edbWire IsNot Nothing Then
                        cable.Wires.TryAdd(edbWire)
                    End If
                    Return cable
                End If
            Else
                Return edbWire
            End If
            Return Nothing
        End Function

        <UntypedAddObjectMethod>
        Private Function AddNewEdbObject(documentId As String, cable As Special_wire_occurrence) As EdbConversionCableEntity
            Dim edbCable As New EdbConversionCableEntity(documentId, GetSystemId(cable), _edbModel.AddNewCable(GetEdbSystemIdInternal(documentId, cable), ResolveShortName(cable), GetCableType(documentId, cable)))

            For Each core As Core_occurrence In cable.Core_Occurrence
                Dim edbCore As EdbConversionWireEntity = AddNewOrGetEdbObject(documentId, core)
                If edbCore IsNot Nothing Then edbCable.Wires.TryAdd(edbCore)
            Next

            Return edbCable
        End Function

        <UntypedAddObjectMethod>
        Private Function AddNewEdbObject(documentId As String, core As Core_occurrence) As EdbConversionWireEntity
            Dim sn As String = ResolveShortName(core)
            Return CreateNewWireEntity(documentId,
                                      _edbModel.AddNewWire(GetEdbSystemIdInternal(documentId, core), sn),
                                      GetSystemId(core),
                                      GetOrCreateCavitites(documentId, core),
                                      GetSpecifier(documentId, core),
                                      GetSqMmCsa(documentId, core),
                                      GetFunctionIdAndName(documentId, core),
                                      GetEdbModuleIdsAndName(documentId, core))
        End Function

        Private Function CreateNewWireEntity(documentId As String, wireItem As Connectivity.Model.Wire, originalId As String, cavities As IEnumerable(Of EdbConversionCavityEntity), specifier As String, csa As Double, functionIdAndName As Tuple(Of String, String), moduleIdsAndName As List(Of Tuple(Of String, String))) As EdbConversionWireEntity
            Dim edbWire As New EdbConversionWireEntity(documentId, originalId, wireItem)
            With edbWire
                .Specifier = specifier
                .Csa = csa
                For Each edbCav As EdbConversionCavityEntity In cavities
                    .Cavities.Add(edbCav)
                Next

                SetFunction(edbWire, functionIdAndName)
                SetModules(edbWire, moduleIdsAndName)

            End With
            Return edbWire
        End Function

    End Class

End Namespace