Partial Public Class CalculateWeightEngine
    Implements IDisposable

    Private _disposedValue As Boolean

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not _disposedValue Then
            If disposing Then

            End If

            _kblMapper = Nothing
            _wires = Nothing
            _coresOfCables = Nothing
            _syncCtx = Nothing
            _disposedValue = True
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub

End Class
