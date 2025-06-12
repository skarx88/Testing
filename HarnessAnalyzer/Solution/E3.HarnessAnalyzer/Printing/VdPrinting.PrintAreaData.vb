Imports System.Drawing.Printing

Namespace Printing

    Partial Class VdPrinting
        Friend Class PrintAreaData

            Public Sub New(e As PrintPageEventArgs)
                RealPrintableArea = GetRealPrintableArea(e.PageSettings, e.PageBounds)
                PrintedMarginArea = GetPrintedMarginArea(e, RealPrintableArea)
            End Sub

            ReadOnly Property RealPrintableArea As Rectangle
            ReadOnly Property PrintedMarginArea As Rectangle

            Private Function GetPrintedMarginArea(e As PrintPageEventArgs, realPrintableArea As Rectangle) As Rectangle
                Dim marginBounds As New Rectangle(e.MarginBounds.Location, e.MarginBounds.Size)
                marginBounds.Intersect(realPrintableArea)
                Return Rectangle.Truncate(marginBounds)
            End Function

            Private Function GetRealPrintableArea(pageSettings As PageSettings, pageBounds As Rectangle) As Rectangle
                With pageSettings.PrintableArea
                    Dim x As Integer = CInt(If(pageSettings.Landscape, .Y, .X))
                    Dim y As Integer = CInt(If(pageSettings.Landscape, .X, .Y))
                    Dim width As Integer = CInt(pageBounds.Width - (2 * pageSettings.HardMarginX))
                    Dim height As Integer = CInt(pageBounds.Height - (2 * pageSettings.HardMarginY))
                    Return New Rectangle(x, y, width, height)
                End With
            End Function

            Public Function GetOutlinePrintRect(printableRectInHofInches As Rectangle, Optional centerToPaper As Boolean = False) As Rectangle
                With printableRectInHofInches
                    Dim rect As New Rectangle(.X, .Y, .Width, .Height)
                    If centerToPaper Then
                        Dim centerVec As Drawing.Point = GetCenterVector(PrintedMarginArea, printableRectInHofInches)
                        rect.X += centerVec.X
                        rect.Y += centerVec.Y
                    End If
                    Return rect
                End With
            End Function

            Public Function GetInchPoint(printableRectInHofInches As Rectangle, dpix As Single, dpiy As Single, Optional centerToPaper As Boolean = False) As Drawing.Point
                Dim inchPt As Drawing.Point
                With printableRectInHofInches
                    If centerToPaper Then
                        Dim centerVec As Drawing.Point = GetCenterVector(PrintedMarginArea, printableRectInHofInches)
                        inchPt = GetPtInInch(.Left + centerVec.X, .Top + centerVec.Y, dpix, dpiy)
                    Else
                        inchPt = GetPtInInch(.Left, .Top, dpix, dpiy)
                    End If
                End With
                Return inchPt
            End Function

            Private Function GetPtInInch(x As Single, y As Single, dpiX As Single, dpiY As Single) As Drawing.Point
                Return New Drawing.Point(CInt(x * dpiX / 100), CInt(y * dpiY / 100))
            End Function

            Private Function GetCenterVector(printedMarginArea As Rectangle, printRect As RectangleF) As Drawing.Point
                Dim centerMargin As New Drawing.Point(CInt((printedMarginArea.X + printedMarginArea.Width) / 2), CInt((printedMarginArea.Y + printedMarginArea.Height) / 2))
                Dim centerRect As New Drawing.Point(CInt((printRect.X + printRect.Width) / 2), CInt((printRect.Y + printRect.Height) / 2))
                Return New Drawing.Point(centerMargin.X - centerRect.X, centerMargin.Y - centerRect.Y)
            End Function

        End Class
    End Class

End Namespace