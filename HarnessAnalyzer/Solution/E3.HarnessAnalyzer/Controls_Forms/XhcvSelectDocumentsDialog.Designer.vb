<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class XhcvSelectDocumentsDialog
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If

            _xhcv = Nothing
            _itemColumnSorter = Nothing
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
        components = New ComponentModel.Container()
        TableLayoutPanel1 = New TableLayoutPanel()
        OK_Button = New Button()
        Cancel_Button = New Button()
        HcvDocumentsListView = New ListView()
        ColumnHcvName = New ColumnHeader()
        FileSize = New ColumnHeader()
        ContextMenuStrip1 = New ContextMenuStrip(components)
        CheckSelectedToolStripMenuItem = New ToolStripMenuItem()
        ToolStripSeparator2 = New ToolStripSeparator()
        OpenToolStripMenuItem = New ToolStripMenuItem()
        ToolStripSeparator1 = New ToolStripSeparator()
        SelectAllToolStripMenuItem = New ToolStripMenuItem()
        DeselectAllToolStripMenuItem = New ToolStripMenuItem()
        Label1 = New Label()
        Label2 = New Label()
        chkOpenTopology = New CheckBox()
        btnRemoveFromDebug = New Button()
        Label3 = New Label()
        TableLayoutPanel1.SuspendLayout()
        ContextMenuStrip1.SuspendLayout()
        SuspendLayout()
        ' 
        ' TableLayoutPanel1
        ' 
        TableLayoutPanel1.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        TableLayoutPanel1.ColumnCount = 2
        TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50F))
        TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50F))
        TableLayoutPanel1.Controls.Add(OK_Button, 0, 0)
        TableLayoutPanel1.Controls.Add(Cancel_Button, 1, 0)
        TableLayoutPanel1.Location = New Point(600, 397)
        TableLayoutPanel1.Margin = New Padding(4, 3, 4, 3)
        TableLayoutPanel1.Name = "TableLayoutPanel1"
        TableLayoutPanel1.RowCount = 1
        TableLayoutPanel1.RowStyles.Add(New RowStyle(SizeType.Percent, 50F))
        TableLayoutPanel1.RowStyles.Add(New RowStyle(SizeType.Absolute, 20F))
        TableLayoutPanel1.Size = New Size(170, 33)
        TableLayoutPanel1.TabIndex = 0
        ' 
        ' OK_Button
        ' 
        OK_Button.Anchor = AnchorStyles.None
        OK_Button.Location = New Point(4, 3)
        OK_Button.Margin = New Padding(4, 3, 4, 3)
        OK_Button.Name = "OK_Button"
        OK_Button.Size = New Size(77, 27)
        OK_Button.TabIndex = 0
        OK_Button.Text = "OK"
        ' 
        ' Cancel_Button
        ' 
        Cancel_Button.Anchor = AnchorStyles.None
        Cancel_Button.Location = New Point(89, 3)
        Cancel_Button.Margin = New Padding(4, 3, 4, 3)
        Cancel_Button.Name = "Cancel_Button"
        Cancel_Button.Size = New Size(77, 27)
        Cancel_Button.TabIndex = 1
        Cancel_Button.Text = "Cancel"
        ' 
        ' HcvDocumentsListView
        ' 
        HcvDocumentsListView.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        HcvDocumentsListView.CheckBoxes = True
        HcvDocumentsListView.Columns.AddRange(New ColumnHeader() {ColumnHcvName, FileSize})
        HcvDocumentsListView.ContextMenuStrip = ContextMenuStrip1
        HcvDocumentsListView.FullRowSelect = True
        HcvDocumentsListView.Location = New Point(21, 60)
        HcvDocumentsListView.Name = "HcvDocumentsListView"
        HcvDocumentsListView.Size = New Size(749, 331)
        HcvDocumentsListView.TabIndex = 1
        HcvDocumentsListView.UseCompatibleStateImageBehavior = False
        HcvDocumentsListView.View = View.Details
        ' 
        ' ColumnHcvName
        ' 
        ColumnHcvName.Text = "Hcv file"
        ' 
        ' FileSize
        ' 
        FileSize.Text = "File sitze (MB)"
        ' 
        ' ContextMenuStrip1
        ' 
        ContextMenuStrip1.Items.AddRange(New ToolStripItem() {CheckSelectedToolStripMenuItem, ToolStripSeparator2, OpenToolStripMenuItem, ToolStripSeparator1, SelectAllToolStripMenuItem, DeselectAllToolStripMenuItem})
        ContextMenuStrip1.Name = "ContextMenuStrip1"
        ContextMenuStrip1.Size = New Size(184, 104)
        ' 
        ' CheckSelectedToolStripMenuItem
        ' 
        CheckSelectedToolStripMenuItem.Image = Resources.checkbox
        CheckSelectedToolStripMenuItem.Name = "CheckSelectedToolStripMenuItem"
        CheckSelectedToolStripMenuItem.Size = New Size(183, 22)
        CheckSelectedToolStripMenuItem.Text = "Check selection only"
        ' 
        ' ToolStripSeparator2
        ' 
        ToolStripSeparator2.Name = "ToolStripSeparator2"
        ToolStripSeparator2.Size = New Size(180, 6)
        ' 
        ' OpenToolStripMenuItem
        ' 
        OpenToolStripMenuItem.Image = ExportCompareResult
        OpenToolStripMenuItem.Name = "OpenToolStripMenuItem"
        OpenToolStripMenuItem.Size = New Size(183, 22)
        OpenToolStripMenuItem.Text = "Show content"
        ' 
        ' ToolStripSeparator1
        ' 
        ToolStripSeparator1.Name = "ToolStripSeparator1"
        ToolStripSeparator1.Size = New Size(180, 6)
        ' 
        ' SelectAllToolStripMenuItem
        ' 
        SelectAllToolStripMenuItem.Name = "SelectAllToolStripMenuItem"
        SelectAllToolStripMenuItem.Size = New Size(183, 22)
        SelectAllToolStripMenuItem.Text = "Select all"
        ' 
        ' DeselectAllToolStripMenuItem
        ' 
        DeselectAllToolStripMenuItem.Name = "DeselectAllToolStripMenuItem"
        DeselectAllToolStripMenuItem.Size = New Size(183, 22)
        DeselectAllToolStripMenuItem.Text = "Uncheck all"
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(21, 33)
        Label1.Name = "Label1"
        Label1.Size = New Size(234, 15)
        Label1.TabIndex = 2
        Label1.Text = "Select documents to be opened from xhcv:"
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        Label2.Location = New Point(18, 9)
        Label2.Name = "Label2"
        Label2.Size = New Size(410, 15)
        Label2.TabIndex = 3
        Label2.Text = "THIS PRE-SELECTION IS IN DEBUG-MODE-ONLY (FOR FASTER TESTING) !"
        ' 
        ' chkOpenTopology
        ' 
        chkOpenTopology.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        chkOpenTopology.AutoSize = True
        chkOpenTopology.Location = New Point(21, 400)
        chkOpenTopology.Name = "chkOpenTopology"
        chkOpenTopology.Size = New Size(107, 19)
        chkOpenTopology.TabIndex = 4
        chkOpenTopology.Text = "Open Drawings"
        chkOpenTopology.UseVisualStyleBackColor = True
        ' 
        ' btnRemoveFromDebug
        ' 
        btnRemoveFromDebug.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnRemoveFromDebug.Location = New Point(604, 12)
        btnRemoveFromDebug.Margin = New Padding(4, 3, 4, 3)
        btnRemoveFromDebug.Name = "btnRemoveFromDebug"
        btnRemoveFromDebug.Size = New Size(162, 27)
        btnRemoveFromDebug.TabIndex = 5
        btnRemoveFromDebug.Text = "Remove this dialog!"
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Font = New Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        Label3.Location = New Point(261, 33)
        Label3.Name = "Label3"
        Label3.Size = New Size(273, 15)
        Label3.TabIndex = 6
        Label3.Text = "(Selecting not the whole xhcv breaks Inliner-Pairs!)"
        ' 
        ' XhcvSelectDocumentsDialog
        ' 
        AcceptButton = OK_Button
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        CancelButton = Cancel_Button
        ClientSize = New Size(784, 444)
        Controls.Add(Label3)
        Controls.Add(btnRemoveFromDebug)
        Controls.Add(chkOpenTopology)
        Controls.Add(Label2)
        Controls.Add(Label1)
        Controls.Add(HcvDocumentsListView)
        Controls.Add(TableLayoutPanel1)
        Margin = New Padding(4, 3, 4, 3)
        MaximizeBox = False
        MinimizeBox = False
        Name = "XhcvSelectDocumentsDialog"
        ShowInTaskbar = False
        StartPosition = FormStartPosition.CenterParent
        Text = "Open xhcv ""{0}"""
        TableLayoutPanel1.ResumeLayout(False)
        ContextMenuStrip1.ResumeLayout(False)
        ResumeLayout(False)
        PerformLayout()

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents OK_Button As System.Windows.Forms.Button
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents HcvDocumentsListView As ListView
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents chkOpenTopology As CheckBox
    Friend WithEvents ColumnHcvName As ColumnHeader
    Friend WithEvents FileSize As ColumnHeader
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents SelectAllToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents btnRemoveFromDebug As Button
    Friend WithEvents Label3 As Label
    Friend WithEvents OpenToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents DeselectAllToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
    Friend WithEvents CheckSelectedToolStripMenuItem As ToolStripMenuItem

End Class
