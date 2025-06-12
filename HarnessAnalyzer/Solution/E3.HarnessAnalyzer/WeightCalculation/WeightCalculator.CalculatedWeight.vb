Imports System.ComponentModel
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.HarnessAnalyzer.Settings.WeightSettings
Imports Zuken.E3.Lib.Converter.Unit

Partial Public Class WeightCalculator

    Private Class CalculatedWeight
        Implements ICalculatedWeight

        Private _kblMapper As KBLMapper
        Private _errorEx As Exception = Nothing
        Private _length As NumericValue

        Friend Sub New(total As Double, conductor As Double, source As Source)
            Me.Total = total
            Me.Conductor = conductor
            Me.Source = source
        End Sub

        Friend Sub New(coreOcc As Core_occurrence, kblMapper As KBLMapper, Optional useLength As NumericValue = Nothing, Optional overrideCsaSqMm As Nullable(Of Double) = Nothing)
            _kblMapper = kblMapper

            Try
                If Not CalculateWeightMethodOne(coreOcc, useLength, overrideCsaSqMm) Then
                    If Not CalculateWeightMethodTwo(coreOcc, useLength) AndAlso My.Application.MainForm.WeightSettings.CopperFallBackEnabled Then
                        Me.CalculateGenericCopperWeight(coreOcc, useLength)
                    End If
                End If
            Catch ex As MaterialFieldNotFoundException
                _errorEx = ex
            Catch ex2 As LengthNotFoundException
                _errorEx = ex2
            Catch ex3 As RegexException
                _errorEx = ex3
            End Try

            _kblMapper = Nothing
        End Sub

        Friend Sub New(wireOcc As Wire_occurrence, kblMapper As KBLMapper, Optional useLength As NumericValue = Nothing, Optional overrideCsaSqMm As Nullable(Of Double) = Nothing)
            _kblMapper = kblMapper

            Try
                If Not CalculateWeightMethodOne(wireOcc, useLength, overrideCsaSqMm) Then
                    If Not CalculateWeightMethodTwo(wireOcc, useLength) AndAlso My.Application.MainForm.WeightSettings.CopperFallBackEnabled Then
                        Me.CalculateGenericCopperWeight(wireOcc, useLength)
                    End If
                End If
            Catch ex As MaterialFieldNotFoundException
                _errorEx = ex
            Catch ex2 As LengthNotFoundException
                _errorEx = ex2
            Catch ex3 As RegexException
                _errorEx = ex3
            End Try

            _kblMapper = Nothing
        End Sub

        Friend Sub New(cblPkg As CorePackage, kblMapper As KBLMapper, Optional useLength As NumericValue = Nothing, Optional overrideCoreLength As NumericValue = Nothing, Optional overrideCsaSqMm As Nullable(Of Double) = Nothing)
            _kblMapper = kblMapper
            If useLength Is Nothing Then useLength = cblPkg.GetMaxCoreLength.Extract(_kblMapper)

            Try
                If Not CalculateWeightMethodOne(cblPkg.OwningCable, useLength, overrideCsaSqMm) Then
                    CalculateWeightMethodTwo(cblPkg, overrideCoreLength)  'HINT: Copper calculation for the cable in the way if it's done in wire or core is not needed here -> because the second calc method for the cable calculates the cable weights for each core and sums it up. The decision if to calculate by copper or materialspec is already done within the calculation of each core.
                End If
            Catch ex As MaterialFieldNotFoundException
                _errorEx = ex
            Catch ex2 As LengthNotFoundException
                _errorEx = ex2
            Catch ex3 As RegexException
                _errorEx = ex3
            End Try

            _kblMapper = Nothing
        End Sub

        Friend Sub New(cable As Special_wire_occurrence, kblMapper As KBLMapper, Optional useLengthForCableCalculation As NumericValue = Nothing, Optional overrideCoreLength As NumericValue = Nothing, Optional overrideCsaSqMm As Nullable(Of Double) = Nothing)
            _kblMapper = kblMapper

            Try
                If Not CalculateWeightMethodOne(cable, useLengthForCableCalculation, overrideCsaSqMm) Then
                    CalculateWeightMethodTwo(cable, overrideCoreLength)  'HINT: Copper calculation for the cable in the way if it's done in wire or core is not needed here -> because the second calc method for the cable calculates the cable weights for each core and sums it up. The decision if to calculate by copper or materialspec is already done within the calculation of each core.
                End If
            Catch ex As MaterialFieldNotFoundException
                _errorEx = ex
            Catch ex2 As LengthNotFoundException
                _errorEx = ex2
            Catch ex3 As RegexException
                _errorEx = ex3
            End Try

            _kblMapper = Nothing
        End Sub

        Public Property Total As Nullable(Of Double) = Nothing
        Public Property Conductor As Nullable(Of Double) = Nothing
        Public Property Source As Source = Nothing
        Public Property CsaSqMm As Nullable(Of Double) = Nothing
        Public Property CalculatedMaterialSpecCores As New Dictionary(Of String, ICalculatedWeight)

