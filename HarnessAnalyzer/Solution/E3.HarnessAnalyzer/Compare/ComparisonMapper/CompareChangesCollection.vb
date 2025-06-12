
Public Class CompareChangesCollection
    Implements IEnumerable(Of ChangedItem)

    Private _modified As New Dictionary(Of String, ChangedItem)
    Private _deleted As New Dictionary(Of String, ChangedItem)
    Private _addedNew As New Dictionary(Of String, ChangedItem)

    Protected Function GetKeyForItem(item As ChangedItem) As String
        Return item.Key
    End Function

    Public Function AddOrReplaceNew(key As String, item As Object, Optional type As CompareChangeType = CompareChangeType.[New]) As ChangedItem
        Dim newItem As New ChangedItem(key, type, item)
        Remove(key, type)
        Me.Add(newItem)
        Return newItem
    End Function

    Public Function AddOrReplaceNew(key As String, kblIdRef As String, kblIdComp As String, item As Object, Optional type As CompareChangeType = CompareChangeType.[New]) As ChangedItem
        Dim newItem As New ChangedItem(key, kblIdRef, kblIdComp, type, item)
        Remove(key, type)
        Me.Add(newItem)
        Return newItem
    End Function

    Private Function Remove(key As String, type As CompareChangeType) As Boolean
        Return SelectDictionary(type).Remove(key)
    End Function

    Private Sub Add(item As ChangedItem)
        AddCore(SelectDictionary(item.Change), item)
    End Sub

    Protected Overridable Sub AddCore(dic As Dictionary(Of String, ChangedItem), item As ChangedItem)
        dic.Add(GetKeyForItem(item), item)
    End Sub

    Public Function AddOrReplaceNewDeleted(key As String, item As Object) As ChangedItem
        Return Me.AddOrReplaceNew(key, item, CompareChangeType.Deleted)
    End Function

    Public Function AddOrReplaceNewDeleted(key As String, kblIdRef As String, kblIdComp As String, item As Object) As ChangedItem
        Return Me.AddOrReplaceNew(key, kblIdRef, kblIdComp, item, CompareChangeType.Deleted)
    End Function

    Public Function AddOrReplaceNewModified(key As String, item As Object) As ChangedItem
        Return Me.AddOrReplaceNew(key, item, CompareChangeType.Modified)
    End Function

    Public Sub Clear()
        _modified.Clear()
        _deleted.Clear()
        _addedNew.Clear()
    End Sub

    ReadOnly Property HasModified As Boolean
        Get
            Return _modified.Count > 0
        End Get
    End Property

    ReadOnly Property HasDeleted As Boolean
        Get
            Return _deleted.Count > 0
        End Get
    End Property

    ReadOnly Property HasNew As Boolean
        Get
            Return _addedNew.Count > 0
        End Get
    End Property

    ReadOnly Property ModifiedItems As IEnumerable(Of Object)
        Get
            Return _modified.Values.Select(Function(cItem) cItem.Item)
        End Get
    End Property

    ReadOnly Property DeletedItems As IEnumerable(Of Object)
        Get
            Return _deleted.Values.Select(Function(cItem) cItem.Item)
        End Get
    End Property

    ReadOnly Property NewItems As IEnumerable(Of Object)
        Get
            Return _addedNew.Values.Select(Function(cItem) cItem.Item)
        End Get
    End Property

    Public Function ByChange(change As CompareChangeType) As IEnumerable(Of ChangedItem)
        Return SelectDictionary(change).Values
    End Function

    Private Function SelectDictionary(change As CompareChangeType) As Dictionary(Of String, ChangedItem)
        Select Case change
            Case CompareChangeType.Deleted
                Return _deleted
            Case CompareChangeType.Modified
                Return _modified
            Case CompareChangeType.New
                Return _addedNew
            Case Else
                Throw New NotImplementedException
        End Select
    End Function

    Public Function Contains(key As String, type As CompareChangeType) As Boolean
        Return Me.SelectDictionary(type).ContainsKey(key)
    End Function

    Public Function ContainsModified(key As String) As Boolean
        Return Me.Contains(key, CompareChangeType.Modified)
    End Function

    Public Function ContainsDeleted(key As String) As Boolean
        Return Me.Contains(key, CompareChangeType.Deleted)
    End Function

    Public Function ContainsNew(key As String) As Boolean
        Return Me.Contains(key, CompareChangeType.New)
    End Function

    Public Function ContainsAny(key As String) As Boolean
        Return _deleted.ContainsKey(key) OrElse _modified.ContainsKey(key) OrElse _addedNew.ContainsKey(key)
    End Function

    Public Function GetEnumerator() As IEnumerator(Of ChangedItem) Implements IEnumerable(Of ChangedItem).GetEnumerator
        Return _addedNew.Values.Concat(_deleted.Values.Concat(_modified.Values)).GetEnumerator
    End Function

    Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return GetEnumerator()
    End Function

    ReadOnly Property Count As Integer
        Get
            Return _addedNew.Count + _deleted.Count + _modified.Count
        End Get
    End Property

End Class

