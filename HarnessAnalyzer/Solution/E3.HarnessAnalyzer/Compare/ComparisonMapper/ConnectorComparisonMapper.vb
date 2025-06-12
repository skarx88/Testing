
Imports Zuken.E3.HarnessAnalyzer.Settings

<KblObjectType(KblObjectType.Connector_occurrence)>
Public Class ConnectorComparisonMapper
    Inherits ComparisonMapper

    Public Sub New(activeContainer As KblMapper, compareContainer As KblMapper, currentActiveObjects As ICollection(Of String), compareActiveObjects As ICollection(Of String), settings As GeneralSettingsBase)
        MyBase.New(activeContainer, compareContainer, currentActiveObjects, compareActiveObjects, settings)
    End Sub

    Public Overrides Sub CompareObjects()
        For Each connectorOcc As Connector_occurrence In _currentContainer.GetHarness.Connector_occurrence
            If (_currentActiveObjects.Contains(connectorOcc.Id)) Then
                With connectorOcc
                    If (_compareActiveObjects.Contains(.Id)) AndAlso (_compareContainer.GetHarness.Connector_occurrence.Any(Function(con) con.Id = .Id)) Then
                        Dim connectorPart As Connector_housing = _currentKBLMapper.GetPart(Of Connector_housing)(connectorOcc.Part)

                        For Each compareConnector As Connector_occurrence In _compareContainer.GetHarness.Connector_occurrence.Where(Function(con) con.Id = .Id)
                            Dim comparePart As Connector_housing = _compareKBLMapper.GetPart(Of Connector_housing)(compareConnector.Part)
                            If (connectorPart.IsKSL AndAlso comparePart.IsKSL AndAlso connectorPart.Part_number = comparePart.Part_number) OrElse (Not connectorPart.IsKSL AndAlso Not comparePart.IsKSL) Then
                                Dim changedProperty As ConnectorChangedProperty = CompareProperties(Of ConnectorChangedProperty)(connectorOcc, compareConnector)
                                If (changedProperty.ChangedProperties.Count <> 0) AndAlso (Not Me.Changes.ContainsModified(.SystemId)) Then
                                    Me.AddOrReplaceChangeWithInverse(.SystemId, .SystemId, compareConnector.SystemId, changedProperty, CompareChangeType.Modified)
                                End If
                            End If
                        Next
                    Else
                        Me.AddOrReplaceChangeWithInverse(.SystemId, .SystemId, "", connectorOcc, CompareChangeType.Deleted)
                    End If
                End With
            End If
        Next

        For Each connectorOcc As Connector_occurrence In _compareContainer.GetHarness.Connector_occurrence
            If (_compareActiveObjects.Contains(connectorOcc.Id) AndAlso Not _currentActiveObjects.Contains(connectorOcc.Id)) OrElse (_compareActiveObjects.Contains(connectorOcc.Id) AndAlso Not _currentContainer.GetHarness.Connector_occurrence.Where(Function(con) con.Id = connectorOcc.Id).Any()) Then
                Me.AddOrReplaceChangeWithInverse(connectorOcc.SystemId, "", connectorOcc.SystemId, connectorOcc, CompareChangeType.New)
            End If
        Next
    End Sub

End Class
