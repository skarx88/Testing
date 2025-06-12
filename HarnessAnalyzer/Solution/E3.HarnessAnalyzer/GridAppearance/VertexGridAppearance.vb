Imports Zuken.E3.Lib.Schema.Kbl.Properties

<KblObjectType(KblObjectType.Node)>
Public Class VertexGridAppearance
    Inherits GridAppearance

    Public Sub New()
        MyBase.New(KblObjectType.Node)
    End Sub

    Public Overrides Sub CreateDefaultTable()
        _gridTable = New GridTable(KblObjectType.Node, KblObjectType.Node.ToLocalizedPluralString)
        With _gridTable.GridColumns
            .AddNew(Me, ObjectPropertyNameStrings.Id, 0, True, True, VertexPropertyName.Id.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.AliasId, 1, True, True, VertexPropertyName.Alias_id.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, "X", 2, True, True, VertexPropertyName.Cartesian_pointX.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, "Y", 3, True, True, VertexPropertyName.Cartesian_pointY.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, "Z", 4, True, True, VertexPropertyName.Cartesian_pointZ.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.ReferencedComponents, 5, True, True, VertexPropertyName.Referenced_components.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.ReferencedCavities, 6, True, True, VertexPropertyName.Referenced_cavities.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.ProcessingInformation, 7, True, True, VertexPropertyName.Processing_information.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.FoldingDirection, 8, True, True, VertexPropertyName.Folding_direction.ToString)
        End With

        _gridTable.GridSubTable = New GridTable(KblObjectType.Protection_on_vertex, KblObjectType.Node.ToLocalizedPluralString + " " + KblObjectType.Wire_protection_occurrence.ToLocalizedPluralString)
        With _gridTable.GridSubTable.GridColumns
            .AddNew(Me, ObjectPropertyNameStrings.Id, 0, True, True, WireProtectionPropertyName.Id.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.AliasId, 1, True, True, WireProtectionPropertyName.Alias_id.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.Description, 2, True, True, WireProtectionPropertyName.Description.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.ProtectionLength, 3, True, True, WireProtectionPropertyName.Protection_length.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.InstallationInformation, 4, True, True, WireProtectionPropertyName.Installation_Information.ToString)

            .AddNew(Me, ObjectPropertyNameStrings.PartNumber, 5, True, True, PartPropertyName.Part_number.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.CompanyName, 6, True, True, PartPropertyName.Company_name.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.PartAliasId, 7, True, True, PartPropertyName.Part_alias_ids.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.Version, 8, True, True, PartPropertyName.Version.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.Abbreviation, 9, True, True, PartPropertyName.Abbreviation.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.PartDescription, 10, True, True, PartPropertyName.Part_description.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.PredecessorPartNumber, 11, True, True, PartPropertyName.Predecessor_part_number.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.DegreeOfMaturity, 12, True, True, PartPropertyName.Degree_of_maturity.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.CopyrightNote, 13, True, True, PartPropertyName.Copyright_note.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.MassInformation, 14, True, True, PartPropertyName.Mass_information.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.ExternalReferences, 15, True, True, PartPropertyName.External_references.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.Change, 16, True, True, PartPropertyName.Change.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.MaterialInformation, 17, True, True, PartPropertyName.Material_information.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.ProcessingInformation, 18, True, True, PartPropertyName.Part_processing_information.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.ProtectionType, 19, True, True, WireProtectionPropertyName.Protection_type.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.TypeDependentParameter, 20, True, True, WireProtectionPropertyName.Type_dependent_parameter.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.WindingType, 21, True, True, WireProtectionPropertyName.Winding_type.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.WindingFirmness, 22, True, True, WireProtectionPropertyName.Winding_firmness.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.PartNumberType, 23, True, True, PartPropertyName.Part_number_type.ToString)
        End With
    End Sub

End Class
