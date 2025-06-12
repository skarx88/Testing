Imports System.ComponentModel
Imports System.Drawing.Printing
Imports System.Runtime.Serialization

Namespace Printing

    <DataContract>
    Public Class MarginsEx
        Implements System.ComponentModel.INotifyPropertyChanged

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        <DataMember> Private _top As New MilliInch100(100) ' 1/100 inch (because the info comes from Drawing.Printing.Margins -> also commented before the whole HA-printing should be completely reworked to System.Printing!)
        <DataMember> Private _left As New MilliInch100(100)
        <DataMember> Private _bottom As New MilliInch100(100)
        <DataMember> Private _right As New MilliInch100(100)

        Public Sub New()
        End Sub

        Friend Sub New(margins2023 As Compatibility.Printing.MarginsEx_2023)
            _top = New MilliInch100(margins2023.Top)
            _left = New MilliInch100(margins2023.Left)
            _bottom = New MilliInch100(margins2023.Bottom)
            _right = New MilliInch100(margins2023.Right)
        End Sub

        Public Sub New(margins As System.Drawing.Printing.Margins)
            Me.New(New MilliInch100(margins.Left), New MilliInch100(margins.Right), New MilliInch100(margins.Top), New MilliInch100(margins.Bottom))
        End Sub

        Public Sub New(left As MilliInch100, right As MilliInch100, top As MilliInch100, bottom As MilliInch100)
            _left = left
            _right = right
            _top = top
            _bottom = bottom
        End Sub

        Property Top As MilliInch100
            Get
                Return _top
            End Get
            Set(value As MilliInch100)
                If (_top Is Nothing AndAlso value IsNot Nothing) OrElse (value Is Nothing AndAlso _top IsNot Nothing) OrElse (_top <> value) Then
                    _top = value
                    OnPropertyChangedAuto()
                End If
            End Set
        End Property

        Property Left As MilliInch100
            Get
                Return _left
            End Get
            Set(value As MilliInch100)
                If (_left Is Nothing AndAlso value IsNot Nothing) OrElse (value Is Nothing AndAlso _left IsNot Nothing) OrElse (_left <> value) Then
                    _left = value
                    OnPropertyChangedAuto()
                End If
            End Set
        End Property

        Property Bottom As MilliInch100
            Get
                Return _bottom
            End Get
            Set(value As MilliInch100)
                If (_bottom Is Nothing AndAlso value IsNot Nothing) OrElse (value Is Nothing AndAlso _bottom IsNot Nothing) OrElse (_bottom <> value) Then
                    _bottom = value
                    OnPropertyChangedAuto()
                End If
            End Set
        End Property

        Property Right As MilliInch100
            Get
                Return _right
            End Get
            Set(value As MilliInch100)
                If (_right Is Nothing AndAlso value IsNot Nothing) OrElse (value Is Nothing AndAlso _right IsNot Nothing) OrElse (_right <> value) Then
                    _right = value
                    OnPropertyChangedAuto()
                End If
            End Set
        End Property

        Protected Overridable Sub OnPropertyChanged(e As PropertyChangedEventArgs)
            RaiseEvent PropertyChanged(Me, e)
        End Sub

        Public Function ToMargins() As Margins
            Return New Margins(CInt(Me.Left.Value), CInt(Me.Right.Value), CInt(Me.Top.Value), CInt(Me.Bottom.Value))
        End Function

    End Class

End Namespace