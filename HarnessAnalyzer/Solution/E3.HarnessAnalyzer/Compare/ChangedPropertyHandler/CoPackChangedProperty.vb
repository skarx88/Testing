Imports System.Reflection
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Public Class CoPackChangedProperty
    Inherits ChangedProperty

    Public Sub New(owner As ComparisonMapper, currentKBLMapper As KblMapper, compareKBLMapper As KblMapper, settings As GeneralSettingsBase)
        MyBase.New(owner, currentKBLMapper, compareKBLMapper, settings)
    End Sub

    Public Overrides Function CompareObjectProperty(objProperty As PropertyInfo, currentObject As Object, compareObject As Object, excludeProperties As List(Of String)) As Boolean
        Dim currentProperty As Object = objProperty.GetValue(currentObject, Nothing)
        Dim compareProperty As Object = objProperty.GetValue(compareObject, Nothing)
        If (currentProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(E3.Lib.Schema.Kbl.Co_pack_occurrence.Alias_id)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Alias_identification)), DirectCast(compareProperty, IEnumerable(Of Alias_identification)))
                    Dim aliasIdComparisonMapper As New AliasIdComparisonMapper(Me._owner, listConvertToDictionary, Me.Settings)
                    aliasIdComparisonMapper.CompareObjects()

                    If (aliasIdComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(NameOf(E3.Lib.Schema.Kbl.Accessory_occurrence.Alias_id), aliasIdComparisonMapper)
                    End If
                Case NameOf(E3.Lib.Schema.Kbl.Co_pack_occurrence.Part)
                    Dim currentCoPackPart As Co_pack_part = _currentKBLMapper.GetPart(Of Co_pack_part)(currentProperty.ToString)
                    Dim compareCoPackPart As Co_pack_part = _compareKBLMapper.GetPart(Of Co_pack_part)(compareProperty.ToString)
                    Dim partChangedProperty As PartChangedProperty = New PartChangedProperty(Me._owner, _currentKBLMapper, _compareKBLMapper, Me.Settings)
                    partChangedProperty.CompareObjectProperties(currentCoPackPart, compareCoPackPart, excludeProperties)

                    If (partChangedProperty.ChangedProperties.Count <> 0) Then
                        MyBase.ChangedProperties.Add(objProperty.Name.ToString, partChangedProperty)
                    End If
                Case NameOf(E3.Lib.Schema.Kbl.Co_pack_occurrence.Installation_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Installation_instruction)), DirectCast(compareProperty, IEnumerable(Of Installation_instruction)))
                    Dim installationInfoComparisonMapper As New InstallationInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    installationInfoComparisonMapper.CompareObjects()

                    If (installationInfoComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(objProperty.Name.ToString, installationInfoComparisonMapper)
                    End If
                Case NameOf(E3.Lib.Schema.Kbl.Co_pack_occurrence.Id), NameOf(E3.Lib.Schema.Kbl.Co_pack_occurrence.Description)
                    If (compareProperty IsNot Nothing AndAlso currentProperty.ToString <> compareProperty.ToString) OrElse (compareProperty Is Nothing AndAlso currentProperty.ToString <> String.Empty) Then
                        If objProperty.Name = NameOf(E3.Lib.Schema.Kbl.Accessory_occurrence.Id) Then
                            MyBase.ChangedProperties.Add(NameOf(E3.Lib.Schema.Kbl.Accessory_occurrence.Id), compareProperty)
                        Else
                            MyBase.ChangedProperties.Add(objProperty.Name.ToString, compareProperty)
                        End If
                    End If
            End Select
        ElseIf (compareProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(E3.Lib.Schema.Kbl.Co_pack_occurrence.Alias_id)
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Alias_identification), DirectCast(compareProperty, IEnumerable(Of Alias_identification)))
                    Dim aliasIdComparisonMapper As New AliasIdComparisonMapper(Me._owner, listConvertToDictionary, Me.Settings)
                    aliasIdComparisonMapper.CompareObjects()

                    If (aliasIdComparisonMapper.HasChanges) Then MyBase.ChangedProperties.Add(AccessoryPropertyName.Alias_id.ToString, aliasIdComparisonMapper)
                Case NameOf(E3.Lib.Schema.Kbl.Co_pack_occurrence.Installation_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Installation_instruction), DirectCast(compareProperty, IEnumerable(Of Installation_instruction)))
                    Dim installationInfoComparisonMapper As New InstallationInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    installationInfoComparisonMapper.CompareObjects()

                    If (installationInfoComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(objProperty.Name.ToString, installationInfoComparisonMapper)
                    End If
                Case Else
                    MyBase.ChangedProperties.Add(objProperty.Name.ToString, compareProperty)
            End Select
        End If
        Return False
    End Function
End Class