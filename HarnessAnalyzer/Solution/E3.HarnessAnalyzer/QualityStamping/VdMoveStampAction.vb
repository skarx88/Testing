Imports VectorDraw.Geometry
Imports VectorDraw.Professional.Actions
Imports VectorDraw.Professional.vdCollections
Imports VectorDraw.Professional.vdFigures
Imports VectorDraw.Professional.vdPrimaries

Namespace QualityStamping

    Public Class VdMoveStampAction
        Inherits ActionEntityEx
        Private _isCompleted As Boolean = False
        Private _orgStamp As VdStamp
        Private _refBoundingBox As Box
        Private _lastNewPosition As gPoint
        Private _lastAngle As Double
        Private _isVerticalRefBB As Boolean = False
        Private _refOutline As gPoints
        Private _polyOutline As vdPolyline
        Private _movingStampWithRefOutline As VdMovingStampWithOutline

        Public Sub New(vdStamp As VdStamp)
            MyBase.New(New gPoint, vdStamp.Document.ActiveLayOut)
            If vdStamp.Reference IsNot Nothing Then
                _polyOutline = New vdPolyline(vdStamp.Document)
                _refBoundingBox = vdStamp.GetReferenceBoundingBox
                _refOutline = GetOutline(vdStamp)
                _isVerticalRefBB = IsVertical(_refBoundingBox)
                If vdStamp.Reference IsNot Nothing AndAlso vdStamp.Reference.Rotation.HasValue Then
                    _lastAngle = vdStamp.Reference.Rotation.Value
                End If
            End If

            _orgStamp = vdStamp
            _movingStampWithRefOutline = New VdMovingStampWithOutline(vdStamp, _polyOutline)
        End Sub

        Friend Shared Function GetOutline(vdStamp As VdStamp) As gPoints
            If vdStamp.Reference IsNot Nothing Then
                Dim points As gPoints = GetAllPoints(vdStamp.Reference.vdSVGRefObjects.ToArray)
                If points.Count > 0 Then
                    points = points.GetOutlineBoundary
                    points.makeClosed()
                End If
                Return points
            End If
            Return Nothing
        End Function

        Protected Overrides Sub [End]()
            _isCompleted = True
            _movingStampWithRefOutline.Layer = Nothing
            _movingStampWithRefOutline.SetUnRegisterDocument(Nothing)
            MyBase.Layout.Document.ActionControl.Cursor = Nothing
            MyBase.Layout.Document.Invalidate()
            MyBase.[End]()
        End Sub

        Protected Overrides Sub OnMyPositionChanged(ByVal newPosition As gPoint)
            If _isCompleted Then Return
            _lastNewPosition = newPosition
            If _orgStamp.Reference.vdSVGRefObjects.Count > 0 Then
                _lastAngle = Me.ReferencePoint.GetAngle(newPosition) + ((270 * Math.PI) / 180)
            ElseIf _orgStamp.Reference.Rotation.HasValue Then
                '  _lastAngle = _orgStamp.Reference.Rotation.Value 
            End If
            _movingStampWithRefOutline.MoveStampTo(Me.ReferencePoint, newPosition, _lastAngle)
        End Sub

        Public Overrides Property ReferencePoint As gPoint
            Get
                Return GetShortestPoint()
            End Get
            Set(value As gPoint)
                'MyBase.ReferencePoint = value
            End Set
        End Property

        Private Function GetShortestPoint() As gPoint
            If _lastNewPosition IsNot Nothing Then
                If _refOutline IsNot Nothing AndAlso _refOutline.Count > 0 Then
                    _polyOutline.VertexList.RemoveAll()
                    _polyOutline.VertexList.AddRange(_refOutline)
                    Dim closestPoint As gPoint = _polyOutline.getClosestPointTo(_lastNewPosition)
                    If closestPoint Is Nothing Then
                        Dim diff As Double
                        Dim centerlevel As Double
                        If _isVerticalRefBB Then
                            diff = Math.Abs(_refBoundingBox.MidPoint.y - _lastNewPosition.y)
                            centerlevel = (_refBoundingBox.Height / 4) + _orgStamp.BoundingBox.Height
                        Else
                            diff = Math.Abs(_refBoundingBox.MidPoint.x - _lastNewPosition.x)
                            centerlevel = (_refBoundingBox.Width / 4) + _orgStamp.BoundingBox.Width
                        End If

                        If diff < centerlevel Then
                            Return _refBoundingBox.MidPoint
                        Else
                            Return GetShortestDistanceFrom(_refOutline, _lastNewPosition)
                        End If
                    Else
                        Return closestPoint
                    End If
                End If
            End If
            If _refBoundingBox IsNot Nothing Then
                Return _refBoundingBox.MidPoint
            End If
            Return New gPoint
        End Function

        Public Overrides Sub KeyDown(e As KeyEventArgs)
            MyBase.KeyDown(e)
            If Me._orgStamp.Reference Is Nothing OrElse Me._orgStamp.Reference.vdSVGRefObjects.Count = 0 Then
                If e.KeyCode = Keys.R AndAlso Not e.Control Then
                    If Not e.Shift Then _lastAngle -= (45 * Math.PI) / 180 Else _lastAngle += (45 * Math.PI) / 180
                    _movingStampWithRefOutline.MoveStampTo(Me.ReferencePoint, _lastNewPosition, _lastAngle)
                    _movingStampWithRefOutline.Invalidate()
                End If
            End If
        End Sub

        Public Function GetReferenceCenter() As gPoint
            If _orgStamp.Reference IsNot Nothing Then
                Return _orgStamp.GetReferenceBoundingBox.MidPoint
            End If
            Return New gPoint
        End Function

        Private Function GetShortestDistanceFrom(outline As gPoints, toPt As gPoint) As gPoint
            Dim dists As New SortedList(Of Double, gPoint)
            For Each pt As gPoint In outline
                Dim newDist As Double = pt.Distance2D(toPt)
                If Not dists.ContainsKey(newDist) Then
                    dists.Add(newDist, pt)
                End If
            Next

            If dists.Count > 0 Then
                Return dists.First.Value
            End If
            Return New gPoint
        End Function

        Private Shared Sub AddPolyFacePoints(mesh As vdPolyface, ByRef list As gPoints)
            If mesh IsNot Nothing Then
                list.AddRangeUnique(New gPoints(mesh.VertexList.OfType(Of gPoint).ToArray), 1)
            End If
        End Sub

        Friend Shared Function GetAllPoints(ParamArray objs() As vdFigure) As gPoints
            Dim list As New gPoints
            For Each obj As vdFigure In objs
                Dim explEntities As vdEntities = obj.Explode
                If explEntities IsNot Nothing Then
                    Dim objects As vdFigure() = explEntities.OfType(Of vdFigure).ToArray
                    For Each vdfig As vdFigure In objects
                        If TypeOf vdfig Is vdLine Then
                            list.AddUnique(CType(vdfig, vdLine).StartPoint, 1)
                            list.AddUnique(CType(vdfig, vdLine).EndPoint, 1)
                        ElseIf TypeOf vdfig Is vdPolyline Then
                            list.AddRangeUnique(New gPoints((CType(vdfig, vdPolyline).VertexList.OfType(Of gPoint)()).ToArray), 1)
                        ElseIf TypeOf vdfig Is vdArc Then
                            AddPolyFacePoints(CType(vdfig, vdArc).ToMesh(3), list)
                        ElseIf TypeOf vdfig Is vdPolyhatch Then
                            ' do nothing (no points to be added)
                        ElseIf TypeOf vdfig Is vdText Then
                            ' do nothing (no points to be added)
                        ElseIf TypeOf vdfig Is VdSVGGroup Then
                            Dim vdSvgGrp As VdSVGGroup = CType(vdfig, VdSVGGroup)
                            If vdSvgGrp.SVGType = SvgType.table.ToString OrElse vdSvgGrp.SVGType = SvgType.row.ToString OrElse vdSvgGrp.SVGType = SvgType.cell.ToString Then
                                Continue For
                            End If
                        ElseIf TypeOf vdfig Is vdCircle Then
                            AddPolyFacePoints(CType(vdfig, vdCircle).ToMesh(3), list)
                        ElseIf TypeOf vdfig Is vdCurve Then
                            AddPolyFacePoints(CType(vdfig, vdCurve).ToMesh(3), list)
                        ElseIf TypeOf vdfig Is vdDimension Then
                            With CType(vdfig, vdDimension)
                                list.AddUnique(.DefPoint1, 1)
                                list.AddUnique(.DefPoint2, 1)
                                list.AddUnique(.DefPoint3, 1)
                                list.AddUnique(.DefPoint4, 1)
                            End With
                        ElseIf TypeOf vdfig Is vdEllipse Then
                            AddPolyFacePoints(CType(vdfig, vdEllipse).ToMesh(3), list)
                        ElseIf TypeOf vdfig Is vdInfinityLine Then
                            AddPolyFacePoints(CType(vdfig, vdInfinityLine).ToMesh(3), list)
                        ElseIf TypeOf vdfig Is vdPoint Then
                            list.AddUnique(CType(vdfig, vdPoint).InsertionPoint, 1)
                        ElseIf TypeOf vdfig Is vdRect Then
                            list.AddRangeUnique(CType(vdfig, vdRect).BoundingBox.TogPoints, 1)
                        ElseIf TypeOf vdfig Is vdPolyface Then
                            AddPolyFacePoints(CType(vdfig, vdPolyface), list)
                        End If

                        list.AddRange(GetAllPoints(vdfig))
                    Next
                End If
            Next
            Return list
        End Function

        Private Function IsVertical(box As Box) As Boolean
            With box
                Return .Height > .Width
            End With
        End Function

        Public Overrides ReadOnly Property Entity() As vdFigure
            Get
                Return _movingStampWithRefOutline
            End Get
        End Property

        Public Overrides ReadOnly Property HideRubberLine() As Boolean
            Get
                Return True
            End Get
        End Property

        Public Overrides ReadOnly Property needUpdate() As Boolean
            Get
                Return True
            End Get
        End Property

        Public Shared Sub MoveStampToNextFree(vdStamp As VdStamp)
            Dim nextFree As gPoint = GetNextFreePosition(vdStamp)
            If nextFree IsNot Nothing Then
                vdStamp.Reference.RelativeX = vdStamp.GetReferenceBoundingBox.MidPoint.x - nextFree.x
                vdStamp.Reference.RelativeY = vdStamp.GetReferenceBoundingBox.MidPoint.y - nextFree.y
                vdStamp.Origin = nextFree
            End If
        End Sub

        Private Shared Function GetNextFreePosition(vdStamp As VdStamp) As gPoint
            Static count As Integer = 0
            Dim stampClone As VdStamp = CType(vdStamp.Clone(vdStamp.Document), QualityStamping.VdStamp)
            stampClone.OwningCanvas = vdStamp.OwningCanvas

            Dim outline As gPoints = GetOutline(stampClone)
            Dim polyLine As New vdPolyline(stampClone.Document, New Vertexes(outline))

            Dim refBB As Box = stampClone.GetReferenceBoundingBox

            Dim orgDistance As Double = gPoint.Distance2D(stampClone.Origin, polyLine.getClosestPointTo(stampClone.Origin))
            Dim max As Double = refBB.Width
            If refBB.Height > max Then max = refBB.Height
            Dim circle As New vdCircle(vdStamp.Document, refBB.MidPoint, (max + VdStamp.OBJECT_DIST) / 2)

            count += 1
            Dim nextPosition As gPoint
            For Each sPoint As gPoint In circle.GetSamplePoints(15, 0.5)
                sPoint = New gPoint(sPoint.x + circle.Center.x, sPoint.y + circle.Center.y)
                nextPosition = polyLine.getClosestPointTo(sPoint)

                If nextPosition IsNot Nothing Then
                    Dim currDistance As Double = gPoint.Distance2D(sPoint, nextPosition)

                    If currDistance <> orgDistance Then
                        Dim dists As New SortedDictionary(Of Double, gPoint)

                        For Each pt As gPoint In SplitLine(nextPosition, sPoint, 10) ' CInt(currDistance)) ' walking the line points upwards -> The first point that hits the distance is the needed point!
                            Dim distDiff As Double = gPoint.Distance2D(pt, nextPosition) - orgDistance

                            If distDiff >= 0 Then
                                nextPosition = pt
                                Exit For
                            End If
                        Next
                    End If

                    stampClone.Origin = nextPosition

                    If Not stampClone.IsStampOverlaying() Then
                        Return nextPosition
                    End If
                End If
            Next

            Return Nothing
        End Function

        Private Shared Function SplitLine(a As gPoint, b As gPoint, count As Integer) As IList(Of gPoint)
            count += 1

            Dim d As [Double] = Math.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y)) / count
            Dim fi As [Double] = Math.Atan2(b.y - a.y, b.x - a.x)

            Dim points As New List(Of gPoint)(count + 1)

            For i As Integer = 0 To count
                points.Add(New gPoint(a.x + i * d * Math.Cos(fi), a.y + i * d * Math.Sin(fi)))
            Next

            Return points
        End Function

        Public Shared Function CmdMoveStamp(ByVal vdStamp As VdStamp) As VdMoveStampAction
            'HINT we cannot move terminals from one component to another as there can be innerconnectivity or inner circuit diagrams which were affected then
            Dim action As New VdMoveStampAction(vdStamp)
            Dim hitPoint As gPoint = TryCast(vdStamp.Document.ActionUtility.getUserActionEntity(action), gPoint)
            If (hitPoint IsNot Nothing) Then
                ProcessAfterMoveStampCmd(action)
                Return action
            End If
            Return Nothing
        End Function

        Friend Shared Sub ProcessAfterMoveStampCmd(action As VdMoveStampAction)
            With action._orgStamp
                Dim refCenter As gPoint = action.GetReferenceCenter
                .Rotation = CType(action.Entity, VdMovingStampWithOutline).StampRotation
                .Reference.Rotation = CType(action.Entity, VdMovingStampWithOutline).StampRotation
                Dim stampP As gPoint = VdStamp.GetStampPositionTo(refCenter, CType(action.Entity, VdMovingStampWithOutline).Position)
                .Reference.RelativeX = stampP.x
                .Reference.RelativeY = stampP.y
                .Origin = CType(action.Entity, VdMovingStampWithOutline).Position
                .Invalidate()
                .Update()
                .Layer.Invalidate()
            End With
        End Sub

    End Class

End Namespace
