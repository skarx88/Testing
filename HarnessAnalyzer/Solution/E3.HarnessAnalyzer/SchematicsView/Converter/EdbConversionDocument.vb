Imports System.Collections.ObjectModel

Namespace Schematics.Converter

    Public Class EdbConversionDocument
        Inherits KeyedCollection(Of String, EdbConversionEntity)

        Private _originalIdToKey As New Concurrent.ConcurrentDictionary(Of String, HashSet(Of String))
        Private _documentId As String

        Public Sub New(documentId As String)
            _documentId = documentId
        End Sub

        Protected Overrides Function GetKeyForItem(item As EdbConversionEntity) As String
            Return item.Id
        End Function

        Public Function TryGetEntity(entityId As String, ByRef resultEntity As EdbConversionEntity) As Boolean
            Return Me.Dictionary.TryGetValue(entityId, resultEntity)
        End Function

        Public Function TryGetEntitiesByOriginalId(originalId As String, ByRef resultEntities As IList(Of EdbConversionEntity)) As Boolean
            Dim keyLst As HashSet(Of String) = Nothing
            If _originalIdToKey.TryGetValue(originalId, keyLst) Then
                resultEntities = New List(Of EdbConversionEntity)
                For Each entityId As String In keyLst
                    Dim entity As EdbConversionEntity = Nothing
                    If Me.TryGetEntity(entityId, entity) Then
                        resultEntities.Add(entity)
                    End If
                Next
                Return True
            End If
            Return False
        End Function

        Public Function ContainsOriginalId(originalId As String) As Boolean
            Return _originalIdToKey.ContainsKey(originalId)
        End Function

        Public Function TryAdd(entity As EdbConversionEntity) As Boolean
            If Not Me.Contains(GetKeyForItem(entity)) Then
                Me.Add(entity)
                Return True
            End If
            Return False
        End Function

        Protected Overrides Sub InsertItem(index As Integer, item As EdbConversionEntity)
            MyBase.InsertItem(index, item)
            OnAfterInsertItem(item)
        End Sub

        Private Sub AddOriginalIdMap(originalId As String, item As EdbConversionEntity)
            Dim lst As HashSet(Of String) = _originalIdToKey.GetOrAdd(originalId, Function() New HashSet(Of String))
            lst.Add(GetKeyForItem(item))
        End Sub

        Private Sub RemoveOriginalIdMap(originalId As String, item As EdbConversionEntity)
            Dim lst As HashSet(Of String) = _originalIdToKey.GetOrAdd(originalId, Function() New HashSet(Of String))
            lst.Remove(GetKeyForItem(item))
            If lst.Count = 0 Then
                _originalIdToKey.TryRemove(originalId, Nothing)
            End If
        End Sub

        Private Sub OnAfterInsertItem(entity As EdbConversionEntity)
            For Each originalId As String In entity.OriginalIds
                AddOriginalIdMap(originalId, entity)
            Next
        End Sub

        Protected Overrides Sub RemoveItem(index As Integer)
            Dim item As EdbConversionEntity = Me(index)
            MyBase.RemoveItem(index)
            OnAfterRemoveItem(item)
        End Sub

        Private Sub OnAfterRemoveItem(entity As EdbConversionEntity)
            For Each originalId As String In entity.OriginalIds
                RemoveOriginalIdMap(originalId, entity)
            Next
        End Sub

        Protected Overrides Sub ClearItems()
            MyBase.ClearItems()
            _originalIdToKey.Clear()
        End Sub

        ReadOnly Property DocumentId As String
            Get
                Return _documentId
            End Get
        End Property

    End Class

End Namespace