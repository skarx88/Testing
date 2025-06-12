Imports System.Xml.Serialization
Imports Zuken.E3.Lib.Schema.Kbl.Properties
<Serializable(), XmlInclude(GetType(InlinerIdentifier))>
Public Class InlinerIdentifierCollection
    Inherits IdentifierBaseCollection(Of InlinerIdentifier)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub AddInlinerIdentifier(identifier As InlinerIdentifier)
        MyBase.AddIdentifier(identifier)
    End Sub

    Public Function FindInlinerIdentifier(kblPropertyName As String) As InlinerIdentifier
        Return CType(MyBase.FindIdentifier(kblPropertyName), InlinerIdentifier)
    End Function

    Public Overrides Sub CreateDefaultIdentifiers()
        Me.Add(New InlinerIdentifier(ConnectorPropertyName.Id.ToString, "X*{-S,-B}*"))
    End Sub

End Class