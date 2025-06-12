Imports devDept.Eyeshot
Imports devDept.Eyeshot.ObjectManipulatorKeyAndScaleManager

Namespace D3D.Shared
    Public Class RotationSettings
        Inherits VpRotationSettings

        Public Property Shortcuts As ShortcutSetting = New ShortcutSetting(Keys.Left, Keys.Right, Keys.Up, Keys.Down)
        Public Property FineShortcuts As ShortcutSetting = New ShortcutSetting(Keys.Shift Or Keys.Left, Keys.Shift Or Keys.Right, Keys.Shift Or Keys.Up, Keys.Shift Or Keys.Down)

        Public Sub [Set](model As devDept.Eyeshot.Design)
            With model
                For Each vp As Viewport In model.Viewports
                    vp.Rotate.KeysStep = DefaultKeyStep
                    With .ShortcutKeys
                        .RotateLeft = Shortcuts.Left
                        .RotateRight = Shortcuts.Right
                        .RotateUp = Shortcuts.Up
                        .RotateDown = Shortcuts.Down
                    End With
                Next
            End With
        End Sub

        Public Sub [Set](omMananger As devDept.Eyeshot.ObjectManipulatorKeyAndScaleManager)
            With omMananger
                With .RotationSettings
                    .DefaultKeyStep = Me.DefaultKeyStep
                    .FineKeyStep = Me.FineKeyStep
                    .Step = .Step
                End With
            End With
        End Sub

        Public Shared ReadOnly Property [Default] As RotationSettings
            Get
                Static _default As New RotationSettings
                Return _default
            End Get
        End Property

    End Class

End Namespace