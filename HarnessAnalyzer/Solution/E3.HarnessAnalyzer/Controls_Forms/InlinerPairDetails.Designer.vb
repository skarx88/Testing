Imports devDept.Eyeshot

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class InlinerPairDetails
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(InlinerPairDetails))
        Dim CancelToolBarButton1 As devDept.Eyeshot.CancelToolBarButton = New devDept.Eyeshot.CancelToolBarButton("Cancel", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
        Dim ProgressBar1 As devDept.Eyeshot.ProgressBar = New devDept.Eyeshot.ProgressBar(devDept.Eyeshot.ProgressBar.styleType.Circular, 0, "Idle", System.Drawing.Color.Black, System.Drawing.Color.Transparent, System.Drawing.Color.Green, 1.0R, True, CancelToolBarButton1, False, 0.1R, 0.333R, True)
        Dim DisplayModeSettingsRendered1 As devDept.Eyeshot.DisplayModeSettingsRendered = New devDept.Eyeshot.DisplayModeSettingsRendered(True, devDept.Eyeshot.edgeColorMethodType.SingleColor, System.Drawing.Color.Black, 1.0!, 2.0!, devDept.Eyeshot.silhouettesDrawingType.Never, False, devDept.Graphics.shadowType.None, Nothing, True, False, 0.3!, devDept.Graphics.realisticShadowQualityType.High)
        Dim BackgroundSettings1 As devDept.Graphics.BackgroundSettings = New devDept.Graphics.BackgroundSettings(devDept.Graphics.backgroundStyleType.LinearGradient, System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer)), System.Drawing.Color.DodgerBlue, System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer)), 0.75R, Nothing, devDept.Graphics.colorThemeType.[Auto], 0.33R)
        Dim Camera1 As devDept.Eyeshot.Camera = New devDept.Eyeshot.Camera(New devDept.Geometry.Point3D(0R, 0R, 45.0R), 380.0R, New devDept.Geometry.Quaternion(0.018434349666532526R, 0.039532590434972079R, 0.42221602280006187R, 0.90544518284475428R), devDept.Graphics.projectionType.Orthographic, 40.0R, 1.1800000820649477R, False, 0.001R)
        Dim HomeToolBarButton1 As devDept.Eyeshot.HomeToolBarButton = New devDept.Eyeshot.HomeToolBarButton("Home", devDept.Eyeshot.ToolBarButton.styleType.PushButton, True, True)
        Dim ZoomToolBarButton1 As devDept.Eyeshot.ZoomToolBarButton = New devDept.Eyeshot.ZoomToolBarButton("Zoom", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
        Dim PanToolBarButton1 As devDept.Eyeshot.PanToolBarButton = New devDept.Eyeshot.PanToolBarButton("Pan", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
        Dim RotateToolBarButton1 As devDept.Eyeshot.RotateToolBarButton = New devDept.Eyeshot.RotateToolBarButton("Rotate", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
        Dim ToolBar1 As devDept.Eyeshot.ToolBar = New devDept.Eyeshot.ToolBar(devDept.Eyeshot.ToolBar.positionType.VerticalMiddleLeft, True, New devDept.Eyeshot.ToolBarButton() {CType(HomeToolBarButton1, devDept.Eyeshot.ToolBarButton), CType(ZoomToolBarButton1, devDept.Eyeshot.ToolBarButton), CType(PanToolBarButton1, devDept.Eyeshot.ToolBarButton), CType(RotateToolBarButton1, devDept.Eyeshot.ToolBarButton)})
        Dim RotateSettings1 As devDept.Eyeshot.RotateSettings = New devDept.Eyeshot.RotateSettings(New devDept.Eyeshot.MouseButton(devDept.Eyeshot.mouseButtonsZPR.Middle, devDept.Eyeshot.modifierKeys.None), 10.0R, True, 1.0R, devDept.Eyeshot.rotationType.Turntable, devDept.Eyeshot.rotationCenterType.ViewportCenter, New devDept.Geometry.Point3D(0R, 0R, 0R), False)
        Dim ZoomSettings1 As devDept.Eyeshot.ZoomSettings = New devDept.Eyeshot.ZoomSettings(New devDept.Eyeshot.MouseButton(devDept.Eyeshot.mouseButtonsZPR.Middle, devDept.Eyeshot.modifierKeys.Shift), 25, True, devDept.Eyeshot.zoomStyleType.AtCursorLocation, False, 1.0R, System.Drawing.Color.Empty, devDept.Eyeshot.Camera.perspectiveFitType.Accurate, False, 10, True)
        Dim PanSettings1 As devDept.Eyeshot.PanSettings = New devDept.Eyeshot.PanSettings(New devDept.Eyeshot.MouseButton(devDept.Eyeshot.mouseButtonsZPR.Middle, devDept.Eyeshot.modifierKeys.Ctrl), 25, True)
        Dim NavigationSettings1 As devDept.Eyeshot.NavigationSettings = New devDept.Eyeshot.NavigationSettings(devDept.Eyeshot.Camera.navigationType.Examine, New devDept.Eyeshot.MouseButton(devDept.Eyeshot.mouseButtonsZPR.Left, devDept.Eyeshot.modifierKeys.None), New devDept.Geometry.Point3D(-1000.0R, -1000.0R, -1000.0R), New devDept.Geometry.Point3D(1000.0R, 1000.0R, 1000.0R), 8.0R, 50.0R, 50.0R)
        Dim SavedViewsManager1 As devDept.Eyeshot.Viewport.SavedViewsManager = New devDept.Eyeshot.Viewport.SavedViewsManager(8)
        Dim Viewport1 As devDept.Eyeshot.Viewport = New devDept.Eyeshot.Viewport(New System.Drawing.Point(0, 0), New System.Drawing.Size(192, 118), BackgroundSettings1, Camera1, New devDept.Eyeshot.ToolBar() {ToolBar1}, devDept.Eyeshot.displayType.Rendered, True, False, False, False, New devDept.Eyeshot.Grid(-1) {}, New devDept.Eyeshot.OriginSymbol(-1) {}, False, RotateSettings1, ZoomSettings1, PanSettings1, NavigationSettings1, SavedViewsManager1, devDept.Eyeshot.viewType.Trimetric)
        Dim CoordinateSystemIcon1 As devDept.Eyeshot.CoordinateSystemIcon = New devDept.Eyeshot.CoordinateSystemIcon(New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte)), System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.FromArgb(CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer)), System.Drawing.Color.OrangeRed, "Origin", "X", "Y", "Z", True, devDept.Eyeshot.coordinateSystemPositionType.BottomLeft, 37, Nothing, False)
        Dim ViewCubeIcon1 As devDept.Eyeshot.ViewCubeIcon = New devDept.Eyeshot.ViewCubeIcon(devDept.Eyeshot.coordinateSystemPositionType.TopRight, True, System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(20, Byte), Integer), CType(CType(60, Byte), Integer)), True, "Links", "Rechts", "Vorne", "Hinten", "Oben", "Unten", System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), Global.Microsoft.VisualBasic.ChrW(83), Global.Microsoft.VisualBasic.ChrW(78), Global.Microsoft.VisualBasic.ChrW(87), Global.Microsoft.VisualBasic.ChrW(69), True, Nothing, System.Drawing.Color.White, System.Drawing.Color.Black, 120, True, True, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, False, New devDept.Geometry.Quaternion(0R, 0R, 0R, 1.0R), True)
        Dim CancelToolBarButton2 As devDept.Eyeshot.CancelToolBarButton = New devDept.Eyeshot.CancelToolBarButton("Cancel", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
        Dim ProgressBar2 As devDept.Eyeshot.ProgressBar = New devDept.Eyeshot.ProgressBar(devDept.Eyeshot.ProgressBar.styleType.Circular, 0, "Idle", System.Drawing.Color.Black, System.Drawing.Color.Transparent, System.Drawing.Color.Green, 1.0R, True, CancelToolBarButton2, False, 0.1R, 0.333R, True)
        Dim DisplayModeSettingsRendered2 As devDept.Eyeshot.DisplayModeSettingsRendered = New devDept.Eyeshot.DisplayModeSettingsRendered(True, devDept.Eyeshot.edgeColorMethodType.SingleColor, System.Drawing.Color.Black, 1.0!, 2.0!, devDept.Eyeshot.silhouettesDrawingType.Never, False, devDept.Graphics.shadowType.None, Nothing, True, False, 0.3!, devDept.Graphics.realisticShadowQualityType.High)
        Dim BackgroundSettings2 As devDept.Graphics.BackgroundSettings = New devDept.Graphics.BackgroundSettings(devDept.Graphics.backgroundStyleType.LinearGradient, System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer)), System.Drawing.Color.DodgerBlue, System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer)), 0.75R, Nothing, devDept.Graphics.colorThemeType.[Auto], 0.33R)
        Dim Camera2 As devDept.Eyeshot.Camera = New devDept.Eyeshot.Camera(New devDept.Geometry.Point3D(0R, 0R, 45.0R), 380.0R, New devDept.Geometry.Quaternion(0.018434349666532526R, 0.039532590434972079R, 0.42221602280006187R, 0.90544518284475428R), devDept.Graphics.projectionType.Orthographic, 40.0R, 0.69620001747158544R, False, 0.001R)
        Dim HomeToolBarButton2 As devDept.Eyeshot.HomeToolBarButton = New devDept.Eyeshot.HomeToolBarButton("Home", devDept.Eyeshot.ToolBarButton.styleType.PushButton, True, True)
        Dim ZoomToolBarButton2 As devDept.Eyeshot.ZoomToolBarButton = New devDept.Eyeshot.ZoomToolBarButton("Zoom", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
        Dim PanToolBarButton2 As devDept.Eyeshot.PanToolBarButton = New devDept.Eyeshot.PanToolBarButton("Pan", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
        Dim RotateToolBarButton2 As devDept.Eyeshot.RotateToolBarButton = New devDept.Eyeshot.RotateToolBarButton("Rotate", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
        Dim ToolBar2 As devDept.Eyeshot.ToolBar = New devDept.Eyeshot.ToolBar(devDept.Eyeshot.ToolBar.positionType.VerticalMiddleLeft, True, New devDept.Eyeshot.ToolBarButton() {CType(HomeToolBarButton2, devDept.Eyeshot.ToolBarButton), CType(ZoomToolBarButton2, devDept.Eyeshot.ToolBarButton), CType(PanToolBarButton2, devDept.Eyeshot.ToolBarButton), CType(RotateToolBarButton2, devDept.Eyeshot.ToolBarButton)})
        Dim RotateSettings2 As devDept.Eyeshot.RotateSettings = New devDept.Eyeshot.RotateSettings(New devDept.Eyeshot.MouseButton(devDept.Eyeshot.mouseButtonsZPR.Middle, devDept.Eyeshot.modifierKeys.None), 10.0R, True, 1.0R, devDept.Eyeshot.rotationType.Turntable, devDept.Eyeshot.rotationCenterType.ViewportCenter, New devDept.Geometry.Point3D(0R, 0R, 0R), False)
        Dim ZoomSettings2 As devDept.Eyeshot.ZoomSettings = New devDept.Eyeshot.ZoomSettings(New devDept.Eyeshot.MouseButton(devDept.Eyeshot.mouseButtonsZPR.Middle, devDept.Eyeshot.modifierKeys.Shift), 25, True, devDept.Eyeshot.zoomStyleType.AtCursorLocation, False, 1.0R, System.Drawing.Color.Empty, devDept.Eyeshot.Camera.perspectiveFitType.Accurate, False, 10, True)
        Dim PanSettings2 As devDept.Eyeshot.PanSettings = New devDept.Eyeshot.PanSettings(New devDept.Eyeshot.MouseButton(devDept.Eyeshot.mouseButtonsZPR.Middle, devDept.Eyeshot.modifierKeys.Ctrl), 25, True)
        Dim NavigationSettings2 As devDept.Eyeshot.NavigationSettings = New devDept.Eyeshot.NavigationSettings(devDept.Eyeshot.Camera.navigationType.Examine, New devDept.Eyeshot.MouseButton(devDept.Eyeshot.mouseButtonsZPR.Left, devDept.Eyeshot.modifierKeys.None), New devDept.Geometry.Point3D(-1000.0R, -1000.0R, -1000.0R), New devDept.Geometry.Point3D(1000.0R, 1000.0R, 1000.0R), 8.0R, 50.0R, 50.0R)
        Dim SavedViewsManager2 As devDept.Eyeshot.Viewport.SavedViewsManager = New devDept.Eyeshot.Viewport.SavedViewsManager(8)
        Dim Viewport2 As devDept.Eyeshot.Viewport = New devDept.Eyeshot.Viewport(New System.Drawing.Point(0, 0), New System.Drawing.Size(200, 118), BackgroundSettings2, Camera2, New devDept.Eyeshot.ToolBar() {ToolBar2}, devDept.Eyeshot.displayType.Rendered, True, False, False, False, New devDept.Eyeshot.Grid(-1) {}, New devDept.Eyeshot.OriginSymbol(-1) {}, False, RotateSettings2, ZoomSettings2, PanSettings2, NavigationSettings2, SavedViewsManager2, devDept.Eyeshot.viewType.Trimetric)
        Dim CoordinateSystemIcon2 As devDept.Eyeshot.CoordinateSystemIcon = New devDept.Eyeshot.CoordinateSystemIcon(New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte)), System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.FromArgb(CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer)), System.Drawing.Color.OrangeRed, "Origin", "X", "Y", "Z", True, devDept.Eyeshot.coordinateSystemPositionType.BottomLeft, 37, Nothing, False)
        Dim ViewCubeIcon2 As devDept.Eyeshot.ViewCubeIcon = New devDept.Eyeshot.ViewCubeIcon(devDept.Eyeshot.coordinateSystemPositionType.TopRight, True, System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(20, Byte), Integer), CType(CType(60, Byte), Integer)), True, "Links", "Rechts", "Vorne", "Hinten", "Oben", "Unten", System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), Global.Microsoft.VisualBasic.ChrW(83), Global.Microsoft.VisualBasic.ChrW(78), Global.Microsoft.VisualBasic.ChrW(87), Global.Microsoft.VisualBasic.ChrW(69), True, Nothing, System.Drawing.Color.White, System.Drawing.Color.Black, 120, True, True, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, False, New devDept.Geometry.Quaternion(0R, 0R, 0R, 1.0R), True)
        Dim UltraTab1 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Dim UltraTab2 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Me.UltraTabPageControl2D = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        Me.vDraw = New VectorDraw.Professional.Control.VectorDrawBaseControl()
        Me.UltraTabPageControl3D = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.DesignLeft = New DesignEx
        Me.DesignRight = New DesignEx
        Me.udsInlinerPair = New Infragistics.Win.UltraWinDataSource.UltraDataSource(Me.components)



        Me.UltraTabControl1 = New Infragistics.Win.UltraWinTabControl.UltraTabControl()
        Me.UltraTabSharedControlsPage1 = New Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage()
        Me.uspInlinerPair = New Infragistics.Win.Misc.UltraSplitter()

        Me.ugInlinerPair = New Infragistics.Win.UltraWinGrid.UltraGrid()
        Me.UltraTabPageControl2D.SuspendLayout()
        Me.UltraTabPageControl3D.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.DesignLeft, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DesignRight, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.udsInlinerPair, System.ComponentModel.ISupportInitialize).BeginInit()



        CType(Me.UltraTabControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.UltraTabControl1.SuspendLayout()

        CType(Me.ugInlinerPair, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'UltraTabPageControl2D
        '
        Me.UltraTabPageControl2D.Controls.Add(Me.vDraw)
        resources.ApplyResources(Me.UltraTabPageControl2D, "UltraTabPageControl2D")
        Me.UltraTabPageControl2D.Name = "UltraTabPageControl2D"
        '
        'vDraw
        '
        Me.vDraw.AccessibleRole = System.Windows.Forms.AccessibleRole.Window
        Me.vDraw.AllowDrop = True
        Me.vDraw.Cursor = System.Windows.Forms.Cursors.Default

        vDraw.Location = New Point(0, 0)
        vDraw.Size = New Size(519, 20)
        Me.vDraw.Name = "vDraw"
        Me.vDraw.Dock = DockStyle.Fill
        vDraw.TabIndex = 0
        '
        'UltraTabPageControl3D
        '
        Me.UltraTabPageControl3D.Controls.Add(Me.SplitContainer1)
        resources.ApplyResources(Me.UltraTabPageControl3D, "UltraTabPageControl3D")
        Me.UltraTabPageControl3D.Name = "UltraTabPageControl3D"
        '
        'SplitContainer1
        '
        resources.ApplyResources(Me.SplitContainer1, "SplitContainer1")
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.DesignLeft)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.DesignRight)
        '
        'Model3DLeft
        '
        Me.DesignLeft.AntiAliasing = True
        Me.DesignLeft.AskForAntiAliasing = True
        Me.DesignLeft.Cursor = System.Windows.Forms.Cursors.Default
        resources.ApplyResources(Me.DesignLeft, "Model3DLeft")
        Me.DesignLeft.Name = "Model3DLeft"
        Me.DesignLeft.ProgressBar = ProgressBar1
        Me.DesignLeft.Rendered = DisplayModeSettingsRendered1
        CoordinateSystemIcon1.LabelFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Viewport1.CoordinateSystemIcon = CoordinateSystemIcon1
        Viewport1.Legends = New devDept.Eyeshot.Legend(-1) {}
        ViewCubeIcon1.Font = Nothing
        ViewCubeIcon1.InitialRotation = New devDept.Geometry.Quaternion(0R, 0R, 0R, 1.0R)
        Viewport1.ViewCubeIcon = ViewCubeIcon1
        Me.DesignLeft.Viewports.Add(Viewport1)
        '
        'Model3DRight
        '
        Me.DesignRight.AntiAliasing = True
        Me.DesignRight.AskForAntiAliasing = True
        Me.DesignRight.Cursor = System.Windows.Forms.Cursors.Default
        resources.ApplyResources(Me.DesignRight, "Model3DRight")
        Me.DesignRight.Name = "Model3DRight"
        Me.DesignRight.ProgressBar = ProgressBar2
        Me.DesignRight.Rendered = DisplayModeSettingsRendered2
        CoordinateSystemIcon2.LabelFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Viewport2.CoordinateSystemIcon = CoordinateSystemIcon2
        Viewport2.Legends = New devDept.Eyeshot.Legend(-1) {}
        ViewCubeIcon2.Font = Nothing
        ViewCubeIcon2.InitialRotation = New devDept.Geometry.Quaternion(0R, 0R, 0R, 1.0R)
        Viewport2.ViewCubeIcon = ViewCubeIcon2
        Me.DesignRight.Viewports.Add(Viewport2)
        '
        'udsInlinerPair
        '
        Me.udsInlinerPair.AllowAdd = False
        Me.udsInlinerPair.AllowDelete = False

        Me.UltraTabControl1.Dock = DockStyle.Bottom
        UltraTabControl1.Location = New Point(0, 111)
        UltraTabControl1.Size = New Size(523, 100)
        Me.ugInlinerPair.Dock = DockStyle.Fill
        Me.ugInlinerPair.DisplayLayout.MaxRowScrollRegions = 1

        '
        'UltraTabControl1
        '
        Me.UltraTabControl1.Controls.Add(Me.UltraTabSharedControlsPage1)
        Me.UltraTabControl1.Controls.Add(Me.UltraTabPageControl2D)
        Me.UltraTabControl1.Controls.Add(Me.UltraTabPageControl3D)
        'resources.ApplyResources(Me.UltraTabControl1, "UltraTabControl1")
        Me.UltraTabControl1.Name = "UltraTabControl1"
        Me.UltraTabControl1.SharedControlsPage = Me.UltraTabSharedControlsPage1
        UltraTab1.Key = "2D"
        UltraTab1.TabPage = Me.UltraTabPageControl2D
        resources.ApplyResources(UltraTab1, "UltraTab1")
        UltraTab1.ForceApplyResources = ""
        UltraTab2.Key = "3D"
        UltraTab2.TabPage = Me.UltraTabPageControl3D
        resources.ApplyResources(UltraTab2, "UltraTab2")
        UltraTab2.ForceApplyResources = ""
        Me.UltraTabControl1.Tabs.AddRange(New Infragistics.Win.UltraWinTabControl.UltraTab() {UltraTab1, UltraTab2})
        '
        'UltraTabSharedControlsPage1
        '
        resources.ApplyResources(Me.UltraTabSharedControlsPage1, "UltraTabSharedControlsPage1")
        Me.UltraTabSharedControlsPage1.Name = "UltraTabSharedControlsPage1"
        '
        'uspInlinerPair
        '
        Me.uspInlinerPair.BackColor = System.Drawing.SystemColors.Control
        resources.ApplyResources(Me.uspInlinerPair, "uspInlinerPair")
        Me.uspInlinerPair.Name = "uspInlinerPair"
        Me.uspInlinerPair.RestoreExtent = 66
        Me.uspInlinerPair.Location = New Point(0, 98)
        Me.uspInlinerPair.Size = New Size(523, 13)
        Me.uspInlinerPair.Dock = DockStyle.Bottom
        uspInlinerPair.TabIndex = 1

        Me.ugInlinerPair.Name = "ugInlinerPair"
        ugInlinerPair.Location = New Point(0, 0)
        ugInlinerPair.Size = New Size(523, 98)
        ugInlinerPair.TabIndex = 2
        '
        'InlinerPairDetails
        '
        resources.ApplyResources(Me, "$this")
        AutoScaleDimensions = New SizeF(7.0F, 15.0F)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Size = New Size(523, 159)

        Me.Controls.Add(Me.ugInlinerPair)
        Me.Controls.Add(Me.uspInlinerPair)
        Me.Controls.Add(Me.UltraTabControl1)

        Me.Name = "InlinerPairDetails"
        Me.UltraTabPageControl2D.ResumeLayout(False)
        Me.UltraTabPageControl3D.ResumeLayout(False)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        CType(Me.DesignLeft, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DesignRight, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.udsInlinerPair, System.ComponentModel.ISupportInitialize).EndInit()



        CType(Me.UltraTabControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.UltraTabControl1.ResumeLayout(False)

        CType(Me.ugInlinerPair, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents udsInlinerPair As Infragistics.Win.UltraWinDataSource.UltraDataSource
    Friend WithEvents ugInlinerPair As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents vDraw As VectorDraw.Professional.Control.VectorDrawBaseControl
    Friend WithEvents uspInlinerPair As Infragistics.Win.Misc.UltraSplitter
    Friend WithEvents UltraTabControl1 As Infragistics.Win.UltraWinTabControl.UltraTabControl
    Friend WithEvents UltraTabSharedControlsPage1 As Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage
    Friend WithEvents UltraTabPageControl2D As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
    Friend WithEvents UltraTabPageControl3D As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
    Friend WithEvents DesignLeft As devDept.Eyeshot.DesignEx
    Friend WithEvents DesignRight As devDept.Eyeshot.DesignEx
    Friend WithEvents SplitContainer1 As SplitContainer
End Class
