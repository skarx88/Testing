Imports System.Reflection
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Public Class CoreOccurrenceChangedProperty
    Inherits ChangedProperty

    Public Sub New(owner As ComparisonMapper, currentKBLMapper As KblMapper, compareKBLMapper As KblMapper, settings As GeneralSettingsBase)
        MyBase.New(owner, currentKBLMapper, compareKBLMapper, settings)
    End Sub

    Public Overrides Function CompareObjectProperty(objProperty As PropertyInfo, currentObject As Object, compareObject As Object, excludeProperties As List(Of String)) As Boolean
        Dim currentProperty As Object = objProperty.GetValue(currentObject, Nothing)
        Dim compareProperty As Object = objProperty.GetValue(compareObject, Nothing)
        If (currentProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(Core_occurrence.Part)
                    Dim currentCore As Core = _currentKBLMapper.GetPart(Of Core)(currentProperty.ToString)
                    Dim compareCore As Core = _compareKBLMapper.GetPart(Of Core)(compareProperty.ToString)
                    Dim partChangedProperty As CoreChangedProperty = New CoreChangedProperty(Me._owner, _currentKBLMapper, _compareKBLMapper, Me.Settings)

                    partChangedProperty.CompareObjectProperties(currentCore, compareCore, excludeProperties)
                    If partChangedProperty.ChangedProperties.Count <> 0 Then
                        MyBase.ChangedProperties.Add(objProperty.Name, partChangedProperty)
                    End If
                Case NameOf(Core_occurrence.Length_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Wire_length)), DirectCast(compareProperty, IEnumerable(Of Wire_length)))
                    Dim wireLengthComparisonMapper As New WireLengthComparisonMapper(Me._owner, listConvertToDictionary, Me.Settings)
                    wireLengthComparisonMapper.CompareObjects()

                    If wireLengthComparisonMapper.HasChanges Then
                        MyBase.ChangedProperties.Add(objProperty.Name, wireLengthComparisonMapper)
                    End If
                Case NameOf(Core_occurrence.Wire_number)
                    If (compareProperty IsNot Nothing AndAlso currentProperty.ToString <> compareProperty.ToString) OrElse (compareProperty Is Nothing AndAlso currentProperty.ToString <> String.Empty) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, compareProperty)
                    End If
            End Select
        ElseIf (compareProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(Core_occurrence.Length_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Wire_length), DirectCast(compareProperty, IEnumerable(Of Wire_length)))
                    Dim wireLengthComparisonMapper As New WireLengthComparisonMapper(Me._owner, listConvertToDictionary, Me.Settings)
                    wireLengthComparisonMapper.CompareObjects()

                    If (wireLengthComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, wireLengthComparisonMapper)
                    End If
                Case Else
                    MyBase.ChangedProperties.Add(objProperty.Name, compareProperty)
            End Select
        End If

        Return False
    End Function

    Public Overrides Sub OnAfterCompareObjectProperties(currentObject As Object, compareObject As Object)
        Dim currentCoreOcc As Core_occurrence = DirectCast(currentObject, Core_occurrence)
        Dim compareCoreOcc As Core_occurrence = DirectCast(compareObject, Core_occurrence)

        Dim currentConnection As Connection = _currentKBLMapper.GetConnectionOfWire(currentCoreOcc.SystemId)
        Dim compareConnection As Connection = _compareKBLMapper.GetConnectionOfWire(compareCoreOcc.SystemId)

        Dim currentStartContactPointId As String = If(currentConnection IsNot Nothing, currentConnection.GetStartContactPointId, String.Empty)
        Dim currentEndContactPointId As String = If(currentConnection IsNot Nothing, currentConnection.GetEndContactPointId, String.Empty)
        Dim compareStartContactPointId As String = If(compareConnection IsNot Nothing, compareConnection.GetStartContactPointId, String.Empty)
        Dim compareEndContactPointId As String = If(compareConnection IsNot Nothing, compareConnection.GetEndContactPointId, String.Empty)

        If (currentConnection IsNot Nothing AndAlso compareConnection IsNot Nothing) AndAlso ((currentConnection.Signal_name Is Nothing AndAlso compareConnection.Signal_name IsNot Nothing) OrElse (currentConnection.Signal_name IsNot Nothing AndAlso compareConnection.Signal_name Is Nothing) OrElse (currentConnection.Signal_name <> compareConnection.Signal_name)) Then
            MyBase.ChangedProperties.Add(ConnectionPropertyName.Signal_name.ToString, If(compareConnection.Signal_name IsNot Nothing, compareConnection.Signal_name, String.Empty))
        End If

        If (currentStartContactPointId <> String.Empty AndAlso compareStartContactPointId <> String.Empty) AndAlso (_currentKBLMapper.GetConnectorOfContactPoint(currentStartContactPointId)?.Id <> _compareKBLMapper.GetConnectorOfContactPoint(compareStartContactPointId)?.Id) Then
            MyBase.ChangedProperties.Add(ConnectionPropertyName.Connector_A.ToString, _compareKBLMapper.GetConnectorOfContactPoint(compareStartContactPointId)?.Id)
        End If

        If (currentStartContactPointId <> String.Empty AndAlso compareStartContactPointId <> String.Empty) AndAlso _currentKBLMapper.GetCavityOfContactPointId(currentStartContactPointId)?.Cavity_number <> _compareKBLMapper.GetCavityOfContactPointId(compareStartContactPointId)?.Cavity_number Then
            MyBase.ChangedProperties.Add(ConnectionPropertyName.Cavity_A.ToString, _compareKBLMapper.GetCavityOfContactPointId(compareStartContactPointId)?.Cavity_number)
        End If

        If (currentEndContactPointId <> String.Empty AndAlso compareEndContactPointId <> String.Empty) AndAlso _currentKBLMapper.GetConnectorOfContactPoint(currentEndContactPointId).Id <> _compareKBLMapper.GetConnectorOfContactPoint(compareEndContactPointId)?.Id Then
            MyBase.ChangedProperties.Add(ConnectionPropertyName.Connector_B.ToString, _compareKBLMapper.GetConnectorOfContactPoint(compareEndContactPointId).Id)
        End If

        If (currentEndContactPointId <> String.Empty AndAlso compareEndContactPointId <> String.Empty) AndAlso _currentKBLMapper.GetCavityOfContactPointId(currentEndContactPointId)?.Cavity_number <> _compareKBLMapper.GetCavityOfContactPointId(compareEndContactPointId)?.Cavity_number Then
            MyBase.ChangedProperties.Add(ConnectionPropertyName.Cavity_B.ToString, _compareKBLMapper.GetCavityOfContactPointId(compareEndContactPointId)?.Cavity_number)
        End If

    End Sub
End Class
