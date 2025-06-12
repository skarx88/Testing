Imports Zuken.E3.HarnessAnalyzer.D3D.Document

Namespace D3D.Consolidated.Controls

    Public Class AfterHarnessInitializeEventArgs
        Inherits AfterInitializeEventArgs

        Public Sub New(design As DocumentDesign, harness As Consolidated3DControl.DocumentDesignClone)
            MyBase.New(design)
            Me.Harness = harness
        End Sub

        ReadOnly Property Harness As Consolidated3DControl.DocumentDesignClone

    End Class

End Namespace
