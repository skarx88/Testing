Public Interface ITypeGroupedKblIds
    Inherits IEnumerable(Of IGrouping(Of KblObjectType, String))
    Inherits ICloneable

    Function ContainsValue(key As KblObjectType, value As String) As Boolean
    Function ContainsKey(key As KblObjectType) As Boolean
    Function TryGetValue(key As KblObjectType, ByRef value As IEnumerable(Of String)) As Boolean
    Function GetValueOrEmpty(key As KblObjectType) As IEnumerable(Of String)

End Interface
