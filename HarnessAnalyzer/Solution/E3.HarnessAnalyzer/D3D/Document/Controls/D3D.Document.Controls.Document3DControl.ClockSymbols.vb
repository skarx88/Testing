Imports devDept.Geometry
Imports Zuken.E3.Lib.Eyeshot.Model
Imports System.Windows.Annotations


Namespace D3D.Document.Controls

    Partial Public Class Document3DControl

        Friend Sub AddClockSymbolLabel(entity As BaseModelEntity, text As String)
            Dim pt As Point3D = Nothing
            If TryCast(entity, TubularBaseEntity) IsNot Nothing Then
                pt = CType(entity, TubularBaseEntity).GetAnnotationAnchorPointFromPercent(0.5)
            Else
                pt = entity.GetBoundingBoxCenter
            End If
            Dim isHidden As Boolean
            If _d3d.HiddenEntities.Select(Function(e) e.Entity).ToList.Contains(entity) Then
                isHidden = True
            End If
            Dim an As New IndicatorAnnotation(entity, pt, GetOffset(entity), text, Color.FromArgb(200, Color.White))
            an.IsHidden = isHidden
            an.Visible = Not isHidden
            entity.Annotations.Add(an)
            an.BorderColor = Color.DarkGray
            _clockSymbolLabels.Add(entity.Id, an)
            If isHidden Then
                HideClockSymbol(entity.Id)
            End If
        End Sub

        Friend Sub ClearClockSymbolsLabels()
            For Each item As KeyValuePair(Of String, IndicatorAnnotation) In _clockSymbolLabels
                Dim entity As BaseModelEntity = devDept.Eyeshot.Entities.Flatten(_document.Entities.GetByEEObjectId(New Guid(item.Key))).Cast(Of BaseModelEntity).FirstOrDefault
                If entity IsNot Nothing Then entity.Annotations.Remove(item.Value)
            Next
            _clockSymbolLabels.Clear()
            Model3DControl1.Design.Refresh()
        End Sub

        Private Sub HideClockSymbols()
            For Each item As KeyValuePair(Of String, IndicatorAnnotation) In _clockSymbolLabels
                Dim entity As BaseModelEntity = devDept.Eyeshot.Entities.Flatten(_document.Entities.GetByEEObjectId(New Guid(item.Key))).Cast(Of BaseModelEntity).FirstOrDefault
                If entity IsNot Nothing Then
                    For Each entry As IAnnotationBase In entity.Annotations
                        entry.Visible = False
                    Next
                End If
            Next
        End Sub

        Private Sub ShowClockSymbols()
            For Each item As KeyValuePair(Of String, IndicatorAnnotation) In _clockSymbolLabels
                Dim entity As BaseModelEntity = devDept.Eyeshot.Entities.Flatten(_document.Entities.GetByEEObjectId(New Guid(item.Key))).Cast(Of BaseModelEntity).FirstOrDefault
                If entity IsNot Nothing Then
                    For Each entry As IAnnotationBase In entity.Annotations
                        If Not CType(entry, IndicatorAnnotation).IsHidden Then entry.Visible = True
                    Next
                End If
            Next
        End Sub

        Public Sub HideClockSymbol(EntityId As String)
            If _clockSymbolLabels.ContainsKey(EntityId) Then
                _clockSymbolLabels.Item(EntityId).Visible = False
                _clockSymbolLabels.Item(EntityId).IsHidden = True
            End If
        End Sub

        Public Sub ShowAllClockSymbols()
            If _clockSymbolLabels.Count > 0 Then ShowClockSymbols(True)
        End Sub
        Public Sub ShowClockSymbol(EntityId As String)
            If _clockSymbolLabels.ContainsKey(EntityId) Then
                Dim lbl As IndicatorAnnotation = _clockSymbolLabels.Item(EntityId)
                lbl.Visible = True
                _d3d.TempEntities.Invalidate(_d3d)
                _d3d.Invalidate()

            End If
        End Sub

        Private Sub ShowClockSymbols(includeHiddens As Boolean)
            If includeHiddens Then
                For Each item As KeyValuePair(Of String, IndicatorAnnotation) In _clockSymbolLabels
                    Dim entity As BaseModelEntity = devDept.Eyeshot.Entities.Flatten(_document.Entities.GetByEEObjectId(New Guid(item.Key))).Cast(Of BaseModelEntity).FirstOrDefault
                    If entity IsNot Nothing Then
                        For Each entry As IAnnotationBase In entity.Annotations
                            entry.Visible = entity.Visible
                        Next
                    End If
                Next
            Else
                For Each item As KeyValuePair(Of String, IndicatorAnnotation) In _clockSymbolLabels
                    If _d3d.Entities.Contains(item.Value.Entity) Then
                        Dim anno As IndicatorAnnotation = item.Value
                        If Not anno.IsHidden Then anno.Visible = anno.Entity.Visible
                    End If
                Next

            End If
        End Sub

    End Class
End Namespace
