<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class CancellationPendingForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(CancellationPendingForm))
        Dim Appearance1 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Me.progressBarMarquee = New System.Windows.Forms.ProgressBar()
        Me.ultraLabel1 = New Infragistics.Win.Misc.UltraLabel()
        Me.SuspendLayout()
        '
        'progressBarMarquee
        '
        resources.ApplyResources(Me.progressBarMarquee, "progressBarMarquee")
        Me.progressBarMarquee.Name = "progressBarMarquee"
        Me.progressBarMarquee.Style = System.Windows.Forms.ProgressBarStyle.Marquee
        '
        'ultraLabel1
        '
        resources.ApplyResources(Me.ultraLabel1, "ultraLabel1")
        resources.ApplyResources(Appearance1, "Appearance1")
        Me.ultraLabel1.Appearance = Appearance1
        Me.ultraLabel1.Name = "ultraLabel1"
        '
        'CancellationPendingForm
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.progressBarMarquee)
        Me.Controls.Add(Me.ultraLabel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "CancellationPendingForm"
        Me.ResumeLayout(False)

    End Sub

    Private WithEvents progressBarMarquee As ProgressBar
    Private WithEvents ultraLabel1 As Infragistics.Win.Misc.UltraLabel
End Class
