Imports System.Reflection
Imports Zuken.E3.HarnessAnalyzer.Settings

Public Class ConnectionChangedProperty
    Inherits ChangedProperty

    Public Sub New(owner As ComparisonMapper, currentKBLMapper As KblMapper, compareKBLMapper As KblMapper, settings As GeneralSettingsBase)
        MyBase.New(owner, currentKBLMapper, compareKBLMapper, settings)
    End Sub

    Public Overrides Function CompareObjectProperty(objProperty As PropertyInfo, currentObject As Object, compareObject As Object, excludeProperties As List(Of String)) As Boolean
        Dim currentProperty As Object = objProperty.GetValue(currentObject, Nothing)
        Dim compareProperty As Object = objProperty.GetValue(compareObject, Nothing)
        If (currentProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(E3.Lib.Schema.Kbl.Connection.Installation_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Installation_instruction)), DirectCast(compareProperty, IEnumerable(Of Installation_instruction)))
                    Dim installationInfoComparisonMapper As New InstallationInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    installationInfoComparisonMapper.CompareObjects()

                    If installationInfoComparisonMapper.HasChanges Then
                        MyBase.ChangedProperties.Add(objProperty.Name.ToString, installationInfoComparisonMapper)
                    End If
                Case NameOf(E3.Lib.Schema.Kbl.Connection.Processing_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Processing_instruction)), DirectCast(compareProperty, IEnumerable(Of Processing_instruction)))
                    Dim processingInfoComparisonMapper As New ProcessingInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    processingInfoComparisonMapper.CompareObjects()

                    If processingInfoComparisonMapper.HasChanges Then
                        MyBase.ChangedProperties.Add(objProperty.Name.ToString, processingInfoComparisonMapper)
                    End If
                Case NameOf(E3.Lib.Schema.Kbl.Connection.External_references)
                    Dim currentExternalReferences As New List(Of External_reference)
                    For Each externalReference As String In currentProperty.ToString.SplitSpace
                        currentExternalReferences.Add(_currentKBLMapper.GetOccurrenceObject(Of External_reference)(externalReference))
                    Next

                    Dim compareExternalReferences As New List(Of External_reference)
                    If compareProperty IsNot Nothing Then
                        For Each externalReference As String In compareProperty.ToString.SplitSpace
                            compareExternalReferences.Add(_compareKBLMapper.GetOccurrenceObject(Of External_reference)(externalReference))
                        Next
                    End If

                    Dim listConvertToDictionary As New ListConvertToDictionary(currentExternalReferences, compareExternalReferences)
                    Dim externalReferenceComparisonMapper As New CommonComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    externalReferenceComparisonMapper.CompareObjects()

                    If externalReferenceComparisonMapper.HasChanges Then
                        MyBase.ChangedProperties.Add(objProperty.Name.ToString, externalReferenceComparisonMapper)
                    End If
                Case NameOf(E3.Lib.Schema.Kbl.Connection.Description), NameOf(E3.Lib.Schema.Kbl.Connection.Nominal_voltage), NameOf(E3.Lib.Schema.Kbl.Connection.Signal_type), NameOf(E3.Lib.Schema.Kbl.Connection.Wire)
                    If (compareProperty IsNot Nothing AndAlso currentProperty.ToString <> compareProperty.ToString) OrElse (compareProperty Is Nothing AndAlso currentProperty.ToString <> String.Empty) Then
                        If (objProperty.Name = NameOf(E3.Lib.Schema.Kbl.Connection.Id)) Then
                            MyBase.ChangedProperties.Add(NameOf(E3.Lib.Schema.Kbl.Connection.Id), compareProperty)
                        Else
                            MyBase.ChangedProperties.Add(objProperty.Name, compareProperty)
                        End If
                    End If
            End Select
        ElseIf (compareProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(E3.Lib.Schema.Kbl.Connection.Installation_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Installation_instruction), DirectCast(compareProperty, IEnumerable(Of Installation_instruction)))
                    Dim installationInfoComparisonMapper As New InstallationInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    installationInfoComparisonMapper.CompareObjects()

                    If (installationInfoComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(objProperty.Name.ToString, installationInfoComparisonMapper)
                    End If
                Case NameOf(E3.Lib.Schema.Kbl.Connection.Processing_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Processing_instruction), DirectCast(compareProperty, IEnumerable(Of Processing_instruction)))
                    Dim processingInfoComparisonMapper As New ProcessingInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    processingInfoComparisonMapper.CompareObjects()

                    If (processingInfoComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(objProperty.Name.ToString, processingInfoComparisonMapper)
                    End If
                Case NameOf(E3.Lib.Schema.Kbl.Connection.External_references)
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
                Case Else
                    MyBase.ChangedProperties.Add(objProperty.Name.ToString, compareProperty)
            End Select
        End If
        Return False
    End Function

End Class
