Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinGrid
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Partial Public Class InformationHub

    Private Sub InitializeNets()
        _netGridAppearance = GridAppearance.All.OfType(Of NetGridAppearance).Single

        If (_comparisonMapperList Is Nothing) Then
            FillDataSource(Me.udsNets, _netGridAppearance, _kblMapper.KBLNetMapper.Count)
        Else
            FillDataSource(Me.udsNets, _netGridAppearance, 0, _comparisonMapperList(KblObjectType.Net))
        End If

        If (Me.udsNets.Band.Columns.Count = 0) OrElse (Me.udsNets.Rows.Count = 0) Then
            Me.utcInformationHub.Tabs(TabNames.tabNets.ToString()).Visible = False

        End If
    End Sub

    Private Sub udsNets_CellDataRequested(sender As Object, e As CellDataRequestedEventArgs) Handles udsNets.CellDataRequested
        e.CacheData = False

        If (e.Row.ParentRow Is Nothing) Then
            Select Case e.Column.Key
                Case ConnectionPropertyName.Signal_name, ConnectionPropertyName.Signal_type
                    Select Case e.Column.Key
                        Case ConnectionPropertyName.Signal_name.ToString
                            e.Data = (e.Row.Tag?.ToString).OrEmpty.Split("|"c).FirstOrDefault
                        Case ConnectionPropertyName.Signal_type.ToString()
                            e.Data = (e.Row.Tag?.ToString).OrEmpty.Split("|"c).LastOrDefault
                    End Select
                Case Else
                    OnUnhandledCellDataRequested(e)
            End Select
        ElseIf e.Row.Tag IsNot Nothing Then
            With DirectCast(e.Row.Tag, Connection)
                Dim fromReference As Boolean = True

                If HasChangeTypeWithInverse(e.Row, CompareChangeType.New) Then
                    fromReference = False
                End If

                Select Case e.Column.Key
                    Case SYSTEM_ID_COLUMN_KEY
                        e.Data = .SystemId
                    Case ConnectionPropertyName.Id
                        If (.Id IsNot Nothing) Then
                            e.Data = .Id
                        End If
                    Case ConnectionPropertyName.Description
                        If (.Description IsNot Nothing) Then
                            e.Data = .Description
                        End If
                    Case ConnectionPropertyName.External_references
                        If (.External_references IsNot Nothing) AndAlso (.External_references.SplitSpace.Length <> 0) Then
                            e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                        End If
                    Case ConnectionPropertyName.Wire
                        If (fromReference) AndAlso (_kblMapper.KBLOccurrenceMapper.ContainsKey(.Wire) AndAlso TypeOf _kblMapper.KBLOccurrenceMapper(.Wire) Is Core_occurrence) Then
                            e.Data = String.Format("{0} [{1}]", _kblMapper.KBLCoreCableNameMapper(.Wire), DirectCast(_kblMapper.KBLOccurrenceMapper(.Wire), Core_occurrence).Wire_number)
                        ElseIf (fromReference) AndAlso (_kblMapper.KBLOccurrenceMapper.ContainsKey(.Wire) AndAlso TypeOf _kblMapper.KBLOccurrenceMapper(.Wire) Is Special_wire_occurrence) Then
                            e.Data = _kblMapper.KBLCoreCableNameMapper(.Wire)
                        ElseIf (fromReference) AndAlso (_kblMapper.KBLOccurrenceMapper.ContainsKey(.Wire) AndAlso TypeOf _kblMapper.KBLOccurrenceMapper(.Wire) Is Wire_occurrence) Then
                            e.Data = DirectCast(_kblMapper.KBLOccurrenceMapper(.Wire), Wire_occurrence).Wire_number
                        ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLOccurrenceMapper.ContainsKey(.Wire) AndAlso TypeOf _kblMapperForCompare.KBLOccurrenceMapper(.Wire) Is Core_occurrence) Then
                            e.Data = String.Format("{0} [{1}]", _kblMapperForCompare.KBLCoreCableNameMapper(.Wire), DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(.Wire), Core_occurrence).Wire_number)
                        ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLOccurrenceMapper.ContainsKey(.Wire) AndAlso TypeOf _kblMapperForCompare.KBLOccurrenceMapper(.Wire) Is Special_wire_occurrence) Then
                            e.Data = _kblMapperForCompare.KBLCoreCableNameMapper(.Wire)
                        ElseIf (Not fromReference) AndAlso (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLOccurrenceMapper.ContainsKey(.Wire) AndAlso TypeOf _kblMapperForCompare.KBLOccurrenceMapper(.Wire) Is Wire_occurrence) Then
                            e.Data = DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(.Wire), Wire_occurrence).Wire_number
                        Else
                            e.Data = .Wire
                        End If
                    Case ConnectionPropertyName.Installation_Information
                        If (.Installation_information.Length > 0) Then
                            e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                        End If
                    Case ConnectionPropertyName.Processing_Information
                        If (.Processing_information.Length > 0) Then
                            e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                        End If
                    Case ConnectionPropertyName.Nominal_voltage.ToString
                        If Not String.IsNullOrEmpty(.Nominal_voltage) Then
                            e.Data = .Nominal_voltage
                        End If
                    Case NameOf(InformationHubStrings.AssignedModules_ColumnCaption)
                        e.Data = GetAssignedModules(.SystemId)
                    Case Else
                        OnUnhandledCellDataRequested(e)
                End Select
            End With
        End If
    End Sub

    Private Sub udsNets_InitializeDataRow(sender As Object, e As InitializeDataRowEventArgs) Handles udsNets.InitializeDataRow
        If (e.Row.ParentRow Is Nothing) Then
            Dim netRowInfoContainer As New NetRowInfoContainer
            Dim rowCount As Integer = 0

            If (e.Row.Band.Tag Is Nothing) Then
                netRowInfoContainer.NetName = _kblMapper.KBLNetList(e.Row.Index)
                For Each connection As Connection In netRowInfoContainer.GetConnections(_kblMapper)
                    If (connection.Signal_type IsNot Nothing AndAlso connection.Signal_type <> String.Empty) Then
                        netRowInfoContainer.NetType = connection.Signal_type
                    End If
                Next

                rowCount = _kblMapper.KBLNetMapper(netRowInfoContainer.NetName).Count
            Else
                Dim compareObjKey As String = DirectCast(e.Row.Band.Tag, Dictionary(Of String, Object)).Keys.ElementAt(e.Row.Index)
                Dim compareObjValue As Object = DirectCast(e.Row.Band.Tag, Dictionary(Of String, Object)).Values.ElementAt(e.Row.Index)

                If (compareObjKey.SplitRemoveEmpty("|"c).Length > 1) Then
                    netRowInfoContainer.NetName = ExtractSystemId(compareObjKey)
                End If

                Dim compareObjects As New Dictionary(Of String, Object)
                If (TypeOf compareObjValue Is ChangedProperty) Then
                    Dim changeProperty As ChangedProperty = DirectCast(compareObjValue, NetChangedProperty)
                    If (changeProperty.ChangedProperties.ContainsKey("NetConnections")) Then
                        Dim comparisonMapper As ConnectionComparisonMapper = DirectCast(changeProperty.ChangedProperties("NetConnections"), ConnectionComparisonMapper)
                        For Each cItem As ChangedItem In comparisonMapper.Changes
                            compareObjects.Add(String.Format("{0}|{1}", cItem.Text, cItem.Key), cItem.Item)
                        Next
                    End If
                End If

                rowCount = compareObjects.Count
                If e.Row.Band.ChildBands?.Count > 0 Then
                    e.Row.GetChildRows(0).Tag = compareObjects
                End If
                SetDiffCellValueFromObjectKey(e.Row, compareObjKey)
            End If

            e.Row.Tag = netRowInfoContainer
            If e.Row.Band.ChildBands.Count > 0 Then
                e.Row.GetChildRows(0).SetCount(rowCount)
            End If
        Else
            If (e.Row.ParentCollection.Tag Is Nothing) Then
                Dim netId As String = e.Row.ParentRow.Tag?.ToString.Split("|".ToCharArray).FirstOrDefault
                If _kblMapper.KBLNetMapper.ContainsKey(netId) Then
                    e.Row.Tag = _kblMapper.KBLNetMapper(netId).ElementAtOrDefault(e.Row.Index)
                End If
            Else
                Dim compareObjKey As String = DirectCast(e.Row.ParentCollection.Tag, Dictionary(Of String, Object)).Keys.ElementAt(e.Row.Index)
                Dim compareObjValue As Object = DirectCast(e.Row.ParentCollection.Tag, Dictionary(Of String, Object)).Values.ElementAt(e.Row.Index)

                If (TypeOf compareObjValue Is Connection) Then
                    e.Row.Tag = DirectCast(compareObjValue, Connection)
                Else
                    e.Row.Tag = _kblMapper.GetConnections.Where(Function(conn) conn.Id = ExtractSystemId(compareObjKey))(0)
                End If

                SetDiffCellValueFromObjectKey(e.Row, compareObjKey)
            End If
        End If
    End Sub

    Private Sub ugNets_AfterRowActivate(sender As Object, e As EventArgs) Handles ugNets.AfterRowActivate
        If (Me.ugNets.ActiveRow.Appearance.BackColor = Color.FromArgb(190, 190, 190)) Then
            Me.ugNets.ActiveRow = Nothing
        ElseIf (_kblMapperForCompare IsNot Nothing) Then
            If (SetInformationHubEventArgs(Nothing, Me.ugNets.ActiveRow, Nothing)) Then
                OnHubSelectionChanged()
            End If

            If (Not Me.Focused) Then
                Me.Focus()
            End If
        End If
    End Sub

    Private Sub ugNets_AfterSelectChange(sender As Object, e As AfterSelectChangeEventArgs) Handles ugNets.AfterSelectChange
        With Me.ugNets
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

    Private Sub ugNets_BeforeSelectChange(sender As Object, e As BeforeSelectChangeEventArgs) Handles ugNets.BeforeSelectChange
        ClearSelectedChildRowsInGrid(Me.ugNets)

        For Each row As UltraGridRow In e.NewSelections.Rows
            If (row.Appearance.BackColor = Color.FromArgb(190, 190, 190)) Then
                e.Cancel = True
                Exit For
            End If
        Next
    End Sub

    Private Sub ugNets_ClickCell(sender As Object, e As ClickCellEventArgs) Handles ugNets.ClickCell
        If (_pressedMouseButton = System.Windows.Forms.MouseButtons.Left) AndAlso (_pressedKey <> Keys.ControlKey AndAlso _pressedKey <> Keys.ShiftKey) AndAlso (_informationHubEventArgs IsNot Nothing) AndAlso (_kblMapperForCompare IsNot Nothing OrElse e.Cell.Row.Appearance.BackColor <> Color.FromArgb(190, 190, 190)) Then
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

    Private Sub ugNets_ClickCellButton(sender As Object, e As CellEventArgs) Handles ugNets.ClickCellButton
        If (_pressedMouseButton = System.Windows.Forms.MouseButtons.Left) Then
            If (TypeOf e.Cell.Tag Is Connection) Then
                With DirectCast(e.Cell.Tag, Connection)
                    Select Case e.Cell.Column.Key
                        Case ConnectionPropertyName.External_references
                            Dim externalReferences As New List(Of External_reference)

                            For Each externalReference As String In .External_references.SplitSpace
                                externalReferences.Add(DirectCast(_kblMapper.KBLOccurrenceMapper(externalReference), External_reference))
                            Next

                            ShowDetailInformationForm(InformationHubStrings.ExtReferences_Caption, externalReferences, Nothing, _kblMapper)
                        Case ConnectionPropertyName.Installation_Information
                            ShowDetailInformationForm(InformationHubStrings.InstallInfo_Caption, .Installation_information, Nothing, _kblMapper)
                        Case ConnectionPropertyName.Processing_Information
                            ShowDetailInformationForm(InformationHubStrings.ProcInfo_Caption, .Processing_information, Nothing, _kblMapper)
                    End Select
                End With
            ElseIf (TypeOf e.Cell.Tag Is KeyValuePair(Of String, Object)) Then
                RequestDialogCompareData(True, DirectCast(e.Cell.Row.Tag, KeyValuePair(Of String, Object)).Key.Split("|"c, StringSplitOptions.RemoveEmptyEntries)(1), DirectCast(e.Cell.Tag, KeyValuePair(Of String, Object)))
            End If
        End If
    End Sub

    Private Sub ugNets_DoubleClickRow(sender As Object, e As DoubleClickRowEventArgs) Handles ugNets.DoubleClickRow
        OnDoubleClickRow(sender, e)
    End Sub

    Private Sub ugNets_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugNets.InitializeLayout
        Me.ugNets.BeginUpdate()
        Me.ugNets.EventManager.AllEventsEnabled = False

        InitializeGridLayout(_netGridAppearance, e.Layout)

        Me.ugNets.EventManager.AllEventsEnabled = True
        Me.ugNets.EndUpdate()
    End Sub

    Private Sub ugNets_InitializeRow(sender As Object, e As InitializeRowEventArgs) Handles ugNets.InitializeRow
        If Not e.ReInitialize Then
            InitializeNetRowCore(e.Row)
        End If
    End Sub

    Private Sub InitializeNetRowCore(row As UltraGridRow)
        If (row.ParentRow Is Nothing) Then
            If (DirectCast(row.ListObject, UltraDataRow).Band.Tag Is Nothing) Then
                row.Tag = DirectCast(row.ListObject, UltraDataRow).Tag

                If _redliningInformation IsNot Nothing AndAlso _redliningInformation.Redlinings.Where(Function(redlining) redlining.ObjectId = row.Tag?.ToString).Any() Then
                    row.Cells(0).Appearance.Image = My.Resources.Redlining
                End If
            Else
                InitializeRowForCompare(DirectCast(DirectCast(row.ListObject, UltraDataRow).Band.Tag, Dictionary(Of String, Object)), row)
            End If

            Me.ugNets.EventManager.AllEventsEnabled = False

            If (row?.ChildBands?.Count).GetValueOrDefault > 0 Then
                For Each childRow As UltraGridRow In row.ChildBands(0).Rows
                    If (_kblMapper.KBLOccurrenceMapper.ContainsKey(childRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString)) Then
                        childRow.Tag = DirectCast(_kblMapper.KBLOccurrenceMapper(childRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString), Connection).Wire
                    ElseIf (_kblMapperForCompare IsNot Nothing) AndAlso (_kblMapperForCompare.KBLOccurrenceMapper.ContainsKey(childRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString)) Then
                        childRow.Tag = DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(childRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString), Connection).Wire
                    End If

                    InitializeNetRowCore(childRow)
                Next
            End If
            Me.ugNets.EventManager.AllEventsEnabled = True
        Else
            If (DirectCast(row.ListObject, UltraDataRow).ParentCollection.Tag IsNot Nothing) Then
                InitializeRowForCompare(DirectCast(DirectCast(row.ListObject, UltraDataRow).ParentCollection.Tag, Dictionary(Of String, Object)), row)
            ElseIf _redliningInformation IsNot Nothing AndAlso _redliningInformation.Redlinings.Where(Function(redlining) redlining.ObjectId = row.Tag?.ToString).Any() Then
                row.Cells(1).Appearance.Image = My.Resources.Redlining
            End If

            For Each cell As UltraGridCell In row.Cells
                If (cell.Value IsNot Nothing AndAlso cell.Value.ToString = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS) Then
                    cell.Style = ColumnStyle.Button

                    If (TypeOf row.Tag Is KeyValuePair(Of String, Object)) Then
                        Dim compareObject As KeyValuePair(Of String, Object) = DirectCast(row.Tag, KeyValuePair(Of String, Object))

                        If (TypeOf compareObject.Value Is ConnectionChangedProperty) Then
                            For Each changedProperty As KeyValuePair(Of String, Object) In DirectCast(compareObject.Value, ConnectionChangedProperty).ChangedProperties
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
                        cell.Tag = _kblMapper.KBLOccurrenceMapper(cell.Row.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString)
                    End If
                End If
            Next
        End If
    End Sub

    Private Sub ugNets_KeyDown(sender As Object, e As KeyEventArgs) Handles ugNets.KeyDown
        OnGridKeyDown(Me, New GridKeyDownEventArgs(e, DirectCast(sender, UltraGrid)))
        If Not e.Handled Then
            If (e.Control) AndAlso (e.KeyCode = Keys.A) Then
                SelectAllRowsOfActiveGrid()
            ElseIf (e.KeyCode = Keys.Escape) Then
                If (Me.ugNets.Selected.Rows.Count = 0) Then
                    ClearMarkedRowsInGrids()
                Else
                    ClearSelectedRowsInGrids()
                End If

                If (SetInformationHubEventArgs(Nothing, Nothing, Me.ugNets.Selected.Rows)) Then
                    OnHubSelectionChanged()
                End If

            ElseIf (e.KeyCode = Keys.Return) Then
                SetSelectedAsMarkedOriginRows(ugNets)
            End If
        End If
    End Sub

    Private Sub ugNets_MouseDown(sender As Object, e As MouseEventArgs) Handles ugNets.MouseDown
        If (_contextMenuGrid.IsOpen) Then
            _contextMenuGrid.ClosePopup()
        End If

        _pressedMouseButton = e.Button
    End Sub

    Private Sub ugNets_MouseLeave(sender As Object, e As EventArgs) Handles ugNets.MouseLeave
        _messageEventArgs.StatusMessage = String.Empty
        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub ugNets_MouseMove(sender As Object, e As MouseEventArgs) Handles ugNets.MouseMove
        _messageEventArgs.StatusMessage = String.Format(InformationHubStrings.RowCount_Label, Me.ugNets.Rows.Count, Me.ugNets.Rows.FilteredInNonGroupByRowCount, Me.ugNets.Rows.VisibleRowCount, Me.ugNets.Selected.Rows.Count)
        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub udsNets_CellDataUpdated(sender As Object, e As CellDataUpdatedEventArgs) Handles udsNets.CellDataUpdated
        RaiseEvent CellValueUpdated(sender, e)
    End Sub

End Class