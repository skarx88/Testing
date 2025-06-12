Imports devDept.Geometry
Imports Zuken.E3.Lib.Eyeshot.Model


Namespace D3D.Document.Controls

    Partial Public Class Document3DControl
        Friend Sub AddHintLabel(entity As BaseModelEntity, text As String)
            Dim pt As Point3D = Nothing
            If TryCast(entity, TubularBaseEntity) IsNot Nothing Then
                pt = CType(entity, TubularBaseEntity).GetAnnotationAnchorPointFromPercent(0.6)
            Else
                pt = entity.GetBoundingBoxCenter
            End If
            Dim isHidden As Boolean
            If _d3d.HiddenEntities.Select(Function(e) e.Entity).ToList.Contains(entity) Then
                isHidden = True
            End If

            Dim an As New IndicatorAnnotation(entity, pt, GetOffset(entity), text, Color.FromArgb(200, Color.White))
            entity.Annotations.Add(an)
            an.Alignment = ContentAlignment.BottomRight
            an.BorderColor = Color.DarkGray
            an.IsHidden = isHidden

            _hintLabels.Add(entity.Id, an)
            If isHidden Then HideHintLabel(entity.Id)
        End Sub
        Friend Sub ClearHintLabels()
            For Each item As KeyValuePair(Of String, IndicatorAnnotation) In _hintLabels
                Dim entity As BaseModelEntity = devDept.Eyeshot.Entities.Flatten(_document.Entities.GetByEEObjectId(New Guid(item.Key))).Cast(Of BaseModelEntity).FirstOrDefault
                If entity IsNot Nothing Then entity.Annotations.Remove(item.Value)
            Next
            _hintLabels.Clear()
            Model3DControl1.Design.Refresh()
        End Sub

        Private Sub ShowHintLabels(includeHiddens As Boolean)
            If includeHiddens Then
                For Each item As KeyValuePair(Of String, IndicatorAnnotation) In _hintLabels
                    Dim entity As BaseModelEntity = devDept.Eyeshot.Entities.Flatten(_document.Entities.GetByEEObjectId(New Guid(item.Key))).Cast(Of BaseModelEntity).FirstOrDefault
                    If entity IsNot Nothing Then
                        For Each entry As IAnnotationBase In entity.Annotations
                            entry.Visible = True
                        Next
                    End If
                Next
            Else
                For Each item As KeyValuePair(Of String, IndicatorAnnotation) In _hintLabels
                    If _d3d.Entities.Contains(item.Value.Entity) Then
                        If Not item.Value.IsHidden Then item.Value.Visible = item.Value.Entity.Visible
                    End If
                Next

            End If
        End Sub
        Private Sub ShowAllHintLabels()
            If _hintLabels.Count > 0 Then ShowHintLabels(True)
        End Sub
        Private Sub HideHintLabel(entityId As String)
            If _hintLabels.ContainsKey(entityId) Then
                _hintLabels.Item(entityId).Visible = False
                _hintLabels.Item(entityId).IsHidden = True
            End If
        End Sub

        Private Sub ShowHintLabel(entityId As String)
            If _hintLabels.ContainsKey(entityId) Then
                _hintLabels.Item(entityId).Visible = True
                _hintLabels.Item(entityId).IsHidden = False
            End If
        End Sub

        Private Sub HideHintLabels()
            For Each item As KeyValuePair(Of String, IndicatorAnnotation) In _hintLabels
                Dim entity As BaseModelEntity = devDept.Eyeshot.Entities.Flatten(_document.Entities.GetByEEObjectId(New Guid(item.Key))).Cast(Of BaseModelEntity).FirstOrDefault
                If entity IsNot Nothing Then
                    For Each entry As IAnnotationBase In entity.Annotations
                        entry.Visible = False
                    Next
                End If
            Next
        End Sub

    End Class
End Namespace

