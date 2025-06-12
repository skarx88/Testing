Imports System.Data
Imports System.Globalization
Imports System.Text.RegularExpressions
Imports Infragistics.Win
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.HarnessAnalyzer.Settings.WeightSettings
Imports Zuken.E3.HarnessAnalyzer.WeightCalculator
Imports Zuken.E3.HarnessAnalyzer.WireResistanceCalculator
Imports Zuken.E3.HarnessAnalyzer.WireResistanceCalculatorForm
Imports Zuken.E3.Lib.Converter.Unit

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class VoltageDropUserControl
    Friend Event UpdateResult(sender As VoltageDropUserControl, e As EventArgs)

    Private _length As NumericValue
    Private _dt As DataTable = Nothing
    Private _index As Integer
    Private _kblMapper As KblMapper
    Private _kblMappers As IEnumerable(Of KblMapper)
    Private _hasError As Boolean
    Private _generalSettings As New GeneralSettings
    Private _defaultWireLengthType As String
    Private _IsFirstSetUp As Boolean
    Private _getMaterialSpecs As New List(Of WeightSettings.MaterialSpec)

    Private _dicOfSystemIdAndGeneralWire As New Dictionary(Of String, General_wire)
    Private _dicOfSystemIdAndCore As New Dictionary(Of String, Core)

    Private _dicOfWireTypeAndGeneralWire As New Dictionary(Of String, General_wire)
    Private _dicOfCoreTypeAndCore As New Dictionary(Of String, Core)

    Private _dicOfUniqueIdOfSysIdOfWireOrCore As New Dictionary(Of String, String)

    Private _dicOfSystemIdOfWireAndTypeOfWire As New Dictionary(Of String, String)
    Private _dicOfSystemIdOfCoreAndTypeOfWire As New Dictionary(Of String, String)

    Private _dicofWireOrCoreOccurrence As New Dictionary(Of String, Object)
    Private _sequentialWireAdjWithVertices As New Dictionary(Of String, Dictionary(Of String, CavityVertex))

    Private _total As Nullable(Of Double) = Nothing
    Private _conductor As Nullable(Of Double) = Nothing
    Private _csaSqMm As Nullable(Of Double) = Nothing

    Private Shared WithEvents _headingLayoutPanel As New TableLayoutPanel
    Private Shared _voltageDropControls As New List(Of VoltageDropUserControl)

    Private Const DEFAULTR1 As Double = 0.03
    Private Const DEFAULTR2 As Double = 0.03
    Private Const CELLHEIGHT As Integer = 41

    Public Sub New(width As Integer, table As DataTable, defaultWireLengthType As String, kblMappers As IEnumerable(Of KblMapper), coreOrWireIds() As String, locationY As Integer, index As Integer, sequentialWireAdjWithVertices As Dictionary(Of String, Dictionary(Of String, CavityVertex)))
        _kblMappers = kblMappers
        _index = index
        _defaultWireLengthType = defaultWireLengthType

        For Each wireAdj As KeyValuePair(Of String, Dictionary(Of String, CavityVertex)) In sequentialWireAdjWithVertices
            For Each vert As KeyValuePair(Of String, CavityVertex) In wireAdj.Value
                Dim wireAdjKbId As String = HarnessConnectivity.GetKblIdFromUniqueId(wireAdj.Key)
                Dim vertKblId As String = HarnessConnectivity.GetKblIdFromUniqueId(vert.Key)
                If Not _sequentialWireAdjWithVertices.ContainsKey(wireAdjKbId) Then
                    Dim vertDic As New Dictionary(Of String, CavityVertex)
                    If Not vertDic.ContainsKey(vert.Key) Then
                        vertDic.Add(vertKblId, vert.Value)
                    End If
                    _sequentialWireAdjWithVertices.Add(wireAdjKbId, vertDic)
                Else
                    _sequentialWireAdjWithVertices(wireAdjKbId).Add(vertKblId, vert.Value)
                End If
            Next
        Next

        InitializeComponent()
        Me.Width = width
        Me.ContentTableLayoutPanel.Width = width
        Me.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        _voltageDropControls.Add(Me)

        Init(table, kblMappers, coreOrWireIds, locationY)
    End Sub
    Private ReadOnly Property MaterialDen As Double
        Get
            Return If(MaterialDensityUNE.Value IsNot Nothing AndAlso TypeOf (MaterialDensityUNE.Value) Is Double, Math.Round(CDbl(MaterialDensityUNE.Value), 4), 0)
        End Get
    End Property
    Private ReadOnly Property ResistivityCustom As Double
        Get
            Return If(ResistivityCustomUNE.Value IsNot Nothing AndAlso TypeOf (ResistivityCustomUNE.Value) Is Double, Math.Round(CDbl(ResistivityCustomUNE.Value), 2), 0)
        End Get
    End Property
    Private ReadOnly Property ConductorWeight As Double
        Get
            Return If(ConductorWeightUNE.Value IsNot Nothing AndAlso TypeOf (ConductorWeightUNE.Value) Is Double, Math.Round(CDbl(ConductorWeightUNE.Value), 1), 0)
        End Get
    End Property
    Private ReadOnly Property TotalWeight As Double
        Get
            Return If(TotalWeightUNE.Value IsNot Nothing AndAlso TypeOf (TotalWeightUNE.Value) Is Double, Math.Round(CDbl(TotalWeightUNE.Value), 1), 0)
        End Get
    End Property
    Private ReadOnly Property Temperature As Integer
        Get
            Return If(TemperatureUNE.Value IsNot Nothing AndAlso TypeOf (TemperatureUNE.Value) Is Integer, CInt(TemperatureUNE.Value), 0)
        End Get
    End Property
    Private ReadOnly Property StartResistance As Double
        Get
            Return If(StartResitanceUNE.Value IsNot Nothing AndAlso TypeOf (StartResitanceUNE.Value) Is Double, Math.Round(CDbl(StartResitanceUNE.Value), 2), 0)
        End Get
    End Property
    Private ReadOnly Property EndResistance As Double
        Get
            Return If(EndResistanceUNE.Value IsNot Nothing AndAlso TypeOf (EndResistanceUNE.Value) Is Double, Math.Round(CDbl(EndResistanceUNE.Value), 2), 0)
        End Get
    End Property
    Private ReadOnly Property Length As Double
        Get
            Return If(LengthUNE.Value IsNot Nothing AndAlso TypeOf (LengthUNE.Value) Is Double, Math.Round(CDbl(LengthUNE.Value), 1), 0)
        End Get
    End Property
    Private ReadOnly Property CSA As Double
        Get
            Return If(CsaUNE.Value IsNot Nothing AndAlso TypeOf (CsaUNE.Value) Is Double, Math.Round(CDbl(CsaUNE.Value), 2), 0)
        End Get
    End Property

    Private ReadOnly Property GeneralSettings As GeneralSettings
        Get
            Return _generalSettings
        End Get
    End Property

    Public ReadOnly Property Resistance As Double
    Public ReadOnly Property HasError() As Boolean
        Get
            Return _hasError
        End Get
    End Property
    Public ReadOnly Property VoltageDropControls As List(Of VoltageDropUserControl)
        Get
            Return _voltageDropControls
        End Get
    End Property
    Private Sub CalculateWeightBySpec(specificWeight As Double, length As NumericValue)
        _csaSqMm = CSA
        If _csaSqMm.HasValue Then
            Dim CSASqcm As Nullable(Of Double) = UnitConverter.Convert(_csaSqMm.Value, ShortUnit.SqMm, ShortUnit.SqCm, True)
            If CSASqcm IsNot Nothing AndAlso length IsNot Nothing Then
                Me._conductor = specificWeight * CSASqcm * length.ToCentimetre 'g/cm³ * cm² * cm = g
                With My.Application.MainForm.WeightSettings.GenericInsulationWeightParameters
                    Dim insulationWeight As Double = InsulationMeterWeightFormula.Execute(.GIW_Offset, .GIW_Slope, .GIW_Square, _csaSqMm.Value) ' = (g/m*mm⁴) * mm⁴ + (g/m*mm²) * mm² + g/m = g/m
                    Me._total = Me._conductor + (insulationWeight * length.ToMetre) ' g + (g/m * m) = g
                End With
                _length = length
            End If
        End If
    End Sub
    Private Sub CalculateGenericWeight(wireOcc As Wire_occurrence, Optional useLength As NumericValue = Nothing)
        If useLength Is Nothing Then
            Dim wireLength As Numerical_value = wireOcc.GetLength
            If wireLength IsNot Nothing Then useLength = wireLength.Extract(_kblMapper)
        End If

        CalculateWeightAsPerSelectedMaterial(useLength)
    End Sub

    Private Sub CalculateGenericWeight(coreOcc As Core_occurrence, Optional useLength As NumericValue = Nothing)
        If useLength Is Nothing Then
            Dim coreLength As Numerical_value = coreOcc.GetLength
            If coreLength IsNot Nothing Then useLength = coreLength.Extract(_kblMapper)
        End If

        CalculateWeightAsPerSelectedMaterial(useLength)
    End Sub

    Private Sub CalculateWeightAsPerSelectedMaterial(Optional useLength As NumericValue = Nothing)
        If ConductorMaterialUCE.SelectedItem IsNot Nothing Then
            If ConductorMaterialUCE.SelectedItem.DisplayText = WireResistanceCalculatorFormStrings.Alu_Text Then
                CalculateWeightBySpec(HarnessAnalyzer.Shared.SPECIFIC_ALUMINIUM_WEIGHT, useLength)
            ElseIf ConductorMaterialUCE.SelectedItem.DisplayText = WireResistanceCalculatorFormStrings.Copper_Text Then
                CalculateWeightBySpec(HarnessAnalyzer.Shared.SPECIFIC_COPPER_WEIGHT, useLength)
            ElseIf ConductorMaterialUCE.SelectedItem.DisplayText = KblObjectType.Custom.ToLocalizedString AndAlso Not IsDBNull(MaterialDensityUNE.Value) Then
                CalculateWeightBySpec(CDbl(MaterialDensityUNE.Value), useLength)
            Else
                If TypeOf (Me.ConductorMaterialUCE.SelectedItem.DataValue) Is WeightSettings.MaterialSpec Then
                    CalculateWeightBySpec(CType(Me.ConductorMaterialUCE.SelectedItem.DataValue, WeightSettings.MaterialSpec).SpecificWeight, useLength)
                End If
            End If
        End If
    End Sub

    Private Sub UpdateResistance(resistivity As Double, Optional temperature As Temperature = Nothing)
        UpdateResistanceCore(Length, CSA, resistivity, temperature)
    End Sub

    Private Sub UpdateResistance(material As MaterialSpec, temperatureValue As Double)
        UpdateResistance(material.Resistivity, New Temperature(temperatureValue, material.TemperatureCoefficient))
    End Sub

    Private Sub UpdateResistanceCore(wireLength As Double, csa As Double, resistivity As Double, Optional temperature As Temperature = Nothing)
        Me._Resistance = 0
        If wireLength <> 0 AndAlso csa <> 0 Then
            Me._Resistance = (resistivity * 0.00000001) * (wireLength / 1000) / (csa * 0.000001)
            If (temperature IsNot Nothing) Then
                Me._Resistance = Me.Resistance * (1 + temperature.Coefficient * 0.001 * ((CInt(temperature.Value) - 20)))
            End If
        End If
    End Sub

    Private Sub Init(table As DataTable, kblMappers As IEnumerable(Of KblMapper), coreOrWireIds() As String, locationY As Integer)
        _generalSettings.ResetToDefaults()

        Me.Size = ContentTableLayoutPanel.Size
        Me.ContentTableLayoutPanel.Dock = DockStyle.Fill
        Me.Location = New Point(0, locationY + (CELLHEIGHT * _index))
        Me.StartResitanceUNE.Value = DEFAULTR1
        Me.EndResistanceUNE.Value = DEFAULTR2

        Me.EndResistanceUNE.Visible = If(_index = coreOrWireIds.Length, True, False)

        Me.TypeOfWireUCE.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.Office2013
        Me.TypeOfWireUCE.HasMRUList = False
        Me.TypeOfWireUCE.AlwaysInEditMode = False
        Me.TypeOfWireUCE.DropDownStyle = DropDownStyle.DropDownList

        Me.ConductorMaterialUCE.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.Office2013
        Me.ConductorMaterialUCE.HasMRUList = False
        Me.ConductorMaterialUCE.AlwaysInEditMode = False
        Me.ConductorMaterialUCE.DropDownStyle = DropDownStyle.DropDownList

        Dim objects As New List(Of Object)

        Dim coreOrWireId As String = coreOrWireIds.GetValue(_index - 1).ToString
        Dim harnessId As String = HarnessConnectivity.GetHarnessFromUniqueId(coreOrWireId)
        Dim resolvedCoreOrWireId As String = HarnessConnectivity.GetKblIdFromUniqueId(coreOrWireId)
        Dim harnessMapper As KblMapper = kblMappers.Where(Function(m) m.HarnessPartNumber = harnessId).FirstOrDefault
        Dim wireNum As String = String.Empty

        _kblMapper = harnessMapper

        If (harnessMapper IsNot Nothing) Then
            Dim coreOccurrence As Core_occurrence = TryCast(harnessMapper.KBLOccurrenceMapper(resolvedCoreOrWireId), Core_occurrence)
            Dim wireOccurrence As Wire_occurrence = TryCast(harnessMapper.KBLOccurrenceMapper(resolvedCoreOrWireId), Wire_occurrence)
            Dim special_wireOcc As Special_wire_occurrence = TryCast(harnessMapper.KBLOccurrenceMapper(resolvedCoreOrWireId), Special_wire_occurrence)

            If coreOccurrence IsNot Nothing Then
                objects.Add(WireResistanceCalculator.OverrideObject.OverrideWithCable(coreOccurrence, harnessMapper))

                If Not _dicofWireOrCoreOccurrence.ContainsKey(coreOccurrence.SystemId) Then
                    _dicofWireOrCoreOccurrence.Add(coreOccurrence.SystemId, coreOccurrence)
                End If
                wireNum = CType(_dicofWireOrCoreOccurrence.Values(0), Core_occurrence).Wire_number
                SetFirstAndLastContactPointDescription(coreOccurrence.SystemId)

            ElseIf wireOccurrence IsNot Nothing Then
                objects.Add(wireOccurrence)
                If Not _dicofWireOrCoreOccurrence.ContainsKey(wireOccurrence.SystemId) Then
                    _dicofWireOrCoreOccurrence.Add(wireOccurrence.SystemId, wireOccurrence)
                End If

                wireNum = CType(_dicofWireOrCoreOccurrence.Values(0), Wire_occurrence).Wire_number
                SetFirstAndLastContactPointDescription(wireOccurrence.SystemId)

            ElseIf special_wireOcc IsNot Nothing Then 'Todo: Check if we need any other occurrence type 
                'objects.Add(special_wireOcc)
                'If Not _dicofWireOrCoreOccurrence.ContainsKey(special_wireOcc.SystemId) Then
                '    _dicofWireOrCoreOccurrence.Add(special_wireOcc.SystemId, special_wireOcc)
                'End If
                'wireNum = CType(_dicofWireOrCoreOccurrence.Values(0), Special_wire_occurrence).Special_wire_id
                'SetFirstAndLastContactPointDescription(special_wireOcc.SystemId)
            End If
        End If

        If _kblMapper IsNot Nothing Then

            If _dicofWireOrCoreOccurrence.Count > 0 Then
                WireOrCoreNumUTE.ReadOnly = True
                WireOrCoreNumUTE.Value = wireNum
            End If

            InitializeControls(coreOrWireIds.GetValue(_index - 1))

            If table IsNot Nothing Then
                Dim row As DataRow = table.NewRow
                row.Item(My.Resources.VoltageDropUserControlStrings.StartResistance) = StartResistance
                row.Item(My.Resources.VoltageDropUserControlStrings.WireOrCorenumber) = WireOrCoreNumUTE.Value
                row.Item(My.Resources.VoltageDropUserControlStrings.StartCPTerminalDescription) = FirstContactPointUTE.Value
                row.Item(My.Resources.VoltageDropUserControlStrings.TypeOfWire) = TypeOfWireUCE.SelectedItem
                row.Item(My.Resources.VoltageDropUserControlStrings.Material) = ConductorMaterialUCE.SelectedItem
                row.Item(My.Resources.VoltageDropUserControlStrings.MaterialDensity) = MaterialDen
                row.Item(My.Resources.VoltageDropUserControlStrings.ResistivityCustom) = ResistivityCustom
                row.Item(My.Resources.VoltageDropUserControlStrings.WireResistance) = Math.Round(Me.Resistance, 4)
                row.Item(My.Resources.VoltageDropUserControlStrings.CSA) = CSA
                row.Item(My.Resources.VoltageDropUserControlStrings.Temperature) = Temperature
                row.Item(My.Resources.VoltageDropUserControlStrings.Length) = Length
                row.Item(My.Resources.VoltageDropUserControlStrings.ConductorWeight) = ConductorWeight
                row.Item(My.Resources.VoltageDropUserControlStrings.TotalWeight) = TotalWeight
                row.Item(My.Resources.VoltageDropUserControlStrings.EndCPTerminalDescription) = LastContactPointUTE.Value

                If coreOrWireIds.Length > 0 Then
                    row.Item(My.Resources.VoltageDropUserControlStrings.EndResistance) = If(EndResistanceUNE.Visible, EndResistance, 0.0)
                    EndResistanceUNE.Value = If(EndResistanceUNE.Visible, EndResistance, 0.0)
                End If

                table.Rows.Add(row)
            End If

            _dt = table

            EndResistanceUNE.Visible = If(coreOrWireIds.Length = 1 Or coreOrWireIds.Length = _index, True, False)
        End If
    End Sub

    Private Sub SetFirstAndLastContactPointDescription(wireId As String)
        If (_kblMapper.KBLWireNetMapper.ContainsKey(wireId)) Then
            If (_kblMapper.KBLWireNetMapper(wireId).Extremities.Length = 2) Then
                If _sequentialWireAdjWithVertices.ContainsKey(wireId) Then
                    If _kblMapper.KBLCavityContactPointMapper.ContainsKey(_sequentialWireAdjWithVertices(wireId).FirstOrDefault.Key) Then
                        Me.FirstContactPointUTE.Value = GetDescription(wireId, _kblMapper.KBLCavityContactPointMapper(_sequentialWireAdjWithVertices(wireId).FirstOrDefault.Key).FirstOrDefault.SystemId)
                    End If

                    If _kblMapper.KBLCavityContactPointMapper.ContainsKey(_sequentialWireAdjWithVertices(wireId).LastOrDefault.Key) Then
                        Me.LastContactPointUTE.Value = GetDescription(wireId, _kblMapper.KBLCavityContactPointMapper(_sequentialWireAdjWithVertices(wireId).LastOrDefault.Key).FirstOrDefault.SystemId)
                    End If
                End If
            End If

        ElseIf (_kblMapper.KBLOccurrenceMapper.ContainsKey(wireId)) AndAlso (TypeOf _kblMapper.KBLOccurrenceMapper(wireId) Is Special_wire_occurrence) Then
            For Each core As Core_occurrence In DirectCast(_kblMapper.KBLOccurrenceMapper(wireId), Special_wire_occurrence).Core_occurrence

                If (_kblMapper.KBLWireNetMapper.ContainsKey(core.SystemId)) AndAlso (_kblMapper.KBLWireNetMapper(core.SystemId).Extremities.Length = 2) Then
                    If _sequentialWireAdjWithVertices.ContainsKey(wireId) Then
                        If _kblMapper.KBLCavityContactPointMapper.ContainsKey(_sequentialWireAdjWithVertices(wireId).FirstOrDefault.Key) Then
                            Me.FirstContactPointUTE.Value = GetDescription(wireId, _kblMapper.KBLCavityContactPointMapper(_sequentialWireAdjWithVertices(wireId).FirstOrDefault.Key).FirstOrDefault.SystemId)
                        End If

                        If _kblMapper.KBLCavityContactPointMapper.ContainsKey(_sequentialWireAdjWithVertices(wireId).LastOrDefault.Key) Then
                            Me.LastContactPointUTE.Value = GetDescription(wireId, _kblMapper.KBLCavityContactPointMapper(_sequentialWireAdjWithVertices(wireId).LastOrDefault.Key).FirstOrDefault.SystemId)
                        End If
                    End If
                    Exit For
                End If
            Next
        End If

    End Sub
    Private Function GetDescription(wireId As String, contactPt As String) As String
        Dim conn_occu As Connector_occurrence = _kblMapper.GetConnectorOccurrences.Where(Function(conn) conn.SystemId = _kblMapper.KBLContactPointConnectorMapper(contactPt)).FirstOrDefault
        Dim contPt As Contact_point = If(conn_occu IsNot Nothing, conn_occu.Contact_points.Where(Function(x) x.SystemId = contactPt).FirstOrDefault, Nothing)
        Dim index As Integer = 0
        Dim description As String = String.Empty

        If contPt IsNot Nothing AndAlso contPt.Associated_parts IsNot Nothing Then
            For Each associatedPart As String In contPt.Associated_parts.SplitSpace
                If Not String.IsNullOrEmpty(associatedPart) Then

                    Dim termOcc As Terminal_occurrence = _kblMapper.GetHarness.GetTerminalOccurrence(associatedPart)
                    Dim terminal As General_terminal = If(termOcc IsNot Nothing, _kblMapper.GetGeneralTerminals.Where(Function(x) x.SystemId = termOcc.Part).FirstOrDefault, Nothing)

                    If terminal IsNot Nothing Then
                        description += If(index = 0, terminal.Description.Trim, String.Format(";{0}", terminal.Description.Trim))
                        index += 1
                    End If


                End If
            Next
        End If
        Return If(Not String.IsNullOrEmpty(description.Trim), description, "-")
    End Function

    Private Sub InitializeControls(wireOrCore As Object)
        If wireOrCore Is Nothing Then
            Throw New ArgumentException("No wire or core objects defined in parameter!", NameOf(wireOrCore))
        End If

        Dim listOfWireTypes As New List(Of String)

        Try
            _IsFirstSetUp = True

            Me.TemperatureUNE.Value = WireResistanceCalculator.Temperature.Default.Value
            Me.TypeOfWireUCE.Items.Add(KblObjectType.Custom, KblObjectType.Custom.ToLocalizedString)

            AddTypeOfWireToComboItems()

            If _dicofWireOrCoreOccurrence.Count > 0 Then
                Dim wirelen As Wire_length = Nothing
                Dim csaVal As Double = 0
                Dim valListItm As ValueListItem = Nothing
                Dim wt As New WeightCalculator(_kblMapper)

                GetValueItemWireLengthCsaAndWeightCalc(valListItm, wirelen, csaVal, wt)


                Me.CsaUNE.Value = csaVal

                If valListItm IsNot Nothing Then
                    Me.TypeOfWireUCE.Value = valListItm
                    Me.CsaUNE.ReadOnly = True
                Else
                    Me.TypeOfWireUCE.Value = Me.TypeOfWireUCE.Items(0)
                    Me.CsaUNE.ReadOnly = False
                End If

                Me.ConductorWeightUNE.Value = If(wt.Calculated.Conductor IsNot Nothing, wt.Calculated.Conductor.Value, 0)
                Me.TotalWeightUNE.Value = If(wt.Calculated.Total IsNot Nothing, wt.Calculated.Total.Value, 0)
                Me.LengthUNE.ReadOnly = True
                Me.LengthUNE.Value = If(wirelen IsNot Nothing AndAlso wirelen.Length_value IsNot Nothing, wirelen.Length_value.Value_component, 0)
                If LengthUNE.Value Is Nothing OrElse Double.IsNaN(CDbl(Length)) OrElse Double.IsInfinity(Length) OrElse Length = 0 Then
                    Me.LengthUNE.Appearance.ForeColor = Color.Red
                Else
                    Me.LengthUNE.Appearance.ForeColor = Color.Black
                End If
            End If

        Catch ex As Exception
            _hasError = True
            MessageBoxEx.ShowError(Me, String.Format(VoltageDropUserControlStrings.ErrorInitCalculator_Msg, vbCrLf, ex.Message))
        Finally
            _IsFirstSetUp = False
        End Try

        _hasError = Not InitializeConductorMaterials()
    End Sub

    Private Sub AddTypeOfWireToComboItems()
        For Each generalWire As General_wire In _kblMapper.GetGeneralWires
            Dim wireTyp As String = String.Empty
            _dicOfSystemIdAndGeneralWire.TryAdd(generalWire.SystemId, generalWire)
            If generalWire.Core.Length = 0 Then
                If Not _dicOfWireTypeAndGeneralWire.ContainsKey(wireTyp) Then

                    If generalWire.Wire_type IsNot Nothing AndAlso Not String.IsNullOrEmpty(generalWire.Wire_type) Then
                        Dim valToSearch As String = If(generalWire.Cross_section_area IsNot Nothing, Math.Round(generalWire.Cross_section_area.Value_component, 2).ToString(CultureInfo.InvariantCulture), Nothing)
                        Dim searchRegex As Match = If(valToSearch IsNot Nothing, Regex.Match(generalWire.Wire_type.ToString(CultureInfo.InvariantCulture), valToSearch, RegexOptions.IgnoreCase), Nothing)

                        If generalWire.Cross_section_area IsNot Nothing AndAlso searchRegex IsNot Nothing AndAlso Not searchRegex.Success Then
                            wireTyp = GenerateWireOrCoreType(generalWire.Wire_type, generalWire.Cross_section_area.Value_component.ToString(CultureInfo.InvariantCulture))
                        Else
                            wireTyp = generalWire.Wire_type.ToString(CultureInfo.InvariantCulture)
                        End If

                    Else
                        If Not String.IsNullOrEmpty(generalWire.Abbreviation) AndAlso generalWire.Cross_section_area IsNot Nothing Then
                            wireTyp = GenerateWireOrCoreType(generalWire.Abbreviation, generalWire.Cross_section_area.Value_component.ToString(CultureInfo.InvariantCulture))
                        End If
                    End If


                    If Not String.IsNullOrEmpty(wireTyp) AndAlso Not _dicOfWireTypeAndGeneralWire.ContainsKey(wireTyp) AndAlso Not _dicOfCoreTypeAndCore.ContainsKey(wireTyp) AndAlso Not Me.TypeOfWireUCE.Items.Contains(wireTyp) Then
                        Me.TypeOfWireUCE.Items.Add(wireTyp, wireTyp)
                        _dicOfWireTypeAndGeneralWire.Add(wireTyp, generalWire)
                    End If

                    If Not String.IsNullOrEmpty(wireTyp) AndAlso Not _dicOfSystemIdOfWireAndTypeOfWire.ContainsKey(generalWire.SystemId) Then
                        _dicOfSystemIdOfWireAndTypeOfWire.Add(generalWire.SystemId, wireTyp)
                    End If

                    Dim uniqueId As String = GenerateUniqueId(generalWire.SystemId, wireTyp)
                    If Not String.IsNullOrEmpty(wireTyp) AndAlso Not _dicOfUniqueIdOfSysIdOfWireOrCore.ContainsKey(uniqueId) Then
                        _dicOfUniqueIdOfSysIdOfWireOrCore.Add(uniqueId, generalWire.SystemId)
                    End If

                End If
            Else
                For Each core As Core In generalWire.Core
                    _dicOfSystemIdAndCore.TryAdd(core.SystemId, core)
                    Dim coreType As String = String.Empty

                    If generalWire.Wire_type IsNot Nothing AndAlso Not String.IsNullOrEmpty(generalWire.Wire_type) Then

                        Dim valToSearch As String = If(core.Cross_section_area IsNot Nothing, Math.Round(core.Cross_section_area.Value_component, 2).ToString(CultureInfo.InvariantCulture), Nothing)
                        Dim searchRegex As Match = If(valToSearch IsNot Nothing, Regex.Match(generalWire.Wire_type, valToSearch, RegexOptions.IgnoreCase), Nothing)

                        If core.Cross_section_area IsNot Nothing AndAlso searchRegex IsNot Nothing AndAlso Not searchRegex.Success Then 'Hint: if core have different csa then we create different wiretype for each csa
                            coreType = GenerateWireOrCoreType(generalWire.Wire_type, core.Cross_section_area.Value_component.ToString(CultureInfo.InvariantCulture))
                        Else
                            coreType = generalWire.Wire_type.ToString(CultureInfo.InvariantCulture)
                        End If

                    Else
                        If Not String.IsNullOrEmpty(generalWire.Abbreviation) AndAlso core.Cross_section_area IsNot Nothing Then
                            coreType = GenerateWireOrCoreType(generalWire.Abbreviation, core.Cross_section_area.Value_component.ToString(CultureInfo.InvariantCulture))
                        End If
                    End If

                    If Not String.IsNullOrEmpty(coreType) AndAlso Not _dicOfCoreTypeAndCore.ContainsKey(coreType) AndAlso Not _dicOfWireTypeAndGeneralWire.ContainsKey(coreType) AndAlso Not Me.TypeOfWireUCE.Items.Contains(coreType) Then
                        Me.TypeOfWireUCE.Items.Add(coreType, coreType)

                        _dicOfCoreTypeAndCore.Add(coreType, core)
                    End If

                    If Not String.IsNullOrEmpty(coreType) AndAlso Not _dicOfSystemIdOfCoreAndTypeOfWire.ContainsKey(core.SystemId) Then
                        _dicOfSystemIdOfCoreAndTypeOfWire.Add(core.SystemId, coreType)
                    End If

                    Dim uniquecoreId As String = GenerateUniqueId(core.SystemId, coreType)
                    If Not String.IsNullOrEmpty(coreType) AndAlso Not _dicOfUniqueIdOfSysIdOfWireOrCore.ContainsKey(uniquecoreId) Then
                        _dicOfUniqueIdOfSysIdOfWireOrCore.Add(uniquecoreId, core.SystemId)
                    End If
                Next
            End If
        Next
    End Sub
    Private Sub GetValueItemWireLengthCsaAndWeightCalc(ByRef valListItm As ValueListItem, ByRef wirelen As Wire_length, ByRef csaVal As Double, ByRef wt As WeightCalculator)
        Dim systemIdOfGeneralWireorCore As String
        Dim lengthVal As Double = 0
        Dim hasLengthInfo As Boolean = False

        If TypeOf (_dicofWireOrCoreOccurrence.Values(0)) Is Core_occurrence Then

            systemIdOfGeneralWireorCore = CType(_dicofWireOrCoreOccurrence.Values(0), Core_occurrence).Part

            If _dicOfSystemIdOfCoreAndTypeOfWire.ContainsKey(systemIdOfGeneralWireorCore) Then
                Dim wireTyp As String = _dicOfSystemIdOfCoreAndTypeOfWire(systemIdOfGeneralWireorCore)
                If Not String.IsNullOrEmpty(wireTyp) Then
                    valListItm = New ValueListItem(wireTyp, wireTyp)
                End If
            End If

            wt.Calculate(CType(_dicofWireOrCoreOccurrence.Values(0), Core_occurrence))
            For Each lengthinfo As Wire_length In CType(_dicofWireOrCoreOccurrence.Values(0), Core_occurrence).Length_information
                If lengthinfo.Length_type.ToLower = _defaultWireLengthType.ToLower Then
                    hasLengthInfo = True
                    Exit For
                End If
            Next

            wirelen = If(hasLengthInfo, CType(_dicofWireOrCoreOccurrence.Values(0), Core_occurrence).Length_information.Where(Function(lengthinfo) lengthinfo.Length_type.ToLower = _defaultWireLengthType.ToLower).FirstOrDefault, Nothing)
            csaVal = If(_dicOfSystemIdAndCore(systemIdOfGeneralWireorCore) IsNot Nothing, CType(_dicOfSystemIdAndCore(systemIdOfGeneralWireorCore), Core).Cross_section_area.Value_component, 0.0)

        ElseIf TypeOf (_dicofWireOrCoreOccurrence.Values(0)) Is Wire_occurrence Then

            systemIdOfGeneralWireorCore = CType(_dicofWireOrCoreOccurrence.Values(0), Wire_occurrence).Part

            If _dicOfSystemIdOfWireAndTypeOfWire.ContainsKey(systemIdOfGeneralWireorCore) Then
                Dim wireTyp As String = _dicOfSystemIdOfWireAndTypeOfWire(systemIdOfGeneralWireorCore)

                If Not String.IsNullOrEmpty(wireTyp) Then
                    valListItm = New ValueListItem(wireTyp, wireTyp)
                End If
            End If

            wt.Calculate(CType(_dicofWireOrCoreOccurrence.Values(0), Wire_occurrence))
            For Each lengthinfo As Wire_length In CType(_dicofWireOrCoreOccurrence.Values(0), Wire_occurrence).Length_information
                If lengthinfo.Length_type.ToLower = _defaultWireLengthType.ToLower Then
                    hasLengthInfo = True
                    Exit For
                End If
            Next

            wirelen = If(hasLengthInfo, CType(_dicofWireOrCoreOccurrence.Values(0), Wire_occurrence).Length_information.Where(Function(lengthinfo) lengthinfo.Length_type.ToLower = _defaultWireLengthType.ToLower).FirstOrDefault, Nothing)
            csaVal = If(_dicOfSystemIdAndGeneralWire(systemIdOfGeneralWireorCore) IsNot Nothing, CType(_dicOfSystemIdAndGeneralWire(systemIdOfGeneralWireorCore), General_wire).Cross_section_area.Value_component, 0.0)
        End If
    End Sub

    Private Function InitializeConductorMaterials() As Boolean
        Dim dCount As Integer = 0

        Me.ConductorMaterialUCE.Items.Add(AluminiumTemperature.Default, WireResistanceCalculatorFormStrings.Alu_Text)
        Me.ConductorMaterialUCE.Items.Add(CopperTemperature.Default, WireResistanceCalculatorFormStrings.Copper_Text)
        Me.ConductorMaterialUCE.Items.Add(New WireResistanceCalculator.Temperature(CDbl(Me.TemperatureUNE.Value), 0), KblObjectType.Custom.ToLocalizedString)

        Me.ConductorMaterialUCE.SelectedItem = Me.ConductorMaterialUCE.Items(1)
        Dim errors As System.Text.StringBuilder = SetMaterialSpecAndGetErrors()

        If Me.ConductorMaterialUCE.SelectedItem IsNot Nothing Then
            If Me.ConductorMaterialUCE.SelectedItem.DisplayText = WireResistanceCalculatorFormStrings.Copper_Text Then
                Me.MaterialDensityUNE.Value = HarnessAnalyzer.Shared.SPECIFIC_COPPER_WEIGHT
                Me.ResistivityCustomUNE.Value = GeneralSettings.ResistivityCopper
            ElseIf Me.ConductorMaterialUCE.SelectedItem.DisplayText = WireResistanceCalculatorFormStrings.Alu_Text Then
                Me.MaterialDensityUNE.Value = HarnessAnalyzer.Shared.SPECIFIC_ALUMINIUM_WEIGHT
                Me.ResistivityCustomUNE.Value = GeneralSettings.ResistivityAluminium
            ElseIf Me.ConductorMaterialUCE.SelectedItem.DisplayText = KblObjectType.Custom.ToLocalizedString Then
                Me.MaterialDensityUNE.Value = 0
                Me.ResistivityCustomUNE.Value = 0
            Else
                SetMaterialDensityAndResistivity(Me)
            End If
        End If

        If errors.Length > 0 Then
            MessageBoxEx.ShowError(Me, errors.ToString)
        End If

        Return errors.Length = 0
    End Function

    Private Sub SetMaterialDensityAndResistivity(cntrl As VoltageDropUserControl)
        If TypeOf (cntrl.ConductorMaterialUCE.Value) Is WeightSettings.MaterialSpec Then
            For Each spec As MaterialSpec In _getMaterialSpecs
                If cntrl.ConductorMaterialUCE.SelectedItem.DisplayText = spec.Description Then
                    cntrl.MaterialDensityUNE.Value = spec.SpecificWeight
                    cntrl.ResistivityCustomUNE.Value = spec.Resistivity
                    Exit For
                End If
            Next
        End If
    End Sub

    Private Function GenerateUniqueId(sysId As String, selectedTyp As String) As String
        Return String.Format("{0}-{1}", sysId, selectedTyp)
    End Function

    Private Function GenerateWireOrCoreType(sysId As String, csa As String) As String
        Return String.Format("{0}-{1}", sysId, csa)
    End Function

    Private Function SetMaterialSpecAndGetErrors() As System.Text.StringBuilder
        Dim errors As New System.Text.StringBuilder
        Dim dCount As Integer = 0

        Dim addedMatSpecs As New List(Of MaterialSpec)
        Dim OccurenceObject As Object = _dicofWireOrCoreOccurrence.Values(0)
        If OccurenceObject IsNot Nothing Then

            If OccurenceObject IsNot Nothing Then
                Dim specs As New List(Of WeightSettings.MaterialSpec)
                Try
                    specs = My.Application.MainForm.WeightSettings.MaterialSpecs.FindMaterialSpecsFor2(GetGeneralWire(OccurenceObject))
                Catch ex As Exception
                    errors.AppendLine(String.Format(ErrorStrings.WeightCalc_ErrorRetrievingMaterialSpec, [Lib].Schema.Kbl.Utils.GetUserId(OccurenceObject), ex.Message))
                End Try

                If specs.Count > 0 Then
                    For Each spec As WeightSettings.MaterialSpec In specs
                        If Not addedMatSpecs.Contains(spec) Then
                            addedMatSpecs.Add(spec)
                            Dim specDescr As String = spec.Description
                            If specDescr.Trim = WireResistanceCalculatorFormStrings.Alu_Text.Trim OrElse specDescr.Trim = WireResistanceCalculatorFormStrings.Copper_Text.Trim Then
                                dCount += 1
                                specDescr = String.Format("{0} ({1})", specDescr, dCount)
                            End If
                            Dim itm As Infragistics.Win.ValueListItem = Me.ConductorMaterialUCE.Items.Insert(0, spec, specDescr)
                            If itm IsNot Nothing Then
                                Me.ConductorMaterialUCE.SelectedItem = itm
                                If Not _getMaterialSpecs.Contains(spec) Then
                                    _getMaterialSpecs.Add(spec)
                                End If

                            End If

                        End If
                    Next
                End If
            End If

        End If
        Return errors
    End Function
    Private Function GetGeneralWire(listObject As Object) As General_wire
        Dim mapper As KblMapper = Nothing
        If TypeOf (listObject) Is Wire_occurrence Then
            mapper = _kblMappers.Where(Function(mp) mp.KBLWireList.Contains(DirectCast(listObject, Wire_occurrence))).FirstOrDefault
            Return DirectCast(mapper.KBLPartMapper(DirectCast(listObject, Wire_occurrence).Part), General_wire)
        ElseIf (TypeOf (listObject) Is Special_wire_occurrence) Then
            mapper = _kblMappers.Where(Function(mp) mp.KBLCableList.Contains(DirectCast(listObject, Special_wire_occurrence))).FirstOrDefault
            Return DirectCast(mapper.KBLPartMapper(DirectCast(listObject, Special_wire_occurrence).Part), General_wire)
        ElseIf (TypeOf (listObject) Is Core_occurrence) Then
            mapper = _kblMappers.Where(Function(mp) mp.KBLCoreList.Contains(DirectCast(listObject, Core_occurrence))).FirstOrDefault
            Dim unknownObj As Object = OverrideObject.OverrideWithCable(DirectCast(listObject, Core_occurrence), mapper).Object

            If TypeOf (unknownObj) Is Special_wire_occurrence Then
                If mapper.KBLCableList.Contains(DirectCast(unknownObj, Special_wire_occurrence)) Then
                    Return GetGeneralWire(unknownObj)
                End If
            End If
        End If
        Return Nothing
    End Function

    Private Sub ResitanceUNE_ValueChanged(sender As Object, e As EventArgs) Handles StartResitanceUNE.ValueChanged
        If _dt IsNot Nothing AndAlso _dt.Rows.Count > 0 AndAlso _dt.Rows(_index - 1) IsNot Nothing Then
            _dt.Rows(_index - 1)(My.Resources.VoltageDropUserControlStrings.StartResistance) = StartResistance
        End If

        RaiseEvent UpdateResult(Me, New EventArgs())
    End Sub
    Private Sub LastResistivityUNE_ValueChanged(sender As Object, e As EventArgs) Handles EndResistanceUNE.ValueChanged
        If _dt IsNot Nothing AndAlso _dt.Rows.Count > 0 Then
            _dt.Rows(_dt.Rows.Count - 1)(My.Resources.VoltageDropUserControlStrings.EndResistance) = EndResistance
        End If

        RaiseEvent UpdateResult(Me, New EventArgs())
    End Sub

    Private Sub TypeOfWireUCE_SelectionChanged(sender As Object, e As EventArgs) Handles TypeOfWireUCE.SelectionChanged
        If Not _IsFirstSetUp AndAlso TypeOfWireUCE IsNot Nothing Then

            Me.CsaUNE.ReadOnly = If(TypeOfWireUCE.SelectedItem.DisplayText <> KblObjectType.Custom.ToLocalizedString, True, False)

            Dim uniqueId As String
            Dim isCsaSet As Boolean = False

            If _dt IsNot Nothing AndAlso _dt.Rows.Count > 0 Then
                _dt.Rows(_index - 1)(My.Resources.VoltageDropUserControlStrings.TypeOfWire) = TypeOfWireUCE.SelectedItem.DisplayText
            End If

            If _dicOfCoreTypeAndCore.ContainsKey(TypeOfWireUCE.SelectedItem.DisplayText) AndAlso Not isCsaSet Then
                uniqueId = GenerateUniqueId(_dicOfCoreTypeAndCore(TypeOfWireUCE.SelectedItem.DisplayText).SystemId, TypeOfWireUCE.SelectedItem.DisplayText)

                If _dicOfUniqueIdOfSysIdOfWireOrCore.ContainsKey(uniqueId) Then
                    If _dicOfSystemIdAndCore.ContainsKey(_dicOfUniqueIdOfSysIdOfWireOrCore(uniqueId)) Then
                        Me.CsaUNE.Value = _dicOfSystemIdAndCore(_dicOfUniqueIdOfSysIdOfWireOrCore(uniqueId)).Cross_section_area.Value_component
                        isCsaSet = True
                    End If
                End If
            End If

            If _dicOfWireTypeAndGeneralWire.ContainsKey(TypeOfWireUCE.SelectedItem.DisplayText) AndAlso Not isCsaSet Then
                uniqueId = GenerateUniqueId(_dicOfWireTypeAndGeneralWire(TypeOfWireUCE.SelectedItem.DisplayText).SystemId, TypeOfWireUCE.SelectedItem.DisplayText)
                If _dicOfUniqueIdOfSysIdOfWireOrCore.ContainsKey(uniqueId) Then
                    If _dicOfSystemIdAndGeneralWire.ContainsKey(_dicOfUniqueIdOfSysIdOfWireOrCore(uniqueId)) Then
                        Me.CsaUNE.Value = _dicOfSystemIdAndGeneralWire(_dicOfUniqueIdOfSysIdOfWireOrCore(uniqueId)).Cross_section_area.Value_component
                    End If
                End If
            End If

            'ToDo : Check if material also changes with typeof of wire ?
            'Hint: Material spec changes for each wire or core occurrence, so changing material in that case is not possible. Because occurrence remains the same.
            'If ConductorMaterialUCE.Items.Contains(itm) AndAlso Me.ConductorMaterialUCE.SelectedItem.DisplayText <> itm.DisplayText Then
            '    Me.ConductorMaterialUCE.Value = itm
            '    UpdateResistance(CType(Me.ConductorMaterialUCE.SelectedItem.DataValue, WeightSettings.MaterialSpec), CDbl(TemperatureUNE.Value))
            '    RaiseEvent UpdateResult(Me, New EventArgs())

            '    If _dt IsNot Nothing AndAlso _dt.Rows.Count > 0 Then
            '        _dt.Rows(_index - 1)(My.Resources.VoltageDropUserControlStrings.Material) = itm.DisplayText
            '    End If
            'End If

            If _dt IsNot Nothing AndAlso _dt.Rows.Count > 0 Then
                UpdateResistanceCore(Length, CSA, ResistivityCustom)
                _dt.Rows(_index - 1)(My.Resources.VoltageDropUserControlStrings.CSA) = CSA
                _dt.Rows(_index - 1)(My.Resources.VoltageDropUserControlStrings.WireResistance) = Math.Round(Me.Resistance, 4)
                RaiseEvent UpdateResult(Me, New EventArgs())
            End If
        End If

    End Sub

    Private Sub CsaUNE_ValueChanged(sender As Object, e As EventArgs) Handles CsaUNE.ValueChanged
        If TypeOf (_dicofWireOrCoreOccurrence.Values(0)) Is Core_occurrence Then
            CalculateGenericWeight(CType(_dicofWireOrCoreOccurrence.Values(0), Core_occurrence))
        ElseIf TypeOf (_dicofWireOrCoreOccurrence.Values(0)) Is Wire_occurrence Then
            CalculateGenericWeight(CType(_dicofWireOrCoreOccurrence.Values(0), Wire_occurrence))
        End If

        Me.ConductorWeightUNE.Value = _conductor
        Me.TotalWeightUNE.Value = _total

        If _dt IsNot Nothing AndAlso _dt.Rows.Count > 0 Then
            UpdateResistanceCore(Length, CSA, ResistivityCustom)
            _dt.Rows(_index - 1)(My.Resources.VoltageDropUserControlStrings.CSA) = CSA
            _dt.Rows(_index - 1)(My.Resources.VoltageDropUserControlStrings.WireResistance) = Math.Round(Me.Resistance, 4)
            _dt.Rows(_index - 1)(My.Resources.VoltageDropUserControlStrings.ConductorWeight) = ConductorWeight
            _dt.Rows(_index - 1)(My.Resources.VoltageDropUserControlStrings.TotalWeight) = TotalWeight
        End If
        RaiseEvent UpdateResult(Me, New EventArgs())

    End Sub
    Private Sub ConductorWeightUNE_ValueChanged(sender As Object, e As EventArgs) Handles ConductorWeightUNE.ValueChanged

        If _dt IsNot Nothing AndAlso _dt.Rows.Count > 0 Then
            _dt.Rows(_index - 1)(My.Resources.VoltageDropUserControlStrings.ConductorWeight) = ConductorWeight
        End If
        RaiseEvent UpdateResult(Me, New EventArgs())
    End Sub

    Private Sub TotalWeightUNE_ValueChanged(sender As Object, e As EventArgs) Handles TotalWeightUNE.ValueChanged
        If _dt IsNot Nothing AndAlso _dt.Rows.Count > 0 Then
            _dt.Rows(_index - 1)(My.Resources.VoltageDropUserControlStrings.TotalWeight) = TotalWeight
        End If
        RaiseEvent UpdateResult(Me, New EventArgs())
    End Sub

    Private Sub TemperatureUNE_ValueChanged(sender As Object, e As EventArgs) Handles TemperatureUNE.ValueChanged
        If _dt IsNot Nothing AndAlso _dt.Rows.Count > 0 Then
            Dim tempCoeff As Double = 0

            If Me.ConductorMaterialUCE.SelectedItem.DisplayText = WireResistanceCalculatorFormStrings.Copper_Text Then
                tempCoeff = GeneralSettings.TemperatureCoefficientCopper
            ElseIf Me.ConductorMaterialUCE.SelectedItem.DisplayText = WireResistanceCalculatorFormStrings.Alu_Text Then
                tempCoeff = GeneralSettings.TemperatureCoefficientAluminium
            Else
                For Each spec As MaterialSpec In _getMaterialSpecs
                    If Me.ConductorMaterialUCE.SelectedItem.DisplayText = spec.Description Then
                        tempCoeff = spec.TemperatureCoefficient
                        Exit For
                    End If
                Next

            End If
            'ToDo: temperature coeffiecient for custom and material spec, currently its zero so resistance remains same. 

            Dim temp As New Temperature(Temperature, tempCoeff)
            UpdateResistance(ResistivityCustom, temp)
            _dt.Rows(_index - 1)(My.Resources.VoltageDropUserControlStrings.Temperature) = Temperature
            _dt.Rows(_index - 1)(My.Resources.VoltageDropUserControlStrings.WireResistance) = Math.Round(Me.Resistance, 4)
        End If

        RaiseEvent UpdateResult(Me, New EventArgs())
    End Sub

    Private Sub ConductorMaterialUCE_ValueChanged(sender As Object, e As EventArgs) Handles ConductorMaterialUCE.ValueChanged

        If Me.ConductorMaterialUCE.SelectedItem IsNot Nothing Then
            If TypeOf Me.ConductorMaterialUCE.SelectedItem.DataValue Is WeightSettings.MaterialSpec Then
                UpdateResistance(CType(Me.ConductorMaterialUCE.SelectedItem.DataValue, WeightSettings.MaterialSpec).Resistivity)

            ElseIf TypeOf Me.ConductorMaterialUCE.SelectedItem.DataValue Is IResistivityProvider Then
                'Todo:Do we need this ? 
                'UpdateResistance(CType(Me.ConductorMaterialUCE.SelectedItem.DataValue, IResistivityProvider).Resistivity)
            End If

            If _headingLayoutPanel IsNot Nothing Then

                Dim IsAnyOfTheControlCustom As Boolean = False
                For Each cntrl As VoltageDropUserControl In _voltageDropControls
                    If (cntrl.ConductorMaterialUCE.SelectedItem IsNot Nothing AndAlso cntrl.ConductorMaterialUCE.SelectedItem.DisplayText = KblObjectType.Custom.ToLocalizedString) Then
                        IsAnyOfTheControlCustom = True
                        Exit For
                    End If
                Next

                For Each cntrl As VoltageDropUserControl In _voltageDropControls
                    If cntrl.ConductorMaterialUCE.SelectedItem IsNot Nothing Then

                        If cntrl.ConductorMaterialUCE.SelectedItem.DisplayText = KblObjectType.Custom.ToLocalizedString Then
                            cntrl.MaterialDensityUNE.Value = 0
                            cntrl.ResistivityCustomUNE.Value = 0
                        Else
                            If cntrl.ConductorMaterialUCE.SelectedItem.DisplayText = WireResistanceCalculatorFormStrings.Copper_Text Then
                                cntrl.MaterialDensityUNE.Value = HarnessAnalyzer.Shared.SPECIFIC_COPPER_WEIGHT
                                cntrl.ResistivityCustomUNE.Value = GeneralSettings.ResistivityCopper
                            ElseIf cntrl.ConductorMaterialUCE.SelectedItem.DisplayText = WireResistanceCalculatorFormStrings.Alu_Text Then
                                cntrl.MaterialDensityUNE.Value = HarnessAnalyzer.Shared.SPECIFIC_ALUMINIUM_WEIGHT
                                cntrl.ResistivityCustomUNE.Value = GeneralSettings.ResistivityAluminium
                            Else
                                SetMaterialDensityAndResistivity(cntrl)
                            End If
                        End If

                        cntrl.MaterialDensityUNE.ReadOnly = If(cntrl.ConductorMaterialUCE.SelectedItem.DisplayText <> KblObjectType.Custom.ToLocalizedString, True, False)
                        cntrl.ResistivityCustomUNE.ReadOnly = If(cntrl.ConductorMaterialUCE.SelectedItem.DisplayText <> KblObjectType.Custom.ToLocalizedString, True, False)
                    End If
                Next


            End If

            If TypeOf (_dicofWireOrCoreOccurrence.Values(0)) Is Core_occurrence Then
                CalculateGenericWeight(CType(_dicofWireOrCoreOccurrence.Values(0), Core_occurrence))
            ElseIf TypeOf (_dicofWireOrCoreOccurrence.Values(0)) Is Wire_occurrence Then
                CalculateGenericWeight(CType(_dicofWireOrCoreOccurrence.Values(0), Wire_occurrence))
            End If
            If ConductorMaterialUCE.SelectedItem IsNot Nothing Then
                Me.ConductorWeightUNE.Value = _conductor
                Me.TotalWeightUNE.Value = _total
            End If
            If _dt IsNot Nothing AndAlso _dt.Rows.Count > 0 AndAlso Me.ConductorMaterialUCE.SelectedItem IsNot Nothing Then
                _dt.Rows(_index - 1)(My.Resources.VoltageDropUserControlStrings.Material) = Me.ConductorMaterialUCE.SelectedItem.DisplayText
                _dt.Rows(_index - 1)(My.Resources.VoltageDropUserControlStrings.MaterialDensity) = MaterialDen
                _dt.Rows(_index - 1)(My.Resources.VoltageDropUserControlStrings.ResistivityCustom) = ResistivityCustom

                UpdateResistance(ResistivityCustom)

                _dt.Rows(_index - 1)(My.Resources.VoltageDropUserControlStrings.WireResistance) = Math.Round(Me.Resistance, 4)
                _dt.Rows(_index - 1)(My.Resources.VoltageDropUserControlStrings.ConductorWeight) = ConductorWeight
                _dt.Rows(_index - 1)(My.Resources.VoltageDropUserControlStrings.TotalWeight) = TotalWeight
            End If
            RaiseEvent UpdateResult(Me, New EventArgs())
        End If

    End Sub
    Private Sub MaterialResitivityUNE_ValueChanged(sender As Object, e As EventArgs) Handles MaterialDensityUNE.ValueChanged
        If MaterialDensityUNE IsNot Nothing Then
            If TypeOf (_dicofWireOrCoreOccurrence.Values(0)) Is Core_occurrence Then
                CalculateGenericWeight(CType(_dicofWireOrCoreOccurrence.Values(0), Core_occurrence))
            ElseIf TypeOf (_dicofWireOrCoreOccurrence.Values(0)) Is Wire_occurrence Then
                CalculateGenericWeight(CType(_dicofWireOrCoreOccurrence.Values(0), Wire_occurrence))
            End If

            If ConductorMaterialUCE.SelectedItem IsNot Nothing Then
                Me.ConductorWeightUNE.Value = _conductor
                Me.TotalWeightUNE.Value = _total
            End If

            If _dt IsNot Nothing AndAlso _dt.Rows.Count > 0 Then
                _dt.Rows(_index - 1)(My.Resources.VoltageDropUserControlStrings.MaterialDensity) = MaterialDen
                _dt.Rows(_index - 1)(My.Resources.VoltageDropUserControlStrings.ResistivityCustom) = ResistivityCustom
                _dt.Rows(_index - 1)(My.Resources.VoltageDropUserControlStrings.ConductorWeight) = ConductorWeight
                _dt.Rows(_index - 1)(My.Resources.VoltageDropUserControlStrings.TotalWeight) = TotalWeight
            End If

            RaiseEvent UpdateResult(Me, New EventArgs())
        End If

    End Sub
    Private Sub ResistivityCustomUNE_ValueChanged(sender As Object, e As EventArgs) Handles ResistivityCustomUNE.ValueChanged
        If ResistivityCustomUNE IsNot Nothing Then
            UpdateResistance(ResistivityCustom)


            If _dt IsNot Nothing AndAlso _dt.Rows.Count > 0 Then
                _dt.Rows(_index - 1)(My.Resources.VoltageDropUserControlStrings.WireResistance) = Math.Round(Me.Resistance, 4)
                _dt.Rows(_index - 1)(My.Resources.VoltageDropUserControlStrings.ResistivityCustom) = ResistivityCustom
            End If

            RaiseEvent UpdateResult(Me, New EventArgs())
        End If
    End Sub

    Private Sub StartResitanceUNE_KeyDown(sender As Object, e As KeyEventArgs) Handles StartResitanceUNE.KeyDown
        If Me.StartResitanceUNE IsNot Nothing AndAlso e.KeyCode = Keys.Enter Then
            Me.StartResitanceUNE.Editor.ExitEditMode(True, True)
            Me.SelectNextControl(StartResitanceUNE, True, True, True, False)
        End If

    End Sub
    Private Sub StartResitanceUNE_MouseClick(sender As Object, e As MouseEventArgs) Handles StartResitanceUNE.MouseClick
        If Me.StartResitanceUNE IsNot Nothing AndAlso e.Button = MouseButtons.Left Then
            Me.StartResitanceUNE.Editor.Focus()
        End If
    End Sub
    Private Sub CsaUNE_KeyDown(sender As Object, e As KeyEventArgs) Handles CsaUNE.KeyDown
        If Me.CsaUNE IsNot Nothing AndAlso e.KeyCode = Keys.Enter Then
            Me.CsaUNE.Editor.ExitEditMode(True, True)
            Me.SelectNextControl(CsaUNE, True, True, True, False)
        End If
    End Sub
    Private Sub CsaUNE_MouseClick(sender As Object, e As MouseEventArgs) Handles CsaUNE.MouseClick
        If Me.CsaUNE IsNot Nothing AndAlso e.Button = MouseButtons.Left Then
            Me.CsaUNE.Editor.Focus()
        End If
    End Sub
    Private Sub EndResistanceUNE_KeyDown(sender As Object, e As KeyEventArgs) Handles EndResistanceUNE.KeyDown
        If Me.EndResistanceUNE IsNot Nothing AndAlso e.KeyCode = Keys.Enter Then
            Me.EndResistanceUNE.Editor.ExitEditMode(True, True)
            Me.SelectNextControl(StartResitanceUNE, True, True, True, False) 'Hint : intention is to shift control to some other control so we exit and can enter again
        End If
    End Sub
    Private Sub EndResistanceUNE_MouseClick(sender As Object, e As MouseEventArgs) Handles EndResistanceUNE.MouseClick
        If Me.EndResistanceUNE IsNot Nothing AndAlso e.Button = MouseButtons.Left Then
            Me.EndResistanceUNE.Editor.Focus()
        End If
    End Sub
    Private Sub MaterialDensityUNE_KeyDown(sender As Object, e As KeyEventArgs) Handles MaterialDensityUNE.KeyDown
        If Me.MaterialDensityUNE IsNot Nothing AndAlso e.KeyCode = Keys.Enter Then
            Me.MaterialDensityUNE.Editor.ExitEditMode(True, True)
            Me.SelectNextControl(MaterialDensityUNE, True, True, True, False)
        End If
    End Sub
    Private Sub MaterialDensityUNE_MouseClick(sender As Object, e As MouseEventArgs) Handles MaterialDensityUNE.MouseClick
        If Me.MaterialDensityUNE IsNot Nothing Then
            Me.MaterialDensityUNE.Editor.Focus()
        End If
    End Sub
    Private Sub ResistivityCustomUNE_KeyDown(sender As Object, e As KeyEventArgs) Handles ResistivityCustomUNE.KeyDown
        If Me.ResistivityCustomUNE IsNot Nothing AndAlso e.KeyCode = Keys.Enter Then
            Me.ResistivityCustomUNE.Editor.ExitEditMode(True, True)
            Me.SelectNextControl(ResistivityCustomUNE, True, True, True, False)
        End If
    End Sub
    Private Sub ResistivityCustomUNE_MouseClick(sender As Object, e As MouseEventArgs) Handles ResistivityCustomUNE.MouseClick
        If Me.ResistivityCustomUNE IsNot Nothing AndAlso e.Button = MouseButtons.Left AndAlso Me.ResistivityCustomUNE.ReadOnly = False Then
            Me.ResistivityCustomUNE.Editor.Focus()
        End If
    End Sub
    Private Sub TemperatureUNE_KeyDown(sender As Object, e As KeyEventArgs) Handles TemperatureUNE.KeyDown
        If Me.TemperatureUNE IsNot Nothing AndAlso e.KeyCode = Keys.Enter Then
            Me.TemperatureUNE.Editor.ExitEditMode(True, True)
            Me.SelectNextControl(TemperatureUNE, True, True, True, False)
        End If
    End Sub
    Private Sub TemperatureUNE_MouseClick(sender As Object, e As MouseEventArgs) Handles TemperatureUNE.MouseClick
        If Me.TemperatureUNE IsNot Nothing AndAlso e.Button = MouseButtons.Left Then
            Me.TemperatureUNE.Editor.Focus()
        End If
    End Sub
    Private Sub MaterialDensityUNE_AfterEnterEditMode(sender As Object, e As EventArgs) Handles MaterialDensityUNE.AfterEnterEditMode
        If Me.MaterialDensityUNE IsNot Nothing AndAlso Me.MaterialDensityUNE.ReadOnly = True Then
            Me.MaterialDensityUNE.Editor.ExitEditMode(True, True)
        End If
    End Sub
    Private Sub ResistivityCustomUNE_AfterEnterEditMode(sender As Object, e As EventArgs) Handles ResistivityCustomUNE.AfterEnterEditMode
        If Me.ResistivityCustomUNE IsNot Nothing AndAlso Me.ResistivityCustomUNE.ReadOnly = True Then
            Me.ResistivityCustomUNE.Editor.ExitEditMode(True, True)
        End If
    End Sub
    Private Sub WireOrCoreNumUTE_AfterEnterEditMode(sender As Object, e As EventArgs) Handles WireOrCoreNumUTE.AfterEnterEditMode
        If Me.WireOrCoreNumUTE IsNot Nothing AndAlso Me.WireOrCoreNumUTE.ReadOnly = True Then
            Me.WireOrCoreNumUTE.Editor.ExitEditMode(True, True)
        End If
    End Sub
    Private Sub CsaUNE_AfterEnterEditMode(sender As Object, e As EventArgs) Handles CsaUNE.AfterEnterEditMode
        If Me.CsaUNE IsNot Nothing AndAlso Me.CsaUNE.ReadOnly = True Then
            Me.CsaUNE.Editor.ExitEditMode(True, True)
        End If
    End Sub
    Private Sub LengthUNE_AfterEnterEditMode(sender As Object, e As EventArgs) Handles LengthUNE.AfterEnterEditMode
        If Me.LengthUNE IsNot Nothing AndAlso Me.LengthUNE.ReadOnly = True Then
            Me.LengthUNE.Editor.ExitEditMode(True, True)
        End If

    End Sub
    Private Sub ConductorWeightUNE_AfterEnterEditMode(sender As Object, e As EventArgs) Handles ConductorWeightUNE.AfterEnterEditMode
        If Me.ConductorWeightUNE IsNot Nothing AndAlso Me.ConductorWeightUNE.ReadOnly = True Then
            Me.ConductorWeightUNE.Editor.ExitEditMode(True, True)
        End If
    End Sub
    Private Sub TotalWeightUNE_AfterEnterEditMode(sender As Object, e As EventArgs) Handles TotalWeightUNE.AfterEnterEditMode
        If Me.TotalWeightUNE IsNot Nothing AndAlso Me.TotalWeightUNE.ReadOnly = True Then
            Me.TotalWeightUNE.Editor.ExitEditMode(True, True)
        End If
    End Sub
    Private Sub FirstContactPointUTE_AfterEnterEditMode(sender As Object, e As EventArgs) Handles FirstContactPointUTE.AfterEnterEditMode
        If Me.FirstContactPointUTE IsNot Nothing Then
            Me.FirstContactPointUTE.Editor.ExitEditMode(True, True)
        End If
    End Sub
    Private Sub LastContactPointUTE_AfterEnterEditMode(sender As Object, e As EventArgs) Handles LastContactPointUTE.AfterEnterEditMode
        If Me.LastContactPointUTE IsNot Nothing Then
            Me.LastContactPointUTE.Editor.ExitEditMode(True, True)
        End If
    End Sub
End Class
