Imports System.Collections.Specialized
Imports Zuken.E3.HarnessAnalyzer.Schematics.Controls
Imports Zuken.E3.HarnessAnalyzer.Schematics.Converter

Namespace Schematics

    Public Class SelectedEntities
        Inherits System.Collections.ObjectModel.Collection(Of EdbConversionEntity)
        Implements IDisposable
        Implements INotifyCollectionChanged

        Private WithEvents _owner As ViewControl
        Private _eventsEnabled As Boolean = True

        Public Event CollectionChanged(sender As Object, e As NotifyCollectionChangedEventArgs) Implements INotifyCollectionChanged.CollectionChanged

        Friend Sub New()
            MyBase.New()
        End Sub

        Public Function ResetTo(items As IEnumerable(Of EdbConversionEntity), Optional onlyWhenNotEqual As Boolean = False) As Boolean
            items = items.Where(Function(item) item IsNot Nothing).ToArray
            If Not onlyWhenNotEqual OrElse Not Me.SequenceEqual(items) Then
                Dim prevEnabled As Boolean = _eventsEnabled
                _eventsEnabled = False
                Try
                    Me.Clear()
                    Me.AddRange(items)
                    Return True
                Finally
                    _eventsEnabled = prevEnabled
                    OnCollectionChanged(New NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset))
                End Try
            End If
            Return False
        End Function

        Public Sub AddRange(items As IEnumerable(Of EdbConversionEntity))
            items = items.Where(Function(item) item IsNot Nothing).ToArray
            Dim prevEnabled As Boolean = _eventsEnabled
            _eventsEnabled = False
            Try
                For Each Item As EdbConversionEntity In items
                    Me.Add(Item)
                Next
            Finally
                _eventsEnabled = prevEnabled
            End Try
            OnCollectionChanged(New NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items.ToList))
        End Sub

        Protected Overrides Sub ClearItems()
            MyBase.ClearItems()
            OnCollectionChanged(New NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset))
        End Sub

        Protected Overrides Sub InsertItem(index As Integer, item As EdbConversionEntity)
            MyBase.InsertItem(index, item)
            OnCollectionChanged(New NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item))
        End Sub

        Protected Overrides Sub SetItem(index As Integer, item As EdbConversionEntity)
            Dim oldItem As EdbConversionEntity = MyBase.Items(index)
            MyBase.SetItem(index, item)
            OnCollectionChanged(New NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, oldItem))
        End Sub

        Protected Overrides Sub RemoveItem(index As Integer)
            Dim item As EdbConversionEntity = MyBase.Items(index)
            MyBase.RemoveItem(index)
            OnCollectionChanged(New NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item))
        End Sub

        Private Sub OnCollectionChanged(e As NotifyCollectionChangedEventArgs)
            If _eventsEnabled Then
                RaiseEvent CollectionChanged(Me, e)
            End If
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' So ermitteln Sie überflüssige Aufrufe

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                End If
                _owner = Nothing
            End If
            Me.disposedValue = True
        End Sub

        ' Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Ändern Sie diesen Code nicht. Fügen Sie oben in Dispose(disposing As Boolean) Bereinigungscode ein.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region


    End Class

End Namespace