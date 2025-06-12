Imports System.ComponentModel
Imports devDept.Eyeshot

Namespace D3D.Shared

    Friend Class InitDefaultsResult
        Inherits Result
        Implements IDisposable

        Public Sub New(result As ResultState, touchManager As MouseAndTouchManager, objManipulatorManager As ObjectManipulatorKeyAndScaleManager)
            MyBase.New(result)
            Me.MouseAndTouchManager = touchManager
            Me.ObjManipulatorManager = objManipulatorManager
        End Sub

        ReadOnly Property MouseAndTouchManager As MouseAndTouchManager
        ReadOnly Property ObjManipulatorManager As ObjectManipulatorKeyAndScaleManager

        Protected Overrides Sub Dispose(disposing As Boolean)
            _MouseAndTouchManager = Nothing
            _ObjManipulatorManager = Nothing
            MyBase.Dispose(disposing)
        End Sub

    End Class

End Namespace