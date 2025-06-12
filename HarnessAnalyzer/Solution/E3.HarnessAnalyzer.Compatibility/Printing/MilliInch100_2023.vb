Namespace Printing

    <Serializable>
    Public Class MilliInch100_2023
        Inherits MilliInch_2023

        Public Sub New(inch100Value As Double)
            MyBase.New(inch100Value)
        End Sub

        Public Overrides ReadOnly Property Inch As Double
            Get
                Return Me.Value / 100
            End Get
        End Property

        Public Function ToInch96() As MilliInch96_2023
            Return New MilliInch96_2023(Me.Inch * 96)
        End Function

        Public Shared Function FromInch(inch As Double) As MilliInch100_2023
            Return New MilliInch100_2023(inch * 100)
        End Function

        Public Shared Function FromMillimeter(millimeter As Double) As MilliInch100_2023
            Return MilliInch100_2023.FromInch(millimeter / MilliInch_2023.INCH_PER_MM)
        End Function

        Overloads Shared Operator <>(inch As MilliInch100_2023, inch2 As MilliInch100_2023) As Boolean
            Return inch.Value <> inch2.Value
        End Operator

        Overloads Shared Operator =(inch As MilliInch100_2023, inch2 As MilliInch100_2023) As Boolean
            Return inch.Value = inch2.Value
        End Operator

    End Class

End Namespace