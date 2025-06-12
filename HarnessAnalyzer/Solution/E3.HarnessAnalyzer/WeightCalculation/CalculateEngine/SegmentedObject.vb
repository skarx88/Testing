
Public MustInherit Class SegmentedObject(Of T)
    Inherits SegmentedBase

    Private _obj As T

    Public Sub New(obj As T, kblMapper As KBLMapper)
        MyBase.New(kblMapper)
        _obj = obj
    End Sub

    ReadOnly Property [Object] As T
        Get
            Return _obj
        End Get
    End Property

End Class
