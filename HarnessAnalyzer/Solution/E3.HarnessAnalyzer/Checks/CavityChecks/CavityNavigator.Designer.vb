Namespace Checks.Cavities

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class CavityNavigator
        Inherits System.Windows.Forms.UserControl

        'UserControl überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
                _document = Nothing
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
            Dim ProgressBar1 As devDept.Eyeshot.ProgressBar = New devDept.Eyeshot.ProgressBar(devDept.Eyeshot.ProgressBar.styleType.Circular, 0, "Idle", System.Drawing.Color.Black, System.Drawing.Color.Transparent, System.Drawing.Color.Green, 1.0R, True, CancelToolBarButton1, False, 0.1R, 0.333R, True)
            Dim DisplayModeSettingsRendered1 As devDept.Eyeshot.DisplayModeSettingsRendered = New devDept.Eyeshot.DisplayModeSettingsRendered(True, devDept.Eyeshot.edgeColorMethodType.SingleColor, System.Drawing.Color.Black, 1.0!, 2.0!, devDept.Eyeshot.silhouettesDrawingType.Never, False, devDept.Graphics.shadowType.None, Nothing, True, False, 0.3!, devDept.Graphics.realisticShadowQualityType.High)
            Dim BackgroundSettings1 As devDept.Graphics.BackgroundSettings = New devDept.Graphics.BackgroundSettings(devDept.Graphics.backgroundStyleType.LinearGradient, System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer)), System.Drawing.Color.DodgerBlue, System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer)), 0.75R, Nothing, devDept.Graphics.colorThemeType.[Auto], 0.33R)
            Dim Camera1 As devDept.Eyeshot.Camera = New devDept.Eyeshot.Camera(New devDept.Geometry.Point3D(0R, 0R, 45.0R), 380.0R, New devDept.Geometry.Quaternion(0.018434349666532526R, 0.039532590434972079R, 0.42221602280006187R, 0.90544518284475428R), devDept.Graphics.projectionType.Orthographic, 40.0R, 0.8300001177088423R, False, 0.001R)
            Dim HomeToolBarButton1 As devDept.Eyeshot.HomeToolBarButton = New devDept.Eyeshot.HomeToolBarButton("Home", devDept.Eyeshot.ToolBarButton.styleType.PushButton, True, True)
            Dim ZoomToolBarButton1 As devDept.Eyeshot.ZoomToolBarButton = New devDept.Eyeshot.ZoomToolBarButton("Zoom", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
            Dim PanToolBarButton1 As devDept.Eyeshot.PanToolBarButton = New devDept.Eyeshot.PanToolBarButton("Pan", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
            Dim RotateToolBarButton1 As devDept.Eyeshot.RotateToolBarButton = New devDept.Eyeshot.RotateToolBarButton("Rotate", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, True, True)
            Dim ToolBar1 As devDept.Eyeshot.ToolBar = New devDept.Eyeshot.ToolBar(devDept.Eyeshot.ToolBar.positionType.VerticalMiddleLeft, True, New devDept.Eyeshot.ToolBarButton() {CType(HomeToolBarButton1, devDept.Eyeshot.ToolBarButton), CType(ZoomToolBarButton1, devDept.Eyeshot.ToolBarButton), CType(PanToolBarButton1, devDept.Eyeshot.ToolBarButton), CType(RotateToolBarButton1, devDept.Eyeshot.ToolBarButton)})
            Dim RotateSettings1 As devDept.Eyeshot.RotateSettings = New devDept.Eyeshot.RotateSettings(New devDept.Eyeshot.MouseButton(devDept.Eyeshot.mouseButtonsZPR.Middle, devDept.Eyeshot.modifierKeys.None), 10.0R, True, 1.0R, devDept.Eyeshot.rotationType.Trackball, devDept.Eyeshot.rotationCenterType.CursorLocation, New devDept.Geometry.Point3D(0R, 0R, 0R), False)
            Dim ZoomSettings1 As devDept.Eyeshot.ZoomSettings = New devDept.Eyeshot.ZoomSettings(New devDept.Eyeshot.MouseButton(devDept.Eyeshot.mouseButtonsZPR.Middle, devDept.Eyeshot.modifierKeys.Shift), 25, True, devDept.Eyeshot.zoomStyleType.AtCursorLocation, False, 1.0R, System.Drawing.Color.Empty, devDept.Eyeshot.Camera.perspectiveFitType.Accurate, False, 10, True)
            Dim PanSettings1 As devDept.Eyeshot.PanSettings = New devDept.Eyeshot.PanSettings(New devDept.Eyeshot.MouseButton(devDept.Eyeshot.mouseButtonsZPR.Middle, devDept.Eyeshot.modifierKeys.Ctrl), 25, True)
            Dim NavigationSettings1 As devDept.Eyeshot.NavigationSettings = New devDept.Eyeshot.NavigationSettings(devDept.Eyeshot.Camera.navigationType.Examine, New devDept.Eyeshot.MouseButton(devDept.Eyeshot.mouseButtonsZPR.Left, devDept.Eyeshot.modifierKeys.None), New devDept.Geometry.Point3D(-1000.0R, -1000.0R, -1000.0R), New devDept.Geometry.Point3D(1000.0R, 1000.0R, 1000.0R), 8.0R, 50.0R, 50.0R)
            Dim SavedViewsManager1 As devDept.Eyeshot.Viewport.SavedViewsManager = New devDept.Eyeshot.Viewport.SavedViewsManager(8)
            Dim Viewport1 As devDept.Eyeshot.Viewport = New devDept.Eyeshot.Viewport(New System.Drawing.Point(0, 0), New System.Drawing.Size(521, 83), BackgroundSettings1, Camera1, New devDept.Eyeshot.ToolBar() {ToolBar1}, devDept.Eyeshot.displayType.Rendered, True, False, False, False, New devDept.Eyeshot.Grid(-1) {}, New devDept.Eyeshot.OriginSymbol(-1) {}, False, RotateSettings1, ZoomSettings1, PanSettings1, NavigationSettings1, SavedViewsManager1, devDept.Eyeshot.viewType.Trimetric)
            Dim CoordinateSystemIcon1 As devDept.Eyeshot.CoordinateSystemIcon = New devDept.Eyeshot.CoordinateSystemIcon(New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte)), System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.FromArgb(CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer)), System.Drawing.Color.OrangeRed, "Origin", "X", "Y", "Z", True, devDept.Eyeshot.coordinateSystemPositionType.BottomLeft, 37, Nothing, False)
            Dim ViewCubeIcon1 As devDept.Eyeshot.ViewCubeIcon = New devDept.Eyeshot.ViewCubeIcon(devDept.Eyeshot.coordinateSystemPositionType.TopRight, True, System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(20, Byte), Integer), CType(CType(60, Byte), Integer)), True, "Links", "Rechts", "Vorne", "Hinten", "Oben", "Unten", System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(77, Byte), Integer)), Global.Microsoft.VisualBasic.ChrW(83), Global.Microsoft.VisualBasic.ChrW(78), Global.Microsoft.VisualBasic.ChrW(87), Global.Microsoft.VisualBasic.ChrW(69), True, Nothing, System.Drawing.Color.White, System.Drawing.Color.Black, 120, True, True, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, False, New devDept.Geometry.Quaternion(0R, 0R, 0R, 1.0R), True)
            Dim Appearance1 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim UltraGridBand1 As Infragistics.Win.UltraWinGrid.UltraGridBand = New Infragistics.Win.UltraWinGrid.UltraGridBand("CavWires", -1)
            Dim UltraGridColumn3 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("WireName")
            Dim UltraGridColumn4 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("CheckState")
            Dim UltraGridColumn5 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("CavityName", -1, Nothing, 0, Infragistics.Win.UltraWinGrid.SortIndicator.None, False)
            Dim Appearance2 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim Appearance3 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim UltraGridColumn6 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("WireCSA")
            Dim UltraGridColumn7 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("WireType")
            Dim UltraGridColumn8 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("WirePartNumber")
            Dim UltraGridColumn9 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("Color")
            Dim UltraGridColumn10 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("StateImage")
            Dim Appearance4 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim UltraGridColumn11 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("ColorImage")
            Dim Appearance5 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim Appearance6 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim Appearance7 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim Appearance8 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim Appearance9 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim Appearance10 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim Appearance11 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim Appearance12 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim Appearance13 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim Appearance14 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim Appearance15 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim Appearance16 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim UltraTab1 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
            Dim UltraTab2 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
            Dim Appearance17 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim UltraGridBand2 As Infragistics.Win.UltraWinGrid.UltraGridBand = New Infragistics.Win.UltraWinGrid.UltraGridBand("Connectors", -1)
            Dim UltraGridColumn12 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("CavWires")
            Dim UltraGridColumn13 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("CheckState")
            Dim UltraGridColumn15 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("StateImage")
            Dim Appearance18 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim UltraGridColumn16 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("Name", -1, Nothing, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, False)
            Dim UltraGridBand3 As Infragistics.Win.UltraWinGrid.UltraGridBand = New Infragistics.Win.UltraWinGrid.UltraGridBand("CavWires", 0)
            Dim UltraGridColumn17 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("WireName")
            Dim UltraGridColumn25 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("CheckState")
            Dim UltraGridColumn1 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("CavityName")
            Dim UltraGridColumn27 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("WireCSA")
            Dim UltraGridColumn28 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("WireType")
            Dim UltraGridColumn41 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("WirePartNumber")
            Dim UltraGridColumn42 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("Color")
            Dim UltraGridColumn43 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("StateImage")
            Dim UltraGridColumn44 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("ColorImage")
            Dim Appearance19 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim Appearance20 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim Appearance21 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim Appearance22 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim Appearance23 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim Appearance24 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim Appearance25 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim Appearance26 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim Appearance27 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim Appearance28 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim Appearance29 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim PopupMenuTool1 As Infragistics.Win.UltraWinToolbars.PopupMenuTool = New Infragistics.Win.UltraWinToolbars.PopupMenuTool("ctxMenuCavitites")
            Dim ButtonTool12 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("btnJumpTo")
            Dim ButtonTool6 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("btnSelectAll")
            Dim ButtonTool7 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("btnToggleCheckState")
            Dim ButtonTool8 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("btnResetCheckState")
            Dim ButtonToolEditRedlining As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("btnEditRedlining")
            Dim PopupMenuTool2 As Infragistics.Win.UltraWinToolbars.PopupMenuTool = New Infragistics.Win.UltraWinToolbars.PopupMenuTool("ctxMenuConnectors")
            Dim ButtonTool13 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("btnJumpTo")
            Dim ButtonTool9 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("btnToggleCheckState")
            Dim ButtonTool3 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("btnResetCheckState")
            Dim ButtonToolEditRedlining2 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("btnEditRedlining")
            Dim ButtonTool1 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("btnToggleCheckState")
            Dim ButtonTool2 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("btnResetCheckState")
            Dim ButtonTool5 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("btnSelectAll")
            Dim ButtonTool10 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("btnResetZoom")
            Dim PopupMenuTool3 As Infragistics.Win.UltraWinToolbars.PopupMenuTool = New Infragistics.Win.UltraWinToolbars.PopupMenuTool("vdContextMenu")
            Dim ButtonTool11 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("btnResetZoom")
            Dim ButtonTool4 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("btnJumpTo")
            Dim btnToolEditRedlining As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("btnEditRedlining")
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(CavityNavigator))
            Me.UltraTabPageControl2D = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
            Me.vdConnectorView = New VectorDraw.Professional.Control.VectorDrawBaseControl()
            Me.UltraTabPageControl3D = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
            Me.Design3D = New [Lib].Eyeshot.Model.DesignEE()
            Me.Panel1 = New System.Windows.Forms.Panel()
            Me.Panel4 = New System.Windows.Forms.Panel()
            Me.grpConnector = New Infragistics.Win.Misc.UltraGroupBox()
            Me.grpBoxConnector = New Infragistics.Win.Misc.UltraGroupBox()
            Me.UltraSplitter1 = New Infragistics.Win.Misc.UltraSplitter()
            Me.tblWires = New System.Windows.Forms.TableLayoutPanel()
            Me.BindingNavigatorCavities = New System.Windows.Forms.BindingNavigator(Me.components)
            Me.ToolStripButton2 = New System.Windows.Forms.ToolStripButton()
            Me.ToolStripButton3 = New System.Windows.Forms.ToolStripButton()
            Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
            Me.ToolStripComboBox = New System.Windows.Forms.ToolStripComboBox()
            Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
            Me.ToolStripButton4 = New System.Windows.Forms.ToolStripButton()
            Me.ToolStripButton5 = New System.Windows.Forms.ToolStripButton()
            Me.ToolStripCheckButton = New System.Windows.Forms.ToolStripButton()
            Me.Panel3 = New System.Windows.Forms.Panel()
            Me.NaviCavitiesGrid = New Zuken.E3.HarnessAnalyzer.Checks.Cavities.NaviUltraGrid()
            Me.CavWiresBindingSource = New System.Windows.Forms.BindingSource(Me.components)
            Me.ConnectorsBindingSource = New System.Windows.Forms.BindingSource(Me.components)
            Me.Panel5 = New System.Windows.Forms.Panel()
            Me.ultraTabControl = New Infragistics.Win.UltraWinTabControl.UltraTabControl()
            Me.UltraTabSharedControlsPage1 = New Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage()
            Me.BindingNavigator1 = New System.Windows.Forms.BindingNavigator(Me.components)
            Me.Panel2 = New System.Windows.Forms.Panel()
            Me.NaviConnectorsGrid = New Zuken.E3.HarnessAnalyzer.Checks.Cavities.NaviUltraGrid()
            Me.UltraSplitter2 = New Infragistics.Win.Misc.UltraSplitter()
            Me._CavityNavigator_Toolbars_Dock_Area_Left = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
            Me.UltraToolbarsManager1 = New Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(Me.components)
            Me._CavityNavigator_Toolbars_Dock_Area_Right = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
            Me._CavityNavigator_Toolbars_Dock_Area_Top = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
            Me._CavityNavigator_Toolbars_Dock_Area_Bottom = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
            Me.UltraTabPageControl2D.SuspendLayout()
            Me.UltraTabPageControl3D.SuspendLayout()
            CType(Me.Design3D, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.Panel1.SuspendLayout()
            Me.Panel4.SuspendLayout()
            CType(Me.grpConnector, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.grpConnector.SuspendLayout()
            CType(Me.grpBoxConnector, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.grpBoxConnector.SuspendLayout()
            Me.tblWires.SuspendLayout()
            CType(Me.BindingNavigatorCavities, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.BindingNavigatorCavities.SuspendLayout()
            Me.Panel3.SuspendLayout()
            CType(Me.NaviCavitiesGrid, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.CavWiresBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.ConnectorsBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.Panel5.SuspendLayout()
            CType(Me.ultraTabControl, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.ultraTabControl.SuspendLayout()
            CType(Me.BindingNavigator1, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.Panel2.SuspendLayout()
            CType(Me.NaviConnectorsGrid, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.UltraToolbarsManager1, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'UltraTabPageControl2D
            '
            Me.UltraTabPageControl2D.Controls.Add(Me.vdConnectorView)
            resources.ApplyResources(Me.UltraTabPageControl2D, "UltraTabPageControl2D")
            Me.UltraTabPageControl2D.Name = "UltraTabPageControl2D"
            '
            'vdConnectorView
            '
            Me.vdConnectorView.AccessibleRole = System.Windows.Forms.AccessibleRole.Window
            Me.vdConnectorView.AllowDrop = True
            Me.UltraToolbarsManager1.SetContextMenuUltra(Me.vdConnectorView, "vdContextMenu")
            Me.vdConnectorView.Cursor = System.Windows.Forms.Cursors.Default
            resources.ApplyResources(Me.vdConnectorView, "vdConnectorView")
            Me.vdConnectorView.Name = "vdConnectorView"
            '
            'UltraTabPageControl3D
            '
            Me.UltraTabPageControl3D.Controls.Add(Me.Design3D)
            resources.ApplyResources(Me.UltraTabPageControl3D, "UltraTabPageControl3D")
            Me.UltraTabPageControl3D.Name = "UltraTabPageControl3D"
            '
            'Model3D
            '
            Me.Design3D.AllowedSelectionCount = 2147483647
            Me.Design3D.AntiAliasing = True
            Me.Design3D.AntiAliasingSamples = devDept.Graphics.antialiasingSamplesNumberType.x8
            Me.Design3D.AskForAntiAliasing = True
            Me.Design3D.AutoDisableMode = devDept.Eyeshot.DisableSelectionMode.None
            Me.Design3D.BlinkSynchronizer = Nothing
            Me.Design3D.Cursor = System.Windows.Forms.Cursors.Default
            resources.ApplyResources(Me.Design3D, "Model3D")
            Me.Design3D.Name = "Model3D"
            Me.Design3D.PickBoxEnabled = False
            Me.Design3D.ProgressBar = ProgressBar1
            Me.Design3D.Rendered = DisplayModeSettingsRendered1
            Me.Design3D.ShowSilhouetteDynamicPickEntities = devDept.Eyeshot.SilhouetteDynamicPickMode.None
            'Me.Design3D.Snapping = devDept.Eyeshot.Snapping.None
            CoordinateSystemIcon1.LabelFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Viewport1.CoordinateSystemIcon = CoordinateSystemIcon1
            Viewport1.Legends = New devDept.Eyeshot.Legend(-1) {}
            ViewCubeIcon1.Font = Nothing
            ViewCubeIcon1.InitialRotation = New devDept.Geometry.Quaternion(0R, 0R, 0R, 1.0R)
            Viewport1.ViewCubeIcon = ViewCubeIcon1
            Me.Design3D.Viewports.Add(Viewport1)
            '
            'Panel1
            '
            Me.Panel1.Controls.Add(Me.Panel4)
            resources.ApplyResources(Me.Panel1, "Panel1")
            Me.Panel1.Name = "Panel1"
            '
            'Panel4
            '
            Me.Panel4.Controls.Add(Me.grpConnector)
            resources.ApplyResources(Me.Panel4, "Panel4")
            Me.Panel4.Name = "Panel4"
            '
            'grpConnector
            '
            Me.grpConnector.Controls.Add(Me.grpBoxConnector)
            resources.ApplyResources(Me.grpConnector, "grpConnector")
            Me.grpConnector.Name = "grpConnector"
            '
            'grpBoxConnector
            '
            Me.grpBoxConnector.CaptionAlignment = Infragistics.Win.Misc.GroupBoxCaptionAlignment.Near
            Me.grpBoxConnector.Controls.Add(Me.UltraSplitter1)
            Me.grpBoxConnector.Controls.Add(Me.tblWires)
            Me.grpBoxConnector.Controls.Add(Me.Panel5)
            Me.grpBoxConnector.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ConnectorsBindingSource, "Name", True))
            resources.ApplyResources(Me.grpBoxConnector, "grpBoxConnector")
            Me.grpBoxConnector.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder
            Me.grpBoxConnector.Name = "grpBoxConnector"
            '
            'UltraSplitter1
            '
            Me.UltraSplitter1.BackColor = System.Drawing.SystemColors.Control
            resources.ApplyResources(Me.UltraSplitter1, "UltraSplitter1")
            Me.UltraSplitter1.Name = "UltraSplitter1"
            Me.UltraSplitter1.RestoreExtent = 109
            '
            'tblWires
            '
            resources.ApplyResources(Me.tblWires, "tblWires")
            Me.tblWires.Controls.Add(Me.BindingNavigatorCavities, 0, 0)
            Me.tblWires.Controls.Add(Me.Panel3, 0, 1)
            Me.tblWires.Name = "tblWires"
            '
            'BindingNavigatorCavities
            '
            Me.BindingNavigatorCavities.AddNewItem = Nothing
            Me.BindingNavigatorCavities.CountItem = Nothing
            Me.BindingNavigatorCavities.DeleteItem = Nothing
            resources.ApplyResources(Me.BindingNavigatorCavities, "BindingNavigatorCavities")
            Me.BindingNavigatorCavities.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.BindingNavigatorCavities.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripButton2, Me.ToolStripButton3, Me.ToolStripSeparator2, Me.ToolStripComboBox, Me.ToolStripSeparator3, Me.ToolStripButton4, Me.ToolStripButton5, Me.ToolStripCheckButton})
            Me.BindingNavigatorCavities.MoveFirstItem = Me.ToolStripButton2
            Me.BindingNavigatorCavities.MoveLastItem = Me.ToolStripButton5
            Me.BindingNavigatorCavities.MoveNextItem = Me.ToolStripButton4
            Me.BindingNavigatorCavities.MovePreviousItem = Me.ToolStripButton3
            Me.BindingNavigatorCavities.Name = "BindingNavigatorCavities"
            Me.BindingNavigatorCavities.PositionItem = Nothing
            '
            'ToolStripButton2
            '
            Me.ToolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            resources.ApplyResources(Me.ToolStripButton2, "ToolStripButton2")
            Me.ToolStripButton2.Name = "ToolStripButton2"
            '
            'ToolStripButton3
            '
            Me.ToolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            resources.ApplyResources(Me.ToolStripButton3, "ToolStripButton3")
            Me.ToolStripButton3.Name = "ToolStripButton3"
            '
            'ToolStripSeparator2
            '
            Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
            resources.ApplyResources(Me.ToolStripSeparator2, "ToolStripSeparator2")
            '
            'ToolStripComboBox
            '
            Me.ToolStripComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest
            Me.ToolStripComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
            Me.ToolStripComboBox.Name = "ToolStripComboBox"
            resources.ApplyResources(Me.ToolStripComboBox, "ToolStripComboBox")
            '
            'ToolStripSeparator3
            '
            Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
            resources.ApplyResources(Me.ToolStripSeparator3, "ToolStripSeparator3")
            '
            'ToolStripButton4
            '
            Me.ToolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            resources.ApplyResources(Me.ToolStripButton4, "ToolStripButton4")
            Me.ToolStripButton4.Name = "ToolStripButton4"
            '
            'ToolStripButton5
            '
            Me.ToolStripButton5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            resources.ApplyResources(Me.ToolStripButton5, "ToolStripButton5")
            Me.ToolStripButton5.Name = "ToolStripButton5"
            '
            'ToolStripCheckButton
            '
            Me.ToolStripCheckButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.ToolStripCheckButton.Image = Global.Zuken.E3.HarnessAnalyzer.My.Resources.Resources.navigate_check
            resources.ApplyResources(Me.ToolStripCheckButton, "ToolStripCheckButton")
            Me.ToolStripCheckButton.Name = "ToolStripCheckButton"
            '
            'Panel3
            '
            Me.Panel3.Controls.Add(Me.NaviCavitiesGrid)
            resources.ApplyResources(Me.Panel3, "Panel3")
            Me.Panel3.Name = "Panel3"
            '
            'NaviCavitiesGrid
            '
            Me.NaviCavitiesGrid.DataSource = Me.CavWiresBindingSource
            Appearance1.BackColor = System.Drawing.SystemColors.Window
            Appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption
            Me.NaviCavitiesGrid.DisplayLayout.Appearance = Appearance1
            Me.NaviCavitiesGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns
            Me.NaviCavitiesGrid.DisplayLayout.MaxColScrollRegions = 1
            Me.NaviCavitiesGrid.DisplayLayout.MaxRowScrollRegions = 1
            UltraGridColumn3.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit
            resources.ApplyResources(UltraGridColumn3.Header, "UltraGridColumn3.Header")
            UltraGridColumn3.Header.Editor = Nothing
            UltraGridColumn3.Header.VisiblePosition = 3
            resources.ApplyResources(UltraGridColumn3, "UltraGridColumn3")
            UltraGridColumn3.Width = 65
            UltraGridColumn3.ForceApplyResources = "Header|"
            UltraGridColumn4.AutoEditMode = Infragistics.Win.DefaultableBoolean.[False]
            UltraGridColumn4.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.VisibleRows
            resources.ApplyResources(UltraGridColumn4.Header, "UltraGridColumn4.Header")
            UltraGridColumn4.Header.Editor = Nothing
            UltraGridColumn4.Header.VisiblePosition = 1
            UltraGridColumn4.Hidden = True
            UltraGridColumn4.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.TriStateCheckBox
            UltraGridColumn4.Width = 79
            UltraGridColumn4.ForceApplyResources = "Header"
            UltraGridColumn5.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit
            resources.ApplyResources(Appearance2, "Appearance2")
            UltraGridColumn5.CellAppearance = Appearance2
            UltraGridColumn5.GroupByMode = Infragistics.Win.UltraWinGrid.GroupByMode.Value
            resources.ApplyResources(UltraGridColumn5.Header, "UltraGridColumn5.Header")
            UltraGridColumn5.Header.Editor = Nothing
            UltraGridColumn5.Header.VisiblePosition = 0
            resources.ApplyResources(Appearance3, "Appearance3")
            UltraGridColumn5.MergedCellAppearance = Appearance3
            UltraGridColumn5.MergedCellEvaluationType = Infragistics.Win.UltraWinGrid.MergedCellEvaluationType.MergeSameText
            UltraGridColumn5.MergedCellStyle = Infragistics.Win.UltraWinGrid.MergedCellStyle.Always
            UltraGridColumn5.Width = 72
            UltraGridColumn5.ForceApplyResources = "Header"
            resources.ApplyResources(UltraGridColumn6, "UltraGridColumn6")
            resources.ApplyResources(UltraGridColumn6.Header, "UltraGridColumn6.Header")
            UltraGridColumn6.Header.Editor = Nothing
            UltraGridColumn6.Header.VisiblePosition = 4
            UltraGridColumn6.Width = 80
            UltraGridColumn6.ForceApplyResources = "|Header"
            resources.ApplyResources(UltraGridColumn7.Header, "UltraGridColumn7.Header")
            UltraGridColumn7.Header.Editor = Nothing
            UltraGridColumn7.Header.VisiblePosition = 5
            UltraGridColumn7.Width = 57
            UltraGridColumn7.ForceApplyResources = "Header"
            resources.ApplyResources(UltraGridColumn8.Header, "UltraGridColumn8.Header")
            UltraGridColumn8.Header.Editor = Nothing
            UltraGridColumn8.Header.VisiblePosition = 6
            UltraGridColumn8.Width = 83
            UltraGridColumn8.ForceApplyResources = "Header"
            UltraGridColumn9.Header.Editor = Nothing
            UltraGridColumn9.Header.VisiblePosition = 7
            UltraGridColumn9.Width = 57
            resources.ApplyResources(UltraGridColumn9.Header, "UltraGridColumn9.Header")
            UltraGridColumn9.ForceApplyResources = "Header"
            Appearance4.ImageHAlign = Infragistics.Win.HAlign.Center
            UltraGridColumn10.CellAppearance = Appearance4
            resources.ApplyResources(UltraGridColumn10.Header, "UltraGridColumn10.Header")
            UltraGridColumn10.Header.Editor = Nothing
            UltraGridColumn10.Header.VisiblePosition = 2
            UltraGridColumn10.IgnoreMultiCellOperation = Infragistics.Win.DefaultableBoolean.[True]
            UltraGridColumn10.Width = 46
            UltraGridColumn10.ForceApplyResources = "Header"
            Appearance5.ImageHAlign = Infragistics.Win.HAlign.Center
            Appearance5.ImageVAlign = Infragistics.Win.VAlign.Middle
            UltraGridColumn11.CellAppearance = Appearance5
            resources.ApplyResources(UltraGridColumn11.Header, "UltraGridColumn11.Header")
            UltraGridColumn11.Header.Editor = Nothing
            UltraGridColumn11.Header.VisiblePosition = 8
            UltraGridColumn11.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image
            UltraGridColumn11.Width = 71
            UltraGridColumn11.ForceApplyResources = "Header"
            UltraGridBand1.Columns.AddRange(New Object() {UltraGridColumn3, UltraGridColumn4, UltraGridColumn5, UltraGridColumn6, UltraGridColumn7, UltraGridColumn8, UltraGridColumn9, UltraGridColumn10, UltraGridColumn11})
            UltraGridBand1.Override.AllowColMoving = Infragistics.Win.UltraWinGrid.AllowColMoving.NotAllowed
            UltraGridBand1.Override.AllowGroupBy = Infragistics.Win.DefaultableBoolean.[False]
            UltraGridBand1.Override.AllowMultiCellOperations = Infragistics.Win.UltraWinGrid.AllowMultiCellOperation.Copy
            UltraGridBand1.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect
            UltraGridBand1.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.[Select]
            Me.NaviCavitiesGrid.DisplayLayout.BandsSerializer.Add(UltraGridBand1)
            Me.NaviCavitiesGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
            Me.NaviCavitiesGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.[False]
            Appearance6.BackColor = System.Drawing.SystemColors.ActiveBorder
            Appearance6.BackColor2 = System.Drawing.SystemColors.ControlDark
            Appearance6.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical
            Appearance6.BorderColor = System.Drawing.SystemColors.Window
            Me.NaviCavitiesGrid.DisplayLayout.GroupByBox.Appearance = Appearance6
            Appearance7.ForeColor = System.Drawing.SystemColors.GrayText
            Me.NaviCavitiesGrid.DisplayLayout.GroupByBox.BandLabelAppearance = Appearance7
            Me.NaviCavitiesGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
            Appearance8.BackColor = System.Drawing.SystemColors.ControlLightLight
            Appearance8.BackColor2 = System.Drawing.SystemColors.Control
            Appearance8.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
            Appearance8.ForeColor = System.Drawing.SystemColors.GrayText
            Me.NaviCavitiesGrid.DisplayLayout.GroupByBox.PromptAppearance = Appearance8
            Me.NaviCavitiesGrid.DisplayLayout.LoadStyle = Infragistics.Win.UltraWinGrid.LoadStyle.LoadOnDemand
            Me.NaviCavitiesGrid.DisplayLayout.MaxRowScrollRegions = 1
            Appearance9.BackColor = System.Drawing.SystemColors.Window
            Appearance9.ForeColor = System.Drawing.SystemColors.ControlText
            Me.NaviCavitiesGrid.DisplayLayout.Override.ActiveCellAppearance = Appearance9
            Appearance10.BackColor = System.Drawing.SystemColors.Highlight
            Appearance10.ForeColor = System.Drawing.SystemColors.HighlightText
            Me.NaviCavitiesGrid.DisplayLayout.Override.ActiveRowAppearance = Appearance10
            Me.NaviCavitiesGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No
            Me.NaviCavitiesGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.[False]
            Me.NaviCavitiesGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.[False]
            Me.NaviCavitiesGrid.DisplayLayout.Override.AutoEditMode = Infragistics.Win.DefaultableBoolean.[False]
            Me.NaviCavitiesGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted
            Me.NaviCavitiesGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted
            Appearance11.BackColor = System.Drawing.SystemColors.Window
            Me.NaviCavitiesGrid.DisplayLayout.Override.CardAreaAppearance = Appearance11
            Appearance12.BorderColor = System.Drawing.Color.Silver
            Appearance12.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter
            Me.NaviCavitiesGrid.DisplayLayout.Override.CellAppearance = Appearance12
            Me.NaviCavitiesGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect
            Me.NaviCavitiesGrid.DisplayLayout.Override.CellPadding = 0
            Me.NaviCavitiesGrid.DisplayLayout.Override.ColumnAutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.AllRowsInBand
            Me.NaviCavitiesGrid.DisplayLayout.Override.ColumnHeaderTextOrientation = New Infragistics.Win.TextOrientationInfo(0, Infragistics.Win.TextFlowDirection.Horizontal)
            Me.NaviCavitiesGrid.DisplayLayout.Override.ColumnSizingArea = Infragistics.Win.UltraWinGrid.ColumnSizingArea.EntireColumn
            Appearance13.BackColor = System.Drawing.SystemColors.Control
            Appearance13.BackColor2 = System.Drawing.SystemColors.ControlDark
            Appearance13.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element
            Appearance13.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
            Appearance13.BorderColor = System.Drawing.SystemColors.Window
            Me.NaviCavitiesGrid.DisplayLayout.Override.GroupByRowAppearance = Appearance13
            resources.ApplyResources(Appearance14, "Appearance14")
            Me.NaviCavitiesGrid.DisplayLayout.Override.HeaderAppearance = Appearance14
            Me.NaviCavitiesGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti
            Me.NaviCavitiesGrid.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand
            Appearance15.BackColor = System.Drawing.SystemColors.Window
            Appearance15.BorderColor = System.Drawing.Color.Silver
            Me.NaviCavitiesGrid.DisplayLayout.Override.RowAppearance = Appearance15
            Me.NaviCavitiesGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.[False]
            Me.NaviCavitiesGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None
            Me.NaviCavitiesGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None
            Appearance16.BackColor = System.Drawing.SystemColors.ControlLight
            Me.NaviCavitiesGrid.DisplayLayout.Override.TemplateAddRowAppearance = Appearance16
            Me.NaviCavitiesGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill
            Me.NaviCavitiesGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate
            Me.NaviCavitiesGrid.DisplayLayout.TabNavigation = Infragistics.Win.UltraWinGrid.TabNavigation.NextControl
            Me.NaviCavitiesGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand
            resources.ApplyResources(Me.NaviCavitiesGrid, "NaviCavitiesGrid")
            Me.NaviCavitiesGrid.Name = "NaviCavitiesGrid"
            '
            'CavWiresBindingSource
            '
            Me.CavWiresBindingSource.DataMember = "CavWires"
            Me.CavWiresBindingSource.DataSource = Me.ConnectorsBindingSource
            '
            'ConnectorsBindingSource
            '
            Me.ConnectorsBindingSource.DataMember = "Connectors"
            Me.ConnectorsBindingSource.DataSource = GetType(Zuken.E3.HarnessAnalyzer.Checks.Cavities.Views.Model.ModelView)
            '
            'Panel5
            '
            Me.Panel5.Controls.Add(Me.ultraTabControl)
            resources.ApplyResources(Me.Panel5, "Panel5")
            Me.Panel5.Name = "Panel5"
            '
            'ultraTabControl
            '
            Me.ultraTabControl.Controls.Add(Me.UltraTabSharedControlsPage1)
            Me.ultraTabControl.Controls.Add(Me.UltraTabPageControl2D)
            Me.ultraTabControl.Controls.Add(Me.UltraTabPageControl3D)
            resources.ApplyResources(Me.ultraTabControl, "ultraTabControl")
            Me.ultraTabControl.Name = "ultraTabControl"
            Me.ultraTabControl.SharedControlsPage = Me.UltraTabSharedControlsPage1
            UltraTab1.Key = "D2D"
            UltraTab1.TabPage = Me.UltraTabPageControl2D
            resources.ApplyResources(UltraTab1, "UltraTab1")
            UltraTab1.ForceApplyResources = ""
            UltraTab2.Key = "D3D"
            UltraTab2.TabPage = Me.UltraTabPageControl3D
            resources.ApplyResources(UltraTab2, "UltraTab2")
            UltraTab2.ForceApplyResources = ""
            Me.ultraTabControl.Tabs.AddRange(New Infragistics.Win.UltraWinTabControl.UltraTab() {UltraTab1, UltraTab2})
            '
            'UltraTabSharedControlsPage1
            '
            resources.ApplyResources(Me.UltraTabSharedControlsPage1, "UltraTabSharedControlsPage1")
            Me.UltraTabSharedControlsPage1.Name = "UltraTabSharedControlsPage1"
            '
            'BindingNavigator1
            '
            Me.BindingNavigator1.AddNewItem = Nothing
            Me.BindingNavigator1.CountItem = Nothing
            Me.BindingNavigator1.DeleteItem = Nothing
            resources.ApplyResources(Me.BindingNavigator1, "BindingNavigator1")
            Me.BindingNavigator1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.BindingNavigator1.MoveFirstItem = Nothing
            Me.BindingNavigator1.MoveLastItem = Nothing
            Me.BindingNavigator1.MoveNextItem = Nothing
            Me.BindingNavigator1.MovePreviousItem = Nothing
            Me.BindingNavigator1.Name = "BindingNavigator1"
            Me.BindingNavigator1.PositionItem = Nothing
            '
            'Panel2
            '
            Me.Panel2.Controls.Add(Me.NaviConnectorsGrid)
            resources.ApplyResources(Me.Panel2, "Panel2")
            Me.Panel2.Name = "Panel2"
            '
            'NaviConnectorsGrid
            '
            Me.UltraToolbarsManager1.SetContextMenuUltra(Me.NaviConnectorsGrid, "ctxMenuConnectors")
            Me.NaviConnectorsGrid.DataSource = Me.ConnectorsBindingSource
            Appearance17.BackColor = System.Drawing.SystemColors.Window
            Appearance17.BorderColor = System.Drawing.SystemColors.InactiveCaption
            Me.NaviConnectorsGrid.DisplayLayout.Appearance = Appearance17
            Me.NaviConnectorsGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn
            Me.NaviConnectorsGrid.DisplayLayout.MaxRowScrollRegions = 1
            Me.NaviConnectorsGrid.DisplayLayout.MaxColScrollRegions = 1
            UltraGridColumn12.Header.Editor = Nothing
            UltraGridColumn12.Header.VisiblePosition = 3
            UltraGridColumn12.SortIndicator = Infragistics.Win.UltraWinGrid.SortIndicator.Disabled
            UltraGridColumn13.Header.Editor = Nothing
            UltraGridColumn13.Header.VisiblePosition = 1
            UltraGridColumn13.Hidden = True
            resources.ApplyResources(UltraGridColumn13.Header, "UltraGridColumn13.Header")
            UltraGridColumn13.ForceApplyResources = "Header"
            Appearance18.ImageHAlign = Infragistics.Win.HAlign.Center
            UltraGridColumn15.CellAppearance = Appearance18
            resources.ApplyResources(UltraGridColumn15.Header, "UltraGridColumn15.Header")
            UltraGridColumn15.Header.Editor = Nothing
            UltraGridColumn15.Header.VisiblePosition = 0
            UltraGridColumn15.IgnoreMultiCellOperation = Infragistics.Win.DefaultableBoolean.[True]
            UltraGridColumn15.ForceApplyResources = "Header"
            UltraGridColumn16.Header.Editor = Nothing
            UltraGridColumn16.Header.VisiblePosition = 2
            UltraGridColumn16.InitialSortDirection = Infragistics.Win.UltraWinGrid.SortDirection.Ascending
            UltraGridBand2.Columns.AddRange(New Object() {UltraGridColumn12, UltraGridColumn13, UltraGridColumn15, UltraGridColumn16})
            UltraGridBand2.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortSingle
            UltraGridColumn17.Header.Editor = Nothing
            UltraGridColumn17.Header.VisiblePosition = 0
            resources.ApplyResources(UltraGridColumn17.Header, "UltraGridColumn17.Header")
            UltraGridColumn17.ForceApplyResources = "Header"
            UltraGridColumn25.Header.Editor = Nothing
            UltraGridColumn25.Header.VisiblePosition = 1
            resources.ApplyResources(UltraGridColumn25.Header, "UltraGridColumn25.Header")
            UltraGridColumn25.ForceApplyResources = "Header"
            UltraGridColumn1.Header.Editor = Nothing
            UltraGridColumn1.Header.VisiblePosition = 2
            UltraGridColumn27.Header.Editor = Nothing
            UltraGridColumn27.Header.VisiblePosition = 3
            resources.ApplyResources(UltraGridColumn27.Header, "UltraGridColumn27.Header")
            UltraGridColumn27.ForceApplyResources = "Header"
            UltraGridColumn28.Header.Editor = Nothing
            UltraGridColumn28.Header.VisiblePosition = 5
            resources.ApplyResources(UltraGridColumn28.Header, "UltraGridColumn28.Header")
            UltraGridColumn28.ForceApplyResources = "Header"
            UltraGridColumn41.Header.Editor = Nothing
            UltraGridColumn41.Header.VisiblePosition = 6
            resources.ApplyResources(UltraGridColumn41.Header, "UltraGridColumn41.Header")
            UltraGridColumn41.ForceApplyResources = "Header"
            UltraGridColumn42.Header.Editor = Nothing
            UltraGridColumn42.Header.VisiblePosition = 7
            resources.ApplyResources(UltraGridColumn42.Header, "UltraGridColumn42.Header")
            UltraGridColumn42.ForceApplyResources = "Header"
            UltraGridColumn43.Header.Editor = Nothing
            UltraGridColumn43.Header.VisiblePosition = 4
            resources.ApplyResources(UltraGridColumn43.Header, "UltraGridColumn43.Header")
            UltraGridColumn43.ForceApplyResources = "Header"
            UltraGridColumn44.Header.Editor = Nothing
            UltraGridColumn44.Header.VisiblePosition = 8
            UltraGridBand3.Columns.AddRange(New Object() {UltraGridColumn17, UltraGridColumn25, UltraGridColumn1, UltraGridColumn27, UltraGridColumn28, UltraGridColumn41, UltraGridColumn42, UltraGridColumn43, UltraGridColumn44})
            Me.NaviConnectorsGrid.DisplayLayout.BandsSerializer.Add(UltraGridBand2)
            Me.NaviConnectorsGrid.DisplayLayout.BandsSerializer.Add(UltraGridBand3)
            Me.NaviConnectorsGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
            Me.NaviConnectorsGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.[False]
            Appearance19.BackColor = System.Drawing.SystemColors.ActiveBorder
            Appearance19.BackColor2 = System.Drawing.SystemColors.ControlDark
            Appearance19.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical
            Appearance19.BorderColor = System.Drawing.SystemColors.Window
            Me.NaviConnectorsGrid.DisplayLayout.GroupByBox.Appearance = Appearance19
            Appearance20.ForeColor = System.Drawing.SystemColors.GrayText
            Me.NaviConnectorsGrid.DisplayLayout.GroupByBox.BandLabelAppearance = Appearance20
            Me.NaviConnectorsGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
            Appearance21.BackColor = System.Drawing.SystemColors.ControlLightLight
            Appearance21.BackColor2 = System.Drawing.SystemColors.Control
            Appearance21.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
            Appearance21.ForeColor = System.Drawing.SystemColors.GrayText
            Me.NaviConnectorsGrid.DisplayLayout.GroupByBox.PromptAppearance = Appearance21
            Me.NaviConnectorsGrid.DisplayLayout.MaxRowScrollRegions = 1
            Appearance22.BackColor = System.Drawing.SystemColors.Window
            Appearance22.ForeColor = System.Drawing.SystemColors.ControlText
            Me.NaviConnectorsGrid.DisplayLayout.Override.ActiveCellAppearance = Appearance22
            Appearance23.BackColor = System.Drawing.SystemColors.Highlight
            Appearance23.ForeColor = System.Drawing.SystemColors.HighlightText
            Me.NaviConnectorsGrid.DisplayLayout.Override.ActiveRowAppearance = Appearance23
            Me.NaviConnectorsGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No
            Me.NaviConnectorsGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.[False]
            Me.NaviConnectorsGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.[False]
            Me.NaviConnectorsGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted
            Me.NaviConnectorsGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted
            Appearance24.BackColor = System.Drawing.SystemColors.Window
            Me.NaviConnectorsGrid.DisplayLayout.Override.CardAreaAppearance = Appearance24
            Appearance25.BorderColor = System.Drawing.Color.Silver
            Appearance25.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter
            Me.NaviConnectorsGrid.DisplayLayout.Override.CellAppearance = Appearance25
            Me.NaviConnectorsGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect
            Me.NaviConnectorsGrid.DisplayLayout.Override.CellPadding = 0
            Me.NaviConnectorsGrid.DisplayLayout.Override.ColumnAutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.AllRowsInBand
            Appearance26.BackColor = System.Drawing.SystemColors.Control
            Appearance26.BackColor2 = System.Drawing.SystemColors.ControlDark
            Appearance26.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element
            Appearance26.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
            Appearance26.BorderColor = System.Drawing.SystemColors.Window
            Me.NaviConnectorsGrid.DisplayLayout.Override.GroupByRowAppearance = Appearance26
            resources.ApplyResources(Appearance27, "Appearance27")
            Me.NaviConnectorsGrid.DisplayLayout.Override.HeaderAppearance = Appearance27
            Me.NaviConnectorsGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti
            Me.NaviConnectorsGrid.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand
            Appearance28.BackColor = System.Drawing.SystemColors.Window
            Appearance28.BorderColor = System.Drawing.Color.Silver
            Me.NaviConnectorsGrid.DisplayLayout.Override.RowAppearance = Appearance28
            Me.NaviConnectorsGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.[False]
            Me.NaviConnectorsGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None
            Me.NaviConnectorsGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None
            Me.NaviConnectorsGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.[Single]
            Appearance29.BackColor = System.Drawing.SystemColors.ControlLight
            Me.NaviConnectorsGrid.DisplayLayout.Override.TemplateAddRowAppearance = Appearance29
            Me.NaviConnectorsGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill
            Me.NaviConnectorsGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate
            Me.NaviConnectorsGrid.DisplayLayout.TabNavigation = Infragistics.Win.UltraWinGrid.TabNavigation.NextControl
            Me.NaviConnectorsGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand
            resources.ApplyResources(Me.NaviConnectorsGrid, "NaviConnectorsGrid")
            Me.NaviConnectorsGrid.Name = "NaviConnectorsGrid"
            '
            'UltraSplitter2
            '
            Me.UltraSplitter2.BackColor = System.Drawing.SystemColors.Control
            resources.ApplyResources(Me.UltraSplitter2, "UltraSplitter2")
            Me.UltraSplitter2.Name = "UltraSplitter2"
            Me.UltraSplitter2.RestoreExtent = 237
            '
            '_CavityNavigator_Toolbars_Dock_Area_Left
            '
            Me._CavityNavigator_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
            Me._CavityNavigator_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control
            Me._CavityNavigator_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left
            Me._CavityNavigator_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText
            resources.ApplyResources(Me._CavityNavigator_Toolbars_Dock_Area_Left, "_CavityNavigator_Toolbars_Dock_Area_Left")
            Me._CavityNavigator_Toolbars_Dock_Area_Left.Name = "_CavityNavigator_Toolbars_Dock_Area_Left"
            Me._CavityNavigator_Toolbars_Dock_Area_Left.ToolbarsManager = Me.UltraToolbarsManager1
            '
            'UltraToolbarsManager1
            '
            Me.UltraToolbarsManager1.DesignerFlags = 1
            Me.UltraToolbarsManager1.DockWithinContainer = Me
            Me.UltraToolbarsManager1.ShowFullMenusDelay = 500
            resources.ApplyResources(PopupMenuTool1.SharedPropsInternal, "PopupMenuTool1.SharedPropsInternal")
            ButtonTool6.InstanceProps.IsFirstInGroup = True
            ButtonTool7.InstanceProps.IsFirstInGroup = True
            PopupMenuTool1.Tools.AddRange(New Infragistics.Win.UltraWinToolbars.ToolBase() {ButtonTool12, ButtonTool6, ButtonTool7, ButtonTool8, ButtonToolEditRedlining})
            PopupMenuTool1.ForceApplyResources = "SharedPropsInternal"
            resources.ApplyResources(PopupMenuTool2.SharedPropsInternal, "PopupMenuTool2.SharedPropsInternal")
            ButtonTool9.InstanceProps.IsFirstInGroup = True
            PopupMenuTool2.Tools.AddRange(New Infragistics.Win.UltraWinToolbars.ToolBase() {ButtonTool13, ButtonTool9, ButtonTool3, ButtonToolEditRedlining2})
            PopupMenuTool2.ForceApplyResources = "SharedPropsInternal"
            resources.ApplyResources(ButtonTool1.SharedPropsInternal, "ButtonTool1.SharedPropsInternal")
            ButtonTool1.SharedPropsInternal.Spring = True
            ButtonTool1.ForceApplyResources = "SharedPropsInternal"
            resources.ApplyResources(ButtonTool2.SharedPropsInternal, "ButtonTool2.SharedPropsInternal")
            ButtonTool2.ForceApplyResources = "SharedPropsInternal"
            resources.ApplyResources(ButtonTool5.SharedPropsInternal, "ButtonTool5.SharedPropsInternal")
            ButtonTool5.ForceApplyResources = "SharedPropsInternal"
            resources.ApplyResources(ButtonTool10.SharedPropsInternal, "ButtonTool10.SharedPropsInternal")
            ButtonTool10.ForceApplyResources = "SharedPropsInternal"
            resources.ApplyResources(PopupMenuTool3.SharedPropsInternal, "PopupMenuTool3.SharedPropsInternal")
            PopupMenuTool3.Tools.AddRange(New Infragistics.Win.UltraWinToolbars.ToolBase() {ButtonTool11})
            PopupMenuTool3.ForceApplyResources = "SharedPropsInternal"
            resources.ApplyResources(ButtonTool4.SharedPropsInternal, "ButtonTool4.SharedPropsInternal")
            ButtonTool4.ForceApplyResources = "SharedPropsInternal"
            resources.ApplyResources(btnToolEditRedlining.SharedPropsInternal, "btnToolEditRedlining.SharedPropsInternal")
            btnToolEditRedlining.ForceApplyResources = "SharedPropsInternal"
            Me.UltraToolbarsManager1.Tools.AddRange(New Infragistics.Win.UltraWinToolbars.ToolBase() {PopupMenuTool1, PopupMenuTool2, ButtonTool1, ButtonTool2, ButtonTool5, ButtonTool10, PopupMenuTool3, ButtonTool4, btnToolEditRedlining})
            '
            '_CavityNavigator_Toolbars_Dock_Area_Right
            '
            Me._CavityNavigator_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
            Me._CavityNavigator_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control
            Me._CavityNavigator_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right
            Me._CavityNavigator_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText
            resources.ApplyResources(Me._CavityNavigator_Toolbars_Dock_Area_Right, "_CavityNavigator_Toolbars_Dock_Area_Right")
            Me._CavityNavigator_Toolbars_Dock_Area_Right.Name = "_CavityNavigator_Toolbars_Dock_Area_Right"
            Me._CavityNavigator_Toolbars_Dock_Area_Right.ToolbarsManager = Me.UltraToolbarsManager1
            '
            '_CavityNavigator_Toolbars_Dock_Area_Top
            '
            Me._CavityNavigator_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
            Me._CavityNavigator_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control
            Me._CavityNavigator_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top
            Me._CavityNavigator_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText
            resources.ApplyResources(Me._CavityNavigator_Toolbars_Dock_Area_Top, "_CavityNavigator_Toolbars_Dock_Area_Top")
            Me._CavityNavigator_Toolbars_Dock_Area_Top.Name = "_CavityNavigator_Toolbars_Dock_Area_Top"
            Me._CavityNavigator_Toolbars_Dock_Area_Top.ToolbarsManager = Me.UltraToolbarsManager1
            '
            '_CavityNavigator_Toolbars_Dock_Area_Bottom
            '
            Me._CavityNavigator_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
            Me._CavityNavigator_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control
            Me._CavityNavigator_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom
            Me._CavityNavigator_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText
            resources.ApplyResources(Me._CavityNavigator_Toolbars_Dock_Area_Bottom, "_CavityNavigator_Toolbars_Dock_Area_Bottom")
            Me._CavityNavigator_Toolbars_Dock_Area_Bottom.Name = "_CavityNavigator_Toolbars_Dock_Area_Bottom"
            Me._CavityNavigator_Toolbars_Dock_Area_Bottom.ToolbarsManager = Me.UltraToolbarsManager1
            '
            'CavityNavigator
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.Panel1)
            Me.Controls.Add(Me.UltraSplitter2)
            Me.Controls.Add(Me.Panel2)
            Me.Controls.Add(Me._CavityNavigator_Toolbars_Dock_Area_Left)
            Me.Controls.Add(Me._CavityNavigator_Toolbars_Dock_Area_Right)
            Me.Controls.Add(Me._CavityNavigator_Toolbars_Dock_Area_Bottom)
            Me.Controls.Add(Me._CavityNavigator_Toolbars_Dock_Area_Top)
            Me.Name = "CavityNavigator"
            Me.UltraTabPageControl2D.ResumeLayout(False)
            Me.UltraTabPageControl3D.ResumeLayout(False)
            CType(Me.Design3D, System.ComponentModel.ISupportInitialize).EndInit()
            Me.Panel1.ResumeLayout(False)
            Me.Panel4.ResumeLayout(False)
            CType(Me.grpConnector, System.ComponentModel.ISupportInitialize).EndInit()
            Me.grpConnector.ResumeLayout(False)
            CType(Me.grpBoxConnector, System.ComponentModel.ISupportInitialize).EndInit()
            Me.grpBoxConnector.ResumeLayout(False)
            Me.tblWires.ResumeLayout(False)
            Me.tblWires.PerformLayout()
            CType(Me.BindingNavigatorCavities, System.ComponentModel.ISupportInitialize).EndInit()
            Me.BindingNavigatorCavities.ResumeLayout(False)
            Me.BindingNavigatorCavities.PerformLayout()
            Me.Panel3.ResumeLayout(False)
            CType(Me.NaviCavitiesGrid, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.CavWiresBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.ConnectorsBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
            Me.Panel5.ResumeLayout(False)
            CType(Me.ultraTabControl, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ultraTabControl.ResumeLayout(False)
            CType(Me.BindingNavigator1, System.ComponentModel.ISupportInitialize).EndInit()
            Me.Panel2.ResumeLayout(False)
            CType(Me.NaviConnectorsGrid, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.UltraToolbarsManager1, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents Panel1 As Panel
        Friend WithEvents BindingNavigator1 As BindingNavigator
        Friend WithEvents Panel2 As Panel
        Friend WithEvents UltraSplitter2 As Infragistics.Win.Misc.UltraSplitter
        Friend WithEvents ConnectorsBindingSource As BindingSource
        Friend WithEvents NaviConnectorsGrid As NaviUltraGrid
        Friend WithEvents CavWiresBindingSource As BindingSource
        Friend WithEvents Panel4 As Panel
        Protected Friend WithEvents tblWires As TableLayoutPanel
        Protected Friend WithEvents BindingNavigatorCavities As BindingNavigator
        Friend WithEvents ToolStripButton2 As ToolStripButton
        Friend WithEvents ToolStripButton3 As ToolStripButton
        Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
        Friend WithEvents ToolStripComboBox As ToolStripComboBox
        Friend WithEvents ToolStripSeparator3 As ToolStripSeparator
        Friend WithEvents ToolStripButton4 As ToolStripButton
        Friend WithEvents ToolStripButton5 As ToolStripButton
        Friend WithEvents ToolStripCheckButton As ToolStripButton
        Friend WithEvents Panel3 As Panel
        Friend WithEvents Panel5 As Panel
        Friend WithEvents NaviCavitiesGrid As NaviUltraGrid
        Friend WithEvents grpConnector As Infragistics.Win.Misc.UltraGroupBox
        Friend WithEvents grpBoxConnector As Infragistics.Win.Misc.UltraGroupBox
        Friend WithEvents UltraSplitter1 As Infragistics.Win.Misc.UltraSplitter
        Friend WithEvents vdConnectorView As VectorDraw.Professional.Control.VectorDrawBaseControl
        Friend WithEvents UltraToolbarsManager1 As Infragistics.Win.UltraWinToolbars.UltraToolbarsManager
        Friend WithEvents _CavityNavigator_Toolbars_Dock_Area_Left As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
        Friend WithEvents _CavityNavigator_Toolbars_Dock_Area_Right As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
        Friend WithEvents _CavityNavigator_Toolbars_Dock_Area_Bottom As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
        Friend WithEvents _CavityNavigator_Toolbars_Dock_Area_Top As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
        Friend WithEvents ultraTabControl As Infragistics.Win.UltraWinTabControl.UltraTabControl
        Friend WithEvents UltraTabSharedControlsPage1 As Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage
        Friend WithEvents UltraTabPageControl2D As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
        Friend WithEvents UltraTabPageControl3D As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
        Friend WithEvents Design3D As [Lib].Eyeshot.Model.DesignEE
    End Class

End Namespace