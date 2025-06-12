Imports System.Reflection
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Public Class AccessoryChangedProperty
    Inherits ChangedProperty

    Public Sub New(owner As ComparisonMapper, currentKBLMapper As KblMapper, compareKBLMapper As KblMapper, settings As GeneralSettingsBase)
        MyBase.New(owner, currentKBLMapper, compareKBLMapper, settings)
    End Sub

    Public Overrides Function CompareObjectProperty(objProperty As PropertyInfo, currentObject As Object, compareObject As Object, excludeProperties As List(Of String)) As Boolean
        Dim currentProperty As Object = objProperty.GetValue(currentObject, Nothing)
        Dim compareProperty As Object = objProperty.GetValue(compareObject, Nothing)
        If (currentProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(Accessory_occurrence.Alias_id)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Alias_identification)), DirectCast(compareProperty, IEnumerable(Of Alias_identification)))
                    Dim aliasIdComparisonMapper As New AliasIdComparisonMapper(Me._owner, listConvertToDictionary, Me.Settings)
                    aliasIdComparisonMapper.CompareObjects()

                    If aliasIdComparisonMapper.HasChanges Then
                        MyBase.ChangedProperties.Add(AccessoryPropertyName.Alias_id.ToString, aliasIdComparisonMapper)
                    End If

                Case NameOf(Accessory_occurrence.Part)
                    Dim currentAccessory As Accessory = _currentKBLMapper.GetPart(Of Accessory)(currentProperty.ToString)
                    Dim compareAccessory As Accessory = _compareKBLMapper.GetPart(Of Accessory)(compareProperty.ToString)
                    Dim partChangedProperty As PartChangedProperty = New PartChangedProperty(Me._owner, _currentKBLMapper, _compareKBLMapper, Me.Settings)

                    partChangedProperty.CompareObjectProperties(currentAccessory, compareAccessory, excludeProperties)
                    If partChangedProperty.ChangedProperties.Count <> 0 Then
                        MyBase.ChangedProperties.Add(objProperty.Name.ToString, partChangedProperty)
                    End If

                Case NameOf(Accessory_occurrence.Installation_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Installation_instruction)), DirectCast(compareProperty, IEnumerable(Of Installation_instruction)))
                    Dim installationInfoComparisonMapper As New InstallationInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    installationInfoComparisonMapper.CompareObjects()

                    If installationInfoComparisonMapper.HasChanges Then
                        MyBase.ChangedProperties.Add(objProperty.Name.ToString, installationInfoComparisonMapper)
                    End If

                Case NameOf(Accessory_occurrence.Reference_element)

                    Dim currentRefElements As New List(Of String)
                    For Each referencedElements As String In currentProperty.ToString.SplitSpace.ToList
                        currentRefElements.Add([Lib].Schema.Kbl.Harness.GetReferenceElement(referencedElements, _currentKBLMapper))
                    Next
                    currentRefElements.Sort()

                    Dim compareRefComponents As New List(Of String)
                    If (compareProperty IsNot Nothing) Then
                        For Each referencedComponent As String In compareProperty.ToString.SplitSpace.ToList
                            compareRefComponents.Add(Harness.GetReferenceElement(referencedComponent, _compareKBLMapper))
                        Next
                        compareRefComponents.Sort()
                    End If

                    Dim currentReferencedElementsString As String = String.Empty
                    Dim compareReferencedElementsString As String = String.Empty

                    For Each id As String In currentRefElements
                        currentReferencedElementsString = String.Format("{0}{1}{2}", currentReferencedElementsString, vbCrLf, id)
                    Next

                    currentReferencedElementsString = currentReferencedElementsString.Trim(vbCrLf.ToCharArray)

                    For Each id As String In compareRefComponents
                        compareReferencedElementsString = String.Format("{0}{1}{2}", compareReferencedElementsString, vbCrLf, id)
                    Next

                    compareReferencedElementsString = compareReferencedElementsString.Trim(vbCrLf.ToCharArray)

                    If (currentReferencedElementsString <> compareReferencedElementsString) Then
                        MyBase.ChangedProperties.Add(AccessoryPropertyName.Reference_element.ToString, compareReferencedElementsString)
                    End If

                Case NameOf(Accessory_occurrence.Localized_description)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Localized_string)), DirectCast(compareProperty, IEnumerable(Of Localized_string)))
                    Dim locDescriptionComparer As New LocalizedDescriptionComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    locDescriptionComparer.CompareObjects()

                    If (locDescriptionComparer.HasChanges) Then
                        MyBase.ChangedProperties.Add(AccessoryPropertyName.Localized_Description.ToString, locDescriptionComparer)
                    End If

                Case NameOf(Accessory_occurrence.Id), NameOf(Accessory_occurrence.Description)
                    If (compareProperty IsNot Nothing AndAlso currentProperty.ToString <> compareProperty.ToString) OrElse (compareProperty Is Nothing AndAlso currentProperty.ToString <> String.Empty) Then
                        MyBase.ChangedProperties.Add(objProperty.Name.ToString, compareProperty)
                    End If

                Case NameOf(Accessory_occurrence.Placement)
                    Dim placementChangedProperty As PlacementChangedProperty = New PlacementChangedProperty(Me._owner, _currentKBLMapper, _compareKBLMapper, Me.Settings)

                    placementChangedProperty.CompareObjectProperties(currentProperty, compareProperty, excludeProperties)
                    If (placementChangedProperty.ChangedProperties.Count <> 0) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, placementChangedProperty)
                    End If
            End Select
        ElseIf (compareProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(Accessory_occurrence.Alias_id)
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Alias_identification), DirectCast(compareProperty, IEnumerable(Of Alias_identification)))
                    Dim aliasIdComparisonMapper As New AliasIdComparisonMapper(Me._owner, listConvertToDictionary, Me.Settings)
                    aliasIdComparisonMapper.CompareObjects()

                    If (aliasIdComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(AccessoryPropertyName.Alias_id.ToString, aliasIdComparisonMapper)
                    End If
                Case NameOf(Accessory_occurrence.Installation_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Installation_instruction), DirectCast(compareProperty, IEnumerable(Of Installation_instruction)))
                    Dim installationInfoComparisonMapper As New InstallationInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    installationInfoComparisonMapper.CompareObjects()

                    If (installationInfoComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(objProperty.Name.ToString, installationInfoComparisonMapper)
                    End If
                Case NameOf(Accessory_occurrence.Reference_element)
                    Dim refInfos As String = String.Empty

                    For Each refId As String In compareProperty.ToString.SplitSpace
                        If (refInfos = String.Empty) Then
                            refInfos = Harness.GetReferenceElement(refId, _compareKBLMapper)
                        Else
                            refInfos = String.Format("{0}{1}{2}", refInfos, vbCrLf, Harness.GetReferenceElement(refId, _compareKBLMapper))
                        End If
                    Next

                    If (refInfos <> String.Empty) Then
                        MyBase.ChangedProperties.Add(AccessoryPropertyName.Reference_element.ToString, String.Format("{0}{1}", vbCrLf, refInfos))
                    End If

                Case NameOf(Accessory_occurrence.Localized_description)
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Localized_string), DirectCast(compareProperty, IEnumerable(Of Localized_string)))
                    Dim locDescriptionComparer As New LocalizedDescriptionComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    locDescriptionComparer.CompareObjects()

                    If (locDescriptionComparer.HasChanges) Then
                        MyBase.ChangedProperties.Add(AccessoryPropertyName.Localized_Description.ToString, locDescriptionComparer)
                    End If
                Case Else
                    MyBase.ChangedProperties.Add(objProperty.Name.ToString, compareProperty)
            End Select
        End If
        Return False
    End Function
End Class
