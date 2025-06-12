<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Public Class TopologyCompareForm
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing Then
                If components IsNot Nothing Then
                    components.Dispose()
                End If
                _storedbundleRadius.Clear()
            End If

            _documentLeft = Nothing
            _documentRight = Nothing
            _segmentItem = Nothing
            _verticesItem = Nothing
            _fixingsItem = Nothing
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(TopologyCompareForm))
        Dim StateEditorButton1 As Infragistics.Win.UltraWinEditors.StateEditorButton = New Infragistics.Win.UltraWinEditors.StateEditorButton()
        txtOverrideRadius = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
        label2 = New Label()
        label1 = New Label()
        cmbAnnotations = New Infragistics.Win.UltraWinEditors.UltraComboEditor()
        btnClose = New Button()
        ultraValidator1 = New Infragistics.Win.Misc.UltraValidator(components)
        contextMenuStrip1 = New ContextMenuStrip(components)
        resetToolStripMenuItem = New ToolStripMenuItem()
        Button1 = New Button()
        Panel1 = New Panel()
        TableLayoutPanel1 = New TableLayoutPanel()
        CType(txtOverrideRadius, ComponentModel.ISupportInitialize).BeginInit()
        CType(cmbAnnotations, ComponentModel.ISupportInitialize).BeginInit()
        CType(ultraValidator1, ComponentModel.ISupportInitialize).BeginInit()
        contextMenuStrip1.SuspendLayout()
        TableLayoutPanel1.SuspendLayout()
        SuspendLayout()
        ' 
        ' txtOverrideRadius
        ' 
        resources.ApplyResources(txtOverrideRadius, "txtOverrideRadius")
        txtOverrideRadius.ButtonsLeft.Add(StateEditorButton1)
        txtOverrideRadius.MaskInput = "{double:4.1}"
        txtOverrideRadius.MaxValue = 1001.0R
        txtOverrideRadius.MinValue = 0R
        txtOverrideRadius.Name = "txtOverrideRadius"
        txtOverrideRadius.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double
        txtOverrideRadius.PromptChar = " "c
        txtOverrideRadius.SpinIncrement = 1.0R
        ultraValidator1.GetValidationSettings(txtOverrideRadius).Condition = New Infragistics.Win.RangeCondition(1.0R, 1000.0R, GetType(Double))
        ultraValidator1.GetValidationSettings(txtOverrideRadius).DataType = GetType(Double)
        ultraValidator1.GetValidationSettings(txtOverrideRadius).IsRequired = True
        ultraValidator1.GetValidationSettings(txtOverrideRadius).NotificationSettings.Action = Infragistics.Win.Misc.NotificationAction.BalloonTip
        ultraValidator1.GetValidationSettings(txtOverrideRadius).NotificationSettings.Text = "Value must be between 1 and 1000"
        ultraValidator1.GetValidationSettings(txtOverrideRadius).RetainFocusOnError = True
        txtOverrideRadius.Value = 2.5R
        ' 
        ' label2
        ' 
        resources.ApplyResources(label2, "label2")
        label2.Name = "label2"
        ' 
        ' label1
        ' 
        resources.ApplyResources(label1, "label1")
        label1.Name = "label1"
        ' 
        ' cmbAnnotations
        ' 
        resources.ApplyResources(cmbAnnotations, "cmbAnnotations")
        cmbAnnotations.CheckedListSettings.CheckBoxStyle = Infragistics.Win.CheckStyle.CheckBox
        cmbAnnotations.CheckedListSettings.EditorValueSource = Infragistics.Win.EditorWithComboValueSource.CheckedItems
        cmbAnnotations.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList
        cmbAnnotations.Name = "cmbAnnotations"
        ' 
        ' btnClose
        ' 
        resources.ApplyResources(btnClose, "btnClose")
        btnClose.DialogResult = DialogResult.OK
        btnClose.Name = "btnClose"
        btnClose.UseVisualStyleBackColor = True
        ' 
        ' contextMenuStrip1
        ' 
        contextMenuStrip1.Items.AddRange(New ToolStripItem() {resetToolStripMenuItem})
        contextMenuStrip1.Name = "contextMenuStrip1"
        resources.ApplyResources(contextMenuStrip1, "contextMenuStrip1")
        ' 
        ' resetToolStripMenuItem
        ' 
        resetToolStripMenuItem.Name = "resetToolStripMenuItem"
        resources.ApplyResources(resetToolStripMenuItem, "resetToolStripMenuItem")
        ' 
        ' Button1
        ' 
        resources.ApplyResources(Button1, "Button1")
        Button1.DialogResult = DialogResult.OK
        Button1.Name = "Button1"
        Button1.UseVisualStyleBackColor = True
        ' 
        ' Panel1
        ' 
        resources.ApplyResources(Panel1, "Panel1")
        Panel1.Name = "Panel1"
        ' 
        ' TableLayoutPanel1
        ' 
        resources.ApplyResources(TableLayoutPanel1, "TableLayoutPanel1")
        TableLayoutPanel1.Controls.Add(txtOverrideRadius, 4, 0)
        TableLayoutPanel1.Controls.Add(label2, 3, 0)
        TableLayoutPanel1.Controls.Add(label1, 0, 0)
        TableLayoutPanel1.Controls.Add(cmbAnnotations, 1, 0)
        TableLayoutPanel1.Name = "TableLayoutPanel1"
        ' 
        ' TopologyCompareForm
        ' 
        resources.ApplyResources(Me, "$this")
        AutoScaleMode = AutoScaleMode.Font
        Controls.Add(TableLayoutPanel1)
        Controls.Add(Panel1)
        Controls.Add(Button1)
        Controls.Add(btnClose)
        Name = "TopologyCompareForm"
        CType(txtOverrideRadius, ComponentModel.ISupportInitialize).EndInit()
        CType(cmbAnnotations, ComponentModel.ISupportInitialize).EndInit()
        CType(ultraValidator1, ComponentModel.ISupportInitialize).EndInit()
        contextMenuStrip1.ResumeLayout(False)
        TableLayoutPanel1.ResumeLayout(False)
        TableLayoutPanel1.PerformLayout()
        ResumeLayout(False)

    End Sub

    Private WithEvents txtOverrideRadius As Infragistics.Win.UltraWinEditors.UltraNumericEditor
    Private WithEvents label2 As Label
    Private WithEvents label1 As Label
    Private WithEvents cmbAnnotations As Infragistics.Win.UltraWinEditors.UltraComboEditor
    Private WithEvents btnClose As Button
    Private WithEvents ultraValidator1 As Infragistics.Win.Misc.UltraValidator
    Private WithEvents contextMenuStrip1 As ContextMenuStrip
    Private WithEvents resetToolStripMenuItem As ToolStripMenuItem
    Private WithEvents Button1 As Button
    Friend WithEvents Panel1 As Panel
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
End Class
