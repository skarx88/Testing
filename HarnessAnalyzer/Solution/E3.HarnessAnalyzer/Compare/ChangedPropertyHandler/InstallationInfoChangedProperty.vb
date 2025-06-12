Imports System.Reflection
Imports Zuken.E3.HarnessAnalyzer.Settings


Public Class InstallationInfoChangedProperty
    Inherits ChangedProperty

    Public Sub New(owner As ComparisonMapper, currentKBLMapper As KblMapper, compareKBLMapper As KblMapper, settings As GeneralSettingsBase)
        MyBase.New(owner, currentKBLMapper, compareKBLMapper, settings)
    End Sub

    Public Overrides Function CompareObjectProperty(objProperty As PropertyInfo, currentObject As Object, compareObject As Object, excludeProperties As List(Of String)) As Boolean
        Dim currentProperty As Object = objProperty.GetValue(currentObject, Nothing)
        Dim compareProperty As Object = objProperty.GetValue(compareObject, Nothing)
        If (currentProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(Processing_instruction.External_reference)
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

                    If (externalReferenceComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(objProperty.Name.ToString, externalReferenceComparisonMapper)
                    End If
                Case Else
                    If (compareProperty IsNot Nothing AndAlso currentProperty.ToString <> compareProperty.ToString) OrElse (compareProperty Is Nothing AndAlso currentProperty.ToString <> String.Empty) Then
                        MyBase.ChangedProperties.Add(objProperty.Name.ToString, compareProperty)
                    End If
            End Select
        ElseIf (compareProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(Processing_instruction.External_reference)
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
