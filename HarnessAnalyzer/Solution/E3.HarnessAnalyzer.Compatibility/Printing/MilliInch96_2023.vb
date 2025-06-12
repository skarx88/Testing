Namespace Printing

    ''' <summary>
    ''' Represents an 1/96 inch - value
    ''' </summary>
    <Serializable>
    Public Class MilliInch96_2023
        Inherits MilliInch_2023

        Public Sub New(inch96Value As Double)
            MyBase.New(inch96Value)
        End Sub

        Public Overrides ReadOnly Property Inch As Double
            Get
                Return Me.Value / 96
            End Get
        End Property

        Public Function ToInch100() As MilliInch100_2023
            Return New MilliInch100_2023(Me.Inch * 100)
        End Function

        Public Shared Function FromInch(inch As Double) As MilliInch96_2023
            Return New MilliInch96_2023(inch * 96)
        End Function

        Public Shared Function FromMillimeter(millimeter As Double) As MilliInch96_2023
            Return MilliInch96_2023.FromInch(millimeter / MilliInch_2023.INCH_PER_MM)
        End Function

        Overloads Shared Operator <>(inch As MilliInch96_2023, inch2 As MilliInch96_2023) As Boolean
            Return inch.Value <> inch2.Value
        End Operator

        Overloads Shared Operator =(inch As MilliInch96_2023, inch2 As MilliInch96_2023) As Boolean
            Return inch.Value = inch2.Value
        End Operator

    End Class

End Namespace