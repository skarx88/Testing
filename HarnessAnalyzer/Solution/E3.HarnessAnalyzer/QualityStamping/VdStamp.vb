Imports VectorDraw.Geometry
Imports VectorDraw.Professional.vdCollections
Imports VectorDraw.Professional.vdFigures

Namespace QualityStamping

    Public Class VdStamp
        Inherits VdSVGGroup

        Friend Const TOP_LENGTH As Integer = 10
        Friend Const CIRCLE_RADIUS As Double = 5
        Friend Const OBJECT_DIST As Integer = 10

        Private _reference As QualityStamping.ObjectReference
        Private _fillColor As System.Drawing.Color = Color.Red
        Private _random As New Random

        Public Sub New()
            MyBase.New()
            SetContrastTextColor()
        End Sub

        Public Sub New(reference As ObjectReference)
            Me.New()
            Me.Reference = reference
        End Sub

        Property Text As String = String.Empty
        Property TextColor As Color = Color.Black

        Property FillColor As Color
            Get
                Return _fillColor
            End Get
            Set(value As Color)
                _fillColor = value
                SetContrastTextColor()
            End Set
        End Property

        Private Sub SetContrastTextColor()
            Me.TextColor = GetContrastColor(_fillColor)
        End Sub

        Private Function GetReferenceBoundingBoxInternal(objRef As ObjectReference) As Box
            Dim selection As New vdSelection()
            selection.SetUnRegisterDocument(Me.Document)

            For Each ref As VdSVGGroup In objRef.vdSVGRefObjects
                selection.Add(ref)
            Next

            Return selection.GetBoundingBox
        End Function

        Protected Overrides Sub OnDocumentDefaults()
            MyBase.OnDocumentDefaults()
            SetContrastTextColor()
        End Sub

        Property Reference As QualityStamping.ObjectReference
            Get
                Return _reference
            End Get
            Set(value As QualityStamping.ObjectReference)
                _reference = value
                If value IsNot Nothing Then
                    Dim refBox As Box = SetRelativeXYWhenEmptyPosition(value)
                    Me.Origin = GetStampPositionTo(refBox.MidPoint, value.RelativePt)
                    Me.Rotation = GetReferenceRotation(value)
                Else
                    Me.Origin = New gPoint()
                End If
            End Set
        End Property

        Private Function GetReferenceRotation(ref As ObjectReference) As Double
            With ref
                If .Rotation Is Nothing Then
                    Return GetDefaultAngle()
                Else
                    Return .Rotation.Value
                End If
            End With
        End Function

        Private Function SetRelativeXYWhenEmptyPosition(ref As ObjectReference) As Box
            With ref
                Dim refbb As Box = GetReferenceBoundingBoxInternal(ref)
                If .IsEmptyPoint Then
                    If .ObjectType <> KblObjectType.Segment Then
                        Dim nRelPoint As gPoint = GetNextRelativePoint()
                        If nRelPoint IsNot Nothing Then
                            .RelativeX = nRelPoint.x
                            .RelativeY = nRelPoint.y
                            Dim org As gPoint = GetStampPositionTo(refbb.MidPoint, .RelativePt)
                            .Rotation = refbb.MidPoint.GetAngle(org) + ((270 * Math.PI) / 180)
                        End If
                    Else
                        .RelativeX = _random.Next(CInt(refbb.Width * 0.03))
                        .RelativeY = _random.Next(CInt(refbb.Height * 0.03))
                    End If
                End If
                Return refbb
            End With
        End Function

        Private Function GetNextRelativePoint() As gPoint
            If Me.Reference.List IsNot Nothing AndAlso Me.Reference.List.Owner IsNot Nothing Then
                Dim stamps As List(Of QMStamp) = Me.Reference.List.Owner.List.Where(Function(stmp) stmp.ObjectReferences.Contains(Me.Reference.KblId)).ToList
                If (stamps.Count > 0) Then
                    Dim vdStampCountOnReference As Integer = stamps.Count
                    Dim refBB As Box = GetReferenceBoundingBox()
                    Dim outline As gPoints = VdMoveStampAction.GetAllPoints(Me.Reference.vdSVGRefObjects.ToArray).GetOutlineBoundary
                    outline.makeClosed()

                    Dim poly As New vdPolyline(Me.Document)
                    poly.setDocumentDefaults()
                    poly.VertexList.AddRange(outline)
                    poly.PenWidth = 1

                    Dim calculatedSamplePoint As gPoint = GetPointOfStampPosition(poly.VertexList, vdStampCountOnReference)
                    calculatedSamplePoint.x -= refBB.MidPoint.x - (CIRCLE_RADIUS * 2)
                    calculatedSamplePoint.y -= refBB.MidPoint.y - (CIRCLE_RADIUS * 2)

                    Return calculatedSamplePoint
                End If
            End If
            Return New gPoint
        End Function

        Private Function GetPointOfStampPosition(vertexList As Vertexes, stampCount As Integer, Optional count As Integer = 1) As gPoint
            If vertexList.Count > 0 Then
                Dim pt As gPoint = vertexList(0)
                Dim currPosition As Integer = stampCount
                If stampCount > vertexList.Count Then
                    currPosition = stampCount - vertexList.Count
                    If currPosition > vertexList.Count Then
                        Return GetPointOfStampPosition(vertexList, currPosition, count + 1)
                    End If
                End If
                Return New gPoint(vertexList(currPosition - 1).x + _random.Next((count * 10)), vertexList(currPosition - 1).y)
            End If
            Return New gPoint
        End Function

        Private Function GetDefaultAngle() As Double
            Return (-45 * Math.PI) / 180
        End Function

        Friend Function GetReferenceBoundingBox() As Box
            If _reference IsNot Nothing Then
                Return GetReferenceBoundingBoxInternal(Me.Reference)
            End If
            Return Nothing
        End Function

        Public Overrides ReadOnly Property BoundingBox As Box
            Get
                Dim box As New Box(MyBase.BoundingBox)
                box.AddWidth(HarnessAnalyzer.[Shared].SELECTING_SIZE / 2)
                Return box
            End Get
        End Property


        Property OwningCanvas As DrawingCanvas

        Public Overrides Property KblId As String
            Get
                If Me.Reference IsNot Nothing Then
                    Return Me.Reference.KblId
                End If
                Return Nothing
            End Get
            Set(value As String)
                If Me.Reference IsNot Nothing Then
                    Me.Reference.KblId = value
                Else
                    Throw New NullReferenceException("Cannot set kbl Id because reference is null!")
                End If
            End Set
        End Property

        Private Function GetCircleMidPoint() As gPoint
            Return New VectorDraw.Geometry.gPoint(0, TOP_LENGTH + CIRCLE_RADIUS)
        End Function

        Private Function GetArc() As vdArc
            Dim arc As New vdArc(Me.Document)
            arc.setDocumentDefaults()
            arc.Radius = CIRCLE_RADIUS
            arc.PenColor = Me.PenColor
            arc.PenWidth = Me.PenWidth
            arc.HatchProperties = New VectorDraw.Professional.vdObjects.vdHatchProperties(VectorDraw.Professional.Constants.VdConstFill.VdFillModeSolid)
            arc.HatchProperties.SetUnRegisterDocument(Me.Document)
            arc.HatchProperties.FillColor.SystemColor = Me.FillColor
            arc.Center = New gPoint(0, 0)
            arc.StartAngle = (0 * Math.PI) / 180
            arc.EndAngle = (180 * Math.PI) / 180
            Return arc
        End Function

        Private Function GetPeak(arc As vdArc) As vdPolyline
            Dim peak As New vdPolyline(Me.Document)
            peak.setDocumentDefaults()
            peak.PenWidth = Me.PenWidth
            peak.PenColor = Me.PenColor
            peak.VertexList.Add(arc.getStartPoint)
            peak.VertexList.Add(New gPoint(0, -TOP_LENGTH))
            peak.VertexList.Add(arc.getEndPoint)
            peak.HatchProperties = New VectorDraw.Professional.vdObjects.vdHatchProperties(VectorDraw.Professional.Constants.VdConstFill.VdFillModeSolid)
            peak.HatchProperties.SetUnRegisterDocument(Me.Document)
            peak.HatchProperties.FillColor.SystemColor = Me.FillColor
            Return peak
        End Function

        Public Overrides Sub FillShapeEntities(ByRef entities As VectorDraw.Professional.vdCollections.vdEntities)
            MyBase.FillShapeEntities(entities)

            Dim arc As vdArc = GetArc()
            entities.Add(arc)
            entities.Add(GetPeak(arc))

            If Not String.IsNullOrWhiteSpace(Text) Then
                Dim innerText As New vdText(Me.Document)
                With innerText
                    .setDocumentDefaults()
                    .TextString = Text
                    .Height = CIRCLE_RADIUS / 2
                    .PenColor.SystemColor = TextColor
                    .Rotation = -Rotation
                    .VerJustify = VectorDraw.Professional.Constants.VdConstVerJust.VdTextVerCen
                    .HorJustify = VectorDraw.Professional.Constants.VdConstHorJust.VdTextHorCenter
                    .InsertionPoint = arc.Center
                End With
                entities.Add(innerText)
            End If

        End Sub

        Public Overrides Sub MatchProperties(_from As VectorDraw.Professional.vdObjects.vdPrimary, thisDocument As VectorDraw.Professional.vdObjects.vdDocument)
            MyBase.MatchProperties(_from, thisDocument)
            With CType(_from, VdStamp)
                Me.Reference = .Reference
                Me.Origin = .Origin
                Me.Text = .Text
                Me.TextColor = .TextColor
            End With
        End Sub

        Private Function GetNewNormalizedPoint(x As Double, y As Double) As gPoint
            Dim pt As New gPoint(x, y)
            Return pt.Polar(Rotation, Me.Origin.Distance2D(New gPoint(x, y)))
        End Function

        Public Overrides Function GetGripPoints() As VectorDraw.Geometry.gPoints
            Dim gripPoints As gPoints = New gPoints()
            gripPoints.Add(New gPoint(0, 0))
            gripPoints.Add(New gPoint(0, -TOP_LENGTH))

            ECSMatrix.Transform(gripPoints)
            Return gripPoints
        End Function


        Private Function GetContrastColor(color As Color) As Color
            Dim d As Integer = 0

            'HINT: Counting the perceptive luminance (eye favors green color...) 
            Dim a As Double = 1 - (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255

            If a < 0.5 Then
                d = 0
            Else
                'HINT: bright colors - black font
                d = 255
            End If
            ' HINT: dark colors - white font
            Return Color.FromArgb(d, d, d)
        End Function

        Friend Function IsStampOverlaying() As Boolean
            If Me.OwningCanvas IsNot Nothing Then
                For Each kblStamp As VdStamp In Me.OwningCanvas.StampMapper.SelectMany(Function(KV) KV.Value).Where(Function(stamp) stamp.Reference.KblId = Me.KblId)
                    If kblStamp IsNot Me Then
                        Dim intPoints As New gPoints
                        If Me.IntersectWith(kblStamp, VectorDraw.Professional.Constants.VdConstInters.VdIntOnBothOperands, intPoints) Then
                            If intPoints.Count > 0 Then
                                Dim poly As New vdPolyline(kblStamp.Document, New Vertexes(intPoints))
                                poly.PenWidth = 1
                                kblStamp.Document.Model.Entities.Add(poly)
                                Return True
                            End If
                        End If
                    End If
                Next
            Else
                Throw New NullReferenceException("Can't check for overlaying. Owningcanvas is not set!")
            End If
            Return False
        End Function

        Friend Shared Function GetStampPositionTo(referenceCenter As gPoint, absolutePosition As gPoint) As gPoint
            Return New gPoint(referenceCenter.x - absolutePosition.x + VdStamp.OBJECT_DIST, referenceCenter.y - absolutePosition.y + OBJECT_DIST)
        End Function

    End Class

End Namespace
