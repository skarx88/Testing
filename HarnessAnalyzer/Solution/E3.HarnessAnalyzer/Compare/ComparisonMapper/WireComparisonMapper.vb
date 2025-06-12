Imports Zuken.E3.HarnessAnalyzer.Settings

<KblObjectType(KblObjectType.Wire_occurrence)>
Public Class WireComparisonMapper
    Inherits ComparisonMapper

    Public Sub New(activeContainer As KblMapper, compareContainer As KblMapper, currentActiveObjects As ICollection(Of String), compareActiveObjects As ICollection(Of String), settings As GeneralSettingsBase)
        MyBase.New(activeContainer, compareContainer, currentActiveObjects, compareActiveObjects, settings)
    End Sub

    Public Overrides Sub CompareObjects()
        For Each wireOcc As Wire_occurrence In _currentKBLMapper.KBLWireList
            If (_currentActiveObjects.Contains(wireOcc.Wire_number)) Then
                With wireOcc
                    If (_compareActiveObjects.Contains(.Wire_number)) AndAlso (_compareKBLMapper.KBLWireList.Any(Function(wire) wire.Wire_number = .Wire_number)) Then
                        Dim compareWire As Wire_occurrence = _compareKBLMapper.KBLWireList.Where(Function(wire) wire.Wire_number = .Wire_number).First
                        If (compareWire IsNot Nothing) AndAlso (Not .Equals(compareWire)) Then
                            Dim changedProperty As WireChangedProperty = CompareProperties(Of WireChangedProperty)(wireOcc, compareWire)
                            If (changedProperty.ChangedProperties.Count <> 0) AndAlso (Not Me.Changes.ContainsModified(.SystemId)) Then
                                Me.AddOrReplaceChangeWithInverse(.SystemId, .SystemId, compareWire.SystemId, changedProperty, CompareChangeType.Modified)
                            End If
                        End If
                    Else
                        Me.AddOrReplaceChangeWithInverse(.SystemId, .SystemId, "", wireOcc, CompareChangeType.Deleted)
                    End If
                End With
            End If
        Next

        For Each wireOcc As Wire_occurrence In _compareKBLMapper.KBLWireList
            If (_compareActiveObjects.Contains(wireOcc.Wire_number) AndAlso Not _currentActiveObjects.Contains(wireOcc.Wire_number)) OrElse (_compareActiveObjects.Contains(wireOcc.Wire_number) AndAlso Not _currentKBLMapper.KBLWireList.Where(Function(wire) wire.Wire_number = wireOcc.Wire_number).Any()) Then
                Me.AddOrReplaceChangeWithInverse(wireOcc.SystemId, "", wireOcc.SystemId, wireOcc, CompareChangeType.New)
            End If
        Next
    End Sub

End Class
