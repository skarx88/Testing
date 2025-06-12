
Imports Zuken.E3.HarnessAnalyzer.Settings

<KblObjectType(KblObjectType.Component_occurrence)>
Public Class ComponentComparisonMapper
    Inherits ComparisonMapper

    Public Sub New(activeContainer As KblMapper, compareContainer As KblMapper, currentActiveObjects As ICollection(Of String), compareActiveObjects As ICollection(Of String), settings As GeneralSettingsBase)
        MyBase.New(activeContainer, compareContainer, currentActiveObjects, compareActiveObjects, settings)
    End Sub

    Public Overrides Sub CompareObjects()
        For Each componentOcc As Component_occurrence In _currentContainer.GetComponentOccurrences
            If (_currentActiveObjects.Contains(componentOcc.Id)) Then
                With componentOcc
                    If (_compareActiveObjects.Contains(.Id)) AndAlso (_compareContainer.GetComponentOccurrences.Any(Function(comp) comp.Id = .Id)) Then
                        Dim compareComponent As Component_occurrence = _compareContainer.GetComponentOccurrences.Where(Function(comp) comp.Id = .Id).First
                        If (compareComponent IsNot Nothing) AndAlso (Not .Equals(compareComponent)) Then
                            Dim changedProperty As ComponentChangedProperty = CompareProperties(Of ComponentChangedProperty)(componentOcc, compareComponent)
                            If (changedProperty.ChangedProperties.Count <> 0) AndAlso (Not Me.Changes.ContainsModified(.SystemId)) Then
                                Me.AddOrReplaceChangeWithInverse(.SystemId, .SystemId, compareComponent.SystemId, changedProperty, CompareChangeType.Modified)
                            End If
                        End If
                    Else
                        Me.AddOrReplaceChangeWithInverse(.SystemId, .SystemId, "", componentOcc, CompareChangeType.Deleted)
                    End If
                End With
            End If
        Next

        For Each componentOcc As Component_occurrence In _compareContainer.GetComponentOccurrences
            If (_compareActiveObjects.Contains(componentOcc.Id) AndAlso Not _currentActiveObjects.Contains(componentOcc.Id)) OrElse (_compareActiveObjects.Contains(componentOcc.Id) AndAlso Not _currentContainer.GetComponentOccurrences.Where(Function(comp) comp.Id = componentOcc.Id).Any()) Then
                Me.AddOrReplaceChangeWithInverse(componentOcc.SystemId, "", componentOcc.SystemId, componentOcc, CompareChangeType.New)
            End If
        Next
    End Sub

End Class
