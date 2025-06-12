Imports System.Reflection
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Public Class GeneralWireChangedProperty
    Inherits ChangedProperty

    Private _isCable As Boolean

    Public Sub New(owner As ComparisonMapper, currentKBLMapper As KblMapper, compareKBLMapper As KblMapper, isCable As Boolean, settings As GeneralSettingsBase)
        MyBase.New(owner, currentKBLMapper, compareKBLMapper, settings)

        _isCable = isCable
    End Sub

    Public Overrides Function CompareObjectProperty(objProperty As PropertyInfo, currentObject As Object, compareObject As Object, excludeProperties As List(Of String)) As Boolean
        Dim currentProperty As Object = objProperty.GetValue(currentObject, Nothing)
        Dim compareProperty As Object = objProperty.GetValue(compareObject, Nothing)
        If objProperty.Name <> NameOf(General_wire.Core) Then
            Dim compareNumValue As Numerical_value = TryCast(compareProperty, Numerical_value)
            If (currentProperty IsNot Nothing) Then
                Select Case objProperty.Name
                    Case NameOf(E3.Lib.Schema.Kbl.General_wire.Mass_information), NameOf(E3.Lib.Schema.Kbl.General_wire.Bend_radius), NameOf(E3.Lib.Schema.Kbl.General_wire.Cross_section_area), NameOf(E3.Lib.Schema.Kbl.General_wire.Outside_diameter)
                        If (compareProperty IsNot Nothing) Then
                            If (Math.Round(DirectCast(currentProperty, Numerical_value).Value_component, 2) <> Math.Round(compareNumValue.Value_component, 2)) Then
                                MyBase.ChangedProperties.Add(objProperty.Name, String.Format("{0} {1}", Math.Round(compareNumValue.Value_component, 2), _compareKBLMapper.GetUnit(compareNumValue.Unit_component).Unit_name))
                            End If
                        Else
                            MyBase.ChangedProperties.Add(objProperty.Name, String.Empty)
                        End If
                    Case NameOf(E3.Lib.Schema.Kbl.General_wire.Material_information)
                        Dim currentMaterial As Material = TryCast(currentProperty, Material)
                        Dim compareMaterial As Material = TryCast(compareProperty, Material)

                        If (compareMaterial IsNot Nothing) Then
                            If (currentMaterial.Material_key <> compareMaterial.Material_key) Then
                                MyBase.ChangedProperties.Add(objProperty.Name, compareMaterial)
                            End If
                        Else
                            MyBase.ChangedProperties.Add(objProperty.Name, String.Empty)
                        End If
                    Case NameOf(E3.Lib.Schema.Kbl.General_wire.Processing_information)
                        Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Processing_instruction)), DirectCast(compareProperty, IEnumerable(Of Processing_instruction)))
                        Dim processingInfoComparisonMapper As New ProcessingInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                        processingInfoComparisonMapper.CompareObjects()

                        If (processingInfoComparisonMapper.HasChanges) Then
                            MyBase.ChangedProperties.Add(PartPropertyName.Part_processing_information.ToString, processingInfoComparisonMapper)  ' HINT: special hack, see commonChangedProperty comments
                        End If
                    Case NameOf(E3.Lib.Schema.Kbl.General_wire.Alias_id)
                        Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Alias_identification)), DirectCast(compareProperty, IEnumerable(Of Alias_identification)))
                        Dim aliasIdComparisonMapper As New AliasIdComparisonMapper(Me._owner, listConvertToDictionary, Me.Settings)
                        aliasIdComparisonMapper.CompareObjects()

                        If (aliasIdComparisonMapper.HasChanges) Then
                            MyBase.ChangedProperties.Add(PartPropertyName.Part_alias_ids.ToString, aliasIdComparisonMapper)  ' HINT: special hack, see commonChangedProperty comments
                        End If
                    Case NameOf(E3.Lib.Schema.Kbl.General_wire.External_references)
                        Dim currentExternalReferences As New List(Of External_reference)
                        For Each externalReference As String In currentProperty.ToString.SplitSpace
                            currentExternalReferences.Add(_currentKBLMapper.GetOccurrenceObject(Of External_reference)(externalReference))
                        Next

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
                            MyBase.ChangedProperties.Add(objProperty.Name.ToString, externalReferenceComparisonMapper)
                        End If
                    Case NameOf(E3.Lib.Schema.Kbl.General_wire.Change)
                        Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Change)), DirectCast(compareProperty, IEnumerable(Of Change)))
                        Dim changeComparisonMapper As New CommonComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                        changeComparisonMapper.CompareObjects()

                        If (changeComparisonMapper.HasChanges) Then
                            MyBase.ChangedProperties.Add(NameOf(E3.Lib.Schema.Kbl.General_wire.Change), changeComparisonMapper)
                        End If
                    Case NameOf(E3.Lib.Schema.Kbl.General_wire.Localized_description)
                        Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Localized_string)), DirectCast(compareProperty, IEnumerable(Of Localized_string)))
                        Dim locDescriptionComparer As New LocalizedDescriptionComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                        locDescriptionComparer.CompareObjects()

                        If (locDescriptionComparer.HasChanges) Then
                            MyBase.ChangedProperties.Add(PartPropertyName.Part_Localized_description.ToString, locDescriptionComparer)  ' HINT: special hack, see commonChangedProperty comments
                        End If
                    Case NameOf(E3.Lib.Schema.Kbl.General_wire.Cover_colour)
                        Dim currentCoverColours As String = DirectCast(currentObject, General_wire).GetColours
                        Dim compareCoverColours As String = DirectCast(compareObject, General_wire).GetColours

                        If (currentCoverColours <> compareCoverColours) Then
                            MyBase.ChangedProperties.Add(If(_isCable, NameOf(E3.Lib.Schema.Kbl.General_wire.Cover_colour), NameOf(E3.Lib.Schema.Kbl.Core.Core_colour)), compareCoverColours)
                        End If
                    Case Else
                        If (TypeOf currentProperty Is String) OrElse (IsNumeric(currentProperty)) Then
                            If (compareProperty IsNot Nothing AndAlso currentProperty.ToString <> compareProperty.ToString) OrElse (compareProperty Is Nothing AndAlso currentProperty.ToString <> String.Empty) Then
                                If (objProperty.Name = WirePropertyName.Description.ToString) Then
                                    MyBase.ChangedProperties.Add(PartPropertyName.Part_description.ToString, compareProperty) ' HINT: special hack, see commonChangedProperty comments
                                Else
                                    MyBase.ChangedProperties.Add(objProperty.Name, compareProperty)
                                End If
                            End If
                        End If
                End Select
            ElseIf (compareProperty IsNot Nothing) Then
                Select Case objProperty.Name
                    Case NameOf(E3.Lib.Schema.Kbl.General_wire.Alias_id)
                        Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Alias_identification), DirectCast(compareProperty, IEnumerable(Of Alias_identification)))
                        Dim aliasIdComparisonMapper As New AliasIdComparisonMapper(Me._owner, listConvertToDictionary, Me.Settings)
                        aliasIdComparisonMapper.CompareObjects()

                        If (aliasIdComparisonMapper.HasChanges) Then
                            MyBase.ChangedProperties.Add(PartPropertyName.Part_alias_ids.ToString, aliasIdComparisonMapper) ' HINT: special hack, see commonChangedProperty comments
                        End If
                    Case NameOf(E3.Lib.Schema.Kbl.General_wire.Change)
                        Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Change), DirectCast(compareProperty, IEnumerable(Of Change)))
                        Dim changeComparisonMapper As New CommonComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                        changeComparisonMapper.CompareObjects()

                        If (changeComparisonMapper.HasChanges) Then
                            MyBase.ChangedProperties.Add(NameOf(E3.Lib.Schema.Kbl.General_wire.Change), changeComparisonMapper)
                        End If
                    Case NameOf(E3.Lib.Schema.Kbl.General_wire.Description)
                        MyBase.ChangedProperties.Add(PartPropertyName.Part_description.ToString, compareProperty) ' HINT: special hack, see commonChangedProperty comments
                    Case NameOf(E3.Lib.Schema.Kbl.General_wire.External_references)
                        Dim externalReferences As New List(Of External_reference)
                        For Each externalReference As String In compareProperty.ToString.SplitSpace
                            externalReferences.Add(_compareKBLMapper.GetOccurrenceObject(Of External_reference)(externalReference))
                        Next

                        Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of External_reference), externalReferences)
                        Dim externalReferenceComparisonMapper As New CommonComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                        externalReferenceComparisonMapper.CompareObjects()

                        If (externalReferenceComparisonMapper.HasChanges) Then
                            MyBase.ChangedProperties.Add(objProperty.Name.ToString, externalReferenceComparisonMapper)
                        End If
                    Case NameOf(E3.Lib.Schema.Kbl.General_wire.Mass_information)
                        MyBase.ChangedProperties.Add(objProperty.Name.ToString, String.Format("{0} {1}", Math.Round(compareNumValue.Value_component, 3), _compareKBLMapper.GetUnit(compareNumValue.Unit_component).Unit_name))
                    Case WirePropertyName.Processing_information.ToString
                        Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Processing_instruction), DirectCast(compareProperty, IEnumerable(Of Processing_instruction)))
                        Dim processingInfoComparisonMapper As New ProcessingInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                        processingInfoComparisonMapper.CompareObjects()

                        If (processingInfoComparisonMapper.HasChanges) Then
                            MyBase.ChangedProperties.Add(PartPropertyName.Part_processing_information.ToString, processingInfoComparisonMapper) ' HINT: special hack, see commonChangedProperty comments
                        End If
                    Case PartPropertyName.Part_Localized_description.ToString ' HINT: special hack, see commonChangedProperty comments
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
        End If

        Return False
    End Function
End Class
