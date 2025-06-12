Imports System.ComponentModel
Imports devDept.Eyeshot
Imports devDept.Eyeshot.Entities
Imports devDept.Eyeshot.Workspace
Imports devDept.Geometry
Imports Zuken.E3.HarnessAnalyzer.Project.Documents
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.HarnessAnalyzer.Shared.Common
Imports Zuken.E3.Lib.Eyeshot.Model
Imports Zuken.E3.Lib.IO.Files
Imports Zuken.E3.Lib.Model

Namespace D3D.Document.Controls

    <Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
    Public Class Document3DControl

        Public Event SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Public Event PanningCancelled(sender As Object, e As EventArgs)
        Public Event ActionModeChanged(sender As Object, e As ActionModeChangedEventArgs)
        Public Event SelectRowsInInformationHub(sender As Object, e As EventArgs)

        Private Const RUBBER_LINE_WEIGHT As Single = 2
        Private Const NEARPLANE_DISTANCE As Integer = 800

        Private _selectEventsEnabled As Boolean = True
        Private _isSelecting As Boolean = False
        Private _oldActionMode As Nullable(Of actionType)
        Private _isPanning As Boolean
        Private _spliceProposalsHighlightClearAllowed As Boolean = True
        Private _rubberLines As New List(Of Entity)
        Private _busyState As New System.Threading.LockStateMachine
        Private _proposalLabels As New Dictionary(Of String, IndicatorAnnotation)

        Private _hintLabels As New Dictionary(Of String, IndicatorAnnotation)
        Private _clockSymbolLabels As New Dictionary(Of String, IndicatorAnnotation)
        Private _secDimObjects As New Dictionary(Of String, List(Of SecDimObject))
        Private _dimensionLabels As New Dictionary(Of String, List(Of IndicatorAnnotation))
        Private _redlinings As New Dictionary(Of String, List(Of RedliningAnnotation))

        Private _secDimObjecta As New Dictionary(Of String, List(Of SecDimObject))
        Private _labelsAreVisible As Boolean
        Private _selectedLabel As LeaderAndTextEx = Nothing
        Private _mouseEventArgs As MouseEventArgs
        Private _mouseDown As Boolean = False
        Private _isMouseMoveDetect As Boolean

        Private _previousSelectedId As String = String.Empty
        Private _isSelectingFromProperties As Boolean = False

        Private _startViewVertex As NodeEntity
        Private _startViewVertexLabel As IndicatorAnnotation

        Private _endViewVertex As NodeEntity
        Private _endViewVertexLabel As IndicatorAnnotation
        Private WithEvents _document As HcvDocument
        Friend WithEvents _d3d As DocumentDesign
        Private WithEvents _ttManager As EntityToolTipsManager

        Private _actionMode As actionType
        Private _isHiddenSelection As Boolean
        Private _generalSettings As GeneralSettingsBase

        Private _lock As New System.Threading.SemaphoreSingle
        Private _currentActionMode As actionType
        Private _zoomedOut As Boolean

        Public LabelsAreVisible As Boolean

        Public Sub New(generalSettings As GeneralSettingsBase)
            InitializeComponent()
            _generalSettings = generalSettings
            _d3d = Model3DControl1.Design

            AddHandler Model3DControl1.SelectRedlining, AddressOf SelectRedlining
            AddHandler Model3DControl1.HideIndicator, AddressOf HideIndicator
            AddHandler Model3DControl1.ShowIndicator, AddressOf ShowIndicator
            AddHandler Model3DControl1.ShowAllIndicators, AddressOf ShowAllIndicators

            With Model3DControl1.Design
                .Viewports(0).ToolBar.Visible = False
                .LineTypes.Add(devDept.Eyeshot.LineTypes.DASH_DOT1)
                .MultipleSelection = True 'HINT: this selection is set to true because our derived eyeshot control is behaving to use this property as enabled multiselect and disabled multiselect but the standard eyeshot control is not behaving like this: eyeshot is not executing a remove on selectionChanged-Event when this property is set to true like a always-pressed-ctrl-key
            End With

            _ttManager = New EntityToolTipsManager(Me)

            _actionMode = Model3DControl1.Design.ActionMode
            Model3DControl1.InitContextMenu(generalSettings)
        End Sub
        Private Sub ShowIndicator(id As String)
            ShowDimension(id)
            ShowHintLabel(id)
            ShowClockSymbol(id)
            ShowSecDimObject(id)
        End Sub
        Private Sub HideIndicator(id As String)
            HideDimension(id)
            HideHintLabel(id)
            HideClockSymbol(id)
            HideSecDimObject(id)
        End Sub
        Private Sub ShowAllIndicators()
            ShowAllDimensions()
            ShowAllHintLabels()
            ShowAllClockSymbols()
            ShowAllSecDimObjects()
        End Sub


        Public Property IsHiddenSelection As Boolean
            Get
                Return _isHiddenSelection
            End Get
            Set(value As Boolean)
                _isHiddenSelection = value

            End Set
        End Property

        Friend Sub OnOpening()
            _busyState.Push()
        End Sub

        Friend Sub OnOpened()
            _busyState.Pop()
        End Sub

        ''' <summary>
        ''' Tries to load additional JT-data over the document but passes progress-informtion in eyeshot-control by using WorkUnits
        ''' </summary>
        ''' <param name="settings">Leave empty for default settings</param>
        ''' <returns></returns>
        Public Async Function TryLoadJTDataAsync(Optional settings As ContentSettings = Nothing) As Task(Of IResult)
            If Document Is Nothing Then
                Throw New Exception("Can't load JT data from document: no document set!")
            End If

            If Not Document.LoadedContent.HasFlag(LoadContentType.JTData) Then
                Using Await _busyState.BeginWaitAsync 'HINT: Also tells the document that it is busy throu the adapter-interface (view is busy) -> all related BL will result now that the document is also busy -> we do the busy-block before view-work is started to be atomic safe between switching from document-work to starting view-work (= adding/updating entities to model asyc)
                    Using Model3DControl1.Design.EnableWaitCursor(WaitCursorEx.WaitCursorExStyle.AppStarting)
                        Using loader As HcvDocument.PreparedWork = Document.PrepareContentWork(LoadContentType.JTData, settings)
                            Dim res As IResult = Await loader.LoadAsync()
                            If res.IsSuccess AndAlso Not res.IsCancelled Then
                                Dim resAdd As IResult = Await Model3DControl1.Design.Entities.AddRangeAsync(Document.Entities, UseWorkerOptions.True) ' HINT: we can do the add on the full entities collection because the addRange only adds (internally) entities that are not already added to the model
                                If Not resAdd.IsFaulted Then ' HINT: can be cancelled when not document entities where added
                                    Dim updateRes As AggregatedResult = Await Model3DControl1.Design.Entities.UpdateAsync(EntityUpdateType.All, UseWorkerOptions.True, UpdateRegenOptions.True)
                                    If updateRes.IsSuccess Then
                                        Model3DControl1.Design.Invalidate()
                                    End If
                                    Return updateRes
                                End If
                            End If
                            Return res
                        End Using
                    End Using
                End Using
            End If

            Return Result.Cancelled("Can't load jt-data again: jt-data has been already loaded in the document!")
        End Function

        Friend Async Sub OnAfterFilterActiveObjects()
            Using Await _lock.BeginWaitAsync
                _d3d.ResetHiddenEntities()
                ClearSpliceHighlightAndRubberLines()
                Await Me.UpdateVisiblityInD3DView()
            End Using
        End Sub

        Private Async Function UpdateVisiblityInD3DView() As Task
            If Me.Document IsNot Nothing AndAlso Me.DocumentForm IsNot Nothing Then
                If Not Me.Document.IsBusy Then
                    Await Me.Document.DoWork(New UpdateEntitiesVisibilityWork("Updating Filters", System.ComponentModel.ProgressState.Updating, Me.DocumentForm.RowFilters))
                End If
            End If
        End Function

        Public ReadOnly Property Document As HcvDocument
            Get
                Return _document
            End Get
        End Property

        Public ReadOnly Property DocumentForm As DocumentForm
            Get
                Return Me.FindParent(Of DocumentForm)
            End Get
        End Property

        Public Sub CloseAllToolTips(Optional forcePinned As Boolean = False)
            _ttManager.CloseAll(forcePinned, delayed:=False)
        End Sub

        Private Sub Model3DControl1_EmptySpaceClicked(sender As Object, e As EventArgs) Handles Model3DControl1.EmptySpaceClicked
            CloseAllToolTips()
        End Sub

        Private Sub Model3DControl1_MouseEnterEntityEntity(sender As Object, e As MouseEntityEventArgs) Handles Model3DControl1.MouseEnterEntity
            If _generalSettings IsNot Nothing AndAlso _generalSettings.AutomaticTooltips Then
                _ttManager.Show(e.Entity, Document?.Model, ToolTipCaptionContextMenuStrip, Nothing, Control.MousePosition, True)
            End If
        End Sub

        Private Sub Model3DControl1_MouseLeaveEntityEntity(sender As Object, e As MouseEntityEventArgs) Handles Model3DControl1.MouseLeaveEntity
            _ttManager.Close(e.Entity, delayed:=Not e.LostFocus)
        End Sub

        Private Sub _toolTip_Hidden(sender As Object, e As ToolTipExEventArgs) Handles _ttManager.Hidden
            _spliceProposalsHighlightClearAllowed = Not _ttManager.IsAnyVisible
        End Sub

        Private Sub _toolTip_BeforeHide(sender As Object, e As CancelToolTipExInfoEventArgs) Handles _ttManager.BeforeHide
            If TypeOf e.ToolTip.Control Is ToolTips.EntityToolTipControl Then
                Dim popUpControl As ToolTips.EntityToolTipControl = CType(e.ToolTip.Control, ToolTips.EntityToolTipControl)
                e.Cancel = _isSelectingFromProperties OrElse Not popUpControl.IsAllowedToClose OrElse (popUpControl?.Entity IsNot Nothing AndAlso Me.Model3DControl1.Design.GetEntityUnderMouseCursor() Is popUpControl.Entity)
            End If
        End Sub

        Private Sub _toolTip_BeforeShown(sender As Object, e As CancelToolTipExInfoEventArgs) Handles _ttManager.BeforeShown
            _spliceProposalsHighlightClearAllowed = False
        End Sub

        Friend Sub ZoomFit(entities As IEnumerable(Of Entity), Optional fitLabels As Boolean = False)
            Me.Model3DControl1.Design.ActiveViewport.ZoomFit(entities.ToList, False, Camera.perspectiveFitType.Accurate, CalculateMargin(entities.OfType(Of IBaseModelEntityEx).ToArray), fitLabels)
        End Sub

        Friend Sub ZoomFit()
            Me.ZoomFit(Model3DControl1.Design.Entities)
        End Sub

        Friend Sub SetZoomWindowActionMode()
            CancelPanActionMode()
            Me.Model3DControl1.Design.SetButtonActionMode(Of ZoomWindowToolBarButton)()
        End Sub

        Friend Sub SetMagnifyActionMode(enabled As Boolean)
            If enabled AndAlso Model3DControl1.Design.ActionMode <> actionType.MagnifyingGlass Then
                CancelPanActionMode()
                _oldActionMode = Model3DControl1.Design.ActionMode
                Me.Model3DControl1.Design.SetButtonActionMode(Of MagnifyingGlassToolBarButton)()
            ElseIf enabled = False AndAlso Model3DControl1.Design.ActionMode = actionType.MagnifyingGlass Then
                Me.Model3DControl1.Design.ActionMode = _oldActionMode.GetValueOrDefault(D3D.Shared.DEFAULT_ACTION_MODE)
            End If
        End Sub

        Friend Sub StartPanActionMode()
            If Model3DControl1.Design.ActionMode <> actionType.Pan Then
                _oldActionMode = Model3DControl1.Design.ActionMode
                _isPanning = True
                Me.Model3DControl1.Design.SetButtonActionMode(Of PanToolBarButton)(False)
            End If
        End Sub

        Friend Sub CancelPanActionMode()
            If Model3DControl1.Design.ActionMode = actionType.Pan AndAlso _oldActionMode.HasValue Then
                Model3DControl1.Design.ActionMode = _oldActionMode.Value
            End If

            Dim oldIsPanning As Boolean = _isPanning
            _isPanning = False
            _oldActionMode = Nothing

            If oldIsPanning = True Then
                OnPanningCancelled(New EventArgs)
            End If
        End Sub

        Private Sub Model3DControl1_ActionModelChanged(sender As Object, e As ActionModeChangedEventArgs) Handles Model3DControl1.ActionModelChanged
            If _isPanning AndAlso e.Action <> actionType.Pan Then
                CancelPanActionMode()
            End If
            OnActionModeChanged(e)
        End Sub

        Protected Overridable Sub OnPanningCancelled(e As EventArgs)
            RaiseEvent PanningCancelled(Me, e)
        End Sub

        Protected Overridable Sub OnActionModeChanged(e As ActionModeChangedEventArgs)
            If e.Action = actionType.Rotate Then
                If My.Application.MainForm.GeneralSettings.UseSelectionCenterForRotation AndAlso SelectedEntities.Count > 0 Then
                    Dim center As Point3D = SelectedEntities.GetBoundingBox().GetCenter()

                    Model3DControl1.Design.ActiveViewport.Rotate.RotationCenter = rotationCenterType.Point
                    Model3DControl1.Design.ActiveViewport.Rotate.Center = center
                Else
                    Model3DControl1.Design.ActiveViewport.Rotate.RotationCenter = rotationCenterType.ViewportCenter
                End If
            End If
            RaiseEvent ActionModeChanged(Me, e)
        End Sub

        Protected Overridable Sub OnSelectionChanged(e As SelectionChangedEventArgs)
            If _selectEventsEnabled Then
                _isSelecting = True
                RaiseEvent SelectionChanged(Me, e)
                _isSelecting = False
            End If
        End Sub

        Private Sub Model3DControl1_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles Model3DControl1.SelectionChanged
            For Each tooltip As ToolTipControlEx In _ttManager.ToArray
                Dim popUpControl As ToolTips.EntityToolTipControl = TryCast(tooltip.Control, ToolTips.EntityToolTipControl)
                If popUpControl IsNot Nothing AndAlso Not tooltip.IsPinned Then
                    If popUpControl.Entity IsNot Nothing AndAlso Not e.AddedItems.Select(Function(item) item.Item).OfType(Of IEntity).Contains(popUpControl.Entity) AndAlso tooltip.IsVisible Then
                        tooltip.Close(delayed:=False)
                    End If
                End If
            Next

            ClearSpliceHighlightAndRubberLines()
            OnSelectionChanged(e)
        End Sub

        Public ReadOnly Property SelectedEntities As ISelectedEntitiesCollection
            Get
                Return Model3DControl1.SelectedEntities
            End Get
        End Property

        Public Function SelectEntitiesByKbl(kblIds As IEnumerable(Of String)) As Boolean
            If Not _isSelecting Then
                _selectEventsEnabled = False
                _isSelecting = True
                Try
                    SelectedEntities.Clear()
                    SelectedEntities.AddRange(Get3DEntitiesByKblIds(kblIds))
                    Return True
                Finally
                    _selectEventsEnabled = True
                    _isSelecting = False
                End Try
            End If
            Return False
        End Function

        Public ReadOnly Property IsSelecting As Boolean
            Get
                Return _isSelecting
            End Get
        End Property

        Public Sub ZoomFitSelection()
            If Me.SelectedEntities.Count > 0 Then
                ZoomFit(Me.SelectedEntities.OfType(Of Entity))
            End If
        End Sub

        Private Function CalculateMargin(ParamArray entities As IBaseModelEntityEx()) As Integer
            Dim margin As Integer = If(entities.Length = 1, 400, 100)
            If entities.Length = 1 Then
                Select Case entities.Single.EntityType
                    Case ModelEntityType.Node, ModelEntityType.Connector, ModelEntityType.Fixing, ModelEntityType.Eylet, ModelEntityType.Splice
                        margin = 2500
                End Select
            End If
            Return margin
        End Function

        Public Shadows Sub Invalidate()
            If Model3DControl1 IsNot Nothing AndAlso Not Model3DControl1.IsDisposed Then
                If Model3DControl1.Design IsNot Nothing AndAlso Not Model3DControl1.Design.IsDisposed Then
                    Model3DControl1.Design.UpdateVisibleSelection()
                    Model3DControl1.Design.Invalidate()
                End If
            End If
            MyBase.Invalidate()
        End Sub

        Private Function Get3DEntitiesByKblIds(ids As IEnumerable(Of String)) As IEnumerable(Of Entity)
            Dim lst As New HashSet(Of Entity)
            If Me.Document IsNot Nothing Then
                For Each id As String In ids
                    lst.AddRange(Me.Document.Entities.GetByKblIds(id).OfType(Of Entity))
                Next
            End If
            Return lst
        End Function

        Private Sub CopyCaptionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyCaptionToolStripMenuItem.Click
            Dim c As Control = TryCast(TryCast(sender, ToolStripMenuItem)?.Owner, ContextMenuStrip)?.SourceControl
            If c IsNot Nothing Then
                Clipboard.SetText(c.Text)
            End If
        End Sub

        Private Sub Model3DControl1_NeedsEEObjectData(sender As Object, e As EEObjectDataEventArgs) Handles Model3DControl1.NeedsEEObjectData
            ' HINT: override the isActive-variant check from to default to our specific document variant which is used to control the disabled/enabled-state of the objects by the document (see Document.ISACTIVE_VARIANT_NAME)
            HcvDocument.ProcessNeedsEEObjectData(e, Me.Document)
        End Sub

        Friend Sub ClearSpliceHighlight()
            If _spliceProposalsHighlightClearAllowed Then
                If Model3DControl1?.Design IsNot Nothing AndAlso _document?.Entities IsNot Nothing Then
                    If (_document?.IsOpen).GetValueOrDefault Then
                        Model3DControl1.Design.StopAllBlinking()
                        ClearProposalLabels()
                        Model3DControl1.Design.Invalidate()
                    End If
                End If
            End If
        End Sub

        Friend Sub HighlightSpliceEntities(proposals As IEnumerable(Of VertexProposal))
            If Not proposals.Any(Function(prop) prop.IsStartSplice) Then
                Throw New ArgumentException("Proposal with start splice is missing", NameOf(proposals))
            End If

            Dim entities As New List(Of Entity)

            _document.Entities.StopAllBlinking()
            If proposals.Count > 1 Then
                entities.AddRange(_document.Entities.BlinkEntities(proposals.Where(Function(prop) Not prop.IsStartSplice).Select(Function(prop) prop.Vertex.Id).ToArray).Flatten.OfType(Of Entity))
            End If

            If proposals.Any() Then
                Dim myEntities As IEnumerable(Of BaseModelEntity) = _document.Entities.GetByEEObjectId(proposals.Where(Function(prop) prop.IsStartSplice).Select(Function(prop) prop.Vertex.Id).ToArray).OfType(Of BaseModelEntity)
                For Each ent As BaseModelEntity In myEntities
                    Dim settings As New BlinkEntitySettings(BlinkStyle.TempEntity, Model3DControl1.Design.ActiveViewport.Background.GetContrastColorInverted)
                    ent.SetBlinkState(True, settings)
                    entities.Add(ent)
                Next
            End If

            ZoomFit(entities, fitLabels:=True)
            Model3DControl1.Design.Invalidate()
        End Sub

        Friend Sub ShowCorrespondingRubberLines(cavity As E3.Lib.Model.Cavity)
            If (_document?.IsOpen).GetValueOrDefault Then
                ClearRubberLines()
                Dim startCavVtx As E3.Lib.Model.Vertex = cavity.GetConnector?.GetVertices().Entries.Where(Function(vtx) vtx.IsInActiveVariant).FirstOrDefault
                Dim startNode As NodeEntity = _document.Entities.GetByEEObjectId((startCavVtx?.Id).GetValueOrDefault).OfType(Of NodeEntity).SingleOrDefault
                If startNode IsNot Nothing Then
                    For Each wireCavNode As NodeEntity In _document.Entities.GetByEEObjectId(GetOtherRoutedCavities(cavity).SelectMany(Function(cav) cav.GetConnector?.GetVertices.Entries.Select(Function(vtx) vtx.Id)).ToArray).OfType(Of NodeEntity)
                        Dim line As New Line(startNode.Position, wireCavNode.Position) With {.LineTypeName = devDept.Eyeshot.LineTypes.DASH_DOT1.Name, .LineTypeMethod = colorMethodType.byEntity, .LineWeight = RUBBER_LINE_WEIGHT, .Color = Model3DControl1.Design.ActiveViewport.Background.GetContrastColor, .ColorMethod = colorMethodType.byEntity}
                        For Each entity As Entity In line.GetLineTypeVerticesAndPoints(Me.Model3DControl1.Design) 'HINT: no use of overlay because the overlay is done in in screen coordinates (which is complicated to calculate from world to screen) and on every zoom&camera change is must be recalculated newly
                            _rubberLines.Add(entity)
                            Model3DControl1.Design.TempEntities.Add(entity)
                        Next
                    Next
                    Model3DControl1.Design.Invalidate()
                End If
            End If
        End Sub

        Friend Sub ClearRubberLines()
            If Model3DControl1?.Design IsNot Nothing AndAlso Not (Model3DControl1?.Design.IsDisposed).GetValueOrDefault AndAlso _rubberLines IsNot Nothing Then
                For Each rLine As Entity In _rubberLines
                    Me.Model3DControl1.Design.TempEntities.Remove(rLine)
                Next
                _rubberLines.Clear()
            End If
        End Sub

        Private Function GetOtherRoutedCavities(cavity As E3.Lib.Model.Cavity) As IEnumerable(Of E3.Lib.Model.Cavity)
            Dim otherCavities As New HashSet(Of E3.Lib.Model.Cavity)
            If (cavity.GetConnector?.IsInActiveVariant).GetValueOrDefault Then
                For Each activeWire As E3.Lib.Model.Wire In cavity.GetWires.Entries.Where(Function(w) w.IsInActiveVariant)
                    otherCavities.AddRange(activeWire.GetCavities.Entries)
                Next
            End If
            otherCavities.Remove(cavity)
            Return otherCavities
        End Function

        Private Sub Model3DControl1_ModelLostFocus(sender As Object, e As EventArgs) Handles Model3DControl1.ModelLostFocus
            If Not Me.Focused Then
                ClearSpliceHighlightAndRubberLines()
            End If
        End Sub

        Public Sub ClearSpliceHighlightAndRubberLines()
            ClearSpliceHighlight()
            ClearRubberLines()
        End Sub

        Private Sub Model3DControl1_ClearSelection(sender As Object, e As EventArgs) Handles Model3DControl1.ClearSelection
            ClearSpliceHighlightAndRubberLines()
            DeselectAllRedlinings()
        End Sub

        Protected Overrides Sub OnVisibleChanged(e As EventArgs)
            ClearSpliceHighlightAndRubberLines()
            MyBase.OnVisibleChanged(e)
        End Sub

        Friend Sub SetActiveOBjectsByKblIds(ParamArray kblIds() As String)
            If _document?.IsOpen Then
                ClearSpliceHighlightAndRubberLines()

                With _document
                    .SetModelObjectsActiveByKblIds(kblIds, False)
                    .Entities.UpdateColors()
                    .Entities.UpdateVisibleSelection()
                    .Entities.Invalidate()
                End With
            End If
        End Sub

        Private Sub Model3DControl1_KeyDown(sender As Object, e As KeyEventArgs) Handles Model3DControl1.KeyDown
            If e.KeyCode = Keys.Escape Then  ' TODO: Bug! this code will never executed
                ClearSpliceHighlightAndRubberLines()
            End If
        End Sub

        Private Sub Document3DViewer_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
            If e.KeyCode = Keys.Escape Then ' TODO: Bug! this code will never executed
                ClearSpliceHighlightAndRubberLines()
            End If
        End Sub

        Friend Sub AddProposalLabel(proposal As VertexProposal, rankMin As Integer, rankMax As Integer)
            If Not proposal.IsStartSplice Then
                'vertexIds.Add(proposal.Vertex.Id)
                Dim borderColor As System.Drawing.Color
                Dim image As System.Drawing.Bitmap = Nothing
                If proposal.Rank = rankMin Then
                    ' green/best = first
                    image = New Bitmap(My.Resources.circle_green, New System.Drawing.Size(24, 24))
                    borderColor = System.Drawing.Color.Green
                ElseIf proposal.Rank = rankMax Then
                    ' red/worst = last
                    image = New Bitmap(My.Resources.circle_red, New System.Drawing.Size(24, 24))
                    borderColor = System.Drawing.Color.Red
                Else
                    ' everything else = yellow
                    image = New Bitmap(My.Resources.circle_yellow, New System.Drawing.Size(24, 24))
                    borderColor = System.Drawing.Color.Yellow
                End If

                For Each entity As BaseModelEntity In devDept.Eyeshot.Entities.Flatten(_document.Entities.GetByEEObjectId(proposal.Vertex.Id)).Cast(Of BaseModelEntity).ToArray
                    Dim an As New IndicatorAnnotation(entity.GetBoundingBoxCenter, GetOffset(entity), String.Format(Document3DStrings.SpliceProposalText, Math.Round(proposal.LengthDelta, 0), proposal.Rank), image, BackColor)
                    entity.Annotations.Add(an)

                    an.BorderColor = borderColor
                    an.Offset = GetOffset(entity)
                    _proposalLabels.Add(entity.Id, an)
                Next
            Else
                For Each entity As BaseModelEntity In devDept.Eyeshot.Entities.Flatten(_document.Entities.GetByEEObjectId(proposal.Vertex.Id)).Cast(Of BaseModelEntity).ToArray
                    Dim an As New IndicatorAnnotation(entity.GetBoundingBoxCenter, GetOffset(entity), entity.DisplayName, Nothing, Model3DControl1.Design.Selection.ColorDynamic)
                    entity.Annotations.Add(an)
                    _proposalLabels.Add(entity.Id, an)
                Next
            End If
        End Sub

        Friend Sub AddRedlining(r As Redlining, entity As BaseModelEntity)
            Dim anchorPoint As devDept.Geometry.Point3D
            If TypeOf entity Is TubularBaseEntity Then
                anchorPoint = CType(entity, TubularBaseEntity).Evaluate(0.5)
            Else
                anchorPoint = entity.GetBoundingBox.GetCenter
            End If

            Dim an As RedliningAnnotation = New RedliningAnnotation(anchorPoint, GetOffset(entity, 10), r.GetClassificationImage, r)
            entity.Annotations.Add(an)

            If _redlinings.ContainsKey(entity.Id) Then
                _redlinings(entity.Id).Add(an)
            Else
                _redlinings.Add(entity.Id, New List(Of RedliningAnnotation)({an}))
            End If

        End Sub

        Friend Sub ClearRedliningLabels()
            For Each item As KeyValuePair(Of String, List(Of RedliningAnnotation)) In _redlinings
                Dim entity As BaseModelEntity = devDept.Eyeshot.Entities.Flatten(_document.Entities.GetByEEObjectId(New Guid(item.Key))).Cast(Of BaseModelEntity).FirstOrDefault
                If entity IsNot Nothing Then
                    For Each an As RedliningAnnotation In item.Value
                        entity.Annotations.Remove(an)
                    Next
                End If
            Next
            _redlinings.Clear()
            _d3d.Invalidate()
        End Sub

        Public Sub DeselectAllRedlinings()
            For Each item As KeyValuePair(Of String, List(Of RedliningAnnotation)) In _redlinings
                Dim entity As BaseModelEntity = devDept.Eyeshot.Entities.Flatten(_document.Entities.GetByEEObjectId(New Guid(item.Key))).Cast(Of BaseModelEntity).FirstOrDefault
                If entity IsNot Nothing Then
                    For Each an As RedliningAnnotation In item.Value
                        For Each lbl As Labels.Label In an.Labels(AnnotationBase.ViewportType.All)
                            lbl.Selected = False
                        Next
                    Next
                    entity.Selected = False
                    _d3d.SelectedEntities.Remove(entity)
                End If
            Next
        End Sub

        Friend Sub CheckAndRealignDimensionLabels()
            Dim anchors As New Dictionary(Of String, List(Of IndicatorAnnotation))
            For Each annos As List(Of IndicatorAnnotation) In _dimensionLabels.Values
                For Each anno As IndicatorAnnotation In annos
                    Dim key As String = String.Format("{0};{1};{2}", Math.Round(anno.AnchorPoint.X, 0), Math.Round(anno.AnchorPoint.Y, 0), Math.Round(anno.AnchorPoint.Z, 0))
                    If (Not anchors.ContainsKey(key)) Then
                        anchors.Add(key, New List(Of IndicatorAnnotation))
                    End If
                    anchors(key).Add(anno)
                Next
            Next

            For Each kvp As KeyValuePair(Of String, List(Of IndicatorAnnotation)) In anchors.Where(Function(e) e.Value.Count > 1)
                Dim ref As Vector2D = Nothing
                kvp.Value.Reverse() 'HINT typically, but not in all cases the sequence of drawing is in the way the labes are added. Reversing helps to avoid overlaying leaders on prolonging
                Dim proLonger As Double = 1.0
                For Each anno As IndicatorAnnotation In kvp.Value
                    If ref Is Nothing Then
                        ref = anno.Offset
                    Else
                        If (anno.Offset.Equals(ref)) Then
                            proLonger += 0.5
                            anno.Offset *= proLonger
                        End If
                    End If
                Next
            Next
        End Sub

        Private Function IsValueOk(val As Double) As Boolean
            Return Not Double.IsInfinity(val)
        End Function

        Private Function GetOffset(entity As BaseModelEntity, Optional length As Integer = 50, Optional checkAngle As Boolean = True) As devDept.Geometry.Vector2D
            Dim n As Integer = 1
            If checkAngle Then n = entity.Annotations.Count + 1

            Dim angle As Double = 30
            If n > 1 Then angle = 60
            Dim x As Double = Math.Round(Math.Sin(Math.PI * n * angle / 180) * length, 1)
            Dim y As Double = Math.Round(Math.Cos(Math.PI * n * angle / 180) * length, 1)
            Dim v As New devDept.Geometry.Vector2D(x, y)
            Return v
        End Function

        Friend Sub AddViewVertexDirections(startNode As NodeEntity, endNode As NodeEntity)
            If (startNode IsNot Nothing) Then
                _startViewVertex = startNode
                _startViewVertexLabel = New IndicatorAnnotation(_startViewVertex.Position, GetOffset(_startViewVertex), "", My.Resources.StartViewDirection)
                _startViewVertex.Annotations.Add(_startViewVertexLabel)
            End If

            If (endNode IsNot Nothing) Then
                _endViewVertex = endNode
                _endViewVertexLabel = New IndicatorAnnotation(_endViewVertex.Position, GetOffset(_endViewVertex), "", My.Resources.EndViewDirection)
                _endViewVertex.Annotations.Add(_endViewVertexLabel)
            End If
            Model3DControl1.Design.Refresh()
        End Sub

        Friend Sub ClearViewVertexDirections()
            If _startViewVertex IsNot Nothing AndAlso _startViewVertex.Annotations.Contains(_startViewVertexLabel) Then
                _startViewVertex.Annotations.Remove(_startViewVertexLabel)
            End If
            If _endViewVertex IsNot Nothing AndAlso _endViewVertex.Annotations.Contains(_endViewVertexLabel) Then
                _endViewVertex.Annotations.Remove(_endViewVertexLabel)
            End If

            Model3DControl1.Design.Refresh()
        End Sub

        Friend Sub ClearProposalLabels()
            For Each item As KeyValuePair(Of String, IndicatorAnnotation) In _proposalLabels
                Dim entity As BaseModelEntity = devDept.Eyeshot.Entities.Flatten(_document.Entities.GetByEEObjectId(New Guid(item.Key))).Cast(Of BaseModelEntity).FirstOrDefault
                If entity IsNot Nothing Then entity.Annotations.Remove(item.Value)
            Next
            _proposalLabels.Clear()
            Model3DControl1.Design.Invalidate()
        End Sub

        Private Sub _ttManager_SelectionChanged(sender As Object, e As ToolTipSelectionChangedEventArgs) Handles _ttManager.ToolTipSelectionChanged
            _isSelectingFromProperties = True
            If (String.IsNullOrEmpty(e.SelectedKblId)) Then
                Model3DControl1.Design.SelectedEntities.RemoveRange(Get3DEntitiesByKblIds(New List(Of String) From {_previousSelectedId}))
            Else
                If (e.ToolTipControl.Entity IsNot Nothing) Then
                    Model3DControl1.Design.SelectedEntities.TryAdd(CType(e.ToolTipControl.Entity, Entity))
                End If
                Model3DControl1.Design.SelectedEntities.AddRange(Get3DEntitiesByKblIds(New List(Of String) From {e.SelectedKblId}))
                Model3DControl1.Design.ZoomFitSelectedLeaves()
            End If

            Model3DControl1.Design.Refresh()
            _previousSelectedId = e.SelectedKblId
            _isSelectingFromProperties = False
        End Sub

        Private Sub HideLabels()
            For Each item As KeyValuePair(Of String, List(Of IndicatorAnnotation)) In _dimensionLabels
                For Each an As IndicatorAnnotation In item.Value
                    If Not an.IsRef Then
                        an.Visible = False
                        an.IsHidden = True
                    End If
                Next
            Next
        End Sub

        Private Sub SelectRedlining(sender As Object, e As SelectRedliningEventArgs)
            Model3DControl1.OnClearSelection(New EventArgs)
            If Not String.IsNullOrEmpty(e.Annotation.SourceId) Then
                Dim ids As New List(Of String) From {e.Annotation.SourceId}
                e.Annotation.Selected = True
                SelectEntitiesByKbl(ids)
                RaiseEvent SelectRowsInInformationHub(ids, New EventArgs)
            End If
        End Sub

        Public Sub ShowEntityToolTip(entity As IEntity, Optional delayed As Boolean = False)
            If _ttManager IsNot Nothing Then
                _ttManager.Show(entity, Document?.Model, ToolTipCaptionContextMenuStrip, Nothing, Control.MousePosition, delayed)
            End If
        End Sub

        Private Sub Document3DControl_MouseWheel(sender As Object, e As MouseEventArgs) Handles Me.MouseWheel
            If Me._d3d.ActiveViewport.Camera.Near < NEARPLANE_DISTANCE Then
                If Not _labelsAreVisible Then
                    ShowDimensionLabels(False)
                    ShowClockSymbols(False)
                    ShowSecDimObjects(False)
                    ShowHintLabels(False)
                    _labelsAreVisible = True
                End If
            Else
                If _labelsAreVisible Then
                    HideDimensionLabels()
                    HideClockSymbols()
                    HideSecDimObjects()
                    HideHintLabels()
                    _labelsAreVisible = False
                End If
            End If
        End Sub

        Private Sub _d3dDesign_MouseDown(sender As Object, e As MouseEventArgs) Handles _d3d.MouseDown
            _mouseDown = True
            _mouseEventArgs = e
            If e.Button = MouseButtons.Left Then
                _currentActionMode = _d3d.ActionMode
            End If

        End Sub

        Private Sub _d3dDesign_MouseUp(sender As Object, e As MouseEventArgs) Handles _d3d.MouseUp
            _mouseDown = False
            If e.Button = MouseButtons.Left Then


                Dim dx As Integer = Math.Abs((_mouseEventArgs?.X).GetValueOrDefault - e.Location.X)
                Dim dy As Integer = Math.Abs((_mouseEventArgs?.Y).GetValueOrDefault - e.Location.Y)
                If (dx < 2 AndAlso dy < 2) Then
                    _isMouseMoveDetect = False
                    CheckSecDimLabelSelection(e)
                Else
                    _isMouseMoveDetect = True
                End If
            End If
        End Sub

        Private Sub _d3d_MouseWheel(sender As Object, e As MouseEventArgs) Handles _d3d.MouseWheel
            If _d3d.ActiveViewport.Camera.Near < NEARPLANE_DISTANCE Then
                _zoomedOut = False
            Else
                _zoomedOut = True
            End If

            If Not _zoomedOut Then
                If Not LabelsAreVisible Then
                    ShowDimensionLabels(False)
                    ShowClockSymbols(False)
                    ShowSecDimObjects(False)
                    ShowHintLabels(False)
                    LabelsAreVisible = True
                End If
            Else
                If LabelsAreVisible Then
                    HideDimensionLabels()
                    HideClockSymbols()
                    HideSecDimObjects()
                    HideHintLabels()
                    LabelsAreVisible = False
                End If
            End If
        End Sub

        Public Property SelectedLabel As LeaderAndTextEx
            Get
                Return _selectedLabel
            End Get
            Set(value As LeaderAndTextEx)
                _selectedLabel = value
            End Set
        End Property
    End Class

End Namespace