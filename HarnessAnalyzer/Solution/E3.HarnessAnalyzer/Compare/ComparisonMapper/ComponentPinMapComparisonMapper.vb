Imports Zuken.E3.HarnessAnalyzer.Settings

Public Class ComponentPinMapComparisonMapper
    Inherits ComparisonMapper

    Public Sub New(owner As ComparisonMapper, activeContainer As KblMapper, compareContainer As KblMapper, currentActiveObjects As ICollection(Of String), compareActiveObjects As ICollection(Of String), fillListToDictionary As ListConvertToDictionary, settings As GeneralSettingsBase)
        MyBase.New(owner, activeContainer, compareContainer, currentActiveObjects, compareActiveObjects, fillListToDictionary, settings)
    End Sub

    Public Overrides Sub CompareObjects()
        For Each dictionaryEntry As KeyValuePair(Of String, Object) In _listConvertToDictionary.CurrentObjects
            If (_listConvertToDictionary.CompareObjects.ContainsKey(dictionaryEntry.Key)) Then
                If (Not dictionaryEntry.Value.Equals(_listConvertToDictionary.CompareObjects(dictionaryEntry.Key))) Then
                    Dim changedProperty As ComponentPinMapChangedProperty = CompareProperties(Of ComponentPinMapChangedProperty)(dictionaryEntry.Value, _listConvertToDictionary.CompareObjects(dictionaryEntry.Key))
                    If (changedProperty.ChangedProperties.Count <> 0) AndAlso (Not Me.Changes.ContainsModified(dictionaryEntry.Key)) Then
                        Me.AddOrReplaceChangeWithInverse(dictionaryEntry.Key, changedProperty, CompareChangeType.Modified)
                    End If
                End If
            Else
                Me.AddOrReplaceChangeWithInverse(dictionaryEntry.Key, dictionaryEntry.Value, CompareChangeType.Deleted)
            End If
        Next

        For Each dictionaryEntry As KeyValuePair(Of String, Object) In _listConvertToDictionary.CompareObjects
            If (Not _listConvertToDictionary.CurrentObjects.ContainsKey(dictionaryEntry.Key)) Then
                Me.AddOrReplaceChangeWithInverse(dictionaryEntry.Key, dictionaryEntry.Value, CompareChangeType.New)
            End If
        Next
    End Sub
End Class
