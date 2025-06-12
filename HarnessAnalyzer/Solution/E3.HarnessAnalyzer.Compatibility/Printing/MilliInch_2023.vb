Namespace Printing

    <Serializable>
    Public MustInherit Class MilliInch_2023

        Protected Const INCH_PER_MM As Double = 25.4

        Public Sub New(value As Double)
            Me.Value = value
        End Sub

        ReadOnly Property Value As Double

        ReadOnly Property Millimeter As Double
            Get
                Return Me.Inch * INCH_PER_MM
            End Get
        End Property

        MustOverride ReadOnly Property Inch As Double

        Overloads Shared Widening Operator CType(inch As MilliInch_2023) As Double
            Return inch.Value
        End Operator

        Overloads Shared Operator /(inch As MilliInch_2023, value As Double) As Double
            Return inch.Value / value
        End Operator

        Overloads Shared Operator <>(inch As MilliInch_2023, value As Double) As Boolean
            Return inch.Value <> value
        End Operator

        Overloads Shared Operator =(inch As MilliInch_2023, value As Double) As Boolean
            Return inch.Value = value
        End Operator

    End Class

End Namespace