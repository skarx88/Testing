Namespace Schematics.Converter.Kbl

    Partial Public Class KblDocumentDataCollection

        Private _disposedValue As Boolean ' So ermitteln Sie überflüssige Aufrufe

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me._disposedValue Then
                If disposing Then
                    CancelGeneration()
                    Dim tsk As Task(Of IDisposable) = _lock.BeginWaitAsync(Nothing)
                    Try
                        tsk.WaitWithPumping()
                        DisposeLastConversionAndModel()
                    Finally
                        tsk.Result?.Dispose()
                    End Try
                End If

                _cancelTokenSource = Nothing
                _lastConversion = Nothing
                _lock = Nothing
            End If
            Me._disposedValue = True
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

    End Class

End Namespace