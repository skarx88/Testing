Imports Zuken.E3.Lib.IO.Files

Namespace Documents

    Public Interface IDocumentWorkChunk
        Inherits IWorkChunk

        ReadOnly Property ChunkType As DocumentWorkChunkType

    End Interface

End Namespace