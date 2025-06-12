Imports System.Reflection
Imports Zuken.E3.HarnessAnalyzer.Settings

Public Class NetChangedProperty
    Inherits ChangedProperty

    Private _currentActiveObjects As ICollection(Of String)
    Private _compareActiveObjects As ICollection(Of String)

    Public Sub New(owner As ComparisonMapper, currentKBLMapper As KblMapper, compareKBLMapper As KblMapper, currentActiveObjects As ICollection(Of String), compareActiveObjects As ICollection(Of String), settings As GeneralSettingsBase)
        MyBase.New(owner, currentKBLMapper, compareKBLMapper, settings)

        _currentActiveObjects = currentActiveObjects
        _compareActiveObjects = compareActiveObjects
    End Sub

    Public Overrides Sub CompareObjectProperties(currentObject As Object, compareObject As Object, excludeProperties As List(Of String))
        Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentObject, IEnumerable(Of Connection)), DirectCast(compareObject, IEnumerable(Of Connection)))
        Dim connectionComparisonMapper As New ConnectionComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, _currentActiveObjects, _compareActiveObjects, listConvertToDictionary, Me.Settings)
        connectionComparisonMapper.CompareObjects()

        If connectionComparisonMapper.HasChanges Then
            MyBase.ChangedProperties.Add("NetConnections", connectionComparisonMapper)
        End If
    End Sub

    Public Overrides Function CompareObjectProperty(objProperty As PropertyInfo, currentObject As Object, compareObject As Object, excludeProperties As List(Of String)) As Boolean
        Return False
    End Function

End Class
