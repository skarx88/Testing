<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UltrasonicSpliceTerminalDistanceForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UltrasonicSpliceTerminalDistanceForm))
        Me.btnExport = New Infragistics.Win.Misc.UltraButton()
        Me.upbUSSpliceTerminalDistances = New Infragistics.Win.Misc.UltraGroupBox()
        Me.uebUSSpliceTerminalDistances = New Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar()
        Me.btnClose = New Infragistics.Win.Misc.UltraButton()
        Me.lblInfo = New Infragistics.Win.Misc.UltraLabel()
        CType(Me.upbUSSpliceTerminalDistances, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.upbUSSpliceTerminalDistances.SuspendLayout()
        CType(Me.uebUSSpliceTerminalDistances, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnExport
        '
        resources.ApplyResources(Me.btnExport, "btnExport")
        Me.btnExport.Name = "btnExport"
        '
        'upbUSSpliceTerminalDistances
        '
        resources.ApplyResources(Me.upbUSSpliceTerminalDistances, "upbUSSpliceTerminalDistances")
        Me.upbUSSpliceTerminalDistances.Controls.Add(Me.uebUSSpliceTerminalDistances)
        Me.upbUSSpliceTerminalDistances.Name = "upbUSSpliceTerminalDistances"
        '
        'uebUSSpliceTerminalDistances
        '
        resources.ApplyResources(Me.uebUSSpliceTerminalDistances, "uebUSSpliceTerminalDistances")
        Me.uebUSSpliceTerminalDistances.Name = "uebUSSpliceTerminalDistances"
        '
        'btnClose
        '
        resources.ApplyResources(Me.btnClose, "btnClose")
        Me.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.btnClose.Name = "btnClose"
        '
        'lblInfo
        '
        resources.ApplyResources(Me.lblInfo, "lblInfo")
        Me.lblInfo.Name = "lblInfo"
        '
        'UltrasonicSpliceTerminalDistanceForm
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        resources.ApplyResources(Me, "$this")
        Me.Controls.Add(Me.lblInfo)
        Me.Controls.Add(Me.btnExport)
        Me.Controls.Add(Me.upbUSSpliceTerminalDistances)
        Me.Controls.Add(Me.btnClose)
        Me.KeyPreview = True
        Me.Name = "UltrasonicSpliceTerminalDistanceForm"
        CType(Me.upbUSSpliceTerminalDistances, System.ComponentModel.ISupportInitialize).EndInit()
        Me.upbUSSpliceTerminalDistances.ResumeLayout(False)
        CType(Me.uebUSSpliceTerminalDistances, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents btnExport As Infragistics.Win.Misc.UltraButton
    Friend WithEvents upbUSSpliceTerminalDistances As Infragistics.Win.Misc.UltraGroupBox
    Friend WithEvents uebUSSpliceTerminalDistances As Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar
    Friend WithEvents btnClose As Infragistics.Win.Misc.UltraButton
    Friend WithEvents lblInfo As Infragistics.Win.Misc.UltraLabel
End Class
