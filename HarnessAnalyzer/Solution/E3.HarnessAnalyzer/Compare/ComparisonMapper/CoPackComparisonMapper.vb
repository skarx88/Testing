
Imports Zuken.E3.HarnessAnalyzer.Settings

<KblObjectType(KblObjectType.Co_pack_occurrence)>
Public Class CoPackComparisonMapper
    Inherits ComparisonMapper

    Public Sub New(activeContainer As KblMapper, compareContainer As KblMapper, currentActiveObjects As ICollection(Of String), compareActiveObjects As ICollection(Of String), settings As GeneralSettingsBase)
        MyBase.New(activeContainer, compareContainer, currentActiveObjects, compareActiveObjects, settings)
    End Sub

    Public Overrides Sub CompareObjects()
        For Each coPackOcc As Co_pack_occurrence In _currentContainer.GetHarness.Co_pack_occurrence
            If (_currentActiveObjects.Contains(coPackOcc.Id)) Then
                With coPackOcc
                    If (_compareActiveObjects.Contains(.Id)) AndAlso (_compareContainer.GetHarness.Co_pack_occurrence.Any(Function(coPkOcc) coPkOcc.Id = .Id)) Then
                        Dim compareCoPackOcc As Co_pack_occurrence = _compareContainer.GetHarness.Co_pack_occurrence.Where(Function(coPkOcc) coPkOcc.Id = .Id).First
                        If (compareCoPackOcc IsNot Nothing) AndAlso (Not .Equals(compareCoPackOcc)) Then
                            Dim changedProperty As CoPackChangedProperty = CompareProperties(Of CoPackChangedProperty)(coPackOcc, compareCoPackOcc)
                            If (changedProperty.ChangedProperties.Count <> 0) AndAlso (Not Me.Changes.ContainsModified(.SystemId)) Then
                                Me.AddOrReplaceChangeWithInverse(.SystemId, .SystemId, compareCoPackOcc.SystemId, changedProperty, CompareChangeType.Modified)
                            End If
                        End If
                    Else
                        Me.AddOrReplaceChangeWithInverse(.SystemId, coPackOcc, CompareChangeType.Deleted)
                    End If
                End With
            End If
        Next

        For Each coPackOcc As Co_pack_occurrence In _compareContainer.GetHarness.Co_pack_occurrence
            If (_compareActiveObjects.Contains(coPackOcc.Id) AndAlso Not _currentActiveObjects.Contains(coPackOcc.Id)) OrElse (_compareActiveObjects.Contains(coPackOcc.Id) AndAlso Not _currentContainer.GetHarness.Co_pack_occurrence.Where(Function(coPkOcc) coPkOcc.Id = coPackOcc.Id).Any()) Then
                Me.AddOrReplaceChangeWithInverse(coPackOcc.SystemId, coPackOcc, CompareChangeType.New)
            End If
        Next
    End Sub

End Class