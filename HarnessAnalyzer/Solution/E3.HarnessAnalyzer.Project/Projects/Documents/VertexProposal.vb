Namespace Documents

    Public Class VertexProposal

        Public Sub New(vertex As Zuken.E3.Lib.Model.Vertex, lengthDelta As Double, weightedLength As Double, rank As Integer, isStartSplice As Boolean)
            Me.Vertex = vertex
            Me.LengthDelta = lengthDelta
            Me.WeightedLength = weightedLength
            Me.Rank = rank
            Me.IsStartSplice = isStartSplice
        End Sub

        ReadOnly Property Vertex As Zuken.E3.Lib.Model.Vertex
        ReadOnly Property LengthDelta As Double
        ReadOnly Property WeightedLength As Double
        ReadOnly Property Rank As Integer
        ReadOnly Property IsStartSplice As Boolean

    End Class

End Namespace