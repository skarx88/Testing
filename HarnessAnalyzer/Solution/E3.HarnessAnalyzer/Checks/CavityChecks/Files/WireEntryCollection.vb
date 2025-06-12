Imports System.Runtime.Serialization
Imports System.Xml.Serialization

Namespace Checks.Cavities.Files

    <CollectionDataContract(Name:="Wires", ItemName:="Wire", [Namespace]:=CavityChecksFile.Namespace)>
    <XmlType(TypeName:="Wires")> ' for schema generation
    Public Class WireEntryCollection
        Inherits ObjectModel.Collection(Of WireEntry)
    End Class

End Namespace
