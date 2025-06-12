Imports System.ComponentModel
Imports System.IO
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Xml
Imports devDept.Eyeshot.Entities
Imports Infragistics.Win.UltraWinDock
Imports Infragistics.Win.UltraWinGrid
Imports Infragistics.Win.UltraWinTabbedMdi
Imports Infragistics.Win.UltraWinTabControl
Imports Infragistics.Win.UltraWinToolbars
Imports Infragistics.Win.UltraWinTree
Imports VectorDraw.Actions
Imports VectorDraw.Geometry
Imports VectorDraw.Professional
Imports VectorDraw.Professional.vdCollections
Imports VectorDraw.Professional.vdObjects
Imports VectorDraw.Professional.vdPrimaries
Imports Zuken.E3.HarnessAnalyzer.Checks.Cavities.Settings
Imports Zuken.E3.HarnessAnalyzer.Checks.Cavities.Views.Document
Imports Zuken.E3.HarnessAnalyzer.D3D
Imports Zuken.E3.HarnessAnalyzer.Project.Documents
Imports Zuken.E3.HarnessAnalyzer.QualityStamping
Imports Zuken.E3.HarnessAnalyzer.Schematics.Converter
Imports Zuken.E3.HarnessAnalyzer.Schematics.Converter.Kbl
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.HarnessAnalyzer.Shared
Imports Zuken.E3.HarnessAnalyzer.Shared.Common
Imports Zuken.E3.Lib.Eyeshot.Model
Imports Zuken.E3.Lib.IO
Imports Zuken.E3.Lib.IO.Files
Imports Zuken.E3.Lib.IO.Files.Hcv
Imports Zuken.E3.Lib.IO.KBL
Imports Zuken.E3.Lib.Model
Imports Zuken.E3.Lib.Packager.Circle
Imports Zuken.E3.Lib.Schema.Kbl.Properties

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class DocumentForm

    Friend Const TAB_DOC3D_KEY As String = "3D"

    Friend _calculatedSegmentDiameters As Dictionary(Of String, Dictionary(Of HarnessModuleConfiguration, PackagingCircle))
    Friend _harnessModuleConfigurations As HarnessModuleConfigurationCollection
    Friend _validityCheckContainer As ValidityCheckContainer

    Private _kblMapper As KblMapper
    Private _entityRedlinings As New Dictionary(Of String, Redlining)

    Friend Event HighlightEntireRoutingPath(sender As InformationHub, e As HighlightEntireRoutingPathEventArgs)
    Friend Event Message(sender As DocumentForm, e As MessageEventArgs)

    Public Event ActiveDrawingCanvasChanged(sender As Object, e As EventArgs)
    Public Event CanvasSelectionChanged(sender As Object, e As CanvasSelectionChangedEventArgs)
    Public Event HubSelectionChanged(sender As Object, e As InformationHubEventArgs)

    Friend WithEvents _D3DControl As D3D.Document.Controls.Document3DControl

    Friend WithEvents _drawingsHub As DrawingsHub
    Friend WithEvents _informationHub As InformationHub
    Friend WithEvents _logHub As LogHub
    Friend WithEvents _memolistHub As MemolistHub
    Friend WithEvents _modulesHub As ModulesHub
    Friend WithEvents _navigatorHub As NavigatorHub

    Public WithEvents MagnifierAction As VdMagnifier
    Public WithEvents PanAction As ActionUtility.ActionPan

    Private WithEvents _activeDrawingCanvas As DrawingCanvas
    Private WithEvents _hcvFile As [Lib].IO.Files.Hcv.HcvFile

    Private _lock_reset_update As New System.Threading.SemaphoreSingle
    Private _isResettingDockControls As Boolean = False

    Private _cancelRecalculation As Boolean
    Private _d3dAsyncLoadCancelSource As New D3D.Consolidated.Controls.D3DCancellationTokenSource
    Private _drawingFile As System.IO.FileInfo
    Private _dirty As Boolean
    Private _harnessConnectivity As HarnessConnectivity
    Private _hideEntitiesWithNoModules As Boolean = False
    Private _inliners As New Dictionary(Of Connector_occurrence, Connector_occurrence)
    Private _recalcLock As New SemaphoreSlim(1)
    Private _isClosingClose As Boolean = False
    Private _isWaitingToCanClose As Boolean = False
    Private _kblVersion As String
    Private _settingsForm As IHarnessAnalyzerSettingsProvider
    Private _messageEventArgs As MessageEventArgs
    Private _modulePartNumbersForActivation As List(Of String)
    Private _oldInactiveModules As List(Of String)
    Private _temporaryJunkFolder As System.IO.DirectoryInfo
    Private _spliceProposalLock As New SemaphoreSlim(1)
    Private _doNotMirror As Boolean = False
    Private _crossViewsAdapter As New WFileViewAdapterCollection(parallelize:=False) ' HINT: cross view collection that propagates adapter-commands to documentView and consolidated 3DView at once
    Private _busyState As New System.Threading.LockStateMachine
    Private _origins As New Dictionary(Of Guid, Integer)
    Private _showImplicitDims As Boolean
    Private _dimMode As DimMode
    Private _id As Guid = Guid.NewGuid
    Private _panes As DocumentToolPanesCollection

    <Flags>
    Private Enum DimMode
        None = 0
        ReferenceDims = 1
        ImplicitDims = 2
    End Enum

    Private WithEvents _document As HcvDocument
    Private WithEvents _documentStateMachine As DocumentStateMachine
    Private WithEvents _analysisStateMachine As AnalysisStateMachine
    Private WithEvents _cavitiesDocumentView As Checks.Cavities.Views.Document.DocumentView
    Private WithEvents _settings As New XmlReaderSettings
    Private WithEvents _progressWorker As IProgress(Of Integer) = New Progress(Of Integer)

    Private _compareResultGraphicalInfo As CheckedCompareResultInformation
    Private _compareResultTechnicalInfo As CheckedCompareResultInformation
    Private _memoList As Memolist
    Private _qmStamps As QMStamps
    Private _redliningInformation As RedliningInformation

    Public Event PanActionDisabled(sender As Object, e As EventArgs)
    Public Event MagnifierActionDisabled(sender As Object, e As EventArgs)

    Public Sub New(settingsForm As IHarnessAnalyzerSettingsProvider, hcv As [Lib].IO.Files.Hcv.HcvFile, Optional drawingFile As System.IO.FileInfo = Nothing, Optional doNotMirror As Boolean = False)
        InitializeComponent()
        Initialize(hcv, settingsForm) ' HINT: initialize (set the messageeventArgs) before setting the hcv because the hcv-progress-event needs the messageeventargs initialized
        _settingsForm = settingsForm
        _doNotMirror = doNotMirror
        _hcvFile = hcv
        _drawingFile = drawingFile
        IsExtendedHCV = TypeOf _hcvFile?.Owner Is [Lib].IO.Files.Hcv.XhcvFile
    End Sub

    Private Sub Initialize(hcv As HcvFile, settingsHarnessAnalyzer As IHarnessAnalyzerSettingsProvider)
        Me.BackColor = Color.White
        Me.Text = IO.Path.GetFileNameWithoutExtension(hcv.FullName)
        If TypeOf settingsHarnessAnalyzer Is Form AndAlso CType(settingsHarnessAnalyzer, Form).IsMdiContainer Then
            Me.MdiParent = TryCast(settingsHarnessAnalyzer, Form)
        End If
        _settingsForm = settingsHarnessAnalyzer
        _calculatedSegmentDiameters = New Dictionary(Of String, Dictionary(Of HarnessModuleConfiguration, PackagingCircle))
        _documentStateMachine = New DocumentStateMachine(Me)
        _analysisStateMachine = New AnalysisStateMachine(Me)
        _drawingsHub = New DrawingsHub
        _harnessModuleConfigurations = New HarnessModuleConfigurationCollection
        _informationHub = New InformationHub(settingsHarnessAnalyzer?.DiameterSettings, settingsHarnessAnalyzer?.GeneralSettings, Me, settingsHarnessAnalyzer?.QMStampSpecifications)
        _logHub = New LogHub
        _logHub.ConnectedToastManager = MainForm?.ToastManager
        _memolistHub = New MemolistHub
        _messageEventArgs = New MessageEventArgs(MessageEventArgs.MsgType.ShowProgressAndMessage)
        _modulesHub = New ModulesHub(IsTouchEnabled, settingsHarnessAnalyzer)
        _navigatorHub = New NavigatorHub(Me)
        _panes = New DocumentToolPanesCollection(udmDocument)
    End Sub

    Private Sub SelectRowsInInformationHub(sender As Object, e As EventArgs)
        Dim ids As List(Of String) = CType(sender, List(Of String))
        SelectRowsInGrids(ids, True, True)
    End Sub

    Private Sub AddOrUpdateNavigatorHubPane()
        If Me.udmDocument.ControlPanes.Exists(PaneKeys.DrawingsHub.ToString) Then
            If Not udmDocument.ControlPanes.Exists(PaneKeys.NavigatorHub.ToString) Then
                ' Add navigator hub dockable control pane on left window position below modules hub
                Dim navigatorHubPane As New DockableControlPane(PaneKeys.NavigatorHub.ToString, UIStrings.NavigatorPane_Caption)
                With navigatorHubPane
                    .Settings.CanDisplayAsMdiChild = Infragistics.Win.DefaultableBoolean.False
                    .Settings.AllowPin = Infragistics.Win.DefaultableBoolean.False
                    .Settings.Appearance.BackColor = Color.White
                    .Size = New Drawing.Size(200, 75)
                End With
                Me.udmDocument.ControlPanes.Add(navigatorHubPane)
                Me.udmDocument.PaneFromKey(PaneKeys.DrawingsHub.ToString).DockAreaPane.ChildPaneStyle = ChildPaneStyle.HorizontalSplit
                Me.udmDocument.PaneFromKey(PaneKeys.DrawingsHub.ToString).DockAreaPane.Panes.Add(navigatorHubPane)
            Else
                With udmDocument.ControlPanes(PaneKeys.NavigatorHub.ToString)
                    .Control = _navigatorHub
                End With
            End If
        End If
    End Sub

    Private Function AreInlinerPairIdentifiersMatching(inlinerIdentifier As InlinerIdentifier, value1 As String, value2 As String) As Boolean
        If inlinerIdentifier.IsMatch(value1) AndAlso inlinerIdentifier.IsMatch(value2) Then
            Dim con1 As String = inlinerIdentifier.GetConnectorRecognizer(value1)
            Dim con2 As String = inlinerIdentifier.GetConnectorRecognizer(value2)
            If Not String.IsNullOrEmpty(con1) AndAlso Not String.IsNullOrEmpty(con1) Then
                Dim pair1 As String = inlinerIdentifier.GetPairRecognizer(value1)
                Dim pair2 As String = inlinerIdentifier.GetPairRecognizer(value2)
                Return CBool(con1 = con2 AndAlso pair1 <> pair2)
            End If
            Return True
        End If
        Return False
    End Function

    Private Function SaveHcv(Optional fileName As String = Nothing) As String
        Try
            _hcvFile.CavityCheckSettings = _cavitiesDocumentView?.Model?.Settings?.GetAsContainerFile
            _hcvFile.ModuleConfigurations = GetHarnessModuleConfigurationsAsContainerFile()
            _hcvFile.Memolist = _memoList?.GetAsContainerFile
            _hcvFile.RedliningInfo = _redliningInformation?.GetAsContainerFile
            _hcvFile.QMStampInfo = _qmStamps?.GetAsContainerFile

            TryCreatePreviewImage(_hcvFile)

            If Not String.IsNullOrEmpty(fileName) Then
                _hcvFile.SaveTo(fileName, useTempIntermediateFile:=True)
            Else
                _hcvFile.Save(useTempIntermediateFile:=True) ' hint use temp intermediate to avoid corrupting zip archives while saving them -> when temp files is used the current file will only be overwritten when saving was successfull
            End If

            Return String.Empty
        Catch ex As Exception
#If DEBUG Or CONFIG = "Debug" Then
            Throw
#Else
            Return ex.Message
#End If
        End Try
    End Function

    Private Function TryCreatePreviewImage(hcv As HcvFile) As Boolean
        If Not hcv.OfType(Of PreviewImageContainerFile).Any() AndAlso (_activeDrawingCanvas?.vdCanvas?.BaseControl?.ActiveDocument IsNot Nothing) Then
            Dim previewImgPath As String = IO.PathEx.GetTempFileName(IO.Path.GetExtension(PreviewImageContainerFile.FILE_NAME))
            Try
                If _activeDrawingCanvas?.vdCanvas?.BaseControl?.ActiveDocument IsNot Nothing Then
                    _activeDrawingCanvas.vdCanvas.BaseControl.ActiveDocument?.ExportToFile(previewImgPath)
                    Using bmp As Image = Image.FromFile(previewImgPath)
                        Dim ct As New PreviewImageContainerFile(bmp.ToArray)
                        hcv.Add(ct)
                    End Using
                    Return True
                End If
            Catch ex As Exception
#If DEBUG Or CONFIG = "Debug" Then
                Throw New Exception("Error creating preview image: " + ex.GetInnerOrDefaultMessage, ex)
#Else
                Return False
