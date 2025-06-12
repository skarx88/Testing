<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class CompareForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    '<System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(CompareForm))
        Dim Appearance34 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance2 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance38 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim UltraToolTipInfo5 As Infragistics.Win.UltraWinToolTip.UltraToolTipInfo = New Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Abgehakte Änderungen in Referenzdokument speichern", Infragistics.Win.ToolTipImage.[Default], Nothing, Infragistics.Win.DefaultableBoolean.[Default])
        Dim Appearance39 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance41 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance40 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance35 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance36 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance37 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim EditorButton1 As Infragistics.Win.UltraWinEditors.EditorButton = New Infragistics.Win.UltraWinEditors.EditorButton("btnEdit")
        Me.ugbCompareSettings = New Infragistics.Win.Misc.UltraGroupBox()
        Me.btnDiffsInformationImage = New Infragistics.Win.Misc.UltraButton()
        Me.picConfigStatus = New Infragistics.Win.UltraWinEditors.UltraPictureBox()
        Me.lblConfigStatus = New Infragistics.Win.Misc.UltraLabel()
        Me.uceCompareConfig = New Infragistics.Win.UltraWinEditors.UltraComboEditor()
        Me.lblCompareConfig = New Infragistics.Win.Misc.UltraLabel()
        Me.uceReferenceConfig = New Infragistics.Win.UltraWinEditors.UltraComboEditor()
        Me.lblReferenceConfig = New Infragistics.Win.Misc.UltraLabel()
        Me.btnSelectDocument = New Infragistics.Win.Misc.UltraButton()
        Me.uceCompareDocument = New Infragistics.Win.UltraWinEditors.UltraComboEditor()
        Me.txtReferenceDocument = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.lblCompareDocument = New Infragistics.Win.Misc.UltraLabel()
        Me.lblReferenceDocument = New Infragistics.Win.Misc.UltraLabel()
        Me.ugbCompareResults = New Infragistics.Win.Misc.UltraGroupBox()
        Me.btnClose = New Infragistics.Win.Misc.UltraButton()
        Me.btnExport = New Infragistics.Win.Misc.UltraButton()
        Me.btnCompare = New Infragistics.Win.Misc.UltraButton()
        Me.bwCompare = New System.ComponentModel.BackgroundWorker()
        Me.upbCompare = New Infragistics.Win.UltraWinProgressBar.UltraProgressBar()
        Me.uttmCompare = New Infragistics.Win.UltraWinToolTip.UltraToolTipManager(Me.components)
        Me.btnSave = New Infragistics.Win.Misc.UltraButton()
        Me.lblLegend1 = New Infragistics.Win.Misc.UltraLabel()
        Me.lblLegend2 = New Infragistics.Win.Misc.UltraLabel()
        Me.lblLegend3 = New Infragistics.Win.Misc.UltraLabel()
        Me.lblComment = New Infragistics.Win.Misc.UltraLabel()
        Me.btnNext = New Infragistics.Win.Misc.UltraButton()
        Me.btnPrevious = New Infragistics.Win.Misc.UltraButton()
        Me.txtComment = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        CType(Me.ugbCompareSettings, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ugbCompareSettings.SuspendLayout()
        CType(Me.uceCompareConfig, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.uceReferenceConfig, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.uceCompareDocument, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtReferenceDocument, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ugbCompareResults, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtComment, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ugbCompareSettings
        '
        resources.ApplyResources(Me.ugbCompareSettings, "ugbCompareSettings")
        Me.ugbCompareSettings.Controls.Add(Me.btnDiffsInformationImage)
        Me.ugbCompareSettings.Controls.Add(Me.picConfigStatus)
        Me.ugbCompareSettings.Controls.Add(Me.lblConfigStatus)
        Me.ugbCompareSettings.Controls.Add(Me.uceCompareConfig)
        Me.ugbCompareSettings.Controls.Add(Me.lblCompareConfig)
        Me.ugbCompareSettings.Controls.Add(Me.uceReferenceConfig)
        Me.ugbCompareSettings.Controls.Add(Me.lblReferenceConfig)
        Me.ugbCompareSettings.Controls.Add(Me.btnSelectDocument)
        Me.ugbCompareSettings.Controls.Add(Me.uceCompareDocument)
        Me.ugbCompareSettings.Controls.Add(Me.txtReferenceDocument)
        Me.ugbCompareSettings.Controls.Add(Me.lblCompareDocument)
        Me.ugbCompareSettings.Controls.Add(Me.lblReferenceDocument)
        Me.ugbCompareSettings.Name = "ugbCompareSettings"
        '
        'btnDiffsInformationImage
        '
        resources.ApplyResources(Me.btnDiffsInformationImage, "btnDiffsInformationImage")
        Appearance34.BackColor = System.Drawing.Color.Transparent
        Appearance34.BackColor2 = System.Drawing.Color.Transparent
        Appearance34.Image = Global.Zuken.E3.HarnessAnalyzer.My.Resources.Resources.information
        Appearance34.ImageHAlign = Infragistics.Win.HAlign.Center
        Appearance34.ImageVAlign = Infragistics.Win.VAlign.Middle
        Me.btnDiffsInformationImage.Appearance = Appearance34
        Me.btnDiffsInformationImage.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Borderless
        Me.btnDiffsInformationImage.ImageSize = New System.Drawing.Size(40, 40)
        Me.btnDiffsInformationImage.Name = "btnDiffsInformationImage"
        Me.btnDiffsInformationImage.UseOsThemes = Infragistics.Win.DefaultableBoolean.[False]
        '
        'picConfigStatus
        '
        Me.picConfigStatus.BorderShadowColor = System.Drawing.Color.Empty
        resources.ApplyResources(Me.picConfigStatus, "picConfigStatus")
        Me.picConfigStatus.Name = "picConfigStatus"
        '
        'lblConfigStatus
        '
        resources.ApplyResources(Me.lblConfigStatus, "lblConfigStatus")
        Me.lblConfigStatus.Name = "lblConfigStatus"
        '
        'uceCompareConfig
        '
        resources.ApplyResources(Me.uceCompareConfig, "uceCompareConfig")
        Me.uceCompareConfig.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList
        Me.uceCompareConfig.Name = "uceCompareConfig"
        '
        'lblCompareConfig
        '
        resources.ApplyResources(Me.lblCompareConfig, "lblCompareConfig")
        Me.lblCompareConfig.Name = "lblCompareConfig"
        '
        'uceReferenceConfig
        '
        Me.uceReferenceConfig.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList
        resources.ApplyResources(Me.uceReferenceConfig, "uceReferenceConfig")
        Me.uceReferenceConfig.Name = "uceReferenceConfig"
        '
        'lblReferenceConfig
        '
        resources.ApplyResources(Me.lblReferenceConfig, "lblReferenceConfig")
        Me.lblReferenceConfig.Name = "lblReferenceConfig"
        '
        'btnSelectDocument
        '
        resources.ApplyResources(Me.btnSelectDocument, "btnSelectDocument")
        Me.btnSelectDocument.Name = "btnSelectDocument"
        '
        'uceCompareDocument
        '
        resources.ApplyResources(Me.uceCompareDocument, "uceCompareDocument")
        Me.uceCompareDocument.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList
        Me.uceCompareDocument.Name = "uceCompareDocument"
        '
        'txtReferenceDocument
        '
        Appearance2.BackColor = System.Drawing.Color.White
        Appearance2.FontData.BoldAsString = resources.GetString("resource.BoldAsString")
        Me.txtReferenceDocument.Appearance = Appearance2
        Me.txtReferenceDocument.BackColor = System.Drawing.Color.White
        resources.ApplyResources(Me.txtReferenceDocument, "txtReferenceDocument")
        Me.txtReferenceDocument.Name = "txtReferenceDocument"
        Me.txtReferenceDocument.ReadOnly = True
        '
        'lblCompareDocument
        '
        resources.ApplyResources(Me.lblCompareDocument, "lblCompareDocument")
        Me.lblCompareDocument.Name = "lblCompareDocument"
        '
        'lblReferenceDocument
        '
        resources.ApplyResources(Me.lblReferenceDocument, "lblReferenceDocument")
        Me.lblReferenceDocument.Name = "lblReferenceDocument"
        '
        'ugbCompareResults
        '
        resources.ApplyResources(Me.ugbCompareResults, "ugbCompareResults")
        Me.ugbCompareResults.Name = "ugbCompareResults"
        '
        'btnClose
        '
        resources.ApplyResources(Me.btnClose, "btnClose")
        Me.btnClose.Name = "btnClose"
        '
        'btnExport
        '
        resources.ApplyResources(Me.btnExport, "btnExport")
        Me.btnExport.Name = "btnExport"
        '
        'btnCompare
        '
        resources.ApplyResources(Me.btnCompare, "btnCompare")
        Me.btnCompare.Name = "btnCompare"
        '
        'bwCompare
        '
        Me.bwCompare.WorkerReportsProgress = True
        Me.bwCompare.WorkerSupportsCancellation = True
        '
        'upbCompare
        '
        resources.ApplyResources(Me.upbCompare, "upbCompare")
        Me.upbCompare.Name = "upbCompare"
        '
        'uttmCompare
        '
        Me.uttmCompare.ContainingControl = Me
        '
        'btnSave
        '
        resources.ApplyResources(Me.btnSave, "btnSave")
        Appearance38.Image = Global.Zuken.E3.HarnessAnalyzer.My.Resources.Resources.SaveCompareResult
        Me.btnSave.Appearance = Appearance38
        Me.btnSave.Name = "btnSave"
        resources.ApplyResources(UltraToolTipInfo5, "UltraToolTipInfo5")
        Me.uttmCompare.SetUltraToolTip(Me.btnSave, UltraToolTipInfo5)
        '
        'lblLegend1
        '
        resources.ApplyResources(Me.lblLegend1, "lblLegend1")
        resources.ApplyResources(Appearance39, "Appearance39")
        Me.lblLegend1.Appearance = Appearance39
        Me.lblLegend1.Name = "lblLegend1"
        '
        'lblLegend2
        '
        resources.ApplyResources(Me.lblLegend2, "lblLegend2")
        resources.ApplyResources(Appearance41, "Appearance41")
        Me.lblLegend2.Appearance = Appearance41
        Me.lblLegend2.Name = "lblLegend2"
        '
        'lblLegend3
        '
        resources.ApplyResources(Me.lblLegend3, "lblLegend3")
        Appearance40.ForeColor = System.Drawing.Color.DarkOrange
        resources.ApplyResources(Appearance40, "Appearance40")
        Me.lblLegend3.Appearance = Appearance40
        Me.lblLegend3.Name = "lblLegend3"
        '
        'lblComment
        '
        resources.ApplyResources(Me.lblComment, "lblComment")
        resources.ApplyResources(Appearance35, "Appearance35")
        Me.lblComment.Appearance = Appearance35
        Me.lblComment.Name = "lblComment"
        '
        'btnNext
        '
        resources.ApplyResources(Me.btnNext, "btnNext")
        Appearance36.FontData.BoldAsString = resources.GetString("resource.BoldAsString1")
        Me.btnNext.Appearance = Appearance36
        Me.btnNext.Name = "btnNext"
        '
        'btnPrevious
        '
        resources.ApplyResources(Me.btnPrevious, "btnPrevious")
        Appearance37.FontData.BoldAsString = resources.GetString("resource.BoldAsString2")
        Me.btnPrevious.Appearance = Appearance37
        Me.btnPrevious.Name = "btnPrevious"
        '
        'txtComment
        '
        resources.ApplyResources(Me.txtComment, "txtComment")
        EditorButton1.Key = "btnEdit"
        Me.txtComment.ButtonsRight.Add(EditorButton1)
        Me.txtComment.Name = "txtComment"
        '
        'SplitContainer1
        '
        resources.ApplyResources(Me.SplitContainer1, "SplitContainer1")
        Me.SplitContainer1.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.BackColor = System.Drawing.Color.White
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.BackColor = System.Drawing.Color.White
        Me.SplitContainer1.Panel2.Controls.Add(Me.ugbCompareResults)
        '
        'CompareForm
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        resources.ApplyResources(Me, "$this")
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.lblComment)
        Me.Controls.Add(Me.btnNext)
        Me.Controls.Add(Me.btnPrevious)
        Me.Controls.Add(Me.txtComment)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.lblLegend1)
        Me.Controls.Add(Me.btnCompare)
        Me.Controls.Add(Me.lblLegend3)
        Me.Controls.Add(Me.btnExport)
        Me.Controls.Add(Me.lblLegend2)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.ugbCompareSettings)
        Me.Controls.Add(Me.upbCompare)
        Me.KeyPreview = True
        Me.Name = "CompareForm"
        CType(Me.ugbCompareSettings, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ugbCompareSettings.ResumeLayout(False)
        Me.ugbCompareSettings.PerformLayout()
        CType(Me.uceCompareConfig, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.uceReferenceConfig, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.uceCompareDocument, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtReferenceDocument, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ugbCompareResults, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtComment, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents bwCompare As System.ComponentModel.BackgroundWorker
    Private WithEvents ugbCompareSettings As Infragistics.Win.Misc.UltraGroupBox
    Private WithEvents btnSelectDocument As Infragistics.Win.Misc.UltraButton
    Private WithEvents uceCompareDocument As Infragistics.Win.UltraWinEditors.UltraComboEditor
    Private WithEvents txtReferenceDocument As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Private WithEvents lblCompareDocument As Infragistics.Win.Misc.UltraLabel
    Private WithEvents lblReferenceDocument As Infragistics.Win.Misc.UltraLabel
    Private WithEvents ugbCompareResults As Infragistics.Win.Misc.UltraGroupBox
    Private WithEvents btnClose As Infragistics.Win.Misc.UltraButton
    Private WithEvents btnExport As Infragistics.Win.Misc.UltraButton
    Private WithEvents btnCompare As Infragistics.Win.Misc.UltraButton
    Private WithEvents upbCompare As Infragistics.Win.UltraWinProgressBar.UltraProgressBar
    Friend WithEvents uttmCompare As Infragistics.Win.UltraWinToolTip.UltraToolTipManager
    Friend WithEvents lblLegend1 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents lblLegend3 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents lblLegend2 As Infragistics.Win.Misc.UltraLabel
    Private WithEvents uceCompareConfig As Infragistics.Win.UltraWinEditors.UltraComboEditor
    Private WithEvents lblCompareConfig As Infragistics.Win.Misc.UltraLabel
    Private WithEvents uceReferenceConfig As Infragistics.Win.UltraWinEditors.UltraComboEditor
    Private WithEvents lblReferenceConfig As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents picConfigStatus As Infragistics.Win.UltraWinEditors.UltraPictureBox
    Friend WithEvents lblConfigStatus As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents btnDiffsInformationImage As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnSave As Infragistics.Win.Misc.UltraButton
    Friend WithEvents lblComment As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents btnNext As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnPrevious As Infragistics.Win.Misc.UltraButton
    Friend WithEvents txtComment As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents SplitContainer1 As SplitContainer
End Class
