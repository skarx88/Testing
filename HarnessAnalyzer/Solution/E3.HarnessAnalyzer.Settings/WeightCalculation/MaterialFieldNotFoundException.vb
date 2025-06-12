Public Class MaterialFieldNotFoundException
    Inherits Exception

    Private _fieldName As String
    Private _part As Object
    Private _instance As Object

    Public Sub New(msg As String, fieldName As String, part As Object, instance As Object)
        MyBase.New(msg)
        _fieldName = fieldName
        _part = part
        _instance = instance
    End Sub

    ReadOnly Property FieldName As String
        Get
            Return _fieldName
        End Get
    End Property

    ReadOnly Property Instance As Object
        Get
            Return _instance
        End Get
    End Property

    ReadOnly Property Part As Object
        Get
            Return _part
        End Get
    End Property

End Class
