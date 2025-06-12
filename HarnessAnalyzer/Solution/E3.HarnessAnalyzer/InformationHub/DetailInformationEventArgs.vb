Public Class DetailInformationEventArgs
    Inherits EventArgs

    Public ObjectId As String
    Public Sub New(id As String)
        ObjectId = id
    End Sub

End Class
