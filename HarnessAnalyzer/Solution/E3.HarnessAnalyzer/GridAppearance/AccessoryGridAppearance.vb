Imports Zuken.E3.Lib.Schema.Kbl.Properties

<KblObjectType(KblObjectType.Accessory_occurrence)>
Public Class AccessoryGridAppearance
    Inherits GridAppearance

    Public Sub New()
        MyBase.New(KblObjectType.Accessory_occurrence)
    End Sub

    Public Overrides Sub CreateDefaultTable()
        _gridTable = New GridTable(KblObjectType.Accessory_occurrence, KblObjectType.Accessory_occurrence.ToLocalizedPluralString)
        With _gridTable.GridColumns
            .AddNew(Me, ObjectPropertyNameStrings.Id, 0, True, True, AccessoryPropertyName.Id, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.PartNumber, 1, True, True, PartPropertyName.Part_number)
            .AddNew(Me, ObjectPropertyNameStrings.PartDescription, 2, True, True, PartPropertyName.Part_description, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.Description, 3, True, True, AccessoryPropertyName.Description, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.Abbreviation, 4, True, True, PartPropertyName.Abbreviation, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.MassInformation, 5, True, True, PartPropertyName.Mass_information, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.ReferenceElement, 6, True, True, AccessoryPropertyName.Reference_element)
            .AddNew(Me, ObjectPropertyNameStrings.InstallationInformation, 7, True, True, AccessoryPropertyName.Installation_Information)

            .AddNew(Me, ObjectPropertyNameStrings.AliasId, 8, True, True, AccessoryPropertyName.Alias_id, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.CompanyName, 9, True, True, PartPropertyName.Company_name)
            .AddNew(Me, ObjectPropertyNameStrings.PartAliasId, 10, True, True, PartPropertyName.Part_alias_ids, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.Version, 11, True, True, PartPropertyName.Version)
            .AddNew(Me, ObjectPropertyNameStrings.PredecessorPartNumber, 12, True, True, PartPropertyName.Predecessor_part_number)
            .AddNew(Me, ObjectPropertyNameStrings.DegreeOfMaturity, 13, True, True, PartPropertyName.Degree_of_maturity)
            .AddNew(Me, ObjectPropertyNameStrings.CopyrightNote, 14, True, True, PartPropertyName.Copyright_note)
            .AddNew(Me, ObjectPropertyNameStrings.ExternalReferences, 15, True, True, PartPropertyName.External_references)
            .AddNew(Me, ObjectPropertyNameStrings.Change, 16, True, True, PartPropertyName.Change)
            .AddNew(Me, ObjectPropertyNameStrings.MaterialInformation, 17, True, True, PartPropertyName.Material_information)
            .AddNew(Me, ObjectPropertyNameStrings.ProcessingInformation, 18, True, True, PartPropertyName.Part_processing_information)

            'HINT The compare settings is blocked out as it is on the segments fixing assignment
            .AddNew(Me, ObjectPropertyNameStrings.StartVertex, 19, True, True, SegmentPropertyName.Start_node, False, True, False, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.SegmentLocation, 20, True, True, AccessoryPropertyName.SegmentLocation, False, True, False, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.LocationAbsolute, 21, True, True, AccessoryPropertyName.SegmentAbsolute_location, False, True, False, New D3DGridColumnSetting(True))

            .AddNew(Me, ObjectPropertyNameStrings.AccessoryType, 22, True, True, AccessoryPropertyName.Accessory_type, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.PartNumberType, 23, True, True, PartPropertyName.Part_number_type)
            .AddNew(Me, ObjectPropertyNameStrings.LocalizedPartDescriptions, 24, True, True, PartPropertyName.Part_Localized_description, True)
            .AddNew(Me, ObjectPropertyNameStrings.LocalizedOccDescriptions, 25, True, True, ConnectorPropertyName.Localized_description, True)
        End With
    End Sub

End Class
