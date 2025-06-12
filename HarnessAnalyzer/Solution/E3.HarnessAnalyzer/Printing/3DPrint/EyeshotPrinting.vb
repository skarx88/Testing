Imports System.Drawing.Drawing2D
Imports devDept.Eyeshot
Imports devDept.Eyeshot.Workspace
Imports Zuken.E3.HarnessAnalyzer.D3D.Document

Public Class EyeshotPrinting

    Private WithEvents _model3d As DocumentDesign
    Private _isPrintWindowSelected As Boolean
    Private _onlyMarginsPreview As Boolean
    Private _isLandscape As Boolean
    Private _printAreaCheckedIndex As Integer
    Private _viewSelectedIndex As Integer
    Private _previousCamera As Camera
    Private _previousPickedWindowCamera As Camera 'hint: To continue the same camera when orientation is changed instead of picking up from original model(which may be different)
    Private WithEvents _outerTempModel As New Design


    Sub New(model3d As DocumentDesign, isPrintingAreaFromModel3DSelected As Boolean)
        Dim vp As New Viewport
        _previousCamera = CType(model3d.ActiveViewport.Camera.Clone, Camera)
        _isPrintWindowSelected = isPrintingAreaFromModel3DSelected

        _outerTempModel.Clear()
        _outerTempModel.CreateControl()
        _outerTempModel.Viewports.Add(vp)

        _outerTempModel.ActiveViewport.Camera = CType(model3d.ActiveViewport.Camera.Clone, Camera)

        model3d.CopyTo(_outerTempModel)
        _model3d = model3d

        RemoveHandler _model3d.ViewChanged, AddressOf model3d_ViewChanged
        AddHandler _model3d.ViewChanged, AddressOf model3d_ViewChanged

        _model3d.EnableToolTip(False)
    End Sub

    Public Property OnlyMarginsPreview As Boolean
        Get
            Return _onlyMarginsPreview
        End Get
        Set(value As Boolean)
            _onlyMarginsPreview = value
        End Set
    End Property

    Public Property IsPrintWindowSelected As Boolean
        Get
            Return _isPrintWindowSelected
        End Get
        Set(value As Boolean)
            _isPrintWindowSelected = value
        End Set
    End Property

    Public ReadOnly Property Model3d As DocumentDesign
        Get
            Return _model3d
        End Get
    End Property
    Public Property PrintAreaCheckedIndex As Integer
        Get
            Return _printAreaCheckedIndex
        End Get
        Set(value As Integer)
            _printAreaCheckedIndex = value
        End Set
    End Property
    Public Property ViewSelectedIndex As Integer
        Get
            Return _viewSelectedIndex
        End Get
        Set(value As Integer)
            _viewSelectedIndex = value
            SetView()
        End Set
    End Property

    Public Sub DisposeModelAfterFormClosed()
        DisposeModel(_outerTempModel)

        If Not _isPrintWindowSelected Then
            _model3d.EnableToolTip(True)
        End If
    End Sub

    Public Sub PrintPage(ByVal e As System.Drawing.Printing.PrintPageEventArgs)
        Dim tempModel As New Design

        _isLandscape = e.PageSettings.Landscape
        Dim printRect As New Rectangle(e.MarginBounds.Left, e.MarginBounds.Top, e.MarginBounds.Width, e.MarginBounds.Height)

        If OnlyMarginsPreview Then
            e.Graphics.DrawRectangle(Pens.Red, printRect)
        Else
            e.Graphics.Flush()
            Dim bmp As Bitmap = RenderBitMapImageForModel(printRect, tempModel)
            Dim scaledSize As Size = ScaleImageSizeToPrintRect(printRect, bmp.Size)
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic
            e.Graphics.DrawImage(bmp, CInt(printRect.Left + (printRect.Width - scaledSize.Width) / 2), CInt(printRect.Top + (printRect.Height - scaledSize.Height) / 2), scaledSize.Width, scaledSize.Height)
        End If

        tempModel.Refresh()
        DisposeModel(tempModel)

        _model3d.ActiveViewport.Camera = _previousCamera
        _model3d.Refresh()
    End Sub


    Private Function RenderBitMapImageForModel(printRect As Rectangle, tempModel As Design) As Bitmap
        Dim vp As New Viewport

        tempModel.CreateControl()
        tempModel.Viewports.Add(vp)
        _outerTempModel.CopyTo(tempModel)

        tempModel.ActiveViewport.Grid.Visible = False
        tempModel.ActiveViewport.OriginSymbol.Visible = False
        tempModel.ActiveViewport.CoordinateSystemIcon.Visible = False

        vp.Size = New Size(printRect.Width, printRect.Height)
        tempModel.MinimumSize = vp.Size
        _outerTempModel.MinimumSize = vp.Size

        If IsPrintWindowSelected Then
            If _printAreaCheckedIndex = 1 And _previousPickedWindowCamera Is Nothing Then
                _previousPickedWindowCamera = CType(_model3d.ActiveViewport.Camera.Clone, Camera)
            End If

            _outerTempModel.ActiveViewport.Zoom.PerspectiveFitMode = Camera.perspectiveFitType.Accurate
            _outerTempModel.ActiveViewport.Zoom.ZoomStyle = zoomStyleType.Centered

            If _previousPickedWindowCamera IsNot Nothing Then
                tempModel.ActiveViewport.Camera = _previousPickedWindowCamera
            Else
                tempModel.ActiveViewport.Camera = CType(_model3d.ActiveViewport.Camera.Clone, Camera)
            End If

        Else
            tempModel.ActiveViewport.Camera = CType(_outerTempModel.ActiveViewport.Camera.Clone, Camera)
            tempModel.ZoomFit()
        End If
        Return tempModel.RenderToBitmap(New System.Drawing.Size(CInt(printRect.Width) * 4, CInt(printRect.Width) * 4), False)
    End Function

    Private Sub DisposeModel(tempModel As Design)
        If tempModel IsNot Nothing Then
            tempModel.Viewports.Clear()
            tempModel.Clear()
            tempModel.Dispose()
        End If
    End Sub


    Private Function ScaleImageSizeToPrintRect(ByVal printRect As RectangleF, ByVal imageSize As Size) As Size
        Dim width As Double, height As Double
        Dim ratio As Double

        ' fit the width of the image inside the width of the print Rectangle
        ratio = printRect.Width / imageSize.Width

        width = imageSize.Width * ratio
        height = imageSize.Height * ratio

        ' fit the other dimension
        If height > printRect.Height Then
            ratio = printRect.Height / height
            width *= ratio
            height *= ratio
        End If

        Return New Size(CInt(Math.Truncate(width)), CInt(Math.Truncate(height)))
    End Function

    Private Sub SetView()

        Select Case ViewSelectedIndex
            Case 0
                'front
                _outerTempModel.SetView(viewType.Left)
            Case 1
                'rear
                _outerTempModel.SetView(viewType.Right)
            Case 2
                'Left
                _outerTempModel.SetView(viewType.Front)
            Case 3
                'Right
                _outerTempModel.SetView(viewType.Rear)
            Case 4
                _outerTempModel.SetView(viewType.Top)
            Case 5
                _outerTempModel.SetView(viewType.Bottom)
            Case 6
                _outerTempModel.SetView(viewType.Isometric)
        End Select
        _outerTempModel.KeepSceneUpright = True
    End Sub

    Public Sub InternalModelZoomFitAndRefresh()
        If _outerTempModel.Viewports.Count > 0 Then
            _outerTempModel.ZoomFit()
            _outerTempModel.Refresh()
        End If
    End Sub


    Private Sub model3d_ViewChanged(sender As Object, e As ViewChangedEventArgs) Handles _model3d.ViewChanged
        If e.ViewType = devDept.Eyeshot.viewType.Top Or e.ViewType = devDept.Eyeshot.viewType.Bottom Or
           e.ViewType = devDept.Eyeshot.viewType.Left Or e.ViewType = devDept.Eyeshot.viewType.Right Or
            e.ViewType = devDept.Eyeshot.viewType.Front Or e.ViewType = devDept.Eyeshot.viewType.Rear Or
              e.ViewType = devDept.Eyeshot.viewType.Isometric Then

            If Not IsPrintWindowSelected Then
                InternalModelZoomFitAndRefresh()
            End If

        End If
    End Sub
End Class

