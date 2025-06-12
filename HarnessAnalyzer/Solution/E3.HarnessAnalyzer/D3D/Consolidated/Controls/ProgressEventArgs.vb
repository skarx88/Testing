Namespace D3D.Consolidated.Controls

    Public Class ProgressEventArgs
        Inherits System.ComponentModel.ProgressChangedEventArgs
        Public Sub New(progressPercentage As Integer)
            MyBase.New(progressPercentage, Nothing)
            ProgressId = Guid.NewGuid
        End Sub

        Public Sub New(progress As Integer, text As String)
            MyBase.New(progress, text)
            ProgressId = Guid.NewGuid
        End Sub

        Public Property ProgressId As Guid

    End Class

End Namespace