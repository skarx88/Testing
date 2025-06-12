Namespace D3D.Shared

    Public Class ShortcutSetting
        Public Sub New(left As Keys, right As Keys, up As Keys, down As Keys)
            Me.Left = left
            Me.Right = right
            Me.Up = up
            Me.Down = down
        End Sub

        Public Left As Keys
        Public Right As Keys
        Public Up As Keys
        Public Down As Keys

    End Class

End Namespace