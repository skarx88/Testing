Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinGrid

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class TooltipForm

    Private _gridHeight As Integer
    Private _gridWidth As Integer

    Private _kblContainer As IKblContainer
    Private _kblCavityContactPointMapper As IStringMappingDictionary(Of Contact_point)
    Private _kblContactPointWireMapper As IStringMappingDictionary(Of String)
    Private _kblCoreCableMapper As IDictionary(Of String, String)
    Private _kblObjectModuleMapper As IStringMappingDictionary(Of String)
    Private _kblOccurrenceMapper As IDictionary(Of String, IKblOccurrence)
    Private _PartMapper As IDictionary(Of String, Object)
    Private _kblUnitMapper As IDictionary(Of String, Unit)

    Public Sub New(kblContainer As IKblContainer, kblCavityContactPointMapper As IStringMappingDictionary(Of Contact_point), kblContactPointWireMapper As IStringMappingDictionary(Of String), kblCoreCableMapper As IDictionary(Of String, String), kblObjectModuleMapper As IStringMappingDictionary(Of String), kblOccurrenceMapper As IDictionary(Of String, IKblOccurrence), PartMapper As IDictionary(Of String, Object), kblUnitMapper As IDictionary(Of String, Unit))
        InitializeComponent()

        _kblContainer = kblContainer
        _kblCavityContactPointMapper = kblCavityContactPointMapper
        _kblContactPointWireMapper = kblContactPointWireMapper
        _kblCoreCableMapper = kblCoreCableMapper
        _kblObjectModuleMapper = kblObjectModuleMapper
        _kblOccurrenceMapper = kblOccurrenceMapper
        _PartMapper = PartMapper
        _kblUnitMapper = kblUnitMapper

        Me.ugTooltip.SyncWithCurrencyManager = False
    End Sub

    Friend Sub FillDataSource(kblId As String)
        Me.udsTooltip.Reset()

        If (_kblOccurrenceMapper.ContainsKey(kblId)) Then
            If (TypeOf _kblOccurrenceMapper(kblId) Is Connector_occurrence) Then
                FillConnectorOrVertexDataSource(_kblOccurrenceMapper(kblId))
            ElseIf (TypeOf _kblOccurrenceMapper(kblId) Is Node) Then
                FillConnectorOrVertexDataSource(_kblOccurrenceMapper(kblId))
            ElseIf (TypeOf _kblOccurrenceMapper(kblId) Is Segment) Then
                FillSegmentDataSource(DirectCast(_kblOccurrenceMapper(kblId), Segment))
            End If
        End If

        Me.ugTooltip.BeginUpdate()
        Me.ugTooltip.DataSource = Me.udsTooltip

        InitializeGridLayout(Me.ugTooltip.DisplayLayout)

        Me.ugTooltip.EndUpdate()
    End Sub


    Private Sub FillSegmentDataSource(segment As Segment)
        Me.ugTooltip.Text = String.Format(TooltipFormStrings.ProtForSeg_Caption, segment.Id)

        With Me.udsTooltip
            With .Band
                .Columns.Add(TooltipFormStrings.Id_ColCaption)
                .Columns.Add(TooltipFormStrings.PartNumber_ColCaption)
                .Columns.Add(TooltipFormStrings.Desc_ColCaption)
                .Columns.Add(TooltipFormStrings.Length_ColCaption)
                .Columns.Add(TooltipFormStrings.ProtType_ColCaption)
            End With

            For Each protectionArea As Protection_area In segment.Protection_area
                Dim wireProtectionOccurrence As Wire_protection_occurrence = DirectCast(_kblOccurrenceMapper(protectionArea.Associated_protection.ToString), Wire_protection_occurrence)
                Dim wireProtectionPart As Wire_protection = DirectCast(_PartMapper(wireProtectionOccurrence.Part), Wire_protection)

                Dim row As UltraDataRow = .Rows.Add
                row.SetCellValue(TooltipFormStrings.Id_ColCaption, wireProtectionOccurrence.Id)
                row.SetCellValue(TooltipFormStrings.PartNumber_ColCaption, wireProtectionPart.Part_number)

                If (wireProtectionOccurrence.Description IsNot Nothing) Then row.SetCellValue(TooltipFormStrings.Desc_ColCaption, wireProtectionOccurrence.Description)
                If (wireProtectionOccurrence.Protection_length IsNot Nothing) Then row.SetCellValue(TooltipFormStrings.Length_ColCaption, String.Format("{0} {1}", Math.Round(wireProtectionOccurrence.Protection_length.Value_component, 2), _kblUnitMapper(wireProtectionOccurrence.Protection_length.Unit_component).Unit_name))
                If (wireProtectionPart.Protection_type IsNot Nothing) Then row.SetCellValue(TooltipFormStrings.ProtType_ColCaption, wireProtectionPart.Protection_type)
            Next
        End With
    End Sub

    Private Sub FillConnectorOrVertexDataSource(connectorOrNodeOcc As Object)
        'This handling is reworked now: modules must be taken from the wires and there should not be a wire concatenation on double crimps.
        'there is only the first slot considered, this might be a general problem also in the informationhub! 

        With Me.udsTooltip
            With .Band
                .Columns.Add(TooltipFormStrings.Pin_ColCaption)
                .Columns.Add(TooltipFormStrings.WirNum_ColCaption)
                .Columns.Add(TooltipFormStrings.Type_ColCaption)
                .Columns.Add(TooltipFormStrings.CSA_ColCaption)
                .Columns.Add(TooltipFormStrings.Color_ColCaption)
                .Columns.Add(TooltipFormStrings.Mods_ColCaption)
                .Columns.Add(TooltipFormStrings.Terminal_ColCaption)
                .Columns.Add(TooltipFormStrings.Seal_ColCaption)
                .Columns.Add(TooltipFormStrings.Plug_ColCaption)
                .Columns.Add(TooltipFormStrings.PlatMat_ColCaption)
            End With
        End With

        Dim connectorOcc As Connector_occurrence = If(TypeOf connectorOrNodeOcc Is Connector_occurrence, DirectCast(connectorOrNodeOcc, Connector_occurrence), Nothing)
        If (connectorOcc Is Nothing) Then
            For Each referenceId As String In DirectCast(connectorOrNodeOcc, Node).Referenced_components.SplitSpace
                If (_kblOccurrenceMapper.ContainsKey(referenceId)) AndAlso (TypeOf _kblOccurrenceMapper(referenceId) Is Connector_occurrence) Then
                    connectorOcc = DirectCast(_kblOccurrenceMapper(referenceId), Connector_occurrence)

                    Exit For
                End If
            Next
        End If

        If (connectorOcc IsNot Nothing) Then
            With connectorOcc
                Me.ugTooltip.Text = String.Format(TooltipFormStrings.Housing_Caption, .Id)

                If (.Slots IsNot Nothing) AndAlso (.Slots.Length <> 0) Then
                    For Each cavityOccurrence As Cavity_occurrence In .Slots(0).Cavities
                        Dim plugPartNumber As String = String.Empty
                        Dim modules As String = String.Empty

                        If (cavityOccurrence.Associated_plug IsNot Nothing) Then
                            plugPartNumber = DirectCast(_PartMapper((DirectCast(_kblOccurrenceMapper(cavityOccurrence.Associated_plug), Cavity_plug_occurrence).Part)), Cavity_plug).Part_number
                        End If

                        If (_kblCavityContactPointMapper.ContainsKey(cavityOccurrence.SystemId)) Then
                            For Each contactPoint As Contact_point In _kblCavityContactPointMapper(cavityOccurrence.SystemId)
                                Dim sealPartNumber As String = String.Empty
                                Dim terminalPartNumber As String = String.Empty
                                Dim terminalPlatingMaterial As String = String.Empty

                                If (contactPoint.Associated_parts IsNot Nothing) AndAlso (contactPoint.Associated_parts <> String.Empty) Then
                                    For Each associatedPart As String In contactPoint.Associated_parts.SplitSpace
                                        If (_kblOccurrenceMapper.ContainsKey(associatedPart) AndAlso TypeOf _kblOccurrenceMapper(associatedPart) Is Cavity_seal_occurrence) Then
                                            sealPartNumber = ConcatenateEntries(sealPartNumber, DirectCast(_PartMapper(DirectCast(_kblOccurrenceMapper(associatedPart), Cavity_seal_occurrence).Part), Cavity_seal).Part_number)

                                        ElseIf (_kblOccurrenceMapper.ContainsKey(associatedPart) AndAlso TypeOf _kblOccurrenceMapper(associatedPart) Is Special_terminal_occurrence) Then
                                            terminalPartNumber = ConcatenateEntries(terminalPartNumber, DirectCast(_PartMapper(DirectCast(_kblOccurrenceMapper(associatedPart), Special_terminal_occurrence).Part), General_terminal).Part_number)
                                            terminalPlatingMaterial = ConcatenateEntries(terminalPlatingMaterial, DirectCast(_PartMapper(DirectCast(_kblOccurrenceMapper(associatedPart), Special_terminal_occurrence).Part), General_terminal).Plating_material)

                                        ElseIf (_kblOccurrenceMapper.ContainsKey(associatedPart) AndAlso TypeOf _kblOccurrenceMapper(associatedPart) Is Terminal_occurrence) Then
                                            terminalPartNumber = ConcatenateEntries(terminalPartNumber, DirectCast(_PartMapper(DirectCast(_kblOccurrenceMapper(associatedPart), Terminal_occurrence).Part), General_terminal).Part_number)
                                            terminalPlatingMaterial = ConcatenateEntries(terminalPlatingMaterial, DirectCast(_PartMapper(DirectCast(_kblOccurrenceMapper(associatedPart), Terminal_occurrence).Part), General_terminal).Plating_material)
                                        End If
                                    Next
                                End If

                                If (_kblContactPointWireMapper.ContainsKey(contactPoint.SystemId)) Then
                                    Dim wireColor As String = String.Empty
                                    Dim wireCSA As String = String.Empty
                                    Dim wireNumber As String = String.Empty
                                    Dim wireType As String = String.Empty

                                    For Each wireId As String In _kblContactPointWireMapper(contactPoint.SystemId)

                                        Dim row As UltraDataRow = Me.udsTooltip.Rows.Add
                                        row.SetCellValue(TooltipFormStrings.Pin_ColCaption, DirectCast(_PartMapper(cavityOccurrence.Part), Cavity).Cavity_number)

                                        If (TypeOf _kblOccurrenceMapper(wireId) Is Core_occurrence) Then
                                            With DirectCast(_kblOccurrenceMapper(wireId), Core_occurrence)
                                                Dim corePart As Core = DirectCast(_PartMapper(.Part), Core)

                                                wireNumber = .Wire_number
                                                If (corePart.Wire_type IsNot Nothing) Then
                                                    wireType = corePart.Wire_type
                                                End If

                                                wireCSA = String.Format("{0} {1}", Math.Round(corePart.Cross_section_area.Value_component, 2), _kblUnitMapper(corePart.Cross_section_area.Unit_component).Unit_name)
                                                wireColor = corePart.GetColours

                                            End With

                                        ElseIf (TypeOf _kblOccurrenceMapper(wireId) Is Wire_occurrence) Then
                                            With DirectCast(_kblOccurrenceMapper(wireId), Wire_occurrence)
                                                Dim wirePart As General_wire = DirectCast(_PartMapper(.Part), General_wire)
                                                wireNumber = .Wire_number

                                                If (wirePart.Wire_type IsNot Nothing) Then
                                                    wireType = wirePart.Wire_type
                                                End If
                                                wireCSA = String.Format("{0} {1}", Math.Round(wirePart.Cross_section_area.Value_component, 2), _kblUnitMapper(wirePart.Cross_section_area.Unit_component).Unit_name)
                                                wireColor = wirePart.GetColours
                                            End With
                                        End If

                                        row.SetCellValue(TooltipFormStrings.Mods_ColCaption, GetAssignedModules(wireId))

                                        row.SetCellValue(TooltipFormStrings.WirNum_ColCaption, wireNumber)
                                        row.SetCellValue(TooltipFormStrings.Type_ColCaption, wireType)
                                        row.SetCellValue(TooltipFormStrings.CSA_ColCaption, wireCSA)
                                        row.SetCellValue(TooltipFormStrings.Color_ColCaption, wireColor)

                                        row.SetCellValue(TooltipFormStrings.Plug_ColCaption, plugPartNumber)
                                        row.SetCellValue(TooltipFormStrings.Terminal_ColCaption, terminalPartNumber)
                                        row.SetCellValue(TooltipFormStrings.Seal_ColCaption, sealPartNumber)
                                        row.SetCellValue(TooltipFormStrings.PlatMat_ColCaption, terminalPlatingMaterial)

                                    Next

                                Else
                                    Dim row As UltraDataRow = Me.udsTooltip.Rows.Add
                                    row.SetCellValue(TooltipFormStrings.Pin_ColCaption, DirectCast(_PartMapper(cavityOccurrence.Part), Cavity).Cavity_number)
                                    row.SetCellValue(TooltipFormStrings.Plug_ColCaption, plugPartNumber)
                                    row.SetCellValue(TooltipFormStrings.Terminal_ColCaption, terminalPartNumber)
                                    row.SetCellValue(TooltipFormStrings.Seal_ColCaption, sealPartNumber)
                                    row.SetCellValue(TooltipFormStrings.PlatMat_ColCaption, terminalPlatingMaterial)

                                End If
                            Next
                        Else
                            Dim row As UltraDataRow = Me.udsTooltip.Rows.Add
                            row.SetCellValue(TooltipFormStrings.Pin_ColCaption, DirectCast(_PartMapper(cavityOccurrence.Part), Cavity).Cavity_number)
                            row.SetCellValue(TooltipFormStrings.Plug_ColCaption, plugPartNumber)
                        End If

                    Next
                End If
            End With
        End If
    End Sub

    Private Function ConcatenateEntries(entry As String, value As String) As String
        If String.IsNullOrEmpty(value) Then
            Return entry
        ElseIf String.IsNullOrEmpty(entry) Then
            Return value
        Else
            Return String.Format("{0};{1}", entry, value)
        End If
    End Function

    Private Function GetAssignedModules(kblId As String) As String
        Dim assignedModules As String = String.Empty
        Dim modules As New List(Of String)

        If (_kblObjectModuleMapper.ContainsKey(kblId)) Then
            For Each moduleId As String In _kblObjectModuleMapper(kblId)
                If (_kblOccurrenceMapper.ContainsKey(moduleId)) AndAlso (Not modules.Contains(moduleId)) Then
                    If (assignedModules <> String.Empty) Then
                        assignedModules = String.Format("{0}|{1}", assignedModules, DirectCast(_kblOccurrenceMapper(moduleId), [Module]).Abbreviation)
                    Else
                        assignedModules = DirectCast(_kblOccurrenceMapper(moduleId), [Module]).Abbreviation
                    End If

                    modules.Add(moduleId)
                End If
            Next
        End If

        Return assignedModules
    End Function

    Private Sub InitializeGridLayout(layout As UltraGridLayout)
        _gridHeight = 0
        _gridWidth = 2

        With layout
            .CaptionVisible = Infragistics.Win.DefaultableBoolean.True
            .GroupByBox.Hidden = True

            With .Override
                .AllowColMoving = AllowColMoving.NotAllowed
                .AllowDelete = Infragistics.Win.DefaultableBoolean.False
                .AllowUpdate = Infragistics.Win.DefaultableBoolean.False
                .RowSelectors = Infragistics.Win.DefaultableBoolean.False
                .SelectTypeRow = SelectType.Single
            End With

            For Each column As UltraGridColumn In .Bands(0).Columns
                If (Not column.Hidden) Then
                    With column
                        .CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center
                        .CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle
                        .Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center
                        .Header.Appearance.TextVAlign = Infragistics.Win.VAlign.Middle
                        .MinWidth = 75

                        If (column.Index = 0) Then
                            .SortComparer = New NumericStringSortComparer
                            .SortIndicator = SortIndicator.Ascending
                        End If
                    End With
                End If
            Next

            .PerformAutoResizeColumns(False, PerformAutoSizeType.AllRowsInBand)

            _gridHeight = .Bands(0).Columns(0).Header.Height * 2

            For Each row As UltraGridRow In .Rows
                _gridHeight += row.Height
            Next

            For Each column As UltraGridColumn In .Bands(0).Columns
                _gridWidth += column.Width
            Next
        End With

        Me.ugTooltip.Height = _gridHeight
        Me.ugTooltip.Width = _gridWidth

        Me.ugTooltip.ActiveRow = Nothing
        Me.ugTooltip.Selected.Rows.Clear()
    End Sub

End Class