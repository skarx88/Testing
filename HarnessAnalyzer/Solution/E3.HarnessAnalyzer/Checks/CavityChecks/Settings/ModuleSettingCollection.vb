Imports System.Runtime.Serialization

Namespace Checks.Cavities.Settings

    <CollectionDataContract>
    Public Class ModuleSettingCollection
        Inherits System.Collections.ObjectModel.KeyedCollection(Of String, ModuleSetting)

        Protected Overrides Function GetKeyForItem(item As ModuleSetting) As String
            Return item.SystemId
        End Function

    End Class

End Namespace