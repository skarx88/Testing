Imports System.Reflection
Imports Zuken.E3.HarnessAnalyzer.Settings

Public Class LocalizedDescriptionChangedProperty
    Inherits ChangedProperty

    Public Sub New(owner As ComparisonMapper, settings As GeneralSettingsBase)
        MyBase.New(owner, settings)
    End Sub

    Public Overrides Sub CompareObjectProperties(currentObject As Object, compareObject As Object, excludeProperties As List(Of String))
        Dim compareLocString As Localized_string = DirectCast(compareObject, Localized_string)
        Dim currentLocString As Localized_string = DirectCast(currentObject, Localized_string)

        If (currentObject Is Nothing OrElse compareObject Is Nothing) Then
            Return
        End If

        With ChangedProperties
            If (currentLocString.Value <> compareLocString.Value) Then
                .Add(NameOf(Zuken.E3.Lib.Schema.Kbl.Localized_string.Value), compareLocString.Value)
            End If
            If currentLocString.Language_code <> compareLocString.Language_code Then
                .Add(NameOf(Zuken.E3.Lib.Schema.Kbl.Localized_string.Language_code), compareLocString.Language_code)
            End If
        End With
    End Sub

    Public Overrides Function CompareObjectProperty(objProperty As PropertyInfo, currentObject As Object, compareObject As Object, excludeProperties As List(Of String)) As Boolean
        Return False
    End Function

End Class
