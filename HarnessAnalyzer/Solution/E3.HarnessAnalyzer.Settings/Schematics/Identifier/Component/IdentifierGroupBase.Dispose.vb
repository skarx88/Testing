Namespace Schematics.Identifier.Component

    Partial Public Class IdentifierGroupBase

        Private _disposedValue As Boolean ' So ermitteln Sie überflüssige Aufrufe

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me._disposedValue Then
                If disposing Then
                    _connectorSuffixes?.Dispose()
                End If
                _connectorSuffixes = Nothing
            End If
            Me._disposedValue = True
        End Sub

        ' Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Ändern Sie diesen Code nicht. Fügen Sie oben in Dispose(disposing As Boolean) Bereinigungscode ein.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

    End Class

End Namespace