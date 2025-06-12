<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ProtectionsForm
    Inherits AnalysisForm

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ProtectionsForm))
        Dim UltraListViewSubItemColumn1 As Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn = New Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("abbreviation")
        Dim UltraListViewSubItemColumn2 As Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn = New Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("type")
        Dim UltraListViewSubItemColumn3 As Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn = New Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("description")
        Me.ugbProtections = New Infragistics.Win.Misc.UltraGroupBox()
        Me.ulvProtections = New Infragistics.Win.UltraWinListView.UltraListView()
        Me.btnView = New Infragistics.Win.Misc.UltraButton()
        Me.btnCancel = New Infragistics.Win.Misc.UltraButton()
        CType(Me.ugbProtections, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ugbProtections.SuspendLayout()
        CType(Me.ulvProtections, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ugbProtections
        '
        resources.ApplyResources(Me.ugbProtections, "ugbProtections")
        Me.ugbProtections.Controls.Add(Me.ulvProtections)
        Me.ugbProtections.Name = "ugbProtections"
        '
        'ulvProtections
        '
        resources.ApplyResources(Me.ulvProtections, "ulvProtections")
        Me.ulvProtections.ItemSettings.HideSelection = False
        Me.ulvProtections.ItemSettings.SelectionType = Infragistics.Win.UltraWinListView.SelectionType.[Single]
        Me.ulvProtections.MainColumn.AllowSorting = Infragistics.Win.DefaultableBoolean.[True]
        Me.ulvProtections.MainColumn.AutoSizeMode = CType((Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.Header Or Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.AllItems), Infragistics.Win.UltraWinListView.ColumnAutoSizeMode)
        Me.ulvProtections.MainColumn.Key = "partNumber"
        Me.ulvProtections.MainColumn.ShowSortIndicators = Infragistics.Win.DefaultableBoolean.[True]
        Me.ulvProtections.MainColumn.Sorting = Infragistics.Win.UltraWinListView.Sorting.Ascending
        Me.ulvProtections.MainColumn.Text = resources.GetString("ulvProtections.MainColumn.Text")
        Me.ulvProtections.MainColumn.VisiblePositionInDetailsView = 0
        Me.ulvProtections.MainColumn.Width = 80
        Me.ulvProtections.Name = "ulvProtections"
        UltraListViewSubItemColumn1.AutoSizeMode = CType((Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.Header Or Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.AllItems), Infragistics.Win.UltraWinListView.ColumnAutoSizeMode)
        UltraListViewSubItemColumn1.Key = "abbreviation"
        resources.ApplyResources(UltraListViewSubItemColumn1, "UltraListViewSubItemColumn1")
        UltraListViewSubItemColumn1.VisiblePositionInDetailsView = 1
        UltraListViewSubItemColumn1.VisiblePositionInTilesView = 1
        UltraListViewSubItemColumn1.Width = 80
        UltraListViewSubItemColumn2.AutoSizeMode = CType((Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.Header Or Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.AllItems), Infragistics.Win.UltraWinListView.ColumnAutoSizeMode)
        UltraListViewSubItemColumn2.Key = "type"
        resources.ApplyResources(UltraListViewSubItemColumn2, "UltraListViewSubItemColumn2")
        UltraListViewSubItemColumn2.VisiblePositionInDetailsView = 2
        UltraListViewSubItemColumn2.VisiblePositionInTilesView = 2
        UltraListViewSubItemColumn3.AutoSizeMode = CType((Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.Header Or Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.AllItems), Infragistics.Win.UltraWinListView.ColumnAutoSizeMode)
        UltraListViewSubItemColumn3.Key = "description"
        resources.ApplyResources(UltraListViewSubItemColumn3, "UltraListViewSubItemColumn3")
        UltraListViewSubItemColumn3.VisiblePositionInDetailsView = 3
        UltraListViewSubItemColumn3.VisiblePositionInTilesView = 3
        Me.ulvProtections.SubItemColumns.AddRange(New Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn() {UltraListViewSubItemColumn1, UltraListViewSubItemColumn2, UltraListViewSubItemColumn3})
        Me.ulvProtections.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.Details
        Me.ulvProtections.ViewSettingsDetails.AutoFitColumns = Infragistics.Win.UltraWinListView.AutoFitColumns.ResizeAllColumns
        Me.ulvProtections.ViewSettingsDetails.ColumnAutoSizeMode = CType((Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.Header Or Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.VisibleItems), Infragistics.Win.UltraWinListView.ColumnAutoSizeMode)
        Me.ulvProtections.ViewSettingsDetails.FullRowSelect = True
        Me.ulvProtections.ViewSettingsDetails.ImageSize = New System.Drawing.Size(0, 0)
        '
        'btnView
        '
        resources.ApplyResources(Me.btnView, "btnView")
        Me.btnView.Name = "btnView"
        '
        'btnCancel
        '
        Me.btnCancel.AccessibleRole = System.Windows.Forms.AccessibleRole.None
        resources.ApplyResources(Me.btnCancel, "btnCancel")
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Name = "btnCancel"
        '
        'ProtectionsForm
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.BackColor = System.Drawing.Color.White
        resources.ApplyResources(Me, "$this")
        Me.Controls.Add(Me.btnView)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.ugbProtections)
        Me.Name = "ProtectionsForm"
        CType(Me.ugbProtections, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ugbProtections.ResumeLayout(False)
        CType(Me.ulvProtections, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents ugbProtections As Infragistics.Win.Misc.UltraGroupBox
    Friend WithEvents ulvProtections As Infragistics.Win.UltraWinListView.UltraListView
    Friend WithEvents btnView As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnCancel As Infragistics.Win.Misc.UltraButton
End Class
