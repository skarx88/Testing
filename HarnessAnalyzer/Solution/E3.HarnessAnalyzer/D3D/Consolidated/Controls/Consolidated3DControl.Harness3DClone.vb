Imports System.ComponentModel
Imports System.IO
Imports devDept.Eyeshot
Imports Zuken.E3.HarnessAnalyzer.D3D.Consolidated.Designs
Imports Zuken.E3.HarnessAnalyzer.D3D.Document
Imports Zuken.E3.HarnessAnalyzer.Project
Imports Zuken.E3.HarnessAnalyzer.Project.Documents
Imports Zuken.E3.Lib.Eyeshot.Model
Imports Zuken.E3.Lib.IO.Files

Namespace D3D.Consolidated.Controls

    Partial Public Class Consolidated3DControl

        Public Class DocumentDesignClone
            Implements IDisposable
            Implements IBaseFileProvider

            Private _addedAndInitialized As New HashSet(Of DocumentDesign)
            Private _disposedValue As Boolean
            Private _source As IWorkFileViewAdapter

            Public Event BeforeInitialize(sender As Object, e As BeforeInitializeEventArgs)
            Public Event AfterInitialize(sender As Object, e As AfterInitializeEventArgs)

            Public Sub New(document As HcvDocument, source As IWorkFileViewAdapter)
                _source = source
                Me.Document = document
                Me.EEModel = document?.Model
            End Sub

            Property Owner As DocumentDesignClonesCollection

            Public Shared Async Function LoadAsync(design As DocumentDesign, clones As IEnumerable(Of DocumentDesignClone)) As Task(Of AggregatedResult)
                clones = clones.Where(Function(cln) Not cln.IsAdded).ToArray ' not loaded
                If clones.Any() Then
                    Dim results As New List(Of IResult)

                    For Each group As IGrouping(Of HcvDocument, DocumentDesignClone) In clones.GroupBy(Function(c) c.Document).ToArray
                        Dim workers As IWorkChunkAsync() = group.Select(Function(clone) clone.GetInitAndAddWork(design)).ToArray
                        results.Add(Await group.Key.Project.DoWorkChunksAsync(workers)) ' execute project-wide because this worker is not document specific (waits for all other document-workers, f.e. and thus it only locks execution of workers which are also project-wide, the document-specific-workers are not locked and blocked by each other)

                        Dim addedWorkspace As Designs.ConsolidatedDesign = GetAddedWorkSpace(group)
                        If addedWorkspace IsNot Nothing Then
                            Await DocumentDesignClone.ProcessRegenEntities(addedWorkspace) ' HINT: manually regen entities here because we always disabled it throu the AfterInitializedEvent (-> regen entities only a single time when more than one documents are loaded at once)
                        End If
                    Next

                    Await design.Entities.UpdateAsync(EntityUpdateType.All, UseWorkerOptions.True, UpdateRegenOptions.RegenUpdateWhenNeeded)

                    Return New AggregatedResult(results)
                Else
                    Return AggregatedResult.Success
                End If
            End Function

            Private Shared Function GetAddedWorkSpace(designs As IEnumerable(Of DocumentDesignClone)) As ConsolidatedDesign
                Return designs.Where(Function(cln) cln.AddedWorkspace IsNot Nothing).Select(Function(cln) cln.AddedWorkspace).Cast(Of Designs.ConsolidatedDesign).Distinct.SingleOrDefault
            End Function

            Public Function GetInitAndAddWork(docDesign As DocumentDesign) As IWorkChunkAsync
                If docDesign Is Nothing Then
                    Throw New ArgumentException($"Parameter with {NameOf(DocumentDesign)} is null!", NameOf(docDesign))
                End If

                Return New WorkUnitWrappedWorkChunk(New InitAndAddRangeWorkChunk(Me, docDesign))
            End Function

            ReadOnly Property SourceView As IWorkFileViewAdapter
                Get
                    Return _source
                End Get
            End Property

            Protected Overridable Sub OnBeforeInitialize(e As BeforeInitializeEventArgs)
                RaiseEvent BeforeInitialize(Me, e)
            End Sub

            Protected Overridable Sub OnAfterInitialize(e As AfterInitializeEventArgs)
                RaiseEvent AfterInitialize(Me, e)
            End Sub

            Friend Shared Async Function ProcessRegenEntities(env As devDept.Eyeshot.Workspace) As Task
                'HINT: regen needed here, because zoomfit will not work correctly after for cloned entities
                With CType(env, DocumentDesign)
                    Await .Entities.RegenAsync()
                    env.SetViewToInitialView(True, False)
                    .Invalidate()
                End With
            End Function

            Public Function FindDocument3DControl() As D3D.Document.Controls.Document3DControl
                Return Me.Document.View.Flatten.OfType(Of D3D.Document.Controls.Document3DControl).SingleOrDefault
            End Function

            Public Function IsInitialized(Optional docModel As DocumentDesign = Nothing) As Boolean
                Return (docModel Is Nothing AndAlso _addedAndInitialized.Count > 0) OrElse (_addedAndInitialized.Contains(docModel))
            End Function

            Property Document As HcvDocument
            Property Group As D3D.Consolidated.Designs.Group3D

            ReadOnly Property EEModel As E3.Lib.Model.EESystemModel
            ReadOnly Property SourceWorkspace As devDept.Eyeshot.Workspace
            ReadOnly Property AddedWorkspace As DocumentDesign

            ReadOnly Property IsAdded As Boolean
                Get
                    Return Me.AddedWorkspace IsNot Nothing
                End Get
            End Property

            Private ReadOnly Property File As IBaseFile Implements IBaseFileProvider.File
                Get
                    Return Me.Document
                End Get
            End Property

            Protected Overridable Sub Dispose(disposing As Boolean)
                If Not _disposedValue Then
                    If disposing Then
                        _addedAndInitialized?.Clear()
                    End If

                    _addedAndInitialized = Nothing
                    _source = Nothing
                    _Owner = Nothing
                    _Document = Nothing
                    _Group = Nothing
                    _SourceWorkspace = Nothing
                    _AddedWorkspace = Nothing
                    _EEModel = Nothing
                    _disposedValue = True
                End If
            End Sub

            Public Sub Dispose() Implements IDisposable.Dispose
                ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
                Dispose(disposing:=True)
                GC.SuppressFinalize(Me)
            End Sub

            Public Class InitAndAddRangeWorkChunk
                Inherits WorkChunkAsync
                Implements IProvidesWorkViewAdapter

                Private _harnessClone As DocumentDesignClone
                Private _docDesign As DocumentDesign
                Private _result As IResult

                Public Sub New(harness3D As DocumentDesignClone, docDesign As DocumentDesign)
                    MyBase.New(Consolidated3DControlStrings.Updating3DData, ComponentModel.ProgressState.Updating)
                    _harnessClone = harness3D
                    _docDesign = docDesign
                End Sub

                Public ReadOnly Property View As IWorkFileViewAdapter Implements IProvidesWorkViewAdapter.View
                    Get
                        Return _harnessClone?.SourceView
                    End Get
                End Property

                Protected Overrides ReadOnly Property File As IO.IBaseFile
                    Get
                        Return _harnessClone.File
                    End Get
                End Property

                Private Function DocumentIsBusyExceptSourceView() As Boolean
                    If _harnessClone?.Document IsNot Nothing AndAlso _harnessClone.Document.IsBusy(BusyResolveType.All Xor BusyResolveType.ReferenceViews) Then
                        Return True
                    End If

                    'remove us from busy check because we are waiting for all others and this would lead to an dead-lock
                    If _harnessClone?.Document?.View IsNot Nothing AndAlso _harnessClone.Document.View.Flatten.Except({_harnessClone.SourceView}).Any(Function(v) v.IsBusy) Then
                        Return True
                    End If
                    Return False
                End Function

                Protected Overrides Sub DoWorkCore()
                    Dim args As New BeforeInitializeEventArgs(_docDesign)
                    _harnessClone.OnBeforeInitialize(args)

                    If Not args.Cancel Then
                        If DocumentIsBusyExceptSourceView() Then
                            Dim oldDescription As String = Me.Description
                            Me.Description = Consolidated3DControlStrings.WaitForDocumentsFinishedLoading
                            Try
                                Me.ProgressReport(0, ProgressInfo.Intermediate)
                                System.Threading.SpinWait.SpinUntil(Function() Not DocumentIsBusyExceptSourceView()) 'HINT: wait until everything else (except 'us': we ware busy in waiting HERE!) is not busy
                            Finally
                                Me.Description = oldDescription
                            End Try
                        End If

                        If (_harnessClone?.Document?.IsOpen).GetValueOrDefault Then
                            _harnessClone.Group = New D3D.Consolidated.Designs.Group3D(_harnessClone.Document.Id.ToString)

                            Dim entities As IBaseModelEntityEx() = _harnessClone.Document.Entities.ToArray
                            Me.Maximum = entities.Length + 2 ' HINT: +2 for the addrange-part + the regenEntities-part

                            Dim i As Integer = -1
                            For Each entity As IBaseModelEntityEx In entities
                                i += 1
                                Dim clone As IBaseModelEntityEx
                                Try
                                    clone = CType(entity.Clone, IBaseModelEntityEx)
                                Catch ex As Exception
                                    Throw New Exception($"Error cloning entity ""{entity.GetType.Name}"" (DisplayName: {entity.DisplayName}, Id: {entity.Id}): {ex.Message}", ex)
                                End Try

                                If _harnessClone.Group.Contains(clone) Then
                                    'HINT: shoudn't be possible under normal conditions but implemented for error-case when something went/implemented wrong with the BL before!
                                    Throw New OperationCanceledException("Already added to document-group!")
                                    Debug.WriteLine($"WTF: Cloned entity ({clone.DisplayName}, index: {i.ToString}) already existis in {_harnessClone.GetType.Name}.{NameOf(_harnessClone.Group)} ? Shoudn't be possible, because normally new instance is created every time in this foreach loop ({Me.GetType.Name}) !")
                                End If

                                _harnessClone.Group.Add(clone)  ' return false is possible when f.e. this is an empty fixingGroup, nothing will be added to group because group exists but with no content

                                If Me.Owner IsNot Nothing Then
                                    Me.ProgressStep()
                                End If
                            Next

                            _harnessClone._SourceWorkspace = TryCast((entities.LastOrDefault?.Workspace), Workspace)

                            Dim args2 As New AfterInitializeEventArgs(_docDesign)
                            _harnessClone.OnAfterInitialize(args2)

                            If args2.AddEntities Then
                                _docDesign.Entities.Add(_harnessClone.Group, False) 'Then ' HINT: using the clones to add it to 3D view --- TODO: move this to the place when 3D view is opened, HINT2: add group and not the content-entities of the group by using the {}-pattern (like the sync-add-command but using the AddRangeAsync-here to add the group-content async + the group itself)  

                                If Me.Owner IsNot Nothing Then
                                    Me.ProgressStep(New UserStateProgressInfo(ProgressType, Nothing)) ' +1
                                End If

                                _harnessClone._AddedWorkspace = _docDesign
                                _harnessClone._addedAndInitialized.Add(_docDesign)

                                If args2.RegenEntities Then
                                    Dim tsk2 As Task = ProcessRegenEntities(_harnessClone.AddedWorkspace)
                                    tsk2.Wait()
                                End If

                                If Me.Owner IsNot Nothing Then
                                    Me.ProgressStep(New UserStateProgressInfo(ProgressType, Nothing)) ' +2
                                End If
                            End If
                        End If
                    End If
                End Sub

                Protected Overrides Sub Dispose(disposing As Boolean)
                    MyBase.Dispose(disposing)
                    If disposing Then

                    End If
                    _result = Nothing
                    _harnessClone = Nothing
                    _docDesign = Nothing
                End Sub

                Public Overrides Function GetResult() As Object
                    Return _result
                End Function
            End Class

        End Class

    End Class

End Namespace
