Imports System.Reflection
Imports Zuken.E3.HarnessAnalyzer.Settings


Public Class WireProtectionOccurrenceChangedProperty
    Inherits ChangedProperty

    Public Sub New(owner As ComparisonMapper, currentKBLMapper As KblMapper, compareKBLMapper As KblMapper, settings As GeneralSettingsBase)
        MyBase.New(owner, currentKBLMapper, compareKBLMapper, settings)
    End Sub

    Public Overrides Function CompareObjectProperty(objProperty As PropertyInfo, currentObject As Object, compareObject As Object, excludeProperties As List(Of String)) As Boolean
        Dim currentProperty As Object = objProperty.GetValue(currentObject, Nothing)
        Dim compareProperty As Object = objProperty.GetValue(compareObject, Nothing)
        Dim compareNumValue As Numerical_value = TryCast(compareProperty, Numerical_value)
        Dim currentNumValue As Numerical_value = TryCast(currentProperty, Numerical_value)
        If (currentProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(Zuken.E3.Lib.Schema.Kbl.Wire_protection_occurrence.Alias_id)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Alias_identification)), DirectCast(compareProperty, IEnumerable(Of Alias_identification)))
                    Dim aliasIdComparisonMapper As New AliasIdComparisonMapper(Me._owner, listConvertToDictionary, Me.Settings)
                    aliasIdComparisonMapper.CompareObjects()

                    If aliasIdComparisonMapper.HasChanges Then
                        MyBase.ChangedProperties.Add(NameOf(Zuken.E3.Lib.Schema.Kbl.Wire_protection_occurrence.Alias_id), aliasIdComparisonMapper)
                    End If
                Case NameOf(Zuken.E3.Lib.Schema.Kbl.Wire_protection_occurrence.Installation_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Installation_instruction)), DirectCast(compareProperty, IEnumerable(Of Installation_instruction)))
                    Dim installationInfoComparisonMapper As New InstallationInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    installationInfoComparisonMapper.CompareObjects()

                    If installationInfoComparisonMapper.HasChanges Then
                        MyBase.ChangedProperties.Add(NameOf(Zuken.E3.Lib.Schema.Kbl.Wire_protection_occurrence.Installation_information), installationInfoComparisonMapper)
                    End If
                Case NameOf(Zuken.E3.Lib.Schema.Kbl.Wire_protection_occurrence.Protection_length)
                    If (compareProperty IsNot Nothing) Then
                        If Math.Round(currentNumValue.Value_component, 2) <> Math.Round(compareNumValue.Value_component, 2) Then
                            MyBase.ChangedProperties.Add(NameOf(Zuken.E3.Lib.Schema.Kbl.Wire_protection_occurrence.Protection_length), String.Format("{0} {1}", Math.Round(compareNumValue.Value_component, 2), _compareKBLMapper.GetUnit(compareNumValue.Unit_component).Unit_name))
                        End If
                    Else
                        MyBase.ChangedProperties.Add(NameOf(Zuken.E3.Lib.Schema.Kbl.Wire_protection_occurrence.Protection_length), String.Empty)
                    End If

                Case NameOf(Zuken.E3.Lib.Schema.Kbl.Wire_protection_occurrence.Part)
                    Dim currentWireProtection As Wire_protection = _currentKBLMapper.GetPart(Of Wire_protection)(currentProperty.ToString)
                    Dim compareWireProtection As Wire_protection = _compareKBLMapper.GetPart(Of Wire_protection)(compareProperty.ToString)
                    Dim partChangedProperty As New PartChangedProperty(Me._owner, _currentKBLMapper, _compareKBLMapper, Me.Settings)

                    partChangedProperty.CompareObjectProperties(currentWireProtection, compareWireProtection, excludeProperties)
                    If partChangedProperty.ChangedProperties.Count <> 0 Then
                        MyBase.ChangedProperties.Add(objProperty.Name, partChangedProperty)
                    End If

                Case Else
                    If TypeOf currentProperty Is String OrElse IsNumeric(currentProperty) Then
                        If (compareProperty IsNot Nothing AndAlso currentProperty.ToString <> compareProperty.ToString) OrElse (compareProperty Is Nothing AndAlso currentProperty.ToString <> String.Empty) Then
                            If (objProperty.Name = NameOf(Zuken.E3.Lib.Schema.Kbl.Wire_protection_occurrence.Id)) Then
                                MyBase.ChangedProperties.Add(NameOf(Zuken.E3.Lib.Schema.Kbl.Wire_protection_occurrence.Id), compareProperty)
                            Else
                                MyBase.ChangedProperties.Add(objProperty.Name, compareProperty)
                            End If
                        End If
                    End If
            End Select
        ElseIf (compareProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(Zuken.E3.Lib.Schema.Kbl.Wire_protection_occurrence.Alias_id)
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Alias_identification), DirectCast(compareProperty, IEnumerable(Of Alias_identification)))
                    Dim aliasIdComparisonMapper As New AliasIdComparisonMapper(Me._owner, listConvertToDictionary, Me.Settings)
                    aliasIdComparisonMapper.CompareObjects()

                    If (aliasIdComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(NameOf(Zuken.E3.Lib.Schema.Kbl.Wire_protection_occurrence.Alias_id), aliasIdComparisonMapper)
                    End If
                Case NameOf(Zuken.E3.Lib.Schema.Kbl.Wire_protection_occurrence.Installation_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Installation_instruction), DirectCast(compareProperty, IEnumerable(Of Installation_instruction)))
                    Dim installationInfoComparisonMapper As New InstallationInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    installationInfoComparisonMapper.CompareObjects()

                    If (installationInfoComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(NameOf(Zuken.E3.Lib.Schema.Kbl.Wire_protection_occurrence.Installation_information), installationInfoComparisonMapper)
                    End If
                Case Else
                    MyBase.ChangedProperties.Add(objProperty.Name, compareProperty)
            End Select
        End If
        Return False
    End Function

End Class
