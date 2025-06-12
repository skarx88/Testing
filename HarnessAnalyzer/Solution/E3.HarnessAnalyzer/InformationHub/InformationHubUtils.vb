Imports Infragistics.Win
Imports Infragistics.Win.UltraWinGrid

Friend Class InformationHubUtils

    Private Shared ReadOnly Property MARKED_ROW_BACK_COLOR As System.Drawing.Color = Color.LightGreen

    Public Shared Sub EnsureRowsVisible(rows As IEnumerable(Of RowMarkableResult), Optional expandParentIfChild As Boolean = True)
        For Each grid_group As IGrouping(Of UltraGridBase, RowMarkableResult) In rows.GroupBy(Function(r_res) r_res.Row.Band.Layout.Grid)
            grid_group.Key.BeginUpdate()
            For Each mRow As RowMarkableResult In grid_group
                mRow.EnsureVisible(expandParentIfChild)
            Next
            grid_group.Key.EndUpdate(True)
        Next
    End Sub

    Public Shared Sub EnsureRowsVisible(rows As IEnumerable(Of UltraGridRow), Optional expandParentIfChild As Boolean = True)
        For Each group As IGrouping(Of UltraGridBase, UltraGridRow) In rows.GroupBy(Function(r) r.Band.Layout.Grid)
            group.Key.BeginUpdate()
            For Each row As UltraGridRow In group
                EnsureRowVisible(row, expandParentIfChild)
            Next
            group.Key.EndUpdate()
        Next
    End Sub

    Public Shared Sub EnsureRowVisible(row As UltraGridRow, Optional expandParentIfChild As Boolean = True)
        If row.ParentRow IsNot Nothing Then
            If expandParentIfChild Then
                row.ParentRow.ExpandAll()
            End If
            row.EnsureVisible
        ElseIf row.HasChild Then
            EnsureRowVisible(row.ChildBands.FirstRow, expandParentIfChild)
        Else
            row.EnsureVisible
        End If
    End Sub

    Public Shared Sub SetMarkedRowAppearance(row As UltraGridRow)
        row.Appearance.BackColor = MARKED_ROW_BACK_COLOR
        row.Appearance.FontData.Bold = DefaultableBoolean.True
    End Sub

    Public Shared Sub ResetMarkedRowAppearance(row As UltraGridRow)
        row.Appearance.ResetBackColor()
        row.Appearance.FontData.Bold = DefaultableBoolean.Default
    End Sub

    Public Shared Function HasMarkedRowColor(row As UltraGridRow) As Boolean
        Return row.Appearance.BackColor = MARKED_ROW_BACK_COLOR
    End Function

    Public Shared Function CanSetMarkedRowAppearance(row As UltraGridRow) As Boolean
        'TODO: checking the rows over the BackColor is cumbersome and very dangerous -> currently not possible to change this -> there is a need for a method that returns if ANY row is a MarkRow or not (this BL was taken from trunk and should be changed)
        Return row.Appearance.BackColor <> Color.FromArgb(190, 190, 190)
    End Function

    Public Shared Function GetKblIds(rows As IEnumerable(Of UltraGridRow), Optional includeCorrespondingIds As IncludeCorrespondingIdsInfo = Nothing, Optional redliningInformation As RedliningInformation = Nothing) As List(Of String)
        Dim kbl_ids As New List(Of String)

        For Each row As UltraGridRow In rows
            kbl_ids.AddRange(GetKblIdsCore(row))
        Next

        If (includeCorrespondingIds?.Value).GetValueOrDefault Then
            kbl_ids.AddRange(GetCorrespondingKblIdsFrom(kbl_ids, includeCorrespondingIds, redliningInformation))
        End If

        Return kbl_ids
    End Function

    Public Shared Function GetCorrespondingKblIdsFrom(kblIds As IEnumerable(Of String), informationHub As InformationHub) As List(Of String)
        Return GetCorrespondingKblIdsFrom(kblIds, IncludeCorrespondingIdsInfo.GetTrue(informationHub.Kbl, informationHub.ActiveGrid), informationHub.RedliningInfo)
    End Function

    Public Shared Function GetCorrespondingKblIdsFrom(kblIds As IEnumerable(Of String), includeCorrespondingIds As IncludeCorrespondingIdsInfo, redliningInformation As RedliningInformation) As List(Of String)
        Dim resulting_ids As New List(Of String)
        Using infoRoundTrip As New InformationRoundTrip(includeCorrespondingIds.KblMapper, redliningInformation, kblIds.ToList, includeCorrespondingIds.CorrespondingGrid)
            resulting_ids.AddRange(infoRoundTrip.GetCorrespondingKBLIds)
        End Using
        Return resulting_ids
    End Function

    Public Shared Function GetKblIds(ParamArray rows As UltraGridRow()) As List(Of String)
        Return GetKblIds(rows, Nothing, Nothing)
    End Function

    Private Shared Function GetKblIdsCore(row As UltraGridRow) As String()
        If (row.Band.Key = KblObjectType.Redlining.ToString) Then
            If row.Cells.Exists("ObjectId") AndAlso (row.Cells("ObjectId").Value IsNot Nothing) Then
                Return {row.Cells("ObjectId").Value.ToString}
            ElseIf TypeOf row.Tag Is String Then
                Return {CStr(row.Tag)}
            ElseIf TypeOf row.Tag Is IEnumerable Then
                Return CType(row.Tag, IEnumerable).OfType(Of String).ToArray
            End If
        ElseIf (row.Tag IsNot Nothing) AndAlso (TypeOf row.Tag Is List(Of String)) AndAlso (DirectCast(row.Tag, List(Of String)).Count > 0) Then
            Return DirectCast(row.Tag, List(Of String)).ToArray
        ElseIf (row.Tag IsNot Nothing) Then
            Return {row.Tag?.ToString}
        ElseIf TypeOf row.Tag Is String Then
            Return {CType(row.Tag, String)}
        ElseIf (TypeOf row.Tag Is KeyValuePair(Of String, Object)) Then
            Return {DirectCast(row.Tag, KeyValuePair(Of String, Object)).Key.SplitRemoveEmpty("|"c).First}
        Else
            Dim dataRow As UltraWinDataSource.UltraDataRow = TryCast(row.ListObject, UltraWinDataSource.UltraDataRow)
            If dataRow IsNot Nothing AndAlso TypeOf dataRow.Tag Is IKblBaseObject Then
                Return {CType(dataRow.Tag, IKblBaseObject).SystemId}
            End If
        End If
        Return Array.Empty(Of String)
    End Function

    Public Shared Function IsRowOrChildOfKblId(row As UltraGridRow, kbl_id As String, Optional ByRef firstChildRowFound As UltraGridRow = Nothing) As Boolean
        If GetKblIds(row).Contains(kbl_id) Then
            Return True
        ElseIf row.HasChild Then
            For Each childRow As UltraGridRow In row.ChildBands(0).Rows
                If Not childRow.Hidden AndAlso childRow.Tag IsNot Nothing Then
                    If GetKblIds(childRow).Contains(kbl_id) Then
                        firstChildRowFound = childRow
                        Return True
                    End If
                End If
            Next
        End If
        Return False
    End Function

    Public Shared Function GetTabNameFromKblObjectType(kblObjectType As KblObjectType) As TabNames
        Select Case kblObjectType
            Case KblObjectType.Connector_occurrence
                Return TabNames.tabConnectors
            Case KblObjectType.Accessory_occurrence, KblObjectType.Accessory
                Return TabNames.tabAccessories
            Case KblObjectType.Approval
                Return TabNames.tabApprovals
            Case KblObjectType.Assembly_part, KblObjectType.Assembly_part_occurrence
                Return TabNames.tabAssemblyParts
            Case KblObjectType.Special_wire_occurrence
                Return TabNames.tabCables
            Case KblObjectType.Change_description
                Return TabNames.tabChangeDescriptions
            Case KblObjectType.Component_box, KblObjectType.Component_box_occurrence
                Return TabNames.tabComponentBoxes
            Case KblObjectType.Component, KblObjectType.Component_occurrence
                Return TabNames.tabComponents
            Case KblObjectType.Connector_occurrence, KblObjectType.Connector_housing
                Return TabNames.tabConnectors
            Case KblObjectType.Co_pack_occurrence, KblObjectType.Co_pack_part, KblObjectType.Specified_co_pack_occurrence
                Return TabNames.tabCoPacks
            Case KblObjectType.Dimension_specification
                Return TabNames.tabDimSpecs
            Case KblObjectType.Default_dimension_specification
                Return TabNames.tabDefDimSpecs
            Case KblObjectType.Fixing, KblObjectType.Fixing_occurrence
                Return TabNames.tabFixings
            Case KblObjectType.Module
                Return TabNames.tabModules
            Case KblObjectType.Net
                Return TabNames.tabNets
            Case KblObjectType.Redlining
                Return TabNames.tabRedlinings
            Case KblObjectType.Segment
                Return TabNames.tabSegments
            Case KblObjectType.Node
                Return TabNames.tabVertices
            Case KblObjectType.Wire_occurrence, KblObjectType.Core_occurrence, KblObjectType.General_wire, KblObjectType.General_wire_occurrence
                Return TabNames.tabWires
            Case KblObjectType.Harness
                Return TabNames.tabHarness
            Case KblObjectType.QMStamp
                Return TabNames.tabQMStamps
            Case Else
                Throw New NotImplementedException($"KblObjectType ""{kblObjectType.ToString}"" not implemented!")
        End Select
    End Function

    Public Shared Function GetKblObjectType(grid As UltraGridBase) As KblObjectType
        Select Case grid.Name
            Case NameOf(InformationHub.ugHarness)
                Return KblObjectType.Harness
            Case NameOf(InformationHub.ugModules)
                Return KblObjectType.Module
            Case NameOf(InformationHub.ugVertices)
                Return KblObjectType.Node
            Case NameOf(InformationHub.ugSegments)
                Return KblObjectType.Segment
            Case NameOf(InformationHub.ugAccessories)
                Return KblObjectType.Accessory_occurrence
            Case NameOf(InformationHub.ugFixings)
                Return KblObjectType.Fixing_occurrence
            Case NameOf(InformationHub.ugComponents)
                Return KblObjectType.Component_occurrence
            Case NameOf(InformationHub.ugConnectors)
                Return KblObjectType.Connector_occurrence
            Case NameOf(InformationHub.ugCables)
                Return KblObjectType.Special_wire_occurrence
            Case NameOf(InformationHub.ugWires)
                Return KblObjectType.Wire_occurrence
            Case NameOf(InformationHub.ugNets)
                Return KblObjectType.Net
            Case NameOf(InformationHub.ugRedlinings)
                Return KblObjectType.Redlining
            Case NameOf(InformationHub.ugComponentBoxes)
                Return KblObjectType.Component_box_occurrence
            Case NameOf(InformationHub.ugAssemblyParts)
                Return KblObjectType.Assembly_part_occurrence
            Case NameOf(InformationHub.ugChangeDescriptions)
                Return KblObjectType.Change_description
            Case NameOf(InformationHub.ugCoPacks)
                Return KblObjectType.Co_pack_occurrence
            Case NameOf(InformationHub.ugApprovals)
                Return KblObjectType.Approval
            Case NameOf(InformationHub.ugDifferences)
                Return KblObjectType.Undefined
            Case NameOf(InformationHub.ugDimSpecs)
                Return KblObjectType.Dimension_specification
            Case NameOf(InformationHub.ugDefDimSpecs)
                Return KblObjectType.Default_dimension_specification
            Case NameOf(InformationHub.ugQMStamps)
                Return KblObjectType.QMStamp
            Case Else
                Throw New NotImplementedException($"Mapping for grid type {grid.Name} not implemented!")
        End Select
    End Function

    Friend Shared Function GetTabName(grid As UltraGridBase) As TabNames
        Dim obj_type As KblObjectType = GetKblObjectType(grid)
        If obj_type <> KblObjectType.Undefined Then
            Return GetTabNameFromKblObjectType(obj_type)
        End If
        Return TabNames.tabNone
    End Function

End Class
