<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class PlatingMaterialForm
    Inherits AnalysisForm
    Private _platingMaterialMapper As Dictionary(Of String, List(Of String))

    Public Sub New()
        MyBase.New

        InitializeComponent()
    End Sub

    Public Sub New(doc As DocumentForm)
        MyBase.New(doc)

        InitializeComponent()
        Me.Icon = My.Resources.AnalysisShowPlatingMat
        InitializePlatingMaterials()
    End Sub

    Private Sub InitializePlatingMaterials()
        Me.Text = AnalysisFormStrings.ShowPlatMat_Caption

        _platingMaterialMapper = New Dictionary(Of String, List(Of String))

        For Each connectorOccurrence As Connector_occurrence In Kbl.GetConnectorOccurrences
            If (connectorOccurrence.Slots IsNot Nothing) AndAlso (connectorOccurrence.Slots.Length <> 0) Then
                For Each cavityOccurrence As Cavity_occurrence In connectorOccurrence.Slots(0).Cavities
                    Dim terminalPart As General_terminal = Nothing

                    For Each contactPoint As Contact_point In connectorOccurrence.Contact_points
                        If (contactPoint.Contacted_cavity.SplitSpace.Contains(cavityOccurrence.SystemId)) AndAlso (contactPoint.Associated_parts IsNot Nothing) Then
                            For Each associatePart As String In contactPoint.Associated_parts.SplitSpace
                                If (Kbl.GetSpecialTerminalOccurrence(associatePart) IsNot Nothing) Then
                                    terminalPart = Kbl.GetGeneralTerminal(Kbl.GetSpecialTerminalOccurrence(associatePart).Part)
                                ElseIf (Kbl.GetTerminalOccurrence(associatePart) IsNot Nothing) Then
                                    terminalPart = Kbl.GetGeneralTerminal(Kbl.GetTerminalOccurrence(associatePart).Part)
                                End If

                                If (terminalPart IsNot Nothing) AndAlso (terminalPart.Plating_material IsNot Nothing) AndAlso (terminalPart.Plating_material <> String.Empty) Then
                                    If (Not _platingMaterialMapper.ContainsKey(terminalPart.Plating_material)) Then _platingMaterialMapper.Add(terminalPart.Plating_material, New List(Of String))
                                    If (Not _platingMaterialMapper(terminalPart.Plating_material).Contains(connectorOccurrence.SystemId)) Then _platingMaterialMapper(terminalPart.Plating_material).Add(connectorOccurrence.SystemId)
                                End If
                            Next

                            Exit For
                        End If
                    Next
                Next
            End If
        Next

        Me.ugbPlatingMaterial.Visible = True

        For Each platingMaterial As String In _platingMaterialMapper.Keys
            Me.ucePlatingMaterial.Items.Add(platingMaterial)
        Next

        Me.ucePlatingMaterial.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending
    End Sub

    Private Sub ucePlatingMaterial_SelectionChanged(sender As Object, e As EventArgs) Handles ucePlatingMaterial.SelectionChanged
        Me.btnView.Enabled = True
    End Sub

    Private Sub btnView_Click(sender As Object, e As EventArgs) Handles btnView.Click
        ActiveObjects.Clear()
        ActiveObjects.AddRange(_platingMaterialMapper(Me.ucePlatingMaterial.SelectedItem.DisplayText))
        MyBase.btnViewClicked()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.Close()
        'MyBase.btnCancelClicked()
    End Sub

    Private Sub PlatingMaterialForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.AnalysisShowPlatingMat
    End Sub

End Class