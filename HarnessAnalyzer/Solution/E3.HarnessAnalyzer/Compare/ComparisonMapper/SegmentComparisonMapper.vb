
Imports Zuken.E3.HarnessAnalyzer.Settings

<KblObjectType(KblObjectType.Segment)>
Public Class SegmentComparisonMapper
    Inherits ComparisonMapper

    Public Sub New(activeContainer As KblMapper, compareContainer As KblMapper, currentActiveObjects As ICollection(Of String), compareActiveObjects As ICollection(Of String), settings As GeneralSettingsBase)
        MyBase.New(activeContainer, compareContainer, currentActiveObjects, compareActiveObjects, settings)
    End Sub

    Public Overrides Sub CompareObjects()
        For Each segment As Segment In _currentContainer.GetSegments
            If (_currentActiveObjects.Contains(segment.Id)) Then
                With segment
                    If (_compareActiveObjects.Contains(.Id)) AndAlso (_compareContainer.GetSegments.Any(Function(seg) seg.Id = .Id)) Then
                        Dim compareSegment As Segment = _compareContainer.GetSegments.Where(Function(seg) seg.Id = .Id).First
                        If (compareSegment IsNot Nothing) AndAlso (Not .Equals(compareSegment)) Then
                            Dim changedProperty As SegmentChangedProperty = CompareProperties(Of SegmentChangedProperty)(segment, compareSegment)
                            If (changedProperty.ChangedProperties.Count <> 0) AndAlso (Not Me.Changes.ContainsModified(.SystemId)) Then
                                Me.AddOrReplaceChangeWithInverse(.SystemId, .SystemId, compareSegment.SystemId, changedProperty, CompareChangeType.Modified)
                            End If
                        End If
                    Else
                        Me.AddOrReplaceChangeWithInverse(.SystemId, .SystemId, "", segment, CompareChangeType.Deleted)
                    End If
                End With
            End If
        Next

        For Each segment As Segment In _compareContainer.GetSegments
            If (_compareActiveObjects.Contains(segment.Id) AndAlso Not _currentActiveObjects.Contains(segment.Id)) OrElse (_compareActiveObjects.Contains(segment.Id) AndAlso Not _currentContainer.GetSegments.Where(Function(seg) seg.Id = segment.Id).Any()) Then
                Me.AddOrReplaceChangeWithInverse(segment.SystemId, "", segment.SystemId, segment, CompareChangeType.New)
            End If
        Next
    End Sub

End Class
