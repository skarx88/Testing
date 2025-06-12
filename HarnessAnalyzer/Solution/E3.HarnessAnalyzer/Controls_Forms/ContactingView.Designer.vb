<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ContactingView
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ContactingView))
        Me.UltraPanel1 = New Infragistics.Win.Misc.UltraPanel()
        Me.RedrawBtn = New Infragistics.Win.Misc.UltraButton()
        Me.CloseBtn = New Infragistics.Win.Misc.UltraButton()
        Me.UltraPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'UltraPanel1
        '
        resources.ApplyResources(Me.UltraPanel1, "UltraPanel1")
        Me.UltraPanel1.Name = "UltraPanel1"
        '
        'RedrawBtn
        '
        resources.ApplyResources(Me.RedrawBtn, "RedrawBtn")
        Me.RedrawBtn.Name = "RedrawBtn"
        '
        'CloseBtn
        '
        resources.ApplyResources(Me.CloseBtn, "CloseBtn")
        Me.CloseBtn.Name = "CloseBtn"
        '
        'ContactingView
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        resources.ApplyResources(Me, "$this")
        Me.Controls.Add(Me.CloseBtn)
        Me.Controls.Add(Me.RedrawBtn)
        Me.Controls.Add(Me.UltraPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
        Me.MinimizeBox = False
        Me.Name = "ContactingView"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.UltraPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents UltraPanel1 As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents RedrawBtn As Infragistics.Win.Misc.UltraButton
    Friend WithEvents CloseBtn As Infragistics.Win.Misc.UltraButton
End Class
