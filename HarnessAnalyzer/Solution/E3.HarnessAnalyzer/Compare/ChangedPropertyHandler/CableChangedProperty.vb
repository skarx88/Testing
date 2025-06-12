Imports System.Reflection
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Public Class CableChangedProperty
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
                Case NameOf(Special_wire_occurrence.Part)
                    Dim currentGeneralWire As General_wire = _currentKBLMapper.GetPart(Of General_wire)(currentProperty.ToString)
                    Dim compareGeneralWire As General_wire = _compareKBLMapper.GetPart(Of General_wire)(compareProperty.ToString)
                    Dim partChangedProperty As GeneralWireChangedProperty = New GeneralWireChangedProperty(Me._owner, _currentKBLMapper, _compareKBLMapper, True, Me.Settings)

                    partChangedProperty.CompareObjectProperties(currentGeneralWire, compareGeneralWire, excludeProperties)
                    If partChangedProperty.ChangedProperties.Count <> 0 Then
                        MyBase.ChangedProperties.Add(objProperty.Name, partChangedProperty)
                    End If
                Case NameOf(Special_wire_occurrence.Installation_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Installation_instruction)), DirectCast(compareProperty, IEnumerable(Of Installation_instruction)))
                    Dim installationInfoComparisonMapper As New InstallationInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, _currentActiveObjects, _compareActiveObjects, listConvertToDictionary, Me.Settings)
                    installationInfoComparisonMapper.CompareObjects()

                    If installationInfoComparisonMapper.HasChanges Then
                        MyBase.ChangedProperties.Add(objProperty.Name, installationInfoComparisonMapper)
                    End If
                Case NameOf(Special_wire_occurrence.Length_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Wire_length)), DirectCast(compareProperty, IEnumerable(Of Wire_length)))
                    Dim wireLengthComparisonMapper As New WireLengthComparisonMapper(Me._owner, listConvertToDictionary, Me.Settings)
                    wireLengthComparisonMapper.CompareObjects()

                    If wireLengthComparisonMapper.HasChanges Then
                        MyBase.ChangedProperties.Add(objProperty.Name, wireLengthComparisonMapper)
                        FillChangesForPrimaryLength(currentProperty, compareProperty, wireLengthComparisonMapper)
                    End If
                Case NameOf(Special_wire_occurrence.Core_occurrence)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Core_occurrence)), DirectCast(compareProperty, IEnumerable(Of Core_occurrence)))
                    Dim installationInfoComparisonMapper As New CoreOccurrenceComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, _currentActiveObjects, _compareActiveObjects, listConvertToDictionary, Me.Settings)
                    installationInfoComparisonMapper.CompareObjects()

                    If installationInfoComparisonMapper.HasChanges Then
                        MyBase.ChangedProperties.Add(objProperty.Name, installationInfoComparisonMapper)
                    End If
                Case NameOf(Special_wire_occurrence.Special_wire_id)
                    If (currentProperty?.ToString <> compareProperty.ToString) OrElse (compareProperty Is Nothing AndAlso Not String.IsNullOrEmpty(CStr(currentProperty))) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, compareProperty)
                    End If
            End Select
        ElseIf (compareProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(Special_wire_occurrence.Installation_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Installation_instruction), DirectCast(compareProperty, IEnumerable(Of Installation_instruction)))
                    Dim installationInfoComparisonMapper As New InstallationInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, _currentActiveObjects, _compareActiveObjects, listConvertToDictionary, Me.Settings)
                    installationInfoComparisonMapper.CompareObjects()

                    If (installationInfoComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, installationInfoComparisonMapper)
                    End If
                Case NameOf(Special_wire_occurrence.Length_information)
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

    Private Sub FillChangesForPrimaryLength(currentProperty As Object, compareProperty As Object, lengthComparisonMapper As WireLengthComparisonMapper)
        'HINT Special hack for Daimler to get changes in wire length display 
        For Each chg As ChangedItem In lengthComparisonMapper.Changes
            If chg.Key = lengthComparisonMapper.DefaultCableLengthType.ToLower Then

                Dim refLengthes As New List(Of Wire_length)
                For Each wlength As Wire_length In DirectCast(currentProperty, Wire_length())
                    If wlength.Length_type.ToLower = lengthComparisonMapper.DefaultCableLengthType.ToLower Then
                        refLengthes.Add(wlength)
                    End If
                Next
                Dim compLengthes As New List(Of Wire_length)
                For Each wlength As Wire_length In DirectCast(compareProperty, Wire_length())
                    If wlength.Length_type.ToLower = lengthComparisonMapper.DefaultCableLengthType.ToLower Then
                        compLengthes.Add(wlength)
                    End If
                Next

                Dim listConvertToDictionary As New ListConvertToDictionary(refLengthes, compLengthes)
                Dim wireLengthComparisonMapper As New WireLengthComparisonMapper(Me._owner, listConvertToDictionary, Me.Settings)
                wireLengthComparisonMapper.CompareObjects()

                MyBase.ChangedProperties.Add(InformationHub.PRIMARY_LENGTH_COLUMN_KEY, wireLengthComparisonMapper)
            End If
        Next
    End Sub
    Public Overrides Sub OnAfterCompareObjectProperties(currentObject As Object, compareObject As Object)
        Dim currentCable As Special_wire_occurrence = DirectCast(currentObject, Special_wire_occurrence)
        Dim compareCable As Special_wire_occurrence = DirectCast(compareObject, Special_wire_occurrence)

        Dim currentSegments As New Dictionary(Of String, String)
        Dim compareSegments As New Dictionary(Of String, String)

        For Each coreOccurrence As Core_occurrence In currentCable.Core_occurrence
            For Each segCurrent As Segment In _currentKBLMapper.KBLWireSegmentMapper.GetSegmentsOrEmpty(coreOccurrence.SystemId)
                currentSegments.TryAdd(segCurrent.SystemId, segCurrent.Id)
            Next
        Next

        For Each coreOccurrence As Core_occurrence In compareCable.Core_occurrence
            For Each segCompare As Segment In _compareKBLMapper.KBLWireSegmentMapper.GetSegmentsOrEmpty(coreOccurrence.SystemId)
                compareSegments.TryAdd(segCompare.SystemId, segCompare.Id)
            Next
        Next

        Dim listConvertToDict As New ListConvertToDictionary(currentSegments, compareSegments)
        Dim commonComparisonMapper As New CommonComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, _currentActiveObjects, _compareActiveObjects, listConvertToDict, Me.Settings)
        commonComparisonMapper.CompareObjects()

        If commonComparisonMapper.HasChanges Then
            MyBase.ChangedProperties.Add(CablePropertyName.Routing.ToString, commonComparisonMapper)
        End If
    End Sub

End Class
