Imports System.ComponentModel
Imports System.Reflection
Imports System.Timers
Imports Infragistics.Win.UltraWinToolbars
Imports Infragistics.Win.UltraWinToolTip
Imports VectorDraw.Geometry
Imports VectorDraw.Professional.Control
Imports VectorDraw.Professional.vdCollections
Imports VectorDraw.Professional.vdFigures
Imports VectorDraw.Professional.vdObjects
Imports VectorDraw.Professional.vdPrimaries
Imports VectorDraw.Professional.vdPrimaries.vdFigure
Imports VectorDraw.Render
Imports Zuken.E3.HarnessAnalyzer.QualityStamping
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.HarnessAnalyzer.Shared.Common
Imports Zuken.E3.Lib.Converter.Svg.Ultra
Imports Zuken.E3.Lib.IO.Files
Imports Zuken.E3.Lib.IO.Files.Hcv

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class DrawingCanvas
    Implements IDrawingCanvas

    Friend Event CanvasMouseClick(sender As Object, e As MouseEventArgs)
    Friend Event CanvasMouseMove(sender As Object, e As MouseEventArgs)
    Friend Event CanvasSelectionChanged(sender As Object)
    Friend Event DrawingDisplayFinished(sender As Object, e As EventArgs) Implements IDrawingCanvas.DrawingDisplayFinished
    Friend Event Message(sender As Object, e As MessageEventArgs)

    Private _activeAnalysisObjects As ICollection(Of String)
    Private _focusLost As Boolean
    Private _fontShapeFilesMissing As Boolean
    Private _generalSettings As GeneralSettingsBase
    Private _gripEntity As vdFigure
    Private _gripIndex As Integer = -1
    Private _highlightPolylines As New List(Of VdPolylineEx)
    Private _highlightRects As List(Of VdRectEx)
    Private _hitEntity As vdFigure
    Private _hitPoint As gPoint
    Private _hoverEntity As vdFigure
    Private _isInMoveMode As Boolean
    Private _issueMapper As New Dictionary(Of String, IList(Of VdSVGGroup))
    Private _messageEventArgs As MessageEventArgs
    Private _panActivated As Boolean
    Private _qmStamps As QMStamps
    Private _redliningInformation As RedliningInformation
    Private _redliningMapper As New Dictionary(Of String, IList(Of VdSVGGroup))
    Private _selection As vdSelection
    Private _stampMapper As New Dictionary(Of String, ICollection(Of VdStamp))
    Private _workState As D3D.D3DWorkState
    Private _groupMapper As New Dictionary(Of String, IDictionary(Of VdSVGGroup, VdSVGGroup))
    Private _progressValue As Long
    Private _magnifierActivated As Boolean
    Private _vertexGroups As New List(Of VdSVGGroup)
    Private _zoomedEx As Boolean = False
    Private _force_drawing_entities_progress As Integer = 0
    Private _displayingDrawing As Boolean
    Private _drawingDisplayed As Boolean
    Private _logEventArgs As LogEventArgs
    Private _initialSvgBoundingBox As Box
    Private _factory As New SvgFactory
    Private _redliningsAndQmStampsHaveBeenUpdatedFromDisplayDrawing As Boolean
    Private _redliningsDirty As New RedliningDirtyInfo(False)
    Private _bbSuspended As Boolean

    Private WithEvents _lazyDrawingEntitiesProgressTimer As New System.Timers.Timer With {.Interval = 500, .AutoReset = False}
    Private _canLazyDrawEntities As Boolean = False

    Private WithEvents _documentForm As DocumentForm
    Private WithEvents _draftConverter As DraftConverter
    Private WithEvents _svgConverter As SVGConverter
    Private WithEvents _tooltipForm As TooltipForm
    Private WithEvents _vdBaseControl As VectorDrawBaseControl
    Private WithEvents _activeDocument As vdDocument

    Public Property DrawingCanvasStatusMessage As String = String.Empty
    Public Property NavigatorImage As Image
    Public Property DrawWideFrame As Boolean = False Implements IDrawingCanvas.DrawWideFrame
    Public Property SvgHeight As Single = 0.0
    Public Property TxtHeight As Double = 1.0
    Public Property IsGroupMapperValid As Boolean

    Public ReadOnly Property PanActivated As Boolean
        Get
            Return _panActivated
        End Get
    End Property

    Public ReadOnly Property MagnifierActivated As Boolean
        Get
            Return _magnifierActivated
        End Get
    End Property

    Public Sub New(documentForm As DocumentForm, qmStamps As QMStamps, redliningInformation As RedliningInformation)
        InitializeComponent()
        Me.Dock = DockStyle.Fill
        Me.vdCanvas.Dock = DockStyle.None
        Me.vdCanvas.Height = 100
        Me.vdCanvas.Width = 100
        _documentForm = documentForm
        _generalSettings = documentForm.GeneralSettings
        _messageEventArgs = New MessageEventArgs(MessageEventArgs.MsgType.ShowProgressAndMessage)
        _qmStamps = qmStamps
        _redliningInformation = redliningInformation
        _tooltipForm = New TooltipForm(documentForm.KBL, documentForm.KBL.KBLCavityContactPointMapper, documentForm.KBL.KBLContactPointWireMapper, documentForm.KBL.KBLCoreCableMapper, documentForm.KBL.KBLObjectModuleMapper, documentForm.KBL.KBLOccurrenceMapper, documentForm.KBL.KBLPartMapper, documentForm.KBL.KBLUnitMapper)
        _vdBaseControl = Me.vdCanvas.BaseControl

        InitBaseControl()

        Me.BackColor = Color.White
        Me.vdCanvas.SetShowLayoutTab(False)

        Me.vdCanvas.Dock = DockStyle.Fill
        Me.vdCanvas.Visible = False
    End Sub

    Public Sub InitBaseControl()
        _selection = New vdSelection()

        With _vdBaseControl
            .AllowDrop = False
            .EnsureDocument()
            .BackColor = Color.White
            With .ActiveDocument
                .Selections.Add(_selection)
                .ShowUCSAxis = False
                .UndoHistory.PushEnable(False)
                .DisableZoomOnResize = True
                .EnableAutoGripOn = False
                .EnableToolTips = False
                .EnableUrls = False
                .Background = Color.White
                .Palette.Background = Color.White
                .RenderMode = vdRender.Mode.Wire2d
                .Limits = New Box(New gPoint(-E3.Lib.Converter.Svg.Defaults.VDRAW_LIMIT, -E3.Lib.Converter.Svg.Defaults.VDRAW_LIMIT), New gPoint(E3.Lib.Converter.Svg.Defaults.VDRAW_LIMIT, E3.Lib.Converter.Svg.Defaults.VDRAW_LIMIT))
                .OrbitActionKey = VectorDraw.Professional.vdObjects.vdDocument.KeyWithMouseStroke.None
                .OsnapDialogKey = Keys.None

                .ProxyClasses.Add(GetType(VdSVGGroup))
                .UrlActionKey = Keys.None
                .ViewSize = 1000.0


                With .GlobalRenderProperties
                    If (TryCast(_documentForm.MdiParent, MainForm)?.GeneralSettings.ShowFullScreenAxisCursor) Then
                        .AxisSize = E3.Lib.Converter.Svg.Defaults.VDRAW_LIMIT
                    Else
                        .AxisSize = 10
                    End If
                    .TimerBreakForDraw = 80
                    .PanUserFlag = vdRenderGlobalProperties.PanUserFlags.ScrollDC ' HINT: this setting makes scrolling smoother on some graphics card drivers: see vd-article-source: https://vdraw.com/articles/wish/70002274-perform-smoother-pan/ -> was tested and makes the scrolling on non-remote also faster!
                    .CrossSize = 8
                    .GridColor = Color.Black
                    .SelectingCrossColor = Color.Transparent
                    .SelectingWindowColor = Color.Transparent
                    .OpenGlAntializing = 8
                    .RenderingQuality = VectorDraw.Render.vdRender.RenderingQualityMode.HighQuality
                    .StartUpActionCursor = Cursors.Default
                End With
            End With
        End With

        _activeDocument = _vdBaseControl.ActiveDocument
        _activeDocument.MeterProgress.RaiseOnPaint = True
        DisableDrawEvents()
        _vdBaseControl.ActiveDocument.DisableRedraw = True
        VectorDraw.Render.vdRenderGlobalProperties.MinLineTypeSegmentLen = VectorDraw.Render.vdRenderGlobalProperties.MinLineTypeSegmentLen / 100
    End Sub

    Public Sub DisableDrawEvents()
        DisableDrawEvents(_vdBaseControl.ActiveDocument)
    End Sub

    Friend Shared Sub DisableDrawEvents(document As vdDocument)
        If document.FreezeEntityDrawEvents.Count > 0 Then
            document.FreezeEntityDrawEvents.Pop()
        End If
        document.FreezeEntityDrawEvents.Push(True)
    End Sub

    Public Sub EnableDrawEvents()
        EnableDrawEvents(_vdBaseControl.ActiveDocument)
    End Sub

    Public Sub EnableOverrideProgressDrawingEntities()
        Threading.Interlocked.Increment(_force_drawing_entities_progress)
    End Sub

    Public Sub ResumeDefaultProgressDrawingEntities()
        Threading.Interlocked.Decrement(_force_drawing_entities_progress)
    End Sub

    Friend Shared Sub EnableDrawEvents(document As vdDocument)
        If document.FreezeEntityDrawEvents.Count > 0 Then
            document.FreezeEntityDrawEvents.Pop()
        End If
        document.FreezeEntityDrawEvents.Push(False)
    End Sub

    ''' <summary>
    ''' Displays the entities when drawing is not displayed
    ''' </summary>
    ''' <param name="forceDisplay"></param>
    Friend Sub DisplayDrawing(Optional forceDisplay As Boolean = False, Optional zoomExtends As Boolean = True)
        If (Not _displayingDrawing AndAlso Not _drawingDisplayed) OrElse forceDisplay Then
            _displayingDrawing = True

            _vdBaseControl.ActiveDocument.DisableRedraw = False

            Me.vdCanvas.Visible = True

            DisableDrawEvents()
            If zoomExtends Then
                TryZoomExtendsFirstTimeOnly()
            End If

            vdCanvas.BaseControl.ActiveDocument.Redraw(False)

            DrawingCanvasStatusMessage = String.Empty
            TryUpdateQmStampsAndRedlinings()
        End If
    End Sub

    Private Function TryUpdateQmStampsAndRedlinings() As Boolean
        If Not _bbSuspended Then
            'HINT: redlinings can only be generated when the bounding box is not suspended (while loading, f.e.) -> or the pencil-entities will result in an near null-size entitiy
            QMStampsChanged(False)
            UpdateRedlinings(False)
            _redliningsAndQmStampsHaveBeenUpdatedFromDisplayDrawing = True
            Return True
        End If
        Return False
    End Function

    Friend Function TryZoomExtendsFirstTimeOnly() As Boolean
        If Not _zoomedEx Then
            vdCanvas.BaseControl.ActiveDocument.ZoomExtents()
            _zoomedEx = True
            Return True
        End If
        Return False
    End Function

    Private Function CheckGroupMapperForTopology() As Boolean
        For Each seg As Segment In _documentForm.KBL.GetSegments
            If Not GroupMapper.ContainsKey(seg.SystemId) Then
                Return False
            End If
        Next

        For Each nd As Node In _documentForm.KBL.GetNodes
            If Not GroupMapper.ContainsKey(nd.SystemId) Then
                Return False
            End If
        Next

        Return True
    End Function

    Friend Function CalculateScaleFac4Magnifier() As Double
        Dim screenHeight As Integer = Screen.FromControl(_vdBaseControl).Bounds.Height
        Dim s2 As Double
        Dim scale As Double = 1.0
        If screenHeight > 0 AndAlso TxtHeight > 0 Then
            s2 = 1080 / screenHeight
            scale = s2 / (TxtHeight * 600)
        End If
        Return scale
    End Function

    Friend Function FilterActiveObjects(row_filter As RowFilterInfo) As Boolean
        Dim changed As Boolean = False

        If DeselectAll() Then
            changed = True
        End If

        If UpdateGroupVisibility_Recursively(_vdBaseControl.ActiveDocument.ActiveLayOut.Entities, VisibilityEnum.Visible, True) Then
            changed = True
        End If

        UpdateGroupLighting_Recursively(_vdBaseControl.ActiveDocument.ActiveLayOut.Entities, Lighting.Normal, changed)

        For Each issueGroup As VdSVGGroup In _issueMapper.SelectMany(Function(kv) kv.Value)
            If issueGroup.SetVisibility(VisibilityEnum.Visible) Then
                changed = True
            End If
            If issueGroup.SetLighting(Lighting.Normal) Then
                changed = True
            End If
        Next

        For Each redliningGroup As VdSVGGroup In _redliningMapper.SelectMany(Function(kv) kv.Value)
            If redliningGroup.SetVisibility(VisibilityEnum.Visible) Then
                changed = True
            End If
            If redliningGroup.SetLighting(Lighting.Normal) Then
                changed = True
            End If
        Next

        For Each stampGroup As VdStamp In _stampMapper.SelectMany(Function(kv) kv.Value)
            If stampGroup.SetVisibility(VisibilityEnum.Visible) Then
                changed = True
            End If

            If stampGroup.SetLighting(Lighting.Normal) Then
                changed = True
            End If
        Next

        Dim inactiveGroups As VdSVGGroup() = row_filter.KBL.InactiveModules.Values.SelectMany(Function(m) _groupMapper.GetValueOrEmpty(m.SystemId).Values).ToArray
        For Each group As VdSVGGroup In inactiveGroups
            If group.SetVisibility(VisibilityEnum.Invisible) Then
                changed = True
            End If
            If UpdateGroupVisibility_Recursively(group.ChildGroups, VisibilityEnum.Invisible, True) Then
                changed = True
            End If
        Next

        Dim inactiveKblIds As String() = row_filter.InactiveObjects.Values.SelectMany(Function(id) id).ToArray
        Dim cnCounter As Integer = _documentForm.KBL.GetConnectorOccurrences.Count
        Dim sgCounter As Integer = _documentForm.KBL.GetSegments.Count
        Dim vxCounter As Integer = _documentForm.KBL.GetNodes.Count
        Dim nObjects As Integer = inactiveKblIds.Length + cnCounter + sgCounter + vxCounter

        Dim inactiveObjectCounter As Integer = 1
        Dim prevPercent As Integer = 0

        _messageEventArgs = New MessageEventArgs(MessageEventArgs.MsgType.ShowProgressAndMessage, 0, DocumentFormStrings.ApplyModConfig_LogMsg)
        For Each kblId_inactive As String In inactiveKblIds
            Dim newPercent As Integer = CInt(inactiveObjectCounter * 100 / nObjects)
            If newPercent > prevPercent Then
                _messageEventArgs.ProgressPercentage = newPercent
                RaiseEvent Message(Me, _messageEventArgs)
                prevPercent = newPercent
            End If

            For Each group As VdSVGGroup In _groupMapper.GetValueOrEmpty(kblId_inactive).Values
                If group.SetVisibility(VisibilityEnum.Invisible) Then
                    changed = True
                End If
                UpdateGroupVisibility_Recursively(group.ChildGroups, VisibilityEnum.Invisible, True)
            Next

            If (_redliningMapper.ContainsKey(kblId_inactive)) Then
                For Each group As VdSVGGroup In _redliningMapper(kblId_inactive)
                    If group.SetVisibility(VisibilityEnum.Invisible) Then
                        changed = True
                    End If
                Next

                For Each group As VdSVGGroup In _vdBaseControl.ActiveDocument.Model.Entities
                    If (group.Layer.Name = VDRAW_LAYER_REDLININGS_BACKGROUND_NAME) AndAlso (group.KblId = kblId_inactive) Then
                        If group.SetVisibility(VisibilityEnum.Invisible) Then
                            changed = True
                        End If
                    End If
                Next
            End If

            For Each stamp As QMStamp In _qmStamps.Stamps.Where(Function(s) s.ObjectReferences IsNot Nothing AndAlso s.ObjectReferences.FindObjRefFromKblId(kblId_inactive) IsNot Nothing)
                For Each group As VdSVGGroup In _stampMapper.GetValueOrEmpty(stamp.Id)
                    If group.SetVisibility(vdFigure.VisibilityEnum.Invisible) Then
                        changed = True
                    End If
                Next
            Next

            inactiveObjectCounter += 1
        Next

        If UpdateGreyOutStateOfConnectors(row_filter.InactiveObjects, inactiveKblIds.Length + 1, nObjects) Then
            changed = True
        End If

        If UpdateGreyOutStateOfSegments(row_filter.InactiveObjects, inactiveKblIds.Length + cnCounter + 1, nObjects) Then
            changed = True
        End If

        If UpdateGreyOutStateOfVertices(row_filter.InactiveObjects, inactiveKblIds.Length + cnCounter + sgCounter + 1, nObjects) Then
            changed = True
        End If

        If _documentForm.MainForm IsNot Nothing AndAlso ControlVisiblityOfEntitiesWithoutModules(_documentForm.MainForm.NoModulesVisible) Then
            changed = True
        End If

        If _activeAnalysisObjects IsNot Nothing Then
            FilterAnalysisViewObjects(_activeAnalysisObjects, False, changed)
        End If

        If changed Then
            _vdBaseControl.ActiveDocument.Model.Entities.Update(False)
            _vdBaseControl.ActiveDocument.Invalidate()
        End If

        Return changed
    End Function

    Public Function ControlVisiblityOfEntitiesWithoutModules(visible As Boolean) As Boolean
        Dim changed As Boolean
        If Not visible Then
            For Each grp As VdSVGGroup In Me.vdCanvas.BaseControl.ActiveDocument.Model.Entities.OfType(Of VdSVGGroup)
                If Not grp.IsControlled AndAlso grp.SymbolType <> SvgSymbolType.DocumentFrame.ToString AndAlso grp.SymbolType <> SvgSymbolType.QMStamp.ToString AndAlso grp.SymbolType <> SvgSymbolType.Redlining.ToString AndAlso (grp.SVGType <> SvgType.table.ToString OrElse Not String.IsNullOrEmpty(grp.KblId)) Then
                    If grp.SetVisibility(VisibilityEnum.Invisible) Then
                        changed = True
                    End If
                End If
            Next
        End If
        Return changed
    End Function

    Friend Function FilterAnalysisViewObjects(activeObjects As ICollection(Of String), Optional invalidate As Boolean = True, Optional ByRef changed As Boolean = False) As String()
        Dim filteredKblIds As New HashSet(Of String)
        _activeAnalysisObjects = activeObjects

        If _activeAnalysisObjects Is Nothing Then
            _vdBaseControl.ActiveDocument.Palette.Background = Color.White
            UpdateGroupLighting_Recursively(_vdBaseControl.ActiveDocument.ActiveLayOut.Entities, Lighting.Normal, changed)
        Else
            _vdBaseControl.ActiveDocument.Palette.Background = Color.FromArgb(255, 240, 226)
            filteredKblIds.AddRange(UpdateGroupLighting_Recursively(_vdBaseControl.ActiveDocument.ActiveLayOut.Entities, Lighting.Lowlight))

            For Each activeObject As String In _activeAnalysisObjects
                If _groupMapper.ContainsKey(activeObject) Then
                    For Each group As VdSVGGroup In _groupMapper(activeObject).Values
                        If group.SetLighting(Lighting.Normal) Then
                            changed = True
                        End If

                        filteredKblIds.Remove(group.KblId)

                        For Each kblId As String In UpdateGroupLighting_Recursively(group.ChildGroups, Lighting.Normal, changed)
                            filteredKblIds.Remove(kblId)
                        Next
                    Next
                End If

                For Each group As VdSVGGroup In _issueMapper.GetValueOrEmpty(activeObject)
                    If group.SetLighting(Lighting.Normal) Then
                        changed = True
                    End If
                    filteredKblIds.Remove(group.KblId)
                Next

                For Each group As VdSVGGroup In _redliningMapper.GetValueOrEmpty(activeObject)
                    If group.SetLighting(Lighting.Normal) Then
                        changed = True
                    End If
                    filteredKblIds.Remove(group.KblId)
                Next
            Next
        End If

        If _activeAnalysisObjects IsNot Nothing AndAlso changed Then
            _vdBaseControl.ActiveDocument.ActiveLayOut.ZoomExtents()
        End If

        If changed AndAlso invalidate Then
            _vdBaseControl.ActiveDocument.Invalidate()
        End If

        Return filteredKblIds.ToArray
    End Function

    Friend Function GetRedliningAndStampScaleFactor() As Double
        Dim documentFrameBoundingBox As Box = If(_groupMapper.ContainsKey(SvgSymbolType.DocumentFrame.ToString), GetDocumentFrameBoundingBox(_groupMapper(SvgSymbolType.DocumentFrame.ToString).Values.ToList), Nothing)
        If documentFrameBoundingBox IsNot Nothing Then
            Return (documentFrameBoundingBox.Height / [Shared].DEFAULT_DOC_HEIGHT) * _generalSettings.RedliningStampIndicatorScaleFactor
        Else
            Return _generalSettings.RedliningStampIndicatorScaleFactor
        End If
    End Function

    Friend Sub InformationHubSelectionChanged(kblIds As IEnumerable(Of String), objType As KblObjectType, Optional forceDeselect As Boolean = True, Optional issueIds As IEnumerable(Of String) = Nothing, Optional stampIds As IEnumerable(Of String) = Nothing)
        If _selection IsNot Nothing Then
            Dim connectorId As String = Nothing

            If forceDeselect Then
                DeselectAll()
            End If

            If kblIds.Any() Then
                Dim firstKblId As String = kblIds.First
                Dim lastKblId As String = kblIds.Last

                If objType = E3.Lib.Schema.Kbl.KblObjectType.Cavity_occurrence Then
                    Dim wireCore As IKblWireCoreOccurrence = _documentForm.KBL.GetWireOrCoreOccurrence(lastKblId) 'TODO: this BL is very unsafe, what when the order in the kbl-id list changes somehow ? Dangerous (this is blind) coding...
                    If wireCore IsNot Nothing Then
                        connectorId = _documentForm.KBL.GetConnectorOfCavity(firstKblId)?.SystemId 'TODO: see above... the same (firstKblId) ... this is coding on the adrenalin-side of life. Like jumping blind into a hole an hoping it's not so deep
                    End If
                End If

                For Each kblId As String In kblIds
                    If Not String.IsNullOrWhiteSpace(kblId) Then
                        If _groupMapper.ContainsKey(kblId) OrElse _documentForm.KBL.GetHarness.SystemId = kblId Then
                            AddGroupToSelection(kblId, True, True, rootGroupKblId:=connectorId, issueIds:=issueIds, stampIds:=stampIds)
                            BringSelectedObjectsIntoView()
                        ElseIf objType = E3.Lib.Schema.Kbl.KblObjectType.Accessory Then
                            'HINT FORD special Accessories should highlight referenced object in Draft mode only
                            Dim accessoryOccurrence As Accessory_occurrence = _documentForm.KBL.GetOccurrenceObject(Of Accessory_occurrence)(kblId)
                            If accessoryOccurrence IsNot Nothing Then
                                For Each refId As String In HarnessAnalyzer.[Shared].Utilities.GetIdrefs(accessoryOccurrence.Reference_element)
                                    If _documentForm.KBL.GetOccurrenceObjectUntyped(refId) IsNot Nothing Then
                                        AddGroupToSelection(refId, True, True, connectorId, issueIds:=issueIds, stampIds:=stampIds)
                                        BringSelectedObjectsIntoView()
                                    End If
                                Next
                            End If
                        End If
                    End If
                Next

                For Each kblObj As [Lib].Schema.Kbl.IKblBaseObject In _documentForm.KBL.GetObjectsOfModule(firstKblId) ' TODO: another adrenalin-like-coding: why is this alway's the correct index? why not taking sometimes the second or the last or the 111's-index? who knows what is going on here and where is the correct position to take from? Where is this information stored what has to be done?! There are sometimes multiple of the same ids here, sometimes different, and what kbl-id's are this, and why? Questions over questions... Has anybody ever thought when implementing this about what CAN (!!!!!!) HAPPEN IN THE FUTURE WHITH THIS STUFF?? Or only what happens in this moment... ? Yeah, it only must function now, i don't care if it must be changed somewhere in the future... good attitude, please use more OBJECTS to classify/wrap informations what is this stuff all about, this would be already enough for the future coders to handle this stuff whitout needing to understand everything from scratch
                    If Not String.IsNullOrEmpty(kblObj?.SystemId) AndAlso (_groupMapper.ContainsKey(kblObj.SystemId)) Then
                        AddGroupToSelection(kblObj.SystemId, True)
                        BringSelectedObjectsIntoView()
                    End If
                Next
            End If
        End If
    End Sub

    Friend Sub LoadAdditionalDrawing(svgFile As Hcv.SvgContainerFile, Optional drawingFile As String = "", Optional doNotMirror As Boolean = False)
        _fontShapeFilesMissing = False

        If TypeOf svgFile Is Hcv.DraftContainerFile Then
            _draftConverter = New DraftConverter(DirectCast(_documentForm.MdiParent, MainForm).FontAndShapeFilesDir, DirectCast(_documentForm.MdiParent, MainForm).GeneralSettings.ShowFullScreenAxisCursor, _documentForm.KBL, _vdBaseControl, drawingFile)
            _draftConverter.ConvertDraftDrawing(doNotMirror)

            InitializeGroupMapper_Recursively(_vdBaseControl.ActiveDocument.ActiveLayOut.Entities)
        Else
            LoadDrawing(svgFile)
        End If
    End Sub

    Friend Function LoadSingleDrawing(svgFile As Hcv.SvgContainerFile, ByRef workState As D3D.D3DWorkState) As Result
        _fontShapeFilesMissing = False
        _workState = workState

        Return LoadDrawing(svgFile)
    End Function

    Friend Sub QMStampsChanged(removePreviousSelection As Boolean)
        If _vdBaseControl.ActiveDocument Is Nothing Then
            Exit Sub
        End If

        If (removePreviousSelection) Then
            DeselectAll()
        End If

        Dim selectedStamps As New List(Of String)

        For Each drawingObjectQMStamps As KeyValuePair(Of String, ICollection(Of VdStamp)) In _stampMapper
            Dim qmStampsForDeletion As New List(Of VdStamp)

            For Each qmStampGroup As VdStamp In drawingObjectQMStamps.Value
                qmStampsForDeletion.Add(qmStampGroup)
            Next

            For Each qmStampGroup As VdStamp In qmStampsForDeletion
                If (_selection.FindItem(qmStampGroup)) AndAlso (Not selectedStamps.Contains(drawingObjectQMStamps.Key)) Then
                    selectedStamps.Add(drawingObjectQMStamps.Key)

                    _selection.RemoveItem(qmStampGroup)
                End If

                _vdBaseControl.ActiveDocument.Model.Entities.RemoveItem(qmStampGroup)
            Next
        Next

        _stampMapper.Clear()

        For Each stamp As QMStamp In _qmStamps.Stamps
            If (Not _stampMapper.ContainsKey(stamp.Id)) Then
                _stampMapper.Add(stamp.Id, New List(Of VdStamp))
            End If

            For Each objectReference As ObjectReference In stamp.ObjectReferences
                If (_groupMapper.ContainsKey(objectReference.KblId)) OrElse (objectReference.ObjectType = E3.Lib.Schema.Kbl.KblObjectType.Harness) Then
                    Dim boundingBox As Box = Nothing
                    Dim svgGroupObjects As New List(Of VdSVGGroup)
                    Dim visibility As VisibilityEnum

                    If (_groupMapper.ContainsKey(objectReference.KblId)) Then
                        svgGroupObjects = _groupMapper(objectReference.KblId).Values.ToList
                    End If

                    objectReference.vdSVGRefObjects.Clear()

                    Using boundsSelection As New vdSelection
                        For Each group As VdSVGGroup In svgGroupObjects
                            Select Case objectReference.ObjectType
                                Case E3.Lib.Schema.Kbl.KblObjectType.Accessory_occurrence, E3.Lib.Schema.Kbl.KblObjectType.Component_occurrence
                                    If (group.SymbolType = SvgSymbolType.Accessory.ToString) AndAlso (group.SVGType <> SvgType.dimension.ToString) Then
                                        boundsSelection.AddItem(GetRootGroup_Recursively(group), True, vdSelection.AddItemCheck.Nochecking)
                                    End If
                                Case E3.Lib.Schema.Kbl.KblObjectType.Connector_occurrence
                                    If (group.SymbolType = SvgSymbolType.Connector.ToString) AndAlso (group.SVGType = SvgType.Undefined.ToString) Then
                                        boundsSelection.AddItem(GetRootGroup_Recursively(group), True, vdSelection.AddItemCheck.Nochecking)
                                    End If
                                Case E3.Lib.Schema.Kbl.KblObjectType.Fixing_occurrence
                                    If (group.SymbolType = SvgSymbolType.Fixing.ToString) AndAlso (group.SVGType <> SvgType.dimension.ToString) Then
                                        boundsSelection.AddItem(GetRootGroup_Recursively(group), True, vdSelection.AddItemCheck.Nochecking)
                                    End If
                                Case E3.Lib.Schema.Kbl.KblObjectType.Wire_protection_occurrence, KblObjectType.Specified_wire_protection_occurrence
                                    If (group.SymbolType = SvgSymbolType.Taping.ToString) AndAlso (group.SVGType = SvgType.table.ToString) Then
                                        boundsSelection.AddItem(group, True, vdSelection.AddItemCheck.Nochecking)
                                    End If
                                Case E3.Lib.Schema.Kbl.KblObjectType.Segment
                                    If (group.SymbolType = SvgSymbolType.Segment.ToString) Then
                                        boundsSelection.AddItem(group, True, vdSelection.AddItemCheck.Nochecking)
                                    End If
                                Case E3.Lib.Schema.Kbl.KblObjectType.Node
                                    If (group.SymbolType = SvgSymbolType.Vertex.ToString) AndAlso (group.SVGType <> SvgType.dimension.ToString) Then
                                        boundsSelection.AddItem(group, True, vdSelection.AddItemCheck.Nochecking)
                                    End If
                                Case E3.Lib.Schema.Kbl.KblObjectType.Harness, KblObjectType.Undefined
                                    boundsSelection.AddItem(group, True, vdSelection.AddItemCheck.Nochecking)
                            End Select
                        Next

                        If (boundsSelection.Count <> 0) Then
                            boundingBox = boundsSelection.GetBoundingBox
                            visibility = boundsSelection(0).visibility
                            objectReference.vdSVGRefObjects.AddRange(boundsSelection.OfType(Of VdSVGGroup))
                        End If
                    End Using

                    If (boundingBox IsNot Nothing) OrElse (objectReference.ObjectType = E3.Lib.Schema.Kbl.KblObjectType.Harness) Then
                        If (_vdBaseControl.ActiveDocument.Layers.FindName(VDRAW_LAYER_QMSTAMPS_NAME) Is Nothing) Then
                            Dim layer As New vdLayer
                            With layer
                                .SetUnRegisterDocument(_vdBaseControl.ActiveDocument)
                                .setDocumentDefaults()

                                .Name = VDRAW_LAYER_QMSTAMPS_NAME
                            End With

                            _vdBaseControl.ActiveDocument.Layers.Add(layer)
                        End If

                        Dim qmStampGroup As VdStamp = DrawQMStamp(objectReference, visibility)

                        _stampMapper(stamp.Id).Add(qmStampGroup)

                        _vdBaseControl.ActiveDocument.Model.Entities.AddItem(qmStampGroup)

                        If (selectedStamps.Contains(stamp.Id)) Then
                            Dim stampIds As New List(Of String)
                            stampIds.Add(stamp.Id)

                            AddGroupToSelection(objectReference.KblId, False, stampIds:=stampIds)
                        End If
                    End If
                End If
            Next
        Next

        _vdBaseControl.ActiveDocument.Invalidate()
    End Sub

    Friend Sub OnRedliningsChanged(removePreviousSelection As Boolean)
        If Me._documentForm.ActiveDrawingCanvas Is Me Then
            UpdateRedlinings(removePreviousSelection)
        Else
            _redliningsDirty.Dirty = True
            _redliningsDirty.RemovePreviousSelection = removePreviousSelection
        End If
    End Sub

    Friend Sub UpdateRedlinings(removePreviousSelection As Boolean)
        Try
            If (_vdBaseControl.ActiveDocument Is Nothing) Then
                Exit Sub
            End If

            If removePreviousSelection Then
                DeselectAll()
            End If

            For Each drawingObjectRedlinings As KeyValuePair(Of String, IList(Of VdSVGGroup)) In _redliningMapper
                Dim redliningGroupsForDeletion As New List(Of VdSVGGroup)

                For Each redliningGroup As VdSVGGroup In drawingObjectRedlinings.Value
                    redliningGroupsForDeletion.Add(redliningGroup)
                Next

                For Each redliningGroup As VdSVGGroup In _vdBaseControl.ActiveDocument.Model.Entities
                    If (redliningGroup.Layer.Name = VDRAW_LAYER_REDLININGS_BACKGROUND_NAME) AndAlso (redliningGroup.KblId = drawingObjectRedlinings.Key) Then
                        redliningGroupsForDeletion.Add(redliningGroup)
                    End If
                Next

                For Each redliningGroup As VdSVGGroup In redliningGroupsForDeletion
                    _vdBaseControl.ActiveDocument.Model.Entities.RemoveItem(redliningGroup)
                Next
            Next

            _redliningMapper.Clear()

            Dim drawingObjectRedliningMapper As New Dictionary(Of String, List(Of Redlining))
            For Each redlining As Redlining In _redliningInformation.Redlinings
                If redlining.IsVisible AndAlso _groupMapper.ContainsKey(redlining.ObjectId) Then
                    If Not drawingObjectRedliningMapper.ContainsKey(redlining.ObjectId) Then
                        FillDrawingObjectRedliningMapper(drawingObjectRedliningMapper, redlining.ObjectId)
                    End If
                ElseIf (redlining.IsVisible) AndAlso (redlining.ObjectType = E3.Lib.Schema.Kbl.KblObjectType.Wire_protection_occurrence) AndAlso (_groupMapper.ContainsKey(redlining.ObjectName)) Then
                    If Not drawingObjectRedliningMapper.ContainsKey(redlining.ObjectName) Then
                        FillDrawingObjectRedliningMapper(drawingObjectRedliningMapper, redlining.ObjectName)
                    End If
                End If
            Next

            For Each drawingObjectRedlinings As KeyValuePair(Of String, List(Of Redlining)) In drawingObjectRedliningMapper
                Dim boundingBox As Box = Nothing
                Dim offsetPoint As New gPoint(0, 0)
                Dim visibility As VisibilityEnum

                _redliningMapper.Add(drawingObjectRedlinings.Key, New List(Of VdSVGGroup))

                Using boundsSelection As New vdSelection
                    If drawingObjectRedlinings.Value.Count > 0 Then
                        Dim first_redlining As Redlining = drawingObjectRedlinings.Value.FirstOrDefault
                        If first_redlining IsNot Nothing Then
                            For Each group As IGrouping(Of String, VdSVGGroup) In _groupMapper(drawingObjectRedlinings.Key).Values.GroupBy(Function(vd_grp) vd_grp.SymbolType)
                                Dim symbol_type As SvgSymbolType = group.Key.TryParse(Of SvgSymbolType).GetValueOrDefault
                                For Each vdsvgGroup As VdSVGGroup In group
                                    Dim svg_type As SvgType = vdsvgGroup.SVGType.TryParse(Of SvgType).GetValueOrDefault
                                    Select Case first_redlining.ObjectType
                                        Case E3.Lib.Schema.Kbl.KblObjectType.Accessory_occurrence, E3.Lib.Schema.Kbl.KblObjectType.Component_occurrence
                                            If (symbol_type = SvgSymbolType.Accessory) AndAlso (svg_type <> SvgType.dimension) Then
                                                boundsSelection.AddItem(GetRootGroup_Recursively(vdsvgGroup), True, vdSelection.AddItemCheck.Nochecking)
                                            End If
                                        Case E3.Lib.Schema.Kbl.KblObjectType.Connector_occurrence
                                            If (symbol_type = SvgSymbolType.Connector) AndAlso (svg_type = SvgType.Undefined) Then
                                                boundsSelection.AddItem(GetRootGroup_Recursively(vdsvgGroup), True, vdSelection.AddItemCheck.Nochecking)
                                            End If
                                        Case E3.Lib.Schema.Kbl.KblObjectType.Dimension_specification
                                            If (symbol_type = SvgSymbolType.Undefined) AndAlso (svg_type = SvgType.dimension) Then
                                                boundsSelection.AddItem(vdsvgGroup, True, vdSelection.AddItemCheck.Nochecking)
                                            End If
                                        Case E3.Lib.Schema.Kbl.KblObjectType.Fixing_occurrence
                                            If (symbol_type = SvgSymbolType.Fixing) AndAlso (svg_type = SvgType.Undefined) Then
                                                boundsSelection.AddItem(GetRootGroup_Recursively(vdsvgGroup), True, vdSelection.AddItemCheck.Nochecking)
                                            End If
                                        Case E3.Lib.Schema.Kbl.KblObjectType.Specified_wire_protection_occurrence, KblObjectType.Wire_protection_occurrence
                                            If (symbol_type = SvgSymbolType.Taping) AndAlso (svg_type = SvgType.table) Then
                                                boundsSelection.AddItem(vdsvgGroup, True, vdSelection.AddItemCheck.Nochecking)
                                            End If
                                        Case E3.Lib.Schema.Kbl.KblObjectType.Segment
                                            If (symbol_type = SvgSymbolType.Segment) Then
                                                boundsSelection.AddItem(vdsvgGroup, True, vdSelection.AddItemCheck.Nochecking)
                                            End If
                                        Case E3.Lib.Schema.Kbl.KblObjectType.Node
                                            If (symbol_type = SvgSymbolType.Vertex) AndAlso (svg_type <> SvgType.dimension) Then
                                                boundsSelection.AddItem(vdsvgGroup, True, vdSelection.AddItemCheck.Nochecking)
                                            End If
                                    End Select
                                Next
                            Next
                        End If
                    End If

                    If (boundsSelection.Count > 0) Then
                        boundingBox = boundsSelection.GetBoundingBox
                        visibility = boundsSelection(0).visibility
                    End If
                End Using

                If boundingBox IsNot Nothing Then
                    If (_vdBaseControl.ActiveDocument.Layers.FindName(VDRAW_LAYER_REDLININGS_NAME) Is Nothing) Then
                        Dim layer As New vdLayer
                        With layer
                            .SetUnRegisterDocument(_vdBaseControl.ActiveDocument)
                            .setDocumentDefaults()
                            .Name = VDRAW_LAYER_REDLININGS_NAME
                        End With

                        _vdBaseControl.ActiveDocument.Layers.Add(layer)
                    End If

                    For Each redlining As Redlining In drawingObjectRedlinings.Value
                        If (redlining.IsVisible) Then
                            If (redlining.Classification = RedliningClassification.GraphicalComment) Then
                                Dim vDraw As New VectorDrawBaseControl
                                vDraw.EnsureDocument()

                                Dim documentLoadedSuccessfully As Boolean
                                Try
                                    Dim documentStream As New IO.MemoryStream(Convert.FromBase64String(redlining.Comment))
                                    documentStream.Position = 0
                                    documentLoadedSuccessfully = vDraw.ActiveDocument.LoadFromMemory(documentStream, True)
                                    documentStream.Close()
                                Catch ex As Exception
                                    documentLoadedSuccessfully = False
                                End Try

                                If documentLoadedSuccessfully Then
                                    Dim redliningGroup As VdSVGGroup = DrawGraphicalRedliningBackgroundBox(vDraw.ActiveDocument.ActiveLayOut.Entities, drawingObjectRedlinings.Key, redlining, visibility)
                                    _vdBaseControl.ActiveDocument.Model.Entities.AddItem(redliningGroup)
                                    redliningGroup = DrawGraphicalRedlining(vDraw.ActiveDocument.ActiveLayOut.Entities, drawingObjectRedlinings.Key, redlining, visibility)
                                    _redliningMapper(drawingObjectRedlinings.Key).Add(redliningGroup)
                                    _vdBaseControl.ActiveDocument.Model.Entities.AddItem(redliningGroup)
                                End If

                                vDraw.Dispose()
                            Else
                                Dim redliningGroup As VdSVGGroup = DrawDefaultRedlining(boundingBox, drawingObjectRedlinings.Key, offsetPoint, redlining, visibility)
                                _redliningMapper(drawingObjectRedlinings.Key).Add(redliningGroup)
                                _vdBaseControl.ActiveDocument.Model.Entities.AddItem(redliningGroup)
                                offsetPoint = offsetPoint + New gPoint(1.5, -1)
                            End If
                        End If
                    Next
                End If
            Next

            _vdBaseControl.ActiveDocument.Invalidate()
        Finally
            _redliningsDirty.Reset()
        End Try
    End Sub

    Private Sub AddGroupToSelection(kblId As String, triggeredByInformationHub As Boolean, Optional allowGettingRowFromCellGroup As Boolean = True, Optional rootGroupKblId As String = Nothing, Optional issueIds As IEnumerable(Of String) = Nothing, Optional stampIds As IEnumerable(Of String) = Nothing)
        If (_groupMapper.ContainsKey(kblId)) Then
            For Each group As VdSVGGroup In _groupMapper(kblId).Values.ToList
                If (group.KblId = kblId AndAlso group.SymbolType <> SvgSymbolType.DocumentFrame.ToString AndAlso group.SVGType <> SvgType.ref.ToString) OrElse (_groupMapper(kblId).Count = 1 AndAlso group.KblId <> kblId AndAlso group.SecondaryKblIds.Contains(kblId)) OrElse (group.SVGType = SvgType.row.ToString AndAlso group.KblId <> kblId AndAlso group.SecondaryKblIds.Contains(kblId)) Then
                    If (group.SVGType = SvgType.cell.ToString) AndAlso (allowGettingRowFromCellGroup) Then
                        group = group.ParentGroup
                    End If

                    If (rootGroupKblId IsNot Nothing) Then
                        Dim rootGroup As VdSVGGroup = GetRootGroup_Recursively(group)
                        If (rootGroup.KblId <> rootGroupKblId) Then
                            Continue For
                        End If
                    End If

                    Dim newLighting As Nullable(Of Lighting) = Nothing

                    If (Not _selection.FindItem(group)) AndAlso (group.Lighting = Lighting.Normal OrElse triggeredByInformationHub) Then
                        If (group.Lighting = Lighting.Lowlight) Then
                            group.Label = Lighting.Lowlight.ToString
                        End If
                        newLighting = Lighting.Highlight

                        _selection.AddItem(group, True, vdSelection.AddItemCheck.Nochecking)
                    ElseIf (E3.Lib.DotNet.Expansions.Devices.My.Computer.Keyboard.CtrlKeyDown) AndAlso (_selection.FindItem(group)) AndAlso (Not triggeredByInformationHub) Then
                        If Not String.IsNullOrEmpty(group.Label) Then
                            newLighting = Lighting.Lowlight
                        Else
                            newLighting = Lighting.Normal
                        End If

                        group.Label = String.Empty
                        _selection.RemoveItem(group)
                    End If

                    If newLighting.HasValue AndAlso group.SetLighting(newLighting.Value) Then
                        group.Update()
                        group.Invalidate()
                    End If
                End If
            Next
        End If

        Dim isIssueMap As Boolean = _issueMapper.ContainsKey(kblId)
        Dim isRedliningMap As Boolean = _redliningMapper.ContainsKey(kblId)
        Dim isStampMap As Boolean = _qmStamps.Stamps.Contains(kblId)

        If (isIssueMap OrElse isRedliningMap OrElse isStampMap) Then
            Dim selectObjects As New List(Of VdSVGGroup)

            If (isIssueMap) Then
                If (issueIds Is Nothing) Then
                    selectObjects.AddRange(_issueMapper(kblId))
                Else
                    selectObjects.AddRange(_issueMapper(kblId).OfType(Of IssueReporting.VdIssue).Join(issueIds, Function(vdIssue) vdIssue.Issue.Id, Function(id) id, Function(vdIssue, id) vdIssue).Cast(Of VdSVGGroup).ToList)
                End If
            End If

            If (isRedliningMap) Then
                selectObjects.AddRange(_redliningMapper(kblId))
            End If

            If (isStampMap) Then
                If (stampIds Is Nothing) Then
                    For Each stamp As QMStamp In _qmStamps.Stamps.GetAllByKblId(kblId)
                        If (_stampMapper.ContainsKey(stamp.Id)) Then
                            selectObjects.AddRange(_stampMapper(stamp.Id))
                        End If
                    Next
                Else
                    For Each stamp As QMStamp In _qmStamps.Stamps.GetAllByKblId(kblId).Join(stampIds, Function(s) s.Id, Function(id) id, Function(s, id) s)
                        If (_stampMapper.ContainsKey(stamp.Id)) Then
                            selectObjects.AddRange(_stampMapper(stamp.Id))
                        End If
                    Next
                End If
            End If

            For Each group As VdSVGGroup In selectObjects.Distinct
                group.Document.Model.Entities.ChangeOrder(group, False)
                Dim changed As Boolean = False

                If (Not _selection.FindItem(group)) AndAlso (group.Lighting = Lighting.Normal OrElse triggeredByInformationHub) Then
                    If (group.Lighting = Lighting.Lowlight) Then
                        group.Label = Lighting.Lowlight.ToString
                    End If
                    changed = group.SetLighting(Lighting.Highlight)

                    _selection.AddItem(group, True, vdSelection.AddItemCheck.Nochecking)

                    If (TypeOf group Is VdStamp) Then
                        CType(group, VdStamp).ShowGrips = True
                    End If
                ElseIf (E3.Lib.DotNet.Expansions.Devices.My.Computer.Keyboard.CtrlKeyDown) AndAlso (_selection.FindItem(group)) Then
                    If (group.Label <> String.Empty) Then
                        changed = group.SetLighting(Lighting.Lowlight)
                    Else
                        changed = group.SetLighting(Lighting.Normal)
                    End If

                    group.Label = String.Empty

                    If (TypeOf group Is VdStamp) Then
                        CType(group, VdStamp).ShowGrips = False
                    End If

                    _selection.RemoveItem(group)
                End If

                group.Update()
                group.Invalidate()
            Next
        End If
    End Sub

    Private Sub BringSelectedObjectsIntoView()
        If (_selection?.Count).GetValueOrDefault > 0 Then
            Using zoomSelection As New vdSelection
                For Each group As VdSVGGroup In _selection
                    Dim rootGroup As VdSVGGroup = GetRootGroup_Recursively(group)

                    If (Not zoomSelection.FindItem(rootGroup)) AndAlso (rootGroup.visibility = VisibilityEnum.Visible) Then
                        zoomSelection.AddItem(rootGroup, True, vdSelection.AddItemCheck.Nochecking)
                    End If
                Next

                If zoomSelection.Count > 0 Then
                    Dim boundingBox As Box = zoomSelection.GetBoundingBox

                    EnableDrawEvents()
                    Me.DrawingCanvasStatusMessage = DrawingCanvasStrings.RefreshSelection
                    _vdBaseControl.ActiveDocument.ActiveLayOut.ZoomWindow(New gPoint(boundingBox.Min.x - 10, boundingBox.Min.y - 10), New gPoint(boundingBox.Max.x + 10, boundingBox.Max.y + 10))
                    _vdBaseControl.ActiveDocument.Invalidate()
                End If
            End Using
        End If
    End Sub

    Private Function DeselectAll() As Boolean
        Dim changed As Boolean = False
        If (_selection?.Count).GetValueOrDefault > 0 Then
            DrawingCanvasStatusMessage = DrawingCanvasStrings.DeselectAll
            For Each group As VdSVGGroup In _selection.ToArray
                Dim newLighting As Lighting = Lighting.Normal
                If (group.Label.IsNotNullOrEmpty) Then
                    newLighting = Lighting.Lowlight
                End If

                If (TypeOf group Is VdStamp) Then
                    CType(group, VdStamp).ShowGrips = False
                    CType(group, VdStamp).Update()
                    changed = True
                End If

                If group.SetLighting(newLighting) Then
                    group.Lighting = newLighting
                    group.Label = String.Empty
                    group.Update()              '               HINT: update/invalidate is needed or deselect will not be shown instantly
                    group.Invalidate()
                    _selection.RemoveItem(group)
                    changed = True
                End If
            Next
        End If
        Return changed
    End Function

    Private Function DisplayTooltip() As Boolean
        Dim group As VdSVGGroup = TryCast(_hoverEntity, VdSVGGroup)

        If (group IsNot Nothing) AndAlso (_documentForm.KBL.KBLOccurrenceMapper.ContainsKey(group.KblId)) Then
            If (TypeOf _documentForm.KBL.KBLOccurrenceMapper(group.KblId) Is Node) Then
                Dim node As Node = DirectCast(_documentForm.KBL.KBLOccurrenceMapper(group.KblId), Node)
                If (node.Referenced_components IsNot Nothing) AndAlso (node.Referenced_components <> String.Empty) Then
                    For Each referenceId As String In node.Referenced_components.SplitSpace
                        If (_documentForm.KBL.KBLOccurrenceMapper.ContainsKey(referenceId)) AndAlso (TypeOf _documentForm.KBL.KBLOccurrenceMapper(referenceId) Is Connector_occurrence) Then
                            Return True
                        End If
                    Next
                End If
            ElseIf (TypeOf _documentForm.KBL.KBLOccurrenceMapper(group.KblId) Is Connector_occurrence) OrElse (TypeOf _documentForm.KBL.KBLOccurrenceMapper(group.KblId) Is Segment AndAlso DirectCast(_documentForm.KBL.KBLOccurrenceMapper(group.KblId), Segment).Protection_area.Length > 0) Then
                Return True
            End If
        End If

        Return False
    End Function

    Private Function DrawDefaultRedlining(boundingBox As Box, kblId As String, offsetPoint As gPoint, redlining As Redlining, visibility As VisibilityEnum) As VdSVGGroup
        Dim redliningGroup As New VdSVGGroup
        With redliningGroup
            .SetUnRegisterDocument(_vdBaseControl.ActiveDocument)
            .setDocumentDefaults()

            .KblId = kblId
            .Layer = _vdBaseControl.ActiveDocument.Layers.FindName(VDRAW_LAYER_REDLININGS_NAME)
            .SecondaryKblIds.Add(redlining.ID)
            .SymbolType = SvgSymbolType.Redlining.ToString
            .SVGType = SvgType.Undefined.ToString
            .visibility = visibility
            .XProperties.Add("Tooltip").PropValue = GetRedliningTooltip(redlining, True)
        End With

        Dim basePoint As gPoint = boundingBox.Max
        Dim scaleFactor As Double = GetRedliningAndStampScaleFactor()

        If (redlining.ObjectType = E3.Lib.Schema.Kbl.KblObjectType.Segment) Then
            basePoint = boundingBox.MidPoint
        End If

        Dim polyline As New VdPolylineEx(_vdBaseControl.ActiveDocument)
        Dim fillcolor As vdColor = Nothing
        With polyline
            Select Case redlining.Classification
                Case RedliningClassification.Confirmation
                    fillcolor = SvgColor.Green
                Case RedliningClassification.Error
                    fillcolor = SvgColor.Red
                Case RedliningClassification.Information
                    fillcolor = SvgColor.Yellow
                Case RedliningClassification.LengthComment
                    fillcolor = SvgColor.Purple
                Case RedliningClassification.Question
                    fillcolor = SvgColor.Blue
            End Select

            .HatchProperties = _factory.GetHatchproperties(_vdBaseControl.ActiveDocument, fillcolor)
            .LineType = _vdBaseControl.ActiveDocument.LineTypes.Solid
            .PenWidth = CSng(0.5 * scaleFactor)

            .VertexList.Add(basePoint + offsetPoint + New gPoint(2.5 * scaleFactor, 2.5 * scaleFactor))
            .VertexList.Add(.VertexList.Last + New gPoint(2.5 * scaleFactor, 5 * scaleFactor))
            .VertexList.Add(.VertexList.Last + New gPoint(10 * scaleFactor, 10 * scaleFactor))
            .VertexList.Add(.VertexList.Last + New gPoint(2.5 * scaleFactor, -2.5 * scaleFactor))
            .VertexList.Add(.VertexList.Last + New gPoint(-10 * scaleFactor, -10 * scaleFactor))
            .VertexList.Add(.VertexList.Last + New gPoint(-5 * scaleFactor, -2.5 * scaleFactor))
        End With

        redliningGroup.AddFigure(polyline)

        polyline = New VdPolylineEx(_vdBaseControl.ActiveDocument)
        With polyline
            .HatchProperties = _factory.GetHatchproperties(_vdBaseControl.ActiveDocument, SvgColor.Black)
            .LineType = _vdBaseControl.ActiveDocument.LineTypes.Solid
            .VertexList.Add(basePoint + offsetPoint + New gPoint(2.5 * scaleFactor, 2.5 * scaleFactor))
            .VertexList.Add(.VertexList.Last + New gPoint(0.8333 * scaleFactor, 1.6666 * scaleFactor))
            .VertexList.Add(.VertexList.Last + New gPoint(0.85 * scaleFactor, -0.85 * scaleFactor))
            .VertexList.Add(.VertexList(0))
        End With

        redliningGroup.AddFigure(polyline)

        polyline = New VdPolylineEx
        With polyline
            .PenWidth = CSng(0.25 * scaleFactor)
            .LineType = _vdBaseControl.ActiveDocument.LineTypes.Solid
            .VertexList.Add(basePoint + offsetPoint + New gPoint(2.5 * scaleFactor, 2.5 * scaleFactor))
            .VertexList.Add(.VertexList.Last + New gPoint(2.5 * scaleFactor, 5 * scaleFactor))
            .VertexList.Add(.VertexList.Last + New gPoint(1.25 * scaleFactor, 0))
            .VertexList.Add(.VertexList.Last + New gPoint(0, -1.25 * scaleFactor))
            .VertexList.Add(.VertexList.Last + New gPoint(1.25 * scaleFactor, 0))
            .VertexList.Add(.VertexList.Last + New gPoint(0, -1.25 * scaleFactor))
            .VertexList.Add(.VertexList(0))
        End With

        redliningGroup.AddFigure(polyline)

        polyline = New VdPolylineEx
        With polyline
            .HatchProperties = _factory.GetHatchproperties(_vdBaseControl.ActiveDocument, SvgColor.Black)
            .LineType = _vdBaseControl.ActiveDocument.LineTypes.Solid
            .PenWidth = CSng(0.25 * scaleFactor)

            .VertexList.Add(basePoint + offsetPoint + New gPoint(2.5 * scaleFactor, 2.5 * scaleFactor))
            .VertexList.Add(.VertexList.Last + New gPoint(2.5 * scaleFactor, 5 * scaleFactor))
            .VertexList.Add(.VertexList.Last + New gPoint(10 * scaleFactor, 10 * scaleFactor))
            .VertexList.Add(.VertexList.Last + New gPoint(2.5 * scaleFactor, -2.5 * scaleFactor))
            .VertexList.Add(.VertexList.Last + New gPoint(-1 * scaleFactor, -1 * scaleFactor))
            .VertexList.Add(.VertexList.Last + New gPoint(-2.5 * scaleFactor, 2.5 * scaleFactor))
            .VertexList.Add(.VertexList(1))
        End With

        redliningGroup.AddFigure(polyline)

        Return redliningGroup
    End Function

    Private Function DrawGraphicalRedlining(entities As vdEntities, kblId As String, redlining As Redlining, visibility As VisibilityEnum) As VdSVGGroup
        Dim redliningGroup As New VdSVGGroup
        Dim doc As vdDocument = _vdBaseControl.ActiveDocument
        With redliningGroup
            .SetUnRegisterDocument(doc)
            .setDocumentDefaults()

            .KblId = kblId
            .Layer = _vdBaseControl.ActiveDocument.Layers.FindName(VDRAW_LAYER_REDLININGS_NAME)
            .SecondaryKblIds.Add(redlining.ID)
            .SymbolType = SvgSymbolType.Redlining.ToString
            .SVGType = SvgType.Undefined.ToString
            .visibility = visibility
            .XProperties.Add("Tooltip").PropValue = GetRedliningTooltip(redlining, False)
        End With

        For Each figure As vdFigure In entities
            If (figure.Layer.Name = "0") Then
                redliningGroup.AddFigure(CType(figure.Clone(doc), vdFigure))
            End If
        Next

        Return redliningGroup
    End Function

    Private Function DrawGraphicalRedliningBackgroundBox(entities As vdEntities, kblId As String, redlining As Redlining, visibility As VisibilityEnum) As VdSVGGroup
        If (_vdBaseControl.ActiveDocument.Layers.FindName(VDRAW_LAYER_REDLININGS_BACKGROUND_NAME) Is Nothing) Then
            Dim layer As New vdLayer
            With layer
                .SetUnRegisterDocument(_vdBaseControl.ActiveDocument)
                .setDocumentDefaults()

                .Lock = True
                .Name = VDRAW_LAYER_REDLININGS_BACKGROUND_NAME
            End With

            _vdBaseControl.ActiveDocument.Layers.InsertAt(0, layer)
        End If

        Dim redliningGroup As New VdSVGGroup
        With redliningGroup
            .SetUnRegisterDocument(_vdBaseControl.ActiveDocument)
            .setDocumentDefaults()

            .KblId = kblId
            .Layer = _vdBaseControl.ActiveDocument.Layers.FindName(VDRAW_LAYER_REDLININGS_BACKGROUND_NAME)
            .SecondaryKblIds.Add(redlining.ID)
            .SymbolType = SvgSymbolType.Redlining.ToString
            .SVGType = SvgType.Undefined.ToString
            .visibility = visibility
        End With

        Using boundsSelection As New vdSelection
            For Each figure As vdFigure In entities
                If (figure.Layer.Name = "Background") Then
                    boundsSelection.AddItem(figure, True, vdSelection.AddItemCheck.Nochecking)
                End If
            Next

            Dim rect As New vdRect(_activeDocument)
            With rect
                .HatchProperties = _factory.GetHatchproperties(_activeDocument, New vdColor(Color.LightGray, 48))
                .InsertionPoint.x = CSng(boundsSelection.GetBoundingBox.Min.x)
                .InsertionPoint.y = CSng(boundsSelection.GetBoundingBox.Min.y)
                .Height = CSng(boundsSelection.GetBoundingBox.Height)
                .PenColor = New vdColor(Color.LightGray, 48)
                .Width = CSng(boundsSelection.GetBoundingBox.Width)
            End With

            redliningGroup.AddFigure(rect)
        End Using

        Return redliningGroup
    End Function

    Private Function DrawQMStamp(objectReference As ObjectReference, visibility As VisibilityEnum) As VdStamp
        Dim qmStampGroup As New VdStamp
        Dim scaleFactor As Double = GetRedliningAndStampScaleFactor()

        With qmStampGroup
            .SetUnRegisterDocument(_vdBaseControl.ActiveDocument)
            .setDocumentDefaults()

            .Layer = _vdBaseControl.ActiveDocument.Layers.FindName(VDRAW_LAYER_QMSTAMPS_NAME)
            .OwningCanvas = Me
            .Reference = objectReference
            .Scales.x = scaleFactor
            .Scales.y = scaleFactor
            .SecondaryKblIds.Add(objectReference.KblId)
            .SVGType = SvgType.Undefined.ToString
            .SymbolType = SvgSymbolType.QMStamp.ToString
            .Text = objectReference.List.Owner.RefNo.ToString
            .visibility = visibility
        End With

        Return qmStampGroup
    End Function

    Private Sub FillDrawingObjectRedliningMapper(drawingObjectRedliningMapper As Dictionary(Of String, List(Of Redlining)), objectKey As String)
        Dim list As List(Of Redlining) = drawingObjectRedliningMapper.GetOrAddNew(objectKey)

        'HINT: add objectKeyId-RL once per classification 
        For Each rl_grp As IGrouping(Of RedliningClassification, Redlining) In _redliningInformation.Redlinings.GroupBy(Function(rl) rl.Classification)
            Dim redlining As Redlining = rl_grp.Where(Function(rl) rl.ObjectId = objectKey).FirstOrDefault
            If redlining IsNot Nothing Then
                list.Add(redlining)
            End If
        Next
    End Sub

    Private Function GetDocumentFrameBoundingBox(groups As List(Of VdSVGGroup)) As Box
        Using boundsSelection As New vdSelection
            For Each group As VdSVGGroup In _groupMapper(SvgSymbolType.DocumentFrame.ToString).Values.ToList
                boundsSelection.AddItem(GetRootGroup_Recursively(group), True, vdSelection.AddItemCheck.Nochecking)
                Exit For
            Next

            If (boundsSelection.Count > 0) Then
                Return boundsSelection.GetBoundingBox
            End If
        End Using

        Return Nothing
    End Function

    Private Function GetRedliningTooltip(redlining As Redlining, showComment As Boolean) As String
        Dim tooltip As String = String.Format(DrawingCanvasStrings.RedliningTooltip1, [Lib].Schema.Kbl.Utils.GetLocalizedName(redlining.ObjectType), redlining.ObjectName, vbCrLf)
        tooltip = String.Format("{0}--------------------{1}", tooltip, vbCrLf)
        tooltip = String.Format(DrawingCanvasStrings.RedliningTooltip2, tooltip, redlining.GetLocalizedClassificationString, vbCrLf)
        If (showComment) Then
            tooltip = String.Format(DrawingCanvasStrings.RedliningTooltip3, tooltip, redlining.Comment, vbCrLf)
        End If
        tooltip = String.Format(DrawingCanvasStrings.RedliningTooltip4, tooltip, If(redlining.OnGroup IsNot Nothing AndAlso redlining.OnGroup <> String.Empty, _redliningInformation.RedliningGroups.Single(Function(rdliningGrp) rdliningGrp.ID = redlining.OnGroup).ChangeTag, "-"))

        Return tooltip
    End Function

    Private Function GetRootGroup_Recursively(group As VdSVGGroup) As VdSVGGroup
        If (group.ParentGroup Is Nothing) Then
            Return group
        Else
            Return GetRootGroup_Recursively(group.ParentGroup)
        End If
    End Function

    Private Sub InitializeGroupMapper_Recursively(ByVal figures As vdEntities)
        For Each figure As vdFigure In figures
            Dim group As VdSVGGroup = TryCast(figure, VdSVGGroup)
            If (group IsNot Nothing) Then
                If (Not _groupMapper.ContainsKey(group.KblId)) Then
                    Dim groups As New Dictionary(Of VdSVGGroup, VdSVGGroup)
                    groups.Add(group, group)

                    _groupMapper.Add(group.KblId, groups)
                ElseIf (Not _groupMapper(group.KblId).ContainsKey(group)) Then
                    _groupMapper(group.KblId).Add(group, group)
                End If

                For Each secondaryKblId As String In group.SecondaryKblIds
                    If (Not _groupMapper.ContainsKey(secondaryKblId)) Then
                        Dim groups As New Dictionary(Of VdSVGGroup, VdSVGGroup)
                        groups.Add(group, group)

                        _groupMapper.Add(secondaryKblId, groups)
                    ElseIf (Not _groupMapper(secondaryKblId).ContainsKey(group)) Then
                        _groupMapper(secondaryKblId).Add(group, group)
                    End If
                Next

                InitializeGroupMapper_Recursively(group.ChildGroups)
            End If
        Next
    End Sub

    Public ReadOnly Property HasKroschuSvg As Boolean

    Private Function LoadDrawing(svgFile As Hcv.SvgContainerFile) As Result
        _logEventArgs = New LogEventArgs
        _messageEventArgs = New MessageEventArgs(MessageEventArgs.MsgType.ShowProgressAndMessage)

        Using s As System.IO.Stream = svgFile.GetDataAsStream
            _svgConverter = New SVGConverter(_documentForm.KBL, s, svgFile.FullName, _vdBaseControl, CType(svgFile, IBaseTopologyFile).IsKroschu)
            Using _svgConverter
                _svgConverter.CalculateBoundingBox = True
                Dim Result As Result = _svgConverter.ConvertSVGDrawing(CType(_workState, System.Threading.CancellationToken))
                If result.IsFaultedOrCancelled Then
                    Return result
                End If

                _HasKroschuSvg = CType(svgFile, IBaseTopologyFile).IsKroschu
                _initialSvgBoundingBox = New Box(_svgConverter.BoundingBox)
            End Using 'free converter resources because they are not needed anymore
        End Using

        SuspendBoundingBoxCalculation(_initialSvgBoundingBox)

        Try
            InitializeGroupMapper_Recursively(_vdBaseControl.ActiveDocument.ActiveLayOut.Entities)

            Dim entities As VdSVGGroup() = _vdBaseControl.ActiveDocument.ActiveLayOut.Entities.OfType(Of VdSVGGroup).ToArray
            For Each svgGrp As VdSVGGroup In entities
                svgGrp.IsControlled = Me._documentForm.KBL.KBLObjectModuleMapper.ContainsKey(svgGrp.KblId)
                If (Not svgGrp.IsControlled) Then
                    If (svgGrp.SymbolType = SvgSymbolType.Segment.ToString) Then
                        If (Me._documentForm.KBL.KBLSegmentControl.ContainsKey(svgGrp.KblId)) Then
                            svgGrp.IsControlled = CBool(Me._documentForm.KBL.KBLSegmentControl(svgGrp.KblId) <> ControlState.NotCtrld)
                        End If
                    ElseIf (svgGrp.SymbolType = SvgSymbolType.Vertex.ToString) Then
                        If (Me._documentForm.KBL.KBLNodeControl.ContainsKey(svgGrp.KblId)) Then
                            svgGrp.IsControlled = CBool(Me._documentForm.KBL.KBLNodeControl(svgGrp.KblId) <> ControlState.NotCtrld)
                        End If
                    End If
                End If
            Next
        Catch ex As Exception
            Return New Result(ex)
        End Try

        Return Result.Success
    End Function

    Private Sub SuspendBoundingBoxCalculation(useBox As Box)
        ' HINT: Write the complete document BB to the last entity only, to avoid re-calculation, but also ensure that the vectorDraw-methods calling the GetBoundingBox-method's are getting the whole BB. The aim of this HACK is to avoid the re-calc on each single entity and thus get a greater performance boost (this only works correctly as long as the document remains static => f.e. while loading is on-going and the user can't change anything) -> compare this BL with some way of BeginUpdate- and EndUpdate-logic for BoundingBox-calc. Begin = set override | End = remove/reset override back to normal (remark: reset = call override-method with <nothing> parameter)
        ' HINT2: if you don't understand this special HACK here at all, you can safely remove BOTH (suspend and resume!) implementations which only will decrease performance (depending on the svg size), but shoudn't have impact on the behaviour at all (in conclusion: there should be always two hack-implementations with this BL)

        Dim last As IVdEntity = Nothing
        For Each entity As IVdEntity In _vdBaseControl.ActiveDocument.ActiveLayOut.Entities.OfType(Of IVdEntity)
            entity.OverrideBoundingBox(Box.EmptyBox)
            last = entity
        Next

        If last IsNot Nothing Then
            CType(last, IVdEntity).OverrideBoundingBox(useBox)
            SetDrawBreakTime(400)
            _bbSuspended = True
        End If
    End Sub

    Friend Function ResumeBoundingBoxCalculation() As Boolean
        If _bbSuspended Then
            _bbSuspended = False
            'HINT: for an overall explanation look at SuspendBoundingBoxCalculation 
            'HINT2: if you don't understand this special HACK here at all, you can safely remove BOTH (suspend and resume!) implementations which only will decrease performance (depending on the svg size), but shoudn't have impact on the behaviour at all (in conclusion: there should be always two hack-implementations with this BL)

            For Each entity As IVdEntity In vdCanvas.BaseControl.ActiveDocument.Model.Entities.OfType(Of IVdEntity)
                entity.OverrideBoundingBox(Nothing)
            Next
            SetDrawBreakTime(200)
            Return True
        End If
        Return False
    End Function

    Private Function MoveDetect() As Boolean
        Return Math.Abs(_hitPoint.Distance2D(_vdBaseControl.ActiveDocument.CCS_CursorPos())) > 1.0
    End Function

    Private Sub OnTimerTick(sender As Object, e As EventArgs)
        If _vdBaseControl.ActiveDocument IsNot Nothing Then
            _vdBaseControl.ActiveDocument.ZoomExtents()
            _vdBaseControl.ActiveDocument.Invalidate()
        End If
    End Sub

    Private Sub SetObjectTypeDependentSelection()
        Dim deletableGroups As New List(Of VdSVGGroup)

        With _documentForm._informationHub.utcInformationHub.ActiveTab
            If .Key = TabNames.tabHarness.ToString Then
                For Each kblId As String In _groupMapper.Keys
                    AddGroupToSelection(kblId, False)
                Next
            Else
                For Each group As VdSVGGroup In _selection
                    If (.Key = TabNames.tabVertices.ToString AndAlso group.SymbolType <> SvgSymbolType.Vertex.ToString) OrElse
                        (.Key = TabNames.tabSegments.ToString AndAlso group.SymbolType <> SvgSymbolType.Segment.ToString AndAlso group.SymbolType <> SvgSymbolType.Taping.ToString) OrElse
                        (.Key = TabNames.tabAccessories.ToString AndAlso group.SymbolType <> SvgSymbolType.Accessory.ToString) OrElse
                        (.Key = TabNames.tabFixings.ToString AndAlso group.SymbolType <> SvgSymbolType.Fixing.ToString) OrElse
                        (.Key = TabNames.tabComponents.ToString AndAlso group.SymbolType <> SvgSymbolType.Connector.ToString) OrElse
                        (.Key = TabNames.tabConnectors.ToString AndAlso group.SymbolType <> SvgSymbolType.Cavity.ToString AndAlso group.SymbolType <> SvgSymbolType.Connector.ToString AndAlso (group.SymbolType <> SvgSymbolType.Vertex.ToString OrElse group.SecondaryKblIds.Count = 0) AndAlso group.SymbolType <> SvgSymbolType.Wire.ToString) OrElse
                        ((.Key = TabNames.tabCables.ToString OrElse .Key = TabNames.tabWires.ToString OrElse .Key = TabNames.tabNets.ToString) AndAlso group.SymbolType <> SvgSymbolType.Segment.ToString AndAlso group.SymbolType <> SvgSymbolType.Wire.ToString) OrElse
                        (.Key = TabNames.tabRedlinings.ToString AndAlso group.SymbolType <> SvgSymbolType.Redlining.ToString) Then

                        Dim newLighting As Lighting = Lighting.Normal
                        If (group.Label <> String.Empty) Then
                            group.Lighting = Lighting.Lowlight
                        End If

                        group.Label = String.Empty
                        deletableGroups.Add(group)

                        If group.SetLighting(newLighting) Then
                            group.Update()
                            group.Invalidate()
                        End If
                    End If
                Next
            End If
        End With

        For Each group As VdSVGGroup In deletableGroups
            _selection.RemoveItem(group)
        Next
    End Sub

    Private Sub StartMoveGripEntity()
        _isInMoveMode = True

        Dim hitPoint As gPoint = _hitPoint
        If (hitPoint Is Nothing) Then
            hitPoint = CType(_gripEntity, vdShape).Origin
        End If

        If _gripIndex = 0 Then
            QualityStamping.VdMoveStampAction.CmdMoveStamp(CType(_gripEntity, QualityStamping.VdStamp))
        ElseIf _gripIndex = 1 Then
            QualityStamping.VdRotateStampAction.CmdRotateStamp(CType(_gripEntity, QualityStamping.VdStamp))
        End If

        _documentForm.SetDirty()
        _hitPoint = Nothing
    End Sub

    Private Function UpdateActiveRowsInConnectorGroup_Recursively(activeParts As ICollection(Of String), groups As vdEntities) As Boolean
        Dim changed As Boolean = False
        For Each group As VdSVGGroup In groups
            If UpdateActiveRowsInConnectorGroup_Recursively(activeParts, group.ChildGroups) Then
                changed = True
            End If

            Dim newLighting As Nullable(Of Lighting) = Nothing
            If String.IsNullOrWhiteSpace(group.KblId) Then
                If group.ChildGroups.Count = 0 Then
                    newLighting = Lighting.Lowlight
                End If
            Else
                newLighting = If(activeParts.Contains(group.KblId) OrElse group.SecondaryKblIds.Any(Function(id) activeParts.Contains(id)), Lighting.Normal, Lighting.Lowlight)
            End If

            If newLighting.HasValue AndAlso newLighting <> group.Lighting Then
                group.Lighting = newLighting.Value
                changed = True
            End If
        Next
        Return changed
    End Function

    Private Function UpdateGreyOutStateOfConnector(activeParts As ICollection(Of String), connector As Connector_occurrence, inactiveObjects As ITypeGroupedKblIds, inactiveParts As ICollection(Of String)) As Boolean
        Dim changed As Boolean = False
        With _documentForm.KBL
            If inactiveObjects.GetValueOrEmpty(KblObjectType.Connector_occurrence).Contains(connector.SystemId) AndAlso (activeParts.Count > 0) Then
                For Each group As VdSVGGroup In _groupMapper(connector.SystemId).Values.ToList
                    Dim changed_grp As Boolean = False
                    If group.SetVisibility(VisibilityEnum.Visible) Then
                        changed_grp = True
                    End If

                    If UpdateGroupVisibility_Recursively(group.ChildGroups, VisibilityEnum.Visible, True) Then
                        changed_grp = True
                    End If

                    If group.SVGType = SvgType.table.ToString Then
                        If UpdateActiveRowsInConnectorGroup_Recursively(activeParts, group.ChildGroups) Then
                            changed_grp = True
                        End If
                    ElseIf group.SetLighting(Lighting.Lowlight) Then
                        changed_grp = True
                    End If

                    If changed_grp Then
                        changed = True
                        group.Invalidate()
                    End If
                Next

                Dim vertextIds As IEnumerable(Of String) = Nothing
                If inactiveObjects.TryGetValue(KblObjectType.Node, vertextIds) Then
                    For Each vertexId As String In vertextIds
                        Dim vertexVdGroups As IDictionary(Of VdSVGGroup, VdSVGGroup) = Nothing
                        If _groupMapper.TryGetValue(vertexId, vertexVdGroups) AndAlso (DirectCast(.KBLOccurrenceMapper(vertexId), Node).Referenced_components IsNot Nothing) Then
                            For Each refComponent As String In DirectCast(.KBLOccurrenceMapper(vertexId), Node).Referenced_components?.SplitSpace.OrEmpty
                                If refComponent = connector.SystemId Then
                                    For Each group As VdSVGGroup In vertexVdGroups.Values.ToList
                                        Dim changed_grp As Boolean = False
                                        If group.SetVisibility(VisibilityEnum.Visible) Then
                                            changed_grp = True
                                        End If

                                        If UpdateGroupVisibility_Recursively(group.ChildGroups, VisibilityEnum.Visible, True) Then
                                            changed_grp = True
                                        End If

                                        If group.SetLighting(Lighting.Lowlight) Then
                                            changed_grp = True
                                        End If

                                        If changed_grp Then
                                            changed = True
                                            group.Invalidate()
                                        End If
                                    Next
                                End If
                            Next
                        End If
                    Next
                End If
            ElseIf inactiveParts.Count > 0 Then
                For Each inactivePart As String In inactiveParts
                    Dim inactiveVdGroups As IDictionary(Of VdSVGGroup, VdSVGGroup) = Nothing
                    If _groupMapper.TryGetValue(inactivePart, inactiveVdGroups) Then
                        For Each group As VdSVGGroup In inactiveVdGroups.Values.ToList
                            Dim changed_grp As Boolean = False
                            If group.SetVisibility(VisibilityEnum.Visible) Then
                                'HINT this causes accessories to be switch on again in draft mode at least. It is not clear if this can be touched MR
                                changed_grp = True
                            End If

                            If Not activeParts.Contains(group.KblId) AndAlso Not group.SecondaryKblIds.ContainsAnyOf(activeParts) Then
                                If group.SetLighting(Lighting.Lowlight) Then
                                    changed_grp = True
                                End If
                            End If

                            If changed_grp Then
                                changed = True
                                group.Invalidate()
                            End If

                            If group.ParentGroup?.SVGType = SvgType.row.ToString Then
                                Dim parent_changed As Boolean = False
                                If group.ParentGroup.SetVisibility(VisibilityEnum.Visible) Then
                                    parent_changed = True
                                End If

                                If Not activeParts.Contains(group.ParentGroup.KblId) AndAlso Not group.ParentGroup.SecondaryKblIds.ContainsAnyOf(activeParts) Then
                                    If group.ParentGroup.SetLighting(Lighting.Lowlight) Then
                                        parent_changed = True
                                    End If
                                End If

                                If parent_changed Then
                                    changed = True
                                    group.ParentGroup.Invalidate()
                                End If
                            End If
                        Next
                    End If
                Next
            End If
        End With
        Return changed
    End Function

    Private Function UpdateGreyOutStateOfConnectors(inactiveObjects As ITypeGroupedKblIds, counter As Integer, count As Integer) As Boolean
        Dim connectorCounter As Integer = counter
        Dim progress As Integer = counter
        Dim oldProgress As Integer = -1
        Dim changed As Boolean = False

        With _documentForm.KBL
            For Each connector As Connector_occurrence In _documentForm.KBL.GetConnectorOccurrences
                progress = CInt(connectorCounter * 100 / count)

                If progress > oldProgress Then
                    _messageEventArgs.ProgressPercentage = progress
                    RaiseEvent Message(Me, _messageEventArgs)
                    oldProgress = progress
                End If

                If _groupMapper.ContainsKey(connector.SystemId) Then
                    Dim activeParts As New HashSet(Of String)
                    Dim inactiveParts As New HashSet(Of String)

                    For Each accessory As Accessory_occurrence In .GetAccessoryOccurrences
                        If (accessory.Reference_element IsNot Nothing) AndAlso (accessory.Reference_element = connector.SystemId) Then
                            If inactiveObjects.ContainsValue(KblObjectType.Accessory_occurrence, accessory.SystemId) Then
                                inactiveParts.Add(accessory.SystemId)
                            Else
                                activeParts.Add(accessory.SystemId)
                            End If
                        End If
                    Next

                    For Each component As Component_occurrence In .GetComponentOccurrences
                        If Not String.IsNullOrEmpty(component.Mounting) Then
                            For Each mounting As String In component.Mounting.SplitSpace
                                If mounting = connector.SystemId Then
                                    If inactiveObjects.ContainsValue(component.ObjectType, component.SystemId) Then
                                        inactiveParts.Add(component.SystemId)
                                    Else
                                        activeParts.Add(component.SystemId)
                                    End If
                                End If
                            Next
                        End If
                    Next

                    For Each contactPoint As Contact_point In connector.Contact_points
                        If Not String.IsNullOrEmpty(contactPoint.Associated_parts) Then
                            For Each associatedPart As String In contactPoint.Associated_parts.SplitSpace
                                Dim occ As IKblOccurrence = .GetOccurrenceObjectUntyped(associatedPart)
                                If occ IsNot Nothing Then
                                    Select Case occ.ObjectType
                                        Case KblObjectType.Cavity_seal_occurrence, KblObjectType.Special_terminal_occurrence, KblObjectType.Terminal_occurrence
                                            If inactiveObjects.ContainsValue(occ.ObjectType, associatedPart) Then
                                                inactiveParts.Add(associatedPart)
                                                inactiveParts.Add(contactPoint.Contacted_cavity)
                                            Else
                                                activeParts.Add(associatedPart)
                                                activeParts.Add(contactPoint.Contacted_cavity)
                                            End If
                                    End Select
                                End If
                            Next
                        End If


                        For Each wireId As String In .KBLContactPointWireMapper.TryGetOrDefault(contactPoint.SystemId)
                            Dim wireOcc As IKblOccurrence = .GetOccurrenceObjectUntyped(wireId)
                            If wireOcc IsNot Nothing Then
                                Select Case wireOcc.ObjectType
                                    Case KblObjectType.Core_occurrence, KblObjectType.Wire_occurrence
                                        Dim kblType As KblObjectType = wireOcc.ObjectType
                                        If kblType = KblObjectType.Core_occurrence Then ' HINT: map BL of the core to Cable and proceed normally
                                            kblType = KblObjectType.Special_wire_occurrence
                                            wireId = .KBLCoreCableMapper(wireId)
                                        End If

                                        If inactiveObjects.ContainsValue(kblType, wireId) Then
                                            inactiveParts.Add(wireId)
                                            inactiveParts.Add(contactPoint.Contacted_cavity)
                                        Else
                                            activeParts.Add(wireId)
                                            activeParts.Add(contactPoint.Contacted_cavity)
                                        End If
                                End Select
                            End If
                        Next
                    Next

                    If UpdateGreyOutStateOfConnector(activeParts, connector, inactiveObjects, inactiveParts) Then
                        changed = True
                    End If
                End If

                connectorCounter += 1
            Next
        End With
        Return changed
    End Function

    Private Function UpdateGreyOutStateOfDimensionsAndFixings(greyedOutSegments As List(Of Segment), invisibleSegments As List(Of Segment)) As Boolean
        Dim changed As Boolean
        With _documentForm.KBL
            For Each greyedOutSegment As Segment In greyedOutSegments
                If (_groupMapper.ContainsKey(greyedOutSegment.Start_node)) Then
                    For Each group As VdSVGGroup In _groupMapper(greyedOutSegment.Start_node).Values.ToList
                        If (group.SVGType = SvgType.dimension.ToString) Then
                            For Each secondaryKblId As String In group.SecondaryKblIds
                                Dim node As Node = .GetOccurrenceObject(Of Node)(secondaryKblId)
                                If node IsNot Nothing AndAlso (secondaryKblId = greyedOutSegment.End_node) Then
                                    If group.Invalidate(VisibilityEnum.Visible, Lighting.Lowlight) Then
                                        changed = True
                                    End If
                                Else
                                    Dim fixing As Fixing_occurrence = .GetOccurrenceObject(Of Fixing_occurrence)(secondaryKblId)
                                    If fixing IsNot Nothing AndAlso (greyedOutSegment.Fixing_assignment IsNot Nothing) Then
                                        For Each fixingAssignment As Fixing_assignment In greyedOutSegment.Fixing_assignment
                                            If (fixingAssignment.Fixing = secondaryKblId) Then
                                                If group.Invalidate(VisibilityEnum.Visible, Lighting.Lowlight) Then
                                                    changed = True
                                                End If
                                            End If
                                        Next
                                    End If
                                End If
                            Next
                        End If
                    Next
                End If

                If (_groupMapper.ContainsKey(greyedOutSegment.End_node)) Then
                    For Each group As VdSVGGroup In _groupMapper(greyedOutSegment.End_node).Values.ToList
                        If (group.SVGType = SvgType.dimension.ToString) Then
                            For Each secondaryKblId As String In group.SecondaryKblIds
                                Dim node As Node = .GetOccurrenceObject(Of Node)(secondaryKblId)
                                If node IsNot Nothing AndAlso (secondaryKblId = greyedOutSegment.Start_node) Then
                                    If group.Invalidate(VisibilityEnum.Visible, Lighting.Lowlight) Then
                                        changed = True
                                    End If
                                Else
                                    Dim fixing As Fixing_occurrence = .GetOccurrenceObject(Of Fixing_occurrence)(secondaryKblId)
                                    If fixing IsNot Nothing AndAlso greyedOutSegment.Fixing_assignment IsNot Nothing Then
                                        For Each fixingAssignment As Fixing_assignment In greyedOutSegment.Fixing_assignment
                                            If fixingAssignment.Fixing = secondaryKblId Then
                                                If group.Invalidate(VisibilityEnum.Visible, Lighting.Lowlight) Then
                                                    changed = True
                                                End If
                                            End If
                                        Next
                                    End If
                                End If
                            Next
                        End If
                    Next
                End If
            Next

            For Each invisibleSegment As Segment In invisibleSegments
                If (_groupMapper.ContainsKey(invisibleSegment.Start_node)) Then
                    For Each group As VdSVGGroup In _groupMapper(invisibleSegment.Start_node).Values.ToList
                        If (group.SVGType = SvgType.dimension.ToString) Then
                            For Each secondaryKblId As String In group.SecondaryKblIds
                                Dim node As Node = .GetOccurrenceObject(Of Node)(secondaryKblId)
                                If node IsNot Nothing AndAlso (secondaryKblId = invisibleSegment.End_node) Then
                                    If group.SetVisibility(VisibilityEnum.Invisible, True) Then
                                        changed = True
                                    End If
                                Else
                                    Dim fixing As Fixing_occurrence = .GetOccurrenceObject(Of Fixing_occurrence)(secondaryKblId)
                                    If fixing IsNot Nothing AndAlso (invisibleSegment.Fixing_assignment IsNot Nothing) Then
                                        For Each fixingAssignment As Fixing_assignment In invisibleSegment.Fixing_assignment
                                            If (fixingAssignment.Fixing = secondaryKblId) Then
                                                If group.SetVisibility(VisibilityEnum.Invisible, True) Then
                                                    changed = True
                                                End If
                                            End If
                                        Next
                                    End If
                                End If
                            Next
                        End If
                    Next
                End If

                If (_groupMapper.ContainsKey(invisibleSegment.End_node)) Then
                    For Each group As VdSVGGroup In _groupMapper(invisibleSegment.End_node).Values.ToList
                        If (group.SVGType = SvgType.dimension.ToString) Then
                            For Each secondaryKblId As String In group.SecondaryKblIds
                                Dim node As Node = .GetOccurrenceObject(Of Node)(secondaryKblId)
                                If node IsNot Nothing AndAlso (secondaryKblId = invisibleSegment.Start_node) Then
                                    If group.SetVisibility(VisibilityEnum.Invisible, True) Then
                                        changed = True
                                    End If
                                Else
                                    Dim fixing As Fixing_occurrence = .GetOccurrenceObject(Of Fixing_occurrence)(secondaryKblId)
                                    If fixing IsNot Nothing AndAlso (invisibleSegment.Fixing_assignment IsNot Nothing) Then
                                        For Each fixingAssignment As Fixing_assignment In invisibleSegment.Fixing_assignment
                                            If (fixingAssignment.Fixing = secondaryKblId) Then
                                                If group.SetVisibility(VisibilityEnum.Invisible, True) Then
                                                    changed = True
                                                End If
                                            End If
                                        Next
                                    End If
                                End If
                            Next
                        End If
                    Next
                End If
            Next
        End With
        Return changed
    End Function

    Private Function UpdateGreyOutStateOfSegments(inactiveObjects As ITypeGroupedKblIds, counter As Integer, count As Integer) As Boolean
        Dim segmentCounter As Integer = counter
        Dim progress As Integer = counter
        Dim oldProgress As Integer = -1
        Dim changed As Boolean = False

        With _documentForm.KBL
            If _documentForm.KBL.GetSegments.Any() Then
                For Each segment As Segment In .GetSegments
                    progress = CInt(segmentCounter * 100 / count)

                    If progress > oldProgress Then
                        _messageEventArgs.ProgressPercentage = progress
                        RaiseEvent Message(Me, _messageEventArgs)
                        oldProgress = progress
                    End If

                    If _groupMapper.ContainsKey(segment.SystemId) Then
                        Dim hasProtections As Boolean = False
                        Dim hasRoutingContent As Boolean = False

                        Dim activeProtections As New List(Of String)
                        Dim inactiveProtections As New List(Of String)

                        If segment.Protection_area.OrEmpty.Length > 0 Then
                            hasProtections = True

                            For Each protectionArea As Protection_area In segment.Protection_area
                                If inactiveObjects.GetValueOrEmpty(KblObjectType.Wire_protection_occurrence).Contains(protectionArea.Associated_protection) Then
                                    inactiveProtections.Add(protectionArea.Associated_protection)
                                Else
                                    activeProtections.Add(protectionArea.Associated_protection)
                                End If
                            Next
                        End If

                        Dim activeWires As New HashSet(Of String)
                        Dim inactiveWires As New HashSet(Of String)

                        If .KBLSegmentWireMapper.ContainsKey(segment.SystemId) Then
                            hasRoutingContent = True

                            For Each wireId As String In .KBLSegmentWireMapper(segment.SystemId)
                                Dim wireCore As IKblOccurrence = .GetOccurrenceObject(Of Wire_occurrence)(wireId)
                                If wireCore Is Nothing Then
                                    wireCore = .GetOccurrenceObject(Of Core_occurrence)(wireId)
                                End If

                                Select Case wireCore.ObjectType
                                    Case KblObjectType.Core_occurrence, KblObjectType.Wire_occurrence
                                        Dim kblObjectType As KblObjectType = wireCore.ObjectType
                                        If kblObjectType = KblObjectType.Core_occurrence Then
                                            wireId = .KBLCoreCableMapper(wireId)
                                            kblObjectType = KblObjectType.Special_wire_occurrence
                                        End If

                                        If Not inactiveObjects.GetValueOrEmpty(kblObjectType).Contains(wireId) Then
                                            activeWires.Add(wireId)
                                        ElseIf inactiveObjects.GetValueOrEmpty(kblObjectType).Contains(wireId) Then
                                            inactiveWires.Add(wireId)
                                        End If
                                End Select
                            Next
                        End If

                        If hasProtections OrElse hasRoutingContent Then
                            Dim lighting As Lighting = Lighting.Normal
                            Dim visibility As VisibilityEnum = VisibilityEnum.Visible

                            If (activeProtections.Count > 0) OrElse (activeWires.Count > 0 OrElse (Not _documentForm.HideEntitiesWithNoModules AndAlso Not hasRoutingContent)) Then
                                lighting = If(activeProtections.Count > 0 OrElse (activeWires.Count > 0 AndAlso Not hasProtections), Lighting.Normal, Lighting.Lowlight)
                            Else
                                visibility = VisibilityEnum.Invisible

                                For Each group As VdSVGGroup In _redliningMapper.GetValueOrEmpty(segment.SystemId)
                                    If group.SetVisibility(visibility, True) Then
                                        changed = True
                                    End If
                                Next

                                For Each stamp As QMStamp In _qmStamps.Stamps.Where(Function(s) s.ObjectReferences IsNot Nothing AndAlso s.ObjectReferences.FindObjRefFromKblId(segment.SystemId) IsNot Nothing)
                                    For Each group As VdSVGGroup In _stampMapper.GetValueOrEmpty(stamp.Id)
                                        If group.SetVisibility(visibility, True) Then
                                            changed = True
                                        End If
                                    Next
                                Next
                            End If

                            For Each group As VdSVGGroup In _groupMapper(segment.SystemId).Values.ToList
                                Dim group_changed As Boolean = False
                                If UpdateGroupVisibility_Recursively(group.ChildGroups, visibility, True) Then
                                    changed = True
                                    group_changed = True
                                End If

                                If group.Invalidate(visibility, lighting, group_changed) Then
                                    changed = True
                                End If
                            Next
                        End If
                    End If

                    segmentCounter += 1
                Next
            End If
        End With
        Return changed
    End Function

    Private Function UpdateGreyOutStateOfVertices(inactiveObjects As ITypeGroupedKblIds, counter As Integer, count As Integer) As Boolean
        Dim vertexCounter As Integer = counter
        Dim progress As Integer = counter
        Dim oldProgress As Integer = -1
        Dim changed As Boolean

        With _documentForm.KBL
            For Each vertex As Node In .GetNodes
                progress = CInt((vertexCounter * 100) / count)

                If progress > oldProgress Then
                    _messageEventArgs.ProgressPercentage = progress
                    RaiseEvent Message(Me, _messageEventArgs)
                    oldProgress = progress
                End If

                Dim vtxmappedSegments As List(Of String) = Nothing
                If (_groupMapper.ContainsKey(vertex.SystemId)) AndAlso (.KBLVertexSegmentMapper.TryGetValue(vertex.SystemId, vtxmappedSegments)) AndAlso Not inactiveObjects.GetValueOrEmpty(KblObjectType.Node).Contains(vertex.SystemId) Then
                    Dim greyedOutSegments As New List(Of Segment)
                    Dim invisibleSegments As New List(Of Segment)
                    Dim visibleSegments As New List(Of Segment)

                    For Each segmentId As String In vtxmappedSegments
                        Dim segmentGroups As IDictionary(Of VdSVGGroup, VdSVGGroup) = Nothing
                        If _groupMapper.TryGetValue(segmentId, segmentGroups) Then
                            For Each group As VdSVGGroup In segmentGroups.Values.ToList
                                Dim seg As Segment = DirectCast(.KBLOccurrenceMapper(segmentId), Segment)
                                If group.visibility = VisibilityEnum.Invisible Then
                                    invisibleSegments.Add(seg)
                                ElseIf group.Lighting = Lighting.Lowlight Then
                                    greyedOutSegments.Add(seg)
                                ElseIf group.Lighting = Lighting.Normal Then
                                    visibleSegments.Add(seg)
                                End If
                            Next
                        End If
                    Next

                    If visibleSegments.Count > 0 Then
                        For Each group As VdSVGGroup In _groupMapper(vertex.SystemId).Values.ToList
                            Dim changed_grp As Boolean = False
                            If group.SetVisibility(VisibilityEnum.Visible) Then
                                changed_grp = True
                            End If

                            If UpdateGroupVisibility_Recursively(group.ChildGroups, VisibilityEnum.Visible, False) Then
                                changed_grp = True
                            End If

                            If group.SetLighting(Lighting.Normal) Then
                                changed_grp = True
                            End If

                            If changed_grp Then
                                group.Invalidate()
                                changed = True
                            End If
                        Next

                        For Each group As VdSVGGroup In _redliningMapper.GetValueOrEmpty(vertex.SystemId)
                            If group.SetVisibility(VisibilityEnum.Visible, True) Then
                                changed = True
                            End If
                        Next

                        For Each stamp As QMStamp In _qmStamps.Stamps.Where(Function(s) s.ObjectReferences IsNot Nothing AndAlso s.ObjectReferences.FindObjRefFromKblId(vertex.SystemId) IsNot Nothing)
                            For Each group As VdSVGGroup In _stampMapper.GetValueOrEmpty(stamp.Id)
                                If group.SetVisibility(VisibilityEnum.Visible, True) Then
                                    changed = True
                                End If
                            Next
                        Next
                    ElseIf (greyedOutSegments.Count <> 0) Then
                        Dim allReferencedComponentsInactive As Boolean = True
                        If vertex.Referenced_components IsNot Nothing Then
                            For Each referencedComponent As String In vertex.Referenced_components.SplitSpace
                                If Not inactiveObjects.GetValueOrEmpty(KblObjectType.Connector_occurrence).Contains(referencedComponent) OrElse Not inactiveObjects.GetValueOrEmpty(KblObjectType.Fixing_occurrence).Contains(referencedComponent) Then
                                    allReferencedComponentsInactive = False
                                    Exit For
                                End If
                            Next
                        End If

                        If allReferencedComponentsInactive Then
                            For Each group As VdSVGGroup In _groupMapper(vertex.SystemId).Values.ToList
                                Dim changed_grp As Boolean = False
                                If group.SetVisibility(VisibilityEnum.Visible) Then
                                    changed_grp = True
                                End If

                                If UpdateGroupVisibility_Recursively(group.ChildGroups, VisibilityEnum.Visible, False) Then
                                    changed_grp = True
                                End If

                                If group.SetLighting(Lighting.Lowlight) Then
                                    changed_grp = True
                                End If

                                If changed_grp Then
                                    changed = True
                                    group.Invalidate()
                                End If
                            Next
                        End If
                    ElseIf invisibleSegments.Count > 0 Then
                        For Each group As VdSVGGroup In _groupMapper(vertex.SystemId).Values.ToList
                            Dim changed_grp As Boolean = False
                            If group.SetVisibility(VisibilityEnum.Invisible) Then
                                changed_grp = True
                            End If

                            If UpdateGroupVisibility_Recursively(group.ChildGroups, VisibilityEnum.Invisible, True) Then
                                changed_grp = True
                            End If

                            If changed_grp Then
                                changed = True
                                group.Invalidate()
                            End If
                        Next

                        Dim redlingVdGroups As IList(Of VdSVGGroup) = Nothing
                        If _redliningMapper.TryGetValue(vertex.SystemId, redlingVdGroups) Then
                            For Each group As VdSVGGroup In redlingVdGroups
                                If group.SetVisibility(VisibilityEnum.Invisible, True) Then
                                    changed = True
                                End If
                            Next
                        End If

                        For Each stamp As QMStamp In _qmStamps.Stamps.Where(Function(s) s.ObjectReferences IsNot Nothing AndAlso s.ObjectReferences.FindObjRefFromKblId(vertex.SystemId) IsNot Nothing)
                            For Each group As VdSVGGroup In _stampMapper.GetValueOrEmpty(stamp.Id)
                                If group.SetVisibility(VisibilityEnum.Invisible, True) Then
                                    changed = True
                                End If
                            Next
                        Next
                    End If

                    If UpdateGreyOutStateOfDimensionsAndFixings(greyedOutSegments, invisibleSegments) Then
                        changed = True
                    End If
                End If

                vertexCounter += 1
            Next
        End With
        Return changed
    End Function

    Private Function UpdateGroupLighting_Recursively(groups As vdEntities, lighting As Lighting, Optional ByRef changed As Boolean = False) As String()
        Dim list As New List(Of String)
        For Each group As VdSVGGroup In groups
            If group.Lighting <> lighting Then
                If group.SetLighting(lighting) Then
                    changed = True
                    group.Update()
                    group.Invalidate()
                End If
            End If

            If Not String.IsNullOrEmpty(group.KblId) Then
                list.Add(group.KblId)
            End If

            list.AddRange(UpdateGroupLighting_Recursively(group.ChildGroups, lighting, changed))
        Next
        Return list.Distinct.ToArray
    End Function

    Private Function UpdateGroupVisibility_Recursively(groups As vdEntities, visibility As VisibilityEnum, considerGroupsWithKblId As Boolean) As Boolean
        Dim changed As Boolean
        For Each group As VdSVGGroup In groups
            If considerGroupsWithKblId OrElse String.IsNullOrEmpty(group.KblId) Then
                If group.SetVisibility(visibility) Then
                    changed = True
                End If

                If UpdateGroupVisibility_Recursively(group.ChildGroups, visibility, considerGroupsWithKblId) Then
                    changed = True
                End If
            End If
        Next
        Return changed
    End Function

    Private Sub _svgConverter_Message(sender As Object, e As Zuken.E3.Lib.Converter.Svg.MessageEventArgs) Handles _svgConverter.Message ', _draftConverter.Message
        If (_workState?.Progress Is Nothing) AndAlso e.MessageType = MessageType.BeginLoadingFile Then
            SetConverterMessageEventArgs(CType(sender, IConverter).Type, e)
            RaiseEvent Message(Me, _messageEventArgs)
        End If

        _documentForm.WriteLogMessage(CType(sender, IConverter).Type, e, _logEventArgs)
    End Sub

    Private Sub _draftConverter_ProgessChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles _draftConverter.ProgressChanged
        OnProgressChangedMessage(e)
    End Sub

    Private Sub _svgConverter_ProgessChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles _svgConverter.ProgressChanged
        OnProgressChangedMessage(New ProgressChangedEventArgs(e.ProgressPercentage, e.UserState))
    End Sub

    Protected Sub OnProgressChangedMessage(e As System.ComponentModel.ProgressChangedEventArgs)
        If (_workState?.Progress IsNot Nothing) Then
            _workState.Progress.Report(e.ProgressPercentage)
        Else
            _messageEventArgs.ProgressPercentage = e.ProgressPercentage
            RaiseEvent Message(Me, _messageEventArgs)
        End If
    End Sub

    Private Sub SetConverterMessageEventArgs(sourceType As ConverterType, e As Zuken.E3.Lib.Converter.Svg.MessageEventArgs)
        Select Case e.MessageType
            Case MessageType.BeginLoadingFile
                Select Case sourceType
                    Case ConverterType.DraftConverter
                        _messageEventArgs.StatusMessage = String.Format(My.Resources.LoggingStrings.DraftConv_LoadingDrawing, IO.Path.GetFileNameWithoutExtension(Me._documentForm.File?.KblDocument?.FullName))
                    Case ConverterType.SvgConverter
                        _messageEventArgs.StatusMessage = String.Format(My.Resources.LoggingStrings.SvgConv_LoadingDrawing, IO.Path.GetFileNameWithoutExtension(e.Message))
                End Select
                _messageEventArgs.SetUseLastPosition()
        End Select
    End Sub

    Private Sub tooltipTimer_Tick(sender As Object, e As EventArgs) Handles tooltipTimer.Tick
        If _hoverEntity IsNot Nothing Then
            _vdBaseControl.ActiveDocument.EnableAutoFocus = True
            _tooltipForm.FillDataSource(DirectCast(_hoverEntity, VdSVGGroup).KblId)
            _tooltipForm.Show()
        End If

        Me.tooltipTimer.Enabled = False
    End Sub

    Private Sub _vdBaseControl_ActionEnd(sender As Object, actionName As String) Handles _vdBaseControl.ActionEnd
        Select Case actionName
            Case "BaseAction_ActionPan"
                _panActivated = False
       '         If (_viewRatio < _generalSettings.TextVisibilityZoomLevel) Then _vdBaseControl.ActiveDocument.Invalidate()
            Case "BaseAction_VdMagnifier"
                _magnifierActivated = False
        End Select
    End Sub

    Private Sub _vdBaseControl_ActionError(sender As Object, actionName As String) Handles _vdBaseControl.ActionError
        Select Case actionName
            Case "BaseAction_ActionPan"
                _panActivated = False
            Case "BaseAction_VdMagnifier"
                _magnifierActivated = False
        End Select
    End Sub

    Private Sub _vdBaseControl_ActionStart(sender As Object, actionName As String, ByRef cancel As Boolean) Handles _vdBaseControl.ActionStart
        Select Case actionName
            Case "BaseAction_ActionPan"
                _panActivated = True

            Case "BaseAction_VdMagnifier"
                _magnifierActivated = True
        End Select
    End Sub

    Private Sub _vdBaseControl_DrawAfter(sender As Object, render As VectorDraw.Render.vdRender) Handles _vdBaseControl.DrawAfter
        If (_highlightPolylines IsNot Nothing) Then
            For Each highlightPolyline As vdPolyline In _highlightPolylines
                highlightPolyline.Draw(render)
            Next
        End If

        If (_highlightRects IsNot Nothing) Then
            For Each highlightRect As vdRect In _highlightRects
                highlightRect.Draw(render)
            Next
        End If
    End Sub

    Private Sub _vdBaseControl_DrawOverAll(sender As Object, render As VectorDraw.Render.vdRender, ByRef cancel As Boolean) Handles _vdBaseControl.DrawOverAll
        _messageEventArgs.StatusMessage = String.Empty
        _messageEventArgs.ProgressPercentage = 0

        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub vdBaseControl_KeyDown(sender As Object, e As KeyEventArgs) Handles _vdBaseControl.KeyDown
        If (e.Control) Then
            If (e.KeyCode = Keys.A) Then
                Using Me.EnableWaitCursor
                    DeselectAll()

                    For Each kblId As String In _groupMapper.Keys
                        AddGroupToSelection(kblId, False)
                    Next

                    If (_generalSettings.ObjectTypeDependentSelection) Then
                        SetObjectTypeDependentSelection()
                    End If

                    _vdBaseControl.ActiveDocument.CommandAction.Zoom("E", 0, 0)
                End Using

                RaiseEvent CanvasSelectionChanged(sender)
            ElseIf (e.KeyCode = Keys.Add) OrElse (e.KeyCode = Keys.Oemplus) Then
                _vdBaseControl.ActiveDocument.CommandAction.Zoom("S", 1.25, 0)
            ElseIf (e.KeyCode = Keys.E) Then
                _vdBaseControl.ActiveDocument.CommandAction.Zoom("E", 0, 0)
            ElseIf (e.KeyCode = Keys.H) AndAlso (Not DirectCast(_documentForm.utmDocument.Tools(HomeTabToolKey.Pan.ToString), StateButtonTool).Checked) Then
                DirectCast(_documentForm.utmDocument.Tools(HomeTabToolKey.Pan.ToString), StateButtonTool).Checked = True

                _vdBaseControl.ActiveDocument.CommandAction.Pan()
            ElseIf (e.KeyCode = Keys.Subtract) OrElse (e.KeyCode = Keys.OemMinus) Then
                _vdBaseControl.ActiveDocument.CommandAction.Zoom("S", (1 / 1.25), 0)
            ElseIf (e.KeyCode = Keys.W) Then
                _vdBaseControl.ActiveDocument.CommandAction.Zoom("W", "USER", "USER")
            ElseIf (e.KeyCode = Keys.Q) Then
                DirectCast(_documentForm.utmDocument.Tools(SettingsTabToolKey.DisplayQMStamps.ToString), StateButtonTool).Checked = True

                _documentForm._informationHub.AddOrEditQMStamp()
            ElseIf (e.KeyCode = Keys.R) Then
                DirectCast(_documentForm.utmDocument.Tools(SettingsTabToolKey.DisplayRedlinings.ToString), StateButtonTool).Checked = True

                _documentForm._informationHub.AddOrEditRedlining(Nothing)
            End If
        ElseIf (e.KeyCode = Keys.Delete) Then
            For Each group As VdSVGGroup In _selection
                If (TypeOf group Is VdStamp) Then
                    _documentForm._informationHub.DeleteQMStamp(False)
                End If
            Next
        ElseIf (e.KeyCode = Keys.Escape) Then
            _vdBaseControl.ActiveDocument.CommandAction.Cancel()

            DirectCast(_documentForm.utmDocument.Tools(HomeTabToolKey.Pan.ToString), StateButtonTool).Checked = False
            DirectCast(_documentForm.utmDocument.Tools(HomeTabToolKey.ZoomMagnifier.ToString), StateButtonTool).Checked = False

            DeselectAll()

            RaiseEvent CanvasSelectionChanged(sender)
        End If
    End Sub

    Private Sub _vdBaseControl_LostFocus(sender As Object, e As EventArgs) Handles _vdBaseControl.LostFocus
        _focusLost = True
    End Sub

    Private Sub vdBaseControl_MouseClick(sender As Object, e As MouseEventArgs) Handles _vdBaseControl.MouseClick
        If (_focusLost) Then
            _focusLost = False
        Else
            If (_vdBaseControl.ActiveDocument.CommandAction.OpenLoops = 0) AndAlso (_hitPoint IsNot Nothing) Then
                If (e.Button = System.Windows.Forms.MouseButtons.Left AndAlso Not E3.Lib.DotNet.Expansions.Devices.My.Computer.Keyboard.CtrlKeyDown) OrElse (e.Button = System.Windows.Forms.MouseButtons.Right) Then
                    DeselectAll()
                End If

                If (e.Button = System.Windows.Forms.MouseButtons.Left) Then
                    _hitEntity = _vdBaseControl.ActiveDocument.ActiveLayOut.GetEntityFromPoint(_vdBaseControl.ActiveDocument.ActiveRender.View2PixelI(_hitPoint), 6, False, vdDocument.LockLayerMethodEnum.DisableAll)
                    If (_hitEntity IsNot Nothing) AndAlso (TryCast(_hitEntity, VdSVGGroup) IsNot Nothing) AndAlso (Not DirectCast(_hitEntity, VdSVGGroup).KblId.IsNullOrEmpty) Then
                        Dim innerGroupKblId As String = String.Empty
                        Dim innerEntities As New VectorDraw.Generics.vdArray(Of vdFigure)

                        _vdBaseControl.ActiveDocument.ActiveLayOut.GetInnerEntitiesListFromPoint(Nothing, _vdBaseControl.ActiveDocument.ActiveRender.View2PixelI(_hitPoint), 6, innerEntities, Nothing, False)

                        If (DirectCast(_hitEntity, VdSVGGroup).SVGType = SvgType.table.ToString) OrElse (DirectCast(_hitEntity, VdSVGGroup).SymbolType = SvgSymbolType.Vertex.ToString AndAlso DirectCast(_hitEntity, VdSVGGroup).SVGType = SvgType.Undefined.ToString) OrElse (innerEntities.OfType(Of VdSVGGroup).Any(Function(g) g.SVGType = SvgType.change.ToString)) Then
                            If (innerEntities.Count <> 0) AndAlso (innerEntities(0).URL <> String.Empty) Then
                                AddGroupToSelection(innerEntities(0).URL, False, False, issueIds:=innerEntities(0).GetIdsIfIssue(True), stampIds:=innerEntities(0).GetIdsIfStamp(True))

                                innerGroupKblId = innerEntities(0).URL
                            End If
                        End If

                        If (innerGroupKblId = String.Empty) Then
                            AddGroupToSelection(DirectCast(_hitEntity, VdSVGGroup).KblId, False, issueIds:=_hitEntity.GetIdsIfIssue(True), stampIds:=_hitEntity.GetIdsIfStamp(True))
                        End If
                        If (_generalSettings.ObjectTypeDependentSelection) Then
                            SetObjectTypeDependentSelection()
                        End If
                    End If
                End If
                RaiseEvent CanvasMouseClick(sender, e)
            End If
        End If

    End Sub

    Private Sub _vdBaseControl_MouseDown(sender As Object, e As MouseEventArgs) Handles _vdBaseControl.MouseDown
        Dim doc As vdDocument = _vdBaseControl.ActiveDocument
        If (e.Button = System.Windows.Forms.MouseButtons.Left) AndAlso (doc.CommandAction.OpenLoops = 0) Then
            _hitPoint = doc.CCS_CursorPos
            _gripEntity = doc.GetGripFromPoint(_hitPoint, _gripIndex)
        ElseIf (e.Button = System.Windows.Forms.MouseButtons.Right) Then
            DeselectAll()
            RaiseEvent CanvasSelectionChanged(sender)
        Else
            _hitPoint = Nothing
        End If
    End Sub

    Private Sub _vdBaseControl_MouseLeave(sender As Object, e As EventArgs) Handles _vdBaseControl.MouseLeave
        If (_vdBaseControl.ActiveDocument.EnableToolTips) Then
            _tooltipForm.Hide()
            _vdBaseControl.ActiveDocument.EnableAutoFocus = False

            Me.tooltipTimer.Enabled = False
        End If
    End Sub

    Private Sub _vdBaseControl_MouseMove(sender As Object, e As MouseEventArgs) Handles _vdBaseControl.MouseMove
        Static mpos As System.Drawing.Point
        If Not (mpos.X - e.Location.X = 0 AndAlso mpos.Y - e.Location.Y = 0) Then
            Static cursorPos As gPoint
            If _vdBaseControl.ActiveDocument.CommandAction.OpenLoops = 0 Then
                Dim hitEntity As VectorDraw.Professional.vdPrimaries.vdFigure = _vdBaseControl.ActiveDocument.ActiveLayOut.GetEntityFromPoint(_vdBaseControl.ActiveDocument.ActiveRender.View2PixelI(_vdBaseControl.ActiveDocument.CCS_CursorPos), 6, False, vdDocument.LockLayerMethodEnum.DisableAll)
                Dim isMouseLeft As Boolean = e.Button = System.Windows.Forms.MouseButtons.Left

                Me.uttmCanvas.HideToolTip()

                If hitEntity IsNot Nothing Then

                    If (hitEntity.XProperties.FindName("Tooltip") IsNot Nothing) AndAlso (hitEntity.XProperties.FindName("Tooltip").PropValue IsNot Nothing) Then
                        Dim ttInfo As UltraToolTipInfo = Me.uttmCanvas.GetUltraToolTip(_vdBaseControl)
                        ttInfo.ToolTipText = hitEntity.XProperties.FindName("Tooltip").PropValue.ToString
                        ttInfo.ToolTipImage = Infragistics.Win.ToolTipImage.Info
                        ttInfo.ToolTipTitle = KblObjectType.Redlining.ToString
                        ttInfo.Enabled = Infragistics.Win.DefaultableBoolean.False

                        Me.uttmCanvas.ShowToolTip(_vdBaseControl)
                    End If
                End If

                If ((isMouseLeft) AndAlso (Not E3.Lib.DotNet.Expansions.Devices.My.Computer.Keyboard.CtrlKeyDown) AndAlso (_hitEntity Is Nothing) AndAlso (_hitPoint IsNot Nothing)) OrElse (isMouseLeft AndAlso Not E3.Lib.DotNet.Expansions.Devices.My.Computer.Keyboard.CtrlKeyDown AndAlso _gripEntity IsNot Nothing AndAlso _hitPoint IsNot Nothing) Then
                    Dim selection As New vdSelection

                    If (_gripEntity IsNot Nothing) Then
                        If MoveDetect() Then StartMoveGripEntity()
                    Else
                        If (_vdBaseControl.ActiveDocument.ActionUtility.getUserStartWindowSelection(_hitPoint, selection) = VectorDraw.Actions.StatusCode.Success) Then
                            Using Me.vdCanvas.EnableWaitCursor
                                DeselectAll()

                                For Each group As VdSVGGroup In selection
                                    If (group.KblId <> String.Empty) AndAlso (group.SVGType <> SvgType.dimension.ToString) Then
                                        AddGroupToSelection(group.KblId, False, issueIds:=group.GetIdsIfIssue)

                                        If (group.SymbolType = SvgSymbolType.Vertex.ToString) AndAlso (group.SVGType = SvgType.Undefined.ToString) Then
                                            For Each childGroup As VdSVGGroup In group.ChildGroups
                                                If (childGroup.KblId <> String.Empty) Then
                                                    AddGroupToSelection(childGroup.KblId, False, issueIds:=childGroup.GetIdsIfIssue)
                                                End If
                                            Next
                                        End If
                                    End If
                                Next

                                If (_generalSettings.ObjectTypeDependentSelection) Then
                                    SetObjectTypeDependentSelection()
                                End If

                                RaiseEvent CanvasSelectionChanged(sender)
                            End Using
                        End If

                        If (selection IsNot Nothing) Then
                            selection.Dispose()
                        End If

                        _hitPoint = Nothing
                    End If
                ElseIf (_vdBaseControl.ActiveDocument.EnableToolTips) Then
                    _hoverEntity = hitEntity

                    If (_hoverEntity IsNot Nothing) AndAlso (DisplayTooltip()) Then
                        _tooltipForm.Location = New Drawing.Point(Control.MousePosition.X + 20, Control.MousePosition.Y + 20)

                        If (Not _tooltipForm.Visible) AndAlso (Not Me.tooltipTimer.Enabled) Then
                            Me.tooltipTimer.Enabled = True
                        End If
                    Else
                        _tooltipForm.Hide()
                        _vdBaseControl.ActiveDocument.EnableAutoFocus = False

                        Me.tooltipTimer.Enabled = False
                    End If
                End If
            End If

            cursorPos = If(_vdBaseControl?.ActiveDocument IsNot Nothing, _vdBaseControl.ActiveDocument.CCS_CursorPos, New gPoint)

            RaiseEvent CanvasMouseMove(sender, e)
        End If
        mpos = e.Location

    End Sub

    Private Sub _vdBaseControl_NoFileFind(sender As Object, ByRef fileName As String, ByRef success As Boolean) Handles _vdBaseControl.NoFileFind
        If Not String.IsNullOrEmpty(IO.Path.GetFileNameWithoutExtension(fileName)) Then
            _documentForm._logHub.WriteLogMessage(New LogEventArgs With {.LogLevel = LogEventArgs.LoggingLevel.Warning, .LogMessage = String.Format(DrawingCanvasStrings.TTFOrSHXFileNotFound_LogMsg, fileName)})
            _fontShapeFilesMissing = True
        End If
    End Sub

    Private Sub _vdBaseControl_Progress(sender As Object, percent As Long, jobDescription As String) Handles _vdBaseControl.Progress
        If percent > _progressValue Then
            If (jobDescription.StartsWith("Converting Drawing")) OrElse (jobDescription.StartsWith("Exporting Drawing")) OrElse (jobDescription.StartsWith("Loading from")) OrElse (jobDescription.StartsWith("Saving to")) Then
                With _messageEventArgs
                    .ProgressPercentage = CInt(percent)
                    .StatusMessage = If(Not Me.vdCanvas.Visible, DrawingCanvasStrings.ImportDrw_LogMsg, DrawingCanvasStrings.ExportDrw_LogMsg)
                    .SetUseLastPosition()
                End With

                RaiseEvent Message(Me, _messageEventArgs)
            ElseIf jobDescription = E3.Lib.Converter.Svg.Common.DRAWING_ENTITIES_JOB_DESCR AndAlso (Not _drawingDisplayed OrElse _force_drawing_entities_progress > 0) Then
                With _messageEventArgs
                    .ProgressPercentage = CInt(percent)
                    .StatusMessage = If(String.IsNullOrWhiteSpace(DrawingCanvasStatusMessage), My.Resources.DrawingCanvasStrings.DrawingEntities_Msg, DrawingCanvasStatusMessage)
                    .SetUseLastPosition()
                End With

                If _canLazyDrawEntities Then 'HINT: we don't want to show this micro-progresses that take under 500 ms to complete to the user because it's more distracting than helpful
                    RaiseEvent Message(Me, _messageEventArgs)
                End If
            End If
        End If
        _progressValue = percent
    End Sub

    Private Sub _activeDocument_OnProgressStart(sender As Object, jobDescription As String, meterLimit As Long) Handles _activeDocument.OnProgressStart
        If jobDescription <> "Drawing Block Reference" Then
            If jobDescription = E3.Lib.Converter.Svg.Common.DRAWING_ENTITIES_JOB_DESCR Then
                _canLazyDrawEntities = False
                _lazyDrawingEntitiesProgressTimer.Start()
            End If

            _progressValue = -1
            _messageEventArgs = New MessageEventArgs(MessageEventArgs.MsgType.ShowProgressAndMessage)
            _messageEventArgs.StatusMessage = If(Not String.IsNullOrEmpty(DrawingCanvasStatusMessage), DrawingCanvasStatusMessage, jobDescription)
        End If
    End Sub

    Private Sub _activeDocument_OnProgressStop(sender As Object, jobDescription As String) Handles _activeDocument.OnProgressStop
        If jobDescription = E3.Lib.Converter.Svg.Common.DRAWING_ENTITIES_JOB_DESCR Then
            _lazyDrawingEntitiesProgressTimer.Stop() 'HINT: if drawing entities progress is finished under the interval time we don't show the progress at all
            _canLazyDrawEntities = False
        End If

        If Not _panActivated Then
            _vdBaseControl.Cursor = Cursors.Default
        End If

        If jobDescription <> "Drawing Block Reference" Then
            DrawingCanvasStatusMessage = String.Empty
            _messageEventArgs.ProgressPercentage = 100
            RaiseEvent Message(Me, _messageEventArgs)

            If Not _drawingDisplayed AndAlso jobDescription = E3.Lib.Converter.Svg.Common.DRAWING_ENTITIES_JOB_DESCR Then
                If _displayingDrawing Then
                    _drawingDisplayed = True
                    RaiseEvent DrawingDisplayFinished(Me, New EventArgs)
                End If
            End If

            _displayingDrawing = False
        End If
    End Sub

    ReadOnly Property DrawingDisplayed As Boolean
        Get
            Return _drawingDisplayed
        End Get
    End Property

    Private Sub SetDrawBreakTime(ms As Integer)
        GetType(vdRenderGlobalProperties).GetField("TimerBreakForBitmap", BindingFlags.NonPublic Or BindingFlags.Static).SetValue(Nothing, ms) ' HINT: this is for reducing the draw-time to make the drawing for the user mor responsitive
    End Sub

    Friend ReadOnly Property CanvasSelection() As vdSelection
        Get
            Return _selection
        End Get
    End Property

    Friend ReadOnly Property ConversionValidationLevel As SVGValidationLevel
        Get
            Return SVGValidationLevel.None
            'Return If(_svgConverter IsNot Nothing, _svgConverter.ValidationLevel, SVGValidationLevel.None)
        End Get
    End Property

    Friend ReadOnly Property GroupMapper() As Dictionary(Of String, IDictionary(Of VdSVGGroup, VdSVGGroup))
        Get
            Return _groupMapper
        End Get
    End Property

    Friend Property HighlightRects() As List(Of VdRectEx)
        Get
            Return _highlightRects
        End Get
        Set(value As List(Of VdRectEx))
            _highlightRects = value
        End Set
    End Property

    Friend ReadOnly Property IssueMapper() As IDictionary(Of String, IList(Of VdSVGGroup))
        Get
            Return _issueMapper
        End Get
    End Property

    Friend ReadOnly Property StampSelected As Boolean
        Get
            Return _hitEntity IsNot Nothing AndAlso TypeOf _hitEntity Is VdStamp
        End Get
    End Property

    Friend ReadOnly Property StampMapper() As IDictionary(Of String, ICollection(Of VdStamp))
        Get
            Return _stampMapper
        End Get
    End Property

    Public Sub HighlightDistanceTrace(segmentTraces As List(Of SegmentDistanceEntry))
        If CheckGroupMapperForTopology() Then

            _vertexGroups.Clear()
            _highlightPolylines.Clear()

            Dim intermediateVertexPositions As New Dictionary(Of String, VectorDraw.Geometry.gPoint)

            Dim starteEntry As SegmentDistanceEntry = Nothing
            Dim endEntry As SegmentDistanceEntry = Nothing
            For Each entry As SegmentDistanceEntry In segmentTraces
                If (entry.IsComplete) Then
                    If (GroupMapper.ContainsKey(entry.Segment.SystemId)) Then
                        GroupMapper(entry.Segment.SystemId).Values.Where(Function(group) group.SVGType = SvgType.Undefined.ToString).FirstOrDefault.Lighting = Lighting.Highlight
                    End If

                    If (Not intermediateVertexPositions.ContainsKey(entry.Segment.Start_node)) AndAlso (GroupMapper.ContainsKey(entry.Segment.Start_node)) Then
                        Dim group As VdSVGGroup = GroupMapper(entry.Segment.Start_node).Values.Where(Function(g) g.SVGType = SvgType.Undefined.ToString).FirstOrDefault
                        If (group IsNot Nothing) AndAlso (group.BoundingBox IsNot Nothing) Then
                            intermediateVertexPositions.Add(entry.Segment.Start_node, group.BoundingBox.MidPoint)
                        End If
                    End If

                    If (Not intermediateVertexPositions.ContainsKey(entry.Segment.End_node)) AndAlso (GroupMapper.ContainsKey(entry.Segment.End_node)) Then
                        Dim group As VdSVGGroup = GroupMapper(entry.Segment.End_node).Values.Where(Function(g) g.SVGType = SvgType.Undefined.ToString).FirstOrDefault
                        If (group IsNot Nothing) AndAlso (group.BoundingBox IsNot Nothing) Then
                            intermediateVertexPositions.Add(entry.Segment.End_node, group.BoundingBox.MidPoint)
                        End If
                    End If
                Else
                    If entry.Start > 0 AndAlso entry.End < 1.0 Then
                        _highlightPolylines.AddRange(PartlyHighlightSegment(entry.FixingIds, intermediateVertexPositions, entry.Segment))
                    Else
                        Dim nId As String = String.Empty
                        If (entry.Start = 0.0) Then
                            If (Not intermediateVertexPositions.ContainsKey(entry.Segment.Start_node)) Then
                                intermediateVertexPositions.Add(entry.Segment.Start_node, GroupMapper(entry.Segment.Start_node).Values.Where(Function(group) group.SVGType = SvgType.Undefined.ToString).FirstOrDefault.BoundingBox.MidPoint)
                            End If

                            nId = entry.Segment.Start_node
                        End If

                        If (entry.End = 1.0) Then
                            If (Not intermediateVertexPositions.ContainsKey(entry.Segment.End_node)) Then
                                intermediateVertexPositions.Add(entry.Segment.End_node, GroupMapper(entry.Segment.End_node).Values.Where(Function(group) group.SVGType = SvgType.Undefined.ToString).FirstOrDefault.BoundingBox.MidPoint)
                            End If

                            nId = entry.Segment.End_node
                        End If

                        _highlightPolylines.AddRange(PartlyHighlightSegment(entry.FixingIds, intermediateVertexPositions, entry.Segment))

                        Dim endVertexGroup As VdSVGGroup = If(GroupMapper.ContainsKey(nId), GroupMapper(nId).Values.Where(Function(group) group.SVGType = SvgType.Undefined.ToString).FirstOrDefault, Nothing)
                        If (endVertexGroup IsNot Nothing) AndAlso (endVertexGroup.Lighting = Lighting.Normal) Then
                            endVertexGroup.Lighting = Lighting.Highlight
                            _vertexGroups.Add(endVertexGroup)
                        End If
                    End If
                End If
            Next

            vdCanvas.BaseControl.ActiveDocument.Invalidate()
        End If
    End Sub

    Public Sub DeHighLightDistanceTrace(segmentTrace As List(Of SegmentDistanceEntry))
        For Each vertexGroup As VdSVGGroup In _vertexGroups
            vertexGroup.Lighting = Lighting.Normal
        Next

        For Each entry As SegmentDistanceEntry In segmentTrace
            If entry.IsComplete Then
                If GroupMapper.ContainsKey(entry.Segment.SystemId) Then
                    GroupMapper(entry.Segment.SystemId).Values.Where(Function(group) group.SVGType = SvgType.Undefined.ToString).FirstOrDefault.Lighting = Lighting.Normal
                End If
            End If
        Next

        _highlightPolylines.Clear()
        _vertexGroups.Clear()

        vdCanvas.BaseControl.ActiveDocument.Invalidate()
    End Sub

    Private Function PartlyHighlightSegment(fixings As List(Of String), intermediateVertexPositions As Dictionary(Of String, VectorDraw.Geometry.gPoint), segment As Segment) As List(Of VdPolylineEx)
        Dim fixing1 As VdSVGGroup = If(GroupMapper.ContainsKey(fixings.FirstOrDefault), GroupMapper(fixings.FirstOrDefault).Values.Where(Function(group) group.SVGType = SvgType.Undefined.ToString).FirstOrDefault, Nothing)
        Dim fixing2 As VdSVGGroup = If(fixings.Count > 1 AndAlso GroupMapper.ContainsKey(fixings.LastOrDefault), GroupMapper(fixings.LastOrDefault).Values.Where(Function(group) group.SVGType = SvgType.Undefined.ToString).FirstOrDefault, Nothing)
        Dim fixingPosition1 As VectorDraw.Geometry.gPoint = If(fixing1 IsNot Nothing, fixing1.BoundingBox.MidPoint, Nothing)
        Dim fixingPosition2 As VectorDraw.Geometry.gPoint = If(fixing2 IsNot Nothing, fixing2.BoundingBox.MidPoint, Nothing)
        Dim highlightPolylines As New List(Of VdPolylineEx)
        Dim leftToRight As Boolean = True

        Dim segmentGroup As VdSVGGroup = If(GroupMapper.ContainsKey(segment.SystemId), GroupMapper(segment.SystemId).Values.Where(Function(group) group.SVGType = SvgType.Undefined.ToString).FirstOrDefault, Nothing)
        If (segmentGroup Is Nothing OrElse fixingPosition1 Is Nothing) Then
            Return highlightPolylines
        End If

        Dim segHeight As Double = segmentGroup.BoundingBox.Height
        Dim segWidth As Double = segmentGroup.BoundingBox.Width

        Dim vertexPosition As VectorDraw.Geometry.gPoint = Nothing

        If (fixingPosition2 IsNot Nothing) Then
            If (segWidth > segHeight AndAlso fixingPosition1.x > fixingPosition2.x) OrElse (segWidth < segHeight AndAlso fixingPosition1.y > fixingPosition2.y) Then
                leftToRight = False
            End If
        Else
            Dim intermediateVertexPosition As VectorDraw.Geometry.gPoint = If(intermediateVertexPositions.ContainsKey(segment.Start_node), intermediateVertexPositions(segment.Start_node), If(intermediateVertexPositions.ContainsKey(segment.End_node), intermediateVertexPositions(segment.End_node), Nothing))
            If (intermediateVertexPosition IsNot Nothing) AndAlso ((segWidth > segHeight AndAlso fixingPosition1.x > intermediateVertexPosition.x) OrElse (segWidth < segHeight AndAlso fixingPosition1.y > intermediateVertexPosition.y)) Then
                leftToRight = False
            End If

            vertexPosition = If(Not intermediateVertexPositions.ContainsKey(segment.Start_node), If(GroupMapper.ContainsKey(segment.Start_node), GroupMapper(segment.Start_node).Values.Where(Function(group) group.SVGType = SvgType.Undefined.ToString).FirstOrDefault.BoundingBox.MidPoint, Nothing), If(Not intermediateVertexPositions.ContainsKey(segment.End_node), If(GroupMapper.ContainsKey(segment.End_node), GroupMapper(segment.End_node).Values.Where(Function(group) group.SVGType = SvgType.Undefined.ToString).FirstOrDefault.BoundingBox.MidPoint, Nothing), Nothing))
        End If

        Dim closestPolylineToFixing As VdPolylineEx = Nothing
        Dim distanceToPolylineFromFixing As Double = Double.MaxValue
        Dim svgLinePolylineElements As New List(Of Object)

        GetSVGLineAndPolylineElementsFromSegmentGroup_Recursively(segmentGroup, svgLinePolylineElements)

        For Each svgLinePolylineElement As Object In svgLinePolylineElements
            Dim polyline As New VdPolylineEx(vdCanvas.BaseControl.ActiveDocument)
            With polyline
                .setDocumentDefaults()
                .PenColor.SystemColor = Color.Magenta

                If (TypeOf svgLinePolylineElement Is VdLineEx) Then
                    With DirectCast(svgLinePolylineElement, VdLineEx)
                        polyline.VertexList.Add(New VectorDraw.Geometry.gPoint(.StartPoint.x, .StartPoint.y))
                        polyline.VertexList.Add(New VectorDraw.Geometry.gPoint(.EndPoint.x, .EndPoint.y))
                    End With
                Else
                    With DirectCast(svgLinePolylineElement, VdPolylineEx)
                        polyline.VertexList.AddRange(.VertexList)
                    End With
                End If

                Using mirrorMatrix As New VectorDraw.Geometry.Matrix
                    mirrorMatrix.A11 = -mirrorMatrix.A11

                    .Transformby(mirrorMatrix)
                End Using
            End With

            Dim midPoint As VectorDraw.Geometry.gPoint = polyline.BoundingBox.MidPoint

            If (leftToRight) Then
                If (fixingPosition2 Is Nothing) Then
                    If (svgLinePolylineElements.Count = 1) OrElse ((segWidth > segHeight AndAlso midPoint.x > fixingPosition1.x) OrElse (segWidth < segHeight AndAlso midPoint.y > fixingPosition1.y)) Then
                        Dim distToPolylineFromFixing As Double = polyline.getClosestPointTo(fixingPosition1).Distance2D(fixingPosition1)
                        If (distToPolylineFromFixing < distanceToPolylineFromFixing) Then
                            closestPolylineToFixing = polyline
                            distanceToPolylineFromFixing = distToPolylineFromFixing
                        End If

                        highlightPolylines.Add(polyline)
                    End If
                Else
                    If (svgLinePolylineElements.Count = 1) OrElse ((segWidth > segHeight AndAlso midPoint.x > fixingPosition1.x AndAlso midPoint.x < fixingPosition2.x) OrElse (segWidth < segHeight AndAlso midPoint.y > fixingPosition1.y AndAlso midPoint.y < fixingPosition2.y)) Then
                        highlightPolylines.Add(polyline)
                    End If
                End If
            Else
                If (fixingPosition2 Is Nothing) Then
                    If (svgLinePolylineElements.Count = 1) OrElse ((segWidth > segHeight AndAlso midPoint.x < fixingPosition1.x) OrElse (segWidth < segHeight AndAlso midPoint.y < fixingPosition1.y)) Then
                        Dim distToPolylineFromFixing As Double = polyline.getClosestPointTo(fixingPosition1).Distance2D(fixingPosition1)
                        If (distToPolylineFromFixing < distanceToPolylineFromFixing) Then
                            closestPolylineToFixing = polyline
                            distanceToPolylineFromFixing = distToPolylineFromFixing
                        End If

                        highlightPolylines.Add(polyline)
                    End If
                Else
                    If (svgLinePolylineElements.Count = 1) OrElse ((segWidth > segHeight AndAlso midPoint.x < fixingPosition1.x AndAlso midPoint.x > fixingPosition2.x) OrElse (segWidth < segHeight AndAlso midPoint.y < fixingPosition1.y AndAlso midPoint.y > fixingPosition2.y)) Then
                        highlightPolylines.Add(polyline)
                    End If
                End If
            End If
        Next

        If (fixingPosition1 IsNot Nothing) AndAlso (closestPolylineToFixing IsNot Nothing) Then
            Dim subPolyline As VectorDraw.Professional.vdPrimaries.vdFigure = Nothing

            closestPolylineToFixing.Break(fixingPosition1, If(vertexPosition IsNot Nothing, vertexPosition, closestPolylineToFixing.BoundingBox.UpperLeft), subPolyline)

            If (subPolyline IsNot Nothing) Then
                highlightPolylines.Remove(closestPolylineToFixing)

                Dim polyline As New VdPolylineEx(vdCanvas.BaseControl.ActiveDocument)
                With polyline
                    .setDocumentDefaults()
                    .PenColor.SystemColor = Color.Magenta
                    .VertexList = DirectCast(subPolyline, vdPolyline).VertexList
                End With

                highlightPolylines.Add(polyline)
            End If
        End If

        Return highlightPolylines
    End Function

    Private Sub GetSVGLineAndPolylineElementsFromSegmentGroup_Recursively(group As VdSVGGroup, ByRef svgLinePolylineElements As List(Of Object))
        For Each figure As Object In group.Figures
            If TypeOf figure Is VdLineEx OrElse TypeOf figure Is VdPolylineEx Then
                svgLinePolylineElements.Add(figure)
            End If
        Next

        For Each childGroup As VdSVGGroup In group.ChildGroups
            GetSVGLineAndPolylineElementsFromSegmentGroup_Recursively(childGroup, svgLinePolylineElements)
        Next
    End Sub

    Private Sub _vdBaseControl_SizeChanged(sender As Object, e As EventArgs) Handles _vdBaseControl.SizeChanged
        If MagnifierActivated AndAlso _documentForm.MagnifierAction IsNot Nothing Then
            _documentForm.MagnifierAction.Scale = CalculateScaleFac4Magnifier()
        End If
    End Sub

    Private Sub _vdBaseControl_MouseEnter(sender As Object, e As EventArgs) Handles _vdBaseControl.MouseEnter
        _documentForm.CheckPanOrMagnifierActions()
    End Sub

    Private Sub _vdBaseControl_AfterModifyObject(sender As Object, propertyname As String) Handles _vdBaseControl.AfterModifyObject
        Dim vdRenderView As IvdRenderingView = TryCast(sender, IvdRenderingView)
        If vdRenderView IsNot Nothing Then
            If propertyname = NameOf(vdDocument.World2ViewMatrix) Then
                If Object.ReferenceEquals(_vdBaseControl.ActiveDocument.ActiveRender, vdRenderView.Render) Then
                    Me.vdCanvas.UpdateScrollExtends()
                End If
            End If
        End If
    End Sub

    Private Sub _drawingEntitiesLazyProgressTimer_Elapsed(sender As Object, e As ElapsedEventArgs) Handles _lazyDrawingEntitiesProgressTimer.Elapsed
        _canLazyDrawEntities = True
    End Sub

    Private Sub _vdBaseControl_Draw(sender As Object, render As vdRender, ByRef cancel As Boolean) Handles _vdBaseControl.Draw
        If TypeOf render Is MultiRender Then ' HINT: this is the renderer of the document entities, all others (selection, etc.) are skipped because we need the call before rendering the normal entities
            ResumeBoundingBoxCalculation()
            If Not _redliningsAndQmStampsHaveBeenUpdatedFromDisplayDrawing Then
                TryUpdateQmStampsAndRedlinings() ' when the update was not done before (normally at DisplayDrawing) we will do it now because the BB is now available on all entities
            ElseIf _redliningsDirty.Dirty Then
                UpdateRedlinings(_redliningsDirty.RemovePreviousSelection)
            End If
        End If
    End Sub

    Private Class RedliningDirtyInfo
        Public Sub New(dirty As Boolean)
            Me.Dirty = dirty
        End Sub

        Public Sub New(dirty As Boolean, removePreviousSelection As Boolean)
            Me.Dirty = dirty
            Me.RemovePreviousSelection = removePreviousSelection
        End Sub

        Property Dirty As Boolean
        Property RemovePreviousSelection As Boolean

        Friend Sub Reset()
            Dirty = False
            RemovePreviousSelection = False
        End Sub

    End Class

End Class