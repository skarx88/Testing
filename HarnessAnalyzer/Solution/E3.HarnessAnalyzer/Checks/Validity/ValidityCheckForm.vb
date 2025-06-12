Imports Infragistics.Win
Imports Infragistics.Win.UltraWinGrid

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class ValidityCheckForm

    Friend Event ValidityCheckEntrySelectionChanged(sender As Object, e As InformationHubEventArgs)

    Private _kblMapper As KblMapper = Nothing
    Private _validityCheckContainer As ValidityCheckContainer = Nothing
    Private _validityCheckEntries As List(Of ValidityCheckEntry) = Nothing

    Public Sub New(kblMapper As KblMapper, validityCheckContainer As ValidityCheckContainer)
        InitializeComponent()

        Me.BackColor = Color.White
        Me.Icon = My.Resources.ValidityCheck
        Me.Text &= ValidityCheckFormStrings.Dialog_CaptionForAll

        _kblMapper = kblMapper
        _validityCheckContainer = validityCheckContainer
    End Sub

    Public Sub New(validityCheckEntries As List(Of ValidityCheckEntry))
        InitializeComponent()

        Me.BackColor = Color.White
        Me.Icon = My.Resources.ValidityCheck
        Me.Text &= String.Format(ValidityCheckFormStrings.Dialog_CaptionForObject, validityCheckEntries.FirstOrDefault.ID)

        _validityCheckEntries = validityCheckEntries
    End Sub


    Private Sub ValidityForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.ugValidityCheckResults.SyncWithCurrencyManager = False

        If (_validityCheckContainer IsNot Nothing) Then
            Me.ugValidityCheckResults.SetDataBinding(_validityCheckContainer.ValidityCheckEntries, String.Empty, True, True)
        ElseIf (_validityCheckEntries IsNot Nothing) Then
            Me.ugValidityCheckResults.SetDataBinding(_validityCheckEntries, String.Empty, True, True)
        End If
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        If (_validityCheckContainer IsNot Nothing) Then
            Me.Hide()
        Else
            Me.DialogResult = DialogResult.OK
        End If
    End Sub

    Private Sub ugValidityCheckResults_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugValidityCheckResults.InitializeLayout
        Me.ugValidityCheckResults.BeginUpdate()

        With e.Layout
            .AutoFitStyle = AutoFitStyle.ExtendLastColumn
            .CaptionVisible = DefaultableBoolean.False
            .GroupByBox.Hidden = True

            With .Override
                .AllowColMoving = AllowColMoving.NotAllowed
                .AllowDelete = DefaultableBoolean.False
                .AllowGroupMoving = AllowGroupMoving.NotAllowed
                .AllowRowFiltering = DefaultableBoolean.True
                .CellClickAction = CellClickAction.RowSelect
                .RowSelectors = DefaultableBoolean.False
                .RowSizing = RowSizing.AutoFree

                If (_validityCheckContainer IsNot Nothing) Then
                    .SelectTypeRow = SelectType.SingleAutoDrag
                Else
                    .ActiveAppearancesEnabled = DefaultableBoolean.False
                    .SelectTypeRow = SelectType.None
                End If
            End With

            For Each band As UltraGridBand In .Bands
                For Each column As UltraGridColumn In band.Columns
                    With column
                        Select Case .Key
                            Case "Code"
                                .Header.Caption = ValidityCheckFormStrings.Code_ColCaption
                                .Header.VisiblePosition = 2
                                .MinWidth = 75
                                .MaxWidth = 150
                                .SortIndicator = SortIndicator.Ascending
                                .Width = If(_validityCheckContainer IsNot Nothing, 110, 125)
                            Case "Description"
                                .CellMultiLine = DefaultableBoolean.True
                                .Header.Caption = ValidityCheckFormStrings.Description_ColCaption
                                .Header.VisiblePosition = 3
                                .MinWidth = 100
                                .MaxWidth = 1024
                                .Width = If(_validityCheckContainer IsNot Nothing, 175, 225)
                            Case "ID", "ObjectType"
                                If (_validityCheckContainer IsNot Nothing) Then
                                    .Header.Caption = If(.Key = "ID", ValidityCheckFormStrings.ID_ColCaption, ValidityCheckFormStrings.ObjectType_ColCaption)
                                    .Header.VisiblePosition = If(.Key = "ID", 0, 1)
                                    .MinWidth = 75
                                    .MaxWidth = 100
                                    .Width = 80
                                Else
                                    .Hidden = True
                                End If
                            Case "ObjectTypeSpecified"
                                .Hidden = True
                            Case "ResolutionHint"
                                .CellMultiLine = DefaultableBoolean.True
                                .Header.Caption = ValidityCheckFormStrings.ResolutionHint_ColCaption
                                .Header.VisiblePosition = 5
                                .MinWidth = 100
                                .MaxWidth = 1024
                                .Width = If(_validityCheckContainer IsNot Nothing, 175, 225)
                            Case "Text"
                                .CellMultiLine = DefaultableBoolean.True
                                .Header.Caption = ValidityCheckFormStrings.Text_ColCaption
                                .Header.VisiblePosition = 4
                                .MinWidth = 125
                                .MaxWidth = 1024
                                .Width = 250
                            Case "Type"
                                .Header.Caption = ValidityCheckFormStrings.Type_ColCaption
                                .Header.VisiblePosition = 6
                                .MinWidth = 50
                                .MaxWidth = 1024
                                .Width = 70
                        End Select
                    End With
                Next
            Next
        End With

        Me.ugValidityCheckResults.EndUpdate()
    End Sub

    Private Sub ugValidityCheckResults_MouseClick(sender As Object, e As MouseEventArgs) Handles ugValidityCheckResults.MouseClick
        If (e.Button = System.Windows.Forms.MouseButtons.Left) Then
            Dim element As UIElement = Me.ugValidityCheckResults.DisplayLayout.UIElement.LastElementEntered
            Dim cell As UltraGridCell = TryCast(element.GetContext(GetType(UltraGridCell)), UltraGridCell)

            If (cell IsNot Nothing) AndAlso (_kblMapper IsNot Nothing) AndAlso (_validityCheckContainer IsNot Nothing) Then
                Dim idOrPartNumber As String = cell.Row.Cells("ID").Value.ToString
                Dim kblObjectType As KblObjectType = CType(cell.Row.Cells("ObjectType").Value, KblObjectType)
                If kblObjectType = KblObjectType.Cavity_occurrence Then
                    Dim connId As String = idOrPartNumber.Split(",".ToCharArray, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault
                    Dim cavNum As String = idOrPartNumber.Split(",".ToCharArray, StringSplitOptions.RemoveEmptyEntries).LastOrDefault

                    If (_kblMapper.KBLIdMapper.ContainsKey(connId)) AndAlso (_kblMapper.KBLOccurrenceMapper.ContainsKey(_kblMapper.KBLIdMapper(connId).FirstOrDefault)) Then
                        Dim connOcc As Connector_occurrence = DirectCast(_kblMapper.KBLOccurrenceMapper(_kblMapper.KBLIdMapper(connId).FirstOrDefault), Connector_occurrence)
                        If (connOcc.Slots IsNot Nothing) Then
                            For Each cavityOcc As Cavity_occurrence In connOcc.Slots.FirstOrDefault.Cavities
                                If (_kblMapper.KBLPartMapper.ContainsKey(cavityOcc.Part)) AndAlso (DirectCast(_kblMapper.KBLPartMapper(cavityOcc.Part), Cavity).Cavity_number = cavNum) Then
                                    RaiseEvent ValidityCheckEntrySelectionChanged(_kblMapper.HarnessPartNumber, New InformationHubEventArgs(_kblMapper.Id, kblObjectType, {cavityOcc.SystemId}))
                                    Exit For
                                End If
                            Next
                        End If
                    End If
                ElseIf _kblMapper.KBLIdMapper.ContainsKey(idOrPartNumber) Then
                    RaiseEvent ValidityCheckEntrySelectionChanged(_kblMapper.HarnessPartNumber, New InformationHubEventArgs(_kblMapper.Id, _kblMapper.KBLIdMapper(idOrPartNumber), kblObjectType))
                Else
                    Dim part As IKblPartObject = _kblMapper.GetPartByNumnber(idOrPartNumber)
                    If part IsNot Nothing Then
                        If (_kblMapper.GetAccessoryOccurrences.Any(Function(occ) occ.Part = part.SystemId)) Then
                            RaiseEvent ValidityCheckEntrySelectionChanged(_kblMapper.HarnessPartNumber, New InformationHubEventArgs(_kblMapper.Id, _kblMapper.GetAccessoryOccurrences.Where(Function(occ) occ.Part = part.SystemId).Select(Function(occ) occ.SystemId), E3.Lib.Schema.Kbl.KblObjectType.Accessory_occurrence))
                        End If
                        If (_kblMapper.GetAssemblyPartOccurrences.Any(Function(occ) occ.Part = part.SystemId)) Then
                            RaiseEvent ValidityCheckEntrySelectionChanged(_kblMapper.HarnessPartNumber, New InformationHubEventArgs(_kblMapper.Id, _kblMapper.GetAssemblyPartOccurrences.Where(Function(occ) occ.Part = part.SystemId).Select(Function(occ) occ.SystemId), E3.Lib.Schema.Kbl.KblObjectType.Assembly_part_occurrence))
                        End If
                        If (_kblMapper.GetCavityPlugOccurrences.Any(Function(occ) occ.Part = part.SystemId)) Then
                            RaiseEvent ValidityCheckEntrySelectionChanged(_kblMapper.HarnessPartNumber, New InformationHubEventArgs(_kblMapper.Id, _kblMapper.GetCavityPlugOccurrences.Where(Function(occ) occ.Part = part.SystemId).Select(Function(occ) occ.SystemId), E3.Lib.Schema.Kbl.KblObjectType.Cavity_plug_occurrence))
                        End If
                        If (_kblMapper.GetCavitySealOccurrences.Any(Function(occ) occ.Part = part.SystemId)) Then
                            RaiseEvent ValidityCheckEntrySelectionChanged(_kblMapper.HarnessPartNumber, New InformationHubEventArgs(_kblMapper.Id, _kblMapper.GetCavitySealOccurrences.Where(Function(occ) occ.Part = part.SystemId).Select(Function(occ) occ.SystemId), E3.Lib.Schema.Kbl.KblObjectType.Cavity_seal_occurrence))
                        End If
                        If (_kblMapper.GetComponentOccurrences.Any(Function(occ) occ.Part = part.SystemId)) Then
                            RaiseEvent ValidityCheckEntrySelectionChanged(_kblMapper.HarnessPartNumber, New InformationHubEventArgs(_kblMapper.Id, _kblMapper.GetComponentOccurrences.Where(Function(occ) occ.Part = part.SystemId).Select(Function(occ) occ.SystemId), E3.Lib.Schema.Kbl.KblObjectType.Component_occurrence))
                        End If
                        If (_kblMapper.GetConnectorOccurrences.Any(Function(occ) occ.Part = part.SystemId)) Then
                            RaiseEvent ValidityCheckEntrySelectionChanged(_kblMapper.HarnessPartNumber, New InformationHubEventArgs(_kblMapper.Id, _kblMapper.GetConnectorOccurrences.Where(Function(occ) occ.Part = part.SystemId).Select(Function(occ) occ.SystemId), E3.Lib.Schema.Kbl.KblObjectType.Connector_occurrence))
                        End If
                        If (_kblMapper.GetCoPackOccurrences.Any(Function(occ) occ.Part = part.SystemId)) Then
                            RaiseEvent ValidityCheckEntrySelectionChanged(_kblMapper.HarnessPartNumber, New InformationHubEventArgs(_kblMapper.Id, _kblMapper.GetCoPackOccurrences.Where(Function(occ) occ.Part = part.SystemId).Select(Function(occ) occ.SystemId), E3.Lib.Schema.Kbl.KblObjectType.Co_pack_occurrence))
                        End If
                        If (_kblMapper.GetFixingOccurrences.Any(Function(occ) occ.Part = part.SystemId)) Then
                            RaiseEvent ValidityCheckEntrySelectionChanged(_kblMapper.HarnessPartNumber, New InformationHubEventArgs(_kblMapper.Id, _kblMapper.GetFixingOccurrences.Where(Function(occ) occ.Part = part.SystemId).Select(Function(occ) occ.SystemId), E3.Lib.Schema.Kbl.KblObjectType.Fixing_occurrence))
                        End If
                        If (_kblMapper.GetGeneralWireOccurrences.Any(Function(occ) occ.Part = part.SystemId)) Then
                            RaiseEvent ValidityCheckEntrySelectionChanged(_kblMapper.HarnessPartNumber, New InformationHubEventArgs(_kblMapper.Id, _kblMapper.GetGeneralWireOccurrences.Where(Function(occ) occ.Part = part.SystemId).Select(Function(occ) occ.SystemId), E3.Lib.Schema.Kbl.KblObjectType.Wire_occurrence))
                        End If
                        If (_kblMapper.GetSpecialTerminalOccurrences.Any(Function(occ) occ.Part = part.SystemId)) Then
                            RaiseEvent ValidityCheckEntrySelectionChanged(_kblMapper.HarnessPartNumber, New InformationHubEventArgs(_kblMapper.Id, _kblMapper.GetSpecialTerminalOccurrences.Where(Function(occ) occ.Part = part.SystemId).Select(Function(occ) occ.SystemId), E3.Lib.Schema.Kbl.KblObjectType.Special_terminal_occurrence))
                        End If
                        If (_kblMapper.GetTerminalOccurrences.Any(Function(occ) occ.Part = part.SystemId)) Then
                            RaiseEvent ValidityCheckEntrySelectionChanged(_kblMapper.HarnessPartNumber, New InformationHubEventArgs(_kblMapper.Id, _kblMapper.GetTerminalOccurrences.Where(Function(occ) occ.Part = part.SystemId).Select(Function(occ) occ.SystemId), E3.Lib.Schema.Kbl.KblObjectType.Terminal_occurrence))
                        End If
                        If (_kblMapper.GetWireProtectionOccurrences.Any(Function(occ) occ.Part = part.SystemId)) Then
                            RaiseEvent ValidityCheckEntrySelectionChanged(_kblMapper.HarnessPartNumber, New InformationHubEventArgs(_kblMapper.Id, _kblMapper.GetWireProtectionOccurrences.Where(Function(occ) occ.Part = part.SystemId).Select(Function(occ) occ.SystemId), E3.Lib.Schema.Kbl.KblObjectType.Wire_protection_occurrence))
                        End If
                    End If
                End If
            End If
        End If
    End Sub

End Class