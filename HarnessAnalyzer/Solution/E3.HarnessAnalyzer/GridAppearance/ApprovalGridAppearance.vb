Imports Zuken.E3.Lib.Schema.Kbl.Properties

<KblObjectType(KblObjectType.Approval)>
Public Class ApprovalGridAppearance
    Inherits GridAppearance

    Public Sub New()
        MyBase.New(KblObjectType.Approval)
    End Sub

    Public Overrides Sub CreateDefaultTable()
        _gridTable = New GridTable(KblObjectType.Approval, KblObjectType.Approval.ToLocalizedPluralString)
        With _gridTable.GridColumns
            .AddNew(Me, ObjectPropertyNameStrings.Name, 0, True, True, ApprovalPropertyName.Name.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.Department, 1, True, True, ApprovalPropertyName.Department.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.DateOfApproval, 2, True, True, ApprovalPropertyName.Date.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.TypeOfApproval, 3, True, True, ApprovalPropertyName.Type_of_approval.ToString)
            .AddNew(Me, ObjectPropertyNameStrings.IsAppliedTo, 4, True, True, ApprovalPropertyName.Is_applied_to.ToString)
        End With
    End Sub

End Class