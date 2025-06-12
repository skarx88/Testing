<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ServerLicensesDialog
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            Me.Manager = Nothing
            Me.Logger = Nothing
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
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.OK_Button = New System.Windows.Forms.Button()
        Me.Refresh_Button = New System.Windows.Forms.Button()
        Me.ListView1 = New System.Windows.Forms.ListView()
        Me.ColumnFeature = New System.Windows.Forms.ColumnHeader()
        Me.ColumnVersion = New System.Windows.Forms.ColumnHeader()
        Me.ColumnLicCount = New System.Windows.Forms.ColumnHeader()
        Me.ColumnVendor = New System.Windows.Forms.ColumnHeader()
        Me.ColumnExpires = New System.Windows.Forms.ColumnHeader()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.RemoveLicensedFeatureToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BorrowToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.UltraActivityIndicator1 = New Infragistics.Win.UltraActivityIndicator.UltraActivityIndicator()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.OK_Button, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Refresh_Button, 0, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(323, 316)
        Me.TableLayoutPanel1.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(170, 33)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'OK_Button
        '
        Me.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.OK_Button.Location = New System.Drawing.Point(89, 3)
        Me.OK_Button.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.OK_Button.Name = "OK_Button"
        Me.OK_Button.Size = New System.Drawing.Size(77, 27)
        Me.OK_Button.TabIndex = 0
        Me.OK_Button.Text = "OK"
        '
        'Refresh_Button
        '
        Me.Refresh_Button.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Refresh_Button.Location = New System.Drawing.Point(4, 3)
        Me.Refresh_Button.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.Refresh_Button.Name = "Refresh_Button"
        Me.Refresh_Button.Size = New System.Drawing.Size(77, 27)
        Me.Refresh_Button.TabIndex = 1
        Me.Refresh_Button.Text = "Refresh"
        '
        'ListView1
        '
        Me.ListView1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ListView1.CheckBoxes = True
        Me.ListView1.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnFeature, Me.ColumnVersion, Me.ColumnLicCount, Me.ColumnVendor, Me.ColumnExpires})
        Me.ListView1.ContextMenuStrip = Me.ContextMenuStrip1
        Me.ListView1.FullRowSelect = True
        Me.ListView1.Location = New System.Drawing.Point(12, 12)
        Me.ListView1.Name = "ListView1"
        Me.ListView1.Size = New System.Drawing.Size(481, 298)
        Me.ListView1.TabIndex = 1
        Me.ListView1.UseCompatibleStateImageBehavior = False
        Me.ListView1.View = System.Windows.Forms.View.Details
        Me.ListView1.Visible = False
        '
        'ColumnFeature
        '
        Me.ColumnFeature.Text = "Feature"
        '
        'ColumnVersion
        '
        Me.ColumnVersion.Text = "Version"
        '
        'ColumnLicCount
        '
        Me.ColumnLicCount.Text = "Licenses"
        '
        'ColumnVendor
        '
        Me.ColumnVendor.Text = "Vendor"
        '
        'ColumnExpires
        '
        Me.ColumnExpires.Text = "Expires"
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.RemoveLicensedFeatureToolStripMenuItem, Me.BorrowToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(199, 48)
        '
        'RemoveLicensedFeatureToolStripMenuItem
        '
        Me.RemoveLicensedFeatureToolStripMenuItem.Name = "RemoveLicensedFeatureToolStripMenuItem"
        Me.RemoveLicensedFeatureToolStripMenuItem.Size = New System.Drawing.Size(198, 22)
        Me.RemoveLicensedFeatureToolStripMenuItem.Text = "Return selected"
        '
        'BorrowToolStripMenuItem
        '
        Me.BorrowToolStripMenuItem.Name = "BorrowToolStripMenuItem"
        Me.BorrowToolStripMenuItem.Size = New System.Drawing.Size(198, 22)
        Me.BorrowToolStripMenuItem.Text = "Borrow selected feature"
        '
        'UltraActivityIndicator1
        '
        Me.UltraActivityIndicator1.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.UltraActivityIndicator1.AnimationEnabled = True
        Me.UltraActivityIndicator1.AnimationSpeed = 25
        Me.UltraActivityIndicator1.CausesValidation = True
        Me.UltraActivityIndicator1.Location = New System.Drawing.Point(81, 147)
        Me.UltraActivityIndicator1.MarqueeAnimationStyle = Infragistics.Win.UltraActivityIndicator.MarqueeAnimationStyle.BounceBack
        Me.UltraActivityIndicator1.Name = "UltraActivityIndicator1"
        Me.UltraActivityIndicator1.Size = New System.Drawing.Size(355, 23)
        Me.UltraActivityIndicator1.TabIndex = 2
        Me.UltraActivityIndicator1.TabStop = True
        '
        'ServerLicensesDialog
        '
        Me.AcceptButton = Me.OK_Button
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(507, 363)
        Me.Controls.Add(Me.UltraActivityIndicator1)
        Me.Controls.Add(Me.ListView1)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ServerLicensesDialog"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Feature status view"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents OK_Button As System.Windows.Forms.Button
    Friend WithEvents ListView1 As ListView
    Friend WithEvents ColumnFeature As ColumnHeader
    Friend WithEvents ColumnVersion As ColumnHeader
    Friend WithEvents ColumnLicCount As ColumnHeader
    Friend WithEvents UltraActivityIndicator1 As Infragistics.Win.UltraActivityIndicator.UltraActivityIndicator
    Friend WithEvents ColumnVendor As ColumnHeader
    Friend WithEvents ColumnExpires As ColumnHeader
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents RemoveLicensedFeatureToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents BorrowToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents Refresh_Button As Button
End Class
