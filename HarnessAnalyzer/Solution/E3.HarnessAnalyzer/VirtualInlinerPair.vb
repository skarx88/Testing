Public Class VirtualInlinerPair
    Inherits InlinerPair

    Private _connectorOccsWithKblMapperA As Dictionary(Of Connector_occurrence, KBLMapper)
    Private _connectorOccsWithKblMapperB As Dictionary(Of Connector_occurrence, KBLMapper)
    Private _virtualInliner As Component_box
    Private _boxInternalPinMapping As New List(Of PinMapEntry)

    Public Sub New(virtualInliner As Component_box)
        MyBase.New(virtualInliner.Part_number)

        _connectorOccsWithKblMapperA = New Dictionary(Of Connector_occurrence, KBLMapper)
        _connectorOccsWithKblMapperB = New Dictionary(Of Connector_occurrence, KBLMapper)
        _virtualInliner = virtualInliner

        For Each connection As Component_box_connection In virtualInliner.Connections
            Dim cavA As String = connection.Cavities.SplitSpace.FirstOrDefault
            Dim cavB As String = connection.Cavities.SplitSpace.LastOrDefault

            Dim connA As Component_box_connector = virtualInliner.Component_box_connectors.Where(Function(c) DirectCast(c.Integrated_slots.FirstOrDefault, Slot).Cavities.Any(Function(cav) cav.SystemId = cavA)).FirstOrDefault
            Dim connB As Component_box_connector = virtualInliner.Component_box_connectors.Where(Function(c) DirectCast(c.Integrated_slots.FirstOrDefault, Slot).Cavities.Any(Function(cav) cav.SystemId = cavB)).FirstOrDefault

            If (connA IsNot Nothing) AndAlso (connB IsNot Nothing) Then
                cavA = DirectCast(connA.Integrated_slots.FirstOrDefault, Slot).Cavities.Where(Function(cav) cav.SystemId = cavA).FirstOrDefault.Cavity_number
                cavB = DirectCast(connB.Integrated_slots.FirstOrDefault, Slot).Cavities.Where(Function(cav) cav.SystemId = cavB).FirstOrDefault.Cavity_number

                _boxInternalPinMapping.Add(New PinMapEntry(String.Format("{0};{1}", connA.Id, cavA), String.Format("{0};{1}", connB.Id, cavB)))
            End If
        Next
    End Sub

    ReadOnly Property PinMap() As List(Of Tuple(Of String, String))
        'Hint we keep the old sucking structure of tuples for the outside world...
        Get
            Dim map As New List(Of Tuple(Of String, String))
            For Each map_entry As PinMapEntry In _boxInternalPinMapping
                map.Add(New Tuple(Of String, String)(map_entry.Item1, map_entry.Item2))
            Next
            Return map
        End Get
    End Property

    Public Function GetCavityOccurenceMapping() As List(Of Tuple(Of String, String))
        'Hint: this cumbersome mapping returns the harness side cavity occurences mapping to each other as defined in the virtual inliner along
        'with the harness partnumber (we assume that the A sides always map properly to the B sides)
        Dim l As New List(Of Tuple(Of String, String))
        For Each inlinerA As KeyValuePair(Of Connector_occurrence, KBLMapper) In ConnectorOccsWithKblMapperA
            Dim harnessPartNumberA As String = inlinerA.Value.HarnessPartNumber
            For Each cavityOccA As Cavity_occurrence In inlinerA.Key.Slots.FirstOrDefault.Cavities
                Dim cavityA As Cavity = DirectCast(inlinerA.Value.KblPartMapper(cavityOccA.Part), Cavity)
                Dim id As String = String.Format("{0};{1}", inlinerA.Key.Id, cavityA.Cavity_number)

                Dim pme As PinMapEntry = _boxInternalPinMapping.Find(Function(p) p.Item1 = id)
                If pme IsNot Nothing Then
                    Dim inlinerIdB As String = pme.Item2.Split(";".ToCharArray, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault
                    Dim cavNameB As String = pme.Item2.Split(";".ToCharArray, StringSplitOptions.RemoveEmptyEntries).LastOrDefault
                    For Each inlinerB As KeyValuePair(Of Connector_occurrence, KBLMapper) In ConnectorOccsWithKblMapperB
                        Dim harnessPartNumberB As String = inlinerB.Value.HarnessPartNumber
                        If inlinerB.Key.Id = inlinerIdB Then
                            For Each cavityOccB As Cavity_occurrence In inlinerB.Key.Slots.FirstOrDefault.Cavities
                                Dim cavityB As Cavity = DirectCast(inlinerB.Value.KblPartMapper(cavityOccB.Part), Cavity)
                                If cavityB.Cavity_number = cavNameB Then
                                    Dim idA As String = HarnessConnectivity.GetUniqueId(harnessPartNumberA, cavityOccA.SystemId)
                                    Dim idB As String = HarnessConnectivity.GetUniqueId(harnessPartNumberB, cavityOccB.SystemId)

                                    l.Add(New Tuple(Of String, String)(idA, idB))
                                    l.Add(New Tuple(Of String, String)(idB, idA))
                                    Exit For
                                End If
                            Next
                        End If
                    Next
                End If
            Next
        Next
        Return l
    End Function

    Public Sub RemapPinning(con As Connector_occurrence, boxCon As Component_box_connector)
        'Hint on adding a harness connector with the corresponding box connector, the internal box pinning is to be remapped
        'to the harness connector
        For Each t As PinMapEntry In _boxInternalPinMapping
            Dim boxConnId As String = t.Item1.Split(";".ToCharArray, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault
            If Not String.IsNullOrEmpty(boxConnId) AndAlso boxCon.Id = boxConnId Then
                If Not t.Item1Replaced Then
                    t.Item1 = t.Item1.Replace(boxConnId, con.Id)
                    t.Item1Replaced = True
                End If
            End If
            boxConnId = t.Item2.Split(";".ToCharArray, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault
            If Not String.IsNullOrEmpty(boxConnId) AndAlso boxCon.Id = boxConnId Then
                If Not t.Item2Replaced Then
                    t.Item2 = t.Item2.Replace(boxConnId, con.Id)
                    t.Item2Replaced = True
                End If
            End If
        Next
    End Sub

    ReadOnly Property ConnectorOccsWithKblMapperA As Dictionary(Of Connector_occurrence, KBLMapper)
        Get
            Return _connectorOccsWithKblMapperA
        End Get
    End Property

    ReadOnly Property ActiveConnectorOccsWithKblMapperA As Dictionary(Of Connector_occurrence, KBLMapper)
        Get
            If InactiveObjectsA Is Nothing OrElse Not InactiveObjectsA.ContainsKey(KblObjectType.Connector_occurrence) Then
                Return _connectorOccsWithKblMapperA
            Else
                Dim d As New Dictionary(Of Connector_occurrence, KBLMapper)
                For Each entry As KeyValuePair(Of Connector_occurrence, KBLMapper) In _connectorOccsWithKblMapperA
                    If Not InactiveObjectsA.ContainsValue(KblObjectType.Connector_occurrence, entry.Key.SystemId) Then
                        d.Add(entry.Key, entry.Value)
                    End If
                Next
                Return d
            End If
        End Get
    End Property

    ReadOnly Property ConnectorOccsWithKblMapperB As Dictionary(Of Connector_occurrence, KBLMapper)
        Get
            Return _connectorOccsWithKblMapperB
        End Get
    End Property

    ReadOnly Property ActiveConnectorOccsWithKblMapperB As Dictionary(Of Connector_occurrence, KBLMapper)
        Get
            If InactiveObjectsB Is Nothing OrElse Not InactiveObjectsB.ContainsKey(KblObjectType.Connector_occurrence) Then
                Return _connectorOccsWithKblMapperB
            Else
                Dim d As New Dictionary(Of Connector_occurrence, KBLMapper)
                For Each entry As KeyValuePair(Of Connector_occurrence, KBLMapper) In _connectorOccsWithKblMapperB
                    If Not InactiveObjectsB.ContainsValue(KblObjectType.Connector_occurrence, entry.Key.SystemId) Then
                        d.Add(entry.Key, entry.Value)
                    End If
                Next
                Return d
            End If
        End Get
    End Property

    Public ReadOnly Property InactiveInliners As Integer
        Get
            Return (ConnectorOccsWithKblMapperA.Count - ActiveConnectorOccsWithKblMapperA.Count) + (ConnectorOccsWithKblMapperB.Count - ActiveConnectorOccsWithKblMapperB.Count)
        End Get
    End Property

    ReadOnly Property VirtualInliner As Component_box
        Get
            Return _virtualInliner
        End Get
    End Property

    Private Class PinMapEntry
        Public Property Item1 As String
        Public Property Item2 As String
        Public Property Item1Replaced As Boolean
        Public Property Item2Replaced As Boolean
        Sub New(itm1 As String, itm2 As String)
            Item1 = itm1
            Item2 = itm2
        End Sub
    End Class

End Class