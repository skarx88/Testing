Imports Zuken.E3.Lib.Eyeshot.Model

Public Class SelectRedliningEventArgs
    Inherits EventArgs

    Public Sub New(annotation As LeaderAndImageEx)
        MyBase.New()
        Me.Annotation = annotation
    End Sub

    Public Property Annotation As LeaderAndImageEx

End Class
