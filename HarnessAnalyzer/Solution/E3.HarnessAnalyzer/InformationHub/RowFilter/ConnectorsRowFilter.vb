Imports Infragistics.Win.UltraWinGrid
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Friend Class ConnectorsRowFilter
    Inherits BaseRootRowFilter

    Public Sub New()
        Me.New(Nothing, Nothing)
    End Sub

    Public Sub New(info As RowFilterInfo, grid As Infragistics.Win.UltraWinGrid.UltraGrid)
        MyBase.New(info, grid)
    End Sub

    Public Overrides Property KblObjectTypes As KblObjectType()
        Get
            Return {KblObjectType.Connector_occurrence}
        End Get
        Protected Set(value As KblObjectType())
            Throw New NotSupportedException("Setting object types to ConnectorsRowFilter not supported!")
        End Set
    End Property

    Protected Overrides Function FilterKblObjectCore(kblObjectType As KblObjectType, systemId As String) As KblObjectFilterType
        If Me.Info.InactiveObjects.ContainsValue(kblObjectType, systemId) Then
            Dim conn_occ As Connector_occurrence = Info.KBL.GetOccurrenceObject(Of Connector_occurrence)(systemId)
            If HasInactiveConnectorActiveParts(conn_occ) Then
                ''HINT special daimler request - connectors which are not in the module should be invisible in 3D but we keep them grayed out visible in 2D to be konsistent with the old logic. 
                Return KblObjectFilterType.FilteredAndGrayOut
            Else
                Return KblObjectFilterType.Filtered
            End If
        End If
        Return KblObjectFilterType.Unfiltered
    End Function

    Private Sub FilterInactiveCoreOrWires(wireOrCoreObjectType As KblObjectType, row As UltraGridRow)
        If Info.InactiveObjects.ContainsKey(wireOrCoreObjectType) AndAlso (row.Cells.Exists(HarnessAnalyzer.[Shared].CORE_WIRE_NUMBER_KEY)) Then
            For Each kblId As String In DirectCast(row.Tag, List(Of String))
                If (Info.KBL.KBLContactPointWireMapper.ContainsKey(kblId)) Then
                    For Each wireId As String In Info.KBL.KBLContactPointWireMapper(kblId)
                        If (Info.InactiveObjects.ContainsValue(wireOrCoreObjectType, wireId)) Then
                            row.Cells(HarnessAnalyzer.[Shared].CORE_WIRE_NUMBER_KEY).Hidden = True
                        End If
                    Next
                End If
            Next
        End If
    End Sub

    Private Sub FilterAllInactiveCavities(row As UltraGridRow)
        Dim cav_objectTypes As KblObjectType() = {KblObjectType.Cavity_plug_occurrence, KblObjectType.Cavity_seal_occurrence, KblObjectType.Special_terminal_occurrence, KblObjectType.Terminal_occurrence}
        For Each objType As KblObjectType In cav_objectTypes
            FilterInactiveCavities(objType, row)
        Next
    End Sub

    Private Sub FilterInactiveCavities(kblObjectType As KblObjectType, row As UltraGridRow)
        Dim anyFiltered As Boolean = False
        If Me.Info.InactiveObjects.ContainsKey(kblObjectType) Then
            Dim group As IEnumerable(Of String) = Me.Info.InactiveObjects(kblObjectType)
            If group IsNot Nothing Then
                Select Case kblObjectType
                    Case KblObjectType.Cavity_plug_occurrence
                        If (row.Cells.Exists(ConnectorPropertyName.Plug_part_information)) Then
                            row.Cells(ConnectorPropertyName.Plug_part_information).Hidden = True
                            anyFiltered = True
                        Else
                            row.Cells(ConnectorPropertyName.Plug_part_information).Hidden = False
                        End If

                        If (row.Cells.Exists(ConnectorPropertyName.Plug_part_number)) Then
                            row.Cells(ConnectorPropertyName.Plug_part_number).Hidden = True
                            anyFiltered = True
                        Else
                            row.Cells(ConnectorPropertyName.Plug_part_number).Hidden = False
                        End If
                    Case KblObjectType.Cavity_seal_occurrence
                        If (row.Cells.Exists(ConnectorPropertyName.Seal_part_information)) Then
                            row.Cells(ConnectorPropertyName.Seal_part_information).Hidden = True
                            anyFiltered = True
                        Else
                            row.Cells(ConnectorPropertyName.Seal_part_information).Hidden = False
                        End If

                        If (row.Cells.Exists(ConnectorPropertyName.Seal_part_number)) Then
                            row.Cells(ConnectorPropertyName.Seal_part_number).Hidden = True
                            anyFiltered = True
                        Else
                            row.Cells(ConnectorPropertyName.Seal_part_number).Hidden = False
                        End If
                    Case KblObjectType.Special_terminal_occurrence, KblObjectType.Terminal_occurrence
                        If (row.Cells.Exists(ConnectorPropertyName.Description)) Then
                            row.Cells(ConnectorPropertyName.Description).Hidden = True
                            anyFiltered = True
                        Else
                            row.Cells(ConnectorPropertyName.Description).Hidden = False
                        End If

                        If (row.Cells.Exists(ConnectorPropertyName.Terminal_part_information)) Then
                            row.Cells(ConnectorPropertyName.Terminal_part_information).Hidden = True
                            anyFiltered = True
                        Else
                            row.Cells(ConnectorPropertyName.Terminal_part_information).Hidden = False
                        End If

                        If (row.Cells.Exists(ConnectorPropertyName.Terminal_part_number)) Then
                            row.Cells(ConnectorPropertyName.Terminal_part_number).Hidden = True
                            anyFiltered = True
                        Else
                            row.Cells(ConnectorPropertyName.Terminal_part_number).Hidden = False
                        End If

                        If (row.Cells.Exists(ConnectorPropertyName.Plating)) Then
                            row.Cells(ConnectorPropertyName.Plating).Hidden = True
                            anyFiltered = True
                        Else
                            row.Cells(ConnectorPropertyName.Plating).Hidden = False
                        End If
                End Select
            End If
        End If
    End Sub

    Private Sub StrikeReplacedPlugs(row As UltraGridRow)
        'HINT if active objects is nothing, it is assumed that all objects are active.(Initial state)
        If (row.Cells.Exists(ConnectorPropertyName.Plug_part_number) AndAlso row.Cells(ConnectorPropertyName.Plug_part_number).Value IsNot Nothing) Then
            row.Cells(ConnectorPropertyName.Plug_part_number).Appearance.FontData.Strikeout = Infragistics.Win.DefaultableBoolean.False

            For Each entryId As String In CType(row.Tag, List(Of String))
                Dim plg As Cavity_plug_occurrence = Info.KBL.GetOccurrenceObject(Of Cavity_plug_occurrence)(entryId)
                If plg IsNot Nothing Then
                    If (Info.SealsOnPlugReplacements.ContainsKey(plg.SystemId)) Then
                        If (Info.ActiveObjects?.Count).GetValueOrDefault = 0 OrElse Info.ActiveObjects.Contains(plg.SystemId) Then
                            For Each slId As String In Info.SealsOnPlugReplacements(plg.SystemId)
                                If (Info.ActiveObjects?.Count).GetValueOrDefault = 0 OrElse Info.ActiveObjects.Contains(slId) Then
                                    row.Cells(ConnectorPropertyName.Plug_part_number).Appearance.FontData.Strikeout = Infragistics.Win.DefaultableBoolean.True
                                    Exit For
                                End If
                            Next
                        End If
                        Exit For
                    End If
                End If
            Next
        End If
    End Sub

    Private Function HasInactiveConnectorActiveParts(connector As Connector_occurrence) As Boolean
        For Each accessory As Accessory_occurrence In Info.KBL.GetAccessoryOccurrences.Where(Function(acc) acc.Reference_element.OrEmpty = connector.SystemId)
            If Not Me.Info.InactiveObjects.ContainsValue(KblObjectType.Accessory_occurrence, accessory.SystemId) Then
                Return True
            End If
        Next

        For Each component As Component_occurrence In Info.KBL.GetComponentOccurrences
            For Each mounting As String In component.Mounting.OrEmpty.Trim.SplitSpace.Where(Function(id) id = connector.SystemId)
                Select Case component.ObjectType
                    Case KblObjectType.Component_occurrence, KblObjectType.Fuse_occurrence
                        If Not Me.Info.InactiveObjects.ContainsValue(component.ObjectType, component.SystemId) Then
                            Return True
                        End If
                End Select
            Next
        Next

        For Each contactPoint As Contact_point In connector.Contact_points
            For Each associatedPart As String In contactPoint.Associated_parts.OrEmpty.Trim.SplitSpace
                Dim kblOcc As IKblOccurrence = Info.KBL.GetAnyOccurrenceObjectOfKblObjectTypes(associatedPart, KblObjectType.Cavity_seal_occurrence, KblObjectType.Special_terminal_occurrence, KblObjectType.Terminal_occurrence)
                If kblOcc IsNot Nothing Then
                    If Not Me.Info.InactiveObjects.ContainsValue(kblOcc.ObjectType, associatedPart) Then
                        Return True
                    End If
                End If
            Next

            For Each wireCore As IKblWireCoreOccurrence In Info.KBL.GetWireOrCoresOfContactPoint(contactPoint.SystemId)
                Dim kblObjType As KblObjectType = wireCore.ObjectType
                If kblObjType = KblObjectType.Core_occurrence Then
                    'HINT: map core to cable BL and proceed normally
                    kblObjType = KblObjectType.Special_wire_occurrence
                    Dim cableId As String = Info.KBL.KBLCoreCableMapper(wireCore.SystemId)
                    If Not Me.Info.InactiveObjects.ContainsValue(kblObjType, cableId) Then
                        Return True
                    End If
                Else
                    If Not Me.Info.InactiveObjects.ContainsValue(kblObjType, wireCore.SystemId) Then
                        Return True
                    End If
                End If
            Next
        Next

        Return False
    End Function

End Class
