Imports Zuken.E3.Lib.Schema.Kbl.Properties

<KblObjectType(KblObjectType.Special_wire_occurrence)>
Public Class CableGridAppearance
    Inherits GridAppearance

    Public Sub New()
        MyBase.New(KblObjectType.Special_wire_occurrence)
    End Sub

    Public Overrides Sub CreateDefaultTable()
        _gridTable = New GridTable(KblObjectType.Special_wire_occurrence, KblObjectType.Special_wire_occurrence.ToLocalizedPluralString)
        With _gridTable.GridColumns
            .AddNew(Me, ObjectPropertyNameStrings.CableNumber, 0, True, True, CablePropertyName.Special_wire_id)
            .AddNew(Me, ObjectPropertyNameStrings.CrossSectionArea, 1, True, True, CablePropertyName.Cross_section_area)
            .AddNew(Me, ObjectPropertyNameStrings.CoverColor, 2, True, True, CablePropertyName.CoverColours)
            .AddNew(Me, ObjectPropertyNameStrings.LengthInformation, 3, True, True, CablePropertyName.Length_Information)

            .AddNew(Me, ObjectPropertyNameStrings.PartNumber, 4, True, True, PartPropertyName.Part_number)
            .AddNew(Me, ObjectPropertyNameStrings.Description, 5, True, True, PartPropertyName.Part_description)
            .AddNew(Me, ObjectPropertyNameStrings.Abbreviation, 6, True, True, PartPropertyName.Abbreviation)
            .AddNew(Me, ObjectPropertyNameStrings.CompanyName, 7, True, True, PartPropertyName.Company_name)
            .AddNew(Me, ObjectPropertyNameStrings.AliasId, 8, True, True, PartPropertyName.Part_alias_ids)
            .AddNew(Me, ObjectPropertyNameStrings.Version, 9, True, True, PartPropertyName.Version)
            .AddNew(Me, ObjectPropertyNameStrings.PredecessorPartNumber, 10, True, True, PartPropertyName.Predecessor_part_number)
            .AddNew(Me, ObjectPropertyNameStrings.DegreeOfMaturity, 11, True, True, PartPropertyName.Degree_of_maturity)
            .AddNew(Me, ObjectPropertyNameStrings.CopyrightNote, 12, True, True, PartPropertyName.Copyright_note)
            .AddNew(Me, String.Format("{0} [{1}]", ObjectPropertyNameStrings.MassInformation, ObjectPropertyNameStrings.PerMeter), 13, True, True, PartPropertyName.Mass_information)
            .AddNew(Me, ObjectPropertyNameStrings.ExternalReferences, 14, True, True, PartPropertyName.External_references)
            .AddNew(Me, ObjectPropertyNameStrings.Change, 15, True, True, PartPropertyName.Change)
            .AddNew(Me, ObjectPropertyNameStrings.MaterialInformation, 16, True, True, PartPropertyName.Material_information)
            .AddNew(Me, ObjectPropertyNameStrings.ProcessingInformation, 17, True, True, PartPropertyName.Part_processing_information)
            .AddNew(Me, ObjectPropertyNameStrings.InstallationInformation, 18, True, True, CablePropertyName.Installation_Information)
            .AddNew(Me, ObjectPropertyNameStrings.CableDesignator, 19, True, True, CablePropertyName.Cable_designator)
            .AddNew(Me, ObjectPropertyNameStrings.WireType, 20, True, True, CablePropertyName.Wire_type)
            .AddNew(Me, ObjectPropertyNameStrings.BendRadius, 21, True, True, CablePropertyName.Bend_radius)
            .AddNew(Me, ObjectPropertyNameStrings.OutsideDiameter, 22, True, True, CablePropertyName.Outside_diameter)
            .AddNew(Me, ObjectPropertyNameStrings.Routing, 23, True, True, CablePropertyName.Routing)
            .AddNew(Me, ObjectPropertyNameStrings.PartNumberType, 24, True, True, PartPropertyName.Part_number_type)
            .AddNew(Me, ObjectPropertyNameStrings.LocalizedPartDescriptions, 25, True, True, PartPropertyName.Part_Localized_description, True)
            '.AddGridColumn(New GridColumn(ObjectPropertyNameStrings.LocalizedOccDescriptions, 26, True, True, ConnectorPropertyName.Localized_Description, True))
        End With

        _gridTable.GridSubTable = New GridTable(KblObjectType.Core_occurrence, KblObjectType.Core_occurrence.ToLocalizedPluralString)
        With _gridTable.GridSubTable.GridColumns
            .AddNew(Me, ObjectPropertyNameStrings.CoreNumber, 0, True, True, WirePropertyName.Wire_number)
            .AddNew(Me, ObjectPropertyNameStrings.CrossSectionArea, 1, True, True, WirePropertyName.Cross_section_area)
            .AddNew(Me, ObjectPropertyNameStrings.CoreColor, 2, True, True, WirePropertyName.Core_Colour)
            .AddNew(Me, ObjectPropertyNameStrings.LengthInformation, 3, True, True, WirePropertyName.Length_information)
            .AddNew(Me, ObjectPropertyNameStrings.StartConnector, 4, True, True, ConnectionPropertyName.Connector_A)
            .AddNew(Me, ObjectPropertyNameStrings.StartCavity, 5, True, True, ConnectionPropertyName.Cavity_A)
            .AddNew(Me, ObjectPropertyNameStrings.EndConnector, 6, True, True, ConnectionPropertyName.Connector_B)
            .AddNew(Me, ObjectPropertyNameStrings.EndCavity, 7, True, True, ConnectionPropertyName.Cavity_B)
            .AddNew(Me, ObjectPropertyNameStrings.NetName, 8, True, True, ConnectionPropertyName.Signal_name)
            .AddNew(Me, ObjectPropertyNameStrings.CoreId, 9, True, True, WirePropertyName.Id)
            .AddNew(Me, ObjectPropertyNameStrings.CableDesignator, 10, True, True, WirePropertyName.Cable_designator)
            .AddNew(Me, ObjectPropertyNameStrings.CoreType, 11, True, True, WirePropertyName.Wire_type)
            .AddNew(Me, ObjectPropertyNameStrings.OutsideDiameter, 12, True, True, WirePropertyName.Outside_diameter)
            .AddNew(Me, ObjectPropertyNameStrings.BendRadius, 13, True, True, WirePropertyName.Bend_radius)
        End With
    End Sub

End Class
