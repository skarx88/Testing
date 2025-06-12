<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ContactingViewerControl
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Me.VDBaseCntrl = New VectorDraw.Professional.Control.VectorDrawBaseControl()
        Me.VdDocumentComponent1 = New VectorDraw.Professional.Components.vdDocumentComponent(Me.components)
        Me.SuspendLayout()
        '
        'VDBaseCntrl
        '
        Me.VDBaseCntrl.AccessibleRole = System.Windows.Forms.AccessibleRole.Window
        Me.VDBaseCntrl.AllowDrop = True
        Me.VDBaseCntrl.Cursor = System.Windows.Forms.Cursors.Default
        Me.VDBaseCntrl.Dock = System.Windows.Forms.DockStyle.Fill
        Me.VDBaseCntrl.Location = New System.Drawing.Point(0, 0)
        Me.VDBaseCntrl.Name = "VDBaseCntrl"
        Me.VDBaseCntrl.Size = New System.Drawing.Size(659, 449)
        Me.VDBaseCntrl.TabIndex = 0
        '
        'CVControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.VDBaseCntrl)
        Me.Name = "CVControl"
        Me.Size = New System.Drawing.Size(659, 449)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents VDBaseCntrl As VectorDraw.Professional.Control.VectorDrawBaseControl
    Friend WithEvents VdDocumentComponent1 As VectorDraw.Professional.Components.vdDocumentComponent
End Class
