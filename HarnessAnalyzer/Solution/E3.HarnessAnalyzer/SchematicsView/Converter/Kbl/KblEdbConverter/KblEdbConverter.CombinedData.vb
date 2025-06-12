Namespace Schematics.Converter.Kbl

    Partial Public Class KblEdbConverter

        Private Class CombinedKblData
            Implements IDisposable

            Private _disposedValue As Boolean
            Private _occurences As GroupedCastableObjectsCollection(Of IKblOccurrence)
            Private _connectorsOfCavityMapper As GroupedObjectsCollection(Of Connector_occurrence)
            Private _cavitiesOfWireMapper As GroupedObjectsCollection(Of List(Of Cavity_occurrence))
            Private _partMapper As GroupedObjectsCollection(Of Object)
            Private _units As GroupedObjectsCollection(Of [Lib].Schema.Kbl.Unit)
            Private _wireConnections As GroupedObjectsCollection(Of [Lib].Schema.Kbl.Connection)
            Private _objectToModuleMapper As GroupedObjectsCollection(Of List(Of [Lib].Schema.Kbl.Module))
            Private _wireToGroupMapper As GroupedObjectsCollection(Of [Lib].Schema.Kbl.Wiring_group)
            Private _wiresOfCavity As GroupedObjectsCollection(Of List(Of IKblOccurrence))
            Private _componentBoxConnectorToComponentBox As GroupedObjectsCollection(Of Component_box_occurrence)
            Private _cavityToComponentBoxConnector As GroupedObjectsCollection(Of Component_box_connector_occurrence)

            Friend Sub New()
            End Sub

            Public Sub Init(data() As KblDocumentData, Optional [onError] As Action(Of InitializeError) = Nothing, Optional cancelToken As Nullable(Of System.Threading.CancellationToken) = Nothing)
                _occurences = New GroupedCastableObjectsCollection(Of IKblOccurrence)(data, Function(kbl) kbl.KBLOccurrenceMapper, cancelToken)
                _connectorsOfCavityMapper = New GroupedObjectsCollection(Of Connector_occurrence)(data, Function(kbl) GetConnectorOfCavityMapper(kbl), Sub(key As String, obj As Object) onError?.Invoke(New InitializeError(InitializeError.Type.AddErrorConnectorAtCavityMapper, key, obj)), cancelToken)
                _cavitiesOfWireMapper = New GroupedObjectsCollection(Of List(Of Cavity_occurrence))(data, Function(kbl) GetCavitiesOfWireMapper(kbl), Sub(key As String, obj As Object) onError?.Invoke(New InitializeError(InitializeError.Type.AddErrorConnectorAtCavityMapper, key, obj)), cancelToken)
                _wiresOfCavity = New GroupedObjectsCollection(Of List(Of IKblOccurrence))(data, Function(kbl) GetWiresOfCavityMapper(kbl), Sub(key As String, obj As Object) onError?.Invoke(New InitializeError(InitializeError.Type.AddErrorWireAtCavityMapper, key, obj)), cancelToken)
                _partMapper = New GroupedObjectsCollection(Of Object)(data, Function(kbl) kbl.KBLPartMapper, Sub(key As String, obj As Object) onError?.Invoke(New InitializeError(InitializeError.Type.AddErrorPartMapper, key, obj)), cancelToken)
                _units = New GroupedObjectsCollection(Of Unit)(data, Function(kbl) kbl.KBLUnitMapper, Sub(key As String, obj As Object) onError?.Invoke(New InitializeError(InitializeError.Type.AddErrorUnitsMapper, key, obj)), cancelToken)
                _wireConnections = New GroupedObjectsCollection(Of [Lib].Schema.Kbl.Connection)(data, Function(kbl) kbl.GetConnections.SelectMany(Function(wg) wg.Wire.SplitSpace.Select(Function(wireId) New KeyValuePair(Of String, Connection)(wireId, wg))), Sub(key As String, obj As Object) onError?.Invoke(New InitializeError(InitializeError.Type.AddErrorWireConenctions, key, obj)), cancelToken)
                _objectToModuleMapper = New GroupedObjectsCollection(Of List(Of [Lib].Schema.Kbl.Module))(data, Function(kbl) GetModulesDic(kbl), Sub(key As String, obj As Object) onError?.Invoke(New InitializeError(InitializeError.Type.AddErrorObjectAtModuleMapper, key, obj)), cancelToken)
                _wireToGroupMapper = New GroupedObjectsCollection(Of Wiring_group)(data, Function(kbl) kbl.GetWiringGroups.SelectMany(Function(wg) wg.Assigned_wire.SplitSpace.Select(Function(wireId) New KeyValuePair(Of String, [Lib].Schema.Kbl.Wiring_group)(wireId, wg))), Sub(key As String, obj As Object) onError?.Invoke(New InitializeError(InitializeError.Type.AddErrorWireAtWireGroupMapper, key, obj)), cancelToken)
                _componentBoxConnectorToComponentBox = New GroupedObjectsCollection(Of Component_box_occurrence)(data, Function(kbl) kbl.KBLComponentBoxConnectorComponentBoxMapper.ToDictionary(Function(kv) kv.Key, Function(kv) CType(kbl.KBLOccurrenceMapper(kv.Value), Component_box_occurrence)), Nothing, cancelToken)
                _cavityToComponentBoxConnector = New GroupedObjectsCollection(Of Component_box_connector_occurrence)(data, Function(kbl) kbl.KBLCavityComponentBoxConnectorMapper.ToDictionary(Function(kv) kv.Key, Function(kv) CType(kbl.KBLOccurrenceMapper(kv.Value), Component_box_connector_occurrence)), Nothing, cancelToken)
            End Sub

            Private Function GetModulesDic(kbl As KBLMapper) As Dictionary(Of String, List(Of [Lib].Schema.Kbl.Module))
                Dim kblModules As Dictionary(Of String, [Lib].Schema.Kbl.Module) = kbl.GetModules.ToDictionary(Function(md) GetSystemId(md), Function(md) md)
                Return kbl.KBLObjectModuleMapper.Select(Function(kv) New KeyValuePair(Of String, List(Of [Lib].Schema.Kbl.Module))(kv.Key, kv.Value.Select(Function(id) kblModules(id)).ToList)).ToDictionary(Function(kv) kv.Key, Function(kv) kv.Value)
            End Function

            Private Function GetCavitiesOfWireMapper(kbl As KBLMapper) As Dictionary(Of String, List(Of Cavity_occurrence))
                Dim dic As New Dictionary(Of String, List(Of Cavity_occurrence))

                For Each cavOfWiresKv As KeyValuePair(Of String, Dictionary(Of String, List(Of Cavity_occurrence))) In kbl.KBLWireCavityMapper
                    For Each wiresKv As KeyValuePair(Of String, List(Of Cavity_occurrence)) In cavOfWiresKv.Value
                        dic.AddOrUpdate(wiresKv.Key, CType(kbl.KBLOccurrenceMapper(cavOfWiresKv.Key), Cavity_occurrence), , True)
                    Next
                Next

                Return dic
            End Function

            Private Function GetWiresOfCavityMapper(kbl As KblMapper) As Dictionary(Of String, List(Of IKblOccurrence))
                Dim wiresCavities As New List(Of KeyValuePair(Of String, String))

                For Each cavOfWires As KeyValuePair(Of String, Dictionary(Of String, List(Of Cavity_occurrence))) In kbl.KBLWireCavityMapper ' cavity
                    For Each kv As KeyValuePair(Of String, List(Of Cavity_occurrence)) In cavOfWires.Value ' key = wire
                        wiresCavities.Add(New KeyValuePair(Of String, String)(cavOfWires.Key, kv.Key)) ' cavityId, wireId
                    Next
                Next

                Return wiresCavities.GroupBy(Function(kv) kv.Key).ToDictionary(Function(grp) grp.Key, Function(grp) grp.Select(Function(kv) kbl.KBLOccurrenceMapper(kv.Value)).ToList)
            End Function

            Private Function GetCavitiesOfConnectorMapper(kbl As KBLMapper) As Dictionary(Of String, List(Of Cavity_occurrence))
                Dim dic As New Dictionary(Of String, List(Of Cavity_occurrence))
                For Each grp As IGrouping(Of String, KeyValuePair(Of String, String)) In kbl.KBLCavityConnectorMapper.GroupBy(Function(kv2) kv2.Value)
                    For Each kv As KeyValuePair(Of String, String) In grp
                        dic.AddOrUpdate(grp.Key, CType(kbl.KBLOccurrenceMapper(kv.Key), Cavity_occurrence), , True)
                    Next
                Next
                Return dic
            End Function

            Private Function GetConnectorOfCavityMapper(kbl As KBLMapper) As Dictionary(Of String, Connector_occurrence)
                Dim dic As New Dictionary(Of String, Connector_occurrence)
                For Each kv As KeyValuePair(Of String, String) In kbl.KBLCavityConnectorMapper
                    dic.Add(kv.Key, CType(kbl.KBLOccurrenceMapper(kv.Value), Connector_occurrence))
                Next
                Return dic
            End Function

            ReadOnly Property WiresOfCavityMapper As GroupedObjectsCollection(Of List(Of IKblOccurrence))
                Get
                    Return _wiresOfCavity
                End Get
            End Property

            ReadOnly Property WireToGroupMapper As GroupedObjectsCollection(Of [Lib].Schema.Kbl.Wiring_group)
                Get
                    Return _wireToGroupMapper
                End Get
            End Property

            ReadOnly Property ObjectToModuleMapper As GroupedObjectsCollection(Of List(Of [Lib].Schema.Kbl.Module))
                Get
                    Return _objectToModuleMapper
                End Get
            End Property

            ReadOnly Property WireConnections As GroupedObjectsCollection(Of [Lib].Schema.Kbl.Connection)
                Get
                    Return _wireConnections
                End Get
            End Property

            ReadOnly Property Units As GroupedObjectsCollection(Of [Lib].Schema.Kbl.Unit)
                Get
                    Return _units
                End Get
            End Property

            ReadOnly Property Parts As GroupedObjectsCollection(Of Object)
                Get
                    Return _partMapper
                End Get
            End Property

            ReadOnly Property Occurrences As GroupedCastableObjectsCollection(Of IKblOccurrence)
                Get
                    Return _occurences
                End Get
            End Property

            ReadOnly Property ConnectorsOfCavity As GroupedObjectsCollection(Of Connector_occurrence)
                Get
                    Return _connectorsOfCavityMapper
                End Get
            End Property

            ReadOnly Property CavitiesOfWire As GroupedObjectsCollection(Of List(Of Cavity_occurrence))
                Get
                    Return _cavitiesOfWireMapper
                End Get
            End Property

            ReadOnly Property ComponentBoxConnectorsToComponentBox As GroupedObjectsCollection(Of Component_box_occurrence)
                Get
                    Return _componentBoxConnectorToComponentBox
                End Get
            End Property

            ReadOnly Property CavityToComponentBoxConnector As GroupedObjectsCollection(Of Component_box_connector_occurrence)
                Get
                    Return _cavityToComponentBoxConnector
                End Get
            End Property

            Protected Overridable Sub Dispose(disposing As Boolean)
                If Not Me._disposedValue Then
                    _occurences = Nothing
                    _connectorsOfCavityMapper = Nothing
                    _cavitiesOfWireMapper = Nothing
                    _partMapper = Nothing
                    _units = Nothing
                    _wireConnections = Nothing
                    _objectToModuleMapper = Nothing
                    _wireToGroupMapper = Nothing
                    _wiresOfCavity = Nothing
                    _componentBoxConnectorToComponentBox = Nothing
                    _cavityToComponentBoxConnector = Nothing
                End If
                Me._disposedValue = True
            End Sub

            Public Sub Dispose() Implements IDisposable.Dispose
                ' Ändern Sie diesen Code nicht. Fügen Sie oben in Dispose(disposing As Boolean) Bereinigungscode ein.
                Dispose(True)
                GC.SuppressFinalize(Me)
            End Sub

        End Class

    End Class

End Namespace

