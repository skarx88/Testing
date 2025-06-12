Imports System.Reflection
Imports Zuken.E3.HarnessAnalyzer.Settings


Public Class WireLengthChangedProperty
    Inherits ChangedProperty

    Public Sub New(owner As ComparisonMapper, settings As GeneralSettingsBase)
        MyBase.New(owner, settings)
    End Sub

    'HINT basically the unit is lost here in the compare but it is not clear what a change with unit has for impacts
    Public Overrides Sub CompareObjectProperties(currentObject As Object, compareObject As Object, excludeProperties As List(Of String))
        If Math.Round(DirectCast(currentObject, Wire_length).Length_value.Value_component, 2) <> Math.Round(DirectCast(compareObject, Wire_length).Length_value.Value_component, 2) Then
            MyBase.ChangedProperties.Add(NameOf(Wire_length.Length_value), Math.Round(DirectCast(compareObject, Wire_length).Length_value.Value_component, 2))
        End If
    End Sub

    Public Overrides Function CompareObjectProperty(objProperty As PropertyInfo, currentObject As Object, compareObject As Object, excludeProperties As List(Of String)) As Boolean
        Return False
    End Function
End Class
