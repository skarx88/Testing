Imports devDept.Eyeshot
Imports devDept.Eyeshot.Entities
Imports Infragistics.Win.UltraWinToolTip
Imports Zuken.E3.HarnessAnalyzer.D3D.Consolidated.Designs
Imports Zuken.E3.Lib.Eyeshot.Model

Namespace D3D.Consolidated.Controls

    Partial Public Class Consolidated3DControl

#Region "Mouse"

        Private _startDownindex As Integer = 0
        Private _mouseDown As Boolean = False
        Private _lastFocusForTransparency As Boolean = False
        Private _toolBarOnMouseDown As Boolean

        Private Sub ViewportLayout1_MouseDown(sender As Object, e As MouseEventArgs) Handles Design3D.MouseDown
            _toolBarOnMouseDown = Design3D.ActiveViewport.ToolBar.Contains(e.Location)

            If AdjustMode AndAlso e.Button = System.Windows.Forms.MouseButtons.Right Then
                _objManipulatorManager.Cancel(True)
            End If

            If e.Button = System.Windows.Forms.MouseButtons.Left AndAlso Not E3.Lib.DotNet.Expansions.Devices.My.Computer.Keyboard.ShiftKeyDown AndAlso Not Design3D.ActiveViewport.ViewCubeIcon.Contains(e.Location) AndAlso Not Design3D.ActiveViewport.ToolBar.Contains(e.Location) Then
                If Not _mouseDown Then
                    _mouseDown = True
                    Dim item As Entity = TryCast(_lastEntity, Entity)
                    Dim entityHit As Boolean = False
                    If item IsNot Nothing Then
                        _startDownindex = Design3D.Entities.IndexOf(item)
                        entityHit = item.Selectable OrElse Not item.Visible
                    End If
                End If
            End If

        End Sub

        Private Function TryGetToolBarButtonAtLocation(pt As System.Drawing.Point) As ToolBarButton
            If Me.Design3D.ActiveViewport.ToolBar.Contains(pt) Then
                For Each tbb As ToolBarButton In Me.Design3D.ActiveViewport.ToolBar.Buttons
                    If tbb.Contains(pt) Then
                        Return tbb
                    End If
                Next
            End If
            Return Nothing
        End Function

        Private Sub Design3D_MouseClick(sender As Object, e As MouseEventArgs) Handles Design3D.MouseClick
            For Each vp As Viewport In Design3D.Viewports
                If vp.Contains(e.Location) Then
                    If vp IsNot _lastActiveViewPort Then
                        _lastActiveViewPort = vp
                        OnActiveViewPortChanged(New EventArgs)
                        Exit For
                    End If
                End If
            Next
        End Sub

        Protected Overridable Sub OnActiveViewPortChanged(e As EventArgs)
            'TODO: event raise to be added
        End Sub

        Private Sub _model3D_mouseMove(sender As Object, e As MouseEventArgs) Handles Design3D.MouseMove
            If Me.Design3D.ActiveViewport IsNot Nothing Then
                Me.StatusTextLabel.Text = String.Format("X: {0}, Y:{1}", e.Location.X, e.Location.Y)
            End If
        End Sub

        Private Sub _Model3D_MouseLeaveEntity(sender As Object, e As MouseEntityEventArgs) Handles Design3D.MouseLeaveEntity
            If _lastEntity IsNot Nothing Then
                UltraToolTipManager1.HideToolTip()
                _lastEntity = Nothing
            End If
        End Sub

        Private Sub _Model3D_MouseEnterEntity(sender As Object, e As MouseEntityEventArgs) Handles Design3D.MouseEnterEntity
            With Design3D
                If _isCameraMoving OrElse _carStateMachine.IsBusy OrElse IsUpdating OrElse Design3D.DrawingOverlay OrElse Design3D.ErrorInPaint Then
                    Return ' HINT IsUpdating check is needed here while Update or UpdateViews is called, because the next steps would crash (DrawOverlay-Exception)
                End If

                If _lastFocusForTransparency AndAlso Not Focused Then
                    _lastFocusForTransparency = False
                    Focus() ' HINT (maybe no longer necessary due to change to new 3D-Model): is done to avoid problems with transparency setting -> After changing the transparency (the transparency control has the focus) and moving the mouse over some entities that can be highlighted the transparency is reseted to it's old value as long as the control has got it's focus back.
                End If

                UltraToolTipManager1.HideToolTip()
                _tip = New UltraToolTipInfo(GetDisplayName(e.Entity), Infragistics.Win.ToolTipImage.None, String.Empty, Infragistics.Win.DefaultableBoolean.True)
                UltraToolTipManager1.SetUltraToolTip(Design3D, _tip)
                UltraToolTipManager1.ShowToolTip(Design3D)
                _lastEntity = e.Entity
            End With
        End Sub

        Private Function GetDisplayName(entity As IEntity) As String
            Dim baseEnt As IBaseModelEntityEx = TryCast(entity, IBaseModelEntityEx)
            If baseEnt Is Nothing Then
                Throw New NotSupportedException($"Entity type '{entity.GetType.Name}' not supported ({NameOf(GetDisplayName)}) !")
            End If

            Select Case baseEnt.EntityType
                Case ModelEntityType.Node
                    Dim myNode As [Lib].Eyeshot.Model.NodeEntity = TryCast(baseEnt, [Lib].Eyeshot.Model.NodeEntity)
                    If myNode IsNot Nothing Then
                        Dim myTxt As String = myNode.DisplayName

                        For Each n As [Lib].Eyeshot.Model.BundleEntity In myNode.Bundles
                            myTxt += vbCrLf + vbTab + n.DisplayName
                        Next

                        For Each c As [Lib].Eyeshot.Model.GenericConnectorEntity In myNode.Connectors
                            myTxt += vbCrLf + vbTab + c.DisplayName
                        Next

                        Return myTxt
                    End If
            End Select

            Return baseEnt.DisplayName
        End Function

        Private Sub ViewportLayout1_MouseUp(sender As Object, e As MouseEventArgs) Handles Design3D.MouseUp
            If _mouseDown AndAlso e.Button = System.Windows.Forms.MouseButtons.Left Then
                _startDownindex = -1
                _mouseDown = False
            End If
        End Sub

