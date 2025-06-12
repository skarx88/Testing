Imports Zuken.E3.HarnessAnalyzer.D3D.Document

Namespace D3D.Consolidated.Controls

    Public Class BeforeInitializeEventArgs
        Inherits System.ComponentModel.CancelEventArgs

        Public Sub New(design As DocumentDesign)
            Me.Design = design
        End Sub

        ReadOnly Property Design As DocumentDesign

    End Class

End Namespace