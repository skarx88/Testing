Namespace Checks.Cavities.Views.Document

    Public Class DocumentChangingEventArgs
        Inherits EventArgs

        Public Sub New(newDocument As DocumentView)
            Me.NewDocument = newDocument
        End Sub

        ReadOnly Property NewDocument As DocumentView

    End Class

End Namespace