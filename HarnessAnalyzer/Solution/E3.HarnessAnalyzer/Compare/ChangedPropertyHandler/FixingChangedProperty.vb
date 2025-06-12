Imports System.Reflection
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Public Class FixingChangedProperty
    Inherits ChangedProperty

    Public Sub New(owner As ComparisonMapper, currentKBLMapper As KblMapper, compareKBLMapper As KblMapper, settings As GeneralSettingsBase)
        MyBase.New(owner, currentKBLMapper, compareKBLMapper, settings)
    End Sub

    Public Overrides Function CompareObjectProperty(objProperty As PropertyInfo, currentObject As Object, compareObject As Object, excludeProperties As List(Of String)) As Boolean
        Dim currentProperty As Object = objProperty.GetValue(currentObject, Nothing)
        Dim compareProperty As Object = objProperty.GetValue(compareObject, Nothing)
        If (currentProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(E3.Lib.Schema.Kbl.Fixing_occurrence.Alias_id)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Alias_identification)), DirectCast(compareProperty, IEnumerable(Of Alias_identification)))
                    Dim aliasIdComparisonMapper As New AliasIdComparisonMapper(Me._owner, listConvertToDictionary, Me.Settings)
                    aliasIdComparisonMapper.CompareObjects()

                    If (aliasIdComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(NameOf(E3.Lib.Schema.Kbl.Fixing_occurrence.Alias_id), aliasIdComparisonMapper)
                    End If

                Case NameOf(E3.Lib.Schema.Kbl.Fixing_occurrence.Part)
                    Dim currentFixing As Fixing = _currentKBLMapper.GetPart(Of Fixing)(currentProperty.ToString)
                    Dim compareFixing As Fixing = _compareKBLMapper.GetPart(Of Fixing)(compareProperty.ToString)
                    Dim partChangedProperty As PartChangedProperty = New PartChangedProperty(Me._owner, _currentKBLMapper, _compareKBLMapper, Me.Settings)

                    partChangedProperty.CompareObjectProperties(currentFixing, compareFixing, excludeProperties)
                    If (partChangedProperty.ChangedProperties.Count <> 0) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, partChangedProperty)
                    End If

                Case NameOf(E3.Lib.Schema.Kbl.Fixing_occurrence.Installation_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Installation_instruction)), DirectCast(compareProperty, IEnumerable(Of Installation_instruction)))
                    Dim installationInfoComparisonMapper As New InstallationInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    installationInfoComparisonMapper.CompareObjects()

                    If (installationInfoComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, installationInfoComparisonMapper)
                    End If

                Case NameOf(E3.Lib.Schema.Kbl.Fixing_occurrence.Id), NameOf(E3.Lib.Schema.Kbl.Fixing_occurrence.Description)
                    If (compareProperty IsNot Nothing AndAlso currentProperty.ToString <> compareProperty.ToString) OrElse (compareProperty Is Nothing AndAlso currentProperty.ToString <> String.Empty) Then
                        If (objProperty.Name = NameOf(E3.Lib.Schema.Kbl.Fixing_occurrence.Id)) Then
                            MyBase.ChangedProperties.Add(NameOf(E3.Lib.Schema.Kbl.Fixing_occurrence.Id), compareProperty)
                        Else
                            MyBase.ChangedProperties.Add(objProperty.Name, compareProperty)
                        End If
                    End If

                Case NameOf(E3.Lib.Schema.Kbl.Fixing_occurrence.Localized_description)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Localized_string)), DirectCast(compareProperty, IEnumerable(Of Localized_string)))
                    Dim locDescriptionComparer As New LocalizedDescriptionComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    locDescriptionComparer.CompareObjects()

                    If (locDescriptionComparer.HasChanges) Then
                        MyBase.ChangedProperties.Add(FixingPropertyName.Localized_description.ToString, locDescriptionComparer)
                    End If
                Case NameOf(E3.Lib.Schema.Kbl.Fixing_occurrence.Placement)

                    Dim placementChangedProperty As PlacementChangedProperty = New PlacementChangedProperty(Me._owner, _currentKBLMapper, _compareKBLMapper, Me.Settings)

                    placementChangedProperty.CompareObjectProperties(currentProperty, compareProperty, excludeProperties)
                    If (placementChangedProperty.ChangedProperties.Count <> 0) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, placementChangedProperty)
                    End If

            End Select
        ElseIf (compareProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(E3.Lib.Schema.Kbl.Fixing_occurrence.Alias_id)
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Alias_identification), DirectCast(compareProperty, IEnumerable(Of Alias_identification)))
                    Dim aliasIdComparisonMapper As New AliasIdComparisonMapper(Me._owner, listConvertToDictionary, Me.Settings)
                    aliasIdComparisonMapper.CompareObjects()

                    If (aliasIdComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(NameOf(E3.Lib.Schema.Kbl.Fixing_occurrence.Alias_id), aliasIdComparisonMapper)
                    End If
                Case NameOf(E3.Lib.Schema.Kbl.Fixing_occurrence.Installation_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Installation_instruction), DirectCast(compareProperty, IEnumerable(Of Installation_instruction)))
                    Dim installationInfoComparisonMapper As New InstallationInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    installationInfoComparisonMapper.CompareObjects()

                    If (installationInfoComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, installationInfoComparisonMapper)
                    End If

                Case NameOf(E3.Lib.Schema.Kbl.Fixing_occurrence.Localized_description)
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Localized_string), DirectCast(compareProperty, IEnumerable(Of Localized_string)))
                    Dim locDescriptionComparer As New LocalizedDescriptionComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    locDescriptionComparer.CompareObjects()

                    If (locDescriptionComparer.HasChanges) Then
                        MyBase.ChangedProperties.Add(NameOf(E3.Lib.Schema.Kbl.Fixing_occurrence.Localized_description), locDescriptionComparer)
                    End If

                Case Else
                    MyBase.ChangedProperties.Add(objProperty.Name, compareProperty)
            End Select
        End If
        Return False
    End Function

End Class
