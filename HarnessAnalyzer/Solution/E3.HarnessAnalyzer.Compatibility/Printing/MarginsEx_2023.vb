Namespace Printing

    <Serializable>
    Public Class MarginsEx_2023
        Private _top As New MilliInch100_2023(100) ' 1/100 inch (because the info comes from Drawing.Printing.Margins -> also commented before the whole HA-printing should be completely reworked to System.Printing!)
        Private _left As New MilliInch100_2023(100)
        Private _bottom As New MilliInch100_2023(100)
        Private _right As New MilliInch100_2023(100)

        Public Sub New()
        End Sub

        Property Top As MilliInch100_2023
            Get
                Return _top
            End Get
            Set(value As MilliInch100_2023)
                If (_top Is Nothing AndAlso value IsNot Nothing) OrElse (value Is Nothing AndAlso _top IsNot Nothing) OrElse (_top <> value) Then
                    _top = value
                End If
            End Set
        End Property

        Property Left As MilliInch100_2023
            Get
                Return _left
            End Get
            Set(value As MilliInch100_2023)
                If (_left Is Nothing AndAlso value IsNot Nothing) OrElse (value Is Nothing AndAlso _left IsNot Nothing) OrElse (_left <> value) Then
                    _left = value
                End If
            End Set
        End Property

        Property Bottom As MilliInch100_2023
            Get
                Return _bottom
            End Get
            Set(value As MilliInch100_2023)
                If (_bottom Is Nothing AndAlso value IsNot Nothing) OrElse (value Is Nothing AndAlso _bottom IsNot Nothing) OrElse (_bottom <> value) Then
                    _bottom = value
                End If
            End Set
        End Property

        Property Right As MilliInch100_2023
            Get
                Return _right
            End Get
            Set(value As MilliInch100_2023)
                If (_right Is Nothing AndAlso value IsNot Nothing) OrElse (value Is Nothing AndAlso _right IsNot Nothing) OrElse (_right <> value) Then
                    _right = value
                End If
            End Set
        End Property

    End Class

End Namespace