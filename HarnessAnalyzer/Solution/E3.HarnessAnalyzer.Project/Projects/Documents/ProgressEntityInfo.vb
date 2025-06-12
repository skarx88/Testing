Imports devDept.Eyeshot.Entities

Namespace Documents

    Public Class ProgressUpdateEntityInfo
        Inherits System.ComponentModel.CancelProgressInfo

        Public Sub New(state As System.ComponentModel.ProgressState, ParamArray entities() As IEntity)
            MyBase.New(state)
            Me.Entities = entities
        End Sub

        Public Shared Function GetUpdating(ParamArray entitiesUpdatedOrAdded() As IEntity) As ProgressUpdateEntityInfo
            Return New ProgressUpdateEntityInfo(System.ComponentModel.ProgressState.Updating, entitiesUpdatedOrAdded)
        End Function

        ReadOnly Property Entities As IEntity()
    End Class

End Namespace