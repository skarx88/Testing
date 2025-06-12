Imports Zuken.E3.Lib.Eyeshot.Model

Namespace D3D

    Public Class ComparerSelectionChangedEventArgs
        Inherits System.EventArgs

        Public Sub New(entity As IBaseModelEntityEx, kblMapperSourceId As String, srcIds As List(Of String), changedObject As ChangedObjectEx)
            Me.SelectedEntity = entity
            Me.Source = srcIds
            Me.ChangedObject = changedObject
            Me.KblMapperSourceId = kblMapperSourceId
        End Sub

        Public Sub New()
            Me.Source = New List(Of String)
        End Sub

        Public Property SelectedEntity As IBaseModelEntityEx
        Public Property Source As List(Of String)
        Public Property ChangedObject As ChangedObjectEx
        Public Property KblMapperSourceId As String
    End Class

End Namespace
