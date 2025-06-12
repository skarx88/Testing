Public Class SegmentedWire
    Inherits SegmentedObject(Of Wire_occurrence)

    Public Sub New(wire As Wire_occurrence, kblMapper As KBLMapper)
        MyBase.New(wire, kblMapper)
    End Sub

    Protected Overrides Function GetObjectLength() As Numerical_value
        Return Me.Object.GetLength
    End Function

    Public Overrides ReadOnly Property Id As String
        Get
            Return Me.Object?.SystemId
        End Get
    End Property

End Class