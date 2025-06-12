Imports System.Reflection
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Public Class CommonChangedProperty
    Inherits ChangedProperty

    Public Sub New(owner As ComparisonMapper, currentKBLMapper As KblMapper, compareKBLMapper As KblMapper, settings As GeneralSettingsBase)
        MyBase.New(owner, currentKBLMapper, compareKBLMapper, settings)
    End Sub

    Public Overrides Function CompareObjectProperty(objProperty As PropertyInfo, currentObject As Object, compareObject As Object, excludeProperties As List(Of String)) As Boolean
        Dim currentProperty As Object = objProperty.GetValue(currentObject, Nothing)
        Dim compareProperty As Object = objProperty.GetValue(compareObject, Nothing)

        If objProperty.Name = NameOf([Lib].Schema.Kbl.Part.Processing_information) Then ' HINT: here the part class was taken because it's the most common class, but there is no general base class for all kbl objects available, so take care of this when changing the properties
            Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Processing_instruction)), DirectCast(compareProperty, IEnumerable(Of Processing_instruction)))
            Dim processingInfoComparisonMapper As New ProcessingInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
            processingInfoComparisonMapper.CompareObjects()

            If (processingInfoComparisonMapper.HasChanges) Then
                MyBase.ChangedProperties.Add(NameOf(Part.Processing_information), processingInfoComparisonMapper)
            End If
        ElseIf objProperty.Name = NameOf([Lib].Schema.Kbl.Part.Localized_description) Then ' HINT: here the part class was taken because it's the most common class, but there is no general base class for all kbl objects available, so take care of this when changing the properties
            Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Localized_string)), DirectCast(compareProperty, IEnumerable(Of Localized_string)))
            Dim locDescriptionComparer As New LocalizedDescriptionComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
            locDescriptionComparer.CompareObjects()

            If (locDescriptionComparer.HasChanges) Then
                MyBase.ChangedProperties.Add(NameOf([Lib].Schema.Kbl.Part.Localized_description), locDescriptionComparer)
            End If

        ElseIf (objProperty.Name = PartPropertyName.Part_Localized_description.ToString) Then
            Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Localized_string)), DirectCast(compareProperty, IEnumerable(Of Localized_string)))
            Dim locDescriptionComparer As New LocalizedDescriptionComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
            locDescriptionComparer.CompareObjects()

            If (locDescriptionComparer.HasChanges) Then ' TODO: the part_localized_description - string is a special hack, don't mix it up with the localized description-string from the part-class, check this hack if there is a better more undestandable way around this
                MyBase.ChangedProperties.Add(PartPropertyName.Part_Localized_description.ToString, locDescriptionComparer)
            End If

        Else
            If currentProperty IsNot Nothing Then
                If currentProperty IsNot compareProperty AndAlso
                            currentProperty.Equals(compareProperty) = False AndAlso
                            ReferenceEquals(compareProperty, currentProperty) = False Then
                    MyBase.ChangedProperties.Add(objProperty.Name.ToString, compareProperty)
                End If
            Else
                If compareProperty IsNot Nothing Then
                    MyBase.ChangedProperties.Add(objProperty.Name.ToString, compareProperty)
                End If
            End If
        End If
        Return False
    End Function


End Class
