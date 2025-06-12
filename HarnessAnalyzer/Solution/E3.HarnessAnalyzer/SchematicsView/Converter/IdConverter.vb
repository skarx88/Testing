Imports System.Text.RegularExpressions

Namespace Schematics.Converter

    Public Class IdConverter

        Private Const STARTC As Char = "["c
        Private Const CLOSEC As Char = "]"c

        Public Shared Function GetParsed(str As String) As MatchGroups
            Return New MatchGroups(str)
        End Function

        Public Shared Function GetCombined(ParamArray ids() As String) As String
            Dim res As New System.Text.StringBuilder
            For Each id As String In ids
                res.Append(GetEnclosed(id))
            Next
            Return res.ToString
        End Function

        Private Shared Function GetEnclosed(str As String) As String
            Return String.Format("{0}{1}{2}", STARTC, str, CLOSEC)
        End Function

        ''' <summary>
        ''' Gets entities by documentId and entity ids that are parsed from each edb id
        ''' </summary>
        ''' <param name="edbIds">Combined ids in format [documentId][ObjectId][...]</param>
        ''' <param name="resolveVirtual">When true: resolves also the child items when found entity is virtual</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function ResolveEntitiesFromDocuments(documents As DocumentsCollection, edbIds() As String, resolveVirtual As Boolean) As EdbConversionEntity()
            Dim entitesFound As New HashSet(Of EdbConversionEntity)
            If edbIds IsNot Nothing AndAlso edbIds.Length > 0 Then
                For Each combinedEntityId As String In edbIds
                    Dim entities As IList(Of EdbConversionEntity) = New List(Of EdbConversionEntity)
                    Dim ids As List(Of String) = IdConverter.GetParsed(combinedEntityId).AllAsValues.ToList

                    If ids.Count = 1 Then
                        For Each document As EdbConversionDocument In documents
                            If document.TryGetEntitiesByOriginalId(combinedEntityId, entities) Then
                                entitesFound.AddRange(ResolveEntitiesCore(documents, entities, resolveVirtual))
                            End If
                        Next
                    Else
                        Dim documentId As String = ids.First
                        ids.Remove(documentId)

                        Dim document As EdbConversionDocument = Nothing
                        If documents.TryGetDocument(documentId, document) Then
                            For Each entityId As String In ids
                                If document.TryGetEntitiesByOriginalId(entityId, entities) Then
                                    entitesFound.AddRange(ResolveEntitiesCore(documents, entities, resolveVirtual))
                                End If
                            Next
                        End If
                    End If
                Next
            End If
            Return entitesFound.ToArray
        End Function

        Private Shared Function ResolveEntitiesCore(blocks As DocumentsCollection, entities As IEnumerable(Of EdbConversionEntity), resolveVirtual As Boolean) As IEnumerable(Of EdbConversionEntity)
            Dim result As New HashSet(Of EdbConversionEntity)
            For Each entity As EdbConversionEntity In entities
                If resolveVirtual AndAlso entity.IsVirtual AndAlso TypeOf entity Is IProvidesConvertedEdbChildrenIds Then
                    result.AddRange(ResolveEntitiesFromDocuments(blocks, CType(entity, IProvidesConvertedEdbChildrenIds).ChildrenIds.ToArray))
                End If
                If entity IsNot Nothing Then
                    result.Add(entity)
                End If
            Next
            Return result
        End Function

        Public Shared Function ResolveEntitiesFromDocuments(blocks As DocumentsCollection, ParamArray edbIds() As String) As EdbConversionEntity()
            Return ResolveEntitiesFromDocuments(blocks, edbIds, True)
        End Function

#Region "GetDefaultEdbId"

        Public Shared Function GetDefaultEdbId(documentId As String, objectId As String) As String
            Return GetCombined(documentId, objectId)
        End Function

        Public Shared Function GetDefaultEdbSystemId(documentId As String, conn As Connector_occurrence) As String
            Return GetDefaultEdbId(documentId, conn.SystemId)
        End Function

        Public Shared Function GetDefaultEdbSystemId(documentId As String, cav As Cavity_occurrence) As String
            Return GetDefaultEdbId(documentId, cav.SystemId)
        End Function

        Public Shared Function GetDefaultEdbSystemId(documentId As String, cmp As Component_occurrence) As String
            Return GetDefaultEdbId(documentId, cmp.SystemId)
        End Function

        Public Shared Function GetDefaultEdbSystemId(documentId As String, wire As Wire_occurrence) As String
            Return GetDefaultEdbId(documentId, wire.SystemId)
        End Function

#End Region

        Public Class MatchGroups
            Inherits List(Of MatchGroups)

            Private _value As String

            Public Sub New(entry As String)
                _value = entry
                Dim matches As MatchCollection = Regex.Matches(entry, String.Format("\{0}((?>[^\{0}\{1}]+|\{0}(?<c>)|\{1}(?<-c>))*(?(c)(?!)))", STARTC, CLOSEC))
                For Each match As Match In matches
                    If Not String.IsNullOrEmpty(match.Groups(1).Value) Then
                        Dim grp As New MatchGroups(match.Groups(1).Value)
                        Me.Add(grp)
                    End If
                Next
            End Sub

            ReadOnly Property Value As String
                Get
                    Return _value
                End Get
            End Property

            Public Overrides Function ToString() As String
                Return Value
            End Function

            Public Function AllAsValues() As String()
                Dim lst As New List(Of String)
                If Me.Count = 0 Then
                    lst.Add(Me.Value)
                Else
                    For Each mGrp As MatchGroups In Me
                        lst.AddRange(mGrp.AllAsValues)
                    Next
                End If
                Return lst.ToArray
            End Function

        End Class

    End Class

End Namespace
