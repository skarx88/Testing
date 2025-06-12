Imports Zuken.E3.HarnessAnalyzer.Shared

Public Class TypeGroupedKblIdsDictionary
    Inherits [Shared].DictionaryGroupedValues(Of KblObjectType, String, ICollectionGrouping(Of KblObjectType, String))
    Implements ITypeGroupedKblIds

    Private Function ITypeGroupedKblIds_TryGetValue(key As KblObjectType, ByRef value As IEnumerable(Of String)) As Boolean Implements ITypeGroupedKblIds.TryGetValue
        Dim itemValue As ICollection(Of String) = Nothing
        If MyBase.TryGetValue(key, itemValue) Then
            value = itemValue
            Return True
        End If
        Return False
    End Function

    Public Function GetOrAddNew(key As KblObjectType) As ICollectionGrouping(Of KblObjectType, String)
        If Not Me.ContainsKey(key) Then
            Me.Add(key, New InternalGroup(key))
        End If
        Return Me(key)
    End Function

    Public Function GetValueOrEmpty(key As KblObjectType) As IEnumerable(Of String) Implements ITypeGroupedKblIds.GetValueOrEmpty
        Dim value As ICollection(Of String) = Nothing
        If Me.TryGetValue(key, value) Then
            Return value
        Else
            Return Enumerable.Empty(Of String)
        End If
    End Function

    Private Function ITypeGroupedKblIds_ContainsKey(key As KblObjectType) As Boolean Implements ITypeGroupedKblIds.ContainsKey
        Return MyBase.ContainsKey(key)
    End Function

    Public Overloads Function ContainsValue(key As KblObjectType, value As String) As Boolean Implements ITypeGroupedKblIds.ContainsValue
        Dim list As ICollection(Of String) = Nothing
        If MyBase.TryGetValue(key, list) Then
            Return list.Contains(value)
        End If
        Return False
    End Function

    Public Function Clone() As Object Implements ICloneable.Clone
        Dim new_clone As New TypeGroupedKblIdsDictionary
        For Each kv As KeyValuePair(Of KblObjectType, ICollectionGrouping(Of KblObjectType, String)) In Me
            Me.Add(kv.Key, If(TypeOf kv.Value Is ICloneable, CType(CType(kv.Value, ICloneable).Clone, ICollectionGrouping(Of KblObjectType, String)), kv.Value))
        Next
        Return new_clone
    End Function

    Private Class InternalGroup
        Inherits Group

        Protected Sub New()
            MyBase.New
        End Sub

        Public Sub New(key As KblObjectType)
            MyBase.New(key)
        End Sub

        Protected Overrides Function IGroupingList_Add(element As String) As Boolean
            Return MyBase.IGroupingList_Add(element) ' HINT: hooks to be done here
        End Function

        Protected Overrides Function IGroupingList_Remove(element As String) As Boolean
            Return MyBase.IGroupingList_Remove(element) ' HINT: hooks to be done here
        End Function

    End Class

End Class
