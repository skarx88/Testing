Imports System.Reflection
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.HarnessAnalyzer.Shared


Public Class ProtectionAreaChangedProperty
    Inherits ChangedProperty

    Public Sub New(owner As ComparisonMapper, currentKBLMapper As KblMapper, compareKBLMapper As KblMapper, settings As GeneralSettingsBase)
        MyBase.New(owner, currentKBLMapper, compareKBLMapper, settings)
    End Sub

    Public Overrides Function CompareObjectProperty(objProperty As PropertyInfo, currentObject As Object, compareObject As Object, excludeProperties As List(Of String)) As Boolean
        Dim currentProperty As Object = objProperty.GetValue(currentObject, Nothing)
        Dim compareProperty As Object = objProperty.GetValue(compareObject, Nothing)
        If (currentProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(Protection_area.Processing_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Processing_instruction)), DirectCast(compareProperty, IEnumerable(Of Processing_instruction)))
                    Dim processingInfoComparisonMapper As New ProcessingInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    processingInfoComparisonMapper.CompareObjects()

                    If processingInfoComparisonMapper.HasChanges Then
                        MyBase.ChangedProperties.Add(NameOf(Protection_area.Processing_information), processingInfoComparisonMapper)
                    End If
                Case NameOf(Protection_area.Associated_protection)
                    Dim currentWireProtectionOccurrence As Wire_protection_occurrence = _currentKBLMapper.GetWireProtectionOccurrences.Where(Function(wireProt) wireProt.SystemId = currentProperty.ToString)(0)
                    Dim compareWireProtectionOccurrence As Wire_protection_occurrence = Nothing

                    If (_compareKBLMapper.GetWireProtectionOccurrences.Where(Function(wireProt) wireProt.SystemId = compareProperty.ToString).Any()) Then
                        compareWireProtectionOccurrence = _compareKBLMapper.GetWireProtectionOccurrences.Where(Function(wireProt) wireProt.SystemId = compareProperty.ToString)(0)
                    End If

                    If (compareWireProtectionOccurrence IsNot Nothing) Then
                        Dim wireProtectionOccurrenceChangedProperty As WireProtectionOccurrenceChangedProperty = New WireProtectionOccurrenceChangedProperty(Me._owner, _currentKBLMapper, _compareKBLMapper, Me.Settings)

                        wireProtectionOccurrenceChangedProperty.CompareObjectProperties(currentWireProtectionOccurrence, compareWireProtectionOccurrence, excludeProperties)
                        If wireProtectionOccurrenceChangedProperty.ChangedProperties.Count <> 0 Then
                            MyBase.ChangedProperties.Add(objProperty.Name, wireProtectionOccurrenceChangedProperty)
                        End If
                    End If
                Case NameOf(Protection_area.Gradient)
                    Dim currentGradient As String = String.Format("{0} {1}", DirectCast(currentProperty, Value_with_unit).SystemId, DirectCast(currentProperty, Value_with_unit).Unit_component)
                    If (compareProperty IsNot Nothing) Then
                        Dim compareGradient As String = String.Format("{0} {1}", DirectCast(compareProperty, Value_with_unit).SystemId, DirectCast(compareProperty, Value_with_unit).Unit_component)

                        If (currentGradient <> compareGradient) Then
                            MyBase.ChangedProperties.Add(objProperty.Name, compareGradient)
                        End If
                    Else
                        MyBase.ChangedProperties.Add(objProperty.Name, String.Empty)
                    End If

                Case NameOf(Protection_area.Absolute_start_location), NameOf(Protection_area.Absolute_end_location)
                    Dim currentAbsoluteLocation As String = String.Format("{0} {1}", Math.Round(DirectCast(currentProperty, Numerical_value).Value_component, 1), DirectCast(currentProperty, Numerical_value).Unit_component)
                    If (compareProperty IsNot Nothing) Then
                        Dim compareAbsoluteLocation As String = String.Format("{0} {1}", Math.Round(DirectCast(compareProperty, Numerical_value).Value_component, 1), DirectCast(compareProperty, Numerical_value).Unit_component)
                        If (currentAbsoluteLocation <> compareAbsoluteLocation) Then
                            MyBase.ChangedProperties.Add(objProperty.Name, compareAbsoluteLocation)
                        End If
                    Else
                        MyBase.ChangedProperties.Add(objProperty.Name, String.Empty)
                    End If
                Case NameOf(Protection_area.Is_on_top_of)
                    Dim currentIsOnTopOfProtections As String = DirectCast(currentObject, Protection_area).GetIsOnTopOfProtections(_currentKBLMapper)
                    If (compareObject IsNot Nothing) Then
                        Dim compareIsOnTopOfProtections As String = DirectCast(compareObject, Protection_area).GetIsOnTopOfProtections(_compareKBLMapper)
                        If (currentIsOnTopOfProtections <> compareIsOnTopOfProtections) Then MyBase.ChangedProperties.Add(objProperty.Name, compareIsOnTopOfProtections)
                    Else
                        MyBase.ChangedProperties.Add(objProperty.Name, String.Empty)
                    End If

                Case NameOf(Protection_area.Start_location), NameOf(Protection_area.End_location)
                    Dim currentLocation As String = String.Format("{0}", Math.Round(CDbl(currentProperty), NOF_DIGITS_LOCATIONS))
                    If (compareProperty IsNot Nothing) Then
                        Dim compareLocation As String = String.Format("{0}", Math.Round(CDbl(compareProperty), NOF_DIGITS_LOCATIONS))
                        If (currentLocation <> compareLocation) Then
                            MyBase.ChangedProperties.Add(objProperty.Name, compareLocation)
                        End If
                    Else
                        MyBase.ChangedProperties.Add(objProperty.Name, String.Empty)
                    End If

                Case Else
                    If (compareProperty IsNot Nothing AndAlso currentProperty.ToString <> compareProperty.ToString) OrElse (compareProperty Is Nothing AndAlso currentProperty.ToString <> String.Empty) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, compareProperty)
                    End If
            End Select
        ElseIf (compareProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(Protection_area.Processing_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Processing_instruction), DirectCast(compareProperty, IEnumerable(Of Processing_instruction)))
                    Dim processingInfoComparisonMapper As New ProcessingInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    processingInfoComparisonMapper.CompareObjects()

                    If (processingInfoComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(NameOf(Protection_area.Processing_information), processingInfoComparisonMapper)
                    End If
                Case NameOf(Protection_area.Associated_protection)
                    Dim wireProtectionOccurrence As Wire_protection_occurrence = Nothing

                    If (_compareKBLMapper.GetWireProtectionOccurrences.Where(Function(wireProt) wireProt.SystemId = compareProperty.ToString).Any()) Then
                        wireProtectionOccurrence = _compareKBLMapper.GetWireProtectionOccurrences.Where(Function(wireProt) wireProt.SystemId = compareProperty.ToString)(0)
                    End If

                    If (wireProtectionOccurrence IsNot Nothing) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, wireProtectionOccurrence.Id)
                    End If
                Case NameOf(Protection_area.Is_on_top_of)
                    Dim compareIsOnTopOfProtections As String = DirectCast(compareObject, Protection_area).GetIsOnTopOfProtections(_compareKBLMapper)
                    If (compareIsOnTopOfProtections <> String.Empty) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, compareIsOnTopOfProtections)
                    End If
                Case Else
                    MyBase.ChangedProperties.Add(objProperty.Name, compareProperty)
            End Select
        End If
        Return False
    End Function

End Class
