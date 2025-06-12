Imports System.ComponentModel
Imports System.Runtime.Serialization
Imports devDept.Eyeshot.Entities
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.HarnessAnalyzer.Shared
Imports Zuken.E3.Lib.Eyeshot.Model
Imports Zuken.E3.Lib.Eyeshot.Model.Converter
Imports Zuken.E3.Lib.IO.Files
Imports Zuken.E3.Lib.IO.Files.Hcv
Imports Zuken.E3.Lib.IO.Files.Project
Imports Zuken.E3.Lib.Locator.Splice
Imports Zuken.E3.Lib.Model
Imports Zuken.E3.Lib.Schema.Kbl

Namespace Documents

    Public Class HcvDocument
        Inherits OcsFile
        Implements IEEModelProvider
        Implements IKblProvider

        Public Shared USE_CONTAINER_IDS As ContainerId() = New ContainerId() {
            ContainerId.Segments,
            ContainerId.Vertices,
            ContainerId.Connectors,
            ContainerId.Components,
            ContainerId.AdditionalParts,
            ContainerId.Protections,
            ContainerId.Supplements}

        Private WithEvents _bundleRecalculator As New AsyncBundleRecalculator(Me)
        Private WithEvents _entities As New DocumentEntitiesCollection(Me)
        Private WithEvents _spliceLocator As SpliceLocator

        Private _hcvFile As HcvFile
        Private _kbl As KblMapper
        Private _converted As ConvertModelResult
        Protected _lock As New System.Threading.LockStateMachine

        Public Event EntitiesUpdated(sender As Object, e As EntitiesUpdatedEventArgs)

        Protected Friend Sub New()
            MyBase.New
        End Sub

        Public Sub New(hcv As HcvFile)
            MyBase.New(hcv.FullName)
            _hcvFile = hcv
        End Sub

        Protected Friend Sub New(info As SerializationInfo, context As StreamingContext)
            MyBase.New(info, context)
        End Sub

        ReadOnly Property Entities As DocumentEntitiesCollection
            Get
                Return _entities
            End Get
        End Property

        Private ReadOnly Property EEModel As IEEModel Implements IEEModelProvider.EEModel
            Get
                Return Me.Model
            End Get
        End Property

        Private ReadOnly Property Kbl_IKBLProvider As IKblBaseContainer Implements IKblProvider.Kbl
            Get
                Return Me.Kbl
            End Get
        End Property

        ReadOnly Property SchemaVersion As KblSchemaVersion
            Get
                Return _kbl.KblSchemaVersion
            End Get
        End Property

        Public Property Kbl As KblMapper
            Get
                If _kbl Is Nothing Then
                    If Not IsOpen Then
                        Throw New Exception($"Can't access {NameOf(KBL_container)} because document is not open: {Me.GetType.FullName} ({IO.Path.GetFileName(Me.FullName)})")
                    End If
                    If _hcvFile Is Nothing Then
                        Throw New Exception($"HcvFile not set to document: {Me.GetType.FullName}")
                    End If
                End If
                Return _kbl
            End Get
            Set(value As KblMapper)
                _kbl = value
            End Set
        End Property

        ReadOnly Property Model As EESystemModel

        ''' <summary>
        ''' Variant used to enable the objects as active or inactive by the document business logic
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property VariantUsedToActivate As [Variant]
        ReadOnly Property LoadedContent As LoadContentType = LoadContentType.None

        Property IsLastOfXhcv As Boolean
        Property IsXhcv As Boolean

        Public Shadows ReadOnly Property Project As BaseProject(Of HcvDocument)
            Get
                Return CType(MyBase.Project, BaseProject(Of HcvDocument))
            End Get
        End Property

        Public Sub SetModelObjectsActiveByKblIds(kblIds As IEnumerable(Of String), active As Boolean)
            If Model IsNot Nothing Then

                Dim idsCount As Integer = kblIds.Count
                For Each ent As IBaseModelEntity In Me.Entities
                    ent.Selectable = True
                Next

                For Each ent As UnboundEntity In Me.Entities.OfType(Of UnboundEntity)
                    ent.Override.IsDrawnActive = If(idsCount = 0, Nothing, New Nullable(Of Boolean)(False)) ' HINT: disable only all UnboundEntities when there are entities to be set, when no entities to be set the overrride state is reset to default (=nothing) which should enable it back normally.
                Next

                For Each obj As Zuken.E3.Lib.Model.ObjectBaseNaming In Model.OfType(Of Zuken.E3.Lib.Model.ObjectBaseNaming)
                    obj.TryActivateInVariantNoInherit(VariantUsedToActivate, True)
                Next

                Dim objectsInheritOptionSaved As New Concurrent.ConcurrentDictionary(Of Zuken.E3.Lib.Model.ObjectBase, Boolean)

                'TODO: check InheritOption (currently disabled to be the same as the current CanvasFilter that is ignoring cavitites for filtering ?)
                For Each obj As Zuken.E3.Lib.Model.ObjectBaseNaming In Me.Entities.GetByKblIds(kblIds.ToArray).SelectMany(Function(kbl_obj) kbl_obj.GetEEModelObjects).OfType(Of ObjectBaseNaming)
                    Dim inheritOption As Boolean
                    If InheritOptionBlock.TryGetInheritOptionValue(obj, inheritOption) Then
                        objectsInheritOptionSaved.TryAdd(obj, inheritOption)
                        InheritOptionBlock.TrySetInheritOptionValue(obj, False)
                    End If

                    obj.ActivateInVariant(VariantUsedToActivate, active)

                    Dim seg As Zuken.E3.Lib.Model.Segment = TryCast(obj, Zuken.E3.Lib.Model.Segment)
                    If seg IsNot Nothing Then
                        For Each prot As Zuken.E3.Lib.Model.Protection In seg.GetProtections.Entries
                            prot.ActivateInVariant(VariantUsedToActivate, active)
                            For Each protEnt As IBaseModelEntity In Me.Entities.GetByEEObjectId(prot.Id)
                                protEnt.Selectable = active
                            Next
                        Next
                    End If

                    For Each ent As IBaseModelEntity In Me.Entities.GetByEEObjectId(obj.Id)
                        ent.Selectable = active
                    Next
                Next

                For Each objValueSaved As KeyValuePair(Of ObjectBase, Boolean) In objectsInheritOptionSaved
                    InheritOptionBlock.TrySetInheritOptionValue(objValueSaved.Key, objValueSaved.Value)
                Next

            End If
        End Sub

        Public Function HasState(state As OcsState) As Boolean
            Return Me.HasState(state, True) OrElse Me.HasState(state, False)
        End Function

        ''' <summary>
        ''' Check the finished/running working states on the document
        ''' </summary>
        ''' <param name="state">the corresponding work-state to be checked</param>
        ''' <param name="isWorkingState">true when needed to check if the state is currently a state that is doing unfinished work, false if to check a state that has successfully finished doing work (f.e. check if the document was opened use state.open and isworkingstate=false, check opening use isworkingstate=true instead) </param>
        ''' <returns></returns>
        Public Function HasState(state As OcsState, isWorkingState As Boolean) As Boolean
            If isWorkingState Then
                Return MyBase.HasAnyWorkingState(state)
            Else
                Return MyBase.HasAnySuccessState(state)
            End If
        End Function

        Public Function CanRecalculateBundleDiameters() As Boolean
            Return (_bundleRecalculator?.CanRecalculate()).GetValueOrDefault
        End Function

        Public Async Function RecalculateBundleDiameters(activeSegments As IEnumerable(Of Zuken.E3.Lib.Model.Segment), diameterSettings As DiameterSettings, Optional ignoreBusy As Boolean = False) As Task(Of IResult)
            If _bundleRecalculator IsNot Nothing Then
                Dim res As IResult = Await _bundleRecalculator.RecalculateBundleDiameters(activeSegments, diameterSettings, ignoreBusy)
                If res.IsSuccess Then
                    Await Entities.RegenAsync()
                    Entities.Invalidate()
                End If
                Return res
            End If
            Return Result.Cancelled("No bundle recalculator available!")
        End Function

        Public Function ShowAsNewTemporalEntities(traces As IEnumerable(Of IKblDistanceEntry), Optional zoomfit As Boolean = True) As TemporalEntitiesResult
            Dim meshes As New List(Of MeshEx)
            For Each trace As IKblDistanceEntry In traces
                For Each bundle As BundleEntity In Me.Entities.GetByKblIds(trace.KblSystemId, trace.KblId).OfType(Of BundleEntity)
                    Dim mesh As MeshEx = bundle.Cut(trace.Start, trace.End)
                    If mesh IsNot Nothing Then
                        meshes.Add(mesh)
                    End If
                Next
            Next

            Dim tempEntities As New TemporalEntitiesResult(Me, meshes)
            tempEntities.AddAsTempEntitiesAndRegen(tempEntities.Workspace?.Selection.Color)
            If zoomfit Then
                tempEntities.ZoomFit()
            End If
            tempEntities.DesignInvalidate()
            Return tempEntities
        End Function

        Private Sub _bundleRecalculator_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) Handles _bundleRecalculator.ProgressChanged
            Dim stateInfo As AsyncBundleRecalculator.ProgressStateInfo = CType(e.UserState, AsyncBundleRecalculator.ProgressStateInfo)
            OnProgress(New ProgressChangedEventArgs(e.ProgressPercentage, New BundleRecalcProgressStateInfo(ProgressState.Updating, stateInfo.State)))
        End Sub

        Protected Overridable Sub OnEntitiesUpdated(e As EntitiesUpdatedEventArgs)
            RaiseEvent EntitiesUpdated(Me, e)
        End Sub

        Private Sub _entities_Updated(sender As Object, e As EntitiesUpdatedEventArgs) Handles _entities.Updated
            OnEntitiesUpdated(e)
        End Sub

        Public Shared Sub ProcessNeedsEEObjectData(e As EEObjectDataEventArgs, document As HcvDocument)
            ' HINT: override the isActive-variant check from to default to our specific document variant which is used to control the disabled/enabled-state of the objects by the document (see Document.ISACTIVE_VARIANT_NAME)
            If e.ResultData IsNot Nothing Then
                Dim obj As Zuken.E3.Lib.Model.ObjectBase = document.Model(e.EEObjectId)
                If obj IsNot Nothing Then
                    e.ResultData.IsInActiveVariant = obj.IsInVariantNoInherit(document.VariantUsedToActivate)
                End If
            End If
        End Sub

        Public Shadows Async Function DoWork(ParamArray workChunks() As WorkChunk) As Task(Of IResult)
            Return Await DoWork(Nothing, workChunks)
        End Function

        Public Shadows Async Function DoWork(userData As Object, workChunks() As WorkChunk) As Task(Of IResult)
            Using Await _lock.BeginWaitAsync(Nothing) 'HINT: nothing = wait for all current locks to be finished and not only the block of this method before proceeding
                Dim wrapped As New List(Of IWorkChunk)
                For Each wc As WorkChunk In workChunks
                    If wc Is Nothing Then
                        Throw New ArgumentNullException("Provided workChunk can't be nothing: some item in provided collection is null!", NameOf(workChunks))
                    End If
                    wrapped.Add(GetWrappedOrDefault(False, wc))
                Next
                Return Await DoWorkChunksAsync(Nothing, userData, wrapped)
            End Using
        End Function

        Protected Overrides Async Function DoWorkChunksAsync(state As E3.Lib.IO.Files.WorkState, userData As Object, workChunks As IEnumerable(Of IWorkChunk)) As Task(Of IResult)
            Return TryConvertToDocumentResult(Await MyBase.DoWorkChunksAsync(state, userData, workChunks), Function() Me)
        End Function

        Private Shared Function GetDocumentResult(res As WorkChunkResult, Optional getDocumentIfNull As Func(Of IWorkChunk, HcvDocument) = Nothing) As DocumentResult
            Dim document As Documents.HcvDocument = Nothing
            If TypeOf res.WorkChunk.Owner Is Documents.HcvDocument Then
                Return New DocumentResult(res, CType(res.WorkChunk.Owner, Documents.HcvDocument))
            ElseIf TypeOf res.WorkChunk Is System.IO.IBaseFileProvider Then
                document = TryCast(TryCast(res.WorkChunk, System.IO.IBaseFileProvider)?.File, Documents.HcvDocument)
            End If
            Return New DocumentResult(res, If(document, getDocumentIfNull?.Invoke(res.WorkChunk)))
        End Function

        Friend Shared Function TryConvertToDocumentResult(result As IResult, Optional getDocumentIfNull As Func(Of IWorkChunk, HcvDocument) = Nothing) As IResult
            Dim results As New Dictionary(Of IWorkChunk, IResult)
            If TypeOf result Is IAggregatedResult Then
                For Each r As WorkChunkResult In CType(result, IAggregatedResult).Results.OfType(Of WorkChunkResult)
                    results.TryAdd(r.WorkChunk, GetDocumentResult(r, getDocumentIfNull))
                Next
            ElseIf TypeOf result Is WorkChunkResult Then
                results.TryAdd(CType(result, WorkChunkResult).WorkChunk, GetDocumentResult(CType(result, WorkChunkResult), getDocumentIfNull))
            Else
                results.Add(Nothing, result)
            End If

            Return New AggregatedResult(results.Values)
        End Function

        Public Overrides Property FullName As String
            Get
                Return _hcvFile?.FullName
            End Get
            Set(value As String)
                Throw New NotSupportedException("Setting fullname is not supported in HcvDocument because the fullName is resolved by the provided HcvFile-Object and must be changed there!")
            End Set
        End Property

        Property File As HcvFile
            Get
                Return _hcvFile
            End Get
            Set(value As HcvFile)
                If _hcvFile IsNot value Then
                    OnPropertyChangingAuto
                    _hcvFile = value
                    OnPropertyChanged(New PropertyChangedEventArgs(NameOf(File)))
                End If
            End Set
        End Property

        Public Overrides Property Caption As String
            Get
                If Not String.IsNullOrEmpty(_hcvFile?.FullName) Then
                    Return IO.Path.GetFileNameWithoutExtension(_hcvFile?.FullName)
                End If
                Return String.Empty
            End Get
            Set(value As String)
                Throw New NotSupportedException("Setting fullname is not supported in HcvDocument because the caption is resolved by the provided HcvFile-Object-FullName and must be changed there!")
            End Set
        End Property

        Protected Overrides Sub Initialize(data As Object)
            If TypeOf data Is String Then
                If System.IO.PathEx.IsValidPath(CStr(data)) Then
                    _hcvFile = HcvFile.Create(CStr(data))
                Else
                    Throw New ArgumentException($"Can't initialize because the given path is not valid: {CStr(data)}", NameOf(data))
                End If
            End If
        End Sub

    End Class

End Namespace