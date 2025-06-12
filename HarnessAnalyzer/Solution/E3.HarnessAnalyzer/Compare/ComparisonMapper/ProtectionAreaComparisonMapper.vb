Imports Zuken.E3.HarnessAnalyzer.Settings

Public Class ProtectionAreaComparisonMapper
    Inherits ComparisonMapper

    Public Sub New(owner As ComparisonMapper, fillListToDictionary As ListConvertToDictionary, currentKBLMapper As KblMapper, compareKBLMapper As KblMapper, currentActiveObjects As ICollection(Of String), compareActiveObjects As ICollection(Of String), settings As GeneralSettingsBase)
        MyBase.New(owner, fillListToDictionary, currentActiveObjects, compareActiveObjects, settings)
        _currentKBLMapper = currentKBLMapper
        _compareKBLMapper = compareKBLMapper
    End Sub

    Public Overrides Sub CompareObjects()
        'Hint this compare is special as the protection areas have no user ID and hence the Id from the protection itself is used.
        'Activity checks need to be done via the system id of the protection occurences.
        For Each dictionaryEntry As KeyValuePair(Of String, Object) In _listConvertToDictionary.CurrentObjects
            Dim currProtId As String = CType(dictionaryEntry.Value, Protection_area).Associated_protection
            If (_currentActiveObjects.Contains(currProtId)) Then
                If _listConvertToDictionary.CompareObjects.ContainsKey(dictionaryEntry.Key) Then
                    Dim compProtId As String = CType(_listConvertToDictionary.CompareObjects(dictionaryEntry.Key), Protection_area).Associated_protection
                    If _compareActiveObjects.Contains(compProtId) Then
                        If (Not dictionaryEntry.Value.Equals(_listConvertToDictionary.CompareObjects(dictionaryEntry.Key))) Then
                            Dim changedProperty As ProtectionAreaChangedProperty = CompareProperties(Of ProtectionAreaChangedProperty)(dictionaryEntry.Value, _listConvertToDictionary.CompareObjects(dictionaryEntry.Key))
                            If (changedProperty.ChangedProperties.Count <> 0) AndAlso (Not Me.Changes.ContainsModified(dictionaryEntry.Key)) Then
                                Me.AddOrReplaceChangeWithInverse(currProtId, currProtId, compProtId, changedProperty, CompareChangeType.Modified)
                            End If
                        End If
                    Else
                        Me.AddOrReplaceChangeWithInverse(currProtId, currProtId, "", dictionaryEntry.Value, CompareChangeType.Deleted)
                    End If
                Else
                    Me.AddOrReplaceChangeWithInverse(currProtId, currProtId, "", dictionaryEntry.Value, CompareChangeType.Deleted)
                End If

            Else
                Me.AddOrReplaceChangeWithInverse(currProtId, currProtId, "", dictionaryEntry.Value, CompareChangeType.Deleted)
            End If
        Next

        For Each dictionaryEntry As KeyValuePair(Of String, Object) In _listConvertToDictionary.CompareObjects
            Dim compProtId As String = CType(dictionaryEntry.Value, Protection_area).Associated_protection
            If (_compareActiveObjects.Contains(compProtId)) Then
                If _listConvertToDictionary.CurrentObjects.ContainsKey(dictionaryEntry.Key) Then
                    Dim currProtId As String = CType(_listConvertToDictionary.CurrentObjects(dictionaryEntry.Key), Protection_area).Associated_protection
                    If Not _currentActiveObjects.Contains(currProtId) Then
                        Me.AddOrReplaceChangeWithInverse(compProtId, "", compProtId, dictionaryEntry.Value, CompareChangeType.New)
                    End If
                Else
                    Me.AddOrReplaceChangeWithInverse(compProtId, "", compProtId, dictionaryEntry.Value, CompareChangeType.New)
                End If
            End If
        Next
    End Sub

End Class
