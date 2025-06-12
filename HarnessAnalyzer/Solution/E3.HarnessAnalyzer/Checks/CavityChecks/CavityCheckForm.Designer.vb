Namespace Checks.Cavities

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class CavityCheckForm
        Inherits System.Windows.Forms.Form

        'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If

                If disposing Then
                    If _documents IsNot Nothing Then
                        _documents.Clear()
                    End If
                End If

                _documents = Nothing
                _MainForm = Nothing
                Me.Icon = Nothing
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(CavityCheckForm))
            Dim UltraToolTipInfo1 As Infragistics.Win.UltraWinToolTip.UltraToolTipInfo = New Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Alle geprüften Inhalte zurücksetzen", Infragistics.Win.ToolTipImage.[Default], Nothing, Infragistics.Win.DefaultableBoolean.[Default])
            Me.CavityCheckForm_Fill_Panel = New Infragistics.Win.Misc.UltraPanel()
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
            Me.btnReset = New Infragistics.Win.Misc.UltraButton()
            Me.ButtonsTableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
            Me.btnClose = New Infragistics.Win.Misc.UltraButton()
            Me.CavityNavigator = New Zuken.E3.HarnessAnalyzer.Checks.Cavities.CavityNavigator()
            Me.UltraToolTipManager1 = New Infragistics.Win.UltraWinToolTip.UltraToolTipManager(Me.components)
            Me.CavityCheckForm_Fill_Panel.ClientArea.SuspendLayout()
            Me.CavityCheckForm_Fill_Panel.SuspendLayout()
            Me.TableLayoutPanel1.SuspendLayout()
            Me.ButtonsTableLayoutPanel.SuspendLayout()
            Me.SuspendLayout()
            '
            'CavityCheckForm_Fill_Panel
            '
            resources.ApplyResources(Me.CavityCheckForm_Fill_Panel, "CavityCheckForm_Fill_Panel")
            '
            'CavityCheckForm_Fill_Panel.ClientArea
            '
            resources.ApplyResources(Me.CavityCheckForm_Fill_Panel.ClientArea, "CavityCheckForm_Fill_Panel.ClientArea")
            Me.CavityCheckForm_Fill_Panel.ClientArea.Controls.Add(Me.TableLayoutPanel1)
            Me.CavityCheckForm_Fill_Panel.ClientArea.Controls.Add(Me.ButtonsTableLayoutPanel)
            Me.CavityCheckForm_Fill_Panel.ClientArea.Controls.Add(Me.CavityNavigator)
            Me.CavityCheckForm_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default
            Me.CavityCheckForm_Fill_Panel.Name = "CavityCheckForm_Fill_Panel"
            '
            'TableLayoutPanel1
            '
            resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
            Me.TableLayoutPanel1.Controls.Add(Me.btnReset, 0, 0)
            Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
            '
            'btnReset
            '
            resources.ApplyResources(Me.btnReset, "btnReset")
            Me.btnReset.Name = "btnReset"
            resources.ApplyResources(UltraToolTipInfo1, "UltraToolTipInfo1")
            Me.UltraToolTipManager1.SetUltraToolTip(Me.btnReset, UltraToolTipInfo1)
            '
            'ButtonsTableLayoutPanel
            '
            resources.ApplyResources(Me.ButtonsTableLayoutPanel, "ButtonsTableLayoutPanel")
            Me.ButtonsTableLayoutPanel.Controls.Add(Me.btnClose, 2, 0)
            Me.ButtonsTableLayoutPanel.Name = "ButtonsTableLayoutPanel"
            '
            'btnClose
            '
            resources.ApplyResources(Me.btnClose, "btnClose")
            Me.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.btnClose.Name = "btnClose"
            '
            'CavityNavigator
            '
            resources.ApplyResources(Me.CavityNavigator, "CavityNavigator")
            Me.CavityNavigator.Name = "CavityNavigator"
            '
            'UltraToolTipManager1
            '
            Me.UltraToolTipManager1.ContainingControl = Me
            Me.UltraToolTipManager1.DisplayStyle = Infragistics.Win.ToolTipDisplayStyle.Standard
            '
            'CavityCheckForm
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
            Me.CancelButton = Me.btnClose
            Me.Controls.Add(Me.CavityCheckForm_Fill_Panel)
            Me.KeyPreview = True
            Me.Name = "CavityCheckForm"
            Me.CavityCheckForm_Fill_Panel.ClientArea.ResumeLayout(False)
            Me.CavityCheckForm_Fill_Panel.ResumeLayout(False)
            Me.TableLayoutPanel1.ResumeLayout(False)
            Me.ButtonsTableLayoutPanel.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub

        Friend WithEvents CavityNavigator As CavityNavigator
        Friend WithEvents CavityCheckForm_Fill_Panel As Infragistics.Win.Misc.UltraPanel
        Friend WithEvents ButtonsTableLayoutPanel As TableLayoutPanel
        Friend WithEvents btnClose As Infragistics.Win.Misc.UltraButton
        Friend WithEvents btnReset As Infragistics.Win.Misc.UltraButton
        Friend WithEvents UltraToolTipManager1 As Infragistics.Win.UltraWinToolTip.UltraToolTipManager
        Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    End Class

End Namespace