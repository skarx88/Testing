<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DetailInformationForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DetailInformationForm))
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
        Me.udsDetailInformation = New Infragistics.Win.UltraWinDataSource.UltraDataSource(Me.components)
        Me.ugeeDetailInformation = New Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter(Me.components)
        Me.btnExport = New Infragistics.Win.Misc.UltraButton()
        Me.btnClose = New Infragistics.Win.Misc.UltraButton()
        Me.btnApply = New Infragistics.Win.Misc.UltraButton()
        Me.ugDetailInformation = New Infragistics.Win.UltraWinGrid.UltraGrid()
        CType(Me.udsDetailInformation, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ugDetailInformation, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'udsDetailInformation
        '
        Me.udsDetailInformation.AllowAdd = False
        Me.udsDetailInformation.AllowDelete = False
        Me.udsDetailInformation.ReadOnly = True
        '
        'ugeeDetailInformation
        '
        '
        'btnExport
        '
        resources.ApplyResources(Me.btnExport, "btnExport")
        Me.btnExport.Name = "btnExport"
        '
        'btnClose
        '
        resources.ApplyResources(Me.btnClose, "btnClose")
        Me.btnClose.Name = "btnClose"
        '
        'btnApply
        '
        resources.ApplyResources(Me.btnApply, "btnApply")
        Me.btnApply.Name = "btnApply"
        '
        'ugDetailInformation
        '
        resources.ApplyResources(Me.ugDetailInformation, "ugDetailInformation")
        Appearance1.BackColor = System.Drawing.SystemColors.Window
        Appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption
        resources.ApplyResources(Appearance1, "Appearance1")
        Me.ugDetailInformation.DisplayLayout.Appearance = Appearance1
        Me.ugDetailInformation.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        Me.ugDetailInformation.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.[False]
        Appearance2.BackColor = System.Drawing.SystemColors.ActiveBorder
        Appearance2.BackColor2 = System.Drawing.SystemColors.ControlDark
        Appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical
        Appearance2.BorderColor = System.Drawing.SystemColors.Window
        resources.ApplyResources(Appearance2, "Appearance2")
        Me.ugDetailInformation.DisplayLayout.GroupByBox.Appearance = Appearance2
        Appearance3.ForeColor = System.Drawing.SystemColors.GrayText
        resources.ApplyResources(Appearance3, "Appearance3")
        Me.ugDetailInformation.DisplayLayout.GroupByBox.BandLabelAppearance = Appearance3
        Me.ugDetailInformation.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        Appearance4.BackColor = System.Drawing.SystemColors.ControlLightLight
        Appearance4.BackColor2 = System.Drawing.SystemColors.Control
        Appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance4.ForeColor = System.Drawing.SystemColors.GrayText
        resources.ApplyResources(Appearance4, "Appearance4")
        Me.ugDetailInformation.DisplayLayout.GroupByBox.PromptAppearance = Appearance4
        Me.ugDetailInformation.DisplayLayout.MaxColScrollRegions = 1
        Me.ugDetailInformation.DisplayLayout.MaxRowScrollRegions = 1
        Appearance5.BackColor = System.Drawing.SystemColors.Window
        Appearance5.ForeColor = System.Drawing.SystemColors.ControlText
        resources.ApplyResources(Appearance5, "Appearance5")
        Me.ugDetailInformation.DisplayLayout.Override.ActiveCellAppearance = Appearance5
        Appearance6.BackColor = System.Drawing.SystemColors.Highlight
        Appearance6.ForeColor = System.Drawing.SystemColors.HighlightText
        resources.ApplyResources(Appearance6, "Appearance6")
        Me.ugDetailInformation.DisplayLayout.Override.ActiveRowAppearance = Appearance6
        Me.ugDetailInformation.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted
        Me.ugDetailInformation.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted
        Appearance7.BackColor = System.Drawing.SystemColors.Window
        resources.ApplyResources(Appearance7, "Appearance7")
        Me.ugDetailInformation.DisplayLayout.Override.CardAreaAppearance = Appearance7
        Appearance8.BorderColor = System.Drawing.Color.Silver
        resources.ApplyResources(Appearance8, "Appearance8")
        Appearance8.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter
        Me.ugDetailInformation.DisplayLayout.Override.CellAppearance = Appearance8
        Me.ugDetailInformation.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText
        Me.ugDetailInformation.DisplayLayout.Override.CellPadding = 0
        Appearance9.BackColor = System.Drawing.SystemColors.Control
        Appearance9.BackColor2 = System.Drawing.SystemColors.ControlDark
        Appearance9.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element
        Appearance9.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance9.BorderColor = System.Drawing.SystemColors.Window
        resources.ApplyResources(Appearance9, "Appearance9")
        Me.ugDetailInformation.DisplayLayout.Override.GroupByRowAppearance = Appearance9
        resources.ApplyResources(Appearance10, "Appearance10")
        Me.ugDetailInformation.DisplayLayout.Override.HeaderAppearance = Appearance10
        Me.ugDetailInformation.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti
        Me.ugDetailInformation.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand
        Appearance11.BackColor = System.Drawing.SystemColors.Window
        Appearance11.BorderColor = System.Drawing.Color.Silver
        resources.ApplyResources(Appearance11, "Appearance11")
        Me.ugDetailInformation.DisplayLayout.Override.RowAppearance = Appearance11
        Me.ugDetailInformation.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.[False]
        Appearance12.BackColor = System.Drawing.SystemColors.ControlLight
        resources.ApplyResources(Appearance12, "Appearance12")
        Me.ugDetailInformation.DisplayLayout.Override.TemplateAddRowAppearance = Appearance12
        Me.ugDetailInformation.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill
        Me.ugDetailInformation.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate
        Me.ugDetailInformation.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy
        Me.ugDetailInformation.Name = "ugDetailInformation"
        '
        'DetailInformationForm
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.Controls.Add(Me.ugDetailInformation)
        Me.Controls.Add(Me.btnApply)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.btnExport)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "DetailInformationForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        CType(Me.udsDetailInformation, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ugDetailInformation, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents udsDetailInformation As Infragistics.Win.UltraWinDataSource.UltraDataSource
    Private WithEvents ugeeDetailInformation As Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter
    Private WithEvents btnExport As Infragistics.Win.Misc.UltraButton
    Private WithEvents btnClose As Infragistics.Win.Misc.UltraButton
    Private WithEvents btnApply As Infragistics.Win.Misc.UltraButton
    Friend WithEvents ugDetailInformation As Infragistics.Win.UltraWinGrid.UltraGrid
End Class
