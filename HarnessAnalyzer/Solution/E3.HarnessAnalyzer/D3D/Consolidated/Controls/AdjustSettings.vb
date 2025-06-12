Imports devDept.Eyeshot
Imports Zuken.E3.HarnessAnalyzer.D3D.Shared

Namespace D3D.Consolidated.Controls

    Public Class AdjustCarSettings
        Implements IDisposable

        Private _disposedValue As Boolean

        Public Sub New(objectManipulator As ObjectManipulatorKeyAndScaleManager, clippingPlanes As ClippingPlaneSet, layout As Layout4)
            Me.ObjectManipulator = objectManipulator
            Me.ClippingPlanes = clippingPlanes
            Me.Layout4 = layout
        End Sub

        ReadOnly Property ObjectManipulator As devDept.Eyeshot.ObjectManipulatorKeyAndScaleManager
        ReadOnly Property ClippingPlanes As [Shared].ClippingPlaneSet
        ReadOnly Property Layout4 As Layout4

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not _disposedValue Then
                If disposing Then
                End If

                _ObjectManipulator = Nothing
                _ClippingPlanes = Nothing
                _Layout4 = Nothing
                _disposedValue = True
            End If
        End Sub


        Public Sub Dispose() Implements IDisposable.Dispose
            ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub
    End Class

End Namespace