Public Class SegmentedCore
    Inherits SegmentedObject(Of Core_occurrence)

    Public Sub New(core As Core_occurrence, kblMapper As KBLMapper)
        MyBase.New(core, kblMapper)
    End Sub

    Protected Overrides Function GetObjectLength() As Numerical_value
        Return Me.Object?.GetLength
    End Function

    Public Overrides ReadOnly Property Id As String
        Get
            Return Me.Object?.SystemId
        End Get
    End Property

End Class