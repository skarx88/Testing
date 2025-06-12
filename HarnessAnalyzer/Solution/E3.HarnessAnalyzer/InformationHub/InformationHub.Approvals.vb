Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinGrid
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Partial Public Class InformationHub

    Private Sub InitializeApprovals()
        _approvalGridAppearance = GridAppearance.All.OfType(Of ApprovalGridAppearance).Single

        If (_comparisonMapperList Is Nothing) Then
            FillDataSource(Me.udsApprovals, _approvalGridAppearance, _kblMapper.GetApprovals.Count)
        Else
            FillDataSource(Me.udsApprovals, _approvalGridAppearance, 0, _comparisonMapperList(KblObjectType.Approval))
        End If

        If (Me.udsApprovals.Band.Columns.Count = 0) OrElse (Me.udsApprovals.Rows.Count = 0) Then
            Me.utcInformationHub.Tabs(TabNames.tabApprovals.ToString).Visible = False
        End If
    End Sub

    Private Sub udsApprovals_CellDataRequested(sender As Object, e As CellDataRequestedEventArgs) Handles udsApprovals.CellDataRequested
        e.CacheData = False

        With DirectCast(e.Row.Tag, Approval)
            Dim kblMapper As KblMapper = _kblMapper

            If (e.Row.Band.Columns.Exists(NameOf(InformationHubStrings.DiffType_ColumnCaption)) AndAlso (e.Row.GetCellValue(NameOf(InformationHubStrings.DiffType_ColumnCaption))).ToString = InformationHubStrings.Added) Then
                kblMapper = If(_kblMapperForCompare IsNot Nothing, _kblMapperForCompare, _kblMapper)
            End If

            Select Case e.Column.Key
                Case ApprovalPropertyName.Name
                    If .Name IsNot Nothing Then
                        e.Data = .Name
                    End If
                Case ApprovalPropertyName.Department
                    If (.Department IsNot Nothing) Then
                        e.Data = .Department
                    End If
                Case ApprovalPropertyName.Date
                    e.Data = .Date
                Case ApprovalPropertyName.Type_of_approval
                    e.Data = .Type_of_approval
                Case ApprovalPropertyName.Is_applied_to
                    If (kblMapper.GetHarness.SystemId = .Is_applied_to) Then
                        e.Data = String.Format("{0} [{1}]", kblMapper.HarnessPartNumber, KblObjectType.Harness.ToLocalizedString)
                    ElseIf (kblMapper.GetHarness.GetHarnessConfiguration(.Is_applied_to) IsNot Nothing) Then
                        e.Data = String.Format("{0} [{1}]", kblMapper.GetHarness.GetHarnessConfiguration(.Is_applied_to).Part_number, KblObjectType.Module_configuration.ToLocalizedString)
                    ElseIf (kblMapper.GetHarness.GetModule(.Is_applied_to) IsNot Nothing) Then
                        e.Data = String.Format("{0} [{1}]", kblMapper.GetHarness.GetModule(.Is_applied_to).Part_number, KblObjectType.Harness_module.ToLocalizedString)
                    End If
                Case Else
                    OnUnhandledCellDataRequested(e)
            End Select
        End With
    End Sub

    Private Sub udsApprovals_InitializeDataRow(sender As Object, e As InitializeDataRowEventArgs) Handles udsApprovals.InitializeDataRow
        Dim approval As Approval = Nothing

        If (e.Row.Band.Tag Is Nothing) Then
            approval = _kblMapper.GetApprovals(e.Row.Index)
        Else
            Dim compareObjKey As String = DirectCast(e.Row.Band.Tag, Dictionary(Of String, Object)).Keys.ElementAt(e.Row.Index)
            Dim compareObjValue As Object = DirectCast(e.Row.Band.Tag, Dictionary(Of String, Object)).Values.ElementAt(e.Row.Index)

            If (TypeOf compareObjValue Is Approval) Then
                approval = DirectCast(compareObjValue, Approval)
            Else
                approval = DirectCast(_kblMapper.KBLOccurrenceMapper(ExtractSystemId(compareObjKey)), Approval)
            End If

            SetDiffCellValueFromObjectKey(e.Row, compareObjKey)
        End If

        e.Row.Tag = approval
    End Sub

    Private Sub ugApprovals_AfterRowActivate(sender As Object, e As EventArgs) Handles ugApprovals.AfterRowActivate
        If (Me.ugApprovals.ActiveRow.Appearance.BackColor = Color.FromArgb(190, 190, 190)) Then
            Me.ugApprovals.ActiveRow = Nothing
        ElseIf (_kblMapperForCompare IsNot Nothing) Then
            If (SetInformationHubEventArgs(Nothing, Me.ugApprovals.ActiveRow, Nothing)) Then
                OnHubSelectionChanged()
            End If
            If (Not Me.Focused) Then Me.Focus()
        End If
    End Sub

    Private Sub ugApprovals_AfterSelectChange(sender As Object, e As AfterSelectChangeEventArgs) Handles ugApprovals.AfterSelectChange
        With Me.ugApprovals
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

    Private Sub ugApprovals_BeforeSelectChange(sender As Object, e As BeforeSelectChangeEventArgs) Handles ugApprovals.BeforeSelectChange
        For Each row As UltraGridRow In e.NewSelections.Rows
            If (row.Appearance.BackColor = Color.FromArgb(190, 190, 190)) Then e.Cancel = True
        Next
    End Sub

    Private Sub ugApprovals_ClickCell(sender As Object, e As ClickCellEventArgs) Handles ugApprovals.ClickCell
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

    Private Sub ugApprovals_DoubleClickRow(sender As Object, e As DoubleClickRowEventArgs) Handles ugApprovals.DoubleClickRow
        OnDoubleClickRow(sender, e)
    End Sub

    Private Sub ugApprovals_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugApprovals.InitializeLayout
        Me.ugApprovals.BeginUpdate()
        Me.ugApprovals.EventManager.AllEventsEnabled = False

        InitializeGridLayout(_approvalGridAppearance, e.Layout)

        Me.ugApprovals.EventManager.AllEventsEnabled = True
        Me.ugApprovals.EndUpdate()
    End Sub

    Private Sub InitializeApprovalRowCore(row As UltraGridRow)
        If (DirectCast(row.ListObject, UltraDataRow).Band.Tag Is Nothing) Then
            row.Tag = DirectCast(DirectCast(row.ListObject, UltraDataRow).Tag, Approval).SystemId

            If _redliningInformation IsNot Nothing AndAlso _redliningInformation.Redlinings.Where(Function(redlining) redlining.ObjectId = row.Tag?.ToString).Any() Then
                row.Cells(0).Appearance.Image = My.Resources.Redlining
            End If
        Else
            InitializeRowForCompare(DirectCast(DirectCast(row.ListObject, UltraDataRow).Band.Tag, Dictionary(Of String, Object)), row)
        End If
    End Sub

    Private Sub ugApprovals_InitializeRow(sender As Object, e As InitializeRowEventArgs) Handles ugApprovals.InitializeRow
        If Not e.ReInitialize Then
            InitializeApprovalRowCore(e.Row)
        End If
    End Sub

    Private Sub ugApprovals_KeyDown(sender As Object, e As KeyEventArgs) Handles ugApprovals.KeyDown
        OnGridKeyDown(Me, New GridKeyDownEventArgs(e, DirectCast(sender, UltraGrid)))
        If Not e.Handled Then
            If (e.Control) AndAlso (e.KeyCode = Keys.A) Then
                SelectAllRowsOfActiveGrid()
            ElseIf (e.KeyCode = Keys.Escape) Then
                If (Me.ugApprovals.Selected.Rows.Count = 0) Then
                    ClearMarkedRowsInGrids()
                Else
                    ClearSelectedRowsInGrids()
                End If

                If (SetInformationHubEventArgs(Nothing, Nothing, Me.ugApprovals.Selected.Rows)) Then
                    OnHubSelectionChanged()
                End If
            ElseIf (e.KeyCode = Keys.Return) Then
                SetSelectedAsMarkedOriginRows(ugApprovals)
            End If
        End If
    End Sub

    Private Sub ugApprovals_MouseDown(sender As Object, e As MouseEventArgs) Handles ugApprovals.MouseDown
        If (_contextMenuGrid.IsOpen) Then _contextMenuGrid.ClosePopup()

        _pressedMouseButton = e.Button
    End Sub

    Private Sub ugApprovals_MouseLeave(sender As Object, e As EventArgs) Handles ugApprovals.MouseLeave
        _messageEventArgs.StatusMessage = String.Empty

        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub ugApprovals_MouseMove(sender As Object, e As MouseEventArgs) Handles ugApprovals.MouseMove
        _messageEventArgs.StatusMessage = String.Format(InformationHubStrings.RowCount_Label, Me.ugApprovals.Rows.Count, Me.ugApprovals.Rows.FilteredInNonGroupByRowCount, Me.ugApprovals.Rows.VisibleRowCount, Me.ugApprovals.Selected.Rows.Count)

        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub udsApprovals_CellDataUpdated(sender As Object, e As CellDataUpdatedEventArgs) Handles udsApprovals.CellDataUpdated
        RaiseEvent CellValueUpdated(sender, e)
    End Sub

End Class