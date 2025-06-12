Imports System.Runtime.Serialization

'HINT: in conclustion theese are skeleton (no further BL) classes to hook into serialization process of the old version, to retrieve the data (CollectionBase is no longer deserializable)
<Serializable()>
Public Class CheckedCompareResultList_2023
    Implements ISerializable
    Implements IEnumerable(Of Object)

    Private _list As New List(Of Object)

    Public Sub New()
    End Sub

    Public Sub New(info As SerializationInfo, context As StreamingContext)
        Me.New
        For Each entry As SerializationEntry In info
            If TypeOf entry.Value Is ICollection Then
                _list.AddRange(CType(entry.Value, ICollection).OfType(Of CheckedCompareResult_2023))
            End If
        Next
    End Sub

    Public Sub GetObjectData(info As SerializationInfo, context As StreamingContext) Implements ISerializable.GetObjectData
        Throw New NotSupportedException("Serialization write is no longer supported please use the new file version instead")
    End Sub

    Public Function GetEnumerator() As IEnumerator(Of Object) Implements IEnumerable(Of Object).GetEnumerator
        Return _list.GetEnumerator
    End Function

    Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return _list.GetEnumerator
    End Function

End Class