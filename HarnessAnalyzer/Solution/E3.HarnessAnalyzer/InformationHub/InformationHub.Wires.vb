Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinGrid
Imports Zuken.E3.Interop
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Partial Public Class InformationHub

    Private Sub InitializeWires()
        _wireGridAppearance = GridAppearance.All.OfType(Of WireGridAppearance).Single
        AddNewRowFilters(ugWires)

        If _comparisonMapperList Is Nothing Then
            For Each wire As Wire_occurrence In _kblMapper.KBLWireList
                _wiresAndCores.Add(wire)
            Next

            For Each core As Core_occurrence In _kblMapper.KBLCoreList
                _wiresAndCores.Add(core)
            Next

            FillDataSource(Me.udsWires, _wireGridAppearance, _wiresAndCores.Count)
        Else
            FillDataSource(Me.udsWires, _wireGridAppearance, 0, _comparisonMapperList(KblObjectType.wire_occurrence))
        End If

        If (Me.udsWires.Band.Columns.Count = 0) OrElse (Me.udsWires.Rows.Count = 0) Then
            Me.utcInformationHub.Tabs(TabNames.tabWires.ToString()).Visible = False
        End If
    End Sub

    Private Sub ShowWireInSchematics(coreWireId As String)
        Dim e3Application As e3Application
        Dim e3Job As e3Job
        Dim e3Cable As e3Device
        Dim e3NetSegment As e3NetSegment
        Dim e3Sheet As e3Sheet
        Dim e3Wire As e3Pin

        Try
            e3Application = New e3Application
            e3Job = CType(e3Application.CreateJobObject, e3Job)
            e3Cable = CType(e3Job.CreateDeviceObject, e3Device)
            e3NetSegment = CType(e3Job.CreateNetSegmentObject, e3NetSegment)
            e3Sheet = CType(e3Job.CreateSheetObject, e3Sheet)
            e3Wire = CType(e3Job.CreatePinObject, e3Pin)

            Dim netSegmentIds As Object = Nothing
            Dim sheetIds As Object = Nothing
            Dim wireIds As Object = Nothing
            Dim cableids As Object = Nothing

            Dim idListForNetSegments As New List(Of Integer)
            Dim idListForSheets As New List(Of Integer)
            Dim cableCount As Integer = e3Job.GetSelectedCableIds(cableids)
            If (cableCount = 0) Then
                cableCount = e3Job.GetCableIds(cableids)
            End If

            For cableIdIndex As Integer = 1 To cableCount
                e3Cable.SetId(CInt(CType(cableids, IEnumerable)(cableIdIndex)))

                Dim wireCount As Integer = e3Cable.GetPinIds(wireIds)

                For wireIdIndex As Integer = 1 To wireCount
                    e3Wire.SetId(CInt(CType(wireIds, IEnumerable)(wireIdIndex)))

                    If (e3Wire.GetName = DirectCast(_kblMapper.KBLOccurrenceMapper(coreWireId), Wire_occurrence).Wire_number) Then
                        Dim netSegmentCount As Integer = e3Wire.GetNetSegmentIds(netSegmentIds)

                        For netSegmentIdIndex As Integer = 1 To netSegmentCount
                            e3NetSegment.SetId(CInt(CType(netSegmentIds, IEnumerable)(netSegmentIdIndex)))

                            Dim netSegmentId As Integer = e3NetSegment.GetId
                            If (netSegmentId <> 0) AndAlso (Not idListForNetSegments.Contains(netSegmentId)) Then
                                idListForNetSegments.Add(netSegmentId)
                            End If
                        Next

                        Exit For
                    End If
                Next
            Next

            For Each netSegmentId As Integer In idListForNetSegments
                e3Sheet.SetId(netSegmentId)

                Dim sheetId As Integer = e3Sheet.GetId
                If (sheetId <> 0) AndAlso (Not CBool(e3Sheet.IsFormboard) AndAlso Not CBool(e3Sheet.IsTopology)) AndAlso (Not idListForSheets.Contains(sheetId)) Then
                    idListForSheets.Add(sheetId)
                End If
            Next

            e3Job.ResetHighlight()

            If (idListForSheets.Count <= 0) Then
                _logEventArgs.LogLevel = LogEventArgs.LoggingLevel.Warning
                _logEventArgs.LogMessage = InformationHubStrings.NoSheetFound_LogMsg

                RaiseEvent LogMessage(Me, _logEventArgs)

                Exit Sub
            End If

            Dim sheetCount As Integer = e3Job.GetSheetIds(sheetIds)
            For sheetIdIndex As Integer = 1 To sheetCount
                e3Sheet.SetId(CInt(CType(sheetIds, IEnumerable)(sheetIdIndex)))
                e3Sheet.Remove()
            Next

            For Each sheetId As Integer In idListForSheets
                e3Sheet.SetId(sheetId)
                e3Sheet.Display()
            Next

            For Each netSegmentId As Integer In idListForNetSegments
                e3NetSegment.SetId(netSegmentId)
                e3NetSegment.Highlight()
            Next
        Catch ex As Exception
            ' Error while highlighting selected wire in E3 application
        Finally
            e3Wire = Nothing
            e3Sheet = Nothing
            e3NetSegment = Nothing
            e3Cable = Nothing
            e3Job = Nothing
            e3Application = Nothing
        End Try
    End Sub

    Private Sub udsWires_CellDataRequested(sender As Object, e As CellDataRequestedEventArgs) Handles udsWires.CellDataRequested
        e.CacheData = False

        If TypeOf e.Row.Tag Is Core_occurrence Then
            CoreCellDataRequested(e)
        ElseIf e.Row.Tag IsNot Nothing Then
            With DirectCast(e.Row.Tag, Wire_occurrence)
                Dim connection As Connection = Nothing
                Dim fromReference As Boolean = True
                Dim wirePart As General_wire = Nothing
                Dim wiringGroups As List(Of Wiring_group) = Nothing

                If HasChangeTypeWithInverse(e.Row, CompareChangeType.New) Then
                    fromReference = False

                    If (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLOccurrenceMapper.ContainsKey(.SystemId)) Then
                        If (_kblMapperForCompare.KBLWireNetMapper.ContainsKey(.SystemId)) Then
                            connection = DirectCast(_kblMapperForCompare.KBLWireNetMapper(.SystemId), Connection)
                        End If

                        If (_kblMapperForCompare.KBLPartMapper.ContainsKey(If(.Part Is Nothing, String.Empty, .Part))) Then
                            wirePart = DirectCast(_kblMapperForCompare.KBLPartMapper(.Part), General_wire)
                        End If

                        wiringGroups = _kblMapperForCompare.GetWiringGroups.ToList
                    End If
                Else
                    If (_kblMapper.KBLOccurrenceMapper.ContainsKey(.SystemId)) Then

                        If (_kblMapper.KBLWireNetMapper.ContainsKey(.SystemId)) Then
                            connection = DirectCast(_kblMapper.KBLWireNetMapper(.SystemId), Connection)
                        End If

                        If (_kblMapper.KBLPartMapper.ContainsKey(If(.Part Is Nothing, String.Empty, .Part))) Then
                            wirePart = DirectCast(_kblMapper.KBLPartMapper(.Part), General_wire)
                        End If

                        wiringGroups = _kblMapper.GetWiringGroups.ToList
                    End If
                End If

                RequestCellPartData(e.Data, fromReference, e.Column.Key, wirePart)

                Select Case e.Column.Key
                    Case PRIMARY_LENGTH_COLUMN_KEY
                        'HINT Daimler Gimmick to get the length of wires directly visible in kbl compare
                        'This is a temporary field filled with the selected length class of wires from general settings
                        If (.Length_information.Length > 0) Then
                            If (_kblMapperForCompare IsNot Nothing) Then
                                For Each wireLength As Wire_length In .Length_information
                                    If (wireLength.Length_type.ToLower = _generalSettings.DefaultWireLengthType.ToLower) Then
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
                    Case WIRE_CLASS_COLUMN_KEY
                        e.Data = E3.Lib.Schema.Kbl.Utils.GetLocalizedName(KblObjectType.Wire_occurrence, System.Globalization.CultureInfo.CurrentUICulture)
                    Case WirePropertyName.Wire_number
                        e.Data = .Wire_number
                    Case WirePropertyName.Wiring_group
                        If (.GetWiringGroup(wiringGroups) IsNot Nothing) Then
                            If (_kblMapperForCompare Is Nothing) Then
                                e.Data = .GetWiringGroup(wiringGroups).Id
                            Else
                                e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                            End If
                        End If
                    Case WirePropertyName.Cable_designator
                        If (wirePart IsNot Nothing) AndAlso (wirePart.Cable_designator IsNot Nothing) Then
                            e.Data = wirePart.Cable_designator
                        End If
                    Case WirePropertyName.Wire_type
                        If (wirePart IsNot Nothing) AndAlso (wirePart.Wire_type IsNot Nothing) Then
                            e.Data = wirePart.Wire_type
                        End If
                    Case WirePropertyName.Bend_radius
                        If (wirePart IsNot Nothing) AndAlso (wirePart.Bend_radius IsNot Nothing) Then
                            If (fromReference) AndAlso (_kblMapper.KBLUnitMapper.ContainsKey(wirePart.Bend_radius.Unit_component)) Then
                                e.Data = String.Format("{0} {1}", Math.Round(wirePart.Bend_radius.Value_component, 2), _kblMapper.KBLUnitMapper(wirePart.Bend_radius.Unit_component).Unit_name)
                            ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLUnitMapper.ContainsKey(wirePart.Bend_radius.Unit_component)) Then
                                e.Data = String.Format("{0} {1}", Math.Round(wirePart.Bend_radius.Value_component, 2), _kblMapperForCompare.KBLUnitMapper(wirePart.Bend_radius.Unit_component).Unit_name)
                            Else
                                e.Data = Math.Round(wirePart.Bend_radius.Value_component, 2)
                            End If
                        End If
                    Case WirePropertyName.Cross_section_area
                        If (wirePart IsNot Nothing) AndAlso (wirePart.Cross_section_area IsNot Nothing) Then
                            If (fromReference) AndAlso (_kblMapper.KBLUnitMapper.ContainsKey(wirePart.Cross_section_area.Unit_component)) Then
                                e.Data = String.Format("{0} {1}", Math.Round(wirePart.Cross_section_area.Value_component, 2), _kblMapper.KBLUnitMapper(wirePart.Cross_section_area.Unit_component).Unit_name)
                            ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLUnitMapper.ContainsKey(wirePart.Cross_section_area.Unit_component)) Then
                                e.Data = String.Format("{0} {1}", Math.Round(wirePart.Cross_section_area.Value_component, 2), _kblMapperForCompare.KBLUnitMapper(wirePart.Cross_section_area.Unit_component).Unit_name)
                            Else
                                e.Data = Math.Round(wirePart.Cross_section_area.Value_component, 2)
                            End If
                        End If
                    Case WirePropertyName.Outside_diameter
                        If (wirePart IsNot Nothing) AndAlso (wirePart.Outside_diameter IsNot Nothing) Then
                            If (fromReference) AndAlso (_kblMapper.KBLUnitMapper.ContainsKey(wirePart.Outside_diameter.Unit_component)) Then
                                e.Data = String.Format("{0} {1}", Math.Round(wirePart.Outside_diameter.Value_component, 2), _kblMapper.KBLUnitMapper(wirePart.Outside_diameter.Unit_component).Unit_name)
                            ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLUnitMapper.ContainsKey(wirePart.Outside_diameter.Unit_component)) Then
                                e.Data = String.Format("{0} {1}", Math.Round(wirePart.Outside_diameter.Value_component, 2), _kblMapperForCompare.KBLUnitMapper(wirePart.Outside_diameter.Unit_component).Unit_name)
                            Else
                                e.Data = Math.Round(wirePart.Outside_diameter.Value_component, 2)
                            End If
                        End If
                    Case WirePropertyName.Core_Colour
                        If (wirePart IsNot Nothing) Then
                            e.Data = wirePart.GetColours
                        End If
                    Case WirePropertyName.Installation_Information
                        If (.Installation_information.Length <> 0) Then
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
                            If (fromReference) AndAlso (_kblMapper.KBLOccurrenceMapper.ContainsKey(contactPointId)) Then
                                e.Data = DirectCast(_kblMapper.KBLPartMapper(DirectCast(_kblMapper.KBLOccurrenceMapper(DirectCast(_kblMapper.KBLOccurrenceMapper(contactPointId), Contact_point).Contacted_cavity.SplitSpace.FirstOrDefault), Cavity_occurrence).Part), Cavity).Cavity_number
                            ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLOccurrenceMapper.ContainsKey(contactPointId)) Then
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
                            If (fromReference) AndAlso (_kblMapper.KBLOccurrenceMapper.ContainsKey(contactPointId)) Then
                                e.Data = DirectCast(_kblMapper.KBLPartMapper(DirectCast(_kblMapper.KBLOccurrenceMapper(DirectCast(_kblMapper.KBLOccurrenceMapper(contactPointId), Contact_point).Contacted_cavity.SplitSpace.FirstOrDefault), Cavity_occurrence).Part), Cavity).Cavity_number
                            ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLOccurrenceMapper.ContainsKey(contactPointId)) Then
                                e.Data = DirectCast(_kblMapperForCompare.KBLPartMapper(DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(contactPointId), Contact_point).Contacted_cavity.SplitSpace.FirstOrDefault), Cavity_occurrence).Part), Cavity).Cavity_number
                            End If
                        End If

                    Case ConnectionPropertyName.Localized_description
                        If (connection IsNot Nothing) Then
                            If (connection.Localized_description.Length > 0) Then
                                If (connection.Localized_description.Length = 1) Then
                                    e.Data = connection.Localized_description(0).Value
                                Else
                                    e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                                End If
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

    Private Sub udsWires_InitializeDataRow(sender As Object, e As InitializeDataRowEventArgs) Handles udsWires.InitializeDataRow
        Dim wireOrCoreOccurrence As Object

        If (e.Row.Band.Tag Is Nothing) Then
            wireOrCoreOccurrence = _wiresAndCores(e.Row.Index)
        Else
            Dim compareObjKey As String = DirectCast(e.Row.Band.Tag, Dictionary(Of String, Object)).Keys.ElementAt(e.Row.Index)
            Dim compareObjValue As Object = DirectCast(e.Row.Band.Tag, Dictionary(Of String, Object)).Values.ElementAt(e.Row.Index)

            If (TypeOf compareObjValue Is Wire_occurrence) Then
                wireOrCoreOccurrence = DirectCast(compareObjValue, Wire_occurrence)
            Else
                wireOrCoreOccurrence = DirectCast(_kblMapper.KBLOccurrenceMapper(ExtractSystemId(compareObjKey)), Wire_occurrence)
            End If

            SetDiffCellValueFromObjectKey(e.Row, compareObjKey)
        End If

        e.Row.Tag = wireOrCoreOccurrence
    End Sub


    Private Sub ugWires_AfterRowActivate(sender As Object, e As EventArgs) Handles ugWires.AfterRowActivate
        If (Me.ugWires.ActiveRow.Appearance.BackColor = Color.FromArgb(190, 190, 190)) Then
            Me.ugWires.ActiveRow = Nothing
        ElseIf (_kblMapperForCompare IsNot Nothing) Then
            If (SetInformationHubEventArgs(Nothing, Me.ugWires.ActiveRow, Nothing)) Then
                OnHubSelectionChanged()
            End If
            If (Not Me.Focused) Then
                Me.Focus()
            End If
        End If
    End Sub

    Private Sub ugWires_AfterSelectChange(sender As Object, e As AfterSelectChangeEventArgs) Handles ugWires.AfterSelectChange
        With Me.ugWires
            .BeginUpdate()

            If (.ActiveRow IsNot Nothing) AndAlso (Not .ActiveRowScrollRegion.VisibleRows.Contains(.ActiveRow)) Then
                .ActiveRowScrollRegion.ScrollRowIntoView(.ActiveRow)
            End If

            .EndUpdate()

            If (SetInformationHubEventArgs(Nothing, Nothing, .Selected.Rows)) Then
                OnHubSelectionChanged()
            End If
        End With

        If (_generalSettings.E3ApplicationHighlightHooksEnabled) Then
            For Each selectedRow As UltraGridRow In Me.ugWires.Selected.Rows
                ShowWireInSchematics(selectedRow.Tag?.ToString)
            Next
        End If
    End Sub

    Private Sub ugWires_BeforeSelectChange(sender As Object, e As BeforeSelectChangeEventArgs) Handles ugWires.BeforeSelectChange
        For Each row As UltraGridRow In e.NewSelections.Rows
            If (row.Appearance.BackColor = Color.FromArgb(190, 190, 190)) Then
                e.Cancel = True
            End If
        Next
    End Sub

    Private Sub ugWires_ClickCell(sender As Object, e As ClickCellEventArgs) Handles ugWires.ClickCell
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
            _contextMenuGrid.ShowPopup()
        End If
    End Sub

    Private Sub ugWires_ClickCellButton(sender As Object, e As CellEventArgs) Handles ugWires.ClickCellButton
        If (_pressedMouseButton = System.Windows.Forms.MouseButtons.Left) Then
            Dim fromReference As Boolean = True

            If HasChangeTypeWithInverse(CType(e.Cell.Row.ListObject, UltraDataRow), CompareChangeType.New) Then
                fromReference = False
            End If

            If (TypeOf e.Cell.Tag Is Core_occurrence) Then
                With DirectCast(e.Cell.Tag, Core_occurrence)
                    If (fromReference) AndAlso (_kblMapper.KBLPartMapper.ContainsKey(DirectCast(_kblMapper.KBLOccurrenceMapper(_kblMapper.KBLCoreCableMapper(.SystemId)), Special_wire_occurrence).Part)) Then
                        RequestDialogPartData(_kblMapper, e.Cell.Column.Key, DirectCast(_kblMapper.KBLPartMapper(DirectCast(_kblMapper.KBLOccurrenceMapper(_kblMapper.KBLCoreCableMapper(.SystemId)), Special_wire_occurrence).Part), Part))
                    ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLPartMapper.ContainsKey(DirectCast(_kblMapper.KBLOccurrenceMapper(_kblMapper.KBLCoreCableMapper(.SystemId)), Special_wire_occurrence).Part)) Then
                        RequestDialogPartData(_kblMapperForCompare, e.Cell.Column.Key, DirectCast(_kblMapperForCompare.KBLPartMapper(DirectCast(_kblMapper.KBLOccurrenceMapper(_kblMapper.KBLCoreCableMapper(.SystemId)), Special_wire_occurrence).Part), Part))
                    End If

                    Select Case e.Cell.Column.Key
                        Case WirePropertyName.Installation_Information
                            ShowDetailInformationForm(InformationHubStrings.InstallInfo_Caption, DirectCast(_kblMapper.KBLOccurrenceMapper(_kblMapper.KBLCoreCableMapper(.SystemId)), Special_wire_occurrence).Installation_information, Nothing, _kblMapper)
                        Case WirePropertyName.Length_information
                            If (_generalSettings IsNot Nothing) Then
                                ShowDetailInformationForm(InformationHubStrings.LengthInfo_Caption, .Length_information, Nothing, _kblMapper, Nothing, _generalSettings.DefaultWireLengthType)
                            Else
                                ShowDetailInformationForm(InformationHubStrings.LengthInfo_Caption, .Length_information, Nothing, _kblMapper)
                            End If
                        Case WirePropertyName.Routing
                            Dim segments As New Dictionary(Of String, Segment)

                            If (fromReference) Then
                                For Each segment As Segment In _kblMapper.KBLWireSegmentMapper.GetSegmentsOrEmpty(.SystemId)
                                    segments.Add(segment.SystemId, segment)
                                Next
                            ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) Then
                                For Each segment As Segment In _kblMapperForCompare.KBLWireSegmentMapper.GetSegmentsOrEmpty(.SystemId)
                                    segments.Add(segment.SystemId, segment)
                                Next
                            End If

                            ShowDetailInformationForm(InformationHubStrings.RoutingInfo_Caption, segments, Nothing, _kblMapper)
                        Case WirePropertyName.Wiring_group
                            Dim wiringGroup As Wiring_group = .GetWiringGroup(_kblMapper.GetWiringGroups)
                            If (wiringGroup Is Nothing) Then
                                wiringGroup = .GetWiringGroup(_kblMapperForCompare.GetWiringGroups)
                            End If

                            ShowDetailInformationForm(KblObjectType.Wiring_group.ToLocalizedString, wiringGroup, Nothing, _kblMapper)
                        Case WirePropertyName.AdditionalExtremities
                            Dim connection As Connection = Nothing
                            Dim contactPoints As New List(Of String)

                            If (fromReference) Then
                                connection = If(_kblMapper.KBLWireNetMapper.ContainsKey(.SystemId), TryCast(_kblMapper.KBLWireNetMapper(.SystemId), Connection), Nothing)
                                If (connection IsNot Nothing) Then
                                    For Each extremity As Extremity In connection.Extremities.Where(Function(ex) ex.Position_on_wire <> 0 AndAlso ex.Position_on_wire <> 1)
                                        contactPoints.Add(String.Format("{0},{1}", DirectCast(_kblMapper.KBLOccurrenceMapper(_kblMapper.KBLContactPointConnectorMapper(extremity.Contact_point)), Connector_occurrence).Id, DirectCast(_kblMapper.KBLPartMapper(DirectCast(_kblMapper.KBLOccurrenceMapper(DirectCast(_kblMapper.KBLOccurrenceMapper(_kblMapper.KBLContactPointConnectorMapper(extremity.Contact_point)), Connector_occurrence).Contact_points.Single(Function(contactPoint) contactPoint.SystemId = extremity.Contact_point).Contacted_cavity), Cavity_occurrence).Part), Cavity).Cavity_number))
                                    Next
                                End If
                            ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) Then
                                connection = If(_kblMapperForCompare.KBLWireNetMapper.ContainsKey(.SystemId), TryCast(_kblMapperForCompare.KBLWireNetMapper(.SystemId), Connection), Nothing)
                                If (connection IsNot Nothing) Then
                                    For Each extremity As Extremity In connection.Extremities.Where(Function(ex) ex.Position_on_wire <> 0 AndAlso ex.Position_on_wire <> 1)
                                        contactPoints.Add(String.Format("{0},{1}", DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(_kblMapperForCompare.KBLContactPointConnectorMapper(extremity.Contact_point)), Connector_occurrence).Id, DirectCast(_kblMapperForCompare.KBLPartMapper(DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(DirectCast(_kblMapper.KBLOccurrenceMapper(_kblMapperForCompare.KBLContactPointConnectorMapper(extremity.Contact_point)), Connector_occurrence).Contact_points.Single(Function(contactPoint) contactPoint.SystemId = extremity.Contact_point).Contacted_cavity), Cavity_occurrence).Part), Cavity).Cavity_number))
                                    Next
                                End If
                            End If

                            ShowDetailInformationForm(InformationHubStrings.AddExtremities_Caption, contactPoints, Nothing, _kblMapper)
                    End Select
                End With
            ElseIf (TypeOf e.Cell.Tag Is Wire_occurrence) Then
                With DirectCast(e.Cell.Tag, Wire_occurrence)
                    If (fromReference) AndAlso (_kblMapper.KBLPartMapper.ContainsKey(If(.Part Is Nothing, String.Empty, .Part))) Then
                        RequestDialogPartData(_kblMapper, e.Cell.Column.Key, DirectCast(_kblMapper.KBLPartMapper(.Part), Part))
                    ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLPartMapper.ContainsKey(If(.Part Is Nothing, String.Empty, .Part))) Then
                        RequestDialogPartData(_kblMapperForCompare, e.Cell.Column.Key, DirectCast(_kblMapperForCompare.KBLPartMapper(.Part), Part))
                    End If

                    Select Case e.Cell.Column.Key
                        Case WirePropertyName.Installation_Information
                            ShowDetailInformationForm(InformationHubStrings.InstallInfo_Caption, .Installation_information, Nothing, _kblMapper)
                        Case WirePropertyName.Length_information
                            If (_generalSettings IsNot Nothing) Then
                                ShowDetailInformationForm(InformationHubStrings.LengthInfo_Caption, .Length_information, Nothing, _kblMapper, Nothing, _generalSettings.DefaultWireLengthType)
                            Else
                                ShowDetailInformationForm(InformationHubStrings.LengthInfo_Caption, .Length_information, Nothing, _kblMapper)
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

                            ShowDetailInformationForm(InformationHubStrings.RoutingInfo_Caption, segments, Nothing, _kblMapper)
                        Case WirePropertyName.Wiring_group
                            Dim wiringGroup As Wiring_group = .GetWiringGroup(_kblMapper.GetWiringGroups)
                            If (wiringGroup Is Nothing) Then
                                wiringGroup = .GetWiringGroup(_kblMapperForCompare.GetWiringGroups)
                            End If

                            ShowDetailInformationForm(KblObjectType.Wiring_group.ToLocalizedString, wiringGroup, Nothing, _kblMapper)
                        Case WirePropertyName.AdditionalExtremities
                            Dim contactPoints As New List(Of String)

                            If (fromReference) Then
                                Dim connection As Connection = If(_kblMapper.KBLWireNetMapper.ContainsKey(.SystemId), TryCast(_kblMapper.KBLWireNetMapper(.SystemId), Connection), Nothing)
                                If (connection IsNot Nothing) Then
                                    For Each extremity As Extremity In connection.Extremities.Where(Function(ex) ex.Position_on_wire <> 0 AndAlso ex.Position_on_wire <> 1)
                                        contactPoints.Add(String.Format("{0},{1}", DirectCast(_kblMapper.KBLOccurrenceMapper(_kblMapper.KBLContactPointConnectorMapper(extremity.Contact_point)), Connector_occurrence).Id, DirectCast(_kblMapper.KBLPartMapper(DirectCast(_kblMapper.KBLOccurrenceMapper(DirectCast(_kblMapper.KBLOccurrenceMapper(_kblMapper.KBLContactPointConnectorMapper(extremity.Contact_point)), Connector_occurrence).Contact_points.Single(Function(contactPoint) contactPoint.SystemId = extremity.Contact_point).Contacted_cavity), Cavity_occurrence).Part), Cavity).Cavity_number))
                                    Next
                                End If
                            ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) Then
                                Dim connection As Connection = If(_kblMapperForCompare.KBLWireNetMapper.ContainsKey(.SystemId), TryCast(_kblMapperForCompare.KBLWireNetMapper(.SystemId), Connection), Nothing)
                                If (connection IsNot Nothing) Then
                                    For Each extremity As Extremity In connection.Extremities.Where(Function(ex) ex.Position_on_wire <> 0 AndAlso ex.Position_on_wire <> 1)
                                        contactPoints.Add(String.Format("{0},{1}", DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(_kblMapperForCompare.KBLContactPointConnectorMapper(extremity.Contact_point)), Connector_occurrence).Id, DirectCast(_kblMapperForCompare.KBLPartMapper(DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(DirectCast(_kblMapper.KBLOccurrenceMapper(_kblMapperForCompare.KBLContactPointConnectorMapper(extremity.Contact_point)), Connector_occurrence).Contact_points.Single(Function(contactPoint) contactPoint.SystemId = extremity.Contact_point).Contacted_cavity), Cavity_occurrence).Part), Cavity).Cavity_number))
                                    Next
                                End If
                            End If

                            ShowDetailInformationForm(InformationHubStrings.AddExtremities_Caption, contactPoints, Nothing, _kblMapper)
                    End Select
                End With
            ElseIf (TypeOf e.Cell.Tag Is KeyValuePair(Of String, Object)) Then
                RequestDialogCompareData(True, DirectCast(e.Cell.Row.Tag, KeyValuePair(Of String, Object)).Key.SplitRemoveEmpty("|"c)(1), DirectCast(e.Cell.Tag, KeyValuePair(Of String, Object)))
            End If
        End If
    End Sub

    Private Sub ugWires_DoubleClickRow(sender As Object, e As DoubleClickRowEventArgs) Handles ugWires.DoubleClickRow
        OnDoubleClickRow(sender, e)
    End Sub

    Private Sub ugWires_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugWires.InitializeLayout
        Me.ugWires.BeginUpdate()
        Me.ugWires.EventManager.AllEventsEnabled = False

        InitializeGridLayout(_wireGridAppearance, e.Layout)

        If (_kblMapperForCompare Is Nothing) AndAlso (e.Layout.Bands(0).Columns.Exists(WIRE_CLASS_COLUMN_KEY)) Then
            e.Layout.Bands(0).Columns(WIRE_CLASS_COLUMN_KEY).Header.Caption = InformationHubStrings.Class_Caption
            e.Layout.Bands(0).Columns(WIRE_CLASS_COLUMN_KEY).Width = 60
        End If

        If (_kblMapperForCompare IsNot Nothing) AndAlso (e.Layout.Bands(0).Columns.Exists(PRIMARY_LENGTH_COLUMN_KEY)) Then
            e.Layout.Bands(0).Columns(PRIMARY_LENGTH_COLUMN_KEY).Header.Caption = String.Format("{0} [{1}]", InformationHubStrings.PrimaryLength_Caption, _generalSettings.DefaultWireLengthType.Split(" ".ToCharArray, StringSplitOptions.RemoveEmptyEntries)(0))
            e.Layout.Bands(0).Columns(PRIMARY_LENGTH_COLUMN_KEY).Width = 150
        End If

        If (_kblMapperForCompare Is Nothing) AndAlso (e.Layout.Bands(0).Columns.Exists(WirePropertyName.Length_information)) AndAlso (_kblMapper.KBLWireLengthTypes.Count > 1) AndAlso (_generalSettings.DefaultWireLengthType <> String.Empty) Then
            e.Layout.Bands(0).Columns(WirePropertyName.Length_information).Header.Caption = String.Format("{0} [{1}]", e.Layout.Bands(0).Columns(WirePropertyName.Length_information.ToString).Header.Caption, _generalSettings.DefaultWireLengthType.SplitSpace(0))
        End If

        Me.ugWires.EventManager.AllEventsEnabled = True
        Me.ugWires.EndUpdate()
    End Sub

    Private Sub InitializeWireRowCore(row As UltraGridRow)
        If (DirectCast(row.ListObject, UltraDataRow).Band.Tag Is Nothing) Then
            If (TypeOf DirectCast(row.ListObject, UltraDataRow).Tag Is Core_occurrence) Then
                row.Tag = DirectCast(DirectCast(row.ListObject, UltraDataRow).Tag, Core_occurrence).SystemId
            Else
                row.Tag = DirectCast(DirectCast(row.ListObject, UltraDataRow).Tag, Wire_occurrence).SystemId
            End If

            If (_redliningInformation IsNot Nothing) AndAlso (_redliningInformation.Redlinings.Where(Function(redlining) redlining.ObjectId = row.Tag?.ToString).Any()) Then
                row.Cells(1).Appearance.Image = My.Resources.Redlining
            End If
        Else
            InitializeRowForCompare(DirectCast(DirectCast(row.ListObject, UltraDataRow).Band.Tag, Dictionary(Of String, Object)), row)
        End If

        For Each cell As UltraGridCell In row.Cells
            If (cell.Value IsNot Nothing AndAlso cell.Value.ToString = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS) Then
                cell.Style = ColumnStyle.Button

                If (TypeOf row.Tag Is KeyValuePair(Of String, Object)) Then
                    Dim compareObject As KeyValuePair(Of String, Object) = DirectCast(row.Tag, KeyValuePair(Of String, Object))

                    If (TypeOf compareObject.Value Is WireChangedProperty) Then
                        For Each changedProperty As KeyValuePair(Of String, Object) In DirectCast(compareObject.Value, WireChangedProperty).ChangedProperties
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
            ElseIf (cell.Column.Key = WirePropertyName.Wiring_group) AndAlso (cell.Value.ToString <> String.Empty) AndAlso (_kblMapperForCompare Is Nothing) Then
                Dim wiringGroup As Wiring_group = Nothing

                If (TypeOf _kblMapper.KBLOccurrenceMapper(row.Tag?.ToString) Is Core_occurrence) Then
                    wiringGroup = DirectCast(_kblMapper.KBLOccurrenceMapper(row.Tag?.ToString), Core_occurrence).GetWiringGroup(_kblMapper.GetWiringGroups)
                ElseIf (TypeOf _kblMapper.KBLOccurrenceMapper(row.Tag?.ToString) Is Wire_occurrence) Then
                    wiringGroup = DirectCast(_kblMapper.KBLOccurrenceMapper(row.Tag?.ToString), Wire_occurrence).GetWiringGroup(_kblMapper.GetWiringGroups)
                End If

                If (wiringGroup.GetConsistencyState(_kblMapper) <> [Lib].Schema.Kbl.WiringGroupConsistencyState.Valid) Then
                    cell.Appearance.Image = My.Resources.MismatchingConfig_Small
                End If

                cell.Style = ColumnStyle.Button
                cell.Tag = _kblMapper.KBLOccurrenceMapper(row.Tag?.ToString)
            ElseIf (cell.Column.Key = WirePropertyName.Length_information.ToString) AndAlso (_kblMapperForCompare Is Nothing) Then
                Dim wireLengthTypes As New List(Of String)

                If (TypeOf _kblMapper.KBLOccurrenceMapper(row.Tag?.ToString) Is Core_occurrence) AndAlso (DirectCast(_kblMapper.KBLOccurrenceMapper(row.Tag?.ToString), Core_occurrence).Length_information.Length <> 0) Then
                    For Each wireLength As Wire_length In DirectCast(_kblMapper.KBLOccurrenceMapper(row.Tag?.ToString), Core_occurrence).Length_information
                        wireLengthTypes.Add(wireLength.Length_type)
                    Next
                ElseIf (TypeOf _kblMapper.KBLOccurrenceMapper(row.Tag?.ToString) Is Wire_occurrence) AndAlso (DirectCast(_kblMapper.KBLOccurrenceMapper(row.Tag?.ToString), Wire_occurrence).Length_information.Length <> 0) Then
                    For Each wireLength As Wire_length In DirectCast(_kblMapper.KBLOccurrenceMapper(row.Tag?.ToString), Wire_occurrence).Length_information
                        wireLengthTypes.Add(wireLength.Length_type)
                    Next
                End If

                If (wireLengthTypes.Count > 1) OrElse (Not wireLengthTypes.Any(Function(lt) lt.ToLower = _generalSettings.DefaultWireLengthType.ToLower)) Then
                    cell.Appearance.Image = My.Resources.About_Small
                    cell.Style = ColumnStyle.Button
                    cell.Tag = _kblMapper.KBLOccurrenceMapper(row.Tag?.ToString)
                End If
            ElseIf (cell.Column.Key = ConnectionPropertyName.Cavity_A.ToString OrElse cell.Column.Key = ConnectionPropertyName.Cavity_B) AndAlso (_kblMapperForCompare Is Nothing) Then
                Dim connection As Connection = If(_kblMapper.KBLWireNetMapper.ContainsKey(row.Tag?.ToString), DirectCast(_kblMapper.KBLOccurrenceMapper(_kblMapper.KBLWireNetMapper(row.Tag?.ToString).SystemId), Connection), Nothing)
                Dim startTerminalPlating As String = String.Empty
                Dim endTerminalPlating As String = String.Empty

                If (connection IsNot Nothing) AndAlso (_kblMapper.KBLOccurrenceMapper.ContainsKey(connection.GetStartContactPointId)) AndAlso (DirectCast(_kblMapper.KBLOccurrenceMapper(connection.GetStartContactPointId), Contact_point).Associated_parts IsNot Nothing) Then
                    For Each associatedPartId As String In DirectCast(_kblMapper.KBLOccurrenceMapper(connection.GetStartContactPointId), Contact_point).Associated_parts.SplitSpace
                        If (_kblMapper.KBLOccurrenceMapper.ContainsKey(associatedPartId)) AndAlso (TypeOf _kblMapper.KBLOccurrenceMapper(associatedPartId) Is Special_terminal_occurrence OrElse TypeOf _kblMapper.KBLOccurrenceMapper(associatedPartId) Is Terminal_occurrence) Then
                            Dim terminalPart As General_terminal = If(_kblMapper.GetHarness.GetSpecialTerminalOccurrence(associatedPartId) IsNot Nothing, _kblMapper.GetGeneralTerminal(_kblMapper.GetHarness.GetSpecialTerminalOccurrence(associatedPartId).Part), _kblMapper.GetGeneralTerminal(_kblMapper.GetHarness.GetTerminalOccurrence(associatedPartId).Part))
                            If (terminalPart IsNot Nothing) Then
                                startTerminalPlating = terminalPart.Plating_material
                            End If
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
    End Sub

    Private Sub ugWires_InitializeRow(sender As Object, e As InitializeRowEventArgs) Handles ugWires.InitializeRow
        If Not e.ReInitialize Then
            InitializeWireRowCore(e.Row)
        End If
    End Sub

    Private Sub ugWires_KeyDown(sender As Object, e As KeyEventArgs) Handles ugWires.KeyDown
        OnGridKeyDown(Me, New GridKeyDownEventArgs(e, DirectCast(sender, UltraGrid)))

        If Not e.Handled Then
            If (e.Control) AndAlso (e.KeyCode = Keys.A) Then
                SelectAllRowsOfActiveGrid()
            ElseIf (e.KeyCode = Keys.Escape) Then
                If (Me.ugWires.Selected.Rows.Count = 0) Then
                    ClearMarkedRowsInGrids()
                Else
                    ClearSelectedRowsInGrids()
                End If

                If (SetInformationHubEventArgs(Nothing, Nothing, Me.ugWires.Selected.Rows)) Then
                    OnHubSelectionChanged()
                End If

            ElseIf (e.KeyCode = Keys.Return) Then
                ToggleMarkedOriginRows(ugWires.Selected.Rows.Cast(Of UltraGridRow))
            End If
        End If
    End Sub

    Private Sub ugWires_MouseDown(sender As Object, e As MouseEventArgs) Handles ugWires.MouseDown
        If (_contextMenuGrid.IsOpen) Then
            _contextMenuGrid.ClosePopup()
        End If
        _pressedMouseButton = e.Button
    End Sub

    Private Sub ugWires_MouseLeave(sender As Object, e As EventArgs) Handles ugWires.MouseLeave
        _messageEventArgs.StatusMessage = String.Empty
        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub ugWires_MouseMove(sender As Object, e As MouseEventArgs) Handles ugWires.MouseMove
        _messageEventArgs.StatusMessage = String.Format(InformationHubStrings.RowCount_Label, Me.ugWires.Rows.Count, Me.ugWires.Rows.FilteredInNonGroupByRowCount, Me.ugWires.Rows.VisibleRowCount, Me.ugWires.Selected.Rows.Count)
        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub udsWires_CellDataUpdated(sender As Object, e As CellDataUpdatedEventArgs) Handles udsWires.CellDataUpdated
        RaiseEvent CellValueUpdated(sender, e)
    End Sub

End Class