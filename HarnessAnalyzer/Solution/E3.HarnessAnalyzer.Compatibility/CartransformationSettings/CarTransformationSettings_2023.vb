<Serializable>
Public Class CarTransformationSettings_2023
    Inherits ObjectModel.KeyedCollection(Of String, CarTransformationSetting_2023)

    Protected Overrides Function GetKeyForItem(item As CarTransformationSetting_2023) As String
        Return item.CarFileName
    End Function

    Public Sub AddOrReplace(setting As CarTransformationSetting_2023)
        If Me.Contains(GetKeyForItem(setting)) Then
            Me.Remove(GetKeyForItem(setting))
        End If
        Me.Add(setting)
    End Sub

End Class
