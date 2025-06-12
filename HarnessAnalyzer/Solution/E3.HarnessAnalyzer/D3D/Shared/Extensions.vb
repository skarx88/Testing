Imports System.Runtime.CompilerServices
Imports devDept.Eyeshot
Imports devDept.Geometry

Namespace D3D.Shared

    <HideModuleName>
    Friend Module Extensions

        Public Const DEFAULT_ACTION_MODE As actionType = actionType.SelectVisibleByPickDynamic

        <Extension>
        Public Sub InitCubeIcon(vp As Viewport)
            With vp.ViewCubeIcon
                .TopText = Consolidated3DControlStrings.CubeTopText
                .BottomText = Consolidated3DControlStrings.CubeBottomText
                .BackText = Consolidated3DControlStrings.CubeRightText
                .FrontText = Consolidated3DControlStrings.CubeLeftText
                .LeftText = Consolidated3DControlStrings.CubeFrontText
                .RightText = Consolidated3DControlStrings.CubeBackText
            End With
        End Sub

        <Extension>
        Friend Sub InitLight(model As devDept.Eyeshot.Design)
            With model
                .AmbientLight = Color.WhiteSmoke

                With .Light1
                    .Type = devDept.Graphics.lightType.Directional
                    .Color = Color.White
                    .ConstantAttenuation = 0.9
                    .LinearAttenuation = 0.2
                    .QuadraticAttenuation = 0.01

                    .Position = New Point3D(-100, -2000, -10)
                    .Direction = New Vector3D(100, 200, 0)

                    '.Specular = Color.WhiteSmoke
                    .SpotHalfAngle = 0.5 * Math.PI / 2
                    .YieldShadow = True
                End With

                With .Light2
                    .Type = devDept.Graphics.lightType.Point
                    .Color = Color.White
                    .ConstantAttenuation = 0.9
                    .LinearAttenuation = 0.2
                    .QuadraticAttenuation = 0.5

                    .Position = New Point3D(-200, -2000, 200)
                    .Direction = New Vector3D(50, 100, 0)

                    '.Specular = Color.Bisque
                    .SpotHalfAngle = 0.2 * Math.PI / 2
                    .YieldShadow = True
                End With

                With .Light3
                    .Type = devDept.Graphics.lightType.Point
                    .Color = Color.White
                    .ConstantAttenuation = 0.0
                    .LinearAttenuation = 0
                    .QuadraticAttenuation = 0.0

                    .Position = New Point3D(0, -100, 100)
                    .Direction = New Vector3D(0, 100, 0)

                    .Specular = Color.White
                    .SpotHalfAngle = Math.PI / 2
                    .YieldShadow = False
                End With

                With .Light8
                    .Type = devDept.Graphics.lightType.Directional
                    .Color = Color.White
                    .ConstantAttenuation = 0.09
                    .LinearAttenuation = 0.02
                    .QuadraticAttenuation = 0.01
                    .SpotHalfAngle = Math.PI / 2
                    .YieldShadow = True
                End With

                .Light1.Active = True
                .Light2.Active = True
                .Light3.Active = False
                .Light4.Active = False
                .Light5.Active = False
                .Light6.Active = False
                .Light7.Active = False
                .Light8.Active = False
            End With
        End Sub

        <Extension>
        Public Function InitDefaults(design As Design, Optional ByRef initMouseAndTouch As Boolean = True, Optional initObjManipulatorManager As Boolean = True, Optional actionMode As Nullable(Of actionType) = Nothing) As InitDefaultsResult
            If design IsNot Nothing Then
                With design
                    If Not actionMode.HasValue Then
                        .ActionMode = DEFAULT_ACTION_MODE
                    Else
                        .ActionMode = actionMode.Value
                    End If

                    .ActiveViewport.SmallSizeRatioMoving = 0 ' Avoids "dynamic rendering" while zoom 

                    If TypeOf design Is DesignEx Then
                        CType(design, DesignEx).PickBoxEnabled = False
                    End If

                    .Rendered.ShowEdges = False
                    .Turbo.MaxComplexity = 30000  ' enables FastZPR when the scene exceeds 30000 objects 
                    .Viewports(0).ToolBar.Position = devDept.Eyeshot.ToolBar.positionType.VerticalMiddleLeft

                    If TypeOf design Is DesignEx Then
                        CType(design, DesignEx).AutoDisableMode = DisableSelectionMode.DisableSelectionBusy ' HINT: for safty reasons to avoid unknown sporadic problems (f.e. DrawEntity,KeyNotFoundExceptions,etc -> currently not clear why this is happening)
                    End If

                    .MultipleSelection = True
                    .Selection.Color = System.Drawing.Color.Magenta
                    .Selection.ColorDynamic = System.Drawing.Color.Magenta
                    .Renderer = rendererType.Direct3D

                    .InitLight

                    For Each vp As Viewport In .Viewports
                        vp.InitCubeIcon
                        If vp.OriginSymbol IsNot Nothing Then
                            vp.OriginSymbol.Visible = False
                        End If
                    Next

                    Dim mouseAndTouchManager As MouseAndTouchManager = Nothing
                    If initMouseAndTouch Then
                        mouseAndTouchManager = .InitMouseAndTouch
                    End If

                    Dim objManipulatatorManager As ObjectManipulatorKeyAndScaleManager = Nothing
                    If initObjManipulatorManager Then
                        objManipulatatorManager = New ObjectManipulatorKeyAndScaleManager(design)
                        objManipulatatorManager.ScaleSettings.Normal = ObjectManipulatorKeyAndScaleManager.VpScaleSettings.Default.Normal
                        objManipulatatorManager.ScaleSettings.Fine = ObjectManipulatorKeyAndScaleManager.VpScaleSettings.Default.Fine
                        objManipulatatorManager.KeySettings.ApplyKey = Keys.None
                        objManipulatatorManager.KeySettings.CancelKey = Keys.None
                        objManipulatatorManager.ShowTransformationLabel = False
                        D3D.Shared.RotationSettings.Default.Set(objManipulatatorManager)
                        D3D.Shared.PanSettings.Default.Set(objManipulatatorManager)
                    End If

                    Return New InitDefaultsResult(System.ComponentModel.ResultState.Success, mouseAndTouchManager, objManipulatatorManager)
                End With
            End If
            Return New InitDefaultsResult(System.ComponentModel.ResultState.Faulted, Nothing, Nothing)
        End Function

        <Extension>
        Public Function InitMouseAndTouch(model As devDept.Eyeshot.Design) As MouseAndTouchManager
            Dim manager As New MouseAndTouchManager(model)
            manager.SetRotateMouseButton(MouseButtons.Left, modifierKeys.None)
            manager.SetPanMouseButton(MouseButtons.Middle, modifierKeys.None)
            Return manager
        End Function

    End Module

End Namespace
