<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class InlinersForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If

            _inlinerPairCheckClassifications = Nothing
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(InlinersForm))
        Me.btnClose = New Infragistics.Win.Misc.UltraButton()
        Me.upbInliners = New Infragistics.Win.Misc.UltraGroupBox()
        Me.uebInliners = New Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar()
        Me.btnExport = New Infragistics.Win.Misc.UltraButton()
        Me.lblInfo = New Infragistics.Win.Misc.UltraLabel()
        Me.ProgressPanel = New System.Windows.Forms.Panel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.UltraProgressBar1 = New Infragistics.Win.UltraWinProgressBar.UltraProgressBar()
        CType(Me.upbInliners, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.upbInliners.SuspendLayout()
        CType(Me.uebInliners, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ProgressPanel.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnClose
        '
        resources.ApplyResources(Me.btnClose, "btnClose")
        Me.btnClose.Name = "btnClose"
        '
        'upbInliners
        '
        resources.ApplyResources(Me.upbInliners, "upbInliners")
        Me.upbInliners.Controls.Add(Me.uebInliners)
        Me.upbInliners.Name = "upbInliners"
        '
        'uebInliners
        '
        resources.ApplyResources(Me.uebInliners, "uebInliners")
        Me.uebInliners.Name = "uebInliners"
        '
        'btnExport
        '
        resources.ApplyResources(Me.btnExport, "btnExport")
        Me.btnExport.Name = "btnExport"
        '
        'lblInfo
        '
        resources.ApplyResources(Me.lblInfo, "lblInfo")
        Me.lblInfo.Name = "lblInfo"
        '
        'ProgressPanel
        '
        resources.ApplyResources(Me.ProgressPanel, "ProgressPanel")
        Me.ProgressPanel.Controls.Add(Me.UltraProgressBar1)
        Me.ProgressPanel.Controls.Add(Me.Label1)
        Me.ProgressPanel.Name = "ProgressPanel"
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
        '
        'UltraProgressBar1
        '
        resources.ApplyResources(Me.UltraProgressBar1, "UltraProgressBar1")
        Me.UltraProgressBar1.Name = "UltraProgressBar1"
        '
        'InlinersForm
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        resources.ApplyResources(Me, "$this")
        Me.Controls.Add(Me.lblInfo)
        Me.Controls.Add(Me.ProgressPanel)
        Me.Controls.Add(Me.btnExport)
        Me.Controls.Add(Me.upbInliners)
        Me.Controls.Add(Me.btnClose)
        Me.Name = "InlinersForm"
        CType(Me.upbInliners, System.ComponentModel.ISupportInitialize).EndInit()
        Me.upbInliners.ResumeLayout(False)
        CType(Me.uebInliners, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ProgressPanel.ResumeLayout(False)
        Me.ProgressPanel.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnClose As Infragistics.Win.Misc.UltraButton
    Friend WithEvents upbInliners As Infragistics.Win.Misc.UltraGroupBox
    Friend WithEvents uebInliners As Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar
    Friend WithEvents btnExport As Infragistics.Win.Misc.UltraButton
    Friend WithEvents lblInfo As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents ProgressPanel As Panel
    Friend WithEvents Label1 As Label
    Friend WithEvents UltraProgressBar1 As Infragistics.Win.UltraWinProgressBar.UltraProgressBar
End Class
