<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class DifferentModulesDialog
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DifferentModulesDialog))
        Dim UltraListViewItem1 As Infragistics.Win.UltraWinListView.UltraListViewItem = New Infragistics.Win.UltraWinListView.UltraListViewItem(Nothing, New Infragistics.Win.UltraWinListView.UltraListViewSubItem() {New Infragistics.Win.UltraWinListView.UltraListViewSubItem(True, Nothing), New Infragistics.Win.UltraWinListView.UltraListViewSubItem(False, Nothing)}, Nothing)
        Dim UltraListViewItem2 As Infragistics.Win.UltraWinListView.UltraListViewItem = New Infragistics.Win.UltraWinListView.UltraListViewItem(Nothing, New Infragistics.Win.UltraWinListView.UltraListViewSubItem() {New Infragistics.Win.UltraWinListView.UltraListViewSubItem(False, Nothing), New Infragistics.Win.UltraWinListView.UltraListViewSubItem(False, Nothing)}, Nothing)
        Dim UltraListViewItem3 As Infragistics.Win.UltraWinListView.UltraListViewItem = New Infragistics.Win.UltraWinListView.UltraListViewItem(Nothing, New Infragistics.Win.UltraWinListView.UltraListViewSubItem() {New Infragistics.Win.UltraWinListView.UltraListViewSubItem(Nothing, Nothing), New Infragistics.Win.UltraWinListView.UltraListViewSubItem(False, Nothing)}, Nothing)
        Dim UltraListViewItem4 As Infragistics.Win.UltraWinListView.UltraListViewItem = New Infragistics.Win.UltraWinListView.UltraListViewItem(Nothing, New Infragistics.Win.UltraWinListView.UltraListViewSubItem() {New Infragistics.Win.UltraWinListView.UltraListViewSubItem(Nothing, Nothing), New Infragistics.Win.UltraWinListView.UltraListViewSubItem(False, Nothing)}, Nothing)
        Dim UltraListViewSubItemColumn1 As Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn = New Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("DocumentColumn")
        Dim Appearance1 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim UltraListViewSubItemColumn2 As Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn = New Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("SettingColumn")
        Dim Appearance2 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Me.UltraPanel1 = New Infragistics.Win.Misc.UltraPanel()
        Me.Recent_Button = New System.Windows.Forms.Button()
        Me.Cancel_Button = New System.Windows.Forms.Button()
        Me.Current_Button = New System.Windows.Forms.Button()
        Me.lblText = New Infragistics.Win.Misc.UltraLabel()
        Me.UltraPanel3 = New Infragistics.Win.Misc.UltraPanel()
        Me.ulvDiffModules = New Infragistics.Win.UltraWinListView.UltraListView()
        Me.UltraPanel1.ClientArea.SuspendLayout()
        Me.UltraPanel1.SuspendLayout()
        Me.UltraPanel3.ClientArea.SuspendLayout()
        Me.UltraPanel3.SuspendLayout()
        CType(Me.ulvDiffModules, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'UltraPanel1
        '
        resources.ApplyResources(Me.UltraPanel1, "UltraPanel1")
        '
        'UltraPanel1.ClientArea
        '
        resources.ApplyResources(Me.UltraPanel1.ClientArea, "UltraPanel1.ClientArea")
        Me.UltraPanel1.ClientArea.Controls.Add(Me.Recent_Button)
        Me.UltraPanel1.ClientArea.Controls.Add(Me.Cancel_Button)
        Me.UltraPanel1.ClientArea.Controls.Add(Me.Current_Button)
        Me.UltraPanel1.ClientArea.Controls.Add(Me.lblText)
        Me.UltraPanel1.ClientArea.Controls.Add(Me.UltraPanel3)
        Me.UltraPanel1.Name = "UltraPanel1"
        '
        'Recent_Button
        '
        resources.ApplyResources(Me.Recent_Button, "Recent_Button")
        Me.Recent_Button.DialogResult = System.Windows.Forms.DialogResult.Yes
        Me.Recent_Button.Name = "Recent_Button"
        '
        'Cancel_Button
        '
        resources.ApplyResources(Me.Cancel_Button, "Cancel_Button")
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Name = "Cancel_Button"
        '
        'Current_Button
        '
        resources.ApplyResources(Me.Current_Button, "Current_Button")
        Me.Current_Button.DialogResult = System.Windows.Forms.DialogResult.No
        Me.Current_Button.Name = "Current_Button"
        '
        'lblText
        '
        resources.ApplyResources(Me.lblText, "lblText")
        Me.lblText.Name = "lblText"
        '
        'UltraPanel3
        '
        resources.ApplyResources(Me.UltraPanel3, "UltraPanel3")
        '
        'UltraPanel3.ClientArea
        '
        resources.ApplyResources(Me.UltraPanel3.ClientArea, "UltraPanel3.ClientArea")
        Me.UltraPanel3.ClientArea.Controls.Add(Me.ulvDiffModules)
        Me.UltraPanel3.Name = "UltraPanel3"
        '
        'ulvDiffModules
        '
        resources.ApplyResources(Me.ulvDiffModules, "ulvDiffModules")
        Me.ulvDiffModules.Items.AddRange(New Infragistics.Win.UltraWinListView.UltraListViewItem() {UltraListViewItem1, UltraListViewItem2, UltraListViewItem3, UltraListViewItem4})
        Me.ulvDiffModules.ItemSettings.SelectionType = Infragistics.Win.UltraWinListView.SelectionType.[Single]
        Me.ulvDiffModules.MainColumn.AllowSorting = Infragistics.Win.DefaultableBoolean.[True]
        Me.ulvDiffModules.MainColumn.AutoSizeMode = CType((Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.Header Or Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.AllItems), Infragistics.Win.UltraWinListView.ColumnAutoSizeMode)
        Me.ulvDiffModules.MainColumn.DataType = GetType(String)
        Me.ulvDiffModules.MainColumn.ShowSortIndicators = Infragistics.Win.DefaultableBoolean.[True]
        Me.ulvDiffModules.MainColumn.Sorting = Infragistics.Win.UltraWinListView.Sorting.Ascending
        Me.ulvDiffModules.MainColumn.Text = resources.GetString("ulvDiffModules.MainColumn.Text")
        Me.ulvDiffModules.Name = "ulvDiffModules"
        UltraListViewSubItemColumn1.AutoSizeMode = CType((Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.Header Or Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.AllItems), Infragistics.Win.UltraWinListView.ColumnAutoSizeMode)
        UltraListViewSubItemColumn1.DataType = GetType(Boolean)
        UltraListViewSubItemColumn1.Key = "DocumentColumn"
        Appearance1.ImageHAlign = Infragistics.Win.HAlign.Center
        resources.ApplyResources(Appearance1, "Appearance1")
        UltraListViewSubItemColumn1.SubItemAppearance = Appearance1
        resources.ApplyResources(UltraListViewSubItemColumn1, "UltraListViewSubItemColumn1")
        UltraListViewSubItemColumn2.AutoSizeMode = Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.AllItems
        UltraListViewSubItemColumn2.DataType = GetType(Boolean)
        UltraListViewSubItemColumn2.Key = "SettingColumn"
        Appearance2.ImageHAlign = Infragistics.Win.HAlign.Center
        resources.ApplyResources(Appearance2, "Appearance2")
        UltraListViewSubItemColumn2.SubItemAppearance = Appearance2
        resources.ApplyResources(UltraListViewSubItemColumn2, "UltraListViewSubItemColumn2")
        Me.ulvDiffModules.SubItemColumns.AddRange(New Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn() {UltraListViewSubItemColumn1, UltraListViewSubItemColumn2})
        Me.ulvDiffModules.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.Details
        Me.ulvDiffModules.ViewSettingsDetails.AllowColumnMoving = False
        Me.ulvDiffModules.ViewSettingsDetails.AllowColumnSorting = False
        Me.ulvDiffModules.ViewSettingsDetails.AutoFitColumns = Infragistics.Win.UltraWinListView.AutoFitColumns.ResizeAllColumns
        Me.ulvDiffModules.ViewSettingsDetails.ColumnAutoSizeMode = CType((Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.Header Or Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.AllItems), Infragistics.Win.UltraWinListView.ColumnAutoSizeMode)
        Me.ulvDiffModules.ViewSettingsDetails.FullRowSelect = True
        Me.ulvDiffModules.ViewSettingsDetails.ImageSize = New System.Drawing.Size(0, 0)
        '
        'DifferentModulesDialog
        '
        Me.AcceptButton = Me.Recent_Button
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.CancelButton = Me.Cancel_Button
        Me.Controls.Add(Me.UltraPanel1)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "DifferentModulesDialog"
        Me.ShowInTaskbar = False
        Me.UltraPanel1.ClientArea.ResumeLayout(False)
        Me.UltraPanel1.ClientArea.PerformLayout()
        Me.UltraPanel1.ResumeLayout(False)
        Me.UltraPanel3.ClientArea.ResumeLayout(False)
        Me.UltraPanel3.ResumeLayout(False)
        CType(Me.ulvDiffModules, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Recent_Button As System.Windows.Forms.Button
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents Current_Button As Button
    Friend WithEvents UltraPanel1 As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents UltraPanel3 As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents ulvDiffModules As Infragistics.Win.UltraWinListView.UltraListView
    Friend WithEvents lblText As Infragistics.Win.Misc.UltraLabel
End Class
