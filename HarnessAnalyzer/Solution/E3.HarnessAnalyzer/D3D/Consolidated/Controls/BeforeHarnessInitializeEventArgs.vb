Imports Zuken.E3.HarnessAnalyzer.D3D.Document

Namespace D3D.Consolidated.Controls

    Public Class BeforeHarnessInitializeEventArgs
        Inherits BeforeInitializeEventArgs

        Public Sub New(model As DocumentDesign, harness As Consolidated3DControl.DocumentDesignClone)
            MyBase.New(model)
            Me.Harness = harness
        End Sub

        ReadOnly Property Harness As Consolidated3DControl.DocumentDesignClone

    End Class

End Namespace