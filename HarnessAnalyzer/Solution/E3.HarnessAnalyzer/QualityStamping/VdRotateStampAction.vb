Imports VectorDraw.Geometry
Imports VectorDraw.Professional.Actions
Imports VectorDraw.Professional.vdPrimaries

Namespace QualityStamping

    Public Class VdRotateStampAction
        Inherits ActionEntityEx

        Private _orgStamp As VdStamp
        Private _rotatingStamp As VdStamp
        Private _isCompleted As Boolean = False

        Public Sub New(vdStamp As VdStamp)
            MyBase.New(New gPoint(vdStamp.Origin.x, vdStamp.Origin.y), vdStamp.Document.ActiveLayOut)
            _orgStamp = vdStamp
            _rotatingStamp = CType(vdStamp.Clone(vdStamp.Document), QualityStamping.VdStamp)
        End Sub

        Protected Overrides Sub [End]()
            _isCompleted = True
            MyBase.Layout.Document.ActionControl.Cursor = Nothing
            MyBase.[End]()
        End Sub

        Protected Overrides Sub OnMyPositionChanged(ByVal newPosition As gPoint)
            If _isCompleted Then Return

            Dim angle As Double = Me.ReferencePoint.GetAngle(newPosition) + ((90 * Math.PI) / 180)
            _rotatingStamp.Rotation = angle
        End Sub

        Public Overrides ReadOnly Property Entity() As vdFigure
            Get
                Return _rotatingStamp
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

        Public Shared Function CmdRotateStamp(ByVal vdStamp As VdStamp) As Boolean
            'HINT we cannot move terminals from one component to another as there can be innerconnectivity or inner circuit diagrams which were affected then
            Dim action As New VdRotateStampAction(vdStamp)
            Dim hitPoint As gPoint = TryCast(vdStamp.Document.ActionUtility.getUserActionEntity(action), gPoint)
            If (hitPoint Is Nothing) Then Return False

            vdStamp.Rotation = CType(action.Entity, VdStamp).Rotation
            vdStamp.Reference.Rotation = CType(action.Entity, VdStamp).Rotation
            vdStamp.Invalidate()
            vdStamp.Update()
            vdStamp.Document.Invalidate(New Rectangle(CInt(vdStamp.BoundingBox.Left - HarnessAnalyzer.[Shared].SELECTING_SIZE), CInt(vdStamp.BoundingBox.Top - HarnessAnalyzer.[Shared].SELECTING_SIZE), CInt(vdStamp.BoundingBox.Width + HarnessAnalyzer.[Shared].SELECTING_SIZE), CInt(vdStamp.BoundingBox.Height + HarnessAnalyzer.[Shared].SELECTING_SIZE)))

            Return True
        End Function

    End Class

End Namespace