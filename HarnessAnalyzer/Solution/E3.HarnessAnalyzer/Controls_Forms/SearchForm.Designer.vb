<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SearchForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SearchForm))
        Me.lblSearchString = New Infragistics.Win.Misc.UltraLabel()
        Me.txtSearchString = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.uckBeginsWith = New Infragistics.Win.UltraWinEditors.UltraCheckEditor()
        Me.uckCaseSensitive = New Infragistics.Win.UltraWinEditors.UltraCheckEditor()
        Me.ugbSearchResults = New Infragistics.Win.Misc.UltraGroupBox()
        Me.utSearchResults = New Infragistics.Win.UltraWinTree.UltraTree()
        Me.btnClose = New Infragistics.Win.Misc.UltraButton()
        Me.uckIgnoreBlanks = New Infragistics.Win.UltraWinEditors.UltraCheckEditor()
        CType(Me.txtSearchString, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.uckBeginsWith, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.uckCaseSensitive, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ugbSearchResults, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ugbSearchResults.SuspendLayout()
        CType(Me.utSearchResults, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.uckIgnoreBlanks, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblSearchString
        '
        resources.ApplyResources(Me.lblSearchString, "lblSearchString")
        Me.lblSearchString.Name = "lblSearchString"
        '
        'txtSearchString
        '
        resources.ApplyResources(Me.txtSearchString, "txtSearchString")
        Me.txtSearchString.Name = "txtSearchString"
        '
        'uckBeginsWith
        '
        resources.ApplyResources(Me.uckBeginsWith, "uckBeginsWith")
        Me.uckBeginsWith.Name = "uckBeginsWith"
        '
        'uckCaseSensitive
        '
        resources.ApplyResources(Me.uckCaseSensitive, "uckCaseSensitive")
        Me.uckCaseSensitive.Name = "uckCaseSensitive"
        '
        'ugbSearchResults
        '
        resources.ApplyResources(Me.ugbSearchResults, "ugbSearchResults")
        Me.ugbSearchResults.Controls.Add(Me.utSearchResults)
        Me.ugbSearchResults.Name = "ugbSearchResults"
        '
        'utSearchResults
        '
        resources.ApplyResources(Me.utSearchResults, "utSearchResults")
        Me.utSearchResults.Name = "utSearchResults"
        '
        'btnClose
        '
        resources.ApplyResources(Me.btnClose, "btnClose")
        Me.btnClose.Name = "btnClose"
        '
        'uckIgnoreBlanks
        '
        resources.ApplyResources(Me.uckIgnoreBlanks, "uckIgnoreBlanks")
        Me.uckIgnoreBlanks.Name = "uckIgnoreBlanks"
        '
        'SearchForm
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.Controls.Add(Me.uckIgnoreBlanks)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.ugbSearchResults)
        Me.Controls.Add(Me.uckCaseSensitive)
        Me.Controls.Add(Me.uckBeginsWith)
        Me.Controls.Add(Me.txtSearchString)
        Me.Controls.Add(Me.lblSearchString)
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "SearchForm"
        CType(Me.txtSearchString, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.uckBeginsWith, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.uckCaseSensitive, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ugbSearchResults, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ugbSearchResults.ResumeLayout(False)
        CType(Me.utSearchResults, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.uckIgnoreBlanks, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblSearchString As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents txtSearchString As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents uckBeginsWith As Infragistics.Win.UltraWinEditors.UltraCheckEditor
    Friend WithEvents uckCaseSensitive As Infragistics.Win.UltraWinEditors.UltraCheckEditor
    Friend WithEvents ugbSearchResults As Infragistics.Win.Misc.UltraGroupBox
    Friend WithEvents btnClose As Infragistics.Win.Misc.UltraButton
    Friend WithEvents utSearchResults As Infragistics.Win.UltraWinTree.UltraTree
    Friend WithEvents uckIgnoreBlanks As Infragistics.Win.UltraWinEditors.UltraCheckEditor
End Class