#End Region

#Region "Key"

        Private Sub ViewportLayout1_KeyUp(sender As Object, e As KeyEventArgs) Handles Design3D.KeyUp
            With Design3D
                Select Case e.KeyCode
                    'Case Keys.Escape
                        '_objManipulatorManager.Cancel(isAdjustMode)
                    'Case Keys.Return, Keys.Enter
                        '_objManipulatorManager.Apply()
                    Case Keys.C
                        _carStateMachine.Selectable = False
                        If Not String.IsNullOrEmpty(_carStateMachine.LayerName) Then
                            Design3D.Layers(_carStateMachine.LayerName).Visible = Not Design3D.Layers(_carStateMachine.LayerName).Visible
                            If AdjustMode Then
                                _objManipulatorManager.Visible = Design3D.Layers(_carStateMachine.LayerName).Visible
                            End If
                            UpdateViews()
                        End If

                        MyBase.OnKeyUp(e)
                    Case Keys.J
                        If GetToolBarButton(ToolBarButtons.ChangeCarTransparency).Enabled Then
                            ToggleTransparencyPopUp(e.KeyCode = Keys.J)
                        End If
                    Case Keys.T
                        If GetToolBarButton(ToolBarButtons.ChangeCarTransparency).Enabled Then
                            ToggleTransparencyPopUp(e.KeyCode = Keys.T)
                        End If
                End Select
            End With
        End Sub

        Private Sub ViewportLayout1_KeyDown(sender As Object, e As KeyEventArgs) Handles Design3D.KeyDown
            With Design3D
                Select Case e.KeyData
                    Case Keys.F
                        ZoomFitAll(True)
                    Case Keys.R
                        If Design3D.ActionMode = actionType.Rotate Then
                            Design3D.ActionMode = [Shared].DEFAULT_ACTION_MODE
                        Else
                            Design3D.SetButtonActionMode(Of RotateToolBarButton)()
                        End If
                    Case Keys.P
                        If Design3D.ActionMode = actionType.Pan Then
                            Design3D.ActionMode = [Shared].DEFAULT_ACTION_MODE
                        Else
                            Design3D.SetButtonActionMode(Of PanToolBarButton)()
                        End If
                    Case Keys.S
                        If Not Design3D.ObjectManipulator.Visible Then
                            Design3D.ActionMode = actionType.SelectByPick
                        End If
                    Case Keys.M
                        If Design3D.ActionMode = actionType.MagnifyingGlass Then
                            Design3D.ActionMode = [Shared].DEFAULT_ACTION_MODE
                            Design3D.ActiveViewport.Invalidate() ' HINT: unclear why invalidated is needed but without, the magnify-glass will stay until mouse-click (user-invalidated)
                        Else
                            Design3D.SetButtonActionMode(Of MagnifyingGlassToolBarButton)()
                        End If
                    Case Keys.Z
                        If Design3D.ActionMode = actionType.Zoom Then
                            Design3D.ActionMode = [Shared].DEFAULT_ACTION_MODE
                        Else
                            Design3D.SetButtonActionMode(Of ZoomToolBarButton)()
                        End If
                    Case Keys.Escape
                        If SelectedEntities.Count > 0 Then
                            SelectedEntities.Clear()
                            UpdateViews()
                        End If
                End Select

            End With
        End Sub

