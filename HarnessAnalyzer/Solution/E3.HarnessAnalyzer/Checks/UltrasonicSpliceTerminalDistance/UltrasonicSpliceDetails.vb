Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinGrid

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class UltrasonicSpliceDetails

    Friend Event UltrasonicSpliceDetailsMouseClick(sender As Object, e As InformationHubEventArgs)

    Private _defaultWireLengthType As String = String.Empty
    Private _kblMapper As KblMapper = Nothing
    Private _ultrasonicSplice As Connector_occurrence = Nothing
    Private _ultrasonicSpliceTerminalDistanceMapping As UltrasonicSpliceTerminalDistanceMapping = Nothing

    Public Sub New(defaultWireLengthType As String, kblMapper As KblMapper, ultrasonicSplice As Connector_occurrence, ultrasonicSpliceTerminalDistanceMapping As UltrasonicSpliceTerminalDistanceMapping)
        InitializeComponent()

        _defaultWireLengthType = defaultWireLengthType
        _kblMapper = kblMapper
        _ultrasonicSplice = ultrasonicSplice
        _ultrasonicSpliceTerminalDistanceMapping = ultrasonicSpliceTerminalDistanceMapping
    End Sub

    Public Sub Initialize()
        InitializeGrid()
    End Sub

    Private Sub HighlightErrorsAndWarnings()
        Dim hasErrors As Boolean = False
        Dim hasWarnings As Boolean = False

        With Me.ugUltrasonicSplice
            .BeginUpdate()

            For Each row As UltraGridRow In .Rows
                Dim hasErrs As Boolean = False
                Dim hasWrngs As Boolean = False

                Dim ultrasonicSpliceTerminalDistanceMap As UltrasonicSpliceTerminalDistanceMap = _ultrasonicSpliceTerminalDistanceMapping.UltrasonicSpliceTerminalDistanceMaps.FindUltrasonicSpliceTerminalDistanceMapFromPartNumber(row.Cells("CounterTerminal").Value.ToString)
                If (ultrasonicSpliceTerminalDistanceMap IsNot Nothing) Then
                    Dim length As Single = CSng(row.Cells("LengthVal").Value)
                    If (length < ultrasonicSpliceTerminalDistanceMap.MinDistance) Then
                        row.Cells("Length").Appearance.BackColor = Color.FromArgb(100, Color.Red)
                        row.Cells("Length").Appearance.Image = My.Resources.Error_Small
                        row.Cells("Length").ToolTipText = String.Format("{0}{1}", UltrasonicSpliceDetailsStrings.Error_TooltipText, String.Format(UltrasonicSpliceDetailsStrings.LengthFallsBelowMinimumAllowedDistance_TooltipText, ultrasonicSpliceTerminalDistanceMap.MinDistance))

                        hasErrs = True
                    End If
                ElseIf (row.Cells("CounterTerminal").Value.ToString <> String.Empty) Then
                    row.Cells("CounterTerminal").Appearance.BackColor = Color.FromArgb(100, Color.Yellow)
                    row.Cells("CounterTerminal").Appearance.Image = My.Resources.MismatchingConfig_Small
                    row.Cells("CounterTerminal").ToolTipText = String.Format("{0}{1}", UltrasonicSpliceDetailsStrings.Warning_TooltipText, UltrasonicSpliceDetailsStrings.TerminalNotFound_TooltipText)

                    hasWrngs = True
                ElseIf (row.Cells("CounterTerminal").Value.ToString = String.Empty) Then
                    row.Cells("CounterTerminal").Appearance.Image = My.Resources.About_Small
                    row.Cells("CounterTerminal").ToolTipText = String.Format("{0}{1}", UltrasonicSpliceDetailsStrings.Info_TooltipText, UltrasonicSpliceDetailsStrings.TerminalPartNumberEmpty_TooltipText)
                End If

                If (hasErrs) Then hasErrors = True
                If (hasWrngs) Then hasWarnings = True
            Next

            If (hasErrors) Then
                DirectCast(Me.Parent.Parent, Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar).Groups(String.Format("{0} [{1}]", _ultrasonicSplice.Id, _ultrasonicSplice.Description)).Settings.AppearancesSmall.HeaderAppearance.Image = My.Resources.StatusRed
            ElseIf (hasWarnings) Then
                DirectCast(Me.Parent.Parent, Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar).Groups(String.Format("{0} [{1}]", _ultrasonicSplice.Id, _ultrasonicSplice.Description)).Settings.AppearancesSmall.HeaderAppearance.Image = My.Resources.StatusYellow
            End If

            .EndUpdate()
        End With
    End Sub

    Private Sub InitializeGrid()
        Me.ugUltrasonicSplice.SyncWithCurrencyManager = False

        With Me.udsUltrasonicSplice
            .Band.Key = "UltrasonicSplice"

            With .Band
                .Columns.Add(UltrasonicSpliceDetailsStrings.CavNum_ColumnCaption)
                .Columns.Add("SeparatorColumn1")
                .Columns.Add("CavityId")
                .Columns.Add("WireId")
                .Columns.Add("Wire")
                .Columns.Add("Type")
                .Columns.Add("CSA")
                .Columns.Add("Color")
                .Columns.Add("Signal")
                .Columns.Add("Length")
                .Columns.Add("LengthVal")
                .Columns.Add("SeparatorColumn2")
                .Columns.Add("CounterTerminal")
                .Columns.Add("CounterTerminalPlating")
                .Columns.Add("CounterCavity")
                .Columns.Add("CounterCavityId")
                .Columns.Add("CounterConnectorId")
            End With
        End With

        InitializeGridData()
    End Sub

    Private Sub InitializeGridData()
        Me.udsUltrasonicSplice.Rows.Clear()

        If (_ultrasonicSplice.Slots.OrEmpty.Length > 0) Then
            Dim usSpliceHousing As Connector_Housing = _kblMapper.GetConnectorHousing(_ultrasonicSplice.Part)
            If usSpliceHousing IsNot Nothing Then
                For Each cavity_occ As Cavity_occurrence In (_ultrasonicSplice.Slots.FirstOrDefault?.Cavities).OrEmpty
                    Dim cavityNumber As String = _kblMapper.GetPart(Of Cavity)(cavity_occ.Part)?.Cavity_number
                    Dim contactPoints_ofCavity As Contact_point() = _ultrasonicSplice.GetContactsPointsOfCavity(cavity_occ)
                    If contactPoints_ofCavity.Length > 0 Then
                        For Each contactPoint As Contact_point In contactPoints_ofCavity
                            Dim wireCoreOccurrences As IKblWireCoreOccurrence() = contactPoint.GetWireOrCores(_kblMapper)
                            If wireCoreOccurrences.Length > 0 Then
                                For Each wire_core_occ As IKblWireCoreOccurrence In wireCoreOccurrences
                                    Dim wireNumber As String = String.Empty
                                    Dim wireType As String = String.Empty
                                    Dim csa As String = String.Empty
                                    Dim color As String = String.Empty
                                    Dim wire_connection As Connection = _kblMapper.GetConnectionOfWire(wire_core_occ.SystemId)
                                    Dim signalName As String = wire_connection?.Signal_name
                                    Dim length As String = String.Empty
                                    Dim lengthVal As Double = 0
                                    Dim wire_core_Part As IKblWireCorePart = CType(wire_core_occ.GetPart(_kblMapper), IKblWireCorePart)

                                    wireNumber = wire_core_occ.Wire_number
                                    wireType = wire_core_Part.Wire_type
                                    csa = If(wire_core_Part.Cross_section_area IsNot Nothing, String.Format("{0} {1}", Math.Round(wire_core_Part.Cross_section_area.Value_component, 2), _kblMapper.GetUnit(wire_core_Part.Cross_section_area.Unit_component).Unit_name), String.Empty)
                                    color = wire_core_Part.GetColours
                                    length = If(wire_core_occ.GetLength IsNot Nothing, String.Format("{0} {1}", Math.Round(wire_core_occ.GetLength.Value_component, 2), _kblMapper.GetUnit(wire_core_occ.GetLength.Unit_component).Unit_name), Zuken.E3.HarnessAnalyzer.Shared.NOT_AVAILABLE)
                                    lengthVal = If(wire_core_occ.GetLength IsNot Nothing, Math.Round(wire_core_occ.GetLength.Value_component, 2), 0)

                                    Dim counterCavityId As String = String.Empty
                                    Dim counterCavityNumber As String = String.Empty
                                    Dim counterConnectorId As String = String.Empty
                                    Dim counterTerminalPartnumber As String = String.Empty
                                    Dim counterTerminalPlating As String = String.Empty

                                    If wire_connection IsNot Nothing Then
                                        Dim counterCavity As CavityStartEndResult.CavityOccurrencePartPair = wire_connection.GetCounterCavity(contactPoint, _kblMapper)
                                        Dim counterContactPoint As Contact_point = _kblMapper.GetOccurrenceObject(Of Contact_point)(counterCavity.ContactPointId)
                                        counterCavityId = counterCavity?.Cavity_Occurrence?.SystemId
                                        counterCavityNumber = counterCavity?.Cavity_Part?.Cavity_number
                                        counterConnectorId = If(_kblMapper.GetConnectorOfContactPoint(counterCavity.ContactPointId)?.Id, String.Empty)

                                        For Each terminalPart As General_terminal In counterContactPoint.GetTerminalParts(_kblMapper)
                                            counterTerminalPartnumber = If(counterTerminalPartnumber = String.Empty, terminalPart.Part_number, String.Format("{0}, {1}", counterTerminalPartnumber, terminalPart.Part_number))
                                            counterTerminalPlating = If(counterTerminalPlating = String.Empty, terminalPart.Plating_material, String.Format("{0}, {1}", counterTerminalPlating, terminalPart.Plating_material))
                                            'TODO: unclear why to iterate over all terminals and not break after the first is found but the BL from old trunk was so implemented... to be re-evaluated
                                        Next
                                    End If

                                    Dim row As UltraDataRow = Me.udsUltrasonicSplice.Rows.Add
                                    With row
                                        .SetCellValue(UltrasonicSpliceDetailsStrings.CavNum_ColumnCaption, cavityNumber)
                                        .SetCellValue("CavityId", contactPoint.Contacted_cavity)
                                        .SetCellValue("WireId", wire_core_occ.SystemId)
                                        .SetCellValue("Wire", wireNumber)
                                        .SetCellValue("Type", wireType)
                                        .SetCellValue("CSA", csa)
                                        .SetCellValue("Color", color)
                                        .SetCellValue("Signal", signalName)
                                        .SetCellValue("Length", length)
                                        .SetCellValue("LengthVal", lengthVal)
                                        .SetCellValue("CounterTerminal", counterTerminalPartnumber)
                                        .SetCellValue("CounterTerminalPlating", counterTerminalPlating)
                                        .SetCellValue("CounterCavity", counterCavityNumber)
                                        .SetCellValue("CounterCavityId", counterCavityId)
                                        .SetCellValue("CounterConnectorId", counterConnectorId)
                                    End With
                                Next
                            Else
                                Dim row As UltraDataRow = Me.udsUltrasonicSplice.Rows.Add
                                With row
                                    .SetCellValue(UltrasonicSpliceDetailsStrings.CavNum_ColumnCaption, cavityNumber)
                                    .SetCellValue("CavityId", cavity_occ.SystemId)
                                End With
                            End If
                        Next
                    Else
                        Dim row As UltraDataRow = Me.udsUltrasonicSplice.Rows.Add
                        With row
                            .SetCellValue(UltrasonicSpliceDetailsStrings.CavNum_ColumnCaption, cavityNumber)
                            .SetCellValue("CavityId", cavity_occ.SystemId)
                        End With
                    End If
                Next
            End If
        End If

        Me.ugUltrasonicSplice.DataSource = Me.udsUltrasonicSplice
    End Sub

    Private Sub UltrasonicSpliceDetails_Load(sender As Object, e As EventArgs) Handles Me.Load
        HighlightErrorsAndWarnings()
    End Sub

    Private Sub ugUltrasonicSplice_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugUltrasonicSplice.InitializeLayout
        Me.ugUltrasonicSplice.BeginUpdate()

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

                    Dim wireGroup As UltraGridGroup = .Groups.Add("Wire", UltrasonicSpliceDetailsStrings.Wire_GroupCaption)
                    Dim counterGroup As UltraGridGroup = .Groups.Add("Counter", UltrasonicSpliceDetailsStrings.Counter_GroupCaption)

                    For Each column As UltraGridColumn In .Columns
                        Select Case column.Key
                            Case UltrasonicSpliceDetailsStrings.CavNum_ColumnCaption
                                column.MergedCellEvaluationType = MergedCellEvaluationType.MergeSameText
                                column.MergedCellStyle = MergedCellStyle.Always
                                column.MergedCellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle
                                column.MaxWidth = 75
                                column.MinWidth = 60
                                column.SortComparer = New NumericStringSortComparer
                                column.SortIndicator = SortIndicator.Ascending

                                cavGroup.Columns.Add(column)
                            Case "CavityId", "WireId", "LengthVal", "CounterCavityId"
                                column.Hidden = True
                            Case "Wire"
                                column.Header.Caption = UltrasonicSpliceDetailsStrings.Wire_ColumnCaption
                                column.MaxWidth = 75
                                column.MinWidth = 50

                                wireGroup.Columns.Add(column)
                            Case "Type"
                                column.Header.Caption = UltrasonicSpliceDetailsStrings.Type_ColumnCaption
                                column.MaxWidth = 125
                                column.MinWidth = 75

                                wireGroup.Columns.Add(column)
                            Case "CSA"
                                column.Header.Caption = UltrasonicSpliceDetailsStrings.CSA_ColumnCaption
                                column.MaxWidth = 75
                                column.MinWidth = 60

                                wireGroup.Columns.Add(column)
                            Case "Color"
                                column.Header.Caption = UltrasonicSpliceDetailsStrings.Color_ColumnCaption
                                column.MaxWidth = 100
                                column.MinWidth = 75

                                wireGroup.Columns.Add(column)
                            Case "Signal"
                                column.Header.Caption = UltrasonicSpliceDetailsStrings.Signal_ColumnCaption
                                column.MaxWidth = 150
                                column.MinWidth = 125

                                wireGroup.Columns.Add(column)
                            Case "Length"
                                column.Header.Caption = UltrasonicSpliceDetailsStrings.Length_ColumnCaption
                                column.MaxWidth = 75
                                column.MinWidth = 60

                                wireGroup.Columns.Add(column)
                            Case "SeparatorColumn1", "SeparatorColumn2"
                                column.Header.Caption = String.Empty

                                If (column.Key = "SeparatorColumn1") Then
                                    wireGroup.Columns.Add(column)
                                Else
                                    counterGroup.Columns.Add(column)
                                End If
                            Case "CounterTerminal"
                                column.Header.Caption = UltrasonicSpliceDetailsStrings.Terminal_ColumnCaption
                                column.MaxWidth = 125
                                column.MinWidth = 100

                                counterGroup.Columns.Add(column)
                            Case "CounterTerminalPlating"
                                column.Header.Caption = UltrasonicSpliceDetailsStrings.Plating_ColumnCaption
                                column.MaxWidth = 75
                                column.MinWidth = 50

                                counterGroup.Columns.Add(column)
                            Case "CounterCavity"
                                column.Header.Caption = UltrasonicSpliceDetailsStrings.CavNum_ColumnCaption
                                column.MaxWidth = 75
                                column.MinWidth = 60

                                counterGroup.Columns.Add(column)
                            Case "CounterConnectorId"
                                column.Header.Caption = UltrasonicSpliceDetailsStrings.CounterConnector_ColumnCaption
                                column.MaxWidth = 100
                                column.MinWidth = 75

                                counterGroup.Columns.Add(column)
                        End Select
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

        Me.ugUltrasonicSplice.EndUpdate()
    End Sub

    Private Sub ugUltrasonicSplice_InitializeRow(sender As Object, e As InitializeRowEventArgs) Handles ugUltrasonicSplice.InitializeRow
        e.Row.Cells("SeparatorColumn1").Appearance.BackColor = Color.Black
        e.Row.Cells("SeparatorColumn2").Appearance.BackColor = Color.Black
    End Sub

    Private Sub ugUltrasonicSplice_MouseClick(sender As Object, e As MouseEventArgs) Handles ugUltrasonicSplice.MouseClick
        If (e.Button = System.Windows.Forms.MouseButtons.Left) Then
            Dim element As Infragistics.Win.UIElement = Me.ugUltrasonicSplice.DisplayLayout.UIElement.LastElementEntered
            Dim cell As UltraGridCell = TryCast(element.GetContext(GetType(UltraGridCell)), UltraGridCell)

            If (cell IsNot Nothing) Then
                Select Case cell.Column.Key
                    Case UltrasonicSpliceDetailsStrings.CavNum_ColumnCaption
                        RaiseEvent UltrasonicSpliceDetailsMouseClick(_kblMapper.HarnessPartNumber, New InformationHubEventArgs(_kblMapper.Id.ToString, E3.Lib.Schema.Kbl.KblObjectType.Cavity, cell.Row.Cells("CavityId").Value.ToString))
                    Case "Wire", "Type", "CSA", "Color", "Signal", "Length"
                        RaiseEvent UltrasonicSpliceDetailsMouseClick(_kblMapper.HarnessPartNumber, New InformationHubEventArgs(_kblMapper.Id.ToString, E3.Lib.Schema.Kbl.KblObjectType.Wire_occurrence, cell.Row.Cells("WireId").Value.ToString))
                    Case "CounterTerminal", "CounterTerminalPlating", "CounterCavity", "CounterConnectorId"
                        RaiseEvent UltrasonicSpliceDetailsMouseClick(_kblMapper.HarnessPartNumber, New InformationHubEventArgs(_kblMapper.Id.ToString, E3.Lib.Schema.Kbl.KblObjectType.Wire_occurrence, cell.Row.Cells("CounterCavityId").Value.ToString))
                End Select
            End If
        End If
    End Sub

End Class