#Region "Interface"

        Private ReadOnly Property I_CalculatedCores As Dictionary(Of String, ICalculatedWeight) Implements ICalculatedWeight.CalculatedMaterialSpecCores
            Get
                Return CalculatedMaterialSpecCores
            End Get
        End Property

        Private ReadOnly Property I_Total As Nullable(Of Double) Implements ICalculatedWeight.Total
            Get
                Return Total
            End Get
        End Property

        Private ReadOnly Property I_Conductor As Nullable(Of Double) Implements ICalculatedWeight.Conductor
            Get
                Return Conductor
            End Get
        End Property

        Private ReadOnly Property I_Source As Source Implements ICalculatedWeight.Source
            Get
                Return Source
            End Get
        End Property

        Private ReadOnly Property I_CsaSqMm As Nullable(Of Double) Implements ICalculatedWeight.CsaSqMm
            Get
                Return CsaSqMm
            End Get
        End Property

        Private ReadOnly Property I_Error As Exception Implements ICalculatedWeight.Error
            Get
                Return _errorEx
            End Get
        End Property

        Private ReadOnly Property I_HasError As Boolean Implements ICalculatedWeight.HasError
            Get
                Return _errorEx IsNot Nothing
            End Get
        End Property

#End Region

        Private Function CalculateWeightMethodOne(wireOcc As General_wire_occurrence, Optional useLength As NumericValue = Nothing, Optional overrideCsaSqMm As Nullable(Of Double) = Nothing) As Boolean
            Dim genWire As General_wire = wireOcc.GetGeneralWire(_kblMapper)
            CsaSqMm = overrideCsaSqMm

            If genWire IsNot Nothing Then
                If Not overrideCsaSqMm.HasValue Then
                    CsaSqMm = genWire.Cross_section_area.ToSqMillimetre(_kblMapper)
                End If

                With genWire
                    If useLength Is Nothing Then
                        Dim wireLength As Numerical_value = wireOcc.GetLength
                        If wireLength IsNot Nothing Then useLength = wireLength.Extract(_kblMapper)
                    End If

                    Return CalculateWeightMethodOne(.Wire_type, useLength, .Part_number)
                End With
            End If
            Return False
        End Function

        Private Function CalculateWeightMethodOne(coreOcc As Core_occurrence, Optional useLength As NumericValue = Nothing, Optional overrideCsaSqMm As Nullable(Of Double) = Nothing) As Boolean
            Dim core As Core = TryCast(_kblMapper.KblPartMapper(coreOcc.Part), Core)
            CsaSqMm = overrideCsaSqMm

            If core IsNot Nothing Then
                If Not overrideCsaSqMm.HasValue Then CsaSqMm = core.Cross_section_area.ToSqMillimetre(_kblMapper)

                With core
                    If useLength Is Nothing Then
                        Dim coreLength As Numerical_value = coreOcc.GetLength
                        If coreLength IsNot Nothing Then useLength = coreLength.Extract(_kblMapper)
                    End If
                    Return CalculateWeightMethodOne(.Wire_type, useLength)
                End With
            End If
            Return False
        End Function

        Private Function CalculateWeightMethodOne(wireType As String, length As NumericValue, Optional partNumber As String = Nothing) As Boolean
            Dim weight As WeightSettings.Weight = Nothing

            If Not String.IsNullOrEmpty(partNumber) Then
                weight = GetWeightByPartNr(partNumber)
                If weight IsNot Nothing Then Source = New Source(CalcSource.LookUp, partNumber)
            End If

            If Not String.IsNullOrEmpty(wireType) AndAlso Source Is Nothing Then
                weight = GetWeightByWireType(wireType)
                If weight IsNot Nothing Then Source = New Source(CalcSource.LookUp, wireType)
            End If

            If Source IsNot Nothing Then
                If weight IsNot Nothing Then
                    If length IsNot Nothing Then
                        _length = length
                        Dim wireLengthMetre As Nullable(Of Double) = length.ToMetre  ' convert to meter if it's not in metre 
                        Conductor = (weight.Conductor * wireLengthMetre)             ' (g/m * m) = g
                        Total = (weight.Total * wireLengthMetre)                     ' (g/m * m) = g
                        Return True
                    End If
                End If
            End If
            Return False
        End Function

        ''' <summary>
        ''' Calculates the weight by materialspec for the cable -> special case here: the cable has the sum of the calculated cores
        ''' </summary>
        ''' <param name="cable"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CalculateWeightMethodTwo(cable As Special_wire_occurrence, Optional overrideCoreLength As NumericValue = Nothing) As Boolean
            If cable.Core_occurrence.Length > 0 Then
                Me.CalculatedMaterialSpecCores.Clear()
                Dim allCoreLengths As New List(Of KeyValuePair(Of Double, NumericValue))
                Dim prevCoreLength As NumericValue = Nothing

                For Each core As Core_occurrence In cable.Core_occurrence
                    Dim currentCoreLength As NumericValue = overrideCoreLength
                    If currentCoreLength Is Nothing Then
                        Dim coreLength As Numerical_value = core.GetLength
                        If coreLength IsNot Nothing Then currentCoreLength = coreLength.Extract(_kblMapper)
                    End If

                    Dim calc As ICalculatedWeight = WeightCalculator.Calculate(core, _kblMapper, currentCoreLength)
                    Me.CalculatedMaterialSpecCores.Add(core.SystemId, calc)
                    If calc.Conductor.HasValue Then Me.Conductor = If(Me.Conductor.HasValue, Me.Conductor + calc.Conductor, calc.Conductor)
                    If calc.Total.HasValue Then Me.Total = If(Me.Total.HasValue, Me.Total + calc.Total, calc.Total)
                    If calc.Source IsNot Nothing Then Me.Source = If(Me.Source IsNot Nothing, New Source(Me.Source.Value Or calc.Source.Value, String.Concat(Me.Source.Info, ";", calc.Source.Info)), New Source(calc.Source.Value, calc.Source.Info))

                    Dim calcLength As NumericValue = calc.Length
                    If prevCoreLength IsNot Nothing Then calcLength = calcLength.ConvertTo(prevCoreLength)
                    If calcLength Is Nothing Then
                        allCoreLengths.Add(New KeyValuePair(Of Double, NumericValue)(0, calcLength))
                    Else
                        allCoreLengths.Add(New KeyValuePair(Of Double, NumericValue)(calcLength.Value, calcLength))
                    End If

                    prevCoreLength = currentCoreLength
                Next

                If allCoreLengths.Count > 0 Then
                    _length = allCoreLengths.OrderByDescending(Function(kv) kv.Key).First.Value
                Else
                    _length = Nothing
                End If

                Return Me.Source IsNot Nothing
            End If
            Return False
        End Function

        ''' <summary>
        ''' Calculates the weight by materialspec for the cable -> special case here: the cable has the sum of the calculated cores
        ''' </summary>
        ''' <param name="cable"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CalculateWeightMethodTwo(cable As CorePackage, Optional overrideCoreLength As NumericValue = Nothing) As Boolean
            If cable.Count > 0 Then
                Me.CalculatedMaterialSpecCores.Clear()
                Dim allCoreLengths As New List(Of KeyValuePair(Of Double, NumericValue))
                Dim prevCoreLength As NumericValue = Nothing

                For Each corePkg As SegmentedCore In cable
                    Dim calc As ICalculatedWeight = WeightCalculator.Calculate(corePkg.Object, _kblMapper, If(overrideCoreLength Is Nothing, corePkg.Length.Extract(_kblMapper), overrideCoreLength))
                    Me.CalculatedMaterialSpecCores.Add(corePkg.Object.SystemId, calc)
                    If calc.Conductor.HasValue Then Me.Conductor = If(Me.Conductor.HasValue, Me.Conductor + calc.Conductor, calc.Conductor)
                    If calc.Total.HasValue Then Me.Total = If(Me.Total.HasValue, Me.Total + calc.Total, calc.Total)
                    If calc.Source IsNot Nothing Then Me.Source = If(Me.Source IsNot Nothing, New Source(Me.Source.Value Or calc.Source.Value, String.Concat(Me.Source.Info, ";", calc.Source.Info)), New Source(calc.Source.Value, calc.Source.Info))

                    Dim calcLength As NumericValue = calc.Length
                    If prevCoreLength IsNot Nothing Then calcLength = calcLength.ConvertTo(prevCoreLength)

                    If calcLength Is Nothing Then
                        allCoreLengths.Add(New KeyValuePair(Of Double, NumericValue)(0, calcLength))
                    Else
                        allCoreLengths.Add(New KeyValuePair(Of Double, NumericValue)(calcLength.Value, calcLength))
                    End If

                    prevCoreLength = calcLength
                Next

                If allCoreLengths.Count > 0 Then
                    _length = allCoreLengths.OrderByDescending(Function(kv) kv.Key).First.Value
                Else
                    _length = Nothing
                End If

                Return Me.Source IsNot Nothing
            End If
            Return False
        End Function

        ''' <summary>
        ''' Calculates the weight by materialspec for the wire
        ''' </summary>
        ''' <param name="wireOcc"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CalculateWeightMethodTwo(wireOcc As Wire_occurrence, Optional useLength As NumericValue = Nothing) As Boolean
            Dim genWire As General_wire = wireOcc.GetGeneralWire(_kblMapper)
            Try
                If useLength Is Nothing Then
                    Dim wireLength As Numerical_value = wireOcc.GetLength
                    If wireLength IsNot Nothing Then useLength = wireLength.Extract(_kblMapper)
                End If

                Return CalculateWeightMethodTwo(genWire, useLength)
            Catch ex As MaterialFieldNotFoundException
                Throw New MaterialFieldNotFoundException(ex.Message, ex.FieldName, ex.Part, wireOcc)
            End Try
        End Function

        Private Function CalculateWeightMethodTwo(coreOcc As Core_occurrence, Optional useLength As NumericValue = Nothing) As Boolean
            Dim cable As Special_wire_occurrence = coreOcc.GetCable(_kblMapper)
            Dim genCableWire As General_wire = cable.GetGeneralWire(_kblMapper)
            Try
                If useLength Is Nothing Then
                    Dim cableLength As Numerical_value = cable.GetLength
                    If cableLength IsNot Nothing Then useLength = cableLength.Extract(_kblMapper)
                End If

                Return CalculateWeightMethodTwo(genCableWire, useLength)
            Catch ex As MaterialFieldNotFoundException
                Throw New MaterialFieldNotFoundException(ex.Message, ex.FieldName, ex.Part, coreOcc)
            End Try
        End Function

        Private Function CalculateWeightMethodTwo(generalWire As General_wire, length As NumericValue) As Boolean
            ArgumentNullException.ThrowIfNull(generalWire)

            If CsaSqMm.HasValue Then
                With My.Application.MainForm.WeightSettings
                    For Each spec As MaterialSpec In My.Application.MainForm.WeightSettings.MaterialSpecs.FindMaterialSpecsFor2(generalWire)
                        If length IsNot Nothing Then
                            CalculateWeightBySpec(spec.SpecificWeight, length)
                            Me.Source = New Source(CalcSource.MaterialSpec, spec.Description)
                            Return True
                        End If
                    Next
                End With
            End If
            Return False
        End Function

        ''' <summary>
        ''' Calculates the weight's from the specific material weight and the given length
        ''' </summary>
        ''' <param name="specificWeight">specific material weight in g/cm³</param>
        ''' <param name="length"></param>
        ''' <remarks></remarks>
        Private Sub CalculateWeightBySpec(specificWeight As Double, length As NumericValue)
            If CsaSqMm.HasValue Then
                Dim CSASqcm As Nullable(Of Double) = UnitConverter.Convert(CsaSqMm.Value, ShortUnit.SqMm, ShortUnit.SqCm, True)
                If CSASqcm IsNot Nothing AndAlso length IsNot Nothing Then
                    Me.Conductor = specificWeight * CSASqcm * length.ToCentimetre 'g/cm³ * cm² * cm = g
                    With My.Application.MainForm.WeightSettings.GenericInsulationWeightParameters
                        Dim insulationWeight As Double = InsulationMeterWeightFormula.Execute(.GIW_Offset, .GIW_Slope, .GIW_Square, CsaSqMm.Value) ' = (g/m*mm⁴) * mm⁴ + (g/m*mm²) * mm² + g/m = g/m
                        Me.Total = Me.Conductor + (insulationWeight * length.ToMetre) ' g + (g/m * m) = g
                    End With
                    _length = length
                End If
            End If
        End Sub

        Private Sub CalculateGenericCopperWeight(wireOcc As Wire_occurrence, Optional useLength As NumericValue = Nothing)
            If useLength Is Nothing Then
                Dim wireLength As Numerical_value = wireOcc.GetLength
                If wireLength IsNot Nothing Then useLength = wireLength.Extract(_kblMapper)
            End If
            CalculateWeightBySpec(HarnessAnalyzer.Shared.SPECIFIC_COPPER_WEIGHT, useLength)
            Me.Source = New Source(CalcSource.NotFound_Copper)
        End Sub

        Private Sub CalculateGenericCopperWeight(coreOcc As Core_occurrence, Optional useLength As NumericValue = Nothing)
            If useLength Is Nothing Then
                Dim coreLength As Numerical_value = coreOcc.GetLength
                If coreLength IsNot Nothing Then useLength = coreLength.Extract(_kblMapper)
            End If

            CalculateWeightBySpec(HarnessAnalyzer.Shared.SPECIFIC_COPPER_WEIGHT, useLength)
            Me.Source = New Source(CalcSource.NotFound_Copper)
        End Sub

        Private Function GetWeightByPartNr(partNumber As String) As WeightSettings.Weight
            Return My.Application.MainForm.WeightSettings.Weights.GetByPartNumber(partNumber).SingleOrDefault()
        End Function

        Private Function GetWeightByWireType(wireType As String) As WeightSettings.Weight
            Return My.Application.MainForm.WeightSettings.Weights.GetByWireType(wireType).SingleOrDefault()
        End Function

        Public Overrides Function ToString() As String
            Return String.Format(String.Format("CalcWeight- Total: {0}, Conductor: {1}, Source {2}", Me.Total, Me.Conductor, Me.Source))
        End Function

        Private Enum errorType
            Unknonwn = 0
            LengthNotFound = 1
            MaterialFieldNotFound = 2
        End Enum

        Public ReadOnly Property Length As NumericValue Implements ICalculatedWeight.Length
            Get
                Return _length
            End Get
        End Property

    End Class

    Public Interface ICalculatedWeight
        ReadOnly Property Total As Nullable(Of Double)
        ReadOnly Property Conductor As Nullable(Of Double)
        ReadOnly Property Source As Source
        ReadOnly Property CsaSqMm As Nullable(Of Double)

        ''' <summary>
        ''' These are the calculated core-weights, when the weight of the cable was calculated by the sum of the calculated material spec weight of each core.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Browsable(False)>
        ReadOnly Property CalculatedMaterialSpecCores As Dictionary(Of String, ICalculatedWeight)

        <Browsable(False)>
        ReadOnly Property HasError As Boolean
        <Browsable(False)>
        ReadOnly Property [Error] As Exception

        ReadOnly Property Length As NumericValue

    End Interface

    Public Class Source

        Private _value As CalcSource
        Private _info As String

        Public Sub New(value As CalcSource, Optional info As String = Nothing)
            _value = value
            _info = info
        End Sub

        Public ReadOnly Property Value As CalcSource
            Get
                Return _value
            End Get
        End Property

        Public ReadOnly Property Info As String
            Get
                Return _info
            End Get
        End Property

    End Class


End Class
