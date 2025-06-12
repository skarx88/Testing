Imports System.Reflection
Imports System.Text
Imports Zuken.E3.HarnessAnalyzer.Settings

Public Class ComponentChangedProperty
    Inherits ChangedProperty

    Public Sub New(owner As ComparisonMapper, currentKBLMapper As KblMapper, compareKBLMapper As KblMapper, settings As GeneralSettingsBase)
        MyBase.New(owner, currentKBLMapper, compareKBLMapper, settings)
    End Sub

    Public Overrides Function CompareObjectProperty(objProperty As PropertyInfo, currentObject As Object, compareObject As Object, excludeProperties As List(Of String)) As Boolean
        Dim currentProperty As Object = objProperty.GetValue(currentObject, Nothing)
        Dim compareProperty As Object = objProperty.GetValue(compareObject, Nothing)
        If (currentProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(E3.Lib.Schema.Kbl.Component_occurrence.Alias_id)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Alias_identification)), DirectCast(compareProperty, IEnumerable(Of Alias_identification)))
                    Dim aliasIdComparisonMapper As New AliasIdComparisonMapper(Me._owner, listConvertToDictionary, Me.Settings)
                    aliasIdComparisonMapper.CompareObjects()

                    If aliasIdComparisonMapper.HasChanges Then
                        MyBase.ChangedProperties.Add(NameOf(E3.Lib.Schema.Kbl.Component_occurrence.Alias_id), aliasIdComparisonMapper)
                    End If

                Case NameOf(E3.Lib.Schema.Kbl.Component_occurrence.Part)
                    Dim currentComponent As Component = _currentKBLMapper.GetPart(Of Component)(currentProperty.ToString)
                    Dim compareComponent As Component = _compareKBLMapper.GetPart(Of Component)(compareProperty.ToString)
                    Dim partChangedProperty As PartChangedProperty = New PartChangedProperty(Me._owner, _currentKBLMapper, _compareKBLMapper, Me.Settings)

                    partChangedProperty.CompareObjectProperties(currentComponent, compareComponent, excludeProperties)
                    If partChangedProperty.ChangedProperties.Count <> 0 Then
                        MyBase.ChangedProperties.Add(objProperty.Name, partChangedProperty)
                    End If

                Case NameOf(E3.Lib.Schema.Kbl.Component_occurrence.Mounting)

                    Dim currentMountingObjects As New List(Of String)
                    For Each referencedElements As String In currentProperty.ToString.SplitSpace.ToList
                        currentMountingObjects.Add(Harness.GetReferenceElement(referencedElements, _currentKBLMapper))
                    Next
                    currentMountingObjects.Sort()

                    Dim compareMountingObjects As New List(Of String)
                    If (compareProperty IsNot Nothing) Then
                        For Each referencedComponent As String In compareProperty.ToString.SplitSpace.ToList
                            compareMountingObjects.Add(Harness.GetReferenceElement(referencedComponent, _compareKBLMapper))
                        Next
                        compareMountingObjects.Sort()
                    End If

                    Dim currentMountingObjectString As String = currentMountingObjects.AsText
                    Dim compareMountingObjectString As String = compareMountingObjects.AsText

                    If (currentMountingObjectString <> compareMountingObjectString) Then
                        MyBase.ChangedProperties.Add(NameOf(E3.Lib.Schema.Kbl.Component_occurrence.Mounting), compareMountingObjectString)
                    End If
                Case NameOf(E3.Lib.Schema.Kbl.Component_occurrence.Component_pin_maps)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Component_pin_map)), DirectCast(compareProperty, IEnumerable(Of Component_pin_map)))
                    Dim componentPinMapComparisonMapper As New ComponentPinMapComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    componentPinMapComparisonMapper.CompareObjects()

                    If (componentPinMapComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(objProperty.Name.ToString, componentPinMapComparisonMapper)
                    End If

                Case NameOf(E3.Lib.Schema.Kbl.Component_occurrence.Installation_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Installation_instruction)), DirectCast(compareProperty, IEnumerable(Of Installation_instruction)))
                    Dim installationInfoComparisonMapper As New InstallationInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    installationInfoComparisonMapper.CompareObjects()

                    If (installationInfoComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(objProperty.Name.ToString, installationInfoComparisonMapper)
                    End If

                Case NameOf(E3.Lib.Schema.Kbl.Component_occurrence.Localized_description)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Localized_string)), DirectCast(compareProperty, IEnumerable(Of Localized_string)))
                    Dim locDescriptionComparer As New LocalizedDescriptionComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    locDescriptionComparer.CompareObjects()

                    If (locDescriptionComparer.HasChanges) Then
                        MyBase.ChangedProperties.Add(NameOf(E3.Lib.Schema.Kbl.Component_occurrence.Localized_description), locDescriptionComparer)
                    End If

                Case NameOf(E3.Lib.Schema.Kbl.Component_occurrence.Id), NameOf(E3.Lib.Schema.Kbl.Component_occurrence.Description)
                    If (compareProperty IsNot Nothing AndAlso currentProperty.ToString <> compareProperty.ToString) OrElse (compareProperty Is Nothing AndAlso currentProperty.ToString <> String.Empty) Then
                        If (objProperty.Name = NameOf(E3.Lib.Schema.Kbl.Component_occurrence.Id)) Then
                            MyBase.ChangedProperties.Add(NameOf(E3.Lib.Schema.Kbl.Component_occurrence.Id), compareProperty)
                        Else
                            MyBase.ChangedProperties.Add(objProperty.Name, compareProperty)
                        End If
                    End If
            End Select

        ElseIf (compareProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(E3.Lib.Schema.Kbl.Component_occurrence.Alias_id)
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Alias_identification), DirectCast(compareProperty, IEnumerable(Of Alias_identification)))
                    Dim aliasIdComparisonMapper As New AliasIdComparisonMapper(Me._owner, listConvertToDictionary, Me.Settings)
                    aliasIdComparisonMapper.CompareObjects()

                    If (aliasIdComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(NameOf(E3.Lib.Schema.Kbl.Component_occurrence.Alias_id), aliasIdComparisonMapper)
                    End If

                Case NameOf(E3.Lib.Schema.Kbl.Component_occurrence.Mounting)
                    Dim refInfos As String = String.Empty

                    For Each refId As String In compareProperty.ToString.SplitSpace
                        If (refInfos = String.Empty) Then
                            refInfos = Harness.GetReferenceElement(refId, _compareKBLMapper)
                        Else
                            refInfos = String.Format("{0}{1}{2}", refInfos, vbCrLf, Harness.GetReferenceElement(refId, _compareKBLMapper))
                        End If
                    Next

                    If (refInfos <> String.Empty) Then
                        MyBase.ChangedProperties.Add(NameOf(E3.Lib.Schema.Kbl.Component_occurrence.Mounting), String.Format("{0}{1}", vbCrLf, refInfos))
                    End If

                            'TODO the same nonsense here...but now in a different way
                            'Dim mountingObjects As String = String.Empty

                            'For Each mountingObject As String In compareProperty.ToString.SplitS
                            '    If (CType(_compareKBLMapper, IKblMappersProvider).KBLOccurrenceMapper.ContainsKey(mountingObject)) Then
                            '        Dim objectId As String = String.Empty
                            '        Dim myObjectType As String = String.Empty
                            '        Dim partNumber As String = String.Empty
                            '        Dim mountingObj As Object = CType(_compareKBLMapper, IKblMappersProvider).KBLOccurrenceMapper(mountingObject)

                            '        If (TypeOf mountingObj Is Cavity_occurrence) Then
                            '            Dim connectorOccurrence As Connector_occurrence = DirectCast(CType(_compareKBLMapper, IKblMappersProvider).KBLOccurrenceMapper(CType(_compareKBLMapper, IKblMappersProvider).KBLCavityConnectorMapper(mountingObject)), Connector_occurrence)
                            '            Dim cavityPart As Cavity = DirectCast(CType(_compareKBLMapper, IKblMappersProvider).KblPartMapper(DirectCast(CType(_compareKBLMapper, IKblMappersProvider).KBLOccurrenceMapper(mountingObject), Cavity_occurrence).Part), Cavity)

                            '            objectId = String.Format("{0},{1}", connectorOccurrence.Id, cavityPart.Cavity_number)
                            '            myObjectType = E3.Lib.Schema.Kbl.ObjectType.Cavity.ToString
                            '            partNumber = If(CType(_compareKBLMapper, IKblMappersProvider).KblPartMapper.ContainsKey(connectorOccurrence.Part), DirectCast(CType(_compareKBLMapper, IKblMappersProvider).KblPartMapper(connectorOccurrence.Part), Part).Part_number, String.Empty)
                            '        ElseIf (TypeOf mountingObj Is Connector_occurrence) Then
                            '            objectId = DirectCast(mountingObj, Connector_occurrence).Id
                            '            myObjectType = E3.Lib.Schema.Kbl.ObjectType.Connector.ToString
                            '            partNumber = If(CType(_compareKBLMapper, IKblMappersProvider).KblPartMapper.ContainsKey(DirectCast(mountingObj, Connector_occurrence).Part), DirectCast(CType(_compareKBLMapper, IKblMappersProvider).KblPartMapper(DirectCast(mountingObj, Connector_occurrence).Part), Part).Part_number, String.Empty)
                            '        ElseIf (TypeOf mountingObj Is Slot_occurrence) Then
                            '            Dim connectorOccurrence As Connector_occurrence = DirectCast(CType(_compareKBLMapper, IKblMappersProvider).KBLOccurrenceMapper(CType(_compareKBLMapper, IKblMappersProvider).KBLSlotConnectorMapper(mountingObject)), Connector_occurrence)

                            '            For Each cavityOccurrence As Cavity_occurrence In DirectCast(CType(_compareKBLMapper, IKblMappersProvider).KBLOccurrenceMapper(mountingObject), Slot_occurrence).Cavities
                            '                objectId = String.Format("{0},{1}", connectorOccurrence.Id, DirectCast(CType(_compareKBLMapper, IKblMappersProvider).KblPartMapper(cavityOccurrence.Part), Cavity).Cavity_number)
                            '                Exit For
                            '            Next

                            '            myObjectType = E3.Lib.Schema.Kbl.ObjectType.Slot.ToString
                            '            partNumber = If(CType(_compareKBLMapper, IKblMappersProvider).KblPartMapper.ContainsKey(connectorOccurrence.Part), DirectCast(CType(_compareKBLMapper, IKblMappersProvider).KblPartMapper(connectorOccurrence.Part), Part).Part_number, String.Empty)
                            '        Else
                            '            objectId = mountingObject
                            '            myObjectType = Replace(mountingObj.GetType.ToString, String.Format("{0}.", mountingObj.GetType.Namespace), String.Empty)
                            '            partNumber = "-"
                            '        End If

                            '        If (mountingObjects = String.Empty) Then
                            '            mountingObjects = String.Format("{0} ({1}) [{2}]", objectId, partNumber, myObjectType)
                            '        Else
                            '            mountingObjects = String.Format("{0}{1}{2}", mountingObjects, vbLf, String.Format("{0} ({1}) [{2}]", objectId, partNumber, myObjectType))
                            '        End If
                            '    End If
                            'Next

                            'If (mountingObjects <> String.Empty) Then MyBase.ChangedProperties.Add(ComponentPropertyName.Mounting.ToString, String.Format("{0}{1}", vbLf, mountingObjects))
                Case NameOf(E3.Lib.Schema.Kbl.Component_occurrence.Component_pin_maps)
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Component_pin_map), DirectCast(compareProperty, IEnumerable(Of Component_pin_map)))
                    Dim componentPinMapComparisonMapper As New ComponentPinMapComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    componentPinMapComparisonMapper.CompareObjects()

                    If (componentPinMapComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(NameOf(E3.Lib.Schema.Kbl.Component_occurrence.Component_pin_maps), componentPinMapComparisonMapper)
                    End If

                Case NameOf(E3.Lib.Schema.Kbl.Component_occurrence.Installation_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Installation_instruction), DirectCast(compareProperty, IEnumerable(Of Installation_instruction)))
                    Dim installationInfoComparisonMapper As New InstallationInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    installationInfoComparisonMapper.CompareObjects()

                    If (installationInfoComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(objProperty.Name.ToString, installationInfoComparisonMapper)
                    End If

                Case NameOf(E3.Lib.Schema.Kbl.Component_occurrence.Description)
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Localized_string), DirectCast(compareProperty, IEnumerable(Of Localized_string)))
                    Dim locDescriptionComparer As New LocalizedDescriptionComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    locDescriptionComparer.CompareObjects()

                    If (locDescriptionComparer.HasChanges) Then
                        MyBase.ChangedProperties.Add(NameOf(E3.Lib.Schema.Kbl.Component_occurrence.Description), locDescriptionComparer)
                    End If

                Case Else
                    MyBase.ChangedProperties.Add(objProperty.Name, compareProperty)
            End Select
        End If
        Return False
    End Function
End Class
