Imports Infragistics.Win.UltraWinTabbedMdi
Imports Zuken.E3.App.Controls
Imports Zuken.E3.HarnessAnalyzer.Schematics.Controls
Imports Zuken.E3.HarnessAnalyzer.Schematics.Converter
Imports Zuken.E3.HarnessAnalyzer.Schematics.Converter.Kbl
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Partial Public Class MainForm

    Private _componentResolvedResults As New Concurrent.ConcurrentDictionary(Of Guid, Dictionary(Of String, ResolvingResult))
    Private _messageEventArgsSchematics As New MessageEventArgs(MessageEventArgs.MsgType.ShowProgressAndMessage, 0, String.Empty)
    Private _lastGetEdbConversionId As Guid = Guid.Empty
    Private _lastEntityCreatedConversionId As Guid = Guid.Empty
    Private _documentsDic As New Dictionary(Of String, DocumentForm)
    Private _documentsOfLastConverter As New Dictionary(Of String, DocumentForm)
    Private _isShowingLock As New System.Threading.SemaphoreSlim(1)

    Private Sub _schematicsView_AfterEntityCreated(sender As Object, e As EntityEventArgs) Handles _schematicsView.AfterEntityCreated
        _lastEntityCreatedConversionId = e.ConverterId
        If _lastEntityCreatedConversionId <> e.ConverterId Then
            _documentsOfLastConverter = GetAllDocuments()
            _lastEntityCreatedConversionId = e.ConverterId
        End If

        Dim doc As DocumentForm = Nothing
        If _documentsOfLastConverter.TryGetValue(e.documentId, doc) Then
            doc.SetActiveConnectiviy(e.Entity)
        End If
    End Sub

    Private Sub _schematicsView_Error(sender As Object, e As Schematics.Converter.Kbl.ErrorEventArgs) Handles _schematicsView.Error
        Dim documents As Dictionary(Of String, DocumentForm) = Me.GetAllDocuments
        If documents.ContainsKey(e.DocumentId) Then
            documents(e.DocumentId)._logHub.WriteLogMessage(New LogEventArgs(LogEventArgs.LoggingLevel.Warning, e.Message))
        End If
    End Sub

    Private Sub _schematicsView_ConnectorResolving(sender As Object, e As ConnectorResolvingEventArgs) Handles _schematicsView.ConnectorResolving
        Dim res As ResolvingResult = ResolveFromProperties(e.Connector.ShortName, e.Connector.Description, e.Connector.AliasIds, e.Connector.InstallationInstructions, e.ConverterId)
        With e.Connector
            If .IsSplice Then
                .Component.Type = Connectivity.Model.ComponentType.Splice
            ElseIf (.IsEyelet) Then
                .Component.Type = Connectivity.Model.ComponentType.Eyelet
            Else
                .Component.Type = res.ComponentType
            End If
            .Component.ShortName = res.ComponentShortName
            .ShortName = res.ConnectorShortName
            .InlinerName = res.InlinerName
        End With
    End Sub

    Private Sub _schematicsView_GetComponentType(sender As Object, e As ComponentTypeResolveEventArgs) Handles _schematicsView.ResolveComponentType
        Dim res As ResolvingResult = ResolveFromProperties(e.ComponentShortName, "", New List(Of String), New List(Of String), e.ConverterId)
        e.ComponentType = res.ComponentType
    End Sub

    Private Function ResolveFromProperties(shortName As String, description As String, aliasIds As List(Of String), installationInstr As List(Of String), converterId As Guid) As ResolvingResult
        Dim dic As Dictionary(Of String, ResolvingResult) = _componentResolvedResults.GetOrAdd(converterId, New Dictionary(Of String, ResolvingResult))
        Dim resResult As ResolvingResult = Nothing
        If Not dic.TryGetValue(shortName, resResult) Then
            resResult = New ResolvingResult
            dic.Add(shortName, resResult)

            With resResult
                .ComponentType = Connectivity.Model.ComponentType.Ecu ' HINT: everything else is ecu
                .ConnectorShortName = shortName
                .ComponentShortName = shortName

                Dim inlinerName As String = ""
                If (IsInliner(shortName, description, aliasIds, installationInstr, inlinerName)) Then
                    .ComponentShortName = inlinerName
                    .InlinerName = inlinerName
                    .ConnectorShortName = shortName
                    .ComponentType = Connectivity.Model.ComponentType.Inliner

                ElseIf (IsSplice(shortName, description, aliasIds, installationInstr)) Then
                    .ComponentType = Connectivity.Model.ComponentType.Splice

                ElseIf (IsEyelet(shortName, description, aliasIds, installationInstr)) Then
                    .ComponentType = Connectivity.Model.ComponentType.Eyelet
                Else
                    Dim indentifier As EcuConnectorIdentifier = _generalSettings.EcuConnectorIdentifier
                    If indentifier IsNot Nothing Then
                        If indentifier.IsMatch(shortName) Then
                            .ComponentShortName = indentifier.GetComponentRecognizer(shortName)
                            .ConnectorShortName = indentifier.GetConnectorRecognizer(shortName)
                        End If
                    End If
                End If
            End With
        End If
        Return resResult
    End Function

    Private Function IsInliner(shortName As String, description As String, aliasIds As List(Of String), installationInstr As List(Of String), ByRef inlinerName As String) As Boolean
        Dim matchCount As Integer = 0

        For Each identifier As InlinerIdentifier In _generalSettings.InlinerIdentifiers
            Select Case identifier.KBLPropertyName
                Case ConnectorPropertyName.Id.ToString
                    If identifier.IsMatch(shortName) Then
                        matchCount += 1
                        If String.IsNullOrEmpty(inlinerName) Then
                            inlinerName = identifier.GetConnectorRecognizer(shortName)
                        End If
                    End If

                Case ConnectorPropertyName.Description.ToString
                    If identifier.IsMatch(description) Then
                        matchCount += 1
                        If String.IsNullOrEmpty(inlinerName) Then
                            inlinerName = identifier.GetConnectorRecognizer(description)
                        End If
                    End If

                Case ConnectorPropertyName.Alias_id.ToString
                    For Each id As String In aliasIds
                        If identifier.IsMatch(id) Then
                            matchCount += 1
                            If String.IsNullOrEmpty(inlinerName) Then
                                inlinerName = identifier.GetConnectorRecognizer(id)
                            End If
                            Exit For
                        End If
                    Next

                Case ConnectorPropertyName.Installation_Information.ToString
                    For Each id As String In installationInstr
                        If identifier.IsMatch(id) Then
                            matchCount += 1
                            If String.IsNullOrEmpty(inlinerName) Then
                                inlinerName = identifier.GetConnectorRecognizer(id)
                            End If
                            Exit For
                        End If
                    Next

            End Select
        Next
        Return CBool(_generalSettings.InlinerIdentifiers.Count > 0 AndAlso (matchCount = _generalSettings.InlinerIdentifiers.Count))
    End Function

    Private Function IsSplice(shortName As String, description As String, aliasIds As List(Of String), installationInstr As List(Of String)) As Boolean
        Dim matchCount As Integer = 0
        For Each identifier As SpliceIdentifier In _generalSettings.SpliceIdentifiers
            Select Case identifier.KBLPropertyName
                Case ConnectorPropertyName.Id.ToString
                    If identifier.IsMatch(shortName) Then
                        matchCount += 1
                    End If

                Case ConnectorPropertyName.Description.ToString
                    If identifier.IsMatch(description) Then
                        matchCount += 1
                    End If

                Case ConnectorPropertyName.Alias_id.ToString
                    For Each id As String In aliasIds
                        If identifier.IsMatch(id) Then
                            matchCount += 1
                            Exit For
                        End If
                    Next

                Case ConnectorPropertyName.Installation_Information.ToString
                    For Each id As String In installationInstr
                        If identifier.IsMatch(id) Then
                            matchCount += 1
                            Exit For
                        End If
                    Next
            End Select
        Next
        Return CBool(_generalSettings.SpliceIdentifiers.Count > 0 AndAlso (matchCount = _generalSettings.SpliceIdentifiers.Count))
    End Function

    Private Function IsEyelet(shortName As String, description As String, aliasIds As List(Of String), installationInstr As List(Of String)) As Boolean
        Dim matchCount As Integer = 0
        For Each identifier As EyeletIdentifier In _generalSettings.EyeletIdentifiers
            Select Case identifier.KBLPropertyName
                Case ConnectorPropertyName.Id.ToString
                    If identifier.IsMatch(shortName) Then
                        matchCount += 1
                    End If

                Case ConnectorPropertyName.Description.ToString
                    If identifier.IsMatch(description) Then
                        matchCount += 1
                    End If

                Case ConnectorPropertyName.Alias_id.ToString
                    For Each id As String In aliasIds
                        If identifier.IsMatch(id) Then
                            matchCount += 1
                            Exit For
                        End If
                    Next

                Case ConnectorPropertyName.Installation_Information.ToString
                    For Each id As String In installationInstr
                        If identifier.IsMatch(id) Then
                            matchCount += 1
                            Exit For
                        End If
                    Next

            End Select
        Next
        Return CBool(_generalSettings.EyeletIdentifiers.Count > 0 AndAlso (matchCount = _generalSettings.EyeletIdentifiers.Count))
    End Function

    Private Class ResolvingResult
        Public Property ComponentShortName As String
        Public Property ConnectorShortName As String
        Public Property InlinerName As String
        Public Property ComponentType As Connectivity.Model.ComponentType
    End Class

    Private Sub _schematicsView_GetEdbId(sender As Object, e As Schematics.Converter.IdEventArgs) Handles _schematicsView.ResolveEntityId
        If _lastGetEdbConversionId <> e.ConverterId Then
            _documentsDic = utmmMain.TabGroups.All.Cast(Of MdiTabGroup).SelectMany(Function(grp) grp.Tabs.All.Cast(Of MdiTab)()).Select(Function(tb) TryCast(tb.Form, DocumentForm)).Where(Function(frm) frm IsNot Nothing).ToDictionary(Function(frm) frm.Id, Function(frm) frm)
            _lastGetEdbConversionId = e.ConverterId
        End If

        Dim doc As DocumentForm = Nothing
        If _documentsDic.TryGetValue(e.documentId, doc) Then
            e.EdbId = doc.CreateEdbId(e.Id)
        End If
    End Sub

    Private Sub _schematicsView_SelectionChanged(sender As Object, e As Schematics.SelectionChangedEventArgs) Handles _schematicsView.SelectionChanged
        If _schematicsView.IsShowingActiveEntities2 Then ' hint: block selection-Change on information-Hub while showing-schematics because this will/can clear the selection indirectly if some entities have been selected in schematics-View
            Return
        End If

        Dim objsByDoc As New Dictionary(Of String, HashSet(Of String))
        For Each entInfo As EdbConversionEntityInfo In e.Selected
            With entInfo
                Dim lst As New HashSet(Of String)
                If Not objsByDoc.TryAdd(.DocumentId, lst) Then
                    lst = objsByDoc(.DocumentId)
                End If
                lst.AddRange(.OriginalIds)
            End With
        Next

        Dim selectedForm As DocumentForm = Nothing
        Dim lastSelectedForm As DocumentForm = Nothing
        Dim d3DIsByDoc As New List(Of String)

        For Each docForm As DocumentForm In GetAllDocuments.Values
            If selectedForm Is Nothing AndAlso Me.utmmMain.TabFromForm(docForm).IsSelected Then
                selectedForm = docForm
            End If
            Dim objIdsToSelect As New HashSet(Of String)
            With docForm
                If objsByDoc.TryGetValue(.Id.ToString, objIdsToSelect) Then
                    d3DIsByDoc.AddRange(objIdsToSelect)
                    If .SelectRowsInGrids(objIdsToSelect.ToList, True, True, False, False).Count > 0 Then ' HINT: 3D Selection per document is deactivated because the multiple document selection can destroy each other on the document -> We do this selection to the 3D view at the end of this block by our own to ensure one big selection call (and not a lot's of document-selection calls)
                        lastSelectedForm = docForm
                        objsByDoc.Remove(.Id.ToString)
                    End If
                Else
                    .SelectRowsInGrids(New List(Of String), True, True, False, False)
                End If
            End With
        Next

        'HINT: see hint above (_informationHub.SelectRowsInGrids)
        'Dim tsk As Task = Me.Select3DEntitiesAsync(d3DIsByDoc.Distinct)

        If lastSelectedForm IsNot Nothing AndAlso selectedForm IsNot lastSelectedForm Then
            Dim frmTab As MdiTab = utmmMain.TabFromForm(lastSelectedForm)
            frmTab.TabGroup.SelectedTab = frmTab
            _schematicsView.Select()
        End If
    End Sub

    Private Sub _schematicsView_ProgressingFinished(sender As Object, e As Schematics.Converter.ConverterEventArgs) Handles _schematicsView.ProgressingFinished
        If _activeDocument IsNot Nothing Then
            'HINT: clean up buffer
            Dim dic As Dictionary(Of String, ResolvingResult) = Nothing
            If Me._componentResolvedResults.TryRemove(e.ConverterId, dic) Then
                dic.Clear()
            End If
        End If
    End Sub

    Friend Function GetAllDocuments() As Dictionary(Of String, DocumentForm)
        Dim docs As New Dictionary(Of String, DocumentForm)
        If utmMain IsNot Nothing Then
            For Each drawingMdiTab As MdiTab In Me.utmmMain.TabGroups.All.Cast(Of MdiTabGroup).SelectMany(Function(grp) grp.Tabs.Cast(Of MdiTab)())
                Dim docForm As DocumentForm = TryCast(drawingMdiTab.Form, DocumentForm)
                If docForm IsNot Nothing Then
                    docs.Add(docForm.Id.ToString, docForm)
                End If
            Next
        End If
        Return docs
    End Function

    Friend Function ShowSchematicsEntities(document As DocumentForm, ParamArray kblSystemIds() As String) As Boolean
        If Me.SchematicsView IsNot Nothing Then
            kblSystemIds = kblSystemIds.Where(Function(id) Not String.IsNullOrEmpty(id)).ToArray
            If document IsNot Nothing AndAlso kblSystemIds.Length > 0 Then
                Dim occurrences As New List(Of IKblOccurrence)

                For Each id As String In kblSystemIds
                    Dim occ As IKblOccurrence = document.KBL.GetOccurrenceObjectUntyped(id)
                    If occ IsNot Nothing Then
                        occurrences.Add(occ)
                    End If
                Next

                If occurrences.Count > 0 Then
                    If occurrences.OfType(Of Connector_occurrence).Any(Function(conn) conn.IsKSL(document.KBL)) Then
                        MessageBoxEx.ShowInfo(MainFormStrings.CantShowKSLConnectorSchematics_Msg)
                        Return False
                    End If

                    Me.ShowSchematicsEntities(occurrences.Select(Function(occ) document.CreateEdbId(occ)).ToArray)
                    Return True
                End If
            Else
                Me.SchematicsView.ActiveEntities.Clear()
            End If
        End If
        Return False
    End Function

    Private Async Sub ShowSchematicsEntities(ParamArray edbIds() As String)
        If edbIds Is Nothing Then
            edbIds = Array.Empty(Of String)
        End If

        edbIds = edbIds.Where(Function(id) Not String.IsNullOrEmpty(id)).ToArray

        If edbIds.Length > 0 Then
            Await _isShowingLock.WaitAsync()
            Try
                If Me.HasSchematicsFeature AndAlso Me.SchematicsView IsNot Nothing Then
                    If Not Me.SchematicsView.Visible OrElse Not (Me.Panes.SchematicsPane?.IsVisible).GetValueOrDefault Then
                        ShowSchematicsView()
                    End If
                    Me.SchematicsView.ActiveEntities.Clear()
                    Me.SchematicsView.ActiveEntities.AddNew(edbIds)
                End If
            Finally
                _isShowingLock.Release()
            End Try
        End If
    End Sub

    Friend Sub SelectSchematicsEntities(ParamArray entitiyIds() As String)
        If Me.SchematicsView IsNot Nothing Then
            Me.SchematicsView.ResetSelectionTo(entitiyIds, False)
        End If
    End Sub

    Friend Sub ShowSchematicsView()
        Static firstShown As Boolean = False

        udmMain.BeginUpdate()
        Me.Panes.SchematicsPane?.Show()

        If (Not firstShown) Then
            firstShown = True

            Dim cX As Integer = CInt(Me.Location.X + (Me.Width) / 2)
            Dim cy As Integer = CInt(Me.Location.Y + (Me.Height) / 2)

            If (My.Settings.AdvConnFloatPosition <> Point.Empty) Then
                cX = My.Settings.AdvConnFloatPosition.X
                cy = My.Settings.AdvConnFloatPosition.Y
            End If

            Me.Panes.SchematicsPane.Float(True, New Rectangle(cX, cy, My.Settings.AdvConnFloatSize.Width, My.Settings.AdvConnFloatSize.Height))
        End If

        udmMain.EndUpdate()
    End Sub


End Class
