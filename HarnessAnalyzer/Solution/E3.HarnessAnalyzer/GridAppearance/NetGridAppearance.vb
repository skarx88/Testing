Imports Zuken.E3.Lib.Schema.Kbl.Properties

<KblObjectType(KblObjectType.Net)>
Public Class NetGridAppearance
    Inherits GridAppearance

    Public Sub New()
        MyBase.New(KblObjectType.Net)
    End Sub

    Public Overrides Sub CreateDefaultTable()
        _gridTable = New GridTable(KblObjectType.Net, KblObjectType.Net.ToLocalizedPluralString)
        _gridTable.GridColumns.AddNew(Me, ObjectPropertyNameStrings.Name, 0, True, True, ConnectionPropertyName.Signal_name)
        _gridTable.GridColumns.AddNew(Me, ObjectPropertyNameStrings.SignalType, 1, True, True, ConnectionPropertyName.Signal_type)

        _gridTable.GridSubTable = New GridTable(KblObjectType.Connection, KblObjectType.Connection.ToLocalizedPluralString)
        With _gridTable.GridSubTable.GridColumns
            .AddNew(Me, ObjectPropertyNameStrings.Id, 0, True, True, ConnectionPropertyName.Id)
            .AddNew(Me, ObjectPropertyNameStrings.Description, 1, True, True, ConnectionPropertyName.Description)
            .AddNew(Me, ObjectPropertyNameStrings.ExternalReferences, 2, True, True, ConnectionPropertyName.External_references)
            .AddNew(Me, KblObjectType.Wire_occurrence.ToLocalizedPluralString, 3, True, True, ConnectionPropertyName.Wire)
            .AddNew(Me, ObjectPropertyNameStrings.InstallationInformation, 4, True, True, ConnectionPropertyName.Installation_Information)
            .AddNew(Me, ObjectPropertyNameStrings.ProcessingInformation, 5, True, True, ConnectionPropertyName.Processing_Information)
            .AddNew(Me, ObjectPropertyNameStrings.NominalVoltage, 6, True, True, ConnectionPropertyName.Nominal_voltage)
        End With
    End Sub

End Class
