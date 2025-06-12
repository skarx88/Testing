Imports Zuken.E3.Lib.Schema.Kbl.Properties

<KblObjectType(KblObjectType.Co_pack_occurrence)>
Public Class CoPackGridAppearance
    Inherits GridAppearance

    Public Sub New()
        MyBase.New(KblObjectType.Co_pack_occurrence)
    End Sub

    Public Overrides Sub CreateDefaultTable()
        _gridTable = New GridTable(KblObjectType.Co_pack_occurrence, KblObjectType.Co_pack_occurrence.ToLocalizedPluralString)
        With _gridTable.GridColumns
            .AddNew(Me, ObjectPropertyNameStrings.Id, 0, True, True, CoPackPropertyName.Id.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.PartNumber, 1, True, True, PartPropertyName.Part_number.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.PartDescription, 2, True, True, PartPropertyName.Part_description.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.Description, 3, True, True, CoPackPropertyName.Description.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.Abbreviation, 4, True, True, PartPropertyName.Abbreviation.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.MassInformation, 5, True, True, PartPropertyName.Mass_information.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.InstallationInformation, 6, True, True, CoPackPropertyName.Installation_Information.ToString)

            .AddNew(Me, ObjectPropertyNameStrings.AliasId, 7, True, True, AssemblyPartPropertyName.Alias_id.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.CompanyName, 8, True, True, PartPropertyName.Company_name.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.PartAliasId, 9, True, True, PartPropertyName.Part_alias_ids.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.Version, 10, True, True, PartPropertyName.Version.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.PredecessorPartNumber, 11, True, True, PartPropertyName.Predecessor_part_number.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.DegreeOfMaturity, 12, True, True, PartPropertyName.Degree_of_maturity.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.CopyrightNote, 13, True, True, PartPropertyName.Copyright_note.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.ExternalReferences, 14, True, True, PartPropertyName.External_references.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.Change, 15, True, True, PartPropertyName.Change.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.MaterialInformation, 16, True, True, PartPropertyName.Material_information.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.ProcessingInformation, 17, True, True, PartPropertyName.Part_processing_information.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.PartType, 18, True, True, AssemblyPartPropertyName.Part_type.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.PartNumberType, 19, True, True, PartPropertyName.Part_number_type.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.LocalizedPartDescriptions, 20, True, True, PartPropertyName.Part_Localized_description.ToString, True)
            .AddNew(Me, ObjectPropertyNameStrings.LocalizedOccDescriptions, 21, True, True, ConnectorPropertyName.Localized_description.ToString, True)
        End With
    End Sub

End Class