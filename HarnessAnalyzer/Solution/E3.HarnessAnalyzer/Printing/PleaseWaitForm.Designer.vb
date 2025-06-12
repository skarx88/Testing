Namespace Printing

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class PleaseWaitForm
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
        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(PleaseWaitForm))
            Me.Label1 = New System.Windows.Forms.Label()
            Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
            Me.SuspendLayout()
            '
            'Label1
            '
            resources.ApplyResources(Me.Label1, "Label1")
            Me.Label1.Name = "Label1"
            '
            'ProgressBar1
            '
            resources.ApplyResources(Me.ProgressBar1, "ProgressBar1")
            Me.ProgressBar1.MarqueeAnimationSpeed = 50
            Me.ProgressBar1.Name = "ProgressBar1"
            Me.ProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee
            '
            'PleaseWaitForm
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
            Me.ControlBox = False
            Me.Controls.Add(Me.ProgressBar1)
            Me.Controls.Add(Me.Label1)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
            Me.Name = "PleaseWaitForm"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents ProgressBar1 As ProgressBar
        Protected Friend WithEvents Label1 As Label
    End Class

End Namespace