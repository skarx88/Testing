Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinGrid
Imports Zuken.E3.Lib.Schema.Kbl.Properties
Imports Zuken.E3.HarnessAnalyzer.Shared

Partial Public Class InformationHub

    Private Sub InitializeSegments()
        _segmentGridAppearance = GridAppearance.All.OfType(Of SegmentGridAppearance).Single
        AddNewRowFilters(ugSegments)

        If _comparisonMapperList Is Nothing Then
            FillDataSource(Me.udsSegments, _segmentGridAppearance, _kblMapper.GetSegments.Count)
        Else
            FillDataSource(Me.udsSegments, _segmentGridAppearance, 0, _comparisonMapperList(KblObjectType.Segment))
        End If

        If (Me.udsSegments.Band.Columns.Count = 0) OrElse (Me.udsSegments.Rows.Count = 0) Then
            Me.utcInformationHub.Tabs(TabNames.tabSegments.ToString()).Visible = False

        End If
    End Sub

    Private Sub SegmentCellDataRequested(e As CellDataRequestedEventArgs)
        With DirectCast(e.Row.Tag, Segment)
            Dim fromReference As Boolean = True

            If HasChangeTypeWithInverse(e.Row, CompareChangeType.New) Then
                fromReference = False
            End If

            Select Case e.Column.Key
                Case SegmentPropertyName.Id
                    e.Data = .Id
                Case SegmentPropertyName.Alias_id
                    If (.Alias_id.Length <> 0) Then
                        If (.Alias_id.Length = 1) AndAlso (.Alias_id(0).Description Is Nothing) AndAlso (.Alias_id(0).Scope Is Nothing) Then
                            e.Data = .Alias_id(0).Alias_id
                        Else
                            e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                        End If
                    End If
                Case SegmentPropertyName.Virtual_length
                    If (.Virtual_length IsNot Nothing) Then
                        If (fromReference) AndAlso (_kblMapper.KBLUnitMapper.ContainsKey(.Virtual_length.Unit_component)) Then
                            e.Data = String.Format("{0} {1}", Math.Round(.Virtual_length.Value_component, 2), _kblMapper.KBLUnitMapper(.Virtual_length.Unit_component).Unit_name)
                        ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLUnitMapper.ContainsKey(.Virtual_length.Unit_component)) Then
                            e.Data = String.Format("{0} {1}", Math.Round(.Virtual_length.Value_component, 2), _kblMapperForCompare.KBLUnitMapper(.Virtual_length.Unit_component).Unit_name)
                        Else
                            e.Data = Math.Round(.Virtual_length.Value_component, 2)
                        End If
                    End If
                Case SegmentPropertyName.Physical_length
                    If (.Physical_length IsNot Nothing) Then
                        If (fromReference) AndAlso (_kblMapper.KBLUnitMapper.ContainsKey(.Physical_length.Unit_component)) Then
                            e.Data = String.Format("{0} {1}", Math.Round(.Physical_length.Value_component, 2), _kblMapper.KBLUnitMapper(.Physical_length.Unit_component).Unit_name)
                        ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLUnitMapper.ContainsKey(.Physical_length.Unit_component)) Then
                            e.Data = String.Format("{0} {1}", Math.Round(.Physical_length.Value_component, 2), _kblMapperForCompare.KBLUnitMapper(.Physical_length.Unit_component).Unit_name)
                        Else
                            e.Data = Math.Round(.Physical_length.Value_component, 2)
                        End If
                    End If
                Case SegmentPropertyName.Form
                    If .FormSpecified Then
                        e.Data = .Form
                    End If
                Case SegmentPropertyName.End_node
                    If (fromReference) AndAlso (_kblMapper.KBLOccurrenceMapper.ContainsKey(.End_node)) AndAlso (TypeOf _kblMapper.KBLOccurrenceMapper(.End_node) Is Node) Then
                        e.Data = DirectCast(_kblMapper.KBLOccurrenceMapper(.End_node), Node).Id
                    ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLOccurrenceMapper.ContainsKey(.End_node)) AndAlso (TypeOf _kblMapperForCompare.KBLOccurrenceMapper(.End_node) Is Node) Then
                        e.Data = DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(.End_node), Node).Id
                    End If
                Case SegmentPropertyName.Start_node
                    If (fromReference) AndAlso (_kblMapper.KBLOccurrenceMapper.ContainsKey(.Start_node)) AndAlso (TypeOf _kblMapper.KBLOccurrenceMapper(.Start_node) Is Node) Then
                        e.Data = DirectCast(_kblMapper.KBLOccurrenceMapper(.Start_node), Node).Id
                    ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLOccurrenceMapper.ContainsKey(.Start_node)) AndAlso (TypeOf _kblMapperForCompare.KBLOccurrenceMapper(.Start_node) Is Node) Then
                        e.Data = DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(.Start_node), Node).Id
                    End If
                Case SegmentPropertyName.Cross_Section_Area_information
                    If (.Cross_section_area_information.Length <> 0) Then
                        If (.Cross_section_area_information.Length = 1) AndAlso (.Cross_section_area_information(0).Area IsNot Nothing) Then
                            If (fromReference) AndAlso (_kblMapper.KBLUnitMapper.ContainsKey(.Cross_section_area_information(0).Area.Unit_component)) Then
                                e.Data = String.Format("{0} {1}", Math.Round(.Cross_section_area_information(0).Area.Value_component, 2), _kblMapper.KBLUnitMapper(.Cross_section_area_information(0).Area.Unit_component).Unit_name)
                            ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLUnitMapper.ContainsKey(.Cross_section_area_information(0).Area.Unit_component)) Then
                                e.Data = String.Format("{0} {1}", Math.Round(.Cross_section_area_information(0).Area.Value_component, 2), _kblMapperForCompare.KBLUnitMapper(.Cross_section_area_information(0).Area.Unit_component).Unit_name)
                            Else
                                e.Data = Math.Round(.Cross_section_area_information(0).Area.Value_component, 2)
                            End If
                        Else
                            e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                        End If
                    End If
                Case SegmentPropertyName.Fixing_Assignment
                    If (.Fixing_assignment.Length > 0) Then
                        e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                    End If
                Case SegmentPropertyName.Processing_information
                    If (.Processing_information.Length > 0) Then
                        e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                    End If
                Case NameOf(InformationHubStrings.AssignedModules_ColumnCaption)
                    e.Data = GetAssignedModules(.SystemId)
                Case Else
                    OnUnhandledCellDataRequested(e)
            End Select
        End With
    End Sub

    Private Sub WireProtectionCellDataRequested(e As CellDataRequestedEventArgs)
        Dim fromReference As Boolean = True
        Dim protectionArea As Protection_area = TryCast(e.Row.Tag, Protection_area)
        Dim wireProtectionOccurrence As Wire_protection_occurrence = Nothing
        Dim wireProtectionPart As Wire_protection = Nothing

        If HasChangeTypeWithInverse(e.Row, CompareChangeType.New) Then
            fromReference = False

            If (_kblMapperForCompare IsNot Nothing) AndAlso (protectionArea Is Nothing) Then
                wireProtectionOccurrence = DirectCast(e.Row.Tag, Wire_protection_occurrence)
            End If

            If (_kblMapperForCompare IsNot Nothing) Then
                Dim associated_prot_id As String = If(protectionArea IsNot Nothing, protectionArea.Associated_protection, wireProtectionOccurrence.SystemId)
                Dim compare_prot_occ As Wire_protection_occurrence = _kblMapperForCompare?.GetOccurrenceObject(Of Wire_protection_occurrence)(associated_prot_id)
                If compare_prot_occ IsNot Nothing Then
                    wireProtectionOccurrence = compare_prot_occ
                    wireProtectionPart = _kblMapperForCompare.GetPart(Of Wire_protection)(wireProtectionOccurrence.Part)
                End If
            Else
                Return
            End If
        Else
            If (protectionArea Is Nothing) Then
                wireProtectionOccurrence = DirectCast(e.Row.Tag, Wire_protection_occurrence)
            End If

            Dim associated_prot_id As String = If(protectionArea IsNot Nothing, protectionArea.Associated_protection, wireProtectionOccurrence.SystemId)
            Dim prot_occ As Wire_protection_occurrence = _kblMapper.GetOccurrenceObject(Of Wire_protection_occurrence)(associated_prot_id)
            If prot_occ IsNot Nothing Then
                If (protectionArea IsNot Nothing) Then
                    wireProtectionOccurrence = prot_occ
                End If
                wireProtectionPart = _kblMapper.GetPart(Of Wire_protection)(wireProtectionOccurrence.Part)
            Else
                Return
            End If
        End If

        RequestCellPartData(e.Data, fromReference, e.Column.Key, wireProtectionPart)

        With wireProtectionOccurrence
            Select Case e.Column.Key
                Case SYSTEM_ID_COLUMN_KEY
                    e.Data = If(protectionArea IsNot Nothing, protectionArea.SystemId, wireProtectionOccurrence.SystemId)
                Case SegmentPropertyName.Start_node
                    If (protectionArea IsNot Nothing) Then
                        Dim seg As Segment = CType(e.Row.ParentRow.Tag, Segment)
                        If (seg IsNot Nothing) Then
                            Dim start_node As Node = _kblMapper.GetOccurrenceObject(Of Node)(seg.Start_node)
                            If start_node IsNot Nothing Then
                                e.Data = start_node.Id
                            End If
                        End If
                    End If
                Case ProtectionAreaPropertyName.Start_location
                    If (protectionArea IsNot Nothing) Then
                        If (Double.IsNaN(protectionArea.Start_location)) OrElse (Double.IsInfinity(protectionArea.Start_location)) Then
                            e.Data = String.Format("{0} %", 0)
                        Else
                            e.Data = String.Format("{0} %", Math.Round(protectionArea.Start_location * 100, NOF_DIGITS_LOCATIONS))
                        End If
                    End If
                Case ProtectionAreaPropertyName.Absolute_start_location
                    If (protectionArea IsNot Nothing) Then
                        If (protectionArea.Absolute_start_location IsNot Nothing) Then
                            If (fromReference) AndAlso (_kblMapper.KBLUnitMapper.ContainsKey(protectionArea.Absolute_start_location.Unit_component)) Then
                                e.Data = String.Format("{0} {1}", Math.Round(protectionArea.Absolute_start_location.Value_component, 2), _kblMapper.KBLUnitMapper(protectionArea.Absolute_start_location.Unit_component).Unit_name)
                            ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLUnitMapper.ContainsKey(protectionArea.Absolute_start_location.Unit_component)) Then
                                e.Data = String.Format("{0} {1}", Math.Round(protectionArea.Absolute_start_location.Value_component, 2), _kblMapperForCompare.KBLUnitMapper(protectionArea.Absolute_start_location.Unit_component).Unit_name)
                            Else
                                e.Data = Math.Round(protectionArea.Absolute_start_location.Value_component, 2)
                            End If
                        End If
                    End If
                Case ProtectionAreaPropertyName.End_location
                    If (protectionArea IsNot Nothing) Then
                        If (Double.IsNaN(protectionArea.End_location)) OrElse (Double.IsInfinity(protectionArea.End_location)) Then
                            e.Data = String.Format("{0} %", 0)
                        Else
                            e.Data = String.Format("{0} %", Math.Round(protectionArea.End_location * 100, NOF_DIGITS_LOCATIONS))
                        End If
                    End If
                Case ProtectionAreaPropertyName.Absolute_end_location
                    If (protectionArea IsNot Nothing) Then
                        If (protectionArea.Absolute_end_location IsNot Nothing) Then
                            If (fromReference) AndAlso (_kblMapper.KBLUnitMapper.ContainsKey(protectionArea.Absolute_end_location.Unit_component)) Then
                                e.Data = String.Format("{0} {1}", Math.Round(protectionArea.Absolute_end_location.Value_component, 2), _kblMapper.KBLUnitMapper(protectionArea.Absolute_end_location.Unit_component).Unit_name)
                            ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLUnitMapper.ContainsKey(protectionArea.Absolute_end_location.Unit_component)) Then
                                e.Data = String.Format("{0} {1}", Math.Round(protectionArea.Absolute_end_location.Value_component, 2), _kblMapperForCompare.KBLUnitMapper(protectionArea.Absolute_end_location.Unit_component).Unit_name)
                            Else
                                e.Data = Math.Round(protectionArea.Absolute_end_location.Value_component, 2)
                            End If
                        End If
                    End If
                Case ProtectionAreaPropertyName.Taping_direction
                    If (protectionArea IsNot Nothing) Then
                        If (protectionArea.Taping_direction IsNot Nothing) Then
                            e.Data = protectionArea.Taping_direction
                        End If
                    End If
                Case WireProtectionPropertyName.Id
                    e.Data = .Id
                Case WireProtectionPropertyName.Alias_id
                    If (.Alias_id.Length <> 0) Then
                        If (.Alias_id.Length = 1) AndAlso (.Alias_id(0).Description Is Nothing) AndAlso (.Alias_id(0).Scope Is Nothing) Then
                            e.Data = .Alias_id(0).Alias_id
                        Else
                            e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                        End If
                    End If
                Case WireProtectionPropertyName.Description
                    If (.Description IsNot Nothing) Then
                        e.Data = .Description
                    End If
                Case WireProtectionPropertyName.Protection_length
                    If (.Protection_length IsNot Nothing) Then
                        If (fromReference) AndAlso (_kblMapper.KBLUnitMapper.ContainsKey(.Protection_length.Unit_component)) Then
                            e.Data = String.Format("{0} {1}", Math.Round(.Protection_length.Value_component, 2), _kblMapper.KBLUnitMapper(.Protection_length.Unit_component).Unit_name)
                        ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLUnitMapper.ContainsKey(.Protection_length.Unit_component)) Then
                            e.Data = String.Format("{0} {1}", Math.Round(.Protection_length.Value_component, 2), _kblMapperForCompare.KBLUnitMapper(.Protection_length.Unit_component).Unit_name)
                        Else
                            e.Data = Math.Round(.Protection_length.Value_component, 2)
                        End If
                    End If
                Case WireProtectionPropertyName.Winding_type
                    If (.Winding_type IsNot Nothing) Then
                        e.Data = .Winding_type
                    End If
                Case WireProtectionPropertyName.Winding_firmness
                    If (.Winding_firmness IsNot Nothing) Then
                        e.Data = .Winding_firmness
                    End If
                Case WireProtectionPropertyName.Protection_type
                    If (wireProtectionPart.Protection_type IsNot Nothing) Then
                        e.Data = wireProtectionPart.Protection_type
                    End If
                Case WireProtectionPropertyName.Type_dependent_parameter
                    If (wireProtectionPart.Type_dependent_parameter IsNot Nothing) Then
                        e.Data = wireProtectionPart.Type_dependent_parameter
                    End If
                Case WireProtectionPropertyName.Installation_Information
                    If (.Installation_information.Length <> 0) Then
                        e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                    End If
                Case ProtectionAreaPropertyName.Is_on_top_of
                    If (protectionArea IsNot Nothing) Then
                        If (protectionArea.Is_on_top_of IsNot Nothing) Then
                            e.Data = protectionArea.GetIsOnTopOfProtections(_kblMapper)
                        End If
                    End If

                Case ProtectionAreaPropertyName.Processing_information
                    If (protectionArea IsNot Nothing) Then
                        If (protectionArea.Processing_information.Length <> 0) Then
                            e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                        End If
                    End If
                Case NameOf(InformationHubStrings.AssignedModules_ColumnCaption)
                    e.Data = GetAssignedModules(.SystemId)
                Case Else
                    OnUnhandledCellDataRequested(e)
            End Select
        End With
    End Sub

    Private Sub udsSegments_CellDataRequested(sender As Object, e As CellDataRequestedEventArgs) Handles udsSegments.CellDataRequested
        e.CacheData = False

        If (e.Row.ParentRow Is Nothing) Then
            SegmentCellDataRequested(e)
        Else
            WireProtectionCellDataRequested(e)
        End If
    End Sub

    Private Sub udsSegments_InitializeDataRow(sender As Object, e As InitializeDataRowEventArgs) Handles udsSegments.InitializeDataRow
        If (e.Row.ParentRow Is Nothing) Then
            Dim segment As Segment = Nothing
            Dim rowCount As Integer = 0

            If (e.Row.Band.Tag Is Nothing) Then
                segment = _kblMapper.GetSegments(e.Row.Index)
                rowCount = segment.Protection_area.Length
            Else
                Dim compareObjKey As String = DirectCast(e.Row.Band.Tag, Dictionary(Of String, Object)).Keys.ElementAt(e.Row.Index)
                Dim compareObjValue As Object = DirectCast(e.Row.Band.Tag, Dictionary(Of String, Object)).Values.ElementAt(e.Row.Index)

                If (TypeOf compareObjValue Is Segment) Then
                    segment = DirectCast(compareObjValue, Segment)
                Else
                    segment = DirectCast(_kblMapper.KBLOccurrenceMapper(ExtractSystemId(compareObjKey)), Segment)
                End If

                Dim compareObjects As New Dictionary(Of String, Object)
                If (TypeOf compareObjValue Is SegmentChangedProperty) Then
                    Dim changeProperty As ChangedProperty = DirectCast(compareObjValue, SegmentChangedProperty)

                    If (changeProperty.ChangedProperties.ContainsKey(SegmentPropertyName.Protection_Area)) Then
                        Dim comparisonMapper As ProtectionAreaComparisonMapper = DirectCast(changeProperty.ChangedProperties(SegmentPropertyName.Protection_Area), ProtectionAreaComparisonMapper)
                        For Each cItem As ChangedItem In comparisonMapper.Changes
                            compareObjects.Add(String.Format("{0}|{1}", cItem.Text, cItem.Key), cItem.Item)
                        Next
                    End If
                End If

                rowCount = compareObjects.Count

                If e.Row.Band.ChildBands.Count > 0 Then
                    e.Row.GetChildRows(0).Tag = compareObjects
                End If
                SetDiffCellValueFromObjectKey(e.Row, compareObjKey)
            End If

            e.Row.Tag = segment
            If e.Row.Band.ChildBands.Count > 0 Then
                e.Row.GetChildRows(0).SetCount(rowCount)
            End If
        Else
            If (e.Row.ParentCollection.Tag Is Nothing) Then
                e.Row.Tag = DirectCast(e.Row.ParentRow.Tag, Segment).Protection_area(e.Row.Index)
            Else
                Dim compareObjKey As String = DirectCast(e.Row.ParentCollection.Tag, Dictionary(Of String, Object)).Keys.ElementAt(e.Row.Index)
                Dim compareObjValue As Object = DirectCast(e.Row.ParentCollection.Tag, Dictionary(Of String, Object)).Values.ElementAt(e.Row.Index)
                Dim wireProtection As Wire_protection_occurrence = Nothing

                If (TypeOf compareObjValue Is Protection_area) Then
                    e.Row.Tag = compareObjValue
                ElseIf (TypeOf compareObjValue Is Wire_protection_occurrence) Then
                    wireProtection = DirectCast(compareObjValue, Wire_protection_occurrence)
                Else
                    wireProtection = _kblMapper.GetOccurrenceObject(Of Wire_protection_occurrence)(ExtractSystemId(compareObjKey))
                End If

                SetDiffCellValueFromObjectKey(e.Row, compareObjKey)

                If (wireProtection IsNot Nothing) Then
                    For Each protectionArea As Protection_area In DirectCast(e.Row.ParentRow.Tag, Segment).Protection_Area
                        If (protectionArea.Associated_protection = wireProtection.SystemId) Then
                            e.Row.Tag = protectionArea
                            Exit For
                        End If
                    Next
                End If
            End If
        End If
    End Sub

    Private Sub ugSegments_AfterRowActivate(sender As Object, e As EventArgs) Handles ugSegments.AfterRowActivate
        If Not InformationHubUtils.CanSetMarkedRowAppearance(Me.ugSegments.ActiveRow) Then
            Me.ugSegments.ActiveRow = Nothing
        ElseIf (_kblMapperForCompare IsNot Nothing) Then
            If (SetInformationHubEventArgs(Nothing, Me.ugSegments.ActiveRow, Nothing)) Then
                OnHubSelectionChanged()
            End If

            If (Not Me.Focused) Then
                Me.Focus()
            End If
        End If
    End Sub

    Private Sub ugSegments_AfterSelectChange(sender As Object, e As AfterSelectChangeEventArgs) Handles ugSegments.AfterSelectChange
        With Me.ugSegments
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

    Private Sub ugSegments_BeforeSelectChange(sender As Object, e As BeforeSelectChangeEventArgs) Handles ugSegments.BeforeSelectChange
        ClearSelectedChildRowsInGrid(Me.ugSegments)

        For Each row As UltraGridRow In e.NewSelections.Rows
            If Not InformationHubUtils.CanSetMarkedRowAppearance(row) Then
                e.Cancel = True
            End If
        Next
    End Sub

    Private Sub ugSegments_ClickCell(sender As Object, e As ClickCellEventArgs) Handles ugSegments.ClickCell
        If (_pressedMouseButton = System.Windows.Forms.MouseButtons.Left) AndAlso (_pressedKey <> Keys.ControlKey AndAlso _pressedKey <> Keys.ShiftKey) AndAlso (_informationHubEventArgs IsNot Nothing) AndAlso (_kblMapperForCompare IsNot Nothing OrElse InformationHubUtils.CanSetMarkedRowAppearance(e.Cell.Row)) Then
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

    Private Sub ugSegments_ClickCellButton(sender As Object, e As CellEventArgs) Handles ugSegments.ClickCellButton
        If (_pressedMouseButton = System.Windows.Forms.MouseButtons.Left) Then
            Dim fromReference As Boolean = True

            If HasChangeTypeWithInverse(CType(e.Cell.Row.ListObject, UltraDataRow), CompareChangeType.New) Then
                fromReference = False
            End If

            If (e.Cell.Row.ParentRow Is Nothing) AndAlso (TypeOf e.Cell.Tag Is Segment) Then
                With DirectCast(e.Cell.Tag, Segment)
                    Select Case e.Cell.Column.Key
                        Case SegmentPropertyName.Alias_id.ToString
                            ShowDetailInformationForm(InformationHubStrings.AliasIds_Caption, .Alias_id, Nothing, _kblMapper)
                        Case SegmentPropertyName.Cross_Section_Area_information.ToString
                            ShowDetailInformationForm(InformationHubStrings.CSAInfo_Caption, .Cross_Section_Area_information, Nothing, _kblMapper)
                        Case SegmentPropertyName.Fixing_Assignment.ToString
                            ShowDetailInformationForm(InformationHubStrings.FixingAssignments_Caption, .Fixing_assignment, Nothing, _kblMapper)
                        Case SegmentPropertyName.Processing_information.ToString
                            ShowDetailInformationForm(InformationHubStrings.ProcInfo_Caption, .Processing_Information, Nothing, _kblMapper)
                    End Select
                End With
            ElseIf (TypeOf e.Cell.Tag Is Protection_area) Then
                Dim protectionArea As Protection_area = DirectCast(e.Cell.Tag, Protection_area)
                Dim wireProtectionOccurrence As Wire_protection_occurrence = Nothing

                If (fromReference) AndAlso (_kblMapper.KBLOccurrenceMapper.ContainsKey(protectionArea.Associated_protection)) Then
                    wireProtectionOccurrence = DirectCast(_kblMapper.KBLOccurrenceMapper(protectionArea.Associated_protection), Wire_protection_occurrence)
                ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLOccurrenceMapper.ContainsKey(protectionArea.Associated_protection)) Then
                    wireProtectionOccurrence = DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(protectionArea.Associated_protection), Wire_protection_occurrence)
                End If

                If (fromReference) AndAlso (_kblMapper.KblPartMapper.ContainsKey(wireProtectionOccurrence.Part)) Then
                    RequestDialogPartData(_kblMapper, e.Cell.Column.Key, DirectCast(_kblMapper.KblPartMapper(wireProtectionOccurrence.Part), Wire_protection))
                ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KblPartMapper.ContainsKey(wireProtectionOccurrence.Part)) Then
                    RequestDialogPartData(_kblMapperForCompare, e.Cell.Column.Key, DirectCast(_kblMapperForCompare.KblPartMapper(wireProtectionOccurrence.Part), Wire_protection))
                End If

                With wireProtectionOccurrence
                    Select Case e.Cell.Column.Key
                        Case WireProtectionPropertyName.Alias_id.ToString
                            ShowDetailInformationForm(InformationHubStrings.AliasIds_Caption, .Alias_id, Nothing, _kblMapper)
                        Case WireProtectionPropertyName.Installation_Information.ToString
                            ShowDetailInformationForm(InformationHubStrings.InstallInfo_Caption, .Installation_Information, Nothing, _kblMapper)
                        Case ProtectionAreaPropertyName.Processing_information.ToString
                            ShowDetailInformationForm(InformationHubStrings.ProcInfo_Caption, protectionArea.Processing_Information, Nothing, _kblMapper)
                    End Select
                End With
            ElseIf (TypeOf e.Cell.Tag Is KeyValuePair(Of String, Object)) Then
                RequestDialogCompareData(True, DirectCast(e.Cell.Row.Tag, KeyValuePair(Of String, Object)).Key.SplitRemoveEmpty("|"c)(1), DirectCast(e.Cell.Tag, KeyValuePair(Of String, Object)))
            ElseIf (e.Cell.Column.Key = SegmentPropertyName.Start_node.ToString) Then
                'HINT Additive selection of start node from protection area
                Dim prevIds As IENumerable(Of String) = _informationHubEventArgs.ObjectIds
                SetInformationHubEventArgs(Nothing, e.Cell.Row, Nothing)

                If (e.Cell.Tag IsNot Nothing) Then
                    _informationHubEventArgs.ObjectIds.Add(e.Cell.Tag.ToString)
                End If

                OnHubSelectionChanged()

                If (Not Me.Focused) Then
                    Me.Focus()
                End If
            End If
        End If
    End Sub

    Private Sub ugSegments_DoubleClickRow(sender As Object, e As DoubleClickRowEventArgs) Handles ugSegments.DoubleClickRow
        OnDoubleClickRow(sender, e)
    End Sub

    Private Sub ugSegments_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugSegments.InitializeLayout
        Me.ugSegments.BeginUpdate()
        Me.ugSegments.EventManager.AllEventsEnabled = False

        InitializeGridLayout(_segmentGridAppearance, e.Layout)

        Me.ugSegments.EventManager.AllEventsEnabled = True
        Me.ugSegments.EndUpdate()
    End Sub

    Private Sub InitializeSegmentRowCore(row As UltraGridRow)
        If (row.ParentRow Is Nothing) Then
            If (DirectCast(row.ListObject, UltraDataRow).Band.Tag Is Nothing) Then
                row.Tag = DirectCast(DirectCast(row.ListObject, UltraDataRow).Tag, Segment).SystemId
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
                        If (TypeOf compareObject.Value Is SegmentChangedProperty) Then
                            For Each changedProperty As KeyValuePair(Of String, Object) In DirectCast(compareObject.Value, SegmentChangedProperty).ChangedProperties
                                If (changedProperty.Key = cell.Column.Key) Then
                                    cell.Tag = changedProperty
                                    Exit For
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

            Me.ugSegments.EventManager.AllEventsEnabled = False

            If (row.ChildBands?.Count).GetValueOrDefault > 0 Then
                For Each childRow As UltraGridRow In row.ChildBands(0).Rows
                    If (_kblMapper?.KBLOccurrenceMapper.ContainsKey(childRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString)).GetValueOrDefault Then
                        childRow.Tag = DirectCast(_kblMapper.KBLOccurrenceMapper(childRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString), Protection_area).Associated_protection
                    ElseIf (_kblMapperForCompare?.KBLOccurrenceMapper.ContainsKey(childRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString)).GetValueOrDefault Then
                        childRow.Tag = DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(childRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString), Protection_area).Associated_protection
                    End If
                    InitializeSegmentRowCore(childRow)
                Next
            End If
            Me.ugSegments.EventManager.AllEventsEnabled = True
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
                        If (TypeOf compareObject.Value Is ProtectionAreaChangedProperty) Then
                            For Each changedProperty As KeyValuePair(Of String, Object) In DirectCast(compareObject.Value, ProtectionAreaChangedProperty).ChangedProperties
                                If (changedProperty.Key = cell.Column.Key) Then
                                    cell.Tag = changedProperty
                                    Exit For
                                ElseIf (changedProperty.Key = NameOf(Protection_area.Associated_protection)) Then
                                    For Each wireProtectionOccurrenceChangedProperty As KeyValuePair(Of String, Object) In DirectCast(changedProperty.Value, WireProtectionOccurrenceChangedProperty).ChangedProperties
                                        If (wireProtectionOccurrenceChangedProperty.Key = cell.Column.Key) Then
                                            cell.Tag = wireProtectionOccurrenceChangedProperty
                                            Exit For
                                        ElseIf (wireProtectionOccurrenceChangedProperty.Key = NameOf(IKblOccurrence.Part)) Then
                                            For Each partChangedProperty As KeyValuePair(Of String, Object) In DirectCast(wireProtectionOccurrenceChangedProperty.Value, PartChangedProperty).ChangedProperties
                                                If (partChangedProperty.Key = cell.Column.Key) Then
                                                    cell.Tag = partChangedProperty
                                                    Exit For
                                                End If
                                            Next
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
                        cell.Tag = _kblMapper.KBLOccurrenceMapper(row.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString)
                    End If
                ElseIf cell.Column.Key = SegmentPropertyName.Start_node.ToString Then
                    'HINT setup start node for protection highlighting
                    If (_kblMapperForCompare Is Nothing) Then
                        Dim pa As Protection_area = DirectCast(DirectCast(row.ListObject, UltraDataRow).Tag, Protection_area)
                        Dim seg As Segment = DirectCast(DirectCast(row.ParentRow.ListObject, UltraDataRow).Tag, Segment)
                        Dim startNode As Node = Nothing
                        If (seg IsNot Nothing) Then
                            If (_kblMapper.KBLOccurrenceMapper.ContainsKey(seg.Start_node) AndAlso TypeOf _kblMapper.KBLOccurrenceMapper(seg.Start_node) Is Node) Then
                                startNode = DirectCast(_kblMapper.KBLOccurrenceMapper(seg.Start_node), Node)
                            End If
                        End If

                        If startNode IsNot Nothing Then
                            cell.Tag = startNode.SystemId
                            cell.Appearance.Image = My.Resources.Show
                            cell.Style = ColumnStyle.Button
                        End If
                    End If
                End If
            Next
        End If
    End Sub

    Private Sub ugSegments_InitializeRow(sender As Object, e As InitializeRowEventArgs) Handles ugSegments.InitializeRow
        If Not e.ReInitialize Then
            InitializeSegmentRowCore(e.Row)
        End If
    End Sub

    Private Sub ugSegments_KeyDown(sender As Object, e As KeyEventArgs) Handles ugSegments.KeyDown
        OnGridKeyDown(Me, New GridKeyDownEventArgs(e, DirectCast(sender, UltraGrid)))
        If Not e.Handled Then
            If (e.Control) AndAlso (e.KeyCode = Keys.A) Then
                SelectAllRowsOfActiveGrid()
            ElseIf (e.KeyCode = Keys.Escape) Then
                If (Me.ugSegments.Selected.Rows.Count = 0) Then
                    ClearMarkedRowsInGrids()
                Else
                    ClearSelectedRowsInGrids()
                End If

                If (SetInformationHubEventArgs(Nothing, Nothing, Me.ugSegments.Selected.Rows)) Then
                    OnHubSelectionChanged()
                End If

            ElseIf (e.KeyCode = Keys.Return) Then
                SetSelectedAsMarkedOriginRows(ugSegments)
            End If
        End If
    End Sub

    Private Sub ugSegments_MouseDown(sender As Object, e As MouseEventArgs) Handles ugSegments.MouseDown
        If (_contextMenuGrid.IsOpen) Then
            _contextMenuGrid.ClosePopup()
        End If
        _pressedMouseButton = e.Button
    End Sub

    Private Sub ugSegments_MouseLeave(sender As Object, e As EventArgs) Handles ugSegments.MouseLeave
        _messageEventArgs.StatusMessage = String.Empty
        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub ugSegments_MouseMove(sender As Object, e As MouseEventArgs) Handles ugSegments.MouseMove
        _messageEventArgs.StatusMessage = String.Format(InformationHubStrings.RowCount_Label, Me.ugSegments.Rows.Count, Me.ugSegments.Rows.FilteredInNonGroupByRowCount, Me.ugSegments.Rows.VisibleRowCount, Me.ugSegments.Selected.Rows.Count)
        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub udsSegments_CellDataUpdated(sender As Object, e As CellDataUpdatedEventArgs) Handles udsSegments.CellDataUpdated
        RaiseEvent CellValueUpdated(sender, e)
    End Sub

End Class