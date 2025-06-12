Imports System.Xml.Serialization
Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinGrid
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Partial Public Class InformationHub

    Private Sub InitializeConnectors()
        _connectorGridAppearance = GridAppearance.All.OfType(Of ConnectorGridAppearance).Single
        AddNewRowFilters(ugConnectors)

        If (_comparisonMapperList Is Nothing) Then
            FillDataSource(Me.udsConnectors, _connectorGridAppearance, _kblMapper.GetConnectorOccurrences.Count)
        Else
            FillDataSource(Me.udsConnectors, _connectorGridAppearance, 0, _comparisonMapperList(KblObjectType.Connector_occurrence))
        End If

        If (Me.udsConnectors.Band.Columns.Count = 0) OrElse (Me.udsConnectors.Rows.Count = 0) Then
            Me.utcInformationHub.Tabs(TabNames.tabConnectors.ToString()).Visible = False
        End If
    End Sub

    Private Sub CavityCellDataPreparation(ByVal e As CellDataRequestedEventArgs, ByRef cavityOccurrence As Cavity_occurrence, contactPoint As Contact_point, ByRef fromReference As Boolean, ByRef terminalDescription As String, terminalPartNumber As String, ByRef terminalPlating As String)
        If (HasChangeTypeWithInverse(e.Row, CompareChangeType.New)) Then
            If (cavityOccurrence Is Nothing) AndAlso (_kblMapperForCompare IsNot Nothing) Then
                cavityOccurrence = _kblMapperForCompare.GetCavityOccurrenceOfContactPointId(contactPoint.SystemId)
                fromReference = False
            End If

            If (cavityOccurrence Is Nothing) Then
                cavityOccurrence = _kblMapper.GetCavityOccurrenceOfContactPointId(contactPoint.SystemId)
                fromReference = True
            End If
        Else
            If (cavityOccurrence Is Nothing) AndAlso (_kblMapperForCompare IsNot Nothing) Then
                cavityOccurrence = _kblMapperForCompare.GetCavityOccurrenceOfContactPointId(contactPoint.SystemId)
                fromReference = False
            End If

            If (cavityOccurrence Is Nothing) Then
                cavityOccurrence = _kblMapper.GetCavityOccurrenceOfContactPointId(contactPoint.SystemId)
                fromReference = True
            End If
        End If

        If (terminalPartNumber <> String.Empty) Then
            Dim terminal As General_terminal = _kblMapper.GetGeneralTerminals.Where(Function(genTer) genTer.Part_number = terminalPartNumber).FirstOrDefault
            If (terminal Is Nothing) Then
                terminal = _kblMapperForCompare.GetGeneralTerminals.Where(Function(genTer) genTer.Part_number = terminalPartNumber).FirstOrDefault
            End If

            If (terminal IsNot Nothing) Then
                terminalDescription = terminal.Description
                terminalPlating = terminal.Plating_material
            End If
        End If
    End Sub

    Private Sub CavityCellDataRequested(e As CellDataRequestedEventArgs)
        Dim cavityHousing As Tuple(Of Object, Tuple(Of String, String)) = TryCast(e.Row.Tag, Tuple(Of Object, Tuple(Of String, String)))
        Dim cavityOccurrence As Cavity_occurrence = TryCast(cavityHousing.Item1, Cavity_occurrence)
        Dim contactPoint As Contact_point = TryCast(cavityHousing.Item1, Contact_point)
        Dim fromReference As Boolean = True
        Dim partsAdded As Boolean = False
        Dim sealPartNumber As String = If(cavityHousing.Item2 IsNot Nothing, cavityHousing.Item2.Item2, String.Empty)
        Dim terminalDescription As String = String.Empty
        Dim terminalPartNumber As String = If(cavityHousing.Item2 IsNot Nothing, cavityHousing.Item2.Item1, String.Empty)
        Dim terminalPlating As String = String.Empty

        CavityCellDataPreparation(e, cavityOccurrence, contactPoint, fromReference, terminalDescription, terminalPartNumber, terminalPlating)

        With cavityOccurrence
            Select Case e.Column.Key
                Case SYSTEM_ID_COLUMN_KEY
                    If (contactPoint IsNot Nothing) Then
                        e.Data = contactPoint.SystemId
                    Else
                        e.Data = .SystemId
                    End If
                Case ConnectorPropertyName.Cavity_number
                    If (fromReference) AndAlso (_kblMapper.KBLOccurrenceMapper.ContainsKey(.SystemId)) AndAlso (_kblMapper.KBLPartMapper.ContainsKey(If(.Part Is Nothing, String.Empty, .Part))) Then
                        e.Data = IIf(TryCast(_kblMapper.KBLPartMapper(.Part), Cavity) IsNot Nothing, DirectCast(_kblMapper.KBLPartMapper(.Part), Cavity).Cavity_number, String.Empty)
                    ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLPartMapper.ContainsKey(If(.Part Is Nothing, String.Empty, .Part))) Then
                        e.Data = IIf(TryCast(_kblMapperForCompare.KBLPartMapper(.Part), Cavity) IsNot Nothing, DirectCast(_kblMapperForCompare.KBLPartMapper(.Part), Cavity).Cavity_number, String.Empty)
                    End If
                Case ConnectorPropertyName.Description
                    e.Data = terminalDescription
                Case ConnectorPropertyName.Terminal_part_number
                    e.Data = terminalPartNumber
                Case ConnectorPropertyName.Terminal_part_information
                    If (terminalPartNumber <> String.Empty) Then
                        e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                    End If
                Case ConnectorPropertyName.Seal_part_number
                    e.Data = sealPartNumber
                Case ConnectorPropertyName.Seal_part_information
                    If (sealPartNumber <> String.Empty) Then
                        e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                    End If
                Case ConnectorPropertyName.Plug_part_number
                    If Not String.IsNullOrEmpty(cavityOccurrence.Associated_plug) Then
                        If fromReference AndAlso _kblMapper IsNot Nothing Then
                            Dim cav_plug_occ As Cavity_plug_occurrence = _kblMapper.GetOccurrenceObject(Of Cavity_plug_occurrence)(cavityOccurrence.Associated_plug)
                            Dim cav_plug_part As Cavity_plug = TryCast(cav_plug_occ?.GetPart(_kblMapper), Cavity_plug)
                            If cav_plug_part IsNot Nothing Then
                                e.Data = cav_plug_part.Part_number
                            End If
                        End If

                        If Not fromReference AndAlso _kblMapperForCompare IsNot Nothing Then
                            Dim cav_plug_occ As Cavity_plug_occurrence = _kblMapperForCompare.GetOccurrenceObject(Of Cavity_plug_occurrence)(cavityOccurrence.Associated_plug)
                            Dim cav_plug_part As Cavity_plug = TryCast(cav_plug_occ?.GetPart(_kblMapperForCompare), Cavity_plug)
                            If cav_plug_part IsNot Nothing Then
                                e.Data = cav_plug_part.Part_number
                            End If
                        End If
                    End If
                Case ConnectorPropertyName.Plug_part_information
                    If (.Associated_plug IsNot Nothing) Then
                        e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                    End If
                Case ConnectorPropertyName.Plating
                    e.Data = terminalPlating
                Case HarnessAnalyzer.[Shared].CORE_WIRE_NUMBER_KEY

                    If contactPoint Is Nothing Then
                        If (fromReference) Then
                            If _kblMapper.KBLCavityContactPointMapper.ContainsKey(cavityOccurrence.SystemId) Then
                                contactPoint = _kblMapper.KBLCavityContactPointMapper(cavityOccurrence.SystemId).FirstOrDefault
                            End If
                        Else
                            If _kblMapperForCompare.KBLCavityContactPointMapper.ContainsKey(cavityOccurrence.SystemId) Then
                                contactPoint = _kblMapperForCompare.KBLCavityContactPointMapper(cavityOccurrence.SystemId).FirstOrDefault
                            End If
                        End If
                    End If

                    GetCoreWireNumberCellValueForCavity(e, contactPoint)

                Case WirePropertyName.Wire_type
                    If fromReference Then
                        GetCoreWireTypeCellValueForCavity(e, contactPoint)
                    End If
                Case WirePropertyName.Cross_section_area
                    If fromReference Then
                        GetCoreWireCrossSectionCellValueForCavity(e, contactPoint)
                    End If
                Case WirePropertyName.Core_Colour
                    If fromReference Then
                        GetCoreWireColorCellValueForCavity(e, contactPoint)
                    End If
                Case NameOf(InformationHubStrings.AssignedModules_ColumnCaption)
                    Dim cavitySealOcc As Cavity_seal_occurrence = Nothing
                    Dim specTerminalOcc As Special_terminal_occurrence = Nothing
                    Dim terminalOcc As Terminal_occurrence = Nothing

                    If (fromReference) Then
                        If (contactPoint IsNot Nothing) AndAlso (contactPoint.Associated_parts IsNot Nothing) Then
                            For Each associatedPartId As String In contactPoint.Associated_parts.SplitSpace
                                If (_kblMapper.KBLOccurrenceMapper.ContainsKey(associatedPartId)) AndAlso (TypeOf _kblMapper.KBLOccurrenceMapper(associatedPartId) Is Cavity_seal_occurrence) AndAlso (_kblMapper.GetCavitySeal(_kblMapper.GetHarness.GetCavitySealOccurrence(associatedPartId).Part).Part_number = sealPartNumber) Then
                                    cavitySealOcc = _kblMapper.GetHarness.GetCavitySealOccurrence(associatedPartId)
                                ElseIf (_kblMapper.KBLOccurrenceMapper.ContainsKey(associatedPartId)) AndAlso (TypeOf _kblMapper.KBLOccurrenceMapper(associatedPartId) Is Special_terminal_occurrence) AndAlso (_kblMapper.GetGeneralTerminal(_kblMapper.GetHarness.GetSpecialTerminalOccurrence(associatedPartId).Part).Part_number = terminalPartNumber) Then
                                    specTerminalOcc = _kblMapper.GetHarness.GetSpecialTerminalOccurrence(associatedPartId)
                                ElseIf (_kblMapper.KBLOccurrenceMapper.ContainsKey(associatedPartId)) AndAlso (TypeOf _kblMapper.KBLOccurrenceMapper(associatedPartId) Is Terminal_occurrence) AndAlso (_kblMapper.GetGeneralTerminal(_kblMapper.GetHarness.GetTerminalOccurrence(associatedPartId).Part).Part_number = terminalPartNumber) Then
                                    terminalOcc = _kblMapper.GetHarness.GetTerminalOccurrence(associatedPartId)
                                End If
                            Next
                        End If

                        GetAssignedModulesCellValueForCavity(e, cavityOccurrence, cavitySealOcc, specTerminalOcc, terminalOcc)
                    Else
                        If (contactPoint IsNot Nothing) AndAlso (contactPoint.Associated_parts IsNot Nothing) Then
                            For Each associatedPartId As String In contactPoint.Associated_parts.SplitSpace
                                If (_kblMapperForCompare.KBLOccurrenceMapper.ContainsKey(associatedPartId)) AndAlso (TypeOf _kblMapperForCompare.KBLOccurrenceMapper(associatedPartId) Is Cavity_seal_occurrence) AndAlso (_kblMapperForCompare.GetCavitySeal(_kblMapperForCompare.GetHarness.GetCavitySealOccurrence(associatedPartId).Part).Part_number = sealPartNumber) Then
                                    cavitySealOcc = _kblMapperForCompare.GetHarness.GetCavitySealOccurrence(associatedPartId)
                                ElseIf (_kblMapperForCompare.KBLOccurrenceMapper.ContainsKey(associatedPartId)) AndAlso (TypeOf _kblMapperForCompare.KBLOccurrenceMapper(associatedPartId) Is Special_terminal_occurrence) AndAlso (_kblMapperForCompare.GetGeneralTerminal(_kblMapperForCompare.GetHarness.GetSpecialTerminalOccurrence(associatedPartId).Part).Part_number = terminalPartNumber) Then
                                    specTerminalOcc = _kblMapperForCompare.GetHarness.GetSpecialTerminalOccurrence(associatedPartId)
                                ElseIf (_kblMapperForCompare.KBLOccurrenceMapper.ContainsKey(associatedPartId)) AndAlso (TypeOf _kblMapperForCompare.KBLOccurrenceMapper(associatedPartId) Is Terminal_occurrence) AndAlso (_kblMapperForCompare.GetGeneralTerminal(_kblMapperForCompare.GetHarness.GetTerminalOccurrence(associatedPartId).Part).Part_number = terminalPartNumber) Then
                                    terminalOcc = _kblMapperForCompare.GetHarness.GetTerminalOccurrence(associatedPartId)
                                End If
                            Next
                        End If

                        GetAssignedModulesCellValueForCavity(e, cavityOccurrence, cavitySealOcc, specTerminalOcc, terminalOcc)
                    End If
                Case Else
                    OnUnhandledCellDataRequested(e)
            End Select
        End With
    End Sub

    Private Sub ConnectorCellDataRequested(e As CellDataRequestedEventArgs)
        With DirectCast(e.Row.Tag, Connector_occurrence)
            Dim connector As Connector_housing = Nothing
            Dim fromReference As Boolean = True

            If HasChangeTypeWithInverse(e.Row, CompareChangeType.New) Then
                fromReference = False

                If (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLPartMapper.ContainsKey(If(.Part Is Nothing, String.Empty, .Part))) Then
                    connector = TryCast(_kblMapperForCompare.KBLPartMapper(.Part), Connector_housing)
                End If
            Else
                If (_kblMapper.KBLOccurrenceMapper.ContainsKey(.SystemId)) AndAlso (_kblMapper.KBLPartMapper.ContainsKey(If(.Part Is Nothing, String.Empty, .Part))) Then
                    connector = TryCast(_kblMapper.KBLPartMapper(.Part), Connector_housing)
                End If
            End If

            If (connector IsNot Nothing) Then
                RequestCellPartData(e.Data, fromReference, e.Column.Key, connector)
            End If

            Select Case e.Column.Key
                Case ConnectorPropertyName.Id
                    e.Data = .Id
                Case ConnectorPropertyName.Alias_id
                    If (.Alias_id.Length <> 0) Then
                        If (.Alias_id.Length = 1) AndAlso (.Alias_id(0).Description Is Nothing) AndAlso (.Alias_id(0).Scope Is Nothing) Then
                            e.Data = .Alias_id(0).Alias_id
                        Else
                            e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                        End If
                    End If
                Case ConnectorPropertyName.Description
                    If Not String.IsNullOrEmpty(.Description) Then
                        e.Data = .Description
                    End If
                Case ConnectorPropertyName.Usage
                    If .UsageSpecified Then
                        e.Data = .Usage.ToStringOrXmlName
                    End If
                Case ConnectorPropertyName.Housing_colour
                    If (connector IsNot Nothing) AndAlso (connector.Housing_colour IsNot Nothing) Then
                        e.Data = connector.Housing_colour
                    End If
                Case ConnectorPropertyName.Housing_code
                    If (connector IsNot Nothing) AndAlso (connector.Housing_code IsNot Nothing) Then
                        e.Data = connector.Housing_code
                    End If
                Case ConnectorPropertyName.Housing_type
                    If (connector IsNot Nothing) AndAlso (connector.Housing_type IsNot Nothing) Then
                        e.Data = connector.Housing_type
                    End If
                Case ConnectorPropertyName.Reference_element
                    If (.Reference_element IsNot Nothing) Then
                        If (.Reference_element.SplitSpace.Length = 1) Then
                            e.Data = Harness.GetReferenceElement(.Reference_element, If(fromReference, _kblMapper, _kblMapperForCompare))
                        ElseIf (.Reference_element.SplitSpace.Length > 1) Then
                            e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                        End If
                    End If
                Case ConnectorPropertyName.Installation_Information
                    If (.Installation_information.Length <> 0) Then
                        e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                    End If
                Case ConnectorPropertyName.Localized_description
                    If (.Localized_description.Length > 0) Then
                        If (.Localized_description.Length = 1) Then
                            e.Data = .Localized_description(0).Value
                        Else
                            e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                        End If
                    End If
                Case CAVITY_COUNT_COLUMN_KEY
                    If (.Slots IsNot Nothing) AndAlso (.Slots.Length <> 0) Then
                        e.Data = .Slots(0).Cavities.Length
                    Else
                        e.Data = 0
                    End If
                Case NameOf(InformationHubStrings.AssignedModules_ColumnCaption)
                    e.Data = GetAssignedModules(.SystemId)
                Case ConnectorPropertyName.Placement
                    If e.Row.Band.Tag IsNot Nothing Then
                        If TryCast(e.Row.Band.Tag, Dictionary(Of String, Object)) IsNot Nothing Then
                            Dim fcp As ConnectorChangedProperty = TryCast(DirectCast(e.Row.Band.Tag, Dictionary(Of String, Object)).Values.First, ConnectorChangedProperty)
                            If fcp IsNot Nothing Then
                                If fcp.ChangedProperties.ContainsKey(PLACEMENT) Then
                                    e.Data = InformationHubStrings.PlacementModified
                                End If
                            End If
                        End If
                    End If

                Case Else
                    OnUnhandledCellDataRequested(e)
            End Select
        End With
    End Sub

    Private Sub GetCoreWireNumberCellValueForCavity(ByVal e As CellDataRequestedEventArgs, ByVal contactPoint As Contact_point)
        If (contactPoint IsNot Nothing) Then
            Dim mapper As KblMapper = Nothing
            If _kblMapper.KBLOccurrenceMapper.Values.Contains(contactPoint) Then
                mapper = _kblMapper
            ElseIf _kblMapperForCompare IsNot Nothing AndAlso _kblMapperForCompare.KBLOccurrenceMapper.Values.Contains(contactPoint) Then
                mapper = _kblMapperForCompare
            End If
            If mapper IsNot Nothing AndAlso (mapper.KBLContactPointWireMapper.ContainsKey(contactPoint.SystemId)) Then
                For Each wireId As String In mapper.KBLContactPointWireMapper(contactPoint.SystemId)
                    If (TypeOf mapper.KBLOccurrenceMapper(wireId) Is Core_occurrence) Then
                        If (e.Data IsNot Nothing) Then
                            e.Data = String.Format("{0}, {1}", e.Data, DirectCast(mapper.KBLOccurrenceMapper(wireId), Core_occurrence).Wire_number)
                        Else
                            e.Data = DirectCast(mapper.KBLOccurrenceMapper(wireId), Core_occurrence).Wire_number
                        End If
                    ElseIf (TypeOf mapper.KBLOccurrenceMapper(wireId) Is Wire_occurrence) Then
                        If (e.Data IsNot Nothing) Then
                            e.Data = String.Format("{0}, {1}", e.Data, DirectCast(mapper.KBLOccurrenceMapper(wireId), Wire_occurrence).Wire_number)
                        Else
                            e.Data = DirectCast(mapper.KBLOccurrenceMapper(wireId), Wire_occurrence).Wire_number
                        End If
                    End If
                Next
            End If
        End If
    End Sub

    Private Sub GetCoreWireCrossSectionCellValueForCavity(ByVal e As CellDataRequestedEventArgs, ByVal contactPoint As Contact_point)
        If (contactPoint IsNot Nothing) AndAlso (_kblMapper.KBLContactPointWireMapper.ContainsKey(contactPoint.SystemId)) Then
            For Each wireId As String In _kblMapper.KBLContactPointWireMapper(contactPoint.SystemId)
                If (TypeOf _kblMapper.KBLOccurrenceMapper(wireId) Is Core_occurrence) Then
                    Dim corOcc As Core_occurrence = CType(_kblMapper.KBLOccurrenceMapper(wireId), Core_occurrence)
                    Dim core As Core = TryCast(_kblMapper.KBLPartMapper(corOcc.Part), Core)

                    If (core IsNot Nothing) AndAlso (core.Cross_section_area IsNot Nothing) Then
                        Dim value As String
                        If (_kblMapper.KBLUnitMapper.ContainsKey(core.Cross_section_area.Unit_component)) Then
                            value = String.Format("{0} {1}", Math.Round(core.Cross_section_area.Value_component, 2), _kblMapper.KBLUnitMapper(core.Cross_section_area.Unit_component).Unit_name)
                        Else
                            value = Math.Round(core.Cross_section_area.Value_component, 2).ToString
                        End If
                        If (e.Data IsNot Nothing) Then
                            If Not e.Data.Equals(value) Then
                                e.Data = String.Format("{0}, {1}", e.Data, value)
                            End If
                        Else
                            e.Data = value
                        End If
                    End If

                ElseIf (TypeOf _kblMapper.KBLOccurrenceMapper(wireId) Is Wire_occurrence) Then
                    Dim wocc As Wire_occurrence = CType(_kblMapper.KBLOccurrenceMapper(wireId), Wire_occurrence)
                    If (_kblMapper.KBLPartMapper.ContainsKey(If(wocc.Part Is Nothing, String.Empty, wocc.Part))) Then
                        Dim wirePart As General_wire = DirectCast(_kblMapper.KBLPartMapper(wocc.Part), General_wire)
                        If (wirePart IsNot Nothing) AndAlso (wirePart.Cross_section_area IsNot Nothing) Then
                            Dim value As String

                            If (_kblMapper.KBLUnitMapper.ContainsKey(wirePart.Cross_section_area.Unit_component)) Then
                                value = String.Format("{0} {1}", Math.Round(wirePart.Cross_section_area.Value_component, 2), _kblMapper.KBLUnitMapper(wirePart.Cross_section_area.Unit_component).Unit_name)
                            Else
                                value = Math.Round(wirePart.Cross_section_area.Value_component, 2).ToString
                            End If

                            If (e.Data IsNot Nothing) Then
                                If Not e.Data.Equals(value) Then
                                    e.Data = String.Format("{0}, {1}", e.Data, value)
                                End If
                            Else
                                e.Data = value
                            End If
                        End If
                    End If
                End If
            Next
        End If
    End Sub

    Private Sub GetCoreWireTypeCellValueForCavity(ByVal e As CellDataRequestedEventArgs, ByVal contactPoint As Contact_point)
        If (contactPoint IsNot Nothing) AndAlso (_kblMapper.KBLContactPointWireMapper.ContainsKey(contactPoint.SystemId)) Then
            For Each wireId As String In _kblMapper.KBLContactPointWireMapper(contactPoint.SystemId)
                If (TypeOf _kblMapper.KBLOccurrenceMapper(wireId) Is Core_occurrence) Then
                    Dim corOcc As Core_occurrence = CType(_kblMapper.KBLOccurrenceMapper(wireId), Core_occurrence)
                    Dim core As Core = TryCast(_kblMapper.KBLPartMapper(corOcc.Part), Core)
                    If (core IsNot Nothing) AndAlso (core.Wire_type IsNot Nothing) Then
                        If (e.Data IsNot Nothing) Then
                            If Not e.Data.Equals(core.Wire_type) Then
                                e.Data = String.Format("{0}, {1}", e.Data, core.Wire_type)
                            End If
                        Else
                            e.Data = core.Wire_type
                        End If
                    End If
                ElseIf (TypeOf _kblMapper.KBLOccurrenceMapper(wireId) Is Wire_occurrence) Then
                    Dim wocc As Wire_occurrence = CType(_kblMapper.KBLOccurrenceMapper(wireId), Wire_occurrence)
                    If (_kblMapper.KBLPartMapper.ContainsKey(If(wocc.Part Is Nothing, String.Empty, wocc.Part))) Then
                        Dim wirePart As General_wire = DirectCast(_kblMapper.KBLPartMapper(wocc.Part), General_wire)
                        If (wirePart IsNot Nothing) AndAlso (wirePart.Wire_type IsNot Nothing) Then
                            If (e.Data IsNot Nothing) Then
                                If Not e.Data.Equals(wirePart.Wire_type) Then
                                    e.Data = String.Format("{0}, {1}", e.Data, wirePart.Wire_type)
                                End If
                            Else
                                e.Data = wirePart.Wire_type
                            End If
                        End If
                    End If
                End If
            Next
        End If
    End Sub

    Private Sub GetCoreWireColorCellValueForCavity(ByVal e As CellDataRequestedEventArgs, ByVal contactPoint As Contact_point)
        If (contactPoint IsNot Nothing) AndAlso (_kblMapper.KBLContactPointWireMapper.ContainsKey(contactPoint.SystemId)) Then
            For Each wireId As String In _kblMapper.KBLContactPointWireMapper(contactPoint.SystemId)
                If (TypeOf _kblMapper.KBLOccurrenceMapper(wireId) Is Core_occurrence) Then
                    Dim corOcc As Core_occurrence = CType(_kblMapper.KBLOccurrenceMapper(wireId), Core_occurrence)
                    Dim core As Core = TryCast(_kblMapper.KBLPartMapper(corOcc.Part), Core)
                    If (core IsNot Nothing) AndAlso (core.GetColours IsNot Nothing) Then
                        If (e.Data IsNot Nothing) Then
                            If Not e.Data.Equals(core.GetColours) Then
                                e.Data = String.Format("{0}, {1}", e.Data, core.GetColours)
                            End If
                        Else
                            e.Data = core.GetColours
                        End If
                    End If
                ElseIf (TypeOf _kblMapper.KBLOccurrenceMapper(wireId) Is Wire_occurrence) Then
                    Dim wocc As Wire_occurrence = CType(_kblMapper.KBLOccurrenceMapper(wireId), Wire_occurrence)
                    If (_kblMapper.KBLPartMapper.ContainsKey(If(wocc.Part Is Nothing, String.Empty, wocc.Part))) Then
                        Dim wirePart As General_wire = DirectCast(_kblMapper.KBLPartMapper(wocc.Part), General_wire)
                        If (wirePart IsNot Nothing) AndAlso (wirePart.GetColours IsNot Nothing) Then
                            If (e.Data IsNot Nothing) Then
                                If Not e.Data.Equals(wirePart.GetColours) Then
                                    e.Data = String.Format("{0}, {1}", e.Data, wirePart.GetColours)
                                End If
                            Else
                                e.Data = wirePart.GetColours
                            End If
                        End If
                    End If
                End If
            Next
        End If
    End Sub

    Private Sub ShowDetailInformationForCavity(ByVal e As CellEventArgs, ByVal fromReference As Boolean)
        Dim cavityOccurrence As Cavity_occurrence = TryCast(e.Cell.Tag, Cavity_occurrence)
        If (cavityOccurrence Is Nothing) Then
            cavityOccurrence = DirectCast(If(fromReference, _kblMapper.KBLOccurrenceMapper(DirectCast(e.Cell.Tag, Contact_point).Contacted_cavity), _kblMapperForCompare.KBLOccurrenceMapper(DirectCast(e.Cell.Tag, Contact_point).Contacted_cavity)), Cavity_occurrence)
        End If

        If (cavityOccurrence Is Nothing) Then
            Exit Sub
        End If

        Dim occId As String = Nothing
        Dim plugPart As Cavity_plug = Nothing
        Dim sealPart As Cavity_seal = Nothing
        Dim terminalPart As General_terminal = Nothing

        If (fromReference) Then
            If (cavityOccurrence.Associated_plug IsNot Nothing) Then
                If (e.Cell.Column.Key = ConnectorPropertyName.Plug_part_information) Then
                    occId = cavityOccurrence.Associated_plug
                End If

                plugPart = DirectCast(_kblMapper.KBLPartMapper(DirectCast(_kblMapper.KBLOccurrenceMapper(cavityOccurrence.Associated_plug), Cavity_plug_occurrence).Part), Cavity_plug)
            End If

            For Each contactPoint As Contact_point In DirectCast(_kblMapper.KBLOccurrenceMapper(_kblMapper.KBLCavityConnectorMapper(cavityOccurrence.SystemId)), Connector_occurrence).Contact_points
                If (contactPoint.Contacted_cavity.SplitSpace.Contains(cavityOccurrence.SystemId)) AndAlso (contactPoint.Associated_parts IsNot Nothing) Then
                    For Each associatePart As String In contactPoint.Associated_parts.SplitSpace
                        If (sealPart Is Nothing) AndAlso (TypeOf _kblMapper.KBLOccurrenceMapper(associatePart) Is Cavity_seal_occurrence) Then
                            Dim part As Cavity_seal = DirectCast(_kblMapper.KBLPartMapper(DirectCast(_kblMapper.KBLOccurrenceMapper(associatePart), Cavity_seal_occurrence).Part), Cavity_seal)

                            If (e.Cell.Row.Cells(ConnectorPropertyName.Seal_part_number).Value.ToString = part.Part_number) Then
                                If (e.Cell.Column.Key = ConnectorPropertyName.Seal_part_information) Then
                                    occId = associatePart
                                End If

                                sealPart = part
                            End If
                        ElseIf (terminalPart Is Nothing) AndAlso (TypeOf _kblMapper.KBLOccurrenceMapper(associatePart) Is Special_terminal_occurrence) Then
                            Dim part As General_terminal = DirectCast(_kblMapper.KBLPartMapper(DirectCast(_kblMapper.KBLOccurrenceMapper(associatePart), Special_terminal_occurrence).Part), General_terminal)

                            If (e.Cell.Row.Cells(ConnectorPropertyName.Terminal_part_number).Value.ToString = part.Part_number) Then
                                If (e.Cell.Column.Key = ConnectorPropertyName.Terminal_part_information) Then
                                    occId = associatePart
                                End If

                                terminalPart = part
                            End If
                        ElseIf (terminalPart Is Nothing) AndAlso (TypeOf _kblMapper.KBLOccurrenceMapper(associatePart) Is Terminal_occurrence) Then
                            Dim part As General_terminal = DirectCast(_kblMapper.KBLPartMapper(DirectCast(_kblMapper.KBLOccurrenceMapper(associatePart), Terminal_occurrence).Part), General_terminal)

                            If (e.Cell.Row.Cells(ConnectorPropertyName.Terminal_part_number).Value.ToString = part.Part_number) Then
                                If (e.Cell.Column.Key = ConnectorPropertyName.Terminal_part_information.ToString) Then
                                    occId = associatePart
                                End If

                                terminalPart = part
                            End If
                        End If
                    Next
                End If
            Next
        Else
            If (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLOccurrenceMapper.ContainsKey(cavityOccurrence.SystemId)) Then
                If (cavityOccurrence.Associated_plug IsNot Nothing) Then
                    If (e.Cell.Column.Key = ConnectorPropertyName.Plug_part_information) Then
                        occId = cavityOccurrence.Associated_plug
                    End If
                    plugPart = DirectCast(_kblMapperForCompare.KBLPartMapper(DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(cavityOccurrence.Associated_plug), Cavity_plug_occurrence).Part), Cavity_plug)
                End If

                For Each contactPoint As Contact_point In DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(_kblMapperForCompare.KBLCavityConnectorMapper(cavityOccurrence.SystemId)), Connector_occurrence).Contact_points
                    If (contactPoint.Contacted_cavity.SplitSpace.Contains(cavityOccurrence.SystemId)) AndAlso (contactPoint.Associated_parts IsNot Nothing) Then
                        For Each associatePart As String In contactPoint.Associated_parts.SplitSpace
                            If (sealPart Is Nothing) AndAlso (TypeOf _kblMapperForCompare.KBLOccurrenceMapper(associatePart) Is Cavity_seal_occurrence) Then
                                Dim part As Cavity_seal = DirectCast(_kblMapperForCompare.KBLPartMapper(DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(associatePart), Cavity_seal_occurrence).Part), Cavity_seal)

                                If (e.Cell.Row.Cells(ConnectorPropertyName.Seal_part_number).Value.ToString = part.Part_number) Then
                                    If (e.Cell.Column.Key = ConnectorPropertyName.Seal_part_information.ToString) Then
                                        occId = associatePart
                                    End If

                                    sealPart = part
                                End If
                            ElseIf (terminalPart Is Nothing) AndAlso (TypeOf _kblMapperForCompare.KBLOccurrenceMapper(associatePart) Is Special_terminal_occurrence) Then
                                Dim part As General_terminal = DirectCast(_kblMapperForCompare.KBLPartMapper(DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(associatePart), Special_terminal_occurrence).Part), General_terminal)

                                If (e.Cell.Row.Cells(ConnectorPropertyName.Terminal_part_number).Value.ToString = part.Part_number) Then
                                    If (e.Cell.Column.Key = ConnectorPropertyName.Terminal_part_information) Then
                                        occId = associatePart
                                    End If

                                    terminalPart = part
                                End If
                            ElseIf (terminalPart Is Nothing) AndAlso (TypeOf _kblMapperForCompare.KBLOccurrenceMapper(associatePart) Is Terminal_occurrence) Then
                                Dim part As General_terminal = DirectCast(_kblMapperForCompare.KBLPartMapper(DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(associatePart), Terminal_occurrence).Part), General_terminal)

                                If (e.Cell.Row.Cells(ConnectorPropertyName.Terminal_part_number).Value.ToString = part.Part_number) Then
                                    If (e.Cell.Column.Key = ConnectorPropertyName.Terminal_part_information) Then
                                        occId = associatePart
                                    End If

                                    terminalPart = part
                                End If
                            End If
                        Next
                    End If
                Next
            End If
        End If

        With cavityOccurrence
            Select Case e.Cell.Column.Key
                Case ConnectorPropertyName.Plug_part_information
                    If (plugPart IsNot Nothing) Then
                        If (fromReference) Then
                            ShowDetailInformationForm(InformationHubStrings.PlugPart_Caption, plugPart, Nothing, _kblMapper, occId)
                        ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) Then
                            ShowDetailInformationForm(InformationHubStrings.PlugPart_Caption, plugPart, Nothing, _kblMapperForCompare, occId)
                        End If
                    End If
                Case ConnectorPropertyName.Seal_part_information
                    If (sealPart IsNot Nothing) Then
                        If (fromReference) Then
                            ShowDetailInformationForm(InformationHubStrings.SealPart_Caption, sealPart, Nothing, _kblMapper, occId)
                        ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) Then
                            ShowDetailInformationForm(InformationHubStrings.SealPart_Caption, sealPart, Nothing, _kblMapperForCompare, occId)
                        End If
                    End If
                Case ConnectorPropertyName.Terminal_part_information
                    If (terminalPart IsNot Nothing) Then
                        If (fromReference) Then
                            ShowDetailInformationForm(InformationHubStrings.TerminalPart_Caption, terminalPart, Nothing, _kblMapper, occId)
                        ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) Then
                            ShowDetailInformationForm(InformationHubStrings.TerminalPart_Caption, terminalPart, Nothing, _kblMapperForCompare, occId)
                        End If
                    End If
            End Select
        End With
    End Sub

    Private Sub ShowDetailInformationForConnector(ByVal e As CellEventArgs, ByVal fromReference As Boolean)
        With DirectCast(e.Cell.Tag, Connector_occurrence)
            If (fromReference) AndAlso (_kblMapper.KBLPartMapper.ContainsKey(If(.Part Is Nothing, String.Empty, .Part))) Then
                RequestDialogPartData(_kblMapper, e.Cell.Column.Key, DirectCast(_kblMapper.KBLPartMapper(.Part), Connector_housing))
            ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLPartMapper.ContainsKey(If(.Part Is Nothing, String.Empty, .Part))) Then
                RequestDialogPartData(_kblMapperForCompare, e.Cell.Column.Key, DirectCast(_kblMapperForCompare.KBLPartMapper(.Part), Connector_housing))
            End If

            Select Case e.Cell.Column.Key
                Case ConnectorPropertyName.Alias_id
                    ShowDetailInformationForm(InformationHubStrings.AliasIds_Caption, .Alias_id, Nothing, _kblMapper)
                Case ConnectorPropertyName.Installation_Information
                    ShowDetailInformationForm(InformationHubStrings.InstallInfo_Caption, .Installation_information, Nothing, _kblMapper)
                Case ConnectorPropertyName.Localized_description
                    ShowDetailInformationForm(InformationHubStrings.LocDescription_Caption, .Localized_description, Nothing, _kblMapper)
                Case ConnectorPropertyName.Reference_element
                    Dim referencedElements As New List(Of String)
                    For Each referencedElement As String In .Reference_element.SplitSpace
                        If (fromReference) AndAlso (_kblMapper.KBLOccurrenceMapper.ContainsKey(referencedElement)) Then
                            referencedElements.Add(Harness.GetReferenceElement(referencedElement, _kblMapper))
                        ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLOccurrenceMapper.ContainsKey(referencedElement)) Then
                            referencedElements.Add(Harness.GetReferenceElement(referencedElement, _kblMapperForCompare))
                        End If
                    Next

                    ShowDetailInformationForm(InformationHubStrings.RefElement_Caption, referencedElements, Nothing, _kblMapper)
            End Select
        End With
    End Sub

    Private Sub udsConnectors_CellDataRequested(sender As Object, e As CellDataRequestedEventArgs) Handles udsConnectors.CellDataRequested
        e.CacheData = False

        If (e.Row.ParentRow Is Nothing) Then
            ConnectorCellDataRequested(e)
        Else
            CavityCellDataRequested(e)
        End If
    End Sub

    Private Function GetChangedMapperWithInverse(type As CompareChangeType) As KblMapper
        Select Case type
            Case CompareChangeType.Modified
                Return _kblMapper
            Case CompareChangeType.New
                Return If(_generalSettings.InverseCompare, _kblMapper, _kblMapperForCompare)
            Case CompareChangeType.Deleted
                Return If(_generalSettings.InverseCompare, _kblMapperForCompare, _kblMapper)
            Case Else
                Throw New NotImplementedException(GetType(CompareChangeType).Name & ": " & type.ToString)
        End Select
    End Function

    Private Sub udsConnectors_InitializeDataRow(sender As Object, e As InitializeDataRowEventArgs) Handles udsConnectors.InitializeDataRow
        If (e.Row.ParentRow Is Nothing) Then
            Dim connector As Connector_occurrence = Nothing
            Dim rowCount As Integer = 0

            If (e.Row.Band.Tag Is Nothing) Then
                connector = _kblMapper.GetConnectorOccurrences(e.Row.Index)

                _connectorCavityContactPointMapper.Add(connector.SystemId, New List(Of Tuple(Of Object, Tuple(Of String, String))))

                If (connector.Slots IsNot Nothing) AndAlso (connector.Slots.Length <> 0) Then
                    For Each cavityOccurence As Cavity_occurrence In connector.Slots(0).Cavities
                        Dim contactPoints As IEnumerable(Of Contact_point) = _kblMapper.GetContactPointsOfCavity(cavityOccurence.SystemId)
                        If (contactPoints IsNot Nothing) Then
                            For Each contactPoint As Contact_point In contactPoints
                                Dim terminalSealPartPairs As ICollection(Of Tuple(Of General_terminal, Cavity_seal)) = _kblMapper.GetTerminalSealPartPairsOfContactPoint(contactPoint, _currentRowFilterInfo.GetActiveObjectsOrNull)

                                If (terminalSealPartPairs.Count <> 0) Then
                                    For Each terminalSealPart As Tuple(Of General_terminal, Cavity_seal) In terminalSealPartPairs
                                        _connectorCavityContactPointMapper(connector.SystemId).Add(New Tuple(Of Object, Tuple(Of String, String))(contactPoint, New Tuple(Of String, String)(If(terminalSealPart.Item1 IsNot Nothing, terminalSealPart.Item1.Part_number, String.Empty), If(terminalSealPart.Item2 IsNot Nothing, terminalSealPart.Item2.Part_number, String.Empty))))
                                    Next
                                Else
                                    _connectorCavityContactPointMapper(connector.SystemId).Add(New Tuple(Of Object, Tuple(Of String, String))(contactPoint, Nothing))
                                End If

                                rowCount += If(terminalSealPartPairs.Count <> 0, terminalSealPartPairs.Count, 1)
                            Next
                        Else
                            _connectorCavityContactPointMapper(connector.SystemId).Add(New Tuple(Of Object, Tuple(Of String, String))(cavityOccurence, Nothing))
                            rowCount += 1
                        End If
                    Next
                End If
            Else
                Dim compareObjKey As String = DirectCast(e.Row.Band.Tag, Dictionary(Of String, Object)).Keys.ElementAt(e.Row.Index)
                Dim compareObjValue As Object = DirectCast(e.Row.Band.Tag, Dictionary(Of String, Object)).Values.ElementAt(e.Row.Index)

                If (TypeOf compareObjValue Is Connector_occurrence) Then
                    connector = DirectCast(compareObjValue, Connector_occurrence)
                Else
                    connector = DirectCast(_kblMapper.KBLOccurrenceMapper(ExtractSystemId(compareObjKey)), Connector_occurrence)
                End If

                Dim compareObjects As New Dictionary(Of String, Object)

                If (TypeOf compareObjValue Is ChangedProperty) Then
                    Dim changeProperty As ChangedProperty = DirectCast(compareObjValue, ConnectorChangedProperty)
                    Dim changedCavities As New Dictionary(Of String, List(Of String))

                    If (changeProperty.ChangedProperties.ContainsKey(NameOf(Connector_occurrence.Contact_points))) Then
                        Dim comparisonMapper As ContactPointComparisonMapper = DirectCast(changeProperty.ChangedProperties(NameOf(Connector_occurrence.Contact_points)), ContactPointComparisonMapper)
                        Dim addCavChange As Action(Of KblMapper, ChangedItem, Nullable(Of KeyValuePair(Of String, Object))) =
                            Sub(mapper As KblMapper, change As ChangedItem, cProp As Nullable(Of KeyValuePair(Of String, Object)))
                                If cProp.HasValue Then
                                    compareObjects.Add(String.Format("{0}|{1}|{2}", change.Text, change.Key, cProp.Value.Key), cProp.Value.Value)
                                Else
                                    compareObjects.Add(String.Format("{0}|{1}", change.Text, change.Key), change.Item)
                                End If
                                If change.Change = CompareChangeType.Modified Then
                                    If (Not changedCavities.ContainsKey(DirectCast(mapper.KBLOccurrenceMapper(change.Key), Contact_point).Contacted_cavity)) Then
                                        changedCavities.Add(DirectCast(mapper.KBLOccurrenceMapper(change.Key), Contact_point).Contacted_cavity, New List(Of String))
                                    End If

                                    changedCavities(DirectCast(mapper.KBLOccurrenceMapper(change.Key), Contact_point).Contacted_cavity).Add(compareObjects.LastOrDefault.Key)
                                End If
                            End Sub

                        For Each change As ChangedItem In comparisonMapper.Changes
                            Dim mapperToUse As KblMapper = GetChangedMapperWithInverse(change.Change)
                            Select Case change.Change
                                Case CompareChangeType.Modified
                                    For Each changedProperty As KeyValuePair(Of String, Object) In CType(change.Item, ChangedProperty).ChangedProperties
                                        addCavChange.Invoke(mapperToUse, change, changedProperty)
                                    Next
                                Case Else
                                    addCavChange.Invoke(mapperToUse, change, Nothing)
                            End Select
                        Next
                    End If

                    If (changeProperty.ChangedProperties.ContainsKey(ConnectorPropertyName.Slots)) Then
                        Dim comparisonMapper As CavityComparisonMapper = DirectCast(changeProperty.ChangedProperties(ConnectorPropertyName.Slots), CavityComparisonMapper)

                        For Each cItem As ChangedItem In comparisonMapper.Changes
                            If (changedCavities.ContainsKey(cItem.Key)) Then
                                For Each cavCompareKey As String In changedCavities(cItem.Key)
                                    Dim val As Object = If(compareObjects.ContainsKey(cavCompareKey), compareObjects(cavCompareKey), Nothing)
                                    If (val Is Nothing) Then
                                        compareObjects.Add(String.Format("{0}${1}", String.Format("{0}|{1}", cItem.Text, cItem.Key), cavCompareKey), New Tuple(Of Object, Object)(cItem.Item, Nothing))
                                    Else
                                        compareObjects.Remove(cavCompareKey)
                                        compareObjects.Add(String.Format("{0}${1}", String.Format("{0}|{1}", cItem.Text, cItem.Key), cavCompareKey), New Tuple(Of Object, Object)(cItem.Item, val))
                                    End If
                                Next
                            Else
                                compareObjects.Add(String.Format("{0}|{1}", cItem.Text, cItem.Key), cItem.Item)
                            End If
                        Next
                    End If
                End If

                rowCount = compareObjects.Count
                If e.Row.Band.ChildBands.Count > 0 Then
                    e.Row.GetChildRows(0).Tag = compareObjects
                End If
                SetDiffTypeCellText(e.Row, ChangedItem.ParseFromText(ExtractChangeText(compareObjKey)))
            End If

            e.Row.Tag = connector
            If e.Row.Band.ChildBands.Count > 0 Then
                e.Row.GetChildRows(0).SetCount(rowCount)
            End If
        Else
            If (e.Row.ParentCollection.Tag Is Nothing) Then
                e.Row.Tag = _connectorCavityContactPointMapper(DirectCast(e.Row.ParentRow.Tag, Connector_occurrence).SystemId)(e.Row.Index)
            Else
                Dim compareObjKey As String = DirectCast(e.Row.ParentCollection.Tag, Dictionary(Of String, Object)).Keys.ElementAt(e.Row.Index)
                Dim compareObjValue As Object = DirectCast(e.Row.ParentCollection.Tag, Dictionary(Of String, Object)).Values.ElementAt(e.Row.Index)

                If (TypeOf compareObjValue Is Cavity_occurrence) Then
                    e.Row.Tag = New Tuple(Of Object, Tuple(Of String, String))(compareObjValue, New Tuple(Of String, String)(String.Empty, String.Empty))
                ElseIf (TypeOf compareObjValue Is Contact_point) Then
                    Dim contactPoint As Contact_point = DirectCast(compareObjValue, Contact_point)
                    Dim kblMapper As KblMapper = If(IsAddedDiffType(compareObjKey) AndAlso Not _generalSettings.InverseCompare, _kblMapperForCompare, _kblMapper)
                    Dim terminalSealPartPairs As ICollection(Of Tuple(Of General_terminal, Cavity_seal)) = kblMapper.GetTerminalSealPartPairsOfContactPoint(contactPoint, _currentRowFilterInfo.GetActiveObjectsOrNull)

                    e.Row.Tag = New Tuple(Of Object, Tuple(Of String, String))(contactPoint, New Tuple(Of String, String)(If(terminalSealPartPairs.FirstOrDefault IsNot Nothing AndAlso terminalSealPartPairs.FirstOrDefault.Item1 IsNot Nothing, terminalSealPartPairs.FirstOrDefault.Item1.Part_number, String.Empty), If(terminalSealPartPairs.FirstOrDefault IsNot Nothing AndAlso terminalSealPartPairs.FirstOrDefault.Item2 IsNot Nothing, terminalSealPartPairs.FirstOrDefault.Item2.Part_number, String.Empty)))
                ElseIf (TypeOf compareObjValue Is CavityChangedProperty) Then
                    Dim cavityOccId As String = ExtractSystemId(compareObjKey)
                    Dim contactPoint As Contact_point = If(IsAddedDiffType(compareObjKey) AndAlso Not _generalSettings.InverseCompare, If(_kblMapperForCompare.KBLCavityContactPointMapper.ContainsKey(cavityOccId), DirectCast(_kblMapperForCompare.KBLCavityContactPointMapper(cavityOccId).FirstOrDefault, Contact_point), Nothing), If(_kblMapper.KBLCavityContactPointMapper.ContainsKey(cavityOccId), DirectCast(_kblMapper.KBLCavityContactPointMapper(cavityOccId).FirstOrDefault, Contact_point), Nothing))
                    Dim kblMapper As KblMapper = If(IsAddedDiffType(compareObjKey) AndAlso Not _generalSettings.InverseCompare, _kblMapperForCompare, _kblMapper)
                    Dim terminalSealPartPairs As ICollection(Of Tuple(Of General_terminal, Cavity_seal)) = kblMapper.GetTerminalSealPartPairsOfContactPoint(contactPoint, _currentRowFilterInfo.GetActiveObjectsOrNull)

                    e.Row.Tag = New Tuple(Of Object, Tuple(Of String, String))(DirectCast(_kblMapper.KBLOccurrenceMapper(cavityOccId), Cavity_occurrence), New Tuple(Of String, String)(If(terminalSealPartPairs.FirstOrDefault IsNot Nothing AndAlso terminalSealPartPairs.FirstOrDefault.Item1 IsNot Nothing, terminalSealPartPairs.FirstOrDefault.Item1.Part_number, String.Empty), If(terminalSealPartPairs.FirstOrDefault IsNot Nothing AndAlso terminalSealPartPairs.FirstOrDefault.Item2 IsNot Nothing, terminalSealPartPairs.FirstOrDefault.Item2.Part_number, String.Empty)))
                ElseIf (TypeOf compareObjValue Is ContactPointChangedProperty) Then
                    Dim contactPoint As Contact_point = If(IsAddedDiffType(compareObjKey) AndAlso Not _generalSettings.InverseCompare, DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(ExtractSystemId(compareObjKey)), Contact_point), DirectCast(_kblMapper.KBLOccurrenceMapper(ExtractSystemId(compareObjKey)), Contact_point))
                    Dim kblMapper As KblMapper = If(IsAddedDiffType(compareObjKey) AndAlso Not _generalSettings.InverseCompare, _kblMapperForCompare, _kblMapper)
                    Dim terminalSealPartPairs As ICollection(Of Tuple(Of General_terminal, Cavity_seal)) = kblMapper.GetTerminalSealPartPairsOfContactPoint(contactPoint, _currentRowFilterInfo.GetActiveObjectsOrNull)

                    e.Row.Tag = New Tuple(Of Object, Tuple(Of String, String))(contactPoint, New Tuple(Of String, String)(If(terminalSealPartPairs.FirstOrDefault IsNot Nothing AndAlso terminalSealPartPairs.FirstOrDefault.Item1 IsNot Nothing, terminalSealPartPairs.FirstOrDefault.Item1.Part_number, String.Empty), If(terminalSealPartPairs.FirstOrDefault IsNot Nothing AndAlso terminalSealPartPairs.FirstOrDefault.Item2 IsNot Nothing, terminalSealPartPairs.FirstOrDefault.Item2.Part_number, String.Empty)))
                ElseIf (compareObjKey.Contains("$"c)) Then
                    Dim changedContactPointKey As String = compareObjKey.Split("$".ToCharArray, StringSplitOptions.RemoveEmptyEntries)(1)
                    Dim contactPoint As Contact_point = If(IsAddedDiffType(changedContactPointKey) AndAlso Not _generalSettings.InverseCompare, DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(ExtractSystemId(changedContactPointKey)), Contact_point), DirectCast(_kblMapper.KBLOccurrenceMapper(ExtractSystemId(changedContactPointKey)), Contact_point))

                    If (changedContactPointKey.Contains("#"c)) Then
                        e.Row.Tag = New Tuple(Of Object, Tuple(Of String, String))(contactPoint, New Tuple(Of String, String)(changedContactPointKey.SplitRemoveEmpty("|"c)(2).Split("#".ToCharArray)(0), changedContactPointKey.SplitRemoveEmpty("|"c)(2).Split("#".ToCharArray)(1)))
                    Else
                        e.Row.Tag = New Tuple(Of Object, Tuple(Of String, String))(contactPoint, New Tuple(Of String, String)(String.Empty, String.Empty))
                    End If
                ElseIf (compareObjKey.Contains("#"c)) Then
                    Dim contactPoint As Contact_point = If(IsAddedDiffType(compareObjKey) AndAlso Not _generalSettings.InverseCompare, DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(ExtractSystemId(compareObjKey)), Contact_point), DirectCast(_kblMapper.KBLOccurrenceMapper(ExtractSystemId(compareObjKey)), Contact_point))

                    e.Row.Tag = New Tuple(Of Object, Tuple(Of String, String))(contactPoint, New Tuple(Of String, String)(compareObjKey.SplitRemoveEmpty("|"c)(2).Split("#".ToCharArray)(0), compareObjKey.SplitRemoveEmpty("|"c)(2).Split("#".ToCharArray)(1)))
                End If

                SetDiffCellValueFromObjectKey(e.Row, compareObjKey)
            End If
        End If
    End Sub

    Private Sub ugConnectors_AfterRowActivate(sender As Object, e As EventArgs) Handles ugConnectors.AfterRowActivate
        If (Me.ugConnectors.ActiveRow.Appearance.BackColor = Color.FromArgb(190, 190, 190)) Then
            Me.ugConnectors.ActiveRow = Nothing
        ElseIf (_kblMapperForCompare IsNot Nothing) Then
            If (SetInformationHubEventArgs(Nothing, Me.ugConnectors.ActiveRow, Nothing)) Then
                OnHubSelectionChanged()
            End If

            If (Not Me.Focused) Then
                Me.Focus()
            End If
        End If
    End Sub

    Private Sub ugConnectors_AfterSelectChange(sender As Object, e As AfterSelectChangeEventArgs) Handles ugConnectors.AfterSelectChange
        With Me.ugConnectors
            .BeginUpdate()
            If (.ActiveRow IsNot Nothing) AndAlso (Not .ActiveRowScrollRegion.VisibleRows.Contains(.ActiveRow)) Then
                .ActiveRowScrollRegion.ScrollRowIntoView(.ActiveRow)
            End If
            .EndUpdate()

            If (SetInformationHubEventArgs(Nothing, Nothing, .Selected.Rows)) Then
                OnHubSelectionChanged()
            End If
        End With
    End Sub

    Private Sub ugConnectors_BeforeSelectChange(sender As Object, e As BeforeSelectChangeEventArgs) Handles ugConnectors.BeforeSelectChange
        ClearSelectedChildRowsInGrid(Me.ugConnectors)

        For Each row As UltraGridRow In e.NewSelections.Rows
            If (row.Appearance.BackColor = Color.FromArgb(190, 190, 190)) Then
                e.Cancel = True
            End If
        Next
    End Sub

    Private Sub ugConnectors_ClickCell(sender As Object, e As ClickCellEventArgs) Handles ugConnectors.ClickCell
        If (_pressedMouseButton = System.Windows.Forms.MouseButtons.Left) AndAlso (_pressedKey <> Keys.ControlKey AndAlso _pressedKey <> Keys.ShiftKey) AndAlso (_informationHubEventArgs IsNot Nothing) AndAlso (_kblMapperForCompare IsNot Nothing OrElse e.Cell.Row.Appearance.BackColor <> Color.FromArgb(190, 190, 190)) Then
            Dim prevIds As IEnumerable(Of String) = _informationHubEventArgs.ObjectIds

            SetInformationHubEventArgs(Nothing, e.Cell.Row, Nothing)

            If (prevIds IsNot Nothing) AndAlso (String.Join(";", prevIds) <> String.Join(";", _informationHubEventArgs.ObjectIds)) Then
                OnHubSelectionChanged()

                If (Not Me.Focused) Then
                    Me.Focus()
                End If
            End If
        ElseIf (_pressedMouseButton = System.Windows.Forms.MouseButtons.Right) AndAlso (_kblMapperForCompare Is Nothing) AndAlso (ChangeGridContextMenuVisibility(e.Cell.Row)) Then

            Dim index As Integer = _contextMenuGrid.Tools.IndexOf(InfoHubMenuToolKey.ShowSpliceProposals.ToString)
            Dim index2 As Integer = _contextMenuGrid.Tools.IndexOf(InfoHubMenuToolKey.ShowRubberlinesToCorrespondingConnectors.ToString)

            If Me.ActiveGrid.Selected.Rows.Count = 1 AndAlso Me.ActiveGrid.Selected.Rows(0) Is e.Cell.Row AndAlso (IsSplice(Me.ActiveGrid.Selected.Rows(0)) OrElse IsCavity(Me.ActiveGrid.Selected.Rows(0))) Then
                Dim activeRow As UltraGridRow = Me.ActiveGrid.Selected.Rows(0)

                If CType(_parentForm, DocumentForm)?.Document IsNot Nothing AndAlso CType(_parentForm, DocumentForm).IsDocument3DActive AndAlso Not CType(_parentForm, DocumentForm).IsAnalysisFormActive Then

                    If IsSplice(activeRow) Then
                        _contextMenuGrid.Tools.Item(index).SharedProps.Visible = True
                        If Not My.Application.MainForm.HasView3DFeature Then
                            _contextMenuGrid.Tools.Item(index).SharedProps.Enabled = False
                        Else
                            _contextMenuGrid.Tools.Item(index).SharedProps.Enabled = True
                        End If
                    Else
                        _contextMenuGrid.Tools.Item(index).SharedProps.Visible = False
                    End If

                    If IsCavity(activeRow) Then
                        _contextMenuGrid.Tools.Item(index2).SharedProps.Visible = True
                        If Not My.Application.MainForm.HasView3DFeature Then
                            _contextMenuGrid.Tools.Item(index2).SharedProps.Enabled = False
                        Else
                            _contextMenuGrid.Tools.Item(index2).SharedProps.Enabled = True
                        End If
                    Else
                        _contextMenuGrid.Tools.Item(index2).SharedProps.Visible = False
                    End If
                Else
                    _contextMenuGrid.Tools.Item(index).SharedProps.Visible = False
                    _contextMenuGrid.Tools.Item(index2).SharedProps.Visible = False
                End If
            End If

            _contextMenuGrid.ShowPopup()
            _contextMenuGrid.Tools.Item(index).SharedProps.Visible = False
            _contextMenuGrid.Tools.Item(index2).SharedProps.Visible = False
        End If
    End Sub

    Private Function IsCavity(row As UltraGridRow) As Boolean
        Dim res As Boolean = False
        If Not String.IsNullOrEmpty(row.Tag?.ToString) Then
            If TypeOf row.Tag Is List(Of String) Then
                Dim myIds As List(Of String) = CType(row.Tag, List(Of String))
                Dim id As String = myIds(0)
                If (TypeOf _kblMapper.KBLOccurrenceMapper(id) Is Cavity_occurrence) Then
                    res = True
                End If
            End If
        End If
        Return res
    End Function

    Private Function IsSplice(row As UltraGridRow) As Boolean
        Dim res As Boolean = False
        If Not String.IsNullOrEmpty(row.Tag?.ToString) Then
            If _kblMapper.KBLOccurrenceMapper.ContainsKey(row.Tag?.ToString) Then
                Dim cn As Connector_occurrence = CType(_kblMapper.KBLOccurrenceMapper.Item(row.Tag?.ToString), Connector_occurrence)
                If cn.UsageSpecified Then
                    If cn.Usage = Connector_usage.splice Then
                        res = True
                    End If
                End If
            End If
        End If
        Return res
    End Function

    Private Sub ugConnectors_ClickCellButton(sender As Object, e As CellEventArgs) Handles ugConnectors.ClickCellButton
        If (_pressedMouseButton = System.Windows.Forms.MouseButtons.Left) Then
            Dim fromReference As Boolean = True

            If HasChangeTypeWithInverse(CType(e.Cell.Row.ListObject, UltraDataRow), CompareChangeType.New) Then
                fromReference = False
            End If

            If (e.Cell.Row.ParentRow Is Nothing) AndAlso (TypeOf e.Cell.Tag Is Connector_occurrence) Then
                ShowDetailInformationForConnector(e, fromReference)
            ElseIf (TypeOf e.Cell.Tag Is Cavity_occurrence) OrElse (TypeOf e.Cell.Tag Is Contact_point) Then
                ShowDetailInformationForCavity(e, fromReference)
            ElseIf (TypeOf e.Cell.Tag Is KeyValuePair(Of String, Object)) Then
                Dim key As String = DirectCast(e.Cell.Row.Tag, KeyValuePair(Of String, Object)).Key

                If (key.Contains("$"c)) Then
                    If (e.Cell.Column.Key = ConnectorPropertyName.Plug_part_information) Then
                        RequestDialogCompareData(True, key.Split("$"c, StringSplitOptions.RemoveEmptyEntries)(0).Split("|"c, StringSplitOptions.RemoveEmptyEntries)(1), DirectCast(e.Cell.Tag, KeyValuePair(Of String, Object)))
                    Else
                        RequestDialogCompareData(True, key.Split("$"c, StringSplitOptions.RemoveEmptyEntries)(1).Split("|"c, StringSplitOptions.RemoveEmptyEntries)(1), DirectCast(e.Cell.Tag, KeyValuePair(Of String, Object)))
                    End If
                Else
                    RequestDialogCompareData(True, key.Split("|"c, StringSplitOptions.RemoveEmptyEntries)(1), DirectCast(e.Cell.Tag, KeyValuePair(Of String, Object)))
                End If
            End If
        End If
    End Sub

    Private Sub ugConnectors_DoubleClickRow(sender As Object, e As DoubleClickRowEventArgs) Handles ugConnectors.DoubleClickRow
        OnDoubleClickRow(sender, e)
    End Sub

    Private Sub ugConnectors_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugConnectors.InitializeLayout
        Me.ugConnectors.BeginUpdate()
        Me.ugConnectors.EventManager.AllEventsEnabled = False

        InitializeGridLayout(_connectorGridAppearance, e.Layout)

        Me.ugConnectors.EventManager.AllEventsEnabled = True
        Me.ugConnectors.EndUpdate()
    End Sub

    Private Sub ugConnectors_InitializeRow(sender As Object, e As InitializeRowEventArgs) Handles ugConnectors.InitializeRow
        If Not e.ReInitialize Then
            InitializeConnectorRowCore(e.Row)
        End If
    End Sub

    Private Sub InitializeConnectorRowCore(row As UltraGridRow)
        If (DirectCast(row.ListObject, UltraDataRow).Band.Tag Is Nothing) Then
            row.Tag = DirectCast(DirectCast(row.ListObject, UltraDataRow).Tag, Connector_occurrence).SystemId

            If (_kblMapper.KBLPartMapper.ContainsKey(DirectCast(DirectCast(row.ListObject, UltraDataRow).Tag, Connector_occurrence).Part)) AndAlso (DirectCast(_kblMapper.KBLPartMapper(DirectCast(DirectCast(row.ListObject, UltraDataRow).Tag, Connector_occurrence).Part), Connector_housing).IsKSL) Then
                row.Appearance.FontData.Italic = Infragistics.Win.DefaultableBoolean.True
            End If

            If _redliningInformation IsNot Nothing AndAlso _redliningInformation.Redlinings.Where(Function(redlining) redlining.ObjectId = row.Tag?.ToString).Any() Then
                row.Cells(0).Appearance.Image = My.Resources.Redlining
            End If
        Else
            InitializeRowForCompare(DirectCast(DirectCast(row.ListObject, UltraDataRow).Band.Tag, Dictionary(Of String, Object)), row)
        End If

        For Each cell As UltraGridCell In row.Cells
            If (cell.Value IsNot Nothing AndAlso cell.Value.ToString = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS) Then
                cell.Style = ColumnStyle.Button

                If (TypeOf row.Tag Is KeyValuePair(Of String, Object)) Then
                    Dim compareObject As KeyValuePair(Of String, Object) = DirectCast(row.Tag, KeyValuePair(Of String, Object))

                    If (TypeOf compareObject.Value Is ConnectorChangedProperty) Then
                        For Each changedProperty As KeyValuePair(Of String, Object) In DirectCast(compareObject.Value, ConnectorChangedProperty).ChangedProperties
                            If (changedProperty.Key = cell.Column.Key) Then
                                cell.Tag = changedProperty

                                Exit For
                            ElseIf (changedProperty.Key = NameOf(IKblOccurrence.Part)) Then
                                For Each partChangedProperty As KeyValuePair(Of String, Object) In DirectCast(changedProperty.Value, PartChangedProperty).ChangedProperties
                                    If (partChangedProperty.Key = cell.Column.Key) Then
                                        cell.Tag = partChangedProperty
                                        Exit For
                                    End If
                                Next
                            End If
                        Next
                    Else
                        cell.Tag = compareObject.Value
                    End If

                    If (cell.Tag Is Nothing) Then
                        cell.Style = ColumnStyle.Default
                        DirectCast(cell.Row.ListObject, UltraDataRow).SetCellValue(cell.Column.Key, String.Empty)
                    End If
                Else
                    cell.Tag = _kblMapper.KBLOccurrenceMapper(row.Tag?.ToString)
                End If
            End If
        Next

        Me.ugConnectors.EventManager.AllEventsEnabled = False

        If row.ChildBands?.Count > 0 Then
            For Each childRow As UltraGridRow In row.ChildBands(0).Rows
                Dim cavityOccurrence As Cavity_occurrence = Nothing
                Dim cavityParts As New List(Of String)
                Dim connectorOccurrence As Connector_occurrence = Nothing
                Dim contactPoint As Contact_point = Nothing

                If (_kblMapper.KBLOccurrenceMapper.ContainsKey(childRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString)) Then
                    cavityOccurrence = _kblMapper.GetOccurrenceObject(Of Cavity_occurrence)(childRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString)
                    contactPoint = _kblMapper.GetOccurrenceObject(Of Contact_point)(childRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString)

                    If (cavityOccurrence Is Nothing) Then
                        _kblMapper.GetCavityOfContactPointId(contactPoint.SystemId, cavityOccurrence)
                    End If

                    connectorOccurrence = _kblMapper.GetConnectorOfCavity(cavityOccurrence.SystemId)
                ElseIf (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLOccurrenceMapper.ContainsKey(childRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString)) Then
                    cavityOccurrence = _kblMapperForCompare.GetOccurrenceObject(Of Cavity_occurrence)(childRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString)
                    contactPoint = _kblMapperForCompare.GetOccurrenceObject(Of Contact_point)(childRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString)
                    If (cavityOccurrence Is Nothing) Then
                        _kblMapperForCompare.GetCavityOfContactPointId(contactPoint.SystemId, cavityOccurrence)
                    End If

                    connectorOccurrence = _kblMapperForCompare.GetConnectorOfCavity(cavityOccurrence.SystemId)
                End If

                cavityParts.TryAdd(cavityOccurrence.SystemId)

                If Not String.IsNullOrWhiteSpace(cavityOccurrence.Associated_plug) Then
                    cavityParts.TryAdd(cavityOccurrence.Associated_plug)
                End If

                If (contactPoint IsNot Nothing) Then
                    cavityParts.TryAdd(contactPoint.SystemId)

                    If Not String.IsNullOrWhiteSpace(contactPoint.Associated_parts) Then
                        cavityParts.AddRange(contactPoint.Associated_parts.SplitSpace)
                    Else
                        For Each wireCore As IKblWireCoreOccurrence In _kblMapper.GetWireOrCoresOfContactPoint(contactPoint.SystemId)
                            cavityParts.TryAdd(wireCore.SystemId)
                        Next
                    End If
                End If

                childRow.Tag = cavityParts

                InitializeConnectorCavityChildRowData(childRow) '? not clear why to initialize manually on a cavity-row with the connector-initialize-row-BL, seems to be unlogical ?? 
            Next
        End If
        Me.ugConnectors.EventManager.AllEventsEnabled = True
    End Sub

    Private Sub InitializeConnectorCavityChildRowData(child_row As UltraGridRow)
        If (DirectCast(child_row.ListObject, UltraDataRow).ParentCollection.Tag IsNot Nothing) Then
            InitializeRowForCompare(DirectCast(DirectCast(child_row.ListObject, UltraDataRow).ParentCollection.Tag, Dictionary(Of String, Object)), child_row)
        Else
            If _redliningInformation IsNot Nothing AndAlso _redliningInformation.Redlinings.Where(Function(redlining) redlining.ObjectId = child_row.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString).Any() Then
                child_row.Cells(1).Appearance.Image = My.Resources.Redlining
            End If
        End If

        For Each cell As UltraGridCell In child_row.Cells
            If (cell.Value IsNot Nothing) Then
                If (cell.Value.ToString = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS) Then
                    cell.Style = ColumnStyle.Button

                    If (TypeOf child_row.Tag Is KeyValuePair(Of String, Object)) Then
                        Dim compareObject As KeyValuePair(Of String, Object) = DirectCast(child_row.Tag, KeyValuePair(Of String, Object))

                        If (TypeOf compareObject.Value Is CavityChangedProperty) OrElse (TypeOf compareObject.Value Is ContactPointChangedProperty) Then
                            For Each changedProperty As KeyValuePair(Of String, Object) In If(TypeOf compareObject.Value Is CavityChangedProperty, DirectCast(compareObject.Value, CavityChangedProperty).ChangedProperties, DirectCast(compareObject.Value, ContactPointChangedProperty).ChangedProperties)
                                If (changedProperty.Key = cell.Column.Key) Then
                                    cell.Tag = changedProperty
                                    Exit For
                                End If
                            Next
                        ElseIf (TypeOf compareObject.Value Is Tuple(Of Object, Object)) Then
                            Dim key As String = String.Empty
                            Dim val1 As Object = Nothing
                            Dim val2 As Object = Nothing

                            If (compareObject.Key.Contains("$"c)) Then
                                Dim cavityChangedKey As String = compareObject.Key.Split("$"c, StringSplitOptions.RemoveEmptyEntries)(0)
                                Dim cavityChangedVal As Object = DirectCast(compareObject.Value, Tuple(Of Object, Object)).Item1

                                If (cell.Column.Key = ConnectorPropertyName.Plug_part_information) AndAlso (TypeOf cavityChangedVal Is CavityChangedProperty) Then
                                    For Each changedProperty As KeyValuePair(Of String, Object) In DirectCast(cavityChangedVal, CavityChangedProperty).ChangedProperties
                                        If (changedProperty.Key = cell.Column.Key) Then
                                            cell.Tag = changedProperty
                                            Exit For
                                        End If
                                    Next
                                End If

                                key = compareObject.Key.Split("$".ToCharArray, StringSplitOptions.RemoveEmptyEntries)(1)

                                If (TypeOf DirectCast(compareObject.Value, Tuple(Of Object, Object)).Item2 Is Tuple(Of Object, Object)) Then
                                    val1 = DirectCast(DirectCast(compareObject.Value, Tuple(Of Object, Object)).Item2, Tuple(Of Object, Object)).Item1
                                    val2 = DirectCast(DirectCast(compareObject.Value, Tuple(Of Object, Object)).Item2, Tuple(Of Object, Object)).Item2
                                ElseIf (TypeOf DirectCast(compareObject.Value, Tuple(Of Object, Object)).Item2 Is Contact_point) Then
                                    val1 = DirectCast(compareObject.Value, Tuple(Of Object, Object)).Item2
                                End If
                            ElseIf (compareObject.Key.Contains("#"c)) Then
                                key = compareObject.Key
                                val1 = DirectCast(compareObject.Value, Tuple(Of Object, Object)).Item1
                                val2 = DirectCast(compareObject.Value, Tuple(Of Object, Object)).Item2
                            End If

                            If (TypeOf val1 Is SingleObjectComparisonMapper) AndAlso (cell.Column.Key = ConnectorPropertyName.Terminal_part_information) Then
                                cell.Tag = New KeyValuePair(Of String, Object)(key.SplitRemoveEmpty("|"c)(2).Split("#".ToCharArray)(0), val1)
                            End If

                            If (val2 IsNot Nothing) AndAlso (TypeOf val2 Is SingleObjectComparisonMapper) AndAlso (cell.Column.Key = ConnectorPropertyName.Seal_part_information) Then
                                cell.Tag = New KeyValuePair(Of String, Object)(key.SplitRemoveEmpty("|"c)(2).Split("#".ToCharArray)(1), val2)
                            End If
                        Else
                            cell.Tag = compareObject.Value
                        End If

                        If (cell.Tag Is Nothing) Then
                            cell.Style = ColumnStyle.Default
                            DirectCast(cell.Row.ListObject, UltraDataRow).SetCellValue(cell.Column.Key, String.Empty)
                        End If
                    ElseIf (TypeOf child_row.Tag Is List(Of String)) Then
                        cell.Tag = _kblMapper.KBLOccurrenceMapper(DirectCast(child_row.Tag, List(Of String))(0))
                    End If
                End If
            End If
        Next
    End Sub

    Private Sub ugConnectors_KeyDown(sender As Object, e As KeyEventArgs) Handles ugConnectors.KeyDown
        OnGridKeyDown(Me, New GridKeyDownEventArgs(e, DirectCast(sender, UltraGrid)))
        If Not e.Handled Then
            If Not e.Handled Then
                If (e.Control) AndAlso (e.KeyCode = Keys.A) Then
                    SelectAllRowsOfActiveGrid()
                ElseIf (e.KeyCode = Keys.Escape) Then
                    If (Me.ugConnectors.Selected.Rows.Count = 0) Then
                        ClearMarkedRowsInGrids()
                    Else
                        ClearSelectedRowsInGrids()
                    End If

                    If (SetInformationHubEventArgs(Nothing, Nothing, Me.ugConnectors.Selected.Rows)) Then
                        OnHubSelectionChanged()
                    End If

                ElseIf (e.KeyCode = Keys.Return) Then
                    SetSelectedAsMarkedOriginRows(ugConnectors)
                End If
            End If
        End If
    End Sub

    Private Sub ugConnectors_MouseDown(sender As Object, e As MouseEventArgs) Handles ugConnectors.MouseDown
        If (_contextMenuGrid.IsOpen) Then
            _contextMenuGrid.ClosePopup()
        End If
        _pressedMouseButton = e.Button
    End Sub

    Private Sub ugConnectors_MouseLeave(sender As Object, e As EventArgs) Handles ugConnectors.MouseLeave
        _messageEventArgs.StatusMessage = String.Empty
        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub ugConnectors_MouseMove(sender As Object, e As MouseEventArgs) Handles ugConnectors.MouseMove
        _messageEventArgs.StatusMessage = String.Format(InformationHubStrings.RowCount_Label, Me.ugConnectors.Rows.Count, Me.ugConnectors.Rows.FilteredInNonGroupByRowCount, Me.ugConnectors.Rows.VisibleRowCount, Me.ugConnectors.Selected.Rows.Count)
        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub udsConnectors_CellDataUpdated(sender As Object, e As CellDataUpdatedEventArgs) Handles udsConnectors.CellDataUpdated
        RaiseEvent CellValueUpdated(sender, e)
    End Sub

End Class