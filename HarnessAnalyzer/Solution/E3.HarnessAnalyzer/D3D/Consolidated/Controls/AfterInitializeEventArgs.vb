Imports Zuken.E3.HarnessAnalyzer.D3D.Document

Namespace D3D.Consolidated.Controls

    Public Class AfterInitializeEventArgs
        Inherits EventArgs

        Public Sub New(design As DocumentDesign)
            Me.Design = design
        End Sub

        ReadOnly Property Design As DocumentDesign
        Property RegenEntities As Boolean = True
        Property AddEntities As Boolean = True

    End Class

End Namespace
