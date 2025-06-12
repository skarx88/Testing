Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Formatters.Binary


<Serializable>
Public Class ViewPortSizeSettingCollection_2023

    Private _list As New List(Of ViewportSizeSetting_2023)

    Private Const VPSETTINGSCLASSNAME_2023 As String = "ViewportSettings"

    Public Shared Function Load(s As System.IO.Stream) As ViewPortSizeSettingCollection_2023
        Dim newSettings As New ViewPortSizeSettingCollection_2023
        Dim binaryFormatter As New BinaryFormatter
        binaryFormatter.Binder = New ViewportSizeSettingtypeBinder
        For Each vps As ViewportSizeSetting_2023 In TryCast(binaryFormatter.Deserialize(s), IEnumerable).OfType(Of ViewportSizeSetting_2023)
            newSettings._list.Add(vps)
        Next
        Return newSettings
    End Function

    Public Function GetSettings() As IEnumerable(Of ViewportSizeSetting_2023)
        Return _list
    End Function

    Private Class ViewportSizeSettingtypeBinder
        Inherits SerializationBinder

        Public Overrides Function BindToType(assemblyName As String, typeName As String) As Type
            If typeName = VPSETTINGSCLASSNAME_2023 Then
                Return GetType(ViewportSizeSetting_2023)
            End If
            Return Nothing
        End Function

    End Class


End Class
