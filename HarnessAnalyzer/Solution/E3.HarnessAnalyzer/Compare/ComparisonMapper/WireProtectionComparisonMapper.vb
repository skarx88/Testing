Imports Zuken.E3.HarnessAnalyzer.Settings

Public Class WireProtectionComparisonMapper
    Inherits ComparisonMapper

    Public Sub New(owner As ComparisonMapper, activeContainer As KblMapper, compareContainer As KblMapper, currentActiveObjects As ICollection(Of String), compareActiveObjects As ICollection(Of String), settings As GeneralSettingsBase)
        MyBase.New(owner, activeContainer, compareContainer, currentActiveObjects, compareActiveObjects, settings)
    End Sub

    Public Overrides Sub CompareObjects()
        For Each wireProtOcc As Wire_protection_occurrence In _currentContainer.GetWireProtectionOccurrences
            If (_currentActiveObjects.Contains(wireProtOcc.Id)) Then
                With wireProtOcc
                    If (_compareActiveObjects.Contains(.Id)) AndAlso (_compareContainer.GetWireProtectionOccurrences.Any(Function(wirProt) wirProt.Id = .Id)) Then
                        Dim compareWireProtection As Wire_protection_occurrence = _compareContainer.GetWireProtectionOccurrences.Where(Function(wirProt) wirProt.Id = .Id).First
                        If (compareWireProtection IsNot Nothing) AndAlso (Not .Equals(compareWireProtection)) Then
                            Dim changedProperty As WireProtectionOccurrenceChangedProperty = CompareProperties(Of WireProtectionOccurrenceChangedProperty)(wireProtOcc, compareWireProtection)
                            If (changedProperty.ChangedProperties.Count <> 0) AndAlso (Not Me.Changes.ContainsModified(.SystemId)) Then
                                Me.AddOrReplaceChangeWithInverse(.SystemId, changedProperty, CompareChangeType.Modified)
                            End If
                        End If
                    Else
                        Me.AddOrReplaceChangeWithInverse(.SystemId, wireProtOcc, CompareChangeType.Deleted)
                    End If
                End With
            End If
        Next

        For Each wireProtOcc As Wire_protection_occurrence In _compareContainer.GetWireProtectionOccurrences
            If (_compareActiveObjects.Contains(wireProtOcc.Id) AndAlso Not _currentActiveObjects.Contains(wireProtOcc.Id)) OrElse (_compareActiveObjects.Contains(wireProtOcc.Id) AndAlso Not _currentContainer.GetWireProtectionOccurrences.Where(Function(wirProt) wirProt.Id = wireProtOcc.Id).Any()) Then
                Me.AddOrReplaceChangeWithInverse(wireProtOcc.SystemId, wireProtOcc, CompareChangeType.New)
            End If
        Next
    End Sub

End Class