Imports Zuken.E3.Lib.Schema.Kbl.Properties

<KblObjectType(KblObjectType.Dimension_specification)>
Public Class DimSpecGridAppearance
    Inherits GridAppearance

    Public Sub New()
        MyBase.New(KblObjectType.dimension_specification)
    End Sub

    Public Overrides Sub CreateDefaultTable()
        _gridTable = New GridTable(KblObjectType.Dimension_specification, KblObjectType.Dimension_specification.ToLocalizedPluralString)
        With _gridTable.GridColumns
            .AddNew(Me, ObjectPropertyNameStrings.Id, 0, True, True, DimensionSpecificationPropertyName.Id.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.DimValue, 1, True, True, DimensionSpecificationPropertyName.Dimension_value.ToString)
            .AddNew(Me, KblObjectType.Segment.ToLocalizedPluralString, 2, True, True, DimensionSpecificationPropertyName.Segments.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.Origin, 3, True, True, DimensionSpecificationPropertyName.Origin.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.Target, 4, True, True, DimensionSpecificationPropertyName.Target.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.ProcessingInformation, 5, True, True, DimensionSpecificationPropertyName.Processing_Information.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.ToleranceIndication, 6, True, True, DimensionSpecificationPropertyName.Tolerance_indication.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.AliasId, 7, True, True, DimensionSpecificationPropertyName.Alias_Id.ToString)
        End With
    End Sub

End Class