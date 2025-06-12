Imports Infragistics.Win.UltraWinGrid

Partial Public Class InformationHub

    Protected Friend Class GridKeyDownEventArgs

        Public Sub New(args As KeyEventArgs, grid As UltraGrid)
            Me.Grid = grid
            Me.KeyEventArgs = args
        End Sub

        ReadOnly Property KeyEventArgs As KeyEventArgs
        ReadOnly Property Grid As UltraGrid

    End Class

End Class
