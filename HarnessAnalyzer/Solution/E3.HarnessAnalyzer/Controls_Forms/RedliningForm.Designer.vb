<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class RedliningForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(RedliningForm))
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
        ugRedlinings = New Infragistics.Win.UltraWinGrid.UltraGrid()
        btnOK = New Infragistics.Win.Misc.UltraButton()
        btnAddNew = New Infragistics.Win.Misc.UltraButton()
        btnDelete = New Infragistics.Win.Misc.UltraButton()
        btnCommentBulkUpdate = New Infragistics.Win.Misc.UltraButton()
        btnCancel = New Infragistics.Win.Misc.UltraButton()
        CType(ugRedlinings, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' ugRedlinings
        ' 
        resources.ApplyResources(ugRedlinings, "ugRedlinings")
        Appearance1.BackColor = SystemColors.Window
        Appearance1.BorderColor = SystemColors.InactiveCaption
        resources.ApplyResources(Appearance1, "Appearance1")
        resources.ApplyResources(Appearance1.FontData, "Appearance1.FontData")
        Appearance1.ForceApplyResources = "FontData|"
        ugRedlinings.DisplayLayout.Appearance = Appearance1
        ugRedlinings.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        ugRedlinings.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False
        Appearance2.BackColor = SystemColors.ActiveBorder
        Appearance2.BackColor2 = SystemColors.ControlDark
        Appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical
        Appearance2.BorderColor = SystemColors.Window
        resources.ApplyResources(Appearance2, "Appearance2")
        resources.ApplyResources(Appearance2.FontData, "Appearance2.FontData")
        Appearance2.ForceApplyResources = "FontData|"
        ugRedlinings.DisplayLayout.GroupByBox.Appearance = Appearance2
        Appearance3.ForeColor = SystemColors.GrayText
        resources.ApplyResources(Appearance3, "Appearance3")
        resources.ApplyResources(Appearance3.FontData, "Appearance3.FontData")
        Appearance3.ForceApplyResources = "FontData|"
        ugRedlinings.DisplayLayout.GroupByBox.BandLabelAppearance = Appearance3
        ugRedlinings.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        Appearance4.BackColor = SystemColors.ControlLightLight
        Appearance4.BackColor2 = SystemColors.Control
        Appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance4.ForeColor = SystemColors.GrayText
        resources.ApplyResources(Appearance4, "Appearance4")
        resources.ApplyResources(Appearance4.FontData, "Appearance4.FontData")
        Appearance4.ForceApplyResources = "FontData|"
        ugRedlinings.DisplayLayout.GroupByBox.PromptAppearance = Appearance4
        ugRedlinings.DisplayLayout.MaxColScrollRegions = 1
        ugRedlinings.DisplayLayout.MaxRowScrollRegions = 1
        Appearance5.BackColor = SystemColors.Window
        Appearance5.ForeColor = SystemColors.ControlText
        resources.ApplyResources(Appearance5, "Appearance5")
        resources.ApplyResources(Appearance5.FontData, "Appearance5.FontData")
        Appearance5.ForceApplyResources = "FontData|"
        ugRedlinings.DisplayLayout.Override.ActiveCellAppearance = Appearance5
        Appearance6.BackColor = SystemColors.Highlight
        Appearance6.ForeColor = SystemColors.HighlightText
        resources.ApplyResources(Appearance6, "Appearance6")
        resources.ApplyResources(Appearance6.FontData, "Appearance6.FontData")
        Appearance6.ForceApplyResources = "FontData|"
        ugRedlinings.DisplayLayout.Override.ActiveRowAppearance = Appearance6
        ugRedlinings.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted
        ugRedlinings.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted
        Appearance7.BackColor = SystemColors.Window
        resources.ApplyResources(Appearance7, "Appearance7")
        resources.ApplyResources(Appearance7.FontData, "Appearance7.FontData")
        Appearance7.ForceApplyResources = "FontData|"
        ugRedlinings.DisplayLayout.Override.CardAreaAppearance = Appearance7
        Appearance8.BorderColor = Color.Silver
        resources.ApplyResources(Appearance8, "Appearance8")
        Appearance8.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter
        resources.ApplyResources(Appearance8.FontData, "Appearance8.FontData")
        Appearance8.ForceApplyResources = "FontData|"
        ugRedlinings.DisplayLayout.Override.CellAppearance = Appearance8
        ugRedlinings.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText
        ugRedlinings.DisplayLayout.Override.CellPadding = 0
        Appearance9.BackColor = SystemColors.Control
        Appearance9.BackColor2 = SystemColors.ControlDark
        Appearance9.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element
        Appearance9.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance9.BorderColor = SystemColors.Window
        resources.ApplyResources(Appearance9, "Appearance9")
        resources.ApplyResources(Appearance9.FontData, "Appearance9.FontData")
        Appearance9.ForceApplyResources = "FontData|"
        ugRedlinings.DisplayLayout.Override.GroupByRowAppearance = Appearance9
        resources.ApplyResources(Appearance10, "Appearance10")
        resources.ApplyResources(Appearance10.FontData, "Appearance10.FontData")
        Appearance10.ForceApplyResources = "FontData|"
        ugRedlinings.DisplayLayout.Override.HeaderAppearance = Appearance10
        ugRedlinings.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti
        ugRedlinings.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand
        Appearance11.BackColor = SystemColors.Window
        Appearance11.BorderColor = Color.Silver
        resources.ApplyResources(Appearance11, "Appearance11")
        resources.ApplyResources(Appearance11.FontData, "Appearance11.FontData")
        Appearance11.ForceApplyResources = "FontData|"
        ugRedlinings.DisplayLayout.Override.RowAppearance = Appearance11
        ugRedlinings.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False
        Appearance12.BackColor = SystemColors.ControlLight
        resources.ApplyResources(Appearance12, "Appearance12")
        resources.ApplyResources(Appearance12.FontData, "Appearance12.FontData")
        Appearance12.ForceApplyResources = "FontData|"
        ugRedlinings.DisplayLayout.Override.TemplateAddRowAppearance = Appearance12
        ugRedlinings.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill
        ugRedlinings.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate
        ugRedlinings.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy
        ugRedlinings.Name = "ugRedlinings"
        ' 
        ' btnOK
        ' 
        resources.ApplyResources(btnOK, "btnOK")
        btnOK.Name = "btnOK"
        ' 
        ' btnAddNew
        ' 
        resources.ApplyResources(btnAddNew, "btnAddNew")
        btnAddNew.Name = "btnAddNew"
        ' 
        ' btnDelete
        ' 
        resources.ApplyResources(btnDelete, "btnDelete")
        btnDelete.Name = "btnDelete"
        ' 
        ' btnCommentBulkUpdate
        ' 
        resources.ApplyResources(btnCommentBulkUpdate, "btnCommentBulkUpdate")
        btnCommentBulkUpdate.Name = "btnCommentBulkUpdate"
        ' 
        ' btnCancel
        ' 
        resources.ApplyResources(btnCancel, "btnCancel")
        btnCancel.Name = "btnCancel"
        ' 
        ' RedliningForm
        ' 
        resources.ApplyResources(Me, "$this")
        AutoScaleMode = AutoScaleMode.Inherit
        Controls.Add(btnCancel)
        Controls.Add(btnCommentBulkUpdate)
        Controls.Add(btnDelete)
        Controls.Add(btnAddNew)
        Controls.Add(btnOK)
        Controls.Add(ugRedlinings)
        KeyPreview = True
        MinimizeBox = False
        Name = "RedliningForm"
        ShowInTaskbar = False
        CType(ugRedlinings, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)

    End Sub
    Friend WithEvents ugRedlinings As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents btnOK As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnAddNew As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnDelete As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnCommentBulkUpdate As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnCancel As Infragistics.Win.Misc.UltraButton
End Class
