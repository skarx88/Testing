Imports VectorDraw.Professional.vdCollections
Imports Zuken.E3.HarnessAnalyzer.Project.Documents
Imports Zuken.E3.Lib.Eyeshot.Model
Imports Zuken.E3.Lib.IO.KBL
Imports Zuken.E3.Lib.Model

Public Class CanvasSelection
    Implements IEnumerable(Of KeyValuePair(Of KblObjectType, HashSet(Of String)))

    Private _selectionByKblId As Dictionary(Of String, List(Of VdSVGGroup))
    Private _selectionByObjectType As New Concurrent.ConcurrentDictionary(Of KblObjectType, HashSet(Of String))

    Private _selection3DByContainerType As New Concurrent.ConcurrentDictionary(Of ContainerId, HashSet(Of String))

    Private _kblMapper As KblMapper
    Private _document As HcvDocument

    Private _issueIds As New List(Of String)
    Private _stampIds As New List(Of String)
    Private _redlingIds As New List(Of String)

    Public Sub New(selection As devDept.Eyeshot.ISelectedEntitiesCollection, document As HcvDocument)
        _Source = CanvasSelectionSource.From3D
        _document = document
        Initialize(selection)
    End Sub

    Public Sub New(selection As vdSelection, kblMapper As KblMapper)
        _Source = CanvasSelectionSource.From2D
        _kblMapper = kblMapper
        Initialize(selection)
    End Sub

    Private Sub Initialize(selection As vdSelection)
        RefreshSelectionByKblId(selection)
        RefreshSelectionByObjectType(selection)
    End Sub

    Private Sub Initialize(selection As devDept.Eyeshot.ISelectedEntitiesCollection)
        'RefreshSelectionByKblId(selection)
        RefreshSelectionByObjectType(selection.OfType(Of IBaseModelEntityEx))
    End Sub

    Friend ReadOnly Property Source As CanvasSelectionSource

    Private Sub RefreshSelectionByKblId(selection As vdSelection)
        _selectionByKblId = selection.OfType(Of VdSVGGroup).GroupBy(Function(svgGrp) svgGrp.KblId).ToDictionary(Function(grp) grp.Key, Function(grp) grp.ToList)
    End Sub

    Public Function RemoveWires() As Boolean
        Select Case Source
            Case CanvasSelectionSource.From2D
                Return _selectionByObjectType.TryRemove(KblObjectType.Wire_occurrence, Nothing)
            Case CanvasSelectionSource.From3D
                Return _selection3DByContainerType.TryRemove(ContainerId.Wires, Nothing)
            Case Else
                ThrowSourceNotImplemented()
        End Select
        Return False
    End Function

    <DebuggerNonUserCode>
    Private Sub ThrowSourceNotImplemented()
        Throw New NotImplementedException($"Source ""{Source.ToString}"" not implemented!")
    End Sub

    Public Sub AddWireIds(wireIds As IEnumerable(Of String))
        Select Case Source
            Case CanvasSelectionSource.From2D
                Dim list As HashSet(Of String) = _selectionByObjectType.GetOrAdd(KblObjectType.Wire_occurrence, Function() New HashSet(Of String))
                list.AddRange(wireIds)
            Case CanvasSelectionSource.From3D
                Dim list As HashSet(Of String) = _selection3DByContainerType.GetOrAdd(ContainerId.Wires, Function() New HashSet(Of String))
                list.AddRange(wireIds)
            Case Else
                ThrowSourceNotImplemented()
        End Select
    End Sub

    Public Sub AddRedlingsIds(redliningIds As IEnumerable(Of String), clear As Boolean)
        Select Case Source
            Case CanvasSelectionSource.From2D
                If clear Then
                    _selectionByObjectType.Clear()
                End If

                Dim redlinings As HashSet(Of String) = _selectionByObjectType.GetOrAdd(KblObjectType.Redlining, Function() New HashSet(Of String))
                redlinings.AddRange(redliningIds)
            Case CanvasSelectionSource.From3D
                ' TODO
            Case Else
                ThrowSourceNotImplemented()
        End Select
    End Sub


    Public Sub AddCavityParts(cavityPartIds As List(Of String))
        Select Case Source
            Case CanvasSelectionSource.From2D
                Dim cavities As HashSet(Of String) = _selectionByObjectType.GetOrAdd(KblObjectType.Cavity_occurrence, Function() New HashSet(Of String))
                cavities.AddRange(cavityPartIds)
            Case CanvasSelectionSource.From3D
                Dim list As HashSet(Of String) = _selection3DByContainerType.GetOrAdd(ContainerId.Cavities, Function() New HashSet(Of String))
                list.AddRange(cavityPartIds)
            Case Else
                ThrowSourceNotImplemented()
        End Select
    End Sub

    Public Sub AddCableIds(cableIds As String(), clear As Boolean)
        Select Case Source
            Case CanvasSelectionSource.From2D
                If clear Then
                    _selectionByObjectType.Clear()
                End If

                Dim list As HashSet(Of String) = _selectionByObjectType.GetOrAdd(KblObjectType.Special_wire_occurrence, Function() New HashSet(Of String))
                list.AddRange(cableIds)
            Case CanvasSelectionSource.From3D
                If clear Then
                    _selection3DByContainerType.Clear()
                End If

                Dim list As HashSet(Of String) = _selection3DByContainerType.GetOrAdd(ContainerId.Cables, Function() New HashSet(Of String))
                list.AddRange(cableIds)
            Case Else
                ThrowSourceNotImplemented()
        End Select
    End Sub

    Public Sub AddComponentIds(componentIds As IEnumerable(Of String), clear As Boolean)
        Select Case Source
            Case CanvasSelectionSource.From2D
                If clear Then
                    _selectionByObjectType.Clear()
                End If

                Dim list As HashSet(Of String) = _selectionByObjectType.GetOrAdd(KblObjectType.Component_occurrence, Function() New HashSet(Of String))
                list.AddRange(componentIds)
            Case CanvasSelectionSource.From3D
                If clear Then
                    _selection3DByContainerType.Clear()
                End If

                Dim list As HashSet(Of String) = _selection3DByContainerType.GetOrAdd(ContainerId.Components, Function() New HashSet(Of String))
                list.AddRange(componentIds)
            Case Else
                ThrowSourceNotImplemented()
        End Select
    End Sub

    Public Sub AddModuleIds(moduleIds As IEnumerable(Of String), clear As Boolean)
        Select Case Source
            Case CanvasSelectionSource.From2D
                If clear Then
                    _selectionByObjectType.Clear()
                End If

                Dim list As HashSet(Of String) = _selectionByObjectType.GetOrAdd(KblObjectType.Module, Function() New HashSet(Of String))
                list.AddRange(moduleIds)
            Case CanvasSelectionSource.From3D
                If clear Then
                    _selection3DByContainerType.Clear()
                End If

                Dim list As HashSet(Of String) = _selection3DByContainerType.GetOrAdd(ContainerId.Modules, Function() New HashSet(Of String))
                list.AddRange(moduleIds)
            Case Else
                ThrowSourceNotImplemented()
        End Select
    End Sub

    Public Sub AddNetNames(netNames As String(), clear As Boolean)
        Select Case Source
            Case CanvasSelectionSource.From2D
                If clear Then
                    _selectionByObjectType.Clear()
                End If

                Dim list As HashSet(Of String) = _selectionByObjectType.GetOrAdd(KblObjectType.Connection, Function() New HashSet(Of String))
                list.AddRange(netNames)
            Case CanvasSelectionSource.From3D
                If clear Then
                    _selection3DByContainerType.Clear()
                End If

                Dim list As HashSet(Of String) = _selection3DByContainerType.GetOrAdd(ContainerId.Nets, Function() New HashSet(Of String))
                list.AddRange(netNames)
        End Select
    End Sub

    Public ReadOnly Property HasWires As Boolean
        Get
            Select Case Source
                Case CanvasSelectionSource.From2D
                    Return _selectionByObjectType.ContainsKey(KblObjectType.Wire_occurrence)
                Case CanvasSelectionSource.From3D
                    Return _selection3DByContainerType.ContainsKey(ContainerId.Wires)
                Case Else
                    ThrowSourceNotImplemented()
            End Select
            Return False
        End Get
    End Property

    Public ReadOnly Property HasCavitites As Boolean
        Get
            Select Case Source
                Case CanvasSelectionSource.From2D
                    Return _selectionByObjectType.ContainsKey(KblObjectType.Cavity_occurrence)
                Case CanvasSelectionSource.From3D
                    Return _selection3DByContainerType.ContainsKey(ContainerId.Cavities)

                Case Else
                    ThrowSourceNotImplemented()
            End Select
            Return False
        End Get
    End Property

    Public ReadOnly Property HasConnector As Boolean
        Get
            Select Case Source
                Case CanvasSelectionSource.From2D
                    Return _selectionByObjectType.ContainsKey(KblObjectType.Connector_occurrence)
                Case CanvasSelectionSource.From3D
                    Return _selection3DByContainerType.ContainsKey(ContainerId.Connectors)
                Case Else
                    ThrowSourceNotImplemented()
            End Select
            Return False
        End Get
    End Property

    Public ReadOnly Property HasSegments As Boolean
        Get
            Select Case Source
                Case CanvasSelectionSource.From2D
                    Return _selectionByObjectType.ContainsKey(KblObjectType.Segment)
                Case CanvasSelectionSource.From3D
                    Return _selection3DByContainerType.ContainsKey(ContainerId.Segments)
                Case Else
                    ThrowSourceNotImplemented()
            End Select
            Return False
        End Get
    End Property

    Public ReadOnly Property StampIds As String()
        Get
            Return _stampIds.ToArray
        End Get
    End Property

    Public ReadOnly Property IssueIds As String()
        Get
            Return _issueIds.ToArray
        End Get
    End Property

    Private Function RemoveTapings() As Boolean
        Select Case Source
            Case CanvasSelectionSource.From2D
                Return _selectionByObjectType.TryRemove(KblObjectType.Wire_protection_occurrence, Nothing)
            Case CanvasSelectionSource.From3D
                'TODO
            Case Else
                ThrowSourceNotImplemented()
        End Select
        Return False
    End Function

    Private Sub RefreshSelectionByObjectType(selection As IEnumerable(Of IBaseModelEntityEx))
        _issueIds.Clear()
        _stampIds.Clear()
        _selection3DByContainerType.Clear()

        For Each entity As IBaseModelEntityEx In selection
            For Each id As Guid In entity.GetEEObjectIds
                Dim obj As ObjectBaseNaming = TryCast(_document.Model(id), ObjectBaseNaming)
                If obj IsNot Nothing Then
                    Dim list As HashSet(Of String) = _selection3DByContainerType.GetOrAdd(obj.HostContainerId, Function() New HashSet(Of String))
                    Dim attr As KblPropertyBagAttribute = obj.CustomAttributes.OfType(Of KblPropertyBagAttribute).SingleOrDefault
                    If attr IsNot Nothing Then
                        list.Add(attr.PropertyBag.SystemId)
                    End If
                End If
            Next

            ' dimension does not exist here
            'If (entity.SVGType <> SvgType.dimension.ToString OrElse (entity.SecondaryKblIds.Count = 0 AndAlso _kblMapper.KBLOccurrenceMapper.ContainsKey(entity.KblId) AndAlso TypeOf _kblMapper.KBLOccurrenceMapper(entity.KblId) Is Dimension_specification)) AndAlso (entity.SymbolType <> SvgSymbolType.QMStamp.ToString) AndAlso (entity.SymbolType <> SvgSymbolType.Redlining.ToString) Then

            'Libraries.ImportExport.KBL.KblToModelResolver.GetModelContainerId()

            '_selectionByObjectType.GetOrAdd(entity.SymbolType, Function() New HashSet(Of String)).AddRange(kblIds)
            'ElseIf (entity.SymbolType = SvgSymbolType.QMStamp.ToString) Then
            '    _stampIds.AddRange(entity.GetIdsIfStamp)
            'ElseIf (entity.SymbolType = SvgSymbolType.Redlining.ToString) Then
            '    _redlingIds.Add(entity.SecondaryKblIds(0))
            'End If
        Next

        If HasSegments Then
            RemoveTapings()
        End If

    End Sub

    Private Sub RefreshSelectionByObjectType(selection As vdSelection)
        _issueIds.Clear()
        _stampIds.Clear()
        _selectionByObjectType.Clear()

        For Each group As VdSVGGroup In selection
            Dim dimSpec As Dimension_specification = If(group.SVGType = SvgType.dimension.ToString, _kblMapper.GetOccurrenceObject(Of Dimension_specification)(group.KblId), Nothing)
            If group.SVGType <> SvgType.dimension.ToString OrElse (group.SecondaryKblIds.Count = 0 AndAlso dimSpec IsNot Nothing AndAlso (group.SymbolType <> SvgSymbolType.QMStamp.ToString) AndAlso (group.SymbolType <> SvgSymbolType.Redlining.ToString)) Then
                Dim kblIds As New List(Of String) From {group.KblId}
                kblIds.AddRange(group.SecondaryKblIds)

                _issueIds.AddRange(group.GetIdsIfIssue)

                If (kblIds.Count = 0) Then
                    kblIds = GetKblIdsFromRootGroup_Recursivley(group.ChildGroups)
                End If

                If (kblIds.Count = 0) Then
                    Continue For
                End If

                Dim symbolType As SvgSymbolType = [Enum](Of SvgSymbolType).Parse(group.SymbolType)
                Dim svgSymbolAsKblObjectType As KblObjectType = symbolType.AsKblObjectType
                _selectionByObjectType.GetOrAdd(svgSymbolAsKblObjectType, Function() New HashSet(Of String)).AddRange(kblIds)
            ElseIf (group.SymbolType = SvgSymbolType.QMStamp.ToString) Then
                _stampIds.AddRange(group.GetIdsIfStamp)
            ElseIf (group.SymbolType = SvgSymbolType.Redlining.ToString) Then
                _redlingIds.Add(group.SecondaryKblIds(0))
            End If
        Next

        If HasSegments Then
            RemoveTapings()
        End If

    End Sub

    Private Function GetKblIdsFromRootGroup_Recursivley(childGroups As vdEntities) As List(Of String)
        For Each group As VdSVGGroup In childGroups
            If (group.KblId <> String.Empty) Then
                Dim kblIds As New List(Of String)
                kblIds.Add(group.KblId)
                kblIds.AddRange(group.SecondaryKblIds)

                Return kblIds
            Else
                GetKblIdsFromRootGroup_Recursivley(group.ChildGroups)
            End If
        Next

        Return New List(Of String)
    End Function

    Public Function GetCavityPartIds() As List(Of String)
        Dim cavityPartKblIds As New List(Of String)

        Select Case Source
            Case CanvasSelectionSource.From2D
                Dim wireKblIds As HashSet(Of String) = Nothing

                If _selectionByObjectType.TryGetValue(KblObjectType.Wire_occurrence, wireKblIds) Then
                    If wireKblIds.Count > 0 Then
                        For Each wireKblId As String In wireKblIds
                            Dim svgGroups As List(Of VdSVGGroup) = Nothing
                            If _selectionByKblId.TryGetValue(wireKblId, svgGroups) Then
                                For Each group As VdSVGGroup In svgGroups
                                    If group.ChildGroups.Count <> 0 Then
                                        If (Not cavityPartKblIds.Contains(DirectCast(group.ChildGroups(0), VdSVGGroup).KblId)) Then
                                            cavityPartKblIds.Add(DirectCast(group.ChildGroups(0), VdSVGGroup).KblId)
                                        End If
                                    End If
                                Next
                            End If
                        Next
                    End If
                End If
            Case CanvasSelectionSource.From3D
                'TODO: not implemented
            Case Else
                ThrowSourceNotImplemented()
        End Select

        Return cavityPartKblIds
    End Function

    Public Function GetModuleIds() As IEnumerable(Of String)
        Dim moduleKblIds As New HashSet(Of String)

        Select Case Source
            Case CanvasSelectionSource.From2D
                moduleKblIds.AddRange(GetModuleFrom(_selectionByObjectType.Values.SelectMany(Function(v) v)))
            Case CanvasSelectionSource.From3D
                moduleKblIds.AddRange(GetModuleFrom(_selection3DByContainerType.Values.SelectMany(Function(v) v)))
            Case Else
                ThrowSourceNotImplemented()
        End Select

        Return moduleKblIds
    End Function

    Private Function GetModuleFrom(kblIds As IEnumerable(Of String)) As String()
        Dim moduleKblIds As New HashSet(Of String)

        Select Case Source
            Case CanvasSelectionSource.From2D
                For Each kblId As String In kblIds
                    If (_kblMapper.KBLObjectModuleMapper.ContainsKey(kblId)) Then
                        For Each moduleKblId As String In _kblMapper.KBLObjectModuleMapper(kblId)
                            moduleKblIds.Add(moduleKblId)
                        Next
                    End If
                Next
            Case CanvasSelectionSource.From3D
                For Each obj As ObjectBase In _document.Entities.GetByKblIds(kblIds.ToArray).SelectMany(Function(kbl) kbl.GetEEModelObjects).OfType(Of ObjectBase)
                    If obj.Mappers.Contains(ContainerId.Modules) Then
                        For Each entry As IMapperEntry In obj.Mappers(ContainerId.Modules).MapperEntries
                            Dim m As Zuken.E3.Lib.Model.Module = CType(entry.MappedBaseObject, Zuken.E3.Lib.Model.Module)
                            Dim sysId As String = GetKblId(m)
                            If Not String.IsNullOrEmpty(sysId) Then
                                moduleKblIds.Add(sysId)
                            End If
                        Next
                    End If
                Next
            Case Else
                ThrowSourceNotImplemented()
        End Select

        Return moduleKblIds.ToArray
    End Function

    Public Function GetComponentIds() As IEnumerable(Of String)
        Dim componentKblIds As New HashSet(Of String)
        If HasConnector Then
            Select Case Source
                Case CanvasSelectionSource.From2D
                    For Each kblId As String In GetConnectorIds()
                        Dim components As IEnumerable(Of Component_occurrence) = _kblMapper.GetComponentOccurrences.Where(Function(comp) comp.Mounting IsNot Nothing AndAlso comp.Mounting IsNot Nothing AndAlso comp.Mounting.Contains(kblId))

                        For Each component As Component_occurrence In components
                            componentKblIds.Add(component.SystemId)
                        Next
                    Next
                Case CanvasSelectionSource.From3D
                    For Each obj As Zuken.E3.Lib.Model.Connector In _document.Entities.GetByKblIds(GetConnectorIds().ToArray).SelectMany(Function(ent) ent.GetEEModelObjects).OfType(Of Zuken.E3.Lib.Model.Connector)
                        For Each comp As Zuken.E3.Lib.Model.Component In obj.GetComponents().Entries
                            Dim sysId As String = GetKblId(comp)
                            If Not String.IsNullOrEmpty(sysId) Then
                                componentKblIds.Add(sysId)
                            End If
                        Next
                    Next
                Case Else
                    ThrowSourceNotImplemented()
            End Select

        End If

        Return componentKblIds
    End Function

    Private Function GetKblId(obj As ObjectBaseNaming) As String
        Return obj.CustomAttributes.OfType(Of KblPropertyBagAttribute).SingleOrDefault?.PropertyBag.SystemId
    End Function

    Public Function GetRedliningsIds() As IEnumerable(Of String)
        Select Case Source
            Case CanvasSelectionSource.From2D
                Return _redlingIds
            Case CanvasSelectionSource.From3D
                'TODO
                Return _redlingIds
            Case Else
                ThrowSourceNotImplemented()
        End Select
        Return Array.Empty(Of String)()
    End Function

    Public Function GetSegmentIds() As IEnumerable(Of String)
        Select Case Source
            Case CanvasSelectionSource.From2D
                Return _selectionByObjectType(KblObjectType.Segment)
            Case CanvasSelectionSource.From3D
                Return _selection3DByContainerType(ContainerId.Segments)
            Case Else
                ThrowSourceNotImplemented()
        End Select
        Return Array.Empty(Of String)()
    End Function

    Public Function GetWiresIds() As IEnumerable(Of String)
        Select Case Source
            Case CanvasSelectionSource.From2D
                Return _selectionByObjectType(KblObjectType.Wire_occurrence)
            Case CanvasSelectionSource.From3D
                Return _selection3DByContainerType(ContainerId.Wires)
            Case Else
                ThrowSourceNotImplemented()
        End Select
        Return Array.Empty(Of String)()
    End Function

    Public Function GetWiresIdsFromSegments(removeSegments As Boolean) As IEnumerable(Of String)
        Dim wireKblIds As New HashSet(Of String)

        If HasSegments Then
            Select Case Source
                Case CanvasSelectionSource.From2D
                    For Each segmentKblId As String In GetSegmentIds()
                        If (_kblMapper.KBLSegmentWireMapper.ContainsKey(segmentKblId)) Then
                            For Each wireKblId As String In _kblMapper.KBLSegmentWireMapper(segmentKblId)
                                wireKblIds.Add(wireKblId)
                            Next
                        End If
                    Next
                Case CanvasSelectionSource.From3D
                    For Each seg As Zuken.E3.Lib.Model.Segment In _document.Entities.GetByKblIds(GetSegmentIds().ToArray).SelectMany(Function(ent) ent.GetEEModelObjects).OfType(Of Zuken.E3.Lib.Model.Segment)
                        For Each wire As Zuken.E3.Lib.Model.Wire In seg.GetWires.Entries
                            Dim sysId As String = GetKblId(wire)
                            If Not String.IsNullOrEmpty(sysId) Then
                                wireKblIds.Add(sysId)
                            End If
                        Next
                    Next
                Case Else
                    ThrowSourceNotImplemented()
            End Select

            If removeSegments Then
                RemoveSegmentsInternal()
            End If
        End If

        Return wireKblIds
    End Function

    Public Function GetConnectorIds() As IEnumerable(Of String)
        Select Case Source
            Case CanvasSelectionSource.From2D
                Return _selectionByObjectType(KblObjectType.Connector_occurrence)
            Case CanvasSelectionSource.From3D
                Return _selection3DByContainerType(ContainerId.Connectors)
            Case Else
                ThrowSourceNotImplemented()
        End Select
        Return Array.Empty(Of String)()
    End Function

    Private Function GetCableFromWire(wireKblId As String) As String
        Select Case Source
            Case CanvasSelectionSource.From2D
                If (_kblMapper.KBLCoreCableMapper.ContainsKey(wireKblId)) Then
                    Return _kblMapper.KBLCoreCableMapper(wireKblId)
                End If
            Case CanvasSelectionSource.From3D
                Dim wire As Zuken.E3.Lib.Model.Wire = _document.Entities.GetByKblIds(wireKblId).SelectMany(Function(ent) ent.GetEEModelObjects).OfType(Of Zuken.E3.Lib.Model.Wire).SingleOrDefault
                If wire IsNot Nothing Then
                    Dim cable As Zuken.E3.Lib.Model.Cable = wire.GetCable()
                    If cable IsNot Nothing Then
                        Return GetKblId(cable)
                    End If
                End If
            Case Else
                ThrowSourceNotImplemented()
        End Select

        Return Nothing
    End Function

    Public Function GetCableIds() As IEnumerable(Of String)
        Dim cableKblIds As New HashSet(Of String)

        If HasSegments Then
            For Each segmentKblId As String In GetSegmentIds()
                For Each wireId As String In GetWiresFromSegment(segmentKblId)
                    Dim cableId As String = GetCableFromWire(wireId)
                    If Not String.IsNullOrEmpty(cableId) Then
                        cableKblIds.Add(cableId)
                    End If
                Next
            Next
        End If

        If HasWires Then
            For Each wireKblId As String In GetWiresIds()
                Dim cableId As String = GetCableFromWire(wireKblId)
                If Not String.IsNullOrEmpty(cableId) Then
                    cableKblIds.Add(cableId)
                End If
            Next
        End If

        Return cableKblIds
    End Function

    Private Function RemoveSegmentsInternal() As Boolean
        Select Case Source
            Case CanvasSelectionSource.From2D
                Return _selectionByObjectType.TryRemove(KblObjectType.Segment, Nothing)
            Case CanvasSelectionSource.From3D
                Return _selection3DByContainerType.TryRemove(ContainerId.Segments, Nothing)
            Case Else
                ThrowSourceNotImplemented()
        End Select
        Return False
    End Function

    Private Function RemoveWiresInternal() As Boolean
        Select Case Source
            Case CanvasSelectionSource.From2D
                Return _selectionByObjectType.TryRemove(KblObjectType.Wire_occurrence, Nothing)
            Case CanvasSelectionSource.From3D
                Return _selection3DByContainerType.TryRemove(ContainerId.Wires, Nothing)
            Case Else
                ThrowSourceNotImplemented()
        End Select
        Return False
    End Function

    Private Function GetWiresFromSegment(segmentKblId As String) As IEnumerable(Of String)
        Dim list As New HashSet(Of String)
        Select Case Source
            Case CanvasSelectionSource.From2D
                If (_kblMapper.KBLSegmentWireMapper.ContainsKey(segmentKblId)) Then
                    For Each wireKblId As String In _kblMapper.KBLSegmentWireMapper(segmentKblId)
                        list.Add(wireKblId)
                    Next
                End If
            Case CanvasSelectionSource.From3D
                For Each seg As Zuken.E3.Lib.Model.Segment In _document.Entities.GetByKblIds(segmentKblId).SelectMany(Function(ent) ent.GetEEModelObjects).OfType(Of Zuken.E3.Lib.Model.Segment)
                    For Each wire As Zuken.E3.Lib.Model.Wire In seg.GetWires.Entries
                        Dim sysId As String = GetKblId(wire)
                        If Not String.IsNullOrEmpty(sysId) Then
                            list.Add(sysId)
                        End If
                    Next
                Next
            Case Else
                ThrowSourceNotImplemented()
        End Select

        Return list
    End Function

    Private Function GetNetFromWireId(wireKblId As String) As String
        Select Case Source
            Case CanvasSelectionSource.From2D
                Dim connection As Connection = If(_kblMapper.KBLWireNetMapper.ContainsKey(wireKblId), TryCast(_kblMapper.KBLWireNetMapper(wireKblId), Connection), Nothing)
                If (connection IsNot Nothing) AndAlso (connection.Signal_name IsNot Nothing) AndAlso (connection.Signal_name <> String.Empty) Then
                    Return connection.Signal_name
                End If
            Case CanvasSelectionSource.From3D
                For Each wire As Zuken.E3.Lib.Model.Wire In _document.Entities.GetByKblIds(wireKblId).SelectMany(Function(ent) ent.GetEEModelObjects).OfType(Of Zuken.E3.Lib.Model.Wire)
                    Dim net As Zuken.E3.Lib.Model.Net = wire.GetNet
                    If net IsNot Nothing Then
                        Return net.GetSignal?.Entry?.ShortName
                    End If
                Next
            Case Else
                ThrowSourceNotImplemented()
        End Select

        Return Nothing
    End Function

    Public Function GetNetNames(removeSegments As Boolean, removeWires As Boolean) As String()
        Dim netNames As New HashSet(Of String)

        If HasSegments Then
            For Each segmentKblId As String In GetSegmentIds()
                For Each wireKblId As String In GetWiresFromSegment(segmentKblId)
                    Dim signalName As String = GetNetFromWireId(wireKblId)
                    If Not String.IsNullOrEmpty(signalName) Then
                        netNames.Add(signalName)
                    End If
                Next
            Next

            If removeSegments Then
                RemoveSegmentsInternal()
            End If
        End If

        If HasWires Then
            For Each wireKblId As String In GetWiresIds()
                Dim signalName As String = GetNetFromWireId(wireKblId)
                If Not String.IsNullOrEmpty(signalName) Then
                    netNames.Add(signalName)
                End If
            Next

            If removeWires Then
                RemoveWiresInternal()
            End If
        End If

        Return netNames.ToArray
    End Function

    Public Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of KblObjectType, HashSet(Of String))) Implements IEnumerable(Of KeyValuePair(Of KblObjectType, HashSet(Of String))).GetEnumerator
        Select Case Source
            Case CanvasSelectionSource.From2D
                Return _selectionByObjectType.GetEnumerator
            Case CanvasSelectionSource.From3D
                Return _selection3DByContainerType.Select(Function(kv) New KeyValuePair(Of KblObjectType, HashSet(Of String))(GetKblOBjectType(kv.Key), kv.Value)).GetEnumerator
            Case Else
                ThrowSourceNotImplemented()
        End Select

        Return (Array.Empty(Of KeyValuePair(Of KblObjectType, HashSet(Of String)))()).AsEnumerable.GetEnumerator
    End Function

    Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return GetEnumerator()
    End Function

    Friend ReadOnly Property AllKblIds As IEnumerable(Of String)
        Get
            Select Case Source
                Case CanvasSelectionSource.From2D
                    Return _selectionByObjectType.Values.SelectMany(Function(list) list)
                Case CanvasSelectionSource.From3D
                    Return _selection3DByContainerType.Values.SelectMany(Function(list) list)
                Case Else
                    ThrowSourceNotImplemented()
            End Select
            Return Array.Empty(Of String)()
        End Get
    End Property

    Private Function ParseContainerIdToSvgSymbolTypeStr(containerIDStr As String) As KblObjectType
        Dim cId As ContainerId
        If [Enum].TryParse(Of ContainerId)(containerIDStr, cId) Then
            Return GetKblOBjectType(cId)
        End If

        Throw New ArgumentException($"Parameter ""{containerIDStr}"" not a ""{NameOf(Zuken.E3.Lib.Model.ContainerId)}""", NameOf(containerIDStr))
    End Function

    Private Function GetKblOBjectType(containerID As ContainerId) As KblObjectType
        Select Case containerID
            Case ContainerId.Wires
                Return KblObjectType.Wire_occurrence
            Case ContainerId.Vertices
                Return KblObjectType.Node
            Case ContainerId.Segments
                Return KblObjectType.Segment
            Case ContainerId.Cavities
                Return KblObjectType.Cavity_occurrence
            Case ContainerId.AdditionalParts
                Return KblObjectType.Fixing_occurrence
            Case ContainerId.Protections
                Return KblObjectType.Wire_protection_occurrence
            Case ContainerId.Connectors
                Return KblObjectType.Connector_occurrence

                'TODO: connectorType needed, because SvgSymbolType does not have Eyelet,Splice, etc.
                'Dim conn As Connector
                'Select Case conn.ConnectorType
                '    Case ConnectorType.Connector
                '        Return SvgSymbolType.Connector
                '    Case ConnectorType.Eyelet
                '        Return SvgSymbolType.
                '    Case ConnectorType.OpenEnd
                '    Case ConnectorType.Splice
                'End Select
            Case ContainerId.Supplements
                Return KblObjectType.Accessory_occurrence
            Case ContainerId.Components
                Return KblObjectType.Component_occurrence
            Case ContainerId.Cables
                Return KblObjectType.Special_wire_occurrence
            Case ContainerId.Nets
                Return KblObjectType.Net
            Case ContainerId.Modules
                Return KblObjectType.Module
        End Select

        Throw New NotImplementedException($"Resolve of {NameOf(Zuken.E3.Lib.Model.ContainerId)}.{containerID.ToString} -> {NameOf(SvgSymbolType)} not implemented!")
    End Function

    Public Enum CanvasSelectionSource
        Unknown = 0
        From2D
        From3D
    End Enum

End Class
