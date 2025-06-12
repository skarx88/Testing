Imports System.Runtime.Serialization

Namespace Checks.Cavities.Settings

    <CollectionDataContract()>
    Public Class ConnectorSettingCollection
        Inherits System.Collections.ObjectModel.KeyedCollection(Of String, ConnectorSetting)

        Public Sub New()
            MyBase.New
        End Sub

        Protected Overrides Function GetKeyForItem(item As ConnectorSetting) As String
            Return item.ConnectorId
        End Function

    End Class

End Namespace