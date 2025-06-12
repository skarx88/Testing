Imports Zuken.E3.HarnessAnalyzer.Settings

<KblObjectType(KblObjectType.Cavity_occurrence)>
Public Class CavityComparisonMapper
    Inherits ComparisonMapper

    Public Sub New(owner As ComparisonMapper, activeContainer As KblMapper, compareContainer As KblMapper, currentActiveObjects As ICollection(Of String), compareActiveObjects As ICollection(Of String), fillListToDictionary As ListConvertToDictionary, settings As GeneralSettingsBase)
        MyBase.New(owner, activeContainer, compareContainer, currentActiveObjects, compareActiveObjects, fillListToDictionary, settings)
    End Sub

    Public Overrides Sub CompareObjects()
        For Each dictionaryEntry As KeyValuePair(Of String, Object) In _listConvertToDictionary.CurrentObjects
            If (_currentActiveObjects.Contains(dictionaryEntry.Key)) Then
                If (_compareActiveObjects.Contains(dictionaryEntry.Key)) AndAlso (_listConvertToDictionary.CompareObjects.ContainsKey(dictionaryEntry.Key)) Then
                    If (Not dictionaryEntry.Value.Equals(_listConvertToDictionary.CompareObjects(dictionaryEntry.Key))) Then
                        Dim changedProperty As CavityChangedProperty = CompareProperties(Of CavityChangedProperty)(dictionaryEntry.Value, _listConvertToDictionary.CompareObjects(dictionaryEntry.Key))
                        If (changedProperty.ChangedProperties.Count <> 0) Then
                            Dim cavity As Cavity_occurrence = DirectCast(dictionaryEntry.Value, Cavity_occurrence)
                            If (Not Me.Changes.ContainsModified(cavity.SystemId)) Then
                                Me.AddOrReplaceChangeWithInverse(cavity.SystemId, changedProperty, CompareChangeType.Modified)
                            End If
                        End If
                    End If
                Else
                    Dim cavity As Cavity_occurrence = DirectCast(dictionaryEntry.Value, Cavity_occurrence)
                    Me.AddOrReplaceChangeWithInverse(cavity.SystemId, cavity, CompareChangeType.Deleted)
                End If
            End If
        Next

        For Each dictionaryEntry As KeyValuePair(Of String, Object) In _listConvertToDictionary.CompareObjects
            If (_compareActiveObjects.Contains(dictionaryEntry.Key) AndAlso Not _currentActiveObjects.Contains(dictionaryEntry.Key)) OrElse (_compareActiveObjects.Contains(dictionaryEntry.Key) AndAlso Not _listConvertToDictionary.CurrentObjects.ContainsKey(dictionaryEntry.Key)) Then
                Dim cavity As Cavity_occurrence = DirectCast(dictionaryEntry.Value, Cavity_occurrence)
                Me.AddOrReplaceChangeWithInverse(cavity.SystemId, cavity, CompareChangeType.[New])
            End If
        Next
    End Sub

End Class
