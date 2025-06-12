Imports System.Runtime.Serialization

<CollectionDataContract(Name:="ConnectorSettingCollection")>
<KnownType(GetType(ConnectorSetting_2023))>
Public Class ConnectorSettingCollection_2023
    Inherits System.Collections.ObjectModel.KeyedCollection(Of String, ConnectorSetting_2023)

    Public Sub New()
        MyBase.New
    End Sub

    Protected Overrides Function GetKeyForItem(item As ConnectorSetting_2023) As String
        Return item.ConnectorId
    End Function
End Class
