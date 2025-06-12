Imports System.Reflection
Imports System.Text
Imports System.Xml.Serialization
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Public Class SearchMachine

    Friend Event SearchEvent(sender As Object, mapperSourceId As String, objectType As String, objectId As String)

    Private _mainForm As MainForm
    Private _gridAppearances As Dictionary(Of KblObjectType, GridAppearance)
    Private _inactiveObjects As Dictionary(Of String, ITypeGroupedKblIds)
    Private _searchPatterns As Dictionary(Of String, SearchPattern)

    Private WithEvents _searchForm As SearchForm

    Public Sub New(mainForm As MainForm)
        _inactiveObjects = New Dictionary(Of String, ITypeGroupedKblIds)
        _mainForm = mainForm
        _searchPatterns = New Dictionary(Of String, SearchPattern)

        Initialize()
    End Sub

    Private Sub _searchForm_closeSearchForm()
        RemoveSearchFormClickEvent()
    End Sub

    Private Sub CreateSearchForm()
        If (_searchForm Is Nothing) OrElse (_searchForm.IsDisposed) Then
            _searchForm = New SearchForm(_gridAppearances, _inactiveObjects, _MainForm.GeneralSettings.TouchEnabledUI, _searchPatterns)

            AddSearchFormClickEvent()
        End If

        If (Not _searchForm.Visible) Then
            _searchForm.Show(_MainForm)
        End If

        _searchForm.BringToFront()
    End Sub

    Private Sub AddSearchFormClickEvent()
        AddHandler _searchForm.CloseSearchForm, AddressOf _searchForm_closeSearchForm
        AddHandler _searchForm.SearchObject, AddressOf _searchForm_searchEvent
    End Sub

    Private Sub RemoveSearchFormClickEvent()
        RemoveHandler _searchForm.CloseSearchForm, AddressOf _searchForm_closeSearchForm
        RemoveHandler _searchForm.SearchObject, AddressOf _searchForm_searchEvent
    End Sub

    Private Sub Initialize()
        _gridAppearances = New Dictionary(Of KblObjectType, GridAppearance)

        Dim accessoryGridAppearance As AccessoryGridAppearance = GridAppearance.All.OfType(Of AccessoryGridAppearance).Single
        Dim approvalGridAppearance As ApprovalGridAppearance = GridAppearance.All.OfType(Of ApprovalGridAppearance).Single
        Dim assemblyPartGridAppearance As AssemblyPartGridAppearance = GridAppearance.All.OfType(Of AssemblyPartGridAppearance).Single
        Dim cableGridAppearance As CableGridAppearance = GridAppearance.All.OfType(Of CableGridAppearance).Single
        Dim changeDescriptionGridAppearance As ChangeDescriptionGridAppearance = GridAppearance.All.OfType(Of ChangeDescriptionGridAppearance).Single
        Dim componentBoxGridAppearance As ComponentBoxGridAppearance = GridAppearance.All.OfType(Of ComponentBoxGridAppearance).Single
        Dim componentGridAppearance As ComponentGridAppearance = GridAppearance.All.OfType(Of ComponentGridAppearance).Single
        Dim connectorGridAppearance As ConnectorGridAppearance = GridAppearance.All.OfType(Of ConnectorGridAppearance).Single
        Dim coPackGridAppearance As CoPackGridAppearance = GridAppearance.All.OfType(Of CoPackGridAppearance).Single
        Dim defDimSpecGridAppearance As DefaultDimSpecGridAppearance = GridAppearance.All.OfType(Of DefaultDimSpecGridAppearance).Single
        Dim dimSpecGridAppearance As DimSpecGridAppearance = GridAppearance.All.OfType(Of DimSpecGridAppearance).Single
        Dim fixingGridAppearance As FixingGridAppearance = GridAppearance.All.OfType(Of FixingGridAppearance).Single
        Dim moduleGridAppearance As ModuleGridAppearance = GridAppearance.All.OfType(Of ModuleGridAppearance).Single
        Dim netGridAppearance As NetGridAppearance = GridAppearance.All.OfType(Of NetGridAppearance).Single
        Dim segmentGridAppearance As SegmentGridAppearance = GridAppearance.All.OfType(Of SegmentGridAppearance).Single
        Dim vertexGridAppearance As VertexGridAppearance = GridAppearance.All.OfType(Of VertexGridAppearance).Single
        Dim wireGridAppearance As WireGridAppearance = GridAppearance.All.OfType(Of WireGridAppearance).Single

        _gridAppearances.TryAddIfNotEmpty((accessoryGridAppearance.GridTable?.Type).GetValueOrDefault, accessoryGridAppearance)
        _gridAppearances.TryAddIfNotEmpty((approvalGridAppearance.GridTable?.Type).GetValueOrDefault, approvalGridAppearance)
        _gridAppearances.TryAddIfNotEmpty((assemblyPartGridAppearance.GridTable?.Type).GetValueOrDefault, assemblyPartGridAppearance)
        _gridAppearances.TryAddIfNotEmpty((cableGridAppearance.GridTable?.Type).GetValueOrDefault, cableGridAppearance)
        _gridAppearances.TryAddIfNotEmpty((changeDescriptionGridAppearance.GridTable?.Type).GetValueOrDefault, changeDescriptionGridAppearance)
        _gridAppearances.TryAddIfNotEmpty((componentBoxGridAppearance.GridTable?.Type).GetValueOrDefault, componentBoxGridAppearance)
        _gridAppearances.TryAddIfNotEmpty((componentGridAppearance.GridTable?.Type).GetValueOrDefault, componentGridAppearance)
        _gridAppearances.TryAddIfNotEmpty((connectorGridAppearance.GridTable?.Type).GetValueOrDefault, connectorGridAppearance)
        _gridAppearances.TryAddIfNotEmpty((coPackGridAppearance.GridTable?.Type).GetValueOrDefault, coPackGridAppearance)
        _gridAppearances.TryAddIfNotEmpty((defDimSpecGridAppearance.GridTable?.Type).GetValueOrDefault, defDimSpecGridAppearance)
        _gridAppearances.TryAddIfNotEmpty((dimSpecGridAppearance.GridTable?.Type).GetValueOrDefault, dimSpecGridAppearance)
        _gridAppearances.TryAddIfNotEmpty((fixingGridAppearance.GridTable?.Type).GetValueOrDefault, fixingGridAppearance)
        _gridAppearances.TryAddIfNotEmpty((moduleGridAppearance.GridTable?.Type).GetValueOrDefault, moduleGridAppearance)
        _gridAppearances.TryAddIfNotEmpty((netGridAppearance.GridTable?.Type).GetValueOrDefault, netGridAppearance)
        _gridAppearances.TryAddIfNotEmpty((segmentGridAppearance.GridTable?.Type).GetValueOrDefault, segmentGridAppearance)
        _gridAppearances.TryAddIfNotEmpty((vertexGridAppearance.GridTable?.Type).GetValueOrDefault, vertexGridAppearance)
        _gridAppearances.TryAddIfNotEmpty((wireGridAppearance.GridTable?.Type).GetValueOrDefault, wireGridAppearance)
    End Sub

    Private Sub LoadAccessoriesIntoSearchMapper(kblMapper As KblMapper, objectSearchPatterns As Dictionary(Of String, ObjectSearchPattern), searchableGridColumns As IEnumerable(Of String))
        For Each accessoryOccurrence As Accessory_occurrence In kblMapper.GetAccessoryOccurrences
            Dim propertySearchPattern As New StringBuilder

            For Each accessoryOccurrenceProperty As PropertyInfo In accessoryOccurrence.GetType.GetProperties
                If (accessoryOccurrenceProperty.Name = AccessoryPropertyName.Alias_id.ToString) AndAlso (searchableGridColumns.Contains(AccessoryPropertyName.Alias_id.ToString)) Then
                    LoadAliasIdsIntoSearchPattern(DirectCast(accessoryOccurrenceProperty.GetValue(accessoryOccurrence), IEnumerable(Of Alias_identification)), propertySearchPattern)
                ElseIf (accessoryOccurrenceProperty.Name = AccessoryPropertyName.Installation_Information.ToString) AndAlso (searchableGridColumns.Contains(AccessoryPropertyName.Installation_Information.ToString)) Then
                    LoadInstallationInformationIntoSearchPattern(DirectCast(accessoryOccurrenceProperty.GetValue(accessoryOccurrence), IEnumerable(Of Installation_instruction)), propertySearchPattern)
                ElseIf (accessoryOccurrenceProperty.Name = AccessoryPropertyName.Part.ToString) AndAlso (kblMapper.KBLPartMapper.ContainsKey(accessoryOccurrenceProperty.GetValue(accessoryOccurrence).ToString)) Then
                    Dim accessory As Accessory = DirectCast(kblMapper.KBLPartMapper(accessoryOccurrenceProperty.GetValue(accessoryOccurrence).ToString), Accessory)

                    For Each accessoryProperty As PropertyInfo In accessory.GetType.GetProperties
                        If (accessoryProperty.Name = AccessoryPropertyName.Alias_id.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Part_alias_ids.ToString)) Then
                            LoadAliasIdsIntoSearchPattern(DirectCast(accessoryProperty.GetValue(accessory), IEnumerable(Of Alias_identification)), propertySearchPattern)
                        ElseIf (accessoryProperty.Name = PartPropertyName.Change.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Change.ToString)) Then
                            LoadChangesIntoSearchPattern(DirectCast(accessoryProperty.GetValue(accessory), IEnumerable(Of Change)), propertySearchPattern)
                        ElseIf (accessoryProperty.Name = PartPropertyName.Mass_information.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Mass_information.ToString)) Then
                            LoadNumericalValueIntoSearchPattern(DirectCast(accessoryProperty.GetValue(accessory), Numerical_value), propertySearchPattern)
                        ElseIf (accessoryProperty.Name = PartPropertyName.Material_information.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Material_information.ToString)) Then
                            LoadMaterialInformationIntoSearchPattern(DirectCast(accessoryProperty.GetValue(accessory), Material), propertySearchPattern)
                        ElseIf (accessoryProperty.Name = CommonPropertyName.Processing_Information) AndAlso (searchableGridColumns.Contains(PartPropertyName.Part_processing_information.ToString)) Then
                            LoadProcessingInformationIntoSearchPattern(DirectCast(accessoryProperty.GetValue(accessory), IEnumerable(Of Processing_instruction)), propertySearchPattern)
                        ElseIf (searchableGridColumns.Contains(accessoryProperty.Name)) OrElse (accessoryProperty.Name = AccessoryPropertyName.Description.ToString AndAlso searchableGridColumns.Contains(PartPropertyName.Part_description.ToString)) Then
                            propertySearchPattern.Append(String.Format("{0}|", accessoryProperty.GetValue(accessory)))
                        End If
                    Next
                ElseIf (searchableGridColumns.Contains(accessoryOccurrenceProperty.Name)) Then
                    propertySearchPattern.Append(String.Format("{0}|", accessoryOccurrenceProperty.GetValue(accessoryOccurrence)))
                End If
            Next

            If (propertySearchPattern.Length > 0) Then
                propertySearchPattern.Remove(propertySearchPattern.Length - 1, 1)
            End If

            Dim objectSearchPattern As New ObjectSearchPattern

            If (accessoryOccurrence.Description IsNot Nothing) AndAlso (accessoryOccurrence.Description <> String.Empty) Then
                objectSearchPattern.DisplayText = String.Format("{0} [{1}]", accessoryOccurrence.Id, accessoryOccurrence.Description)
            Else
                objectSearchPattern.DisplayText = accessoryOccurrence.Id
            End If

            objectSearchPattern.PropertyValuePattern = propertySearchPattern.ToString
            objectSearchPatterns.Add(accessoryOccurrence.SystemId, objectSearchPattern)
        Next
    End Sub

    Private Sub LoadAliasIdsIntoSearchPattern(aliasIds As IEnumerable(Of Alias_identification), propertySearchPattern As StringBuilder)
        For Each aliasId As Alias_identification In aliasIds
            For Each aliasIdProperty As PropertyInfo In aliasId.GetType.GetProperties
                If (aliasIdProperty.Name <> Zuken.E3.HarnessAnalyzer.Shared.SYSTEM_ID) Then
                    propertySearchPattern.Append(String.Format("{0}|", aliasIdProperty.GetValue(aliasId)))
                End If
            Next
        Next
    End Sub

    Private Sub LoadApprovalsIntoSearchMapper(kblMapper As KblMapper, objectSearchPatterns As Dictionary(Of String, ObjectSearchPattern), searchableGridColumns As IEnumerable(Of String))
        For Each approval As Approval In kblMapper.GetApprovals
            Dim propertySearchPattern As New StringBuilder

            For Each approvalProperty As PropertyInfo In approval.GetType.GetProperties
                If (approvalProperty.Name = ApprovalPropertyName.Is_applied_to.ToString) AndAlso (approvalProperty.GetValue(approval) IsNot Nothing) Then
                    If (kblMapper.KBLPartMapper.ContainsKey(approvalProperty.GetValue(approval).ToString)) Then propertySearchPattern.Append(String.Format("{0}|", DirectCast(kblMapper.KBLPartMapper(approvalProperty.GetValue(approval).ToString), Part_with_title_block).Part_number))
                ElseIf (searchableGridColumns.Contains(approvalProperty.Name)) Then
                    propertySearchPattern.Append(String.Format("{0}|", approvalProperty.GetValue(approval)))
                End If
            Next

            If (propertySearchPattern.Length > 0) Then propertySearchPattern.Remove(propertySearchPattern.Length - 1, 1)

            Dim objectSearchPattern As New ObjectSearchPattern

            If (approval.Name IsNot Nothing) AndAlso (approval.Name <> String.Empty) Then
                objectSearchPattern.DisplayText = String.Format("{0} [{1}]", approval.Name, approval.Type_of_approval)
            Else
                objectSearchPattern.DisplayText = approval.Type_of_approval
            End If

            objectSearchPattern.PropertyValuePattern = propertySearchPattern.ToString
            objectSearchPatterns.Add(approval.SystemId, objectSearchPattern)
        Next
    End Sub

    Private Sub LoadAssemblyPartsIntoSearchMapper(kblMapper As KblMapper, objectSearchPatterns As Dictionary(Of String, ObjectSearchPattern), searchableGridColumns As IEnumerable(Of String))
        For Each assemblyPartOccurrence As Assembly_part_occurrence In kblMapper.GetHarness.Assembly_part_occurrence
            Dim propertySearchPattern As New StringBuilder

            For Each assemblyPartOccurrenceProperty As PropertyInfo In assemblyPartOccurrence.GetType.GetProperties
                If (assemblyPartOccurrenceProperty.Name = AccessoryPropertyName.Alias_id.ToString) AndAlso (searchableGridColumns.Contains(AssemblyPartPropertyName.Alias_id.ToString)) Then
                    LoadAliasIdsIntoSearchPattern(DirectCast(assemblyPartOccurrenceProperty.GetValue(assemblyPartOccurrence), IEnumerable(Of Alias_identification)), propertySearchPattern)
                ElseIf (assemblyPartOccurrenceProperty.Name = AccessoryPropertyName.Installation_Information.ToString) AndAlso (searchableGridColumns.Contains(AssemblyPartPropertyName.Installation_Information.ToString)) Then
                    LoadInstallationInformationIntoSearchPattern(DirectCast(assemblyPartOccurrenceProperty.GetValue(assemblyPartOccurrence), IEnumerable(Of Installation_instruction)), propertySearchPattern)
                ElseIf (assemblyPartOccurrenceProperty.Name = AssemblyPartPropertyName.Part.ToString) AndAlso (kblMapper.KBLPartMapper.ContainsKey(assemblyPartOccurrenceProperty.GetValue(assemblyPartOccurrence).ToString)) Then
                    Dim assemblyPart As Assembly_part = DirectCast(kblMapper.KBLPartMapper(assemblyPartOccurrenceProperty.GetValue(assemblyPartOccurrence).ToString), Assembly_part)

                    For Each assemblyPartProperty As PropertyInfo In assemblyPart.GetType.GetProperties
                        If (assemblyPartProperty.Name = AssemblyPartPropertyName.Alias_id.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Part_alias_ids.ToString)) Then
                            LoadAliasIdsIntoSearchPattern(DirectCast(assemblyPartProperty.GetValue(assemblyPart), IEnumerable(Of Alias_identification)), propertySearchPattern)
                        ElseIf (assemblyPartProperty.Name = PartPropertyName.Change.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Change.ToString)) Then
                            LoadChangesIntoSearchPattern(DirectCast(assemblyPartProperty.GetValue(assemblyPart), IEnumerable(Of Change)), propertySearchPattern)
                        ElseIf (assemblyPartProperty.Name = PartPropertyName.Mass_information.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Mass_information.ToString)) Then
                            LoadNumericalValueIntoSearchPattern(DirectCast(assemblyPartProperty.GetValue(assemblyPart), Numerical_value), propertySearchPattern)
                        ElseIf (assemblyPartProperty.Name = PartPropertyName.Material_information.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Material_information.ToString)) Then
                            LoadMaterialInformationIntoSearchPattern(DirectCast(assemblyPartProperty.GetValue(assemblyPart), Material), propertySearchPattern)
                        ElseIf (assemblyPartProperty.Name = CommonPropertyName.Processing_Information.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Part_processing_information.ToString)) Then
                            LoadProcessingInformationIntoSearchPattern(DirectCast(assemblyPartProperty.GetValue(assemblyPart), IEnumerable(Of Processing_instruction)), propertySearchPattern)
                        ElseIf (searchableGridColumns.Contains(assemblyPartProperty.Name)) OrElse (assemblyPartProperty.Name = AssemblyPartPropertyName.Description.ToString AndAlso searchableGridColumns.Contains(PartPropertyName.Part_description.ToString)) Then
                            propertySearchPattern.Append(String.Format("{0}|", assemblyPartProperty.GetValue(assemblyPart)))
                        End If
                    Next
                ElseIf (searchableGridColumns.Contains(assemblyPartOccurrenceProperty.Name)) Then
                    propertySearchPattern.Append(String.Format("{0}|", assemblyPartOccurrenceProperty.GetValue(assemblyPartOccurrence)))
                End If
            Next

            If (propertySearchPattern.Length > 0) Then propertySearchPattern.Remove(propertySearchPattern.Length - 1, 1)

            Dim objectSearchPattern As New ObjectSearchPattern

            If (assemblyPartOccurrence.Description IsNot Nothing) AndAlso (assemblyPartOccurrence.Description <> String.Empty) Then
                objectSearchPattern.DisplayText = String.Format("{0} [{1}]", assemblyPartOccurrence.Id, assemblyPartOccurrence.Description)
            Else
                objectSearchPattern.DisplayText = assemblyPartOccurrence.Id
            End If

            objectSearchPattern.PropertyValuePattern = propertySearchPattern.ToString
            objectSearchPatterns.Add(assemblyPartOccurrence.SystemId, objectSearchPattern)
        Next
    End Sub

    Private Sub LoadCablesIntoSearchMapper(kblMapper As KblMapper, objectSearchPatterns As Dictionary(Of String, ObjectSearchPattern), searchableGridColumns As IEnumerable(Of String))
        For Each cableOccurrence As Special_wire_occurrence In kblMapper.KBLCableList
            Dim propertySearchPattern As New StringBuilder

            For Each cableOccurrenceProperty As PropertyInfo In cableOccurrence.GetType.GetProperties
                If (cableOccurrenceProperty.Name = CablePropertyName.Length_Information.ToString) AndAlso (searchableGridColumns.Contains(CablePropertyName.Length_Information.ToString)) Then
                    LoadLengthInformationIntoSearchPattern(DirectCast(cableOccurrenceProperty.GetValue(cableOccurrence), IEnumerable(Of Wire_length)), propertySearchPattern)
                ElseIf (cableOccurrenceProperty.Name = CablePropertyName.Installation_Information.ToString) AndAlso (searchableGridColumns.Contains(CablePropertyName.Installation_Information.ToString)) Then
                    LoadInstallationInformationIntoSearchPattern(DirectCast(cableOccurrenceProperty.GetValue(cableOccurrence), IEnumerable(Of Installation_instruction)), propertySearchPattern)
                ElseIf (cableOccurrenceProperty.Name = CablePropertyName.Part.ToString) AndAlso (kblMapper.KBLPartMapper.ContainsKey(cableOccurrenceProperty.GetValue(cableOccurrence).ToString)) Then
                    Dim generalWire As General_wire = DirectCast(kblMapper.KBLPartMapper(cableOccurrenceProperty.GetValue(cableOccurrence).ToString), General_wire)

                    For Each generalWireProperty As PropertyInfo In generalWire.GetType.GetProperties
                        If (generalWireProperty.Name = CommonPropertyName.Alias_Id.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Part_alias_ids.ToString)) Then
                            LoadAliasIdsIntoSearchPattern(DirectCast(generalWireProperty.GetValue(generalWire), IEnumerable(Of Alias_identification)), propertySearchPattern)
                        ElseIf (generalWireProperty.Name = CablePropertyName.Bend_radius.ToString) AndAlso (searchableGridColumns.Contains(CablePropertyName.Bend_radius.ToString)) Then
                            LoadNumericalValueIntoSearchPattern(DirectCast(generalWireProperty.GetValue(generalWire), Numerical_value), propertySearchPattern)
                        ElseIf (generalWireProperty.Name = PartPropertyName.Change.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Change.ToString)) Then
                            LoadChangesIntoSearchPattern(DirectCast(generalWireProperty.GetValue(generalWire), IEnumerable(Of Change)), propertySearchPattern)
                        ElseIf (generalWireProperty.Name = CablePropertyName.CoverColours.ToString) AndAlso (searchableGridColumns.Contains(CablePropertyName.CoverColours.ToString)) Then
                            LoadWireColoursIntoSearchPattern(DirectCast(generalWireProperty.GetValue(generalWire), IEnumerable(Of Wire_colour)), propertySearchPattern)
                        ElseIf (generalWireProperty.Name = CablePropertyName.Cross_section_area.ToString) AndAlso (searchableGridColumns.Contains(CablePropertyName.Cross_section_area.ToString)) Then
                            LoadNumericalValueIntoSearchPattern(DirectCast(generalWireProperty.GetValue(generalWire), Numerical_value), propertySearchPattern)
                        ElseIf (generalWireProperty.Name = PartPropertyName.Mass_information.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Mass_information.ToString)) Then
                            LoadNumericalValueIntoSearchPattern(DirectCast(generalWireProperty.GetValue(generalWire), Numerical_value), propertySearchPattern)
                        ElseIf (generalWireProperty.Name = PartPropertyName.Material_information.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Material_information.ToString)) Then
                            LoadMaterialInformationIntoSearchPattern(DirectCast(generalWireProperty.GetValue(generalWire), Material), propertySearchPattern)
                        ElseIf (generalWireProperty.Name = CablePropertyName.Outside_diameter.ToString) AndAlso (searchableGridColumns.Contains(CablePropertyName.Outside_diameter.ToString)) Then
                            LoadNumericalValueIntoSearchPattern(DirectCast(generalWireProperty.GetValue(generalWire), Numerical_value), propertySearchPattern)
                        ElseIf (generalWireProperty.Name = CommonPropertyName.Processing_Information.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Part_processing_information.ToString)) Then
                            LoadProcessingInformationIntoSearchPattern(DirectCast(generalWireProperty.GetValue(generalWire), IEnumerable(Of Processing_instruction)), propertySearchPattern)
                        ElseIf (searchableGridColumns.Contains(generalWireProperty.Name)) OrElse (generalWireProperty.Name = CommonPropertyName.Description.ToString AndAlso searchableGridColumns.Contains(PartPropertyName.Part_description.ToString)) Then
                            propertySearchPattern.Append(String.Format("{0}|", generalWireProperty.GetValue(generalWire)))
                        End If
                    Next
                ElseIf (searchableGridColumns.Contains(cableOccurrenceProperty.Name)) Then
                    propertySearchPattern.Append(String.Format("{0}|", cableOccurrenceProperty.GetValue(cableOccurrence)))
                End If
            Next

            If (propertySearchPattern.Length > 0) Then propertySearchPattern.Remove(propertySearchPattern.Length - 1, 1)

            Dim objectSearchPattern As New ObjectSearchPattern

            If (cableOccurrence.Part IsNot Nothing) AndAlso (kblMapper.KBLPartMapper.ContainsKey(cableOccurrence.Part)) Then
                objectSearchPattern.DisplayText = String.Format("{0} [{1}]", cableOccurrence.Special_wire_id, DirectCast(kblMapper.KBLPartMapper(cableOccurrence.Part), General_wire).Part_number)
            Else
                objectSearchPattern.DisplayText = cableOccurrence.Special_wire_id
            End If

            objectSearchPattern.PropertyValuePattern = propertySearchPattern.ToString
            objectSearchPatterns.Add(cableOccurrence.SystemId, objectSearchPattern)
        Next
    End Sub

    Private Sub LoadCavitiesIntoSearchMapper(kblMapper As KblMapper, objectSearchPatterns As Dictionary(Of String, ObjectSearchPattern), searchableGridColumns As IEnumerable(Of String))
        For Each connectorOccurrence As Connector_occurrence In kblMapper.GetConnectorOccurrences
            For Each contactPoint As Contact_point In connectorOccurrence.Contact_points
                For Each cavityId As String In contactPoint.Contacted_cavity.SplitSpace
                    Dim cavityOccurrence As Cavity_occurrence = DirectCast(kblMapper.KBLOccurrenceMapper(cavityId), Cavity_occurrence)
                    If (cavityOccurrence IsNot Nothing) AndAlso (kblMapper.KBLPartMapper.ContainsKey(cavityOccurrence.Part)) AndAlso (Not objectSearchPatterns.ContainsKey(cavityOccurrence.SystemId)) Then
                        Dim cavityOccurrenceId As String = cavityOccurrence.SystemId
                        Dim propertySearchPattern As New StringBuilder

                        If (searchableGridColumns.Contains(ConnectorPropertyName.Cavity_number.ToString)) Then propertySearchPattern.Append(String.Format("{0}|", DirectCast(kblMapper.KBLPartMapper(cavityOccurrence.Part), Cavity).Cavity_number))

                        If (contactPoint.Associated_parts IsNot Nothing) Then
                            For Each associatedPart As String In contactPoint.Associated_parts.SplitSpace
                                If (kblMapper.KBLOccurrenceMapper.ContainsKey(associatedPart)) Then
                                    cavityOccurrenceId = String.Format("{0}|{1}", cavityOccurrenceId, associatedPart)

                                    If (TypeOf kblMapper.KBLOccurrenceMapper(associatedPart) Is Special_terminal_occurrence AndAlso kblMapper.KBLPartMapper.ContainsKey(DirectCast(kblMapper.KBLOccurrenceMapper(associatedPart), Special_terminal_occurrence).Part)) OrElse (TypeOf kblMapper.KBLOccurrenceMapper(associatedPart) Is Terminal_occurrence AndAlso kblMapper.KBLPartMapper.ContainsKey(DirectCast(kblMapper.KBLOccurrenceMapper(associatedPart), Terminal_occurrence).Part)) Then
                                        Dim generalTerminal As General_terminal = If(kblMapper.GetHarness.GetSpecialTerminalOccurrence(associatedPart) IsNot Nothing, kblMapper.GetGeneralTerminal(kblMapper.GetHarness.GetSpecialTerminalOccurrence(associatedPart).Part), kblMapper.GetGeneralTerminal(kblMapper.GetHarness.GetTerminalOccurrence(associatedPart).Part))

                                        If (searchableGridColumns.Contains(ConnectorPropertyName.Terminal_part_number.ToString)) Then propertySearchPattern.Append(String.Format("{0}|", generalTerminal.Part_number))
                                        If (searchableGridColumns.Contains(ConnectorPropertyName.Terminal_part_information.ToString)) Then
                                            For Each generalTerminalProperty As PropertyInfo In generalTerminal.GetType.GetProperties
                                                If (generalTerminalProperty.Name = CommonPropertyName.Alias_Id.ToString) Then
                                                    LoadAliasIdsIntoSearchPattern(DirectCast(generalTerminalProperty.GetValue(generalTerminal), IEnumerable(Of Alias_identification)), propertySearchPattern)
                                                ElseIf (generalTerminalProperty.Name = PartPropertyName.Change.ToString) Then
                                                    LoadChangesIntoSearchPattern(DirectCast(generalTerminalProperty.GetValue(generalTerminal), IEnumerable(Of Change)), propertySearchPattern)
                                                ElseIf (generalTerminalProperty.Name = WirePropertyName.Cross_section_area.ToString) Then
                                                    Dim crossSectionArea As Value_range = DirectCast(generalTerminalProperty.GetValue(generalTerminal), Value_range)
                                                    If (crossSectionArea IsNot Nothing) Then propertySearchPattern.Append(String.Format("{0}|", String.Format("{0}|{1}", crossSectionArea.Minimum, crossSectionArea.Maximum)))
                                                ElseIf (generalTerminalProperty.Name = PartPropertyName.Mass_information.ToString) Then
                                                    LoadNumericalValueIntoSearchPattern(DirectCast(generalTerminalProperty.GetValue(generalTerminal), Numerical_value), propertySearchPattern)
                                                ElseIf (generalTerminalProperty.Name = PartPropertyName.Material_information.ToString) Then
                                                    LoadMaterialInformationIntoSearchPattern(DirectCast(generalTerminalProperty.GetValue(generalTerminal), Material), propertySearchPattern)
                                                ElseIf (generalTerminalProperty.Name = WirePropertyName.Outside_diameter.ToString) Then
                                                    Dim outsideDiameter As Value_range = DirectCast(generalTerminalProperty.GetValue(generalTerminal), Value_range)
                                                    If (outsideDiameter IsNot Nothing) Then propertySearchPattern.Append(String.Format("{0}|", String.Format("{0}|{1}", outsideDiameter.Minimum, outsideDiameter.Maximum)))
                                                ElseIf (generalTerminalProperty.Name = CavityPropertyName.Processing_information.ToString) Then
                                                    LoadProcessingInformationIntoSearchPattern(DirectCast(generalTerminalProperty.GetValue(generalTerminal), IEnumerable(Of Processing_instruction)), propertySearchPattern)
                                                ElseIf (generalTerminalProperty.Name <> Zuken.E3.HarnessAnalyzer.Shared.SYSTEM_ID) AndAlso (generalTerminalProperty.Name <> PartPropertyName.Part_number.ToString) Then
                                                    propertySearchPattern.Append(String.Format("{0}|", generalTerminalProperty.GetValue(generalTerminal)))
                                                End If
                                            Next
                                        End If
                                    ElseIf (TypeOf kblMapper.KBLOccurrenceMapper(associatedPart) Is Cavity_seal_occurrence AndAlso kblMapper.KBLPartMapper.ContainsKey(DirectCast(kblMapper.KBLOccurrenceMapper(associatedPart), Cavity_seal_occurrence).Part)) Then
                                        If (searchableGridColumns.Contains(ConnectorPropertyName.Seal_part_number.ToString)) Then
                                            propertySearchPattern.Append(String.Format("{0}|", DirectCast(kblMapper.KBLPartMapper(DirectCast(kblMapper.KBLOccurrenceMapper(associatedPart), Cavity_seal_occurrence).Part), Cavity_seal).Part_number))
                                        End If
                                        If (searchableGridColumns.Contains(ConnectorPropertyName.Seal_part_information.ToString)) Then
                                            Dim cavitySeal As Cavity_seal = DirectCast(kblMapper.KBLPartMapper(DirectCast(kblMapper.KBLOccurrenceMapper(associatedPart), Cavity_seal_occurrence).Part), Cavity_seal)

                                            For Each cavitySealProperty As PropertyInfo In cavitySeal.GetType.GetProperties
                                                If (cavitySealProperty.Name = CommonPropertyName.Alias_Id.ToString) Then
                                                    LoadAliasIdsIntoSearchPattern(DirectCast(cavitySealProperty.GetValue(cavitySeal), IEnumerable(Of Alias_identification)), propertySearchPattern)
                                                ElseIf (cavitySealProperty.Name = PartPropertyName.Change.ToString) Then
                                                    LoadChangesIntoSearchPattern(DirectCast(cavitySealProperty.GetValue(cavitySeal), IEnumerable(Of Change)), propertySearchPattern)
                                                ElseIf (cavitySealProperty.Name = PartPropertyName.Mass_information.ToString) Then
                                                    LoadNumericalValueIntoSearchPattern(DirectCast(cavitySealProperty.GetValue(cavitySeal), Numerical_value), propertySearchPattern)
                                                ElseIf (cavitySealProperty.Name = PartPropertyName.Material_information.ToString) Then
                                                    LoadMaterialInformationIntoSearchPattern(DirectCast(cavitySealProperty.GetValue(cavitySeal), Material), propertySearchPattern)
                                                ElseIf (cavitySealProperty.Name = CavityPropertyName.Processing_information.ToString) Then
                                                    LoadProcessingInformationIntoSearchPattern(DirectCast(cavitySealProperty.GetValue(cavitySeal), IEnumerable(Of Processing_instruction)), propertySearchPattern)
                                                ElseIf (cavitySealProperty.Name = "Wire_size") Then
                                                    Dim wireSize As Value_range = DirectCast(cavitySealProperty.GetValue(cavitySeal), Value_range)
                                                    If (wireSize IsNot Nothing) Then propertySearchPattern.Append(String.Format("{0}|", String.Format("{0}|{1}", wireSize.Minimum, wireSize.Maximum)))
                                                ElseIf (cavitySealProperty.Name <> Zuken.E3.HarnessAnalyzer.Shared.SYSTEM_ID) AndAlso (cavitySealProperty.Name <> PartPropertyName.Part_number.ToString) Then
                                                    propertySearchPattern.Append(String.Format("{0}|", cavitySealProperty.GetValue(cavitySeal)))
                                                End If
                                            Next
                                        End If
                                    End If
                                End If
                            Next
                        End If

                        If (cavityOccurrence.Associated_plug IsNot Nothing) AndAlso (kblMapper.KBLOccurrenceMapper.ContainsKey(cavityOccurrence.Associated_plug)) AndAlso (kblMapper.KBLPartMapper.ContainsKey(DirectCast(kblMapper.KBLOccurrenceMapper(cavityOccurrence.Associated_plug), Cavity_plug_occurrence).Part)) Then
                            If (searchableGridColumns.Contains(ConnectorPropertyName.Plug_part_number)) Then
                                propertySearchPattern.Append(String.Format("{0}|", DirectCast(kblMapper.KBLPartMapper(DirectCast(kblMapper.KBLOccurrenceMapper(cavityOccurrence.Associated_plug), Cavity_plug_occurrence).Part), Cavity_plug).Part_number))
                            End If

                            If (searchableGridColumns.Contains(ConnectorPropertyName.Plug_part_information.ToString)) Then
                                Dim cavityPlug As Cavity_plug = DirectCast(kblMapper.KBLPartMapper(DirectCast(kblMapper.KBLOccurrenceMapper(cavityOccurrence.Associated_plug), Cavity_plug_occurrence).Part), Cavity_plug)

                                cavityOccurrenceId = String.Format("{0}|{1}", cavityOccurrenceId, cavityOccurrence.Associated_plug)

                                For Each cavityPlugProperty As PropertyInfo In cavityPlug.GetType.GetProperties
                                    If (cavityPlugProperty.Name = CommonPropertyName.Alias_Id.ToString) Then
                                        LoadAliasIdsIntoSearchPattern(DirectCast(cavityPlugProperty.GetValue(cavityPlug), IEnumerable(Of Alias_identification)), propertySearchPattern)
                                    ElseIf (cavityPlugProperty.Name = PartPropertyName.Change.ToString) Then
                                        LoadChangesIntoSearchPattern(DirectCast(cavityPlugProperty.GetValue(cavityPlug), IEnumerable(Of Change)), propertySearchPattern)
                                    ElseIf (cavityPlugProperty.Name = PartPropertyName.Mass_information.ToString) Then
                                        LoadNumericalValueIntoSearchPattern(DirectCast(cavityPlugProperty.GetValue(cavityPlug), Numerical_value), propertySearchPattern)
                                    ElseIf (cavityPlugProperty.Name = PartPropertyName.Material_information.ToString) Then
                                        LoadMaterialInformationIntoSearchPattern(DirectCast(cavityPlugProperty.GetValue(cavityPlug), Material), propertySearchPattern)
                                    ElseIf (cavityPlugProperty.Name = CavityPropertyName.Processing_information.ToString) Then
                                        LoadProcessingInformationIntoSearchPattern(DirectCast(cavityPlugProperty.GetValue(cavityPlug), IEnumerable(Of Processing_instruction)), propertySearchPattern)
                                    ElseIf (cavityPlugProperty.Name <> Zuken.E3.HarnessAnalyzer.Shared.SYSTEM_ID) AndAlso (cavityPlugProperty.Name <> PartPropertyName.Part_number.ToString) Then
                                        propertySearchPattern.Append(String.Format("{0}|", cavityPlugProperty.GetValue(cavityPlug)))
                                    End If
                                Next
                            End If
                        End If

                        If (propertySearchPattern.Length > 0) Then
                            propertySearchPattern.Remove(propertySearchPattern.Length - 1, 1)
                        End If

                        Dim objectSearchPattern As New ObjectSearchPattern

                        If (cavityOccurrence.Part IsNot Nothing) AndAlso (kblMapper.KBLPartMapper.ContainsKey(cavityOccurrence.Part)) Then
                            objectSearchPattern.DisplayText = String.Format("{0} [{1}]", DirectCast(kblMapper.KBLPartMapper(cavityOccurrence.Part), Cavity).Cavity_number, connectorOccurrence.Id)
                        Else
                            objectSearchPattern.DisplayText = String.Format("{0} [{1}]", cavityOccurrence.SystemId, connectorOccurrence.Id)
                        End If

                        objectSearchPattern.PropertyValuePattern = propertySearchPattern.ToString

                        If (Not objectSearchPatterns.ContainsKey(cavityOccurrenceId)) Then
                            objectSearchPatterns.Add(cavityOccurrenceId, objectSearchPattern)
                        End If
                    End If
                Next
            Next
        Next
    End Sub

    Private Sub LoadChangesIntoSearchPattern(changes As IEnumerable(Of Change), propertySearchPattern As StringBuilder)
        For Each change As Change In changes
            For Each changeProperty As PropertyInfo In change.GetType.GetProperties
                If (changeProperty.Name <> Zuken.E3.HarnessAnalyzer.Shared.SYSTEM_ID) Then
                    propertySearchPattern.Append(String.Format("{0}|", changeProperty.GetValue(change)))
                End If
            Next
        Next
    End Sub

    Private Sub LoadChangeDescriptionsIntoSearchMapper(kblMapper As KblMapper, objectSearchPatterns As Dictionary(Of String, ObjectSearchPattern), searchableGridColumns As IEnumerable(Of String))
        For Each changeDescription As Change_description In kblMapper.GetChangeDescriptions
            Dim propertySearchPattern As New StringBuilder

            For Each changeDescriptionProperty As PropertyInfo In changeDescription.GetType.GetProperties
                If (changeDescriptionProperty.Name = ChangeDescriptionPropertyName.Changed_elements.ToString) Then

                ElseIf (changeDescriptionProperty.Name = ChangeDescriptionPropertyName.Related_changes.ToString) AndAlso (changeDescriptionProperty.GetValue(changeDescription) IsNot Nothing) Then
                    Dim changes As New List(Of Change)

                    For Each relatedChangeId As String In changeDescriptionProperty.GetValue(changeDescription).ToString.SplitSpace
                        If (kblMapper.KBLChangeMapper.ContainsKey(relatedChangeId)) Then
                            changes.Add(kblMapper.KBLChangeMapper(relatedChangeId))
                        End If
                    Next

                    LoadChangesIntoSearchPattern(changes, propertySearchPattern)
                ElseIf (searchableGridColumns.Contains(changeDescriptionProperty.Name)) Then
                    propertySearchPattern.Append(String.Format("{0}|", changeDescriptionProperty.GetValue(changeDescription)))
                End If
            Next

            If (propertySearchPattern.Length > 0) Then propertySearchPattern.Remove(propertySearchPattern.Length - 1, 1)

            Dim objectSearchPattern As New ObjectSearchPattern

            If (changeDescription.Description IsNot Nothing) AndAlso (changeDescription.Description <> String.Empty) Then
                objectSearchPattern.DisplayText = String.Format("{0} [{1}]", changeDescription.Id, changeDescription.Description)
            Else
                objectSearchPattern.DisplayText = changeDescription.Id
            End If

            objectSearchPattern.PropertyValuePattern = propertySearchPattern.ToString
            objectSearchPatterns.Add(changeDescription.SystemId, objectSearchPattern)
        Next
    End Sub

    Private Sub LoadComponentBoxesIntoSearchMapper(kblMapper As KblMapper, objectSearchPatterns As Dictionary(Of String, ObjectSearchPattern), searchableGridColumns As IEnumerable(Of String))
        For Each compBoxOccurrence As Component_box_occurrence In kblMapper.GetHarness.Component_box_occurrence
            Dim propertySearchPattern As New StringBuilder

            For Each compBoxOccurrenceProperty As PropertyInfo In compBoxOccurrence.GetType.GetProperties
                If (compBoxOccurrenceProperty.Name = ComponentBoxPropertyName.Alias_id.ToString) AndAlso (searchableGridColumns.Contains(ComponentBoxPropertyName.Alias_id.ToString)) Then
                    LoadAliasIdsIntoSearchPattern(DirectCast(compBoxOccurrenceProperty.GetValue(compBoxOccurrence), IEnumerable(Of Alias_identification)), propertySearchPattern)
                ElseIf (compBoxOccurrenceProperty.Name = ComponentBoxPropertyName.Installation_Information.ToString) AndAlso (searchableGridColumns.Contains(ComponentBoxPropertyName.Installation_Information.ToString)) Then
                    LoadInstallationInformationIntoSearchPattern(DirectCast(compBoxOccurrenceProperty.GetValue(compBoxOccurrence), IEnumerable(Of Installation_instruction)), propertySearchPattern)
                ElseIf (compBoxOccurrenceProperty.Name = ComponentBoxPropertyName.Part.ToString) AndAlso (kblMapper.KBLPartMapper.ContainsKey(compBoxOccurrenceProperty.GetValue(compBoxOccurrence).ToString)) Then
                    Dim componentBox As Component_box = DirectCast(kblMapper.KBLPartMapper(compBoxOccurrenceProperty.GetValue(compBoxOccurrence).ToString), Component_box)

                    For Each compBoxProperty As PropertyInfo In componentBox.GetType.GetProperties
                        If (compBoxProperty.Name = ComponentBoxPropertyName.Alias_id.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Part_alias_ids.ToString)) Then
                            LoadAliasIdsIntoSearchPattern(DirectCast(compBoxProperty.GetValue(componentBox), IEnumerable(Of Alias_identification)), propertySearchPattern)
                        ElseIf (compBoxProperty.Name = PartPropertyName.Change.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Change.ToString)) Then
                            LoadChangesIntoSearchPattern(DirectCast(compBoxProperty.GetValue(componentBox), IEnumerable(Of Change)), propertySearchPattern)
                        ElseIf (compBoxProperty.Name = PartPropertyName.Mass_information.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Mass_information.ToString)) Then
                            LoadNumericalValueIntoSearchPattern(DirectCast(compBoxProperty.GetValue(componentBox), Numerical_value), propertySearchPattern)
                        ElseIf (compBoxProperty.Name = PartPropertyName.Material_information.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Material_information.ToString)) Then
                            LoadMaterialInformationIntoSearchPattern(DirectCast(compBoxProperty.GetValue(componentBox), Material), propertySearchPattern)
                        ElseIf (compBoxProperty.Name = CommonPropertyName.Processing_Information.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Part_processing_information.ToString)) Then
                            LoadProcessingInformationIntoSearchPattern(DirectCast(compBoxProperty.GetValue(componentBox), IEnumerable(Of Processing_instruction)), propertySearchPattern)
                        ElseIf (searchableGridColumns.Contains(compBoxProperty.Name)) OrElse (compBoxProperty.Name = AssemblyPartPropertyName.Description.ToString AndAlso searchableGridColumns.Contains(PartPropertyName.Part_description.ToString)) Then
                            propertySearchPattern.Append(String.Format("{0}|", compBoxProperty.GetValue(componentBox)))
                        End If
                    Next
                ElseIf (searchableGridColumns.Contains(compBoxOccurrenceProperty.Name)) Then
                    propertySearchPattern.Append(String.Format("{0}|", compBoxOccurrenceProperty.GetValue(compBoxOccurrence)))
                End If
            Next

            If (propertySearchPattern.Length > 0) Then propertySearchPattern.Remove(propertySearchPattern.Length - 1, 1)

            Dim objectSearchPattern As New ObjectSearchPattern

            If (compBoxOccurrence.Description IsNot Nothing) AndAlso (compBoxOccurrence.Description <> String.Empty) Then
                objectSearchPattern.DisplayText = String.Format("{0} [{1}]", compBoxOccurrence.Id, compBoxOccurrence.Description)
            Else
                objectSearchPattern.DisplayText = compBoxOccurrence.Id
            End If

            objectSearchPattern.PropertyValuePattern = propertySearchPattern.ToString
            objectSearchPatterns.Add(compBoxOccurrence.SystemId, objectSearchPattern)
        Next
    End Sub

    Private Sub LoadComponentsIntoSearchMapper(kblMapper As KblMapper, objectSearchPatterns As Dictionary(Of String, ObjectSearchPattern), searchableGridColumns As IEnumerable(Of String))
        For Each componentOccurrence As Component_occurrence In kblMapper.GetComponentOccurrences
            Dim propertySearchPattern As New StringBuilder

            For Each componentOccurrenceProperty As PropertyInfo In componentOccurrence.GetType.GetProperties
                If (componentOccurrenceProperty.Name = ComponentPropertyName.Alias_id.ToString) AndAlso (searchableGridColumns.Contains(ComponentPropertyName.Alias_id.ToString)) Then
                    LoadAliasIdsIntoSearchPattern(DirectCast(componentOccurrenceProperty.GetValue(componentOccurrence), IEnumerable(Of Alias_identification)), propertySearchPattern)
                ElseIf (componentOccurrenceProperty.Name = ComponentPropertyName.Installation_Information.ToString) AndAlso (searchableGridColumns.Contains(ComponentPropertyName.Installation_Information.ToString)) Then
                    LoadInstallationInformationIntoSearchPattern(DirectCast(componentOccurrenceProperty.GetValue(componentOccurrence), IEnumerable(Of Installation_instruction)), propertySearchPattern)
                ElseIf (componentOccurrenceProperty.Name = ComponentPropertyName.Part.ToString) AndAlso (kblMapper.KBLPartMapper.ContainsKey(componentOccurrenceProperty.GetValue(componentOccurrence).ToString)) Then
                    Dim component As Component = DirectCast(kblMapper.KBLPartMapper(componentOccurrenceProperty.GetValue(componentOccurrence).ToString), Component)

                    For Each componentProperty As PropertyInfo In component.GetType.GetProperties
                        If (componentProperty.Name = ComponentPropertyName.Alias_id.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Part_alias_ids.ToString)) Then
                            LoadAliasIdsIntoSearchPattern(DirectCast(componentProperty.GetValue(component), IEnumerable(Of Alias_identification)), propertySearchPattern)
                        ElseIf (componentProperty.Name = PartPropertyName.Change.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Change.ToString)) Then
                            LoadChangesIntoSearchPattern(DirectCast(componentProperty.GetValue(component), IEnumerable(Of Change)), propertySearchPattern)
                        ElseIf (componentProperty.Name = PartPropertyName.Mass_information.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Mass_information.ToString)) Then
                            LoadNumericalValueIntoSearchPattern(DirectCast(componentProperty.GetValue(component), Numerical_value), propertySearchPattern)
                        ElseIf (componentProperty.Name = PartPropertyName.Material_information.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Material_information.ToString)) Then
                            LoadMaterialInformationIntoSearchPattern(DirectCast(componentProperty.GetValue(component), Material), propertySearchPattern)
                        ElseIf (componentProperty.Name = CommonPropertyName.Processing_Information.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Part_processing_information.ToString)) Then
                            LoadProcessingInformationIntoSearchPattern(DirectCast(componentProperty.GetValue(component), IEnumerable(Of Processing_instruction)), propertySearchPattern)
                        ElseIf (searchableGridColumns.Contains(componentProperty.Name)) OrElse (componentProperty.Name = ComponentPropertyName.Description.ToString AndAlso searchableGridColumns.Contains(PartPropertyName.Part_description.ToString)) Then
                            propertySearchPattern.Append(String.Format("{0}|", componentProperty.GetValue(component)))
                        End If
                    Next
                ElseIf (componentOccurrenceProperty.Name = ComponentPropertyName.ComponentPinMaps.ToString) AndAlso (searchableGridColumns.Contains(ComponentPropertyName.ComponentPinMaps.ToString)) Then
                    For Each componentPinMap As Component_pin_map In DirectCast(componentOccurrenceProperty.GetValue(componentOccurrence), IEnumerable(Of Component_pin_map))
                        For Each componentPinMapProperty As PropertyInfo In componentPinMap.GetType.GetProperties
                            If (componentPinMapProperty.Name <> Zuken.E3.HarnessAnalyzer.Shared.SYSTEM_ID) Then propertySearchPattern.Append(String.Format("{0}|", componentPinMapProperty.GetValue(componentPinMap)))
                        Next
                    Next
                ElseIf (searchableGridColumns.Contains(componentOccurrenceProperty.Name)) Then
                    propertySearchPattern.Append(String.Format("{0}|", componentOccurrenceProperty.GetValue(componentOccurrence)))
                End If
            Next

            If (propertySearchPattern.Length > 0) Then
                propertySearchPattern.Remove(propertySearchPattern.Length - 1, 1)
            End If

            Dim objectSearchPattern As New ObjectSearchPattern

            If (componentOccurrence.Description IsNot Nothing) AndAlso (componentOccurrence.Description <> String.Empty) Then
                objectSearchPattern.DisplayText = String.Format("{0} [{1}]", componentOccurrence.Id, componentOccurrence.Description)
            Else
                objectSearchPattern.DisplayText = componentOccurrence.Id
            End If

            objectSearchPattern.PropertyValuePattern = propertySearchPattern.ToString
            objectSearchPatterns.Add(componentOccurrence.SystemId, objectSearchPattern)
        Next
    End Sub

    Private Sub LoadConnectorsIntoSearchMapper(kblMapper As KblMapper, objectSearchPatterns As Dictionary(Of String, ObjectSearchPattern), searchableGridColumns As IEnumerable(Of String))
        For Each connectorOccurrence As Connector_occurrence In kblMapper.GetConnectorOccurrences
            Dim propertySearchPattern As New StringBuilder

            For Each connectorOccurrenceProperty As PropertyInfo In connectorOccurrence.GetType.GetProperties
                If (connectorOccurrenceProperty.Name = ConnectorPropertyName.Alias_id.ToString) AndAlso (searchableGridColumns.Contains(ConnectorPropertyName.Alias_id.ToString)) Then
                    LoadAliasIdsIntoSearchPattern(DirectCast(connectorOccurrenceProperty.GetValue(connectorOccurrence), IEnumerable(Of Alias_identification)), propertySearchPattern)
                ElseIf (connectorOccurrenceProperty.Name = ConnectorPropertyName.Installation_Information.ToString) AndAlso (searchableGridColumns.Contains(ConnectorPropertyName.Installation_Information.ToString)) Then
                    LoadInstallationInformationIntoSearchPattern(DirectCast(connectorOccurrenceProperty.GetValue(connectorOccurrence), IEnumerable(Of Installation_instruction)), propertySearchPattern)
                ElseIf (connectorOccurrenceProperty.Name = ConnectorPropertyName.Part.ToString) AndAlso (kblMapper.KBLPartMapper.ContainsKey(connectorOccurrenceProperty.GetValue(connectorOccurrence).ToString)) Then
                    Dim connectorHousing As Connector_housing = DirectCast(kblMapper.KBLPartMapper(connectorOccurrenceProperty.GetValue(connectorOccurrence).ToString), Connector_housing)

                    For Each conHousingProperty As PropertyInfo In connectorHousing.GetType.GetProperties
                        If (conHousingProperty.Name = ConnectorPropertyName.Alias_id.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Part_alias_ids.ToString)) Then
                            LoadAliasIdsIntoSearchPattern(DirectCast(conHousingProperty.GetValue(connectorHousing), IEnumerable(Of Alias_identification)), propertySearchPattern)
                        ElseIf (conHousingProperty.Name = PartPropertyName.Change.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Change.ToString)) Then
                            LoadChangesIntoSearchPattern(DirectCast(conHousingProperty.GetValue(connectorHousing), IEnumerable(Of Change)), propertySearchPattern)
                        ElseIf (conHousingProperty.Name = PartPropertyName.Mass_information.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Mass_information.ToString)) Then
                            LoadNumericalValueIntoSearchPattern(DirectCast(conHousingProperty.GetValue(connectorHousing), Numerical_value), propertySearchPattern)
                        ElseIf (conHousingProperty.Name = PartPropertyName.Material_information.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Material_information.ToString)) Then
                            LoadMaterialInformationIntoSearchPattern(DirectCast(conHousingProperty.GetValue(connectorHousing), Material), propertySearchPattern)
                        ElseIf (conHousingProperty.Name = CommonPropertyName.Processing_Information.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Part_processing_information.ToString)) Then
                            LoadProcessingInformationIntoSearchPattern(DirectCast(conHousingProperty.GetValue(connectorHousing), IEnumerable(Of Processing_instruction)), propertySearchPattern)
                        ElseIf (searchableGridColumns.Contains(conHousingProperty.Name)) OrElse (conHousingProperty.Name = ConnectorPropertyName.Description.ToString AndAlso searchableGridColumns.Contains(PartPropertyName.Part_description.ToString)) Then
                            propertySearchPattern.Append(String.Format("{0}|", conHousingProperty.GetValue(connectorHousing)))
                        End If
                    Next
                ElseIf (searchableGridColumns.Contains(connectorOccurrenceProperty.Name)) Then
                    propertySearchPattern.Append(String.Format("{0}|", connectorOccurrenceProperty.GetValue(connectorOccurrence)))
                End If
            Next

            If (propertySearchPattern.Length > 0) Then propertySearchPattern.Remove(propertySearchPattern.Length - 1, 1)

            Dim objectSearchPattern As New ObjectSearchPattern

            If (connectorOccurrence.Description IsNot Nothing) AndAlso (connectorOccurrence.Description <> String.Empty) Then
                objectSearchPattern.DisplayText = String.Format("{0} [{1}]", connectorOccurrence.Id, connectorOccurrence.Description)
            Else
                objectSearchPattern.DisplayText = connectorOccurrence.Id
            End If

            objectSearchPattern.PropertyValuePattern = propertySearchPattern.ToString
            objectSearchPatterns.Add(connectorOccurrence.SystemId, objectSearchPattern)
        Next
    End Sub

    Private Sub LoadCoPacksIntoSearchMapper(kblMapper As KblMapper, objectSearchPatterns As Dictionary(Of String, ObjectSearchPattern), searchableGridColumns As IEnumerable(Of String))
        For Each coPackOccurrence As Co_pack_occurrence In kblMapper.GetHarness.Co_pack_occurrence
            Dim propertySearchPattern As New StringBuilder

            For Each coPackOccurrenceProperty As PropertyInfo In coPackOccurrence.GetType.GetProperties
                If (coPackOccurrenceProperty.Name = CoPackPropertyName.Alias_Id.ToString) AndAlso (searchableGridColumns.Contains(CoPackPropertyName.Alias_Id.ToString)) Then
                    LoadAliasIdsIntoSearchPattern(DirectCast(coPackOccurrenceProperty.GetValue(coPackOccurrence), IEnumerable(Of Alias_identification)), propertySearchPattern)
                ElseIf (coPackOccurrenceProperty.Name = CoPackPropertyName.Installation_Information.ToString) AndAlso (searchableGridColumns.Contains(CoPackPropertyName.Installation_Information.ToString)) Then
                    LoadInstallationInformationIntoSearchPattern(DirectCast(coPackOccurrenceProperty.GetValue(coPackOccurrence), IEnumerable(Of Installation_instruction)), propertySearchPattern)
                ElseIf (coPackOccurrenceProperty.Name = CoPackPropertyName.Part.ToString) AndAlso (kblMapper.KBLPartMapper.ContainsKey(coPackOccurrenceProperty.GetValue(coPackOccurrence).ToString)) Then
                    Dim coPackPart As Co_pack_part = DirectCast(kblMapper.KBLPartMapper(coPackOccurrenceProperty.GetValue(coPackOccurrence).ToString), Co_pack_part)

                    For Each coPackPartProperty As PropertyInfo In coPackPart.GetType.GetProperties
                        If (coPackPartProperty.Name = CoPackPropertyName.Alias_Id.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Part_alias_ids.ToString)) Then
                            LoadAliasIdsIntoSearchPattern(DirectCast(coPackPartProperty.GetValue(coPackPart), IEnumerable(Of Alias_identification)), propertySearchPattern)
                        ElseIf (coPackPartProperty.Name = PartPropertyName.Change.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Change.ToString)) Then
                            LoadChangesIntoSearchPattern(DirectCast(coPackPartProperty.GetValue(coPackPart), IEnumerable(Of Change)), propertySearchPattern)
                        ElseIf (coPackPartProperty.Name = PartPropertyName.Mass_information.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Mass_information.ToString)) Then
                            LoadNumericalValueIntoSearchPattern(DirectCast(coPackPartProperty.GetValue(coPackPart), Numerical_value), propertySearchPattern)
                        ElseIf (coPackPartProperty.Name = PartPropertyName.Material_information.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Material_information.ToString)) Then
                            LoadMaterialInformationIntoSearchPattern(DirectCast(coPackPartProperty.GetValue(coPackPart), Material), propertySearchPattern)
                        ElseIf (coPackPartProperty.Name = CommonPropertyName.Processing_Information.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Part_processing_information.ToString)) Then
                            LoadProcessingInformationIntoSearchPattern(DirectCast(coPackPartProperty.GetValue(coPackPart), IEnumerable(Of Processing_instruction)), propertySearchPattern)
                        ElseIf (searchableGridColumns.Contains(coPackPartProperty.Name)) OrElse (coPackPartProperty.Name = CoPackPropertyName.Description.ToString AndAlso searchableGridColumns.Contains(PartPropertyName.Part_description.ToString)) Then
                            propertySearchPattern.Append(String.Format("{0}|", coPackPartProperty.GetValue(coPackPart)))
                        End If
                    Next
                ElseIf (searchableGridColumns.Contains(coPackOccurrenceProperty.Name)) Then
                    propertySearchPattern.Append(String.Format("{0}|", coPackOccurrenceProperty.GetValue(coPackOccurrence)))
                End If
            Next

            If (propertySearchPattern.Length > 0) Then propertySearchPattern.Remove(propertySearchPattern.Length - 1, 1)

            Dim objectSearchPattern As New ObjectSearchPattern

            If (coPackOccurrence.Description IsNot Nothing) AndAlso (coPackOccurrence.Description <> String.Empty) Then
                objectSearchPattern.DisplayText = String.Format("{0} [{1}]", coPackOccurrence.Id, coPackOccurrence.Description)
            Else
                objectSearchPattern.DisplayText = coPackOccurrence.Id
            End If

            objectSearchPattern.PropertyValuePattern = propertySearchPattern.ToString
            objectSearchPatterns.Add(coPackOccurrence.SystemId, objectSearchPattern)
        Next
    End Sub

    Private Sub LoadCoresIntoSearchMapper(kblMapper As KblMapper, objectSearchPatterns As Dictionary(Of String, ObjectSearchPattern), searchableGridColumns As IEnumerable(Of String))
        For Each cableOccurrence As Special_wire_occurrence In kblMapper.KBLCableList
            For Each coreOccurrence As Core_occurrence In cableOccurrence.Core_occurrence
                Dim propertySearchPattern As New StringBuilder

                For Each coreOccurrenceProperty As PropertyInfo In coreOccurrence.GetType.GetProperties
                    If (coreOccurrenceProperty.Name = WirePropertyName.Length_information.ToString) AndAlso (searchableGridColumns.Contains(WirePropertyName.Length_information.ToString)) Then
                        LoadLengthInformationIntoSearchPattern(DirectCast(coreOccurrenceProperty.GetValue(coreOccurrence), IEnumerable(Of Wire_length)), propertySearchPattern)
                    ElseIf (coreOccurrenceProperty.Name = WirePropertyName.Part.ToString) AndAlso (kblMapper.KBLPartMapper.ContainsKey(coreOccurrenceProperty.GetValue(coreOccurrence).ToString)) Then
                        Dim core As Core = DirectCast(kblMapper.KBLPartMapper(coreOccurrenceProperty.GetValue(coreOccurrence).ToString), Core)

                        For Each coreProperty As PropertyInfo In core.GetType.GetProperties
                            If (coreProperty.Name = WirePropertyName.Bend_radius.ToString) AndAlso (searchableGridColumns.Contains(WirePropertyName.Bend_radius.ToString)) Then
                                LoadNumericalValueIntoSearchPattern(DirectCast(coreProperty.GetValue(core), Numerical_value), propertySearchPattern)
                            ElseIf (coreProperty.Name = WirePropertyName.Core_Colour.ToString) AndAlso (searchableGridColumns.Contains(WirePropertyName.Core_Colour.ToString)) Then
                                LoadWireColoursIntoSearchPattern(DirectCast(coreProperty.GetValue(core), IEnumerable(Of Wire_colour)), propertySearchPattern)
                            ElseIf (coreProperty.Name = WirePropertyName.Cross_section_area.ToString) AndAlso (searchableGridColumns.Contains(WirePropertyName.Cross_section_area.ToString)) Then
                                LoadNumericalValueIntoSearchPattern(DirectCast(coreProperty.GetValue(core), Numerical_value), propertySearchPattern)
                            ElseIf (coreProperty.Name = WirePropertyName.Outside_diameter.ToString) AndAlso (searchableGridColumns.Contains(WirePropertyName.Outside_diameter.ToString)) Then
                                LoadNumericalValueIntoSearchPattern(DirectCast(coreProperty.GetValue(core), Numerical_value), propertySearchPattern)
                            ElseIf (searchableGridColumns.Contains(coreProperty.Name)) Then
                                propertySearchPattern.Append(String.Format("{0}|", coreProperty.GetValue(core)))
                            End If
                        Next
                    ElseIf (searchableGridColumns.Contains(coreOccurrenceProperty.Name)) Then
                        propertySearchPattern.Append(String.Format("{0}|", coreOccurrenceProperty.GetValue(coreOccurrence)))
                    End If
                Next

                If (propertySearchPattern.Length > 0) Then propertySearchPattern.Remove(propertySearchPattern.Length - 1, 1)

                Dim objectSearchPattern As New ObjectSearchPattern

                If (coreOccurrence.Part IsNot Nothing) AndAlso (kblMapper.KBLPartMapper.ContainsKey(coreOccurrence.Part)) Then
                    objectSearchPattern.DisplayText = String.Format("{0} [{1}]", coreOccurrence.Wire_number, DirectCast(kblMapper.KBLPartMapper(coreOccurrence.Part), Core).Id)
                Else
                    objectSearchPattern.DisplayText = coreOccurrence.Wire_number
                End If

                objectSearchPattern.PropertyValuePattern = propertySearchPattern.ToString
                objectSearchPatterns.Add(coreOccurrence.SystemId, objectSearchPattern)
            Next
        Next
    End Sub

    Private Sub LoadDefDimSpecsIntoSearchMapper(kblMapper As KblMapper, objectSearchPatterns As Dictionary(Of String, ObjectSearchPattern), searchableGridColumns As IEnumerable(Of String))
        For Each defDimSpec As Default_dimension_specification In kblMapper.GetDefaultDimensionSpecifications
            Dim propertySearchPattern As New StringBuilder

            For Each defDimSpecProperty As PropertyInfo In defDimSpec.GetType.GetProperties
                If (defDimSpecProperty.Name = DefaultDimensionSpecificationPropertyName.Dimension_value_range.ToString) AndAlso (searchableGridColumns.Contains(DefaultDimensionSpecificationPropertyName.Dimension_value_range.ToString)) Then
                    propertySearchPattern.Append(String.Format("{0}|", DirectCast(defDimSpecProperty.GetValue(defDimSpec), Value_range).Minimum))
                    propertySearchPattern.Append(String.Format("{0}|", DirectCast(defDimSpecProperty.GetValue(defDimSpec), Value_range).Maximum))
                ElseIf (defDimSpecProperty.Name = DefaultDimensionSpecificationPropertyName.Tolerance_indication.ToString) AndAlso (searchableGridColumns.Contains(DefaultDimensionSpecificationPropertyName.Tolerance_indication.ToString)) Then
                    Dim tolerance As Tolerance = DirectCast(defDimSpecProperty.GetValue(defDimSpec), Tolerance)
                    If (tolerance.Lower_limit IsNot Nothing) Then LoadNumericalValueIntoSearchPattern(tolerance.Lower_limit, propertySearchPattern)
                    If (tolerance.Upper_limit IsNot Nothing) Then LoadNumericalValueIntoSearchPattern(tolerance.Upper_limit, propertySearchPattern)
                ElseIf (searchableGridColumns.Contains(defDimSpecProperty.Name)) Then
                    propertySearchPattern.Append(String.Format("{0}|", defDimSpecProperty.GetValue(defDimSpec)))
                End If
            Next

            If (propertySearchPattern.Length > 0) Then propertySearchPattern.Remove(propertySearchPattern.Length - 1, 1)

            Dim objectSearchPattern As New ObjectSearchPattern

            If (defDimSpec.Tolerance_type IsNot Nothing) AndAlso (defDimSpec.Tolerance_type <> String.Empty) Then
                objectSearchPattern.DisplayText = String.Format("{0} [{1}]", defDimSpec.Tolerance_type, defDimSpec.SystemId)
            Else
                objectSearchPattern.DisplayText = defDimSpec.SystemId
            End If

            objectSearchPattern.PropertyValuePattern = propertySearchPattern.ToString
            objectSearchPatterns.Add(defDimSpec.SystemId, objectSearchPattern)
        Next
    End Sub

    Private Sub LoadDimSpecsIntoSearchMapper(kblMapper As KblMapper, objectSearchPatterns As Dictionary(Of String, ObjectSearchPattern), searchableGridColumns As IEnumerable(Of String))
        For Each dimSpec As Dimension_specification In kblMapper.GetDimensionSpecifications
            Dim propertySearchPattern As New StringBuilder

            For Each dimSpecProperty As PropertyInfo In dimSpec.GetType.GetProperties
                If (dimSpecProperty.Name = AccessoryPropertyName.Alias_id.ToString) AndAlso (searchableGridColumns.Contains(AccessoryPropertyName.Alias_id.ToString)) Then
                    LoadAliasIdsIntoSearchPattern(DirectCast(dimSpecProperty.GetValue(dimSpec), IEnumerable(Of Alias_identification)), propertySearchPattern)
                ElseIf (dimSpecProperty.Name = DimensionSpecificationPropertyName.Dimension_value.ToString) AndAlso (searchableGridColumns.Contains(DimensionSpecificationPropertyName.Dimension_value.ToString)) Then
                    LoadNumericalValueIntoSearchPattern(DirectCast(dimSpecProperty.GetValue(dimSpec), Numerical_value), propertySearchPattern)
                ElseIf (dimSpecProperty.Name = DimensionSpecificationPropertyName.Processing_Information.ToString) AndAlso (searchableGridColumns.Contains(DimensionSpecificationPropertyName.Processing_Information.ToString)) Then
                    LoadProcessingInformationIntoSearchPattern(DirectCast(dimSpecProperty.GetValue(dimSpec), IEnumerable(Of Processing_instruction)), propertySearchPattern)
                ElseIf (dimSpecProperty.Name = DimensionSpecificationPropertyName.Tolerance_indication.ToString) AndAlso (searchableGridColumns.Contains(DimensionSpecificationPropertyName.Tolerance_indication.ToString)) Then
                    Dim tolerance As Tolerance = TryCast(dimSpecProperty.GetValue(dimSpec), Tolerance)
                    If (tolerance IsNot Nothing) AndAlso (tolerance.Lower_limit IsNot Nothing) Then LoadNumericalValueIntoSearchPattern(tolerance.Lower_limit, propertySearchPattern)
                    If (tolerance IsNot Nothing) AndAlso (tolerance.Upper_limit IsNot Nothing) Then LoadNumericalValueIntoSearchPattern(tolerance.Upper_limit, propertySearchPattern)
                ElseIf (searchableGridColumns.Contains(dimSpecProperty.Name)) Then
                    propertySearchPattern.Append(String.Format("{0}|", dimSpecProperty.GetValue(dimSpec)))
                End If
            Next

            If (propertySearchPattern.Length > 0) Then propertySearchPattern.Remove(propertySearchPattern.Length - 1, 1)

            Dim objectSearchPattern As New ObjectSearchPattern

            If (dimSpec.Id IsNot Nothing) AndAlso (dimSpec.Id <> String.Empty) Then
                objectSearchPattern.DisplayText = String.Format("{0} [{1}]", dimSpec.Id, dimSpec.SystemId)
            Else
                objectSearchPattern.DisplayText = dimSpec.SystemId
            End If

            objectSearchPattern.PropertyValuePattern = propertySearchPattern.ToString
            objectSearchPatterns.Add(dimSpec.SystemId, objectSearchPattern)
        Next
    End Sub

    Private Sub LoadFixingsIntoSearchMapper(kblMapper As KblMapper, objectSearchPatterns As Dictionary(Of String, ObjectSearchPattern), searchableGridColumns As IEnumerable(Of String))
        For Each fixingOccurrence As Fixing_occurrence In kblMapper.GetFixingOccurrences
            Dim propertySearchPattern As New StringBuilder

            For Each fixingOccurrenceProperty As PropertyInfo In fixingOccurrence.GetType.GetProperties
                If (fixingOccurrenceProperty.Name = FixingPropertyName.Alias_Id.ToString) AndAlso (searchableGridColumns.Contains(FixingPropertyName.Alias_Id.ToString)) Then
                    LoadAliasIdsIntoSearchPattern(DirectCast(fixingOccurrenceProperty.GetValue(fixingOccurrence), IEnumerable(Of Alias_identification)), propertySearchPattern)
                ElseIf (fixingOccurrenceProperty.Name = FixingPropertyName.Installation_Information.ToString) AndAlso (searchableGridColumns.Contains(FixingPropertyName.Installation_Information.ToString)) Then
                    LoadInstallationInformationIntoSearchPattern(DirectCast(fixingOccurrenceProperty.GetValue(fixingOccurrence), IEnumerable(Of Installation_instruction)), propertySearchPattern)
                ElseIf (fixingOccurrenceProperty.Name = FixingPropertyName.Part.ToString) AndAlso (kblMapper.KBLPartMapper.ContainsKey(fixingOccurrenceProperty.GetValue(fixingOccurrence).ToString)) Then
                    Dim fixing As Fixing = DirectCast(kblMapper.KBLPartMapper(fixingOccurrenceProperty.GetValue(fixingOccurrence).ToString), Fixing)

                    For Each fixingProperty As PropertyInfo In fixing.GetType.GetProperties
                        If (fixingProperty.Name = FixingPropertyName.Alias_Id.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Part_alias_ids.ToString)) Then
                            LoadAliasIdsIntoSearchPattern(DirectCast(fixingProperty.GetValue(fixing), IEnumerable(Of Alias_identification)), propertySearchPattern)
                        ElseIf (fixingProperty.Name = PartPropertyName.Change.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Change.ToString)) Then
                            LoadChangesIntoSearchPattern(DirectCast(fixingProperty.GetValue(fixing), IEnumerable(Of Change)), propertySearchPattern)
                        ElseIf (fixingProperty.Name = PartPropertyName.Mass_information.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Mass_information.ToString)) Then
                            LoadNumericalValueIntoSearchPattern(DirectCast(fixingProperty.GetValue(fixing), Numerical_value), propertySearchPattern)
                        ElseIf (fixingProperty.Name = PartPropertyName.Material_information.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Material_information.ToString)) Then
                            LoadMaterialInformationIntoSearchPattern(DirectCast(fixingProperty.GetValue(fixing), Material), propertySearchPattern)
                        ElseIf (fixingProperty.Name = CommonPropertyName.Processing_Information.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Part_processing_information.ToString)) Then
                            LoadProcessingInformationIntoSearchPattern(DirectCast(fixingProperty.GetValue(fixing), IEnumerable(Of Processing_instruction)), propertySearchPattern)
                        ElseIf (searchableGridColumns.Contains(fixingProperty.Name)) OrElse (fixingProperty.Name = FixingPropertyName.Description.ToString AndAlso searchableGridColumns.Contains(PartPropertyName.Part_description.ToString)) Then
                            propertySearchPattern.Append(String.Format("{0}|", fixingProperty.GetValue(fixing)))
                        End If
                    Next
                ElseIf (searchableGridColumns.Contains(fixingOccurrenceProperty.Name)) Then
                    propertySearchPattern.Append(String.Format("{0}|", fixingOccurrenceProperty.GetValue(fixingOccurrence)))
                End If
            Next

            If (propertySearchPattern.Length > 0) Then propertySearchPattern.Remove(propertySearchPattern.Length - 1, 1)

            Dim objectSearchPattern As New ObjectSearchPattern

            If (fixingOccurrence.Description IsNot Nothing) AndAlso (fixingOccurrence.Description <> String.Empty) Then
                objectSearchPattern.DisplayText = String.Format("{0} [{1}]", fixingOccurrence.Id, fixingOccurrence.Description)
            Else
                objectSearchPattern.DisplayText = fixingOccurrence.Id
            End If

            objectSearchPattern.PropertyValuePattern = propertySearchPattern.ToString
            objectSearchPatterns.Add(fixingOccurrence.SystemId, objectSearchPattern)
        Next
    End Sub

    Private Sub LoadInstallationInformationIntoSearchPattern(installationInformation As IEnumerable(Of Installation_instruction), propertySearchPattern As StringBuilder)
        For Each installationInstruction As Installation_instruction In installationInformation
            propertySearchPattern.Append(String.Format("{0}|", installationInstruction.Instruction_value))

            If (installationInstruction.ClassificationSpecified) Then
                propertySearchPattern.Append(String.Format("{0}|", installationInstruction.Classification))
            End If
        Next
    End Sub

    Private Sub LoadLengthInformationIntoSearchPattern(lengthInformation As IEnumerable(Of Wire_length), propertySearchPattern As StringBuilder)
        For Each wireLength As Wire_length In lengthInformation
            propertySearchPattern.Append(String.Format("{0}|", wireLength.Length_value.Value_component))
        Next
    End Sub

    Private Sub LoadMaterialInformationIntoSearchPattern(materialInformation As Material, propertySearchPattern As StringBuilder)
        If (materialInformation IsNot Nothing) Then
            For Each materialInfoProperty As PropertyInfo In materialInformation.GetType.GetProperties
                If (materialInfoProperty.Name <> Zuken.E3.HarnessAnalyzer.Shared.SYSTEM_ID) Then
                    propertySearchPattern.Append(String.Format("{0}|", materialInfoProperty.GetValue(materialInformation)))
                End If
            Next
        End If
    End Sub

    Private Sub LoadModuleChangesIntoSearchMapper(kblMapper As KblMapper, objectSearchPatterns As Dictionary(Of String, ObjectSearchPattern), searchableGridColumns As IEnumerable(Of String))
        For Each [module] As [Lib].Schema.Kbl.[Module] In kblMapper.GetModules
            For Each moduleChange As Change In [module].Change
                Dim propertySearchPattern As New StringBuilder

                For Each changeProperty As PropertyInfo In moduleChange.GetType.GetProperties
                    If (searchableGridColumns.Contains(changeProperty.Name)) Then propertySearchPattern.Append(String.Format("{0}|", changeProperty.GetValue(moduleChange)))
                Next

                If (propertySearchPattern.Length > 0) Then propertySearchPattern.Remove(propertySearchPattern.Length - 1, 1)

                Dim objectSearchPattern As New ObjectSearchPattern

                If (moduleChange.Description IsNot Nothing) AndAlso (moduleChange.Description <> String.Empty) Then
                    objectSearchPattern.DisplayText = String.Format("{0} [{1}]", moduleChange.Id, moduleChange.Description)
                Else
                    objectSearchPattern.DisplayText = String.Format("{0} [{1}]", moduleChange.Id, [module].Part_number)
                End If

                objectSearchPattern.PropertyValuePattern = propertySearchPattern.ToString
                objectSearchPatterns.Add(moduleChange.SystemId, objectSearchPattern)
            Next
        Next
    End Sub

    Private Sub LoadModulesIntoSearchMapper(kblMapper As KblMapper, objectSearchPatterns As Dictionary(Of String, ObjectSearchPattern), searchableGridColumns As IEnumerable(Of String))
        For Each [module] As [Lib].Schema.Kbl.[Module] In kblMapper.GetModules
            Dim propertySearchPattern As New StringBuilder

            For Each moduleProperty As PropertyInfo In [module].GetType.GetProperties
                If (moduleProperty.Name = CommonPropertyName.Alias_Id) AndAlso (searchableGridColumns.Contains(PartPropertyName.Part_alias_ids)) Then
                    LoadAliasIdsIntoSearchPattern(DirectCast(moduleProperty.GetValue([module]), IEnumerable(Of Alias_identification)), propertySearchPattern)
                ElseIf (moduleProperty.Name = PartPropertyName.Mass_information) AndAlso (searchableGridColumns.Contains(PartPropertyName.Mass_information)) Then
                    LoadNumericalValueIntoSearchPattern(DirectCast(moduleProperty.GetValue([module]), Numerical_value), propertySearchPattern)
                ElseIf (moduleProperty.Name = PartPropertyName.Material_information) AndAlso (searchableGridColumns.Contains(PartPropertyName.Material_information)) Then
                    LoadMaterialInformationIntoSearchPattern(DirectCast(moduleProperty.GetValue([module]), Material), propertySearchPattern)
                ElseIf (moduleProperty.Name = ModulePropertyName.Module_configuration) Then
                    Dim moduleConfiguration As Module_configuration = DirectCast(moduleProperty.GetValue([module]), Module_configuration)

                    If searchableGridColumns.Contains(ModulePropertyName.Configuration_type.ToString) AndAlso (moduleConfiguration.Configuration_typeSpecified) Then
                        propertySearchPattern.Append(String.Format("{0}|", moduleConfiguration.Configuration_type.ToStringOrXmlName))
                    End If

                    If (searchableGridColumns.Contains(ModulePropertyName.Logistic_control_information)) Then
                        propertySearchPattern.Append(String.Format("{0}|", moduleConfiguration.Logistic_control_information))
                    End If

                ElseIf (moduleProperty.Name = CommonPropertyName.Processing_Information.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Part_processing_information)) Then
                    LoadProcessingInformationIntoSearchPattern(DirectCast(moduleProperty.GetValue([module]), IEnumerable(Of Processing_instruction)), propertySearchPattern)
                ElseIf (searchableGridColumns.Contains(moduleProperty.Name)) OrElse (moduleProperty.Name = CommonPropertyName.Description.ToString AndAlso searchableGridColumns.Contains(PartPropertyName.Part_description)) Then
                    propertySearchPattern.Append(String.Format("{0}|", moduleProperty.GetValue([module])))
                End If
            Next

            If (propertySearchPattern.Length > 0) Then propertySearchPattern.Remove(propertySearchPattern.Length - 1, 1)

            Dim objectSearchPattern As New ObjectSearchPattern

            If ([module].Description IsNot Nothing) AndAlso ([module].Description <> String.Empty) Then
                objectSearchPattern.DisplayText = String.Format("{0} [{1}]", [module].Abbreviation, [module].Description)
            Else
                objectSearchPattern.DisplayText = String.Format("{0} [{1}]", [module].Abbreviation, [module].Part_number)
            End If

            objectSearchPattern.PropertyValuePattern = propertySearchPattern.ToString
            objectSearchPatterns.Add([module].SystemId, objectSearchPattern)
        Next
    End Sub

    Private Sub LoadNetsIntoSearchMapper(kblMapper As KblMapper, objectSearchPatterns As Dictionary(Of String, ObjectSearchPattern), searchableGridColumns As IEnumerable(Of String))
        For Each connection As Connection In kblMapper.GetConnections
            Dim propertySearchPattern As New StringBuilder

            For Each connectionProperty As PropertyInfo In connection.GetType.GetProperties
                If (connectionProperty.Name = ConnectionPropertyName.Installation_Information.ToString) AndAlso (searchableGridColumns.Contains(ConnectionPropertyName.Installation_Information.ToString)) Then
                    LoadInstallationInformationIntoSearchPattern(DirectCast(connectionProperty.GetValue(connection), IEnumerable(Of Installation_instruction)), propertySearchPattern)
                ElseIf (connectionProperty.Name = ConnectionPropertyName.Processing_Information.ToString) AndAlso (searchableGridColumns.Contains(ConnectionPropertyName.Processing_Information.ToString)) Then
                    LoadProcessingInformationIntoSearchPattern(DirectCast(connectionProperty.GetValue(connection), IEnumerable(Of Processing_instruction)), propertySearchPattern)
                ElseIf (searchableGridColumns.Contains(connectionProperty.Name)) Then
                    propertySearchPattern.Append(String.Format("{0}|", connectionProperty.GetValue(connection)))
                End If
            Next

            If (propertySearchPattern.Length > 0) Then propertySearchPattern.Remove(propertySearchPattern.Length - 1, 1)

            Dim objectSearchPattern As New ObjectSearchPattern

            If (connection.Signal_name IsNot Nothing) AndAlso (connection.Signal_name <> String.Empty) Then
                objectSearchPattern.DisplayText = String.Format("{0} [{1}]", connection.Signal_name, connection.Wire)
            Else
                objectSearchPattern.DisplayText = String.Format("{0} [{1}]", connection.Id, connection.Wire)
            End If

            objectSearchPattern.PropertyValuePattern = propertySearchPattern.ToString
            objectSearchPatterns.Add(connection.SystemId, objectSearchPattern)
        Next
    End Sub

    Private Sub LoadNumericalValueIntoSearchPattern(numericalValue As Numerical_value, propertySearchPattern As StringBuilder)
        If (numericalValue IsNot Nothing) Then propertySearchPattern.Append(String.Format("{0}|", numericalValue.Value_component))
    End Sub

    Private Sub LoadProcessingInformationIntoSearchPattern(processingInformation As IEnumerable(Of Processing_instruction), propertySearchPattern As StringBuilder)
        For Each processingInstruction As Processing_instruction In processingInformation
            propertySearchPattern.Append(String.Format("{0}|", processingInstruction.Instruction_value))

            If (processingInstruction.ClassificationSpecified) Then
                propertySearchPattern.Append(String.Format("{0}|", processingInstruction.Classification.ToStringOrXmlName))
            End If
        Next
    End Sub

    Private Sub LoadProtection_On_SegmentsIntoSearchMapper(kblMapper As KblMapper, objectSearchPatterns As Dictionary(Of String, ObjectSearchPattern), searchableGridColumns As IEnumerable(Of String))
        For Each segment As Segment In kblMapper.GetSegments
            For Each protectionArea As Protection_area In segment.Protection_area
                If (Not objectSearchPatterns.ContainsKey(protectionArea.Associated_protection)) Then
                    Dim propertySearchPattern As New StringBuilder
                    Dim protectionOccurrence As Wire_protection_occurrence = Nothing

                    For Each protectionProperty As PropertyInfo In protectionArea.GetType.GetProperties
                        If (protectionProperty.Name = ProtectionAreaPropertyName.Associated_protection.ToString) AndAlso (kblMapper.KBLOccurrenceMapper.ContainsKey(protectionProperty.GetValue(protectionArea).ToString)) Then
                            protectionOccurrence = DirectCast(kblMapper.KBLOccurrenceMapper(protectionProperty.GetValue(protectionArea).ToString), Wire_protection_occurrence)
                            If (protectionOccurrence IsNot Nothing) Then
                                For Each protectionOccurrenceProperty As PropertyInfo In protectionOccurrence.GetType.GetProperties
                                    If (protectionOccurrenceProperty.Name = CommonPropertyName.Alias_Id.ToString) AndAlso (searchableGridColumns.Contains(WireProtectionPropertyName.Alias_id.ToString)) Then
                                        LoadAliasIdsIntoSearchPattern(DirectCast(protectionOccurrenceProperty.GetValue(protectionOccurrence), IEnumerable(Of Alias_identification)), propertySearchPattern)
                                    ElseIf (protectionOccurrenceProperty.Name = WireProtectionPropertyName.Installation_Information.ToString) AndAlso (searchableGridColumns.Contains(WireProtectionPropertyName.Installation_Information.ToString)) Then
                                        LoadInstallationInformationIntoSearchPattern(DirectCast(protectionOccurrenceProperty.GetValue(protectionOccurrence), IEnumerable(Of Installation_instruction)), propertySearchPattern)
                                    ElseIf (protectionOccurrenceProperty.Name = WireProtectionPropertyName.Part.ToString) AndAlso (kblMapper.KBLPartMapper.ContainsKey(protectionOccurrenceProperty.GetValue(protectionOccurrence).ToString)) Then
                                        Dim wireProtection As Wire_protection = DirectCast(kblMapper.KBLPartMapper(protectionOccurrenceProperty.GetValue(protectionOccurrence).ToString), Wire_protection)

                                        For Each wireProtectionProperty As PropertyInfo In wireProtection.GetType.GetProperties
                                            If (wireProtectionProperty.Name = CommonPropertyName.Alias_Id.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Part_alias_ids.ToString)) Then
                                                LoadAliasIdsIntoSearchPattern(DirectCast(wireProtectionProperty.GetValue(wireProtection), IEnumerable(Of Alias_identification)), propertySearchPattern)
                                            ElseIf (wireProtectionProperty.Name = PartPropertyName.Change.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Change.ToString)) Then
                                                LoadChangesIntoSearchPattern(DirectCast(wireProtectionProperty.GetValue(wireProtection), IEnumerable(Of Change)), propertySearchPattern)
                                            ElseIf (wireProtectionProperty.Name = PartPropertyName.Mass_information.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Mass_information.ToString)) Then
                                                LoadNumericalValueIntoSearchPattern(DirectCast(wireProtectionProperty.GetValue(wireProtection), Numerical_value), propertySearchPattern)
                                            ElseIf (wireProtectionProperty.Name = PartPropertyName.Material_information.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Material_information.ToString)) Then
                                                LoadMaterialInformationIntoSearchPattern(DirectCast(wireProtectionProperty.GetValue(wireProtection), Material), propertySearchPattern)
                                            ElseIf (wireProtectionProperty.Name = CommonPropertyName.Processing_Information.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Part_processing_information.ToString)) Then
                                                LoadProcessingInformationIntoSearchPattern(DirectCast(wireProtectionProperty.GetValue(wireProtection), IEnumerable(Of Processing_instruction)), propertySearchPattern)
                                            ElseIf (searchableGridColumns.Contains(wireProtectionProperty.Name)) OrElse (wireProtectionProperty.Name = WireProtectionPropertyName.Description.ToString AndAlso searchableGridColumns.Contains(PartPropertyName.Part_description.ToString)) Then
                                                propertySearchPattern.Append(String.Format("{0}|", wireProtectionProperty.GetValue(wireProtection)))
                                            End If
                                        Next
                                    ElseIf (protectionOccurrenceProperty.Name = WireProtectionPropertyName.Protection_length.ToString) AndAlso (searchableGridColumns.Contains(WireProtectionPropertyName.Protection_length.ToString)) Then
                                        LoadNumericalValueIntoSearchPattern(DirectCast(protectionOccurrenceProperty.GetValue(protectionOccurrence), Numerical_value), propertySearchPattern)
                                    ElseIf (searchableGridColumns.Contains(protectionOccurrenceProperty.Name)) Then
                                        propertySearchPattern.Append(String.Format("{0}|", protectionOccurrenceProperty.GetValue(protectionOccurrence)))
                                    End If
                                Next
                            End If
                        ElseIf (protectionProperty.Name = ProtectionAreaPropertyName.Processing_information.ToString) AndAlso (searchableGridColumns.Contains(ProtectionAreaPropertyName.Processing_information.ToString)) Then
                            LoadProcessingInformationIntoSearchPattern(DirectCast(protectionProperty.GetValue(protectionArea), IEnumerable(Of Processing_instruction)), propertySearchPattern)
                        ElseIf (searchableGridColumns.Contains(protectionProperty.Name)) Then
                            propertySearchPattern.Append(String.Format("{0}|", protectionProperty.GetValue(protectionArea)))
                        End If
                    Next

                    If (propertySearchPattern.Length > 0) Then propertySearchPattern.Remove(propertySearchPattern.Length - 1, 1)

                    Dim objectSearchPattern As New ObjectSearchPattern

                    If (protectionOccurrence IsNot Nothing) AndAlso (protectionOccurrence.Description IsNot Nothing) AndAlso (protectionOccurrence.Description <> String.Empty) Then
                        objectSearchPattern.DisplayText = String.Format("{0} [{1}]", protectionOccurrence.Id, protectionOccurrence.Description)
                    ElseIf (protectionOccurrence IsNot Nothing) AndAlso (kblMapper.KBLPartMapper.ContainsKey(protectionOccurrence.Part)) Then
                        objectSearchPattern.DisplayText = String.Format("{0} [{1}]", protectionOccurrence.Id, DirectCast(kblMapper.KBLPartMapper(protectionOccurrence.Part), Wire_protection).Part_number)
                    ElseIf (protectionOccurrence IsNot Nothing) Then
                        objectSearchPattern.DisplayText = protectionOccurrence.Id
                    Else
                        objectSearchPattern.DisplayText = protectionArea.Associated_protection
                    End If

                    objectSearchPattern.PropertyValuePattern = propertySearchPattern.ToString
                    objectSearchPatterns.Add(protectionArea.Associated_protection, objectSearchPattern)
                End If
            Next
        Next
    End Sub

    Private Sub LoadProtectionsOnVerticesIntoSearchMapper(kblMapper As KblMapper, objectSearchPatterns As Dictionary(Of String, ObjectSearchPattern), searchableGridColumns As IEnumerable(Of String))
        For Each node As Node In kblMapper.GetNodes
            If (node.Referenced_components IsNot Nothing) AndAlso (node.Referenced_components <> String.Empty) Then
                For Each referencedComponent As String In node.Referenced_components.SplitSpace
                    If (kblMapper.KBLOccurrenceMapper.ContainsKey(referencedComponent)) AndAlso (TypeOf kblMapper.KBLOccurrenceMapper(referencedComponent) Is Wire_protection_occurrence) AndAlso (Not objectSearchPatterns.ContainsKey(referencedComponent)) Then
                        Dim propertySearchPattern As New StringBuilder
                        Dim protectionOccurrence As Wire_protection_occurrence = DirectCast(kblMapper.KBLOccurrenceMapper(referencedComponent), Wire_protection_occurrence)

                        For Each protectionOccurrenceProperty As PropertyInfo In protectionOccurrence.GetType.GetProperties
                            If (protectionOccurrenceProperty.Name = CommonPropertyName.Alias_Id.ToString) AndAlso (searchableGridColumns.Contains(WireProtectionPropertyName.Alias_id.ToString)) Then
                                LoadAliasIdsIntoSearchPattern(DirectCast(protectionOccurrenceProperty.GetValue(protectionOccurrence), IEnumerable(Of Alias_identification)), propertySearchPattern)
                            ElseIf (protectionOccurrenceProperty.Name = WireProtectionPropertyName.Installation_Information.ToString) AndAlso (searchableGridColumns.Contains(WireProtectionPropertyName.Installation_Information.ToString)) Then
                                LoadInstallationInformationIntoSearchPattern(DirectCast(protectionOccurrenceProperty.GetValue(protectionOccurrence), IEnumerable(Of Installation_instruction)), propertySearchPattern)
                            ElseIf (protectionOccurrenceProperty.Name = WireProtectionPropertyName.Part.ToString) AndAlso (kblMapper.KBLPartMapper.ContainsKey(protectionOccurrenceProperty.GetValue(protectionOccurrence).ToString)) Then
                                Dim wireProtection As Wire_protection = DirectCast(kblMapper.KBLPartMapper(protectionOccurrenceProperty.GetValue(protectionOccurrence).ToString), Wire_protection)

                                For Each wireProtectionProperty As PropertyInfo In wireProtection.GetType.GetProperties
                                    If (wireProtectionProperty.Name = CommonPropertyName.Alias_Id.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Part_alias_ids.ToString)) Then
                                        LoadAliasIdsIntoSearchPattern(DirectCast(wireProtectionProperty.GetValue(wireProtection), IEnumerable(Of Alias_identification)), propertySearchPattern)
                                    ElseIf (wireProtectionProperty.Name = PartPropertyName.Change.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Change.ToString)) Then
                                        LoadChangesIntoSearchPattern(DirectCast(wireProtectionProperty.GetValue(wireProtection), IEnumerable(Of Change)), propertySearchPattern)
                                    ElseIf (wireProtectionProperty.Name = PartPropertyName.Mass_information.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Mass_information.ToString)) Then
                                        LoadNumericalValueIntoSearchPattern(DirectCast(wireProtectionProperty.GetValue(wireProtection), Numerical_value), propertySearchPattern)
                                    ElseIf (wireProtectionProperty.Name = PartPropertyName.Material_information.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Material_information.ToString)) Then
                                        LoadMaterialInformationIntoSearchPattern(DirectCast(wireProtectionProperty.GetValue(wireProtection), Material), propertySearchPattern)
                                    ElseIf (wireProtectionProperty.Name = CommonPropertyName.Processing_Information.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Part_processing_information.ToString)) Then
                                        LoadProcessingInformationIntoSearchPattern(DirectCast(wireProtectionProperty.GetValue(wireProtection), IEnumerable(Of Processing_instruction)), propertySearchPattern)
                                    ElseIf (searchableGridColumns.Contains(wireProtectionProperty.Name)) OrElse (wireProtectionProperty.Name = WireProtectionPropertyName.Description.ToString AndAlso searchableGridColumns.Contains(PartPropertyName.Part_description.ToString)) Then
                                        propertySearchPattern.Append(String.Format("{0}|", wireProtectionProperty.GetValue(wireProtection)))
                                    End If
                                Next
                            ElseIf (protectionOccurrenceProperty.Name = WireProtectionPropertyName.Protection_length.ToString) AndAlso (searchableGridColumns.Contains(WireProtectionPropertyName.Protection_length.ToString)) Then
                                LoadNumericalValueIntoSearchPattern(DirectCast(protectionOccurrenceProperty.GetValue(protectionOccurrence), Numerical_value), propertySearchPattern)
                            ElseIf (searchableGridColumns.Contains(protectionOccurrenceProperty.Name)) Then
                                propertySearchPattern.Append(String.Format("{0}|", protectionOccurrenceProperty.GetValue(protectionOccurrence)))
                            End If
                        Next

                        If (propertySearchPattern.Length > 0) Then propertySearchPattern.Remove(propertySearchPattern.Length - 1, 1)

                        Dim objectSearchPattern As New ObjectSearchPattern

                        If (protectionOccurrence.Description IsNot Nothing) AndAlso (protectionOccurrence.Description <> String.Empty) Then
                            objectSearchPattern.DisplayText = String.Format("{0} [{1}]", protectionOccurrence.Id, protectionOccurrence.Description)
                        ElseIf (kblMapper.KBLPartMapper.ContainsKey(protectionOccurrence.Part)) Then
                            objectSearchPattern.DisplayText = String.Format("{0} [{1}]", protectionOccurrence.Id, DirectCast(kblMapper.KBLPartMapper(protectionOccurrence.Part), Wire_protection).Part_number)
                        Else
                            objectSearchPattern.DisplayText = protectionOccurrence.Id
                        End If

                        objectSearchPattern.PropertyValuePattern = propertySearchPattern.ToString
                        objectSearchPatterns.Add(protectionOccurrence.SystemId, objectSearchPattern)
                    End If
                Next
            End If
        Next
    End Sub

    Private Sub LoadSegmentsIntoSearchMapper(kblMapper As KblMapper, objectSearchPatterns As Dictionary(Of String, ObjectSearchPattern), searchableGridColumns As IEnumerable(Of String))
        For Each segment As Segment In kblMapper.GetSegments
            Dim propertySearchPattern As New StringBuilder

            For Each segmentProperty As PropertyInfo In segment.GetType.GetProperties
                If (segmentProperty.Name = CommonPropertyName.Alias_Id.ToString) AndAlso (searchableGridColumns.Contains(SegmentPropertyName.Alias_id.ToString)) Then
                    LoadAliasIdsIntoSearchPattern(DirectCast(segmentProperty.GetValue(segment), IEnumerable(Of Alias_identification)), propertySearchPattern)
                ElseIf (segmentProperty.Name = SegmentPropertyName.Cross_Section_Area_information.ToString) AndAlso (searchableGridColumns.Contains(SegmentPropertyName.Cross_Section_Area_information.ToString)) Then
                    For Each crossSectionArea As Cross_section_area In DirectCast(segmentProperty.GetValue(segment), IEnumerable(Of Cross_section_area))
                        LoadNumericalValueIntoSearchPattern(crossSectionArea.Area, propertySearchPattern)
                    Next
                ElseIf (segmentProperty.Name = SegmentPropertyName.Fixing_Assignment.ToString) AndAlso (searchableGridColumns.Contains(SegmentPropertyName.Fixing_Assignment.ToString)) Then
                    For Each fixingAssignment As Fixing_assignment In DirectCast(segmentProperty.GetValue(segment), IEnumerable(Of Fixing_assignment))
                        For Each fixingAssignmentProperty As PropertyInfo In fixingAssignment.GetType.GetProperties
                            If (fixingAssignmentProperty.Name <> Zuken.E3.HarnessAnalyzer.Shared.SYSTEM_ID) Then
                                If (fixingAssignmentProperty.Name = FixingAssignmentPropertyName.Alias_Id.ToString) Then
                                    LoadAliasIdsIntoSearchPattern(DirectCast(fixingAssignmentProperty.GetValue(fixingAssignment), IEnumerable(Of Alias_identification)), propertySearchPattern)
                                Else
                                    propertySearchPattern.Append(String.Format("{0}|", fixingAssignmentProperty.GetValue(fixingAssignment)))
                                End If
                            End If
                        Next
                    Next
                ElseIf (segmentProperty.Name = SegmentPropertyName.Physical_length.ToString AndAlso searchableGridColumns.Contains(SegmentPropertyName.Physical_length.ToString)) OrElse (segmentProperty.Name = SegmentPropertyName.Virtual_length.ToString AndAlso searchableGridColumns.Contains(SegmentPropertyName.Virtual_length.ToString)) Then
                    LoadNumericalValueIntoSearchPattern(DirectCast(segmentProperty.GetValue(segment), Numerical_value), propertySearchPattern)
                ElseIf (searchableGridColumns.Contains(segmentProperty.Name)) Then
                    propertySearchPattern.Append(String.Format("{0}|", segmentProperty.GetValue(segment)))
                End If
            Next

            If (propertySearchPattern.Length > 0) Then propertySearchPattern.Remove(propertySearchPattern.Length - 1, 1)

            Dim objectSearchPattern As New ObjectSearchPattern
            objectSearchPattern.DisplayText = segment.Id
            objectSearchPattern.PropertyValuePattern = propertySearchPattern.ToString
            objectSearchPatterns.Add(segment.SystemId, objectSearchPattern)
        Next
    End Sub

    Private Sub LoadVerticesIntoSearchMapper(kblMapper As KblMapper, objectSearchPatterns As Dictionary(Of String, ObjectSearchPattern), searchableGridColumns As IEnumerable(Of String))
        For Each node As Node In kblMapper.GetNodes
            Dim propertySearchPattern As New StringBuilder

            For Each nodeProperty As PropertyInfo In node.GetType.GetProperties
                If (nodeProperty.Name = CommonPropertyName.Alias_Id.ToString) AndAlso (searchableGridColumns.Contains(CommonPropertyName.Alias_Id.ToString)) Then
                    LoadAliasIdsIntoSearchPattern(DirectCast(nodeProperty.GetValue(node), IEnumerable(Of Alias_identification)), propertySearchPattern)
                ElseIf (nodeProperty.Name = VertexPropertyName.Cartesian_point.ToString) AndAlso (nodeProperty.GetValue(node) IsNot Nothing) Then
                    If (searchableGridColumns.Contains(VertexPropertyName.Cartesian_pointX.ToString)) Then
                        propertySearchPattern.Append(String.Format("{0}|", DirectCast(kblMapper.KBLOccurrenceMapper(nodeProperty.GetValue(node).ToString), Cartesian_point).Coordinates(0)))
                    End If
                    If (searchableGridColumns.Contains(VertexPropertyName.Cartesian_pointY.ToString)) Then
                        propertySearchPattern.Append(String.Format("{0}|", DirectCast(kblMapper.KBLOccurrenceMapper(nodeProperty.GetValue(node).ToString), Cartesian_point).Coordinates(1)))
                    End If
                    If (searchableGridColumns.Contains(VertexPropertyName.Cartesian_pointZ.ToString)) AndAlso (TryCast(kblMapper.KBLOccurrenceMapper(nodeProperty.GetValue(node).ToString), Cartesian_point).Coordinates.Length > 2) Then
                        propertySearchPattern.Append(String.Format("{0}|", DirectCast(kblMapper.KBLOccurrenceMapper(nodeProperty.GetValue(node).ToString), Cartesian_point).Coordinates(2)))
                    End If
                ElseIf (nodeProperty.Name = VertexPropertyName.Processing_information.ToString) AndAlso (searchableGridColumns.Contains(VertexPropertyName.Processing_information.ToString)) Then
                    LoadProcessingInformationIntoSearchPattern(DirectCast(nodeProperty.GetValue(node), IEnumerable(Of Processing_instruction)), propertySearchPattern)
                ElseIf (searchableGridColumns.Contains(nodeProperty.Name)) Then
                    propertySearchPattern.Append(String.Format("{0}|", nodeProperty.GetValue(node)))
                End If
            Next

            If (propertySearchPattern.Length > 0) Then propertySearchPattern.Remove(propertySearchPattern.Length - 1, 1)

            Dim objectSearchPattern As New ObjectSearchPattern
            objectSearchPattern.DisplayText = node.Id
            objectSearchPattern.PropertyValuePattern = propertySearchPattern.ToString
            objectSearchPatterns.Add(node.SystemId, objectSearchPattern)
        Next
    End Sub

    Private Sub LoadWireColoursIntoSearchPattern(wireColours As IEnumerable(Of Wire_colour), propertySearchPattern As StringBuilder)
        For Each wireColour As Wire_colour In wireColours
            If (wireColour.Colour_value <> String.Empty) AndAlso (wireColour.Colour_value <> "/NULL") Then
                propertySearchPattern.Append(String.Format("{0}|", wireColour.Colour_value))
            End If
        Next
    End Sub

    Private Sub LoadWiresIntoSearchMapper(kblMapper As KblMapper, objectSearchPatterns As Dictionary(Of String, ObjectSearchPattern), searchableGridColumns As IEnumerable(Of String))
        For Each wireOccurrence As Wire_occurrence In kblMapper.KBLWireList
            Dim propertySearchPattern As New StringBuilder

            For Each wireOccurrenceProperty As PropertyInfo In wireOccurrence.GetType.GetProperties
                If (wireOccurrenceProperty.Name = WirePropertyName.Length_information.ToString) AndAlso (searchableGridColumns.Contains(WirePropertyName.Length_information.ToString)) Then
                    LoadLengthInformationIntoSearchPattern(DirectCast(wireOccurrenceProperty.GetValue(wireOccurrence), IEnumerable(Of Wire_length)), propertySearchPattern)
                ElseIf (wireOccurrenceProperty.Name = WireProtectionPropertyName.Installation_Information.ToString) AndAlso (searchableGridColumns.Contains(WireProtectionPropertyName.Installation_Information.ToString)) Then
                    LoadInstallationInformationIntoSearchPattern(DirectCast(wireOccurrenceProperty.GetValue(wireOccurrence), IEnumerable(Of Installation_instruction)), propertySearchPattern)
                ElseIf (wireOccurrenceProperty.Name = WirePropertyName.Part.ToString) AndAlso (kblMapper.KBLPartMapper.ContainsKey(wireOccurrenceProperty.GetValue(wireOccurrence).ToString)) Then
                    Dim wire As General_wire = DirectCast(kblMapper.KBLPartMapper(wireOccurrenceProperty.GetValue(wireOccurrence).ToString), General_wire)

                    For Each wireProperty As PropertyInfo In wire.GetType.GetProperties
                        If (wireProperty.Name = CommonPropertyName.Alias_Id.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Part_alias_ids.ToString)) Then
                            LoadAliasIdsIntoSearchPattern(DirectCast(wireProperty.GetValue(wire), IEnumerable(Of Alias_identification)), propertySearchPattern)
                        ElseIf (wireProperty.Name = WirePropertyName.Bend_radius.ToString) AndAlso (searchableGridColumns.Contains(WirePropertyName.Bend_radius.ToString)) Then
                            LoadNumericalValueIntoSearchPattern(DirectCast(wireProperty.GetValue(wire), Numerical_value), propertySearchPattern)
                        ElseIf (wireProperty.Name = CablePropertyName.CoverColours.ToString) AndAlso (searchableGridColumns.Contains(WirePropertyName.Core_Colour.ToString)) Then
                            LoadWireColoursIntoSearchPattern(DirectCast(wireProperty.GetValue(wire), IEnumerable(Of Wire_colour)), propertySearchPattern)
                        ElseIf (wireProperty.Name = WirePropertyName.Cross_section_area.ToString) AndAlso (searchableGridColumns.Contains(WirePropertyName.Cross_section_area.ToString)) Then
                            LoadNumericalValueIntoSearchPattern(DirectCast(wireProperty.GetValue(wire), Numerical_value), propertySearchPattern)
                        ElseIf (wireProperty.Name = PartPropertyName.Change.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Change.ToString)) Then
                            LoadChangesIntoSearchPattern(DirectCast(wireProperty.GetValue(wire), IEnumerable(Of Change)), propertySearchPattern)
                        ElseIf (wireProperty.Name = PartPropertyName.Mass_information.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Mass_information.ToString)) Then
                            LoadNumericalValueIntoSearchPattern(DirectCast(wireProperty.GetValue(wire), Numerical_value), propertySearchPattern)
                        ElseIf (wireProperty.Name = PartPropertyName.Material_information.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Material_information.ToString)) Then
                            LoadMaterialInformationIntoSearchPattern(DirectCast(wireProperty.GetValue(wire), Material), propertySearchPattern)
                        ElseIf (wireProperty.Name = WirePropertyName.Outside_diameter.ToString) AndAlso (searchableGridColumns.Contains(WirePropertyName.Outside_diameter.ToString)) Then
                            LoadNumericalValueIntoSearchPattern(DirectCast(wireProperty.GetValue(wire), Numerical_value), propertySearchPattern)
                        ElseIf (wireProperty.Name = WirePropertyName.Processing_information.ToString) AndAlso (searchableGridColumns.Contains(PartPropertyName.Part_processing_information.ToString)) Then
                            LoadProcessingInformationIntoSearchPattern(DirectCast(wireProperty.GetValue(wire), IEnumerable(Of Processing_instruction)), propertySearchPattern)
                        ElseIf (searchableGridColumns.Contains(wireProperty.Name)) OrElse (wireProperty.Name = WirePropertyName.Description.ToString AndAlso searchableGridColumns.Contains(PartPropertyName.Part_description.ToString)) Then
                            propertySearchPattern.Append(String.Format("{0}|", wireProperty.GetValue(wire)))
                        End If
                    Next
                ElseIf (searchableGridColumns.Contains(wireOccurrenceProperty.Name)) Then
                    propertySearchPattern.Append(String.Format("{0}|", wireOccurrenceProperty.GetValue(wireOccurrence)))
                End If
            Next

            If (propertySearchPattern.Length > 0) Then propertySearchPattern.Remove(propertySearchPattern.Length - 1, 1)

            Dim objectSearchPattern As New ObjectSearchPattern

            If (wireOccurrence.Part IsNot Nothing) AndAlso (kblMapper.KBLPartMapper.ContainsKey(wireOccurrence.Part)) Then
                objectSearchPattern.DisplayText = String.Format("{0} [{1}]", wireOccurrence.Wire_number, DirectCast(kblMapper.KBLPartMapper(wireOccurrence.Part), General_wire).Part_number)
            Else
                objectSearchPattern.DisplayText = wireOccurrence.Wire_number
            End If

            objectSearchPattern.PropertyValuePattern = propertySearchPattern.ToString
            objectSearchPatterns.Add(wireOccurrence.SystemId, objectSearchPattern)
        Next
    End Sub

    Private Sub _searchForm_searchEvent(sender As Object, mapperSourceId As String, objectType As String, objectId As String)
        RaiseEvent SearchEvent(Me, mapperSourceId, objectType, objectId)
    End Sub


    Friend Sub CreateSearchFormWithPredefinedSearchString(beginsWithChecked As Boolean, caseSensitiveChecked As Boolean, reloadSearchForm As Boolean, searchString As String)
        If (reloadSearchForm) AndAlso (_searchForm IsNot Nothing) AndAlso (_searchForm.Visible) Then
            _searchForm.Close()
        End If

        CreateSearchForm()

        _searchForm.uckBeginsWith.Checked = beginsWithChecked
        _searchForm.uckCaseSensitive.Checked = caseSensitiveChecked
        _searchForm.txtSearchString.Text = searchString
    End Sub

    Friend Sub InitializeKBLSearchPattern(kblMapper As KblMapper)
        If (_searchPatterns.ContainsKey(kblMapper.Id)) Then
            Exit Sub
        Else
            _searchPatterns.Add(kblMapper.Id, New SearchPattern(kblMapper.HarnessPartNumber, kblMapper.GetHarness.Version))
        End If

        Try
            Dim docSearchPatterns As SearchPattern = _searchPatterns(kblMapper.Id)

            For Each gridAppearance As KeyValuePair(Of KblObjectType, GridAppearance) In _gridAppearances
                docSearchPatterns.Content.Add(gridAppearance.Key, New Dictionary(Of String, ObjectSearchPattern))

                Dim objectSearchPatterns As Dictionary(Of String, ObjectSearchPattern) = docSearchPatterns.Content(gridAppearance.Key)
                Dim searchableGridColumns As New List(Of String)

                For Each gridColumn As GridColumn In gridAppearance.Value.GridTable.GridColumns
                    If (gridColumn.Searchable) AndAlso (gridColumn.Visible) Then
                        searchableGridColumns.Add(gridColumn.KBLPropertyName)
                    End If
                Next

                If (searchableGridColumns.Count <> 0) Then
                    Select Case gridAppearance.Value.GridTable.Type
                        Case KblObjectType.Accessory_occurrence
                            LoadAccessoriesIntoSearchMapper(kblMapper, objectSearchPatterns, searchableGridColumns)
                        Case KblObjectType.Approval
                            LoadApprovalsIntoSearchMapper(kblMapper, objectSearchPatterns, searchableGridColumns)
                        Case KblObjectType.Assembly_part_occurrence
                            LoadAssemblyPartsIntoSearchMapper(kblMapper, objectSearchPatterns, searchableGridColumns)
                        Case KblObjectType.Special_wire_occurrence
                            LoadCablesIntoSearchMapper(kblMapper, objectSearchPatterns, searchableGridColumns)
                        Case KblObjectType.Change_description
                            LoadChangeDescriptionsIntoSearchMapper(kblMapper, objectSearchPatterns, searchableGridColumns)
                        Case KblObjectType.Component_box_occurrence
                            LoadComponentBoxesIntoSearchMapper(kblMapper, objectSearchPatterns, searchableGridColumns)
                        Case KblObjectType.Component_occurrence
                            LoadComponentsIntoSearchMapper(kblMapper, objectSearchPatterns, searchableGridColumns)
                        Case KblObjectType.Connector_occurrence
                            LoadConnectorsIntoSearchMapper(kblMapper, objectSearchPatterns, searchableGridColumns)

                            If gridAppearance.Value?.GridTable?.GridSubTable IsNot Nothing Then
                                docSearchPatterns.Content.TryAdd((gridAppearance.Value?.GridTable?.GridSubTable?.Type).GetValueOrDefault, New Dictionary(Of String, ObjectSearchPattern))
                                objectSearchPatterns = docSearchPatterns.Content((gridAppearance.Value?.GridTable?.GridSubTable?.Type).GetValueOrDefault)
                                searchableGridColumns.Clear()

                                For Each gridColumn As GridColumn In gridAppearance.Value.GridTable.GridSubTable.GridColumns
                                    If (gridColumn.Searchable) AndAlso (gridColumn.Visible) Then
                                        searchableGridColumns.Add(gridColumn.KBLPropertyName)
                                    End If
                                Next
                            End If

                            If (searchableGridColumns.Count <> 0) Then
                                LoadCavitiesIntoSearchMapper(kblMapper, objectSearchPatterns, searchableGridColumns)
                            End If
                        Case KblObjectType.Co_pack_occurrence
                            LoadCoPacksIntoSearchMapper(kblMapper, objectSearchPatterns, searchableGridColumns)
                        Case KblObjectType.Default_dimension_specification
                            LoadDefDimSpecsIntoSearchMapper(kblMapper, objectSearchPatterns, searchableGridColumns)
                        Case KblObjectType.Dimension_specification
                            LoadDimSpecsIntoSearchMapper(kblMapper, objectSearchPatterns, searchableGridColumns)
                        Case KblObjectType.Fixing_occurrence
                            LoadFixingsIntoSearchMapper(kblMapper, objectSearchPatterns, searchableGridColumns)
                        Case KblObjectType.Module
                            LoadModulesIntoSearchMapper(kblMapper, objectSearchPatterns, searchableGridColumns)

                            docSearchPatterns.Content.Add(gridAppearance.Value.GridTable.GridSubTable.Type, New Dictionary(Of String, ObjectSearchPattern))
                            objectSearchPatterns = docSearchPatterns.Content(gridAppearance.Value.GridTable.GridSubTable.Type)
                            searchableGridColumns.Clear()

                            For Each gridColumn As GridColumn In gridAppearance.Value.GridTable.GridSubTable.GridColumns
                                If (gridColumn.Searchable) AndAlso (gridColumn.Visible) Then
                                    searchableGridColumns.Add(gridColumn.KBLPropertyName)
                                End If
                            Next

                            If (searchableGridColumns.Count <> 0) Then
                                LoadModuleChangesIntoSearchMapper(kblMapper, objectSearchPatterns, searchableGridColumns)
                            End If
                        Case KblObjectType.Net
                            For Each gridColumn As GridColumn In gridAppearance.Value.GridTable.GridSubTable.GridColumns
                                If (gridColumn.Searchable) AndAlso (gridColumn.Visible) Then
                                    searchableGridColumns.Add(gridColumn.KBLPropertyName)
                                End If
                            Next

                            LoadNetsIntoSearchMapper(kblMapper, objectSearchPatterns, searchableGridColumns)
                        Case KblObjectType.Segment
                            LoadSegmentsIntoSearchMapper(kblMapper, objectSearchPatterns, searchableGridColumns)

                            docSearchPatterns.Content.Add(gridAppearance.Value.GridTable.GridSubTable.Type, New Dictionary(Of String, ObjectSearchPattern))
                            objectSearchPatterns = docSearchPatterns.Content(gridAppearance.Value.GridTable.GridSubTable.Type)
                            searchableGridColumns.Clear()

                            For Each gridColumn As GridColumn In gridAppearance.Value.GridTable.GridSubTable.GridColumns
                                If (gridColumn.Searchable) AndAlso (gridColumn.Visible) Then
                                    searchableGridColumns.Add(gridColumn.KBLPropertyName)
                                End If
                            Next

                            If (searchableGridColumns.Count <> 0) Then
                                LoadProtection_On_SegmentsIntoSearchMapper(kblMapper, objectSearchPatterns, searchableGridColumns)
                            End If
                        Case KblObjectType.Node
                            LoadVerticesIntoSearchMapper(kblMapper, objectSearchPatterns, searchableGridColumns)

                            docSearchPatterns.Content.Add(gridAppearance.Value.GridTable.GridSubTable.Type, New Dictionary(Of String, ObjectSearchPattern))
                            objectSearchPatterns = docSearchPatterns.Content(gridAppearance.Value.GridTable.GridSubTable.Type)
                            searchableGridColumns.Clear()

                            For Each gridColumn As GridColumn In gridAppearance.Value.GridTable.GridSubTable.GridColumns
                                If (gridColumn.Searchable) AndAlso (gridColumn.Visible) Then
                                    searchableGridColumns.Add(gridColumn.KBLPropertyName)
                                End If
                            Next

                            If (searchableGridColumns.Count <> 0) Then
                                LoadProtectionsOnVerticesIntoSearchMapper(kblMapper, objectSearchPatterns, searchableGridColumns)
                            End If
                        Case KblObjectType.Wire_occurrence
                            LoadWiresIntoSearchMapper(kblMapper, objectSearchPatterns, searchableGridColumns)
                            LoadCoresIntoSearchMapper(kblMapper, objectSearchPatterns, searchableGridColumns)
                    End Select
                End If
            Next
        Catch ex As Exception
            If DebugEx.IsDebug Then
                Throw ' HINT: directly throw exception on debug, otherwise show a messagebox
            Else
                MessageBoxEx.ShowError(String.Format(ErrorStrings.SearchMachine_InitPatternFailed_Msg, vbCrLf, ex.Message))
            End If
        End Try
    End Sub

    Friend Function IsSearchFormVisible() As Boolean
        Return (_searchForm?.Visible).GetValueOrDefault
    End Function

    Friend Sub RemoveKBLSearchPattern(kblMapper As KblMapper)
        If kblMapper IsNot Nothing Then
            _searchPatterns.Remove(kblMapper.Id)
        End If
    End Sub

    Friend Sub UpdateInactiveObjects(filterInfo As RowFilterInfo)
        If (Not _inactiveObjects.ContainsKey(filterInfo.KBL.Id)) AndAlso (filterInfo.InactiveObjects IsNot Nothing) Then
            _inactiveObjects.Add(filterInfo.KBL.Id, filterInfo.InactiveObjects)
        ElseIf _inactiveObjects.Remove(filterInfo.KBL.Id) Then
            If (filterInfo.InactiveObjects IsNot Nothing) Then
                _inactiveObjects.Add(filterInfo.KBL.Id, filterInfo.InactiveObjects)
            End If
        End If
    End Sub

    Friend Sub VisibleSearchForm(ByVal visible As Boolean)
        If (_searchForm IsNot Nothing) AndAlso (_searchForm.IsDisposed <> True) Then
            _searchForm.Visible = visible
        End If
    End Sub


    Public ReadOnly Property BeginsWithEnabled() As Boolean
        Get
            If (_searchForm IsNot Nothing) Then
                Return _searchForm.uckBeginsWith.Checked
            Else
                Return False
            End If
        End Get
    End Property

    Public ReadOnly Property CaseSensitiveEnabled() As Boolean
        Get
            If (_searchForm IsNot Nothing) Then
                Return _searchForm.uckCaseSensitive.Checked
            Else
                Return False
            End If
        End Get
    End Property

    Public ReadOnly Property CurrentSearchString() As String
        Get
            If (_searchForm IsNot Nothing) Then
                Return _searchForm.txtSearchString.Text
            Else
                Return String.Empty
            End If
        End Get
    End Property

End Class
