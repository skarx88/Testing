Imports System.IO
Imports System.Text
Imports Infragistics.Documents.Excel
Imports Infragistics.Win.UltraWinExplorerBar
Imports Infragistics.Win.UltraWinGrid
Imports Zuken.E3.HarnessAnalyzer.Settings

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class UltrasonicSpliceTerminalDistanceForm

    Friend Event UltrasonicSpliceSelectionChanged(sender As Object, e As InformationHubEventArgs)

    Private _kblMapper As KblMapper = Nothing
    Private _ultrasonicSpliceIdentifiers As UltrasonicSpliceIdentifierCollection = Nothing

    Public Sub New(defaultWireLengthType As String, kblMapper As KblMapper, ultrasonicSpliceIdentifiers As UltrasonicSpliceIdentifierCollection, ultraSonicSpliceTerminalDistanceMapping As UltrasonicSpliceTerminalDistanceMapping)
        InitializeComponent()

        Me.Icon = My.Resources.Checks_UltraSonicSpliceTerminalDistance
        Me.Text = String.Format("{0} [{1}]", Me.Text, If(Not String.IsNullOrEmpty(kblMapper.GetHarness.Description), String.Format("{0} ({1})", kblMapper.HarnessPartNumber, kblMapper.GetHarness.Description), kblMapper.HarnessPartNumber))

        _kblMapper = kblMapper
        _ultrasonicSpliceIdentifiers = ultrasonicSpliceIdentifiers

        Dim ultrasonicSpliceCounter As Integer = 0
        Dim ultrasonicSplices As SortedList(Of String, Connector_occurrence) = GetUltrasonicSplices()

        With Me.uebUSSpliceTerminalDistances
            .BeginUpdate()

            .ContextMenuStrip = New ContextMenuStrip
            .ContextMenuStrip.Items.Add(UltrasonicSpliceTerminalDistanceFormStrings.CollapseAll_CtxtMnu_Caption, My.Resources.CollapseAll.ToBitmap)
            .ContextMenuStrip.Items.Add(UltrasonicSpliceTerminalDistanceFormStrings.ExpandAll_CtxtMnu_Caption, My.Resources.ExpandAll.ToBitmap)

            AddHandler .ContextMenuStrip.ItemClicked, AddressOf OnContextMenuItemClicked

            For Each ultrasonicSplice As Connector_occurrence In ultrasonicSplices.Values.Where(Function(u) u.Contact_points.Length <> 0)
                Dim ultrasonicSpliceDetails As New UltrasonicSpliceDetails(defaultWireLengthType, _kblMapper, ultrasonicSplice, ultraSonicSpliceTerminalDistanceMapping)
                ultrasonicSpliceDetails.Dock = DockStyle.Fill

                AddHandler ultrasonicSpliceDetails.UltrasonicSpliceDetailsMouseClick, AddressOf OnUltrasonicSpliceDetailsMouseClick

                Dim containerCtrl As New UltraExplorerBarContainerControl
                With containerCtrl
                    .Controls.Add(ultrasonicSpliceDetails)
                    .Name = "UltrasonicSpliceDetails"
                End With

                .Controls.Add(containerCtrl)

                Dim group As New UltraExplorerBarGroup(String.Format("{0} [{1}]", ultrasonicSplice.Id, ultrasonicSplice.Description))
                With group
                    .Container = containerCtrl
                    .Expanded = False
                    .Settings.AppearancesSmall.HeaderAppearance.Image = My.Resources.StatusGreen
                    .Settings.ContainerHeight = 300
                    .Settings.Style = GroupStyle.ControlContainer
                    .Tag = ultrasonicSplice
                    .Text = String.Format("{0} [{1}]", ultrasonicSplice.Id, If(ultrasonicSplice.Description IsNot Nothing AndAlso ultrasonicSplice.Description <> String.Empty, ultrasonicSplice.Description, "-"))
                    .ToolTipText = GetTooltipText(ultrasonicSplice)
                End With

                .Groups.Add(group)

                ultrasonicSpliceDetails.Initialize()

                ultrasonicSpliceCounter += 1
            Next

            .Groups.ExpandAll()
            .ShowDefaultContextMenu = False

            .EndUpdate()
        End With

        Me.lblInfo.Text = String.Format(UltrasonicSpliceTerminalDistanceFormStrings.Info_Label, ultrasonicSpliceCounter)

        If (ultrasonicSpliceCounter < ultrasonicSplices.Count) Then
            Dim emptyUltrasonicSplices As String = String.Empty

            For Each ultrasonicSplice As Connector_occurrence In ultrasonicSplices.Values.Where(Function(u) u.Contact_points.Length = 0)
                If (emptyUltrasonicSplices = String.Empty) Then
                    emptyUltrasonicSplices = ultrasonicSplice.Id
                Else
                    emptyUltrasonicSplices = String.Format("{0}, {1}", emptyUltrasonicSplices, ultrasonicSplice.Id)
                End If
            Next

            MessageBoxEx.ShowWarning(Me, String.Format(UltrasonicSpliceTerminalDistanceFormStrings.UnconnectedUltrasonicSplices_Msg, ultrasonicSplices.Count - ultrasonicSpliceCounter, vbCrLf, emptyUltrasonicSplices))
        End If
    End Sub


    Private Sub ExportToExcel(fileName As String)
        Dim workbook As New Workbook

        If (IO.Path.GetExtension(fileName).ToLower = ".xlsx") Then
            workbook.SetCurrentFormat(WorkbookFormat.Excel2007)
        Else
            workbook.SetCurrentFormat(WorkbookFormat.Excel97To2003)
        End If

        workbook.Worksheets.Add("USSpliceTerminalDistances")

        With workbook.Worksheets("USSpliceTerminalDistances")
            Dim rowCounter As Integer = 0

            .DefaultColumnWidth = 4000

            For Each group As UltraExplorerBarGroup In Me.uebUSSpliceTerminalDistances.Groups
                Dim ultrasonicSplice As Connector_occurrence = DirectCast(group.Tag, Connector_occurrence)
                Dim ultrasonicSpliceDetails As UltrasonicSpliceDetails = DirectCast(group.Container.Controls(0), UltrasonicSpliceDetails)

                .Rows(rowCounter).CellFormat.Font.Bold = ExcelDefaultableBoolean.True

                .Rows(rowCounter).Cells(0).CellFormat.TopBorderStyle = CellBorderLineStyle.Thin
                .Rows(rowCounter).Cells(1).Value = UltrasonicSpliceTerminalDistanceFormStrings.Wire_ExcelCellCaption
                .Rows(rowCounter).Cells(7).Value = UltrasonicSpliceTerminalDistanceFormStrings.CounterConnector_ExcelCellCaption

                .MergedCellsRegions.Add(rowCounter, 1, rowCounter, 6).CellFormat.TopBorderStyle = CellBorderLineStyle.Thin
                .MergedCellsRegions.Add(rowCounter, 7, rowCounter, 10).CellFormat.TopBorderStyle = CellBorderLineStyle.Thin

                rowCounter += 1

                .Rows(rowCounter).CellFormat.Font.Italic = ExcelDefaultableBoolean.True
                .Rows(rowCounter).Cells(1).Value = String.Format("{0} [{1}]", ultrasonicSplice.Id, If(ultrasonicSplice.Description IsNot Nothing AndAlso ultrasonicSplice.Description <> String.Empty, ultrasonicSplice.Description, "-"))

                .MergedCellsRegions.Add(rowCounter, 1, rowCounter, 6)
                .MergedCellsRegions.Add(rowCounter, 7, rowCounter, 10)

                For Each row As UltraGridRow In ultrasonicSpliceDetails.ugUltrasonicSplice.Rows
                    rowCounter += 1

                    Dim colCounter As Integer = 0

                    If (row.Index = 0) Then
                        .Rows(rowCounter).CellFormat.Font.Bold = ExcelDefaultableBoolean.True

                        For Each column As UltraGridColumn In ultrasonicSpliceDetails.ugUltrasonicSplice.DisplayLayout.Bands(0).Columns
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

        workbook.Save(fileName)
    End Sub

    Private Function GetUltrasonicSplices() As SortedList(Of String, Connector_occurrence)
        Dim ultrasonicSplices As New SortedList(Of String, Connector_occurrence)

        For Each connectorOccurrence As Connector_occurrence In _kblMapper.GetConnectorOccurrences
            Dim connectorHousing As Connector_Housing = _kblMapper.GetPart(Of Connector_Housing)(connectorOccurrence.Part) ' will be nothing of not anything is found 
            Dim idCriteriaMatches As Boolean = True
            Dim aliasIdCriteriaMatches As Boolean = True
            Dim descriptionCriteriaMatches As Boolean = True
            Dim installationInformationCriteriaMatches As Boolean = True
            Dim partNumberCriteriaMatches As Boolean = True
            Dim partDescriptionCriteriaMatches As Boolean = True
            Dim partProcessingInformationCriteriaMatches As Boolean = True

            With connectorOccurrence
                For Each ultrasonicSpliceIdentifier As UltrasonicSpliceIdentifier In _ultrasonicSpliceIdentifiers
                    Select Case ultrasonicSpliceIdentifier.KBLPropertyName
                        Case ObjectPropertyNameStrings.Id
                            idCriteriaMatches = IsUltrasonicSpliceIdentifierMatching(ultrasonicSpliceIdentifier.IdentificationCriteria, .Id)
                        Case ObjectPropertyNameStrings.AliasId
                            If (.Alias_id.Length <> 0) Then
                                aliasIdCriteriaMatches = False
                                For Each aliasId As Alias_identification In .Alias_id
                                    aliasIdCriteriaMatches = IsUltrasonicSpliceIdentifierMatching(ultrasonicSpliceIdentifier.IdentificationCriteria, aliasId.Alias_id)
                                    If (aliasIdCriteriaMatches) Then
                                        Exit For
                                    End If
                                Next
                            Else
                                aliasIdCriteriaMatches = False
                            End If
                        Case ObjectPropertyNameStrings.Description
                            If (.Description IsNot Nothing) Then
                                descriptionCriteriaMatches = IsUltrasonicSpliceIdentifierMatching(ultrasonicSpliceIdentifier.IdentificationCriteria, .Description)
                            Else
                                descriptionCriteriaMatches = False
                            End If
                        Case ObjectPropertyNameStrings.InstallationInformation
                            If (.Installation_information.Length <> 0) Then
                                installationInformationCriteriaMatches = False
                                For Each installationInformation As Installation_instruction In .Installation_information
                                    installationInformationCriteriaMatches = IsUltrasonicSpliceIdentifierMatching(ultrasonicSpliceIdentifier.IdentificationCriteria, installationInformation.Instruction_value)
                                    If (installationInformationCriteriaMatches) Then
                                        Exit For
                                    End If
                                Next
                            Else
                                installationInformationCriteriaMatches = False
                            End If
                        Case ObjectPropertyNameStrings.PartNumber
                            partNumberCriteriaMatches = If(connectorHousing IsNot Nothing, IsUltrasonicSpliceIdentifierMatching(ultrasonicSpliceIdentifier.IdentificationCriteria, connectorHousing.Part_number), False)
                        Case ObjectPropertyNameStrings.PartDescription
                            partDescriptionCriteriaMatches = If(connectorHousing IsNot Nothing, IsUltrasonicSpliceIdentifierMatching(ultrasonicSpliceIdentifier.IdentificationCriteria, connectorHousing.Description), False)
                        Case ObjectPropertyNameStrings.PartProcessingInformation
                            If (connectorHousing IsNot Nothing) AndAlso (connectorHousing.Processing_information.Length <> 0) Then
                                partProcessingInformationCriteriaMatches = False

                                For Each processingInformation As Processing_instruction In connectorHousing.Processing_information
                                    partProcessingInformationCriteriaMatches = IsUltrasonicSpliceIdentifierMatching(ultrasonicSpliceIdentifier.IdentificationCriteria, processingInformation.Instruction_value)
                                    If (partProcessingInformationCriteriaMatches) Then
                                        Exit For
                                    End If
                                Next
                            Else
                                partProcessingInformationCriteriaMatches = False
                            End If
                    End Select
                Next
            End With

            Dim isKSL As Boolean = False
            Dim conn_housing As Connector_Housing = _kblMapper.GetPart(Of Connector_Housing)(connectorOccurrence.Part)
            If Not String.IsNullOrEmpty(connectorHousing?.Description) Then
                isKSL = conn_housing.IsKSL
            End If

            If (_ultrasonicSpliceIdentifiers.Count <> 0) AndAlso (idCriteriaMatches AndAlso aliasIdCriteriaMatches AndAlso descriptionCriteriaMatches AndAlso installationInformationCriteriaMatches AndAlso partNumberCriteriaMatches AndAlso partDescriptionCriteriaMatches AndAlso partProcessingInformationCriteriaMatches) AndAlso (Not isKSL) AndAlso (Not ultrasonicSplices.ContainsKey(connectorOccurrence.Id)) Then
                ultrasonicSplices.Add(connectorOccurrence.Id, connectorOccurrence)
            End If
        Next

        Return ultrasonicSplices
    End Function

    Private Function GetTooltipText(ultrasonicSplice As Connector_occurrence) As String
        Dim connectorHousing As Connector_Housing = TryCast(_kblMapper.KBLPartMapper(ultrasonicSplice.Part), Connector_Housing)
        Dim tooltipText As New StringBuilder

        tooltipText.AppendLine(ultrasonicSplice.Id)
        tooltipText.AppendLine("----------------------------------------")
        tooltipText.AppendLine(String.Format("{0}: {1}", ObjectPropertyNameStrings.PartNumber, If(connectorHousing IsNot Nothing, connectorHousing.Part_number, "-")))
        tooltipText.AppendLine(String.Format("{0}: {1}", ObjectPropertyNameStrings.PartDescription, If(connectorHousing IsNot Nothing, connectorHousing.Description, "-")))
        tooltipText.AppendLine(String.Format("{0}: {1}", ObjectPropertyNameStrings.Description, ultrasonicSplice.Description))
        tooltipText.AppendLine(String.Format("{0}: {1}", ObjectPropertyNameStrings.Abbreviation, If(connectorHousing IsNot Nothing, connectorHousing.Abbreviation, "-")))
        tooltipText.AppendLine(String.Format("{0}: {1}", ObjectPropertyNameStrings.HousingColor, If(connectorHousing IsNot Nothing, connectorHousing.Housing_colour, "-")))
        tooltipText.AppendLine(String.Format("{0}: {1}", ObjectPropertyNameStrings.HousingType, If(connectorHousing IsNot Nothing, connectorHousing.Housing_type, "-")))

        Return tooltipText.ToString
    End Function

    Private Function IsUltrasonicSpliceIdentifierMatching(identificationCriteria As String, propertyValue As String) As Boolean
        If (identificationCriteria <> "*") Then
            If (identificationCriteria.StartsWith("*"c)) Then
                If (identificationCriteria.EndsWith("*"c)) Then
                    If (Not propertyValue.Contains(identificationCriteria)) Then
                        Return False
                    End If
                ElseIf (Not propertyValue.EndsWith(identificationCriteria.Substring(1))) Then
                    Return False
                End If
            ElseIf (identificationCriteria.EndsWith("*"c)) Then
                If (Not propertyValue.StartsWith(identificationCriteria.Substring(0, identificationCriteria.Length - 1))) Then
                    Return False
                End If
            ElseIf (identificationCriteria <> propertyValue) Then
                Return False
            End If
        End If

        Return True
    End Function

    Private Sub OnContextMenuItemClicked(sender As Object, e As ToolStripItemClickedEventArgs)
        Me.Cursor = Cursors.WaitCursor

        With Me.uebUSSpliceTerminalDistances
            .BeginUpdate()
            .EventManager.AllEventsEnabled = False

            Select Case e.ClickedItem.Text
                Case UltrasonicSpliceTerminalDistanceFormStrings.CollapseAll_CtxtMnu_Caption
                    .Groups.CollapseAll()
                Case UltrasonicSpliceTerminalDistanceFormStrings.ExpandAll_CtxtMnu_Caption
                    .Groups.ExpandAll()
            End Select

            .EventManager.AllEventsEnabled = True
            .EndUpdate()
        End With

        Me.Cursor = Cursors.Default
    End Sub

    Private Sub OnUltrasonicSpliceDetailsMouseClick(sender As Object, e As InformationHubEventArgs)
        RaiseEvent UltrasonicSpliceSelectionChanged(sender, e)
    End Sub


    Private Sub UltrasonicSpliceTerminalDistanceForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        RemoveHandler Me.uebUSSpliceTerminalDistances.ContextMenuStrip.ItemClicked, AddressOf OnContextMenuItemClicked

        For Each group As UltraExplorerBarGroup In Me.uebUSSpliceTerminalDistances.Groups
            RemoveHandler DirectCast(group.Container.Controls(0), UltrasonicSpliceDetails).UltrasonicSpliceDetailsMouseClick, AddressOf OnUltrasonicSpliceDetailsMouseClick
        Next
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        Using sfdExcel As New SaveFileDialog
            With sfdExcel
                .DefaultExt = KnownFile.XLSX.Trim("."c)
                .FileName = String.Format("{0}{1}{2}_Ultrasonic_splice_terminal_distances.xlsx", Now.Year, Format(Now.Month, "00"), Format(Now.Day, "00"))
                .Filter = "Excel files (*.xlsx)|*.xlsx|Excel files (97-2003) (*.xls)|*.xls"
                .Title = UltrasonicSpliceTerminalDistanceFormStrings.ExportExcelFile_Title

                If (.ShowDialog(Me) = DialogResult.OK) Then
                    Try
                        ExportToExcel(.FileName)

                        If (MessageBox.Show(UltrasonicSpliceTerminalDistanceFormStrings.ExportExcelSuccess_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) = System.Windows.Forms.DialogResult.Yes) Then ProcessEx.Start(.FileName)
                    Catch ex As Exception
                        MessageBox.Show(String.Format(UltrasonicSpliceTerminalDistanceFormStrings.ExportExcelError_Msg, vbCrLf, ex.Message), [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Try
                End If
            End With
        End Using
    End Sub

End Class