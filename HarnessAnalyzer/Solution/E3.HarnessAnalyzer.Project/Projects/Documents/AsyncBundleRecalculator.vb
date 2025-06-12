Imports System.ComponentModel
Imports devDept.Eyeshot.Entities
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.HarnessAnalyzer.Shared
Imports Zuken.E3.Lib.Eyeshot.Model

Namespace Documents

    Friend Class AsyncBundleRecalculator
        Implements IDisposable

        Public Event ProgressChanged(sender As Object, e As ProgressChangedEventArgs)

        Private _cancelRecalculation As Boolean
        Private _recalcLock As New Threading.SemaphoreSlim(1)
        Private _isRecalculating As Boolean
        Private _document As HcvDocument
        Private WithEvents _progress As New ProgressEx()

        Public Sub New(document As HcvDocument)
            _document = document
        End Sub

        Public Function CanRecalculate() As Boolean
            Return _document.IsOpen AndAlso Not _document.IsBusy AndAlso GetFirstEntityNotInModel() Is Nothing
        End Function

        Private Function GetFirstEntityNotInModel() As IBaseModelEntity
            Return _document.Entities.Where(Function(ent) ent.Workspace Is Nothing).FirstOrDefault
        End Function

        Public Async Function RecalculateBundleDiameters(activeSegments As IEnumerable(Of Zuken.E3.Lib.Model.Segment), diameterSettings As DiameterSettings, Optional ignoreBusy As Boolean = False) As Task(Of IResult)
            'HINT MR: the active state of the wires in a segment must be set before in the correspondig variant
            'the same could have been used possibly for the segemnts...

            _cancelRecalculation = True
            Await _recalcLock.WaitAsync()
            _cancelRecalculation = False
            _isRecalculating = True

            Try
                If _document.IsOpen Then
                    Await TaskEx.WaitUntil(Function() ignoreBusy OrElse _cancelRecalculation OrElse Not _document.IsBusy)
                    If Not _cancelRecalculation AndAlso _document.IsOpen Then
                        If activeSegments IsNot Nothing AndAlso activeSegments.Count > 0 Then

                            Dim notInModelEntity As IBaseModelEntity = GetFirstEntityNotInModel()
                            If notInModelEntity IsNot Nothing Then
                                Throw New EntityException($"Can't recalculate: Some entities in document are not attached to model ({NameOf(devDept.Eyeshot.Workspace)}, entity: {notInModelEntity})", notInModelEntity)
                            End If

                            _progress.Init(5) ' HINT: we have 4 sub-work-chunks of progress that need to be summarized into an overall progress-percentage (add more when there are additional chunks implemented: see BeginChunk&EndChunk-blocks)
                            _progress.Report(New ProgressStateInfo(BundleRecalcState.Start))

                            Try
                                Await Task.Factory.StartNew(
                                Sub()
                                    ' HINT: Calculate diameters for each segment and set it to corresponding 3D bundle
                                    Dim updatedBundles As IDictionary(Of String, BundleEntity) = UpdateBundleDiameters(activeSegments, diameterSettings)

                                    If Not _cancelRecalculation Then
                                        With _progress
                                            Using .NewChunk(updatedBundles.Values.Count, userStateEnd:=New ProgressStateInfo(BundleRecalcState.Updating))
                                                For Each bundle As BundleEntity In updatedBundles.Values
                                                    If bundle.StartNode IsNot Nothing Then
                                                        bundle.StartNode.Bundles.Where(Function(bdl) bdl.Visible).Where(Function(bdl) Not updatedBundles.ContainsKey(bdl.Id)).ForEach(Sub(bdl) updatedBundles.Add(bdl.Id, bdl))
                                                    End If
                                                    If bundle.EndNode IsNot Nothing Then
                                                        bundle.EndNode.Bundles.Where(Function(bdl) bdl.Visible).Where(Function(bdl) Not updatedBundles.ContainsKey(bdl.Id)).ForEach(Sub(bdl) updatedBundles.Add(bdl.Id, bdl))
                                                    End If
                                                    .ProgressChunkStep(New ProgressStateInfo(BundleRecalcState.Updating))
                                                    If _cancelRecalculation Then
                                                        Return
                                                    End If
                                                Next
                                            End Using

                                            Using .NewChunk(updatedBundles.Values.Count, userStateEnd:=New ProgressStateInfo(BundleRecalcState.Updating))
                                                ' HINT: See d3dBundle.Update in Paralle.ForEach above why this is done here seperate
                                                For Each bundle As BundleEntity In updatedBundles.Values
                                                    bundle.Update()
                                                    .ProgressChunkStep(New ProgressStateInfo(BundleRecalcState.Updating))
                                                    If _cancelRecalculation Then
                                                        Return
                                                    End If
                                                Next
                                            End Using

                                            Using .NewChunk(updatedBundles.Values.Count, userStateEnd:=New ProgressStateInfo(BundleRecalcState.Updating))
                                                'Update all the protections
                                                For Each bundle As BundleEntity In updatedBundles.Values
                                                    For Each prot As ProtectionEntity In bundle.Protections
                                                        prot.Update()
                                                        If _cancelRecalculation Then
                                                            Return
                                                        End If
                                                    Next
                                                    .ProgressChunkStep(New ProgressStateInfo(BundleRecalcState.Updating))
                                                    If _cancelRecalculation Then
                                                        Return
                                                    End If
                                                Next
                                            End Using

                                            Dim fixings As FixingEntity() = _document.Entities.Where(Function(ent) ent.EntityType = ModelEntityType.Fixing).Cast(Of IEntity).Flatten.Cast(Of FixingEntity).ToArray ' HINT: this can be a FixingEntity or a FixingGroup here (so we need flattern to yield only FixingEntities from groups)
                                            Using .NewChunk(fixings.Count, userStateEnd:=New ProgressStateInfo(BundleRecalcState.Updating))
                                                For Each d3dfixing As FixingEntity In fixings
                                                    Dim fixingBdl As BundleEntity = Nothing
                                                    If updatedBundles.TryGetValue(d3dfixing.BundleEntityId, fixingBdl) Then
                                                        d3dfixing.Radius = fixingBdl.Radius
                                                        d3dfixing.Update()
                                                    End If
                                                    .ProgressChunkStep(New ProgressStateInfo(BundleRecalcState.Updating))
                                                    If _cancelRecalculation Then
                                                        Return
                                                    End If
                                                Next
                                            End Using
                                        End With
                                    End If
                                End Sub, creationOptions:=TaskCreationOptions.LongRunning)
                            Catch ex As Exception
                                Return New Result(ex)
                            Finally
                                _progress.Report(New ProgressStateInfo(BundleRecalcState.Finished))
                            End Try

                            If Not _cancelRecalculation Then
                                Return Result.Success
                            End If
                        End If
                    End If
                Else
                    Throw New Exception($"Can't recalculate bundle diameters because document not opened ({_document.Caption})")
                End If
            Finally
                _isRecalculating = False
                _recalcLock.Release()
            End Try
            Return Result.Cancelled()
        End Function

        Private Function UpdateBundleDiameters(segments As IEnumerable(Of Zuken.E3.Lib.Model.Segment), diameterSettings As DiameterSettings) As IDictionary(Of String, BundleEntity)
            Dim po As New ParallelOptions
            'HINT: Parallel seems to be faster here but we leave one processor for the response and safety (not too much heavy load on all cores)
            po.MaxDegreeOfParallelism = Math.Max(Environment.ProcessorCount - 1, 1)

            Dim diametersBySegment As New Concurrent.ConcurrentDictionary(Of String, Nullable(Of Double))
            Dim bundlesToUpdate As New Concurrent.ConcurrentDictionary(Of String, BundleEntity)
            Dim totalSegs As Integer = segments.Count

            Using _progress.NewChunk(totalSegs, userStateBegin:=New ProgressStateInfo(BundleRecalcState.Calculating), userStateEnd:=New ProgressStateInfo(BundleRecalcState.Calculating))
                Parallel.ForEach(segments, po,
                              Sub(segment As Zuken.E3.Lib.Model.Segment, state As ParallelLoopState)
                                  If _cancelRecalculation Then
                                      state.Break()
                                      Return
                                  End If

                                  Dim calculatedBundleDiameter As Nullable(Of Double) = diametersBySegment.GetOrAdd(segment.Id.ToString, Function() CalculateSegmentDiameter(segment, diameterSettings))
                                  Dim d3dBundle As Zuken.E3.Lib.Eyeshot.Model.BundleEntity = Nothing
                                  If calculatedBundleDiameter.HasValue Then
                                      For Each bundle As BundleEntity In _document.Entities.GetByEEObjectId(segment.Id)
                                          If Not bundle.Override.IsGeometryEnabled Then ' (overwirtten by JT-data -> no recalculate)
                                              bundle.Radius = calculatedBundleDiameter.Value / 2
                                              ' HINT: Update here is not possible because there is sequence depencendy of the bundle updates
                                              bundlesToUpdate.TryAdd(bundle.Id, bundle)
                                          End If
                                      Next

                                      _progress.ProgressChunkStep(New ProgressStateInfo(BundleRecalcState.Calculating))
                                  End If
                              End Sub)
            End Using
            Return bundlesToUpdate
        End Function

        Private Function CalculateSegmentDiameter(segment As Zuken.E3.Lib.Model.Segment, diameterSettings As DiameterSettings) As Nullable(Of Double)
            Dim cancelled As Boolean
            Dim value As Double = DiameterCalculation.CalculateSegmentDiameter(segment, diameterSettings, cancelled)

            If Not cancelled Then
                Return value
            End If

            Return Nothing
        End Function

        Private Sub _progress_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) Handles _progress.ProgressChanged
            OnProgressChanged(e)
        End Sub

        Protected Overridable Sub OnProgressChanged(e As ProgressChangedEventArgs)
            RaiseEvent ProgressChanged(Me, e)
        End Sub

        Friend Class ProgressStateInfo

            Public Sub New(state As BundleRecalcState)
                Me.State = state
            End Sub

            Property State As BundleRecalcState ' HINT: placeholder for additional progress state- information
        End Class


#Region "IDisposable Support"
        Private disposedValue As Boolean ' Dient zur Erkennung redundanter Aufrufe.

        ' IDisposable
        Protected Overridable Async Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    _cancelRecalculation = True
                    'HINT: just wait for recalculation finished
                    Await _recalcLock.WaitAsync
                    _recalcLock.Release()
                    _cancelRecalculation = False
                    _recalcLock.Dispose()
                End If
                _document = Nothing
                _recalcLock = Nothing
            End If
            disposedValue = True
        End Sub

        ' TODO: Finalize() nur überschreiben, wenn Dispose(disposing As Boolean) weiter oben Code zur Bereinigung nicht verwalteter Ressourcen enthält.
        'Protected Overrides Sub Finalize()
        '    ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(disposing As Boolean) weiter oben ein.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(disposing As Boolean) weiter oben ein.
            Dispose(True)
            ' TODO: Auskommentierung der folgenden Zeile aufheben, wenn Finalize() oben überschrieben wird.
            ' GC.SuppressFinalize(Me)
        End Sub

#End Region

    End Class

    Public Enum BundleRecalcState
        None = 0
        Calculating = 1
        Updating = 2
        Start = 3
        Finished = 4
    End Enum

End Namespace