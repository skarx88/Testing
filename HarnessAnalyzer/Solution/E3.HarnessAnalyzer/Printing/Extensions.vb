Imports System.Runtime.CompilerServices

Namespace Printing

    <HideModuleName>
    Friend Module Extensions

        <Extension>
        Public Function ToRectangle(rect As RectangleF) As Rectangle
            Return New Rectangle(CInt(rect.Location.X), CInt(rect.Location.Y), CInt(rect.Width), CInt(rect.Height))
        End Function

        <Extension>
        Public Sub DrawDashedRectangle(graphics As Graphics, color As Drawing.Color, rect As Rectangle)
            Dim dashedPen As New Pen(color)
            With dashedPen
                .DashPattern = New Single() {((1.0! * graphics.DpiY) / 25.4!), ((0.5! * graphics.DpiY) / 25.4!)}
                .DashStyle = Drawing2D.DashStyle.Custom
            End With
            graphics.DrawRectangle(dashedPen, rect)
        End Sub

    End Module

End Namespace

