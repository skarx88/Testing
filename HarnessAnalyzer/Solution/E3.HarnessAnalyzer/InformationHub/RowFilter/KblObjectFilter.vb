Friend MustInherit Class KblObjectFilter
    Implements IDisposable

    Private _disposedValue As Boolean
    Private _KblObjectTypes As KblObjectType() = Array.Empty(Of KblObjectType)

    Protected Sub New()
    End Sub

    Protected Sub New(info As RowFilterInfo)
        Me.Info = info
    End Sub

    Public Function FilterKblObject(systemId As String, Optional overrideKblObjectType As Nullable(Of KblObjectType) = Nothing) As KblObjectFilterType
        Dim kblObject_types As KblObjectType() = Me.KblObjectTypes
        If overrideKblObjectType.HasValue Then
            kblObject_types = {overrideKblObjectType.Value}
        End If

        Dim final_result As KblObjectFilterType = KblObjectFilterType.Unfiltered
        For Each kbl_type As KblObjectType In kblObject_types
            Dim result As KblObjectFilterType = FilterKblObjectCore(kbl_type, systemId)
            If result.HasFlag(KblObjectFilterType.Filtered) Then
                final_result = final_result Or result
            End If
        Next

        Return final_result
    End Function

    Public Overridable Property KblObjectTypes As KblObjectType()
        Get
            Return _KblObjectTypes
        End Get
        Protected Set
            _KblObjectTypes = Value
        End Set
    End Property

    Protected Overridable Function FilterKblObjectCore(kblObjectType As KblObjectType, systemId As String) As KblObjectFilterType
        If Info.InactiveObjects IsNot Nothing Then
            If Info.InactiveObjects.ContainsValue(kblObjectType, systemId) Then
                Return KblObjectFilterType.Filtered
            End If
        End If
        Return KblObjectFilterType.Unfiltered
    End Function

    Friend Property Info As RowFilterInfo

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not _disposedValue Then
            If disposing Then
                Info?.Dispose()
            End If

            _Info = Nothing
            _disposedValue = True
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub

End Class
