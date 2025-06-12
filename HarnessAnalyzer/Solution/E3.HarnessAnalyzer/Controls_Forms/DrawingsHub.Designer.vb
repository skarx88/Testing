<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DrawingsHub
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            _hcv = Nothing
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DrawingsHub))
        Me.utDrawings = New Infragistics.Win.UltraWinTree.UltraTree()
        Me.upnButton = New Infragistics.Win.Misc.UltraPanel()
        Me.btnCancel = New Infragistics.Win.Misc.UltraButton()
        Me.upnTree = New Infragistics.Win.Misc.UltraPanel()
        CType(Me.utDrawings, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.upnButton.ClientArea.SuspendLayout()
        Me.upnButton.SuspendLayout()
        Me.upnTree.ClientArea.SuspendLayout()
        Me.upnTree.SuspendLayout()
        Me.SuspendLayout()
        '
        'utDrawings
        '
        resources.ApplyResources(Me.utDrawings, "utDrawings")
        Me.utDrawings.Name = "utDrawings"
        '
        'upnButton
        '
        resources.ApplyResources(Me.upnButton, "upnButton")
        '
        'upnButton.ClientArea
        '
        resources.ApplyResources(Me.upnButton.ClientArea, "upnButton.ClientArea")
        Me.upnButton.ClientArea.Controls.Add(Me.btnCancel)
        Me.upnButton.Name = "upnButton"
        '
        'btnCancel
        '
        resources.ApplyResources(Me.btnCancel, "btnCancel")
        Me.btnCancel.Name = "btnCancel"
        '
        'upnTree
        '
        resources.ApplyResources(Me.upnTree, "upnTree")
        '
        'upnTree.ClientArea
        '
        resources.ApplyResources(Me.upnTree.ClientArea, "upnTree.ClientArea")
        Me.upnTree.ClientArea.Controls.Add(Me.utDrawings)
        Me.upnTree.Name = "upnTree"
        '
        'DrawingsHub
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.upnTree)
        Me.Controls.Add(Me.upnButton)
        Me.Name = HarnessAnalyzer.Shared.Common.PaneKeys.DrawingsHub.ToString
        CType(Me.utDrawings, System.ComponentModel.ISupportInitialize).EndInit()
        Me.upnButton.ClientArea.ResumeLayout(False)
        Me.upnButton.ResumeLayout(False)
        Me.upnTree.ClientArea.ResumeLayout(False)
        Me.upnTree.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents utDrawings As Infragistics.Win.UltraWinTree.UltraTree
    Friend WithEvents upnButton As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents upnTree As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents btnCancel As Infragistics.Win.Misc.UltraButton

End Class
