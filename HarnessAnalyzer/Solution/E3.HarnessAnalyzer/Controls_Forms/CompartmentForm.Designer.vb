<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class CompartmentForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(CompartmentForm))
        Me.lblHarnessPartNumber = New Infragistics.Win.Misc.UltraLabel()
        Me.txtHarnessPartNumber = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.lblHarnessDescription = New Infragistics.Win.Misc.UltraLabel()
        Me.txtHarnessDescription = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.btnCancel = New Infragistics.Win.Misc.UltraButton()
        Me.btnOK = New Infragistics.Win.Misc.UltraButton()
        CType(Me.txtHarnessPartNumber, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtHarnessDescription, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblHarnessPartNumber
        '
        resources.ApplyResources(Me.lblHarnessPartNumber, "lblHarnessPartNumber")
        Me.lblHarnessPartNumber.Name = "lblHarnessPartNumber"
        '
        'txtHarnessPartNumber
        '
        resources.ApplyResources(Me.txtHarnessPartNumber, "txtHarnessPartNumber")
        Me.txtHarnessPartNumber.MaxLength = 255
        Me.txtHarnessPartNumber.Name = "txtHarnessPartNumber"
        Me.txtHarnessPartNumber.Nullable = False
        '
        'lblHarnessDescription
        '
        resources.ApplyResources(Me.lblHarnessDescription, "lblHarnessDescription")
        Me.lblHarnessDescription.Name = "lblHarnessDescription"
        '
        'txtHarnessDescription
        '
        resources.ApplyResources(Me.txtHarnessDescription, "txtHarnessDescription")
        Me.txtHarnessDescription.MaxLength = 255
        Me.txtHarnessDescription.Name = "txtHarnessDescription"
        '
        'btnCancel
        '
        resources.ApplyResources(Me.btnCancel, "btnCancel")
        Me.btnCancel.Name = "btnCancel"
        '
        'btnOK
        '
        resources.ApplyResources(Me.btnOK, "btnOK")
        Me.btnOK.Name = "btnOK"
        '
        'CompartmentForm
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.txtHarnessDescription)
        Me.Controls.Add(Me.lblHarnessDescription)
        Me.Controls.Add(Me.txtHarnessPartNumber)
        Me.Controls.Add(Me.lblHarnessPartNumber)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "CompartmentForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        CType(Me.txtHarnessPartNumber, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtHarnessDescription, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblHarnessPartNumber As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents txtHarnessPartNumber As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents lblHarnessDescription As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents txtHarnessDescription As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents btnCancel As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnOK As Infragistics.Win.Misc.UltraButton
End Class
