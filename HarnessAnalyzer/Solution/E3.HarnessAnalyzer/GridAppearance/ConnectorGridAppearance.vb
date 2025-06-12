Imports Windows.Win32.System
Imports Zuken.E3.Lib.Schema.Kbl.Properties

<KblObjectType(KblObjectType.Connector_occurrence)>
Public Class ConnectorGridAppearance
    Inherits GridAppearance

    Public Sub New()
        MyBase.New(KblObjectType.Connector_occurrence)
    End Sub

    Public Overrides Sub CreateDefaultTable()
        _gridTable = New GridTable(KblObjectType.Connector_occurrence, KblObjectType.Connector_occurrence.ToLocalizedPluralString)
        With _gridTable.GridColumns
            .AddNew(Me, ObjectPropertyNameStrings.Id, 0, True, True, ConnectorPropertyName.Id, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.PartNumber, 1, True, True, PartPropertyName.Part_number)
            .AddNew(Me, ObjectPropertyNameStrings.PartDescription, 2, True, True, PartPropertyName.Part_description, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.Description, 3, True, True, ConnectorPropertyName.Description, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.Abbreviation, 4, True, True, PartPropertyName.Abbreviation, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.CavityCount, 5, True, True, "Cavity count")
            .AddNew(Me, ObjectPropertyNameStrings.HousingColor, 6, True, True, ConnectorPropertyName.Housing_colour, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.HousingType, 7, True, True, ConnectorPropertyName.Housing_type, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.MassInformation, 8, True, True, PartPropertyName.Mass_information)
            .AddNew(Me, ObjectPropertyNameStrings.InstallationInformation, 9, True, True, ConnectorPropertyName.Installation_Information)

            .AddNew(Me, ObjectPropertyNameStrings.AliasId, 10, True, True, ConnectorPropertyName.Alias_id, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.Usage, 11, True, True, ConnectorPropertyName.Usage)
            .AddNew(Me, ObjectPropertyNameStrings.CompanyName, 12, True, True, PartPropertyName.Company_name)
            .AddNew(Me, ObjectPropertyNameStrings.PartAliasId, 13, True, True, PartPropertyName.Part_alias_ids, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.Version, 14, True, True, PartPropertyName.Version)
            .AddNew(Me, ObjectPropertyNameStrings.PredecessorPartNumber, 15, True, True, PartPropertyName.Predecessor_part_number)
            .AddNew(Me, ObjectPropertyNameStrings.DegreeOfMaturity, 16, True, True, PartPropertyName.Degree_of_maturity)
            .AddNew(Me, ObjectPropertyNameStrings.CopyrightNote, 17, True, True, PartPropertyName.Copyright_note)
            .AddNew(Me, ObjectPropertyNameStrings.ExternalReferences, 18, True, True, PartPropertyName.External_references)
            .AddNew(Me, ObjectPropertyNameStrings.Change, 19, True, True, PartPropertyName.Change)
            .AddNew(Me, ObjectPropertyNameStrings.MaterialInformation, 20, True, True, PartPropertyName.Material_information)
            .AddNew(Me, ObjectPropertyNameStrings.ProcessingInformation, 21, True, True, PartPropertyName.Part_processing_information)
            .AddNew(Me, ObjectPropertyNameStrings.HousingCode, 22, True, True, ConnectorPropertyName.Housing_code, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.ReferenceElement, 23, True, True, ConnectorPropertyName.Reference_element)
            .AddNew(Me, ObjectPropertyNameStrings.PartNumberType, 24, True, True, PartPropertyName.Part_number_type)
            .AddNew(Me, ObjectPropertyNameStrings.LocalizedPartDescriptions, 25, True, True, PartPropertyName.Part_Localized_description, True)
            .AddNew(Me, ObjectPropertyNameStrings.LocalizedOccDescriptions, 26, True, True, ConnectorPropertyName.Localized_description, True)
        End With

        _gridTable.GridSubTable = New GridTable(KblObjectType.Cavity_occurrence, KblObjectType.Cavity_occurrence.ToLocalizedPluralString)
        With _gridTable.GridSubTable.GridColumns
            .AddNew(Me, ObjectPropertyNameStrings.CavityNumber, 0, True, True, ConnectorPropertyName.Cavity_number, True)

            .AddNew(Me, ObjectPropertyNameStrings.WireCoreNumber, 1, True, True, HarnessAnalyzer.[Shared].CORE_WIRE_NUMBER_KEY, False, True, True, Nothing)
            .AddNew(Me, ObjectPropertyNameStrings.WireType, 2, True, True, WirePropertyName.Wire_type, False, True, True, Nothing)
            .AddNew(Me, ObjectPropertyNameStrings.CrossSectionArea, 3, True, True, WirePropertyName.Cross_section_area, False, True, True, Nothing)
            .AddNew(Me, ObjectPropertyNameStrings.Color, 4, True, True, WirePropertyName.Core_Colour, False, True, True, Nothing)

            .AddNew(Me, ObjectPropertyNameStrings.Description, 5, True, True, ConnectorPropertyName.Description)
            .AddNew(Me, ObjectPropertyNameStrings.TerminalPartNumber, 6, True, True, ConnectorPropertyName.Terminal_part_number, True)
            .AddNew(Me, ObjectPropertyNameStrings.TerminalInformation, 7, True, True, ConnectorPropertyName.Terminal_part_information)
            .AddNew(Me, ObjectPropertyNameStrings.SealPartNumber, 8, True, True, ConnectorPropertyName.Seal_part_number, True)
            .AddNew(Me, ObjectPropertyNameStrings.SealInformation, 9, True, True, ConnectorPropertyName.Seal_part_information)
            .AddNew(Me, ObjectPropertyNameStrings.PlugPartNumber, 10, True, True, ConnectorPropertyName.Plug_part_number, True)
            .AddNew(Me, ObjectPropertyNameStrings.PlugInformation, 11, True, True, ConnectorPropertyName.Plug_part_information)
            .AddNew(Me, ObjectPropertyNameStrings.Plating, 12, True, True, ConnectorPropertyName.Plating, True)
        End With
    End Sub

End Class
