Namespace D3D

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class FaderCntrl
        Inherits System.Windows.Forms.UserControl

        'UserControl überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
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
            lblReference = New Label()
            lblCompare = New Label()
            TrackBar1 = New TrackBar()
            TableLayoutPanel1 = New TableLayoutPanel()
            CType(TrackBar1, ComponentModel.ISupportInitialize).BeginInit()
            TableLayoutPanel1.SuspendLayout()
            SuspendLayout()
            ' 
            ' lblReference
            ' 
            lblReference.AutoSize = True
            lblReference.BackColor = Color.Transparent
            lblReference.Dock = DockStyle.Fill
            lblReference.Location = New Point(2, 2)
            lblReference.Margin = New Padding(2)
            lblReference.Name = "lblReference"
            lblReference.Size = New Size(59, 45)
            lblReference.TabIndex = 0
            lblReference.Text = "Reference"
            lblReference.TextAlign = ContentAlignment.MiddleCenter
            ' 
            ' lblCompare
            ' 
            lblCompare.AutoSize = True
            lblCompare.BackColor = Color.Transparent
            lblCompare.Dock = DockStyle.Fill
            lblCompare.Location = New Point(255, 2)
            lblCompare.Margin = New Padding(2)
            lblCompare.Name = "lblCompare"
            lblCompare.Size = New Size(56, 45)
            lblCompare.TabIndex = 1
            lblCompare.Text = "Compare"
            lblCompare.TextAlign = ContentAlignment.MiddleCenter
            ' 
            ' TrackBar1
            ' 
            TrackBar1.Anchor = AnchorStyles.Left Or AnchorStyles.Right
            TrackBar1.BackColor = Color.White
            TrackBar1.LargeChange = 100
            TrackBar1.Location = New Point(65, 2)
            TrackBar1.Margin = New Padding(2)
            TrackBar1.Maximum = 100
            TrackBar1.Minimum = -100
            TrackBar1.Name = "TrackBar1"
            TrackBar1.Size = New Size(186, 45)
            TrackBar1.SmallChange = 20
            TrackBar1.TabIndex = 2
            TrackBar1.TickFrequency = 10
            TrackBar1.TickStyle = TickStyle.None
            ' 
            ' TableLayoutPanel1
            ' 
            TableLayoutPanel1.ColumnCount = 3
            TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle())
            TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100F))
            TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle())
            TableLayoutPanel1.Controls.Add(TrackBar1, 1, 0)
            TableLayoutPanel1.Controls.Add(lblCompare, 2, 0)
            TableLayoutPanel1.Controls.Add(lblReference, 0, 0)
            TableLayoutPanel1.Dock = DockStyle.Fill
            TableLayoutPanel1.Location = New Point(0, 0)
            TableLayoutPanel1.Name = "TableLayoutPanel1"
            TableLayoutPanel1.RowCount = 1
            TableLayoutPanel1.RowStyles.Add(New RowStyle(SizeType.Percent, 100F))
            TableLayoutPanel1.Size = New Size(313, 49)
            TableLayoutPanel1.TabIndex = 3
            ' 
            ' FaderCntrl
            ' 
            AutoScaleDimensions = New SizeF(7F, 15F)
            AutoScaleMode = AutoScaleMode.Font
            BackColor = Color.Transparent
            Controls.Add(TableLayoutPanel1)
            Margin = New Padding(0)
            Name = "FaderCntrl"
            RightToLeft = RightToLeft.No
            Size = New Size(313, 49)
            CType(TrackBar1, ComponentModel.ISupportInitialize).EndInit()
            TableLayoutPanel1.ResumeLayout(False)
            TableLayoutPanel1.PerformLayout()
            ResumeLayout(False)

        End Sub

        Friend WithEvents lblReference As Label
        Friend WithEvents lblCompare As Label
        Friend WithEvents TrackBar1 As TrackBar
        Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    End Class

End Namespace