Imports Zuken.E3.Lib.Schema.Kbl.Properties

<KblObjectType(KblObjectType.Fixing_occurrence)>
Public Class FixingGridAppearance
    Inherits GridAppearance

    Public Sub New()
        MyBase.New(KblObjectType.Fixing_occurrence)
    End Sub

    Public Overrides Sub CreateDefaultTable()
        _gridTable = New GridTable(KblObjectType.Fixing_occurrence, KblObjectType.Fixing_occurrence.ToLocalizedPluralString)
        With _gridTable.GridColumns
            .AddNew(Me, ObjectPropertyNameStrings.Id, 0, True, True, FixingPropertyName.Id.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.AliasId, 1, True, True, FixingPropertyName.Alias_Id.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.PartNumber, 2, True, True, PartPropertyName.Part_number.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.PartDescription, 3, True, True, PartPropertyName.Part_description.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.Description, 4, True, True, FixingPropertyName.Description.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.Abbreviation, 5, True, True, PartPropertyName.Abbreviation.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.MassInformation, 6, True, True, PartPropertyName.Mass_information.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.FixingType, 7, True, True, FixingPropertyName.Fixing_type.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.ExternalReferences, 8, True, True, PartPropertyName.External_references.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.InstallationInformation, 9, True, True, FixingPropertyName.Installation_Information.ToString)

            .AddNew(Me, ObjectPropertyNameStrings.CompanyName, 10, True, True, PartPropertyName.Company_name.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.PartAliasId, 11, True, True, PartPropertyName.Part_alias_ids.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.Version, 12, True, True, PartPropertyName.Version.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.PredecessorPartNumber, 13, True, True, PartPropertyName.Predecessor_part_number.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.DegreeOfMaturity, 14, True, True, PartPropertyName.Degree_of_maturity.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.CopyrightNote, 15, True, True, PartPropertyName.Copyright_note.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.Change, 16, True, True, PartPropertyName.Change.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.MaterialInformation, 17, True, True, PartPropertyName.Material_information.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.ProcessingInformation, 18, True, True, PartPropertyName.Part_processing_information.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.ProcessingInformationAssignment, 19, True, True, FixingPropertyName.AssignmentProcessingInfos.ToString) 'HINT : these processing informations here on the assignment are not searchable, but there is no restriction/visualisation right now...

            'HINT The compare settings is blocked out as it is on the segments fixing assignment
            .AddNew(Me, ObjectPropertyNameStrings.StartVertex, 20, True, True, SegmentPropertyName.Start_node.ToString, False, True, False, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.SegmentLocation, 21, True, True, FixingPropertyName.SegmentLocation.ToString, False, True, False, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.LocationAbsolute, 22, True, True, FixingPropertyName.SegmentAbsolute_location.ToString, False, True, False, New D3DGridColumnSetting(True))

            .AddNew(Me, ObjectPropertyNameStrings.PartNumberType, 23, True, True, PartPropertyName.Part_number_type.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.LocalizedPartDescriptions, 24, True, True, PartPropertyName.Part_Localized_description.ToString, True)
            .AddNew(Me, ObjectPropertyNameStrings.LocalizedOccDescriptions, 25, True, True, ConnectorPropertyName.Localized_description.ToString, True)
        End With
    End Sub

End Class
