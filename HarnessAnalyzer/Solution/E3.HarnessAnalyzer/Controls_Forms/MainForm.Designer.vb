Imports Zuken.E3.HarnessAnalyzer.Checks.Cavities.Views.Document

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing Then
                If components IsNot Nothing Then
                    components.Dispose()
                End If

                If _toastManager IsNot Nothing Then
                    _toastManager.Dispose()
                End If

                If MainFormController IsNot Nothing Then
                    MainFormController.Dispose()
                End If

                If _documentViews IsNot Nothing Then
                    For Each dView As DocumentView In _documentViews
                        dView.Dispose()
                    Next
                End If

                _panes?.Dispose()
                _project.SynchronizationContext = Nothing
                _project.Dispose()
                _mainStateMachine?.XHcvFile?.Dispose()
            End If

            _panes = Nothing
            _project = Nothing
            _documentViews = Nothing
            _toastManager = Nothing
            MainFormController = Nothing
            Application.RemoveMessageFilter(Me)
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
        components = New ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Dim Appearance1 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance2 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim UltraStatusPanel1 As Infragistics.Win.UltraWinStatusBar.UltraStatusPanel = New Infragistics.Win.UltraWinStatusBar.UltraStatusPanel()
        Dim UltraStatusPanel2 As Infragistics.Win.UltraWinStatusBar.UltraStatusPanel = New Infragistics.Win.UltraWinStatusBar.UltraStatusPanel()
        Dim UltraStatusPanel3 As Infragistics.Win.UltraWinStatusBar.UltraStatusPanel = New Infragistics.Win.UltraWinStatusBar.UltraStatusPanel()
        MainForm_Toolbars_Dock_Area_Left = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        utmMain = New Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(components)
        MainForm_Toolbars_Dock_Area_Right = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        MainForm_Toolbars_Dock_Area_Top = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        MainForm_Toolbars_Dock_Area_Bottom = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        utmmMain = New Infragistics.Win.UltraWinTabbedMdi.UltraTabbedMdiManager(components)
        usbMain = New Infragistics.Win.UltraWinStatusBar.UltraStatusBar()
        udmMain = New Infragistics.Win.UltraWinDock.UltraDockManager(components)
        _BrowserFormUnpinnedTabAreaLeft = New Infragistics.Win.UltraWinDock.UnpinnedTabArea()
        _BrowserFormUnpinnedTabAreaRight = New Infragistics.Win.UltraWinDock.UnpinnedTabArea()
        _BrowserFormUnpinnedTabAreaTop = New Infragistics.Win.UltraWinDock.UnpinnedTabArea()
        _BrowserFormUnpinnedTabAreaBottom = New Infragistics.Win.UltraWinDock.UnpinnedTabArea()
        _BrowserFormAutoHideControl = New Infragistics.Win.UltraWinDock.AutoHideControl()
        CType(utmMain, ComponentModel.ISupportInitialize).BeginInit()
        CType(utmmMain, ComponentModel.ISupportInitialize).BeginInit()
        CType(usbMain, ComponentModel.ISupportInitialize).BeginInit()
        CType(udmMain, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' MainForm_Toolbars_Dock_Area_Left
        ' 
        MainForm_Toolbars_Dock_Area_Left.AccessibleRole = AccessibleRole.Grouping
        MainForm_Toolbars_Dock_Area_Left.BackColor = Color.FromArgb(CByte(191), CByte(219), CByte(255))
        MainForm_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left
        MainForm_Toolbars_Dock_Area_Left.ForeColor = SystemColors.ControlText
        MainForm_Toolbars_Dock_Area_Left.InitialResizeAreaExtent = 8
        resources.ApplyResources(MainForm_Toolbars_Dock_Area_Left, "MainForm_Toolbars_Dock_Area_Left")
        MainForm_Toolbars_Dock_Area_Left.Name = "MainForm_Toolbars_Dock_Area_Left"
        MainForm_Toolbars_Dock_Area_Left.ToolbarsManager = utmMain
        ' 
        ' utmMain
        ' 
        utmMain.DesignerFlags = 1
        utmMain.DockWithinContainer = Me
        utmMain.DockWithinContainerBaseType = GetType(Form)
        utmMain.Ribbon.Visible = True
        utmMain.ShowFullMenusDelay = 500
        utmMain.Style = Infragistics.Win.UltraWinToolbars.ToolbarStyle.Office2010
        ' 
        ' MainForm_Toolbars_Dock_Area_Right
        ' 
        MainForm_Toolbars_Dock_Area_Right.AccessibleRole = AccessibleRole.Grouping
        MainForm_Toolbars_Dock_Area_Right.BackColor = Color.FromArgb(CByte(191), CByte(219), CByte(255))
        MainForm_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right
        MainForm_Toolbars_Dock_Area_Right.ForeColor = SystemColors.ControlText
        MainForm_Toolbars_Dock_Area_Right.InitialResizeAreaExtent = 8
        resources.ApplyResources(MainForm_Toolbars_Dock_Area_Right, "MainForm_Toolbars_Dock_Area_Right")
        MainForm_Toolbars_Dock_Area_Right.Name = "MainForm_Toolbars_Dock_Area_Right"
        MainForm_Toolbars_Dock_Area_Right.ToolbarsManager = utmMain
        ' 
        ' MainForm_Toolbars_Dock_Area_Top
        ' 
        MainForm_Toolbars_Dock_Area_Top.AccessibleRole = AccessibleRole.Grouping
        MainForm_Toolbars_Dock_Area_Top.BackColor = Color.FromArgb(CByte(191), CByte(219), CByte(255))
        MainForm_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top
        MainForm_Toolbars_Dock_Area_Top.ForeColor = SystemColors.ControlText
        resources.ApplyResources(MainForm_Toolbars_Dock_Area_Top, "MainForm_Toolbars_Dock_Area_Top")
        MainForm_Toolbars_Dock_Area_Top.Name = "MainForm_Toolbars_Dock_Area_Top"
        MainForm_Toolbars_Dock_Area_Top.ToolbarsManager = utmMain
        ' 
        ' MainForm_Toolbars_Dock_Area_Bottom
        ' 
        MainForm_Toolbars_Dock_Area_Bottom.AccessibleRole = AccessibleRole.Grouping
        MainForm_Toolbars_Dock_Area_Bottom.BackColor = Color.FromArgb(CByte(191), CByte(219), CByte(255))
        MainForm_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom
        MainForm_Toolbars_Dock_Area_Bottom.ForeColor = SystemColors.ControlText
        resources.ApplyResources(MainForm_Toolbars_Dock_Area_Bottom, "MainForm_Toolbars_Dock_Area_Bottom")
        MainForm_Toolbars_Dock_Area_Bottom.Name = "MainForm_Toolbars_Dock_Area_Bottom"
        MainForm_Toolbars_Dock_Area_Bottom.ToolbarsManager = utmMain
        ' 
        ' utmmMain
        ' 
        utmmMain.MdiParent = Me
        Appearance1.BackColor = Color.DodgerBlue
        Appearance1.ForeColor = Color.White
        utmmMain.TabSettings.ActiveTabAppearance = Appearance1
        resources.ApplyResources(Appearance2, "Appearance2")
        Appearance2.ForceApplyResources = ""
        utmmMain.TabSettings.TabAppearance = Appearance2
        ' 
        ' usbMain
        ' 
        resources.ApplyResources(usbMain, "usbMain")
        usbMain.Name = "usbMain"
        UltraStatusPanel1.Key = "panCursorPosition"
        UltraStatusPanel1.MinWidth = 100
        UltraStatusPanel1.Style = Infragistics.Win.UltraWinStatusBar.PanelStyle.CursorPosition
        UltraStatusPanel1.Width = 200
        UltraStatusPanel1.WrapText = Infragistics.Win.DefaultableBoolean.False
        resources.ApplyResources(UltraStatusPanel1, "UltraStatusPanel1")
        resources.ApplyResources(UltraStatusPanel1.KeyStateInfo, "UltraStatusPanel1.KeyStateInfo")
        resources.ApplyResources(UltraStatusPanel1.ProgressBarInfo, "UltraStatusPanel1.ProgressBarInfo")
        UltraStatusPanel1.ForceApplyResources = "|KeyStateInfo|ProgressBarInfo"
        UltraStatusPanel2.Key = "panMessage"
        UltraStatusPanel2.MinWidth = 100
        UltraStatusPanel2.SizingMode = Infragistics.Win.UltraWinStatusBar.PanelSizingMode.Spring
        UltraStatusPanel2.Width = 200
        UltraStatusPanel2.WrapText = Infragistics.Win.DefaultableBoolean.False
        resources.ApplyResources(UltraStatusPanel2, "UltraStatusPanel2")
        resources.ApplyResources(UltraStatusPanel2.KeyStateInfo, "UltraStatusPanel2.KeyStateInfo")
        resources.ApplyResources(UltraStatusPanel2.ProgressBarInfo, "UltraStatusPanel2.ProgressBarInfo")
        UltraStatusPanel2.ForceApplyResources = "|KeyStateInfo|ProgressBarInfo"
        UltraStatusPanel3.Key = "panProgress"
        UltraStatusPanel3.MinWidth = 50
        UltraStatusPanel3.ProgressBarInfo.Style = Infragistics.Win.UltraWinProgressBar.ProgressBarStyle.Continuous
        UltraStatusPanel3.Style = Infragistics.Win.UltraWinStatusBar.PanelStyle.Progress
        UltraStatusPanel3.Visible = False
        UltraStatusPanel3.Width = 150
        resources.ApplyResources(UltraStatusPanel3, "UltraStatusPanel3")
        resources.ApplyResources(UltraStatusPanel3.KeyStateInfo, "UltraStatusPanel3.KeyStateInfo")
        resources.ApplyResources(UltraStatusPanel3.ProgressBarInfo, "UltraStatusPanel3.ProgressBarInfo")
        UltraStatusPanel3.ForceApplyResources = "|KeyStateInfo|ProgressBarInfo"
        usbMain.Panels.AddRange(New Infragistics.Win.UltraWinStatusBar.UltraStatusPanel() {UltraStatusPanel1, UltraStatusPanel2, UltraStatusPanel3})
        ' 
        ' udmMain
        ' 
        udmMain.AllowDrop = False
        udmMain.CompressUnpinnedTabs = False
        udmMain.DefaultPaneSettings.AllowMaximize = Infragistics.Win.DefaultableBoolean.True
        udmMain.DefaultPaneSettings.AllowMinimize = Infragistics.Win.DefaultableBoolean.True
        udmMain.DragWindowStyle = Infragistics.Win.UltraWinDock.DragWindowStyle.LayeredWindowWithIndicators
        udmMain.HostControl = Me
        udmMain.SettingsKey = "MainForm.udmMain"
        udmMain.ShowMaximizeButton = True
        ' 
        ' _BrowserFormUnpinnedTabAreaLeft
        ' 
        resources.ApplyResources(_BrowserFormUnpinnedTabAreaLeft, "_BrowserFormUnpinnedTabAreaLeft")
        _BrowserFormUnpinnedTabAreaLeft.Name = "_BrowserFormUnpinnedTabAreaLeft"
        _BrowserFormUnpinnedTabAreaLeft.Owner = udmMain
        ' 
        ' _BrowserFormUnpinnedTabAreaRight
        ' 
        resources.ApplyResources(_BrowserFormUnpinnedTabAreaRight, "_BrowserFormUnpinnedTabAreaRight")
        _BrowserFormUnpinnedTabAreaRight.Name = "_BrowserFormUnpinnedTabAreaRight"
        _BrowserFormUnpinnedTabAreaRight.Owner = udmMain
        ' 
        ' _BrowserFormUnpinnedTabAreaTop
        ' 
        resources.ApplyResources(_BrowserFormUnpinnedTabAreaTop, "_BrowserFormUnpinnedTabAreaTop")
        _BrowserFormUnpinnedTabAreaTop.Name = "_BrowserFormUnpinnedTabAreaTop"
        _BrowserFormUnpinnedTabAreaTop.Owner = udmMain
        ' 
        ' _BrowserFormUnpinnedTabAreaBottom
        ' 
        resources.ApplyResources(_BrowserFormUnpinnedTabAreaBottom, "_BrowserFormUnpinnedTabAreaBottom")
        _BrowserFormUnpinnedTabAreaBottom.Name = "_BrowserFormUnpinnedTabAreaBottom"
        _BrowserFormUnpinnedTabAreaBottom.Owner = udmMain
        ' 
        ' _BrowserFormAutoHideControl
        ' 
        resources.ApplyResources(_BrowserFormAutoHideControl, "_BrowserFormAutoHideControl")
        _BrowserFormAutoHideControl.Name = "_BrowserFormAutoHideControl"
        _BrowserFormAutoHideControl.Owner = udmMain
        ' 
        ' MainForm
        ' 
        AutoScaleMode = AutoScaleMode.Inherit
        resources.ApplyResources(Me, "$this")
        Controls.Add(_BrowserFormAutoHideControl)
        Controls.Add(_BrowserFormUnpinnedTabAreaTop)
        Controls.Add(_BrowserFormUnpinnedTabAreaBottom)
        Controls.Add(_BrowserFormUnpinnedTabAreaLeft)
        Controls.Add(_BrowserFormUnpinnedTabAreaRight)
        Controls.Add(MainForm_Toolbars_Dock_Area_Left)
        Controls.Add(MainForm_Toolbars_Dock_Area_Right)
        Controls.Add(MainForm_Toolbars_Dock_Area_Bottom)
        Controls.Add(usbMain)
        Controls.Add(MainForm_Toolbars_Dock_Area_Top)
        DoubleBuffered = True
        IsMdiContainer = True
        KeyPreview = True
        Name = "MainForm"
        WindowState = FormWindowState.Maximized
        CType(utmMain, ComponentModel.ISupportInitialize).EndInit()
        CType(utmmMain, ComponentModel.ISupportInitialize).EndInit()
        CType(usbMain, ComponentModel.ISupportInitialize).EndInit()
        CType(udmMain, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)

    End Sub
    Friend WithEvents utmMain As Infragistics.Win.UltraWinToolbars.UltraToolbarsManager
    Friend WithEvents MainForm_Toolbars_Dock_Area_Left As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents MainForm_Toolbars_Dock_Area_Right As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents MainForm_Toolbars_Dock_Area_Bottom As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents MainForm_Toolbars_Dock_Area_Top As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents utmmMain As Infragistics.Win.UltraWinTabbedMdi.UltraTabbedMdiManager
    Friend WithEvents usbMain As Infragistics.Win.UltraWinStatusBar.UltraStatusBar
    Friend WithEvents _BrowserFormAutoHideControl As Infragistics.Win.UltraWinDock.AutoHideControl
    Friend WithEvents udmMain As Infragistics.Win.UltraWinDock.UltraDockManager
    Friend WithEvents _BrowserFormUnpinnedTabAreaBottom As Infragistics.Win.UltraWinDock.UnpinnedTabArea
    Friend WithEvents _BrowserFormUnpinnedTabAreaTop As Infragistics.Win.UltraWinDock.UnpinnedTabArea
    Friend WithEvents _BrowserFormUnpinnedTabAreaRight As Infragistics.Win.UltraWinDock.UnpinnedTabArea
    Friend WithEvents _BrowserFormUnpinnedTabAreaLeft As Infragistics.Win.UltraWinDock.UnpinnedTabArea

End Class
