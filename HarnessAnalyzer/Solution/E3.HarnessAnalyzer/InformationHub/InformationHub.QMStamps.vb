Imports Infragistics.Win
Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinGrid

Imports Zuken.E3.HarnessAnalyzer.QualityStamping
Imports Zuken.E3.HarnessAnalyzer.Settings.QualityStamping.Specification

Partial Public Class InformationHub

    Private Const SPEC_POSTFIX_COLUMN_KEY As String = "Specification postfix"

    Friend Sub AddOrEditQMStamp()
        If (Not TypeOf _parentForm Is DocumentForm) OrElse (DirectCast(_parentForm, DocumentForm).ActiveDrawingCanvas Is Nothing) Then
            Exit Sub
        End If

        Dim selectedGridRows As New List(Of UltraGridRow)

        For Each grid As KeyValuePair(Of String, UltraGrid) In _grids
            For Each row As UltraGridRow In grid.Value.Selected.Rows
                selectedGridRows.Add(row)
            Next
        Next

        Dim objectReferences As List(Of ObjectReference) = GetObjectReferences(selectedGridRows)

        If (selectedGridRows.Count = 0) OrElse (objectReferences.Count = 0) Then
            For Each group As VdSVGGroup In DirectCast(_parentForm, DocumentForm).ActiveDrawingCanvas.CanvasSelection
                Dim svgSymbolType As SvgSymbolType = [Enum].Parse(Of SvgSymbolType)(group.SymbolType)
                Dim svgSymblAsKblObjectType As KblObjectType = svgSymbolType.AsKblObjectType
                objectReferences.Add(New ObjectReference With {.KblId = group.KblId, .ObjectName = group.KblId, .ObjectType = svgSymblAsKblObjectType})
            Next
        End If

        If (objectReferences.Count = 0) Then
            Dim objRef As New ObjectReference With {.KblId = _kblMapper.GetHarness.SystemId, .ObjectName = _kblMapper.HarnessPartNumber, .ObjectType = E3.Lib.Schema.Kbl.KblObjectType.Harness}
            objectReferences.Add(objRef)

            Dim scaleFactor As Double = DirectCast(_parentForm, DocumentForm).ActiveDrawingCanvas.GetRedliningAndStampScaleFactor
            Using tempStamp As New VdStamp(objRef)
                tempStamp.SetUnRegisterDocument(DirectCast(_parentForm, DocumentForm).ActiveDrawingCanvas.vdCanvas.BaseControl.ActiveDocument)
                tempStamp.setDocumentDefaults()

                tempStamp.Scales.x = scaleFactor
                tempStamp.Scales.y = scaleFactor

                Dim action As VdMoveStampAction = VdMoveStampAction.CmdMoveStamp(tempStamp)
                If (action Is Nothing) Then
                    Return
                End If
            End Using
        End If

        Dim stamp As QMStamp = _qmStamps.Stamps.AddNew()
        stamp.ObjectReferences.AddRange(objectReferences)

        If (Me.utcInformationHub.ActiveTab IsNot Nothing) Then
            ClearSelectedRowsInGrid(DirectCast(Me.utcInformationHub.ActiveTab.TabPage.Controls(0), UltraGrid))
        End If

        If (Me.utcInformationHub.Tabs(TabNames.tabQMStamps.ToString).Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True) Then
            ClearMarkedRowsInGrids()
        End If

        InitializeQMStamps()

        Me.ugQMStamps.UpdateData()

        BringTabIntoViewCore(TabNames.tabQMStamps.ToString, False)

        _informationHubEventArgs.ObjectIds = New HashSet(Of String)

        For Each objRef As ObjectReference In objectReferences
            _informationHubEventArgs.ObjectIds.Add(objRef.KblId)
            _informationHubEventArgs.ObjectType = objRef.ObjectType
        Next

        _informationHubEventArgs.StampIds = New List(Of String)
        _informationHubEventArgs.StampIds.Add(stamp.Id)
        _informationHubEventArgs.RemovePreviousSelection = True

        RaiseEvent QMStampsChanged(Me, _informationHubEventArgs)

        SelectKblIdRowsInGrid(Me.ugQMStamps, {stamp.Id}, False)

        If (DirectCast(_parentForm, DocumentForm).ActiveDrawingCanvas.StampMapper.ContainsKey(stamp.Id)) AndAlso (DirectCast(_parentForm, DocumentForm).ActiveDrawingCanvas.StampMapper(stamp.Id).Count = 0) Then
            DeleteQMStamp(False, False)
        Else
            Me.ugQMStamps.ActiveCell = Me.ugQMStamps.Selected.Rows(0).Cells(NameOf(InformationHubStrings.Specification_ColumnCaption))
            Me.ugQMStamps.PerformAction(UltraGridAction.EnterEditMode)
        End If
    End Sub

    Friend Sub DeleteQMStamp(promptMessage As Boolean, Optional confirmDeletion As Boolean = True)
        Dim selectedGridRows As New List(Of UltraGridRow)

        For Each row As UltraGridRow In Me.ugQMStamps.Selected.Rows
            selectedGridRows.Add(row)
        Next

        If (selectedGridRows.Count = 0) Then
            If (Me.ugQMStamps.ActiveRow IsNot Nothing) Then
                If (Me.ugQMStamps.ActiveCell Is Nothing OrElse Not Me.ugQMStamps.ActiveCell.IsInEditMode) Then
                    selectedGridRows.Add(Me.ugQMStamps.ActiveRow)
                Else
                    Exit Sub
                End If
            ElseIf (Me.ugQMStamps.ActiveCell IsNot Nothing) Then
                If (Not Me.ugQMStamps.ActiveCell.IsInEditMode) Then
                    selectedGridRows.Add(Me.ugQMStamps.ActiveCell.Row)
                Else
                    Exit Sub
                End If
            End If
        End If

        If (selectedGridRows.Count = 0) Then
            If (promptMessage) Then
                MessageBox.Show(InformationHubStrings.SelectObjectFirst_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

            Exit Sub
        ElseIf (confirmDeletion) AndAlso (MessageBoxEx.ShowQuestion(InformationHubStrings.DeleteQMStamp_Msg) = MsgBoxResult.No) Then
            Exit Sub
        End If

        If (Me.utcInformationHub.ActiveTab IsNot Nothing) Then
            ClearSelectedRowsInGrid(DirectCast(Me.utcInformationHub.ActiveTab.TabPage.Controls(0), UltraGrid))
        End If

        If (Me.utcInformationHub.Tabs(TabNames.tabQMStamps.ToString()).Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True) Then

            ClearMarkedRowsInGrids()
        End If

        For Each selectedGridRow As UltraGridRow In selectedGridRows
            Dim qmStampsForDeletion As New List(Of QMStamp)

            For Each stamp As QMStamp In _qmStamps.Stamps.Where(Function(s) s.Id = selectedGridRow.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString)
                qmStampsForDeletion.Add(stamp)
            Next

            For Each stamp As QMStamp In qmStampsForDeletion
                _qmStamps.Stamps.Remove(stamp)
            Next
        Next

        InitializeQMStamps()

        Me.ugQMStamps.UpdateData()

        _informationHubEventArgs.ObjectIds.NewOrClear
        _informationHubEventArgs.ObjectType = E3.Lib.Schema.Kbl.KblObjectType.QMStamp
        _informationHubEventArgs.RemovePreviousSelection = True

        RaiseEvent QMStampsChanged(Me, _informationHubEventArgs)
    End Sub

    Friend Sub ImportQMStamps()
        InitializeQMStamps()
        BeginGridUpdate()

        Me.ugQMStamps.UpdateData()

        _informationHubEventArgs.ObjectIds.NewOrClear

        For Each stamp As QMStamp In _qmStamps.Stamps
            _informationHubEventArgs.ObjectIds.Add(stamp.Id)
        Next

        _informationHubEventArgs.ObjectType = E3.Lib.Schema.Kbl.KblObjectType.QMStamp
        _informationHubEventArgs.RemovePreviousSelection = True

        EndGridUpdate()

        RaiseEvent QMStampsChanged(Me, _informationHubEventArgs)
    End Sub


    Private Sub GetConnectorInformation(kblId As String, ByRef specificationValues As List(Of String))
        Dim connectorOcc As Connector_occurrence = _kblMapper.GetHarness.GetConnectorOccurrence(kblId)
        If (connectorOcc IsNot Nothing) Then
            If (Not specificationValues.Contains(connectorOcc.Id)) Then
                specificationValues.Add(connectorOcc.Id)
            End If

            If (connectorOcc.Alias_id IsNot Nothing) Then
                For Each aliasId As Alias_identification In connectorOcc.Alias_id
                    If (Not specificationValues.Contains(aliasId.Alias_id)) Then
                        specificationValues.Add(aliasId.Alias_id)
                    End If
                Next
            End If

            Dim connector As Connector_housing = _kblMapper.GetConnectorHousing(connectorOcc.Part)
            If (connector IsNot Nothing) AndAlso (connector.Abbreviation IsNot Nothing) AndAlso (connector.Abbreviation <> String.Empty) AndAlso (Not specificationValues.Contains(connector.Abbreviation)) Then
                specificationValues.Add(connector.Abbreviation)
            End If
        End If
    End Sub

    Private Sub GetFixingInformation(kblId As String, ByRef specificationValues As List(Of String))
        Dim fixingOcc As Fixing_occurrence = _kblMapper.GetHarness.GetFixingOccurrence(kblId)
        If (fixingOcc IsNot Nothing) Then
            If (Not specificationValues.Contains(fixingOcc.Id)) Then
                specificationValues.Add(fixingOcc.Id)
            End If

            If (fixingOcc.Alias_id IsNot Nothing) Then
                For Each aliasId As Alias_identification In fixingOcc.Alias_id
                    If (Not specificationValues.Contains(aliasId.Alias_id)) Then
                        specificationValues.Add(aliasId.Alias_id)
                    End If
                Next
            End If

            Dim fixing As Fixing = _kblMapper.GetFixing(fixingOcc.Part)
            If (fixing IsNot Nothing) AndAlso (fixing.Abbreviation IsNot Nothing) AndAlso (fixing.Abbreviation <> String.Empty) AndAlso (Not specificationValues.Contains(fixing.Abbreviation)) Then
                specificationValues.Add(fixing.Abbreviation)
            End If
        End If
    End Sub

    Private Sub GetFixingInformationFromSegment(segment As Segment, ByRef specificationValues As List(Of String))
        For Each fixingAssignment As Fixing_assignment In segment.Fixing_assignment
            GetFixingInformation(fixingAssignment.Fixing, specificationValues)
        Next
    End Sub

    Private Sub GetProtectionInformation(kblId As String, ByRef specificationValues As List(Of String))
        Dim protectionOcc As Wire_protection_occurrence = _kblMapper.GetHarness.GetWireProtectionOccurrence(kblId)
        If (protectionOcc IsNot Nothing) Then
            If (Not specificationValues.Contains(protectionOcc.Id)) Then
                specificationValues.Add(protectionOcc.Id)
            End If

            If (protectionOcc.Alias_id IsNot Nothing) Then
                For Each aliasId As Alias_identification In protectionOcc.Alias_id
                    If (Not specificationValues.Contains(aliasId.Alias_id)) Then
                        specificationValues.Add(aliasId.Alias_id)
                    End If
                Next
            End If

            Dim protection As Wire_protection = _kblMapper.GetWireProtection(protectionOcc.Part)
            If (protection IsNot Nothing) AndAlso (protection.Abbreviation IsNot Nothing) AndAlso (protection.Abbreviation <> String.Empty) AndAlso (Not specificationValues.Contains(protection.Abbreviation)) Then
                specificationValues.Add(protection.Abbreviation)
            End If
        End If
    End Sub

    Private Sub GetProtectionInformationFromSegment(segment As Segment, ByRef specificationValues As List(Of String))
        For Each protectionArea As Protection_area In segment.Protection_area
            GetProtectionInformation(protectionArea.Associated_protection, specificationValues)
        Next
    End Sub

    Private Function GetSpecificationValues(stamp As QMStamp) As List(Of AutocompleteMenuNS.AutocompleteItem)
        Dim addedSegmentLengthes As String = String.Empty
        Dim specificationValues As New List(Of String)

        For Each objRef As ObjectReference In stamp.ObjectReferences
            Select Case objRef.ObjectType
                Case E3.Lib.Schema.Kbl.KblObjectType.Connector_occurrence
                    For Each stampSpec As QMStampSpecification In _qmStampSpecifications.Connector
                        If (Not specificationValues.Contains(stampSpec.Specification)) Then
                            specificationValues.Add(stampSpec.Specification)
                        End If
                    Next

                    GetConnectorInformation(objRef.KblId, specificationValues)

                    For Each node As Node In _kblMapper.GetNodes.Where(Function(n) n.Referenced_components IsNot Nothing AndAlso n.Referenced_components <> String.Empty AndAlso n.Referenced_components.SplitSpace.ToList.Contains(objRef.KblId))
                        For Each segment As Segment In _kblMapper.GetSegments.Where(Function(seg) seg.Start_node = node.SystemId OrElse seg.End_node = node.SystemId)
                            If (Not specificationValues.Contains(segment.Id)) Then
                                specificationValues.Add(segment.Id)
                            End If

                            Dim counterNode As Node = If(segment.Start_node = node.SystemId, _kblMapper.GetNode(segment.End_node), _kblMapper.GetNode(segment.Start_node))
                            If (counterNode IsNot Nothing) AndAlso (counterNode.Referenced_components IsNot Nothing) AndAlso (counterNode.Referenced_components <> String.Empty) Then
                                For Each recComponent As String In counterNode.Referenced_components.SplitSpace
                                    Dim connectorOcc As Connector_occurrence = _kblMapper.GetHarness.GetConnectorOccurrence(recComponent)
                                    If (connectorOcc IsNot Nothing) AndAlso (Not specificationValues.Contains(connectorOcc.Id)) Then
                                        specificationValues.Add(connectorOcc.Id)
                                    End If
                                Next
                            End If

                            If (segment.Fixing_assignment IsNot Nothing) Then
                                GetFixingInformationFromSegment(segment, specificationValues)
                            End If
                        Next
                    Next
                Case E3.Lib.Schema.Kbl.KblObjectType.Fixing_occurrence
                    For Each stampSpec As QMStampSpecification In _qmStampSpecifications.Fixing
                        If (Not specificationValues.Contains(stampSpec.Specification)) Then
                            specificationValues.Add(stampSpec.Specification)
                        End If
                    Next

                    GetFixingInformation(objRef.KblId, specificationValues)

                    For Each segment As Segment In _kblMapper.GetSegments.Where(Function(seg) seg.Fixing_assignment IsNot Nothing AndAlso seg.Fixing_assignment.Any(Function(fix) fix.Fixing = objRef.KblId))
                        If (Not specificationValues.Contains(segment.Id)) Then
                            specificationValues.Add(segment.Id)
                        End If

                        If (segment.Fixing_assignment IsNot Nothing) Then
                            GetFixingInformationFromSegment(segment, specificationValues)
                        End If

                        Dim nodes As New List(Of Node)
                        nodes.Add(_kblMapper.GetNode(segment.Start_node))
                        nodes.Add(_kblMapper.GetNode(segment.End_node))

                        For Each node As Node In nodes
                            If (node.Referenced_components IsNot Nothing) Then
                                For Each refComponent As String In node.Referenced_components.SplitSpace
                                    GetConnectorInformation(refComponent, specificationValues)
                                Next
                            End If
                        Next
                    Next
                Case E3.Lib.Schema.Kbl.KblObjectType.Wire_protection_occurrence
                    For Each stampSpec As QMStampSpecification In _qmStampSpecifications.Protection
                        If (Not specificationValues.Contains(stampSpec.Specification)) Then
                            specificationValues.Add(stampSpec.Specification)
                        End If
                    Next

                    GetProtectionInformation(objRef.KblId, specificationValues)

                    For Each segment As Segment In _kblMapper.GetSegments.Where(Function(seg) seg.Protection_area IsNot Nothing AndAlso seg.Protection_area.Any(Function(area) area.Associated_protection = objRef.KblId))
                        If (Not specificationValues.Contains(segment.Id)) Then
                            specificationValues.Add(segment.Id)
                        End If
                    Next
                Case E3.Lib.Schema.Kbl.KblObjectType.Segment
                    For Each stampSpec As QMStampSpecification In _qmStampSpecifications.Segment
                        If (Not specificationValues.Contains(stampSpec.Specification)) Then
                            specificationValues.Add(stampSpec.Specification)
                        End If
                    Next

                    Dim segment As Segment = _kblMapper.GetSegment(objRef.KblId)
                    If (segment IsNot Nothing) Then
                        If (Not specificationValues.Contains(segment.Id)) Then
                            specificationValues.Add(segment.Id)
                        End If

                        If (segment.Alias_id IsNot Nothing) Then
                            For Each aliasId As Alias_identification In segment.Alias_id
                                If (Not specificationValues.Contains(aliasId.Alias_id)) Then
                                    specificationValues.Add(aliasId.Alias_id)
                                End If
                            Next
                        End If

                        If (addedSegmentLengthes = String.Empty) Then
                            addedSegmentLengthes = String.Format("({0}", Math.Round(segment.Physical_length.Value_component, 2))
                        Else
                            addedSegmentLengthes = String.Format("{0}+{1}", addedSegmentLengthes, Math.Round(segment.Physical_length.Value_component, 2))
                        End If

                        If (segment.Fixing_assignment IsNot Nothing) Then
                            GetFixingInformationFromSegment(segment, specificationValues)
                        End If
                        If (segment.Protection_area IsNot Nothing) Then
                            GetProtectionInformationFromSegment(segment, specificationValues)
                        End If

                        Dim nodes As New List(Of Node)
                        nodes.Add(_kblMapper.GetNode(segment.Start_node))
                        nodes.Add(_kblMapper.GetNode(segment.End_node))

                        For Each node As Node In nodes
                            If (node.Referenced_components IsNot Nothing) Then
                                For Each refComponent As String In node.Referenced_components.SplitSpace
                                    GetConnectorInformation(refComponent, specificationValues)
                                Next
                            End If
                        Next
                    End If
                Case Else
                    For Each stampSpec As QMStampSpecification In _qmStampSpecifications.Unspecified
                        If (Not specificationValues.Contains(stampSpec.Specification)) Then
                            specificationValues.Add(stampSpec.Specification)
                        End If
                    Next
            End Select
        Next

        If (addedSegmentLengthes <> String.Empty) Then
            addedSegmentLengthes += ")"
            specificationValues.Add(addedSegmentLengthes)
        End If

        specificationValues.Sort()

        Dim specificationItems As New List(Of AutocompleteMenuNS.AutocompleteItem)
        For Each specificationValue As String In specificationValues
            If (Not String.IsNullOrWhiteSpace(specificationValue)) Then
                specificationItems.Add(New AutocompleteMenuNS.AutocompleteItem(specificationValue.Trim))
            End If
        Next

        Return specificationItems
    End Function

    Private Sub InitializeQMStamps()
        With Me.udsQMStamps
            If (.Band.Columns.Count = 0) Then
                With .Band
                    .Columns.Add(SYSTEM_ID_COLUMN_KEY)
                    .Columns.Add(NameOf(InformationHubStrings.RefNo_ColumnCaption), GetType(Integer))
                    .Columns.Add(NameOf(InformationHubStrings.Specification_ColumnCaption))
                    .Columns.Add(SPEC_POSTFIX_COLUMN_KEY)
                    .Columns.Add(NameOf(InformationHubStrings.CreatedBy_ColumnCaption))
                    .Columns.Add(NameOf(InformationHubStrings.DateOfCreation_ColumnCaption))
                    .Columns.Add(NameOf(InformationHubStrings.CheckResult_ColumnCaption))
                    .Columns.Add(NameOf(InformationHubStrings.CheckComment_ColumnCaption))
                    .Columns.Add(NameOf(InformationHubStrings.Passed_ColumnCaption), GetType(Boolean))
                    .Columns.Add(NameOf(InformationHubStrings.CheckedBy_ColumnCaption))
                    .Columns.Add(NameOf(InformationHubStrings.DateOfCheck_ColumnCaption))
                    .Columns.Add(NameOf(InformationHubStrings.ObjectNames_ColumnCaption))
                    .Columns.Add(NameOf(InformationHubStrings.ObjectTypes_ColumnCaption))

                    .Key = "QMStamps"
                End With
            End If

            .Rows.Clear()

            If (_qmStamps.Stamps.Count > 0) Then
                Me.utcInformationHub.Tabs(TabNames.tabQMStamps.ToString()).Visible = True

                For Each stamp As QMStamp In _qmStamps.Stamps
                    Dim row As UltraDataRow = .Rows.Add
                    With row
                        .SetCellValue(SYSTEM_ID_COLUMN_KEY, stamp.Id)
                        .SetCellValue(NameOf(InformationHubStrings.RefNo_ColumnCaption), stamp.RefNo)

                        Dim spec As String = stamp.Specification
                        If (spec IsNot Nothing) AndAlso (spec.Contains(vbTab)) Then
                            Dim posOfLastSpaceInSpec As Integer = spec.LastIndexOf(vbTab)
                            Dim firstPartOfSpec As String = spec.Substring(0, posOfLastSpaceInSpec)
                            Dim lastPartOfSpec As String = spec.Substring(posOfLastSpaceInSpec + 1).Trim

                            .SetCellValue(NameOf(InformationHubStrings.Specification_ColumnCaption), firstPartOfSpec)
                            .SetCellValue(SPEC_POSTFIX_COLUMN_KEY, lastPartOfSpec)
                        Else
                            .SetCellValue(NameOf(InformationHubStrings.Specification_ColumnCaption), stamp.Specification)
                        End If

                        .SetCellValue(NameOf(InformationHubStrings.CreatedBy_ColumnCaption), stamp.CreatedBy)
                        .SetCellValue(NameOf(InformationHubStrings.DateOfCreation_ColumnCaption), stamp.DateOfCreation)
                        .SetCellValue(NameOf(InformationHubStrings.CheckResult_ColumnCaption), stamp.CheckResult)
                        .SetCellValue(NameOf(InformationHubStrings.CheckComment_ColumnCaption), stamp.CheckComment)
                        .SetCellValue(NameOf(InformationHubStrings.Passed_ColumnCaption), stamp.Passed)
                        .SetCellValue(NameOf(InformationHubStrings.CheckedBy_ColumnCaption), stamp.CheckedBy)
                        .SetCellValue(NameOf(InformationHubStrings.DateOfCheck_ColumnCaption), If(stamp.DateOfCheck = DateTime.MinValue, String.Empty, stamp.DateOfCheck.ToString))

                        Dim objNames As New List(Of String)
                        Dim objTypes As New List(Of KblObjectType)

                        For Each objRef As ObjectReference In stamp.ObjectReferences
                            objNames.Add(objRef.ObjectName)
                            objTypes.TryAdd(objRef.ObjectType)
                        Next

                        objNames.Sort()
                        objTypes.Sort()

                        Dim objectNames As String = String.Empty
                        Dim localizedObjectTypes As String = String.Empty

                        For Each objName As String In objNames
                            objectNames = If(objectNames = String.Empty, objName, String.Format("{0}; {1}", objectNames, objName))
                        Next

                        For Each objType As KblObjectType In objTypes
                            Dim localizedObjTypeName As String = [Lib].Schema.Kbl.Utils.GetLocalizedName(objType)
                            localizedObjectTypes = If(localizedObjectTypes = String.Empty, localizedObjTypeName, String.Format("{0}; {1}", localizedObjectTypes, localizedObjTypeName))
                        Next

                        .SetCellValue(NameOf(InformationHubStrings.ObjectNames_ColumnCaption), objectNames)
                        .SetCellValue(NameOf(InformationHubStrings.ObjectTypes_ColumnCaption), localizedObjectTypes)

                        .Tag = stamp
                    End With
                Next
            Else
                Me.utcInformationHub.Tabs(TabNames.tabQMStamps.ToString()).Visible = False
            End If
        End With

    End Sub

    Private Sub OnQMStampChanged(stamp As QMStamp)
        _informationHubEventArgs.ObjectIds = New HashSet(Of String)

        For Each objRef As ObjectReference In stamp.ObjectReferences
            _informationHubEventArgs.ObjectIds.Add(objRef.KblId)
            _informationHubEventArgs.ObjectType = objRef.ObjectType
        Next

        _informationHubEventArgs.StampIds = New List(Of String)
        _informationHubEventArgs.StampIds.Add(stamp.Id)
        _informationHubEventArgs.RemovePreviousSelection = False

        RaiseEvent QMStampsChanged(Me, _informationHubEventArgs)
    End Sub

    Private Sub ugQMStamps_AfterExitEditMode(sender As Object, e As EventArgs) Handles ugQMStamps.AfterExitEditMode
        If (Me.ugQMStamps.ActiveCell Is Nothing) Then
            Exit Sub
        End If

        Dim stamp As QMStamp = _qmStamps.Stamps.Where(Function(s) s.Id = Me.ugQMStamps.ActiveCell.Row.Cells(SYSTEM_ID_COLUMN_KEY).Value.ToString).FirstOrDefault
        If (stamp IsNot Nothing) Then
            Select Case Me.ugQMStamps.ActiveCell.Column.Key
                Case NameOf(InformationHubStrings.RefNo_ColumnCaption)
                    Dim refNo As Integer = CInt(Me.ugQMStamps.ActiveCell.Value)
                    If (refNo <> stamp.RefNo) Then
                        stamp.RefNo = refNo

                        OnQMStampChanged(stamp)
                    End If
                Case NameOf(InformationHubStrings.Specification_ColumnCaption)
                    Me.ugQMStamps.ActiveCell.Value = Me.ugQMStamps.ActiveCell.Value.ToString.Trim

                    Dim spec As String = Me.ugQMStamps.ActiveCell.Value.ToString
                    Dim specPostfix As String = Me.ugQMStamps.ActiveCell.Row.Cells(SPEC_POSTFIX_COLUMN_KEY).Value.ToString
                    Dim specification As String = String.Empty

                    If (spec IsNot Nothing AndAlso spec <> String.Empty AndAlso (specPostfix Is Nothing OrElse specPostfix = String.Empty)) Then
                        specification = spec
                    ElseIf ((spec Is Nothing OrElse spec = String.Empty) AndAlso specPostfix IsNot Nothing AndAlso specPostfix <> String.Empty) Then
                        specification = specPostfix
                    Else
                        specification = String.Format("{0}{1}{2}", spec, vbTab, specPostfix).Trim
                    End If

                    If (specification <> stamp.Specification) Then
                        stamp.Specification = specification
                        OnQMStampChanged(stamp)
                    End If
                Case SPEC_POSTFIX_COLUMN_KEY
                    Me.ugQMStamps.ActiveCell.Value = Me.ugQMStamps.ActiveCell.Value.ToString.Trim

                    Dim spec As String = Me.ugQMStamps.ActiveCell.Row.Cells(NameOf(InformationHubStrings.Specification_ColumnCaption)).Value.ToString
                    Dim specPostfix As String = Me.ugQMStamps.ActiveCell.Value.ToString
                    Dim specification As String = String.Empty

                    If (spec IsNot Nothing AndAlso spec <> String.Empty AndAlso (specPostfix Is Nothing OrElse specPostfix = String.Empty)) Then
                        specification = spec
                    ElseIf ((spec Is Nothing OrElse spec = String.Empty) AndAlso specPostfix IsNot Nothing AndAlso specPostfix <> String.Empty) Then
                        specification = specPostfix
                    Else
                        specification = String.Format("{0}{1}{2}", spec, vbTab, specPostfix).Trim
                    End If

                    If (specification <> stamp.Specification) Then
                        stamp.Specification = specification

                        OnQMStampChanged(stamp)
                    End If
                Case NameOf(InformationHubStrings.CreatedBy_ColumnCaption)
                    Me.ugQMStamps.ActiveCell.Value = Me.ugQMStamps.ActiveCell.Value.ToString.Trim

                    Dim createdBy As String = Me.ugQMStamps.ActiveCell.Value.ToString
                    If (createdBy <> stamp.CreatedBy) Then
                        stamp.CreatedBy = createdBy

                        OnQMStampChanged(stamp)
                    End If
                Case NameOf(InformationHubStrings.CheckResult_ColumnCaption)
                    Me.ugQMStamps.ActiveCell.Value = Me.ugQMStamps.ActiveCell.Value.ToString.Trim

                    Dim checkResult As String = Me.ugQMStamps.ActiveCell.Value.ToString
                    If (checkResult <> stamp.CheckResult) Then
                        stamp.CheckedBy = System.Security.Principal.WindowsIdentity.GetCurrent.Name
                        stamp.CheckResult = checkResult
                        stamp.DateOfCheck = Now

                        Me.ugQMStamps.ActiveCell.Row.Cells(NameOf(InformationHubStrings.CheckedBy_ColumnCaption)).Value = stamp.CheckedBy
                        Me.ugQMStamps.ActiveCell.Row.Cells(NameOf(InformationHubStrings.DateOfCheck_ColumnCaption)).Value = stamp.DateOfCheck

                        OnQMStampChanged(stamp)
                    End If
                Case NameOf(InformationHubStrings.CheckComment_ColumnCaption)
                    Me.ugQMStamps.ActiveCell.Value = Me.ugQMStamps.ActiveCell.Value.ToString.Trim

                    Dim checkComment As String = Me.ugQMStamps.ActiveCell.Value.ToString
                    If (checkComment <> stamp.CheckComment) Then
                        stamp.CheckedBy = System.Security.Principal.WindowsIdentity.GetCurrent.Name
                        stamp.DateOfCheck = Now
                        stamp.CheckComment = checkComment

                        Me.ugQMStamps.ActiveCell.Row.Cells(NameOf(InformationHubStrings.CheckedBy_ColumnCaption)).Value = stamp.CheckedBy
                        Me.ugQMStamps.ActiveCell.Row.Cells(NameOf(InformationHubStrings.DateOfCheck_ColumnCaption)).Value = stamp.DateOfCheck

                        OnQMStampChanged(stamp)
                    End If
                Case NameOf(InformationHubStrings.Passed_ColumnCaption)
                    Dim passed As Boolean = CBool(Me.ugQMStamps.ActiveCell.Value)
                    If (passed <> stamp.Passed) Then
                        stamp.CheckedBy = System.Security.Principal.WindowsIdentity.GetCurrent.Name
                        stamp.DateOfCheck = Now
                        stamp.Passed = passed

                        Me.ugQMStamps.ActiveCell.Row.Cells(NameOf(InformationHubStrings.CheckedBy_ColumnCaption)).Value = stamp.CheckedBy
                        Me.ugQMStamps.ActiveCell.Row.Cells(NameOf(InformationHubStrings.DateOfCheck_ColumnCaption)).Value = stamp.DateOfCheck

                        OnQMStampChanged(stamp)
                    End If
                Case NameOf(InformationHubStrings.CheckedBy_ColumnCaption)
                    Me.ugQMStamps.ActiveCell.Value = Me.ugQMStamps.ActiveCell.Value.ToString.Trim

                    Dim checkedBy As String = Me.ugQMStamps.ActiveCell.Value.ToString
                    If (checkedBy <> stamp.CheckedBy) Then
                        stamp.CheckedBy = checkedBy
                        stamp.DateOfCheck = Now

                        Me.ugQMStamps.ActiveCell.Row.Cells(NameOf(InformationHubStrings.DateOfCheck_ColumnCaption)).Value = stamp.DateOfCheck

                        OnQMStampChanged(stamp)
                    End If
            End Select
        End If
    End Sub

    Private Sub ugQMStamps_AfterRowActivate(sender As Object, e As EventArgs) Handles ugQMStamps.AfterRowActivate
        If (Me.ugQMStamps.ActiveRow.Appearance.BackColor = Color.FromArgb(190, 190, 190)) Then
            Me.ugQMStamps.ActiveRow = Nothing
        ElseIf (Not Me.ugQMStamps.ActiveRow.Selected) Then
            Me.ugQMStamps.Selected.Rows.Add(Me.ugQMStamps.ActiveRow)
        End If
    End Sub

    Private Sub ugQMStamps_AfterSelectChange(sender As Object, e As AfterSelectChangeEventArgs) Handles ugQMStamps.AfterSelectChange
        Dim kblIds As New List(Of String)

        With Me.ugQMStamps
            .BeginUpdate()

            Dim selectedRows As New List(Of UltraGridRow)
            Dim stampIds As New List(Of String)

            If (.ActiveCell IsNot Nothing) AndAlso (.Selected.Rows.Count = 0) Then
                selectedRows.Add(.ActiveCell.Row)
            ElseIf (.ActiveRow IsNot Nothing) AndAlso (.Selected.Rows.Count = 0) Then
                selectedRows.Add(.ActiveRow)
            ElseIf (.Selected.Rows.Count <> 0) Then
                For Each selectedRow As UltraGridRow In .Selected.Rows
                    selectedRows.Add(selectedRow)
                Next
            End If

            For Each selectedRow As UltraGridRow In selectedRows
                If (Not kblIds.Contains(_qmStamps.Stamps.Single(Function(s) s.Id = selectedRow.Tag?.ToString).Id)) Then
                    Dim stamp As QMStamp = _qmStamps.Stamps.Single(Function(s) s.Id = selectedRow.Tag?.ToString)
                    With stamp
                        stampIds.Add(.Id)

                        For Each objRef As ObjectReference In stamp.ObjectReferences
                            kblIds.Add(objRef.KblId)
                        Next
                    End With
                End If
            Next

            SetInformationHubEventArgs(kblIds, Nothing, Nothing)

            _informationHubEventArgs.StampIds = stampIds

            If (.ActiveCell IsNot Nothing) AndAlso (Not .ActiveRowScrollRegion.VisibleRows.Contains(.ActiveCell.Row)) Then
                .ActiveRowScrollRegion.ScrollRowIntoView(.ActiveCell.Row)
            ElseIf (.ActiveRow IsNot Nothing) AndAlso (Not .ActiveRowScrollRegion.VisibleRows.Contains(.ActiveRow)) Then
                .ActiveRowScrollRegion.ScrollRowIntoView(.ActiveRow)
            End If

            .EndUpdate()
        End With

        OnHubSelectionChanged()
    End Sub

    Private Sub ugQMStamps_BeforeCellUpdate(sender As Object, e As BeforeCellUpdateEventArgs) Handles ugQMStamps.BeforeCellUpdate
        If (e.Cell.Column.Key = NameOf(InformationHubStrings.RefNo_ColumnCaption)) AndAlso (e.NewValue.ToString = String.Empty) Then
            MessageBox.Show(InformationHubStrings.RefNoCannotBeEmpty_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
            e.Cancel = True
        ElseIf e.Cell.Column.Key = NameOf(InformationHubStrings.RefNo_ColumnCaption) AndAlso ((CInt(e.Cell.Value) <> CInt(e.NewValue)) AndAlso (_qmStamps.Stamps.Any(Function(stamp) stamp.RefNo.Value = CInt(e.NewValue))) AndAlso (MessageBoxEx.ShowQuestion(InformationHubStrings.RefNoAlreadyInUse_Msg) = DialogResult.No)) Then
            e.Cancel = True
        End If
    End Sub

    Private Sub ugQMStamps_BeforeEnterEditMode(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles ugQMStamps.BeforeEnterEditMode
        If (Me.ugQMStamps.ActiveCell Is Nothing) Then
            e.Cancel = True
        End If
    End Sub

    Private Sub ugQMStamps_BeforeSelectChange(sender As Object, e As BeforeSelectChangeEventArgs) Handles ugQMStamps.BeforeSelectChange
        For Each row As UltraGridRow In e.NewSelections.Rows
            If (row.Appearance.BackColor = Color.FromArgb(190, 190, 190)) Then
                e.Cancel = True
            End If
        Next
    End Sub

    Private Sub ugQMStamps_CellDataError(sender As Object, e As CellDataErrorEventArgs) Handles ugQMStamps.CellDataError
        e.RaiseErrorEvent = False
        e.RestoreOriginalValue = True
    End Sub

    Private Sub ugQMStamps_ClickCell(sender As Object, e As ClickCellEventArgs) Handles ugQMStamps.ClickCell
        If (_pressedMouseButton = System.Windows.Forms.MouseButtons.Left) AndAlso (_pressedKey <> Keys.ControlKey AndAlso _pressedKey <> Keys.ShiftKey) AndAlso (_informationHubEventArgs IsNot Nothing) AndAlso (_kblMapperForCompare IsNot Nothing OrElse e.Cell.Row.Appearance.BackColor <> Color.FromArgb(190, 190, 190)) Then
            Dim ids As New List(Of String)
            Dim prevIds As IEnumerable(Of String) = _informationHubEventArgs.ObjectIds
            Dim stamp As QMStamp = _qmStamps.Stamps.Single(Function(s) s.Id = e.Cell.Row.Tag?.ToString)

            For Each objRef As ObjectReference In stamp.ObjectReferences
                ids.Add(objRef.KblId)
            Next

            SetInformationHubEventArgs(ids, Nothing, Nothing)

            If (prevIds IsNot Nothing) AndAlso (String.Join(";", prevIds) <> String.Join(";", _informationHubEventArgs.ObjectIds)) Then
                OnHubSelectionChanged()
            End If
        ElseIf (_pressedMouseButton = System.Windows.Forms.MouseButtons.Right) AndAlso (_kblMapperForCompare Is Nothing) AndAlso (ChangeGridContextMenuVisibility(e.Cell.Row)) Then
            _contextMenuGrid.ShowPopup()
        End If
    End Sub

    Private Sub ugQMStamps_DoubleClickRow(sender As Object, e As DoubleClickRowEventArgs) Handles ugQMStamps.DoubleClickRow
        OnDoubleClickRow(sender, e)
    End Sub

    Private Sub ugQMStamps_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugQMStamps.InitializeLayout
        Me.ugQMStamps.BeginUpdate()
        Me.ugQMStamps.EventManager.AllEventsEnabled = False

        InitializeGridLayout(Nothing, e.Layout)

        Dim specGroup As UltraGridGroup = e.Layout.Bands(0).Groups.Add(NameOf(InformationHubStrings.Specification_ColumnCaption))
        specGroup.Header.Caption = InformationHubStrings.Specification_ColumnCaption
        specGroup.RowLayoutGroupInfo.OriginX = 2

        e.Layout.Bands(0).RowLayoutStyle = RowLayoutStyle.GroupLayout

        e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True
        e.Layout.Override.CellClickAction = CellClickAction.Default
        e.Layout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True

        For Each column As UltraGridColumn In e.Layout.Bands(0).Columns
            If Not column.Hidden Then
                With column
                    .CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center
                    .CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle
                    .Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center
                    .Header.Appearance.TextVAlign = Infragistics.Win.VAlign.Middle

                    Select Case .Index
                        Case 0
                            .Hidden = True
                        Case 1
                            .RowLayoutColumnInfo.OriginX = 0
                        Case 2
                            .CellAppearance.TextHAlign = HAlign.Left
                            .RowLayoutColumnInfo.LabelPosition = LabelPosition.None
                            .RowLayoutColumnInfo.ParentGroup = specGroup
                            .Width = 250
                        Case 3
                            .CellAppearance.TextHAlign = HAlign.Right
                            .RowLayoutColumnInfo.LabelPosition = LabelPosition.None
                            .RowLayoutColumnInfo.ParentGroup = specGroup
                            .Width = 75
                        Case 6, 7
                            .Width = 200
                        Case 4, 9
                            If (.Index = 4) Then
                                .RowLayoutColumnInfo.OriginX = 6
                                .RowLayoutColumnInfo.OriginY = 0
                            End If

                            If (_generalSettings Is Nothing) OrElse (Not _generalSettings.LastChangedByEditable) Then
                                .CellActivation = Activation.NoEdit
                            End If
                        Case 5, 10
                            .CellActivation = Activation.NoEdit
                        Case 11, 12
                            .CellActivation = Activation.NoEdit
                            .Width = 300
                    End Select
                End With
            End If
        Next

        Me.ugQMStamps.EventManager.AllEventsEnabled = True
        Me.ugQMStamps.EndUpdate()
    End Sub

    Private Sub InitializeQmStampRowCore(row As UltraGridRow)
        row.Cells(NameOf(InformationHubStrings.Specification_ColumnCaption)).Editor = New EditorWithText
        row.Cells(SPEC_POSTFIX_COLUMN_KEY).Editor = New EditorWithText
        row.Tag = row.Cells(SYSTEM_ID_COLUMN_KEY).Value

        Dim autocompleteMenu As New AutocompleteMenuNS.AutocompleteMenu
        With autocompleteMenu
            .AppearInterval = 250
            .MinFragmentLength = 0
            .SetAutocompleteItems(GetSpecificationValues(DirectCast(DirectCast(row.ListObject, UltraDataRow).Tag, QMStamp)))
            .SetAutocompleteMenu(DirectCast(row.Cells(NameOf(InformationHubStrings.Specification_ColumnCaption)).Editor, EditorWithText).TextBox, autocompleteMenu)
            .SetAutocompleteMenu(DirectCast(row.Cells(SPEC_POSTFIX_COLUMN_KEY).Editor, EditorWithText).TextBox, autocompleteMenu)
        End With
    End Sub

    Private Sub ugQMStamps_InitializeRow(sender As Object, e As InitializeRowEventArgs) Handles ugQMStamps.InitializeRow
        If (Not e.ReInitialize) Then
            InitializeQmStampRowCore(e.Row)
        End If
    End Sub

    Private Sub ugQMStamps_KeyDown(sender As Object, e As KeyEventArgs) Handles ugQMStamps.KeyDown
        OnGridKeyDown(Me, New GridKeyDownEventArgs(e, DirectCast(sender, UltraGrid)))
        If Not e.Handled Then
            If (e.Control) AndAlso (e.KeyCode = Keys.A) Then
                SelectAllRowsOfActiveGrid()
            ElseIf (e.KeyCode = Keys.Delete) Then
                DeleteQMStamp(True)
            ElseIf (e.KeyCode = Keys.Escape) Then
                Me.ugQMStamps.ActiveCell = Nothing
                Me.ugQMStamps.ActiveRow = Nothing

                If Me.ugQMStamps.Selected.Rows.Count = 0 Then
                    ClearMarkedRowsInGrids()
                Else
                    ClearSelectedRowsInGrids()
                End If

                If SetInformationHubEventArgs(Nothing, Nothing, Me.ugQMStamps.Selected.Rows) Then
                    OnHubSelectionChanged()
                End If
            ElseIf e.KeyCode = Keys.Return Then
                SetSelectedAsMarkedOriginRows(ugQMStamps)
            End If
        End If
    End Sub

    Private Sub ugQMStamps_MouseDown(sender As Object, e As MouseEventArgs) Handles ugQMStamps.MouseDown
        If (_contextMenuGrid.IsOpen) Then
            _contextMenuGrid.ClosePopup()
        End If
        _pressedMouseButton = e.Button
    End Sub

    Private Sub ugQMStamps_MouseLeave(sender As Object, e As EventArgs) Handles ugQMStamps.MouseLeave
        _messageEventArgs.StatusMessage = String.Empty
        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub ugQMStamps_MouseMove(sender As Object, e As MouseEventArgs) Handles ugQMStamps.MouseMove
        _messageEventArgs.StatusMessage = String.Format(InformationHubStrings.RowCount_Label, Me.ugQMStamps.Rows.Count, Me.ugQMStamps.Rows.FilteredInNonGroupByRowCount, Me.ugQMStamps.Rows.VisibleRowCount, Me.ugQMStamps.Selected.Rows.Count)
        RaiseEvent Message(Me, _messageEventArgs)
    End Sub

    Private Sub udsQMStamps_CellDataUpdated(sender As Object, e As CellDataUpdatedEventArgs) Handles udsQMStamps.CellDataUpdated
        RaiseEvent CellValueUpdated(sender, e)
    End Sub

End Class