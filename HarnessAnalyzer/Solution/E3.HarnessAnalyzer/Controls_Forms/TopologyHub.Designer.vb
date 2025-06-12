<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TopologyHub
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            _carTopologyViewFile = Nothing
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(TopologyHub))
        vDraw = New VectorDraw.Professional.Control.VectorDrawBaseControl()
        btnReload = New Infragistics.Win.Misc.UltraButton()
        SuspendLayout()
        ' 
        ' vDraw
        ' 
        vDraw.AccessibleRole = AccessibleRole.Window
        vDraw.AllowDrop = True
        resources.ApplyResources(vDraw, "vDraw")
        vDraw.Name = "vDraw"
        ' 
        ' btnReload
        ' 
        resources.ApplyResources(btnReload, "btnReload")
        btnReload.Name = "btnReload"
        ' 
        ' TopologyHub
        ' 
        resources.ApplyResources(Me, "$this")
        AutoScaleMode = AutoScaleMode.Font
        Controls.Add(btnReload)
        Controls.Add(vDraw)
        Name = "TopologyHub"
        ResumeLayout(False)

    End Sub
    Friend WithEvents vDraw As VectorDraw.Professional.Control.VectorDrawBaseControl
    Friend WithEvents btnReload As Infragistics.Win.Misc.UltraButton

End Class
