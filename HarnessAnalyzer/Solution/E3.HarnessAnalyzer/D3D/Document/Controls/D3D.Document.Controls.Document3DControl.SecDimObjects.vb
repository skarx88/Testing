Imports devDept.Eyeshot
Imports devDept.Eyeshot.Entities
Imports Zuken.E3.Lib.Eyeshot.Model
Imports System.ComponentModel


Namespace D3D.Document.Controls

    Partial Public Class Document3DControl

        Friend Sub AddSecDimTargetLabel(Bundle As TubularBaseEntity, text As String, posAsPercent As Single, subCurve As CurveEx)
            Dim secobj As SecDimObject = Nothing
            Dim bdl As TubularBaseEntity = Bundle

            If subCurve IsNot Nothing Then
                Dim anchorPoint As devDept.Geometry.Point3D = bdl.GetAnnotationAnchorPointFromPercent(posAsPercent)
                secobj = New SecDimObject(_d3d, bdl, anchorPoint, GetOffset(bdl,, False), text, subCurve)

                If _secDimObjects.ContainsKey(bdl.Id) Then
                    _secDimObjects.Item(bdl.Id).Add(secobj)
                Else
                    _secDimObjects.Add(bdl.Id, New List(Of SecDimObject) From {secobj})
                End If

            End If

            If secobj.IsHidden Then HideSecDimObject(Bundle.Id)
            If Not Bundle.Visible Then secobj.Visible = False
        End Sub
        Friend Sub ClearSecDimensionObjects()
            Dim model As devDept.Eyeshot.Design = CType(_d3d, devDept.Eyeshot.Design)
            For Each item As KeyValuePair(Of String, List(Of SecDimObject)) In _secDimObjects
                For Each obj As SecDimObject In item.Value
                    obj.Clear()
                    obj = Nothing
                Next
            Next

            _d3d.TempEntities.Invalidate(_d3d)
            _secDimObjects.Clear()
            _d3d.Invalidate()
        End Sub
        Private Sub CheckSecDimLabelSelection(e As MouseEventArgs)
            Dim index As Integer = CType(_d3d, Design).GetLabelUnderMouseCursor(e.Location)
            ClearSelectedSecDimLabel()
            If index <> -1 Then
                _d3d.ActionMode = actionType.None
                Dim label As devDept.Eyeshot.Labels.Label = _d3d.ActiveViewport.Labels.Item(index)

                If label.LabelData IsNot Nothing Then
                    SelectedLabel = CType(label, LeaderAndTextEx)
                    label.Selected = True

                    Dim subCurve As CurveEx = CType(label.LabelData, CurveEx)
                    subCurve.Regen(0.1)

                    _d3d.TempEntities.Add(CType(subCurve, Entity))
                End If
            End If
        End Sub

        Private Sub ClearSelectedSecDimLabel()
            If SelectedLabel IsNot Nothing Then
                _d3d.TempEntities.Remove(CType(CType(SelectedLabel.LabelData, CurveEx), Entity))
                Dim index As Integer = _d3d.ActiveViewport.Labels.IndexOf(SelectedLabel)
                If index > -1 Then
                    Dim label As devDept.Eyeshot.Labels.Label = _d3d.ActiveViewport.Labels.Item(index)
                    label.Selected = False
                    SelectedLabel = Nothing
                End If
            End If
        End Sub

        Public Sub ShowAllSecDimObjects()
            If _secDimObjects.Count > 0 Then ShowSecDimObjects(True)
        End Sub

        Public Sub ShowSecDimObject(segmentid As String)
            If _secDimObjects.ContainsKey(segmentid) Then
                For Each obj As SecDimObject In _secDimObjects.Item(segmentid)
                    For Each m As Entity In obj.Markers
                        If Not _d3d.TempEntities.Contains(m) Then
                            _d3d.TempEntities.TryAdd(m)
                            m.Regen(0.1)
                        End If
                    Next
                    For Each lbl As devDept.Eyeshot.Labels.Label In obj.Labels(AnnotationBase.ViewportType.All)
                        lbl.Visible = True
                    Next
                    obj.IsHidden = False
                Next
            End If
            _d3d.Invalidate()
        End Sub

        Public Sub HideSecDimObject(bundleId As String)
            Dim ok As Boolean
            If _secDimObjects.ContainsKey(bundleId) Then
                For Each obj As SecDimObject In _secDimObjects.Item(bundleId)

                    For Each m As Entity In obj.Markers
                        m.Regen(0.1)
                        Dim index As Integer = _d3d.TempEntities.IndexOf(m)
                        ok = _d3d.TempEntities.Remove(m)
                        If ok Then m.Regen(0.1)
                        m.Invalidate(_d3d)
                    Next

                    Dim en As Entity = CType(obj.SubCurve, Entity)
                    ok = _d3d.TempEntities.Remove(en)

                    For Each lbl As devDept.Eyeshot.Labels.Label In obj.Labels(AnnotationBase.ViewportType.All)
                        lbl.Visible = False
                    Next
                    obj.IsHidden = True
                Next
                _d3d.TempEntities.Invalidate(_d3d)
                _d3d.Invalidate()

            End If
        End Sub
        Private Sub HideSecDimObjects()
            For Each item As KeyValuePair(Of String, List(Of SecDimObject)) In _secDimObjects
                For Each obj As SecDimObject In item.Value
                    obj.Hide()
                Next
            Next
            _d3d.TempEntities.Clear()
            _d3d.TempEntities.Invalidate(_d3d)
            LabelsAreVisible = False

            Me._d3d.Invalidate()

        End Sub
        Private Sub ShowSecDimObjects(includeHiddens As Boolean)
            For Each item As KeyValuePair(Of String, List(Of SecDimObject)) In _secDimObjects
                For Each obj As SecDimObject In item.Value

                    Dim en As BaseModelEntity = obj.Entity
                    If Not includeHiddens Then
                        If Not obj.IsHidden Then
                            For Each lbl As devDept.Eyeshot.Labels.Label In obj.Labels(AnnotationBase.ViewportType.All)
                                lbl.Visible = en.Visible
                            Next
                            If en.Visible Then
                                For Each m As Entity In obj.Markers
                                    m.Regen(0.1)
                                    If Not _d3d.TempEntities.Contains(m) Then
                                        Me._d3d.TempEntities.TryAdd(m)
                                    End If
                                Next
                            End If
                        End If
                    Else
                        For Each lbl As devDept.Eyeshot.Labels.Label In obj.Labels(AnnotationBase.ViewportType.All)
                            lbl.Visible = en.Visible
                        Next
                        If en.Visible Then
                            For Each m As Entity In obj.Markers
                                m.Regen(0.1)
                                If Not _d3d.TempEntities.Contains(m) Then
                                    Me._d3d.TempEntities.TryAdd(m)
                                End If
                            Next
                        End If
                        obj.IsHidden = False
                    End If
                Next
            Next
            LabelsAreVisible = True
            _d3d.Invalidate()
        End Sub

    End Class
End Namespace