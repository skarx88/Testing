Imports System.Runtime.Serialization

<Serializable>
<KnownType(GetType(CarTransformationSetting))>
Public Class CarTransformationSettings
    Inherits ObjectModel.KeyedCollection(Of String, CarTransformationSetting)

    Protected Overrides Function GetKeyForItem(item As CarTransformationSetting) As String
        Return item.CarFileName
    End Function

    Public Sub AddOrReplace(setting As CarTransformationSetting)
        If Me.Contains(GetKeyForItem(setting)) Then
            Me.Remove(GetKeyForItem(setting))
        End If
        Me.Add(setting)
    End Sub

End Class
