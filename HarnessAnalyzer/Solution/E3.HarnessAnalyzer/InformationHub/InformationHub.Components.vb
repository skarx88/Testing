Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinGrid
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Partial Public Class InformationHub

    Private Sub InitializeComponents()
        _componentGridAppearance = GridAppearance.All.OfType(Of ComponentGridAppearance).Single
        AddNewRowFilters(ugComponents)

        If (_comparisonMapperList Is Nothing) Then
            FillDataSource(Me.udsComponents, _componentGridAppearance, _kblMapper.GetComponentOccurrences.Count)
        Else
            FillDataSource(Me.udsComponents, _componentGridAppearance, 0, _comparisonMapperList(KblObjectType.Component_occurrence))
        End If

        If (Me.udsComponents.Band.Columns.Count = 0) OrElse (Me.udsComponents.Rows.Count = 0) Then
            Me.utcInformationHub.Tabs(TabNames.tabComponents.ToString()).Visible = False
        End If
    End Sub

    Private Sub udsComponents_CellDataRequested(sender As Object, e As CellDataRequestedEventArgs) Handles udsComponents.CellDataRequested
        e.CacheData = False

        With DirectCast(e.Row.Tag, Component_occurrence)
            Dim component As Component = Nothing
            Dim fromReference As Boolean = True

            If HasChangeTypeWithInverse(e.Row, CompareChangeType.New) Then
                fromReference = False
                If (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLPartMapper.ContainsKey(If(.Part Is Nothing, String.Empty, .Part))) Then
                    component = TryCast(_kblMapperForCompare.KBLPartMapper(.Part), Component)
                End If
            Else
                If (_kblMapper.KBLOccurrenceMapper.ContainsKey(.SystemId)) AndAlso (_kblMapper.KBLPartMapper.ContainsKey(If(.Part Is Nothing, String.Empty, .Part))) Then
                    component = TryCast(_kblMapper.KBLPartMapper(.Part), Component)
                End If
            End If

            If (component IsNot Nothing) Then
                RequestCellPartData(e.Data, fromReference, e.Column.Key, component)
            End If

            Select Case e.Column.Key
                Case COMPONENT_CLASS_COLUMN_KEY
                    e.Data = If(TypeOf e.Row.Tag Is Fuse_occurrence, KblObjectType.Fuse, KblObjectType.Component)
                Case ComponentPropertyName.Id
                    e.Data = .Id
                Case ComponentPropertyName.Alias_id
                    If (.Alias_id.Length <> 0) Then
                        If (.Alias_id.Length = 1) AndAlso (.Alias_id(0).Description Is Nothing) AndAlso (.Alias_id(0).Scope Is Nothing) Then
                            e.Data = .Alias_id(0).Alias_id
                        Else
                            e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                        End If
                    End If
                Case ComponentPropertyName.Localized_Description
                    If (.Localized_description.Length > 0) Then
                        If (.Localized_description.Length = 1) Then
                            e.Data = .Localized_description(0).Value
                        Else
                            e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                        End If
                    End If
                Case ComponentPropertyName.Description
                    If (.Description IsNot Nothing) Then
                        e.Data = .Description
                    End If
                Case ComponentPropertyName.Mounting
                    If (.Mounting IsNot Nothing) AndAlso (.Mounting <> String.Empty) Then
                        If (.Mounting.SplitSpace.Length = 1) Then
                            If (fromReference) AndAlso (_kblMapper.KBLOccurrenceMapper.ContainsKey(.Mounting) AndAlso TypeOf _kblMapper.KBLOccurrenceMapper(.Mounting) Is Cavity_occurrence) Then
                                Dim connectorOccurrence As Connector_occurrence = DirectCast(_kblMapper.KBLOccurrenceMapper(_kblMapper.KBLCavityConnectorMapper(.Mounting)), Connector_occurrence)
                                Dim cavityPart As Cavity = DirectCast(_kblMapper.KBLPartMapper(DirectCast(_kblMapper.KBLOccurrenceMapper(.Mounting), Cavity_occurrence).Part), Cavity)

                                e.Data = String.Format("{0},{1}", connectorOccurrence.Id, cavityPart.Cavity_number)
                            ElseIf (fromReference) AndAlso (_kblMapper.KBLOccurrenceMapper.ContainsKey(.Mounting) AndAlso TypeOf _kblMapper.KBLOccurrenceMapper(.Mounting) Is Connector_occurrence) Then
                                e.Data = DirectCast(_kblMapper.KBLOccurrenceMapper(.Mounting), Connector_occurrence).Id
                            ElseIf (fromReference) AndAlso (_kblMapper.KBLOccurrenceMapper.ContainsKey(.Mounting) AndAlso TypeOf _kblMapper.KBLOccurrenceMapper(.Mounting) Is Slot_occurrence) Then
                                Dim connectorOccurrence As Connector_occurrence = DirectCast(_kblMapper.KBLOccurrenceMapper(_kblMapper.KBLSlotConnectorMapper(.Mounting)), Connector_occurrence)

                                For Each cavityOccurrence As Cavity_occurrence In DirectCast(_kblMapper.KBLOccurrenceMapper(.Mounting), Slot_occurrence).Cavities
                                    e.Data = String.Format("{0},{1}", connectorOccurrence.Id, DirectCast(_kblMapper.KBLPartMapper(cavityOccurrence.Part), Cavity).Cavity_number)
                                    Exit For
                                Next
                            Else
                                e.Data = .Mounting
                            End If
                        Else
                            e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                        End If
                    End If
                Case ComponentPropertyName.ComponentPinMaps
                    If (.Component_pin_maps.Length <> 0) Then
                        e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
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

    Private Sub udsComponents_InitializeDataRow(sender As Object, e As InitializeDataRowEventArgs) Handles udsComponents.InitializeDataRow
        Dim componentOccurrence As Component_occurrence

        If (e.Row.Band.Tag Is Nothing) Then
            componentOccurrence = _kblMapper.GetComponentOccurrences(e.Row.Index)
        Else
            Dim compareObjKey As String = DirectCast(e.Row.Band.Tag, Dictionary(Of String, Object)).Keys.ElementAt(e.Row.Index)
            Dim compareObjValue As Object = DirectCast(e.Row.Band.Tag, Dictionary(Of String, Object)).Values.ElementAt(e.Row.Index)

            If (TypeOf compareObjValue Is Component_occurrence) Then
                componentOccurrence = DirectCast(compareObjValue, Component_occurrence)
            Else
                componentOccurrence = DirectCast(_kblMapper.KBLOccurrenceMapper(ExtractSystemId(compareObjKey)), Component_occurrence)
            End If

            SetDiffCellValueFromObjectKey(e.Row, compareObjKey)
        End If

        e.Row.Tag = componentOccurrence
    End Sub


    Private Sub ugComponents_AfterRowActivate(sender As Object, e As EventArgs) Handles ugComponents.AfterRowActivate
        If (Me.ugComponents.ActiveRow.Appearance.BackColor = Color.FromArgb(190, 190, 190)) Then
            Me.ugComponents.ActiveRow = Nothing
        ElseIf (_kblMapperForCompare IsNot Nothing) Then
            If (SetInformationHubEventArgs(Nothing, Me.ugComponents.ActiveRow, Nothing)) Then
                OnHubSelectionChanged()
            End If
            If (Not Me.Focused) Then Me.Focus()
        End If
    End Sub

    Private Sub ugComponents_AfterSelectChange(sender As Object, e As AfterSelectChangeEventArgs) Handles ugComponents.AfterSelectChange
        With Me.ugComponents
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

    Private Sub ugComponents_BeforeSelectChange(sender As Object, e As BeforeSelectChangeEventArgs) Handles ugComponents.BeforeSelectChange
        For Each row As UltraGridRow In e.NewSelections.Rows
            If (row.Appearance.BackColor = Color.FromArgb(190, 190, 190)) Then e.Cancel = True
        Next
    End Sub

    Private Sub ugComponents_ClickCell(sender As Object, e As ClickCellEventArgs) Handles ugComponents.ClickCell
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

    Private Sub ugComponents_ClickCellButton(sender As Object, e As CellEventArgs) Handles ugComponents.ClickCellButton
        If (_pressedMouseButton = System.Windows.Forms.MouseButtons.Left) Then
            Dim fromReference As Boolean = True

            If HasChangeTypeWithInverse(CType(e.Cell.Row.ListObject, UltraDataRow), CompareChangeType.New) Then
                fromReference = False
            End If

            If (TypeOf e.Cell.Tag Is Component_occurrence) Then
                Dim componentOccurrence As Component_occurrence = DirectCast(e.Cell.Tag, Component_occurrence)
                Dim componentPart As Component = Nothing

                If (fromReference) AndAlso (_kblMapper.KBLPartMapper.ContainsKey(componentOccurrence.Part)) Then
                    componentPart = DirectCast(_kblMapper.KBLPartMapper(componentOccurrence.Part), Component)

                    RequestDialogPartData(_kblMapper, e.Cell.Column.Key, componentPart)
                ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLPartMapper.ContainsKey(componentOccurrence.Part)) Then
                    componentPart = DirectCast(_kblMapperForCompare.KBLPartMapper(componentOccurrence.Part), Component)

                    RequestDialogPartData(_kblMapperForCompare, e.Cell.Column.Key, componentPart)
                End If

                With componentOccurrence
                    Select Case e.Cell.Column.Key
                        Case ComponentPropertyName.Alias_id.ToString
                            ShowDetailInformationForm(InformationHubStrings.AliasIds_Caption, .Alias_id, Nothing, _kblMapper)
                        Case ComponentPropertyName.Mounting.ToString
                            If (.Mounting IsNot Nothing) Then
                                Dim mountingObjects As New List(Of Tuple(Of String, String))

                                For Each mounting As String In .Mounting.SplitSpace
                                    If (fromReference) AndAlso (_kblMapper.KBLOccurrenceMapper.ContainsKey(mounting)) Then
                                        If (TypeOf _kblMapper.KBLOccurrenceMapper(mounting) Is Cavity_occurrence) Then
                                            Dim connectorOccurrence As Connector_occurrence = DirectCast(_kblMapper.KBLOccurrenceMapper(_kblMapper.KBLCavityConnectorMapper(mounting)), Connector_occurrence)
                                            Dim cavityPart As Cavity = DirectCast(_kblMapper.KBLPartMapper(DirectCast(_kblMapper.KBLOccurrenceMapper(mounting), Cavity_occurrence).Part), Cavity)

                                            mountingObjects.Add(New Tuple(Of String, String)(mounting, String.Format("[{0}] {1},{2}", KblObjectType.Cavity_occurrence.ToLocalizedString, connectorOccurrence.Id, cavityPart.Cavity_number)))
                                        ElseIf (TypeOf _kblMapper.KBLOccurrenceMapper(mounting) Is Connector_occurrence) Then
                                            mountingObjects.Add(New Tuple(Of String, String)(mounting, String.Format("[{0}] {1}", KblObjectType.Connector_occurrence.ToLocalizedString, DirectCast(_kblMapper.KBLOccurrenceMapper(mounting), Connector_occurrence).Id)))
                                        ElseIf (TypeOf _kblMapper.KBLOccurrenceMapper(mounting) Is Slot_occurrence) Then
                                            Dim connectorOccurrence As Connector_occurrence = DirectCast(_kblMapper.KBLOccurrenceMapper(_kblMapper.KBLSlotConnectorMapper(mounting)), Connector_occurrence)

                                            For Each cavityOccurrence As Cavity_occurrence In DirectCast(_kblMapper.KBLOccurrenceMapper(mounting), Slot_occurrence).Cavities
                                                mountingObjects.Add(New Tuple(Of String, String)(mounting, String.Format("[{0}] {1},{2}", KblObjectType.Slot, connectorOccurrence.Id, DirectCast(_kblMapper.KBLPartMapper(cavityOccurrence.Part), Cavity).Cavity_number)))
                                                Exit For
                                            Next
                                        End If
                                    ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLOccurrenceMapper.ContainsKey(mounting)) Then
                                        If (TypeOf _kblMapperForCompare.KBLOccurrenceMapper(mounting) Is Cavity_occurrence) Then
                                            Dim connectorOccurrence As Connector_occurrence = DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(_kblMapperForCompare.KBLCavityConnectorMapper(mounting)), Connector_occurrence)
                                            Dim cavityPart As Cavity = DirectCast(_kblMapperForCompare.KBLPartMapper(DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(mounting), Cavity_occurrence).Part), Cavity)

                                            mountingObjects.Add(New Tuple(Of String, String)(mounting, String.Format("[{0}] {1},{2}", KblObjectType.Cavity_occurrence.ToLocalizedString, connectorOccurrence.Id, cavityPart.Cavity_number)))
                                        ElseIf (TypeOf _kblMapperForCompare.KBLOccurrenceMapper(mounting) Is Connector_occurrence) Then
                                            mountingObjects.Add(New Tuple(Of String, String)(mounting, String.Format("[{0}] {1}", KblObjectType.Connector_occurrence.ToLocalizedString, DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(mounting), Connector_occurrence).Id)))
                                        ElseIf (TypeOf _kblMapperForCompare.KBLOccurrenceMapper(mounting) Is Slot_occurrence) Then
                                            Dim connectorOccurrence As Connector_occurrence = DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(_kblMapperForCompare.KBLSlotConnectorMapper(mounting)), Connector_occurrence)

                                            For Each cavityOccurrence As Cavity_occurrence In DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(mounting), Slot_occurrence).Cavities
                                                mountingObjects.Add(New Tuple(Of String, String)(mounting, String.Format("[{0}] {1},{2}", KblObjectType.Slot, connectorOccurrence.Id, DirectCast(_kblMapperForCompare.KBLPartMapper(cavityOccurrence.Part), Cavity).Cavity_number)))
                                                Exit For
                                            Next
                                        End If
                                    End If
                                Next

                                ShowDetailInformationForm(InformationHubStrings.MountingObjects_Caption, mountingObjects, Nothing, _kblMapper)
                            End If
                        Case ComponentPropertyName.ComponentPinMaps.ToString
                            ShowDetailInformationForm(ObjectPropertyNameStrings.ComponentPinMaps, .Component_pin_maps, Nothing, _kblMapper)
                        Case ComponentPropertyName.Installation_Information.ToString
                            ShowDetailInformationForm(InformationHubStrings.InstallInfo_Caption, .Installation_information, Nothing, _kblMapper)
                    End Select
                End With
            ElseIf (TypeOf e.Cell.Tag Is KeyValuePair(Of String, Object)) Then
                RequestDialogCompareData(True, DirectCast(e.Cell.Row.Tag, KeyValuePair(Of String, Object)).Key.SplitRemoveEmpty("|"c)(1), DirectCast(e.Cell.Tag, KeyValuePair(Of String, Object)))
            End If
        End If
    End Sub

    Private Sub ugComponents_DoubleClickRow(sender As Object, e As DoubleClickRowEventArgs) Handles ugComponents.DoubleClickRow
        OnDoubleClickRow(sender, e)
    End Sub

    Private Sub ugComponents_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugComponents.InitializeLayout
        Me.ugComponents.BeginUpdate()
        Me.ugComponents.EventManager.AllEventsEnabled = False

        InitializeGridLayout(_componentGridAppearance, e.Layout)

        If (_kblMapperForCompare Is Nothing) AndAlso (e.Layout.Bands(0).Columns.Exists(COMPONENT_CLASS_COLUMN_KEY)) Then
            e.Layout.Bands(0).Columns(COMPONENT_CLASS_COLUMN_KEY).Header.Caption = InformationHubStrings.Class_Caption
            e.Layout.Bands(0).Columns(COMPONENT_CLASS_COLUMN_KEY).Width = 80
        End If

        Me.ugComponents.EventManager.AllEventsEnabled = True
        Me.ugComponents.EndUpdate()
    End Sub

    Private Sub InitializeComponentRowCore(row As UltraGridRow)
        If (DirectCast(row.ListObject, UltraDataRow).Band.Tag Is Nothing) Then
            row.Tag = DirectCast(DirectCast(row.ListObject, UltraDataRow).Tag, Component_occurrence).SystemId

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

                    If (TypeOf compareObject.Value Is ComponentChangedProperty) Then
                        For Each changedProperty As KeyValuePair(Of String, Object) In DirectCast(compareObject.Value, ComponentChangedProperty).ChangedProperties
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

    Private Sub ugComponents_InitializeRow(sender As Object, e As InitializeRowEventArgs) Handles ugComponents.InitializeRow
        If Not e.ReInitialize Then
            InitializeComponentRowCore(e.Row)
        End If
    End Sub

    Private Sub ugComponents_KeyDown(sender As Object, e As KeyEventArgs) Handles ugComponents.KeyDown
        OnGridKeyDown(Me, New GridKeyDownEventArgs(e, DirectCast(sender, UltraGrid)))
        If Not e.Handled Then
            If (e.Control) AndAlso (e.KeyCode = Keys.A) Then
                SelectAllRowsOfActiveGrid()
            ElseIf (e.KeyCode = Keys.Escape) Then
                If (Me.ugComponents.Selected.Rows.Count = 0) Then
                    ClearMarkedRowsInGrids()
                Else
                    ClearSelectedRowsInGrids()
                End If

                If (SetInformationHubEventArgs(Nothing, Nothing, Me.ugComponents.Selected.Rows)) Then
                    OnHubSelectionChanged()
                End If

            ElseIf (e.KeyCode = Keys.Return) Then
                SetSelectedAsMarkedOriginRows(ugComponents)
            End If
        End If
    End Sub

    Private Sub ugComponents_MouseDown(sender As Object, e As MouseEventArgs) Handles ugComponents.MouseDown
        If (_contextMenuGrid.IsOpen) Then _contextMenuGrid.ClosePopup()

        _pressedMouseButton = e.Button
    End Sub

    Private Sub ugComponents_MouseLeave(sender As Object, e As EventArgs) Handles ugComponents.MouseLeave
        _messageEventArgs.StatusMessage = String.Empty

        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub ugComponents_MouseMove(sender As Object, e As MouseEventArgs) Handles ugComponents.MouseMove
        _messageEventArgs.StatusMessage = String.Format(InformationHubStrings.RowCount_Label, Me.ugComponents.Rows.Count, Me.ugComponents.Rows.FilteredInNonGroupByRowCount, Me.ugComponents.Rows.VisibleRowCount, Me.ugComponents.Selected.Rows.Count)
        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub udsComponents_CellDataUpdated(sender As Object, e As CellDataUpdatedEventArgs) Handles udsComponents.CellDataUpdated
        RaiseEvent CellValueUpdated(sender, e)
    End Sub

End Class