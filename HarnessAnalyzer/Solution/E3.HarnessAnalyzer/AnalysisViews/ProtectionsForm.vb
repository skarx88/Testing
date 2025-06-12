Imports Infragistics.Win.UltraWinListView

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class ProtectionsForm
    Inherits AnalysisForm
    Private _protectionMapper As Dictionary(Of String, List(Of String))

    Public Sub New()
        MyBase.New
        InitializeComponent()
    End Sub

    Public Sub New(doc As DocumentForm)
        MyBase.New(doc)

        InitializeComponent()
        InitializeProtections()
    End Sub

    Private Sub InitializeProtections()
        Me.Text = AnalysisFormStrings.ShowProtections_Caption

        _protectionMapper = New Dictionary(Of String, List(Of String))

        For Each seg As Segment In Kbl.GetSegments
            For Each pA As Protection_area In seg.Protection_area
                Dim wProtOcc As Wire_protection_occurrence = Kbl.GetWireProtectionOccurrence(pA.Associated_protection)
                Dim wProt As Wire_protection = Kbl.GetWireProtection(wProtOcc.Part)

                If _protectionMapper.AddOrUpdate(wProt.SystemId, seg.SystemId) = AddOrUpdateResult.Added Then
                    Dim newItem As New UltraListViewItem(wProt.Part_number, New Object() {wProt.Abbreviation, wProt.Protection_type, wProt.Description})

                    newItem.Key = wProt.SystemId
                    ulvProtections.Items.Add(newItem)
                End If
            Next
        Next

        Me.ugbProtections.Visible = True
    End Sub

    Private Sub ulvProtections_ItemSelectionChanged(sender As Object, e As ItemSelectionChangedEventArgs) Handles ulvProtections.ItemSelectionChanged
        Me.btnView.Enabled = e.SelectedItems.Count > 0
    End Sub

    Private Sub ulvProtections_ItemDoubleClick(sender As Object, e As ItemDoubleClickEventArgs) Handles ulvProtections.ItemDoubleClick
        btnView.PerformClick()
    End Sub

    Private Sub btnView_Click(sender As Object, e As EventArgs) Handles btnView.Click
        ActiveObjects.Clear()
        ActiveObjects.AddRange(_protectionMapper(Me.ulvProtections.SelectedItems.Single.Key))
        MyBase.btnViewClicked()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.Close()
        'MyBase.btnCancelClicked()
    End Sub

    Private Sub ProtectionsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.AnalysisShowProtections
    End Sub
End Class