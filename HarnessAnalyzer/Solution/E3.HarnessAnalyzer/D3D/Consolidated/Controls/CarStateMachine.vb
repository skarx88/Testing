Imports System.ComponentModel
Imports System.IO
Imports System.Runtime.Serialization
Imports devDept.Eyeshot
Imports devDept.Eyeshot.Entities
Imports devDept.Geometry
Imports Zuken.E3.HarnessAnalyzer.Compatibility
Imports Zuken.E3.HarnessAnalyzer.D3D.Consolidated.Designs
Imports Zuken.E3.HarnessAnalyzer.D3D.Document
Imports Zuken.E3.HarnessAnalyzer.Settings

Namespace D3D.Consolidated.Controls

    Public Class CarStateMachine
        Implements IDisposable

        Public Event Changed(sender As Object, e As CarChangeEventArgs)
        Public Event Changing(sender As Object, e As CarChangeEventArgs)
        Public Event Progress(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs)
        Public Event Initialized(sender As Object, e As EventArgs)
        Public Event DirtyChanged(sender As Object, e As EventArgs)
        Public Event DirtyChanging(sender As Object, e As System.ComponentModel.CancelEventArgs)

        Private WithEvents _objectManipulator As ObjectManipulatorKeyAndScaleManager
        Private WithEvents _adjustStateMachine As AdjustStateMachine

        Private _model As DocumentDesign
        Private _carGroup As New Consolidated.Designs.Group3D()
        Private _carTransformationSetting As New CarTransformationSetting
        Private _lock As New System.Threading.LockStateMachine
        Private _adjustSettings As AdjustCarSettings
        Private _disposedValue As Boolean
        Private _initialCarPath As String

        Public Sub New(model As DocumentDesign, settings As AdjustCarSettings, adjustStateMachine As AdjustStateMachine)
            _model = model
            _adjustSettings = settings
            _objectManipulator = settings.ObjectManipulator
            _adjustStateMachine = adjustStateMachine
        End Sub

        ReadOnly Property IsBusy As Boolean
            Get
                Return (_lock?.HasAnyState).GetValueOrDefault
            End Get
        End Property

        Public Function HasChanges() As Boolean
            Return _objectManipulator.HasChanges OrElse HasChangedCarModel()
        End Function

        Private Function HasChangedCarModel() As Boolean
            Return _carGroup?.FilePath <> _initialCarPath
        End Function

        Friend Sub ApplyCarModelChanges()
            _initialCarPath = _carGroup?.FilePath
        End Sub

        Property Selected As Boolean
            Get
                Return (_carGroup?.Selected).GetValueOrDefault
            End Get
            Set(value As Boolean)
                If _carGroup IsNot Nothing Then
                    If _carGroup.Selected <> value Then
                        _carGroup.Selected = value
                    End If
                End If
            End Set
        End Property

        Friend Sub DisableManiuplation(mode As ObjectManipulatorChangeType)
            GetAllModelEntitiesExceptCar.RevertSelectable()
            Selected = False

            If _objectManipulator.Enabled Then
                Select Case mode
                    Case ObjectManipulatorChangeType.Cancel
                        _objectManipulator.Cancel()
                    Case Else
                        _objectManipulator.Apply()
                End Select
            End If
        End Sub

        Friend Function EnableManipulation() As Boolean
            GetAllModelEntitiesExceptCar.SetSelectable(False)
            If Not _objectManipulator.Enabled Then
                Return _objectManipulator.Enable(GetEntities)
            End If
            Return False
        End Function

        Property Selectable As Boolean
            Get
                Return (_carGroup?.Selectable).GetValueOrDefault
            End Get
            Set(value As Boolean)
                If _carGroup IsNot Nothing Then
                    If _carGroup.Selectable <> value Then
                        _carGroup.Selectable = value
                    End If
                End If
            End Set
        End Property

        ReadOnly Property LayerName As String
            Get
                Return _carGroup?.LayerName
            End Get
        End Property

        ReadOnly Property FilePath As String
            Get
                Return _carGroup?.FilePath
            End Get
        End Property

        Public Sub UpdateRotation(x As Single, y As Single, z As Single, settings As AdjustCarSettings)
            With settings.ObjectManipulator.Differences.Rotation
                .X = x
                .Y = y
                .Z = z
            End With

            settings.ObjectManipulator.Differences.Rotation.Update()
        End Sub

        Public Sub UpdatePosition(x As Single, y As Single, z As Single, settings As AdjustCarSettings)
            With settings.ObjectManipulator.Differences.Position
                .X = x
                .Y = y
                .Z = z
            End With

            settings.ObjectManipulator.Differences.Position.Update()
        End Sub

        Public Sub UpdateScale(percent As Single, settings As AdjustCarSettings)
            settings.ObjectManipulator.Differences.Scale.Percent = percent
            settings.ObjectManipulator.Differences.Scale.Update()
        End Sub


        Private Async Function LoadAndAddCarGroupCore(fullPath As String, Optional cancel As D3DCancellationTokenSource = Nothing) As Task(Of Group3D)
            Dim carGroup As Group3D = Await Group3D.LoadFromFileAsync(fullPath, _model, My.Settings.LastTransparency, Sub(e2 As System.ComponentModel.ProgressChangedEventArgs) OnProgress(New ProgressEventArgs(e2.ProgressPercentage, "Loading vehicle model")))
            OnProgress(New System.ComponentModel.ProgressChangedEventArgs(0, String.Empty))
            Await AddCarGroupAfterLoad(carGroup, fullPath)
            Return carGroup
        End Function

        Private Sub OnProgress(e As System.ComponentModel.ProgressChangedEventArgs)
            RaiseEvent Progress(Me, e)
        End Sub

        Private Sub AdjustModelAndInitCarSettings()
            AdjustCarModelToBundles()
            UpdateCarFromSettings()
            OnInitialized(New EventArgs)
        End Sub

        Private Sub UpdateCarFromSettings()
            SyncCurrentTransformationToCarGroup()
            _model.Entities.Regen()

            If _carGroup IsNot Nothing Then
                For Each vp As Viewport In _model.Viewports
                    vp.Rotate.Center = _carGroup.GetBoundingBox.GetCenter
                Next
            End If

            ZoomFitAndUpdateViews()
        End Sub

        Public Function GetEntities() As IEnumerable(Of Entity)
            If _carGroup IsNot Nothing Then
                Return _carGroup.Flatten.OfType(Of Entity)
            Else
                Return Array.Empty(Of Entity)()
            End If
        End Function

        Public Function GetAllModelEntitiesExceptCar() As IEnumerable(Of IEntity)
            If _carGroup IsNot Nothing Then
                Return _model.Entities.OfType(Of IEntity).Except(_carGroup)
            Else
                Return _model.Entities.OfType(Of IEntity)
            End If
        End Function

        Protected Overridable Sub OnInitialized(e As EventArgs)
            RaiseEvent Initialized(Me, e)
        End Sub

        Public Async Function LoadFromFile(carModelFile As String) As Task(Of IResult)
            Using Await _lock.BeginWaitAsync
                Dim retry As Boolean = False
                Do
                    Try
                        OnChanging(New CarChangeEventArgs(CarGroupChangeType.Add))

                        If _carGroup IsNot Nothing Then
                            _carGroup.TryRemoveMaterials(_model) ' HINT: we must remove the materials of the current car before loading them because eyeshot will recycle the current materials if they have the same name. The remove must be done BEFORE loading, because it seems that the file loader tries to add the materials while loading automatically (at this point there shoudn't be the same materialnames within the dictionary at the ViewportLayout)
                        End If

                        _carGroup = Await LoadAndAddCarGroupCore(carModelFile)

                        OnChanged(New CarChangeEventArgs(CarGroupChangeType.Add))
                    Catch ex As Exception
