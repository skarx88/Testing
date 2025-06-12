<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class OffsetVectorPreviewForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(OffsetVectorPreviewForm))
        Dim Appearance1 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance2 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance3 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance4 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Me.vDraw = New VectorDraw.Professional.Control.VectorDrawBaseControl()
        Me.btnClose = New Infragistics.Win.Misc.UltraButton()
        Me.lblMostFreqOffsetVector = New Infragistics.Win.Misc.UltraLabel()
        Me.uneOffsetVectorY = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
        Me.uneOffsetVectorX = New Infragistics.Win.UltraWinEditors.UltraNumericEditor()
        Me.lblInfo = New Infragistics.Win.Misc.UltraLabel()
        Me.lblDrwInfo1 = New Infragistics.Win.Misc.UltraLabel()
        Me.UltraLabel1 = New Infragistics.Win.Misc.UltraLabel()
        Me.uttmOffsetVectorPreview = New Infragistics.Win.UltraWinToolTip.UltraToolTipManager(Me.components)
        CType(Me.uneOffsetVectorY, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.uneOffsetVectorX, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'vDraw
        '
        Me.vDraw.AccessibleRole = System.Windows.Forms.AccessibleRole.Window
        Me.vDraw.AllowDrop = True
        resources.ApplyResources(Me.vDraw, "vDraw")
        Me.vDraw.Cursor = System.Windows.Forms.Cursors.Default
        Me.vDraw.DisableVdrawDxf = False
        Me.vDraw.EnableAutoGripOn = True
        Me.vDraw.Name = "vDraw"
        '
        'btnClose
        '
        resources.ApplyResources(Me.btnClose, "btnClose")
        Me.btnClose.Name = "btnClose"
        '
        'lblMostFreqOffsetVector
        '
        resources.ApplyResources(Me.lblMostFreqOffsetVector, "lblMostFreqOffsetVector")
        Me.lblMostFreqOffsetVector.Name = "lblMostFreqOffsetVector"
        '
        'uneOffsetVectorY
        '
        resources.ApplyResources(Me.uneOffsetVectorY, "uneOffsetVectorY")
        Appearance1.BackColor = System.Drawing.Color.White
        Me.uneOffsetVectorY.Appearance = Appearance1
        Me.uneOffsetVectorY.BackColor = System.Drawing.Color.White
        Me.uneOffsetVectorY.FormatString = "#0.##"
        Me.uneOffsetVectorY.MaxValue = 10000.0R
        Me.uneOffsetVectorY.MinValue = -10000.0R
        Me.uneOffsetVectorY.Name = "uneOffsetVectorY"
        Me.uneOffsetVectorY.NumericType = Infragistics.Win.UltraWinEditors.NumericType.[Double]
        Me.uneOffsetVectorY.PromptChar = Global.Microsoft.VisualBasic.ChrW(32)
        Me.uneOffsetVectorY.ReadOnly = True
        '
        'uneOffsetVectorX
        '
        resources.ApplyResources(Me.uneOffsetVectorX, "uneOffsetVectorX")
        Appearance2.BackColor = System.Drawing.Color.White
        Me.uneOffsetVectorX.Appearance = Appearance2
        Me.uneOffsetVectorX.BackColor = System.Drawing.Color.White
        Me.uneOffsetVectorX.FormatString = "#0.##"
        Me.uneOffsetVectorX.MaxValue = 10000.0R
        Me.uneOffsetVectorX.MinValue = -10000.0R
        Me.uneOffsetVectorX.Name = "uneOffsetVectorX"
        Me.uneOffsetVectorX.NumericType = Infragistics.Win.UltraWinEditors.NumericType.[Double]
        Me.uneOffsetVectorX.PromptChar = Global.Microsoft.VisualBasic.ChrW(32)
        Me.uneOffsetVectorX.ReadOnly = True
        '
        'lblInfo
        '
        resources.ApplyResources(Me.lblInfo, "lblInfo")
        Me.lblInfo.Name = "lblInfo"
        '
        'lblDrwInfo1
        '
        resources.ApplyResources(Me.lblDrwInfo1, "lblDrwInfo1")
        Appearance3.BackColor = System.Drawing.Color.White
        Appearance3.ForeColor = System.Drawing.Color.Red
        Me.lblDrwInfo1.Appearance = Appearance3
        Me.lblDrwInfo1.Name = "lblDrwInfo1"
        '
        'UltraLabel1
        '
        resources.ApplyResources(Me.UltraLabel1, "UltraLabel1")
        Appearance4.BackColor = System.Drawing.Color.White
        Appearance4.ForeColor = System.Drawing.Color.Green
        Me.UltraLabel1.Appearance = Appearance4
        Me.UltraLabel1.Name = "UltraLabel1"
        '
        'uttmOffsetVectorPreview
        '
        Me.uttmOffsetVectorPreview.ContainingControl = Me
        '
        'OffsetVectorPreviewForm
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        resources.ApplyResources(Me, "$this")
        Me.Controls.Add(Me.UltraLabel1)
        Me.Controls.Add(Me.lblDrwInfo1)
        Me.Controls.Add(Me.lblInfo)
        Me.Controls.Add(Me.uneOffsetVectorY)
        Me.Controls.Add(Me.uneOffsetVectorX)
        Me.Controls.Add(Me.lblMostFreqOffsetVector)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.vDraw)
        Me.Name = "OffsetVectorPreviewForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        CType(Me.uneOffsetVectorY, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.uneOffsetVectorX, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents vDraw As VectorDraw.Professional.Control.VectorDrawBaseControl
    Friend WithEvents btnClose As Infragistics.Win.Misc.UltraButton
    Friend WithEvents lblMostFreqOffsetVector As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents uneOffsetVectorY As Infragistics.Win.UltraWinEditors.UltraNumericEditor
    Friend WithEvents uneOffsetVectorX As Infragistics.Win.UltraWinEditors.UltraNumericEditor
    Friend WithEvents lblInfo As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents lblDrwInfo1 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents UltraLabel1 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents uttmOffsetVectorPreview As Infragistics.Win.UltraWinToolTip.UltraToolTipManager
End Class
