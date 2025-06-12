Imports System.ComponentModel

Namespace Documents

    Public Class VerticesProposalsResult

        Private _vertices As New List(Of VertexProposal)

        Public Sub New(resultType As ProposalsResultType)
            Me.ResultType = resultType
        End Sub

        Public Sub New(result As ProposalsResultType, vertices As IEnumerable(Of VertexProposal))
            Me.New(result)
            _vertices = New List(Of VertexProposal)(vertices)
        End Sub

        ReadOnly Property Vertices As IReadOnlyCollection(Of VertexProposal)
            Get
                Return _vertices
            End Get
        End Property

        ReadOnly Property ResultType As ProposalsResultType

    End Class

    Public Class AsyncVertexProposalsResult
        Inherits VerticesProposalsResult

        Public Sub New(result As ProposalsResultType)
            MyBase.New(result)
        End Sub

        Public Sub New(resultType As ProposalsResultType, vertices As IEnumerable(Of VertexProposal), result As IResult)
            MyBase.New(resultType, vertices)
            Me.Result = result
        End Sub

        ReadOnly Property Result As IResult

    End Class

    Public Enum ProposalsResultType
        NotExecuted = 0
        Success = 1
        NothingFound = 2
        Cancelled = 3
    End Enum

End Namespace