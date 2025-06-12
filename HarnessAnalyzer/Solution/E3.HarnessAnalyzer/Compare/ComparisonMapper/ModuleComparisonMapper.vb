
Imports Zuken.E3.HarnessAnalyzer.Settings

<KblObjectType(KblObjectType.[Module])>
Public Class ModuleComparisonMapper
    Inherits ComparisonMapper

    Public Sub New(activeContainer As KblMapper, compareContainer As KblMapper, currentActiveObjects As ICollection(Of String), compareActiveObjects As ICollection(Of String), settings As GeneralSettingsBase)
        MyBase.New(activeContainer, compareContainer, currentActiveObjects, compareActiveObjects, settings)
    End Sub

    Public Overrides Sub CompareObjects()
        For Each [module] As [Module] In _currentContainer.GetModules
            With [module]
                Dim compareModule As [Module] = Nothing
                If _compareContainer.TryGetModuleByPartNr(.Part_number, compareModule) Then
                    If (compareModule IsNot Nothing) AndAlso (Not .Equals(compareModule)) Then
                        Dim changedProperty As PartChangedProperty = CompareProperties(Of PartChangedProperty)([module], compareModule)
                        If (changedProperty.ChangedProperties.Count <> 0) AndAlso (Not Me.Changes.ContainsModified(.SystemId)) Then
                            Me.AddOrReplaceChangeWithInverse(.SystemId, changedProperty, CompareChangeType.Modified)
                        End If
                    End If
                Else
                    Me.AddOrReplaceChangeWithInverse(.SystemId, [module], CompareChangeType.Deleted)
                End If
            End With
        Next

        For Each [module] As [Module] In _compareContainer.GetModules
            If Not _currentContainer.TryGetModuleByPartNr([module].Part_number, Nothing) Then
                Me.AddOrReplaceChangeWithInverse([module].SystemId, [module], CompareChangeType.New)
            End If
        Next
    End Sub

End Class
