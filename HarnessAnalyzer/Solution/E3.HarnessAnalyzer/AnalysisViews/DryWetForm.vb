<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class DryWetForm
    Inherits AnalysisForm

    Private _dryWetMapper As Dictionary(Of String, List(Of String))

    Public Sub New()
        MyBase.New
        InitializeComponent()
    End Sub

    Public Sub New(doc As DocumentForm)
        MyBase.New(doc)

        InitializeComponent()
        Init()
    End Sub

    Private Sub Init()

        Me.Icon = My.Resources.AnalysisShowDryWet
        Me.Text = AnalysisFormStrings.ShowEnv_Caption
        BackColor = Color.White

        Me.btnView.Enabled = True
        Me.ugbEnvironmentSetting.Visible = True
        Me.uosEnvironmentSetting.CheckedIndex = 0

        _dryWetMapper = New Dictionary(Of String, List(Of String)) From {
            {"dry", New List(Of String)},
            {"wet", New List(Of String)}
        }

        For Each node As Node In Kbl.GetNodes
            If (node.Referenced_components IsNot Nothing) AndAlso (node.Referenced_components <> String.Empty) Then
                For Each refComponent As String In node.Referenced_components.SplitSpace
                    If (Kbl.KBLOccurrenceMapper.ContainsKey(refComponent)) AndAlso (TypeOf Kbl.KBLOccurrenceMapper(refComponent) Is Connector_occurrence) Then
                        Dim connector As Connector_occurrence = DirectCast(Kbl.KBLOccurrenceMapper(refComponent), Connector_occurrence)
                        If (connector.Slots IsNot Nothing) AndAlso (connector.Slots.Length <> 0) Then
                            Dim hasPlugOrSealPart As Boolean = False

                            For Each cavity As Cavity_occurrence In connector.Slots(0).Cavities
                                If (Kbl.KBLCavityContactPointMapper.ContainsKey(cavity.SystemId)) Then
                                    For Each contactPoint As Contact_point In Kbl.KBLCavityContactPointMapper(cavity.SystemId)
                                        If (contactPoint.Associated_parts IsNot Nothing) AndAlso (contactPoint.Associated_parts <> String.Empty) Then
                                            For Each associatedPart As String In contactPoint.Associated_parts.SplitSpace
                                                If (Kbl.KBLOccurrenceMapper.ContainsKey(associatedPart)) AndAlso (TypeOf Kbl.KBLOccurrenceMapper(associatedPart) Is Cavity_seal_occurrence) Then
                                                    hasPlugOrSealPart = True

                                                    Exit For
                                                End If
                                            Next

                                            If (hasPlugOrSealPart) Then Exit For
                                        End If
                                    Next
                                ElseIf (cavity.Associated_plug IsNot Nothing) AndAlso (cavity.Associated_plug <> String.Empty) Then
                                    hasPlugOrSealPart = True
                                End If

                                If (hasPlugOrSealPart) Then Exit For
                            Next

                            If (hasPlugOrSealPart) Then
                                If (Not _dryWetMapper("wet").Contains(node.SystemId)) Then _dryWetMapper("wet").Add(node.SystemId)
                                If (Not _dryWetMapper("wet").Contains(connector.SystemId)) Then _dryWetMapper("wet").Add(connector.SystemId)
                            Else
                                If (Not _dryWetMapper("dry").Contains(node.SystemId)) Then _dryWetMapper("dry").Add(node.SystemId)
                                If (Not _dryWetMapper("dry").Contains(connector.SystemId)) Then _dryWetMapper("dry").Add(connector.SystemId)
                            End If
                        End If
                    End If
                Next
            End If
        Next
    End Sub
    Private Sub BtnView_Click(sender As Object, e As EventArgs) Handles btnView.Click
        ActiveObjects.Clear()
        ActiveObjects.AddRange(_dryWetMapper(Me.uosEnvironmentSetting.Items(Me.uosEnvironmentSetting.CheckedIndex).DataValue.ToString))
        MyBase.btnViewClicked()
    End Sub
    Private Sub BtnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
        'MyBase.btnCancelClicked()
    End Sub

    Private Sub DryWetForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.AnalysisShowDryWet
    End Sub

End Class