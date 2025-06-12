<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TopologyEditorForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(TopologyEditorForm))
        Dim Appearance3 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance4 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Me.upnTopologyEditor = New Infragistics.Win.Misc.UltraPanel()
        Me.lblDrawCommand = New Infragistics.Win.Misc.UltraLabel()
        Me.uckShowBackground = New Infragistics.Win.UltraWinEditors.UltraCheckEditor()
        Me.lblCoordinates = New Infragistics.Win.Misc.UltraLabel()
        Me.btnClose = New Infragistics.Win.Misc.UltraButton()
        Me.vDraw = New VectorDraw.Professional.Control.VectorDrawBaseControl()
        Me.utmTopologyEditor = New Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(Me.components)
        Me._TopologyEditorForm_Toolbars_Dock_Area_Left = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        Me._TopologyEditorForm_Toolbars_Dock_Area_Right = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        Me._TopologyEditorForm_Toolbars_Dock_Area_Top = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        Me._TopologyEditorForm_Toolbars_Dock_Area_Bottom = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        Me.ofdTopologyEditor = New System.Windows.Forms.OpenFileDialog()
        Me.sfdTopologyEditor = New System.Windows.Forms.SaveFileDialog()
        Me.upnTopologyEditor.ClientArea.SuspendLayout()
        Me.upnTopologyEditor.SuspendLayout()
        CType(Me.uckShowBackground, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.utmTopologyEditor, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'upnTopologyEditor
        '
        resources.ApplyResources(Me.upnTopologyEditor, "upnTopologyEditor")
        '
        'upnTopologyEditor.ClientArea
        '
        resources.ApplyResources(Me.upnTopologyEditor.ClientArea, "upnTopologyEditor.ClientArea")
        Me.upnTopologyEditor.ClientArea.Controls.Add(Me.lblDrawCommand)
        Me.upnTopologyEditor.ClientArea.Controls.Add(Me.uckShowBackground)
        Me.upnTopologyEditor.ClientArea.Controls.Add(Me.lblCoordinates)
        Me.upnTopologyEditor.ClientArea.Controls.Add(Me.btnClose)
        Me.upnTopologyEditor.ClientArea.Controls.Add(Me.vDraw)
        Me.upnTopologyEditor.Cursor = System.Windows.Forms.Cursors.Default
        Me.upnTopologyEditor.Name = "upnTopologyEditor"
        '
        'lblDrawCommand
        '
        resources.ApplyResources(Me.lblDrawCommand, "lblDrawCommand")
        Appearance3.BackColor = System.Drawing.Color.Transparent
        Appearance3.FontData.BoldAsString = resources.GetString("resource.BoldAsString")
        Appearance3.FontData.ItalicAsString = resources.GetString("resource.ItalicAsString")
        Appearance3.FontData.StrikeoutAsString = resources.GetString("resource.StrikeoutAsString")
        Appearance3.FontData.UnderlineAsString = resources.GetString("resource.UnderlineAsString")
        Appearance3.ForeColor = System.Drawing.Color.Red
        resources.ApplyResources(Appearance3, "Appearance3")
        Me.lblDrawCommand.Appearance = Appearance3
        Me.lblDrawCommand.Name = "lblDrawCommand"
        '
        'uckShowBackground
        '
        resources.ApplyResources(Me.uckShowBackground, "uckShowBackground")
        Me.uckShowBackground.Name = "uckShowBackground"
        '
        'lblCoordinates
        '
        resources.ApplyResources(Me.lblCoordinates, "lblCoordinates")
        resources.ApplyResources(Appearance4, "Appearance4")
        Me.lblCoordinates.Appearance = Appearance4
        Me.lblCoordinates.Name = "lblCoordinates"
        '
        'btnClose
        '
        resources.ApplyResources(Me.btnClose, "btnClose")
        Me.btnClose.Name = "btnClose"
        '
        'vDraw
        '
        resources.ApplyResources(Me.vDraw, "vDraw")
        Me.vDraw.AccessibleRole = System.Windows.Forms.AccessibleRole.Window
        Me.vDraw.AllowDrop = True
        Me.vDraw.Cursor = System.Windows.Forms.Cursors.Default
        Me.vDraw.DisableVdrawDxf = False
        Me.vDraw.EnableAutoGripOn = True
        Me.vDraw.Name = "vDraw"
        '
        'utmTopologyEditor
        '
        Me.utmTopologyEditor.DesignerFlags = 1
        Me.utmTopologyEditor.DockWithinContainer = Me
        Me.utmTopologyEditor.DockWithinContainerBaseType = GetType(System.Windows.Forms.Form)
        '
        '_TopologyEditorForm_Toolbars_Dock_Area_Left
        '
        resources.ApplyResources(Me._TopologyEditorForm_Toolbars_Dock_Area_Left, "_TopologyEditorForm_Toolbars_Dock_Area_Left")
        Me._TopologyEditorForm_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me._TopologyEditorForm_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control
        Me._TopologyEditorForm_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left
        Me._TopologyEditorForm_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText
        Me._TopologyEditorForm_Toolbars_Dock_Area_Left.Name = "_TopologyEditorForm_Toolbars_Dock_Area_Left"
        Me._TopologyEditorForm_Toolbars_Dock_Area_Left.ToolbarsManager = Me.utmTopologyEditor
        '
        '_TopologyEditorForm_Toolbars_Dock_Area_Right
        '
        resources.ApplyResources(Me._TopologyEditorForm_Toolbars_Dock_Area_Right, "_TopologyEditorForm_Toolbars_Dock_Area_Right")
        Me._TopologyEditorForm_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me._TopologyEditorForm_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control
        Me._TopologyEditorForm_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right
        Me._TopologyEditorForm_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText
        Me._TopologyEditorForm_Toolbars_Dock_Area_Right.Name = "_TopologyEditorForm_Toolbars_Dock_Area_Right"
        Me._TopologyEditorForm_Toolbars_Dock_Area_Right.ToolbarsManager = Me.utmTopologyEditor
        '
        '_TopologyEditorForm_Toolbars_Dock_Area_Top
        '
        resources.ApplyResources(Me._TopologyEditorForm_Toolbars_Dock_Area_Top, "_TopologyEditorForm_Toolbars_Dock_Area_Top")
        Me._TopologyEditorForm_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me._TopologyEditorForm_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control
        Me._TopologyEditorForm_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top
        Me._TopologyEditorForm_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText
        Me._TopologyEditorForm_Toolbars_Dock_Area_Top.Name = "_TopologyEditorForm_Toolbars_Dock_Area_Top"
        Me._TopologyEditorForm_Toolbars_Dock_Area_Top.ToolbarsManager = Me.utmTopologyEditor
        '
        '_TopologyEditorForm_Toolbars_Dock_Area_Bottom
        '
        resources.ApplyResources(Me._TopologyEditorForm_Toolbars_Dock_Area_Bottom, "_TopologyEditorForm_Toolbars_Dock_Area_Bottom")
        Me._TopologyEditorForm_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me._TopologyEditorForm_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control
        Me._TopologyEditorForm_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom
        Me._TopologyEditorForm_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText
        Me._TopologyEditorForm_Toolbars_Dock_Area_Bottom.Name = "_TopologyEditorForm_Toolbars_Dock_Area_Bottom"
        Me._TopologyEditorForm_Toolbars_Dock_Area_Bottom.ToolbarsManager = Me.utmTopologyEditor
        '
        'ofdTopologyEditor
        '
        resources.ApplyResources(Me.ofdTopologyEditor, "ofdTopologyEditor")
        '
        'sfdTopologyEditor
        '
        resources.ApplyResources(Me.sfdTopologyEditor, "sfdTopologyEditor")
        '
        'TopologyEditorForm
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.Controls.Add(Me.upnTopologyEditor)
        Me.Controls.Add(Me._TopologyEditorForm_Toolbars_Dock_Area_Left)
        Me.Controls.Add(Me._TopologyEditorForm_Toolbars_Dock_Area_Right)
        Me.Controls.Add(Me._TopologyEditorForm_Toolbars_Dock_Area_Bottom)
        Me.Controls.Add(Me._TopologyEditorForm_Toolbars_Dock_Area_Top)
        Me.Name = "TopologyEditorForm"
        Me.upnTopologyEditor.ClientArea.ResumeLayout(False)
        Me.upnTopologyEditor.ResumeLayout(False)
        CType(Me.uckShowBackground, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.utmTopologyEditor, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents utmTopologyEditor As Infragistics.Win.UltraWinToolbars.UltraToolbarsManager
    Friend WithEvents upnTopologyEditor As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents btnClose As Infragistics.Win.Misc.UltraButton
    Friend WithEvents vDraw As VectorDraw.Professional.Control.VectorDrawBaseControl
    Friend WithEvents _TopologyEditorForm_Toolbars_Dock_Area_Left As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents _TopologyEditorForm_Toolbars_Dock_Area_Right As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents _TopologyEditorForm_Toolbars_Dock_Area_Bottom As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents _TopologyEditorForm_Toolbars_Dock_Area_Top As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents ofdTopologyEditor As System.Windows.Forms.OpenFileDialog
    Friend WithEvents sfdTopologyEditor As System.Windows.Forms.SaveFileDialog
    Friend WithEvents uckShowBackground As Infragistics.Win.UltraWinEditors.UltraCheckEditor
    Friend WithEvents lblCoordinates As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents lblDrawCommand As Infragistics.Win.Misc.UltraLabel
End Class
