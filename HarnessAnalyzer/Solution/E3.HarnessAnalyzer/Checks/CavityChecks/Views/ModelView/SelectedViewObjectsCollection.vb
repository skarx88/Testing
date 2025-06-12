Imports System.Collections.Specialized

Namespace Checks.Cavities.Views.Model

    Public Class SelectedViewObjectsCollection
        Inherits ObjectModel.ObservableCollection(Of BaseView)

        Property EventsEnabled As Boolean = True

        Private _isResetting As Boolean

        Protected Overrides Sub OnCollectionChanged(e As NotifyCollectionChangedEventArgs)
            If EventsEnabled Then
                MyBase.OnCollectionChanged(e)
            End If
        End Sub

        Public Function Reset(objects As IEnumerable(Of BaseView)) As Boolean
            If Not _isResetting Then
                Try
                    _isResetting = True
                    Dim oldEventsEnabled As Boolean = Me.EventsEnabled
                    If objects.Any() Then
                        Me.EventsEnabled = False
                        Try
                            Me.Clear()
                            For Each item As BaseView In objects
                                Me.Add(item)
                            Next
                        Finally
                            Me.EventsEnabled = oldEventsEnabled
                        End Try
                        OnCollectionChanged(New NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset))
                    ElseIf Me.Count > 0 Then
                        Me.Clear()
                    End If
                Finally
                    _isResetting = False
                End Try
                Return True
            End If
            Return False
        End Function

    End Class

End Namespace