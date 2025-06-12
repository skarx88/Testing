Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinGrid
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Partial Public Class InformationHub

    Private Sub InitializeModules()
        _moduleGridAppearance = GridAppearance.All.OfType(Of ModuleGridAppearance).Single
        AddNewRowFilters(ugModules)

        If (_comparisonMapperList Is Nothing) Then
            FillDataSource(Me.udsModules, _moduleGridAppearance, _kblMapper.GetModules.Count)
        Else
            FillDataSource(Me.udsModules, _moduleGridAppearance, 0, _comparisonMapperList(KblObjectType.Module))

            Me.udsModules.Band.Columns.Add(ModulePropertyName.Controlled_components)
        End If

        If (Me.udsModules.Band.Columns.Count = 0) OrElse (Me.udsModules.Rows.Count = 0) Then
            Me.utcInformationHub.Tabs(TabNames.tabModules.ToString()).Visible = False
        End If
    End Sub

    Private Sub udsModules_CellDataRequested(sender As Object, e As CellDataRequestedEventArgs) Handles udsModules.CellDataRequested
        e.CacheData = False

        If (e.Row.ParentRow Is Nothing) Then
            With DirectCast(e.Row.Tag, [Module])
                Dim fromReference As Boolean = True

                If HasChangeTypeWithInverse(e.Row, CompareChangeType.New) Then
                    fromReference = False
                End If

                RequestCellPartData(e.Data, fromReference, e.Column.Key, DirectCast(e.Row.Tag, [Module]))

                Select Case e.Column.Key
                    Case ModulePropertyName.Project_number
                        If (.Project_number IsNot Nothing) Then
                            e.Data = .Project_number
                        End If
                    Case "ZGS"
                        If (.Change.Length <> 0) Then
                            Dim changes As List(Of Change) = .Change.Where(Function(change) change.Change_date IsNot Nothing AndAlso DateTime.TryParse(change.Change_date, New DateTime)).ToList
                            If (changes.Count <> 0) Then
                                e.Data = changes.Where(Function(change) CDate(change.Change_date).Ticks = changes.Max(Function(cng) CDate(cng.Change_date).Ticks)).LastOrDefault.Id
                            Else
                                e.Data = .Change.LastOrDefault.Id
                            End If
                        End If
                    Case "KEM"
                        If (.Change.Length <> 0) Then
                            Dim changes As List(Of Change) = .Change.Where(Function(change) change.Change_date IsNot Nothing AndAlso DateTime.TryParse(change.Change_date, New DateTime)).ToList
                            If (changes.Count <> 0) Then
                                e.Data = changes.Where(Function(change) CDate(change.Change_date).Ticks = changes.Max(Function(cng) CDate(cng.Change_date).Ticks)).LastOrDefault.Change_request
                            Else
                                e.Data = .Change.LastOrDefault.Change_request
                            End If
                        End If
                    Case ModulePropertyName.Car_classification_level_2
                        e.Data = .Car_classification_level_2
                    Case ModulePropertyName.Car_classification_level_3
                        If (.Car_classification_level_3 IsNot Nothing) Then
                            e.Data = .Car_classification_level_3
                        End If
                    Case ModulePropertyName.Car_classification_level_4
                        If (.Car_classification_level_4 IsNot Nothing) Then
                            e.Data = .Car_classification_level_4
                        End If
                    Case ModulePropertyName.Model_year
                        e.Data = .Model_year
                    Case ModulePropertyName.Content
                        e.Data = .Content
                    Case ModulePropertyName.Of_family
                        If (.Of_family IsNot Nothing) Then
                            If (fromReference) AndAlso (_kblMapper.KBLOccurrenceMapper.ContainsKey(.Of_family)) AndAlso (TypeOf _kblMapper.KBLOccurrenceMapper(.Of_family) Is Module_family) Then
                                e.Data = DirectCast(_kblMapper.KBLOccurrenceMapper(.Of_family), Module_family).Id
                            ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLOccurrenceMapper.ContainsKey(.Of_family)) Then
                                e.Data = DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(.Of_family), Module_family).Id
                            End If
                        End If
                    Case ModulePropertyName.Logistic_control_information
                        e.Data = .Module_configuration.Logistic_control_information
                    Case ModulePropertyName.Configuration_type
                        If .Module_configuration.Configuration_typeSpecified Then
                            e.Data = .Module_configuration.Configuration_type
                        End If
                    Case ModulePropertyName.Controlled_components
                        If (.Module_configuration IsNot Nothing) AndAlso (.Module_configuration.Controlled_components <> String.Empty) Then
                            e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                        End If
                    Case Else
                        OnUnhandledCellDataRequested(e)
                End Select
            End With
        Else
            With DirectCast(e.Row.Tag, Change)
                Select Case e.Column.Key
                    Case SYSTEM_ID_COLUMN_KEY
                        e.Data = .SystemId
                    Case ChangePropertyName.Id
                        If (.Id IsNot Nothing) Then
                            e.Data = .Id
                        End If
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
                    Case Else
                        OnUnhandledCellDataRequested(e)
                End Select
            End With
        End If
    End Sub

    Private Sub udsModules_InitializeDataRow(sender As Object, e As InitializeDataRowEventArgs) Handles udsModules.InitializeDataRow
        If (e.Row.ParentRow Is Nothing) Then
            Dim [module] As [Module]
            Dim rowCount As Integer

            If (e.Row.Band.Tag Is Nothing) Then
                [module] = _kblMapper.GetModules(e.Row.Index)
                rowCount = [module].Change.Length
            Else
                Dim compareObjKey As String = DirectCast(e.Row.Band.Tag, Dictionary(Of String, Object)).Keys.ElementAt(e.Row.Index)
                Dim compareObjValue As Object = DirectCast(e.Row.Band.Tag, Dictionary(Of String, Object)).Values.ElementAt(e.Row.Index)

                If (TypeOf compareObjValue Is [Module]) Then
                    [module] = DirectCast(compareObjValue, [Module])
                Else
                    [module] = DirectCast(_kblMapper.KBLOccurrenceMapper(ExtractSystemId(compareObjKey)), [Module])
                End If

                Dim compareObjects As New Dictionary(Of String, Object)

                If (TypeOf compareObjValue Is ChangedProperty) Then
                    Dim changeProperty As ChangedProperty = DirectCast(compareObjValue, PartChangedProperty)
                    If (changeProperty.ChangedProperties.ContainsKey(PartPropertyName.Change)) Then
                        Dim comparisonMapper As CommonComparisonMapper = DirectCast(changeProperty.ChangedProperties(PartPropertyName.Change), CommonComparisonMapper)

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

            e.Row.Tag = [module]
            If e.Row.Band.ChildBands.Count > 0 Then
                e.Row.GetChildRows(0).SetCount(rowCount)
            End If
        Else
            If (e.Row.ParentCollection.Tag Is Nothing) Then
                e.Row.Tag = DirectCast(e.Row.ParentRow.Tag, [Module]).Change(e.Row.Index)
            Else
                Dim compareObjKey As String = DirectCast(e.Row.ParentCollection.Tag, Dictionary(Of String, Object)).Keys.ElementAt(e.Row.Index)
                Dim compareObjValue As Object = DirectCast(e.Row.ParentCollection.Tag, Dictionary(Of String, Object)).Values.ElementAt(e.Row.Index)

                If (TypeOf compareObjValue Is Change) Then
                    e.Row.Tag = DirectCast(compareObjValue, Change)
                Else
                    For Each change As Change In DirectCast(e.Row.ParentRow.Tag, [Module]).Change
                        If (change.Id = ExtractSystemId(compareObjKey)) Then
                            e.Row.Tag = change

                            Exit For
                        End If
                    Next
                End If

                SetDiffCellValueFromObjectKey(e.Row, compareObjKey)
            End If
        End If
    End Sub

    Private Sub ugModules_AfterRowActivate(sender As Object, e As EventArgs) Handles ugModules.AfterRowActivate
        If (Me.ugModules.ActiveRow.Appearance.BackColor = Color.FromArgb(190, 190, 190)) Then
            Me.ugModules.ActiveRow = Nothing
        ElseIf (_kblMapperForCompare IsNot Nothing) Then
            If (SetInformationHubEventArgs(Nothing, Me.ugModules.ActiveRow, Nothing)) Then
                OnHubSelectionChanged()
            End If

            If (Not Me.Focused) Then
                Me.Focus()
            End If
        End If
    End Sub

    Private Sub ugModules_AfterSelectChange(sender As Object, e As AfterSelectChangeEventArgs) Handles ugModules.AfterSelectChange
        With Me.ugModules
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

    Private Sub ugModules_BeforeSelectChange(sender As Object, e As BeforeSelectChangeEventArgs) Handles ugModules.BeforeSelectChange
        ClearSelectedChildRowsInGrid(Me.ugModules)
    End Sub

    Private Sub ugModules_ClickCell(sender As Object, e As ClickCellEventArgs) Handles ugModules.ClickCell
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

    Private Sub ugModules_ClickCellButton(sender As Object, e As CellEventArgs) Handles ugModules.ClickCellButton
        If (_pressedMouseButton = System.Windows.Forms.MouseButtons.Left) Then
            Dim fromReference As Boolean = True

            If HasChangeTypeWithInverse(CType(e.Cell.Row.ListObject, UltraDataRow), CompareChangeType.New) Then
                fromReference = False
            End If

            If (TypeOf e.Cell.Tag Is [Module]) Then
                If (e.Cell.Column.Key = ModulePropertyName.Controlled_components) Then
                    ShowDetailInformationForm(InformationHubStrings.ControlledComponents_Caption, DirectCast(e.Cell.Tag, [Module]).Module_configuration.Controlled_components.SplitSpace.ToList, Nothing, _kblMapper)
                ElseIf (fromReference) Then
                    RequestDialogPartData(_kblMapper, e.Cell.Column.Key, DirectCast(e.Cell.Tag, [Module]))
                ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) Then
                    RequestDialogPartData(_kblMapperForCompare, e.Cell.Column.Key, DirectCast(e.Cell.Tag, [Module]))
                End If
            ElseIf (TypeOf e.Cell.Tag Is KeyValuePair(Of String, Object)) Then
                RequestDialogCompareData(True, DirectCast(e.Cell.Row.Tag, KeyValuePair(Of String, Object)).Key.SplitRemoveEmpty("|"c)(1), DirectCast(e.Cell.Tag, KeyValuePair(Of String, Object)))
            End If
        End If
    End Sub

    Private Sub ugModules_DoubleClickRow(sender As Object, e As DoubleClickRowEventArgs) Handles ugModules.DoubleClickRow
        OnDoubleClickRow(sender, e)
    End Sub

    Private Sub ugModules_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugModules.InitializeLayout
        Me.ugModules.BeginUpdate()
        Me.ugModules.EventManager.AllEventsEnabled = False

        InitializeGridLayout(_moduleGridAppearance, e.Layout)

        If Me.ugModules.DisplayLayout.Bands.Count > 0 AndAlso Me.ugModules.DisplayLayout.Bands(0).Columns.Exists(PartPropertyName.Part_description.ToString) Then
            Me.ugModules.DisplayLayout.Bands(0).Columns(PartPropertyName.Part_description.ToString).Width = 400
        End If
        Me.ugModules.EventManager.AllEventsEnabled = True
        Me.ugModules.EndUpdate()
    End Sub

    Private Sub InitializeModuleRowCore(row As UltraGridRow)
        If (row.ParentRow Is Nothing) Then
            If (DirectCast(row.ListObject, UltraDataRow).Band.Tag Is Nothing) Then
                row.Tag = DirectCast(DirectCast(row.ListObject, UltraDataRow).Tag, [Module]).SystemId

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
                        If (TypeOf compareObject.Value Is PartChangedProperty) Then
                            For Each changedProperty As KeyValuePair(Of String, Object) In DirectCast(compareObject.Value, PartChangedProperty).ChangedProperties
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

            Me.ugModules.EventManager.AllEventsEnabled = False

            If row.ChildBands?.Count > 0 Then
                For Each childRow As UltraGridRow In row.ChildBands(0).Rows
                    childRow.Tag = childRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString
                    InitializeModuleRowCore(childRow)
                Next
            End If

            Me.ugModules.EventManager.AllEventsEnabled = True
        ElseIf (DirectCast(row.ListObject, UltraDataRow).ParentCollection.Tag IsNot Nothing) Then
            InitializeRowForCompare(DirectCast(DirectCast(row.ListObject, UltraDataRow).ParentCollection.Tag, Dictionary(Of String, Object)), row)
        ElseIf _redliningInformation IsNot Nothing AndAlso _redliningInformation.Redlinings.Where(Function(redlining) redlining.ObjectId = row.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString).Any() Then
            row.Cells(1).Appearance.Image = My.Resources.Redlining
        End If
    End Sub

    Private Sub ugModules_InitializeRow(sender As Object, e As InitializeRowEventArgs) Handles ugModules.InitializeRow
        If Not e.ReInitialize Then
            InitializeModuleRowCore(e.Row)
        End If
    End Sub

    Private Sub ugModules_KeyDown(sender As Object, e As KeyEventArgs) Handles ugModules.KeyDown
        OnGridKeyDown(Me, New GridKeyDownEventArgs(e, DirectCast(sender, UltraGrid)))
        If Not e.Handled Then
            If (e.Control) AndAlso (e.KeyCode = Keys.A) Then
                SelectAllRowsOfActiveGrid()
            ElseIf (e.KeyCode = Keys.Escape) Then
                If (Me.ugModules.Selected.Rows.Count = 0) Then
                    ClearMarkedRowsInGrids()
                Else
                    ClearSelectedRowsInGrids()
                End If

                If (SetInformationHubEventArgs(Nothing, Nothing, Me.ugModules.Selected.Rows)) Then
                    OnHubSelectionChanged()
                End If

            ElseIf (e.KeyCode = Keys.Return) Then
                SetSelectedAsMarkedOriginRows(ugModules)
            End If
        End If
    End Sub

    Private Sub ugModules_MouseDown(sender As Object, e As MouseEventArgs) Handles ugModules.MouseDown
        If (_contextMenuGrid.IsOpen) Then
            _contextMenuGrid.ClosePopup()
        End If
        _pressedMouseButton = e.Button
    End Sub

    Private Sub ugModules_MouseLeave(sender As Object, e As EventArgs) Handles ugModules.MouseLeave
        _messageEventArgs.StatusMessage = String.Empty
        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub ugModules_MouseMove(sender As Object, e As MouseEventArgs) Handles ugModules.MouseMove
        _messageEventArgs.StatusMessage = String.Format(InformationHubStrings.RowCount_Label, Me.ugModules.Rows.Count, Me.ugModules.Rows.FilteredInNonGroupByRowCount, Me.ugModules.Rows.VisibleRowCount, Me.ugModules.Selected.Rows.Count)
        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub udsModules_CellDataUpdated(sender As Object, e As CellDataUpdatedEventArgs) Handles udsModules.CellDataUpdated
        RaiseEvent CellValueUpdated(sender, e)
    End Sub

End Class