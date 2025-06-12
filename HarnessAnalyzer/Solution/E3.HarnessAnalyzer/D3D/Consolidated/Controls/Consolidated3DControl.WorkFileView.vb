Imports System.ComponentModel
Imports Infragistics.Win.UltraWinTabbedMdi
Imports Zuken.E3.HarnessAnalyzer.Project
Imports Zuken.E3.HarnessAnalyzer.Project.Documents
Imports Zuken.E3.Lib.Eyeshot.Model
Imports Zuken.E3.Lib.IO.Files

Namespace D3D.Consolidated.Controls

    'HINT: execution should be ensured after Adapter for document3D has finished (to have fully created/updated entities that can be cloned seamlessly)
    Partial Public Class Consolidated3DControl
        Implements IWorkFileViewAdapter

        Private _lockSetDocumentsVisible As New System.Threading.SemaphoreSlim(1)

        Private Function OnBeforeStartWorkAsync(state As IWorkState, userData As Object) As Task(Of Boolean) Implements IWorkFileViewAdapter.OnBeforeStartWorkAsync
            Return Task.FromResult(True)
        End Function

        Private Function OnWorkFinishedAsync(state As IWorkState, result As IResult) As Task Implements IWorkFileViewAdapter.OnWorkFinishedAsync
            ' nothing currently to do here
            Return Task.CompletedTask
        End Function

        Private Async Function SetDocumentsVisibleCheckBox(state As Boolean, wait As Boolean, ParamArray documents() As DocumentForm) As Task
            Await _lockSetDocumentsVisible.WaitAsync
            Try
                For Each docForm As DocumentForm In documents
                    Dim tab As MdiTab = docForm.MainForm.utmmMain.TabFromForm(docForm)
                    If tab IsNot Nothing Then
                        Await docForm.SetConsolidateVisibileCheckBoxState(state, wait)
                    End If
                Next
            Finally
                _lockSetDocumentsVisible.Release()
            End Try
        End Function

        Protected Overridable Sub OnDocumentsChanging(e As DocumentsChangeEventArgs)
            RaiseEvent DocumentsChanging(Me, e)
        End Sub

        Private Sub OnDocumentsChanging(type As DocumentsChangeType)
            OnDocumentsChanging(New DocumentsChangeEventArgs(type))
        End Sub

        Protected Overridable Sub OnDocumentsChanged(e As DocumentsChangeEventArgs)
            RaiseEvent DocumentsChanged(Me, e)
        End Sub

        Private Sub OnDocumentsChanged(type As DocumentsChangeType)
            OnDocumentsChanged(New DocumentsChangeEventArgs(type))
        End Sub

        Private Async Function LoadAllAttachedAsync(Optional waitForSetCheckBoxTabUIElement As Boolean = True) As Task
            'HINT: first at all: block multiple/parallel loads of "LoadAllAttachedAsync-method" -> we don't want to execute parallel loads of all attached -> the process should be -> attaching documents async to consolidated (while invisible) -> when finished or control is now visible -> execute load (cloning, etc.) of all documents that have been attached before -> clear attached -> and maybe repeat if there have been new documents attached while processing the old attached documents/views
            Using Await _lockState.BeginWaitAsync ' changes the busy state to: a new work has started (wich is "busy" when lockstates > 0)
                If IsInitialized Then
                    Dim attSnapShotColl As List(Of DocumentDesignClone) = _attached?.ToList ' copy current state to list to make a snapshot (which could - but should not - be changed while proceeding with BL - for safety reasons)
                    Dim missingAddedDocuments As DocumentForm() = attSnapShotColl.Where(Function(cln) Not cln.IsAdded).Select(Function(cln) GetDocumentFormFromAttached(cln.Document)).Where(Function(docFrm) Not docFrm.D3DCancellationTokenSource.IsCancellationRequested).ToArray

                    If missingAddedDocuments.Length > 0 Then
                        OnDocumentsChanging(DocumentsChangeType.Load)

                        Dim mainForm As MainForm = missingAddedDocuments.Where(Function(d) d.MainForm IsNot Nothing).FirstOrDefault?.MainForm

                        Await SetDocumentsVisibleCheckBox(False, waitForSetCheckBoxTabUIElement, missingAddedDocuments)

                        Dim res As AggregatedResult = Await LoadAllHarnessClonesAsync(attSnapShotColl)
                        If WriteErrorsLogsIfFaulted(res) Then
                            Return ' break when faulted
                        End If

                        If missingAddedDocuments.Any(Function(doc) (doc?.Document?.IsLastOfXhcv).GetValueOrDefault) Then
                            If Not String.IsNullOrEmpty(mainForm?.D3D?.ConsolidatedCarSetting?.CarFileName) Then
                                Dim result As CarStateMachine.CarModelLoadResult = Await _carStateMachine.LoadAndAddCarModel(mainForm.GeneralSettings, mainForm.D3D.ConsolidatedCarSetting, CType(mainForm.D3D, D3D.MainFormController).CancelSource)
                                If result.IsFaulted Then
                                    mainForm.ActiveDocument._logHub.WriteLogMessage(New LogEventArgs(LogEventArgs.LoggingLevel.Error, result.Message))
                                ElseIf result.IsSuccess Then
                                    ZoomFitAllOrWhenVisible()
                                End If
                            End If
                        End If

                        If mainForm?.D3D IsNot Nothing AndAlso Not (mainForm?.D3D?.IsCancellationRequested).GetValueOrDefault Then
                            missingAddedDocuments = missingAddedDocuments.Where(Function(dFrm) Not dFrm.D3DCancellationTokenSource.IsCancellationRequested).ToArray
                            Await SetDocumentsVisibleCheckBox(True, waitForSetCheckBoxTabUIElement, missingAddedDocuments)
                        End If

                        OnDocumentsChanged(DocumentsChangeType.Load)
                    End If
                End If
            End Using
        End Function

        Private Function OnBeforeChunkWork(chunk As IWorkChunk) As Task(Of Boolean) Implements IWorkFileViewAdapter.OnBeforeChunkWork
            'HINT: this view ist connectect to the project and each document at the same time, thus both chunks will be invoked here.
            'But we only want the project-chunks to be processed with our 3d-model (the document-design-view is processing the document-chunks and we would replace the attached model without this BL)
            If TypeOf TryCast(chunk.Owner, IOwner)?.Owner Is HarnessAnalyzerProject Then
                TryCast(chunk, WorkUnitWrappedWorkChunk)?.AttachWorkspace(Design3D)
            End If
            Return Task.FromResult(True)
        End Function

        Private Function OnAfterChunkWork(e As WorkChunkFinishedEventArgs) As Task Implements IWorkFileViewAdapter.OnAfterChunkWork
            'HINT: see OnBeforeWorkChunkBeginAsync-hint
            If TypeOf TryCast(e.WorkChunk.Owner, IOwner)?.Owner Is HarnessAnalyzerProject Then
                TryCast(e.WorkChunk, WorkUnitWrappedWorkChunk)?.DetachWorkspace()
            End If

            Return Task.CompletedTask
        End Function

        Private Function WriteErrorsLogsIfFaulted(res As AggregatedResult) As Boolean
            If res.IsFaulted Then
                For Each result As DocumentResult In res.FindResults(Of DocumentResult)
                    'HINT: write all error messages to each corresponding document-form-logHub
                    WriteDocumentMessage(result)
                Next
                Return True
            End If
            Return False
        End Function

        Private Sub WriteDocumentMessage(documentResult As DocumentResult)
            Dim documentFrm As DocumentForm = GetDocumentFormFromAttached(documentResult.Document)
            documentFrm.InvokeOrDefault(Sub() documentFrm._logHub.WriteLogMessage(documentResult.Message, If(documentResult.IsFaulted, LogEventArgs.LoggingLevel.Error, LogEventArgs.LoggingLevel.Information)))
        End Sub

        Private Sub ZoomFitAllOrWhenVisible()
            Dim visibleChangedHandler As Global.System.EventHandler =
            Sub()
                If Visible Then
                    RemoveHandler MyBase.VisibleChanged, visibleChangedHandler
                    Me.ZoomFitAll(True)
                End If
            End Sub

            If Not Me.Visible Then
                AddHandler MyBase.VisibleChanged, visibleChangedHandler
            Else
                Me.ZoomFitAll(True)
            End If
        End Sub

        Private Function GetDocumentFormFromAttached(doc As HcvDocument) As DocumentForm
            If TypeOf doc.View Is IEnumerable Then
                If IsAttached(doc) Then
                    Dim docCtrl As D3D.Document.Controls.Document3DControl = GetAttachedFromDocument(doc).FindDocument3DControl
                    If docCtrl IsNot Nothing Then
                        Return CType(docCtrl.ParentForm, DocumentForm)
                    End If
                End If
                Return Nothing
            End If
            Return CType(CType(doc.View, D3D.Document.Controls.Document3DControl).ParentForm, DocumentForm)
        End Function

        Public Function GetAttachedDocuments(Optional onlyLoaded As Boolean = False) As IEnumerable(Of HcvDocument)
            Return _attached.Where(Function(cln) Not onlyLoaded OrElse cln.IsAdded).Select(Function(cln) cln.Document)
        End Function

        Public Function IsAttached(document As HcvDocument) As Boolean
            Return (_attached?.Contains(document)).GetValueOrDefault
        End Function

        Private Function GetAttachedFromDocument(doc As HcvDocument) As DocumentDesignClone
            Return _attached(doc)
        End Function

        Private Async Function OnAfterAttachedAsync(file As IWorkFile) As Task(Of IResult) Implements IWorkFileViewAdapter.OnAfterAttachedAsync
            Dim doc As HcvDocument = TryCast(file, HcvDocument)
            If doc IsNot Nothing Then
                Using Await _lockState.BeginWaitAsync() ' lock multiple calls of "OnAfterAttachedAsync-method"
                    Try
                        SetViewportToolBarVisible(False)
                        With Design3D
                            .CancelWork() ' stop all work
                            .ZoomFitMode = devDept.Eyeshot.zoomFitType.ConvexHull ' performace tweak for heavy 3D models
                            Await TaskEx.WaitUntil(Function() Not .IsBusy)
                            TryRegisterAttachedDocument(doc) ' register data now as attached to consolidated control
                            ' load this data (all that have been attached before) async now, BUT ONLY when consolidated control IS VISIBLE!
                            Dim t As Task = TryLoadAllAttachedAsyncIfVisible().ContinueWith(Sub() SetViewportToolBarVisible(True))
                            Return Result.Success
                        End With
                    Catch ex As Exception
                        SetViewportToolBarVisible(True)
                        Throw
                    End Try
                End Using
            ElseIf TypeOf file Is HarnessAnalyzerProject Then
                'do nothing more here is moved to VisibleChanged
                Return Result.Success
            Else
                Throw New ArgumentException($"Invalid {NameOf(file)}-type: {file.GetType.FullName}. Can only attach file-types of {GetType(HcvDocument).FullName} to view {Me.GetType.FullName} !")
            End If
        End Function

        Private Sub SetViewportToolBarVisible(visible As Boolean)
            If Design3D?.Viewports(0)?.ToolBar IsNot Nothing Then
                Design3D.Viewports(0).ToolBar.Visible = visible
            End If
        End Sub

        Private Function TryRegisterAttachedDocument(doc As HcvDocument) As Boolean
            If Not IsAttached(doc) Then
                OnDocumentsChanging(DocumentsChangeType.Attach)
                Dim harClone As New DocumentDesignClone(doc, Me)
                AttachEEModel(doc.Model)
                _attached.Add(harClone)
                OnDocumentsChanged(DocumentsChangeType.Attach)
                Return True
            End If
            Return False
        End Function

        Private Sub OnAfterDetached(file As IWorkFile) Implements IWorkFileViewAdapter.OnAfterDetached
            If TypeOf file Is HcvDocument Then
                Using _lockState.Begin
                    TryDetachDocument(CType(file, HcvDocument))
                End Using
            ElseIf TypeOf file Is HarnessAnalyzerProject Then
                'With CType(file, Project)
                '    _isBusyCount = .Documents.Count
                '    Try
                '        'HINT: try to remove all documents from project that have been attached
                '        For Each doc As HcvDocument In .Documents
                '            TryDetachDocument(doc)
                '        Next
                '    Finally
                '        _isBusyCount = 0
                '    End Try
                'End With
            End If
        End Sub

        Private Function TryDetachDocument(document As HcvDocument) As Boolean
            OnDocumentsChanging(DocumentsChangeType.Detach)
            If TryUnRegisterAttached(document) Then
                Dim group As IBaseModelEntityEx = Design3D.Entities.GetGroupOrEntity(document.Id.ToString).SingleOrDefault
                If group IsNot Nothing Then
                    Design3D.CancelWork()
                    System.Threading.SpinWait.SpinUntil(Function() Not Design3D.IsBusy)
                    Design3D.Entities.Remove(group)
