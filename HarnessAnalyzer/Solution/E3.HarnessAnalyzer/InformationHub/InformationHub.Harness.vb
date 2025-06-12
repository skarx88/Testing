Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinGrid
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Partial Public Class InformationHub

    Private Sub InitializeHarness()
        Dim compareHarness As Harness = Nothing

        If (_kblMapperForCompare IsNot Nothing) Then
            compareHarness = _kblMapperForCompare.GetHarness
        End If

        With Me.udsHarness
            With .Band
                .Columns.Add(NameOf(InformationHubStrings.PropName_ColumnCaption))
                .Columns.Add(NameOf(InformationHubStrings.PropVal_ColumnCaption))

                .Key = "Harness"
            End With

            Dim row As UltraDataRow

            With _kblMapper.GetHarness
                row = udsHarness.Rows.Add
                row.SetCellValue(NameOf(InformationHubStrings.PropName_ColumnCaption), InformationHubStrings.PartNumber_Caption)
                row.SetCellValue(NameOf(InformationHubStrings.PropVal_ColumnCaption), .Part_number)

                If (compareHarness IsNot Nothing) AndAlso (.Part_number <> compareHarness.Part_number) Then
                    row.Tag = New KeyValuePair(Of String, Object)(PartPropertyName.Part_number, compareHarness.Part_number)
                End If

                row = udsHarness.Rows.Add
                row.SetCellValue(NameOf(InformationHubStrings.PropName_ColumnCaption), InformationHubStrings.Description_Caption)
                row.SetCellValue(NameOf(InformationHubStrings.PropVal_ColumnCaption), .Description)

                If (compareHarness IsNot Nothing) AndAlso (.Description <> compareHarness.Description) Then
                    row.Tag = New KeyValuePair(Of String, Object)(PartPropertyName.Part_description, compareHarness.Description)
                End If

                row = udsHarness.Rows.Add
                row.SetCellValue(NameOf(InformationHubStrings.PropName_ColumnCaption), InformationHubStrings.LocDescription_Caption)
                If (.Localized_description IsNot Nothing AndAlso .Localized_description.Length <> 0) Then
                    If (.Localized_description.Length = 1) AndAlso (.Localized_description(0).Value Is Nothing) Then
                        row.SetCellValue(NameOf(InformationHubStrings.PropVal_ColumnCaption), .Localized_description(0).SystemId)
                    Else
                        row.SetCellValue(NameOf(InformationHubStrings.PropVal_ColumnCaption), Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS)
                    End If
                End If
                If (compareHarness IsNot Nothing) Then
                    Dim listConvertToDictionary As New ListConvertToDictionary(If(.Localized_description Is Nothing, Array.Empty(Of Localized_string), .Localized_description), If(compareHarness.Localized_description Is Nothing, Array.Empty(Of Localized_string), compareHarness.Localized_description))
                    Dim locDescriptionComparer As New LocalizedDescriptionComparisonMapper(Nothing, _kblMapper, _kblMapperForCompare, Nothing, Nothing, listConvertToDictionary, _generalSettings)
                    locDescriptionComparer.CompareObjects()

                    If (locDescriptionComparer.HasChanges) Then
                        row.Tag = New KeyValuePair(Of String, Object)(PartPropertyName.Part_Localized_description, locDescriptionComparer)
                    End If
                End If

                If (compareHarness Is Nothing) Then
                    row = udsHarness.Rows.Add
                    row.SetCellValue(NameOf(InformationHubStrings.PropName_ColumnCaption), ObjectPropertyNameStrings.ZGS)
                    If (.Change.Length <> 0) Then
                        Dim changes As List(Of Change) = .Change.Where(Function(change) change.Change_date IsNot Nothing AndAlso DateTime.TryParse(change.Change_date, New DateTime)).ToList
                        If (changes.Count <> 0) Then
                            row.SetCellValue(NameOf(InformationHubStrings.PropVal_ColumnCaption), changes.Where(Function(change) CDate(change.Change_date).Ticks = changes.Max(Function(cng) CDate(cng.Change_date).Ticks)).LastOrDefault.Id)
                        Else
                            row.SetCellValue(NameOf(InformationHubStrings.PropVal_ColumnCaption), .Change.LastOrDefault.Id)
                        End If
                    End If

                    row = udsHarness.Rows.Add
                    row.SetCellValue(NameOf(InformationHubStrings.PropName_ColumnCaption), ObjectPropertyNameStrings.KEM)

                    If (.Change.Length <> 0) Then
                        Dim changes As List(Of Change) = .Change.Where(Function(change) change.Change_date IsNot Nothing AndAlso DateTime.TryParse(change.Change_date, New DateTime)).ToList
                        If (changes.Count <> 0) Then
                            row.SetCellValue(NameOf(InformationHubStrings.PropVal_ColumnCaption), changes.Where(Function(change) CDate(change.Change_date).Ticks = changes.Max(Function(cng) CDate(cng.Change_date).Ticks)).LastOrDefault.Change_request)
                        Else
                            row.SetCellValue(NameOf(InformationHubStrings.PropVal_ColumnCaption), .Change.LastOrDefault.Change_request)
                        End If
                    End If
                End If

                row = udsHarness.Rows.Add
                row.SetCellValue(NameOf(InformationHubStrings.PropName_ColumnCaption), InformationHubStrings.ModelYear_Caption)
                row.SetCellValue(NameOf(InformationHubStrings.PropVal_ColumnCaption), .Model_year)

                If (compareHarness IsNot Nothing) AndAlso (.Model_year <> compareHarness.Model_year) Then
                    row.Tag = New KeyValuePair(Of String, Object)("Model_year", compareHarness.Model_year)
                End If

                row = udsHarness.Rows.Add
                row.SetCellValue(NameOf(InformationHubStrings.PropName_ColumnCaption), InformationHubStrings.DegreeOfMaturity_Caption)

                If (.Degree_of_maturity IsNot Nothing) Then
                    row.SetCellValue(NameOf(InformationHubStrings.PropVal_ColumnCaption), .Degree_of_maturity)
                End If

                If (compareHarness IsNot Nothing) AndAlso (.Degree_of_maturity <> compareHarness.Degree_of_maturity) Then
                    row.Tag = New KeyValuePair(Of String, Object)(PartPropertyName.Degree_of_maturity.ToString, compareHarness.Degree_of_maturity)
                End If

                row = udsHarness.Rows.Add
                row.SetCellValue(NameOf(InformationHubStrings.PropName_ColumnCaption), InformationHubStrings.Version_Caption)
                row.SetCellValue(NameOf(InformationHubStrings.PropVal_ColumnCaption), .Version)

                If (compareHarness IsNot Nothing) AndAlso (.Version <> compareHarness.Version) Then
                    row.Tag = New KeyValuePair(Of String, Object)(PartPropertyName.Version.ToString, compareHarness.Version)
                End If

                row = udsHarness.Rows.Add
                row.SetCellValue(NameOf(InformationHubStrings.PropName_ColumnCaption), InformationHubStrings.CarClassLvl2_Caption)
                row.SetCellValue(NameOf(InformationHubStrings.PropVal_ColumnCaption), .Car_classification_level_2)

                If (compareHarness IsNot Nothing) AndAlso (.Car_classification_level_2 <> compareHarness.Car_classification_level_2) Then
                    row.Tag = New KeyValuePair(Of String, Object)("Car_classification_level_2", compareHarness.Car_classification_level_2)
                End If

                row = udsHarness.Rows.Add
                row.SetCellValue(NameOf(InformationHubStrings.PropName_ColumnCaption), InformationHubStrings.CarClassLvl3_Caption)
                row.SetCellValue(NameOf(InformationHubStrings.PropVal_ColumnCaption), .Car_classification_level_3)

                If (compareHarness IsNot Nothing) AndAlso (.Car_classification_level_3 <> compareHarness.Car_classification_level_3) Then
                    row.Tag = New KeyValuePair(Of String, Object)("Car_classification_level_3", compareHarness.Car_classification_level_3)
                End If

                row = udsHarness.Rows.Add
                row.SetCellValue(NameOf(InformationHubStrings.PropName_ColumnCaption), InformationHubStrings.CarClassLvl4_Caption)
                row.SetCellValue(NameOf(InformationHubStrings.PropVal_ColumnCaption), .Car_classification_level_4)

                If (compareHarness IsNot Nothing) AndAlso (.Car_classification_level_4 <> compareHarness.Car_classification_level_4) Then
                    row.Tag = New KeyValuePair(Of String, Object)("Car_classification_level_4", compareHarness.Car_classification_level_4)
                End If

                row = udsHarness.Rows.Add
                row.SetCellValue(NameOf(InformationHubStrings.PropName_ColumnCaption), InformationHubStrings.CompanyName_Caption)
                row.SetCellValue(NameOf(InformationHubStrings.PropVal_ColumnCaption), .Company_name)

                If (compareHarness IsNot Nothing) AndAlso (.Company_name <> compareHarness.Company_name) Then
                    row.Tag = New KeyValuePair(Of String, Object)(PartPropertyName.Company_name.ToString, compareHarness.Company_name)
                End If

                row = udsHarness.Rows.Add
                row.SetCellValue(NameOf(InformationHubStrings.PropName_ColumnCaption), InformationHubStrings.Change_Caption)
                If (.Change.Length <> 0) Then
                    row.SetCellValue(NameOf(InformationHubStrings.PropVal_ColumnCaption), Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS)
                End If

                If (compareHarness IsNot Nothing) Then
                    Dim listConvertToDictionary As New ListConvertToDictionary(If(.Change Is Nothing, New List(Of Change), .Change.ToList), If(compareHarness.Change Is Nothing, New List(Of Change), compareHarness.Change.ToList))
                    Dim changeComparisonMapper As New CommonComparisonMapper(Nothing, _kblMapper, _kblMapperForCompare, Nothing, Nothing, listConvertToDictionary, _generalSettings)
                    changeComparisonMapper.CompareObjects()

                    If (changeComparisonMapper.HasChanges) Then
                        row.Tag = New KeyValuePair(Of String, Object)(PartPropertyName.Change.ToString, changeComparisonMapper)
                    End If
                End If

                row = udsHarness.Rows.Add
                row.SetCellValue(NameOf(InformationHubStrings.PropName_ColumnCaption), InformationHubStrings.ExtReferences_Caption)
                If (.External_references IsNot Nothing) AndAlso (.External_references.SplitSpace.Length <> 0) Then
                    row.SetCellValue(NameOf(InformationHubStrings.PropVal_ColumnCaption), Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS)
                End If

                If (compareHarness IsNot Nothing) Then
                    Dim currentExternalReferences As New List(Of External_reference)
                    If (.External_references IsNot Nothing) Then
                        For Each externalReference As String In .External_references.ToString.SplitSpace
                            If (_kblMapper.KBLOccurrenceMapper.ContainsKey(externalReference)) Then
                                currentExternalReferences.Add(DirectCast(_kblMapper.KBLOccurrenceMapper(externalReference), External_reference))
                            End If
                        Next
                    End If

                    Dim compareExternalReferences As New List(Of External_reference)
                    If (compareHarness.External_references IsNot Nothing) Then
                        For Each externalReference As String In compareHarness.External_references.ToString.SplitSpace
                            If (_kblMapperForCompare.KBLOccurrenceMapper.ContainsKey(externalReference)) Then
                                compareExternalReferences.Add(DirectCast(_kblMapperForCompare.KBLOccurrenceMapper(externalReference), External_reference))
                            End If
                        Next
                    End If

                    Dim listConvertToDictionary As New ListConvertToDictionary(currentExternalReferences, compareExternalReferences)
                    Dim externalReferenceComparisonMapper As New CommonComparisonMapper(Nothing, _kblMapper, _kblMapperForCompare, Nothing, Nothing, listConvertToDictionary, _generalSettings)
                    externalReferenceComparisonMapper.CompareObjects()

                    If (externalReferenceComparisonMapper.HasChanges) Then
                        row.Tag = New KeyValuePair(Of String, Object)(PartPropertyName.External_references.ToString, externalReferenceComparisonMapper)
                    End If
                End If

                row = udsHarness.Rows.Add
                row.SetCellValue(NameOf(InformationHubStrings.PropName_ColumnCaption), InformationHubStrings.AliasIds_Caption)

                If (.Alias_id.Length <> 0) Then
                    If (.Alias_id.Length = 1) AndAlso (.Alias_id(0).Description Is Nothing) AndAlso (.Alias_id(0).Scope Is Nothing) Then
                        row.SetCellValue(NameOf(InformationHubStrings.PropVal_ColumnCaption), .Alias_id(0).Alias_id)
                    Else
                        row.SetCellValue(NameOf(InformationHubStrings.PropVal_ColumnCaption), Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS)
                    End If
                End If

                If (compareHarness IsNot Nothing) Then
                    Dim listConvertToDictionary As New ListConvertToDictionary(If(.Alias_id Is Nothing, New List(Of Alias_identification), .Alias_id.ToList), If(compareHarness.Alias_id Is Nothing, New List(Of Alias_identification), compareHarness.Alias_id.ToList))
                    Dim aliasIdComparisonMapper As New AliasIdComparisonMapper(Nothing, listConvertToDictionary, _generalSettings)
                    aliasIdComparisonMapper.CompareObjects()

                    If (aliasIdComparisonMapper.HasChanges) Then
                        row.Tag = New KeyValuePair(Of String, Object)(PartPropertyName.Part_alias_ids.ToString, aliasIdComparisonMapper)
                    End If
                End If

                row = udsHarness.Rows.Add
                row.SetCellValue(NameOf(InformationHubStrings.PropName_ColumnCaption), InformationHubStrings.Abbreviation_Caption)
                row.SetCellValue(NameOf(InformationHubStrings.PropVal_ColumnCaption), .Abbreviation)

                If (compareHarness IsNot Nothing) AndAlso (.Abbreviation <> compareHarness.Abbreviation) Then
                    row.Tag = New KeyValuePair(Of String, Object)(PartPropertyName.Abbreviation.ToString, compareHarness.Abbreviation)
                End If

                row = udsHarness.Rows.Add
                row.SetCellValue(NameOf(InformationHubStrings.PropName_ColumnCaption), InformationHubStrings.PredPartNumber_Caption)
                If (.Predecessor_part_number IsNot Nothing) Then
                    row.SetCellValue(NameOf(InformationHubStrings.PropVal_ColumnCaption), .Predecessor_part_number)
                End If

                If (compareHarness IsNot Nothing) AndAlso (.Predecessor_part_number <> compareHarness.Predecessor_part_number) Then
                    row.Tag = New KeyValuePair(Of String, Object)(PartPropertyName.Predecessor_part_number.ToString, compareHarness.Predecessor_part_number)
                End If

                row = udsHarness.Rows.Add
                row.SetCellValue(NameOf(InformationHubStrings.PropName_ColumnCaption), InformationHubStrings.CopyrightNote_Caption)

                If (.Copyright_note IsNot Nothing) Then
                    row.SetCellValue(NameOf(InformationHubStrings.PropVal_ColumnCaption), .Copyright_note)
                End If

                If (compareHarness IsNot Nothing) AndAlso (.Copyright_note <> compareHarness.Copyright_note) Then
                    row.Tag = New KeyValuePair(Of String, Object)(PartPropertyName.Copyright_note.ToString, compareHarness.Copyright_note)
                End If

                row = udsHarness.Rows.Add
                row.SetCellValue(NameOf(InformationHubStrings.PropName_ColumnCaption), InformationHubStrings.MassInfo_Caption)

                If (.Mass_information IsNot Nothing) Then
                    row.SetCellValue(NameOf(InformationHubStrings.PropVal_ColumnCaption), String.Format("{0} {1}", Math.Round(.Mass_information.Value_component, 3), _kblMapper.KBLUnitMapper(.Mass_information.Unit_component).Unit_name))
                End If

                If (compareHarness IsNot Nothing) AndAlso ((.Mass_information Is Nothing AndAlso compareHarness.Mass_information IsNot Nothing) OrElse (.Mass_information IsNot Nothing AndAlso compareHarness.Mass_information Is Nothing) OrElse (.Mass_information IsNot Nothing AndAlso compareHarness.Mass_information IsNot Nothing AndAlso Math.Round(.Mass_information.Value_component, 3) <> Math.Round(compareHarness.Mass_information.Value_component, 3))) Then
                    row.Tag = New KeyValuePair(Of String, Object)(PartPropertyName.Mass_information.ToString, compareHarness.Mass_information)
                End If

                row = udsHarness.Rows.Add
                row.SetCellValue(NameOf(InformationHubStrings.PropName_ColumnCaption), InformationHubStrings.PartNumberType_Caption)

                If (.Part_number_type IsNot Nothing) Then
                    row.SetCellValue(NameOf(InformationHubStrings.PropVal_ColumnCaption), .Part_number_type)
                End If

                If (compareHarness IsNot Nothing) AndAlso (.Part_number_type <> compareHarness.Part_number_type) Then
                    row.Tag = New KeyValuePair(Of String, Object)(PartPropertyName.Part_number_type.ToString, compareHarness.Part_number_type)
                End If

                row = udsHarness.Rows.Add
                row.SetCellValue(NameOf(InformationHubStrings.PropName_ColumnCaption), InformationHubStrings.MatInfo_Caption)

                If (.Material_information IsNot Nothing) Then
                    If (.Material_information.Material_reference_system Is Nothing) Then
                        row.SetCellValue(NameOf(InformationHubStrings.PropVal_ColumnCaption), .Material_information.Material_key)
                    Else
                        row.SetCellValue(NameOf(InformationHubStrings.PropVal_ColumnCaption), Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS)
                    End If
                End If

                If (compareHarness IsNot Nothing) AndAlso ((.Material_information Is Nothing AndAlso compareHarness.Material_information IsNot Nothing) OrElse (.Material_information IsNot Nothing AndAlso compareHarness.Material_information Is Nothing) OrElse (.Material_information IsNot Nothing AndAlso compareHarness.Material_information IsNot Nothing AndAlso .Material_information.Material_key <> compareHarness.Material_information.Material_key)) Then
                    row.Tag = New KeyValuePair(Of String, Object)(PartPropertyName.Material_information.ToString, compareHarness.Material_information)
                End If

                row = udsHarness.Rows.Add
                row.SetCellValue(NameOf(InformationHubStrings.PropName_ColumnCaption), InformationHubStrings.ProcInfo_Caption)

                If (.Processing_information.Length <> 0) Then
                    row.SetCellValue(NameOf(InformationHubStrings.PropVal_ColumnCaption), Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS)
                End If

                If (compareHarness IsNot Nothing) Then
                    Dim listConvertToDictionary As New ListConvertToDictionary(If(.Processing_information Is Nothing, New List(Of Processing_instruction), .Processing_information.ToList), If(compareHarness.Processing_information Is Nothing, New List(Of Processing_instruction), compareHarness.Processing_information.ToList))
                    Dim processingInfoComparisonMapper As New ProcessingInfoComparisonMapper(Nothing, _kblMapper, _kblMapperForCompare, Nothing, Nothing, listConvertToDictionary, _generalSettings)
                    processingInfoComparisonMapper.CompareObjects()

                    If (processingInfoComparisonMapper.HasChanges) Then
                        row.Tag = New KeyValuePair(Of String, Object)(PartPropertyName.Part_processing_information.ToString, processingInfoComparisonMapper)
                    End If
                End If

                row = udsHarness.Rows.Add
                row.SetCellValue(NameOf(InformationHubStrings.PropName_ColumnCaption), InformationHubStrings.PrjNumber_Caption)
                row.SetCellValue(NameOf(InformationHubStrings.PropVal_ColumnCaption), .Project_number)

                If (compareHarness IsNot Nothing) AndAlso (.Project_number <> compareHarness.Project_number) Then
                    row.Tag = New KeyValuePair(Of String, Object)("Project_number", compareHarness.Project_number)
                End If

                row = udsHarness.Rows.Add
                row.SetCellValue(NameOf(InformationHubStrings.PropName_ColumnCaption), InformationHubStrings.Content_Caption)
                row.SetCellValue(NameOf(InformationHubStrings.PropVal_ColumnCaption), .Content)

                If (compareHarness IsNot Nothing) AndAlso (.Content <> compareHarness.Content) Then
                    row.Tag = New KeyValuePair(Of String, Object)("Content", compareHarness.Content.ToString)
                End If
            End With
        End With
    End Sub

    Private Sub udsHarness_CellDataUpdated(sender As Object, e As CellDataUpdatedEventArgs) Handles udsHarness.CellDataUpdated
        RaiseEvent CellValueUpdated(sender, e)
    End Sub

    Private Sub ugHarness_ClickCellButton(sender As Object, e As CellEventArgs) Handles ugHarness.ClickCellButton
        If (_pressedMouseButton = System.Windows.Forms.MouseButtons.Left) Then
            If (TypeOf e.Cell.Tag Is KeyValuePair(Of String, Object)) Then
                RequestDialogCompareData(True, KblObjectType.Harness.ToLocalizedString, DirectCast(e.Cell.Tag, KeyValuePair(Of String, Object)))
            Else
                RequestDialogPartData(_kblMapper, ugHarness.Rows(e.Cell.Row.Index).Cells(0).Value.ToString, _kblMapper.GetHarness)
            End If
        End If
    End Sub

    Private Sub ugHarness_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugHarness.InitializeLayout
        Me.ugHarness.BeginUpdate()
        Me.ugHarness.EventManager.AllEventsEnabled = False

        InitializeGridLayout(Nothing, e.Layout)

        Me.ugHarness.EventManager.AllEventsEnabled = True
        Me.ugHarness.EndUpdate()
    End Sub

    Private Sub InitializeHarnessRowCore(row As UltraGridRow)
        For Each cell As UltraGridCell In row.Cells
            If (cell.Column.Index = 1) AndAlso (DirectCast(cell.Row.ListObject, UltraDataRow).Tag IsNot Nothing) Then
                ChangeModifiedCellAppearance(cell, DirectCast(cell.Row.ListObject, UltraDataRow).Tag)
                cell.Tag = DirectCast(cell.Row.ListObject, UltraDataRow).Tag
            End If

            If (cell.Value IsNot Nothing AndAlso cell.Value.ToString = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS) Then
                cell.Style = ColumnStyle.Button
            End If
        Next
    End Sub

    Private Sub ugHarness_InitializeRow(sender As Object, e As InitializeRowEventArgs) Handles ugHarness.InitializeRow
        If Not e.ReInitialize Then
            InitializeHarnessRowCore(e.Row)
        End If
    End Sub

    Private Sub ugHarness_MouseDown(sender As Object, e As MouseEventArgs) Handles ugHarness.MouseDown
        _pressedMouseButton = e.Button
    End Sub

End Class