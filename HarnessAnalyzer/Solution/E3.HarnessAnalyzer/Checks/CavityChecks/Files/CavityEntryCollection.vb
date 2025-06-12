Imports System.Runtime.Serialization
Imports System.Xml.Serialization

Namespace Checks.Cavities.Files

    <CollectionDataContract(Name:="Cavitites", ItemName:="Cavity", [Namespace]:=CavityChecksFile.Namespace)>
    <XmlType(TypeName:="Cavitites")> ' for schema generation
    Public Class CavityEntryCollection
        Inherits ObjectModel.Collection(Of CavityEntry)
    End Class

End Namespace