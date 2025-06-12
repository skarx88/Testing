<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class D3DSaveChangesMsgBox
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(D3DSaveChangesMsgBox))
        Dim UltraToolTipInfo4 As Infragistics.Win.UltraWinToolTip.UltraToolTipInfo = New Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Save changes and leave", Infragistics.Win.ToolTipImage.[Default], Nothing, Infragistics.Win.DefaultableBoolean.[Default])
        Dim UltraToolTipInfo3 As Infragistics.Win.UltraWinToolTip.UltraToolTipInfo = New Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Dismiss changes and leave", Infragistics.Win.ToolTipImage.[Default], Nothing, Infragistics.Win.DefaultableBoolean.[Default])
        Dim UltraToolTipInfo1 As Infragistics.Win.UltraWinToolTip.UltraToolTipInfo = New Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Accept changes without saving", Infragistics.Win.ToolTipImage.[Default], Nothing, Infragistics.Win.DefaultableBoolean.[Default])
        Dim UltraToolTipInfo2 As Infragistics.Win.UltraWinToolTip.UltraToolTipInfo = New Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Do nothing", Infragistics.Win.ToolTipImage.[Default], Nothing, Infragistics.Win.DefaultableBoolean.[Default])
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnYes = New System.Windows.Forms.Button()
        Me.btnNo = New System.Windows.Forms.Button()
        Me.btnIgnore = New System.Windows.Forms.Button()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.UltraToolTipManager1 = New Infragistics.Win.UltraWinToolTip.UltraToolTipManager(Me.components)
        Me.Label2 = New System.Windows.Forms.Label()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
        '
        'btnYes
        '
        resources.ApplyResources(Me.btnYes, "btnYes")
        Me.btnYes.DialogResult = System.Windows.Forms.DialogResult.Yes
        Me.btnYes.Name = "btnYes"
        resources.ApplyResources(UltraToolTipInfo4, "UltraToolTipInfo4")
        Me.UltraToolTipManager1.SetUltraToolTip(Me.btnYes, UltraToolTipInfo4)
        Me.btnYes.UseVisualStyleBackColor = True
        '
        'btnNo
        '
        resources.ApplyResources(Me.btnNo, "btnNo")
        Me.btnNo.DialogResult = System.Windows.Forms.DialogResult.No
        Me.btnNo.Name = "btnNo"
        resources.ApplyResources(UltraToolTipInfo3, "UltraToolTipInfo3")
        Me.UltraToolTipManager1.SetUltraToolTip(Me.btnNo, UltraToolTipInfo3)
        Me.btnNo.UseVisualStyleBackColor = True
        '
        'btnIgnore
        '
        resources.ApplyResources(Me.btnIgnore, "btnIgnore")
        Me.btnIgnore.DialogResult = System.Windows.Forms.DialogResult.Ignore
        Me.btnIgnore.Name = "btnIgnore"
        resources.ApplyResources(UltraToolTipInfo1, "UltraToolTipInfo1")
        Me.UltraToolTipManager1.SetUltraToolTip(Me.btnIgnore, UltraToolTipInfo1)
        Me.btnIgnore.UseVisualStyleBackColor = True
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = Global.Zuken.E3.HarnessAnalyzer.My.Resources.Resources.Question
        resources.ApplyResources(Me.PictureBox1, "PictureBox1")
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.TabStop = False
        '
        'btnCancel
        '
        resources.ApplyResources(Me.btnCancel, "btnCancel")
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Name = "btnCancel"
        resources.ApplyResources(UltraToolTipInfo2, "UltraToolTipInfo2")
        Me.UltraToolTipManager1.SetUltraToolTip(Me.btnCancel, UltraToolTipInfo2)
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'UltraToolTipManager1
        '
        Me.UltraToolTipManager1.ContainingControl = Me
        '
        'Label2
        '
        resources.ApplyResources(Me.Label2, "Label2")
        Me.Label2.Name = "Label2"
        '
        'D3DSaveChangesMsgBox
        '
        Me.AcceptButton = Me.btnIgnore
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnCancel
        Me.ControlBox = False
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.btnIgnore)
        Me.Controls.Add(Me.btnNo)
        Me.Controls.Add(Me.btnYes)
        Me.Controls.Add(Me.Label1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "D3DSaveChangesMsgBox"
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btnYes As System.Windows.Forms.Button
    Friend WithEvents btnNo As System.Windows.Forms.Button
    Friend WithEvents btnIgnore As System.Windows.Forms.Button
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents UltraToolTipManager1 As Infragistics.Win.UltraWinToolTip.UltraToolTipManager
    Friend WithEvents Label2 As System.Windows.Forms.Label
End Class
