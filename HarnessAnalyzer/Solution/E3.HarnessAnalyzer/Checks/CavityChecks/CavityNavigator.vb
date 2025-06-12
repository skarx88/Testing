Imports System.ComponentModel
Imports devDept.Eyeshot
Imports devDept.Eyeshot.Entities
Imports Infragistics.Win
Imports Infragistics.Win.UltraWinGrid
Imports Infragistics.Win.UltraWinToolbars
Imports VectorDraw.Professional.vdCollections
Imports VectorDraw.Professional.vdPrimaries
Imports Zuken.E3.HarnessAnalyzer.Checks.Cavities.Views
Imports Zuken.E3.HarnessAnalyzer.Checks.Cavities.Views.Document
Imports Zuken.E3.HarnessAnalyzer.Checks.Cavities.Views.Model
Imports Zuken.E3.HarnessAnalyzer.D3D.Shared
Imports Zuken.E3.HarnessAnalyzer.Shared
Imports Zuken.E3.Lib.Eyeshot.Model

Namespace Checks.Cavities

    <Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
    Public Class CavityNavigator

        Private WithEvents _model As ModelView
        Private WithEvents _document As Views.Document.DocumentView
        Private WithEvents _parentForm As Form
        Private _selectedRowsBeforeOnCheckEnter As UltraGridRow()
        Private _overCheckCellInSelection As Boolean
        Private _parentFormState As FormWindowState
        Private _ctxSourceControl As Control
        Private _isSelectingCavityGridRowsInternally As Boolean = False
        Private _isSyncingSelectionToModel As Boolean = False

        Public Sub New()
            Try
                InitializeComponent()
            Catch ex As Exception
                If Not ex.OnDebugCheckForVectorDrawEvaluationError Then
                    Throw
                Else
                    ' Skip evaluation errors is active, try to proceed as far as possible, for debugging reasons
                End If
            End Try

            If NaviCavitiesGrid IsNot Nothing Then
                For Each mapp As KeyActionMappingBase In Me.NaviCavitiesGrid.KeyActionMappings.Cast(Of KeyActionMappingBase).Where(Function(km) km.KeyCode = Keys.Space AndAlso CType(km.ActionCode, UltraGridAction) = UltraGridAction.ToggleRowSel).ToArray
                    Me.NaviCavitiesGrid.KeyActionMappings.Remove(mapp)
                Next
            End If

            InitVdDraw()
            InitD3D()
        End Sub

        Private Sub InitD3D()
            Dim initResult As InitDefaultsResult = Design3D.InitDefaults(initObjManipulatorManager:=False, actionMode:=devDept.Eyeshot.actionType.None) ' HINT: init after all viewports are added
            If initResult.IsSuccess Then
                Design3D.OrientationMode = devDept.Graphics.orientationType.UpAxisZ
            End If
        End Sub

        Private Sub InitDocument()
            UltraTabPageControl3D.Tab.Visible = My.Application.MainForm.HasView3DFeature AndAlso (_document?.Has3DData).GetValueOrDefault
            If _document IsNot Nothing Then

                If NaviCavitiesGrid IsNot Nothing Then
                    Me.NaviCavitiesGrid.SyncWithCurrencyManager = True
                End If

                If NaviConnectorsGrid IsNot Nothing Then
                    Me.NaviConnectorsGrid.SyncWithCurrencyManager = False ' HINT: see NaviConnectorsGrid_AfterRowActivate why it has been deactivated
                End If

                Dim selectedKblIds As String() = _document.Canvas?.GetSelectedKblIds
                If selectedKblIds.Length = 0 Then
                    If NaviConnectorsGrid IsNot Nothing Then
                        With NaviConnectorsGrid
                            If .Rows.Count > 0 Then
                                ResetSelectedConnectorRowsTo(.Rows(0))
                            End If
                        End With
                    End If

                    If NaviCavitiesGrid IsNot Nothing Then
                        ResetSelectedCavitiesRowsTo(Nothing)
                    End If
                Else
                    Dim selected As New List(Of UltraGridRow)
                    For Each row As UltraGridRow In Me.NaviConnectorsGrid.Rows
                        If selectedKblIds.Contains(CType(row.ListObject, ConnectorView).KblId) Then
                            selected.Add(row)
                        End If
                    Next

                    Me.NaviConnectorsGrid.ActiveRow = selected.LastOrDefault
                End If
            End If
        End Sub

        Private Function InitVdDraw() As Boolean
            If vdConnectorView IsNot Nothing Then
                With Me.vdConnectorView
                    .AllowDrop = False
                    .EnsureDocument()
                    .InputDevice_TouchGesture.Actions = VectorDraw.Professional.Actions.TouchGestureActions.Pan Or VectorDraw.Professional.Actions.TouchGestureActions.Zoom
                    With .ActiveDocument
                        .UndoHistory.PushEnable(False)
                        .DisableZoomOnResize = True
                        .EnableAutoGripOn = False
                        .EnableUrls = False

                        With .GlobalRenderProperties
                            .AxisSize = 10
                            .CrossSize = 8
                            .GridColor = Color.Black
                            .SelectingCrossColor = Color.Transparent
                            .SelectingWindowColor = Color.Transparent
                        End With

                        .GridMode = False
                        .OrbitActionKey = VectorDraw.Professional.vdObjects.vdDocument.KeyWithMouseStroke.None
                        .OsnapDialogKey = Keys.None
                        .Palette.Background = Color.White
                        .ShowUCSAxis = False
                        .UrlActionKey = Keys.None
                        .Selections.Add(New vdSelection)
                        .EnableToolTips = False
                    End With
                End With
                Return True
            End If
            Return False
        End Function

        Private Sub CavWiresBindingSource_DataSourceChanged(sender As Object, e As EventArgs) Handles CavWiresBindingSource.DataSourceChanged
            Me.ToolStripComboBox.ComboBox.DisplayMember = NameOf(Views.Model.CavityWireView.WireName)
            Me.ToolStripComboBox.ComboBox.DataSource = CavWiresBindingSource
            Me.BindingNavigatorCavities.BindingSource = CavWiresBindingSource
        End Sub

        Friend Property Document As Views.Document.DocumentView
            Get
                Return _document
            End Get
            Set(value As Views.Document.DocumentView)
                If value IsNot Nothing Then
                    Me.ConnectorsBindingSource.DataSource = value.Model
                    _model = value.Model
                Else
                    Me.ConnectorsBindingSource.DataSource = New ModelView(Nothing)
                    If vdConnectorView.ActiveDocument IsNot Nothing Then
                        Me.vdConnectorView.ActiveDocument.Model.Entities.RemoveAll()
                    End If
                    _model = Nothing
                End If
                _document = value
                InitDocument()
            End Set
        End Property

        Private Sub ComboBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles ToolStripComboBox.KeyDown
            If e.KeyCode = Keys.Enter Then
                NaviCavitiesGrid.Focus()
            End If
        End Sub

        Private Sub ToolStripCheckButton_Click(sender As Object, e As EventArgs) Handles ToolStripCheckButton.Click
            ToggleSelectedRows(NaviCavitiesGrid, True)
        End Sub

        Private Sub dgvCavWires_KeyDown(sender As Object, e As KeyEventArgs) Handles NaviCavitiesGrid.KeyDown
            Select Case e.KeyCode
                Case Keys.Space
                    If e.Modifiers = Keys.None Then
                        _overCheckCellInSelection = False
                        ToggleSelectedRows(NaviCavitiesGrid, True)
                    End If
                Case Keys.A
                    If e.Control Then
                        _overCheckCellInSelection = False
                        NaviCavitiesGrid.Selected.Rows.AddRange(NaviCavitiesGrid.Rows.Cast(Of UltraGridRow).ToArray)
                    End If
            End Select
        End Sub

        Private Sub ToggleSelectedRows(grid As UltraGrid, Optional moveNext As Boolean = False)
            Dim rows As List(Of UltraGridRow) = grid.Selected.Rows.Cast(Of UltraGridRow).ToList
            If grid.ActiveRow IsNot Nothing AndAlso Not rows.Contains(grid.ActiveRow) Then
                rows.Add(grid.ActiveRow)
            End If
            If rows.Count > 0 Then
                grid.BeginUpdate()
                ToggleCheckStateOnSelectedConnector(rows.ToArray)
                If rows.Count <= 1 AndAlso moveNext Then
                    Me.MoveNext(grid)
                End If
                grid.EndUpdate(True)
                grid.UpdateData()
            End If
        End Sub

        Private Sub MoveNext(grid As UltraGrid)
            grid.PerformAction(UltraGridAction.NextRow)
        End Sub

        Private Sub ToggleCheckStateOnSelectedConnector(ParamArray rows() As UltraGridRow)
            If Me.SelectedConnector IsNot Nothing Then
                If rows IsNot Nothing Then
                    Me.SelectedConnector.ToggleCheckStates(rows.Select(Function(r) CType(r.ListObject, CavityWireView)).ToArray)
                Else
                    Me.SelectedConnector.ToggleCheckStates(Nothing)
                End If
            End If
        End Sub

        Private Sub NaviConnectorsGrid_AfterRowActivate(sender As Object, e As EventArgs) Handles NaviConnectorsGrid.AfterRowActivate
            UpdateVectorConnectorView(TryCast(CType(sender, UltraGrid).ActiveRow?.ListObject, ConnectorView))
        End Sub

        Private Sub UpdateVectorConnectorView(conn As ConnectorView)
            With vdConnectorView
                .ActiveDocument.Model.Entities.Cast(Of vdFigure).ForEach(Sub(fig) fig.visibility = vdFigure.VisibilityEnum.Invisible)
                Design3D.Entities.Clear()

                If conn IsNot Nothing AndAlso conn.Visible Then
                    Me.grpBoxConnector.Text = conn.Name
                    Dim tpl As Tuple(Of vdFigure, IEntity()) = conn.GetOrAddGraphicEntities(Me)
                    If tpl.Item1 IsNot Nothing Then
                        tpl.Item1.visibility = vdFigure.VisibilityEnum.Visible
                        ConnViewZooomExtents()
                    End If

                    If tpl.Item2 IsNot Nothing AndAlso tpl.Item2.Length > 0 Then
                        Design3D.Entities.AddRange(tpl.Item2)
                        Design3D.Entities.OfType(Of IBaseModelEntityEx).ForEach(Sub(ent) ent.Update())
                        Design3D.Entities.Regen()
                        If Design3D.Created Then
                            For Each vp As Viewport In Design3D.Viewports
                                vp.SetView(vp.InitialView)
                            Next
                            Design3D.ZoomFit()
                        End If
                        Design3D.Invalidate()
                    End If
                Else
                    Me.grpBoxConnector.Text = "_"
                End If

                .Invalidate()
            End With
        End Sub

        Private Sub ConnViewZooomExtents()
            With vdConnectorView
                .ActiveDocument.ZoomExtents()
                .ActiveDocument.ZoomScale(1.1)
                .Invalidate()
            End With
        End Sub

        Private Sub NaviGridCavitites_MouseDown(sender As Object, e As ClickCellEventArgs) Handles NaviCavitiesGrid.ClickCell
            If IsStateColumn(e.Cell.Column) AndAlso e.Cell.Row.ListObject IsNot Nothing Then
                NaviCavitiesGrid.BeginUpdate()
                With CType(e.Cell.Row.ListObject, CavityWireView)
                    .ToggleCheckState()
                    SelectedConnector.CheckCavitites(_selectedRowsBeforeOnCheckEnter.Select(Function(rw) CType(rw.ListObject, CavityWireView)), .CheckState)
                End With
                NaviCavitiesGrid.EndUpdate(True)
            End If
        End Sub

        Public ReadOnly Property SelectedConnector As ConnectorView
            Get
                If Me.NaviConnectorsGrid IsNot Nothing AndAlso Not Me.NaviConnectorsGrid.IsDisposed Then
                    Return TryCast(Me.NaviConnectorsGrid.ActiveRow.ListObject, ConnectorView)
                End If
                Return Nothing
            End Get
        End Property

        Private Sub NaviGridCavitites_MouseEnterElement(sender As Object, e As UIElementEventArgs) Handles NaviCavitiesGrid.MouseEnterElement
            If e.Element IsNot Nothing Then
                Dim cell As UltraGridCell = CType(e.Element.GetContext(GetType(UltraGridCell)), UltraGridCell)
                If cell IsNot Nothing Then
                    If IsStateColumn(cell.Column) AndAlso NaviCavitiesGrid?.Selected?.Rows IsNot Nothing Then
                        _selectedRowsBeforeOnCheckEnter = NaviCavitiesGrid.Selected.Rows.OfType(Of UltraGridRow).ToArray
                        _overCheckCellInSelection = cell.Row.Selected
                    End If
                End If
            End If
        End Sub

        Private Function IsStateColumn(column As UltraGridColumn) As Boolean
            Return column.Key = NameOf(CavityWireView.StateImage)
        End Function

        Private Sub NaviGridCavitites_MouseLeaveElement(sender As Object, e As UIElementEventArgs) Handles NaviCavitiesGrid.MouseLeaveElement
            If e.Element IsNot Nothing Then
                Dim cell As UltraGridCell = CType(e.Element.GetContext(GetType(UltraGridCell)), UltraGridCell)
                If cell IsNot Nothing Then
                    _selectedRowsBeforeOnCheckEnter = Array.Empty(Of UltraGridRow)()

                    If IsStateColumn(cell.Column) Then
                        _overCheckCellInSelection = False
                    End If
                End If
            End If
        End Sub

        Private Sub NaviGridCavitites_BeforeSelectChange(sender As Object, e As BeforeSelectChangeEventArgs) Handles NaviCavitiesGrid.BeforeSelectChange
            If Not _isSelectingCavityGridRowsInternally Then
                If _overCheckCellInSelection Then
                    e.Cancel = True
                Else
                    _selectedRowsBeforeOnCheckEnter = e.NewSelections.Rows.OfType(Of UltraGridRow).ToArray
                End If
            End If
        End Sub

        Private Sub ToolStripButton5_Click(sender As Object, e As EventArgs) Handles ToolStripButton5.Click
            If Me.ConnectorsBindingSource.DataSource IsNot Nothing Then
                NaviCavitiesGrid.Invalidate()
                NaviCavitiesGrid.UpdateData()
            End If
        End Sub

        Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
            If Me.ConnectorsBindingSource.DataSource IsNot Nothing Then
                NaviCavitiesGrid.Invalidate()
                NaviCavitiesGrid.UpdateData()
            End If
        End Sub

        Private Sub ToolStripComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ToolStripComboBox.SelectedIndexChanged
            If Me.ConnectorsBindingSource.DataSource IsNot Nothing Then
                NaviCavitiesGrid.Invalidate()
                NaviCavitiesGrid.UpdateData()
            End If
        End Sub

        Private Sub NaviGridCavitites_InitializeRow(sender As Object, e As InitializeRowEventArgs) Handles NaviCavitiesGrid.InitializeRow, NaviConnectorsGrid.InitializeRow
            If TypeOf e.Row.ListObject Is Views.BaseView Then
                Dim prevHidden As Boolean = e.Row.Hidden
                With CType(e.Row.ListObject, Views.BaseView)
                    .InitializeRow(e.Row)
                End With

                If TypeOf e.Row.ListObject Is CavityWireView Then
                    If e.Row.ListObject Is Me.CavWiresBindingSource.Current AndAlso e.Row.Hidden <> prevHidden Then
                        If e.Row.Hidden Then
                            Me.ToolStripComboBox.Text = String.Empty
                        Else
                            Me.ToolStripComboBox.Text = CType(e.Row.ListObject, CavityWireView).WireName
                        End If

                    End If

                    If (_document?.RedLiningInformation.Redlinings.Where(Function(redlining) redlining.ObjectId = CType(e.Row.ListObject, CavityWireView).KblCavityId Or redlining.ObjectId = CType(e.Row.ListObject, CavityWireView).KblContactPointId).Count <> 0) Then
                        e.Row.Cells(0).Appearance.Image = My.Resources.Redlining
                    Else
                        e.Row.Cells(0).Appearance.Image = Nothing
                    End If

                End If

                If TypeOf e.Row.ListObject Is ConnectorView Then
                    If e.Row.ListObject Is ConnectorsBindingSource.Current AndAlso prevHidden <> e.Row.Hidden Then
                        Me.UpdateVectorConnectorView(CType(e.Row.ListObject, ConnectorView))
                    End If

                    If _document.RedLiningInformation IsNot Nothing Then
                        If (_document?.RedLiningInformation.Redlinings.Where(Function(redlining) redlining.ObjectId = CType(e.Row.ListObject, ConnectorView).KblId).Count <> 0) Then
                            e.Row.Cells(3).Appearance.Image = My.Resources.Redlining
                        Else
                            e.Row.Cells(3).Appearance.Image = Nothing
                        End If
                    End If
                End If
            End If
        End Sub

        Private Sub NaviConnectorsGrid_KeyDown(sender As Object, e As KeyEventArgs) Handles NaviConnectorsGrid.KeyDown
            If e.KeyCode = Keys.Space Then
                If e.Modifiers = Keys.None Then
                    NaviCavitiesGrid.Focus()
                    dgvCavWires_KeyDown(NaviCavitiesGrid, e)
                End If
            End If
        End Sub

        Private Sub NaviConnectorsGrid_AfterSortChange(sender As Object, e As BandEventArgs) Handles NaviConnectorsGrid.AfterSortChange
            Static isSortSynching As Boolean = False
            If Not isSortSynching Then
                Try
                    isSortSynching = True
                    If e.Band.Key = NameOf(ModelView.Connectors) Then
                        If e.Band.SortedColumns.Exists(NameOf(ConnectorView.StateImage)) Then
                            e.Band.SortedColumns.Add(NameOf(ConnectorView.CheckState), e.Band.SortedColumns(NameOf(ConnectorView.StateImage)).SortIndicator = SortIndicator.Descending)
                        End If
                    End If
                Finally
                    isSortSynching = False
                End Try
            End If
        End Sub

        Private Sub _model_CavitiesVisibilityChanged(sender As Object, e As EventArgs) Handles _model.CavitiesVisibilityChanged
            If Me.NaviCavitiesGrid.Rows IsNot Nothing Then
                Me.NaviCavitiesGrid.Rows.Refresh(RefreshRow.FireInitializeRow)
            End If
        End Sub

        Private Sub _model_ConnectorsVisibilityChanged(sender As Object, e As EventArgs) Handles _model.ConnectorsVisibilityChanged
            If Me.NaviConnectorsGrid.Rows IsNot Nothing Then
                Me.NaviConnectorsGrid?.Rows.Refresh(RefreshRow.FireInitializeRow)
            End If
        End Sub

        Private Sub NaviGridCavitites_BeforePerformAction(sender As Object, e As BeforeUltraGridPerformActionEventArgs) Handles NaviCavitiesGrid.BeforePerformAction, NaviConnectorsGrid.BeforePerformAction
            If e.UltraGridAction = UltraGridAction.Copy Then
                Dim txt As New System.Text.StringBuilder
                Dim rows As New HashSet(Of UltraGridRow)
                If DirectCast(sender, UltraGrid).ActiveRow IsNot Nothing Then
                    rows.Add(DirectCast(sender, UltraGrid).ActiveRow)
                End If

                For Each row As UltraGridRow In DirectCast(sender, UltraGrid).Selected.Rows
                    rows.Add(row)
                Next

                For Each row As UltraGridRow In rows
                    txt.AppendLine()
                    Dim rowTxt As New System.Text.StringBuilder
                    For Each cell As UltraGridCell In row.Cells.Cast(Of UltraGridCell).OrderBy(Function(c) c.Column.Header.VisiblePosition)
                        If Not cell.Column.Hidden AndAlso cell.Column.IgnoreMultiCellOperation <> DefaultableBoolean.True Then
                            rowTxt.Append(String.Format("{0}{1}", cell.Value, vbTab))
                        End If
                    Next
                    txt.Append(rowTxt.ToString.TrimEnd(vbTab.ToCharArray))
                Next

                If txt.Length > 0 Then
                    E3.Lib.DotNet.Expansions.Devices.My.Computer.Clipboard.SetText(txt.ToString.TrimStart(vbCrLf.ToArray))
                    e.Cancel = True
                End If
            End If
        End Sub

        Private Sub UltraSplitter1_SplitterDragCompleted(sender As Object, e As Misc.SplitterDragCompletedEventArgs) Handles UltraSplitter1.SplitterDragCompleted
            ConnViewZooomExtents()
        End Sub

        Private Sub CavityNavigator_ParentChanged(sender As Object, e As EventArgs) Handles Me.ParentChanged
            _parentForm = Me.FindForm
            If _parentForm IsNot Nothing Then
                _parentFormState = _parentForm.WindowState
            End If
        End Sub

        Private Sub _parentForm_ResizeEnd(sender As Object, e As EventArgs) Handles _parentForm.ResizeEnd
            ConnViewZooomExtents()
        End Sub

        Private Sub _parentForm_SizeChanged(sender As Object, e As EventArgs) Handles _parentForm.SizeChanged
            If _parentFormState <> _parentForm.WindowState Then
                _parentFormState = _parentForm.WindowState
                OnParentFormWindowStateChanged()
            End If
        End Sub

        Private Sub OnParentFormWindowStateChanged()
            ConnViewZooomExtents()
        End Sub

        Private Sub NaviGridCavitites_AfterSelectChange(sender As Object, e As AfterSelectChangeEventArgs) Handles NaviCavitiesGrid.AfterSelectChange
            If Not _isSelectingCavityGridRowsInternally Then ' if selection source was document-model we don't sync back only if selection did come from grid directly
                OnNaviCavitiesGridSelectionChanged(e)
            End If
        End Sub

        Protected Overridable Sub OnNaviCavitiesGridSelectionChanged(e As AfterSelectChangeEventArgs)
            If Me.Visible AndAlso My.Application.MainForm.GeneralSettings.AutoSyncCavityChecksSelection Then
                SyncGridSelectionToModel() ' HINT: dont enable attached events that sync back from model to grid because we are comming from grid with selection
            End If
        End Sub

        Friend Sub SyncGridSelectionToModel()
            If Not _isSyncingSelectionToModel Then
                _isSyncingSelectionToModel = True
                If _model IsNot Nothing Then
                    With _model.Selected
                        .Reset(SelectedObjects.ToList)
                    End With
                End If
                _isSyncingSelectionToModel = False
            End If
        End Sub

        Friend Sub ResetAllCheckStates()
            For Each connView As ConnectorView In _document.Model.Connectors
                connView.CheckCavitites(CheckState.Indeterminate, True)
            Next
        End Sub

        ReadOnly Property SelectedObjects As IEnumerable(Of BaseView)
            Get
                Dim selectedConnectorObjects As BaseView() = Array.Empty(Of BaseView)()
                Dim selectedCavityObjects As BaseView() = Array.Empty(Of BaseView)()

                If NaviConnectorsGrid?.Selected?.Rows IsNot Nothing Then
                    selectedConnectorObjects = Me.NaviConnectorsGrid.Selected.Rows.Cast(Of UltraGridRow).Select(Function(rw) rw.ListObject).OfType(Of BaseView).ToArray
                End If

                If NaviConnectorsGrid?.Selected?.Rows IsNot Nothing Then
                    selectedCavityObjects = Me.NaviCavitiesGrid.Selected.Rows.Cast(Of UltraGridRow).Select(Function(rw) rw.ListObject).OfType(Of BaseView).ToArray
                End If

                Return selectedConnectorObjects.Concat(selectedCavityObjects)
            End Get
        End Property

        Private Sub ResetSelectedConnectorRowsTo(row As UltraGridRow, Optional activate As Boolean = True)
            ResetSelectedConnectorRowsTo({row}, If(activate, row, Nothing))
        End Sub

        Private Sub ResetSelectedConnectorRowsTo(rows() As UltraGridRow, activateRow As UltraGridRow)
            Static isSelecting As Boolean = False
            If Not isSelecting Then
                isSelecting = True
                Try
                    NaviConnectorsGrid.BeginUpdate()
                    NaviConnectorsGrid.Selected.Rows.Clear()

                    If Me.NaviConnectorsGrid.DisplayLayout.Override.SelectTypeRow = SelectType.Single OrElse Me.NaviConnectorsGrid.DisplayLayout.Override.SelectTypeRow = SelectType.SingleAutoDrag Then
                        NaviConnectorsGrid.Selected.Rows.Add(rows.Where(Function(r) r IsNot Nothing).FirstOrDefault)
                    Else
                        NaviConnectorsGrid.Selected.Rows.AddRange(rows.Where(Function(r) r IsNot Nothing).ToArray)
                    End If

                    NaviConnectorsGrid.EndUpdate()
                    NaviConnectorsGrid.ActiveRow = activateRow
                Finally
                    isSelecting = False
                End Try
            End If
        End Sub

        Private Sub ResetSelectedCavitiesRowsTo(row As UltraGridRow, Optional activate As Boolean = True)
            ResetSelectedCavitiesRowsTo({row}, If(activate, row, Nothing))
        End Sub

        Private Sub ResetSelectedCavitiesRowsTo(rows As UltraGridRow(), activateRow As UltraGridRow)
            Static isSelecting As Boolean = False
            If Not isSelecting Then
                Dim oldSelectedCount As Integer = NaviConnectorsGrid.Selected.Rows.Count
                _isSelectingCavityGridRowsInternally = True
                isSelecting = True
                Try
                    NaviCavitiesGrid.BeginUpdate()
                    NaviCavitiesGrid.Selected.Rows.Clear()

                    If Me.NaviCavitiesGrid.DisplayLayout.Override.SelectTypeRow = SelectType.Single OrElse Me.NaviCavitiesGrid.DisplayLayout.Override.SelectTypeRow = SelectType.SingleAutoDrag Then
                        NaviCavitiesGrid.Selected.Rows.Add(rows.Where(Function(r) r IsNot Nothing).FirstOrDefault)
                    Else
                        NaviCavitiesGrid.Selected.Rows.AddRange(rows.Where(Function(r) r IsNot Nothing).ToArray)
                    End If

                    NaviCavitiesGrid.ActiveRow = activateRow
                    NaviCavitiesGrid.EndUpdate()
                Finally
                    If oldSelectedCount > 0 OrElse rows.Length > 0 Then
                        OnNaviCavitiesGridSelectionChanged(New AfterSelectChangeEventArgs(Nothing))
                    End If
                    _isSelectingCavityGridRowsInternally = False
                    isSelecting = False
                End Try
            End If
        End Sub

        Friend Function SetRowSelection(kblIds As IEnumerable(Of String)) As Boolean
            Static isSettingRowSelection As Boolean = False
            Try
                If Not isSettingRowSelection Then
                    isSettingRowSelection = True
                    Dim matchingConnectorRow As UltraGridRow = Nothing
                    Dim hashList As New HashSet(Of String)(kblIds.Distinct)
                    If Not NaviConnectorsGrid.IsDisposed Then
                        With NaviConnectorsGrid
                            matchingConnectorRow = .Rows.OfType(Of UltraGridRow).Where(Function(r) hashList.Contains(CType(r.ListObject, ConnectorView).KblId)).FirstOrDefault
                            If matchingConnectorRow IsNot Nothing Then
                                ResetSelectedConnectorRowsTo(matchingConnectorRow)
                            End If
                        End With
                    End If

                    If Not NaviCavitiesGrid.IsDisposed Then
                        With NaviCavitiesGrid
                            Dim potentialCavityRows As New List(Of UltraGridRow)
                            Dim matchingCavityRow As UltraGridRow = Nothing
                            For Each row As UltraGridRow In .Rows
                                Dim cav As Checks.Cavities.Views.Model.CavityWireView = CType(row.ListObject, CavityWireView)
                                'HINT find the matching row even if there are multiple wires in the cavity
                                If Not String.IsNullOrEmpty(cav.KblWireId) AndAlso hashList.Contains(cav.KblWireId) Then
                                    matchingCavityRow = row
                                ElseIf (Not String.IsNullOrEmpty(cav.KblContactPointId) AndAlso hashList.Contains(cav.KblContactPointId)) Then
                                    matchingCavityRow = row
                                ElseIf (hashList.Contains(cav.KblCavityId)) Then
                                    potentialCavityRows.Add(row)
                                End If
                            Next

                            If matchingCavityRow IsNot Nothing Then
                                ResetSelectedCavitiesRowsTo(matchingCavityRow)
                            ElseIf (potentialCavityRows.Count > 0) Then
                                ResetSelectedCavitiesRowsTo(potentialCavityRows(0))
                            ElseIf (matchingConnectorRow IsNot Nothing) Then
                                If (NaviCavitiesGrid.Rows.Count > 0) Then
                                    ResetSelectedCavitiesRowsTo(NaviCavitiesGrid.Rows(0))
                                Else
                                    ResetSelectedCavitiesRowsTo(Nothing)
                                End If
                            End If
                        End With

                        If Not _document?.Model.IsSelecting Then ' if selection source was document-model we don't sync back only if selection did come from grid
                            SyncGridSelectionToModel()
                        End If
                        Return True
                    End If
                End If
                Return False
            Finally
                isSettingRowSelection = False
            End Try
            Return False
        End Function

        Private Sub _document_ModelSelectionChanged(sender As Object, e As EventArgs) Handles _document.ModelSelectionChanged
            If Not _isSyncingSelectionToModel Then ' if selection source was  sync-from-grid-to-model we don't sync back only to grid again, only if selection did come from directly from document model
                Using Me.NaviConnectorsGrid.ProtectProperty(NameOf(Me.NaviConnectorsGrid.SyncWithCurrencyManager), True)
                    SetRowSelection(_document.Model.Selected.GetKblIdsFromViews)
                End Using
            End If
        End Sub

        Private Sub _document_CanvasSelectionChanged(sender As Object, e As EventArgs) Handles _document.CanvasSelectionChanged
            If Not _isSyncingSelectionToModel Then ' if selection source was sync-from-grid-to-model we don't sync back only to grid again, only if selection did come from directly from document model
                SetRowSelection(_document.Canvas.Selected)
            End If
        End Sub

        Private Sub NaviGridCavitites_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles NaviCavitiesGrid.InitializeLayout
            With e.Layout.Bands(0)
                If .Columns.Exists(Columns.CavityName) Then
                    With .Columns(Columns.CavityName)
                        .SortComparer = New CavityNumberGridCellSortComparer
                        .SortIndicator = SortIndicator.Ascending
                    End With
                    .PerformAutoResizeColumns(False, PerformAutoSizeType.AllRowsInBand, True)
                End If
            End With
        End Sub

        Private Sub NaviConnectorsGrid_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles NaviConnectorsGrid.InitializeLayout
            With e.Layout.Bands(0)
                .PerformAutoResizeColumns(False, PerformAutoSizeType.AllRowsInBand, True)
            End With
        End Sub

        Private Sub _document_ActiveModulesChanged(sender As Object, e As ActiveModulesChangedEventArgs) Handles _document.ActiveModulesChanged
            ' HINT: update module settings only when visible (so the process what user is doing is ongoing... -> process is virtually finished when cavityNavigator/CavityChecks-Form is closed for the user)
            If Me.Visible Then
                With _document.Model.Settings
                    .Update()
                    .ActiveHarnessConfigurationId = e.ActiveHarnessConfigurationId
                End With
            End If
        End Sub

        Private Sub _document_RedliningChanged(sender As Object, e As EventArgs) Handles _document.RedliningChanged
            With Me.NaviCavitiesGrid
                .BeginUpdate()
                .Rows.Refresh(RefreshRow.FireInitializeRow)
                .EndUpdate()
            End With
            With Me.NaviConnectorsGrid
                .BeginUpdate()
                .Rows.Refresh(RefreshRow.FireInitializeRow)
                .EndUpdate()
            End With
        End Sub

        Private Sub UltraToolbarsManager1_BeforeToolDropdown(sender As Object, e As UltraWinToolbars.BeforeToolDropdownEventArgs) Handles UltraToolbarsManager1.BeforeToolDropdown
            If TypeOf e.Tool Is PopupMenuTool Then
                _ctxSourceControl = e.SourceControl
                If TypeOf e.SourceControl Is UltraGrid Then
                    With CType(e.SourceControl, UltraGrid)
                        .BeginUpdate()
                        Dim element As UIElement = .DisplayLayout.UIElement.ElementFromPoint(.PointToClient(Form.MousePosition))
                        Dim row As UltraGridRow = If(element IsNot Nothing, TryCast(element.GetContext(GetType(UltraGridRow)), UltraGridRow), Nothing)
                        If row IsNot Nothing Then
                            If Not row.Selected Then
                                If e.SourceControl Is NaviCavitiesGrid Then
                                    ResetSelectedCavitiesRowsTo(row)
                                ElseIf e.SourceControl Is NaviConnectorsGrid Then
                                    ResetSelectedConnectorRowsTo(row)
                                Else
                                    Throw New NotImplementedException($"TypeOf Of Grid ({CType(e.SourceControl, UltraGrid).Name}) Not implemented")
                                End If
                            Else
                                row.Activated = True
                            End If
                        End If
                        .EndUpdate(True)
                        e.Cancel = element Is Nothing OrElse row Is Nothing
                    End With
                End If
            End If
        End Sub

        Private Sub UltraToolbarsManager1_ToolClick(sender As Object, e As ToolClickEventArgs) Handles UltraToolbarsManager1.ToolClick
            Select Case e.Tool.Key
                Case Buttons.ToggleCheckState   ' ButtonTool
                    ToggleCheckState_Click()
                Case Buttons.ResetCheckState    ' ButtonTool
                    ResetCheckState_Click()
                Case Buttons.SelectAll          ' ButtonTool
                    SelectAll_Click()
                Case Buttons.ResetZoom          ' ButtonTool
                    ResetVdViewZoom_Click()
                Case Buttons.JumpTo
                    JumpTo_Click()
                Case Buttons.EditRedlining
                    EditRedlining_Click()
            End Select
        End Sub

        Private Sub SelectAll_Click()
            If _ctxSourceControl Is NaviCavitiesGrid Then
                ResetSelectedCavitiesRowsTo(Me.NaviCavitiesGrid.Rows.OfType(Of UltraGridRow).ToArray, Nothing)
            ElseIf _ctxSourceControl Is NaviConnectorsGrid Then
                ResetSelectedConnectorRowsTo(Me.NaviConnectorsGrid.Rows.OfType(Of UltraGridRow).ToArray, Nothing)
            Else
                Throw New NotImplementedException($"TypeOf Of Grid ({CType(_ctxSourceControl, UltraGrid).Name}) Not implemented")
            End If
        End Sub

        Private Sub ToggleCheckState_Click()
            If _ctxSourceControl Is NaviCavitiesGrid Then
                With NaviCavitiesGrid
                    Me.ToggleCheckStateOnSelectedConnector(.Selected.Rows.Cast(Of UltraGridRow).ToArray)
                End With
            ElseIf _ctxSourceControl Is NaviConnectorsGrid Then
                Me.ToggleCheckStateOnSelectedConnector(Nothing)
            Else
                Throw New NotImplementedException($"TypeOf Of Grid ({CType(_ctxSourceControl, UltraGrid).Name}) Not implemented")
            End If
        End Sub

        Private Sub ResetCheckState_Click()
            If _ctxSourceControl Is NaviCavitiesGrid Then
                With NaviCavitiesGrid
                    If Me.SelectedConnector IsNot Nothing AndAlso .Selected?.Rows IsNot Nothing Then
                        Me.SelectedConnector.CheckCavitites(.Selected.Rows.Cast(Of UltraGridRow).Select(Function(row) CType(row.ListObject, CavityWireView)), CheckState.Indeterminate, True)
                    End If
                End With
            ElseIf _ctxSourceControl Is NaviConnectorsGrid Then
                With NaviConnectorsGrid
                    If Me.SelectedConnector IsNot Nothing AndAlso .Selected?.Rows IsNot Nothing Then
                        Me.SelectedConnector.CheckCavitites(CheckState.Indeterminate, True)
                    End If
                End With
            End If
        End Sub

        Private Sub JumpTo_Click(Optional grid As UltraGrid = Nothing)
            If grid Is Nothing Then
                grid = CType(_ctxSourceControl, UltraGrid)
            End If
            _model.Selected.Reset(grid.Selected.Rows.Cast(Of UltraGridRow).Select(Function(row) row.ListObject).OfType(Of BaseView))
        End Sub

        Private Sub EditRedlining_Click()
            JumpTo_Click()
            _document.RaiseInitializeRedliningDialog()
        End Sub

        Private Sub ResetVdViewZoom_Click()
            ConnViewZooomExtents()
        End Sub

        Private Sub NaviGridCavitites_MouseClick(sender As Object, e As MouseEventArgs) Handles NaviCavitiesGrid.MouseClick
            If e.Button = MouseButtons.Right Then
                Dim element As UIElement = NaviCavitiesGrid.DisplayLayout.UIElement.ElementFromPoint(e.Location)
                Dim cell As UltraGridCell = TryCast(element.GetContext(GetType(UltraGridCell)), UltraGridCell)
                Dim row As UltraGridRow = TryCast(element.GetContext(GetType(UltraGridRow)), UltraGridRow)
                If row IsNot Nothing Then
                    If Not row.Selected Then
                        ResetSelectedCavitiesRowsTo(row)
                    End If

                    UltraToolbarsManager1.ShowPopup("ctxMenuCavitites", Form.MousePosition, CType(sender, Control))
                End If
            End If
        End Sub

        Private Sub NaviGridCavitites_PreviewKeyDown(sender As Object, e As PreviewKeyDownEventArgs) Handles NaviCavitiesGrid.PreviewKeyDown
            If e.KeyCode = Keys.Escape Then
                If NaviCavitiesGrid.Selected.Rows.Count > 1 Then
                    e.IsInputKey = True
                    Dim activeRow As UltraGridRow = NaviCavitiesGrid.ActiveRow
                    ResetSelectedCavitiesRowsTo(activeRow)
                End If
            End If
        End Sub

        Private Sub NaviConnectorsGrid_DoubleClickCell(sender As Object, e As DoubleClickCellEventArgs) Handles NaviConnectorsGrid.DoubleClickCell, NaviCavitiesGrid.DoubleClickCell
            If Not My.Application.MainForm.GeneralSettings.AutoSyncCavityChecksSelection Then
                JumpTo_Click(DirectCast(sender, UltraGrid))
            End If
        End Sub

        Private Sub CavityNavigator_VisibleChanged(sender As Object, e As EventArgs) Handles Me.VisibleChanged
            If Me.Visible Then
                If Me.UltraToolbarsManager1 IsNot Nothing AndAlso Me.UltraToolbarsManager1.Tools IsNot Nothing AndAlso Me.UltraToolbarsManager1.Tools.Exists(Buttons.JumpTo) Then
                    Me.UltraToolbarsManager1.Tools(Buttons.JumpTo).SharedProps.Visible = Not (My.Application?.MainForm?.GeneralSettings?.AutoSyncCavityChecksSelection).GetValueOrDefault
                End If
            End If
        End Sub

        Private Sub Model1_HandleCreated(sender As Object, e As EventArgs) Handles Design3D.HandleCreated
            Design3D.ZoomFit()
        End Sub

        Private Sub NaviConnectorsGrid_AfterSelectChange(sender As Object, e As AfterSelectChangeEventArgs) Handles NaviConnectorsGrid.AfterSelectChange
            Dim row As UltraGridRow = Me.NaviConnectorsGrid.Selected.Rows.OfType(Of UltraGridRow).FirstOrDefault
            If row IsNot Nothing Then
                row.Activated = True

                Me.CavWiresBindingSource.DataSource = CType(row.ListObject, ConnectorView).CavWires

                If Me.NaviCavitiesGrid IsNot Nothing AndAlso Not Me.NaviCavitiesGrid.IsDisposed Then
                    Dim activeRow As UltraGridRow = Me.NaviCavitiesGrid.ActiveRow
                    Dim rows As List(Of UltraGridRow) = Me.NaviCavitiesGrid.Selected.Rows.OfType(Of UltraGridRow).ToList
                    Me.ResetSelectedCavitiesRowsTo(rows.ToArray, activeRow) 'HINT: the rows are selected after DataSource was changed but no event was fired -> we are re-selecting the currently selected rows here to force raising the event
                End If
            End If
        End Sub

    End Class

End Namespace