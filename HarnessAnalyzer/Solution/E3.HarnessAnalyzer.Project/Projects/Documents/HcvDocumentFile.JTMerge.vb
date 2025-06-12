Imports System.ComponentModel
Imports System.IO
Imports devDept.Eyeshot.Entities
Imports devDept.Geometry
Imports Zuken.E3.Lib.Eyeshot.Model
Imports Zuken.E3.Lib.Eyeshot.Model.Converter
Imports Zuken.E3.Lib.IO.Files
Imports Zuken.E3.Lib.IO.Files.Hcv
Imports Zuken.E3.Lib.IO.KBL
Imports Zuken.E3.Lib.Model

Namespace Documents

    Partial Public Class HcvDocument

        Private Class MergeJTEntitiesWorkChunk
            Inherits WorkChunkAsync
            Implements IDocumentWorkChunk

            Private _document As HcvDocument
            Private _convertResult As ConvertModelResult = Nothing
            Private _jtSettings As ContentSettings.JTSettings

            Public Sub New(document As HcvDocument, entitiesConverted As ConvertModelResult, jtSettings As ContentSettings.JTSettings)
                MyBase.New("Merge JT entities...", ProgressState.Updating)
                _document = document
                _convertResult = entitiesConverted
                _jtSettings = jtSettings
            End Sub

            Public Sub New(document As HcvDocument, jtSettings As ContentSettings.JTSettings)
                Me.New(document, Nothing, jtSettings)
            End Sub

            Public ReadOnly Property ChunkType As DocumentWorkChunkType Implements IDocumentWorkChunk.ChunkType
                Get
                    Return DocumentWorkChunkType.MergeJTEntities
                End Get
            End Property

            Protected Overrides Sub DoWorkCore()
                Dim allEntitiesToMerge As Entity() = CType((Me.Owner.OfType(Of IDocumentWorkChunk).Where(Function(chunk) chunk.ChunkType = DocumentWorkChunkType.ReadJTEntities).SingleOrDefault?.GetResult), IEnumerable(Of Entity)).ToArray

                If allEntitiesToMerge IsNot Nothing Then
                    If _convertResult Is Nothing Then
                        _convertResult = CType((Me.Owner.OfType(Of IDocumentWorkChunk).Where(Function(chunk) chunk.ChunkType = DocumentWorkChunkType.ConvertModel).SingleOrDefault?.GetResult), ConvertModelResult)
                    End If

                    If _convertResult IsNot Nothing Then
                        Dim mergeData As New MergeData(_convertResult, _document.Model)
                        Dim groups As List(Of IGrouping(Of String, Entity)) = allEntitiesToMerge.GroupBy(Function(ent) TryCast(ent.EntityData, devDept.Eyeshot.Translators.JTEntityData)?.Id.ToString).ToList

                        Me.Maximum = groups.Count
                        Dim mergedOrAdded As New List(Of IBaseModelEntity)

                        For Each entityGrp As IGrouping(Of String, Entity) In groups
                            Dim mergedOrAddedInGroup As New List(Of IBaseModelEntity)
                            If Not String.IsNullOrEmpty(entityGrp.Key) Then
                                Dim mObjects As List(Of ObjectBaseNaming) = mergeData.GetModelObjectsByJTLinkID(entityGrp.Key) 'HIN: it's possible that we have different entities with the same JT-id (see gemini issue E3HA-1566)
                                For Each groupModelObject As ObjectBaseNaming In mObjects
                                    If groupModelObject IsNot Nothing Then
                                        Dim overwriteGeomertry As Boolean = _jtSettings.OverwriteGeomertry.Contains(groupModelObject.HostContainerId)
                                        If overwriteGeomertry OrElse _jtSettings.UseColors Then
                                            Dim jtSourceEntity As New ConsoliatedMeshInfo(entityGrp.OfType(Of Mesh))
                                            If (jtSourceEntity.HasData) Then
                                                Select Case groupModelObject.HostContainerId
                                                    Case ContainerId.Components, ContainerId.Protections, ContainerId.AdditionalParts, ContainerId.Supplements
                                                        'HINT: add new entity or replace it's vertices/triangles when [objectBase] was already converted before
                                                        Dim entities As IEnumerable(Of IBaseModelEntity) = _convertResult.AddNewOrGet(groupModelObject)
                                                        If entities IsNot Nothing Then
                                                            For Each entity As IBaseModelEntity In entities
                                                                MergeTo(entity, jtSourceEntity, overwriteGeomertry, _jtSettings.UseColors)
                                                                If overwriteGeomertry Then
                                                                    _convertResult.TryAdd(entity)
                                                                End If
                                                                ThrowCancelIfRequested() 'HINT: additional more granular cancellation-check
                                                            Next
                                                            If overwriteGeomertry Then
                                                                mergedOrAddedInGroup.AddRange(entities)
                                                            End If
                                                        End If
                                                    Case Else 'HINT: for default setting, always replace vertices/triangles of every found connector, and/or when use dynamic feature is active replace the rest (Bundles/Segments + Nodes/Vertices)
                                                        Dim targetEnt As IBaseModelEntity = mergeData.GetTargetEntitiesByJTLinkId(entityGrp.Key).SingleOrDefault
                                                        If targetEnt IsNot Nothing Then
                                                            MergeTo(targetEnt, jtSourceEntity, overwriteGeomertry, _jtSettings.UseColors)
                                                            If overwriteGeomertry Then
                                                                mergedOrAddedInGroup.Add(targetEnt)
                                                            End If
                                                        End If
                                                End Select
                                            End If
                                        End If
                                    Else
                                        ' we have an jt-entity that does not exist in model -> we create an non-selectable unbound entity that has the only purpose to show this additional JT Entity data to the user
                                        Dim jtEntities As Mesh() = entityGrp.OfType(Of Mesh).ToArray
                                        If jtEntities.Length > 0 Then
                                            Dim jtSourceEntityInfo As New ConsoliatedMeshInfo(jtEntities)
                                            If (jtSourceEntityInfo.HasData) Then
                                                Dim newEntity As New UnboundEntity
                                                MergeTo(newEntity, jtSourceEntityInfo, True, True)
                                                _convertResult.Add(newEntity)
                                                mergedOrAddedInGroup.Add(newEntity)
                                            End If
                                        End If
                                    End If
                                Next
                            End If

                            mergedOrAdded.AddRange(mergedOrAddedInGroup)

                            Dim info As ProgressUpdateEntityInfo = ProgressUpdateEntityInfo.GetUpdating(mergedOrAddedInGroup.Cast(Of IEntity).ToArray)
                            ProgressStep(info) ' HINT: cancellation check is also done here
                        Next

                        If Not _jtSettings.UseColors Then
                            For Each entity As BaseModelEntity In mergedOrAdded.OfType(Of BaseModelEntity)
                                With entity
                                    If TypeOf entity Is BundleEntity Then
                                        For Each prot As ProtectionEntity In CType(entity, BundleEntity).Protections
                                            ThrowCancelIfRequested() 'HINT: additional more granular cancellation-check
                                            'HINT: when we have overwritten segments from JT-Data, so the color alpha of it's protections is removed to avoid the "color-jittering"-effect because the bundle may go into it's to protection
                                            If prot.Override.IsGeometryEnabled Then
                                                prot.ActiveAlpha = 255
                                                prot.InactiveAlpha = 255
                                            End If
                                        Next
                                    End If
                                End With
                            Next
                        End If
                    End If
                End If
            End Sub

            Private Class ConsoliatedMeshInfo

                Public Sub New(meshes As IEnumerable(Of Mesh))
                    For Each mergeEntity As Mesh In meshes
                        Dim meshTriangles As New List(Of g3.Index3i)
                        Dim meshVertices As New List(Of g3.Vector3d)

                        With mergeEntity
                            For Each t As IndexTriangle In .Triangles
                                meshTriangles.Add(New g3.Index3i(t.V1, t.V2, t.V3))
                            Next

                            For Each v As Point3D In .Vertices
                                meshVertices.Add(New g3.Vector3d(Math.Round(v.X, 3), Math.Round(v.Y, 3), Math.Round(v.Z, 3)))
                            Next

                            If Mesh Is Nothing Then
                                Mesh = g3.DMesh3Builder.Build(Of g3.Vector3d, g3.Index3i, g3.Index3i)(meshVertices, meshTriangles)
                            Else
                                g3.MeshEditor.Append(Mesh, g3.DMesh3Builder.Build(Of g3.Vector3d, g3.Index3i, g3.Index3i)(meshVertices, meshTriangles))
                            End If
                        End With
                    Next

                    Me.Colors = meshes.Select(Function(m) m.Color).ToArray
                End Sub

                ReadOnly Property Mesh As g3.DMesh3 = Nothing
                ReadOnly Property Colors As Drawing.Color() = New Drawing.Color() {}

                ReadOnly Property HasData As Boolean
                    Get
                        Return Mesh IsNot Nothing
                    End Get
                End Property

            End Class

            Private Sub MergeTo(targetEntity As IBaseModelEntity, meshInfo As ConsoliatedMeshInfo, overwriteGeometry As Boolean, overwriteColors As Boolean)
                If overwriteGeometry OrElse overwriteColors Then
                    Dim meshTriangles As New List(Of IndexTriangle)
                    Dim meshVertices As New List(Of devDept.Geometry.Point3D)
                    Dim meshNormals As New List(Of devDept.Geometry.Vector3D)
                    Dim meshColor As Nullable(Of Drawing.Color) = Nothing
                    With meshInfo
                        If overwriteGeometry Then
                            For Each t As g3.Index3i In .Mesh.Triangles
                                meshTriangles.Add(New IndexTriangle(t.a, t.b, t.c))
                            Next

                            For Each v As g3.Vector3f In .Mesh.Vertices
                                meshVertices.Add(New devDept.Geometry.Point3D(v.x, v.y, v.z))
                            Next

                            If .Mesh.HasVertexNormals Then
                                For i As Integer = 0 To .Mesh.NormalsBuffer.Length - 1
                                    Dim normalVec As g3.Vector3f = .Mesh.GetVertexNormal(i)
                                    meshNormals.Add(New devDept.Geometry.Vector3D(normalVec.x, normalVec.y, normalVec.z))
                                Next
                            End If
                        End If
                        If overwriteColors Then
                            meshColor = If(meshInfo.Colors.Length > 0, meshInfo.Colors.First, Nothing)
                        End If
                    End With

                    If TypeOf targetEntity Is BaseModelEntity Then
                        MergeToEntity(CType(targetEntity, BaseModelEntity), If(meshTriangles.Count > 0, meshTriangles, Nothing), If(meshVertices.Count > 0, meshVertices, Nothing), If(meshNormals.Count > 0, meshNormals, Nothing), meshColor)
                    ElseIf TypeOf targetEntity Is IEnumerable Then
                        'HINT: added support for merging to multiple entities per group, because a FixingGroup f.e. can hold different entities with the same JT-linked-id (see gemini issue E3HA-1566)
                        For Each subEntity As IBaseModelEntity In CType(targetEntity, IEnumerable).OfType(Of IBaseModelEntity)
                            MergeTo(subEntity, meshInfo, overwriteGeometry, overwriteColors)
                        Next
                    End If
                End If
            End Sub

            Private Sub MergeToEntity(entity As BaseModelEntity, meshTriangles As IEnumerable(Of IndexTriangle), meshVertices As IEnumerable(Of devDept.Geometry.Point3D), meshNormals As IEnumerable(Of devDept.Geometry.Vector3D), Optional overrideColor As Nullable(Of Drawing.Color) = Nothing)
                With entity
                    .Override.Enabled = (meshTriangles?.Count).GetValueOrDefault > 0 OrElse (meshVertices?.Count).GetValueOrDefault > 0 OrElse (meshNormals?.Count).GetValueOrDefault > 0 OrElse overrideColor.HasValue
                    .Override.Triangles = Nothing
                    .Override.Vertices = Nothing
                    .Override.Normals = Nothing

                    If (meshTriangles?.Count).GetValueOrDefault > 0 OrElse (meshVertices?.Count).GetValueOrDefault > 0 OrElse (meshNormals?.Count).GetValueOrDefault > 0 Then
                        .Override.Vertices = meshVertices?.ToArray
                        .Override.Triangles = meshTriangles?.ToArray
                        .Override.Normals = meshNormals?.ToArray
                    End If

                    If overrideColor.HasValue Then
                        .Override.ActiveColor = overrideColor.Value
                    End If

                    If TypeOf entity Is GenericConnectorEntity Then
                        CType(entity, GenericConnectorEntity).Transformation = New Identity
                    End If

                    .Update()
                End With
            End Sub

            Public Overrides Function GetResult() As Object
                Return _convertResult
            End Function

            Private Class MergeData

                Private _model As EESystemModel
                Private _entitiesByJTLinkId As New Concurrent.ConcurrentDictionary(Of String, List(Of IBaseModelEntity))
                Private _modelByJtLinkId As New Concurrent.ConcurrentDictionary(Of String, List(Of ObjectBaseNaming))

                Public Sub New(targetEntities As IEnumerable(Of IBaseModelEntity), model As EESystemModel)
                    Dim entitiesByModelId As ILookup(Of Guid, IBaseModelEntity) = targetEntities.OfType(Of IBaseModelEntity).ToEEIdLookUp
                    _model = model

                    For Each obj As ObjectBaseNaming In model.OfType(Of ObjectBaseNaming)
                        Dim jtId As String = obj.CustomAttributes.OfType(Of KblPropertyBagAttribute).SingleOrDefault?.PropertyBag.JtLink

                        If Not String.IsNullOrEmpty(jtId) Then
                            _modelByJtLinkId.GetOrAdd(jtId, Function() New List(Of ObjectBaseNaming)).Add(obj)

                            If entitiesByModelId.Contains(obj.Id) Then
                                _entitiesByJTLinkId.GetOrAdd(jtId, Function() New List(Of IBaseModelEntity)).AddRange(entitiesByModelId(obj.Id))
                            End If
                        End If
                    Next
                End Sub

                Public Function IsModelContainerId(jtLinkId As String, containerId As Zuken.E3.Lib.Model.ContainerId) As Boolean
                    Dim objects As List(Of ObjectBaseNaming) = GetModelObjectsByJTLinkID(jtLinkId)
                    Return objects.Any(Function(obj) obj.HostContainerId = containerId)
                End Function

                Public Function GetModelObjectsByJTLinkID(jtLinkId As String) As List(Of ObjectBaseNaming)
                    Dim objLst As List(Of ObjectBaseNaming) = Nothing
                    If _modelByJtLinkId.TryGetValue(jtLinkId, objLst) Then
                        Return objLst
                    End If
                    Return New List(Of ObjectBaseNaming)
                End Function

                Public Function GetTargetEntitiesByJTLinkId(jtLinkId As String) As List(Of IBaseModelEntity)
                    Dim ents As List(Of IBaseModelEntity) = Nothing
                    If _entitiesByJTLinkId.TryGetValue(jtLinkId, ents) Then
                        Return ents
                    End If
                    Return New List(Of IBaseModelEntity)
                End Function

            End Class

            Protected Overrides Sub Dispose(disposing As Boolean)
                MyBase.Dispose(disposing)
                If disposing Then
                End If

                _document = Nothing
                _jtSettings = Nothing
                _convertResult = Nothing
            End Sub

        End Class

        Private Class ReadJTEntitiesWorkChunk
            Inherits WorkChunkAsyncStream
            Implements IDocumentWorkChunk

            Private WithEvents _dummyWorker As New BackgroundWorker ' to get the process-percentage from reader
            Private _jtData As IBaseDataFile
            Private _workCompleted As Boolean = False
            Private _error As Exception
            Private _disposeContainer As Boolean

            Public Sub New(jtData As IBaseDataFile)
                MyBase.New("Read JT entities", ProgressState.Reading, jtData.GetDataStream)
                _jtData = jtData
                _dummyWorker.WorkerReportsProgress = True
            End Sub

            Public Sub New(hcvFilePath As String, Optional cachingMethod As ZipCachingMethod = ZipCachingMethod.Memory)
                Me.New(HcvFile.Create(hcvFilePath, cachingMethod))
                _disposeContainer = True
            End Sub

            ReadOnly Property Entities As Entity() = Array.Empty(Of Entity)

            Public ReadOnly Property ChunkType As DocumentWorkChunkType Implements IDocumentWorkChunk.ChunkType
                Get
                    Return DocumentWorkChunkType.ReadJTEntities
                End Get
            End Property

            Protected Overrides Sub DoWorkCore(s As System.IO.Stream)
                _workCompleted = False
                _error = Nothing

                If KnownFile.IsType(KnownFile.Type.JT, _jtData.FullName) Then
                    If Environment.Is64BitProcess Then ' HINT: currently the ReadJT2 is only working in x64Bit
                        Try
                            Me.Maximum = 100
                            _dummyWorker.RunWorkerAsync(s)
                            Threading.SpinWait.SpinUntil(Function() _workCompleted OrElse _dummyWorker.CancellationPending)
                            If _error IsNot Nothing Then
                                Throw New Exception(_error.Message, _error)
                            End If
                        Catch ex As Exception
                            Throw New Exception($"Couldn't extract JT-entry ""{Path.GetFileName(_jtData.FullName)}"": {ex.Message}", ex)
                        End Try
                    Else
                        Debug.WriteLine("Loading JT-Data only possible in x64-process: skipped loading jt-data!")
                    End If
                End If
            End Sub

            Protected Overrides Sub OnBytesMoved(e As StreamProgressableReportEventArgs, progressInfo As ProgressInfo)
                'HINT: disable onbytes moved because the reader is using a copy of the stream (making a clone which will also raise on bytes-moved), we are skipping this progress here and using the progress from the reader instead
            End Sub

            Private Sub _dummyWorker_DoWork(sender As Object, e As DoWorkEventArgs) Handles _dummyWorker.DoWork
                Using reader As New devDept.Eyeshot.Translators.ReadJT2(CType(e.Argument, Stream))
                    reader.ReadTessellationOnly = True
                    reader.RunWork(_dummyWorker, e)
                    _Entities = reader.Entities.ToArray
                End Using
            End Sub

            Private Sub _dummyWorker_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) Handles _dummyWorker.ProgressChanged
                ProgressReport(e.ProgressPercentage, New UserStateProgressInfo(ProgressType, e.UserState))
            End Sub

            Private Sub _dummyWorker_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles _dummyWorker.RunWorkerCompleted
                _error = e.Error
                _workCompleted = True
            End Sub

            Public Overrides Function GetResult() As Object
                Return Entities
            End Function

            Protected Overrides Sub Dispose(disposing As Boolean)
                MyBase.Dispose(disposing)
                If disposing Then
                    _dummyWorker?.Dispose()
                    If _disposeContainer Then
                        _jtData?.Dispose()
                    End If
                End If
                _jtData = Nothing
                _error = Nothing
                _workCompleted = Nothing
                _dummyWorker = Nothing
            End Sub

        End Class

        Public Class UnboundEntity
            Inherits AlwaysOverrideBaseEntity

            Protected Sub New(another As Mesh) ' HINT: needed for clone
                MyBase.New(another)
            End Sub

            Public Sub New()
                MyBase.New(Guid.NewGuid.ToString)
            End Sub

            Public Overrides ReadOnly Property EntityType As ModelEntityType
                Get
                    Return ModelEntityType.Undefined
                End Get
            End Property

            Protected Overrides ReadOnly Property Priority As Integer
                Get
                    Return 100
                End Get
            End Property

            Public Overrides Property Selectable As Boolean
                Get
                    Return False
                End Get
                Set(value As Boolean)
                    MyBase.Selectable = value
                End Set
            End Property

            Public Overrides Function GetGrips() As List(Of devDept.Geometry.Point3D)
                Return New List(Of devDept.Geometry.Point3D)
            End Function

            Public Overrides Function GetGripHitIndex(hitPoint As devDept.Geometry.Point3D) As Integer
                Return -1
            End Function

            Public Overrides Function GetClosestPoint(fromPoint As devDept.Geometry.Point3D) As devDept.Geometry.Point3D
                Return devDept.Geometry.Point3D.Origin
            End Function

            Protected Overrides Function UpdateCore(data As BaseEEData, updateType As EntityUpdateType) As Boolean
                Return True
            End Function

        End Class

    End Class

End Namespace