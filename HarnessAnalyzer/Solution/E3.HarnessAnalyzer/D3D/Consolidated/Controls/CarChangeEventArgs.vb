Namespace D3D.Consolidated.Controls

    Public Class CarChangeEventArgs
        Inherits EventArgs

        Public Sub New(changeType As CarGroupChangeType)
            Me.ChangeType = changeType
        End Sub

        ReadOnly Property ChangeType As CarGroupChangeType

    End Class

End Namespace