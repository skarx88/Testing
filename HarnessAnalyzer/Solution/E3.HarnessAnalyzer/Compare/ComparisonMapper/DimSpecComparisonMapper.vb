
Imports Zuken.E3.HarnessAnalyzer.Settings

<KblObjectType(KblObjectType.Dimension_specification)>
Public Class DimSpecComparisonMapper
    Inherits ComparisonMapper

    Public Sub New(activeContainer As KblMapper, compareContainer As KblMapper, currentActiveObjects As ICollection(Of String), compareActiveObjects As ICollection(Of String), settings As GeneralSettingsBase)
        MyBase.New(activeContainer, compareContainer, currentActiveObjects, compareActiveObjects, settings)
    End Sub

    Public Overrides Sub CompareObjects()
        For Each dimSpec As Dimension_specification In _currentContainer.GetDimensionSpecifications
            With dimSpec
                If (.Id IsNot Nothing) AndAlso (_currentActiveObjects.Contains(.Id)) Then
                    If (_compareActiveObjects.Contains(.Id)) AndAlso (_compareContainer.GetDimensionSpecifications.Any(Function(ds) ds.Id IsNot Nothing AndAlso ds.Id = .Id)) Then
                        Dim compareDimSpec As Dimension_specification = _compareContainer.GetDimensionSpecifications.Where(Function(ds) ds.Id IsNot Nothing AndAlso ds.Id = .Id).FirstOrDefault
                        If (compareDimSpec IsNot Nothing) AndAlso (Not .Equals(compareDimSpec)) Then
                            Dim changedProperty As DimSpecChangedProperty = CompareProperties(Of DimSpecChangedProperty)(dimSpec, compareDimSpec)
                            If (changedProperty.ChangedProperties.Count <> 0) AndAlso (Not Me.Changes.ContainsModified(.SystemId)) Then Me.AddOrReplaceChangeWithInverse(.SystemId, changedProperty, CompareChangeType.Modified)
                        End If
                    Else
                        Me.AddOrReplaceChangeWithInverse(.SystemId, dimSpec, CompareChangeType.Deleted)
                    End If
                End If
            End With
        Next

        For Each dimSpec As Dimension_specification In _compareContainer.GetDimensionSpecifications
            If (dimSpec.Id IsNot Nothing AndAlso _compareActiveObjects.Contains(dimSpec.Id) AndAlso Not _currentActiveObjects.Contains(dimSpec.Id)) OrElse (dimSpec.Id IsNot Nothing AndAlso _compareActiveObjects.Contains(dimSpec.Id) AndAlso Not _currentContainer.GetDimensionSpecifications.Where(Function(ds) ds.Id IsNot Nothing AndAlso ds.Id = dimSpec.Id).Any()) Then Me.AddOrReplaceChangeWithInverse(dimSpec.SystemId, dimSpec, CompareChangeType.New)
        Next
    End Sub

End Class