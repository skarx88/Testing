Imports System.IO
Imports System.Text
Imports Infragistics.Documents.Excel
Imports Infragistics.Win.UltraWinExplorerBar
Imports Infragistics.Win.UltraWinGrid
Imports Zuken.E3.HarnessAnalyzer.Settings

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class InlinersForm

    Friend Event InlinerSelectionChanged(sender As Object, e As InformationHubEventArgs)

    Private _harnessesWithRelevantModules As SortedDictionary(Of String, Dictionary(Of [Module], Boolean))
    Private _trivialInlinerPairs As List(Of TrivialInlinerPair)
    Private _virtualInlinerPairs As List(Of VirtualInlinerPair)
    Private _inactiveInlinerPairs As Integer
    Private _inlinerPairCheckClassifications As InlinerPairCheckClassificationList

    Public Sub New(virtualInlinerPairs As List(Of VirtualInlinerPair), trivialInlinerPairs As List(Of TrivialInlinerPair), inlinerPairCheckClassifications As InlinerPairCheckClassificationList, inactiveInlinerPairs As Integer)
        InitializeComponent()

        Me.Icon = My.Resources.Inliners

        _trivialInlinerPairs = trivialInlinerPairs
        _virtualInlinerPairs = virtualInlinerPairs
        _inactiveInlinerPairs = inactiveInlinerPairs
        _inlinerPairCheckClassifications = inlinerPairCheckClassifications
    End Sub

    Private Sub AddInlinerPairDetailGroup(inlinerPair As InlinerPair, inlinerPairCheckClassifications As InlinerPairCheckClassificationList)
        With Me.uebInliners
            Dim group As New UltraExplorerBarGroup(inlinerPair.Id)
            Dim inlinerPairDetails As New InlinerPairDetails(inlinerPair, inlinerPairCheckClassifications)
            AddHandler inlinerPairDetails.InlinerPairDetailsMouseClick, AddressOf OnInlinerPairDetailsMouseClick

            Dim containerCtrl As New UltraExplorerBarContainerControl
            .Controls.Add(containerCtrl)

            With containerCtrl
                .Controls.Add(inlinerPairDetails)
                .Name = "InlinerPairDetails"
            End With

            inlinerPairDetails.Dock = DockStyle.Fill

            .Groups.Add(group)

            With group
                .Container = containerCtrl
                .Expanded = True
                .Settings.AppearancesSmall.HeaderAppearance.Image = My.Resources.StatusGreen
                .Settings.ContainerHeight = 400
                .Settings.Style = GroupStyle.ControlContainer
                .Tag = inlinerPair
                '.Container.Controls.Add(inlinerPairDetails)
            End With

            Dim grpText As String = String.Empty

            If (TypeOf inlinerPair Is TrivialInlinerPair) Then
                Dim inlPair As TrivialInlinerPair = DirectCast(inlinerPair, TrivialInlinerPair)

                grpText = String.Format("{0} [{1}: {2}] <-> {3} [{4}: {5}]", inlinerPair.InlinerIdA, inlPair.KblMapperA.HarnessPartNumber, If(inlPair.KblMapperA.GetHarness.Description IsNot Nothing AndAlso inlPair.KblMapperA.GetHarness.Description <> String.Empty, inlPair.KblMapperA.GetHarness.Description, "-"), inlinerPair.InlinerIdB, inlPair.KblMapperB.HarnessPartNumber, If(inlPair.KblMapperB.GetHarness.Description IsNot Nothing AndAlso inlPair.KblMapperB.GetHarness.Description <> String.Empty, inlPair.KblMapperB.GetHarness.Description, "-"))
            Else
                Dim inlPair As VirtualInlinerPair = DirectCast(inlinerPair, VirtualInlinerPair)
                For Each connectorOccWithKblMapperA As KeyValuePair(Of Connector_occurrence, KblMapper) In inlPair.ActiveConnectorOccsWithKblMapperA.OrderBy(Function(c) c.Key.Id)
                    grpText = If(grpText = String.Empty, String.Format("{0} [{1}]", connectorOccWithKblMapperA.Key.Id, connectorOccWithKblMapperA.Value.HarnessPartNumber), String.Format("{0}, {1} [{2}]", grpText, connectorOccWithKblMapperA.Key.Id, connectorOccWithKblMapperA.Value.HarnessPartNumber))
                Next
                grpText &= " <-> "

                For Each connectorOccWithKblMapperB As KeyValuePair(Of Connector_occurrence, KblMapper) In inlPair.ActiveConnectorOccsWithKblMapperB.OrderBy(Function(c) c.Key.Id)
                    grpText = String.Format(If(grpText.EndsWith(" <-> "), "{0}{1} [{2}]", "{0}, {1} [{2}]"), grpText, connectorOccWithKblMapperB.Key.Id, connectorOccWithKblMapperB.Value.HarnessPartNumber)
                Next
            End If
            With group
                .ToolTipText = GetTooltipText(inlinerPair)
                .Text = grpText
            End With
            inlinerPairDetails.Initialize()
        End With
    End Sub

    Private Sub ExportToExcel(fileName As String)
        Dim workbook As New Workbook

        If (IO.Path.GetExtension(fileName).ToLower = ".xlsx") Then
            workbook.SetCurrentFormat(WorkbookFormat.Excel2007)
        Else
            workbook.SetCurrentFormat(WorkbookFormat.Excel97To2003)
        End If

        workbook.Worksheets.Add("Inliner pairs")

        With workbook.Worksheets("Inliner pairs")
            Dim rowCounter As Integer = 0

            .DefaultColumnWidth = 4000

            For Each group As UltraExplorerBarGroup In Me.uebInliners.Groups
                Dim inlinerPair As InlinerPair = DirectCast(group.Tag, InlinerPair)
                Dim inlinerPairDetails As InlinerPairDetails = DirectCast(group.Container.Controls(0), InlinerPairDetails)

                .Rows(rowCounter).CellFormat.Font.Bold = ExcelDefaultableBoolean.True
                .Rows(rowCounter).Cells(1).Value = InlinerFormStrings.LeftInliner_ExcelCellCaption
                .Rows(rowCounter).Cells(If(TypeOf inlinerPair Is TrivialInlinerPair, 9, 10)).Value = InlinerFormStrings.RightInliner_ExcelCellCaption

                .Rows(rowCounter).Cells(0).CellFormat.TopBorderStyle = CellBorderLineStyle.Thin

                .MergedCellsRegions.Add(rowCounter, 1, rowCounter, If(TypeOf inlinerPair Is TrivialInlinerPair, 8, 9)).CellFormat.TopBorderStyle = CellBorderLineStyle.Thin
                .MergedCellsRegions.Add(rowCounter, If(TypeOf inlinerPair Is TrivialInlinerPair, 9, 10), rowCounter, If(TypeOf inlinerPair Is TrivialInlinerPair, 16, 17)).CellFormat.TopBorderStyle = CellBorderLineStyle.Thin

                rowCounter += 1

                .Rows(rowCounter).CellFormat.Font.Italic = ExcelDefaultableBoolean.True

                If (TypeOf inlinerPair Is TrivialInlinerPair) Then
                    Dim inlPair As TrivialInlinerPair = DirectCast(inlinerPair, TrivialInlinerPair)

                    .Rows(rowCounter).Cells(1).Value = String.Format("{0} [{1}: {2}]", inlPair.InlinerIdA, inlPair.KblMapperA.HarnessPartNumber, If(inlPair.KblMapperA.GetHarness.Description IsNot Nothing AndAlso inlPair.KblMapperA.GetHarness.Description <> String.Empty, inlPair.KblMapperA.GetHarness.Description, "-"))
                    .Rows(rowCounter).Cells(9).Value = String.Format("{0} [{1}: {2}]", inlPair.InlinerIdB, inlPair.KblMapperB.HarnessPartNumber, If(inlPair.KblMapperB.GetHarness.Description IsNot Nothing AndAlso inlPair.KblMapperB.GetHarness.Description <> String.Empty, inlPair.KblMapperB.GetHarness.Description, "-"))
                Else
                    Dim inlPair As VirtualInlinerPair = DirectCast(inlinerPair, VirtualInlinerPair)
                    Dim text As String = String.Empty

                    For Each connectorOccWithKblMapperA As KeyValuePair(Of Connector_occurrence, KblMapper) In inlPair.ActiveConnectorOccsWithKblMapperA.OrderBy(Function(c) c.Key.Id)
                        text = If(text = String.Empty, String.Format("{0} [{1}]", connectorOccWithKblMapperA.Key.Id, connectorOccWithKblMapperA.Value.HarnessPartNumber), String.Format("{0}, {1} [{2}]", text, connectorOccWithKblMapperA.Key.Id, connectorOccWithKblMapperA.Value.HarnessPartNumber))
                    Next

                    .Rows(rowCounter).Cells(1).Value = text

                    text = String.Empty

                    For Each connectorOccWithKblMapperB As KeyValuePair(Of Connector_occurrence, KblMapper) In inlPair.ActiveConnectorOccsWithKblMapperB.OrderBy(Function(c) c.Key.Id)
                        text = If(text = String.Empty, String.Format("{0} [{1}]", connectorOccWithKblMapperB.Key.Id, connectorOccWithKblMapperB.Value.HarnessPartNumber), String.Format("{0}, {1} [{2}]", text, connectorOccWithKblMapperB.Key.Id, connectorOccWithKblMapperB.Value.HarnessPartNumber))
                    Next

                    .Rows(rowCounter).Cells(9).Value = text
                End If

                .MergedCellsRegions.Add(rowCounter, 1, rowCounter, If(TypeOf inlinerPair Is TrivialInlinerPair, 8, 9))
                .MergedCellsRegions.Add(rowCounter, If(TypeOf inlinerPair Is TrivialInlinerPair, 9, 10), rowCounter, If(TypeOf inlinerPair Is TrivialInlinerPair, 16, 17))

                For Each row As UltraGridRow In inlinerPairDetails.ugInlinerPair.Rows
                    rowCounter += 1

                    Dim colCounter As Integer = 0

                    If (row.Index = 0) Then
                        .Rows(rowCounter).CellFormat.Font.Bold = ExcelDefaultableBoolean.True

                        For Each column As UltraGridColumn In inlinerPairDetails.ugInlinerPair.DisplayLayout.Bands(0).Columns
                            If (Not column.Hidden) AndAlso (column.Header.Caption <> String.Empty) Then
                                .Rows(rowCounter).Cells(colCounter).Value = column.Header.Caption

                                colCounter += 1
                            End If
                        Next

                        rowCounter += 1
                    End If

                    colCounter = 0

                    For Each cell As UltraGridCell In row.Cells
                        If (Not cell.Column.Hidden) AndAlso (cell.Column.Header.Caption <> String.Empty) Then
                            .Rows(rowCounter).Cells(colCounter).Value = cell.Value

                            If (Not cell.Appearance.ForeColor.IsEmpty) Then .Rows(rowCounter).Cells(colCounter).CellFormat.Font.ColorInfo = New WorkbookColorInfo(cell.Appearance.ForeColor)
                            If (Not cell.Appearance.BackColor.IsEmpty) Then
                                .Rows(rowCounter).Cells(colCounter).CellFormat.Fill = New CellFillPattern(New WorkbookColorInfo(cell.Appearance.BackColor), New WorkbookColorInfo(Color.White), FillPatternStyle.Solid)

                                If (cell.Row.ToolTipText IsNot Nothing AndAlso cell.Row.ToolTipText <> String.Empty) Then .Rows(rowCounter).Cells(colCounter).Comment = New WorksheetCellComment With {.Text = New FormattedString(cell.Row.ToolTipText)}
                            End If

                            colCounter += 1
                        End If
                    Next
                Next

                rowCounter += 2
            Next
        End With

        For Each harnessWithRelevantModules As KeyValuePair(Of String, Dictionary(Of [Module], Boolean)) In _harnessesWithRelevantModules
            Dim worksheet As Worksheet = workbook.Worksheets.Add(String.Format("Harness {0}", harnessWithRelevantModules.Key))
            With worksheet
                Dim rowCounter As Integer = 0

                .DefaultColumnWidth = 4000

                .Rows(rowCounter).CellFormat.Font.Bold = ExcelDefaultableBoolean.True
                .Rows(rowCounter).Cells(0).Value = "Module part number"
                .Rows(rowCounter).Cells(1).Value = "Module abbreviation"
                .Rows(rowCounter).Cells(2).Value = "Module description"
                .Rows(rowCounter).Cells(3).Value = "Is active"

                rowCounter += 1

                For Each relevantModule As KeyValuePair(Of [Module], Boolean) In harnessWithRelevantModules.Value
                    .Rows(rowCounter).Cells(0).Value = relevantModule.Key.Part_number
                    .Rows(rowCounter).Cells(1).Value = If(relevantModule.Key.Abbreviation IsNot Nothing, relevantModule.Key.Abbreviation, String.Empty)
                    .Rows(rowCounter).Cells(2).Value = If(relevantModule.Key.Description IsNot Nothing, relevantModule.Key.Description, String.Empty)
                    .Rows(rowCounter).Cells(3).Value = If(relevantModule.Value, "Yes", "No")

                    rowCounter += 1
                Next
            End With
        Next

        workbook.Save(fileName)
    End Sub

    Private Function GetAssignedModules(kblMapper As KblMapper, id As String) As Dictionary(Of [Module], Boolean)
        Dim assignedModules As New Dictionary(Of [Module], Boolean)

        If (kblMapper.KBLObjectModuleMapper.ContainsKey(id)) Then
            For Each moduleId As String In kblMapper.KBLObjectModuleMapper(id)
                Dim assignedModule As [Module] = TryCast(kblMapper.KBLOccurrenceMapper(moduleId), [Module])
                If (assignedModule IsNot Nothing) AndAlso (Not assignedModules.ContainsKey(assignedModule)) Then assignedModules.Add(assignedModule, Not kblMapper.InactiveModules.ContainsKey(assignedModule.SystemId))
            Next
        End If

        Return assignedModules
    End Function

    Private Function GetRelevantModules() As SortedDictionary(Of String, Dictionary(Of [Module], Boolean))
        Dim allInlinerPairs As New List(Of InlinerPair)
        Dim harnessesWithRelevantModules As New SortedDictionary(Of String, Dictionary(Of [Module], Boolean))

        allInlinerPairs.AddRange(_trivialInlinerPairs)
        allInlinerPairs.AddRange(_virtualInlinerPairs)

        For Each inlinerPair As InlinerPair In allInlinerPairs
            Dim inliners As New Dictionary(Of KblMapper, Connector_occurrence)

            If (TypeOf inlinerPair Is TrivialInlinerPair) Then
                Dim inlPair As TrivialInlinerPair = DirectCast(inlinerPair, TrivialInlinerPair)

                If (inlPair.ConnectorOccA IsNot Nothing) Then
                    inliners.Add(inlPair.KblMapperA, inlPair.ConnectorOccA)
                End If
                If (inlPair.ConnectorOccB IsNot Nothing) Then
                    inliners.Add(inlPair.KblMapperB, inlPair.ConnectorOccB)
                End If
            Else
                Dim inlPair As VirtualInlinerPair = DirectCast(inlinerPair, VirtualInlinerPair)

                If (inlPair.ActiveConnectorOccsWithKblMapperA.Count > 0) Then
                    inlPair.ActiveConnectorOccsWithKblMapperA.ForEach(Sub(c) inliners.Add(c.Value, c.Key))
                End If
                If (inlPair.ActiveConnectorOccsWithKblMapperB.Count > 0) Then
                    inlPair.ActiveConnectorOccsWithKblMapperB.ForEach(Sub(c) inliners.Add(c.Value, c.Key))
                End If
            End If

            For Each inlinerMap As KeyValuePair(Of KblMapper, Connector_occurrence) In inliners
                Dim con As Connector_occurrence = inlinerMap.Value
                Dim kblMapper As KblMapper = inlinerMap.Key

                If (Not harnessesWithRelevantModules.ContainsKey(kblMapper.HarnessPartNumber)) Then
                    harnessesWithRelevantModules.Add(kblMapper.HarnessPartNumber, New Dictionary(Of [Module], Boolean))
                End If

                For Each assignedModule As KeyValuePair(Of [Module], Boolean) In GetAssignedModules(kblMapper, con.SystemId)
                    If (Not harnessesWithRelevantModules(kblMapper.HarnessPartNumber).ContainsKey(assignedModule.Key)) Then
                        harnessesWithRelevantModules(kblMapper.HarnessPartNumber).Add(assignedModule.Key, assignedModule.Value)
                    End If
                Next

                If (con.Contact_points IsNot Nothing) Then
                    For Each contactPoint As Contact_point In con.Contact_points
                        Dim terminalPartNumber As String = String.Empty
                        Dim terminalPlating As String = String.Empty

                        If (contactPoint.Associated_parts IsNot Nothing) Then
                            For Each associatedPartId As String In contactPoint.Associated_parts.SplitSpace
                                If (kblMapper.KBLOccurrenceMapper.ContainsKey(associatedPartId)) AndAlso (TypeOf kblMapper.KBLOccurrenceMapper(associatedPartId) Is Special_terminal_occurrence OrElse TypeOf kblMapper.KBLOccurrenceMapper(associatedPartId) Is Terminal_occurrence) Then
                                    For Each assignedModule As KeyValuePair(Of [Module], Boolean) In GetAssignedModules(kblMapper, associatedPartId)
                                        If (Not harnessesWithRelevantModules(kblMapper.HarnessPartNumber).ContainsKey(assignedModule.Key)) Then
                                            harnessesWithRelevantModules(kblMapper.HarnessPartNumber).Add(assignedModule.Key, assignedModule.Value)
                                        End If
                                    Next
                                End If
                            Next
                        End If

                        If (kblMapper.KBLContactPointWireMapper.ContainsKey(contactPoint.SystemId)) Then
                            For Each wireId As String In kblMapper.KBLContactPointWireMapper(contactPoint.SystemId)
                                If (kblMapper.KBLOccurrenceMapper.ContainsKey(wireId)) Then
                                    For Each assignedModule As KeyValuePair(Of [Module], Boolean) In GetAssignedModules(kblMapper, wireId)
                                        If (Not harnessesWithRelevantModules(kblMapper.HarnessPartNumber).ContainsKey(assignedModule.Key)) Then
                                            harnessesWithRelevantModules(kblMapper.HarnessPartNumber).Add(assignedModule.Key, assignedModule.Value)
                                        End If
                                    Next
                                End If
                            Next
                        End If
                    Next
                End If
            Next
        Next

        Return harnessesWithRelevantModules
    End Function

    Private Function GetTooltipText(inlinerPair As InlinerPair) As String
        Dim inlinerMappingsA As Dictionary(Of Connector_occurrence, KblMapper) = If(TypeOf inlinerPair Is TrivialInlinerPair, If(DirectCast(inlinerPair, TrivialInlinerPair).ConnectorOccA IsNot Nothing, New Dictionary(Of Connector_occurrence, KblMapper) From {{DirectCast(inlinerPair, TrivialInlinerPair).ConnectorOccA, DirectCast(inlinerPair, TrivialInlinerPair).KblMapperA}}, Nothing), DirectCast(inlinerPair, VirtualInlinerPair).ActiveConnectorOccsWithKblMapperA)
        Dim inlinerMappingsB As Dictionary(Of Connector_occurrence, KblMapper) = If(TypeOf inlinerPair Is TrivialInlinerPair, If(DirectCast(inlinerPair, TrivialInlinerPair).ConnectorOccB IsNot Nothing, New Dictionary(Of Connector_occurrence, KblMapper) From {{DirectCast(inlinerPair, TrivialInlinerPair).ConnectorOccB, DirectCast(inlinerPair, TrivialInlinerPair).KblMapperB}}, Nothing), DirectCast(inlinerPair, VirtualInlinerPair).ActiveConnectorOccsWithKblMapperB)
        Dim tooltipText As New StringBuilder

        If (inlinerMappingsA IsNot Nothing) Then
            For Each inlinerMap As KeyValuePair(Of Connector_occurrence, KblMapper) In inlinerMappingsA.OrderBy(Function(i) i.Key.Id)
                Dim housing As Connector_Housing = TryCast(inlinerMap.Value.KBLPartMapper(inlinerMap.Key.Part), Connector_Housing)

                If (tooltipText.ToString <> String.Empty) Then tooltipText.AppendLine("########################################")

                tooltipText.AppendLine(inlinerMap.Key.Id)
                tooltipText.AppendLine("----------------------------------------")
                tooltipText.AppendLine(String.Format("{0}: {1}", ObjectPropertyNameStrings.PartNumber, If(housing IsNot Nothing, housing.Part_number, "-")))
                tooltipText.AppendLine(String.Format("{0}: {1}", ObjectPropertyNameStrings.PartDescription, If(housing IsNot Nothing, housing.Description, "-")))
                tooltipText.AppendLine(String.Format("{0}: {1}", ObjectPropertyNameStrings.Description, inlinerMap.Key.Description))
                tooltipText.AppendLine(String.Format("{0}: {1}", ObjectPropertyNameStrings.Abbreviation, If(housing IsNot Nothing, housing.Abbreviation, "-")))
                tooltipText.AppendLine(String.Format("{0}: {1}", ObjectPropertyNameStrings.HousingColor, If(housing IsNot Nothing, housing.Housing_colour, "-")))
                tooltipText.AppendLine(String.Format("{0}: {1}", ObjectPropertyNameStrings.HousingType, If(housing IsNot Nothing, housing.Housing_type, "-")))
            Next
        End If

        If (inlinerMappingsB IsNot Nothing) Then
            For Each inlinerMap As KeyValuePair(Of Connector_occurrence, KblMapper) In inlinerMappingsB.OrderBy(Function(i) i.Key.Id)
                Dim housing As Connector_Housing = TryCast(inlinerMap.Value.KBLPartMapper(inlinerMap.Key.Part), Connector_Housing)

                If (tooltipText.ToString <> String.Empty) Then tooltipText.AppendLine("########################################")

                tooltipText.AppendLine(inlinerMap.Key.Id)
                tooltipText.AppendLine("----------------------------------------")
                tooltipText.AppendLine(String.Format("{0}: {1}", ObjectPropertyNameStrings.PartNumber, If(housing IsNot Nothing, housing.Part_number, "-")))
                tooltipText.AppendLine(String.Format("{0}: {1}", ObjectPropertyNameStrings.PartDescription, If(housing IsNot Nothing, housing.Description, "-")))
                tooltipText.AppendLine(String.Format("{0}: {1}", ObjectPropertyNameStrings.Description, inlinerMap.Key.Description))
                tooltipText.AppendLine(String.Format("{0}: {1}", ObjectPropertyNameStrings.Abbreviation, If(housing IsNot Nothing, housing.Abbreviation, "-")))
                tooltipText.AppendLine(String.Format("{0}: {1}", ObjectPropertyNameStrings.HousingColor, If(housing IsNot Nothing, housing.Housing_colour, "-")))
                tooltipText.AppendLine(String.Format("{0}: {1}", ObjectPropertyNameStrings.HousingType, If(housing IsNot Nothing, housing.Housing_type, "-")))
            Next
        End If

        Return tooltipText.ToString
    End Function

    Private Sub OnContextMenuItemClicked(sender As Object, e As ToolStripItemClickedEventArgs)
        Me.Cursor = Cursors.WaitCursor

        With Me.uebInliners
            .BeginUpdate()

            Select Case e.ClickedItem.Text
                Case InlinerFormStrings.CollapseAll_CtxtMnu_Caption
                    .Groups.CollapseAll()
                Case InlinerFormStrings.ExpandAll_CtxtMnu_Caption
                    .Groups.ExpandAll()
            End Select

            .EndUpdate()
        End With

        Me.Cursor = Cursors.Default
    End Sub

    Private Sub OnInlinerPairDetailsMouseClick(sender As Object, e As InformationHubEventArgs)
        RaiseEvent InlinerSelectionChanged(sender, e)
    End Sub

    Protected Overrides Sub OnShown(e As EventArgs)
        MyBase.OnShown(e)
        Dim activeInlinerPairs As Integer = 0

        Dim virtualInlines As VirtualInlinerPair() = _virtualInlinerPairs.OrderBy(Function(p) p.Id).ToArray
        Dim trivialInliners As TrivialInlinerPair() = _trivialInlinerPairs.OrderBy(Function(p) p.Id).ToArray

        UltraProgressBar1.Maximum = virtualInlines.Length + trivialInliners.Length
        UltraProgressBar1.Step = 1

        Me.btnClose.Enabled = False
        Me.btnExport.Enabled = False

        SetProgressVisibility(True)

        With Me.uebInliners
            .BeginUpdate()

            .ContextMenuStrip = New ContextMenuStrip
            .ContextMenuStrip.Items.Add(InlinerFormStrings.CollapseAll_CtxtMnu_Caption, My.Resources.CollapseAll.ToBitmap)
            .ContextMenuStrip.Items.Add(InlinerFormStrings.ExpandAll_CtxtMnu_Caption, My.Resources.ExpandAll.ToBitmap)

            AddHandler .ContextMenuStrip.ItemClicked, AddressOf OnContextMenuItemClicked

            For Each virtualInlinerPair As VirtualInlinerPair In virtualInlines
                If (virtualInlinerPair.ConnectorOccsWithKblMapperB.Count = 0) Then
                    Continue For
                End If

                AddInlinerPairDetailGroup(virtualInlinerPair, _inlinerPairCheckClassifications)
                ' uebInliners.Update()
                activeInlinerPairs += 1
                UltraProgressBar1.PerformStep()
                'Me.Update()
            Next

            For Each trivialInlinerPair As TrivialInlinerPair In trivialInliners
                If (trivialInlinerPair.InlinerIdB Is Nothing) Then
                    Continue For
                End If

                AddInlinerPairDetailGroup(trivialInlinerPair, _inlinerPairCheckClassifications)
                'uebInliners.Update()
                activeInlinerPairs += 1
                UltraProgressBar1.PerformStep()
                'Me.Update()
            Next

            SetProgressVisibility(False)


            .Groups.ExpandAll()
            .ShowDefaultContextMenu = False
            .EndUpdate()
        End With

        Me.btnClose.Enabled = True
        Me.btnExport.Enabled = True

        _harnessesWithRelevantModules = GetRelevantModules()

        Me.lblInfo.Text = String.Format(InlinerFormStrings.Info_Label, activeInlinerPairs, _inactiveInlinerPairs)
    End Sub

    Private Sub SetProgressVisibility(visible As Boolean)
        upbInliners.Visible = Not visible
        ProgressPanel.Visible = visible
        Me.Update()
    End Sub

    Private Sub InlinersForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        RemoveHandler Me.uebInliners.ContextMenuStrip.ItemClicked, AddressOf OnContextMenuItemClicked

        For Each group As UltraExplorerBarGroup In Me.uebInliners.Groups
            RemoveHandler DirectCast(group.Container.Controls(0), InlinerPairDetails).InlinerPairDetailsMouseClick, AddressOf OnInlinerPairDetailsMouseClick
        Next
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        Using sfdExcel As New SaveFileDialog
            With sfdExcel
                .DefaultExt = KnownFile.XLSX.Trim("."c)

                .FileName = String.Format("{0}{1}{2}_Inliner_pairs.xlsx", Now.Year, Format(Now.Month, "00"), Format(Now.Day, "00"))
                .Filter = "Excel files (*.xlsx)|*.xlsx|Excel files (97-2003) (*.xls)|*.xls"
                .Title = InlinerFormStrings.ExportExcelFile_Title

                If (.ShowDialog(Me) = DialogResult.OK) Then
                    Try
                        ExportToExcel(.FileName)

                        If (MessageBox.Show(InlinerFormStrings.ExportExcelSuccess_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) = System.Windows.Forms.DialogResult.Yes) Then
                            ProcessEx.Start(.FileName)
                        End If
                    Catch ex As Exception
                        MessageBox.Show(String.Format(InlinerFormStrings.ExportExcelError_Msg, vbCrLf, ex.Message), [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Try
                End If
            End With
        End Using
    End Sub

    Private Sub uebInliners_MouseWheel(sender As Object, e As MouseEventArgs) Handles uebInliners.MouseWheel
        Dim args As HandledMouseEventArgs = TryCast(e, HandledMouseEventArgs)
        If (args IsNot Nothing) Then args.Handled = True
    End Sub

End Class