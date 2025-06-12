Imports System.Runtime.CompilerServices
Imports Zuken.E3.Lib.Converter.Unit
Imports Zuken.E3.Lib.Schema.Kbl.Units

<HideModuleName>
Public Module UnitConverterExtensions

    <Extension>
    Function GetCalcUnit(numValue As Numerical_value, kbl As [Lib].Schema.Kbl.IKblContainer) As CalcUnit
        Dim inputUnit As Unit = kbl.GetUnits.Where(Function(unit) unit.SystemId = numValue.Unit_component).FirstOrDefault
        If inputUnit IsNot Nothing Then
            Return CalcUnitPair.ParseOrEmpty(KblUnitConverter.GetUnitInfo(inputUnit)).First
        End If
        Return Nothing
    End Function

    <Extension>
    Public Function ToCalcUnitPair(inputUnit As Unit) As CalcUnitPair
        Return KblUnitConverter.GetUnitInfo(inputUnit).ToCalcUnitPair
    End Function

    <Extension>
    Public Function ToSqMillimetre(numValue As Numerical_value, kbl As [Lib].Schema.Kbl.IKblContainer, Optional disableExWhenFromString As Boolean = True) As Nullable(Of Double)
        If numValue IsNot Nothing Then
            Dim unitConv As New KblUnitConverter(kbl)
            Dim unitValue As New Numerical_value() With {.Unit_component = numValue.Unit_component, .Value_component = numValue.Value_component}
            Return unitConv.Convert(unitValue, New CalcUnit(CalcUnit.UnitEnum.Metre, CalcUnit.UPrefixEnum.Milli, CalcUnit.UDimensionEnum.Square), disableExWhenFromString)
        End If
        Return Nothing
    End Function


    Friend Class KblUnitConverter
        Inherits KblBaseUnitConverter

        Public Sub New(kbl As [Lib].Schema.Kbl.IKblContainer)
            MyBase.New(kbl)
        End Sub

        Protected Overrides Function ResolveUnit(unitId As String) As KblPlainUnit
            Dim u As Unit = CType(Kbl, [Lib].Schema.Kbl.IKblContainer).GetUnits.Where(Function(unit) unit.SystemId = unitId).FirstOrDefault
            Return GetUnitInfo(u)
        End Function

        Friend Shared Function GetUnitInfo(u As Unit) As KblPlainUnit
            Return New KblPlainUnit(u)
        End Function

    End Class


End Module

