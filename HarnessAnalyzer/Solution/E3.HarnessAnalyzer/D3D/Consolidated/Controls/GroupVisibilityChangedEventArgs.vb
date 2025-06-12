Namespace D3D.Consolidated.Controls

    Public Class GroupVisibilityChangedEventArgs
        Inherits EventArgs

        Private _group As Consolidated.Designs.Group3D

        Public Sub New(group As Consolidated.Designs.Group3D)
            _group = group
        End Sub

        ReadOnly Property Group As Consolidated.Designs.Group3D
            Get
                Return _group
            End Get
        End Property

    End Class

End Namespace