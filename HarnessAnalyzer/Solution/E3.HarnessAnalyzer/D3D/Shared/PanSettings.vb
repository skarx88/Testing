Imports devDept.Eyeshot.ObjectManipulatorKeyAndScaleManager

Namespace D3D.Shared

    Public Class PanSettings
        Inherits VpPanSettings

        Public ReadOnly Property Shortcuts As New ShortcutSetting(Keys.Control Or Keys.Left, Keys.Control Or Keys.Right, Keys.Control Or Keys.Up, Keys.Control Or Keys.Down)
        Public ReadOnly Property FineShortcuts As New ShortcutSetting(Keys.Control Or Keys.Shift Or Keys.Left, Keys.Control Or Keys.Shift Or Keys.Right, Keys.Control Or Keys.Shift Or Keys.Up, Keys.Control Or Keys.Shift Or Keys.Down)

        Public Shared Shadows ReadOnly Property [Default] As PanSettings
            Get
                Static _default As New PanSettings
                Return _default
            End Get
        End Property

        Public Sub [Set](model As devDept.Eyeshot.Design)
            With model
                For Each vp As devDept.Eyeshot.Viewport In model.Viewports
                    vp.Pan.KeysStep = Me.KeyStep
                Next

                With .ShortcutKeys
                    .PanLeft = Shortcuts.Left
                    .PanRight = Shortcuts.Right
                    .PanUp = Shortcuts.Up
                    .PanDown = Shortcuts.Down
                End With
            End With
        End Sub

        Public Sub [Set](omMananger As devDept.Eyeshot.ObjectManipulatorKeyAndScaleManager)
            With omMananger
                With .PanSettings
                    .Factor = Me.Factor
                    .FineKeyStep = Me.FineKeyStep
                    .KeyStep = Me.KeyStep
                End With
            End With
        End Sub

    End Class

End Namespace