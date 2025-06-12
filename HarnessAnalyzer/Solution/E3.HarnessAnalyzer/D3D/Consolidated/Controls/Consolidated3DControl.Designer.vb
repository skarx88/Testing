Namespace D3D.Consolidated.Controls

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class Consolidated3DControl
        Inherits UserControl

        'UserControl overrides dispose to clean up the component list.
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing Then
                    RemoveParentWindowEvents()
                    RemoveEvents()
                    RemoveToolBarButtonEvents()

                    If _documentsListForm IsNot Nothing Then
                        _documentsListForm.Dispose()
                    End If

                    If _transparencyCtrlCar IsNot Nothing Then
                        _transparencyCtrlCar.Dispose()
                    End If

                    If _transparencyCtrlJT IsNot Nothing Then
                        _transparencyCtrlJT.Dispose()
                    End If

                    ClearAndRemoveJTModelGroup()

                    If _adjustModeStateMachine IsNot Nothing Then
                        _adjustModeStateMachine.Dispose()
                    End If

                    If _adjustCarSetting IsNot Nothing Then
                        _adjustCarSetting.Dispose()
                    End If

                    If _carStateMachine IsNot Nothing Then
                        Dim tsk As Task = _carStateMachine.RemoveCarModel()
                        tsk.Wait()
                        _carStateMachine.Dispose()
                    End If

                    ClearAttached()

                    If _mouseAndTouchManager IsNot Nothing Then
                        _mouseAndTouchManager.Dispose()
                    End If

                    If _objManipulatorManager IsNot Nothing Then
                        _objManipulatorManager.Dispose() ' HINT: after Remove carGroup (remove needs an non disposed Manager) 
                    End If

                    If components IsNot Nothing Then
                        components.Dispose()
                    End If
                End If

                _initialized = InitializedType.None
                _carStateMachine = Nothing
                _adjustModeStateMachine = Nothing
                _adjustCarSetting = Nothing
                _jtModelGroup = Nothing
                _documentsListForm = Nothing
                _transparencyCtrlCar = Nothing
                _transparencyCtrlJT = Nothing
                _objManipulatorManager = Nothing
                _mouseAndTouchManager = Nothing
                _attached = Nothing
                _lastEntity = Nothing
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        ' <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            components = New ComponentModel.Container()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Consolidated3DControl))
            Dim Appearance1 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim ShortcutKeysSettings1 As devDept.Eyeshot.ShortcutKeysSettings = New devDept.Eyeshot.ShortcutKeysSettings(CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.A), System.Windows.Forms.Keys), CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.I), System.Windows.Forms.Keys), System.Windows.Forms.Keys.None, CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.F), System.Windows.Forms.Keys), CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Add), System.Windows.Forms.Keys), CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Subtract), System.Windows.Forms.Keys), CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.C), System.Windows.Forms.Keys), CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.V), System.Windows.Forms.Keys), CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.X), System.Windows.Forms.Keys), CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.G), System.Windows.Forms.Keys), CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Shift) _
                Or System.Windows.Forms.Keys.G), System.Windows.Forms.Keys), System.Windows.Forms.Keys.Right, System.Windows.Forms.Keys.Up, System.Windows.Forms.Keys.Left, System.Windows.Forms.Keys.Down, CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Right), System.Windows.Forms.Keys), CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Up), System.Windows.Forms.Keys), CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Left), System.Windows.Forms.Keys), CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Down), System.Windows.Forms.Keys), System.Windows.Forms.Keys.Escape, System.Windows.Forms.Keys.D, System.Windows.Forms.Keys.A, System.Windows.Forms.Keys.E, System.Windows.Forms.Keys.Q, System.Windows.Forms.Keys.W, System.Windows.Forms.Keys.S)
            Dim ObjectManipulatorPartProperties1 As devDept.Eyeshot.ObjectManipulatorPartProperties = New devDept.Eyeshot.ObjectManipulatorPartProperties(System.Drawing.Color.Red, True, True)
            Dim ObjectManipulatorPartProperties2 As devDept.Eyeshot.ObjectManipulatorPartProperties = New devDept.Eyeshot.ObjectManipulatorPartProperties(System.Drawing.Color.Red, True, True)
            Dim ObjectManipulatorPartProperties3 As devDept.Eyeshot.ObjectManipulatorPartProperties = New devDept.Eyeshot.ObjectManipulatorPartProperties(System.Drawing.Color.Green, True, True)
            Dim ObjectManipulatorPartProperties4 As devDept.Eyeshot.ObjectManipulatorPartProperties = New devDept.Eyeshot.ObjectManipulatorPartProperties(System.Drawing.Color.Blue, True, True)
            Dim ObjectManipulatorPartProperties5 As devDept.Eyeshot.ObjectManipulatorPartProperties = New devDept.Eyeshot.ObjectManipulatorPartProperties(System.Drawing.Color.Red, True, True)
            Dim ObjectManipulatorPartProperties6 As devDept.Eyeshot.ObjectManipulatorPartProperties = New devDept.Eyeshot.ObjectManipulatorPartProperties(System.Drawing.Color.Green, True, True)
            Dim ObjectManipulatorPartProperties7 As devDept.Eyeshot.ObjectManipulatorPartProperties = New devDept.Eyeshot.ObjectManipulatorPartProperties(System.Drawing.Color.Blue, True, True)
            Dim ObjectManipulatorPartProperties8 As devDept.Eyeshot.ObjectManipulatorPartProperties = New devDept.Eyeshot.ObjectManipulatorPartProperties(System.Drawing.Color.Red, False, True)
            Dim ObjectManipulatorPartProperties9 As devDept.Eyeshot.ObjectManipulatorPartProperties = New devDept.Eyeshot.ObjectManipulatorPartProperties(System.Drawing.Color.Green, False, True)
            Dim ObjectManipulatorPartProperties10 As devDept.Eyeshot.ObjectManipulatorPartProperties = New devDept.Eyeshot.ObjectManipulatorPartProperties(System.Drawing.Color.Blue, False, True)
            Dim ObjectManipulator1 As devDept.Eyeshot.ObjectManipulator = New devDept.Eyeshot.ObjectManipulator(8, False, False, devDept.Eyeshot.ObjectManipulator.styleType.Standard, devDept.Eyeshot.ObjectManipulator.ballActionType.Translate, ObjectManipulatorPartProperties1, ObjectManipulatorPartProperties2, ObjectManipulatorPartProperties3, ObjectManipulatorPartProperties4, ObjectManipulatorPartProperties5, ObjectManipulatorPartProperties6, ObjectManipulatorPartProperties7, ObjectManipulatorPartProperties8, ObjectManipulatorPartProperties9, ObjectManipulatorPartProperties10, 0R, 0R, 0R, Drawing.Color.Transparent, Drawing.Color.Black, False)
            Dim CancelToolBarButton1 As devDept.Eyeshot.CancelToolBarButton = New devDept.Eyeshot.CancelToolBarButton("Cancel", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
            Dim ProgressBar1 As devDept.Eyeshot.ProgressBar = New devDept.Eyeshot.ProgressBar(devDept.Eyeshot.ProgressBar.styleType.Linear, 0, "Idle", System.Drawing.Color.Black, System.Drawing.Color.Transparent, System.Drawing.Color.Green, 1.0R, True, CancelToolBarButton1, True, 0.1R, 0.333R, True)
            Dim DisplayModeSettingsRendered1 As devDept.Eyeshot.DisplayModeSettingsRendered = New devDept.Eyeshot.DisplayModeSettingsRendered(False, devDept.Eyeshot.edgeColorMethodType.EntityColor, System.Drawing.Color.Black, 1.0!, 0!, devDept.Eyeshot.silhouettesDrawingType.LastFrame, False, devDept.Graphics.shadowType.None, Nothing, True, False, 0.3!, devDept.Graphics.realisticShadowQualityType.High)
            Dim BackgroundSettings1 As devDept.Graphics.BackgroundSettings = New devDept.Graphics.BackgroundSettings(devDept.Graphics.backgroundStyleType.LinearGradient, System.Drawing.Color.White, System.Drawing.Color.DodgerBlue, System.Drawing.Color.White, 0.75R, Nothing, devDept.Graphics.colorThemeType.[Auto], 0.3R)
            Dim Camera1 As devDept.Eyeshot.Camera = New devDept.Eyeshot.Camera(New devDept.Geometry.Point3D(-22.100165100440847R, 38.278608809624359R, 144.78791400459872R), 600.0R, New devDept.Geometry.Quaternion(0.12940952255126034R, 0.22414386804201339R, 0.4829629131445341R, 0.83651630373780794R), devDept.Graphics.projectionType.Perspective, 150.0R, 300195.70485643693R, False, 0.01R)
            Dim HomeToolBarButton1 As devDept.Eyeshot.HomeToolBarButton = New devDept.Eyeshot.HomeToolBarButton("Home (resets transformation in Adjust mode)", devDept.Eyeshot.ToolBarButton.styleType.PushButton, True, True)
            Dim MagnifyingGlassToolBarButton1 As devDept.Eyeshot.MagnifyingGlassToolBarButton = New devDept.Eyeshot.MagnifyingGlassToolBarButton("Magnifying Glass [M]", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
            Dim ZoomWindowToolBarButton1 As devDept.Eyeshot.ZoomWindowToolBarButton = New devDept.Eyeshot.ZoomWindowToolBarButton("Zoom Window", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
            Dim ZoomToolBarButton1 As devDept.Eyeshot.ZoomToolBarButton = New devDept.Eyeshot.ZoomToolBarButton("Zoom [Z]", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
            Dim PanToolBarButton1 As devDept.Eyeshot.PanToolBarButton = New devDept.Eyeshot.PanToolBarButton("Pan [P]", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
            Dim RotateToolBarButton1 As devDept.Eyeshot.RotateToolBarButton = New devDept.Eyeshot.RotateToolBarButton("Rotate [R]", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
            Dim ZoomFitToolBarButton1 As devDept.Eyeshot.ZoomFitToolBarButton = New devDept.Eyeshot.ZoomFitToolBarButton("Zoom Fit [F]", devDept.Eyeshot.ToolBarButton.styleType.PushButton, True, True)
            Dim ToolBarButton1 As devDept.Eyeshot.ToolBarButton = New devDept.Eyeshot.ToolBarButton(Global.Zuken.E3.HarnessAnalyzer.My.Resources.Resources.CalibrationMark, "ToggleAdjustMode", "Toggle adjust Mode", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True, Nothing, Nothing)
            Dim ToolBarButton2 As devDept.Eyeshot.ToolBarButton = New devDept.Eyeshot.ToolBarButton(Global.Zuken.E3.HarnessAnalyzer.My.Resources.Resources.CarModel, "LoadCarModel", "Load car model", devDept.Eyeshot.ToolBarButton.styleType.PushButton, True, True, Nothing, Nothing)
            Dim ToolBarButton3 As devDept.Eyeshot.ToolBarButton = New devDept.Eyeshot.ToolBarButton(Global.Zuken.E3.HarnessAnalyzer.My.Resources.Resources.TransparencyCar, "ChangeCarTransparency", "Change car transparency [T]", devDept.Eyeshot.ToolBarButton.styleType.PushButton, True, True, Nothing, Nothing)
            Dim ToolBarButton4 As devDept.Eyeshot.ToolBarButton = New devDept.Eyeshot.ToolBarButton(Global.Zuken.E3.HarnessAnalyzer.My.Resources.Resources.TransparencyJT, "ChangeJTTransparency", "Change JT transparency [J]", devDept.Eyeshot.ToolBarButton.styleType.PushButton, True, True, Nothing, Nothing)
            Dim ToolBarButton5 As devDept.Eyeshot.ToolBarButton = New devDept.Eyeshot.ToolBarButton(Global.Zuken.E3.HarnessAnalyzer.My.Resources.Resources.Sync3D, "SynchronizeSelection", "Sync selection", devDept.Eyeshot.ToolBarButton.styleType.PushButton, True, False, Nothing, Nothing)
            Dim ToolBar1 As devDept.Eyeshot.ToolBar = New devDept.Eyeshot.ToolBar(devDept.Eyeshot.ToolBar.positionType.VerticalMiddleLeft, True, New devDept.Eyeshot.ToolBarButton() {CType(HomeToolBarButton1, devDept.Eyeshot.ToolBarButton), CType(MagnifyingGlassToolBarButton1, devDept.Eyeshot.ToolBarButton), CType(ZoomWindowToolBarButton1, devDept.Eyeshot.ToolBarButton), CType(ZoomToolBarButton1, devDept.Eyeshot.ToolBarButton), CType(PanToolBarButton1, devDept.Eyeshot.ToolBarButton), CType(RotateToolBarButton1, devDept.Eyeshot.ToolBarButton), CType(ZoomFitToolBarButton1, devDept.Eyeshot.ToolBarButton), ToolBarButton1, ToolBarButton2, ToolBarButton3, ToolBarButton4, ToolBarButton5})
            Dim Grid1 As devDept.Eyeshot.Grid = New devDept.Eyeshot.Grid(New devDept.Geometry.Point3D(-100.0R, -100.0R, 0R), New devDept.Geometry.Point3D(100.0R, 100.0R, 0R), 5.0R, New devDept.Geometry.Plane(New devDept.Geometry.Point3D(0R, 0R, 0R), New devDept.Geometry.Vector3D(0R, 0R, 1.0R)), System.Drawing.Color.FromArgb(CType(CType(127, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(127, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(127, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer)), False, True, False, False, 10, 100, 5, System.Drawing.Color.FromArgb(CType(CType(127, Byte), Integer), CType(CType(90, Byte), Integer), CType(CType(90, Byte), Integer), CType(CType(90, Byte), Integer)), System.Drawing.Color.Transparent, False, System.Drawing.Color.FromArgb(CType(CType(12, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(255, Byte), Integer)))
            Dim OriginSymbol1 As devDept.Eyeshot.OriginSymbol = New devDept.Eyeshot.OriginSymbol(10, devDept.Eyeshot.originSymbolStyleType.Ball, New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte)), System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.Red, System.Drawing.Color.Green, System.Drawing.Color.Blue, "Origin", "X", "Y", "Z", True, Nothing, False)
            Dim RotateSettings1 As devDept.Eyeshot.RotateSettings = New devDept.Eyeshot.RotateSettings(New devDept.Eyeshot.MouseButton(devDept.Eyeshot.mouseButtonsZPR.Left, devDept.Eyeshot.modifierKeys.Shift), 15.0R, True, 1.0R, devDept.Eyeshot.rotationType.Trackball, devDept.Eyeshot.rotationCenterType.ViewportCenter, New devDept.Geometry.Point3D(0R, 0R, 0R), False)
            Dim ZoomSettings1 As devDept.Eyeshot.ZoomSettings = New devDept.Eyeshot.ZoomSettings(New devDept.Eyeshot.MouseButton(devDept.Eyeshot.mouseButtonsZPR.Middle, devDept.Eyeshot.modifierKeys.None), 25, True, devDept.Eyeshot.zoomStyleType.AtCursorLocation, False, 1.0R, System.Drawing.Color.Empty, devDept.Eyeshot.Camera.perspectiveFitType.Accurate, False, 10, True)
            Dim PanSettings1 As devDept.Eyeshot.PanSettings = New devDept.Eyeshot.PanSettings(New devDept.Eyeshot.MouseButton(devDept.Eyeshot.mouseButtonsZPR.Middle, devDept.Eyeshot.modifierKeys.None), 25, True)
            Dim NavigationSettings1 As devDept.Eyeshot.NavigationSettings = New devDept.Eyeshot.NavigationSettings(devDept.Eyeshot.Camera.navigationType.Examine, New devDept.Eyeshot.MouseButton(devDept.Eyeshot.mouseButtonsZPR.Left, devDept.Eyeshot.modifierKeys.None), New devDept.Geometry.Point3D(-1000.0R, -1000.0R, -1000.0R), New devDept.Geometry.Point3D(1000.0R, 1000.0R, 1000.0R), 8.0R, 50.0R, 50.0R)
            Dim SavedViewsManager1 As devDept.Eyeshot.Viewport.SavedViewsManager = New devDept.Eyeshot.Viewport.SavedViewsManager(8)
            Dim Viewport1 As devDept.Eyeshot.Viewport = New devDept.Eyeshot.Viewport(New System.Drawing.Point(0, 0), New System.Drawing.Size(455, 277), BackgroundSettings1, Camera1, New devDept.Eyeshot.ToolBar() {ToolBar1}, devDept.Eyeshot.displayType.Rendered, False, False, False, False, New devDept.Eyeshot.Grid() {Grid1}, New devDept.Eyeshot.OriginSymbol() {OriginSymbol1}, False, RotateSettings1, ZoomSettings1, PanSettings1, NavigationSettings1, SavedViewsManager1, devDept.Eyeshot.viewType.Trimetric)
            Dim CoordinateSystemIcon1 As devDept.Eyeshot.CoordinateSystemIcon = New devDept.Eyeshot.CoordinateSystemIcon(New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte)), System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.FromArgb(CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer)), System.Drawing.Color.OrangeRed, "Origin", "X", "Y", "Z", True, devDept.Eyeshot.coordinateSystemPositionType.BottomLeft, 37, Nothing, False)
            Dim ViewCubeIcon1 As devDept.Eyeshot.ViewCubeIcon = New devDept.Eyeshot.ViewCubeIcon(devDept.Eyeshot.coordinateSystemPositionType.TopRight, True, System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(20, Byte), Integer), CType(CType(60, Byte), Integer)), True, "Links", "Rechts", "Vorne", "Hinten", "Oben", "Unten", System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), Global.Microsoft.VisualBasic.ChrW(83), Global.Microsoft.VisualBasic.ChrW(78), Global.Microsoft.VisualBasic.ChrW(87), Global.Microsoft.VisualBasic.ChrW(69), True, Nothing, System.Drawing.Color.White, System.Drawing.Color.Black, 120, True, True, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, False, New devDept.Geometry.Quaternion(0R, 0R, 0R, 1.0R), True)
            Dim BackgroundSettings2 As devDept.Graphics.BackgroundSettings = New devDept.Graphics.BackgroundSettings(devDept.Graphics.backgroundStyleType.Solid, System.Drawing.Color.White, System.Drawing.Color.DodgerBlue, System.Drawing.Color.White, 0.75R, Nothing, devDept.Graphics.colorThemeType.[Auto], 0.3R)
            Dim Camera2 As devDept.Eyeshot.Camera = New devDept.Eyeshot.Camera(New devDept.Geometry.Point3D(0R, 0R, 50.0R), 600.0R, New devDept.Geometry.Quaternion(0.49999999999999989R, 0.5R, 0.5R, 0.50000000000000011R), devDept.Graphics.projectionType.Perspective, 50.0R, 84801.058188922441R, False, 0.001R)
            Dim HomeToolBarButton2 As devDept.Eyeshot.HomeToolBarButton = New devDept.Eyeshot.HomeToolBarButton("Home (resets transformation in Adjust mode)", devDept.Eyeshot.ToolBarButton.styleType.PushButton, True, True)
            Dim MagnifyingGlassToolBarButton2 As devDept.Eyeshot.MagnifyingGlassToolBarButton = New devDept.Eyeshot.MagnifyingGlassToolBarButton("Magnifying Glass", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
            Dim ZoomWindowToolBarButton2 As devDept.Eyeshot.ZoomWindowToolBarButton = New devDept.Eyeshot.ZoomWindowToolBarButton("Zoom Window", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
            Dim ZoomToolBarButton2 As devDept.Eyeshot.ZoomToolBarButton = New devDept.Eyeshot.ZoomToolBarButton("Zoom", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
            Dim PanToolBarButton2 As devDept.Eyeshot.PanToolBarButton = New devDept.Eyeshot.PanToolBarButton("Pan", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
            Dim RotateToolBarButton2 As devDept.Eyeshot.RotateToolBarButton = New devDept.Eyeshot.RotateToolBarButton("Rotate", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
            Dim ZoomFitToolBarButton2 As devDept.Eyeshot.ZoomFitToolBarButton = New devDept.Eyeshot.ZoomFitToolBarButton("Zoom Fit", devDept.Eyeshot.ToolBarButton.styleType.PushButton, True, True)
            Dim ToolBar2 As devDept.Eyeshot.ToolBar = New devDept.Eyeshot.ToolBar(devDept.Eyeshot.ToolBar.positionType.VerticalMiddleRight, False, New devDept.Eyeshot.ToolBarButton() {CType(HomeToolBarButton2, devDept.Eyeshot.ToolBarButton), CType(MagnifyingGlassToolBarButton2, devDept.Eyeshot.ToolBarButton), CType(ZoomWindowToolBarButton2, devDept.Eyeshot.ToolBarButton), CType(ZoomToolBarButton2, devDept.Eyeshot.ToolBarButton), CType(PanToolBarButton2, devDept.Eyeshot.ToolBarButton), CType(RotateToolBarButton2, devDept.Eyeshot.ToolBarButton), CType(ZoomFitToolBarButton2, devDept.Eyeshot.ToolBarButton)})
            Dim Grid2 As devDept.Eyeshot.Grid = New devDept.Eyeshot.Grid(New devDept.Geometry.Point3D(-100.0R, -100.0R, 0R), New devDept.Geometry.Point3D(100.0R, 100.0R, 0R), 1.0R, New devDept.Geometry.Plane(New devDept.Geometry.Point3D(0R, 0R, 0R), New devDept.Geometry.Vector3D(0R, 0R, 1.0R)), System.Drawing.Color.FromArgb(CType(CType(127, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(127, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(127, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer)), False, True, False, False, 10, 100, 10, System.Drawing.Color.FromArgb(CType(CType(127, Byte), Integer), CType(CType(90, Byte), Integer), CType(CType(90, Byte), Integer), CType(CType(90, Byte), Integer)), System.Drawing.Color.Transparent, True, System.Drawing.Color.FromArgb(CType(CType(12, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(255, Byte), Integer)))
            Dim OriginSymbol2 As devDept.Eyeshot.OriginSymbol = New devDept.Eyeshot.OriginSymbol(10, devDept.Eyeshot.originSymbolStyleType.Ball, New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte)), System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.Red, System.Drawing.Color.Green, System.Drawing.Color.Blue, "Origin", "X", "Y", "Z", True, Nothing, False)
            Dim RotateSettings2 As devDept.Eyeshot.RotateSettings = New devDept.Eyeshot.RotateSettings(New devDept.Eyeshot.MouseButton(devDept.Eyeshot.mouseButtonsZPR.None, devDept.Eyeshot.modifierKeys.None), 15.0R, True, 1.0R, devDept.Eyeshot.rotationType.Trackball, devDept.Eyeshot.rotationCenterType.ViewportCenter, New devDept.Geometry.Point3D(0R, 0R, 0R), False)
            Dim ZoomSettings2 As devDept.Eyeshot.ZoomSettings = New devDept.Eyeshot.ZoomSettings(New devDept.Eyeshot.MouseButton(devDept.Eyeshot.mouseButtonsZPR.Middle, devDept.Eyeshot.modifierKeys.None), 25, True, devDept.Eyeshot.zoomStyleType.AtCursorLocation, False, 1.0R, System.Drawing.Color.Empty, devDept.Eyeshot.Camera.perspectiveFitType.Accurate, False, 10, True)
            Dim PanSettings2 As devDept.Eyeshot.PanSettings = New devDept.Eyeshot.PanSettings(New devDept.Eyeshot.MouseButton(devDept.Eyeshot.mouseButtonsZPR.Middle, devDept.Eyeshot.modifierKeys.None), 25, True)
            Dim NavigationSettings2 As devDept.Eyeshot.NavigationSettings = New devDept.Eyeshot.NavigationSettings(devDept.Eyeshot.Camera.navigationType.Examine, New devDept.Eyeshot.MouseButton(devDept.Eyeshot.mouseButtonsZPR.Left, devDept.Eyeshot.modifierKeys.None), New devDept.Geometry.Point3D(-1000.0R, -1000.0R, -1000.0R), New devDept.Geometry.Point3D(1000.0R, 1000.0R, 1000.0R), 8.0R, 50.0R, 50.0R)
            Dim SavedViewsManager2 As devDept.Eyeshot.Viewport.SavedViewsManager = New devDept.Eyeshot.Viewport.SavedViewsManager(8)
            Dim Viewport2 As devDept.Eyeshot.Viewport = New devDept.Eyeshot.Viewport(New System.Drawing.Point(459, 0), New System.Drawing.Size(456, 277), BackgroundSettings2, Camera2, New devDept.Eyeshot.ToolBar() {ToolBar2}, devDept.Eyeshot.displayType.Rendered, False, False, False, False, New devDept.Eyeshot.Grid() {Grid2}, New devDept.Eyeshot.OriginSymbol() {OriginSymbol2}, False, RotateSettings2, ZoomSettings2, PanSettings2, NavigationSettings2, SavedViewsManager2, devDept.Eyeshot.viewType.Top)
            Dim CoordinateSystemIcon2 As devDept.Eyeshot.CoordinateSystemIcon = New devDept.Eyeshot.CoordinateSystemIcon(New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte)), System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.FromArgb(CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer)), System.Drawing.Color.OrangeRed, "Origin", "X", "Y", "Z", True, devDept.Eyeshot.coordinateSystemPositionType.BottomLeft, 37, Nothing, False)
            Dim ViewCubeIcon2 As devDept.Eyeshot.ViewCubeIcon = New devDept.Eyeshot.ViewCubeIcon(devDept.Eyeshot.coordinateSystemPositionType.TopRight, True, System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(20, Byte), Integer), CType(CType(60, Byte), Integer)), True, "Links", "Rechts", "Vorne", "Hinten", "Oben", "Unten", System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), Global.Microsoft.VisualBasic.ChrW(83), Global.Microsoft.VisualBasic.ChrW(78), Global.Microsoft.VisualBasic.ChrW(87), Global.Microsoft.VisualBasic.ChrW(69), True, Nothing, System.Drawing.Color.White, System.Drawing.Color.Black, 120, True, True, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, False, New devDept.Geometry.Quaternion(0R, 0R, 0R, 1.0R), True)
            Dim BackgroundSettings3 As devDept.Graphics.BackgroundSettings = New devDept.Graphics.BackgroundSettings(devDept.Graphics.backgroundStyleType.Solid, System.Drawing.Color.White, System.Drawing.Color.DodgerBlue, System.Drawing.Color.White, 0.75R, Nothing, devDept.Graphics.colorThemeType.[Auto], 0.3R)
            Dim Camera3 As devDept.Eyeshot.Camera = New devDept.Eyeshot.Camera(New devDept.Geometry.Point3D(0R, 0R, 50.0R), 600.0R, New devDept.Geometry.Quaternion(0R, 0R, 0R, 1.0R), devDept.Graphics.projectionType.Perspective, 50.0R, 84801.058188922441R, False, 0.001R)
            Dim ToolBarButton6 As devDept.Eyeshot.ToolBarButton = New devDept.Eyeshot.ToolBarButton(Global.Zuken.E3.HarnessAnalyzer.My.Resources.Resources.Sync3D, "SynchronizeSelection", "Sync selection", devDept.Eyeshot.ToolBarButton.styleType.PushButton, True, False, Nothing, Nothing)
            Dim HomeToolBarButton3 As devDept.Eyeshot.HomeToolBarButton = New devDept.Eyeshot.HomeToolBarButton("Home (resets transformation in Adjust mode)", devDept.Eyeshot.ToolBarButton.styleType.PushButton, True, True)
            Dim MagnifyingGlassToolBarButton3 As devDept.Eyeshot.MagnifyingGlassToolBarButton = New devDept.Eyeshot.MagnifyingGlassToolBarButton("Magnifying Glass", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
            Dim ZoomWindowToolBarButton3 As devDept.Eyeshot.ZoomWindowToolBarButton = New devDept.Eyeshot.ZoomWindowToolBarButton("Zoom Window", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
            Dim ZoomToolBarButton3 As devDept.Eyeshot.ZoomToolBarButton = New devDept.Eyeshot.ZoomToolBarButton("Zoom", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
            Dim PanToolBarButton3 As devDept.Eyeshot.PanToolBarButton = New devDept.Eyeshot.PanToolBarButton("Pan", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
            Dim RotateToolBarButton3 As devDept.Eyeshot.RotateToolBarButton = New devDept.Eyeshot.RotateToolBarButton("Rotate", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
            Dim ZoomFitToolBarButton3 As devDept.Eyeshot.ZoomFitToolBarButton = New devDept.Eyeshot.ZoomFitToolBarButton("Zoom Fit", devDept.Eyeshot.ToolBarButton.styleType.PushButton, True, True)
            Dim ToolBar3 As devDept.Eyeshot.ToolBar = New devDept.Eyeshot.ToolBar(devDept.Eyeshot.ToolBar.positionType.VerticalMiddleRight, False, New devDept.Eyeshot.ToolBarButton() {ToolBarButton6, CType(HomeToolBarButton3, devDept.Eyeshot.ToolBarButton), CType(MagnifyingGlassToolBarButton3, devDept.Eyeshot.ToolBarButton), CType(ZoomWindowToolBarButton3, devDept.Eyeshot.ToolBarButton), CType(ZoomToolBarButton3, devDept.Eyeshot.ToolBarButton), CType(PanToolBarButton3, devDept.Eyeshot.ToolBarButton), CType(RotateToolBarButton3, devDept.Eyeshot.ToolBarButton), CType(ZoomFitToolBarButton3, devDept.Eyeshot.ToolBarButton)})
            Dim Grid3 As devDept.Eyeshot.Grid = New devDept.Eyeshot.Grid(New devDept.Geometry.Point3D(-100.0R, -100.0R, 0R), New devDept.Geometry.Point3D(100.0R, 100.0R, 0R), 1.0R, New devDept.Geometry.Plane(New devDept.Geometry.Point3D(0R, 0R, 0R), New devDept.Geometry.Vector3D(0R, 0R, 1.0R)), System.Drawing.Color.FromArgb(CType(CType(127, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(127, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(127, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer)), False, True, False, False, 10, 100, 10, System.Drawing.Color.FromArgb(CType(CType(127, Byte), Integer), CType(CType(90, Byte), Integer), CType(CType(90, Byte), Integer), CType(CType(90, Byte), Integer)), System.Drawing.Color.Transparent, True, System.Drawing.Color.FromArgb(CType(CType(12, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(255, Byte), Integer)))
            Dim OriginSymbol3 As devDept.Eyeshot.OriginSymbol = New devDept.Eyeshot.OriginSymbol(10, devDept.Eyeshot.originSymbolStyleType.Ball, New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte)), System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.Red, System.Drawing.Color.Green, System.Drawing.Color.Blue, "Origin", "X", "Y", "Z", True, Nothing, False)
            Dim RotateSettings3 As devDept.Eyeshot.RotateSettings = New devDept.Eyeshot.RotateSettings(New devDept.Eyeshot.MouseButton(devDept.Eyeshot.mouseButtonsZPR.None, devDept.Eyeshot.modifierKeys.None), 15.0R, True, 1.0R, devDept.Eyeshot.rotationType.Trackball, devDept.Eyeshot.rotationCenterType.ViewportCenter, New devDept.Geometry.Point3D(0R, 0R, 0R), False)
            Dim ZoomSettings3 As devDept.Eyeshot.ZoomSettings = New devDept.Eyeshot.ZoomSettings(New devDept.Eyeshot.MouseButton(devDept.Eyeshot.mouseButtonsZPR.Middle, devDept.Eyeshot.modifierKeys.None), 25, True, devDept.Eyeshot.zoomStyleType.AtCursorLocation, False, 1.0R, System.Drawing.Color.Empty, devDept.Eyeshot.Camera.perspectiveFitType.Accurate, False, 10, True)
            Dim PanSettings3 As devDept.Eyeshot.PanSettings = New devDept.Eyeshot.PanSettings(New devDept.Eyeshot.MouseButton(devDept.Eyeshot.mouseButtonsZPR.Middle, devDept.Eyeshot.modifierKeys.None), 25, True)
            Dim NavigationSettings3 As devDept.Eyeshot.NavigationSettings = New devDept.Eyeshot.NavigationSettings(devDept.Eyeshot.Camera.navigationType.Examine, New devDept.Eyeshot.MouseButton(devDept.Eyeshot.mouseButtonsZPR.Left, devDept.Eyeshot.modifierKeys.None), New devDept.Geometry.Point3D(-1000.0R, -1000.0R, -1000.0R), New devDept.Geometry.Point3D(1000.0R, 1000.0R, 1000.0R), 8.0R, 50.0R, 50.0R)
            Dim SavedViewsManager3 As devDept.Eyeshot.Viewport.SavedViewsManager = New devDept.Eyeshot.Viewport.SavedViewsManager(8)
            Dim Viewport3 As devDept.Eyeshot.Viewport = New devDept.Eyeshot.Viewport(New System.Drawing.Point(0, 281), New System.Drawing.Size(455, 278), BackgroundSettings3, Camera3, New devDept.Eyeshot.ToolBar() {ToolBar3}, devDept.Eyeshot.displayType.Rendered, False, False, False, False, New devDept.Eyeshot.Grid() {Grid3}, New devDept.Eyeshot.OriginSymbol() {OriginSymbol3}, False, RotateSettings3, ZoomSettings3, PanSettings3, NavigationSettings3, SavedViewsManager3, devDept.Eyeshot.viewType.Right)
            Dim CoordinateSystemIcon3 As devDept.Eyeshot.CoordinateSystemIcon = New devDept.Eyeshot.CoordinateSystemIcon(New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte)), System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.FromArgb(CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer)), System.Drawing.Color.OrangeRed, "Origin", "X", "Y", "Z", True, devDept.Eyeshot.coordinateSystemPositionType.BottomLeft, 37, Nothing, False)
            Dim ViewCubeIcon3 As devDept.Eyeshot.ViewCubeIcon = New devDept.Eyeshot.ViewCubeIcon(devDept.Eyeshot.coordinateSystemPositionType.TopRight, True, System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(20, Byte), Integer), CType(CType(60, Byte), Integer)), True, "Links", "Rechts", "Vorne", "Hinten", "Oben", "Unten", System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), Global.Microsoft.VisualBasic.ChrW(83), Global.Microsoft.VisualBasic.ChrW(78), Global.Microsoft.VisualBasic.ChrW(87), Global.Microsoft.VisualBasic.ChrW(69), True, Nothing, System.Drawing.Color.White, System.Drawing.Color.Black, 120, True, True, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, False, New devDept.Geometry.Quaternion(0R, 0R, 0R, 1.0R), True)
            Dim BackgroundSettings4 As devDept.Graphics.BackgroundSettings = New devDept.Graphics.BackgroundSettings(devDept.Graphics.backgroundStyleType.Solid, System.Drawing.Color.White, System.Drawing.Color.DodgerBlue, System.Drawing.Color.White, 0.75R, Nothing, devDept.Graphics.colorThemeType.[Auto], 0.3R)
            Dim Camera4 As devDept.Eyeshot.Camera = New devDept.Eyeshot.Camera(New devDept.Geometry.Point3D(0R, 0R, 50.0R), 600.0R, New devDept.Geometry.Quaternion(0R, 0R, 0.70710678118654746R, 0.70710678118654757R), devDept.Graphics.projectionType.Perspective, 50.0R, 84801.058188922441R, False, 0.001R)
            Dim HomeToolBarButton4 As devDept.Eyeshot.HomeToolBarButton = New devDept.Eyeshot.HomeToolBarButton("Home (resets transformation in Adjust mode)", devDept.Eyeshot.ToolBarButton.styleType.PushButton, True, True)
            Dim MagnifyingGlassToolBarButton4 As devDept.Eyeshot.MagnifyingGlassToolBarButton = New devDept.Eyeshot.MagnifyingGlassToolBarButton("Magnifying Glass", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
            Dim ZoomWindowToolBarButton4 As devDept.Eyeshot.ZoomWindowToolBarButton = New devDept.Eyeshot.ZoomWindowToolBarButton("Zoom Window", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
            Dim ZoomToolBarButton4 As devDept.Eyeshot.ZoomToolBarButton = New devDept.Eyeshot.ZoomToolBarButton("Zoom", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
            Dim PanToolBarButton4 As devDept.Eyeshot.PanToolBarButton = New devDept.Eyeshot.PanToolBarButton("Pan", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
            Dim RotateToolBarButton4 As devDept.Eyeshot.RotateToolBarButton = New devDept.Eyeshot.RotateToolBarButton("Rotate", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
            Dim ZoomFitToolBarButton4 As devDept.Eyeshot.ZoomFitToolBarButton = New devDept.Eyeshot.ZoomFitToolBarButton("Zoom Fit", devDept.Eyeshot.ToolBarButton.styleType.PushButton, True, True)
            Dim ToolBar4 As devDept.Eyeshot.ToolBar = New devDept.Eyeshot.ToolBar(devDept.Eyeshot.ToolBar.positionType.VerticalMiddleRight, False, New devDept.Eyeshot.ToolBarButton() {CType(HomeToolBarButton4, devDept.Eyeshot.ToolBarButton), CType(MagnifyingGlassToolBarButton4, devDept.Eyeshot.ToolBarButton), CType(ZoomWindowToolBarButton4, devDept.Eyeshot.ToolBarButton), CType(ZoomToolBarButton4, devDept.Eyeshot.ToolBarButton), CType(PanToolBarButton4, devDept.Eyeshot.ToolBarButton), CType(RotateToolBarButton4, devDept.Eyeshot.ToolBarButton), CType(ZoomFitToolBarButton4, devDept.Eyeshot.ToolBarButton)})
            Dim Grid4 As devDept.Eyeshot.Grid = New devDept.Eyeshot.Grid(New devDept.Geometry.Point3D(-100.0R, -100.0R, 0R), New devDept.Geometry.Point3D(100.0R, 100.0R, 0R), 1.0R, New devDept.Geometry.Plane(New devDept.Geometry.Point3D(0R, 0R, 0R), New devDept.Geometry.Vector3D(0R, 0R, 1.0R)), System.Drawing.Color.FromArgb(CType(CType(127, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(127, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(127, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer)), False, True, False, False, 10, 100, 10, System.Drawing.Color.FromArgb(CType(CType(127, Byte), Integer), CType(CType(90, Byte), Integer), CType(CType(90, Byte), Integer), CType(CType(90, Byte), Integer)), System.Drawing.Color.Transparent, True, System.Drawing.Color.FromArgb(CType(CType(12, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(255, Byte), Integer)))
            Dim OriginSymbol4 As devDept.Eyeshot.OriginSymbol = New devDept.Eyeshot.OriginSymbol(10, devDept.Eyeshot.originSymbolStyleType.Ball, New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte)), System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.Red, System.Drawing.Color.Green, System.Drawing.Color.Blue, "Origin", "X", "Y", "Z", True, Nothing, False)
            Dim RotateSettings4 As devDept.Eyeshot.RotateSettings = New devDept.Eyeshot.RotateSettings(New devDept.Eyeshot.MouseButton(devDept.Eyeshot.mouseButtonsZPR.None, devDept.Eyeshot.modifierKeys.None), 15.0R, True, 1.0R, devDept.Eyeshot.rotationType.Trackball, devDept.Eyeshot.rotationCenterType.ViewportCenter, New devDept.Geometry.Point3D(0R, 0R, 0R), False)
            Dim ZoomSettings4 As devDept.Eyeshot.ZoomSettings = New devDept.Eyeshot.ZoomSettings(New devDept.Eyeshot.MouseButton(devDept.Eyeshot.mouseButtonsZPR.Middle, devDept.Eyeshot.modifierKeys.None), 25, True, devDept.Eyeshot.zoomStyleType.AtCursorLocation, False, 1.0R, System.Drawing.Color.Empty, devDept.Eyeshot.Camera.perspectiveFitType.Accurate, False, 10, True)
            Dim PanSettings4 As devDept.Eyeshot.PanSettings = New devDept.Eyeshot.PanSettings(New devDept.Eyeshot.MouseButton(devDept.Eyeshot.mouseButtonsZPR.Middle, devDept.Eyeshot.modifierKeys.None), 25, True)
            Dim NavigationSettings4 As devDept.Eyeshot.NavigationSettings = New devDept.Eyeshot.NavigationSettings(devDept.Eyeshot.Camera.navigationType.Examine, New devDept.Eyeshot.MouseButton(devDept.Eyeshot.mouseButtonsZPR.Left, devDept.Eyeshot.modifierKeys.None), New devDept.Geometry.Point3D(-1000.0R, -1000.0R, -1000.0R), New devDept.Geometry.Point3D(1000.0R, 1000.0R, 1000.0R), 8.0R, 50.0R, 50.0R)
            Dim SavedViewsManager4 As devDept.Eyeshot.Viewport.SavedViewsManager = New devDept.Eyeshot.Viewport.SavedViewsManager(8)
            Dim Viewport4 As devDept.Eyeshot.Viewport = New devDept.Eyeshot.Viewport(New System.Drawing.Point(459, 281), New System.Drawing.Size(456, 278), BackgroundSettings4, Camera4, New devDept.Eyeshot.ToolBar() {ToolBar4}, devDept.Eyeshot.displayType.Rendered, False, False, False, False, New devDept.Eyeshot.Grid() {Grid4}, New devDept.Eyeshot.OriginSymbol() {OriginSymbol4}, False, RotateSettings4, ZoomSettings4, PanSettings4, NavigationSettings4, SavedViewsManager4, devDept.Eyeshot.viewType.Front)
            Dim CoordinateSystemIcon4 As devDept.Eyeshot.CoordinateSystemIcon = New devDept.Eyeshot.CoordinateSystemIcon(New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte)), System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.FromArgb(CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer)), System.Drawing.Color.OrangeRed, "Origin", "X", "Y", "Z", True, devDept.Eyeshot.coordinateSystemPositionType.BottomLeft, 37, Nothing, False)
            Dim ViewCubeIcon4 As devDept.Eyeshot.ViewCubeIcon = New devDept.Eyeshot.ViewCubeIcon(devDept.Eyeshot.coordinateSystemPositionType.TopRight, True, System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(20, Byte), Integer), CType(CType(60, Byte), Integer)), True, "Links", "Rechts", "Vorne", "Hinten", "Oben", "Unten", System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), Global.Microsoft.VisualBasic.ChrW(83), Global.Microsoft.VisualBasic.ChrW(78), Global.Microsoft.VisualBasic.ChrW(87), Global.Microsoft.VisualBasic.ChrW(69), True, Nothing, System.Drawing.Color.White, System.Drawing.Color.Black, 120, True, True, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, False, New devDept.Geometry.Quaternion(0R, 0R, 0R, 1.0R), True)
            Me.UltraPanel1 = New Infragistics.Win.Misc.UltraPanel()
            Me.btnAutoAdjust = New Infragistics.Win.Misc.UltraButton()
            Me.cm_cntrl = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.LoadCarModelToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.TransparencyToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.AdjustmentautoToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.AdjustmentToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.FineAdjustmentToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.ListOfDocumentsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.FitSelectionToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.FitAllToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.SearchToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.UltraToolTipManager1 = New Infragistics.Win.UltraWinToolTip.UltraToolTipManager(Me.components)
            Me.UltraToolTipManager2 = New Infragistics.Win.UltraWinToolTip.UltraToolTipManager(Me.components)
            Me.uneRotY = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
            Me.uneRotZ = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
            Me.uneScalePercent = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
            Me.une_PosY = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
            Me.une_PosX = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
            Me.une_PosZ = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
            Me.uneRotX = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
            Me.Design3D = New Zuken.E3.HarnessAnalyzer.D3D.Consolidated.Designs.ConsolidatedDesign()
            Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
            Me.StatusTextLabel = New System.Windows.Forms.ToolStripStatusLabel()
            Me.StatusTextLabel2 = New System.Windows.Forms.ToolStripStatusLabel()
            Me.UpperTableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
            Me.grp_Position = New Infragistics.Win.Misc.UltraGroupBox()
            Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
            Me.Label5 = New System.Windows.Forms.Label()
            Me.Label4 = New System.Windows.Forms.Label()
            Me.Label6 = New System.Windows.Forms.Label()
            Me.grp_Rotation = New Infragistics.Win.Misc.UltraGroupBox()
            Me.TableLayoutPanel4 = New System.Windows.Forms.TableLayoutPanel()
            Me.Label3 = New System.Windows.Forms.Label()
            Me.Label2 = New System.Windows.Forms.Label()
            Me.Label1 = New System.Windows.Forms.Label()
            Me.Panel1 = New System.Windows.Forms.Panel()
            Me.grp_Scale = New Infragistics.Win.Misc.UltraGroupBox()
            Me.UltraTabbedMdiManager1 = New Infragistics.Win.UltraWinTabbedMdi.UltraTabbedMdiManager(Me.components)
            Me.CarModelsViewControl = New Zuken.E3.HarnessAnalyzer.D3D.Consolidated.Controls.CarModelsViewControl()
            Me.viewModelFilesPopUp = New Infragistics.Win.Misc.UltraPopupControlContainer(Me.components)
            Me.UltraPanel1.ClientArea.SuspendLayout()
            Me.UltraPanel1.SuspendLayout()
            Me.cm_cntrl.SuspendLayout()
            CType(Me.uneRotY, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.uneRotZ, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.uneScalePercent, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.une_PosY, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.une_PosX, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.une_PosZ, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.uneRotX, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.TableLayoutPanel1.SuspendLayout()
            CType(Me.Design3D, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.StatusStrip1.SuspendLayout()
            Me.UpperTableLayoutPanel.SuspendLayout()
            CType(Me.grp_Position, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.grp_Position.SuspendLayout()
            Me.TableLayoutPanel2.SuspendLayout()
            CType(Me.grp_Rotation, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.grp_Rotation.SuspendLayout()
            Me.TableLayoutPanel4.SuspendLayout()
            Me.Panel1.SuspendLayout()
            CType(Me.grp_Scale, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.grp_Scale.SuspendLayout()
            CType(Me.UltraTabbedMdiManager1, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            ' 
            ' UltraPanel1
            ' 
            ' 
            ' UltraPanel1.ClientArea
            ' 
            UltraPanel1.ClientArea.Controls.Add(btnAutoAdjust)
            resources.ApplyResources(UltraPanel1, "UltraPanel1")
            UltraPanel1.Name = "UltraPanel1"
            ' 
            ' btnAutoAdjust
            ' 
            resources.ApplyResources(btnAutoAdjust, "btnAutoAdjust")
            Appearance1.Image = resources.GetObject("Appearance1.Image")
            Appearance1.ImageHAlign = Infragistics.Win.HAlign.Center
            Appearance1.ImageVAlign = Infragistics.Win.VAlign.Middle
            btnAutoAdjust.Appearance = Appearance1
            btnAutoAdjust.Name = "btnAutoAdjust"
            ' 
            ' cm_cntrl
            ' 
            cm_cntrl.Items.AddRange(New ToolStripItem() {LoadCarModelToolStripMenuItem, TransparencyToolStripMenuItem, AdjustmentautoToolStripMenuItem, AdjustmentToolStripMenuItem, FineAdjustmentToolStripMenuItem, ListOfDocumentsToolStripMenuItem, FitSelectionToolStripMenuItem, FitAllToolStripMenuItem, SearchToolStripMenuItem})
            cm_cntrl.Name = "cm_cntrl"
            resources.ApplyResources(cm_cntrl, "cm_cntrl")
            ' 
            ' LoadCarModelToolStripMenuItem
            ' 
            LoadCarModelToolStripMenuItem.Name = "LoadCarModelToolStripMenuItem"
            resources.ApplyResources(LoadCarModelToolStripMenuItem, "LoadCarModelToolStripMenuItem")
            ' 
            ' TransparencyToolStripMenuItem
            ' 
            TransparencyToolStripMenuItem.Name = "TransparencyToolStripMenuItem"
            resources.ApplyResources(TransparencyToolStripMenuItem, "TransparencyToolStripMenuItem")
            ' 
            ' AdjustmentautoToolStripMenuItem
            ' 
            AdjustmentautoToolStripMenuItem.Name = "AdjustmentautoToolStripMenuItem"
            resources.ApplyResources(AdjustmentautoToolStripMenuItem, "AdjustmentautoToolStripMenuItem")
            ' 
            ' AdjustmentToolStripMenuItem
            ' 
            AdjustmentToolStripMenuItem.Name = "AdjustmentToolStripMenuItem"
            resources.ApplyResources(AdjustmentToolStripMenuItem, "AdjustmentToolStripMenuItem")
            ' 
            ' FineAdjustmentToolStripMenuItem
            ' 
            FineAdjustmentToolStripMenuItem.Name = "FineAdjustmentToolStripMenuItem"
            resources.ApplyResources(FineAdjustmentToolStripMenuItem, "FineAdjustmentToolStripMenuItem")
            ' 
            ' ListOfDocumentsToolStripMenuItem
            ' 
            ListOfDocumentsToolStripMenuItem.Name = "ListOfDocumentsToolStripMenuItem"
            resources.ApplyResources(ListOfDocumentsToolStripMenuItem, "ListOfDocumentsToolStripMenuItem")
            ' 
            ' FitSelectionToolStripMenuItem
            ' 
            FitSelectionToolStripMenuItem.Name = "FitSelectionToolStripMenuItem"
            resources.ApplyResources(FitSelectionToolStripMenuItem, "FitSelectionToolStripMenuItem")
            ' 
            ' FitAllToolStripMenuItem
            ' 
            FitAllToolStripMenuItem.Name = "FitAllToolStripMenuItem"
            resources.ApplyResources(FitAllToolStripMenuItem, "FitAllToolStripMenuItem")
            ' 
            ' SearchToolStripMenuItem
            ' 
            SearchToolStripMenuItem.Name = "SearchToolStripMenuItem"
            resources.ApplyResources(SearchToolStripMenuItem, "SearchToolStripMenuItem")
            ' 
            ' UltraToolTipManager1
            ' 
            UltraToolTipManager1.ContainingControl = Me
            UltraToolTipManager1.DisplayStyle = Infragistics.Win.ToolTipDisplayStyle.Standard
            UltraToolTipManager1.Enabled = False
            UltraToolTipManager1.ToolTipImage = Infragistics.Win.ToolTipImage.None
            ' 
            ' UltraToolTipManager2
            ' 
            UltraToolTipManager2.ContainingControl = Me
            UltraToolTipManager2.DisplayStyle = Infragistics.Win.ToolTipDisplayStyle.Standard
            UltraToolTipManager2.ToolTipImage = Infragistics.Win.ToolTipImage.None
            ' 
            ' uneRotY
            ' 
            resources.ApplyResources(uneRotY, "uneRotY")
            uneRotY.MaskInput = "{LOC}-nnn.nn"
            uneRotY.MaxValue = 360.0R
            uneRotY.MinValue = -360.0R
            uneRotY.Name = "uneRotY"
            uneRotY.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double
            uneRotY.PromptChar = " "c
            uneRotY.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Always
            uneRotY.SpinButtonIntervalSettings.AccelerationEnabled = True
            uneRotY.SpinIncrement = 1.0R
            ' 
            ' uneRotZ
            ' 
            resources.ApplyResources(uneRotZ, "uneRotZ")
            uneRotZ.MaskInput = "{LOC}-nnn.nn"
            uneRotZ.MaxValue = 360.0R
            uneRotZ.MinValue = -360.0R
            uneRotZ.Name = "uneRotZ"
            uneRotZ.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double
            uneRotZ.PromptChar = " "c
            uneRotZ.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Always
            uneRotZ.SpinButtonIntervalSettings.AccelerationEnabled = True
            uneRotZ.SpinIncrement = 1.0R
            ' 
            ' uneScalePercent
            ' 
            resources.ApplyResources(uneScalePercent, "uneScalePercent")
            uneScalePercent.MaskInput = "{LOC}-nnnnnnnnnn.nn %"
            uneScalePercent.MaxValue = 1.79769313486232E+307R
            uneScalePercent.MinValue = 1.0E-18R
            uneScalePercent.Name = "uneScalePercent"
            uneScalePercent.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double
            uneScalePercent.PromptChar = " "c
            uneScalePercent.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Always
            uneScalePercent.SpinButtonIntervalSettings.AccelerationEnabled = True
            uneScalePercent.SpinIncrement = 1.0R
            uneScalePercent.Value = 100.0R
            ' 
            ' une_PosY
            ' 
            resources.ApplyResources(une_PosY, "une_PosY")
            une_PosY.MaskInput = "{LOC}+nnnnnnnnnn.nn"
            une_PosY.MaxValue = 1.79769313486232E+307R
            une_PosY.MinValue = -1.79769313486232E+307R
            une_PosY.Name = "une_PosY"
            une_PosY.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double
            une_PosY.PromptChar = " "c
            une_PosY.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Always
            une_PosY.SpinButtonIntervalSettings.AccelerationEnabled = True
            une_PosY.SpinIncrement = 1.0R
            ' 
            ' une_PosX
            ' 
            resources.ApplyResources(une_PosX, "une_PosX")
            une_PosX.MaskInput = "{LOC}+nnnnnnnnnn.nn"
            une_PosX.MaxValue = 1.79769313486232E+307R
            une_PosX.MinValue = -1.79769313486232E+307R
            une_PosX.Name = "une_PosX"
            une_PosX.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double
            une_PosX.PromptChar = " "c
            une_PosX.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Always
            une_PosX.SpinButtonIntervalSettings.AccelerationEnabled = True
            une_PosX.SpinIncrement = 1.0R
            ' 
            ' une_PosZ
            ' 
            resources.ApplyResources(une_PosZ, "une_PosZ")
            une_PosZ.MaskInput = "{LOC}+nnnnnnnnnn.nn"
            une_PosZ.MaxValue = 1.79769313486232E+307R
            une_PosZ.MinValue = -1.79769313486232E+307R
            une_PosZ.Name = "une_PosZ"
            une_PosZ.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double
            une_PosZ.PromptChar = " "c
            une_PosZ.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Always
            une_PosZ.SpinButtonIntervalSettings.AccelerationEnabled = True
            une_PosZ.SpinIncrement = 1.0R
            ' 
            ' uneRotX
            ' 
            resources.ApplyResources(uneRotX, "uneRotX")
            uneRotX.MaskInput = "{LOC}-nnn.nn"
            uneRotX.MaxValue = 360.0R
            uneRotX.MinValue = -360.0R
            uneRotX.Name = "uneRotX"
            uneRotX.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double
            uneRotX.PromptChar = " "c
            uneRotX.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Always
            uneRotX.SpinButtonIntervalSettings.AccelerationEnabled = True
            uneRotX.SpinIncrement = 1.0R
            ' 
            ' TableLayoutPanel1
            ' 
            resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
            Me.TableLayoutPanel1.Controls.Add(Me.Design3D, 0, 1)
            Me.TableLayoutPanel1.Controls.Add(Me.StatusStrip1, 0, 2)
            Me.TableLayoutPanel1.Controls.Add(Me.UpperTableLayoutPanel, 0, 0)
            Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
            '
            'Model3D
            '
            Me.Design3D.AllowedSelectionCount = 2147483647
            Me.Design3D.AnimateCamera = True
            Me.Design3D.AntiAliasingSamples = devDept.Graphics.antialiasingSamplesNumberType.x2
            Me.Design3D.AutoDisableMode = devDept.Eyeshot.DisableSelectionMode.None
            Me.Design3D.BlinkSynchronizer = Nothing
            Me.Design3D.Cursor = System.Windows.Forms.Cursors.Default
            Me.Design3D.ShortcutKeys = ShortcutKeysSettings1
            resources.ApplyResources(Me.Design3D, "Model3D")
            Me.Design3D.EEModel = Nothing
            Me.Design3D.Name = "Model3D"
            ObjectManipulator1.InitialTransformation = CType(resources.GetObject("ObjectManipulator1.InitialTransformation"), devDept.Geometry.Transformation)
            ObjectManipulator1.LabelFont = Nothing
            Me.Design3D.ObjectManipulator = ObjectManipulator1
            Me.Design3D.PickBoxEnabled = False
            Me.Design3D.ProgressBar = ProgressBar1
            Me.Design3D.Rendered = DisplayModeSettingsRendered1
            Me.Design3D.Renderer = devDept.Eyeshot.rendererType.Direct3D

            Me.Design3D.Selection.Color = System.Drawing.Color.Magenta
            Me.Design3D.Selection.ColorDynamic = System.Drawing.Color.Magenta

            Me.Design3D.ShowSilhouetteDynamicPickEntities = devDept.Eyeshot.SilhouetteDynamicPickMode.None
            'Me.Model3D.Snapping = devDept.Eyeshot.Snapping.None
            OriginSymbol1.LabelFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            CoordinateSystemIcon1.LabelFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Viewport1.CoordinateSystemIcon = CoordinateSystemIcon1
            Viewport1.Legends = New devDept.Eyeshot.Legend(-1) {}
            ViewCubeIcon1.Font = Nothing
            ViewCubeIcon1.InitialRotation = New devDept.Geometry.Quaternion(0R, 0R, 0R, 1.0R)
            Viewport1.ViewCubeIcon = ViewCubeIcon1
            OriginSymbol2.LabelFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            CoordinateSystemIcon2.LabelFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Viewport2.CoordinateSystemIcon = CoordinateSystemIcon2
            Viewport2.Legends = New devDept.Eyeshot.Legend(-1) {}
            ViewCubeIcon2.Font = Nothing
            ViewCubeIcon2.InitialRotation = New devDept.Geometry.Quaternion(0R, 0R, 0R, 1.0R)
            Viewport2.ViewCubeIcon = ViewCubeIcon2
            OriginSymbol3.LabelFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            CoordinateSystemIcon3.LabelFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Viewport3.CoordinateSystemIcon = CoordinateSystemIcon3
            Viewport3.Legends = New devDept.Eyeshot.Legend(-1) {}
            ViewCubeIcon3.Font = Nothing
            ViewCubeIcon3.InitialRotation = New devDept.Geometry.Quaternion(0R, 0R, 0R, 1.0R)
            Viewport3.ViewCubeIcon = ViewCubeIcon3
            OriginSymbol4.LabelFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            CoordinateSystemIcon4.LabelFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Viewport4.CoordinateSystemIcon = CoordinateSystemIcon4
            Viewport4.Legends = New devDept.Eyeshot.Legend(-1) {}
            ViewCubeIcon4.Font = Nothing
            ViewCubeIcon4.InitialRotation = New devDept.Geometry.Quaternion(0R, 0R, 0R, 1.0R)
            Viewport4.ViewCubeIcon = ViewCubeIcon4
            Me.Design3D.Viewports.Add(Viewport1)
            Me.Design3D.Viewports.Add(Viewport2)
            Me.Design3D.Viewports.Add(Viewport3)
            Me.Design3D.Viewports.Add(Viewport4)
            Me.Design3D.ViewportSizingEnabled = True
            ' 
            ' StatusStrip1
            ' 
            StatusStrip1.Items.AddRange(New ToolStripItem() {StatusTextLabel, StatusTextLabel2})
            resources.ApplyResources(StatusStrip1, "StatusStrip1")
            StatusStrip1.Name = "StatusStrip1"
            ' 
            ' StatusTextLabel
            ' 
            StatusTextLabel.Name = "StatusTextLabel"
            resources.ApplyResources(StatusTextLabel, "StatusTextLabel")
            ' 
            ' StatusTextLabel2
            ' 
            StatusTextLabel2.Name = "StatusTextLabel2"
            resources.ApplyResources(StatusTextLabel2, "StatusTextLabel2")
            ' 
            ' UpperTableLayoutPanel
            ' 
            resources.ApplyResources(UpperTableLayoutPanel, "UpperTableLayoutPanel")
            UpperTableLayoutPanel.Controls.Add(grp_Position, 2, 0)
            UpperTableLayoutPanel.Controls.Add(grp_Rotation, 0, 0)
            UpperTableLayoutPanel.Controls.Add(Panel1, 1, 0)
            UpperTableLayoutPanel.Controls.Add(UltraPanel1, 3, 0)
            UpperTableLayoutPanel.Name = "UpperTableLayoutPanel"
            ' 
            ' grp_Position
            ' 
            resources.ApplyResources(grp_Position, "grp_Position")
            grp_Position.Controls.Add(TableLayoutPanel2)
            grp_Position.Name = "grp_Position"
            ' 
            ' TableLayoutPanel2
            ' 
            resources.ApplyResources(TableLayoutPanel2, "TableLayoutPanel2")
            TableLayoutPanel2.Controls.Add(Label5, 0, 0)
            TableLayoutPanel2.Controls.Add(une_PosZ, 5, 0)
            TableLayoutPanel2.Controls.Add(une_PosX, 1, 0)
            TableLayoutPanel2.Controls.Add(Label4, 4, 0)
            TableLayoutPanel2.Controls.Add(Label6, 2, 0)
            TableLayoutPanel2.Controls.Add(une_PosY, 3, 0)
            TableLayoutPanel2.Name = "TableLayoutPanel2"
            ' 
            ' Label5
            ' 
            resources.ApplyResources(Label5, "Label5")
            Label5.Name = "Label5"
            ' 
            ' Label4
            ' 
            resources.ApplyResources(Label4, "Label4")
            Label4.Name = "Label4"
            ' 
            ' Label6
            ' 
            resources.ApplyResources(Label6, "Label6")
            Label6.Name = "Label6"
            ' 
            ' grp_Rotation
            ' 
            resources.ApplyResources(grp_Rotation, "grp_Rotation")
            grp_Rotation.Controls.Add(TableLayoutPanel4)
            grp_Rotation.Name = "grp_Rotation"
            ' 
            ' TableLayoutPanel4
            ' 
            resources.ApplyResources(TableLayoutPanel4, "TableLayoutPanel4")
            TableLayoutPanel4.Controls.Add(Label3, 4, 0)
            TableLayoutPanel4.Controls.Add(uneRotZ, 5, 0)
            TableLayoutPanel4.Controls.Add(Label2, 2, 0)
            TableLayoutPanel4.Controls.Add(Label1, 0, 0)
            TableLayoutPanel4.Controls.Add(uneRotX, 1, 0)
            TableLayoutPanel4.Controls.Add(uneRotY, 3, 0)
            TableLayoutPanel4.Name = "TableLayoutPanel4"
            ' 
            ' Label3
            ' 
            resources.ApplyResources(Label3, "Label3")
            Label3.Name = "Label3"
            ' 
            ' Label2
            ' 
            resources.ApplyResources(Label2, "Label2")
            Label2.Name = "Label2"
            ' 
            ' Label1
            ' 
            resources.ApplyResources(Label1, "Label1")
            Label1.Name = "Label1"
            ' 
            ' Panel1
            ' 
            resources.ApplyResources(Panel1, "Panel1")
            Panel1.Controls.Add(grp_Scale)
            Panel1.Name = "Panel1"
            ' 
            ' grp_Scale
            ' 
            resources.ApplyResources(grp_Scale, "grp_Scale")
            grp_Scale.Controls.Add(uneScalePercent)
            grp_Scale.Name = "grp_Scale"
            ' 
            ' CarModelsViewControl
            ' 
            CarModelsViewControl.CarStateMachine = Nothing
            CarModelsViewControl.CurrentCarFilePath = Nothing
            CarModelsViewControl.Directory = ""
            CarModelsViewControl.ItemsEnabled = True
            resources.ApplyResources(CarModelsViewControl, "CarModelsViewControl")
            CarModelsViewControl.Name = "CarModelsViewControl"
            ' 
            ' viewModelFilesPopUp
            ' 
            viewModelFilesPopUp.PopupControl = CarModelsViewControl
            ' 
            ' Consolidated3DControl
            ' 
            AutoScaleMode = AutoScaleMode.Inherit
            resources.ApplyResources(Me, "$this")
            Controls.Add(TableLayoutPanel1)
            Controls.Add(CarModelsViewControl)
            Name = "Consolidated3DControl"
            UltraPanel1.ClientArea.ResumeLayout(False)
            UltraPanel1.ResumeLayout(False)
            cm_cntrl.ResumeLayout(False)
            CType(uneRotY, ComponentModel.ISupportInitialize).EndInit()
            CType(uneRotZ, ComponentModel.ISupportInitialize).EndInit()
            CType(uneScalePercent, ComponentModel.ISupportInitialize).EndInit()
            CType(une_PosY, ComponentModel.ISupportInitialize).EndInit()
            CType(une_PosX, ComponentModel.ISupportInitialize).EndInit()
            CType(une_PosZ, ComponentModel.ISupportInitialize).EndInit()
            CType(uneRotX, ComponentModel.ISupportInitialize).EndInit()
            TableLayoutPanel1.ResumeLayout(False)
            TableLayoutPanel1.PerformLayout()
            StatusStrip1.ResumeLayout(False)
            StatusStrip1.PerformLayout()
            UpperTableLayoutPanel.ResumeLayout(False)
            CType(grp_Position, ComponentModel.ISupportInitialize).EndInit()
            grp_Position.ResumeLayout(False)
            TableLayoutPanel2.ResumeLayout(False)
            TableLayoutPanel2.PerformLayout()
            CType(grp_Rotation, ComponentModel.ISupportInitialize).EndInit()
            grp_Rotation.ResumeLayout(False)
            TableLayoutPanel4.ResumeLayout(False)
            TableLayoutPanel4.PerformLayout()
            Panel1.ResumeLayout(False)
            CType(grp_Scale, ComponentModel.ISupportInitialize).EndInit()
            grp_Scale.ResumeLayout(False)
            grp_Scale.PerformLayout()
            CType(UltraTabbedMdiManager1, ComponentModel.ISupportInitialize).EndInit()
            ResumeLayout(False)

        End Sub

        Friend WithEvents cm_cntrl As ContextMenuStrip
        Friend WithEvents LoadCarModelToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents TransparencyToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents AdjustmentautoToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents AdjustmentToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents ListOfDocumentsToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents FitSelectionToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents FitAllToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents SearchToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents Design3D As D3D.Consolidated.Designs.ConsolidatedDesign
        Friend WithEvents FineAdjustmentToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents UltraToolTipManager1 As Infragistics.Win.UltraWinToolTip.UltraToolTipManager
        Friend WithEvents viewModelFilesPopUp As Infragistics.Win.Misc.UltraPopupControlContainer
        Friend WithEvents CarModelsViewControl As CarModelsViewControl
        Friend WithEvents UltraToolTipManager2 As Infragistics.Win.UltraWinToolTip.UltraToolTipManager
        Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
        Friend WithEvents StatusTextLabel As System.Windows.Forms.ToolStripStatusLabel
        Friend WithEvents StatusTextLabel2 As System.Windows.Forms.ToolStripStatusLabel
        Friend WithEvents UltraTabbedMdiManager1 As Infragistics.Win.UltraWinTabbedMdi.UltraTabbedMdiManager
        Friend WithEvents UpperTableLayoutPanel As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents grp_Position As Infragistics.Win.Misc.UltraGroupBox
        Friend WithEvents grp_Scale As Infragistics.Win.Misc.UltraGroupBox
        Friend WithEvents uneScalePercent As Infragistics.Win.UltraWinEditors.UltraNumericEditor
        Friend WithEvents grp_Rotation As Infragistics.Win.Misc.UltraGroupBox
        Friend WithEvents TableLayoutPanel4 As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents Label3 As System.Windows.Forms.Label
        Friend WithEvents uneRotZ As Infragistics.Win.UltraWinEditors.UltraNumericEditor
        Friend WithEvents Label2 As System.Windows.Forms.Label
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Friend WithEvents uneRotX As Infragistics.Win.UltraWinEditors.UltraNumericEditor
        Friend WithEvents uneRotY As Infragistics.Win.UltraWinEditors.UltraNumericEditor
        Friend WithEvents TableLayoutPanel2 As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents Label5 As System.Windows.Forms.Label
        Friend WithEvents une_PosZ As Infragistics.Win.UltraWinEditors.UltraNumericEditor
        Friend WithEvents une_PosX As Infragistics.Win.UltraWinEditors.UltraNumericEditor
        Friend WithEvents Label4 As System.Windows.Forms.Label
        Friend WithEvents Label6 As System.Windows.Forms.Label
        Friend WithEvents une_PosY As Infragistics.Win.UltraWinEditors.UltraNumericEditor
        Friend WithEvents Panel1 As System.Windows.Forms.Panel
        Friend WithEvents btnAutoAdjust As Infragistics.Win.Misc.UltraButton
        Friend WithEvents UltraPanel1 As Infragistics.Win.Misc.UltraPanel
    End Class

End Namespace