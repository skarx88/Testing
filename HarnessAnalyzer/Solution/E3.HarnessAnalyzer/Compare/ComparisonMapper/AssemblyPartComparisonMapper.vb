Imports Zuken.E3.HarnessAnalyzer.Settings

<KblObjectType(KblObjectType.Assembly_part_occurrence)>
Public Class AssemblyPartComparisonMapper
    Inherits ComparisonMapper

    Public Sub New(activeContainer As KblMapper, compareContainer As KblMapper, currentActiveObjects As ICollection(Of String), compareActiveObjects As ICollection(Of String), settings As GeneralSettingsBase)
        MyBase.New(activeContainer, compareContainer, currentActiveObjects, compareActiveObjects, settings)
    End Sub

    Public Overrides Sub CompareObjects()
        For Each assemblyPartOcc As Assembly_part_occurrence In _currentContainer.GetHarness.Assembly_part_occurrence
            If (_currentActiveObjects.Contains(assemblyPartOcc.Id)) Then
                With assemblyPartOcc
                    If (_compareActiveObjects.Contains(.Id)) AndAlso (_compareContainer.GetHarness.Assembly_part_occurrence.Any(Function(ass) ass.Id = .Id)) Then
                        Dim compareAssemblyPart As Assembly_part_occurrence = _compareContainer.GetHarness.Assembly_part_occurrence.Where(Function(ass) ass.Id = .Id).First
                        If (compareAssemblyPart IsNot Nothing) AndAlso (Not .Equals(compareAssemblyPart)) Then
                            Dim changedProperty As AssemblyPartChangedProperty = CompareProperties(Of AssemblyPartChangedProperty)(assemblyPartOcc, compareAssemblyPart)
                            If (changedProperty.ChangedProperties.Count <> 0) AndAlso (Not Me.Changes.ContainsModified(.SystemId)) Then
                                Me.AddOrReplaceChangeWithInverse(.SystemId, changedProperty, CompareChangeType.Modified)
                            End If
                        End If
                    Else
                        Me.AddOrReplaceChangeWithInverse(.SystemId, assemblyPartOcc, CompareChangeType.Deleted)
                    End If
                End With
            End If
        Next

        For Each assemblyPartOcc As Assembly_part_occurrence In _compareContainer.GetHarness.Assembly_part_occurrence
            If (_compareActiveObjects.Contains(assemblyPartOcc.Id) AndAlso Not _currentActiveObjects.Contains(assemblyPartOcc.Id)) OrElse (_compareActiveObjects.Contains(assemblyPartOcc.Id) AndAlso Not _currentContainer.GetHarness.Assembly_part_occurrence.Where(Function(ass) ass.Id = assemblyPartOcc.Id).Any()) Then
                Me.AddOrReplaceChangeWithInverse(assemblyPartOcc.SystemId, assemblyPartOcc, CompareChangeType.New)
            End If
        Next
    End Sub

End Class