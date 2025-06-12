Imports Zuken.E3.HarnessAnalyzer.Settings

Public Class CommonComparisonMapper
    Inherits ComparisonMapper

    Public Sub New(owner As ComparisonMapper, activeContainer As KblMapper, compareContainer As KblMapper, currentActiveObjects As ICollection(Of String), compareActiveObjects As ICollection(Of String), fillListToDictionary As ListConvertToDictionary, settings As GeneralSettingsBase)
        MyBase.New(owner, activeContainer, compareContainer, currentActiveObjects, compareActiveObjects, fillListToDictionary, settings)
    End Sub

    Public Overrides Sub CompareObjects()
        For Each dictionaryEntry As KeyValuePair(Of String, Object) In _listConvertToDictionary.CurrentObjects
            If _listConvertToDictionary.CompareObjects.ContainsKey(dictionaryEntry.Key) Then
                If dictionaryEntry.Value.Equals(_listConvertToDictionary.CompareObjects(dictionaryEntry.Key)) = False Then
                    Dim changedProperty As New CommonChangedProperty(Me, _currentKBLMapper, _compareKBLMapper, Me.Settings)

                    If (TypeOf dictionaryEntry.Value Is String) AndAlso (TypeOf _listConvertToDictionary.CompareObjects(dictionaryEntry.Key) Is String) Then
                        changedProperty.ChangedProperties.Add("StringValue", _listConvertToDictionary.CompareObjects(dictionaryEntry.Key))
                    Else
                        changedProperty = CompareProperties(Of CommonChangedProperty)(dictionaryEntry.Value, _listConvertToDictionary.CompareObjects(dictionaryEntry.Key))
                    End If

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
