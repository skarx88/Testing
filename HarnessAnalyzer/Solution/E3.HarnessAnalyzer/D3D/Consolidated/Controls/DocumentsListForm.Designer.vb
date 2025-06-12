Namespace D3D.Consolidated.Controls
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class DocumentsListForm
        Inherits System.Windows.Forms.Form

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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DocumentsListForm))
            Me.CheckedListBox1 = New System.Windows.Forms.CheckedListBox()
            Me.SuspendLayout()
            '
            'CheckedListBox1
            '
            Me.CheckedListBox1.CheckOnClick = True
            resources.ApplyResources(Me.CheckedListBox1, "CheckedListBox1")
            Me.CheckedListBox1.FormattingEnabled = True
            Me.CheckedListBox1.Name = "CheckedListBox1"
            '
            'HarnessListForm
            '
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
            resources.ApplyResources(Me, "$this")
            Me.Controls.Add(Me.CheckedListBox1)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
            Me.Name = "HarnessListForm"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.ResumeLayout(False)

        End Sub

        Friend WithEvents CheckedListBox1 As CheckedListBox
    End Class
End Namespace