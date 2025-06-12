Imports System.Reflection
Imports System.Text
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Public Class ConnectorChangedProperty
    Inherits ChangedProperty

    Private _currentActiveObjects As ICollection(Of String)
    Private _compareActiveObjects As ICollection(Of String)

    Public Sub New(owner As ComparisonMapper, currentKBLMapper As KblMapper, compareKBLMapper As KblMapper, currentActiveObjects As IEnumerable(Of String), compareActiveObjects As IEnumerable(Of String), settings As GeneralSettingsBase)
        MyBase.New(owner, currentKBLMapper, compareKBLMapper, settings)

        _currentActiveObjects = currentActiveObjects.ToList
        _compareActiveObjects = compareActiveObjects.ToList
    End Sub

    Public Overrides Function CompareObjectProperty(objProperty As PropertyInfo, currentObject As Object, compareObject As Object, excludeProperties As List(Of String)) As Boolean
        Dim currentProperty As Object = objProperty.GetValue(currentObject, Nothing)
        Dim compareProperty As Object = objProperty.GetValue(compareObject, Nothing)
        If (currentProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(Connector_occurrence.Alias_id)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Alias_identification)), DirectCast(compareProperty, IEnumerable(Of Alias_identification)))
                    Dim aliasIdComparisonMapper As New AliasIdComparisonMapper(Me._owner, listConvertToDictionary, Me.Settings)
                    aliasIdComparisonMapper.CompareObjects()

                    If aliasIdComparisonMapper.HasChanges Then
                        MyBase.ChangedProperties.Add(NameOf(Connector_occurrence.Alias_id), aliasIdComparisonMapper)
                    End If

                Case NameOf(Connector_occurrence.Part)
                    Dim currentConnectorHousing As Connector_housing = _currentKBLMapper.GetPart(Of Connector_housing)(currentProperty.ToString)
                    Dim compareConnectorHousing As Connector_housing = _compareKBLMapper.GetPart(Of Connector_housing)(compareProperty.ToString)
                    Dim partChangedProperty As PartChangedProperty = New PartChangedProperty(Me._owner, _currentKBLMapper, _compareKBLMapper, Me.Settings)

                    partChangedProperty.CompareObjectProperties(currentConnectorHousing, compareConnectorHousing, excludeProperties)
                    If partChangedProperty.ChangedProperties.Count <> 0 Then
                        MyBase.ChangedProperties.Add(objProperty.Name, partChangedProperty)
                    End If

                Case NameOf(Connector_occurrence.Installation_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Installation_instruction)), DirectCast(compareProperty, IEnumerable(Of Installation_instruction)))
                    Dim installationInfoComparisonMapper As New InstallationInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, _currentActiveObjects, _compareActiveObjects, listConvertToDictionary, Me.Settings)
                    installationInfoComparisonMapper.CompareObjects()

                    If installationInfoComparisonMapper.HasChanges Then
                        MyBase.ChangedProperties.Add(objProperty.Name, installationInfoComparisonMapper)
                    End If

                Case NameOf(Connector_occurrence.Slots)
                    Dim currentConnector As Connector_occurrence = DirectCast(currentObject, Connector_occurrence)
                    Dim compareConnector As Connector_occurrence = DirectCast(compareObject, Connector_occurrence)

                    Dim currentCavities As New Dictionary(Of String, Cavity_occurrence)
                    Dim compareCavities As New Dictionary(Of String, Cavity_occurrence)

                    If (currentConnector.Slots IsNot Nothing) AndAlso (currentConnector.Slots.Length <> 0) Then
                        Dim currentCavityOccurrences As List(Of Cavity_occurrence) = currentConnector.Slots(0).Cavities.ToList

                        For Each cavity As Cavity_occurrence In currentCavityOccurrences
                            If (Not currentCavities.ContainsKey(String.Format("Cav_{0}|{1}", currentConnector.Id, _currentKBLMapper.GetPart(Of Cavity)(cavity.Part).Cavity_number))) Then
                                currentCavities.Add(String.Format("Cav_{0}|{1}", currentConnector.Id, _currentKBLMapper.GetPart(Of Cavity)(cavity.Part).Cavity_number), cavity)
                            End If
                        Next
                    End If

                    If (compareConnector.Slots IsNot Nothing) AndAlso (compareConnector.Slots.Length <> 0) Then
                        Dim compareCavityOccurrences As List(Of Cavity_occurrence) = compareConnector.Slots(0).Cavities.ToList

                        For Each cavity As Cavity_occurrence In compareCavityOccurrences
                            If (Not compareCavities.ContainsKey(String.Format("Cav_{0}|{1}", compareConnector.Id, _compareKBLMapper.GetPart(Of Cavity)(cavity.Part).Cavity_number))) Then
                                compareCavities.Add(String.Format("Cav_{0}|{1}", compareConnector.Id, _compareKBLMapper.GetPart(Of Cavity)(cavity.Part).Cavity_number), cavity)
                            End If
                        Next
                    End If

                    Dim listConvertToDictionary As New ListConvertToDictionary(currentCavities, compareCavities)
                    Dim cavityComparisonMapper As New CavityComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, _currentActiveObjects, _compareActiveObjects, listConvertToDictionary, Me.Settings)
                    cavityComparisonMapper.CompareObjects()

                    If cavityComparisonMapper.HasChanges Then
                        MyBase.ChangedProperties.Add(NameOf(Connector_occurrence.Slots), cavityComparisonMapper)
                    End If

                Case NameOf(Connector_occurrence.Reference_element)

                    Dim currentRefElements As New List(Of String)
                    For Each referencedElements As String In currentProperty.ToString.SplitSpace.ToList
                        currentRefElements.Add(Harness.GetReferenceElement(referencedElements, _currentKBLMapper))
                    Next
                    currentRefElements.Sort()

                    Dim compareRefComponents As New List(Of String)

                    If (compareProperty IsNot Nothing) Then
                        For Each referencedComponent As String In compareProperty.ToString.SplitSpace.ToList
                            compareRefComponents.Add(Harness.GetReferenceElement(referencedComponent, _compareKBLMapper))
                        Next
                        compareRefComponents.Sort()
                    End If

                    Dim currentReferencedElementsString As String = currentRefElements.AsText
                    Dim compareReferencedElementsString As String = compareRefComponents.AsText

                    If (currentReferencedElementsString <> compareReferencedElementsString) Then
                        MyBase.ChangedProperties.Add(NameOf(Connector_occurrence.Reference_element), compareReferencedElementsString)
                    End If

                Case NameOf(Connector_occurrence.Description), NameOf(Connector_occurrence.Usage)
                    If (compareProperty IsNot Nothing AndAlso currentProperty.ToString <> compareProperty.ToString) OrElse (compareProperty Is Nothing AndAlso currentProperty.ToString <> String.Empty) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, compareProperty)
                    End If

                Case NameOf(Connector_occurrence.Localized_description)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Localized_string)), DirectCast(compareProperty, IEnumerable(Of Localized_string)))
                    Dim locDescriptionComparer As New LocalizedDescriptionComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    locDescriptionComparer.CompareObjects()

                    If (locDescriptionComparer.HasChanges) Then
                        MyBase.ChangedProperties.Add(ConnectorPropertyName.Localized_description.ToString, locDescriptionComparer)
                    End If

                Case NameOf(Connector_occurrence.Contact_points)
                    Dim currentConnector As Connector_occurrence = DirectCast(currentObject, Connector_occurrence)
                    Dim compareConnector As Connector_occurrence = DirectCast(compareObject, Connector_occurrence)

                    Dim currentContactPoints As New Dictionary(Of String, Contact_point)
                    Dim compareContactPoints As New Dictionary(Of String, Contact_point)

                    If (currentConnector.Contact_points IsNot Nothing) AndAlso (currentConnector.Contact_points.Length <> 0) Then
                        For Each contactPoint As Contact_point In currentConnector.Contact_points
                            Dim contactPointId As String = contactPoint.GetUniqueIdForCompare(currentConnector.Id, _currentKBLMapper)

                            If (Not currentContactPoints.ContainsKey(contactPointId)) Then
                                currentContactPoints.Add(contactPointId, contactPoint)
                            End If
                        Next
                    End If

                    If (compareConnector.Contact_points IsNot Nothing) AndAlso (compareConnector.Contact_points.Length <> 0) Then
                        For Each contactPoint As Contact_point In compareConnector.Contact_points
                            Dim contactPointId As String = contactPoint.GetUniqueIdForCompare(compareConnector.Id, _compareKBLMapper)

                            If (Not compareContactPoints.ContainsKey(contactPointId)) Then
                                compareContactPoints.Add(contactPointId, contactPoint)
                            End If
                        Next
                    End If

                    Dim listConvertToDictionary As New ListConvertToDictionary(currentContactPoints, compareContactPoints)
                    Dim contactPointComparisonMapper As New ContactPointComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, _currentActiveObjects, _compareActiveObjects, listConvertToDictionary, Me.Settings)
                    contactPointComparisonMapper.CompareObjects()

                    If (contactPointComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(NameOf(Connector_occurrence.Contact_points), contactPointComparisonMapper)
                    End If
                Case NameOf(Connector_occurrence.Placement)
                    Dim placementChangedProperty As PlacementChangedProperty = New PlacementChangedProperty(Me._owner, _currentKBLMapper, _compareKBLMapper, Me.Settings)

                    placementChangedProperty.CompareObjectProperties(currentProperty, compareProperty, excludeProperties)
                    If (placementChangedProperty.ChangedProperties.Count <> 0) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, placementChangedProperty)
                    End If
            End Select
        ElseIf (compareProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(Connector_occurrence.Alias_id)
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Alias_identification), DirectCast(compareProperty, IEnumerable(Of Alias_identification)))
                    Dim aliasIdComparisonMapper As New AliasIdComparisonMapper(Me._owner, listConvertToDictionary, Me.Settings)
                    aliasIdComparisonMapper.CompareObjects()

                    If (aliasIdComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(NameOf(Connector_occurrence.Alias_id), aliasIdComparisonMapper)
                    End If

                Case NameOf(Connector_occurrence.Installation_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Installation_instruction), DirectCast(compareProperty, IEnumerable(Of Installation_instruction)))
                    Dim installationInfoComparisonMapper As New InstallationInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, _currentActiveObjects, _compareActiveObjects, listConvertToDictionary, Me.Settings)
                    installationInfoComparisonMapper.CompareObjects()

                    If (installationInfoComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, installationInfoComparisonMapper)
                    End If

                Case NameOf(Connector_occurrence.Localized_description)
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Localized_string), DirectCast(compareProperty, IEnumerable(Of Localized_string)))
                    Dim locDescriptionComparer As New LocalizedDescriptionComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    locDescriptionComparer.CompareObjects()

                    If (locDescriptionComparer.HasChanges) Then
                        MyBase.ChangedProperties.Add(NameOf(Connector_occurrence.Localized_description), locDescriptionComparer)
                    End If

                Case NameOf(Connector_occurrence.Reference_element)
                    Dim refInfos As String = String.Empty

                    For Each refId As String In compareProperty.ToString.SplitSpace
                        If (refInfos = String.Empty) Then
                            refInfos = Harness.GetReferenceElement(refId, _compareKBLMapper)
                        Else
                            refInfos = String.Format("{0}{1}{2}", refInfos, vbCrLf, Harness.GetReferenceElement(refId, _compareKBLMapper))
                        End If
                    Next

                    If (refInfos <> String.Empty) Then
                        MyBase.ChangedProperties.Add(NameOf(Connector_occurrence.Reference_element), String.Format("{0}{1}", vbCrLf, refInfos))
                    End If

                Case Else
                    MyBase.ChangedProperties.Add(objProperty.Name, compareProperty)
            End Select
        End If
        Return False
    End Function
End Class
