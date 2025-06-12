<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ModulesWeightForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ModulesWeightForm))
        Me.ugModulesWeight = New Infragistics.Win.UltraWinGrid.UltraGrid()
        Me.btnClose = New Infragistics.Win.Misc.UltraButton()
        Me.btnExport = New Infragistics.Win.Misc.UltraButton()
        Me.udsModulesWeight = New Infragistics.Win.UltraWinDataSource.UltraDataSource(Me.components)
        Me.ugeeModulesWeight = New Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter(Me.components)
        Me.utmModulesWeight = New Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(Me.components)
        Me._ModulesWeightForm_Toolbars_Dock_Area_Left = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        Me._ModulesWeightForm_Toolbars_Dock_Area_Right = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        Me._ModulesWeightForm_Toolbars_Dock_Area_Top = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        Me._ModulesWeightForm_Toolbars_Dock_Area_Bottom = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        CType(Me.ugModulesWeight, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.udsModulesWeight, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.utmModulesWeight, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ugModulesWeight
        '
        resources.ApplyResources(Me.ugModulesWeight, "ugModulesWeight")
        Me.ugModulesWeight.DisplayLayout.Override.AllowMultiCellOperations = Infragistics.Win.UltraWinGrid.AllowMultiCellOperation.Copy
        Me.ugModulesWeight.Name = "ugModulesWeight"
        '
        'btnClose
        '
        resources.ApplyResources(Me.btnClose, "btnClose")
        Me.btnClose.Name = "btnClose"
        '
        'btnExport
        '
        resources.ApplyResources(Me.btnExport, "btnExport")
        Me.btnExport.Name = "btnExport"
        '
        'ugeeModulesWeight
        '
        '
        'utmModulesWeight
        '
        Me.utmModulesWeight.DesignerFlags = 1
        Me.utmModulesWeight.DockWithinContainer = Me
        Me.utmModulesWeight.DockWithinContainerBaseType = GetType(System.Windows.Forms.Form)
        '
        '_ModulesWeightForm_Toolbars_Dock_Area_Left
        '
        Me._ModulesWeightForm_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me._ModulesWeightForm_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control
        Me._ModulesWeightForm_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left
        Me._ModulesWeightForm_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText
        resources.ApplyResources(Me._ModulesWeightForm_Toolbars_Dock_Area_Left, "_ModulesWeightForm_Toolbars_Dock_Area_Left")
        Me._ModulesWeightForm_Toolbars_Dock_Area_Left.Name = "_ModulesWeightForm_Toolbars_Dock_Area_Left"
        Me._ModulesWeightForm_Toolbars_Dock_Area_Left.ToolbarsManager = Me.utmModulesWeight
        '
        '_ModulesWeightForm_Toolbars_Dock_Area_Right
        '
        Me._ModulesWeightForm_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me._ModulesWeightForm_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control
        Me._ModulesWeightForm_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right
        Me._ModulesWeightForm_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText
        resources.ApplyResources(Me._ModulesWeightForm_Toolbars_Dock_Area_Right, "_ModulesWeightForm_Toolbars_Dock_Area_Right")
        Me._ModulesWeightForm_Toolbars_Dock_Area_Right.Name = "_ModulesWeightForm_Toolbars_Dock_Area_Right"
        Me._ModulesWeightForm_Toolbars_Dock_Area_Right.ToolbarsManager = Me.utmModulesWeight
        '
        '_ModulesWeightForm_Toolbars_Dock_Area_Top
        '
        Me._ModulesWeightForm_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me._ModulesWeightForm_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control
        Me._ModulesWeightForm_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top
        Me._ModulesWeightForm_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText
        resources.ApplyResources(Me._ModulesWeightForm_Toolbars_Dock_Area_Top, "_ModulesWeightForm_Toolbars_Dock_Area_Top")
        Me._ModulesWeightForm_Toolbars_Dock_Area_Top.Name = "_ModulesWeightForm_Toolbars_Dock_Area_Top"
        Me._ModulesWeightForm_Toolbars_Dock_Area_Top.ToolbarsManager = Me.utmModulesWeight
        '
        '_ModulesWeightForm_Toolbars_Dock_Area_Bottom
        '
        Me._ModulesWeightForm_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me._ModulesWeightForm_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control
        Me._ModulesWeightForm_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom
        Me._ModulesWeightForm_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText
        resources.ApplyResources(Me._ModulesWeightForm_Toolbars_Dock_Area_Bottom, "_ModulesWeightForm_Toolbars_Dock_Area_Bottom")
        Me._ModulesWeightForm_Toolbars_Dock_Area_Bottom.Name = "_ModulesWeightForm_Toolbars_Dock_Area_Bottom"
        Me._ModulesWeightForm_Toolbars_Dock_Area_Bottom.ToolbarsManager = Me.utmModulesWeight
        '
        'ModulesWeightForm
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        resources.ApplyResources(Me, "$this")
        Me.Controls.Add(Me.btnExport)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.ugModulesWeight)
        Me.Controls.Add(Me._ModulesWeightForm_Toolbars_Dock_Area_Left)
        Me.Controls.Add(Me._ModulesWeightForm_Toolbars_Dock_Area_Right)
        Me.Controls.Add(Me._ModulesWeightForm_Toolbars_Dock_Area_Bottom)
        Me.Controls.Add(Me._ModulesWeightForm_Toolbars_Dock_Area_Top)
        Me.KeyPreview = True
        Me.Name = "ModulesWeightForm"
        CType(Me.ugModulesWeight, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.udsModulesWeight, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.utmModulesWeight, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ugModulesWeight As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents btnClose As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnExport As Infragistics.Win.Misc.UltraButton
    Friend WithEvents udsModulesWeight As Infragistics.Win.UltraWinDataSource.UltraDataSource
    Friend WithEvents ugeeModulesWeight As Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter
    Friend WithEvents utmModulesWeight As Infragistics.Win.UltraWinToolbars.UltraToolbarsManager
    Friend WithEvents _ModulesWeightForm_Toolbars_Dock_Area_Left As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents _ModulesWeightForm_Toolbars_Dock_Area_Right As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents _ModulesWeightForm_Toolbars_Dock_Area_Bottom As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents _ModulesWeightForm_Toolbars_Dock_Area_Top As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
End Class
