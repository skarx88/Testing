Public Class DocumentFormMessageEventArgs
    Inherits MessageEventArgs

    Public Sub New(MessageType As MsgType, document As DocumentForm)
        MyBase.New(MessageType)
        Me.DocumentFrm = document
    End Sub

    Public Sub New(MessageType As MsgType, progressValue As Integer, document As DocumentForm)
        MyBase.New(MessageType, progressValue)
        Me.DocumentFrm = document
    End Sub

    Public Sub New(MessageType As MsgType, progressValue As Integer, statusMsg As String, document As DocumentForm)
        MyBase.New(MessageType, progressValue, statusMsg)
        Me.DocumentFrm = document
    End Sub

    ReadOnly Property DocumentFrm As DocumentForm

End Class
