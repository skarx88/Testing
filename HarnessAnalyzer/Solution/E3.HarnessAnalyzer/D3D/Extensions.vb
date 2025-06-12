Imports System.Drawing.Imaging
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports devDept.Eyeshot
Imports devDept.Eyeshot.Entities
Imports devDept.Geometry
Imports Zuken.E3.HarnessAnalyzer.D3D.Consolidated.Designs

Namespace D3D

    <HideModuleName>
    Friend Module Extensions

        <Extension>
        Public Function IsValidPoint(pt As Point3D) As Boolean
            Dim ok As Boolean
            If Not Double.IsInfinity(pt.X) OrElse Not Double.IsInfinity(pt.Y) OrElse Not Double.IsInfinity(pt.Z) Then
                ok = True
            End If
            Return ok
        End Function

        <Extension>
        Public Function IsInfinity(size As Size3D) As Boolean
            Return Double.IsInfinity(size.Diagonal)
        End Function

        <Extension>
        Public Function HasValidBox(ent As Entity) As Boolean
            Return ent.BoxMax IsNot Nothing AndAlso ent.BoxMin IsNot Nothing AndAlso ent.BoxSize IsNot Nothing AndAlso Not ent.BoxSize.IsInfinity
        End Function

        <Extension()>
        Public Function Subtract(fromPoint As Point3D, point As Point3D) As Point3D
            Return New Point3D(fromPoint.X - point.X, fromPoint.Y - point.Y, fromPoint.Z - point.Z)
        End Function

        <Extension>
        Public Function Subtract(box1 As devDept.Eyeshot.BoundingBox, box2 As devDept.Eyeshot.BoundingBox) As Size3D
            Dim bb As New BoundingBox
            Return New Size3D(box1.Min.Subtract(box2.Min), box1.Max.Subtract(box2.Max))
        End Function

        <Extension>
        Public Function IsNullOrEmpty(trans As devDept.Geometry.Transformation) As Boolean
            Static emptyTrans As Double() = New devDept.Geometry.Transformation().Matrix.Cast(Of Double).ToArray
            Return trans Is Nothing OrElse trans.Matrix.Cast(Of Double).SequenceEqual(emptyTrans)
        End Function

        <Extension>
        Public Function GetVolume(bb As BoundingBox) As Double
            Return New Size3D(bb.Min, bb.Max).GetVolume
        End Function

        <Extension>
        Public Function GetVolume(size As Size3D) As Double
            Return size.X * size.Y * size.Z
        End Function

        <Extension()>
        Public Function Add(point1 As Point3D, point2 As Point3D) As Point3D
            Return New Point3D(point1.X + point2.X, point1.Y + point2.Y, point1.Z + point2.Z)
        End Function

        <Extension()>
        Public Function GetTranslation(m(,) As Double) As Point3D
            Return New Point3D(CType(m.GetValue(0, 3), Double), CType(m.GetValue(1, 3), Double), CType(m.GetValue(2, 3), Double))
        End Function

        <Extension>
        Public Function GetTranslation(trans As devDept.Geometry.Transformation) As Point3D
            Return GetTranslation(trans.Matrix)
        End Function

        <Extension()>
        Public Function ToVector3D(p As Point3D) As Vector3D
            Return New Vector3D(p.X, p.Y, p.Z)
        End Function

        <Extension()>
        Public Function ToPoint3D(v As Vector3D) As Point3D
            Return New Point3D(v.X, v.Y, v.Z)
        End Function

        <Extension>
        Public Function GetInverted(trans As devDept.Geometry.Transformation) As devDept.Geometry.Transformation
            Dim inverted As devDept.Geometry.Transformation = CType(trans.Clone, devDept.Geometry.Transformation)
            inverted.Invert()
            Return inverted
        End Function

        <Extension()>
        Public Function ScalePoint(p As Point3D, axis As Point3D) As Point3D
            Dim mypoint As New Point3D(p.X * axis.X, p.Y * axis.Y, p.Z * axis.Z)
            Return mypoint
        End Function

        <Extension()>
        Public Function ScalePoint(p As Point3D, x As Double, y As Double, z As Double) As Point3D
            Dim mypoint As New Point3D(p.X * x, p.Y * y, p.Z * z)
            Return mypoint
        End Function

        <Extension()>
        Public Function TranslatePoint(p As Point3D, axis As Point3D) As Point3D
            Dim trans As New devDept.Geometry.Transformation
            trans.Translation(p.ToVector3D)
            Return trans * p
        End Function

        <Extension()>
        Public Function RotXPoint(p As Point3D, angleInRadians As Double) As Point3D
            Dim trans As New devDept.Geometry.Transformation
            trans.Rotation(angleInRadians, Vector3D.AxisX)
            Return trans * p
        End Function

        <Extension()>
        Public Function RotYPoint(p As Point3D, angleInRadians As Double) As Point3D
            Dim trans As New devDept.Geometry.Transformation
            trans.Rotation(angleInRadians, Vector3D.AxisY)
            Return trans * p
        End Function

        <Extension()>
        Public Function RotZPoint(p As Point3D, angleInRadians As Double) As Point3D
            Dim trans As New devDept.Geometry.Transformation
            trans.Rotation(angleInRadians, Vector3D.AxisZ)
            Return trans * p
        End Function

        <Extension()>
        Public Function RotXVector(v As Vector3D, angleInRadians As Double) As Vector3D
            Dim trans As New devDept.Geometry.Transformation
            trans.Rotation(angleInRadians, Vector3D.AxisX)
            Return trans * v
        End Function

        <Extension()>
        Public Function RotYVector(v As Vector3D, angleInRadians As Double) As Vector3D
            Dim trans As New devDept.Geometry.Transformation
            trans.Rotation(angleInRadians, Vector3D.AxisY)
            Return trans * v
        End Function

        <Extension()>
        Public Function RotZVector(v As Vector3D, angleInRadians As Double) As Vector3D
            Dim trans As New devDept.Geometry.Transformation
            trans.Rotation(angleInRadians, Vector3D.AxisZ)
            Return trans * v
        End Function

        <Extension()>
        Public Function RotVector(v As Vector3D, p As Point3D) As Vector3D
            Dim transX, transY, transZ As New devDept.Geometry.Transformation
            transX.Rotation(p.X, Vector3D.AxisX)
            transY.Rotation(p.Y, Vector3D.AxisY)
            transZ.Rotation(p.Z, Vector3D.AxisZ)
            Return transX * transY * transZ * v
        End Function

        <Extension()>
        Public Function TransVector(v As Vector3D, p As Point3D) As Vector3D
            Dim trans As New devDept.Geometry.Transformation
            trans.Translation(New Vector3D(p.X, p.Y, p.Z))
            Return trans * v
        End Function

        <Extension()>
        Public Sub ForEach(Of T)(items As IEnumerable(Of T), action As Action(Of T))
            For Each item As T In items
                action.Invoke(item)
            Next
        End Sub

        <Extension>
        Friend Function GetStartupTransformation(vp As Viewport) As devDept.Geometry.Transformation
            Dim myAxis As Vector3D = New Vector3D
            Dim myAngle As Double
            Dim myQuat As Quaternion = vp.Camera.Rotation()
            myQuat.ToAxisAngle(myAxis, myAngle)
            Dim startUpTrans As New devDept.Geometry.Transformation
            startUpTrans.Rotation(Utility.DegToRad(myAngle), myAxis)
            Return startUpTrans
        End Function

        <Extension>
        Public Function GetBoundingBox(points As IEnumerable(Of Point3D)) As BoundingBox
            Dim boxMin, boxMax As New Point3D
            UtilityEx.BoundingBox(points.ToList, boxMin, boxMax)
            Return New BoundingBox(boxMax, boxMin)
        End Function

        <Extension>
        Public Function GetBoundingBox(entities As IEnumerable(Of IEntity)) As BoundingBox
            For Each ent As Entity In entities
                ent.UpdateBoundingBox(New TraversalParams())
            Next
            Return GetBoundingBox(entities.OfType(Of Entity).Where(Function(ent) ent.HasValidBox).SelectMany(Function(ent) New Point3D() {ent.BoxMin, ent.BoxMax}))
        End Function

        <Extension>
        Friend Function GetSize(box As BoundingBox) As Size3D
            With box
                Return New Size3D(.Max.X - .Min.X, .Max.Y - .Min.Y, .Max.Z - .Min.Z)
            End With
        End Function

        <Extension>
        Friend Sub Invalidate(entity As Entity, model As devDept.Eyeshot.Design)
            Dim min As Point3D = model.WorldToScreen(entity.BoxMin)
            Dim max As Point3D = model.WorldToScreen(entity.BoxMax)
            model.Invalidate()
            model.Invalidate(New System.Drawing.Rectangle(CInt(min.X), CInt(min.Y), CInt(max.X - min.X), CInt(max.Y - min.Y)))
        End Sub

        <Extension>
        Friend Sub Invalidate(entities As IEnumerable(Of Entity), model As devDept.Eyeshot.Design)
            If entities.Any() Then
                Dim box As BoundingBox = entities.GetBoundingBox
                Dim min As Point3D = model.WorldToScreen(box.Min)
                Dim max As Point3D = model.WorldToScreen(box.Max)
                model.Invalidate(New Rectangle(CInt(min.X), CInt(min.Y), CInt(max.X - min.X), CInt(max.Y - min.Y)))
            End If
        End Sub

        <Extension>
        Public Sub MoveTo(entities As IEnumerable(Of Entity), fromPoint As Point3D, toPoint As Point3D)
            Dim delta As Vector3D = Vector3D.Subtract(toPoint, fromPoint)
            MoveTo(entities, delta)
        End Sub

        <Extension>
        Public Sub MoveTo(entities As IEnumerable(Of Entity), distance As Vector3D)
            If distance.Length > 0 Then
                For Each entity As Entity In entities
                    entity.Translate(distance)
                Next
            End If
        End Sub

        <Extension>
        Public Sub MoveTo(entity As Entity, fromPoint As Point3D, toPoint As Point3D)
            Dim delta As Vector3D = Vector3D.Subtract(toPoint, fromPoint)
            entity.Translate(delta)
        End Sub

        <Extension>
        Public Sub SetSelectable(entities As IEnumerable(Of IEntity), selectable As Boolean)
            For Each entity As Entity In entities.OfType(Of Entity)
                If Not TypeOf entity.EntityData Is EntityData Then
                    entity.EntityData = New EntityData(entity)
                End If
                DirectCast(entity.EntityData, EntityData).SetSelectable(selectable)
            Next
        End Sub

        <Extension>
        Public Sub RevertSelectable(entities As IEnumerable(Of IEntity))
            For Each entity As Entity In entities.OfType(Of Entity)
                If TypeOf entity.EntityData Is EntityData Then
                    DirectCast(entity.EntityData, EntityData).RevertSelectable()
                End If
            Next
        End Sub

        <Extension>
        Public Function TryPopAll(Of T)(stack As System.Collections.Concurrent.ConcurrentStack(Of T), ByRef result() As T) As Integer
            Dim count As Integer = 0
            If Not stack.IsEmpty Then
                Dim items(stack.Count - 1) As T
                count = stack.TryPopRange(items)
                result = items
                Return count
            End If
            Return count
        End Function

        <Extension>
        Public Function SetPropertyItemValue(img As Bitmap, id As Integer, value As String) As PropertyItem
            Dim propItem As System.Drawing.Imaging.PropertyItem
            If Not img.PropertyIdList.Contains(id) Then
                propItem = CreatePropertyItem(id) ' Get description metadata
            Else
                propItem = img.GetPropertyItem(id)
            End If

            propItem.Type = 2
            Dim msgData As Byte() = System.Text.Encoding.UTF8.GetBytes(value)
            propItem.Len = msgData.Length
            propItem.Value = msgData
            img.SetPropertyItem(propItem)
            Return propItem
        End Function

        Private Function CreatePropertyItem(id As Integer) As PropertyItem
            Dim ci As Type = GetType(PropertyItem)
            Dim o As ConstructorInfo = ci.GetConstructor(BindingFlags.NonPublic Or BindingFlags.Instance Or BindingFlags.[Public], Nothing, Array.Empty(Of Type)(), Nothing)

            Dim item As PropertyItem = DirectCast(o.Invoke(Nothing), PropertyItem)
            item.Id = id
            Return item
        End Function

        <Extension>
        Public Function ExtractRotation(trans As devDept.Geometry.Transformation) As devDept.Geometry.Transformation
            Dim rotTrans As New devDept.Geometry.Transformation

            rotTrans.Matrix.SetValue(trans.Matrix.GetValue(0, 0), 0, 0)
            rotTrans.Matrix.SetValue(trans.Matrix.GetValue(0, 1), 0, 1)
            rotTrans.Matrix.SetValue(trans.Matrix.GetValue(0, 2), 0, 2)

            rotTrans.Matrix.SetValue(trans.Matrix.GetValue(1, 0), 1, 0)
            rotTrans.Matrix.SetValue(trans.Matrix.GetValue(1, 1), 1, 1)
            rotTrans.Matrix.SetValue(trans.Matrix.GetValue(1, 2), 1, 2)

            rotTrans.Matrix.SetValue(trans.Matrix.GetValue(2, 0), 2, 0)
            rotTrans.Matrix.SetValue(trans.Matrix.GetValue(2, 1), 2, 1)
            rotTrans.Matrix.SetValue(trans.Matrix.GetValue(2, 2), 2, 2)

            Return rotTrans
        End Function

        <Extension>
        Public Function ExtractTranslation(trans As devDept.Geometry.Transformation) As devDept.Geometry.Transformation
            Dim tTrans As New devDept.Geometry.Transformation
            Dim extractT As devDept.Geometry.Point3D = trans.GetTranslation
            SetTranslation(tTrans, extractT.X, extractT.Y, extractT.Z)
            Return tTrans
        End Function

        <Extension>
        Public Function ExceptTranslation(trans As devDept.Geometry.Transformation) As devDept.Geometry.Transformation
            Dim tCopy As devDept.Geometry.Transformation = CType(trans.Clone, devDept.Geometry.Transformation)
            Dim ident As New Identity
            SetTranslation(tCopy, CDbl(ident.Matrix.GetValue(0, 3)), CDbl(ident.Matrix.GetValue(1, 3)), CDbl(ident.Matrix.GetValue(2, 3)))
            Return tCopy
        End Function

        <Extension>
        Public Function ExceptRotation(trans As devDept.Geometry.Transformation) As devDept.Geometry.Transformation
            Dim tCopy As devDept.Geometry.Transformation = CType(trans.Clone, devDept.Geometry.Transformation)
            Dim ident As New Identity
            With tCopy
                .Matrix.SetValue(ident.Matrix.GetValue(0, 0), 0, 0)
                .Matrix.SetValue(ident.Matrix.GetValue(0, 1), 0, 1)
                .Matrix.SetValue(ident.Matrix.GetValue(0, 2), 0, 2)

                .Matrix.SetValue(ident.Matrix.GetValue(1, 0), 1, 0)
                .Matrix.SetValue(ident.Matrix.GetValue(1, 1), 1, 1)
                .Matrix.SetValue(ident.Matrix.GetValue(1, 2), 1, 2)

                .Matrix.SetValue(ident.Matrix.GetValue(2, 0), 2, 0)
                .Matrix.SetValue(ident.Matrix.GetValue(2, 1), 2, 1)
                .Matrix.SetValue(ident.Matrix.GetValue(2, 2), 2, 2)
            End With
            Return tCopy
        End Function

        Private Sub SetTranslation(trans As devDept.Geometry.Transformation, x As Double, y As Double, z As Double)
            With trans.Matrix
                .SetValue(x, 0, 3)
                .SetValue(y, 1, 3)
                .SetValue(z, 2, 3)
                .SetValue(1, 3, 3)
            End With
        End Sub

        <Extension>
        Public Function CrossProduct(vector1 As Vector3D, vector2 As Vector3D) As Vector3D
            Return New Vector3D(vector1.Y * vector2.Z - vector1.Z * vector2.Y,
                                vector1.Z * vector2.X - vector1.X * vector2.Z,
                                vector1.X * vector2.Y - vector1.Y * vector2.X)
        End Function

        <Extension>
        Public Function AsNegated(vector As Vector3D) As Vector3D
            vector.Negate()
            Return vector
        End Function

    End Module

End Namespace