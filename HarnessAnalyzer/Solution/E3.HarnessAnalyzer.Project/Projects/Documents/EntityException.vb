Imports devDept.Eyeshot.Entities

Namespace Documents

    Public Class EntityException
        Inherits Exception

        Public Sub New(entity As IBaseEntity)
            MyBase.New()
            Me.Entity = entity
        End Sub

        Public Sub New(message As String, entity As IBaseEntity)
            MyBase.New(message)
            Me.Entity = entity
        End Sub

        Public Sub New(message As String, entity As IBaseEntity, innerException As Exception)
            MyBase.New(message, innerException)
            Me.Entity = entity
        End Sub

        ReadOnly Property Entity As IBaseEntity

    End Class

End Namespace