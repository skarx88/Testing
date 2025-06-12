Imports System.Reflection
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Public Class ChangeDescriptionChangedProperty
    Inherits ChangedProperty

    Public Sub New(owner As ComparisonMapper, currentKBLMapper As KblMapper, compareKBLMapper As KblMapper, settings As GeneralSettingsBase)
        MyBase.New(owner, currentKBLMapper, compareKBLMapper, settings)
    End Sub

    Public Overrides Function CompareObjectProperty(objProperty As PropertyInfo, currentObject As Object, compareObject As Object, excludeProperties As List(Of String)) As Boolean
        Dim currentProperty As Object = objProperty.GetValue(currentObject, Nothing)
        Dim compareProperty As Object = objProperty.GetValue(compareObject, Nothing)
        If (currentProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(Change_description.Description), NameOf(Change_description.Change_request), NameOf(Change_description.Change_date), NameOf(Change_description.Responsible_designer), NameOf(Change_description.Designer_department), NameOf(Change_description.Approver_name), NameOf(Change_description.Approver_department)
                    If (compareProperty Is Nothing) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, String.Empty)
                    ElseIf (currentProperty.ToString <> compareProperty.ToString) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, compareProperty.ToString)
                    End If

                Case NameOf(Change_description.Changed_elements)
                    Dim currentChangedElements As New List(Of String)
                    For Each referencedElements As String In currentProperty.ToString.SplitSpace.ToList
                        currentChangedElements.Add(Harness.GetReferenceElement(referencedElements, _currentKBLMapper))
                    Next
                    currentChangedElements.Sort()

                    Dim compareChangedElements As New List(Of String)
                    If (compareProperty IsNot Nothing) Then
                        For Each referencedComponent As String In compareProperty.ToString.SplitSpace.ToList
                            compareChangedElements.Add(Harness.GetReferenceElement(referencedComponent, _compareKBLMapper))
                        Next
                        compareChangedElements.Sort()
                    End If

                    Dim currentChangedElementsString As String = String.Empty
                    Dim compareChangedElementsString As String = String.Empty

                    For Each id As String In currentChangedElements
                        currentChangedElementsString = String.Format("{0}{1}{2}", currentChangedElementsString, vbCrLf, id)
                    Next
                    currentChangedElementsString = currentChangedElementsString.Trim(vbCrLf.ToCharArray)

                    For Each id As String In compareChangedElements
                        compareChangedElementsString = String.Format("{0}{1}{2}", compareChangedElementsString, vbCrLf, id)
                    Next
                    compareChangedElementsString = compareChangedElementsString.Trim(vbCrLf.ToCharArray)

                    If (currentChangedElementsString <> compareChangedElementsString) Then
                        MyBase.ChangedProperties.Add(ChangeDescriptionPropertyName.Changed_elements.ToString, compareChangedElementsString)
                    End If

                Case NameOf(Change_description.External_references)
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

                Case NameOf(Change_description.Related_changes)
                    Dim currentChanges As New List(Of Change)
                    Dim compareChanges As New List(Of Change)

                    For Each changeId As String In currentProperty.ToString.SplitSpace
                        If (_currentKBLMapper.KBLChangeMapper.ContainsKey(changeId)) Then
                            currentChanges.Add(_currentKBLMapper.KBLChangeMapper(changeId))
                        End If
                    Next

                    If (compareProperty IsNot Nothing) Then
                        For Each changeId As String In compareProperty.ToString.SplitSpace
                            If (_compareKBLMapper.KBLChangeMapper.ContainsKey(changeId)) Then
                                compareChanges.Add(_compareKBLMapper.KBLChangeMapper(changeId))
                            End If
                        Next
                    End If

                    Dim listConvertToDictionary As New ListConvertToDictionary(currentChanges, compareChanges)
                    Dim changeComparisonMapper As New CommonComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    changeComparisonMapper.CompareObjects()

                    If (changeComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, changeComparisonMapper)
                    End If

            End Select
        ElseIf (compareProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(Change_description.Changed_elements)
                    Dim refInfos As String = String.Empty

                    For Each refId As String In compareProperty.ToString.SplitSpace
                        If (refInfos = String.Empty) Then
                            refInfos = Harness.GetReferenceElement(refId, _compareKBLMapper)
                        Else
                            refInfos = String.Format("{0}{1}{2}", refInfos, vbCrLf, Harness.GetReferenceElement(refId, _compareKBLMapper))
                        End If
                    Next

                    If (refInfos <> String.Empty) Then
                        MyBase.ChangedProperties.Add(NameOf(Change_description.Changed_elements), String.Format("{0}{1}", vbCrLf, refInfos))
                    End If

                Case NameOf(Change_description.External_references)
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

                Case ChangeDescriptionPropertyName.Related_changes.ToString
                    Dim currentChanges As New List(Of Change)
                    Dim compareChanges As New List(Of Change)

                    For Each changeId As String In compareProperty.ToString.SplitSpace
                        If (_compareKBLMapper.KBLChangeMapper.ContainsKey(changeId)) Then
                            compareChanges.Add(_compareKBLMapper.KBLChangeMapper(changeId))
                        End If
                    Next

                    Dim listConvertToDictionary As New ListConvertToDictionary(currentChanges, compareChanges)
                    Dim changeComparisonMapper As New CommonComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    changeComparisonMapper.CompareObjects()

                    If (changeComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, changeComparisonMapper)
                    End If

                Case Else

                    MyBase.ChangedProperties.Add(objProperty.Name.ToString, compareProperty)
            End Select
        End If
        Return False
    End Function
End Class