Public Class SearchPattern

    Public Property HarnessPartnumber As String
    Public Property HarnessVersion As String
    Public ReadOnly Property Content As New Dictionary(Of KblObjectType, Dictionary(Of String, ObjectSearchPattern))

    Sub New(Partnumber As String, Version As String)
        HarnessPartnumber = Partnumber
        HarnessVersion = Version
    End Sub

End Class
