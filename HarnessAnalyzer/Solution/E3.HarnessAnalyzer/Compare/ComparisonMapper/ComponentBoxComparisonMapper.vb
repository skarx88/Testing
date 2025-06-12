
Imports Zuken.E3.HarnessAnalyzer.Settings

<KblObjectType(KblObjectType.Component_box_occurrence)>
Public Class ComponentBoxComparisonMapper
    Inherits ComparisonMapper

    Public Sub New(activeContainer As KblMapper, compareContainer As KblMapper, currentActiveObjects As ICollection(Of String), compareActiveObjects As ICollection(Of String), settings As GeneralSettingsBase)
        MyBase.New(activeContainer, compareContainer, currentActiveObjects, compareActiveObjects, settings)
    End Sub

    Public Overrides Sub CompareObjects()
        For Each componentBoxOcc As Component_box_occurrence In _currentContainer.GetHarness.Component_box_occurrence
            If (_currentActiveObjects.Contains(componentBoxOcc.Id)) Then
                With componentBoxOcc
                    If (_compareActiveObjects.Contains(.Id)) AndAlso (_compareContainer.GetHarness.Component_box_occurrence.Any(Function(cmpBoxOcc) cmpBoxOcc.Id = .Id)) Then
                        Dim compareComponentBox As Component_box_occurrence = _compareContainer.GetHarness.Component_box_occurrence.Where(Function(cmpBoxOcc) cmpBoxOcc.Id = .Id).First
                        If (compareComponentBox IsNot Nothing) AndAlso (Not .Equals(compareComponentBox)) Then
                            Dim changedProperty As ComponentBoxChangedProperty = CompareProperties(Of ComponentBoxChangedProperty)(componentBoxOcc, compareComponentBox)
                            If (changedProperty.ChangedProperties.Count <> 0) AndAlso (Not Me.Changes.ContainsModified(.SystemId)) Then

                                Me.AddOrReplaceChangeWithInverse(.SystemId, .SystemId, compareComponentBox.SystemId, changedProperty, CompareChangeType.Modified)
                            End If
                        End If
                    Else

                        Me.AddOrReplaceChangeWithInverse(.SystemId, .SystemId, "", componentBoxOcc, CompareChangeType.Deleted)
                    End If
                End With
            End If
        Next

        For Each componentBoxOcc As Component_box_occurrence In _compareContainer.GetHarness.Component_box_occurrence
            If (_compareActiveObjects.Contains(componentBoxOcc.Id) AndAlso Not _currentActiveObjects.Contains(componentBoxOcc.Id)) OrElse (_compareActiveObjects.Contains(componentBoxOcc.Id) AndAlso Not _currentContainer.GetHarness.Component_box_occurrence.Where(Function(cmpBoxOcc) cmpBoxOcc.Id = componentBoxOcc.Id).Any()) Then
                Me.AddOrReplaceChangeWithInverse(componentBoxOcc.SystemId, "", componentBoxOcc.SystemId, componentBoxOcc, CompareChangeType.New)
            End If
        Next
    End Sub

End Class