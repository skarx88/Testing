Imports devDept.Eyeshot

Namespace D3D.Consolidated.Designs
    ' HINT: Small layout object (shrunk down to the smallest) to get the inaccessible-events, accessible for the public

    <Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
    Public Class ConsolidatedDesign
        Inherits D3D.Document.DocumentDesign

        Public Event BeforeDrawViewPort(sender As Object, e As DrawViewPortEventArgs)
        Public Event PreviewMouseUp(sender As Object, e As PreviewMouseUpEventArgs)
        Public Event AfterMouseUp(sender As Object, e As MouseEventArgs)

        Private _mouseGapState As MouseSizeMoveViewportState = MouseSizeMoveViewportState.None
        Private _isViewportMoving As Boolean = False
        Private _inBetweenMovingViewports As New Tuple(Of ViewportInfo, ViewportInfo)(Nothing, Nothing)
        Private _viewportMovingStartPos As System.Drawing.Point

        Private _vpMoving_CurrUp As Viewport
        Private _vpMoving_CurrDown As Viewport
        Private _vpMoving_CurrLeft As Viewport
        Private _vpMoving_CurrRight As Viewport
        Private _drawingOverlay As Boolean = False

        <DebuggerStepThrough>
        Protected Overrides Sub DrawOverlay(data As DrawSceneParams)
            _drawingOverlay = True
            MyBase.DrawOverlay(data)
            _drawingOverlay = False
        End Sub

        <DebuggerStepThrough>
        Protected Overrides Sub DrawViewport(myParams As DrawSceneParams)
            RaiseEvent BeforeDrawViewPort(Me, New DrawViewPortEventArgs(myParams))
            MyBase.DrawViewport(myParams)
        End Sub

        Friend Shadows Function SortEntitiesForTransparency(viewport As devDept.Eyeshot.Viewport, ents As System.Collections.Generic.IList(Of devDept.Eyeshot.Entities.Entity)) As System.Collections.Generic.IList(Of devDept.Eyeshot.Entities.Entity)
            Return MyBase.SortEntitiesForTransparency(viewport, ents)
        End Function

        Friend Shadows Sub DrawText(x As Integer, y As Integer, text As String, textFont As System.Drawing.Font, textColor As System.Drawing.Color, textAlign As System.Drawing.ContentAlignment)
            MyBase.DrawText(x, y, text, textFont, textColor, textAlign)
        End Sub

        Friend Shadows Sub DrawText(x As Integer, y As Integer, text As String, textFont As System.Drawing.Font, textColor As System.Drawing.Color, fillColor As System.Drawing.Color, textAlign As System.Drawing.ContentAlignment, rotateFlip As System.Drawing.RotateFlipType)
            MyBase.DrawText(x, y, text, textFont, textColor, fillColor, textAlign, rotateFlip)
        End Sub

        Friend Shadows Sub DrawText(x As Integer, y As Integer, text As String, textFont As System.Drawing.Font, textColor As System.Drawing.Color, fillColor As System.Drawing.Color, textAlign As System.Drawing.ContentAlignment)
            MyBase.DrawText(x, y, text, textFont, textColor, fillColor, textAlign)
        End Sub

        Private Function TryGetToolBarButtonOfLocation(pt As Point) As ToolBarButton
            If ActiveViewport.ToolBar.Contains(pt) Then
                For Each toolButton As ToolBarButton In ActiveViewport.ToolBar.Buttons
                    If toolButton.Contains(pt) Then
                        Return toolButton
                    End If
                Next
            End If
            Return Nothing
        End Function

        '        Protected Overrides Sub OnMouseUp(e As MouseEventArgs)
        '            If e.Button.HasFlag(System.Windows.Forms.MouseButtons.Left) Then
        '                EndViewportMoving()
        '            End If

        '            Dim args As New PreviewMouseUpEventArgs(e.Button, e.Clicks, e.X, e.Y, e.Delta)
        '            RaiseEvent PreviewMouseUp(Me, args)
        '            If Not args.Handled Then
        '                Try
        '                    MyBase.OnMouseUp(e)
        '                Catch ex As EyeshotException 'HINT: this is for safety reason, because of the "Internal MouseUp Workaround-Crash" (See D3DControl.KeysAndMouse.vb - tries to avoid this exception!). Under some circumstances it seems that this workaround does not always solve this problem.
        '#If DEBUG Then
        '                    Throw ex
        '#End If
        '                End Try
        '            End If
        '            RaiseEvent AfterMouseUp(Me, args)
        '        End Sub

        'Protected Overrides Sub OnMouseMove(e As MouseEventArgs)
        '    'ProcessResizeOnMove(e)

        '    'If _mouseGapState = MouseSizeMoveViewportState.None Then
        '    '    MyBase.OnMouseMove(e)
        '    'End If
        'End Sub

        'Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
        '    'If _mouseGapState = MouseSizeMoveViewportState.None Then
        '    '    MyBase.OnMouseDown(e)
        '    'ElseIf e.Button.HasFlag(System.Windows.Forms.MouseButtons.Left) Then
        '    '    If Not _isViewportMoving Then
        '    '        If ViewportSizingEnabled Then
        '    '            If e.Button = System.Windows.Forms.MouseButtons.Left Then
        '    '                StartViewportMoving()
        '    '            End If
        '    '        End If
        '    '    End If

        '    '    _viewportMovingStartPos = New Point(e.Location.X, e.Location.Y)
        '    'End If
        'End Sub

        ReadOnly Property DrawingOverlay As Boolean
            Get
                Return _drawingOverlay
            End Get
        End Property


        Public Class DrawViewPortEventArgs
            Inherits EventArgs

            Private _params As DrawSceneParams

            <DebuggerStepThrough>
            Public Sub New(params As DrawSceneParams)
                MyBase.New()
                _params = params
            End Sub

            ReadOnly Property Params As DrawSceneParams
                <DebuggerStepThrough>
                Get
                    Return _params
                End Get
            End Property

        End Class

    End Class

    Public Class PreviewMouseUpEventArgs
        Inherits MouseEventArgs

        Public Sub New(mousebutton As MouseButtons, clicks As Integer, x As Integer, y As Integer, delta As Integer)
            MyBase.New(mousebutton, clicks, x, y, delta)
        End Sub

        Property Handled As Boolean

    End Class

End Namespace
