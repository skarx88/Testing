Imports System.Data
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Windows
Imports Infragistics.Documents.Excel
Imports Infragistics.Win.Misc
Imports VectorDraw.Geometry
Imports Zuken.E3.HarnessAnalyzer.OverallConnectivityForm
Imports Zuken.E3.Lib.Converter.Unit
Imports Zuken.E3.Lib.Converter.Unit.CalcUnit

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class WireCurrentCalculatorForm

    Private _hasVoltageDropError As Boolean = False
    Private _dt As DataTable
    Private _voltageDropCntrlsList As New List(Of VoltageDropUserControl)
    Private _resistivityOfwireSum As Double = 0
    Private _selectedWireOrCore As SelectedWireOrCoreVdDrawUserControl
    Private _dicOfWireAdjAndVertices As New Dictionary(Of String, Dictionary(Of String, CavityVertex))
    Private _originWireNumberFromTable As String = String.Empty

    Private Const LABELOFFSET As Integer = 25
    Private Const BUTTONOFFSET As Integer = 15
    Private Const RATIOFOROFFSET As Double = 1.7
    Private Const MINWIDTH As Integer = 1200
    Private Const HEADINGHEIGHT As Integer = 41

    Public Sub New(kblmappers As IEnumerable(Of KblMapper), harnessDescriptionAndColor As Dictionary(Of String, Color), originWireNumber As String, wireAdjacencies As List(Of WireAdjacency), defaultWireLengthType As String, ConnectivityOfSelectedWire As HarnessConnectivity, overallConnectivity As OverallConnectivity, originHarnessConnectivity As HarnessConnectivity, harnessesWithInactiveCoresWires As Dictionary(Of String, List(Of String)))
        InitializeComponent()

        _originWireNumberFromTable = originWireNumber
        Dim harrnessConnectivityOverallOriginal As HarnessConnectivity = GetHarnessConnectivityOnlyForSelectedWireAdjacencies(wireAdjacencies, ConnectivityOfSelectedWire)

        Dim coreOrWireIds() As String = wireAdjacencies.Select(Function(wireAdj) wireAdj.Key).ToArray
        Me.BackColor = Colors.Background
        Me.Text = String.Format(VoltageDropUserControlStrings.VoltageDropTitel, originWireNumber)
        Me.ClientSize = New Drawing.Size(1500, 1730)

        _selectedWireOrCore = New SelectedWireOrCoreVdDrawUserControl(kblmappers, harnessDescriptionAndColor, wireAdjacencies, defaultWireLengthType, harrnessConnectivityOverallOriginal, _dicOfWireAdjAndVertices, overallConnectivity, originHarnessConnectivity, harnessesWithInactiveCoresWires)

        If Not _selectedWireOrCore.IsSelectionInvalidErrorOccurred Then
            If _selectedWireOrCore.GetWireAdjacenciesSequence.Count > 0 Then
                Init(kblmappers, _selectedWireOrCore.GetWireAdjacenciesSequence.Select(Function(x) x.Key).ToArray, defaultWireLengthType)
            Else
                Init(kblmappers, coreOrWireIds, defaultWireLengthType)
            End If
            _selectedWireOrCore.FitToPanel()
        End If

    End Sub
    Private ReadOnly Property InputCurrent As Double
        Get
            Return If(InputCurrentUNE.Value IsNot Nothing AndAlso TypeOf (InputCurrentUNE.Value) Is Double, Math.Round(CDbl(InputCurrentUNE.Value), 2), 0)
        End Get
    End Property
    Private Function GetHarnessConnectivityOnlyForSelectedWireAdjacencies(wireAdjacencies As List(Of WireAdjacency), ConnectivityOfSelectedWire As HarnessConnectivity) As HarnessConnectivity

        Dim harrnessConnectivityOverallOriginal As New HarnessConnectivity()
        Dim dicOfvertices As New Dictionary(Of String, CavityVertex)

        For Each vert As CavityVertex In ConnectivityOfSelectedWire.CavityVertices
            For Each adjVert As AdjacencyToVertex In vert.AdjacenciesToSuccessors
                Dim vertices As New Dictionary(Of String, CavityVertex)
                If wireAdjacencies.Contains(adjVert.WireAdjacency) Then

                    vertices.TryAdd(vert.Key, vert)
                    dicOfvertices.TryAdd(vert.Key, vert)
                    vertices.TryAdd(adjVert.Vertex.Key, adjVert.Vertex)
                    dicOfvertices.TryAdd(adjVert.Vertex.Key, adjVert.Vertex)

                    If Not harrnessConnectivityOverallOriginal.HasCavityVertex(vert.Key) Then
                        harrnessConnectivityOverallOriginal.AddCavityVertex(vert)
                    End If

                    If Not harrnessConnectivityOverallOriginal.HasCavityVertex(adjVert.Vertex.Key) Then
                        harrnessConnectivityOverallOriginal.AddCavityVertex(adjVert.Vertex)
                    End If

                    If Not harrnessConnectivityOverallOriginal.HasWireAdjacency(adjVert.WireAdjacency.Key) Then
                        harrnessConnectivityOverallOriginal.AddWireAdjacency(adjVert.WireAdjacency)
                        _dicOfWireAdjAndVertices.TryAdd(adjVert.WireAdjacency.Key, vertices)
                    End If

                End If
            Next
        Next

        'Hint : Adding virtual wire adjacencies
        For Each vert As CavityVertex In ConnectivityOfSelectedWire.CavityVertices
            For Each adjVert As AdjacencyToVertex In vert.AdjacenciesToSuccessors
                Dim vertices As New Dictionary(Of String, CavityVertex)

                If adjVert.WireAdjacency.CoreWireOccurrence Is Nothing Then
                    If vert.IsInliner AndAlso adjVert.Vertex.IsInliner Then
                        If dicOfvertices.ContainsKey(vert.Key) AndAlso dicOfvertices.ContainsKey(adjVert.Vertex.Key) Then
                            vertices.TryAdd(vert.Key, vert)
                            If Not vertices.ContainsKey(adjVert.Vertex.Key) Then
                                vertices.Add(adjVert.Vertex.Key, adjVert.Vertex)
                            End If

                            If Not harrnessConnectivityOverallOriginal.HasCavityVertex(adjVert.WireAdjacency.Key) Then
                                harrnessConnectivityOverallOriginal.AddWireAdjacency(adjVert.WireAdjacency)
                                _dicOfWireAdjAndVertices.TryAdd(adjVert.WireAdjacency.Key, vertices)
                            End If
                        End If
                    End If

                End If
            Next
        Next

        Return harrnessConnectivityOverallOriginal
    End Function

    Private Sub Init(kblmappers As IEnumerable(Of KblMapper), coreOrWireIds() As String, defaultWireLengthType As String)
        _selectedWireOrCore.Dock = DockStyle.Fill
        Me.VectorDrawControlPanel.Location = New System.Drawing.Point(0, 0)
        Me.VectorDrawControlPanel.ClientArea.Controls.Add(_selectedWireOrCore)

        Dim locationY As Integer = AddInputCurrentControlToTableLayoutAndGetLocationY()
        Dim headingLocationY As Integer = locationY + 2 * LABELOFFSET
        Dim totalHeadingY As Integer = 0

        AddHeadingForVoltageDropUserControl(defaultWireLengthType, headingLocationY)

        Dim HasToActivateMaterialDensityColumn As Boolean = False
        Dim table As New DataTable

        table.TableName = My.Resources.VoltageDropUserControlStrings.VoltageDrop
        table.Columns.Add(My.Resources.VoltageDropUserControlStrings.StartResistance)
        table.Columns.Add(My.Resources.VoltageDropUserControlStrings.WireOrCorenumber)
        table.Columns.Add(My.Resources.VoltageDropUserControlStrings.StartCPTerminalDescription)
        table.Columns.Add(My.Resources.VoltageDropUserControlStrings.TypeOfWire)
        table.Columns.Add(My.Resources.VoltageDropUserControlStrings.Material)
        table.Columns.Add(My.Resources.VoltageDropUserControlStrings.MaterialDensity)
        table.Columns.Add(My.Resources.VoltageDropUserControlStrings.ResistivityCustom)
        table.Columns.Add(My.Resources.VoltageDropUserControlStrings.WireResistance)
        table.Columns.Add(My.Resources.VoltageDropUserControlStrings.CSA)
        table.Columns.Add(My.Resources.VoltageDropUserControlStrings.Temperature)
        table.Columns.Add(My.Resources.VoltageDropUserControlStrings.Length)
        table.Columns.Add(My.Resources.VoltageDropUserControlStrings.ConductorWeight)
        table.Columns.Add(My.Resources.VoltageDropUserControlStrings.TotalWeight)
        table.Columns.Add(My.Resources.VoltageDropUserControlStrings.EndCPTerminalDescription)
        table.Columns.Add(My.Resources.VoltageDropUserControlStrings.EndResistance)

        Dim voltageDropWidth As Integer = 0
        For i As Integer = 1 To coreOrWireIds.Length
            Dim voltageDropCntrl As New VoltageDropUserControl(Me.ClientSize.Width, table, defaultWireLengthType, kblmappers, coreOrWireIds, headingLocationY, i, _selectedWireOrCore.SequentialWireAdjacenciesWithVertices)

            AddHandler voltageDropCntrl.UpdateResult, AddressOf VoltageDropCntrl_UpdateResult
            _resistivityOfwireSum += voltageDropCntrl.Resistance
            _voltageDropCntrlsList.Add(voltageDropCntrl)
            If i = 1 Then
                voltageDropWidth = voltageDropCntrl.Width
            End If
            If voltageDropCntrl.HasError And Not _hasVoltageDropError Then
                _hasVoltageDropError = True
            End If
            Me.Controls.Add(voltageDropCntrl)
        Next

        totalHeadingY = headingLocationY + (HEADINGHEIGHT * (coreOrWireIds.Length + 1)) + LABELOFFSET

        If My.Settings.InitialCurrent <= CDbl(Me.InputCurrentUNE.MaxValue) AndAlso My.Settings.InitialCurrent >= CDbl(Me.InputCurrentUNE.MinValue) Then
            Me.InputCurrentUNE.Value = My.Settings.InitialCurrent
        Else
            Me.InputCurrentUNE.Value = 0
        End If
        Me.ExportUltraBtn.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        Me.CancelUltraBtn.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        Me.Outputlbl.Location = New Drawing.Point(LABELOFFSET, totalHeadingY + LABELOFFSET)
        Me.OutputTableLayoutPanel.Location = New Drawing.Point(LABELOFFSET, Me.Outputlbl.Location.Y + LABELOFFSET)
        Me.ExportUltraBtn.Location = New Drawing.Point(Me.ClientSize.Width - Me.CancelUltraBtn.Width - Me.ExportUltraBtn.Width - CInt(RATIOFOROFFSET * BUTTONOFFSET), CInt(Me.ClientSize.Height - (RATIOFOROFFSET * LABELOFFSET)))
        Me.CancelUltraBtn.Location = New Drawing.Point(Me.ClientSize.Width - Me.CancelUltraBtn.Width - BUTTONOFFSET, CInt(Me.ClientSize.Height - (RATIOFOROFFSET * LABELOFFSET)))
        Me.ClientSize = New Drawing.Size(voltageDropWidth + LABELOFFSET, Me.OutputTableLayoutPanel.Location.Y + 7 * LABELOFFSET)
        Me.MinimumSize = New Drawing.Size(MINWIDTH, Me.ClientSize.Height)
        Me.VectorDrawControlPanel.Width = Me.ClientSize.Width
        Me.VectorDrawControlPanel.BorderStyle = Infragistics.Win.UIElementBorderStyle.None
        Me.OutputTableLayoutPanel.Anchor = AnchorStyles.Left Or AnchorStyles.Bottom
        Me.Outputlbl.Anchor = AnchorStyles.Left Or AnchorStyles.Bottom

        Me.InputCurrentUltralbl.Text = VoltageDropUserControlStrings.InputCurrent
        Me.Outputlbl.Text = VoltageDropUserControlStrings.Output
        Me.RgesLbl.Text = VoltageDropUserControlStrings.Rges
        Me.DeltaULbl.Text = VoltageDropUserControlStrings.DeltaU
        Me.PvWattLbl.Text = VoltageDropUserControlStrings.PowerDissipation
        Me.TotalConductorWeightLbl.Text = VoltageDropUserControlStrings.TotalCondutorWeight
        Me.TotalWeightLbl.Text = VoltageDropUserControlStrings.TotalWeightLblText
        Me.CancelUltraBtn.Text = VoltageDropUserControlStrings.CancelBtnText

        Me.RgesUltraFTE.ReadOnly = True
        Me.DeltaUUltaFTE.ReadOnly = True
        Me.PvWattUltraFTE.ReadOnly = True
        Me.TotalConductorWeightUltraFTE.ReadOnly = True
        Me.TotalWeightUltraFTE.ReadOnly = True

        _dt = table
        Getoutput(False)
    End Sub

    Private Function AddInputCurrentControlToTableLayoutAndGetLocationY() As Integer
        Dim inputCurrentTableLayoutPanel As New System.Windows.Forms.TableLayoutPanel()

        Me.InputCurrentUltralbl.Dock = DockStyle.Fill
        Me.InputCurrentUNE.Dock = DockStyle.Fill

        inputCurrentTableLayoutPanel.Anchor = AnchorStyles.Top Or AnchorStyles.Left
        inputCurrentTableLayoutPanel.RowCount = 1
        inputCurrentTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        inputCurrentTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        inputCurrentTableLayoutPanel.Controls.Add(InputCurrentUltralbl, 0, 0)
        inputCurrentTableLayoutPanel.Controls.Add(InputCurrentUNE, 1, 0)
        inputCurrentTableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        inputCurrentTableLayoutPanel.TabIndex = 0
        inputCurrentTableLayoutPanel.BorderStyle = BorderStyle.None
        inputCurrentTableLayoutPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.None
        inputCurrentTableLayoutPanel.Margin = New Padding(3, 0, 3, 0)
        inputCurrentTableLayoutPanel.Location = New Drawing.Point(LABELOFFSET, Me.VectorDrawControlPanel.ClientSize.Height + LABELOFFSET)
        inputCurrentTableLayoutPanel.Size = New Drawing.Size(OutputTableLayoutPanel.Size.Width, HEADINGHEIGHT)

        Me.Controls.Add(inputCurrentTableLayoutPanel)

        Return inputCurrentTableLayoutPanel.Location.Y
    End Function

    Private Sub AddHeadingForVoltageDropUserControl(defaultWireLengthType As String, headingLocationY As Integer)
        Dim locationY As Integer = InputCurrentUltralbl.Location.Y + 2 * LABELOFFSET
        Dim width As Integer = Me.ClientSize.Width
        Dim headingLayoutPanel As New System.Windows.Forms.TableLayoutPanel()

        Dim Resistivitylbl As New Infragistics.Win.Misc.UltraLabel()
        Dim WireOrCorenumberlbl As New Infragistics.Win.Misc.UltraLabel()
        Dim FirstCpDescriptionlbl As New Infragistics.Win.Misc.UltraLabel()
        Dim TypeOfWirelbl As New Infragistics.Win.Misc.UltraLabel()
        Dim Materiallbl As New Infragistics.Win.Misc.UltraLabel()
        Dim materialDensitylbl As New Infragistics.Win.Misc.UltraLabel()
        Dim resistivityCustomlbl As New Infragistics.Win.Misc.UltraLabel()
        Dim CSAlbl As New Infragistics.Win.Misc.UltraLabel()
        Dim Temperaturelbl As New Infragistics.Win.Misc.UltraLabel()
        Dim Lengthlbl As New Infragistics.Win.Misc.UltraLabel()
        Dim ConductorWeightbl As New Infragistics.Win.Misc.UltraLabel()
        Dim TotalWeightbl As New Infragistics.Win.Misc.UltraLabel()
        Dim LastCpDescriptionlbl As New Infragistics.Win.Misc.UltraLabel()
        Dim LastResistivitylbl As New Infragistics.Win.Misc.UltraLabel()

        headingLayoutPanel.ColumnCount = 14

        SetMyLabelAndAddToHeadinglayoutPanel(Resistivitylbl, headingLayoutPanel, My.Resources.VoltageDropUserControlStrings.R, 12.5!, 0, 0)
        SetMyLabelAndAddToHeadinglayoutPanel(WireOrCorenumberlbl, headingLayoutPanel, My.Resources.VoltageDropUserControlStrings.WireOrCorenumber, 16.5!, 1, 0)
        SetMyLabelAndAddToHeadinglayoutPanel(FirstCpDescriptionlbl, headingLayoutPanel, My.Resources.VoltageDropUserControlStrings.StartCPTerminalDescription, 30.5!, 2, 0)
        SetMyLabelAndAddToHeadinglayoutPanel(TypeOfWirelbl, headingLayoutPanel, My.Resources.VoltageDropUserControlStrings.TypeOfWire, 23.0!, 3, 0)
        SetMyLabelAndAddToHeadinglayoutPanel(Materiallbl, headingLayoutPanel, My.Resources.VoltageDropUserControlStrings.Material, 15.5!, 4, 0)
        SetMyLabelAndAddToHeadinglayoutPanel(materialDensitylbl, headingLayoutPanel, My.Resources.VoltageDropUserControlStrings.MaterialDensity, 16.0!, 5, 0)
        SetMyLabelAndAddToHeadinglayoutPanel(resistivityCustomlbl, headingLayoutPanel, My.Resources.VoltageDropUserControlStrings.ResistivityCustom, 22.5!, 6, 0)
        SetMyLabelAndAddToHeadinglayoutPanel(CSAlbl, headingLayoutPanel, My.Resources.VoltageDropUserControlStrings.CSA, 15.5!, 7, 0)
        SetMyLabelAndAddToHeadinglayoutPanel(Temperaturelbl, headingLayoutPanel, My.Resources.VoltageDropUserControlStrings.Temperature, 15.5!, 8, 0)
        SetMyLabelAndAddToHeadinglayoutPanel(Lengthlbl, headingLayoutPanel, String.Format("{0} {1} [{2}] ", My.Resources.VoltageDropUserControlStrings.Length, vbCrLf, defaultWireLengthType), 21.5!, 9, 0)
        SetMyLabelAndAddToHeadinglayoutPanel(ConductorWeightbl, headingLayoutPanel, My.Resources.VoltageDropUserControlStrings.ConductorWeight, 15.5!, 10, 0)
        SetMyLabelAndAddToHeadinglayoutPanel(TotalWeightbl, headingLayoutPanel, My.Resources.VoltageDropUserControlStrings.TotalWeight, 18.0!, 11, 0)
        SetMyLabelAndAddToHeadinglayoutPanel(LastCpDescriptionlbl, headingLayoutPanel, My.Resources.VoltageDropUserControlStrings.EndCPTerminalDescription, 30.5!, 12, 0)
        SetMyLabelAndAddToHeadinglayoutPanel(LastResistivitylbl, headingLayoutPanel, My.Resources.VoltageDropUserControlStrings.R, 15.5!, 13, 0)

        headingLayoutPanel.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        headingLayoutPanel.Location = New System.Drawing.Point(0, headingLocationY)
        headingLayoutPanel.RowCount = 1
        headingLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        headingLayoutPanel.Size = New System.Drawing.Size(width, HEADINGHEIGHT)
        headingLayoutPanel.TabIndex = 0
        headingLayoutPanel.BorderStyle = BorderStyle.FixedSingle
        headingLayoutPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single
        headingLayoutPanel.Margin = New Padding(3, 0, 3, 0)
        headingLayoutPanel.Location = New Drawing.Point(0, headingLocationY)
        headingLayoutPanel.Size = New System.Drawing.Size(width, HEADINGHEIGHT)

        Me.Controls.Add(headingLayoutPanel)
    End Sub

    Private Sub SetMyLabelAndAddToHeadinglayoutPanel(mylbl As UltraLabel, headingLayoutPanel As System.Windows.Forms.TableLayoutPanel, text As String, columnPercentage As Single, columnindex As Integer, rowindex As Integer)
        mylbl.Dock = System.Windows.Forms.DockStyle.Fill
        mylbl.Text = text
        mylbl.Appearance.BackColor = System.Drawing.Color.LightBlue
        headingLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, columnPercentage))
        headingLayoutPanel.Controls.Add(mylbl, columnindex, rowindex)
    End Sub

    Private Sub CancelUltraBtn_Click(sender As Object, e As EventArgs) Handles CancelUltraBtn.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
    End Sub

    Private Sub Getoutput(isUpdate As Boolean)
        Dim index As Integer = 0
        Dim rSum As Double = 0
        Dim conductorweight As Double = 0
        Dim TotalWeightOfEachWire As Double = 0

        If _dt IsNot Nothing AndAlso _dt.Rows.Count > 0 Then

            For Each r As DataRow In _dt.Rows

                rSum += If(Not IsDBNull(_dt.Rows(index)(My.Resources.VoltageDropUserControlStrings.StartResistance)), CDbl(_dt.Rows(index)(My.Resources.VoltageDropUserControlStrings.StartResistance)), 0.0)

                If isUpdate Then
                    rSum += If(Not IsDBNull(_dt.Rows(index)(My.Resources.VoltageDropUserControlStrings.WireResistance)), CDbl(_dt.Rows(index)(My.Resources.VoltageDropUserControlStrings.WireResistance)), 0.0)
                End If

                If _dt.Rows(index)(My.Resources.VoltageDropUserControlStrings.Material).ToString = KblObjectType.Custom.ToLocalizedString Then
                    Dim csa As Double = If(Not IsDBNull(_dt.Rows(index)(My.Resources.VoltageDropUserControlStrings.CSA)), CDbl(UnitConverter.Convert(CDbl(_dt.Rows(index)(My.Resources.VoltageDropUserControlStrings.CSA)), ShortUnit.SqMm, ShortUnit.SqCm, True)), 0.0)
                    Dim length As Double = If(Not IsDBNull(_dt.Rows(index)(My.Resources.VoltageDropUserControlStrings.Length)), CDbl(ToCentimetre(CDbl(_dt.Rows(index)(My.Resources.VoltageDropUserControlStrings.Length)))), 0.0)
                    Dim matDensity As Double = If(Not IsDBNull(_dt.Rows(index)(My.Resources.VoltageDropUserControlStrings.MaterialDensity)), CDbl(_dt.Rows(index)(My.Resources.VoltageDropUserControlStrings.MaterialDensity)), 0.0)

                    conductorweight += csa * length * matDensity  'hint: custom material density mm*mm²*g/mm³ ->density in g/mm3(mass/vol) 
                Else
                    conductorweight += If(Not IsDBNull(_dt.Rows(index)(My.Resources.VoltageDropUserControlStrings.ConductorWeight)), CDbl(_dt.Rows(index)(My.Resources.VoltageDropUserControlStrings.ConductorWeight)), 0.0)

                End If
                TotalWeightOfEachWire += If(Not IsDBNull(_dt.Rows(index)(My.Resources.VoltageDropUserControlStrings.TotalWeight)), CDbl(_dt.Rows(index)(My.Resources.VoltageDropUserControlStrings.TotalWeight)), 0.0)

                index += 1
            Next

            rSum += If(Not IsDBNull(_dt.Rows(_dt.Rows.Count - 1)(My.Resources.VoltageDropUserControlStrings.EndResistance)), CDbl(_dt.Rows(_dt.Rows.Count - 1)(My.Resources.VoltageDropUserControlStrings.EndResistance)), 0.0)
        End If

        If Not isUpdate Then
            rSum += _resistivityOfwireSum
        End If

        RgesUltraFTE.Text = String.Format(VoltageDropUserControlStrings.ResistanceUnit, Math.Round(rSum, 4).ToString)
        DeltaUUltaFTE.Text = String.Format(VoltageDropUserControlStrings.DeltaUUnit, Math.Round(InputCurrent * rSum, 2).ToString)  'I*Rges
        PvWattUltraFTE.Text = String.Format(VoltageDropUserControlStrings.PowerUnit, Math.Round(((Math.Pow(InputCurrent, 2) * rSum)), 2).ToString)  'I^2*Rges
        TotalConductorWeightUltraFTE.Text = String.Format(VoltageDropUserControlStrings.WeightUnit, Math.Round(conductorweight, 2).ToString)   'sum( wire conductorweight)
        TotalWeightUltraFTE.Text = String.Format(VoltageDropUserControlStrings.WeightUnit, Math.Round(TotalWeightOfEachWire, 2).ToString)   'sum( wire totalweight)
    End Sub

    Private Function ToCentimetre(length As Double) As Nullable(Of Double)
        Return UnitConverter.Convert(length, CalcUnit.MilliMetre, New CalcUnit(CalcUnit.UnitEnum.Metre, CalcUnit.UPrefixEnum.Centi, UDimensionEnum.None), True)
    End Function
    Private Sub ExportUltraBtn_Click(sender As Object, e As EventArgs) Handles ExportUltraBtn.Click

        Using sfdExcel As New SaveFileDialog
            With sfdExcel
                .DefaultExt = KnownFile.XLSX.Trim("."c)
                .FileName = String.Format("{0}{1}{2}_{3}_{4}.xlsx", Now.Year, Format(Now.Month, "00"), Format(Now.Day, "00"), Regex.Replace(_originWireNumberFromTable, "\W", "_"), VoltageDropUserControlStrings.VoltageDrop)
                .Filter = "Excel files (*.xlsx)|*.xlsx|Excel files (97-2003) (*.xls)|*.xls"
                .Title = String.Format(VoltageDropUserControlStrings.ExportTitel, _originWireNumberFromTable)

                If (.ShowDialog(Me) = DialogResult.OK) Then
                    Try
                        Me.EnableWaitCursor(WaitCursorEx.WaitCursorExStyle.WaitCursor)
                        Dim workbook As New Infragistics.Documents.Excel.Workbook()
                        Dim Worksheet As Infragistics.Documents.Excel.Worksheet = workbook.Worksheets.Add(_dt.TableName)
                        Dim rowIndex As Integer = 5
                        Dim totalwidth As Integer = 0

                        SetCellValueForWorksheet(Worksheet, 2, 0, InputCurrentUltralbl.Text, True)
                        SetCellValueForWorksheet(Worksheet, 2, 1, String.Format("{0} A", InputCurrentUNE.Value), False)

                        ' Create column headers for each column
                        For columnIndex As Integer = 0 To _dt.Columns.Count - 1
                            If _dt.Columns.Item(columnIndex).ColumnName = My.Resources.VoltageDropUserControlStrings.EndResistance Then
                                SetCellValueForWorksheet(Worksheet, 4, columnIndex, My.Resources.VoltageDropUserControlStrings.EndResistance, True)
                            Else
                                SetCellValueForWorksheet(Worksheet, 4, columnIndex, _dt.Columns.Item(columnIndex).ColumnName, True)
                            End If
                            Worksheet.Columns(columnIndex).AutoFitWidth()
                            totalwidth += Worksheet.Columns(columnIndex).Width
                        Next

                        SetImageForExcel(Worksheet, totalwidth) 'row 0

                        For Each dataRow As DataRow In _dt.Rows
                            rowIndex = rowIndex + 1
                            Dim row As Infragistics.Documents.Excel.WorksheetRow = Worksheet.Rows.Item(rowIndex)

                            For columnIndex As Integer = 0 To dataRow.ItemArray.Length - 1

                                row.Cells.Item(columnIndex).Value = SetValueWithUnit(_dt.Columns.Item(columnIndex).ColumnName, dataRow.ItemArray(columnIndex).ToString)
                                row.Cells.Item(columnIndex).CellFormat.Style.StyleFormat.Font.Bold = ExcelDefaultableBoolean.False
                                Worksheet.Columns(columnIndex).AutoFitWidth()
                            Next
                        Next

                        rowIndex += 2
                        SetCellValueForWorksheet(Worksheet, rowIndex, 0, Outputlbl.Text, True)

                        rowIndex += 1
                        SetCellValueForWorksheet(Worksheet, rowIndex, 0, RgesLbl.Text, False)
                        SetCellValueForWorksheet(Worksheet, rowIndex, 1, RgesUltraFTE.Value, False)

                        rowIndex += 1
                        SetCellValueForWorksheet(Worksheet, rowIndex, 0, DeltaULbl.Text, False)
                        SetCellValueForWorksheet(Worksheet, rowIndex, 1, DeltaUUltaFTE.Value, False)

                        rowIndex += 1
                        SetCellValueForWorksheet(Worksheet, rowIndex, 0, PvWattLbl.Text, False)
                        SetCellValueForWorksheet(Worksheet, rowIndex, 1, PvWattUltraFTE.Value, False)

                        rowIndex += 1
                        SetCellValueForWorksheet(Worksheet, rowIndex, 0, TotalConductorWeightLbl.Text, False)
                        SetCellValueForWorksheet(Worksheet, rowIndex, 1, TotalConductorWeightUltraFTE.Value, False)

                        rowIndex += 1
                        SetCellValueForWorksheet(Worksheet, rowIndex, 0, TotalWeightLbl.Text, False)
                        SetCellValueForWorksheet(Worksheet, rowIndex, 1, TotalWeightUltraFTE.Value, False)
                        Worksheet.Columns(0).AutoFitWidth()


                        If (IO.Path.GetExtension(.FileName).ToLower = ".xlsx") Then
                            workbook.SetCurrentFormat(WorkbookFormat.Excel2007)
                        Else
                            workbook.SetCurrentFormat(WorkbookFormat.Excel97To2003)
                        End If

                        workbook.Save(.FileName)

                        Me.Cursor = Cursors.Default

                        If Forms.MessageBox.Show(Me, My.Resources.VoltageDropUserControlStrings.OpenExporteExcel_Dlg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) = DialogResult.Yes Then
                            System.Diagnostics.ProcessEx.Start(.FileName)
                        End If

                    Catch ex As Exception
                        Forms.MessageBox.Show(Me, String.Format(ErrorStrings.VoltageDropExport_ErrorOccurred, vbCrLf, ex.Message), ErrorStrings.VoltageDropExport_ErrorTitel, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Try
                End If
            End With
        End Using


    End Sub
    Private Sub SetCellValueForWorksheet(Worksheet As Worksheet, rowIndex As Integer, cellindex As Integer, cellValue As Object, IsHeader As Boolean)
        If IsHeader Then
            Worksheet.Rows.Item(rowIndex).Cells.Item(cellindex).CellFormat.Style.StyleFormat.Font.Bold = ExcelDefaultableBoolean.True
            Worksheet.Rows.Item(rowIndex).Cells.Item(cellindex).CellFormat.Fill = CellFill.CreateSolidFill(Color.LightBlue)
        Else
            Worksheet.Rows.Item(rowIndex).Cells.Item(cellindex).CellFormat.Style.StyleFormat.Font.Bold = ExcelDefaultableBoolean.False
        End If

        Worksheet.Rows.Item(rowIndex).Cells.Item(cellindex).Value = cellValue
    End Sub
    Private Function SetValueWithUnit(columnName As String, value As String) As String
        Select Case columnName
            Case My.Resources.VoltageDropUserControlStrings.StartResistance
                Return String.Format(VoltageDropUserControlStrings.ResistanceUnit, value)
            Case My.Resources.VoltageDropUserControlStrings.WireOrCorenumber, My.Resources.VoltageDropUserControlStrings.TypeOfWire,
                 My.Resources.VoltageDropUserControlStrings.StartCPTerminalDescription,
                 My.Resources.VoltageDropUserControlStrings.Material, My.Resources.VoltageDropUserControlStrings.EndCPTerminalDescription
                Return value
            Case My.Resources.VoltageDropUserControlStrings.MaterialDensity
                Return String.Format(VoltageDropUserControlStrings.MaterialDensityUnit, value)
            Case My.Resources.VoltageDropUserControlStrings.ResistivityCustom
                Return String.Format(VoltageDropUserControlStrings.ResistivityUnit, value)
            Case My.Resources.VoltageDropUserControlStrings.WireResistance
                Return String.Format(VoltageDropUserControlStrings.ResistanceUnit, value)
            Case My.Resources.VoltageDropUserControlStrings.CSA
                Return String.Format(VoltageDropUserControlStrings.CsaUnit, value)
            Case My.Resources.VoltageDropUserControlStrings.Temperature
                Return String.Format(VoltageDropUserControlStrings.TemperatureUnit, value)
            Case My.Resources.VoltageDropUserControlStrings.Length
                Return String.Format(VoltageDropUserControlStrings.LengthUnit, value)
            Case My.Resources.VoltageDropUserControlStrings.ConductorWeight
                Return String.Format(VoltageDropUserControlStrings.WeightUnit, value)
            Case My.Resources.VoltageDropUserControlStrings.TotalWeight
                Return String.Format(VoltageDropUserControlStrings.WeightUnit, value)
            Case My.Resources.VoltageDropUserControlStrings.EndResistance
                Return String.Format(VoltageDropUserControlStrings.ResistanceUnit, value)
            Case Else
                Return String.Empty
        End Select
    End Function
    Private Sub SetImageForExcel(Worksheet As Infragistics.Documents.Excel.Worksheet, totalwidth As Integer)
        Dim imageStream As New MemoryStream
        Dim boundingBox As New Box(_selectedWireOrCore.vDraw.ActiveDocument.ActiveLayOut.Entities.GetBoundingBox(False, True))
        Dim widthLimitForImg As Integer = CInt(totalwidth * 0.5)
        Dim img As Bitmap = Nothing
        Dim width As Double, height As Double, ratio As Double

        ratio = widthLimitForImg / boundingBox.Width ' hint:fit the width of the image withing the limit

        width = boundingBox.Width * ratio
        height = boundingBox.Height * ratio

        img = New Bitmap(CInt(width), CInt(height))

        Dim graphics As Graphics = Graphics.FromImage(img)
        With graphics
            .SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            .TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias
            .InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low

            If (boundingBox.IsEmpty) Then
                .Clear(_selectedWireOrCore.vDraw.ActiveDocument.Palette.Background)
            Else
                boundingBox.TransformBy(_selectedWireOrCore.vDraw.ActiveDocument.ActiveLayOut.World2ViewMatrix)

                Dim backgroundColor As Color = _selectedWireOrCore.vDraw.ActiveDocument.Palette.Background
                With _selectedWireOrCore.vDraw.ActiveDocument
                    .ActiveLayOut.Label = "VoltageDrop"
                    .Palette.Background = Color.White
                    .ActiveLayOut.RenderToGraphics(graphics, boundingBox, img.Width, img.Height)
                    .Palette.Background = backgroundColor
                    .ActiveLayOut.Label = String.Empty

                End With

                img.Save(imageStream, Imaging.ImageFormat.Png)
            End If
        End With

        Dim mergedRegion1 As WorksheetMergedCellsRegion = Worksheet.MergedCellsRegions.Add(0, 0, 0, _dt.Columns.Count - 1)
        mergedRegion1.CellFormat.Alignment = HorizontalCellAlignment.Center

        Dim wsImage As New Infragistics.Documents.Excel.WorksheetImage(System.Drawing.Image.FromStream(imageStream))
        With wsImage
            .TopLeftCornerCell = Worksheet.Rows(0).Cells(0)
            .TopLeftCornerPosition = New PointF(0.0F, 0.0F)
            .BottomRightCornerCell = Worksheet.Rows(0).Cells(_dt.Columns.Count - 1)
            .BottomRightCornerPosition = New PointF(100.0F, 100.0F)
        End With

        Worksheet.Rows(0).Height = img.Height
        Worksheet.Shapes.Add(wsImage)
    End Sub

    Public Shared Shadows Function ShowDialog(owner As IWin32Window, harnessDescriptionAndColor As Dictionary(Of String, Color), originWireNumber As String, mappers As IEnumerable(Of KblMapper), wireadjacencies As List(Of WireAdjacency), defaultWireLengthType As String, ConnectivityOfSelectedWire As HarnessConnectivity, harnessesWithInactiveCoresWires As Dictionary(Of String, List(Of String)), overallConnectivity As OverallConnectivity, originHarnessConnectivity As HarnessConnectivity, Optional additionalResistanceMultiply As UInt32 = 0) As DialogResult
        Dim dlg As WireCurrentCalculatorForm = Nothing

        Try
            dlg = New WireCurrentCalculatorForm(mappers, harnessDescriptionAndColor, originWireNumber, wireadjacencies, defaultWireLengthType, ConnectivityOfSelectedWire, overallConnectivity, originHarnessConnectivity, harnessesWithInactiveCoresWires)
        Catch ex As Exception
            ex.ShowComponentCrashMessage(If(dlg?.Text, NameOf(WireCurrentCalculatorForm)), False)
            If dlg IsNot Nothing Then
                dlg.Dispose()
            End If
            Return DialogResult.Abort
        End Try

        If dlg IsNot Nothing Then
            Using dlg
                Return dlg.ShowDialog(owner)
            End Using
        End If

        Return DialogResult.Abort
    End Function
    Public Shadows Function ShowDialog(owner As IWin32Window) As DialogResult
        If (Not _selectedWireOrCore.IsSelectionInvalidErrorOccurred AndAlso Not _hasVoltageDropError) Then
            Return MyBase.ShowDialog(owner)
        End If
        If _selectedWireOrCore.IsSelectionInvalidErrorOccurred Then
            Return DialogResult.Cancel
        End If

        Return DialogResult.Abort
    End Function

    Public Shadows Function ShowDialog() As DialogResult
        If (Not _selectedWireOrCore.IsSelectionInvalidErrorOccurred AndAlso Not _hasVoltageDropError) Then
            Return MyBase.ShowDialog()
        End If

        Return DialogResult.Abort
    End Function

    Private Sub VoltageDropCntrl_UpdateResult(sender As VoltageDropUserControl, e As EventArgs) 'Handles _voltageDropCntrl.UpdateResult
        Getoutput(True)
    End Sub

    Private Sub InputCurrentUNE_ValueChanged(sender As Object, e As EventArgs) Handles InputCurrentUNE.ValueChanged
        Getoutput(True)
    End Sub

    Private Sub WireCurrentCalculatorForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        My.Settings.InitialCurrent = InputCurrent
        For Each cntrl As VoltageDropUserControl In _voltageDropCntrlsList
            cntrl.VoltageDropControls.Clear()
            RemoveHandler cntrl.UpdateResult, AddressOf VoltageDropCntrl_UpdateResult
            cntrl.Controls.Clear()
        Next

        _selectedWireOrCore.Controls.Clear()
        Me.Controls.Clear()
        _voltageDropCntrlsList.Clear()
    End Sub

    Private Sub InputCurrentUNE_MouseClick(sender As Object, e As MouseEventArgs) Handles InputCurrentUNE.MouseClick
        If Me.InputCurrentUNE IsNot Nothing AndAlso e.Button = MouseButtons.Left Then
            Me.InputCurrentUNE.Editor.Focus()
        End If
    End Sub

    Private Sub InputCurrentUNE_KeyDown(sender As Object, e As KeyEventArgs) Handles InputCurrentUNE.KeyDown
        If Me.InputCurrentUNE IsNot Nothing AndAlso e.KeyCode = Keys.Enter Then
            Me.InputCurrentUNE.Editor.ExitEditMode(True, True)
            Me.SelectNextControl(InputCurrentUNE, True, True, True, False)
        End If
    End Sub

    Private Sub RgesUltraFTE_AfterEnterEditMode(sender As Object, e As EventArgs) Handles RgesUltraFTE.AfterEnterEditMode
        If Me.RgesUltraFTE IsNot Nothing Then
            Me.RgesUltraFTE.Editor.ExitEditMode(True, True)
        End If
    End Sub

    Private Sub PvWattUltraFTE_AfterEnterEditMode(sender As Object, e As EventArgs) Handles PvWattUltraFTE.AfterEnterEditMode
        If Me.PvWattUltraFTE IsNot Nothing Then
            Me.PvWattUltraFTE.Editor.ExitEditMode(True, True)
        End If
    End Sub

    Private Sub DeltaUUltaFTE_AfterEnterEditMode(sender As Object, e As EventArgs) Handles DeltaUUltaFTE.AfterEnterEditMode
        If Me.DeltaUUltaFTE IsNot Nothing Then
            Me.DeltaUUltaFTE.Editor.ExitEditMode(True, True)
        End If
    End Sub

    Private Sub TotalWeightUltraFTE_AfterEnterEditMode(sender As Object, e As EventArgs) Handles TotalWeightUltraFTE.AfterEnterEditMode
        If Me.TotalWeightUltraFTE IsNot Nothing Then
            Me.TotalWeightUltraFTE.Editor.ExitEditMode(True, True)
        End If
    End Sub

    Private Sub TotalConductorWeightUltraFTE_AfterEnterEditMode(sender As Object, e As EventArgs) Handles TotalConductorWeightUltraFTE.AfterEnterEditMode
        If Me.TotalConductorWeightUltraFTE IsNot Nothing Then
            Me.TotalConductorWeightUltraFTE.Editor.ExitEditMode(True, True)
        End If
    End Sub

    Private Sub WireCurrentCalculatorForm_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        If _selectedWireOrCore IsNot Nothing Then
            _selectedWireOrCore.FitToPanel()
        End If
    End Sub
End Class