Imports devDept.Eyeshot
Imports devDept.Geometry

Namespace D3D.Shared

    Public Class ClippingPlaneSet

        Public Sub New(model As devDept.Eyeshot.Design)
            With model
                Me.Top = .ClippingPlane1
                Me.Front = .ClippingPlane2
                Me.Left = .ClippingPlane3
                Me.Bottom = .ClippingPlane4
                Me.Rear = .ClippingPlane5
                Me.Right = .ClippingPlane6
            End With
        End Sub

        Public Sub Init()
            With Me.Top
                .Normal = New Vector3D(0, 0, 1) 'Top
                .Distance = 50
            End With

            With Me.Front
                .Normal = New Vector3D(1, 0, 0) 'Front
                .Distance = 25
            End With

            With Me.Left
                .Normal = New Vector3D(0, -1, 0) 'Left
                .Distance = 25
            End With

            With Me.Bottom
                .Normal = New Vector3D(0, 0, -1) 'Bottom
                .Distance = 25
            End With

            With Me.Rear
                .Normal = New Vector3D(-1, 0, 0) 'Back
                .Distance = 25
            End With

            With Me.Right
                .Normal = New Vector3D(0, 1, 0) 'Right
                .Distance = 25
            End With
        End Sub

        Public Sub Init(center As Point3D, min As Point3D, max As Point3D)
            Dim dx As Double = (max.X - min.X) / 4
            Dim dy As Double = (max.Y - min.Y) / 4
            Dim dz As Double = (max.Z - min.Z) / 4

            With Me.Top
                .Normal = New Vector3D(0, 0, 1) 'Top
                .Distance = center.Z + dz
            End With

            With Me.Front
                .Normal = New Vector3D(1, 0, 0) 'Front
                .Distance = center.X + dx
            End With

            With Me.Left
                .Normal = New Vector3D(0, -1, 0) 'Left
                .Distance = center.Y - dy
            End With

            With Me.Bottom
                .Normal = New Vector3D(0, 0, -1) 'Bottom
                .Distance = center.Z - dz
            End With

            With Me.Rear
                .Normal = New Vector3D(-1, 0, 0) 'Rear
                .Distance = -(center.X - dx)
            End With

            With Me.Right
                .Normal = New Vector3D(0, 1, 0) 'Right
                .Distance = center.Y + dy
            End With
        End Sub


        ReadOnly Property Top As ClippingPlane
        ReadOnly Property Front As ClippingPlane
        ReadOnly Property Left As ClippingPlane
        ReadOnly Property Bottom As ClippingPlane
        ReadOnly Property Rear As ClippingPlane
        ReadOnly Property Right As ClippingPlane

    End Class
End Namespace