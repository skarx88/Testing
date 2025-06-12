Imports Zuken.E3.HarnessAnalyzer.Settings.WeightSettings
Imports Zuken.E3.HarnessAnalyzer.WireResistanceCalculator

Public Class WireResistanceCalculator

    Private _mapper As KBLMapper
    Private _name As String
    Private _csa As Double
    Private _length As Double
    Private _isNameSet As Boolean
    Private _isCsaOverridden As Boolean = False
    Private _isLengthOverridden As Boolean = False

    Public Sub New(mapper As KBLMapper)
        _mapper = mapper
    End Sub

    Private Sub UpdateResistanceCore(wireLength As Double, csa As Double, resistivity As Double, Optional temperature As Temperature = Nothing)
        Me._Resistance = 0
        If wireLength <> 0 AndAlso csa <> 0 Then
            Me._Resistance = (resistivity * 0.00000001) * (wireLength / 1000) / (csa * 0.000001)
            If (temperature IsNot Nothing) Then
                Me._Resistance = Me.Resistance * (1 + temperature.Coefficient * 0.001 * ((CInt(temperature.Value) - 20)))
            End If
        End If
    End Sub

    Public Sub UpdateResistance(resistivity As Double, Optional temperature As Temperature = Nothing)
        UpdateResistanceCore(Me.Length, Me.Csa, resistivity, temperature)
    End Sub

    Public Sub UpdateResistance(material As MaterialSpec, temperatureValue As Double)
        UpdateResistance(material.Resistivity, New Temperature(temperatureValue, material.TemperatureCoefficient))
    End Sub

    Public Sub UpdateValues(coreOcc As Core_occurrence, Optional overrideName As String = Nothing, Optional overrideCsa As Nullable(Of Double) = Nothing, Optional overrideLength As Nullable(Of Double) = Nothing)
        If _isCsaOverridden OrElse overrideCsa IsNot Nothing Then
            _csa = overrideCsa.Value
            _isCsaOverridden = True
        Else
            _csa = If(GetCsa(coreOcc, _mapper), 0)
        End If

        If _isLengthOverridden OrElse overrideLength IsNot Nothing Then
            _length = overrideLength.Value
            _isLengthOverridden = True
        Else
            _length = GetLength(coreOcc, _mapper)
        End If

        If overrideName IsNot Nothing Then
            _name = overrideName
            _isNameSet = True
        ElseIf Not _isNameSet Then
            _name = coreOcc.Wire_number
        End If
    End Sub

    Public Sub UpdateValues(wireOcc As Wire_occurrence, Optional overrideName As String = Nothing, Optional overrideCsa As Nullable(Of Double) = Nothing, Optional overrideLength As Nullable(Of Double) = Nothing)
        If _isCsaOverridden OrElse overrideCsa IsNot Nothing Then
            _csa = overrideCsa.Value
            _isCsaOverridden = True
        Else
            _csa = If(GetCsa(wireOcc, _mapper), 0)
        End If

        If _isLengthOverridden OrElse overrideLength IsNot Nothing Then
            _length = overrideLength.Value
            _isLengthOverridden = True
        Else
            _length = GetLength(wireOcc, _mapper)
        End If

        If overrideName IsNot Nothing Then
            _name = overrideName
            _isNameSet = True
        ElseIf Not _isNameSet Then
            _name = wireOcc.Wire_number
        End If
    End Sub

    Public Sub UpdateValues(cable As Special_wire_occurrence, Optional overrideName As String = Nothing, Optional overrideCsa As Nullable(Of Double) = Nothing, Optional overrideLength As Nullable(Of Double) = Nothing)
        If _isCsaOverridden OrElse overrideCsa IsNot Nothing Then
            _csa = overrideCsa.Value
            _isCsaOverridden = True
        Else
            _csa = GetCsa(cable)
        End If

        If _isLengthOverridden OrElse overrideLength IsNot Nothing Then
            _length = overrideLength.Value
            _isLengthOverridden = True
        Else
            _length = GetLength(cable, _mapper)
        End If

        If overrideName IsNot Nothing Then
            _name = overrideName
            _isNameSet = True
        ElseIf Not _isNameSet Then
            _name = cable.GetGeneralWire(_mapper).Abbreviation
        End If
    End Sub

    Public Property Csa As Double
        Get
            Return _csa
        End Get
        Set(value As Double)
            _csa = value
            If Not Double.IsNaN(value) Then
                _isLengthOverridden = True
            Else
                _isLengthOverridden = False
            End If
        End Set
    End Property

    Public Property Length As Double
        Get
            Return _length
        End Get
        Set(value As Double)
            _length = value
            If Not Double.IsNaN(value) Then
                _isLengthOverridden = True
            Else
                _isLengthOverridden = False
            End If
        End Set
    End Property

    Public Property Name As String
        Get
            Return _name
        End Get
        Set(value As String)
            _name = value
            If value IsNot Nothing Then
                _isNameSet = True
            Else
                _isNameSet = False
            End If
        End Set
    End Property

    Public ReadOnly Property Resistance As Double

    Friend Shared Function GetLength(cable As Special_wire_occurrence, mapper As KBLMapper) As Double
        Return GetLengthInternal(cable.Core_Occurrence.SelectMany(Function(core) core.Length_Information), mapper)
    End Function

    Friend Shared Function GetLength(coreOcc As Core_occurrence, mapper As KBLMapper) As Double
        Return GetLengthInternal(coreOcc.Length_Information, mapper)
    End Function

    Friend Shared Function GetLength(wireOcc As Wire_occurrence, mapper As KBLMapper) As Double
        Return GetLengthInternal(wireOcc.Length_Information, mapper)
    End Function

    Private Shared Function GetLengthInternal(lengthInfos As IEnumerable(Of Wire_length), mapper As KBLMapper) As Double
        Dim length As Double
        For Each wireLength As Wire_length In lengthInfos
            If (wireLength.Length_type.ToLower = My.Application.MainForm.GeneralSettings.DefaultWireLengthType.ToLower) Then
                length = Math.Round(wireLength.Length_value.Value_component, 2)

                Dim unit As Unit = mapper.KBLUnitMapper(wireLength.Length_value.Unit_component)
                If (unit.Si_unit_name = SI_unit_name.metre) AndAlso (Not unit.Si_prefixSpecified) Then
                    length *= 1000
                End If

                If (unit.Si_unit_name = SI_unit_name.metre) AndAlso (unit.Si_prefixSpecified AndAlso unit.Si_prefix = SI_prefix.centi) Then
                    length *= 10
                End If

                Exit For
            End If
        Next
        Return length
    End Function

    Private Function GetCsa(cable As Special_wire_occurrence) As Double
        Dim csa As Double = 0
        If _mapper.KblPartMapper.ContainsKey(cable.Part) Then
            Dim generalWire As General_wire = cable.GetGeneralWire(_mapper)
            If (generalWire.Cross_section_area IsNot Nothing) Then
                csa = Math.Round(generalWire.Cross_section_area.Value_component, 2)

                Dim unit As Unit = _mapper.KBLUnitMapper(generalWire.Cross_section_area.Unit_component)
                If (unit.Si_unit_name = SI_unit_name.metre) AndAlso (unit.Si_dimensionSpecified AndAlso unit.Si_dimension = Unit_dimension.square) AndAlso (Not unit.Si_prefixSpecified) Then
                    csa *= 1000
                End If
                If (unit.Si_unit_name = SI_unit_name.metre) AndAlso (unit.Si_dimensionSpecified AndAlso unit.Si_dimension = Unit_dimension.square) AndAlso (unit.Si_prefixSpecified AndAlso unit.Si_prefix = SI_prefix.centi) Then
                    csa *= 10
                End If
            End If
        End If
        Return csa
    End Function

    Friend Shared Function GetCsa(wireOcc As Wire_occurrence, mapper As KBLMapper) As Nullable(Of Double)
        Dim csa As Nullable(Of Double) = Nothing
        If mapper.KblPartMapper.ContainsKey(wireOcc.Part) Then
            Dim generalWire As General_wire = DirectCast(mapper.KblPartMapper(wireOcc.Part), General_wire)
            If (generalWire.Cross_section_area IsNot Nothing) Then
                csa = Math.Round(generalWire.Cross_section_area.Value_component, 2)

                Dim unit As Unit = mapper.KBLUnitMapper(generalWire.Cross_section_area.Unit_component)
                If (unit.Si_unit_name = SI_unit_name.metre) AndAlso (unit.Si_dimensionSpecified AndAlso unit.Si_dimension = Unit_dimension.square) AndAlso (Not unit.Si_prefixSpecified) Then
                    csa *= 1000
                End If

                If (unit.Si_unit_name = SI_unit_name.metre) AndAlso (unit.Si_dimensionSpecified AndAlso unit.Si_dimension = Unit_dimension.square) AndAlso (unit.Si_prefixSpecified AndAlso unit.Si_prefix = SI_prefix.centi) Then
                    csa *= 10
                End If
            End If
        End If
        Return csa
    End Function

    Friend Shared Function GetCsa(coreOcc As Core_occurrence, mapper As KBLMapper) As Nullable(Of Double)
        Dim csa As Nullable(Of Double) = Nothing
        If mapper.KblPartMapper.ContainsKey(coreOcc.Part) Then
            Dim core As Core = DirectCast(mapper.KblPartMapper(coreOcc.Part), Core)
            If (core.Cross_section_area IsNot Nothing) Then
                csa = Math.Round(core.Cross_section_area.Value_component, 2)

                Dim unit As Unit = mapper.KBLUnitMapper(core.Cross_section_area.Unit_component)
                If (unit.Si_unit_name = SI_unit_name.metre) AndAlso (unit.Si_dimensionSpecified AndAlso unit.Si_dimension = Unit_dimension.square) AndAlso (Not unit.Si_prefixSpecified) Then
                    csa *= 1000
                End If
                If (unit.Si_unit_name = SI_unit_name.metre) AndAlso (unit.Si_dimensionSpecified AndAlso unit.Si_dimension = Unit_dimension.square) AndAlso (unit.Si_prefixSpecified AndAlso unit.Si_prefix = SI_prefix.centi) Then
                    csa *= 10
                End If
            End If
        End If
        Return csa
    End Function

    Public Class Temperature
        Implements ICloneable

        Public Sub New(value As Double, coefficient As Double)
            Me.Value = value
            Me.Coefficient = coefficient
        End Sub

        Public Property Coefficient As Double = 0
        Public Property Value As Double = 0

        Public ReadOnly Property [Empty] As Temperature
            Get
                Static myTemp As New Temperature(0, 0)
                Return myTemp
            End Get
        End Property

        Protected Overridable Function IClonable_Clone() As Object Implements ICloneable.Clone
            Return New Temperature(Me.Value, Me.Coefficient)
        End Function

        Public Function Clone() As Temperature
            Return CType(Me.IClonable_Clone, Temperature)
        End Function

        Public Shared ReadOnly Property [Default] As Temperature
            Get
                Static myDef As New Temperature(20, 0)
                Return myDef
            End Get
        End Property

    End Class

    ''' <summary>
    ''' Override Object in calculation and/or it's values (name,csa and length)
    ''' </summary>
    Friend Class OverrideObject

        Public Sub New(obj As Object, name As String, Optional csa As Nullable(Of Double) = Nothing, Optional length As Nullable(Of Double) = Nothing)
            Me.Object = obj
            Me.Name = name
            Me.Csa = csa
            Me.Length = length
        End Sub

        Public Property [Object] As Object
        Public Property Name As String
        Public Property Csa As Nullable(Of Double)
        Public Property Length As Nullable(Of Double)

        ''' <summary>
        '''  Maps the core to its cable but with the csa, length and name (core-number)
        ''' </summary>
        ''' <param name="core"></param>
        ''' <param name="mapper"></param>
        ''' <returns></returns>
        Public Shared Function OverrideWithCable(core As Core_occurrence, mapper As KBLMapper) As OverrideObject
            Return New OverrideObject(core.GetCable(mapper), core.Wire_number, WireResistanceCalculator.GetCsa(core, mapper), WireResistanceCalculator.GetLength(core, mapper))
        End Function

    End Class

End Class

Public Class WireResistanceCalcCollection
    Implements IEnumerable(Of WireResistanceCalculator)

    Private _kblMappers As KBLMapper()
    Private _list As New List(Of WireResistanceCalculator)

    Public Sub New(kblMappers As IEnumerable(Of KBLMapper), objects As IEnumerable(Of Object))
        Me.New(kblMappers)
        Me.AddNewRange(objects)
    End Sub

    Public Sub New(kblMapper As KBLMapper, objects As IEnumerable(Of Object))
        Me.New(kblMapper)
    End Sub

    Public Sub New(kblMappers As IEnumerable(Of KBLMapper))
        _kblMappers = kblMappers.ToArray
    End Sub

    Public Sub New(kblMapper As KBLMapper)
        Me.New(New KBLMapper() {kblMapper})
    End Sub

    Public Function AddNewRange(objects As IEnumerable(Of Object)) As WireResistanceCalculator()
        Dim result As New List(Of WireResistanceCalculator)
        For Each obj As Object In objects
            result.Add(AddNewAutoInternal(obj))
        Next
        Return result.ToArray
    End Function

    Private Function AddNewAutoInternal([object] As Object, Optional overrideName As String = Nothing, Optional overrideCsa As Nullable(Of Double) = Nothing, Optional overrideLength As Nullable(Of Double) = Nothing) As WireResistanceCalculator
        If TypeOf [object] Is Core_occurrence Then
            Return Me.AddNew(CType([object], Core_occurrence), overrideName, overrideCsa)
        ElseIf TypeOf [object] Is Wire_occurrence Then
            Return Me.AddNew(CType([object], Wire_occurrence), overrideName, overrideCsa)
        ElseIf TypeOf [object] Is Special_wire_occurrence Then
            Return Me.AddNew(CType([object], Special_wire_occurrence), overrideName, overrideCsa, overrideLength)
        ElseIf TypeOf [object] Is OverrideObject Then
            Return AddNewAutoInternal(CType([object], OverrideObject).Object, CType([object], OverrideObject).Name, CType([object], OverrideObject).Csa, CType([object], OverrideObject).Length)
        Else
            Throw New ArgumentException(String.Format("Invalid object-type ""{0}"" in argument (only {1} and {2} is supported)", [object].GetType.Name, GetType(Core_occurrence).Name, GetType(Wire_occurrence).Name), NameOf([object]))
        End If
    End Function

    Public Sub Clear()
        _list.Clear()
    End Sub

    Public Function AddNew(coreOcc As Core_occurrence, Optional overrideName As String = Nothing, Optional overrideCsa As Nullable(Of Double) = Nothing, Optional override_length As Nullable(Of Double) = Nothing) As WireResistanceCalculator
        Dim newCalc As WireResistanceCalculator = CreateNew(coreOcc)
        newCalc.UpdateValues(coreOcc, overrideName, overrideCsa, override_length)
        _list.Add(newCalc)
        AfterAdd(newCalc)
        Return newCalc
    End Function

    Public Function AddNew(wireOcc As Wire_occurrence, Optional overrideName As String = Nothing, Optional overrideCsa As Nullable(Of Double) = Nothing, Optional overridelength As Nullable(Of Double) = Nothing) As WireResistanceCalculator
        Dim newCalc As WireResistanceCalculator = CreateNew(wireOcc)
        newCalc.UpdateValues(wireOcc, overrideName, overrideCsa, overridelength)
        _list.Add(newCalc)
        AfterAdd(newCalc)
        Return newCalc
    End Function

    Public Function AddNew(cable As Special_wire_occurrence, Optional overrideName As String = Nothing, Optional overrideCsa As Nullable(Of Double) = Nothing, Optional overridelength As Nullable(Of Double) = Nothing) As WireResistanceCalculator
        Dim newCalc As WireResistanceCalculator = CreateNew(cable)
        newCalc.UpdateValues(cable, overrideName, overrideCsa, overridelength)
        _list.Add(newCalc)
        AfterAdd(newCalc)
        Return newCalc
    End Function

    Public Function AddNew(coreOcc As Core_occurrence, resistivity As Double, Optional temperature As WireResistanceCalculator.Temperature = Nothing) As WireResistanceCalculator
        Dim added As WireResistanceCalculator = Me.AddNew(coreOcc)
        added.UpdateResistance(resistivity, temperature)
        Return added
    End Function

    Public Function AddNew(wireOcc As Wire_occurrence, resistivity As Double, Optional temperature As WireResistanceCalculator.Temperature = Nothing) As WireResistanceCalculator
        Dim added As WireResistanceCalculator = Me.AddNew(wireOcc)
        added.UpdateResistance(resistivity, temperature)
        Return added
    End Function

    Public Function AddNew(coreOcc As Core_occurrence, material As MaterialSpec, temperatureValue As Double) As WireResistanceCalculator
        Dim added As WireResistanceCalculator = Me.AddNew(coreOcc)
        added.UpdateResistance(material, temperatureValue)
        Return added
    End Function

    Public Function AddNew(wireOcc As Wire_occurrence, material As MaterialSpec, temperatureValue As Double) As WireResistanceCalculator
        Dim added As WireResistanceCalculator = Me.AddNew(wireOcc)
        added.UpdateResistance(material, temperatureValue)
        Return added
    End Function

    Private Sub AfterAdd(calc As WireResistanceCalculator)
        CType(Csa, IInternalSumInfo).AddToSum(calc.Csa)
        CType(Length, IInternalSumInfo).AddToSum(calc.Length)
        _Names.Add(calc.Name)
        _ResistanceSum += calc.Resistance
    End Sub

    Private Function CreateNew(core As Core_occurrence) As WireResistanceCalculator
        Return New WireResistanceCalculator(_kblMappers.Where(Function(mp) mp.KBLCoreList.Contains(core)).FirstOrDefault)
    End Function

    Private Function CreateNew(wire As Wire_occurrence) As WireResistanceCalculator
        Return New WireResistanceCalculator(_kblMappers.Where(Function(mp) mp.KBLWireList.Contains(wire)).FirstOrDefault)
    End Function

    Private Function CreateNew(cable As Special_wire_occurrence) As WireResistanceCalculator
        Return New WireResistanceCalculator(_kblMappers.Where(Function(mp) mp.KBLCableList.Contains(cable)).FirstOrDefault)
    End Function

    Public Sub UpdateResistance(resistivity As Double, Optional temperature As WireResistanceCalculator.Temperature = Nothing)
        Me._ResistanceSum = 0
        For Each calc As WireResistanceCalculator In _list
            calc.UpdateResistance(resistivity, temperature)
            Me._ResistanceSum += calc.Resistance
        Next
    End Sub

    Public Sub UpdateResistance(material As MaterialSpec, temperatureValue As Double)
        UpdateResistance(material.Resistivity, New WireResistanceCalculator.Temperature(temperatureValue, material.TemperatureCoefficient))
    End Sub

    Public ReadOnly Property Count As Integer
        Get
            Return _list.Count
        End Get
    End Property

    Default Public ReadOnly Property Item(idx As Integer) As WireResistanceCalculator
        Get
            Return _list(idx)
        End Get
    End Property

    Public ReadOnly Property Csa As New SumInfo
    Public ReadOnly Property Length As New SumInfo
    Public ReadOnly Property Names As New List(Of String)
    Public ReadOnly Property ResistanceSum As Double = 0

    Public Sub SetEachLengthRelativeToSum(newLength As Double)
        Dim currSum As Double = Length.Sum
        Me._Length = New SumInfo()
        For Each wc As WireResistanceCalculator In Me
            Dim perc As Double = If(currSum > 0, (wc.Length * 100) / currSum, 1)
            wc.Length = (perc * newLength) / 100
            CType(Me.Length, IInternalSumInfo).AddToSum(wc.Length)
        Next
    End Sub

    Public Function GetEnumerator() As IEnumerator(Of WireResistanceCalculator) Implements IEnumerable(Of WireResistanceCalculator).GetEnumerator
        Return _list.GetEnumerator
    End Function

    Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return _list.GetEnumerator
    End Function

    Public Class SumInfo
        Implements IInternalSumInfo

        Private _previousAdded As Nullable(Of Double) = Nothing
        Private _allEqual As Boolean = True
        Private _sum As Double = 0

        Public ReadOnly Property Sum As Double Implements IInternalSumInfo.Sum
            Get
                Return _sum
            End Get
        End Property

        Public ReadOnly Property AllEqual As Boolean Implements IInternalSumInfo.AllEqual
            Get
                Return _allEqual
            End Get
        End Property

        Private Sub AddToSum(value As Double) Implements IInternalSumInfo.AddToSum
            If _previousAdded IsNot Nothing AndAlso _allEqual Then
                _allEqual = _previousAdded.GetValueOrDefault = value
            End If

            _sum += value
            _previousAdded = value
        End Sub
    End Class

    Private Interface IInternalSumInfo
        ReadOnly Property Sum As Double
        ReadOnly Property AllEqual As Boolean
        Sub AddToSum(value As Double)
    End Interface


End Class
