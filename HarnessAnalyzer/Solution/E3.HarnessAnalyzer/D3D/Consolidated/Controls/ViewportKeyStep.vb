Imports devDept.Eyeshot

Namespace D3D.Consolidated.Controls

    Public MustInherit Class ViewportKeyStep

        Public Sub New(viewportIndex As Integer, keysStep As Double)
            Me.ViewportIndex = viewportIndex
            Me.KeysStep = keysStep
        End Sub

        Public Sub [Set](model As Design)
            If model.Viewports.Count > 0 AndAlso (ViewportIndex < model.Viewports.Count) Then
                SetViewportValue(model.Viewports(Me.ViewportIndex))
            End If
        End Sub

        Protected MustOverride Sub SetViewportValue(vp As Viewport)
        ReadOnly Property KeysStep As Double
        ReadOnly Property ViewportIndex As Integer
        MustOverride ReadOnly Property Type As StepType


    End Class

End Namespace