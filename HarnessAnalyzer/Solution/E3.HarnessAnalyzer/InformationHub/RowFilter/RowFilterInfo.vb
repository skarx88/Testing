Imports System.ComponentModel
Imports Infragistics.Win.UltraWinGrid
Imports Infragistics.Win.UltraWinTabControl

Friend Class RowFilterInfo
    Implements IDisposable

    Public Event ActiveGridChanged(sender As Object, e As EventArgs)

    Private WithEvents _infoHub As InformationHub
    Private WithEvents _markedRows As New MarkedRowsCollection(Me)

    Private _sealsOnPlugReplacements As Dictionary(Of String, List(Of String))
    Private _disposedValue As Boolean
    Private _activeMarkRowsInvalid As Boolean = False
    Private _activeGridMarkableRows As New List(Of RowMarkableResult)
    Private _validSortedGrids As New HashSet(Of UltraGridBase)

    Public Sub New(infoHub As InformationHub) 'TODO: get rid of the info-hub reference
        _infoHub = infoHub
    End Sub

    Public ReadOnly Property KBL As KblMapper
        Get
            Return _infoHub?.Kbl
        End Get
    End Property

    Public Property InfoHub As InformationHub
        Get
            Return _infoHub
        End Get
        Set(value As InformationHub)
            _infoHub = value
        End Set
    End Property

    Public ReadOnly Property InactiveObjects As New TypeGroupedKblIdsDictionary
    Public ReadOnly Property ActiveSegments As New HashSet(Of String)
    Public ReadOnly Property SegmentsWithInactiveProtections As New HashSet(Of String)
    Public ReadOnly Property ActiveWiresAndCores As New HashSet(Of String)
    Public ReadOnly Property ActiveSegmentsByRouting As New HashSet(Of String)
    Public ReadOnly Property ActiveObjects As New ActiveObjectIdsCollection

    Public ReadOnly Property MarkedRows As MarkedRowsCollection
        Get
            Return _markedRows
        End Get
    End Property

    Public ReadOnly Property ActiveGridMarkableRows As List(Of RowMarkableResult)
        Get
            If _activeMarkRowsInvalid Then
                RefreshActiveGridMarkedRows()
            End If
            Return _activeGridMarkableRows
        End Get
    End Property

    Public ReadOnly Property ActiveGrid As UltraGrid
        Get
            Return _infoHub.ActiveGrid
        End Get
    End Property

    Public ReadOnly Property RedliningInfo As RedliningInformation
        Get
            Return _infoHub.RedliningInfo
        End Get
    End Property

    Public ReadOnly Property SealsOnPlugReplacements As Dictionary(Of String, List(Of String))
        Get
            If _sealsOnPlugReplacements Is Nothing Then
                RefreshSealsOnPlugReplacements() ' HINT special initial case - the Refresh was not called initial
            End If
            Return _sealsOnPlugReplacements
        End Get
    End Property

    Public Function GetActiveObjectsOrNull() As ActiveObjectIdsCollection
        If Me.ActiveObjects.IsInitialized Then
            Return Me.ActiveObjects
        End If
        Return Nothing
    End Function

    Private Function GetActiveGridMarkingRows() As List(Of RowMarkableResult)
        Return MarkedRowsNumericStringSortComparer.FindMarkingRows(Me.ActiveGrid.Rows, Me)
    End Function

    Friend Function IsActiveGridMarkingRow(row As UltraGridRow) As Boolean
        Return Me.ActiveGridMarkableRows.Any(Function(row_res) row_res.Row Is row OrElse (row_res.HasChildren AndAlso row_res.Children.Contains(row)))
    End Function

    Private Sub RefreshActiveGridMarkedRows()
        ActiveGrid.Rows.Refresh(RefreshRow.FireInitializeRow) ' HINT: if the rows are not initialized it will be done here before accessing to the row.tag-objects in the following BL
        OnActiveGridChanged(New EventArgs) ' HINT: must come before GetActiveGridMarkingRows is called because the BL relays on that event (needs the activeGridMarkedKblIds which are rebuild on this event)
        _activeGridMarkableRows.Clear()
        _activeGridMarkableRows.AddRange(Me.GetActiveGridMarkingRows)
        _activeMarkRowsInvalid = False
    End Sub

    Public Function RefreshSortingOfActiveMarkedRows(Optional invalidate As Boolean = True, Optional ensureVisible As Boolean = True) As Boolean
        Return RefreshSortingOfMarkedRowsCore(Me.ActiveGridMarkableRows, invalidate, ensureVisible)
    End Function

    Private Function RefreshSortingOfMarkedRowsCore(markingRows As IEnumerable(Of RowMarkableResult), invalidate As Boolean, ensureVisible As Boolean) As Boolean
        Dim updatedAny As Boolean = False
        For Each group_grid As IGrouping(Of UltraGridBase, RowMarkableResult) In markingRows.GroupBy(Function(r_res) r_res.Row.Band.Layout.Grid)
            If _validSortedGrids.Add(group_grid.Key) Then
                group_grid.Key.BeginUpdate()

                For Each result As RowMarkableResult In markingRows
                    result.Row.RefreshSortPosition()
                Next

                Dim first_row_res As RowMarkableResult = markingRows.FirstOrDefault
                If first_row_res IsNot Nothing Then
                    If ensureVisible Then
                        InformationHubUtils.EnsureRowsVisible(markingRows)
                    End If
                End If

                group_grid.Key.EndUpdate(invalidate)
                updatedAny = True
            End If
        Next
        Return updatedAny
    End Function

    Public Sub RefreshSortingOfMarkedRows(rows As IEnumerable(Of UltraGridRow), Optional invalidate As Boolean = True, Optional ensureVisible As Boolean = True)
        RefreshSortingOfMarkedRowsCore(MarkedRowsNumericStringSortComparer.FindMarkingRows(rows, Me), invalidate, ensureVisible)
    End Sub

    Public Sub Refresh(inactiveModules As Dictionary(Of String, E3.Lib.Schema.Kbl.[Module]), hideEntitiesWithNoModules As Boolean)
        Me.ActiveObjects.InitializeOrReset()
        Me.ActiveWiresAndCores.Clear()
        Me.ActiveSegmentsByRouting.Clear()
        Me.ActiveSegments.Clear()
        Me.InactiveObjects.Clear()
        Me.SegmentsWithInactiveProtections.Clear()

        For Each [module] As E3.Lib.Schema.Kbl.[Module] In KBL.GetModules
            Dim objectsOfModule As IGroupingKblObjects = Nothing
            If Not inactiveModules.ContainsKey([module].SystemId) AndAlso (KBL.KBLModuleObjectMapper.TryGetValue([module].SystemId, objectsOfModule)) Then
                'Handle all active objects with module assignment
                For Each kblObject As IKblBaseObject In objectsOfModule.Cast(Of IKblBaseObject).ToArray
                    If ActiveObjects.Add(kblObject.SystemId) Then ' HINT: skip when already exists (this is a hashset collection)
                        Select Case kblObject.ObjectType
                            Case KblObjectType.Wire_occurrence, KblObjectType.Special_wire_occurrence
                                ActiveWiresAndCores.TryAdd(kblObject.SystemId)
                                ActiveSegmentsByRouting.TryAddRange(KBL.KBLWireSegmentMapper.GetSegmentIdsOrEmpty(kblObject.SystemId))
                            Case KblObjectType.Segment
                                ActiveSegments.TryAdd(kblObject.SystemId)
                        End Select
                    End If
                Next
            End If
        Next

        RefreshCore(inactiveModules, hideEntitiesWithNoModules)
        RefreshSealsOnPlugReplacements()
    End Sub

    Private Sub RefreshCore(inactiveModules As Dictionary(Of String, E3.Lib.Schema.Kbl.[Module]), hideEntitiesWithNoModules As Boolean)
        For Each wireOcc As Wire_occurrence In KBL.KBLWireList
            UpdateActiveSegmentStateForWiresAndCablesWithoutModulAssignment(wireOcc)
        Next

        For Each cabOcc As Special_wire_occurrence In KBL.KBLCableList
            UpdateActiveSegmentStateForWiresAndCablesWithoutModulAssignment(cabOcc)
        Next

        If hideEntitiesWithNoModules Then
            Dim moduleControlledTypes As New List(Of KblObjectType) From {
                    KblObjectType.Accessory_occurrence,
                    KblObjectType.Assembly_part_occurrence,
                    KblObjectType.Cavity_plug_occurrence,
                    KblObjectType.Cavity_seal_occurrence,
                    KblObjectType.Co_pack_occurrence,
                    KblObjectType.Component_box_occurrence,
                    KblObjectType.Component_occurrence,
                    KblObjectType.Connection,
                    KblObjectType.Connector_occurrence,
                    KblObjectType.Core_occurrence,
                    KblObjectType.Fixing_occurrence,
                    KblObjectType.Fuse_occurrence,
                    KblObjectType.Special_terminal_occurrence,
                    KblObjectType.Special_wire_occurrence,
                    KblObjectType.Terminal_occurrence,
                    KblObjectType.Wire_occurrence,
                    KblObjectType.Wire_protection_occurrence,
                    KblObjectType.Wiring_group,
                    KblObjectType.Dimension_specification
                }

            For Each occurrenceMap As KeyValuePair(Of String, IKblOccurrence) In KBL.KBLOccurrenceMapper.Where(Function(map) moduleControlledTypes.Contains(map.Value.GetType.GetKblObjectType) AndAlso Not KBL.KBLObjectModuleMapper.ContainsKey(map.Key))
                If Not ActiveObjects.Contains(occurrenceMap.Key) Then
                    Dim kblObjType As KblObjectType = occurrenceMap.Value.GetType.GetKblObjectType
                    _InactiveObjects.GetOrAddNew(kblObjType).Add(occurrenceMap.Key)
                End If
            Next
        End If

        For Each inactiveModule_id As String In inactiveModules.Keys
            'HINT Handle all inactive objects with module assignment
            For Each kblObject As IKblBaseObject In KBL.GetObjectsOfModule(inactiveModule_id).OrEmpty.Cast(Of IKblBaseObject)
                'Hint: this is used to remove dimensions if not both ends are in active modules.Unclear if this will work? MR
                If kblObject.ObjectType = KblObjectType.Dimension_specification Then
                    ActiveObjects.Remove(kblObject.SystemId)
                End If

                If Not ActiveObjects.Contains(kblObject.SystemId) Then
                    If Not InactiveObjects.GetOrAddNew(kblObject.ObjectType).Contains(kblObject.SystemId) Then
                        Select Case kblObject.ObjectType
                            Case KblObjectType.Segment
                                If ActiveSegmentsByRouting.Contains(kblObject.SystemId) OrElse (Not hideEntitiesWithNoModules AndAlso KBL.KBLSegmentWireMapper.GetValueOrEmpty(kblObject.SystemId).Count = 0) Then
                                    'HINT keep segment if it has active wires or if it is fully empty, even if it is inactive by protection
                                Else
                                    _InactiveObjects(kblObject.ObjectType).Add(kblObject.SystemId)
                                End If
                            Case KblObjectType.Node
                                'HINT these nodes come here if directly controlled and are to be removed at a first glance. They need to remain if at least one segment has active routing or active protection or if it is controlled and not hidden
                                If KBL.KBLVertexSegmentMapper.ContainsKey(kblObject.SystemId) Then
                                    If Not IsNodeActive(kblObject.SystemId, hideEntitiesWithNoModules) Then
                                        _InactiveObjects(kblObject.ObjectType).Add(kblObject.SystemId)
                                    End If
                                Else
                                    InactiveObjects(kblObject.ObjectType).Add(kblObject.SystemId)
                                End If
                            Case KblObjectType.Wire_protection_occurrence
                                If KBL.KBLProtectionSegmentMapper.ContainsKey(kblObject.SystemId) Then
                                    _SegmentsWithInactiveProtections.TryAddRange(KBL.KBLProtectionSegmentMapper(kblObject.SystemId))
                                End If
                                _InactiveObjects(kblObject.ObjectType).Add(kblObject.SystemId)
                            Case Else
                                _InactiveObjects(kblObject.ObjectType).Add(kblObject.SystemId)
                        End Select
                    End If
                End If
            Next
        Next

        For Each entry As KeyValuePair(Of String, ControlState) In KBL.KBLSegmentControl
            Select Case entry.Value
                Case ControlState.IndirectCtrld
                    If Not ActiveSegmentsByRouting.Contains(entry.Key) Then
                        _InactiveObjects.GetOrAddNew(KblObjectType.Segment).Add(entry.Key) ' valueCollection are hash-set, not check if exists are needed
                    End If
                Case ControlState.NotCtrld
                    If hideEntitiesWithNoModules Then
                        _InactiveObjects.GetOrAddNew(KblObjectType.Segment).Add(entry.Key) ' valueCollection are hash-set, not check if exists are needed
                    End If
            End Select
        Next

        For Each entry As KeyValuePair(Of String, ControlState) In KBL.KBLNodeControl
            Select Case entry.Value
                Case ControlState.IndirectCtrld
                    If KBL.KBLVertexSegmentMapper.ContainsKey(entry.Key) Then
                        If Not IsNodeActive(entry.Key, hideEntitiesWithNoModules) Then
                            _InactiveObjects.GetOrAddNew(KblObjectType.Node).Add(entry.Key)
                        End If
                    End If
                Case ControlState.NotCtrld
                    If hideEntitiesWithNoModules Then
                        _InactiveObjects.GetOrAddNew(KblObjectType.Node).Add(entry.Key)
                    End If
            End Select
        Next
    End Sub

    Private Function IsNodeActive(nodeId As String, hideEntitiesWithNoModules As Boolean) As Boolean
        For Each sid As String In KBL.KBLVertexSegmentMapper(nodeId)
            If ActiveSegmentsByRouting.Contains(sid) Then
                Return True
            End If

            If Not hideEntitiesWithNoModules AndAlso KBL.KBLSegmentWireMapper.GetValueOrEmpty(sid).Count = 0 Then
                Return True
            End If

            Dim seg As Segment = TryCast(KBL.KBLOccurrenceMapper(sid), Segment)
            If seg.Protection_area.OrEmpty.Length > 0 Then
                If ActiveObjects.ContainsAnyOf(seg.Protection_area.Select(Function(pa) pa.Associated_protection)) Then
                    Return True
                End If
            End If
        Next
        Return False
    End Function

    Private Sub RefreshSealsOnPlugReplacements()
        _sealsOnPlugReplacements.NewOrClear
        'HINT id = plugId, list of seals ids which replace plug
        For Each seal As Cavity_seal_occurrence In KBL.GetCavitySealOccurrences
            If seal.Replacing?.Length > 0 Then
                For Each plg As Part_substitution In seal.Replacing
                    If Not String.IsNullOrEmpty(plg.Replaced) Then
                        _sealsOnPlugReplacements.GetOrAddNew(plg.Replaced).Add(seal.SystemId)
                    End If
                Next
            End If
        Next
    End Sub

    Private Sub UpdateActiveSegmentStateForWiresAndCablesWithoutModulAssignment(wireOrCable As General_wire_occurrence)
        If Not KBL.KBLObjectModuleMapper.ContainsKey(wireOrCable.SystemId) Then
            If Not ActiveObjects.Contains(wireOrCable.SystemId) Then
                ActiveSegmentsByRouting.TryAddRange(KBL.KBLWireSegmentMapper.GetSegmentIdsOrEmpty(wireOrCable.SystemId))
            End If
        End If
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not _disposedValue Then
            If disposing Then
                _validSortedGrids?.Clear()
                _markedRows?.Dispose()
            End If

            _validSortedGrids = Nothing
            _sealsOnPlugReplacements = Nothing
            _activeGridMarkableRows = Nothing
            _InactiveObjects = Nothing
            _disposedValue = True
            _markedRows = Nothing
            _infoHub = Nothing
        End If
    End Sub

    Public Function FindRows(inRowsCollection As IEnumerable(Of UltraGridRow), kblIds As IEnumerable(Of String)) As List(Of RowMarkableResult)
        If InfoHub IsNot Nothing Then
            Return InfoHub.KblIdRowCache.FindRows(inRowsCollection, kblIds)
        End If
        Return New List(Of RowMarkableResult)
    End Function

    Protected Overridable Sub OnActiveGridChanged(e As EventArgs)
        RaiseEvent ActiveGridChanged(Me, e)
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub

    Private Sub _infoHub_ActiveTabChanged(sender As Object, e As ActiveTabChangedEventArgs) Handles _infoHub.ActiveTabChanged
        RefreshActiveGridMarkedRows()  ' HINT: updates the information which row is to be declared as marked in the active grid
        RefreshSortingOfActiveMarkedRows(invalidate:=False, ensureVisible:=True) 'HINT: invalidates the markedrow-colum-Sorters on the active grid
        Me.InfoHub?.RowFiltes?.Refresh() ' HINT: invalidate the filters -> when row is drawn/filtered-in/-out the color of the marking row will be set over the filters
    End Sub

    Private Sub _markedRows_CollectionChanged(sender As Object, e As CollectionChangeEventArgs) Handles _markedRows.CollectionChanged
        _activeMarkRowsInvalid = True
        _validSortedGrids.Clear()
    End Sub

    Public Class MarkedRowsCollection
        Inherits System.Collections.Generic.HashList(Of UltraGridRow)
        Implements IDisposable

        Public Event CollectionChanged(sender As Object, e As System.ComponentModel.CollectionChangeEventArgs)

        Private _kblIds As New HashSet(Of String)
        Private _activeGridIdsInvalid As Boolean = False
        Private _activeGridKblIds As New List(Of String)
        Private _disposedValue As Boolean
        Private WithEvents _parent As RowFilterInfo

        Public Sub New(parent As RowFilterInfo)
            _parent = parent
        End Sub

        Protected Overrides Sub OnAfterAddItem(item As UltraGridRow)
            MyBase.OnAfterAddItem(item)
            _kblIds.AddRange(InformationHubUtils.GetKblIds(item))
            OnCollectionChanged(New System.ComponentModel.CollectionChangeEventArgs(System.ComponentModel.CollectionChangeAction.Add, item))
        End Sub

        Protected Overrides Sub OnAfterRemovedItems(items As IEnumerable(Of UltraGridRow))
            For Each id As String In (InformationHubUtils.GetKblIds(items.ToArray))
                _kblIds.Remove(id)
            Next
            OnCollectionChanged(New System.ComponentModel.CollectionChangeEventArgs(System.ComponentModel.CollectionChangeAction.Remove, items))
            MyBase.OnAfterRemovedItems(items)
        End Sub

        Protected Overridable Sub OnCollectionChanged(e As System.ComponentModel.CollectionChangeEventArgs)
            RaiseEvent CollectionChanged(Me, e)
        End Sub

        Private Function ResolveActiveGridKblIds() As List(Of String)
            If _parent.ActiveGrid IsNot Nothing AndAlso Me.Count > 0 AndAlso Me.First.Band.Layout.Grid IsNot _parent.ActiveGrid Then ' HINT: grid is different from where the marked rows have been set -> get the current grid corresponding kblids from the marked rows of the other grid
                Return InformationHubUtils.GetCorrespondingKblIdsFrom(_kblIds, IncludeCorrespondingIdsInfo.GetTrue(_parent.KBL, _parent.ActiveGrid), _parent.RedliningInfo)
            Else
                Return _kblIds.ToList
            End If
        End Function

        Public ReadOnly Property ActiveGridKblIds As List(Of String)
            Get
                If _activeGridIdsInvalid Then
                    _activeGridKblIds = ResolveActiveGridKblIds()
                    _activeGridIdsInvalid = False
                End If
                Return _activeGridKblIds
            End Get
        End Property

        Private Sub _parent_ActiveGridChanged(sender As Object, e As EventArgs) Handles _parent.ActiveGridChanged
            _activeGridIdsInvalid = True
        End Sub

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not _disposedValue Then
                If disposing Then
                    BaseClearItems() ' don't call the Me.Clear-method because the OnAfterItemsRemoved and OnAfterItemsAdded should not be triggered on dispose
                End If

                _activeGridIdsInvalid = Nothing
                _activeGridKblIds = Nothing
                _parent = Nothing
                _kblIds = Nothing
                _disposedValue = True
            End If
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub

    End Class

End Class
