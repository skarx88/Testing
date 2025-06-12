Imports Zuken.E3.Lib.Schema.Kbl.Properties

'HINT There must not be a type descriptor as this special appearance is not the source of information to the compare

Public Class D3DSupplementTableAppearance
    Inherits GridAppearance

    Public Sub New()
        MyBase.New(KblObjectType.Supplement_occurrence_D3D)
    End Sub

    Public Overrides Sub CreateDefaultTable()
        _gridTable = New GridTable(KblObjectType.Supplement_occurrence_D3D, KblObjectType.Supplement_occurrence_D3D.ToLocalizedPluralString)
        With _gridTable.GridColumns
            .AddNew(Me, ObjectPropertyNameStrings.Id, 0, True, True, AccessoryPropertyName.Id.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.PartDescription, 1, True, True, PartPropertyName.Part_description.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.PartNumber, 2, True, True, PartPropertyName.Part_number.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.HarnessModule, 3, True, True, HarnessAnalyzer.Shared.MODULE_KEY)
        End With
    End Sub
End Class
