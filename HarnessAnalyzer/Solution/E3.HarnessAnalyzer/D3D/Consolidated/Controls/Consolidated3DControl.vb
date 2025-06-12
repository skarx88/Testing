Imports System.ComponentModel
Imports System.IO
Imports System.Reflection
Imports devDept.Eyeshot
Imports devDept.Eyeshot.Entities
Imports devDept.Eyeshot.Workspace
Imports devDept.Geometry
Imports devDept.Geometry.Entities.GMesh
Imports devDept.Graphics
Imports Infragistics.Win.UltraWinDock
Imports Infragistics.Win.UltraWinToolTip
Imports Zuken.E3.HarnessAnalyzer.D3D.Consolidated.Designs
Imports Zuken.E3.HarnessAnalyzer.D3D.Shared
Imports Zuken.E3.HarnessAnalyzer.Project.Documents
Imports Zuken.E3.Lib.Eyeshot.Model
Imports Zuken.E3.Lib.IO.Files.Hcv
Imports Zuken.E3.Lib.Model

Namespace D3D.Consolidated.Controls

    <Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
    Partial Public Class Consolidated3DControl

        Public Event ToolBarButtonClick(sender As Object, e As ToolBarButtonEventArgs)
        Public Event SelectedEntitiesChanged(sender As Object, e As SelectedEntitiesChangedEventArgs)
        Public Event GroupVisibilityChanged(sender As Object, e As GroupVisibilityChangedEventArgs)
        Public Event CarChanging(sender As Object, e As CarChangeEventArgs)
        Public Event CarChanged(sender As Object, e As CarChangeEventArgs)
        Public Event AfterCarUnloaded(sender As Object, e As EventArgs)
        Public Event Progress(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs)
        Public Event AdjustModeChanged(sender As Object, e As EventArgs)
        Public Event DocumentsChanging(sender As Object, e As DocumentsChangeEventArgs)
        Public Event DocumentsChanged(sender As Object, e As DocumentsChangeEventArgs)
        Public Shadows Event VisibleChanged(sender As Object, e As EventArgs)

        Private WithEvents _documentsListForm As DocumentsListForm
        Private WithEvents _transparencyCtrlCar As New OpacityControl
        Private WithEvents _transparencyCtrlJT As New OpacityControl
        Private WithEvents _attached As New DocumentDesignClonesCollection(Me)
        Private WithEvents _carStateMachine As CarStateMachine
        Private WithEvents _adjustModeStateMachine As AdjustStateMachine

        Private _adjustCarSetting As AdjustCarSettings
        Private _parentWindowPane As DockableControlPane
        Private _activeClipPlanes As New List(Of ClippingPlane)
        Private _isUpdating As Boolean = False
        Private _isCameraMoving As Boolean = False
        Private _isUpdatingViews As Boolean = False
        Private _tip As UltraToolTipInfo
        Private _lastEntity As IEntity = Nothing

        Private _eventsEnabled As Boolean = True
        Private _previousEventsEnabled As Boolean = True
        Private _toolBarButtons As New Dictionary(Of String, ToolBarButton)
        Private _lastActiveViewPort As Viewport

        Private _keyRotateEnabled As Boolean
        Private _keyPanEnabled As Boolean

        Private _transparencyCarPopUp As New Infragistics.Win.Misc.UltraPopupControlContainer
        Private _transparencyJTPopup As New Infragistics.Win.Misc.UltraPopupControlContainer

        Private _jtModelGroup As New Consolidated.Designs.Group3D() 'JTModelGroup only contains objects that do not have a linking to the model!  (the description is not optimal!) 

        Private _isControlLoaded As Boolean = False
        Private _initialized As InitializedType = InitializedType.None
        Private _lock As New System.Threading.LockStateMachine

        Private _clippingPlanes As ClippingPlaneSet
        Private _keyStepsBefore As ViewportSteps
        Private _layout4 As Layout4

        Private _lockState As New System.Threading.LockStateMachine
        Private _internalRemovingJTModel As Boolean = False
        Private _paneKey As String = Nothing
        Private _isDisplayed As Boolean

        Private WithEvents _objManipulatorManager As ObjectManipulatorKeyAndScaleManager
        Private WithEvents _mouseAndTouchManager As MouseAndTouchManager

        Sub New()
            InitializeComponent()

            With Me.Design3D
                _layout4 = New Layout4(Me.Design3D)
                _clippingPlanes = New ClippingPlaneSet(Me.Design3D)

                .OrientationMode = orientationType.UpAxisZ
                .WheelZoomEnabled = True
                .MultipleSelection = True 'HINT: this selection is set to true because our derived eyeshot control is behaving to use this property as enabled multiselect and disabled multiselect but the standard eyeshot control is not behaving like this: eyeshot is not executing a remove on selectionChanged-Event when this property is set to true like a always-pressed-ctrl-key

                InitViewport()

                D3D.Shared.PanSettings.Default.Set(Design3D)
                D3D.Shared.RotationSettings.Default.Set(Design3D)

                Dim initResult As InitDefaultsResult = .InitDefaults() ' HINT: init after all viewports are added
                _mouseAndTouchManager = initResult.MouseAndTouchManager
                _objManipulatorManager = initResult.ObjManipulatorManager
                _objManipulatorManager.IsFineAdjustment = False
                _objManipulatorManager.Differences.Scale.Max = CSng(uneScalePercent.MaxValue)
                _objManipulatorManager.Differences.Scale.Min = CSng(uneScalePercent.MinValue)
                _lastActiveViewPort = .ActiveViewport
                _keyStepsBefore = ViewportSteps.CreateFrom(Design3D)

                With _layout4
                    _keyRotateEnabled = .Main.Rotate.KeysStep > 0
                    _keyPanEnabled = .Main.Pan.KeysStep > 0
                End With

                SetUpperPanelCollapsed(True) ' HINT: initial is always hidden

                _transparencyCarPopUp.PopupControl = _transparencyCtrlCar
                _transparencyJTPopup.PopupControl = _transparencyCtrlJT

                .SetCameraProjectionMode(projectionType.Orthographic)

                .EEModel = New EEModelsCollection ' HINT: no auto-entities conversion here (settings set to nothing), because we want to set the entities manually, but we connect the model
                _adjustCarSetting = New AdjustCarSettings(_objManipulatorManager, _clippingPlanes, _layout4)

                With AdjustStateMachine.Create(Design3D, _adjustCarSetting)
                    _carStateMachine = .CarStateMachine
                    _adjustModeStateMachine = .AdjustStateMachine
                End With

                CarModelsViewControl.CarStateMachine = _carStateMachine
                .CreateControl() ' HINT: Ensures that the handle for the viewport layout is created at the beginning. (Forces to create the handle in constructor because sometimes there is an access (that needs a ViewportLayout.HandleCreated=true) needed to the ViewportLayout before the (handle was created/) control was shown - normally on Control.Show)

                AddEvents()
                InitContextMenu()
            End With
        End Sub

        Private Sub AddEvents()
            AddHandler Design3D.Entities.GroupPropertyChanged, AddressOf _Design3D_GroupPropertyChanged
        End Sub

        Private Sub RemoveEvents()
            If Design3D?.Entities IsNot Nothing Then
                RemoveHandler Design3D.Entities.GroupPropertyChanged, AddressOf _Design3D_GroupPropertyChanged
            End If

            RemoveHandler itemShowAll.Click, AddressOf ResetAllHiddenEntities
            RemoveHandler itemShow.Click, AddressOf ResetHiddenEntity
            RemoveHandler itemHide.Click, AddressOf AddEntityToHiddens
        End Sub

        Public Sub Initialize(carModelsDirectory As String, autoSyncSelection As Boolean, Optional reInitialize As Boolean = False)
            If reInitialize OrElse Initialized = InitializedType.None Then
                Me.CarModelsDirectory = carModelsDirectory
                GetToolBarButton(D3D.Consolidated.Controls.Consolidated3DControl.ToolBarButtons.ChangeCarTransparency).Enabled = False
                GetToolBarButton(D3D.Consolidated.Controls.Consolidated3DControl.ToolBarButtons.ChangeJTTransparency).Enabled = False
                GetToolBarButton(D3D.Consolidated.Controls.Consolidated3DControl.ToolBarButtons.SynchronizeSelection).Visible = Not autoSyncSelection
                Design3D.ViewportSizingEnabled = False
                _initialized = InitializedType.First
            ElseIf _initialized = InitializedType.First Then
                _initialized = InitializedType.More
            End If
        End Sub

        Shadows Sub Update(Optional adjust As Boolean = True, Optional invalidate As Boolean = True)
            _isUpdating = True
            Try
                With Me.Design3D
                    Try
                        .Entities.Regen()
                    Catch ex As NullReferenceException
                        Throw New OperationCanceledException("Eyeshot entities regenerate null reference internal, cancelled update: " & ex.Message)
                    End Try

                    If adjust Then
                        _carStateMachine.AdjustCarModelToBundles()
                        ZoomFitAll()
                    End If

                    If invalidate Then
                        UpdateViews()
                    End If

                    MyBase.Update()
                End With
            Catch ex As OperationCanceledException
                ' Update was cancelled
            Finally
                _isUpdating = False
            End Try
        End Sub

        Private Sub Me_Load(sender As Object, e As EventArgs) Handles Me.Load
            Me.Design3D.ActionMode = D3D.Shared.DEFAULT_ACTION_MODE

            SetCarTransparencySettingToControl()
            SetJTTransparencySettingToControl()
            Me.StatusStrip1.Visible = DebugEx.IsDebug
            _isControlLoaded = True
        End Sub

        Friend Sub SaveCarTransformationToFile(filePath As String)
            _carStateMachine?.SaveCarTransformationTo(filePath)
        End Sub

        Function GetTransformationAsContainerFile() As CarTransformationContainerFile
            Using ms As New MemoryStream
                _carStateMachine?.SaveCarTransformation(ms)
                ms.Position = 0
                Return New CarTransformationContainerFile(ms.ToArray)
            End Using
        End Function

        Private Sub ClearAttached()
            If _attached IsNot Nothing Then
                For Each har As DocumentDesignClone In _attached
                    RemoveEvents(har)
                    har.Dispose()
                Next
                _attached.Clear()
            End If
        End Sub

        Private Sub ViewportLayout1_BeforeDrawViewPort(sender As Object, e As ConsolidatedDesign.DrawViewPortEventArgs)
            e.Params.Entities = Design3D.SortEntitiesForTransparency(e.Params.Viewport, e.Params.Entities)
        End Sub

        Private Sub RemoveOldCamera()
            With Design3D
                Dim myOldCam As Mesh = CType(.Entities.OfType(Of Entity).Where(Function(en) en.EntityData Is "cam").FirstOrDefault, Mesh)
                If myOldCam IsNot Nothing Then
                    .Entities.Remove(myOldCam)
                End If
                .Entities.OfType(Of Entity).Where(Function(e) e.EntityData Is "line").ToList().ForEach(Function(e) .Entities.Remove(e))
            End With
        End Sub

        Public Sub SetClipPlanes()
            If _activeClipPlanes IsNot Nothing Then
                Dim Plane As Mesh
                Dim x1, x2, y2, y1 As Double
                Dim dx, dy As Double

                dx = 200
                dy = 80

                x1 = -dx / 2
                x2 = dx / 2
                y2 = dy / 2
                y1 = -dy / 2

                For Each c As ClippingPlane In _activeClipPlanes
                    Dim myplane As Plane = CType(c.Plane.Clone, devDept.Geometry.Plane)

                    Dim myEntities As New EntityList '= CType(_myChild.Entities.Where(Function(en) TypeOf (en) Is Mesh).ToList(), EntityList)
                    Dim myVertices As New List(Of devDept.Geometry.Point3D)

                    Dim BoxMin As New devDept.Geometry.Point3D
                    Dim BoxMax As New devDept.Geometry.Point3D

                    Utility.BoundingBox(myVertices, BoxMin, BoxMax)

                    Dim ptList As New List(Of Point2D)
                    Dim fac As Double = 0.2

                    If c.Normal.Y = 0 AndAlso c.Normal.Z = 0 Then
                        x1 = BoxMin.Y
                        x2 = BoxMax.Y
                        y1 = BoxMin.Z
                        y2 = BoxMax.Z
                    End If

                    If c.Normal.X = 0 AndAlso c.Normal.Z = 0 Then
                        x1 = BoxMin.Z * c.Normal.Y
                        x2 = BoxMax.Z * c.Normal.Y
                        y1 = -BoxMin.X * c.Normal.Y
                        y2 = -BoxMax.X * c.Normal.Y
                    End If

                    If c.Normal.X = 0 AndAlso c.Normal.Y = 0 Then
                        x1 = BoxMin.X
                        x2 = BoxMax.X
                        y1 = BoxMin.Y
                        y2 = BoxMax.Y
                    End If

                    x1 = x1 - fac * Math.Abs(x1)
                    x2 = x2 + fac * Math.Abs(x2)
                    y1 = y1 - fac * Math.Abs(y1)
                    y2 = y2 + fac * Math.Abs(y2)

                    ptList.Add(New Point2D(x1, y1))
                    ptList.Add(New Point2D(x1, y2))
                    ptList.Add(New Point2D(x2, y2))

                    ptList.Add(New Point2D(x2, y2))
                    ptList.Add(New Point2D(x2, y1))
                    ptList.Add(New Point2D(x1, y1))

                    Try
                        Plane = Mesh.CreatePlanar(myplane, ptList, natureType.RichPlain)
                        Plane.EntityData = "myPlane"
                        Plane.ColorMethod = colorMethodType.byEntity
                        Plane.Color = Color.FromArgb(150, 50, 50, 50)
                        Plane.Regen(0.1)
                    Catch ex As Exception
                        Throw
                        ' TODO: check  if is possible to crop that down to only the wanted exception(s) -> ALL excetions get caught and thrown away !
                    End Try
                Next
            End If
        End Sub

        Private Sub UpdateViews()
            _isUpdatingViews = True
            With Me.Design3D
                .SuspendUpdate(True)
                .Entities.Regen()
                _layout4.InvalidateAll()
                .SuspendUpdate(False)
            End With
            _isUpdatingViews = False
        End Sub

        Private Function CalculateFitMargin() As Integer
            Return CInt(((Me.Bounds.Width + Me.Bounds.Height) / 2) * 0.05)
        End Function

        Public Sub ZoomFitActiveToSelection()
            If Me.SelectedEntities.Count > 0 Then
                With Me.Design3D
                    If (.IsHandleCreated) Then
                        .ActiveViewport.SaveView(.ActiveViewport.Camera)
                        Dim proportion As Double = Me.Bounds.Width / Me.Bounds.Height
                        If proportion >= 0.4 AndAlso proportion <= 3.0 Then
                            .ActiveViewport.ZoomFit(True, Camera.perspectiveFitType.Accurate, CalculateFitMargin)
                        Else
                            .ActiveViewport.ZoomFit(True, Camera.perspectiveFitType.Accurate)
                        End If
                    End If
                End With
            End If
        End Sub

        Public Sub ZoomFitAll(Optional invalidate As Boolean = False)
            With Me.Design3D
                If .IsHandleCreated Then
                    .SuspendUpdate(True)
                    Using .ProtectProperty(NameOf(.AnimateCamera), False)
                        _layout4.ZoomfitAll(CalculateFitMargin)
                        '.ObjectManipulator.Visible = True
                        .SuspendUpdate(False)

                        If invalidate Then
                            .Invalidate()
                        End If
                    End Using
                End If
            End With
        End Sub

        Private Sub Me_CameraMoveBegin(sender As Object, e As CameraMoveEventArgs) Handles Design3D.CameraMoveBegin
            _isCameraMoving = True
        End Sub

        Private Sub Me_CameraMoveEnd(sender As Object, e As CameraMoveEventArgs) Handles Design3D.CameraMoveEnd
            _isCameraMoving = False
        End Sub

        Public Sub RevertEventsEnabled()
            Me.EventsEnabled = _previousEventsEnabled
        End Sub

        Private Sub _toolBarButton_Click(sender As Object, e As EventArgs)
            RaiseEvent ToolBarButtonClick(Me, New ToolBarButtonEventArgs(DirectCast(sender, devDept.Eyeshot.ToolBarButton)))
        End Sub

        Public Function ToolBarButtonExists(buttonType As ToolBarButtons) As Boolean
            If _toolBarButtons IsNot Nothing Then
                Return _toolBarButtons.ContainsKey(buttonType.ToString)
            End If
            Return False
        End Function

        Public Function GetToolBarButton(buttonEnum As ToolBarButtons) As ToolBarButton
            Dim button As ToolBarButton = Nothing
            _toolBarButtons.TryGetValue(buttonEnum.ToString, button)
            Return button
        End Function

        Private Sub OnProgress(e As System.ComponentModel.ProgressChangedEventArgs)
            RaiseEvent Progress(Me, e)
        End Sub

        Protected Overridable Sub OnAdjustModeChanged(e As EventArgs)
            RaiseEvent AdjustModeChanged(Me, e)
        End Sub

        Private Sub D3DControl_ParentChanged(sender As Object, e As EventArgs) Handles Me.ParentChanged
            If TypeOf Me.Parent Is DockableWindow Then
                RemoveParentWindowEvents()
                _parentWindowPane = CType(Me.Parent, DockableWindow).Pane
                AddParentWindowEvents()
            End If
        End Sub

        Private Sub _parent_subObjectPropChanged(e As Infragistics.Shared.PropChangeInfo)
            If CType(e.PropId, Infragistics.Win.UltraWinDock.PropertyIds) = Infragistics.Win.UltraWinDock.PropertyIds.Closed Then
                If _transparencyCtrlCar.Visible Then
                    _transparencyCtrlCar.Hide()
                End If

                If _transparencyCtrlJT.Visible Then
                    _transparencyCtrlJT.Hide()
                End If

                If _documentsListForm IsNot Nothing AndAlso _documentsListForm.Visible Then
                    _documentsListForm.Hide()
                End If
            End If
        End Sub

        Private Sub AddParentWindowEvents()
            AddHandler _parentWindowPane.SubObjectPropChanged, AddressOf _parent_subObjectPropChanged
        End Sub

        Private Sub RemoveParentWindowEvents()
            If _parentWindowPane IsNot Nothing Then
                RemoveHandler _parentWindowPane.SubObjectPropChanged, AddressOf _parent_subObjectPropChanged
            End If
        End Sub

        Private Sub _Design3D_GroupPropertyChanged(sender As Object, e As EntityPropertyChangedEventArgs)
            If TypeOf e.Entity Is Group3D AndAlso e.PropertyName = NameOf(Group3D.Visible) Then
                RaiseEvent GroupVisibilityChanged(Me, New GroupVisibilityChangedEventArgs(CType(e.Entity, Group3D)))
            End If
        End Sub

        Public Async Function WriteVisibleNonCarEntitiesAsync(fullPath As String) As Task
            Dim entities As IEnumerable(Of IEntity) = _carStateMachine.GetAllModelEntitiesExceptCar.Where(Function(ent) ent.Visible)
            Using writer As New D3D.Consolidated.Designs.ObjectFileWriter(Me.Design3D)
                writer.Entities.AddRange(entities.OfType(Of Entity))
                Await writer.SaveAsync(fullPath)
            End Using
        End Function

        Private Sub btnAutoAdjust_Click(sender As Object, e As EventArgs) Handles btnAutoAdjust.Click
            _carStateMachine.AdjustCarModelToBundles(True)
            ' Me.Update(False)
            UpdateTextBoxes()
        End Sub

        Private Sub Design3D_EEObjectDataRequested(sender As Object, e As EEObjectDataEventArgs) Handles Design3D.EEObjectDataRequested
            ' HINT: override the isActive-variant check from to default to our specific document variant which is used to control the disabled/enabled-state of the objects by the document (see Document.ISACTIVE_VARIANT_NAME)
            Dim doc As HcvDocument = GetAttachedEEObjectContainingDocument(e.EEObjectId)
            If doc IsNot Nothing Then
                HcvDocument.ProcessNeedsEEObjectData(e, doc)
            End If
        End Sub

        Private Function GetAttachedEEObjectContainingDocument(eeObjectId As Guid) As HcvDocument
            For Each har As DocumentDesignClone In _attached
                If har.Document?.Model IsNot Nothing AndAlso har.Document.Model.Contains(eeObjectId) Then
                    Return har.Document
                End If
            Next
            Return Nothing
        End Function

        Private Sub Design3D_BeforeActionChanging(sender As Object, e As ActionChangingEventArgs) Handles Design3D.BeforeActionChanging
            If e.Action = actionType.Rotate Then
                If Design3D.ActiveViewport IsNot _layout4.Main Then ' HINT: block rotation for all other viewports except main
                    e.Cancel = True
                End If
                If My.Application.MainForm.GeneralSettings.UseSelectionCenterForRotation AndAlso SelectedEntities.Count > 0 Then
                    Dim center As Point3D = SelectedEntities.GetBoundingBox.GetCenter
                    Design3D.ActiveViewport.Rotate.RotationCenter = rotationCenterType.Point
                    Design3D.ActiveViewport.Rotate.Center = center
                End If

            End If
        End Sub

        Private Sub _carStateMachine_Progress(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles _carStateMachine.Progress
            OnProgress(e)
        End Sub

        Private Sub _carStateMachine_CarChanged(sender As Object, e As CarChangeEventArgs) Handles _carStateMachine.Changed
            RaiseEvent CarChanged(Me, e)
        End Sub

        Private Sub _carStateMachine_Initialized(sender As Object, e As EventArgs) Handles _carStateMachine.Initialized
            _carStateMachine.TransparencyPercent = _transparencyCtrlCar.TransparencyPercent
            GetToolBarButton(ToolBarButtons.ChangeCarTransparency).Enabled = True
        End Sub

        Private ReadOnly Property ToggleAdjustModeToolBarButton As ToolBarButton
            Get
                Return GetToolBarButton(ToolBarButtons.ToggleAdjustMode)
            End Get
        End Property

        Private Sub _adjustModeStateMachine_Changed(sender As Object, e As AdjustModeChangedEventArgs) Handles _adjustModeStateMachine.Changed
            Me.KeyRotateEnabled = Not _adjustModeStateMachine.AdjustMode
            Me.KeyPanEnabled = Not _adjustModeStateMachine.AdjustMode

            If _adjustModeStateMachine.AdjustMode AndAlso HasCar Then
                _carStateMachine.EnableManipulation()
                SetUpperPanelCollapsed(False)
                Me.ZoomFitAll()
            Else
                Dim hasChanges As Boolean = (_objManipulatorManager?.HasChanges).GetValueOrDefault

                If _carStateMachine IsNot Nothing Then
                    _carStateMachine.DisableManiuplation(e.OmChangeType)
                End If

                SetUpperPanelCollapsed(True)

                If hasChanges Then
                    UpdateTextsFromObjManipulator()
                End If
            End If

            If ToggleAdjustModeToolBarButton IsNot Nothing Then
                ToggleAdjustModeToolBarButton.Pushed = AdjustMode
            End If

            OnActiveViewPortChanged(New EventArgs)
        End Sub

        Protected Overrides Sub OnParentChanged(e As EventArgs)
            Static lastForm As DockableWindow = Nothing
            MyBase.OnParentChanged(e)

            Dim myForm As DockableWindow = Me.FindParent(Of DockableWindow)
            If myForm IsNot lastForm Then
                If myForm IsNot Nothing Then
                    If lastForm IsNot Nothing Then
                        RemoveHandler lastForm.Pane.Manager.PaneHidden, AddressOf _dockManager_PaneHidden
                        RemoveHandler lastForm.Pane.Manager.PaneDisplayed, AddressOf _dockManager_PaneDisplayed
                    End If

                    lastForm = myForm

                    AddHandler myForm.Pane.Manager.PaneHidden, AddressOf _dockManager_PaneHidden
                    AddHandler myForm.Pane.Manager.PaneDisplayed, AddressOf _dockManager_PaneDisplayed
                    _paneKey = myForm.Pane.Key
                Else
                    RemoveHandler lastForm.Pane.Manager.PaneHidden, AddressOf _dockManager_PaneHidden
                    RemoveHandler myForm.Pane.Manager.PaneDisplayed, AddressOf _dockManager_PaneDisplayed
                    lastForm = Nothing
                    _paneKey = String.Empty
                End If
            End If
        End Sub

        Private Sub _dockManager_PaneHidden(sender As Object, e As PaneHiddenEventArgs)
            If e.Pane.Key = _paneKey Then
                _isDisplayed = False
                OnVisibleChangedInternal()
            End If
        End Sub

        Private Sub _dockManager_PaneDisplayed(ByVal sender As Object, ByVal e As PaneDisplayedEventArgs)
            If e.Pane.Key = _paneKey Then
                _isDisplayed = True
                OnVisibleChangedInternal()
            End If
        End Sub

        Friend Overloads Sub OnVisibleChangedInternal()
            ProcessOnAfterVisibleChanged()
        End Sub

        Public Shadows Sub Invalidate(children As Boolean)
            If children = True Then
                Me.Design3D.UpdateBoundingBox() ' HINT: needed to reflect visibility-changes > eyeshot 2022
                Me.Design3D.Invalidate(children)
            End If

            MyBase.Invalidate(children)
        End Sub

    End Class

End Namespace