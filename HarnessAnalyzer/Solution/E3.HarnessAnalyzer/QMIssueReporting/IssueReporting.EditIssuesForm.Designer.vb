Namespace IssueReporting
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class EditIssuesForm
        Inherits System.Windows.Forms.Form

        'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                    _issuesDataSet.Dispose()
                End If

                RemoveHandler _activeDrawingCanvas.CanvasSelectionChanged, AddressOf _activeCanvas_SelectionChanged
                RemoveHandler _activeDrawingCanvas.CanvasMouseClick, AddressOf _activeCanvas_MouseClick
                RemoveHandler _mainUtMdiManager.TabClosing, AddressOf _utmmMain_TabClosing
                RemoveHandler _activeDocument._informationHub.HubSelectionChanged, AddressOf _activeDocument_HubSelectionChanged

                _reportFile = Nothing
                _activeDocument = Nothing
                _activeDrawingCanvas = Nothing
                _mainUtMdiManager = Nothing
                _issuesDataSet = Nothing
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
            Me.components = New System.ComponentModel.Container()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(EditIssuesForm))
            Dim UltraGridBand1 As Infragistics.Win.UltraWinGrid.UltraGridBand = New Infragistics.Win.UltraWinGrid.UltraGridBand("Issues", -1)
            Dim UltraGridColumn1 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("Id")
            Dim UltraGridColumn2 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("ObjectReference")
            Dim UltraGridColumn3 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("Description")
            Dim UltraGridColumn4 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("IssueTag")
            Dim UltraGridColumn5 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("NumberOfOccurrences")
            Dim UltraGridColumn6 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("ConfirmedBy")
            Dim UltraGridColumn7 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("DateOfConfirmation")
            Dim UltraGridColumn8 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("ConfirmationComment")
            Dim UltraGridColumn9 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("ListObject")
            Me.MainPanel = New Infragistics.Win.Misc.UltraPanel()
            Me.MainTableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
            Me.ugIssues = New Infragistics.Win.UltraWinGrid.UltraGrid()
            Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.EditValueReturnToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
            Me.DeleteEntfToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.CopyToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.CutValueToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.PasteValuesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.IssuesBindingSource = New System.Windows.Forms.BindingSource(Me.components)
            Me.QMIssueReportingDataSet = New Zuken.E3.HarnessAnalyzer.IssueReporting.QMIssueReportingDataSet()
            Me.ugbHeaderInformation = New Infragistics.Win.Misc.UltraExpandableGroupBox()
            Me.UltraExpandableGroupBoxPanel1 = New Infragistics.Win.Misc.UltraExpandableGroupBoxPanel()
            Me.GroupBoxTableLayout = New System.Windows.Forms.TableLayoutPanel()
            Me.ulVersion = New Infragistics.Win.Misc.UltraLabel()
            Me.UltraLabel1 = New Infragistics.Win.Misc.UltraLabel()
            Me.UltraLabel2 = New Infragistics.Win.Misc.UltraLabel()
            Me.UltraLabel3 = New Infragistics.Win.Misc.UltraLabel()
            Me.txtAuthoringSystem = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
            Me.ReportFileBindingSource = New System.Windows.Forms.BindingSource(Me.components)
            Me.txtVersion = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
            Me.txtCreatedBy = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
            Me.ObservationPanel = New Infragistics.Win.Misc.UltraPanel()
            Me.utxtEndOfObservation = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
            Me.utxtStartOfObservation = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
            Me.UltraLabel4 = New Infragistics.Win.Misc.UltraLabel()
            Me.ButtonPanel = New Infragistics.Win.Misc.UltraPanel()
            Me.btnExport = New Infragistics.Win.Misc.UltraButton()
            Me.btnClose = New Infragistics.Win.Misc.UltraButton()
            Me.ExportFileDialog = New System.Windows.Forms.SaveFileDialog()
            Me.UltraGridExcelExporter1 = New Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter(Me.components)
            Me.MainPanel.ClientArea.SuspendLayout()
            Me.MainPanel.SuspendLayout()
            Me.MainTableLayoutPanel.SuspendLayout()
            CType(Me.ugIssues, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.ContextMenuStrip1.SuspendLayout()
            CType(Me.IssuesBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.QMIssueReportingDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.ugbHeaderInformation, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.ugbHeaderInformation.SuspendLayout()
            Me.UltraExpandableGroupBoxPanel1.SuspendLayout()
            Me.GroupBoxTableLayout.SuspendLayout()
            CType(Me.txtAuthoringSystem, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.ReportFileBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.txtVersion, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.txtCreatedBy, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.ObservationPanel.ClientArea.SuspendLayout()
            Me.ObservationPanel.SuspendLayout()
            CType(Me.utxtEndOfObservation, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.utxtStartOfObservation, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.ButtonPanel.ClientArea.SuspendLayout()
            Me.ButtonPanel.SuspendLayout()
            Me.SuspendLayout()
            '
            'MainPanel
            '
            resources.ApplyResources(Me.MainPanel, "MainPanel")
            '
            'MainPanel.ClientArea
            '
            resources.ApplyResources(Me.MainPanel.ClientArea, "MainPanel.ClientArea")
            Me.MainPanel.ClientArea.Controls.Add(Me.MainTableLayoutPanel)
            Me.MainPanel.Name = "MainPanel"
            '
            'MainTableLayoutPanel
            '
            resources.ApplyResources(Me.MainTableLayoutPanel, "MainTableLayoutPanel")
            Me.MainTableLayoutPanel.Controls.Add(Me.ugIssues, 0, 1)
            Me.MainTableLayoutPanel.Controls.Add(Me.ugbHeaderInformation, 0, 0)
            Me.MainTableLayoutPanel.Controls.Add(Me.ButtonPanel, 0, 2)
            Me.MainTableLayoutPanel.Name = "MainTableLayoutPanel"
            '
            'ugIssues
            '
            resources.ApplyResources(Me.ugIssues, "ugIssues")
            Me.ugIssues.ContextMenuStrip = Me.ContextMenuStrip1
            Me.ugIssues.DataSource = Me.IssuesBindingSource
            Me.ugIssues.DisplayLayout.AddNewBox.Prompt = resources.GetString("ugIssues.DisplayLayout.AddNewBox.Prompt")
            Me.ugIssues.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns
            UltraGridColumn1.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit
            UltraGridColumn1.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.CellSelect
            UltraGridColumn1.Header.Editor = Nothing
            UltraGridColumn1.Header.VisiblePosition = 0
            UltraGridColumn1.Width = 58
            resources.ApplyResources(UltraGridColumn1, "UltraGridColumn1")
            UltraGridColumn1.ForceApplyResources = ""
            UltraGridColumn2.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit
            UltraGridColumn2.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.CellSelect
            resources.ApplyResources(UltraGridColumn2.Header, "UltraGridColumn2.Header")
            UltraGridColumn2.Header.Editor = Nothing
            UltraGridColumn2.Header.VisiblePosition = 1
            UltraGridColumn2.Width = 74
            UltraGridColumn2.ForceApplyResources = "Header"
            UltraGridColumn3.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit
            UltraGridColumn3.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.CellSelect
            resources.ApplyResources(UltraGridColumn3.Header, "UltraGridColumn3.Header")
            UltraGridColumn3.Header.Editor = Nothing
            UltraGridColumn3.Header.VisiblePosition = 2
            UltraGridColumn3.Width = 105
            UltraGridColumn4.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit
            UltraGridColumn4.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.CellSelect
            resources.ApplyResources(UltraGridColumn4.Header, "UltraGridColumn4.Header")
            UltraGridColumn4.Header.Editor = Nothing
            UltraGridColumn4.Header.VisiblePosition = 3
            UltraGridColumn4.Width = 107
            UltraGridColumn4.ForceApplyResources = "Header"
            UltraGridColumn5.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit
            UltraGridColumn5.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.CellSelect
            resources.ApplyResources(UltraGridColumn5.Header, "UltraGridColumn5.Header")
            UltraGridColumn5.Header.Editor = Nothing
            UltraGridColumn5.Header.VisiblePosition = 4
            UltraGridColumn5.Width = 143
            UltraGridColumn5.ForceApplyResources = "Header"
            UltraGridColumn6.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.CellSelect
            resources.ApplyResources(UltraGridColumn6.Header, "UltraGridColumn6.Header")
            UltraGridColumn6.Header.Editor = Nothing
            UltraGridColumn6.Header.VisiblePosition = 5
            UltraGridColumn6.Width = 72
            UltraGridColumn6.ForceApplyResources = "Header"
            UltraGridColumn7.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit
            UltraGridColumn7.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.CellSelect
            resources.ApplyResources(UltraGridColumn7.Header, "UltraGridColumn7.Header")
            UltraGridColumn7.Header.Editor = Nothing
            UltraGridColumn7.Header.VisiblePosition = 6
            resources.ApplyResources(UltraGridColumn7, "UltraGridColumn7")
            UltraGridColumn7.Nullable = Infragistics.Win.UltraWinGrid.Nullable.Null
            UltraGridColumn7.Width = 93
            UltraGridColumn7.ForceApplyResources = "|Header"
            UltraGridColumn8.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.CellSelect
            resources.ApplyResources(UltraGridColumn8.Header, "UltraGridColumn8.Header")
            UltraGridColumn8.Header.Editor = Nothing
            UltraGridColumn8.Header.VisiblePosition = 8
            UltraGridColumn8.Width = 107
            UltraGridColumn8.ForceApplyResources = "Header"
            UltraGridColumn9.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit
            UltraGridColumn9.Header.Editor = Nothing
            UltraGridColumn9.Header.VisiblePosition = 7
            UltraGridColumn9.Hidden = True
            UltraGridColumn9.Width = 79
            UltraGridBand1.Columns.AddRange(New Object() {UltraGridColumn1, UltraGridColumn2, UltraGridColumn3, UltraGridColumn4, UltraGridColumn5, UltraGridColumn6, UltraGridColumn7, UltraGridColumn8, UltraGridColumn9})
            Me.ugIssues.DisplayLayout.BandsSerializer.Add(UltraGridBand1)
            Me.ugIssues.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.[False]
            Me.ugIssues.DisplayLayout.MaxRowScrollRegions = 1
            Me.ugIssues.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No
            Me.ugIssues.DisplayLayout.Override.AllowColMoving = Infragistics.Win.UltraWinGrid.AllowColMoving.NotAllowed
            Me.ugIssues.DisplayLayout.Override.AllowColSwapping = Infragistics.Win.UltraWinGrid.AllowColSwapping.NotAllowed
            Me.ugIssues.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.[False]
            Me.ugIssues.DisplayLayout.Override.AllowGroupMoving = Infragistics.Win.UltraWinGrid.AllowGroupMoving.NotAllowed
            Me.ugIssues.DisplayLayout.Override.AllowGroupSwapping = Infragistics.Win.UltraWinGrid.AllowGroupSwapping.NotAllowed
            Me.ugIssues.DisplayLayout.Override.AllowMultiCellOperations = CType((((((Infragistics.Win.UltraWinGrid.AllowMultiCellOperation.Copy Or Infragistics.Win.UltraWinGrid.AllowMultiCellOperation.Cut) _
            Or Infragistics.Win.UltraWinGrid.AllowMultiCellOperation.Delete) _
            Or Infragistics.Win.UltraWinGrid.AllowMultiCellOperation.Paste) _
            Or Infragistics.Win.UltraWinGrid.AllowMultiCellOperation.Undo) _
            Or Infragistics.Win.UltraWinGrid.AllowMultiCellOperation.Redo), Infragistics.Win.UltraWinGrid.AllowMultiCellOperation)
            Me.ugIssues.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.[True]
            Me.ugIssues.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.CellSelect
            Me.ugIssues.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortSingle
            Me.ugIssues.DisplayLayout.Override.RowSizing = Infragistics.Win.UltraWinGrid.RowSizing.Fixed
            Me.ugIssues.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand
            Me.ugIssues.Name = "ugIssues"
            '
            'ContextMenuStrip1
            '
            resources.ApplyResources(Me.ContextMenuStrip1, "ContextMenuStrip1")
            Me.ContextMenuStrip1.ImageScalingSize = New System.Drawing.Size(20, 20)
            Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.EditValueReturnToolStripMenuItem, Me.ToolStripSeparator1, Me.DeleteEntfToolStripMenuItem, Me.CopyToolStripMenuItem, Me.CutValueToolStripMenuItem, Me.PasteValuesToolStripMenuItem})
            Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
            '
            'EditValueReturnToolStripMenuItem
            '
            resources.ApplyResources(Me.EditValueReturnToolStripMenuItem, "EditValueReturnToolStripMenuItem")
            Me.EditValueReturnToolStripMenuItem.Name = "EditValueReturnToolStripMenuItem"
            '
            'ToolStripSeparator1
            '
            resources.ApplyResources(Me.ToolStripSeparator1, "ToolStripSeparator1")
            Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
            '
            'DeleteEntfToolStripMenuItem
            '
            resources.ApplyResources(Me.DeleteEntfToolStripMenuItem, "DeleteEntfToolStripMenuItem")
            Me.DeleteEntfToolStripMenuItem.Name = "DeleteEntfToolStripMenuItem"
            '
            'CopyToolStripMenuItem
            '
            resources.ApplyResources(Me.CopyToolStripMenuItem, "CopyToolStripMenuItem")
            Me.CopyToolStripMenuItem.Name = "CopyToolStripMenuItem"
            '
            'CutValueToolStripMenuItem
            '
            resources.ApplyResources(Me.CutValueToolStripMenuItem, "CutValueToolStripMenuItem")
            Me.CutValueToolStripMenuItem.Name = "CutValueToolStripMenuItem"
            '
            'PasteValuesToolStripMenuItem
            '
            resources.ApplyResources(Me.PasteValuesToolStripMenuItem, "PasteValuesToolStripMenuItem")
            Me.PasteValuesToolStripMenuItem.Name = "PasteValuesToolStripMenuItem"
            '
            'IssuesBindingSource
            '
            Me.IssuesBindingSource.DataMember = "Issues"
            Me.IssuesBindingSource.DataSource = Me.QMIssueReportingDataSet
            '
            'QMIssueReportingDataSet
            '
            Me.QMIssueReportingDataSet.DataSetName = "QMIssueReportingDataSet"
            Me.QMIssueReportingDataSet.Locale = New System.Globalization.CultureInfo("")
            Me.QMIssueReportingDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
            '
            'ugbHeaderInformation
            '
            resources.ApplyResources(Me.ugbHeaderInformation, "ugbHeaderInformation")
            Me.ugbHeaderInformation.Controls.Add(Me.UltraExpandableGroupBoxPanel1)
            Me.ugbHeaderInformation.ExpandedSize = New System.Drawing.Size(780, 123)
            Me.ugbHeaderInformation.Name = "ugbHeaderInformation"
            '
            'UltraExpandableGroupBoxPanel1
            '
            resources.ApplyResources(Me.UltraExpandableGroupBoxPanel1, "UltraExpandableGroupBoxPanel1")
            Me.UltraExpandableGroupBoxPanel1.Controls.Add(Me.GroupBoxTableLayout)
            Me.UltraExpandableGroupBoxPanel1.Name = "UltraExpandableGroupBoxPanel1"
            '
            'GroupBoxTableLayout
            '
            resources.ApplyResources(Me.GroupBoxTableLayout, "GroupBoxTableLayout")
            Me.GroupBoxTableLayout.Controls.Add(Me.ulVersion, 0, 0)
            Me.GroupBoxTableLayout.Controls.Add(Me.UltraLabel1, 0, 1)
            Me.GroupBoxTableLayout.Controls.Add(Me.UltraLabel2, 0, 2)
            Me.GroupBoxTableLayout.Controls.Add(Me.UltraLabel3, 0, 3)
            Me.GroupBoxTableLayout.Controls.Add(Me.txtAuthoringSystem, 1, 2)
            Me.GroupBoxTableLayout.Controls.Add(Me.txtVersion, 1, 0)
            Me.GroupBoxTableLayout.Controls.Add(Me.txtCreatedBy, 1, 1)
            Me.GroupBoxTableLayout.Controls.Add(Me.ObservationPanel, 1, 3)
            Me.GroupBoxTableLayout.Name = "GroupBoxTableLayout"
            '
            'ulVersion
            '
            resources.ApplyResources(Me.ulVersion, "ulVersion")
            Me.ulVersion.Name = "ulVersion"
            '
            'UltraLabel1
            '
            resources.ApplyResources(Me.UltraLabel1, "UltraLabel1")
            Me.UltraLabel1.Name = "UltraLabel1"
            '
            'UltraLabel2
            '
            resources.ApplyResources(Me.UltraLabel2, "UltraLabel2")
            Me.UltraLabel2.Name = "UltraLabel2"
            '
            'UltraLabel3
            '
            resources.ApplyResources(Me.UltraLabel3, "UltraLabel3")
            Me.UltraLabel3.Name = "UltraLabel3"
            '
            'txtAuthoringSystem
            '
            resources.ApplyResources(Me.txtAuthoringSystem, "txtAuthoringSystem")
            Me.txtAuthoringSystem.DataBindings.Add(New System.Windows.Forms.Binding("Value", Me.ReportFileBindingSource, "AuthoringSystem", True))
            Me.txtAuthoringSystem.Name = "txtAuthoringSystem"
            Me.txtAuthoringSystem.ReadOnly = True
            '
            'ReportFileBindingSource
            '
            Me.ReportFileBindingSource.DataSource = GetType(Zuken.E3.HarnessAnalyzer.IssueReporting.ReportFile)
            '
            'txtVersion
            '
            resources.ApplyResources(Me.txtVersion, "txtVersion")
            Me.txtVersion.DataBindings.Add(New System.Windows.Forms.Binding("Value", Me.ReportFileBindingSource, "Version", True))
            Me.txtVersion.Name = "txtVersion"
            Me.txtVersion.ReadOnly = True
            '
            'txtCreatedBy
            '
            resources.ApplyResources(Me.txtCreatedBy, "txtCreatedBy")
            Me.txtCreatedBy.DataBindings.Add(New System.Windows.Forms.Binding("Value", Me.ReportFileBindingSource, "CreatedBy", True))
            Me.txtCreatedBy.Name = "txtCreatedBy"
            Me.txtCreatedBy.ReadOnly = True
            '
            'ObservationPanel
            '
            resources.ApplyResources(Me.ObservationPanel, "ObservationPanel")
            '
            'ObservationPanel.ClientArea
            '
            resources.ApplyResources(Me.ObservationPanel.ClientArea, "ObservationPanel.ClientArea")
            Me.ObservationPanel.ClientArea.Controls.Add(Me.utxtEndOfObservation)
            Me.ObservationPanel.ClientArea.Controls.Add(Me.utxtStartOfObservation)
            Me.ObservationPanel.ClientArea.Controls.Add(Me.UltraLabel4)
            Me.ObservationPanel.Name = "ObservationPanel"
            '
            'utxtEndOfObservation
            '
            resources.ApplyResources(Me.utxtEndOfObservation, "utxtEndOfObservation")
            Me.utxtEndOfObservation.DataBindings.Add(New System.Windows.Forms.Binding("Value", Me.ReportFileBindingSource, "EndOfObservation", True))
            Me.utxtEndOfObservation.Name = "utxtEndOfObservation"
            Me.utxtEndOfObservation.ReadOnly = True
            '
            'utxtStartOfObservation
            '
            resources.ApplyResources(Me.utxtStartOfObservation, "utxtStartOfObservation")
            Me.utxtStartOfObservation.DataBindings.Add(New System.Windows.Forms.Binding("Value", Me.ReportFileBindingSource, "BeginOfObservation", True))
            Me.utxtStartOfObservation.Name = "utxtStartOfObservation"
            Me.utxtStartOfObservation.ReadOnly = True
            '
            'UltraLabel4
            '
            resources.ApplyResources(Me.UltraLabel4, "UltraLabel4")
            Me.UltraLabel4.Name = "UltraLabel4"
            '
            'ButtonPanel
            '
            resources.ApplyResources(Me.ButtonPanel, "ButtonPanel")
            '
            'ButtonPanel.ClientArea
            '
            resources.ApplyResources(Me.ButtonPanel.ClientArea, "ButtonPanel.ClientArea")
            Me.ButtonPanel.ClientArea.Controls.Add(Me.btnExport)
            Me.ButtonPanel.ClientArea.Controls.Add(Me.btnClose)
            Me.ButtonPanel.Name = "ButtonPanel"
            '
            'btnExport
            '
            resources.ApplyResources(Me.btnExport, "btnExport")
            Me.btnExport.Name = "btnExport"
            '
            'btnClose
            '
            resources.ApplyResources(Me.btnClose, "btnClose")
            Me.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.btnClose.Name = "btnClose"
            '
            'ExportFileDialog
            '
            Me.ExportFileDialog.DefaultExt = IO.KnownFile.XLSX.Trim("."c)
            Me.ExportFileDialog.FileName = "*.xlsx"
            resources.ApplyResources(Me.ExportFileDialog, "ExportFileDialog")
            '
            'UltraGridExcelExporter1
            '
            Me.UltraGridExcelExporter1.FileLimitBehaviour = Infragistics.Win.UltraWinGrid.ExcelExport.FileLimitBehaviour.TruncateData
            '
            'EditIssuesForm
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
            Me.CancelButton = Me.btnClose
            Me.Controls.Add(Me.MainPanel)
            Me.Name = "EditIssuesForm"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.MainPanel.ClientArea.ResumeLayout(False)
            Me.MainPanel.ResumeLayout(False)
            Me.MainTableLayoutPanel.ResumeLayout(False)
            CType(Me.ugIssues, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ContextMenuStrip1.ResumeLayout(False)
            CType(Me.IssuesBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.QMIssueReportingDataSet, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.ugbHeaderInformation, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ugbHeaderInformation.ResumeLayout(False)
            Me.UltraExpandableGroupBoxPanel1.ResumeLayout(False)
            Me.UltraExpandableGroupBoxPanel1.PerformLayout()
            Me.GroupBoxTableLayout.ResumeLayout(False)
            Me.GroupBoxTableLayout.PerformLayout()
            CType(Me.txtAuthoringSystem, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.ReportFileBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.txtVersion, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.txtCreatedBy, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ObservationPanel.ClientArea.ResumeLayout(False)
            Me.ObservationPanel.ClientArea.PerformLayout()
            Me.ObservationPanel.ResumeLayout(False)
            Me.ObservationPanel.PerformLayout()
            CType(Me.utxtEndOfObservation, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.utxtStartOfObservation, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ButtonPanel.ClientArea.ResumeLayout(False)
            Me.ButtonPanel.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents MainPanel As Infragistics.Win.Misc.UltraPanel
        Friend WithEvents ugIssues As Infragistics.Win.UltraWinGrid.UltraGrid
        Friend WithEvents ExportFileDialog As System.Windows.Forms.SaveFileDialog
        Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
        Friend WithEvents DeleteEntfToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents EditValueReturnToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents CopyToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents CutValueToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents PasteValuesToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents ugbHeaderInformation As Infragistics.Win.Misc.UltraExpandableGroupBox
        Friend WithEvents UltraExpandableGroupBoxPanel1 As Infragistics.Win.Misc.UltraExpandableGroupBoxPanel
        Friend WithEvents GroupBoxTableLayout As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents ulVersion As Infragistics.Win.Misc.UltraLabel
        Friend WithEvents UltraLabel1 As Infragistics.Win.Misc.UltraLabel
        Friend WithEvents UltraLabel2 As Infragistics.Win.Misc.UltraLabel
        Friend WithEvents UltraLabel3 As Infragistics.Win.Misc.UltraLabel
        Friend WithEvents txtAuthoringSystem As Infragistics.Win.UltraWinEditors.UltraTextEditor
        Friend WithEvents txtVersion As Infragistics.Win.UltraWinEditors.UltraTextEditor
        Friend WithEvents ReportFileBindingSource As System.Windows.Forms.BindingSource
        Friend WithEvents txtCreatedBy As Infragistics.Win.UltraWinEditors.UltraTextEditor
        Friend WithEvents ObservationPanel As Infragistics.Win.Misc.UltraPanel
        Friend WithEvents UltraLabel4 As Infragistics.Win.Misc.UltraLabel
        Friend WithEvents MainTableLayoutPanel As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents ButtonPanel As Infragistics.Win.Misc.UltraPanel
        Friend WithEvents btnExport As Infragistics.Win.Misc.UltraButton
        Friend WithEvents btnClose As Infragistics.Win.Misc.UltraButton
        Friend WithEvents UltraGridExcelExporter1 As Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter
        Friend WithEvents IssuesBindingSource As System.Windows.Forms.BindingSource
        Friend WithEvents QMIssueReportingDataSet As Zuken.E3.HarnessAnalyzer.IssueReporting.QMIssueReportingDataSet
        Friend WithEvents utxtStartOfObservation As Infragistics.Win.UltraWinEditors.UltraTextEditor
        Friend WithEvents utxtEndOfObservation As Infragistics.Win.UltraWinEditors.UltraTextEditor
    End Class
End Namespace