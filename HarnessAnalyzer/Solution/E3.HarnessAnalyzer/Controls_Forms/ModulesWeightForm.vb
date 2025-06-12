Imports System.IO
Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinGrid
Imports Infragistics.Win.UltraWinToolbars
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.Lib.Converter.Unit
Imports Zuken.E3.Lib.Schema.Kbl.Properties

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class ModulesWeightForm
    Private _contextMenu As PopupMenuTool
    Private _exportFileName As String
    Private _kblMapper As KblMapper
    Private _moduleRows As SelectedRowsCollection

    Private Enum ContextMenuToolKey
        CollapseAll
        ExpandAll
    End Enum

    Public Sub New(kblMapper As KblMapper, moduleRows As SelectedRowsCollection)
        InitializeComponent()

        _kblMapper = kblMapper
        _moduleRows = moduleRows

        Initialize()
    End Sub

    Private Function GetWeightPerMeters(part As Part, occurrences As List(Of IKblOccurrence)) As Double
        Dim weight As Double = Double.NaN

        If (TypeOf part Is General_wire) Then
            Using engine As New CalculateWeightEngine(occurrences, _kblMapper)
                With engine
                    Dim res As CalculateWeightEngine.CalculateResult = .CalculateData
                    If res.Success Then
                        For Each row As CalculatedWeightRow In res.Rows
                            If (row.KblMass.HasValue) Then
                                If (Double.IsNaN(weight)) Then
                                    weight = 0
                                End If
                                weight += row.KblMass.Value
                            ElseIf (row.IsCable) AndAlso (row.Total.HasValue) Then
                                If Double.IsNaN(weight) Then
                                    weight = 0
                                End If
                                weight += row.Total.Value
                            End If
                        Next
                    Else
                        Throw New Exception(res.Message, res.Exception)
                    End If
                End With
            End Using
        ElseIf (TypeOf part Is Wire_protection) AndAlso (part.Mass_information IsNot Nothing) Then
            Dim weightVal As NumericValue = part.Mass_information.Extract(_kblMapper)
            If weightVal IsNot Nothing Then
                Dim accLength As Double = Double.NaN

                weight = weightVal.Value

                If (weightVal.Unit.Unit = CalcUnit.UnitEnum.Gram) Then
                    Select Case weightVal.Unit.UnitPrefix
                        Case CalcUnit.UPrefixEnum.None
                            weight *= 1
                        Case CalcUnit.UPrefixEnum.Kilo
                            weight *= 1000
                        Case CalcUnit.UPrefixEnum.Centi
                            weight *= 0.01
                        Case CalcUnit.UPrefixEnum.Milli
                            weight *= 0.001
                        Case CalcUnit.UPrefixEnum.Micro
                            weight *= 0.000001
                    End Select
                End If

                For Each wireProtOcc As Wire_protection_occurrence In occurrences
                    For Each segment As Segment In _kblMapper.GetSegments.Where(Function(seg) seg.Protection_area IsNot Nothing AndAlso seg.Protection_area.Any(Function(protArea) protArea.Associated_protection = wireProtOcc.SystemId))
                        If (segment.Physical_length IsNot Nothing) Then
                            Dim length As Double = Double.NaN
                            Dim lengthVal As NumericValue = segment.Physical_length.Extract(_kblMapper)
                            Dim protectionArea As Protection_area = segment.Protection_area.Where(Function(protArea) protArea.Associated_protection = wireProtOcc.SystemId).FirstOrDefault

                            length = lengthVal.Value

                            If (lengthVal.Unit.Unit = CalcUnit.UnitEnum.Metre) Then
                                Select Case lengthVal.Unit.UnitPrefix
                                    Case CalcUnit.UPrefixEnum.None
                                        length *= 1
                                    Case CalcUnit.UPrefixEnum.Kilo
                                        length *= 1000
                                    Case CalcUnit.UPrefixEnum.Centi
                                        length *= 0.01
                                    Case CalcUnit.UPrefixEnum.Milli
                                        length *= 0.001
                                    Case CalcUnit.UPrefixEnum.Micro
                                        length *= 0.000001
                                End Select
                            End If

                            If (Not Double.IsNaN(length)) Then
                                length *= (protectionArea.End_location - protectionArea.Start_location)

                                If (Double.IsNaN(accLength)) Then
                                    accLength = 0
                                End If

                                accLength += length
                            End If
                        End If
                    Next
                Next

                weight = weight * If(Double.IsNaN(accLength), 1, accLength)
            Else
                Throw New Exception(String.Format(ErrorStrings.KBL_UnitComponentIdNotFoundInKbl, part.Mass_information.Unit_component) & $" ({ObjectPropertyNameStrings.PartNumber}: {part.Part_number})")
            End If
        End If

        Return If(Double.IsNaN(weight), weight, Math.Round(weight, 1))
    End Function

    Private Function GetWeightPerPieces(part As Part, occurrenceCount As Integer, unitName As String) As Double
        Dim weight As Double = Double.NaN

        If (part.Mass_information IsNot Nothing) Then
            Dim weightVal As NumericValue = part.Mass_information.Extract(_kblMapper)

            weight = weightVal.Value * occurrenceCount

            If (weightVal.Unit.Unit = CalcUnit.UnitEnum.Gram) Then
                Select Case weightVal.Unit.UnitPrefix
                    Case CalcUnit.UPrefixEnum.None
                        If (unitName = "g") Then
                            weight *= 1
                        ElseIf (unitName = "kg") Then
                            weight *= 0.001
                        End If
                    Case CalcUnit.UPrefixEnum.Kilo
                        If (unitName = "g") Then
                            weight *= 1000
                        ElseIf (unitName = "kg") Then
                            weight *= 1
                        End If
                    Case CalcUnit.UPrefixEnum.Centi
                        If (unitName = "g") Then
                            weight *= 0.01
                        ElseIf (unitName = "kg") Then
                            weight *= 0.00001
                        End If
                    Case CalcUnit.UPrefixEnum.Milli
                        If (unitName = "g") Then
                            weight *= 0.001
                        ElseIf (unitName = "kg") Then
                            weight *= 0.000001
                        End If
                    Case CalcUnit.UPrefixEnum.Micro
                        If (unitName = "g") Then
                            weight *= 0.000001
                        ElseIf (unitName = "kg") Then
                            weight *= 0.000000001
                        End If
                End Select
            End If
        End If

        Return If(Double.IsNaN(weight), weight, Math.Round(weight, 1))
    End Function

    Private Function GetPartContentOfModule(controlledComponents As List(Of String)) As Dictionary(Of IKblPartObject, List(Of IKblOccurrence))
        Dim partContent As New Dictionary(Of IKblPartObject, List(Of IKblOccurrence))

        For Each contrCmpId As String In controlledComponents
            Dim occurrenceObject As IKblOccurrence = _kblMapper.GetOccurrenceObjectUntyped(contrCmpId)
            If (occurrenceObject IsNot Nothing) Then
                Dim kbl_part As IKblPartObject = occurrenceObject.GetPart(_kblMapper, occurrenceObject)
                If TypeOf kbl_part Is ICollection Then ' HINT -> it's possibly a wiring_group
                    For Each gw As General_wire In CType(kbl_part, ICollection).OfType(Of General_wire)
                        partContent.GetOrAddNew(gw).TryAdd(occurrenceObject)
                    Next
                ElseIf kbl_part IsNot Nothing Then
                    partContent.GetOrAddNew(kbl_part).TryAdd(occurrenceObject)
                End If
            End If
        Next

        For Each genWireContent As KeyValuePair(Of IKblPartObject, List(Of IKblOccurrence)) In partContent.Where(Function(p) TypeOf p.Key Is General_wire)
            If (genWireContent.Value.Any(Function(occ) TypeOf occ Is Core_occurrence) AndAlso genWireContent.Value.Any(Function(occ) TypeOf occ Is Special_wire_occurrence)) Then
                Dim coreOccs As List(Of IKblOccurrence) = genWireContent.Value.Where(Function(occ) TypeOf occ Is Core_occurrence).ToList
                For Each coreOcc As IKblOccurrence In coreOccs
                    genWireContent.Value.Remove(coreOcc)
                Next
            End If
        Next

        Return partContent
    End Function

    Private Sub Initialize()
        Me.BackColor = Color.White
        Me.Icon = My.Resources.Weight

        _contextMenu = New PopupMenuTool("GridContextMenu")

        InitializeGridContextMenu()
        InitializeGridData()
    End Sub

    Private Sub InitializeGridContextMenu()
        With _contextMenu
            .DropDownArrowStyle = DropDownArrowStyle.None

            Dim collapseAllButton As New ButtonTool(ContextMenuToolKey.CollapseAll.ToString)
            collapseAllButton.SharedProps.Caption = InformationHubStrings.CollapseAll_CtxtMnu_Caption
            collapseAllButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.CollapseAll.ToBitmap

            Dim expandAllButton As New ButtonTool(ContextMenuToolKey.ExpandAll.ToString)
            expandAllButton.SharedProps.Caption = InformationHubStrings.ExpandAll_CtxtMnu_Caption
            expandAllButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.ExpandAll.ToBitmap

            Me.utmModulesWeight.Tools.AddRange(New ToolBase() {_contextMenu, collapseAllButton, expandAllButton})

            .Tools.AddTool(collapseAllButton.Key)
            .Tools.AddTool(expandAllButton.Key)
        End With
    End Sub

    Private Sub InitializeGridData()
        With Me.udsModulesWeight
            .Band.Key = "Modules"

            With .Band
                .Columns.Add(ObjectPropertyNameStrings.Abbreviation)
                .Columns.Add(ObjectPropertyNameStrings.PartNumber)
                .Columns.Add(ObjectPropertyNameStrings.Description)
                .Columns.Add(ObjectPropertyNameStrings.Version)
                .Columns.Add("WeightVal", GetType(Double))
                .Columns.Add(ModulesWeightFormStrings.Weight_ColumnCaption)
            End With

            .Band.ChildBands.Add("Parts")

            With .Band.ChildBands(0)
                .Columns.Add(ObjectPropertyNameStrings.PartNumber)
                .Columns.Add(ObjectPropertyNameStrings.Abbreviation)
                .Columns.Add(ObjectPropertyNameStrings.Description)
                .Columns.Add(ObjectPropertyNameStrings.Version)
                .Columns.Add(ModulesWeightFormStrings.Occurrences_ColumnCaption)
                .Columns.Add("WeightVal", GetType(Double))
                .Columns.Add(ModulesWeightFormStrings.Weight_ColumnCaption)
            End With

            For Each selModuleRow As UltraGridRow In _moduleRows
                Dim [module] As [Module] = DirectCast(DirectCast(selModuleRow.ListObject, UltraDataRow).Tag, [Module])
                Dim moduleRow As UltraDataRow = .Rows.Add

                With moduleRow
                    .SetCellValue(ObjectPropertyNameStrings.Abbreviation, selModuleRow.Cells(PartPropertyName.Abbreviation.ToString).Value)
                    .SetCellValue(ObjectPropertyNameStrings.PartNumber, selModuleRow.Cells(PartPropertyName.Part_number.ToString).Value)
                    .SetCellValue(ObjectPropertyNameStrings.Description, selModuleRow.Cells(PartPropertyName.Part_description.ToString).Value)
                    .SetCellValue(ObjectPropertyNameStrings.Version, selModuleRow.Cells(PartPropertyName.Version.ToString).Value)

                    Dim weight As Double = GetWeightPerPieces([module], 1, "g")

                    If (Double.IsNaN(weight)) Then
                        .SetCellValue("WeightVal", 0)
                        .SetCellValue(ModulesWeightFormStrings.Weight_ColumnCaption, Zuken.E3.HarnessAnalyzer.Shared.NOT_AVAILABLE)
                    Else
                        .SetCellValue("WeightVal", weight)
                        .SetCellValue(ModulesWeightFormStrings.Weight_ColumnCaption, String.Format("{0} g", Math.Round(CDbl(.GetCellValue("WeightVal")), 1)))
                    End If

                    .Tag = [module]
                End With

                InitializePartChildRows(moduleRow)
            Next
        End With

        Me.ugModulesWeight.DataSource = Me.udsModulesWeight
    End Sub

    Private Sub InitializePartChildRows(moduleRow As UltraDataRow)
        Dim [module] As [Module] = DirectCast(moduleRow.Tag, [Module])
        If ([module].Module_configuration.Controlled_components?.Length > 0) Then
            Try
                For Each partContent As KeyValuePair(Of IKblPartObject, List(Of IKblOccurrence)) In GetPartContentOfModule([module].Module_configuration.Controlled_components.SplitSpace.ToList)
                    Dim part As IKblPartObject = partContent.Key
                    Dim partRow As UltraDataRow = moduleRow.GetChildRows(Me.udsModulesWeight.Band.ChildBands(0)).Add

                    With partRow
                        partRow.SetCellValue(ObjectPropertyNameStrings.PartNumber, part.Part_Number)
                        partRow.SetCellValue(ObjectPropertyNameStrings.Abbreviation, part.Abbreviation)
                        partRow.SetCellValue(ObjectPropertyNameStrings.Description, part.Description)
                        partRow.SetCellValue(ObjectPropertyNameStrings.Version, part.Version)
                        partRow.SetCellValue(ModulesWeightFormStrings.Occurrences_ColumnCaption, String.Format("{0} pcs", partContent.Value.Count))

                        Dim weight As Double = Double.NaN

                        If (TypeOf part Is General_wire) OrElse (TypeOf part Is Wire_protection) Then
                            weight = GetWeightPerMeters(CType(part, Part), partContent.Value)
                        ElseIf TypeOf part Is Part Then
                            weight = GetWeightPerPieces(CType(part, Part), partContent.Value.Count, "g")
                        End If

                        If (Double.IsNaN(weight)) Then
                            partRow.SetCellValue("WeightVal", 0)
                            partRow.SetCellValue(ModulesWeightFormStrings.Weight_ColumnCaption, Zuken.E3.HarnessAnalyzer.Shared.NOT_AVAILABLE)
                        Else
                            partRow.SetCellValue("WeightVal", weight)
                            partRow.SetCellValue(ModulesWeightFormStrings.Weight_ColumnCaption, String.Format("{0} g", Math.Round(CDbl(.GetCellValue("WeightVal")), 1)))
                        End If

                        .Tag = part
                    End With
                Next
            Catch ex As Exception
                ex.ShowMessageBox()
            End Try
        End If
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
    End Sub

    Private Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        Using sfdExcel As New SaveFileDialog
            With sfdExcel
                .DefaultExt = KnownFile.XLSX.Trim("."c)

                If (_kblMapper.GetChanges.Any()) Then
                    .FileName = String.Format("{0}{1}{2}_{3}_{4}_ModuleWeights.xlsx", Now.Year, Format(Now.Month, "00"), Format(Now.Day, "00"), Replace(_kblMapper.HarnessPartNumber, " ", String.Empty), _kblMapper.GetChanges.Max(Function(change) change.Id))
                Else
                    .FileName = String.Format("{0}{1}{2}_{3}_ModuleWeights.xlsx", Now.Year, Format(Now.Month, "00"), Format(Now.Day, "00"), Replace(_kblMapper.HarnessPartNumber, " ", String.Empty))
                End If

                .Filter = "Excel files (*.xlsx)|*.xlsx|Excel files (97-2003) (*.xls)|*.xls"
                .Title = ModulesWeightFormStrings.ExportModulesWeightToExcelFile_Title

                If (.ShowDialog(Me) = DialogResult.OK) Then
                    Try
                        _exportFileName = .FileName

                        If (IO.Path.GetExtension(_exportFileName).ToLower = ".xlsx") Then
                            Me.ugeeModulesWeight.ExportAsync(Me.ugModulesWeight, .FileName, Infragistics.Documents.Excel.WorkbookFormat.Excel2007)
                        Else
                            Me.ugeeModulesWeight.ExportAsync(Me.ugModulesWeight, .FileName, Infragistics.Documents.Excel.WorkbookFormat.Excel97To2003)
                        End If
                    Catch ex As Exception
                        ex.ShowMessageBox(String.Format(ModulesWeightFormStrings.ErrorExportModulesWeight_Msg, vbCrLf, ex.Message))
                    End Try
                End If
            End With
        End Using
    End Sub

    Private Sub ugeeModulesWeight_ExportEnded(sender As Object, e As ExcelExport.ExportEndedEventArgs) Handles ugeeModulesWeight.ExportEnded
        If (Not e.Canceled) AndAlso (MessageBoxEx.ShowQuestion(ModulesWeightFormStrings.ExportExcelFinished_Msg) = System.Windows.Forms.DialogResult.Yes) Then
            ProcessEx.Start(_exportFileName)
        End If
    End Sub

    Private Sub ugeeModulesWeight_HeaderRowExporting(sender As Object, e As ExcelExport.HeaderRowExportingEventArgs) Handles ugeeModulesWeight.HeaderRowExporting
        If (e.CurrentRowIndex <> 0) AndAlso (e.CurrentOutlineLevel = 0) Then
            e.Cancel = True
        End If
    End Sub

    Private Sub ugModulesWeight_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugModulesWeight.InitializeLayout
        Me.ugModulesWeight.BeginUpdate()

        With e.Layout
            .AutoFitStyle = AutoFitStyle.ResizeAllColumns
            .CaptionVisible = Infragistics.Win.DefaultableBoolean.False

            For filterOperatorsValueCounter As Integer = 5 To 1 Step -1
                .FilterOperatorsValueList.ValueListItems.RemoveAt(filterOperatorsValueCounter)
            Next

            .GroupByBox.Hidden = True
            .LoadStyle = LoadStyle.LoadOnDemand

            With .Override
                .AllowColMoving = AllowColMoving.NotAllowed
                .AllowDelete = Infragistics.Win.DefaultableBoolean.False
                .AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True
                .AllowUpdate = Infragistics.Win.DefaultableBoolean.False
                .ButtonStyle = Infragistics.Win.UIElementButtonStyle.Button3D
                .CellClickAction = CellClickAction.RowSelect
                .RowSelectors = Infragistics.Win.DefaultableBoolean.False
                .SelectTypeRow = SelectType.Single
            End With

            For Each band As UltraGridBand In .Bands
                With band
                    For Each column As UltraGridColumn In .Columns
                        If (Not column.Hidden) Then
                            With column
                                .CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center
                                .CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle
                                .Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center
                                .Header.Appearance.TextVAlign = Infragistics.Win.VAlign.Middle

                                If .Key = ModulesWeightFormStrings.Weight_ColumnCaption Then
                                    .SortComparer = New NumericStringSortComparer
                                End If

                                If (.Key = "WeightVal") Then
                                    .AllowRowSummaries = AllowRowSummaries.True
                                    .Hidden = True

                                    Dim sumSettings As SummarySettings = band.Summaries.Add(SummaryType.Sum, column, SummaryPosition.Left)
                                    sumSettings.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True
                                    sumSettings.DisplayFormat = "{0:F1} g"

                                    band.SummaryFooterCaption = ModulesWeightFormStrings.AccWeight_SummaryRowCaption
                                    band.Override.SummaryFooterCaptionAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True
                                End If

                                If (.Index = 0) Then
                                    .SortIndicator = SortIndicator.Ascending
                                End If
                            End With
                        End If
                    Next
                End With
            Next
        End With

        Me.ugModulesWeight.EndUpdate()
    End Sub

    Private Sub ugModulesWeight_InitializeRow(sender As Object, e As InitializeRowEventArgs) Handles ugModulesWeight.InitializeRow
        e.Row.ExpandAll()

        If (e.Row.Cells(ModulesWeightFormStrings.Weight_ColumnCaption).Value.ToString = Zuken.E3.HarnessAnalyzer.Shared.NOT_AVAILABLE) Then
            e.Row.Cells(ModulesWeightFormStrings.Weight_ColumnCaption).Appearance.Image = My.Resources.Warning
        End If
    End Sub

    Private Sub ugModulesWeight_MouseClick(sender As Object, e As MouseEventArgs) Handles ugModulesWeight.MouseClick
        If (e.Button = System.Windows.Forms.MouseButtons.Right) Then
            Dim element As Infragistics.Win.UIElement = Me.ugModulesWeight.DisplayLayout.UIElement.LastElementEntered
            Dim header As ColumnHeader = TryCast(element.GetContext(GetType(ColumnHeader)), ColumnHeader)
            If (header IsNot Nothing) Then
                _contextMenu.ShowPopup()
            End If
        End If
    End Sub

    Private Sub utmModulesWeight_ToolClick(sender As Object, e As ToolClickEventArgs) Handles utmModulesWeight.ToolClick
        Select Case e.Tool.Key
            Case ContextMenuToolKey.CollapseAll.ToString
                With Me.ugModulesWeight
                    .BeginUpdate()
                    .Rows.CollapseAll(True)
                    .EndUpdate()
                End With
            Case ContextMenuToolKey.ExpandAll.ToString
                With Me.ugModulesWeight
                    .BeginUpdate()
                    .Rows.ExpandAll(True)
                    .EndUpdate()
                End With
        End Select
    End Sub

End Class