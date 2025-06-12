<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DocumentFilterControl
    Inherits System.Windows.Forms.UserControl

    'UserControl überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If

            _project = Nothing
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
        Me.ulvDrawings = New Infragistics.Win.UltraWinListView.UltraListView()
        CType(Me.ulvDrawings, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ulvDrawings
        '
        Me.ulvDrawings.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ulvDrawings.Location = New System.Drawing.Point(0, 0)
        Me.ulvDrawings.MainColumn.Key = "Name"
        Me.ulvDrawings.Name = "ulvDrawings"
        Me.ulvDrawings.Size = New System.Drawing.Size(191, 177)
        Me.ulvDrawings.TabIndex = 0
        Me.ulvDrawings.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.List
        Me.ulvDrawings.ViewSettingsList.CheckBoxStyle = Infragistics.Win.UltraWinListView.CheckBoxStyle.CheckBox
        '
        'DocumentFilterControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.ulvDrawings)
        Me.Name = "DocumentFilterControl"
        Me.Size = New System.Drawing.Size(191, 177)
        CType(Me.ulvDrawings, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ulvDrawings As Infragistics.Win.UltraWinListView.UltraListView

End Class
