<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PdfSaveForm
    Inherits System.Windows.Forms.Form

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(PdfSaveForm))
        Dim Appearance1 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance2 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance3 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim EditorButton1 As Infragistics.Win.UltraWinEditors.EditorButton = New Infragistics.Win.UltraWinEditors.EditorButton()
        Dim Appearance4 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim EditorButton2 As Infragistics.Win.UltraWinEditors.EditorButton = New Infragistics.Win.UltraWinEditors.EditorButton()
        Dim UltraTab1 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Dim UltraTab2 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Me.DrawingTabControl = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        Me.chkScaleToMaximum = New Infragistics.Win.UltraWinEditors.UltraCheckEditor()
        Me.btnDrawingCancel = New Infragistics.Win.Misc.UltraButton()
        Me.btnDrawingOK = New Infragistics.Win.Misc.UltraButton()
        Me.UltraLabel2 = New Infragistics.Win.Misc.UltraLabel()
        Me.uckEmbeddTTF = New Infragistics.Win.UltraWinEditors.UltraCheckEditor()
        Me.txtDrawingSavePath = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.SchematicsTabControl = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        Me.btnSchematicsOK = New Infragistics.Win.Misc.UltraButton()
        Me.btnSchematicsCancel = New Infragistics.Win.Misc.UltraButton()
        Me.UltraLabel1 = New Infragistics.Win.Misc.UltraLabel()
        Me.txtSchematicsSavePath = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.UltraTabControl1 = New Infragistics.Win.UltraWinTabControl.UltraTabControl()
        Me.UltraTabSharedControlsPage1 = New Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage()
        Me.DrawingTabControl.SuspendLayout()
        CType(Me.chkScaleToMaximum, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.uckEmbeddTTF, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtDrawingSavePath, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SchematicsTabControl.SuspendLayout()
        CType(Me.txtSchematicsSavePath, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.UltraTabControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.UltraTabControl1.SuspendLayout()
        Me.SuspendLayout()
        '
        'DrawingTabControl
        '
        Me.DrawingTabControl.Controls.Add(Me.chkScaleToMaximum)
        Me.DrawingTabControl.Controls.Add(Me.btnDrawingCancel)
        Me.DrawingTabControl.Controls.Add(Me.btnDrawingOK)
        Me.DrawingTabControl.Controls.Add(Me.UltraLabel2)
        Me.DrawingTabControl.Controls.Add(Me.uckEmbeddTTF)
        Me.DrawingTabControl.Controls.Add(Me.txtDrawingSavePath)
        resources.ApplyResources(Me.DrawingTabControl, "DrawingTabControl")
        Me.DrawingTabControl.Name = "DrawingTabControl"
        '
        'chkScaleToMaximum
        '
        resources.ApplyResources(Me.chkScaleToMaximum, "chkScaleToMaximum")
        Appearance1.BackColor = System.Drawing.Color.White
        Me.chkScaleToMaximum.Appearance = Appearance1
        Me.chkScaleToMaximum.BackColor = System.Drawing.Color.White
        Me.chkScaleToMaximum.BackColorInternal = System.Drawing.Color.White
        Me.chkScaleToMaximum.Name = "chkScaleToMaximum"
        '
        'btnDrawingCancel
        '
        resources.ApplyResources(Me.btnDrawingCancel, "btnDrawingCancel")
        Me.btnDrawingCancel.Name = "btnDrawingCancel"
        '
        'btnDrawingOK
        '
        resources.ApplyResources(Me.btnDrawingOK, "btnDrawingOK")
        Me.btnDrawingOK.Name = "btnDrawingOK"
        '
        'UltraLabel2
        '
        resources.ApplyResources(Me.UltraLabel2, "UltraLabel2")
        Appearance2.BackColor = System.Drawing.Color.Transparent
        Me.UltraLabel2.Appearance = Appearance2
        Me.UltraLabel2.Name = "UltraLabel2"
        '
        'uckEmbeddTTF
        '
        resources.ApplyResources(Me.uckEmbeddTTF, "uckEmbeddTTF")
        Appearance3.BackColor = System.Drawing.Color.White
        Me.uckEmbeddTTF.Appearance = Appearance3
        Me.uckEmbeddTTF.BackColor = System.Drawing.Color.White
        Me.uckEmbeddTTF.BackColorInternal = System.Drawing.Color.White
        Me.uckEmbeddTTF.Name = "uckEmbeddTTF"
        '
        'txtDrawingSavePath
        '
        resources.ApplyResources(Me.txtDrawingSavePath, "txtDrawingSavePath")
        resources.ApplyResources(EditorButton1, "EditorButton1")
        Me.txtDrawingSavePath.ButtonsRight.Add(EditorButton1)
        Me.txtDrawingSavePath.Name = "txtDrawingSavePath"
        '
        'SchematicsTabControl
        '
        Me.SchematicsTabControl.Controls.Add(Me.btnSchematicsOK)
        Me.SchematicsTabControl.Controls.Add(Me.btnSchematicsCancel)
        Me.SchematicsTabControl.Controls.Add(Me.UltraLabel1)
        Me.SchematicsTabControl.Controls.Add(Me.txtSchematicsSavePath)
        resources.ApplyResources(Me.SchematicsTabControl, "SchematicsTabControl")
        Me.SchematicsTabControl.Name = "SchematicsTabControl"
        '
        'btnSchematicsOK
        '
        resources.ApplyResources(Me.btnSchematicsOK, "btnSchematicsOK")
        Me.btnSchematicsOK.Name = "btnSchematicsOK"
        '
        'btnSchematicsCancel
        '
        resources.ApplyResources(Me.btnSchematicsCancel, "btnSchematicsCancel")
        Me.btnSchematicsCancel.Name = "btnSchematicsCancel"
        '
        'UltraLabel1
        '
        resources.ApplyResources(Me.UltraLabel1, "UltraLabel1")
        Appearance4.BackColor = System.Drawing.Color.Transparent
        Me.UltraLabel1.Appearance = Appearance4
        Me.UltraLabel1.Name = "UltraLabel1"
        '
        'txtSchematicsSavePath
        '
        resources.ApplyResources(Me.txtSchematicsSavePath, "txtSchematicsSavePath")
        resources.ApplyResources(EditorButton2, "EditorButton2")
        Me.txtSchematicsSavePath.ButtonsRight.Add(EditorButton2)
        Me.txtSchematicsSavePath.Name = "txtSchematicsSavePath"
        '
        'UltraTabControl1
        '
        resources.ApplyResources(Me.UltraTabControl1, "UltraTabControl1")
        Me.UltraTabControl1.Controls.Add(Me.UltraTabSharedControlsPage1)
        Me.UltraTabControl1.Controls.Add(Me.DrawingTabControl)
        Me.UltraTabControl1.Controls.Add(Me.SchematicsTabControl)
        Me.UltraTabControl1.Name = "UltraTabControl1"
        Me.UltraTabControl1.SharedControlsPage = Me.UltraTabSharedControlsPage1
        Me.UltraTabControl1.TabOrientation = Infragistics.Win.UltraWinTabs.TabOrientation.LeftTop
        UltraTab1.Key = "Drawing"
        UltraTab1.TabPage = Me.DrawingTabControl
        resources.ApplyResources(UltraTab1, "UltraTab1")
        UltraTab1.ForceApplyResources = ""
        UltraTab2.Key = "Schematics"
        UltraTab2.TabPage = Me.SchematicsTabControl
        resources.ApplyResources(UltraTab2, "UltraTab2")
        UltraTab2.ForceApplyResources = ""
        Me.UltraTabControl1.Tabs.AddRange(New Infragistics.Win.UltraWinTabControl.UltraTab() {UltraTab1, UltraTab2})
        Me.UltraTabControl1.TextOrientation = Infragistics.Win.UltraWinTabs.TextOrientation.Horizontal
        '
        'UltraTabSharedControlsPage1
        '
        resources.ApplyResources(Me.UltraTabSharedControlsPage1, "UltraTabSharedControlsPage1")
        Me.UltraTabSharedControlsPage1.Name = "UltraTabSharedControlsPage1"
        '
        'PdfSaveForm
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        resources.ApplyResources(Me, "$this")
        Me.Controls.Add(Me.UltraTabControl1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "PdfSaveForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.DrawingTabControl.ResumeLayout(False)
        Me.DrawingTabControl.PerformLayout()
        CType(Me.chkScaleToMaximum, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.uckEmbeddTTF, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtDrawingSavePath, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SchematicsTabControl.ResumeLayout(False)
        Me.SchematicsTabControl.PerformLayout()
        CType(Me.txtSchematicsSavePath, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.UltraTabControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.UltraTabControl1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents txtDrawingSavePath As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents uckEmbeddTTF As Infragistics.Win.UltraWinEditors.UltraCheckEditor
    Friend WithEvents UltraTabControl1 As Infragistics.Win.UltraWinTabControl.UltraTabControl
    Friend WithEvents UltraTabSharedControlsPage1 As Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage
    Friend WithEvents DrawingTabControl As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
    Friend WithEvents SchematicsTabControl As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
    Friend WithEvents UltraLabel2 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents UltraLabel1 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents txtSchematicsSavePath As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents btnDrawingCancel As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnDrawingOK As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnSchematicsOK As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnSchematicsCancel As Infragistics.Win.Misc.UltraButton
    Friend WithEvents chkScaleToMaximum As Infragistics.Win.UltraWinEditors.UltraCheckEditor
End Class
