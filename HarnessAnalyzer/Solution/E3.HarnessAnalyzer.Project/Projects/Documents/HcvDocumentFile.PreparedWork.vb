Imports System.ComponentModel
Imports Zuken.E3.Lib.Eyeshot.Model.Converter
Imports Zuken.E3.Lib.IO.Files

Namespace Documents

    Partial Public Class HcvDocument

        Public Class PreparedWork
            Implements IDisposable

            Private WithEvents _doc As HcvDocument
            Private _providedChunks As New List(Of IWorkChunk)
            Private _content As LoadContentType
            Private _result As IResult
            Private _disposedValue As Boolean

            Friend Sub New(skipLoadAsyncResult As IResult)
                _result = skipLoadAsyncResult
            End Sub

            Friend Sub New(doc As HcvDocument, content As LoadContentType, ParamArray chunksToProgress As IWorkChunk())
                _doc = doc
                _content = content
                For Each c As IWorkChunk In chunksToProgress
                    _providedChunks.Add(c)
                Next
            End Sub

            Private Sub OnPostStart()
                _doc._LoadedContent = _doc.LoadedContent Or _content
                If _content.HasFlag(LoadContentType.Entities) Then
                    _doc._converted = CType((_providedChunks.OfType(Of IDocumentWorkChunk).Where(Function(chunk) chunk.ChunkType = DocumentWorkChunkType.ConvertModel).SingleOrDefault?.GetResult), ConvertModelResult)
                End If
            End Sub

            Public Async Function LoadAsync() As Task(Of IResult)
                If _result IsNot Nothing Then
                    Return _result
                End If

                If _providedChunks?.Count = 0 Then
                    Return Result.Cancelled($"Cancelled loading, because there are chunks to Load. Provided content was ""{ _content.ToString}""")
                End If

                If _providedChunks?.Count > 0 Then
                    Try
                        Dim result As IResult = Await _doc.DoWorkChunksAsync(Nothing, Nothing, _providedChunks.ToArray)
                        If Not result.IsFaultedOrCancelled Then
                            OnPostStart()
                        End If
                        Return result
                    Catch ex As Exception
                        Return New Result(ex)
                    End Try
                End If
                Return ComponentModel.Result.Success
            End Function

            Protected Overridable Sub Dispose(disposing As Boolean)
                If Not _disposedValue Then
                    If disposing Then
                        For Each wc As IWorkChunk In _providedChunks.ToArray
                            wc.Dispose()
                        Next
                    End If

                    _doc = Nothing
                    _providedChunks = Nothing
                    _result = Nothing
                    _content = Nothing
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