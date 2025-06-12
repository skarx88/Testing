Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinGrid
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Partial Public Class InformationHub

    Private Sub InitializeDimSpecs()
        _dimSpecGridAppearance = GridAppearance.All.OfType(Of DimSpecGridAppearance).Single
        AddNewRowFilters(ugDimSpecs)

        If (_comparisonMapperList Is Nothing) Then
            FillDataSource(Me.udsDimSpecs, _dimSpecGridAppearance, _kblMapper.GetDimensionSpecifications.Count)
        Else
            FillDataSource(Me.udsDimSpecs, _dimSpecGridAppearance, 0, _comparisonMapperList(KblObjectType.dimension_specification))
        End If

        If (Me.udsDimSpecs.Band.Columns.Count = 0) OrElse (Me.udsDimSpecs.Rows.Count = 0) Then
            Me.utcInformationHub.Tabs(TabNames.tabDimSpecs.ToString()).Visible = False
        End If
    End Sub

    Private Sub udsDimSpecs_CellDataRequested(sender As Object, e As CellDataRequestedEventArgs) Handles udsDimSpecs.CellDataRequested
        e.CacheData = False

        With DirectCast(e.Row.Tag, Dimension_specification)
            Dim kblMapper As KblMapper
            If HasChangeTypeWithInverse(e.Row, CompareChangeType.New) Then
                kblMapper = If(_kblMapperForCompare IsNot Nothing, _kblMapperForCompare, _kblMapper)
            Else
                kblMapper = _kblMapper
            End If

            Select Case e.Column.Key
                Case DimensionSpecificationPropertyName.Id
                    If (.Id IsNot Nothing) Then
                        e.Data = .Id
                    End If
                Case DimensionSpecificationPropertyName.Alias_Id
                    If (.Alias_id.Length <> 0) Then
                        If (.Alias_id.Length = 1) AndAlso (.Alias_id(0).Description Is Nothing) AndAlso (.Alias_id(0).Scope Is Nothing) Then
                            e.Data = .Alias_id(0).Alias_id
                        Else
                            e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                        End If
                    End If
                Case DimensionSpecificationPropertyName.Dimension_value
                    If (.Dimension_value IsNot Nothing) Then
                        e.Data = String.Format("{0} {1}", Math.Round(.Dimension_value.Value_component, 1), kblMapper.KBLUnitMapper(.Dimension_value.Unit_component).Unit_name)
                    End If
                Case DimensionSpecificationPropertyName.Segments
                    If (.Segments IsNot Nothing) Then
                        For Each segmentId As String In .Segments.SplitSpace
                            e.Data = If(e.Data Is Nothing, [Lib].Schema.Kbl.Utils.GetUserId(kblMapper.KBLOccurrenceMapper(segmentId)), String.Format("{0}; {1}", e.Data.ToString, [Lib].Schema.Kbl.Utils.GetUserId(kblMapper.KBLOccurrenceMapper(segmentId))))
                        Next
                    End If
                Case DimensionSpecificationPropertyName.Origin
                    If (.Origin IsNot Nothing) Then
                        Dim fixing_ass As Fixing_assignment = kblMapper.GetOccurrenceObject(Of Fixing_assignment)(.Origin)
                        If fixing_ass IsNot Nothing Then
                            If ([Lib].Schema.Kbl.Utils.GetUserId(fixing_ass) Is Nothing) Then
                                e.Data = [Lib].Schema.Kbl.Utils.GetUserId(kblMapper.GetFixingOccurrence(fixing_ass.Fixing))
                            Else
                                e.Data = [Lib].Schema.Kbl.Utils.GetUserId(fixing_ass)
                            End If
                        Else
                            e.Data = [Lib].Schema.Kbl.Utils.GetUserId(kblMapper.KBLOccurrenceMapper(.Origin))
                        End If
                    End If
                Case DimensionSpecificationPropertyName.Target
                    If (.Target IsNot Nothing) Then
                        If (TypeOf kblMapper.KBLOccurrenceMapper(.Target) Is Fixing_assignment) Then
                            If ([Lib].Schema.Kbl.Utils.GetUserId(kblMapper.KBLOccurrenceMapper(.Target)) Is Nothing) Then
                                e.Data = [Lib].Schema.Kbl.Utils.GetUserId(kblMapper.KBLOccurrenceMapper(CType(kblMapper.KBLOccurrenceMapper(.Target), Fixing_assignment).Fixing))
                            Else
                                e.Data = [Lib].Schema.Kbl.Utils.GetUserId(kblMapper.KBLOccurrenceMapper(.Target))
                            End If
                        Else
                            e.Data = [Lib].Schema.Kbl.Utils.GetUserId(kblMapper.KBLOccurrenceMapper(.Target))
                        End If
                    End If
                Case DimensionSpecificationPropertyName.Processing_Information
                    If (.Processing_information.Length <> 0) Then
                        e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                    End If
                Case DimensionSpecificationPropertyName.Tolerance_indication
                    If (.Tolerance_indication IsNot Nothing) Then
                        If (.Tolerance_indication.Lower_limit IsNot Nothing) AndAlso (.Tolerance_indication.Upper_limit IsNot Nothing) Then
                            e.Data = String.Format("{0}: {1} / {2}: {3}", ObjectPropertyNameStrings.LowerLimit, String.Format("{0} {1}", Math.Round(.Tolerance_indication.Lower_limit.Value_component, 1), kblMapper.KBLUnitMapper(.Tolerance_indication.Lower_limit.Unit_component).Unit_name), ObjectPropertyNameStrings.UpperLimit, String.Format("{0} {1}", Math.Round(.Tolerance_indication.Upper_limit.Value_component, 2), kblMapper.KBLUnitMapper(.Tolerance_indication.Upper_limit.Unit_component).Unit_name))
                        ElseIf (.Tolerance_indication.Lower_limit IsNot Nothing) Then
                            e.Data = String.Format("{0}: {1}", ObjectPropertyNameStrings.LowerLimit, String.Format("{0} {1}", Math.Round(.Tolerance_indication.Lower_limit.Value_component, 1), kblMapper.KBLUnitMapper(.Tolerance_indication.Lower_limit.Unit_component).Unit_name))
                        Else
                            e.Data = String.Format("{0}: {1}", ObjectPropertyNameStrings.UpperLimit, String.Format("{0} {1}", Math.Round(.Tolerance_indication.Upper_limit.Value_component, 1), kblMapper.KBLUnitMapper(.Tolerance_indication.Upper_limit.Unit_component).Unit_name))
                        End If
                    End If
                Case Else
                    OnUnhandledCellDataRequested(e)
            End Select
        End With
    End Sub

    Private Sub udsDimSpecs_InitializeDataRow(sender As Object, e As InitializeDataRowEventArgs) Handles udsDimSpecs.InitializeDataRow
        Dim dimSpec As Dimension_specification = Nothing

        If (e.Row.Band.Tag Is Nothing) Then
            dimSpec = _kblMapper.GetDimensionSpecifications(e.Row.Index)
        Else
            Dim compareObjKey As String = DirectCast(e.Row.Band.Tag, Dictionary(Of String, Object)).Keys.ElementAt(e.Row.Index)
            Dim compareObjValue As Object = DirectCast(e.Row.Band.Tag, Dictionary(Of String, Object)).Values.ElementAt(e.Row.Index)

            If (TypeOf compareObjValue Is Dimension_specification) Then
                dimSpec = DirectCast(compareObjValue, Dimension_specification)
            Else
                dimSpec = _kblMapper.GetOccurrenceObject(Of Dimension_specification)(ExtractSystemId(compareObjKey))
            End If

            SetDiffCellValueFromObjectKey(e.Row, compareObjKey)
        End If

        e.Row.Tag = dimSpec
    End Sub

    Private Sub udsDimSpecs_CellDataUpdated(sender As Object, e As CellDataUpdatedEventArgs) Handles udsDimSpecs.CellDataUpdated
        RaiseEvent CellValueUpdated(sender, e)
    End Sub

    Private Sub ugDimSpecs_AfterRowActivate(sender As Object, e As EventArgs) Handles ugDimSpecs.AfterRowActivate
        If (Me.ugDimSpecs.ActiveRow.Appearance.BackColor = Color.FromArgb(190, 190, 190)) Then
            Me.ugDimSpecs.ActiveRow = Nothing
        ElseIf (_kblMapperForCompare IsNot Nothing) Then
            If (SetInformationHubEventArgs(Nothing, Me.ugDimSpecs.ActiveRow, Nothing)) Then
                OnHubSelectionChanged()
            End If
            If (Not Me.Focused) Then Me.Focus()
        End If
    End Sub

    Private Sub ugDimSpecs_AfterSelectChange(sender As Object, e As AfterSelectChangeEventArgs) Handles ugDimSpecs.AfterSelectChange
        With Me.ugDimSpecs
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

    Private Sub ugDimSpecs_BeforeSelectChange(sender As Object, e As BeforeSelectChangeEventArgs) Handles ugDimSpecs.BeforeSelectChange
        For Each row As UltraGridRow In e.NewSelections.Rows
            If (row.Appearance.BackColor = Color.FromArgb(190, 190, 190)) Then
                e.Cancel = True
            End If
        Next
    End Sub

    Private Sub ugDimSpecs_ClickCell(sender As Object, e As ClickCellEventArgs) Handles ugDimSpecs.ClickCell
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

    Private Sub ugDimSpecs_ClickCellButton(sender As Object, e As CellEventArgs) Handles ugDimSpecs.ClickCellButton
        If (_pressedMouseButton = System.Windows.Forms.MouseButtons.Left) Then
            If (TypeOf e.Cell.Tag Is Dimension_specification) Then
                With DirectCast(e.Cell.Tag, Dimension_specification)
                    Select Case e.Cell.Column.Key
                        Case DimensionSpecificationPropertyName.Alias_Id.ToString
                            ShowDetailInformationForm(InformationHubStrings.AliasIds_Caption, .Alias_Id, Nothing, _kblMapper)
                        Case DimensionSpecificationPropertyName.Processing_Information.ToString
                            ShowDetailInformationForm(InformationHubStrings.ProcInfo_Caption, .Processing_Information, Nothing, _kblMapper)
                    End Select
                End With
            ElseIf (TypeOf e.Cell.Tag Is KeyValuePair(Of String, Object)) Then
                RequestDialogCompareData(True, DirectCast(e.Cell.Row.Tag, KeyValuePair(Of String, Object)).Key.SplitRemoveEmpty("|"c)(1), DirectCast(e.Cell.Tag, KeyValuePair(Of String, Object)))
            End If
        End If
    End Sub

    Private Sub ugDimSpecs_DoubleClickRow(sender As Object, e As DoubleClickRowEventArgs) Handles ugDimSpecs.DoubleClickRow
        OnDoubleClickRow(sender, e)
    End Sub

    Private Sub ugDimSpecs_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugDimSpecs.InitializeLayout
        Me.ugDimSpecs.BeginUpdate()
        Me.ugDimSpecs.EventManager.AllEventsEnabled = False

        InitializeGridLayout(_dimSpecGridAppearance, e.Layout)

        Me.ugDimSpecs.EventManager.AllEventsEnabled = True
        Me.ugDimSpecs.EndUpdate()
    End Sub

    Private Sub InitializeDimSpecRowCore(row As UltraGridRow)
        If (DirectCast(row.ListObject, UltraDataRow).Band.Tag Is Nothing) Then
            row.Tag = DirectCast(DirectCast(row.ListObject, UltraDataRow).Tag, Dimension_specification).SystemId

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

                    If (TypeOf compareObject.Value Is DimSpecChangedProperty) Then
                        For Each changedProperty As KeyValuePair(Of String, Object) In DirectCast(compareObject.Value, DimSpecChangedProperty).ChangedProperties
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

    Private Sub ugDimSpecs_InitializeRow(sender As Object, e As InitializeRowEventArgs) Handles ugDimSpecs.InitializeRow
        If Not e.ReInitialize Then
            InitializeDimSpecRowCore(e.Row)
        End If
    End Sub

    Private Sub ugDimSpecs_KeyDown(sender As Object, e As KeyEventArgs) Handles ugDimSpecs.KeyDown
        OnGridKeyDown(Me, New GridKeyDownEventArgs(e, DirectCast(sender, UltraGrid)))

        If Not e.Handled Then
            If (e.Control) AndAlso (e.KeyCode = Keys.A) Then
                SelectAllRowsOfActiveGrid()
            ElseIf (e.KeyCode = Keys.Escape) Then
                If (Me.ugDimSpecs.Selected.Rows.Count = 0) Then
                    ClearMarkedRowsInGrids()
                Else
                    ClearSelectedRowsInGrids()
                End If

                If (SetInformationHubEventArgs(Nothing, Nothing, Me.ugDimSpecs.Selected.Rows)) Then
                    OnHubSelectionChanged()
                End If
            ElseIf (e.KeyCode = Keys.Return) Then
                SetSelectedAsMarkedOriginRows(ugDimSpecs)
            End If
        End If
    End Sub

    Private Sub ugDimSpecs_MouseDown(sender As Object, e As MouseEventArgs) Handles ugDimSpecs.MouseDown
        If (_contextMenuGrid.IsOpen) Then
            _contextMenuGrid.ClosePopup()
        End If
        _pressedMouseButton = e.Button
    End Sub

    Private Sub ugDimSpecs_MouseLeave(sender As Object, e As EventArgs) Handles ugDimSpecs.MouseLeave
        _messageEventArgs.StatusMessage = String.Empty
        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub ugDimSpecs_MouseMove(sender As Object, e As MouseEventArgs) Handles ugDimSpecs.MouseMove
        _messageEventArgs.StatusMessage = String.Format(InformationHubStrings.RowCount_Label, Me.ugDimSpecs.Rows.Count, Me.ugDimSpecs.Rows.FilteredInNonGroupByRowCount, Me.ugDimSpecs.Rows.VisibleRowCount, Me.ugDimSpecs.Selected.Rows.Count)
        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

End Class