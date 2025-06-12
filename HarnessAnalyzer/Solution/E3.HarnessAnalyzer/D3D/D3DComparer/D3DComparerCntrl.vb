Imports devDept.Eyeshot
Imports devDept.Eyeshot.Entities
Imports devDept.Geometry
Imports Infragistics.Win.UltraWinGrid
Imports Zuken.E3.HarnessAnalyzer.D3D.Consolidated.Controls
Imports Zuken.E3.HarnessAnalyzer.D3D.Document.Controls
Imports Zuken.E3.HarnessAnalyzer.D3D.Shared
Imports Zuken.E3.HarnessAnalyzer.Project.Documents
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.Lib.Eyeshot.Model
Imports Zuken.E3.Lib.IO.KBL
Imports Zuken.E3.Lib.Model

Namespace D3D

    <Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
    Partial Public Class D3DComparerCntrl

        Private _actionModeChanging As actionType
        Private _actionMode As actionType
        Private _isSelectingFromProperties As Boolean = False

        Private WithEvents _documents As New ClonedDocuments(Me)

        Public Event NeedsEEObjectData(sender As Object, e As EEObjectDataEventArgs)
        Public Event EmptySpaceClicked(sender As Object, e As EventArgs)
        Public Event ActionModelChanged(sender As Object, e As ActionModeChangedEventArgs)
        Public Event ModelLostFocus(sender As Object, e As EventArgs)
        Public Event ClearSelection(sender As Object, e As EventArgs)
        Public Event SelectionChangedInD3DComparer(sender As Object, e As ComparerSelectionChangedEventArgs)
        Public Event IsLoaded(sender As Object, e As EventArgs)
        Public Event ToolBarButtonClick(sender As Object, e As ToolBarButtonEventArgs)

        Private _referenceDocument As DocumentClone
        Private _compareDocument As DocumentClone
        Private _refChanges As New List(Of IBaseModelEntityEx)
        Private _compChanges As New List(Of IBaseModelEntityEx)
        Private _refEntities As New Dictionary(Of String, Entity)
        Private _compEntities As New Dictionary(Of String, Entity)
        Private _myRefEntities As New Dictionary(Of Guid, IBaseModelEntityEx)
        Private _myCompEntities As New Dictionary(Of Guid, IBaseModelEntityEx)
        Private _compSelection As New List(Of IBaseModelEntityEx)
        Private _refSelection As New List(Of IBaseModelEntityEx)
        Private _changedObjects As Dictionary(Of KblObjectType, List(Of ChangedObject))
        Private _compMapper As KblMapper
        Private _refMapper As KblMapper
        Private _sourceObjects As New Dictionary(Of String, List(Of String))
        Private _toolBarButtons As New Dictionary(Of String, ToolBarButton)
        Private WithEvents _ttManager As EntityToolTipsManager

        Private WithEvents _compareHub As InformationHub

        Friend WithEvents ToolTipCaptionContextMenuStrip As ContextMenuStrip

        Private _refEEModel As E3.Lib.Model.EESystemModel
        Private _compEEModel As E3.Lib.Model.EESystemModel
        Private _isHiddenSelection As Boolean
        Private _isInSelection As Boolean
        Private _kblId_EntityMapper As New Dictionary(Of String, ChangedObjectEx)
        Private _wireId_EntityMapper As New Dictionary(Of String, List(Of ChangedObjectEx))
        Private _idEntityMapperRef As New Dictionary(Of String, IBaseModelEntityEx)
        Private _idEntityMapperComp As New Dictionary(Of String, IBaseModelEntityEx)
        Private _routedRefElements As New List(Of IBaseModelEntityEx)
        Private _routedCompElements As New List(Of IBaseModelEntityEx)

        Friend WithEvents Fader As New FaderCntrl
        Friend WithEvents Trackbar As TrackBar = Fader.TrackBar1

        Friend selection As New List(Of IBaseModelEntityEx)
        Private _oldTrackbarValue As Integer = 1
        Private _isSelecting As Boolean
        Public EventsEnabled As Boolean = True

        Private _refDoc As HcvDocument
        Private _compDoc As HcvDocument
        Private _mouseEventArgs As MouseEventArgs
        Private _selectModus As SelectionModus
        Friend _grids As New List(Of UltraGrid)

        Private _refListIdMapper As New Dictionary(Of String, IBaseModelEntityEx)
        Private _compListIdMapper As New Dictionary(Of String, IBaseModelEntityEx)
        Private _defaultMargin As Integer = 1000
        Private _colorMapper As New Dictionary(Of String, Color)
        Private _mergedModels As New E3.Lib.Model.EEModelsCollection
        Private _tmpEntities As New List(Of IBaseModelEntityEx)

        Private _isHubSelection As Boolean = False
        Private _inverse As Boolean

        Public Sub New(generalSettings As GeneralSettingsBase, compareHub As InformationHub, activeDocument As IDocumentForm, compareDocument As IDocumentForm, changedObjects As Dictionary(Of KblObjectType, List(Of ChangedObject)))
            InitializeComponent()
            Design3D.InitDefaults(initObjManipulatorManager:=True)
            InitializeD3DComparer(generalSettings, compareHub, activeDocument, compareDocument, changedObjects)
            Design3D.ViewportBorder.Visible = False
            Design3D.MultipleSelection = False
            _selectModus = SelectionModus.reference

            _grids.Add(_compareHub.ugAccessories)
            _grids.Add(_compareHub.ugApprovals)
            _grids.Add(_compareHub.ugAssemblyParts)
            _grids.Add(_compareHub.ugCables)
            _grids.Add(_compareHub.ugChangeDescriptions)
            _grids.Add(_compareHub.ugComponentBoxes)
            _grids.Add(_compareHub.ugComponents)
            _grids.Add(_compareHub.ugConnectors)
            _grids.Add(_compareHub.ugCoPacks)
            _grids.Add(_compareHub.ugDefDimSpecs)
            _grids.Add(_compareHub.ugDifferences)
            _grids.Add(_compareHub.ugDimSpecs)
            _grids.Add(_compareHub.ugWires)
            _grids.Add(_compareHub.ugFixings)
            _grids.Add(_compareHub.ugNets)
            _grids.Add(_compareHub.ugSegments)
            _grids.Add(_compareHub.ugVertices)
            _grids.Add(_compareHub.ugHarness)
            _grids.Add(_compareHub.ugModules)
            _grids.Add(_compareHub.ugQMStamps)
            _grids.Add(_compareHub.ugRedlinings)
            SetSingleSelect()
            AddToolBarButtonEvents()
        End Sub

        Private Sub AddToolBarButtonEvents()
            RemoveToolBarButtonEvents()

            For Each button As ToolBarButton In Me.Design3D.ActiveViewport.ToolBar.Buttons
                _toolBarButtons.Add(button.Name, button)
                button.ToolTipText = GetToolBarButtonTTText(button.Name)
                AddHandler button.Click, AddressOf _toolBarButton_Click
            Next
        End Sub

        Private Function GetToolBarButtonTTText(buttonName As String) As String
            Static resName As String = Nothing
            Static man As System.Resources.ResourceManager

            If resName Is Nothing Then
                resName = Me.GetType.Assembly.GetManifestResourceNames.Where(Function(rName) Not String.IsNullOrEmpty(rName) AndAlso rName.EndsWith(String.Format(".{0}.resources", NameOf(Consolidated3DControlStrings)))).Single
                man = New System.Resources.ResourceManager(resName.Replace(".resources", ""), Me.GetType.Assembly)
            End If

            Return man.GetString(String.Format("TT_ToolBarButton_{0}", buttonName))
        End Function

        Private Sub RemoveToolBarButtonEvents()
            If _toolBarButtons IsNot Nothing Then
                For Each button As devDept.Eyeshot.ToolBarButton In _toolBarButtons.Values
                    RemoveHandler button.Click, AddressOf _toolBarButton_Click
                Next
                _toolBarButtons.Clear()
            End If
        End Sub

        Private Sub _toolBarButton_Click(sender As Object, e As EventArgs)
            RaiseEvent ToolBarButtonClick(Me, New ToolBarButtonEventArgs(DirectCast(sender, devDept.Eyeshot.ToolBarButton)))
        End Sub

        Public Function ToolBarButtonExists(buttonType As ToolBarButtons) As Boolean
            If _toolBarButtons IsNot Nothing Then
                Return _toolBarButtons.ContainsKey(buttonType.ToString)
            End If
            Return False
        End Function

        Public Function GetToolBarButton(buttonEnum As ToolBarButtons) As ToolBarButton
            Dim button As ToolBarButton = Nothing
            _toolBarButtons.TryGetValue(buttonEnum.ToString, button)
            Return button
        End Function

        Private Sub SetTransparency(en As IBaseModelEntityEx, transparency As Double)
            Dim origColor As Color = _colorMapper.Item(en.Id)
            en.Color = Color.FromArgb(CInt(origColor.A * transparency), origColor)
        End Sub

        Private Sub SetTransparency(en As IBaseModelEntityEx, transparency As Double, color As Color)
            Dim origColor As Color = _colorMapper.Item(en.Id)
            en.Color = Color.FromArgb(CInt(origColor.A * transparency), color)
        End Sub

        Private Sub DeselectAllRowsInGrids()
            For Each Grid As UltraGrid In _grids
                Grid.EventManager.AllEventsEnabled = False
                Grid.Selected.Rows.Clear()
                Grid.EventManager.AllEventsEnabled = True
            Next
        End Sub

        Private Sub SetSingleSelect()
            For Each Grid As UltraGrid In _grids
                Grid.DisplayLayout.Override.SelectTypeRow = SelectType.Single
            Next
        End Sub

        Private Sub SetMultiSelect()
            For Each Grid As UltraGrid In _grids
                Grid.DisplayLayout.Override.SelectTypeRow = SelectType.Extended
            Next
        End Sub

        Public Property IsHiddenSelection As Boolean
            Get
                Return _isHiddenSelection
            End Get
            Set(value As Boolean)
                _isHiddenSelection = value
            End Set
        End Property

        Public Sub InitializeD3DComparer(generalSettings As GeneralSettingsBase, compareHub As InformationHub, activeDocument As IDocumentForm, compareDocument As IDocumentForm, changedObjects As Dictionary(Of KblObjectType, List(Of ChangedObject)))
            _compareHub = compareHub
            Design3D.EEModel = New EEModelsCollection
            Me.BorderStyle = BorderStyle.None
            Me._changedObjects = changedObjects

            _refDoc = CType(activeDocument.HcvDocument, HcvDocument)
            _compDoc = CType(compareDocument.HcvDocument, HcvDocument)

            _referenceDocument = New DocumentClone(CType(activeDocument.HcvDocument, HcvDocument))
            _compareDocument = New DocumentClone(CType(compareDocument.HcvDocument, HcvDocument))

            _documents.Add(_referenceDocument)
            _documents.Add(Me._compareDocument)

            _refMapper = CType(activeDocument.Kbl, KblMapper)
            _compMapper = CType(compareDocument.Kbl, KblMapper)

            _refEEModel = _referenceDocument.EEModel
            _compEEModel = Me._compareDocument.EEModel

            _mergedModels.Add(_refEEModel)
            _mergedModels.Add(_compEEModel)

            AttachEEModel(_refEEModel)
            AttachEEModel(_compEEModel)

            InitData()
            CheckForTopologyChanges()

            InitContextMenu(generalSettings)

            Me.CreateControl()

            Design3D.ActiveViewport.ZoomFit(False, Camera.perspectiveFitType.Accurate, 0)

            _ttManager = New EntityToolTipsManager(Me)

            Design3D.WheelZoomEnabled = True
            InitFader()
            Design3D.Invalidate()

            Design3D.ActiveViewport.Camera.UpdateLocation()
            Design3D.ActiveViewport.Camera.UpdateMatrices()
        End Sub

        Private Sub InitData()
            Dim changes As Dictionary(Of KblObjectType, List(Of ChangedObject)) = _changedObjects
            Dim alpha As Integer = 20
            _colorMapper.Clear()
            _refChanges.Clear()
            _compChanges.Clear()
            _myRefEntities.Clear()
            _myCompEntities.Clear()

            For Each refEntity As IBaseModelEntityEx In _referenceDocument.Entities
                _colorMapper.Add(refEntity.Id, refEntity.Color)
                refEntity.Selectable = False
                Design3D.Entities.Add(refEntity)

                Dim myItem As ObjectBase
                If refEntity.EntityType = ModelEntityType.Fixing Then
                    Dim fxGroup As FixingGroup = CType(refEntity, FixingGroup)

                    For Each id As Guid In fxGroup.GetEEObjectIds().ToList
                        myItem = _refEEModel.Item(id)
                        If myItem IsNot Nothing Then
                            Dim myNamingItem As ObjectBaseNaming = CType(myItem, ObjectBaseNaming)

                            If ItemIsChanged(myNamingItem, refEntity, ModelType.reference) Then
                                _refChanges.Add(refEntity)
                                _myRefEntities.Add(myNamingItem.Id, refEntity)
                            End If

                            If TypeOf refEntity IsNot VirtualGroupEntity Then
                                Dim kblid As String = GetKblId(myNamingItem)
                                _idEntityMapperRef.Add(kblid, refEntity)
                            End If
                        End If
                    Next
                Else
                    Dim id As Guid
                    If Guid.TryParse(refEntity.Id, id) Then
                        myItem = _refEEModel.Item(id)
                        If myItem IsNot Nothing Then
                            Dim myNamingItem As ObjectBaseNaming = CType(myItem, ObjectBaseNaming)

                            If ItemIsChanged(myNamingItem, refEntity, ModelType.reference) Then
                                _refChanges.Add(refEntity)
                                _myRefEntities.Add(myNamingItem.Id, refEntity)
                            End If

                            If TypeOf refEntity IsNot VirtualGroupEntity Then
                                Dim kblid As String = GetKblId(myNamingItem)
                                _idEntityMapperRef.Add(kblid, refEntity)
                            Else
                                Dim kblid As String = GetKblId(myNamingItem)
                                _idEntityMapperRef.Add(kblid, refEntity)
                            End If
                        End If
                    End If
                End If
            Next

            For Each compEntity As IBaseModelEntityEx In Me._compareDocument.Entities
                Dim myItem As ObjectBase
                _colorMapper.Add(compEntity.Id, compEntity.Color)
                If compEntity.EntityType = ModelEntityType.Fixing Then
                    Dim fxGroup As FixingGroup = CType(compEntity, FixingGroup)

                    For Each id As Guid In fxGroup.GetEEObjectIds().ToList
                        myItem = _compEEModel.Item(id)
                        If myItem IsNot Nothing Then
                            Dim myNamingItem As ObjectBaseNaming = CType(myItem, ObjectBaseNaming)
                            If ItemIsChanged(myNamingItem, compEntity, ModelType.compare) Then
                                _compChanges.Add(compEntity)
                                _myCompEntities.Add(myNamingItem.Id, compEntity)
                                Dim kblid As String = GetKblId(myNamingItem)
                                _idEntityMapperComp.Add(kblid, compEntity)
                            End If
                        End If

                    Next
                Else
                    Dim id As Guid
                    If Guid.TryParse(compEntity.Id, id) Then
                        myItem = _compEEModel.Item(id)
                        If myItem IsNot Nothing Then
                            Dim myNamingItem As ObjectBaseNaming = CType(myItem, ObjectBaseNaming)

                            If ItemIsChanged(myNamingItem, compEntity, ModelType.compare) Then
                                If _myCompEntities.TryAdd(myNamingItem.Id, compEntity) Then
                                    _compChanges.Add(compEntity)
                                End If
                            End If

                            Dim kbl_id As String = GetKblId(myNamingItem)
                            _idEntityMapperComp.Add(kbl_id, compEntity)
                        End If

                    End If
                End If
            Next

            Dim changed As New List(Of IBaseModelEntityEx)
            _refChanges.ForEach(Sub(item)
                                    _refListIdMapper.Add(item.Id, item)
                                    changed.Add(item)
                                End Sub)
            _compChanges.ForEach(Sub(item)
                                     _compListIdMapper.Add(item.Id, item)
                                     changed.Add(item)
                                 End Sub)

            _wireId_EntityMapper = (From item In _wireId_EntityMapper Order By item.Key Ascending).ToDictionary(Function(entry) entry.Key, Function(entry) entry.Value)

            For Each item As KeyValuePair(Of String, List(Of ChangedObjectEx)) In _wireId_EntityMapper
                For Each obj As ChangedObjectEx In item.Value
                    If obj.ObjType = KblObjectType.Segment AndAlso Not changed.Contains(obj.Entity) Then
                        obj.Entity.IsNotActive(_colorMapper)
                        obj.Entity.Selectable = False
                        _colorMapper(obj.ID) = obj.Entity.Color
                    End If
                Next
            Next
        End Sub

        Private Function ItemIsChanged(item As ObjectBaseNaming, en As IBaseModelEntityEx, modelType As ModelType) As Boolean
            Dim returnValue As Boolean = False
            If item IsNot Nothing Then
                Dim myMapper As KblMapper

                If modelType = ModelType.compare Then
                    myMapper = _compMapper
                Else
                    myMapper = _refMapper
                End If

                Dim kblid As String = GetKblId(item)

                Dim my_occ As IKblOccurrence = myMapper.GetOccurrenceObjectUntyped(kblid)
                Dim myChangedObjects As List(Of ChangedObject) = GetChangedObject(item, my_occ.ObjectType, kblid, modelType)

                If myChangedObjects IsNot Nothing AndAlso myChangedObjects.Count > 0 Then
                    en.Selectable = True
                    For Each obj As ChangedObject In myChangedObjects
                        Dim Key As String = ""
                        If obj.SourceType = KblObjectType.Wire_occurrence OrElse obj.SourceType = KblObjectType.Special_wire_occurrence OrElse obj.SourceType = KblObjectType.Connection Then

                            If obj.ObjType = KblObjectType.Connector_occurrence Then
                                returnValue = True
                            End If

                            Key = myMapper.Id.ToString + "|" + obj.SrcId
                            Dim myChangedObjectEx As New ChangedObjectEx(obj, en, myMapper.Id)

                            If Not _wireId_EntityMapper.ContainsKey(Key) Then
                                _wireId_EntityMapper.Add(Key, New List(Of ChangedObjectEx) From {myChangedObjectEx})
                            Else
                                _wireId_EntityMapper.Item(Key).Add(myChangedObjectEx)
                            End If
                            AddToKblIdEntityMapper(Key, obj, en)
                        Else
                            If obj.SourceType <> KblObjectType.Undefined Then
                                Key = myMapper.Id.ToString + "|" + obj.SrcId
                            Else
                                Key = myMapper.Id.ToString + "|" + kblid
                            End If

                            AddToKblIdEntityMapper(Key, obj, en)

                            If Not _colorMapper.ContainsKey(en.Id) Then
                                _colorMapper.Add(en.Id, en.Color)
                            End If
                            returnValue = True
                        End If
                    Next

                    Return returnValue
                Else
                    en.IsNotActive(_colorMapper)
                End If
            End If
            Return returnValue
        End Function

        Private Function GetEntity(id As String, mapperIdOfId As String, entityType As ModelEntityType) As IBaseModelEntityEx
            Dim entity As IBaseModelEntityEx = Nothing
            Dim myDocument As DocumentClone
            Dim myModel As E3.Lib.Model.EESystemModel

            If mapperIdOfId = _refMapper.Id Then
                myDocument = _referenceDocument
                myModel = _refEEModel
            Else
                myDocument = _compareDocument
                myModel = _compEEModel
            End If

            For Each item As IBaseModelEntityEx In myDocument.Entities.Where(Function(e) e.EntityType = entityType)
                Dim entityId As Guid
                If Guid.TryParse(item.Id, entityId) Then
                    Dim myItem As ObjectBase = myModel.Item(entityId)
                    If myItem IsNot Nothing Then
                        Dim myNamingItem As ObjectBaseNaming = CType(myItem, ObjectBaseNaming)
                        If GetKblId(myNamingItem) = id Then
                            entity = item
                            Exit For
                        End If
                    End If
                End If
            Next

            Return entity
        End Function

        Private Sub Trackbar_ValueChanged(sender As Object, e As EventArgs) Handles Trackbar.ValueChanged
            If Design3D Is Nothing Then
                Return
            End If

            Dim value As Integer = Trackbar.Value

            SetSelectability()

            TrackbarValueChanged(value)

            _oldTrackbarValue = value
            Design3D.Invalidate()
        End Sub

        Private Sub SetSelectability()
            Dim value As Integer = Trackbar.Value

            If value <= 0 AndAlso _oldTrackbarValue > 0 Then
                DisableComp()
                EnableRef()
            ElseIf value > 0 AndAlso _oldTrackbarValue <= 0 Then
                DisableRef()
                EnableComp()
            End If
        End Sub

        Private Function GetRefTrans() As Single
            Return 1 - CSng((Trackbar.Value + 100) / (200))
        End Function

        Private Function GetCompTrans() As Single
            Return CSng((Trackbar.Value + 100) / (200))
        End Function

        Public Sub TrackbarValueChanged(value As Integer)
            If Design3D Is Nothing Then
                Return
            End If
            Dim maxAlpha As Single = 1
            Dim transcomp As Single = GetCompTrans()
            Dim transref As Single = GetRefTrans()
            Dim refList As New List(Of IBaseModelEntityEx)
            Dim compList As New List(Of IBaseModelEntityEx)

            If _refSelection.Count > 0 OrElse _compSelection.Count > 0 OrElse _routedRefElements.Count > 0 OrElse _routedCompElements.Count > 0 Then
                maxAlpha = 0.1
            End If

            Dim myHiddens As List(Of String) = Design3D.HiddenEntities.Select(Function(h) CType(h.Entity, BaseModelEntity).Id).ToList()
            Dim myHiddenRefEntities As New List(Of IBaseModelEntityEx)
            Dim myHiddenCompEntities As New List(Of IBaseModelEntityEx)

            refList = _refChanges
            compList = _compChanges

            For Each hidden_id As String In myHiddens
                If _refListIdMapper.ContainsKey(hidden_id) Then
                    Dim entity As IBaseModelEntityEx = _refListIdMapper.Item(hidden_id)
                    If TypeOf entity Is VirtualGroupEntity Then
                        For Each item As IBaseModelEntityEx In CType(entity, VirtualGroupEntity)
                            SetTransparency(entity, Math.Min(_hiddenAlpha / 255, maxAlpha * transref))
                        Next
                    Else
                        SetTransparency(entity, Math.Min(_hiddenAlpha / 255, maxAlpha * transref))
                    End If
                    refList.Remove(_refListIdMapper.Item(hidden_id))
                    myHiddenRefEntities.Add(_refListIdMapper.Item(hidden_id))
                End If

                If _compListIdMapper.ContainsKey(hidden_id) Then
                    Dim entity As IBaseModelEntityEx = _compListIdMapper.Item(hidden_id)
                    If TypeOf entity Is VirtualGroupEntity Then
                        For Each item As IBaseModelEntityEx In CType(entity, VirtualGroupEntity)
                            Dim trans As Double = Math.Min(_hiddenAlpha / 255, maxAlpha * transcomp)
                            SetTransparency(CType(item, IBaseModelEntityEx), trans)
                        Next
                    Else
                        Dim trans As Double = Math.Min(_hiddenAlpha / 255, maxAlpha * transcomp)
                        SetTransparency(entity, trans)
                    End If

                    compList.Remove(_compListIdMapper.Item(hidden_id))
                    myHiddenCompEntities.Add(_compListIdMapper.Item(hidden_id))
                End If
            Next

            For Each entity As IBaseModelEntityEx In refList
                If TypeOf entity Is VirtualGroupEntity Then
                    For Each item As IBaseModelEntityEx In CType(entity, VirtualGroupEntity)
                        Dim en As BaseModelEntity = CType(item, BaseModelEntity)
                        Dim trans As Single = maxAlpha * transref
                        SetTransparency(en, trans)
                    Next
                Else
                    If Not (value > 0 AndAlso _refSelection.Contains(entity)) Then
                        Dim en As BaseModelEntity = CType(entity, BaseModelEntity)
                        Dim trans As Single = maxAlpha * transref
                        SetTransparency(en, trans)
                    End If
                End If
            Next

            For Each entity As IBaseModelEntityEx In compList
                If TypeOf entity Is VirtualGroupEntity Then
                    For Each item As IBaseModelEntityEx In CType(entity, VirtualGroupEntity)
                        Dim trans As Single = maxAlpha * transcomp
                        SetTransparency(entity, trans)
                    Next
                Else
                    If Not (value <= 0 AndAlso _compSelection.Contains(entity)) Then
                        Dim trans As Single = maxAlpha * transcomp
                        SetTransparency(entity, trans)
                    End If
                End If
            Next

            For Each en As IBaseModelEntityEx In _refSelection
                If TypeOf en Is VirtualGroupEntity Then

                    For Each item As IBaseModelEntityEx In CType(en, VirtualGroupEntity)
                        SetTransparency(CType(item, IBaseModelEntityEx), transref)
                    Next
                Else
                    SetTransparency(en, transref)
                End If
            Next

            For Each en As IBaseModelEntityEx In _compSelection
                If TypeOf en Is VirtualGroupEntity Then
                    For Each item As IBaseModelEntityEx In CType(en, VirtualGroupEntity)
                        SetTransparency(item, transcomp)
                    Next
                Else
                    SetTransparency(en, transcomp)
                End If
            Next

            For Each en As IBaseModelEntityEx In _routedRefElements
                SetTransparency(en, transref, en.Color)
            Next

            For Each en As IBaseModelEntityEx In _routedCompElements
                SetTransparency(en, transcomp, en.Color)
            Next
        End Sub

        Private Sub EnableRef()
            For Each en As IBaseModelEntityEx In _myRefEntities.Select(Function(item) item.Value).ToList
                If Not en.IsHidden(Design3D.HiddenEntities) Then
                    CType(en, IBaseModelEntityEx).Selectable = True
                End If
            Next
            For Each en As IBaseModelEntityEx In _routedRefElements
                If Not en.IsHidden(Design3D.HiddenEntities) Then
                    en.Selectable = True
                End If
            Next
        End Sub

        Private Sub DisableRef()
            For Each en As IBaseModelEntityEx In _myRefEntities.Select(Function(item) item.Value).ToList
                en.Selectable = False
            Next

            For Each en As IBaseModelEntityEx In _routedRefElements
                en.Selectable = False
            Next
        End Sub

        Private Sub EnableComp()
            For Each en As IBaseModelEntityEx In _myCompEntities.Select(Function(item) item.Value).ToList
                If Not en.IsHidden(Design3D.HiddenEntities) Then
                    en.Selectable = True
                End If
            Next

            For Each en As IBaseModelEntityEx In _routedCompElements
                If Not en.IsHidden(Design3D.HiddenEntities) Then
                    en.Selectable = True
                End If
            Next
        End Sub

        Private Sub DisableComp()
            For Each en As IBaseModelEntityEx In _myCompEntities.Select(Function(item) item.Value).ToList
                en.Selectable = False
            Next

            For Each en As IBaseModelEntityEx In _routedCompElements
                en.Selectable = False
            Next
        End Sub

        Private Sub D3DCntrl_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
            If e.KeyCode = Keys.Escape Then
                RaiseEvent SelectionChangedInD3DComparer(Me, New ComparerSelectionChangedEventArgs())
            End If
        End Sub

        Private Function GetChangedObjectsIds() As List(Of String)
            Dim mylist As New List(Of String)
            For Each item As KeyValuePair(Of KblObjectType, List(Of ChangedObject)) In _changedObjects
                For Each value As ChangedObject In item.Value
                    mylist.Add(value.SrcId)
                Next
            Next
            Return mylist
        End Function

        Private Sub InitFader()
            Fader.MaximumSize = New Size(350, 30)
            Fader.MinimumSize = New Size(350, 30)
            Fader.Margin = New Padding(0, 0, 0, 13)

            Me.Controls.Add(Fader)
            Fader.BringToFront()
            Trackbar.Value = -1
            Trackbar.Update()
            TrackbarValueChanged(-1)
        End Sub

        Private Sub AddToKblIdEntityMapper(key As String, obj As ChangedObject, en As IBaseModelEntityEx)
            Dim kblMapperId As String
            If obj.ModelType = ModelType.compare Then
                kblMapperId = _compMapper.Id
            Else
                kblMapperId = _refMapper.Id
            End If

            If Not _kblId_EntityMapper.ContainsKey(key) Then
                Dim myChangedObjectEx As New ChangedObjectEx(obj, en, kblMapperId)
                _kblId_EntityMapper.Add(key, myChangedObjectEx)

                If obj.ChangeType = CompareChangeType.Modified AndAlso obj.ModelType = ModelType.compare Then
                    Dim myModifiedObject As ChangedObjectEx = _kblId_EntityMapper.Values.Where(Function(val) val.KblIdRef = myChangedObjectEx.KblIdRef).FirstOrDefault
                    If myModifiedObject IsNot Nothing Then
                        myModifiedObject.ModifiedEntity = en
                        myChangedObjectEx.ModifiedEntity = myModifiedObject.Entity
                    End If
                End If
            End If
        End Sub

        Private Sub FillSourceObjects(en As IBaseModelEntityEx, myChangedObject As ChangedObject)
            If (en.EntityType = ModelEntityType.Fixing) Then
                If _sourceObjects.ContainsKey(en.Id) Then
                    _sourceObjects.Item(en.Id).Add(myChangedObject.SrcId)
                Else
                    _sourceObjects.Add(en.Id, New List(Of String) From {myChangedObject.SrcId})
                End If
            Else
                If _sourceObjects.ContainsKey(en.Id) Then
                    _sourceObjects.Item(en.Id).Add(myChangedObject.SrcId)
                Else
                    _sourceObjects.Add(en.Id, New List(Of String) From {myChangedObject.SrcId})
                End If
            End If
        End Sub

        Private Function GetChangedObject(item As ObjectBaseNaming, kblObjectType As KblObjectType, kblid As String, modelType As ModelType) As List(Of ChangedObject)
            Dim myChangedObject As ChangedObject = Nothing
            Dim myChangedObjects As New List(Of ChangedObject)

            If _changedObjects.ContainsKey(kblObjectType) Then
                If modelType = ModelType.reference Then
                    myChangedObjects = _changedObjects.Item(kblObjectType).Where(Function(entry) entry.KblIdRef = kblid AndAlso entry.ModelType = ModelType.reference).ToList()
                Else
                    myChangedObjects = _changedObjects.Item(kblObjectType).Where(Function(entry) entry.KblIdComp = kblid AndAlso entry.ModelType = ModelType.compare).ToList()
                End If
            End If

            Return myChangedObjects
        End Function

        Private Function SegmentHasChanges(kblid As String) As Boolean
            Dim res As Boolean = False
            If _changedObjects.ContainsKey(KblObjectType.Segment) Then
                Dim myChangedObjects As List(Of ChangedObject) = _changedObjects.Item(KblObjectType.Segment).Where(Function(entry) entry.KblIdRef = kblid AndAlso entry.ModelType = ModelType.reference).ToList()
                res = myChangedObjects.Count > 0
            End If
            Return res
        End Function

        Private Function GetKblId(obj As ObjectBaseNaming) As String
            Return obj.CustomAttributes.OfType(Of KblPropertyBagAttribute).SingleOrDefault?.PropertyBag.SystemId
        End Function


        Private Sub CheckForTopologyChanges()
            For Each compEntity As KeyValuePair(Of Guid, IBaseModelEntityEx) In _myCompEntities
                compEntity.Value.Selectable = True
                Design3D.Entities.Add(compEntity.Value)
                If _myRefEntities.ContainsKey(compEntity.Key) Then
                    Dim myref As IBaseModelEntityEx = _myRefEntities.Item(compEntity.Key)
                    If myref IsNot Nothing Then
                        If myref.EntityType = ModelEntityType.Fixing Then
                            Dim nRefVertices As Integer = 0
                            For Each ent As Entity In CType(myref, VirtualGroupEntity).Flatten
                                nRefVertices += ent.Vertices.Length
                            Next
                            Dim nCompVertices As Integer = 0
                            For Each ent As Entity In CType(compEntity.Value, VirtualGroupEntity).Flatten
                                nCompVertices += ent.Vertices.Length
                            Next

                            If myref.BoxMax = compEntity.Value.BoxMax AndAlso myref.BoxMin = compEntity.Value.BoxMin AndAlso nRefVertices = nCompVertices Then
                                Dim myrefEntity As IBaseModelEntityEx = _referenceDocument.Document.Entities.Where(Function(ref) ref.Id = myref.Id).First()
                                Dim col As Color = Color.FromArgb(255, myrefEntity.Color) '' HINT prevent alpha = 0 !!
                                myref.Color = col
                                _refChanges.Remove(myref)
                                _compChanges.Remove(compEntity.Value)
                            End If
                        Else
                            If myref.BoxMax = compEntity.Value.BoxMax AndAlso myref.BoxMin = compEntity.Value.BoxMin AndAlso CType(myref, Mesh).Vertices.Length = CType(compEntity.Value, Mesh).Vertices.Length Then
                                Dim myrefEntity As IBaseModelEntityEx = _referenceDocument.Document.Entities.Where(Function(ref) ref.Id = myref.Id).First()
                                Dim col As Color = Color.FromArgb(255, myrefEntity.Color) '' HINT prevent alpha = 0 !!
                                myref.Color = col
                                _refChanges.Remove(myref)
                                _compChanges.Remove(compEntity.Value)
                            End If
                        End If
                    End If
                End If
            Next
        End Sub

        Friend Function GetModifiedCompareId(refId As String, objType As String) As List(Of String)
            Dim result As New List(Of String)
            Dim objects As New List(Of ChangedObjectEx)
            Dim ids As New List(Of String)
            Dim Id As String = ""
            Dim refHarnessId As String = _refMapper.Id
            Dim comHarnessId As String = _compMapper.Id

            If Not String.IsNullOrEmpty(refId) Then

                Dim myType As KblObjectType = Nothing
                If objType = KblObjectType.Wire_occurrence.ToString Then
                    myType = KblObjectType.Wire_occurrence
                    Id = GetWireId(New List(Of String) From {refId}, refHarnessId)
                ElseIf objType = KblObjectType.Connector_occurrence.ToString Then
                    myType = KblObjectType.Connector_occurrence
                ElseIf objType = KblObjectType.Special_wire_occurrence.ToString Then
                    myType = KblObjectType.Special_wire_occurrence
                    Id = GetSpecialWireOccurrenceId(New List(Of String) From {refId}, refHarnessId)
                    If Not String.IsNullOrEmpty(Id) Then
                        refId = Id
                    End If
                ElseIf objType = KblObjectType.Net.ToString Then
                    myType = KblObjectType.Net
                ElseIf objType = KblObjectType.Node.ToString Then
                    myType = KblObjectType.Node
                ElseIf objType = KblObjectType.Segment.ToString Then
                    myType = KblObjectType.Segment
                ElseIf objType = KblObjectType.Fixing.ToString Then
                    myType = KblObjectType.Fixing
                ElseIf objType = KblObjectType.Accessory.ToString Then
                    myType = KblObjectType.Accessory

                End If

                If myType <> Nothing Then

                    Dim key As String = refHarnessId.ToString + "|" + refId
                    Dim refObj As ChangedObject = Nothing
                    Dim myModelType As ModelType = ModelType.reference

                    If myType = KblObjectType.Wire_occurrence Then
                        refObj = _changedObjects.Item(myType).Where(Function(entry) entry.SrcId = Id AndAlso entry.ObjType = myType AndAlso entry.SourceType = myType AndAlso entry.ModelType = ModelType.reference).FirstOrDefault()
                        If refObj IsNot Nothing Then result.Add(refObj.KblIdComp)

                    ElseIf myType = KblObjectType.Connection Then
                        Dim refobjects As List(Of ChangedObject) = _changedObjects.Item(myType).Where(Function(entry) entry.ObjType = myType AndAlso entry.ModelType = ModelType.reference AndAlso entry.ChangeType = CompareChangeType.Modified).ToList
                        Dim myRefObject As ChangedObject = refobjects.Where(Function(entry) entry.WireId = refId).FirstOrDefault

                        Dim allCompareObjects As List(Of ChangedObject)
                        allCompareObjects = _changedObjects.Item(myType).Where(Function(entry) entry.ObjType = myType AndAlso entry.ModelType = ModelType.compare AndAlso entry.ChangeType = CompareChangeType.Modified).ToList()

                        Dim mynetObjects As List(Of ChangedObject) = allCompareObjects.Where(Function(entry) entry.Net = myRefObject.Net).ToList

                        For Each net As ChangedObject In mynetObjects
                            result.Add(net.WireId)
                        Next

                    ElseIf myType = KblObjectType.Special_wire_occurrence Then
                        refObj = _changedObjects.Item(myType).Where(Function(entry) entry.SrcId = refId AndAlso entry.ObjType = myType AndAlso entry.SourceType = myType AndAlso entry.ModelType = myModelType).FirstOrDefault()
                        If refObj IsNot Nothing Then result.Add(refObj.KblIdComp)
                    Else
                        refObj = _changedObjects.Item(myType).Where(Function(entry) entry.SrcId = refId AndAlso entry.ObjType = myType AndAlso entry.ModelType = ModelType.reference).FirstOrDefault()
                        If refObj IsNot Nothing Then result.Add(refObj.KblIdComp)
                    End If


                    If objects.Count > 0 Then
                        If objType = KblObjectType.Wire_occurrence.ToString Then
                            ids.Add(objects.First.WireId)
                        End If
                    End If
                End If
            End If
            Return result
        End Function
        Private Function GetAttachedEEObjectContainingDocument(eeObjectId As Guid) As HcvDocument
            For Each doc_clone As DocumentClone In _documents
                If doc_clone.Document?.Model IsNot Nothing AndAlso doc_clone.Document.Model.Contains(eeObjectId) Then
                    Return doc_clone.Document
                End If
            Next
            Return Nothing
        End Function

        Private Sub D3D_HandleCreated(sender As Object, e As EventArgs) Handles Design3D.HandleCreated
            If Me.IsHandleCreated AndAlso Design3D.IsHandleCreated Then
                Design3D.SuspendUpdate(False)
                Design3D.ActiveViewport.UpdateWorkspace()
                Design3D.ActiveViewport.ZoomFit() 'Model.ZoomFit()
            End If
        End Sub

        Private Sub D3DComparerControl_HandleCreated(sender As Object, e As EventArgs) Handles Me.HandleCreated
            If Design3D.IsHandleCreated AndAlso Me.IsHandleCreated Then
                Design3D.ActiveViewport.ZoomFit()
            End If
        End Sub

        Public ReadOnly Property SelectedEntities As IEESelectedEntitiesCollection
            Get
                Return Design3D.SelectedEntities
            End Get
        End Property

        Private Sub Model3D_MouseEnterEntity(sender As Object, e As MouseEntityEventArgs) Handles Design3D.MouseEnterEntity
            If _generalSettings IsNot Nothing AndAlso _generalSettings.AutomaticTooltips Then
                _ttManager.Show(e.Entity, _mergedModels, ToolTipCaptionContextMenuStrip, Nothing, Control.MousePosition, True)
            End If
        End Sub

        Private Sub Model3D_MouseLeaveEntity(sender As Object, e As MouseEntityEventArgs) Handles Design3D.MouseLeaveEntity
            _ttManager.Close(e.Entity, delayed:=Not e.LostFocus)
        End Sub

        Private Sub toolTip_BeforeHide(sender As Object, e As CancelToolTipExEventArgs) Handles _ttManager.BeforeHide
            If TypeOf e.ToolTip.Control Is ToolTips.EntityToolTipControl Then
                Dim popUpControl As ToolTips.EntityToolTipControl = CType(e.ToolTip.Control, ToolTips.EntityToolTipControl)
                e.Cancel = _isSelectingFromProperties OrElse Not popUpControl.IsAllowedToClose OrElse (popUpControl?.Entity IsNot Nothing AndAlso Me.Design3D.GetEntityUnderMouseCursor() Is popUpControl.Entity)
            End If
        End Sub

        Public Sub ShowEntityToolTip(entity As IEntity, Optional delayed As Boolean = False)
            If _ttManager IsNot Nothing Then
                Dim col As New E3.Lib.Model.EEModelsCollection()
                col.Add(_refEEModel)
                col.Add(_compEEModel)

                _ttManager.Show(entity, _mergedModels, ToolTipCaptionContextMenuStrip, Nothing, Control.MousePosition, delayed)
            End If
        End Sub

        Private Sub D3D_SelectionChanged(sender As Object, e As Workspace.SelectionChangedEventArgs) Handles Design3D.SelectionChanged

            If _isHubSelection Then
                Exit Sub
            End If

            If e.AddedItems.Count = 0 AndAlso e.RemovedItems.Count = 0 Then
                Exit Sub
            End If

            ClearSelectedEntities(e)

            _isInSelection = True

            Dim changedObject As ChangedObjectEx = Nothing
            Dim otherParts As New List(Of String)
            Dim newItems As New List(Of IBaseModelEntityEx)
            If e.AddedItems IsNot Nothing Then

                Dim myEntity As BaseModelEntity
                If e.AddedItems.Count > 0 AndAlso e.AddedItems.FirstOrDefault IsNot Nothing Then
                    myEntity = CType(e.AddedItems.First.Item, BaseModelEntity)
                    Dim kblMapperId As String = CurrentMapperId()
                    Dim myCurrentValues As List(Of KeyValuePair(Of String, ChangedObjectEx)) = _kblId_EntityMapper.Where(Function(k) k.Key.Contains(kblMapperId)).ToList
                    changedObject = myCurrentValues.Where(Function(val) val.Value.Entity.Id = myEntity.Id).FirstOrDefault.Value
                    If changedObject Is Nothing Then
                        Dim wireObjects As List(Of ChangedObjectEx) = GetConnectorParts(myEntity.Id)
                        If wireObjects.Count > 0 Then
                            ' SetMultiSelect()  'TODO MR this is not used in trunk version??
                            changedObject = wireObjects.First
                        End If
                    End If
                    OnSelectedEntitiesChanged(myEntity, changedObject, otherParts)
                Else
                    OnSelectedEntitiesChanged(Nothing, Nothing, Nothing)
                End If
            End If

            Design3D.ActiveViewport.Zoom.ZoomStyle = zoomStyleType.AtCursorLocation
            _isInSelection = False
        End Sub

        Protected Overridable Sub OnSelectedEntitiesChanged(en As IBaseModelEntityEx, changedObject As ChangedObjectEx, otherParts As List(Of String))
            If EventsEnabled Then
                _isSelecting = True
                Try
                    If en IsNot Nothing Then
                        Dim id As String = CType(en, BaseModelEntity).Id
                        Dim ids As New List(Of String)
                        Dim kbl_mapper_id As String = String.Empty

                        If changedObject IsNot Nothing Then

                            If changedObject.ChangeType = CompareChangeType.New Then
                                If _inverse Then
                                    ids.Add(changedObject.KblIdRef)
                                    kbl_mapper_id = _refMapper.Id
                                Else
                                    ids.Add(changedObject.KblIdComp)
                                    kbl_mapper_id = _compMapper.Id
                                End If
                            ElseIf changedObject.ChangeType = CompareChangeType.Routed Then
                                'TODO MR inverse Compare ??
                                ids.Add(changedObject.SrcId)
                                kbl_mapper_id = changedObject.MapperSourceId
                            Else
                                'TODO MR inverse Compare ??
                                kbl_mapper_id = CurrentMapperId()
                                If kbl_mapper_id = _refMapper.Id Then
                                    ids.Add(changedObject.KblIdRef)
                                Else
                                    ids.Add(changedObject.KblIdComp)
                                End If
                            End If

                            If changedObject.ObjType = KblObjectType.Wire_protection_occurrence Then
                                Dim key As String = changedObject.MapperSourceId.ToString + "|" + changedObject.SrcId
                                If _kblId_EntityMapper.ContainsKey(key) Then
                                    Dim seg As ChangedObjectEx = _kblId_EntityMapper.Item(key)
                                    If seg IsNot Nothing Then
                                        If CurrentMapperId = _refMapper.Id Then
                                            ids.Add(seg.KblIdRef)
                                        Else
                                            ids.Add(seg.KblIdComp)
                                        End If
                                    End If
                                End If
                            End If
                        End If

                        If otherParts.Count > 0 Then
                            ids.AddRange(otherParts)
                        End If

                        If ids.Count > 0 Then
                            Dim myEventargs As New ComparerSelectionChangedEventArgs(en, kbl_mapper_id, ids, changedObject)
                            RaiseEvent SelectionChangedInD3DComparer(Me, myEventargs)
                        End If

                    Else
                        If _generalSettings.UseSelectionCenterForRotation Then
                            Design3D.ActiveViewport.Rotate.RotationCenter = rotationCenterType.CursorLocation
                        End If
                        Dim myEventargs As New ComparerSelectionChangedEventArgs()
                        myEventargs.KblMapperSourceId = CurrentMapperId()
                        RaiseEvent SelectionChangedInD3DComparer(Me, myEventargs)
                    End If
                    CheckSelection()
                Finally
                    _isSelecting = False
                End Try
            End If
        End Sub

        Private Sub SetZoomStyle()
            If GetSelection.Count > 0 Then
                For Each vp As Viewport In Design3D.Viewports
                    vp.Zoom.ZoomStyle = zoomStyleType.Centered
                Next
            Else
                For Each vp As Viewport In Design3D.Viewports
                    vp.Zoom.ZoomStyle = zoomStyleType.AtCursorLocation
                Next
            End If
        End Sub

        Private Function GetConnectorParts(entityId As String) As List(Of ChangedObjectEx)
            Dim result As New List(Of String)
            Dim myObjects As New List(Of ChangedObjectEx)
            For Each entry As KeyValuePair(Of String, List(Of ChangedObjectEx)) In _wireId_EntityMapper
                If entry.Key.Contains(CurrentMapperId.ToString) Then
                    For Each obj As ChangedObjectEx In entry.Value
                        If obj.Entity.Id = entityId Then
                            If Not result.Contains(obj.SrcId) Then
                                result.Add(obj.SrcId)
                                'result.Add(obj.ChangeType.ToString + "|" + obj.SrcId)
                                myObjects.Add(obj)
                            End If
                        End If
                    Next
                End If
            Next
            Return myObjects
        End Function

        Friend Function GetCurrentKBLMapper() As KblMapper
            Dim id As String = CurrentMapperId()
            If id = _compMapper.Id Then
                Return _compMapper
            Else
                Return _refMapper
            End If
        End Function

        Friend ReadOnly Property CurrentMapperId As String
            Get
                If Trackbar.Value <= 0 Then
                    Return _refMapper.Id
                Else
                    Return _compMapper.Id
                End If
            End Get
        End Property

        Private Sub CheckSelection()
            _refSelection.Clear()
            _compSelection.Clear()

            For Each item As Entity In GetSelection()
                Dim entity As IBaseModelEntityEx
                If TypeOf (item) Is FixingEntity Then
                    entity = CType(CType(item, FixingEntity).Group, IBaseModelEntityEx)
                Else
                    entity = CType(item, IBaseModelEntityEx)
                End If

                If _refChanges.Contains(entity) Then
                    _refSelection.Add(entity)
                ElseIf _compChanges.Contains(entity) Then
                    _compSelection.Add(entity)
                End If
            Next
            TrackbarValueChanged(_oldTrackbarValue)
        End Sub

        Private Function GetSegmentIdInRefFromComp(kblid As String) As String
            Dim id As String = ""
            Dim segComp As [Lib].Schema.Kbl.Segment = _compMapper.GetSegments.Where(Function(s) s.SystemId = kblid).FirstOrDefault
            If segComp IsNot Nothing Then
                Dim segRef As [Lib].Schema.Kbl.Segment = _refMapper.GetSegments.Where(Function(s) s.Id = segComp.Id).FirstOrDefault
                If segRef IsNot Nothing Then
                    id = segRef.SystemId
                End If
            End If

            Return id
        End Function

        Private Function GetConnectionFromWire(wireId As String, mapperSourceId As String) As Connection
            Dim cn As Connection = Nothing
            Dim mapper As KblMapper
            If mapperSourceId = _refMapper.Id Then
                mapper = _refMapper
            Else
                mapper = _compMapper
            End If

            If mapper.KBLWireNetMapper.ContainsKey(wireId) Then
                cn = mapper.KBLWireNetMapper.Item(wireId)
            End If

            Return cn
        End Function

        Private Function GetConnectionsFromSignalName(name As String, kblMapperId As String) As List(Of ChangedObject)
            Dim myitems As List(Of ChangedObject) = _changedObjects.GetValueOrEmpty(KblObjectType.Connection).Where(Function(entry) entry.Net = name).ToList()
            Dim items As New List(Of ChangedObject)
            If (kblMapperId = _refMapper.Id) Then
                items = myitems.Where(Function(p) p.ModelType = ModelType.reference).ToList()
            Else
                items = myitems.Where(Function(p) p.ModelType = ModelType.compare).ToList()
            End If
            Return items
        End Function

        Private Sub SelectRoutedSegmentsFromNet(ids As List(Of String), harnessid As String)

            Dim myCoreIds As List(Of String) = GetCoreIds(ids, harnessid)
            Dim myWireIds As List(Of String) = GetWireIds(ids, harnessid)
            Dim isref As Boolean = False
            If harnessid.ToString = _refMapper.Id Then
                isref = True
            End If

            Dim myModelTYpe As ModelType
            If isref Then
                myModelTYpe = ModelType.reference
            Else
                myModelTYpe = ModelType.compare
            End If



            For Each id As String In myWireIds
                Dim myObjects As List(Of ChangedObject) = _changedObjects.Item(KblObjectType.Connection).Where(Function(p) p.WireId = id AndAlso p.ModelType = myModelTYpe).ToList
                Dim myNet As String = ""
                If myObjects.Count > 0 Then
                    myNet = myObjects.First.Net
                End If

                Dim mySegments As List(Of ChangedObject) = _changedObjects.Item(KblObjectType.Segment).Where(Function(p) p.Net = myNet).ToList
                For Each Segment As ChangedObject In mySegments
                    If Not String.IsNullOrEmpty(Segment.KblIdRef) Then
                        Dim k As String = GetKey(harnessid, Segment.KblIdRef)
                        If _kblId_EntityMapper.ContainsKey(k) Then
                            Dim myItem As ChangedObjectEx = _kblId_EntityMapper.Item(k)
                            If myItem IsNot Nothing AndAlso myItem.Entity IsNot Nothing Then
                                If Not _routedRefElements.Contains(myItem.Entity) Then _routedRefElements.Add(myItem.Entity)
                            End If
                        End If
                    End If
                    If Not String.IsNullOrEmpty(Segment.KblIdComp) Then
                        Dim k As String = GetKey(_compMapper.Id, Segment.KblIdComp)
                        If _kblId_EntityMapper.ContainsKey(k) Then
                            Dim myItem As ChangedObjectEx = _kblId_EntityMapper.Item(k)
                            If myItem IsNot Nothing AndAlso myItem.Entity IsNot Nothing Then
                                If Not _routedCompElements.Contains(myItem.Entity) Then _routedCompElements.Add(myItem.Entity)
                            End If
                        End If
                    End If

                Next
            Next

            For Each id As String In myCoreIds
                Dim myObjects As List(Of ChangedObject) = _changedObjects.Item(KblObjectType.Connection).Where(Function(p) p.WireId = id AndAlso p.ModelType = myModelTYpe).ToList
                Dim myNet As String = ""
                If myObjects.Count > 0 Then
                    myNet = myObjects.First.Net
                End If
                Dim mySegments As List(Of ChangedObject) = _changedObjects.Item(KblObjectType.Segment).Where(Function(p) p.Net = myNet).ToList
                For Each Segment As ChangedObject In mySegments
                    If Not String.IsNullOrEmpty(Segment.KblIdRef) Then
                        Dim k As String = GetKey(harnessid, Segment.KblIdRef)
                        If _kblId_EntityMapper.ContainsKey(k) Then
                            Dim myItem As ChangedObjectEx = _kblId_EntityMapper.Item(k)
                            If myItem IsNot Nothing AndAlso myItem.Entity IsNot Nothing Then
                                If Not _routedRefElements.Contains(myItem.Entity) Then _routedRefElements.Add(myItem.Entity)
                            End If
                        End If
                    End If
                    If Not String.IsNullOrEmpty(Segment.KblIdComp) Then
                        Dim k As String = GetKey(_compMapper.Id, Segment.KblIdComp)
                        If _kblId_EntityMapper.ContainsKey(k) Then
                            Dim myItem As ChangedObjectEx = _kblId_EntityMapper.Item(k)
                            If myItem IsNot Nothing AndAlso myItem.Entity IsNot Nothing Then
                                If Not _routedCompElements.Contains(myItem.Entity) Then _routedCompElements.Add(myItem.Entity)
                            End If
                        End If
                    End If

                Next
            Next

        End Sub

        Private Sub SelectionChanged(harnessId As String, ids As List(Of String), objType As KblObjectType)

            Dim currentSelection As List(Of Entity) = GetSelection()
            Dim myChangeType As CompareChangeType = GetChangeType()

            If (objType = KblObjectType.Wire_occurrence) Then
                If _generalSettings.Mark3DConnectorsOnWireModification Then SelectConnectorsFromWire(ids, harnessId)
                SelectRoutedSegmentsFromWire(ids, harnessId)

            ElseIf (objType = KblObjectType.Special_wire_occurrence) Then
                If _generalSettings.Mark3DConnectorsOnWireModification Then SelectConnectorsFromCable(ids, harnessId)
                SelectRoutedSegmentsFromCable(ids, harnessId)

            ElseIf objType = KblObjectType.Net Then
                SelectRoutedSegmentsFromNet(ids, harnessId)

            ElseIf objType = KblObjectType.Cavity Then
                SelectConnectorFromCavity(ids, harnessId)

            Else
                myChangeType = SelectObjects(ids, harnessId)
            End If

            _refSelection = _refSelection.Distinct.ToList
            _compSelection = _compSelection.Distinct.ToList

            For Each en As IBaseModelEntityEx In _refSelection
                If TypeOf en Is VirtualGroupEntity Then
                    For Each item As IBaseEntity In CType(en, VirtualGroupEntity)
                        CType(item, IBaseEntity).Selected = True
                    Next
                End If
                en.Selected = True
            Next

            For Each en As IBaseModelEntityEx In _compSelection
                If TypeOf en Is VirtualGroupEntity Then
                    For Each item As IBaseEntity In CType(en, VirtualGroupEntity)
                        CType(item, IBaseEntity).Selected = True
                    Next
                End If
                en.Selected = True
            Next


            For Each en As IBaseModelEntityEx In _routedCompElements
                CType(en, IBaseEntity).Selectable = True
                CType(en, IBaseEntity).Selected = True
            Next
            For Each en As IBaseModelEntityEx In _routedRefElements
                CType(en, IBaseEntity).Selectable = True
                CType(en, IBaseEntity).Selected = True

            Next

            If _inverse Then
                If myChangeType = CompareChangeType.Deleted AndAlso _oldTrackbarValue <= 0 Then
                    Trackbar.Value = -_oldTrackbarValue
                ElseIf myChangeType = CompareChangeType.New AndAlso _oldTrackbarValue > 0 Then
                    Trackbar.Value = -_oldTrackbarValue
                End If
            Else
                If myChangeType = CompareChangeType.Deleted AndAlso _oldTrackbarValue > 0 Then
                    Trackbar.Value = -_oldTrackbarValue
                ElseIf myChangeType = CompareChangeType.New AndAlso _oldTrackbarValue <= 0 Then
                    Trackbar.Value = -_oldTrackbarValue
                End If
            End If

            TrackbarValueChanged(Trackbar.Value)
        End Sub

        Private Function SelectConnectorFromCavity(ids As List(Of String), Harnessid As String) As CompareChangeType
            Dim change As CompareChangeType
            If _compareHub.ActiveGrid.ActiveRow IsNot Nothing Then
                Dim r As UltraGridRow = _compareHub.ActiveGrid.ActiveRow
                Dim parent As UltraGridRow = r.ParentRow
                Dim id As String = GetIdFromRow(parent)
                change = SelectObjects(New List(Of String) From {id}, Harnessid.ToString)
            End If
            Return change
        End Function

        Private Function GetChangeType(key As String) As CompareChangeType
            Static pipe_char As Char() = {"|"c}
            Dim myChangeType As String = key.Split(pipe_char).First

            If myChangeType = "Added" Then
                Return CompareChangeType.New
            ElseIf myChangeType = "Modified" Then
                Return CompareChangeType.Modified
            ElseIf myChangeType = "Deleted" Then
                Return CompareChangeType.Deleted
            End If

            Return CompareChangeType.Deleted
        End Function

        Friend Function GetChangeType() As CompareChangeType

            ''this hack is necessary because the handling of selection and activation of rows depends on click in Grid (=> activation of row) or click on a graphic element (=> selection of row)

            Dim myChangeType As CompareChangeType = Nothing
            Dim myTag As KeyValuePair(Of String, Object) = Nothing
            If _compareHub.ActiveGrid.Selected IsNot Nothing AndAlso _compareHub.ActiveGrid.ActiveRow IsNot Nothing AndAlso TypeOf _compareHub.ActiveGrid.ActiveRow.Tag Is KeyValuePair(Of String, Object) Then
                myTag = CType(_compareHub.ActiveGrid.ActiveRow.Tag, KeyValuePair(Of String, Object))
            Else
                If _compareHub.ActiveGrid.Selected IsNot Nothing AndAlso _compareHub.ActiveGrid.Selected.Rows.Count > 0 AndAlso TypeOf _compareHub.ActiveGrid.Selected.Rows.Item(0).Tag Is KeyValuePair(Of String, Object) Then
                    myTag = CType(_compareHub.ActiveGrid.Selected.Rows.Item(0).Tag, KeyValuePair(Of String, Object))
                End If
            End If
            If myTag.Key <> Nothing Then
                myChangeType = GetChangeType(myTag.Key)
            End If
            Return myChangeType
        End Function

        Friend Function GetIdFromRow(r As UltraGridRow) As String
            Static pipe_char As Char() = {"|"c}
            Dim id As String = ""
            Dim myTag As KeyValuePair(Of String, Object)
            If TypeOf r.Tag Is KeyValuePair(Of String, Object) Then
                myTag = CType(r.Tag, KeyValuePair(Of String, Object))
                id = myTag.Key.Split(pipe_char)(1)
            End If
            Return id
        End Function

        Private Sub _compareHub_HubSelectionChanged(sender As InformationHub, e As InformationHubEventArgs) Handles _compareHub.HubSelectionChanged
            If _isInSelection OrElse e.ObjectType = KblObjectType.Cavity_occurrence Then
                Exit Sub
            End If

            _isHubSelection = True
            ClearSelectedEntities()

            Design3D.Invalidate()

            SelectionChanged(e.KblMapperSourceId, e.ObjectIds.ToList, e.ObjectType)
            ZoomfitActiveToSelection()
            SetZoomStyle()
            Design3D.Invalidate()
            _isHubSelection = False
        End Sub

        Private Sub SelectConnectorsFromCable(ids As IEnumerable(Of String), mapperSourceId As String)
            Dim coreIds As List(Of String) = GetCoreIds(ids, mapperSourceId)
            Dim key As String = ""

            Dim myId As String = GetSpecialWireOccurrenceId(ids, mapperSourceId)
            Dim kblObjectType As [Lib].Schema.Kbl.KblObjectType = [Lib].Schema.Kbl.KblObjectType.Special_wire_occurrence
            Dim myModelType As ModelType = ModelType.reference

            If mapperSourceId = _compMapper.Id Then
                myModelType = ModelType.compare
            End If

            Dim myChangedObject As ChangedObject = _changedObjects.Item(kblObjectType).Where(Function(entry) entry.SrcId = myId AndAlso entry.ObjType = kblObjectType AndAlso entry.SourceType = kblObjectType AndAlso entry.ModelType = myModelType).FirstOrDefault()
            If myChangedObject IsNot Nothing Then
                If Not String.IsNullOrEmpty(myChangedObject.KblIdRef) Then
                    key = _refMapper.Id.ToString + "|" + myId
                    If _wireId_EntityMapper.ContainsKey(key) Then
                        Dim myObjects As List(Of ChangedObjectEx) = _wireId_EntityMapper.Item(key)
                        Dim connectors As List(Of ChangedObjectEx) = myObjects.Where(Function(c) c.ObjType = Global.Zuken.E3.Lib.Schema.Kbl.KblObjectType.Connector_occurrence AndAlso c.ID = myChangedObject.ID).ToList
                        For Each item As ChangedObjectEx In connectors
                            If item.Entity IsNot Nothing AndAlso Not _refSelection.Contains(item.Entity) AndAlso Not item.Entity.IsHidden(Design3D.HiddenEntities) Then
                                _refSelection.Add(item.Entity)
                            End If
                        Next
                    End If
                End If

                If Not String.IsNullOrEmpty(myChangedObject.KblIdComp) Then
                    key = _compMapper.Id.ToString + "|" + myId
                    If _wireId_EntityMapper.ContainsKey(key) Then
                        Dim myObjects As List(Of ChangedObjectEx) = _wireId_EntityMapper.Item(key)
                        Dim connectors As List(Of ChangedObjectEx) = myObjects.Where(Function(c) c.ObjType = Global.Zuken.E3.Lib.Schema.Kbl.KblObjectType.Connector_occurrence AndAlso c.ID = myChangedObject.ID).ToList
                        For Each item As ChangedObjectEx In connectors
                            If item.Entity IsNot Nothing AndAlso Not _compSelection.Contains(item.Entity) AndAlso Not item.Entity.IsHidden(Design3D.HiddenEntities) Then
                                _compSelection.Add(item.Entity)
                                If Design3D.Entities.IndexOf(CType(item.Entity, Entity)) < 0 Then
                                    Design3D.Entities.Add(item.Entity)
                                    _tmpEntities.Add(item.Entity)
                                End If
                            End If
                        Next
                    End If
                End If
            End If
        End Sub

        Private Function SelectRoutedSegmentsFromCable(ids As IEnumerable(Of String), mapperSourceId As String) As CompareChangeType
            Dim myCompIds As New List(Of String)
            Dim myChangedObject As ChangedObject = Nothing
            Dim myChangeType As CompareChangeType = CompareChangeType.Modified

            Dim myId As String = GetSpecialWireOccurrenceId(ids, mapperSourceId)
            Dim myModelType As ModelType = ModelType.reference
            If mapperSourceId = _compMapper.Id Then
                myModelType = ModelType.compare
            End If

            Dim objects As List(Of ChangedObject) = _changedObjects.Item(KblObjectType.Special_wire_occurrence).Where(Function(entry) entry.SrcId = myId AndAlso entry.ObjType = KblObjectType.Special_wire_occurrence AndAlso entry.SourceType = KblObjectType.Special_wire_occurrence AndAlso entry.ModelType = myModelType).ToList()

            For Each myChangedObject In objects

                If myChangedObject IsNot Nothing Then
                    myChangeType = myChangedObject.ChangeType
                    If Not String.IsNullOrEmpty(myChangedObject.KblIdRef) Then
                        Dim key As String = _refMapper.Id.ToString + "|" + myChangedObject.KblIdRef
                        If _wireId_EntityMapper.ContainsKey(key) Then
                            Dim myObjects As List(Of ChangedObjectEx) = _wireId_EntityMapper.Item(key)
                            Dim segments As List(Of ChangedObjectEx) = myObjects.Where(Function(c) c.ObjType = Global.Zuken.E3.Lib.Schema.Kbl.KblObjectType.Segment).ToList
                            For Each item As ChangedObjectEx In segments
                                If item.Entity IsNot Nothing AndAlso Not _refSelection.Contains(item.Entity) Then
                                    _refSelection.Add(item.Entity)
                                End If
                            Next
                        End If
                    End If

                    If Not String.IsNullOrEmpty(myChangedObject.KblIdComp) Then
                        Dim key As String = _compMapper.Id.ToString + "|" + myChangedObject.SrcId
                        If _wireId_EntityMapper.ContainsKey(key) Then
                            Dim myObjects As List(Of ChangedObjectEx) = _wireId_EntityMapper.Item(key)
                            Dim segments As List(Of ChangedObjectEx) = myObjects.Where(Function(c) c.ObjType = KblObjectType.Segment).ToList
                            For Each item As ChangedObjectEx In segments
                                If item.Entity IsNot Nothing AndAlso Not _compSelection.Contains(item.Entity) Then
                                    _compSelection.Add(item.Entity)
                                End If
                            Next
                        End If
                    End If
                End If
            Next
            Return myChangeType
        End Function

        Private Function GetSpecialWireOccurrenceId(ids As IEnumerable(Of String), mapperSourceId As String) As String
            Dim res As String = ""
            Dim myMapper As KblMapper
            If mapperSourceId = _refMapper.Id Then
                myMapper = _refMapper
            Else
                myMapper = _compMapper
            End If

            For Each id As String In ids
                If myMapper.KBLCoreCableMapper.ContainsKey(id) Then
                    res = myMapper.KBLCoreCableMapper.Item(id)
                    Exit For
                End If
            Next

            Return res
        End Function
        Private Function GetKey(Harnessid As Guid, id As String) As String
            Dim key As String = Harnessid.ToString + "|" + id
            Return key
        End Function
        Private Function GetKey(Harnessid As String, id As String) As String
            Dim key As String = Harnessid + "|" + id
            Return key
        End Function

        Private Function SelectObjects(ids As IEnumerable(Of String), mapperSourceId As String) As CompareChangeType
            Dim myChangeType As CompareChangeType = CompareChangeType.Modified

            For Each id As String In ids
                Dim key As String = mapperSourceId + "|" + id
                Dim myObj As ChangedObjectEx = Nothing

                If _kblId_EntityMapper.ContainsKey(key) Then
                    myObj = _kblId_EntityMapper.Item(key)
                End If

                If myObj IsNot Nothing Then
                    myChangeType = myObj.ChangeType
                    If myObj.ChangeType = CompareChangeType.Modified Then

                        Dim myRefKey As String = _refMapper.Id.ToString + "|" + myObj.KblIdRef
                        If _kblId_EntityMapper.ContainsKey(myRefKey) Then
                            Dim myRefObj As ChangedObjectEx = _kblId_EntityMapper(myRefKey)
                            If myRefObj.Entity IsNot Nothing Then
                                _refSelection.Add(myRefObj.Entity)
                            End If
                        End If

                        Dim myCompKey As String = _compMapper.Id.ToString + "|" + myObj.KblIdComp
                        If _kblId_EntityMapper.ContainsKey(myCompKey) Then
                            Dim myCompObj As ChangedObjectEx = _kblId_EntityMapper(myCompKey)
                            If myCompObj.Entity IsNot Nothing Then
                                _compSelection.Add(myCompObj.Entity)
                            End If
                        End If
                    ElseIf myObj.ChangeType = CompareChangeType.Deleted Then
                        _refSelection.Add(myObj.Entity)
                        If Trackbar.Value > 0 Then
                            Trackbar.Value = -Trackbar.Value
                        End If

                    ElseIf myObj.ChangeType = CompareChangeType.New Then
                        _compSelection.Add(myObj.Entity)
                        If Trackbar.Value <= 0 Then
                            Trackbar.Value = -Trackbar.Value
                        End If
                    End If
                End If
            Next
            Return myChangeType
        End Function

        Private Sub SelectConnectorsFromWire(ids As IEnumerable(Of String), mapperSourceId As String)
            Dim wireId As String = GetWireId(ids, mapperSourceId)
            Dim key As String = ""
            Dim myModelType As ModelType = ModelType.reference

            If mapperSourceId = _compMapper.Id Then
                myModelType = ModelType.compare
            End If

            Dim myChangedObject As ChangedObject = _changedObjects.Item(KblObjectType.Wire_occurrence).Where(Function(entry) entry.SrcId = wireId AndAlso entry.ObjType = KblObjectType.Wire_occurrence AndAlso entry.ModelType = myModelType).FirstOrDefault()
            If Not String.IsNullOrEmpty(myChangedObject.KblIdRef) Then

                key = _refMapper.Id.ToString + "|" + wireId
                If _wireId_EntityMapper.ContainsKey(key) Then
                    Dim myObjects As List(Of ChangedObjectEx) = _wireId_EntityMapper.Item(key)
                    Dim connectors As List(Of ChangedObjectEx) = myObjects.Where(Function(c) c.ObjType = KblObjectType.Connector_occurrence AndAlso c.ID = myChangedObject.ID AndAlso c.ModelType = ModelType.reference).ToList
                    For Each item As ChangedObjectEx In connectors
                        If item.Entity IsNot Nothing AndAlso Not _refSelection.Contains(item.Entity) AndAlso Not item.Entity.IsHidden(Design3D.HiddenEntities) Then
                            _refSelection.Add(item.Entity)
                        End If
                    Next
                End If
            End If

            If Not String.IsNullOrEmpty(myChangedObject.KblIdComp) Then
                key = _compMapper.Id.ToString + "|" + wireId
                If _wireId_EntityMapper.ContainsKey(key) Then
                    Dim myObjects As List(Of ChangedObjectEx) = _wireId_EntityMapper.Item(key)
                    Dim connectors As List(Of ChangedObjectEx) = myObjects.Where(Function(c) c.ObjType() = KblObjectType.Connector_occurrence AndAlso c.ID = myChangedObject.ID AndAlso c.ModelType = ModelType.compare).ToList
                    For Each item As ChangedObjectEx In connectors
                        If item.Entity IsNot Nothing AndAlso Not _compSelection.Contains(item.Entity) AndAlso Not item.Entity.IsHidden(Design3D.HiddenEntities) Then
                            _compSelection.Add(item.Entity)
                            If Design3D.Entities.IndexOf(CType(item.Entity, Entity)) < 0 Then
                                Design3D.Entities.Add(item.Entity)
                                _tmpEntities.Add(item.Entity)
                            End If
                        End If
                    Next
                End If
            End If
        End Sub

        Private Function SelectRoutedSegmentsFromWire(ids As IEnumerable(Of String), mapperSourceId As String) As CompareChangeType
            Dim myChangeType As CompareChangeType = CompareChangeType.Modified
            Dim myChangedWire As ChangedObject = Nothing
            Dim wireId As String = GetWireId(ids, mapperSourceId)
            Dim myModelType As ModelType = ModelType.reference

            If mapperSourceId = _compMapper.Id Then
                myModelType = ModelType.compare
            End If

            If _changedObjects.ContainsKey(KblObjectType.Wire_occurrence) Then
                myChangedWire = _changedObjects.Item(KblObjectType.Wire_occurrence).Where(Function(entry) entry.SrcId = wireId AndAlso entry.ObjType = KblObjectType.Wire_occurrence AndAlso entry.SourceType = KblObjectType.Wire_occurrence AndAlso entry.ModelType = myModelType).FirstOrDefault()
                If myChangedWire IsNot Nothing Then myChangeType = myChangedWire.ChangeType
            End If

            Dim key As String
            If myChangedWire IsNot Nothing AndAlso Not String.IsNullOrEmpty(myChangedWire.KblIdRef) Then
                myChangeType = myChangedWire.ChangeType

                If Not String.IsNullOrEmpty(myChangedWire.SrcId) Then
                    key = _refMapper.Id.ToString + "|" + myChangedWire.SrcId
                    If _wireId_EntityMapper.ContainsKey(key) Then
                        Dim myObjects As List(Of ChangedObjectEx) = _wireId_EntityMapper.Item(key)
                        Dim myRoutedObjects As List(Of ChangedObjectEx) = myObjects.Where(Function(obj) obj.ChangeType = CompareChangeType.Routed AndAlso obj.ObjType = KblObjectType.Segment AndAlso obj.ModelType = ModelType.reference).ToList
                        For Each item As ChangedObjectEx In myRoutedObjects
                            If item.Entity IsNot Nothing Then
                                If Not _routedRefElements.Contains(item.Entity) Then
                                    _routedRefElements.Add(item.Entity)
                                End If
                            End If
                        Next
                    End If
                End If
            End If

            If myChangedWire IsNot Nothing AndAlso Not String.IsNullOrEmpty(myChangedWire.KblIdComp) Then
                key = _compMapper.Id.ToString + "|" + myChangedWire.SrcId
                If _wireId_EntityMapper.ContainsKey(key) Then
                    Dim myObjects As List(Of ChangedObjectEx) = _wireId_EntityMapper.Item(key)
                    Dim myRoutedObjects As List(Of ChangedObjectEx) = myObjects.Where(Function(obj) obj.ID = myChangedWire.ID AndAlso obj.ChangeType = CompareChangeType.Routed AndAlso obj.ObjType = KblObjectType.Segment AndAlso obj.ModelType = ModelType.compare).ToList
                    For Each item As ChangedObjectEx In myRoutedObjects
                        If item.Entity IsNot Nothing Then
                            If Not _routedCompElements.Contains(item.Entity) Then
                                _routedCompElements.Add(item.Entity)
                                If Design3D.Entities.IndexOf(CType(item.Entity, Entity)) < 0 Then
                                    Design3D.Entities.Add(item.Entity)
                                End If
                            End If
                        End If
                    Next
                End If
            End If

            Return myChangeType
        End Function

        Private Function GetWireId(ids As IEnumerable(Of String), mapperSourceId As String) As String
            Dim wrId As String = ""
            If mapperSourceId = _refMapper.Id Then
                For Each id As String In ids
                    If _refMapper.KBLWireList.Select(Function(obj) obj.SystemId).ToList().Contains(id) Then
                        wrId = id
                        Exit For
                    End If
                Next
            Else
                For Each id As String In ids
                    If _compMapper.KBLWireList.Select(Function(obj) obj.SystemId).ToList().Contains(id) Then
                        wrId = id
                        Exit For
                    End If
                Next
            End If
            Return wrId
        End Function

        Friend Function GetWireIds(ids As IEnumerable(Of String), mapperSourceId As String) As List(Of String)
            Dim wrIds As New List(Of String)
            If mapperSourceId = _refMapper.Id Then
                For Each id As String In ids
                    If _refMapper.KBLWireList.Select(Function(obj) obj.SystemId).ToList().Contains(id) Then
                        wrIds.Add(id)
                    End If
                Next
            Else
                For Each id As String In ids
                    If _compMapper.KBLWireList.Select(Function(obj) obj.SystemId).ToList().Contains(id) Then
                        wrIds.Add(id)
                    End If
                Next
            End If
            Return wrIds
        End Function

        Private Function GetCoreIds(ids As IEnumerable(Of String), mapperSourceId As String) As List(Of String)
            Dim coreIds As New List(Of String)
            If mapperSourceId = _refMapper.Id Then
                For Each id As String In ids
                    If _refMapper.KBLCoreList.Select(Function(obj) obj.SystemId).ToList().Contains(id) Then
                        coreIds.Add(id)
                    End If
                Next
            Else
                For Each id As String In ids
                    If _compMapper.KBLCoreList.Select(Function(obj) obj.SystemId).ToList().Contains(id) Then
                        coreIds.Add(id)
                    End If
                Next
            End If
            Return coreIds
        End Function

        Private Function GetMargin() As Integer
            Dim margin As Integer
            Dim h As Integer = Design3D.ActiveViewport.GetBounds.Height
            Dim w As Integer = Design3D.ActiveViewport.GetBounds.Width

            If GetSelection().Count = 1 AndAlso GetSelection().FirstOrDefault IsNot Nothing AndAlso CType(GetSelection().First(), BaseModelEntity).EntityType = ModelEntityType.Node Then
                If h < w Then
                    'margin = CInt(w / 1.5)
                    margin = CInt(0.5 * h - 30)
                Else
                    'margin = CInt(h / 1.5)
                    margin = CInt(0.5 * w - 30)
                End If
            Else
                If h < w Then
                    'margin = CInt(w / 4)
                    margin = CInt(0.5 * h + h / 2.5)
                Else
                    'margin = CInt(h / 4)
                    margin = CInt(0.5 * w + w / 2.5)
                End If
            End If
            Return margin
        End Function

        Private Function CalculateMargin(ParamArray entities As IBaseModelEntityEx()) As Integer
            Dim margin As Integer = If(entities.Length = 1, 400, 100)
            If entities.Length = 1 Then
                Select Case entities.Single.EntityType
                    Case ModelEntityType.Node, ModelEntityType.Connector, ModelEntityType.Fixing, ModelEntityType.Eylet, ModelEntityType.Splice
                        margin = _defaultMargin
                End Select
            End If
            Return margin
        End Function

        Public Sub AttachEEModel(model As E3.Lib.Model.EESystemModel)
            Me.Design3D.EEModel = model  ' HINT: no auto-entities conversion here (settings set to nothing), because we want to set the entities manually, but we connect the model
        End Sub

        Private Function IsSelectionChanged(objectIds As List(Of String)) As Boolean
            Dim currentSelection As List(Of String) = GetIdsFromSelection()
            Dim res As Boolean = False
            If currentSelection.Count = objectIds.Count Then
                For Each id As String In objectIds
                    If Not currentSelection.Contains(id) Then
                        res = True
                        Exit For
                    End If
                Next
            Else
                res = True
            End If
            Return res
        End Function

        Private Function GetIdsFromSelection() As List(Of String)
            Dim mylist As New List(Of String)
            For Each entity As Entity In GetSelection()
                Dim id As KeyValuePair(Of String, ChangedObjectEx) = _kblId_EntityMapper.Where(Function(en) en.Value.Entity.Id = CType(entity, IBaseModelEntityEx).Id).FirstOrDefault
                If id.Value IsNot Nothing Then
                    mylist.Add(id.Key)
                End If
            Next
            Return mylist
        End Function

        Protected Overridable Sub OnEmptySpaceClicked(e As EventArgs)
            RaiseEvent EmptySpaceClicked(Me, e)
        End Sub

        Protected Overridable Sub OnActionModeChanged(e As ActionModeChangedEventArgs)
            Me._actionMode = e.Action
            If e.Action = actionType.Rotate Then
                If My.Application.MainForm.GeneralSettings.UseSelectionCenterForRotation AndAlso GetSelection().Count > 0 Then
                    SetSelectionCenter(GetSelection().First)
                Else
                    Design3D.ActiveViewport.Rotate.RotationCenter = rotationCenterType.ViewportCenter
                    Design3D.ActiveViewport.Rotate.ShowCenter = False
                End If
            End If
        End Sub

        Private Function GetSelectionCenter(Optional selectedEntity As Entity = Nothing) As devDept.Geometry.Point3D
            If selectedEntity IsNot Nothing Then
                Dim points As Point3D() = selectedEntity.EstimateBoundingBox(Design3D.Blocks, Design3D.Layers)
                Dim mn As New devDept.Geometry.Point3D
                Dim mx As New devDept.Geometry.Point3D
                Utility.BoundingBox(points, mn, mx)
                Dim center As devDept.Geometry.Point3D = GetCenter(mn, mx)
                Return center
            Else
                'Dim center As devDept.Geometry.Point3D = D3D.GetSelectedEntities.GetBoundingBox().GetCenter()
                Dim center As devDept.Geometry.Point3D = GetSelection.GetBoundingBox().GetCenter()
                Return center
            End If
        End Function

        Private Sub SetSelectionCenter(Optional selectedEntity As Entity = Nothing)
            If selectedEntity IsNot Nothing Then
                Dim center As devDept.Geometry.Point3D = GetSelectionCenter(selectedEntity)

                Design3D.ActiveViewport.Rotate.RotationCenter = rotationCenterType.Point
                Design3D.ActiveViewport.Rotate.Center = center
                Design3D.ActiveViewport.Rotate.ShowCenter = True

            Else
                Dim center As devDept.Geometry.Point3D = GetSelectionCenter(selectedEntity)
                Design3D.ActiveViewport.Rotate.RotationCenter = rotationCenterType.Point
                Design3D.ActiveViewport.Rotate.Center = center
            End If
        End Sub

        Public Shared Function GetCenter(min As devDept.Geometry.Point3D, max As devDept.Geometry.Point3D) As devDept.Geometry.Point3D
            Dim mWithX As Double = (max.X - min.X) / 2
            Dim mWithY As Double = (max.Y - min.Y) / 2
            Dim mWithZ As Double = (max.Z - min.Z) / 2

            Return New devDept.Geometry.Point3D(min.X + mWithX, min.Y + mWithY, min.Z + mWithZ)
        End Function

        Private Sub Model3D_AfterActionModeChanged(sender As Object, e As ActionModeEventArgs) Handles Design3D.AfterActionModeChanged
            Dim args As New ActionModeChangedEventArgs(e.Action, e.Button, _actionModeChanging)
            OnActionModeChanged(args)
        End Sub

        Private Sub Model3D_KeyDown(sender As Object, e As KeyEventArgs) Handles Design3D.KeyDown
            If e.KeyCode = Keys.Escape Then
                RaiseEvent SelectionChangedInD3DComparer(Me, New ComparerSelectionChangedEventArgs())
            End If

            If e.Control And e.KeyCode = Keys.S Then
                IsHiddenSelection = True

                Dim myHiddens As New List(Of Entity)
                For Each entity As Entity In GetSelection()
                    myHiddens.Add(entity)
                Next

                For Each entity As Entity In myHiddens
                    _currentEntityFromContext = CType(entity, BaseModelEntity)
                    AddEntityToHiddens(Me, New EventArgs)
                Next

                e.Handled = True
            End If

            If e.Control And e.KeyCode = Keys.E Then
                ResetAllHiddenEntities(Me, New EventArgs)
                e.Handled = True
            End If

            If e.Control And e.KeyCode = Keys.D Then
                e.Handled = True
            End If
        End Sub

        Private Sub Model3D_NeedsEEObjectData(sender As Object, e As EEObjectDataEventArgs) Handles Design3D.EEObjectDataRequested
            OnNeedsEEObjectData(e)
        End Sub

        Protected Overridable Sub OnNeedsEEObjectData(e As EEObjectDataEventArgs)
            RaiseEvent NeedsEEObjectData(Me, e)
        End Sub

        Protected Friend Overridable Sub OnClearSelection(e As EventArgs)
            RaiseEvent ClearSelection(Me, e)
        End Sub

        Private Sub Model3D_LostFocus(sender As Object, e As EventArgs) Handles Design3D.LostFocus
            If Not Design3D.Focused Then
                RaiseEvent ModelLostFocus(Me, e)
            End If
        End Sub

        Public Sub ClearSelectedEntities(e As Workspace.SelectionChangedEventArgs)
            Dim tmpRoutedRef As New List(Of IBaseModelEntityEx)
            Dim tmpRoutedComp As New List(Of IBaseModelEntityEx)

            Dim removed As New List(Of IBaseModelEntityEx)
            For Each en As Workspace.SelectedItem In e.RemovedItems
                If Not removed.Contains(CType(en.Item, IBaseModelEntityEx)) Then
                    removed.Add(CType(en.Item, IBaseModelEntityEx))
                End If
            Next

            For Each en As IBaseModelEntityEx In removed
                If _refSelection.Contains(en) Then
                    en.ResetColor(_colorMapper)
                    SetTransparency(en, GetRefTrans())
                End If

                If _compSelection.Contains(en) Then
                    en.ResetColor(_colorMapper)
                    SetTransparency(en, GetCompTrans())
                End If

                If _routedRefElements.Contains(en) Then
                    en.Selectable = False
                    If Not _myRefEntities.ContainsValue(en) Then
                        en.IsNotActive(_colorMapper)
                        tmpRoutedRef.Add(en)
                    Else
                        en.ResetColor(_colorMapper)
                        en.Selectable = False
                    End If
                End If

                If _routedCompElements.Contains(en) Then
                    en.Selectable = False
                    If Not _myCompEntities.ContainsValue(en) Then
                        en.IsNotActive(_colorMapper)
                        tmpRoutedComp.Add(en)
                    Else
                        en.ResetColor(_colorMapper)
                        en.Selectable = False
                    End If
                End If
            Next

            For Each entity As IBaseModelEntityEx In _tmpEntities
                Design3D.Entities.Remove(entity)
            Next

            _tmpEntities.Clear()

            For Each item As IBaseModelEntityEx In tmpRoutedRef
                item.Selectable = False
                _routedRefElements.Remove(item)
            Next

            For Each item As IBaseModelEntityEx In tmpRoutedComp
                item.Selectable = False
                _routedCompElements.Remove(item)
            Next

            _refSelection.Clear()
            _compSelection.Clear()
            SetSelectability()
            Design3D.Entities.Regen()
            Design3D.Invalidate()
        End Sub

        Public Sub ClearSelectedEntities()
            If Design3D.Entities.Count > 0 Then
                Design3D.Entities.ClearSelection()

                For Each element As IBaseModelEntityEx In _refSelection
                    element.Selected = False
                    element.ResetColor(_colorMapper)
                    SetTransparency(element, GetRefTrans())
                Next

                For Each element As IBaseModelEntityEx In _compSelection
                    element.Selected = False
                    element.ResetColor(_colorMapper)
                    SetTransparency(element, GetCompTrans())
                Next

                For Each element As IBaseModelEntityEx In _routedRefElements
                    element.Selected = False
                    element.ResetColor(_colorMapper)
                    If Not _myRefEntities.ContainsValue(element) Then
                        element.Selectable = False
                        element.IsNotActive(_colorMapper)
                    End If
                Next

                For Each element As IBaseModelEntityEx In _routedCompElements
                    element.Selected = False
                    element.ResetColor(_colorMapper)
                    If Not _myCompEntities.ContainsValue(element) Then
                        element.Selectable = False
                        element.IsNotActive(_colorMapper)
                    End If
                Next

                For Each entity As IBaseModelEntityEx In _tmpEntities
                    Design3D.Entities.Remove(entity)
                Next
                _tmpEntities.Clear()

                _routedRefElements.Clear()
                _routedCompElements.Clear()
                _refSelection.Clear()
                _compSelection.Clear()
                SetSelectability()
                Design3D.UpdateVisibleSelection()
                Design3D.Entities.Regen()
                Design3D.Invalidate()
            End If
        End Sub

        Private Sub D3D_AfterActionModeChanged(sender As Object, e As ActionModeEventArgs) Handles Design3D.AfterActionModeChanged
            Dim args As New ActionModeChangedEventArgs(e.Action, e.Button, _actionModeChanging)
            RaiseEvent ActionModelChanged(Me, args)
        End Sub

        Private Sub D3DCntrl_SizeChanged(sender As Object, e As EventArgs) Handles Me.SizeChanged
            Dim width0 As Integer = Me.Width

            If (width0 > Fader.Width) Then
                Fader.Left = CInt((width0 - Fader.Width) / 2)
            Else
                Fader.Width = width0
                Fader.Left = 0
            End If

            Dim height0 As Integer = Me.Height
            Fader.Top = height0 - Fader.Height
        End Sub

        Private Function EntityIsCompareEntity(e As IEntity) As Boolean
            Dim ids As List(Of String) = _myCompEntities.Select(Function(val) val.Value.Id).ToList()
            Dim id As String = CType(e, IBaseModelEntityEx).Id
            If ids.Contains(id) Then
                Return True
            Else
                Return False
            End If
        End Function

        Private Function EntityIsReferenceEntity(e As IEntity) As Boolean
            Dim ids As List(Of String) = _myRefEntities.Select(Function(val) val.Value.Id).ToList()
            Dim id As String = CType(e, IBaseModelEntityEx).Id
            If ids.Contains(id) Then
                Return True
            Else
                Return False
            End If
        End Function

        Private Function Get3DEntitiesByKblIds(ids As IEnumerable(Of String)) As IEnumerable(Of Entity)
            Dim lst As New HashSet(Of Entity)
            Dim entities As IEnumerable(Of Entity)
            For Each id As String In ids
                entities = _referenceDocument.Entities.GetByKblIds(id).OfType(Of Entity)
                If Not entities.Any() Then
                    entities = _compareDocument.Entities.GetByKblIds(id).OfType(Of Entity)
                End If
                lst.AddRange(entities)
            Next
            Return lst
        End Function

        Private Function GetActiveEntity(location As System.Drawing.Point) As Entity
            Dim myEntity As Entity = Nothing
            Dim pos As New System.Drawing.Point(location.X - 3, location.Y - 3)
            Dim sz As New Size(6, 6)
            Dim rct As New Rectangle(pos, sz)
            Dim indices As Integer() = Design3D.GetAllCrossingEntities(rct, True)
            If indices.Length > 0 Then
                For Each index As Integer In indices
                    Dim myCurrentEntity As Entity = Design3D.Entities.Item(index)
                    Dim myBaseEntity As BaseModelEntity = CType(myCurrentEntity, BaseModelEntity)
                    If Trackbar.Value <= 0 Then
                        If _myRefEntities.ContainsKey(New Guid(myBaseEntity.Id)) Then
                            myEntity = myCurrentEntity
                        End If
                    ElseIf Trackbar.Value > 0 Then
                        If _myCompEntities.ContainsKey(New Guid(myBaseEntity.Id)) Then
                            myEntity = myCurrentEntity
                        End If
                    End If
                Next
            End If
            Return myEntity
        End Function

        Private Function GetSelection() As List(Of Entity)
            Dim myEntities As New List(Of Entity)
            For Each en As Entity In Design3D.Entities
                If en.Selected Then myEntities.Add(en)
            Next
            Return myEntities
        End Function

        Private Function GetSelectionAsIEntity() As List(Of IBaseModelEntityEx)
            Dim myEntities As New List(Of IBaseModelEntityEx)
            For Each en As Entity In Design3D.Entities
                If en.Selected Then myEntities.Add(CType(en, IBaseModelEntityEx))
            Next
            Return myEntities
        End Function

        Private Sub D3D_MouseDown(sender As Object, e As MouseEventArgs) Handles Design3D.MouseDown
            _mouseEventArgs = e
        End Sub

        Private Sub D3DComparerControl_MouseClick(sender As Object, e As MouseEventArgs) Handles Design3D.MouseClick
            If e.Button = MouseButtons.Left AndAlso Not Design3D.ActiveViewport.ToolBar.Contains(e.Location) Then
                Dim entity As Entity = Design3D.GetEntityUnderMouseCursor(e.Location, True)

                If (entity Is Nothing AndAlso Not IsMoved(e)) Then

                    If (_routedRefElements.Count > 0 OrElse _routedCompElements.Count > 0) Then
                        ClearRoutedElements()
                    End If

                    If (GetSelection.Count > 0) Then
                        RaiseEvent SelectionChangedInD3DComparer(Me, New ComparerSelectionChangedEventArgs())
                    End If
                End If

            End If
        End Sub

        Private Sub ClearRoutedElements()
            For Each item As IBaseModelEntityEx In _routedRefElements
                item.IsNotActive(_colorMapper)
                item.Selectable = False
            Next

            For Each item As IBaseModelEntityEx In _routedCompElements
                item.IsNotActive(_colorMapper)
                item.Selectable = False
            Next

            _routedRefElements.Clear()
            _routedCompElements.Clear()
        End Sub

        Private Function IsMoved(e As MouseEventArgs) As Boolean
            If _mouseEventArgs.Location.X <> e.Location.X OrElse _mouseEventArgs.Location.Y <> e.Location.Y Then
                Return True
            Else
                Return False
            End If
        End Function

        Private Function CalculateFitMargin() As Integer
            Return CInt(((Me.Bounds.Width + Me.Bounds.Height) / 2) * 0.05)
        End Function

        Public Sub ZoomfitActiveToSelection()
            If Design3D.Entities IsNot Nothing AndAlso Design3D.Entities.Count > 0 Then
                Dim currentView As Camera = Design3D.ActiveViewport.Camera
                Design3D.ActiveViewport.SaveView(currentView)
                Dim margin As Integer = CalculateMargin2(GetSelectionAsIEntity().ToArray)
                Design3D.ActiveViewport.ZoomFit(True, Camera.perspectiveFitType.Accurate, margin)
            End If
        End Sub

        Private Function SelectionIsInView() As Boolean
            Dim myEq As PlaneEquation() = Design3D.ActiveViewport.GetCameraFrustum()
            Dim myFrustum As New FrustumParams(myEq, Design3D)
            Dim res As Boolean = True
            For Each entity As Entity In GetSelection()
                If Not entity.IsInFrustum(myFrustum) Then
                    res = False
                    Exit For
                End If
            Next
            Return res
        End Function

        Private Function CalculateMargin2(ParamArray entities As IBaseModelEntityEx()) As Integer
            Dim margin As Integer = If(entities.Length = 1, 400, 100)
            If entities.Length = 1 Then
                Select Case entities.Single.EntityType
                    Case ModelEntityType.Node, ModelEntityType.Connector, ModelEntityType.Fixing, ModelEntityType.Eylet, ModelEntityType.Splice
                        margin = _defaultMargin
                End Select
            End If
            Return margin
        End Function

        Private Sub Trackbar_MouseUp(sender As Object, e As MouseEventArgs) Handles Trackbar.MouseUp
            Design3D.Select()
            Design3D.Entities.Regen()
            Design3D.Invalidate()
        End Sub

        Private Function Get3DEntityByKblId(doc As HcvDocument, id As String) As IBaseModelEntityEx
            Dim res() As IBaseModelEntityEx = Nothing
            If doc IsNot Nothing Then
                res = doc.Entities.GetByKblIds(id)
            End If
            Return res.FirstOrDefault
        End Function

        Private Sub D3DComparerControl_EmptySpaceClicked(sender As Object, e As EventArgs) Handles Me.EmptySpaceClicked
            CloseAllToolTips()
        End Sub
        Public Sub CloseAllToolTips(Optional forcePinned As Boolean = False)
            _ttManager.CloseAll(forcePinned, delayed:=False)
        End Sub

        Private Sub ClearAttached()
            For Each doc As DocumentClone In _documents
                doc.Dispose()
            Next
        End Sub

        Private Sub D3D_CameraChanged(sender As Object, e As Workspace.CameraMoveEventArgs) Handles Design3D.CameraChanged
            Design3D.ActiveViewport.Camera.UpdateLocation()
            Design3D.ActiveViewport.Camera.UpdateMatrices()
            'Debug.WriteLine("")
            'Debug.WriteLine("Zoom Speed:" + D3D.Zoom.Speed.ToString)
            ''Debug.WriteLine("Delta:" + e..Delta.ToString)
            'Debug.WriteLine("ZoomFit Margin:" + D3D.Zoom.FitMargin.ToString)
            'Debug.WriteLine("Far:" + D3D.ActiveViewport.Camera.Far.ToString)
            'Debug.WriteLine("Near:" + D3D.ActiveViewport.Camera.Near.ToString)
            'Debug.WriteLine("Zoomfactor:" + D3D.ActiveViewport.Camera.ZoomFactor.ToString)
            'Dim pos As devDept.Geometry.Point3D = D3D.ActiveViewport.Camera.Location
            'Debug.WriteLine("CameraPosition:" + pos.X.ToString + "  " + pos.Y.ToString + "  " + pos.Z.ToString)
            'Debug.WriteLine("CameraDistance:" + D3D.ActiveViewport.Camera.Distance.ToString)

        End Sub

        Private Enum SelectionModus
            reference
            compare
        End Enum

        Public Enum ToolBarButtons
            Home
            MagnifyingGlass
            ZoomWindow
            Zoom
            Pan
            Rotate
            ZoomFit
        End Enum

    End Class

End Namespace