Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinGrid
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Partial Public Class InformationHub

    Private Sub InitializeAssemblyParts()
        _assemblyPartGridAppearance = GridAppearance.All.OfType(Of AssemblyPartGridAppearance).Single
        AddNewRowFilters(ugAssemblyParts)

        If (_comparisonMapperList Is Nothing) Then
            FillDataSource(Me.udsAssemblyParts, _assemblyPartGridAppearance, _kblMapper.GetHarness.Assembly_part_occurrence.Length)
        Else
            FillDataSource(Me.udsAssemblyParts, _assemblyPartGridAppearance, 0, _comparisonMapperList(KblObjectType.Assembly_part_occurrence))
        End If

        If (Me.udsAssemblyParts.Band.Columns.Count = 0) OrElse (Me.udsAssemblyParts.Rows.Count = 0) Then
            Me.utcInformationHub.Tabs(TabNames.tabAssemblyParts.ToString).Visible = False
        End If
    End Sub

    Private Sub udsAssemblyParts_CellDataRequested(sender As Object, e As CellDataRequestedEventArgs) Handles udsAssemblyParts.CellDataRequested
        e.CacheData = False

        With DirectCast(e.Row.Tag, Assembly_part_occurrence)
            Dim assemblyPart As Assembly_part = Nothing
            Dim fromReference As Boolean = True

            If HasChangeTypeWithInverse(e.Row, CompareChangeType.New) Then
                fromReference = False

                If (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLPartMapper.ContainsKey(If(.Part Is Nothing, String.Empty, .Part))) Then
                    assemblyPart = TryCast(_kblMapperForCompare.KBLPartMapper(.Part), Assembly_part)
                End If
            Else
                If (_kblMapper.KBLOccurrenceMapper.ContainsKey(.SystemId)) AndAlso (_kblMapper.KBLPartMapper.ContainsKey(If(.Part Is Nothing, String.Empty, .Part))) Then
                    assemblyPart = TryCast(_kblMapper.KBLPartMapper(.Part), Assembly_part)
                End If
            End If

            If (assemblyPart IsNot Nothing) Then RequestCellPartData(e.Data, fromReference, e.Column.Key, assemblyPart)

            Select Case e.Column.Key
                Case AssemblyPartPropertyName.Id
                    e.Data = .Id
                Case AssemblyPartPropertyName.Alias_id
                    If (.Alias_id.Length <> 0) Then
                        If (.Alias_id.Length = 1) AndAlso (.Alias_id(0).Description Is Nothing) AndAlso (.Alias_id(0).Scope Is Nothing) Then
                            e.Data = .Alias_id(0).Alias_id
                        Else
                            e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                        End If
                    End If
                Case AssemblyPartPropertyName.Description
                    If (.Description IsNot Nothing) Then e.Data = .Description
                Case AssemblyPartPropertyName.Part_type
                    If (assemblyPart IsNot Nothing) AndAlso (assemblyPart.Part_type IsNot Nothing) Then e.Data = assemblyPart.Part_type
                Case AssemblyPartPropertyName.Installation_Information
                    If (.Installation_information.Length <> 0) Then
                        e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                    End If
                Case NameOf(InformationHubStrings.AssignedModules_ColumnCaption)
                    e.Data = GetAssignedModules(.SystemId)
                Case Else
                    OnUnhandledCellDataRequested(e)
            End Select
        End With
    End Sub

    Private Sub udsAssemblyParts_InitializeDataRow(sender As Object, e As InitializeDataRowEventArgs) Handles udsAssemblyParts.InitializeDataRow
        Dim assemblyPartOccurrence As Assembly_part_occurrence

        If (e.Row.Band.Tag Is Nothing) Then
            assemblyPartOccurrence = _kblMapper.GetHarness.Assembly_part_occurrence(e.Row.Index)
        Else
            Dim compareObjKey As String = DirectCast(e.Row.Band.Tag, Dictionary(Of String, Object)).Keys.ElementAt(e.Row.Index)
            Dim compareObjValue As Object = DirectCast(e.Row.Band.Tag, Dictionary(Of String, Object)).Values.ElementAt(e.Row.Index)

            If (TypeOf compareObjValue Is Assembly_part_occurrence) Then
                assemblyPartOccurrence = DirectCast(compareObjValue, Assembly_part_occurrence)
            Else
                assemblyPartOccurrence = DirectCast(_kblMapper.KBLOccurrenceMapper(ExtractSystemId(compareObjKey)), Assembly_part_occurrence)
            End If

            SetDiffCellValueFromObjectKey(e.Row, compareObjKey)
        End If

        e.Row.Tag = assemblyPartOccurrence
    End Sub


    Private Sub ugAssemblyParts_AfterRowActivate(sender As Object, e As EventArgs) Handles ugAssemblyParts.AfterRowActivate
        If (Me.ugAssemblyParts.ActiveRow.Appearance.BackColor = Color.FromArgb(190, 190, 190)) Then
            Me.ugAssemblyParts.ActiveRow = Nothing
        ElseIf (_kblMapperForCompare IsNot Nothing) Then
            If (SetInformationHubEventArgs(Nothing, Me.ugAssemblyParts.ActiveRow, Nothing)) Then
                OnHubSelectionChanged()
            End If
            If (Not Me.Focused) Then Me.Focus()
        End If
    End Sub

    Private Sub ugAssemblyParts_AfterSelectChange(sender As Object, e As AfterSelectChangeEventArgs) Handles ugAssemblyParts.AfterSelectChange
        With Me.ugAssemblyParts
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

    Private Sub ugAssemblyParts_BeforeSelectChange(sender As Object, e As BeforeSelectChangeEventArgs) Handles ugAssemblyParts.BeforeSelectChange
        For Each row As UltraGridRow In e.NewSelections.Rows
            If (row.Appearance.BackColor = Color.FromArgb(190, 190, 190)) Then e.Cancel = True
        Next
    End Sub

    Private Sub ugAssemblyParts_ClickCell(sender As Object, e As ClickCellEventArgs) Handles ugAssemblyParts.ClickCell
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

    Private Sub ugAssemblyParts_ClickCellButton(sender As Object, e As CellEventArgs) Handles ugAssemblyParts.ClickCellButton
        If (_pressedMouseButton = System.Windows.Forms.MouseButtons.Left) Then
            Dim fromReference As Boolean = True

            If HasChangeTypeWithInverse(CType(e.Cell.Row.ListObject, UltraDataRow), CompareChangeType.New) Then
                fromReference = False
            End If

            If (TypeOf e.Cell.Tag Is Assembly_part_occurrence) Then
                Dim assemblyPartOccurrence As Assembly_part_occurrence = DirectCast(e.Cell.Tag, Assembly_part_occurrence)
                Dim assemblyPart As Assembly_part = Nothing

                If (fromReference) AndAlso (_kblMapper.KBLPartMapper.ContainsKey(assemblyPartOccurrence.Part)) Then
                    assemblyPart = DirectCast(_kblMapper.KBLPartMapper(assemblyPartOccurrence.Part), Assembly_part)

                    RequestDialogPartData(_kblMapper, e.Cell.Column.Key, assemblyPart)
                ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLPartMapper.ContainsKey(assemblyPartOccurrence.Part)) Then
                    assemblyPart = DirectCast(_kblMapperForCompare.KBLPartMapper(assemblyPartOccurrence.Part), Assembly_part)

                    RequestDialogPartData(_kblMapperForCompare, e.Cell.Column.Key, assemblyPart)
                End If

                With assemblyPartOccurrence
                    Select Case e.Cell.Column.Key
                        Case AssemblyPartPropertyName.Alias_id.ToString
                            ShowDetailInformationForm(InformationHubStrings.AliasIds_Caption, .Alias_id, Nothing, _kblMapper)
                        Case AssemblyPartPropertyName.Installation_Information.ToString
                            ShowDetailInformationForm(InformationHubStrings.InstallInfo_Caption, .Installation_information, Nothing, _kblMapper)
                    End Select
                End With
            ElseIf (TypeOf e.Cell.Tag Is KeyValuePair(Of String, Object)) Then
                RequestDialogCompareData(True, DirectCast(e.Cell.Row.Tag, KeyValuePair(Of String, Object)).Key.SplitRemoveEmpty("|"c)(1), DirectCast(e.Cell.Tag, KeyValuePair(Of String, Object)))
            End If
        End If
    End Sub

    Private Sub ugAssemblyParts_DoubleClickRow(sender As Object, e As DoubleClickRowEventArgs) Handles ugAssemblyParts.DoubleClickRow
        OnDoubleClickRow(sender, e)
    End Sub

    Private Sub ugAssemblyParts_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugAssemblyParts.InitializeLayout
        Me.ugAssemblyParts.BeginUpdate()
        Me.ugAssemblyParts.EventManager.AllEventsEnabled = False

        InitializeGridLayout(_assemblyPartGridAppearance, e.Layout)

        Me.ugAssemblyParts.EventManager.AllEventsEnabled = True
        Me.ugAssemblyParts.EndUpdate()
    End Sub

    Private Sub InitializeAssemblyPartRowCore(row As UltraGridRow)
        If (DirectCast(row.ListObject, UltraDataRow).Band.Tag Is Nothing) Then
            row.Tag = DirectCast(DirectCast(row.ListObject, UltraDataRow).Tag, Assembly_part_occurrence).SystemId

            If _redliningInformation IsNot Nothing AndAlso _redliningInformation IsNot Nothing AndAlso _redliningInformation.Redlinings.Where(Function(redlining) redlining.ObjectId = row.Tag?.ToString).Any() Then
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
                    If (TypeOf compareObject.Value Is AssemblyPartChangedProperty) Then
                        For Each changedProperty As KeyValuePair(Of String, Object) In DirectCast(compareObject.Value, AssemblyPartChangedProperty).ChangedProperties
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

    Private Sub ugAssemblyParts_InitializeRow(sender As Object, e As InitializeRowEventArgs) Handles ugAssemblyParts.InitializeRow
        If Not e.ReInitialize Then
            InitializeAssemblyPartRowCore(e.Row)
        End If
    End Sub

    Private Sub ugAssemblyParts_KeyDown(sender As Object, e As KeyEventArgs) Handles ugAssemblyParts.KeyDown
        OnGridKeyDown(Me, New GridKeyDownEventArgs(e, DirectCast(sender, UltraGrid)))
        If Not e.Handled Then
            If (e.Control) AndAlso (e.KeyCode = Keys.A) Then
                SelectAllRowsOfActiveGrid()
            ElseIf (e.KeyCode = Keys.Escape) Then
                If (Me.ugAssemblyParts.Selected.Rows.Count = 0) Then
                    ClearMarkedRowsInGrids()
                Else
                    ClearSelectedRowsInGrids()
                End If

                If (SetInformationHubEventArgs(Nothing, Nothing, Me.ugAssemblyParts.Selected.Rows)) Then
                    OnHubSelectionChanged()
                End If

            ElseIf (e.KeyCode = Keys.Return) Then
                SetSelectedAsMarkedOriginRows(ugAssemblyParts)
            End If
        End If
    End Sub

    Private Sub ugAssemblyParts_MouseDown(sender As Object, e As MouseEventArgs) Handles ugAssemblyParts.MouseDown
        If (_contextMenuGrid.IsOpen) Then
            _contextMenuGrid.ClosePopup()
        End If

        _pressedMouseButton = e.Button
    End Sub

    Private Sub ugAssemblyParts_MouseLeave(sender As Object, e As EventArgs) Handles ugAssemblyParts.MouseLeave
        _messageEventArgs.StatusMessage = String.Empty

        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub ugAssemblyParts_MouseMove(sender As Object, e As MouseEventArgs) Handles ugAssemblyParts.MouseMove
        _messageEventArgs.StatusMessage = String.Format(InformationHubStrings.RowCount_Label, Me.ugAssemblyParts.Rows.Count, Me.ugAssemblyParts.Rows.FilteredInNonGroupByRowCount, Me.ugAssemblyParts.Rows.VisibleRowCount, Me.ugAssemblyParts.Selected.Rows.Count)

        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub udsAssemblyParts_CellDataUpdated(sender As Object, e As CellDataUpdatedEventArgs) Handles udsAssemblyParts.CellDataUpdated
        RaiseEvent CellValueUpdated(sender, e)
    End Sub

End Class