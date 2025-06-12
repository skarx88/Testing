Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Xml
Imports Infragistics.Documents.Excel
Imports Infragistics.Win
Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinGrid
Imports Infragistics.Win.UltraWinToolbars
Imports Zuken.E3.HarnessAnalyzer.Settings

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class BOMForm

    Private _activeModulePartNumbers As List(Of String)
    Private _contextMenu As PopupMenuTool
    Private _documentForm As DocumentForm
    Private _exportFileName As String

    'HINT these are hidden columns
    Private Const RAW_WEIGHT As String = "WeightVal"
    Private Const INITIAL_OCCURENCE_COUNT As String = "InitialOccCount"
    Private Const WEIGHT_ENTRY As String = "WeightEntry"
    Private Const UNIT_INFORMATION As String = "Unit"
    Private Const REPLACING_SEALS As String = "ReplacingSeals"
    Private Const INFO As String = "Info"
    Private _generalSettings As GeneralSettings

    Private Const APPROX_FACTOR As Single = 1.25

    Private _tapePattern As String
    Private _tubePattern As String
    Private _tapeList As New List(Of String)
    Private _tubeList As New List(Of String)

    Private Enum ContextMenuToolKey
        CollapseAll
        ExpandAll
    End Enum

    Friend Event SearchPartNo(partNo As String)

    Public Sub New(documentForm As DocumentForm)
        InitializeComponent()
        _generalSettings = documentForm.MainForm.GeneralSettings
        _contextMenu = New PopupMenuTool("GridContextMenu")
        _documentForm = documentForm

        Initialize()
        InitializeContextMenu()
    End Sub

    Private Function SelectPartNumberNode(node As XmlNode) As XmlNode
        Return node.SelectSingleNode("partNumber")
    End Function

    Private Function SelectDrawingNode(node As XmlNode) As XmlNode
        Return node.SelectSingleNode("drawing")
    End Function

    Private Function SelectSupplierNode(node As XmlNode) As XmlNode
        Return node.SelectSingleNode("supplier")
    End Function

    Private Function SelectDescriptionNode(node As XmlNode) As XmlNode
        Return node.SelectSingleNode("description")
    End Function

    Private Function SelectOccurencesNode(node As XmlNode) As XmlNode
        Return node.SelectSingleNode("occurences")
    End Function

    Private Function SelectVersionNode(node As XmlNode) As XmlNode
        Return node.SelectSingleNode("version")
    End Function

    Private Function SelectCodeNode(node As XmlNode) As XmlNode
        Return node.SelectSingleNode("codee")
    End Function

    Private Function GetCodeFromNode(node As XmlNode) As String
        Return SelectCodeNode(node)?.InnerText.Trim
    End Function

    Private Function GetOccurencesFromNode(node As XmlNode) As String
        Return SelectOccurencesNode(node)?.InnerText.Trim
    End Function

    Private Function GetPartNumberFromNode(node As XmlNode) As String
        Return SelectPartNumberNode(node)?.InnerText.Trim
    End Function

    Private Function GetDrawingFromNode(node As XmlNode) As String
        Return SelectVersionNode(node)?.InnerText.Trim
    End Function

    Private Function GetSupplierFromNode(node As XmlNode) As String
        Return SelectSupplierNode(node)?.InnerText.Trim
    End Function

    Private Function GetDescriptionFromNode(node As XmlNode) As String
        Return SelectDescriptionNode(node)?.InnerText.Trim
    End Function

    Private Function GetVersionFromNode(node As XmlNode) As String
        Return SelectVersionNode(node)?.InnerText.Trim
    End Function

    Private Sub Initialize()
        With _generalSettings
            _tapePattern = .ProtectionWeightCalculationTapePattern
            _tubePattern = .ProtectionWeightCalculationTubePattern
            _tapeList = .ProtectionWeightCalculationTapeList.Split(CChar(";")).ToList
            _tubeList = .ProtectionWeightCalculationTubeList.Split(CChar(";")).ToList
        End With
        Me.BackColor = Color.White
        Me.Icon = My.Resources.BOMAll

        Me.ugBOM.SyncWithCurrencyManager = False

        With Me.udsBOM
            .Band.Key = "HarnessModules"

            With .Band
                .Columns.Add(NameOf(BOMFormStrings.ModPartNumber_ColumnCaption))
                .Columns.Add(NameOf(BOMFormStrings.Version_ColumnCaption))
                .Columns.Add(NameOf(BOMFormStrings.Description_ColumnCaption))
                .Columns.Add(NameOf(BOMFormStrings.Weight_ColumnCaption))
                .Columns.Add(RAW_WEIGHT)
                .Columns.Add(NameOf(BOMFormStrings.Code_ColumnCaption))
                .Columns.Add(NameOf(BOMFormStrings.Flag_ColumnCaption))
            End With

            .Band.ChildBands.Add("Parts")

            With .Band.ChildBands(0)
                .Columns.Add(INFO)
                .Columns.Add(NameOf(BOMFormStrings.PartNumber_ColumnCaption))
                .Columns.Add(NameOf(BOMFormStrings.Drawing_ColumnCaption))
                .Columns.Add(NameOf(BOMFormStrings.Supplier_ColumnCaption))
                .Columns.Add(NameOf(BOMFormStrings.Occurrences_ColumnCaption))
                .Columns.Add(INITIAL_OCCURENCE_COUNT)
                .Columns.Add(NameOf(BOMFormStrings.Description_ColumnCaption))
                .Columns.Add(NameOf(BOMFormStrings.Weight_ColumnCaption))
                .Columns.Add(RAW_WEIGHT)
                .Columns.Add(WEIGHT_ENTRY)
                .Columns.Add(UNIT_INFORMATION)
                .Columns.Add(REPLACING_SEALS)
                .Columns.Add(NameOf(BOMFormStrings.Flag_ColumnCaption))
            End With

            Try
                Dim bomXMLDoc As New XmlDocument
                If (_documentForm.File.Index?.HasData).GetValueOrDefault Then
                    Using s As Stream = _documentForm.File.Index.GetDataAsStream
                        bomXMLDoc.Load(s)
                    End Using
                End If

                'Modules block
                For Each harnessModuleNode As XmlNode In bomXMLDoc.SelectNodes("//harnessContainer/harnessModules/harnessModule")

                    Dim harnessModule As [Module] = _documentForm.KBL.GetModules.Where(Function(m) AlignPartnumber(m.Part_number?.Trim) = AlignPartnumber(GetPartNumberFromNode(harnessModuleNode))).FirstOrDefault
                    Dim moduleRow As UltraDataRow = .Rows.Add

                    moduleRow.SetCellValue(NameOf(BOMFormStrings.ModPartNumber_ColumnCaption), GetPartNumberFromNode(harnessModuleNode))
                    moduleRow.SetCellValue(NameOf(BOMFormStrings.Version_ColumnCaption), GetVersionFromNode(harnessModuleNode))

                    If (SelectDescriptionNode(harnessModuleNode) IsNot Nothing) Then
                        moduleRow.SetCellValue(NameOf(BOMFormStrings.Description_ColumnCaption), GetDescriptionFromNode(harnessModuleNode))
                    End If

                    If (harnessModule IsNot Nothing) AndAlso (harnessModule.Mass_information IsNot Nothing) Then

                        If (_documentForm.KBL.KBLUnitMapper.ContainsKey(harnessModule.Mass_information.Unit_component)) Then
                            Dim unit As Unit = _documentForm.KBL.KBLUnitMapper(harnessModule.Mass_information.Unit_component)
                            If Not unit.Si_prefixSpecified Then
                                moduleRow.SetCellValue(NameOf(BOMFormStrings.Weight_ColumnCaption), String.Format("{0} {1}", Math.Round(harnessModule.Mass_information.Value_component, 1), unit.Unit_name))
                                moduleRow.SetCellValue(RAW_WEIGHT, harnessModule.Mass_information.Value_component)
                            ElseIf (unit.Si_prefix = SI_prefix.kilo) Then
                                moduleRow.SetCellValue(NameOf(BOMFormStrings.Weight_ColumnCaption), String.Format("{0} {1}", Math.Round(harnessModule.Mass_information.Value_component, 3), unit.Unit_name))
                                moduleRow.SetCellValue(RAW_WEIGHT, harnessModule.Mass_information.Value_component * 1000)
                            ElseIf (unit.Si_prefix = SI_prefix.milli) Then
                                moduleRow.SetCellValue(NameOf(BOMFormStrings.Weight_ColumnCaption), String.Format("{0} {1}", Math.Round(harnessModule.Mass_information.Value_component, 1), unit.Unit_name))
                                moduleRow.SetCellValue(RAW_WEIGHT, harnessModule.Mass_information.Value_component / 1000)
                            End If
                        Else
                            moduleRow.SetCellValue(NameOf(BOMFormStrings.Weight_ColumnCaption), Math.Round(harnessModule.Mass_information.Value_component, 1))
                        End If
                    Else
                        moduleRow.SetCellValue(NameOf(BOMFormStrings.Weight_ColumnCaption), Zuken.E3.HarnessAnalyzer.Shared.NOT_AVAILABLE)
                        moduleRow.SetCellValue(RAW_WEIGHT, 0)
                    End If

                    If (SelectCodeNode(harnessModuleNode) IsNot Nothing) Then
                        moduleRow.SetCellValue(NameOf(BOMFormStrings.Code_ColumnCaption), GetCodeFromNode(harnessModuleNode))
                    End If

                    moduleRow.SetCellValue(NameOf(BOMFormStrings.Flag_ColumnCaption), harnessModuleNode.SelectSingleNode("kzflag").InnerText)

                    'Parts on module block
                    For Each partNode As XmlNode In harnessModuleNode.SelectNodes("part")
                        Dim pn As String = GetPartNumberFromNode(partNode)
                        Dim drawingText As String = GetDrawingFromNode(partNode)
                        Dim supplierText As String = GetSupplierFromNode(partNode)
                        Dim descriptionText As String = GetDescriptionFromNode(partNode)

                        Dim part As IKblPartObject = _documentForm.KBL.GetPartByNumnber(pn)
                        Dim partRow As UltraDataRow = moduleRow.GetChildRows(.Band.ChildBands(0)).Add

                        partRow.SetCellValue(NameOf(BOMFormStrings.PartNumber_ColumnCaption), pn)

                        If (SelectDrawingNode(partNode) IsNot Nothing) Then
                            partRow.SetCellValue(NameOf(BOMFormStrings.Drawing_ColumnCaption), drawingText)
                        End If

                        If (SelectSupplierNode(partNode) IsNot Nothing) Then
                            partRow.SetCellValue(NameOf(BOMFormStrings.Supplier_ColumnCaption), supplierText)
                        End If

                        Dim occurenceCount As Integer = 0
                        Dim valStr As String = GetOccurencesFromNode(partNode)
                        If Not String.IsNullOrEmpty(valStr) AndAlso IsNumeric(valStr) Then
                            occurenceCount = CInt(valStr)
                        End If

                        partRow.SetCellValue(NameOf(BOMFormStrings.Occurrences_ColumnCaption), occurenceCount)
                        partRow.SetCellValue(INITIAL_OCCURENCE_COUNT, occurenceCount)
                        partRow.SetCellValue(NameOf(BOMFormStrings.Description_ColumnCaption), descriptionText)

                        partRow.SetCellValue(REPLACING_SEALS, String.Empty)

                        partRow.SetCellValue(WEIGHT_ENTRY, 0.0)

                        If (part IsNot Nothing) AndAlso (part.Mass_information IsNot Nothing) Then

                            If TypeOf part Is Wire_protection Then
                                Dim p As Wire_protection = CType(part, Wire_protection)
                                Dim myType As String = p.Protection_type


                                If harnessModule IsNot Nothing AndAlso Not String.IsNullOrEmpty(myType) AndAlso (_tapeList.Contains(myType.ToLower) OrElse Regex.Matches(myType, _tapePattern).Count > 0) Then
                                    Dim ProtectionCalculation As ProtectionWeigthCalculation = New ProtectionWeigthCalculation(harnessModule, _documentForm.KBL, CType(part, Wire_protection), _generalSettings, True)
                                    FillProtectionWeightValues(partRow, ProtectionCalculation)

                                ElseIf harnessModule IsNot Nothing AndAlso Not String.IsNullOrEmpty(myType) AndAlso (_tubeList.Contains(myType.ToLower) OrElse Regex.Matches(myType, _tubePattern).Count > 0) Then
                                    Dim isTape As Boolean = False
                                    Dim ProtectionCalculation As ProtectionWeigthCalculation = New ProtectionWeigthCalculation(harnessModule, _documentForm.KBL, CType(part, Wire_protection), _generalSettings, False)
                                    FillProtectionWeightValues(partRow, ProtectionCalculation)

                                End If

                            Else
                                If (IsNumeric(part.Mass_information.Value_component)) Then
                                    partRow.SetCellValue(WEIGHT_ENTRY, part.Mass_information.Value_component)
                                Else
                                    partRow.SetCellValue(WEIGHT_ENTRY, 0.0)
                                End If

                                partRow.SetCellValue(UNIT_INFORMATION, String.Empty)

                                If (_documentForm.KBL.KBLUnitMapper.ContainsKey(part.Mass_information.Unit_component)) Then
                                    Dim unit As Unit = _documentForm.KBL.KBLUnitMapper(part.Mass_information.Unit_component)

                                    If (unit.Si_prefixSpecified) Then
                                        partRow.SetCellValue(UNIT_INFORMATION, String.Format("{0};{1}", unit.Unit_name.ToLower, unit.Si_prefix))
                                    Else
                                        partRow.SetCellValue(UNIT_INFORMATION, String.Format("{0};{1}", unit.Unit_name.ToLower, ""))
                                    End If
                                End If
                            End If
                        Else
                            partRow.SetCellValue(UNIT_INFORMATION, DBNull.Value)
                            partRow.SetCellValue(WEIGHT_ENTRY, 0.0)
                            partRow.SetCellValue(NameOf(BOMFormStrings.Weight_ColumnCaption), Zuken.E3.HarnessAnalyzer.Shared.NOT_AVAILABLE)
                            partRow.SetCellValue(RAW_WEIGHT, 0.0)
                        End If

                        partRow.SetCellValue(NameOf(BOMFormStrings.Flag_ColumnCaption), partNode.SelectSingleNode("kzflag").InnerText)
                    Next
                Next

                bomXMLDoc = Nothing
            Catch ex As Exception
                ex.ShowMessageBox(String.Format(BOMFormStrings.ErrorInitBOM_Msg, vbCrLf, ex.Message))
            End Try
        End With

        Me.ugBOM.DataSource = Me.udsBOM
    End Sub
    Private Sub FillProtectionWeightValues(partRow As UltraDataRow, Optional Calculation As ProtectionWeigthCalculation = Nothing)
        If Calculation Is Nothing Then
            partRow.SetCellValue(UNIT_INFORMATION, DBNull.Value)
            partRow.SetCellValue(WEIGHT_ENTRY, 0.0)
            partRow.SetCellValue(BOMFormStrings.Weight_ColumnCaption, Zuken.E3.HarnessAnalyzer.Shared.NOT_AVAILABLE)
            partRow.SetCellValue(RAW_WEIGHT, 0.0)
        Else

            If Not String.IsNullOrEmpty(Calculation.Info.GetErrors) Then
                partRow.SetCellValue(INFO, Calculation.Info.GetErrors)
            ElseIf Not String.IsNullOrEmpty(Calculation.Info.GetInfos) Then
                partRow.SetCellValue(INFO, Calculation.Info.GetInfos)
            End If

            If (Calculation.n > 0) Then
                partRow.SetCellValue(INITIAL_OCCURENCE_COUNT, 1)
            End If

            partRow.SetCellValue(UNIT_INFORMATION, Calculation.weightWithUnit.Unit_component)
            partRow.SetCellValue(WEIGHT_ENTRY, Calculation.weightWithUnit.Value_component)
            partRow.SetCellValue(NameOf(BOMFormStrings.Weight_ColumnCaption), String.Format("{0} {1}", Calculation.weightWithUnit.Value_component, Calculation.weightWithUnit.Unit_component))
            'partRow.SetCellValue(NameOf(BOMFormStrings.Weight_ColumnCaption), String.Format("{0}", Calculation.weightWithUnit.Value_component))

            If Calculation.weightWithUnit.Unit_component = "g" Then
                partRow.SetCellValue(RAW_WEIGHT, Calculation.GetRoundedVal(Calculation.weightWithUnit.Value_component))
            ElseIf Calculation.weightWithUnit.Unit_component = "kg" Then
                partRow.SetCellValue(RAW_WEIGHT, Calculation.weightWithUnit.Value_component / 1000)
            Else
                partRow.SetCellValue(RAW_WEIGHT, 0.0)
            End If
        End If

    End Sub
    Private Sub InitializeContextMenu()
        With _contextMenu
            .DropDownArrowStyle = DropDownArrowStyle.None

            Dim collapseAllButton As New ButtonTool(ContextMenuToolKey.CollapseAll.ToString)
            collapseAllButton.SharedProps.Caption = InformationHubStrings.CollapseAll_CtxtMnu_Caption
            collapseAllButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.CollapseAll.ToBitmap

            Dim expandAllButton As New ButtonTool(ContextMenuToolKey.ExpandAll.ToString)
            expandAllButton.SharedProps.Caption = InformationHubStrings.ExpandAll_CtxtMnu_Caption
            expandAllButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.ExpandAll.ToBitmap

            Me.utmBOM.Tools.AddRange(New ToolBase() {_contextMenu, collapseAllButton, expandAllButton})

            .Tools.AddTool(collapseAllButton.Key)
            .Tools.AddTool(expandAllButton.Key)
        End With
    End Sub


    Private Sub BOMForm_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        ugBOM.BeginUpdate()
        RestoreOccurenceCount()
        RestoreModuleRows()

        If (_activeModulePartNumbers IsNot Nothing) Then
            Me.Text = BOMFormStrings.Caption1

            For Each row As UltraGridRow In Me.ugBOM.Rows
                If (Not _activeModulePartNumbers.Contains(AlignPartnumber(row.Cells(NameOf(BOMFormStrings.ModPartNumber_ColumnCaption)).Value.ToString))) Then
                    row.Hidden = True
                End If
            Next
            AdjustPlugParts(GetPlugReplacements())
        Else
            Me.Text = BOMFormStrings.Caption2
        End If
        ugBOM.EndUpdate()
    End Sub

    Public Shared Function AlignPartnumber(pn As String) As String
        'HINT Removes the whitespace on older Daimler Partnumbers
        Return Regex.Replace(pn, "\s", String.Empty)
    End Function

    Private Sub RestoreModuleRows()
        For Each row As UltraGridRow In Me.ugBOM.Rows
            row.Hidden = False
        Next
    End Sub

    Private Sub RestoreOccurenceCount()
        For Each row As UltraDataRow In Me.udsBOM.Rows
            For Each partRow As UltraDataRow In row.GetChildRows(0)
                partRow.SetCellValue(NameOf(BOMFormStrings.Occurrences_ColumnCaption), partRow.GetCellValue(INITIAL_OCCURENCE_COUNT))
                partRow.SetCellValue(REPLACING_SEALS, String.Empty)
                RecalculateWeightRow(partRow)
            Next
        Next
    End Sub

    Private Sub RecalculateWeightRow(partRow As UltraDataRow)
        'HINT: if Unit is dbnull then we have n/a entry !

        If Not IsDBNull(partRow.GetCellValue(UNIT_INFORMATION)) Then
            Dim i As Integer = CInt(partRow.GetCellValue(NameOf(BOMFormStrings.Occurrences_ColumnCaption)))
            Dim part_weight_entry As Double = CDbl(partRow.GetCellValue(WEIGHT_ENTRY))
            Dim numberOfParts As Integer = CInt(partRow.GetCellValue(NameOf(BOMFormStrings.Occurrences_ColumnCaption)))
            Dim val As Double = CInt(partRow.GetCellValue(NameOf(BOMFormStrings.Occurrences_ColumnCaption))) * part_weight_entry

            Dim unitName As String = String.Empty
            Dim siPrefix As String = String.Empty

            Dim unit_info_parts As String() = CStr(partRow.GetCellValue(UNIT_INFORMATION)).Split(";"c)

            unitName = unit_info_parts(0)
            If unit_info_parts.Length > 1 Then siPrefix = unit_info_parts(1)

            If String.IsNullOrEmpty(siPrefix) Then
                If (unitName = "g/m") Then  'HINT this is a hack, length dependent data is given in g/m and the occurences are without unit but for now typically in mm
                    val /= 1000
                    unitName = "g"
                End If
                Dim value As String = String.Format("{0} {1}", Math.Round(val, 1), unitName)
                'Dim value As String = String.Format("{0}", Math.Round(val, 1))

                partRow.SetCellValue(NameOf(BOMFormStrings.Weight_ColumnCaption), value)
                partRow.SetCellValue(RAW_WEIGHT, val)

            ElseIf (siPrefix = SI_prefix.kilo.ToString) Then
                Dim value As String = String.Format("{0} {1}", Math.Round(val, 3), unitName)
                'Dim value As String = String.Format("{0}", Math.Round(val, 3))
                partRow.SetCellValue(RAW_WEIGHT, Math.Round(val * 1000, 3))

            ElseIf (siPrefix = SI_prefix.milli.ToString) Then

                Dim value As String = String.Format("{0} {1}", Math.Round(val, 1), unitName)
                'Dim value As String = String.Format("{0}", Math.Round(val, 1))

                partRow.SetCellValue(NameOf(BOMFormStrings.Weight_ColumnCaption), value)
                partRow.SetCellValue(RAW_WEIGHT, val / 1000)
            End If
        End If
    End Sub

    Private Sub ugBOM_InitializeRow(sender As Object, e As InitializeRowEventArgs) Handles ugBOM.InitializeRow
        If (e.Row.Band.Key = "Parts") Then
            Dim isCalculated As Boolean = False

            If TypeOf e.Row.Cells(INFO).Value Is String Then
                Dim val As String = e.Row.Cells(INFO).Value.ToString
                If val.StartsWith(BOMFormStrings.Error_) Then
                    e.Row.Cells(1).Appearance.Image = My.Resources.Warning
                Else
                    e.Row.Cells(1).Appearance.Image = My.Resources.information
                End If
                e.Row.Cells(NameOf(BOMFormStrings.Weight_ColumnCaption)).Appearance.FontData.Italic = Infragistics.Win.DefaultableBoolean.True
            End If

            e.Row.Cells(NameOf(BOMFormStrings.Occurrences_ColumnCaption)).Appearance.FontData.Italic = Infragistics.Win.DefaultableBoolean.False
            e.Row.Appearance.FontData.Strikeout = Infragistics.Win.DefaultableBoolean.False
            If (IsNumeric(e.Row.Cells(NameOf(BOMFormStrings.Occurrences_ColumnCaption)).Value) AndAlso IsNumeric(e.Row.Cells(INITIAL_OCCURENCE_COUNT).Value)) Then
                Dim occurences As Integer = CInt(e.Row.Cells(NameOf(BOMFormStrings.Occurrences_ColumnCaption)).Value)
                Dim initialOccurences As Integer = CInt(e.Row.Cells(INITIAL_OCCURENCE_COUNT).Value)
                If (occurences <> initialOccurences) Then
                    e.Row.Cells(NameOf(BOMFormStrings.Occurrences_ColumnCaption)).Appearance.FontData.Italic = Infragistics.Win.DefaultableBoolean.True
                End If
                If (occurences = 0) Then
                    e.Row.Appearance.FontData.Strikeout = Infragistics.Win.DefaultableBoolean.True
                End If
            End If


            If (String.IsNullOrEmpty(CStr(e.Row.Cells(REPLACING_SEALS).Value))) Then
                e.Row.ToolTipText = String.Empty
            Else
                e.Row.ToolTipText = String.Format(BOMFormStrings.Replacing_Seals, CStr(e.Row.Cells(NameOf(BOMFormStrings.PartNumber_ColumnCaption)).Value), CStr(e.Row.Cells(REPLACING_SEALS).Value))
            End If
        End If
    End Sub

    Private Sub AdjustPlugParts(replacements As Dictionary(Of String, List(Of Tuple(Of String, String))))
        For Each kvp As KeyValuePair(Of String, List(Of Tuple(Of String, String))) In replacements
            For Each row As UltraDataRow In Me.udsBOM.Rows
                If (kvp.Key = AlignPartnumber(row.GetCellValue(NameOf(BOMFormStrings.ModPartNumber_ColumnCaption)).ToString)) Then
                    For Each plugPn As Tuple(Of String, String) In kvp.Value
                        For Each partRow As UltraDataRow In row.GetChildRows(0)
                            If (plugPn.Item1 = AlignPartnumber(partRow.GetCellValue(NameOf(BOMFormStrings.PartNumber_ColumnCaption)).ToString)) Then
                                If (IsNumeric(partRow.GetCellValue(NameOf(BOMFormStrings.Occurrences_ColumnCaption)))) Then
                                    Dim count As Integer = CInt(partRow.GetCellValue(NameOf(BOMFormStrings.Occurrences_ColumnCaption)))
                                    count -= 1
                                    If (count < 0) Then
                                        count = 0
                                    End If

                                    partRow.SetCellValue(NameOf(BOMFormStrings.Occurrences_ColumnCaption), count.ToString)

                                    If (String.IsNullOrEmpty(partRow.GetCellValue(REPLACING_SEALS).ToString)) Then
                                        partRow.SetCellValue(REPLACING_SEALS, plugPn.Item2)
                                    Else
                                        If (Not partRow.GetCellValue(REPLACING_SEALS).ToString.Contains(plugPn.Item2)) Then
                                            partRow.SetCellValue(REPLACING_SEALS, String.Format("{0},{1}", partRow.GetCellValue(REPLACING_SEALS).ToString, plugPn.Item2))
                                        End If
                                    End If

                                    RecalculateWeightRow(partRow)
                                End If
                            End If
                        Next
                    Next
                End If
            Next
        Next
    End Sub

    Private Function GetPlugReplacements() As Dictionary(Of String, List(Of Tuple(Of String, String)))
        'HINT: beware due to alternative modules, we could have more replacements than actually in the part list!
        Dim plugReplacements As New Dictionary(Of String, List(Of Tuple(Of String, String))) 'modPartnumber, list of [plugpartnumber,SealPartnuber]
        For Each seal As Cavity_seal_occurrence In _documentForm.KBL.GetCavitySealOccurrences
            If seal.Replacing IsNot Nothing AndAlso seal.Replacing.Length > 0 Then
                For Each m As [Module] In _documentForm.KBL.GetModulesOfObject(seal.SystemId)
                    If _activeModulePartNumbers.Contains(AlignPartnumber(m.Part_number)) Then
                        For Each plg As Part_substitution In seal.Replacing
                            If Not String.IsNullOrEmpty(plg.Replaced) Then
                                For Each m2 As [Module] In _documentForm.KBL.GetModulesOfObject(plg.Replaced)
                                    Dim plugPartId As String = _documentForm.KBL.GetOccurrenceObject(Of Cavity_plug_occurrence)(plg.Replaced)?.Part
                                    If Not String.IsNullOrEmpty(plugPartId) Then
                                        Dim modpan As String = AlignPartnumber(m2.Part_number)
                                        Dim plgPartNumber As String = CType(_documentForm.KBL.KBLPartMapper(plugPartId), Part).Part_number
                                        Dim sealPartNumber As String = String.Empty
                                        If (_documentForm.KBL.KBLPartMapper.ContainsKey(seal.Part)) Then
                                            sealPartNumber = CType(_documentForm.KBL.KBLPartMapper(seal.Part), Part).Part_number
                                        End If

                                        If Not plugReplacements.ContainsKey(modpan) Then
                                            plugReplacements.Add(modpan, New List(Of Tuple(Of String, String)))
                                        End If

                                        plugReplacements(modpan).Add(New Tuple(Of String, String)(plgPartNumber, sealPartNumber))
                                    End If
                                Next
                            End If
                        Next
                    End If
                Next
            End If
        Next
        Return plugReplacements
    End Function

    Private Sub BOMForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If e.CloseReason = System.Windows.Forms.CloseReason.UserClosing Then
            Me.Hide()
            e.Cancel = True
        End If
    End Sub

    Private Sub BOMForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If (e.KeyCode = Keys.Escape) Then
            Me.Close()
        End If
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Hide()
    End Sub

    Private Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        Using sfdExcel As New SaveFileDialog
            With sfdExcel
                .DefaultExt = KnownFile.XLSX.Trim("."c)

                If (_documentForm.KBL.GetChanges.Any()) Then
                    .FileName = String.Format("{0}{1}{2}_{3}_{4}_BOM.xlsx", Now.Year, Format(Now.Month, "00"), Format(Now.Day, "00"), Replace(_documentForm.KBL.HarnessPartNumber, " ", String.Empty), _documentForm.KBL.GetChanges.Max(Function(change) change.Id))
                Else
                    .FileName = String.Format("{0}{1}{2}_{3}_BOM.xlsx", Now.Year, Format(Now.Month, "00"), Format(Now.Day, "00"), Replace(_documentForm.KBL.HarnessPartNumber, " ", String.Empty))
                End If

                .Filter = "Excel files (*.xlsx)|*.xlsx|Excel files (97-2003) (*.xls)|*.xls"
                .Title = BOMFormStrings.ExportBOMToExcelFile_Title

                If (.ShowDialog(Me) = DialogResult.OK) Then
                    Try
                        _exportFileName = .FileName

                        Dim workbook As New Workbook

                        If (IO.Path.GetExtension(_exportFileName).ToLower = ".xlsx") Then
                            workbook.SetCurrentFormat(WorkbookFormat.Excel2007)
                        Else
                            workbook.SetCurrentFormat(WorkbookFormat.Excel97To2003)
                        End If

                        workbook.Worksheets.Add(GraphicalCompareFormStrings.Worksheet_Name)

                        Me.ugeeBOM.ExportAsync(Me.ugBOM, workbook, 1, 0)
                    Catch ex As Exception
                        ex.ShowMessageBox(String.Format(BOMFormStrings.ErrorExportBOM_Msg, vbCrLf, ex.Message))
                    End Try
                End If
            End With
        End Using
    End Sub

    Private Sub ugBOM_DoubleClickCell(sender As Object, e As DoubleClickCellEventArgs) Handles ugBOM.DoubleClickCell
        If (e.Cell.Row.HasChild) Then
            RaiseEvent SearchPartNo(e.Cell.Row.Cells(NameOf(BOMFormStrings.ModPartNumber_ColumnCaption)).Value.ToString)
        Else
            RaiseEvent SearchPartNo(e.Cell.Row.Cells(NameOf(BOMFormStrings.PartNumber_ColumnCaption)).Value.ToString)
        End If
    End Sub

    Private Sub ugBOM_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugBOM.InitializeLayout
        Me.ugBOM.BeginUpdate()

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
                        column.Header.Caption = BOMFormStrings.ResourceManager.GetString(column.Key)
                        If Not column.Hidden Then
                            With column
                                .CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center
                                .CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle
                                .Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center
                                .Header.Appearance.TextVAlign = Infragistics.Win.VAlign.Middle
                                If (.Key) = INFO Then .Hidden = True
                                If .Key = NameOf(BOMFormStrings.Flag_ColumnCaption) Then
                                    .Style = ColumnStyle.CheckBox
                                End If

                                If .Key = NameOf(BOMFormStrings.Weight_ColumnCaption) Then
                                    .SortComparer = New NumericStringSortComparer
                                End If

                                If .Key = RAW_WEIGHT Then
                                    .AllowRowSummaries = AllowRowSummaries.True
                                    .Hidden = True

                                    Dim calc As ICustomSummaryCalculator = New CustomWeightSummaryCalculator
                                    Dim sumSettings As SummarySettings = band.Summaries.Add(SummaryType.Custom, calc, column, SummaryPosition.Left, Nothing)
                                    sumSettings.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True
                                    sumSettings.DisplayFormat = "{0}"

                                    band.SummaryFooterCaption = BOMFormStrings.AccWeigth_SummaryRowCaption
                                    band.Override.SummaryFooterCaptionAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True
                                End If

                                If .Key = INITIAL_OCCURENCE_COUNT OrElse .Key = WEIGHT_ENTRY OrElse .Key = UNIT_INFORMATION OrElse .Key = REPLACING_SEALS Then
                                    .Hidden = True
                                End If

                                If .Index = 0 Then
                                    .SortIndicator = SortIndicator.Ascending
                                End If
                            End With
                        End If
                    Next
                End With
            Next
        End With

        Me.ugBOM.EndUpdate()
    End Sub

    Private Sub ugBOM_MouseClick(sender As Object, e As MouseEventArgs) Handles ugBOM.MouseClick
        If e.Button = System.Windows.Forms.MouseButtons.Right Then
            Dim element As Infragistics.Win.UIElement = Me.ugBOM.DisplayLayout.UIElement.LastElementEntered
            Dim header As ColumnHeader = TryCast(element.GetContext(GetType(ColumnHeader)), ColumnHeader)
            If header IsNot Nothing Then
                _contextMenu.ShowPopup()
            End If
        End If
    End Sub

    Private Sub ugeeBOM_ExportEnded(sender As Object, e As ExcelExport.ExportEndedEventArgs) Handles ugeeBOM.ExportEnded
        Try
            e.Workbook.Save(_exportFileName)
        Catch ex As Exception
            ex.ShowMessageBox(String.Format(BOMFormStrings.ErrorExportBOM_Msg, vbCrLf, ex.Message))
            Return
        End Try

        If (Not e.Canceled) AndAlso (MessageBoxEx.ShowQuestion(BOMFormStrings.ExportExcelFinished_Msg) = System.Windows.Forms.DialogResult.Yes) Then
            ProcessEx.Start(_exportFileName)
        End If
    End Sub

    Private Sub ugeeBOM_HeaderRowExporting(sender As Object, e As ExcelExport.HeaderRowExportingEventArgs) Handles ugeeBOM.HeaderRowExporting
        If (e.CurrentRowIndex <> 0) AndAlso (e.CurrentOutlineLevel = 0) Then
            e.Cancel = True
        End If
    End Sub

    Private Sub utmBOM_ToolClick(sender As Object, e As ToolClickEventArgs) Handles utmBOM.ToolClick
        Select Case e.Tool.Key
            Case ContextMenuToolKey.CollapseAll.ToString
                With Me.ugBOM
                    .BeginUpdate()
                    .Rows.CollapseAll(True)
                    .EndUpdate()
                End With
            Case ContextMenuToolKey.ExpandAll.ToString
                With Me.ugBOM
                    .BeginUpdate()
                    .Rows.ExpandAll(True)
                    .EndUpdate()
                End With
        End Select
    End Sub



    Private Sub ugBOM_MouseEnterElement(sender As Object, e As UIElementEventArgs) Handles ugBOM.MouseEnterElement
        If TypeOf e.Element Is CellUIElement Then
            Dim cell As UltraGridCell = CType(e.Element, CellUIElement).Cell
            If cell.Column.Index = 1 Then
                Dim rw As UltraGridRow = cell.Row
                If rw.Band.Key = "Parts" Then
                    If TypeOf rw.GetCellValue(INFO) Is String Then
                        cell.ToolTipText = rw.GetCellValue(INFO).ToString
                    End If
                End If
            End If
        End If


    End Sub

    Friend Property ActiveModulePartNumbers() As List(Of String)
        Get
            Return _activeModulePartNumbers
        End Get
        Set(value As List(Of String))
            _activeModulePartNumbers = value
        End Set
    End Property

    Private Class CustomWeightSummaryCalculator
        Implements ICustomSummaryCalculator

        Private _weight As Double = 0

        Public Sub BeginCustomSummary(summarySettings As SummarySettings, rows As RowsCollection) Implements ICustomSummaryCalculator.BeginCustomSummary
            _weight = 0
        End Sub

        Public Sub AggregateCustomSummary(summarySettings As SummarySettings, row As UltraGridRow) Implements ICustomSummaryCalculator.AggregateCustomSummary
            _weight += CDbl(row.Cells(summarySettings.SourceColumn).Value)
            Debug.WriteLine(CDbl(row.Cells(summarySettings.SourceColumn).Value))
        End Sub

        Public Function EndCustomSummary(summarySettings As SummarySettings, rows As RowsCollection) As Object Implements ICustomSummaryCalculator.EndCustomSummary
            If (_weight > 1000) Then
                _weight /= 1000.0
                Return String.Format("ca. {0} kg", Math.Round(_weight, 2).ToString)
            Else
                Return String.Format("ca. {0} g", Math.Round(_weight, 1).ToString)
            End If

        End Function
    End Class

    Private Class ProtectionWeigthCalculation
        Public area As Double = 0
        Public weight As Double = 0
        Public weightWithUnit As New Numerical_value

        Public n As Integer = 0
        Public Info As New CalculationInfo
        Public calcLength As Double

        Private _minUnitWeight As Single
        Private _min_mm2 As Single
        Private _max_mm2 As Single

        Private _min_mm As Single
        Private _max_mm As Single

        Public Sub New(modul As [Module], kblMapper As KblMapper, p As Wire_protection, generalSettings As GeneralSettings, Optional IsTape As Boolean = False)

            With generalSettings

                _minUnitWeight = .ProtectionWeightCalculationMinWeight
                _min_mm2 = .ProtectionWeightCalculationMin_mm2
                _max_mm2 = .ProtectionWeightCalculationMax_mm2
                _min_mm = .ProtectionWeightCalculationMin_mm
                _max_mm = .ProtectionWeightCalculationMax_mm

            End With


            Dim sArea As Double = 0
            Dim unitName As String = ""
            calcLength = 0.0
            Dim wireProtectionOccurrences As List(Of Wire_protection_occurrence) = kblMapper.GetWireProtectionOccurrences().Where(Function(prot) prot.Part = p.SystemId AndAlso modul.Module_configuration.Controlled_components.Contains(prot.SystemId)).ToList
            weightWithUnit.Unit_component = "g"


            unitName = GetUnitName(kblMapper, p)

            For Each occurrence As Wire_protection_occurrence In wireProtectionOccurrences
                For Each id As String In kblMapper.KBLProtectionSegmentMapper.Item(occurrence.SystemId)
                    If IsInModule(modul, kblMapper, id) Then

                        If IsNumeric(p.Mass_information.Value_component) AndAlso (unitName.ToLower = "kg" OrElse unitName.ToLower = "g" OrElse unitName.ToLower = "mg") Then

                            Dim w As Double = 0
                            If unitName.ToLower = "kg" Then
                                w = p.Mass_information.Value_component * 10 ^ 3
                            ElseIf unitName.ToLower = "g" Then
                                w = p.Mass_information.Value_component
                            ElseIf unitName.ToLower = "mg" Then
                                w = p.Mass_information.Value_component * 10 ^ -3
                            End If
                            If w < _minUnitWeight Then
                                Info.Err.Add(BOMFormStrings.Err_Weight + ": " + w.ToString + "g")
                            End If
                            weight += w
                            n += 1
                        Else

                            Dim s As Segment = kblMapper.GetSegment(id)

                            If s IsNot Nothing Then

                                sArea = getarea(kblMapper, s)
                                If sArea < 0 Then
                                    Info.Err.Add("Could not calculate diameter!")
                                Else
                                    If IsTape Then sArea = APPROX_FACTOR * sArea

                                    Dim unitInformation As Numerical_value = GetUnitInformation(unitName)
                                    Dim specWeight As Double
                                    Dim w As Double = 0.0
                                    If IsNumeric(p.Mass_information.Value_component) Then specWeight = p.Mass_information.Value_component * unitInformation.Value_component

                                    If unitInformation.Unit_component = "mm2" Then
                                        If specWeight < _min_mm2 OrElse specWeight > _max_mm2 Then

                                            Dim err As String = String.Format(BOMFormStrings.Err_outsideValueRange, p.Mass_information.Value_component.ToString, unitName)
                                            Info.Err.Add(err)
                                        End If
                                        w += specWeight * sArea
                                        weight += w
                                        n += 1
                                    ElseIf unitInformation.Unit_component = "mm" Then
                                        specWeight = p.Mass_information.Value_component * unitInformation.Value_component
                                        If specWeight < _min_mm OrElse specWeight > _max_mm Then
                                            Dim err As String = String.Format(BOMFormStrings.Err_outsideValueRange, p.Mass_information.Value_component.ToString, unitName)
                                            Info.Err.Add(err)
                                        End If
                                        w += specWeight * sArea
                                        weight += w
                                        n += 1

                                    End If

                                End If
                            End If
                        End If
                    End If
                Next
            Next
            Dim myWeightUnitName As String = ""
            If weight > 500 Then
                weight = weight * 10 ^ -3
                weight = Math.Round(weight, 3)

                weightWithUnit.Value_component = weight
                myWeightUnitName = "kg"
            Else
                weight = Math.Round(weight, 3)
                weightWithUnit.Value_component = weight
                myWeightUnitName = "g"
            End If
            weightWithUnit.Unit_component = myWeightUnitName

            Info.Inf.Add(String.Format(BOMFormStrings.TotalLength, GetRoundedVal(calcLength).ToString()))

        End Sub

        Public Function GetRoundedVal(val As Double) As Double
            Dim res As Double = val
            If val < 0.1 Then
                res = Math.Round(val, 3)
            ElseIf val < 1 Then
                res = Math.Round(val, 2)
            ElseIf val < 10 Then
                res = Math.Round(val, 1)
            Else
                res = Math.Round(val, 0)
            End If
            Return res
        End Function
        Private Function IsInModule(m As [Module], kbl As KblMapper, id As String) As Boolean
            Dim res As Boolean
            If kbl.KBLObjectModuleMapper.ContainsKey(id) Then
                Dim test As List(Of String) = kbl.KBLObjectModuleMapper.Item(id).ToList
                If test.Contains(m.SystemId) Then res = True
            End If
            Return res
        End Function

        Private Function GetUnitName(kbl As KblMapper, p As Wire_protection) As String
            Dim myUnitName As String = ""
            If kbl.KBLUnitMapper.ContainsKey(p.Mass_information.Unit_component) Then
                Dim Unit As Unit = kbl.KBLUnitMapper.Item(p.Mass_information.Unit_component)
                If Unit IsNot Nothing AndAlso Unit.Unit_name IsNot Nothing Then
                    myUnitName = Unit.Unit_name
                Else
                    myUnitName = p.Mass_information.Unit_component
                End If
            End If
            Return myUnitName
        End Function


        Private Function GetUnitInformation(unitName As String) As Numerical_value
            Dim val As New Numerical_value
            Dim fac As Double = 0.0
            If unitName.Contains("kg") Then
                If unitName.Contains("m2") OrElse unitName.Contains("m²") Then
                    fac = 10 ^ -3
                    val.Unit_component = "mm2"
                ElseIf unitName.Contains("mm2") OrElse unitName.Contains("mm²") Then
                    fac = 10 ^ 3
                ElseIf unitName.Contains("mm") Then
                    fac = 10 ^ 3
                    val.Unit_component = "mm"

                ElseIf unitName.Contains("m") Then
                    fac = 1
                    val.Unit_component = "mm"
                End If

            ElseIf unitName.Contains("g") Then
                If unitName.Contains("m2") OrElse unitName.Contains("m²") Then
                    fac = 10 ^ -6
                    val.Unit_component = "mm2"
                ElseIf unitName.Contains("mm2") OrElse unitName.Contains("mm²") Then
                    fac = 1
                    val.Unit_component = "mm2"
                ElseIf unitName.Contains("mm") Then
                    fac = 1
                    val.Unit_component = "mm"

                ElseIf unitName.Contains("m") Then
                    fac = 10 ^ -3
                    val.Unit_component = "mm"

                End If
            End If
            val.Value_component = fac

            Return val
        End Function


        Private Function getarea(kbl As KblMapper, s As Segment) As Double
            Dim sarea As Double = -1

            For Each parea As Protection_area In s.Protection_area
                Dim slength As Double = 0.0
                If s.Virtual_length.Value_component > 0 Then
                    slength = s.Virtual_length.Value_component
                Else
                    slength = s.Physical_length.Value_component
                End If


                Dim l As Double = (parea.End_location - parea.Start_location) * slength
                'Info.Inf.Add("length: " + Math.Round(l, 3).ToString)
                calcLength += l
                Dim infos As List(Of Cross_section_area) = s.Cross_section_area_information.ToList
                If (infos IsNot Nothing) Then
                    If (infos.Count > 0) Then
                        Dim area As Double = 0
                        Dim unit As String = String.Empty

                        For Each cs As Cross_section_area In infos
                            'HINT use max
                            If cs.Area.Value_component > area Then
                                area = cs.Area.Value_component
                                unit = cs.Area.Unit_component
                                If kbl.KBLUnitMapper.ContainsKey(unit) Then

                                    Dim myUnit As Unit = kbl.KBLUnitMapper(unit)
                                    unit = myUnit.Unit_name
                                    If unit.Contains("mm2") OrElse unit.Contains("mm²") Then
                                        area = area
                                    ElseIf unit.Contains("m2") OrElse unit.Contains("m²") Then
                                        area = area * 10 ^ 6
                                    ElseIf unit.Contains("cm2") OrElse unit.Contains("cm²") Then
                                        area = area * 100
                                    Else
                                        Dim err As String = "no valid unit information found!"
                                        Info.Err.Add(err)
                                    End If

                                End If
                            End If
                        Next

                        Dim diameter As Double = (Math.Sqrt(area / Math.PI)) * 2
                        sarea += diameter * l * Math.PI
                    End If
                End If

            Next

            Return sarea

        End Function

    End Class
    Public Class CalculationInfo


        Public Err As New List(Of String)
        Public Inf As New List(Of String)
        Public Sub New()

        End Sub

        Public Function GetErrors() As String
            Dim res As String = ""

            If Err.Count > 0 Then
                res = BOMFormStrings.Error_ + ":"
                res += Environment.NewLine
                res += Err(0)
            End If



            Return res

        End Function

        Public Function GetInfos() As String
            Dim res As String = ""

            If Inf.Count > 0 Then
                res = "Info:"
                res += Environment.NewLine
                res += Inf(0)
            End If


            Return res

        End Function


    End Class


End Class