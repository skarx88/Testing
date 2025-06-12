
Imports Zuken.E3.HarnessAnalyzer.Settings

<KblObjectType(KblObjectType.Special_wire_occurrence)>
Public Class CableComparisonMapper
    Inherits ComparisonMapper

    Public Sub New(activeContainer As KblMapper, compareContainer As KblMapper, currentActiveObjects As ICollection(Of String), compareActiveObjects As ICollection(Of String), settings As GeneralSettingsBase)
        MyBase.New(Nothing, activeContainer, compareContainer, currentActiveObjects, compareActiveObjects, settings)
    End Sub

    Public Overrides Sub CompareObjects()
        For Each cable As Special_wire_occurrence In _currentKBLMapper.KBLCableList
            If (_currentActiveObjects.Contains(cable.Special_wire_id)) Then
                With cable
                    If (_compareActiveObjects.Contains(.Special_wire_id)) AndAlso (_compareKBLMapper.KBLCableList.Any(Function(cab) cab.Special_wire_id = .Special_wire_id)) Then
                        Dim compareCable As Special_wire_occurrence = _compareKBLMapper.KBLCableList.Where(Function(cab) cab.Special_wire_id = .Special_wire_id).First
                        If (compareCable IsNot Nothing) AndAlso (Not .Equals(compareCable)) Then
                            Dim changedProperty As CableChangedProperty = CompareProperties(Of CableChangedProperty)(cable, compareCable)
                            If (changedProperty.ChangedProperties.Count <> 0) AndAlso (Not Me.Changes.ContainsModified(.SystemId)) Then
                                Me.AddOrReplaceChangeWithInverse(.SystemId, .SystemId, compareCable.SystemId, changedProperty, CompareChangeType.Modified)
                            End If
                        End If
                    Else
                        Me.AddOrReplaceChangeWithInverse(.SystemId, .SystemId, "", cable, CompareChangeType.Deleted)
                    End If
                End With
            End If
        Next

        For Each cable As Special_wire_occurrence In _compareKBLMapper.KBLCableList
            If (_compareActiveObjects.Contains(cable.Special_wire_id) AndAlso Not _currentActiveObjects.Contains(cable.Special_wire_id)) OrElse (_compareActiveObjects.Contains(cable.Special_wire_id) AndAlso Not _currentKBLMapper.KBLCableList.Where(Function(cab) cab.Special_wire_id = cable.Special_wire_id).Any()) Then
                Me.AddOrReplaceChangeWithInverse(cable.SystemId, "", cable.SystemId, cable, CompareChangeType.New)
            End If
        Next
    End Sub

End Class
