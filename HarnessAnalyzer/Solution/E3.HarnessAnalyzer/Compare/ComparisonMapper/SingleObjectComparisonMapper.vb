Imports Zuken.E3.HarnessAnalyzer.Settings

Public Class SingleObjectComparisonMapper
    Inherits ComparisonMapper

    Public Sub New(owner As ComparisonMapper, currentObject As Object, compareObject As Object, activeContainer As KblMapper, compareContainer As KblMapper, settings As GeneralSettingsBase)
        MyBase.New(owner, currentObject, compareObject, activeContainer, compareContainer, settings)
    End Sub

    Public Overrides Sub CompareObjects()
        If _currentObject IsNot Nothing Then
            If _compareObject IsNot Nothing Then
                If (TypeOf _currentObject Is Part) AndAlso (TypeOf _compareObject Is Part) Then
                    Dim changedProperty As PartChangedProperty = CompareProperties(Of PartChangedProperty)(_currentObject, _compareObject)
                    If (changedProperty.ChangedProperties.Count <> 0) AndAlso (Not Me.Changes.ContainsModified(_currentObject.ToString)) Then
                        Me.AddOrReplaceChangeWithInverse(_currentObject.ToString, changedProperty, CompareChangeType.Modified)
                    End If
                ElseIf (TypeOf _currentObject Is Value_range) AndAlso (TypeOf _compareObject Is Value_range) Then
                    Dim changedProperty As CommonChangedProperty = CompareProperties(Of CommonChangedProperty)(_currentObject, _compareObject)
                    If (changedProperty.ChangedProperties.Count <> 0) AndAlso (Not Me.Changes.ContainsModified(_currentObject.ToString)) Then
                        Me.AddOrReplaceChangeWithInverse(_currentObject.ToString, changedProperty, CompareChangeType.Modified)
                    End If
                ElseIf (TypeOf _currentObject Is Wiring_group) AndAlso (TypeOf _compareObject Is Wiring_group) Then
                    Dim changedProperty As WiringGroupChangedProperty = CompareProperties(Of WiringGroupChangedProperty)(_currentObject, _compareObject)
                    If (changedProperty.ChangedProperties.Count <> 0) AndAlso (Not Me.Changes.ContainsModified(_currentObject.ToString)) Then
                        Me.AddOrReplaceChangeWithInverse(_currentObject.ToString, changedProperty, CompareChangeType.Modified)
                    End If
                End If
            Else
                Me.AddOrReplaceChangeWithInverse(_currentObject.ToString, _currentObject, CompareChangeType.Deleted)
            End If
        Else
            If _compareObject IsNot Nothing Then
                Me.AddOrReplaceChangeWithInverse(_compareObject.ToString, _compareObject, CompareChangeType.New)
            End If
        End If
    End Sub


End Class
