
Imports Zuken.E3.HarnessAnalyzer.Settings

<KblObjectType(KblObjectType.Fixing_occurrence)>
Public Class FixingComparisonMapper
    Inherits ComparisonMapper

    Public Sub New(activeContainer As KblMapper, compareContainer As KblMapper, currentActiveObjects As ICollection(Of String), compareActiveObjects As ICollection(Of String), settings As GeneralSettingsBase)
        MyBase.New(activeContainer, compareContainer, currentActiveObjects, compareActiveObjects, settings)
    End Sub

    Public Overrides Sub CompareObjects()
        For Each fixingOcc As Fixing_occurrence In _currentContainer.GetFixingOccurrences
            If (_currentActiveObjects.Contains(fixingOcc.Id)) Then
                With fixingOcc
                    If (_compareActiveObjects.Contains(.Id)) AndAlso (_compareContainer.GetFixingOccurrences.Any(Function(fix) fix.Id = .Id)) Then
                        Dim compareFixing As Fixing_occurrence = _compareContainer.GetFixingOccurrences.Where(Function(fix) fix.Id = .Id).First
                        If (compareFixing IsNot Nothing) AndAlso (Not .Equals(compareFixing)) Then
                            Dim changedProperty As FixingChangedProperty = CompareProperties(Of FixingChangedProperty)(fixingOcc, compareFixing)
                            If (changedProperty.ChangedProperties.Count <> 0) AndAlso (Not Me.Changes.ContainsModified(.SystemId)) Then
                                Me.AddOrReplaceChangeWithInverse(.SystemId, .SystemId, compareFixing.SystemId, changedProperty, CompareChangeType.Modified)
                            End If
                        End If
                    Else
                        Me.AddOrReplaceChangeWithInverse(.SystemId, .SystemId, "", fixingOcc, CompareChangeType.Deleted)
                    End If
                End With
            End If
        Next

        For Each fixingOcc As Fixing_occurrence In _compareContainer.GetFixingOccurrences
            If (_compareActiveObjects.Contains(fixingOcc.Id) AndAlso Not _currentActiveObjects.Contains(fixingOcc.Id)) OrElse (_compareActiveObjects.Contains(fixingOcc.Id) AndAlso Not _currentContainer.GetFixingOccurrences.Where(Function(fix) fix.Id = fixingOcc.Id).Any()) Then
                Me.AddOrReplaceChangeWithInverse(fixingOcc.SystemId, "", fixingOcc.SystemId, fixingOcc, CompareChangeType.New)
            End If
        Next
    End Sub

End Class
