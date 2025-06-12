Imports System.ComponentModel
Imports Infragistics.Win.UltraWinGrid
Imports VectorDraw.Professional.vdPrimaries

Namespace IssueReporting

    <Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
    Public Class EditIssuesForm

        Private _issuesLayer As vdLayer
        Private _vdraw As VectorDraw.Professional.Control.VectorDrawBaseControl
        Private _activeDrawingCanvas As DrawingCanvas
        Private _activeDocument As DocumentForm
        Private _mainUtMdiManager As Infragistics.Win.UltraWinTabbedMdi.UltraTabbedMdiManager
        Private _reportFile As ReportFile
        Private _internalSelect As Boolean = False
        Private _lastSelectedRows As New List(Of UltraGridRow)
        Private _colorRanges As ColorRangeCollection
        Private _forceClose As Boolean = False
        Private _issuesDataSet As New QMIssueReportingDataSet
        Private _cellClick As Boolean

        Public Sub New(ByRef reportFile As ReportFile, colorRanges As ColorRangeCollection)
            InitializeComponent()

            _reportFile = reportFile
            _colorRanges = colorRanges
        End Sub

        Protected Overrides Sub OnShown(e As EventArgs)
            Init()
            MyBase.OnShown(e)
        End Sub

        Protected Overrides Sub OnLoad(e As EventArgs)
            MyBase.OnLoad(e)
            ReportFileBindingSource.DataSource = _reportFile
            With My.Settings
                If .QMIssuesLastPosition <> Drawing.Point.Empty Then
                    If IsOnScreen(New Rectangle(.QMIssuesLastPosition, .QMIssuesLastSize)) Then
                        Me.Location = .QMIssuesLastPosition
                    End If
                End If

                If .QMIssuesLastSize <> Drawing.Size.Empty Then Me.Size = .QMIssuesLastSize
                Me.ugbHeaderInformation.Expanded = .QMIssuesHeaderExpanded
            End With
        End Sub

        Public Function IsOnScreen(Rec As System.Drawing.Rectangle, Optional MinPercentOnScreen As Double = 0.2) As Boolean
            Dim PixelsVisible As Double = 0

            For Each scrn As Screen In Screen.AllScreens
                Dim r As System.Drawing.Rectangle = System.Drawing.Rectangle.Intersect(Rec, scrn.WorkingArea)
                ' intersect rectangle with screen
                If r.Width <> 0 And r.Height <> 0 Then
                    ' tally visible pixels
                    PixelsVisible += (r.Width * r.Height)
                End If
            Next
            Return PixelsVisible >= (Rec.Width * Rec.Height) * MinPercentOnScreen
        End Function

        Protected Overrides Sub OnClosed(e As EventArgs)
            MyBase.OnClosed(e)

            For Each vdIssue As VdIssue In _issuesLayer.GetReferenceObjects.OfType(Of VdIssue)()
                _vdraw.ActiveDocument.Model.Entities.RemoveItem(vdIssue)
                vdIssue.Layer = Nothing
                Me._activeDrawingCanvas.IssueMapper.Remove(vdIssue.KblId)
            Next

            _issuesLayer.Document.Layers.RemoveItem(_issuesLayer)
            _vdraw = Nothing
            _issuesLayer = Nothing
        End Sub

        Private Sub SaveSettings()
            With My.Settings
                .QMIssuesLastPosition = Me.Location
                .QMIssuesLastSize = Me.Size
                .QMIssuesHeaderExpanded = Me.ugbHeaderInformation.Expanded

                Dim colStr As New System.Text.StringBuilder
                For Each sortedColumn As UltraGridColumn In ugIssues.DisplayLayout.Bands(0).SortedColumns
                    Dim descending As Boolean = sortedColumn.SortIndicator = SortIndicator.Descending
                    colStr.Append(String.Concat(sortedColumn.Key, ";", descending.ToString, "|"))
                Next
                .QMEditIssuesSortedColums = colStr.ToString.TrimEnd("|"c)
            End With
        End Sub

        Protected Overrides Sub OnClosing(e As CancelEventArgs)
            MyBase.OnClosing(e)
            If Not e.Cancel Then
                SaveSettings()
                e.Cancel = Not SaveReportFile()
            End If
        End Sub

        Private Function SaveReportFile() As Boolean
            Me.ugIssues.UpdateData()
            If _issuesDataSet.HasChanges Then
                Try
                    _issuesDataSet.Issues.SubmitChanges()
                    If _reportFile.HasChanges Then
                        Return SaveReportFile2()
                    End If
                    Return True
                Catch ex As Exception
                    Dim buttons As MessageBoxButtons = MessageBoxButtons.AbortRetryIgnore
                    If _forceClose Then buttons = MessageBoxButtons.RetryCancel
                    Select Case MessageBox.Show(String.Format(ErrorStrings.QM_ErrorSavingIssueReport, _reportFile.FileName, ex.Message), [Shared].MSG_BOX_TITLE, buttons)
                        Case System.Windows.Forms.DialogResult.Retry
                            Return SaveReportFile()
                        Case System.Windows.Forms.DialogResult.Ignore, System.Windows.Forms.DialogResult.Cancel
                            Return True
                    End Select
                End Try
                Return False
            End If
            Return True
        End Function

        Private Function SaveReportFile2() As Boolean
            Try
                _reportFile.Save()
                Return True
            Catch ex As Exception
                Dim buttons As MessageBoxButtons = MessageBoxButtons.AbortRetryIgnore
                If _forceClose Then buttons = MessageBoxButtons.RetryCancel
                Select Case MessageBox.Show(String.Format(ErrorStrings.QM_ErrorSavingIssueReport, _reportFile.FileName, ex.Message), [Shared].MSG_BOX_TITLE, buttons)
                    Case System.Windows.Forms.DialogResult.Retry
                        Return SaveReportFile2
                    Case System.Windows.Forms.DialogResult.Ignore, System.Windows.Forms.DialogResult.Cancel
                        Return True
                End Select
            End Try
            Return False
        End Function


        Private Sub Init()
            _vdraw = My.Application.MainForm.ActiveDocument.ActiveDrawingCanvas.vdCanvas.BaseControl
            _activeDrawingCanvas = My.Application.MainForm.ActiveDocument.ActiveDrawingCanvas
            _activeDocument = My.Application.MainForm.ActiveDocument
            _mainUtMdiManager = My.Application.MainForm.utmmMain

            AddHandler _activeDrawingCanvas.CanvasSelectionChanged, AddressOf _activeCanvas_SelectionChanged
            AddHandler _activeDrawingCanvas.CanvasMouseClick, AddressOf _activeCanvas_MouseClick
            AddHandler _mainUtMdiManager.TabClosing, AddressOf _utmmMain_TabClosing
            AddHandler _activeDocument._informationHub.HubSelectionChanged, AddressOf _activeDocument_HubSelectionChanged

            _issuesLayer = New vdLayer(_vdraw.ActiveDocument)
            _vdraw.ActiveDocument.Layers.Add(_issuesLayer)
            InitIssues()
        End Sub

        Private Sub InitIssues()
            'get all other text's (not from table) in the drawing
            Dim myGroupMapper As List(Of IDictionary(Of VdSVGGroup, VdSVGGroup)) = My.Application.MainForm.ActiveDocument.ActiveDrawingCanvas.GroupMapper.Values.ToList
            Dim otherAverageTextHeight As Single = myGroupMapper.SelectMany(Function(valList) valList.Values).Where(Function(vd) vd.SVGType <> SvgType.table.ToString AndAlso vd.SVGType <> SvgType.row.ToString AndAlso vd.SVGType <> SvgType.cell.ToString).SelectMany(Function(vd) vd.Figures.OfType(Of VdTextEx)()).Average(Function(txt) CSng(txt.Height))
            Dim vdIssues As New Dictionary(Of String, List(Of VdIssue))

            For Each Issue As Issue In _reportFile.Issues
                If _activeDocument.KBL.KBLIdMapper.ContainsKey(Issue.ObjectReference) Then
                    Dim correspondingKblIds As ICollection(Of String) = _activeDocument.KBL.KBLIdMapper(Issue.ObjectReference)
                    For Each kblId As String In correspondingKblIds
                        With My.Application.MainForm.ActiveDocument.ActiveDrawingCanvas.GroupMapper
                            If (.ContainsKey(kblId)) Then
                                Dim table As VdSVGGroup = .Item(kblId).Values.ToList.Where(Function(grp) grp.SVGType = SvgType.table.ToString).FirstOrDefault
                                For Each grp As VdSVGGroup In .Item(kblId).Values.ToList
                                    If Not grp.IsTableOrRedlining Then
                                        Dim vdIssue As VdIssue = AddNewVdIssue(grp, Issue)
                                        vdIssue.visibility = If(_activeDocument.InactiveObjects.Any(Function(objs) objs.Contains(kblId)), vdFigure.VisibilityEnum.Invisible, vdFigure.VisibilityEnum.Visible)
                                        Issue.vdIssues.Add(vdIssue)
                                        vdIssue.vdReference = grp
                                        If table IsNot Nothing Then
                                            vdIssue.TextHeight = CSng(FindSvgElement(Of VdTextEx)(table).Height)
                                        ElseIf otherAverageTextHeight <> 0 Then
                                            vdIssue.TextHeight = otherAverageTextHeight
                                        End If
                                        vdIssues.AddOrUpdate(kblId, vdIssue)
                                    End If
                                Next
                            End If
                        End With
                    Next
                End If
                Dim issRow As QMIssueReportingDataSet.IssuesRow = _issuesDataSet.Issues.AddIssueRow(Issue)
            Next

            For Each vdIssKV As KeyValuePair(Of String, List(Of VdIssue)) In vdIssues
                Dim i As Integer = -1
                For Each vdIss As VdIssue In vdIssKV.Value
                    i += 1
                    vdIss.CalculateCirclePosition(i, vdIssKV.Value.Count)
                Next
            Next

            _issuesDataSet.AcceptChanges()
            _reportFile.AcceptChanges()

            Me.IssuesBindingSource.DataSource = _issuesDataSet
            My.Application.MainForm.ActiveDocument.ActiveDrawingCanvas.vdCanvas.BaseControl.Update()
        End Sub

        Private Function FindSvgElement(Of T)(group As VdSVGGroup) As T
            For Each elem As T In group.Figures.OfType(Of T)()
                Return elem
            Next
            For Each cGrp As VdSVGGroup In group.ChildGroups
                Dim cElem As T = FindSvgElement(Of T)(cGrp)
                If cElem IsNot Nothing Then
                    Return cElem
                End If
            Next
            Return Nothing
        End Function

        Private Function AddNewVdIssue(vdReference As VdSVGGroup, issue As Issue) As VdIssue
            Dim ent As New IssueReporting.VdIssue(issue)
            ent.SetUnRegisterDocument(My.Application.MainForm.ActiveDocument.ActiveDrawingCanvas.vdCanvas.BaseControl.ActiveDocument)
            ent.setDocumentDefaults()
            ent.vdReference = vdReference
            ent.SVGType = SvgType.Undefined.ToString
            ent.FillColor = _colorRanges.GetColor(issue.NofOccurrences)
            ent.Layer = _issuesLayer
            ent.Origin.z = -999
            My.Application.MainForm.ActiveDocument.ActiveDrawingCanvas.vdCanvas.BaseControl.ActiveDocument.Model.Entities.Add(ent)
            My.Application.MainForm.ActiveDocument.ActiveDrawingCanvas.IssueMapper.GetOrAdd(vdReference.KblId, Function() New List(Of VdSVGGroup)).Add(ent)

            Return ent
        End Function

        Private Sub _utmmMain_TabClosing(sender As Object, e As Infragistics.Win.UltraWinTabbedMdi.CancelableMdiTabEventArgs)
            If Not e.Cancel Then
                If e.Tab.Form Is _activeDocument Then
                    _forceClose = True
                    'HINT: _activeDocument.FormClosing ist too late. (after some business logic in _activeDocument.FormClosed is already done) -> the closed event of th editIssuesForm is caught and tries to remove the filter which is no longer possible
                    Me.Close()
                End If
            End If
        End Sub

        Private Sub _activeCanvas_MouseClick(sender As Object, e As MouseEventArgs)
            SelectionChangedAction()
        End Sub

        Private Sub _activeCanvas_SelectionChanged(sender As Object)
            SelectionChangedAction()
        End Sub

        Private Sub _activeDocument_HubSelectionChanged(sender As Object, e As InformationHubEventArgs)
            SelectionChangedAction()
        End Sub

        Private Sub SelectionChangedAction()
            _internalSelect = True
            Dim canvas As DrawingCanvas = My.Application.MainForm.ActiveDocument.ActiveDrawingCanvas
            Dim canvasSel As Dictionary(Of String, VdIssue) = canvas.CanvasSelection.OfType(Of VdIssue).GroupBy(Function(vdIss) vdIss.Issue.Id).ToDictionary(Function(grp) grp.Key, Function(grp) grp.First)

            Me.ugIssues.BeginUpdate()
            Me.ugIssues.Selected.Rows.Clear()

            If canvasSel.Count > 0 Then
                For Each row As UltraGridRow In ugIssues.Rows
                    If Not Me.ugIssues.Selected.Rows.Contains(row) Then
                        If canvasSel.ContainsKey(CType(CType(CType(row.ListObject, Data.DataRowView).Row, QMIssueReportingDataSet.IssuesRow).ListObject, Issue).Id) Then
                            Me.ugIssues.Selected.Rows.Add(row)
                        End If
                    End If
                Next

                If ugIssues.Selected.Rows.Count > 0 Then
                    ugIssues.ActiveRow = CType(Me.ugIssues.Selected.Rows.All.Last, UltraGridRow)
                End If
            End If

            Me.ugIssues.EndUpdate()
            _internalSelect = False
        End Sub

        Private Sub ugIssues_AfterCellUpdate(sender As Object, e As Infragistics.Win.UltraWinGrid.CellEventArgs) Handles ugIssues.AfterCellUpdate
            Dim rV As Data.DataRowView = CType(e.Cell.Row.ListObject, Data.DataRowView)
            Dim row As QMIssueReportingDataSet.IssuesRow = CType(rV.Row, QMIssueReportingDataSet.IssuesRow)
            If (e.Cell.Column.Key = QMIssueReportingDataSet.Issues.ConfirmationCommentColumn.ColumnName) Then
                If row.IsConfirmedByNull AndAlso row.IsConfirmationCommentNull Then
                    row.SetDateOfConfirmationNull()
                Else
                    If row.IsDateOfConfirmationNull Then
                        row.DateOfConfirmation = Now
                    End If
                    If row.IsConfirmedByNull Then
                        row.ConfirmedBy = Environment.UserName
                    End If
                End If
            End If

            If (e.Cell.Column.Key = QMIssueReportingDataSet.Issues.ConfirmedByColumn.ColumnName) Then
                If row.IsConfirmedByNull AndAlso row.IsConfirmationCommentNull Then
                    row.SetDateOfConfirmationNull()
                Else
                    If row.IsDateOfConfirmationNull Then
                        row.DateOfConfirmation = Now
                    End If
                End If
            End If
        End Sub

        Private Sub ugIssues_AfterSelectChange(sender As Object, e As AfterSelectChangeEventArgs) Handles ugIssues.AfterSelectChange
            If Not _internalSelect Then
                If Me.ugIssues.ActiveRow Is Nothing OrElse (Not Me.ugIssues.ActiveRow.IsTemplateAddRow AndAlso Not Me.ugIssues.ActiveRow.IsUnmodifiedTemplateAddRow) Then
                    Dim selectedVdIssues As VdIssue() = Me.ugIssues.Selected.Rows.Cast(Of UltraGridRow).SelectMany(Function(row) CType(CType(CType(row.ListObject, Data.DataRowView).Row, QMIssueReportingDataSet.IssuesRow).ListObject, Issue).vdIssues).Distinct.ToArray

                    My.Application.MainForm.ActiveDocument.Cursor = Cursors.WaitCursor
                    My.Application.MainForm.ActiveDocument.OnCanvasSelectionChanged(True, selectedVdIssues.ToVdSelection)
                    If (TypeOf My.Application.MainForm.ActiveDocument.utcDocument.ActiveTab.TabPage.Controls(0) Is DrawingCanvas) Then
                        DirectCast(My.Application.MainForm.ActiveDocument.utcDocument.ActiveTab.TabPage.Controls(0), DrawingCanvas).InformationHubSelectionChanged(selectedVdIssues.Select(Function(iss) iss.KblId).Distinct.ToList, KblObjectType.Undefined, issueIds:=selectedVdIssues.Select(Function(iss) iss.Issue.Id))
                    End If
                    My.Application.MainForm.ActiveDocument.Cursor = Cursors.Default
                End If
            End If
        End Sub

        Private Sub ugIssues_BeforeSelectChange(sender As Object, e As BeforeSelectChangeEventArgs) Handles ugIssues.BeforeSelectChange
            _lastSelectedRows = ugIssues.Selected.Rows.Cast(Of UltraGridRow).ToList
        End Sub

        Private Sub ugIssues_KeyPress(sender As Object, e As KeyPressEventArgs) Handles ugIssues.KeyPress
            If ugIssues.ActiveCell IsNot Nothing Then
                If e.KeyChar = ChrW(13) Then
                    If ugIssues.ActiveCell.IsInEditMode Then
                        ugIssues.PerformAction(UltraGridAction.ExitEditMode)
                    ElseIf ugIssues.ActiveCell.CanEnterEditMode AndAlso ugIssues.ActiveCell.ActivationResolved = Activation.AllowEdit Then
                        ugIssues.PerformAction(UltraGridAction.EnterEditMode)
                    End If
                ElseIf Not Char.IsControl(e.KeyChar) Then
                    If Not ugIssues.ActiveCell.IsInEditMode AndAlso ugIssues.ActiveCell.CanEnterEditMode AndAlso ugIssues.ActiveCell.ActivationResolved = Activation.AllowEdit Then
                        If ugIssues.PerformAction(UltraGridAction.EnterEditMode) Then
                            With ugIssues.ActiveCell
                                If .EditorResolved.SupportsSelectableText Then
                                    .EditorResolved.SelectionStart = 0
                                    .EditorResolved.SelectionLength = .EditorResolved.TextLength
                                End If
                                If TypeOf .EditorResolved Is Infragistics.Win.EditorWithMask Then
                                    ' just clear the selected text and let the grid
                                    ' forward the keypress to the editor
                                    .EditorResolved.SelectedText = String.Empty
                                Else
                                    .EditorResolved.SelectedText = New String(e.KeyChar, 1)
                                    ' mark the event as handled so the grid doesn't process it
                                    e.Handled = True
                                End If
                            End With
                        End If
                    End If
                End If
            End If
        End Sub

        Private Sub ugIssues_KeyDown(sender As Object, e As KeyEventArgs) Handles ugIssues.KeyDown
            If e.Control AndAlso e.KeyCode = Keys.A Then
                ugIssues.Selected.Cells.AddRange(ugIssues.Rows.SelectMany(Function(rw) rw.Cells.Cast(Of UltraGridCell)()).ToArray)
            ElseIf e.KeyCode = Keys.Delete Then
                If Not ugIssues.Selected.Cells.Cast(Of UltraGridCell)().Any(Function(cell) cell.CanEnterEditMode) Then
                    e.Handled = True
                End If
            End If
        End Sub

        Private Sub ugIssues_AfterEnterEditMode(sender As Object, e As EventArgs) Handles ugIssues.AfterEnterEditMode
            DeleteEntfToolStripMenuItem.Enabled = False
            EditValueReturnToolStripMenuItem.Enabled = False
        End Sub

        Private Sub ugIssues_AfterExitEditMode(sender As Object, e As EventArgs) Handles ugIssues.AfterExitEditMode
            DeleteEntfToolStripMenuItem.Enabled = True
            EditValueReturnToolStripMenuItem.Enabled = True
        End Sub

        Private Sub DeleteEntfToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteEntfToolStripMenuItem.Click
            ugIssues.PerformAction(UltraGridAction.DeleteCells)
        End Sub

        Private Sub EditValueReturnToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EditValueReturnToolStripMenuItem.Click
            ugIssues_KeyPress(ugIssues, New KeyPressEventArgs(ChrW(13)))
        End Sub

        Private Sub CopyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyToolStripMenuItem.Click
            ugIssues.PerformAction(UltraGridAction.Copy)
        End Sub

        Private Sub CutValueToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CutValueToolStripMenuItem.Click
            ugIssues.PerformAction(UltraGridAction.Cut)
        End Sub

        Private Sub PasteValuesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PasteValuesToolStripMenuItem.Click
            ugIssues.PerformAction(UltraGridAction.Paste)
        End Sub

        Private Sub ugIssues_DoubleClickCell(sender As Object, e As DoubleClickCellEventArgs) Handles ugIssues.DoubleClickCell
            Dim noActiveCell As Boolean = True
            If Me.ugIssues.ActiveCell IsNot Nothing AndAlso Me.ugIssues.ActiveCell.IsInEditMode Then
                noActiveCell = Me.ugIssues.PerformAction(UltraGridAction.ExitEditMode)
            End If

            If noActiveCell Then
                If Not e.Cell.IsInEditMode AndAlso e.Cell.CanEnterEditMode AndAlso ugIssues.ActiveCell.ActivationResolved = Activation.AllowEdit Then
                    ugIssues.ActiveCell = e.Cell
                    ugIssues.PerformAction(UltraGridAction.EnterEditMode)
                End If
            End If

        End Sub

        Private Sub ContextMenuStrip1_Opening(sender As Object, e As CancelEventArgs) Handles ContextMenuStrip1.Opening
            If _cellClick Then
                DeleteEntfToolStripMenuItem.Enabled = ugIssues.ActiveCell IsNot Nothing AndAlso ugIssues.ActiveCell.CanEnterEditMode AndAlso ugIssues.ActiveCell.ActivationResolved = Activation.AllowEdit
                PasteValuesToolStripMenuItem.Enabled = ugIssues.ActiveCell IsNot Nothing AndAlso ugIssues.ActiveCell.CanEnterEditMode AndAlso E3.Lib.DotNet.Expansions.Devices.My.Computer.Clipboard.ContainsText AndAlso ugIssues.ActiveCell.ActivationResolved = Activation.AllowEdit
                EditValueReturnToolStripMenuItem.Enabled = ugIssues.ActiveCell IsNot Nothing AndAlso Not ugIssues.ActiveCell.IsInEditMode AndAlso ugIssues.ActiveCell.CanEnterEditMode AndAlso ugIssues.ActiveCell.ActivationResolved = Activation.AllowEdit
                CutValueToolStripMenuItem.Enabled = ugIssues.ActiveCell IsNot Nothing AndAlso ugIssues.ActiveCell.CanEnterEditMode AndAlso ugIssues.ActiveCell.ActivationResolved = Activation.AllowEdit
            Else
                e.Cancel = True
            End If
        End Sub

        Private Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
            If (_activeDocument.KBL.GetChanges.Any()) Then
                ExportFileDialog.FileName = String.Format("{0}{1}{2}_{3}_{4}_{5}.xlsx", Now.Year, Format(Now.Month, "00"), Format(Now.Day, "00"), Replace(_activeDocument.KBL.HarnessPartNumber, " ", String.Empty), _activeDocument.KBL.GetChanges.Max(Function(change) change.Id), QualityIssuesStrings.FileNameSuffix)
            Else
                ExportFileDialog.FileName = String.Format("{0}{1}{2}_{3}_{4}.xlsx", Now.Year, Format(Now.Month, "00"), Format(Now.Day, "00"), Replace(_activeDocument.KBL.HarnessPartNumber, " ", String.Empty), QualityIssuesStrings.FileNameSuffix)
            End If

            If ExportFileDialog.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                Try
                    UltraGridExcelExporter1.Export(Me.ugIssues, ExportFileDialog.FileName)
                    If MessageBox.Show(String.Format(QualityIssuesStrings.SuccessfullyExportedMsg, ExportFileDialog.FileName), [Shared].MSG_BOX_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = System.Windows.Forms.DialogResult.Yes Then
                        ProcessEx.Start(ExportFileDialog.FileName)
                    End If
                Catch ex As Exception
                    MessageBox.Show(ex.Message, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        End Sub

        Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
            Me.Close()
        End Sub

        Private Sub ugIssues_InitializeRow(sender As Object, e As InitializeRowEventArgs) Handles ugIssues.InitializeRow
            Dim issRow As QMIssueReportingDataSet.IssuesRow = CType(CType(e.Row.ListObject, Data.DataRowView).Row, IssueReporting.QMIssueReportingDataSet.IssuesRow)
            Dim iss As Issue = CType(issRow.ListObject, Issue)

            If Not IsInDocument(iss) Then
                e.Row.Activation = Activation.Disabled
                e.Row.Appearance.FontData.Strikeout = Infragistics.Win.DefaultableBoolean.True
            End If
        End Sub

        Friend Shared Function IsInDocument(iss As Issue, Optional checkTableOrRedlining As Boolean = False) As Boolean
            If iss.vdIssues.Count > 0 OrElse checkTableOrRedlining Then
                With My.Application.MainForm.ActiveDocument
                    Dim kblIds As New HashSet(Of String)
                    If .KBL.KBLIdMapper.ContainsKey(iss.ObjectReference) Then
                        kblIds = .KBL.KBLIdMapper(iss.ObjectReference)
                    End If

                    For Each id As String In kblIds
                        If .ActiveDrawingCanvas.GroupMapper.ContainsKey(id) Then
                            If Not checkTableOrRedlining Then
                                Return True
                            Else
                                For Each vdSVGGroup As VdSVGGroup In .ActiveDrawingCanvas.GroupMapper(id).Values.ToList
                                    If Not vdSVGGroup.IsTableOrRedlining Then
                                        Return True
                                    End If
                                Next
                            End If
                        End If
                    Next
                End With
            End If
            Return False
        End Function

        Private Sub ugIssues_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugIssues.InitializeLayout
            With Me.ugIssues.DisplayLayout.Bands(0)
                With .Columns("Id")
                    .SortComparer = New IssueIdComparer
                    .Band.SortedColumns.Clear()
                    If Not String.IsNullOrEmpty(My.Settings.QMEditIssuesSortedColums) Then
                        Dim sortedColumns As String() = My.Settings.QMEditIssuesSortedColums.Split("|"c)
                        For Each colKeyValue As String In sortedColumns
                            Dim kv As String() = colKeyValue.Split(";"c)
                            .Band.SortedColumns.Add(kv(0), Boolean.Parse(kv(1)))
                        Next
                    Else
                        .Band.SortedColumns.Add(.Key, False)
                    End If
                End With
                With .Columns("ConfirmedBy")
                    .CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.CellSelect
                    If My.Application.MainForm.GeneralSettings.LastChangedByEditable Then
                        .CellActivation = Activation.AllowEdit
                    Else
                        .CellActivation = Activation.NoEdit
                    End If
                End With
            End With
        End Sub

        Private Sub ugIssues_MouseDown(sender As Object, e As MouseEventArgs) Handles ugIssues.MouseDown
            If e.Button = System.Windows.Forms.MouseButtons.Right Then
                _cellClick = False
                Dim element As Infragistics.Win.UIElement = ugIssues.DisplayLayout.UIElement.ElementFromPoint(e.Location)
                If element IsNot Nothing Then
                    Dim cell As UltraGridCell = CType(element.GetContext(GetType(UltraGridCell)), UltraGridCell)
                    If cell IsNot Nothing Then
                        _cellClick = True
                        cell.Activate()
                        If Not Me.ugIssues.Selected.Cells.Contains(cell) Then
                            ugIssues.Selected.Cells.Clear()
                            cell.Selected = True
                        End If
                    End If
                End If
            End If
        End Sub
    End Class

End Namespace