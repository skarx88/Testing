<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PasteResultForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(PasteResultForm))
        Me.ugbPasteResults = New Infragistics.Win.Misc.UltraGroupBox()
        Me.ulvPasteResults = New Infragistics.Win.UltraWinListView.UltraListView()
        Me.btnOK = New Infragistics.Win.Misc.UltraButton()
        Me.btnCancel = New Infragistics.Win.Misc.UltraButton()
        CType(Me.ugbPasteResults, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ugbPasteResults.SuspendLayout()
        CType(Me.ulvPasteResults, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ugbPasteResults
        '
        resources.ApplyResources(Me.ugbPasteResults, "ugbPasteResults")
        Me.ugbPasteResults.Controls.Add(Me.ulvPasteResults)
        Me.ugbPasteResults.Name = "ugbPasteResults"
        '
        'ulvPasteResults
        '
        resources.ApplyResources(Me.ulvPasteResults, "ulvPasteResults")
        Me.ulvPasteResults.Name = "ulvPasteResults"
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
        'PasteResultForm
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.ugbPasteResults)
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "PasteResultForm"
        CType(Me.ugbPasteResults, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ugbPasteResults.ResumeLayout(False)
        CType(Me.ulvPasteResults, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ugbPasteResults As Infragistics.Win.Misc.UltraGroupBox
    Friend WithEvents btnOK As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnCancel As Infragistics.Win.Misc.UltraButton
    Friend WithEvents ulvPasteResults As Infragistics.Win.UltraWinListView.UltraListView
End Class
