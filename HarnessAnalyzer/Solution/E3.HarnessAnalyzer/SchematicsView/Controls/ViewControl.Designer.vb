Namespace Schematics.Controls
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ViewControl
        Inherits System.Windows.Forms.UserControl

        'UserControl überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                DisposeInternals(disposing)
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
            Me.components = New System.ComponentModel.Container()
            Dim Appearance1 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ViewControl))
            Dim Appearance2 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim UltraToolTipInfo2 As Infragistics.Win.UltraWinToolTip.UltraToolTipInfo = New Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Bring selection into view", Infragistics.Win.ToolTipImage.[Default], Nothing, Infragistics.Win.DefaultableBoolean.[Default])
            Dim Appearance3 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim UltraToolTipInfo3 As Infragistics.Win.UltraWinToolTip.UltraToolTipInfo = New Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Reset", Infragistics.Win.ToolTipImage.[Default], Nothing, Infragistics.Win.DefaultableBoolean.[Default])
            Dim Appearance4 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
            Dim UltraToolTipInfo4 As Infragistics.Win.UltraWinToolTip.UltraToolTipInfo = New Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Redraw (refresh)", Infragistics.Win.ToolTipImage.[Default], Nothing, Infragistics.Win.DefaultableBoolean.[Default])
            Dim UltraToolTipInfo1 As Infragistics.Win.UltraWinToolTip.UltraToolTipInfo = New Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Zurücksetzen", Infragistics.Win.ToolTipImage.[Default], Nothing, Infragistics.Win.DefaultableBoolean.[Default])
            Me.UltraPanel1 = New Infragistics.Win.Misc.UltraPanel()
            Me.lblNoEntities = New System.Windows.Forms.Label()
            Me.btnSyncSelection = New Zuken.E3.HarnessAnalyzer.Schematics.Controls.RoundedButton()
            Me.btnReset = New Zuken.E3.HarnessAnalyzer.Schematics.Controls.RoundedButton()
            Me.btnRefresh = New Zuken.E3.HarnessAnalyzer.Schematics.Controls.RoundedButton()
            Me.ProgressBar = New ProgressBarCircularEx()
            Me.UltraToolTipManager1 = New Infragistics.Win.UltraWinToolTip.UltraToolTipManager(Me.components)
            Me.UltraPanel1.ClientArea.SuspendLayout()
            Me.UltraPanel1.SuspendLayout()
            Me.SuspendLayout()
            '
            'UltraPanel1
            '
            Appearance1.BackColor = System.Drawing.Color.White
            Me.UltraPanel1.Appearance = Appearance1
            '
            'UltraPanel1.ClientArea
            '
            Me.UltraPanel1.ClientArea.Controls.Add(Me.lblNoEntities)
            Me.UltraPanel1.ClientArea.Controls.Add(Me.btnSyncSelection)
            Me.UltraPanel1.ClientArea.Controls.Add(Me.btnReset)
            Me.UltraPanel1.ClientArea.Controls.Add(Me.btnRefresh)
            Me.UltraPanel1.ClientArea.Controls.Add(Me.ProgressBar)
            Dim names As String() = Reflection.Assembly.GetEntryAssembly.GetManifestResourceNames
            resources.ApplyResources(Me.UltraPanel1, "UltraPanel1")
            Me.UltraPanel1.Name = "UltraPanel1"
            '
            'lblNoEntities
            '
            resources.ApplyResources(Me.lblNoEntities, "lblNoEntities")
            Me.lblNoEntities.Name = "lblNoEntities"
            '
            'btnSyncSelection
            '
            Appearance2.BorderColor = System.Drawing.Color.Transparent
            Appearance2.Image = Global.Zuken.E3.HarnessAnalyzer.My.Resources.Resources.Sync3D
            Appearance2.ImageHAlign = Infragistics.Win.HAlign.Center
            Appearance2.ImageVAlign = Infragistics.Win.VAlign.Middle
            Me.btnSyncSelection.Appearance = Appearance2
            Me.btnSyncSelection.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton
            Me.btnSyncSelection.CornerRadius = CType(34US, UShort)
            resources.ApplyResources(Me.btnSyncSelection, "btnSyncSelection")
            Me.btnSyncSelection.ImageSize = New System.Drawing.Size(28, 28)
            Me.btnSyncSelection.Name = "btnSyncSelection"
            Me.btnSyncSelection.ShowFocusRect = False
            Me.btnSyncSelection.ShowOutline = False
            resources.ApplyResources(UltraToolTipInfo2, "UltraToolTipInfo2")
            Me.UltraToolTipManager1.SetUltraToolTip(Me.btnSyncSelection, UltraToolTipInfo2)
            Me.btnSyncSelection.UseOsThemes = Infragistics.Win.DefaultableBoolean.[False]
            '
            'btnReset
            '
            Appearance3.BorderColor = System.Drawing.Color.Transparent
            Appearance3.Image = Global.Zuken.E3.HarnessAnalyzer.My.Resources.Resources.Home
            Appearance3.ImageHAlign = Infragistics.Win.HAlign.Center
            Appearance3.ImageVAlign = Infragistics.Win.VAlign.Middle
            Me.btnReset.Appearance = Appearance3
            Me.btnReset.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton
            Me.btnReset.CornerRadius = CType(34US, UShort)
            resources.ApplyResources(Me.btnReset, "btnReset")
            Me.btnReset.ImageSize = New System.Drawing.Size(28, 28)
            Me.btnReset.ImageTransparentColor = System.Drawing.Color.White
            Me.btnReset.Name = "btnReset"
            Me.btnReset.ShowFocusRect = False
            Me.btnReset.ShowOutline = False
            resources.ApplyResources(UltraToolTipInfo3, "UltraToolTipInfo3")
            Me.UltraToolTipManager1.SetUltraToolTip(Me.btnReset, UltraToolTipInfo3)
            Me.btnReset.UseOsThemes = Infragistics.Win.DefaultableBoolean.[False]
            '
            'btnRefresh
            '
            Appearance4.BorderColor = System.Drawing.Color.Transparent
            Appearance4.Image = Global.Zuken.E3.HarnessAnalyzer.My.Resources.Resources.Refresh
            Appearance4.ImageHAlign = Infragistics.Win.HAlign.Center
            Appearance4.ImageVAlign = Infragistics.Win.VAlign.Middle
            Me.btnRefresh.Appearance = Appearance4
            Me.btnRefresh.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton
            Me.btnRefresh.CornerRadius = CType(34US, UShort)
            resources.ApplyResources(Me.btnRefresh, "btnRefresh")
            Me.btnRefresh.ImageSize = New System.Drawing.Size(28, 28)
            Me.btnRefresh.Name = "btnRefresh"
            Me.btnRefresh.ShowFocusRect = False
            Me.btnRefresh.ShowOutline = False
            resources.ApplyResources(UltraToolTipInfo4, "UltraToolTipInfo4")
            Me.UltraToolTipManager1.SetUltraToolTip(Me.btnRefresh, UltraToolTipInfo4)
            Me.btnRefresh.UseOsThemes = Infragistics.Win.DefaultableBoolean.[False]
            '
            'ProgressBar
            '
            resources.ApplyResources(Me.ProgressBar, "ProgressBar")
            Me.ProgressBar.BackColor = System.Drawing.Color.White
            Me.ProgressBar.ForeColor = System.Drawing.Color.DimGray
            Me.ProgressBar.FormatValue = "{0} %"
            Me.ProgressBar.LineColor = System.Drawing.Color.Silver
            Me.ProgressBar.Maximum = CType(100, Long)
            Me.ProgressBar.Name = "ProgressBar"
            Me.ProgressBar.ProgressColor1 = System.Drawing.Color.LightGray
            Me.ProgressBar.ProgressColor2 = System.Drawing.Color.LightGray
            Me.ProgressBar.ProgressShape = ProgressBarCircularEx._ProgressShape.Flat
            Me.ProgressBar.Value = CType(57, Long)
            '
            'UltraToolTipManager1
            '
            Me.UltraToolTipManager1.ContainingControl = Me
            Me.UltraToolTipManager1.DisplayStyle = Infragistics.Win.ToolTipDisplayStyle.Standard
            '
            'ViewControl
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.UltraPanel1)
            Me.Name = "ViewControl"
            resources.ApplyResources(UltraToolTipInfo1, "UltraToolTipInfo1")
            Me.UltraToolTipManager1.SetUltraToolTip(Me, UltraToolTipInfo1)
            Me.UltraPanel1.ClientArea.ResumeLayout(False)
            Me.UltraPanel1.ClientArea.PerformLayout()
            Me.UltraPanel1.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents UltraPanel1 As Infragistics.Win.Misc.UltraPanel
        Friend WithEvents UltraToolTipManager1 As Infragistics.Win.UltraWinToolTip.UltraToolTipManager
        Friend WithEvents btnRefresh As Zuken.E3.HarnessAnalyzer.Schematics.Controls.RoundedButton
        Friend WithEvents btnReset As Zuken.E3.HarnessAnalyzer.Schematics.Controls.RoundedButton
        Friend WithEvents ProgressBar As ProgressBarCircularEx
        Friend WithEvents btnSyncSelection As Zuken.E3.HarnessAnalyzer.Schematics.Controls.RoundedButton
        Friend WithEvents lblNoEntities As System.Windows.Forms.Label

    End Class

End Namespace