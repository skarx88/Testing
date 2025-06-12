<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ProgressLoadingXhcvForm
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            _mainStateMachine = Nothing

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
        lblOpeningFile = New Label()
        ProgressBar1 = New ProgressBar()
        grpOpeningFilesFinished = New GroupBox()
        grpOpeningFilesFinished.SuspendLayout()
        SuspendLayout()
        ' 
        ' lblOpeningFile
        ' 
        lblOpeningFile.AutoSize = True
        lblOpeningFile.Location = New Point(12, 9)
        lblOpeningFile.Name = "lblOpeningFile"
        lblOpeningFile.Size = New Size(94, 15)
        lblOpeningFile.TabIndex = 0
        lblOpeningFile.Text = "Opening File: {0}"
        ' 
        ' ProgressBar1
        ' 
        ProgressBar1.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        ProgressBar1.Location = New Point(6, 21)
        ProgressBar1.Name = "ProgressBar1"
        ProgressBar1.Size = New Size(388, 23)
        ProgressBar1.Style = ProgressBarStyle.Marquee
        ProgressBar1.TabIndex = 2
        ' 
        ' grpOpeningFilesFinished
        ' 
        grpOpeningFilesFinished.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        grpOpeningFilesFinished.Controls.Add(ProgressBar1)
        grpOpeningFilesFinished.Location = New Point(12, 42)
        grpOpeningFilesFinished.Name = "grpOpeningFilesFinished"
        grpOpeningFilesFinished.Size = New Size(400, 61)
        grpOpeningFilesFinished.TabIndex = 3
        grpOpeningFilesFinished.TabStop = False
        grpOpeningFilesFinished.Text = "Opening finished: {0}/{1}"
        ' 
        ' ProgressLoadingXhcvForm
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(424, 129)
        ControlBox = False
        Controls.Add(grpOpeningFilesFinished)
        Controls.Add(lblOpeningFile)
        FormBorderStyle = FormBorderStyle.FixedDialog
        Name = "ProgressLoadingXhcvForm"
        ShowInTaskbar = False
        StartPosition = FormStartPosition.CenterParent
        Text = "Opening XHCV..."
        grpOpeningFilesFinished.ResumeLayout(False)
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents lblOpeningFile As Label
    Friend WithEvents ProgressBar1 As ProgressBar
    Friend WithEvents grpOpeningFilesFinished As GroupBox
End Class
