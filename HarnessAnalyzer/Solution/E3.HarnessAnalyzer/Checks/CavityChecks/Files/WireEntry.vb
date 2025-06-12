Imports System.Runtime.Serialization
Imports System.Xml.Serialization

Namespace Checks.Cavities.Files

    <DataContract([Namespace]:=CavityChecksFile.Namespace)>
    <XmlType(TypeName:="Wire")> ' for schema generation
    Public Class WireEntry

        Friend Sub New()
        End Sub

        Friend Overridable Sub CopyPropertiesFrom(cavWire As Views.Model.CavityWireView)
            Checked = CheckedString.GetCheckedState(cavWire.CheckState)
            KblSystemId = cavWire.KblWireId
            WireNumber = cavWire.WireName
        End Sub

        <IgnoreDataMember>
        <XmlIgnore> ' for xsd schema generation
        Property KblSystemId As String

        <IgnoreDataMember>
        <XmlIgnore> ' for xsd schema generation
        Property Id As String

        <DataMember(Order:=0)>
        <XmlElement(IsNullable:=True, Order:=0)> ' for xsd generation
        Property WireNumber As String

        <DataMember(Order:=1)>
        <XmlElement(IsNullable:=False, Order:=1)> ' for xsd generation
        Property Checked As CheckedState

        Public Overrides Function ToString() As String
            Return String.Format("{0}: {1}", Me.KblSystemId, Me.Checked)
        End Function

    End Class

End Namespace