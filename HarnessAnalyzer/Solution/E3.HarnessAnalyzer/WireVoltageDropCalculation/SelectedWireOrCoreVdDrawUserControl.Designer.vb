<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SelectedWireOrCoreVdDrawUserControl
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
        Me.vDraw = New VectorDraw.Professional.Control.VectorDrawBaseControl()
        Me.SuspendLayout()
        '
        'vDraw
        '
        'Me.vDraw.AccessibleRole = System.Windows.Forms.AccessibleRole.Window
        Me.vDraw.AllowDrop = True
        Me.vDraw.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.vDraw.Dock = System.Windows.Forms.DockStyle.Fill
        Me.vDraw.Location = New System.Drawing.Point(0, 0)
        Me.vDraw.Name = "vDraw"
        Me.vDraw.Size = New System.Drawing.Size(1639, 610)
        Me.vDraw.TabIndex = 3
        '
        'SelectedWireOrCoreVdDrawUserControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(16.0!, 31.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.vDraw)
        Me.Name = "SelectedWireOrCoreVdDrawUserControl"
        Me.Size = New System.Drawing.Size(1639, 610)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents vDraw As VectorDraw.Professional.Control.VectorDrawBaseControl
End Class
