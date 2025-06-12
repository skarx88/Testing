Imports System.Reflection
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Public Class CavityChangedProperty
    Inherits ChangedProperty

    Private _currentActiveObjects As ICollection(Of String)
    Private _compareActiveObjects As ICollection(Of String)

    Public Sub New(owner As ComparisonMapper, currentKBLMapper As KblMapper, compareKBLMapper As KblMapper, currentActiveObjects As ICollection(Of String), compareActiveObjects As ICollection(Of String), settings As GeneralSettingsBase)
        MyBase.New(owner, currentKBLMapper, compareKBLMapper, settings)

        _currentActiveObjects = currentActiveObjects
        _compareActiveObjects = compareActiveObjects
    End Sub

    Public Overrides Function CompareObjectProperty(objProperty As PropertyInfo, currentObject As Object, compareObject As Object, excludeProperties As List(Of String)) As Boolean
        Dim currentProperty As Object = objProperty.GetValue(currentObject, Nothing)
        Dim compareProperty As Object = objProperty.GetValue(compareObject, Nothing)
        If (currentProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(Cavity_occurrence.Associated_plug)
                    Dim currentCavityPlug As Cavity_plug = _currentKBLMapper.GetPart(Of Cavity_plug)(_currentKBLMapper.GetOccurrenceObject(Of Cavity_plug_occurrence)(currentProperty.ToString).Part)
                    Dim compareCavityPlug As Cavity_plug = Nothing
                    If (compareProperty IsNot Nothing) Then
                        compareCavityPlug = _compareKBLMapper.GetPart(Of Cavity_plug)(_compareKBLMapper.GetOccurrenceObject(Of Cavity_plug_occurrence)(compareProperty.ToString).Part)
                    End If

                    Dim plugComparisonMapper As New SingleObjectComparisonMapper(Me._owner, currentCavityPlug, compareCavityPlug, _currentKBLMapper, _compareKBLMapper, Me.Settings)
                    If Not excludeProperties.Contains(ConnectorPropertyName.Plug_part_information) Then
                        plugComparisonMapper.CompareObjects()
                    End If

                    If (plugComparisonMapper.HasChanges) AndAlso (plugComparisonMapper.Changes.HasModified OrElse plugComparisonMapper.Changes.HasNew) Then
                        MyBase.ChangedProperties.Add(ConnectorPropertyName.Plug_part_information, plugComparisonMapper)
                    End If

                    If (currentCavityPlug IsNot Nothing) Then
                        If (compareCavityPlug IsNot Nothing) Then
                            If (currentCavityPlug.Part_number <> compareCavityPlug.Part_number) Then
                                MyBase.ChangedProperties.Add(ConnectorPropertyName.Plug_part_number, compareCavityPlug.Part_number)
                            End If
                        Else
                            MyBase.ChangedProperties.Add(ConnectorPropertyName.Plug_part_number, String.Empty)
                        End If
                    ElseIf (compareCavityPlug IsNot Nothing) Then
                        MyBase.ChangedProperties.Add(ConnectorPropertyName.Plug_part_number, compareCavityPlug.Part_number)
                    End If
            End Select
        ElseIf (compareProperty IsNot Nothing) Then
            If (objProperty.Name = NameOf(Cavity_occurrence.Associated_plug)) Then
                MyBase.ChangedProperties.Add(ConnectorPropertyName.Plug_part_number, _compareKBLMapper.GetPart(Of Cavity_plug)(_compareKBLMapper.GetOccurrenceObject(Of Cavity_plug_occurrence)(compareProperty.ToString).Part).Part_number)
            End If
        End If

        Return False
    End Function

End Class
