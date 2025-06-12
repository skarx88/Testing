Imports System.Reflection
Imports Zuken.E3.HarnessAnalyzer.Settings


Public Class CoreChangedProperty
    Inherits ChangedProperty

    Public Sub New(owner As ComparisonMapper, currentKBLMapper As KblMapper, compareKBLMapper As KblMapper, settings As GeneralSettingsBase)
        MyBase.New(owner, currentKBLMapper, compareKBLMapper, settings)
    End Sub

    Public Overrides Function CompareObjectProperty(objProperty As PropertyInfo, currentObject As Object, compareObject As Object, excludeProperties As List(Of String)) As Boolean
        Dim currentProperty As Object = objProperty.GetValue(currentObject, Nothing)
        Dim compareProperty As Object = objProperty.GetValue(compareObject, Nothing)
        If (currentProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(Core.Bend_radius), NameOf(Core.Cross_section_area), NameOf(Core.Outside_diameter)
                    If (compareProperty IsNot Nothing) Then
                        If (Math.Round(DirectCast(currentProperty, Numerical_value).Value_component, 2) <> Math.Round(DirectCast(compareProperty, Numerical_value).Value_component, 2)) Then
                            MyBase.ChangedProperties.Add(objProperty.Name.ToString, String.Format("{0} {1}", Math.Round(DirectCast(compareProperty, Numerical_value).Value_component, 2), _compareKBLMapper.GetUnit(DirectCast(compareProperty, Numerical_value).Unit_component).Unit_name))
                        End If
                    Else
                        MyBase.ChangedProperties.Add(objProperty.Name.ToString, String.Empty)
                    End If

                Case NameOf(Core.Core_colour)
                    Dim currentCoreColours As String = DirectCast(currentObject, Core).GetColours
                    Dim compareCoreColours As String = DirectCast(compareObject, Core).GetColours

                    If (currentCoreColours <> compareCoreColours) Then MyBase.ChangedProperties.Add(NameOf(Core.Core_colour), compareCoreColours)
                Case NameOf(Core.Processing_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Processing_instruction)), DirectCast(compareProperty, IEnumerable(Of Processing_instruction)))
                    Dim processingInfoComparisonMapper As New ProcessingInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    processingInfoComparisonMapper.CompareObjects()

                    If (processingInfoComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(NameOf(Core.Processing_information), processingInfoComparisonMapper)
                    End If
                Case NameOf(Core.Cable_designator), NameOf(Core.Wire_type)
                    If (compareProperty IsNot Nothing AndAlso currentProperty.ToString <> compareProperty.ToString) OrElse (compareProperty Is Nothing AndAlso currentProperty.ToString <> String.Empty) Then
                        MyBase.ChangedProperties.Add(objProperty.Name.ToString, compareProperty)
                    End If
            End Select
        ElseIf (compareProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(Core.Processing_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Processing_instruction), DirectCast(compareProperty, IEnumerable(Of Processing_instruction)))
                    Dim processingInfoComparisonMapper As New ProcessingInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    processingInfoComparisonMapper.CompareObjects()

                    If (processingInfoComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(NameOf(Core.Processing_information), processingInfoComparisonMapper)
                    End If
                Case Else
                    MyBase.ChangedProperties.Add(objProperty.Name.ToString, compareProperty)
            End Select
        End If
        Return False
    End Function
End Class
