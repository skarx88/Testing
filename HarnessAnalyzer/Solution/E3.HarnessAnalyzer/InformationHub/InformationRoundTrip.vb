Imports Infragistics.Win.UltraWinGrid

Public Class InformationRoundTrip
    Implements IDisposable

    Private _kblMapper As KblMapper
    Private _kblIds As List(Of String)
    Private _redliningInformation As RedliningInformation
    Private _selectedKblIds As List(Of String)
    Private _tabName As String

    Public Sub New(kblMapper As KblMapper, redliningInformation As RedliningInformation, selectedKblIds As List(Of String), tabName As String)
        _kblMapper = kblMapper
        _redliningInformation = redliningInformation
        _selectedKblIds = selectedKblIds
        _tabName = tabName
    End Sub

    Public Sub New(kblMapper As KblMapper, redliningInformation As RedliningInformation, selectedKblIds As List(Of String), sourceGrid As UltraGridBase)
        _kblMapper = kblMapper
        _redliningInformation = redliningInformation
        _selectedKblIds = selectedKblIds

        Dim grid_obj_type As KblObjectType = InformationHubUtils.GetKblObjectType(sourceGrid)
        If grid_obj_type <> KblObjectType.Undefined Then
            _tabName = InformationHubUtils.GetTabNameFromKblObjectType(grid_obj_type).ToString
        End If
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then

            End If

            _kblMapper = Nothing
            _kblIds = Nothing
            _redliningInformation = Nothing
            _selectedKblIds = Nothing
            _tabName = Nothing
        End If
        Me.disposedValue = True
    End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

    Friend Function GetCorrespondingKBLIds() As List(Of String)
        _kblIds = New List(Of String)

        For Each selectedKblId As String In _selectedKblIds
            If (_kblMapper.KBLOccurrenceMapper.ContainsKey(selectedKblId)) Then
                Dim occurrenceObject As Object = _kblMapper.KBLOccurrenceMapper(selectedKblId)

                If (TypeOf occurrenceObject Is Accessory_occurrence) Then
                    GetCorrespondingIdsFromSelectedAccessory(DirectCast(occurrenceObject, Accessory_occurrence))
                ElseIf (TypeOf occurrenceObject Is Approval) Then
                    GetCorrespondingIdsFromSelectedApproval(DirectCast(occurrenceObject, Approval))
                ElseIf (TypeOf occurrenceObject Is Assembly_part_occurrence) Then
                    GetCorrespondingIdsFromSelectedAssemblyPart(DirectCast(occurrenceObject, Assembly_part_occurrence), DirectCast(_kblMapper.KBLPartMapper(DirectCast(occurrenceObject, Assembly_part_occurrence).Part), Assembly_part))
                ElseIf (TypeOf occurrenceObject Is Cavity_occurrence) Then
                    GetCorrespondingIdsFromSelectedCavity(DirectCast(occurrenceObject, Cavity_occurrence))
                ElseIf (TypeOf occurrenceObject Is Change_description) Then
                    GetCorrespondingIdsFromSelectedChangeDescription(DirectCast(occurrenceObject, Change_description))
                ElseIf (TypeOf occurrenceObject Is Component_box_occurrence) Then
                    GetCorrespondingIdsFromSelectedComponentBox(DirectCast(occurrenceObject, Component_box_occurrence))
                ElseIf (TypeOf occurrenceObject Is Component_occurrence) Then
                    GetCorrespondingIdsFromSelectedComponent(DirectCast(occurrenceObject, Component_occurrence))
                ElseIf (TypeOf occurrenceObject Is Connector_occurrence) Then
                    GetCorrespondingIdsFromSelectedConnector(DirectCast(occurrenceObject, Connector_occurrence))
                ElseIf (TypeOf occurrenceObject Is Contact_point) Then
                    GetCorrespondingIdsFromSelectedContactPoint(DirectCast(occurrenceObject, Contact_point))
                    If (_tabName = TabNames.tabVertices.ToString) Then
                        Exit For
                    End If
                ElseIf (TypeOf occurrenceObject Is Core_occurrence) Then
                    GetCorrespondingIdsFromSelectedCore(DirectCast(occurrenceObject, Core_occurrence))
                ElseIf (TypeOf occurrenceObject Is Co_pack_occurrence) Then
                    GetCorrespondingIdsFromSelectedCoPack(DirectCast(occurrenceObject, Co_pack_occurrence))
                ElseIf (TypeOf occurrenceObject Is Dimension_specification) Then
                    GetCorrespondingIdsFromSelectedDimSpec(DirectCast(occurrenceObject, Dimension_specification))
                ElseIf (TypeOf occurrenceObject Is Fixing_occurrence) Then
                    GetCorrespondingIdsFromSelectedFixing(DirectCast(occurrenceObject, Fixing_occurrence))
                ElseIf (TypeOf occurrenceObject Is [Module]) Then
                    GetCorrespondingIdsFromSelectedModule(DirectCast(occurrenceObject, [Module]))
                ElseIf (TypeOf occurrenceObject Is Node) Then
                    GetCorrespondingIdsFromSelectedVertex(DirectCast(occurrenceObject, Node))
                ElseIf (TypeOf occurrenceObject Is Segment) Then
                    GetCorrespondingIdsFromSelectedSegment(DirectCast(occurrenceObject, Segment))
                ElseIf (TypeOf occurrenceObject Is Special_wire_occurrence) Then
                    GetCorrespondingIdsFromSelectedCable(DirectCast(occurrenceObject, Special_wire_occurrence))
                ElseIf (TypeOf occurrenceObject Is Wire_occurrence) Then
                    GetCorrespondingIdsFromSelectedWire(DirectCast(occurrenceObject, Wire_occurrence))
                ElseIf (TypeOf occurrenceObject Is Wire_protection_occurrence) Then
                    GetCorrespondingIdsFromSelectedWireProtection(DirectCast(occurrenceObject, Wire_protection_occurrence))
                End If
            ElseIf (_kblMapper.KBLNetMapper.ContainsKey(selectedKblId)) Then
                For Each connection As Connection In _kblMapper.KBLNetMapper(selectedKblId)
                    If (_kblMapper.KBLOccurrenceMapper.ContainsKey(connection.Wire)) Then
                        If (_tabName = TabNames.tabChangeDescriptions.ToString) Then
                            GetCorrespondingChangeDescriptionIdsFromSelectedObjectId(connection.SystemId)
                        Else
                            Dim kblWireCore As IKblWireCoreOccurrence = _kblMapper.GetWireOrCoreOccurrence(connection.Wire)
                            If kblWireCore IsNot Nothing Then
                                Select Case kblWireCore.ObjectType
                                    Case KblObjectType.Core_occurrence
                                        GetCorrespondingIdsFromSelectedCore(DirectCast(kblWireCore, Core_occurrence))
                                    Case KblObjectType.Wire_occurrence
                                        GetCorrespondingIdsFromSelectedWire(DirectCast(kblWireCore, Wire_occurrence))
                                End Select
                            End If
                        End If
                    End If
                Next
            ElseIf (_tabName = TabNames.tabRedlinings.ToString) Then
                For Each redlining As Redlining In _redliningInformation.Redlinings.Where(Function(rdlining) rdlining.ObjectId = selectedKblId)
                    If Not _kblIds.Contains(redlining.ID) Then
                        _kblIds.Add(redlining.ID)
                    End If
                Next
            End If
        Next

        Return _kblIds
    End Function

    Private Sub GetCorrespondingChangeDescriptionIdsFromSelectedObjectId(objectId As String)
        If (_kblMapper.KBLOccurrenceMapper.ContainsKey(objectId)) Then
            For Each changeDescription As Change_description In _kblMapper.GetChangeDescriptions
                For Each changedElementId As String In HarnessAnalyzer.[Shared].Utilities.GetIdrefs(changeDescription.Changed_elements)
                    If (changedElementId = objectId) Then
                        _kblIds.Add(changeDescription.SystemId)
                    End If
                Next
            Next
        End If
    End Sub

    Private Sub GetCorrespondingComponentBoxIdsFromSelectedObjectId(objectId As String)
        If (_kblMapper.KBLOccurrenceMapper.ContainsKey(objectId)) Then
            For Each componentBoxOccurrence As Component_box_occurrence In _kblMapper.GetComponentBoxOccurrences
                For Each kblId As String In HarnessAnalyzer.[Shared].Utilities.GetIdrefs(componentBoxOccurrence.Reference_element)
                    If ((kblId = objectId) AndAlso (Not _kblIds.Contains(componentBoxOccurrence.SystemId))) Then
                        _kblIds.Add(componentBoxOccurrence.SystemId)
                    End If
                Next
            Next
        End If
    End Sub

    Private Sub GetCorrespondingIdsFromSelectedAccessory(accessoryOccurrence As Accessory_occurrence)
        If (_tabName = TabNames.tabAssemblyParts.ToString) Then
            If (TypeOf accessoryOccurrence Is Specified_accessory_occurrence) Then
                _kblIds.AddRange(DirectCast(accessoryOccurrence, Specified_accessory_occurrence).Related_assembly.SplitSpace)
            End If
        ElseIf (_tabName = TabNames.tabChangeDescriptions.ToString) Then
            GetCorrespondingChangeDescriptionIdsFromSelectedObjectId(accessoryOccurrence.SystemId)
        ElseIf (_tabName = TabNames.tabComponentBoxes.ToString) Then
            GetCorrespondingComponentBoxIdsFromSelectedObjectId(accessoryOccurrence.SystemId)
        ElseIf (_tabName = TabNames.tabConnectors.ToString) Then
            For Each kblId As String In HarnessAnalyzer.[Shared].Utilities.GetIdrefs(accessoryOccurrence.Reference_element)
                If (_kblMapper.KBLOccurrenceMapper.ContainsKey(kblId)) AndAlso (TypeOf _kblMapper.KBLOccurrenceMapper(kblId) Is Connector_occurrence) Then
                    _kblIds.Add(kblId)
                End If
            Next
        ElseIf (_tabName = TabNames.tabCoPacks.ToString) Then
            For Each kblId As String In HarnessAnalyzer.[Shared].Utilities.GetIdrefs(accessoryOccurrence.Reference_element)
                If (_kblMapper.KBLOccurrenceMapper.ContainsKey(kblId)) AndAlso (TypeOf _kblMapper.KBLOccurrenceMapper(kblId) Is Co_pack_occurrence) Then
                    _kblIds.Add(kblId)
                End If
            Next
        ElseIf (_tabName = TabNames.tabDimSpecs.ToString) Then
            For Each dimensionSpecification As Dimension_specification In _kblMapper.GetDimensionSpecifications.Where(Function(dimSpec) (dimSpec.Origin IsNot Nothing AndAlso dimSpec.Origin = accessoryOccurrence.SystemId) OrElse (dimSpec.Target IsNot Nothing AndAlso dimSpec.Target = accessoryOccurrence.SystemId))
                If (Not _kblIds.Contains(dimensionSpecification.SystemId)) Then _kblIds.Add(dimensionSpecification.SystemId)
            Next
        ElseIf (_tabName = TabNames.tabModules.ToString) Then
            GetCorrespondingModuleIdsFromSelectedObjectId(accessoryOccurrence.SystemId)
        ElseIf (_tabName = TabNames.tabRedlinings.ToString) Then
            For Each redlining As Redlining In _redliningInformation.Redlinings.Where(Function(rdlining) rdlining.ObjectId = accessoryOccurrence.SystemId)
                If (Not _kblIds.Contains(redlining.ID)) Then _kblIds.Add(redlining.ID)
            Next
        ElseIf (_tabName = TabNames.tabFixings.ToString) Then
            For Each kblId As String In HarnessAnalyzer.[Shared].Utilities.GetIdrefs(accessoryOccurrence.Reference_element)
                If (_kblMapper.KBLOccurrenceMapper.ContainsKey(kblId)) AndAlso (TypeOf _kblMapper.KBLOccurrenceMapper(kblId) Is Fixing_occurrence) Then
                    _kblIds.Add(kblId)
                End If
            Next
        End If
    End Sub

    Private Sub GetCorrespondingIdsFromSelectedApproval(approval As Approval)
        If (_tabName = TabNames.tabModules.ToString) Then
            If (_kblMapper.GetHarness.GetModule(approval.Is_applied_to) IsNot Nothing) Then
                If (Not _kblIds.Contains(approval.Is_applied_to)) Then
                    _kblIds.Add(approval.Is_applied_to)
                End If
            End If
        End If
    End Sub

    Private Sub GetCorrespondingIdsFromSelectedAssemblyPart(assemblyPartOccurrence As Assembly_part_occurrence, assemblyPart As Assembly_part)
        If (_tabName = TabNames.tabAccessories.ToString) AndAlso (assemblyPart IsNot Nothing) AndAlso (assemblyPart.Accessory_occurrence IsNot Nothing) Then
            For Each accessoryOccurrence As Specified_accessory_occurrence In _kblMapper.GetAccessoryOccurrences.Where(Function(acc) TypeOf acc Is Specified_accessory_occurrence)
                If (accessoryOccurrence.Related_assembly.SplitSpace.Contains(assemblyPartOccurrence.SystemId)) Then
                    _kblIds.Add(accessoryOccurrence.SystemId)
                End If
            Next
        ElseIf (_tabName = TabNames.tabCables.ToString) AndAlso (assemblyPart IsNot Nothing) AndAlso (assemblyPart.General_wire_occurrence IsNot Nothing) Then
            For Each wireOccurrence As Specified_special_wire_occurrence In _kblMapper.GetGeneralWireOccurrences.Where(Function(wir) TypeOf wir Is Specified_special_wire_occurrence)
                If (wireOccurrence.Related_assembly.SplitSpace.Contains(assemblyPartOccurrence.SystemId)) Then
                    _kblIds.Add(wireOccurrence.SystemId)
                End If
            Next
        ElseIf (_tabName = TabNames.tabChangeDescriptions.ToString) Then
            GetCorrespondingChangeDescriptionIdsFromSelectedObjectId(assemblyPartOccurrence.SystemId)
        ElseIf (_tabName = TabNames.tabComponentBoxes.ToString) Then
            GetCorrespondingComponentBoxIdsFromSelectedObjectId(assemblyPartOccurrence.SystemId)
        ElseIf (_tabName = TabNames.tabComponents.ToString) AndAlso (assemblyPart IsNot Nothing) AndAlso (assemblyPart.Component_occurrence IsNot Nothing) Then
            For Each componentOccurrence As Specified_component_occurrence In _kblMapper.GetComponentOccurrences.Where(Function(comp) TypeOf comp Is Specified_component_occurrence)
                If (componentOccurrence.Related_assembly.SplitSpace.Contains(assemblyPartOccurrence.SystemId)) Then
                    _kblIds.Add(componentOccurrence.SystemId)
                End If
            Next
        ElseIf (_tabName = TabNames.tabConnectors.ToString) AndAlso (assemblyPart IsNot Nothing) AndAlso (assemblyPart.Connector_occurrence IsNot Nothing) Then
            For Each connectorOccurrence As Specified_connector_occurrence In _kblMapper.GetConnectorOccurrences.Where(Function(conn) TypeOf conn Is Specified_connector_occurrence)
                If (connectorOccurrence.Related_assembly.SplitSpace.Contains(assemblyPartOccurrence.SystemId)) Then
                    _kblIds.Add(connectorOccurrence.SystemId)
                End If
            Next
        ElseIf (_tabName = TabNames.tabCoPacks.ToString) AndAlso (assemblyPart IsNot Nothing) AndAlso (assemblyPart.Co_pack_occurrence IsNot Nothing) Then
            For Each coPackOccurrence As Specified_co_pack_occurrence In _kblMapper.GetHarness.Co_pack_occurrence.Where(Function(cop) TypeOf cop Is Specified_co_pack_occurrence)
                If (coPackOccurrence.Related_assembly.SplitSpace.Contains(assemblyPartOccurrence.SystemId)) Then
                    _kblIds.Add(coPackOccurrence.SystemId)
                End If
            Next
        ElseIf (_tabName = TabNames.tabDimSpecs.ToString) Then
            For Each dimensionSpecification As Dimension_specification In _kblMapper.GetDimensionSpecifications.Where(Function(dimSpec) (dimSpec.Origin IsNot Nothing AndAlso dimSpec.Origin = assemblyPartOccurrence.SystemId) OrElse (dimSpec.Target IsNot Nothing AndAlso dimSpec.Target = assemblyPartOccurrence.SystemId))
                If (Not _kblIds.Contains(dimensionSpecification.SystemId)) Then _kblIds.Add(dimensionSpecification.SystemId)
            Next
        ElseIf (_tabName = TabNames.tabFixings.ToString) AndAlso (assemblyPart IsNot Nothing) AndAlso (assemblyPart.Fixing_occurrence IsNot Nothing) Then
            For Each fixingOccurrence As Specified_fixing_occurrence In _kblMapper.GetFixingOccurrences.Where(Function(fix) TypeOf fix Is Specified_fixing_occurrence)
                If (fixingOccurrence.Related_assembly.SplitSpace.Contains(assemblyPartOccurrence.SystemId)) Then
                    _kblIds.Add(fixingOccurrence.SystemId)
                End If
            Next
        ElseIf (_tabName = TabNames.tabModules.ToString) Then
            GetCorrespondingModuleIdsFromSelectedObjectId(assemblyPartOccurrence.SystemId)
        ElseIf (_tabName = TabNames.tabNets.ToString) AndAlso (assemblyPart IsNot Nothing) AndAlso (assemblyPart.Connection IsNot Nothing) Then
            For Each connection As Connection In assemblyPart.Connection
                For Each wireOccurrence As Specified_wire_occurrence In _kblMapper.GetGeneralWireOccurrences.Where(Function(wir) TypeOf wir Is Specified_wire_occurrence)
                    If (wireOccurrence.Related_assembly.SplitSpace.Contains(connection.Wire)) Then
                        _kblIds.Add(wireOccurrence.SystemId)
                    End If
                Next
            Next
        ElseIf (_tabName = TabNames.tabRedlinings.ToString) Then
            For Each redlining As Redlining In _redliningInformation.Redlinings.Where(Function(rdlining) rdlining.ObjectId = assemblyPartOccurrence.SystemId)
                If (Not _kblIds.Contains(redlining.ID)) Then
                    _kblIds.Add(redlining.ID)
                End If
            Next
        ElseIf (_tabName = TabNames.tabSegments.ToString) AndAlso (assemblyPart IsNot Nothing) AndAlso (assemblyPart.Wire_protection_occurrence IsNot Nothing) Then
            For Each wireProtectionOccurrence As Specified_wire_protection_occurrence In _kblMapper.GetHarness.Wire_protection_occurrence.Where(Function(prot) TypeOf prot Is Specified_wire_protection_occurrence)
                If (wireProtectionOccurrence.Related_assembly.SplitSpace.Contains(assemblyPartOccurrence.SystemId)) Then
                    For Each segment As Segment In _kblMapper.GetSegments.Where(Function(seg) seg.Protection_area IsNot Nothing AndAlso seg.Protection_area.Any(Function(protectionArea) protectionArea.Associated_protection = wireProtectionOccurrence.SystemId))
                        If (Not _kblIds.Contains(segment.SystemId)) Then
                            _kblIds.Add(segment.SystemId)
                        End If
                    Next
                End If
            Next
        ElseIf (_tabName = TabNames.tabVertices.ToString) AndAlso (assemblyPart IsNot Nothing) AndAlso (assemblyPart.Wire_protection_occurrence IsNot Nothing) Then
            For Each wireProtectionOccurrence As Specified_wire_protection_occurrence In _kblMapper.GetHarness.Wire_protection_occurrence.Where(Function(prot) TypeOf prot Is Specified_wire_protection_occurrence)
                If (wireProtectionOccurrence.Related_assembly.SplitSpace.Contains(assemblyPartOccurrence.SystemId)) Then
                    For Each node As Node In _kblMapper.GetNodes.Where(Function(n) n.Referenced_components IsNot Nothing AndAlso n.Referenced_components.SplitSpace.ToList.Contains(wireProtectionOccurrence.SystemId))
                        If (Not _kblIds.Contains(node.SystemId)) Then
                            _kblIds.Add(node.SystemId)
                        End If
                    Next
                End If
            Next
        ElseIf (_tabName = TabNames.tabWires.ToString) AndAlso (assemblyPart IsNot Nothing) AndAlso (assemblyPart.General_wire_occurrence IsNot Nothing) Then
            For Each wireOccurrence As Specified_wire_occurrence In _kblMapper.GetGeneralWireOccurrences.Where(Function(wir) TypeOf wir Is Specified_wire_occurrence)
                If (wireOccurrence.Related_assembly.SplitSpace.Contains(assemblyPartOccurrence.SystemId)) Then
                    _kblIds.Add(wireOccurrence.SystemId)
                End If
            Next
        End If
    End Sub

    Private Sub GetCorrespondingIdsFromSelectedCable(cableOccurrence As Special_wire_occurrence)
        If (_tabName = TabNames.tabAssemblyParts.ToString) Then
            If (TypeOf cableOccurrence Is Specified_special_wire_occurrence) Then _kblIds.AddRange(DirectCast(cableOccurrence, Specified_special_wire_occurrence).Related_assembly.SplitSpace)
        ElseIf (_tabName = TabNames.tabChangeDescriptions.ToString) Then
            GetCorrespondingChangeDescriptionIdsFromSelectedObjectId(cableOccurrence.SystemId)
        ElseIf (_tabName = TabNames.tabComponentBoxes.ToString) Then
            GetCorrespondingComponentBoxIdsFromSelectedObjectId(cableOccurrence.SystemId)
        ElseIf (_tabName = TabNames.tabConnectors.ToString) Then
            For Each coreOccurrence As Core_occurrence In cableOccurrence.Core_occurrence
                For Each connection As Connection In _kblMapper.GetConnections
                    If (connection.Wire = coreOccurrence.SystemId) Then
                        For Each extremity As Extremity In connection.Extremities
                            _kblIds.Add(extremity.Contact_point)
                        Next

                        Exit For
                    End If
                Next
            Next
        ElseIf (_tabName = TabNames.tabModules.ToString) Then
            GetCorrespondingModuleIdsFromSelectedObjectId(cableOccurrence.SystemId)
        ElseIf (_tabName = TabNames.tabNets.ToString) OrElse (_tabName = TabNames.tabWires.ToString) Then
            For Each coreOccurrence As Core_occurrence In cableOccurrence.Core_occurrence
                _kblIds.TryAdd(coreOccurrence.SystemId)
            Next
        ElseIf (_tabName = TabNames.tabRedlinings.ToString) Then
            For Each redlining As Redlining In _redliningInformation.Redlinings.Where(Function(rdlining) rdlining.ObjectId = cableOccurrence.SystemId)
                _kblIds.TryAdd(redlining.ID)
            Next
        ElseIf (_tabName = TabNames.tabSegments.ToString) Then
            For Each coreOccurrence As Core_occurrence In cableOccurrence.Core_occurrence
                For Each segment As Segment In _kblMapper.KBLWireSegmentMapper.GetSegmentsOrEmpty(coreOccurrence.SystemId)
                    _kblIds.TryAdd(segment.SystemId)
                Next
            Next
        ElseIf (_tabName = TabNames.tabVertices.ToString) Then
            For Each coreOccurrence As Core_occurrence In cableOccurrence.Core_occurrence
                For Each segment As Segment In _kblMapper.KBLWireSegmentMapper.GetSegmentsOrEmpty(coreOccurrence.SystemId)
                    _kblIds.TryAdd(segment.End_node)
                    _kblIds.TryAdd(segment.Start_node)
                Next
            Next
        End If
    End Sub

    Private Sub GetCorrespondingIdsFromSelectedCavity(cavityOccurrence As Cavity_occurrence)
        If (_tabName = TabNames.tabAssemblyParts.ToString) Then
            If (cavityOccurrence.Associated_plug IsNot Nothing) AndAlso (cavityOccurrence.Associated_plug <> String.Empty) Then
                Dim plugOccurrence As Cavity_plug_occurrence = TryCast(_kblMapper.KBLOccurrenceMapper(cavityOccurrence.Associated_plug), Cavity_plug_occurrence)
                If (plugOccurrence IsNot Nothing) AndAlso (TypeOf plugOccurrence Is Specified_cavity_plug_occurrence) Then _kblIds.AddRange(DirectCast(plugOccurrence, Specified_cavity_plug_occurrence).Related_assembly.SplitSpace)
            ElseIf (_kblMapper.KBLCavityContactPointMapper.ContainsKey(cavityOccurrence.SystemId)) Then
                For Each contactPoint As Contact_point In _kblMapper.KBLCavityContactPointMapper(cavityOccurrence.SystemId)
                    For Each associatedPartId As String In HarnessAnalyzer.[Shared].Utilities.GetIdrefs(contactPoint.Associated_parts)
                        If (TypeOf _kblMapper.KBLOccurrenceMapper(associatedPartId) Is Cavity_seal_occurrence) Then
                            Dim sealOccurrence As Cavity_seal_occurrence = TryCast(_kblMapper.KBLOccurrenceMapper(associatedPartId), Cavity_seal_occurrence)
                            If (sealOccurrence IsNot Nothing) AndAlso (TypeOf sealOccurrence Is Specified_cavity_seal_occurrence) Then _kblIds.AddRange(DirectCast(sealOccurrence, Specified_cavity_seal_occurrence).Related_assembly.SplitSpace)
                        ElseIf (TypeOf _kblMapper.KBLOccurrenceMapper(associatedPartId) Is Terminal_occurrence) Then
                            Dim terminalOccurrence As Terminal_occurrence = TryCast(_kblMapper.KBLOccurrenceMapper(associatedPartId), Terminal_occurrence)
                            If (terminalOccurrence IsNot Nothing) AndAlso (TypeOf terminalOccurrence Is Specified_terminal_occurrence) Then _kblIds.AddRange(DirectCast(terminalOccurrence, Specified_terminal_occurrence).Related_assembly.SplitSpace)
                        End If
                    Next
                Next
            End If
        ElseIf (_tabName = TabNames.tabChangeDescriptions.ToString) Then
            If (cavityOccurrence.Associated_plug IsNot Nothing) AndAlso (cavityOccurrence.Associated_plug <> String.Empty) Then
                GetCorrespondingChangeDescriptionIdsFromSelectedObjectId(cavityOccurrence.Associated_plug)
            ElseIf (_kblMapper.KBLCavityContactPointMapper.ContainsKey(cavityOccurrence.SystemId)) Then
                For Each contactPoint As Contact_point In _kblMapper.KBLCavityContactPointMapper(cavityOccurrence.SystemId)
                    For Each associatedPartId As String In HarnessAnalyzer.[Shared].Utilities.GetIdrefs(contactPoint.Associated_parts)
                        GetCorrespondingChangeDescriptionIdsFromSelectedObjectId(associatedPartId)
                    Next
                Next
            End If
        ElseIf (_tabName = TabNames.tabModules.ToString) Then
            If (cavityOccurrence.Associated_plug IsNot Nothing) AndAlso (cavityOccurrence.Associated_plug <> String.Empty) Then GetCorrespondingModuleIdsFromSelectedObjectId(cavityOccurrence.Associated_plug)
        ElseIf (_tabName = TabNames.tabRedlinings.ToString) Then
            For Each redlining As Redlining In _redliningInformation.Redlinings.Where(Function(rdlining) rdlining.ObjectId = cavityOccurrence.SystemId)
                If (Not _kblIds.Contains(redlining.ID)) Then _kblIds.Add(redlining.ID)
            Next
        End If
    End Sub

    Private Sub GetCorrespondingIdsFromSelectedChangeDescription(changeDescription As Change_description)
        If (changeDescription.Changed_elements IsNot Nothing) AndAlso (changeDescription.Changed_elements <> String.Empty) Then
            Dim changedElementIds As List(Of String) = changeDescription.Changed_elements.SplitSpace.ToList

            If (_tabName = TabNames.tabAccessories.ToString) Then
                For Each accessoryOcc As Accessory_occurrence In _kblMapper.GetAccessoryOccurrences.Where(Function(accOcc) changedElementIds.Contains(accOcc.SystemId))
                    _kblIds.Add(accessoryOcc.SystemId)
                Next
            ElseIf (_tabName = TabNames.tabAssemblyParts.ToString) Then
                For Each assemblyPartOcc As Assembly_part_occurrence In _kblMapper.GetHarness.Assembly_part_occurrence.Where(Function(assPartOcc) changedElementIds.Contains(assPartOcc.SystemId))
                    _kblIds.Add(assemblyPartOcc.SystemId)
                Next
            ElseIf (_tabName = TabNames.tabCables.ToString) OrElse (_tabName = TabNames.tabWires.ToString) Then
                For Each generalWireOcc As General_wire_occurrence In _kblMapper.GetGeneralWireOccurrences.Where(Function(genWireOcc) changedElementIds.Contains(genWireOcc.SystemId))
                    _kblIds.Add(generalWireOcc.SystemId)
                Next
            ElseIf (_tabName = TabNames.tabComponentBoxes.ToString) Then
                For Each componentBoxOcc As Component_box_occurrence In _kblMapper.GetHarness.Component_box_occurrence.Where(Function(compBoxOcc) changedElementIds.Contains(compBoxOcc.SystemId))
                    _kblIds.Add(componentBoxOcc.SystemId)
                Next
            ElseIf (_tabName = TabNames.tabComponents.ToString) Then
                For Each componentOcc As Component_occurrence In _kblMapper.GetComponentOccurrences.Where(Function(compOcc) changedElementIds.Contains(compOcc.SystemId))
                    _kblIds.Add(componentOcc.SystemId)
                Next
            ElseIf (_tabName = TabNames.tabConnectors.ToString) Then
                For Each cavityPlugOcc As Cavity_plug_occurrence In _kblMapper.GetHarness.Cavity_plug_occurrence.Where(Function(cavPlugOcc) changedElementIds.Contains(cavPlugOcc.SystemId))
                    _kblIds.Add(cavityPlugOcc.SystemId)
                Next

                For Each cavitySealOcc As Cavity_seal_occurrence In _kblMapper.GetCavitySealOccurrences.Where(Function(cavSealOcc) changedElementIds.Contains(cavSealOcc.SystemId))
                    _kblIds.Add(cavitySealOcc.SystemId)
                Next

                For Each connectorOcc As Connector_occurrence In _kblMapper.GetConnectorOccurrences.Where(Function(conOcc) changedElementIds.Contains(conOcc.SystemId))
                    _kblIds.Add(connectorOcc.SystemId)
                Next

                For Each terminalOcc As Special_terminal_occurrence In _kblMapper.GetHarness.Special_terminal_occurrence.Where(Function(termOcc) changedElementIds.Contains(termOcc.SystemId))
                    _kblIds.Add(terminalOcc.SystemId)
                Next

                For Each terminalOcc As Terminal_occurrence In _kblMapper.GetHarness.Terminal_occurrence.Where(Function(termOcc) changedElementIds.Contains(termOcc.SystemId))
                    _kblIds.Add(terminalOcc.SystemId)
                Next
            ElseIf (_tabName = TabNames.tabCoPacks.ToString) Then
                For Each coPackOcc As Co_pack_occurrence In _kblMapper.GetHarness.Co_pack_occurrence.Where(Function(coPkOcc) changedElementIds.Contains(coPkOcc.SystemId))
                    _kblIds.Add(coPackOcc.SystemId)
                Next
            ElseIf (_tabName = TabNames.tabFixings.ToString) Then
                For Each fixingOcc As Fixing_occurrence In _kblMapper.GetFixingOccurrences.Where(Function(fixOcc) changedElementIds.Contains(fixOcc.SystemId))
                    _kblIds.Add(fixingOcc.SystemId)
                Next
            ElseIf (_tabName = TabNames.tabModules.ToString) Then
                For Each [module] As [Module] In _kblMapper.GetModules.Where(Function(m) m.Module_configuration IsNot Nothing AndAlso changedElementIds.Contains(m.Module_configuration.SystemId))
                    _kblIds.Add([module].SystemId)
                Next
            ElseIf (_tabName = TabNames.tabNets.ToString) Then
                For Each connection As Connection In _kblMapper.GetConnections.Where(Function(con) changedElementIds.Contains(con.SystemId))
                    _kblIds.Add(connection.Wire)
                Next
            ElseIf (_tabName = TabNames.tabRedlinings.ToString) Then
                For Each redlining As Redlining In _redliningInformation.Redlinings.Where(Function(rdlining) rdlining.ObjectId = changeDescription.SystemId)
                    If (Not _kblIds.Contains(redlining.ID)) Then _kblIds.Add(redlining.ID)
                Next
            ElseIf (_tabName = TabNames.tabSegments.ToString) Then
                For Each segment As Segment In _kblMapper.GetSegments.Where(Function(seg) changedElementIds.Contains(seg.SystemId))
                    _kblIds.Add(segment.SystemId)
                Next

                For Each wireProtectionOcc As Wire_protection_occurrence In _kblMapper.GetHarness.Wire_protection_occurrence.Where(Function(protectionOcc) changedElementIds.Contains(protectionOcc.SystemId))
                    _kblIds.Add(wireProtectionOcc.SystemId)
                Next
            ElseIf (_tabName = TabNames.tabVertices.ToString) Then
                For Each node As Node In _kblMapper.GetNodes.Where(Function(n) changedElementIds.Contains(n.SystemId))
                    _kblIds.Add(node.SystemId)
                Next

                For Each wireProtectionOcc As Wire_protection_occurrence In _kblMapper.GetHarness.Wire_protection_occurrence.Where(Function(protectionOcc) changedElementIds.Contains(protectionOcc.SystemId))
                    _kblIds.Add(wireProtectionOcc.SystemId)
                Next
            End If
        End If
    End Sub

    Private Sub GetCorrespondingIdsFromSelectedComponent(componentOccurrence As Component_occurrence)
        If (_tabName = TabNames.tabAssemblyParts.ToString) Then
            If (TypeOf componentOccurrence Is Specified_component_occurrence) Then _kblIds.AddRange(DirectCast(componentOccurrence, Specified_component_occurrence).Related_assembly.SplitSpace)
        ElseIf (_tabName = TabNames.tabChangeDescriptions.ToString) Then
            GetCorrespondingChangeDescriptionIdsFromSelectedObjectId(componentOccurrence.SystemId)
        ElseIf (_tabName = TabNames.tabComponentBoxes.ToString) Then
            GetCorrespondingComponentBoxIdsFromSelectedObjectId(componentOccurrence.SystemId)
        ElseIf (_tabName = TabNames.tabConnectors.ToString) Then
            For Each kblId As String In HarnessAnalyzer.[Shared].Utilities.GetIdrefs(componentOccurrence.Mounting)
                _kblIds.Add(kblId)
            Next
        ElseIf (_tabName = TabNames.tabModules.ToString) Then
            GetCorrespondingModuleIdsFromSelectedObjectId(componentOccurrence.SystemId)
        ElseIf (_tabName = TabNames.tabRedlinings.ToString) Then
            For Each redlining As Redlining In _redliningInformation.Redlinings.Where(Function(rdlining) rdlining.ObjectId = componentOccurrence.SystemId)
                If (Not _kblIds.Contains(redlining.ID)) Then _kblIds.Add(redlining.ID)
            Next
        End If
    End Sub

    Private Sub GetCorrespondingIdsFromSelectedComponentBox(componentBoxOccurrence As Component_box_occurrence)
        If (_tabName = TabNames.tabAccessories.ToString) Then
            For Each kblId As String In HarnessAnalyzer.[Shared].Utilities.GetIdrefs(componentBoxOccurrence.Reference_element)
                Dim a_occ As IKblOccurrence = _kblMapper.GetOccurrenceObject(Of Accessory_occurrence)(kblId)
                If a_occ IsNot Nothing Then
                    _kblIds.TryAdd(a_occ.SystemId)
                End If
            Next
        ElseIf (_tabName = TabNames.tabAssemblyParts.ToString) Then
            For Each kblId As String In HarnessAnalyzer.[Shared].Utilities.GetIdrefs(componentBoxOccurrence.Reference_element)
                Dim a_occ As IKblOccurrence = _kblMapper.GetOccurrenceObject(Of Assembly_part_occurrence)(kblId)
                If a_occ IsNot Nothing Then
                    _kblIds.TryAdd(a_occ.SystemId)
                End If
            Next
        ElseIf (_tabName = TabNames.tabCables.ToString) OrElse (_tabName = TabNames.tabWires.ToString) Then
            For Each kblId As String In HarnessAnalyzer.[Shared].Utilities.GetIdrefs(componentBoxOccurrence.Reference_element)
                Dim gw_occ As IKblOccurrence = _kblMapper.GetOccurrenceObjectUntyped(kblId)
                If TypeOf gw_occ Is General_wire_occurrence Then
                    _kblIds.TryAdd(gw_occ.SystemId)
                End If
            Next
        ElseIf (_tabName = TabNames.tabChangeDescriptions.ToString) Then
            GetCorrespondingChangeDescriptionIdsFromSelectedObjectId(componentBoxOccurrence.SystemId)
        ElseIf (_tabName = TabNames.tabComponents.ToString) Then
            For Each kblId As String In HarnessAnalyzer.[Shared].Utilities.GetIdrefs(componentBoxOccurrence.Reference_element)
                Dim comp_occ As IKblOccurrence = _kblMapper.GetOccurrenceObject(Of Component_occurrence)(kblId)
                If comp_occ IsNot Nothing Then
                    _kblIds.TryAdd(comp_occ.SystemId)
                End If
            Next
        ElseIf (_tabName = TabNames.tabConnectors.ToString) Then
            For Each kblId As String In HarnessAnalyzer.[Shared].Utilities.GetIdrefs(componentBoxOccurrence.Reference_element)
                Dim conn_occ As IKblOccurrence = _kblMapper.GetOccurrenceObject(Of Connector_occurrence)(kblId)
                If conn_occ IsNot Nothing Then
                    _kblIds.TryAdd(conn_occ.SystemId)
                End If
            Next
        ElseIf (_tabName = TabNames.tabCoPacks.ToString) Then
            For Each kblId As String In HarnessAnalyzer.[Shared].Utilities.GetIdrefs(componentBoxOccurrence.Reference_element)
                Dim co_pack_occ As IKblOccurrence = _kblMapper.GetOccurrenceObject(Of Co_pack_occurrence)(kblId)
                If co_pack_occ IsNot Nothing Then
                    _kblIds.TryAdd(co_pack_occ.SystemId)
                End If
            Next
        ElseIf (_tabName = TabNames.tabFixings.ToString) Then
            For Each kblId As String In HarnessAnalyzer.[Shared].Utilities.GetIdrefs(componentBoxOccurrence.Reference_element)
                Dim fix_occ As IKblOccurrence = _kblMapper.GetOccurrenceObject(Of Fixing_occurrence)(kblId)
                If fix_occ IsNot Nothing Then
                    _kblIds.TryAdd(fix_occ.SystemId)
                End If
            Next
        ElseIf (_tabName = TabNames.tabModules.ToString) Then
            GetCorrespondingModuleIdsFromSelectedObjectId(componentBoxOccurrence.SystemId)
        ElseIf (_tabName = TabNames.tabRedlinings.ToString) Then
            For Each redlining As Redlining In _redliningInformation.Redlinings.Where(Function(rdlining) rdlining.ObjectId = componentBoxOccurrence.SystemId)
                _kblIds.TryAdd(redlining.ID)
            Next
        ElseIf (_tabName = TabNames.tabSegments.ToString) Then
            For Each kblId As String In HarnessAnalyzer.[Shared].Utilities.GetIdrefs(componentBoxOccurrence.Reference_element)
                Dim w_prot_occ As IKblOccurrence = _kblMapper.GetOccurrenceObject(Of Wire_protection_occurrence)(kblId)
                If w_prot_occ IsNot Nothing Then
                    _kblIds.TryAdd(w_prot_occ.SystemId)
                End If
            Next
        ElseIf (_tabName = TabNames.tabVertices.ToString) Then
            For Each kblId As String In HarnessAnalyzer.[Shared].Utilities.GetIdrefs(componentBoxOccurrence.Reference_element)
                Dim w_prot_occ As IKblOccurrence = _kblMapper.GetOccurrenceObject(Of Wire_protection_occurrence)(kblId)
                If w_prot_occ IsNot Nothing Then
                    _kblIds.TryAdd(w_prot_occ.SystemId)
                End If
            Next
        End If
    End Sub

    Private Sub GetCorrespondingIdsFromSelectedConnector(connectorOccurrence As Connector_occurrence)
        If (_tabName = TabNames.tabAccessories.ToString) Then
            For Each accessoryOccurrence As Accessory_occurrence In _kblMapper.GetAccessoryOccurrences
                If (HarnessAnalyzer.[Shared].Utilities.GetIdrefs(accessoryOccurrence.Reference_element).Contains(connectorOccurrence.SystemId)) Then
                    _kblIds.TryAdd(accessoryOccurrence.SystemId)
                End If
            Next
        ElseIf (_tabName = TabNames.tabAssemblyParts.ToString) Then
            If (TypeOf connectorOccurrence Is Specified_connector_occurrence) Then
                _kblIds.TryAddRange(DirectCast(connectorOccurrence, Specified_connector_occurrence).Related_assembly.SplitSpace)
            End If
        ElseIf (_tabName = TabNames.tabCables.ToString) OrElse (_tabName = TabNames.tabWires.ToString) Then
            For Each contactPoint As Contact_point In connectorOccurrence.Contact_points
                For Each wireCore As IKblWireCoreOccurrence In _kblMapper.GetWireOrCoresOfContactPoint(contactPoint.SystemId)
                    _kblIds.TryAdd(wireCore.SystemId)
                Next
            Next
        ElseIf (_tabName = TabNames.tabChangeDescriptions.ToString) Then
            GetCorrespondingChangeDescriptionIdsFromSelectedObjectId(connectorOccurrence.SystemId)
        ElseIf (_tabName = TabNames.tabComponentBoxes.ToString) Then
            GetCorrespondingComponentBoxIdsFromSelectedObjectId(connectorOccurrence.SystemId)
        ElseIf (_tabName = TabNames.tabComponents.ToString) Then
            For Each componentOccurrence As Component_occurrence In _kblMapper.GetComponentOccurrences
                For Each mountingId As String In HarnessAnalyzer.[Shared].Utilities.GetIdrefs(componentOccurrence.Mounting)
                    If (mountingId = connectorOccurrence.SystemId) Then
                        _kblIds.Add(componentOccurrence.SystemId)
                        Exit For
                    End If
                Next
            Next
        ElseIf (_tabName = TabNames.tabCoPacks.ToString) Then
            For Each coPackOccurrence As Co_pack_occurrence In _kblMapper.GetHarness.Co_pack_occurrence
                If (HarnessAnalyzer.[Shared].Utilities.GetIdrefs(coPackOccurrence.Reference_element).Contains(connectorOccurrence.SystemId)) Then
                    _kblIds.TryAdd(coPackOccurrence.SystemId)
                End If
            Next
        ElseIf (_tabName = TabNames.tabDimSpecs.ToString) Then
            For Each dimensionSpecification As Dimension_specification In _kblMapper.GetDimensionSpecifications.Where(Function(dimSpec) (dimSpec.Origin IsNot Nothing AndAlso dimSpec.Origin = connectorOccurrence.SystemId) OrElse (dimSpec.Target IsNot Nothing AndAlso dimSpec.Target = connectorOccurrence.SystemId))
                _kblIds.TryAdd(dimensionSpecification.SystemId)
            Next
        ElseIf (_tabName = TabNames.tabModules.ToString) Then
            GetCorrespondingModuleIdsFromSelectedObjectId(connectorOccurrence.SystemId)
        ElseIf (_tabName = TabNames.tabRedlinings.ToString) Then
            For Each redlining As Redlining In _redliningInformation.Redlinings.Where(Function(rdlining) rdlining.ObjectId = connectorOccurrence.SystemId)
                _kblIds.TryAdd(redlining.ID)
            Next
        ElseIf (_tabName = TabNames.tabVertices.ToString) Then
            For Each vertex As Node In _kblMapper.GetNodes
                For Each refComponentId As String In HarnessAnalyzer.[Shared].Utilities.GetIdrefs(vertex.Referenced_components)
                    If (refComponentId = connectorOccurrence.SystemId) Then
                        _kblIds.TryAdd(vertex.SystemId)

                        Exit For
                    End If
                Next
            Next
        End If
    End Sub

    Private Sub GetCorrespondingIdsFromSelectedContactPoint(contactPoint As Contact_point)
        If (_tabName = TabNames.tabAssemblyParts.ToString) Then
            For Each associatedPartId As String In HarnessAnalyzer.[Shared].Utilities.GetIdrefs(contactPoint.Associated_parts)
                Dim occ As IKblOccurrence = _kblMapper.GetOccurrenceObjectUntyped(associatedPartId)
                If TypeOf occ Is Specified_cavity_seal_occurrence Then
                    _kblIds.AddRange(DirectCast(occ, Specified_cavity_seal_occurrence).Related_assembly.SplitSpace)
                ElseIf TypeOf occ Is Terminal_occurrence Then
                    If TypeOf occ Is Specified_terminal_occurrence Then
                        _kblIds.AddRange(DirectCast(occ, Specified_terminal_occurrence).Related_assembly.SplitSpace)
                    End If
                End If
            Next
        ElseIf (_tabName = TabNames.tabCables.ToString) OrElse (_tabName = TabNames.tabNets.ToString) OrElse (_tabName = TabNames.tabWires.ToString) Then
            If (_kblMapper.KBLContactPointWireMapper.ContainsKey(contactPoint.SystemId)) Then
                For Each wireId As String In _kblMapper.KBLContactPointWireMapper(contactPoint.SystemId)
                    _kblIds.TryAdd(wireId)
                Next
            End If
        ElseIf (_tabName = TabNames.tabChangeDescriptions.ToString) Then
            GetCorrespondingChangeDescriptionIdsFromSelectedObjectId(contactPoint.SystemId)
        ElseIf (_tabName = TabNames.tabModules.ToString) Then
            Dim cavityParts As New List(Of String)
            cavityParts.Add(contactPoint.SystemId)

            For Each kblId As String In HarnessAnalyzer.[Shared].Utilities.GetIdrefs(contactPoint.Associated_parts)
                cavityParts.Add(kblId)
                GetCorrespondingModuleIdsFromSelectedObjectId(kblId)
            Next
        ElseIf (_tabName = TabNames.tabVertices.ToString) Then
            For Each associatedPartId As String In HarnessAnalyzer.[Shared].Utilities.GetIdrefs(contactPoint.Associated_parts)
                Dim sp_term_occ As Special_terminal_occurrence = _kblMapper.GetOccurrenceObject(Of Special_terminal_occurrence)(associatedPartId)
                If sp_term_occ IsNot Nothing Then
                    For Each vertex As Node In _kblMapper.GetNodes
                        For Each refComponentId As String In HarnessAnalyzer.[Shared].Utilities.GetIdrefs(vertex.Referenced_components)
                            If (refComponentId = associatedPartId) Then
                                _kblIds.Add(vertex.SystemId)

                                Exit For
                            End If
                        Next
                    Next
                End If
            Next

        End If
    End Sub

    Private Sub GetCorrespondingIdsFromSelectedCore(coreOccurrence As Core_occurrence)
        If (_tabName = TabNames.tabAssemblyParts.ToString) Then
            Dim cable As Special_wire_occurrence = If(_kblMapper.KBLCoreCableMapper.ContainsKey(coreOccurrence.SystemId), DirectCast(_kblMapper.KBLOccurrenceMapper(_kblMapper.KBLCoreCableMapper(coreOccurrence.SystemId)), Special_wire_occurrence), Nothing)
            If (cable IsNot Nothing) AndAlso (TypeOf cable Is Specified_special_wire_occurrence) Then _kblIds.AddRange(HarnessAnalyzer.[Shared].Utilities.GetIdrefs(DirectCast(cable, Specified_special_wire_occurrence).Related_assembly))
        ElseIf (_tabName = TabNames.tabCables.ToString) Then
            _kblIds.TryAdd(_kblMapper.KBLCoreCableMapper(coreOccurrence.SystemId))
        ElseIf (_tabName = TabNames.tabChangeDescriptions.ToString) Then
            GetCorrespondingChangeDescriptionIdsFromSelectedObjectId(coreOccurrence.SystemId)
        ElseIf (_tabName = TabNames.tabComponentBoxes.ToString) Then
            GetCorrespondingComponentBoxIdsFromSelectedObjectId(_kblMapper.KBLCoreCableMapper(coreOccurrence.SystemId))
        ElseIf (_tabName = TabNames.tabConnectors.ToString) Then
            For Each connection As Connection In _kblMapper.GetConnections
                If (connection.Wire = coreOccurrence.SystemId) Then
                    For Each extremity As Extremity In connection.Extremities
                        _kblIds.TryAdd(extremity.Contact_point)
                    Next

                    Exit For
                End If
            Next
        ElseIf (_tabName = TabNames.tabModules.ToString) Then
            GetCorrespondingModuleIdsFromSelectedObjectId(coreOccurrence.SystemId)
        ElseIf (_tabName = TabNames.tabNets.ToString) OrElse (_tabName = TabNames.tabWires.ToString) Then
            _kblIds.TryAdd(coreOccurrence.SystemId)
        ElseIf (_tabName = TabNames.tabRedlinings.ToString) Then
            For Each redlining As Redlining In _redliningInformation.Redlinings.Where(Function(rdlining) rdlining.ObjectId = coreOccurrence.SystemId)
                _kblIds.TryAdd(redlining.ID)
            Next
        ElseIf (_tabName = TabNames.tabSegments.ToString) Then
            For Each segment As Segment In _kblMapper.KBLWireSegmentMapper.GetSegmentsOrEmpty(coreOccurrence.SystemId)
                _kblIds.TryAdd(segment.SystemId)
            Next
        ElseIf (_tabName = TabNames.tabVertices.ToString) Then
            For Each segment As Segment In _kblMapper.KBLWireSegmentMapper.GetSegmentsOrEmpty(coreOccurrence.SystemId)
                _kblIds.TryAdd(segment.End_node)
                _kblIds.TryAdd(segment.Start_node)
            Next
        End If
    End Sub

    Private Sub GetCorrespondingIdsFromSelectedCoPack(coPackOccurrence As Co_pack_occurrence)
        If (_tabName = TabNames.tabAccessories.ToString) Then
            For Each accessoryOccurrence As Accessory_occurrence In _kblMapper.GetAccessoryOccurrences.Where(Function(accessoryOcc) HarnessAnalyzer.[Shared].Utilities.GetIdrefs(accessoryOcc.Reference_element).Contains(coPackOccurrence.SystemId))
                _kblIds.TryAdd(accessoryOccurrence.SystemId)
            Next
        ElseIf (_tabName = TabNames.tabAssemblyParts.ToString) Then
            If (TypeOf coPackOccurrence Is Specified_co_pack_occurrence) Then
                _kblIds.TryAddRange(HarnessAnalyzer.[Shared].Utilities.GetIdrefs(DirectCast(coPackOccurrence, Specified_co_pack_occurrence).Related_assembly))
            End If
        ElseIf (_tabName = TabNames.tabChangeDescriptions.ToString) Then
            GetCorrespondingChangeDescriptionIdsFromSelectedObjectId(coPackOccurrence.SystemId)
        ElseIf (_tabName = TabNames.tabComponentBoxes.ToString) Then
            GetCorrespondingComponentBoxIdsFromSelectedObjectId(coPackOccurrence.SystemId)
        ElseIf (_tabName = TabNames.tabConnectors.ToString) Then
            For Each connectorOccurrence As Connector_occurrence In _kblMapper.GetConnectorOccurrences.Where(Function(connectorOcc) HarnessAnalyzer.[Shared].Utilities.GetIdrefs(connectorOcc.Reference_element).Contains(coPackOccurrence.SystemId))
                _kblIds.TryAdd(connectorOccurrence.SystemId)
            Next
        ElseIf (_tabName = TabNames.tabModules.ToString) Then
            GetCorrespondingModuleIdsFromSelectedObjectId(coPackOccurrence.SystemId)
        ElseIf (_tabName = TabNames.tabRedlinings.ToString) Then
            For Each redlining As Redlining In _redliningInformation.Redlinings.Where(Function(rdlining) rdlining.ObjectId = coPackOccurrence.SystemId)
                _kblIds.TryAdd(redlining.ID)
            Next
        End If
    End Sub

    Private Sub GetCorrespondingIdsFromSelectedDimSpec(dimSpec As Dimension_specification)
        Dim occ_origin As IKblOccurrence = If(dimSpec?.Origin IsNot Nothing, _kblMapper.GetOccurrenceObjectUntyped(dimSpec.Origin), Nothing)
        Dim occ_target As IKblOccurrence = If(dimSpec?.Target IsNot Nothing, _kblMapper.GetOccurrenceObjectUntyped(dimSpec.Target), Nothing)

        If (_tabName = TabNames.tabAccessories.ToString) Then
            If TypeOf occ_origin Is Accessory_occurrence Then
                _kblIds.TryAdd(dimSpec.Origin)
            End If
            If TypeOf occ_target Is Accessory_occurrence Then
                _kblIds.TryAdd(dimSpec.Target)
            End If
        ElseIf (_tabName = TabNames.tabAssemblyParts.ToString) Then
            If TypeOf occ_origin Is Assembly_part_occurrence Then
                _kblIds.TryAdd(dimSpec.Origin)
            End If
            If TypeOf occ_target Is Assembly_part_occurrence Then
                _kblIds.TryAdd(dimSpec.Target)
            End If
        ElseIf (_tabName = TabNames.tabConnectors.ToString) Then
            If TypeOf occ_origin Is Connector_occurrence Then
                _kblIds.TryAdd(dimSpec.Origin)
            End If
            If TypeOf occ_target Is Connector_occurrence Then
                _kblIds.TryAdd(dimSpec.Target)
            End If
        ElseIf (_tabName = TabNames.tabFixings.ToString) Then
            If TypeOf occ_origin Is Fixing_occurrence Then
                _kblIds.TryAdd(dimSpec.Origin)
            End If
            If TypeOf occ_target Is Fixing_occurrence Then
                _kblIds.TryAdd(dimSpec.Target)
            End If
        ElseIf (_tabName = TabNames.tabSegments.ToString) Then

            For Each segmentId As String In HarnessAnalyzer.[Shared].Utilities.GetIdrefs(dimSpec.Segments)
                _kblIds.TryAdd(segmentId)
            Next

            If TypeOf occ_origin Is Wire_protection_occurrence Then
                _kblIds.TryAdd(dimSpec.Origin)
            End If

            If TypeOf occ_target Is Wire_protection_occurrence Then
                _kblIds.TryAdd(dimSpec.Target)
            End If

        ElseIf (_tabName = TabNames.tabVertices.ToString) Then
            If TypeOf occ_origin Is Node Then
                _kblIds.TryAdd(dimSpec.Origin)
            End If

            If TypeOf occ_target Is Node Then
                _kblIds.TryAdd(dimSpec.Target)
            End If
        End If
    End Sub

    Private Sub GetCorrespondingIdsFromSelectedFixing(fixingOccurrence As Fixing_occurrence)
        If (_tabName = TabNames.tabAssemblyParts.ToString) Then
            If (TypeOf fixingOccurrence Is Specified_fixing_occurrence) Then
                _kblIds.TryAddRange(HarnessAnalyzer.[Shared].Utilities.GetIdrefs(DirectCast(fixingOccurrence, Specified_fixing_occurrence).Related_assembly))
            End If
        ElseIf (_tabName = TabNames.tabChangeDescriptions.ToString) Then
            GetCorrespondingChangeDescriptionIdsFromSelectedObjectId(fixingOccurrence.SystemId)
        ElseIf (_tabName = TabNames.tabComponentBoxes.ToString) Then
            GetCorrespondingComponentBoxIdsFromSelectedObjectId(fixingOccurrence.SystemId)
        ElseIf (_tabName = TabNames.tabDimSpecs.ToString) Then
            For Each dimensionSpecification As Dimension_specification In _kblMapper.GetDimensionSpecifications.Where(Function(dimSpec) (dimSpec.Origin IsNot Nothing AndAlso dimSpec.Origin = fixingOccurrence.SystemId) OrElse (dimSpec.Target IsNot Nothing AndAlso dimSpec.Target = fixingOccurrence.SystemId))
                _kblIds.TryAdd(dimensionSpecification.SystemId)
            Next
        ElseIf (_tabName = TabNames.tabModules.ToString) Then
            GetCorrespondingModuleIdsFromSelectedObjectId(fixingOccurrence.SystemId)
        ElseIf (_tabName = TabNames.tabRedlinings.ToString) Then
            For Each redlining As Redlining In _redliningInformation.Redlinings.Where(Function(rdlining) rdlining.ObjectId = fixingOccurrence.SystemId)
                _kblIds.TryAdd(redlining.ID)
            Next
        ElseIf (_tabName = TabNames.tabSegments.ToString) Then
            If (_kblMapper.KBLFixingSegmentMapper.ContainsKey(fixingOccurrence.SystemId)) Then
                For Each segmentId As String In _kblMapper.KBLFixingSegmentMapper(fixingOccurrence.SystemId)
                    _kblIds.TryAdd(segmentId)
                Next
            End If
        ElseIf (_tabName = TabNames.tabVertices.ToString) Then
            For Each vertex As Node In _kblMapper.GetNodes
                For Each refComponentId As String In HarnessAnalyzer.[Shared].Utilities.GetIdrefs(vertex.Referenced_components)
                    If (refComponentId = fixingOccurrence.SystemId) Then
                        _kblIds.TryAdd(vertex.SystemId)

                        Exit For
                    End If
                Next
            Next
        ElseIf (_tabName = TabNames.tabAccessories.ToString) Then
            For Each accOcc As Accessory_occurrence In _kblMapper.GetAccessoryOccurrences
                For Each refId As String In HarnessAnalyzer.[Shared].Utilities.GetIdrefs(accOcc.Reference_element)
                    If (refId = fixingOccurrence.SystemId) Then
                        _kblIds.TryAdd(accOcc.SystemId)
                    End If
                Next
            Next
        End If
    End Sub

    Private Sub GetCorrespondingIdsFromSelectedModule([module] As [Module])
        If (_kblMapper.KBLModuleObjectMapper.ContainsKey([module].SystemId)) Then
            If (_tabName = TabNames.tabAccessories.ToString) Then
                For Each kbl_obj As IKblBaseObject In _kblMapper.GetObjectsOfModule([module].SystemId).OfType(Of Accessory_occurrence)
                    _kblIds.TryAdd(kbl_obj.SystemId)
                Next
            ElseIf (_tabName = TabNames.tabApprovals.ToString) Then
                For Each approval As Approval In _kblMapper.GetApprovals.Where(Function(appr) appr.Is_applied_to = [module].SystemId)
                    _kblIds.TryAdd(approval.SystemId)
                Next
            ElseIf (_tabName = TabNames.tabAssemblyParts.ToString) Then
                For Each kbl_obj As IKblBaseObject In _kblMapper.GetObjectsOfModule([module].SystemId).OfType(Of Assembly_part_occurrence)
                    _kblIds.TryAdd(kbl_obj.SystemId)
                Next
            ElseIf (_tabName = TabNames.tabCables.ToString) Then
                For Each kbl_obj As Special_wire_occurrence In _kblMapper.GetObjectsOfModule([module].SystemId).OfType(Of Special_wire_occurrence)
                    _kblIds.TryAdd(kbl_obj.SystemId)
                Next
            ElseIf (_tabName = TabNames.tabChangeDescriptions.ToString) Then
                If ([module].Module_configuration IsNot Nothing) Then
                    GetCorrespondingChangeDescriptionIdsFromSelectedObjectId([module].Module_configuration.SystemId)
                End If
            ElseIf (_tabName = TabNames.tabComponentBoxes.ToString) Then
                For Each kbl_obj As Component_box_occurrence In _kblMapper.GetObjectsOfModule([module].SystemId).OfType(Of Component_box_occurrence)
                    _kblIds.Add(kbl_obj.SystemId)
                Next
            ElseIf (_tabName = TabNames.tabComponents.ToString) Then
                For Each kbl_obj As Component_occurrence In _kblMapper.GetObjectsOfModule([module].SystemId).OfType(Of Component_occurrence)
                    _kblIds.TryAdd(kbl_obj.SystemId)
                Next
            ElseIf (_tabName = TabNames.tabConnectors.ToString) Then
                For Each kbl_obj As Connector_occurrence In _kblMapper.GetObjectsOfModule([module].SystemId).OfType(Of Connector_occurrence)
                    _kblIds.Add(kbl_obj.SystemId)
                Next
            ElseIf (_tabName = TabNames.tabCoPacks.ToString) Then
                For Each kbl_obj As Co_pack_occurrence In _kblMapper.GetObjectsOfModule([module].SystemId).OfType(Of Co_pack_occurrence)
                    _kblIds.TryAdd(kbl_obj.SystemId)
                Next
            ElseIf (_tabName = TabNames.tabFixings.ToString) Then
                For Each kbl_obj As Fixing_occurrence In _kblMapper.GetObjectsOfModule([module].SystemId).OfType(Of Fixing_occurrence)
                    _kblIds.TryAdd(kbl_obj.SystemId)
                Next
            ElseIf (_tabName = TabNames.tabNets.ToString) OrElse (_tabName = TabNames.tabWires.ToString) Then
                For Each kbl_obj As IKblWireCoreOccurrence In _kblMapper.GetObjectsOfModule([module].SystemId).OfType(Of IKblWireCoreOccurrence)
                    _kblIds.TryAdd(kbl_obj.SystemId)
                Next
            ElseIf (_tabName = TabNames.tabRedlinings.ToString) Then
                For Each redlining As Redlining In _redliningInformation.Redlinings.Where(Function(rdlining) rdlining.ObjectId = [module].SystemId)
                    _kblIds.TryAdd(redlining.ID)
                Next
            ElseIf (_tabName = TabNames.tabSegments.ToString) Then
                For Each kbl_obj As IKblBaseObject In _kblMapper.GetObjectsOfModule([module].SystemId).OfType(Of Segment)
                    _kblIds.TryAdd(kbl_obj.SystemId)
                Next
            ElseIf (_tabName = TabNames.tabVertices.ToString) Then
                For Each kbl_obj As IKblBaseObject In _kblMapper.GetObjectsOfModule([module].SystemId).OfType(Of Node)
                    _kblIds.Add(kbl_obj.SystemId)
                Next
            End If
        End If
    End Sub

    Private Sub GetCorrespondingIdsFromSelectedSegment(segment As Segment)
        If (_tabName = TabNames.tabAssemblyParts.ToString) AndAlso (segment.Protection_area IsNot Nothing) Then
            For Each protectionArea As Protection_area In segment.Protection_area
                Dim wireProtectionOccurrence As Wire_protection_occurrence = _kblMapper.GetOccurrenceObject(Of Wire_protection_occurrence)(protectionArea.Associated_protection)
                If TypeOf wireProtectionOccurrence Is Specified_wire_protection_occurrence Then
                    _kblIds.TryAddRange(HarnessAnalyzer.[Shared].Utilities.GetIdrefs(DirectCast(wireProtectionOccurrence, Specified_wire_protection_occurrence).Related_assembly))
                End If
            Next
        ElseIf (_tabName = TabNames.tabCables.ToString) OrElse (_tabName = TabNames.tabWires.ToString) Then
            If (_kblMapper.KBLSegmentWireMapper.ContainsKey(segment.SystemId)) Then
                For Each wireId As String In _kblMapper.KBLSegmentWireMapper(segment.SystemId)
                    _kblIds.TryAdd(wireId)
                Next
            End If
        ElseIf (_tabName = TabNames.tabChangeDescriptions.ToString) Then
            GetCorrespondingChangeDescriptionIdsFromSelectedObjectId(segment.SystemId)
        ElseIf (_tabName = TabNames.tabComponentBoxes.ToString) Then
            For Each protectionArea As Protection_area In segment.Protection_area
                GetCorrespondingComponentBoxIdsFromSelectedObjectId(protectionArea.Associated_protection)
            Next
        ElseIf (_tabName = TabNames.tabDimSpecs.ToString) Then
            For Each dimensionSpecification As Dimension_specification In _kblMapper.GetDimensionSpecifications.Where(Function(dimSpec) HarnessAnalyzer.[Shared].Utilities.GetIdrefs(dimSpec.Segments).Contains(segment.SystemId))
                _kblIds.TryAdd(dimensionSpecification.SystemId)
            Next
        ElseIf (_tabName = TabNames.tabFixings.ToString) AndAlso (segment.Fixing_assignment IsNot Nothing) Then
            For Each fixingAssignment As Fixing_assignment In segment.Fixing_assignment
                _kblIds.TryAdd(fixingAssignment.Fixing)
            Next
        ElseIf (_tabName = TabNames.tabRedlinings.ToString) Then
            For Each redlining As Redlining In _redliningInformation.Redlinings.Where(Function(rdlining) rdlining.ObjectId = segment.SystemId)
                _kblIds.TryAdd(redlining.ID)
            Next
        ElseIf (_tabName = TabNames.tabVertices.ToString) Then
            _kblIds.TryAdd(segment.End_node)
            _kblIds.TryAdd(segment.Start_node)
        End If
    End Sub

    Private Sub GetCorrespondingIdsFromSelectedVertex(vertex As Node)
        If (_tabName = TabNames.tabAssemblyParts.ToString) Then
            For Each referencedComponent As String In HarnessAnalyzer.[Shared].Utilities.GetIdrefs(vertex.Referenced_components)
                If (_kblMapper.KBLOccurrenceMapper.ContainsKey(referencedComponent)) Then
                    If (TypeOf _kblMapper.KBLOccurrenceMapper(referencedComponent) Is Specified_accessory_occurrence) Then
                        _kblIds.AddRange(HarnessAnalyzer.[Shared].Utilities.GetIdrefs(_kblMapper.GetOccurrenceObject(Of Specified_accessory_occurrence)(referencedComponent).Related_assembly))
                    ElseIf (TypeOf _kblMapper.KBLOccurrenceMapper(referencedComponent) Is Specified_connector_occurrence) Then
                        _kblIds.AddRange(HarnessAnalyzer.[Shared].Utilities.GetIdrefs(_kblMapper.GetOccurrenceObject(Of Specified_connector_occurrence)(referencedComponent).Related_assembly))
                    ElseIf (TypeOf _kblMapper.KBLOccurrenceMapper(referencedComponent) Is Specified_fixing_occurrence) Then
                        _kblIds.AddRange(HarnessAnalyzer.[Shared].Utilities.GetIdrefs(_kblMapper.GetOccurrenceObject(Of Specified_fixing_occurrence)(referencedComponent).Related_assembly))
                    ElseIf (TypeOf _kblMapper.KBLOccurrenceMapper(referencedComponent) Is Specified_wire_protection_occurrence) Then
                        _kblIds.AddRange(HarnessAnalyzer.[Shared].Utilities.GetIdrefs(_kblMapper.GetOccurrenceObject(Of Specified_wire_protection_occurrence)(referencedComponent).Related_assembly))
                    End If
                End If
            Next
        ElseIf (_tabName = TabNames.tabCables.ToString) OrElse (_tabName = TabNames.tabWires.ToString) Then
            For Each segment As Segment In _kblMapper.GetSegments
                If (segment.End_node = vertex.SystemId OrElse segment.Start_node = vertex.SystemId) AndAlso (_kblMapper.KBLSegmentWireMapper.ContainsKey(segment.SystemId)) Then
                    For Each wireId As String In _kblMapper.KBLSegmentWireMapper(segment.SystemId)
                        _kblIds.TryAdd(wireId)
                    Next
                End If
            Next
        ElseIf (_tabName = TabNames.tabChangeDescriptions.ToString) Then
            GetCorrespondingChangeDescriptionIdsFromSelectedObjectId(vertex.SystemId)
        ElseIf (_tabName = TabNames.tabComponentBoxes.ToString) Then
            For Each referencedComponent As String In HarnessAnalyzer.[Shared].Utilities.GetIdrefs(vertex.Referenced_components)
                GetCorrespondingComponentBoxIdsFromSelectedObjectId(referencedComponent)
            Next
        ElseIf (_tabName = TabNames.tabConnectors.ToString) OrElse (_tabName = TabNames.tabFixings.ToString) Then
            For Each referencedComponent As String In HarnessAnalyzer.[Shared].Utilities.GetIdrefs(vertex.Referenced_components)
                _kblIds.TryAdd(referencedComponent)
            Next
        ElseIf (_tabName = TabNames.tabDimSpecs.ToString) Then
            For Each dimensionSpecification As Dimension_specification In _kblMapper.GetDimensionSpecifications.Where(Function(dimSpec) (dimSpec.Origin IsNot Nothing AndAlso dimSpec.Origin = vertex.SystemId) OrElse (dimSpec.Target IsNot Nothing AndAlso dimSpec.Target = vertex.SystemId))
                _kblIds.TryAdd(dimensionSpecification.SystemId)
            Next
        ElseIf (_tabName = TabNames.tabRedlinings.ToString) Then
            For Each redlining As Redlining In _redliningInformation.Redlinings.Where(Function(rdlining) rdlining.ObjectId = vertex.SystemId)
                _kblIds.TryAdd(redlining.ID)
            Next
        ElseIf (_tabName = TabNames.tabSegments.ToString) Then
            For Each segment As Segment In _kblMapper.GetSegments
                If (segment.End_node = vertex.SystemId OrElse segment.Start_node = vertex.SystemId) AndAlso (Not _kblIds.Contains(segment.SystemId)) Then
                    _kblIds.TryAdd(segment.SystemId)
                End If
            Next
        End If
    End Sub

    Private Sub GetCorrespondingIdsFromSelectedWire(wireOccurrence As Wire_occurrence)
        If (_tabName = TabNames.tabAssemblyParts.ToString) Then
            If (TypeOf wireOccurrence Is Specified_wire_occurrence) Then
                _kblIds.TryAddRange(HarnessAnalyzer.[Shared].Utilities.GetIdrefs(DirectCast(wireOccurrence, Specified_wire_occurrence).Related_assembly))
            End If
        ElseIf (_tabName = TabNames.tabChangeDescriptions.ToString) Then
            GetCorrespondingChangeDescriptionIdsFromSelectedObjectId(wireOccurrence.SystemId)
        ElseIf (_tabName = TabNames.tabComponentBoxes.ToString) Then
            GetCorrespondingComponentBoxIdsFromSelectedObjectId(wireOccurrence.SystemId)
        ElseIf (_tabName = TabNames.tabConnectors.ToString) Then
            For Each connection As Connection In _kblMapper.GetConnections
                If connection.Wire = wireOccurrence.SystemId Then
                    For Each extremity As Extremity In connection.Extremities
                        _kblIds.TryAdd(extremity.Contact_point)
                    Next

                    Exit For
                End If
            Next
        ElseIf (_tabName = TabNames.tabModules.ToString) Then
            GetCorrespondingModuleIdsFromSelectedObjectId(wireOccurrence.SystemId)
        ElseIf (_tabName = TabNames.tabNets.ToString) OrElse (_tabName = TabNames.tabWires.ToString) Then
            _kblIds.TryAdd(wireOccurrence.SystemId)
        ElseIf (_tabName = TabNames.tabRedlinings.ToString) Then
            For Each redlining As Redlining In _redliningInformation.Redlinings.Where(Function(rdlining) rdlining.ObjectId = wireOccurrence.SystemId)
                _kblIds.TryAdd(redlining.ID)
            Next
        ElseIf (_tabName = TabNames.tabSegments.ToString) Then
            _kblIds.TryAddRange(_kblMapper.KBLWireSegmentMapper.GetSegmentIdsOrEmpty(wireOccurrence.SystemId))
        ElseIf (_tabName = TabNames.tabVertices.ToString) Then
            For Each segment As Segment In _kblMapper.KBLWireSegmentMapper.GetSegmentsOrEmpty(wireOccurrence.SystemId)
                _kblIds.TryAdd(segment.End_node)
                _kblIds.TryAdd(segment.Start_node)
            Next
        End If
    End Sub

    Private Sub GetCorrespondingIdsFromSelectedWireProtection(protectionOccurrence As Wire_protection_occurrence)
        If (_tabName = TabNames.tabAssemblyParts.ToString) Then
            If (TypeOf protectionOccurrence Is Specified_wire_protection_occurrence) Then
                _kblIds.TryAddRange(DirectCast(protectionOccurrence, Specified_wire_protection_occurrence).Related_assembly.SplitSpace)
            End If
        ElseIf (_tabName = TabNames.tabCables.ToString) OrElse (_tabName = TabNames.tabWires.ToString) Then
            For Each segment As Segment In _kblMapper.GetSegments
                For Each protectionArea As Protection_area In segment.Protection_area
                    If (protectionArea.Associated_protection = protectionOccurrence.SystemId) Then
                        For Each wire_core As IKblWireCoreOccurrence In _kblMapper.GetWireOrCoresOfSegment(segment.SystemId)
                            _kblIds.TryAdd(wire_core.SystemId)
                        Next
                    End If
                Next
            Next
        ElseIf (_tabName = TabNames.tabChangeDescriptions.ToString) Then
            GetCorrespondingChangeDescriptionIdsFromSelectedObjectId(protectionOccurrence.SystemId)
        ElseIf (_tabName = TabNames.tabComponentBoxes.ToString) Then
            GetCorrespondingComponentBoxIdsFromSelectedObjectId(protectionOccurrence.SystemId)
        ElseIf (_tabName = TabNames.tabDimSpecs.ToString) Then
            For Each dimensionSpecification As Dimension_specification In _kblMapper.GetDimensionSpecifications.Where(Function(dimSpec) (dimSpec.Origin IsNot Nothing AndAlso dimSpec.Origin = protectionOccurrence.SystemId) OrElse (dimSpec.Target IsNot Nothing AndAlso dimSpec.Target = protectionOccurrence.SystemId))
                _kblIds.TryAdd(dimensionSpecification.SystemId)
            Next
        ElseIf (_tabName = TabNames.tabModules.ToString) Then
            GetCorrespondingModuleIdsFromSelectedObjectId(protectionOccurrence.SystemId)
        ElseIf (_tabName = TabNames.tabRedlinings.ToString) Then
            For Each redlining As Redlining In _redliningInformation.Redlinings.Where(Function(rdlining) rdlining.ObjectName = protectionOccurrence.SystemId)
                _kblIds.TryAdd(redlining.ID)
            Next
        End If
    End Sub

    Private Sub GetCorrespondingModuleIdsFromSelectedObjectId(objectId As String)
        For Each m As [Module] In _kblMapper.GetModulesOfObject(objectId)
            _kblIds.TryAdd(m.SystemId)
        Next
    End Sub

End Class