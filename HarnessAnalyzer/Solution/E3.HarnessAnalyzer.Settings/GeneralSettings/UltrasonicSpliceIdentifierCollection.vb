Imports System.Xml.Serialization
Imports Zuken.E3.Lib.Schema.Kbl.Properties

<Serializable(), XmlInclude(GetType(UltrasonicSpliceIdentifier))>
Public Class UltrasonicSpliceIdentifierCollection
    Inherits IdentifierBaseCollection(Of UltrasonicSpliceIdentifier)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub AddUltrasonicSpliceIdentifier(identifier As UltrasonicSpliceIdentifier)
        MyBase.AddIdentifier(identifier)
    End Sub

    Public Function FindUltrasonicSpliceIdentifier(kblPropertyName As String) As UltrasonicSpliceIdentifier
        Return CType(MyBase.FindIdentifier(kblPropertyName), UltrasonicSpliceIdentifier)
    End Function

    Public Overrides Sub CreateDefaultIdentifiers()
        Me.Add(New UltrasonicSpliceIdentifier(ConnectorPropertyName.Id.ToString, "Z*"))
    End Sub
End Class