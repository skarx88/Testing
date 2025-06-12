Imports System.Reflection
Imports System.Text
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Public Class VertexChangedProperty
    Inherits ChangedProperty

    Private _currentActiveObjects As ICollection(Of String)
    Private _compareActiveObjects As ICollection(Of String)

    Public Sub New(owner As ComparisonMapper, currentKBLMapper As KblMapper, compareKBLMapper As KblMapper, currentActiveObjects As ICollection(Of String), compareActiveObjects As ICollection(Of String), settings As GeneralSettingsBase)
        MyBase.New(owner, currentKBLMapper, compareKBLMapper, settings)

        _currentActiveObjects = currentActiveObjects
        _compareActiveObjects = compareActiveObjects
    End Sub

    Public Overrides Function CompareObjectProperty(objProperty As PropertyInfo, currentObject As Object, compareObject As Object, excludeProperties As List(Of String)) As Boolean
        Dim currentProperty As Object = objProperty.GetValue(currentObject, Nothing)
        Dim compareProperty As Object = objProperty.GetValue(compareObject, Nothing)
        If (currentProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(Zuken.E3.Lib.Schema.Kbl.Node.Alias_id)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Alias_identification)), DirectCast(compareProperty, IEnumerable(Of Alias_identification)))
                    Dim aliasIdComparisonMapper As New AliasIdComparisonMapper(Me._owner, listConvertToDictionary, Me.Settings)
                    aliasIdComparisonMapper.CompareObjects()

                    If aliasIdComparisonMapper.HasChanges Then
                        MyBase.ChangedProperties.Add(NameOf(Zuken.E3.Lib.Schema.Kbl.Node.Alias_id), aliasIdComparisonMapper)
                    End If

                Case NameOf(Zuken.E3.Lib.Schema.Kbl.Node.Folding_direction)
                    If (compareProperty IsNot Nothing) Then
                        Dim currentFoldingDirSegment As String = If(_currentKBLMapper.GetOccurrenceObject(Of Segment)(currentProperty.ToString)?.Id, String.Empty)
                        Dim compareFoldingDirSegment As String = If(_compareKBLMapper.GetOccurrenceObject(Of Segment)(compareProperty.ToString)?.Id, String.Empty)

                        If (currentFoldingDirSegment <> compareFoldingDirSegment) Then
                            MyBase.ChangedProperties.Add(objProperty.Name, compareFoldingDirSegment)
                        End If
                    Else
                        MyBase.ChangedProperties.Add(objProperty.Name, compareProperty)
                    End If

                Case NameOf(Zuken.E3.Lib.Schema.Kbl.Node.Processing_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Processing_instruction)), DirectCast(compareProperty, IEnumerable(Of Processing_instruction)))
                    Dim processingInfoComparisonMapper As New ProcessingInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, _currentActiveObjects, _compareActiveObjects, listConvertToDictionary, Me.Settings)
                    processingInfoComparisonMapper.CompareObjects()

                    If processingInfoComparisonMapper.HasChanges Then
                        MyBase.ChangedProperties.Add(NameOf(Zuken.E3.Lib.Schema.Kbl.Node.Processing_information), processingInfoComparisonMapper)
                    End If

                Case NameOf(Zuken.E3.Lib.Schema.Kbl.Node.Referenced_components)

                    Dim currentRefComponents As New List(Of String)
                    For Each referencedComponent As String In currentProperty.ToString.SplitSpace.ToList
                        currentRefComponents.Add(Harness.GetReferenceElement(referencedComponent, _currentKBLMapper))
                    Next
                    currentRefComponents.Sort()

                    Dim compareRefComponents As New List(Of String)
                    If (compareProperty IsNot Nothing) Then
                        For Each referencedComponent As String In compareProperty.ToString.SplitSpace.ToList
                            compareRefComponents.Add(Harness.GetReferenceElement(referencedComponent, _compareKBLMapper))
                        Next
                        compareRefComponents.Sort()
                    End If

                    Dim currentReferencedCompString As String = currentRefComponents.AsText
                    Dim compareReferencedCompString As String = compareRefComponents.AsText

                    If (currentReferencedCompString <> compareReferencedCompString) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, compareReferencedCompString)
                    End If

                    'TODO Ref to wire protection occurence does not really work here properly-we detect only add or missing protections but no changes, as changes on the protection are not visible
                    'here in any grid! We need i.e. a sub grid with these protections under the vertices-protections are only visible under the segments today
                    'GetReferencedElement seems to resolve a few properties and combine it to a string, so some but not all properties are compared...


                Case NameOf(Zuken.E3.Lib.Schema.Kbl.Node.Cartesian_point)
                    Dim currentCartesianPoint As Cartesian_point = _currentKBLMapper.GetOccurrenceObject(Of Cartesian_point)(currentProperty.ToString)
                    Dim compareCartesianPoint As Cartesian_point = _compareKBLMapper.GetOccurrenceObject(Of Cartesian_point)(compareProperty.ToString)

                    If Math.Round(currentCartesianPoint.Coordinates(0), 1) <> Math.Round(compareCartesianPoint.Coordinates(0), 1) Then
                        MyBase.ChangedProperties.Add(VertexPropertyName.Cartesian_pointX.ToString, Math.Round(compareCartesianPoint.Coordinates(0), 1))
                    End If

                    If Math.Round(currentCartesianPoint.Coordinates(1), 1) <> Math.Round(compareCartesianPoint.Coordinates(1), 1) Then
                        MyBase.ChangedProperties.Add(VertexPropertyName.Cartesian_pointY.ToString, Math.Round(compareCartesianPoint.Coordinates(1), 1))
                    End If

                    If (currentCartesianPoint.Coordinates.Length > 2) Then
                        If Math.Round(currentCartesianPoint.Coordinates(2), 1) <> Math.Round(compareCartesianPoint.Coordinates(2), 1) Then
                            MyBase.ChangedProperties.Add(VertexPropertyName.Cartesian_pointZ.ToString, Math.Round(compareCartesianPoint.Coordinates(2), 1))
                        End If
                    End If

                Case NameOf(Zuken.E3.Lib.Schema.Kbl.Node.Referenced_cavities)
                    If (compareProperty IsNot Nothing) Then
                        Dim currentRefCavities As New List(Of String)
                        For Each referencedCavityId As String In currentProperty.ToString.SplitSpace
                            currentRefCavities.Add(Harness.GetReferenceElement(referencedCavityId, _currentKBLMapper))
                        Next
                        currentRefCavities.Sort()

                        Dim compareRefCavities As New List(Of String)
                        For Each referencedCavityId As String In compareProperty.ToString.SplitSpace
                            compareRefCavities.Add(Harness.GetReferenceElement(referencedCavityId, _compareKBLMapper))
                        Next
                        compareRefCavities.Sort()

                        Dim currentReferencedCavString As String = currentRefCavities.AsText
                        Dim compareReferencedCavString As String = compareRefCavities.AsText

                        If (currentReferencedCavString <> compareReferencedCavString) Then
                            MyBase.ChangedProperties.Add(NameOf(Zuken.E3.Lib.Schema.Kbl.Node.Referenced_cavities), compareReferencedCavString)
                        End If

                    Else
                        MyBase.ChangedProperties.Add(objProperty.Name, compareProperty)
                    End If
                Case Else
                    If (TypeOf currentProperty Is String) OrElse (IsNumeric(currentProperty)) Then
                        If (compareProperty IsNot Nothing AndAlso currentProperty.ToString <> compareProperty.ToString) OrElse (compareProperty Is Nothing AndAlso currentProperty.ToString <> String.Empty) Then
                            If (objProperty.Name = NameOf(Zuken.E3.Lib.Schema.Kbl.Node.Id)) Then
                                MyBase.ChangedProperties.Add(NameOf(Zuken.E3.Lib.Schema.Kbl.Node.Id), compareProperty)
                            Else
                                MyBase.ChangedProperties.Add(objProperty.Name, compareProperty)
                            End If
                        End If
                    End If
            End Select

        ElseIf (compareProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(Zuken.E3.Lib.Schema.Kbl.Node.Alias_id)
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Alias_identification), DirectCast(compareProperty, IEnumerable(Of Alias_identification)))
                    Dim aliasIdComparisonMapper As New AliasIdComparisonMapper(Me._owner, listConvertToDictionary, Me.Settings)
                    aliasIdComparisonMapper.CompareObjects()

                    If (aliasIdComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(NameOf(Zuken.E3.Lib.Schema.Kbl.Node.Alias_id), aliasIdComparisonMapper)
                    End If

                Case NameOf(Zuken.E3.Lib.Schema.Kbl.Node.Folding_direction)
                    Dim compareFoldingDirSegment As String = If(_compareKBLMapper.GetSegment(compareProperty.ToString)?.Id, String.Empty)

                    MyBase.ChangedProperties.Add(objProperty.Name, compareFoldingDirSegment)

                Case NameOf(Zuken.E3.Lib.Schema.Kbl.Node.Processing_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Processing_instruction), DirectCast(compareProperty, IEnumerable(Of Processing_instruction)))
                    Dim processingInfoComparisonMapper As New ProcessingInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, _currentActiveObjects, _compareActiveObjects, listConvertToDictionary, Me.Settings)
                    processingInfoComparisonMapper.CompareObjects()

                    If (processingInfoComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(NameOf(Zuken.E3.Lib.Schema.Kbl.Node.Processing_information), processingInfoComparisonMapper)
                    End If

                Case NameOf(Zuken.E3.Lib.Schema.Kbl.Node.Referenced_cavities)
                    Dim referencedCavities As String = String.Empty
                    Dim compareRefCavities As New List(Of String)
                    For Each referencedCavity As String In compareProperty.ToString.SplitSpace.ToList
                        compareRefCavities.Add(Harness.GetReferenceElement(referencedCavity, _compareKBLMapper))
                    Next
                    For Each entry As String In compareRefCavities
                        If (referencedCavities = String.Empty) Then
                            referencedCavities = entry
                        Else
                            referencedCavities = String.Format("{0}{1}{2}", referencedCavities, vbCrLf, entry)
                        End If
                    Next
                    If (referencedCavities <> String.Empty) Then
                        MyBase.ChangedProperties.Add(NameOf(Zuken.E3.Lib.Schema.Kbl.Node.Referenced_cavities), String.Format("{0}{1}", vbCrLf, referencedCavities))
                    End If

                Case NameOf(Zuken.E3.Lib.Schema.Kbl.Node.Referenced_components)
                    Dim referencedComponents As String = String.Empty
                    Dim compareRefComponents As New List(Of String)
                    For Each referencedComp As String In compareProperty.ToString.SplitSpace.ToList
                        compareRefComponents.Add(Harness.GetReferenceElement(referencedComp, _compareKBLMapper))
                    Next
                    For Each entry As String In compareRefComponents
                        If (referencedComponents = String.Empty) Then
                            referencedComponents = entry
                        Else
                            referencedComponents = String.Format("{0}{1}{2}", referencedComponents, vbCrLf, entry)
                        End If
                    Next
                    If (referencedComponents <> String.Empty) Then
                        MyBase.ChangedProperties.Add(NameOf(Zuken.E3.Lib.Schema.Kbl.Node.Referenced_components), String.Format("{0}{1}", vbCrLf, referencedComponents))
                    End If

                Case Else
                    MyBase.ChangedProperties.Add(objProperty.Name, compareProperty)
            End Select
        End If
        Return False
    End Function

End Class
