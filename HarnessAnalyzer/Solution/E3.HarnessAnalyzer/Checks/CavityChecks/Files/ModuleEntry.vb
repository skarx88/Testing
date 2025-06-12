Imports System.Runtime.Serialization
Imports System.Xml.Schema
Imports System.Xml.Serialization

Namespace Checks.Cavities.Files

    <KnownType("GetKnownTypes")>
    <DataContract(Name:="Module", [Namespace]:=CavityChecksFile.Namespace)>
    <XmlType(TypeName:="Module")> ' for schema generation
    Public Class ModuleEntry

        Private Sub New()
        End Sub

        Public Sub New(obj As [Module])
            obj.CopyPropertiesTo(Me)
        End Sub

        Public Shared Function GetKnownTypes() As IEnumerable(Of Type)
            Return GetType(ModuleEntry).GetProperties.Select(Function(p) p.PropertyType)
        End Function

        <DataMember(Order:=0)>
        <XmlElement(Form:=XmlSchemaForm.Unqualified, IsNullable:=True, Order:=0)>
        Public Property Abbreviation As String

        <DataMember(Order:=1)>
        <XmlElement(Form:=XmlSchemaForm.Unqualified, IsNullable:=True, Order:=1)>
        Public Property Description As String

        <DataMember(Order:=2)>
        <XmlElement(Form:=XmlSchemaForm.Unqualified, IsNullable:=True, Order:=2)>
        Public Property PartNumber As String

        <DataMember(Order:=3)>
        <XmlElement(Form:=XmlSchemaForm.Unqualified, IsNullable:=True, Order:=3)>
        Public Property CopyrightNote As String

        <DataMember(Order:=4)>
        <XmlElement(Form:=XmlSchemaForm.Unqualified, IsNullable:=True, Order:=4)>
        Public Property Version As String

        <DataMember(Order:=5)>
        <XmlElement(Form:=XmlSchemaForm.Unqualified, IsNullable:=True, Order:=5)>
        Public Property CompanyName As String

        <XmlIgnore>
        <IgnoreDataMember>
        Public Property SystemId As String

    End Class

End Namespace