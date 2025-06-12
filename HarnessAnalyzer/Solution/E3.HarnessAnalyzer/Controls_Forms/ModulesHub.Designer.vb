<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ModulesHub
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ModulesHub))
        Dim Appearance1 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Me.upnConfig = New Infragistics.Win.Misc.UltraPanel()
        Me.uceModuleConfigs = New Infragistics.Win.UltraWinEditors.UltraComboEditor()
        Me.upnButton = New Infragistics.Win.Misc.UltraPanel()
        Me.btnApply = New Infragistics.Win.Misc.UltraButton()
        Me.upnTree = New Infragistics.Win.Misc.UltraPanel()
        Me.utModules = New Infragistics.Win.UltraWinTree.UltraTree()
        Me.utchpModulesHub = New Infragistics.Win.Touch.UltraTouchProvider(Me.components)
        Me.upnHarnessInfo = New Infragistics.Win.Misc.UltraPanel()
        Me.lblHarnessInfo = New Infragistics.Win.Misc.UltraLabel()
        Me.utmModulesHub = New Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(Me.components)
        Me._ModulesHub_Toolbars_Dock_Area_Left = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        Me._ModulesHub_Toolbars_Dock_Area_Right = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        Me._ModulesHub_Toolbars_Dock_Area_Top = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        Me._ModulesHub_Toolbars_Dock_Area_Bottom = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        Me.upnConfig.ClientArea.SuspendLayout()
        Me.upnConfig.SuspendLayout()
        CType(Me.uceModuleConfigs, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.upnButton.ClientArea.SuspendLayout()
        Me.upnButton.SuspendLayout()
        Me.upnTree.ClientArea.SuspendLayout()
        Me.upnTree.SuspendLayout()
        CType(Me.utModules, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.utchpModulesHub, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.upnHarnessInfo.ClientArea.SuspendLayout()
        Me.upnHarnessInfo.SuspendLayout()
        CType(Me.utmModulesHub, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'upnConfig
        '
        '
        'upnConfig.ClientArea
        '
        Me.upnConfig.ClientArea.Controls.Add(Me.uceModuleConfigs)
        Me.upnConfig.ClientArea.Controls.Add(Me.upnButton)
        resources.ApplyResources(Me.upnConfig, "upnConfig")
        Me.upnConfig.Name = "upnConfig"
        '
        'uceModuleConfigs
        '
        resources.ApplyResources(Me.uceModuleConfigs, "uceModuleConfigs")
        Me.uceModuleConfigs.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList
        Me.uceModuleConfigs.Name = "uceModuleConfigs"
        '
        'upnButton
        '
        '
        'upnButton.ClientArea
        '
        Me.upnButton.ClientArea.Controls.Add(Me.btnApply)
        resources.ApplyResources(Me.upnButton, "upnButton")
        Me.upnButton.Name = "upnButton"
        '
        'btnApply
        '
        resources.ApplyResources(Me.btnApply, "btnApply")
        Me.btnApply.Name = "btnApply"
        '
        'upnTree
        '
        resources.ApplyResources(Me.upnTree, "upnTree")
        '
        'upnTree.ClientArea
        '
        Me.upnTree.ClientArea.Controls.Add(Me.utModules)
        Me.upnTree.Name = "upnTree"
        '
        'utModules
        '
        resources.ApplyResources(Me.utModules, "utModules")
        Me.utModules.Name = "utModules"
        '
        'utchpModulesHub
        '
        Me.utchpModulesHub.ContainingControl = Me
        '
        'upnHarnessInfo
        '
        '
        'upnHarnessInfo.ClientArea
        '
        Me.upnHarnessInfo.ClientArea.Controls.Add(Me.lblHarnessInfo)
        resources.ApplyResources(Me.upnHarnessInfo, "upnHarnessInfo")
        Me.upnHarnessInfo.Name = "upnHarnessInfo"
        '
        'lblHarnessInfo
        '
        resources.ApplyResources(Appearance1, "Appearance1")
        Me.lblHarnessInfo.Appearance = Appearance1
        resources.ApplyResources(Me.lblHarnessInfo, "lblHarnessInfo")
        Me.lblHarnessInfo.Name = "lblHarnessInfo"
        '
        'utmModulesHub
        '
        Me.utmModulesHub.DesignerFlags = 1
        Me.utmModulesHub.DockWithinContainer = Me
        '
        '_ModulesHub_Toolbars_Dock_Area_Left
        '
        Me._ModulesHub_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me._ModulesHub_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control
        Me._ModulesHub_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left
        Me._ModulesHub_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText
        resources.ApplyResources(Me._ModulesHub_Toolbars_Dock_Area_Left, "_ModulesHub_Toolbars_Dock_Area_Left")
        Me._ModulesHub_Toolbars_Dock_Area_Left.Name = "_ModulesHub_Toolbars_Dock_Area_Left"
        Me._ModulesHub_Toolbars_Dock_Area_Left.ToolbarsManager = Me.utmModulesHub
        '
        '_ModulesHub_Toolbars_Dock_Area_Right
        '
        Me._ModulesHub_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me._ModulesHub_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control
        Me._ModulesHub_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right
        Me._ModulesHub_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText
        resources.ApplyResources(Me._ModulesHub_Toolbars_Dock_Area_Right, "_ModulesHub_Toolbars_Dock_Area_Right")
        Me._ModulesHub_Toolbars_Dock_Area_Right.Name = "_ModulesHub_Toolbars_Dock_Area_Right"
        Me._ModulesHub_Toolbars_Dock_Area_Right.ToolbarsManager = Me.utmModulesHub
        '
        '_ModulesHub_Toolbars_Dock_Area_Top
        '
        Me._ModulesHub_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me._ModulesHub_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control
        Me._ModulesHub_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top
        Me._ModulesHub_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText
        resources.ApplyResources(Me._ModulesHub_Toolbars_Dock_Area_Top, "_ModulesHub_Toolbars_Dock_Area_Top")
        Me._ModulesHub_Toolbars_Dock_Area_Top.Name = "_ModulesHub_Toolbars_Dock_Area_Top"
        Me._ModulesHub_Toolbars_Dock_Area_Top.ToolbarsManager = Me.utmModulesHub
        '
        '_ModulesHub_Toolbars_Dock_Area_Bottom
        '
        Me._ModulesHub_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me._ModulesHub_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control
        Me._ModulesHub_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom
        Me._ModulesHub_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText
        resources.ApplyResources(Me._ModulesHub_Toolbars_Dock_Area_Bottom, "_ModulesHub_Toolbars_Dock_Area_Bottom")
        Me._ModulesHub_Toolbars_Dock_Area_Bottom.Name = "_ModulesHub_Toolbars_Dock_Area_Bottom"
        Me._ModulesHub_Toolbars_Dock_Area_Bottom.ToolbarsManager = Me.utmModulesHub
        '
        'ModulesHub
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.upnHarnessInfo)
        Me.Controls.Add(Me.upnTree)
        Me.Controls.Add(Me.upnConfig)
        Me.Controls.Add(Me._ModulesHub_Toolbars_Dock_Area_Left)
        Me.Controls.Add(Me._ModulesHub_Toolbars_Dock_Area_Right)
        Me.Controls.Add(Me._ModulesHub_Toolbars_Dock_Area_Bottom)
        Me.Controls.Add(Me._ModulesHub_Toolbars_Dock_Area_Top)
        Me.Name = "ModulesHub"
        Me.upnConfig.ClientArea.ResumeLayout(False)
        Me.upnConfig.ClientArea.PerformLayout()
        Me.upnConfig.ResumeLayout(False)
        CType(Me.uceModuleConfigs, System.ComponentModel.ISupportInitialize).EndInit()
        Me.upnButton.ClientArea.ResumeLayout(False)
        Me.upnButton.ResumeLayout(False)
        Me.upnTree.ClientArea.ResumeLayout(False)
        Me.upnTree.ResumeLayout(False)
        CType(Me.utModules, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.utchpModulesHub, System.ComponentModel.ISupportInitialize).EndInit()
        Me.upnHarnessInfo.ClientArea.ResumeLayout(False)
        Me.upnHarnessInfo.ResumeLayout(False)
        CType(Me.utmModulesHub, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents upnTree As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents utModules As Infragistics.Win.UltraWinTree.UltraTree
    Friend WithEvents upnConfig As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents uceModuleConfigs As Infragistics.Win.UltraWinEditors.UltraComboEditor
    Friend WithEvents upnButton As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents btnApply As Infragistics.Win.Misc.UltraButton
    Friend WithEvents utchpModulesHub As Infragistics.Win.Touch.UltraTouchProvider
    Friend WithEvents upnHarnessInfo As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents lblHarnessInfo As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents _ModulesHub_Toolbars_Dock_Area_Left As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents utmModulesHub As Infragistics.Win.UltraWinToolbars.UltraToolbarsManager
    Friend WithEvents _ModulesHub_Toolbars_Dock_Area_Right As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents _ModulesHub_Toolbars_Dock_Area_Bottom As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents _ModulesHub_Toolbars_Dock_Area_Top As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
End Class
