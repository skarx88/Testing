Imports Zuken.E3.Lib.Schema.Kbl.Properties

'<KblObjectType(KblObjectType.Connector_occurrence)>
'HINT There must not be a type descriptor as this special appearance is not the source of information to the compare
Public Class D3DConnectorTableAppearance
    Inherits GridAppearance

    Public Sub New()
        MyBase.New(KblObjectType.Connector_occurrence_D3D)
    End Sub

    Public Overrides Sub CreateDefaultTable()
        _gridTable = New GridTable(KblObjectType.Connector_occurrence_D3D, KblObjectType.Connector_occurrence_D3D.ToLocalizedPluralString)
        With _gridTable.GridColumns
            .AddNew(Me, ObjectPropertyNameStrings.CavityNumber, 0, True, True, ConnectorPropertyName.Cavity_number)
            .AddNew(Me, ObjectPropertyNameStrings.WireCoreNumber, 1, True, True, HarnessAnalyzer.[Shared].CORE_WIRE_NUMBER_KEY)
            .AddNew(Me, ObjectPropertyNameStrings.WireType, 2, True, True, WirePropertyName.Wire_type)
            .AddNew(Me, ObjectPropertyNameStrings.CrossSectionArea, 3, True, True, WirePropertyName.Cross_section_area)
            .AddNew(Me, ObjectPropertyNameStrings.Color, 4, True, True, WirePropertyName.Core_Colour)
            .AddNew(Me, ObjectPropertyNameStrings.HarnessModule, 5, True, True, HarnessAnalyzer.Shared.MODULE_KEY)
            .AddNew(Me, ObjectPropertyNameStrings.TerminalPartNumber, 6, True, True, ConnectorPropertyName.Terminal_part_number)
            .AddNew(Me, ObjectPropertyNameStrings.SealPartNumber, 7, True, True, ConnectorPropertyName.Seal_part_number)
            .AddNew(Me, ObjectPropertyNameStrings.PlugPartNumber, 8, True, True, ConnectorPropertyName.Plug_part_number)
            .AddNew(Me, ObjectPropertyNameStrings.Plating, 9, True, True, ConnectorPropertyName.Plating)
        End With
    End Sub

End Class
