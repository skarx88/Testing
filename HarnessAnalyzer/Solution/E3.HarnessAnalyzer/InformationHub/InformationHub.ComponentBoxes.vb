Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinGrid
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Partial Public Class InformationHub

    Private Sub InitializeComponentBoxes()
        _componentBoxGridAppearance = GridAppearance.All.OfType(Of ComponentBoxGridAppearance).Single
        AddNewRowFilters(ugComponentBoxes)

        If (_comparisonMapperList Is Nothing) Then
            FillDataSource(Me.udsComponentBoxes, _componentBoxGridAppearance, _kblMapper.GetComponentBoxOccurrences.Count)
        Else
            FillDataSource(Me.udsComponentBoxes, _componentBoxGridAppearance, 0, _comparisonMapperList(KblObjectType.Component_box_occurrence))
        End If

        If (Me.udsComponentBoxes.Band.Columns.Count = 0) OrElse (Me.udsComponentBoxes.Rows.Count = 0) Then
            Me.utcInformationHub.Tabs(TabNames.tabComponentBoxes.ToString()).Visible = False

        End If
    End Sub

    Private Sub udsComponentBoxes_CellDataRequested(sender As Object, e As CellDataRequestedEventArgs) Handles udsComponentBoxes.CellDataRequested
        e.CacheData = False

        If e.Row.Tag Is Nothing Then
            Exit Sub
        End If

        With DirectCast(e.Row.Tag, Component_box_occurrence)
            Dim componentBox As Component_box = Nothing
            Dim fromReference As Boolean = True

            If HasChangeTypeWithInverse(e.Row, CompareChangeType.New) Then
                fromReference = False

                If (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLPartMapper.ContainsKey(If(.Part Is Nothing, String.Empty, .Part))) Then componentBox = TryCast(_kblMapperForCompare.KBLPartMapper(.Part), Component_box)
            Else
                If (_kblMapper.KBLOccurrenceMapper.ContainsKey(.SystemId)) AndAlso (_kblMapper.KBLPartMapper.ContainsKey(If(.Part Is Nothing, String.Empty, .Part))) Then componentBox = TryCast(_kblMapper.KBLPartMapper(.Part), Component_box)
            End If

            If (componentBox IsNot Nothing) Then
                RequestCellPartData(e.Data, fromReference, e.Column.Key, componentBox)
            End If

            Select Case e.Column.Key
                Case ComponentBoxPropertyName.Id
                    e.Data = .Id
                Case ComponentBoxPropertyName.Alias_id
                    If (.Alias_id.Length <> 0) Then
                        If (.Alias_id.Length = 1) AndAlso (.Alias_id(0).Description Is Nothing) AndAlso (.Alias_id(0).Scope Is Nothing) Then
                            e.Data = .Alias_id(0).Alias_id
                        Else
                            e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                        End If
                    End If
                Case ComponentBoxPropertyName.Description
                    If (.Description IsNot Nothing) Then
                        e.Data = .Description
                    End If
                Case ComponentBoxPropertyName.Reference_element
                    If (.Reference_element IsNot Nothing) AndAlso (.Reference_element <> String.Empty) Then
                        If (fromReference) Then
                            If (_kblMapper.KBLOccurrenceMapper.ContainsKey(.Reference_element)) Then e.Data = Harness.GetReferenceElement(.Reference_element, _kblMapper)
                        Else
                            If (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLOccurrenceMapper.ContainsKey(.Reference_element)) Then
                                e.Data = Harness.GetReferenceElement(.Reference_element, _kblMapperForCompare)
                            End If
                        End If
                    End If
                Case ComponentBoxPropertyName.Installation_Information
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

    Private Sub udsComponentBoxes_InitializeDataRow(sender As Object, e As InitializeDataRowEventArgs) Handles udsComponentBoxes.InitializeDataRow
        Dim componentBoxOccurrence As Component_box_occurrence = Nothing

        If (e.Row.Band.Tag Is Nothing) Then
            componentBoxOccurrence = _kblMapper.GetComponentBoxOccurrences(e.Row.Index)
        Else
            Dim compareObjKey As String = DirectCast(e.Row.Band.Tag, Dictionary(Of String, Object)).Keys.ElementAt(e.Row.Index)
            Dim compareObjValue As Object = DirectCast(e.Row.Band.Tag, Dictionary(Of String, Object)).Values.ElementAt(e.Row.Index)

            If (TypeOf compareObjValue Is Accessory_occurrence) Then
                componentBoxOccurrence = DirectCast(compareObjValue, Component_box_occurrence)
            Else
                Dim key As String = ExtractSystemId(compareObjKey)
                If _kblMapper.KBLOccurrenceMapper.ContainsKey(key) Then
                    componentBoxOccurrence = DirectCast(_kblMapper.KBLOccurrenceMapper(ExtractSystemId(compareObjKey)), Component_box_occurrence)
                End If
            End If

            SetDiffCellValueFromObjectKey(e.Row, compareObjKey)
        End If

        e.Row.Tag = componentBoxOccurrence
    End Sub


    Private Sub ugComponentBoxes_AfterRowActivate(sender As Object, e As EventArgs) Handles ugComponentBoxes.AfterRowActivate
        If (Me.ugComponentBoxes.ActiveRow.Appearance.BackColor = Color.FromArgb(190, 190, 190)) Then
            Me.ugComponentBoxes.ActiveRow = Nothing
        ElseIf (_kblMapperForCompare IsNot Nothing) Then
            If (SetInformationHubEventArgs(Nothing, Me.ugComponentBoxes.ActiveRow, Nothing)) Then
                OnHubSelectionChanged()
            End If
            If (Not Me.Focused) Then Me.Focus()
        End If
    End Sub

    Private Sub ugComponentBoxes_AfterSelectChange(sender As Object, e As AfterSelectChangeEventArgs) Handles ugComponentBoxes.AfterSelectChange
        With Me.ugComponentBoxes
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

    Private Sub ugComponentBoxes_BeforeSelectChange(sender As Object, e As BeforeSelectChangeEventArgs) Handles ugComponentBoxes.BeforeSelectChange
        For Each row As UltraGridRow In e.NewSelections.Rows
            If (row.Appearance.BackColor = Color.FromArgb(190, 190, 190)) Then
                e.Cancel = True
            End If
        Next
    End Sub

    Private Sub ugComponentBoxes_ClickCell(sender As Object, e As ClickCellEventArgs) Handles ugComponentBoxes.ClickCell
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

    Private Sub ugComponentBoxes_ClickCellButton(sender As Object, e As CellEventArgs) Handles ugComponentBoxes.ClickCellButton
        If (_pressedMouseButton = System.Windows.Forms.MouseButtons.Left) Then
            Dim fromReference As Boolean = True

            If HasChangeTypeWithInverse(CType(e.Cell.Row.ListObject, UltraDataRow), CompareChangeType.New) Then
                fromReference = False
            End If

            If (TypeOf e.Cell.Tag Is Component_box_occurrence) Then
                Dim componentBoxOccurrence As Component_box_occurrence = DirectCast(e.Cell.Tag, Component_box_occurrence)
                Dim componentBox As Component_box = Nothing

                If (fromReference) AndAlso (_kblMapper.KblPartMapper.ContainsKey(componentBoxOccurrence.Part)) Then
                    componentBox = DirectCast(_kblMapper.KblPartMapper(componentBoxOccurrence.Part), Component_box)

                    RequestDialogPartData(_kblMapper, e.Cell.Column.Key, componentBox)
                ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KblPartMapper.ContainsKey(componentBoxOccurrence.Part)) Then
                    componentBox = DirectCast(_kblMapperForCompare.KblPartMapper(componentBoxOccurrence.Part), Component_box)

                    RequestDialogPartData(_kblMapperForCompare, e.Cell.Column.Key, componentBox)
                End If

                With componentBoxOccurrence
                    Select Case e.Cell.Column.Key
                        Case ComponentBoxPropertyName.Alias_id.ToString
                            ShowDetailInformationForm(InformationHubStrings.AliasIds_Caption, .Alias_Id, Nothing, _kblMapper)
                        Case ComponentBoxPropertyName.Installation_Information.ToString
                            ShowDetailInformationForm(InformationHubStrings.InstallInfo_Caption, .Installation_Information, Nothing, _kblMapper)
                    End Select
                End With
            ElseIf (TypeOf e.Cell.Tag Is KeyValuePair(Of String, Object)) Then
                RequestDialogCompareData(True, DirectCast(e.Cell.Row.Tag, KeyValuePair(Of String, Object)).Key.SplitRemoveEmpty("|"c)(1), DirectCast(e.Cell.Tag, KeyValuePair(Of String, Object)))
            End If
        End If
    End Sub

    Private Sub ugComponentBoxes_DoubleClickRow(sender As Object, e As DoubleClickRowEventArgs) Handles ugComponentBoxes.DoubleClickRow
        OnDoubleClickRow(sender, e)
    End Sub

    Private Sub ugComponentBoxes_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugComponentBoxes.InitializeLayout
        Me.ugComponentBoxes.BeginUpdate()
        Me.ugComponentBoxes.EventManager.AllEventsEnabled = False

        InitializeGridLayout(_componentBoxGridAppearance, e.Layout)

        Me.ugComponentBoxes.EventManager.AllEventsEnabled = True
        Me.ugComponentBoxes.EndUpdate()
    End Sub

    Private Sub InitializeComponentBoxRowCore(row As UltraGridRow)
        If (DirectCast(row.ListObject, UltraDataRow).Band.Tag Is Nothing) Then
            row.Tag = DirectCast(DirectCast(row.ListObject, UltraDataRow).Tag, Component_box_occurrence).SystemId

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
                    If (TypeOf compareObject.Value Is ComponentBoxChangedProperty) Then
                        For Each changedProperty As KeyValuePair(Of String, Object) In DirectCast(compareObject.Value, ComponentBoxChangedProperty).ChangedProperties
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

    Private Sub ugComponentBoxes_InitializeRow(sender As Object, e As InitializeRowEventArgs) Handles ugComponentBoxes.InitializeRow
        If Not e.ReInitialize Then
            InitializeComponentBoxRowCore(e.Row)
        End If
    End Sub

    Private Sub ugComponentBoxes_KeyDown(sender As Object, e As KeyEventArgs) Handles ugComponentBoxes.KeyDown
        OnGridKeyDown(Me, New GridKeyDownEventArgs(e, DirectCast(sender, UltraGrid)))
        If Not e.Handled Then
            If (e.Control) AndAlso (e.KeyCode = Keys.A) Then
                SelectAllRowsOfActiveGrid()
            ElseIf (e.KeyCode = Keys.Escape) Then
                If (Me.ugComponentBoxes.Selected.Rows.Count = 0) Then
                    ClearMarkedRowsInGrids()
                Else
                    ClearSelectedRowsInGrids()
                End If

                If (SetInformationHubEventArgs(Nothing, Nothing, Me.ugComponentBoxes.Selected.Rows)) Then
                    OnHubSelectionChanged()
                End If

            ElseIf (e.KeyCode = Keys.Return) Then
                SetSelectedAsMarkedOriginRows(ugComponentBoxes)
            End If
        End If
    End Sub

    Private Sub ugComponentBoxes_MouseDown(sender As Object, e As MouseEventArgs) Handles ugComponentBoxes.MouseDown
        If (_contextMenuGrid.IsOpen) Then _contextMenuGrid.ClosePopup()

        _pressedMouseButton = e.Button
    End Sub

    Private Sub ugComponentBoxes_MouseLeave(sender As Object, e As EventArgs) Handles ugComponentBoxes.MouseLeave
        _messageEventArgs.StatusMessage = String.Empty

        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub ugComponentBoxes_MouseMove(sender As Object, e As MouseEventArgs) Handles ugComponentBoxes.MouseMove
        _messageEventArgs.StatusMessage = String.Format(InformationHubStrings.RowCount_Label, Me.ugComponentBoxes.Rows.Count, Me.ugComponentBoxes.Rows.FilteredInNonGroupByRowCount, Me.ugComponentBoxes.Rows.VisibleRowCount, Me.ugComponentBoxes.Selected.Rows.Count)

        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub udsComponentBoxes_CellDataUpdated(sender As Object, e As CellDataUpdatedEventArgs) Handles udsComponentBoxes.CellDataUpdated
        RaiseEvent CellValueUpdated(sender, e)
    End Sub

End Class