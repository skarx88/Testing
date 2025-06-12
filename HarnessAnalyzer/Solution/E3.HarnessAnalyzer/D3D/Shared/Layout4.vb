Imports devDept.Eyeshot
Imports Zuken.E3.HarnessAnalyzer.D3D

Namespace D3D.Shared

    Public Class Layout4

        Public Sub New(model As devDept.Eyeshot.Design)
            With model
                Me.Main = .Viewports.OfType(Of Viewport).FirstOrDefault

                If .Viewports.Count > 1 Then
                    Me.Top = .Viewports(1)
                End If

                If .Viewports.Count > 2 Then
                    Me.Right = .Viewports(2)
                End If

                If .Viewports.Count > 3 Then
                    Me.Front = .Viewports(3)
                End If
            End With
        End Sub

        Public Sub New(main As Viewport, top As Viewport, right As Viewport, front As Viewport)
            Me.Main = main
            Me.Top = top
            Me.Right = right
            Me.Front = front
        End Sub

        ReadOnly Property Main As Viewport
        ReadOnly Property Top As Viewport
        ReadOnly Property Right As Viewport
        ReadOnly Property Front As Viewport

        Public Sub InitModel(model As devDept.Eyeshot.Design)
            TryAddVp(Me.Main, model)
            TryAddVp(Me.Top, model)
            TryAddVp(Me.Right, model)
            TryAddVp(Me.Front, model)
            model.LayoutMode = viewportLayoutType.FourViewports
        End Sub

        Public Sub InvalidateAll()
            TryInvalidate(Me.Main)
            TryInvalidate(Me.Front)
            TryInvalidate(Me.Top)
            TryInvalidate(Me.Right)
        End Sub

        Public Sub ZoomfitAll(Optional margin As Nullable(Of Integer) = Nothing)
            TryZooomFit(Main, margin)
            TryZooomFit(Front, margin)
            TryZooomFit(Top, margin)
            TryZooomFit(Right, margin)
        End Sub

        Private Sub TryZooomFit(vp As Viewport, margin As Nullable(Of Integer))
            If vp IsNot Nothing Then
                If margin.HasValue Then
                    vp.ZoomFit(margin.Value)
                Else
                    vp.ZoomFit()
                End If
            End If
        End Sub

        Public Function GetAllStartUpTransformations() As List(Of devDept.Geometry.Transformation)
            Dim list As New List(Of devDept.Geometry.Transformation)

            If Me.Main IsNot Nothing Then
                list.Add(Me.Main.GetStartupTransformation)
            End If

            If Me.Top IsNot Nothing Then
                list.Add(Me.Top.GetStartupTransformation)
            End If

            If Me.Right IsNot Nothing Then
                list.Add(Me.Right.GetStartupTransformation)
            End If

            If Me.Front IsNot Nothing Then
                list.Add(Me.Front.GetStartupTransformation)
            End If

            Return list
        End Function

        Public Sub HideAllGrids()
            TryHideGrid(Me.Main)
            TryHideGrid(Me.Top)
            TryHideGrid(Me.Right)
            TryHideGrid(Me.Front)
        End Sub

        Private Sub TryHideGrid(vp As Viewport)
            If vp IsNot Nothing Then
                vp.Grid.Visible = False
            End If
        End Sub

        Private Sub TryAddVp(vp As Viewport, model As devDept.Eyeshot.Design)
            If vp IsNot Nothing Then
                model.Viewports.Add(vp)
            End If
        End Sub

        Private Sub TryInvalidate(vp As Viewport)
            If vp IsNot Nothing Then
                vp.Invalidate()
            End If
        End Sub

    End Class

End Namespace