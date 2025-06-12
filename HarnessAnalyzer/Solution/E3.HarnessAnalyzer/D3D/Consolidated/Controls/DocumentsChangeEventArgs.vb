Namespace D3D.Consolidated.Controls

    Public Class DocumentsChangeEventArgs

        Public Sub New(changeType As DocumentsChangeType)
            Me.ChangeType = changeType
        End Sub

        ReadOnly Property ChangeType As DocumentsChangeType

    End Class

End Namespace