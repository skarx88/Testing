Imports System.Collections.Specialized

Namespace Schematics.Converter.Kbl

    Public Class ConnectorInfoCollection
        Inherits System.Collections.ObjectModel.KeyedCollection(Of String, ConnectorInfo)
        Implements System.Collections.Specialized.INotifyCollectionChanged

        Public Event CollectionChanged(sender As Object, e As Specialized.NotifyCollectionChangedEventArgs) Implements Specialized.INotifyCollectionChanged.CollectionChanged

        Public Sub New()
            MyBase.New()
        End Sub

        Friend Sub New(initialConnector As ConnectorInfo)
            MyBase.InsertItem(Me.Count, initialConnector)
        End Sub

        Protected Overrides Function GetKeyForItem(item As ConnectorInfo) As String
            Return item.Id
        End Function

        Protected Overrides Sub InsertItem(index As Integer, item As ConnectorInfo)
            MyBase.InsertItem(index, item)
            OnCollectionChanged(New NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item))
        End Sub

        Protected Overrides Sub RemoveItem(index As Integer)
            Dim oldItem As ConnectorInfo = Me.Items(index)
            MyBase.RemoveItem(index)
            OnCollectionChanged(New NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem))
        End Sub

        Protected Overrides Sub SetItem(index As Integer, item As ConnectorInfo)
            Dim oldItem As ConnectorInfo = Me.Items(index)
            MyBase.SetItem(index, item)
            OnCollectionChanged(New NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, oldItem))
        End Sub

        Protected Overrides Sub ClearItems()
            For Each Item As ConnectorInfo In Me.ToList
                Me.Remove(Item)
            Next
        End Sub

        Protected Overridable Sub OnCollectionChanged(e As Specialized.NotifyCollectionChangedEventArgs)
            RaiseEvent CollectionChanged(Me, e)
        End Sub

        Public Function TryAdd(item As ConnectorInfo) As Boolean
            If Not Me.Contains(item) Then
                Me.Add(item)
                Return True
            End If
            Return False
        End Function

    End Class

End Namespace