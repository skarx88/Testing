Imports devDept.Eyeshot
Imports devDept.Eyeshot.Entities
Imports devDept.Geometry
Imports devDept.Geometry.Entities.GMesh
Imports Zuken.E3.HarnessAnalyzer.D3D.Consolidated.Controls
Imports Zuken.E3.Lib.Eyeshot.Model

Namespace D3D.Consolidated.Designs

    Public Class Group3D
        Inherits VirtualGroupEntity

        Public Event VisibilityChanged(sender As Object, e As EventArgs)

        Private _transformation As devDept.Geometry.Transformation = New devDept.Geometry.Identity()
        Private _layerName As String = String.Empty
        Private _transparencyPercent As UShort = 100
        Private _materials As New Dictionary(Of String, devDept.Graphics.Material)
        Private _initialEntitiesAlpha As New Dictionary(Of IEntity, Integer)

        Public Sub New()
            Me.New(Guid.NewGuid.ToString)
        End Sub

        Public Sub New(id As String)
            MyBase.New(id)
        End Sub

        ReadOnly Property Id As String
            Get
                Return MyBase.Name
            End Get
        End Property

        Property IsXhcv As Boolean
        Property FilePath As String

        Protected Overrides Function GetEntitiesForSelection(selectionBox As Rectangle) As IEnumerable(Of Entity)
            If Me.Workspace IsNot Nothing Then
                Dim list As List(Of Entity) = MyBase.GetEntitiesForSelection(selectionBox).ToList
                Dim indicies As Integer() = Workspace.GetCrossingEntities(selectionBox, list, True)
                Dim result As New List(Of Entity)
                For Each idx As Integer In indicies
                    result.Add(list(idx))
                Next
                Return result
            End If
            Return New List(Of Entity)
        End Function

        Protected Overrides Function AddCore(entity As IEntity) As Boolean
            If _layerName <> String.Empty Then
                If _layerName <> entity.LayerName Then
                    Throw New NotSupportedException(String.Format("Adding entity from a different layer to ""{0}"" is not supported!", entity.GetType.Name))
                End If
            Else
                _layerName = entity.LayerName
            End If

            If Not _initialEntitiesAlpha.ContainsKey(entity) Then
                _initialEntitiesAlpha.Add(entity, entity.Color.A)
            End If

            Return MyBase.AddCore(entity)
        End Function

        Protected Overrides Function RemoveCore(entity As IEntity, removeBehaviour As RemoveBehaviour) As Boolean
            _initialEntitiesAlpha.Remove(entity)
            Return MyBase.RemoveCore(entity, removeBehaviour)
        End Function

        Public Property Transformation As devDept.Geometry.Transformation
            Get
                Return _transformation
            End Get
            Set(value As devDept.Geometry.Transformation)
                _transformation = value
                For Each en As Entity In Me
                    en.TransformBy(_transformation)
                Next
                OnPropertyChanged()
            End Set
        End Property

        Property TransparencyPercent As UShort
            Get
                Return _transparencyPercent
            End Get
            Set(value As UShort)
                If _transparencyPercent <> value Then
                    If value < 0 Then
                        _transparencyPercent = 0
                    ElseIf (value > 100) Then
                        _transparencyPercent = 100
                    Else
                        _transparencyPercent = value
                    End If
                    _transparencyPercent = value

                    For Each ent As Entity In Me.GetEntities
                        Dim initialAlpha As Integer = 255
                        If _initialEntitiesAlpha.ContainsKey(ent) Then
                            initialAlpha = _initialEntitiesAlpha(ent)
                        End If
                        ent.Color = Color.FromArgb(CInt(initialAlpha * (_transparencyPercent / 100)), ent.Color.R, ent.Color.G, ent.Color.B)
                    Next
                    OnPropertyChanged(NameOf(TransparencyPercent))
                End If
            End Set
        End Property

        ReadOnly Property Materials As IReadOnlyDictionary(Of String, devDept.Graphics.Material)
            Get
                Return _materials
            End Get
        End Property

        Property LayerName As String
            Get
                Return _layerName
            End Get
            Set(value As String)
                If _layerName <> value Then
                    For Each entity As Entity In Me.GetEntities.OfType(Of Entity)
                        entity.LayerName = value
                    Next
                    OnPropertyChanged(NameOf(LayerName))
                End If
            End Set
        End Property

        Public Overrides Property Color As Color
            Get
                Return MyBase.Color
            End Get
            Set(value As Color)
                If MyBase.Color <> value Then
                    MyBase.Color = value
                End If
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0}, Count: {1}", Me.Name, Me.Count)
        End Function

        Public Sub TransformBy(Optional transformation As devDept.Geometry.Transformation = Nothing)
            If transformation Is Nothing Then
                transformation = _transformation
            End If

            For Each en As Entity In Me
                en.TransformBy(transformation)
            Next
        End Sub

        Public Sub Scale(factor As Double)
            Dim box As New BoundingBox()
            box.Max = BoxMax
            box.Min = BoxMin
            Dim scaleTransform As New devDept.Geometry.Transformation
            scaleTransform.Scaling(box.GetCenter, factor)
            _transformation = scaleTransform * Transformation
            TransformBy(scaleTransform)
        End Sub

        Public Sub Translate(myTranslation As Vector3D)
            Dim trans As New devDept.Geometry.Transformation
            trans.Translation(myTranslation)
            _transformation = trans * Transformation
            TransformBy(trans)
        End Sub

        Public Sub Translate(x As Double, y As Double, z As Double)
            Dim trans As New devDept.Geometry.Transformation
            trans.Translation(x, y, z)
            _transformation = trans * Transformation
            TransformBy(trans)
        End Sub

        Public Sub Rotate(angle As Double, axis As Vector3D)
            Dim trans As New devDept.Geometry.Transformation
            trans.Rotation(angle, axis, Me.GetBoundingBox.GetCenter)
            _transformation = trans * Me.Transformation
            TransformBy(trans)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="fullPath"></param>
        ''' <param name="model"></param>
        ''' <param name="transparencyPercent"></param>
        ''' <param name="progressChanged"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function LoadFromFile(fullPath As String, model As devDept.Eyeshot.Design, transparencyPercent As UShort, Optional progressChanged As Action(Of System.ComponentModel.ProgressChangedEventArgs) = Nothing) As Group3D
            Dim data As ModelLoaderInit = CreateAndInitCarModelLoader(model, fullPath, progressChanged)
            data.Loader.Load(fullPath)
            data.RemoveHandler()

            Return CreateGroup(fullPath, model, data.Loader, transparencyPercent)
        End Function

        Public Shared Async Function LoadFromFileAsync(fullPath As String, model As devDept.Eyeshot.Design, transparencyPercent As UShort, Optional progressChanged As Action(Of System.ComponentModel.ProgressChangedEventArgs) = Nothing, Optional cancel As D3DCancellationTokenSource = Nothing) As Task(Of Group3D)
            Dim data As ModelLoaderInit = CreateAndInitCarModelLoader(model, fullPath, progressChanged)

            Await data.Loader.LoadAsync(fullPath, cancel)
            data.RemoveHandler()

            Return CreateGroup(fullPath, model, data.Loader, transparencyPercent)
        End Function

        Private Shared Function CreateAndInitCarModelLoader(model As devDept.Eyeshot.Design, fullPath As String, Optional progressChanged As Action(Of System.ComponentModel.ProgressChangedEventArgs) = Nothing) As ModelLoaderInit

            Dim carModelLoader As BaseFileLoader = BaseFileLoader.Create(fullPath, model)
            Dim prgChanged As ObjectFileLoader.ProgressChangedEventHandler = Sub(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) progressChanged.Invoke(e)

            If (progressChanged IsNot Nothing) Then
                AddHandler carModelLoader.ProgressChanged, prgChanged
            End If

            Return New ModelLoaderInit(carModelLoader, prgChanged)
        End Function

        Public Function TryRemoveMaterials(model As devDept.Eyeshot.Design) As Boolean
            Dim removed As Boolean = True
            For Each kv As KeyValuePair(Of String, devDept.Graphics.Material) In _materials
                If kv.Key <> "Default" Then
                    If Not model.Materials.Remove(kv.Key) Then
                        removed = False
                    End If
                End If
            Next
            Return removed
        End Function

        Private Shared Function CreateGroup(fullPath As String, model As devDept.Eyeshot.Design, modelLoader As BaseFileLoader, transparencyPercent As UShort) As D3D.Consolidated.Designs.Group3D
            Dim layerName As String = String.Empty

            If transparencyPercent < 0 Then
                transparencyPercent = 0
            ElseIf transparencyPercent > 100 Then
                transparencyPercent = 100
            End If

            If (modelLoader.Layer IsNot Nothing) Then
                layerName = modelLoader.Layer.Name
            End If

            Dim group As New Group3D(fullPath)
            group.FilePath = fullPath

            For Each mat As devDept.Graphics.Material In modelLoader.Materials
                group._materials.Add(mat.Name, mat)
            Next

            Dim processEntity As Action(Of Entity) =
                Sub(entity As Entity)
                    If (TypeOf modelLoader Is ObjectFileLoader AndAlso Not String.IsNullOrEmpty(entity.MaterialName) AndAlso modelLoader.Materials.Contains(entity.MaterialName)) Then
                        entity.ColorMethod = colorMethodType.byEntity

                        Dim mat As devDept.Graphics.Material = modelLoader.Materials(entity.MaterialName)
                        With mat
                            entity.Color = Color.FromArgb(entity.Color.A, .Diffuse.R, .Diffuse.G, .Diffuse.B)
                        End With
                    End If

                    If (TypeOf modelLoader Is ObjectFileLoader) Then entity.MaterialName = Nothing

                    entity.LayerName = layerName

                    If (TypeOf modelLoader Is ObjectFileLoader) Then
                        'Dim entData As String = If(part.EntityData IsNot Nothing, part.EntityData.ToString, "")
                        'part.EntityData = New EntityData(entData & "|" & Guid.NewGuid.ToString, part)
                        entity.RegenMode = regenType.RegenAndCompile
                    End If

                    entity.Selectable = False

                    If (TypeOf modelLoader Is ObjectFileLoader) Then
                        Dim myXList As New List(Of KeyValuePair(Of Short, Object))
                        myXList.Add(New KeyValuePair(Of Short, Object)(0, IO.Path.GetFileName(fullPath)))
                        entity.AutodeskProperties = New AutodeskMiscProperties
                        entity.AutodeskProperties.XData = myXList
                    End If

                    group.Add(entity)
                End Sub

            If (modelLoader.Entities IsNot Nothing) Then
                For Each entity As Entity In modelLoader.Entities
                    If entity IsNot Nothing AndAlso processEntity IsNot Nothing Then
                        processEntity.Invoke(entity)
                    End If
                Next
            End If

            group.TransparencyPercent = transparencyPercent

            Return group
        End Function


        Public Function GetBox(Optional color As System.Drawing.Color = Nothing, Optional transparency As Integer = 0) As Mesh
            Dim bbox As New BoundingBox()
            bbox.Max = BoxMax
            bbox.Min = BoxMin

            Dim box As Mesh = Mesh.CreateBox(bbox.Max.X - bbox.Min.X, bbox.Max.Y - bbox.Min.Y, bbox.Max.Z - bbox.Min.Z, natureType.Smooth)
            box.ColorMethod = colorMethodType.byEntity
            box.Color = color
            box.Color = System.Drawing.Color.FromArgb(transparency, box.Color)
            box.MoveTo(box.BoxMin, bbox.Min)
            'box.EntityData = New EntityData(Guid.NewGuid.ToString, box)
            Return box
        End Function

    End Class


    Friend Class ModelLoaderInit

        Protected _handler As BaseFileLoader.ProgressChangedEventHandler
        Protected _loader As BaseFileLoader

        Public Sub New(loader As BaseFileLoader, Optional progressChangedEventHandler As BaseFileLoader.ProgressChangedEventHandler = Nothing)
            _loader = loader
            _handler = progressChangedEventHandler
        End Sub

        ReadOnly Property Loader As BaseFileLoader
            Get
                Return _loader
            End Get
        End Property

        Public Sub [RemoveHandler]()
            If _handler IsNot Nothing Then RemoveHandler _loader.ProgressChanged, _handler
        End Sub

    End Class

End Namespace