<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MemolistHub
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MemolistHub))
        Me.utMemolist = New Infragistics.Win.UltraWinTree.UltraTree()
        Me.utmMemolist = New Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(Me.components)
        Me._MemolistHub_Toolbars_Dock_Area_Left = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        Me._MemolistHub_Toolbars_Dock_Area_Right = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        Me._MemolistHub_Toolbars_Dock_Area_Top = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        Me._MemolistHub_Toolbars_Dock_Area_Bottom = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        CType(Me.utMemolist, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.utmMemolist, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'utMemolist
        '
        resources.ApplyResources(Me.utMemolist, "utMemolist")
        Me.utMemolist.Name = "utMemolist"
        '
        'utmMemolist
        '
        Me.utmMemolist.DesignerFlags = 1
        Me.utmMemolist.DockWithinContainer = Me
        '
        '_MemolistHub_Toolbars_Dock_Area_Left
        '
        resources.ApplyResources(Me._MemolistHub_Toolbars_Dock_Area_Left, "_MemolistHub_Toolbars_Dock_Area_Left")
        Me._MemolistHub_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me._MemolistHub_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control
        Me._MemolistHub_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left
        Me._MemolistHub_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText
        Me._MemolistHub_Toolbars_Dock_Area_Left.Name = "_MemolistHub_Toolbars_Dock_Area_Left"
        Me._MemolistHub_Toolbars_Dock_Area_Left.ToolbarsManager = Me.utmMemolist
        '
        '_MemolistHub_Toolbars_Dock_Area_Right
        '
        resources.ApplyResources(Me._MemolistHub_Toolbars_Dock_Area_Right, "_MemolistHub_Toolbars_Dock_Area_Right")
        Me._MemolistHub_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me._MemolistHub_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control
        Me._MemolistHub_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right
        Me._MemolistHub_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText
        Me._MemolistHub_Toolbars_Dock_Area_Right.Name = "_MemolistHub_Toolbars_Dock_Area_Right"
        Me._MemolistHub_Toolbars_Dock_Area_Right.ToolbarsManager = Me.utmMemolist
        '
        '_MemolistHub_Toolbars_Dock_Area_Top
        '
        resources.ApplyResources(Me._MemolistHub_Toolbars_Dock_Area_Top, "_MemolistHub_Toolbars_Dock_Area_Top")
        Me._MemolistHub_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me._MemolistHub_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control
        Me._MemolistHub_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top
        Me._MemolistHub_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText
        Me._MemolistHub_Toolbars_Dock_Area_Top.Name = "_MemolistHub_Toolbars_Dock_Area_Top"
        Me._MemolistHub_Toolbars_Dock_Area_Top.ToolbarsManager = Me.utmMemolist
        '
        '_MemolistHub_Toolbars_Dock_Area_Bottom
        '
        resources.ApplyResources(Me._MemolistHub_Toolbars_Dock_Area_Bottom, "_MemolistHub_Toolbars_Dock_Area_Bottom")
        Me._MemolistHub_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me._MemolistHub_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control
        Me._MemolistHub_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom
        Me._MemolistHub_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText
        Me._MemolistHub_Toolbars_Dock_Area_Bottom.Name = "_MemolistHub_Toolbars_Dock_Area_Bottom"
        Me._MemolistHub_Toolbars_Dock_Area_Bottom.ToolbarsManager = Me.utmMemolist
        '
        'MemolistHub
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.utMemolist)
        Me.Controls.Add(Me._MemolistHub_Toolbars_Dock_Area_Left)
        Me.Controls.Add(Me._MemolistHub_Toolbars_Dock_Area_Right)
        Me.Controls.Add(Me._MemolistHub_Toolbars_Dock_Area_Bottom)
        Me.Controls.Add(Me._MemolistHub_Toolbars_Dock_Area_Top)
        Me.Name = "MemolistHub"
        CType(Me.utMemolist, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.utmMemolist, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents utMemolist As Infragistics.Win.UltraWinTree.UltraTree
    Friend WithEvents utmMemolist As Infragistics.Win.UltraWinToolbars.UltraToolbarsManager
    Friend WithEvents _MemolistHub_Toolbars_Dock_Area_Left As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents _MemolistHub_Toolbars_Dock_Area_Right As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents _MemolistHub_Toolbars_Dock_Area_Bottom As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents _MemolistHub_Toolbars_Dock_Area_Top As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea

End Class
