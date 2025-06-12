<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class QMIssuesForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(QMIssuesForm))
        Dim Appearance1 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim EditorButton1 As Infragistics.Win.UltraWinEditors.EditorButton = New Infragistics.Win.UltraWinEditors.EditorButton("FileSelect")
        ugbQM = New Infragistics.Win.Misc.UltraGroupBox()
        btnDebugGenerate = New Infragistics.Win.Misc.UltraButton()
        ugbColors = New Infragistics.Win.Misc.UltraGroupBox()
        colorRange5 = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
        ucp5 = New Infragistics.Win.UltraWinEditors.UltraColorPicker()
        UltraLabel6 = New Infragistics.Win.Misc.UltraLabel()
        colorRange4 = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
        colorRange3 = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
        colorRange2 = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
        colorRange1 = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
        ucp4 = New Infragistics.Win.UltraWinEditors.UltraColorPicker()
        ucp3 = New Infragistics.Win.UltraWinEditors.UltraColorPicker()
        ucp2 = New Infragistics.Win.UltraWinEditors.UltraColorPicker()
        ucp1 = New Infragistics.Win.UltraWinEditors.UltraColorPicker()
        UltraLabel5 = New Infragistics.Win.Misc.UltraLabel()
        UltraLabel4 = New Infragistics.Win.Misc.UltraLabel()
        UltraLabel3 = New Infragistics.Win.Misc.UltraLabel()
        UltraLabel2 = New Infragistics.Win.Misc.UltraLabel()
        txtQMFile = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        UltraLabel1 = New Infragistics.Win.Misc.UltraLabel()
        btnView = New Infragistics.Win.Misc.UltraButton()
        btnCancel = New Infragistics.Win.Misc.UltraButton()
        CType(ugbQM, ComponentModel.ISupportInitialize).BeginInit()
        ugbQM.SuspendLayout()
        CType(ugbColors, ComponentModel.ISupportInitialize).BeginInit()
        ugbColors.SuspendLayout()
        CType(colorRange5, ComponentModel.ISupportInitialize).BeginInit()
        CType(ucp5, ComponentModel.ISupportInitialize).BeginInit()
        CType(colorRange4, ComponentModel.ISupportInitialize).BeginInit()
        CType(colorRange3, ComponentModel.ISupportInitialize).BeginInit()
        CType(colorRange2, ComponentModel.ISupportInitialize).BeginInit()
        CType(colorRange1, ComponentModel.ISupportInitialize).BeginInit()
        CType(ucp4, ComponentModel.ISupportInitialize).BeginInit()
        CType(ucp3, ComponentModel.ISupportInitialize).BeginInit()
        CType(ucp2, ComponentModel.ISupportInitialize).BeginInit()
        CType(ucp1, ComponentModel.ISupportInitialize).BeginInit()
        CType(txtQMFile, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' ugbQM
        ' 
        resources.ApplyResources(ugbQM, "ugbQM")
        ugbQM.Controls.Add(btnDebugGenerate)
        ugbQM.Controls.Add(ugbColors)
        ugbQM.Controls.Add(txtQMFile)
        ugbQM.Controls.Add(UltraLabel1)
        ugbQM.Name = "ugbQM"
        ' 
        ' btnDebugGenerate
        ' 
        resources.ApplyResources(btnDebugGenerate, "btnDebugGenerate")
        btnDebugGenerate.Name = "btnDebugGenerate"
        ' 
        ' ugbColors
        ' 
        resources.ApplyResources(ugbColors, "ugbColors")
        ugbColors.Controls.Add(colorRange5)
        ugbColors.Controls.Add(ucp5)
        ugbColors.Controls.Add(UltraLabel6)
        ugbColors.Controls.Add(colorRange4)
        ugbColors.Controls.Add(colorRange3)
        ugbColors.Controls.Add(colorRange2)
        ugbColors.Controls.Add(colorRange1)
        ugbColors.Controls.Add(ucp4)
        ugbColors.Controls.Add(ucp3)
        ugbColors.Controls.Add(ucp2)
        ugbColors.Controls.Add(ucp1)
        ugbColors.Controls.Add(UltraLabel5)
        ugbColors.Controls.Add(UltraLabel4)
        ugbColors.Controls.Add(UltraLabel3)
        ugbColors.Controls.Add(UltraLabel2)
        ugbColors.Name = "ugbColors"
        ' 
        ' colorRange5
        ' 
        resources.ApplyResources(colorRange5, "colorRange5")
        colorRange5.Name = "colorRange5"
        colorRange5.Nullable = True
        colorRange5.NullText = "<Infinite>"
        colorRange5.PromptChar = " "c
        colorRange5.MaxValue = Byte.MaxValue
        colorRange5.Value = String.Empty
        ' 
        ' ucp5
        ' 
        ucp5.DefaultColor = Color.Red
        resources.ApplyResources(ucp5, "ucp5")
        ucp5.Name = "ucp5"
        ' 
        ' UltraLabel6
        ' 
        resources.ApplyResources(UltraLabel6, "UltraLabel6")
        UltraLabel6.Name = "UltraLabel6"
        ' 
        ' colorRange4
        ' 
        resources.ApplyResources(colorRange4, "colorRange4")
        colorRange4.Name = "colorRange4"
        colorRange4.Nullable = True
        colorRange4.NullText = "<Infinite>"
        colorRange4.PromptChar = " "c
        colorRange4.MaxValue = Byte.MaxValue
        colorRange4.Value = 20
        ' 
        ' colorRange3
        ' 
        resources.ApplyResources(colorRange3, "colorRange3")
        colorRange3.Name = "colorRange3"
        colorRange3.Nullable = True
        colorRange3.NullText = "<Infinite>"
        colorRange3.PromptChar = " "c
        colorRange3.MaxValue = Byte.MaxValue
        colorRange3.Value = 10
        ' 
        ' colorRange2
        ' 
        resources.ApplyResources(colorRange2, "colorRange2")
        colorRange2.Name = "colorRange2"
        colorRange2.Nullable = True
        colorRange2.NullText = "<Infinite>"
        colorRange2.PromptChar = " "c
        colorRange2.MaxValue = Byte.MaxValue
        colorRange2.Value = 5
        ' 
        ' colorRange1
        ' 
        resources.ApplyResources(colorRange1, "colorRange1")
        colorRange1.Name = "colorRange1"
        colorRange1.Nullable = True
        colorRange1.NullText = "<Infinite>"
        colorRange1.PromptChar = " "c
        colorRange1.MaxValue = Byte.MaxValue
        colorRange1.Value = 2
        ' 
        ' ucp4
        ' 
        ucp4.DefaultColor = Color.FromArgb(CByte(255), CByte(50), CByte(50))
        resources.ApplyResources(ucp4, "ucp4")
        ucp4.Name = "ucp4"
        ' 
        ' ucp3
        ' 
        ucp3.DefaultColor = Color.FromArgb(CByte(255), CByte(100), CByte(100))
        resources.ApplyResources(ucp3, "ucp3")
        ucp3.Name = "ucp3"
        ' 
        ' ucp2
        ' 
        ucp2.DefaultColor = Color.FromArgb(CByte(255), CByte(150), CByte(150))
        resources.ApplyResources(ucp2, "ucp2")
        ucp2.Name = "ucp2"
        ' 
        ' ucp1
        ' 
        ucp1.DefaultColor = Color.FromArgb(CByte(255), CByte(192), CByte(192))
        resources.ApplyResources(ucp1, "ucp1")
        ucp1.Name = "ucp1"
        ' 
        ' UltraLabel5
        ' 
        resources.ApplyResources(UltraLabel5, "UltraLabel5")
        UltraLabel5.Name = "UltraLabel5"
        ' 
        ' UltraLabel4
        ' 
        resources.ApplyResources(UltraLabel4, "UltraLabel4")
        UltraLabel4.Name = "UltraLabel4"
        ' 
        ' UltraLabel3
        ' 
        resources.ApplyResources(UltraLabel3, "UltraLabel3")
        UltraLabel3.Name = "UltraLabel3"
        ' 
        ' UltraLabel2
        ' 
        resources.ApplyResources(UltraLabel2, "UltraLabel2")
        UltraLabel2.Name = "UltraLabel2"
        ' 
        ' txtQMFile
        ' 
        resources.ApplyResources(txtQMFile, "txtQMFile")
        Appearance1.BackColor = Color.White
        txtQMFile.Appearance = Appearance1
        txtQMFile.BackColor = Color.White
        EditorButton1.Key = "FileSelect"
        resources.ApplyResources(EditorButton1, "EditorButton1")
        txtQMFile.ButtonsRight.Add(EditorButton1)
        txtQMFile.Name = "txtQMFile"
        txtQMFile.ReadOnly = True
        ' 
        ' UltraLabel1
        ' 
        resources.ApplyResources(UltraLabel1, "UltraLabel1")
        UltraLabel1.Name = "UltraLabel1"
        ' 
        ' btnView
        ' 
        resources.ApplyResources(btnView, "btnView")
        btnView.Name = "btnView"
        ' 
        ' btnCancel
        ' 
        btnCancel.AccessibleRole = AccessibleRole.None
        resources.ApplyResources(btnCancel, "btnCancel")
        btnCancel.DialogResult = DialogResult.Cancel
        btnCancel.Name = "btnCancel"
        ' 
        ' QMIssuesForm
        ' 
        AutoScaleMode = AutoScaleMode.Inherit
        BackColor = Color.White
        resources.ApplyResources(Me, "$this")
        Controls.Add(btnView)
        Controls.Add(btnCancel)
        Controls.Add(ugbQM)
        MinimizeBox = False
        Name = "QMIssuesForm"
        CType(ugbQM, ComponentModel.ISupportInitialize).EndInit()
        ugbQM.ResumeLayout(False)
        ugbQM.PerformLayout()
        CType(ugbColors, ComponentModel.ISupportInitialize).EndInit()
        ugbColors.ResumeLayout(False)
        ugbColors.PerformLayout()
        CType(colorRange5, ComponentModel.ISupportInitialize).EndInit()
        CType(ucp5, ComponentModel.ISupportInitialize).EndInit()
        CType(colorRange4, ComponentModel.ISupportInitialize).EndInit()
        CType(colorRange3, ComponentModel.ISupportInitialize).EndInit()
        CType(colorRange2, ComponentModel.ISupportInitialize).EndInit()
        CType(colorRange1, ComponentModel.ISupportInitialize).EndInit()
        CType(ucp4, ComponentModel.ISupportInitialize).EndInit()
        CType(ucp3, ComponentModel.ISupportInitialize).EndInit()
        CType(ucp2, ComponentModel.ISupportInitialize).EndInit()
        CType(ucp1, ComponentModel.ISupportInitialize).EndInit()
        CType(txtQMFile, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)

    End Sub

    Friend WithEvents ugbQM As Infragistics.Win.Misc.UltraGroupBox
    Friend WithEvents btnDebugGenerate As Infragistics.Win.Misc.UltraButton
    Friend WithEvents ugbColors As Infragistics.Win.Misc.UltraGroupBox
    Friend WithEvents colorRange5 As Infragistics.Win.UltraWinEditors.UltraNumericEditor
    Friend WithEvents ucp5 As Infragistics.Win.UltraWinEditors.UltraColorPicker
    Friend WithEvents UltraLabel6 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents colorRange4 As Infragistics.Win.UltraWinEditors.UltraNumericEditor
    Friend WithEvents colorRange3 As Infragistics.Win.UltraWinEditors.UltraNumericEditor
    Friend WithEvents colorRange2 As Infragistics.Win.UltraWinEditors.UltraNumericEditor
    Friend WithEvents colorRange1 As Infragistics.Win.UltraWinEditors.UltraNumericEditor
    Friend WithEvents ucp4 As Infragistics.Win.UltraWinEditors.UltraColorPicker
    Friend WithEvents ucp3 As Infragistics.Win.UltraWinEditors.UltraColorPicker
    Friend WithEvents ucp2 As Infragistics.Win.UltraWinEditors.UltraColorPicker
    Friend WithEvents ucp1 As Infragistics.Win.UltraWinEditors.UltraColorPicker
    Friend WithEvents UltraLabel5 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents UltraLabel4 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents UltraLabel3 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents UltraLabel2 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents txtQMFile As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents UltraLabel1 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents btnView As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnCancel As Infragistics.Win.Misc.UltraButton
End Class
