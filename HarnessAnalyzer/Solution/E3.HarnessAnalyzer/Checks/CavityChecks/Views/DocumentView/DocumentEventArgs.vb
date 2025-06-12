Namespace Checks.Cavities.Views.Document

    Public Class DocumentEventArgs
        Inherits EventArgs

        Public Sub New(document As DocumentView)
            Me.Document = document
        End Sub

        ReadOnly Property Document As DocumentView

    End Class

End Namespace