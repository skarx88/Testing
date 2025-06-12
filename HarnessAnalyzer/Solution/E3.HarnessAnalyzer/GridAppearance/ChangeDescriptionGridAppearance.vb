Imports Zuken.E3.Lib.Schema.Kbl.Properties

<KblObjectType(KblObjectType.Change_description)>
Public Class ChangeDescriptionGridAppearance
    Inherits GridAppearance

    Public Sub New()
        MyBase.New(KblObjectType.Change_description)
    End Sub

    Public Overrides Sub CreateDefaultTable()
        _gridTable = New GridTable(KblObjectType.Change_description, KblObjectType.Change_description.ToLocalizedPluralString)
        With _gridTable.GridColumns
            .AddNew(Me, ObjectPropertyNameStrings.Id, 0, True, True, ChangePropertyName.Id.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.Description, 1, True, True, ChangePropertyName.Description.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.ChangeRequest, 2, True, True, ChangePropertyName.Change_request.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.ChangeDate, 3, True, True, ChangePropertyName.Change_date.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.ResponsibleDesigner, 4, True, True, ChangePropertyName.Responsible_designer.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.DesignerDepartment, 5, True, True, ChangePropertyName.Designer_department.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.ApproverName, 6, True, True, ChangePropertyName.Approver_name.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.ApproverDepartment, 7, True, True, ChangePropertyName.Approver_department.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.ChangedElements, 8, True, True, ChangeDescriptionPropertyName.Changed_elements.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.RelatedChanges, 9, True, True, ChangeDescriptionPropertyName.Related_changes.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.ExternalReferences, 10, True, True, ChangeDescriptionPropertyName.External_references.ToString)
        End With
    End Sub

End Class