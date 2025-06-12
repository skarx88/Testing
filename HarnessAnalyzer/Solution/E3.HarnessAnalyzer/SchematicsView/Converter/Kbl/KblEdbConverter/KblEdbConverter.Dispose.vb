Namespace Schematics.Converter.Kbl

    Partial Public Class KblEdbConverter

        Private disposedValue As Boolean

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    _combinedMappers.Dispose()
                    _edbIdsResolved.Clear()
                    _getEdbIdInternalGetter.Dispose()
                    _addNewOrGetObjectGetter.Dispose()
                    _edbConnectorInfoMapper.Dispose()
                    If _cavityPartsToOccurrencesMapper IsNot Nothing Then
                        _cavityPartsToOccurrencesMapper.Dispose()
                    End If
                End If

                _edbModel = Nothing
                _currentConvertedItems = Nothing
                _getEdbIdInternalGetter = Nothing
                _addNewOrGetObjectGetter = Nothing
                _combinedMappers = Nothing
                _inlinerCavityMap = Nothing
                _edbIdsResolved = Nothing
                _componentTypesResolved = Nothing
                _edbConnectorInfoMapper = Nothing
                _cavityPartsToOccurrencesMapper = Nothing
            End If

            Me.disposedValue = True
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' Ändern Sie diesen Code nicht. Fügen Sie oben in Dispose(disposing As Boolean) Bereinigungscode ein.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

    End Class

End Namespace