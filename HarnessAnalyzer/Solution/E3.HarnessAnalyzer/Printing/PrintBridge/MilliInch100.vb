Imports System.Runtime.Serialization

Namespace Printing

    <DataContract>
    Public Class MilliInch100
        Inherits MilliInch

        Public Sub New(inch100Value As Double)
            MyBase.New(inch100Value)
        End Sub

        Public Overrides ReadOnly Property Inch As Double
            Get
                Return Me.Value / 100
            End Get
        End Property

        Public Function ToInch96() As MilliInch96
            Return New MilliInch96(Me.Inch * 96)
        End Function

        Public Shared Function FromInch(inch As Double) As MilliInch100
            Return New MilliInch100(inch * 100)
        End Function

        Public Shared Function FromMillimeter(millimeter As Double) As MilliInch100
            Return MilliInch100.FromInch(millimeter / MilliInch.INCH_PER_MM)
        End Function

        Overloads Shared Operator <>(inch As MilliInch100, inch2 As MilliInch100) As Boolean
            Return inch.Value <> inch2.Value
        End Operator

        Overloads Shared Operator =(inch As MilliInch100, inch2 As MilliInch100) As Boolean
            Return inch.Value = inch2.Value
        End Operator

    End Class

End Namespace