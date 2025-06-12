<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class GridAppearanceForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(GridAppearanceForm))
        Dim Override1 As Infragistics.Win.UltraWinTree.Override = New Infragistics.Win.UltraWinTree.Override()
        Me.ugbGridAppearance = New Infragistics.Win.Misc.UltraGroupBox()
        Me.txtFilter = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.UltraLabel1 = New Infragistics.Win.Misc.UltraLabel()
        Me.utGridAppearance = New Infragistics.Win.UltraWinTree.UltraTree()
        Me.btnCancel = New Infragistics.Win.Misc.UltraButton()
        Me.btnOK = New Infragistics.Win.Misc.UltraButton()
        Me.btnReset = New Infragistics.Win.Misc.UltraButton()
        Me.utmGridAppearance = New Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(Me.components)
        Me._GridAppearanceForm_Toolbars_Dock_Area_Left = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        Me._GridAppearanceForm_Toolbars_Dock_Area_Right = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        Me._GridAppearanceForm_Toolbars_Dock_Area_Top = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        Me._GridAppearanceForm_Toolbars_Dock_Area_Bottom = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        Me.UltraToolTipManager = New Infragistics.Win.UltraWinToolTip.UltraToolTipManager(Me.components)
        CType(Me.ugbGridAppearance, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ugbGridAppearance.SuspendLayout()
        CType(Me.txtFilter, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.utGridAppearance, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.utmGridAppearance, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ugbGridAppearance
        '
        resources.ApplyResources(Me.ugbGridAppearance, "ugbGridAppearance")
        Me.ugbGridAppearance.Controls.Add(Me.txtFilter)
        Me.ugbGridAppearance.Controls.Add(Me.UltraLabel1)
        Me.ugbGridAppearance.Controls.Add(Me.utGridAppearance)
        Me.ugbGridAppearance.Name = "ugbGridAppearance"
        '
        'txtFilter
        '
        resources.ApplyResources(Me.txtFilter, "txtFilter")
        Me.txtFilter.Name = "txtFilter"
        '
        'UltraLabel1
        '
        resources.ApplyResources(Me.UltraLabel1, "UltraLabel1")
        Me.UltraLabel1.Name = "UltraLabel1"
        '
        'utGridAppearance
        '
        resources.ApplyResources(Me.utGridAppearance, "utGridAppearance")
        Me.utGridAppearance.Name = "utGridAppearance"
        Override1.SelectionType = Infragistics.Win.UltraWinTree.SelectType.Extended
        Me.utGridAppearance.Override = Override1
        '
        'btnCancel
        '
        resources.ApplyResources(Me.btnCancel, "btnCancel")
        Me.btnCancel.Name = "btnCancel"
        '
        'btnOK
        '
        resources.ApplyResources(Me.btnOK, "btnOK")
        Me.btnOK.Name = "btnOK"
        '
        'btnReset
        '
        resources.ApplyResources(Me.btnReset, "btnReset")
        Me.btnReset.Name = "btnReset"
        '
        'utmGridAppearance
        '
        Me.utmGridAppearance.DesignerFlags = 1
        Me.utmGridAppearance.DockWithinContainer = Me
        Me.utmGridAppearance.DockWithinContainerBaseType = GetType(System.Windows.Forms.Form)
        Me.utmGridAppearance.ShowFullMenusDelay = 500
        '
        '_GridAppearanceForm_Toolbars_Dock_Area_Left
        '
        Me._GridAppearanceForm_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me._GridAppearanceForm_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control
        Me._GridAppearanceForm_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left
        Me._GridAppearanceForm_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText
        resources.ApplyResources(Me._GridAppearanceForm_Toolbars_Dock_Area_Left, "_GridAppearanceForm_Toolbars_Dock_Area_Left")
        Me._GridAppearanceForm_Toolbars_Dock_Area_Left.Name = "_GridAppearanceForm_Toolbars_Dock_Area_Left"
        Me._GridAppearanceForm_Toolbars_Dock_Area_Left.ToolbarsManager = Me.utmGridAppearance
        '
        '_GridAppearanceForm_Toolbars_Dock_Area_Right
        '
        Me._GridAppearanceForm_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me._GridAppearanceForm_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control
        Me._GridAppearanceForm_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right
        Me._GridAppearanceForm_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText
        resources.ApplyResources(Me._GridAppearanceForm_Toolbars_Dock_Area_Right, "_GridAppearanceForm_Toolbars_Dock_Area_Right")
        Me._GridAppearanceForm_Toolbars_Dock_Area_Right.Name = "_GridAppearanceForm_Toolbars_Dock_Area_Right"
        Me._GridAppearanceForm_Toolbars_Dock_Area_Right.ToolbarsManager = Me.utmGridAppearance
        '
        '_GridAppearanceForm_Toolbars_Dock_Area_Top
        '
        Me._GridAppearanceForm_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me._GridAppearanceForm_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control
        Me._GridAppearanceForm_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top
        Me._GridAppearanceForm_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText
        resources.ApplyResources(Me._GridAppearanceForm_Toolbars_Dock_Area_Top, "_GridAppearanceForm_Toolbars_Dock_Area_Top")
        Me._GridAppearanceForm_Toolbars_Dock_Area_Top.Name = "_GridAppearanceForm_Toolbars_Dock_Area_Top"
        Me._GridAppearanceForm_Toolbars_Dock_Area_Top.ToolbarsManager = Me.utmGridAppearance
        '
        '_GridAppearanceForm_Toolbars_Dock_Area_Bottom
        '
        Me._GridAppearanceForm_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me._GridAppearanceForm_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control
        Me._GridAppearanceForm_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom
        Me._GridAppearanceForm_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText
        resources.ApplyResources(Me._GridAppearanceForm_Toolbars_Dock_Area_Bottom, "_GridAppearanceForm_Toolbars_Dock_Area_Bottom")
        Me._GridAppearanceForm_Toolbars_Dock_Area_Bottom.Name = "_GridAppearanceForm_Toolbars_Dock_Area_Bottom"
        Me._GridAppearanceForm_Toolbars_Dock_Area_Bottom.ToolbarsManager = Me.utmGridAppearance
        '
        'UltraToolTipManager
        '
        Me.UltraToolTipManager.ContainingControl = Me
        Me.UltraToolTipManager.DisplayStyle = Infragistics.Win.ToolTipDisplayStyle.Standard
        '
        'GridAppearanceForm
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        resources.ApplyResources(Me, "$this")
        Me.Controls.Add(Me.btnReset)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.ugbGridAppearance)
        Me.Controls.Add(Me._GridAppearanceForm_Toolbars_Dock_Area_Left)
        Me.Controls.Add(Me._GridAppearanceForm_Toolbars_Dock_Area_Right)
        Me.Controls.Add(Me._GridAppearanceForm_Toolbars_Dock_Area_Bottom)
        Me.Controls.Add(Me._GridAppearanceForm_Toolbars_Dock_Area_Top)
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "GridAppearanceForm"
        CType(Me.ugbGridAppearance, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ugbGridAppearance.ResumeLayout(False)
        Me.ugbGridAppearance.PerformLayout()
        CType(Me.txtFilter, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.utGridAppearance, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.utmGridAppearance, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ugbGridAppearance As Infragistics.Win.Misc.UltraGroupBox
    Friend WithEvents btnCancel As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnOK As Infragistics.Win.Misc.UltraButton
    Friend WithEvents utGridAppearance As Infragistics.Win.UltraWinTree.UltraTree
    Friend WithEvents btnReset As Infragistics.Win.Misc.UltraButton
    Friend WithEvents utmGridAppearance As Infragistics.Win.UltraWinToolbars.UltraToolbarsManager
    Friend WithEvents _GridAppearanceForm_Toolbars_Dock_Area_Left As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents _GridAppearanceForm_Toolbars_Dock_Area_Right As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents _GridAppearanceForm_Toolbars_Dock_Area_Bottom As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents _GridAppearanceForm_Toolbars_Dock_Area_Top As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents UltraToolTipManager As Infragistics.Win.UltraWinToolTip.UltraToolTipManager
    Friend WithEvents txtFilter As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents UltraLabel1 As Infragistics.Win.Misc.UltraLabel
End Class
