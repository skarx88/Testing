Imports Infragistics.Win
Imports Infragistics.Win.UltraWinGrid
Imports Zuken.E3.HarnessAnalyzer.Shared.Common

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class RedliningForm

    Private _graphicalRedlining As String
    Private _kblMapper As KblMapper
    Private _lastChangedByEditable As Boolean
    Private _objectIds As ICollection(Of String)
    Private _objectNames As IList(Of String)
    Private _objectType As KblObjectType
    Private _prevRedliningGroup As RedliningGroup
    Private _redliningInformation As RedliningInformation
    Private _redliningList As RedliningList

    Private _isCavitiyNavigatorRedlining As Boolean
    Private _isD3D As Boolean = False

    Public Sub New(kblMapper As KblMapper, lastChangedByEditable As Boolean, objectIds As ICollection(Of String), objectNames As IList(Of String), objectType As KblObjectType, redliningInformation As RedliningInformation, isD3D As Boolean, Optional redliningList As RedliningList = Nothing)
        InitializeComponent()
        _kblMapper = kblMapper
        _lastChangedByEditable = lastChangedByEditable
        _objectIds = objectIds
        _objectNames = objectNames
        _objectType = objectType
        _redliningList = redliningList
        _redliningInformation = redliningInformation
        _isD3D = isD3D
        Initialize()
    End Sub

    Public Property IsRedliningForCavitiyNavigator As Boolean '**
        Get
            Return _isCavitiyNavigatorRedlining
        End Get
        Set(value As Boolean)
            _isCavitiyNavigatorRedlining = value
        End Set
    End Property

    Private Function GetRedliningGroups() As ValueList
        Dim valueList As New ValueList
        valueList.ValueListItems.Add(Guid.Empty.ToString, $"[{RedliningFormStrings.Edit_CellButton_Caption}]").Appearance.FontData.Bold = DefaultableBoolean.True
        valueList.ValueListItems.Add(String.Empty, String.Empty)

        If (_redliningInformation.RedliningGroups IsNot Nothing) Then
            Dim redliningGroups As New Dictionary(Of String, String)

            For Each redliningGroup As RedliningGroup In _redliningInformation.RedliningGroups
                redliningGroups.TryAdd(redliningGroup.ID, redliningGroup.ChangeTag)
            Next

            Dim orderedRedliningGroups As IOrderedEnumerable(Of KeyValuePair(Of String, String)) = redliningGroups.OrderBy(Function(group) group.Value)

            For Each orderedRedliningGroup As KeyValuePair(Of String, String) In orderedRedliningGroups
                valueList.ValueListItems.Add(orderedRedliningGroup.Key, orderedRedliningGroup.Value)
            Next
        End If

        valueList.SortStyle = ValueListSortStyle.Ascending

        Return valueList
    End Function

    Private Sub Initialize()
        Me.BackColor = Color.White
        Me.Icon = My.Resources.EditRedlining

        If (_objectIds.Count = 1) Then
            Me.Text = String.Format(RedliningFormStrings.Caption1, _objectType, _objectNames(0))
        Else
            Me.Text = String.Format(RedliningFormStrings.Caption2, _objectType)
        End If

        If (_redliningList Is Nothing) Then
            _redliningList = New RedliningList

            For Each redlining As Redlining In _redliningInformation.Redlinings
                If (_objectIds.Contains(redlining.ObjectId)) Then
                    Dim clonedRedlining As New Redlining(redlining.ObjectId, redlining.ObjectName, redlining.ObjectType, redlining.Classification, redlining.Comment, redlining.IsVisible)
                    clonedRedlining.LastChangedBy = redlining.LastChangedBy
                    clonedRedlining.LastChangedOn = redlining.LastChangedOn
                    clonedRedlining.ID = redlining.ID
                    clonedRedlining.OnGroup = redlining.OnGroup

                    If (redlining.AssignedModules IsNot Nothing) Then
                        clonedRedlining.AssignedModules = New AssignedModuleList

                        For Each assignedModule As AssignedModule In redlining.AssignedModules
                            clonedRedlining.AssignedModules.Add(New AssignedModule With {.Abbreviation = assignedModule.Abbreviation, .Description = assignedModule.Description, .KEM = assignedModule.KEM, .PartNumber = assignedModule.PartNumber, .ZGS = assignedModule.ZGS})
                        Next
                    End If

                    _redliningList.Add(clonedRedlining)
                End If
            Next
        End If

        Me.ugRedlinings.SyncWithCurrencyManager = False
        Me.ugRedlinings.DataSource = _redliningList
    End Sub


    Private Sub RedliningForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If (Me.DialogResult = System.Windows.Forms.DialogResult.OK) Then
            Dim emptyCommentsExists As Boolean = False

            For Each row As UltraGridRow In Me.ugRedlinings.Rows
                If (row.Cells(NameOf(Redlining.Comment)).Value.ToString = String.Empty) Then
                    MessageBox.Show(String.Format(RedliningFormStrings.CommentCannotEmpty_Msg, row.Index + 1), [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)

                    emptyCommentsExists = True

                    Exit For
                End If
            Next

            If (emptyCommentsExists) Then
                e.Cancel = True
            Else
                For Each redlining As Redlining In _redliningList
                    If (redlining.Classification = RedliningClassification.GraphicalComment) Then
                        redlining.Comment = _graphicalRedlining
                    End If

                    If (_redliningInformation.Redlinings.Contains(redlining)) Then
                        With _redliningInformation.Redlinings.GetById(redlining.ID).Single
                            If (.Classification <> redlining.Classification) OrElse (.Comment <> redlining.Comment) OrElse (.LastChangedBy <> redlining.LastChangedBy) OrElse (.IsVisible <> redlining.IsVisible) OrElse (.OnGroup <> redlining.OnGroup) Then
                                .LastChangedBy = If(.LastChangedBy <> redlining.LastChangedBy, redlining.LastChangedBy, Security.Principal.WindowsIdentity.GetCurrent.Name)
                                .LastChangedOn = Now
                            End If

                            .Classification = redlining.Classification
                            .Comment = redlining.Comment
                            .IsVisible = redlining.IsVisible
                            .OnGroup = redlining.OnGroup
                        End With
                    Else
                        _redliningInformation.Redlinings.Add(redlining)
                    End If
                Next

                Dim redliningsForDeletion As New RedliningList

                For Each redlining As Redlining In _redliningInformation.Redlinings.Where(Function(rdl) _objectIds.Contains(rdl.ObjectId))
                    If Not _redliningList.Contains(redlining) Then
                        redliningsForDeletion.Add(redlining)
                    End If
                Next

                For Each redlining As Redlining In redliningsForDeletion
                    _redliningInformation.Redlinings.Remove(redlining)
                Next

                For Each redlining As Redlining In _redliningInformation.Redlinings
                    If (Not redlining.OnGroup?.IsNullOrEmpty) AndAlso (Not _redliningInformation.RedliningGroups.Any(Function(group) group.ID = redlining.OnGroup)) Then
                        redlining.OnGroup = String.Empty
                    End If
                Next
            End If
        End If
    End Sub

    Private Sub RedliningForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If (e.KeyCode = Keys.Escape) Then
            Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        End If
    End Sub

    Private Sub btnAddNew_Click(sender As Object, e As EventArgs) Handles btnAddNew.Click
        Dim addedRedliningIds As New List(Of String)

        With Me.ugRedlinings
            .BeginUpdate()

            .Selected.Rows.Clear()

            For objectIndex As Integer = 0 To _objectIds.Count - 1
                Dim redlining As New Redlining(_objectIds(objectIndex), _objectNames(objectIndex), _objectType, RedliningClassification.Error, String.Empty, True)
                _redliningList.Add(redlining)

                For Each m As [Module] In _kblMapper.GetModulesOfObject(redlining.ObjectId)
                    Dim assignedModule As New AssignedModule

                    With assignedModule
                        .Abbreviation = m.Abbreviation
                        .Description = m.Description

                        If m.Change.Length <> 0 Then
                            Dim changes As List(Of Change) = m.Change.Where(Function(change) change.Change_date IsNot Nothing AndAlso DateTime.TryParse(change.Change_date, New DateTime)).ToList
                            If (changes.Count > 0) Then
                                .KEM = changes.Where(Function(change) CDate(change.Change_date).Ticks = changes.Max(Function(cng) CDate(cng.Change_date).Ticks)).LastOrDefault.Change_request
                            Else
                                .KEM = m.Change.LastOrDefault.Change_request
                            End If
                        End If

                        .PartNumber = m.Part_number

                        If m.Change.Length <> 0 Then
                            Dim changes As List(Of Change) = m.Change.Where(Function(change) change.Change_date IsNot Nothing AndAlso DateTime.TryParse(change.Change_date, New DateTime)).ToList
                            If changes.Count > 0 Then
                                .ZGS = changes.Where(Function(change) CDate(change.Change_date).Ticks = changes.Max(Function(cng) CDate(cng.Change_date).Ticks)).LastOrDefault.Id
                            Else
                                .ZGS = m.Change.LastOrDefault.Id
                            End If
                        End If
                    End With

                    If (redlining.AssignedModules Is Nothing) Then
                        redlining.AssignedModules = New AssignedModuleList
                    End If

                    redlining.AssignedModules.Add(assignedModule)
                Next

                addedRedliningIds.Add(redlining.ID)
            Next

            For Each row As UltraGridRow In .Rows
                If (addedRedliningIds.Contains(row.Cells(NameOf(Redlining.ID)).Value?.ToString)) Then
                    row.Selected = True
                End If
            Next

            .EndUpdate()
        End With
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
    End Sub

    Private Sub btnCommentBulkUpdate_Click(sender As Object, e As EventArgs) Handles btnCommentBulkUpdate.Click
        If (Me.ugRedlinings.Selected.Rows.Count <> 0) Then
            Using inputForm As New InputForm(RedliningFormStrings.ModifyComment_InputBoxPrompt, [Shared].MSG_BOX_TITLE, Me.ugRedlinings.Selected.Rows(0).Cells(NameOf(Redlining.Comment)).Value.ToString)
                If (inputForm.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then
                    Dim comment As String = inputForm.Response.Trim
                    If Not comment.IsNullOrEmpty Then
                        Me.ugRedlinings.BeginUpdate()
                        For Each row As UltraGridRow In Me.ugRedlinings.Selected.Rows
                            If (Not row.Cells(NameOf(Redlining.Classification)).Value.ToString = RedliningFormStrings.Graphical_RedlClass) Then
                                row.Cells(NameOf(Redlining.Comment)).Value = comment
                            End If
                        Next

                        Me.ugRedlinings.EndUpdate()
                    End If
                End If
            End Using
        End If
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        With Me.ugRedlinings
            .BeginUpdate()
            .DeleteSelectedRows()
            .Update()
            .EndUpdate()
        End With
        Me.Update()
    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
    End Sub

    Private Sub ugRedlinings_AfterCellListCloseUp(sender As Object, e As CellEventArgs) Handles ugRedlinings.AfterCellListCloseUp
        If (e.Cell.Column.Key = NameOf(Redlining.Classification)) Then
            Me.ugRedlinings.EventManager.AllEventsEnabled = False

            If (e.Cell.Text = RedliningFormStrings.Graphical_RedlClass) Then
                If (e.Cell.Row.Cells(NameOf(Redlining.Comment)).Style <> ColumnStyle.Button) Then
                    If Not _graphicalRedlining.IsNullOrEmpty OrElse _redliningList.HasGraphicalComments Then
                        MessageBoxEx.ShowWarning(RedliningFormStrings.OnlyOneAllowed_Msg)

                        e.Cell.Value = RedliningClassification.Information.ToString
                    Else
                        e.Cell.Row.Cells(NameOf(Redlining.Comment)).Style = ColumnStyle.Button
                        e.Cell.Row.Cells(NameOf(Redlining.Comment)).Value = RedliningFormStrings.ClickBGGraphics_CellButton_Caption
                    End If
                End If
            ElseIf (e.Cell.Text = RedliningFormStrings.LengthMod_RedlClass) Then
                e.Cell.Row.Cells(NameOf(Redlining.Comment)).Style = ColumnStyle.IntegerNonNegative

                If (Not IsNumeric(e.Cell.Row.Cells(NameOf(Redlining.Comment)).Value)) Then
                    e.Cell.Row.Cells(NameOf(Redlining.Comment)).Value = 0
                End If
            Else
                If (e.Cell.Row.Cells(NameOf(Redlining.Comment)).Style = ColumnStyle.Button) Then
                    e.Cell.Row.Cells(NameOf(Redlining.Comment)).Value = String.Empty

                    _graphicalRedlining = String.Empty
                End If

                e.Cell.Row.Cells(NameOf(Redlining.Comment)).Style = ColumnStyle.Default
            End If

            Me.ugRedlinings.EventManager.AllEventsEnabled = True
        ElseIf e.Cell.Column.Key = NameOf(Redlining.OnGroup) Then
            If e.Cell.Text = $"[{RedliningFormStrings.Edit_CellButton_Caption}]" Then
                Using redliningGroupsForm As New RedliningGroupsForm(_redliningInformation)
                    redliningGroupsForm.SelectedRedliningGroup = _prevRedliningGroup

                    If (redliningGroupsForm.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then
                        e.Cell.Column.ValueList = GetRedliningGroups()

                        If (redliningGroupsForm.SelectedRedliningGroup Is Nothing) Then
                            e.Cell.Value = DirectCast(e.Cell.ValueListResolved, ValueList).ValueListItems(0).DisplayText

                            _redliningList.Single(Function(rdlining) rdlining.ID = e.Cell.Row.Cells(NameOf(Redlining.ID)).Value.ToString).OnGroup = String.Empty
                        Else
                            For Each valueListItem As IValueListItem In e.Cell.ValueListResolved.GetValueListItems
                                If (valueListItem.DataValue?.ToString = redliningGroupsForm.SelectedRedliningGroup.ID) Then
                                    e.Cell.Value = valueListItem.Text
                                End If
                            Next

                            _redliningList.Single(Function(rdlining) rdlining.ID = e.Cell.Row.Cells(NameOf(Redlining.ID)).Value.ToString).OnGroup = redliningGroupsForm.SelectedRedliningGroup.ID
                        End If

                        For Each redlining As Redlining In _redliningList
                            If (Not redlining.OnGroup.IsNullOrEmpty) AndAlso (Not _redliningInformation.RedliningGroups.Any(Function(group) group.ID = redlining.OnGroup)) Then
                                redlining.OnGroup = String.Empty
                            End If
                        Next
                    Else
                        Dim item As IValueListItem = e.Cell.ValueListResolved.GetSelectedItem
                        e.Cell.Value = If(_prevRedliningGroup Is Nothing, String.Empty, item?.Text)
                    End If
                End Using
            ElseIf (e.Cell.ValueListResolved.SelectedItemIndex > -1) Then
                Dim item As IValueListItem = e.Cell.ValueListResolved.GetSelectedItem
                e.Cell.Value = item.Text
                _redliningList.Single(Function(rdlining) rdlining.ID = e.Cell.Row.Cells(NameOf(Redlining.ID)).Value.ToString).OnGroup = item.DataValue?.ToString
            End If
        End If
    End Sub

    Private Sub ugRedlinings_BeforeCellListDropDown(sender As Object, e As CancelableCellEventArgs) Handles ugRedlinings.BeforeCellListDropDown
        If (e.Cell.Column.Key = NameOf(Redlining.OnGroup)) Then
            Dim item As IValueListItem = e.Cell.ValueListResolved.GetSelectedItem

            _prevRedliningGroup = If(e.Cell.Text = String.Empty, Nothing, _redliningInformation.RedliningGroups.Single(Function(rdliningGrp) rdliningGrp.ID = item.DataValue?.ToString))
        End If
    End Sub

    Private Sub ugRedlinings_BeforeRowsDeleted(sender As Object, e As BeforeRowsDeletedEventArgs) Handles ugRedlinings.BeforeRowsDeleted
        e.DisplayPromptMsg = False

        Dim msgBoxText As String = String.Empty

        If (e.Rows.Length = 1) Then
            If (e.Rows(0).Cells(NameOf(Redlining.Classification)).Value.ToString = RedliningClassification.GraphicalComment.ToString) Then
                msgBoxText = RedliningFormStrings.DelGraphRedl_Msg
            Else
                msgBoxText = String.Format(RedliningFormStrings.DelRedl_Msg, e.Rows(0).Cells(NameOf(Redlining.Comment)).Value)
            End If
        ElseIf (e.Rows.Length > 1) Then
            msgBoxText = RedliningFormStrings.DelSelRedl_Msg
        End If

        If (MessageBox.Show(msgBoxText, [Shared].MSG_BOX_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = MsgBoxResult.No) Then
            e.Cancel = True
        Else
            For Each row As UltraGridRow In e.Rows
                If (row.Cells(NameOf(Redlining.Classification)).Value.ToString = RedliningClassification.GraphicalComment.ToString) Then
                    _graphicalRedlining = String.Empty
                End If
            Next
        End If
    End Sub

    Private Sub ugRedlinings_CellDataError(sender As Object, e As CellDataErrorEventArgs) Handles ugRedlinings.CellDataError
        e.RaiseErrorEvent = False
        e.RestoreOriginalValue = True
    End Sub

    Private Sub ugRedlinings_ClickCellButton(sender As Object, e As CellEventArgs) Handles ugRedlinings.ClickCellButton
        Me.ugRedlinings.EventManager.AllEventsEnabled = False

        If (_graphicalRedlining = RedliningFormStrings.ClickBGGraphics_CellButton_Caption) Then
            e.Cell.Value = String.Empty
        Else
            e.Cell.Value = _graphicalRedlining
        End If

        Me.DialogResult = System.Windows.Forms.DialogResult.Retry
    End Sub

    Private Sub ugRedlinings_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugRedlinings.InitializeLayout
        Me.ugRedlinings.BeginUpdate()

        With e.Layout
            .AutoFitStyle = AutoFitStyle.ResizeAllColumns
            .CaptionVisible = Infragistics.Win.DefaultableBoolean.False
            .GroupByBox.Hidden = True

            With .Override
                .AllowColMoving = AllowColMoving.NotAllowed
                .RowSelectors = Infragistics.Win.DefaultableBoolean.True
            End With

            For Each band As UltraGridBand In .Bands
                If band.Key = NameOf(Redlining.AssignedModules) Then
                    band.Hidden = True
                Else
                    For Each column As UltraGridColumn In band.Columns
                        If Not column.Hidden Then
                            With column
                                .CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center
                                .CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle
                                .Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center
                                .Header.Appearance.TextVAlign = Infragistics.Win.VAlign.Middle

                                Select Case column.Key
                                    Case NameOf(Redlining.ObjectName)
                                        If (_objectIds.Count = 1) Then
                                            .Hidden = True
                                        Else
                                            .CellActivation = Activation.NoEdit
                                            .Header.Caption = RedliningFormStrings.ObjName_ColCaption
                                        End If
                                    Case NameOf(Redlining.Classification)
                                        .Style = ColumnStyle.DropDownList
                                        .Header.Caption = RedliningFormStrings.Classification_ColCaption
                                        Dim valueList As New ValueList

                                        valueList.ValueListItems.Add(New ValueListItem(RedliningClassification.Confirmation, RedliningFormStrings.Confirmation_RedlClass))
                                        valueList.FindByDataValue(RedliningClassification.Confirmation).Appearance.Image = My.Resources.RedliningConfirmation

                                        valueList.ValueListItems.Add(New ValueListItem(RedliningClassification.Error, RedliningFormStrings.Error_RedlClass))
                                        valueList.FindByDataValue(RedliningClassification.Error).Appearance.Image = My.Resources.RedliningError

                                        If (_objectIds.Count = 1 AndAlso Not _isD3D) Then
                                            Select Case _objectType
                                                Case E3.Lib.Schema.Kbl.KblObjectType.Accessory_occurrence, E3.Lib.Schema.Kbl.KblObjectType.Connector_occurrence, E3.Lib.Schema.Kbl.KblObjectType.Fixing_occurrence, E3.Lib.Schema.Kbl.KblObjectType.Segment, E3.Lib.Schema.Kbl.KblObjectType.Node
                                                    valueList.ValueListItems.Add(New ValueListItem(RedliningClassification.GraphicalComment, RedliningFormStrings.Graphical_RedlClass))
                                                    valueList.FindByDataValue(RedliningClassification.GraphicalComment).Appearance.Image = My.Resources.RedliningGraphical
                                            End Select
                                        End If

                                        valueList.ValueListItems.Add(New ValueListItem(RedliningClassification.Information, RedliningFormStrings.Information_RedlClass))
                                        valueList.FindByDataValue(RedliningClassification.Information).Appearance.Image = My.Resources.RedliningInformation

                                        If (_objectType = E3.Lib.Schema.Kbl.KblObjectType.Segment) Then
                                            valueList.ValueListItems.Add(New ValueListItem(RedliningClassification.LengthComment, RedliningFormStrings.LengthMod_RedlClass))
                                            valueList.FindByDataValue(RedliningClassification.LengthComment).Appearance.Image = My.Resources.RedliningLengthComment
                                        End If

                                        valueList.ValueListItems.Add(New ValueListItem(RedliningClassification.Question, RedliningFormStrings.Question_RedlClass))
                                        valueList.FindByDataValue(RedliningClassification.Question).Appearance.Image = My.Resources.RedliningQuestion

                                        .ValueList = valueList
                                    Case NameOf(Redlining.Comment)
                                        .Width = 200
                                        .Header.Caption = RedliningFormStrings.Comment_ColCaption
                                    Case NameOf(Redlining.IsVisible)
                                        .Header.Caption = RedliningFormStrings.Visible_ColCaption
                                        .Style = ColumnStyle.CheckBox
                                    Case NameOf(Redlining.LastChangedBy)
                                        If (Not _lastChangedByEditable) Then
                                            .CellActivation = Activation.NoEdit
                                        End If

                                        .Header.Caption = RedliningFormStrings.ChangedBy_ColCaption
                                    Case NameOf(Redlining.LastChangedOn)
                                        .CellActivation = Activation.NoEdit
                                        .Format = String.Format("{0} {1}", My.Application.Culture.DateTimeFormat.ShortDatePattern, My.Application.Culture.DateTimeFormat.ShortTimePattern)
                                        .Header.Caption = RedliningFormStrings.ChangedOn_ColCaption
                                    Case NameOf(Redlining.OnGroup)
                                        .Header.Caption = RedliningFormStrings.Group_ColCaption
                                        .Style = ColumnStyle.DropDownList
                                        .ValueList = GetRedliningGroups()
                                    Case Else
                                        .Hidden = True
                                End Select
                            End With
                        End If
                    Next
                End If
            Next
        End With

        Me.ugRedlinings.EndUpdate()
    End Sub

    Private Sub InitializeRedliningRowCore(row As UltraGridRow)
        If (row.Cells(NameOf(Redlining.Classification)).Value.ToString = RedliningClassification.GraphicalComment.ToString) Then
            If (_graphicalRedlining Is Nothing) Then
                _graphicalRedlining = row.Cells(NameOf(Redlining.Comment)).Value.ToString
            End If

            row.Cells(NameOf(Redlining.Comment)).Style = ColumnStyle.Button

            If (_graphicalRedlining = String.Empty) Then
                row.Cells(NameOf(Redlining.Comment)).Value = RedliningFormStrings.ClickBGGraphics_CellButton_Caption
            ElseIf (_graphicalRedlining <> RedliningFormStrings.ClickBGGraphics_CellButton_Caption) Then
                row.Cells(NameOf(Redlining.Comment)).Value = RedliningFormStrings.Edit_CellButton_Caption
            End If
        End If
    End Sub

    Private Sub ugRedlinings_InitializeRow(sender As Object, e As InitializeRowEventArgs) Handles ugRedlinings.InitializeRow
        InitializeRedliningRowCore(e.Row)
    End Sub

    Private Sub RedliningForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        If IsRedliningForCavitiyNavigator Then
            'If no redlining then ,Click add new button then set status to Error and focus it to comment.
            Dim dicOfObjIdAndIsRedliningCreated As New Dictionary(Of String, String)
            Dim selectedRows As New List(Of UltraGridRow)
            For Each row As UltraGridRow In Me.ugRedlinings.Rows
                Dim obj_id As String = row.Cells(NameOf(Redlining.ID)).Value?.ToString
                dicOfObjIdAndIsRedliningCreated.TryAdd(obj_id, obj_id)
            Next

            If dicOfObjIdAndIsRedliningCreated.Count = 0 Or _objectIds.Count > Me.ugRedlinings.Rows.Count Then
                btnAddNew.PerformClick()
            End If

            For Each row As UltraGridRow In Me.ugRedlinings.Rows
                Dim obj_id As String = row.Cells(NameOf(Redlining.ID)).Value?.ToString
                If Not dicOfObjIdAndIsRedliningCreated.ContainsKey(obj_id) Or dicOfObjIdAndIsRedliningCreated.Count = 0 Then
                    row.Cells(NameOf(Redlining.Comment)).Activate()
                    Me.ugRedlinings.PerformAction(UltraGridAction.EnterEditMode, False, False)
                    selectedRows.Add(row)
                End If
            Next

            If selectedRows.Count > 1 Then
                ' bulk comment for multiselect
                For Each row As UltraGridRow In selectedRows
                    row.Cells(NameOf(Redlining.Comment)).Activated = False
                    row.Selected = True
                Next
                btnCommentBulkUpdate.PerformClick()
            End If

        End If
    End Sub

    Friend ReadOnly Property RedliningList() As RedliningList
        Get
            Return _redliningList
        End Get
    End Property

End Class