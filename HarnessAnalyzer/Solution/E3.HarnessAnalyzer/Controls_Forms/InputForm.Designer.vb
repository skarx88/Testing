<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class InputForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(InputForm))
        Me.lblPrompt = New Infragistics.Win.Misc.UltraLabel()
        Me.btnOK = New Infragistics.Win.Misc.UltraButton()
        Me.btnCancel = New Infragistics.Win.Misc.UltraButton()
        Me.uceResponse = New Infragistics.Win.UltraWinEditors.UltraComboEditor()
        Me.txtResponse = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        CType(Me.uceResponse, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtResponse, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblPrompt
        '
        resources.ApplyResources(Me.lblPrompt, "lblPrompt")
        Me.lblPrompt.Name = "lblPrompt"
        '
        'btnOK
        '
        resources.ApplyResources(Me.btnOK, "btnOK")
        Me.btnOK.Name = "btnOK"
        '
        'btnCancel
        '
        resources.ApplyResources(Me.btnCancel, "btnCancel")
        Me.btnCancel.Name = "btnCancel"
        '
        'uceResponse
        '
        resources.ApplyResources(Me.uceResponse, "uceResponse")
        Me.uceResponse.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList
        Me.uceResponse.Name = "uceResponse"
        '
        'txtResponse
        '
        resources.ApplyResources(Me.txtResponse, "txtResponse")
        Me.txtResponse.Name = "txtResponse"
        '
        'InputForm
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.Controls.Add(Me.txtResponse)
        Me.Controls.Add(Me.uceResponse)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.lblPrompt)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "InputForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        CType(Me.uceResponse, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtResponse, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblPrompt As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents btnOK As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnCancel As Infragistics.Win.Misc.UltraButton
    Friend WithEvents uceResponse As Infragistics.Win.UltraWinEditors.UltraComboEditor
    Friend WithEvents txtResponse As Infragistics.Win.UltraWinEditors.UltraTextEditor
End Class
