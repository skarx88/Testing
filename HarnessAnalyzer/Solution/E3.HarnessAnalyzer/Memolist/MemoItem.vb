Imports System.ComponentModel

<Serializable()> _
Public Class MemoItem

    Private _objectType As String

    Public Property Id As String
    Public Property Name As String

    Public Property Type As String
        Get
            Return _objectType
        End Get
        Set(value As String)
            If Not String.IsNullOrEmpty(value) Then
                _objectType = [Lib].Schema.Kbl.Utils.ParseKblObjectTypeOrNearest(value).ToString
            Else
                _objectType = KblObjectType.Undefined.ToString
            End If
        End Set
    End Property

    Public Property Comment As String
    Public Property IsSelected As Boolean

    Public Sub New()
        Comment = String.Empty
        Id = String.Empty
        IsSelected = False
        Name = String.Empty
        Type = String.Empty
    End Sub

    Public Sub New(objectId As String, objectName As String, objectType As String, objectComment As String, isObjectSelected As Boolean)
        Comment = objectComment
        Id = objectId
        IsSelected = isObjectSelected
        Name = objectName
        Me.Type = objectType
    End Sub

End Class

Public Class MemoItemList
    Inherits BindingList(Of MemoItem)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub AddMemoItem(memoItem As MemoItem)
        If (Not ContainsMemoItem(memoItem)) Then
            Me.Add(memoItem)
        End If
    End Sub

    Public Function ContainsMemoItem(memoItem As MemoItem) As Boolean
        If (Where(Function(memItem) memItem.Id = memoItem.Id).Any()) Then
            Return True
        End If

        Return False
    End Function

    Public Sub DeleteMemoItem(memoItem As MemoItem)
        Me.Remove(memoItem)
    End Sub

    Public Function FindMemoItem(id As String) As MemoItem
        If (Where(Function(memItem) memItem.Id = id).Any()) Then
            Return Me.Where(Function(memItem) memItem.Id = id)(0)
        Else
            Return Nothing
        End If
    End Function

    Public Function GetIds() As IEnumerable(Of String)
        Return Me.Select(Function(item) item.Id)
    End Function

End Class