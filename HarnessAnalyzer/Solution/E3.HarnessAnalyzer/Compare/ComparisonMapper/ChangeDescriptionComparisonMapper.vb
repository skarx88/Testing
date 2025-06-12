
Imports Zuken.E3.HarnessAnalyzer.Settings

<KblObjectType(KblObjectType.Change_description)>
Public Class ChangeDescriptionComparisonMapper
    Inherits ComparisonMapper

    Public Sub New(activeContainer As KblMapper, compareContainer As KblMapper, currentActiveObjects As ICollection(Of String), compareActiveObjects As ICollection(Of String), settings As GeneralSettingsBase)
        MyBase.New(Nothing, activeContainer, compareContainer, currentActiveObjects, compareActiveObjects, settings)
    End Sub

    Public Overrides Sub CompareObjects()
        For Each changeDescription As Change_description In _currentContainer.GetChangeDescriptions
            With changeDescription
                If (.Id IsNot Nothing) AndAlso (_currentActiveObjects.Contains(.Id)) Then
                    If (_compareActiveObjects.Contains(.Id)) AndAlso (_compareContainer.GetChangeDescriptions.Where(Function(changeDesc) changeDesc.Id IsNot Nothing AndAlso changeDesc.Id = .Id).Any()) Then
                        Dim compareChangeDescription As Change_description = _compareContainer.GetChangeDescriptions.Where(Function(changeDesc) changeDesc.Id IsNot Nothing AndAlso changeDesc.Id = .Id).First
                        If (compareChangeDescription IsNot Nothing) AndAlso (Not .Equals(compareChangeDescription)) Then
                            Dim changedProperty As ChangeDescriptionChangedProperty = CompareProperties(Of ChangeDescriptionChangedProperty)(changeDescription, compareChangeDescription)
                            If (changedProperty.ChangedProperties.Count <> 0) AndAlso (Not Me.Changes.ContainsModified(.SystemId)) Then
                                Me.AddOrReplaceChangeWithInverse(.SystemId, changedProperty, CompareChangeType.Modified)
                            End If
                        End If
                    Else
                        Me.AddOrReplaceChangeWithInverse(.SystemId, changeDescription, CompareChangeType.Deleted)
                    End If
                End If
            End With
        Next

        For Each changeDescription As Change_description In _compareContainer.GetChangeDescriptions
            If (changeDescription.Id IsNot Nothing AndAlso _compareActiveObjects.Contains(changeDescription.Id) AndAlso Not _currentActiveObjects.Contains(changeDescription.Id)) OrElse (changeDescription.Id IsNot Nothing AndAlso _compareActiveObjects.Contains(changeDescription.Id) AndAlso Not _currentContainer.GetChangeDescriptions.Where(Function(changeDesc) changeDesc.Id IsNot Nothing AndAlso changeDesc.Id = changeDescription.Id).Any()) Then
                Me.AddOrReplaceChangeWithInverse(changeDescription.SystemId, changeDescription, CompareChangeType.New)
            End If
        Next
    End Sub

End Class
