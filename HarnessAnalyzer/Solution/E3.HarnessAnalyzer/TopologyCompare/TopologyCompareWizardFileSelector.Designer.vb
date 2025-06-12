<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TopologyCompareWizardFileSelector
    Inherits App.Windows.Controls.Comparer.Topology.FileSelectorControl

    'UserControl überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            _hcvLeft = Nothing
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(TopologyCompareWizardFileSelector))
        cmbRightDocument = New Infragistics.Win.UltraWinEditors.UltraComboEditor()
        txtLeftFilePath = New TextBox()
        cmbLengthClassRef = New Infragistics.Win.UltraWinEditors.UltraComboEditor()
        cmbLengthClassComp = New Infragistics.Win.UltraWinEditors.UltraComboEditor()
        Label2 = New Label()
        Label3 = New Label()
        CType(ultraTextEditor2, ComponentModel.ISupportInitialize).BeginInit()
        CType(ultraTextEditor1, ComponentModel.ISupportInitialize).BeginInit()
        CType(cmbRightDocument, ComponentModel.ISupportInitialize).BeginInit()
        CType(cmbLengthClassRef, ComponentModel.ISupportInitialize).BeginInit()
        CType(cmbLengthClassComp, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' ultraLabel2
        ' 
        resources.ApplyResources(ultraLabel2, "ultraLabel2")
        ' 
        ' ultraTextEditor2
        ' 
        resources.ApplyResources(ultraTextEditor2, "ultraTextEditor2")
        ' 
        ' ultraLabel1
        ' 
        resources.ApplyResources(ultraLabel1, "ultraLabel1")
        ' 
        ' ultraTextEditor1
        ' 
        resources.ApplyResources(ultraTextEditor1, "ultraTextEditor1")
        ' 
        ' Label1
        ' 
        resources.ApplyResources(Label1, "Label1")
        ' 
        ' cmbRightDocument
        ' 
        resources.ApplyResources(cmbRightDocument, "cmbRightDocument")
        cmbRightDocument.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList
        cmbRightDocument.Name = "cmbRightDocument"
        ' 
        ' txtLeftFilePath
        ' 
        resources.ApplyResources(txtLeftFilePath, "txtLeftFilePath")
        txtLeftFilePath.Name = "txtLeftFilePath"
        txtLeftFilePath.ReadOnly = True
        ' 
        ' cmbLengthClassRef
        ' 
        resources.ApplyResources(cmbLengthClassRef, "cmbLengthClassRef")
        cmbLengthClassRef.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList
        cmbLengthClassRef.Name = "cmbLengthClassRef"
        ' 
        ' cmbLengthClassComp
        ' 
        resources.ApplyResources(cmbLengthClassComp, "cmbLengthClassComp")
        cmbLengthClassComp.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList
        cmbLengthClassComp.Name = "cmbLengthClassComp"
        ' 
        ' Label2
        ' 
        resources.ApplyResources(Label2, "Label2")
        Label2.Name = "Label2"
        ' 
        ' Label3
        ' 
        resources.ApplyResources(Label3, "Label3")
        Label3.Name = "Label3"
        ' 
        ' TopologyCompareWizardFileSelector
        ' 
        resources.ApplyResources(Me, "$this")
        AutoScaleMode = AutoScaleMode.Font
        Controls.Add(Label3)
        Controls.Add(Label2)
        Controls.Add(cmbLengthClassComp)
        Controls.Add(cmbLengthClassRef)
        Controls.Add(txtLeftFilePath)
        Controls.Add(cmbRightDocument)
        Name = "TopologyCompareWizardFileSelector"
        Controls.SetChildIndex(cmbRightDocument, 0)
        Controls.SetChildIndex(txtLeftFilePath, 0)
        Controls.SetChildIndex(ultraTextEditor1, 0)
        Controls.SetChildIndex(ultraLabel1, 0)
        Controls.SetChildIndex(ultraTextEditor2, 0)
        Controls.SetChildIndex(ultraLabel2, 0)
        Controls.SetChildIndex(Label1, 0)
        Controls.SetChildIndex(cmbLengthClassRef, 0)
        Controls.SetChildIndex(cmbLengthClassComp, 0)
        Controls.SetChildIndex(Label2, 0)
        Controls.SetChildIndex(Label3, 0)
        CType(ultraTextEditor2, ComponentModel.ISupportInitialize).EndInit()
        CType(ultraTextEditor1, ComponentModel.ISupportInitialize).EndInit()
        CType(cmbRightDocument, ComponentModel.ISupportInitialize).EndInit()
        CType(cmbLengthClassRef, ComponentModel.ISupportInitialize).EndInit()
        CType(cmbLengthClassComp, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()

    End Sub

    Friend WithEvents cmbRightDocument As Infragistics.Win.UltraWinEditors.UltraComboEditor
    Friend WithEvents txtLeftFilePath As TextBox
    Friend WithEvents cmbLengthClassRef As Infragistics.Win.UltraWinEditors.UltraComboEditor
    Friend WithEvents cmbLengthClassComp As Infragistics.Win.UltraWinEditors.UltraComboEditor
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
End Class
