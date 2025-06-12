Imports System.Reflection
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Public Class CrossSectionAreaChangedProperty
    Inherits ChangedProperty

    Public Sub New(owner As ComparisonMapper, settings As GeneralSettingsBase)
        MyBase.New(owner, settings)
    End Sub

    Public Overrides Sub CompareObjectProperties(currentObject As Object, compareObject As Object, excludeProperties As List(Of String))
        If currentObject Is Nothing Or compareObject Is Nothing Then Exit Sub

        Dim currentCrossSectionArea As Cross_section_area = DirectCast(currentObject, Cross_section_area)
        Dim compareCrossSectionArea As Cross_section_area = DirectCast(compareObject, Cross_section_area)

        'HINT not used any longer as determination is now used as compare discriminator
        'If currentCrossSectionArea.Value_determination <> compareCrossSectionArea.Value_determination Then
        '    MyBase.ChangedProperties.Add(CrossSectionAreaPropertyName.Value_determination.ToString, compareCrossSectionArea.Value_determination)
        'End If

        If Math.Round(currentCrossSectionArea.Area.Value_component, 2) <> Math.Round(compareCrossSectionArea.Area.Value_component, 2) Then
            MyBase.ChangedProperties.Add(CrossSectionAreaPropertyName.Area.ToString, compareCrossSectionArea.Area)
        End If
    End Sub

    Public Overrides Function CompareObjectProperty(objProperty As PropertyInfo, currentObject As Object, compareObject As Object, excludeProperties As List(Of String)) As Boolean
        Return False
    End Function

End Class
