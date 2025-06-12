<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DryWetForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DryWetForm))
        Dim ValueListItem3 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
        Dim ValueListItem4 As Infragistics.Win.ValueListItem = New Infragistics.Win.ValueListItem()
        Me.ugbEnvironmentSetting = New Infragistics.Win.Misc.UltraGroupBox()
        Me.uosEnvironmentSetting = New Infragistics.Win.UltraWinEditors.UltraOptionSet()
        Me.btnView = New Infragistics.Win.Misc.UltraButton()
        Me.btnClose = New Infragistics.Win.Misc.UltraButton()
        CType(Me.ugbEnvironmentSetting, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ugbEnvironmentSetting.SuspendLayout()
        CType(Me.uosEnvironmentSetting, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ugbEnvironmentSetting
        '
        resources.ApplyResources(Me.ugbEnvironmentSetting, "ugbEnvironmentSetting")
        Me.ugbEnvironmentSetting.Controls.Add(Me.uosEnvironmentSetting)
        Me.ugbEnvironmentSetting.Name = "ugbEnvironmentSetting"
        '
        'uosEnvironmentSetting
        '
        resources.ApplyResources(Me.uosEnvironmentSetting, "uosEnvironmentSetting")
        Me.uosEnvironmentSetting.BorderStyle = Infragistics.Win.UIElementBorderStyle.None
        ValueListItem3.DataValue = "dry"
        resources.ApplyResources(ValueListItem3, "ValueListItem3")
        ValueListItem3.ForceApplyResources = ""
        ValueListItem4.DataValue = "wet"
        resources.ApplyResources(ValueListItem4, "ValueListItem4")
        ValueListItem4.ForceApplyResources = ""
        Me.uosEnvironmentSetting.Items.AddRange(New Infragistics.Win.ValueListItem() {ValueListItem3, ValueListItem4})
        Me.uosEnvironmentSetting.ItemSpacingVertical = 5
        Me.uosEnvironmentSetting.Name = "uosEnvironmentSetting"
        '
        'btnView
        '
        resources.ApplyResources(Me.btnView, "btnView")
        Me.btnView.Name = "btnView"
        '
        'btnClose
        '
        Me.btnClose.AccessibleRole = System.Windows.Forms.AccessibleRole.None
        resources.ApplyResources(Me.btnClose, "btnClose")
        Me.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnClose.Name = "btnClose"
        '
        'DryWetForm
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.BackColor = System.Drawing.Color.White
        resources.ApplyResources(Me, "$this")
        Me.Controls.Add(Me.ugbEnvironmentSetting)
        Me.Controls.Add(Me.btnView)
        Me.Controls.Add(Me.btnClose)
        Me.Name = "DryWetForm"
        CType(Me.ugbEnvironmentSetting, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ugbEnvironmentSetting.ResumeLayout(False)
        CType(Me.uosEnvironmentSetting, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents btnView As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnClose As Infragistics.Win.Misc.UltraButton
    Friend WithEvents ugbEnvironmentSetting As Infragistics.Win.Misc.UltraGroupBox
    Friend WithEvents uosEnvironmentSetting As Infragistics.Win.UltraWinEditors.UltraOptionSet
End Class
