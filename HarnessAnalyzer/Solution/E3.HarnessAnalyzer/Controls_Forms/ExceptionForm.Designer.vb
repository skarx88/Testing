<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ExceptionForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ExceptionForm))
        Me.tlpButtons = New System.Windows.Forms.TableLayoutPanel()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.OK_Button = New System.Windows.Forms.Button()
        Me.lblError = New System.Windows.Forms.Label()
        Me.lblDebuInfo = New System.Windows.Forms.Label()
        Me.txtDebugInfo = New System.Windows.Forms.TextBox()
        Me.picErrorLogo = New System.Windows.Forms.PictureBox()
        Me.sfdException = New System.Windows.Forms.SaveFileDialog()
        Me.tlpButtons.SuspendLayout()
        CType(Me.picErrorLogo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'tlpButtons
        '
        resources.ApplyResources(Me.tlpButtons, "tlpButtons")
        Me.tlpButtons.Controls.Add(Me.btnSave, 0, 0)
        Me.tlpButtons.Controls.Add(Me.OK_Button, 1, 0)
        Me.tlpButtons.Name = "tlpButtons"
        '
        'btnSave
        '
        resources.ApplyResources(Me.btnSave, "btnSave")
        Me.btnSave.Name = "btnSave"
        '
        'OK_Button
        '
        resources.ApplyResources(Me.OK_Button, "OK_Button")
        Me.OK_Button.Name = "OK_Button"
        '
        'lblError
        '
        resources.ApplyResources(Me.lblError, "lblError")
        Me.lblError.Name = "lblError"
        '
        'lblDebuInfo
        '
        resources.ApplyResources(Me.lblDebuInfo, "lblDebuInfo")
        Me.lblDebuInfo.Name = "lblDebuInfo"
        '
        'txtDebugInfo
        '
        resources.ApplyResources(Me.txtDebugInfo, "txtDebugInfo")
        Me.txtDebugInfo.Name = "txtDebugInfo"
        Me.txtDebugInfo.ReadOnly = True
        '
        'picErrorLogo
        '
        resources.ApplyResources(Me.picErrorLogo, "picErrorLogo")
        Me.picErrorLogo.Name = "picErrorLogo"
        Me.picErrorLogo.TabStop = False
        '
        'sfdException
        '
        Me.sfdException.DefaultExt = IO.KnownFile.TXT.Trim("."c)
        Me.sfdException.FileName = "UnhandledException.txt"
        resources.ApplyResources(Me.sfdException, "sfdException")
        '
        'ExceptionForm
        '
        Me.AcceptButton = Me.OK_Button
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.ControlBox = False
        Me.Controls.Add(Me.picErrorLogo)
        Me.Controls.Add(Me.txtDebugInfo)
        Me.Controls.Add(Me.lblDebuInfo)
        Me.Controls.Add(Me.lblError)
        Me.Controls.Add(Me.tlpButtons)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ExceptionForm"
        Me.ShowInTaskbar = False
        Me.tlpButtons.ResumeLayout(False)
        CType(Me.picErrorLogo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents tlpButtons As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents OK_Button As System.Windows.Forms.Button
    Friend WithEvents lblError As System.Windows.Forms.Label
    Friend WithEvents lblDebuInfo As System.Windows.Forms.Label
    Friend WithEvents txtDebugInfo As System.Windows.Forms.TextBox
    Friend WithEvents picErrorLogo As System.Windows.Forms.PictureBox
    Friend WithEvents sfdException As System.Windows.Forms.SaveFileDialog
    Friend WithEvents btnSave As System.Windows.Forms.Button

End Class
