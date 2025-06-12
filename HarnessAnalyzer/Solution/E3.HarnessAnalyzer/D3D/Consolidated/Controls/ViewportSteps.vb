Imports devDept.Eyeshot

Namespace D3D.Consolidated.Controls

    Public Class ViewportSteps
        Inherits System.Collections.ObjectModel.Collection(Of ViewportKeyStep)

        Public Sub Replace(Of T As ViewportKeyStep)(oldStep As T, newStep As T)
            Me.Remove(oldStep)
            Me.Add(newStep)
        End Sub

        Public Sub SetViewportKeySteps(Of T As ViewportKeyStep)(model As devDept.Eyeshot.Design)
            Dim dic As Dictionary(Of Integer, T()) = Me.OfType(Of T).GroupBy(Function(ks) ks.ViewportIndex).ToDictionary(Function(grp) grp.Key, Function(grp) grp.ToArray)
            Dim i As Integer = -1
            For Each vp As Viewport In model.Viewports
                i += 1
                Dim values As T() = Nothing
                If dic.TryGetValue(i, values) Then
                    For Each ks As T In values
                        ks.Set(model)
                    Next
                End If
            Next
        End Sub

        Public Function TryGetStepOfViewport(Of T As ViewportKeyStep)(index As Integer) As T
            For Each keyStep As T In Me.OfType(Of T)
                If keyStep.ViewportIndex = index Then
                    Return keyStep
                End If
            Next
            Return Nothing
        End Function

        Public Shared Function CreateFrom(model As devDept.Eyeshot.Design) As ViewportSteps
            Dim coll As New ViewportSteps
            Dim i As Integer = -1
            For Each vp As Viewport In model.Viewports
                i += 1
                coll.Add(New ViewportRotateStep(i, vp.Rotate.KeysStep))
                coll.Add(New ViewportPanStep(i, vp.Pan.KeysStep))
            Next
            Return coll
        End Function

    End Class

End Namespace