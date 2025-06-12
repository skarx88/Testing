Imports Zuken.E3.HarnessAnalyzer.Settings

Public Class ContactPointComparisonMapper
    Inherits ComparisonMapper

    Public Sub New(owner As ComparisonMapper, activeContainer As KblMapper, compareContainer As KblMapper, currentActiveObjects As ICollection(Of String), compareActiveObjects As ICollection(Of String), fillListToDictionary As ListConvertToDictionary, settings As GeneralSettingsBase)
        MyBase.New(owner, activeContainer, compareContainer, currentActiveObjects, compareActiveObjects, fillListToDictionary, settings)
    End Sub

    Public Overrides Sub CompareObjects()
        For Each dictionaryEntry As KeyValuePair(Of String, Object) In _listConvertToDictionary.CurrentObjects
            If (_currentActiveObjects.Contains(dictionaryEntry.Key)) Then
                If (_compareActiveObjects.Contains(dictionaryEntry.Key)) AndAlso (_listConvertToDictionary.CompareObjects.ContainsKey(dictionaryEntry.Key)) Then
                    If (Not dictionaryEntry.Value.Equals(_listConvertToDictionary.CompareObjects(dictionaryEntry.Key))) Then
                        Dim changedProperty As ContactPointChangedProperty = CompareProperties(Of ContactPointChangedProperty)(dictionaryEntry.Value, _listConvertToDictionary.CompareObjects(dictionaryEntry.Key))
                        If (changedProperty.ChangedProperties.Count <> 0) Then
                            Dim contactPoint As Contact_point = DirectCast(dictionaryEntry.Value, Contact_point)
                            If (Not Me.Changes.ContainsModified(contactPoint.SystemId)) Then
                                Me.AddOrReplaceChangeWithInverse(contactPoint.SystemId, changedProperty, CompareChangeType.Modified)
                            End If
                        End If
                    End If
                Else
                    Dim contactPoint As Contact_point = DirectCast(dictionaryEntry.Value, Contact_point)
                    Me.AddOrReplaceChangeWithInverse(contactPoint.SystemId, dictionaryEntry.Value, CompareChangeType.Deleted)
                End If
            End If
        Next

        For Each dictionaryEntry As KeyValuePair(Of String, Object) In _listConvertToDictionary.CompareObjects
            If (_compareActiveObjects.Contains(dictionaryEntry.Key) AndAlso Not _currentActiveObjects.Contains(dictionaryEntry.Key)) OrElse (_compareActiveObjects.Contains(dictionaryEntry.Key) AndAlso Not _listConvertToDictionary.CurrentObjects.ContainsKey(dictionaryEntry.Key)) Then
                Dim contactPoint As Contact_point = DirectCast(dictionaryEntry.Value, Contact_point)
                Me.AddOrReplaceChangeWithInverse(contactPoint.SystemId, dictionaryEntry.Value, CompareChangeType.New)
            End If
        Next
    End Sub

End Class