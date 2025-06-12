Imports System.Runtime.Serialization

'HINT: these are are skeleton (no further BL) classes to only hook into serialization process of the old version, making it possible retrieve the data (f.e. CollectionBase is no longer deserializable)
<Serializable()>
Public Class CheckedCompareResultEntryList_2023
    Implements ISerializable
    Implements IEnumerable(Of Object)

    Private _list As New List(Of Object)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(info As SerializationInfo, context As StreamingContext)
        For Each entry As SerializationEntry In info
            If TypeOf entry.Value Is ArrayList Then ' hint the old value was an arraylist
                _list.AddRange(CType(entry.Value, ArrayList).Cast(Of Object))
                Exit For
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