Imports Infragistics.Win.UltraWinGrid

Friend Class KblIdRowCache
    Inherits DisposableObject

    Private _rowsByKblId As New Dictionary(Of String, HashSet(Of UltraGridRow))

    Public Sub New()
        MyBase.New
    End Sub

    Public Sub New(rebuildOnRows As IEnumerable(Of UltraGridRow))
        Me.New
        TryAddRowsRange(rebuildOnRows)
    End Sub

    Public Sub TryAddRowsRange(rows As IEnumerable(Of UltraGridRow))
        For Each row As UltraGridRow In rows
            TryAddRow(row)
        Next
    End Sub

    Public Sub TryAddRow(row As UltraGridRow)
        Dim kblIds As List(Of String) = InformationHubUtils.GetKblIds(row)
        For Each kblId As String In kblIds
            _rowsByKblId.GetOrAddNew(kblId).Add(row)
        Next

        If row.HasChild Then
            For Each childRow As UltraGridRow In row.ChildBands(0).Rows
                If Not childRow.Hidden AndAlso childRow.Tag IsNot Nothing Then
                    'Dim dic_child_rows As Dictionary(Of UltraGridRow, HashSet(Of String)) = _childsByRow.GetOrAddNew(row)
                    Dim childKblIds As List(Of String) = InformationHubUtils.GetKblIds(childRow)
                    'dic_child_rows.GetOrAddNew(childRow).AddRange(childKblIds)

                    For Each child_kblId As String In childKblIds
                        _rowsByKblId.GetOrAddNew(child_kblId).Add(childRow)
                    Next
                End If
            Next
        End If
    End Sub

    Public Function ContainsAny(kblIds As IEnumerable(Of String), Optional restrictToGrid As UltraGridBase = Nothing) As Boolean
        For Each id As String In kblIds
            Dim rows As HashSet(Of UltraGridRow) = Nothing
            If _rowsByKblId.TryGetValue(id, rows) Then
                If restrictToGrid IsNot Nothing AndAlso (rows?.Count).GetValueOrDefault > 0 Then
                    For Each row As UltraGridRow In rows
                        If row.Band.Layout.Grid Is restrictToGrid Then
                            Return True
                        End If
                    Next
                Else
                    Return (rows?.Count).GetValueOrDefault > 0
                End If
            End If
        Next
        Return False
    End Function

    Private Function FindKblIdRowInGrid(kblId As String, grid As UltraGridBase, Optional ByRef inChildsRowFound As IEnumerable(Of UltraGridRow) = Nothing) As UltraGridRow
        Dim childRows As New HashSet(Of UltraGridRow)
        Dim root As UltraGridRow = Nothing
        For Each row As UltraGridRow In FindRowsWithKblId(kblId)
            If row.Band.Layout.Grid Is grid Then
                If row.HasParent Then
                    childRows.Add(row)
                    root = row.ParentRow
                Else
                    root = row
                End If
            End If
        Next
        inChildsRowFound = childRows
        Return root
    End Function

    Private Function FindRowsWithKblId(kblId As String) As UltraGridRow()
        Dim rows As HashSet(Of UltraGridRow) = Nothing
        If _rowsByKblId.TryGetValue(kblId, rows) Then
            Return rows.ToArray
        End If
        Return Array.Empty(Of UltraGridRow)
    End Function

    Public Function FindRows(inRowsCollection As IEnumerable(Of UltraGridRow), kblIds As IEnumerable(Of String)) As List(Of RowMarkableResult)
        Dim result As New List(Of RowMarkableResult)
        If kblIds.Any() Then
            For Each grid_group As IGrouping(Of UltraGridBase, UltraGridRow) In inRowsCollection.GroupBy(Function(r) r.Band.Layout.Grid)
                TryAddRowsRange(grid_group)

                If ContainsAny(kblIds, grid_group.Key) Then
                    Dim rows_consolidated_found As New Dictionary(Of UltraGridRow, HashSet(Of UltraGridRow))
                    For Each kblId As String In kblIds
                        Dim inChildRowsFound As IEnumerable(Of UltraGridRow) = Nothing
                        Dim row As UltraGridRow = FindKblIdRowInGrid(kblId, grid_group.Key, inChildRowsFound)
                        If row IsNot Nothing Then
                            Dim children_list As HashSet(Of UltraGridRow) = rows_consolidated_found.GetOrAddNew(row)
                            If inChildRowsFound IsNot Nothing Then
                                children_list.AddRange(inChildRowsFound)
                            End If
                        End If
                    Next

                    For Each kv As KeyValuePair(Of UltraGridRow, HashSet(Of UltraGridRow)) In rows_consolidated_found
                        result.Add(New RowMarkableResult(kv.Key, kv.Value))
                    Next
                End If
            Next
        End If
        Return result
    End Function

    Public Sub Clear()
        _rowsByKblId.Clear()
    End Sub

    Public Sub Rebuild()
        Dim myRowsColl As List(Of UltraGridRow) = _rowsByKblId.SelectMany(Function(kv) kv.Value).ToList
        Me.Clear()
        Me.TryAddRowsRange(myRowsColl)
    End Sub

End Class
