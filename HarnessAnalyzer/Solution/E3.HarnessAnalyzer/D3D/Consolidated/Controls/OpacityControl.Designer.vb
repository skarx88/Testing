Namespace D3D.Consolidated.Controls

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class OpacityControl
        Inherits System.Windows.Forms.UserControl

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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(OpacityControl))
            Me.TrackBar1 = New System.Windows.Forms.TrackBar()
            Me.Label1 = New System.Windows.Forms.Label()
            Me.Label2 = New System.Windows.Forms.Label()
            Me.lbl = New System.Windows.Forms.Label()
            CType(Me.TrackBar1, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'TrackBar1
            '
            resources.ApplyResources(Me.TrackBar1, "TrackBar1")
            Me.TrackBar1.Maximum = 100
            Me.TrackBar1.Name = "TrackBar1"
            '
            'Label1
            '
            resources.ApplyResources(Me.Label1, "Label1")
            Me.Label1.Name = "Label1"
            '
            'Label2
            '
            resources.ApplyResources(Me.Label2, "Label2")
            Me.Label2.Name = "Label2"
            '
            'lbl
            '
            resources.ApplyResources(Me.lbl, "lbl")
            Me.lbl.Name = "lbl"
            '
            'CtrlOpacity
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
            Me.Controls.Add(Me.lbl)
            Me.Controls.Add(Me.Label2)
            Me.Controls.Add(Me.Label1)
            Me.Controls.Add(Me.TrackBar1)
            Me.Name = "CtrlOpacity"
            CType(Me.TrackBar1, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Friend WithEvents TrackBar1 As TrackBar
        Friend WithEvents Label1 As Label
        Friend WithEvents Label2 As Label
        Friend WithEvents lbl As Label
    End Class

End Namespace