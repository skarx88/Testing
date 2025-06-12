Imports System.Runtime.Serialization

<DataContract(Name:="ConnectorSetting")>
<KnownType(GetType(CavityWireSetting_2023))>
Public Class ConnectorSetting_2023

    Public Sub New()
    End Sub

    <DataMember> Property ConnectorId As String
    <DataMember> Property Name As String
    <DataMember> Property Cavities As New List(Of CavityWireSetting_2023)

End Class