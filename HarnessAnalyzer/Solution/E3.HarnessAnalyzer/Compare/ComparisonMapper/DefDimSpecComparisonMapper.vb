
Imports Zuken.E3.HarnessAnalyzer.Settings

<KblObjectType(KblObjectType.Default_dimension_specification)>
Public Class DefDimSpecComparisonMapper
    Inherits ComparisonMapper

    Public Sub New(activeContainer As KblMapper, compareContainer As KblMapper, currentActiveObjects As ICollection(Of String), compareActiveObjects As ICollection(Of String), settings As GeneralSettingsBase)
        MyBase.New(activeContainer, compareContainer, currentActiveObjects, compareActiveObjects, settings)
    End Sub

    Public Overrides Sub CompareObjects()
        For Each defDimSpec As Default_dimension_specification In _currentContainer.GetDefaultDimensionSpecifications
            With defDimSpec
                Dim dimValRange As String = If(.Dimension_value_range IsNot Nothing, String.Format("Min.: {0} / Max.: {1}", Math.Round(.Dimension_value_range.Minimum, 2), Math.Round(.Dimension_value_range.Maximum, 2)), String.Empty)

                If (dimValRange <> String.Empty) AndAlso (_currentActiveObjects.Contains(dimValRange)) Then
                    If (_compareActiveObjects.Contains(dimValRange)) AndAlso (_compareContainer.GetDefaultDimensionSpecifications.Any(Function(dds) dds.Dimension_value_range IsNot Nothing AndAlso String.Format("Min.: {0} / Max.: {1}", Math.Round(dds.Dimension_value_range.Minimum, 2), Math.Round(dds.Dimension_value_range.Maximum, 2)) = dimValRange)) Then
                        Dim compareDefDimSpec As Default_dimension_specification = _compareContainer.GetDefaultDimensionSpecifications.Where(Function(dds) dds.Dimension_value_range IsNot Nothing AndAlso String.Format("Min.: {0} / Max.: {1}", Math.Round(dds.Dimension_value_range.Minimum, 2), Math.Round(dds.Dimension_value_range.Maximum, 2)) = dimValRange).FirstOrDefault
                        If (compareDefDimSpec IsNot Nothing) AndAlso (Not .Equals(compareDefDimSpec)) Then
                            Dim changedProperty As DefDimSpecChangedProperty = CompareProperties(Of DefDimSpecChangedProperty)(defDimSpec, compareDefDimSpec)
                            If (changedProperty.ChangedProperties.Count <> 0) AndAlso (Not Me.Changes.ContainsModified(.SystemId)) Then
                                Me.AddOrReplaceChangeWithInverse(.SystemId, changedProperty, CompareChangeType.Modified)
                            End If
                        End If
                    Else
                        Me.AddOrReplaceChangeWithInverse(.SystemId, defDimSpec, CompareChangeType.Deleted)
                    End If
                End If
            End With
        Next

        For Each defDimSpec As Default_dimension_specification In _compareContainer.GetDefaultDimensionSpecifications
            Dim dimValRange As String = If(defDimSpec.Dimension_value_range IsNot Nothing, String.Format("Min.: {0} / Max.: {1}", Math.Round(defDimSpec.Dimension_value_range.Minimum, 2), Math.Round(defDimSpec.Dimension_value_range.Maximum, 2)), String.Empty)
            If (dimValRange <> String.Empty AndAlso _compareActiveObjects.Contains(dimValRange) AndAlso Not _currentActiveObjects.Contains(dimValRange)) OrElse (dimValRange <> String.Empty AndAlso _compareActiveObjects.Contains(dimValRange) AndAlso Not _currentContainer.GetDefaultDimensionSpecifications.Where(Function(dds) dds.Dimension_value_range IsNot Nothing AndAlso String.Format("Min.: {0} / Max.: {1}", Math.Round(dds.Dimension_value_range.Minimum, 2), Math.Round(dds.Dimension_value_range.Maximum, 2)) = dimValRange).Any()) Then
                Me.AddOrReplaceChangeWithInverse(defDimSpec.SystemId, defDimSpec, CompareChangeType.New)
            End If
        Next
    End Sub

End Class
