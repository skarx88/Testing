Namespace D3D.Consolidated.Controls
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class CarModelsViewControl
        Inherits System.Windows.Forms.UserControl

        'UserControl überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                    _popUpTimer.Dispose()
                End If

                _carStateMachine = Nothing
                'HINT: remove all items from
                _asyncDirectoryLoadingStack.TryPopAll(Nothing)
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
            Me.components = New System.ComponentModel.Container()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(CarModelsViewControl))
            Me.UltraListView1 = New Infragistics.Win.UltraWinListView.UltraListView()
            Me.UltraToolTipManager1 = New Infragistics.Win.UltraWinToolTip.UltraToolTipManager(Me.components)
            Me.btnChangeCarModelPath = New System.Windows.Forms.Button()
            Me.changeCarModelPathPanel = New System.Windows.Forms.Panel()
            CType(Me.UltraListView1, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.changeCarModelPathPanel.SuspendLayout()
            Me.SuspendLayout()
            '
            'UltraListView1
            '
            resources.ApplyResources(Me.UltraListView1, "UltraListView1")
            Me.UltraListView1.ItemSettings.SelectionType = Infragistics.Win.UltraWinListView.SelectionType.[Single]
            Me.UltraListView1.Name = "UltraListView1"
            Me.UltraListView1.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.Details
            Me.UltraListView1.ViewSettingsDetails.AutoFitColumns = Infragistics.Win.UltraWinListView.AutoFitColumns.ResizeAllColumns
            Me.UltraListView1.ViewSettingsDetails.CheckBoxStyle = Infragistics.Win.UltraWinListView.CheckBoxStyle.CheckBox
            Me.UltraListView1.ViewSettingsDetails.ColumnAutoSizeMode = CType((Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.Header Or Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.AllItems), Infragistics.Win.UltraWinListView.ColumnAutoSizeMode)
            Me.UltraListView1.ViewSettingsDetails.FullRowSelect = True
            '
            'UltraToolTipManager1
            '
            Me.UltraToolTipManager1.ContainingControl = Me
            Me.UltraToolTipManager1.Enabled = False
            '
            'btnChangeCarModelPath
            '
            resources.ApplyResources(Me.btnChangeCarModelPath, "btnChangeCarModelPath")
            Me.btnChangeCarModelPath.Name = "btnChangeCarModelPath"
            Me.btnChangeCarModelPath.UseVisualStyleBackColor = True
            '
            'changeCarModelPathPanel
            '
            Me.changeCarModelPathPanel.Controls.Add(Me.btnChangeCarModelPath)
            resources.ApplyResources(Me.changeCarModelPathPanel, "changeCarModelPathPanel")
            Me.changeCarModelPathPanel.Name = "changeCarModelPathPanel"
            '
            'CarModelsViewControl
            '
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
            Me.Controls.Add(Me.changeCarModelPathPanel)
            Me.Controls.Add(Me.UltraListView1)
            Me.Name = "CarModelsViewControl"
            resources.ApplyResources(Me, "$this")
            CType(Me.UltraListView1, System.ComponentModel.ISupportInitialize).EndInit()
            Me.changeCarModelPathPanel.ResumeLayout(False)
            Me.changeCarModelPathPanel.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents UltraListView1 As Infragistics.Win.UltraWinListView.UltraListView
        Friend WithEvents UltraToolTipManager1 As Infragistics.Win.UltraWinToolTip.UltraToolTipManager
        Friend WithEvents btnChangeCarModelPath As Button
        Friend WithEvents changeCarModelPathPanel As Panel
    End Class
End Namespace