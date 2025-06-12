Imports System.Collections.ObjectModel
Imports System.Collections.Specialized
Imports Zuken.E3.App.Controls

Namespace Schematics.Converter

    Public MustInherit Class BaseChildrensEntity(Of TEdbItem As Connectivity.Model.BaseItem, TChild As EdbConversionEntity)
        Inherits EdbConversionEntity(Of TEdbItem)
        Implements IProvidesConvertedEdbChildrenIds

        Protected WithEvents _children As IdCollection(Of TChild)

        Public Sub New(blockId As String, originalSystemIds() As String, edbSysId As String, shortName As String, edbItem As TEdbItem, objectType As Connectivity.Model.ObjType)
            MyBase.New(blockId, originalSystemIds, edbSysId, shortName, edbItem, objectType)
            _children = GetNewChildrens()
        End Sub

        Public Sub New(blockId As String, originalSystemId As String, edbSysId As String, shortName As String, edbItem As TEdbItem, objectType As Connectivity.Model.ObjType)
            Me.New(blockId, New String() {originalSystemId}, edbSysId, shortName, edbItem, objectType)
        End Sub

        Public Sub New(blockId As String, originalSystemId As String, edbItem As TEdbItem)
            MyBase.New(blockId, originalSystemId, edbItem)
            _children = GetNewChildrens()
        End Sub

        Protected ReadOnly Property Children As IdCollection(Of TChild)
            Get
                Return _children
            End Get
        End Property

        Public Overrides Property IsVirtual As Boolean Implements IProvidesConvertedEdbChildrenIds.IsVirtual
            Get
                Return MyBase.IsVirtual
            End Get
            Set(value As Boolean)
                MyBase.IsVirtual = value
            End Set
        End Property

        Protected Overridable Function GetNewChildrens() As IdCollection(Of TChild)
            Return New BaseChildrensEntity(Of TEdbItem, TChild).IdCollection(Of TChild)
        End Function

        Protected Overridable Sub OnAfterChildrenCollectionItemsAdded(newItems As IEnumerable(Of TChild))
        End Sub

        Protected Overridable Function OnAddingNewItem(sysId As String, shortName As String, parameters() As Object) As TChild
            Return Nothing
        End Function

        Private Sub _children_CollectionChanged(sender As Object, e As NotifyCollectionChangedEventArgs) Handles _children.CollectionChanged
            If e.Action = NotifyCollectionChangedAction.Add Then
                OnAfterChildrenCollectionItemsAdded(e.NewItems.Cast(Of TChild))
            Else
                Throw New NotImplementedException("Action: " & e.Action.ToString)
            End If
        End Sub

        Private ReadOnly Property ChildrenIds As IEnumerable(Of String) Implements IProvidesConvertedEdbChildrenIds.ChildrenIds
            Get
                Return Me.Children
            End Get
        End Property

        Public Class ObserableCollection(Of T1)
            Inherits ObservableCollection(Of T1)

            Public Sub AddRange(items As IEnumerable(Of T1))
                For Each Item As T1 In items
                    MyBase.InsertItem(Me.Count, Item)
                Next
                Me.OnCollectionChanged(New NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items.ToList))
            End Sub

        End Class

        Public Class IdCollection(Of TConvertedItem As EdbConversionEntity)
            Implements IEnumerable(Of String)

            Friend Event CollectionChanged(sender As Object, e As NotifyCollectionChangedEventArgs)

            Private WithEvents _collection As New HashSet(Of String)

            Protected Function AddInternal(Item As TConvertedItem) As Boolean
                Return _collection.Add(GetId(Item))
            End Function

            Private Function GetId(item As TConvertedItem) As String
                Return item.Id
            End Function

            Public Function TryAdd(item As TConvertedItem) As Boolean
                If AddInternal(item) Then
                    OnCollectionChanged(New NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item))
                    Return True
                End If
                Return False
            End Function

            Public Sub Add(item As TConvertedItem)
                If Not TryAdd(item) Then
                    Throw New ArgumentException(String.Format("Entity with id ""{0}"" already added to collection!", GetId(item)))
                End If
            End Sub

            Public Sub AddRange(cavities As IEnumerable(Of TConvertedItem))
                Dim baseItems As List(Of TConvertedItem) = cavities.ToList
                For Each cItem As TConvertedItem In baseItems
                    _collection.Add(cItem.Id)
                Next
                OnCollectionChanged(New Specialized.NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, baseItems))
            End Sub

            Protected Overridable Sub OnCollectionChanged(e As NotifyCollectionChangedEventArgs)
                RaiseEvent CollectionChanged(Me, e)
            End Sub

            Public Overridable Sub Clear()
                _collection.Clear()
                RaiseEvent CollectionChanged(Me, New Specialized.NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset))
            End Sub

            Public Function GetEnumerator() As IEnumerator(Of String) Implements IEnumerable(Of String).GetEnumerator
                Return _collection.GetEnumerator
            End Function

            Private Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
                Return GetEnumerator()
            End Function

            ReadOnly Property Count As Integer
                Get
                    Return _collection.Count
                End Get
            End Property

        End Class

    End Class

    Public Interface IProvidesConvertedEdbChildrenIds
        ReadOnly Property ChildrenIds As IEnumerable(Of String)
        Property IsVirtual As Boolean
    End Interface

End Namespace
