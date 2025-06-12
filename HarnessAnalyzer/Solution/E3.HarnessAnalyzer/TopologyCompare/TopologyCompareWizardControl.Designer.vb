<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class TopologyCompareWizardControl
    Inherits App.Windows.Controls.Comparer.Topology.CompareWizardControl

    'UserControl überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing Then
                If components IsNot Nothing Then
                    components.Dispose()
                End If
                If _fileSelectorControl IsNot Nothing Then
                    RemoveHandler CType(_fileSelectorControl, TopologyCompareWizardFileSelector).SelectedDocumentChanged, AddressOf _fileSelectorControl_SelectedDocumentChanged
                End If
            End If
            _fileSelectorControl = Nothing
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(TopologyCompareWizardControl))
        CType(UltraTabControl1, ComponentModel.ISupportInitialize).BeginInit()
        UltraTabControl1.SuspendLayout()
        sharedPageTitlePanel.SuspendLayout()
        VerticesPairsPanel.SuspendLayout()
        CType(numLengthTolerance, ComponentModel.ISupportInitialize).BeginInit()
        UltraTabPageControl1.SuspendLayout()
        SuspendLayout()
        ' 
        ' UltraTabControl1
        ' 
        resources.ApplyResources(UltraTabControl1, "UltraTabControl1")
        UltraTabControl1.TabPageMargins.ForceSerialization = True
        ' 
        ' sharedPageTitlePanel
        ' 
        resources.ApplyResources(sharedPageTitlePanel, "sharedPageTitlePanel")
        ' 
        ' VerticesPairsPanel
        ' 
        resources.ApplyResources(VerticesPairsPanel, "VerticesPairsPanel")
        ' 
        ' lblPageNumber
        ' 
        resources.ApplyResources(lblPageNumber, "lblPageNumber")
        ' 
        ' btnNext
        ' 
        resources.ApplyResources(btnNext, "btnNext")
        ' 
        ' btnBack
        ' 
        resources.ApplyResources(btnBack, "btnBack")
        ' 
        ' chkRemoveBridgeSegments
        ' 
        resources.ApplyResources(chkRemoveBridgeSegments, "chkRemoveBridgeSegments")
        ' 
        ' chkSwapDetection
        ' 
        resources.ApplyResources(chkSwapDetection, "chkSwapDetection")
        ' 
        ' chkRemoveSingularVertices
        ' 
        resources.ApplyResources(chkRemoveSingularVertices, "chkRemoveSingularVertices")
        ' 
        ' Label2
        ' 
        resources.ApplyResources(Label2, "Label2")
        ' 
        ' numLengthTolerance
        ' 
        resources.ApplyResources(numLengthTolerance, "numLengthTolerance")
        ' 
        ' UltraTabPageControl1
        ' 
        resources.ApplyResources(UltraTabPageControl1, "UltraTabPageControl1")
        ' 
        ' _fileSelectorControl
        ' 
        resources.ApplyResources(_fileSelectorControl, "_fileSelectorControl")
        ' 
        ' TopologyCompareWizardControl
        ' 
        resources.ApplyResources(Me, "$this")
        AutoScaleMode = AutoScaleMode.Font
        Name = "TopologyCompareWizardControl"
        CType(UltraTabControl1, ComponentModel.ISupportInitialize).EndInit()
        UltraTabControl1.ResumeLayout(False)
        sharedPageTitlePanel.ResumeLayout(False)
        VerticesPairsPanel.ResumeLayout(False)
        CType(numLengthTolerance, ComponentModel.ISupportInitialize).EndInit()
        UltraTabPageControl1.ResumeLayout(False)
        ResumeLayout(False)
        PerformLayout()

    End Sub

End Class