#End If
            Finally
                System.IO.FileEx.TryDelete(previewImgPath)
            End Try
        End If
        Return False
    End Function

    Private Function ContainsConnectorTableGroupHighlightingWireRow_Recursively(group As VdSVGGroup, wireId As String) As Boolean
        If (group.KblId = wireId AndAlso group.SVGType = SvgType.row.ToString) OrElse (group.SecondaryKblIds.Contains(wireId) AndAlso group.SVGType = SvgType.row.ToString) Then Return True

        Dim retVal As Boolean = False

        For Each childGroup As VdSVGGroup In group.ChildGroups
            If (ContainsConnectorTableGroupHighlightingWireRow_Recursively(childGroup, wireId)) Then
                retVal = True

                Exit For
            End If
        Next
        Return retVal
    End Function

    Private Function ConvertToDocumentIdPairs(ids As IEnumerable(Of String)) As List(Of KeyValuePair(Of String, String))
        Dim myIds As List(Of String) = ids.ToList
        Dim resPairs As New List(Of KeyValuePair(Of String, String))

        Do While myIds.Count > 0
            If myIds.Count > 1 Then
                resPairs.Add(New KeyValuePair(Of String, String)(myIds(0), myIds(1)))
                myIds.RemoveRange(0, 2)
            Else
                Exit Do
            End If
        Loop

        Return resPairs
    End Function

    Private Sub FillMatchingCounterInliners()
        'HINT Fill in all matching counter inliners in other opened HCVs
        If MainForm IsNot Nothing Then
            For Each tabGroup As MdiTabGroup In MainForm.utmmMain.TabGroups
                For Each tab As MdiTab In tabGroup.Tabs
                    Dim documentForm As DocumentForm = TryCast(tab.Form, DocumentForm)
                    If (documentForm IsNot Nothing AndAlso documentForm IsNot Me) Then
                        For inlinerIndex As Integer = 0 To _inliners.Keys.Count - 1
                            Dim inliner As Connector_occurrence = _inliners.Keys(inlinerIndex)
                            Dim counterInliner As Connector_occurrence = GetCounterInliner(documentForm, inliner)

                            If (_inliners(inliner) Is Nothing) AndAlso (counterInliner IsNot Nothing) Then
                                _inliners(inliner) = counterInliner
                                documentForm.Inliners(counterInliner) = inliner

                                If (MainForm.MainStateMachine.OverallConnectivity IsNot Nothing) AndAlso (IsExtendedHCV) Then
                                    Dim cavities As New Dictionary(Of String, String)
                                    Dim counterCavities As New Dictionary(Of String, String)

                                    If (inliner.Slots IsNot Nothing) AndAlso (inliner.Slots.Length <> 0) Then
                                        For Each cavityOcc As Cavity_occurrence In inliner.Slots(0).Cavities
                                            If (_kblMapper.KBLPartMapper.ContainsKey(cavityOcc.Part)) AndAlso (Not cavities.ContainsKey(DirectCast(_kblMapper.KBLPartMapper(cavityOcc.Part), [Lib].Schema.Kbl.Cavity).Cavity_number)) Then
                                                cavities.Add(DirectCast(_kblMapper.KBLPartMapper(cavityOcc.Part), [Lib].Schema.Kbl.Cavity).Cavity_number, cavityOcc.SystemId)
                                            End If
                                        Next
                                    End If

                                    If (counterInliner.Slots IsNot Nothing) AndAlso (counterInliner.Slots.Length <> 0) Then
                                        For Each cavityOcc As Cavity_occurrence In counterInliner.Slots(0).Cavities
                                            If (documentForm.KBL.KBLPartMapper.ContainsKey(cavityOcc.Part)) AndAlso (Not counterCavities.ContainsKey(DirectCast(documentForm.KBL.KBLPartMapper(cavityOcc.Part), [Lib].Schema.Kbl.Cavity).Cavity_number)) Then
                                                counterCavities.Add(DirectCast(documentForm.KBL.KBLPartMapper(cavityOcc.Part), [Lib].Schema.Kbl.Cavity).Cavity_number, cavityOcc.SystemId)
                                            End If
                                        Next
                                    End If

                                    For Each cav As KeyValuePair(Of String, String) In cavities
                                        If counterCavities.ContainsKey(cav.Key) Then
                                            Dim inlinerid As String = HarnessConnectivity.GetUniqueId(_kblMapper.HarnessPartNumber, cav.Value)
                                            Dim counterInlinerid As String = HarnessConnectivity.GetUniqueId(documentForm.KBL.HarnessPartNumber, counterCavities(cav.Key))
                                            MainForm.MainStateMachine.OverallConnectivity.InlinerCavityPairs.TryAdd(inlinerid, counterInlinerid)
                                            MainForm.MainStateMachine.OverallConnectivity.InlinerCavityPairs.TryAdd(counterInlinerid, inlinerid)
                                        End If
                                    Next

                                End If
                            End If
                        Next
                    End If
                Next
            Next
        End If
    End Sub

    Private Function GetCavityVertex(contactPoint As String) As CavityVertex
        Dim cavityVertex As CavityVertex = Nothing

        Dim cavity_on_ContactPoint As Cavity_occurrence = If(Not String.IsNullOrEmpty(contactPoint), _kblMapper.GetCavityOccurrenceOfContactPointId(contactPoint), Nothing)
        If cavity_on_ContactPoint IsNot Nothing Then
            Dim cavityNumber As String = _kblMapper.GetPart(Of E3.Lib.Schema.Kbl.Cavity)(cavity_on_ContactPoint.Part).Cavity_number
            Dim connectorOcc As Connector_occurrence = _kblMapper.GetConnectorOfContactPoint(contactPoint)

            Dim cavityVertexId As String = HarnessConnectivity.GetUniqueId(_kblMapper.HarnessPartNumber, cavity_on_ContactPoint.SystemId)

            If (connectorOcc IsNot Nothing) Then
                If (_harnessConnectivity.HasCavityVertex(cavityVertexId)) Then
                    cavityVertex = _harnessConnectivity.GetCavityVertex(cavityVertexId)
                Else
                    cavityVertex = New CavityVertex(cavityVertexId)
                    cavityVertex.CavityNumber = cavityNumber

                    If (_kblMapper.GetHarness.Description <> String.Empty) Then
                        cavityVertex.HarnessDescription = _kblMapper.GetHarness.Description
                    Else
                        cavityVertex.HarnessDescription = _kblMapper.HarnessPartNumber
                    End If

                    cavityVertex.ConnectorHousing = If(_kblMapper.KBLPartMapper.ContainsKey(connectorOcc.Part), TryCast(_kblMapper.KBLPartMapper(connectorOcc.Part), Connector_Housing), Nothing)
                    cavityVertex.ConnectorOccurrence = connectorOcc
                    cavityVertex.HarnessPartNumber = _kblMapper.HarnessPartNumber
                    cavityVertex.IsInliner = _inliners.ContainsKey(connectorOcc)

                    _harnessConnectivity.AddCavityVertex(cavityVertex)
                End If
            End If
        End If

        Return cavityVertex
    End Function

    Private Function GetCounterInliner(docForm As DocumentForm, inliner As Connector_occurrence) As Connector_occurrence
        If (docForm.Inliners IsNot Nothing) Then
            For Each connectorOccurrence As Connector_occurrence In docForm.Inliners.Keys

                Dim matchCounter As Integer = 0
                With connectorOccurrence
                    For Each inlinerIdentifer As InlinerIdentifier In MainForm.GeneralSettings.InlinerIdentifiers
                        Select Case inlinerIdentifer.KBLPropertyName
                            Case ConnectorPropertyName.Id.ToString
                                If AreInlinerPairIdentifiersMatching(inlinerIdentifer, inliner.Id, .Id) Then
                                    matchCounter += 1
                                End If

                            Case ObjectPropertyNameStrings.AliasId
                                If (.Alias_id.Length <> 0) AndAlso (inliner.Alias_id.Length <> 0) Then
                                    Dim match As Boolean = False
                                    For Each aliasId As Alias_identification In .Alias_id
                                        For Each aId As Alias_identification In inliner.Alias_id
                                            If AreInlinerPairIdentifiersMatching(inlinerIdentifer, aId.Alias_id, aliasId.Alias_id) Then
                                                matchCounter += 1
                                                match = True
                                                Exit For
                                            End If
                                        Next
                                        If (match) Then Exit For
                                    Next
                                End If
                            Case ObjectPropertyNameStrings.Description
                                If (.Description IsNot Nothing) AndAlso (inliner.Description IsNot Nothing) Then
                                    If AreInlinerPairIdentifiersMatching(inlinerIdentifer, inliner.Description, .Description) Then
                                        matchCounter += 1
                                    End If
                                End If
                            Case ObjectPropertyNameStrings.InstallationInformation
                                If (.Installation_information.Length <> 0) AndAlso (inliner.Installation_information.Length <> 0) Then

                                    Dim match As Boolean = False
                                    For Each installationInformation As Installation_instruction In .Installation_information
                                        For Each instInformation As Installation_instruction In inliner.Installation_information.Where(Function(instruction) instruction.Instruction_type = installationInformation.Instruction_type)
                                            If AreInlinerPairIdentifiersMatching(inlinerIdentifer, instInformation.Instruction_value, installationInformation.Instruction_value) Then
                                                matchCounter += 1
                                                match = True
                                                Exit For
                                            End If
                                        Next
                                        If (match) Then
                                            Exit For
                                        End If
                                    Next
                                End If
                        End Select
                    Next
                End With

                If (MainForm.GeneralSettings.InlinerIdentifiers.Count > 0) AndAlso matchCounter = (MainForm.GeneralSettings.InlinerIdentifiers.Count) Then
                    Return connectorOccurrence
                End If
            Next
        End If
        Return Nothing
    End Function

    Public Function GetCounterInlinerToBoxConnector(boxConnector As Component_box_connector) As Connector_occurrence
        If (Me.Inliners IsNot Nothing) Then
            For Each connectorOccurrence As Connector_occurrence In Me.Inliners.Keys

                Dim matchCounter As Integer = 0
                With connectorOccurrence
                    For Each inlinerIdentifer As InlinerIdentifier In MainForm.GeneralSettings.InlinerIdentifiers
                        Select Case inlinerIdentifer.KBLPropertyName
                            Case ConnectorPropertyName.Id.ToString
                                If AreInlinerPairIdentifiersMatching(inlinerIdentifer, boxConnector.Id, .Id) Then
                                    matchCounter += 1
                                End If

                            Case ObjectPropertyNameStrings.AliasId
                                If (.Alias_id.Length <> 0) Then
                                    Dim match As Boolean = False
                                    For Each aliasId As Alias_identification In .Alias_id
                                        If AreInlinerPairIdentifiersMatching(inlinerIdentifer, boxConnector.Id, aliasId.Alias_id) Then
                                            matchCounter += 1
                                            match = True
                                        End If
                                        If (match) Then Exit For
                                    Next
                                End If
                            Case ObjectPropertyNameStrings.Description
                                If (.Description IsNot Nothing) Then
                                    If AreInlinerPairIdentifiersMatching(inlinerIdentifer, boxConnector.Id, .Description) Then
                                        matchCounter += 1
                                    End If
                                End If
                            Case ObjectPropertyNameStrings.InstallationInformation
                                If (.Installation_information.Length <> 0) Then

                                    Dim match As Boolean = False
                                    For Each installationInformation As Installation_instruction In .Installation_information

                                        If AreInlinerPairIdentifiersMatching(inlinerIdentifer, boxConnector.Id, installationInformation.Instruction_value) Then
                                            matchCounter += 1
                                            match = True
                                        End If

                                        If (match) Then
                                            Exit For
                                        End If
                                    Next
                                End If
                        End Select
                    Next
                End With

                If (MainForm.GeneralSettings.InlinerIdentifiers.Count > 0) AndAlso matchCounter = (MainForm.GeneralSettings.InlinerIdentifiers.Count) Then
                    Return connectorOccurrence
                End If
            Next
        End If
        Return Nothing
    End Function

    Private Function InactiveModulesAreIdentical() As Boolean
        If (_oldInactiveModules Is Nothing) Then
            Return True
        End If

        Dim inactiveModules As List(Of String) = _kblMapper.InactiveModules.Keys.ToList
        inactiveModules.Sort()

        If (inactiveModules.Count <> _oldInactiveModules.Count) Then
            Return False
        Else
            For inactiveModCounter As Integer = 0 To inactiveModules.Count - 1
                If (inactiveModules(inactiveModCounter) <> _oldInactiveModules(inactiveModCounter)) Then
                    Return False
                End If
            Next
        End If

        Return True
    End Function

    Private Sub InitializeHarnessConnectivity()
        _harnessConnectivity = New HarnessConnectivity(_kblMapper.HarnessPartNumber)

        For Each connection As Connection In _kblMapper.GetConnections
            If (connection.Wire <> String.Empty) Then
                Dim wireAdjacency As WireAdjacency = Nothing

                Dim adjacencyId As String = HarnessConnectivity.GetUniqueId(_kblMapper.HarnessPartNumber, connection.Wire)

                If (_harnessConnectivity.HasWireAdjacency(adjacencyId)) Then
                    wireAdjacency = _harnessConnectivity.GetWireAdjacency(adjacencyId)
                Else
                    wireAdjacency = New WireAdjacency(adjacencyId)

                    If Not String.IsNullOrEmpty(connection.Wire) AndAlso (_kblMapper.KBLOccurrenceMapper.ContainsKey(connection.Wire)) Then
                        wireAdjacency.CoreWireOccurrence = _kblMapper.KBLOccurrenceMapper(connection.Wire)
                    End If

                    wireAdjacency.SignalName = connection.Signal_name

                    If (wireAdjacency.CoreWireOccurrence IsNot Nothing) Then
                        If (TypeOf wireAdjacency.CoreWireOccurrence Is Core_occurrence) Then
                            If (_kblMapper.KBLPartMapper.ContainsKey(DirectCast(wireAdjacency.CoreWireOccurrence, Core_occurrence).Part)) Then
                                wireAdjacency.CoreWirePart = _kblMapper.KBLPartMapper(DirectCast(wireAdjacency.CoreWireOccurrence, Core_occurrence).Part)
                                wireAdjacency.WireNumber = DirectCast(wireAdjacency.CoreWireOccurrence, Core_occurrence).Wire_number
                            End If
                        ElseIf (TypeOf wireAdjacency.CoreWireOccurrence Is Wire_occurrence) Then
                            If (_kblMapper.KBLPartMapper.ContainsKey(DirectCast(wireAdjacency.CoreWireOccurrence, Wire_occurrence).Part)) Then
                                wireAdjacency.CoreWirePart = _kblMapper.KBLPartMapper(DirectCast(wireAdjacency.CoreWireOccurrence, Wire_occurrence).Part)
                                wireAdjacency.WireNumber = DirectCast(wireAdjacency.CoreWireOccurrence, Wire_occurrence).Wire_number
                            End If
                        End If
                    End If

                    _harnessConnectivity.AddWireAdjacency(wireAdjacency)
                End If

                Dim cavityVertexA As CavityVertex = GetCavityVertex(connection.GetStartContactPointId)
                Dim cavityVertexB As CavityVertex = GetCavityVertex(connection.GetEndContactPointId)

                Dim linkVertexAToVertexB As Boolean = True
                Dim linkVertexBToVertexA As Boolean = True

                If (cavityVertexA IsNot Nothing) Then
                    For Each adjacencyToVertex As AdjacencyToVertex In cavityVertexA.AdjacenciesToSuccessors
                        If (adjacencyToVertex.WireAdjacency.Key = wireAdjacency.Key) AndAlso (adjacencyToVertex.Vertex.Key = cavityVertexB.Key) Then
                            linkVertexAToVertexB = False
                            Exit For
                        End If
                    Next
                End If

                If (cavityVertexB IsNot Nothing) Then
                    For Each adjacencyToVertex As AdjacencyToVertex In cavityVertexB.AdjacenciesToSuccessors
                        If (adjacencyToVertex.WireAdjacency.Key = wireAdjacency.Key) AndAlso (adjacencyToVertex.Vertex.Key = cavityVertexA.Key) Then
                            linkVertexBToVertexA = False
                            Exit For
                        End If
                    Next
                End If

                If (cavityVertexA IsNot Nothing) AndAlso (linkVertexAToVertexB) Then
                    cavityVertexA.LinkToSuccessorCavityVertexViaWireAdjacency(cavityVertexB, wireAdjacency)
                End If
                If (cavityVertexB IsNot Nothing) AndAlso (linkVertexBToVertexA) Then
                    cavityVertexB.LinkToSuccessorCavityVertexViaWireAdjacency(cavityVertexA, wireAdjacency)
                End If
            End If
        Next

        If (MainForm?.MainStateMachine.OverallConnectivity IsNot Nothing) Then
            Dim partNumber As String = _kblMapper.HarnessPartNumber
            MainForm.MainStateMachine.OverallConnectivity.HarnessConnectivities.AddOrUpdate(partNumber, Function() _harnessConnectivity, Function() _harnessConnectivity)
        End If
    End Sub

    Private Sub InitializeInliners()
        _inliners = New Dictionary(Of Connector_occurrence, Connector_occurrence)

        'HINT Fill in all inliners of current HCV document filtered by defined identification criterias
        For Each connectorOccurrence As Connector_occurrence In _kblMapper.GetObjects(Of Connector_occurrence)
            If IsInliner(connectorOccurrence) Then
                _inliners.Add(connectorOccurrence, Nothing)
            End If
        Next
    End Sub

    Private Sub LoadHarnessModuleConfigurationsFromHcv(s As System.IO.Stream)
        If s.CanSeek Then
            s.Position = 0
        End If

        Dim kblContainer As KBL_container = KBL_container.Read(s)
        For Each harnessConfig As Harness_configuration In kblContainer.Harness.Harness_configuration
            Dim harnessModuleConfiguration As New HarnessModuleConfiguration
            With harnessModuleConfiguration
                .ConfigurationType = [Enum].Parse(Of HarnessModuleConfigurationType)(harnessConfig.Description)
                .HarnessConfiguration = harnessConfig
            End With

            _harnessModuleConfigurations.Add(harnessModuleConfiguration)
        Next
    End Sub

    Private Sub LoadHarnessModuleConfigurationsFromKBL()
        If (_kblMapper.GetHarnessConfigurations IsNot Nothing) AndAlso (_kblMapper.GetHarnessConfigurations.Any()) Then
            For Each harnessConfig As Harness_configuration In _kblMapper.GetHarnessConfigurations
                Dim harnessModuleConfiguration As New HarnessModuleConfiguration
                With harnessModuleConfiguration
                    .ConfigurationType = HarnessModuleConfigurationType.FromKBL
                    .HarnessConfiguration = harnessConfig
                End With

                _harnessModuleConfigurations.Add(harnessModuleConfiguration)
            Next
        End If

        If (_kblMapper.GetModules IsNot Nothing) AndAlso (_kblMapper.GetModules.Any()) AndAlso (Not _harnessModuleConfigurations.Where(Function(harnessModuleConfig) harnessModuleConfig.ConfigurationType = HarnessModuleConfigurationType.Custom).Any()) Then
            Dim harnessConfig As New Harness_configuration
            With harnessConfig
                .SystemId = Guid.NewGuid.ToString

                .Abbreviation = String.Empty
                .Description = HarnessModuleConfigurationType.Custom.ToString
                .Modules = String.Empty
                .Part_number = KblObjectType.Custom.ToLocalizedString
                .Version = "1"

                For Each [module] As [Lib].Schema.Kbl.[Module] In _kblMapper.GetModules
                    If (_modulePartNumbersForActivation.Count = 0) OrElse (_modulePartNumbersForActivation.Contains([module].Part_number.Trim) OrElse _modulePartNumbersForActivation.Contains([module].Part_number.Trim.Replace(" ", String.Empty))) Then
                        If (.Modules = String.Empty) Then
                            .Modules = [module].SystemId
                        Else
                            .Modules = String.Format("{0} {1}", .Modules, [module].SystemId)
                        End If
                    End If
                Next
            End With

            Dim harnessModuleConfiguration As New HarnessModuleConfiguration
            With harnessModuleConfiguration
                .ConfigurationType = HarnessModuleConfigurationType.Custom
                .HarnessConfiguration = harnessConfig
                .IsActive = True
            End With

            _harnessModuleConfigurations.Add(harnessModuleConfiguration)
        End If

        For Each harnessModuleConfig As HarnessModuleConfiguration In _harnessModuleConfigurations
            With harnessModuleConfig
                If (.ConfigurationType = HarnessModuleConfigurationType.Custom OrElse .ConfigurationType = HarnessModuleConfigurationType.UserDefined) AndAlso (_kblMapper.HarnessPartNumber = .HarnessConfiguration.Part_number) AndAlso (.HarnessConfiguration.Abbreviation IsNot Nothing) AndAlso (.HarnessConfiguration.Abbreviation <> String.Empty) Then
                    .HarnessConfiguration.Part_number = .HarnessConfiguration.Abbreviation
                    .HarnessConfiguration.Abbreviation = String.Empty
                End If

                If (.ConfigurationType = HarnessModuleConfigurationType.Custom) Then
                    .HarnessConfiguration.Part_number = KblObjectType.Custom.ToLocalizedString
                End If
            End With
        Next
    End Sub

    Private Sub OnEntireRoutingPathOrOverallConnectivityViewMouseClick(sender As Object, e As InformationHubEventArgs)
        Dim pairs As List(Of KeyValuePair(Of String, String)) = ConvertToDocumentIdPairs(e.ObjectIds)
        Dim allDocuments As Dictionary(Of String, List(Of DocumentForm)) = MainForm.utmmMain.TabGroups.Cast(Of MdiTabGroup).SelectMany(Function(tGrp) tGrp.Tabs.Cast(Of MdiTab)()).Where(Function(t) t.Form IsNot Nothing).GroupBy(Function(t) CType(t.Form, DocumentForm).Tag.ToString, Function(t) CType(t.Form, DocumentForm)).ToDictionary(Function(grp) grp.Key, Function(grp) grp.Distinct.ToList)
        Dim IdsSelected As New Dictionary(Of DocumentForm, List(Of String))

        For Each grp As IGrouping(Of String, KeyValuePair(Of String, String)) In pairs.GroupBy(Function(pair) pair.Key)
            Dim documentForms As List(Of DocumentForm) = Nothing

            If (allDocuments.TryGetValue(grp.Key, documentForms)) Then
                Dim idsOfDocs As List(Of String) = grp.Select(Function(p) p.Value).Distinct.ToList

                For Each doc As DocumentForm In documentForms
                    With doc
                        .Activate()

                        ' HINT: Deactivate 3D selection and highlight in connectivity-view because the selection/highlight is done here document-wise and each new selection/highlight would override the previous selection in 3DView/ConnectivityView
                        Dim selected As List(Of String) = .SelectRowsInGrids(idsOfDocs, True, True, False, False)
                        IdsSelected.Add(doc, selected)
                    End With
                Next
            End If
        Next

        If e.HighlightInConnectivityView Then
            MainForm.SelectSchematicsEntities(IdsSelected.SelectMany(Function(kv) kv.Key.CreateEdbId(kv.Value)).ToArray)
        End If
    End Sub

    Private Function GetHarnessModuleConfigurationsAsContainerFile() As Hcv.ModuleConfigsContainerFile
        Dim kblContainer As New KBL_container
        With kblContainer
            .SystemId = _kblMapper.HarnessSystemId
            .version_id = _kblMapper.Version_Id

            .Harness = New [Lib].Schema.Kbl.Harness

            With .Harness
                .SystemId = _kblMapper.GetHarness.SystemId
                .Part_number = _kblMapper.HarnessPartNumber
                .Company_name = _kblMapper.GetHarness.Company_name
                .Version = _kblMapper.GetHarness.Version
                .Abbreviation = _kblMapper.GetHarness.Abbreviation
                .Description = _kblMapper.GetHarness.Description
                .Car_classification_level_2 = _kblMapper.GetHarness.Car_classification_level_2
                .Model_year = _kblMapper.GetHarness.Model_year
                .Content = _kblMapper.GetHarness.Content

                .Harness_configuration = Array.Empty(Of Harness_configuration)

                For Each harnessModuleConfiguration As HarnessModuleConfiguration In _harnessModuleConfigurations
                    If (harnessModuleConfiguration.ConfigurationType <> HarnessModuleConfigurationType.FromKBL) Then
                        .Harness_configuration.Add(harnessModuleConfiguration.HarnessConfiguration)
                    End If
                Next

                .Module = Array.Empty(Of [Lib].Schema.Kbl.[Module])

                For Each kblModule As [Lib].Schema.Kbl.[Module] In _kblMapper.GetModules
                    Dim [module] As New [Lib].Schema.Kbl.[Module]

                    With kblModule
                        [module].SystemId = .SystemId
                        [module].Part_number = .Part_number
                        [module].Company_name = .Company_name
                        [module].Version = .Version
                        [module].Abbreviation = .Abbreviation
                        [module].Description = .Description
                        [module].Car_classification_level_2 = .Car_classification_level_2
                        [module].Model_year = .Model_year
                        [module].Content = .Content
                    End With

                    .Module.Add([module])
                Next
            End With
        End With

        Using s As New MemoryStream
            KBL_container.Write(s, kblContainer)
            s.Position = 0
            Dim f As New Hcv.ModuleConfigsContainerFile(s.ToArray)
            Return f
        End Using
    End Function

    Public ReadOnly Property IsDocument3DActive As Boolean
        Get
            If utcDocument?.ActiveTab?.TabPage.Controls.Count = 0 Then
                Return False
            Else
                Return TypeOf utcDocument?.ActiveTab?.TabPage.Controls(0) Is D3D.Document.Controls.Document3DControl
            End If
        End Get
    End Property

    Public ReadOnly Property IsAnalysisFormActive As Boolean
        Get
            Return _analysisStateMachine.IsAnalysisActive
        End Get
    End Property

    Public ReadOnly Property Has2D As Boolean
        Get
            Dim res As Boolean = False
            For Each tab As UltraTab In utcDocument?.Tabs
                If tab.TabPage.Controls.Count > 0 AndAlso TypeOf (tab.TabPage.Controls(0)) Is DrawingCanvas Then
                    res = True
                    Exit For
                End If
            Next
            Return res
        End Get

    End Property

    Private Sub LoadDrawings(workState As D3DWorkState, workerPack As BackgroundWorkerPack)
        Dim kblFile As Hcv.KblContainerFile = workerPack.File

        _messageEventArgs = New MessageEventArgs(MessageEventArgs.MsgType.ShowProgressAndMessage)

        'HINT Load SVG harness drawing
        If (Me.utcDocument.ActiveTab IsNot Nothing) Then
            Dim drawingCanvasTab As UltraTab = Me.utcDocument.Tabs.OfType(Of UltraTab).Where(Function(tab) TypeOf tab?.TabPage.Controls(0) Is DrawingCanvas).SingleOrDefault

            _activeDrawingCanvas = TryCast(Me.utcDocument.ActiveTab.TabPage.Controls(0), DrawingCanvas)

            If drawingCanvasTab IsNot Nothing AndAlso Not workState.IsCancellingOrCancelled Then ' HINT: needs rework, cumbersome handling of IsCancellationPending
                Dim drawingFile As Hcv.SvgContainerFile = CType(_hcvFile.OfType(Of Hcv.IDataContainerFile).Where(Function(f) f.FullName = drawingCanvasTab.Key).FirstOrDefault, Hcv.SvgContainerFile) ' HINT: this handling is only a temp solution until the whole document/project/hcv handling is streamlined together
                Dim res As System.ComponentModel.Result = DirectCast(drawingCanvasTab.TabPage.Controls(0), DrawingCanvas).LoadSingleDrawing(drawingFile, workState)
                workState.AddIfFaultedOrCancelled(res)
            End If
        End If

        If (MainForm?.HasView3DFeature).GetValueOrDefault AndAlso (_document.AnalyseContent(HcvDocument.ContentAnalyseType.HasJTData) OrElse _document.AnalyseContent(HcvDocument.ContentAnalyseType.HasKBLZData)) Then
            Dim openSettings As New ContentSettings With
            {
                .LengthClass = MainForm.GeneralSettings.LengthClassDocument3D,
                .UseKblAbsoluteLocations = MainForm.GeneralSettings.UseKblAbsoluteLocations
            }

            Dim result As IResult = _document.Open(LoadContentType.Entities, openSettings) ' jtData will be loaded (if available) later when all is finished
            NotifyConsoleFromResult(result)
            If result.IsFaultedOrCancelled Then
                ThrowErrorOpeningDocument(result)
            End If
        End If

        workState.Progress.Report(0)
    End Sub

    Private Sub NotifyConsoleFromResult(result As IResult)
        For Each import_result As E3.Lib.IO.KBL.ImportKblWithMessagesResult In result.FindResults(Of E3.Lib.IO.KBL.ImportKblWithMessagesResult)
            For Each msg As MessageInfo In import_result.ImporterMessages
                _logHub.WriteLogMessage(New LogEventArgs(msg))
            Next
        Next
    End Sub

    <DebuggerHidden>
    Private Sub ThrowErrorOpeningDocument(result As IResult)
        If TypeOf result Is IAggregatedResult Then
            If CType(result, IAggregatedResult).Results.OfType(Of DocumentResult).Any() Then
                For Each res As DocumentResult In CType(result, IAggregatedResult).Results.OfType(Of DocumentResult)
                    ThrowErrorOpeningDocumentCore(res.Message, CType(res, DocumentResult).Document, CType(res, DocumentResult).WorkChunkName)
                Next
            Else
                ThrowErrorOpeningDocumentCore(result.Message, _document)
            End If
        ElseIf TypeOf result Is DocumentResult Then
            ThrowErrorOpeningDocumentCore(result.Message, CType(result, DocumentResult).Document, CType(result, DocumentResult).WorkChunkName)
        Else
            ThrowErrorOpeningDocumentCore(result.Message, _document)
        End If
    End Sub

    <DebuggerHidden>
    Private Sub ThrowErrorOpeningDocumentCore(message As String, hcvDocument As HcvDocument, Optional workerChunkSourceName As String = Nothing)
        Dim extraWorkChunkMsgInfo As String = If(workerChunkSourceName Is Nothing, String.Empty, $"(WorkChunk: {workerChunkSourceName})")
        Throw New Exception($"Error opening document 3D ""{IO.Path.GetFileName(hcvDocument.FullName)}"": {message} " + extraWorkChunkMsgInfo)  'include extended document result information in exception throw, when the results contains Document result
    End Sub

    Private Async Function LoadDrawingsFinished(workerPack As BackgroundWorkerPack, workState As D3DWorkState) As Task(Of IResult)
        Using Await _busyState.BeginWaitAsync ' lock double calls on "LoadDrawingsFinished" and increase busy-count
            'HINT Raise status message which displays loading of SVG drawing has been finished
            With _messageEventArgs
                .StatusMessage = DocumentFormStrings.LoadSVGFinish_LogMsg
                .ProgressPercentage = 100
            End With

            OnMessage(_messageEventArgs)

            If Not workState.IsCancellingOrCancelled AndAlso Not workState.Result.IsFaulted Then
                Me.udmDocument.BeginUpdate()

                _drawingsHub.upnButton.BeginUpdate()
                _drawingsHub.upnButton.Visible = False
                _drawingsHub.upnButton.EndUpdate()

                If _document?.IsOpen Then
                    RegisterView(CreateDocument3DTab) ' HINT: must come after document.open because now the document has loaded file content which is now detectable if we have content (remind: entities without Z have been skipped)
                    _drawingsHub.TryAddDocument3DNode(_drawingFile?.FullName)
                End If

                If Not Me.udmDocument.ControlPanes.Exists(PaneKeys.NavigatorHub.ToString) Then
                    AddOrUpdateNavigatorHubPane()
                End If

                'HINT Display latest loaded drawing and initialize navigator hub
                If Me.utcDocument.Tabs.Count > 0 Then
                    If (Me.utcDocument.Tabs.Count = 1) Then
                        Me.utcDocument.SelectedTab = Nothing 'HINT MR kick ActiveTabChanged event after first created Tab!
                    End If

                    Me.utcDocument.Tabs.Item(Me.utcDocument.Tabs.Count - 1).Selected = True
                Else
                    If Me.udmDocument.ControlPanes.Exists(PaneKeys.NavigatorHub.ToString) Then
                        Me.udmDocument.ControlPanes(PaneKeys.NavigatorHub.ToString).Close()
                    End If
                    _messageEventArgs.SetClearStatusMsg()
                    OnMessage(_messageEventArgs)
                End If

                Me.udmDocument.EndUpdate()

                _drawingsHub.utDrawings.Enabled = True
                _memolistHub.Enabled = True
                _modulesHub.Enabled = True
            Else
                'HINT Display message and close drawing canvas
                If workState.Result.IsFaulted Then
                    MessageBox.Show(String.Format(DocumentFormStrings.CannotLoadSVG_Msg, vbCrLf, workState.Result.Message), [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If

                Me.utcDocument.BeginUpdate()
                If Me.utcDocument.ActiveTab IsNot Nothing Then
                    Me.utcDocument.Tabs.Remove(Me.utcDocument.ActiveTab)
                End If
                Me.utcDocument.EndUpdate()

                If Not utcDocument.Tabs.OfType(Of UltraTab).Select(Function(tab) tab.TabPage.Controls(0)).OfType(Of DrawingCanvas).Any() Then
                    _activeDrawingCanvas = Nothing
                End If

                _drawingsHub.upnButton.Visible = False
                _drawingsHub.utDrawings.Enabled = True

                With _drawingsHub.utDrawings
                    .BeginUpdate()
                    .EventManager.AllEventsEnabled = False

                    For Each node As UltraTreeNode In .Nodes
                        node.CheckedState = CheckState.Unchecked

                        If (node.LeftImages.Count > 1) Then
                            node.LeftImages.RemoveAt(1)
                        End If
                    Next

                    .EventManager.AllEventsEnabled = True
                    .EndUpdate()
                End With

                _memolistHub.Enabled = True
                _modulesHub.Enabled = True
            End If

            _cavitiesDocumentView = MainForm?.DocumentViews.AddNew(Me.Text, Me.Id, _kblMapper, Me)
            If _cavitiesDocumentView IsNot Nothing Then
                _cavitiesDocumentView.RedLiningInformation = _redliningInformation
            End If

            If TypeOf _hcvFile Is KblFile Then 'HINT: Check the only single node when we have loaded a kbl or vec directly
                Dim displayNavToolButton As StateButtonTool = Me.Tools.Settings.DisplayNavigatorHub
                displayNavToolButton.Checked = Not MainForm.GeneralSettings.HideNavigatorHubOnLoad
                _drawingsHub.utDrawings.Nodes(0).CheckedState = CheckState.Checked
            End If

            If utcDocument.Tabs.Exists(TAB_DOC3D_KEY) AndAlso _document?.IsOpen Then
                'HINT: Apply Module configuration before SetView-> entities will be cloned for 3D view
                Await ApplyModelConfigurationsOrUpdateOldInactiveModules(False, True, False) ' HINT: wait until recalc is finished before proceeding to load JT-entities to avoid multiple entitiy-access, don't recalc here (entities are not added to devDept-Model, which makes recalc not possible -> is later done on SetViewAsync)
                Dim documentView As D3D.Document.Controls.Document3DControl = CType(utcDocument.Tabs(TAB_DOC3D_KEY).TabPage.Controls(0), D3D.Document.Controls.Document3DControl)
                Dim errorList As New List(Of Exception)
                Dim setResult As IResult = Await _document.SetViewAsync(_crossViewsAdapter)

                If setResult.IsSuccess Then
                    'HINT: recalc here because it was skipped above on ApplyModelConfigurationsOrUpdateOldInactiveModules
                    Await RecalculateD3DBundleDiameters(True) ' ignore busy state (because we are within a isbusy=true-block) to avoid thread lock

                    Dim settings As New ContentSettings
                    With settings.JT
                        If MainForm.GeneralSettings.UseDynamicBundles Then 'HINT: When dynamic bundles we remove segments vertices and protections from merge (not static), because by default all is tagged to be overwritable from JT-Data, by default all is enabled for merging
                            .OverwriteGeomertry.Remove(E3.Lib.Model.ContainerId.Segments)
                            .OverwriteGeomertry.Remove(E3.Lib.Model.ContainerId.Vertices)
                            .OverwriteGeomertry.Remove(E3.Lib.Model.ContainerId.Protections)
                        End If
                        .UseColors = MainForm.GeneralSettings.UseJTColors
                    End With

                    If (_document?.IsOpen).GetValueOrDefault Then ' HINT: check isOpen again because we have set the view async before can be cancelled by the user (document close)
                        Dim res As IResult = Await documentView.TryLoadJTDataAsync(settings)
                        If res.IsFaulted Then
                            _logHub.WriteLogMessage(String.Format(Document3DStrings.ErrorLoadingJTData, res.Message), LogEventArgs.LoggingLevel.Error)
                        End If
                    End If
                Else
                    Dim updateEntitiesResult As IAggregatedResult = TryCast(setResult, IAggregatedResult)
                    If updateEntitiesResult IsNot Nothing AndAlso updateEntitiesResult.Results.OfType(Of EntityUpdateError).Any() Then
                        Return Result.Faulted(String.Format(ErrorStrings.D3D_ErrorUpdatingEntity, CType(updateEntitiesResult.Results.OfType(Of EntityUpdateError).First.Entity, IBaseModelEntityEx).DisplayName, updateEntitiesResult.Message))
                    Else
                        Return Result.Faulted(String.Format(ErrorStrings.D3D_ErrorUpdatingEntity, ErrorStrings.D3D_UnknownEntity, setResult.Message))
                    End If
                End If

                documentView.Model3DControl1.Design.WheelZoomEnabled = True
            Else
                Await ApplyModelConfigurationsOrUpdateOldInactiveModules(False, True, False)
            End If

            _cavitiesDocumentView?.NotifyOpened(_hcvFile, _harnessModuleConfigurations.Where(Function(mc) mc.IsActive).SingleOrDefault?.HarnessConfigurationObject?.SystemId)

            ToggleEnableStateOfMenuTools(True)
            CreateRedlinings()
        End Using
        Return Result.Success
    End Function

    Private Async Function ApplyModelConfigurationsOrUpdateOldInactiveModules(recalcBundleDiameters As Boolean, Optional updateInformationHubFilters As Boolean = True, Optional updateCanvasFilters As Boolean = True) As Task
        'HINT Activate predefined module originated from file name or entire car container setting
        Dim objectIds As New List(Of String)

        For Each modulePartNumberForActivation As String In _modulePartNumbersForActivation
            For Each [module] As [Lib].Schema.Kbl.[Module] In _kblMapper.GetModules.Where(Function([mod]) [mod].Part_number.Trim.Replace(" ", String.Empty) = modulePartNumberForActivation)
                objectIds.Add([module].SystemId)
            Next
        Next

        If (objectIds.Count > 0) Then
            'HINT: apply module configuration must come after 3D-view was set (adds entities to model-view) because in apply module configuration is a Recalculation of Bundle diameter possible (when enabled and which needs entities added to 3d-model/environment)
            Try
                Await ApplyModuleConfiguration(New InformationHubEventArgs(_kblMapper.Id, objectIds, KblObjectType.Harness_module), recalcBundleDiameters, updateInformationHubFilters, updateCanvasFilters)
            Finally
                _dirty = False
            End Try
        Else
            UpdateOldInactiveModules()
            Await Task.CompletedTask
        End If
    End Function

    Public Sub RegisterView(doc3DControl As D3D.Document.Controls.Document3DControl)
        _crossViewsAdapter.Add(doc3DControl)
        _crossViewsAdapter.Add(MainForm.D3D.Consolidated) 'HINT: added after view3D to ensure execution of adapter after view3D has finished (non-parallel-setting)
    End Sub

    Private Sub ProgressLoadDrawings(percent As Integer)
        'HINT Raise status message which displays loading of SVG drawing in progress and current progress percentage value
        With _messageEventArgs
            .ProgressPercentage = percent

            If (Me.utcDocument.ActiveTab IsNot Nothing) Then
                .StatusMessage = String.Format(DocumentFormStrings.LoadSVG_LogMsg, IO.Path.GetFileNameWithoutExtension(Me.utcDocument.ActiveTab.Key))
            Else
                .SetClearStatusMsg()
            End If
            .SetUseLastPosition()
        End With

        OnMessage(_messageEventArgs)
    End Sub

    Private Sub LoadKBLDataModel(state As D3DWorkState, workerPack As BackgroundWorkerPack)
        'HINT Load complete KBL container into data object model
        _messageEventArgs = New MessageEventArgs(MessageEventArgs.MsgType.ShowProgressAndMessage)
        _messageEventArgs.StatusMessage = "Reading kbl container ..."

        Dim containerFile As [Lib].IO.Files.Hcv.KblContainerFile = workerPack.File
        Try
            If containerFile.GetVersion = KblSchemaVersion.V22 Then 'HINT: when we are in on-demand-mode the container-file will be opened here to access the version within the compressed xml
                Throw New NotSupportedException($"Kbl schema version not supported please let the ""{NameOf([Lib].IO.Files.Hcv.KblContainerFile)}"" transform the container by itself when opening/creating it")
            End If

            _kblMapper = New KblMapper(containerFile.GetKblContainer)
            _kblVersion = [Lib].Schema.Kbl.Utils.GetVersionString(containerFile.GetVersion)
            If _document IsNot Nothing Then
                _document.Kbl = _kblMapper
            End If
        Catch ex2 As Exception
            Dim isUnexpectedEndOfStream As Boolean = ex2.Message.StartsWith("Unexpected end of file has occurred")
#If CONFIG = "Debug" Or DEBUG Then
            If isUnexpectedEndOfStream Then
                Throw New System.IO.EndOfStreamException("Can't load kbl data: Unexpected end of stream. Xml file is incomplete!")
            Else
                Throw
            End If
#Else
            If isUnexpectedEndOfStream Then
                state.AddResult(Result.Cancelled("Unexpected end of file has occurred"))
            Else
                state.AddResult(New Result(ex2))
            End If
#End If
            Return
        End Try

        If _kblMapper IsNot Nothing Then

            InitHcvContainerData() 'HINT: must be done before loading Module configurations from kbl and must also be done before accessing qmStamps,Redlinginginfo, etc. otherwise it will not be loaded and not be accessible (InitializeDataSources needs this here before)

            If _kblMapper.HasHarness Then
                LoadHarnessModuleConfigurationsFromKBL()
            End If

            Dim progressAction As Action(Of Integer) = Sub(percent) state?.Progress?.Report(percent)

            If _informationHub.InitializeDataSources(_kblMapper, _qmStamps, _redliningInformation, Nothing, displayLengthClassMsg:=Not IsExtendedHCV OrElse (MainForm IsNot Nothing AndAlso MainForm.utmmMain.TabGroups(0).Tabs.Count = 1)) Then
                InitializeInliners()
                InitializeHarnessConnectivity()

                If (MainForm?.GeneralSettings.ValidateKBLAfterLoad) Then
                    ValidateKBL(containerFile, False)
                End If
            Else
                ValidateKBL(containerFile, False)
            End If

        End If

        TryCast(state.Progress, IProgress(Of Integer))?.Report(0)
    End Sub

    Private Sub InitHcvContainerData()
        'HINT Initialize checked compare result base information
        Dim result As AggregatedCheckedCompareResultInfoResult = _hcvFile.GetCompareInfoOrDefault()
        Dim hasFaultedGraphical As Boolean = result.HasFaultedGraphical
        Dim hasGraphical As Boolean = result.HasGraphical

        Dim hasFaultedTechnical As Boolean = result.HasFaultedTechnical
        Dim hasTechnical As Boolean = result.HasTechnical

        If hasFaultedGraphical OrElse Not hasGraphical Then
            If hasFaultedGraphical Then
                Me._logHub.WriteLogWarning($"Loading graphical compare results skipped: Wrong compare result information format ({result.GetFaultedGraphical.Message})")
            End If
            _compareResultGraphicalInfo = CheckedCompareResultInformation.CreateNew(KnownContainerFileFlags.GCRI)
        ElseIf hasGraphical Then
            _compareResultGraphicalInfo = result.GetGraphical.CCInfo
        End If

        If hasFaultedTechnical OrElse Not hasTechnical Then
            If hasFaultedTechnical Then
                Me._logHub.WriteLogWarning($"Loading technical compare results skipped: Wrong compare result information format ({result.GetFaultedTechnical.Message})")
            End If
            _compareResultTechnicalInfo = CheckedCompareResultInformation.CreateNew(KnownContainerFileFlags.TCRI)
        ElseIf hasTechnical Then
            _compareResultTechnicalInfo = result.GetTechnical.CCInfo
        End If

        If String.IsNullOrEmpty(_compareResultGraphicalInfo?.HarnessPartNumber) Then
            If _compareResultGraphicalInfo IsNot Nothing Then
                _compareResultGraphicalInfo.HarnessPartNumber = _kblMapper.HarnessPartNumber
                _compareResultGraphicalInfo.HarnessVersion = _kblMapper.GetHarness.Version
            End If
        End If

        If String.IsNullOrEmpty(_compareResultTechnicalInfo?.HarnessPartNumber) Then
            If _compareResultTechnicalInfo IsNot Nothing Then
                _compareResultTechnicalInfo.HarnessPartNumber = _kblMapper.HarnessPartNumber
                _compareResultTechnicalInfo.HarnessVersion = _kblMapper.GetHarness.Version
            End If
        End If

        _memoList = _hcvFile.Memolist.ToMemoListOrDefault
        'HINT Initialize memolist base information
        If String.IsNullOrEmpty(_memoList?.HarnessPartNumber) Then
            If _memoList IsNot Nothing Then
                _memoList.HarnessPartNumber = _kblMapper.HarnessPartNumber
                _memoList.HarnessVersion = _kblMapper.GetHarness.Version
            End If
        End If

        _qmStamps = _hcvFile.QMStampInfo.ToQmStampsOrDefault
        'HINT Initialize QM stamp base information
        If String.IsNullOrEmpty(_qmStamps?.HarnessPartNumber) Then
            If _qmStamps IsNot Nothing Then
                _qmStamps.HarnessPartNumber = _kblMapper.HarnessPartNumber
                _qmStamps.HarnessVersion = _kblMapper.GetHarness.Version
            End If
        End If

            _redliningInformation = _hcvFile.RedliningInfo.ToRedliningInfoOrDefault
        'HINT Initialize redlining base information
        If String.IsNullOrEmpty(_redliningInformation?.HarnessPartNumber) Then
            If _redliningInformation IsNot Nothing Then
                _redliningInformation.HarnessPartNumber = _kblMapper.HarnessPartNumber
                _redliningInformation.HarnessVersion = _kblMapper.GetHarness.Version
            End If
        End If

        If (_hcvFile.ValityCheck?.HasData).GetValueOrDefault Then
            Try
                _validityCheckContainer = _hcvFile.ValityCheck.ToValidyCheck
            Catch ex As Exception
                MainForm.AppInitManager.ErrorManager.ShowMsgBoxExclamtionOrWriteConsole(String.Format("{0}{1}{2}", ex.Message, vbCrLf, If(ex.InnerException IsNot Nothing, ex.InnerException.Message, String.Empty)), ErrorCodes.ERROR_EA_FILE_CORRUPT)
            Finally
                Me.utmDocument.Tools(ChecksTabToolKey.Validity.ToString).SharedProps.Visible = _validityCheckContainer IsNot Nothing
            End Try
        End If

        If (_hcvFile.ModuleConfigurations?.HasData).GetValueOrDefault Then
            'HINT Get harness module configurations from XML file
            Using s As Stream = _hcvFile.ModuleConfigurations.GetDataAsStream
                LoadHarnessModuleConfigurationsFromHcv(s)
            End Using
        End If
    End Sub

    Private Async Function LoadKBLDataModelFinished(workState As D3DWorkState, workerPack As BackgroundWorkerPack) As Task(Of IResult)
        'HINT Raise status message which displays loading of KBL data has been finished
        With _messageEventArgs
            .StatusMessage = DocumentFormStrings.LoadKBLFinish_LogMsg
            .SetUseLastPosition()
        End With

        OnMessage(_messageEventArgs)

        If (workState?.Result?.IsSuccess).GetValueOrDefault() AndAlso _kblMapper IsNot Nothing Then
            FillMatchingCounterInliners()

            Me.udmDocument.BeginUpdate()
            Me.udmDocument.ShowPinButton = False

            'HINT Add information hub dockable control pane on bottom window position
            Dim informationHubPane As New DockableControlPane(PaneKeys.InformationHub.ToString, UIStrings.DataTablesPane_Caption)
            With informationHubPane
                .Settings.CanDisplayAsMdiChild = Infragistics.Win.DefaultableBoolean.False
                .Settings.AllowPin = Infragistics.Win.DefaultableBoolean.False
                .Size = New Drawing.Size(200, 200)
                Me.udmDocument.ControlPanes.Add(informationHubPane)
                .Dock(DockedSide.Bottom)
            End With

            'HINT Add drawings hub dockable control pane on top left window position
            Dim drawingsHubPane As New DockableControlPane(PaneKeys.DrawingsHub.ToString, UIStrings.DrawingsPane_Caption)
            With drawingsHubPane
                .Settings.CanDisplayAsMdiChild = Infragistics.Win.DefaultableBoolean.False
                .Settings.AllowPin = Infragistics.Win.DefaultableBoolean.False
                .Size = New Drawing.Size(200, 75)
                Me.udmDocument.ControlPanes.Add(drawingsHubPane)
                .Dock(DockedSide.Left)
            End With

            'HINT Add memolist hub dockable control pane on right window position
            Dim memolistHubPane As New DockableControlPane(PaneKeys.MemolistHub.ToString, UIStrings.MemolistPane_Caption)
            With memolistHubPane
                .Settings.CanDisplayAsMdiChild = Infragistics.Win.DefaultableBoolean.False
                .Settings.AllowPin = Infragistics.Win.DefaultableBoolean.False
                .Size = New Drawing.Size(200, 200)
                Me.udmDocument.ControlPanes.Add(memolistHubPane)
                .Dock(DockedSide.Right)
                .Close()
            End With

            'HINT Add modules hub dockable control pane on lower left window position
            Dim modulesHubPane As New DockableControlPane(PaneKeys.ModulesHub.ToString, UIStrings.ModulesPane_Caption)
            With modulesHubPane
                .Settings.CanDisplayAsMdiChild = Infragistics.Win.DefaultableBoolean.False
                .Settings.AllowPin = Infragistics.Win.DefaultableBoolean.False
                .Size = New Drawing.Size(200, 200)

                Me.udmDocument.ControlPanes.Add(modulesHubPane)
                drawingsHubPane.DockAreaPane.ChildPaneStyle = ChildPaneStyle.HorizontalSplit
                drawingsHubPane.DockAreaPane.Panes.Add(modulesHubPane)
            End With

            If String.IsNullOrEmpty(_kblMapper.GetHarness.Version) Then
                _kblMapper.GetHarness.Version = "1.0"
            End If

            'HINT Fill SVG drawings into tree of drawings hub control
            If _drawingsHub.InitializeTree(_hcvFile, workerPack.LoadDrawings) Then
                _drawingsHub.upnButton.Visible = True
                _drawingsHub.utDrawings.Enabled = False
                drawingsHubPane.Control = _drawingsHub
            Else
                Me.udmDocument.ControlPanes.Remove(drawingsHubPane.Key)
            End If

            'HINT Fill KBL data into grids of information hub control
            If (_informationHub.InitializeGrids()) Then
                informationHubPane.Control = _informationHub
            Else
                Me.udmDocument.ControlPanes.Remove(informationHubPane.Key)
            End If

            'HINT Fill memolist data of memolist hub control
            If (_memolistHub.InitializeTree(_kblMapper)) Then
                memolistHubPane.Control = _memolistHub
                _memolistHub.InitializeData(_memoList.MemoItems, False)
                _memolistHub.utMemolist.Nodes.Override.Sort = SortType.Ascending
                _memolistHub.Enabled = False
            Else
                Me.udmDocument.ControlPanes.Remove(memolistHubPane.Key)
            End If

            'HINT Fill modules and their related families into tree of modules hub control
            If IsExtendedHCV AndAlso (MainForm.MainStateMachine.EntireCarSettings IsNot Nothing) Then
                Dim harness As EntireCarHarness = MainForm.MainStateMachine.EntireCarSettings.Harnesses.FindHarnessFromPartNumberAndVersion(_kblMapper.HarnessPartNumber, _kblMapper.GetHarness.Version)
                If (harness IsNot Nothing) Then
                    For Each actModule As ActiveModule In harness.ActiveModules
                        _modulePartNumbersForActivation.Add(actModule.PartNumber.Trim.Replace(" ", String.Empty))
                    Next
                End If
                'HINT This junk is necessary to get a consistent state in the config mananger and the modules hub 
                'the entire handling of modul partnumbers and their internal ids is junk, too. Basically to be reworked
                For Each hc As HarnessModuleConfiguration In _harnessModuleConfigurations
                    If hc.ConfigurationType = HarnessModuleConfigurationType.Custom Then
                        hc.HarnessConfiguration.Modules = String.Empty
                        For Each [module] As [Lib].Schema.Kbl.[Module] In _kblMapper.GetModules
                            If _modulePartNumbersForActivation.Contains([module].Part_number.Trim.Replace(" ", String.Empty)) Then
                                If (hc.HarnessConfiguration.Modules = String.Empty) Then
                                    hc.HarnessConfiguration.Modules = [module].SystemId
                                Else
                                    hc.HarnessConfiguration.Modules = String.Format("{0} {1}", hc.HarnessConfiguration.Modules, [module].SystemId)
                                End If
                            End If
                        Next
                        Exit For
                    End If
                Next
            End If

            _modulesHub.Enabled = False

            If (_modulesHub.InitializeTree(_kblMapper, _modulePartNumbersForActivation)) AndAlso (_modulesHub.InitializeConfigurations(_harnessModuleConfigurations)) Then
                If (_modulesHub.upnButton.Visible) Then
                    _modulesHub._blinkingTimer.Start()
                End If

                modulesHubPane.Control = _modulesHub
            Else
                Me.udmDocument.ControlPanes.Remove(modulesHubPane.Key)
            End If

            Me.udmDocument.EndUpdate()

            drawingsHubPane.DockAreaPane.AutoSize

            Me.Tag = If(Not String.IsNullOrEmpty(_kblMapper.HarnessPartNumber), _kblMapper.HarnessPartNumber, _kblMapper.GetHarness.SystemId)

            Dim t As Task = Task.Run(Sub() MainForm?.SearchMachine.InitializeKBLSearchPattern(_kblMapper))

            'HINT Load SVG drawings
            If workerPack.LoadDrawings Then
                If (_hcvFile.Topology?.HasData).GetValueOrDefault Then
                    Dim drawingCanvas As New DrawingCanvas(Me, _qmStamps, _redliningInformation)
                    Dim drawingTab As UltraTab = Me.utcDocument.Tabs.Add(_hcvFile.Topology.FullName, _hcvFile.Topology.FullName)
                    drawingTab.TabPage.Controls.Add(drawingCanvas)
                    drawingCanvas.Dock = DockStyle.Fill
                    drawingTab.Text = IO.Path.GetFileNameWithoutExtension(_hcvFile.Topology.FullName)
                End If
            End If

            'HINT Start open document background worker process and address delegate objects to referring methods
            workState.Progress = New Progress(Of Integer)(AddressOf ProgressLoadDrawings)

            'start work for "LoadKBLDataModelFinished-Work"

            Dim work As New WorkDescriptor(Sub(userData As Object)
                                               Dim data As Tuple(Of D3DWorkState, BackgroundWorkerPack) = CType(userData, Tuple(Of D3DWorkState, BackgroundWorkerPack))
                                               LoadDrawings(data.Item1, data.Item2)
                                           End Sub,
                                           Async Function(res As IResult, userData As Object) ' HINT: enclose the whole loadDrawings process (loadDrawings+LoadDrawingsFinished) in a complete work block / busy block (no busy gaps!)
                                               Dim data As Tuple(Of D3DWorkState, BackgroundWorkerPack) = CType(userData, Tuple(Of D3DWorkState, BackgroundWorkerPack))
                                               workState.AddIfFaultedOrCancelled(res)
                                               Return Await LoadDrawingsFinished(workerPack, data.Item1)
                                           End Function, New Tuple(Of D3DWorkState, BackgroundWorkerPack)(workState, workerPack))

            workState.Result = Await StartWork(work)
        Else
            If MainForm IsNot Nothing Then
                If workState.Result.IsFaulted Then
                    MainForm.AppInitManager.ErrorManager.ShowMsgBoxErrorOrWriteConsole(String.Format(DocumentFormStrings.LoadKBLFailed_Msg, vbCrLf, workState.Result.Message), ErrorCodes.ERROR_INVALID_DATA)
                Else
                    MainForm.AppInitManager.ErrorManager.ShowMsgBoxErrorOrWriteConsole(DocumentFormStrings.LoadKBLFailedFileTypeMismatch_Msg, ErrorCodes.ERROR_INVALID_DATA)
                End If
            End If
            ToggleEnableStateOfMenuTools(True)

            Me.Close()
        End If
        Return workState.Result
    End Function

    Private Sub ProgressLoadKBLDataModel(percent As Integer)
        'HINT Raise status message which displays loading of KBL data in progress and current progress percentage value
        With _messageEventArgs
            .ProgressPercentage = percent
            .StatusMessage = DocumentFormStrings.LoadKBL_LogMsg
            .SetUseLastPosition()
        End With

        OnMessage(_messageEventArgs)
    End Sub

    Private Sub OnWorkerStarted()
        Me.Cursor = Cursors.WaitCursor
    End Sub

    Private Sub OnWorkerCompleted()
        Me.Cursor = Cursors.Default
    End Sub

    <Obfuscation(Feature:="renaming", Exclude:=True, ApplyToMembers:=True)>
    Private Class WorkDescriptor
        Public Sub New(action As Action(Of Object), continueAsync As Func(Of IResult, Object, Task(Of IResult)))
            Me.Action = action
            Me.ContinueAsync = continueAsync
        End Sub

        Public Sub New(action As Action(Of Object), continueAsync As Func(Of IResult, Object, Task(Of IResult)), userData As Object)
            Me.Action = action
            Me.ContinueAsync = continueAsync
            Me.UserData = userData
        End Sub

        Property Action As Action(Of Object)
        Property ContinueAsync As Func(Of IResult, Object, Task(Of IResult))
        Property UserData As Object

        Public Sub Invoke()
            If Action IsNot Nothing Then
                Action.Invoke(UserData)
            End If
        End Sub

        Public Function InvokeContinue(res As IResult) As Task(Of IResult)
            If Me.ContinueAsync IsNot Nothing Then
                Return Me.ContinueAsync.Invoke(res, Me.UserData)
            End If
            Return Task.FromResult(Of IResult)(res)
        End Function

    End Class

    Private Async Function StartWork(work As WorkDescriptor, <CallerMemberName> Optional name As String = NameOf(StartWork)) As Task(Of IResult)
        If _busyState IsNot Nothing Then
            Using Await _busyState.BeginWaitAsync(name)
                Return Await StartWorkCore(work)
            End Using
        Else
            Return Await StartWorkCore(work)
        End If
    End Function

    Private Async Function StartWorkCore(work As WorkDescriptor) As Task(Of IResult)
        OnWorkerStarted()
        Dim result As IResult = System.ComponentModel.Result.Success
        Try
            Try
                Dim t1 As Task = Task.Factory.StartNew(work.Action, work.UserData, _busyState.CurrentBeginToken.GetValueOrDefault)
                Await t1
                If t1.IsFaulted Then
                    result = ComponentModel.Result.Faulted(t1.Exception.Message)
                ElseIf t1.IsCanceled Then
                    result = ComponentModel.Result.Cancelled
                End If
            Catch ex As Exception
                If TypeOf ex Is OperationCanceledException OrElse TypeOf ex Is TaskCanceledException Then
                    result = System.ComponentModel.Result.Cancelled(ex.Message)
                Else
                    result = New Result(ex)
                End If
            End Try
        Catch ex As Exception
            If TypeOf ex Is OperationCanceledException Then
                Return ComponentModel.Result.Cancelled(ex.Message)
            End If
            Return New Result(ex)
        Finally
            OnWorkerCompleted()
        End Try

        Try
            Dim t2 As Task(Of IResult) = work.InvokeContinue(result)
            Await t2
            If t2.IsFaulted Then
                result = ComponentModel.Result.Faulted(t2.Exception.Message)
            ElseIf t2.IsCanceled Then
                result = ComponentModel.Result.Cancelled
            End If
            Return t2.Result
        Catch ex As Exception
            If TypeOf ex Is OperationCanceledException OrElse TypeOf ex Is TaskCanceledException Then
                result = System.ComponentModel.Result.Cancelled(ex.Message)
            Else
#If DEBUG Or CONFIG = "Debug" Then
                    Throw
#Else
                result = New Result(ex)
#End If
            End If
        End Try

        Return result
    End Function

    Private Function GetAllMyConnectivityEntities() As EdbConversionEntity()
        Dim block As EdbConversionDocument = Nothing
        If MainForm?.SchematicsView IsNot Nothing AndAlso MainForm.SchematicsView.Entities.TryGetDocument(Me.Id.ToString, block) Then
            Return block.ToArray
        End If
        Return Array.Empty(Of EdbConversionEntity)()
    End Function

    Private Async Function RecalculateD3DBundleDiameters(Optional ignoreBusy As Boolean = False) As Task
        Try
            _cancelRecalculation = True
            Await _recalcLock.WaitAsync()
            _cancelRecalculation = False
            If _settingsForm?.GeneralSettings.UseDynamicBundles AndAlso _document?.IsOpen Then
                Dim activeWireIds As String() = _informationHub.CurrentRowFilterInfo.ActiveWiresAndCores.ToArray
                Dim actWires As IEnumerable(Of Wire) = _document.Entities.GetByKblIds(activeWireIds).SelectMany(Function(w) w.GetEEModelObjects).OfType(Of E3.Lib.Model.Wire)
                For Each w As Wire In _document.Model.Wires
                    w.ActivateInVariant(_document.VariantUsedToActivate, actWires.Contains(w))
                Next

                'HINT MR what is this consolidation for? Why do we need the segments from the wires? --> see parameters: activeWires and activeSegments, calculation should be done on activeWires and activeSegments together -> the segments from activeWires are consolidated into provided activeSegments to get "all" activeSegments for this calculation which is done in the core only on segments (see AsyncBundleReclculator)
                Dim wireSegments As IEnumerable(Of E3.Lib.Model.Segment) = actWires.SelectMany(Function(wire) wire.GetSegments.Entries.Select(Function(seg) seg))
                Dim res As IResult = Await _document.RecalculateBundleDiameters(wireSegments.Concat(_document.Entities.GetByKblIds(_informationHub.CurrentRowFilterInfo.ActiveSegments.ToArray).OfType(Of E3.Lib.Model.Segment)).Distinct, _settingsForm?.DiameterSettings, ignoreBusy)

                If Not res.IsFaulted Then

                    Dim bundlesToUpdate As New Dictionary(Of String, BundleEntity)
                    For Each ent As BundleEntity In MainForm.D3D.Consolidated.Entities.OfType(Of BundleEntity)
                        For Each bundle As BundleEntity In Me.Document.Entities.GetByEEObjectId(ent.EEObjectId)
                            ent.Radius = bundle.Radius
                            bundlesToUpdate.Add(ent.Id, ent)
                        Next
                    Next

                    For Each ent As BundleEntity In bundlesToUpdate.Values
                        ent.Update()
                        For Each prot As ProtectionEntity In ent.Protections
                            prot.Update()
                        Next
                    Next

                    For Each d3dfixing As FixingEntity In devDept.Eyeshot.Entities.Extensions.Flatten(MainForm.D3D.Consolidated.Entities.OfType(Of FixingEntity))
                        Dim fixingBdl As BundleEntity = Nothing
                        If bundlesToUpdate.TryGetValue(d3dfixing.BundleEntityId, fixingBdl) Then
                            d3dfixing.Radius = fixingBdl.Radius
                            d3dfixing.Update()
                        End If
                    Next

                    MainForm?.D3D.Consolidated.Entities.Regen()
                    MainForm?.D3D.Consolidated.Invalidate(True)
                Else
                    _logHub.WriteLogMessage(String.Format(ErrorStrings.D3D_Error_RecalculateBundles, res.Message), LogEventArgs.LoggingLevel.Error)
                End If

                For Each w As Wire In _document.Model.Wires
                    w.ActivateInVariant(_document.VariantUsedToActivate, True)
                Next

            End If
        Catch ex As Exception
            _logHub.WriteLogMessage(String.Format(ErrorStrings.D3D_Error_RecalculateBundles, ex.Message), LogEventArgs.LoggingLevel.Error)
        Finally
            _recalcLock.Release()
        End Try
    End Function

    Private Sub ToggleEnableStateOfApplicationMenu(enabled As Boolean)
        If (Me.utmDocument.MdiParentManager Is Nothing) Then
            MainForm?.utmmMain?.TabFromForm(Me)?.Activate()

            Me.utmDocument?.MdiParentManager?.BeginUpdate()
            Dim tools As ToolsCollection = Me.utmDocument?.MdiParentManager?.Ribbon?.ApplicationMenu2010?.NavigationMenu?.Tools
            If tools IsNot Nothing Then
                For Each tool As ToolBase In tools
                    If (enabled) Then
                        tool.SharedProps.Enabled = CBool(tool.Tag)
                    Else
                        tool.Tag = tool.SharedProps.Enabled
                        tool.SharedProps.Enabled = False
                    End If

                    If (TypeOf tool Is PopupMenuTool) Then
                        For Each popupMenuTool As ToolBase In DirectCast(tool, PopupMenuTool).Tools
                            popupMenuTool.SharedProps.Enabled = enabled
                        Next
                    End If
                Next
                Me.utmDocument?.MdiParentManager?.EndUpdate()
            End If
        End If

    End Sub

    Public Function ToggleVisibleOfEntitiesWithNoModules(visible As Boolean) As Boolean
        _hideEntitiesWithNoModules = Not visible

        ApplyModuleConfiguration(False, False)

        If _activeDrawingCanvas IsNot Nothing Then
            Return True
        End If

        Return False
    End Function

    Private Sub ToggleEnableStateOfMenuTools(enabled As Boolean)
        ToggleEnableStateOfApplicationMenu(enabled)

        Me.utmDocument.BeginUpdate()
        For Each tool As ToolBase In Me.utmDocument.Tools
            If enabled Then
                tool.SharedProps.Enabled = CBool(tool.Tag)
            Else
                tool.Tag = tool.SharedProps.Enabled
                tool.SharedProps.Enabled = False
            End If
        Next

        If enabled AndAlso Document IsNot Nothing AndAlso IsDocument3DActive Then
            _analysisStateMachine.DisableButton(HomeTabToolKey.AnalysisShowQMIssues)
        End If

        If enabled AndAlso _D3DControl Is Nothing AndAlso Not IsDocument3DActive Then
            Me.Tools.Settings.Indicators.SharedProps.Enabled = False
        End If

        If Not (_hcvFile.Index?.HasData).GetValueOrDefault Then
            Me.Tools.Home.BOMActive.SharedProps.Enabled = False
            Me.Tools.Home.BOMAll.SharedProps.Enabled = False
        End If

        Me.utmDocument.EndUpdate()
    End Sub

    Private Sub UpdateOldInactiveModules()
        _oldInactiveModules = _kblMapper.InactiveModules.Keys.ToList
        _oldInactiveModules.Sort()
    End Sub

    Friend ReadOnly Property RowFilters As RowFiltersCollection
        Get
            Return _informationHub?.RowFiltes
        End Get
    End Property

    'TODO: FilterActiveObjects moving to Document (Project.HcvDocument in future state would be optimal but is currently not possible due to too much re-work of whole structure)
    Private Function FilterActiveObjects(Optional updateInformationHubFilters As Boolean = True, Optional updateCanvas As Boolean = True) As Boolean
        Dim changed As Boolean
        If updateInformationHubFilters Then
            _informationHub.FilterActiveObjects(_kblMapper.InactiveModules, _hideEntitiesWithNoModules)
        End If

        If Not InactiveModulesAreIdentical() Then
            For Each moduleConfigWithCircles As Dictionary(Of HarnessModuleConfiguration, PackagingCircle) In _calculatedSegmentDiameters.Values
                moduleConfigWithCircles.Clear()
            Next
        End If

        UpdateOldInactiveModules()

        If updateCanvas Then
            For Each canvas As DrawingCanvas In GetAllDrawingCanvas()
                If canvas.FilterActiveObjects(_informationHub.CurrentRowFilterInfo) Then
                    changed = True
                End If
            Next
        End If

        If updateInformationHubFilters Then
            MainForm?.SearchMachine.UpdateInactiveObjects(_informationHub.CurrentRowFilterInfo)
        End If

        If updateCanvas Then
            If SetVisibilityOfAllUnboundEntities(Not _hideEntitiesWithNoModules) Then ' HINT: hide all unbound entities in general when hide-with-no-modules is activated because theese entities don't have any module or object binding to the model
                changed = True
            End If

            If _D3DControl IsNot Nothing Then
                _D3DControl.OnAfterFilterActiveObjects()
            End If
        End If
        Return changed
    End Function

    Private Function SetVisibilityOfAllUnboundEntities(visible As Boolean) As Boolean
        Dim changed As Boolean
        If _document?.IsOpen Then
            For Each unboundEnt As HcvDocument.UnboundEntity In _document.Entities.OfType(Of HcvDocument.UnboundEntity)
                If unboundEnt.Visible <> visible Then
                    unboundEnt.Visible = visible
                    changed = True
                End If
            Next
        End If
        Return changed
    End Function

    Public Class FilterResult
        Public Sub New()
        End Sub

        Public Sub New(activeWiresAndCores As ICollection(Of String), activeSegments As ICollection(Of String), any3DVisibilityChanged As Boolean)
            Me.ActiveWiresAndCores = activeWiresAndCores
            Me.ActiveSegments = activeSegments
            Me.Any3DVisibilityChanged = any3DVisibilityChanged
        End Sub

        Public ReadOnly Property ActiveWiresAndCores As ICollection(Of String) = Array.Empty(Of String)
        Public ReadOnly Property ActiveSegments As ICollection(Of String) = Array.Empty(Of String)
        Public ReadOnly Property Any3DVisibilityChanged As Boolean
    End Class

    ' HINT: this method can be used to avoid warnings
    Friend Async Sub ApplyModuleConfiguration(isDirty As Boolean, updateTree As Boolean, Optional harnessConfig As Harness_configuration = Nothing, Optional recalcBundleDiameters As Boolean = True, Optional updateInformationHubFilters As Boolean = True, Optional updateCanvasFilters As Boolean = True)
        Await ApplyModuleConfigurationAsync(isDirty, updateTree, harnessConfig, recalcBundleDiameters, updateInformationHubFilters, updateCanvasFilters)
    End Sub

    Friend Async Function ApplyModuleConfigurationAsync(isDirty As Boolean, updateTree As Boolean, Optional harnessConfig As Harness_configuration = Nothing, Optional recalcBundleDiameters As Boolean = True, Optional updateInformationHubFilters As Boolean = True, Optional updateCanvasFilters As Boolean = True) As Task
        If (harnessConfig Is Nothing) Then
            harnessConfig = CType(If(_harnessModuleConfigurations.Any(Function(harnessModuleConfig) harnessModuleConfig.ConfigurationType = HarnessModuleConfigurationType.Custom), _harnessModuleConfigurations.Where(Function(harnessModuleConfig) harnessModuleConfig.ConfigurationType = HarnessModuleConfigurationType.Custom).FirstOrDefault.HarnessConfigurationObject, Nothing), Harness_configuration)
        End If

        FilterActiveObjects(updateInformationHubFilters, updateCanvasFilters)

        If (MainForm?.SearchMachine.IsSearchFormVisible) AndAlso updateInformationHubFilters Then
            MainForm.SearchMachine.CreateSearchFormWithPredefinedSearchString(MainForm.SearchMachine.BeginsWithEnabled, MainForm.SearchMachine.CaseSensitiveEnabled, True, MainForm.SearchMachine.CurrentSearchString)
        End If

        If updateTree Then
            For Each harnessModuleConfig As HarnessModuleConfiguration In _harnessModuleConfigurations
                If harnessModuleConfig.HarnessConfiguration.Equals(harnessConfig) Then
                    harnessModuleConfig.IsActive = True

                    With _modulesHub
                        .SystemChange = True

                        For Each item As Infragistics.Win.ValueListItem In .uceModuleConfigs.Items
                            If item.DataValue.Equals(harnessModuleConfig.HarnessConfiguration) Then
                                .uceModuleConfigs.SelectedItem = item

                                Exit For
                            End If
                        Next

                        .SystemChange = False
                    End With
                Else
                    harnessModuleConfig.IsActive = False
                End If
            Next

            _modulesHub.UpdateTreeNodes_Recursively(Nothing)
            _modulesHub.upnButton.Visible = False
        End If

        If (_documentStateMachine._bomForm IsNot Nothing) AndAlso (_documentStateMachine._bomForm.Visible) AndAlso (_documentStateMachine._bomForm.ActiveModulePartNumbers IsNot Nothing) Then
            _documentStateMachine.ShowBOMInformation(True)
        End If

        If isDirty Then
            _dirty = True
            If MainForm IsNot Nothing Then
                MainForm.utmMain.Tools(ApplicationMenuToolKey.SaveDocument.ToString).SharedProps.Enabled = True
                MainForm.utmMain.Tools(ApplicationMenuToolKey.SaveDocuments.ToString).SharedProps.Enabled = True
            End If
        End If

        Me.SetActiveConnectiviy(GetAllMyConnectivityEntities)

        'TODO: check why document is nothing on specific xhcvs
        _D3DControl?.Document?.Entities?.Invalidate()

        If recalcBundleDiameters Then
            Await RecalculateD3DBundleDiameters()
        End If

        'HINT: This is done here the first time because the recalculate is async (the invalidate is done there the 2nd time). So first we invaliate the invisible/visible status of the bundles and the we invalidate the recalculated diamters of the bundles
        MainForm?.D3D?.Consolidated?.Invalidate(True)
        MainForm?.InvalidateActiveCavityChecker()
    End Function

    Private Function GetFirstAvailableEdbId(edbIds As IEnumerable(Of String)) As String
        If MainForm IsNot Nothing Then
            Return edbIds.Where(Function(id) Me.MainForm.SchematicsView.ContainsItem(id)).FirstOrDefault
        End If
        Return Nothing
    End Function

    Friend Function CreateFirstAvailableEdbId(kblIds As IEnumerable(Of String)) As String
        Dim ids As IEnumerable(Of String) = kblIds.Select(Function(id) Me.CreateEdbId(id)).Where(Function(edbId) Not String.IsNullOrEmpty(edbId))
        Return GetFirstAvailableEdbId(ids)
    End Function

    Friend Function CreateEdbId(kblIds As IEnumerable(Of String)) As String()
        Return kblIds.Select(Function(kblId) Me.CreateEdbId(kblId)).Where(Function(edbId) Not String.IsNullOrEmpty(edbId)).ToArray
    End Function

    Friend Function CreateEdbId(kblId As String) As String
        If Not String.IsNullOrEmpty(kblId) Then
            Dim occ As IKblOccurrence = Nothing
            If _kblMapper.KBLOccurrenceMapper.TryGetValue(kblId, occ) Then
                Return CreateEdbId(occ)
            End If
        End If
        Return String.Empty
    End Function

    Friend Function CreateEdbId(occ As IKblOccurrence) As String
        If occ IsNot Nothing Then
            Dim createEdbIdAction As Func(Of String, String) = Function(id As String) IdConverter.GetCombined(Me.Id.ToString, id)

            If TypeOf occ Is [Lib].Schema.Kbl.Segment Then
                Return String.Empty 'HINT: ignore the segments because they do not belong to the connectivity 
            End If

            Return createEdbIdAction.Invoke(occ.SystemId)
        End If
        Return String.Empty
    End Function

    Friend Sub OnHubSelectionChanged(sender As Object, e As InformationHubEventArgs)
        If (MainForm?.TopologyHub IsNot Nothing) Then
            MainForm.TopologyHub.SelectCompartments({_kblMapper.HarnessPartNumber}, Nothing)
        End If

        If (e.ObjectType = KblObjectType.Harness_module) Then
            _modulesHub.SelectionChangedFromDocumentForm(e.ObjectIds)
        End If

        If e.HighlightInConnectivityView AndAlso MainForm IsNot Nothing Then
            MainForm.SelectSchematicsEntities(Me.CreateEdbId(e.ObjectIds).ToArray)
        End If

        If _cavitiesDocumentView IsNot Nothing AndAlso _cavitiesDocumentView.Model IsNot Nothing AndAlso Not _cavitiesDocumentView.Model.IsSelecting AndAlso e.ObjectIds.Count > 0 Then
            _cavitiesDocumentView.Model.Selected.Reset(_cavitiesDocumentView.Model.TryGetFirstConnectorAndCavityViewsFromKbl(e.ObjectIds))
        End If

        For Each drawingTab As UltraTab In Me.utcDocument.Tabs
            If (drawingTab.Visible) AndAlso (drawingTab.TabPage.Controls.Count <> 0) Then
                TrySelectInDrawingCanvas(drawingTab, e)
            End If
        Next

        RaiseEvent HubSelectionChanged(Me, e)
    End Sub

    Private Function TrySelectInDrawingCanvas(drawingTab As UltraTab, e As InformationHubEventArgs) As Boolean
        If TypeOf drawingTab.TabPage.Controls(0) Is DrawingCanvas Then
            Dim ids As New List(Of String)
            ids.AddRange(AddDimensionRelatedObjects(e.ObjectIds.ToArray))
            DirectCast(drawingTab.TabPage.Controls(0), DrawingCanvas).InformationHubSelectionChanged(ids, e.ObjectType, stampIds:=e.StampIds)
            Return True
        ElseIf TypeOf drawingTab.TabPage.Controls(0) Is D3D.Document.Controls.Document3DControl Then
            Dim docForm As DocumentForm = TryCast(TryCast(drawingTab.TabPage.Controls(0), D3D.Document.Controls.Document3DControl)?.ParentForm, DocumentForm)
            If docForm IsNot Nothing Then
                docForm.SelectIn3DView(e.ObjectIds.ToArray)
                Return True
            End If
        End If
        Return False
    End Function

    Private Function AddModulContentIds(ParamArray kblIds() As String) As List(Of String)
        Dim ids As New List(Of String)
        If kblIds IsNot Nothing Then
            For Each id As String In kblIds
                ids.Add(id)
                If (_kblMapper.KBLModuleObjectMapper.ContainsKey(id)) Then
                    ids.AddRange(_kblMapper.KBLModuleObjectMapper(id).OfType(Of IKblBaseObject).Select(Function(obj) obj.SystemId))
                End If
            Next
        End If
        Return ids.Distinct.ToList
    End Function

    Private Function AddDimensionRelatedObjects(ParamArray kblIds() As String) As List(Of String)
        Dim ids As New List(Of String)
        If kblIds IsNot Nothing Then
            For Each id As String In kblIds
                ids.Add(id)
                If (_kblMapper.KBLOccurrenceMapper.ContainsKey(id) AndAlso TypeOf (_kblMapper.KBLOccurrenceMapper(id)) Is Dimension_specification) Then
                    Dim spec As Dimension_specification = CType(_kblMapper.KBLOccurrenceMapper(id), Dimension_specification)
                    If _kblMapper.KBLOccurrenceMapper.ContainsKey(spec.Origin) AndAlso TypeOf _kblMapper.KBLOccurrenceMapper(spec.Origin) Is Fixing_assignment Then
                        ids.Add(CType(_kblMapper.KBLOccurrenceMapper(spec.Origin), Fixing_assignment).Fixing)
                    Else
                        ids.Add(spec.Origin)
                    End If

                    If _kblMapper.KBLOccurrenceMapper.ContainsKey(spec.Target) AndAlso TypeOf _kblMapper.KBLOccurrenceMapper(spec.Target) Is Fixing_assignment Then
                        ids.Add(CType(_kblMapper.KBLOccurrenceMapper(spec.Target), Fixing_assignment).Fixing)
                    Else
                        ids.Add(spec.Target)
                    End If

                    For Each segId As String In HarnessAnalyzer.[Shared].Utilities.GetIdrefs(spec.Segments)
                        ids.Add(spec.Target)
                    Next
                End If
            Next
        End If
        Return ids.Distinct.ToList
    End Function

    Friend Function SelectIn3DView(ParamArray kblIds() As String) As Boolean
        If _D3DControl IsNot Nothing AndAlso _document?.IsOpen AndAlso Not _document.IsBusy AndAlso Not _D3DControl?.IsSelecting Then
            With _D3DControl
                Dim ids As New List(Of String)
                ids.AddRange(AddModulContentIds(kblIds))
                ids.AddRange(AddDimensionRelatedObjects(kblIds))

                .SelectEntitiesByKbl(ids.Distinct.ToList)
                .ZoomFitSelection()
                .Invalidate()
                Return True
            End With
        End If
        Return False
    End Function

    Friend Function IsInliner(connectorOccurrence As Connector_occurrence) As Boolean
        Dim matchCounter As Integer = 0
        With connectorOccurrence
            For Each inlinerIdentifer As InlinerIdentifier In (_settingsForm?.GeneralSettings.InlinerIdentifiers).OrEmpty
                Select Case inlinerIdentifer.KBLPropertyName
                    Case ObjectPropertyNameStrings.Id
                        If inlinerIdentifer.IsMatch(.Id) Then
                            matchCounter += 1
                        End If

                    Case ObjectPropertyNameStrings.AliasId
                        If (.Alias_id.Length <> 0) Then
                            For Each aliasId As Alias_identification In .Alias_id
                                If inlinerIdentifer.IsMatch(aliasId.Alias_id) Then
                                    matchCounter += 1
                                    Exit For
                                End If
                            Next
                        End If

                    Case ObjectPropertyNameStrings.Description
                        If (.Description IsNot Nothing) Then
                            If (inlinerIdentifer.IsMatch(.Description)) Then
                                matchCounter += 1
                            End If
                        End If
                    Case ObjectPropertyNameStrings.InstallationInformation
                        If (.Installation_information.Length <> 0) Then
                            For Each installationInformation As Installation_instruction In .Installation_information
                                If inlinerIdentifer.IsMatch(installationInformation.Instruction_value) Then
                                    matchCounter += 1
                                    Exit For
                                End If
                            Next
                        End If
                End Select
            Next
        End With

        Dim isKSL As Boolean = False
        If (_kblMapper.KBLPartMapper.ContainsKey(connectorOccurrence.Part)) AndAlso (DirectCast(_kblMapper.KBLPartMapper(connectorOccurrence.Part), Part).Description IsNot Nothing) Then
            isKSL = DirectCast(_kblMapper.KBLPartMapper(connectorOccurrence.Part), Connector_Housing).IsKSL
        End If

        If (_settingsForm?.GeneralSettings.InlinerIdentifiers.Count).GetValueOrDefault > 0 AndAlso matchCounter = _settingsForm.GeneralSettings.InlinerIdentifiers.Count AndAlso (Not isKSL) Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Async Function OpenDocument() As Task(Of IResult)
        Return Await OpenDocument(New List(Of String))
    End Function

    <Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=True)>
    Public Async Function OpenDocument(modulePartNumbersForActivation As List(Of String), Optional loadDrawings As Boolean = True) As Task(Of IResult)
        Using Await _busyState.BeginWaitAsync ' create a big busy block without gaps
            Await TaskEx.WaitUntil(Function() Not _hcvFile.IsOpening)
            If Not _hcvFile.IsXhcvChild Then
                _hcvFile.OpenMode = HcvFileOpenMode.OpenOnDemand ' HINT: change do on-demand when single hcv-file because we want to let the file open when the data is needed within (thats within the async thread and does not block the ui, instead opening here directly)
            End If

#If CONFIG = "Release" OrElse RELEASE Then
            With MainForm.AppInitManager.LicenseManager
                Dim res As [Lib].Licensing.LicenseManagerBase.AuthFeaturesResult = .AuthenticateAvailableFeatures()

                If res.IsFaulted Then
                    If (Not MainForm.AppInitManager.IsInTestMode) Then
                        MessageBoxEx.ShowError(res.Message)
                    Else
                        Environment.Exit(res.DiagnosticsErrorCode)
                    End If

                    Return New Result(ResultState.Faulted, "Error init license: " & res.Message)
                End If
            End With
#End If

            Me.udmDocument.BeginUpdate()

            _document = MainForm?.Project.Documents.AddNew(_hcvFile)
            If _document IsNot Nothing Then
                _document.IsXhcv = IsExtendedHCV
            End If

            'HINT Add log hub dockable control pane
            Dim logHubPane As New DockableControlPane(PaneKeys.LogHub.ToString, UIStrings.LogPane_Caption)
            With logHubPane
                .Settings.CanDisplayAsMdiChild = Infragistics.Win.DefaultableBoolean.False
                .Settings.AllowPin = Infragistics.Win.DefaultableBoolean.False
                .Size = New Drawing.Size(400, 100)

                Me.udmDocument.ControlPanes.Add(logHubPane)

                .Dock(DockedSide.Top)
                .Close()
            End With

            Me.udmDocument.ControlPanes(PaneKeys.LogHub.ToString).Control = _logHub
            Me.udmDocument.EndUpdate()

            _modulePartNumbersForActivation = New List(Of String)

            If (modulePartNumbersForActivation IsNot Nothing) Then
                _modulePartNumbersForActivation.AddRange(modulePartNumbersForActivation)
            End If

            For Each kblFile As [Lib].IO.Files.Hcv.KblContainerFile In _hcvFile.OfType(Of [Lib].IO.Files.Hcv.KblContainerFile).ToArray
                'HINT Start background working process for loading KBL data objects to InformationHub control
                ToggleEnableStateOfMenuTools(False)

                Dim workerPack As New BackgroundWorkerPack(kblFile, loadDrawings)
                Dim result As IResult = System.ComponentModel.Result.Success

                ' HINT start work for "OpenDocument-Work" / blocks twice "OpenDocument"-calls
                Dim stateD3D As New D3DWorkState(AddressOf ProgressLoadKBLDataModel)
                Dim work As New WorkDescriptor(Sub(userdata As Object)
                                                   Dim data As Tuple(Of D3DWorkState, BackgroundWorkerPack) = CType(userdata, Tuple(Of D3DWorkState, BackgroundWorkerPack))
                                                   LoadKBLDataModel(data.Item1, data.Item2)
                                               End Sub,
                                               Function(res, userData)
                                                   Dim data As Tuple(Of D3DWorkState, BackgroundWorkerPack) = CType(userData, Tuple(Of D3DWorkState, BackgroundWorkerPack))
                                                   Dim newState As New D3DWorkState(data.Item1.Handler, res)
                                                   Return LoadKBLDataModelFinished(newState, data.Item2)
                                               End Function, New Tuple(Of D3DWorkState, BackgroundWorkerPack)(stateD3D, workerPack))
                Dim resKbl As IResult = Await StartWorkCore(work) ' skip making a busy block, because we already have created one
                If (resKbl?.IsFaultedOrCancelled).GetValueOrDefault Then
                    If resKbl.IsFaulted Then
                        Me._logHub.WriteLogMessage(resKbl.Message, LogEventArgs.LoggingLevel.Error)
                    End If
                    Return resKbl
                End If
            Next
        End Using

        If MainForm Is Nothing Then 'HINT: when documentForm is shown in standalone we directly show here the canvas-content because normally the mainstateMachine would do this, but here it doesn't exist
            Me.ActiveDrawingCanvas.DisplayDrawing()
        End If

        Return Result.Success
    End Function

    Friend Sub ZoomWindowAction()
        If IsDocument3DActive Then
            _D3DControl.SetZoomWindowActionMode()
        ElseIf (_activeDrawingCanvas IsNot Nothing) Then
            _activeDrawingCanvas.vdCanvas.BaseControl.ActiveDocument.CommandAction.Zoom("W", "USER", "USER")
        End If
    End Sub

    Friend Sub CancelPanIn3D()
        If _D3DControl IsNot Nothing Then
            _D3DControl.CancelPanActionMode()
        End If
    End Sub

    Friend Sub CancelMagnifierIn3D()
        If _D3DControl IsNot Nothing Then
            _D3DControl.SetMagnifyActionMode(False)
        End If
    End Sub

    Friend Sub HandlePanAction(enabled As Boolean)
        If _D3DControl IsNot Nothing Then
            If enabled Then
                _D3DControl.StartPanActionMode()
            Else
                _D3DControl.CancelPanActionMode() 'HINT: event PanActionDisabled is fired through event-chain of DocumentView
            End If
        End If

        If _activeDrawingCanvas IsNot Nothing AndAlso _activeDrawingCanvas.vdCanvas.BaseControl.ActiveDocument IsNot Nothing Then
            Dim doc As vdDocument = _activeDrawingCanvas.vdCanvas.BaseControl.ActiveDocument
            If PanAction IsNot Nothing Then
                PanAction.CancelAction(PanAction)
            End If
            If enabled Then
                PanAction = New ActionUtility.ActionPan(doc.ActionLayout)
                If Not doc.ActiveLayOut.Actions.Contains(PanAction) Then doc.ActiveLayOut.ActionAdd(PanAction)
                PanAction.Start()
            End If
        End If
    End Sub

    Friend Sub HandleMagnifierAction(enable As Boolean)
        If _D3DControl IsNot Nothing Then
            _D3DControl.SetMagnifyActionMode(enable)
        End If

        If Not IsDocument3DActive AndAlso _activeDrawingCanvas IsNot Nothing Then
            Dim doc As vdObjects.vdDocument = _activeDrawingCanvas.vdCanvas.BaseControl.ActiveDocument
            If MagnifierAction IsNot Nothing Then MagnifierAction.CancelAction(MagnifierAction)
            If enable Then
                If doc IsNot Nothing AndAlso doc.ActiveLayOut.ActiveViewPort Is Nothing Then
                    Dim scaleFac As Double = _activeDrawingCanvas.CalculateScaleFac4Magnifier()
                    If MagnifierAction IsNot Nothing Then
                        MagnifierAction.CancelAction(MagnifierAction)
                    End If

                    MagnifierAction = New VdMagnifier(doc.ActiveLayOut, (_settingsForm?.GeneralSettings.MagnifierSize).GetValueOrDefault, (_settingsForm?.GeneralSettings.MagnifierZoom).GetValueOrDefault, scaleFac)

                    If Not doc.ActiveLayOut.Actions.Contains(MagnifierAction) Then
                        doc.ActiveLayOut.ActionAdd(MagnifierAction)
                    End If

                    MagnifierAction.Start()
                End If
            End If
        End If
    End Sub


    Friend Sub ZoomInAction()
        If IsDocument3DActive Then
            _D3DControl.Model3DControl1.Design.ZoomIn(10)
            _D3DControl.Model3DControl1.Design.Invalidate()
        ElseIf _activeDrawingCanvas IsNot Nothing Then
            _activeDrawingCanvas.vdCanvas.BaseControl.ActiveDocument.CommandAction.Zoom("S", 1.25, 0)
        End If
    End Sub

    Friend Sub ZoomOutAction()
        If IsDocument3DActive Then
            _D3DControl.Model3DControl1.Design.ZoomOut(10)
            _D3DControl.Model3DControl1.Design.Invalidate()
        ElseIf (_activeDrawingCanvas IsNot Nothing) Then
            _activeDrawingCanvas.vdCanvas.BaseControl.ActiveDocument.CommandAction.Zoom("S", (1 / 1.25), 0)
        End If
    End Sub

    Private Sub _document3DView_PanningCancelled(sender As Object, e As EventArgs) Handles _D3DControl.PanningCancelled
        OnPanActionDisabled(New EventArgs)
    End Sub

    Private Sub _document3DView_ActionModeChanged(sender As Object, e As D3D.Document.Controls.ActionModeChangedEventArgs) Handles _D3DControl.ActionModeChanged
        If e.OldActionMode = devDept.Eyeshot.actionType.MagnifyingGlass Then
            OnMagnifierActionDisabled(New EventArgs)
        End If
    End Sub

    Protected Friend Overridable Sub OnPanActionDisabled(e As EventArgs)
        RaiseEvent PanActionDisabled(Me, e)
    End Sub

    Protected Friend Overridable Sub OnMagnifierActionDisabled(e As EventArgs)
        RaiseEvent MagnifierActionDisabled(Me, e)
    End Sub

    Private Function CreateDocument3DTab() As D3D.Document.Controls.Document3DControl
        If Not utcDocument.Tabs.Exists(TAB_DOC3D_KEY) Then

            If _D3DControl Is Nothing Then
                _D3DControl = New D3D.Document.Controls.Document3DControl(_settingsForm.GeneralSettings)
                AddHandler _D3DControl.SelectRowsInInformationHub, AddressOf SelectRowsInInformationHub
            End If

            Dim drawingTab As UltraTab = Me.utcDocument.Tabs.Add(TAB_DOC3D_KEY, UIStrings.Document3DPane_Caption)
            drawingTab.TabPage.Controls.Add(_D3DControl)

            _D3DControl.Dock = DockStyle.Fill
        End If
        Return _D3DControl
    End Function

    Friend Async Sub ResetDockableControls()
        If Not _isResettingDockControls Then
            Using Await _lock_reset_update.BeginWaitAsync
                _isResettingDockControls = True
                Try
                    Me.udmDocument.BeginUpdate()
                    Me.udmDocument.DockAreas.Clear()
                    Me.udmDocument.ControlPanes.Clear()
                    Me._D3DControl.Model3DControl1.Design.SuspendUpdate(True)
                    If _logHub IsNot Nothing Then
                        Dim logHubPane As New DockableControlPane(PaneKeys.LogHub.ToString, UIStrings.LogPane_Caption)
                        With logHubPane
                            .Settings.CanDisplayAsMdiChild = Infragistics.Win.DefaultableBoolean.False
                            .Settings.AllowPin = Infragistics.Win.DefaultableBoolean.False
                            .Size = New Drawing.Size(400, 100)
                            .Control = _logHub

                            udmDocument.ControlPanes.Add(logHubPane)

                            .Dock(DockedSide.Top)
                            .Close()
                        End With
                    End If

                    'HINT Add information hub dockable control pane on bottom window position
                    If _informationHub IsNot Nothing Then
                        Dim informationHubPane As New DockableControlPane(PaneKeys.InformationHub.ToString, UIStrings.DataTablesPane_Caption)
                        With informationHubPane
                            .Settings.CanDisplayAsMdiChild = Infragistics.Win.DefaultableBoolean.False
                            .Settings.AllowPin = Infragistics.Win.DefaultableBoolean.False
                            .Size = New Drawing.Size(200, 200)
                            .Control = _informationHub
                            Me.udmDocument.ControlPanes.Add(informationHubPane)

                            .Dock(DockedSide.Bottom)
                        End With
                    End If

                    'HINT Add drawings hub dockable control pane on top left window position
                    If _drawingsHub IsNot Nothing Then
                        Dim drawingsHubPane As New DockableControlPane(PaneKeys.DrawingsHub.ToString, UIStrings.DrawingsPane_Caption)
                        With drawingsHubPane
                            .Settings.CanDisplayAsMdiChild = Infragistics.Win.DefaultableBoolean.False
                            .Settings.AllowPin = Infragistics.Win.DefaultableBoolean.False
                            .Size = New Drawing.Size(200, 75)
                            .Control = _drawingsHub

                            Me.udmDocument.ControlPanes.Add(drawingsHubPane)

                            .Dock(DockedSide.Left)
                        End With
                    End If

                    'HINT Add memolist hub dockable control pane on right window position
                    If _memolistHub IsNot Nothing Then
                        Dim memolistHubPane As New DockableControlPane(PaneKeys.MemolistHub.ToString, UIStrings.MemolistPane_Caption)
                        With memolistHubPane
                            .Settings.CanDisplayAsMdiChild = Infragistics.Win.DefaultableBoolean.False
                            .Settings.AllowPin = Infragistics.Win.DefaultableBoolean.False
                            .Size = New Drawing.Size(200, 200)
                            .Control = _memolistHub

                            Me.udmDocument.ControlPanes.Add(memolistHubPane)

                            .Dock(DockedSide.Right)
                            .Close()
                        End With

                    End If

                    'HINT Add modules hub dockable control pane on lower left window position
                    If _modulesHub IsNot Nothing Then
                        Dim modulesHubPane As New DockableControlPane(PaneKeys.ModulesHub.ToString, UIStrings.ModulesPane_Caption)
                        With modulesHubPane
                            .Settings.CanDisplayAsMdiChild = Infragistics.Win.DefaultableBoolean.False
                            .Settings.AllowPin = Infragistics.Win.DefaultableBoolean.False
                            .Size = New Drawing.Size(200, 200)
                            .Control = _modulesHub

                            udmDocument.ControlPanes.Add(modulesHubPane)

                            If Me.udmDocument.ControlPanes.Exists(PaneKeys.DrawingsHub.ToString) Then
                                With Me.udmDocument.PaneFromKey(PaneKeys.DrawingsHub.ToString)
                                    .DockAreaPane.ChildPaneStyle = ChildPaneStyle.HorizontalSplit
                                    .DockAreaPane.Panes.Add(modulesHubPane)
                                End With
                            End If
                        End With
                    End If

                    If _navigatorHub IsNot Nothing Then
                        Dim navigatorHubPane As New DockableControlPane(PaneKeys.NavigatorHub.ToString, UIStrings.NavigatorPane_Caption)
                        With navigatorHubPane
                            .Settings.CanDisplayAsMdiChild = Infragistics.Win.DefaultableBoolean.False
                            .Settings.AllowPin = Infragistics.Win.DefaultableBoolean.False
                            .Settings.Appearance.BackColor = Color.White
                            .Size = New Drawing.Size(200, 75)
                            .Control = _navigatorHub

                            udmDocument.ControlPanes.Add(navigatorHubPane)

                            If Me.udmDocument.ControlPanes.Exists(PaneKeys.DrawingsHub.ToString) Then
                                With Me.udmDocument.PaneFromKey(PaneKeys.DrawingsHub.ToString)
                                    .DockAreaPane.ChildPaneStyle = ChildPaneStyle.HorizontalSplit
                                    .DockAreaPane.Panes.Add(navigatorHubPane)
                                End With
                            End If
                        End With
                    End If

                    Me._D3DControl.Model3DControl1.Design.SuspendUpdate(False)
                    Me.udmDocument.EndUpdate()
                    Await Task.Delay(500) 'HINT: this is a temporarily solution for issue #39 - we don't know if the exception comes from the virtual system emulation or it's a general bug - this delay will reduce the possible problem to a minimum for now - > for more info see HA issue #39
                Finally
                    _isResettingDockControls = False
                End Try
            End Using
        End If
    End Sub

    Friend Function SaveDocument(Optional fileName As String = Nothing) As Boolean
        If _hcvFile IsNot Nothing Then
            Using Me.EnableWaitCursor
                _memolistHub.UpdateData(_memoList.MemoItems)

                If IsExtendedHCV Then
                    If String.IsNullOrEmpty(fileName) Then ' just save the whole xhcv when a single hcv has changed
                        If (_settingsForm?.HasView3DFeature AndAlso MainForm?.D3D?.Consolidated?.CurrentTrans IsNot Nothing) Then
                            MainForm.MainStateMachine.XHcvFile.CarTransformation = MainForm.D3D.Consolidated.GetTransformationAsContainerFile
                        End If

                        MainForm?.MainStateMachine.SaveXhcV()

                        _dirty = False

                        Return True
                    Else ' in this case the user want's to "extract" a hcv from the xhcv, so we save the hcv to the given path and let the xhcv in the current state
                        Dim saveError As String = SaveHcv(fileName)
                        If Not String.IsNullOrEmpty(saveError) Then
                            MessageBoxEx.ShowError(String.Format(DocumentFormStrings.ErrorCompressHCV_Msg, vbCrLf, saveError))
                            Return False
                        End If
                        Return True
                    End If
                Else ' HINT: a normal save of a single hcv to the current path or to a new path
                    Dim saveError As String = SaveHcv(fileName)
                    If Not String.IsNullOrEmpty(saveError) Then
                        MessageBoxEx.ShowError(String.Format(DocumentFormStrings.ErrorCompressHCV_Msg, vbCrLf, saveError))
                        Return False
                    End If
                    Return True
                End If
            End Using
        Else
            MessageBoxEx.ShowWarning(String.Format(DocumentFormStrings.CannotSaveForKBL_Msg, IO.Path.GetFileNameWithoutExtension(_hcvFile.FullName), vbCrLf))
            Return False
        End If
    End Function

    Friend Sub SetActiveConnectiviy(ParamArray entities() As EdbConversionEntity)
        Me.SetActiveConnectiviy(entities, True)
    End Sub

    Friend Sub SetActiveConnectiviy(entities As IEnumerable(Of EdbConversionEntity), update As Boolean)
        'HINT: Doing an change on active modules while generation in in progress would normally not work here (can't read the model until finished generating async). 
        '      But we have implemented an instant IsActive-setter, on every entity that was newly created by the converter, in the mainForm (set's the IsActive on the current module state). See event: "AfterEdbEntityCreated" on control in mainform.
        If MainForm IsNot Nothing AndAlso MainForm.SchematicsView IsNot Nothing AndAlso entities IsNot Nothing Then
            Dim myEntities As List(Of EdbConversionEntity) = entities.Where(Function(ent) ent.IsVirtual OrElse ent.DocumentId = Me.Id.ToString).ToList
            If myEntities.Count > 0 Then
                Dim myDocumentBlock As Schematics.Converter.EdbConversionDocument = Nothing
                If MainForm.SchematicsView.Entities.TryGetDocument(Me.Id.ToString, myDocumentBlock) Then
                    Dim inactiveIds As New HashSet(Of String)

                    For Each inactiveModule As KeyValuePair(Of String, [Lib].Schema.Kbl.[Module]) In _kblMapper.InactiveModules
                        Dim objectsInModule As IGroupingKblObjects = Nothing
                        If (_kblMapper.KBLModuleObjectMapper.TryGetValue(inactiveModule.Key, objectsInModule)) Then
                            For Each kblObject As IKblBaseObject In objectsInModule.Cast(Of IKblBaseObject)
                                inactiveIds.Add(kblObject.SystemId)
                            Next
                        End If
                    Next

                    For Each activeModule As [Lib].Schema.Kbl.[Module] In _kblMapper.GetModules.Except(_kblMapper.InactiveModules.Values)
                        Dim objectsInModule As IGroupingKblObjects = Nothing
                        If (_kblMapper.KBLModuleObjectMapper.TryGetValue(activeModule.SystemId, objectsInModule)) Then
                            For Each kblObject As IKblBaseObject In objectsInModule.Cast(Of IKblBaseObject)
                                inactiveIds.Remove(kblObject.SystemId)
                            Next
                        End If
                    Next

                    Dim componentsOfConnectors As New Dictionary(Of String, EdbConversionComponentEntity)

                    For Each entity As EdbConversionEntity In myEntities
                        If entity IsNot Nothing Then
                            entity.IsActive = Not inactiveIds.ContainsAnyOf(entity.OriginalIds)
                            Dim connectorEntity As EdbConversionConnectorEntity = TryCast(entity, EdbConversionConnectorEntity)
                            If connectorEntity IsNot Nothing Then
                                componentsOfConnectors.TryAdd(connectorEntity.OwnerComponent.Id, connectorEntity.OwnerComponent)
                            End If
                        End If
                    Next

                    For Each edbComp As EdbConversionComponentEntity In myEntities.OfType(Of EdbConversionComponentEntity).Where(Function(comp) comp.IsVirtual).Union(componentsOfConnectors.Values)
                        edbComp.UpdateIsActive(myDocumentBlock)
                    Next

                    For Each edbCav As EdbConversionCavityEntity In myEntities.OfType(Of EdbConversionCavityEntity)()
                        Dim parentConnector As EdbConversionEntity = Nothing
                        If myDocumentBlock.TryGetEntity(edbCav.ParentConnectorEdbid, parentConnector) Then
                            edbCav.IsActive = Not inactiveIds.ContainsAnyOf(parentConnector.OriginalIds)
                        End If
                    Next

                    If update Then
                        MainForm.SchematicsView.Update()
                    End If
                End If
            End If
        End If
    End Sub

    Friend Sub SetDirty()
        _dirty = True

        If MainForm IsNot Nothing Then
            MainForm.Tools.Application.SaveDocument.SharedProps.Enabled = True
            MainForm.Tools.Application.SaveDocuments.SharedProps.Enabled = True
        End If
    End Sub

    Friend Sub ValidateKBL(kbl As Hcv.KblContainerFile, reportSuccess As Boolean)
        Dim result As KBLValidationResult = kbl.Validate
        If (result.Errors.Count <> 0 OrElse result.Warnings.Count <> 0) Then
            For Each errMsg As String In result.Errors
                _logHub.WriteLogMessage(New LogEventArgs(LogEventArgs.LoggingLevel.Error, DocumentFormStrings.ErrorKBLValidation_Msg & errMsg))
            Next

            For Each wMsg As String In result.Warnings
                _logHub.WriteLogMessage(New LogEventArgs(LogEventArgs.LoggingLevel.Warning, DocumentFormStrings.WarningKBLValidation_LogMsg & wMsg))
            Next

            MainForm?.AppInitManager.ErrorManager.ShowMsgBoxErrorOrWriteConsole(String.Format(ErrorStrings.DocStatMachine_ErrorSchemaValidation_Msg, IO.Path.GetFileName(_hcvFile.FullName)), ErrorCodes.ERROR_BAD_FORMAT)
        ElseIf (reportSuccess) Then
            MainForm?.AppInitManager.ErrorManager.ShowMsgBoxInformationOrWriteConsole(String.Format(DialogStrings.DocStatMachine_SchemValidationSuccess_Msg, IO.Path.GetFileName(_hcvFile.FullName)))
        Else
            _logHub.WriteLogMessage(New LogEventArgs(LogEventArgs.LoggingLevel.Information, String.Format(DialogStrings.DocStatMachine_SchemValidationSuccess_Msg, IO.Path.GetFileName(_hcvFile.FullName))))
        End If
    End Sub

    Private Sub DocumentForm_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        Dim otherHCVDocFromExtenedHCVExists As Boolean = False
        If MainForm?.utmmMain?.TabGroups IsNot Nothing Then
            For Each tabGroup As MdiTabGroup In MainForm.utmmMain.TabGroups
                For Each tab As MdiTab In tabGroup.Tabs
                    Dim documentForm As DocumentForm = TryCast(tab.Form, DocumentForm)
                    If documentForm IsNot Nothing Then
                        If (documentForm Is Me) Then
                            tab.Text = "<Closed>"
                        End If

                        If (documentForm IsNot Me) AndAlso (Not tab.Text = "<Closed>") AndAlso (documentForm.IsExtendedHCV) Then
                            otherHCVDocFromExtenedHCVExists = True
                        End If
                    End If
                Next
            Next
        End If

        If (MainForm?.MainStateMachine?.OverallConnectivity IsNot Nothing) Then
            MainForm.MainStateMachine.OverallConnectivity.HarnessConnectivities.TryRemove(_kblMapper.HarnessPartNumber, Nothing)
        End If

        If (IsExtendedHCV) AndAlso (Not otherHCVDocFromExtenedHCVExists) Then
            If (MainForm.udmMain.ControlPanes.Exists(PaneKeys.TopologyHub.ToString)) Then
                If (MainForm.Panes.TopologyHubPane.IsVisible) Then
                    MainForm.Panes.TopologyHubPane.Close()
                End If

                MainForm.TopologyHub.vDraw.Dispose()
                MainForm.TopologyHub.Dispose()
                MainForm.Tools.Settings.DisplayTopologyHub.SharedProps.Visible = False
            End If

            MainForm.MainStateMachine.EntireCarSettings = Nothing
            MainForm.MainStateMachine.XHcvFile?.Dispose()
            MainForm.MainStateMachine.XHcvFile = Nothing
            MainForm.MainStateMachine.OverallConnectivity = Nothing
        End If

        With _messageEventArgs
            .SetHideProgress()
            .SetClearStatusMsg()
            .SetUseLastPosition()
        End With

        MainForm?.RemoveDocumentFromConnectivityView(Me.Id)
        _cavitiesDocumentView?.NotifyClosed()

        MainForm?.Project.Documents.Remove(_document)

        OnMessage(_messageEventArgs)
    End Sub

    Private ReadOnly Property IsLastDocWith3DChanges As Boolean
        Get
            Return (MainForm?.utmmMain.TabGroups.All.Cast(Of MdiTabGroup).Sum(Function(g) g.Tabs.Count) = 1 AndAlso (MainForm?.D3D.Consolidated IsNot Nothing AndAlso MainForm?.D3D.Consolidated.HasChanges AndAlso MainForm?.D3D.Consolidated.CurrentTrans IsNot Nothing)).GetValueOrDefault
        End Get
    End Property

    Private Async Sub DocumentForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        'HINT Prevent closing of document if background worker still busy
        If Me.IsBusy OrElse _isWaitingToCanClose Then
            e.Cancel = True
            Exit Sub
        End If

        If Not _isClosingClose Then ' avoid double messages when close call is coming from within FormClosing
            If _cavitiesDocumentView IsNot Nothing Then _cavitiesDocumentView.NotifyClosing()
            'HINT Check for changes in 3D view and prompt question to user if this changes should be saved into document
            If Me.IsDirty OrElse (MainForm?.HasView3DFeature) AndAlso (MainForm?.D3D?.Consolidated?.HasChanges) Then
                If Me.IsDirty OrElse IsLastDocWith3DChanges Then
                    Dim msg As String = DocumentFormStrings.SaveChangesForDoc_Msg

                    If IsLastDocWith3DChanges Then
                        msg = DocumentFormStrings.SaveChangesForDocWith3D
                    End If

                    Select Case MessageBoxEx.ShowQuestion(String.Format(msg, Me.Text), MessageBoxButtons.YesNoCancel)
                        Case DialogResult.Yes
                            If (IsLastDocWith3DChanges AndAlso Not IsExtendedHCV) Then
                                TryCast(MainForm?.D3D, D3D.MainFormController)?.SaveCarTransformationAction()
                            End If

                            If Me.IsExtendedHCV OrElse _dirty Then
                                If Not SaveDocument() Then
                                    e.Cancel = True
                                    Exit Sub
                                End If
                            End If
                        Case DialogResult.Cancel
                            e.Cancel = True
                            Exit Sub
                    End Select
                End If
            End If

            If (MainForm?.MainStateMachine._compareForm IsNot Nothing AndAlso MainForm?.MainStateMachine._compareForm.Visible) OrElse (MainForm?.MainStateMachine._graphicalCompareForm IsNot Nothing AndAlso MainForm?.MainStateMachine._graphicalCompareForm.Visible) Then
                If MessageBoxEx.ShowQuestion(DocumentFormStrings.CloseHCVAndCompDlgs_Msg) = DialogResult.No Then
                    e.Cancel = True
                    Exit Sub
                End If
            End If
        End If

        _analysisStateMachine.CloseOpenForms()
        If _D3DControl IsNot Nothing Then
            _D3DControl.ClearClockSymbolsLabels() 'HINT: It maybe a Bug from Eyeshot that we have to clear all Eyeshot objects first before we can close the Document, otherwise the application hangs !!
            _D3DControl.ClearViewVertexDirections()
            _D3DControl.ClearDimensionLabels()
            _D3DControl.ClearRedliningLabels()
            _D3DControl.ClearHintLabels()
        End If

        If D3DCancellationTokenSource.CanBeCancelled Then
            _d3dAsyncLoadCancelSource.CancelAndWait(TimeSpan.FromSeconds(5))
        End If

        If _recalcLock.CurrentCount = 0 Then
            If Not _isClosingClose Then
                e.Cancel = True
                _cancelRecalculation = True
                Await _recalcLock.WaitAsync
                Try
                    _isClosingClose = True
                    Try
                        _cancelRecalculation = False
                        Me.Close()
                    Finally
                        _isClosingClose = False
                    End Try

                Finally
                    _recalcLock.Release()
                End Try
                Exit Sub
            End If
        End If

        _informationHub.timerE3Application.Stop()
        _informationHub.timerE3Application.Dispose()

        'HINT Remove referenced counter inliners in remaining opened HCV documents
        If MainForm?.utmmMain?.TabGroups IsNot Nothing Then
            For Each tabGroup As MdiTabGroup In MainForm.utmmMain.TabGroups
                For Each tab As MdiTab In tabGroup.Tabs
                    Dim documentForm As DocumentForm = TryCast(tab.Form, DocumentForm)
                    If (documentForm IsNot Nothing) AndAlso (documentForm IsNot Me) AndAlso (documentForm.Inliners IsNot Nothing) AndAlso (documentForm.Inliners.Count <> 0) Then
                        For Each inliner As Connector_occurrence In _inliners.Keys
                            Dim counterInliners As New List(Of Connector_occurrence)

                            For Each inlinerPair As KeyValuePair(Of Connector_occurrence, Connector_occurrence) In documentForm.Inliners
                                If (inlinerPair.Value IsNot Nothing) AndAlso (inlinerPair.Value.Equals(inliner)) Then
                                    counterInliners.Add(inlinerPair.Key)
                                End If
                            Next

                            For Each counterInliner As Connector_occurrence In counterInliners
                                documentForm.Inliners(counterInliner) = Nothing
                            Next
                        Next
                    End If
                Next
            Next
        End If

        If (MainForm?.MainStateMachine._compareForm IsNot Nothing) AndAlso (MainForm?.MainStateMachine._compareForm.Visible) Then
            MainForm.MainStateMachine._compareForm.Close()
            MainForm.MainStateMachine._compareForm.Dispose()
        End If

        If (MainForm?.MainStateMachine._graphicalCompareForm IsNot Nothing) AndAlso (MainForm?.MainStateMachine._graphicalCompareForm.Visible) Then
            MainForm.MainStateMachine._graphicalCompareForm.Close()

            If (Not MainForm.MainStateMachine._graphicalCompareForm.Visible) Then
                MainForm.MainStateMachine._graphicalCompareForm.Dispose()
            End If
        End If

        MainForm?.SearchMachine.RemoveKBLSearchPattern(_kblMapper)
        MainForm?.SearchMachine.VisibleSearchForm(False)

        If (MainForm?.HasView3DFeature) AndAlso (MainForm?.D3D.Consolidated IsNot Nothing) Then
            MainForm.D3D.Consolidated.RemoveJTModel()
        End If

        If (_documentStateMachine._bomForm IsNot Nothing) AndAlso (_documentStateMachine._bomForm.Visible) Then
            _documentStateMachine._bomForm.Close()
            _documentStateMachine._bomForm.Dispose()
        End If

        If (_documentStateMachine._ultrasonicSpliceTerminalDistanceForm IsNot Nothing) AndAlso (_documentStateMachine._ultrasonicSpliceTerminalDistanceForm.Visible) Then
            _documentStateMachine._ultrasonicSpliceTerminalDistanceForm.Close()
            _documentStateMachine._ultrasonicSpliceTerminalDistanceForm.Dispose()
        End If

        If (_documentStateMachine._validityCheckForm IsNot Nothing) AndAlso (_documentStateMachine._validityCheckForm.Visible) Then
            _documentStateMachine._validityCheckForm.Close()
            _documentStateMachine._validityCheckForm.Dispose()
        End If

        'HINT Dispose all Vdraw canvas on the different tabs
        If utcDocument IsNot Nothing Then
            For Each tab As UltraTab In utcDocument.Tabs
                If tab.TabPage?.Controls.Count > 0 AndAlso TypeOf (tab.TabPage?.Controls(0)) Is DrawingCanvas Then
                    Dim canvas As DrawingCanvas = CType(tab.TabPage?.Controls(0), DrawingCanvas)

                    If (canvas.vdCanvas IsNot Nothing AndAlso canvas.vdCanvas.BaseControl IsNot Nothing AndAlso canvas.vdCanvas.BaseControl.ActiveDocument IsNot Nothing) Then
                        For Each entity As vdFigure In canvas.vdCanvas.BaseControl.ActiveDocument.Model.Entities
                            entity.Dispose()
                        Next
                        canvas.vdCanvas.BaseControl.ActiveDocument.ClearAll()
                    End If

                    If (canvas.vdCanvas IsNot Nothing) Then
                        canvas.vdCanvas.Dispose()
                    End If

                    canvas.Dispose()
                End If
            Next
        End If

        If (_hcvFile?.IsOpen).GetValueOrDefault Then
            _hcvFile?.Close()
        End If

        If _document?.IsOpen Then
            Dim res As IResult = _document.Close(True)
            If res.IsFaultedOrCancelled AndAlso Not String.IsNullOrEmpty(res.Message) Then
                If res.IsFaulted Then
                    _logHub.WriteLogMessage(New LogEventArgs(LogEventArgs.LoggingLevel.Error, res.Message))
                Else
                    _logHub.WriteLogMessage(New LogEventArgs(LogEventArgs.LoggingLevel.Warning, res.Message))
                End If
            End If
        End If
    End Sub

    Private Async Sub DocumentForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If (e.Control) AndAlso (e.KeyCode = Keys.F) AndAlso MainForm IsNot Nothing AndAlso (Not MainForm.SearchMachine.IsSearchFormVisible) Then
            MainForm.SearchMachine.CreateSearchFormWithPredefinedSearchString(MainForm.SearchMachine.BeginsWithEnabled, MainForm.SearchMachine.CaseSensitiveEnabled, False, MainForm.SearchMachine.CurrentSearchString)
        ElseIf (e.Control) AndAlso (e.KeyCode = Keys.O) AndAlso MainForm IsNot Nothing Then
            Await MainForm?.MainStateMachine.OpenDocument()
        ElseIf (e.Control) AndAlso (e.KeyCode = Keys.P) AndAlso MainForm IsNot Nothing Then
            MainForm.MainStateMachine.PrintDocument()
        ElseIf (e.Control) AndAlso (e.KeyCode = Keys.S) AndAlso (_dirty) AndAlso MainForm IsNot Nothing Then
            MainForm.MainStateMachine.SaveDocument()
        ElseIf (e.Control) AndAlso (e.KeyCode = Keys.V) AndAlso (Clipboard.ContainsText) AndAlso (Clipboard.GetText <> String.Empty) AndAlso (Clipboard.GetText <> " ") Then
            _documentStateMachine.PasteModulePartNumbersFromClipboard()
        ElseIf (e.Control) AndAlso (e.KeyCode = Keys.X) Then
            Me.Close()
        ElseIf _informationHub?.FindParent(Of Infragistics.Win.UltraWinDock.DockableWindow)?.Pane?.IsActive AndAlso _informationHub?.ActiveGrid?.ActiveRow Is Nothing Then
            Select Case e.KeyCode
                Case Keys.Down
                    _informationHub.ActiveGrid?.Select()
                    _informationHub.ActiveGrid?.Focus()
                    _informationHub.ActiveGrid?.ActiveRowScrollRegion?.FirstRow?.Activate()
                Case Keys.Up
                    _informationHub.ActiveGrid?.Select()
                    _informationHub.ActiveGrid?.Focus()
                    _informationHub.ActiveGrid?.ActiveRowScrollRegion?.FirstRow?.Activate()
            End Select
        End If
    End Sub

    Private Sub _activeDrawingCanvas_CanvasMouseMove(sender As Object, e As MouseEventArgs) Handles _activeDrawingCanvas.CanvasMouseMove
        With _messageEventArgs
            If (_activeDrawingCanvas.vdCanvas IsNot Nothing) AndAlso (_activeDrawingCanvas.vdCanvas.BaseControl.ActiveDocument IsNot Nothing) Then
                .Set(MessageEventArgs.MsgType.UseLastProgress Or MessageEventArgs.MsgType.UseLastStatusMsg)
                .XPosition = _activeDrawingCanvas.vdCanvas.BaseControl.ActiveDocument.CCS_CursorPos.x
                .YPosition = _activeDrawingCanvas.vdCanvas.BaseControl.ActiveDocument.CCS_CursorPos.y
                .MessageType = MessageEventArgs.MsgType.ShowCoordinates
            End If
        End With

        OnMessage(_messageEventArgs)
    End Sub

    Private Sub _activeDrawingCanvas_Message(sender As Object, e As MessageEventArgs) Handles _activeDrawingCanvas.Message
        OnMessage(e)
    End Sub

    Private Sub _documentStateMachine_LogMessage(sender As DocumentForm, e As LogEventArgs) Handles _documentStateMachine.LogMessage
        _logHub.WriteLogMessage(e)
    End Sub

    Private Sub _drawingsHub_CancelLoad(sender As DrawingsHub, e As DrawingsHubEventArgs) Handles _drawingsHub.CancelLoad
        If Me.IsBusy Then
            _busyState.CancelAll()
        End If
    End Sub

    Public Property ActiveDrawingCanvas As DrawingCanvas
        Get
            Return _activeDrawingCanvas
        End Get
        Set(value As DrawingCanvas)
            If _activeDrawingCanvas IsNot value Then
                _activeDrawingCanvas = value
                OnActiveDrawingCanvasChanged(New EventArgs)
            End If
        End Set
    End Property

    Private Async Sub _drawingsHub_HubCheckStateChanged(sender As DrawingsHub, e As DrawingsHubEventArgs) Handles _drawingsHub.HubCheckStateChanged
        Using e.SelectedNode.ProtectProperty(NameOf(UltraTreeNode.Enabled), False)
            Using Await _busyState.BeginWaitAsync
                If Not String.IsNullOrEmpty(e.File?.FullName) Then
                    If Me.utcDocument.Tabs.Exists(e.File.FullName) Then
                        If Not e.SelectedNode.CheckedState = CheckState.Checked AndAlso Me.utcDocument.Tabs(e.File.FullName).Active Then
                            If ActiveDrawingCanvas IsNot Nothing Then
                                ActiveDrawingCanvas.vdCanvas.BaseControl.ActiveDocument.CommandAction.Cancel()
                                ActiveDrawingCanvas = Nothing
                            End If
                        End If

                        Me.utcDocument.Tabs(e.File.FullName).Visible = e.SelectedNode.CheckedState = CheckState.Checked

                        For Each tab As UltraTab In Me.utcDocument.Tabs
                            If (tab.Visible) Then
                                Exit Sub
                            End If
                        Next

                        Me.udmDocument.PaneFromKey(PaneKeys.NavigatorHub.ToString).Close()

                        DirectCast(Me.utmDocument.Tools(SettingsTabToolKey.DisplayNavigatorHub.ToString), StateButtonTool).Checked = False
                    ElseIf TypeOf e.File Is SvgContainerFile Then
                        For Each tool As ToolBase In Me.utmDocument.Tools
                            If (tool.Key.StartsWith("Analysis")) AndAlso (DirectCast(tool, StateButtonTool).Checked) Then
                                DirectCast(tool, StateButtonTool).Checked = False
                            End If
                        Next

                        Dim selectedNode As UltraTreeNode = e.SelectedNode
                        selectedNode.LeftImages.Add(My.Resources.LoadDrawing.ToBitmap)
                        'HINT Create new drawing canvas control instance
                        Debug.WriteLine("Open:" & e.SelectedNode.Key)
                        Dim drawingCanvas As DrawingCanvas = CreateNewDrawingCanvas()

                        Await Task.Run(
                                        Sub()
                                            Try
                                                drawingCanvas.LoadAdditionalDrawing(CType(e.File, SvgContainerFile), If(_drawingFile Is Nothing, String.Empty, _drawingFile.FullName), _doNotMirror)
                                            Catch ex As Exception
#If DEBUG Or CONFIG = "Debug" Then
                                                Throw
#End If
                                                'HINT Create log message while an error occurred on loading SVG drawing
                                                _logHub.WriteLogMessage(New LogEventArgs(LogEventArgs.LoggingLevel.Warning, String.Format(My.Resources.LoggingStrings.SvgConv_LoadProblem, ex.Message)))
                                            End Try
                                        End Sub)

                        Try

                            If (KnownFile.IsDwg(_drawingFile?.FullName) OrElse KnownFile.IsDXF(_drawingFile?.FullName)) Then
                                drawingCanvas.DrawWideFrame = True
                            End If

                            drawingCanvas.DisableDrawEvents()
                            drawingCanvas.FilterActiveObjects(_informationHub.CurrentRowFilterInfo)
                            drawingCanvas.vdCanvas.BaseControl.ActiveDocument.ZoomExtents()
                            AddOrUpdateTab(e, drawingCanvas)

                            'HINT: Running Actions will be closed, because there is no way to restart a running action before the drawingtab is drawing on display

                            If (_settingsForm?.GeneralSettings.DisplaySVGValidationMessage) Then
                                Select Case drawingCanvas.ConversionValidationLevel
                                    Case SVGValidationLevel.Warning
                                        MessageBoxEx.ShowWarning(Me, String.Format(DrawingCanvasStrings.SVGValidationWarning_Msg, IO.Path.GetFileNameWithoutExtension(e.File.FullName)))
                                    Case SVGValidationLevel.Error
                                        MessageBoxEx.ShowError(Me, String.Format(DrawingCanvasStrings.SVGValidationError_Msg, IO.Path.GetFileNameWithoutExtension(e.File.FullName)))
                                End Select
                            End If
                        Catch ex As Exception
