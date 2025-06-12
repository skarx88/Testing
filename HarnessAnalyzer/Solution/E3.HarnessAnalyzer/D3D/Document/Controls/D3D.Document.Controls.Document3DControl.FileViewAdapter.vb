Imports System.ComponentModel
Imports devDept.Eyeshot
Imports Zuken.E3.HarnessAnalyzer.Project.Documents
Imports Zuken.E3.Lib.Eyeshot.Model
Imports Zuken.E3.Lib.IO.Files

Namespace D3D.Document.Controls

    Partial Public Class Document3DControl
        Implements IWorkFileViewAdapter 'HINT: here we are bridging the progress/message/etc. from each work-chunk from the document to our gui-element (eyeshot)

        Public Function OnBeforeStartWorkAsync(state As IWorkState, userData As Object) As Task(Of Boolean) Implements IWorkFileViewAdapter.OnBeforeStartWorkAsync
            Return Task.FromResult(True)
        End Function

        Private Function OnWorkFinishedAsync(state As IWorkState, result As IResult) As Task Implements IWorkFileViewAdapter.OnWorkFinishedAsync
            ' nothing currently to do here
            Return Task.CompletedTask
        End Function

        Private Async Function OnAfterAttachedAsync(file As IWorkFile) As Task(Of IResult) Implements IWorkFileViewAdapter.OnAfterAttachedAsync
            If TypeOf file Is HcvDocument Then
                _document = CType(file, HcvDocument)
                With Model3DControl1.Design
                    Using Await _busyState.BeginWaitAsync
                        Using .EnableWaitCursor
                            .CancelWork() ' stop all work
                            .Viewports(0).ToolBar.Visible = False
                            Model3DControl1.AttachDocument(CType(file, HcvDocument))
                            .ZoomFitMode = devDept.Eyeshot.zoomFitType.ConvexHull ' performace tweak for heavy 3D models
                            .Entities.Clear()
                            Dim resAdd As IResult = Await .Entities.AddRangeAsync(CType(file, HcvDocument).Entities, New UseWorkerOptions(False))
                            Dim res As IResult = Await .Entities.UpdateAsync(EntityUpdateType.All, Nothing, UpdateRegenOptions.True)
                            .SetViewToInitialView(True, False)
                            .Invalidate()
                            Return res
                        End Using
                    End Using
                End With
            Else
                Throw New ArgumentException($"Invalid {NameOf(file)}-type: {file.GetType.FullName}. Can only attach file-types of {GetType(HcvDocument).FullName} to view {Me.GetType.FullName} !")
            End If
        End Function

        Private Sub OnAfterDetached(file As IWorkFile) Implements IWorkFileViewAdapter.OnAfterDetached
            If _document Is file Then
                _document = Nothing
            End If
        End Sub

        Private Function OnBeforeChunkWork(chunk As IWorkChunk) As Task(Of Boolean) Implements IWorkFileViewAdapter.OnBeforeChunkWork
            TryCast(chunk, DocumentWorkUnitWrappedWorkChunk)?.AttachWorkspace(Model3DControl1.Design)
            Return Task.FromResult(True)
        End Function

        Private Function OnAfterChunkWork(e As WorkChunkFinishedEventArgs) As Task Implements IWorkFileViewAdapter.OnAfterChunkWork
            TryCast(e.WorkChunk, DocumentWorkUnitWrappedWorkChunk)?.DetachWorkspace()
            Return Task.CompletedTask
        End Function

        Private Function IsBusy(Optional type As BusyResolveType = BusyResolveType.All) As Boolean Implements IWorkFileViewAdapter.IsBusy
            Return (type.HasFlag(BusyResolveType.Base) AndAlso _busyState.HasAnyState) Or (type.HasFlag(BusyResolveType.ReferenceViews) AndAlso (TryCast(ParentForm, DocumentForm)?.IsBusy).GetValueOrDefault)
        End Function

    End Class

End Namespace