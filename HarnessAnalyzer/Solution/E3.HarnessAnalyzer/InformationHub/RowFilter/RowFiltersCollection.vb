Imports Infragistics.Win.UltraWinGrid
Imports Zuken.E3.Lib.Eyeshot.Model

Friend Class RowFiltersCollection
    Inherits System.Collections.ObjectModel.Collection(Of BaseRowFilter)
    Implements IDisposable

    Private _info As RowFilterInfo

    Public Sub New(info As RowFilterInfo)
        MyBase.New
        _info = info
    End Sub

    Public Sub AddNew(Of T As BaseRowFilter)(grid As UltraGrid)
        Dim rowFilter As T = Nothing
        rowFilter = CType(Activator.CreateInstance(GetType(T), True), T)

        rowFilter.Info = Me._info
        rowFilter.Grid = grid
        Me.Add(rowFilter)
    End Sub

    Public Sub AddNewInactiveObjectsRowFilter(grid As UltraGrid)
        Dim inactiveFilter As New BaseRowFilter(Me._info, grid)
        Me.Add(inactiveFilter)
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        For Each rw As BaseRowFilter In Me
            rw.Dispose()
        Next

        Me.Clear()
        _info = Nothing
    End Sub

    Public Sub Refresh(inactiveModules As Dictionary(Of String, E3.Lib.Schema.Kbl.[Module]), hideEntitiesWithNoModules As Boolean)
        _info.Refresh(inactiveModules, hideEntitiesWithNoModules)
        Me.Refresh()
    End Sub

    Public Sub Refresh()
        For Each rw As BaseRowFilter In Me
            rw.Refresh()
        Next
    End Sub

    Public Overloads Function IsFiltered(entity As IBaseModelEntity) As Boolean
        For Each kbl_obj As IKblBaseObject In entity.GetKblObjects
            If IsFiltered(kbl_obj) Then
                Return True
            End If
        Next
        Return False
    End Function

    Public Overloads Function IsFiltered(kblObject As IKblBaseObject) As Boolean
        Return IsKblObjectFiltered(kblObject.ObjectType, kblObject.SystemId)
    End Function

    Public Function IsKblObjectFiltered(kblObjectType As KblObjectType, kblSystemId As String) As Boolean
        Dim filter As KblObjectFilter = Me.Where(Function(f) f.KblObjectTypes.Contains(kblObjectType)).FirstOrDefault
        If filter IsNot Nothing AndAlso filter.FilterKblObject(kblSystemId).HasFlag(KblObjectFilterType.Filtered) Then
            Return True
        End If
        Return False
    End Function

End Class
