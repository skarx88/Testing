
Imports Zuken.E3.HarnessAnalyzer.Settings

<KblObjectType(KblObjectType.Net)>
Public Class NetComparisonMapper
    Inherits ComparisonMapper

    Public Sub New(activeContainer As KblMapper, compareContainer As KblMapper, currentActiveObjects As ICollection(Of String), compareActiveObjects As ICollection(Of String), settings As GeneralSettingsBase)
        MyBase.New(activeContainer, compareContainer, currentActiveObjects, compareActiveObjects, settings)
    End Sub

    Public Overrides Sub CompareObjects()
        For Each signalName As String In _currentKBLMapper.KBLNetList
            If (_currentKBLMapper.KBLNetMapper.ContainsKey(signalName) AndAlso _currentKBLMapper.KBLNetMapper(signalName).Any(Function(e) _currentActiveObjects.Contains(e.Id))) Then
                If (_compareKBLMapper.KBLNetMapper.ContainsKey(signalName) AndAlso _compareKBLMapper.KBLNetMapper(signalName).Any(Function(e) _compareActiveObjects.Contains(e.Id))) Then
                    Dim changedProperty As NetChangedProperty = CompareProperties(Of NetChangedProperty)(_currentKBLMapper.KBLNetMapper(signalName), _compareKBLMapper.KBLNetMapper(signalName))
                    If (changedProperty.ChangedProperties.Count <> 0) AndAlso (Not Me.Changes.ContainsModified(signalName)) Then
                        Me.AddOrReplaceChangeWithInverse(signalName, changedProperty, CompareChangeType.Modified)
                    End If
                Else
                    Me.AddOrReplaceChangeWithInverse(signalName, _currentKBLMapper.KBLNetMapper(signalName), CompareChangeType.Deleted)
                End If
            End If
        Next

        For Each signalName As String In _compareKBLMapper.KBLNetList
            If (_compareKBLMapper.KBLNetMapper.ContainsKey(signalName) AndAlso _compareKBLMapper.KBLNetMapper(signalName).Any(Function(e) _compareActiveObjects.Contains(e.Id))) Then
                If (Not (_currentKBLMapper.KBLNetMapper.ContainsKey(signalName) AndAlso _currentKBLMapper.KBLNetMapper(signalName).Any(Function(e) _currentActiveObjects.Contains(e.Id)))) Then
                    Me.AddOrReplaceChangeWithInverse(signalName, _compareKBLMapper.KBLNetMapper(signalName), CompareChangeType.New)
                End If
            End If
        Next
    End Sub

End Class
