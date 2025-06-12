Imports Infragistics.Win.UltraWinGrid
Imports Zuken.E3.HarnessAnalyzer.Shared
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Friend Class CavityChildRowFilter
    Inherits BaseChildRowFilter

    Friend Sub New()
        MyBase.New
    End Sub

    Protected Sub New(info As RowFilterInfo, grid As UltraGrid, Optional disableFilteredRowsOnly As Boolean = False)
        MyBase.New(info, grid, disableFilteredRowsOnly)
    End Sub

    Public Overrides Function DoRowFilter(row As UltraGridRow, Optional overrideKblObjectType As KblObjectType? = Nothing) As KblObjectFilterType
        row.TrySetCellHidden(HarnessAnalyzer.[Shared].CORE_WIRE_NUMBER_KEY, False)
        FilterInactiveCoreOrWires(KblObjectType.Wire_occurrence, row)
        FilterInactiveCoreOrWires(KblObjectType.Core_occurrence, row)
        FilterAllInactiveCavities(row)
        StrikeReplacedPlugs(row)
        Return KblObjectFilterType.Unfiltered
    End Function

    Public Overrides Property KblObjectTypes As KblObjectType()
        Get
            Return {KblObjectType.Cavity_plug_occurrence, KblObjectType.Cavity_seal_occurrence, KblObjectType.Special_terminal_occurrence, KblObjectType.Terminal_occurrence}
        End Get
        Protected Set(value As KblObjectType())
            Throw New NotSupportedException("Setting object types to CavityRowFilter not supported!")
        End Set
    End Property

    Private Sub StrikeReplacedPlugs(row As UltraGridRow)
        If row.Cells.Exists(ConnectorPropertyName.Plug_part_number) Then
            row.Cells(ConnectorPropertyName.Plug_part_number).Appearance.FontData.Strikeout = Infragistics.Win.DefaultableBoolean.Default
        End If

        'HINT if active objects is nothing, it is assumed that all objects are active.(Initial state)
        If (row.Cells.Exists(ConnectorPropertyName.Plug_part_number) AndAlso row.Cells(ConnectorPropertyName.Plug_part_number).Value IsNot Nothing) Then
            For Each entryId As String In TryCast(row.Tag, List(Of String)).OrEmpty
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

    Private Sub FilterInactiveCoreOrWires(wireOrCoreObjectType As KblObjectType, row As UltraGridRow)
        If Info.InactiveObjects.ContainsKey(wireOrCoreObjectType) AndAlso (row.Cells.Exists(HarnessAnalyzer.[Shared].CORE_WIRE_NUMBER_KEY)) Then
            For Each kblId As String In TryCast(row.Tag, List(Of String)).OrEmpty
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
        row.TrySetCellHidden(ConnectorPropertyName.Plug_part_information, False)
        row.TrySetCellHidden(ConnectorPropertyName.Plug_part_number, False)
        row.TrySetCellHidden(ConnectorPropertyName.Seal_part_information, False)
        row.TrySetCellHidden(ConnectorPropertyName.Seal_part_number, False)
        row.TrySetCellHidden(ConnectorPropertyName.Description, False)
        row.TrySetCellHidden(ConnectorPropertyName.Terminal_part_information, False)
        row.TrySetCellHidden(ConnectorPropertyName.Terminal_part_number, False)
        row.TrySetCellHidden(ConnectorPropertyName.Plating, False)

        For Each objType As KblObjectType In KblObjectTypes.Except({KblObjectType.Connector_occurrence})
            FilterInactiveCavities(objType, row)
        Next
    End Sub

    Private Function FilterInactiveCavities(kblObjectType As KblObjectType, row As UltraGridRow) As Boolean
        Dim anyFiltered As Boolean = False
        If Me.Info.InactiveObjects.ContainsKey(kblObjectType) Then
            Dim group As ICollectionGrouping(Of KblObjectType, String) = Me.Info.InactiveObjects(kblObjectType)
            If group IsNot Nothing Then
                Select Case kblObjectType
                    Case KblObjectType.Cavity_plug_occurrence
                        If (row.TrySetCellHidden(ConnectorPropertyName.Plug_part_information, True)) Then
                            anyFiltered = True
                        End If

                        If (row.TrySetCellHidden(ConnectorPropertyName.Plug_part_number, True)) Then
                            anyFiltered = True
                        End If
                    Case KblObjectType.Cavity_seal_occurrence
                        If (row.TrySetCellHidden(ConnectorPropertyName.Seal_part_information, True)) Then
                            anyFiltered = True
                        End If

                        If (row.TrySetCellHidden(ConnectorPropertyName.Seal_part_number, True)) Then
                            anyFiltered = True
                        End If
                    Case KblObjectType.Special_terminal_occurrence, KblObjectType.Terminal_occurrence
                        If (row.TrySetCellHidden(ConnectorPropertyName.Description, True)) Then
                            anyFiltered = True
                        End If

                        If (row.TrySetCellHidden(ConnectorPropertyName.Terminal_part_information, True)) Then
                            anyFiltered = True
                        End If

                        If (row.TrySetCellHidden(ConnectorPropertyName.Terminal_part_number, True)) Then
                            anyFiltered = True
                        End If

                        If (row.TrySetCellHidden(ConnectorPropertyName.Plating, True)) Then
                            anyFiltered = True
                        End If
                End Select
            End If
        End If
        Return anyFiltered
    End Function

End Class
