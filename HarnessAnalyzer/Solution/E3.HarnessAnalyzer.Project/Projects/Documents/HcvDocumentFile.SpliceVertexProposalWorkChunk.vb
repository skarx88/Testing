Imports System.ComponentModel
Imports Zuken.E3.Lib.IO.Files
Imports Zuken.E3.Lib.Locator.Splice
Imports Zuken.E3.Lib.Model

Namespace Documents

    Partial Public Class HcvDocument

        Public Class SpliceVertexProposalWorkChunk
            Inherits WorkChunkAsync

            Private WithEvents _spliceLocator As SpliceLocator
            Private _splice As Connector
            Private _prevPercent As Integer = -1

            Public Sub New(splice As Connector)
                MyBase.New("Detection of vertex proposals...", ProgressState.Reading)
                _splice = splice
                _spliceLocator = New SpliceLocator(splice.HostContainer.Model)
                Me.Maximum = 100
            End Sub

            ReadOnly Property Result As New VerticesProposalsResult(ProposalsResultType.NotExecuted)

            Protected Overrides Sub DoWorkCore()
                If _splice IsNot Nothing Then
                    Dim model As EESystemModel = _splice.HostContainer.Model
                    Dim targetConnectors As List(Of Connector) = model.Connectors.OfType(Of Connector).Where(Function(conn) conn.ConnectorType <> ConnectorType.Splice).ToList
                    Dim proposalResults As Dictionary(Of String, ProposalResult) = _spliceLocator.GetSplicePositionProposals(_splice, targetConnectors)
                    Dim spliceVertex As Vertex = _splice.GetVertices.Entries.FirstOrDefault
                    If proposalResults.Count > 1 AndAlso spliceVertex IsNot Nothing Then ' HINT: because the own splice-vertex-position is always given back (which is no real result for the user)
                        Dim list As New List(Of VertexProposal)
                        Dim spliceResults As ProposalResult = proposalResults.Item(spliceVertex.Id.ToString)
                        If spliceResults IsNot Nothing Then
                            Dim offset As Double = If(spliceResults.NotRoutedWireIds.Count = 0, proposalResults(spliceVertex.Id.ToString).WeightedLength, 0)
                            For Each item As KeyValuePair(Of String, ProposalResult) In proposalResults.Where(Function(r) r.Value.NotRoutedWireIds.Count = 0)
                                Dim vertexId As Guid = Guid.Parse(item.Value.VertexId)
                                Dim currentProposal As New VertexProposal(model.Vertices(vertexId), item.Value.WeightedLength - offset, item.Value.WeightedLength, item.Value.Rank, vertexId = spliceVertex.Id)
                                list.Add(currentProposal)
                                ThrowCancelIfRequested() 'HINT: no progressStep-execution here (currently no progress) -> we check for cancalltion manually
                            Next
                        End If
                        _Result = New VerticesProposalsResult(ProposalsResultType.Success, list)
                    Else
                        _Result = New VerticesProposalsResult(ProposalsResultType.NothingFound)
                    End If
                End If
            End Sub

            Private Sub _spliceLocator_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) Handles _spliceLocator.ProgressChanged
                If e.ProgressPercentage > _prevPercent Then
                    SyncLock Me
                        Dim diff As Integer = e.ProgressPercentage - _prevPercent
                        Me.ProgressStep(diff, New SpliceLocatorProgressStateInfo(Me.ProgressType, e.UserState))
                        _prevPercent = e.ProgressPercentage
                    End SyncLock
                End If
            End Sub

            Public Overrides Function GetResult() As Object
                Return Me.Result
            End Function

            Protected Overrides Sub Dispose(disposing As Boolean)
                MyBase.Dispose(disposing)
                If disposing Then
                    '_spliceLocator?.dispose ' maybe implement IDisposable-interface into SpliceLocator ?
                End If
                _spliceLocator = Nothing
                _prevPercent = Nothing
                _splice = Nothing
                _Result = Nothing
            End Sub

        End Class

        Public Async Function CalculateVertexProposalsAsync(splice As Connector) As Task(Of AsyncVertexProposalsResult)
            Dim chunk As New SpliceVertexProposalWorkChunk(splice)
            Dim result As IResult = Await Me.DoWorkChunksAsync(Nothing, Nothing, {chunk})
            If Not result.IsCancelled Then
                Return New AsyncVertexProposalsResult(chunk.Result.ResultType, chunk.Result.Vertices, result)
            Else
                Return New AsyncVertexProposalsResult(ProposalsResultType.Cancelled, New VertexProposal() {}, Nothing)
            End If
        End Function

        Public Function CancelWait() As Task
            Return MyBase.CancelWaitWorkChunksAsync(Nothing)
        End Function

    End Class

End Namespace