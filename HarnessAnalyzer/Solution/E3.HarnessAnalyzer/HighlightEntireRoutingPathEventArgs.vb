Public Class HighlightEntireRoutingPathEventArgs
    Inherits EventArgs

    Public CoreWireIds As List(Of String)
    Public IsCable As Boolean
    Public SelectRowsInGrids As Boolean

    Public Sub New()

    End Sub

    Public Sub New(corWirIds As List(Of String), isCab As Boolean, selRowsInGrids As Boolean)
        CoreWireIds = corWirIds
        IsCable = isCab
        SelectRowsInGrids = selRowsInGrids
    End Sub

End Class
