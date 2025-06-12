<Serializable()>
Public Class CheckedCompareResultEntry

    Public Sub New()
        Comment = String.Empty
        CompareSignature = String.Empty
        ReferenceSignature = String.Empty
    End Sub

    Public Sub New(compSignature As String, refSignature As String)
        CompareSignature = compSignature
        ReferenceSignature = refSignature
    End Sub

    Public Property Comment As String
    Public Property CompareSignature As String
    Public Property ReferenceSignature As String
    Public Property ToBeChanged As Boolean

End Class


