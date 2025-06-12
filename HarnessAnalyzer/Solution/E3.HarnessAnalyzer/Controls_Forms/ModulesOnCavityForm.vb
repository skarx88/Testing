Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinGrid

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class ModulesOnCavityForm

    Private _kblMapper As KblMapper
    Private _objectIds As List(Of String)

    Public Sub New(kblMapper As KblMapper, objectIds As List(Of String))
        InitializeComponent()

        _kblMapper = kblMapper
        _objectIds = objectIds

        Initialize()
    End Sub

    Private Sub Initialize()
        Me.BackColor = Color.White

        Me.ugModulesOnPlug.SyncWithCurrencyManager = False
        Me.ugModulesOnSeal.SyncWithCurrencyManager = False
        Me.ugModulesOnTerminal.SyncWithCurrencyManager = False

        With Me.udsModulesOnPlug
            With .Band
                .Columns.Add(ModulesOnCavityFormStrings.ModId_ColCaption)
                .Columns.Add(ModulesOnCavityFormStrings.PartNumber_ColCaption)
                .Columns.Add(ModulesOnCavityFormStrings.Abbreviation_ColCaption)
                .Columns.Add(ModulesOnCavityFormStrings.Desc_ColCaption)
                .Columns.Add(ModulesOnCavityFormStrings.ModFam_ColCaption)
            End With
        End With

        With Me.udsModulesOnSeal
            With .Band
                .Columns.Add(ModulesOnCavityFormStrings.ModId_ColCaption)
                .Columns.Add(ModulesOnCavityFormStrings.PartNumber_ColCaption)
                .Columns.Add(ModulesOnCavityFormStrings.Abbreviation_ColCaption)
                .Columns.Add(ModulesOnCavityFormStrings.Desc_ColCaption)
                .Columns.Add(ModulesOnCavityFormStrings.ModFam_ColCaption)
            End With
        End With

        With Me.udsModulesOnTerminal
            With .Band
                .Columns.Add(ModulesOnCavityFormStrings.ModId_ColCaption)
                .Columns.Add(ModulesOnCavityFormStrings.PartNumber_ColCaption)
                .Columns.Add(ModulesOnCavityFormStrings.Abbreviation_ColCaption)
                .Columns.Add(ModulesOnCavityFormStrings.Desc_ColCaption)
                .Columns.Add(ModulesOnCavityFormStrings.ModFam_ColCaption)
            End With
        End With

        Dim modulesOnTerminal As New List(Of [Module])
        Dim modulesOnSeal As New List(Of [Module])
        Dim modulesOnPlug As New List(Of [Module])

        For Each objectId As String In _objectIds
            If (_kblMapper.KBLObjectModuleMapper.ContainsKey(objectId)) Then
                For Each kblId As String In _kblMapper.KBLObjectModuleMapper(objectId)
                    If (_kblMapper.KBLOccurrenceMapper.ContainsKey(kblId)) AndAlso (TypeOf _kblMapper.KBLOccurrenceMapper(objectId) Is Terminal_occurrence) AndAlso (Not modulesOnTerminal.Contains(DirectCast(_kblMapper.KBLOccurrenceMapper(kblId), [Module]))) Then
                        modulesOnTerminal.Add(DirectCast(_kblMapper.KBLOccurrenceMapper(kblId), [Module]))
                    ElseIf (_kblMapper.KBLOccurrenceMapper.ContainsKey(kblId)) AndAlso (TypeOf _kblMapper.KBLOccurrenceMapper(objectId) Is Special_terminal_occurrence) AndAlso (Not modulesOnTerminal.Contains(DirectCast(_kblMapper.KBLOccurrenceMapper(kblId), [Module]))) Then
                        modulesOnTerminal.Add(DirectCast(_kblMapper.KBLOccurrenceMapper(kblId), [Module]))
                    ElseIf (_kblMapper.KBLOccurrenceMapper.ContainsKey(kblId)) AndAlso (TypeOf _kblMapper.KBLOccurrenceMapper(objectId) Is Cavity_seal_occurrence) AndAlso (Not modulesOnSeal.Contains(DirectCast(_kblMapper.KBLOccurrenceMapper(kblId), [Module]))) Then
                        modulesOnSeal.Add(DirectCast(_kblMapper.KBLOccurrenceMapper(kblId), [Module]))
                    ElseIf (_kblMapper.KBLOccurrenceMapper.ContainsKey(kblId)) AndAlso (TypeOf _kblMapper.KBLOccurrenceMapper(objectId) Is Cavity_plug_occurrence) AndAlso (Not modulesOnPlug.Contains(DirectCast(_kblMapper.KBLOccurrenceMapper(kblId), [Module]))) Then
                        modulesOnPlug.Add(DirectCast(_kblMapper.KBLOccurrenceMapper(kblId), [Module]))
                    End If
                Next
            End If
        Next

        For Each [module] As [Module] In modulesOnTerminal
            Dim row As UltraDataRow = Me.udsModulesOnTerminal.Rows.Add
            row.SetCellValue(ModulesOnCavityFormStrings.ModId_ColCaption, [module].SystemId)
            row.SetCellValue(ModulesOnCavityFormStrings.PartNumber_ColCaption, [module].Part_number)
            row.SetCellValue(ModulesOnCavityFormStrings.Abbreviation_ColCaption, [module].Abbreviation)
            row.SetCellValue(ModulesOnCavityFormStrings.Desc_ColCaption, [module].Description)

            If ([module].Of_family IsNot Nothing) Then row.SetCellValue(ModulesOnCavityFormStrings.ModFam_ColCaption, DirectCast(_kblMapper.KBLOccurrenceMapper([module].Of_family), Module_family).Id)
        Next

        For Each [module] As [Module] In modulesOnSeal
            Dim row As UltraDataRow = Me.udsModulesOnSeal.Rows.Add
            row.SetCellValue(ModulesOnCavityFormStrings.ModId_ColCaption, [module].SystemId)
            row.SetCellValue(ModulesOnCavityFormStrings.PartNumber_ColCaption, [module].Part_number)
            row.SetCellValue(ModulesOnCavityFormStrings.Abbreviation_ColCaption, [module].Abbreviation)
            row.SetCellValue(ModulesOnCavityFormStrings.Desc_ColCaption, [module].Description)

            If ([module].Of_family IsNot Nothing) Then row.SetCellValue(ModulesOnCavityFormStrings.ModFam_ColCaption, DirectCast(_kblMapper.KBLOccurrenceMapper([module].Of_family), Module_family).Id)
        Next

        For Each [module] As [Module] In modulesOnPlug
            Dim row As UltraDataRow = Me.udsModulesOnPlug.Rows.Add
            row.SetCellValue(ModulesOnCavityFormStrings.ModId_ColCaption, [module].SystemId)
            row.SetCellValue(ModulesOnCavityFormStrings.PartNumber_ColCaption, [module].Part_number)
            row.SetCellValue(ModulesOnCavityFormStrings.Abbreviation_ColCaption, [module].Abbreviation)
            row.SetCellValue(ModulesOnCavityFormStrings.Desc_ColCaption, [module].Description)

            If ([module].Of_family IsNot Nothing) Then row.SetCellValue(ModulesOnCavityFormStrings.ModFam_ColCaption, DirectCast(_kblMapper.KBLOccurrenceMapper([module].Of_family), Module_family).Id)
        Next
    End Sub

    Private Sub InitializeGridLayout(layout As UltraGridLayout)
        With layout
            .AutoFitStyle = AutoFitStyle.ResizeAllColumns
            .CaptionVisible = Infragistics.Win.DefaultableBoolean.False
            .GroupByBox.Hidden = True

            With .Override
                .AllowColMoving = AllowColMoving.NotAllowed
                .AllowDelete = Infragistics.Win.DefaultableBoolean.False
                .AllowUpdate = Infragistics.Win.DefaultableBoolean.False
                .ButtonStyle = Infragistics.Win.UIElementButtonStyle.Button3D
                .CellClickAction = CellClickAction.RowSelect
                .RowSelectors = Infragistics.Win.DefaultableBoolean.False
                .SelectTypeRow = SelectType.Single
            End With

            For Each band As UltraGridBand In .Bands
                With band
                    For Each column As UltraGridColumn In .Columns
                        If Not column.Hidden Then
                            With column
                                If column.Key = NameOf(ModulesOnCavityFormStrings.ModId_ColCaption) Then
                                    .Hidden = True
                                End If

                                .CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center
                                .CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle
                                .Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center
                                .Header.Appearance.TextVAlign = Infragistics.Win.VAlign.Middle

                                If column.Index = 1 Then
                                    .SortIndicator = SortIndicator.Ascending
                                End If
                            End With
                        End If
                    Next
                End With
            Next
        End With
    End Sub


    Private Sub ModulesOnCavityForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If (e.KeyCode = Keys.Escape) Then
            Me.Close()
        End If
    End Sub

    Private Sub ModulesOnCavityForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        If (Me.udsModulesOnTerminal.Rows.Count = 0) Then
            Me.upnTerminal.Visible = False
        Else
            Me.ugModulesOnTerminal.DataSource = Me.udsModulesOnTerminal
        End If

        If (Me.udsModulesOnSeal.Rows.Count = 0) Then
            Me.upnSeal.Visible = False
        Else
            Me.ugModulesOnSeal.DataSource = Me.udsModulesOnSeal
        End If

        If (Me.udsModulesOnPlug.Rows.Count = 0) Then
            Me.upnPlug.Visible = False
        Else
            Me.ugModulesOnPlug.DataSource = Me.udsModulesOnPlug
        End If

        If (Not Me.upnPlug.Visible) Then
            If (Me.upnSeal.Visible) Then
                Me.MinimumSize = New Size(500, 435)

                Me.upnSeal.Dock = DockStyle.Fill
                Me.upnTerminal.Size = New Size(Me.upnTerminal.Size.Width, CInt(Me.upnTerminal.Size.Height * 1.5))
            ElseIf (Me.upnTerminal.Visible) Then
                Me.upnTerminal.Dock = DockStyle.Fill
            End If
        Else
            If (Me.upnSeal.Visible) AndAlso (Me.upnTerminal.Visible) Then
                Me.MinimumSize = New Size(500, 522)
            End If
        End If
    End Sub

    Private Sub btnApply_Click(sender As Object, e As EventArgs) Handles btnApply.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Abort
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub ugModulesOnPlug_InitializeLayout(sender As Object, e As Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs) Handles ugModulesOnPlug.InitializeLayout
        Me.ugModulesOnPlug.BeginUpdate()

        InitializeGridLayout(e.Layout)

        Me.ugModulesOnPlug.EndUpdate()
    End Sub

    Private Sub ugModulesOnSeal_InitializeLayout(sender As Object, e As Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs) Handles ugModulesOnSeal.InitializeLayout
        Me.ugModulesOnSeal.BeginUpdate()

        InitializeGridLayout(e.Layout)

        Me.ugModulesOnSeal.EndUpdate()
    End Sub

    Private Sub ugModulesOnTerminal_InitializeLayout(sender As Object, e As Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs) Handles ugModulesOnTerminal.InitializeLayout
        Me.ugModulesOnTerminal.BeginUpdate()

        InitializeGridLayout(e.Layout)

        Me.ugModulesOnTerminal.EndUpdate()
    End Sub

End Class