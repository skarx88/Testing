Namespace D3D.Consolidated.Controls

    Public Class ToolBarButtonEventArgs
        Inherits EventArgs

        Private _button As devDept.Eyeshot.ToolBarButton

        Public Sub New(button As devDept.Eyeshot.ToolBarButton)
            MyBase.New()
            _button = button
        End Sub

        ReadOnly Property Button As devDept.Eyeshot.ToolBarButton
            Get
                Return _button
            End Get
        End Property

    End Class

End Namespace
