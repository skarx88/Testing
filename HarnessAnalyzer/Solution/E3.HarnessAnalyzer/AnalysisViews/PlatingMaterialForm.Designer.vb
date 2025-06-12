<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PlatingMaterialForm
    Inherits AnalysisForm

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
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

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(PlatingMaterialForm))
        Me.btnView = New Infragistics.Win.Misc.UltraButton()
        Me.btnCancel = New Infragistics.Win.Misc.UltraButton()
        Me.ugbPlatingMaterial = New Infragistics.Win.Misc.UltraGroupBox()
        Me.ucePlatingMaterial = New Infragistics.Win.UltraWinEditors.UltraComboEditor()
        CType(Me.ugbPlatingMaterial, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ugbPlatingMaterial.SuspendLayout()
        CType(Me.ucePlatingMaterial, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnView
        '
        resources.ApplyResources(Me.btnView, "btnView")
        Me.btnView.Name = "btnView"
        '
        'btnCancel
        '
        Me.btnCancel.AccessibleRole = System.Windows.Forms.AccessibleRole.None
        resources.ApplyResources(Me.btnCancel, "btnCancel")
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Name = "btnCancel"
        '
        'ugbPlatingMaterial
        '
        resources.ApplyResources(Me.ugbPlatingMaterial, "ugbPlatingMaterial")
        Me.ugbPlatingMaterial.Controls.Add(Me.ucePlatingMaterial)
        Me.ugbPlatingMaterial.Name = "ugbPlatingMaterial"
        '
        'ucePlatingMaterial
        '
        resources.ApplyResources(Me.ucePlatingMaterial, "ucePlatingMaterial")
        Me.ucePlatingMaterial.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList
        Me.ucePlatingMaterial.Name = "ucePlatingMaterial"
        '
        'PlatingMaterialForm
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.BackColor = System.Drawing.Color.White
        resources.ApplyResources(Me, "$this")
        Me.Controls.Add(Me.btnView)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.ugbPlatingMaterial)
        Me.Name = "PlatingMaterialForm"
        CType(Me.ugbPlatingMaterial, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ugbPlatingMaterial.ResumeLayout(False)
        Me.ugbPlatingMaterial.PerformLayout()
        CType(Me.ucePlatingMaterial, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents ugbPlatingMaterial As Infragistics.Win.Misc.UltraGroupBox
    Friend WithEvents ucePlatingMaterial As Infragistics.Win.UltraWinEditors.UltraComboEditor
    Friend WithEvents btnView As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnCancel As Infragistics.Win.Misc.UltraButton
End Class
