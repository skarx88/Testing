Imports Zuken.E3.Lib.Schema.Kbl.Properties

<KblObjectType(KblObjectType.[Module])>
Public Class ModuleGridAppearance
    Inherits GridAppearance

    Public Sub New()
        MyBase.New(KblObjectType.Module)
    End Sub

    Public Overrides Sub CreateDefaultTable()
        _gridTable = New GridTable(KblObjectType.Module, KblObjectType.Harness_module.ToLocalizedPluralString)
        With _gridTable.GridColumns
            .AddNew(Me, ObjectPropertyNameStrings.Abbreviation, 0, True, True, PartPropertyName.Abbreviation)
            .AddNew(Me, ObjectPropertyNameStrings.PartNumber, 1, True, True, PartPropertyName.Part_number)
            .AddNew(Me, ObjectPropertyNameStrings.Description, 2, True, True, PartPropertyName.Part_description)
            .AddNew(Me, ObjectPropertyNameStrings.ZGS, 3, True, True, "ZGS")
            .AddNew(Me, ObjectPropertyNameStrings.KEM, 4, True, True, "KEM")
            .AddNew(Me, ObjectPropertyNameStrings.MassInformation, 5, True, True, PartPropertyName.Mass_information)
            .AddNew(Me, ObjectPropertyNameStrings.ModelYear, 6, True, True, ModulePropertyName.Model_year)
            .AddNew(Me, ObjectPropertyNameStrings.DegreeOfMaturity, 7, True, True, PartPropertyName.Degree_of_maturity)
            .AddNew(Me, ObjectPropertyNameStrings.Version, 8, True, True, PartPropertyName.Version)
            .AddNew(Me, ObjectPropertyNameStrings.CarClassLevel2, 9, True, True, ModulePropertyName.Car_classification_level_2)
            .AddNew(Me, ObjectPropertyNameStrings.CarClassLevel3, 10, True, True, ModulePropertyName.Car_classification_level_3)
            .AddNew(Me, ObjectPropertyNameStrings.CarClassLevel4, 11, True, True, ModulePropertyName.Car_classification_level_4)
            .AddNew(Me, ObjectPropertyNameStrings.CompanyName, 12, True, True, PartPropertyName.Company_name)
            .AddNew(Me, ObjectPropertyNameStrings.ExternalReferences, 13, True, True, PartPropertyName.External_references)

            .AddNew(Me, ObjectPropertyNameStrings.AliasId, 14, True, True, PartPropertyName.Part_alias_ids)
            .AddNew(Me, ObjectPropertyNameStrings.PredecessorPartNumber, 15, True, True, PartPropertyName.Predecessor_part_number)
            .AddNew(Me, ObjectPropertyNameStrings.CopyrightNote, 16, True, True, PartPropertyName.Copyright_note)
            .AddNew(Me, ObjectPropertyNameStrings.MaterialInformation, 17, True, True, PartPropertyName.Material_information)
            .AddNew(Me, ObjectPropertyNameStrings.ProcessingInformation, 18, True, True, PartPropertyName.Part_processing_information)
            .AddNew(Me, ObjectPropertyNameStrings.ProjectNumber, 19, True, True, ModulePropertyName.Project_number)
            .AddNew(Me, ObjectPropertyNameStrings.Content, 20, True, True, ModulePropertyName.Content)
            .AddNew(Me, KblObjectType.Module_family.ToLocalizedPluralString, 21, True, True, ModulePropertyName.Of_family)
            .AddNew(Me, ObjectPropertyNameStrings.LogisticControlInfo, 22, True, True, ModulePropertyName.Logistic_control_information)
            .AddNew(Me, ObjectPropertyNameStrings.ModConfigType, 23, True, True, ModulePropertyName.Configuration_type)
            .AddNew(Me, ObjectPropertyNameStrings.PartNumberType, 24, True, True, PartPropertyName.Part_number_type)
            .AddNew(Me, ObjectPropertyNameStrings.LocalizedPartDescriptions, 25, True, True, PartPropertyName.Part_Localized_description)
        End With

        _gridTable.GridSubTable = New GridTable(KblObjectType.Change, KblObjectType.Module_change.ToLocalizedPluralString)
        With _gridTable.GridSubTable.GridColumns
            .AddNew(Me, ObjectPropertyNameStrings.Id, 0, True, True, ChangePropertyName.Id)
            .AddNew(Me, ObjectPropertyNameStrings.Description, 1, True, True, ChangePropertyName.Description)
            .AddNew(Me, ObjectPropertyNameStrings.ChangeRequest, 2, True, True, ChangePropertyName.Change_request)
            .AddNew(Me, ObjectPropertyNameStrings.ChangeDate, 3, True, True, ChangePropertyName.Change_date)
            .AddNew(Me, ObjectPropertyNameStrings.ResponsibleDesigner, 4, True, True, ChangePropertyName.Responsible_designer)
            .AddNew(Me, ObjectPropertyNameStrings.DesignerDepartment, 5, True, True, ChangePropertyName.Designer_department)
            .AddNew(Me, ObjectPropertyNameStrings.ApproverName, 6, True, True, ChangePropertyName.Approver_name)
            .AddNew(Me, ObjectPropertyNameStrings.ApproverDepartment, 7, True, True, ChangePropertyName.Approver_department)
        End With
    End Sub

End Class
