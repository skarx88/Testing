Imports System.Runtime.Serialization

<Serializable()>
<KnownType(GetType(CheckedCompareResultEntryList))>
Public Class CheckedCompareResult

    Private _checkedCompareResultEntries As New CheckedCompareResultEntryList

    Public Sub New()
        CompareDrawingName = String.Empty
        ReferenceDrawingName = String.Empty
    End Sub

    Public Sub New(compDrawingName As String, refDrawingName As String)
        Me.CompareDrawingName = compDrawingName
        Me.ReferenceDrawingName = refDrawingName
    End Sub

    Public ReadOnly Property CheckedCompareResultEntries() As CheckedCompareResultEntryList
        Get
            Return CType(_checkedCompareResultEntries, CheckedCompareResultEntryList)
        End Get
    End Property

    Public Property CompareDrawingName As String
    Public Property ReferenceDrawingName As String

End Class


