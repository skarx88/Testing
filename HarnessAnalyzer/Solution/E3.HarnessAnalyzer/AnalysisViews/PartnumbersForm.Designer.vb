<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class PartNumbersForm
    Inherits AnalysisForm

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(PartNumbersForm))
        Dim UltraListViewSubItemColumn1 As Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn = New Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("description")
        Dim UltraListViewSubItemColumn2 As Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn = New Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("typeName")
        Me.ugbPartnumbers = New Infragistics.Win.Misc.UltraGroupBox()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.tbx_partnumber = New System.Windows.Forms.TextBox()
        Me.lbl_partnumber = New System.Windows.Forms.Label()
        Me.ulvPartnumbers = New Infragistics.Win.UltraWinListView.UltraListView()
        Me.btnView = New Infragistics.Win.Misc.UltraButton()
        Me.btnClose = New Infragistics.Win.Misc.UltraButton()
        CType(Me.ugbPartnumbers, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ugbPartnumbers.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        CType(Me.ulvPartnumbers, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ugbPartnumbers
        '
        resources.ApplyResources(Me.ugbPartnumbers, "ugbPartnumbers")
        Me.ugbPartnumbers.Controls.Add(Me.TableLayoutPanel1)
        Me.ugbPartnumbers.Controls.Add(Me.ulvPartnumbers)
        Me.ugbPartnumbers.Name = "ugbPartnumbers"
        '
        'TableLayoutPanel1
        '
        resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
        Me.TableLayoutPanel1.Controls.Add(Me.tbx_partnumber, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.lbl_partnumber, 0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        '
        'tbx_partnumber
        '
        resources.ApplyResources(Me.tbx_partnumber, "tbx_partnumber")
        Me.tbx_partnumber.Name = "tbx_partnumber"
        '
        'lbl_partnumber
        '
        resources.ApplyResources(Me.lbl_partnumber, "lbl_partnumber")
        Me.lbl_partnumber.Name = "lbl_partnumber"
        '
        'ulvPartnumbers
        '
        resources.ApplyResources(Me.ulvPartnumbers, "ulvPartnumbers")
        Me.ulvPartnumbers.ItemSettings.HideSelection = False
        Me.ulvPartnumbers.ItemSettings.SelectionType = Infragistics.Win.UltraWinListView.SelectionType.[Single]
        Me.ulvPartnumbers.MainColumn.AllowSorting = Infragistics.Win.DefaultableBoolean.[True]
        Me.ulvPartnumbers.MainColumn.AutoSizeMode = CType((Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.Header Or Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.AllItems), Infragistics.Win.UltraWinListView.ColumnAutoSizeMode)
        Me.ulvPartnumbers.MainColumn.Key = "partNumber"
        Me.ulvPartnumbers.MainColumn.ShowSortIndicators = Infragistics.Win.DefaultableBoolean.[True]
        Me.ulvPartnumbers.MainColumn.Sorting = Infragistics.Win.UltraWinListView.Sorting.Ascending
        Me.ulvPartnumbers.MainColumn.Text = resources.GetString("ulvPartnumbers.MainColumn.Text")
        Me.ulvPartnumbers.MainColumn.VisiblePositionInDetailsView = 0
        Me.ulvPartnumbers.MainColumn.Width = 80
        Me.ulvPartnumbers.Name = "ulvPartnumbers"
        UltraListViewSubItemColumn1.AutoSizeMode = CType((Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.Header Or Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.AllItems), Infragistics.Win.UltraWinListView.ColumnAutoSizeMode)
        UltraListViewSubItemColumn1.Key = "description"
        resources.ApplyResources(UltraListViewSubItemColumn1, "UltraListViewSubItemColumn1")
        UltraListViewSubItemColumn1.VisiblePositionInDetailsView = 0
        UltraListViewSubItemColumn2.Key = "typeName"
        resources.ApplyResources(UltraListViewSubItemColumn2, "UltraListViewSubItemColumn2")
        UltraListViewSubItemColumn2.VisiblePositionInDetailsView = 1
        Me.ulvPartnumbers.SubItemColumns.AddRange(New Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn() {UltraListViewSubItemColumn1, UltraListViewSubItemColumn2})
        Me.ulvPartnumbers.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.Details
        Me.ulvPartnumbers.ViewSettingsDetails.AutoFitColumns = Infragistics.Win.UltraWinListView.AutoFitColumns.ResizeAllColumns
        Me.ulvPartnumbers.ViewSettingsDetails.ColumnAutoSizeMode = CType((Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.Header Or Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.VisibleItems), Infragistics.Win.UltraWinListView.ColumnAutoSizeMode)
        Me.ulvPartnumbers.ViewSettingsDetails.FullRowSelect = True
        Me.ulvPartnumbers.ViewSettingsDetails.ImageSize = New System.Drawing.Size(0, 0)
        '
        'btnView
        '
        resources.ApplyResources(Me.btnView, "btnView")
        Me.btnView.Name = "btnView"
        '
        'btnClose
        '
        Me.btnClose.AccessibleRole = System.Windows.Forms.AccessibleRole.None
        resources.ApplyResources(Me.btnClose, "btnClose")
        Me.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnClose.Name = "btnClose"
        '
        'PartnumbersForm
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.BackColor = System.Drawing.Color.White
        resources.ApplyResources(Me, "$this")
        Me.Controls.Add(Me.btnView)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.ugbPartnumbers)
        Me.Name = "PartnumbersForm"
        CType(Me.ugbPartnumbers, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ugbPartnumbers.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        CType(Me.ulvPartnumbers, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents ugbPartnumbers As Infragistics.Win.Misc.UltraGroupBox
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents tbx_partnumber As TextBox
    Friend WithEvents lbl_partnumber As Label
    Friend WithEvents ulvPartnumbers As Infragistics.Win.UltraWinListView.UltraListView
    Friend WithEvents btnView As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnClose As Infragistics.Win.Misc.UltraButton
End Class
