Public Class ActiveObjectIdsCollection
    Inherits HashSet(Of String)

    ReadOnly Property IsInitialized As Boolean

    Public Sub InitializeOrReset()
        _IsInitialized = True
        MyBase.Clear()
    End Sub

    ''' <summary>
    ''' Returns the content of the collection when initialized. When not null is returned.
    ''' </summary>
    ''' <returns></returns>
    Public Function OrUninitializedNull() As ICollection(Of String)
        If IsInitialized Then
            Return CType(Me, ICollection(Of String))
        End If
        Return Nothing
    End Function

End Class
