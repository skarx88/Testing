Imports Infragistics.Win
Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinGrid
Imports Zuken.E3.HarnessAnalyzer.QualityStamping
Imports Zuken.E3.HarnessAnalyzer.Shared.Common

Partial Public Class InformationHub

    Private _IsRedliningForCavityNavigator As Boolean = False

    Private Sub InitializeRedlinings()
        With Me.udsRedlinings
            If (.Band.Columns.Count = 0) Then
                With .Band
                    .Columns.Add(SYSTEM_ID_COLUMN_KEY)
                    .Columns.Add("ObjectId")
                    .Columns.Add(NameOf(InformationHubStrings.ObjName_ColumnCaption))
                    .Columns.Add(NameOf(InformationHubStrings.ObjType_ColumnCaption))
                    .Columns.Add(NameOf(InformationHubStrings.Classification_ColumnCaption))
                    .Columns.Add(NameOf(InformationHubStrings.Comment_ColumnCaption))
                    .Columns.Add(NameOf(InformationHubStrings.Visible_ColumnCaption))
                    .Columns.Add(NameOf(InformationHubStrings.LastChangedBy_ColumnCaption))
                    .Columns.Add(NameOf(InformationHubStrings.LastChangedOn_ColumnCaption))
                    .Columns.Add(NameOf(InformationHubStrings.Group_ColumnCaption))

                    .Key = KblObjectType.Redlining.ToString(True)
                End With
            End If

            .Rows.Clear()

            If _redliningInformation IsNot Nothing AndAlso _redliningInformation.Redlinings.Count > 0 Then
                Me.utcInformationHub.Tabs(TabNames.tabRedlinings.ToString()).Visible = True

                For Each redlining As Redlining In _redliningInformation.Redlinings
                    Dim row As UltraDataRow = .Rows.Add
                    With row
                        .SetCellValue(SYSTEM_ID_COLUMN_KEY, redlining.ID)
                        .SetCellValue("ObjectId", redlining.ObjectId)
                        .SetCellValue(NameOf(InformationHubStrings.ObjName_ColumnCaption), redlining.ObjectName)

                        Dim CellCaption As String = [Lib].Schema.Kbl.Utils.GetLocalizedName(redlining.ObjectType)
                        .SetCellValue(NameOf(InformationHubStrings.ObjType_ColumnCaption), CellCaption)

                        Select Case redlining.Classification
                            Case RedliningClassification.Confirmation
                                .SetCellValue(NameOf(InformationHubStrings.Classification_ColumnCaption), InformationHubStrings.Confirmation_RedliningType)
                            Case RedliningClassification.Error
                                .SetCellValue(NameOf(InformationHubStrings.Classification_ColumnCaption), InformationHubStrings.Error_RedliningType)
                            Case RedliningClassification.GraphicalComment
                                .SetCellValue(NameOf(InformationHubStrings.Classification_ColumnCaption), InformationHubStrings.Graphical_RedliningType)
                            Case RedliningClassification.Information
                                .SetCellValue(NameOf(InformationHubStrings.Classification_ColumnCaption), InformationHubStrings.Info_RedliningType)
                            Case RedliningClassification.LengthComment
                                .SetCellValue(NameOf(InformationHubStrings.Classification_ColumnCaption), InformationHubStrings.Length_RedliningType)
                            Case RedliningClassification.Question
                                .SetCellValue(NameOf(InformationHubStrings.Classification_ColumnCaption), InformationHubStrings.Question_RedliningType)
                        End Select

                        If (redlining.Classification <> RedliningClassification.GraphicalComment) Then
                            .SetCellValue(NameOf(InformationHubStrings.Comment_ColumnCaption), redlining.Comment)
                        End If

                        .SetCellValue(NameOf(InformationHubStrings.Visible_ColumnCaption), redlining.IsVisible)
                        .SetCellValue(NameOf(InformationHubStrings.LastChangedBy_ColumnCaption), redlining.LastChangedBy)
                        .SetCellValue(NameOf(InformationHubStrings.LastChangedOn_ColumnCaption), Format(redlining.LastChangedOn, String.Format("{0} {1}", My.Application.Culture.DateTimeFormat.ShortDatePattern, My.Application.Culture.DateTimeFormat.ShortTimePattern)))
                        .SetCellValue(NameOf(InformationHubStrings.Group_ColumnCaption), If(redlining.OnGroup IsNot Nothing AndAlso redlining.OnGroup <> String.Empty, _redliningInformation?.RedliningGroups?.SingleOrDefault(Function(rdliningGrp) rdliningGrp.ID = redlining.OnGroup)?.ChangeTag, String.Empty))
                    End With
                Next
            Else
                Me.utcInformationHub.Tabs(TabNames.tabRedlinings.ToString()).Visible = False
            End If
        End With

    End Sub

    Friend Sub AddOrEditRedliningForCavityNavigator()
        Dim SelectedRows As New List(Of UltraGridRow)
        If _selectedConnectorRow IsNot Nothing Then
            SelectedRows.Add(_selectedConnectorRow)
        End If

        SelectedRows.AddRange(_selectedChildRows)

        Dim objectReferences As List(Of ObjectReference) = GetObjectReferences(SelectedRows)
        If (objectReferences.Count > 0) Then
            _IsRedliningForCavityNavigator = True
            ShowRedliningDialog(objectReferences.Select(Function(objRef) objRef.KblId).ToList, objectReferences.Select(Function(objRef) objRef.ObjectName).ToList, objectReferences.Select(Function(objRef) objRef.ObjectType).FirstOrDefault, Nothing, SelectedRows)

            Me.ugRedlinings.BeginUpdate()
            Me.ugRedlinings.DisplayLayout.Bands(0).Columns(NameOf(InformationHubStrings.Group_ColumnCaption)).ValueList = GetRedliningGroups()
            Me.ugRedlinings.EndUpdate()

        Else
            MessageBox.Show(InformationHubStrings.RedliningNotPossible_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
        _selectedConnectorRow = Nothing
    End Sub

    Friend Sub AddOrEditRedlining(clickedRow As UltraGridRow)
        Dim selectedGridRows As New List(Of UltraGridRow)
        _IsRedliningForCavityNavigator = True

        If (clickedRow IsNot Nothing) Then
            selectedGridRows.Add(clickedRow)
        ElseIf (Me.utcInformationHub.ActiveTab IsNot Nothing) Then
            For Each row As UltraGridRow In DirectCast(Me.utcInformationHub.ActiveTab.TabPage.Controls(0), UltraGrid).Selected.Rows
                selectedGridRows.Add(row)
            Next
        End If

        If (selectedGridRows.Count = 0) Then
            MessageBox.Show(InformationHubStrings.SelectObjectFirst_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        Dim objectReferences As List(Of ObjectReference) = GetObjectReferences(selectedGridRows)
        If (objectReferences.Count > 0) Then
            ShowRedliningDialog(objectReferences.Select(Function(objRef) objRef.KblId).ToList, objectReferences.Select(Function(objRef) objRef.ObjectName).ToList, objectReferences.Select(Function(objRef) objRef.ObjectType).FirstOrDefault, Nothing, selectedGridRows)

            Me.ugRedlinings.BeginUpdate()
            Me.ugRedlinings.DisplayLayout.Bands(0).Columns(NameOf(InformationHubStrings.Group_ColumnCaption)).ValueList = GetRedliningGroups()
            Me.ugRedlinings.EndUpdate()
        Else
            MessageBox.Show(InformationHubStrings.RedliningNotPossible_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Friend Sub DeleteRedlining()
        Dim selectedGridRows As New List(Of UltraGridRow)

        If (Me.utcInformationHub.ActiveTab IsNot Nothing) Then
            With DirectCast(Me.utcInformationHub.ActiveTab.TabPage.Controls(0), UltraGrid)
                For Each row As UltraGridRow In .Selected.Rows
                    selectedGridRows.Add(row)
                Next

                If (selectedGridRows.Count = 0) Then
                    If (.ActiveRow IsNot Nothing) Then
                        If (.ActiveCell Is Nothing OrElse Not .ActiveCell.IsInEditMode) Then
                            selectedGridRows.Add(.ActiveRow)
                        Else
                            Exit Sub
                        End If
                    ElseIf (.ActiveCell IsNot Nothing) Then
                        If (Not .ActiveCell.IsInEditMode) Then
                            selectedGridRows.Add(.ActiveCell.Row)
                        Else
                            Exit Sub
                        End If
                    End If
                End If
            End With
        End If

        If (selectedGridRows.Count = 0) Then
            MessageBox.Show(InformationHubStrings.SelectObjectFirst_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        ElseIf (MessageBoxex.ShowQuestion(InformationHubStrings.DeleteRedlining_Msg) = MsgBoxResult.No) Then
            Exit Sub
        End If

        If (Me.utcInformationHub.ActiveTab IsNot Nothing) Then
            ClearSelectedRowsInGrid(DirectCast(Me.utcInformationHub.ActiveTab.TabPage.Controls(0), UltraGrid))
        End If
        If (Me.utcInformationHub.Tabs("tabRedlinings").Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True) Then
            ClearMarkedRowsInGrids()
        End If

        Dim objectIds As New HashSet(Of String)
        Dim objectType As KblObjectType = KblObjectType.Undefined

        For Each selectedGridRow As UltraGridRow In selectedGridRows
            Dim redliningsForDeletion As New List(Of Redlining)

            If selectedGridRow.Band.Key = KblObjectType.Redlining.ToString AndAlso _redliningInformation IsNot Nothing Then
                For Each redlining As Redlining In _redliningInformation.Redlinings.Where(Function(rdlining) rdlining.ID = selectedGridRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString)
                    redliningsForDeletion.Add(redlining)
                Next
            ElseIf selectedGridRow.ParentRow Is Nothing AndAlso _redliningInformation IsNot Nothing Then
                For Each redlining As Redlining In _redliningInformation.Redlinings.Where(Function(rdlining) rdlining.ObjectId = selectedGridRow.Tag?.ToString)
                    objectIds.Add(redlining.ObjectId)
                    objectType = redlining.ObjectType
                    redliningsForDeletion.Add(redlining)
                Next

                selectedGridRow.Cells(0).Appearance.Image = Nothing
            ElseIf selectedGridRow.Band.Key = KblObjectType.Connection.ToString AndAlso _redliningInformation IsNot Nothing Then
                For Each redlining As Redlining In _redliningInformation.Redlinings.Where(Function(rdlining) rdlining.ObjectId = selectedGridRow.Tag?.ToString)
                    objectIds.Add(redlining.ObjectId)
                    objectType = redlining.ObjectType
                    redliningsForDeletion.Add(redlining)
                Next

                selectedGridRow.Cells(1).Appearance.Image = Nothing
            ElseIf _redliningInformation IsNot Nothing Then
                For Each redlining As Redlining In _redliningInformation.Redlinings.Where(Function(rdlining) rdlining.ObjectId = selectedGridRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString)
                    objectIds.Add(redlining.ObjectId)
                    objectType = redlining.ObjectType
                    redliningsForDeletion.Add(redlining)
                Next
                selectedGridRow.Cells(1).Appearance.Image = Nothing
            End If

            For Each redlining As Redlining In redliningsForDeletion
                If _redliningInformation IsNot Nothing Then
                    _redliningInformation.Redlinings.DeleteRedlining(redlining)
                End If
                ResetRedliningIconStateInGridCells(redlining.ObjectId, redlining.ObjectType)
            Next
        Next

        InitializeRedlinings()

        Me.ugRedlinings.UpdateData()

        _informationHubEventArgs.ObjectIds = objectIds
        _informationHubEventArgs.ObjectType = objectType

        RaiseEvent RedliningsChanged(Me, _informationHubEventArgs)
    End Sub

    Friend Sub ImportRedlinings()
        InitializeRedlinings()
        BeginGridUpdate()

        Me.ugRedlinings.UpdateData()

        _informationHubEventArgs.ObjectIds.NewOrClear
        If _redliningInformation IsNot Nothing Then
            For Each redlining As Redlining In _redliningInformation.Redlinings
                _informationHubEventArgs.ObjectIds.Add(redlining.ObjectId)
                ImportSingleRedlining(redlining)
            Next
        End If
        _informationHubEventArgs.ObjectType = KblObjectType.Undefined

        EndGridUpdate()

        RaiseEvent RedliningsChanged(Me, _informationHubEventArgs)
    End Sub


    Private Function GetRedliningGroups() As ValueList
        Dim valueList As New ValueList
        valueList.ValueListItems.Add(String.Empty, String.Empty)

        If _redliningInformation?.RedliningGroups IsNot Nothing Then
            For Each redliningGroup As RedliningGroup In _redliningInformation.RedliningGroups
                If (Not valueList.ValueListItems.Contains(redliningGroup.ID)) Then
                    valueList.ValueListItems.Add(redliningGroup.ID, redliningGroup.ChangeTag)
                End If
            Next
        End If

        valueList.SortStyle = ValueListSortStyle.Ascending

        Return valueList
    End Function

    Private Sub ImportSingleRedlining(redlining As Redlining)
        Select Case redlining.ObjectType
            Case E3.Lib.Schema.Kbl.KblObjectType.Accessory_occurrence
                If (ugAccessories.Rows.Where(Function(gridRow) gridRow.Tag?.ToString = redlining.ObjectId).Any()) Then
                    Me.ugAccessories.Rows.Single(Function(gridRow) gridRow.Tag?.ToString = redlining.ObjectId).Cells(0).Appearance.Image = My.Resources.Redlining
                End If
            Case E3.Lib.Schema.Kbl.KblObjectType.Assembly_part_occurrence, KblObjectType.Assembly_part
                If (ugAssemblyParts.Rows.Where(Function(gridRow) gridRow.Tag?.ToString = redlining.ObjectId).Any()) Then
                    Me.ugAssemblyParts.Rows.Single(Function(gridRow) gridRow.Tag?.ToString = redlining.ObjectId).Cells(0).Appearance.Image = My.Resources.Redlining
                End If
            Case E3.Lib.Schema.Kbl.KblObjectType.Special_wire_occurrence
                If (ugCables.Rows.Where(Function(gridRow) gridRow.Tag?.ToString = redlining.ObjectId).Any()) Then
                    Me.ugCables.Rows.Single(Function(gridRow) gridRow.Tag?.ToString = redlining.ObjectId).Cells(0).Appearance.Image = My.Resources.Redlining
                End If
            Case E3.Lib.Schema.Kbl.KblObjectType.Cavity_occurrence
                If (_kblMapper.KBLCavityConnectorMapper.ContainsKey(redlining.ObjectId)) Then
                    For Each connectorRow As UltraGridRow In Me.ugConnectors.Rows.Where(Function(gridRow) gridRow.Tag?.ToString = _kblMapper.KBLCavityConnectorMapper(redlining.ObjectId))
                        connectorRow.ChildBands(0).Rows.Single(Function(gridRow) gridRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString = redlining.ObjectId).Cells(1).Appearance.Image = My.Resources.Redlining
                    Next
                End If
            Case E3.Lib.Schema.Kbl.KblObjectType.Change_description
                If (ugChangeDescriptions.Rows.Where(Function(gridRow) gridRow.Tag?.ToString = redlining.ObjectId).Any()) Then
                    Me.ugChangeDescriptions.Rows.Single(Function(gridRow) gridRow.Tag?.ToString = redlining.ObjectId).Cells(0).Appearance.Image = My.Resources.Redlining
                End If
            Case E3.Lib.Schema.Kbl.KblObjectType.Module_change
                For Each moduleRow As UltraGridRow In Me.ugModules.Rows
                    If (moduleRow.HasChild) Then
                        For Each changeRow As UltraGridRow In moduleRow.ChildBands(0).Rows.Where(Function(gridRow) gridRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString = redlining.ObjectId)
                            changeRow.Cells(1).Appearance.Image = My.Resources.Redlining
                        Next
                    End If
                Next
            Case E3.Lib.Schema.Kbl.KblObjectType.Component_occurrence
                If (ugComponents.Rows.Where(Function(gridRow) gridRow.Tag?.ToString = redlining.ObjectId).Any()) Then
                    Me.ugComponents.Rows.Single(Function(gridRow) gridRow.Tag?.ToString = redlining.ObjectId).Cells(0).Appearance.Image = My.Resources.Redlining
                End If
            Case E3.Lib.Schema.Kbl.KblObjectType.Component_box_occurrence
                If (ugComponentBoxes.Rows.Where(Function(gridRow) gridRow.Tag?.ToString = redlining.ObjectId).Any()) Then
                    Me.ugComponentBoxes.Rows.Single(Function(gridRow) gridRow.Tag?.ToString = redlining.ObjectId).Cells(0).Appearance.Image = My.Resources.Redlining
                End If
            Case E3.Lib.Schema.Kbl.KblObjectType.Connector_occurrence
                If (ugConnectors.Rows.Where(Function(gridRow) gridRow.Tag?.ToString = redlining.ObjectId).Any()) Then
                    Me.ugConnectors.Rows.Single(Function(gridRow) gridRow.Tag?.ToString = redlining.ObjectId).Cells(0).Appearance.Image = My.Resources.Redlining
                End If
            Case E3.Lib.Schema.Kbl.KblObjectType.Core_occurrence
                For Each cableRow As UltraGridRow In Me.ugCables.Rows.Where(Function(gridRow) gridRow.Tag?.ToString = _kblMapper.KBLCoreCableMapper(redlining.ObjectId))
                    cableRow.ChildBands(0).Rows.Single(Function(gridRow) gridRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString = redlining.ObjectId).Cells(1).Appearance.Image = My.Resources.Redlining
                Next

                If (ugWires.Rows.Where(Function(gridRow) gridRow.Tag?.ToString = redlining.ObjectId).Any()) Then
                    Me.ugWires.Rows.Single(Function(gridRow) gridRow.Tag?.ToString = redlining.ObjectId).Cells(1).Appearance.Image = My.Resources.Redlining
                End If

                For Each netRow As UltraGridRow In Me.ugNets.Rows
                    If (netRow.HasChild) Then
                        For Each connectionRow As UltraGridRow In netRow.ChildBands(0).Rows.Where(Function(gridRow) gridRow.Tag?.ToString = redlining.ObjectId)
                            connectionRow.Cells(1).Appearance.Image = My.Resources.Redlining
                        Next
                    End If
                Next
            Case E3.Lib.Schema.Kbl.KblObjectType.Co_pack_occurrence
                If (ugCoPacks.Rows.Where(Function(gridRow) gridRow.Tag?.ToString = redlining.ObjectId).Any()) Then
                    Me.ugCoPacks.Rows.Single(Function(gridRow) gridRow.Tag?.ToString = redlining.ObjectId).Cells(0).Appearance.Image = My.Resources.Redlining
                End If
            Case E3.Lib.Schema.Kbl.KblObjectType.Fixing_occurrence
                If (ugFixings.Rows.Where(Function(gridRow) gridRow.Tag?.ToString = redlining.ObjectId).Any()) Then
                    Me.ugFixings.Rows.Single(Function(gridRow) gridRow.Tag?.ToString = redlining.ObjectId).Cells(0).Appearance.Image = My.Resources.Redlining
                End If
            Case KblObjectType.Harness_module
                If (ugModules.Rows.Where(Function(gridRow) gridRow.Tag?.ToString = redlining.ObjectId).Any()) Then
                    Me.ugModules.Rows.Single(Function(gridRow) gridRow.Tag?.ToString = redlining.ObjectId).Cells(0).Appearance.Image = My.Resources.Redlining
                End If
            Case E3.Lib.Schema.Kbl.KblObjectType.Net
                If (ugNets.Rows.Where(Function(gridRow) gridRow.Tag?.ToString = redlining.ObjectId).Any()) Then
                    Me.ugNets.Rows.Single(Function(gridRow) gridRow.Tag?.ToString = redlining.ObjectId).Cells(0).Appearance.Image = My.Resources.Redlining
                End If
            Case E3.Lib.Schema.Kbl.KblObjectType.Wire_protection_occurrence
                For Each segmentRow As UltraGridRow In Me.ugSegments.Rows
                    If (segmentRow.HasChild) Then
                        For Each protectionRow As UltraGridRow In segmentRow.ChildBands(0).Rows.Where(Function(gridRow) gridRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString = redlining.ObjectId)
                            protectionRow.Cells(1).Appearance.Image = My.Resources.Redlining
                        Next
                    End If
                Next

                For Each vertexRow As UltraGridRow In Me.ugVertices.Rows
                    If (vertexRow.HasChild) Then
                        For Each protectionRow As UltraGridRow In vertexRow.ChildBands(0).Rows.Where(Function(gridRow) gridRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString = redlining.ObjectId)
                            protectionRow.Cells(1).Appearance.Image = My.Resources.Redlining
                        Next
                    End If
                Next
            Case E3.Lib.Schema.Kbl.KblObjectType.Segment
                If (ugSegments.Rows.Where(Function(gridRow) gridRow.Tag?.ToString = redlining.ObjectId).Any()) Then
                    Me.ugSegments.Rows.Single(Function(gridRow) gridRow.Tag?.ToString = redlining.ObjectId).Cells(0).Appearance.Image = My.Resources.Redlining
                End If
            Case E3.Lib.Schema.Kbl.KblObjectType.Node
                If (ugVertices.Rows.Where(Function(gridRow) gridRow.Tag?.ToString = redlining.ObjectId).Any()) Then
                    Me.ugVertices.Rows.Single(Function(gridRow) gridRow.Tag?.ToString = redlining.ObjectId).Cells(0).Appearance.Image = My.Resources.Redlining
                End If
            Case E3.Lib.Schema.Kbl.KblObjectType.Wire_occurrence
                If (ugWires.Rows.Where(Function(gridRow) gridRow.Tag?.ToString = redlining.ObjectId).Any()) Then
                    Me.ugWires.Rows.Single(Function(gridRow) gridRow.Tag?.ToString = redlining.ObjectId).Cells(0).Appearance.Image = My.Resources.Redlining
                End If

                For Each netRow As UltraGridRow In Me.ugNets.Rows
                    If (netRow.HasChild) Then
                        For Each connectionRow As UltraGridRow In netRow.ChildBands(0).Rows.Where(Function(gridRow) gridRow.Tag?.ToString = redlining.ObjectId)
                            connectionRow.Cells(1).Appearance.Image = My.Resources.Redlining
                        Next
                    End If
                Next
        End Select
    End Sub

    Private Sub ResetRedliningIconStateInGridCells(redliningObjectId As String, redliningObjectType As KblObjectType)
        If _redliningInformation IsNot Nothing AndAlso Not _redliningInformation.Redlinings.Where(Function(redlining) redlining.ObjectId = redliningObjectId).Any() Then
            Select Case redliningObjectType
                Case E3.Lib.Schema.Kbl.KblObjectType.Accessory_occurrence
                    If (ugAccessories.Rows.Where(Function(gridRow) gridRow.Tag?.ToString = redliningObjectId).Any()) Then
                        Me.ugAccessories.Rows.Single(Function(gridRow) gridRow.Tag?.ToString = redliningObjectId).Cells(0).Appearance.Image = Nothing
                    End If
                Case E3.Lib.Schema.Kbl.KblObjectType.Assembly_part_occurrence, KblObjectType.Assembly_part
                    If (ugAssemblyParts.Rows.Where(Function(gridRow) gridRow.Tag?.ToString = redliningObjectId).Any()) Then
                        Me.ugAssemblyParts.Rows.Single(Function(gridRow) gridRow.Tag?.ToString = redliningObjectId).Cells(0).Appearance.Image = Nothing
                    End If
                Case E3.Lib.Schema.Kbl.KblObjectType.Special_wire_occurrence
                    If (ugCables.Rows.Where(Function(gridRow) gridRow.Tag?.ToString = redliningObjectId).Any()) Then
                        Me.ugCables.Rows.Single(Function(gridRow) gridRow.Tag?.ToString = redliningObjectId).Cells(0).Appearance.Image = Nothing
                    End If
                Case E3.Lib.Schema.Kbl.KblObjectType.Cavity_occurrence
                    If _kblMapper.KBLCavityConnectorMapper.ContainsKey(redliningObjectId) Then
                        For Each connectorRow As UltraGridRow In Me.ugConnectors.Rows.Where(Function(gridRow) gridRow.Tag?.ToString = _kblMapper.KBLCavityConnectorMapper(redliningObjectId))
                            connectorRow.ChildBands(0).Rows.Single(Function(gridRow) gridRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString = redliningObjectId).Cells(1).Appearance.Image = Nothing
                        Next
                    ElseIf _kblMapper.KBLContactPointConnectorMapper.ContainsKey(redliningObjectId) Then
                        For Each connectorRow As UltraGridRow In Me.ugConnectors.Rows.Where(Function(gridRow) gridRow.Tag?.ToString = _kblMapper.KBLContactPointConnectorMapper(redliningObjectId))
                            connectorRow.ChildBands(0).Rows.Single(Function(gridRow) gridRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString = redliningObjectId).Cells(1).Appearance.Image = Nothing
                        Next
                    End If
                Case E3.Lib.Schema.Kbl.KblObjectType.Change_description
                    If (ugChangeDescriptions.Rows.Where(Function(gridRow) gridRow.Tag?.ToString = redliningObjectId).Any()) Then
                        Me.ugChangeDescriptions.Rows.Single(Function(gridRow) gridRow.Tag?.ToString = redliningObjectId).Cells(0).Appearance.Image = Nothing
                    End If
                Case E3.Lib.Schema.Kbl.KblObjectType.Module_change
                    For Each moduleRow As UltraGridRow In Me.ugModules.Rows
                        If (moduleRow.HasChild) Then
                            For Each changeRow As UltraGridRow In moduleRow.ChildBands(0).Rows.Where(Function(gridRow) gridRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString = redliningObjectId)
                                changeRow.Cells(1).Appearance.Image = Nothing
                            Next
                        End If
                    Next
                Case E3.Lib.Schema.Kbl.KblObjectType.Component_occurrence
                    If (ugComponents.Rows.Where(Function(gridRow) gridRow.Tag?.ToString = redliningObjectId).Any()) Then
                        Me.ugComponents.Rows.Single(Function(gridRow) gridRow.Tag?.ToString = redliningObjectId).Cells(0).Appearance.Image = Nothing
                    End If
                Case E3.Lib.Schema.Kbl.KblObjectType.Component_box_occurrence
                    If (ugComponentBoxes.Rows.Where(Function(gridRow) gridRow.Tag?.ToString = redliningObjectId).Any()) Then
                        Me.ugComponentBoxes.Rows.Single(Function(gridRow) gridRow.Tag?.ToString = redliningObjectId).Cells(0).Appearance.Image = Nothing
                    End If
                Case E3.Lib.Schema.Kbl.KblObjectType.Connector_occurrence
                    If (ugConnectors.Rows.Where(Function(gridRow) gridRow.Tag?.ToString = redliningObjectId).Any()) Then
                        Me.ugConnectors.Rows.Single(Function(gridRow) gridRow.Tag?.ToString = redliningObjectId).Cells(0).Appearance.Image = Nothing
                    End If
                Case E3.Lib.Schema.Kbl.KblObjectType.Core_occurrence
                    For Each cableRow As UltraGridRow In Me.ugCables.Rows.Where(Function(gridRow) gridRow.Tag?.ToString = _kblMapper.KBLCoreCableMapper(redliningObjectId))
                        cableRow.ChildBands(0).Rows.Single(Function(gridRow) gridRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString = redliningObjectId).Cells(1).Appearance.Image = Nothing
                    Next

                    If (ugWires.Rows.Where(Function(gridRow) gridRow.Tag?.ToString = redliningObjectId).Any()) Then
                        Me.ugWires.Rows.Single(Function(gridRow) gridRow.Tag?.ToString = redliningObjectId).Cells(1).Appearance.Image = Nothing
                    End If

                    For Each netRow As UltraGridRow In Me.ugNets.Rows
                        If (netRow.HasChild) Then
                            For Each connectionRow As UltraGridRow In netRow.ChildBands(0).Rows.Where(Function(gridRow) gridRow.Tag?.ToString = redliningObjectId)
                                connectionRow.Cells(1).Appearance.Image = Nothing
                            Next
                        End If
                    Next
                Case E3.Lib.Schema.Kbl.KblObjectType.Co_pack_occurrence
                    If (ugCoPacks.Rows.Where(Function(gridRow) gridRow.Tag?.ToString = redliningObjectId).Any()) Then
                        Me.ugCoPacks.Rows.Single(Function(gridRow) gridRow.Tag?.ToString = redliningObjectId).Cells(0).Appearance.Image = Nothing
                    End If
                Case E3.Lib.Schema.Kbl.KblObjectType.Fixing_occurrence
                    If (ugFixings.Rows.Where(Function(gridRow) gridRow.Tag?.ToString = redliningObjectId).Any()) Then
                        Me.ugFixings.Rows.Single(Function(gridRow) gridRow.Tag?.ToString = redliningObjectId).Cells(0).Appearance.Image = Nothing
                    End If
                Case KblObjectType.Harness_module
                    If (ugModules.Rows.Where(Function(gridRow) gridRow.Tag?.ToString = redliningObjectId).Any()) Then
                        Me.ugModules.Rows.Single(Function(gridRow) gridRow.Tag?.ToString = redliningObjectId).Cells(0).Appearance.Image = Nothing
                    End If
                Case E3.Lib.Schema.Kbl.KblObjectType.Net
                    If (ugNets.Rows.Where(Function(gridRow) gridRow.Tag?.ToString = redliningObjectId).Any()) Then
                        Me.ugNets.Rows.Single(Function(gridRow) gridRow.Tag?.ToString = redliningObjectId).Cells(0).Appearance.Image = Nothing
                    End If
                Case E3.Lib.Schema.Kbl.KblObjectType.Wire_protection_occurrence
                    For Each segmentRow As UltraGridRow In Me.ugSegments.Rows
                        If (segmentRow.HasChild) Then
                            For Each protectionRow As UltraGridRow In segmentRow.ChildBands(0).Rows.Where(Function(gridRow) gridRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString = redliningObjectId)
                                protectionRow.Cells(1).Appearance.Image = Nothing
                            Next
                        End If
                    Next

                    For Each vertexRow As UltraGridRow In Me.ugVertices.Rows
                        If (vertexRow.HasChild) Then
                            For Each protectionRow As UltraGridRow In vertexRow.ChildBands(0).Rows.Where(Function(gridRow) gridRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString = redliningObjectId)
                                protectionRow.Cells(1).Appearance.Image = Nothing
                            Next
                        End If
                    Next
                Case E3.Lib.Schema.Kbl.KblObjectType.Segment
                    If (ugSegments.Rows.Where(Function(gridRow) gridRow.Tag?.ToString = redliningObjectId).Any()) Then
                        Me.ugSegments.Rows.Single(Function(gridRow) gridRow.Tag?.ToString = redliningObjectId).Cells(0).Appearance.Image = Nothing
                    End If
                Case E3.Lib.Schema.Kbl.KblObjectType.Node
                    If (ugVertices.Rows.Where(Function(gridRow) gridRow.Tag?.ToString = redliningObjectId).Any()) Then
                        Me.ugVertices.Rows.Single(Function(gridRow) gridRow.Tag?.ToString = redliningObjectId).Cells(0).Appearance.Image = Nothing
                    End If
                Case E3.Lib.Schema.Kbl.KblObjectType.Wire_occurrence
                    If (ugWires.Rows.Where(Function(gridRow) gridRow.Tag?.ToString = redliningObjectId).Any()) Then
                        Me.ugWires.Rows.Single(Function(gridRow) gridRow.Tag?.ToString = redliningObjectId).Cells(1).Appearance.Image = Nothing
                    End If

                    For Each netRow As UltraGridRow In Me.ugNets.Rows
                        If (netRow.HasChild) Then
                            For Each connectionRow As UltraGridRow In netRow.ChildBands(0).Rows.Where(Function(gridRow) gridRow.Tag?.ToString = redliningObjectId)
                                connectionRow.Cells(1).Appearance.Image = Nothing
                            Next
                        End If
                    Next
            End Select
        End If
    End Sub

    Private Sub UpdateLastChangedInformation(redlining As Redlining)
        redlining.LastChangedBy = System.Security.Principal.WindowsIdentity.GetCurrent.Name
        redlining.LastChangedOn = Now

        Me.ugRedlinings.ActiveCell.Row.Cells(NameOf(InformationHubStrings.LastChangedBy_ColumnCaption)).Value = redlining.LastChangedBy
        Me.ugRedlinings.ActiveCell.Row.Cells(NameOf(InformationHubStrings.LastChangedOn_ColumnCaption)).Value = redlining.LastChangedOn

        _informationHubEventArgs.ObjectIds = New HashSet(Of String)
        _informationHubEventArgs.ObjectIds.Add(redlining.ObjectId)
        _informationHubEventArgs.ObjectType = redlining.ObjectType

        RaiseEvent RedliningsChanged(Me, _informationHubEventArgs)
    End Sub

    Private Sub ugRedlinings_AfterExitEditMode(sender As Object, e As EventArgs) Handles ugRedlinings.AfterExitEditMode
        If (Me.ugRedlinings.ActiveCell Is Nothing) Then
            Exit Sub
        End If

        Dim redlining As Redlining = If(_redliningInformation IsNot Nothing, _redliningInformation.Redlinings.Where(Function(rdlining) rdlining.ID = Me.ugRedlinings.ActiveCell.Row.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString).FirstOrDefault, Nothing)
        If (redlining IsNot Nothing) Then
            Select Case Me.ugRedlinings.ActiveCell.Column.Key
                Case NameOf(InformationHubStrings.Classification_ColumnCaption)
                    Dim classification As RedliningClassification = HarnessAnalyzer.Redlining.ParseFromLocalizedClassificationString(Me.ugRedlinings.ActiveCell.Text)
                    If (classification <> redlining.Classification) Then
                        redlining.Classification = classification

                        UpdateLastChangedInformation(redlining)
                    End If
                Case NameOf(InformationHubStrings.Comment_ColumnCaption)
                    Dim comment As String = Me.ugRedlinings.ActiveCell.Value.ToString
                    If (comment <> redlining.Comment) Then
                        redlining.Comment = comment

                        UpdateLastChangedInformation(redlining)
                    End If
                Case NameOf(InformationHubStrings.Group_ColumnCaption)
                    Dim onGroup As String = Me.ugRedlinings.ActiveCell.ValueListResolved.GetValue(Me.ugRedlinings.ActiveCell.Text, Nothing).ToString
                    If (redlining.OnGroup Is Nothing AndAlso onGroup <> String.Empty) OrElse (onGroup <> redlining.OnGroup) Then
                        redlining.OnGroup = onGroup

                        UpdateLastChangedInformation(redlining)
                    End If
            End Select
        End If
    End Sub

    Private Sub ugRedlinings_AfterRowActivate(sender As Object, e As EventArgs) Handles ugRedlinings.AfterRowActivate
        If (Me.ugRedlinings.ActiveRow.Appearance.BackColor = Color.FromArgb(190, 190, 190)) Then
            Me.ugRedlinings.ActiveRow = Nothing
        ElseIf (Not Me.ugRedlinings.ActiveRow.Selected) Then
            Me.ugRedlinings.Selected.Rows.Add(Me.ugRedlinings.ActiveRow)
        End If
    End Sub

    Private Sub ugRedlinings_AfterSelectChange(sender As Object, e As AfterSelectChangeEventArgs) Handles ugRedlinings.AfterSelectChange
        Dim kblIds As New List(Of String)
        With Me.ugRedlinings
            .BeginUpdate()
            Dim selectedRows As New List(Of UltraGridRow)

            If (.ActiveCell IsNot Nothing) Then
                selectedRows.Add(.ActiveCell.Row)
            ElseIf (.ActiveRow IsNot Nothing) Then
                selectedRows.Add(.ActiveRow)
            ElseIf (.Selected.Rows.Count <> 0) Then
                For Each selectedRow As UltraGridRow In .Selected.Rows
                    selectedRows.Add(selectedRow)
                Next
            End If

            If _redliningInformation IsNot Nothing Then
                For Each selectedRow As UltraGridRow In selectedRows
                    If (Not kblIds.Contains(_redliningInformation.Redlinings.Single(Function(redlining) redlining.ID = selectedRow.Tag?.ToString).ObjectId)) Then
                        kblIds.Add(_redliningInformation.Redlinings.Single(Function(redlining) redlining.ID = selectedRow.Tag?.ToString).ObjectId)
                    End If
                Next
            End If

            SetInformationHubEventArgs(kblIds, Nothing, Nothing)

            If (.ActiveCell IsNot Nothing) AndAlso (Not .ActiveRowScrollRegion.VisibleRows.Contains(.ActiveCell.Row)) Then
                .ActiveRowScrollRegion.ScrollRowIntoView(.ActiveCell.Row)
            ElseIf (.ActiveRow IsNot Nothing) AndAlso (Not .ActiveRowScrollRegion.VisibleRows.Contains(.ActiveRow)) Then
                .ActiveRowScrollRegion.ScrollRowIntoView(.ActiveRow)
            End If

            .EndUpdate()
        End With

        OnHubSelectionChanged()
    End Sub

    Private Sub ugRedlinings_BeforeCellUpdate(sender As Object, e As BeforeCellUpdateEventArgs) Handles ugRedlinings.BeforeCellUpdate
        If e.Cell.Column.Key = NameOf(InformationHubStrings.Comment_ColumnCaption) AndAlso (e.NewValue.ToString = String.Empty) Then
            MessageBox.Show(String.Format(RedliningFormStrings.CommentCannotEmpty_Msg, e.Cell.Row.Index), [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
            e.Cancel = True
        End If
    End Sub

    Private Sub ugRedlinings_BeforeEnterEditMode(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles ugRedlinings.BeforeEnterEditMode
        If (Me.ugRedlinings.ActiveCell Is Nothing) Then
            e.Cancel = True
        ElseIf _redliningInformation IsNot Nothing Then
            Dim redlining As Redlining = _redliningInformation.Redlinings.Where(Function(rdlining) rdlining.ID = Me.ugRedlinings.ActiveCell.Row.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString).FirstOrDefault
            If redlining IsNot Nothing AndAlso Me.ugRedlinings.ActiveCell.Column.Index = 4 Then
                Me.ugRedlinings.ActiveCell.Column.ValueList = GetValueList(redlining)
            End If
        End If
    End Sub

    Private Sub ugRedlinings_BeforeSelectChange(sender As Object, e As BeforeSelectChangeEventArgs) Handles ugRedlinings.BeforeSelectChange
        For Each row As UltraGridRow In e.NewSelections.Rows
            If (row.Appearance.BackColor = Color.FromArgb(190, 190, 190)) Then
                e.Cancel = True
            End If
        Next
    End Sub

    Private Sub ugRedlinings_CellDataError(sender As Object, e As CellDataErrorEventArgs) Handles ugRedlinings.CellDataError
        e.RaiseErrorEvent = False
        e.RestoreOriginalValue = True
    End Sub

    Private Sub ugRedlinings_ClickCell(sender As Object, e As ClickCellEventArgs) Handles ugRedlinings.ClickCell
        If (_pressedMouseButton = System.Windows.Forms.MouseButtons.Left) AndAlso (_pressedKey <> Keys.ControlKey AndAlso _pressedKey <> Keys.ShiftKey) AndAlso (_informationHubEventArgs IsNot Nothing) AndAlso (_kblMapperForCompare IsNot Nothing OrElse e.Cell.Row.Appearance.BackColor <> Color.FromArgb(190, 190, 190)) Then
            Dim id As String = _redliningInformation?.Redlinings.Single(Function(redlining) redlining.ID = e.Cell.Row.Tag?.ToString).ObjectId
            Dim prevIds As IEnumerable(Of String) = _informationHubEventArgs.ObjectIds

            SetInformationHubEventArgs(New List(Of String)(New String() {id}), Nothing, Nothing)

            If (prevIds IsNot Nothing) AndAlso (String.Join(";", prevIds) <> String.Join(";", _informationHubEventArgs.ObjectIds)) Then
                OnHubSelectionChanged()
            End If
        ElseIf (_pressedMouseButton = System.Windows.Forms.MouseButtons.Right) AndAlso (_kblMapperForCompare Is Nothing) AndAlso (ChangeGridContextMenuVisibility(e.Cell.Row)) Then
            _contextMenuGrid.ShowPopup()
        End If
    End Sub

    Private Sub ugRedlinings_DoubleClickRow(sender As Object, e As DoubleClickRowEventArgs) Handles ugRedlinings.DoubleClickRow
        OnDoubleClickRow(sender, e)
    End Sub

    Private Sub ugRedlinings_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugRedlinings.InitializeLayout
        Me.ugRedlinings.BeginUpdate()
        Me.ugRedlinings.EventManager.AllEventsEnabled = False

        InitializeGridLayout(Nothing, e.Layout)

        e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True
        e.Layout.Override.CellClickAction = CellClickAction.Default

        For Each column As UltraGridColumn In e.Layout.Bands(0).Columns
            If (Not column.Hidden) Then
                With column
                    .CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center
                    .CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle
                    .Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center
                    .Header.Appearance.TextVAlign = Infragistics.Win.VAlign.Middle

                    Select Case .Index
                        Case 0, 1
                            .Hidden = True
                        Case 2, 3, 6, 8
                            .CellActivation = Activation.NoEdit
                        Case 4
                            .Style = ColumnStyle.DropDownList

                            Dim valueList As New ValueList

                            valueList.ValueListItems.Add(RedliningClassification.Confirmation, RedliningFormStrings.Confirmation_RedlClass)
                            valueList.FindByDataValue(RedliningClassification.Confirmation).Appearance.Image = My.Resources.RedliningConfirmation

                            valueList.ValueListItems.Add(RedliningClassification.Error, RedliningFormStrings.Error_RedlClass)
                            valueList.FindByDataValue(RedliningClassification.Error).Appearance.Image = My.Resources.RedliningError

                            valueList.ValueListItems.Add(RedliningClassification.Information, RedliningFormStrings.Information_RedlClass)
                            valueList.FindByDataValue(RedliningClassification.Information).Appearance.Image = My.Resources.RedliningInformation

                            valueList.ValueListItems.Add(RedliningClassification.Question, RedliningFormStrings.Question_RedlClass)
                            valueList.FindByDataValue(RedliningClassification.Question).Appearance.Image = My.Resources.RedliningQuestion

                            .ValueList = valueList
                        Case 7
                            If (_generalSettings Is Nothing) OrElse (Not _generalSettings.LastChangedByEditable) Then
                                .CellActivation = Activation.NoEdit
                            End If
                        Case 9
                            .Style = ColumnStyle.DropDownList
                            .ValueList = GetRedliningGroups()
                    End Select
                End With
            End If
        Next

        Me.ugRedlinings.EventManager.AllEventsEnabled = True
        Me.ugRedlinings.EndUpdate()
    End Sub

    Private Sub ugRedlinings_InitializeRow(sender As Object, e As InitializeRowEventArgs) Handles ugRedlinings.InitializeRow
        If Not e.ReInitialize Then
            e.Row.Tag = e.Row.Cells(SYSTEM_ID_COLUMN_KEY).Value
        End If
    End Sub

    Private Sub ugRedlinings_KeyDown(sender As Object, e As KeyEventArgs) Handles ugRedlinings.KeyDown
        OnGridKeyDown(Me, New GridKeyDownEventArgs(e, DirectCast(sender, UltraGrid)))
        If Not e.Handled Then
            If (e.Control) AndAlso (e.KeyCode = Keys.A) Then
                SelectAllRowsOfActiveGrid()
            ElseIf (e.KeyCode = Keys.Delete) Then
                DeleteRedlining()
            ElseIf (e.KeyCode = Keys.Escape) Then
                If (Me.ugRedlinings.Selected.Rows.Count = 0) Then
                    ClearMarkedRowsInGrids()
                Else
                    ClearSelectedRowsInGrids()
                End If

                If (SetInformationHubEventArgs(Nothing, Nothing, Me.ugRedlinings.Selected.Rows)) Then
                    OnHubSelectionChanged()
                End If
            ElseIf (e.KeyCode = Keys.Return) Then
                SetSelectedAsMarkedOriginRows(ugRedlinings)
            End If
        End If
    End Sub

    Private Sub ugRedlinings_MouseDown(sender As Object, e As MouseEventArgs) Handles ugRedlinings.MouseDown
        If (_contextMenuGrid.IsOpen) Then
            _contextMenuGrid.ClosePopup()
        End If
        _pressedMouseButton = e.Button
    End Sub

    Private Sub ugRedlinings_MouseLeave(sender As Object, e As EventArgs) Handles ugRedlinings.MouseLeave
        _messageEventArgs.StatusMessage = String.Empty
        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub ugRedlinings_MouseMove(sender As Object, e As MouseEventArgs) Handles ugRedlinings.MouseMove
        _messageEventArgs.StatusMessage = String.Format(InformationHubStrings.RowCount_Label, Me.ugRedlinings.Rows.Count, Me.ugRedlinings.Rows.FilteredInNonGroupByRowCount, Me.ugRedlinings.Rows.VisibleRowCount, Me.ugRedlinings.Selected.Rows.Count)
        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub udsRedlinings_CellDataUpdated(sender As Object, e As CellDataUpdatedEventArgs) Handles udsRedlinings.CellDataUpdated
        RaiseEvent CellValueUpdated(sender, e)
    End Sub

    Private Function GetRedlinigClassification(redliningClass As String) As RedliningClassification

        Dim res As New RedliningClassification

        If redliningClass = RedliningClassification.Confirmation.ToString Then
            res = RedliningClassification.Confirmation

        ElseIf redliningClass = RedliningClassification.Error.ToString Then
            res = RedliningClassification.Error

        ElseIf redliningClass = RedliningClassification.LengthComment.ToString Then
            res = RedliningClassification.LengthComment

        ElseIf redliningClass = RedliningClassification.GraphicalComment.ToString Then
            res = RedliningClassification.GraphicalComment

        ElseIf redliningClass = RedliningClassification.Question.ToString Then
            res = RedliningClassification.Question

        ElseIf redliningClass = RedliningClassification.Information.ToString Then
            res = RedliningClassification.Information
        End If

        Return res

    End Function
    Private Function GetValueList(red As Redlining) As ValueList

        Dim valueList As New ValueList

        valueList.ValueListItems.Add(New ValueListItem(RedliningClassification.Confirmation, RedliningFormStrings.Confirmation_RedlClass))
        valueList.FindByDataValue(RedliningClassification.Confirmation).Appearance.Image = My.Resources.RedliningConfirmation

        valueList.ValueListItems.Add(New ValueListItem(RedliningClassification.Error, RedliningFormStrings.Error_RedlClass))
        valueList.FindByDataValue(RedliningClassification.Error).Appearance.Image = My.Resources.RedliningError

        If _parentForm IsNot Nothing AndAlso Not DirectCast(_parentForm, DocumentForm).IsDocument3DActive Then
            Select Case red.ObjectType
                Case KblObjectType.Accessory, KblObjectType.Connector_occurrence, KblObjectType.Fixing, KblObjectType.Segment, KblObjectType.Node
                    valueList.ValueListItems.Add(New ValueListItem(RedliningClassification.GraphicalComment, RedliningFormStrings.Graphical_RedlClass))
                    valueList.FindByDataValue(RedliningClassification.GraphicalComment).Appearance.Image = My.Resources.RedliningGraphical
            End Select
        End If

        valueList.ValueListItems.Add(New ValueListItem(RedliningClassification.Information, RedliningFormStrings.Information_RedlClass))
            valueList.FindByDataValue(RedliningClassification.Information).Appearance.Image = My.Resources.RedliningInformation

        If (red.ObjectType <> Nothing AndAlso red.ObjectType = KblObjectType.Segment) Then
            valueList.ValueListItems.Add(New ValueListItem(RedliningClassification.LengthComment, RedliningFormStrings.LengthMod_RedlClass))
            valueList.FindByDataValue(RedliningClassification.LengthComment).Appearance.Image = My.Resources.RedliningLengthComment
        End If

        valueList.ValueListItems.Add(New ValueListItem(RedliningClassification.Question, RedliningFormStrings.Question_RedlClass))
        valueList.FindByDataValue(RedliningClassification.Question).Appearance.Image = My.Resources.RedliningQuestion

            Return valueList
    End Function
End Class