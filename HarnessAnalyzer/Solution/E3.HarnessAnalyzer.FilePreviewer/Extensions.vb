Imports System.Drawing
Imports System.Runtime.CompilerServices

<HideModuleName>
Friend Module Extensions

    ''' <summary>
    ''' Scale rectangle to fixed size with optional keeping of aspect ratios
    ''' </summary>
    ''' <param name="src"></param>
    ''' <param name="newSize"></param>
    ''' <param name="keepWidth"></param>
    ''' <param name="keepHeight"></param>
    ''' <returns></returns>
    <Extension>
    Friend Function Resize(ByVal src As SizeF, ByVal newSize As SizeF, Optional ByVal keepWidth As Boolean = True, Optional ByVal keepHeight As Boolean = False) As SizeF
        Dim destSize As New SizeF()
        Dim sourceAspect As Single = src.Width / src.Height
        Dim destAspect As Single = newSize.Width / newSize.Height

        If sourceAspect > destAspect Then
            destSize.Width = newSize.Width
            destSize.Height = newSize.Width / sourceAspect

            If keepHeight Then
                Dim resizePerc As Single = newSize.Height / destSize.Height
                destSize.Width = newSize.Width * resizePerc
                destSize.Height = newSize.Height
            End If
        Else
            destSize.Height = newSize.Height
            destSize.Width = newSize.Height * sourceAspect

            If keepWidth Then
                Dim resizePerc As Single = newSize.Width / destSize.Width
                destSize.Width = newSize.Width
                destSize.Height = newSize.Height * resizePerc
            End If
        End If

        Return destSize
    End Function

    <Extension>
    Friend Function GetAsBitmapFast(browser As WebBrowser, size As System.Drawing.Size) As System.Drawing.Bitmap
        Dim bmp As New Bitmap(size.Width, size.Height)
        browser.DrawToBitmapFast(bmp, New Rectangle(0, 0, size.Width, size.Height))
        Return bmp
    End Function

    <Extension>
    Friend Sub DrawToBitmapFast(browser As WebBrowser, bmp As Bitmap, targetRect As Rectangle)
        Using gr As Graphics = Graphics.FromImage(bmp)
            gr.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
            Dim hDC = gr.GetHdc()
            Dim viewObject = CType(browser.Document.DomDocument, IViewObject)
            viewObject.Draw(CUInt(DVASPECT.DVASPECT_CONTENT), -1, CType(0, IntPtr), CType(0, IntPtr), CType(0, IntPtr), hDC, targetRect, CType(0, IntPtr), CType(0, IntPtr), 0)
            gr.ReleaseHdc(hDC)
        End Using
    End Sub

#Region "KBLViewing-Extensions (currently unsued because of the need of vectordraw)"
    '<Extension>
    'Friend Function GetImage(control As VectorDrawBaseControl) As System.Drawing.Bitmap
    '    Const BOUNDINGBOX_ENLARGEMENT As Single = 0.01 '1%
    '    Dim boundingBox As New Box(control.ActiveDocument.ActiveLayOut.Entities.GetBoundingBox(False, False))
    '    boundingBox.AddWidth(boundingBox.Width * BOUNDINGBOX_ENLARGEMENT)

    '    Dim bx As Double = boundingBox.Width / boundingBox.Height
    '    Dim sc As Double = Screen.PrimaryScreen.Bounds.Width / Screen.PrimaryScreen.Bounds.Height
    '    Dim bmp As Bitmap = Nothing
    '    If (bx >= sc) Then
    '        bmp = New Bitmap(Screen.PrimaryScreen.Bounds.Width, CInt(Screen.PrimaryScreen.Bounds.Width / bx))
    '    ElseIf (boundingBox.Height > 0) AndAlso (boundingBox.Width > 0) Then
    '        bmp = New Bitmap(CInt(Screen.PrimaryScreen.Bounds.Height * bx), Screen.PrimaryScreen.Bounds.Height)
    '    End If

    '    If bmp IsNot Nothing Then
    '        Dim graphics As Graphics = Graphics.FromImage(bmp)
    '        With graphics
    '            .SmoothingMode = Drawing2D.SmoothingMode.HighSpeed
    '            .TextRenderingHint = Drawing.Text.TextRenderingHint.SingleBitPerPixel
    '            .InterpolationMode = Drawing.Drawing2D.InterpolationMode.Low
    '        End With

    '        boundingBox.TransformBy(control.ActiveDocument.ActiveLayOut.World2ViewMatrix)

    '        Dim backgroundColor As Color = control.ActiveDocument.Palette.Background
    '        With control.ActiveDocument
    '            .Palette.Background = Color.White
    '            .ActiveLayOut.RenderToGraphics(graphics, boundingBox, bmp.Width, bmp.Height)
    '            .Palette.Background = backgroundColor
    '            .ActiveLayOut.Label = String.Empty
    '        End With
    '        Return bmp
    '    End If
    '    Return Nothing
    'End Function

    '<Extension>
    'Friend Function GetImage(container As E3.Lib.IO.Files.Hcv.PreviewImageContainerFile) As System.Drawing.Image
    '    Using s = container.GetDataAsStream
    '        Return System.Drawing.Image.FromStream(s)
    '    End Using
    'End Function

    '<Extension>
    'Friend Function CreatePreviewContainer(kbl As E3.Lib.IO.Files.Hcv.KblContainerFile) As PreviewImageContainerFile
    '    Dim vdraw As New VectorDraw.Professional.Control.VectorDrawBaseControl
    '    If kbl.CreateVdPreview(vdraw).IsSuccess Then
    '        Dim bmp As New System.Drawing.Bitmap(1024, 768)
    '        vdraw.DrawToBitmap(bmp, New Drawing.Rectangle(0, 0, 1024, 768))
    '        Using ms As New System.IO.MemoryStream
    '            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg)
    '            Return New PreviewImageContainerFile(ms.ToArray)
    '        End Using
    '    Else
    '        Return Nothing
    '    End If
    'End Function

    '<Extension>
    'Friend Function CreateVdPreview(kbl As E3.Lib.IO.Files.Hcv.KblContainerFile, ByRef vdraw As VectorDraw.Professional.Control.VectorDrawBaseControl) As Result
    '    Using s = kbl.GetDataAsStream
    '        Dim kblMapper As KblMapper = KblMapper.Create(s)
    '        vdraw.CreateControl()
    '        vdraw.EnsureDocument()

    '        Dim draftConv As New DraftConverter(kblMapper, vdraw)
    '        Return draftConv.ConvertDraftDrawing()
    '    End Using
    '    Return Nothing
    'End Function
#End Region


End Module
