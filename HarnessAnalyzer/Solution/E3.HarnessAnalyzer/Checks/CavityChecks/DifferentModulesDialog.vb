Imports Infragistics.Win.UltraWinListView

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class DifferentModulesDialog

    Private _documentColumnText As String

    Public Sub New()
        InitializeComponent()

        With ulvDiffModules
            _documentColumnText = .SubItemColumns("DocumentColumn").Text
            .MainColumn.SortComparer = New Checks.Cavities.ModulesNameSortComparer
        End With
    End Sub

    Private Sub Yes_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Recent_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Yes
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Current_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.No
        Me.Close()
    End Sub

    Public Shadows Function ShowDialog(owner As IWin32Window, modelView As Checks.Cavities.Views.Model.ModelView) As DialogResult
        Try
            ulvDiffModules.BeginUpdate()
            Me.ulvDiffModules.Items.Clear()

            For Each m As [Module] In modelView.GetModules
                Dim settingsActive As Boolean = modelView.Settings.ActiveModules.Contains(m.SystemId)
                Dim documentActive As Boolean = Not modelView.CurrentInactiveModules.ContainsKey(m.SystemId)
                Dim newItem As New UltraListViewItem()
                newItem.Value = ModulesHub.GetNodeText(m.Abbreviation, m.Part_number)
                ulvDiffModules.Items.Add(newItem)
                newItem.SubItems("DocumentColumn").Value = documentActive
                newItem.SubItems("SettingColumn").Value = settingsActive
            Next

            With Me.ulvDiffModules.SubItemColumns("DocumentColumn")
                .PerformAutoResize(autoSizeMode:=ColumnAutoSizeMode.AllItemsAndHeader)
            End With

            ulvDiffModules.EndUpdate(True)
        Catch ex As Exception
            MessageBoxEx.ShowError(ex.GetInnerOrDefaultMessage)
            Return DialogResult.None
        End Try

        Return MyBase.ShowDialog(owner)
    End Function


End Class
