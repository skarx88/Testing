Imports System.IO
Imports System.Reflection
Imports devDept.Eyeshot.Entities
Imports Infragistics.Win.UltraWinDock
Imports Infragistics.Win.UltraWinTabbedMdi
Imports Infragistics.Win.UltraWinToolbars
Imports Zuken.E3.HarnessAnalyzer.D3D.Consolidated.Controls
Imports Zuken.E3.HarnessAnalyzer.D3D.Consolidated.Controls.Consolidated3DControl
Imports Zuken.E3.HarnessAnalyzer.D3D.Consolidated.Designs
Imports Zuken.E3.HarnessAnalyzer.Project.Documents
Imports Zuken.E3.HarnessAnalyzer.Shared.Common
Imports Zuken.E3.Lib.Eyeshot.Model
Imports Zuken.E3.Lib.IO.Files
Imports Zuken.E3.Lib.IO.Files.Hcv

Namespace D3D

    Friend Class MainFormController
        Implements IDisposable
        Implements ID3DAccessor

        Public Event Progress(sender As Object, e As DocumentFormMessageEventArgs)

        Private WithEvents _consolidated3D As New Consolidated3DControl
        Private WithEvents _mainStateMachine As MainStateMachine
        Private WithEvents _tabCheckBoxFilter As TabDocumentCheckBoxCreationFilter
        Private WithEvents _project As HarnessAnalyzerProject

        Private WithEvents _mainForm As MainForm
        Private _d3dAsyncLoadCarCancelSource As New D3D.Consolidated.Controls.D3DCancellationTokenSource
        Private _isChangingVisibilityByType As ChangingVisibilityType
        Private _visibilityChangeLock As New System.Threading.SemaphoreSingle
        Private _lockDisplayView3DButton As New System.Threading.SemaphoreSlim(1)
        Private _entireCarTransformationSetting As Settings.CarTransformationSetting
        Private _lockChange3DMode As New System.Threading.SemaphoreSlim(1)
        Private _lockButtonChange As New System.Threading.SemaphoreSlim(1)
        Private _disposedValue As Boolean

        Friend WithEvents _d3dComparer As D3DComparerCntrl

        Friend Sub New(mainform As MainForm)
            _mainForm = mainform

            If mainform.Project Is Nothing Then
                Throw New ArgumentNullException(NameOf(mainform), $"{mainform.GetType.Name}.{NameOf(mainform.Project)} must be set before initializing ""{Me.GetType.Name}""")
            End If

            If mainform.MainStateMachine Is Nothing Then
                Throw New ArgumentNullException(NameOf(mainform), $"{mainform.GetType.Name}.{NameOf(mainform.MainStateMachine)} must be set before initializing ""{Me.GetType.Name}""")
            End If

            _project = mainform.Project
            _mainStateMachine = mainform.MainStateMachine
            _consolidated3D = New Consolidated3DControl()
            _tabCheckBoxFilter = New TabDocumentCheckBoxCreationFilter(mainform.utmmMain)
            _tabCheckBoxFilter.Visible = False
            mainform.utmmMain.CreationFilter = _tabCheckBoxFilter

            AddHandler mainform.udmMain.BeforePaneButtonClick, AddressOf udmMain_BeforePaneButtonClick
        End Sub

        ReadOnly Property Consolidated As D3D.Consolidated.Controls.Consolidated3DControl Implements ID3DAccessor.Consolidated
            Get
                Return _consolidated3D
            End Get
        End Property

        Public ReadOnly Property ActiveDocument As D3D.Document.Controls.Document3DControl Implements ID3DAccessor.ActiveDocument
            Get
                Return ActiveDocumentForm._D3DControl
            End Get
        End Property

        Private ReadOnly Property ActiveDocumentForm As DocumentForm
            Get
                Return _mainForm.ActiveDocument
            End Get
        End Property

        ReadOnly Property CancelSource As D3D.Consolidated.Controls.D3DCancellationTokenSource
            Get
                Return _d3dAsyncLoadCarCancelSource
            End Get
        End Property

        Friend ReadOnly Property ConsolidatedCarSetting() As Settings.CarTransformationSetting Implements ID3DAccessor.ConsolidatedCarSetting
            Get
                Return _entireCarTransformationSetting
            End Get
        End Property

        Public ReadOnly Property IsCancellationRequested As Boolean Implements ID3DAccessor.IsCancellationRequested
            Get
                Return (CancelSource?.IsCancellationRequested).GetValueOrDefault
            End Get
        End Property

        Protected Overridable Sub OnProgress(e As DocumentFormMessageEventArgs)
            RaiseEvent Progress(Me, e)
        End Sub

        Private Async Sub UpdateAllCarChangeButtons()
            Await _lockButtonChange.WaitAsync()
            Try
                Await TaskEx.WaitUntil(Function() Not (_consolidated3D?.IsBusy).GetValueOrDefault Or Not (_consolidated3D?.Visible).GetValueOrDefault OrElse _disposedValue)
                If Not _disposedValue Then
                    UpdateCarRealtedButtonsEnabled()
                    _consolidated3D.InvokeOrDefault(
                        Sub()
                            _consolidated3D.GetToolBarButton(ToolBarButtons.LoadCarModel).Enabled = _consolidated3D.GetAttachedDocuments(onlyLoaded:=True).Any() AndAlso Not _consolidated3D.IsBusy
                            _mainForm.utmMain.Tools.Item(HomeTabToolKey.Export3DModel.ToString).SharedProps.Enabled = _consolidated3D.GetAttachedDocuments(onlyLoaded:=True).Any() AndAlso Not _consolidated3D.IsBusy
                        End Sub)
                End If
            Finally
                _lockButtonChange.Release()
            End Try
        End Sub

        Private Sub _consolidated3D_CarChanging(sender As Object, e As CarChangeEventArgs) Handles _consolidated3D.CarChanging
            UpdateAllCarChangeButtons()
        End Sub

        Private Sub _consolidated3D_CarChanged(sender As Object, e As CarChangeEventArgs) Handles _consolidated3D.CarChanged
            UpdateAllCarChangeButtons()
        End Sub

        Private Sub _consolidated3D_DocumentsChanging(sender As Object, e As DocumentsChangeEventArgs) Handles _consolidated3D.DocumentsChanging
            Select Case e.ChangeType
                Case DocumentsChangeType.Detach, DocumentsChangeType.Load
                    UpdateAllCarChangeButtons()
            End Select
        End Sub

        Private Sub _consolidated3D_DocumentsChanged(sender As Object, e As DocumentsChangeEventArgs) Handles _consolidated3D.DocumentsChanged
            Select Case e.ChangeType
                Case DocumentsChangeType.Detach, DocumentsChangeType.Load
                    UpdateAllCarChangeButtons()
            End Select
        End Sub

        Private Sub _consolidated3D_Progress(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles _consolidated3D.Progress
            If ActiveDocumentForm IsNot Nothing Then
                Dim myMessageEventargs As New DocumentFormMessageEventArgs(MessageEventArgs.MsgType.ShowProgressAndMessage, e.ProgressPercentage, CStr(e.UserState), ActiveDocumentForm)
                If TypeOf e Is D3D.Consolidated.Controls.ProgressEventArgs Then
                    myMessageEventargs.ProgressId = CType(e, D3D.Consolidated.Controls.ProgressEventArgs).ProgressId
                End If
                OnProgress(myMessageEventargs)
            End If
        End Sub

        Private Sub UpdateCarRealtedButtonsEnabled()
            If Not _disposedValue AndAlso _mainForm IsNot Nothing Then
                With _mainForm
                    .InvokeOrDefault(
                        Sub()
                            .utmMain.Tools(HomeTabToolKey.LoadCarTransformation.ToString).SharedProps.Enabled = _consolidated3D.Visible AndAlso _consolidated3D.GetAttachedDocuments(True).Any() AndAlso Not _consolidated3D.IsBusy
                            .utmMain.Tools(HomeTabToolKey.SaveCarTransformation.ToString).SharedProps.Enabled = _consolidated3D.Visible AndAlso _consolidated3D.GetAttachedDocuments(True).Any() AndAlso _consolidated3D.HasCar AndAlso Not _consolidated3D.IsBusy
                        End Sub)
                End With
            End If
        End Sub

        Friend Sub InitializeConsolidated3D()
            If _mainForm.HasView3DFeature Then
                _consolidated3D.Initialize(_mainForm.GeneralSettings.CarModelDirectory, _mainForm.GeneralSettings.AutoSync3DSelection)
                If _consolidated3D.Initialized = D3D.Consolidated.Controls.InitializedType.First Then
                    _mainForm.udmMain.BeginUpdate()

                    Dim view3DPane As New DockableControlPane(HomeTabToolKey.Display3DView.ToString, UIStrings.Car3dPane_Caption)
                    Init3dPane(view3DPane)

                    _mainForm.udmMain.EndUpdate()
                End If
            End If
        End Sub

        Private Sub Init3dPane(view3DPane As DockableControlPane)
            With view3DPane
                .Settings.CanDisplayAsMdiChild = Infragistics.Win.DefaultableBoolean.False
                .Settings.AllowDockAsTab = Infragistics.Win.DefaultableBoolean.False
                .Settings.DoubleClickAction = PaneDoubleClickAction.None
                .Settings.AllowDockBottom = Infragistics.Win.DefaultableBoolean.False
                .Settings.AllowDockLeft = Infragistics.Win.DefaultableBoolean.False
                .Settings.AllowDockRight = Infragistics.Win.DefaultableBoolean.False
                .Settings.AllowDockTop = Infragistics.Win.DefaultableBoolean.False
                .Settings.AllowPin = Infragistics.Win.DefaultableBoolean.False
                .Size = My.Settings.D3DFloatSize

                _mainForm.udmMain.ControlPanes.Add(view3DPane)

                .Dock(DockedSide.Bottom)
                .Close()

                .Control = _consolidated3D
            End With
        End Sub

        Private Sub _consolidated3D_SelectedEntitiesChanged(sender As Object, e As D3D.Consolidated.Controls.SelectedEntitiesChangedEventArgs) Handles _consolidated3D.SelectedEntitiesChanged
            Dim selectedByGroups As Dictionary(Of String, List(Of IBaseModelEntityEx)) = _consolidated3D.Entities.OfType(Of IBaseModelEntityEx).Where(Function(ent) ent.Selected).GroupBy(Function(ent) TryCast(CType(ent, IEntityProvidesGroup).Group, IProvidesId)?.Id).ToDictionary(Function(grp) grp.Key, Function(grp) grp.ToList)

            For Each drawingMdiTab As MdiTab In _mainForm.utmmMain.TabGroups.All.Cast(Of MdiTabGroup).SelectMany(Function(grp) grp.Tabs.Cast(Of MdiTab)())
                Dim docForm As DocumentForm = CType(drawingMdiTab.Form, DocumentForm)
                For Each entity As IBaseModelEntityEx In docForm.Document.Entities
                    If entity.Selectable Then
                        entity.Selected = False
                    End If
                Next

                docForm.Document.Entities.UpdateVisibleSelection()
            Next

            For Each drawingMdiTab As MdiTab In _mainForm.utmmMain.TabGroups.All.Cast(Of MdiTabGroup).SelectMany(Function(grp) grp.Tabs.Cast(Of MdiTab)())
                Dim docForm As DocumentForm = CType(drawingMdiTab.Form, DocumentForm)

                If selectedByGroups.ContainsKey(docForm.Document.Id.ToString) Then
                    Dim docEntsDic As Dictionary(Of String, IBaseModelEntityEx) = docForm.Document.Entities.ToDictionary(Function(ent) ent.Id, Function(ent) ent)

                    For Each entity As IBaseModelEntityEx In selectedByGroups(docForm.Document.Id.ToString)
                        If entity IsNot Nothing Then
                            Dim docEntity As IBaseModelEntityEx = Nothing
                            If docEntsDic.TryGetValue(entity.Id, docEntity) Then
                                docEntity.Selected = True
                            End If
                        End If
                    Next

                    docForm.Document.Entities.UpdateVisibleSelection()
                End If
            Next
        End Sub

        Private Async Sub udmMain_BeforePaneButtonClick(sender As Object, e As CancelablePaneButtonEventArgs)
            If e.Pane.IsVisible AndAlso e.Button = PaneButton.Close AndAlso e.Pane.Key = HomeTabToolKey.Display3DView.ToString Then
                e.Cancel = Not Await ProcessBeforePane3DClose(toggleAdjust:=False)
            End If
        End Sub

        Private Sub ToggleOrDisableAdjustMode(toggle As Boolean, type As devDept.Eyeshot.ObjectManipulatorChangeType)
            If toggle Then
                Consolidated.ToggleAdjustMode(type)
            Else
                Consolidated.DisableAdjustMode(type)
            End If
        End Sub

        Private Async Function ProcessBeforePane3DClose(toggleAdjust As Boolean) As Task(Of Boolean)
            Await _lockChange3DMode.WaitAsync
            Try
                If _consolidated3D.IsInitialized Then
                    If Consolidated.HasChanges Then
                        Dim dlg As New D3DSaveChangesMsgBox
                        Dim diagRes As DialogResult = DialogResult.Ignore
                        If Consolidated.AdjustMode Then
                            diagRes = dlg.ShowDialog() 'HINT: Yes = Save, No = Dismiss, Ignore = Accept only, Cancel = Cancel
                        ElseIf Not toggleAdjust Then
                            diagRes = MessageBoxEx.ShowQuestion(DialogStringsD3D.HasUnsavedChangesQuestion, MessageBoxButtons.YesNoCancel)
                        End If

                        Select Case diagRes
                            Case DialogResult.Cancel
                                Return False
                            Case DialogResult.Yes, DialogResult.Ignore
                                Dim cancelSave As Boolean = False
                                If _mainStateMachine.IsXhcv Then
                                    _consolidated3D.ApplyCarModelChanges()
                                    ToggleOrDisableAdjustMode(toggleAdjust, devDept.Eyeshot.ObjectManipulatorChangeType.Apply)
                                    If diagRes <> DialogResult.Ignore Then
                                        If Consolidated.CurrentTrans IsNot Nothing Then
                                            If Consolidated.HasCar Then
                                                _mainStateMachine.XHcvFile.CarTransformation = Consolidated.GetTransformationAsContainerFile
                                                _mainStateMachine.SaveXhcV()
                                            Else
                                                RemoveCarTransformationFile()
                                            End If
                                        ElseIf Not Consolidated.HasCar Then
                                            RemoveCarTransformationFile()
                                        End If
                                    End If
                                Else
                                    If Not _consolidated3D.HasCar Then
                                        If MessageBox.Show(DialogStringsD3D.SaveNotPossibleBecauseCarWasRemoved, HarnessAnalyzer.[Shared].MSG_BOX_TITLE, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = DialogResult.Cancel Then
                                            Return False
                                        End If
                                    End If

                                    If diagRes = DialogResult.Yes AndAlso _consolidated3D.HasCar Then
                                        Dim result As SaveResult = SaveCarTransformationAction()
                                        cancelSave = result <> SaveResult.Ok
                                    End If

                                    If Not cancelSave Then
                                        _consolidated3D.ApplyCarModelChanges()
                                        ToggleOrDisableAdjustMode(toggleAdjust, devDept.Eyeshot.ObjectManipulatorChangeType.Apply)
                                    End If

                                End If
                            Case System.Windows.Forms.DialogResult.No
                                Await Consolidated.RevertCarModel
                                ToggleOrDisableAdjustMode(toggleAdjust, devDept.Eyeshot.ObjectManipulatorChangeType.Cancel)
                        End Select
                    Else
                        ToggleOrDisableAdjustMode(toggleAdjust, devDept.Eyeshot.ObjectManipulatorChangeType.Cancel)
                    End If
                End If
                Return True
            Finally
                _lockChange3DMode.Release()
            End Try
        End Function

        Private Async Sub _consolidated3D_ToolBarButtonClick(sender As Object, e As D3D.Consolidated.Controls.ToolBarButtonEventArgs) Handles _consolidated3D.ToolBarButtonClick
            Select Case e.Button.Name
                Case ToolBarButtons.ToggleAdjustMode.ToString
                    Await ProcessBeforePane3DClose(toggleAdjust:=True)
            End Select
        End Sub

        Private Sub RemoveCarTransformationFile()
            If (_mainStateMachine.XHcvFile.CarTransformation?.HasData).GetValueOrDefault Then
                _mainStateMachine.XHcvFile.Remove(_mainStateMachine.XHcvFile.CarTransformation)
                _mainStateMachine.SaveXhcV()
            End If
        End Sub

        Private Sub _tabCheckBoxCreationFilter_CheckBoxPropertyChanged(sender As Object, e As TabDocumentCheckBoxCreationFilter.CheckBoxProperyChangedEventArgs) Handles _tabCheckBoxFilter.CheckBoxPropertyChanged
            If e.PropertyName = NameOf(e.CheckBox.State) Then
                Dim doc As DocumentForm = Nothing
                If _mainForm.GetAllDocuments.TryGetValue(e.CheckBox.DocumentId, doc) Then
                    Dim harnessGroup As Group3D = Nothing
                    'TODO: maybe this _consolidated3D should be removed because when creation filter checkbox is available the 3DView MUST also be available
                    If _consolidated3D IsNot Nothing AndAlso _consolidated3D.Entities.Exists(doc.Document.Id.ToString) Then
                        harnessGroup = CType(_consolidated3D.Entities.GetGroupOrEntity(doc.Document.Id.ToString).FirstOrDefault, Group3D)
                        ChangeGroupVisibilityAsync(harnessGroup, e.CheckBox.State = CheckState.Checked, ChangingVisibilityType.FromTab)
                    End If
                End If
            End If
        End Sub

        Private Sub _tabCheckBoxCreationFilter_InitializeCheckBox(sender As Object, e As TabDocumentCheckBoxCreationFilter.CheckBoxEventArgs) Handles _tabCheckBoxFilter.InitializeCheckBox
            Dim doc As DocumentForm = Nothing
            If _mainForm.GetAllDocuments.TryGetValue(e.CheckBox.DocumentId, doc) Then
                e.CheckBox.Enabled = _consolidated3D.Entities.Exists(doc.Document?.Id.ToString)
            End If
        End Sub

        Private Async Sub ChangeGroupVisibilityAsync(d3DGroup As D3D.Consolidated.Designs.Group3D, visibility As Boolean, changeSource As ChangingVisibilityType)
            Using Await _visibilityChangeLock.BeginWaitAsync
                If _isChangingVisibilityByType = ChangingVisibilityType.None Then
                    Await Task.Run(
                        Sub()
                            _isChangingVisibilityByType = changeSource
                            d3DGroup.Visible = visibility
                            _isChangingVisibilityByType = ChangingVisibilityType.None
                        End Sub)
                    _consolidated3D.Invalidate(True)
                End If
            End Using
        End Sub

        Private Sub _mainStateMachine_DocumentFilterItemCheckedChanged(sender As Object, e As DocumentFilterControl.ItemCheckedEventArgs) Handles _mainStateMachine.DocumentFilterItemCheckedChanged
            If _consolidated3D.Entities.Exists(e.Item.Key) Then
                Dim harnessGroup As Group3D = CType(_consolidated3D.Entities.GetGroupOrEntity(e.Item.Key).FirstOrDefault, Group3D)
                ChangeGroupVisibilityAsync(harnessGroup, e.Checked, ChangingVisibilityType.FromRibbon)
            End If
        End Sub

        Private Sub _consolidated3D_GroupVisibilityChanged(sender As Object, e As GroupVisibilityChangedEventArgs) Handles _consolidated3D.GroupVisibilityChanged
            Select Case _isChangingVisibilityByType
                Case ChangingVisibilityType.FromRibbon
                    Dim tab As MdiTab = _mainForm.utmmMain.TabGroups.OfType(Of MdiTabGroup).SelectMany(Function(grp) grp.Tabs.OfType(Of MdiTab)).Where(Function(tb) CType(tb.Form, DocumentForm).Document.Id.ToString = e.Group.Name).Single
                    With _tabCheckBoxFilter
                        .EventsEnabled = False
                        .SetCheckBoxOfTab(tab, e.Group.Visible)
                        .EventsEnabled = True
                    End With
                Case ChangingVisibilityType.FromTab
                    Dim ppContainer As PopupControlContainerTool = DirectCast(_mainForm.utmMain.Tools(HomeTabToolKey.Document3DFilter.ToString), PopupControlContainerTool)
                    Dim filterControl As DocumentFilterControl = CType(ppContainer.Control, DocumentFilterControl)
                    With filterControl
                        .EventsEnabled = False
                        .SetCheckStateOfItem(e.Group.Name, e.Group.Visible)
                        .EventsEnabled = True
                    End With
                Case Else
                    Throw New NotImplementedException
            End Select
        End Sub

        Public Async Sub DisplayConsolidatedViewAction(button As StateButtonTool)
            Await _lockDisplayView3DButton.WaitAsync
            Try
                button.SharedProps.Enabled = False
                If button.Checked Then
                    InitializeConsolidated3D()
                    If _mainForm.Project.View IsNot _consolidated3D Then
                        Await Me._mainForm.Project.SetViewAsync(_consolidated3D)
                    End If
                    _mainForm.udmMain.BeginUpdate()
                    _mainForm.udmMain.ControlPanes(HomeTabToolKey.Display3DView.ToString).Show()

                    If _consolidated3D.Initialized = D3D.Consolidated.Controls.InitializedType.First Then ' the first time after it was initialized the position will be set
                        Dim cX As Integer = CInt(Me._mainForm.Location.X + (Me._mainForm.Width) / 2)
                        Dim cy As Integer = CInt(Me._mainForm.Location.Y + (Me._mainForm.Height) / 2)

                        If (My.Settings.D3DFloatPosition <> System.Drawing.Point.Empty) Then
                            cX = My.Settings.D3DFloatPosition.X
                            cy = My.Settings.D3DFloatPosition.Y
                        End If

                        _mainForm.udmMain.ControlPanes(HomeTabToolKey.Display3DView.ToString).Float(True, New System.Drawing.Point(cX, cy))
                        _consolidated3D.CurrentTrans = _entireCarTransformationSetting
                    End If

                    _mainForm.udmMain.EndUpdate()
                Else
                    If Await ProcessBeforePane3DClose(toggleAdjust:=False) Then
                        _mainForm.udmMain.PaneFromKey(HomeTabToolKey.Display3DView.ToString).Close()
                    Else
                        button.Checked = True ' re-enable if close was cancelled
                    End If
                End If
            Catch ex As Exception
                ex.ShowMessageBox(_mainForm)
            Finally
                button.SharedProps.Enabled = True
                _lockDisplayView3DButton.Release()
            End Try
        End Sub

        Public Sub LoadCarTransformationByUserAction()
            Dim successOrCancel As Boolean
            If _consolidated3D IsNot Nothing Then
                Do
                    successOrCancel = False

                    Using fo As New OpenFileDialog
                        fo.Filter = FileExtensionFilters.CarTransformation
                        fo.Title = DialogStrings.LoadCarTransformation_Caption
                        fo.FileName = My.Settings.LastTransformationFile

                        If (fo.ShowDialog = DialogResult.OK) Then
                            Try
                                My.Settings.LastTransformationFile = fo.FileName
                                Dim tsk1 As Task = _consolidated3D.RemoveCar()
                                tsk1.WaitWithPumping
                                _consolidated3D.LoadCarTransformationSettings(fo.FileName) ' due to RemoveCar also removes the transformationSetting
                                successOrCancel = True

                                If Not String.IsNullOrEmpty(_consolidated3D.CurrentTrans?.CarFileName) Then
                                    Dim tsk As Task(Of CarStateMachine.CarModelLoadResult) = _consolidated3D.LoadAndAddCarModel(_mainForm.GeneralSettings, _consolidated3D.CurrentTrans)
                                    tsk.ContinueWith(
                                        Sub()
                                            If tsk.IsFaulted Then ' must be checked first: no result if faulted
                                                If (tsk.Exception.ShowMessageBox(MessageBoxButtons.RetryCancel) = DialogResult.Retry) Then
                                                    LoadCarTransformationByUserAction()
                                                End If
                                            ElseIf tsk.Result.IsSuccess Then
                                                _consolidated3D.SetSelectedCar(tsk.Result.CarFullPath)
                                                _consolidated3D.ApplyCarModelChanges()
                                            Else
                                                MessageBoxEx.ShowError(tsk.Result.Message)
                                            End If
                                        End Sub, TaskScheduler.FromCurrentSynchronizationContext)
                                End If
                            Catch ex As Exception
                                If (MessageBoxEx.ShowError(String.Format(ErrorStrings.D3D_NotATransformationFile, fo.FileName), MessageBoxButtons.RetryCancel) = DialogResult.Cancel) Then
                                    successOrCancel = True
                                End If
                            End Try
                        Else
                            successOrCancel = True
                        End If
                    End Using
                Loop Until successOrCancel
            End If
        End Sub

        Friend Sub OnBeforeOpenDocument(filePath As String, isXhcv As Boolean)
            _entireCarTransformationSetting = Nothing

            If (isXhcv AndAlso _mainForm.HasView3DFeature AndAlso _consolidated3D IsNot Nothing) Then
                Dim tsk As Task = _consolidated3D.RemoveCar
                tsk.Wait()
            End If
        End Sub

        Public Sub LoadCarTransformationSettings(stream As System.IO.Stream)
            _entireCarTransformationSetting = D3D.Consolidated.Controls.CarStateMachine.LoadCarTransformationSettingsCore(stream)
        End Sub

        Public Sub LoadCarTransformationSettingsFromContainer(container As IDataContainerFile)
            If container IsNot Nothing Then
                Using s As Stream = container.GetDataStream
                    LoadCarTransformationSettings(s)
                End Using
            End If
        End Sub

        Public Sub ClearCarTransformationSettings()
            _entireCarTransformationSetting = Nothing
        End Sub

        Friend Enum SaveResult
            Ok = 0
            Cancelled = 1
            [Error] = 2
        End Enum

        Friend Async Function Export3DModelAction() As Task(Of SaveResult)
            If _consolidated3D IsNot Nothing Then
                Do
                    Using fo As New SaveFileDialog
                        fo.Filter = FileExtensionFilters.Wavefront
                        fo.DefaultExt = IO.Path.GetExtension(fo.Filter.Split("|"c).Last)
                        fo.Title = DialogStrings.Export3DModel_Caption
                        fo.FileName = My.Settings.Last3DModelExportFile

                        If (fo.ShowDialog = DialogResult.OK) Then
                            Try
                                My.Settings.Last3DModelExportFile = fo.FileName
                                Await _consolidated3D.WriteVisibleNonCarEntitiesAsync(fo.FileName)
                                MessageBox.Show(String.Format(DialogStringsD3D.ModelExportSuccessfullyFinished, fo.FileName), E3.HarnessAnalyzer.[Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
                                Return SaveResult.Ok
                            Catch ex As Exception
                                If (MessageBoxEx.ShowError(String.Format(ErrorStrings.D3D_ErrorExportingModel, ex.Message, fo.FileName), MessageBoxButtons.RetryCancel) = DialogResult.Cancel) Then
                                    Return SaveResult.Error
                                End If
                            End Try
                        Else
                            Return SaveResult.Cancelled
                        End If
                    End Using
                Loop
            End If
            Return SaveResult.Cancelled
        End Function

        Friend Function SaveCarTransformationAction(Optional apply As Boolean = False) As SaveResult
            If _consolidated3D IsNot Nothing Then
                Do
                    Using fo As New SaveFileDialog
                        fo.Filter = FileExtensionFilters.CarTransformation ' TODO: the whole file extensions topic (whole HA App) needs a consolidated global class where all this is handled centralized
                        fo.DefaultExt = IO.Path.GetExtension(fo.Filter.Split("|"c).Last)
                        fo.Title = DialogStrings.SaveCarTransformation_Caption
                        fo.FileName = My.Settings.LastTransformationFile

                        If (fo.ShowDialog = DialogResult.OK) Then
                            Try
                                _consolidated3D.SaveCarTransformationToFile(fo.FileName)
                                If apply Then
                                    _consolidated3D.ApplyCarModelChanges() ' HINT: The apply is needed because the save can also be triggered by user-action (does not close 3D-Pane) and change will be normally detected if car has changed after point when control got visible
                                    _consolidated3D.DisableAdjustMode(devDept.Eyeshot.ObjectManipulatorChangeType.Apply)
                                End If
                                My.Settings.LastTransformationFile = fo.FileName

                                Return SaveResult.Ok
                            Catch ex As Exception
                                If MessageBoxEx.ShowError(String.Format(ErrorStrings.D3D_ErrorSavingTransformationFile, ex.Message, fo.FileName), MessageBoxButtons.RetryCancel) = DialogResult.Cancel Then
                                    Return SaveResult.Error
                                End If
                            End Try
                        Else
                            Return SaveResult.Cancelled
                        End If
                    End Using
                Loop
            Else
                Return SaveResult.Cancelled
            End If
        End Function

        Friend Sub OnAfterDocumentClosed(document As DocumentForm)
            TryRemoveFromConsolidated3D(document)

            If _mainForm.utmmMain.TabGroups.Count = 0 Then
                If _consolidated3D IsNot Nothing AndAlso Not _consolidated3D.IsDisposed Then
                    Dim tsk As Task = _consolidated3D.RemoveCar()
                    tsk.Wait()
                    _consolidated3D.DisableAdjustMode(devDept.Eyeshot.ObjectManipulatorChangeType.Cancel)
                    _consolidated3D.Invalidate(True)

                    If _consolidated3D.Visible Then
                        Dim D3DView As DockableControlPane = _mainForm.udmMain.PaneFromControl(_consolidated3D)
                        If D3DView IsNot Nothing Then
                            D3DView.Close()
                        End If
                    End If

                    _mainForm.utmMain.Ribbon.Tabs(NameOf(MenuButtonStrings.Home)).Groups(NameOf(My.Resources.MenuButtonStrings._3D)).Visible = False
                End If
            End If
        End Sub

        Friend Sub OnAfterPaneHidden(pane As DockableControlPane)
            If pane.Key = HomeTabToolKey.Display3DView.ToString Then
                _tabCheckBoxFilter.Visible = False
                If _mainForm.utmMain.Tools.Exists(HomeTabToolKey.Document3DFilter.ToString) Then
                    _mainForm.utmMain.Tools(HomeTabToolKey.Document3DFilter.ToString).SharedProps.Enabled = False
                End If
            End If
        End Sub

        Friend Sub OnAfterPaneDisplayed(pane As DockableControlPane)
            If pane.Key = HomeTabToolKey.Display3DView.ToString Then
                _tabCheckBoxFilter.Visible = True
                If _mainForm.utmMain.Tools.Exists(HomeTabToolKey.Document3DFilter.ToString) Then
                    _mainForm.utmMain.Tools(HomeTabToolKey.Document3DFilter.ToString).SharedProps.Enabled = True
                End If
            End If
        End Sub

        Public Sub UpdateFromGeneralSettings()
            If _consolidated3D IsNot Nothing Then
                _consolidated3D.CarModelsDirectory = _mainForm.GeneralSettings.CarModelDirectory
                _consolidated3D.GetToolBarButton(ToolBarButtons.SynchronizeSelection).Visible = Not _mainForm.GeneralSettings.AutoSync3DSelection
            End If
        End Sub

        Private ReadOnly Property DocumentFilterControl As DocumentFilterControl
            Get
                If _mainForm.utmMain.Tools.Exists(HomeTabToolKey.Document3DFilter.ToString) Then
                    Dim ppContainer As PopupControlContainerTool = DirectCast(_mainForm.utmMain.Tools(HomeTabToolKey.Document3DFilter.ToString), PopupControlContainerTool)
                    Return CType(ppContainer.Control, DocumentFilterControl)
                End If
                Return Nothing
            End Get
        End Property

        Public Async Function SetConsolidateVisibileCheckBoxState(doc As DocumentForm, enabled As Boolean, Optional waitForSetCheckBoxTabUIElement As Boolean = False) As Task
            Dim tab As MdiTab = _mainForm.utmmMain.TabFromForm(doc)
            If (tab IsNot Nothing) Then
                If Not enabled Then
                    Me.DocumentFilterControl.RemoveDocument(doc.Document)
                End If

                Await _tabCheckBoxFilter.SetEnabledCheckBoxOfTab(tab, enabled, waitForSetCheckBoxTabUIElement)

                If enabled Then
                    Me.DocumentFilterControl.AddDocument(doc.Document, doc.Text)
                End If
            Else
                Return
            End If
        End Function

        Friend Function TryRemoveFromConsolidated3D(document As DocumentForm) As Boolean
            If (document IsNot Nothing AndAlso _consolidated3D IsNot Nothing) Then
                Dim removed As Boolean
                For Each entity As IEntity In _consolidated3D.Entities.GetGroupOrEntity(document.Id)
                    If _consolidated3D.Entities.Remove(entity) Then
                        _consolidated3D.Invalidate(True)
                        removed = True
                    End If
                Next
                Return removed
            End If

            Return False
        End Function

        Public Enum ChangingVisibilityType
            None
            FromTab
            FromRibbon
        End Enum

        Private Sub _consolidated3D_VisibleChanged(sender As Object, e As EventArgs) Handles _consolidated3D.VisibleChanged
            UpdateAllCarChangeButtons()
        End Sub

        Private Sub _project_DocumentStateChanged(sender As Object, e As WorkStateResultFileEventArgs) Handles _project.DocumentStateChanged
            If _project.Documents.Count = 0 OrElse _project.Documents.OfType(Of HcvDocument).All(Function(doc) Not doc.IsOpen) Then
                _entireCarTransformationSetting = Nothing 'HINT: Clean setting after all documents have been closed (f.e. xHCV close -> open hcv, don't use settings from xhcv)
            End If
        End Sub

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not _disposedValue Then
                If disposing Then
                    If _mainForm?.udmMain IsNot Nothing Then
                        RemoveHandler _mainForm.udmMain.BeforePaneButtonClick, AddressOf udmMain_BeforePaneButtonClick
                    End If

                    _d3dAsyncLoadCarCancelSource?.Dispose()
                    _consolidated3D?.Dispose()
                    _lockDisplayView3DButton?.Dispose()
                    _tabCheckBoxFilter?.Dispose()
                End If

                _project = Nothing
                _mainForm = Nothing
                _consolidated3D = Nothing
                _tabCheckBoxFilter = Nothing
                _mainStateMachine = Nothing
                _d3dAsyncLoadCarCancelSource = Nothing
                _lockDisplayView3DButton = Nothing
                _entireCarTransformationSetting = Nothing
                _disposedValue = True
            End If
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub

        Private Sub MainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles _mainForm.FormClosing
            If Not e.Cancel Then
                SaveGeneralSettings()
                TryResetDataTableSettings()
                SaveConsolidated3DSettings()
                SaveAdvConnectivitySettings()
                _mainForm.DetachFromClipboard()
            End If
        End Sub

        Private Sub SaveGeneralSettings()
            _mainForm?.GeneralSettings?.Save(GetGeneralSettingsConfigurationFile)
        End Sub

        <Obfuscation(Feature:="renaming")>
        Friend Shared Function GetGeneralSettingsConfigurationFile() As String
            Dim e3HarnessAnalyzerDir As String = Zuken.E3.HarnessAnalyzer.[Shared].Utilities.GetApplicationSettingsPath

            If (Not IO.Directory.Exists(e3HarnessAnalyzerDir)) Then
                IO.Directory.CreateDirectory(e3HarnessAnalyzerDir)
            End If

            Return IO.Path.Combine(e3HarnessAnalyzerDir, "GeneralSettings.xml")
        End Function

        Private Sub TryResetDataTableSettings()
            If My.Settings.ResetDataTableSettings Then
                For Each a As GridAppearance In GridAppearance.All
                    a.TryDeleteFile()
                Next
                My.Settings.ResetDataTableSettings = False
            End If
        End Sub

        Friend Sub SaveConsolidated3DSettings()
            If _mainForm.HasView3DFeature AndAlso Me.Panes.Consolidated3DPane IsNot Nothing Then
                My.Settings.D3DFloatPosition = Me.Panes.Consolidated3DPane.DockAreaPane.FloatingLocation
                My.Settings.D3DFloatSize = Me.Panes.Consolidated3DPane.DockAreaPane.Size
            End If
        End Sub

        Friend Sub SaveAdvConnectivitySettings()
            If _mainForm.HasSchematicsFeature AndAlso Panes.SchematicsPane IsNot Nothing Then
                My.Settings.AdvConnFloatPosition = (Panes.SchematicsPane?.DockAreaPane?.FloatingLocation).GetValueOrDefault
                My.Settings.AdvConnFloatSize = (Panes.SchematicsPane?.DockAreaPane?.Size).GetValueOrDefault
            End If
        End Sub

        ReadOnly Property Panes As TabToolPanesCollection
            Get
                Return _mainForm.Panes
            End Get
        End Property

    End Class

End Namespace