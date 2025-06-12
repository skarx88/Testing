Imports System.Reflection
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Public Class DefDimSpecChangedProperty
    Inherits ChangedProperty

    Public Sub New(owner As ComparisonMapper, currentKBLMapper As KblMapper, compareKBLMapper As KblMapper, settings As GeneralSettingsBase)
        MyBase.New(owner, currentKBLMapper, compareKBLMapper, settings)
    End Sub

    Public Overrides Function CompareObjectProperty(objProperty As PropertyInfo, currentObject As Object, compareObject As Object, excludeProperties As List(Of String)) As Boolean
        Dim currentProperty As Object = objProperty.GetValue(currentObject, Nothing)
        Dim compareProperty As Object = objProperty.GetValue(compareObject, Nothing)
        Dim compareValueRange As Value_range = TryCast(compareProperty, Value_range)
        Dim currentValueRange As Value_range = TryCast(currentProperty, Value_range)
        Dim currentUnitId As String = currentValueRange?.Unit_component
        Dim currentUnit As Unit = _currentKBLMapper?.GetUnit(currentUnitId)
        Dim compareUnitId As String = compareValueRange?.Unit_component
        Dim compareUnit As Unit = _compareKBLMapper.GetUnit(compareUnitId)

        If (currentProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(E3.Lib.Schema.Kbl.Default_dimension_specification.Dimension_value_range)
                    Dim current_a As String = String.Format("{0} {1}", Math.Round(currentValueRange.Minimum, 2), _currentKBLMapper.GetUnit(currentValueRange.Unit_component).Unit_name)
                    Dim current_b As String = String.Format("{0} {1}", Math.Round(currentValueRange.Maximum, 2), _currentKBLMapper.GetUnit(currentValueRange.Unit_component).Unit_name)

                    Dim compare_a As String = String.Format("{0} {1}", Math.Round(compareValueRange.Minimum, 2), _compareKBLMapper.KBLUnitMapper(compareValueRange.Unit_component).Unit_name)
                    Dim compare_b As String = String.Format("{0} {1}", Math.Round(compareValueRange.Maximum, 2), _compareKBLMapper.KBLUnitMapper(compareValueRange.Unit_component).Unit_name)

                    Dim currentDimValRange As String = String.Format("Min.: {0} / Max.: {1}", current_a, current_b)
                    Dim compareDimValRange As String = If(compareProperty Is Nothing, String.Empty, String.Format("Min.: {0} / Max.: {1}", compare_a, compare_b))

                    If (currentDimValRange <> compareDimValRange) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, compareDimValRange)
                    End If
                Case NameOf(E3.Lib.Schema.Kbl.Default_dimension_specification.External_references)
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
                Case NameOf(E3.Lib.Schema.Kbl.Default_dimension_specification.Tolerance_indication)
                    Dim currentTolerance As Tolerance = DirectCast(currentProperty, Tolerance)
                    Dim compareTolerance As Tolerance = If(compareProperty IsNot Nothing, DirectCast(compareProperty, Tolerance), Nothing)
                    Dim currentToleranceIndication As String = String.Empty
                    Dim compareToleranceIndication As String = String.Empty
                    Dim compareLowerLimitUnit As Unit = _compareKBLMapper.GetUnit(compareTolerance.Lower_limit.Unit_component)
                    Dim compareUpperLimitUnit As Unit = _compareKBLMapper.GetUnit(compareTolerance.Upper_limit.Unit_component)
                    Dim currentLowerLimitUnit As Unit = _currentKBLMapper.GetUnit(currentTolerance.Lower_limit.Unit_component)
                    Dim currentUpperLimitUnit As Unit = _currentKBLMapper.GetUnit(currentTolerance.Upper_limit.Unit_component)
                    Dim currentToleranceUnit As Unit = _currentKBLMapper.GetUnit(currentTolerance.Upper_limit.Unit_component)
                    Dim currentUpperLimitToleranceUnit As Unit = _currentKBLMapper.GetUnit(currentTolerance.Upper_limit.Unit_component)
                    Dim currentLowerLimitToleranceUnit As Unit = _currentKBLMapper.GetUnit(currentTolerance.Lower_limit.Unit_component)

                    If (currentTolerance.Lower_limit IsNot Nothing) AndAlso (currentTolerance.Upper_limit IsNot Nothing) Then
                        currentToleranceIndication = String.Format("{0}: {1} / {2}: {3}", ObjectPropertyNameStrings.LowerLimit, String.Format("{0} {1}", Math.Round(currentTolerance.Lower_limit.Value_component, 2), currentLowerLimitToleranceUnit.Unit_name), ObjectPropertyNameStrings.UpperLimit, String.Format("{0} {1}", Math.Round(currentTolerance.Upper_limit.Value_component, 2), currentUpperLimitToleranceUnit.Unit_name))
                    ElseIf (currentTolerance.Lower_limit IsNot Nothing) Then
                        currentToleranceIndication = String.Format("{0}: {1}", ObjectPropertyNameStrings.LowerLimit, String.Format("{0} {1}", Math.Round(currentTolerance.Lower_limit.Value_component, 2), currentLowerLimitToleranceUnit.Unit_name))
                    Else
                        currentToleranceIndication = String.Format("{0}: {1}", ObjectPropertyNameStrings.UpperLimit, String.Format("{0} {1}", Math.Round(currentTolerance.Upper_limit.Value_component, 2), currentUpperLimitToleranceUnit.Unit_name))
                    End If

                    If (compareTolerance IsNot Nothing) Then
                        If (compareTolerance.Lower_limit IsNot Nothing) AndAlso (compareTolerance.Upper_limit IsNot Nothing) Then
                            Dim c_a As String = String.Format("{0} {1}", Math.Round(compareTolerance.Lower_limit.Value_component, 2), _compareKBLMapper.GetUnit(compareTolerance.Lower_limit.Unit_component).Unit_name)
                            Dim c_b As String = String.Format("{0} {1}", Math.Round(compareTolerance.Upper_limit.Value_component, 2), _compareKBLMapper.GetUnit(compareTolerance.Upper_limit.Unit_component).Unit_name)
                            compareToleranceIndication = String.Format("{0}: {1} / {2}: {3}", ObjectPropertyNameStrings.LowerLimit, c_a, ObjectPropertyNameStrings.UpperLimit, c_b)
                        ElseIf (compareTolerance.Lower_limit IsNot Nothing) Then
                            Dim ll_a As String = String.Format("{0} {1}", Math.Round(compareTolerance.Lower_limit.Value_component, 2), compareLowerLimitUnit.Unit_name)
                            compareToleranceIndication = String.Format("{0}: {1}", ObjectPropertyNameStrings.LowerLimit, ll_a)
                        Else
                            Dim ul_a As String = String.Format("{0} {1}", Math.Round(compareTolerance.Upper_limit.Value_component, 2), compareUpperLimitUnit.Unit_name)
                            compareToleranceIndication = String.Format("{0}: {1}", ObjectPropertyNameStrings.UpperLimit, ul_a)
                        End If
                    End If

                    If (currentToleranceIndication <> compareToleranceIndication) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, compareToleranceIndication)
                    End If
                Case DefaultDimensionSpecificationPropertyName.Tolerance_type
                    If (compareProperty IsNot Nothing AndAlso currentProperty.ToString <> compareProperty.ToString) OrElse (compareProperty Is Nothing AndAlso currentProperty.ToString <> String.Empty) Then
                        MyBase.ChangedProperties.Add(objProperty.Name.ToString, compareProperty)
                    End If
            End Select
        ElseIf (compareProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(E3.Lib.Schema.Kbl.Default_dimension_specification.Dimension_value_range)
                    Dim min_s As String = String.Format("{0} {1}", Math.Round(DirectCast(compareProperty, Value_range).Minimum, 2), compareUnit.Unit_name)
                    Dim max_s As String = String.Format("{0} {1}", Math.Round(DirectCast(compareProperty, Value_range).Maximum, 2), compareUnit.Unit_name)
                    MyBase.ChangedProperties.Add(objProperty.Name.ToString, String.Format("Min.: {0} / Max.: {1}", min_s, max_s))
                Case NameOf(E3.Lib.Schema.Kbl.Default_dimension_specification.External_references)
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
                Case NameOf(E3.Lib.Schema.Kbl.Default_dimension_specification.Tolerance_indication)
                    Dim compareTolerance As Tolerance = DirectCast(compareProperty, Tolerance)
                    Dim compareToleranceIndication As String = String.Empty

                    If (compareTolerance.Lower_limit IsNot Nothing) AndAlso (compareTolerance.Upper_limit IsNot Nothing) Then
                        Dim compare_ll As String = String.Format("{0} {1}", Math.Round(compareTolerance.Lower_limit.Value_component, 2), _compareKBLMapper.GetUnit(compareTolerance.Lower_limit.Unit_component).Unit_name)
                        Dim compare_ul As String = String.Format("{0} {1}", Math.Round(compareTolerance.Upper_limit.Value_component, 2), _compareKBLMapper.GetUnit(compareTolerance.Upper_limit.Unit_component).Unit_name)
                        compareToleranceIndication = String.Format("{0}: {1} / {2}: {3}", ObjectPropertyNameStrings.LowerLimit, compare_ll, ObjectPropertyNameStrings.UpperLimit, compare_ul)
                    ElseIf (compareTolerance.Lower_limit IsNot Nothing) Then
                        compareToleranceIndication = String.Format("{0}: {1}", ObjectPropertyNameStrings.LowerLimit, String.Format("{0} {1}", Math.Round(compareTolerance.Lower_limit.Value_component, 2), _compareKBLMapper.GetUnit(compareTolerance.Lower_limit.Unit_component).Unit_name))
                    Else
                        compareToleranceIndication = String.Format("{0}: {1}", ObjectPropertyNameStrings.UpperLimit, String.Format("{0} {1}", Math.Round(compareTolerance.Upper_limit.Value_component, 2), _compareKBLMapper.GetUnit(compareTolerance.Upper_limit.Unit_component).Unit_name))
                    End If

                    MyBase.ChangedProperties.Add(objProperty.Name, compareToleranceIndication)
                Case NameOf(E3.Lib.Schema.Kbl.Default_dimension_specification.Tolerance_type)
                    MyBase.ChangedProperties.Add(objProperty.Name.ToString, compareProperty)
            End Select
        End If

        Return False
    End Function
End Class