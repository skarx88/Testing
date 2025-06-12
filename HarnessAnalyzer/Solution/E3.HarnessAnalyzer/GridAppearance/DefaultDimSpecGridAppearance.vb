Imports Zuken.E3.Lib.Schema.Kbl.Properties

<KblObjectType(KblObjectType.Default_dimension_specification)>
Public Class DefaultDimSpecGridAppearance
    Inherits GridAppearance

    Public Sub New()
        MyBase.New(KblObjectType.Default_dimension_specification)
    End Sub

    Public Overrides Sub CreateDefaultTable()
        _gridTable = New GridTable(KblObjectType.Default_dimension_specification, KblObjectType.Default_dimension_specification.ToLocalizedPluralString)
        With _gridTable.GridColumns
            .AddNew(Me, ObjectPropertyNameStrings.DimValueRange, 0, True, True, DefaultDimensionSpecificationPropertyName.Dimension_value_range.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.ToleranceType, 1, True, True, DefaultDimensionSpecificationPropertyName.Tolerance_type.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.ExternalReferences, 2, True, True, DefaultDimensionSpecificationPropertyName.External_references.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.ToleranceIndication, 3, True, True, DefaultDimensionSpecificationPropertyName.Tolerance_indication.ToString)
        End With
    End Sub

End Class