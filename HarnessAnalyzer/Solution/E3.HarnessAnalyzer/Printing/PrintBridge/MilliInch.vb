Imports System.Runtime.Serialization

Namespace Printing

    <DataContract>
    Public MustInherit Class MilliInch

        Protected Const INCH_PER_MM As Double = 25.4

        <DataMember> Private _value As Double

        Public Sub New(value As Double)
            _value = value
        End Sub

        Protected Sub New()
        End Sub

        Public ReadOnly Property Value As Double
            Get
                Return _value
            End Get
        End Property

        Public ReadOnly Property Millimeter As Double
            Get
                Return Me.Inch * INCH_PER_MM
            End Get
        End Property

        Public MustOverride ReadOnly Property Inch As Double

        Public Overloads Shared Widening Operator CType(inch As MilliInch) As Double
            Return inch.Value
        End Operator

        Public Overloads Shared Operator /(inch As MilliInch, value As Double) As Double
            Return inch.Value / value
        End Operator

        Public Overloads Shared Operator <>(inch As MilliInch, value As Double) As Boolean
            Return inch.Value <> value
        End Operator

        Public Overloads Shared Operator =(inch As MilliInch, value As Double) As Boolean
            Return inch.Value = value
        End Operator

    End Class

End Namespace