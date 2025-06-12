Imports System.ComponentModel
Imports System.Xml.Serialization

<Serializable()> _
Public Class RedliningGroup

    Public Sub New()
        ChangeTag = String.Empty
        Comment = String.Empty
        LastChangedBy = System.Security.Principal.WindowsIdentity.GetCurrent.Name
        LastChangedOn = Now
        ID = Guid.NewGuid.ToString
    End Sub

    <XmlAttributeAttribute(DataType:="ID")>
    Public Property ID As String
    Public Property ChangeTag As String
    Public Property Comment As String
    Public Property LastChangedBy As String
    Public Property LastChangedOn As Date

End Class