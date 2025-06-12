Imports Infragistics.Win.UltraWinGrid

''' <summary>
''' This Comparer does numericStringSortComparison when no marked rows are selected, when selected the markedRowSortComparison is executed with higher priority than the numericStringSortComparer on rows that should be marked and/or are marked
''' </summary>
Friend Class MarkedRowsNumericStringSortComparer
    Inherits NumericStringSortComparer

    Private _rowInfos As RowFilterInfo

    Public Sub New(rowInfos As RowFilterInfo)
        _rowInfos = rowInfos
    End Sub

    Public Overrides Function Compare(x As Object, y As Object) As Integer
        If TypeOf x Is UltraGridCell AndAlso TypeOf y Is UltraGridCell Then
            Dim xCell As UltraGridCell = DirectCast(x, UltraGridCell)
            Dim yCell As UltraGridCell = DirectCast(y, UltraGridCell)
            Dim xMarkingRow As Boolean = IsMarkingRow(xCell.Row)
            Dim yMarkingRow As Boolean = IsMarkingRow(yCell.Row)

            If xMarkingRow OrElse yMarkingRow Then
                If xMarkingRow Then
                    Return -1 ' bring x-row at top position's (x is less
                Else
                    Return 1 ' bring y-row at top position's (y is less)
                End If
            End If
        End If

        If NumericStringComparerEnabled Then
            Return MyBase.Compare(x, y)
        End If

        Return 0
    End Function

    Property NumericStringComparerEnabled As Boolean = True

    Public Function IsMarkingRow(row As UltraGridRow) As Boolean
        Return FindMarkingRows({row}, _rowInfos).Count > 0
    End Function

    Public Shared Function FindMarkingRows(inRowsCollection As IEnumerable(Of UltraGridRow), rowInfos As RowFilterInfo) As List(Of RowMarkableResult)
        If rowInfos.MarkedRows.Count > 0 Then
            Dim findInRowsList As New List(Of UltraGridRow)(inRowsCollection.Distinct)
            Dim results As New Dictionary(Of UltraGridRow, RowMarkableResult)
            For Each row As UltraGridRow In findInRowsList.ToArray
                If rowInfos.MarkedRows.Contains(row) Then
                    results.Add(row, New RowMarkableResult(row, True))
                    findInRowsList.Remove(row)
                End If
            Next

            If findInRowsList.Count > 0 Then
                For Each grid_group As IGrouping(Of UltraGridBase, UltraGridRow) In findInRowsList.GroupBy(Function(r) r.Band.Layout.Grid)
                    Dim result As List(Of RowMarkableResult) = rowInfos.InfoHub.KblIdRowCache.FindRows(grid_group, rowInfos.MarkedRows.ActiveGridKblIds)
                    If result.Count > 0 Then
                        For Each mRowResult As RowMarkableResult In result.Where(Function(res) res.CanBeMarkRow)
                            results.TryAdd(mRowResult.Row, mRowResult)
                        Next
                    End If
                Next
            End If
            Return results.Values.ToList
        End If
        Return New List(Of RowMarkableResult)
    End Function

End Class
