Imports System.ComponentModel
Imports Zuken.E3.Lib.IO.Files

Namespace Documents

    Public Class DocumentWorkUnitWrappedWorkChunk
        Inherits WorkUnitWrappedWorkChunk
        Implements IDocumentWorkChunk

        Public Sub New(workChunk As IWorkChunk)
            MyBase.New(workChunk)
        End Sub

        Private ReadOnly Property ChunkType As DocumentWorkChunkType Implements IDocumentWorkChunk.ChunkType
            Get
                Return TryCast(Work, IDocumentWorkChunk).ChunkType
            End Get
        End Property

        Protected Overrides Sub OnReportProgress(e As System.ComponentModel.ProgressChangedEventArgs)
            Dim pWInfo As UserStateProgressInfo = TryCast(e.UserState, UserStateProgressInfo)
            If pWInfo IsNot Nothing Then
                If BackgroundWorker.IsBusy Then
                    BackgroundWorker.ReportProgress(e.ProgressPercentage, pWInfo.UserState) ' HINT: report the overall progress for all chunks combined
                End If
            Else
                MyBase.OnReportProgress(e)
            End If
        End Sub

    End Class

End Namespace