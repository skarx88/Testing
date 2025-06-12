Imports System.ComponentModel
Imports System.IO
Imports Zuken.E3.HarnessAnalyzer.Shared
Imports Zuken.E3.Lib.Eyeshot.Model
Imports Zuken.E3.Lib.Eyeshot.Model.Converter
Imports Zuken.E3.Lib.IO
Imports Zuken.E3.Lib.IO.Files
Imports Zuken.E3.Lib.IO.Files.Hcv
Imports Zuken.E3.Lib.Model
Imports Zuken.E3.Lib.Schema.Kbl

Namespace Documents

    Partial Public Class HcvDocument

        Public Shadows Function Open(Optional loadContent As Nullable(Of LoadContentType) = Nothing, Optional settings As ContentSettings = Nothing, Optional openChilds As OpenChildFilesFlag = OpenChildFilesFlag.None) As IResult 'Expose open method to public
            Return MyBase.Open(New DocumentOpenInfo(loadContent, settings, openChilds))
        End Function

        Protected Overrides Async Function OpenCore(state As E3.Lib.IO.Files.WorkState, userData As Object) As Task(Of IResult)
            Dim info As DocumentOpenInfo = TryCast(userData, DocumentOpenInfo)
            If _hcvFile IsNot Nothing Then
                If _hcvFile.KblDocument IsNot Nothing AndAlso Not _hcvFile.KblDocument.IsOpen Then
                    _hcvFile.KblDocument.Open()
                    _kbl = KblMapper.Create(_hcvFile.KblDocument.GetKblContainer, _hcvFile.KblDocument.GetVersion)
                End If

                Using prep_work As PreparedWork = PrepareContentWork(info?.LoadContent, info?.ContentSettings)
                    Dim res As IResult = Await prep_work.LoadAsync
                    If res.IsSuccess Then
                        _KblZDataAvailable = AnalyseContent(ContentAnalyseType.HasKBLZData)
                    End If
                    Return res
                End Using
            End If
            Return Result.Faulted("Error opening document: No hcv file set!")
        End Function

        Private Function GetAllContentValues() As LoadContentType
            Dim content As LoadContentType = LoadContentType.None
            For Each value As Integer In [Enum].GetValues(GetType(LoadContentType))
                content = content Or CType(value, LoadContentType)
            Next
            Return content
        End Function

        Private Sub InitializeVariants(model As EESystemModel)
            model.ActiveVariant = model.Variants.AddMasterVariant
            If Not String.IsNullOrWhiteSpace(ISACTIVE_VARIANT_NAME) Then
                model.Variants.Add(New [Variant] With {.ShortName = ISACTIVE_VARIANT_NAME})
            End If
            _VariantUsedToActivate = GetVariantCheckedIsActive(model)
        End Sub

        Private Function GetVariantCheckedIsActive(model As EESystemModel) As [Variant]
            If model IsNot Nothing Then
                If Not String.IsNullOrWhiteSpace(ISACTIVE_VARIANT_NAME) Then
                    Return model.Variants.OfType(Of [Variant]).Where(Function(v) v.ShortName = ISACTIVE_VARIANT_NAME).SingleOrDefault
                End If
                Return model.Variants.MasterVariant
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' Perpares given load content workers to be initialized. 
        ''' </summary>
        ''' <param name="contentValue"></param>
        ''' <param name="settings">Leave empty for default settings</param>
        ''' <param name="showProgress"></param>
        ''' <returns>Returns a prepared load content result which can be used to execute loading the prepared document loadable content</returns>
        Public Function PrepareContentWork(Optional contentValue As Nullable(Of LoadContentType) = Nothing, Optional settings As ContentSettings = Nothing, Optional showProgress As Boolean = True) As PreparedWork
            Dim content As LoadContentType = contentValue.GetValueOrDefault(GetAllContentValues)
            If settings Is Nothing Then
                settings = New ContentSettings ' default settings
            End If

            Dim hasJtData As Boolean = AnalyseContent(ContentAnalyseType.HasJTData)

            If content.HasFlag(LoadContentType.JTData) AndAlso Not hasJtData Then
                content = content Xor LoadContentType.JTData
            End If

            Dim loadJtDataOnly As Boolean = content.HasFlag(LoadContentType.JTData) AndAlso Not content.HasFlag(LoadContentType.Entities)

            Dim myChunks As New List(Of IWorkChunk)

            If content.HasFlag(LoadContentType.Model) OrElse (content.HasFlag(LoadContentType.Entities) AndAlso Not LoadedContent.HasFlag(LoadContentType.Model)) Then
                _Model = New EESystemModel With {
                    .CurrentLengthClass = settings.LengthClass
                }
                _Model.CustomAttributes.Add(New BooleanAttribute(USE_KBL_ABSOLUTE_LOCATIONS, settings.UseKblAbsoluteLocations))

                InitializeVariants(_Model) ' HINT: must come before ReadModelWorkChunk is executed because this workchunk will set all objects-variants to active after finished

                myChunks.Add(GetWrappedOrDefault(showProgress, New ReadModelWorkChunk(_hcvFile.KblDocument, _Model)))
            End If

            If loadJtDataOnly OrElse (content.HasFlag(LoadContentType.Entities)) Then
                If _converted Is Nothing OrElse content.HasFlag(LoadContentType.Entities) Then
                    myChunks.Add(GetWrappedOrDefault(showProgress, New ConvertModelWorkChunk(Me)))
                End If
            End If

            If content.HasFlag(LoadContentType.JTData) AndAlso hasJtData Then
                myChunks.Add(GetWrappedOrDefault(showProgress, New ReadJTEntitiesWorkChunk(_hcvFile.JT)))  'TODO: maybe implement a lazy loading for JT entities to be loaded
                myChunks.Add(GetWrappedOrDefault(showProgress, New MergeJTEntitiesWorkChunk(Me, _converted, settings.JT))) ' if converted is empty the data will be retrieved over the collection from WorkChunk internally
            End If

            If loadJtDataOnly OrElse (content.HasFlag(LoadContentType.Entities)) Then
                myChunks.Add(GetWrappedOrDefault(showProgress, New UpdateDocumentEntitiesWorkChunk(Me, _converted, _converted Is Nothing))) ' if converted is empty the data will be retrieved over the collection from WorkChunk internally
            End If

            Return New PreparedWork(Me, content, myChunks.ToArray)
        End Function

        Private Function GetWrappedOrDefault(showProgress As Boolean, workChunk As IWorkChunk) As IWorkChunk
            If showProgress Then
                Return New DocumentWorkUnitWrappedWorkChunk(workChunk)
            Else
                Return workChunk
            End If
        End Function

        Public Function AnalyseContent(type As ContentAnalyseType) As Boolean
            Select Case type
                Case ContentAnalyseType.HasEntities
                    Return Me.LoadedContent.HasFlag(LoadContentType.Entities) AndAlso Me.Entities.Count > 0
                Case ContentAnalyseType.HasJTData, ContentAnalyseType.HasModelData, ContentAnalyseType.HasKBLZData
                    Return AnalyseContent(_hcvFile, type)
                Case ContentAnalyseType.None
                    Return _hcvFile Is Nothing
            End Select
            Return False
        End Function

        Public Shared Function AnalyseContent(hcvFile As HcvFile, type As ContentAnalyseType) As Boolean
            Select Case type
                Case ContentAnalyseType.HasEntities
                    Throw New ArgumentException($"Argument-type ""{type.ToString}"" not supported in this method!")
                Case ContentAnalyseType.HasJTData
                    Return (hcvFile?.JT?.HasData).GetValueOrDefault
                Case ContentAnalyseType.HasModelData
                    Return (hcvFile?.KblDocument?.HasData).GetValueOrDefault
                Case ContentAnalyseType.None
                    Return hcvFile Is Nothing
                Case ContentAnalyseType.HasKBLZData
                    Return HasKblZData(hcvFile)
            End Select
            Return False
        End Function

        Public Shared Function HasKblZData(data As HcvFile) As Boolean
            If (data?.KblDocument?.HasData).GetValueOrDefault Then
                Dim cartesianPoints As New List(Of Double())
                Using s As Stream = data.KblDocument.GetDataAsStream
                    Dim kblDoc As XDocument = XDocument.Load(s)
                    For Each cPt As XElement In kblDoc.Root.Elements.Where(Function(el) el.Name.LocalName = NameOf(Cartesian_point))
                        Dim valueList As New List(Of Double)
                        If cPt.HasElements Then
                            For Each coord As XElement In cPt.Elements
                                Dim value As Double
                                If Double.TryParse(coord.Value, Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, value) Then
                                    valueList.Add(value)
                                End If
                            Next
                        End If
                        cartesianPoints.Add(valueList.ToArray)
                    Next
                End Using

                Return cartesianPoints.Where(Function(pt) pt.Length > 2).Any(Function(pts) pts(2) <> 0)
            End If
            Return False
        End Function

        Public Enum ContentAnalyseType
            None
            HasEntities
            HasJTData
            HasModelData
            HasKBLZData
        End Enum

        ReadOnly Property KblZDataAvailable As Boolean

        Private Class DocumentOpenInfo
            Inherits OpenInfo
            Public Sub New(loadContent As Nullable(Of LoadContentType), settings As ContentSettings, openFiles As OpenChildFilesFlag)
                MyBase.New(openFiles = OpenChildFilesFlag.OpenFiles Or openFiles = OpenChildFilesFlag.OpenFilesRecursive, openFiles = OpenChildFilesFlag.OpenFilesRecursive)
                Me.LoadContent = loadContent
                Me.ContentSettings = settings
            End Sub

            Property LoadContent As Nullable(Of LoadContentType)
            Property ContentSettings As ContentSettings
        End Class

#Region "WorkChunks"

        Private Class UpdateDocumentEntitiesWorkChunk
            Inherits WorkChunk
            Implements IDocumentWorkChunk

            Private _document As HcvDocument
            Private _convertResult As ConvertModelResult
            Private _clearEntities As Boolean
            Private _isSuccess As Boolean = False

            Public Sub New(document As HcvDocument, convertResult As ConvertModelResult, clearEntities As Boolean)
                MyBase.New("Updating entities...", ProgressState.Updating)
                _document = document
                _convertResult = convertResult
                _clearEntities = clearEntities
            End Sub

            Public Sub New(document As HcvDocument, clearEntities As Boolean)
                Me.New(document, Nothing, clearEntities)
            End Sub

            Public ReadOnly Property ChunkType As DocumentWorkChunkType Implements IDocumentWorkChunk.ChunkType
                Get
                    Return DocumentWorkChunkType.UpdateDocumentEntities
                End Get
            End Property

            Public Overrides Function GetResult() As Object
                If _isSuccess Then
                    Return Result.Success
                Else
                    Return Result.Faulted("Unknown")
                End If
            End Function

            Protected Overrides Sub DoWorkCore()
                _isSuccess = False

                If _convertResult Is Nothing Then
                    _convertResult = CType(Me.Owner.OfType(Of IDocumentWorkChunk).Where(Function(chunk) chunk.ChunkType = DocumentWorkChunkType.ConvertModel).Single.GetResult, ConvertModelResult)
                End If

                Dim added As New List(Of IBaseModelEntity)
                Me.Maximum = _convertResult.Count
                _document.Entities.BeginUpdate(_clearEntities)
                Try
                    For Each entity As IBaseModelEntityEx In _convertResult.OfType(Of IBaseModelEntityEx)
                        If _document.Entities.TryAdd(entity) Then
                            added.Add(entity)
                            _isSuccess = True
                        End If
                        ProgressStep() 'HINT: cancellation-check is included here
                    Next
                Finally
                    _document.Entities.EndUpdate(New EntitiesUpdateInfo(added))
                End Try
            End Sub

            Protected Overrides ReadOnly Property File As IBaseFile
                Get
                    Return _document
                End Get
            End Property

        End Class

        Private Class ReadModelWorkChunk
            Inherits WorkChunkAsyncStream
            Implements IDocumentWorkChunk

            Private _model As EESystemModel
            Private _result As ModelResult
            Private _fileContainer As KblContainerFile

            Public Sub New(filePath As String, model As EESystemModel)
                Me.New(KblContainerFile.Create(filePath), model)
            End Sub

            Public Sub New(fileContainer As KblContainerFile, model As EESystemModel)
                MyBase.New("Importing to model...", ProgressState.Reading, fileContainer.GetDataAsStream)
                _model = model
                _fileContainer = fileContainer
            End Sub

            Public ReadOnly Property ChunkType As DocumentWorkChunkType Implements IDocumentWorkChunk.ChunkType
                Get
                    Return DocumentWorkChunkType.ReadModel
                End Get
            End Property

            Protected Overrides Sub DoWorkCore(s As IO.Stream)

                _result = _fileContainer.CopyToModel(_model)
                If _result.IsSuccess Then
                    ActivateAllObjectsInDocumentIsActiveVariant(_model)
                ElseIf Not _result.IsCancelled Then
                    Dim faultedContainers As KblContainerResult() = _result.OfType(Of KblContainerResult).Where(Function(c) c.IsFaulted).ToArray
                    If faultedContainers.Length > 0 Then
                        Dim fileUri As Uri = Nothing
#If NET5_0_OR_GREATER Then
                        If Uri.TryCreate(IO.Path.GetFileName(faultedContainers.First.Name), New UriCreationOptions, fileUri) Then
#Else
                        If Uri.TryCreate(IO.Path.GetFileName(faultedContainers.First.Name), UriKind.Absolute, fileUri) Then
#End If
                            Throw New FileFormatException(fileUri, _result.Message)
                        End If
                    End If
                    Throw New Exception(_result.Message)
                End If
            End Sub

            Private Sub ActivateAllObjectsInDocumentIsActiveVariant(model As EESystemModel)
                Dim activateInDocVariant As [Variant] = TryCast(TryCast(Me.Owner, IOwner)?.Owner, HcvDocument)?.VariantUsedToActivate
                If activateInDocVariant IsNot Nothing Then
                    For Each obj As ObjectBase In model
                        obj.TryActivateInVariantNoInherit(activateInDocVariant, True)
                    Next
                End If
            End Sub

            Public Overrides Function GetResult() As Object
                Return _result
            End Function

            Protected Overrides ReadOnly Property File As IBaseFile
                Get
                    Return _fileContainer
                End Get
            End Property

        End Class

        Private Class ConvertModelWorkChunk
            Inherits WorkChunkAsync
            Implements IDocumentWorkChunk

            Private _document As HcvDocument

            Public Sub New(document As HcvDocument)
                MyBase.New("Converting model...", ProgressState.Reading)
                _document = document
                Me.Maximum = 100
            End Sub

            ReadOnly Property Result As ConvertModelResult

            Private ReadOnly Property ChunkType As DocumentWorkChunkType Implements IDocumentWorkChunk.ChunkType
                Get
                    Return DocumentWorkChunkType.ConvertModel
                End Get
            End Property

            Protected Overrides Sub DoWorkCore()
                If _document?.Model IsNot Nothing Then
                    Dim prgInited As Boolean = False
                    Dim lastPerc As Integer = 0
                    Dim settings As New Convert3DSettings
                    settings.Progress =
                        Sub(e)
                            'HINT: increment the overall process by the percentage- difference (because this is a step here -> we are stepping percentwise due to limited information: no maximum/current count available, etc.)
                            If e.ProgressPercentage <> lastPerc Then
                                Dim diff As Integer = e.ProgressPercentage - lastPerc
                                ProgressStep(diff) 'HINT: cancellation-check is also included here
                                lastPerc = e.ProgressPercentage
                            End If
                        End Sub
                    Using filtermodel As New FilteringModel(_document.Model)
                        _Result = filtermodel.ConvertTo3D(HcvDocument.USE_CONTAINER_IDS, settings)
                    End Using
                End If
            End Sub

            Public Overrides Function GetResult() As Object
                Return Result
            End Function

            Private Class FilteringModel ' HINT: this class is a layer between the eeModel-Object and the 3D-converter to make filtering model-objects, which should be included in conversion, possible
                Implements IEEModel
                Implements IDisposable

                Private _containerObjectsFiltered As List(Of IObjectsBase)
                Private _disposedValue As Boolean

                Public Sub New(model As EESystemModel)
                    _containerObjectsFiltered = CreateContainerObjectsFiltered(model)
                End Sub

                Private Function CreateContainerObjectsFiltered(model As EESystemModel) As List(Of IObjectsBase)
                    Dim containers As New List(Of IObjectsBase)

                    For Each container As ObjectsBase In model.Containers
                        Dim containerObjectsList As New ObjectsElementsWrapper(container.Model, container.ContainerId)

                        For Each obj As ObjectBase In container
                            If Not IsFiltered(obj) Then
                                containerObjectsList.Add(obj)
                            End If
                        Next

                        containers.Add(containerObjectsList)
                    Next

                    Return containers
                End Function

                Private Function IsFiltered(obj As ObjectBase) As Boolean
                    'HINT: here are all model-specific object filters implemented:

                    If TypeOf obj Is Zuken.E3.Lib.Model.Protection Then
                        Return IsFilteredNodeProtection(CType(obj, Protection))
                    End If

                    Return False
                End Function

                Private Function IsFilteredNodeProtection(prot As Zuken.E3.Lib.Model.Protection) As Boolean
                    'HINT: remove all Protections from model that are only connected to a node (see Issue E3HA-1593). 3D entities structure does not support drawing protections over nodes.
                    If prot.GetSegments.Count = 0 Then
                        'TODO: check if protection has mapped nodes -> this is currently not possible because the model does not support/has not implemented this mapper
                        Return True
                    End If
                    Return False
                End Function

                Public ReadOnly Property Containers As IEnumerable(Of IObjectsBase) Implements IEEModel.Containers
                    Get
                        Return _containerObjectsFiltered
                    End Get
                End Property

                Default Public ReadOnly Property Item(Id As Guid) As IObjectBase Implements IEEModel.Item
                    Get
                        Return GetObjectById(Id)
                    End Get
                End Property

                Private Function GetObjectById(id As Guid) As IObjectBase
                    For Each c As IObjectsBase In Me.Containers
                        If c.Contains(id) Then
                            Return c.Item(id)
                        End If
                    Next
                    Return Nothing
                End Function

                Public ReadOnly Property IsWrapper As Boolean Implements IEEModel.IsWrapper
                    Get
                        Return True
                    End Get
                End Property

                Public Function Contains(id As Guid) As Boolean Implements IEEModel.Contains
                    Return GetObjectById(id) IsNot Nothing
                End Function

                Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
                    Return _containerObjectsFiltered.GetEnumerator
                End Function

                Protected Overridable Sub Dispose(disposing As Boolean)
                    If Not _disposedValue Then
                        If disposing Then
                            If _containerObjectsFiltered IsNot Nothing Then
                                For Each c As IObjectsBase In _containerObjectsFiltered
                                    c.Dispose()
                                Next
                            End If
                        End If

                        _containerObjectsFiltered = Nothing
                        _disposedValue = True
                    End If
                End Sub

                Public Sub Dispose() Implements IDisposable.Dispose
                    ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
                    Dispose(disposing:=True)
                    GC.SuppressFinalize(Me)
                End Sub
            End Class

            Protected Overrides ReadOnly Property File As IBaseFile
                Get
                    Return _document
                End Get
            End Property

            Private Class ObjectsElementsWrapper
                Implements IObjectsBase

                Private _list As New List(Of IObjectBase)
                Private _dic As New Dictionary(Of Guid, IObjectBase)

                Public Sub New(model As IEEModel, containerId As ContainerId)
                    Me.Model = model
                    Me.ContainerId = containerId
                End Sub

                Public ReadOnly Property Model As IEEModel Implements IObjectsBase.Model
                Public ReadOnly Property ContainerId As ContainerId Implements IObjectsBase.ContainerId

                Public ReadOnly Property IsFixedSize As Boolean Implements IList.IsFixedSize
                    Get
                        Return False
                    End Get
                End Property

                Public ReadOnly Property IsReadOnly As Boolean Implements IList.IsReadOnly
                    Get
                        Return False
                    End Get
                End Property

                Default Public Property Item(index As Integer) As Object Implements IList.Item
                    Get
                        Return _list(index)
                    End Get
                    Set(value As Object)
                        If value Is Nothing OrElse TypeOf value Is IObjectBase Then
                            Dim current As IObjectBase = _list(index)
                            _dic.Remove(current.Id)
                            _list(index) = CType(value, IObjectBase)
                            If value IsNot Nothing Then
                                _dic.Add(CType(value, IObjectBase).Id, CType(value, IObjectBase))
                            End If
                        Else
                            Throw New ArgumentException($"Invalid type {value.GetType.FullName}. Expected: {GetType(IObjectBase).FullName}")
                        End If
                    End Set
                End Property

                Public ReadOnly Property Item2(id As Guid) As IObjectBase Implements IObjectsBase.Item
                    Get
                        Return _dic(id)
                    End Get
                End Property

                Public ReadOnly Property Count As Integer Implements ICollection.Count
                    Get
                        Return _list.Count
                    End Get
                End Property

                Public ReadOnly Property IsSynchronized As Boolean Implements ICollection.IsSynchronized
                    Get
                        Return CType(_list, IList).IsSynchronized
                    End Get
                End Property

                Public ReadOnly Property SyncRoot As Object Implements ICollection.SyncRoot
                    Get
                        Return CType(_list, IList).SyncRoot
                    End Get
                End Property

                Private Function IObjectsBase_Contains(id As Guid) As Boolean Implements IObjectsBase.Contains
                    Return _dic.ContainsKey(id)
                End Function

                Public Sub Clear() Implements IList.Clear
                    _list.Clear()
                    _dic.Clear()
                End Sub

                Public Sub Insert(index As Integer, value As Object) Implements IList.Insert
                    If TypeOf value Is IObjectBase Then
                        CType(_list, IList).Insert(index, value)
                        _dic.Add(CType(value, IObjectBase).Id, CType(value, IObjectBase))
                    Else
                        Throw New ArgumentException($"Invalid type {value?.GetType.FullName}. Expected: {GetType(IObjectBase).FullName}")
                    End If
                End Sub

                Public Sub Remove(value As Object) Implements IList.Remove
                    If value Is Nothing OrElse TypeOf value Is IObjectBase Then
                        If _list.Remove(CType(value, IObjectBase)) Then
                            _dic.Remove(CType(value, IObjectBase).Id)
                        End If
                    Else
                        Throw New ArgumentException($"Invalid type {value?.GetType.FullName}. Expected: {GetType(IObjectBase).FullName}")
                    End If
                End Sub

                Public Sub RemoveAt(index As Integer) Implements IList.RemoveAt
                    Dim item As IObjectBase = CType(Me.Item(index), IObjectBase)
                    _list.RemoveAt(index)
                    _dic.Remove(item.Id)
                End Sub

                Public Sub CopyTo(array As Array, index As Integer) Implements ICollection.CopyTo
                    CType(_list, IList).CopyTo(array, index)
                End Sub

                Public Sub Dispose() Implements IDisposable.Dispose
                    _list?.Clear()
                    _dic?.Clear()
                    _dic = Nothing
                    _list = Nothing
                    _Model = Nothing
                    _ContainerId = Nothing
                End Sub

                Public Function Add(value As Object) As Integer Implements IList.Add
                    If value Is Nothing OrElse TypeOf value Is IObjectBase Then
                        Dim index As Integer = CType(_list, IList).Add(CType(value, IObjectBase))
                        If index <> -1 Then
                            _dic.Add(CType(value, IObjectBase).Id, CType(value, IObjectBase))
                            Return index
                        End If
                    Else
                        Throw New ArgumentException($"Invalid type {value?.GetType.FullName}. Expected: {GetType(IObjectBase).FullName}")
                    End If
                    Return -1
                End Function

                Public Function Contains(value As Object) As Boolean Implements IList.Contains
                    Return CType(_list, IList).Contains(value)
                End Function

                Public Function IndexOf(value As Object) As Integer Implements IList.IndexOf
                    Return CType(_list, IList).IndexOf(value)
                End Function

                Public Function GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
                    Return CType(_list, IList).GetEnumerator
                End Function

                Public Function Clone() As Object Implements ICloneable.Clone
                    Dim myclone As New ObjectsElementsWrapper(Me.Model, Me.ContainerId)
                    myclone._list.AddRange(_list)
                    myclone._dic = New Dictionary(Of Guid, IObjectBase)(_dic)
                    Return myclone
                End Function
            End Class

        End Class

#End Region

    End Class

End Namespace