Imports devDept.Eyeshot.Entities
Imports devDept.Eyeshot.Translators

Namespace D3D.Consolidated.Designs

    Public Class EntityData
        Inherits JTEntityData

        Private _owner As Entity
        Private _oldSelectable As Boolean

        Public Sub New(owner As Entity)
            MyBase.New(String.Empty, 0, String.Empty)
            _owner = owner
            _oldSelectable = owner.Selectable
            If TypeOf _owner.EntityData Is JTEntityData Then
                With CType(_owner.EntityData, JTEntityData)
                    Me.BodyId = .BodyId
                    Me.BodyName = .BodyName
                    Me.Id = .Id
                End With
            End If
        End Sub

        Public Sub SetSelectable(selectable As Boolean)
            If _owner.Selectable <> selectable Then
                _oldSelectable = _owner.Selectable
                _owner.Selectable = selectable
            End If
        End Sub

        Public Sub RevertSelectable()
            If _owner.Selectable <> _oldSelectable Then
                _owner.Selectable = _oldSelectable
            End If
        End Sub

    End Class

End Namespace