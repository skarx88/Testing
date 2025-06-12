Imports Zuken.E3.Lib.Schema.Kbl.Properties

<KblObjectType(KblObjectType.Component_occurrence)>
Public Class ComponentGridAppearance
    Inherits GridAppearance

    Public Sub New()
        MyBase.New(KblObjectType.Component_occurrence)
    End Sub

    Public Overrides Sub CreateDefaultTable()
        _gridTable = New GridTable(KblObjectType.Component_occurrence, KblObjectType.Component_occurrence.ToLocalizedPluralString)
        With _gridTable.GridColumns
            .AddNew(Me, ObjectPropertyNameStrings.Id, 0, True, True, ComponentPropertyName.Id.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.PartNumber, 1, True, True, PartPropertyName.Part_number.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.PartDescription, 2, True, True, PartPropertyName.Part_description.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.Description, 3, True, True, ComponentPropertyName.Description.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.Abbreviation, 4, True, True, PartPropertyName.Abbreviation.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.MassInformation, 5, True, True, PartPropertyName.Mass_information.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.ExternalReferences, 6, True, True, PartPropertyName.External_references.ToString)

            .AddNew(Me, ObjectPropertyNameStrings.AliasId, 7, True, True, ComponentPropertyName.Alias_id.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.MountingObjects, 8, True, True, ComponentPropertyName.Mounting.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.CompanyName, 9, True, True, PartPropertyName.Company_name.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.PartAliasId, 10, True, True, PartPropertyName.Part_alias_ids.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.Version, 11, True, True, PartPropertyName.Version.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.PredecessorPartNumber, 12, True, True, PartPropertyName.Predecessor_part_number.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.DegreeOfMaturity, 13, True, True, PartPropertyName.Degree_of_maturity.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.CopyrightNote, 14, True, True, PartPropertyName.Copyright_note.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.Change, 15, True, True, PartPropertyName.Change.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.MaterialInformation, 16, True, True, PartPropertyName.Material_information.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.ProcessingInformation, 17, True, True, PartPropertyName.Part_processing_information.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.ComponentPinMaps, 18, True, True, ComponentPropertyName.ComponentPinMaps.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.InstallationInformation, 19, True, True, ComponentPropertyName.Installation_Information.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.PartNumberType, 20, True, True, PartPropertyName.Part_number_type.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.LocalizedPartDescriptions, 21, True, True, PartPropertyName.Part_Localized_description.ToString, True)
            .AddNew(Me, ObjectPropertyNameStrings.LocalizedOccDescriptions, 22, True, True, ConnectorPropertyName.Localized_description.ToString, True)
        End With
    End Sub

End Class
