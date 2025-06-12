<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TooltipForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(TooltipForm))
        Dim Appearance1 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance2 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance3 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance4 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance5 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance6 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance7 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance8 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance9 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance10 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance11 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance12 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Me.udsTooltip = New Infragistics.Win.UltraWinDataSource.UltraDataSource(Me.components)
        Me.ugTooltip = New Infragistics.Win.UltraWinGrid.UltraGrid()
        CType(Me.udsTooltip, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ugTooltip, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'udsTooltip
        '
        Me.udsTooltip.AllowAdd = False
        Me.udsTooltip.AllowDelete = False
        Me.udsTooltip.ReadOnly = True
        '
        'ugTooltip
        '
        resources.ApplyResources(Me.ugTooltip, "ugTooltip")
        Appearance1.BackColor = System.Drawing.SystemColors.Window
        Appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption
        resources.ApplyResources(Appearance1, "Appearance1")
        Me.ugTooltip.DisplayLayout.Appearance = Appearance1
        Me.ugTooltip.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        Me.ugTooltip.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.[False]
        Appearance2.BackColor = System.Drawing.SystemColors.ActiveBorder
        Appearance2.BackColor2 = System.Drawing.SystemColors.ControlDark
        Appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical
        Appearance2.BorderColor = System.Drawing.SystemColors.Window
        resources.ApplyResources(Appearance2, "Appearance2")
        Me.ugTooltip.DisplayLayout.GroupByBox.Appearance = Appearance2
        Appearance3.ForeColor = System.Drawing.SystemColors.GrayText
        resources.ApplyResources(Appearance3, "Appearance3")
        Me.ugTooltip.DisplayLayout.GroupByBox.BandLabelAppearance = Appearance3
        Me.ugTooltip.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        Appearance4.BackColor = System.Drawing.SystemColors.ControlLightLight
        Appearance4.BackColor2 = System.Drawing.SystemColors.Control
        Appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance4.ForeColor = System.Drawing.SystemColors.GrayText
        resources.ApplyResources(Appearance4, "Appearance4")
        Me.ugTooltip.DisplayLayout.GroupByBox.PromptAppearance = Appearance4
        Me.ugTooltip.DisplayLayout.MaxColScrollRegions = 1
        Me.ugTooltip.DisplayLayout.MaxRowScrollRegions = 1
        Appearance5.BackColor = System.Drawing.SystemColors.Window
        Appearance5.ForeColor = System.Drawing.SystemColors.ControlText
        resources.ApplyResources(Appearance5, "Appearance5")
        Me.ugTooltip.DisplayLayout.Override.ActiveCellAppearance = Appearance5
        Appearance6.BackColor = System.Drawing.SystemColors.Highlight
        Appearance6.ForeColor = System.Drawing.SystemColors.HighlightText
        resources.ApplyResources(Appearance6, "Appearance6")
        Me.ugTooltip.DisplayLayout.Override.ActiveRowAppearance = Appearance6
        Me.ugTooltip.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted
        Me.ugTooltip.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted
        Appearance7.BackColor = System.Drawing.SystemColors.Window
        resources.ApplyResources(Appearance7, "Appearance7")
        Me.ugTooltip.DisplayLayout.Override.CardAreaAppearance = Appearance7
        Appearance8.BorderColor = System.Drawing.Color.Silver
        resources.ApplyResources(Appearance8, "Appearance8")
        Appearance8.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter
        Me.ugTooltip.DisplayLayout.Override.CellAppearance = Appearance8
        Me.ugTooltip.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText
        Me.ugTooltip.DisplayLayout.Override.CellPadding = 0
        Appearance9.BackColor = System.Drawing.SystemColors.Control
        Appearance9.BackColor2 = System.Drawing.SystemColors.ControlDark
        Appearance9.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element
        Appearance9.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance9.BorderColor = System.Drawing.SystemColors.Window
        resources.ApplyResources(Appearance9, "Appearance9")
        Me.ugTooltip.DisplayLayout.Override.GroupByRowAppearance = Appearance9
        resources.ApplyResources(Appearance10, "Appearance10")
        Me.ugTooltip.DisplayLayout.Override.HeaderAppearance = Appearance10
        Me.ugTooltip.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti
        Me.ugTooltip.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand
        Appearance11.BackColor = System.Drawing.SystemColors.Window
        Appearance11.BorderColor = System.Drawing.Color.Silver
        resources.ApplyResources(Appearance11, "Appearance11")
        Me.ugTooltip.DisplayLayout.Override.RowAppearance = Appearance11
        Me.ugTooltip.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.[False]
        Appearance12.BackColor = System.Drawing.SystemColors.ControlLight
        resources.ApplyResources(Appearance12, "Appearance12")
        Me.ugTooltip.DisplayLayout.Override.TemplateAddRowAppearance = Appearance12
        Me.ugTooltip.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill
        Me.ugTooltip.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate
        Me.ugTooltip.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy
        Me.ugTooltip.Name = "ugTooltip"
        '
        'TooltipForm
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.Controls.Add(Me.ugTooltip)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "TooltipForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.TopMost = True
        CType(Me.udsTooltip, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ugTooltip, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents udsTooltip As Infragistics.Win.UltraWinDataSource.UltraDataSource
    Friend WithEvents ugTooltip As Infragistics.Win.UltraWinGrid.UltraGrid
End Class
