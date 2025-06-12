Public Class ListConvertToDictionary

    Private _currentObjects As Dictionary(Of String, Object)
    Private _compareObjects As Dictionary(Of String, Object)

    Public Sub New(currentAliasIds As IEnumerable(Of Alias_identification), compareAliasIds As IEnumerable(Of Alias_identification))
        _currentObjects = New Dictionary(Of String, Object)
        _compareObjects = New Dictionary(Of String, Object)

        If (currentAliasIds IsNot Nothing) Then
            For Each aliasId As Alias_identification In currentAliasIds
                If (Not _currentObjects.ContainsKey(aliasId.Alias_id)) Then _currentObjects.Add(aliasId.Alias_id, aliasId)
            Next
        End If

        If (compareAliasIds IsNot Nothing) Then
            For Each aliasId As Alias_identification In compareAliasIds
                If (Not _compareObjects.ContainsKey(aliasId.Alias_id)) Then _compareObjects.Add(aliasId.Alias_id, aliasId)
            Next
        End If

    End Sub

    Public Sub New(currentProcessingInformations As IEnumerable(Of Processing_instruction), compareProcessingInformations As IEnumerable(Of Processing_instruction))
        _currentObjects = New Dictionary(Of String, Object)
        _compareObjects = New Dictionary(Of String, Object)

        If (currentProcessingInformations IsNot Nothing) Then
            For Each processingInstruction As Processing_instruction In currentProcessingInformations
                If (Not _currentObjects.ContainsKey(processingInstruction.Instruction_type)) Then _currentObjects.Add(processingInstruction.Instruction_type, processingInstruction)
            Next
        End If
        If (compareProcessingInformations IsNot Nothing) Then
            For Each processingInstruction As Processing_instruction In compareProcessingInformations
                If (Not _compareObjects.ContainsKey(processingInstruction.Instruction_type)) Then _compareObjects.Add(processingInstruction.Instruction_type, processingInstruction)
            Next
        End If

    End Sub

    Public Sub New(currentLocalizedStrings As IEnumerable(Of Localized_string), compareLocalizedStrings As IEnumerable(Of Localized_string))
        _currentObjects = New Dictionary(Of String, Object)
        _compareObjects = New Dictionary(Of String, Object)

        If (currentLocalizedStrings IsNot Nothing) Then
            For Each locString As Localized_string In currentLocalizedStrings
                If (Not _currentObjects.ContainsKey(locString.Language_code.ToString)) Then _currentObjects.Add(locString.Language_code.ToString, locString)
            Next
        End If
        If (compareLocalizedStrings IsNot Nothing) Then
            For Each locString As Localized_string In compareLocalizedStrings
                If (Not _compareObjects.ContainsKey(locString.Language_code.ToString)) Then _compareObjects.Add(locString.Language_code.ToString, locString)
            Next
        End If

    End Sub

    Public Sub New(currentChanges As IEnumerable(Of Change), compareChanges As IEnumerable(Of Change))
        _currentObjects = New Dictionary(Of String, Object)
        _compareObjects = New Dictionary(Of String, Object)

        If (currentChanges IsNot Nothing) Then
            For Each change As Change In currentChanges
                If (change.Id IsNot Nothing) AndAlso (Not _currentObjects.ContainsKey(change.Id)) Then _currentObjects.Add(change.Id, change)
            Next
        End If

        If (compareChanges IsNot Nothing) Then
            For Each change As Change In compareChanges
                If (change.Id IsNot Nothing) AndAlso (Not _compareObjects.ContainsKey(change.Id)) Then _compareObjects.Add(change.Id, change)
            Next
        End If
    End Sub

    Public Sub New(currentExternalReferences As IEnumerable(Of External_reference), compareExternalReferences As IEnumerable(Of External_reference))
        _currentObjects = New Dictionary(Of String, Object)
        _compareObjects = New Dictionary(Of String, Object)

        If (currentExternalReferences IsNot Nothing) Then
            For Each externalReference As External_reference In currentExternalReferences
                If (externalReference IsNot Nothing) Then
                    Dim extRefId As String = String.Format("{0}|{1}|{2}", externalReference.Document_type, externalReference.Document_number, externalReference.Change_level)

                    If (externalReference.File_name IsNot Nothing) Then extRefId = String.Format("{0}|{1}", extRefId, externalReference.File_name)
                    If (externalReference.Location IsNot Nothing) Then extRefId = String.Format("{0}|{1}", extRefId, externalReference.Location)

                    extRefId = String.Format("{0}|{1}", extRefId, externalReference.Data_format)

                    If (externalReference.Creating_system IsNot Nothing) Then extRefId = String.Format("{0}|{1}", extRefId, externalReference.Creating_system)

                    If (Not _currentObjects.ContainsKey(extRefId)) Then _currentObjects.Add(extRefId, externalReference)
                End If
            Next
        End If

        If (compareExternalReferences IsNot Nothing) Then
            For Each externalReference As External_reference In compareExternalReferences
                If (externalReference IsNot Nothing) Then
                    Dim extRefId As String = String.Format("{0}|{1}|{2}", externalReference.Document_type, externalReference.Document_number, externalReference.Change_level)

                    If (externalReference.File_name IsNot Nothing) Then extRefId = String.Format("{0}|{1}", extRefId, externalReference.File_name)
                    If (externalReference.Location IsNot Nothing) Then extRefId = String.Format("{0}|{1}", extRefId, externalReference.Location)

                    extRefId = String.Format("{0}|{1}", extRefId, externalReference.Data_format)

                    If (externalReference.Creating_system IsNot Nothing) Then extRefId = String.Format("{0}|{1}", extRefId, externalReference.Creating_system)

                    If (Not _compareObjects.ContainsKey(extRefId)) Then _compareObjects.Add(extRefId, externalReference)
                End If
            Next
        End If

    End Sub

    Public ReadOnly Property CurrentObjects() As Dictionary(Of String, Object)
        Get
            Return _currentObjects
        End Get
    End Property

    Public ReadOnly Property CompareObjects() As Dictionary(Of String, Object)
        Get
            Return _compareObjects
        End Get
    End Property


    'Public Sub New(currentCrossSectionAreas As ICollection(Of Cross_section_area), currentSegmentId As String, compareCrossSectionAreas As ICollection(Of Cross_section_area), compareSegmentId As String)
    '    _currentObjects = New Dictionary(Of String, Object)
    '    _compareObjects = New Dictionary(Of String, Object)

    '    Dim currentCSAIndex As Integer = 1
    '    Dim compareCSAIndex As Integer = 1
    '    'This is basically a nonsense- it would be better to use the determination entry as identifier

    '    If (currentCrossSectionAreas IsNot Nothing) Then
    '        For Each crossSectionArea As Cross_section_area In currentCrossSectionAreas
    '            _currentObjects.Add(String.Format("{0}_{1}", currentSegmentId, currentCSAIndex), crossSectionArea)

    '            currentCSAIndex += 1
    '        Next
    '    End If


    '    If (compareCrossSectionAreas IsNot Nothing) Then
    '        For Each crossSectionArea As Cross_section_area In compareCrossSectionAreas
    '            _compareObjects.Add(String.Format("{0}_{1}", compareSegmentId, compareCSAIndex), crossSectionArea)

    '            compareCSAIndex += 1
    '        Next
    '    End If

    'End Sub
    Public Sub New(currentCrossSectionAreas As IEnumerable(Of Cross_section_area), currentSegmentId As String, compareCrossSectionAreas As IEnumerable(Of Cross_section_area), compareSegmentId As String)
        _currentObjects = New Dictionary(Of String, Object)
        _compareObjects = New Dictionary(Of String, Object)

        If (currentCrossSectionAreas IsNot Nothing) Then
            For Each crossSectionArea As Cross_section_area In currentCrossSectionAreas
                If (Not _currentObjects.ContainsKey(CInt(crossSectionArea.Value_determination).ToString)) Then
                    _currentObjects.Add(CInt(crossSectionArea.Value_determination).ToString, crossSectionArea)
                End If
            Next
        End If


        If (compareCrossSectionAreas IsNot Nothing) Then
            For Each crossSectionArea As Cross_section_area In compareCrossSectionAreas
                If (Not _compareObjects.ContainsKey(CInt(crossSectionArea.Value_determination).ToString)) Then
                    _compareObjects.Add(CInt(crossSectionArea.Value_determination).ToString, crossSectionArea)
                End If
            Next
        End If

    End Sub

    Public Sub New(currentFixingAssignments As IEnumerable(Of Fixing_assignment), compareFixingAssignments As IEnumerable(Of Fixing_assignment))
        _currentObjects = New Dictionary(Of String, Object)
        _compareObjects = New Dictionary(Of String, Object)

        If (currentFixingAssignments IsNot Nothing) Then
            For Each fixingAssignment As Fixing_assignment In currentFixingAssignments
                Dim id As String = If(fixingAssignment.Id, fixingAssignment.Fixing) 'HINT: Serializer is no longer filling in the id (sometimes?) but there is also no id in the kbl, in old trunk this id filled with the same value as the .Fixing-Property
                If (Not _currentObjects.ContainsKey(id)) Then
                    _currentObjects.Add(id, fixingAssignment)
                End If
            Next
        End If

        If (compareFixingAssignments IsNot Nothing) Then
            For Each fixingAssignment As Fixing_assignment In compareFixingAssignments
                Dim id As String = If(fixingAssignment.Id, fixingAssignment.Fixing) 'HINT: Serializer is no longer filling in the id (sometimes?) but there is also no id in the kbl, in old trunk this id filled with the same value as the .Fixing-Property
                If Not _compareObjects.ContainsKey(id) Then
                    _compareObjects.Add(id, fixingAssignment)
                End If
            Next
        End If

    End Sub

    Public Sub New(currentKblMapper As KblMapper, currentProtectionAreas As IEnumerable(Of Protection_area), compareKblMapper As KblMapper, compareProtectionAreas As IEnumerable(Of Protection_area))
        _currentObjects = New Dictionary(Of String, Object)
        _compareObjects = New Dictionary(Of String, Object)

        If (currentProtectionAreas IsNot Nothing) Then
            For Each protectionArea As Protection_area In currentProtectionAreas
                Dim key As String = currentKblMapper.GetHarness.GetWireProtectionOccurrence(protectionArea.Associated_protection).Id
                If (Not _currentObjects.ContainsKey(key)) Then
                    _currentObjects.Add(key, protectionArea)
                End If
            Next
        End If

        If (compareProtectionAreas IsNot Nothing) Then
            For Each protectionArea As Protection_area In compareProtectionAreas
                Dim key As String = compareKblMapper.GetHarness.GetWireProtectionOccurrence(protectionArea.Associated_protection).Id
                If (Not _compareObjects.ContainsKey(key)) Then
                    _compareObjects.Add(key, protectionArea)
                End If
            Next
        End If
    End Sub

    Public Sub New(currentInstallationInstructions As IEnumerable(Of Installation_instruction), compareInstallationInstructions As IEnumerable(Of Installation_instruction))
        _currentObjects = New Dictionary(Of String, Object)
        _compareObjects = New Dictionary(Of String, Object)

        If (currentInstallationInstructions IsNot Nothing) Then
            For Each installationInstruction As Installation_instruction In currentInstallationInstructions
                If (Not _currentObjects.ContainsKey(installationInstruction.Instruction_type)) Then
                    _currentObjects.Add(installationInstruction.Instruction_type, installationInstruction)
                End If
            Next
        End If

        If (compareInstallationInstructions IsNot Nothing) Then
            For Each installationInstruction As Installation_instruction In compareInstallationInstructions
                If (Not _compareObjects.ContainsKey(installationInstruction.Instruction_type)) Then _compareObjects.Add(installationInstruction.Instruction_type, installationInstruction)
            Next
        End If

    End Sub

    Public Sub New(currentConnections As IEnumerable(Of [Lib].Schema.Kbl.Connection), compareConnections As IEnumerable(Of [Lib].Schema.Kbl.Connection))
        _currentObjects = New Dictionary(Of String, Object)
        _compareObjects = New Dictionary(Of String, Object)
        If (currentConnections IsNot Nothing) Then
            For Each currentConnection As [Lib].Schema.Kbl.Connection In currentConnections
                If (currentConnection.Id IsNot Nothing) AndAlso (Not _currentObjects.ContainsKey(currentConnection.Id)) Then
                    _currentObjects.Add(currentConnection.Id, currentConnection)
                End If
            Next
        End If

        If (compareConnections IsNot Nothing) Then
            For Each compareConnection As [Lib].Schema.Kbl.Connection In compareConnections
                If (compareConnection.Id IsNot Nothing) AndAlso (Not _compareObjects.ContainsKey(compareConnection.Id)) Then
                    _compareObjects.Add(compareConnection.Id, compareConnection)
                End If
            Next
        End If
    End Sub

    Public Sub New(currentWireLengths As IEnumerable(Of Wire_length), compareWireLengths As IEnumerable(Of Wire_length))
        _currentObjects = New Dictionary(Of String, Object)
        _compareObjects = New Dictionary(Of String, Object)

        If (currentWireLengths IsNot Nothing) Then
            For Each wireLength As Wire_length In currentWireLengths
                If (Not _currentObjects.ContainsKey(wireLength.Length_type.ToLower)) Then
                    _currentObjects.Add(wireLength.Length_type.ToLower, wireLength)
                End If
            Next
        End If

        If (compareWireLengths IsNot Nothing) Then
            For Each wireLength As Wire_length In compareWireLengths
                If (Not _compareObjects.ContainsKey(wireLength.Length_type.ToLower)) Then
                    _compareObjects.Add(wireLength.Length_type.ToLower, wireLength)
                End If
            Next
        End If
    End Sub

    Public Sub New(currentCoreOccurrences As IEnumerable(Of Core_occurrence), compareCoreOccurrences As IEnumerable(Of Core_occurrence))
        _currentObjects = New Dictionary(Of String, Object)
        _compareObjects = New Dictionary(Of String, Object)

        If (currentCoreOccurrences IsNot Nothing) Then
            For Each coreOccurrence As Core_occurrence In currentCoreOccurrences
                If (Not _currentObjects.ContainsKey(coreOccurrence.Wire_number)) Then _currentObjects.Add(coreOccurrence.Wire_number, coreOccurrence)
            Next
        End If

        If (compareCoreOccurrences IsNot Nothing) Then
            For Each coreOccurrence As Core_occurrence In compareCoreOccurrences
                If (Not _compareObjects.ContainsKey(coreOccurrence.Wire_number)) Then _compareObjects.Add(coreOccurrence.Wire_number, coreOccurrence)
            Next
        End If

    End Sub

    Public Sub New(currentCavityOccurrences As IDictionary(Of String, Cavity_occurrence), compareCavityOccurrences As IDictionary(Of String, Cavity_occurrence))
        _currentObjects = New Dictionary(Of String, Object)
        _compareObjects = New Dictionary(Of String, Object)

        If (currentCavityOccurrences IsNot Nothing) Then
            For Each dictionaryEntry As KeyValuePair(Of String, Cavity_occurrence) In currentCavityOccurrences
                _currentObjects.Add(dictionaryEntry.Key, dictionaryEntry.Value)
            Next
        End If

        If (compareCavityOccurrences IsNot Nothing) Then
            For Each dictionaryEntry As KeyValuePair(Of String, Cavity_occurrence) In compareCavityOccurrences
                _compareObjects.Add(dictionaryEntry.Key, dictionaryEntry.Value)
            Next
        End If
    End Sub

    Public Sub New(currentContactPoints As IDictionary(Of String, Contact_point), compareContactPoints As IDictionary(Of String, Contact_point))
        _currentObjects = New Dictionary(Of String, Object)
        _compareObjects = New Dictionary(Of String, Object)

        If (currentContactPoints IsNot Nothing) Then
            For Each dictionaryEntry As KeyValuePair(Of String, Contact_point) In currentContactPoints
                _currentObjects.Add(dictionaryEntry.Key, dictionaryEntry.Value)
            Next
        End If

        If (compareContactPoints IsNot Nothing) Then
            For Each dictionaryEntry As KeyValuePair(Of String, Contact_point) In compareContactPoints
                _compareObjects.Add(dictionaryEntry.Key, dictionaryEntry.Value)
            Next
        End If

    End Sub

    Public Sub New(currentRouting As IDictionary(Of String, String), compareRouting As IDictionary(Of String, String))
        _currentObjects = New Dictionary(Of String, Object)
        _compareObjects = New Dictionary(Of String, Object)

        If (currentRouting IsNot Nothing) Then
            For Each dictionaryEntry As KeyValuePair(Of String, String) In currentRouting
                _currentObjects.Add(dictionaryEntry.Key, dictionaryEntry.Value)
            Next
        End If

        If (compareRouting IsNot Nothing) Then
            For Each dictionaryEntry As KeyValuePair(Of String, String) In compareRouting
                _compareObjects.Add(dictionaryEntry.Key, dictionaryEntry.Value)
            Next
        End If

    End Sub

    Public Sub New(currentAssignedWires As String, currentKBLMapper As KBLMapper, compareAssignedWires As String, compareKBLMapper As KBLMapper)
        'HINT basically shit as too generic and not type safe!
        _currentObjects = New Dictionary(Of String, Object)
        _compareObjects = New Dictionary(Of String, Object)

        If (currentAssignedWires IsNot Nothing) Then
            For Each wireCoreId As String In currentAssignedWires.SplitSpace
                Dim wireCoreOcc As IKblWireCoreOccurrence = currentKBLMapper.GetWireOrCoreOccurrence(wireCoreId)
                If wireCoreOcc IsNot Nothing Then
                    _currentObjects.Add(wireCoreOcc.Wire_number, wireCoreOcc.Wire_number)
                End If
            Next
        End If

        If (compareAssignedWires IsNot Nothing) Then
            For Each wireCoreId As String In compareAssignedWires.SplitSpace
                Dim wireCoreOcc As IKblWireCoreOccurrence = compareKBLMapper.GetWireOrCoreOccurrence(wireCoreId)
                If wireCoreOcc IsNot Nothing Then
                    _currentObjects.Add(wireCoreOcc.Wire_number, wireCoreOcc.Wire_number)
                End If
            Next
        End If

    End Sub

    Public Sub New(currentWireProtectionOccurrences As IEnumerable(Of Wire_protection_occurrence), compareWireProtectionOccurrences As IEnumerable(Of Wire_protection_occurrence))
        _currentObjects = New Dictionary(Of String, Object)
        _compareObjects = New Dictionary(Of String, Object)

        If (currentWireProtectionOccurrences IsNot Nothing) Then
            For Each wireProtectionOccurrence As Wire_protection_occurrence In currentWireProtectionOccurrences
                _currentObjects.TryAdd(wireProtectionOccurrence.SystemId, wireProtectionOccurrence)
            Next
        End If

        If (compareWireProtectionOccurrences IsNot Nothing) Then
            For Each wireProtectionOccurrence As Wire_protection_occurrence In compareWireProtectionOccurrences
                _compareObjects.TryAdd(wireProtectionOccurrence.SystemId, wireProtectionOccurrence)
            Next
        End If

    End Sub

    Public Sub New(currentComponentPinMaps As IEnumerable(Of Component_pin_map), compareComponentPinMap As IEnumerable(Of Component_pin_map))
        _currentObjects = New Dictionary(Of String, Object)
        _compareObjects = New Dictionary(Of String, Object)

        If (currentComponentPinMaps IsNot Nothing) Then
            For Each componentPinMap As Component_pin_map In currentComponentPinMaps
                If (Not _currentObjects.ContainsKey(String.Format("{0}|{1}", componentPinMap.Component_pin_number, componentPinMap.Cavity_number))) Then
                    _currentObjects.Add(String.Format("{0}|{1}", componentPinMap.Component_pin_number, componentPinMap.Cavity_number), componentPinMap)
                End If
            Next
        End If

        If (compareComponentPinMap IsNot Nothing) Then
            For Each componentPinMap As Component_pin_map In compareComponentPinMap
                If (Not _compareObjects.ContainsKey(String.Format("{0}|{1}", componentPinMap.Component_pin_number, componentPinMap.Cavity_number))) Then
                    _compareObjects.Add(String.Format("{0}|{1}", componentPinMap.Component_pin_number, componentPinMap.Cavity_number), componentPinMap)
                End If
            Next
        End If

    End Sub
End Class