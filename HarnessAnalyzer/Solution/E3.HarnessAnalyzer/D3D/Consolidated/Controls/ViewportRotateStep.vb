Imports devDept.Eyeshot

Namespace D3D.Consolidated.Controls
    Public Class ViewportRotateStep
        Inherits ViewportKeyStep

        Public Sub New(viewportIndex As Integer, keysStep As Double)
            MyBase.New(viewportIndex, keysStep)
        End Sub

        Public Overrides ReadOnly Property Type As StepType
            Get
                Return StepType.Rotate
            End Get
        End Property

        Protected Overrides Sub SetViewportValue(vp As Viewport)
            vp.Rotate.KeysStep = Me.KeysStep
        End Sub

    End Class

End Namespace
