Imports System.Windows.Media.Media3D
Imports devDept.Eyeshot
Imports Zuken.E3.HarnessAnalyzer.D3D.Document

Namespace D3D
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class D3DComparerCntrl
        Inherits System.Windows.Forms.UserControl

        'UserControl überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    RemoveHandler _itemShowAll.Click, AddressOf ResetAllHiddenEntities
                    RemoveHandler _itemShow.Click, AddressOf ResetHiddenEntity
                    RemoveHandler _itemHide.Click, AddressOf AddEntityToHiddens
                    RemoveHandler _itemShowTooltip.Click, AddressOf ShowTooltipOnEntity

                    If Design3D IsNot Nothing AndAlso Not Design3D.IsDisposed Then

                        _kblId_EntityMapper = Nothing
                        _wireId_EntityMapper = Nothing
                        _idEntityMapperRef = Nothing
                        _idEntityMapperComp = Nothing
                        _routedRefElements = Nothing
                        _routedCompElements = Nothing
                        selection = Nothing
                        _refChanges = Nothing
                        _compChanges = Nothing
                        _refEntities = Nothing
                        _compEntities = Nothing
                        _myRefEntities = Nothing
                        _myCompEntities = Nothing
                        _compSelection = Nothing
                        _refSelection = Nothing
                        _changedObjects = Nothing
                        _sourceObjects = Nothing

                        ClearAttached()
                        _documents = Nothing
                        If _ttManager IsNot Nothing Then
                            _ttManager.Dispose()
                            _ttManager = Nothing
                        End If
                        RemoveToolBarButtonEvents()
                        Design3D.Dispose()
                    End If
                    components.Dispose()
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
            components = New ComponentModel.Container()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(D3DComparerCntrl))
            Dim CancelToolBarButton1 As CancelToolBarButton = New CancelToolBarButton("Cancel", ToolBarButton.styleType.ToggleButton, True, True)
            Dim ProgressBar1 As ProgressBar = New ProgressBar(ProgressBar.styleType.Linear, 0, "Idle", Color.Black, Color.Transparent, Color.Green, 1.0R, True, CancelToolBarButton1, True, 0.1R, 0.333R, True)
            Dim DisplayModeSettingsRendered1 As DisplayModeSettingsRendered = New DisplayModeSettingsRendered(True, edgeColorMethodType.EntityColor, Color.Black, 1.0F, 12.0F, silhouettesDrawingType.Never, False, devDept.Graphics.shadowType.None, Nothing, False, False, 0.3F, devDept.Graphics.realisticShadowQualityType.Low)
            Dim DisplayModeSettingsShaded1 As DisplayModeSettingsShaded = New DisplayModeSettingsShaded(True, edgeColorMethodType.EntityColor, Color.Black, 1.0F, 12.0F, silhouettesDrawingType.Never, False, devDept.Graphics.shadowType.None)
            Dim ShortcutKeysSettings1 As ShortcutKeysSettings = New ShortcutKeysSettings(Keys.Control Or Keys.A, Keys.Control Or Keys.I, Keys.None, Keys.Control Or Keys.F, Keys.Control Or Keys.Add, Keys.Control Or Keys.Subtract, Keys.Control Or Keys.C, Keys.Control Or Keys.V, Keys.Control Or Keys.X, Keys.Control Or Keys.G, Keys.Control Or Keys.Shift Or Keys.G, Keys.Right, Keys.Up, Keys.Left, Keys.Down, Keys.Control Or Keys.Right, Keys.Control Or Keys.Up, Keys.Control Or Keys.Left, Keys.Control Or Keys.Down, Keys.Escape, Keys.D, Keys.A, Keys.E, Keys.Q, Keys.W, Keys.S)
            Dim BackgroundSettings1 As devDept.Graphics.BackgroundSettings = New devDept.Graphics.BackgroundSettings(devDept.Graphics.backgroundStyleType.Solid, Color.FromArgb(CByte(245), CByte(245), CByte(245)), Color.White, Color.White, 0.75R, Nothing, devDept.Graphics.colorThemeType.Auto, 0.33R)
            Dim Camera1 As devDept.Eyeshot.Camera = New devDept.Eyeshot.Camera(New devDept.Geometry.Point3D(0R, 0R, 44.999999999999993R), 380.0R, New devDept.Geometry.Quaternion(0.018434349666532512R, 0.039532590434972065R, 0.42221602280006187R, 0.90544518284475428R), devDept.Graphics.projectionType.Perspective, 40.0R, 7.49999088763451R, False, 0.001R)
            Dim HomeToolBarButton1 As HomeToolBarButton = New HomeToolBarButton("Home", ToolBarButton.styleType.PushButton, True, True)
            Dim MagnifyingGlassToolBarButton1 As MagnifyingGlassToolBarButton = New MagnifyingGlassToolBarButton("Magnifying Glass", ToolBarButton.styleType.ToggleButton, True, True)
            Dim ZoomWindowToolBarButton1 As ZoomWindowToolBarButton = New ZoomWindowToolBarButton("Zoom Window", ToolBarButton.styleType.ToggleButton, True, True)
            Dim ZoomToolBarButton1 As ZoomToolBarButton = New ZoomToolBarButton("Zoom", ToolBarButton.styleType.ToggleButton, True, True)
            Dim PanToolBarButton1 As PanToolBarButton = New PanToolBarButton("Pan", ToolBarButton.styleType.ToggleButton, True, True)
            Dim RotateToolBarButton1 As RotateToolBarButton = New RotateToolBarButton("Rotate", ToolBarButton.styleType.ToggleButton, True, True)
            Dim ZoomFitToolBarButton1 As ZoomFitToolBarButton = New ZoomFitToolBarButton("Zoom Fit", ToolBarButton.styleType.PushButton, True, True)
            Dim ToolBar1 As ToolBar = New ToolBar(ToolBar.positionType.VerticalMiddleRight, True, New ToolBarButton() {HomeToolBarButton1, MagnifyingGlassToolBarButton1, ZoomWindowToolBarButton1, ZoomToolBarButton1, PanToolBarButton1, RotateToolBarButton1, ZoomFitToolBarButton1})
            Dim Histogram1 As Histogram = New Histogram(30, 80, "Title", Color.Blue, Color.Gray, Color.Black, Color.Red, Color.LightYellow, False, True, False, "{0:+0.###;-0.###;0}")
            Dim Grid1 As Grid = New Grid(New devDept.Geometry.Point3D(-100.0R, -100.0R, 0R), New devDept.Geometry.Point3D(100.0R, 100.0R, 0R), 10.0R, New devDept.Geometry.Plane(New devDept.Geometry.Point3D(0R, 0R, 0R), New devDept.Geometry.Vector3D(0R, 0R, 1.0R)), Color.FromArgb(CByte(63), CByte(128), CByte(128), CByte(128)), Color.FromArgb(CByte(127), CByte(255), CByte(0), CByte(0)), Color.FromArgb(CByte(127), CByte(0), CByte(128), CByte(0)), False, False, False, False, 10, 100, 10, Color.FromArgb(CByte(127), CByte(90), CByte(90), CByte(90)), Color.Transparent, False, Color.FromArgb(CByte(12), CByte(0), CByte(0), CByte(255)))
            Dim OriginSymbol1 As OriginSymbol = New OriginSymbol(10, originSymbolStyleType.Ball, New Font("Microsoft Sans Serif", 8.25F), Color.Black, Color.Black, Color.Black, Color.Black, Color.Red, Color.Green, Color.Blue, "Origin", "X", "Y", "Z", False, Nothing, False)
            Dim RotateSettings1 As RotateSettings = New RotateSettings(New MouseButton(mouseButtonsZPR.Middle, devDept.Eyeshot.modifierKeys.None), 10.0R, True, 1.0R, rotationType.Trackball, rotationCenterType.ViewportCenter, New devDept.Geometry.Point3D(0R, 0R, 0R), False)
            Dim ZoomSettings1 As ZoomSettings = New ZoomSettings(New MouseButton(mouseButtonsZPR.Middle, devDept.Eyeshot.modifierKeys.Shift), 25, True, zoomStyleType.AtCursorLocation, False, 1.0R, Color.Empty, devDept.Eyeshot.Camera.perspectiveFitType.Quick, False, 10, True)
            Dim PanSettings1 As PanSettings = New PanSettings(New MouseButton(mouseButtonsZPR.Middle, devDept.Eyeshot.modifierKeys.Ctrl), 25, True)
            Dim NavigationSettings1 As NavigationSettings = New NavigationSettings(devDept.Eyeshot.Camera.navigationType.Examine, New MouseButton(mouseButtonsZPR.Left, devDept.Eyeshot.modifierKeys.None), New devDept.Geometry.Point3D(-1000.0R, -1000.0R, -1000.0R), New devDept.Geometry.Point3D(1000.0R, 1000.0R, 1000.0R), 8.0R, 50.0R, 50.0R)
            Dim CoordinateSystemIcon1 As CoordinateSystemIcon = New CoordinateSystemIcon(New Font("Microsoft Sans Serif", 8.25F), Color.Black, Color.Black, Color.Black, Color.Black, Color.FromArgb(CByte(80), CByte(80), CByte(80)), Color.FromArgb(CByte(80), CByte(80), CByte(80)), Color.OrangeRed, "Origin", "X", "Y", "Z", True, coordinateSystemPositionType.BottomLeft, 37, Nothing, False)
            Dim ViewCubeIcon1 As ViewCubeIcon = New ViewCubeIcon(coordinateSystemPositionType.TopRight, True, Color.FromArgb(CByte(30), CByte(144), CByte(255)), True, "Links", "Rechts", "Vorne", "Hinten", "Oben", "Unten", Color.FromArgb(CByte(240), CByte(77), CByte(77), CByte(77)), Color.FromArgb(CByte(240), CByte(77), CByte(77), CByte(77)), Color.FromArgb(CByte(240), CByte(77), CByte(77), CByte(77)), Color.FromArgb(CByte(240), CByte(77), CByte(77), CByte(77)), Color.FromArgb(CByte(240), CByte(77), CByte(77), CByte(77)), Color.FromArgb(CByte(240), CByte(77), CByte(77), CByte(77)), "S"c, "N"c, "W"c, "E"c, True, Nothing, Color.White, Color.Black, 120, True, True, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, False, New devDept.Geometry.Quaternion(0R, 0R, 0R, 1.0R), True)
            Dim SavedViewsManager1 As devDept.Eyeshot.Viewport.SavedViewsManager = New devDept.Eyeshot.Viewport.SavedViewsManager(8)
            Dim Viewport1 As devDept.Eyeshot.Viewport = New devDept.Eyeshot.Viewport(New System.Drawing.Point(0, 0), New System.Drawing.Size(501, 450), BackgroundSettings1, Camera1, New devDept.Eyeshot.ToolBar() {ToolBar1}, devDept.Eyeshot.displayType.Rendered, True, False, False, False, New devDept.Eyeshot.Grid() {Grid1}, New devDept.Eyeshot.OriginSymbol() {OriginSymbol1}, False, RotateSettings1, ZoomSettings1, PanSettings1, NavigationSettings1, SavedViewsManager1, devDept.Eyeshot.viewType.Trimetric)

            Design3D = New DocumentDesign()
            ContextMenuStrip1 = New ContextMenuStrip(components)
            ContextMenuStripCaption = New ContextMenuStrip(components)
            CopyCaptionToolStripMenuItem = New ToolStripMenuItem()
            CType(Design3D, ComponentModel.ISupportInitialize).BeginInit()
            ContextMenuStripCaption.SuspendLayout()
            SuspendLayout()
            ' 
            ' D3D
            ' 
            Design3D.AllowedSelectionCount = Integer.MaxValue
            Design3D.AnimateCamera = True
            Design3D.AnimateCameraDuration = 100
            Design3D.AntiAliasing = True
            Design3D.AntiAliasingSamples = devDept.Graphics.antialiasingSamplesNumberType.x8
            Design3D.AskForAntiAliasing = True
            Design3D.AutoDisableMode = DisableSelectionMode.None
            Design3D.BlinkSynchronizer = Nothing
            Design3D.ContextMenuStrip = ContextMenuStrip1
            Design3D.Cursor = Cursors.Default
            Design3D.Dock = DockStyle.Fill
            Design3D.EEModel = Nothing
            Design3D.Font = New Font("Segoe UI", 9.0F)
            Design3D.IsHiddenSelection = False
            Design3D.IsResetHiddenEntity = False
            Design3D.Location = New Point(0, 0)
            Design3D.Margin = New Padding(4, 3, 4, 3)
            Design3D.Name = "D3DModel"
            Design3D.PickBoxEnabled = False
            Design3D.ProgressBar = ProgressBar1
            Design3D.Rendered = DisplayModeSettingsRendered1
            Design3D.Renderer = rendererType.Direct3D
            Design3D.Selection.Color = Color.Magenta
            Design3D.Selection.ColorDynamic = Color.FromArgb(CByte(255), CByte(128), CByte(255))
            Design3D.Shaded = DisplayModeSettingsShaded1
            Design3D.ShortcutKeys = ShortcutKeysSettings1
            Design3D.ShowSilhouetteDynamicPickEntities = SilhouetteDynamicPickMode.None
            Design3D.Size = New Size(1167, 750)
            Design3D.TabIndex = 1
            Design3D.Text = "Design3D"
            Design3D.Viewports.Add(Viewport1)
            Design3D.WheelZoomEnabled = False
            ' 
            ' ContextMenuStrip1
            ' 
            ContextMenuStrip1.Name = "ContextMenuStrip1"
            ContextMenuStrip1.Size = New Size(61, 4)
            ' 
            ' ContextMenuStripCaption
            ' 
            ContextMenuStripCaption.Items.AddRange(New ToolStripItem() {CopyCaptionToolStripMenuItem})
            ContextMenuStripCaption.Name = "ContextMenuStripCaption"
            ContextMenuStripCaption.Size = New Size(68, 26)
            ' 
            ' CopyCaptionToolStripMenuItem
            ' 
            CopyCaptionToolStripMenuItem.Image = My.Resources.Resources.copy
            CopyCaptionToolStripMenuItem.Name = "CopyCaptionToolStripMenuItem"
            CopyCaptionToolStripMenuItem.Size = New Size(67, 22)
            ' 
            ' D3DComparerControl
            ' 
            AutoScaleDimensions = New SizeF(7.0F, 15.0F)
            AutoScaleMode = AutoScaleMode.Font
            Controls.Add(Design3D)
            Margin = New Padding(4, 3, 4, 3)
            Name = "D3DComparerCtrl"
            Size = New Size(1167, 750)
            CType(Design3D, ComponentModel.ISupportInitialize).EndInit()
            ContextMenuStripCaption.ResumeLayout(False)
            ResumeLayout(False)

        End Sub

        Protected Friend WithEvents Design3D As Zuken.E3.HarnessAnalyzer.D3D.Document.DocumentDesign
        Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
        Friend WithEvents ContextMenuStripCaption As ContextMenuStrip
        Friend WithEvents CopyCaptionToolStripMenuItem As ToolStripMenuItem

    End Class
End Namespace
