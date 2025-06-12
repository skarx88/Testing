<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class RedliningGroupsForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(RedliningGroupsForm))
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
        Me.panBottom = New Infragistics.Win.Misc.UltraPanel()
        Me.btnDelete = New Infragistics.Win.Misc.UltraButton()
        Me.btnAddNew = New Infragistics.Win.Misc.UltraButton()
        Me.btnOK = New Infragistics.Win.Misc.UltraButton()
        Me.btnCancel = New Infragistics.Win.Misc.UltraButton()
        Me.panBackground = New Infragistics.Win.Misc.UltraPanel()
        Me.ugRedliningGroups = New Infragistics.Win.UltraWinGrid.UltraGrid()
        Me.panBottom.ClientArea.SuspendLayout()
        Me.panBottom.SuspendLayout()
        Me.panBackground.ClientArea.SuspendLayout()
        Me.panBackground.SuspendLayout()
        CType(Me.ugRedliningGroups, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'panBottom
        '
        '
        'panBottom.ClientArea
        '
        Me.panBottom.ClientArea.Controls.Add(Me.btnDelete)
        Me.panBottom.ClientArea.Controls.Add(Me.btnAddNew)
        Me.panBottom.ClientArea.Controls.Add(Me.btnOK)
        Me.panBottom.ClientArea.Controls.Add(Me.btnCancel)
        resources.ApplyResources(Me.panBottom, "panBottom")
        Me.panBottom.Name = "panBottom"
        '
        'btnDelete
        '
        resources.ApplyResources(Me.btnDelete, "btnDelete")
        Me.btnDelete.Name = "btnDelete"
        '
        'btnAddNew
        '
        resources.ApplyResources(Me.btnAddNew, "btnAddNew")
        Me.btnAddNew.Name = "btnAddNew"
        '
        'btnOK
        '
        resources.ApplyResources(Me.btnOK, "btnOK")
        Me.btnOK.Name = "btnOK"
        '
        'btnCancel
        '
        resources.ApplyResources(Me.btnCancel, "btnCancel")
        Me.btnCancel.Name = "btnCancel"
        '
        'panBackground
        '
        '
        'panBackground.ClientArea
        '
        Me.panBackground.ClientArea.Controls.Add(Me.ugRedliningGroups)
        resources.ApplyResources(Me.panBackground, "panBackground")
        Me.panBackground.Name = "panBackground"
        '
        'ugRedliningGroups
        '
        Appearance1.BackColor = System.Drawing.SystemColors.Window
        Appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption
        Me.ugRedliningGroups.DisplayLayout.Appearance = Appearance1
        Me.ugRedliningGroups.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        Me.ugRedliningGroups.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.[False]
        Appearance2.BackColor = System.Drawing.SystemColors.ActiveBorder
        Appearance2.BackColor2 = System.Drawing.SystemColors.ControlDark
        Appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical
        Appearance2.BorderColor = System.Drawing.SystemColors.Window
        Me.ugRedliningGroups.DisplayLayout.GroupByBox.Appearance = Appearance2
        Appearance3.ForeColor = System.Drawing.SystemColors.GrayText
        Me.ugRedliningGroups.DisplayLayout.GroupByBox.BandLabelAppearance = Appearance3
        Me.ugRedliningGroups.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        Appearance4.BackColor = System.Drawing.SystemColors.ControlLightLight
        Appearance4.BackColor2 = System.Drawing.SystemColors.Control
        Appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance4.ForeColor = System.Drawing.SystemColors.GrayText
        Me.ugRedliningGroups.DisplayLayout.GroupByBox.PromptAppearance = Appearance4
        Me.ugRedliningGroups.DisplayLayout.MaxColScrollRegions = 1
        Me.ugRedliningGroups.DisplayLayout.MaxRowScrollRegions = 1
        Appearance5.BackColor = System.Drawing.SystemColors.Window
        Appearance5.ForeColor = System.Drawing.SystemColors.ControlText
        Me.ugRedliningGroups.DisplayLayout.Override.ActiveCellAppearance = Appearance5
        Appearance6.BackColor = System.Drawing.SystemColors.Highlight
        Appearance6.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.ugRedliningGroups.DisplayLayout.Override.ActiveRowAppearance = Appearance6
        Me.ugRedliningGroups.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted
        Me.ugRedliningGroups.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted
        Appearance7.BackColor = System.Drawing.SystemColors.Window
        Me.ugRedliningGroups.DisplayLayout.Override.CardAreaAppearance = Appearance7
        Appearance8.BorderColor = System.Drawing.Color.Silver
        Appearance8.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter
        Me.ugRedliningGroups.DisplayLayout.Override.CellAppearance = Appearance8
        Me.ugRedliningGroups.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText
        Me.ugRedliningGroups.DisplayLayout.Override.CellPadding = 0
        Appearance9.BackColor = System.Drawing.SystemColors.Control
        Appearance9.BackColor2 = System.Drawing.SystemColors.ControlDark
        Appearance9.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element
        Appearance9.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance9.BorderColor = System.Drawing.SystemColors.Window
        Me.ugRedliningGroups.DisplayLayout.Override.GroupByRowAppearance = Appearance9
        Me.ugRedliningGroups.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti
        Me.ugRedliningGroups.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand
        Appearance10.BackColor = System.Drawing.SystemColors.Window
        Appearance10.BorderColor = System.Drawing.Color.Silver
        Me.ugRedliningGroups.DisplayLayout.Override.RowAppearance = Appearance10
        Me.ugRedliningGroups.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.[False]
        Appearance11.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ugRedliningGroups.DisplayLayout.Override.TemplateAddRowAppearance = Appearance11
        Me.ugRedliningGroups.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill
        Me.ugRedliningGroups.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate
        Me.ugRedliningGroups.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy
        resources.ApplyResources(Me.ugRedliningGroups, "ugRedliningGroups")
        Me.ugRedliningGroups.Name = "ugRedliningGroups"
        '
        'RedliningGroupsForm
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        resources.ApplyResources(Me, "$this")
        Me.Controls.Add(Me.panBackground)
        Me.Controls.Add(Me.panBottom)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "RedliningGroupsForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.panBottom.ClientArea.ResumeLayout(False)
        Me.panBottom.ResumeLayout(False)
        Me.panBackground.ClientArea.ResumeLayout(False)
        Me.panBackground.ResumeLayout(False)
        CType(Me.ugRedliningGroups, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents panBottom As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents panBackground As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents btnCancel As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnDelete As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnAddNew As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnOK As Infragistics.Win.Misc.UltraButton
    Friend WithEvents ugRedliningGroups As Infragistics.Win.UltraWinGrid.UltraGrid
End Class
