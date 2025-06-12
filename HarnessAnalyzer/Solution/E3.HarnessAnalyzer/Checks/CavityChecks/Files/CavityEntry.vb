Imports System.Runtime.Serialization
Imports System.Xml.Serialization

Namespace Checks.Cavities.Files

    <DataContract(Name:="Cavity", [Namespace]:=CavityChecksFile.Namespace)>
    <XmlType(TypeName:="Cavity")> ' for schema generation
    <KnownType(GetType(WireEntryCollection))>
    Public Class CavityEntry

        Friend Sub New()
        End Sub

        <DataMember(Name:="Name", Order:=0)>
        <XmlElement(ElementName:="Name", IsNullable:=True, Order:=0)> ' for xsd generation
        Property CavityName As String

        <IgnoreDataMember>
        <XmlIgnore> ' for xsd generation
        Property CavityKblId As String

        <DataMember(Order:=1)>
        <XmlElement(IsNullable:=False, Order:=1)> ' for xsd generation
        Property Checked As CheckedState

        <DataMember(Order:=2)>
        <XmlElement(IsNullable:=False, Order:=2)> ' for xsd generation
        ReadOnly Property Wires As New WireEntryCollection

        Protected Friend Overridable Sub CopyPropertiesFrom(cvWire As Views.Model.CavityWireView)
            CavityKblId = cvWire.KblCavityId
            CavityName = cvWire.CavityName
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("{0}, Wires: {1}", Me.CavityKblId, Me.Wires.Count)
        End Function

    End Class


End Namespace