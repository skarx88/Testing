Imports VectorDraw.Geometry
Imports VectorDraw.Professional.vdFigures
Imports VectorDraw.Professional.vdPrimaries
Imports Zuken.E3.HarnessAnalyzer.QualityStamping

Public Class VdMovingStampWithOutline
    Inherits vdShape

    Private _polyOutline As vdPolyline
    Private _vdMovingStamp As VdMoveStampShape

    Public Sub New(vdStamp As VdStamp, polyOutline As vdPolyline)
        MyBase.New()
        Me.SetUnRegisterDocument(vdStamp.Document)
        Me.setDocumentDefaults()
        If polyOutline IsNot Nothing Then
            _polyOutline = polyOutline
            _polyOutline.PenColor.SystemColor = Color.Green
            _polyOutline.PenWidth = 0
            _polyOutline.LineType = _polyOutline.Document.LineTypes.DPIDashDot
        End If
        _vdMovingStamp = New VdMoveStampShape(vdStamp)
    End Sub

    Public Sub MoveStampTo(referencePoint As gPoint, pos As gPoint, angle As Double)
        _vdMovingStamp.Origin = referencePoint
        _vdMovingStamp.MoveStampRelativeTo(pos, angle)
        _vdMovingStamp.Update()
        _vdMovingStamp.Invalidate()
        _vdMovingStamp.Document.Invalidate() 'HINT Hack to get proper drawing restauration on free QStamps move-it is unclear why it doesn't work
    End Sub

    Public Overrides Sub FillShapeEntities(ByRef entities As VectorDraw.Professional.vdCollections.vdEntities)
        MyBase.FillShapeEntities(entities)
        If _polyOutline IsNot Nothing Then entities.Add(_polyOutline)
        entities.Add(_vdMovingStamp)
    End Sub

    ReadOnly Property StampRotation As Double
        Get
            Return _vdMovingStamp.StampRotation
        End Get
    End Property

    ReadOnly Property Position As gPoint
        Get
            Return _vdMovingStamp.Position
        End Get
    End Property

End Class
