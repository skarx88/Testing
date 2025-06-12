Imports devDept.Eyeshot

Namespace D3D.Consolidated

    Public Class AdjustModeChangedEventArgs
        Public Sub New(omChangeType As ObjectManipulatorChangeType)
            Me.OmChangeType = omChangeType
        End Sub

        ReadOnly Property OmChangeType As devDept.Eyeshot.ObjectManipulatorChangeType

    End Class

End Namespace