Namespace D3D.Document.Controls

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class Document3DControl
        Inherits System.Windows.Forms.UserControl

        'UserControl überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing Then
                    ClearSpliceHighlightAndRubberLines()
                    ClearClockSymbolsLabels()

                    If ToolTipCaptionContextMenuStrip IsNot Nothing Then
                        ToolTipCaptionContextMenuStrip.Dispose()
                    End If

                    If ToolTipCaptionContextMenuStrip IsNot Nothing Then
                        ToolTipCaptionContextMenuStrip.Dispose()
                    End If

                    If Model3DControl1 IsNot Nothing Then
                        RemoveHandler Model3DControl1.HideIndicator, AddressOf HideIndicator
                        RemoveHandler Model3DControl1.ShowIndicator, AddressOf ShowIndicator
                        RemoveHandler Model3DControl1.ShowAllIndicators, AddressOf ShowAllIndicators
                        RemoveHandler Model3DControl1.SelectRedlining, AddressOf SelectRedlining
                        Model3DControl1.Dispose()
                    End If

                    If _ttManager IsNot Nothing Then
                        _ttManager.Dispose()
                    End If

                End If

                _busyState = Nothing
                _ttManager = Nothing
                _clockSymbolLabels = Nothing
                _proposalLabels = Nothing
                _dimensionLabels = Nothing
                _redlinings = Nothing
                _rubberLines = Nothing
                _document = Nothing
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        'Wird vom Windows Form-Designer benötigt.
        'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
        'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
        'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Document3DControl))
            Me.Model3DControl1 = New DocumentDesignControl
            Me.ToolTipCaptionContextMenuStrip = New System.Windows.Forms.ContextMenuStrip()
            Me.CopyCaptionToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.ToolTipCaptionContextMenuStrip.SuspendLayout()
            Me.SuspendLayout()
            '
            'Model3DControl1
            '
            resources.ApplyResources(Me.Model3DControl1, "Model3DControl1")
            Me.Model3DControl1.DrawBorderColor = Nothing
            Me.Model3DControl1.Name = "Model3DControl1"
            'Me.Model3D.Snapping = devDept.Eyeshot.Snapping.None
            'ContextMenuStripCaption
            '
            resources.ApplyResources(Me.ToolTipCaptionContextMenuStrip, "ContextMenuStripCaption")
            Me.ToolTipCaptionContextMenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CopyCaptionToolStripMenuItem})
            Me.ToolTipCaptionContextMenuStrip.Name = "ContextMenuStripCaption"
            '
            'CopyCaptionToolStripMenuItem
            '
            resources.ApplyResources(Me.CopyCaptionToolStripMenuItem, "CopyCaptionToolStripMenuItem")
            Me.CopyCaptionToolStripMenuItem.Image = Global.Zuken.E3.HarnessAnalyzer.My.Resources.Resources.copy
            Me.CopyCaptionToolStripMenuItem.Name = "CopyCaptionToolStripMenuItem"
            '
            'Document3DViewer
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.Model3DControl1)
            Me.Name = "Document3DViewer"
            Me.ToolTipCaptionContextMenuStrip.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub

        Friend WithEvents Model3DControl1 As DocumentDesignControl
        Friend WithEvents ToolTipCaptionContextMenuStrip As ContextMenuStrip
        Friend WithEvents CopyCaptionToolStripMenuItem As ToolStripMenuItem
    End Class

End Namespace