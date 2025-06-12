Imports System.ComponentModel

Namespace Documents

    Public Class DocumentResult
        Inherits [Lib].IO.Files.WorkChunkResult

        Protected Sub New()
            MyBase.New
        End Sub

        Public Sub New(result As IResult, document As HcvDocument)
            MyBase.New(result, TryCast(result, [Lib].IO.Files.WorkChunkResult).WorkChunk)
            Me.Document = document
        End Sub

        ReadOnly Property Document As HcvDocument

        Public Shared Shadows ReadOnly Property Success(document As HcvDocument) As DocumentResult
            Get
                Dim result As DocumentResult = CreateSuccess(Of DocumentResult)()
                result._Document = document
                Return result
            End Get
        End Property

        Public Shared Shadows ReadOnly Property Cancelled(Optional message As String = "", Optional document As HcvDocument = Nothing) As DocumentResult
            Get
                Dim result As DocumentResult = CreateCancelled(Of DocumentResult)(message)
                result._Document = document
                Return result
            End Get
        End Property

        Public Shared Shadows ReadOnly Property Faulted(message As String, Optional document As HcvDocument = Nothing) As DocumentResult
            Get
                Dim result As DocumentResult = CreateFaulted(Of DocumentResult)(message)
                result._Document = document
                Return result
            End Get
        End Property

    End Class

End Namespace