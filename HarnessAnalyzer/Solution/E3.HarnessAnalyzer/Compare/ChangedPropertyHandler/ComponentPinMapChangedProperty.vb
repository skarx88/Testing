Imports System.Reflection
Imports Zuken.E3.HarnessAnalyzer.Settings

Public Class ComponentPinMapChangedProperty
    Inherits ChangedProperty

    Public Sub New(owner As ComparisonMapper, currentKBLMapper As KblMapper, compareKBLMapper As KblMapper, settings As GeneralSettingsBase)
        MyBase.New(owner, currentKBLMapper, compareKBLMapper, settings)
    End Sub

    Public Overrides Sub CompareObjectProperties(currentObject As Object, compareObject As Object, excludeProperties As List(Of String))
        Dim compareComponentPinMap As Component_pin_map = DirectCast(compareObject, Component_pin_map)
        Dim currentComponentPinMap As Component_pin_map = DirectCast(currentObject, Component_pin_map)

        If (currentObject Is Nothing OrElse compareObject Is Nothing) Then
            Return
        End If

        With ChangedProperties
            If (currentComponentPinMap.Component_pin_number <> compareComponentPinMap.Component_pin_number) Then
                .Add(NameOf(E3.Lib.Schema.Kbl.Component_pin_map.Component_pin_number), compareComponentPinMap.Component_pin_number)
            End If
            If (currentComponentPinMap.Cavity_number <> compareComponentPinMap.Cavity_number) Then
                .Add(NameOf(E3.Lib.Schema.Kbl.Component_pin_map.Cavity_number), compareComponentPinMap.Cavity_number)
            End If

            If (currentComponentPinMap.Connected_contact_points IsNot Nothing) Then
                If (compareComponentPinMap.Connected_contact_points IsNot Nothing) Then
                    If (currentComponentPinMap.GetConnectedContactPointInformation(_currentKBLMapper) <> compareComponentPinMap.GetConnectedContactPointInformation(_compareKBLMapper)) Then
                        .Add(NameOf(E3.Lib.Schema.Kbl.Component_pin_map.Connected_contact_points).ToString, compareComponentPinMap.GetConnectedContactPointInformation(_compareKBLMapper))
                    End If
                Else
                    .Add(NameOf(E3.Lib.Schema.Kbl.Component_pin_map.Connected_contact_points), String.Empty)
                End If
            ElseIf (compareComponentPinMap.Connected_contact_points IsNot Nothing) Then
                .Add(NameOf(E3.Lib.Schema.Kbl.Component_pin_map.Connected_contact_points), compareComponentPinMap.GetConnectedContactPointInformation(_compareKBLMapper))
            End If
        End With
    End Sub

    Public Overrides Function CompareObjectProperty(objProperty As PropertyInfo, currentObject As Object, compareObject As Object, excludeProperties As List(Of String)) As Boolean
        Return False
    End Function
End Class
