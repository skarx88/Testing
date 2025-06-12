Imports Zuken.E3.Lib.Converter.Unit
Imports Zuken.E3.Lib.Schema.Kbl
Imports Zuken.E3.Lib.Schema.Kbl.Units

Public Class WeightNumericValue
    Inherits NumericValue

    Property LengthUnit As CalcUnit

    Public Overrides Function ToString() As String
        Return String.Format("{0} | LengthUnit: {1}", MyBase.ToString, If(LengthUnit IsNot Nothing, LengthUnit.ToString, "<Unknown>"))
    End Function

    Public Shared Function Extract(numValue As Numerical_value, kblMapper As KBLMapper) As WeightNumericValue
        If numValue IsNot Nothing Then
            Dim inputUnit As Unit = kblMapper.GetUnit(numValue.Unit_component)
            Dim pair As CalcUnitPair = inputUnit.ToCalcUnitPair
            Return New WeightNumericValue() With {.LengthUnit = pair.Second, .Unit = pair.First, .Value = numValue.Value_component}
        End If
        Return New WeightNumericValue()
    End Function

End Class
