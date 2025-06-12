Imports devDept.Eyeshot.Entities

Namespace Documents

    Public Class EntitiesEventArgs
        Inherits EventArgs

        Public Sub New(entities As IEnumerable(Of IEntity))
            Me.Entities = entities
        End Sub

        ReadOnly Property Entities As IEnumerable(Of IEntity)

    End Class

End Namespace