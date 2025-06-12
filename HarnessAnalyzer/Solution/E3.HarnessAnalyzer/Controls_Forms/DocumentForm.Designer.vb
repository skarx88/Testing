<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class DocumentForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing Then
                If _D3DControl IsNot Nothing Then
                    RemoveHandler _D3DControl.SelectRowsInInformationHub, AddressOf SelectRowsInInformationHub
                    _D3DControl.Dispose()
                End If
                _document?.Dispose()
                _cavitiesDocumentView?.Dispose()
                '_idAdapter?.Dispose()
                components?.Dispose()
                _logHub?.Dispose()
                _busyState?.Dispose()
                _drawingsHub?.Dispose()
                _informationHub?.Dispose()
                _memolistHub?.Dispose()
                _modulesHub?.Dispose()
                _navigatorHub?.Dispose()
                _panes?.Dispose()
                ActiveDrawingCanvas?.Dispose() ' HINT: is created by DocumentForm in LoadKBLDocumentFinished
            End If

            ActiveDrawingCanvas = Nothing
            _navigatorHub = Nothing
            _modulesHub = Nothing
            _messageEventArgs = Nothing
            _memolistHub = Nothing
            _informationHub = Nothing
            _harnessModuleConfigurations = Nothing
            _drawingsHub = Nothing
            _analysisStateMachine = Nothing
            _documentStateMachine = Nothing
            _settingsForm = Nothing
            _drawingFile = Nothing
            _hcvFile = Nothing
            _busyState = Nothing
            _logHub = Nothing
            _D3DControl = Nothing
            _document = Nothing
            _cavitiesDocumentView = Nothing
            _panes = Nothing
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DocumentForm))
        upanDocument = New Infragistics.Win.Misc.UltraPanel()
        utcDocument = New Infragistics.Win.UltraWinTabControl.UltraTabControl()
        utscpDocument = New Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage()
        udmDocument = New Infragistics.Win.UltraWinDock.UltraDockManager(components)
        _DocumentFormUnpinnedTabAreaLeft = New Infragistics.Win.UltraWinDock.UnpinnedTabArea()
        _DocumentFormUnpinnedTabAreaRight = New Infragistics.Win.UltraWinDock.UnpinnedTabArea()
        _DocumentFormUnpinnedTabAreaTop = New Infragistics.Win.UltraWinDock.UnpinnedTabArea()
        _DocumentFormUnpinnedTabAreaBottom = New Infragistics.Win.UltraWinDock.UnpinnedTabArea()
        _DocumentFormAutoHideControl = New Infragistics.Win.UltraWinDock.AutoHideControl()
        utmDocument = New Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(components)
        _DocumentForm_Toolbars_Dock_Area_Left = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        _DocumentForm_Toolbars_Dock_Area_Right = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        _DocumentForm_Toolbars_Dock_Area_Top = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        _DocumentForm_Toolbars_Dock_Area_Bottom = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        upanDocument.ClientArea.SuspendLayout()
        upanDocument.SuspendLayout()
        CType(utcDocument, ComponentModel.ISupportInitialize).BeginInit()
        utcDocument.SuspendLayout()
        CType(udmDocument, ComponentModel.ISupportInitialize).BeginInit()
        CType(utmDocument, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' upanDocument
        ' 
        ' 
        ' upanDocument.ClientArea
        ' 
        upanDocument.ClientArea.Controls.Add(utcDocument)
        resources.ApplyResources(upanDocument, "upanDocument")
        upanDocument.Name = "upanDocument"
        ' 
        ' utcDocument
        ' 
        utcDocument.Controls.Add(utscpDocument)
        resources.ApplyResources(utcDocument, "utcDocument")
        utcDocument.Name = "utcDocument"
        utcDocument.SharedControlsPage = utscpDocument
        ' 
        ' utscpDocument
        ' 
        resources.ApplyResources(utscpDocument, "utscpDocument")
        utscpDocument.Name = "utscpDocument"
        ' 
        ' udmDocument
        ' 
        udmDocument.CompressUnpinnedTabs = False
        udmDocument.DragWindowStyle = Infragistics.Win.UltraWinDock.DragWindowStyle.LayeredWindowWithIndicators
        udmDocument.HostControl = Me
        ' 
        ' _DocumentFormUnpinnedTabAreaLeft
        ' 
        resources.ApplyResources(_DocumentFormUnpinnedTabAreaLeft, "_DocumentFormUnpinnedTabAreaLeft")
        _DocumentFormUnpinnedTabAreaLeft.Name = "_DocumentFormUnpinnedTabAreaLeft"
        _DocumentFormUnpinnedTabAreaLeft.Owner = udmDocument
        ' 
        ' _DocumentFormUnpinnedTabAreaRight
        ' 
        resources.ApplyResources(_DocumentFormUnpinnedTabAreaRight, "_DocumentFormUnpinnedTabAreaRight")
        _DocumentFormUnpinnedTabAreaRight.Name = "_DocumentFormUnpinnedTabAreaRight"
        _DocumentFormUnpinnedTabAreaRight.Owner = udmDocument
        ' 
        ' _DocumentFormUnpinnedTabAreaTop
        ' 
        resources.ApplyResources(_DocumentFormUnpinnedTabAreaTop, "_DocumentFormUnpinnedTabAreaTop")
        _DocumentFormUnpinnedTabAreaTop.Name = "_DocumentFormUnpinnedTabAreaTop"
        _DocumentFormUnpinnedTabAreaTop.Owner = udmDocument
        ' 
        ' _DocumentFormUnpinnedTabAreaBottom
        ' 
        resources.ApplyResources(_DocumentFormUnpinnedTabAreaBottom, "_DocumentFormUnpinnedTabAreaBottom")
        _DocumentFormUnpinnedTabAreaBottom.Name = "_DocumentFormUnpinnedTabAreaBottom"
        _DocumentFormUnpinnedTabAreaBottom.Owner = udmDocument
        ' 
        ' _DocumentFormAutoHideControl
        ' 
        resources.ApplyResources(_DocumentFormAutoHideControl, "_DocumentFormAutoHideControl")
        _DocumentFormAutoHideControl.Name = "_DocumentFormAutoHideControl"
        _DocumentFormAutoHideControl.Owner = udmDocument
        ' 
        ' utmDocument
        ' 
        utmDocument.DesignerFlags = 1
        utmDocument.DockWithinContainer = Me
        utmDocument.DockWithinContainerBaseType = GetType(Form)
        ' 
        ' _DocumentForm_Toolbars_Dock_Area_Left
        ' 
        _DocumentForm_Toolbars_Dock_Area_Left.AccessibleRole = AccessibleRole.Grouping
        _DocumentForm_Toolbars_Dock_Area_Left.BackColor = SystemColors.Control
        _DocumentForm_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left
        _DocumentForm_Toolbars_Dock_Area_Left.ForeColor = SystemColors.ControlText
        resources.ApplyResources(_DocumentForm_Toolbars_Dock_Area_Left, "_DocumentForm_Toolbars_Dock_Area_Left")
        _DocumentForm_Toolbars_Dock_Area_Left.Name = "_DocumentForm_Toolbars_Dock_Area_Left"
        _DocumentForm_Toolbars_Dock_Area_Left.ToolbarsManager = utmDocument
        ' 
        ' _DocumentForm_Toolbars_Dock_Area_Right
        ' 
        _DocumentForm_Toolbars_Dock_Area_Right.AccessibleRole = AccessibleRole.Grouping
        _DocumentForm_Toolbars_Dock_Area_Right.BackColor = SystemColors.Control
        _DocumentForm_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right
        _DocumentForm_Toolbars_Dock_Area_Right.ForeColor = SystemColors.ControlText
        resources.ApplyResources(_DocumentForm_Toolbars_Dock_Area_Right, "_DocumentForm_Toolbars_Dock_Area_Right")
        _DocumentForm_Toolbars_Dock_Area_Right.Name = "_DocumentForm_Toolbars_Dock_Area_Right"
        _DocumentForm_Toolbars_Dock_Area_Right.ToolbarsManager = utmDocument
        ' 
        ' _DocumentForm_Toolbars_Dock_Area_Top
        ' 
        _DocumentForm_Toolbars_Dock_Area_Top.AccessibleRole = AccessibleRole.Grouping
        _DocumentForm_Toolbars_Dock_Area_Top.BackColor = SystemColors.Control
        _DocumentForm_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top
        _DocumentForm_Toolbars_Dock_Area_Top.ForeColor = SystemColors.ControlText
        resources.ApplyResources(_DocumentForm_Toolbars_Dock_Area_Top, "_DocumentForm_Toolbars_Dock_Area_Top")
        _DocumentForm_Toolbars_Dock_Area_Top.Name = "_DocumentForm_Toolbars_Dock_Area_Top"
        _DocumentForm_Toolbars_Dock_Area_Top.ToolbarsManager = utmDocument
        ' 
        ' _DocumentForm_Toolbars_Dock_Area_Bottom
        ' 
        _DocumentForm_Toolbars_Dock_Area_Bottom.AccessibleRole = AccessibleRole.Grouping
        _DocumentForm_Toolbars_Dock_Area_Bottom.BackColor = SystemColors.Control
        _DocumentForm_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom
        _DocumentForm_Toolbars_Dock_Area_Bottom.ForeColor = SystemColors.ControlText
        resources.ApplyResources(_DocumentForm_Toolbars_Dock_Area_Bottom, "_DocumentForm_Toolbars_Dock_Area_Bottom")
        _DocumentForm_Toolbars_Dock_Area_Bottom.Name = "_DocumentForm_Toolbars_Dock_Area_Bottom"
        _DocumentForm_Toolbars_Dock_Area_Bottom.ToolbarsManager = utmDocument
        ' 
        ' DocumentForm
        ' 
        AutoScaleMode = AutoScaleMode.Inherit
        resources.ApplyResources(Me, "$this")
        Controls.Add(_DocumentFormAutoHideControl)
        Controls.Add(upanDocument)
        Controls.Add(_DocumentFormUnpinnedTabAreaBottom)
        Controls.Add(_DocumentFormUnpinnedTabAreaTop)
        Controls.Add(_DocumentFormUnpinnedTabAreaRight)
        Controls.Add(_DocumentFormUnpinnedTabAreaLeft)
        Controls.Add(_DocumentForm_Toolbars_Dock_Area_Left)
        Controls.Add(_DocumentForm_Toolbars_Dock_Area_Right)
        Controls.Add(_DocumentForm_Toolbars_Dock_Area_Bottom)
        Controls.Add(_DocumentForm_Toolbars_Dock_Area_Top)
        DoubleBuffered = True
        KeyPreview = True
        Name = "DocumentForm"
        ShowIcon = False
        ShowInTaskbar = False
        WindowState = FormWindowState.Minimized
        upanDocument.ClientArea.ResumeLayout(False)
        upanDocument.ResumeLayout(False)
        CType(utcDocument, ComponentModel.ISupportInitialize).EndInit()
        utcDocument.ResumeLayout(False)
        CType(udmDocument, ComponentModel.ISupportInitialize).EndInit()
        CType(utmDocument, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)

    End Sub
    Friend WithEvents udmDocument As Infragistics.Win.UltraWinDock.UltraDockManager
    Friend WithEvents _DocumentFormAutoHideControl As Infragistics.Win.UltraWinDock.AutoHideControl
    Friend WithEvents _DocumentFormUnpinnedTabAreaBottom As Infragistics.Win.UltraWinDock.UnpinnedTabArea
    Friend WithEvents _DocumentFormUnpinnedTabAreaTop As Infragistics.Win.UltraWinDock.UnpinnedTabArea
    Friend WithEvents _DocumentFormUnpinnedTabAreaRight As Infragistics.Win.UltraWinDock.UnpinnedTabArea
    Friend WithEvents _DocumentFormUnpinnedTabAreaLeft As Infragistics.Win.UltraWinDock.UnpinnedTabArea
    Friend WithEvents upanDocument As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents _DocumentForm_Toolbars_Dock_Area_Left As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents utmDocument As Infragistics.Win.UltraWinToolbars.UltraToolbarsManager
    Friend WithEvents _DocumentForm_Toolbars_Dock_Area_Right As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents _DocumentForm_Toolbars_Dock_Area_Bottom As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents _DocumentForm_Toolbars_Dock_Area_Top As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents utcDocument As Infragistics.Win.UltraWinTabControl.UltraTabControl
    Friend WithEvents utscpDocument As Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage
End Class
