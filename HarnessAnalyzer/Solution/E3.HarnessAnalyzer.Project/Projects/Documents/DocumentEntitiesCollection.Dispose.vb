Namespace Documents
    Partial Public Class DocumentEntitiesCollection

#Region "IDisposable Support"
        Private disposedValue As Boolean ' Dient zur Erkennung redundanter Aufrufe.

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    If _entitiesByEEObjectId IsNot Nothing Then
                        _entitiesByEEObjectId.Clear()
                    End If
                    Me.Clear()
                End If

                _doc = Nothing
                _entitiesByEEObjectId = Nothing
            End If
            disposedValue = True
        End Sub


        ' Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(disposing As Boolean) weiter oben ein.
            Dispose(True)
            ' TODO: Auskommentierung der folgenden Zeile aufheben, wenn Finalize() oben überschrieben wird.
            ' GC.SuppressFinalize(Me)
        End Sub


#End Region

    End Class

End Namespace