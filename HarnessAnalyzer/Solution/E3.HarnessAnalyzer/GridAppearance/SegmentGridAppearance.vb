Imports Zuken.E3.Lib.Schema.Kbl.Properties

<KblObjectType(KblObjectType.Segment)>
Public Class SegmentGridAppearance
    Inherits GridAppearance

    Public Sub New()
        MyBase.New(KblObjectType.Segment)
    End Sub

    Public Overrides Sub CreateDefaultTable()
        _gridTable = New GridTable(KblObjectType.Segment, KblObjectType.Segment.ToLocalizedPluralString)
        With _gridTable.GridColumns
            .AddNew(Me, ObjectPropertyNameStrings.Id, 0, True, True, SegmentPropertyName.Id.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.VirtualLength, 1, True, True, SegmentPropertyName.Virtual_length.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.PhysicalLength, 2, True, True, SegmentPropertyName.Physical_length.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.CrossSectionArea, 3, True, True, SegmentPropertyName.Cross_Section_Area_information.ToString, True, New D3DGridColumnSetting(True))

            .AddNew(Me, ObjectPropertyNameStrings.AliasId, 4, True, True, SegmentPropertyName.Alias_id.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.Form, 5, True, True, SegmentPropertyName.Form.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.StartVertex, 6, True, True, SegmentPropertyName.Start_node.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.EndVertex, 7, True, True, SegmentPropertyName.End_node.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.FixingAssignment, 8, True, True, SegmentPropertyName.Fixing_Assignment.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.ProcessingInformation, 9, True, True, SegmentPropertyName.Processing_information.ToString)
        End With

        _gridTable.GridSubTable = New GridTable(KblObjectType.Protection_on_segment, KblObjectType.Segment.ToLocalizedString + " " + KblObjectType.Wire_protection.ToLocalizedPluralString)
        With _gridTable.GridSubTable.GridColumns
            .AddNew(Me, ObjectPropertyNameStrings.Id, 0, True, True, WireProtectionPropertyName.Id.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.PartNumber, 1, True, True, PartPropertyName.Part_number.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.ProtectionLength, 2, True, True, WireProtectionPropertyName.Protection_length.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.StartVertex, 3, True, True, SegmentPropertyName.Start_node.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.StartLocation, 4, True, True, ProtectionAreaPropertyName.Start_location.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.StartLocationAbsolute, 5, True, True, ProtectionAreaPropertyName.Absolute_start_location.ToString, True, New D3DGridColumnSetting(True))

            'HINT this end node should only be accessible in 3D (spanning protection gimmick) but is system internal and not shown in the settings dialog(ordinal = -1)
            .AddNew(Me, ObjectPropertyNameStrings.EndVertex, -1, False, False, SegmentPropertyName.End_node.ToString, False, New D3DGridColumnSetting(True))


            .AddNew(Me, ObjectPropertyNameStrings.EndLocation, 6, True, True, ProtectionAreaPropertyName.End_location.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.EndLocationAbsolute, 7, True, True, ProtectionAreaPropertyName.Absolute_end_location.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.AliasId, 8, True, True, WireProtectionPropertyName.Alias_id.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.Description, 9, True, True, WireProtectionPropertyName.Description.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.TapingDirection, 10, True, True, ProtectionAreaPropertyName.Taping_direction.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.ProcessingInformation, 11, True, True, ProtectionAreaPropertyName.Processing_information.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.CompanyName, 12, True, True, PartPropertyName.Company_name.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.PartAliasId, 13, True, True, PartPropertyName.Part_alias_ids.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.Version, 14, True, True, PartPropertyName.Version.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.Abbreviation, 15, True, True, PartPropertyName.Abbreviation.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.PartDescription, 16, True, True, PartPropertyName.Part_description.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.PredecessorPartNumber, 17, True, True, PartPropertyName.Predecessor_part_number.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.DegreeOfMaturity, 18, True, True, PartPropertyName.Degree_of_maturity.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.CopyrightNote, 19, True, True, PartPropertyName.Copyright_note.ToString)
            .AddNew(Me, String.Format("{0} [{1}]", ObjectPropertyNameStrings.MassInformation, ObjectPropertyNameStrings.PerMeter), 20, True, True, PartPropertyName.Mass_information.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.ExternalReferences, 21, True, True, PartPropertyName.External_references.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.Change, 22, True, True, PartPropertyName.Change.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.MaterialInformation, 23, True, True, PartPropertyName.Material_information.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.PartProcessingInformation, 24, True, True, PartPropertyName.Part_processing_information.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.ProtectionType, 25, True, True, WireProtectionPropertyName.Protection_type.ToString, True, New D3DGridColumnSetting(True))
            .AddNew(Me, ObjectPropertyNameStrings.TypeDependentParameter, 26, True, True, WireProtectionPropertyName.Type_dependent_parameter.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.InstallationInformation, 27, True, True, WireProtectionPropertyName.Installation_Information.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.IsOnTopOf, 28, True, True, ProtectionAreaPropertyName.Is_on_top_of.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.WindingType, 29, True, True, WireProtectionPropertyName.Winding_type.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.WindingFirmness, 30, True, True, WireProtectionPropertyName.Winding_firmness.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.PartNumberType, 31, True, True, PartPropertyName.Part_number_type.ToString)
        End With
    End Sub

End Class
