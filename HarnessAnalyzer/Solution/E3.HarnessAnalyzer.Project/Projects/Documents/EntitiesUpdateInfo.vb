Imports Zuken.E3.Lib.Eyeshot.Model

Namespace Documents

    Public Class EntitiesUpdateInfo

        Public Sub New()
        End Sub

        Public Sub New(added As IEnumerable(Of IBaseModelEntity))
            Me.Added = added.ToList
        End Sub

        Property Added As New List(Of IBaseModelEntity)

    End Class

End Namespace