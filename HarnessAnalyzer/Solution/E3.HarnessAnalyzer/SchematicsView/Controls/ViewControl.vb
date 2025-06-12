Imports System.Collections.Specialized
Imports System.Reflection
Imports Zuken.E3.App.Controls
Imports Zuken.E3.App.Controls.Connectivity.View
Imports Zuken.E3.HarnessAnalyzer.Schematics.Converter
Imports Zuken.E3.HarnessAnalyzer.Schematics.Converter.Kbl

Namespace Schematics.Controls

    <Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
    Public Class ViewControl
        Implements IMessageFilter

        Public Event [Error](sender As Object, e As Schematics.Converter.Kbl.ErrorEventArgs)
        Public Event ConnectorResolving(sender As Object, e As ConnectorResolvingEventArgs)
        Public Event ResolveComponentType(sender As Object, e As ComponentTypeResolveEventArgs)
        Public Event ResolveEntityId(sender As Object, e As IdEventArgs)
        Public Event SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Public Event EntityDoubleClicked(sender As Object, e As EntityEventArgs)
        Public Event EntityRightClicked(sender As Object, e As EntityMouseEventArgs)
        Public Event MouseOverItem(sender As Object, e As EntityEventArgs)
        Public Event AfterEntityCreated(sender As Object, e As EntityEventArgs)
        Public Event ActiveEntitiesChanged(sender As Object, e As EventArgs)
        Public Event ProgressChanged(sender As Object, e As ConverterProgressChangedEventArgs)
        Public Event ProgressingFinished(sender As Object, e As Converter.ConverterEventArgs)

        Private WithEvents _ctrlView As ConnectivityViewCtrl
        Private WithEvents _kblDocumentsData As New KblDocumentDataCollection
        Private WithEvents _selected As New SelectedEntities()
        Private WithEvents _activeEntities As New ActiveEntities(Me)

        Private _entities As New DocumentsCollection()
        Private _modulesSet As New Dictionary(Of String, String)
        Private _functionSet As New Dictionary(Of String, String)
        Private _currentConverterId As Guid
        Private _modelIsAttached As Boolean = False
        Private _selectInConnectivityCtrlOnSelectionCollectionChanged As Boolean = True
        Private _selectionChangedEnabled As Boolean = True

        Public Sub New()
            InitializeComponent()
            _ctrlView = New ConnectivityViewCtrl(False, True)

            With _ctrlView
                Me.UltraPanel1.ClientArea.Controls.Add(_ctrlView)
                Me.CreateControl() 'HINT: is needed for c++ to build the handle here (is needed for working with the edbmodel without showing the control for the first time)
                _ctrlView.Dock = DockStyle.Fill
                .SetColorScheme()
                .ExtractionLimit = 500
            End With
            Application.AddMessageFilter(Me)
        End Sub

        Private Sub _kblBlocks_AfterEntityCreated(sender As Object, e As EntityEventArgs) Handles _kblDocumentsData.AfterEntityCreated
            ConversionResult.ConversionResultAdapter.AfterEdbEntitiyCreated(_entities, e.Entity)
            OnAfterEntityCreated(e)
        End Sub

        Protected Overridable Sub OnAfterEntityCreated(e As EntityEventArgs)
            RaiseEvent AfterEntityCreated(Me, e)
        End Sub

        Private Sub _kblBlocks_AfterFunctionSet(sender As Object, e As FunctionSetEventArgs) Handles _kblDocumentsData.AfterFunctionSet
            _functionSet.TryAdd(e.FunctionId, e.FunctionName)
        End Sub

        Private Sub _kblBlocks_AfterModulesSet(sender As Object, e As ModuleSetEventArgs) Handles _kblDocumentsData.AfterModulesSet
            e.Modules.ForEach(Sub(md) _modulesSet.TryAdd(md.Id, md.Name))
        End Sub

        Private Sub _kblBlocks_BeforeGenerationStart(sender As Object, e As KblDocumentDataCollection.GenerationEventArgs) Handles _kblDocumentsData.BeforeGenerationStart
            Me.UseWaitCursor = True
            If _ctrlView IsNot Nothing Then
                _ctrlView.ShowComponent(String.Empty) '  HINT: this is for safety reason (although we are hiding the _ctrlview before re-generating this "clear" of content is added for safty reason to ensure avoiding a possible crash while generating and repaint/mouseover over the control)
            End If
            If Me.ProgressBarOnGenerate Then
                Me.ProgressBar.Visible = True
                Me.btnRefresh.Visible = False
                Me.btnReset.Visible = False
                Me.btnSyncSelection.Visible = False
            End If

            _entities.Clear()
            _modelIsAttached = False
            _currentConverterId = e.ConverterId
        End Sub

        Private Sub _kblBlocks_GenerationFinished(sender As Object, e As KblDocumentDataCollection.GenerationFinishedEventArgs) Handles _kblDocumentsData.GenerationFinished
            Me.btnReset.Visible = True
            Me.btnRefresh.Visible = True
            Me.ProgressBar.Visible = False
            Me.btnSyncSelection.Visible = Me.SyncButtonVisible
            Me.UseWaitCursor = False
            _currentConverterId = Guid.Empty

            If Not e.Cancelled AndAlso e.Result IsNot Nothing AndAlso e.Result.IsFinishedSuccessfully Then
                _currentConverterId = e.ConverterId
                _ctrlView.AttachModel(e.Result.Model)
                _modelIsAttached = True
                Me.Reset()
            End If
            OnProgressingFinished(New Schematics.Converter.ConverterEventArgs(e.ConverterId))
        End Sub

        Private Sub _kblBlocks_ResolveEntityId(sender As Object, e As IdEventArgs) Handles _kblDocumentsData.ResolveEntityId
            OnResolveEntityId(e)
        End Sub

        Protected Overridable Sub OnResolveEntityId(e As IdEventArgs)
            If EventsEnabled Then
                RaiseEvent ResolveEntityId(Me, e)
            End If
        End Sub

        Private Sub _kblBlocks_ConnectorResolving(sender As Object, e As ConnectorResolvingEventArgs) Handles _kblDocumentsData.ConnectorResolving
            OnConnectorResolving(e)
        End Sub

        Protected Overridable Sub OnConnectorResolving(e As ConnectorResolvingEventArgs)
            If EventsEnabled Then
                RaiseEvent ConnectorResolving(Me, e)
            End If
        End Sub

        Private Sub _kblBlocks_ResolveComponentType(sender As Object, e As ComponentTypeResolveEventArgs) Handles _kblDocumentsData.ResolveComponentType
            OnResolveComponentType(e)
        End Sub

        Protected Overridable Sub OnResolveComponentType(e As ComponentTypeResolveEventArgs)
            If EventsEnabled Then
                RaiseEvent ResolveComponentType(Me, e)
            End If
        End Sub

        Private Sub _ctrlView_DropHintEvent(sender As Object, e As Zuken.E3.App.Controls.Connectivity.View.DropHintEventArgs) Handles _ctrlView.DropHintEvent
            'HINT: Warning the exceptions here are not thrown, maybe this is a bug of windows 7 (depends on the event order from c++?). In Windows 10 the exceptions are thrown normally (as a workaround we add a try catch block with a messagebox)
            Try
                For Each entity As EdbConversionEntity In IdConverter.ResolveEntitiesFromDocuments(Me.Entities, New String() {e.DropHintId.Item1}, False)
                    If entity IsNot Nothing Then
                        OnMouseOverItem(entity)
                    End If
                Next
            Catch ex As Exception
