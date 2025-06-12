Imports System.Collections.Specialized
Imports Zuken.E3.HarnessAnalyzer.D3D.Consolidated.Controls
Imports Zuken.E3.HarnessAnalyzer.Project.Documents

Namespace D3D.Document.Controls

    Public Class ClonedDocuments

        Inherits System.Collections.ObjectModel.HookableKeyedCollection(Of HcvDocument, DocumentClone)
        Implements System.Collections.Specialized.INotifyCollectionChanged
        Implements IDisposable

        Private _disposedValue As Boolean

        Public Event CollectionChanged As NotifyCollectionChangedEventHandler Implements INotifyCollectionChanged.CollectionChanged
        Public Event BeforeHarnessInitialize(sender As Object, e As BeforeHarnessInitializeEventArgs)
        Public Event AfterHarnessInitialize(sender As Object, e As AfterHarnessInitializeEventArgs)

        Public Sub New(owner As D3DComparerCntrl)
            MyBase.New
            Me.Owner = owner
        End Sub

        ReadOnly Property Owner As D3DComparerCntrl

        Protected Overrides Function GetKeyForItem(item As DocumentClone) As HcvDocument
            Return item.Document
        End Function


        'Public Function TryGet(doc As HcvDocument, ByRef har As DocumentClone) As Boolean
        '    If Me.Dictionary IsNot Nothing Then
        '        Return Me.Dictionary.TryGetValue(doc, har)
        '    End If

        '    For Each item As DocumentClone In Me
        '        If item.Document Is doc Then
        '            har = item
        '            Return True
        '        End If
        '    Next

        '    Return False
        'End Function

        'Protected Overridable Sub OnBeforeHarnessInitialize(e As BeforeHarnessInitializeEventArgs)
        '    RaiseEvent BeforeHarnessInitialize(Me, e)
        'End Sub

        'Protected Overridable Sub OnAfterHarnessInitialize(e As AfterHarnessInitializeEventArgs)
        '    RaiseEvent AfterHarnessInitialize(Me, e)
        'End Sub

        'Protected Overrides Sub ClearItems()
        '    MyBase.ClearItems()
        '    OnCollectionChanged(New NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset))
        'End Sub

        Protected Sub OnCollectionChanged(e As Specialized.NotifyCollectionChangedEventArgs)
            RaiseEvent CollectionChanged(Me, e)
        End Sub

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not _disposedValue Then
                If disposing Then
                    Me.Clear()
                End If

                Me._Owner = Nothing
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