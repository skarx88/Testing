Imports System.Runtime.Serialization

<CollectionDataContract(Name:="ModuleSettingCollection")>
Public Class ModuleSettingCollection_2023
    Inherits System.Collections.ObjectModel.KeyedCollection(Of String, ModuleSetting_2023)

    Protected Overrides Function GetKeyForItem(item As ModuleSetting_2023) As String
        Return item.SystemId
    End Function

End Class
