Public Class DictionaryKblIds(Of T)
    Implements IEnumerable(Of KeyValuePair(Of DictionaryKblKey, T))

    Private _kblIdToValueDic As New Dictionary(Of String, T)
    Private _systemIdToKeyMapper As New Dictionary(Of String, String)

    Public Sub Add(kblId As String, kblSystemId As String, value As T)
        _systemIdToKeyMapper.Add(kblSystemId, kblId)
        _kblIdToValueDic.Add(kblId, value)
    End Sub

    Public Sub Clear()
        _kblIdToValueDic.Clear()
        _systemIdToKeyMapper.Clear()
    End Sub

    Public Function Remove(key As String) As Boolean
        If _kblIdToValueDic.Remove(key) Then
            For Each kv As KeyValuePair(Of String, String) In _systemIdToKeyMapper.Where(Function(kv2) kv2.Value = key).ToArray
                _systemIdToKeyMapper.Remove(kv.Key)
            Next
            Return True
        End If
        Return False
    End Function

    Public Function RemoveBySystemId(kblSystemId As String) As Boolean
        If _systemIdToKeyMapper.ContainsKey(kblSystemId) Then
            Dim kblId As String = _systemIdToKeyMapper(kblSystemId)
            Return _kblIdToValueDic.Remove(kblId)
        End If
        Return False
    End Function

    Default ReadOnly Property Item(key As String) As T
        Get
            Return _kblIdToValueDic.Item(key)
        End Get
    End Property

    Public Function GetBySystemId(kblSystemId As String) As T
        If _systemIdToKeyMapper.ContainsKey(kblSystemId) Then
            Dim kblId As String = _systemIdToKeyMapper(kblSystemId)
            Return _kblIdToValueDic.Item(kblId)
        End If
        Return Nothing
    End Function

    Public Function ContainsSystemId(kblSystemId As String) As Boolean
        If _systemIdToKeyMapper.ContainsKey(kblSystemId) Then
            Dim kblId As String = _systemIdToKeyMapper(kblSystemId)
            Return _kblIdToValueDic.ContainsKey(kblId)
        End If
        Return False
    End Function

    Public Function ContainsKey(id As String) As Boolean
        Return _kblIdToValueDic.ContainsKey(id)
    End Function

    Public Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of DictionaryKblKey, T)) Implements IEnumerable(Of KeyValuePair(Of DictionaryKblKey, T)).GetEnumerator
        Dim list As New List(Of KeyValuePair(Of DictionaryKblKey, T))
        For Each kv As KeyValuePair(Of String, String) In _systemIdToKeyMapper
            Dim kblKey As New DictionaryKblKey(kv.Key, kv.Value)
            Dim value As T = _kblIdToValueDic(kv.Value)
            list.Add(New KeyValuePair(Of DictionaryKblKey, T)(kblKey, value))
        Next
        Return list.GetEnumerator
    End Function

    Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return GetEnumerator()
    End Function

End Class

Public Class DictionaryKblKey
    Public Sub New(systemId As String, kblId As String)
        Me.SystemId = systemId
        Me.KblId = kblId
    End Sub

    ReadOnly Property SystemId As String
    ReadOnly Property KblId As String
End Class