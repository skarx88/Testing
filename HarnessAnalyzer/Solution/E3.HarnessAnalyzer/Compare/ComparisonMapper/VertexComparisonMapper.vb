Imports Zuken.E3.HarnessAnalyzer.Settings

<KblObjectType(KblObjectType.Node)>
Public Class VertexComparisonMapper
    Inherits ComparisonMapper

    Public Sub New(activeContainer As KblMapper, compareContainer As KblMapper, currentActiveObjects As ICollection(Of String), compareActiveObjects As ICollection(Of String), settings As GeneralSettingsBase)
        MyBase.New(activeContainer, compareContainer, currentActiveObjects, compareActiveObjects, settings)
    End Sub

    Public Overrides Sub CompareObjects()
        For Each node As Node In _currentContainer.GetNodes
            If (_currentActiveObjects.Contains(node.Id)) Then
                With node
                    If (_compareActiveObjects.Contains(.Id)) AndAlso (_compareContainer.GetNodes.Any(Function(nod) nod.Id = .Id)) Then
                        Dim compareNode As Node = TryCast(_compareContainer.GetNodes.Where(Function(nod) nod.Id = .Id)(0), Node)
                        If (compareNode IsNot Nothing) AndAlso (Not .Equals(compareNode)) Then
                            Dim changedProperty As VertexChangedProperty = CompareProperties(Of VertexChangedProperty)(node, compareNode)
                            If (changedProperty.ChangedProperties.Count <> 0) AndAlso (Not Me.Changes.ContainsModified(.SystemId)) Then
                                Me.AddOrReplaceChangeWithInverse(.SystemId, .SystemId, compareNode.SystemId, changedProperty, CompareChangeType.Modified)
                            End If
                        End If
                    Else
                        Me.AddOrReplaceChangeWithInverse(.SystemId, .SystemId, "", node, CompareChangeType.Deleted)
                    End If
                End With
            End If
        Next

        For Each node As Node In _compareContainer.GetNodes
            If (_compareActiveObjects.Contains(node.Id) AndAlso Not _currentActiveObjects.Contains(node.Id)) OrElse (_compareActiveObjects.Contains(node.Id) AndAlso Not _currentContainer.GetNodes.Where(Function(nod) nod.Id = node.Id).Any()) Then
                Me.AddOrReplaceChangeWithInverse(node.SystemId, "", node.SystemId, node, CompareChangeType.New)
            End If
        Next
    End Sub

End Class
