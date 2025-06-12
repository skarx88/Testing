Imports Infragistics.Win.UltraWinGrid

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class RedliningGroupsForm

    Private _redliningGroupList As RedliningGroupList
    Private _redliningInformation As RedliningInformation

    Public Property SelectedRedliningGroup As RedliningGroup

    Public Sub New(redliningInformation As RedliningInformation)
        InitializeComponent()

        _redliningInformation = redliningInformation

        Initialize()
    End Sub

    Private Sub Initialize()
        _redliningGroupList = New RedliningGroupList

        If (_redliningInformation.RedliningGroups IsNot Nothing) Then
            For Each redliningGroup As RedliningGroup In _redliningInformation.RedliningGroups
                Dim clonedRedliningGroup As New RedliningGroup
                With clonedRedliningGroup
                    .ChangeTag = redliningGroup.ChangeTag
                    .Comment = redliningGroup.Comment
                    .ID = redliningGroup.ID
                    .LastChangedBy = redliningGroup.LastChangedBy
                    .LastChangedOn = redliningGroup.LastChangedOn
                End With

                _redliningGroupList.Add(clonedRedliningGroup)
            Next
        End If

        Me.ugRedliningGroups.SyncWithCurrencyManager = False
        Me.ugRedliningGroups.DataSource = _redliningGroupList
    End Sub


    Private Sub RedliningGroupsForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If (Me.DialogResult = System.Windows.Forms.DialogResult.OK) Then
            Dim emptyChangeTagExists As Boolean = False

            For Each row As UltraGridRow In Me.ugRedliningGroups.Rows
                If (row.Cells("ChangeTag").Value.ToString = String.Empty) Then
                    MessageBox.Show(String.Format(RedliningGroupsFormStrings.ChangeTagCannotEmpty_Msg, row.Index + 1), [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)

                    emptyChangeTagExists = True

                    Exit For
                End If
            Next

            If (emptyChangeTagExists) Then
                e.Cancel = True
            Else
                If (_redliningInformation.RedliningGroups Is Nothing) Then _redliningInformation.RedliningGroups = New RedliningGroupList

                For Each redliningGroup As RedliningGroup In _redliningGroupList
                    If (_redliningInformation.RedliningGroups.ContainsRedliningGroup(redliningGroup)) Then
                        With _redliningInformation.RedliningGroups.Single(Function(rdliningGrp) rdliningGrp.ID = redliningGroup.ID)
                            If (.ChangeTag <> redliningGroup.ChangeTag) OrElse (.Comment <> redliningGroup.Comment) Then
                                .LastChangedBy = System.Security.Principal.WindowsIdentity.GetCurrent.Name
                                .LastChangedOn = Now
                            End If

                            .ChangeTag = redliningGroup.ChangeTag
                            .Comment = redliningGroup.Comment
                        End With
                    Else
                        _redliningInformation.RedliningGroups.Add(redliningGroup)
                    End If
                Next

                Dim redliningGroupsForDeletion As New RedliningGroupList

                For Each redliningGroup As RedliningGroup In _redliningInformation.RedliningGroups
                    If (Not _redliningGroupList.ContainsRedliningGroup(redliningGroup)) Then redliningGroupsForDeletion.Add(redliningGroup)
                Next

                For Each redliningGroup As RedliningGroup In redliningGroupsForDeletion
                    _redliningInformation.RedliningGroups.Remove(redliningGroup)
                Next

                If (Me.ugRedliningGroups.ActiveRow IsNot Nothing) Then
                    SelectedRedliningGroup = _redliningInformation.RedliningGroups.Single(Function(rdliningGrp) rdliningGrp.ID = Me.ugRedliningGroups.ActiveRow.Cells("ID").Value.ToString)
                ElseIf (Me.ugRedliningGroups.Selected.Rows.Count <> 0) Then
                    SelectedRedliningGroup = _redliningInformation.RedliningGroups.Single(Function(rdliningGrp) rdliningGrp.ID = Me.ugRedliningGroups.Selected.Rows(0).Cells("ID").Value.ToString)
                Else
                    SelectedRedliningGroup = Nothing
                End If
            End If
        End If
    End Sub

    Private Sub RedliningGroupsForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If (e.KeyCode = Keys.Escape) Then Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
    End Sub

    Private Sub btnAddNew_Click(sender As Object, e As EventArgs) Handles btnAddNew.Click
        Dim addedRedliningGroup As New RedliningGroup

        With Me.ugRedliningGroups
            .BeginUpdate()

            .Selected.Rows.Clear()

            _redliningGroupList.Add(addedRedliningGroup)

            .EndUpdate()
        End With

        With Me.ugRedliningGroups
            .BeginUpdate()

            For Each row As UltraGridRow In .Rows
                If (row.Cells("ID").Value.ToString = addedRedliningGroup.ID) Then row.Selected = True
            Next

            .ActiveRowScrollRegion.ScrollRowIntoView(.Selected.Rows(0))

            .EndUpdate()
        End With
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Me.ugRedliningGroups.DeleteSelectedRows()
    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
    End Sub

    Private Sub ugRedliningGroups_BeforeRowsDeleted(sender As Object, e As BeforeRowsDeletedEventArgs) Handles ugRedliningGroups.BeforeRowsDeleted
        e.DisplayPromptMsg = False

        Dim msgBoxText As String = String.Empty

        If (e.Rows.Length = 1) Then
            msgBoxText = String.Format(RedliningGroupsFormStrings.DelRedlGrp_Msg, e.Rows(0).Cells("ChangeTag").Value)
        ElseIf (e.Rows.Length > 1) Then
            msgBoxText = RedliningGroupsFormStrings.DelSelRedlGrp_Msg
        End If

        If (MessageBox.Show(msgBoxText, [Shared].MSG_BOX_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = MsgBoxResult.No) Then e.Cancel = True
    End Sub

    Private Sub ugRedliningGroups_CellDataError(sender As Object, e As CellDataErrorEventArgs) Handles ugRedliningGroups.CellDataError
        e.RaiseErrorEvent = False
        e.RestoreOriginalValue = True
    End Sub

    Private Sub ugRedliningGroups_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugRedliningGroups.InitializeLayout
        Me.ugRedliningGroups.BeginUpdate()

        With e.Layout
            .AutoFitStyle = AutoFitStyle.ResizeAllColumns
            .CaptionVisible = Infragistics.Win.DefaultableBoolean.False
            .GroupByBox.Hidden = True

            With .Override
                .AllowColMoving = AllowColMoving.NotAllowed
                .RowSelectors = Infragistics.Win.DefaultableBoolean.True
            End With

            For Each band As UltraGridBand In .Bands
                With band
                    For Each column As UltraGridColumn In .Columns
                        If (Not column.Hidden) Then
                            With column
                                .CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center
                                .CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle
                                .Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center
                                .Header.Appearance.TextVAlign = Infragistics.Win.VAlign.Middle

                                If (.Index = 0) Then
                                    .Hidden = True
                                ElseIf (.Index = 1) Then
                                    .Header.Caption = RedliningGroupsFormStrings.ChangeTag_ColCaption
                                ElseIf (.Index = 2) Then
                                    .Header.Caption = RedliningGroupsFormStrings.Comment_ColCaption
                                    .Width = 200
                                ElseIf (.Index = 3) Then
                                    .CellActivation = Activation.NoEdit
                                    .Header.Caption = RedliningFormStrings.ChangedBy_ColCaption
                                ElseIf (.Index = 4) Then
                                    .CellActivation = Activation.NoEdit
                                    .Header.Caption = RedliningFormStrings.ChangedOn_ColCaption
                                End If

                            End With
                        End If
                    Next
                End With
            Next
        End With

        Me.ugRedliningGroups.EndUpdate()
    End Sub

End Class