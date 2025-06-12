Namespace D3D.Consolidated.Controls

    Public Class SelectedEntitiesChangedEventArgs
        Inherits EventArgs

        Private _ids As String()
        Private _action As Specialized.NotifyCollectionChangedAction

        Public Sub New(action As Specialized.NotifyCollectionChangedAction, ParamArray ids() As String)
            _ids = ids
            _action = action
        End Sub

        ReadOnly Property Action As Specialized.NotifyCollectionChangedAction
            Get
                Return _action
            End Get
        End Property

        ReadOnly Property Ids As String()
            Get
                Return _ids
            End Get
        End Property

    End Class

End Namespace