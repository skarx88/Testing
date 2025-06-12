Imports System.ComponentModel
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.Lib.Converter.Unit

Public Class CalculatedWeightRow
    Implements INotifyPropertyChanged

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

    Private _listObject As Object
    Private _id As String
    Private _sourceString As String = Nothing
    Private _source As WeightCalculator.Source
    Private _kblMass As Nullable(Of Double)
    Private _conductor As Nullable(Of Double)
    Private _total As Nullable(Of Double)
    Private _csaSqMm As Nullable(Of Double)
    Private _error As Exception
    Private _length As NumericValue
    Private _kblMapper As KBLMapper
    Private _currentCalculator As WeightCalculator
    Private _coresDic As New Dictionary(Of String, CalculatedWeightRow)
    Private _coresList As New BindingList(Of CalculatedWeightRow)()
    Private _isUpdating As Boolean = False
    Private _isChangingCsaSqMm As Boolean
    Private _isChangingLength As Boolean
    Private _isCable As Boolean = False
    Private _originalLengthValue As Nullable(Of Double) = Nothing
    Private _lengthDeltaValue As Nullable(Of Double) = Nothing
    Private _resultingLength As NumericValue = Nothing

    Friend Sub New(listObject As Object, id As String, kblMapper As KBLMapper, Optional source As WeightCalculator.Source = Nothing, Optional csaSqMm As Nullable(Of Double) = Nothing, Optional conductor As Nullable(Of Double) = Nothing, Optional total As Nullable(Of Double) = Nothing, Optional kblMass As Nullable(Of Double) = Nothing, Optional length As NumericValue = Nothing)
        _listObject = listObject
        _id = id
        _kblMapper = kblMapper

        _source = source
        _sourceString = GetSourceString(source)

        If csaSqMm.HasValue Then _csaSqMm = csaSqMm.Value
        If conductor.HasValue Then _conductor = conductor.Value
        If total.HasValue Then _total = total.Value
        If kblMass.HasValue Then _kblMass = kblMass.Value

        SetInitialLength(length)

        _coresList.AllowNew = False
        _coresList.AllowRemove = False
    End Sub

    Public Sub New(cable As Special_wire_occurrence, kblMapper As KBLMapper, useLength As NumericValue)
        Me.New(cable, GetIdString(cable), kblMapper)
        _isCable = True
        SetInitialLength(OrGetLengthFromObjectIfEmtpy(useLength))
        Me.Update()
    End Sub

    Public Sub New(cable As CorePackage, kblMapper As KBLMapper, useLength As NumericValue)
        Me.New(cable.OwningCable, GetIdString(cable.OwningCable), kblMapper)
        _isCable = True
        SetInitialLength(OrGetLengthFromObjectIfEmtpy(useLength))
        Me.Update()
    End Sub

    Public Sub New(segmentedWire As SegmentedWire)
        Me.New(segmentedWire.Object, segmentedWire.Mapper, segmentedWire.Length.Extract(segmentedWire.Mapper))
    End Sub

    Private Sub New(wire As Wire_occurrence, kblMapper As KBLMapper, Optional useLength As NumericValue = Nothing)
        Me.New(wire, wire.Wire_number, kblMapper)
        SetInitialLength(OrGetLengthFromObjectIfEmtpy(useLength))
        Me.Update()
    End Sub

    Public Sub New(coreOcc As Core_occurrence, kblMapper As KBLMapper, Optional useLength As NumericValue = Nothing)
        Me.New(coreOcc, GetIdString(coreOcc), kblMapper)
        SetInitialLength(OrGetLengthFromObjectIfEmtpy(useLength))
        Me.Update()
    End Sub

    Public Sub New(segCore As SegmentedCore)
        Me.New(segCore.Object, segCore.Mapper, segCore.Length.Extract(segCore.Mapper))
    End Sub

    Private Sub ExecuteWithAutoUpdateEnabled(action As Action, autoUpdateEnabledWhileInAction As Boolean)
        Dim auEnabled As Boolean = AutoUpdateEnabled
        AutoUpdateEnabled = False
        action.Invoke()
        AutoUpdateEnabled = auEnabled
    End Sub

    Private Function CalculateWeight(useLengthForCalculation As NumericValue, overrideCoreLength As Boolean, Optional overrideCsaSqMm As Nullable(Of Double) = Nothing) As WeightCalculator
        Dim WeightCalc As New WeightCalculator(_kblMapper)
        With WeightCalc
            .OverrideCoreLength = overrideCoreLength
            .UntypedCalculate(_listObject, useLengthForCalculation, overrideCsaSqMm)
        End With
        Return WeightCalc
    End Function

    Private Sub SetInitialLength(length As NumericValue)
        _length = length
        _resultingLength = CreateNewNumericValueWithCurrentLengthUnit(GetCurrentResultingLengthValue)
        UpdateResultingLength()
        If length IsNot Nothing Then
            _originalLengthValue = length.Value
        End If
    End Sub

    Private Function OrGetLengthFromObjectIfEmtpy(length As NumericValue) As NumericValue
        If TypeOf _listObject Is Special_wire_occurrence Then
            If length Is Nothing Then
                Dim cable As Special_wire_occurrence = CType(_listObject, Special_wire_occurrence)
                Dim cablelength As Numerical_value = cable.GetLength
                length = cablelength.Extract(_kblMapper)
            End If
        ElseIf TypeOf _listObject Is Wire_occurrence Then
            If length Is Nothing Then
                Dim wire As Wire_occurrence = CType(_listObject, Wire_occurrence)
                Dim wireLength As Numerical_value = wire.GetLength
                length = wireLength.Extract(_kblMapper)
            End If
        ElseIf TypeOf _listObject Is Core_occurrence Then
            If length Is Nothing Then
                Dim core As Core_occurrence = CType(_listObject, Core_occurrence)
                Dim coreLength As Numerical_value = core.GetLength
                length = coreLength.Extract(_kblMapper)
            End If
        Else
            Throw New NotImplementedException(String.Format("Un-implemented listObject type ""{0}""", _listObject.GetType.Name))
        End If
        Return length
    End Function

    Private Function GetKblMass(calculator As WeightCalculator, length As NumericValue) As Nullable(Of Double)
        Dim weightInGram As Nullable(Of Double) = UnitConverter.Convert(calculator.OriginalWeight.Value, calculator.OriginalWeight.Unit, CalcUnit.Gram, True)
        If weightInGram IsNot Nothing Then
            Dim lengthUnitFromWeight As CalcUnit = Nothing
            If calculator.OriginalWeight.LengthUnit IsNot Nothing Then
                If calculator.OriginalWeight.LengthUnit.UnitDimension = CalcUnit.UDimensionEnum.None Then
                    lengthUnitFromWeight = calculator.OriginalWeight.LengthUnit
                End If
            Else
                lengthUnitFromWeight = New CalcUnit(CalcUnit.UnitEnum.Metre) ' use the default unit for weight per length calculation
            End If

            Dim calculatedMass As Nullable(Of Double) = Nothing
            Dim weightUnitsToProbe As New Queue(Of CalcUnit)(CalcUnit.AllPossible(CalcUnit.UnitEnum.Metre))
            Do
                If lengthUnitFromWeight Is Nothing Then
                    'HINT: probe every possible known unit (weightUnitsToProbe) length with min/max
                    If weightUnitsToProbe.Count > 0 Then
                        lengthUnitFromWeight = weightUnitsToProbe.Dequeue()
                    Else
                        Return Nothing
                    End If
                End If

                Dim calcResult As Double
                If TryCalculateWeightInWeightLengthUnit(weightInGram.Value, length, lengthUnitFromWeight, calculator.Calculated.CsaSqMm, calcResult) Then
                    calculatedMass = calcResult
                End If
                lengthUnitFromWeight = Nothing
            Loop Until calculatedMass.HasValue

            Return calculatedMass
        End If
        Return Nothing
    End Function

    Private Function TryCalculateWeightInWeightLengthUnit(weightInGram As Double, length As NumericValue, lengthUnitFromWeight As CalcUnit, CsaSqMm As Nullable(Of Double), ByRef calcKblMass As Double) As Boolean
        Dim lengthInWeightLengthUnit As Nullable(Of Double)
        If length IsNot Nothing Then
            lengthInWeightLengthUnit = UnitConverter.Convert(length.Value, length.Unit, lengthUnitFromWeight, True)
            If lengthInWeightLengthUnit IsNot Nothing Then
                If IsBetweenMinMax(weightInGram, CsaSqMm) Then
                    calcKblMass = CDbl(weightInGram * lengthInWeightLengthUnit)
                    Return True
                End If
            End If
        End If
        Return False
    End Function

    Private Function IsBetweenMinMax(weightInGram As Double, csaSqMm As Nullable(Of Double)) As Boolean
        If csaSqMm.HasValue Then
            Dim min As Double = CDbl(9 * csaSqMm) - 1
            Dim max As Double = CDbl(11 * csaSqMm) + 15
            Return weightInGram >= min AndAlso weightInGram <= max
        End If
        Return False
    End Function

    Private Function GetSourceString(source As WeightCalculator.Source) As String
        If source IsNot Nothing Then
            Return String.Join(";", source.Value.GetFlags().Select(Function(flg) String.Format(WeightCalculationSourceStrings.ResourceManager.GetString(flg.ToString), source.Info)))
        End If
        Return Nothing
    End Function

    Private Function GetByPartOrWireType(part As String, wireType As String) As WeightSettings.Weight
        Dim weight As WeightSettings.Weight = My.Application.MainForm.WeightSettings.Weights.GetByPartNumber(part).SingleOrDefault()
        If weight Is Nothing Then
            weight = My.Application.MainForm.WeightSettings.Weights.GetByWireType(wireType).SingleOrDefault()
        End If
        Return weight
    End Function

    Friend Shared Function GetIdString(cable As Special_wire_occurrence) As String
        Return String.Format(WeightCalculationPrefixStrings.Cable, cable.Special_wire_id)
    End Function

    Friend Shared Function GetIdString(coreOcc As Core_occurrence) As String
        Return String.Format(WeightCalculationPrefixStrings.Core, coreOcc.Wire_number)
    End Function

    Private Sub Update(useLength As NumericValue, overrideCoreLength As Boolean, useCsa As Nullable(Of Double))
        If Not _isUpdating Then
            _isUpdating = True
            SetCurrentCalculator(useLength, overrideCoreLength, useCsa)
            UpdateCurrentCalculated(useLength)
            UpdateCores()
            _isUpdating = False
        End If
    End Sub

    Private Sub Update()
        Me.Update(Me.ResultingLength, True, _csaSqMm)
    End Sub

    Private Sub SetCurrentCalculator(useLength As NumericValue, overrideCoreLength As Boolean, useCsa As Nullable(Of Double))
        _currentCalculator = CalculateWeight(useLength, overrideCoreLength, useCsa)
    End Sub

    Private Sub UpdateCores()
        ' retrieve the calculated cores if it's an wire
        Dim oldCoresDic As New Dictionary(Of String, CalculatedWeightRow)
        _coresDic.ForEach(Sub(Kv) oldCoresDic.Add(Kv.Key, Kv.Value))
        _coresList.Clear()
        _coresDic.Clear()

        With _currentCalculator
            If .Calculated.CalculatedMaterialSpecCores.Count > 0 Then                            ' the cable weight was calculated by the sum of the cores (by materialspec)
                For Each coreKV As KeyValuePair(Of String, WeightCalculator.ICalculatedWeight) In .Calculated.CalculatedMaterialSpecCores
                    Dim coreOcc As Core_occurrence = _kblMapper.GetOccurrenceObject(Of Core_occurrence)(coreKV.Key)
                    With coreKV.Value
                        Dim avMass As NumericValue = coreOcc.GetAverageWeight(_kblMapper)
                        Dim newRow As New CalculatedWeightRow(coreOcc, GetIdString(coreOcc), _kblMapper, .Source, .CsaSqMm, .Conductor, .Total, avMass.Value, .Length)
                        If oldCoresDic.ContainsKey(coreKV.Key) Then
                            newRow.CopyPropertiesTo(oldCoresDic(coreKV.Key))                     ' Update only the properties if already available
                            newRow = oldCoresDic(coreKV.Key)
                        End If

                        _coresDic.Add(coreKV.Key, newRow)
                        _coresList.Add(newRow)
                    End With
                Next
            ElseIf TypeOf _listObject Is Special_wire_occurrence Then                  ' the cable weigth was found by lookUp (no MaterialSpec and was calculated by the lookUp data) -> we have to calculate the weight for the cores for viewing
                For Each core As Core_occurrence In CType(_listObject, Special_wire_occurrence).Core_Occurrence
                    Dim newRow As New CalculatedWeightRow(core, _kblMapper)
                    If _coresDic.ContainsKey(core.SystemId) Then
                        newRow.CopyPropertiesTo(oldCoresDic(core.SystemId))            ' Update only the properties if already available
                        newRow = oldCoresDic(core.SystemId)
                    End If

                    _coresDic.Add(core.SystemId, newRow)
                    _coresList.Add(newRow)
                Next
            End If
        End With

        OnPropertyChanged("Cores")
    End Sub

    Private Sub UpdateCurrentCalculated(lengthForKblMass As NumericValue)
        With _currentCalculator.Calculated
            Me.Conductor = .Conductor
            Me.Total = .Total
            Me.Source = .Source
            Me.SourceString = GetSourceString(.Source)
            Me.KblMass = GetKblMass(_currentCalculator, lengthForKblMass)
            Me.CsaSqMm = .CsaSqMm
            _error = .Error
        End With
    End Sub

    Private Function GetCurrentResultingLengthValue() As Double
        Return If(Me._length IsNot Nothing, _length.Value, 0) + If(Me._lengthDeltaValue IsNot Nothing, Me._lengthDeltaValue.Value, 0)
    End Function

    Private Sub UpdateResultingLength()
        Me.ResultingLengthValue = GetCurrentResultingLengthValue()
    End Sub

    Public Function GetErrorMessage() As String
        If TypeOf _error Is LengthNotFoundException Then
            Dim lnF As LengthNotFoundException = CType(_error, LengthNotFoundException)
            Return String.Format(ErrorStrings.WeightCalc_LengthNotFound, lnF.DefaultLength)
        ElseIf TypeOf _error Is MaterialFieldNotFoundException Then
            Dim mfNf As MaterialFieldNotFoundException = CType(_error, MaterialFieldNotFoundException)
            With mfNf
                If TypeOf .Instance Is Special_wire_occurrence Then
                    Return String.Format(ErrorStrings.WeightCalc_MaterialSpecFieldNotFoundInCable, .FieldName)
                ElseIf TypeOf .Instance Is Wire_occurrence Then
                    Return String.Format(ErrorStrings.WeightCalc_MaterialSpecFieldNotFoundInWire, .FieldName)
                ElseIf TypeOf .Instance Is Core_occurrence Then
                    Return String.Format(ErrorStrings.WeightCalc_MaterialSpecFieldNotFoundInCore, .FieldName)
                Else
                    Throw New NotImplementedException(String.Format("Unimplemented instance type ""{0}"" for ""{1}""", .Instance.GetType.Name, GetType(MaterialFieldNotFoundException).Name))
                End If
            End With
        ElseIf TypeOf _error Is RegexException Then
            Dim regExc As RegexException = CType(_error, RegexException)
            Dim refObj As WeightSettings.MaterialSpec = CType(regExc.ReferenceObject, WeightSettings.MaterialSpec)
            If refObj.SpecRegEx IsNot Nothing Then
                Return String.Format(ErrorStrings.WeightCalc_RegexPatternError, refObj.SpecRegEx, refObj.Description)
            Else
                Return String.Format(ErrorStrings.WeightCalc_RegexNothingError, refObj.Description)
            End If
        End If
        Return String.Empty
    End Function

    Private Sub OnPropertyChanged(propName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propName))
    End Sub

    Private Function CreateNewNumericValueWithCurrentLengthUnit(value As Nullable(Of Double)) As NumericValue
        Dim newLength As New NumericValue
        newLength.Unit = If(Me.Length IsNot Nothing, Me.Length.Unit, CalcUnit.MilliMetre)
        If value.HasValue Then
            newLength.Value = value.Value
        Else
            newLength.Value = 0
        End If
        Return newLength
    End Function

    Friend ReadOnly Property Type As WeightRowObjectType
        Get
            If TypeOf Me.ListObject Is Core_occurrence Then
                Return WeightRowObjectType.Core
            ElseIf TypeOf Me.ListObject Is Wire_occurrence Then
                Return WeightRowObjectType.Wire
            ElseIf TypeOf Me.ListObject Is Special_wire_occurrence Then
                Return WeightRowObjectType.Cable
            Else
                Return WeightRowObjectType.Unkown
            End If
        End Get
    End Property

    Public Sub CopyPropertiesTo(target As CalculatedWeightRow)
        target._id = _id
        target._source = _source
        target._sourceString = _sourceString
        target._csaSqMm = _csaSqMm
        target._conductor = _conductor
        target._total = _total
        target._kblMass = _kblMass
        target._length = _length
        target._originalLengthValue = _originalLengthValue
        target._resultingLength = _resultingLength
        target._lengthDeltaValue = _lengthDeltaValue
    End Sub

    Private Enum CalculationType
        ''' <summary>
        ''' Uses Length-Property to calculate weight
        ''' </summary>
        ''' <remarks></remarks>
        [Default] = 0
        ''' <summary>
        ''' Avoids length property (nothing) for weight calculation: when calculating the cores, the length of each core will be used when length property is empty
        ''' </summary>
        ''' <remarks></remarks>
        EmptyLength = 1
    End Enum

    Friend Enum WeightRowObjectType
        Unkown = 0
        Wire = 1
        Core = 2
        Cable = 3
    End Enum

End Class