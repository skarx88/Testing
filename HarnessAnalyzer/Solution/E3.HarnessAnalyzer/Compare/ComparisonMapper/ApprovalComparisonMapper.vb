Imports Zuken.E3.HarnessAnalyzer.Settings

<KblObjectType(KblObjectType.Approval)>
Public Class ApprovalComparisonMapper
    Inherits ComparisonMapper

    Public Sub New(activeContainer As KblMapper, compareContainer As KblMapper, currentActiveObjects As ICollection(Of String), compareActiveObjects As ICollection(Of String), settings As GeneralSettingsBase)
        MyBase.New(activeContainer, compareContainer, currentActiveObjects, compareActiveObjects, settings)
    End Sub

    Public Overrides Sub CompareObjects()
        For Each approval As Approval In _currentContainer.GetApprovals
            With approval
                If (.Name IsNot Nothing) AndAlso (_currentActiveObjects.Contains(.Name)) Then
                    If (_compareActiveObjects.Contains(.Name)) AndAlso (_compareContainer.GetApprovals.Any(Function(appr) appr.Name IsNot Nothing AndAlso appr.Name = .Name)) Then
                        Dim compareApproval As Approval = TryCast(_compareContainer.GetApprovals.Where(Function(appr) appr.Name IsNot Nothing AndAlso appr.Name = .Name)(0), Approval)
                        If compareApproval IsNot Nothing AndAlso Not .Equals(compareApproval) Then
                            Dim changedProperty As ApprovalChangedProperty = CompareProperties(Of ApprovalChangedProperty)(approval, compareApproval)
                            If changedProperty.ChangedProperties.Count <> 0 AndAlso Not Me.Changes.ContainsModified(.SystemId) Then
                                Me.AddOrReplaceChangeWithInverse(.SystemId, changedProperty, CompareChangeType.Modified)
                            End If
                        End If
                    Else
                        Me.AddOrReplaceChangeWithInverse(.SystemId, approval, CompareChangeType.Deleted)
                    End If
                End If
            End With
        Next

        For Each approval As Approval In _compareContainer.GetApprovals
            If (approval.Name IsNot Nothing AndAlso _compareActiveObjects.Contains(approval.Name) AndAlso Not _currentActiveObjects.Contains(approval.Name)) OrElse (approval.Name IsNot Nothing AndAlso _compareActiveObjects.Contains(approval.Name) AndAlso Not _currentContainer.GetApprovals.Where(Function(appr) appr.Name IsNot Nothing AndAlso appr.Name = approval.Name).Any()) Then
                Me.AddOrReplaceChangeWithInverse(approval.SystemId, approval, CompareChangeType.New)
            End If
        Next
    End Sub

End Class