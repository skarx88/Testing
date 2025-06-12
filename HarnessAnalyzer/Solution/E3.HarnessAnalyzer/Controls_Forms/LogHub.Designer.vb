<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class LogHub
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            If disposing Then
                _connectedToastManager = Nothing
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
        components = New ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(LogHub))
        Dim UltraListViewSubItemColumn1 As Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn = New Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("LogMessage")
        btnExport = New Infragistics.Win.Misc.UltraButton()
        ulvLog = New Infragistics.Win.UltraWinListView.UltraListView()
        ContextMenuStrip1 = New ContextMenuStrip(components)
        ShowToolStripMenuItem = New ToolStripMenuItem()
        CopyToolStripMenuItem = New ToolStripMenuItem()
        CType(ulvLog, ComponentModel.ISupportInitialize).BeginInit()
        ContextMenuStrip1.SuspendLayout()
        SuspendLayout()
        ' 
        ' btnExport
        ' 
        resources.ApplyResources(btnExport, "btnExport")
        btnExport.Name = "btnExport"
        ' 
        ' ulvLog
        ' 
        ulvLog.ContextMenuStrip = ContextMenuStrip1
        resources.ApplyResources(ulvLog, "ulvLog")
        ulvLog.ImageTransparentColor = Color.Transparent
        ulvLog.MainColumn.AllowMoving = Infragistics.Win.DefaultableBoolean.False
        ulvLog.MainColumn.DataType = GetType(String)
        ulvLog.MainColumn.Key = "LogLevel"
        ulvLog.MainColumn.Text = resources.GetString("ulvLog.MainColumn.Text")
        ulvLog.MainColumn.Width = 100
        ulvLog.Name = "ulvLog"
        UltraListViewSubItemColumn1.AllowMoving = Infragistics.Win.DefaultableBoolean.False
        UltraListViewSubItemColumn1.DataType = GetType(String)
        UltraListViewSubItemColumn1.Key = "LogMessage"
        resources.ApplyResources(UltraListViewSubItemColumn1, "UltraListViewSubItemColumn1")
        UltraListViewSubItemColumn1.Width = 800
        ulvLog.SubItemColumns.AddRange(New Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn() {UltraListViewSubItemColumn1})
        ulvLog.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.Details
        ulvLog.ViewSettingsDetails.FullRowSelect = True
        ' 
        ' ContextMenuStrip1
        ' 
        ContextMenuStrip1.Items.AddRange(New ToolStripItem() {ShowToolStripMenuItem, CopyToolStripMenuItem})
        ContextMenuStrip1.Name = "ContextMenuStrip1"
        resources.ApplyResources(ContextMenuStrip1, "ContextMenuStrip1")
        ' 
        ' ShowToolStripMenuItem
        ' 
        ShowToolStripMenuItem.Name = "ShowToolStripMenuItem"
        resources.ApplyResources(ShowToolStripMenuItem, "ShowToolStripMenuItem")
        ' 
        ' CopyToolStripMenuItem
        ' 
        CopyToolStripMenuItem.Name = "CopyToolStripMenuItem"
        resources.ApplyResources(CopyToolStripMenuItem, "CopyToolStripMenuItem")
        ' 
        ' LogHub
        ' 
        resources.ApplyResources(Me, "$this")
        AutoScaleMode = AutoScaleMode.Font
        Controls.Add(btnExport)
        Controls.Add(ulvLog)
        Name = HarnessAnalyzer.Shared.Common.PaneKeys.LogHub.ToString
        CType(ulvLog, ComponentModel.ISupportInitialize).EndInit()
        ContextMenuStrip1.ResumeLayout(False)
        ResumeLayout(False)

    End Sub
    Friend WithEvents btnExport As Infragistics.Win.Misc.UltraButton
    Friend WithEvents ulvLog As Infragistics.Win.UltraWinListView.UltraListView
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents CopyToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ShowToolStripMenuItem As ToolStripMenuItem

End Class
