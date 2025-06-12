Imports VectorDraw.Geometry
Imports VectorDraw.Professional.vdFigures
Imports VectorDraw.Professional.vdPrimaries
Imports Zuken.E3.HarnessAnalyzer.QualityStamping

Public Class VdMoveStampShape
    Inherits vdShape

    Private _stampClone As VdStamp
    Private _position As New gPoint

    Public Sub New(stamp As VdStamp)
        MyBase.New()

        Me.SetUnRegisterDocument(stamp.Document)
        Me.setDocumentDefaults()

        _stampClone = CType(stamp.Clone(stamp.Document), VdStamp)
        _stampClone.Layer = stamp.Layer
    End Sub

    Public Sub MoveStampRelativeTo(pt As gPoint, rotation As Double)
        _stampClone.Origin = New gPoint(pt.x - Me.Origin.x, pt.y - Me.Origin.y)
        _stampClone.Rotation = rotation
        _position = pt

        _stampClone.Update()
        _stampClone.Invalidate()

    End Sub

    ReadOnly Property Position As gPoint
        Get
            Return _position
        End Get
    End Property

    ReadOnly Property StampRotation As Double
        Get
            Return _stampClone.Rotation
        End Get
    End Property

    Public Overrides Sub FillShapeEntities(ByRef entities As VectorDraw.Professional.vdCollections.vdEntities)
        MyBase.FillShapeEntities(entities)

        entities.Add(_stampClone)

        If _stampClone.Reference IsNot Nothing AndAlso _stampClone.Reference.vdSVGRefObjects.Count > 0 Then
            Dim line As New vdLine(Me.Document)
            line.setDocumentDefaults()
            line.StartPoint = New gPoint(0, 0)
            line.LineType = Me.Document.LineTypes.DPIDash
            line.EndPoint = _stampClone.Origin
            entities.Add(line)
        End If
    End Sub





End Class
