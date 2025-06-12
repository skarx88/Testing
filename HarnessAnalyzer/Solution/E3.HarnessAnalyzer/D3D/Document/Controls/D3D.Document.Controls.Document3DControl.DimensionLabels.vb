Imports Zuken.E3.Lib.Eyeshot.Model
Imports Zuken.E3.Lib.Model


Namespace D3D.Document.Controls

    Partial Public Class Document3DControl
        Friend Sub AddDimensionReferenceLabel(entityId As Guid, refText As String, Optional percent As Single = 0.0, Optional segmentId As Guid = Nothing)
            Dim entity As BaseModelEntity = devDept.Eyeshot.Entities.Flatten(_document.Entities.GetByEEObjectId(entityId)).Cast(Of BaseModelEntity).ToList.FirstOrDefault
            If entity IsNot Nothing Then
                Dim exists As Boolean

                Dim isHidden As Boolean
                If _d3d.HiddenEntities.Select(Function(e) e.Entity).ToList.Contains(entity) Then
                    isHidden = True
                End If

                If _dimensionLabels.ContainsKey(entity.Id) Then
                    For Each an As IndicatorAnnotation In _dimensionLabels(entity.Id)
                        If an.IsRef Then
                            exists = True
                            Exit For
                        End If
                    Next
                End If

                If Not exists Then
                    If segmentId = Nothing OrElse segmentId = Guid.Empty Then
                        Dim anchor As devDept.Geometry.Point3D
                        If TypeOf entity Is TubularBaseEntity Then
                            anchor = CType(entity, TubularBaseEntity).GetAnnotationAnchorPointFromPercent(0.5)
                        Else
                            anchor = entity.GetBoundingBoxCenter
                        End If
                        Dim anOrigin As IndicatorAnnotation = New IndicatorAnnotation(entity, anchor, GetOffset(entity), refText, Color.FromArgb(200, Color.Yellow), True)
                        anOrigin.BorderColor = Color.Black
                        anOrigin.IsHidden = isHidden
                        entity.Annotations.Add(anOrigin)
                        AddLabel(entity.Id, anOrigin)
                    Else
                        Dim seg As BaseModelEntity = devDept.Eyeshot.Entities.Flatten(_document.Entities.GetByEEObjectId(segmentId)).Cast(Of BaseModelEntity).ToList.FirstOrDefault
                        If seg IsNot Nothing AndAlso TypeOf seg Is TubularBaseEntity Then
                            Dim anchorPoint As devDept.Geometry.Point3D = CType(seg, TubularBaseEntity).GetAnnotationAnchorPointFromPercent(percent)
                            Dim anOrigin As IndicatorAnnotation = New IndicatorAnnotation(entity, anchorPoint, GetOffset(entity), refText, Color.FromArgb(200, Color.Yellow), True)
                            entity.Annotations.Add(anOrigin)
                            anOrigin.BorderColor = Color.Black
                            entity.Annotations.Add(anOrigin)
                            anOrigin.SegmentId = segmentId
                            anOrigin.IsHidden = isHidden
                            AddLabel(entity.Id, anOrigin)
                        End If
                    End If
                End If
                If isHidden Then HideDimension(entity.Id)

            End If

        End Sub
        Friend Sub AddDimensionTargetLabel(entityId As Guid, origId As Guid, refTxt As String, dms As E3.Lib.Model.Dimension, Optional percent As Single = 0.0, Optional segmentId As Guid = Nothing, Optional isref As Boolean = True)
            Dim txt As String = GetTextFromDimension(dms, refTxt)
            AddDimensionTargetLabel(entityId, origId, txt, percent, segmentId)
        End Sub

        Friend Sub AddDimensionTargetLabel(entityId As Guid, origId As Guid, dmsText As String, Optional percent As Single = 0.0, Optional segmentId As Guid = Nothing)
            Dim entity As BaseModelEntity = devDept.Eyeshot.Entities.Flatten(_document.Entities.GetByEEObjectId(entityId)).Cast(Of BaseModelEntity).ToList.FirstOrDefault
            If entity IsNot Nothing Then
                Dim isHidden As Boolean
                If _d3d.HiddenEntities.Select(Function(e) e.Entity).ToList.Contains(entity) Then
                    isHidden = True
                End If
                If segmentId = Nothing OrElse segmentId = Guid.Empty Then
                    Dim anchor As devDept.Geometry.Point3D
                    If TypeOf entity Is ProtectionEntity Then
                        anchor = CType(entity, ProtectionEntity).GetAnnotationAnchorPointFromPercent(percent)
                    ElseIf TypeOf entity Is BundleEntity Then
                        anchor = CType(entity, BundleEntity).GetAnnotationAnchorPointFromPercent(0.5)
                    Else
                        anchor = entity.GetBoundingBoxCenter
                    End If
                    If anchor.IsValidPoint Then
                        Dim an As New IndicatorAnnotation(entity, anchor, GetOffset(entity), dmsText, Color.FromArgb(200, Color.LightYellow))
                        an.IsHidden = isHidden
                        entity.Annotations.Add(an)
                        an.BorderColor = Color.LightGray
                        an.OrigId = origId
                        AddLabel(entity.Id, an)
                    End If
                Else
                    Dim seg As BaseModelEntity = devDept.Eyeshot.Entities.Flatten(_document.Entities.GetByEEObjectId(segmentId)).Cast(Of BaseModelEntity).ToList.FirstOrDefault
                    If seg IsNot Nothing AndAlso TypeOf seg Is TubularBaseEntity Then
                        Dim anchorPoint As devDept.Geometry.Point3D = CType(seg, TubularBaseEntity).GetAnnotationAnchorPointFromPercent(percent)
                        Dim an As New IndicatorAnnotation(entity, anchorPoint, GetOffset(entity), dmsText, Color.FromArgb(200, Color.LightYellow))
                        an.BorderColor = Color.LightGray
                        an.OrigId = origId
                        an.IsHidden = isHidden
                        entity.Annotations.Add(an)
                        AddLabel(entity.Id, an)
                    End If
                End If
                If isHidden Then HideDimension(entity.Id)
            End If
        End Sub

        Public Sub ShowAllDimensions()
            If _dimensionLabels.Count > 0 Then ShowDimensionLabels(True)
        End Sub
        Public Sub ShowDimension(EntityId As String)
            If _dimensionLabels.ContainsKey(EntityId) Then
                Dim lblList As List(Of IndicatorAnnotation) = _dimensionLabels.Item(EntityId)
                For Each lbl As IndicatorAnnotation In lblList
                    lbl.Visible = True
                Next
                _d3d.TempEntities.Invalidate(_d3d)
                _d3d.Invalidate()

            End If
        End Sub
        Public Sub HideDimension(EntityId As String)
            If _dimensionLabels.ContainsKey(EntityId) Then
                Dim lblList As List(Of IndicatorAnnotation) = _dimensionLabels.Item(EntityId)
                For Each lbl As IndicatorAnnotation In lblList
                    lbl.Visible = False
                    lbl.IsHidden = True
                Next
                _d3d.TempEntities.Invalidate(_d3d)
                _d3d.Invalidate()

            End If
        End Sub
        Private Sub AddLabel(id As String, an As IndicatorAnnotation)
            If an.IsHidden Then an.Visible = False
            If _dimensionLabels.ContainsKey(id) Then
                _dimensionLabels(id).Add(an)
            Else
                _dimensionLabels.Add(id, New List(Of IndicatorAnnotation)({an}))
            End If
            If Not an.IsRef Then
                Dim myOrigin As IndicatorAnnotation = _dimensionLabels(an.OrigId.ToString).Where(Function(a) a.IsRef = True).FirstOrDefault
                If myOrigin IsNot Nothing Then
                    myOrigin.Childs.Add(an)
                End If
            End If
        End Sub

        Private Sub HideDimensionLabels()
            For Each lbl As KeyValuePair(Of String, List(Of IndicatorAnnotation)) In _dimensionLabels
                For Each entry As IndicatorAnnotation In lbl.Value
                    entry.Visible = False
                Next
            Next
        End Sub

        Private Sub ShowDimensionLabels(includeHiddens As Boolean)
            If includeHiddens Then
                For Each item As KeyValuePair(Of String, List(Of IndicatorAnnotation)) In _dimensionLabels
                    For Each anno As IndicatorAnnotation In item.Value
                        If _d3d.Entities.Contains(anno.Entity) Then
                            For Each entry As IndicatorAnnotation In item.Value
                                entry.Visible = anno.Entity.Visible
                            Next
                        End If
                    Next
                Next
            Else
                For Each item As KeyValuePair(Of String, List(Of IndicatorAnnotation)) In _dimensionLabels
                    For Each anno As IndicatorAnnotation In item.Value
                        If _d3d.Entities.Contains(anno.Entity) Then
                            For Each entry As IndicatorAnnotation In item.Value
                                If Not entry.IsHidden Then entry.Visible = anno.Entity.Visible
                            Next
                        End If
                    Next
                Next
            End If

        End Sub
        Friend Sub ClearDimensionLabels()
            For Each item As KeyValuePair(Of String, List(Of IndicatorAnnotation)) In _dimensionLabels
                Dim id As String = item.Key
                Dim entity As BaseModelEntity
                entity = devDept.Eyeshot.Entities.Flatten(_document.Entities.GetByEEObjectId(New Guid(id))).Cast(Of BaseModelEntity).FirstOrDefault
                If entity IsNot Nothing Then
                    For Each an As AnnotationBase In item.Value
                        entity.Annotations.Remove(an)
                    Next
                End If
            Next
            _dimensionLabels.Clear()
        End Sub
        Private Function GetTextFromDimension(d As [Lib].Model.Dimension, refText As String) As String
            Dim val As String = Math.Round(d.Value, 1).ToString
            Dim pTol As Single = d.PositiveTolerance
            Dim mTol As Single = d.NegativeTolerance
            Dim unit As String = "mm"

            Dim txt As String = String.Format("{0}{1}", val, unit)

            If mTol > 0 Then
                txt = String.Format("{0} -{1}", txt, mTol.ToString)
            End If

            If pTol > 0 Then
                txt = String.Format("{0} +{1}", txt, pTol.ToString)
            End If

            txt = String.Format("{0} {1}{2}", txt, Document3DStrings.ReferenceEx, refText)

            Return txt
        End Function
    End Class
End Namespace