#If DEBUG Or CONFIG = "Debug" Then
                My.Application.InitManager.ErrorManager.ShowMsgBoxExceptionOrWriteConsole(String.Empty, ex)
#End If
            End Try
        End Sub

        Protected Overridable Sub OnMouseOverItem(e As EntityEventArgs)
            If EventsEnabled Then
                RaiseEvent MouseOverItem(Me, e)
            End If
        End Sub

        Private Sub OnMouseOverItem(entity As EdbConversionEntity)
            OnMouseOverItem(New EntityEventArgs(CurrentConverterId, entity))
        End Sub

        Private Sub _ctrlView_SelectionChangedEvent(sender As Object, e As Connectivity.View.SelectionChangedEventArgs) Handles _ctrlView.SelectionChangedEvent
            ExecuteWithNotSelectionInConnectivity(Sub() ResetSelectionToWhenDifferent(IdConverter.ResolveEntitiesFromDocuments(Me.Entities, e.Selection.Select(Function(tpl) tpl.Item1).ToArray)))
        End Sub

        Private Sub UpdateSyncSelectionButton()
            If Me.Selected IsNot Nothing Then
                Me.btnSyncSelection.Enabled = Me.Selected.Count > 0
            End If
        End Sub

        Private Function ResetSelectionToWhenDifferent(newEntities As IEnumerable(Of EdbConversionEntity)) As Boolean
            Return _selected.ResetTo(newEntities, True)
        End Function

        Protected Overridable Sub OnSelectionChanged(e As Schematics.SelectionChangedEventArgs)
            If EventsEnabled Then
                RaiseEvent SelectionChanged(Me, e)
            End If
        End Sub

        Protected Overridable Sub OnProgressingFinished(e As Schematics.Converter.ConverterEventArgs)
            If EventsEnabled Then
                RaiseEvent ProgressingFinished(Me, e)
            End If
        End Sub

        Public Function ResetSelectionTo(entityIds() As String, Optional raiseSelectionChangedEvent As Boolean = True) As Boolean
            If Me.Entities IsNot Nothing Then
                Return Me.ResetSelectionTo(IdConverter.ResolveEntitiesFromDocuments(Me.Entities, entityIds, False), raiseSelectionChangedEvent)
            End If
            Return False
        End Function

        Public Function ResetSelectionTo(entities As IEnumerable(Of EdbConversionEntity), Optional raiseSelectionChangedEvent As Boolean = True) As Boolean
            If Me.Selected IsNot Nothing Then
                If Not raiseSelectionChangedEvent Then
                    Return ExecuteWithDisabledSelectionChangedEvent(Function() ResetSelectionToWhenDifferent(entities))
                Else
                    Return ResetSelectionToWhenDifferent(entities)
                End If
            End If
            Return False
        End Function

        Private Sub ExecuteWithNotSelectionInConnectivity(action As Action)
            Dim oldSelectInControl As Boolean = _selectInConnectivityCtrlOnSelectionCollectionChanged
            _selectInConnectivityCtrlOnSelectionCollectionChanged = False
            Try
                action.Invoke()
            Finally
                _selectInConnectivityCtrlOnSelectionCollectionChanged = oldSelectInControl
            End Try
        End Sub

        Private Function ExecuteWithDisabledSelectionChangedEvent(Of TResult)(action As Func(Of TResult)) As TResult
            Dim oldSelectionChanged As Boolean = _selectionChangedEnabled
            _selectionChangedEnabled = False
            Try
                Return action.Invoke()
            Finally
                _selectionChangedEnabled = oldSelectionChanged
            End Try
        End Function

        Private Sub _selected_CollectionChanged(sender As Object, e As NotifyCollectionChangedEventArgs) Handles _selected.CollectionChanged
            Select Case e.Action
                Case NotifyCollectionChangedAction.Add, NotifyCollectionChangedAction.Remove, NotifyCollectionChangedAction.Reset
                    If _selectInConnectivityCtrlOnSelectionCollectionChanged AndAlso ModelIsAvail Then
                        Dim selectListId As New HashSet(Of String)(_selected.Select(Function(entity) entity.Id))
                        For Each entity As EdbConversionEntity In _selected
                            If TypeOf entity Is EdbConversionConnectorEntity Then
                                Select Case CType(entity, EdbConversionConnectorEntity).OwnerComponent.ComponentType
                                    Case Connectivity.Model.ComponentType.Splice, Connectivity.Model.ComponentType.Eyelet ' HINT: the splice and eylet components have seperated connectors (hardcoded business logic of the kblEdbConverter, therefore it's hardcoded here-> would be better on every object some get selected- id's method ?)
                                        selectListId.Add(CType(entity, EdbConversionConnectorEntity).OwnerComponent.Id)
                                End Select
                            End If
                        Next

                        _ctrlView.Select(selectListId.ToList)
                        If AutoZoomSelection Then ZoomToSelection()
                    End If

                    UpdateSyncSelectionButton()

                    If _selectionChangedEnabled Then
                        Dim selectedItems As EdbConversionEntityInfo() = Array.Empty(Of EdbConversionEntityInfo)()

                        Select Case e.Action
                            Case NotifyCollectionChangedAction.Add, NotifyCollectionChangedAction.Remove
                                selectedItems = e.NewItems.Cast(Of EdbConversionEntityInfo).ToArray
                            Case NotifyCollectionChangedAction.Reset
                                selectedItems = _selected.ToArray
                        End Select

                        OnSelectionChanged(New SelectionChangedEventArgs(selectedItems))

                    End If
                Case Else
                    Throw New NotSupportedException(String.Format("Change selection command ""{0}"" is not supported is not supported by the underlying ""{1}""", e.Action.ToString, _ctrlView.GetType.Name))
            End Select
        End Sub

        <DebuggerNonUserCode>
        Private Function PreFilterMessage(ByRef m As Message) As Boolean Implements IMessageFilter.PreFilterMessage
            If m.HWnd = _ctrlView.Handle Then
                Select Case CType(m.Msg, System.Windows.WindowsMessage)
                    Case System.Windows.WindowsMessage.LBUTTONDBLCLK
                        Dim entity As EdbConversionEntity = CurrentHitEntity
                        If entity IsNot Nothing Then
                            OnEntityDoubleClicked(entity)
                        End If
                    Case System.Windows.WindowsMessage.RBUTTONUP
                        Dim entity As EdbConversionEntity = CurrentHitEntity
                        If entity IsNot Nothing Then
                            Dim MOUX As Integer = Form.MousePosition.X
                            Dim MOUY As Integer = Form.MousePosition.Y
                            OnEntityRightClicked(New EntityMouseEventArgs(CurrentConverterId, entity, New Point(MOUX, MOUY)))
                        End If
                End Select
            End If
            Return False
        End Function

        Private ReadOnly Property CurrentHitEntity As EdbConversionEntity
            Get
                Dim hitEnt As Tuple(Of String, Connectivity.Model.ObjType) = CurrentHitEntityId
                If hitEnt IsNot Nothing Then
                    Return IdConverter.ResolveEntitiesFromDocuments(Me.Entities, New String() {hitEnt.Item1}, False).SingleOrDefault
                End If
                Return Nothing
            End Get
        End Property

        Private ReadOnly Property CurrentHitEntityId As Tuple(Of String, Connectivity.Model.ObjType)
            Get
                Return _ctrlView.CurrentHitEntity
            End Get
        End Property

        Protected Overridable Sub OnEntityRightClicked(e As EntityMouseEventArgs)
            If EventsEnabled Then
                RaiseEvent EntityRightClicked(Me, e)
            End If
        End Sub

        Protected Overridable Sub OnEntityDoubleClicked(e As EntityEventArgs)
            If EventsEnabled Then
                RaiseEvent EntityDoubleClicked(Me, e)
            End If
        End Sub

        Private Sub OnEntityDoubleClicked(entity As EdbConversionEntity)
            OnEntityDoubleClicked(New EntityEventArgs(CurrentConverterId, entity))
        End Sub

        Public Shadows Sub Update()
            _ctrlView.ReDraw()
            MyBase.Update()
        End Sub

        Public Function Reset() As Boolean
            If Me.ActiveEntities.Count > 0 Then
                Dim ids As String() = Me.ActiveEntities.Select(Function(entity) entity.Id).ToArray
                Me.ActiveEntities.Clear()
                Me.ActiveEntities.AddNew(ids)
                Return True
            End If

            Me.Update()
            Return False
        End Function

        Private Sub btnActive_Click(sender As Object, e As EventArgs) Handles btnReset.Click
            Me.Reset()
        End Sub

        Private Sub btnRegenerate_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
            If _ctrlView IsNot Nothing Then
                _ctrlView.Regenerate()
            End If
        End Sub

        Private Sub btnSyncSelection_Click(sender As Object, e As EventArgs) Handles btnSyncSelection.Click
            Me.ZoomToSelection()
        End Sub

        Protected Overridable Sub OnProgressChanged(e As Schematics.Converter.ConverterProgressChangedEventArgs)
            If EventsEnabled Then
                RaiseEvent ProgressChanged(Me, e)
            End If
        End Sub

        Protected Overridable Sub OnActiveEntitiesChanged(e As EventArgs)
            If EventsEnabled Then
                RaiseEvent ActiveEntitiesChanged(Me, e)
            End If
        End Sub

        Private Sub _kblBlocks_ProgressChanged(sender As Object, e As Converter.ConverterProgressChangedEventArgs) Handles _kblDocumentsData.ProgressChanged
            If Me.ProgressBarOnGenerate Then
                Me.ProgressBar.Value = e.ProgressPercentage
            End If
            OnProgressChanged(e)
        End Sub

        Private Sub _kblBlocks_Error(sender As Object, e As ErrorEventArgs) Handles _kblDocumentsData.Error
            OnError(e)
        End Sub

        ReadOnly Property IsBusy As Boolean
            Get
                Return _kblDocumentsData.IsGenerating
            End Get
        End Property

        Protected Overridable Sub OnError(e As ErrorEventArgs)
            If EventsEnabled Then
                RaiseEvent Error(Me, e)
            End If
        End Sub

        Private Sub _active_Changed(sender As Object, e As NotifyCollectionChangedEventArgs) Handles _activeEntities.CollectionChanged
            If ModelIsAvail Then
                ShowActiveEntities()
            End If
            OnActiveEntitiesChanged(e)
        End Sub

        ''' <summary>
        ''' Property returns true as long ShowActiveEntities-method is executed
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property IsShowingActiveEntities2 As Boolean

        Property EventsEnabled As Boolean = True

        Private Sub ShowActiveEntities()
            _IsShowingActiveEntities2 = True
            Try
                If _ctrlView IsNot Nothing Then
                    lblNoEntities.Visible = True
                    btnReset.Enabled = False
                    btnSyncSelection.Enabled = False
                    btnRefresh.Enabled = False

                    With _ctrlView
                        For Each grp As IGrouping(Of ActiveObjectType, ActiveEntity) In Me.ActiveEntities.GroupBy(Function(ent) ent.Type)
                            Select Case grp.Key
                                Case ActiveObjectType.Wire
                                    .ShowWires(grp.Select(Function(ent) ent.Id))
                                Case ActiveObjectType.Module
                                    .ShowModules(grp.Select(Function(ent) ent.Id))
                                Case ActiveObjectType.Function
                                    .ShowFunctions(grp.Select(Function(ent) ent.Id))
                                Case ActiveObjectType.Connector
                                    .ShowConnectors(grp.Select(Function(ent) ent.Id))
                                Case ActiveObjectType.Component
                                    .ShowComponents(grp.Select(Function(ent) ent.Id))
                                Case ActiveObjectType.Cavity
                                    Throw New NotImplementedException("Cavity show method not implemented")
                                Case ActiveObjectType.Cable
                                    .ShowCables(grp.Select(Function(ent) ent.Id))
                                Case ActiveObjectType.None
                                    .ShowComponent(String.Empty)
                                Case Else
                                    Throw New NotImplementedException($"Edb-type ""{grp.Key.ToString}"" not implemented!")
                            End Select
                        Next
                    End With

                    'HINT: no enable of SyncSelection-Button because on ShowXY there is always no entity selection (selection-Change will enable the button)
                    btnReset.Enabled = Me.ActiveEntities.Count > 0 AndAlso Me.ActiveEntities.All(Function(tp) tp.Type <> ActiveObjectType.None)
                    btnRefresh.Enabled = Me.ActiveEntities.Count > 0 AndAlso Me.ActiveEntities.All(Function(tp) tp.Type <> ActiveObjectType.None)
                    lblNoEntities.Visible = Me.ActiveEntities.Count = 0 OrElse Me.ActiveEntities.All(Function(tp) tp.Type = ActiveObjectType.None)
                End If
            Finally
                _IsShowingActiveEntities2 = False
            End Try
        End Sub

        Public Sub ExportToPDF(filePath As String)
            _ctrlView.ExportToPdf(filePath)
        End Sub

        Public Sub Print()
            _ctrlView.Print()
        End Sub

        Public Sub Print(graphics As System.Drawing.Graphics)
            _ctrlView.Print(graphics.GetHdc)
        End Sub

        Public Function ZoomToSelection() As Boolean
            If _ctrlView IsNot Nothing Then
                _ctrlView.ZoomSelection()
                Return True
            End If
            Return False
        End Function

        Public Sub SetLicense()
            _ctrlView.SetLicense()
        End Sub

        Private Sub DisposeInternals(disposing As Boolean)
            Application.RemoveMessageFilter(Me)
            If disposing Then
                _kblDocumentsData.Dispose()
            End If
            _kblDocumentsData = Nothing
            _selected = Nothing

            If disposing Then
                _ctrlView.Dispose()
            End If
            _ctrlView = Nothing

            _modelIsAttached = False
            _entities = Nothing
            _modulesSet = Nothing
            _functionSet = Nothing
            _currentConverterId = Nothing
        End Sub

    End Class

End Namespace
