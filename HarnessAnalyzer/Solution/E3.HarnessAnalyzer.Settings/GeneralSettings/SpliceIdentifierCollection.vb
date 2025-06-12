Imports System.Xml.Serialization
Imports Zuken.E3.Lib.Schema.Kbl.Properties

<Serializable(), XmlInclude(GetType(SpliceIdentifier))>
Public Class SpliceIdentifierCollection
    Inherits IdentifierBaseCollection(Of SpliceIdentifier)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub AddSpliceIdentifier(identifier As SpliceIdentifier)
        MyBase.AddIdentifier(identifier)
    End Sub

    Public Function FindSpliceIdentifier(kblPropertyName As String) As SpliceIdentifier
        Return CType(MyBase.FindIdentifier(kblPropertyName), SpliceIdentifier)
    End Function

    Public Overrides Sub CreateDefaultIdentifiers()
        Me.Add(New SpliceIdentifier(ConnectorPropertyName.Id.ToString, "Z*"))
    End Sub
End Class
