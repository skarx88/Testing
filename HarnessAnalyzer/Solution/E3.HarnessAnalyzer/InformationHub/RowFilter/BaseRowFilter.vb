Imports System.ComponentModel
Imports Infragistics.Win.UltraWinGrid

Friend Class BaseRowFilter
    Inherits KblObjectFilter

    Private WithEvents _grid As UltraGrid

    Protected Sub New()
        Me.New(Nothing, Nothing)
    End Sub

    Public Sub New(info As RowFilterInfo, grid As UltraGrid, Optional disableFilteredRowsOnly As Boolean = False)
        MyBase.New(info)
        _grid = grid

        Me.DisableFilteredRowsOnly = disableFilteredRowsOnly
        Me.Init()
    End Sub

    Private Sub Init()
        If _grid IsNot Nothing AndAlso Me.KblObjectTypes.Length = 0 Then
            Me.KblObjectTypes = {InformationHubUtils.GetKblObjectType(_grid)}
        End If
    End Sub

    Protected Friend Overridable Property Grid As UltraGrid
        Get
            Return _grid
        End Get
        Set
            If _grid IsNot Value Then
                _grid = Value
                Init()
            End If
        End Set
    End Property

    Property Enabled As Boolean = True

    Friend Property DisableFilteredRowsOnly As Boolean

    Public Overridable Function DoRowFilter(row As UltraGridRow, Optional overrideKblObjectType As Nullable(Of KblObjectType) = Nothing) As KblObjectFilterType
        Dim result As KblObjectFilterType
        If overrideKblObjectType.HasValue Then
            result = Me.FilterKblObject(InformationHubUtils.GetKblIds(row).FirstOrDefault, overrideKblObjectType.Value)
        Else
            result = Me.FilterKblObject(InformationHubUtils.GetKblIds(row).FirstOrDefault)
        End If

        If result.HasFlag(KblObjectFilterType.Filtered) Then
            Return result
        End If
        Return KblObjectFilterType.Unfiltered
    End Function

    Protected Overridable Function CanFilterRow(row As UltraGridRow) As Boolean
        Return True
    End Function

    Private Sub _grid_FilterRow(sender As Object, e As FilterRowEventArgs) Handles _grid.FilterRow
        If Enabled Then
            PreRowFilter(e.Row)
            If CanFilterRow(e.Row) Then
                InitRowDefaultValues(e.Row)
                Dim result As KblObjectFilterType = DoRowFilter(e.Row)
                If result.HasFlag(KblObjectFilterType.ForceGrayOut) Then
                    e.Row.CellAppearance.ForeColor = Color.Gray
                Else
                    e.Row.Appearance.ForeColor = e.Row.Band.Layout.Appearance.ForeColor
                    If result.HasFlag(KblObjectFilterType.Filtered) Then
                        e.RowFilteredOut = True
                    End If
                End If
            End If
        End If
    End Sub

    Protected Overridable Sub PreRowFilter(row As UltraGridRow)
    End Sub

    Private Sub _grid_BindingContextChanged(sender As Object, e As EventArgs) Handles _grid.BindingContextChanged
        _grid.DisplayLayout.Override.ExpansionIndicator = ShowExpansionIndicator.CheckOnExpand
        For Each band As UltraGridBand In _grid.DisplayLayout.Bands
            If _grid.DisplayLayout.Bands.Count > 1 Then
                band.Override.ExpansionIndicator = ShowExpansionIndicator.CheckOnExpand
            Else
                band.Override.ExpansionIndicator = ShowExpansionIndicator.Never
            End If

            If band.ParentBand Is Nothing Then
                band.Override.RowFilterAction = If(_DisableFilteredRowsOnly, RowFilterAction.DisableFilteredOutRows, RowFilterAction.HideFilteredOutRows)
            Else
                band.Override.RowFilterAction = RowFilterAction.HideFilteredOutRows
            End If
        Next
    End Sub

    Private Sub InitRowDefaultValues(row As UltraGridRow)
        If row.IsFilteredOut OrElse row.Hidden AndAlso (row.Selected OrElse row.IsActiveRow) Then
            Using Me.Grid.EventManager.ProtectProperty(NameOf(GridEventManager.AllEventsEnabled), False) ' ClearAllSelections before they get shown -> old BL:ClearSelectedRowsInGrids
                row.Selected = False
                row.Activated = False
            End Using
        End If

        If Not Me.Info.IsActiveGridMarkingRow(row) Then
            InformationHubUtils.ResetMarkedRowAppearance(row)
        Else
            InformationHubUtils.SetMarkedRowAppearance(row)
        End If
    End Sub

    Public Overridable Sub Refresh()
        If _grid IsNot Nothing Then
            _grid.DisplayLayout.RefreshFilters()
        End If
    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)
        MyBase.Dispose(disposing)
        _grid = Nothing
    End Sub

    Private Sub _grid_BeforeRowExpanded(sender As Object, e As CancelableRowEventArgs) Handles _grid.BeforeRowExpanded
        e.Cancel = e.Row.Hidden OrElse e.Row.IsFilteredOut
    End Sub

End Class