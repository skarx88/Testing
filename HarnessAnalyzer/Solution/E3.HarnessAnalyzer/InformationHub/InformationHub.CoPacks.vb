Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinGrid
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Partial Public Class InformationHub

    Private Sub InitializeCoPacks()
        _coPackGridAppearance = GridAppearance.All.OfType(Of CoPackGridAppearance).Single
        AddNewRowFilters(ugCoPacks)

        If (_comparisonMapperList Is Nothing) Then
            FillDataSource(Me.udsCoPacks, _coPackGridAppearance, _kblMapper.GetCoPackOccurrences.Count)
        Else
            FillDataSource(Me.udsCoPacks, _coPackGridAppearance, 0, _comparisonMapperList(KblObjectType.Co_pack_occurrence))
        End If

        If (Me.udsCoPacks.Band.Columns.Count = 0) OrElse (Me.udsCoPacks.Rows.Count = 0) Then
            Me.utcInformationHub.Tabs(TabNames.tabCoPacks.ToString()).Visible = False
        End If
    End Sub

    Private Sub udsCoPacks_CellDataRequested(sender As Object, e As CellDataRequestedEventArgs) Handles udsCoPacks.CellDataRequested
        e.CacheData = False

        With DirectCast(e.Row.Tag, Co_pack_occurrence)
            Dim coPackPart As Co_pack_part = Nothing
            Dim fromReference As Boolean = True

            If HasChangeTypeWithInverse(e.Row, CompareChangeType.New) Then
                fromReference = False
                If (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLPartMapper.ContainsKey(If(.Part Is Nothing, String.Empty, .Part))) Then
                    coPackPart = _kblMapperForCompare.GetPart(Of Co_pack_part)(.Part)
                End If
            Else
                If (_kblMapper.KBLOccurrenceMapper.ContainsKey(.SystemId)) AndAlso (_kblMapper.KBLPartMapper.ContainsKey(If(.Part Is Nothing, String.Empty, .Part))) Then
                    coPackPart = _kblMapper.GetPart(Of Co_pack_part)(.Part)
                End If
            End If

            If (coPackPart IsNot Nothing) Then
                RequestCellPartData(e.Data, fromReference, e.Column.Key, coPackPart)
            End If

            Select Case e.Column.Key
                Case CoPackPropertyName.Id
                    e.Data = .Id
                Case CoPackPropertyName.Alias_Id
                    If (.Alias_id.Length <> 0) Then
                        If (.Alias_id.Length = 1) AndAlso (.Alias_id(0).Description Is Nothing) AndAlso (.Alias_id(0).Scope Is Nothing) Then
                            e.Data = .Alias_id(0).Alias_id
                        Else
                            e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                        End If
                    End If
                Case CoPackPropertyName.Description
                    If (.Description IsNot Nothing) Then
                        e.Data = .Description
                    End If
                Case CoPackPropertyName.Part_type
                    If (coPackPart IsNot Nothing) AndAlso (coPackPart.Part_type IsNot Nothing) Then
                        e.Data = coPackPart.Part_type
                    End If
                Case CoPackPropertyName.Installation_Information
                    If (.Installation_information.Length > 0) Then
                        e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                    End If
                Case CoPackPropertyName.Localized_Description
                    If (.Localized_description.Length > 0) Then
                        If (.Localized_description.Length = 1) Then
                            e.Data = .Localized_description(0).Value
                        Else
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

    Private Sub udsCoPacks_InitializeDataRow(sender As Object, e As InitializeDataRowEventArgs) Handles udsCoPacks.InitializeDataRow
        Dim coPackOccurrence As Co_pack_occurrence

        If (e.Row.Band.Tag Is Nothing) Then
            coPackOccurrence = _kblMapper.GetCoPackOccurrences(e.Row.Index)
        Else
            Dim compareObjKey As String = DirectCast(e.Row.Band.Tag, Dictionary(Of String, Object)).Keys.ElementAt(e.Row.Index)
            Dim compareObjValue As Object = DirectCast(e.Row.Band.Tag, Dictionary(Of String, Object)).Values.ElementAt(e.Row.Index)

            If (TypeOf compareObjValue Is Co_pack_occurrence) Then
                coPackOccurrence = DirectCast(compareObjValue, Co_pack_occurrence)
            Else
                coPackOccurrence = DirectCast(_kblMapper.KBLOccurrenceMapper(ExtractSystemId(compareObjKey)), Co_pack_occurrence)
            End If

            SetDiffCellValueFromObjectKey(e.Row, compareObjKey)
        End If

        e.Row.Tag = coPackOccurrence
    End Sub


    Private Sub ugCoPacks_AfterRowActivate(sender As Object, e As EventArgs) Handles ugCoPacks.AfterRowActivate
        If (Me.ugCoPacks.ActiveRow.Appearance.BackColor = Color.FromArgb(190, 190, 190)) Then
            Me.ugCoPacks.ActiveRow = Nothing
        ElseIf (_kblMapperForCompare IsNot Nothing) Then
            If (SetInformationHubEventArgs(Nothing, Me.ugCoPacks.ActiveRow, Nothing)) Then
                OnHubSelectionChanged()
            End If
            If (Not Me.Focused) Then Me.Focus()
        End If
    End Sub

    Private Sub ugCoPacks_AfterSelectChange(sender As Object, e As AfterSelectChangeEventArgs) Handles ugCoPacks.AfterSelectChange
        With Me.ugCoPacks
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

    Private Sub ugCoPacks_BeforeSelectChange(sender As Object, e As BeforeSelectChangeEventArgs) Handles ugCoPacks.BeforeSelectChange
        For Each row As UltraGridRow In e.NewSelections.Rows
            If (row.Appearance.BackColor = Color.FromArgb(190, 190, 190)) Then e.Cancel = True
        Next
    End Sub

    Private Sub ugCoPacks_ClickCell(sender As Object, e As ClickCellEventArgs) Handles ugCoPacks.ClickCell
        If (_pressedMouseButton = System.Windows.Forms.MouseButtons.Left) AndAlso (_pressedKey <> Keys.ControlKey AndAlso _pressedKey <> Keys.ShiftKey) AndAlso (_informationHubEventArgs IsNot Nothing) AndAlso (_kblMapperForCompare IsNot Nothing OrElse e.Cell.Row.Appearance.BackColor <> Color.FromArgb(190, 190, 190)) Then
            Dim prevIds As IENumerable(Of String) = _informationHubEventArgs.ObjectIds

            SetInformationHubEventArgs(Nothing, e.Cell.Row, Nothing)

            If (prevIds IsNot Nothing) AndAlso (String.Join(";", prevIds) <> String.Join(";", _informationHubEventArgs.ObjectIds)) Then
                OnHubSelectionChanged()

                If (Not Me.Focused) Then Me.Focus()
            End If
        ElseIf (_pressedMouseButton = System.Windows.Forms.MouseButtons.Right) AndAlso (_kblMapperForCompare Is Nothing) AndAlso (ChangeGridContextMenuVisibility(e.Cell.Row)) Then
            _contextMenuGrid.ShowPopup()
        End If
    End Sub

    Private Sub ugCoPacks_ClickCellButton(sender As Object, e As CellEventArgs) Handles ugCoPacks.ClickCellButton
        If (_pressedMouseButton = System.Windows.Forms.MouseButtons.Left) Then
            Dim fromReference As Boolean = True

            If HasChangeTypeWithInverse(CType(e.Cell.Row.ListObject, UltraDataRow), CompareChangeType.New) Then
                fromReference = False
            End If

            If (TypeOf e.Cell.Tag Is Co_pack_occurrence) Then
                Dim coPackOccurrence As Co_pack_occurrence = DirectCast(e.Cell.Tag, Co_pack_occurrence)
                Dim coPackPart As Co_pack_part = Nothing

                If (fromReference) AndAlso (_kblMapper.KblPartMapper.ContainsKey(coPackOccurrence.Part)) Then
                    coPackPart = DirectCast(_kblMapper.KblPartMapper(coPackOccurrence.Part), Co_pack_part)

                    RequestDialogPartData(_kblMapper, e.Cell.Column.Key, coPackPart)
                ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KblPartMapper.ContainsKey(coPackOccurrence.Part)) Then
                    coPackPart = DirectCast(_kblMapperForCompare.KblPartMapper(coPackOccurrence.Part), Co_pack_part)

                    RequestDialogPartData(_kblMapperForCompare, e.Cell.Column.Key, coPackPart)
                End If

                With coPackOccurrence
                    Select Case e.Cell.Column.Key
                        Case CoPackPropertyName.Alias_Id.ToString
                            ShowDetailInformationForm(InformationHubStrings.AliasIds_Caption, .Alias_Id, Nothing, _kblMapper)
                        Case CoPackPropertyName.Installation_Information.ToString
                            ShowDetailInformationForm(InformationHubStrings.InstallInfo_Caption, .Installation_Information, Nothing, _kblMapper)
                    End Select
                End With
            ElseIf (TypeOf e.Cell.Tag Is KeyValuePair(Of String, Object)) Then
                RequestDialogCompareData(True, DirectCast(e.Cell.Row.Tag, KeyValuePair(Of String, Object)).Key.SplitRemoveEmpty("|"c)(1), DirectCast(e.Cell.Tag, KeyValuePair(Of String, Object)))
            End If
        End If
    End Sub

    Private Sub ugCoPacks_DoubleClickRow(sender As Object, e As DoubleClickRowEventArgs) Handles ugCoPacks.DoubleClickRow
        OnDoubleClickRow(sender, e)
    End Sub

    Private Sub ugCoPacks_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugCoPacks.InitializeLayout
        Me.ugCoPacks.BeginUpdate()
        Me.ugCoPacks.EventManager.AllEventsEnabled = False

        InitializeGridLayout(_coPackGridAppearance, e.Layout)

        Me.ugCoPacks.EventManager.AllEventsEnabled = True
        Me.ugCoPacks.EndUpdate()
    End Sub

    Private Sub InitializeCoPackRowCore(row As UltraGridRow)
        If (DirectCast(row.ListObject, UltraDataRow).Band.Tag Is Nothing) Then
            row.Tag = DirectCast(DirectCast(row.ListObject, UltraDataRow).Tag, Co_pack_occurrence).SystemId

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

                    If (TypeOf compareObject.Value Is CoPackChangedProperty) Then
                        For Each changedProperty As KeyValuePair(Of String, Object) In DirectCast(compareObject.Value, CoPackChangedProperty).ChangedProperties
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
    End Sub

    Private Sub ugCoPacks_InitializeRow(sender As Object, e As InitializeRowEventArgs) Handles ugCoPacks.InitializeRow
        If Not e.ReInitialize Then
            InitializeCoPackRowCore(e.Row)
        End If
    End Sub

    Private Sub ugCoPacks_KeyDown(sender As Object, e As KeyEventArgs) Handles ugCoPacks.KeyDown
        OnGridKeyDown(Me, New GridKeyDownEventArgs(e, DirectCast(sender, UltraGrid)))
        If Not e.Handled Then
            If (e.Control) AndAlso (e.KeyCode = Keys.A) Then
                SelectAllRowsOfActiveGrid()
            ElseIf (e.KeyCode = Keys.Escape) Then
                If (Me.ugCoPacks.Selected.Rows.Count = 0) Then
                    ClearMarkedRowsInGrids()
                Else
                    ClearSelectedRowsInGrids()
                End If

                If (SetInformationHubEventArgs(Nothing, Nothing, Me.ugCoPacks.Selected.Rows)) Then
                    OnHubSelectionChanged()
                End If

            ElseIf (e.KeyCode = Keys.Return) Then
                SetSelectedAsMarkedOriginRows(ugCoPacks)
            End If
        End If
    End Sub

    Private Sub ugCoPacks_MouseDown(sender As Object, e As MouseEventArgs) Handles ugCoPacks.MouseDown
        If (_contextMenuGrid.IsOpen) Then
            _contextMenuGrid.ClosePopup()
        End If

        _pressedMouseButton = e.Button
    End Sub

    Private Sub ugCoPacks_MouseLeave(sender As Object, e As EventArgs) Handles ugCoPacks.MouseLeave
        _messageEventArgs.StatusMessage = String.Empty

        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub ugCoPacks_MouseMove(sender As Object, e As MouseEventArgs) Handles ugCoPacks.MouseMove
        _messageEventArgs.StatusMessage = String.Format(InformationHubStrings.RowCount_Label, Me.ugCoPacks.Rows.Count, Me.ugCoPacks.Rows.FilteredInNonGroupByRowCount, Me.ugCoPacks.Rows.VisibleRowCount, Me.ugCoPacks.Selected.Rows.Count)

        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub udsCoPacks_CellDataUpdated(sender As Object, e As CellDataUpdatedEventArgs) Handles udsCoPacks.CellDataUpdated
        RaiseEvent CellValueUpdated(sender, e)
    End Sub

End Class