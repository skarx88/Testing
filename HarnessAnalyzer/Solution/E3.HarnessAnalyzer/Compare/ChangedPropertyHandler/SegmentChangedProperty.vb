Imports System.Reflection
Imports Zuken.E3.HarnessAnalyzer.Settings


Public Class SegmentChangedProperty
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
        Dim compareNumValue As Numerical_value = TryCast(compareProperty, Numerical_value)
        If (currentProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(Segment.Alias_id)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Alias_identification)), DirectCast(compareProperty, IEnumerable(Of Alias_identification)))
                    Dim aliasIdComparisonMapper As New AliasIdComparisonMapper(Me._owner, listConvertToDictionary, Me.Settings)
                    aliasIdComparisonMapper.CompareObjects()

                    If aliasIdComparisonMapper.HasChanges Then
                        MyBase.ChangedProperties.Add(NameOf(Segment.Alias_id), aliasIdComparisonMapper)
                    End If
                Case NameOf(Segment.Virtual_length), NameOf(Segment.Physical_length)
                    If (compareProperty IsNot Nothing) Then
                        If Math.Round(DirectCast(currentProperty, Numerical_value).Value_component, 2) <> Math.Round(DirectCast(compareProperty, Numerical_value).Value_component, 2) Then
                            MyBase.ChangedProperties.Add(objProperty.Name, String.Format("{0} {1}", Math.Round(compareNumValue.Value_component, 2), _compareKBLMapper.GetUnit(compareNumValue.Unit_component).Unit_name))
                        End If
                    Else
                        MyBase.ChangedProperties.Add(objProperty.Name, String.Empty)
                    End If
                Case NameOf(Segment.Cross_section_area_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Cross_section_area)), DirectCast(currentObject, Segment).Id, DirectCast(compareProperty, IEnumerable(Of Cross_section_area)), DirectCast(compareObject, Segment).Id)
                    Dim crossSectionAreaComparisonMapper As New CrossSectionAreaComparisonMapper(Me._owner, listConvertToDictionary, Me.Settings)
                    crossSectionAreaComparisonMapper.CompareObjects()

                    If crossSectionAreaComparisonMapper.HasChanges Then
                        MyBase.ChangedProperties.Add(objProperty.Name, crossSectionAreaComparisonMapper)
                    End If
                Case NameOf(Segment.End_node), NameOf(Segment.Start_node)
                    Dim currentNode As Node = _currentKBLMapper.GetOccurrenceObject(Of Node)(currentProperty.ToString)
                    Dim compareNode As Node = _compareKBLMapper.GetOccurrenceObject(Of Node)(compareProperty.ToString)

                    If (currentNode IsNot Nothing) AndAlso (compareNode IsNot Nothing) AndAlso (currentNode.Id <> compareNode.Id) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, compareNode.Id)
                    End If
                Case NameOf(Segment.Fixing_assignment)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Fixing_assignment)), DirectCast(compareProperty, IEnumerable(Of Fixing_assignment)))
                    Dim fixingAssignmentComparisonMapper As New FixingAssignmentComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, _currentActiveObjects, _compareActiveObjects, listConvertToDictionary, Me.Settings)
                    fixingAssignmentComparisonMapper.CompareObjects()

                    If fixingAssignmentComparisonMapper.HasChanges Then
                        MyBase.ChangedProperties.Add(NameOf(Segment.Fixing_assignment), fixingAssignmentComparisonMapper)
                    End If
                Case NameOf(Segment.Processing_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Processing_instruction)), DirectCast(compareProperty, IEnumerable(Of Processing_instruction)))
                    Dim processingInfoComparisonMapper As New ProcessingInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, _currentActiveObjects, _compareActiveObjects, listConvertToDictionary, Me.Settings)
                    processingInfoComparisonMapper.CompareObjects()

                    If processingInfoComparisonMapper.HasChanges Then
                        MyBase.ChangedProperties.Add(NameOf(Segment.Processing_information), processingInfoComparisonMapper)
                    End If
                Case NameOf(Segment.Protection_area)
                    Dim listConvertToDictionary As New ListConvertToDictionary(_currentKBLMapper, DirectCast(currentProperty, IEnumerable(Of Protection_area)), _compareKBLMapper, DirectCast(compareProperty, IEnumerable(Of Protection_area)))
                    Dim protectionAreaComparisonMapper As New ProtectionAreaComparisonMapper(Me._owner, listConvertToDictionary, _currentKBLMapper, _compareKBLMapper, _currentActiveObjects, _compareActiveObjects, Me.Settings)
                    protectionAreaComparisonMapper.CompareObjects()

                    If protectionAreaComparisonMapper.HasChanges Then
                        MyBase.ChangedProperties.Add(objProperty.Name, protectionAreaComparisonMapper)
                    End If
                Case NameOf(Segment.Id), NameOf(Segment.Form)
                    If (compareProperty IsNot Nothing AndAlso currentProperty.ToString <> compareProperty.ToString) OrElse (compareProperty Is Nothing AndAlso currentProperty.ToString <> String.Empty) Then
                        If (objProperty.Name = NameOf(Segment.Id)) Then
                            MyBase.ChangedProperties.Add(NameOf(Segment.Id), compareProperty)
                        Else
                            MyBase.ChangedProperties.Add(objProperty.Name, compareProperty)
                        End If
                    End If
            End Select
        ElseIf (compareProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(Segment.Alias_id)
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Alias_identification), DirectCast(compareProperty, IEnumerable(Of Alias_identification)))
                    Dim aliasIdComparisonMapper As New AliasIdComparisonMapper(Me._owner, listConvertToDictionary, Me.Settings)
                    aliasIdComparisonMapper.CompareObjects()

                    If (aliasIdComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(NameOf(Segment.Alias_id), aliasIdComparisonMapper)
                    End If
                Case NameOf(Segment.Cross_section_area_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Cross_section_area), DirectCast(currentObject, Segment).Id, DirectCast(compareProperty, IEnumerable(Of Cross_section_area)), DirectCast(compareObject, Segment).Id)
                    Dim crossSectionAreaComparisonMapper As New CrossSectionAreaComparisonMapper(Me._owner, listConvertToDictionary, Me.Settings)
                    crossSectionAreaComparisonMapper.CompareObjects()

                    If (crossSectionAreaComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, crossSectionAreaComparisonMapper)
                    End If
                Case NameOf(Segment.End_node), NameOf(Segment.Start_node)
                    Dim compareNode As Node = _compareKBLMapper.GetOccurrenceObject(Of Node)(compareProperty.ToString)
                    If (compareNode IsNot Nothing) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, compareNode.Id)
                    End If
                Case NameOf(Segment.Fixing_assignment)
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Fixing_assignment), DirectCast(compareProperty, IEnumerable(Of Fixing_assignment)))
                    Dim fixingAssignmentComparisonMapper As New FixingAssignmentComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, _currentActiveObjects, _compareActiveObjects, listConvertToDictionary, Me.Settings)
                    fixingAssignmentComparisonMapper.CompareObjects()

                    If (fixingAssignmentComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(NameOf(Segment.Fixing_assignment), fixingAssignmentComparisonMapper)
                    End If
                Case NameOf(Segment.Processing_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Processing_instruction), DirectCast(compareProperty, IEnumerable(Of Processing_instruction)))
                    Dim processingInfoComparisonMapper As New ProcessingInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    processingInfoComparisonMapper.CompareObjects()

                    If (processingInfoComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(NameOf(Segment.Processing_information), processingInfoComparisonMapper)
                    End If
                Case Else
                    MyBase.ChangedProperties.Add(objProperty.Name, compareProperty)
            End Select
        End If
        Return False
    End Function
End Class
