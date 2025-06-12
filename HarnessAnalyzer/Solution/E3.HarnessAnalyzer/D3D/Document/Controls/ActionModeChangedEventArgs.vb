Imports devDept.Eyeshot

Namespace D3D.Document.Controls


    Public Class ActionModeChangedEventArgs
        Inherits devDept.Eyeshot.ActionModeEventArgs

        Public Sub New(action As actionType, oldAction As actionType)
            MyBase.New(action)
            Me.OldActionMode = oldAction
        End Sub

        Public Sub New(action As actionType, button As DefaultToolBarButton, oldAction As actionType)
            MyBase.New(action, button)
            Me.OldActionMode = oldAction
        End Sub

        ReadOnly Property OldActionMode As actionType

    End Class

End Namespace
