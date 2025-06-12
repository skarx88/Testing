Imports System.Collections.Specialized
Imports System.IO
Imports Zuken.E3.HarnessAnalyzer.Project.Documents

Namespace D3D.Consolidated.Controls

    Partial Public Class Consolidated3DControl

        Public Class DocumentDesignClonesCollection
            Inherits System.Collections.ObjectModel.HookableKeyedCollection(Of HcvDocument, DocumentDesignClone)
            Implements System.Collections.Specialized.INotifyCollectionChanged
            Implements IDisposable
            Implements IBaseFileProvider

            Private _disposedValue As Boolean

            Public Event CollectionChanged As NotifyCollectionChangedEventHandler Implements INotifyCollectionChanged.CollectionChanged
            Public Event BeforeHarnessInitialize(sender As Object, e As BeforeHarnessInitializeEventArgs)
            Public Event AfterHarnessInitialize(sender As Object, e As AfterHarnessInitializeEventArgs)

            Public Sub New(owner As Consolidated3DControl)
                MyBase.New
                Me.Owner = owner
            End Sub

            ReadOnly Property Owner As Consolidated3DControl

            Private ReadOnly Property IBaseFileProvider_File As IBaseFile Implements IBaseFileProvider.File
                Get
                    If TypeOf Me.Owner Is IBaseFileProvider Then
                        Return CType(Me.Owner, IBaseFileProvider).File
                    End If
                    If TypeOf Me.Owner Is IBaseFile Then
                        Return CType(Me.Owner, IBaseFile)
                    End If
                    Return Nothing
                End Get
            End Property

            Protected Overrides Function GetKeyForItem(item As DocumentDesignClone) As HcvDocument
                Return item.Document
            End Function

            Protected Overrides Sub AfterInsertItem(item As DocumentDesignClone)
                MyBase.AfterInsertItem(item)
                item.Owner = Me
                AddHandler item.AfterInitialize, AddressOf _item_AfterInitialize
                AddHandler item.BeforeInitialize, AddressOf _item_BeforeInitialize
                OnCollectionChanged(New NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item))
            End Sub

            Protected Overrides Sub AfterRemoveItem(item As DocumentDesignClone)
                MyBase.AfterRemoveItem(item)
                RemoveHandler item.AfterInitialize, AddressOf _item_AfterInitialize
                RemoveHandler item.BeforeInitialize, AddressOf _item_BeforeInitialize
                item.Owner = Nothing
                OnCollectionChanged(New NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item))
            End Sub

            Private Sub _item_BeforeInitialize(sender As Object, e As BeforeInitializeEventArgs)
                Dim args As New BeforeHarnessInitializeEventArgs(e.Design, DirectCast(sender, DocumentDesignClone))
                OnBeforeHarnessInitialize(args)
                e.Cancel = args.Cancel
            End Sub

            Private Sub _item_AfterInitialize(sender As Object, e As AfterInitializeEventArgs)
                Dim args As New AfterHarnessInitializeEventArgs(e.Design, DirectCast(sender, DocumentDesignClone))
                OnAfterHarnessInitialize(args)
                e.AddEntities = args.AddEntities
                e.RegenEntities = args.RegenEntities
            End Sub

            Public Function TryGet(doc As HcvDocument, ByRef har As DocumentDesignClone) As Boolean
                If Me.Dictionary IsNot Nothing Then
                    Return Me.Dictionary.TryGetValue(doc, har)
                End If

                For Each item As DocumentDesignClone In Me
                    If item.Document Is doc Then
                        har = item
                        Return True
                    End If
                Next

                Return False
            End Function

            Public Function TryRemove(doc As HcvDocument, ByRef har As DocumentDesignClone) As Boolean
                If TryGet(doc, har) Then
                    Return Me.Remove(har)
                End If
                Return False
            End Function

            Protected Overridable Sub OnBeforeHarnessInitialize(e As BeforeHarnessInitializeEventArgs)
                RaiseEvent BeforeHarnessInitialize(Me, e)
            End Sub

            Protected Overridable Sub OnAfterHarnessInitialize(e As AfterHarnessInitializeEventArgs)
                RaiseEvent AfterHarnessInitialize(Me, e)
            End Sub

            Protected Overrides Sub ClearItems()
                MyBase.ClearItems()
                OnCollectionChanged(New NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset))
            End Sub

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

    End Class

End Namespace