#End Region

        Private Sub _transparencyCtrlCar_KeyUp(sender As Object, e As KeyEventArgs) Handles _transparencyCtrlCar.KeyUp
            If Not e.Alt AndAlso Not e.Shift AndAlso Not e.Control Then
                Select Case e.KeyCode
                    Case Keys.T
                        ToggleTransparencyPopUp(e.KeyCode = Keys.T)
                End Select
            End If
        End Sub

        Private Sub _transparencyCtrlJT_KeyUp(sender As Object, e As KeyEventArgs) Handles _transparencyCtrlJT.KeyUp
            If Not e.Alt AndAlso Not e.Shift AndAlso Not e.Control Then
                Select Case e.KeyCode
                    Case Keys.J
                        ToggleTransparencyPopUp(e.KeyCode = Keys.J)
                End Select
            End If
        End Sub

        Private Sub ViewportLayout1_PreviewMouseUp(sender As Object, e As PreviewMouseUpEventArgs) Handles Design3D.PreviewMouseUp
            'HINT: see "Internal MouseUp Workaround-Crash"
            If e.Button = System.Windows.Forms.MouseButtons.Left AndAlso Design3D.ObjectManipulator.Visible AndAlso _toolBarOnMouseDown Then
                If Not Design3D.ActiveViewport.ToolBar.Contains(e.Location) Then
                    e.Handled = True ' because of the workaround an invalidate of the button where the down was pressed is missing (looks like pressed but it's property is not set to true)
                End If
            End If
        End Sub

#Region "Internal MouseUp Workaround-Crash"
        ' This workaround is very special an maybe a bug for eyeshot. The error comes up when the object manipulator is enabled (car loaded) 
        ' and we war clicking on the toolbar button (car load button) - leave the left mouse button pressed and move the mouse
        ' outside of the toolbar. Then after that we are outside in the viewport layout with a pressed left mouse button (under the mouse
        ' are no entities!) and then we release the mouse button: the OnMouseUp-Event crashes now internally by trying to change by themselves into another actionMode (?).
        ' Further examination is not possible because the code is too obfuscated to see what happens here.
#End Region

    End Class

End Namespace