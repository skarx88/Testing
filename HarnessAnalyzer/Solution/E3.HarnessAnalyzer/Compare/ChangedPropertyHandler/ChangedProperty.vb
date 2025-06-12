Imports System.Reflection
Imports Zuken.E3.HarnessAnalyzer.Settings

Public MustInherit Class ChangedProperty

    Protected _currentKBLMapper As KblMapper
    Protected _compareKBLMapper As KblMapper
    Protected _owner As ComparisonMapper
    Private _settings As GeneralSettingsBase

    Public MustOverride Function CompareObjectProperty(objProperty As PropertyInfo, currentObject As Object, compareObject As Object, excludeProperties As List(Of String)) As Boolean

    Public Overridable Sub OnAfterCompareObjectProperties(currentObject As Object, compareObject As Object)
    End Sub

    Protected Sub New(owner As ComparisonMapper, currentKBLMapper As KblMapper, compareKBLMapper As KblMapper, settings As GeneralSettingsBase)
        _currentKBLMapper = currentKBLMapper
        _compareKBLMapper = compareKBLMapper
        _owner = owner
        _settings = settings
        ChangedProperties = New Dictionary(Of String, Object)
    End Sub

    Protected Sub New(owner As ComparisonMapper, settings As GeneralSettingsBase)
        owner = owner
        ChangedProperties = New Dictionary(Of String, Object)
    End Sub

    Public Property ChangedProperties As Dictionary(Of String, Object)

    ReadOnly Property Settings As GeneralSettingsBase
        Get
            Return _settings
        End Get
    End Property

    Public Overridable Sub CompareObjectProperties(currentObject As Object, compareObject As Object, excludeProperties As List(Of String))
        If currentObject Is Nothing Or compareObject Is Nothing Then
            Exit Sub
        End If

        For Each objProperty As PropertyInfo In currentObject.GetType.GetProperties
            If (objProperty.Name.ToString <> Zuken.E3.HarnessAnalyzer.Shared.SYSTEM_ID) AndAlso Not excludeProperties.Contains(GetOverriddenPropertyName(objProperty.Name)) Then
                If CompareObjectProperty(objProperty, currentObject, compareObject, excludeProperties) Then
                    Continue For 'HINT MR this case is only on slots of connectors for whatever reason
                End If
            End If
        Next
        OnAfterCompareObjectProperties(currentObject, compareObject)
    End Sub

    Protected Overridable Function GetOverriddenPropertyName(propName As String) As String
        Return propName
    End Function

End Class
