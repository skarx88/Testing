Imports System.Reflection
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.HarnessAnalyzer.Shared


Public Class FixingAssignmentChangedProperty
    Inherits ChangedProperty

    Public Sub New(owner As ComparisonMapper, currentKBLMapper As KblMapper, compareKBLMapper As KblMapper, settings As GeneralSettingsBase)
        MyBase.New(owner, currentKBLMapper, compareKBLMapper, settings)
    End Sub

    Public Overrides Function CompareObjectProperty(objProperty As PropertyInfo, currentObject As Object, compareObject As Object, excludeProperties As List(Of String)) As Boolean
        Dim currentProperty As Object = objProperty.GetValue(currentObject, Nothing)
        Dim compareProperty As Object = objProperty.GetValue(compareObject, Nothing)
        If (currentProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(E3.Lib.Schema.Kbl.Fixing_assignment.Alias_id)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Alias_identification)), DirectCast(compareProperty, IEnumerable(Of Alias_identification)))
                    Dim aliasIdComparisonMapper As New AliasIdComparisonMapper(Me._owner, listConvertToDictionary, Me.Settings)
                    aliasIdComparisonMapper.CompareObjects()

                    If (aliasIdComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(NameOf(E3.Lib.Schema.Kbl.Fixing_assignment.Alias_id), aliasIdComparisonMapper)
                    End If
                Case NameOf(E3.Lib.Schema.Kbl.Fixing_assignment.Location)
                    Dim currentLocation As Double = Math.Round(CDbl(currentProperty), NOF_DIGITS_LOCATIONS)
                    Dim compareLocation As Double = Math.Round(CDbl(compareProperty), NOF_DIGITS_LOCATIONS)

                    If (currentLocation <> compareLocation) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, compareLocation)
                    End If

                Case NameOf(E3.Lib.Schema.Kbl.Fixing_assignment.Absolute_location)
                    Dim currentLocation As Double = Math.Round(CDbl(CType(currentProperty, Numerical_value).Value_component), 2)
                    If (compareProperty IsNot Nothing) Then
                        Dim compareLocation As Double = Math.Round(CDbl(CType(compareProperty, Numerical_value).Value_component), 2)

                        If (currentLocation <> compareLocation) Then
                            MyBase.ChangedProperties.Add(objProperty.Name, compareLocation)
                        End If
                    Else
                        MyBase.ChangedProperties.Add(objProperty.Name, String.Empty)
                    End If

                Case NameOf(E3.Lib.Schema.Kbl.Fixing_assignment.Fixing)
                    Dim currentRef As String = Harness.GetReferenceElement(currentProperty.ToString, _currentKBLMapper)
                    Dim compareRef As String = Harness.GetReferenceElement(compareProperty.ToString, _compareKBLMapper)

                    If (currentRef <> compareRef) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, compareRef)
                    End If

                Case NameOf(E3.Lib.Schema.Kbl.Fixing_assignment.Processing_information)

                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Processing_instruction)), DirectCast(compareProperty, IEnumerable(Of Processing_instruction)))
                    Dim processingInfoComparisonMapper As New ProcessingInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    processingInfoComparisonMapper.CompareObjects()

                    If (processingInfoComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(NameOf(E3.Lib.Schema.Kbl.Fixing_assignment.Processing_information), processingInfoComparisonMapper)
                    End If

                Case Else
                    If (compareProperty IsNot Nothing AndAlso currentProperty.ToString <> compareProperty.ToString) OrElse (compareProperty Is Nothing AndAlso currentProperty.ToString <> String.Empty) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, compareProperty)
                    End If
            End Select
        ElseIf (compareProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(E3.Lib.Schema.Kbl.Fixing_assignment.Alias_id)
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Alias_identification), DirectCast(compareProperty, IEnumerable(Of Alias_identification)))
                    Dim aliasIdComparisonMapper As New AliasIdComparisonMapper(Me._owner, listConvertToDictionary, Me.Settings)
                    aliasIdComparisonMapper.CompareObjects()

                    If (aliasIdComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(NameOf(E3.Lib.Schema.Kbl.Fixing_assignment.Alias_id), aliasIdComparisonMapper)
                    End If
                Case NameOf(E3.Lib.Schema.Kbl.Fixing_assignment.Location)
                    Dim compareLocation As Double = Math.Round(CDbl(compareProperty), NOF_DIGITS_LOCATIONS)

                    MyBase.ChangedProperties.Add(objProperty.Name, compareLocation)

                Case NameOf(E3.Lib.Schema.Kbl.Fixing_assignment.Absolute_location)
                    Dim compareLocation As Double = Math.Round(CDbl(CType(compareProperty, Numerical_value).Value_component), 1)

                    MyBase.ChangedProperties.Add(objProperty.Name, compareLocation)

                Case NameOf(E3.Lib.Schema.Kbl.Fixing_assignment.Fixing)
                    Dim compareRef As String = Harness.GetReferenceElement(compareProperty.ToString, _compareKBLMapper)
                    If (compareRef <> String.Empty) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, compareRef)
                    End If
                Case Else
                    MyBase.ChangedProperties.Add(objProperty.Name, compareProperty)
            End Select
        End If
        Return False
    End Function

End Class
