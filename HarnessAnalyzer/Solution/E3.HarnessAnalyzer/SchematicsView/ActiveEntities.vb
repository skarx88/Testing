Imports System.Collections.Specialized
Imports Zuken.E3.App.Controls
Imports Zuken.E3.HarnessAnalyzer.Schematics.Controls

Namespace Schematics

    Public Class ActiveEntities
        Inherits ObjectModel.ObservableCollection(Of ActiveEntity)
        Implements IDisposable

        Private _control As ViewControl
        Private _disposedValue As Boolean

        Friend Sub New(control As ViewControl)
            _control = control
        End Sub

        Public Function AddNew(ParamArray ids() As String) As IEnumerable(Of ActiveEntity)
            Dim entities As New List(Of ActiveEntity)

            For Each id As String In ids
                Dim result As Tuple(Of String, ActiveObjectType) = GetNameAndType(id)
                Dim entity As New ActiveEntity(id, result.Item1, result.Item2)
                entities.Add(entity)
            Next

            Me.AddRange(entities)
            Return entities
        End Function

        Public Sub AddRange(items As IEnumerable(Of ActiveEntity))
            Dim oldEnabled As Boolean = EventsEnabled
            Try
                EventsEnabled = False
                For Each entity As ActiveEntity In items
                    Me.Add(entity)
                Next
            Finally
                EventsEnabled = oldEnabled
                If items.Any() Then
                    OnCollectionChanged(New NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items))
                End If
            End Try
        End Sub

        Protected Overrides Sub OnCollectionChanged(e As NotifyCollectionChangedEventArgs)
            If EventsEnabled Then
                MyBase.OnCollectionChanged(e)
            End If
        End Sub

        Friend Function GetNameAndType(id As String) As Tuple(Of String, ActiveObjectType)
            Dim name As String = String.Empty
            Dim type As ActiveObjectType = ActiveObjectType.None

            With _control
                If .TryGetModuleName(id, name) Then
                    type = ActiveObjectType.Module
                ElseIf .TryGetFunctionName(id, name) Then
                    type = ActiveObjectType.Function
                Else
                    Dim item As Connectivity.Model.BaseItem = .TryGetEdbItems(id).FirstOrDefault
                    If item IsNot Nothing Then
                        type = CType(item.Type, ActiveObjectType)
                        name = item.Name
                    End If
                End If
            End With
            Return New Tuple(Of String, ActiveObjectType)(name, type)
        End Function

        ReadOnly Property Control As ViewControl
            Get
                Return _control
            End Get
        End Property

        Property EventsEnabled As Boolean = True

        Protected Overrides Sub ClearItems()
            Dim items As ActiveEntity() = Me.ToArray
            MyBase.ClearItems()
            For Each item As ActiveEntity In items
                item.Dispose()
            Next
        End Sub

        Protected Overrides Sub RemoveItem(index As Integer)
            Dim item As ActiveEntity = Me(index)
            MyBase.RemoveItem(index)
            item.Dispose()
        End Sub

        Protected Overrides Sub InsertItem(index As Integer, item As ActiveEntity)
            MyBase.InsertItem(index, item)
            item.Owner = Me
        End Sub

        Protected Overrides Sub SetItem(index As Integer, item As ActiveEntity)
            Me.RemoveAt(index)
            Me.InsertItem(index, item)
        End Sub

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not _disposedValue Then
                If disposing Then
                    Me.Clear()
                End If
                _control = Nothing
                _disposedValue = True
            End If
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub

    End Class

End Namespace