#If DEBUG Then
                        Throw
#End If
                        Select Case MessageBox.Show(String.Format(DialogStringsD3D.ErrorLoadingCarFile, carModelFile, ex.Message), HarnessAnalyzer.[Shared].MSG_BOX_TITLE, MessageBoxButtons.RetryCancel)
                            Case DialogResult.Cancel
                                Return Result.Success
                            Case DialogResult.Retry
                                retry = True
                        End Select
                    End Try
                Loop Until Not retry
                Return Result.Success
            End Using
        End Function

        Friend Sub OnVisibleChanged(visible As Boolean)
            If visible Then
                UpdateInitialCarPath()
            End If
        End Sub

        Private Sub UpdateInitialCarPath()
            If _carGroup IsNot Nothing Then
                _initialCarPath = _carGroup.FilePath
            Else
                _initialCarPath = Nothing
            End If
        End Sub

        Friend Async Function LoadAndAddCarModel(generalSettings As GeneralSettings, hcvTransformationSetting As CarTransformationSetting, Optional cancel As D3DCancellationTokenSource = Nothing) As Task(Of CarModelLoadResult)
            Dim result As New CarModelLoadResult(System.ComponentModel.ResultState.Cancelled, Nothing)

            Dim currentCarFileName As String = System.IO.Path.GetFileName(Me.FilePath)
            If Not String.IsNullOrEmpty(hcvTransformationSetting.CarFileName) AndAlso Not String.IsNullOrEmpty(generalSettings.CarModelDirectory) Then
                If hcvTransformationSetting.CarFileName = currentCarFileName Then
                    Return CarModelLoadResult.Success(Me.FilePath)
                End If

                If String.IsNullOrEmpty(generalSettings.CarModelDirectory) Then
                    Return Await Task.Run(Function() CarModelLoadResult.Faulted(ErrorStrings.D3D_CarModelDirectoryIsNotSet)) ' provides an awaiter
                End If

                If (Not E3.Lib.DotNet.Expansions.Devices.My.Computer.FileSystem.DirectoryExists(generalSettings.CarModelDirectory)) Then
                    Return Await Task.Run(Function() CarModelLoadResult.Faulted(String.Format(ErrorStrings.D3D_CarModelDirectoryIsNotFound, generalSettings.CarModelDirectory))) ' provides an awaiter
                End If

                Dim fullPath As String = IO.Path.Combine(generalSettings.CarModelDirectory, hcvTransformationSetting.CarFileName)
                If System.IO.File.Exists(fullPath) Then

                    If (cancel IsNot Nothing) Then
                        cancel.CanBeCancelled = True
                    End If

                    _carTransformationSetting = hcvTransformationSetting
                    Await LoadAndAddCarGroupCore(fullPath, cancel)

                    If (cancel IsNot Nothing) Then
                        If (cancel.IsCancellationRequested) Then
                            cancel.TellCancelHasDone()
                        End If
                        cancel.CanBeCancelled = False
                    End If

                    Return CarModelLoadResult.Success(fullPath)
                Else
                    Return Await Task.FromResult(CarModelLoadResult.Faulted(String.Format(ErrorStrings.D3D_CarModelDoesNotExistIn, hcvTransformationSetting.CarFileName, generalSettings.CarModelDirectory)))
                End If
            Else
                Return CarModelLoadResult.Cancelled(DialogStringsD3D.VehicleFileNotFound)
            End If
        End Function

        Public Async Function RevertCarModel() As Task
            If HasChangedCarModel() Then
                Await RemoveCarModel()
                If Not String.IsNullOrEmpty(_initialCarPath) Then
                    Await Me.LoadFromFile(_initialCarPath)
                End If
            End If
        End Function

        Public Async Function RemoveCarModel() As Task
            Using Await _lock.BeginWaitAsync
                If HasCar Then
                    OnChanging(New CarChangeEventArgs(CarGroupChangeType.Remove))
                    If _carGroup IsNot Nothing Then
                        If Not _carGroup.IsXhcv Then
                            _carTransformationSetting = Nothing
                        End If

                        _model.Entities.Remove(_carGroup)
                        _carGroup.Dispose()
                        _carGroup = Nothing
                    End If
                    OnChanged(New CarChangeEventArgs(CarGroupChangeType.Remove))
                End If
            End Using
        End Function

        Public Sub LoadCarTransformationSettings(path As String)
            _carTransformationSetting = LoadCarTransformationSettingsCore(path)
        End Sub

        Friend Shared Function LoadCarTransformationSettingsCore(path As String) As CarTransformationSetting
            Dim fi As New FileInfo(path)
            If fi.Exists Then
                Using fs As New FileStream(path, FileMode.Open)
                    Return LoadCarTransformationSettingsCore(fs)
                End Using
            End If
            Return Nothing
        End Function

        Friend Shared Function LoadCarTransformationSettingsCore(s As System.IO.Stream) As CarTransformationSetting
            If BinaryFile.IsBinFormatted(s) Then
                Dim oldSetting As CarTransformationSetting_2023 = CarTransformationSetting_2023.Load(s)
                Return New CarTransformationSetting(oldSetting.Transformation, oldSetting.CarFileName)
            ElseIf XmlFile.IsXml(s) Then
                Return CarTransformationSetting.Load(s)
            Else
                Throw New FileFormatException("Can't load car transformation: invalid format for deserializing!")
            End If
            Return CarTransformationSetting.Load(s)
        End Function

        Friend Shared Sub SaveCarTransformationSettingsCore(s As System.IO.Stream, settings As CarTransformationSetting)
            settings.Save(s)
        End Sub

        Private Sub AdjustAfterHandleCreated(sender As Object, e As EventArgs)
            If sender IsNot Nothing Then
                RemoveHandler CType(sender, Control).HandleCreated, AddressOf AdjustAfterHandleCreated
            End If

            If _carGroup IsNot Nothing AndAlso _carGroup.Transformation.ScaleFactorX = 0 Then
                _carGroup.Transformation.Zero()
                AdjustModelAndInitCarSettings()
            ElseIf _carGroup IsNot Nothing Then
                If _carGroup.Transformation.ScaleFactorX = 0 Then
                    AdjustModelAndInitCarSettings()
                Else
                    _carGroup.TransformBy()
                    AdjustModelAndInitCarSettings()
                End If
            End If
        End Sub

        Private Sub ZoomFitAndUpdateViews()
            ZoomfitAll()
            UpdateViews()
        End Sub

        Private Async Sub UpdateViews()
            Using Await _lock.BeginWaitAsync
                _model.SuspendUpdate(True)
                _model.Entities.Regen()
                For Each vp As Viewport In _model.Viewports
                    vp.Invalidate()
                Next
                _model.SuspendUpdate(False)
            End Using
        End Sub

        Public Sub ZoomfitAll(Optional invalidate As Boolean = False)
            With _model
                If (.IsHandleCreated) Then
                    .SuspendUpdate(True)
                    Using .ProtectProperty(NameOf(.AnimateCamera), False)
                        For Each vp As Viewport In _model.Viewports
                            vp.ZoomFit(CalculateFitMargin)
                        Next
                        '.ObjectManipulator.Visible = True
                        .SuspendUpdate(False)
                        If invalidate Then
                            .Invalidate()
                        End If
                    End Using
                End If
            End With
        End Sub

        Private Function CalculateFitMargin() As Integer
            Return CInt(((_model.Bounds.Width + _model.Bounds.Height) / 2) * 0.05)
        End Function

        Private Async Function AddCarGroupAfterLoad(carGroup As Designs.Group3D, fullPath As String) As Task
            If Not carGroup.IsXhcv Then
                carGroup.FilePath = fullPath
            End If

            If HasCar Then
                Dim filePath As String = If(_carGroup?.FilePath IsNot Nothing, _carGroup.FilePath, String.Empty)
                Await Me.RemoveCarModel()
            End If

            OnChanging(New CarChangeEventArgs(CarGroupChangeType.Add))

            _carGroup = carGroup
            _model.Entities.Add(_carGroup)
            _model.Entities.Regen()
            CreateEmptyTransformationSettingIfNeeded()
            SetCarTransformationFileName()

            OnChanged(New CarChangeEventArgs(CarGroupChangeType.Add))

            If _model.IsHandleCreated Then
                AdjustAfterHandleCreated(Nothing, New EventArgs)
            Else
                AddHandler _model.HandleCreated, AddressOf AdjustAfterHandleCreated
            End If
        End Function


        Private Sub OnChanged(e As CarChangeEventArgs)
            RaiseEvent Changed(Me, e)
        End Sub

        Private Sub OnChanging(e As CarChangeEventArgs)
            RaiseEvent Changing(Me, e)
        End Sub

        Property TransparencyPercent As UShort
            Get
                Return (_carGroup?.TransparencyPercent).GetValueOrDefault
            End Get
            Set(value As UShort)
                If _carGroup IsNot Nothing Then
                    _carGroup.TransparencyPercent = value
                End If
            End Set
        End Property

        Private Sub CreateEmptyTransformationSettingIfNeeded()
            If _carTransformationSetting Is Nothing Then
                _carTransformationSetting = New CarTransformationSetting
            End If
        End Sub

        Private Sub SetCarTransformationFileName()
            If _carTransformationSetting IsNot Nothing Then
                _carTransformationSetting.CarFileName = IO.Path.GetFileName(_carGroup?.FilePath)
            End If
        End Sub

        Private Sub SyncCurrentTransformationToCarGroup()
            If _carTransformationSetting IsNot Nothing AndAlso _carGroup IsNot Nothing Then
                ' Can only be called when entities are at origin, otherwise the transformation will be done additionally to the current position
                Dim matrix As Double(,) = _carTransformationSetting.Transformation
                If (matrix?.Length).GetValueOrDefault > 0 Then
                    _carGroup.Transformation = New devDept.Geometry.Transformation(matrix)
                Else
                    _carGroup.Transformation = New devDept.Geometry.Identity
                End If
            End If
        End Sub

        Public Sub AdjustCarModelToBundles(Optional forceCarAdjustment As Boolean = False)
            Dim entitiesExceptCar As List(Of IEntity) = GetAllModelEntitiesExceptCar().ToList

            If entitiesExceptCar.Count > 0 Then
                Dim entitiesExceptCarBox As BoundingBox = entitiesExceptCar.GetBoundingBox
                Dim entitiesExceptCarSize As Point3D = entitiesExceptCarBox.GetSize
                Dim entitiesExceptCarCenter As Point3D = entitiesExceptCarBox.GetCenter

                Dim carModelBox As New BoundingBox
                Dim carModelCenter As New Point3D
                Dim carModelSize As New Point3D

                If (_carGroup?.Count).GetValueOrDefault > 0 Then
                    carModelBox = _carGroup.GetBoundingBox
                    carModelCenter = carModelBox.GetCenter
                    carModelSize = carModelBox.GetSize
                End If

                If Double.IsInfinity(entitiesExceptCarCenter.AsVector.Length) Then
                    Throw New Exception("Can't adjust vehicle model as center of entities is infinite!")
                End If

                _adjustSettings.ClippingPlanes.Init(entitiesExceptCarCenter, entitiesExceptCarBox.Min, entitiesExceptCarBox.Max)

                If Not forceCarAdjustment Then
                    If (_carGroup?.Count).GetValueOrDefault > 0 AndAlso entitiesExceptCar.Count > 0 Then
                        'HINT: we are not adjusting the car to the entities when their boundingBoxes are overlapping or else when the volume-difference of the 2 Boxes are more than 20% (so ensure a plausable visible result for the user - only when the difference is to huge! - when he loads a car to the current entities)
                        If Not devDept.Geometry.Utility.DoOverlap(entitiesExceptCarBox.Min, entitiesExceptCarBox.Max, carModelBox.Min, carModelBox.Max) Then
                            Dim iMin As Point3D = Nothing
                            Dim iMax As Point3D = Nothing
                            Dim carBoxVolume As Double = carModelBox.GetVolume
                            Dim entitiesBoxVolume As Double = entitiesExceptCarBox.GetVolume
                            Dim volDiffPercent As Double = entitiesBoxVolume / carBoxVolume
                            If volDiffPercent <= 1.2 AndAlso volDiffPercent >= 0.2 Then
                                Return
                            End If
                        Else
                            Return
                        End If
                    End If
                End If

                'HINT: Order to multiplied: 1.Position,2.Rotation,3.Scaling

                Dim facx As Double = entitiesExceptCarSize.X / carModelSize.X
                Dim facy As Double = entitiesExceptCarSize.Y / carModelSize.Y
                Dim facz As Double = entitiesExceptCarSize.Z / carModelSize.Z

                Dim max1 As Double = Math.Max(facx, facy)
                Dim myScaleFac As Double = Math.Max(max1, facz)

                Dim dx As Double = entitiesExceptCarCenter.X - carModelCenter.X
                Dim dy As Double = entitiesExceptCarCenter.Y - carModelCenter.Y
                Dim dz As Double = entitiesExceptCarCenter.Z - carModelCenter.Z
                Dim myTransVector As New Vector3D(dx, dy, dz)

                Dim om_scale As devDept.Geometry.Transformation = Nothing
                Dim om_translation As devDept.Geometry.Transformation = Nothing
                Dim om_rotation As devDept.Geometry.Transformation = Nothing

                Dim isObjectManipulator As Boolean = _model.ObjectManipulator.Visible

                '------- 1. Position
                If isObjectManipulator Then
                    _objectManipulator.Cancel(True)
                    'CancelObjectManipulator(True, False, False)
                    om_translation = myTransVector.ToTranslation
                ElseIf _carGroup IsNot Nothing Then
                    _carGroup.Translate(myTransVector)
                End If

                ' ------- 2. Rotation
                ' HINT: 
                ' Rotation is not possible because it's in sum too complicated!
                ' The BoundingBox is always orientated to the coordinate system and not directly to the object (missing angle)
                ' Without the angle we had to determine this box-angle of the object by our self to make a rotation of the car in the direction of the bundles possible.
                ' This has to be done interative in every coordinate-system axis to "search" for the correct angle -> very complicated

                If Not isObjectManipulator AndAlso om_rotation IsNot Nothing AndAlso _carGroup IsNot Nothing Then
                    _carGroup.Transformation *= om_rotation
                End If

                ' ------- 3. Scaling

                If myScaleFac > 0 Then
                    If isObjectManipulator Then
                        om_scale = _objectManipulator.GetScaledTransformation(myScaleFac)
                    ElseIf _carGroup IsNot Nothing Then
                        _carGroup.Scale(myScaleFac)
                    End If
                End If

                If isObjectManipulator Then
                    _objectManipulator.SetOmTransformation(om_translation, om_rotation, om_scale)
                End If
            Else
                UpdateViews()
            End If
        End Sub

        Friend Sub SaveCarTransformation(s As System.IO.Stream)
            Dim setting As New CarTransformationSetting()
            setting.CarFileName = _carTransformationSetting?.CarFileName
            If Me._objectManipulator.Enabled Then
                setting.Transformation = GetSettingsTransformationCore() ' setting is updated after apply, but saving transformation while enabled has no updated settings -> directly create from current transformation
            Else
                setting = _carTransformationSetting
            End If

            SaveCarTransformationSettingsCore(s, setting)
        End Sub

        Friend Sub SaveCarTransformationTo(filePath As String)
            If String.IsNullOrEmpty(filePath) Then
                Throw New ArgumentNullException(NameOf(filePath), "Filepath is null/empty!")
            End If

            Using fs As New FileStream(filePath, FileMode.Create)
                SaveCarTransformation(fs)
            End Using
        End Sub

        Private Sub UpdateTransformationSettings()
            CreateEmptyTransformationSettingIfNeeded()
            _carTransformationSetting.Transformation = GetSettingsTransformationCore()
        End Sub

        Private Function GetSettingsTransformationCore() As Double(,)
            Dim objTrans As devDept.Geometry.Transformation = CType(_objectManipulator.Transformation.Clone, devDept.Geometry.Transformation)
            Dim settingsMatrix As Double(,) = _carTransformationSetting.Transformation
            Dim settingsTransformation As devDept.Geometry.Transformation = If(settingsMatrix?.Length > 0, New devDept.Geometry.Transformation(settingsMatrix), New devDept.Geometry.Identity)
            Dim resultTrans As Transformation = objTrans * settingsTransformation
            Return resultTrans.Matrix
        End Function

        Private Sub _objectManipulator_Changed(sender As Object, e As ObjectManipulatorChangeEventArgs) Handles _objectManipulator.Changed
            If e.ChangeType.HasFlag(ObjectManipulatorChangeType.Apply) Then
                Me.UpdateTransformationSettings()
                Me.ApplyCarModelChanges()
            End If
        End Sub

        ReadOnly Property HasCar As Boolean
            Get
                Return (_carGroup?.Count).GetValueOrDefault > 0
            End Get
        End Property

        Public Property Transformation As CarTransformationSetting
            Get
                Return _carTransformationSetting
            End Get
            Set
                If Not Value Is _carTransformationSetting Then
                    _carTransformationSetting = Value

                    If _objectManipulator IsNot Nothing Then
                        If Value IsNot Nothing AndAlso Value.Transformation.Length > 0 Then
                            _objectManipulator.TransformationTotal = New devDept.Geometry.Transformation(Value.Transformation)
                        Else
                            _objectManipulator.TransformationTotal = New Identity
                        End If
                    End If

                    If Value Is Nothing Then
                        _carGroup = New Designs.Group3D()
                    End If
                End If
            End Set
        End Property

        Private Sub _adjustStateMachine_Changed(sender As Object, e As AdjustModeChangedEventArgs) Handles _adjustStateMachine.Changed
            If _adjustStateMachine.AdjustMode Then
                UpdateInitialCarPath()
            End If
        End Sub

        Friend Class CarModelLoadResult
            Inherits System.ComponentModel.Result

            Protected Sub New()
                MyBase.New
            End Sub

            Public Sub New(result As System.ComponentModel.ResultState, carFullPath As String)
                Me.New(result, Nothing, carFullPath)
            End Sub

            Public Sub New(result As System.ComponentModel.ResultState, message As String, carFullPath As String)
                MyBase.New(result, message)
                Me.CarFullPath = carFullPath
            End Sub

            Public Shared Shadows ReadOnly Property Success(carFullPath As String) As CarModelLoadResult
                Get
                    Dim result As CarModelLoadResult = CreateSuccess(Of CarModelLoadResult)()
                    result._CarFullPath = carFullPath
                    Return result
                End Get
            End Property

            Public Shared Shadows ReadOnly Property Cancelled(Optional message As String = "", Optional carFullPath As String = Nothing) As CarModelLoadResult
                Get
                    Dim result As CarModelLoadResult = CreateCancelled(Of CarModelLoadResult)(message)
                    result._CarFullPath = carFullPath
                    Return result
                End Get
            End Property

            Public Shared Shadows ReadOnly Property Faulted(message As String, Optional carFullPath As String = Nothing) As CarModelLoadResult
                Get
                    Dim result As CarModelLoadResult = CreateFaulted(Of CarModelLoadResult)(message)
                    result._CarFullPath = carFullPath
                    Return result
                End Get
            End Property

            Public ReadOnly Property CarFullPath As String

        End Class

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not _disposedValue Then
                If disposing Then
                    If _lock IsNot Nothing Then
                        _lock.Dispose()
                    End If
                End If

                _adjustStateMachine = Nothing
                _model = Nothing
                _carGroup = Nothing
                _carTransformationSetting = Nothing
                _lock = Nothing
                _adjustSettings = Nothing
                _objectManipulator = Nothing
                _disposedValue = True
            End If
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub

    End Class

End Namespace