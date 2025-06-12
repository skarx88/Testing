Namespace Documents

    Public Class EntitiesUpdatedEventArgs
        Inherits EventArgs

        Public Sub New(info As EntitiesUpdateInfo)
            MyBase.New
            Me.Info = info
        End Sub

        ReadOnly Property Info As EntitiesUpdateInfo

    End Class

End Namespace