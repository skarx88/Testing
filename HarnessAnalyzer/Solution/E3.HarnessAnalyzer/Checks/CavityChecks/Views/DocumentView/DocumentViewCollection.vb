Imports System.ComponentModel

Namespace Checks.Cavities.Views.Document

    Public Class DocumentViewCollection
        Inherits BindingList(Of DocumentView)

        Public Event ActiveChanged(sender As Object, e As EventArgs)
        Public Event ActiveChanging(sender As Object, e As DocumentChangingEventArgs)
        Public Event Closing(sender As Object, e As DocumentEventArgs)
        Public Event Closed(sender As Object, e As DocumentEventArgs)

        Public Event ModelSelectionChanged(sender As Object, e As EventArgs)

        Private _dic As New Dictionary(Of String, DocumentView)
        Private _active As DocumentView
        Private _isActivating As Boolean = False

        Public Sub New()
            MyBase.New
        End Sub

        Public Overloads Function AddNew(name As String, id As String, kbl As KBLMapper, document As DocumentForm) As DocumentView
            Dim newDoc As New DocumentView(name, id, kbl, document)
            Me.Add(newDoc)
            Return newDoc
        End Function

        Public Overloads Function Contains(documentId As String) As Boolean
            Return _dic.ContainsKey(documentId)
        End Function

        Public Overloads Function Remove(documentID As String) As Boolean
            If _dic.ContainsKey(documentID) Then
                Dim doc As DocumentView = _dic(documentID)
                Return Me.Remove(doc)
            End If
            Return False
        End Function

        Default Overloads ReadOnly Property Item(id As String) As DocumentView
            Get
                Return _dic(id)
            End Get
        End Property

        Protected Overrides Sub SetItem(index As Integer, item As DocumentView)
            Throw New NotSupportedException
        End Sub

        Protected Overrides Sub ClearItems()
            For Each item As DocumentView In Me
                OnBeforeRemoveItem(item)
            Next
            MyBase.ClearItems()
        End Sub

        Protected Overrides Sub RemoveItem(index As Integer)
            Dim item As DocumentView = Me(index)
            OnBeforeRemoveItem(item)
            MyBase.RemoveItem(index)
        End Sub

        Protected Overrides Sub InsertItem(index As Integer, item As DocumentView)
            MyBase.InsertItem(index, item)
            OnAfterInsertItem(item)
        End Sub

        Private Sub OnAfterInsertItem(item As DocumentView)
            If item.Active Then
                Active = item
            End If
            _dic.Add(item.Id, item)
            AddHandler item.PropertyChanged, AddressOf _item_PropertyChanged
            AddHandler item.ModelSelectionChanged, AddressOf _item_ModelSelectionChanged
            AddHandler item.Closing, AddressOf _item_Closing
            AddHandler item.Closed, AddressOf _item_Closed
        End Sub

        Private Sub _item_Closed(sender As Object, e As EventArgs)
            Me.Remove(DirectCast(sender, DocumentView))
            OnDocumentClosed(Me, New DocumentEventArgs(DirectCast(sender, DocumentView)))
        End Sub

        Private Sub _item_Closing(sender As Object, e As EventArgs)
            OnDocumentClosing(Me, New DocumentEventArgs(DirectCast(sender, DocumentView)))
        End Sub

        Private Sub _item_PropertyChanged(sender As Object, e As PropertyChangedEventArgs)
            If e.PropertyName = NameOf(DocumentView.Active) Then
                Dim docView As DocumentView = DirectCast(sender, DocumentView)
                If docView.Active Then
                    Active = docView
                ElseIf Active Is docView Then
                    Active = Nothing
                End If
            End If
        End Sub

        Private Sub OnBeforeRemoveItem(item As DocumentView)
            If item.Active AndAlso Me.Active Is item Then
                Active = Nothing
            End If
            _dic.Remove(item.Id)
            RemoveHandler item.PropertyChanged, AddressOf _item_PropertyChanged
            RemoveHandler item.ModelSelectionChanged, AddressOf _item_ModelSelectionChanged
            RemoveHandler item.Closing, AddressOf _item_Closing
            RemoveHandler item.Closed, AddressOf _item_Closed
        End Sub

        Private Sub _item_ModelSelectionChanged(sender As Object, e As EventArgs)
            OnModelSelectionChanged(sender, e)
        End Sub

        Property Active As DocumentView
            Get
                Return _active
            End Get
            Set(value As DocumentView)
                If Not _isActivating Then
                    Try
                        _isActivating = True
                        If _active IsNot value Then
                            OnActiveChanging(Me, New DocumentChangingEventArgs(value))
                            Dim oldActive As DocumentView = _active
                            _active = value
                            If oldActive IsNot Nothing Then
                                oldActive.Active = False
                            End If
                            OnActiveChanged(Me, New EventArgs)
                        End If
                    Finally
                        _isActivating = False
                    End Try
                End If
            End Set
        End Property

        Protected Overridable Sub OnActiveChanged(sender As Object, e As EventArgs)
            RaiseEvent ActiveChanged(sender, e)
        End Sub

        Protected Overridable Sub OnActiveChanging(sender As Object, e As DocumentChangingEventArgs)
            RaiseEvent ActiveChanging(sender, e)
        End Sub

        Protected Overridable Sub OnModelSelectionChanged(sender As Object, e As EventArgs)
            RaiseEvent ModelSelectionChanged(sender, e)
        End Sub

        Protected Overridable Sub OnDocumentClosed(sender As Object, e As DocumentEventArgs)
            RaiseEvent Closed(sender, e)
        End Sub

        Protected Overridable Sub OnDocumentClosing(sender As Object, e As DocumentEventArgs)
            RaiseEvent Closing(sender, e)
        End Sub


    End Class

End Namespace