Public Class CompareChangesCollection(Of TNew, TDeleted, TModified)
    Inherits CompareChangesCollection

    Shadows ReadOnly Property ModifiedItems As IEnumerable(Of TModified)
        Get
            Return MyBase.ModifiedItems.OfType(Of TModified)
        End Get
    End Property

    Shadows ReadOnly Property DeletedItems As IEnumerable(Of TDeleted)
        Get
            Return MyBase.DeletedItems.OfType(Of TDeleted)
        End Get
    End Property

    Shadows ReadOnly Property NewItems As IEnumerable(Of TNew)
        Get
            Return MyBase.NewItems.OfType(Of TNew)
        End Get
    End Property

    Protected Overrides Sub AddCore(dic As Dictionary(Of String, ChangedItem), cItem As ChangedItem)
        Select Case cItem.Change
            Case CompareChangeType.Deleted
                If Not TypeOf cItem.Item Is TDeleted Then
                    ThrowInvalidItemType(Of TDeleted)(cItem.Item)
                End If
            Case CompareChangeType.Modified
                If Not TypeOf cItem.Item Is TModified Then
                    ThrowInvalidItemType(Of TDeleted)(cItem.Item)
                End If
            Case CompareChangeType.New
                If Not TypeOf cItem.Item Is TNew Then
                    ThrowInvalidItemType(Of TNew)(cItem.Item)
                End If
            Case Else
                Throw New NotImplementedException("Unimplemented changeType:" & cItem.Change.ToString)
        End Select
        MyBase.AddCore(dic, cItem)
    End Sub

    Private Sub ThrowInvalidItemType(Of TAllowed)(obj As Object)
        Throw New ArgumentException(String.Format("Item type ""{0}"" not allowed in this collection. Expected: ""{1}""", obj.GetType.Name, GetType(TAllowed).Name))
    End Sub

End Class

Public Class ChangedItem

    Public Sub New(key As String, changeType As CompareChangeType, item As Object)
        Me.Key = key
        Me.Change = changeType
        Me.Item = item
    End Sub

    Public Sub New(key As String, kblIdref As String, kblIdComp As String, changeType As CompareChangeType, item As Object)
        Me.Key = key
        Me.Change = changeType
        Me.Item = item
        Me.KblIdRef = kblIdref
        Me.KblIdComp = kblIdComp
    End Sub

    ReadOnly Property Key As String
    ReadOnly Property [Change] As CompareChangeType
    ReadOnly Property Item As Object
    ReadOnly Property KblIdRef As String
    ReadOnly Property KblIdComp As String

    ReadOnly Property Text As String
        Get
            Return GetText(Me.Change)
        End Get
    End Property

    Public Shared Function GetText(change As CompareChangeType, Optional inversed As Boolean = False) As String
        Select Case change
            Case CompareChangeType.Deleted
                Return If(inversed, GetText(CompareChangeType.[New]), "Deleted")
            Case CompareChangeType.Modified
                Return "Modified"
            Case CompareChangeType.New
                Return If(inversed, GetText(CompareChangeType.Deleted), "Added")
            Case Else
                Throw New NotImplementedException
        End Select
    End Function

    Public Shared Function ParseFromText(text As String) As CompareChangeType
        Dim changeType As Nullable(Of CompareChangeType) = TryParseFromText(text)
        If changeType.HasValue Then
            Return changeType.Value
        Else
            Throw New NotImplementedException(String.Format("Unknown text ""{0}"" to parse to type ""{1}""", text, GetType(CompareChangeType).Name))
        End If
    End Function

    Public Shared Function TryParseFromText(text As String) As Nullable(Of CompareChangeType)
        Select Case text.ToLower
            Case "deleted"
                Return CompareChangeType.Deleted
            Case "modified"
                Return CompareChangeType.Modified
            Case "added"
                Return CompareChangeType.New
            Case "unmodified"
                Return CompareChangeType.Unchanged
            Case Else
                Return Nothing
        End Select
    End Function

    Public Shared Function IsModified(changeText As String) As Boolean
        Dim parsed As Nullable(Of CompareChangeType) = TryParseFromText(changeText)
        Return parsed.HasValue AndAlso parsed.Value = CompareChangeType.Modified
    End Function

    Public Shared Function IsDeleted(changeText As String) As Boolean
        Dim parsed As Nullable(Of CompareChangeType) = TryParseFromText(changeText)
        Return parsed.HasValue AndAlso parsed.Value = CompareChangeType.Deleted
    End Function

    Public Shared Function IsNew(changeText As String) As Boolean
        Dim parsed As Nullable(Of CompareChangeType) = TryParseFromText(changeText)
        Return parsed.HasValue AndAlso parsed.Value = CompareChangeType.[New]
    End Function

    Public Shared Function IsNewOrDeleted(changeText As String) As Boolean
        Dim type As Nullable(Of CompareChangeType) = TryParseFromText(changeText)
        Return type.HasValue AndAlso (type.Value = CompareChangeType.New OrElse type.Value = CompareChangeType.Deleted)
    End Function

End Class

