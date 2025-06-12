Imports System.Drawing.Printing

Imports VectorDraw.Generics
Imports VectorDraw.Geometry
Imports VectorDraw.Professional.vdCollections
Imports VectorDraw.Professional.vdObjects
Imports VectorDraw.Professional.vdPrimaries
Imports VectorDraw.Render

Namespace Printing

    Public Class VdPrinting
        Implements IDisposable

        Private _document As vdDocument
        Private _entities As vdEntities
        Private _hardMargins As New VectorDraw.Professional.vdObjects.MARGINS(0, 0, 0, 0)
        Private _landscape As Boolean
        Private _layout As vdLayout
        Private _margins As New VectorDraw.Professional.vdObjects.MARGINS(0, 0, 0, 0)
        Private _maxBitmapSize As Integer = 255
        Private _paperSize As New Rectangle(0, 0, 827, 1169)
        Private _printScale As New PrinterScale(1.0, 1.0)
        Private _printWindow As New Box
        Private _resolution As Integer = 96
        Private _render As vdRender

        Public Sub New(document As vdDocument)
            _document = document
            _entities = document.ActiveLayOut.Entities
            _layout = document.ActiveLayOut
            _render = _layout.Render
        End Sub

        Property OnlyMarginsPreview As Boolean
        Property PrintExtends As Boolean
        Property PrintSelection As Box
        Property ScaleDenumerator As Double = 1.0
        Property ScaleNumerator As Double = 1.0
        Property ScaleToFit As Boolean = True
        Property CenterToPaper As Boolean = True
        Property DocumentName As String
        Property CancelPrint As Boolean = False
        ReadOnly Property IsPrinting As Boolean

        Private Sub DrawOnRender(render As vdRender)
            render.Clear(render.BkColor, True)
            For Each figure As vdFigure In _entities
                If CancelPrint Then Return
                If (Not figure.Layer.Frozen) Then
                    If TypeOf figure Is VdSVGGroup Then
                        With CType(figure, VdSVGGroup)
                            Dim oldLighting As Lighting = .Lighting
                            If .Lighting = Lighting.Highlight Then ' remove only highlight's ( not LowLight )
                                .Lighting = Lighting.Normal
                            End If
                            .Draw(render)
                            .Lighting = oldLighting
                        End With
                    Else
                        figure.Draw(render)
                    End If
                End If
            Next
        End Sub

        Private Function GetPrintSelection() As Box
            Dim box As Box = PrintSelection
            If (box Is Nothing) Then box = New Box

            If (box.IsEmpty OrElse box.Height = 0.0) Then
                Dim layout As vdLayout = TryCast(_render.OwnerObject, vdLayout)
                If (layout IsNot Nothing) Then
                    Dim b As New Box(layout.Limits)
                    b.TransformBy(layout.User2WorldMatrix)
                    box.AddBox(b)
                End If
            End If

            If (box.IsEmpty OrElse box.Height = 0.0) Then box = New Box(New gPoint(), New gPoint(10.0, 10.0))

            box.TransformBy(_render.CurrentMatrix)

            If (_render.IsPerspectiveModeOn) Then
                Dim fittingViewSize As Double = _render.GetFittingViewSize(box)
                Return New Box(New gPoint(fittingViewSize * _render.AspectRatio, fittingViewSize) * 0.5, New gPoint(fittingViewSize * _render.AspectRatio, fittingViewSize) * 0.5)
            End If
            Return box
        End Function

        Private Function GetEntsBoundingBox(ByVal onlyVisible As Boolean, ByVal recalculate As Boolean) As Box
            With _entities
                If (recalculate OrElse .InternalGetBoundingBox.IsEmpty) Then
                    .InternalGetBoundingBox.Empty()
                    If (Not .Document Is Nothing) Then
                        .Document.UndoHistory.PushEnable(False)
                        .Document.FreezeModifyEvents.Push(True)

                    End If

                    Dim figure As vdFigure
                    For Each figure In _entities
                        If Me.CancelPrint Then Exit For
                        If figure.Deleted Then
                            Continue For
                        End If
                        If (Not onlyVisible OrElse figure.IsVisible) Then
                            .InternalGetBoundingBox.AddBox(figure.BoundingBox)
                        End If

                    Next
                    If (Not .Document Is Nothing) Then
                        .Document.UndoHistory.PopEnable()
                        .Document.FreezeModifyEvents.Pop()

                    End If
                End If
                Return .InternalGetBoundingBox
            End With
        End Function

        Private Function GetPrintExtends() As Box
            Dim box As New Box(GetEntsBoundingBox(True, False))

            If (box.IsEmpty OrElse box.Height = 0.0) Then
                Dim layout As vdLayout = TryCast(_render.OwnerObject, vdLayout)
                If (layout IsNot Nothing) Then
                    Dim b As New Box(layout.Limits)
                    b.TransformBy(layout.User2WorldMatrix)
                    box.AddBox(b)
                End If
            End If

            If (box.IsEmpty OrElse box.Height = 0.0) Then box = New Box(New gPoint(), New gPoint(10.0, 10.0))

            box.TransformBy(_render.CurrentMatrix)

            If (_render.IsPerspectiveModeOn) Then
                Dim fittingViewSize As Double = _render.GetFittingViewSize(box)
                Return New Box(New gPoint(fittingViewSize * _render.AspectRatio, fittingViewSize) * 0.5, New gPoint(fittingViewSize * _render.AspectRatio, fittingViewSize) * 0.5)
            End If

            Return box
        End Function

        Private Function GetScaleToFit(printedMarginArea As Rectangle) As PrinterScale
            Dim printableWidth As Double = printedMarginArea.Width
            Dim printableHeight As Double = printedMarginArea.Height

            printableWidth *= 0.254
            printableHeight *= 0.254

            Dim width As Double = _printWindow.Width
            Dim height As Double = _printWindow.Height

            If (Globals.AreEqual(width, 0.0, Globals.DefaultScaleEquality) OrElse Globals.AreEqual(height, 0.0, Globals.DefaultScaleEquality)) Then Return New PrinterScale(1.0, 1.0)
            If (printableWidth / width < printableHeight / height) Then Return New PrinterScale(1.0, width / printableWidth)

            Return New PrinterScale(1.0, height / printableHeight)
        End Function

        Private Function PaperSizeInHofInches() As RectangleF
            If (_landscape) Then Return New RectangleF(0.0F, 0.0F, _paperSize.Height, _paperSize.Width)

            Return New RectangleF(0.0F, 0.0F, _paperSize.Width, _paperSize.Height)
        End Function

        Private Function PrintablePaperMaringsInHofInches() As RectangleF
            Dim paperSize As RectangleF = PaperSizeInHofInches()
            Dim result As New RectangleF

            With result
                .X = paperSize.Left + _margins.Left
                .Y = paperSize.Top + _margins.Top
                .Width = paperSize.Width - _margins.Left - _margins.Right
                .Height = paperSize.Height - _margins.Top - _margins.Bottom
            End With

            Return result
        End Function

        Private Function PrintablePaperSizeInHofInches() As RectangleF
            Dim paperSize As RectangleF = PaperSizeInHofInches()
            Dim result As New RectangleF

            With result
                .X = paperSize.Left + _hardMargins.Left
                .Y = paperSize.Top + _hardMargins.Top
                .Width = paperSize.Width - _hardMargins.Left - _hardMargins.Right
                .Height = paperSize.Height - _hardMargins.Top - _hardMargins.Bottom
            End With

            Return result
        End Function

        Private Function PrintableRectInDU() As Box
            Dim rect As RectangleF = PrintablePaperMaringsInHofInches()
            Dim w As Double = rect.Width * PrintScaleHofInches2DU()
            Dim h As Double = rect.Height * PrintScaleHofInches2DU()
            Dim minWidth As Double = Math.Min(w, _printWindow.Width)
            Dim minHeight As Double = Math.Min(h, _printWindow.Height)
            Dim minP As New gPoint(_printWindow.Min)
            Dim maxP As New gPoint(_printWindow.Max)
            maxP.x = _printWindow.Min.x + minWidth
            minP.y = _printWindow.Max.y - minHeight

            Return New Box(minP, maxP)
        End Function

        Private Function PrintableRectInHofInches() As Rectangle
            Dim rect As RectangleF = PrintablePaperMaringsInHofInches()
            Dim w As Double = rect.Width * PrintScaleHofInches2DU()
            Dim h As Double = rect.Height * PrintScaleHofInches2DU()
            Dim minWidth As Double = Math.Min(w, _printWindow.Width)
            Dim minHeight As Double = Math.Min(h, _printWindow.Height)
            Dim width As Single = CSng(minWidth / PrintScaleHofInches2DU())
            Dim height As Single = CSng(minHeight / PrintScaleHofInches2DU())

            Dim result As New Rectangle(0, 0, CInt(width), CInt(height))
            result.X += _hardMargins.Left + _margins.Left
            result.Y += _hardMargins.Top + _margins.Top

            Return result
        End Function

        Public Sub PrintPage(e As PrintPageEventArgs)
            Try
                If Not CancelPrint Then
                    _IsPrinting = True
                    Try
                        Dim printArea As New PrintAreaData(e)
                        If Not CancelPrint Then
                            UpdatePrintingProperties(e.PageSettings)

                            _printWindow = If(PrintExtends, GetPrintExtends(), GetPrintSelection())
                            _printScale = If(ScaleToFit, GetScaleToFit(printArea.PrintedMarginArea), New PrinterScale(ScaleNumerator, ScaleDenumerator))

                            'ScaleNumerator = _printScale.Numerator 'HINT HH: disabled because we only use the scaleToFitEnumerator internally if the user has set it externally this makes it possible to switch between the scaleToFit and the old given ScaleEnumerators which is more the behavior that's expected from the gui (user is not caring about the enumerators when scaleToFit and vice versa)
                            'ScaleDenumerator = _printScale.Denumerator

                            Dim printRectHofInches As Rectangle = PrintableRectInHofInches()
                            With printRectHofInches
                                .Offset(-_hardMargins.Left, -_hardMargins.Top)

                                If (.IsEmpty) Then
                                    If (Single.IsNaN(.Height)) Then .Height = 10
                                    If (Single.IsNaN(.Width)) Then .Width = 10
                                End If

                                If Not CancelPrint Then
                                    If OnlyMarginsPreview Then
                                        Dim printREct As Rectangle = printArea.GetOutlinePrintRect(printRectHofInches, Me.CenterToPaper)
                                        e.Graphics.DrawDashedRectangle(Color.Gray, Rectangle.Truncate(printArea.RealPrintableArea))
                                        e.Graphics.DrawRectangle(Pens.Red, printArea.PrintedMarginArea)
                                        e.Graphics.DrawRectangle(If(printREct.Equals(printArea.PrintedMarginArea), New Pen(Color.Blue) With {.DashStyle = Drawing2D.DashStyle.Dash}, New Pen(Color.Blue)), printREct) ' HINT: when margin-rect and print-rect are equal draw rect dashStyle to not totally overdraw the margin-rect
                                    Else
                                        With printArea.GetInchPoint(printRectHofInches, e.Graphics.DpiX, e.Graphics.DpiY, Me.CenterToPaper)
                                            RenderToGraphics(PrintableRectInDU, .X, .Y, e.Graphics, 1.0, 1.0, Color.White, 2)
                                        End With
                                    End If
                                End If
                            End With
                        End If
                    Finally
                        _IsPrinting = False
                    End Try
                End If
            Finally
                CancelPrint = False
            End Try
        End Sub

        Private Function PrintScaleHofInches2DU() As Single
            Return CSng(0.254F * (_printScale.Denumerator / _printScale.Numerator))
        End Function

        Private Sub RenderToGraphics(printableArea As Box, x As Integer, y As Integer, gr As Graphics, previewScale As Double, scaleGraphics As Double, bkColor As Color, addPixelsBound As Integer)
            gr.PageUnit = GraphicsUnit.Pixel

            Dim printerScale As Double = _printScale.Numerator / _printScale.Denumerator
            Dim height As Integer = CInt(gr.DpiY / 25.4 * printableArea.Height * previewScale * printerScale * scaleGraphics)
            Dim width As Integer = CInt(gr.DpiX / 25.4 * printableArea.Width * previewScale * printerScale * scaleGraphics)

            If (width <= 0 OrElse height <= 0) Then Exit Sub

            Dim imgReduceSize As Integer = 1
            Dim maxBmpMemorySize As Integer = _document.GlobalRenderProperties.MaxBmpMemorySize

            _document.GlobalRenderProperties.MaxBmpMemorySize = _maxBitmapSize * 1024 * 1024

            Dim renderViewPropertiesArray As vdArray(Of VdRenderViewProperties) = SplitRender(New VdRenderViewProperties(printableArea.Height, printableArea.MidPoint, width, height, New Drawing.Point(x, y)), gr, If(_render.IsWire2d, 0, _maxBitmapSize))

            If (_layout IsNot Nothing) AndAlso (_layout.Render.PerspectiveMod = vdRender.VdConstPerspectiveMod.PerspectON) AndAlso (Not _layout.Render.IsWire2d) Then
                renderViewPropertiesArray = New vdArray(Of VdRenderViewProperties)(New VdRenderViewProperties() {New VdRenderViewProperties(printableArea.Height, printableArea.MidPoint, width, height, New Drawing.Point(x, y))})
                imgReduceSize = CInt(width / _document.GlobalRenderProperties.GetImageReduceSize(New Drawing.Size(width, height), 32).Width)
            End If

            'TryInvoke(Sub() _entities.u()) 'HINT HH: Removed: makes all entities invalid which also forces the whole drawing to be re-drawn after printing is finished (f.e. after the printForm is closed), which can take very long! - We decided that this update is not needed for us (code was vom vectorDraw) and we handle the temporarily de-highlight of the entities manually

            Dim background As Color = _document.Palette.Background
            Dim forground As Color = _document.Palette.Forground

            If (Not bkColor.IsEmpty) Then
                _document.Palette.SetBkColorFixForground(bkColor)
            Else
                _document.Palette.SetBkColorFixForground(Color.White)
            End If

            Dim counter As Integer = 0
            Dim penStyles As vdGdiPenStyles = _document.Palette.AsSystemPalette()

            For Each penStyle As vdGdiPenStyle In penStyles
                If CancelPrint Then Exit For
                'If (counter < _penWidth.Count) Then penStyle.PrintingExtraWidth = _penWidth(counter)
                counter += 1
            Next

            If Not CancelPrint Then
                For Each renderViewProperties As VdRenderViewProperties In renderViewPropertiesArray
                    Dim render As vdRender = Nothing

                    If CancelPrint Then Exit For

                    If (_render.IsWire2d) Then
                        render = New GDIPlusRender(Nothing)
                        With render
                            .MatchProperties(_render)
                            .graphics = gr
                            .UpperLeft = renderViewProperties.UpperLeft
                            .Width = CInt(renderViewProperties.Width / imgReduceSize)
                            .Height = CInt(renderViewProperties.Height / imgReduceSize)
                        End With
                    Else
                        render = New GDIPlusRender(Nothing)
                        With render
                            .MatchProperties(_render)

                            Dim bitmap As Bitmap = New Bitmap(CInt(renderViewProperties.Width / imgReduceSize), CInt(renderViewProperties.Height / imgReduceSize), gr)
                            Dim graphics As Graphics = Graphics.FromImage(bitmap)

                            vdRender.MatchGraphicsProperties(graphics, gr)

                            .MemoryBitmap = bitmap
                            .graphics = graphics
                            .UpperLeft = Nothing
                            .Width = CInt(renderViewProperties.Width / imgReduceSize)
                            .Height = CInt(renderViewProperties.Height / imgReduceSize)
                        End With
                    End If

                    If Not Me.CancelPrint Then
                        With render
                            .ViewCenter = renderViewProperties.ViewCenter
                            .ViewSize = renderViewProperties.ViewSize
                            .ViewSize += addPixelsBound * .PixelSize
                            .Palette = penStyles
                            .BkColor = penStyles.Background
                            .ColorPalette = vdRender.ColorDisplay.TrueColor

                            .Init()

                            .Display = vdRender.DisplayMode.PRINT_PRINTER
                            .PrintPreviewScale = previewScale
                            .PrinterScale = printerScale

                            .StartDraw(True)


                            If .Started Then
                                If Not CancelPrint Then
                                    DrawOnRender(render)
                                End If
                                .EndDraw()
                            End If

                            .Refresh()

                            If (.MemoryBitmap IsNot Nothing AndAlso .graphics IsNot gr) Then
                                .graphics.Flush()
                                gr.Flush()
                                .graphics.Dispose()
                                .MemoryBitmap.Dispose()
                            End If

                            .Destroy(False)
                        End With
                    End If
                Next
            End If

            _document.Palette.Background = background
            _document.Palette.Forground = forground

            ' _entities.update() 'HINT HH: Removed (unclear why it's twice here): makes all entities invalid which also forces the whole drawing to be re-drawn after printing is finished (f.e. after the printForm is closed), which can take very long! - We decided that this update is not needed for us (code was vom vectorDraw) and we handle the temporarily de-highlight of the entities manually

            _document.GlobalRenderProperties.MaxBmpMemorySize = maxBmpMemorySize
        End Sub

        Private Function SplitRender(renderViewProps As VdRenderViewProperties, gr As Graphics, maxBitmapSize As Integer) As vdArray(Of VdRenderViewProperties)
            Dim renderViewPropertiesArray As New vdArray(Of VdRenderViewProperties)
            Dim width As Integer = renderViewProps.Width
            Dim height As Integer = renderViewProps.Height
            Dim size As Double = renderViewProps.ViewSize / height
            Dim box As New Box

            box.AddPoint(New gPoint(renderViewProps.ViewCenter.x - size * renderViewProps.Width / 2.0, renderViewProps.ViewCenter.y - size * renderViewProps.Height / 2.0))
            box.AddPoint(New gPoint(renderViewProps.ViewCenter.x + size * renderViewProps.Width / 2.0, renderViewProps.ViewCenter.y + size * renderViewProps.Height / 2.0))

            Dim aspectRatioX As Integer = width
            Dim aspectRatioY As Integer = height
            Dim bitmapDivision As Integer = vdRender.GetBitmapDivision(gr, width, height, maxBitmapSize)

            aspectRatioY = CInt(aspectRatioY / bitmapDivision)
            aspectRatioX = CInt(aspectRatioX / bitmapDivision)

            If (bitmapDivision = 0) Then bitmapDivision = 1

            Dim widthArray As Integer(,) = New Integer(bitmapDivision - 1, bitmapDivision - 1) {}
            Dim heightArray As Integer(,) = New Integer(bitmapDivision - 1, bitmapDivision - 1) {}

            For i As Integer = 0 To bitmapDivision - 1
                For j As Integer = 0 To bitmapDivision - 1
                    widthArray(i, j) = aspectRatioX
                    heightArray(i, j) = aspectRatioY

                    If (j = bitmapDivision - 1) Then widthArray(i, j) = width - aspectRatioX * j
                    If (i = bitmapDivision - 1) Then heightArray(i, j) = height - aspectRatioY * i
                Next
            Next

            For k As Integer = 0 To bitmapDivision - 1
                For l As Integer = 0 To bitmapDivision - 1
                    Dim currentWidth As Integer = widthArray(k, l)
                    Dim currentHeight As Integer = heightArray(k, l)
                    Dim renderViewProperties As New VdRenderViewProperties

                    With renderViewProperties
                        .Width = currentWidth
                        .Height = currentHeight

                        Dim pt As New gPoint
                        pt.x = box.Left
                        pt.y = box.Top

                        Dim offsetX As Integer = 0
                        Dim offsetY As Integer = 0

                        For m As Integer = 0 To l - 1
                            pt.x += widthArray(0, m) * size
                            offsetX += widthArray(0, m)
                        Next

                        For n As Integer = 0 To k - 1
                            pt.y -= heightArray(n, 0) * size
                            offsetY += heightArray(n, 0)
                        Next

                        pt.x += currentWidth * size / 2.0
                        pt.y -= currentHeight * size / 2.0

                        .ViewSize = size * currentHeight
                        .ViewCenter.CopyFrom(pt)
                        .UpperLeft = New Drawing.Point(offsetX + renderViewProps.UpperLeft.X, offsetY + renderViewProps.UpperLeft.Y)
                    End With

                    renderViewPropertiesArray.AddItem(renderViewProperties)
                Next
            Next

            Return renderViewPropertiesArray
        End Function

        Private Sub UpdatePrintingProperties(settings As PageSettings)
            With settings
                _landscape = .Landscape
                _resolution = Math.Max(settings.PrinterResolution.X, settings.PrinterResolution.Y)

                If (_resolution <= 0) Then
                    Dim graphics As Graphics = settings.PrinterSettings.CreateMeasurementGraphics()

                    _resolution = Math.Max(CInt(graphics.DpiX), CInt(graphics.DpiY))
                    graphics.Dispose()
                End If

                _margins.Left = .Margins.Left
                _margins.Top = .Margins.Top
                _margins.Right = .Margins.Right
                _margins.Bottom = .Margins.Bottom

                _paperSize.Width = .PaperSize.Width
                _paperSize.Height = .PaperSize.Height

                _hardMargins.SetFrom(New VectorDraw.Professional.vdObjects.MARGINS)
                _hardMargins.Left = CInt(settings.HardMarginX)
                _hardMargins.Top = CInt(settings.HardMarginY)

                With settings.PrintableArea
                    If (.Width <> 0.0F AndAlso .Height <> 0.0F) Then

                        If (Not _landscape) Then
                            _hardMargins.Right = CInt(settings.PaperSize.Width - .Width - _hardMargins.Left)
                            _hardMargins.Bottom = CInt(settings.PaperSize.Height - .Height - _hardMargins.Top)
                        Else
                            _hardMargins.Right = CInt(settings.PaperSize.Height - .Height - _hardMargins.Left)
                            _hardMargins.Bottom = CInt(settings.PaperSize.Width - .Width - _hardMargins.Top)
                        End If

                    Else
                        _hardMargins.Right = 0
                        _hardMargins.Bottom = 0
                    End If

                End With

                If (_hardMargins.Right < 0) Then _hardMargins.Right = 0
                If (_hardMargins.Bottom < 0) Then _hardMargins.Bottom = 0
            End With
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                End If
            End If
            Me.disposedValue = True
        End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

#End Region

    End Class

End Namespace