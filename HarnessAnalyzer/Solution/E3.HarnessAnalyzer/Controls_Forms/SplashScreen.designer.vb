<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> Partial Class SplashScreen
#Region "Vom Windows Form-Designer generierter Code "
    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()> Protected Overloads Overrides Sub Dispose(ByVal Disposing As Boolean)
        If Disposing Then
            If Not components Is Nothing Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(Disposing)
    End Sub
    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer
    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Verändern mit dem Windows Form-Designer ist nicht möglich.
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SplashScreen))
        Dim Appearance1 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.lblMessage = New System.Windows.Forms.Label()
        Me.lineSep = New System.Windows.Forms.Label()
        Me.lblDisclaimer = New System.Windows.Forms.Label()
        Me.lblDescription = New System.Windows.Forms.Label()
        Me.lblBuild = New System.Windows.Forms.Label()
        Me.panBackground = New System.Windows.Forms.Panel()
        Me.llblZuken = New System.Windows.Forms.LinkLabel()
        Me.lblBorrowLic = New Infragistics.Win.Misc.UltraLabel()
        Me.picLogo = New System.Windows.Forms.PictureBox()
        Me.panBackground.SuspendLayout()
        CType(Me.picLogo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnClose
        '
        resources.ApplyResources(Me.btnClose, "btnClose")
        Me.btnClose.BackColor = System.Drawing.SystemColors.Control
        Me.btnClose.Cursor = System.Windows.Forms.Cursors.Default
        Me.btnClose.ForeColor = System.Drawing.SystemColors.ControlText
        Me.btnClose.Name = "btnClose"
        Me.btnClose.UseVisualStyleBackColor = False
        '
        'lblMessage
        '
        resources.ApplyResources(Me.lblMessage, "lblMessage")
        Me.lblMessage.BackColor = System.Drawing.Color.Transparent
        Me.lblMessage.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblMessage.ForeColor = System.Drawing.SystemColors.ControlText
        Me.lblMessage.Name = "lblMessage"
        '
        'lineSep
        '
        resources.ApplyResources(Me.lineSep, "lineSep")
        Me.lineSep.BackColor = System.Drawing.SystemColors.WindowText
        Me.lineSep.Name = "lineSep"
        '
        'lblDisclaimer
        '
        resources.ApplyResources(Me.lblDisclaimer, "lblDisclaimer")
        Me.lblDisclaimer.BackColor = System.Drawing.Color.Transparent
        Me.lblDisclaimer.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblDisclaimer.ForeColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.lblDisclaimer.Name = "lblDisclaimer"
        '
        'lblDescription
        '
        resources.ApplyResources(Me.lblDescription, "lblDescription")
        Me.lblDescription.BackColor = System.Drawing.Color.Transparent
        Me.lblDescription.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblDescription.ForeColor = System.Drawing.Color.Black
        Me.lblDescription.Name = "lblDescription"
        '
        'lblBuild
        '
        resources.ApplyResources(Me.lblBuild, "lblBuild")
        Me.lblBuild.BackColor = System.Drawing.Color.Transparent
        Me.lblBuild.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblBuild.ForeColor = System.Drawing.SystemColors.ControlText
        Me.lblBuild.Name = "lblBuild"
        '
        'panBackground
        '
        Me.panBackground.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.panBackground.Controls.Add(Me.llblZuken)
        Me.panBackground.Controls.Add(Me.btnClose)
        Me.panBackground.Controls.Add(Me.lblBorrowLic)
        Me.panBackground.Controls.Add(Me.picLogo)
        resources.ApplyResources(Me.panBackground, "panBackground")
        Me.panBackground.Name = "panBackground"
        '
        'llblZuken
        '
        resources.ApplyResources(Me.llblZuken, "llblZuken")
        Me.llblZuken.Name = "llblZuken"
        Me.llblZuken.TabStop = True
        '
        'lblBorrowLic
        '
        resources.ApplyResources(Me.lblBorrowLic, "lblBorrowLic")
        resources.ApplyResources(Appearance1, "Appearance1")
        Me.lblBorrowLic.Appearance = Appearance1
        Me.lblBorrowLic.Name = "lblBorrowLic"
        '
        'picLogo
        '
        resources.ApplyResources(Me.picLogo, "picLogo")
        Me.picLogo.Image = Nothing
        Me.picLogo.Name = "picLogo"
        Me.picLogo.TabStop = False
        '
        'SplashScreen
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.BackColor = System.Drawing.Color.White
        resources.ApplyResources(Me, "$this")
        Me.Controls.Add(Me.lblMessage)
        Me.Controls.Add(Me.lineSep)
        Me.Controls.Add(Me.lblDisclaimer)
        Me.Controls.Add(Me.lblDescription)
        Me.Controls.Add(Me.lblBuild)
        Me.Controls.Add(Me.panBackground)
        Me.Cursor = System.Windows.Forms.Cursors.Default
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "SplashScreen"
        Me.ShowInTaskbar = False
        Me.panBackground.ResumeLayout(False)
        Me.panBackground.PerformLayout()
        CType(Me.picLogo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Public WithEvents btnClose As System.Windows.Forms.Button
    Public WithEvents lblMessage As System.Windows.Forms.Label
    Public WithEvents lineSep As System.Windows.Forms.Label
    Public WithEvents lblDisclaimer As System.Windows.Forms.Label
    Public WithEvents lblDescription As System.Windows.Forms.Label
    Public WithEvents lblBuild As System.Windows.Forms.Label
    Friend WithEvents panBackground As System.Windows.Forms.Panel
    Friend WithEvents picLogo As System.Windows.Forms.PictureBox
    Friend WithEvents lblBorrowLic As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents llblZuken As System.Windows.Forms.LinkLabel
#End Region
End Class