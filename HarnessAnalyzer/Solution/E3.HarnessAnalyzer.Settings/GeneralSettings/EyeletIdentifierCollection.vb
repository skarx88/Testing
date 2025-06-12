Imports System.Xml.Serialization
Imports Zuken.E3.Lib.Schema.Kbl.Properties

<Serializable(), XmlInclude(GetType(EyeletIdentifier))>
Public Class EyeletIdentifierCollection
    Inherits IdentifierBaseCollection(Of EyeletIdentifier)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub AddEyeletIdentifier(identifier As EyeletIdentifier)
        MyBase.AddIdentifier(identifier)
    End Sub

    Public Function FindUltrasonicSpliceIdentifier(kblPropertyName As String) As EyeletIdentifier
        Return CType(MyBase.FindIdentifier(kblPropertyName), EyeletIdentifier)

    End Function

    Public Overrides Sub CreateDefaultIdentifiers()
        Me.Add(New EyeletIdentifier(ConnectorPropertyName.Id.ToString, "W*"))
    End Sub
End Class
