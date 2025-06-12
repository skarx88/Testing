Imports Zuken.E3.Lib.Comparer.Topology.Documents

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class TopologyCompareWizardForm '(Of TDocument As CompareDocument)
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()>
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

    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(TopologyCompareWizardForm))
        btnCancel = New Button()
        btnOk = New Button()
        statusStrip1 = New StatusStrip()
        toolStripStatusLabel1 = New ToolStripStatusLabel()
        toolStripProgressBar = New ToolStripProgressBar()
        TableLayoutPanel1 = New TableLayoutPanel()
        CompareWizard = New TopologyCompareWizardControl()
        Panel1 = New Panel()
        statusStrip1.SuspendLayout()
        TableLayoutPanel1.SuspendLayout()
        Panel1.SuspendLayout()
        SuspendLayout()
        ' 
        ' btnCancel
        ' 
        resources.ApplyResources(btnCancel, "btnCancel")
        btnCancel.DialogResult = DialogResult.Cancel
        btnCancel.Name = "btnCancel"
        btnCancel.UseVisualStyleBackColor = True
        ' 
        ' btnOk
        ' 
        resources.ApplyResources(btnOk, "btnOk")
        btnOk.Name = "btnOk"
        btnOk.UseVisualStyleBackColor = True
        ' 
        ' statusStrip1
        ' 
        statusStrip1.Items.AddRange(New ToolStripItem() {toolStripStatusLabel1, toolStripProgressBar})
        resources.ApplyResources(statusStrip1, "statusStrip1")
        statusStrip1.Name = "statusStrip1"
        ' 
        ' toolStripStatusLabel1
        ' 
        toolStripStatusLabel1.Name = "toolStripStatusLabel1"
        resources.ApplyResources(toolStripStatusLabel1, "toolStripStatusLabel1")
        ' 
        ' toolStripProgressBar
        ' 
        toolStripProgressBar.Name = "toolStripProgressBar"
        resources.ApplyResources(toolStripProgressBar, "toolStripProgressBar")
        ' 
        ' TableLayoutPanel1
        ' 
        resources.ApplyResources(TableLayoutPanel1, "TableLayoutPanel1")
        TableLayoutPanel1.Controls.Add(btnCancel, 0, 0)
        TableLayoutPanel1.Controls.Add(btnOk, 1, 0)
        TableLayoutPanel1.Name = "TableLayoutPanel1"
        ' 
        ' CompareWizard
        ' 
        resources.ApplyResources(CompareWizard, "CompareWizard")
        CompareWizard.FileLeft = Nothing
        CompareWizard.FileRight = Nothing
        CompareWizard.LengthTolerance = 0
        CompareWizard.Name = "CompareWizard"
        CompareWizard.RemoveBridgeSegments = False
        CompareWizard.SelectedCompareLengthClass = [Lib].Model.LengthClass.DMU
        CompareWizard.SelectedRefLengthClass = [Lib].Model.LengthClass.DMU
        CompareWizard.UseSwapDetection = False
        CompareWizard.Dock = DockStyle.Fill
        ' 
        ' Panel1
        ' 
        resources.ApplyResources(Panel1, "Panel1")
        Panel1.Controls.Add(CompareWizard)
        Panel1.Name = "Panel1"
        ' 
        ' TopologyCompareWizardForm
        ' 
        AcceptButton = btnOk
        AutoScaleMode = AutoScaleMode.Inherit
        CancelButton = btnCancel
        resources.ApplyResources(Me, "$this")
        Controls.Add(Panel1)
        Controls.Add(TableLayoutPanel1)
        Controls.Add(statusStrip1)
        Name = "TopologyCompareWizardForm"
        statusStrip1.ResumeLayout(False)
        statusStrip1.PerformLayout()
        TableLayoutPanel1.ResumeLayout(False)
        Panel1.ResumeLayout(False)
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Private WithEvents btnCancel As Button
    Private WithEvents btnOk As Button
    Private WithEvents statusStrip1 As StatusStrip
    Private WithEvents toolStripStatusLabel1 As ToolStripStatusLabel
    Private WithEvents toolStripProgressBar As ToolStripProgressBar
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents CompareWizard As TopologyCompareWizardControl
    Friend WithEvents Panel1 As Panel
End Class
