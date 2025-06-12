Imports System.Reflection
Imports Zuken.E3.HarnessAnalyzer.Settings


Public Class ApprovalChangedProperty
    Inherits ChangedProperty

    Public Sub New(owner As ComparisonMapper, currentKBLMapper As KblMapper, compareKBLMapper As KblMapper, settings As GeneralSettingsBase)
        MyBase.New(owner, currentKBLMapper, compareKBLMapper, settings)
    End Sub

    Public Overrides Function CompareObjectProperty(objProperty As PropertyInfo, currentObject As Object, compareObject As Object, excludeProperties As List(Of String)) As Boolean
        Dim currentProperty As Object = objProperty.GetValue(currentObject, Nothing)
        Dim compareProperty As Object = objProperty.GetValue(compareObject, Nothing)
        If (currentProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(Approval.Department), NameOf(Approval.Date), NameOf(Approval.Type_of_approval)
                    If (compareProperty Is Nothing) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, String.Empty)
                    ElseIf (currentProperty.ToString <> compareProperty.ToString) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, compareProperty.ToString)
                    End If
                Case NameOf(Approval.Is_applied_to)
                    If (compareProperty Is Nothing) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, String.Empty)
                    Else
                        Dim currentAppliedId As String = String.Empty
                        Dim compareAppliedId As String = String.Empty

                        If (_currentKBLMapper.GetHarness.SystemId = currentProperty.ToString) Then
                            currentAppliedId = String.Format("{0} [{1}]", _currentKBLMapper.HarnessPartNumber, KblObjectType.Harness.ToLocalizedString)
                        ElseIf (_currentKBLMapper.GetHarnessConfiguration(currentProperty.ToString) IsNot Nothing) Then
                            currentAppliedId = String.Format("{0} [{1}]", _currentKBLMapper.GetHarnessConfiguration(currentProperty.ToString).Part_number, KblObjectType.Module_configuration.ToLocalizedString)
                        ElseIf (_currentKBLMapper.GetModule(currentProperty.ToString) IsNot Nothing) Then
                            currentAppliedId = String.Format("{0} [{1}]", _currentKBLMapper.GetModule(currentProperty.ToString).Part_number, KblObjectType.Harness_module.ToLocalizedString)
                        End If

                        If (_compareKBLMapper.GetHarness.SystemId = compareProperty.ToString) Then
                            compareAppliedId = String.Format("{0} [{1}]", _compareKBLMapper.HarnessPartNumber, KblObjectType.Harness.ToLocalizedString)
                        ElseIf (_compareKBLMapper.GetHarnessConfiguration(compareProperty.ToString) IsNot Nothing) Then
                            compareAppliedId = String.Format("{0} [{1}]", _compareKBLMapper.GetHarnessConfiguration(compareProperty.ToString).Part_number, KblObjectType.Module_configuration.ToLocalizedString)
                        ElseIf (_compareKBLMapper.GetModule(compareProperty.ToString) IsNot Nothing) Then
                            compareAppliedId = String.Format("{0} [{1}]", _compareKBLMapper.GetModule(compareProperty.ToString).Part_number, KblObjectType.Harness_module.ToLocalizedString)
                        End If

                        If (currentAppliedId <> compareAppliedId) Then
                            MyBase.ChangedProperties.Add(objProperty.Name, compareAppliedId)
                        End If
                    End If
            End Select
        ElseIf (compareProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(Approval.Department), NameOf(Approval.Date), NameOf(Approval.Type_of_approval)
                    MyBase.ChangedProperties.Add(objProperty.Name.ToString, compareProperty)
                Case NameOf(Approval.Is_applied_to)
                    Dim compareAppliedId As String = String.Empty

                    If (_compareKBLMapper.GetHarness.SystemId = compareProperty.ToString) Then
                        compareAppliedId = String.Format("{0} [{1}]", _compareKBLMapper.HarnessPartNumber, KblObjectType.Harness.ToLocalizedString)
                    ElseIf (_compareKBLMapper.GetHarnessConfiguration(compareProperty.ToString) IsNot Nothing) Then
                        compareAppliedId = String.Format("{0} [{1}]", _compareKBLMapper.GetHarnessConfiguration(compareProperty.ToString).Part_number, KblObjectType.Module_configuration.ToLocalizedString)
                    ElseIf (_compareKBLMapper.GetModule(compareProperty.ToString) IsNot Nothing) Then
                        compareAppliedId = String.Format("{0} [{1}]", _compareKBLMapper.GetModule(compareProperty.ToString).Part_number, KblObjectType.Harness_module.ToLocalizedString)
                    End If

                    MyBase.ChangedProperties.Add(objProperty.Name, compareAppliedId)
            End Select
        End If
        Return False
    End Function

End Class