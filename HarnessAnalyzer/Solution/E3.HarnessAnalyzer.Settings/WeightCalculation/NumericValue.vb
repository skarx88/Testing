Imports Zuken.E3.Lib.Converter.Unit

Public Class NumericValue

    Property Unit As CalcUnit
    Property Value As Double

    Public Overrides Function ToString() As String
        Return String.Format("Value {0}; Unit {1}", Value, Unit.ToString)
    End Function

    Public Function ToMetre() As Nullable(Of Double)
        Return UnitConverter.Convert(Me.Value, Me.Unit, New CalcUnit(CalcUnit.UnitEnum.Metre, CalcUnit.UPrefixEnum.None, Unit.UnitDimension), True)
    End Function

    Public Function ToCentimetre() As Nullable(Of Double)
        Return UnitConverter.Convert(Me.Value, Me.Unit, New CalcUnit(CalcUnit.UnitEnum.Metre, CalcUnit.UPrefixEnum.Centi, Unit.UnitDimension), True)
    End Function

    Public Function ConvertTo(otherNv As NumericValue) As NumericValue
        Dim convValue As Nullable(Of Double) = UnitConverter.Convert(Me.Value, Me.Unit, otherNv.Unit)
        If convValue.HasValue Then
            Return New NumericValue() With {.Unit = otherNv.Unit, .Value = convValue.Value}
        End If
        Return Nothing
    End Function

    Public Function Add(otherNumValue As NumericValue) As NumericValue
        Dim other As NumericValue = otherNumValue
        If otherNumValue.Unit.Unit <> Me.Unit.Unit OrElse otherNumValue.Unit.UnitDimension <> Me.Unit.UnitDimension OrElse otherNumValue.Unit.UnitPrefix <> Me.Unit.UnitPrefix Then
            Dim otherToMeValue As Nullable(Of Double) = UnitConverter.Convert(otherNumValue.Value, otherNumValue.Unit, Me.Unit)
            other = New NumericValue() With {.Unit = Me.Unit, .Value = otherToMeValue.Value}
        End If
        Return New NumericValue With {.Unit = Me.Unit, .Value = other.Value + Me.Value}
    End Function

End Class