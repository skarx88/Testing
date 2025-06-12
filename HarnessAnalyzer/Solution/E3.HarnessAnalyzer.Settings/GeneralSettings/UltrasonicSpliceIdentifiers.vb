
<Serializable()>
Public Class UltrasonicSpliceIdentifier
    Inherits IdentifierBase

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(kblPropName As String, criteria As String)
        MyBase.New(kblPropName, criteria)
    End Sub

End Class