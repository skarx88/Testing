Imports System.Drawing
Imports System.Windows.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class PreviewControl
    Inherits System.Windows.Forms.UserControl

    'UserControl überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If

            If disposing AndAlso _swapViewer IsNot Nothing Then
                _swapViewer.Dispose()
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
        Me.PanelTop = New System.Windows.Forms.Panel()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.chkJT = New System.Windows.Forms.CheckBox()
        Me.chkTopo3D = New System.Windows.Forms.CheckBox()
        Me.chkTopo2D = New System.Windows.Forms.CheckBox()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.GroupBox1.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelTop
        '
        Me.PanelTop.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PanelTop.Location = New System.Drawing.Point(3, 3)
        Me.PanelTop.Name = "PanelTop"
        Me.PanelTop.Size = New System.Drawing.Size(497, 369)
        Me.PanelTop.TabIndex = 0
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.TableLayoutPanel2)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox1.Location = New System.Drawing.Point(3, 378)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(497, 105)
        Me.GroupBox1.TabIndex = 1
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Details"
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.ColumnCount = 2
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.Controls.Add(Me.chkJT, 1, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.chkTopo3D, 0, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.chkTopo2D, 0, 1)
        Me.TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(3, 16)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 2
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(491, 86)
        Me.TableLayoutPanel2.TabIndex = 2
        '
        'chkJT
        '
        Me.chkJT.AutoSize = True
        Me.chkJT.Dock = System.Windows.Forms.DockStyle.Fill
        Me.chkJT.Enabled = False
        Me.chkJT.Location = New System.Drawing.Point(117, 3)
        Me.chkJT.Margin = New System.Windows.Forms.Padding(10, 3, 10, 3)
        Me.chkJT.Name = "chkJT"
        Me.chkJT.Size = New System.Drawing.Size(364, 37)
        Me.chkJT.TabIndex = 2
        Me.chkJT.Text = "JT"
        Me.chkJT.UseVisualStyleBackColor = True
        '
        'chkTopo3D
        '
        Me.chkTopo3D.AutoSize = True
        Me.chkTopo3D.Dock = System.Windows.Forms.DockStyle.Fill
        Me.chkTopo3D.Enabled = False
        Me.chkTopo3D.Location = New System.Drawing.Point(10, 3)
        Me.chkTopo3D.Margin = New System.Windows.Forms.Padding(10, 3, 10, 3)
        Me.chkTopo3D.Name = "chkTopo3D"
        Me.chkTopo3D.Size = New System.Drawing.Size(87, 37)
        Me.chkTopo3D.TabIndex = 0
        Me.chkTopo3D.Text = "3D Topology"
        Me.chkTopo3D.UseVisualStyleBackColor = True
        '
        'chkTopo2D
        '
        Me.chkTopo2D.AutoSize = True
        Me.chkTopo2D.Dock = System.Windows.Forms.DockStyle.Fill
        Me.chkTopo2D.Enabled = False
        Me.chkTopo2D.Location = New System.Drawing.Point(10, 46)
        Me.chkTopo2D.Margin = New System.Windows.Forms.Padding(10, 3, 10, 3)
        Me.chkTopo2D.Name = "chkTopo2D"
        Me.chkTopo2D.Size = New System.Drawing.Size(87, 37)
        Me.chkTopo2D.TabIndex = 1
        Me.chkTopo2D.Text = "2D Topology"
        Me.chkTopo2D.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.GroupBox1, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.PanelTop, 0, 0)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 2
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(503, 486)
        Me.TableLayoutPanel1.TabIndex = 2
        '
        'PreviewControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Name = "PreviewControl"
        Me.Size = New System.Drawing.Size(503, 486)
        Me.GroupBox1.ResumeLayout(False)
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.TableLayoutPanel2.PerformLayout()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PanelTop As Panel
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents chkTopo2D As CheckBox
    Friend WithEvents chkTopo3D As CheckBox
    Friend WithEvents TableLayoutPanel2 As TableLayoutPanel
    Friend WithEvents chkJT As CheckBox
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
End Class
