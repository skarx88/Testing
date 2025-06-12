<Serializable()>
Public Class CheckedCompareResult_2023

    Public Property CompareDrawingName As String = String.Empty
    Public Property ReferenceDrawingName As String = String.Empty

    Private _checkedCompareResultEntries As New CheckedCompareResultEntryList_2023

    Public Sub New()
    End Sub

    Public ReadOnly Property CheckedCompareResultEntries() As CheckedCompareResultEntryList_2023
        Get
            Return _checkedCompareResultEntries
        End Get
    End Property

End Class

