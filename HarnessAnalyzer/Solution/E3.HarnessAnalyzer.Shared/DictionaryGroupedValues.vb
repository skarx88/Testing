Public Class DictionaryGroupedValues(Of TKey, TValue, TGroup As ICollectionGrouping(Of TKey, TValue))
    Inherits Dictionary(Of TKey, TGroup)
    Implements IDictionary(Of TKey, ICollection(Of TValue))
    Implements IEnumerable(Of IGrouping(Of TKey, TValue))

    Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of KeyValuePair(Of TKey, ICollection(Of TValue))).IsReadOnly
        Get
            Return False
        End Get
    End Property

    Private Property IDictionary_Item(key As TKey) As ICollection(Of TValue) Implements IDictionary(Of TKey, ICollection(Of TValue)).Item
        Get
            Return Me.Item(key)
        End Get
        Set(value As ICollection(Of TValue))
            Me.Item(key) = CType(value, TGroup)
        End Set
    End Property

    Private ReadOnly Property IDictionary_Keys As ICollection(Of TKey) Implements IDictionary(Of TKey, ICollection(Of TValue)).Keys
        Get
            Return MyBase.Keys
        End Get
    End Property

    Private ReadOnly Property IDictionary_Values As ICollection(Of ICollection(Of TValue)) Implements IDictionary(Of TKey, ICollection(Of TValue)).Values
        Get
            Return Me.Values.Cast(Of ICollection(Of TValue)).ToList
        End Get
    End Property

    Private ReadOnly Property ICollection_Count As Integer Implements ICollection(Of KeyValuePair(Of TKey, ICollection(Of TValue))).Count
        Get
            Return Me.Count
        End Get
    End Property

    Private Overloads Sub Add(key As TKey, value As ICollection(Of TValue)) Implements IDictionary(Of TKey, ICollection(Of TValue)).Add
        MyBase.Add(key, CType(value, TGroup))
    End Sub

    Private Overloads Sub Add(item As KeyValuePair(Of TKey, ICollection(Of TValue))) Implements ICollection(Of KeyValuePair(Of TKey, ICollection(Of TValue))).Add
        MyBase.Add(item.Key, CType(item.Value, TGroup))
    End Sub

    Private Sub ICollection_CopyTo(array() As KeyValuePair(Of TKey, ICollection(Of TValue)), arrayIndex As Integer) Implements ICollection(Of KeyValuePair(Of TKey, ICollection(Of TValue))).CopyTo
        CType(Me, ICollection).CopyTo(array, arrayIndex)
    End Sub

    Private Sub ICollection_Clear() Implements ICollection(Of KeyValuePair(Of TKey, ICollection(Of TValue))).Clear
        Me.Clear()
    End Sub

    Public Shadows Function TryGetValue(key As TKey, ByRef value As ICollection(Of TValue)) As Boolean Implements IDictionary(Of TKey, ICollection(Of TValue)).TryGetValue
        Dim result As TGroup = Nothing
        If MyBase.TryGetValue(key, result) Then
            value = result
            Return True
        Else
            value = Nothing
            Return False
        End If
    End Function

    Public Shadows Function ContainsKey(key As TKey) As Boolean Implements IDictionary(Of TKey, ICollection(Of TValue)).ContainsKey
        Return MyBase.ContainsKey(key)
    End Function

    Public Function Contains(item As KeyValuePair(Of TKey, ICollection(Of TValue))) As Boolean Implements ICollection(Of KeyValuePair(Of TKey, ICollection(Of TValue))).Contains
        Return MyBase.ContainsKey(item.Key) AndAlso item.Value.Equals(Me(item.Key))
    End Function

    Private Overloads Function Remove(item As KeyValuePair(Of TKey, ICollection(Of TValue))) As Boolean Implements ICollection(Of KeyValuePair(Of TKey, ICollection(Of TValue))).Remove
        Return MyBase.Remove(item.Key)
    End Function

    Private Function IDictionary_Remove(key As TKey) As Boolean Implements IDictionary(Of TKey, ICollection(Of TValue)).Remove
        Return MyBase.Remove(key)
    End Function

    Private Function IEnumerable_GetEnumerator() As IEnumerator(Of KeyValuePair(Of TKey, ICollection(Of TValue))) Implements IEnumerable(Of KeyValuePair(Of TKey, ICollection(Of TValue))).GetEnumerator
        Return MyBase.Select(Function(kv) New KeyValuePair(Of TKey, ICollection(Of TValue))(kv.Key, kv.Value)).GetEnumerator
    End Function

    Public Function AsIDictionary() As IDictionary(Of TKey, ICollection(Of TValue))
        Return CType(Me, IDictionary(Of TKey, ICollection(Of TValue)))
    End Function

    Public Function AsGroups() As IEnumerable(Of IGrouping(Of TKey, TValue))
        Return CType(Me, IEnumerable(Of IGrouping(Of TKey, TValue)))
    End Function

    Private Function IEnumerable_GetEnumerator1() As IEnumerator(Of IGrouping(Of TKey, TValue)) Implements IEnumerable(Of IGrouping(Of TKey, TValue)).GetEnumerator
        Return MyBase.Select(Function(kv) kv.Value).GetEnumerator
    End Function

    Protected Class Group
        Inherits HashSet(Of TValue)
        Implements ICollectionGrouping(Of TKey, TValue)
        Implements ICloneable

        Protected Sub New()
        End Sub

        Public Sub New(key As TKey)
            Me.Key = key
        End Sub

        Public ReadOnly Property Key As TKey Implements ICollectionGrouping(Of TKey, TValue).Key

        Public Function Clone() As Object Implements ICloneable.Clone
            Dim new_grp As Group = CType(Activator.CreateInstance(Me.GetType), Group)
            new_grp._Key = Me.Key
            For Each item As TValue In Me
                new_grp.Add(item)
            Next
            Return new_grp
        End Function

        Protected Overridable Function IGroupingList_Add(element As TValue) As Boolean Implements ICollectionGrouping(Of TKey, TValue).Add
            Return Me.Add(element)
        End Function

        Protected Overridable Function IGroupingList_Remove(element As TValue) As Boolean Implements ICollectionGrouping(Of TKey, TValue).Remove
            Return Me.Remove(element)
        End Function
    End Class

End Class
