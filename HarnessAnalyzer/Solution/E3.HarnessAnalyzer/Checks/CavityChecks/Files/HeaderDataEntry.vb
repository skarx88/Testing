Imports System.Runtime.Serialization
Imports System.Xml.Schema
Imports System.Xml.Serialization

Namespace Checks.Cavities.Files

    <DataContract(Name:="Header", [Namespace]:=CavityChecksFile.Namespace)>
    <XmlType(TypeName:="Header")> ' for schema generation
    Public Class HeaderDataEntry

        Private Sub New()
        End Sub

        Private Sub New(version As String)
            Me.Version = version
        End Sub

        <DataMember(Order:=0)>
        <XmlAttribute>
        Property Version As String

        <DataMember(Order:=1)>
        <XmlElement(Form:=XmlSchemaForm.Unqualified, IsNullable:=True, Order:=1)>
        Property CreatedBy As String

        <DataMember(Order:=2)>
        <XmlElement(Form:=XmlSchemaForm.Unqualified, IsNullable:=False, Order:=2)>
        Property DateOfCreation As DateTime

        <DataMember(Order:=3)>
        <XmlElement(Form:=XmlSchemaForm.Unqualified, IsNullable:=True, Order:=3)>
        Property HarnessProject As String

        <DataMember(Order:=4)>
        <XmlElement(Form:=XmlSchemaForm.Unqualified, IsNullable:=True, Order:=4)>
        Property HarnessPartNumber As String

        <DataMember(Order:=5)>
        <XmlElement(Form:=XmlSchemaForm.Unqualified, IsNullable:=True, Order:=5)>
        Property HarnessVersion As String


        Public Shared Function New10Version() As HeaderDataEntry
            Return New HeaderDataEntry("1.0") With {.CreatedBy = Environment.UserName, .DateOfCreation = DateTime.Now}
        End Function

    End Class

End Namespace