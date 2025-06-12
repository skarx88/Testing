Public Class CanvasSelectionChangedEventArgs
    Inherits EventArgs

    Public Sub New(selection As CanvasSelection)
        Me.Selection = selection
    End Sub

    ReadOnly Property Selection As CanvasSelection

End Class
