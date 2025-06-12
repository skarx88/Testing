Namespace Printing

    Partial Class VdPrinting
        Private Class VdRenderViewProperties

            Public Sub New()
            End Sub

            Public Property ViewSize As Double = 1.0
            Public Property ViewCenter As New VectorDraw.Geometry.gPoint
            Public Property UpperLeft As New Drawing.Point
            Public Property Width As Integer = 1
            Public Property Height As Integer = 1
            Public Property Div As Integer = 1
            Public Property OriginalWidth As Integer = 1
            Public Property OriginalHeight As Integer = 1

            Public Sub New(viewSize As Double, viewCenter As VectorDraw.Geometry.gPoint, width As Integer, height As Integer, upperLeft As Drawing.Point)
                Me.ViewSize = viewSize
                Me.Width = width
                Me.OriginalWidth = width
                Me.Height = height
                Me.OriginalHeight = height
                Me.UpperLeft = upperLeft
                Me.ViewCenter.CopyFrom(viewCenter)
            End Sub

            Public Function GetExtends() As VectorDraw.Geometry.Box
                Dim num As Double = ViewSize / CDec(Height)
                Dim x As Double = num * CDec(Width)
                Dim pt As VectorDraw.Geometry.gPoint = New VectorDraw.Geometry.gPoint(x, ViewSize) * 0.5

                Return New VectorDraw.Geometry.Box(ViewCenter - pt, ViewCenter + pt)
            End Function

        End Class
    End Class

End Namespace