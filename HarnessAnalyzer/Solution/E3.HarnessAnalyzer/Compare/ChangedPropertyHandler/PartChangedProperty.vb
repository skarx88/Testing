Imports System.Reflection
Imports System.Xml.Serialization
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Public Class PartChangedProperty
    Inherits ChangedProperty

    Public Sub New(owner As ComparisonMapper, currentKBLMapper As KblMapper, compareKBLMapper As KblMapper, settings As GeneralSettingsBase)
        MyBase.New(owner, currentKBLMapper, compareKBLMapper, settings)
    End Sub

    Protected Overrides Function GetOverriddenPropertyName(propName As String) As String
        'HACK this is needed to remap the names for part properties to the names of the excluded properties
        If propName = NameOf(Part.Alias_id) Then
            Return PartPropertyName.Part_alias_ids.ToString
        ElseIf propName = NameOf(Part.Localized_description) Then
            Return PartPropertyName.Part_Localized_description.ToString
        ElseIf propName = NameOf(Part.Description) Then
            Return PartPropertyName.Part_description.ToString
        Else
            Return propName
        End If
    End Function

    Public Overrides Function CompareObjectProperty(objProperty As PropertyInfo, currentObject As Object, compareObject As Object, excludeProperties As List(Of String)) As Boolean
        Dim currentProperty As Object = objProperty.GetValue(currentObject, Nothing)
        Dim compareProperty As Object = objProperty.GetValue(compareObject, Nothing)
        Dim compareNumericalValue As Numerical_value = TryCast(compareProperty, Numerical_value)
        Dim currentNumericalValue As Numerical_value = TryCast(currentProperty, Numerical_value)
        If (currentProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(Part.Mass_information), NameOf(E3.Lib.Schema.Kbl.Fuse.Nominal_current)
                    If (compareProperty IsNot Nothing) Then
                        If (Math.Round(currentNumericalValue.Value_component, 3) <> Math.Round(DirectCast(compareProperty, Numerical_value).Value_component, 3)) Then
                            MyBase.ChangedProperties.Add(objProperty.Name, String.Format("{0} {1}", Math.Round(compareNumericalValue.Value_component, 3), If(_compareKBLMapper.KBLUnitMapper.ContainsKey(compareNumericalValue.Unit_component), _compareKBLMapper.GetUnit(compareNumericalValue.Unit_component).Unit_name, String.Empty)))
                        End If
                    Else
                        MyBase.ChangedProperties.Add(objProperty.Name, String.Empty)
                    End If
                Case NameOf(Part.Material_information)
                    Dim currentMaterial As Material = DirectCast(currentProperty, Material)
                    Dim compareMaterial As Material = DirectCast(compareProperty, Material)

                    If (compareMaterial IsNot Nothing) Then
                        If (currentMaterial.Material_key <> compareMaterial.Material_key) Then
                            MyBase.ChangedProperties.Add(objProperty.Name, compareMaterial)
                        End If
                    Else
                        MyBase.ChangedProperties.Add(objProperty.Name, String.Empty)
                    End If
                Case NameOf(E3.Lib.Schema.Kbl.Module.Module_configuration)
                    CompareModuleConfiguration(currentProperty, compareProperty, excludeProperties)
                Case NameOf(Part.Processing_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Processing_instruction)), DirectCast(compareProperty, IEnumerable(Of Processing_instruction)))
                    Dim processingInfoComparisonMapper As New ProcessingInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    processingInfoComparisonMapper.CompareObjects()

                    If (processingInfoComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(NameOf(Part.Processing_information), processingInfoComparisonMapper)
                    End If
                Case PartPropertyName.Part_alias_ids.ToString
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Alias_identification)), DirectCast(compareProperty, IEnumerable(Of Alias_identification)))
                    Dim aliasIdComparisonMapper As New AliasIdComparisonMapper(Me._owner, listConvertToDictionary, Me.Settings)
                    aliasIdComparisonMapper.CompareObjects()

                    If (aliasIdComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(PartPropertyName.Part_alias_ids.ToString, aliasIdComparisonMapper)
                    End If
                Case NameOf(Part.External_references)
                    Dim currentExternalReferences As New List(Of External_reference)
                    If (currentProperty IsNot Nothing) Then
                        For Each externalReference As String In currentProperty.ToString.SplitSpace
                            currentExternalReferences.Add(_currentKBLMapper.GetOccurrenceObject(Of External_reference)(externalReference))
                        Next
                    End If

                    Dim compareExternalReferences As New List(Of External_reference)
                    If (compareProperty IsNot Nothing) Then
                        For Each externalReference As String In compareProperty.ToString.SplitSpace
                            compareExternalReferences.Add(_compareKBLMapper.GetOccurrenceObject(Of External_reference)(externalReference))
                        Next
                    End If

                    Dim listConvertToDictionary As New ListConvertToDictionary(currentExternalReferences, compareExternalReferences)
                    Dim externalReferenceComparisonMapper As New CommonComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    externalReferenceComparisonMapper.CompareObjects()

                    If (externalReferenceComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, externalReferenceComparisonMapper)
                    End If
                Case NameOf(Part.Change)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Change)), DirectCast(compareProperty, IEnumerable(Of Change)))
                    Dim changeComparisonMapper As New CommonComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    changeComparisonMapper.CompareObjects()

                    If (changeComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(NameOf(Part.Change), changeComparisonMapper)
                    End If
                Case "Wire_size", NameOf(Core.Cross_section_area), NameOf(Core.Outside_diameter)
                    Dim currentValueRange As Value_range = DirectCast(currentProperty, Value_range)
                    Dim compareValueRange As Value_range = DirectCast(compareProperty, Value_range)
                    Dim oneObjectComparisonMapper As New SingleObjectComparisonMapper(Me._owner, currentValueRange, compareValueRange, _currentKBLMapper, _compareKBLMapper, Me.Settings)
                    oneObjectComparisonMapper.CompareObjects()

                    If (oneObjectComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, oneObjectComparisonMapper)
                    End If
                Case PartPropertyName.Part_Localized_description.ToString
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Localized_string)), DirectCast(compareProperty, IEnumerable(Of Localized_string)))
                    Dim locDescriptionComparer As New LocalizedDescriptionComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    locDescriptionComparer.CompareObjects()

                    If (locDescriptionComparer.HasChanges) Then
                        MyBase.ChangedProperties.Add(PartPropertyName.Part_Localized_description.ToString, locDescriptionComparer)
                    End If

                Case NameOf(Connector_occurrence.Slots)
                    Return True

                Case NameOf(E3.Lib.Schema.Kbl.Module.Of_family)
                    Dim currentModuleFamily As Module_family = _currentKBLMapper.GetOccurrenceObject(Of Module_family)(currentProperty.ToString)

                    If (compareProperty IsNot Nothing) Then
                        Dim compareModuleFamily As Module_family = _compareKBLMapper.GetOccurrenceObject(Of Module_family)(compareProperty.ToString)

                        If (currentModuleFamily IsNot Nothing) AndAlso (compareModuleFamily IsNot Nothing) AndAlso (currentModuleFamily.Id <> compareModuleFamily.Id) Then
                            MyBase.ChangedProperties.Add(NameOf(E3.Lib.Schema.Kbl.Module.Of_family), compareModuleFamily.Id)
                        End If
                    Else
                        MyBase.ChangedProperties.Add(NameOf(E3.Lib.Schema.Kbl.Module.Of_family), String.Empty)
                    End If
                Case NameOf(E3.Lib.Schema.Kbl.Fuse.Type)
                    Dim currentType As String = DirectCast(currentProperty, Fuse_type).Key
                    Dim compareType As String = DirectCast(compareProperty, Fuse_type).Key

                    If (currentType <> compareType) Then
                        MyBase.ChangedProperties.Add(NameOf(E3.Lib.Schema.Kbl.Fuse.Type), compareType)
                    End If
                Case Else
                    If (TypeOf currentProperty Is String) OrElse (IsNumeric(currentProperty)) Then
                        If (compareProperty IsNot Nothing AndAlso currentProperty.ToString <> compareProperty.ToString) OrElse (compareProperty Is Nothing AndAlso currentProperty.ToString <> String.Empty) Then
                            If objProperty.Name = PartPropertyName.Part_description.ToString Then
                                MyBase.ChangedProperties.Add(PartPropertyName.Part_description.ToString, compareProperty)
                            Else
                                MyBase.ChangedProperties.Add(objProperty.Name, compareProperty)
                            End If
                        End If
                    End If
            End Select
        ElseIf (compareProperty IsNot Nothing) Then
            Dim comparNumValue As Numerical_value = TryCast(compareProperty, Numerical_value)
            Select Case objProperty.Name
                Case NameOf(E3.Lib.Schema.Kbl.Component_occurrence.Alias_id)
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Alias_identification), DirectCast(compareProperty, IEnumerable(Of Alias_identification)))
                    Dim aliasIdComparisonMapper As New AliasIdComparisonMapper(Me._owner, listConvertToDictionary, Me.Settings)
                    aliasIdComparisonMapper.CompareObjects()

                    If (aliasIdComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(PartPropertyName.Part_alias_ids.ToString, aliasIdComparisonMapper)
                    End If
                Case PartPropertyName.Part_alias_ids.ToString
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Change), DirectCast(compareProperty, IEnumerable(Of Change)))
                    Dim changeComparisonMapper As New CommonComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    changeComparisonMapper.CompareObjects()

                    If (changeComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(PartPropertyName.Change.ToString, changeComparisonMapper)
                    End If
                Case NameOf(E3.Lib.Schema.Kbl.Component_occurrence.Description)
                    MyBase.ChangedProperties.Add(PartPropertyName.Part_description.ToString, compareProperty)
                Case PartPropertyName.External_references.ToString
                    Dim externalReferences As New List(Of External_reference)
                    For Each externalReference As String In compareProperty.ToString.SplitSpace
                        Dim ext_ref_occ As External_reference = _compareKBLMapper.GetOccurrenceObject(Of External_reference)(externalReference)
                        If ext_ref_occ IsNot Nothing Then
                            externalReferences.Add(ext_ref_occ)
                        End If
                    Next

                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of External_reference), externalReferences)
                    Dim externalReferenceComparisonMapper As New CommonComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    externalReferenceComparisonMapper.CompareObjects()

                    If (externalReferenceComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(NameOf(E3.Lib.Schema.Kbl.Part.External_references), externalReferenceComparisonMapper)
                    End If
                Case NameOf(E3.Lib.Schema.Kbl.Part.Mass_information)
                    MyBase.ChangedProperties.Add(objProperty.Name.ToString, String.Format("{0} {1}", Math.Round(comparNumValue.Value_component, 3), If(_compareKBLMapper.KBLUnitMapper.ContainsKey(comparNumValue.Unit_component), _compareKBLMapper.GetUnit(comparNumValue.Unit_component).Unit_name, String.Empty)))
                Case NameOf(E3.Lib.Schema.Kbl.Part.Processing_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Processing_instruction), DirectCast(compareProperty, IEnumerable(Of Processing_instruction)))
                    Dim processingInfoComparisonMapper As New ProcessingInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    processingInfoComparisonMapper.CompareObjects()

                    If (processingInfoComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(NameOf(Part.Processing_information), processingInfoComparisonMapper)
                    End If

                Case PartPropertyName.Part_Localized_description.ToString
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Localized_string), DirectCast(compareProperty, IEnumerable(Of Localized_string)))
                    Dim locDescriptionComparer As New LocalizedDescriptionComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    locDescriptionComparer.CompareObjects()

                    If (locDescriptionComparer.HasChanges) Then
                        MyBase.ChangedProperties.Add(PartPropertyName.Part_Localized_description.ToString, locDescriptionComparer)
                    End If
                Case Else
                    MyBase.ChangedProperties.Add(objProperty.Name, compareProperty)
            End Select
        End If
        Return False
    End Function

    Private Sub CompareModuleConfiguration(ByVal currentProperty As Object, ByVal compareProperty As Object, excludeProperties As List(Of String))
        Dim currentModuleConfiguration As Module_configuration = DirectCast(currentProperty, Module_configuration)
        Dim compareModuleConfiguration As Module_configuration = DirectCast(compareProperty, Module_configuration)
        Dim compare_configType As String = If(currentModuleConfiguration.Configuration_typeSpecified, currentModuleConfiguration.Configuration_type.ToStringOrXmlName, String.Empty)
        Dim current_configType As String = If(compareModuleConfiguration.Configuration_typeSpecified, compareModuleConfiguration.Configuration_type.ToStringOrXmlName, String.Empty)

        If (currentModuleConfiguration.Logistic_control_information <> compareModuleConfiguration.Logistic_control_information) Then
            MyBase.ChangedProperties.Add(ModulePropertyName.Logistic_control_information, compareModuleConfiguration.Logistic_control_information)
        End If

        If (current_configType <> compare_configType) Then
            MyBase.ChangedProperties.Add(ModulePropertyName.Configuration_type, compareModuleConfiguration.Configuration_type)
        End If

        Dim currentControlledComponents As New List(Of String)
        Dim compareControlledComponents As New List(Of String)

        If (currentModuleConfiguration.Controlled_components IsNot Nothing) Then
            currentControlledComponents = Harness.GetInformationOfComponentsControlledByModule(currentModuleConfiguration.Controlled_components.SplitSpace.ToList, _currentKBLMapper)
        End If

        If (compareModuleConfiguration.Controlled_components IsNot Nothing) Then
            compareControlledComponents = Harness.GetInformationOfComponentsControlledByModule(compareModuleConfiguration.Controlled_components.SplitSpace.ToList, _compareKBLMapper)
        End If

        If (currentControlledComponents.Count >= compareControlledComponents.Count) Then
            For Each currentControlledComponent As String In currentControlledComponents
                If (Not compareControlledComponents.Contains(currentControlledComponent)) Then
                    MyBase.ChangedProperties.Add(ModulePropertyName.Controlled_components, compareControlledComponents)
                    Exit For
                End If
            Next
        Else
            For Each compareControlledComponent As String In compareControlledComponents
                If (Not currentControlledComponents.Contains(compareControlledComponent)) Then
                    MyBase.ChangedProperties.Add(ModulePropertyName.Controlled_components, compareControlledComponents)
                    Exit For
                End If
            Next

        End If
    End Sub

End Class
