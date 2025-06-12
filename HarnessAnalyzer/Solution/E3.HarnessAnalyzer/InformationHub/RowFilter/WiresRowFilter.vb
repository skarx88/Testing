Imports Infragistics.Win.UltraWinGrid
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Friend Class WiresRowFilter
    Inherits BaseRootRowFilter

    Friend Sub New()
        Me.New(Nothing, Nothing)
    End Sub

    Public Sub New(info As RowFilterInfo, grid As UltraGrid)
        MyBase.New(info, grid)
    End Sub

    Public Overrides Property KblObjectTypes As KblObjectType()
        Get
            Return {KblObjectType.Wire_occurrence}
        End Get
        Protected Set(value As KblObjectType())
            Throw New NotSupportedException("Setting object types to WiresRowFilter not supported!")
        End Set
    End Property

    Protected Overrides Sub PreRowFilter(row As UltraGridRow)
        If (row.Cells.Exists(WirePropertyName.Wiring_group.ToString)) Then
            row.Cells(WirePropertyName.Wiring_group.ToString).Hidden = False
        End If
    End Sub

    Public Overrides Function DoRowFilter(row As UltraGridRow, Optional overrideKblObjectType As KblObjectType? = Nothing) As KblObjectFilterType
        FilterWiringGroupCells(row)
        Dim result As KblObjectFilterType = MyBase.DoRowFilter(row, overrideKblObjectType)
        If result = KblObjectFilterType.Unfiltered Then
            result = MyBase.DoRowFilter(row, KblObjectType.Core_occurrence) ' HINT: we also have cores as non root-rows here -> TODO: make rowFilters overall more generic to make them reusable here
        End If
        Return result
    End Function

    Private Sub FilterWiringGroupCells(row As UltraGridRow)
        If (row.Cells.Exists(WirePropertyName.Wiring_group.ToString)) Then
            If (Not String.IsNullOrEmpty(row.Cells(WirePropertyName.Wiring_group.ToString).Value?.ToString)) Then
                Dim wireOcc As Wire_occurrence = TryCast(row.Cells(WirePropertyName.Wiring_group.ToString).Tag, Wire_occurrence)
                If wireOcc IsNot Nothing Then
                    For Each wiringGroup As Wiring_group In Info.KBL.GetWiringGroups.Where(Function(wirGroup) wirGroup.Assigned_wire.SplitSpace.Contains(wireOcc.SystemId))
                        If Info.InactiveObjects.ContainsValue(KblObjectType.Wiring_group, wiringGroup.SystemId) Then
                            row.Cells(WirePropertyName.Wiring_group.ToString).Hidden = True
                            Exit For
                        End If
                    Next
                End If
            End If
        End If
    End Sub

End Class
