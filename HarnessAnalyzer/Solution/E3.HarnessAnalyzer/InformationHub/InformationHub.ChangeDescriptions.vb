Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinGrid
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Partial Public Class InformationHub

    Private Sub InitializeChangeDescriptions()
        _changeDescriptionGridAppearance = GridAppearance.All.OfType(Of ChangeDescriptionGridAppearance).Single

        If (_comparisonMapperList Is Nothing) Then
            FillDataSource(Me.udsChangeDescriptions, _changeDescriptionGridAppearance, _kblMapper.GetChangeDescriptions.Count)
        Else
            FillDataSource(Me.udsChangeDescriptions, _changeDescriptionGridAppearance, 0, _comparisonMapperList(KblObjectType.Change_description))
        End If

        If (Me.udsChangeDescriptions.Band.Columns.Count = 0) OrElse (Me.udsChangeDescriptions.Rows.Count = 0) Then
            Me.utcInformationHub.Tabs(TabNames.tabChangeDescriptions.ToString).Visible = False
        End If
    End Sub

    Private Sub udsChangeDescriptions_CellDataRequested(sender As Object, e As CellDataRequestedEventArgs) Handles udsChangeDescriptions.CellDataRequested
        e.CacheData = False

        With DirectCast(e.Row.Tag, Change_description)
            Dim kblMapper As KblMapper = _kblMapper

            If e.Row.Band.Columns.Exists(NameOf(InformationHubStrings.DiffType_ColumnCaption)) AndAlso e.Row.GetCellValue(NameOf(InformationHubStrings.DiffType_ColumnCaption)).ToString = InformationHubStrings.Added Then
                kblMapper = If(_kblMapperForCompare, _kblMapper)
            End If

            Select Case e.Column.Key
                Case ChangePropertyName.Id
                    e.Data = .Id
                Case ChangePropertyName.Description
                    If (.Description IsNot Nothing) Then
                        e.Data = .Description
                    End If
                Case ChangePropertyName.Change_request
                    If (.Change_request IsNot Nothing) Then
                        e.Data = .Change_request
                    End If
                Case ChangePropertyName.Change_date
                    If (.Change_date IsNot Nothing) Then
                        e.Data = .Change_date
                    End If
                Case ChangePropertyName.Responsible_designer
                    e.Data = .Responsible_designer
                Case ChangePropertyName.Designer_department
                    e.Data = .Designer_department
                Case ChangePropertyName.Approver_name
                    If (.Approver_name IsNot Nothing) Then
                        e.Data = .Approver_name
                    End If
                Case ChangePropertyName.Approver_department
                    If (.Approver_department IsNot Nothing) Then
                        e.Data = .Approver_department
                    End If
                Case ChangeDescriptionPropertyName.Changed_elements
                    If (.Changed_elements IsNot Nothing) AndAlso (.Changed_elements <> String.Empty) Then
                        Dim changedElementIds As List(Of String) = .Changed_elements.SplitSpace.ToList
                        If (changedElementIds.Count = 1) Then
                            e.Data = Change_description.GetChangedElement(changedElementIds.FirstOrDefault, kblMapper)
                            'needs data object with link
                        Else
                            e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                        End If
                    End If
                Case ChangeDescriptionPropertyName.External_references
                    If (.External_references IsNot Nothing) AndAlso (.External_references <> String.Empty) Then e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                Case ChangeDescriptionPropertyName.Related_changes
                    If (.Related_changes IsNot Nothing) AndAlso (.Related_changes <> String.Empty) Then
                        e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                    End If
                Case Else
                    OnUnhandledCellDataRequested(e)
            End Select
        End With
    End Sub

    Private Sub udsChangeDescriptions_InitializeDataRow(sender As Object, e As InitializeDataRowEventArgs) Handles udsChangeDescriptions.InitializeDataRow
        Dim changeDescription As Change_description = Nothing

        If (e.Row.Band.Tag Is Nothing) Then
            changeDescription = _kblMapper.GetChangeDescriptions(e.Row.Index)
        Else
            Dim compareObjKey As String = DirectCast(e.Row.Band.Tag, Dictionary(Of String, Object)).Keys.ElementAt(e.Row.Index)
            Dim compareObjValue As Object = DirectCast(e.Row.Band.Tag, Dictionary(Of String, Object)).Values.ElementAt(e.Row.Index)

            If (TypeOf compareObjValue Is Change_description) Then
                changeDescription = DirectCast(compareObjValue, Change_description)
            Else
                changeDescription = DirectCast(_kblMapper.KBLOccurrenceMapper(ExtractSystemId(compareObjKey)), Change_description)
            End If

            SetDiffCellValueFromObjectKey(e.Row, compareObjKey)
        End If

        e.Row.Tag = changeDescription
    End Sub


    Private Sub ugChangeDescriptions_AfterRowActivate(sender As Object, e As EventArgs) Handles ugChangeDescriptions.AfterRowActivate
        If (Me.ugChangeDescriptions.ActiveRow.Appearance.BackColor = Color.FromArgb(190, 190, 190)) Then
            Me.ugChangeDescriptions.ActiveRow = Nothing
        ElseIf (_kblMapperForCompare IsNot Nothing) Then
            If (SetInformationHubEventArgs(Nothing, Me.ugChangeDescriptions.ActiveRow, Nothing)) Then
                OnHubSelectionChanged()
            End If
            If (Not Me.Focused) Then
                Me.Focus()
            End If
        End If
    End Sub

    Private Sub ugChangeDescriptions_AfterSelectChange(sender As Object, e As AfterSelectChangeEventArgs) Handles ugChangeDescriptions.AfterSelectChange
        With Me.ugChangeDescriptions
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

    Private Sub ugChangeDescriptions_BeforeSelectChange(sender As Object, e As BeforeSelectChangeEventArgs) Handles ugChangeDescriptions.BeforeSelectChange
        For Each row As UltraGridRow In e.NewSelections.Rows
            If (row.Appearance.BackColor = Color.FromArgb(190, 190, 190)) Then e.Cancel = True
        Next
    End Sub

    Private Sub ugChangeDescriptions_ClickCell(sender As Object, e As ClickCellEventArgs) Handles ugChangeDescriptions.ClickCell
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

    Private Sub ugChangeDescriptions_ClickCellButton(sender As Object, e As CellEventArgs) Handles ugChangeDescriptions.ClickCellButton
        If (_pressedMouseButton = System.Windows.Forms.MouseButtons.Left) Then
            If (TypeOf e.Cell.Tag Is Change_description) Then
                Dim changeDescription As Change_description = DirectCast(e.Cell.Tag, Change_description)
                With changeDescription
                    Select Case e.Cell.Column.Key
                        Case ChangeDescriptionPropertyName.Changed_elements.ToString
                            Dim changedElements As New List(Of Tuple(Of String, String))

                            For Each changedElementId As String In .Changed_elements.SplitSpace
                                changedElements.Add(New Tuple(Of String, String)(changedElementId, Change_description.GetChangedElement(changedElementId, _kblMapper)))
                            Next

                            ShowDetailInformationForm(InformationHubStrings.ChangedElements_Caption, changedElements, Nothing, _kblMapper)
                        Case ChangeDescriptionPropertyName.External_references.ToString
                            Dim externalReferences As New List(Of External_reference)

                            For Each externalReference As String In .External_references.SplitSpace
                                externalReferences.Add(DirectCast(_kblMapper.KBLOccurrenceMapper(externalReference), External_reference))
                            Next

                            ShowDetailInformationForm(InformationHubStrings.ExtReferences_Caption, externalReferences, Nothing, _kblMapper)
                        Case ChangeDescriptionPropertyName.Related_changes.ToString
                            Dim changes As New List(Of Change)

                            For Each changeId As String In .Related_changes.SplitSpace
                                If (_kblMapper.KBLChangeMapper.ContainsKey(changeId)) Then
                                    changes.Add(_kblMapper.KBLChangeMapper(changeId))
                                End If
                            Next

                            ShowDetailInformationForm(PartPropertyName.Change.ToString, changes, Nothing, _kblMapper)
                    End Select
                End With
            ElseIf (TypeOf e.Cell.Tag Is KeyValuePair(Of String, Object)) Then
                RequestDialogCompareData(True, DirectCast(e.Cell.Row.Tag, KeyValuePair(Of String, Object)).Key.SplitRemoveEmpty("|"c)(1), DirectCast(e.Cell.Tag, KeyValuePair(Of String, Object)))
            Else
                Dim prevIds As IENumerable(Of String) = _informationHubEventArgs.ObjectIds
                SetInformationHubEventArgs(Nothing, e.Cell.Row, Nothing)
                If (e.Cell.Tag IsNot Nothing) Then
                    _informationHubEventArgs.ObjectIds.Add(e.Cell.Tag.ToString)
                End If
                OnHubSelectionChanged()
                If (Not Me.Focused) Then Me.Focus()
            End If
        End If
    End Sub

    Private Sub ugChangeDescriptions_DoubleClickRow(sender As Object, e As DoubleClickRowEventArgs) Handles ugChangeDescriptions.DoubleClickRow
        OnDoubleClickRow(sender, e)
    End Sub

    Private Sub ugChangeDescriptions_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugChangeDescriptions.InitializeLayout
        Me.ugChangeDescriptions.BeginUpdate()
        Me.ugChangeDescriptions.EventManager.AllEventsEnabled = False

        InitializeGridLayout(_changeDescriptionGridAppearance, e.Layout)

        Me.ugChangeDescriptions.EventManager.AllEventsEnabled = True
        Me.ugChangeDescriptions.EndUpdate()
    End Sub

    Private Sub InitializeChangeDescriptionRowCore(row As UltraGridRow)
        If (DirectCast(row.ListObject, UltraDataRow).Band.Tag Is Nothing) Then
            row.Tag = DirectCast(DirectCast(row.ListObject, UltraDataRow).Tag, Change_description).SystemId

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
                    If (TypeOf compareObject.Value Is ChangeDescriptionChangedProperty) Then
                        For Each changedProperty As KeyValuePair(Of String, Object) In DirectCast(compareObject.Value, ChangeDescriptionChangedProperty).ChangedProperties
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
            ElseIf cell.Column.Key = ChangeDescriptionPropertyName.Changed_elements.ToString Then
                Dim change_element_Id As String = DirectCast(DirectCast(row.ListObject, UltraDataRow).Tag, Change_description).Changed_elements
                If (Not String.IsNullOrEmpty(change_element_Id)) Then
                    cell.Style = ColumnStyle.Button
                    cell.Tag = change_element_Id
                End If
            End If
        Next
    End Sub

    Private Sub ugChangeDescriptions_InitializeRow(sender As Object, e As InitializeRowEventArgs) Handles ugChangeDescriptions.InitializeRow
        If Not e.ReInitialize Then
            InitializeChangeDescriptionRowCore(e.Row)
        End If
    End Sub

    Private Sub ugChangeDescriptions_KeyDown(sender As Object, e As KeyEventArgs) Handles ugChangeDescriptions.KeyDown
        OnGridKeyDown(Me, New GridKeyDownEventArgs(e, DirectCast(sender, UltraGrid)))
        If Not e.Handled Then
            If (e.Control) AndAlso (e.KeyCode = Keys.A) Then
                SelectAllRowsOfActiveGrid()
            ElseIf (e.KeyCode = Keys.Escape) Then
                If (Me.ugChangeDescriptions.Selected.Rows.Count = 0) Then
                    ClearMarkedRowsInGrids()
                Else
                    ClearSelectedRowsInGrids()
                End If

                If (SetInformationHubEventArgs(Nothing, Nothing, Me.ugChangeDescriptions.Selected.Rows)) Then
                    OnHubSelectionChanged()
                End If

            ElseIf (e.KeyCode = Keys.Return) Then
                SetSelectedAsMarkedOriginRows(ugChangeDescriptions)
            End If
        End If
    End Sub

    Private Sub ugChangeDescriptions_MouseDown(sender As Object, e As MouseEventArgs) Handles ugChangeDescriptions.MouseDown
        If (_contextMenuGrid.IsOpen) Then _contextMenuGrid.ClosePopup()

        _pressedMouseButton = e.Button
    End Sub

    Private Sub ugChangeDescriptions_MouseLeave(sender As Object, e As EventArgs) Handles ugChangeDescriptions.MouseLeave
        _messageEventArgs.StatusMessage = String.Empty

        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub ugChangeDescriptions_MouseMove(sender As Object, e As MouseEventArgs) Handles ugChangeDescriptions.MouseMove
        _messageEventArgs.StatusMessage = String.Format(InformationHubStrings.RowCount_Label, Me.ugChangeDescriptions.Rows.Count, Me.ugChangeDescriptions.Rows.FilteredInNonGroupByRowCount, Me.ugChangeDescriptions.Rows.VisibleRowCount, Me.ugChangeDescriptions.Selected.Rows.Count)

        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub udsChangeDescriptions_CellDataUpdated(sender As Object, e As CellDataUpdatedEventArgs) Handles udsChangeDescriptions.CellDataUpdated
        RaiseEvent CellValueUpdated(sender, e)
    End Sub

    Private Sub ugChangeDescriptions_InitializePrintPreview(sender As Object, e As CancelablePrintPreviewEventArgs) Handles ugChangeDescriptions.InitializePrintPreview

    End Sub
End Class