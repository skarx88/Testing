Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinGrid
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Partial Public Class InformationHub

    Private Sub InitializeVertices()
        _vertexGridAppearance = GridAppearance.All.OfType(Of VertexGridAppearance).Single
        AddNewRowFilters(ugVertices)

        If (_comparisonMapperList Is Nothing) Then
            FillDataSource(Me.udsVertices, _vertexGridAppearance, _kblMapper.GetNodes.Count)
        Else
            FillDataSource(Me.udsVertices, _vertexGridAppearance, 0, _comparisonMapperList(KblObjectType.Node))
        End If

        If (Me.udsVertices.Band.Columns.Count = 0) OrElse (Me.udsVertices.Rows.Count = 0) Then
            Me.utcInformationHub.Tabs(TabNames.tabVertices.ToString()).Visible = False
        End If
    End Sub

    Private Sub udsVertices_CellDataRequested(sender As Object, e As CellDataRequestedEventArgs) Handles udsVertices.CellDataRequested
        e.CacheData = False

        If e.Row.ParentRow Is Nothing Then
            With DirectCast(e.Row.Tag, Node)
                Dim fromReference As Boolean = True

                If HasChangeTypeWithInverse(e.Row, CompareChangeType.New) Then
                    fromReference = False
                End If

                Select Case e.Column.Key
                    Case VertexPropertyName.Id
                        e.Data = .Id
                    Case VertexPropertyName.Alias_id
                        If .Alias_id.Length > 0 Then
                            If (.Alias_id.Length = 1) AndAlso (.Alias_id(0).Description Is Nothing) AndAlso (.Alias_id(0).Scope Is Nothing) Then
                                e.Data = .Alias_id(0).Alias_id
                            Else
                                e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                            End If
                        End If
                    Case VertexPropertyName.Cartesian_pointX
                        If .Cartesian_point IsNot Nothing Then
                            If (fromReference) AndAlso (_kblMapper.KBLOccurrenceMapper.ContainsKey(.Cartesian_point)) Then
                                e.Data = Math.Round(DirectCast(_kblMapper.KBLOccurrenceMapper(.Cartesian_point), Cartesian_point).Coordinates(0), 1)
                            ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLOccurrenceMapper.ContainsKey(.Cartesian_point)) Then
                                e.Data = Math.Round(DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(.Cartesian_point), Cartesian_point).Coordinates(0), 1)
                            End If
                        End If
                    Case VertexPropertyName.Cartesian_pointY
                        If .Cartesian_point IsNot Nothing Then
                            If (fromReference) AndAlso (_kblMapper.KBLOccurrenceMapper.ContainsKey(.Cartesian_point)) Then
                                e.Data = Math.Round(DirectCast(_kblMapper.KBLOccurrenceMapper(.Cartesian_point), Cartesian_point).Coordinates(1), 1)
                            ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLOccurrenceMapper.ContainsKey(.Cartesian_point)) Then
                                e.Data = Math.Round(DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(.Cartesian_point), Cartesian_point).Coordinates(1), 1)
                            End If
                        End If
                    Case VertexPropertyName.Cartesian_pointZ
                        If .Cartesian_point IsNot Nothing Then
                            If (fromReference) AndAlso (_kblMapper.KBLOccurrenceMapper.ContainsKey(.Cartesian_point)) Then
                                If (DirectCast(_kblMapper.KBLOccurrenceMapper(.Cartesian_point), Cartesian_point).Coordinates.Length > 2) Then
                                    e.Data = Math.Round(DirectCast(_kblMapper.KBLOccurrenceMapper(.Cartesian_point), Cartesian_point).Coordinates(2), 1)
                                End If
                            ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLOccurrenceMapper.ContainsKey(.Cartesian_point)) Then
                                If (DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(.Cartesian_point.ToString), Cartesian_point).Coordinates.Length > 2) Then
                                    e.Data = Math.Round(DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(.Cartesian_point.ToString), Cartesian_point).Coordinates(2), 1)
                                End If
                            End If
                        End If
                    Case VertexPropertyName.Folding_direction
                        If .Folding_direction IsNot Nothing Then
                            If (fromReference) AndAlso (_kblMapper.KBLOccurrenceMapper.ContainsKey(.Folding_direction)) Then
                                e.Data = DirectCast(_kblMapper.KBLOccurrenceMapper(.Folding_direction), Segment).Id
                            ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLOccurrenceMapper.ContainsKey(.Folding_direction)) Then
                                e.Data = DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(.Folding_direction), Segment).Id
                            End If
                        End If
                    Case VertexPropertyName.Referenced_components
                        If .Referenced_components IsNot Nothing Then
                            If (.Referenced_components.SplitSpace.Length = 1) Then
                                e.Data = Harness.GetReferenceElement(.Referenced_components, If(fromReference, _kblMapper, _kblMapperForCompare))
                            ElseIf (.Referenced_components.SplitSpace.Length > 1) Then
                                e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                            End If
                        End If
                    Case VertexPropertyName.Referenced_cavities
                        If .Referenced_cavities IsNot Nothing Then
                            If (.Referenced_cavities.SplitSpace.Length = 1) Then
                                e.Data = Harness.GetReferenceElement(.Referenced_cavities, If(fromReference, _kblMapper, _kblMapperForCompare))
                            ElseIf (.Referenced_cavities.SplitSpace.Length > 1) Then
                                e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                            End If
                        End If
                    Case VertexPropertyName.Processing_information
                        If .Processing_information.Length > 0 Then
                            e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                        End If
                    Case NameOf(InformationHubStrings.AssignedModules_ColumnCaption)
                        e.Data = GetAssignedModules(.SystemId)
                    Case Else
                        OnUnhandledCellDataRequested(e)
                End Select
            End With
        Else
            WireProtectionCellDataRequested(e)
        End If
    End Sub

    Protected Overridable Sub OnUnhandledCellDataRequested(e As CellDataRequestedEventArgs)
        RaiseEvent UnhandledCellDataRequested(Me, e)
    End Sub

    Private Sub udsVertices_InitializeDataRow(sender As Object, e As InitializeDataRowEventArgs) Handles udsVertices.InitializeDataRow
        If (e.Row.ParentRow Is Nothing) Then
            Dim vertex As Node = Nothing
            Dim rowCount As Integer = 0

            If (e.Row.Band.Tag Is Nothing) Then
                vertex = _kblMapper.GetNodes(e.Row.Index)
                rowCount = If(vertex.Referenced_components IsNot Nothing, vertex.Referenced_components.SplitSpace.Where(Function(refComp) _kblMapper.KBLOccurrenceMapper.ContainsKey(refComp) AndAlso TypeOf _kblMapper.KBLOccurrenceMapper(refComp) Is Wire_protection_occurrence).Count, 0)
            Else
                Dim compareObjKey As String = DirectCast(e.Row.Band.Tag, Dictionary(Of String, Object)).Keys.ElementAt(e.Row.Index)
                Dim compareObjValue As Object = DirectCast(e.Row.Band.Tag, Dictionary(Of String, Object)).Values.ElementAt(e.Row.Index)

                If (TypeOf compareObjValue Is Node) Then
                    vertex = DirectCast(compareObjValue, Node)
                Else
                    vertex = DirectCast(_kblMapper.KBLOccurrenceMapper(ExtractSystemId(compareObjKey)), Node)
                End If

                Dim compareObjects As New Dictionary(Of String, Object)

                If (TypeOf compareObjValue Is VertexChangedProperty) Then
                    Dim changeProperty As ChangedProperty = DirectCast(compareObjValue, VertexChangedProperty)
                    'HINT it seems, that the following code is not used respectively working at all (even nit in the previous version).
                    'The key is unclear and never written on VertexChangedProperties
                    If (changeProperty.ChangedProperties.ContainsKey(NameOf([Lib].Schema.Kbl.Wire_protection))) Then '"WireProtections"
                        Dim comparisonMapper As WireProtectionComparisonMapper = DirectCast(changeProperty.ChangedProperties(NameOf([Lib].Schema.Kbl.Wire_protection)), WireProtectionComparisonMapper)

                        For Each cItem As ChangedItem In comparisonMapper.Changes
                            compareObjects.Add(String.Format("{0}|{1}", cItem.Text, cItem.Key), cItem.Item)
                        Next
                    End If
                End If

                rowCount = compareObjects.Count

                If e.Row.Band.ChildBands.Count > 0 Then
                    e.Row.GetChildRows(0).Tag = compareObjects
                End If

                SetDiffTypeCellText(e.Row, ChangedItem.ParseFromText(ExtractChangeText(compareObjKey)))
            End If

            e.Row.Tag = vertex
            If e.Row.Band.ChildBands.Count > 0 Then
                e.Row.GetChildRows(0).SetCount(rowCount)
            End If
        Else
            If (e.Row.ParentCollection.Tag Is Nothing) Then
                e.Row.Tag = _kblMapper.KBLOccurrenceMapper(DirectCast(e.Row.ParentRow.Tag, Node).Referenced_components.SplitSpace.Where(Function(refComp) TypeOf _kblMapper.KBLOccurrenceMapper(refComp) Is Wire_protection_occurrence)(e.Row.Index))
            Else
                Dim compareObjKey As String = DirectCast(e.Row.ParentCollection.Tag, Dictionary(Of String, Object)).Keys.ElementAt(e.Row.Index)
                Dim compareObjValue As Object = DirectCast(e.Row.ParentCollection.Tag, Dictionary(Of String, Object)).Values.ElementAt(e.Row.Index)
                Dim wireProtection As Wire_protection_occurrence = Nothing

                If (TypeOf compareObjValue Is Wire_protection_occurrence) Then
                    wireProtection = DirectCast(compareObjValue, Wire_protection_occurrence)
                Else
                    wireProtection = DirectCast(_kblMapper.KBLOccurrenceMapper(ExtractSystemId(compareObjKey)), Wire_protection_occurrence)
                End If

                SetDiffTypeCellText(e.Row, ChangedItem.ParseFromText(ExtractChangeText(compareObjKey)))
                e.Row.Tag = wireProtection
            End If
        End If
    End Sub

    Private Sub SetDiffTypeCellText(row As UltraDataRow, type As CompareChangeType)
        Select Case type
            Case CompareChangeType.New
                row.SetCellValue(NameOf(InformationHubStrings.DiffType_ColumnCaption), InformationHubStrings.Added)
            Case CompareChangeType.Modified
                row.SetCellValue(NameOf(InformationHubStrings.DiffType_ColumnCaption), InformationHubStrings.Modified)
            Case CompareChangeType.Deleted
                row.SetCellValue(NameOf(InformationHubStrings.DiffType_ColumnCaption), InformationHubStrings.Deleted)
        End Select
    End Sub

    Private Sub ugVertices_AfterRowActivate(sender As Object, e As EventArgs) Handles ugVertices.AfterRowActivate
        If (Me.ugVertices.ActiveRow.Appearance.BackColor = Color.FromArgb(190, 190, 190)) Then
            Me.ugVertices.ActiveRow = Nothing
        ElseIf (_kblMapperForCompare IsNot Nothing) Then
            If (SetInformationHubEventArgs(Nothing, Me.ugVertices.ActiveRow, Nothing)) Then
                OnHubSelectionChanged()
            End If
            If Not Me.Focused Then
                Me.Focus()
            End If
        End If
    End Sub

    Private Sub ugVertices_AfterSelectChange(sender As Object, e As AfterSelectChangeEventArgs) Handles ugVertices.AfterSelectChange
        With Me.ugVertices
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

    Private Sub ugVertices_BeforeSelectChange(sender As Object, e As BeforeSelectChangeEventArgs) Handles ugVertices.BeforeSelectChange
        For Each row As UltraGridRow In e.NewSelections.Rows
            If (row.Appearance.BackColor = Color.FromArgb(190, 190, 190)) Then
                e.Cancel = True
            End If
        Next
    End Sub

    Private Sub ugVertices_ClickCell(sender As Object, e As ClickCellEventArgs) Handles ugVertices.ClickCell
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

    Private Sub ugVertices_ClickCellButton(sender As Object, e As CellEventArgs) Handles ugVertices.ClickCellButton
        If (_pressedMouseButton = System.Windows.Forms.MouseButtons.Left) Then
            Dim fromReference As Boolean = True

            If HasChangeTypeWithInverse(CType(e.Cell.Row.ListObject, UltraDataRow), CompareChangeType.New) Then
                fromReference = False
            End If

            If (e.Cell.Row.ParentRow Is Nothing) AndAlso (TypeOf e.Cell.Tag Is Node) Then
                With DirectCast(e.Cell.Tag, Node)
                    Select Case e.Cell.Column.Key
                        Case VertexPropertyName.Alias_id
                            ShowDetailInformationForm(InformationHubStrings.AliasIds_Caption, .Alias_id, Nothing, _kblMapper)
                        Case VertexPropertyName.Referenced_components
                            Dim referencedComponents As New List(Of String)
                            For Each referencedComponent As String In .Referenced_components.SplitSpace
                                If (fromReference) AndAlso (_kblMapper.KBLOccurrenceMapper.ContainsKey(referencedComponent)) Then
                                    referencedComponents.Add(Harness.GetReferenceElement(referencedComponent, _kblMapper))
                                ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLOccurrenceMapper.ContainsKey(referencedComponent)) Then
                                    referencedComponents.Add(Harness.GetReferenceElement(referencedComponent, _kblMapperForCompare))
                                End If
                            Next

                            ShowDetailInformationForm(InformationHubStrings.RefComps_Caption, referencedComponents, Nothing, _kblMapper)
                        Case VertexPropertyName.Referenced_cavities
                            Dim referencedCavities As New List(Of String)
                            For Each referencedCavityId As String In .Referenced_cavities.SplitSpace
                                If (fromReference) AndAlso (_kblMapper.KBLOccurrenceMapper.ContainsKey(referencedCavityId)) Then
                                    referencedCavities.Add(Harness.GetReferenceElement(referencedCavityId, _kblMapper))
                                ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLOccurrenceMapper.ContainsKey(referencedCavityId)) Then
                                    referencedCavities.Add(Harness.GetReferenceElement(referencedCavityId, _kblMapperForCompare))
                                End If
                            Next

                            ShowDetailInformationForm(InformationHubStrings.RefCavs_Caption, referencedCavities, Nothing, _kblMapper)
                        Case VertexPropertyName.Processing_information
                            ShowDetailInformationForm(InformationHubStrings.ProcInfo_Caption, .Processing_Information, Nothing, _kblMapper)
                    End Select
                End With
            ElseIf (TypeOf e.Cell.Tag Is Wire_protection_occurrence) Then
                Dim wireProtectionOccurrence As Wire_protection_occurrence = DirectCast(e.Cell.Tag, Wire_protection_occurrence)
                If (fromReference) AndAlso (_kblMapper.KblPartMapper.ContainsKey(wireProtectionOccurrence.Part)) Then
                    RequestDialogPartData(_kblMapper, e.Cell.Column.Key, DirectCast(_kblMapper.KblPartMapper(wireProtectionOccurrence.Part), Wire_protection))
                ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KblPartMapper.ContainsKey(wireProtectionOccurrence.Part)) Then
                    RequestDialogPartData(_kblMapperForCompare, e.Cell.Column.Key, DirectCast(_kblMapperForCompare.KblPartMapper(wireProtectionOccurrence.Part), Wire_protection))
                End If

                With wireProtectionOccurrence
                    Select Case e.Cell.Column.Key
                        Case WireProtectionPropertyName.Alias_id
                            ShowDetailInformationForm(InformationHubStrings.AliasIds_Caption, .Alias_id, Nothing, _kblMapper)
                        Case WireProtectionPropertyName.Installation_Information
                            ShowDetailInformationForm(InformationHubStrings.InstallInfo_Caption, .Installation_Information, Nothing, _kblMapper)
                    End Select
                End With
            ElseIf (TypeOf e.Cell.Tag Is KeyValuePair(Of String, Object)) Then
                RequestDialogCompareData(True, DirectCast(e.Cell.Row.Tag, KeyValuePair(Of String, Object)).Key.Split("|"c, StringSplitOptions.RemoveEmptyEntries)(1), DirectCast(e.Cell.Tag, KeyValuePair(Of String, Object)))
            End If
        End If
    End Sub

    Private Sub ugVertices_DoubleClickRow(sender As Object, e As DoubleClickRowEventArgs) Handles ugVertices.DoubleClickRow
        OnDoubleClickRow(sender, e)
    End Sub

    Private Sub ugVertices_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugVertices.InitializeLayout
        Me.ugVertices.BeginUpdate()
        Me.ugVertices.EventManager.AllEventsEnabled = False

        InitializeGridLayout(_vertexGridAppearance, e.Layout)

        Me.ugVertices.EventManager.AllEventsEnabled = True
        Me.ugVertices.EndUpdate()
    End Sub

    Private Sub InitializeVertexRowCore(row As UltraGridRow)
        If (row.ParentRow Is Nothing) Then
            If (DirectCast(row.ListObject, UltraDataRow).Band.Tag Is Nothing) Then
                row.Tag = DirectCast(DirectCast(row.ListObject, UltraDataRow).Tag, Node).SystemId

                If (_redliningInformation IsNot Nothing) AndAlso (_redliningInformation.Redlinings.Where(Function(redlining) redlining.ObjectId = row.Tag?.ToString).Any()) Then
                    row.Cells(0).Appearance.Image = My.Resources.Redlining
                End If
            Else
                InitializeRowForCompare(DirectCast(DirectCast(row.ListObject, UltraDataRow).Band.Tag, Dictionary(Of String, Object)), row)
            End If

            For Each cell As UltraGridCell In row.Cells
                If (cell.Value IsNot Nothing AndAlso cell.Value.ToString = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS) Then
                    cell.Style = ColumnStyle.Button

                    If TypeOf row.Tag Is KeyValuePair(Of String, Object) Then
                        Dim compareObject As KeyValuePair(Of String, Object) = DirectCast(row.Tag, KeyValuePair(Of String, Object))

                        If (TypeOf compareObject.Value Is VertexChangedProperty) Then
                            For Each changedProperty As KeyValuePair(Of String, Object) In DirectCast(compareObject.Value, VertexChangedProperty).ChangedProperties
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

            Me.ugVertices.EventManager.AllEventsEnabled = False

            If (row?.ChildBands?.Count).GetValueOrDefault > 0 Then
                For Each childRow As UltraGridRow In row.ChildBands(0).Rows
                    If (_kblMapper.KBLOccurrenceMapper.ContainsKey(childRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString)) Then
                        childRow.Tag = DirectCast(_kblMapper.KBLOccurrenceMapper(childRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString), Wire_protection_occurrence).SystemId
                    ElseIf (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLOccurrenceMapper.ContainsKey(childRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString)) Then
                        childRow.Tag = DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(childRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString), Wire_protection_occurrence).SystemId
                    End If

                    InitializeVertexRowCore(childRow)
                Next
            End If
            Me.ugVertices.EventManager.AllEventsEnabled = True
        Else
            If (DirectCast(row.ListObject, UltraDataRow).ParentCollection.Tag IsNot Nothing) Then
                InitializeRowForCompare(DirectCast(DirectCast(row.ListObject, UltraDataRow).ParentCollection.Tag, Dictionary(Of String, Object)), row)
            Else
                If (_redliningInformation IsNot Nothing) AndAlso (_redliningInformation.Redlinings.Where(Function(redlining) redlining.ObjectId = row.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString).Any()) Then
                    row.Cells(1).Appearance.Image = My.Resources.Redlining
                End If
            End If

            For Each cell As UltraGridCell In row.Cells
                If (cell.Value IsNot Nothing AndAlso cell.Value.ToString = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS) Then
                    cell.Style = ColumnStyle.Button

                    If (TypeOf row.Tag Is KeyValuePair(Of String, Object)) Then
                        Dim compareObject As KeyValuePair(Of String, Object) = DirectCast(row.Tag, KeyValuePair(Of String, Object))

                        If (TypeOf compareObject.Value Is WireProtectionOccurrenceChangedProperty) Then
                            For Each changedProperty As KeyValuePair(Of String, Object) In DirectCast(compareObject.Value, WireProtectionOccurrenceChangedProperty).ChangedProperties
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
                    ElseIf (row.Tag IsNot Nothing) Then
                        cell.Tag = _kblMapper.KBLOccurrenceMapper(row.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString)
                    End If
                End If
            Next
        End If
    End Sub

    Private Sub ugVertices_InitializeRow(sender As Object, e As InitializeRowEventArgs) Handles ugVertices.InitializeRow
        If Not e.ReInitialize Then
            InitializeVertexRowCore(e.Row)
        End If
    End Sub

    Private Sub ugVertices_KeyDown(sender As Object, e As KeyEventArgs) Handles ugVertices.KeyDown
        OnGridKeyDown(Me, New GridKeyDownEventArgs(e, DirectCast(sender, UltraGrid)))
        If Not e.Handled Then
            If (e.Control) AndAlso (e.KeyCode = Keys.A) Then
                SelectAllRowsOfActiveGrid()
            ElseIf (e.KeyCode = Keys.Escape) Then
                If (Me.ugVertices.Selected.Rows.Count = 0) Then
                    ClearMarkedRowsInGrids()
                Else
                    ClearSelectedRowsInGrids()
                End If

                If (SetInformationHubEventArgs(Nothing, Nothing, Me.ugVertices.Selected.Rows)) Then
                    OnHubSelectionChanged()
                End If

            ElseIf (e.KeyCode = Keys.Return) Then
                SetSelectedAsMarkedOriginRows(ugVertices)
            End If
        End If
    End Sub

    Private Sub ugVertices_MouseDown(sender As Object, e As MouseEventArgs) Handles ugVertices.MouseDown
        If (_contextMenuGrid.IsOpen) Then
            _contextMenuGrid.ClosePopup()
        End If
        _pressedMouseButton = e.Button
    End Sub

    Private Sub ugVertices_MouseLeave(sender As Object, e As EventArgs) Handles ugVertices.MouseLeave
        _messageEventArgs.StatusMessage = String.Empty
        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub ugVertices_MouseMove(sender As Object, e As MouseEventArgs) Handles ugVertices.MouseMove
        _messageEventArgs.StatusMessage = String.Format(InformationHubStrings.RowCount_Label, Me.ugVertices.Rows.Count, Me.ugVertices.Rows.FilteredInNonGroupByRowCount, Me.ugVertices.Rows.VisibleRowCount, Me.ugVertices.Selected.Rows.Count)
        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub udsVertices_CellDataUpdated(sender As Object, e As CellDataUpdatedEventArgs) Handles udsVertices.CellDataUpdated
        RaiseEvent CellValueUpdated(sender, e)
    End Sub

End Class