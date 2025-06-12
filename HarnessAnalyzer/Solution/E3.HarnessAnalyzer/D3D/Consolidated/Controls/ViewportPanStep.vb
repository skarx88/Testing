Imports devDept.Eyeshot

Namespace D3D.Consolidated.Controls

    Public Class ViewportPanStep
        Inherits ViewportKeyStep

        Public Sub New(viewportIndex As Integer, keysStep As Double)
            MyBase.New(viewportIndex, keysStep)
        End Sub

        Public Overrides ReadOnly Property Type As StepType
            Get
                Return StepType.Pan
            End Get
        End Property

        Protected Overrides Sub SetViewportValue(vp As Viewport)
            vp.Pan.KeysStep = CInt(Me.KeysStep)
        End Sub

    End Class

End Namespace