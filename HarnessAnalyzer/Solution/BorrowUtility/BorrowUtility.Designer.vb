<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class BorrowUtility
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            If disposing AndAlso _logView IsNot Nothing Then
                _logView.Dispose()
            End If
            If disposing AndAlso _serverLicDlg IsNot Nothing Then
                _serverLicDlg.Dispose()
            End If
            If disposing AndAlso _manager IsNot Nothing Then
                Me._manager.Dispose()
            End If
            _manager = Nothing
            _serverLicDlg = Nothing
            _logView = Nothing
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim Appearance1 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(BorrowUtility))
        Dim Appearance2 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance3 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim DateButton2 As Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton = New Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton()
        Dim Appearance4 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance11 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance5 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance6 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance12 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim DateButton1 As Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton = New Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton()
        Me.UltraPanel1 = New Infragistics.Win.Misc.UltraPanel()
        Me.UltraPanel2 = New Infragistics.Win.Misc.UltraPanel()
        Me.ulStatus = New Infragistics.Win.Misc.UltraLabel()
        Me.ubtnClose = New Infragistics.Win.Misc.UltraButton()
        Me.upReturn = New Infragistics.Win.Misc.UltraPanel()
        Me.WaitReturn_Panel = New System.Windows.Forms.Panel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.UltraActivityIndicator1 = New Infragistics.Win.UltraActivityIndicator.UltraActivityIndicator()
        Me.ucalExpire = New Infragistics.Win.UltraWinSchedule.UltraCalendarCombo()
        Me.UltraLabel4 = New Infragistics.Win.Misc.UltraLabel()
        Me.UltraLabel1 = New Infragistics.Win.Misc.UltraLabel()
        Me.ubtnReturn = New Infragistics.Win.Misc.UltraButton()
        Me.upBorrow = New Infragistics.Win.Misc.UltraPanel()
        Me.WaitBorrow_Panel = New System.Windows.Forms.Panel()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.UltraActivityIndicator2 = New Infragistics.Win.UltraActivityIndicator.UltraActivityIndicator()
        Me.UltraLabel5 = New Infragistics.Win.Misc.UltraLabel()
        Me.UltraLabel2 = New Infragistics.Win.Misc.UltraLabel()
        Me.ubtnBorrow = New Infragistics.Win.Misc.UltraButton()
        Me.ucalBorrow = New Infragistics.Win.UltraWinSchedule.UltraCalendarCombo()
        Me.uvalBorrowDate = New Infragistics.Win.Misc.UltraValidator(Me.components)
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.ToolsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ViewLogToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ShowLicenseEnvVariablesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowServerLicensesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.CloseToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolsToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.EditLicensePathToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.ResetFlexLmToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.UltraPanel1.SuspendLayout()
        Me.UltraPanel2.ClientArea.SuspendLayout()
        Me.UltraPanel2.SuspendLayout()
        Me.upReturn.ClientArea.SuspendLayout()
        Me.upReturn.SuspendLayout()
        Me.WaitReturn_Panel.SuspendLayout()
        CType(Me.ucalExpire, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.upBorrow.ClientArea.SuspendLayout()
        Me.upBorrow.SuspendLayout()
        Me.WaitBorrow_Panel.SuspendLayout()
        CType(Me.ucalBorrow, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.uvalBorrowDate, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'UltraPanel1
        '
        Appearance1.ImageBackground = CType(resources.GetObject("Appearance1.ImageBackground"), System.Drawing.Image)
        resources.ApplyResources(Appearance1.FontData, "Appearance1.FontData")
        resources.ApplyResources(Appearance1, "Appearance1")
        Appearance1.ForceApplyResources = "FontData|"
        Me.UltraPanel1.Appearance = Appearance1
        Me.TableLayoutPanel1.SetColumnSpan(Me.UltraPanel1, 2)
        resources.ApplyResources(Me.UltraPanel1, "UltraPanel1")
        Me.UltraPanel1.Name = "UltraPanel1"
        '
        'UltraPanel2
        '
        Appearance2.BackColor = System.Drawing.Color.White
        resources.ApplyResources(Appearance2.FontData, "Appearance2.FontData")
        resources.ApplyResources(Appearance2, "Appearance2")
        Appearance2.ForceApplyResources = "FontData|"
        Me.UltraPanel2.Appearance = Appearance2
        '
        'UltraPanel2.ClientArea
        '
        Me.UltraPanel2.ClientArea.Controls.Add(Me.ulStatus)
        Me.UltraPanel2.ClientArea.Controls.Add(Me.ubtnClose)
        Me.TableLayoutPanel1.SetColumnSpan(Me.UltraPanel2, 2)
        resources.ApplyResources(Me.UltraPanel2, "UltraPanel2")
        Me.UltraPanel2.Name = "UltraPanel2"
        '
        'ulStatus
        '
        resources.ApplyResources(Me.ulStatus, "ulStatus")
        Me.ulStatus.Name = "ulStatus"
        '
        'ubtnClose
        '
        resources.ApplyResources(Me.ubtnClose, "ubtnClose")
        Me.ubtnClose.Name = "ubtnClose"
        '
        'upReturn
        '
        Appearance3.BackColor = System.Drawing.Color.White
        resources.ApplyResources(Appearance3.FontData, "Appearance3.FontData")
        resources.ApplyResources(Appearance3, "Appearance3")
        Appearance3.ForceApplyResources = "FontData|"
        Me.upReturn.Appearance = Appearance3
        '
        'upReturn.ClientArea
        '
        Me.upReturn.ClientArea.Controls.Add(Me.WaitReturn_Panel)
        Me.upReturn.ClientArea.Controls.Add(Me.ucalExpire)
        Me.upReturn.ClientArea.Controls.Add(Me.UltraLabel4)
        Me.upReturn.ClientArea.Controls.Add(Me.UltraLabel1)
        Me.upReturn.ClientArea.Controls.Add(Me.ubtnReturn)
        resources.ApplyResources(Me.upReturn, "upReturn")
        Me.upReturn.Name = "upReturn"
        '
        'WaitReturn_Panel
        '
        resources.ApplyResources(Me.WaitReturn_Panel, "WaitReturn_Panel")
        Me.WaitReturn_Panel.Controls.Add(Me.Label1)
        Me.WaitReturn_Panel.Controls.Add(Me.UltraActivityIndicator1)
        Me.WaitReturn_Panel.Name = "WaitReturn_Panel"
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
        '
        'UltraActivityIndicator1
        '
        resources.ApplyResources(Me.UltraActivityIndicator1, "UltraActivityIndicator1")
        Me.UltraActivityIndicator1.AnimationEnabled = True
        Me.UltraActivityIndicator1.AnimationSpeed = 25
        Me.UltraActivityIndicator1.CausesValidation = True
        Me.UltraActivityIndicator1.MarqueeAnimationStyle = Infragistics.Win.UltraActivityIndicator.MarqueeAnimationStyle.BounceBack
        Me.UltraActivityIndicator1.Name = "UltraActivityIndicator1"
        Me.UltraActivityIndicator1.TabStop = True
        '
        'ucalExpire
        '
        Me.ucalExpire.DateButtons.Add(DateButton2)
        resources.ApplyResources(Me.ucalExpire, "ucalExpire")
        Me.ucalExpire.Name = "ucalExpire"
        Me.ucalExpire.NonAutoSizeHeight = 25
        Me.ucalExpire.ReadOnly = True
        Me.ucalExpire.Value = New Date(2014, 5, 22, 0, 0, 0, 0)
        '
        'UltraLabel4
        '
        resources.ApplyResources(Me.UltraLabel4, "UltraLabel4")
        Appearance4.BackColor = System.Drawing.Color.White
        resources.ApplyResources(Appearance4.FontData, "Appearance4.FontData")
        resources.ApplyResources(Appearance4, "Appearance4")
        Appearance4.ForceApplyResources = "FontData|"
        Me.UltraLabel4.Appearance = Appearance4
        Me.UltraLabel4.Name = "UltraLabel4"
        '
        'UltraLabel1
        '
        resources.ApplyResources(Me.UltraLabel1, "UltraLabel1")
        Me.UltraLabel1.Name = "UltraLabel1"
        '
        'ubtnReturn
        '
        Appearance11.Image = Global.Zuken.E3.HarnessAnalyzer.BorrowUtility.My.Resources.Resources.return_small
        resources.ApplyResources(Appearance11.FontData, "Appearance11.FontData")
        resources.ApplyResources(Appearance11, "Appearance11")
        Appearance11.ForceApplyResources = "FontData|"
        Me.ubtnReturn.Appearance = Appearance11
        resources.ApplyResources(Me.ubtnReturn, "ubtnReturn")
        Me.ubtnReturn.Name = "ubtnReturn"
        '
        'upBorrow
        '
        Appearance5.BackColor = System.Drawing.Color.White
        resources.ApplyResources(Appearance5.FontData, "Appearance5.FontData")
        resources.ApplyResources(Appearance5, "Appearance5")
        Appearance5.ForceApplyResources = "FontData|"
        Me.upBorrow.Appearance = Appearance5
        '
        'upBorrow.ClientArea
        '
        Me.upBorrow.ClientArea.Controls.Add(Me.WaitBorrow_Panel)
        Me.upBorrow.ClientArea.Controls.Add(Me.UltraLabel5)
        Me.upBorrow.ClientArea.Controls.Add(Me.UltraLabel2)
        Me.upBorrow.ClientArea.Controls.Add(Me.ubtnBorrow)
        Me.upBorrow.ClientArea.Controls.Add(Me.ucalBorrow)
        resources.ApplyResources(Me.upBorrow, "upBorrow")
        Me.upBorrow.Name = "upBorrow"
        '
        'WaitBorrow_Panel
        '
        resources.ApplyResources(Me.WaitBorrow_Panel, "WaitBorrow_Panel")
        Me.WaitBorrow_Panel.Controls.Add(Me.Label2)
        Me.WaitBorrow_Panel.Controls.Add(Me.UltraActivityIndicator2)
        Me.WaitBorrow_Panel.Name = "WaitBorrow_Panel"
        '
        'Label2
        '
        resources.ApplyResources(Me.Label2, "Label2")
        Me.Label2.Name = "Label2"
        '
        'UltraActivityIndicator2
        '
        resources.ApplyResources(Me.UltraActivityIndicator2, "UltraActivityIndicator2")
        Me.UltraActivityIndicator2.AnimationEnabled = True
        Me.UltraActivityIndicator2.AnimationSpeed = 25
        Me.UltraActivityIndicator2.CausesValidation = True
        Me.UltraActivityIndicator2.MarqueeAnimationStyle = Infragistics.Win.UltraActivityIndicator.MarqueeAnimationStyle.BounceBack
        Me.UltraActivityIndicator2.Name = "UltraActivityIndicator2"
        Me.UltraActivityIndicator2.TabStop = True
        '
        'UltraLabel5
        '
        resources.ApplyResources(Me.UltraLabel5, "UltraLabel5")
        Appearance6.BackColor = System.Drawing.Color.White
        resources.ApplyResources(Appearance6.FontData, "Appearance6.FontData")
        resources.ApplyResources(Appearance6, "Appearance6")
        Appearance6.ForceApplyResources = "FontData|"
        Me.UltraLabel5.Appearance = Appearance6
        Me.UltraLabel5.Name = "UltraLabel5"
        '
        'UltraLabel2
        '
        resources.ApplyResources(Me.UltraLabel2, "UltraLabel2")
        Me.UltraLabel2.Name = "UltraLabel2"
        '
        'ubtnBorrow
        '
        Appearance12.Image = Global.Zuken.E3.HarnessAnalyzer.BorrowUtility.My.Resources.Resources.Borrow_small
        resources.ApplyResources(Appearance12.FontData, "Appearance12.FontData")
        resources.ApplyResources(Appearance12, "Appearance12")
        Appearance12.ForceApplyResources = "FontData|"
        Me.ubtnBorrow.Appearance = Appearance12
        resources.ApplyResources(Me.ubtnBorrow, "ubtnBorrow")
        Me.ubtnBorrow.Name = "ubtnBorrow"
        '
        'ucalBorrow
        '
        Me.ucalBorrow.AllowNull = False
        Me.ucalBorrow.DateButtons.Add(DateButton1)
        resources.ApplyResources(Me.ucalBorrow, "ucalBorrow")
        Me.ucalBorrow.Name = "ucalBorrow"
        Me.ucalBorrow.NonAutoSizeHeight = 25
        Me.ucalBorrow.Value = New Date(2014, 5, 22, 0, 0, 0, 0)
        '
        'uvalBorrowDate
        '
        '
        'TableLayoutPanel1
        '
        resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
        Me.TableLayoutPanel1.Controls.Add(Me.UltraPanel1, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.UltraPanel2, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.Panel1, 0, 1)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        '
        'Panel1
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.Panel1, 2)
        Me.Panel1.Controls.Add(Me.upBorrow)
        Me.Panel1.Controls.Add(Me.upReturn)
        resources.ApplyResources(Me.Panel1, "Panel1")
        Me.Panel1.Name = "Panel1"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolsToolStripMenuItem, Me.ToolsToolStripMenuItem1})
        resources.ApplyResources(Me.MenuStrip1, "MenuStrip1")
        Me.MenuStrip1.Name = "MenuStrip1"
        '
        'ToolsToolStripMenuItem
        '
        Me.ToolsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ViewLogToolStripMenuItem, Me.ToolStripSeparator1, Me.ShowLicenseEnvVariablesToolStripMenuItem, Me.ToolStripMenuItem1, Me.ShowServerLicensesToolStripMenuItem, Me.ToolStripSeparator2, Me.CloseToolStripMenuItem})
        Me.ToolsToolStripMenuItem.Name = "ToolsToolStripMenuItem"
        resources.ApplyResources(Me.ToolsToolStripMenuItem, "ToolsToolStripMenuItem")
        '
        'ViewLogToolStripMenuItem
        '
        Me.ViewLogToolStripMenuItem.Name = "ViewLogToolStripMenuItem"
        resources.ApplyResources(Me.ViewLogToolStripMenuItem, "ViewLogToolStripMenuItem")
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        resources.ApplyResources(Me.ToolStripSeparator1, "ToolStripSeparator1")
        '
        'ShowLicenseEnvVariablesToolStripMenuItem
        '
        Me.ShowLicenseEnvVariablesToolStripMenuItem.Name = "ShowLicenseEnvVariablesToolStripMenuItem"
        resources.ApplyResources(Me.ShowLicenseEnvVariablesToolStripMenuItem, "ShowLicenseEnvVariablesToolStripMenuItem")
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        resources.ApplyResources(Me.ToolStripMenuItem1, "ToolStripMenuItem1")
        '
        'ShowServerLicensesToolStripMenuItem
        '
        Me.ShowServerLicensesToolStripMenuItem.Name = "ShowServerLicensesToolStripMenuItem"
        resources.ApplyResources(Me.ShowServerLicensesToolStripMenuItem, "ShowServerLicensesToolStripMenuItem")
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        resources.ApplyResources(Me.ToolStripSeparator2, "ToolStripSeparator2")
        '
        'CloseToolStripMenuItem
        '
        Me.CloseToolStripMenuItem.Name = "CloseToolStripMenuItem"
        resources.ApplyResources(Me.CloseToolStripMenuItem, "CloseToolStripMenuItem")
        '
        'ToolsToolStripMenuItem1
        '
        Me.ToolsToolStripMenuItem1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.EditLicensePathToolStripMenuItem, Me.ToolStripSeparator3, Me.ResetFlexLmToolStripMenuItem})
        Me.ToolsToolStripMenuItem1.Name = "ToolsToolStripMenuItem1"
        resources.ApplyResources(Me.ToolsToolStripMenuItem1, "ToolsToolStripMenuItem1")
        '
        'EditLicensePathToolStripMenuItem
        '
        Me.EditLicensePathToolStripMenuItem.Name = "EditLicensePathToolStripMenuItem"
        resources.ApplyResources(Me.EditLicensePathToolStripMenuItem, "EditLicensePathToolStripMenuItem")
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        resources.ApplyResources(Me.ToolStripSeparator3, "ToolStripSeparator3")
        '
        'ResetFlexLmToolStripMenuItem
        '
        Me.ResetFlexLmToolStripMenuItem.Name = "ResetFlexLmToolStripMenuItem"
        resources.ApplyResources(Me.ResetFlexLmToolStripMenuItem, "ResetFlexLmToolStripMenuItem")
        '
        'BorrowUtility
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.CancelButton = Me.ubtnClose
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.MainMenuStrip = Me.MenuStrip1
        Me.MaximizeBox = False
        Me.Name = "BorrowUtility"
        Me.UltraPanel1.ResumeLayout(False)
        Me.UltraPanel2.ClientArea.ResumeLayout(False)
        Me.UltraPanel2.ResumeLayout(False)
        Me.upReturn.ClientArea.ResumeLayout(False)
        Me.upReturn.ClientArea.PerformLayout()
        Me.upReturn.ResumeLayout(False)
        Me.WaitReturn_Panel.ResumeLayout(False)
        Me.WaitReturn_Panel.PerformLayout()
        CType(Me.ucalExpire, System.ComponentModel.ISupportInitialize).EndInit()
        Me.upBorrow.ClientArea.ResumeLayout(False)
        Me.upBorrow.ClientArea.PerformLayout()
        Me.upBorrow.ResumeLayout(False)
        Me.WaitBorrow_Panel.ResumeLayout(False)
        Me.WaitBorrow_Panel.PerformLayout()
        CType(Me.ucalBorrow, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.uvalBorrowDate, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ubtnClose As Infragistics.Win.Misc.UltraButton
    Friend WithEvents ubtnBorrow As Infragistics.Win.Misc.UltraButton
    Friend WithEvents ubtnReturn As Infragistics.Win.Misc.UltraButton
    Friend WithEvents ucalBorrow As Infragistics.Win.UltraWinSchedule.UltraCalendarCombo
    Friend WithEvents UltraPanel1 As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents UltraPanel2 As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents upReturn As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents upBorrow As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents UltraLabel1 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents UltraLabel2 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents ucalExpire As Infragistics.Win.UltraWinSchedule.UltraCalendarCombo
    Friend WithEvents ulStatus As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents uvalBorrowDate As Infragistics.Win.Misc.UltraValidator
    Friend WithEvents UltraLabel4 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents UltraLabel5 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents ToolsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ShowServerLicensesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ViewLogToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolsToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents EditLicensePathToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents Panel1 As Panel
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
    Friend WithEvents CloseToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ResetFlexLmToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator3 As ToolStripSeparator
    Friend WithEvents ShowLicenseEnvVariablesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents WaitBorrow_Panel As Panel
    Friend WithEvents Label2 As Label
    Friend WithEvents UltraActivityIndicator2 As Infragistics.Win.UltraActivityIndicator.UltraActivityIndicator
    Friend WithEvents WaitReturn_Panel As Panel
    Friend WithEvents Label1 As Label
    Friend WithEvents UltraActivityIndicator1 As Infragistics.Win.UltraActivityIndicator.UltraActivityIndicator
End Class
