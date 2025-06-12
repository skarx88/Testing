Public Class RegexException
    Inherits Exception

    Private _regExString As String
    Private _refObject As Object

    Public Sub New(msg As String, regExString As String, refObject As Object, Optional innerEx As Exception = Nothing)
        MyBase.New(msg, innerEx)
        _regExString = regExString
        _refObject = refObject
    End Sub

    ReadOnly Property RegexString As String
        Get
            Return _regExString
        End Get
    End Property

    ReadOnly Property ReferenceObject As Object
        Get
            Return _refObject
        End Get
    End Property

End Class