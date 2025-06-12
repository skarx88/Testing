
Namespace Documents
    Partial Public Class HcvDocument

        Private _workFileDisposeCoreEnabled As Boolean = False

        Private Sub DisposeEachEntity()
            For Each entity As IDisposable In Me.Entities.OfType(Of IDisposable)
                entity.Dispose()
            Next
        End Sub

        Protected Overrides Sub WorkFileDisposeCore(disposing As Boolean)
            If _workFileDisposeCoreEnabled Then
                MyBase.WorkFileDisposeCore(disposing)
            End If
        End Sub

        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing Then
                _workFileDisposeCoreEnabled = True
                Try
                    WorkFileDisposeCore(disposing) 'HINT: dispose core (SetView to nothing with related-events/work on view before disposed fully executed, because these methods/events may need access to non disposed data/model -> try to execute this first but not the full dispose method because we don't want to switch to disposed state until disposed has finished (Base methods will switch disposed = true at the end)
                Finally
                    _workFileDisposeCoreEnabled = False
                End Try

                If _bundleRecalculator IsNot Nothing Then
                    _bundleRecalculator.Dispose()
                End If

                If Entities IsNot Nothing Then
                    Entities.Dispose()
                End If

                ExecuteCloseCoreWork()
            End If

            _kbl = Nothing
            _hcvFile = Nothing
            _entities = Nothing
            _bundleRecalculator = Nothing

            MyBase.Dispose(disposing) ' HINT = disposedValue = true (dispose core is disabled because already forced to be executed before when disposing = true -> see Hint: WorkFileDisposeCore)
        End Sub

    End Class

End Namespace