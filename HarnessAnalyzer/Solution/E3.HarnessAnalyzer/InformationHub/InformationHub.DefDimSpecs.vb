Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinGrid
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Partial Public Class InformationHub

    Private Sub InitializeDefDimSpecs()
        _defDimSpecGridAppearance = GridAppearance.All.OfType(Of DefaultDimSpecGridAppearance).Single
        'HINT: there is no row filtere here 

        If (_comparisonMapperList Is Nothing) Then
            FillDataSource(Me.udsDefDimSpecs, _defDimSpecGridAppearance, _kblMapper.GetDefaultDimensionSpecifications.Count)
        Else
            FillDataSource(Me.udsDefDimSpecs, _defDimSpecGridAppearance, 0, _comparisonMapperList(KblObjectType.Default_dimension_specification))
        End If

        If (Me.udsDefDimSpecs.Band.Columns.Count = 0) OrElse (Me.udsDefDimSpecs.Rows.Count = 0) Then
            Me.utcInformationHub.Tabs(TabNames.tabDefDimSpecs.ToString()).Visible = False
        End If

    End Sub

    Private Sub udsDefDimSpecs_CellDataRequested(sender As Object, e As CellDataRequestedEventArgs) Handles udsDefDimSpecs.CellDataRequested
        e.CacheData = False

        With DirectCast(e.Row.Tag, Default_dimension_specification)
            Dim kblMapper As KblMapper
            If HasChangeTypeWithInverse(e.Row, CompareChangeType.New) Then
                kblMapper = If(_kblMapperForCompare IsNot Nothing, _kblMapperForCompare, _kblMapper)
            Else
                kblMapper = _kblMapper
            End If

            Select Case e.Column.Key
                Case DefaultDimensionSpecificationPropertyName.Dimension_value_range
                    If (.Dimension_value_range IsNot Nothing) Then
                        e.Data = String.Format("Min.: {0} / Max.: {1}", String.Format("{0} {1}", Math.Round(.Dimension_value_range.Minimum, 2), kblMapper.KBLUnitMapper(.Dimension_value_range.Unit_component).Unit_name), String.Format("{0} {1}", Math.Round(.Dimension_value_range.Maximum, 2), kblMapper.KBLUnitMapper(.Dimension_value_range.Unit_component).Unit_name))
                    End If
                Case DefaultDimensionSpecificationPropertyName.Tolerance_type
                    If (.Tolerance_type IsNot Nothing) Then
                        e.Data = .Tolerance_type
                    End If
                Case DefaultDimensionSpecificationPropertyName.External_references
                    If (.External_references IsNot Nothing) AndAlso (.External_references.SplitSpace.Length <> 0) Then
                        e.Data = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                    End If
                Case DefaultDimensionSpecificationPropertyName.Tolerance_indication
                    If (.Tolerance_indication IsNot Nothing) Then
                        If (.Tolerance_indication.Lower_limit IsNot Nothing) AndAlso (.Tolerance_indication.Upper_limit IsNot Nothing) Then
                            e.Data = String.Format("{0}: {1} / {2}: {3}", ObjectPropertyNameStrings.LowerLimit, String.Format("{0} {1}", Math.Round(.Tolerance_indication.Lower_limit.Value_component, 2), kblMapper.KBLUnitMapper(.Tolerance_indication.Lower_limit.Unit_component).Unit_name), ObjectPropertyNameStrings.UpperLimit, String.Format("{0} {1}", Math.Round(.Tolerance_indication.Upper_limit.Value_component, 2), kblMapper.KBLUnitMapper(.Tolerance_indication.Upper_limit.Unit_component).Unit_name))
                        ElseIf (.Tolerance_indication.Lower_limit IsNot Nothing) Then
                            e.Data = String.Format("{0}: {1}", ObjectPropertyNameStrings.LowerLimit, String.Format("{0} {1}", Math.Round(.Tolerance_indication.Lower_limit.Value_component, 2), kblMapper.KBLUnitMapper(.Tolerance_indication.Lower_limit.Unit_component).Unit_name))
                        Else
                            e.Data = String.Format("{0}: {1}", ObjectPropertyNameStrings.UpperLimit, String.Format("{0} {1}", Math.Round(.Tolerance_indication.Upper_limit.Value_component, 2), kblMapper.KBLUnitMapper(.Tolerance_indication.Upper_limit.Unit_component).Unit_name))
                        End If
                    End If
                Case Else
                    OnUnhandledCellDataRequested(e)
            End Select
        End With
    End Sub

    Private Sub udsDefDimSpecs_InitializeDataRow(sender As Object, e As InitializeDataRowEventArgs) Handles udsDefDimSpecs.InitializeDataRow
        Dim def_Dim_Spec As Default_dimension_specification

        If (e.Row.Band.Tag Is Nothing) Then
            def_Dim_Spec = _kblMapper.GetDefaultDimensionSpecifications(e.Row.Index)
        Else
            Dim compareObjKey As String = DirectCast(e.Row.Band.Tag, Dictionary(Of String, Object)).Keys.ElementAt(e.Row.Index)
            Dim compareObjValue As Object = DirectCast(e.Row.Band.Tag, Dictionary(Of String, Object)).Values.ElementAt(e.Row.Index)

            If (TypeOf compareObjValue Is Default_dimension_specification) Then
                def_Dim_Spec = DirectCast(compareObjValue, Default_dimension_specification)
            Else
                def_Dim_Spec = DirectCast(_kblMapper.KBLOccurrenceMapper(ExtractSystemId(compareObjKey)), Default_dimension_specification)
            End If

            SetDiffCellValueFromObjectKey(e.Row, compareObjKey)
        End If

        e.Row.Tag = def_Dim_Spec
    End Sub

    Private Sub udsDefDimSpecs_CellDataUpdated(sender As Object, e As CellDataUpdatedEventArgs) Handles udsDefDimSpecs.CellDataUpdated
        RaiseEvent CellValueUpdated(sender, e)
    End Sub

    Private Sub ugDefDimSpecs_AfterRowActivate(sender As Object, e As EventArgs) Handles ugDefDimSpecs.AfterRowActivate
        If (Me.ugDefDimSpecs.ActiveRow.Appearance.BackColor = Color.FromArgb(190, 190, 190)) Then
            Me.ugDefDimSpecs.ActiveRow = Nothing
        ElseIf (_kblMapperForCompare IsNot Nothing) Then
            If (SetInformationHubEventArgs(Nothing, Me.ugDefDimSpecs.ActiveRow, Nothing)) Then
                OnHubSelectionChanged()

            End If
            If (Not Me.Focused) Then Me.Focus()
        End If
    End Sub

    Private Sub ugDefDimSpecs_AfterSelectChange(sender As Object, e As AfterSelectChangeEventArgs) Handles ugDefDimSpecs.AfterSelectChange
        With Me.ugDefDimSpecs
            .BeginUpdate()
            If (.ActiveRow IsNot Nothing) AndAlso (Not .ActiveRowScrollRegion.VisibleRows.Contains(.ActiveRow)) Then .ActiveRowScrollRegion.ScrollRowIntoView(.ActiveRow)
            .EndUpdate()
            If (SetInformationHubEventArgs(Nothing, Nothing, .Selected.Rows)) Then
                OnHubSelectionChanged()
            End If
        End With


    End Sub

    Private Sub ugDefDimSpecs_BeforeSelectChange(sender As Object, e As BeforeSelectChangeEventArgs) Handles ugDefDimSpecs.BeforeSelectChange
        For Each row As UltraGridRow In e.NewSelections.Rows
            If (row.Appearance.BackColor = Color.FromArgb(190, 190, 190)) Then e.Cancel = True
        Next
    End Sub

    Private Sub ugDefDimSpecs_ClickCell(sender As Object, e As ClickCellEventArgs) Handles ugDefDimSpecs.ClickCell
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

    Private Sub ugDefDimSpecs_ClickCellButton(sender As Object, e As CellEventArgs) Handles ugDefDimSpecs.ClickCellButton
        If (_pressedMouseButton = System.Windows.Forms.MouseButtons.Left) Then
            If (TypeOf e.Cell.Tag Is Default_dimension_specification) Then
                With DirectCast(e.Cell.Tag, Default_dimension_specification)
                    Select Case e.Cell.Column.Key
                        Case DefaultDimensionSpecificationPropertyName.External_references.ToString
                            Dim externalReferences As New List(Of External_reference)

                            For Each externalReference As String In .External_references.SplitSpace
                                externalReferences.Add(DirectCast(_kblMapper.KBLOccurrenceMapper(externalReference), External_reference))
                            Next

                            ShowDetailInformationForm(InformationHubStrings.ExtReferences_Caption, externalReferences, Nothing, _kblMapper)
                    End Select
                End With
            ElseIf (TypeOf e.Cell.Tag Is KeyValuePair(Of String, Object)) Then
                RequestDialogCompareData(True, DirectCast(e.Cell.Row.Tag, KeyValuePair(Of String, Object)).Key.SplitRemoveEmpty("|"c)(1), DirectCast(e.Cell.Tag, KeyValuePair(Of String, Object)))
            End If
        End If
    End Sub

    Private Sub ugDefDimSpecs_DoubleClickRow(sender As Object, e As DoubleClickRowEventArgs) Handles ugDefDimSpecs.DoubleClickRow
        OnDoubleClickRow(sender, e)
    End Sub

    Private Sub ugDefDimSpecs_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugDefDimSpecs.InitializeLayout
        Me.ugDefDimSpecs.BeginUpdate()
        Me.ugDefDimSpecs.EventManager.AllEventsEnabled = False

        InitializeGridLayout(_defDimSpecGridAppearance, e.Layout)

        Me.ugDefDimSpecs.EventManager.AllEventsEnabled = True
        Me.ugDefDimSpecs.EndUpdate()
    End Sub

    Private Sub InitializeDefDimSpecRowCore(row As UltraGridRow)
        If (DirectCast(row.ListObject, UltraDataRow).Band.Tag Is Nothing) Then
            row.Tag = DirectCast(DirectCast(row.ListObject, UltraDataRow).Tag, Default_dimension_specification).SystemId

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

                    If (TypeOf compareObject.Value Is DefDimSpecChangedProperty) Then
                        For Each changedProperty As KeyValuePair(Of String, Object) In DirectCast(compareObject.Value, DefDimSpecChangedProperty).ChangedProperties
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

    Private Sub ugDefDimSpecs_InitializeRow(sender As Object, e As InitializeRowEventArgs) Handles ugDefDimSpecs.InitializeRow
        If Not e.ReInitialize Then
            InitializeDefDimSpecRowCore(e.Row)
        End If
    End Sub

    Private Sub ugDefDimSpecs_KeyDown(sender As Object, e As KeyEventArgs) Handles ugDefDimSpecs.KeyDown
        OnGridKeyDown(Me, New GridKeyDownEventArgs(e, DirectCast(sender, UltraGrid)))
        If Not e.Handled Then
            If Not e.Handled Then
                If (e.Control) AndAlso (e.KeyCode = Keys.A) Then
                    SelectAllRowsOfActiveGrid()
                ElseIf (e.KeyCode = Keys.Escape) Then
                    If (Me.ugDefDimSpecs.Selected.Rows.Count = 0) Then
                        ClearMarkedRowsInGrids()
                    Else
                        ClearSelectedRowsInGrids()
                    End If

                    If (SetInformationHubEventArgs(Nothing, Nothing, Me.ugDefDimSpecs.Selected.Rows)) Then
                        OnHubSelectionChanged()
                    End If

                ElseIf (e.KeyCode = Keys.Return) Then
                    SetSelectedAsMarkedOriginRows(ugDefDimSpecs)
                End If
            End If
        End If
    End Sub

    Private Sub ugDefDimSpecs_MouseDown(sender As Object, e As MouseEventArgs) Handles ugDefDimSpecs.MouseDown
        If (_contextMenuGrid.IsOpen) Then
            _contextMenuGrid.ClosePopup()
        End If

        _pressedMouseButton = e.Button
    End Sub

    Private Sub ugDefDimSpecs_MouseLeave(sender As Object, e As EventArgs) Handles ugDefDimSpecs.MouseLeave
        _messageEventArgs.StatusMessage = String.Empty

        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub ugDefDimSpecs_MouseMove(sender As Object, e As MouseEventArgs) Handles ugDefDimSpecs.MouseMove
        _messageEventArgs.StatusMessage = String.Format(InformationHubStrings.RowCount_Label, Me.ugDefDimSpecs.Rows.Count, Me.ugDefDimSpecs.Rows.FilteredInNonGroupByRowCount, Me.ugDefDimSpecs.Rows.VisibleRowCount, Me.ugDefDimSpecs.Selected.Rows.Count)

        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub ugDifferences_KeyDown(sender As Object, e As KeyEventArgs) Handles ugDifferences.KeyDown
        OnGridKeyDown(Me, New GridKeyDownEventArgs(e, DirectCast(sender, UltraGrid)))
        If Not e.Handled Then

        End If
    End Sub

End Class