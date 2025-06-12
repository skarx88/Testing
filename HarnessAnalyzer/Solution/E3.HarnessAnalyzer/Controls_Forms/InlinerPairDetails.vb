Imports System.Text
Imports devDept.Eyeshot
Imports devDept.Eyeshot.Entities
Imports Infragistics.Win
Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinExplorerBar
Imports Infragistics.Win.UltraWinGrid
Imports VectorDraw.Geometry
Imports VectorDraw.Professional.vdFigures
Imports VectorDraw.Professional.vdObjects
Imports VectorDraw.Professional.vdPrimaries
Imports Zuken.E3.HarnessAnalyzer.D3D.Shared
Imports Zuken.E3.HarnessAnalyzer.Project.Documents
Imports Zuken.E3.HarnessAnalyzer.Settings

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class InlinerPairDetails

    Friend Event InlinerPairDetailsMouseClick(sender As Object, e As InformationHubEventArgs)

    Private _leftCavityNumbers As List(Of String)
    Private _inlinerPair As InlinerPair
    Private _inlinerPairCheckClassifications As InlinerPairCheckClassificationList
    Private _rects As New List(Of vdRect)
    Private _rightCavityNumbers As List(Of String)

    Public Sub New(inlinerPair As InlinerPair, inlinerPairCheckClassifications As InlinerPairCheckClassificationList)
        InitializeComponent()

        DesignLeft.InitDefaults(initObjManipulatorManager:=False, actionMode:=devDept.Eyeshot.actionType.None)
        DesignLeft.Renderer = devDept.Eyeshot.rendererType.OpenGL ' HINT: better performance when lots of Eyeshot-Models are shown on the screen. (and we don't have much geometry to show, no hughe performance impact when rotating, etc.)

        DesignRight.InitDefaults(initObjManipulatorManager:=False, actionMode:=devDept.Eyeshot.actionType.None)
        DesignRight.Renderer = devDept.Eyeshot.rendererType.OpenGL ' HINT: better performance when lots of Eyeshot-Models are shown on the screen. (and we don't have much geometry to show, no hughe performance impact when rotating, etc.)

        _inlinerPair = inlinerPair
        _inlinerPairCheckClassifications = inlinerPairCheckClassifications
    End Sub

    Private Sub AddConnectorFaceToDrawing(connectorId As String, connectorFace As VdSVGGroup)
        Dim clonedConnectorFace As VdSVGGroup = DirectCast(connectorFace.Clone(Me.vDraw.ActiveDocument), VdSVGGroup)
        clonedConnectorFace.Lighting = Lighting.Normal

        Me.vDraw.ActiveDocument.ActiveLayOut.Entities.AddItem(clonedConnectorFace)

        Dim matrix As New Matrix
        matrix.A11 = -matrix.A11
        matrix.TranslateMatrix(-clonedConnectorFace.BoundingBox.Min.x, -clonedConnectorFace.BoundingBox.Min.y, 0)

        clonedConnectorFace.Transformby(matrix)

        Dim rotation As Double = 0

        For Each figure As vdFigure In clonedConnectorFace.ChildGroups
            If (TryCast(figure, VdSVGGroup) IsNot Nothing) Then
                rotation = DirectCast(figure, VdSVGGroup).Rotation

                Exit For
            End If
        Next

        clonedConnectorFace.Rotation = rotation

        Me.vDraw.ActiveDocument.CommandAction.CmdMove(clonedConnectorFace, clonedConnectorFace.BoundingBox.Min, If(Me.vDraw.ActiveDocument.Model.Entities.Count <= 1, New gPoint(0, 0), New gPoint(_rects.Last.BoundingBox.Max.x + _rects.Last.BoundingBox.Width, 0)))

        Dim text As New vdText
        With text
            .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
            .setDocumentDefaults()

            .Height = 8
            .InsertionPoint = clonedConnectorFace.BoundingBox.Min + New gPoint(0, -12)
            .TextString = connectorId
        End With

        Me.vDraw.ActiveDocument.Model.Entities.AddItem(text)

        Dim rect As New vdRect
        With rect
            .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
            .setDocumentDefaults()

            .Height = clonedConnectorFace.BoundingBox.Height + 30
            .InsertionPoint = clonedConnectorFace.BoundingBox.Min + New gPoint(-15, -15)
            .Width = If(clonedConnectorFace.BoundingBox.Width > text.BoundingBox.Width, clonedConnectorFace.BoundingBox.Width + 30, text.BoundingBox.Width + 30)
        End With

        Me.vDraw.ActiveDocument.Model.Entities.AddItem(rect)

        _rects.Add(rect)
    End Sub

    Private Sub AddNewConnector3DEntityFromId(kblSystemId As String, doc As HcvDocument, design As DesignEx, Optional update As Boolean = True)
        If My.Application.MainForm?.Project IsNot Nothing Then
            With design
                .Entities.Clear()

                If doc IsNot Nothing AndAlso doc.IsOpen Then
                    For Each entity As Entity In doc.Entities.GetAsClones(kblSystemId).OfType(Of Entity)
                        entity.Selected = False
                        .Entities.Add(entity)
                    Next
                End If
            End With

            If update Then
                UpdateDesign(design)
            End If
        End If
    End Sub

    Private Sub UpdateDesign(design As DesignEx)
        If design.Created Then
            design.Entities.Regen()
            design.SetViewToInitialView(True, False)
            design.Refresh()
        Else
            Dim createdEvent As DesignEx.ControlCreatedEventHandler = Nothing
            createdEvent = Sub(s As Object, e As EventArgs)
                               RemoveHandler DirectCast(s, DesignEx).ControlCreated, createdEvent
                               If Not CType(s, Control).IsDisposed Then
                                   UpdateDesign(CType(s, DesignEx))
                               End If
                           End Sub
            AddHandler design.ControlCreated, createdEvent
        End If
    End Sub

    Private Sub FillInlinerHousingInformation(inliner As Connector_occurrence, isLeft As Boolean, kblMapper As KblMapper, Optional pinMapping As List(Of Tuple(Of String, String)) = Nothing)
        Dim inlinerHousing As Connector_Housing = kblMapper.GetConnectorHousing(inliner.Part)
        If (inlinerHousing IsNot Nothing) Then
            If (isLeft) Then
                _leftCavityNumbers = New List(Of String)
            Else
                _rightCavityNumbers = New List(Of String)
            End If

            For Each cavity As Cavity_occurrence In inliner.Slots.FirstOrDefault?.Cavities
                Dim cavityNumber As String = If(kblMapper.KBLPartMapper.ContainsKey(cavity.Part), DirectCast(kblMapper.KBLPartMapper(cavity.Part), Cavity).Cavity_number, String.Empty)

                If (cavityNumber <> String.Empty) Then
                    If (isLeft) Then
                        If (Not _leftCavityNumbers.Contains(cavityNumber)) Then _leftCavityNumbers.Add(cavityNumber)
                    Else
                        If (Not _rightCavityNumbers.Contains(cavityNumber)) Then _rightCavityNumbers.Add(cavityNumber)
                    End If
                End If

                If (inliner.Contact_points.Any(Function(c) c.Contacted_cavity = cavity.SystemId)) Then
                    Dim cnt As Integer = inliner.Contact_points.Where(Function(c) c.Contacted_cavity = cavity.SystemId).Count
                    For Each contactPoint As Contact_point In inliner.Contact_points.Where(Function(c) c.Contacted_cavity = cavity.SystemId)
                        Dim terminalPartNumber As String = String.Empty
                        Dim terminalPlating As String = String.Empty

                        If (contactPoint.Associated_parts IsNot Nothing) Then
                            For Each associatedPartId As String In contactPoint.Associated_parts.SplitSpace
                                If (kblMapper.KBLOccurrenceMapper.ContainsKey(associatedPartId)) AndAlso (TypeOf kblMapper.KBLOccurrenceMapper(associatedPartId) Is Special_terminal_occurrence OrElse TypeOf kblMapper.KBLOccurrenceMapper(associatedPartId) Is Terminal_occurrence) Then
                                    Dim terminalPart As General_terminal = If(kblMapper.GetHarness.GetSpecialTerminalOccurrence(associatedPartId) IsNot Nothing, kblMapper.GetGeneralTerminal(kblMapper.GetHarness.GetSpecialTerminalOccurrence(associatedPartId).Part), kblMapper.GetGeneralTerminal(kblMapper.GetHarness.GetTerminalOccurrence(associatedPartId).Part))
                                    If (terminalPart IsNot Nothing) Then
                                        terminalPartNumber = If(terminalPartNumber = String.Empty, terminalPart.Part_number, String.Format("{0}, {1}", terminalPartNumber, terminalPart.Part_number))
                                        terminalPlating = If(terminalPlating = String.Empty, terminalPart.Plating_material, String.Format("{0}, {1}", terminalPlating, terminalPart.Plating_material))
                                    End If
                                End If
                            Next
                        End If

                        If (kblMapper.KBLContactPointWireMapper.ContainsKey(contactPoint.SystemId)) Then
                            For Each wireId As String In kblMapper.KBLContactPointWireMapper(contactPoint.SystemId)
                                If (kblMapper.KBLOccurrenceMapper.ContainsKey(wireId)) Then
                                    Dim wireNumber As String = String.Empty
                                    Dim wireType As String = String.Empty
                                    Dim csa As String = String.Empty
                                    Dim color As String = String.Empty
                                    Dim modules As String = GetAssignedModules(kblMapper, wireId)
                                    Dim signal As String = If(kblMapper.KBLWireNetMapper.ContainsKey(wireId), kblMapper.KBLWireNetMapper(wireId).Signal_name, String.Empty)

                                    If (TypeOf kblMapper.KBLOccurrenceMapper(wireId) Is Core_occurrence) Then
                                        Dim core As Core_occurrence = DirectCast(kblMapper.KBLOccurrenceMapper(wireId), Core_occurrence)
                                        Dim corePart As Core = DirectCast(kblMapper.KBLPartMapper(core.Part), Core)

                                        wireNumber = core.Wire_number
                                        wireType = corePart.Wire_type
                                        csa = If(corePart.Cross_section_area IsNot Nothing, String.Format("{0} {1}", Math.Round(corePart.Cross_section_area.Value_component, 2), kblMapper.KBLUnitMapper(corePart.Cross_section_area.Unit_component).Unit_name), String.Empty)
                                        color = corePart.GetColours
                                    Else
                                        Dim wire As Wire_occurrence = DirectCast(kblMapper.KBLOccurrenceMapper(wireId), Wire_occurrence)
                                        Dim wirePart As General_wire = DirectCast(kblMapper.KBLPartMapper(wire.Part), General_wire)

                                        wireNumber = wire.Wire_number
                                        wireType = wirePart.Wire_type
                                        csa = If(wirePart.Cross_section_area IsNot Nothing, String.Format("{0} {1}", Math.Round(wirePart.Cross_section_area.Value_component, 2), kblMapper.KBLUnitMapper(wirePart.Cross_section_area.Unit_component).Unit_name), String.Empty)
                                        color = wirePart.GetColours
                                    End If

                                    Dim row As UltraDataRow = If(isLeft, Me.udsInlinerPair.Rows.Add, Nothing)
                                    If (row Is Nothing) Then row = GetRow(cavityNumber, inliner, pinMapping)

                                    With row
                                        .SetCellValue(NameOf(InlinerPairDetailsStrings.CavNum_ColumnCaption), cavityNumber)

                                        .SetCellValue(String.Format("{0}CavityId", If(isLeft, "Left", "Right")), contactPoint.Contacted_cavity)
                                        .SetCellValue(String.Format("{0}ConnectorId", If(isLeft, "Left", "Right")), inliner.Id)


                                        .SetCellValue(String.Format("{0}WireId", If(isLeft, "Left", "Right")), wireId)
                                        .SetCellValue(String.Format("{0}Wire", If(isLeft, "Left", "Right")), wireNumber)
                                        .SetCellValue(String.Format("{0}Type", If(isLeft, "Left", "Right")), wireType)
                                        .SetCellValue(String.Format("{0}CSA", If(isLeft, "Left", "Right")), csa)
                                        .SetCellValue(String.Format("{0}Color", If(isLeft, "Left", "Right")), color)
                                        .SetCellValue(String.Format("{0}Signal", If(isLeft, "Left", "Right")), signal)
                                        .SetCellValue(String.Format("{0}Modules", If(isLeft, "Left", "Right")), modules)
                                        .SetCellValue(String.Format("{0}Terminal", If(isLeft, "Left", "Right")), terminalPartNumber)
                                        .SetCellValue(String.Format("{0}TerminalPlating", If(isLeft, "Left", "Right")), terminalPlating)

                                        If (Not isLeft) OrElse (pinMapping IsNot Nothing) Then
                                            If (pinMapping Is Nothing) Then
                                                .Tag = cavityNumber
                                            Else
                                                .Tag = pinMapping.Where(Function(pm) pm.Item1 = String.Format("{0};{1}", inliner.Id, cavityNumber) OrElse pm.Item2 = String.Format("{0};{1}", inliner.Id, cavityNumber)).FirstOrDefault
                                            End If
                                        End If
                                    End With
                                End If
                            Next
                        ElseIf (cnt = 1) Then 'Empty cavity with contact point
                            Dim row As UltraDataRow = If(isLeft, Me.udsInlinerPair.Rows.Add, Nothing)
                            If (row Is Nothing) Then row = GetRow(cavityNumber, inliner, pinMapping)

                            With row
                                .SetCellValue(NameOf(InlinerPairDetailsStrings.CavNum_ColumnCaption), cavityNumber)
                                .SetCellValue(String.Format("{0}CavityId", If(isLeft, "Left", "Right")), cavity.SystemId)
                                .SetCellValue(String.Format("{0}ConnectorId", If(isLeft, "Left", "Right")), inliner.Id)

                                If (Not isLeft) OrElse (pinMapping IsNot Nothing) Then
                                    If (pinMapping Is Nothing) Then
                                        .Tag = cavityNumber
                                    Else
                                        .Tag = pinMapping.Where(Function(pm) pm.Item1 = String.Format("{0};{1}", inliner.Id, cavityNumber) OrElse pm.Item2 = String.Format("{0};{1}", inliner.Id, cavityNumber)).FirstOrDefault
                                    End If
                                End If
                            End With
                        End If
                    Next
                Else 'Empty cavity without contact point
                    Dim row As UltraDataRow = If(isLeft, Me.udsInlinerPair.Rows.Add, Nothing)
                    If (row Is Nothing) Then
                        row = GetRow(cavityNumber, inliner, pinMapping)
                    End If

                    With row
                        .SetCellValue(NameOf(InlinerPairDetailsStrings.CavNum_ColumnCaption), cavityNumber)
                        .SetCellValue(String.Format("{0}CavityId", If(isLeft, "Left", "Right")), cavity.SystemId)
                        .SetCellValue(String.Format("{0}ConnectorId", If(isLeft, "Left", "Right")), inliner.Id)


                        If (Not isLeft) OrElse (pinMapping IsNot Nothing) Then
                            If (pinMapping Is Nothing) Then
                                .Tag = cavityNumber
                            Else
                                .Tag = pinMapping.Where(Function(pm) pm.Item1 = String.Format("{0};{1}", inliner.Id, cavityNumber) OrElse pm.Item2 = String.Format("{0};{1}", inliner.Id, cavityNumber)).FirstOrDefault
                            End If
                        End If
                    End With
                End If
            Next
        End If
    End Sub

    Private Function GetAssignedModules(kblMapper As KblMapper, wireId As String) As String
        Dim assignedModules As New StringBuilder

        If (kblMapper.KBLObjectModuleMapper.ContainsKey(wireId)) Then
            For Each moduleId As String In kblMapper.KBLObjectModuleMapper(wireId)
                Dim assignedModule As [Module] = TryCast(kblMapper.KBLOccurrenceMapper(moduleId), [Module])
                If (assignedModule IsNot Nothing) Then
                    assignedModules.Append(If(assignedModules.Length = 0, assignedModule.Abbreviation, String.Format("/{0}", assignedModule.Abbreviation)))
                End If
            Next
        End If

        Return assignedModules.ToString
    End Function

    Private Function GetRow(cavityNumber As String, inliner As Connector_occurrence, pinMapping As List(Of Tuple(Of String, String))) As UltraDataRow
        Dim row As UltraDataRow = Nothing

        If (pinMapping IsNot Nothing) Then
            Dim mapId As String = String.Format("{0};{1}", inliner.Id, cavityNumber)

            For Each existingRow As UltraDataRow In Me.udsInlinerPair.Rows
                If (existingRow.Tag IsNot Nothing) AndAlso (DirectCast(existingRow.Tag, Tuple(Of String, String)).Item1 = mapId OrElse DirectCast(existingRow.Tag, Tuple(Of String, String)).Item2 = mapId) Then
                    row = existingRow

                    Exit For
                End If
            Next
        Else
            For Each existingRow As UltraDataRow In Me.udsInlinerPair.Rows
                If (existingRow.GetCellValue(NameOf(InlinerPairDetailsStrings.CavNum_ColumnCaption)).ToString = cavityNumber) AndAlso (existingRow.Tag Is Nothing) Then
                    row = existingRow

                    Exit For
                End If
            Next
        End If

        If (row Is Nothing) Then
            row = Me.udsInlinerPair.Rows.Add
        End If

        Return row
    End Function

    Private Sub HighlightCriticalCells(row As UltraGridRow, key As String, color As Color, tooltipText As String)
        row.Cells(String.Format("Left{0}", key)).Appearance.BackColor = color
        row.Cells(String.Format("Right{0}", key)).Appearance.BackColor = color

        Select Case key
            Case "Color"
                tooltipText = String.Format("{0}{1}", tooltipText, InlinerPairDetailsStrings.ColorMismatch_TooltipText)
            Case "CSA"
                tooltipText = String.Format("{0}{1}", tooltipText, InlinerPairDetailsStrings.CSAMismatch_TooltipText)
            Case "Signal"
                tooltipText = String.Format("{0}{1}", tooltipText, InlinerPairDetailsStrings.SignalMismatch_TooltipText)
            Case "TerminalPlating"
                tooltipText = String.Format("{0}{1}", tooltipText, InlinerPairDetailsStrings.TerminalPlatingMismatch_TooltipText)
            Case "Type"
                tooltipText = String.Format("{0}{1}", tooltipText, InlinerPairDetailsStrings.TypeMismatch_TooltipText)
        End Select

        row.Cells(String.Format("Left{0}", key)).ToolTipText = tooltipText
        row.Cells(String.Format("Right{0}", key)).ToolTipText = tooltipText
    End Sub

    Private Sub HighlightErrorsAndWarnings()
        Dim cavities As New SortedDictionary(Of String, List(Of UltraGridRow))
        Dim hasErrors As Boolean = False
        Dim hasWarnings As Boolean = False

        For Each row As UltraGridRow In Me.ugInlinerPair.Rows
            If (Not cavities.ContainsKey(row.Cells(NameOf(InlinerPairDetailsStrings.CavNum_ColumnCaption)).Value.ToString)) Then
                cavities.Add(row.Cells(NameOf(InlinerPairDetailsStrings.CavNum_ColumnCaption)).Value.ToString, New List(Of UltraGridRow))
            End If
            cavities(row.Cells(NameOf(InlinerPairDetailsStrings.CavNum_ColumnCaption)).Value.ToString).Add(row)
        Next

        With Me.ugInlinerPair
            .BeginUpdate()

            For Each cavityContent As List(Of UltraGridRow) In cavities.Values
                Dim leftProperties As New Dictionary(Of String, List(Of String))
                leftProperties.Add("Type", New List(Of String))
                leftProperties.Add("CSA", New List(Of String))
                leftProperties.Add("Color", New List(Of String))
                leftProperties.Add("Signal", New List(Of String))
                leftProperties.Add("TerminalPlating", New List(Of String))

                Dim rightProperties As New Dictionary(Of String, List(Of String))
                rightProperties.Add("Type", New List(Of String))
                rightProperties.Add("CSA", New List(Of String))
                rightProperties.Add("Color", New List(Of String))
                rightProperties.Add("Signal", New List(Of String))
                rightProperties.Add("TerminalPlating", New List(Of String))

                For Each row As UltraGridRow In cavityContent
                    With row
                        Dim leftWireId As String = .Cells("LeftWireId").Value.ToString
                        Dim rightWireId As String = .Cells("RightWireId").Value.ToString

                        If Not (_inlinerPair.InactiveObjectsA Is Nothing OrElse (_inlinerPair.InactiveObjectsA.ContainsValue(KblObjectType.Core_occurrence, leftWireId)) OrElse (_inlinerPair.InactiveObjectsA.ContainsValue(KblObjectType.Wire_occurrence, leftWireId))) Then
                            If (.Cells("LeftType").Value.ToString <> String.Empty) AndAlso (Not leftProperties("Type").Contains(.Cells("LeftType").Value.ToString.ToLower)) Then
                                leftProperties("Type").Add(.Cells("LeftType").Value.ToString.ToLower)
                            End If
                            If (.Cells("LeftCSA").Value.ToString <> String.Empty) AndAlso (Not leftProperties("CSA").Contains(.Cells("LeftCSA").Value.ToString.ToLower)) Then
                                leftProperties("CSA").Add(.Cells("LeftCSA").Value.ToString.ToLower)
                            End If
                            If (.Cells("LeftColor").Value.ToString <> String.Empty) AndAlso (Not leftProperties("Color").Contains(.Cells("LeftColor").Value.ToString.ToLower)) Then
                                leftProperties("Color").Add(.Cells("LeftColor").Value.ToString.ToLower)
                            End If
                            If (.Cells("LeftSignal").Value.ToString <> String.Empty) AndAlso (Not leftProperties("Signal").Contains(.Cells("LeftSignal").Value.ToString.ToLower)) Then
                                leftProperties("Signal").Add(.Cells("LeftSignal").Value.ToString.ToLower)
                            End If
                            If (.Cells("LeftTerminalPlating").Value.ToString <> String.Empty) AndAlso (Not leftProperties("TerminalPlating").Contains(.Cells("LeftTerminalPlating").Value.ToString.ToLower)) Then
                                leftProperties("TerminalPlating").Add(.Cells("LeftTerminalPlating").Value.ToString.ToLower)
                            End If
                        Else
                            Me.ugInlinerPair.Rows.Move(row, cavityContent.Max(Function(r) r.Index))
                        End If

                        If Not (_inlinerPair.InactiveObjectsB Is Nothing OrElse (_inlinerPair.InactiveObjectsB.ContainsValue(KblObjectType.Core_occurrence, rightWireId)) OrElse (_inlinerPair.InactiveObjectsB.ContainsValue(KblObjectType.Wire_occurrence, rightWireId))) Then
                            If (.Cells("RightType").Value.ToString <> String.Empty) AndAlso (Not rightProperties("Type").Contains(.Cells("RightType").Value.ToString.ToLower)) Then
                                rightProperties("Type").Add(.Cells("RightType").Value.ToString.ToLower)
                            End If
                            If (.Cells("RightCSA").Value.ToString <> String.Empty) AndAlso (Not rightProperties("CSA").Contains(.Cells("RightCSA").Value.ToString.ToLower)) Then
                                rightProperties("CSA").Add(.Cells("RightCSA").Value.ToString.ToLower)
                            End If
                            If (.Cells("RightColor").Value.ToString <> String.Empty) AndAlso (Not rightProperties("Color").Contains(.Cells("RightColor").Value.ToString.ToLower)) Then
                                rightProperties("Color").Add(.Cells("RightColor").Value.ToString.ToLower)
                            End If
                            If (.Cells("RightSignal").Value.ToString <> String.Empty) AndAlso (Not rightProperties("Signal").Contains(.Cells("RightSignal").Value.ToString.ToLower)) Then
                                rightProperties("Signal").Add(.Cells("RightSignal").Value.ToString.ToLower)
                            End If
                            If (.Cells("RightTerminalPlating").Value.ToString <> String.Empty) AndAlso (Not rightProperties("TerminalPlating").Contains(.Cells("RightTerminalPlating").Value.ToString.ToLower)) Then
                                rightProperties("TerminalPlating").Add(.Cells("RightTerminalPlating").Value.ToString.ToLower)
                            End If
                        Else
                            Me.ugInlinerPair.Rows.Move(row, cavityContent.Max(Function(r) r.Index))
                        End If
                    End With
                Next

                Dim hasErrs As Boolean = False
                Dim hasWrngs As Boolean = False

                For Each inlinerPairCheckClassification As InlinerPairCheckClassification In _inlinerPairCheckClassifications
                    If (leftProperties.ContainsKey(inlinerPairCheckClassification.PropertyName)) AndAlso (rightProperties.ContainsKey(inlinerPairCheckClassification.PropertyName)) AndAlso ((leftProperties(inlinerPairCheckClassification.PropertyName).Count <> rightProperties(inlinerPairCheckClassification.PropertyName).Count) OrElse (Not leftProperties(inlinerPairCheckClassification.PropertyName).TrueForAll(Function(prop) rightProperties(inlinerPairCheckClassification.PropertyName).Contains(prop)))) Then
                        If (inlinerPairCheckClassification.Classification = "Error") Then
                            cavityContent.FirstOrDefault.Cells(NameOf(InlinerPairDetailsStrings.CavNum_ColumnCaption)).Appearance.Image = My.Resources.Error_Small

                            For Each row As UltraGridRow In cavityContent
                                HighlightCriticalCells(row, inlinerPairCheckClassification.PropertyName, Color.FromArgb(100, Color.Red), InlinerPairDetailsStrings.Error_TooltipText)
                            Next

                            hasErrs = True
                        ElseIf (inlinerPairCheckClassification.Classification = "Warning") Then
                            If (Not hasErrs) Then
                                cavityContent.FirstOrDefault.Cells(NameOf(InlinerPairDetailsStrings.CavNum_ColumnCaption)).Appearance.Image = My.Resources.MismatchingConfig_Small
                            End If

                            For Each row As UltraGridRow In cavityContent
                                HighlightCriticalCells(row, inlinerPairCheckClassification.PropertyName, Color.FromArgb(100, Color.Yellow), InlinerPairDetailsStrings.Warning_TooltipText)
                            Next

                            hasWrngs = True
                        End If
                    End If
                Next

                If (hasErrs) Then hasErrors = True
                If (hasWrngs) Then hasWarnings = True
            Next

            If (hasErrors) Then
                DirectCast(Me.Parent.Parent, UltraExplorerBar).Groups(_inlinerPair.Id).Settings.AppearancesSmall.HeaderAppearance.Image = My.Resources.StatusRed
            ElseIf (hasWarnings) Then
                DirectCast(Me.Parent.Parent, UltraExplorerBar).Groups(_inlinerPair.Id).Settings.AppearancesSmall.HeaderAppearance.Image = My.Resources.StatusYellow
            End If

            For Each cavGrp As IGrouping(Of String, UltraGridRow) In ugInlinerPair.Rows.GroupBy(Of String)(Function(r) r.GetCellValue(NameOf(InlinerPairDetailsStrings.CavNum_ColumnCaption)).ToString)
                If cavGrp.Count > 1 Then
                    Dim leftConn As String = cavGrp.Select(Of String)(Function(r) r.GetCellValue("LeftConnectorId").ToString).Where(Function(rx) Not String.IsNullOrEmpty(rx)).FirstOrDefault
                    Dim rightConn As String = cavGrp.Select(Of String)(Function(r) r.GetCellValue("RightConnectorId").ToString).Where(Function(rx) Not String.IsNullOrEmpty(rx)).FirstOrDefault
                    For Each entr As UltraGridRow In cavGrp
                        If String.IsNullOrEmpty(entr.GetCellValue("LeftConnectorId").ToString) Then
                            entr.Cells("LeftConnectorId").Value = leftConn
                        End If
                        If String.IsNullOrEmpty(entr.GetCellValue("RightConnectorId").ToString) Then
                            entr.Cells("RightConnectorId").Value = rightConn
                        End If
                    Next
                End If
            Next
            .EndUpdate()
        End With
    End Sub

    Public Sub Initialize()
        InitializeDrawing()
        InitializeGrid()
    End Sub

    Private Sub InitializeDrawing()
        If _inlinerPair.ConnectorFacesA.All(Function(cf) cf.Value Is Nothing) AndAlso _inlinerPair.ConnectorFacesB.All(Function(cf) cf.Value Is Nothing) Then
            If Not Has3DDocuments Then
                Me.ugInlinerPair.Size = New System.Drawing.Size(400, 400)
                Exit Sub
            Else
                Me.UltraTabPageControl2D.Tab.Visible = False
            End If
        Else
            Me.UltraTabPageControl3D.Tab.Visible = Has3DDocuments
        End If

        Me.vDraw.AllowDrop = False
        Me.vDraw.EnsureDocument()
        Me.vDraw.InputDevice_TouchGesture.Actions = VectorDraw.Professional.Actions.TouchGestureActions.Pan Or VectorDraw.Professional.Actions.TouchGestureActions.Zoom

        With Me.vDraw.ActiveDocument
            .UndoHistory.PushEnable(False)

            .EnableAutoFocus = True
            .EnableAutoGripOn = False
            .EnableUrls = False

            With .GlobalRenderProperties
                .AxisSize = 10
                .CrossSize = 8
                .GridColor = Color.Black
                .SelectingCrossColor = Color.Transparent
                .SelectingWindowColor = Color.Transparent
            End With

            .GridMode = False
            .OrbitActionKey = vdDocument.KeyWithMouseStroke.None
            .OsnapDialogKey = Keys.None
            .Palette.Background = Color.White
            .ShowUCSAxis = False
            .UrlActionKey = Keys.None
        End With

        For Each cf As KeyValuePair(Of DictionaryKblKey, VdSVGGroup) In _inlinerPair.ConnectorFacesA.Concat(_inlinerPair.ConnectorFacesB)
            If cf.Value IsNot Nothing Then
                AddConnectorFaceToDrawing(cf.Key.KblId, cf.Value)
            End If
        Next
        Me.vDraw.ActiveDocument.Update()
        Me.vDraw.ActiveDocument.ZoomExtents()
        Me.vDraw.ActiveDocument.Invalidate()

        For Each cf As KeyValuePair(Of DictionaryKblKey, VdSVGGroup) In _inlinerPair.ConnectorFacesA ' left side
            AddNewConnector3DEntityFromId(cf.Key.SystemId, _inlinerPair.DocumentA, Me.DesignLeft, False)
        Next

        For Each cf As KeyValuePair(Of DictionaryKblKey, VdSVGGroup) In _inlinerPair.ConnectorFacesB ' right side
            AddNewConnector3DEntityFromId(cf.Key.SystemId, _inlinerPair.DocumentB, Me.DesignRight, False)
        Next

        UpdateDesign(Me.DesignLeft)
        UpdateDesign(Me.DesignRight)
    End Sub

    Private ReadOnly Property Has3DDocuments As Boolean
        Get
            If My.Application.MainForm?.Project IsNot Nothing Then
                Return My.Application.MainForm.HasView3DFeature AndAlso My.Application.MainForm.Project.Documents.Cast(Of HcvDocument).Any(Function(doc) doc.KblZDataAvailable)
            End If
            Return False
        End Get
    End Property

    Private Sub InitializeGrid()
        Me.ugInlinerPair.SyncWithCurrencyManager = False

        With Me.udsInlinerPair
            .Band.Key = "InlinerPair"

            With .Band
                .Columns.Add(NameOf(InlinerPairDetailsStrings.CavNum_ColumnCaption))
                .Columns.Add("SeparatorColumn1")
                .Columns.Add("LeftCavityId")
                .Columns.Add("LeftConnectorId")
                .Columns.Add("LeftWireId")
                .Columns.Add("LeftWire")
                .Columns.Add("LeftType")
                .Columns.Add("LeftCSA")
                .Columns.Add("LeftColor")
                .Columns.Add("LeftSignal")
                .Columns.Add("LeftModules")
                .Columns.Add("LeftTerminal")
                .Columns.Add("LeftTerminalPlating")
                .Columns.Add("SeparatorColumn2")
                .Columns.Add("RightCavityId")
                .Columns.Add("RightConnectorId")
                .Columns.Add("RightWireId")
                .Columns.Add("RightWire")
                .Columns.Add("RightType")
                .Columns.Add("RightCSA")
                .Columns.Add("RightColor")
                .Columns.Add("RightSignal")
                .Columns.Add("RightModules")
                .Columns.Add("RightTerminal")
                .Columns.Add("RightTerminalPlating")
            End With
        End With

        InitializeGridData()
    End Sub

    Private Sub InitializeGridData()
        Me.udsInlinerPair.Rows.Clear()

        _leftCavityNumbers = New List(Of String)
        _rightCavityNumbers = New List(Of String)

        If (TypeOf _inlinerPair Is TrivialInlinerPair) Then
            Dim inlPair As TrivialInlinerPair = DirectCast(_inlinerPair, TrivialInlinerPair)
            If (inlPair.ConnectorOccA.Slots IsNot Nothing AndAlso inlPair.ConnectorOccA.Slots.Length <> 0) Then
                FillInlinerHousingInformation(inlPair.ConnectorOccA, True, inlPair.KblMapperA)
            End If

            If (inlPair.ConnectorOccB.Slots IsNot Nothing AndAlso inlPair.ConnectorOccB.Slots.Length <> 0) Then
                FillInlinerHousingInformation(inlPair.ConnectorOccB, False, inlPair.KblMapperB)
            End If
        Else
            Dim inlPair As VirtualInlinerPair = DirectCast(_inlinerPair, VirtualInlinerPair)

            For Each connOccWithKblMapperA As KeyValuePair(Of Connector_occurrence, KblMapper) In inlPair.ActiveConnectorOccsWithKblMapperA
                If (connOccWithKblMapperA.Key.Slots IsNot Nothing AndAlso connOccWithKblMapperA.Key.Slots.Length <> 0) Then
                    FillInlinerHousingInformation(connOccWithKblMapperA.Key, True, connOccWithKblMapperA.Value, inlPair.PinMap)
                End If
            Next

            For Each connOccWithKblMapperB As KeyValuePair(Of Connector_occurrence, KblMapper) In inlPair.ActiveConnectorOccsWithKblMapperB
                If (connOccWithKblMapperB.Key.Slots IsNot Nothing AndAlso connOccWithKblMapperB.Key.Slots.Length <> 0) Then
                    FillInlinerHousingInformation(connOccWithKblMapperB.Key, False, connOccWithKblMapperB.Value, inlPair.PinMap)
                End If
            Next
        End If

        For Each cavityNumber As String In _leftCavityNumbers
            If (Not _rightCavityNumbers.Contains(cavityNumber)) Then
                For Each existingRow As UltraDataRow In Me.udsInlinerPair.Rows
                    If (existingRow.GetCellValue(NameOf(InlinerPairDetailsStrings.CavNum_ColumnCaption)).ToString = cavityNumber) Then
                        existingRow.SetCellValue("RightWireId", "<MISSING>")
                    End If
                Next
            End If
        Next

        For Each cavityNumber As String In _rightCavityNumbers
            If (Not _leftCavityNumbers.Contains(cavityNumber)) Then
                For Each existingRow As UltraDataRow In Me.udsInlinerPair.Rows
                    If (existingRow.GetCellValue(NameOf(InlinerPairDetailsStrings.CavNum_ColumnCaption)).ToString = cavityNumber) Then
                        existingRow.SetCellValue("LeftWireId", "<MISSING>")
                    End If
                Next
            End If
        Next

        Me.ugInlinerPair.DataSource = Me.udsInlinerPair
    End Sub

    Private Sub InlinerPairDetails_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.ugInlinerPair.DrawFilter = New VerticalTextOrientationDrawFilter

        HighlightErrorsAndWarnings()
    End Sub

    Private Sub ugInlinerPair_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugInlinerPair.InitializeLayout
        Me.ugInlinerPair.BeginUpdate()

        With e.Layout
            .AutoFitStyle = AutoFitStyle.ResizeAllColumns
            .CaptionVisible = Infragistics.Win.DefaultableBoolean.False
            .GroupByBox.Hidden = True
            .LoadStyle = LoadStyle.LoadOnDemand

            With .Override
                .AllowColMoving = AllowColMoving.NotAllowed
                .AllowGroupMoving = AllowGroupMoving.NotAllowed
                .CellClickAction = CellClickAction.RowSelect
                .RowSelectors = Infragistics.Win.DefaultableBoolean.False
                .SelectTypeRow = SelectType.SingleAutoDrag
            End With

            For Each band As UltraGridBand In .Bands
                With band
                    Dim cavGroup As UltraGridGroup = .Groups.Add("Cavity", String.Empty)
                    cavGroup.Header.Appearance.Image = My.Resources.InlinerPair

                    Dim t_inlinerPair As TrivialInlinerPair = TryCast(_inlinerPair, TrivialInlinerPair)
                    Dim leftGroup As UltraGridGroup = .Groups.Add("Left", String.Format("{0} [{1}]", InlinerPairDetailsStrings.LeftInlHousing_GroupCaption, If(t_inlinerPair?.InlinerIdA, TryCast(_inlinerPair, VirtualInlinerPair)?.InlinerIdA)))
                    Dim rightGroup As UltraGridGroup = .Groups.Add("Right", String.Format("{0} [{1}]", InlinerPairDetailsStrings.RightInlHousing_GroupCaption, If(t_inlinerPair?.InlinerIdB, TryCast(_inlinerPair, VirtualInlinerPair)?.InlinerIdB)))

                    For Each column As UltraGridColumn In .Columns
                        Select Case column.Key
                            Case "LeftCavityId", "LeftWireId", "RightCavityId", "RightWireId"
                                column.Hidden = True
                            Case "LeftConnectorId", "RightConnectorId"
                                If (TypeOf _inlinerPair Is VirtualInlinerPair) Then
                                    column.Header.Caption = InlinerPairDetailsStrings.Connector_ColumnCaption
                                    column.MaxWidth = 40
                                    column.MinWidth = 40
                                Else
                                    column.Hidden = True
                                End If
                            Case "LeftWire", "RightWire"
                                column.Header.Caption = InlinerPairDetailsStrings.Wire_ColumnCaption
                                column.MinWidth = 50
                            Case "LeftType", "RightType"
                                column.Header.Caption = InlinerPairDetailsStrings.Type_ColumnCaption
                            Case "LeftCSA", "RightCSA"
                                column.Header.Caption = InlinerPairDetailsStrings.CSA_ColumnCaption
                            Case "LeftColor", "RightColor"
                                column.Header.Caption = InlinerPairDetailsStrings.Color_ColumnCaption
                            Case "LeftSignal", "RightSignal"
                                column.Header.Caption = InlinerPairDetailsStrings.Signal_ColumnCaption
                            Case "LeftModules", "RightModules"
                                column.Header.Caption = InlinerPairDetailsStrings.Modules_ColumnCaption
                            Case "LeftTerminal", "RightTerminal"
                                column.Header.Caption = InlinerPairDetailsStrings.Terminal_ColumnCaption
                            Case "LeftTerminalPlating", "RightTerminalPlating"
                                column.Header.Caption = InlinerPairDetailsStrings.Plating_ColumnCaption
                                column.MinWidth = 50
                            Case "SeparatorColumn1", "SeparatorColumn2"
                                column.Header.Caption = String.Empty
                        End Select

                        If (column.Key.StartsWith("Left")) OrElse (column.Key = "SeparatorColumn1") Then
                            If (column.Key.EndsWith("ConnectorId")) Then
                                column.MergedCellEvaluationType = MergedCellEvaluationType.MergeSameText
                                column.MergedCellStyle = MergedCellStyle.Always
                                column.MergedCellAppearance.TextVAlign = VAlign.Middle
                            End If

                            leftGroup.Columns.Add(column)
                        ElseIf (column.Key.StartsWith("Right")) OrElse (column.Key = "SeparatorColumn2") Then
                            If (column.Key.EndsWith("ConnectorId")) Then
                                column.MergedCellEvaluationType = MergedCellEvaluationType.MergeSameText
                                column.MergedCellStyle = MergedCellStyle.Always
                                column.MergedCellAppearance.TextVAlign = VAlign.Middle
                            End If

                            rightGroup.Columns.Add(column)
                        Else
                            column.MergedCellEvaluationType = MergedCellEvaluationType.MergeSameText
                            column.MergedCellStyle = MergedCellStyle.Always
                            column.MergedCellAppearance.TextVAlign = VAlign.Middle
                            column.SortComparer = New NumericStringSortComparer
                            column.SortIndicator = SortIndicator.Ascending

                            cavGroup.Columns.Add(column)
                        End If
                    Next

                    .PerformAutoResizeColumns(False, PerformAutoSizeType.VisibleRows)

                    With .Columns("SeparatorColumn1")
                        .MaxWidth = 5
                        .MinWidth = 5
                        .Width = 5
                    End With

                    With .Columns("SeparatorColumn2")
                        .MaxWidth = 5
                        .MinWidth = 5
                        .Width = 5
                    End With
                End With
            Next
        End With

        Me.ugInlinerPair.EndUpdate()
    End Sub

    Private Sub ugInlinerPair_InitializeRow(sender As Object, e As InitializeRowEventArgs) Handles ugInlinerPair.InitializeRow
        Dim leftWireId As String = e.Row.Cells("LeftWireId").Value.ToString
        Dim leftWire As String = e.Row.Cells("LeftWire").Value.ToString
        Dim rightWireId As String = e.Row.Cells("RightWireId").Value.ToString
        Dim rightWire As String = e.Row.Cells("RightWire").Value.ToString
        Dim ultraExplBar As UltraExplorerBar = DirectCast(Me.Parent.Parent, UltraExplorerBar)

        If (leftWireId = "<MISSING>") AndAlso (leftWire = String.Empty) Then
            e.Row.Cells(NameOf(InlinerPairDetailsStrings.CavNum_ColumnCaption)).Appearance.Image = My.Resources.Error_Small

            For Each cell As UltraGridCell In e.Row.Cells
                If (cell.Column.Key.StartsWith("Left")) AndAlso (Not cell.Column.Key.EndsWith("ConnectorId")) Then
                    cell.Appearance.BackColor = Color.Gray
                    cell.ToolTipText = InlinerPairDetailsStrings.CavityWireCountMismatch_TooltipText
                    cell.Value = String.Empty
                End If
            Next

            ultraExplBar.Groups(_inlinerPair.Id).Settings.AppearancesSmall.HeaderAppearance.Image = My.Resources.StatusRed
        ElseIf (rightWireId = "<MISSING>") AndAlso (rightWire = String.Empty) Then
            e.Row.Cells(NameOf(InlinerPairDetailsStrings.CavNum_ColumnCaption)).Appearance.Image = My.Resources.Error_Small

            For Each cell As UltraGridCell In e.Row.Cells
                If (cell.Column.Key.StartsWith("Right")) AndAlso (Not cell.Column.Key.EndsWith("ConnectorId")) Then
                    cell.Appearance.BackColor = Color.Gray
                    cell.ToolTipText = InlinerPairDetailsStrings.CavityWireCountMismatch_TooltipText
                    cell.Value = String.Empty
                End If
            Next

            ultraExplBar.Groups(_inlinerPair.Id).Settings.AppearancesSmall.HeaderAppearance.Image = My.Resources.StatusRed
        End If

        If (leftWireId <> String.Empty) Then
            If ((_inlinerPair.InactiveObjectsA.ContainsValue(KblObjectType.Core_occurrence, leftWireId)) OrElse (_inlinerPair.InactiveObjectsA.ContainsValue(KblObjectType.Wire_occurrence, leftWireId))) Then
                With e.Row
                    .Cells("LeftWire").Appearance.ForeColor = Color.Gray
                    .Cells("LeftType").Appearance.ForeColor = Color.Gray
                    .Cells("LeftCSA").Appearance.ForeColor = Color.Gray
                    .Cells("LeftColor").Appearance.ForeColor = Color.Gray
                    .Cells("LeftSignal").Appearance.ForeColor = Color.Gray
                    .Cells("LeftTerminal").Appearance.ForeColor = Color.Gray
                    .Cells("LeftTerminalPlating").Appearance.ForeColor = Color.Gray
                End With
            End If
        End If

        If (rightWireId <> String.Empty) Then
            If (_inlinerPair.InactiveObjectsB.ContainsValue(KblObjectType.Core_occurrence, rightWireId)) OrElse (_inlinerPair.InactiveObjectsB.ContainsValue(KblObjectType.Wire_occurrence, rightWireId)) Then
                With e.Row
                    .Cells("RightWire").Appearance.ForeColor = Color.Gray
                    .Cells("RightType").Appearance.ForeColor = Color.Gray
                    .Cells("RightCSA").Appearance.ForeColor = Color.Gray
                    .Cells("RightColor").Appearance.ForeColor = Color.Gray
                    .Cells("RightSignal").Appearance.ForeColor = Color.Gray
                    .Cells("RightTerminal").Appearance.ForeColor = Color.Gray
                    .Cells("RightTerminalPlating").Appearance.ForeColor = Color.Gray
                End With
            End If
        End If

        e.Row.Cells("SeparatorColumn1").Appearance.BackColor = Color.Black
        e.Row.Cells("SeparatorColumn2").Appearance.BackColor = Color.Black
    End Sub

    Private Sub ugInlinerPair_MouseClick(sender As Object, e As MouseEventArgs) Handles ugInlinerPair.MouseClick
        If (e.Button = System.Windows.Forms.MouseButtons.Left) Then
            Dim element As UIElement = Me.ugInlinerPair.DisplayLayout.UIElement.LastElementEntered
            Dim cell As UltraGridCell = TryCast(element.GetContext(GetType(UltraGridCell)), UltraGridCell)

            If (cell IsNot Nothing) AndAlso (TypeOf _inlinerPair Is TrivialInlinerPair) Then
                Dim inlPair As TrivialInlinerPair = DirectCast(_inlinerPair, TrivialInlinerPair)

                If (cell.Column.Key.StartsWith("Left")) Then
                    Dim leftCavityId As String = cell.Row.Cells("LeftCavityId").Value.ToString
                    If (inlPair.KblMapperA.KBLOccurrenceMapper.ContainsKey(leftCavityId)) Then
                        RaiseEvent InlinerPairDetailsMouseClick(inlPair.KblMapperA.HarnessPartNumber, New InformationHubEventArgs(inlPair.KblMapperA.Id, KblObjectType.Cavity_occurrence, leftCavityId))
                    Else
                        RaiseEvent InlinerPairDetailsMouseClick(inlPair.KblMapperA.HarnessPartNumber, New InformationHubEventArgs(inlPair.KblMapperA.Id, KblObjectType.Connector_occurrence, inlPair.ConnectorOccA.SystemId))
                    End If
                Else
                    Dim rightCavityId As String = cell.Row.Cells("RightCavityId").Value.ToString
                    If inlPair.KblMapperB.KBLOccurrenceMapper.ContainsKey(rightCavityId) Then
                        RaiseEvent InlinerPairDetailsMouseClick(inlPair.KblMapperB.HarnessPartNumber, New InformationHubEventArgs(inlPair.KblMapperB.Id, KblObjectType.Cavity_occurrence, rightCavityId))
                    Else
                        RaiseEvent InlinerPairDetailsMouseClick(inlPair.KblMapperB.HarnessPartNumber, New InformationHubEventArgs(inlPair.KblMapperB.Id, KblObjectType.Connector_occurrence, inlPair.ConnectorOccB.SystemId))
                    End If
                End If
            ElseIf (cell IsNot Nothing) AndAlso (TypeOf _inlinerPair Is VirtualInlinerPair) Then
                Dim inlPair As VirtualInlinerPair = DirectCast(_inlinerPair, VirtualInlinerPair)

                If (cell.Column.Key.StartsWith("Left")) Then
                    Dim leftCavityId As String = cell.Row.Cells("LeftCavityId").Value.ToString
                    If (inlPair.ConnectorOccsWithKblMapperA.Any(Function(c) c.Value.KBLOccurrenceMapper.ContainsKey(leftCavityId))) Then
                        Dim kblMapper As KblMapper = inlPair.ConnectorOccsWithKblMapperA.Where(Function(c) c.Value.KBLOccurrenceMapper.ContainsKey(leftCavityId)).FirstOrDefault.Value

                        RaiseEvent InlinerPairDetailsMouseClick(kblMapper.HarnessPartNumber, New InformationHubEventArgs(kblMapper.Id, KblObjectType.Cavity_occurrence, leftCavityId))
                    Else
                        Dim leftConnectorId As String = cell.Row.Cells("LeftConnectorId").Value.ToString
                        Dim kblMapper As KblMapper = inlPair.ConnectorOccsWithKblMapperA.Where(Function(c) c.Value.KBLIdMapper.ContainsKey(leftConnectorId)).FirstOrDefault.Value
                        If (kblMapper IsNot Nothing) Then
                            RaiseEvent InlinerPairDetailsMouseClick(kblMapper.HarnessPartNumber, New InformationHubEventArgs(kblMapper.Id, KblObjectType.Connector_occurrence, kblMapper.KBLIdMapper(leftConnectorId).FirstOrDefault))
                        End If
                    End If
                Else
                    Dim rightCavityId As String = cell.Row.Cells("RightCavityId").Value.ToString
                    If (inlPair.ConnectorOccsWithKblMapperB.Any(Function(c) c.Value.KBLOccurrenceMapper.ContainsKey(rightCavityId))) Then
                        Dim kblMapper As KblMapper = inlPair.ConnectorOccsWithKblMapperB.Where(Function(c) c.Value.KBLOccurrenceMapper.ContainsKey(rightCavityId)).FirstOrDefault.Value
                        RaiseEvent InlinerPairDetailsMouseClick(kblMapper.HarnessPartNumber, New InformationHubEventArgs(kblMapper.Id, KblObjectType.Cavity_occurrence, rightCavityId))
                    Else
                        Dim rightConnectorId As String = cell.Row.Cells("RightConnectorId").Value.ToString
                        Dim kblMapper As KblMapper = inlPair.ConnectorOccsWithKblMapperB.Where(Function(c) c.Value.KBLIdMapper.ContainsKey(rightConnectorId)).FirstOrDefault.Value
                        If (kblMapper IsNot Nothing) Then
                            RaiseEvent InlinerPairDetailsMouseClick(kblMapper.HarnessPartNumber, New InformationHubEventArgs(kblMapper.Id, KblObjectType.Connector_occurrence, kblMapper.KBLIdMapper(rightConnectorId).FirstOrDefault))
                        End If
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub CtrlPaint_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        vDraw.Invalidate() 'Hack because the backbuffer of VectorDraw does not update itself
    End Sub

    Private Sub Model1_HandleCreated(sender As Object, e As EventArgs) Handles DesignLeft.HandleCreated, DesignRight.HandleCreated
        DirectCast(sender, devDept.Eyeshot.Design).ZoomFit()
        DirectCast(sender, devDept.Eyeshot.Design).Invalidate()
    End Sub

    Private Class VerticalTextOrientationDrawFilter
        Implements IUIElementDrawFilter

        Private Function IUIElementDrawFilter_GetPhasesToFilter(ByRef drawParams As UIElementDrawParams) As DrawPhase Implements IUIElementDrawFilter.GetPhasesToFilter
            If (TypeOf drawParams.Element Is EditorWithTextDisplayTextUIElement) Then
                Return DrawPhase.BeforeDrawForeground
            End If

            Return DrawPhase.None
        End Function

        Private Function IUIElementDrawFilter_DrawElement(drawPhase As DrawPhase, ByRef drawParams As UIElementDrawParams) As Boolean Implements IUIElementDrawFilter.DrawElement
            Dim textElement As EditorWithTextDisplayTextUIElement = TryCast(drawParams.Element, EditorWithTextDisplayTextUIElement)
            Dim mergedCell As MergedCellUIElement = TryCast(textElement?.Parent?.Parent, MergedCellUIElement)
            If mergedCell IsNot Nothing AndAlso mergedCell.Cell.Column.Key.EndsWith("ConnectorId") Then
                Dim sf As New StringFormat(StringFormatFlags.DirectionVertical)
                sf.Alignment = StringAlignment.Center
                sf.LineAlignment = StringAlignment.Center

                drawParams.Graphics.DrawString(textElement.Text, textElement.Control.Font, Brushes.Black, textElement.Rect, sf)

                Return True
            End If

            Return False
        End Function

    End Class

End Class