#If DEBUG Or CONFIG = "Debug" Then
                            Throw
#End If
                            'HINT Create log message while an error occurred on loading SVG drawing
                            _logHub.WriteLogMessage(New LogEventArgs(LogEventArgs.LoggingLevel.Warning, String.Format(My.Resources.LoggingStrings.SvgConv_LoadProblem, ex.Message)))
                        End Try
                    End If
                ElseIf e.SelectedNode.Key = DraftContainerFile.FILE_NAME Then
                    AddOrUpdateTab(e)
                End If
            End Using
        End Using
    End Sub

    Private Function CreateNewDrawingCanvas() As DrawingCanvas
        Return New DrawingCanvas(Me, _qmStamps, _redliningInformation)
    End Function

    Private Function AddOrUpdateTab(e As DrawingsHubEventArgs, Optional canvas As DrawingCanvas = Nothing) As UltraTab
        If canvas Is Nothing Then
            canvas = CreateNewDrawingCanvas()
        End If

        'HINT Add new drawing tab to documentForm
        Dim drawingTab As UltraTab = Nothing

        If Not String.IsNullOrEmpty(e.File?.FullName) AndAlso (Me.utcDocument.Tabs.Exists(e.File.FullName)) Then
            drawingTab = Me.utcDocument.Tabs(e.File.FullName)
        Else
            If e.File IsNot Nothing Then
                drawingTab = Me.utcDocument.Tabs.Add(e.File.FullName)
            Else
                drawingTab = Me.utcDocument.Tabs.Add(e.SelectedNode.Key)
            End If

            Me.utcDocument.SelectedTab = Nothing 'HINT MR kick ActiveTabChanged event after first created Tab!
            With drawingTab
                .TabPage.Controls.Add(canvas)
                If e.SelectedNode.Key = DraftContainerFile.FILE_NAME AndAlso e.File Is Nothing Then
                    .Appearance.FontData.Italic = Infragistics.Win.DefaultableBoolean.True
                End If

                If (TryCast(e.File, IContainerFile)?.Type).GetValueOrDefault = KnownContainerFileFlags.Draft Then
                    .Text = UIStrings.DraftDrawing_Caption
                Else
                    .Text = Path.GetFileNameWithoutExtension(e.File.FullName)
                End If
            End With
        End If

        e.SelectedNode.LeftImages.RemoveAt(1)
        _messageEventArgs.ProgressPercentage = 100
        OnMessage(_messageEventArgs)
        drawingTab.Selected = True
        Return drawingTab
    End Function

    Private Sub _informationHub_ApplyModuleConfiguration(sender As InformationHub, e As InformationHubEventArgs) Handles _informationHub.ApplyModuleConfiguration
        Dim tsk As Task = ApplyModuleConfiguration(e)
    End Sub

    Private Async Function ApplyModuleConfiguration(e As InformationHubEventArgs, Optional recalcBundleDiameters As Boolean = True, Optional updateInformationHubFilters As Boolean = True, Optional updateCanvasFilters As Boolean = True) As Task
        Using Me.EnableWaitCursor
            _kblMapper.InactiveModules.Clear()

            For Each [module] As [Lib].Schema.Kbl.[Module] In _kblMapper.GetModules
                If Not e.ObjectIds.Contains([module].SystemId) Then
                    _kblMapper.InactiveModules.TryAdd([module].SystemId, [module])
                End If
            Next

            Await ApplyModuleConfigurationAsync(True, True, Nothing, recalcBundleDiameters, updateInformationHubFilters, updateCanvasFilters)
        End Using
    End Function

    Private Sub _informationHub_HighlightEntireRoutingPath(sender As InformationHub, e As HighlightEntireRoutingPathEventArgs) Handles _informationHub.HighlightEntireRoutingPath
        RaiseEvent HighlightEntireRoutingPath(sender, e)
    End Sub

    Private Sub _informationHub_HubSelectionChanged(sender As InformationHub, e As InformationHubEventArgs) Handles _informationHub.HubSelectionChanged
        OnHubSelectionChanged(sender, e)
    End Sub

    Private Sub _informationHub_LogMessage(sender As InformationHub, e As LogEventArgs) Handles _informationHub.LogMessage
        _logHub.WriteLogMessage(e)
    End Sub

    Private Sub _informationHub_MemolistChanged(sender As InformationHub, e As Dictionary(Of String, Object)) Handles _informationHub.MemolistChanged
        _dirty = True
        _memolistHub.AddObjects(e)

        Tools.Settings.DisplayMemolistHub.Checked = True
        If MainForm IsNot Nothing Then
            MainForm.Tools.Application.SaveDocument.SharedProps.Enabled = True
            MainForm.Tools.Application.SaveDocuments.SharedProps.Enabled = True
        End If
        Me.udmDocument.PaneFromKey(PaneKeys.MemolistHub.ToString).Show()
    End Sub

    Private Sub _informationHub_Message(sender As InformationHub, e As MessageEventArgs) Handles _informationHub.Message
        OnMessage(e)
    End Sub

    Private Sub _informationHub_QMStampsChanged(sender As InformationHub, e As InformationHubEventArgs) Handles _informationHub.QMStampsChanged
        SetDirty()

        If (_activeDrawingCanvas IsNot Nothing) Then
            ActiveDrawingCanvas.QMStampsChanged(e.RemovePreviousSelection)
        End If
    End Sub

    Private Sub _informationHub_RedliningChanged(sender As InformationHub, e As InformationHubEventArgs) Handles _informationHub.RedliningsChanged
        SetDirty()

        For Each canvas As DrawingCanvas In Me.GetAllDrawingCanvas
            canvas.OnRedliningsChanged(True)
        Next

        If (_D3DControl IsNot Nothing) Then
            CreateRedlinings()
        End If

        _cavitiesDocumentView.UpdateRedliningIconsForCavityNavigator()
    End Sub

    Private Sub _informationHub_ShowEntireRoutingPath(sender As InformationHub, e As InformationHubEventArgs) Handles _informationHub.ShowEntireRoutingPath
        If (Me.utcDocument.ActiveTab IsNot Nothing) AndAlso (ActiveDrawingCanvas IsNot Nothing) Then
            If (Me.utcDocument.ActiveTab.Key = DraftContainerFile.FILE_NAME) Then
                MessageBox.Show(DocumentFormStrings.EntRoutPathNA_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                Using entireRoutingPathForm As New EntireRoutingPathForm(TryCast(MainForm, MainForm)?.EntireRoutingPath, MainForm.GeneralSettings.InlinerPairCheckClassifications)
                    Dim activeDrawingCanvas As DrawingCanvas = Nothing

                    AddHandler entireRoutingPathForm.EntireRoutingPathViewMouseClick, AddressOf OnEntireRoutingPathOrOverallConnectivityViewMouseClick

                    If MainForm IsNot Nothing Then
                        For Each routingPath As KeyValuePair(Of String, RoutingPath) In MainForm.EntireRoutingPath
                            For Each tabGroup As MdiTabGroup In MainForm.utmmMain.TabGroups
                                If tabGroup.Tabs.Exists(routingPath.Key) Then
                                    activeDrawingCanvas = DirectCast(tabGroup.Tabs(routingPath.Key).Form, DocumentForm).ActiveDrawingCanvas

                                    If activeDrawingCanvas IsNot Nothing AndAlso DirectCast(tabGroup.Tabs(routingPath.Key).Form, DocumentForm).utcDocument.ActiveTab.Key = DraftContainerFile.FILE_NAME Then
                                        activeDrawingCanvas = Nothing
                                    End If

                                    entireRoutingPathForm.AddConnectorGroups(activeDrawingCanvas, DirectCast(tabGroup.Tabs(routingPath.Key).Form, DocumentForm).KBL, routingPath.Value)
                                End If
                            Next
                        Next
                    End If

                    entireRoutingPathForm.ShowDialog(Me)

                    RemoveHandler entireRoutingPathForm.EntireRoutingPathViewMouseClick, AddressOf OnEntireRoutingPathOrOverallConnectivityViewMouseClick
                End Using
            End If
        Else
            MessageBoxEx.ShowWarning(DocumentFormStrings.NoDrwLoaded_Msg)
        End If
    End Sub

    Private Sub _informationHub_ShowOverallConnectivity(sender As InformationHub, e As InformationHubEventArgs) Handles _informationHub.ShowOverallConnectivity
        Dim startingPoint As Tuple(Of String, String) = Nothing

        For Each coreWireId As String In e.ObjectIds
            Dim connection As Connection = _kblMapper.GetConnections.Where(Function(con) con.Wire = coreWireId).FirstOrDefault
            If (connection IsNot Nothing) Then
                Dim startingCoreWireId As String = HarnessConnectivity.GetUniqueId(_kblMapper.HarnessPartNumber, coreWireId)
                Dim startCavityId As String = HarnessConnectivity.GetUniqueId(_kblMapper.HarnessPartNumber, DirectCast(_kblMapper.KBLOccurrenceMapper(connection.GetStartContactPointId), Contact_point).Contacted_cavity)
                Dim endCavityId As String = HarnessConnectivity.GetUniqueId(_kblMapper.HarnessPartNumber, DirectCast(_kblMapper.KBLOccurrenceMapper(connection.GetEndContactPointId), Contact_point).Contacted_cavity)
                If (_harnessConnectivity.HasCavityVertex(startCavityId)) Then
                    Dim startCavityVertex As CavityVertex = _harnessConnectivity.GetCavityVertex(startCavityId)
                    If (startCavityVertex.IsInliner) Then
                        startCavityId = endCavityId 'HINT the layout gets better if we do not start with an inliner, it is not clear why the start cavity is determined here anyway...
                    End If
                End If
                startingPoint = New Tuple(Of String, String)(startCavityId, startingCoreWireId)
                Exit For
            End If
        Next

        If (startingPoint IsNot Nothing) Then
            Using overallConnectivityForm As New OverallConnectivityForm(startingPoint, _settingsForm?.GeneralSettings.DefaultWireLengthType, GetInactiveWiresAndCores, MainForm.MainStateMachine.OverallConnectivity, _harnessConnectivity, Me.MainForm.GetAllDocuments.Values.Select(Function(doc) doc.KBL))
                AddHandler overallConnectivityForm.OverallConnectivityViewMouseClick, AddressOf OnEntireRoutingPathOrOverallConnectivityViewMouseClick
                overallConnectivityForm.ShowDialog(Me)
                RemoveHandler overallConnectivityForm.OverallConnectivityViewMouseClick, AddressOf OnEntireRoutingPathOrOverallConnectivityViewMouseClick
            End Using
        Else
            MessageBoxEx.ShowError(DocumentFormStrings.OverallConnViewNA_Msg)
        End If
    End Sub

    Private Function GetInactiveWiresAndCores() As Dictionary(Of String, List(Of String))
        Dim harnessesWithInactiveCoresWires As New Dictionary(Of String, List(Of String))
        If MainForm IsNot Nothing Then
            For Each tabGroup As MdiTabGroup In MainForm.utmmMain.TabGroups
                For Each tab As MdiTab In tabGroup.Tabs
                    Dim documentForm As DocumentForm = DirectCast(tab.Form, DocumentForm)
                    Dim harness_part_number As String = documentForm.KBL.HarnessPartNumber

                    harnessesWithInactiveCoresWires.AddOrUpdate(harness_part_number, documentForm.InactiveObjects.GetValueOrEmpty(KblObjectType.Core_occurrence))
                    harnessesWithInactiveCoresWires.AddOrUpdate(harness_part_number, documentForm.InactiveObjects.GetValueOrEmpty(KblObjectType.Wire_occurrence))
                Next
            Next
        End If
        Return harnessesWithInactiveCoresWires
    End Function

    Private Sub _informationHub_ShowStartEndConnectors(sender As InformationHub, e As InformationHubEventArgs) Handles _informationHub.ShowStartEndConnectors
        If (Me.utcDocument.ActiveTab IsNot Nothing) AndAlso (ActiveDrawingCanvas IsNot Nothing) Then
            If Me.utcDocument.ActiveTab.Key = DraftContainerFile.FILE_NAME Then
                MessageBox.Show(DocumentFormStrings.StartEndConnViewNA_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                Using startEndConnectorsForm As New StartEndConnectorsForm
                    If (TypeOf _kblMapper.KBLOccurrenceMapper(e.ObjectIds(0)) Is Core_occurrence) Then
                        startEndConnectorsForm.WireIdToHighLight = DirectCast(_kblMapper.KBLOccurrenceMapper(e.ObjectIds(0)), Core_occurrence).SystemId
                        startEndConnectorsForm.WireShortName = DirectCast(_kblMapper.KBLOccurrenceMapper(e.ObjectIds(0)), Core_occurrence).Wire_number
                    Else
                        startEndConnectorsForm.WireIdToHighLight = DirectCast(_kblMapper.KBLOccurrenceMapper(e.ObjectIds(0)), Wire_occurrence).SystemId
                        startEndConnectorsForm.WireShortName = DirectCast(_kblMapper.KBLOccurrenceMapper(e.ObjectIds(0)), Wire_occurrence).Wire_number
                    End If

                    e.ObjectIds.Remove(e.ObjectIds.ToList.First)

                    For Each connectorId As String In e.ObjectIds
                        Dim connectorTableAdded As Boolean = False

                        If (ActiveDrawingCanvas.GroupMapper.ContainsKey(connectorId)) Then
                            For Each group As VdSVGGroup In ActiveDrawingCanvas.GroupMapper(connectorId).Values.ToList
                                If (group.SVGType = SvgType.dimension.ToString) OrElse (group.SVGType = SvgType.ref.ToString) Then
                                    Continue For
                                End If

                                If (group.SVGType = SvgType.table.ToString) Then
                                    If (connectorTableAdded) OrElse (Not ContainsConnectorTableGroupHighlightingWireRow_Recursively(group, startEndConnectorsForm.WireIdToHighLight)) Then
                                        Continue For
                                    Else
                                        connectorTableAdded = True
                                    End If
                                End If

                                startEndConnectorsForm.AddFigure(group, DirectCast(_kblMapper.KBLOccurrenceMapper(connectorId), Connector_occurrence).Id)
                            Next
                        End If
                    Next

                    startEndConnectorsForm.ShowDialog(Me)
                End Using
            End If
        Else
            MessageBoxEx.ShowWarning(DocumentFormStrings.NoDrwLoaded_Msg)
        End If
    End Sub

    Private Sub _memolistHub_HubDoubleClicked(sender As MemolistHub, id As String) Handles _memolistHub.HubDoubleClicked
        Dim kblIds As New HashSet(Of String)({id})

        SelectRowsInGrids(kblIds, True, True)

        If (_informationHub.utcInformationHub.ActiveTab IsNot Nothing) AndAlso (_informationHub.utcInformationHub.ActiveTab.Key = TabNames.tabModules.ToString) Then
            'HINT MR unclear what this is for, seems to be something with the cavity checker...This corrupts the selection of modules from the memolist deselect in grid
            OnHubSelectionChanged(sender, New InformationHubEventArgs(_kblMapper.Id.ToString) With {.ObjectIds = kblIds, .ObjectType = KblObjectType.Harness_module})
        End If
    End Sub

    Private Sub _memolistHub_MemolistChanged(sender As MemolistHub, e As String) Handles _memolistHub.MemolistChanged
        _dirty = True

        If MainForm IsNot Nothing Then
            MainForm.utmMain.Tools(ApplicationMenuToolKey.SaveDocument.ToString).SharedProps.Enabled = True
            MainForm.utmMain.Tools(ApplicationMenuToolKey.SaveDocuments.ToString).SharedProps.Enabled = True
        End If
    End Sub

    Private Sub _modulesHub_ActiveModulesChanged(sender As ModulesHub, e As ModulesHubEventArgs) Handles _modulesHub.ActiveModulesChanged
        ApplyModuleConfiguration(e.IsDirty, False, e.HarnessConfig)
        If _cavitiesDocumentView IsNot Nothing Then
            _cavitiesDocumentView.NotifyActiveModulesChanged(e.HarnessConfig.SystemId)
        End If
        _documentStateMachine.ResetIndicators()
    End Sub

    Private Sub _modulesHub_ModuleSelectionChanged(sender As ModulesHub, e As ModulesHubEventArgs) Handles _modulesHub.ModuleSelectionChanged
        With _informationHub.ugModules
            .BeginUpdate()
            Using .EventManager.ProtectProperty(NameOf(GridEventManager.AllEventsEnabled), False)
                .ActiveRow = Nothing
                .Selected.Rows.Clear()
            End Using
            .EndUpdate()
        End With

        SelectRowsInGrids(e.ModuleIds, False, True)
        GetAllDrawingCanvas.Where(Function(c) c.Visible).ForEach(Sub(canvas) canvas.InformationHubSelectionChanged(e.ModuleIds, KblObjectType.Harness_module))
    End Sub

    Private Sub udmDocument_PaneDeactivate(sender As Object, e As ControlPaneEventArgs) Handles udmDocument.PaneDeactivate
        If (e.Pane.Key = "InformationHub") Then
            _informationHub?.CloseContextMenus()
        End If
    End Sub

    Private Sub EnableDrawingEntitiesProgress()
        With _activeDrawingCanvas
            .EnableOverrideProgressDrawingEntities()
            .EnableDrawEvents()
        End With
    End Sub

    Private Sub RestoreDefaultDrawingEntitiesProgress()
        With _activeDrawingCanvas
            .DrawingCanvasStatusMessage = String.Empty
            .DisableDrawEvents()
            .ResumeDefaultProgressDrawingEntities()
        End With
    End Sub

    Private Sub UpdateNavigatorImage(Optional loadView As Boolean = True)
        With utcDocument.ActiveTab
            If .Key = TAB_DOC3D_KEY Then
                If Panes.NavigatorHub Is Nothing Then
                    AddOrUpdateNavigatorHubPane()
                End If

                _navigatorHub.SetEmptyImage()
                _navigatorHub.upbNavigator.Refresh()
                _navigatorHub.Update()
            Else
                If ActiveDrawingCanvas IsNot Nothing Then
                    If (Panes.NavigatorHub Is Nothing OrElse _navigatorHub.Parent Is Nothing) Then
                        AddOrUpdateNavigatorHubPane()
                    End If

                    If MainForm?.Tools.Settings.DisplayTopologyHub IsNot Nothing Then
                        If Tools.Settings.DisplayNavigatorHub.Checked Then
                            If loadView Then
                                EnableDrawingEntitiesProgress()
                                _activeDrawingCanvas.DrawingCanvasStatusMessage = String.Format(DocumentFormStrings.LoadNavigatorHub)
                                _navigatorHub.LoadView()
                                RestoreDefaultDrawingEntitiesProgress()
                                OnMessage(New MessageEventArgs(MessageEventArgs.MsgType.HideProgress))
                            End If
                        Else
                            Panes.NavigatorHub?.Close()
                        End If
                    End If
                End If
            End If
        End With
    End Sub

    Friend Sub DisplayDrawing(Optional zoomExtends As Boolean = True)
        If ActiveDrawingCanvas IsNot Nothing Then
            ActiveDrawingCanvas.DrawingCanvasStatusMessage = DrawingCanvasStrings.InitSVGDrw_LogMsg
            ActiveDrawingCanvas.DisplayDrawing(forceDisplay:=False, zoomExtends:=zoomExtends)
            ActiveDrawingCanvas.DrawingCanvasStatusMessage = String.Empty
        End If
    End Sub

    Private Sub utcDocument_ActiveTabChanged(sender As Object, e As ActiveTabChangedEventArgs) Handles utcDocument.ActiveTabChanged
        If (e.Tab IsNot Nothing AndAlso e.Tab.TabPage.Controls.Count <> 0) Then
            If e.Tab.Key = TAB_DOC3D_KEY Then
                Me.udmDocument.BeginUpdate()

                ActiveDrawingCanvas = Nothing
                _analysisStateMachine.DisableButton(HomeTabToolKey.AnalysisShowQMIssues)

                Me.Tools.Settings.Indicators.SharedProps.Enabled = True
                UpdateNavigatorImage(Me.MainForm.MainStateMachine.CurrentLoadingDocumentsCount <= 1) ' set loadview is not really needed because it's not used in 3D, but for orthogonalization reasons we just do the same as for the 2D

                Me.udmDocument.EndUpdate()
            Else
                Using Me.EnableWaitCursor
                    ActiveDrawingCanvas = TryCast(e.Tab.TabPage.Controls(0), DrawingCanvas)
                    Me.Tools.Settings.Indicators.SharedProps.Enabled = False
                    If ActiveDrawingCanvas IsNot Nothing Then
                        _analysisStateMachine.EnableButton(HomeTabToolKey.AnalysisShowQMIssues)

                        If MainForm?.MainStateMachine.CurrentLoadingDocumentsCount <= 1 Then
                            Me.DisplayDrawing(True)
                        End If

                        _documentStateMachine.ActiveDrawingCanvas = ActiveDrawingCanvas
                        _navigatorHub.VDCanvas = ActiveDrawingCanvas.vdCanvas.BaseControl

                        Me.udmDocument.BeginUpdate()
                        UpdateNavigatorImage((Me.MainForm?.MainStateMachine.CurrentLoadingDocumentsCount).GetValueOrDefault = 0) ' only draw the image when load is finished of active tab has changed after all documents have benn loaded
                        'HINT: navigator-Hub-loading is now moved to the after the vectorDraw control has been finished to be drawn
                    End If

                    Me.udmDocument.EndUpdate()
                    OnMessage(New MessageEventArgs(MessageEventArgs.MsgType.HideProgress))
                End Using
            End If
        End If

    End Sub

    Private Sub utcDocument_Click(sender As Object, e As EventArgs) Handles utcDocument.Click
        If (ActiveDrawingCanvas IsNot Nothing) Then
            ActiveDrawingCanvas.vdCanvas.Focus()
        End If
    End Sub

    Friend Sub OnMessage(e As MessageEventArgs)
        RaiseEvent Message(Me, e)
    End Sub

    Private Sub _documentView_ModelChanged(sender As Object, e As EventArgs) Handles _cavitiesDocumentView.ModelChanged
        SetDirty()
    End Sub

    Private Sub _documentView_NeedsInactiveModulesUpdated(sender As Object, e As EventArgs) Handles _cavitiesDocumentView.NeedsInactiveModuleSettingsApplied
        Dim doc As DocumentView = DirectCast(sender, DocumentView)
        With doc.Model.Settings
            Dim activeHarnessConfig As Harness_configuration = CType(_harnessModuleConfigurations.Select(Function(hmc) hmc.HarnessConfigurationObject).Where(Function(hc) hc.SystemId = .ActiveHarnessConfigurationId).FirstOrDefault, Harness_configuration)
            If activeHarnessConfig IsNot Nothing Then

                _kblMapper.InactiveModules.Clear()
                For Each [module] As [Lib].Schema.Kbl.[Module] In _kblMapper.GetModules
                    If Not .ActiveModules.Contains([module].SystemId) Then
                        _kblMapper.InactiveModules.Add([module].SystemId, [module])
                    End If
                Next

                ApplyModuleConfiguration(True, True, activeHarnessConfig)
            Else
                MessageBox.Show(Me, ErrorStrings.CanTResolveHarnessConfigurationNotFoundInCurrentDocument, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End With
    End Sub

    Private Sub _document3DView_SelectionChanged(sender As Object, e As EventArgs) Handles _D3DControl.SelectionChanged
        Me.OnCanvasSelectionChanged(False, _D3DControl.SelectedEntities)
    End Sub

    Private Sub OnCanvasSelectionChanged(raiseSelectionChangedEventForModules As Boolean, selection As devDept.Eyeshot.ISelectedEntitiesCollection)
        Me.OnCanvasSelectionChanged(raiseSelectionChangedEventForModules, New CanvasSelection(selection, _document))
    End Sub

    Friend Sub OnCanvasSelectionChanged(raiseSelectionChangedEventForModules As Boolean, selection As vdSelection)
        Me.OnCanvasSelectionChanged(raiseSelectionChangedEventForModules, New CanvasSelection(selection, _kblMapper))
    End Sub

    Friend Sub OnCanvasSelectionChanged(raiseSelectionChangedEventForModules As Boolean, selection As CanvasSelection)
        _informationHub.CanvasSelectionChangedSyncRowsAction(raiseSelectionChangedEventForModules, selection)
        RaiseEvent CanvasSelectionChanged(Me, New CanvasSelectionChangedEventArgs(selection))
    End Sub

    Friend Function SelectRowsInGrids(ids As ICollection(Of String), removePreviousSelection As Boolean, singleObjectType As Boolean, Optional selectIn3DView As Boolean = True, Optional highlightInConnectivityView As Boolean = True) As List(Of String)
        Return _informationHub.SelectRowsInGrids(ids, removePreviousSelection, singleObjectType, selectIn3DView, highlightInConnectivityView)
    End Function

    Private Sub _documentView_ModelSelectionChanged(sender As Object, e As EventArgs) Handles _cavitiesDocumentView.ModelSelectionChanged
        Dim doc As DocumentView = DirectCast(sender, Checks.Cavities.Views.Document.DocumentView)
        SelectRowsInGrids(GetKblIdsFromViews(doc.Model.Selected), True, True)
    End Sub

    Public Sub _documentView_InitializeRedliningDialog(sender As Object, e As EventArgs) Handles _cavitiesDocumentView.InitializeRedliningDialog
        _informationHub.AddOrEditRedliningForCavityNavigator()
    End Sub

    Public ReadOnly Property D3DCancellationTokenSource As D3D.Consolidated.Controls.D3DCancellationTokenSource
        Get
            Return _d3dAsyncLoadCancelSource
        End Get
    End Property

    <Obfuscation(Feature:="renaming")>
    Public ReadOnly Property File() As [Lib].IO.Files.Hcv.HcvFile
        Get
            Return _hcvFile
        End Get
    End Property

    'TODO: better renaming to only document but currently the document is not finished to be the central element - only finished for 3D so to be clear the naming is currently Document3D which will evolve to Document only
    Public ReadOnly Property Document As HcvDocument
        Get
            Return _document
        End Get
    End Property

    Public ReadOnly Property HarnessConnectivity As HarnessConnectivity
        Get
            Return _harnessConnectivity
        End Get
    End Property

    Public ReadOnly Property Id As String
        Get
            Return _id.ToString
        End Get
    End Property

    Public ReadOnly Property InactiveObjects As ITypeGroupedKblIds
        Get
            Return _informationHub.CurrentRowFilterInfo.InactiveObjects
        End Get
    End Property

    Public ReadOnly Property Inliners() As Dictionary(Of Connector_occurrence, Connector_occurrence)
        Get
            Return _inliners
        End Get
    End Property

    Friend Property IsDirty() As Boolean
        Get
            Return _dirty
        End Get
        Set(value As Boolean)
            _dirty = value
        End Set
    End Property

    Public ReadOnly Property IsExtendedHCV() As Boolean

    Public ReadOnly Property IsTouchEnabled() As Boolean
        Get
            Return (MainForm?.GeneralSettings?.TouchEnabledUI).GetValueOrDefault
        End Get
    End Property

    Public ReadOnly Property TemporaryJunkFolder() As IO.DirectoryInfo
        Get
            Return _temporaryJunkFolder
        End Get
    End Property

    Public ReadOnly Property MainForm As MainForm
        Get
            Return TryCast(_settingsForm, MainForm)
        End Get
    End Property

    Public Property WorkerJobDelegate() As JobDelegate
    Public Property WorkerJobFinishedDelegate() As JobFinishedDelegate
    Public Property WorkerJobProgressDelegate() As JobProgressDelegate

    Friend Function GetCompareResultInfoFileContainer(type As [Lib].IO.Files.Hcv.KnownContainerFileFlags) As [Lib].IO.Files.Hcv.CheckedCompareResultInfoContainerFile
        Return GetCompareResultInfo(type).AsFileContainer()
    End Function

    Friend Function GetCompareResultInfo(type As [Lib].IO.Files.Hcv.KnownContainerFileFlags) As CheckedCompareResultInformation
        Select Case type
            Case [Lib].IO.Files.Hcv.KnownContainerFileFlags.GCRI
                Return _compareResultGraphicalInfo
            Case [Lib].IO.Files.Hcv.KnownContainerFileFlags.TCRI
                Return _compareResultTechnicalInfo
            Case KnownContainerFileFlags.Unspecified
                Throw New ArgumentException(String.Format("<Unspecified> type is not supported/valid! Expected values are ""{0}""", String.Join(", ", KnownContainerFileFlags.GCRI.ToString, KnownContainerFileFlags.TCRI.ToString)))
            Case Else
                Throw New ArgumentException(String.Format("Invalid type: {0}", type))
        End Select
    End Function

    Public Sub SetCompareResultInfo(value As CheckedCompareResultInformation)
        Select Case value.Type
            Case KnownContainerFileFlags.GCRI
                _compareResultGraphicalInfo = value
            Case KnownContainerFileFlags.TCRI
                _compareResultTechnicalInfo = value
            Case Else
                Throw New ArgumentException(String.Format("Invalid type: {0}", value.Type.ToString))
        End Select
    End Sub

    'TODO: it would be better to move this method into Project.HcvDocument but the current structure (lots of changes that can't be done currently) does not allow this, so we are using DocumentForm (It's a GUI-element, but the best result that can't be done at this state) to consolidate 2D/3D business logic. In the future this should be migrated for DrawingCanvas (2D) and Document3DViewer (3D) into single non-GUI-document-objects
    Friend Function FilterAnalysisViewObjects(activeObjects As ICollection(Of String)) As String()
        SelectIn3DView()

        Dim filteredKblIds As String() = GetAllDrawingCanvas().SelectMany(Function(canvas) canvas.FilterAnalysisViewObjects(activeObjects)).Distinct.ToArray
        If (_informationHub IsNot Nothing) Then
            filteredKblIds = filteredKblIds.Concat(_informationHub.FilterAnalysisViewObjects(activeObjects)).ToArray
        End If

        If _D3DControl IsNot Nothing Then
            _D3DControl.SetActiveOBjectsByKblIds(filteredKblIds)
        End If

        Return filteredKblIds.Distinct.ToArray
    End Function

    Friend Function GetAllDrawingCanvas() As DrawingCanvas()
        Dim list As New List(Of DrawingCanvas)
        For Each tab As UltraTab In utcDocument.Tabs
            If tab.TabPage.Controls.Count > 0 AndAlso TypeOf tab.TabPage.Controls(0) Is DrawingCanvas Then
                list.Add(CType(tab.TabPage.Controls(0), DrawingCanvas))
            End If
        Next
        Return list.ToArray
    End Function

    'TODO: it would be better to move this property into Project.HcvDocument but the current structure (lots of changes that can't be done currently) does not allow this, so we are using the current structure (GUI-element,DocumentForm) to migrate and read this properties, but the Model-object (Document) should to this in the future for DrawingCanvas (2D) and Document3DViewer (3D) - migration all 2D/3D properties into single property at Document is needed to be done!
    Public ReadOnly Property IsBusy As Boolean
        Get
            Return (_busyState?.HasAnyState).GetValueOrDefault
        End Get
    End Property

    'TODO: it would be better to move this property into Project.HcvDocumentt but the current structure (lots of changes that can't be done currently) does not allow this, so we are using the current structure (GUI-element,DocumentForm) to migrate and read this properties, but the Model-object (Document) should to this in the future for DrawingCanvas (2D) and Document3DViewer (3D) - migration all 2D/3D properties into single property at Document is needed to be done!
    Public ReadOnly Property Is2DOr3DCanvasActive As Boolean
        Get
            For Each childTab As UltraTab In utcDocument.Tabs
                If (childTab.Visible) AndAlso (TypeOf childTab.TabPage.Controls(0) Is DrawingCanvas OrElse TypeOf childTab.TabPage.Controls(0) Is D3D.Document.Controls.Document3DControl) Then
                    Return True
                End If
            Next
            Return False
        End Get
    End Property

    Public ReadOnly Property HideEntitiesWithNoModules As Boolean
        Get
            Return _hideEntitiesWithNoModules
        End Get
    End Property

    ''' <summary>
    ''' Show proposals for given splice (blink-highlight). Method is auto blocking multiple calls. Only once per calculation allowed
    ''' </summary>
    ''' <param name="id">kbl id of the splice</param>
    ''' <param name="maximum"></param>
    ''' <returns>returns false if document 3d was not opened, can only be used after document 3d was opened</returns>
    Friend Async Function TryShowSpliceProposalIn3D(id As String, Optional maximum As Integer = 4) As Task(Of Boolean)
        Await _spliceProposalLock.WaitAsync()
        Try
            If (_document?.IsOpen).GetValueOrDefault Then
                _D3DControl.ClearSpliceHighlightAndRubberLines()

                Dim model As E3.Lib.Model.EESystemModel = Me.Document.Model
                If model IsNot Nothing Then
                    Dim activeModuleIds As String() = GetKblActiveModuleIds()

                    For Each conn As E3.Lib.Model.Connector In model.Connectors
                        SetModelObjVariant(conn, model.Variants.MasterVariant(), activeModuleIds)
                    Next

                    For Each wir As E3.Lib.Model.Wire In model.Wires
                        SetModelObjVariant(wir, model.Variants.MasterVariant(), activeModuleIds)
                    Next

                    Dim splice As E3.Lib.Model.Connector = TryCast(_document.Entities.GetByKblIds(id).SelectMany(Function(ent) ent.GetEEModelObjects).FirstOrDefault, E3.Lib.Model.Connector)
                    Dim result As AsyncVertexProposalsResult = Await _document.CalculateVertexProposalsAsync(splice)

                    Select Case result.ResultType
                        Case ProposalsResultType.Success
                            Dim proposals As VertexProposal() = result.Vertices.Where(Function(v) v.IsStartSplice).Concat(result.Vertices.Where(Function(v) Not v.IsStartSplice).Take(maximum)).ToArray
                            Dim backColor As Drawing.Color = _D3DControl.Model3DControl1.Design.ActiveViewport.Background.GetContrastColorInverted
                            Dim rankMin As Integer = proposals.Select(Function(v) v.Rank).Min
                            Dim rankMax As Integer = proposals.Select(Function(v) v.Rank).Max

                            For Each proposal As VertexProposal In proposals
                                _D3DControl.AddProposalLabel(proposal, rankMin, rankMax)
                            Next

                            _D3DControl.HighlightSpliceEntities(proposals) ' HINT: first id will be excluded and shown in different highlight

                        Case ProposalsResultType.NotExecuted
                            'HINT do nothing here
                        Case ProposalsResultType.NothingFound
                            MessageBox.Show(Me, InformationHubStrings.SpliceLocator_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End Select

                    _messageEventArgs = New MessageEventArgs(MessageEventArgs.MsgType.ShowProgressAndMessage, 0, InformationHubStrings.ShowBestProposal)
                    OnMessage(_messageEventArgs)
                    Return True
                End If
            End If
        Catch ex As Exception
            _D3DControl.ClearSpliceHighlight()
            MessageBoxEx.ShowError(ex.Message)
        Finally
            _spliceProposalLock.Release()
        End Try
        Return False
    End Function

    Friend Sub ShowHints()
        Dim instruction As String = ""
        If (_document?.IsOpen).GetValueOrDefault Then
            Dim model As E3.Lib.Model.EESystemModel = Me.Document.Model
            If model IsNot Nothing Then

                For Each item As IBaseModelEntityEx In Me.Document.Entities
                    Dim obj As ObjectBaseNaming = CType(model(New Guid(item.Id)), ObjectBaseNaming)
                    If obj IsNot Nothing Then
                        Dim bag As KblPropertyBagBase = obj.CustomAttributes.OfType(Of KblPropertyBagAttribute).SingleOrDefault?.PropertyBag
                        If bag IsNot Nothing Then
                            Select Case obj.HostContainerId
                                Case ContainerId.AdditionalParts
                                    instruction = CType(bag, KblFixingPropertyBag).Instruction

                                Case ContainerId.Supplements
                                    instruction = CType(bag, KblAccessoryPropertyBag).Instruction

                                Case ContainerId.Connectors
                                    instruction = CType(bag, KblConnectorPropertyBag).Instruction

                                Case ContainerId.Components
                                    instruction = CType(bag, KblComponentPropertyBag).Instruction

                                Case ContainerId.Segments
                                    instruction = CType(bag, KblSegmentPropertyBag).Instruction

                                Case ContainerId.Vertices
                                    instruction = CType(bag, KblNodePropertyBag).Instruction

                                Case ContainerId.Protections
                                    instruction = CType(bag, KblProtectionPropertyBag).Instruction

                            End Select

                            If Not String.IsNullOrEmpty(instruction) Then
                                Dim entity As BaseModelEntity = devDept.Eyeshot.Entities.Flatten(_document.Entities.GetByEEObjectId(New Guid(item.Id)).Cast(Of IEntity)).Cast(Of BaseModelEntity).FirstOrDefault
                                If entity IsNot Nothing Then
                                    Dim txt As String = instruction
                                    _D3DControl.AddHintLabel(entity, txt)
                                End If
                            End If

                        End If
                    End If
                Next
                _D3DControl.Model3DControl1.Design.Invalidate()
            End If
        End If
    End Sub

    Friend Sub ShowClockSymbols()
        Dim fixingDirection As String = String.Empty
        Dim mountingDirection As String = String.Empty
        Dim segmentDirection As String = String.Empty
        If (_document?.IsOpen).GetValueOrDefault Then
            Dim model As E3.Lib.Model.EESystemModel = Me.Document.Model
            If model IsNot Nothing Then
                For Each item As IBaseModelEntityEx In Me.Document.Entities
                    Dim obj As ObjectBaseNaming = CType(model(New Guid(item.Id)), ObjectBaseNaming)
                    If obj IsNot Nothing Then
                        Dim bag As KblPropertyBagBase = obj.CustomAttributes.OfType(Of KblPropertyBagAttribute).SingleOrDefault?.PropertyBag
                        If bag IsNot Nothing Then
                            Select Case obj.HostContainerId
                                Case ContainerId.AdditionalParts
                                    fixingDirection = CType(bag, KblFixingPropertyBag).FixingDirection
                                    mountingDirection = CType(bag, KblFixingPropertyBag).MountingDirection
                                    If Not String.IsNullOrEmpty(fixingDirection) OrElse Not String.IsNullOrEmpty(mountingDirection) Then
                                        Dim entity As BaseModelEntity = devDept.Eyeshot.Entities.Flatten(_document.Entities.GetByEEObjectId(New Guid(item.Id)).Cast(Of IEntity)).Cast(Of BaseModelEntity).FirstOrDefault
                                        If entity IsNot Nothing Then
                                            Dim txt As String = String.Format("Δ:{0}{1}□:{2}", fixingDirection, vbCrLf, mountingDirection)
                                            _D3DControl.AddClockSymbolLabel(entity, txt)
                                        End If
                                    End If
                                Case ContainerId.Segments
                                    segmentDirection = CType(bag, KblSegmentPropertyBag).SegmentDirection
                                    If Not String.IsNullOrEmpty(segmentDirection) Then
                                        Dim entity As BaseModelEntity = devDept.Eyeshot.Entities.Flatten(_document.Entities.GetByEEObjectId(New Guid(item.Id)).Cast(Of IEntity)).Cast(Of BaseModelEntity).FirstOrDefault
                                        If entity IsNot Nothing Then
                                            Dim txt As String = String.Format("Δ:{0}", segmentDirection)
                                            _D3DControl.AddClockSymbolLabel(entity, txt)
                                        End If
                                    End If
                            End Select
                        End If
                    End If
                Next
                _D3DControl.Model3DControl1.Design.Invalidate()
            End If
        End If
    End Sub

    Friend Sub ClearHints()
        _D3DControl.ClearHintLabels()
        _D3DControl.Model3DControl1.Design.Invalidate()
    End Sub

    Friend Sub ClearClockSymbols()
        _D3DControl.ClearClockSymbolsLabels()
        _D3DControl.Model3DControl1.Design.Invalidate()
    End Sub

    Friend Sub ShowVertexViewDirections()
        If (_document?.IsOpen).GetValueOrDefault Then
            Dim model As E3.Lib.Model.EESystemModel = Me.Document.Model
            If model IsNot Nothing Then
                Dim startNode As NodeEntity = Nothing
                Dim endNode As NodeEntity = Nothing
                For Each item As IBaseModelEntityEx In Me.Document.Entities
                    Dim obj As ObjectBaseNaming = CType(model(New Guid(item.Id)), ObjectBaseNaming)
                    If obj IsNot Nothing Then
                        Dim bag As KblPropertyBagBase = obj.CustomAttributes.OfType(Of KblPropertyBagAttribute).SingleOrDefault?.PropertyBag
                        If bag IsNot Nothing Then
                            Select Case obj.HostContainerId
                                Case ContainerId.Vertices
                                    Dim ndBag As KblNodePropertyBag = CType(bag, KblNodePropertyBag)
                                    If ndBag.IsViewingStartPoint Then
                                        startNode = CType(item, NodeEntity)
                                    ElseIf (ndBag.IsViewingEndPoint) Then
                                        endNode = CType(item, NodeEntity)
                                    End If
                                    If startNode IsNot Nothing AndAlso endNode IsNot Nothing Then
                                        Exit For
                                    End If
                            End Select
                        End If
                    End If
                Next
                _D3DControl.AddViewVertexDirections(startNode, endNode)
                _D3DControl.Model3DControl1.Design.Invalidate()
            End If
        End If
    End Sub

    Private Function ShowOrigins() As Dictionary(Of Guid, String)
        Dim finalOrigins As New Dictionary(Of Guid, String)
        If (_document?.IsOpen).GetValueOrDefault Then
            If Me.Document.Model IsNot Nothing Then

                _D3DControl.ClearDimensionLabels()

                Dim tempOrigins As New Dictionary(Of Guid, String)
                If _dimMode.HasFlag(DimMode.ReferenceDims) Then
                    For Each dms As [Lib].Model.Dimension In Me.Document.Model.Dimensions
                        If dms.ReferenceObjectId <> Guid.Empty Then
                            Dim refObj As ObjectBase = Me.Document.Model.Item(dms.ReferenceObjectId)
                            If refObj IsNot Nothing Then
                                Dim refpointName As String = String.Empty
                                Dim vt As [Lib].Model.Vertex = TryCast(refObj, [Lib].Model.Vertex)
                                If vt IsNot Nothing Then
                                    Dim bag As KblPropertyBagBase = vt.CustomAttributes.OfType(Of KblPropertyBagAttribute).SingleOrDefault?.PropertyBag
                                    If bag IsNot Nothing Then
                                        Dim ndBag As KblNodePropertyBag = CType(bag, KblNodePropertyBag)
                                        refpointName = ndBag.ReferencePointName
                                    End If
                                End If
                                tempOrigins.TryAdd(dms.ReferenceObjectId, refpointName)
                            End If
                        End If
                    Next
                End If

                If _dimMode.HasFlag(DimMode.ImplicitDims) Then
                    For Each item As IBaseModelEntityEx In Me.Document.Entities
                        Dim txt As String = String.Empty
                        Dim modelObject As ObjectBaseNaming = CType(Me.Document.Model(New Guid(item.Id)), ObjectBaseNaming)

                        If modelObject IsNot Nothing Then
                            Dim bagBase As KblPropertyBagBase = modelObject.CustomAttributes.OfType(Of KblPropertyBagAttribute).SingleOrDefault?.PropertyBag
                            If bagBase IsNot Nothing Then

                                Select Case modelObject.HostContainerId

                                    Case ContainerId.AdditionalParts
                                        If TypeOf bagBase Is KblFixingPropertyBag Then
                                            Dim bag As KblFixingPropertyBag = CType(bagBase, KblFixingPropertyBag)
                                            If bag.AbsoluteLocationValue > 0 Then
                                                Dim refnode As IBaseModelEntityEx = Me.Document.Entities.GetByKblIds(New String() {bag.StartNodeId}).FirstOrDefault()
                                                If refnode IsNot Nothing Then
                                                    Dim sysId As Guid = New Guid(refnode.Id)
                                                    tempOrigins.TryAdd(sysId, String.Empty)
                                                End If
                                            End If
                                        End If
                                    Case ContainerId.Protections
                                        Dim bag As KblProtectionPropertyBag = CType(bagBase, KblProtectionPropertyBag)
                                        Dim prot As Protection = CType(modelObject, Protection)

                                        Dim refnode As IBaseModelEntityEx = Me.Document.Entities.GetByKblIds(New String() {bag.StartNodeId}).FirstOrDefault()
                                        If refnode IsNot Nothing Then

                                            Dim sysId As Guid = New Guid(refnode.Id)
                                            If (prot.StartingPercentage > 0.0 AndAlso prot.StartingPercentage < 1.0) Then
                                                tempOrigins.TryAdd(sysId, String.Empty)
                                            End If
                                        End If

                                        refnode = Me.Document.Entities.GetByKblIds(New String() {bag.EndNodeId}).FirstOrDefault()
                                        If refnode IsNot Nothing Then
                                            Dim sysId As Guid = New Guid(refnode.Id)
                                            If (prot.EndingPercentage > 0.0 AndAlso prot.EndingPercentage < 1.0) Then
                                                tempOrigins.TryAdd(sysId, String.Empty)
                                            End If
                                        End If

                                    Case ContainerId.Supplements
                                        If TypeOf bagBase Is KblAccessoryPropertyBag Then
                                            Dim bag As KblAccessoryPropertyBag = CType(bagBase, KblAccessoryPropertyBag)
                                            Dim refnode As IBaseModelEntityEx = Me.Document.Entities.GetByKblIds(New String() {bag.StartNodeId}).FirstOrDefault()
                                            If refnode IsNot Nothing Then
                                                Dim sysId As Guid = New Guid(refnode.Id)
                                                tempOrigins.TryAdd(sysId, String.Empty)
                                            End If
                                        End If
                                End Select
                            End If
                        End If
                    Next
                End If


                Dim cntr As Integer = 0
                Dim usedIndices As New List(Of Integer)
                For Each origin As KeyValuePair(Of Guid, String) In tempOrigins
                    If Not String.IsNullOrEmpty(origin.Value) Then
                        Dim suffix As String = Regex.Replace(origin.Value, Document3DStrings.Reference, "", RegexOptions.IgnoreCase).Trim()
                        Dim res As Integer = 0
                        If Integer.TryParse(suffix, res) Then
                            usedIndices.Add(res)
                        End If
                    End If
                Next
                For Each origin As KeyValuePair(Of Guid, String) In tempOrigins
                    Dim refText As String
                    If String.IsNullOrEmpty(origin.Value) Then
                        cntr += 1
                        Do While (usedIndices.Contains(cntr))
                            usedIndices.Remove(cntr)
                            cntr += 1
                        Loop

                        refText = String.Format("{0} {1}", Document3DStrings.Reference, cntr.ToString)
                    Else
                        refText = origin.Value
                    End If
                    finalOrigins.Add(origin.Key, refText)
                Next

                For Each origin As KeyValuePair(Of Guid, String) In finalOrigins
                    Dim obj As ObjectBase = Me.Document.Model.Item(origin.Key)
                    If obj IsNot Nothing Then
                        If TypeOf obj Is AdditionalPartAssignment Then
                            Dim assignment As AdditionalPartAssignment = CType(obj, AdditionalPartAssignment)
                            Dim seg As [Lib].Model.Segment = assignment.GetSegment
                            Dim adtp As AdditionalPart = assignment.GetAdditionalPart
                            If seg IsNot Nothing AndAlso adtp IsNot Nothing Then
                                _D3DControl.AddDimensionReferenceLabel(adtp.Id, origin.Value, assignment.StartingPercentage, seg.Id)
                            End If
                        Else
                            _D3DControl.AddDimensionReferenceLabel(origin.Key, origin.Value)
                        End If
                    End If
                Next
            End If
        End If
        Return finalOrigins
    End Function

    Private Sub ShowTargets(origins As Dictionary(Of Guid, String))
        If (_document?.IsOpen).GetValueOrDefault Then
            If Me.Document.Model IsNot Nothing Then
                If _dimMode.HasFlag(DimMode.ReferenceDims) Then
                    For Each dms As [Lib].Model.Dimension In Me.Document.Model.Dimensions
                        Dim target As ObjectBase = dms.GetTargetObject
                        If target IsNot Nothing Then
                            If TypeOf target Is AdditionalPartAssignment Then
                                Dim assignment As AdditionalPartAssignment = CType(target, AdditionalPartAssignment)
                                If assignment IsNot Nothing Then
                                    Dim seg As [Lib].Model.Segment = assignment.GetSegment
                                    Dim part As AdditionalPart = assignment.GetAdditionalPart
                                    If seg IsNot Nothing AndAlso part IsNot Nothing Then
                                        _D3DControl.AddDimensionTargetLabel(part.Id, dms.ReferenceObjectId, origins(dms.ReferenceObjectId), dms, assignment.StartingPercentage, seg.Id)
                                    End If
                                End If
                            Else
                                _D3DControl.AddDimensionTargetLabel(target.Id, dms.ReferenceObjectId, origins(dms.ReferenceObjectId), dms)
                            End If
                        End If
                    Next

                End If
                If _dimMode.HasFlag(DimMode.ImplicitDims) Then
                    Dim targets As New Dictionary(Of Guid, List(Of ObjectBaseNaming))
                    For Each item As IBaseModelEntityEx In Me.Document.Entities
                        Dim txt As String = String.Empty
                        Dim modelObject As ObjectBaseNaming = CType(Me.Document.Model(New Guid(item.Id)), ObjectBaseNaming)

                        If modelObject IsNot Nothing Then
                            Dim bagBase As KblPropertyBagBase = modelObject.CustomAttributes.OfType(Of KblPropertyBagAttribute).SingleOrDefault?.PropertyBag
                            If bagBase IsNot Nothing Then

                                Select Case modelObject.HostContainerId

                                    Case ContainerId.AdditionalParts
                                        If TypeOf bagBase Is KblFixingPropertyBag Then
                                            Dim bag As KblFixingPropertyBag = CType(bagBase, KblFixingPropertyBag)
                                            If bag.AbsoluteLocationValue > 0 Then
                                                Dim refnode As IBaseModelEntityEx = Me.Document.Entities.GetByKblIds(New String() {bag.StartNodeId}).FirstOrDefault()
                                                If refnode IsNot Nothing Then
                                                    Dim sysId As Guid = New Guid(refnode.Id)

                                                    If targets.ContainsKey(sysId) Then
                                                        targets(sysId).Add(modelObject)
                                                    Else
                                                        targets.Add(sysId, New List(Of ObjectBaseNaming)({modelObject}))
                                                    End If
                                                End If
                                            End If
                                        End If
                                    Case ContainerId.Protections
                                        Dim bag As KblProtectionPropertyBag = CType(bagBase, KblProtectionPropertyBag)
                                        Dim prot As Protection = CType(modelObject, Protection)
                                        Dim refnode As IBaseModelEntityEx = Me.Document.Entities.GetByKblIds(New String() {bag.StartNodeId}).FirstOrDefault()

                                        If refnode IsNot Nothing Then
                                            Dim sysId As Guid = New Guid(refnode.Id)
                                            If (prot.StartingPercentage > 0.0 AndAlso prot.StartingPercentage < 1.0) Then
                                                If targets.ContainsKey(sysId) Then
                                                    If Not targets(sysId).Contains(modelObject) Then
                                                        targets(sysId).Add(modelObject)
                                                    End If
                                                Else
                                                    targets.Add(sysId, New List(Of ObjectBaseNaming)({modelObject}))
                                                End If
                                            End If
                                        End If


                                        refnode = Me.Document.Entities.GetByKblIds(New String() {bag.EndNodeId}).FirstOrDefault()
                                        If refnode IsNot Nothing Then
                                            Dim sysId As Guid = New Guid(refnode.Id)
                                            If (prot.EndingPercentage > 0.0 AndAlso prot.EndingPercentage < 1.0) Then

                                                If targets.ContainsKey(sysId) Then
                                                    If Not targets(sysId).Contains(modelObject) Then
                                                        targets(sysId).Add(modelObject)
                                                    End If
                                                Else
                                                    targets.Add(sysId, New List(Of ObjectBaseNaming)({modelObject}))
                                                End If
                                            End If
                                        End If

                                    Case ContainerId.Supplements
                                        If TypeOf bagBase Is KblAccessoryPropertyBag Then
                                            Dim bag As KblAccessoryPropertyBag = CType(bagBase, KblAccessoryPropertyBag)
                                            Dim refnode As IBaseModelEntityEx = Me.Document.Entities.GetByKblIds(New String() {bag.StartNodeId}).FirstOrDefault()
                                            If refnode IsNot Nothing Then
                                                Dim sysId As Guid = New Guid(refnode.Id)

                                                If targets.ContainsKey(sysId) Then
                                                    targets(sysId).Add(modelObject)
                                                Else
                                                    targets.Add(sysId, New List(Of ObjectBaseNaming)({modelObject}))
                                                End If
                                            End If
                                        End If

                                End Select

                            End If
                        End If
                    Next
                    For Each orig As KeyValuePair(Of Guid, String) In origins
                        If targets.ContainsKey(orig.Key) Then
                            For Each modelObject As ObjectBaseNaming In targets(orig.Key)
                                Dim bagBase As KblPropertyBagBase = modelObject.CustomAttributes.OfType(Of KblPropertyBagAttribute).SingleOrDefault?.PropertyBag
                                If bagBase IsNot Nothing Then
                                    Select Case modelObject.HostContainerId

                                        Case ContainerId.AdditionalParts
                                            If TypeOf bagBase Is KblFixingPropertyBag Then
                                                Dim bag As KblFixingPropertyBag = CType(bagBase, KblFixingPropertyBag)
                                                Dim absoluteLocationValue As Double = bag.AbsoluteLocationValue
                                                If (absoluteLocationValue > 0) Then
                                                    Dim refnode As IBaseModelEntityEx = Me.Document.Entities.GetByKblIds(New String() {bag.StartNodeId}).FirstOrDefault()
                                                    If refnode IsNot Nothing Then
                                                        _D3DControl.AddDimensionTargetLabel(modelObject.Id, orig.Key, String.Format("{0} {1}{2}", bag.AbsoluteLocation, Document3DStrings.ReferenceEx, orig.Value), CSng(bag.LocationValue))
                                                    End If
                                                End If
                                            End If

                                        Case ContainerId.Protections
                                            Dim bag As KblProtectionPropertyBag = CType(bagBase, KblProtectionPropertyBag)
                                            Dim prot As Protection = CType(modelObject, Protection)


                                            Dim refnode As IBaseModelEntityEx = Me.Document.Entities.GetByKblIds(New String() {bag.StartNodeId}).FirstOrDefault()
                                            If refnode IsNot Nothing AndAlso refnode.Id = orig.Key.ToString Then
                                                If (prot.StartingPercentage > 0.0 AndAlso prot.StartingPercentage < 1.0) Then
                                                    _D3DControl.AddDimensionTargetLabel(modelObject.Id, orig.Key, String.Format("{0} {1}{2}", bag.AbsoluteStartLocation, Document3DStrings.ReferenceEx, orig.Value), 0.0)
                                                End If
                                            End If

                                            refnode = Me.Document.Entities.GetByKblIds(New String() {bag.EndNodeId}).FirstOrDefault()
                                            If refnode IsNot Nothing AndAlso refnode.Id = orig.Key.ToString Then
                                                If (prot.EndingPercentage > 0.0 AndAlso prot.EndingPercentage < 1.0) Then
                                                    _D3DControl.AddDimensionTargetLabel(modelObject.Id, orig.Key, String.Format("{0} {1}{2}", bag.AbsoluteEndLocation, Document3DStrings.ReferenceEx, orig.Value), 1.0)
                                                End If
                                            End If

                                        Case ContainerId.Supplements
                                            If TypeOf bagBase Is KblAccessoryPropertyBag Then
                                                Dim bag As KblAccessoryPropertyBag = CType(bagBase, KblAccessoryPropertyBag)
                                                Dim refnode As IBaseModelEntityEx = Me.Document.Entities.GetByKblIds(New String() {bag.StartNodeId}).FirstOrDefault()
                                                If refnode IsNot Nothing Then
                                                    _D3DControl.AddDimensionTargetLabel(modelObject.Id, orig.Key, String.Format("{0} {1}{2}", bag.AbsoluteLocation, Document3DStrings.ReferenceEx, orig.Value), CSng(bag.LocationValue))
                                                End If
                                            End If
                                    End Select
                                End If
                            Next
                        End If
                    Next
                End If
                _D3DControl.CheckAndRealignDimensionLabels()
            End If
        End If
    End Sub

    Friend Sub ShowDimensions()
        _dimMode = _dimMode Or DimMode.ReferenceDims
        ShowTargets(ShowOrigins())
        _D3DControl.Model3DControl1.Design.Invalidate()
    End Sub

    Friend Sub ShowImplicitDimensions()
        _dimMode = _dimMode Or DimMode.ImplicitDims
        ShowTargets(ShowOrigins())
        _D3DControl.Model3DControl1.Design.Invalidate()
    End Sub

    'Private Sub ShowImplicitDimensionsInternal()
    '    'HINT implicit locations are derived from the percentual location information on the assignments
    '    _showImplicitDims = True
    '    Dim index As Integer = 1
    '    If _origins.Values.Count > 0 Then
    '        index = _origins.Values.Max + 1
    '    End If

    '    Dim targets As New Dictionary(Of Guid, List(Of ObjectBaseNaming))

    '    If (_document?.IsOpen).GetValueOrDefault Then
    '        Dim model As E3.Lib.Model.EESystemModel = Me.Document.Model

    '        If model IsNot Nothing Then
    '            For Each item As IBaseModelEntityEx In Me.Document.Entities
    '                Dim txt As String = String.Empty
    '                Dim modelObject As ObjectBaseNaming = CType(model(New Guid(item.Id)), ObjectBaseNaming)

    '                If modelObject IsNot Nothing Then
    '                    Dim bagBase As KblPropertyBagBase = modelObject.CustomAttributes.OfType(Of KblPropertyBagAttribute).SingleOrDefault?.PropertyBag
    '                    If bagBase IsNot Nothing Then

    '                        Select Case modelObject.HostContainerId

    '                            Case ContainerId.AdditionalParts
    '                                If TypeOf bagBase Is KblFixingPropertyBag Then
    '                                    Dim bag As KblFixingPropertyBag = CType(bagBase, KblFixingPropertyBag)
    '                                    If bag.AbsoluteLocationValue > 0 Then
    '                                        Dim refnode As IBaseModelEntityEx = Me.Document.Entities.GetByKblIds(New String() {bag.StartNodeId}).FirstOrDefault()
    '                                        If refnode IsNot Nothing Then
    '                                            Dim sysId As Guid = New Guid(refnode.Id)
    '                                            If Not _origins.ContainsKey(sysId) Then
    '                                                _origins.Add(sysId, index)
    '                                                index += 1
    '                                            End If
    '                                            If targets.ContainsKey(sysId) Then
    '                                                targets(sysId).Add(modelObject)
    '                                            Else
    '                                                targets.Add(sysId, New List(Of ObjectBaseNaming)({modelObject}))
    '                                            End If
    '                                        End If
    '                                    End If
    '                                End If
    '                            Case ContainerId.Protections
    '                                Dim bag As KblProtectionPropertyBag = CType(bagBase, KblProtectionPropertyBag)
    '                                Dim refnode As IBaseModelEntityEx = Me.Document.Entities.GetByKblIds(New String() {bag.StartNodeId}).FirstOrDefault()
    '                                If refnode IsNot Nothing Then
    '                                    Dim prot As Protection = CType(modelObject, Protection)
    '                                    Dim sysId As Guid = New Guid(refnode.Id)
    '                                    If (prot.StartingPercentage > 0.0 AndAlso prot.StartingPercentage < 1.0) OrElse (prot.EndingPercentage > 0.0 AndAlso prot.EndingPercentage < 1.0) Then
    '                                        If Not _origins.ContainsKey(sysId) Then
    '                                            _origins.Add(sysId, index)
    '                                            index += 1
    '                                        End If
    '                                        If targets.ContainsKey(sysId) Then
    '                                            targets(sysId).Add(modelObject)
    '                                        Else
    '                                            targets.Add(sysId, New List(Of ObjectBaseNaming)({modelObject}))
    '                                        End If
    '                                    End If
    '                                End If

    '                            Case ContainerId.Supplements
    '                                If TypeOf bagBase Is KblAccessoryPropertyBag Then
    '                                    Dim bag As KblAccessoryPropertyBag = CType(bagBase, KblAccessoryPropertyBag)
    '                                    Dim refnode As IBaseModelEntityEx = Me.Document.Entities.GetByKblIds(New String() {bag.StartNodeId}).FirstOrDefault()
    '                                    If refnode IsNot Nothing Then
    '                                        Dim sysId As Guid = New Guid(refnode.Id)
    '                                        If Not _origins.ContainsKey(sysId) Then
    '                                            _origins.Add(sysId, index)
    '                                            index += 1
    '                                        End If
    '                                        If targets.ContainsKey(sysId) Then
    '                                            targets(sysId).Add(modelObject)
    '                                        Else
    '                                            targets.Add(sysId, New List(Of ObjectBaseNaming)({modelObject}))
    '                                        End If
    '                                    End If
    '                                End If

    '                        End Select

    '                    End If
    '                End If
    '            Next
    '        End If

    '        For Each orig As KeyValuePair(Of Guid, Integer) In _origins
    '            _D3DControl.AddDimensionReferenceLabel(orig.Key, orig.Value)
    '            If targets.ContainsKey(orig.Key) Then
    '                For Each modelObject As ObjectBaseNaming In targets(orig.Key)
    '                    Dim bagBase As KblPropertyBagBase = modelObject.CustomAttributes.OfType(Of KblPropertyBagAttribute).SingleOrDefault?.PropertyBag
    '                    If bagBase IsNot Nothing Then
    '                        Select Case modelObject.HostContainerId

    '                            Case ContainerId.AdditionalParts
    '                                If TypeOf bagBase Is KblFixingPropertyBag Then
    '                                    Dim bag As KblFixingPropertyBag = CType(bagBase, KblFixingPropertyBag)
    '                                    Dim absoluteLocationValue As Double = bag.AbsoluteLocationValue
    '                                    If (absoluteLocationValue > 0) Then
    '                                        Dim refnode As IBaseModelEntityEx = Me.Document.Entities.GetByKblIds(New String() {bag.StartNodeId}).FirstOrDefault()
    '                                        If refnode IsNot Nothing Then
    '                                            _D3DControl.AddDimensionTargetLabel(modelObject.Id, orig.Key, String.Format("{0} {1} {2}", bag.AbsoluteLocation, Document3DStrings.ReferenceEx, orig.Value.ToString), CSng(bag.LocationValue))
    '                                        End If
    '                                    End If
    '                                End If

    '                            Case ContainerId.Protections
    '                                Dim bag As KblProtectionPropertyBag = CType(bagBase, KblProtectionPropertyBag)
    '                                Dim prot As Protection = CType(modelObject, Protection)

    '                                Dim refnode As IBaseModelEntityEx = Me.Document.Entities.GetByKblIds(New String() {bag.StartNodeId}).FirstOrDefault()
    '                                If refnode IsNot Nothing Then
    '                                    If (prot.StartingPercentage > 0.0 AndAlso prot.StartingPercentage < 1.0) Then
    '                                        _D3DControl.AddDimensionTargetLabel(modelObject.Id, orig.Key, String.Format("{0} {1} {2}", bag.AbsoluteStartLocation, Document3DStrings.ReferenceEx, orig.Value.ToString), 0.0)
    '                                    End If

    '                                    If (prot.EndingPercentage > 0.0 AndAlso prot.EndingPercentage < 1.0) Then
    '                                        _D3DControl.AddDimensionTargetLabel(modelObject.Id, orig.Key, String.Format("{0} {1} {2}", bag.AbsoluteEndLocation, Document3DStrings.ReferenceEx, orig.Value.ToString), 1.0)
    '                                    End If
    '                                End If

    '                            Case ContainerId.Supplements
    '                                If TypeOf bagBase Is KblAccessoryPropertyBag Then
    '                                    Dim bag As KblAccessoryPropertyBag = CType(bagBase, KblAccessoryPropertyBag)
    '                                    Dim refnode As IBaseModelEntityEx = Me.Document.Entities.GetByKblIds(New String() {bag.StartNodeId}).FirstOrDefault()
    '                                    If refnode IsNot Nothing Then
    '                                        _D3DControl.AddDimensionTargetLabel(modelObject.Id, orig.Key, String.Format("{0} {1} {2}", bag.AbsoluteLocation, Document3DStrings.ReferenceEx, orig.Value.ToString), CSng(bag.LocationValue))
    '                                    End If
    '                                End If
    '                        End Select
    '                    End If
    '                Next
    '            End If
    '        Next

    '        _D3DControl.CheckAndRealignDimensionLabels()
    '        _D3DControl.Model3DControl1.Design.Invalidate()
    '    End If
    'End Sub

    'Private Function GetLocation(p As Protection, nodetype As NodeType) As Double

    '    If nodetype = NodeType.StartNode Then
    '        For Each s As E3.Lib.Model.Segment In p.GetSegments().Entries
    '            Dim vertex = s.GetVertices().Entries.ToList().Where(Function(v) v.Id = p.StartVertexId).FirstOrDefault()
    '            If vertex IsNot Nothing AndAlso s.NomLength <> Nothing Then
    '                Return s.NomLength * p.StartingPercentage
    '                Exit For
    '            End If
    '        Next
    '    End If

    '    If nodetype = NodeType.EndNode Then
    '        Dim l As Double = 0.0
    '        For Each s As E3.Lib.Model.Segment In p.GetSegments().Entries
    '            l += s.NomLength
    '        Next
    '        For Each s As E3.Lib.Model.Segment In p.GetSegments().Entries
    '            Dim vertex = s.GetVertices().Entries.ToList().Where(Function(v) v.Id = p.EndVertexId).FirstOrDefault()
    '            If vertex IsNot Nothing AndAlso s.NomLength <> Nothing Then
    '                Return l - (s.NomLength * p.EndingPercentage)
    '                Exit For
    '            End If
    '        Next
    '    End If

    '    Return 0

    'End Function


    'Private Sub ShowDimensionsInternal()
    '    'HINT these dimensions are specified within the kbl
    '    _showDims = True
    '    If Not _showImplicitDims Then _origins.Clear()
    '    Dim index As Integer = 1

    '    If _origins.Values.Count > 0 Then
    '        index = _origins.Values.Max + 1
    '    End If

    '    If (_document?.IsOpen).GetValueOrDefault Then
    '        Dim model As E3.Lib.Model.EESystemModel = Me.Document.Model
    '        If model IsNot Nothing Then
    '            For Each item As [Lib].Model.Dimension In model.Dimensions
    '                If item.ReferenceObjectId <> Guid.Empty Then
    '                    Dim originId As Guid = item.ReferenceObjectId
    '                    Dim entity As IBaseModelEntityEx = Me.Document.Entities.Where(Function(x) x.Id = originId.ToString).FirstOrDefault()
    '                    If entity IsNot Nothing Then
    '                        If Not _origins.ContainsKey(originId) Then
    '                            _origins.Add(originId, index)
    '                            index += 1
    '                        End If
    '                    End If
    '                End If
    '            Next
    '        End If

    '        For Each org As KeyValuePair(Of Guid, Integer) In _origins
    '            Dim entity As IBaseModelEntityEx = Me.Document.Entities.Where(Function(x) x.Id = org.Key.ToString).FirstOrDefault()
    '            If entity IsNot Nothing Then
    '                Dim ent As ObjectBase = model.Item(org.Key)
    '                If TypeOf ent Is AdditionalPartAssignment Then
    '                    Dim assignment As AdditionalPartAssignment = CType(entity, AdditionalPartAssignment)

    '                    If assignment IsNot Nothing Then
    '                        Dim seg As E3.Lib.Model.Segment = assignment.GetSegment
    '                        Dim part As AdditionalPart = assignment.GetAdditionalPart
    '                        If seg IsNot Nothing AndAlso part IsNot Nothing Then
    '                            _D3DControl.AddDimensionReferenceLabel(part.Id, org.Value, assignment.StartingPercentage, seg.Id)
    '                        End If
    '                    End If
    '                Else
    '                    _D3DControl.AddDimensionReferenceLabel(org.Key, org.Value)
    '                End If
    '            End If
    '        Next

    '        For Each dms As E3.Lib.Model.Dimension In model.Dimensions
    '            Dim target As ObjectBase = dms.GetTargetObject
    '            If target IsNot Nothing Then
    '                If TypeOf target Is AdditionalPartAssignment Then
    '                    Dim assignment As AdditionalPartAssignment = CType(target, AdditionalPartAssignment)
    '                    If assignment IsNot Nothing Then
    '                        Dim seg As E3.Lib.Model.Segment = assignment.GetSegment
    '                        Dim part As AdditionalPart = assignment.GetAdditionalPart
    '                        If seg IsNot Nothing AndAlso part IsNot Nothing Then
    '                            _D3DControl.AddDimensionTargetLabel(part.Id, dms.ReferenceObjectId, _origins(dms.ReferenceObjectId), dms, assignment.StartingPercentage, seg.Id)
    '                        End If
    '                    End If
    '                Else
    '                    _D3DControl.AddDimensionTargetLabel(target.Id, dms.ReferenceObjectId, _origins(dms.ReferenceObjectId), dms)
    '                End If
    '            End If
    '        Next
    '        _D3DControl.Model3DControl1.Design.Invalidate()
    '    End If
    'End Sub

    Friend Sub ClearStartViewVertex()
        _D3DControl.ClearViewVertexDirections()
        _D3DControl.Model3DControl1.Design.Invalidate()
    End Sub

    Friend Sub ClearDimensions()
        _dimMode = _dimMode And Not DimMode.ReferenceDims
        ShowTargets(ShowOrigins())
        _D3DControl.Model3DControl1.Design.Invalidate()
    End Sub

    Friend Sub ClearImplicitDimensions()
        _dimMode = _dimMode And Not DimMode.ImplicitDims
        ShowTargets(ShowOrigins())
        _D3DControl.Model3DControl1.Design.Invalidate()
    End Sub

    Private Function GetKblActiveModuleIds() As String()
        Dim activeModuleIds As New List(Of String)
        For Each m As [Lib].Schema.Kbl.[Module] In _kblMapper.GetModules
            If Not _kblMapper.InactiveModules.ContainsKey(m.SystemId) Then
                activeModuleIds.Add(m.SystemId)
            End If
        Next
        Return activeModuleIds.ToArray
    End Function

    Friend Sub ShowCorrespondingRubberlines(cavityId As String)
        Dim model As E3.Lib.Model.EESystemModel = Me.Document.Model
        If model IsNot Nothing Then
            If (TypeOf _kblMapper.KBLOccurrenceMapper(cavityId) Is Cavity_occurrence) Then
                Try
                    Dim activeModuleIds As New List(Of String)
                    For Each m As [Lib].Schema.Kbl.[Module] In _kblMapper.GetModules
                        If Not _kblMapper.InactiveModules.ContainsKey(m.SystemId) Then
                            activeModuleIds.Add(m.SystemId)
                        End If
                    Next

                    For Each conn As E3.Lib.Model.Connector In model.Connectors
                        SetModelObjVariant(conn, model.Variants.MasterVariant(), activeModuleIds)
                    Next

                    For Each wir As E3.Lib.Model.Wire In model.Wires
                        SetModelObjVariant(wir, model.Variants.MasterVariant(), activeModuleIds)
                    Next

                    Dim cavity As E3.Lib.Model.Cavity = CType(_document.Entities.GetByKblIds(cavityId).SelectMany(Function(ent) ent.GetEEModelObjects).SingleOrDefault, E3.Lib.Model.Cavity)
                    _D3DControl.ShowCorrespondingRubberLines(cavity)
                Catch ex As Exception
                    _D3DControl.ClearRubberLines()
                    MessageBoxEx.ShowError(Me, ex.GetInnerOrDefaultMessage)
                End Try
            Else
                Throw New ArgumentException($"KBL-id ""{cavityId}"" is not a cavity!")
            End If
        End If
    End Sub

    Public Async Function SetConsolidateVisibileCheckBoxState(enabled As Boolean, Optional waitForSetCheckBoxTabUIElement As Boolean = False) As Task
        Await CType(Me.MainForm.D3D, D3D.MainFormController).SetConsolidateVisibileCheckBoxState(Me, enabled)
    End Function

    Private Sub SetModelObjVariant(obj As E3.Lib.Model.ObjectBaseNaming, var As E3.Lib.Model.Variant, activeModuleIds As IEnumerable(Of String))
        Dim attr As KblPropertyBagAttribute = obj.CustomAttributes.OfType(Of KblPropertyBagAttribute).SingleOrDefault
        If attr IsNot Nothing Then
            Dim bag As KblPropertyBagBase = CType(attr.PropertyBag, KblPropertyBagBase)
            If (_kblMapper.KBLObjectModuleMapper.ContainsKey(bag.SystemId)) Then
                obj.ActivateInVariant(var, activeModuleIds.ContainsAnyOf(_kblMapper.KBLObjectModuleMapper(bag.SystemId)))
            End If
        End If
    End Sub

    Private Sub _magnifier_OnFinish(theAction As BaseAction) Handles MagnifierAction.OnFinish
        If Me.Tools.Home.ZoomMagnifierBtn.Checked Then
            Me.Tools.Home.ZoomMagnifierBtn.Checked = False
        End If
    End Sub

    Private Sub PanAction_OnFinish(theAction As BaseAction) Handles PanAction.OnFinish
        If Me.Tools.Home.PanBtn.Checked Then
            Me.Tools.Home.PanBtn.Checked = False
        End If
    End Sub

    Private Sub PanAction_OnMouseDown(args As BaseAction.BaseActionMouseEventArgs) Handles PanAction.OnMouseDown
        If args.MouseArgs.Button = MouseButtons.Right Then PanAction.FinishAction(PanAction) 'HINT: Pan Action does not finish on right mouse, it uses cancel internally
    End Sub

    Private Sub utcDocument_SelectedTabChanged(sender As Object, e As SelectedTabChangedEventArgs) Handles utcDocument.SelectedTabChanged
        CheckPanOrMagnifierActions()
    End Sub

    Private Sub utcDocument_ActiveTabChanging(sender As Object, e As ActiveTabChangingEventArgs) Handles utcDocument.ActiveTabChanging
        Dim tab As UltraTab = CType(sender, UltraTabControl).ActiveTab
        If tab IsNot Nothing Then
            CancelPanAndMagnifierActions(tab)
        End If
    End Sub

    Public Sub CheckPanOrMagnifierActions()
        If ActiveDrawingCanvas IsNot Nothing OrElse IsDocument3DActive Then
            Dim panStateButton As StateButtonTool = Me.Tools.Home.PanBtn
            Dim magnifierStateButton As StateButtonTool = Me.Tools.Home.ZoomMagnifierBtn

            If panStateButton.Checked Then
                HandlePanAction(True)
            Else
                HandlePanAction(False)
                OnPanActionDisabled(New EventArgs)
            End If

            If magnifierStateButton.Checked Then
                HandleMagnifierAction(True)
            Else
                HandleMagnifierAction(False)
                OnMagnifierActionDisabled(New EventArgs)
            End If

        End If
    End Sub

    Private Sub CancelPanAndMagnifierActions(tab As UltraTab)
        Dim myCanvas As DrawingCanvas = TryCast(tab.TabPage.Controls(0), DrawingCanvas)
        If myCanvas IsNot Nothing Then
            Dim doc As vdDocument = myCanvas.vdCanvas.BaseControl.ActiveDocument

            If Me.Tools.Home.PanBtn?.Checked OrElse Me.Tools.Home.ZoomMagnifierBtn?.Checked Then
                doc.CommandAction.Cancel()
            End If

        End If
    End Sub

    Friend Function GetSelectedKblIds() As String()
        Dim kblIds As New HashSet(Of String)
        kblIds.AddRange(Me.GetAllDrawingCanvas.SelectMany(Function(cv) cv.CanvasSelection.OfType(Of VdSVGGroup).Select(Function(vdIss) vdIss.KblId)))
        If _document.IsOpen AndAlso _D3DControl IsNot Nothing Then
            kblIds.AddRange(_D3DControl.Model3DControl1.Design.Entities.OfType(Of devDept.Eyeshot.Entities.Entity).Where(Function(ent) ent.Selected).OfType(Of IBaseModelEntity).SelectMany(Function(ent) ent.GetKblIds).ToArray)
        End If
        Return kblIds.ToArray
    End Function

    Public Sub CreateRedlinings()
        If _D3DControl IsNot Nothing Then
            _entityRedlinings.Clear()
            _D3DControl.ClearRedliningLabels()

            For Each r As Redlining In _redliningInformation.Redlinings
                Dim objId As String = r.ObjectId
                If (_kblMapper.KBLContactPointConnectorMapper.ContainsKey(objId)) Then
                    objId = _kblMapper.KBLContactPointConnectorMapper(objId)
                End If

                Dim entity As IBaseModelEntityEx = Me.Document.Entities.GetByKblIds(New String() {objId}).FirstOrDefault()
                If (entity IsNot Nothing) Then
                    If Not _entityRedlinings.TryAdd(entity.Id, r) Then
                        If _entityRedlinings.Item(entity.Id).Classification < r.Classification Then
                            _entityRedlinings.Item(entity.Id) = r
                        End If
                    End If
                End If
            Next

            For Each item As KeyValuePair(Of String, Redlining) In _entityRedlinings
                Dim myEntity As BaseModelEntity = devDept.Eyeshot.Entities.Flatten(_document.Entities.GetByEEObjectId(New Guid(item.Key)).Cast(Of IEntity)).Cast(Of BaseModelEntity).ToList.FirstOrDefault
                If myEntity IsNot Nothing Then
                    _D3DControl.AddRedlining(item.Value, myEntity)
                End If
            Next

            _D3DControl.Model3DControl1.Design.Invalidate()
        End If
    End Sub

    Public ReadOnly Property MemoList As Memolist
        Get
            Return _memoList
        End Get
    End Property

    Public ReadOnly Property QmStamps As QMStamps
        Get
            Return _qmStamps
        End Get
    End Property

    Public ReadOnly Property redliningInformation As RedliningInformation
        Get
            Return _redliningInformation
        End Get
    End Property

    Public Sub HideRedlinings()
        If _D3DControl IsNot Nothing Then
            _D3DControl.ClearRedliningLabels()
            _D3DControl.Model3DControl1.Design.Invalidate()
        End If
    End Sub

    Private Enum NodeType
        StartNode
        EndNode
    End Enum

    Public Sub WriteLogMessage(sourceType As ConverterType, e As Zuken.E3.Lib.Converter.Svg.MessageEventArgs, uselogEventArgs As LogEventArgs)
        If uselogEventArgs Is Nothing Then
            uselogEventArgs = New LogEventArgs
        End If

        Select Case e.MessageType
            Case Zuken.E3.Lib.Converter.Svg.MessageType.Undefined
                uselogEventArgs.LogLevel = LogEventArgs.LoggingLevel.Information
                uselogEventArgs.LogMessage = e.Message

            Case Zuken.E3.Lib.Converter.Svg.MessageType.ErrorCheckSVG
                Select Case sourceType
                    Case ConverterType.SvgConverter
                        uselogEventArgs.LogLevel = LogEventArgs.LoggingLevel.Error
                        uselogEventArgs.LogMessage = String.Format(My.Resources.LoggingStrings.SvgConv_ProblemCheckSVG, e.Message)
                    Case Else
                        Throw New NotImplementedException
                End Select

            Case Zuken.E3.Lib.Converter.Svg.MessageType.ErrorLoadFile
                Dim info As XmlPositionInfo = CType(e.UserData, XmlPositionInfo)
                uselogEventArgs.LogLevel = LogEventArgs.LoggingLevel.Error

                Select Case sourceType
                    Case ConverterType.SvgConverter
                        uselogEventArgs.LogMessage = String.Format(My.Resources.LoggingStrings.SvgConv_ProblemLoadSVG, info.LineNumber, IO.Path.GetFileName(info.XmlFileName), e.Message)
                    Case ConverterType.DraftConverter
                        uselogEventArgs.LogMessage = String.Format(My.Resources.LoggingStrings.DraftConv_ProblemLoadDrawing, e.Message)
                End Select

            Case Zuken.E3.Lib.Converter.Svg.MessageType.BeginLoadingFile
                uselogEventArgs.LogLevel = LogEventArgs.LoggingLevel.Information
                Select Case sourceType
                    Case ConverterType.SvgConverter
                        uselogEventArgs.LogMessage = String.Format(My.Resources.LoggingStrings.SvgConv_StartLoadFile, IO.Path.GetFileName(e.Message), Now)
                    Case ConverterType.DraftConverter
                        uselogEventArgs.LogMessage = String.Format(My.Resources.LoggingStrings.DraftConv_StartLoadDrawing, IO.Path.GetFileName(e.Message), Now)
                End Select

            Case Zuken.E3.Lib.Converter.Svg.MessageType.FinishedLoadDrawing
                uselogEventArgs.LogLevel = LogEventArgs.LoggingLevel.Information

                Select Case sourceType
                    Case ConverterType.SvgConverter
                        uselogEventArgs.LogMessage = String.Format(My.Resources.LoggingStrings.SvgConv_FinishedLoadDrawing, IO.Path.GetFileName(e.Message), Now)
                    Case ConverterType.DraftConverter
                        uselogEventArgs.LogMessage = String.Format(My.Resources.LoggingStrings.DraftConv_FinishedLoadDrawing, IO.Path.GetFileName(e.Message), Now)
                    Case Else
                        Throw New NotImplementedException(sourceType.ToString)
                End Select

            Case Zuken.E3.Lib.Converter.Svg.MessageType.ValidationFailsGroup
                Select Case sourceType
                    Case ConverterType.SvgConverter
                        Dim info As XmlPositionInfo = CType(e.UserData, XmlPositionInfo)
                        uselogEventArgs.LogLevel = LogEventArgs.LoggingLevel.Warning
                        uselogEventArgs.LogMessage = String.Format(My.Resources.LoggingStrings.SvgConv_ValidationFailsGroup, IO.Path.GetFileNameWithoutExtension(info.XmlFileName), info.ElementName, info.LineNumber)
                    Case Else
                        Throw New NotImplementedException
                End Select
            Case Zuken.E3.Lib.Converter.Svg.MessageType.CancelledLoadDrawing
                Select Case sourceType
                    Case ConverterType.SvgConverter
                        uselogEventArgs.LogLevel = LogEventArgs.LoggingLevel.Warning
                        uselogEventArgs.LogMessage = String.Format(My.Resources.LoggingStrings.SvgConv_CancelledLoadDrawing, IO.Path.GetFileName(e.Message), Now)
                    Case Else
                        Throw New NotImplementedException
                End Select

            Case Zuken.E3.Lib.Converter.Svg.MessageType.ValidationFails
                Select Case sourceType
                    Case ConverterType.SvgConverter
                        Dim info As E3.Lib.Converter.Svg.XmlGroupStylePositionInfo = CType(e.UserData, XmlGroupStylePositionInfo)
                        uselogEventArgs.LogLevel = LogEventArgs.LoggingLevel.Warning
                        uselogEventArgs.LogMessage = String.Format(My.Resources.LoggingStrings.SvgConv_ValidationFailsText, IO.Path.GetFileNameWithoutExtension(info.XmlFileName), info.ElementName, info.LineNumber, If(info.Group IsNot Nothing AndAlso info.Group.Figures.Count > 0, DirectCast(info.Group.Figures.Last, VdTextEx).TextString, If(info.Style.FillPattern IsNot Nothing, DirectCast(info.Style.FillPattern.Entities.Last, VdTextEx).TextString, Zuken.E3.HarnessAnalyzer.Shared.NOT_AVAILABLE)))
                    Case Else
                        Throw New NotImplementedException
                End Select

            Case [Lib].Converter.Svg.MessageType.WarningNoSegmentVertexInKbl
                Select Case sourceType
                    Case ConverterType.DraftConverter
                        uselogEventArgs.LogLevel = LogEventArgs.LoggingLevel.Warning
                        uselogEventArgs.LogMessage = String.Format(My.Resources.LoggingStrings.DraftConv_NoSegmentVertexInKBL, e.Message)
                    Case Else
                        Throw New NotImplementedException()
                End Select

            Case [Lib].Converter.Svg.MessageType.ErrorLoadingVertices
                Select Case sourceType
                    Case ConverterType.DraftConverter
                        uselogEventArgs.LogLevel = LogEventArgs.LoggingLevel.Error
                        uselogEventArgs.LogMessage = String.Format(My.Resources.LoggingStrings.DraftConv_ProblemLoadVertices, e.Message)
                    Case Else
                        Throw New NotImplementedException()
                End Select

            Case [Lib].Converter.Svg.MessageType.ErrorLoadingSegments
                Select Case sourceType
                    Case ConverterType.DraftConverter
                        uselogEventArgs.LogLevel = LogEventArgs.LoggingLevel.Error
                        uselogEventArgs.LogMessage = String.Format(My.Resources.LoggingStrings.DraftConv_ProblemLoadSegments, e.Message)
                    Case Else
                        Throw New NotImplementedException()
                End Select

            Case [Lib].Converter.Svg.MessageType.ErrorLoadingFixings
                Select Case sourceType
                    Case ConverterType.DraftConverter
                        uselogEventArgs.LogLevel = LogEventArgs.LoggingLevel.Error
                        uselogEventArgs.LogMessage = String.Format(My.Resources.LoggingStrings.DraftConv_ProblemLoadFixings, e.Message)
                    Case Else
                        Throw New NotImplementedException()
                End Select

            Case [Lib].Converter.Svg.MessageType.ErrorLoadingAccessories
                Select Case sourceType
                    Case ConverterType.DraftConverter
                        uselogEventArgs.LogLevel = LogEventArgs.LoggingLevel.Error
                        uselogEventArgs.LogMessage = String.Format(My.Resources.LoggingStrings.DraftConv_ProblemLoadAccessories, e.Message)
                    Case Else
                        Throw New NotImplementedException()
                End Select
            Case Else
                Throw New NotImplementedException(e.MessageType.ToString)
        End Select

        _logHub.WriteLogMessage(uselogEventArgs)
    End Sub

    Protected Overridable Sub OnActiveDrawingCanvasChanged(e As EventArgs)
        RaiseEvent ActiveDrawingCanvasChanged(Me, e)
    End Sub

    Private Sub _hcvFile_Progress(sender As Object, e As ProgressChangedEventArgs) Handles _hcvFile.Progress
        If _messageEventArgs IsNot Nothing Then
            _messageEventArgs.ProgressPercentage = e.ProgressPercentage
            OnMessage(_messageEventArgs)
        End If
    End Sub

    Private Sub _activeDrawingCanvas_CanvasDrawFinished(sender As Object, e As EventArgs) Handles _activeDrawingCanvas.DrawingDisplayFinished
        Me.FilterActiveObjects(False, True)
        Me.UpdateNavigatorImage(True)
    End Sub

    Public ReadOnly Property Panes As DocumentToolPanesCollection
        Get
            Return _panes
        End Get
    End Property

    Public ReadOnly Property Tools As UtDocumentTools
        Get
            Return _documentStateMachine.Tools
        End Get
    End Property

    Public ReadOnly Property KBL As KblMapper
        Get
            Return _kblMapper
        End Get
    End Property

End Class