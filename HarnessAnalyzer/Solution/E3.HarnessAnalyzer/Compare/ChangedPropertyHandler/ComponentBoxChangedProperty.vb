Imports System.Reflection
Imports Zuken.E3.HarnessAnalyzer.Settings

Public Class ComponentBoxChangedProperty
    Inherits ChangedProperty

    Public Sub New(owner As ComparisonMapper, currentKBLMapper As KblMapper, compareKBLMapper As KblMapper, settings As GeneralSettingsBase)
        MyBase.New(owner, currentKBLMapper, compareKBLMapper, settings)
    End Sub

    Public Overrides Function CompareObjectProperty(objProperty As PropertyInfo, currentObject As Object, compareObject As Object, excludeProperties As List(Of String)) As Boolean
        Dim currentProperty As Object = objProperty.GetValue(currentObject, Nothing)
        Dim compareProperty As Object = objProperty.GetValue(compareObject, Nothing)
        If (currentProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(Component_box_occurrence.Alias_id)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Alias_identification)), DirectCast(compareProperty, IEnumerable(Of Alias_identification)))
                    Dim aliasIdComparisonMapper As New AliasIdComparisonMapper(Me._owner, listConvertToDictionary, Me.Settings)
                    aliasIdComparisonMapper.CompareObjects()

                    If (aliasIdComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(NameOf(Component_box_occurrence.Alias_id), aliasIdComparisonMapper)
                    End If
                Case NameOf(Component_box_occurrence.Part)
                    Dim currentComponentBox As Component_box = _currentKBLMapper.GetPart(Of Component_box)(currentProperty.ToString)
                    Dim compareComponentBox As Component_box = _compareKBLMapper.GetPart(Of Component_box)(compareProperty.ToString)
                    Dim partChangedProperty As PartChangedProperty = New PartChangedProperty(Me._owner, _currentKBLMapper, _compareKBLMapper, Me.Settings)
                    partChangedProperty.CompareObjectProperties(currentComponentBox, compareComponentBox, excludeProperties)

                    If (partChangedProperty.ChangedProperties.Count <> 0) Then
                        MyBase.ChangedProperties.Add(objProperty.Name.ToString, partChangedProperty)
                    End If
                Case NameOf(Component_box_occurrence.Installation_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Installation_instruction)), DirectCast(compareProperty, IEnumerable(Of Installation_instruction)))
                    Dim installationInfoComparisonMapper As New InstallationInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    installationInfoComparisonMapper.CompareObjects()

                    If (installationInfoComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(objProperty.Name.ToString, installationInfoComparisonMapper)
                    End If
                Case NameOf(Component_box_occurrence.Reference_element)
                    Dim currentReferenceElement As String = Harness.GetReferenceElement(currentProperty.ToString, _currentKBLMapper)
                    Dim compareReferenceElement As String = If(compareProperty Is Nothing, String.Empty, Harness.GetReferenceElement(compareProperty.ToString, _compareKBLMapper))

                    If (currentReferenceElement <> compareReferenceElement) Then
                        MyBase.ChangedProperties.Add(objProperty.Name.ToCharArray, compareReferenceElement)
                    End If
                Case NameOf(Component_box_occurrence.Id), NameOf(Component_box_occurrence.Description)
                    If (compareProperty IsNot Nothing AndAlso currentProperty.ToString <> compareProperty.ToString) OrElse (compareProperty Is Nothing AndAlso currentProperty.ToString <> String.Empty) Then
                        MyBase.ChangedProperties.Add(objProperty.Name.ToString, compareProperty)
                    End If
            End Select
        ElseIf (compareProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(Component_box_occurrence.Alias_id)
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Alias_identification), DirectCast(compareProperty, IEnumerable(Of Alias_identification)))
                    Dim aliasIdComparisonMapper As New AliasIdComparisonMapper(Me._owner, listConvertToDictionary, Me.Settings)
                    aliasIdComparisonMapper.CompareObjects()

                    If (aliasIdComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(NameOf(Component_box_occurrence.Alias_id), aliasIdComparisonMapper)
                    End If
                Case NameOf(Component_box_occurrence.Installation_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Installation_instruction), DirectCast(compareProperty, IEnumerable(Of Installation_instruction)))
                    Dim installationInfoComparisonMapper As New InstallationInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    installationInfoComparisonMapper.CompareObjects()

                    If (installationInfoComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(objProperty.Name.ToString, installationInfoComparisonMapper)
                    End If
                Case NameOf(Component_box_occurrence.Reference_element)
                    Dim currentReferenceElement As String = String.Empty
                    Dim compareReferenceElement As String = Harness.GetReferenceElement(compareProperty.ToString, _compareKBLMapper)

                    If (currentReferenceElement <> compareReferenceElement) Then
                        MyBase.ChangedProperties.Add(NameOf(Component_box_occurrence.Reference_element), String.Format("{0}{1}", vbLf, compareReferenceElement))
                    End If
                Case Else
                    MyBase.ChangedProperties.Add(objProperty.Name.ToString, compareProperty)
            End Select
        End If
        Return False
    End Function

End Class