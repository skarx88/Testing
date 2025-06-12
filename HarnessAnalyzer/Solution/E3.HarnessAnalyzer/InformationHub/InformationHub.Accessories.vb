Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinGrid
Imports Zuken.E3.Lib.Schema.Kbl.Properties
Imports Zuken.E3.HarnessAnalyzer.Shared

Partial Public Class InformationHub

    Private Sub InitializeAccessories()
        _accessoryGridAppearance = GridAppearance.All.OfType(Of AccessoryGridAppearance).Single
        AddNewRowFilters(ugAccessories)

        If (_comparisonMapperList Is Nothing) Then
            FillDataSource(Me.udsAccessories, _accessoryGridAppearance, _kblMapper.GetAccessoryOccurrences.Count)
        Else
            FillDataSource(Me.udsAccessories, _accessoryGridAppearance, 0, _comparisonMapperList(KblObjectType.Accessory_occurrence))
        End If

        If (Me.udsAccessories.Band.Columns.Count = 0) OrElse (Me.udsAccessories.Rows.Count = 0) Then
            Me.utcInformationHub.Tabs(TabNames.tabAccessories.ToString).Visible = False
        End If
    End Sub

    Private Sub udsAccessories_CellDataRequested(sender As Object, e As CellDataRequestedEventArgs) Handles udsAccessories.CellDataRequested
        e.CacheData = False

        With DirectCast(e.Row.Tag, Accessory_occurrence)
            Dim accessory As Accessory = Nothing
            Dim fromReference As Boolean = True
            Dim fixingAssignment As Fixing_assignment = Nothing
            Dim startNode As Node = Nothing

            If HasChangeTypeWithInverse(e.Row, CompareChangeType.New) Then
                fromReference = False

                If (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLPartMapper.ContainsKey(If(.Part Is Nothing, String.Empty, .Part))) Then
                    accessory = TryCast(_kblMapperForCompare.KBLPartMapper(.Part), Accessory)
                    For Each segment As Segment In _kblMapperForCompare.GetSegments.Where(Function(seg) seg.Fixing_assignment IsNot Nothing AndAlso seg.Fixing_assignment.Any(Function(fixAssignment) fixAssignment.Fixing = .SystemId))
                        fixingAssignment = segment.Fixing_assignment.Where(Function(fixAssignment) fixAssignment.Fixing = .SystemId).FirstOrDefault
                        startNode = _kblMapper.GetOccurrenceObject(Of Node)(segment.Start_node)
                        If startNode IsNot Nothing Then
                            Exit For
                        End If
                    Next
                End If
            Else
                If (_kblMapper.KBLOccurrenceMapper.ContainsKey(.SystemId)) AndAlso (_kblMapper.KBLPartMapper.ContainsKey(If(.Part Is Nothing, String.Empty, .Part))) Then
                    accessory = TryCast(_kblMapper.KBLPartMapper(.Part), Accessory)
                    For Each segment As Segment In _kblMapper.GetSegments.Where(Function(seg) seg.Fixing_assignment IsNot Nothing AndAlso seg.Fixing_assignment.Any(Function(fixAssignment) fixAssignment.Fixing = .SystemId))
                        fixingAssignment = segment.Fixing_assignment.Where(Function(fixAssignment) fixAssignment.Fixing = .SystemId).FirstOrDefault
                        startNode = _kblMapper.GetOccurrenceObject(Of Node)(segment.Start_node)
                        If startNode IsNot Nothing Then
                            Exit For
                        End If
                    Next
                End If
            End If

            If (accessory IsNot Nothing) Then
                RequestCellPartData(e.Data, fromReference, e.Column.Key, accessory)
            End If

            Select Case e.Column.Key
                Case AccessoryPropertyName.Id
                    e.Data = .Id
                Case AccessoryPropertyName.Alias_id
                    If (.Alias_id.Length <> 0) Then
                        If (.Alias_id.Length = 1) AndAlso (.Alias_id(0).Description Is Nothing) AndAlso (.Alias_id(0).Scope Is Nothing) Then
                            e.Data = .Alias_id(0).Alias_id
                        Else
                            e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                        End If
                    End If
                Case AccessoryPropertyName.Description
                    If (.Description IsNot Nothing) Then
                        e.Data = .Description
                    End If
                Case AccessoryPropertyName.Reference_element

                    If (.Reference_element IsNot Nothing) Then
                        If (.Reference_element.SplitSpace.Length = 1) Then
                            e.Data = Harness.GetReferenceElement(.Reference_element, If(fromReference, _kblMapper, _kblMapperForCompare))
                        ElseIf (.Reference_element.SplitSpace.Length > 1) Then
                            e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                        End If
                    End If

                Case AccessoryPropertyName.Accessory_type
                    If (accessory IsNot Nothing) AndAlso (accessory.Accessory_type IsNot Nothing) Then
                        e.Data = accessory.Accessory_type
                    End If
                Case AccessoryPropertyName.Installation_Information
                    If (.Installation_information.Length <> 0) Then
                        e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                    End If
                Case AccessoryPropertyName.Localized_Description
                    If (.Localized_description.Length > 0) Then
                        If (.Localized_description.Length = 1) Then
                            e.Data = .Localized_description(0).Value
                        Else
                            e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                        End If
                    End If

                Case SegmentPropertyName.Start_node
                    If startNode IsNot Nothing Then
                        e.Data = startNode.Id
                    End If

                Case FixingPropertyName.SegmentLocation
                    If (fixingAssignment IsNot Nothing) Then
                        e.Data = String.Format("{0} %", Math.Round(fixingAssignment.Location * 100, NOF_DIGITS_LOCATIONS))
                    End If
                Case FixingPropertyName.SegmentAbsolute_location
                    If (fixingAssignment?.Absolute_location IsNot Nothing) Then
                        e.Data = String.Format("{0} {1}", Math.Round(fixingAssignment.Absolute_location.Value_component, 2), _kblMapper.KBLUnitMapper(fixingAssignment.Absolute_location.Unit_component).Unit_name)
                    End If

                Case NameOf(InformationHubStrings.AssignedModules_ColumnCaption)
                    e.Data = GetAssignedModules(.SystemId)

                Case AccessoryPropertyName.Placement
                    If e.Row.Band.Tag IsNot Nothing Then
                        If TryCast(e.Row.Band.Tag, Dictionary(Of String, Object)) IsNot Nothing Then
                            Dim fcp As AccessoryChangedProperty = TryCast(DirectCast(e.Row.Band.Tag, Dictionary(Of String, Object)).Values.First, AccessoryChangedProperty)
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

    Private Sub udsAccessories_InitializeDataRow(sender As Object, e As InitializeDataRowEventArgs) Handles udsAccessories.InitializeDataRow
        Dim accessoryOccurrence As Accessory_occurrence

        If (e.Row.Band.Tag Is Nothing) Then
            accessoryOccurrence = _kblMapper.GetAccessoryOccurrences(e.Row.Index)
        Else
            Dim compareObjKey As String = DirectCast(e.Row.Band.Tag, Dictionary(Of String, Object)).Keys.ElementAt(e.Row.Index)
            Dim compareObjValue As Object = DirectCast(e.Row.Band.Tag, Dictionary(Of String, Object)).Values.ElementAt(e.Row.Index)

            If (TypeOf compareObjValue Is Accessory_occurrence) Then
                accessoryOccurrence = DirectCast(compareObjValue, Accessory_occurrence)
            Else
                accessoryOccurrence = _kblMapper.GetOccurrenceObject(Of Accessory_occurrence)(ExtractSystemId(compareObjKey))
            End If

            SetDiffCellValueFromObjectKey(e.Row, compareObjKey)
        End If

        e.Row.Tag = accessoryOccurrence
    End Sub


    Private Sub ugAccessories_AfterRowActivate(sender As Object, e As EventArgs) Handles ugAccessories.AfterRowActivate
        If (Me.ugAccessories.ActiveRow.Appearance.BackColor = Color.FromArgb(190, 190, 190)) Then
            Me.ugAccessories.ActiveRow = Nothing
        ElseIf (_kblMapperForCompare IsNot Nothing) Then
            If (SetInformationHubEventArgs(Nothing, Me.ugAccessories.ActiveRow, Nothing)) Then
                OnHubSelectionChanged()
            End If
            If Not Me.Focused Then
                Me.Focus()
            End If
        End If
    End Sub

    Private Sub ugAccessories_AfterSelectChange(sender As Object, e As AfterSelectChangeEventArgs) Handles ugAccessories.AfterSelectChange
        With Me.ugAccessories
            .BeginUpdate()
            If (.ActiveRow IsNot Nothing) AndAlso (Not .ActiveRowScrollRegion.VisibleRows.Contains(.ActiveRow)) Then
                .ActiveRowScrollRegion.ScrollRowIntoView(.ActiveRow)
            End If
            .EndUpdate()

            If SetInformationHubEventArgs(Nothing, Nothing, .Selected.Rows) Then
                OnHubSelectionChanged()
            End If

        End With
    End Sub

    Private Sub ugAccessories_BeforeSelectChange(sender As Object, e As BeforeSelectChangeEventArgs) Handles ugAccessories.BeforeSelectChange
        For Each row As UltraGridRow In e.NewSelections.Rows
            If Not InformationHubUtils.CanSetMarkedRowAppearance(row) Then
                e.Cancel = True
            End If
        Next
    End Sub

    Private Sub ugAccessories_ClickCell(sender As Object, e As ClickCellEventArgs) Handles ugAccessories.ClickCell
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

    Private Sub ugAccessories_ClickCellButton(sender As Object, e As CellEventArgs) Handles ugAccessories.ClickCellButton
        If (_pressedMouseButton = System.Windows.Forms.MouseButtons.Left) Then
            Dim fromReference As Boolean = True

            If HasChangeTypeWithInverse(CType(e.Cell.Row.ListObject, UltraDataRow), CompareChangeType.New) Then
                fromReference = False
            End If

            If (TypeOf e.Cell.Tag Is Accessory_occurrence) Then
                Dim accessoryOccurrence As Accessory_occurrence = DirectCast(e.Cell.Tag, Accessory_occurrence)
                Dim accessoryPart As Accessory = Nothing

                If (fromReference) AndAlso (_kblMapper.KblPartMapper.ContainsKey(accessoryOccurrence.Part)) Then
                    accessoryPart = DirectCast(_kblMapper.KblPartMapper(accessoryOccurrence.Part), Accessory)

                    RequestDialogPartData(_kblMapper, e.Cell.Column.Key, accessoryPart)
                ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KblPartMapper.ContainsKey(accessoryOccurrence.Part)) Then
                    accessoryPart = DirectCast(_kblMapperForCompare.KblPartMapper(accessoryOccurrence.Part), Accessory)

                    RequestDialogPartData(_kblMapperForCompare, e.Cell.Column.Key, accessoryPart)
                End If

                With accessoryOccurrence
                    Select Case e.Cell.Column.Key
                        Case AccessoryPropertyName.Alias_id.ToString
                            ShowDetailInformationForm(InformationHubStrings.AliasIds_Caption, .Alias_id, Nothing, _kblMapper)
                        Case AccessoryPropertyName.Installation_Information.ToString
                            ShowDetailInformationForm(InformationHubStrings.InstallInfo_Caption, .Installation_Information, Nothing, _kblMapper)
                        Case AccessoryPropertyName.Reference_element.ToString
                            Dim referencedElements As New List(Of String)
                            For Each referencedElement As String In .Reference_element.SplitSpace
                                Dim ref_occ_Current As IKblOccurrence = If(fromReference, _kblMapper.GetOccurrenceObjectUntyped(referencedElement), Nothing)
                                Dim ref_occ_Compare As IKblOccurrence = If(Not fromReference, _kblMapperForCompare.GetOccurrenceObjectUntyped(referencedElement), Nothing)

                                If ref_occ_Current IsNot Nothing Then
                                    referencedElements.Add(Harness.GetReferenceElement(referencedElement, _kblMapper))
                                ElseIf ref_occ_Compare IsNot Nothing Then
                                    referencedElements.Add(Harness.GetReferenceElement(referencedElement, _kblMapperForCompare))
                                End If
                            Next

                            ShowDetailInformationForm(InformationHubStrings.RefElement_Caption, referencedElements, Nothing, _kblMapper)
                    End Select
                End With
            ElseIf (TypeOf e.Cell.Tag Is KeyValuePair(Of String, Object)) Then
                RequestDialogCompareData(True, DirectCast(e.Cell.Row.Tag, KeyValuePair(Of String, Object)).Key.SplitRemoveEmpty("|"c)(1), DirectCast(e.Cell.Tag, KeyValuePair(Of String, Object)))
            ElseIf (e.Cell.Column.Key = SegmentPropertyName.Start_node) Then
                'HINT Additive selection of start node from accessories
                Dim prevIds As IEnumerable(Of String) = _informationHubEventArgs.ObjectIds
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

    Private Sub ugAccessories_DoubleClickRow(sender As Object, e As DoubleClickRowEventArgs) Handles ugAccessories.DoubleClickRow
        OnDoubleClickRow(sender, e)
    End Sub

    Private Sub ugAccessories_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugAccessories.InitializeLayout
        Me.ugAccessories.BeginUpdate()
        Me.ugAccessories.EventManager.AllEventsEnabled = False

        InitializeGridLayout(_accessoryGridAppearance, e.Layout)

        Me.ugAccessories.EventManager.AllEventsEnabled = True
        Me.ugAccessories.EndUpdate()
    End Sub

    Private Sub InitializeAccessoryRowCore(row As UltraGridRow)
        If (DirectCast(row.ListObject, UltraDataRow).Band.Tag Is Nothing) Then
            row.Tag = DirectCast(DirectCast(row.ListObject, UltraDataRow).Tag, Accessory_occurrence).SystemId

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
                    If (TypeOf compareObject.Value Is AccessoryChangedProperty) Then
                        For Each changedProperty As KeyValuePair(Of String, Object) In DirectCast(compareObject.Value, AccessoryChangedProperty).ChangedProperties
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
                    cell.Tag = _kblMapper.GetOccurrenceObjectUntyped(row.Tag?.ToString)
                End If
            ElseIf cell.Column.Key = SegmentPropertyName.Start_node Then
                'HINT setup start node for fixing assignment highlighting
                If (_kblMapperForCompare Is Nothing) Then
                    Dim acc As Accessory_occurrence = DirectCast(DirectCast(row.ListObject, UltraDataRow).Tag, Accessory_occurrence)
                    Dim startNode As Node = Nothing
                    If (_kblMapper.KBLOccurrenceMapper.ContainsKey(acc.SystemId)) AndAlso (_kblMapper.KBLPartMapper.ContainsKey(If(acc.Part Is Nothing, String.Empty, acc.Part))) Then
                        For Each segment As Segment In _kblMapper.GetSegments.Where(Function(seg) seg.Fixing_assignment IsNot Nothing AndAlso seg.Fixing_assignment.Any(Function(fixAssignment) fixAssignment.Fixing = acc.SystemId))
                            Dim fixingAssignment As Fixing_assignment = segment.Fixing_assignment.Where(Function(fixAssignment) fixAssignment.Fixing = acc.SystemId).FirstOrDefault
                            startNode = _kblMapper.GetOccurrenceObject(Of Node)(segment.Start_node)
                            If startNode IsNot Nothing Then
                                Exit For
                            End If
                        Next
                    End If
                    If startNode IsNot Nothing Then
                        cell.Tag = startNode.SystemId
                        cell.Appearance.Image = My.Resources.Show
                        cell.Style = ColumnStyle.Button
                    End If
                End If
            End If
        Next
    End Sub

    Private Sub ugAccessories_InitializeRow(sender As Object, e As InitializeRowEventArgs) Handles ugAccessories.InitializeRow
        If Not e.ReInitialize Then
            InitializeAccessoryRowCore(e.Row)
        End If
    End Sub

    Private Sub ugAccessories_KeyDown(sender As Object, e As KeyEventArgs) Handles ugAccessories.KeyDown
        OnGridKeyDown(Me, New GridKeyDownEventArgs(e, DirectCast(sender, UltraGrid)))
        If Not e.Handled Then
            If (e.Control) AndAlso (e.KeyCode = Keys.A) Then
                SelectAllRowsOfActiveGrid()
            ElseIf (e.KeyCode = Keys.Escape) Then
                If (Me.ugAccessories.Selected.Rows.Count = 0) Then
                    ClearMarkedRowsInGrids()
                Else
                    ClearSelectedRowsInGrids()
                End If

                If (SetInformationHubEventArgs(Nothing, Nothing, Me.ugAccessories.Selected.Rows)) Then
                    OnHubSelectionChanged()
                End If
            ElseIf (e.KeyCode = Keys.Return) Then
                SetSelectedAsMarkedOriginRows(ugAccessories)
            End If
        End If
    End Sub

    Private Sub ugAccessories_MouseDown(sender As Object, e As MouseEventArgs) Handles ugAccessories.MouseDown
        If (_contextMenuGrid.IsOpen) Then
            _contextMenuGrid.ClosePopup()
        End If

        _pressedMouseButton = e.Button
    End Sub

    Private Sub ugAccessories_MouseLeave(sender As Object, e As EventArgs) Handles ugAccessories.MouseLeave
        _messageEventArgs.StatusMessage = String.Empty

        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub ugAccessories_MouseMove(sender As Object, e As MouseEventArgs) Handles ugAccessories.MouseMove
        _messageEventArgs.StatusMessage = String.Format(InformationHubStrings.RowCount_Label, Me.ugAccessories.Rows.Count, Me.ugAccessories.Rows.FilteredInNonGroupByRowCount, Me.ugAccessories.Rows.VisibleRowCount, Me.ugAccessories.Selected.Rows.Count)

        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub udsAccessories_CellDataUpdated(sender As Object, e As CellDataUpdatedEventArgs) Handles udsAccessories.CellDataUpdated
        RaiseEvent CellValueUpdated(sender, e)
    End Sub

End Class