Imports System.Runtime.Serialization

<DataContract(Name:="CavityWireSetting")>
Public Class CavityWireSetting_2023

    Public Sub New()
    End Sub

    <DataMember> Property CavityName As String
    <DataMember> Property WireNumber As String
    <DataMember> Property Checked As Integer
    <DataMember> Property ConnectorId As String
    <DataMember> Property CavityId As String
    <DataMember> Property WireId As String

End Class
