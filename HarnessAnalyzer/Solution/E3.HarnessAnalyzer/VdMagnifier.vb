Imports System.Drawing.Drawing2D
Imports VectorDraw.Actions
Imports VectorDraw.Geometry
Imports VectorDraw.Professional.vdPrimaries
Imports VectorDraw.Render

Public Class VdMagnifier
    Inherits BaseAction

    Private _magnifierSize As Integer = 210
    Private _magnifierZoom As Integer = 3
    Private _vdLayout As vdLayout
    Private _isalldraw As Boolean
    Private _magnifierPos As Drawing.Point = Drawing.Point.Empty

    Private _zoomFac As Integer
    Public Property Scale As Double = 1.0

    Public Property MagnifierSize() As Integer
        Get
            Return _magnifierSize
        End Get
        Set(value As Integer)
            _magnifierSize = value
        End Set
    End Property

    Public Property MagnifierZoom() As Integer
        'HINT must be > 1
        Get
            Return _magnifierZoom
        End Get
        Set(value As Integer)
            _magnifierZoom = value
        End Set
    End Property


    Friend Sub New(Layout As vdLayout, MagnifierSize As Integer, Zoomfac As Integer, scaleFac As Double)
        _vdLayout = Layout
        _magnifierSize = MagnifierSize
        _magnifierZoom = MagnifierZoom
        _zoomFac = Zoomfac
        Scale = scaleFac
    End Sub

    Public Overrides Sub MouseDown(e As MouseEventArgs)
        If e.Button = MouseButtons.Right Then Me.FinishAction(Me)
    End Sub

    Public Overrides ReadOnly Property UserWaiting As Boolean
        Get
            Return True
        End Get
    End Property
    Private Function SelectClipPath(mControlGraphics As Graphics) As System.Drawing.Region
        Dim clip As System.Drawing.Region = mControlGraphics.Clip
        mControlGraphics.ResetClip()
        If Not _magnifierPos.IsEmpty Then
            Dim rect As Rectangle = New Rectangle(_magnifierPos.X - Me.MagnifierSize \ 2, _magnifierPos.Y - Me.MagnifierSize \ 2, Me.MagnifierSize, Me.MagnifierSize)
            Using graphicsPath As New GraphicsPath()
                graphicsPath.AddEllipse(rect)
                mControlGraphics.SetClip(graphicsPath, CombineMode.Replace)
            End Using
        End If
        Return clip
    End Function

    Private Sub MagnifierRestoreBitmap()
        Me._isalldraw = True
        If Me.MagnifierZoom >= 1 Then
            If (Me.Render.GdiPlusGraphics IsNot Nothing) Then
                Me.Render.GdiPlusGraphics.DrawImage(Me._vdLayout.MemoryBitmap, 0, 0)
                _magnifierPos = Drawing.Point.Empty
            End If
        End If
    End Sub

    Private Sub MagnifierDrawViewPort()
        If Not Me._isalldraw Then
            If (Me.Render.GdiPlusGraphics IsNot Nothing AndAlso MyBase.GdiMouseLocation <> Nothing) Then
                Dim pt As gPoint = Me._vdLayout.Pixel2ViewMatrix.Transform(New gPoint(CDec(MyBase.GdiMouseLocation.X), CDec(MyBase.GdiMouseLocation.Y)))
                Dim box As Box = New Box()
                box.AddPoint(pt)
                box.AddWidth(CDec(Me.MagnifierSize) / 2.0 * Me._vdLayout.PixelSize / CDec(Me.MagnifierZoom))
                Dim bitmap As Bitmap = New Bitmap(Me.MagnifierSize, Me.MagnifierSize, BitmapWrapper.DefaultPixelFormat)
                Dim graphics As Graphics = Graphics.FromImage(bitmap)
                _vdLayout.RenderToGraphics(graphics, box, Me.MagnifierSize, Me.MagnifierSize)
                Me.DrawReferenceLine()
                Dim clip As System.Drawing.Region = Me.SelectClipPath(Me.Render.GdiPlusGraphics)
                vdRender.DrawImageUnscaled(Me.Render.GdiPlusGraphics, bitmap, _magnifierPos.X - Me.MagnifierSize \ 2, _magnifierPos.Y - Me.MagnifierSize \ 2, False)
                Me.DrawMagnifierPerigram()
                Me.Render.GdiPlusGraphics.Clip = clip
                Me._isalldraw = True

                bitmap.Dispose()
                graphics.Dispose()
            End If

        End If

    End Sub

    Private Sub DrawMagnifierPerigram()
        If (Me.Render.GdiPlusGraphics IsNot Nothing) Then
            Dim rectangle As Rectangle = New Rectangle(_magnifierPos.X - Me.MagnifierSize \ 2, _magnifierPos.Y - Me.MagnifierSize \ 2, Me.MagnifierSize, Me.MagnifierSize)
            rectangle.Inflate(-1, -1)

            Dim pen As New Pen(New HatchBrush(HatchStyle.Percent90, Color.FromArgb(255, Color.Magenta)), 2.0)
            Me.Render.GdiPlusGraphics.DrawEllipse(pen, rectangle)
        End If
    End Sub

    Private Sub MagnifierDraw()
        Me._isalldraw = False
        If Me.MagnifierZoom >= 1 Then
            If (Me.Render.GdiPlusGraphics IsNot Nothing AndAlso MyBase.GdiMouseLocation <> Nothing) Then
                _magnifierPos = MyBase.GdiMouseLocation
                _magnifierPos.X += Me.MagnifierSize \ 2
                _magnifierPos.Y -= Me.MagnifierSize \ 2
                Dim destRect As Rectangle = New Rectangle(_magnifierPos.X - Me.MagnifierSize \ 2, _magnifierPos.Y - Me.MagnifierSize \ 2, Me.MagnifierSize, Me.MagnifierSize)
                Dim num As Integer = Me.MagnifierSize \ Me.MagnifierZoom
                Dim rectangle As Rectangle = New Rectangle(MyBase.GdiMouseLocation.X - num \ 2, MyBase.GdiMouseLocation.Y - num \ 2, num, num)
                Dim clip As System.Drawing.Region = Me.SelectClipPath(Me.Render.GdiPlusGraphics)
                Me.DrawReferenceLine()
                Me.Render.GdiPlusGraphics.DrawImage(Me._vdLayout.MemoryBitmap, destRect, rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height, GraphicsUnit.Pixel)
                Me.DrawMagnifierPerigram()
                Me.Render.GdiPlusGraphics.Clip = clip
            End If
        End If
    End Sub

    Public Overrides Sub KeyDown(e As KeyEventArgs)
        MyBase.KeyDown(e)
        If MyBase.IsStarted Then
            If e.KeyCode = Keys.Escape Then
                Me.CancelAction(Me)
            End If
        End If
    End Sub

    Public Overrides Sub OnIdle()
        MyBase.OnIdle()
        If Not MyBase.IsVisible OrElse Not Me.Render.IsValid Then Return
        Me.MagnifierDrawViewPort()
    End Sub

    Private Sub DrawReferenceLine()
        If Me.Render.GdiPlusGraphics IsNot Nothing AndAlso MyBase.MouseLocation <> Nothing Then
            Dim gPoint2 As gPoint = Me.Render.World2Pixelmatrix.projectTransform(MyBase.MouseLocation)
            Dim r1 As Double = _magnifierSize * Math.Sqrt(2) / 2 - _magnifierSize / 2
            Dim delta As Double = 1
            If r1 <> 0 Then
                delta = Math.Sqrt(0.5 * r1 ^ 2)
            End If

            Dim p0 As New Drawing.Point(CInt(gPoint2.x), CInt(gPoint2.y))
            Dim p1 As New Drawing.Point(CInt(gPoint2.x + delta), CInt(gPoint2.y - delta))
            Me.Render.GdiPlusGraphics.DrawLine(New Pen(Color.Magenta, 2), p0, p1)

            Dim rct As New Rectangle(CInt(gPoint2.x) - 4, CInt(gPoint2.y) - 4, 8, 8)
            Me.Render.GdiPlusGraphics.DrawEllipse(New Pen(Color.Magenta, 2), rct)
        End If
    End Sub

    Public Overrides Function Draw() As Boolean
        If Me.Render.IsValid Then
            _magnifierZoom = Math.Max(1, CInt(Math.Round(_zoomFac * _vdLayout.Document.ViewSize * Scale, 0)))
            Me.MagnifierRestoreBitmap()
            Me.MagnifierDraw()
            Return True
        Else
            Return False
        End If
    End Function

    Public Overrides Sub MouseWheel(e As MouseEventArgs)
        MyBase.MouseWheel(e)
        Draw()
    End Sub

End Class
