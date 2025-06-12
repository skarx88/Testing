Imports System.Runtime.Serialization

Namespace Printing

    ''' <summary>
    ''' Represents an 1/96 inch - value
    ''' </summary>
    <DataContract>
    Public Class MilliInch96
        Inherits MilliInch

        Public Sub New(inch96Value As Double)
            MyBase.New(inch96Value)
        End Sub

        Public Overrides ReadOnly Property Inch As Double
            Get
                Return Me.Value / 96
            End Get
        End Property

        Public Function ToInch100() As MilliInch100
            Return New MilliInch100(Me.Inch * 100)
        End Function

        Public Shared Function FromInch(inch As Double) As MilliInch96
            Return New MilliInch96(inch * 96)
        End Function

        Public Shared Function FromMillimeter(millimeter As Double) As MilliInch96
            Return MilliInch96.FromInch(millimeter / MilliInch.INCH_PER_MM)
        End Function

        Public Overloads Shared Operator <>(inch As MilliInch96, inch2 As MilliInch96) As Boolean
            Return inch.Value <> inch2.Value
        End Operator

        Public Overloads Shared Operator =(inch As MilliInch96, inch2 As MilliInch96) As Boolean
            Return inch.Value = inch2.Value
        End Operator

    End Class

End Namespace