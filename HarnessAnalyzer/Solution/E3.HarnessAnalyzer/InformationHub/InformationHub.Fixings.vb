Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinGrid
Imports Zuken.E3.Lib.Schema.Kbl.Properties
Imports Zuken.E3.HarnessAnalyzer.Shared

Partial Public Class InformationHub

    Private Sub InitializeFixings()
        _fixingGridAppearance = GridAppearance.All.OfType(Of FixingGridAppearance).Single
        AddNewRowFilters(ugFixings)

        If (_comparisonMapperList Is Nothing) Then
            FillDataSource(Me.udsFixings, _fixingGridAppearance, _kblMapper.GetFixingOccurrences.Count)
        Else
            FillDataSource(Me.udsFixings, _fixingGridAppearance, 0, _comparisonMapperList(KblObjectType.Fixing_occurrence))
        End If

        If (Me.udsFixings.Band.Columns.Count = 0) OrElse (Me.udsFixings.Rows.Count = 0) Then
            Me.utcInformationHub.Tabs(TabNames.tabFixings.ToString()).Visible = False
        End If
    End Sub

    Private Sub udsFixings_CellDataRequested(sender As Object, e As CellDataRequestedEventArgs) Handles udsFixings.CellDataRequested
        Static fixing_occ_r As New SegmentStartNodeFixingResult(DirectCast(e.Row.Tag, Fixing_occurrence))
        If fixing_occ_r?.FixingOccurrence IsNot DirectCast(e.Row.Tag, Fixing_occurrence) Then
            fixing_occ_r?.Dispose()
            fixing_occ_r = New SegmentStartNodeFixingResult(DirectCast(e.Row.Tag, Fixing_occurrence))
        End If

        e.CacheData = False

        Dim fromReference As Boolean = True
        If HasChangeTypeWithInverse(e.Row, CompareChangeType.New) Then
            fromReference = False
            If Not fixing_occ_r.WasInitializedFrom(_kblMapperForCompare) AndAlso fixing_occ_r.PartExists(_kblMapperForCompare) Then
                fixing_occ_r.Initialize(_kblMapperForCompare)
            End If
        Else
            If Not fixing_occ_r.WasInitializedFrom(_kblMapper) AndAlso fixing_occ_r.PartExists(_kblMapper) Then
                fixing_occ_r.Initialize(_kblMapper)
            End If
        End If

        If (fixing_occ_r.FixingPart IsNot Nothing) Then
            RequestCellPartData(e.Data, fromReference, e.Column.Key, fixing_occ_r.FixingPart)
        End If

        Select Case e.Column.Key
            Case FixingPropertyName.Id
                e.Data = fixing_occ_r.Id
            Case FixingPropertyName.Alias_Id
                If (fixing_occ_r.HasAliasIds) Then
                    If (fixing_occ_r.AliasId.Length = 1) AndAlso (fixing_occ_r.FirstAliasId.Description Is Nothing) AndAlso (fixing_occ_r.FirstAliasId.Scope Is Nothing) Then
                        e.Data = fixing_occ_r.FirstAliasId.Alias_id
                    Else
                        e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                    End If
                End If
            Case FixingPropertyName.Description
                If (fixing_occ_r.HasDescription) Then
                    e.Data = fixing_occ_r.Description
                End If
            Case FixingPropertyName.Fixing_type
                If (fixing_occ_r?.FixingPart IsNot Nothing) AndAlso (fixing_occ_r.FixingPart.Fixing_type IsNot Nothing) Then
                    e.Data = fixing_occ_r.FixingPart.Fixing_type
                End If
            Case FixingPropertyName.Installation_Information
                If (fixing_occ_r.HasInstallationInformations) Then
                    e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                End If
            Case FixingPropertyName.AssignmentProcessingInfos
                If (fixing_occ_r?.Segment?.FixingAssignment?.Processing_information.Length).GetValueOrDefault > 0 Then
                    e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                End If
            Case FixingPropertyName.Localized_description
                If (fixing_occ_r.HasLocalizeddescriptions) Then
                    If (fixing_occ_r.FixingOccurrence.Localized_description.Length = 1) Then
                        e.Data = fixing_occ_r.FixingOccurrence.Localized_description(0).Value
                    Else
                        e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                    End If
                End If
            Case SegmentPropertyName.Start_node
                If fixing_occ_r?.Segment?.StartNode IsNot Nothing Then
                    e.Data = fixing_occ_r.Segment.StartNode.Id
                End If

            Case FixingPropertyName.SegmentLocation
                If (fixing_occ_r?.HasSegmentFixingAssignment).GetValueOrDefault Then
                    e.Data = String.Format("{0} %", Math.Round(fixing_occ_r.Segment.FixingAssignment.Location * 100, NOF_DIGITS_LOCATIONS))
                End If
            Case FixingPropertyName.SegmentAbsolute_location
                If (fixing_occ_r?.Segment?.FixingAssignment?.Absolute_location IsNot Nothing) Then
                    e.Data = String.Format("{0} {1}", Math.Round(fixing_occ_r.Segment.FixingAssignment.Absolute_location.Value_component, 2), _kblMapper.KBLUnitMapper(fixing_occ_r.Segment.FixingAssignment.Absolute_location.Unit_component).Unit_name)
                End If

            Case FixingPropertyName.Placement
                If e.Row.Band.Tag IsNot Nothing Then
                    If TryCast(e.Row.Band.Tag, Dictionary(Of String, Object)) IsNot Nothing Then
                        Dim fcp As FixingChangedProperty = TryCast(DirectCast(e.Row.Band.Tag, Dictionary(Of String, Object)).Values.First, FixingChangedProperty)
                        If fcp IsNot Nothing Then
                            If fcp.ChangedProperties.ContainsKey(PLACEMENT) Then
                                e.Data = InformationHubStrings.PlacementModified
                            End If
                        End If
                    End If
                End If

            Case NameOf(InformationHubStrings.AssignedModules_ColumnCaption)
                e.Data = GetAssignedModules(fixing_occ_r.SystemId)
            Case Else
                OnUnhandledCellDataRequested(e)
        End Select
    End Sub

    Private Sub udsFixings_InitializeDataRow(sender As Object, e As InitializeDataRowEventArgs) Handles udsFixings.InitializeDataRow
        Dim fixingOccurrence As Fixing_occurrence = Nothing

        If (e.Row.Band.Tag Is Nothing) Then
            fixingOccurrence = _kblMapper.GetFixingOccurrences(e.Row.Index)
        Else
            Dim compareObjKey As String = DirectCast(e.Row.Band.Tag, Dictionary(Of String, Object)).Keys.ElementAt(e.Row.Index)
            Dim compareObjValue As Object = DirectCast(e.Row.Band.Tag, Dictionary(Of String, Object)).Values.ElementAt(e.Row.Index)

            If (TypeOf compareObjValue Is Fixing_occurrence) Then
                fixingOccurrence = DirectCast(compareObjValue, Fixing_occurrence)
            Else
                fixingOccurrence = DirectCast(_kblMapper.KBLOccurrenceMapper(ExtractSystemId(compareObjKey)), Fixing_occurrence)
            End If

            SetDiffCellValueFromObjectKey(e.Row, compareObjKey)
        End If

        e.Row.Tag = fixingOccurrence
    End Sub


    Private Sub ugFixings_AfterRowActivate(sender As Object, e As EventArgs) Handles ugFixings.AfterRowActivate
        If (Me.ugFixings.ActiveRow.Appearance.BackColor = Color.FromArgb(190, 190, 190)) Then
            Me.ugFixings.ActiveRow = Nothing

        ElseIf (_kblMapperForCompare IsNot Nothing) Then
            If (SetInformationHubEventArgs(Nothing, Me.ugFixings.ActiveRow, Nothing)) Then
                OnHubSelectionChanged()
            End If

            If (Not Me.Focused) Then
                Me.Focus()
            End If
        End If
    End Sub

    Private Sub ugFixings_AfterSelectChange(sender As Object, e As AfterSelectChangeEventArgs) Handles ugFixings.AfterSelectChange
        With Me.ugFixings
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

    Private Sub ugFixings_BeforeSelectChange(sender As Object, e As BeforeSelectChangeEventArgs) Handles ugFixings.BeforeSelectChange
        For Each row As UltraGridRow In e.NewSelections.Rows
            If (row.Appearance.BackColor = Color.FromArgb(190, 190, 190)) Then
                e.Cancel = True
            End If
        Next
    End Sub

    Private Sub ugFixings_ClickCell(sender As Object, e As ClickCellEventArgs) Handles ugFixings.ClickCell
        If (_pressedMouseButton = System.Windows.Forms.MouseButtons.Left) AndAlso (_pressedKey <> Keys.ControlKey AndAlso _pressedKey <> Keys.ShiftKey) AndAlso (_informationHubEventArgs IsNot Nothing) AndAlso (_kblMapperForCompare IsNot Nothing OrElse e.Cell.Row.Appearance.BackColor <> Color.FromArgb(190, 190, 190)) Then
            Dim prevIds As IENumerable(Of String) = _informationHubEventArgs.ObjectIds

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

    Private Sub ugFixings_ClickCellButton(sender As Object, e As CellEventArgs) Handles ugFixings.ClickCellButton
        If (_pressedMouseButton = System.Windows.Forms.MouseButtons.Left) Then
            Dim fromReference As Boolean = True

            If HasChangeTypeWithInverse(CType(e.Cell.Row.ListObject, UltraDataRow), CompareChangeType.New) Then
                fromReference = False
            End If

            If (TypeOf e.Cell.Tag Is Fixing_occurrence) Then
                Dim fixingOccurrence As Fixing_occurrence = DirectCast(e.Cell.Tag, Fixing_occurrence)
                Dim fixingPart As Fixing = Nothing

                If (fromReference) AndAlso (_kblMapper.KblPartMapper.ContainsKey(fixingOccurrence.Part)) Then
                    fixingPart = DirectCast(_kblMapper.KblPartMapper(fixingOccurrence.Part), Fixing)

                    RequestDialogPartData(_kblMapper, e.Cell.Column.Key, fixingPart)
                ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KblPartMapper.ContainsKey(fixingOccurrence.Part)) Then
                    fixingPart = DirectCast(_kblMapperForCompare.KblPartMapper(fixingOccurrence.Part), Fixing)

                    RequestDialogPartData(_kblMapperForCompare, e.Cell.Column.Key, fixingPart)
                End If

                With fixingOccurrence
                    Select Case e.Cell.Column.Key
                        Case FixingPropertyName.Alias_Id.ToString
                            ShowDetailInformationForm(InformationHubStrings.AliasIds_Caption, .Alias_Id, Nothing, _kblMapper)
                        Case FixingPropertyName.Installation_Information.ToString
                            ShowDetailInformationForm(InformationHubStrings.InstallInfo_Caption, .Installation_Information, Nothing, _kblMapper)
                        Case FixingPropertyName.AssignmentProcessingInfos.ToString
                            Dim fixingAssignment As Fixing_assignment = GetFixingAssignment(.SystemId)
                            If fixingAssignment IsNot Nothing Then
                                ShowDetailInformationForm(InformationHubStrings.ProcInfo_Caption, fixingAssignment.Processing_Information, Nothing, _kblMapper)
                            End If

                    End Select
                End With

            ElseIf (TypeOf e.Cell.Tag Is KeyValuePair(Of String, Object)) Then
                RequestDialogCompareData(True, DirectCast(e.Cell.Row.Tag, KeyValuePair(Of String, Object)).Key.SplitRemoveEmpty("|"c)(1), DirectCast(e.Cell.Tag, KeyValuePair(Of String, Object)))

            ElseIf (e.Cell.Column.Key = SegmentPropertyName.Start_node) Then
                'HINT Additive selection of start node from fixing assignments
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

    Private Sub ugFixings_DoubleClickRow(sender As Object, e As DoubleClickRowEventArgs) Handles ugFixings.DoubleClickRow
        OnDoubleClickRow(sender, e)
    End Sub

    Private Sub OnDoubleClickRow(sender As Object, e As DoubleClickRowEventArgs)
        If _kblMapperForCompare Is Nothing AndAlso InformationHubUtils.CanSetMarkedRowAppearance(e.Row) Then
            If Not My.Computer.Keyboard.CtrlKeyDown Then
                ToggleMarkedOriginRow(e.Row)
            Else
                AddMarkedOriginRow(e.Row, True, _currentRowFilterInfo.MarkedRows.FirstOrDefault?.Band.Layout.Grid IsNot e.Row.Band.Layout.Grid, True)
            End If
        End If
    End Sub

    Private Sub ugFixings_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugFixings.InitializeLayout
        Me.ugFixings.BeginUpdate()
        Me.ugFixings.EventManager.AllEventsEnabled = False

        InitializeGridLayout(_fixingGridAppearance, e.Layout)

        Me.ugFixings.EventManager.AllEventsEnabled = True
        Me.ugFixings.EndUpdate()
    End Sub

    Private Sub InitializeFixingRowCore(row As UltraGridRow)
        If (DirectCast(row.ListObject, UltraDataRow).Band.Tag Is Nothing) Then
            row.Tag = DirectCast(DirectCast(row.ListObject, UltraDataRow).Tag, Fixing_occurrence).SystemId

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

                    If (TypeOf compareObject.Value Is FixingChangedProperty) Then
                        For Each changedProperty As KeyValuePair(Of String, Object) In DirectCast(compareObject.Value, FixingChangedProperty).ChangedProperties
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

            ElseIf cell.Column.Key = SegmentPropertyName.Start_node Then
                'HINT setup startnode for fixing assignment highlighting
                If (_kblMapperForCompare Is Nothing) Then
                    Dim fx As Fixing_occurrence = DirectCast(DirectCast(row.ListObject, UltraDataRow).Tag, Fixing_occurrence)
                    Dim startNode As Node = Nothing
                    If (_kblMapper.KBLOccurrenceMapper.ContainsKey(fx.SystemId)) AndAlso (_kblMapper.KBLPartMapper.ContainsKey(If(fx.Part Is Nothing, String.Empty, fx.Part))) Then
                        For Each segment As Segment In _kblMapper.GetSegments.Where(Function(seg) seg.Fixing_assignment IsNot Nothing AndAlso seg.Fixing_assignment.Any(Function(fixAssignment) fixAssignment.Fixing = fx.SystemId))
                            Dim fixingAssignment As Fixing_assignment = segment.Fixing_assignment.Where(Function(fixAssignment) fixAssignment.Fixing = fx.SystemId).FirstOrDefault
                            If (_kblMapper.KBLOccurrenceMapper.ContainsKey(segment.Start_node) AndAlso TypeOf _kblMapper.KBLOccurrenceMapper(segment.Start_node) Is Node) Then
                                startNode = DirectCast(_kblMapper.KBLOccurrenceMapper(segment.Start_node), Node)
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

    Private Sub ugFixings_InitializeRow(sender As Object, e As InitializeRowEventArgs) Handles ugFixings.InitializeRow
        If Not e.ReInitialize Then
            InitializeFixingRowCore(e.Row)
        End If
    End Sub

    Private Sub ugFixings_KeyDown(sender As Object, e As KeyEventArgs) Handles ugFixings.KeyDown
        OnGridKeyDown(Me, New GridKeyDownEventArgs(e, DirectCast(sender, UltraGrid)))
        If Not e.Handled Then
            If (e.Control) AndAlso (e.KeyCode = Keys.A) Then
                SelectAllRowsOfActiveGrid()
            ElseIf (e.KeyCode = Keys.Escape) Then
                If (Me.ugFixings.Selected.Rows.Count = 0) Then
                    ClearMarkedRowsInGrids()
                Else
                    ClearSelectedRowsInGrids()
                End If

                If (SetInformationHubEventArgs(Nothing, Nothing, Me.ugFixings.Selected.Rows)) Then
                    OnHubSelectionChanged()
                End If

            ElseIf (e.KeyCode = Keys.Return) Then
                SetSelectedAsMarkedOriginRows(ugFixings)
            End If
        End If
    End Sub

    Private Sub ugFixings_MouseDown(sender As Object, e As MouseEventArgs) Handles ugFixings.MouseDown
        If (_contextMenuGrid.IsOpen) Then
            _contextMenuGrid.ClosePopup()
        End If

        _pressedMouseButton = e.Button
    End Sub

    Private Sub ugFixings_MouseLeave(sender As Object, e As EventArgs) Handles ugFixings.MouseLeave
        _messageEventArgs.StatusMessage = String.Empty

        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub ugFixings_MouseMove(sender As Object, e As MouseEventArgs) Handles ugFixings.MouseMove
        _messageEventArgs.StatusMessage = String.Format(InformationHubStrings.RowCount_Label, Me.ugFixings.Rows.Count, Me.ugFixings.Rows.FilteredInNonGroupByRowCount, Me.ugFixings.Rows.VisibleRowCount, Me.ugFixings.Selected.Rows.Count)

        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub udsFixings_CellDataUpdated(sender As Object, e As CellDataUpdatedEventArgs) Handles udsFixings.CellDataUpdated
        RaiseEvent CellValueUpdated(sender, e)
    End Sub

    Private Sub ugHarness_KeyDown(sender As Object, e As KeyEventArgs) Handles ugHarness.KeyDown
        OnGridKeyDown(Me, New GridKeyDownEventArgs(e, DirectCast(sender, UltraGrid)))
    End Sub

End Class