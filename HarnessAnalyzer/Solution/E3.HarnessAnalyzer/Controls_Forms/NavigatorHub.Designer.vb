<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class NavigatorHub
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing Then
                components?.Dispose()
            End If
            _vdCanvas = Nothing
            _document = Nothing
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(NavigatorHub))
        Me.upbNavigator = New Infragistics.Win.UltraWinEditors.UltraPictureBox()
        Me.SuspendLayout()
        '
        'upbNavigator
        '
        resources.ApplyResources(Me.upbNavigator, "upbNavigator")
        Me.upbNavigator.BorderShadowColor = System.Drawing.Color.Empty
        Me.upbNavigator.Name = "upbNavigator"
        '
        'NavigatorHub
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.upbNavigator)
        Me.Name = HarnessAnalyzer.Shared.Common.PaneKeys.NavigatorHub.ToString
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents upbNavigator As Infragistics.Win.UltraWinEditors.UltraPictureBox

End Class
