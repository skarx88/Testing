Imports System.Collections.ObjectModel

Namespace Schematics.Converter

    Public Class DocumentsCollection
        Inherits KeyedCollection(Of String, EdbConversionDocument)

        Protected Overrides Function GetKeyForItem(item As EdbConversionDocument) As String
            Return item.DocumentId
        End Function

        Public Function TryGetDocument(documentId As String, ByRef document As EdbConversionDocument) As Boolean
            If Me.Dictionary IsNot Nothing Then
                Return Me.Dictionary.TryGetValue(documentId, document)
            End If
            Return False
        End Function

        Public Function TryGetEntitiesByOriginalId(documentId As String, originalId As String, ByRef entities As IList(Of EdbConversionEntity)) As Boolean
            Dim coll As EdbConversionDocument = Nothing
            If TryGetDocument(documentId, coll) Then
                Return coll.TryGetEntitiesByOriginalId(originalId, entities)
            End If
            Return False
        End Function

        Public Function TryGetDocumentEntity(documentId As String, entityId As String, ByRef entity As EdbConversionEntity) As Boolean
            Dim coll As EdbConversionDocument = Nothing
            If TryGetDocument(documentId, coll) Then
                Return coll.TryGetEntity(entityId, entity)
            End If
            Return False
        End Function

        Public Function TryGetEntitiesByOriginalId(Of TEntity As EdbConversionEntity)(documentId As String, originalId As String, ByRef entities As List(Of TEntity)) As Boolean
            Dim itemsTemp As List(Of EdbConversionEntity) = Nothing
            Dim success As Boolean = TryGetEntitiesByOriginalId(documentId, originalId, itemsTemp)
            If success Then
                entities = itemsTemp.OfType(Of TEntity).ToList
            End If
            Return success
        End Function

        Public Function TryGetDocumentEntity(Of TEntity As EdbConversionEntity)(documentId As String, itemKey As String, ByRef entity As TEntity) As Boolean
            Dim resultEntity As TEntity = Nothing
            Dim success As Boolean = TryGetDocumentEntity(documentId, itemKey, resultEntity)
            entity = CType(resultEntity, TEntity)
            Return success
        End Function

        Public Function TryGetDocumentEntity(documentId As String, entityId As String) As EdbConversionEntity
            Dim result As EdbConversionEntity = Nothing
            Me.TryGetDocumentEntity(documentId, entityId, result)
            Return result
        End Function

        Public Function TryGetDocumentEntity(Of TEntity As EdbConversionEntity)(documentId As String, entityId As String) As TEntity
            Return CType(TryGetDocumentEntity(documentId, entityId), TEntity)
        End Function

        Public Function AddNewOrGet(documentId As String) As EdbConversionDocument
            If Me.Contains(documentId) Then
                Return Me(documentId)
            Else
                Dim newColl As New EdbConversionDocument(documentId)
                Me.Add(newColl)
                Return newColl
            End If
        End Function

    End Class

End Namespace