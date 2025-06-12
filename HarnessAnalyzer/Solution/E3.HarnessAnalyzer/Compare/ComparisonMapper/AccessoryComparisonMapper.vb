Imports Zuken.E3.HarnessAnalyzer.Settings

<KblObjectType(KblObjectType.Accessory_occurrence)>
Public Class AccessoryComparisonMapper
    Inherits ComparisonMapper

    Public Sub New(activeContainer As KblMapper, compareContainer As KblMapper, currentActiveObjects As ICollection(Of String), compareActiveObjects As ICollection(Of String), settings As GeneralSettingsBase)
        MyBase.New(Nothing, activeContainer, compareContainer, currentActiveObjects, compareActiveObjects, settings)
    End Sub

    Public Overrides Sub CompareObjects()
        For Each accessoryOcc As Accessory_occurrence In _currentContainer.GetHarness.Accessory_occurrence
            If _currentActiveObjects.Contains(accessoryOcc.Id) Then
                With accessoryOcc
                    If (_compareActiveObjects.Contains(.Id)) AndAlso (_compareContainer.GetHarness.Accessory_occurrence.Any(Function(acc) acc.Id = .Id)) Then
                        Dim compareAccessory As Accessory_occurrence = _compareContainer.GetHarness.Accessory_occurrence.Where(Function(acc) acc.Id = .Id).First
                        If (compareAccessory IsNot Nothing) AndAlso (Not .Equals(compareAccessory)) Then
                            Dim changedProperty As AccessoryChangedProperty = CompareProperties(Of AccessoryChangedProperty)(accessoryOcc, compareAccessory)
                            If (changedProperty.ChangedProperties.Count <> 0) AndAlso (Not Me.Changes.ContainsModified(.SystemId)) Then
                                Me.AddOrReplaceChangeWithInverse(accessoryOcc.SystemId, accessoryOcc.SystemId, compareAccessory.SystemId, changedProperty, CompareChangeType.Modified)
                            End If
                        End If
                    Else
                        Me.AddOrReplaceChangeWithInverse(.SystemId, .SystemId, "", accessoryOcc, CompareChangeType.Deleted)
                    End If
                End With
            End If
        Next

        For Each accessoryOcc As Accessory_occurrence In _compareContainer.GetHarness.Accessory_occurrence
            If (_compareActiveObjects.Contains(accessoryOcc.Id) AndAlso Not _currentActiveObjects.Contains(accessoryOcc.Id)) OrElse (_compareActiveObjects.Contains(accessoryOcc.Id) AndAlso Not _currentContainer.GetAccessoryOccurrences.Where(Function(acc) acc.Id = accessoryOcc.Id).Any()) Then
                Me.AddOrReplaceChangeWithInverse(accessoryOcc.SystemId, "", accessoryOcc.SystemId, accessoryOcc, CompareChangeType.New)
            End If
        Next
    End Sub

End Class
