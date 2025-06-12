Public Class LengthNotFoundException
    Inherits Exception

    Private _lengthInfos As List(Of Wire_length)
    Private _defaultLength As String

    Public Sub New(msg As String, defaultLength As String, lengthInfos As List(Of Wire_length))
        MyBase.New(msg)
        _lengthInfos = lengthInfos
        _defaultLength = defaultLength
    End Sub

    ReadOnly Property LengthInfos As List(Of Wire_length)
        Get
            Return _lengthInfos
        End Get
    End Property

    ReadOnly Property DefaultLength As String
        Get
            Return _defaultLength
        End Get
    End Property

End Class
