Imports devDept
Imports devDept.Eyeshot.Entities
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.Lib.Eyeshot.Model
Imports Zuken.E3.Lib.IO.KBL
Imports Zuken.E3.Lib.Model

Partial Public Class DocumentForm

    Dim points As New Dictionary(Of Guid, List(Of DistancePoints))


    Private Function GetLength(segment As E3.Lib.Model.Segment) As Integer
        If _settingsForm.GeneralSettings.LengthClassDocument3D = LengthClass.DMU Then
            Return segment.DMULength
        Else
            Return segment.NomLength
        End If
    End Function

    Friend Sub ShowSectionDimensions()
#Const showAllSegments = True

        Dim model As Zuken.E3.Lib.Model.EESystemModel = Me.Document.Model
        Dim protectionUnit As String = String.Empty

        If model IsNot Nothing Then
            points = New Dictionary(Of Guid, List(Of DistancePoints))

            For Each item As TubularBaseEntity In Me.Document.Entities.Where(Function(obj) obj.EntityType = ModelEntityType.Bundle).ToList

#If showAllSegments Then
                Dim seg As E3.Lib.Model.Segment = model.Segments(item.EEObjectId)
                Dim length As Double = GetLength(seg)
                AddPoints(New DistancePoints(0, 0), New DistancePoints(length, length), seg)
