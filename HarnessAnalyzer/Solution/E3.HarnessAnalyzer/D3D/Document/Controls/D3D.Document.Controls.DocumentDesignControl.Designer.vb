Namespace D3D.Document.Controls

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class DocumentDesignControl
        Inherits System.Windows.Forms.UserControl

        'UserControl überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing Then
                    RemoveHandler _itemShowAll.Click, AddressOf ResetAllHiddenEntities
                    RemoveHandler _itemShow.Click, AddressOf ResetHiddenEntity
                    RemoveHandler _itemHide.Click, AddressOf AddEntityToHiddens
                    RemoveHandler _itemShowTooltip.Click, AddressOf ShowTooltipOnEntity

                    If Me.ContextMenuStrip1 IsNot Nothing Then
                        Me.ContextMenuStrip1.Dispose()
                    End If
                    If Design IsNot Nothing AndAlso Not Design.IsDisposed Then
                        RemoveHandler Design.MouseClickLabel, AddressOf OnLabelClicked
                        Design.Dispose()
                    End If
                    If components IsNot Nothing Then
                        components.Dispose()
                    End If
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        'Wird vom Windows Form-Designer benötigt.
        Private components As System.ComponentModel.IContainer

        'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
        'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
        'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container()
            Dim CancelToolBarButton1 As devDept.Eyeshot.CancelToolBarButton = New devDept.Eyeshot.CancelToolBarButton("Cancel", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
            Dim ProgressBar1 As devDept.Eyeshot.ProgressBar = New devDept.Eyeshot.ProgressBar(devDept.Eyeshot.ProgressBar.styleType.Linear, 0, "Idle", System.Drawing.Color.Black, System.Drawing.Color.Transparent, System.Drawing.Color.Green, 1.0R, True, CancelToolBarButton1, True, 0.1R, 0.333R, True)
            Dim DisplayModeSettingsRendered1 As devDept.Eyeshot.DisplayModeSettingsRendered = New devDept.Eyeshot.DisplayModeSettingsRendered(True, devDept.Eyeshot.edgeColorMethodType.EntityColor, System.Drawing.Color.Black, 1.0!, 12.0!, devDept.Eyeshot.silhouettesDrawingType.Never, False, devDept.Graphics.shadowType.None, Nothing, False, False, 0.3!, devDept.Graphics.realisticShadowQualityType.Low)
            Dim DisplayModeSettingsShaded1 As devDept.Eyeshot.DisplayModeSettingsShaded = New devDept.Eyeshot.DisplayModeSettingsShaded(True, devDept.Eyeshot.edgeColorMethodType.EntityColor, System.Drawing.Color.Black, 1.0!, 12.0!, devDept.Eyeshot.silhouettesDrawingType.Never, False, devDept.Graphics.shadowType.None)
            Dim ShortcutKeysSettings1 As devDept.Eyeshot.ShortcutKeysSettings = New devDept.Eyeshot.ShortcutKeysSettings(CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.A), System.Windows.Forms.Keys), CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.I), System.Windows.Forms.Keys), System.Windows.Forms.Keys.None, CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.F), System.Windows.Forms.Keys), CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Add), System.Windows.Forms.Keys), CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Subtract), System.Windows.Forms.Keys), CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.C), System.Windows.Forms.Keys), CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.V), System.Windows.Forms.Keys), CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.X), System.Windows.Forms.Keys), CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.G), System.Windows.Forms.Keys), CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Shift) _
                Or System.Windows.Forms.Keys.G), System.Windows.Forms.Keys), System.Windows.Forms.Keys.Right, System.Windows.Forms.Keys.Up, System.Windows.Forms.Keys.Left, System.Windows.Forms.Keys.Down, CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Right), System.Windows.Forms.Keys), CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Up), System.Windows.Forms.Keys), CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Left), System.Windows.Forms.Keys), CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Down), System.Windows.Forms.Keys), System.Windows.Forms.Keys.Escape, System.Windows.Forms.Keys.D, System.Windows.Forms.Keys.A, System.Windows.Forms.Keys.E, System.Windows.Forms.Keys.Q, System.Windows.Forms.Keys.W, System.Windows.Forms.Keys.S)
            Dim BackgroundSettings1 As devDept.Graphics.BackgroundSettings = New devDept.Graphics.BackgroundSettings(devDept.Graphics.backgroundStyleType.Solid, System.Drawing.Color.FromArgb(CType(CType(245, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(245, Byte), Integer)), System.Drawing.Color.White, System.Drawing.Color.White, 0.75R, Nothing, devDept.Graphics.colorThemeType.[Auto], 0.33R)
            Dim Camera1 As devDept.Eyeshot.Camera = New devDept.Eyeshot.Camera(New devDept.Geometry.Point3D(0R, 0R, 44.999999999999993R), 380.0R, New devDept.Geometry.Quaternion(0.018434349666532512R, 0.039532590434972065R, 0.42221602280006187R, 0.90544518284475428R), devDept.Graphics.projectionType.Perspective, 40.0R, 4.4999945993846859R, False, 0.001R)
            Dim HomeToolBarButton1 As devDept.Eyeshot.HomeToolBarButton = New devDept.Eyeshot.HomeToolBarButton("Home", devDept.Eyeshot.ToolBarButton.styleType.PushButton, True, True)
            Dim MagnifyingGlassToolBarButton1 As devDept.Eyeshot.MagnifyingGlassToolBarButton = New devDept.Eyeshot.MagnifyingGlassToolBarButton("Magnifying Glass", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
            Dim ZoomWindowToolBarButton1 As devDept.Eyeshot.ZoomWindowToolBarButton = New devDept.Eyeshot.ZoomWindowToolBarButton("Zoom Window", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
            Dim ZoomToolBarButton1 As devDept.Eyeshot.ZoomToolBarButton = New devDept.Eyeshot.ZoomToolBarButton("Zoom", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
            Dim PanToolBarButton1 As devDept.Eyeshot.PanToolBarButton = New devDept.Eyeshot.PanToolBarButton("Pan", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
            Dim RotateToolBarButton1 As devDept.Eyeshot.RotateToolBarButton = New devDept.Eyeshot.RotateToolBarButton("Rotate", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
            Dim ZoomFitToolBarButton1 As devDept.Eyeshot.ZoomFitToolBarButton = New devDept.Eyeshot.ZoomFitToolBarButton("Zoom Fit", devDept.Eyeshot.ToolBarButton.styleType.PushButton, True, True)
            Dim ToolBar1 As devDept.Eyeshot.ToolBar = New devDept.Eyeshot.ToolBar(devDept.Eyeshot.ToolBar.positionType.VerticalMiddleRight, True, New devDept.Eyeshot.ToolBarButton() {CType(HomeToolBarButton1, devDept.Eyeshot.ToolBarButton), CType(MagnifyingGlassToolBarButton1, devDept.Eyeshot.ToolBarButton), CType(ZoomWindowToolBarButton1, devDept.Eyeshot.ToolBarButton), CType(ZoomToolBarButton1, devDept.Eyeshot.ToolBarButton), CType(PanToolBarButton1, devDept.Eyeshot.ToolBarButton), CType(RotateToolBarButton1, devDept.Eyeshot.ToolBarButton), CType(ZoomFitToolBarButton1, devDept.Eyeshot.ToolBarButton)})
            Dim Grid1 As devDept.Eyeshot.Grid = New devDept.Eyeshot.Grid(New devDept.Geometry.Point3D(-100.0R, -100.0R, 0R), New devDept.Geometry.Point3D(100.0R, 100.0R, 0R), 10.0R, New devDept.Geometry.Plane(New devDept.Geometry.Point3D(0R, 0R, 0R), New devDept.Geometry.Vector3D(0R, 0R, 1.0R)), System.Drawing.Color.FromArgb(CType(CType(63, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(127, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(127, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(0, Byte), Integer)), False, False, False, False, 10, 100, 10, System.Drawing.Color.FromArgb(CType(CType(127, Byte), Integer), CType(CType(90, Byte), Integer), CType(CType(90, Byte), Integer), CType(CType(90, Byte), Integer)), System.Drawing.Color.Transparent, False, System.Drawing.Color.FromArgb(CType(CType(12, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(255, Byte), Integer)))
            Dim OriginSymbol1 As devDept.Eyeshot.OriginSymbol = New devDept.Eyeshot.OriginSymbol(10, devDept.Eyeshot.originSymbolStyleType.Ball, New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte)), System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.Red, System.Drawing.Color.Green, System.Drawing.Color.Blue, "Origin", "X", "Y", "Z", True, Nothing, False)
            Dim RotateSettings1 As devDept.Eyeshot.RotateSettings = New devDept.Eyeshot.RotateSettings(New devDept.Eyeshot.MouseButton(devDept.Eyeshot.mouseButtonsZPR.Middle, devDept.Eyeshot.modifierKeys.None), 10.0R, True, 1.0R, devDept.Eyeshot.rotationType.Trackball, devDept.Eyeshot.rotationCenterType.ViewportCenter, New devDept.Geometry.Point3D(0R, 0R, 0R), False)
            Dim ZoomSettings1 As devDept.Eyeshot.ZoomSettings = New devDept.Eyeshot.ZoomSettings(New devDept.Eyeshot.MouseButton(devDept.Eyeshot.mouseButtonsZPR.Middle, devDept.Eyeshot.modifierKeys.Shift), 25, True, devDept.Eyeshot.zoomStyleType.AtCursorLocation, False, 1.0R, System.Drawing.Color.Empty, devDept.Eyeshot.Camera.perspectiveFitType.Quick, False, 10, True)
            Dim PanSettings1 As devDept.Eyeshot.PanSettings = New devDept.Eyeshot.PanSettings(New devDept.Eyeshot.MouseButton(devDept.Eyeshot.mouseButtonsZPR.Middle, devDept.Eyeshot.modifierKeys.Ctrl), 25, True)
            Dim NavigationSettings1 As devDept.Eyeshot.NavigationSettings = New devDept.Eyeshot.NavigationSettings(devDept.Eyeshot.Camera.navigationType.Examine, New devDept.Eyeshot.MouseButton(devDept.Eyeshot.mouseButtonsZPR.Left, devDept.Eyeshot.modifierKeys.None), New devDept.Geometry.Point3D(-1000.0R, -1000.0R, -1000.0R), New devDept.Geometry.Point3D(1000.0R, 1000.0R, 1000.0R), 8.0R, 50.0R, 50.0R)
            Dim SavedViewsManager1 As devDept.Eyeshot.Viewport.SavedViewsManager = New devDept.Eyeshot.Viewport.SavedViewsManager(8)
            Dim Viewport1 As devDept.Eyeshot.Viewport = New devDept.Eyeshot.Viewport(New System.Drawing.Point(0, 0), New System.Drawing.Size(501, 450), BackgroundSettings1, Camera1, New devDept.Eyeshot.ToolBar() {ToolBar1}, devDept.Eyeshot.displayType.Rendered, True, False, False, False, New devDept.Eyeshot.Grid() {Grid1}, New devDept.Eyeshot.OriginSymbol() {OriginSymbol1}, False, RotateSettings1, ZoomSettings1, PanSettings1, NavigationSettings1, SavedViewsManager1, devDept.Eyeshot.viewType.Trimetric)
            Dim CoordinateSystemIcon1 As devDept.Eyeshot.CoordinateSystemIcon = New devDept.Eyeshot.CoordinateSystemIcon(New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte)), System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.FromArgb(CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer)), System.Drawing.Color.OrangeRed, "Origin", "X", "Y", "Z", True, devDept.Eyeshot.coordinateSystemPositionType.BottomLeft, 37, Nothing, False)
            Dim ViewCubeIcon1 As devDept.Eyeshot.ViewCubeIcon = New devDept.Eyeshot.ViewCubeIcon(devDept.Eyeshot.coordinateSystemPositionType.TopRight, True, System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(20, Byte), Integer), CType(CType(60, Byte), Integer)), True, "Links", "Rechts", "Vorne", "Hinten", "Oben", "Unten", System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), Global.Microsoft.VisualBasic.ChrW(83), Global.Microsoft.VisualBasic.ChrW(78), Global.Microsoft.VisualBasic.ChrW(87), Global.Microsoft.VisualBasic.ChrW(69), True, Nothing, System.Drawing.Color.White, System.Drawing.Color.Black, 120, True, True, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, False, New devDept.Geometry.Quaternion(0R, 0R, 0R, 1.0R), True)
            Me.Design = New Zuken.E3.HarnessAnalyzer.D3D.Document.DocumentDesign()
            Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
            CType(Me.Design, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.ContextMenuStrip1.SuspendLayout()
            Me.SuspendLayout()
            '
            'Design
            '
            Me.Design.AllowedSelectionCount = 2147483647
            Me.Design.AnimateCamera = True
            Me.Design.AnimateCameraDuration = 100
            Me.Design.AntiAliasing = True
            Me.Design.AntiAliasingSamples = devDept.Graphics.antialiasingSamplesNumberType.x8
            Me.Design.AskForAntiAliasing = True
            Me.Design.AutoDisableMode = devDept.Eyeshot.DisableSelectionMode.None
            Me.Design.BlinkSynchronizer = Nothing
            Me.Design.ContextMenuStrip = Me.ContextMenuStrip1
            Me.Design.Cursor = System.Windows.Forms.Cursors.Default
            Me.Design.Dock = System.Windows.Forms.DockStyle.Fill
            Me.Design.EEModel = Nothing
            Me.Design.Location = New System.Drawing.Point(0, 0)
            Me.Design.Name = "Design"
            Me.Design.PickBoxEnabled = False
            Me.Design.ProgressBar = ProgressBar1
            Me.Design.Rendered = DisplayModeSettingsRendered1
            Me.Design.Renderer = devDept.Eyeshot.rendererType.Direct3D
            Me.Design.Selection.Color = System.Drawing.Color.Magenta
            Me.Design.Selection.ColorDynamic = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer))
            Me.Design.Shaded = DisplayModeSettingsShaded1
            Me.Design.ShortcutKeys = ShortcutKeysSettings1
            Me.Design.ShowSilhouetteDynamicPickEntities = devDept.Eyeshot.SilhouetteDynamicPickMode.None
            Me.Design.Size = New System.Drawing.Size(501, 450)
            'Me.Design.Snapping = devDept.Eyeshot.Snapping.None
            Me.Design.TabIndex = 1
            Me.Design.Text = "Design"
            OriginSymbol1.LabelFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            CoordinateSystemIcon1.LabelFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Viewport1.CoordinateSystemIcon = CoordinateSystemIcon1
            Viewport1.Legends = New devDept.Eyeshot.Legend(-1) {}
            ViewCubeIcon1.Font = Nothing
            ViewCubeIcon1.InitialRotation = New devDept.Geometry.Quaternion(0R, 0R, 0R, 1.0R)
            Viewport1.ViewCubeIcon = ViewCubeIcon1
            Me.Design.Viewports.Add(Viewport1)
            Me.Design.WheelZoomEnabled = False
            '
            'ContextMenuStrip1
            '
            Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
            Me.ContextMenuStrip1.Size = New System.Drawing.Size(207, 48)
            '
            '
            'Document3DModelControl
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.Design)
            Me.Name = "Document3DModelControl"
            Me.Size = New System.Drawing.Size(501, 450)
            CType(Me.Design, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ContextMenuStrip1.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub

        Protected Friend WithEvents Design As HarnessAnalyzer.D3D.Document.DocumentDesign
        Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
    End Class

End Namespace