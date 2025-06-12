Imports Zuken.E3.Lib.Schema.Kbl.Properties

<KblObjectType(KblObjectType.Wire_occurrence)>
Public Class WireGridAppearance
    Inherits GridAppearance

    Public Sub New()
        MyBase.New(KblObjectType.wire_occurrence)

    End Sub

    Public Overrides Sub CreateDefaultTable()
        _gridTable = New GridTable(KblObjectType.Wire_occurrence, KblObjectType.Wire_occurrence.ToLocalizedPluralString + "/" + KblObjectType.Core_occurrence.ToLocalizedPluralString)
        With _gridTable.GridColumns
            .AddNew(Me, ObjectPropertyNameStrings.WireNumber, 0, True, True, WirePropertyName.Wire_number.ToString)
            .AddNew(Me, KblObjectType.Wiring_group.ToLocalizedPluralString, 1, True, True, WirePropertyName.Wiring_group.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.CrossSectionArea, 2, True, True, WirePropertyName.Cross_section_area.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.Color, 3, True, True, WirePropertyName.Core_Colour.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.LengthInformation, 4, True, True, WirePropertyName.Length_information.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.StartConnector, 5, True, True, ConnectionPropertyName.Connector_A.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.StartCavity, 6, True, True, ConnectionPropertyName.Cavity_A.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.EndConnector, 7, True, True, ConnectionPropertyName.Connector_B.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.EndCavity, 8, True, True, ConnectionPropertyName.Cavity_B.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.NetName, 9, True, True, ConnectionPropertyName.Signal_name.ToString)

            .AddNew(Me, ObjectPropertyNameStrings.PartNumber, 10, True, True, PartPropertyName.Part_number.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.Description, 11, True, True, PartPropertyName.Part_description.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.Abbreviation, 12, True, True, PartPropertyName.Abbreviation.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.CompanyName, 13, True, True, PartPropertyName.Company_name.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.AliasId, 14, True, True, PartPropertyName.Part_alias_ids.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.Version, 15, True, True, PartPropertyName.Version.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.PredecessorPartNumber, 16, True, True, PartPropertyName.Predecessor_part_number.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.DegreeOfMaturity, 17, True, True, PartPropertyName.Degree_of_maturity.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.CopyrightNote, 18, True, True, PartPropertyName.Copyright_note.ToString)
            .AddNew(Me, String.Format("{0} [{1}]", ObjectPropertyNameStrings.MassInformation, ObjectPropertyNameStrings.PerMeter), 19, True, True, PartPropertyName.Mass_information.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.ExternalReferences, 20, True, True, PartPropertyName.External_references.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.Change, 21, True, True, PartPropertyName.Change.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.MaterialInformation, 22, True, True, PartPropertyName.Material_information.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.ProcessingInformation, 23, True, True, PartPropertyName.Part_processing_information.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.InstallationInformation, 24, True, True, WirePropertyName.Installation_Information.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.CableDesignator, 25, True, True, WirePropertyName.Cable_designator.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.WireType, 26, True, True, WirePropertyName.Wire_type.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.BendRadius, 27, True, True, WirePropertyName.Bend_radius.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.OutsideDiameter, 28, True, True, WirePropertyName.Outside_diameter.ToString)

            .AddNew(Me, ObjectPropertyNameStrings.Routing, 29, True, True, WirePropertyName.Routing.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.AddExtremities, 30, True, True, WirePropertyName.AdditionalExtremities.ToString)

            .AddNew(Me, ObjectPropertyNameStrings.PartNumberType, 31, True, True, PartPropertyName.Part_number_type.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.LocalizedPartDescriptions, 32, True, True, PartPropertyName.Part_Localized_description.ToString, True)

        End With
    End Sub

End Class