#End If
            Next

            For Each item As IBaseModelEntityEx In Me.Document.Entities.Where(Function(obj) obj.EntityType = ModelEntityType.Protection).ToList

                Dim modelObject As ObjectBaseNaming = CType(model(New Guid(item.Id)), ObjectBaseNaming)
                Dim protection As E3.Lib.Model.Protection = CType(modelObject, Protection)
                Dim bagBase As KblPropertyBagBase = modelObject.CustomAttributes.OfType(Of KblPropertyBagAttribute).SingleOrDefault?.PropertyBag
                Dim bag As KblProtectionPropertyBag = CType(bagBase, KblProtectionPropertyBag)

                protectionUnit = bag.LengthUnit

                Dim refnode As IBaseEntity = Me.Document.Entities.GetByKblIds(New String() {bag.StartNodeId}).FirstOrDefault()
                Dim seg As E3.Lib.Model.Segment = Nothing
                If refnode IsNot Nothing Then
                    Dim d1 As Double = 0
                    Dim d2 As Double = 0

                    Dim b1 As Double = 0
                    Dim b2 As Double = 0

                    Dim protSegments As Mapper(Of Segment) = protection.GetSegments()
                    Dim nSegments As Integer = protSegments.Count

                    If nSegments > 1 Then
                        seg = GetSegment(protection, NodeType.StartNode)

                        If seg IsNot Nothing Then
                            d1 = CSng(GetPos(protection, seg, NodeType.StartNode))
                            AddPoint(New DistancePoints(d1, bag.AbsoluteStartLocationValue), seg)
                        End If

                        seg = GetSegment(protection, NodeType.EndNode)

                        If seg IsNot Nothing Then
                            d2 = CSng(GetPos(protection, seg, NodeType.EndNode))
                            AddPoint(New DistancePoints(d2, bag.AbsoluteEndLocationValue), seg)
                        End If

                    Else
                        Dim nd As BaseModelEntity = CType(refnode, BaseModelEntity)
                        seg = protSegments(0).Entry
                        Dim Length As Integer = GetLength(seg)
                        Dim StartVertexId As Guid = nd.EEObjectId

                        If protection.StartVertexId = StartVertexId Then
                            d1 = protection.StartingPercentage * Length
                            d2 = Length - protection.EndingPercentage * Length
                        Else
                            d1 = Length - protection.StartingPercentage * Length
                            d2 = protection.EndingPercentage * Length
                        End If

                        AddPoints(New DistancePoints(d1, bag.AbsoluteStartLocationValue), New DistancePoints(d2, bag.AbsoluteEndLocationValue), seg)

                    End If
                End If
            Next

            For Each item As IBaseModelEntityEx In Me.Document.Entities.Where(Function(obj) obj.EntityType = ModelEntityType.Supplement).ToList
                Dim modelObject As ObjectBaseNaming = CType(model(New Guid(item.Id)), ObjectBaseNaming)
                Dim sup As E3.Lib.Model.Supplement = CType(modelObject, Supplement)
                Dim bagBase As KblPropertyBagBase = modelObject.CustomAttributes.OfType(Of KblPropertyBagAttribute).SingleOrDefault?.PropertyBag
                Dim bag As KblAccessoryPropertyBag = CType(bagBase, KblAccessoryPropertyBag)

                Dim refnode As IBaseModelEntityEx = Me.Document.Entities.GetByKblIds(New String() {bag.StartNodeId}).FirstOrDefault()
                If refnode IsNot Nothing Then
                    Dim relDis As Double = bag.LocationValue

                    Dim bndl As IBaseModelEntityEx = Me.Document.Entities.GetByKblIds(New String() {bag.SegmentId}).FirstOrDefault()
                    Dim tb As TubularBaseEntity = CType(bndl, TubularBaseEntity)
                    Dim seg As E3.Lib.Model.Segment = model.Segments(tb.EEObjectId)

                    If seg IsNot Nothing Then
                        Dim dis As Double = relDis * GetLength(seg)
                        AddPoint(New DistancePoints(dis, bag.AbsoluteLocationValue), seg)
                    End If
                End If
            Next

            For Each item As IBaseModelEntityEx In Me.Document.Entities.Where(Function(obj) obj.EntityType = ModelEntityType.Fixing).ToList
                If TypeOf item Is FixingGroup Then
                    Dim fg As FixingGroup = CType(item, FixingGroup)
                    For Each fix As FixingEntity In fg
                        Dim modelFxObject As ObjectBaseNaming = CType(model(New Guid(fix.Id)), ObjectBaseNaming)
                        Dim bagBase As KblPropertyBagBase = modelFxObject.CustomAttributes.OfType(Of KblPropertyBagAttribute).SingleOrDefault?.PropertyBag
                        Dim bag As KblFixingPropertyBag = CType(bagBase, KblFixingPropertyBag)

                        Dim BundleEntityId As Guid = New Guid(fix.BundleEntityId)
                        Dim seg As E3.Lib.Model.Segment = model.Segments.Item(BundleEntityId)

                        Dim refnode As IBaseModelEntityEx = Me.Document.Entities.GetByKblIds(New String() {bag.StartNodeId}).FirstOrDefault()

                        If refnode IsNot Nothing Then
                            Dim dis As Double = bag.LocationValue * GetLength(seg)
                            AddPoint(New DistancePoints(dis, bag.AbsoluteLocationValue), seg)
                        End If
                    Next
                End If
            Next
        End If


        For Each point As KeyValuePair(Of Guid, List(Of DistancePoints)) In points
            Dim seg As E3.Lib.Model.Segment = Me.Document.Model.Segments(point.Key)
            Dim bdl As IBaseEntityEx = Me.Document.Entities.Where(Function(obj) obj.EntityType = Zuken.E3.Lib.Eyeshot.Model.ModelEntityType.Bundle AndAlso obj.Id = point.Key.ToString).FirstOrDefault
            Dim Length As Integer = GetLength(seg)
            Dim myPoints As List(Of DistancePoints) = point.Value.OrderBy(Function(x) x.CalcPoint).ToList()

            For index As Integer = 0 To myPoints.Count - 1

                Dim pos As Double = Math.Min(1, myPoints(index).CalcPoint / Length)
                Dim tbl As TubularBaseEntity = CType(bdl, TubularBaseEntity)

                If index < point.Value.Count - 1 Then
                    Dim distance As Single = CSng(Math.Round(myPoints(index + 1).CalcPoint - myPoints(index).CalcPoint, 0))
                    Dim rasterDist As Single = CSng(Math.Round(myPoints(index + 1).BagPoint - myPoints(index).BagPoint, 0))
                    Dim labelPos As Single = CSng((myPoints(index).CalcPoint + 0.5 * distance) / Length)

                    Dim subCurve As CurveEx = Nothing

                    Dim startPercentage As Double = CDbl(myPoints(index).CalcPoint / Length)
                    Dim endPercentage As Double = Math.Min(1, CDbl(myPoints(index + 1).CalcPoint / Length))
                    If startPercentage <> endPercentage Then
                        tbl.GetCenterCurve.SubCurve(startPercentage, endPercentage, subCurve)
                        If subCurve IsNot Nothing Then
                            _D3DControl.AddSecDimTargetLabel(tbl, rasterDist.ToString + protectionUnit, labelPos, subCurve)
                        End If
                    End If
                End If
            Next
        Next
        _D3DControl.LabelsAreVisible = True
        _D3DControl.Model3DControl1.Design.ActiveViewport.Labels.Regen()
        _D3DControl.Model3DControl1.Design.Invalidate()
    End Sub

    Private Sub AddPoint(d As DistancePoints, seg As E3.Lib.Model.Segment)
        d.CalcPoint = CDbl(Math.Round(d.CalcPoint, 1))
        d.BagPoint = CDbl(Math.Round(d.BagPoint, 1))
        If (points.ContainsKey(seg.Id)) Then
            If Not points.Item(seg.Id).Select(Function(p) p.CalcPoint).ToList.Contains(d.CalcPoint) Then
                points.Item(seg.Id).Add(d)
            End If
        Else
            points.Add(seg.Id, New List(Of DistancePoints) From {d})
        End If
    End Sub


    Private Sub AddPoints(d1 As DistancePoints, d2 As DistancePoints, seg As E3.Lib.Model.Segment)
        AddPoint(d1, seg)
        AddPoint(d2, seg)
    End Sub

    Private Function GetSegment(p As Protection, nodeType As NodeType) As E3.Lib.Model.Segment
        Dim s As E3.Lib.Model.Segment = Nothing
        If nodeType = NodeType.StartNode Then
            For Each segment As E3.Lib.Model.Segment In p.GetSegments().Entries
                Dim vertex As E3.Lib.Model.Vertex = segment.GetVertices().Entries.ToList().Where(Function(v) v.Id = p.StartVertexId).FirstOrDefault()
                If vertex IsNot Nothing Then
                    s = segment
                    Exit For
                End If
            Next

        ElseIf nodeType = NodeType.EndNode Then
            For Each segment As E3.Lib.Model.Segment In p.GetSegments().Entries
                Dim vertex As E3.Lib.Model.Vertex = segment.GetVertices().Entries.ToList().Where(Function(v) v.Id = p.EndVertexId).FirstOrDefault()
                If vertex IsNot Nothing Then
                    s = segment
                    Exit For
                End If
            Next
        End If

        Return s
    End Function

    Private Function GetPos(p As Protection, segment As E3.Lib.Model.Segment, nodetype As NodeType) As Double

        Dim l As Double = 0.0
        Dim segmentLength As Integer = GetLength(segment)

        If nodetype = NodeType.StartNode Then
            If p.StartVertexId = segment.StartVertexId Then
                l = segmentLength * p.StartingPercentage
                Return l
            Else
                l = segmentLength * (1 - p.StartingPercentage)
                Return l
            End If

        ElseIf nodetype = NodeType.EndNode Then
            If p.EndVertexId = segment.StartVertexId Then
                l = segmentLength * p.EndingPercentage
                Return l
            Else
                l = segmentLength * (1 - p.EndingPercentage)
                Return l
            End If
        End If
        Return l
    End Function

    Friend Sub ClearSectionDimensions()
        _D3DControl.ClearSecDimensionObjects()
    End Sub

    Public ReadOnly Property GeneralSettings As GeneralSettingsBase
        Get
            Return _settingsForm?.GeneralSettings
        End Get
    End Property

    Friend ReadOnly Property HasSchematicsFeature As Boolean
        Get
            Return (_settingsForm?.HasSchematicsFeature).GetValueOrDefault
        End Get
    End Property

    Friend ReadOnly Property HasTopoCompareFeature As Boolean
        Get
            Return (_settingsForm?.HasTopoCompareFeature).GetValueOrDefault
        End Get
    End Property

    Friend ReadOnly Property HasView3DFeature As Boolean
        Get
            Return (_settingsForm?.HasView3DFeature).GetValueOrDefault
        End Get
    End Property

    Private Class DistancePoints
        Public Property CalcPoint As Double
        Public Property BagPoint As Double
        Public Sub New(calc As Double, bag As Double)
            CalcPoint = calc
            BagPoint = bag
        End Sub
    End Class

End Class
