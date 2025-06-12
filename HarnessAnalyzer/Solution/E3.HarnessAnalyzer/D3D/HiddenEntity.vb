Imports devDept.Eyeshot.Entities

Namespace D3D

    Public Class HiddenEntity
        Public OriginalColor As Color
        Private _entity As Entity

        Public Sub New(en As Entity, Optional alpha As Integer = 10)
            OriginalColor = en.Color
            en.Color = Color.FromArgb(alpha, OriginalColor)
            Entity = en
            Entity.Selectable = False
        End Sub

        Public Property Entity As Entity
            Get

                Return _entity
            End Get
            Set(value As Entity)
                _entity = value
            End Set
        End Property

        Public Sub Reset()
            Entity.Color = OriginalColor
            Entity.Selectable = True
        End Sub

    End Class

End Namespace