Imports System.Reflection
Imports Zuken.E3.HarnessAnalyzer.Settings


Public Class DimSpecChangedProperty
    Inherits ChangedProperty

    Public Sub New(owner As ComparisonMapper, currentKBLMapper As KblMapper, compareKBLMapper As KblMapper, settings As GeneralSettingsBase)
        MyBase.New(owner, currentKBLMapper, compareKBLMapper, settings)
    End Sub

    Public Overrides Function CompareObjectProperty(objProperty As PropertyInfo, currentObject As Object, compareObject As Object, excludeProperties As List(Of String)) As Boolean
        Dim currentProperty As Object = objProperty.GetValue(currentObject, Nothing)
        Dim compareProperty As Object = objProperty.GetValue(compareObject, Nothing)
        Dim compareNumericalValue As Numerical_value = TryCast(compareProperty, Numerical_value)
        Dim currentNumericalValue As Numerical_value = TryCast(currentProperty, Numerical_value)

        If (currentProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(E3.Lib.Schema.Kbl.Dimension_specification.Alias_id)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Alias_identification)), DirectCast(compareProperty, IEnumerable(Of Alias_identification)))
                    Dim aliasIdComparisonMapper As New AliasIdComparisonMapper(Me._owner, listConvertToDictionary, Me.Settings)
                    aliasIdComparisonMapper.CompareObjects()

                    If aliasIdComparisonMapper.HasChanges Then
                        MyBase.ChangedProperties.Add(NameOf(E3.Lib.Schema.Kbl.Dimension_specification.Alias_id), aliasIdComparisonMapper)
                    End If
                Case NameOf(E3.Lib.Schema.Kbl.Dimension_specification.Dimension_value)
                    Dim currentDimVal As String = String.Format("{0} {1}", Math.Round(DirectCast(currentProperty, Numerical_value).Value_component, 2), _currentKBLMapper.GetUnit(currentNumericalValue.Unit_component).Unit_name)
                    Dim compareDimVal As String = If(compareProperty Is Nothing, String.Empty, String.Format("{0} {1}", Math.Round(compareNumericalValue.Value_component, 2), _compareKBLMapper.GetUnit(compareNumericalValue.Unit_component).Unit_name))

                    If (currentDimVal <> compareDimVal) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, compareDimVal)
                    End If
                Case NameOf(E3.Lib.Schema.Kbl.Dimension_specification.Segments)
                    Dim currentSegmentIds As String = String.Empty
                    Dim compareSegmentIds As String = String.Empty

                    For Each segmentId As String In currentProperty.ToString.SplitSpace
                        currentSegmentIds = If(currentSegmentIds = String.Empty, [Lib].Schema.Kbl.Utils.GetUserId(_currentKBLMapper.GetOccurrenceObjectUntyped(segmentId)), String.Format("{0}; {1}", currentSegmentIds, [Lib].Schema.Kbl.Utils.GetUserId(_currentKBLMapper.GetOccurrenceObjectUntyped(segmentId))))
                    Next

                    If (compareProperty IsNot Nothing) Then
                        For Each segmentId As String In compareProperty.ToString.SplitSpace
                            compareSegmentIds = If(compareSegmentIds = String.Empty, [Lib].Schema.Kbl.Utils.GetUserId(_compareKBLMapper.GetOccurrenceObjectUntyped(segmentId)), String.Format("{0}; {1}", compareSegmentIds, [Lib].Schema.Kbl.Utils.GetUserId(_compareKBLMapper.GetOccurrenceObjectUntyped(segmentId))))
                        Next
                    End If

                    If (currentSegmentIds <> compareSegmentIds) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, compareSegmentIds)
                    End If
                Case NameOf(E3.Lib.Schema.Kbl.Dimension_specification.Origin), NameOf(E3.Lib.Schema.Kbl.Dimension_specification.Target)
                    Dim currentOriginTargetId As String = [Lib].Schema.Kbl.Utils.GetUserId(_currentKBLMapper.GetOccurrenceObjectUntyped(currentProperty.ToString))
                    Dim compareOriginTargetId As String = If(compareProperty IsNot Nothing, [Lib].Schema.Kbl.Utils.GetUserId(_compareKBLMapper.GetOccurrenceObjectUntyped(compareProperty.ToString)), String.Empty)

                    If (currentOriginTargetId <> compareOriginTargetId) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, compareOriginTargetId)
                    End If
                Case NameOf(E3.Lib.Schema.Kbl.Dimension_specification.Processing_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Processing_instruction)), DirectCast(compareProperty, IEnumerable(Of Processing_instruction)))
                    Dim processingInfoComparisonMapper As New ProcessingInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    processingInfoComparisonMapper.CompareObjects()

                    If (processingInfoComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(objProperty.Name.ToString, processingInfoComparisonMapper)
                    End If
                Case NameOf(E3.Lib.Schema.Kbl.Dimension_specification.Tolerance_indication)
                    Dim currentTolerance As Tolerance = DirectCast(currentProperty, Tolerance)
                    Dim compareTolerance As Tolerance = If(compareProperty IsNot Nothing, DirectCast(compareProperty, Tolerance), Nothing)
                    Dim currentToleranceIndication As String = String.Empty
                    Dim compareToleranceIndication As String = String.Empty

                    If (currentTolerance.Lower_limit IsNot Nothing) AndAlso (currentTolerance.Upper_limit IsNot Nothing) Then
                        currentToleranceIndication = String.Format("{0}: {1} / {2}: {3}", ObjectPropertyNameStrings.LowerLimit, String.Format("{0} {1}", Math.Round(currentTolerance.Lower_limit.Value_component, 2), _currentKBLMapper.GetUnit(currentTolerance.Lower_limit.Unit_component).Unit_name), ObjectPropertyNameStrings.UpperLimit, String.Format("{0} {1}", Math.Round(currentTolerance.Upper_limit.Value_component, 2), _currentKBLMapper.GetUnit(currentTolerance.Upper_limit.Unit_component).Unit_name))
                    ElseIf (currentTolerance.Lower_limit IsNot Nothing) Then
                        currentToleranceIndication = String.Format("{0}: {1}", ObjectPropertyNameStrings.LowerLimit, String.Format("{0} {1}", Math.Round(currentTolerance.Lower_limit.Value_component, 2), _currentKBLMapper.GetUnit(currentTolerance.Lower_limit.Unit_component).Unit_name))
                    Else
                        currentToleranceIndication = String.Format("{0}: {1}", ObjectPropertyNameStrings.UpperLimit, String.Format("{0} {1}", Math.Round(currentTolerance.Upper_limit.Value_component, 2), _currentKBLMapper.GetUnit(currentTolerance.Upper_limit.Unit_component).Unit_name))
                    End If

                    If (compareTolerance IsNot Nothing) Then
                        If (compareTolerance.Lower_limit IsNot Nothing) AndAlso (compareTolerance.Upper_limit IsNot Nothing) Then
                            compareToleranceIndication = String.Format("{0}: {1} / {2}: {3}", ObjectPropertyNameStrings.LowerLimit, String.Format("{0} {1}", Math.Round(compareTolerance.Lower_limit.Value_component, 2), _compareKBLMapper.GetUnit(compareTolerance.Lower_limit.Unit_component).Unit_name), ObjectPropertyNameStrings.UpperLimit, String.Format("{0} {1}", Math.Round(compareTolerance.Upper_limit.Value_component, 2), _compareKBLMapper.GetUnit(compareTolerance.Upper_limit.Unit_component).Unit_name))
                        ElseIf (compareTolerance.Lower_limit IsNot Nothing) Then
                            compareToleranceIndication = String.Format("{0}: {1}", ObjectPropertyNameStrings.LowerLimit, String.Format("{0} {1}", Math.Round(compareTolerance.Lower_limit.Value_component, 2), _compareKBLMapper.GetUnit(compareTolerance.Lower_limit.Unit_component).Unit_name))
                        Else
                            compareToleranceIndication = String.Format("{0}: {1}", ObjectPropertyNameStrings.UpperLimit, String.Format("{0} {1}", Math.Round(compareTolerance.Upper_limit.Value_component, 2), _compareKBLMapper.GetUnit(compareTolerance.Upper_limit.Unit_component).Unit_name))
                        End If
                    End If

                    If (currentToleranceIndication <> compareToleranceIndication) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, compareToleranceIndication)
                    End If
            End Select
        ElseIf (compareProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(E3.Lib.Schema.Kbl.Dimension_specification.Alias_id)
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Alias_identification), DirectCast(compareProperty, IEnumerable(Of Alias_identification)))
                    Dim aliasIdComparisonMapper As New AliasIdComparisonMapper(Me._owner, listConvertToDictionary, Me.Settings)
                    aliasIdComparisonMapper.CompareObjects()

                    If (aliasIdComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(NameOf(E3.Lib.Schema.Kbl.Dimension_specification.Alias_id), aliasIdComparisonMapper)
                    End If
                Case NameOf(E3.Lib.Schema.Kbl.Dimension_specification.Dimension_value)
                    MyBase.ChangedProperties.Add(objProperty.Name.ToString, String.Format("{0} {1}", Math.Round(compareNumericalValue.Value_component, 2), _compareKBLMapper.GetUnit(compareNumericalValue.Unit_component).Unit_name))
                Case NameOf(E3.Lib.Schema.Kbl.Dimension_specification.Segments)
                    Dim compareSegmentIds As String = String.Empty

                    For Each segmentId As String In compareProperty.ToString.SplitSpace
                        compareSegmentIds = If(compareSegmentIds = String.Empty, [Lib].Schema.Kbl.Utils.GetUserId(_compareKBLMapper.GetOccurrenceObjectUntyped(segmentId)), String.Format("{0}; {1}", compareSegmentIds, [Lib].Schema.Kbl.Utils.GetUserId(_compareKBLMapper.GetOccurrenceObjectUntyped(segmentId))))
                    Next

                    MyBase.ChangedProperties.Add(objProperty.Name, compareSegmentIds)
                Case NameOf(E3.Lib.Schema.Kbl.Dimension_specification.Processing_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Processing_instruction), DirectCast(compareProperty, IEnumerable(Of Processing_instruction)))
                    Dim processingInfoComparisonMapper As New ProcessingInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    processingInfoComparisonMapper.CompareObjects()

                    If (processingInfoComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(objProperty.Name.ToString, processingInfoComparisonMapper)
                    End If
                Case NameOf(E3.Lib.Schema.Kbl.Dimension_specification.Tolerance_indication)
                    Dim compareTolerance As Tolerance = DirectCast(compareProperty, Tolerance)
                    Dim compareToleranceIndication As String = String.Empty

                    If (compareTolerance.Lower_limit IsNot Nothing) AndAlso (compareTolerance.Upper_limit IsNot Nothing) Then
                        compareToleranceIndication = String.Format("{0}: {1} / {2}: {3}", ObjectPropertyNameStrings.LowerLimit, String.Format("{0} {1}", Math.Round(compareTolerance.Lower_limit.Value_component, 2), _compareKBLMapper.GetUnit(compareTolerance.Lower_limit.Unit_component).Unit_name), ObjectPropertyNameStrings.UpperLimit, String.Format("{0} {1}", Math.Round(compareTolerance.Upper_limit.Value_component, 2), _compareKBLMapper.GetUnit(compareTolerance.Upper_limit.Unit_component).Unit_name))
                    ElseIf (compareTolerance.Lower_limit IsNot Nothing) Then
                        compareToleranceIndication = String.Format("{0}: {1}", ObjectPropertyNameStrings.LowerLimit, String.Format("{0} {1}", Math.Round(compareTolerance.Lower_limit.Value_component, 2), _compareKBLMapper.GetUnit(compareTolerance.Lower_limit.Unit_component).Unit_name))
                    Else
                        compareToleranceIndication = String.Format("{0}: {1}", ObjectPropertyNameStrings.UpperLimit, String.Format("{0} {1}", Math.Round(compareTolerance.Upper_limit.Value_component, 2), _compareKBLMapper.GetUnit(compareTolerance.Upper_limit.Unit_component).Unit_name))
                    End If

                    MyBase.ChangedProperties.Add(objProperty.Name, compareToleranceIndication)
            End Select
        End If
        Return False
    End Function
End Class