#If DEBUG Or CONFIG = "Debug" Then
                    If Design3D.Entities.All.Contains(group) Then
                        Throw New Exception($"Group ""{group.Id}"" was not removed correctly! (Debug exception)")
                    End If
#End If
                    Design3D.Invalidate()
                    OnDocumentsChanged(DocumentsChangeType.Detach)
                End If
                Return True
            End If
            Return False
        End Function

        Private Function TryUnRegisterAttached(doc As HcvDocument) As Boolean
            Dim dsnClone As DocumentDesignClone = Nothing
            If _attached IsNot Nothing Then
                If _attached.TryRemove(doc, dsnClone) Then
                    If dsnClone IsNot Nothing Then
                        RemoveEvents(dsnClone)
                        DetachEEModel(dsnClone.EEModel)
                        dsnClone.Dispose()
                    End If
                    Return True
                End If
            End If
            Return False
        End Function

        Private Sub AddEvents(har As DocumentDesignClone)
            If har.SourceWorkspace IsNot Nothing Then
                AddHandler har.SourceWorkspace.SelectionChanged, AddressOf _env_SelectionChanged ' connect to source-model (clone-source) to attach sync-events
                AddHandler har.Document.Entities.EntitiesVisibilityChanged, AddressOf _document_entities_EntitiesVisibilityChanged
            End If
        End Sub

        Private Sub RemoveEvents(har As DocumentDesignClone)
            If har.SourceWorkspace IsNot Nothing Then
                RemoveHandler har.SourceWorkspace.SelectionChanged, AddressOf _env_SelectionChanged
            End If
            If har.Document IsNot Nothing Then
                RemoveHandler har.Document.Entities.EntitiesVisibilityChanged, AddressOf _document_entities_EntitiesVisibilityChanged
            End If
        End Sub

        Private Sub _env_SelectionChanged(sender As Object, e As devDept.Eyeshot.Workspace.SelectionChangedEventArgs)
            If Not Me.Design3D.IsSelecting Then ' HINT: avoid to process selections which are initiated and circled circled back to our self
                Dim oldEvents As Boolean = Me.Design3D.SelectedEntities.EventsEnabled
                Me.Design3D.SelectedEntities.EventsEnabled = False
                Try
                    Me.Design3D.StopAllBlinking()

                    For Each item As devDept.Eyeshot.Workspace.SelectedItem In e.AddedItems.Concat(e.RemovedItems)
                        Dim entity As IBaseModelEntityEx = CType(item.Item, IBaseModelEntityEx)
                        If Me.Design3D.Entities.Exists(entity.Id) Then
                            For Each ent As IBaseModelEntityEx In Design3D.Entities(entity.Id)
                                ent.Selected = entity.Selected 'sync selection from document3D to OverAllView3D
                                If e.AddedItems.Contains(item) Then
                                    ent.SetBlinkState(True, New devDept.Eyeshot.Entities.BlinkEntitySettings(devDept.Eyeshot.Entities.BlinkStyle.TempEntity, Design3D.ActiveViewport.Background.TopColor), 4)
                                End If
                            Next
                        End If
                    Next

                    Me.Design3D.UpdateVisibleSelection(False) 'HINT: sync SelectedEntities-Collection to current selected entities (without event, because event was already called)
                    If e.AddedItems.Count > 0 AndAlso My.Application.MainForm.GeneralSettings.AutoSync3DSelection Then
                        Me.ZoomFitActiveToSelection()
                    End If
                Finally
                    Me.Design3D.Invalidate()
                    Me.Design3D.SelectedEntities.EventsEnabled = oldEvents
                End Try
            End If
        End Sub

        Private Sub _document_entities_EntitiesVisibilityChanged(sender As Object, e As EntitiesEventArgs)
            For Each entity As IBaseModelEntityEx In e.Entities.OfType(Of IBaseModelEntityEx)
                If Me.Design3D.Entities.Exists(entity.Id) Then
                    For Each ent As IBaseModelEntityEx In Design3D.Entities(entity.Id)
                        ent.Visible = entity.Visible
                    Next
                End If
            Next
        End Sub

        Private Sub AttachEEModel(eeModel As E3.Lib.Model.EESystemModel)
            TryCast(Me.Design3D.EEModel, E3.Lib.Model.EEModelsCollection)?.Add(eeModel)
        End Sub

        Private Sub DetachEEModel(eeModel As E3.Lib.Model.EESystemModel)
            TryCast(Me.Design3D?.EEModel, E3.Lib.Model.EEModelsCollection)?.Remove(eeModel)
        End Sub

        Public Function IsBusy(Optional type As BusyResolveType = BusyResolveType.All) As Boolean Implements IWorkFileViewAdapter.IsBusy
            Return (type.HasFlag(BusyResolveType.Base) AndAlso _lockState.HasAnyState) _
                OrElse (_carStateMachine?.IsBusy()).GetValueOrDefault _
                OrElse (type.HasFlag(BusyResolveType.ReferenceViews) AndAlso Me.IsUpdating)
        End Function

        Private Async Function LoadAllHarnessClonesAsync(harnessClonesCollection As IEnumerable(Of DocumentDesignClone)) As Task(Of AggregatedResult)
            If harnessClonesCollection.Any() Then
                Return Await DocumentDesignClone.LoadAsync(Design3D, harnessClonesCollection) ' only loads documents that don't have been added to a Environment
            Else
                Return AggregatedResult.Success()
            End If
        End Function

        Private Sub _attached_AfterHarnessInitialize(sender As Object, e As AfterHarnessInitializeEventArgs) Handles _attached.AfterHarnessInitialize
            Dim docForm As DocumentForm = GetDocumentFormFromAttached(e.Harness.Document)
            AddEvents(e.Harness) ' attach synchronization-events (selection/visibility) after harness was initialized/added to model
            e.RegenEntities = False ' HINT: disable auto-regen for each single Harness3D because we want to execute this manually
            e.Harness.Group.FilePath = docForm.File.FullName
            e.Harness.Group.IsXhcv = docForm.IsExtendedHCV
        End Sub

        Private Async Sub ProcessOnAfterVisibleChanged()
            Await TryLoadAllAttachedAsyncIfVisible()

            If Me.Parent IsNot Nothing AndAlso Not Me.IsDisposed Then
                If _carStateMachine IsNot Nothing Then
                    _carStateMachine.OnVisibleChanged(Me.Visible)
                End If
            End If
        End Sub

        Private Async Function TryLoadAllAttachedAsyncIfVisible() As Task
            If Me.Parent IsNot Nothing AndAlso Not Me.IsDisposed AndAlso Me.Visible Then
                Await LoadAllAttachedAsync(True) 'HINT: do not wait for exection to avoid blocking of the SetViewAsync-method on document while we are loading the consolidated-view (but multiple calls of this method will be stacked/wait-blocked)
            End If
        End Function

    End Class

End Namespace