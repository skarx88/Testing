Imports System.Reflection
Imports Zuken.E3.HarnessAnalyzer.Settings

Public Class WeightCalculator

    Private _calculated As CalculatedWeight
    Private _original As WeightNumericValue
    Private _kblMapper As KBLMapper

    Public Sub New(kblMapper As KBLMapper)
        _kblMapper = kblMapper
    End Sub

    ''' <summary>
    ''' Calculates weight for given object (without having the object-type)
    ''' </summary>
    ''' <param name="obj">object to calculate the weight: wire, core, cable, etc.</param>
    ''' <param name="useLength">Use length for calculation. When null the length is tried to extract from the given object</param>
    ''' <param name="overrideCsaSqMm">Override the csa for the calculation. When null the Csa is extracted from the given object. If this was not possible the csa of the weightCalculator is null at the end of the calculation.</param>
    ''' <remarks></remarks>
    Public Sub UntypedCalculate(obj As Object, Optional useLength As NumericValue = Nothing, Optional overrideCsaSqMm As Nullable(Of Double) = Nothing)
        Dim calcMethod As MethodInfo = GetCorrespondingCalculateMethod(obj)
        If calcMethod IsNot Nothing Then
            calcMethod.Invoke(Me, New Object() {obj, useLength, overrideCsaSqMm})
        Else
            Throw New NotImplementedException(String.Format("Calculate method for type ""{0}"" not found in ""{1}""", obj.GetType.Name, Me.GetType.Name))
        End If
    End Sub

    Private Function GetCorrespondingCalculateMethod(obj As Object) As MethodInfo
        For Each method As MethodInfo In Me.GetType.GetMethods(BindingFlags.Public Or BindingFlags.Instance)
            If method.GetCustomAttributes(GetType(CalculateMethodAttribute), False).Length > 0 Then
                If method.GetParameters.Where(Function(pi) pi.ParameterType = obj.GetType OrElse pi.ParameterType = GetType(NumericValue) OrElse pi.ParameterType = GetType(Nullable(Of Double))).Count = 3 Then
                    Return method
                End If
            End If
        Next
        Return Nothing
    End Function

#Region "Instance Calculate methods"


    ''' <summary>
    ''' Calculates the mass for this core (KBL/original-mass is taken as average from cable)
    ''' </summary>
    ''' <param name="coreOcc"></param>
    ''' <remarks></remarks>
    <CalculateMethod>
    Public Sub Calculate(coreOcc As Core_occurrence, Optional useLength As NumericValue = Nothing, Optional overrideCsaSqMm As Nullable(Of Double) = Nothing)
        With coreOcc
            _original = coreOcc.GetAverageWeight(_kblMapper)
            _calculated = New CalculatedWeight(coreOcc, _kblMapper, useLength, overrideCsaSqMm)
        End With
    End Sub

    ''' <summary>
    ''' Calculates the mass for a wire 
    ''' </summary>
    ''' <param name="wire"></param>
    ''' <remarks></remarks>
    <CalculateMethod>
    Public Sub Calculate(wire As Wire_occurrence, Optional useLength As NumericValue = Nothing, Optional overrideCsaSqMm As Nullable(Of Double) = Nothing)
        With wire
            _original = wire.GetWeight(_kblMapper)
            _calculated = New CalculatedWeight(wire, _kblMapper, useLength, overrideCsaSqMm)
        End With
    End Sub

    ''' <summary>
    ''' Calculates the mass for a wire 
    ''' </summary>
    ''' <param name="wire"></param>
    ''' <remarks></remarks>
    <CalculateMethod>
    Public Sub Calculate(wire As Specified_wire_occurrence, Optional useLength As NumericValue = Nothing, Optional overrideCsaSqMm As Nullable(Of Double) = Nothing)
        With wire
            _original = wire.GetWeight(_kblMapper)
            _calculated = New CalculatedWeight(wire, _kblMapper, useLength, overrideCsaSqMm)
        End With
    End Sub

    ''' <summary>
    ''' Calculates the mass for a cable 
    ''' </summary>
    ''' <param name="cable"></param>
    ''' <remarks></remarks>
    <CalculateMethod>
    Public Sub Calculate(cable As Specified_special_wire_occurrence, Optional overrideLength As NumericValue = Nothing, Optional overrideCsaSqMm As Nullable(Of Double) = Nothing)
        With cable
            _original = cable.GetWeight(_kblMapper)
            _calculated = New CalculatedWeight(cable, _kblMapper, overrideLength, If(OverrideCoreLength, overrideLength, Nothing), overrideCsaSqMm)
        End With
    End Sub

    ''' <summary>
    ''' Calculates the mass for a cable 
    ''' </summary>
    ''' <param name="cable"></param>
    ''' <remarks></remarks>
    <CalculateMethod>
    Public Sub Calculate(cable As Special_wire_occurrence, Optional overrideLength As NumericValue = Nothing, Optional overrideCsaSqMm As Nullable(Of Double) = Nothing)
        With cable
            _original = cable.GetWeight(_kblMapper)
            _calculated = New CalculatedWeight(cable, _kblMapper, overrideLength, If(OverrideCoreLength, overrideLength, Nothing), overrideCsaSqMm)
        End With
    End Sub

    ''' <summary>
    ''' Calculates the mass for a cable 
    ''' </summary>
    ''' <param name="cable"></param>
    ''' <remarks></remarks>
    <CalculateMethod>
    Public Sub Calculate(cable As CorePackage, Optional overrideLength As NumericValue = Nothing, Optional overrideCsaSqMm As Nullable(Of Double) = Nothing)
        With cable
            _original = cable.OwningCable.GetWeight(_kblMapper)
            _calculated = New CalculatedWeight(cable, _kblMapper, overrideLength, If(OverrideCoreLength, overrideLength, Nothing), overrideCsaSqMm)
        End With
    End Sub

#End Region

#Region "Shared Calculate methods"

    Public Shared Function Calculate(wire As Wire_occurrence, kblMapper As KBLMapper, Optional useLength As NumericValue = Nothing, Optional overrideCsaSqMm As Nullable(Of Double) = Nothing) As ICalculatedWeight
        Dim calc As New WeightCalculator(kblMapper)
        calc.Calculate(wire, useLength, overrideCsaSqMm)
        Return calc.Calculated
    End Function

    Public Shared Function Calculate(cable As Special_wire_occurrence, kblMapper As KBLMapper, Optional useLength As NumericValue = Nothing, Optional overrideCsaSqMm As Nullable(Of Double) = Nothing) As ICalculatedWeight
        Dim calc As New WeightCalculator(kblMapper)
        calc.Calculate(cable, useLength, overrideCsaSqMm)
        Return calc.Calculated
    End Function

    Public Shared Function Calculate(core As Core_occurrence, kblMapper As KBLMapper, Optional useLength As NumericValue = Nothing, Optional overrideCsaSqMm As Nullable(Of Double) = Nothing) As ICalculatedWeight
        Dim calc As New WeightCalculator(kblMapper)
        calc.Calculate(core, useLength, overrideCsaSqMm)
        Return calc.Calculated
    End Function

#End Region

    Public ReadOnly Property Calculated As ICalculatedWeight
        Get
            Return _calculated
        End Get
    End Property

    Public ReadOnly Property OriginalWeight As WeightNumericValue
        Get
            Return _original
        End Get
    End Property

    ''' <summary>
    ''' Overrides the core length with the provided length when calculating the weight over the cores seperatly
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property OverrideCoreLength As Boolean = False

    <Flags>
    Public Enum CalcSource
        ''' <summary>
        ''' Souce is undefined
        ''' </summary>
        ''' <remarks></remarks>
        None = 0
        ''' <summary>
        ''' Weight was calculated by lookup
        ''' </summary>
        ''' <remarks></remarks>
        LookUp = 1
        ''' <summary>
        ''' Weight was calculated by material specification (look up not found)
        ''' </summary>
        ''' <remarks></remarks>
        MaterialSpec = 2
        ''' <summary>
        ''' Weight coun't be calculated by lookup or material spec because none of them was found. The weight was calculated by copper spec because it was enabled in the weight settings.
        ''' </summary>
        ''' <remarks></remarks>
        NotFound_Copper = 4
    End Enum

    Public Class InsulationMeterWeightFormula
        ''' <summary>
        ''' Calculates the InsulationWeight in gram per metre
        ''' </summary>
        ''' <param name="GIWOffset"> g/m </param>
        ''' <param name="GIWSlope"> g/(m*mm²)</param>
        ''' <param name="GIWSquare"> g/(m*mm⁴)</param>
        ''' <param name="CSAMm"> cross section area in mm² </param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Execute(GIWOffset As Double, GIWSlope As Double, GIWSquare As Double, CSAMm As Double) As Double
            Return GIWOffset + GIWSlope * CSAMm + GIWSquare * CSAMm ^ 2
        End Function

    End Class

    Public Enum CoreCalcType
        FromEachCore = 0
        FromGivenLengthOrEachCore = 1
    End Enum

    ''' <summary>
    ''' Used for Reflection when obfuscated to find the calculate-methods which are renamed after obfuscation
    ''' </summary>
    ''' <remarks></remarks>
    <AttributeUsage(AttributeTargets.Method)>
    Private Class CalculateMethodAttribute
        Inherits Attribute
    End Class


End Class
