<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UltrasonicSpliceDetails
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UltrasonicSpliceDetails))
        Me.ugUltrasonicSplice = New Infragistics.Win.UltraWinGrid.UltraGrid()
        Me.udsUltrasonicSplice = New Infragistics.Win.UltraWinDataSource.UltraDataSource(Me.components)
        CType(Me.ugUltrasonicSplice, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.udsUltrasonicSplice, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ugUltrasonicSplice
        '
        resources.ApplyResources(Me.ugUltrasonicSplice, "ugUltrasonicSplice")
        Me.ugUltrasonicSplice.Name = "ugUltrasonicSplice"
        '
        'udsUltrasonicSplice
        '
        Me.udsUltrasonicSplice.AllowAdd = False
        Me.udsUltrasonicSplice.AllowDelete = False
        '
        'UltrasonicSpliceDetails
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.ugUltrasonicSplice)
        Me.Name = "UltrasonicSpliceDetails"
        CType(Me.ugUltrasonicSplice, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.udsUltrasonicSplice, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents ugUltrasonicSplice As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents udsUltrasonicSplice As Infragistics.Win.UltraWinDataSource.UltraDataSource
End Class
