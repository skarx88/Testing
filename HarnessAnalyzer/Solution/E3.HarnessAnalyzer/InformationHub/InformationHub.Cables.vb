Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinGrid
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Partial Public Class InformationHub

    Private Sub InitializeCables()
        _cableGridAppearance = GridAppearance.All.OfType(Of CableGridAppearance).Single
        AddNewRowFilters(ugCables)

        If (_comparisonMapperList Is Nothing) Then
            FillDataSource(Me.udsCables, _cableGridAppearance, _kblMapper.KBLCableList.Count)
        Else
            FillDataSource(Me.udsCables, _cableGridAppearance, 0, _comparisonMapperList(KblObjectType.Special_wire_occurrence))
        End If

        If (Me.udsCables.Band.Columns.Count = 0) OrElse (Me.udsCables.Rows.Count = 0) Then
            Me.utcInformationHub.Tabs(TabNames.tabCables.ToString).Visible = False
        End If
    End Sub

    Private Sub CoreCellDataRequested(ByVal e As CellDataRequestedEventArgs)
        If e.Row.Tag IsNot Nothing Then
            With DirectCast(e.Row.Tag, Core_occurrence)
                Dim connection As Connection = Nothing
                Dim core As Core = Nothing
                Dim fromReference As Boolean = True
                Dim wiringGroups As List(Of Wiring_group) = Nothing

                If HasChangeTypeWithInverse(e.Row, CompareChangeType.New) Then
                    fromReference = False

                    If (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLPartMapper.ContainsKey(If(.Part Is Nothing, String.Empty, .Part))) Then
                        connection = If(_kblMapperForCompare.KBLWireNetMapper.ContainsKey(.SystemId), TryCast(_kblMapperForCompare.KBLWireNetMapper(.SystemId), Connection), Nothing)
                        core = TryCast(_kblMapperForCompare.KBLPartMapper(.Part), Core)
                        wiringGroups = _kblMapperForCompare.GetWiringGroups.ToList
                    End If
                Else
                    If (_kblMapper.KBLOccurrenceMapper.ContainsKey(.SystemId)) AndAlso (_kblMapper.KBLPartMapper.ContainsKey(If(.Part Is Nothing, String.Empty, .Part))) Then
                        connection = If(_kblMapper.KBLWireNetMapper.ContainsKey(.SystemId), TryCast(_kblMapper.KBLWireNetMapper(.SystemId), Connection), Nothing)
                        core = TryCast(_kblMapper.KBLPartMapper(.Part), Core)
                        wiringGroups = _kblMapper.GetWiringGroups.ToList
                    End If
                End If

                Select Case e.Column.Key
                    Case SYSTEM_ID_COLUMN_KEY
                        e.Data = .SystemId
                    Case WIRE_CLASS_COLUMN_KEY
                        e.Data = KblObjectType.Core_occurrence.ToLocalizedString
                    Case WirePropertyName.Wire_number
                        e.Data = .Wire_number
                    Case WirePropertyName.Id
                        If (core IsNot Nothing) Then e.Data = core.Id
                    Case WirePropertyName.Wiring_group
                        If (.GetWiringGroup(wiringGroups) IsNot Nothing) Then
                            If (_kblMapperForCompare Is Nothing) Then
                                e.Data = .GetWiringGroup(wiringGroups).Id
                            Else
                                e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                            End If
                        End If
                    Case WirePropertyName.Cable_designator
                        If (core IsNot Nothing) AndAlso (core.Cable_designator IsNot Nothing) Then e.Data = core.Cable_designator
                    Case WirePropertyName.Wire_type
                        If (core IsNot Nothing) AndAlso (core.Wire_type IsNot Nothing) Then e.Data = core.Wire_type
                    Case WirePropertyName.Cross_section_area
                        If (core IsNot Nothing) AndAlso (core.Cross_section_area IsNot Nothing) Then
                            If (fromReference) AndAlso (_kblMapper.KBLUnitMapper.ContainsKey(core.Cross_section_area.Unit_component)) Then
                                e.Data = String.Format("{0} {1}", Math.Round(core.Cross_section_area.Value_component, 2), _kblMapper.KBLUnitMapper(core.Cross_section_area.Unit_component).Unit_name)
                            ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLUnitMapper.ContainsKey(core.Cross_section_area.Unit_component)) Then
                                e.Data = String.Format("{0} {1}", Math.Round(core.Cross_section_area.Value_component, 2), _kblMapperForCompare.KBLUnitMapper(core.Cross_section_area.Unit_component).Unit_name)
                            Else
                                e.Data = Math.Round(core.Cross_section_area.Value_component, 2)
                            End If
                        End If
                    Case WirePropertyName.Outside_diameter
                        If (core IsNot Nothing) AndAlso (core.Outside_diameter IsNot Nothing) Then
                            If (fromReference) AndAlso (_kblMapper.KBLUnitMapper.ContainsKey(core.Outside_diameter.Unit_component)) Then
                                e.Data = String.Format("{0} {1}", Math.Round(core.Outside_diameter.Value_component, 2), _kblMapper.KBLUnitMapper(core.Outside_diameter.Unit_component).Unit_name)
                            ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLUnitMapper.ContainsKey(core.Outside_diameter.Unit_component)) Then
                                e.Data = String.Format("{0} {1}", Math.Round(core.Outside_diameter.Value_component, 2), _kblMapperForCompare.KBLUnitMapper(core.Outside_diameter.Unit_component).Unit_name)
                            Else
                                e.Data = Math.Round(core.Outside_diameter.Value_component, 2)
                            End If
                        End If
                    Case WirePropertyName.Bend_radius
                        If (core IsNot Nothing) AndAlso (core.Bend_radius IsNot Nothing) Then
                            If (fromReference) AndAlso (_kblMapper.KBLUnitMapper.ContainsKey(core.Bend_radius.Unit_component)) Then
                                e.Data = String.Format("{0} {1}", Math.Round(core.Bend_radius.Value_component, 2), _kblMapper.KBLUnitMapper(core.Bend_radius.Unit_component).Unit_name)
                            ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLUnitMapper.ContainsKey(core.Bend_radius.Unit_component)) Then
                                e.Data = String.Format("{0} {1}", Math.Round(core.Bend_radius.Value_component, 2), _kblMapperForCompare.KBLUnitMapper(core.Bend_radius.Unit_component).Unit_name)
                            Else
                                e.Data = Math.Round(core.Bend_radius.Value_component, 2)
                            End If
                        End If
                    Case WirePropertyName.Core_Colour
                        If (core IsNot Nothing) Then e.Data = core.GetColours
                    Case WirePropertyName.Installation_Information
                        If (DirectCast(_kblMapper.KBLOccurrenceMapper(_kblMapper.KBLCoreCableMapper(.SystemId)), Special_wire_occurrence).Installation_information.Length <> 0) Then
                            e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                        End If
                    Case WirePropertyName.Length_information
                        If (.Length_information.Length <> 0) Then
                            If (_kblMapperForCompare Is Nothing) Then
                                For Each wireLength As Wire_length In .Length_information
                                    If (wireLength.Length_type.ToLower = _generalSettings.DefaultWireLengthType.ToLower) Then
                                        e.Data = String.Format("{0} {1}", Math.Round(wireLength.Length_value.Value_component, 2), _kblMapper.KBLUnitMapper(wireLength.Length_value.Unit_component).Unit_name)
                                        Exit For
                                    End If
                                Next
                            Else
                                If (.Length_information.Length = 1) Then
                                    If (fromReference) AndAlso (_kblMapper.KBLUnitMapper.ContainsKey(.Length_information(0).Length_value.Unit_component)) Then
                                        e.Data = String.Format("{0} {1}", Math.Round(.Length_information(0).Length_value.Value_component, 2), _kblMapper.KBLUnitMapper(.Length_information(0).Length_value.Unit_component).Unit_name)
                                    ElseIf (Not fromReference) AndAlso (_kblMapperForCompare.KBLUnitMapper.ContainsKey(.Length_information(0).Length_value.Unit_component)) Then
                                        e.Data = String.Format("{0} {1}", Math.Round(.Length_information(0).Length_value.Value_component, 2), _kblMapperForCompare.KBLUnitMapper(.Length_information(0).Length_value.Unit_component).Unit_name)
                                    Else
                                        e.Data = Math.Round(.Length_information(0).Length_value.Value_component, 2)
                                    End If
                                Else
                                    e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                                End If
                            End If
                        End If
                    Case NameOf(InformationHubStrings.AddLengthInfo_ColumnCaption)
                        If (.Length_information.Length > 1) Then
                            e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                        End If
                    Case ConnectionPropertyName.Signal_name
                        If (connection IsNot Nothing) AndAlso (connection.Signal_name IsNot Nothing) Then
                            e.Data = connection.Signal_name
                        End If
                    Case ConnectionPropertyName.Connector_A
                        If (connection IsNot Nothing) Then
                            Dim contactPointId As String = connection.GetStartContactPointId

                            If (fromReference) AndAlso (_kblMapper.KBLContactPointConnectorMapper.ContainsKey(contactPointId)) Then
                                With DirectCast(_kblMapper.KBLOccurrenceMapper(_kblMapper.KBLContactPointConnectorMapper(contactPointId)), Connector_occurrence)
                                    If (_kblMapperForCompare Is Nothing) AndAlso (.Description IsNot Nothing) AndAlso (.Description <> String.Empty) Then
                                        e.Data = String.Format("{0} - {1}", .Id, .Description)
                                    Else
                                        e.Data = .Id
                                    End If
                                End With
                            ElseIf (fromReference) AndAlso (_kblMapper.KBLContactPointComponentBoxMapper.ContainsKey(contactPointId)) Then
                                With DirectCast(_kblMapper.KBLOccurrenceMapper(_kblMapper.KBLContactPointComponentBoxMapper(contactPointId)), Component_box_occurrence)
                                    If (_kblMapperForCompare Is Nothing) AndAlso (.Description IsNot Nothing) AndAlso (.Description <> String.Empty) Then
                                        e.Data = String.Format("{0} - {1}", .Id, .Description)
                                    Else
                                        e.Data = .Id
                                    End If
                                End With
                            ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLContactPointConnectorMapper.ContainsKey(contactPointId)) Then
                                e.Data = DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(_kblMapperForCompare.KBLContactPointConnectorMapper(contactPointId)), Connector_occurrence).Id
                            ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLContactPointComponentBoxMapper.ContainsKey(contactPointId)) Then
                                e.Data = DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(_kblMapperForCompare.KBLContactPointComponentBoxMapper(contactPointId)), Component_box_occurrence).Id
                            End If
                        End If
                    Case ConnectionPropertyName.Cavity_A
                        If (connection IsNot Nothing) Then
                            Dim contactPointId As String = connection.GetStartContactPointId

                            If (fromReference) AndAlso (_kblMapper.KBLOccurrenceMapper.ContainsKey(contactPointId)) AndAlso (TypeOf _kblMapper.KBLOccurrenceMapper(contactPointId) Is Contact_point) Then
                                e.Data = DirectCast(_kblMapper.KBLPartMapper(DirectCast(_kblMapper.KBLOccurrenceMapper(DirectCast(_kblMapper.KBLOccurrenceMapper(contactPointId), Contact_point).Contacted_cavity.SplitSpace.FirstOrDefault), Cavity_occurrence).Part), Cavity).Cavity_number
                            ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLOccurrenceMapper.ContainsKey(contactPointId)) AndAlso (TypeOf _kblMapperForCompare.KBLOccurrenceMapper(contactPointId) Is Contact_point) Then
                                e.Data = DirectCast(_kblMapperForCompare.KBLPartMapper(DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(contactPointId), Contact_point).Contacted_cavity.SplitSpace.FirstOrDefault), Cavity_occurrence).Part), Cavity).Cavity_number
                            End If
                        End If
                    Case ConnectionPropertyName.Connector_B
                        If (connection IsNot Nothing) Then
                            Dim contactPointId As String = connection.GetEndContactPointId

                            If (fromReference) AndAlso (_kblMapper.KBLContactPointConnectorMapper.ContainsKey(contactPointId)) Then
                                With DirectCast(_kblMapper.KBLOccurrenceMapper(_kblMapper.KBLContactPointConnectorMapper(contactPointId)), Connector_occurrence)
                                    If (_kblMapperForCompare Is Nothing) AndAlso (.Description IsNot Nothing) AndAlso (.Description <> String.Empty) Then
                                        e.Data = String.Format("{0} - {1}", .Id, .Description)
                                    Else
                                        e.Data = .Id
                                    End If
                                End With
                            ElseIf (fromReference) AndAlso (_kblMapper.KBLContactPointComponentBoxMapper.ContainsKey(contactPointId)) Then
                                With DirectCast(_kblMapper.KBLOccurrenceMapper(_kblMapper.KBLContactPointComponentBoxMapper(contactPointId)), Component_box_occurrence)
                                    If (_kblMapperForCompare Is Nothing) AndAlso (.Description IsNot Nothing) AndAlso (.Description <> String.Empty) Then
                                        e.Data = String.Format("{0} - {1}", .Id, .Description)
                                    Else
                                        e.Data = .Id
                                    End If
                                End With
                            ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLContactPointConnectorMapper.ContainsKey(contactPointId)) Then
                                e.Data = DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(_kblMapperForCompare.KBLContactPointConnectorMapper(contactPointId)), Connector_occurrence).Id
                            ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLContactPointComponentBoxMapper.ContainsKey(contactPointId)) Then
                                e.Data = DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(_kblMapperForCompare.KBLContactPointComponentBoxMapper(contactPointId)), Component_box_occurrence).Id
                            End If
                        End If
                    Case ConnectionPropertyName.Cavity_B
                        If (connection IsNot Nothing) Then
                            Dim contactPointId As String = connection.GetEndContactPointId

                            If (fromReference) AndAlso (_kblMapper.KBLOccurrenceMapper.ContainsKey(contactPointId)) AndAlso (TypeOf _kblMapper.KBLOccurrenceMapper(contactPointId) Is Contact_point) Then
                                e.Data = DirectCast(_kblMapper.KBLPartMapper(DirectCast(_kblMapper.KBLOccurrenceMapper(DirectCast(_kblMapper.KBLOccurrenceMapper(contactPointId), Contact_point).Contacted_cavity.SplitSpace.FirstOrDefault), Cavity_occurrence).Part), Cavity).Cavity_number
                            ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLOccurrenceMapper.ContainsKey(contactPointId)) AndAlso (TypeOf _kblMapperForCompare.KBLOccurrenceMapper(contactPointId) Is Contact_point) Then
                                e.Data = DirectCast(_kblMapperForCompare.KBLPartMapper(DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(contactPointId), Contact_point).Contacted_cavity.SplitSpace.FirstOrDefault), Cavity_occurrence).Part), Cavity).Cavity_number
                            End If
                        End If
                    Case WirePropertyName.Routing
                        Dim segments As New Dictionary(Of String, Segment)

                        If (fromReference) Then
                            For Each segment As Segment In _kblMapper.KBLWireSegmentMapper.GetSegmentsOrEmpty(.SystemId)
                                segments.TryAdd(segment.SystemId, segment)
                            Next
                        ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) Then
                            For Each segment As Segment In _kblMapperForCompare.KBLWireSegmentMapper.GetSegmentsOrEmpty(.SystemId)
                                segments.TryAdd(segment.SystemId, segment)
                            Next
                        End If

                        If (segments.Count = 1) Then
                            e.Data = segments.Values(0).Id
                        ElseIf (segments.Count > 1) Then
                            e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                        End If
                    Case WirePropertyName.AdditionalExtremities
                        If (connection IsNot Nothing) AndAlso (connection.Extremities.Length > 2) Then
                            e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                        End If
                    Case NameOf(InformationHubStrings.AssignedModules_ColumnCaption)
                        e.Data = GetAssignedModules(.SystemId)
                    Case Else
                        OnUnhandledCellDataRequested(e)
                End Select
            End With
        End If
    End Sub

    Private Sub udsCables_CellDataRequested(sender As Object, e As CellDataRequestedEventArgs) Handles udsCables.CellDataRequested
        e.CacheData = False

        If (e.Row.ParentRow Is Nothing) AndAlso e.Row.Tag IsNot Nothing Then
            With DirectCast(e.Row.Tag, Special_wire_occurrence)
                Dim fromReference As Boolean = True
                Dim genWire As General_wire = Nothing

                If HasChangeTypeWithInverse(e.Row, CompareChangeType.New) Then
                    fromReference = False

                    If (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLPartMapper.ContainsKey(If(.Part Is Nothing, String.Empty, .Part))) Then
                        genWire = TryCast(_kblMapperForCompare.KBLPartMapper(.Part), General_wire)
                    End If
                Else
                    If (_kblMapper.KBLOccurrenceMapper.ContainsKey(.SystemId)) AndAlso (_kblMapper.KBLPartMapper.ContainsKey(If(.Part Is Nothing, String.Empty, .Part))) Then
                        genWire = TryCast(_kblMapper.KBLPartMapper(.Part), General_wire)
                    End If
                End If

                If (genWire IsNot Nothing) Then
                    If Not RequestCellPartData(e.Data, fromReference, e.Column.Key, genWire) Then
                        OnUnhandledCellDataRequested(e)
                    End If
                End If

                Select Case e.Column.Key
                    Case PRIMARY_LENGTH_COLUMN_KEY
                        'HINT Daimler Gimmick to get the length of cables directly visible in kbl compare
                        'This is a temporary field filled with the selected length class of wires from general settings
                        If (.Length_information.Length > 0) Then
                            If (_kblMapperForCompare IsNot Nothing) Then
                                For Each wireLength As Wire_length In .Length_information
                                    If (wireLength.Length_type.ToLower = _generalSettings.DefaultCableLengthType.ToLower) Then
                                        If (fromReference) AndAlso (_kblMapper.KBLUnitMapper.ContainsKey(wireLength.Length_value.Unit_component)) Then
                                            e.Data = String.Format("{0} {1}", Math.Round(wireLength.Length_value.Value_component, 2), _kblMapper.KBLUnitMapper(wireLength.Length_value.Unit_component).Unit_name)
                                        ElseIf (Not fromReference) AndAlso (_kblMapperForCompare.KBLUnitMapper.ContainsKey(wireLength.Length_value.Unit_component)) Then
                                            e.Data = String.Format("{0} {1}", Math.Round(wireLength.Length_value.Value_component, 2), _kblMapperForCompare.KBLUnitMapper(wireLength.Length_value.Unit_component).Unit_name)
                                        Else
                                            e.Data = Math.Round(wireLength.Length_value.Value_component, 2)
                                        End If
                                        Exit For
                                    End If
                                Next
                            End If
                        End If

                    Case CablePropertyName.Special_wire_id.ToString
                        e.Data = .Special_wire_id
                    Case CablePropertyName.Cable_designator.ToString
                        If (genWire IsNot Nothing) AndAlso (genWire.Cable_designator IsNot Nothing) Then
                            e.Data = genWire.Cable_designator
                        End If
                    Case CablePropertyName.Wire_type.ToString
                        If (genWire IsNot Nothing) AndAlso (genWire.Wire_type IsNot Nothing) Then
                            e.Data = genWire.Wire_type
                        End If
                    Case CablePropertyName.Bend_radius.ToString
                        If (genWire IsNot Nothing) AndAlso (genWire.Bend_radius IsNot Nothing) Then
                            If (fromReference) AndAlso (_kblMapper.KBLUnitMapper.ContainsKey(genWire.Bend_radius.Unit_component)) Then
                                e.Data = String.Format("{0} {1}", Math.Round(genWire.Bend_radius.Value_component, 2), _kblMapper.KBLUnitMapper(genWire.Bend_radius.Unit_component).Unit_name)
                            ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLUnitMapper.ContainsKey(genWire.Bend_radius.Unit_component)) Then
                                e.Data = String.Format("{0} {1}", Math.Round(genWire.Bend_radius.Value_component, 2), _kblMapperForCompare.KBLUnitMapper(genWire.Bend_radius.Unit_component).Unit_name)
                            Else
                                e.Data = Math.Round(genWire.Bend_radius.Value_component, 2)
                            End If
                        End If
                    Case CablePropertyName.Cross_section_area.ToString
                        If (genWire IsNot Nothing) AndAlso (genWire.Cross_section_area IsNot Nothing) Then
                            If (fromReference) AndAlso (_kblMapper.KBLUnitMapper.ContainsKey(genWire.Cross_section_area.Unit_component)) Then
                                e.Data = String.Format("{0} {1}", Math.Round(genWire.Cross_section_area.Value_component, 2), _kblMapper.KBLUnitMapper(genWire.Cross_section_area.Unit_component).Unit_name)
                            ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLUnitMapper.ContainsKey(genWire.Cross_section_area.Unit_component)) Then
                                e.Data = String.Format("{0} {1}", Math.Round(genWire.Cross_section_area.Value_component, 2), _kblMapperForCompare.KBLUnitMapper(genWire.Cross_section_area.Unit_component).Unit_name)
                            Else
                                e.Data = Math.Round(genWire.Cross_section_area.Value_component, 2)
                            End If
                        End If
                    Case CablePropertyName.Outside_diameter.ToString
                        If (genWire IsNot Nothing) AndAlso (genWire.Outside_diameter IsNot Nothing) Then
                            If (fromReference) AndAlso (_kblMapper.KBLUnitMapper.ContainsKey(genWire.Outside_diameter.Unit_component)) Then
                                e.Data = String.Format("{0} {1}", Math.Round(genWire.Outside_diameter.Value_component, 2), _kblMapper.KBLUnitMapper(genWire.Outside_diameter.Unit_component).Unit_name)
                            ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLUnitMapper.ContainsKey(genWire.Outside_diameter.Unit_component)) Then
                                e.Data = String.Format("{0} {1}", Math.Round(genWire.Outside_diameter.Value_component, 2), _kblMapperForCompare.KBLUnitMapper(genWire.Outside_diameter.Unit_component).Unit_name)
                            Else
                                e.Data = Math.Round(genWire.Outside_diameter.Value_component, 2)
                            End If
                        End If
                    Case CablePropertyName.CoverColours.ToString
                        If (genWire IsNot Nothing) Then e.Data = genWire.GetColours
                    Case CablePropertyName.Installation_Information.ToString
                        If (.Installation_information.Length <> 0) Then
                            e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                        End If
                    Case CablePropertyName.Length_Information.ToString
                        If (.Length_information.Length <> 0) Then
                            If (_kblMapperForCompare Is Nothing) Then
                                For Each wireLength As Wire_length In .Length_information
                                    If (wireLength.Length_type.ToLower = _generalSettings.DefaultCableLengthType.ToLower) Then
                                        e.Data = String.Format("{0} {1}", Math.Round(wireLength.Length_value.Value_component, 2), _kblMapper.KBLUnitMapper(wireLength.Length_value.Unit_component).Unit_name)

                                        Exit For
                                    End If
                                Next
                            Else
                                If (.Length_information.Length = 1) Then
                                    If (fromReference) AndAlso (_kblMapper.KBLUnitMapper.ContainsKey(.Length_information(0).Length_value.Unit_component)) Then
                                        e.Data = String.Format("{0} {1}", Math.Round(.Length_information(0).Length_value.Value_component, 2), _kblMapper.KBLUnitMapper(.Length_information(0).Length_value.Unit_component).Unit_name)
                                    ElseIf (Not fromReference) AndAlso (_kblMapperForCompare.KBLUnitMapper.ContainsKey(.Length_information(0).Length_value.Unit_component)) Then
                                        e.Data = String.Format("{0} {1}", Math.Round(.Length_information(0).Length_value.Value_component, 2), _kblMapperForCompare.KBLUnitMapper(.Length_information(0).Length_value.Unit_component).Unit_name)
                                    Else
                                        e.Data = Math.Round(.Length_information(0).Length_value.Value_component, 2)
                                    End If
                                Else
                                    e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                                End If
                            End If
                        End If
                    Case CablePropertyName.Routing.ToString
                        Dim segments As New Dictionary(Of String, Segment)

                        For Each coreOccurrence As Core_occurrence In .Core_occurrence
                            If (fromReference) Then
                                For Each segment As Segment In _kblMapper.KBLWireSegmentMapper.GetSegmentsOrEmpty(coreOccurrence.SystemId)
                                    segments.TryAdd(segment.SystemId, segment)
                                Next
                            ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) Then
                                For Each segment As Segment In _kblMapperForCompare.KBLWireSegmentMapper.GetSegmentsOrEmpty(coreOccurrence.SystemId)
                                    segments.TryAdd(segment.SystemId, segment)
                                Next
                            End If
                        Next

                        If (segments.Count = 1) Then
                            e.Data = segments.Values(0).Id
                        ElseIf (segments.Count > 1) Then
                            e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                        End If
                    Case NameOf(InformationHubStrings.AddLengthInfo_ColumnCaption)
                        If (.Length_information.Length > 1) Then
                            e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                        End If
                    Case NameOf(InformationHubStrings.AssignedModules_ColumnCaption)
                        e.Data = GetAssignedModules(.SystemId)
                    Case Else
                        OnUnhandledCellDataRequested(e)
                End Select
            End With
        Else
            CoreCellDataRequested(e)
        End If
    End Sub

    Private Sub udsCables_InitializeDataRow(sender As Object, e As InitializeDataRowEventArgs) Handles udsCables.InitializeDataRow
        If e.Row.ParentRow Is Nothing Then
            Dim cable As IKblOccurrence
            Dim rowCount As Integer

            If (e.Row.Band.Tag Is Nothing) Then
                cable = _kblMapper.KBLCableList(e.Row.Index)
                rowCount = CType(cable, Special_wire_occurrence).Core_occurrence.Length
            Else
                Dim compareObjKey As String = DirectCast(e.Row.Band.Tag, Dictionary(Of String, Object)).Keys.ElementAt(e.Row.Index)
                Dim compareObjValue As Object = DirectCast(e.Row.Band.Tag, Dictionary(Of String, Object)).Values.ElementAt(e.Row.Index)

                If (TypeOf compareObjValue Is Special_wire_occurrence) Then
                    cable = DirectCast(compareObjValue, Special_wire_occurrence)
                Else
                    cable = TryCast(_kblMapper.KBLOccurrenceMapper(ExtractSystemId(compareObjKey)), IKblOccurrence) 'HINT: it's not safe that this is always a special_wire_occurrence it could also be a core_occurrence
                End If

                Dim compareObjects As New Dictionary(Of String, Object)

                If TypeOf compareObjValue Is CableChangedProperty Then
                    Dim changeProperty As ChangedProperty = DirectCast(compareObjValue, CableChangedProperty)
                    If (changeProperty.ChangedProperties.ContainsKey(CablePropertyName.Core_Occurrence)) Then
                        Dim comparisonMapper As CoreOccurrenceComparisonMapper = DirectCast(changeProperty.ChangedProperties(CablePropertyName.Core_Occurrence), CoreOccurrenceComparisonMapper)
                        For Each cItem As ChangedItem In comparisonMapper.Changes
                            compareObjects.Add(String.Format("{0}|{1}", cItem.Text, cItem.Key), cItem.Item)
                        Next
                    End If
                End If

                rowCount = compareObjects.Count
                If e.Row.Band.ChildBands.Count > 0 Then
                    e.Row.GetChildRows(0).Tag = compareObjects
                End If

                SetDiffTypeCellText(e.Row, ChangedItem.ParseFromText(ExtractChangeText(compareObjKey)))
            End If

            e.Row.Tag = cable
            If e.Row.Band.ChildBands.Count > 0 Then
                e.Row.GetChildRows(0).SetCount(rowCount)
            End If
        Else
            If e.Row.ParentCollection.Tag Is Nothing Then
                e.Row.Tag = DirectCast(e.Row.ParentRow.Tag, Special_wire_occurrence).Core_occurrence(e.Row.Index)
            Else
                Dim compareObjKey As String = DirectCast(e.Row.ParentCollection.Tag, Dictionary(Of String, Object)).Keys.ElementAt(e.Row.Index)
                Dim compareObjValue As Object = DirectCast(e.Row.ParentCollection.Tag, Dictionary(Of String, Object)).Values.ElementAt(e.Row.Index)

                If (TypeOf compareObjValue Is Core_occurrence) Then
                    e.Row.Tag = DirectCast(compareObjValue, Core_occurrence)
                Else
                    For Each coreOcc As Core_occurrence In DirectCast(e.Row.ParentRow.Tag, Special_wire_occurrence).Core_occurrence
                        If (coreOcc.Wire_number = ExtractSystemId(compareObjKey)) Then
                            e.Row.Tag = coreOcc

                            Exit For
                        End If
                    Next
                End If

                SetDiffTypeCellText(e.Row, ChangedItem.ParseFromText(ExtractChangeText(compareObjKey)))
            End If
        End If
    End Sub


    Private Sub ugCables_AfterRowActivate(sender As Object, e As EventArgs) Handles ugCables.AfterRowActivate
        If (Me.ugCables.ActiveRow.Appearance.BackColor = Color.FromArgb(190, 190, 190)) Then
            Me.ugCables.ActiveRow = Nothing
        ElseIf (_kblMapperForCompare IsNot Nothing) Then
            If (SetInformationHubEventArgs(Nothing, Me.ugCables.ActiveRow, Nothing)) Then
                OnHubSelectionChanged()
            End If
            If (Not Me.Focused) Then
                Me.Focus()
            End If
        End If
    End Sub

    Private Sub ugCables_AfterSelectChange(sender As Object, e As AfterSelectChangeEventArgs) Handles ugCables.AfterSelectChange
        With Me.ugCables
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

    Private Sub ugCables_BeforeSelectChange(sender As Object, e As BeforeSelectChangeEventArgs) Handles ugCables.BeforeSelectChange
        ClearSelectedChildRowsInGrid(Me.ugCables)

        For Each row As UltraGridRow In e.NewSelections.Rows
            If (row.Appearance.BackColor = Color.FromArgb(190, 190, 190)) Then e.Cancel = True
        Next
    End Sub

    Private Sub ugCables_ClickCell(sender As Object, e As ClickCellEventArgs) Handles ugCables.ClickCell
        If (_pressedMouseButton = System.Windows.Forms.MouseButtons.Left) AndAlso (_pressedKey <> Keys.ControlKey AndAlso _pressedKey <> Keys.ShiftKey) AndAlso (_informationHubEventArgs IsNot Nothing) AndAlso (_kblMapperForCompare IsNot Nothing OrElse e.Cell.Row.Appearance.BackColor <> Color.FromArgb(190, 190, 190)) Then
            Dim prevIds As IEnumerable(Of String) = _informationHubEventArgs.ObjectIds

            SetInformationHubEventArgs(Nothing, e.Cell.Row, Nothing)

            If (prevIds IsNot Nothing) AndAlso (String.Join(";", prevIds) <> String.Join(";", _informationHubEventArgs.ObjectIds)) Then
                OnHubSelectionChanged()

                If (Not Me.Focused) Then Me.Focus()
            End If
        ElseIf (_pressedMouseButton = System.Windows.Forms.MouseButtons.Right) AndAlso (_kblMapperForCompare Is Nothing) AndAlso (ChangeGridContextMenuVisibility(e.Cell.Row)) Then
            _contextMenuGrid.ShowPopup()
        End If
    End Sub

    Private Sub ugCables_ClickCellButton(sender As Object, e As CellEventArgs) Handles ugCables.ClickCellButton
        If (_pressedMouseButton = System.Windows.Forms.MouseButtons.Left) Then
            Dim fromReference As Boolean = True

            If HasChangeTypeWithInverse(CType(e.Cell.Row.ListObject, UltraDataRow), CompareChangeType.New) Then
                fromReference = False
            End If

            If (e.Cell.Row.ParentRow Is Nothing) AndAlso (TypeOf e.Cell.Tag Is Special_wire_occurrence) Then
                With DirectCast(e.Cell.Tag, Special_wire_occurrence)
                    If (fromReference) AndAlso (_kblMapper.KBLPartMapper.ContainsKey(If(.Part Is Nothing, String.Empty, .Part))) Then
                        RequestDialogPartData(_kblMapper, e.Cell.Column.Key, DirectCast(_kblMapper.KBLPartMapper(.Part), Part))
                    ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLPartMapper.ContainsKey(If(.Part Is Nothing, String.Empty, .Part))) Then
                        RequestDialogPartData(_kblMapperForCompare, e.Cell.Column.Key, DirectCast(_kblMapperForCompare.KBLPartMapper(.Part), Part))
                    End If

                    Select Case e.Cell.Column.Key
                        Case CablePropertyName.Installation_Information
                            ShowDetailInformationForm(InformationHubStrings.InstallInfo_Caption, .Installation_information, Nothing, _kblMapper)
                        Case CablePropertyName.Length_Information
                            If (_generalSettings IsNot Nothing) Then
                                ShowDetailInformationForm(InformationHubStrings.LengthInfo_Caption, .Length_information, Nothing, _kblMapper, Nothing, _generalSettings.DefaultCableLengthType)
                            Else
                                ShowDetailInformationForm(InformationHubStrings.LengthInfo_Caption, .Length_information, Nothing, _kblMapper)
                            End If
                        Case CablePropertyName.Routing
                            Dim segments As New Dictionary(Of String, Segment)

                            For Each coreOccurrence As Core_occurrence In .Core_occurrence
                                If (fromReference) Then
                                    For Each segment As Segment In _kblMapper.KBLWireSegmentMapper.GetSegmentsOrEmpty(coreOccurrence.SystemId)
                                        segments.TryAdd(segment.SystemId, segment)
                                    Next
                                ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) Then
                                    For Each segment As Segment In _kblMapperForCompare.KBLWireSegmentMapper.GetSegmentsOrEmpty(coreOccurrence.SystemId)
                                        segments.TryAdd(segment.SystemId, segment)
                                    Next
                                End If
                            Next

                            ShowDetailInformationForm(InformationHubStrings.RoutingInfo_Caption, segments, Nothing, _kblMapper)
                    End Select
                End With
            ElseIf (TypeOf e.Cell.Tag Is Core_occurrence) Then
                With DirectCast(e.Cell.Tag, Core_occurrence)
                    Select Case e.Cell.Column.Key
                        Case CablePropertyName.Length_Information.ToString
                            If (_generalSettings IsNot Nothing) Then
                                ShowDetailInformationForm(InformationHubStrings.LengthInfo_Caption, .Length_information, Nothing, _kblMapper, Nothing, _generalSettings.DefaultWireLengthType)
                            Else
                                ShowDetailInformationForm(InformationHubStrings.LengthInfo_Caption, .Length_information, Nothing, _kblMapper)
                            End If
                    End Select
                End With
            ElseIf (TypeOf e.Cell.Tag Is KeyValuePair(Of String, Object)) Then
                Dim objectId As String = DirectCast(e.Cell.Row.Tag, KeyValuePair(Of String, Object)).Key.SplitRemoveEmpty("|"c)(1)
                If (Not _kblMapper.KBLOccurrenceMapper.ContainsKey(objectId)) Then
                    objectId = e.Cell.Row.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString
                End If

                RequestDialogCompareData(True, objectId, DirectCast(e.Cell.Tag, KeyValuePair(Of String, Object)))
            End If
        End If
    End Sub

    Private Sub ugCables_DoubleClickRow(sender As Object, e As DoubleClickRowEventArgs) Handles ugCables.DoubleClickRow
        OnDoubleClickRow(sender, e)
    End Sub

    Private Sub ugCables_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugCables.InitializeLayout
        Me.ugCables.BeginUpdate()
        Me.ugCables.EventManager.AllEventsEnabled = False

        InitializeGridLayout(_cableGridAppearance, e.Layout)

        If (_kblMapperForCompare IsNot Nothing) AndAlso (e.Layout.Bands(0).Columns.Exists(PRIMARY_LENGTH_COLUMN_KEY)) Then
            e.Layout.Bands(0).Columns(PRIMARY_LENGTH_COLUMN_KEY).Header.Caption = String.Format("{0} [{1}]", InformationHubStrings.PrimaryLength_Caption, _generalSettings.DefaultCableLengthType.Split(" ".ToCharArray, StringSplitOptions.RemoveEmptyEntries)(0))
            e.Layout.Bands(0).Columns(PRIMARY_LENGTH_COLUMN_KEY).Width = 150
        End If

        If (_kblMapperForCompare Is Nothing) Then
            If e.Layout.Bands.Count > 0 AndAlso (e.Layout.Bands(0).Columns.Exists(CablePropertyName.Length_Information)) AndAlso (_kblMapper.KBLCableLengthTypes.Count > 1) AndAlso (_generalSettings.DefaultCableLengthType IsNot Nothing AndAlso _generalSettings.DefaultCableLengthType <> String.Empty) Then
                e.Layout.Bands(0).Columns(CablePropertyName.Length_Information).Header.Caption = String.Format("{0} [{1}]", e.Layout.Bands(0).Columns(CablePropertyName.Length_Information).Header.Caption, _generalSettings.DefaultCableLengthType.SplitSpace(0))
            End If
            If e.Layout.Bands.Count > 1 AndAlso (e.Layout.Bands(1).Columns.Exists(WirePropertyName.Length_information)) AndAlso (_kblMapper.KBLWireLengthTypes.Count > 1) AndAlso (_generalSettings.DefaultWireLengthType IsNot Nothing AndAlso _generalSettings.DefaultWireLengthType <> String.Empty) Then
                e.Layout.Bands(1).Columns(WirePropertyName.Length_information).Header.Caption = String.Format("{0} [{1}]", e.Layout.Bands(1).Columns(WirePropertyName.Length_information).Header.Caption, _generalSettings.DefaultWireLengthType.SplitSpace(0))
            End If
        End If

        Me.ugCables.EventManager.AllEventsEnabled = True
        Me.ugCables.EndUpdate()
    End Sub

    Private Sub InitializeCableRowCore(row As UltraGridRow)
        If (row.ParentRow Is Nothing) Then
            If (DirectCast(row.ListObject, UltraDataRow).Band.Tag Is Nothing) Then
                row.Tag = DirectCast(DirectCast(row.ListObject, UltraDataRow).Tag, Special_wire_occurrence).SystemId

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

                        If (TypeOf compareObject.Value Is CableChangedProperty) Then
                            For Each changedProperty As KeyValuePair(Of String, Object) In DirectCast(compareObject.Value, CableChangedProperty).ChangedProperties
                                If (changedProperty.Key = cell.Column.Key) Then
                                    cell.Tag = changedProperty

                                    Exit For
                                ElseIf (changedProperty.Key = NameOf(IKblOccurrence.Part)) Then
                                    For Each partChangedProperty As KeyValuePair(Of String, Object) In DirectCast(changedProperty.Value, GeneralWireChangedProperty).ChangedProperties
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
                ElseIf (cell.Column.Key = CablePropertyName.Length_Information) AndAlso (_kblMapperForCompare Is Nothing) AndAlso (_kblMapper.KBLOccurrenceMapper.ContainsKey(row.Tag?.ToString)) AndAlso (TypeOf _kblMapper.KBLOccurrenceMapper(row.Tag?.ToString) Is Special_wire_occurrence) AndAlso (DirectCast(_kblMapper.KBLOccurrenceMapper(row.Tag?.ToString), Special_wire_occurrence).Length_information.Length <> 0) Then
                    Dim cableLengthTypes As New List(Of String)

                    For Each wireLength As Wire_length In DirectCast(_kblMapper.KBLOccurrenceMapper(row.Tag?.ToString), Special_wire_occurrence).Length_information
                        cableLengthTypes.Add(wireLength.Length_type)
                    Next

                    If (cableLengthTypes.Count > 1) OrElse (Not cableLengthTypes.Any(Function(lt) lt.ToLower = _generalSettings.DefaultCableLengthType.ToLower)) Then
                        cell.Appearance.Image = My.Resources.About_Small
                        cell.Style = ColumnStyle.Button
                        cell.Tag = _kblMapper.KBLOccurrenceMapper(row.Tag?.ToString)
                    End If
                End If
            Next

            Me.ugCables.EventManager.AllEventsEnabled = False

            If (row?.ChildBands?.Count).GetValueOrDefault > 0 Then
                For Each childRow As UltraGridRow In row.ChildBands(0).Rows
                    childRow.Tag = childRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString
                    InitializeCableRowCore(childRow)
                Next
            End If
            Me.ugCables.EventManager.AllEventsEnabled = True
        Else
            If (DirectCast(row.ListObject, UltraDataRow).ParentCollection.Tag IsNot Nothing) Then
                InitializeRowForCompare(DirectCast(DirectCast(row.ListObject, UltraDataRow).ParentCollection.Tag, Dictionary(Of String, Object)), row)
            Else
                If _redliningInformation IsNot Nothing AndAlso _redliningInformation.Redlinings.Where(Function(redlining) redlining.ObjectId = row.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString).Any() Then
                    row.Cells(1).Appearance.Image = My.Resources.Redlining
                End If
            End If

            For Each cell As UltraGridCell In row.Cells
                If (cell.Value IsNot Nothing AndAlso cell.Value.ToString = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS) Then
                    cell.Style = ColumnStyle.Button

                    If (TypeOf row.Tag Is KeyValuePair(Of String, Object)) Then
                        Dim compareObject As KeyValuePair(Of String, Object) = DirectCast(row.Tag, KeyValuePair(Of String, Object))

                        If (TypeOf compareObject.Value Is CoreOccurrenceChangedProperty) Then
                            For Each changedProperty As KeyValuePair(Of String, Object) In DirectCast(compareObject.Value, CoreOccurrenceChangedProperty).ChangedProperties
                                If (changedProperty.Key = cell.Column.Key) Then
                                    cell.Tag = changedProperty

                                    Exit For
                                ElseIf (changedProperty.Key = NameOf(IKblOccurrence.Part)) Then
                                    For Each partChangedProperty As KeyValuePair(Of String, Object) In DirectCast(changedProperty.Value, CoreChangedProperty).ChangedProperties
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
                    ElseIf (row.Tag IsNot Nothing) Then
                        cell.Tag = _kblMapper.KBLOccurrenceMapper(row.Tag?.ToString)
                    End If
                ElseIf (cell.Column.Key = WirePropertyName.Length_information) AndAlso (_kblMapperForCompare Is Nothing) AndAlso (DirectCast(_kblMapper.KBLOccurrenceMapper(row.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString), Core_occurrence).Length_information.Length <> 0) Then
                    Dim wireLengthTypes As New List(Of String)

                    For Each wireLength As Wire_length In DirectCast(_kblMapper.KBLOccurrenceMapper(row.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString), Core_occurrence).Length_information
                        wireLengthTypes.Add(wireLength.Length_type)
                    Next

                    If (wireLengthTypes.Count > 1) OrElse (Not wireLengthTypes.Any(Function(lt) lt.ToLower = _generalSettings.DefaultWireLengthType.ToLower)) Then
                        cell.Appearance.Image = My.Resources.About_Small
                        cell.Style = ColumnStyle.Button
                        cell.Tag = _kblMapper.KBLOccurrenceMapper(row.Tag?.ToString)
                    End If
                ElseIf (cell.Column.Key = ConnectionPropertyName.Cavity_A OrElse cell.Column.Key = ConnectionPropertyName.Cavity_B.ToString) AndAlso (_kblMapperForCompare Is Nothing) Then
                    Dim connection As Connection = If(_kblMapper.KBLWireNetMapper.ContainsKey(row.Tag?.ToString), DirectCast(_kblMapper.KBLOccurrenceMapper(_kblMapper.KBLWireNetMapper(row.Tag?.ToString).SystemId), Connection), Nothing)
                    Dim startTerminalPlating As String = String.Empty
                    Dim endTerminalPlating As String = String.Empty

                    If (connection IsNot Nothing) AndAlso (_kblMapper.KBLOccurrenceMapper.ContainsKey(connection.GetStartContactPointId)) AndAlso (DirectCast(_kblMapper.KBLOccurrenceMapper(connection.GetStartContactPointId), Contact_point).Associated_parts IsNot Nothing) Then
                        For Each associatedPartId As String In DirectCast(_kblMapper.KBLOccurrenceMapper(connection.GetStartContactPointId), Contact_point).Associated_parts.SplitSpace
                            If (_kblMapper.KBLOccurrenceMapper.ContainsKey(associatedPartId)) AndAlso (TypeOf _kblMapper.KBLOccurrenceMapper(associatedPartId) Is Special_terminal_occurrence OrElse TypeOf _kblMapper.KBLOccurrenceMapper(associatedPartId) Is Terminal_occurrence) Then
                                Dim terminalPart As General_terminal = If(_kblMapper.GetHarness.GetSpecialTerminalOccurrence(associatedPartId) IsNot Nothing, _kblMapper.GetGeneralTerminal(_kblMapper.GetHarness.GetSpecialTerminalOccurrence(associatedPartId).Part), _kblMapper.GetGeneralTerminal(_kblMapper.GetHarness.GetTerminalOccurrence(associatedPartId).Part))
                                If (terminalPart IsNot Nothing) Then startTerminalPlating = terminalPart.Plating_material
                            End If
                        Next
                    End If

                    If (connection IsNot Nothing) AndAlso (_kblMapper.KBLOccurrenceMapper.ContainsKey(connection.GetEndContactPointId)) AndAlso (DirectCast(_kblMapper.KBLOccurrenceMapper(connection.GetEndContactPointId), Contact_point).Associated_parts IsNot Nothing) Then
                        For Each associatedPartId As String In DirectCast(_kblMapper.KBLOccurrenceMapper(connection.GetEndContactPointId), Contact_point).Associated_parts.SplitSpace
                            If (_kblMapper.KBLOccurrenceMapper.ContainsKey(associatedPartId)) AndAlso (TypeOf _kblMapper.KBLOccurrenceMapper(associatedPartId) Is Special_terminal_occurrence OrElse TypeOf _kblMapper.KBLOccurrenceMapper(associatedPartId) Is Terminal_occurrence) Then
                                Dim terminalPart As General_terminal = If(_kblMapper.GetHarness.GetSpecialTerminalOccurrence(associatedPartId) IsNot Nothing, _kblMapper.GetGeneralTerminal(_kblMapper.GetHarness.GetSpecialTerminalOccurrence(associatedPartId).Part), _kblMapper.GetGeneralTerminal(_kblMapper.GetHarness.GetTerminalOccurrence(associatedPartId).Part))
                                If (terminalPart IsNot Nothing) Then
                                    endTerminalPlating = terminalPart.Plating_material
                                End If
                            End If
                        Next
                    End If

                    If (startTerminalPlating <> endTerminalPlating) AndAlso (startTerminalPlating <> String.Empty AndAlso endTerminalPlating <> String.Empty) Then
                        cell.Appearance.Image = My.Resources.About_Small
                        cell.ToolTipText = If(cell.Column.Key = ConnectionPropertyName.Cavity_A, String.Format(InformationHubStrings.TermPlatingMatDiff_TooltipText, startTerminalPlating), String.Format(InformationHubStrings.TermPlatingMatDiff_TooltipText, endTerminalPlating))
                    End If
                End If
            Next
        End If
    End Sub

    Private Sub ugCables_InitializeRow(sender As Object, e As InitializeRowEventArgs) Handles ugCables.InitializeRow
        If Not e.ReInitialize Then
            InitializeCableRowCore(e.Row)
        End If
    End Sub

    Private Sub ugCables_KeyDown(sender As Object, e As KeyEventArgs) Handles ugCables.KeyDown
        OnGridKeyDown(Me, New GridKeyDownEventArgs(e, DirectCast(sender, UltraGrid)))
        If Not e.Handled Then
            If (e.Control) AndAlso (e.KeyCode = Keys.A) Then
                SelectAllRowsOfActiveGrid()
            ElseIf (e.KeyCode = Keys.Escape) Then
                If (Me.ugCables.Selected.Rows.Count = 0) Then
                    ClearMarkedRowsInGrids()
                Else
                    ClearSelectedRowsInGrids()
                End If

                If (SetInformationHubEventArgs(Nothing, Nothing, Me.ugCables.Selected.Rows)) Then
                    OnHubSelectionChanged()
                End If

            ElseIf (e.KeyCode = Keys.Return) Then
                SetSelectedAsMarkedOriginRows(ugCables)
            End If
        End If
    End Sub

    Private Sub ugCables_MouseDown(sender As Object, e As MouseEventArgs) Handles ugCables.MouseDown
        If (_contextMenuGrid.IsOpen) Then _contextMenuGrid.ClosePopup()

        _pressedMouseButton = e.Button
    End Sub

    Private Sub ugCables_MouseLeave(sender As Object, e As EventArgs) Handles ugCables.MouseLeave
        _messageEventArgs.StatusMessage = String.Empty

        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub ugCables_MouseMove(sender As Object, e As MouseEventArgs) Handles ugCables.MouseMove
        _messageEventArgs.StatusMessage = String.Format(InformationHubStrings.RowCount_Label, Me.ugCables.Rows.Count, Me.ugCables.Rows.FilteredInNonGroupByRowCount, Me.ugCables.Rows.VisibleRowCount, Me.ugCables.Selected.Rows.Count)

        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub udsCables_CellDataUpdated(sender As Object, e As CellDataUpdatedEventArgs) Handles udsCables.CellDataUpdated
        RaiseEvent CellValueUpdated(sender, e)
    End Sub

End Class