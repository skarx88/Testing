
Namespace Schematics.Converter.Kbl

    Partial Public Class KblEdbConverter

        Private Class ConnectorCavityBlocksInternal
            Inherits System.Collections.ObjectModel.KeyedCollection(Of String, ConnectorCavityDocumentBlock)
            Implements IDisposable

            Friend Sub New(data As KblEdbConverter.CombinedKblData)
                Me.Data = data
            End Sub

            Protected Overrides Function GetKeyForItem(item As ConnectorCavityDocumentBlock) As String
                Return item.DocumentId
            End Function

            Public Function AddOrUpdatePart(documentId As String, cavity As Cavity_occurrence) As InternalCavityPartGroup
                If Not Me.Contains(documentId) Then
                    Me.AddNew(documentId)
                End If

                Dim documentBlock As ConnectorCavityDocumentBlock = Me(documentId)
                Return documentBlock.AddOrUpdatePart(cavity)
            End Function

            Public Function AddNew(blockId As String) As ConnectorCavityDocumentBlock
                Dim cavDocumentBlock As New ConnectorCavityDocumentBlock(blockId)
                Me.Add(cavDocumentBlock)
                Return cavDocumentBlock
            End Function

            Protected Overrides Sub InsertItem(index As Integer, item As ConnectorCavityDocumentBlock)
                MyBase.InsertItem(index, item)
                item.Collection = Me
            End Sub

            Protected Overrides Sub RemoveItem(index As Integer)
                Me(index).Collection = Nothing
                MyBase.RemoveItem(index)
            End Sub

            Protected Overrides Sub ClearItems()
                For Each item As ConnectorCavityDocumentBlock In Me
                    item.Collection = Nothing
                Next
                MyBase.ClearItems()
            End Sub

            Public Function AddOrGet(documentId As String) As ConnectorCavityDocumentBlock
                If Not Me.Contains(documentId) Then
                    Me.AddNew(documentId)
                End If
                Return Me(documentId)
            End Function

            Public ReadOnly Property Data As KblEdbConverter.CombinedKblData

            Public Sub Dispose() Implements IDisposable.Dispose
                For Each docBlk As ConnectorCavityDocumentBlock In Me
                    For Each conn As InternalBlockConnector In docBlk
                        conn.Clear()
                    Next
                    docBlk.Clear()
                Next

                Clear()
                _Data = Nothing
            End Sub

            Public Function GetCavitiesOfPart(documentId As String, cavityPartResolve As Cavity_occurrence) As InternalCavityPartGroup
                If Me.Contains(documentId) Then
                    Dim docBlk As ConnectorCavityDocumentBlock = Me(documentId)
                    Dim connector As Connector_occurrence = Me.Data.ConnectorsOfCavity.TryGetDocumentObject(cavityPartResolve.SystemId, documentId)
                    If connector IsNot Nothing AndAlso docBlk.Contains(connector.SystemId) Then
                        Dim cn As InternalBlockConnector = docBlk(connector.SystemId)
                        Dim cavityPart As Cavity = CType(Me.Data.Parts.TryGetDocumentObject(cavityPartResolve.Part, documentId), Cavity)
                        If cn.Contains(cavityPart.SystemId) Then
                            Return cn(cavityPart.SystemId)
                        End If
                    End If
                End If
                Return Nothing
            End Function

        End Class

        Private Class ConnectorCavityDocumentBlock
            Inherits ObjectModel.KeyedCollection(Of String, InternalBlockConnector)

            Public Sub New(documentId As String)
                MyBase.New
                Me.DocumentId = documentId
            End Sub

            Protected Overrides Function GetKeyForItem(item As InternalBlockConnector) As String
                Return item.ConnectorId
            End Function

            Protected Overrides Sub InsertItem(index As Integer, item As InternalBlockConnector)
                MyBase.InsertItem(index, item)
                item.DocumentBlock = Me
            End Sub

            Protected Overrides Sub RemoveItem(index As Integer)
                Me(index).DocumentBlock = Nothing
                MyBase.RemoveItem(index)
            End Sub

            Protected Overrides Sub ClearItems()
                For Each entity As InternalBlockConnector In Me
                    entity.DocumentBlock = Nothing
                Next
                MyBase.ClearItems()
            End Sub

            Public Function AddOrUpdatePart(cavity As Cavity_occurrence) As InternalCavityPartGroup
                Dim conn As Connector_occurrence = Me.Collection.Data.ConnectorsOfCavity.TryGetDocumentObject(cavity.SystemId, DocumentId)
                If conn Is Nothing Then
                    Throw New ArgumentException($"Connector of cavity ""{cavity.SystemId}"" not found in document ""{DocumentId}""!")
                End If

                If Not Me.Contains(conn.SystemId) Then
                    Me.Add(New InternalBlockConnector(conn.SystemId))
                End If

                Return Me(conn.SystemId).AddOrUpdatePart(cavity)
            End Function

            ReadOnly Property DocumentId As String
            Property Collection As ConnectorCavityBlocksInternal

        End Class

        Private Class InternalBlockConnector
            Inherits ObjectModel.KeyedCollection(Of String, InternalCavityPartGroup)

            Public Sub New(connectorId As String)
                Me.ConnectorId = connectorId
            End Sub

            ReadOnly Property ConnectorId As String

            Property DocumentBlock As ConnectorCavityDocumentBlock

            Public Function AddOrUpdatePart(cavity As Cavity_occurrence) As InternalCavityPartGroup
                Dim cavityPart As Cavity = CType(Me.DocumentBlock.Collection.Data.Parts.TryGetDocumentObject(cavity.Part, Me.DocumentBlock.DocumentId), Cavity)
                If cavityPart Is Nothing Then
                    Throw New ArgumentException($"Cavity part of cavity ""{cavity.SystemId}"" not found in document ""{Me.DocumentBlock.DocumentId}""!")
                End If

                If Not Me.Contains(cavityPart.SystemId) Then
                    Me.Add(New InternalCavityPartGroup(cavityPart.SystemId))
                End If

                Dim partGroup As InternalCavityPartGroup = Me(cavityPart.SystemId)

                If Not partGroup.Contains(cavity.SystemId) Then
                    partGroup.Add(cavity)
                End If

                Return partGroup
            End Function

            Protected Overrides Function GetKeyForItem(item As InternalCavityPartGroup) As String
                Return item.PartId
            End Function

        End Class

        Private Class InternalCavityPartGroup
            Inherits ObjectModel.KeyedCollection(Of String, Cavity_occurrence)

            Public Sub New(partId As String)
                MyBase.New
                Me.PartId = partId
            End Sub

            Protected Overrides Function GetKeyForItem(item As Cavity_occurrence) As String
                Return item.SystemId
            End Function

            ReadOnly Property PartId As String

        End Class

    End Class

End Namespace