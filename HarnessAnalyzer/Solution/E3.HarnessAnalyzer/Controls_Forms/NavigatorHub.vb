Imports VectorDraw.Geometry
Imports VectorDraw.Professional.Control

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class NavigatorHub

    Private _isMarkerValid As Boolean
    Private _objectCountThreshold As Integer
    Private _rect As Rectangle
    Private _document As DocumentForm

    Private WithEvents _vdCanvas As VectorDrawBaseControl

    Public Sub New(document As DocumentForm)
        InitializeComponent()
        upbNavigator.Appearance.BackColor = Color.White
        _objectCountThreshold = If(document.MainForm?.GeneralSettings.NavigatorObjectCountThreshold, Settings.Defaults.DEFAULT_NAVIGATOR_OBJECT_COUNT_THRESHOLD)
        _document = document
    End Sub

    Public Function LoadView() As Boolean
        Return UpdatePictureBox(False)
    End Function

    Public Function ClearView() As Boolean
        Return UpdatePictureBox(True)
    End Function

    Private Sub DrawViewMarker()
        If _vdCanvas IsNot Nothing AndAlso _vdCanvas.ActiveDocument IsNot Nothing AndAlso Me.upbNavigator.Image IsNot Nothing Then
            Dim boundingBox As Box = GetBoundingBox()
            If Not boundingBox.IsEmpty Then
                Dim scale As Double = GetScaleFromAspectRatio(boundingBox)

                Dim cx As Double = (_vdCanvas.ActiveDocument.ViewCenter.x - boundingBox.MidPoint.x) * scale
                Dim cy As Double = -(_vdCanvas.ActiveDocument.ViewCenter.y - boundingBox.MidPoint.y) * scale

                Dim viewWidth As Double = (_vdCanvas.ActiveDocument.ViewSize * _vdCanvas.Width / _vdCanvas.Height) * scale
                Dim viewSize As Double = _vdCanvas.ActiveDocument.ViewSize * scale

                If (Not Double.IsInfinity(viewSize) AndAlso Not Double.IsNaN(viewSize)) AndAlso (Not Double.IsInfinity(viewWidth) AndAlso Not Double.IsNaN(viewWidth)) Then
                    _rect.X = CInt((cx + Me.upbNavigator.Width / 2) - viewWidth / 2)
                    _rect.Y = CInt((cy + Me.upbNavigator.Height / 2) - viewSize / 2)
                    _rect.Height = CInt(viewSize)
                    _rect.Width = CInt(viewWidth)

                    If (_rect.Height / Me.upbNavigator.Height < 0.01) Then
                        _rect.Height = CInt(Me.upbNavigator.Height * 0.01)
                    End If

                    If (_rect.Width / Me.upbNavigator.Width < 0.01) Then
                        _rect.Width = CInt(Me.upbNavigator.Width * 0.01)
                    End If

                    Using graphics As Graphics = Me.upbNavigator.CreateGraphics
                        Dim image As System.Drawing.Image = System.Drawing.UtilsEx.CloneImage(Me.upbNavigator.Image)
                        Using g As Graphics = Graphics.FromImage(image)
                            DrawViewMarker(g)
                            _isMarkerValid = True
                        End Using
                    End Using
                End If

                Me.upbNavigator.Invalidate()
            End If
        End If
    End Sub

    Private Sub DrawViewMarker(ByVal graphics As Graphics)
        Const THICKNESS As Double = 0.05 '5%
        Const START_PERCENTAGE As Single = 0.25 '25%
        Const END_PERCENTAGE As Single = 0.75 '75%

        Dim borderPen As New Pen(Color.Magenta, 2)
        graphics.FillRectangle(New SolidBrush(Color.FromArgb(50, Color.Magenta)), _rect)
        graphics.DrawRectangle(borderPen, _rect)

        If (_rect.Right <= 0) Then
            Dim points(2) As Drawing.Point
            points(0) = New Drawing.Point(0, CInt(upbNavigator.Height / 2))
            points(1) = New Drawing.Point(CInt(upbNavigator.Width * THICKNESS), CInt(upbNavigator.Height * START_PERCENTAGE))
            points(2) = New System.Drawing.Point(CInt(upbNavigator.Width * THICKNESS), CInt(upbNavigator.Height * END_PERCENTAGE))

            graphics.FillPolygon(New SolidBrush(Color.FromArgb(50, Color.Magenta)), points)
            graphics.DrawPolygon(borderPen, points)
        End If

        If (_rect.Left >= Me.upbNavigator.Width) Then
            Dim points(2) As Drawing.Point
            points(0) = New Drawing.Point(upbNavigator.Width, CInt(upbNavigator.Height / 2))
            points(1) = New Drawing.Point(CInt(upbNavigator.Width * (1 - THICKNESS)), CInt(upbNavigator.Height * START_PERCENTAGE))
            points(2) = New Drawing.Point(CInt(upbNavigator.Width * (1 - THICKNESS)), CInt(upbNavigator.Height * END_PERCENTAGE))

            graphics.FillPolygon(New SolidBrush(Color.FromArgb(50, Color.Magenta)), points)
            graphics.DrawPolygon(borderPen, points)
        End If

        If (_rect.Top >= Me.upbNavigator.Height) Then
            Dim points(2) As Drawing.Point
            points(0) = New Drawing.Point(CInt(upbNavigator.Width / 2), upbNavigator.Height)
            points(1) = New Drawing.Point(CInt(upbNavigator.Width * START_PERCENTAGE), CInt(upbNavigator.Height * (1 - THICKNESS)))
            points(2) = New Drawing.Point(CInt(upbNavigator.Width * END_PERCENTAGE), CInt(upbNavigator.Height * (1 - THICKNESS)))

            graphics.FillPolygon(New SolidBrush(Color.FromArgb(50, Color.Magenta)), points)
            graphics.DrawPolygon(borderPen, points)
        End If

        If (_rect.Bottom <= 0) Then
            Dim points(2) As Drawing.Point
            points(0) = New Drawing.Point(CInt(upbNavigator.Width / 2), 0)
            points(1) = New Drawing.Point(CInt(upbNavigator.Width * START_PERCENTAGE), CInt(upbNavigator.Height * THICKNESS))
            points(2) = New Drawing.Point(CInt(upbNavigator.Width * END_PERCENTAGE), CInt(upbNavigator.Height * THICKNESS))

            graphics.FillPolygon(New SolidBrush(Color.FromArgb(50, Color.Magenta)), points)
            graphics.DrawPolygon(borderPen, points)
        End If
    End Sub

    Private Function GetBoundingBox() As Box
        Const BOUNDINGBOX_ENLARGEMENT As Single = 0.01 '1%

        Dim boundingBox As New Box(_vdCanvas.ActiveDocument.ActiveLayOut.Entities.GetBoundingBox(False, False))
        boundingBox.AddWidth(boundingBox.Width * BOUNDINGBOX_ENLARGEMENT)

        Return boundingBox
    End Function

    Private Function GetScaleFromAspectRatio(ByVal boundingBox As Box) As Double
        Return Math.Min(Me.upbNavigator.Width / boundingBox.Width, Me.upbNavigator.Height / boundingBox.Height)
    End Function

    Public Function SetEmptyImage() As Image
        Dim image As Image = Nothing

        Dim bx As Double = upbNavigator.Width / upbNavigator.Height
        Dim sc As Double = E3.Lib.DotNet.Expansions.Devices.My.Computer.Screen.Bounds.Width / E3.Lib.DotNet.Expansions.Devices.My.Computer.Screen.Bounds.Height

        If (bx >= sc) Then
            image = New Bitmap(E3.Lib.DotNet.Expansions.Devices.My.Computer.Screen.Bounds.Width, CInt(E3.Lib.DotNet.Expansions.Devices.My.Computer.Screen.Bounds.Width / bx))
        ElseIf (upbNavigator.Height > 0) AndAlso (upbNavigator.Width > 0) Then
            image = New Bitmap(CInt(E3.Lib.DotNet.Expansions.Devices.My.Computer.Screen.Bounds.Height * bx), E3.Lib.DotNet.Expansions.Devices.My.Computer.Screen.Bounds.Height)
        End If

        Dim graphics As Graphics = Graphics.FromImage(image)
        graphics.Clear(Color.White)

        With Me.upbNavigator
            .BackColor = Color.White
            .Image = image
            .ScaleImage = Infragistics.Win.ScaleImage.Always
            .Invalidate()
            .Refresh()
        End With

        Me.BackColor = Color.White

        _isMarkerValid = False
        Return image
    End Function

    Private Function UpdatePictureBox(doClear As Boolean) As Boolean
        If _vdCanvas IsNot Nothing AndAlso _vdCanvas.ActiveDocument IsNot Nothing AndAlso Not _vdCanvas.ActiveDocument.DisableRedraw AndAlso Me.ParentForm IsNot Nothing Then
            Dim drawingCanvas As DrawingCanvas = _vdCanvas.FindParent(Of DrawingCanvas)
            If (drawingCanvas?.DrawingDisplayed).GetValueOrDefault Then
                Using Me.ParentForm.EnableWaitCursor
                    Dim img As Image = drawingCanvas.NavigatorImage

                    If Me.upbNavigator.Height > 0 AndAlso Me.upbNavigator.Width > 0 Then
                        If img Is Nothing Then
                            If (doClear OrElse VDCanvas.ActiveDocument?.ActiveLayOut?.Render?.GraphicsContext?.MemoryBitmap Is Nothing) Then
                                Dim boundingBox As Box = GetBoundingBox()
                                Dim bx As Double = boundingBox.Width / boundingBox.Height
                                Dim sc As Double = E3.Lib.DotNet.Expansions.Devices.My.Computer.Screen.Bounds.Width / E3.Lib.DotNet.Expansions.Devices.My.Computer.Screen.Bounds.Height
                                If bx >= sc Then
                                    img = New Bitmap(E3.Lib.DotNet.Expansions.Devices.My.Computer.Screen.Bounds.Width, CInt(E3.Lib.DotNet.Expansions.Devices.My.Computer.Screen.Bounds.Width / bx))
                                ElseIf (boundingBox.Height > 0) AndAlso (boundingBox.Width > 0) Then
                                    img = New Bitmap(CInt(E3.Lib.DotNet.Expansions.Devices.My.Computer.Screen.Bounds.Height * bx), E3.Lib.DotNet.Expansions.Devices.My.Computer.Screen.Bounds.Height)
                                End If

                                If img Is Nothing Then
                                    Return False
                                End If

                                Using g As Graphics = Graphics.FromImage(img)
                                    g.SmoothingMode = Drawing2D.SmoothingMode.HighSpeed
                                    g.TextRenderingHint = Drawing.Text.TextRenderingHint.SingleBitPerPixel
                                    g.InterpolationMode = Drawing.Drawing2D.InterpolationMode.Low
                                    g.Clear(_vdCanvas.ActiveDocument.Palette.Background)
                                End Using

                                _isMarkerValid = False
                            Else
                                img = CType(_vdCanvas.ActiveDocument.ActiveLayOut.Render.GraphicsContext.MemoryBitmap.Clone, Bitmap)
                                drawingCanvas.NavigatorImage = img
                            End If
                        End If

                        With Me.upbNavigator
                            .BackColor = Color.White
                            .Image = img
                            .ScaleImage = Infragistics.Win.ScaleImage.Always
                            .Invalidate()
                        End With

                        If Not doClear Then
                            DrawViewMarker()
                        End If
                    End If
                    Return True
                End Using
            End If
        End If
        Return False
    End Function

    Private Sub upbNavigator_MouseClickMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles upbNavigator.MouseClick, upbNavigator.MouseMove
        If _vdCanvas IsNot Nothing AndAlso _vdCanvas.ActiveDocument IsNot Nothing Then
            If e.Button = System.Windows.Forms.MouseButtons.Left Then
                Dim boundingBox As Box = GetBoundingBox()

                If Not boundingBox.IsEmpty Then
                    Dim scale As Double = GetScaleFromAspectRatio(boundingBox)
                    _vdCanvas.ActiveDocument.ViewCenter.x = (e.X - Me.upbNavigator.Width / 2) / scale + boundingBox.MidPoint.x
                    _vdCanvas.ActiveDocument.ViewCenter.y = -(e.Y - Me.upbNavigator.Height / 2) / scale + boundingBox.MidPoint.y
                    _vdCanvas.ActiveDocument.Invalidate()
                End If
            End If
        End If
    End Sub

    Private Sub upbNavigator_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles upbNavigator.Paint
        If _isMarkerValid Then
            DrawViewMarker(e.Graphics)
        End If
    End Sub

    Private Sub upbNavigator_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles upbNavigator.Resize
        If _isMarkerValid Then
            DrawViewMarker()
        End If
    End Sub

    Private Sub _vdCanvas_DrawOverAll(ByVal sender As Object, ByVal render As VectorDraw.Render.vdRender, ByRef cancel As Boolean) Handles _vdCanvas.DrawOverAll
        If _isMarkerValid Then
            DrawViewMarker()
        End If
    End Sub

    Public Property VDCanvas() As VectorDrawBaseControl
        Set(ByVal value As VectorDrawBaseControl)
            _vdCanvas = value
        End Set
        Get
            Return _vdCanvas
        End Get
    End Property

End Class
