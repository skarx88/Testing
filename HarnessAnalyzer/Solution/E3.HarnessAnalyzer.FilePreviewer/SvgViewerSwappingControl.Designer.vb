Imports System.Drawing
Imports System.Windows.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class SvgViewerSwappingControl
    Inherits System.Windows.Forms.UserControl

    'UserControl überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
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
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
        Me.lblInfoText = New System.Windows.Forms.Label()
        Me.lblMessage = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.ProgressBar1.Location = New System.Drawing.Point(149, 222)
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(331, 23)
        Me.ProgressBar1.Step = 1
        Me.ProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee
        Me.ProgressBar1.TabIndex = 0
        Me.ProgressBar1.Visible = False
        '
        'lblInfoText
        '
        Me.lblInfoText.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.lblInfoText.AutoSize = True
        Me.lblInfoText.BackColor = System.Drawing.Color.Transparent
        Me.lblInfoText.Location = New System.Drawing.Point(294, 286)
        Me.lblInfoText.Name = "lblInfoText"
        Me.lblInfoText.Size = New System.Drawing.Size(54, 13)
        Me.lblInfoText.TabIndex = 1
        Me.lblInfoText.Text = "Loading..."
        Me.lblInfoText.Visible = False
        '
        'lblMessage
        '
        Me.lblMessage.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblMessage.Location = New System.Drawing.Point(77, 357)
        Me.lblMessage.Name = "lblMessage"
        Me.lblMessage.Size = New System.Drawing.Size(477, 124)
        Me.lblMessage.TabIndex = 2
        Me.lblMessage.Text = "lblMessage"
        Me.lblMessage.TextAlign = System.Drawing.ContentAlignment.TopCenter
        Me.lblMessage.Visible = False
        '
        'SvgViewerSwappingControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.Controls.Add(Me.lblMessage)
        Me.Controls.Add(Me.lblInfoText)
        Me.Controls.Add(Me.ProgressBar1)
        Me.Margin = New System.Windows.Forms.Padding(0)
        Me.Name = "SvgViewerSwappingControl"
        Me.Size = New System.Drawing.Size(617, 540)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub


    Friend WithEvents ProgressBar1 As ProgressBar
    Friend WithEvents lblInfoText As Label
    Friend WithEvents lblMessage As Label
End Class
