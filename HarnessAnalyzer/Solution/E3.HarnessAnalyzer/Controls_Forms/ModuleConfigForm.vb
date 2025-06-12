Imports System.Text.RegularExpressions
Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinGrid
Imports Infragistics.Win.UltraWinToolbars
Imports Zuken.E3.HarnessAnalyzer.Shared.Common

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class ModuleConfigForm

    Friend Event LogMessage(sender As ModuleConfigForm, e As LogEventArgs)

    Private _clickedColumn As UltraGridColumn
    Private _clipboardModulePartNumberRegEx As String
    Private _contextMenu As PopupMenuTool
    Private _contextMenuExport As PopupMenuTool
    Private _harnessModuleConfigurations As HarnessModuleConfigurationCollection
    Private _isDirty As Boolean
    Private _kblMapper As KblMapper
    Private _systemChange As Boolean

    Private Enum ContextMenuToolKey
        ActivateModConfig
        AddNewModConfig
        CopyModConfig
        DeleteModConfig
        DeselectAllModules
        ExportActive
        ExportAll
        ExportUserDefined
        PasteFromClipboard
        RenameModConfig
        SelectAllModules
        ShowHideModConfig
    End Enum

    Public Sub New(clipboardModulePartNumberRegEx As String, harnessModuleConfigurations As HarnessModuleConfigurationCollection, kblMapper As KblMapper)
        InitializeComponent()

        Me.BackColor = Color.White
        Me.Icon = My.Resources.ModuleConfigManager

        _clipboardModulePartNumberRegEx = clipboardModulePartNumberRegEx
        _contextMenu = New PopupMenuTool("ModuleConfigContextMenu")
        _contextMenuExport = New PopupMenuTool("ExportContextMenu")
        _harnessModuleConfigurations = harnessModuleConfigurations
        _kblMapper = kblMapper
    End Sub

    Private Sub AddOrCopyHarnessModuleConfiguration(harnessModuleConfigName As String, Optional harnessModuleConfigForCopy As HarnessModuleConfiguration = Nothing, Optional harnessConfig As Harness_configuration = Nothing)
        Dim harnessModuleConfiguration As New HarnessModuleConfiguration

        If (harnessModuleConfigName = ModuleConfigFormStrings.CustomCopy_ModConfigName) Then
            harnessModuleConfigName = ModuleConfigFormStrings.CustomCopy2_ModConfigName
        End If

        If (harnessConfig IsNot Nothing) Then
            With harnessModuleConfiguration
                .ConfigurationType = [Enum].Parse(Of HarnessModuleConfigurationType)(harnessConfig.Description)
                .HarnessConfiguration = harnessConfig
            End With
        Else
            harnessConfig = New Harness_configuration
            With harnessConfig
                .SystemId = Guid.NewGuid.ToString

                .Description = HarnessModuleConfigurationType.UserDefined.ToString
                .Part_number = harnessModuleConfigName
                .Version = "1"

                If (harnessModuleConfigForCopy IsNot Nothing) Then
                    .Modules = harnessModuleConfigForCopy.HarnessConfiguration.Modules
                Else
                    .Modules = String.Empty
                End If
            End With

            With harnessModuleConfiguration
                .ConfigurationType = HarnessModuleConfigurationType.UserDefined
                .HarnessConfiguration = harnessConfig
            End With
        End If

        Try
            Me.udsModuleConfig.Band.Columns.Add(harnessModuleConfigName)
        Catch ex As Exception
            Dim copyCount As Integer = 0
            Dim copyRepeatCount As Integer = 0

            For Each column As UltraDataColumn In Me.udsModuleConfig.Band.Columns

                If (column.Key.EndsWith(ModuleConfigFormStrings.Copy_String)) Then
                    copyCount += 1
                End If

                If (column.Key.Contains(ModuleConfigFormStrings.Copy_String) AndAlso IsNumeric(column.Key.Split("_".ToCharArray, StringSplitOptions.RemoveEmptyEntries)(column.Key.SplitRemoveEmpty("_"c).Length - 1))) Then
                    copyRepeatCount = Math.Max(copyRepeatCount, CInt(column.Key.Split("_".ToCharArray, StringSplitOptions.RemoveEmptyEntries)(column.Key.SplitRemoveEmpty("_"c).Length - 1)) + 1)
                End If
            Next

            harnessModuleConfigName = String.Format("{0}_{1}", harnessModuleConfigName, Math.Max(copyCount, copyRepeatCount))

            Me.udsModuleConfig.Band.Columns.Add(harnessModuleConfigName)
        End Try

        harnessModuleConfiguration.HarnessConfiguration.Part_number = harnessModuleConfigName

        Dim moduleIds As List(Of String) = harnessConfig.Modules.SplitSpace.ToList

        For Each row As UltraDataRow In Me.udsModuleConfig.Rows
            row.SetCellValue(harnessModuleConfigName, moduleIds.Contains(row.GetCellValue(ModuleConfigFormStrings.Id_ColCaption).ToString))
        Next

        Me.uceActiveConfig.Items.Add(harnessModuleConfiguration, harnessModuleConfigName)

        Me.ugModuleConfig.BeginUpdate()

        With Me.ugModuleConfig.DisplayLayout.Bands(0).Columns(harnessModuleConfigName)
            .CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center
            .CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle
            .Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center
            .Header.Appearance.TextVAlign = Infragistics.Win.VAlign.Middle
            .Header.ToolTipText = ModuleConfigFormStrings.UserDefModConifg_Tooltip
            .Style = ColumnStyle.CheckBox
            .Tag = harnessModuleConfiguration
        End With

        Me.ugModuleConfig.ActiveColScrollRegion.ScrollColIntoView(Me.ugModuleConfig.DisplayLayout.Bands(0).Columns(harnessModuleConfigName))
        Me.ugModuleConfig.EndUpdate()

        _isDirty = True
    End Sub

    Private Sub ExportConfigurations(exportType As String)
        Using sfdModuleConfig As New SaveFileDialog
            With sfdModuleConfig
                .DefaultExt = "hmcfg"

                If _kblMapper.GetChanges.Any Then
                    .FileName = String.Format("{0}{1}{2}_{3}_{4}_Module_Config.hmcfg", Now.Year, Format(Now.Month, "00"), Format(Now.Day, "00"), Replace(_kblMapper.HarnessPartNumber, " ", String.Empty), _kblMapper.GetChanges.Max(Function(change) change.Id))
                Else
                    .FileName = String.Format("{0}{1}{2}_{3}_Module_Config.hmcfg", Now.Year, Format(Now.Month, "00"), Format(Now.Day, "00"), Replace(_kblMapper.HarnessPartNumber, " ", String.Empty))
                End If

                .Filter = "Harness module configuration files (*.hmcfg)|*.hmcfg"
                .Title = ModuleConfigFormStrings.ExportConfigToFile_Title

                If (sfdModuleConfig.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then
                    Try
                        Dim kblContainer As New KBL_container
                        With kblContainer
                            .SystemId = CType(_kblMapper, IKblBaseObject).SystemId
                            .version_id = _kblMapper.Version_Id

                            .Harness = New Harness

                            With .Harness
                                .SystemId = _kblMapper.GetHarness.SystemId
                                .Part_number = _kblMapper.HarnessPartNumber
                                .Company_name = _kblMapper.HarnessCompanyName
                                .Version = _kblMapper.HarnessVersion
                                .Abbreviation = _kblMapper.HarnessAbbreviation
                                .Description = _kblMapper.HarnessDescription
                                .Car_classification_level_2 = _kblMapper.HarnessCar_classification_level_2
                                .Model_year = _kblMapper.HarnessModel_year
                                .Content = _kblMapper.HarnessContent

                                .Harness_configuration = Array.Empty(Of Harness_configuration)

                                For Each column As UltraGridColumn In Me.ugModuleConfig.DisplayLayout.Bands(0).Columns
                                    If (column.Tag IsNot Nothing) Then
                                        With DirectCast(column.Tag, HarnessModuleConfiguration)
                                            If (.ConfigurationType = HarnessModuleConfigurationType.Custom) OrElse (exportType = ContextMenuToolKey.ExportAll.ToString) OrElse (exportType = ContextMenuToolKey.ExportActive.ToString AndAlso .IsActive) OrElse (exportType = ContextMenuToolKey.ExportUserDefined.ToString AndAlso .ConfigurationType = HarnessModuleConfigurationType.UserDefined) Then
                                                kblContainer.Harness.Harness_configuration.Add(.HarnessConfiguration)

                                                If (kblContainer.Harness.Harness_configuration.Last.Part_number = String.Empty) AndAlso (Regex.Replace(kblContainer.Harness.Harness_configuration.Last.Abbreviation, "\s", String.Empty) <> String.Empty) Then
                                                    kblContainer.Harness.Harness_configuration.Last.Part_number = kblContainer.Harness.Harness_configuration.Last.Abbreviation
                                                ElseIf (kblContainer.Harness.Harness_configuration.Last.Part_number = String.Empty) AndAlso (Regex.Replace(kblContainer.Harness.Harness_configuration.Last.Description, "\s", String.Empty) <> String.Empty) Then
                                                    kblContainer.Harness.Harness_configuration.Last.Part_number = kblContainer.Harness.Harness_configuration.Last.Description
                                                End If

                                                If (.ConfigurationType = HarnessModuleConfigurationType.FromKBL) OrElse (.ConfigurationType = HarnessModuleConfigurationType.UserDefined) Then
                                                    kblContainer.Harness.Harness_configuration.Last.Description = HarnessModuleConfigurationType.UserDefined.ToString
                                                Else
                                                    kblContainer.Harness.Harness_configuration.Last.Description = HarnessModuleConfigurationType.Custom.ToString
                                                End If
                                            End If
                                        End With
                                    End If
                                Next

                                .Module = Array.Empty(Of [Lib].Schema.Kbl.[Module])

                                For Each row As UltraGridRow In Me.ugModuleConfig.Rows
                                    If (row.Tag IsNot Nothing) Then
                                        Dim [module] As New [Lib].Schema.Kbl.[Module]

                                        With DirectCast(row.Tag, [Module])
                                            [module].SystemId = .SystemId
                                            [module].Part_number = .Part_number
                                            [module].Company_name = .Company_name
                                            [module].Version = .Version
                                            [module].Abbreviation = .Abbreviation
                                            [module].Description = .Description
                                            [module].Car_classification_level_2 = .Car_classification_level_2
                                            [module].Model_year = .Model_year
                                            [module].Content = .Content
                                        End With

                                        .Module.Add([module])
                                    End If
                                Next
                            End With
                        End With

                        KBL_container.SaveTo(.FileName, kblContainer)

                        MessageBox.Show(ModuleConfigFormStrings.ExportConfigSuccess_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Catch ex As Exception
                        ex.ShowMessageBox(String.Format(ModuleConfigFormStrings.ErrorExportConfig_Msg, vbCrLf, ex.Message))
                    End Try
                End If
            End With
        End Using
    End Sub

    Private Sub InitializeContextMenus()
        With _contextMenu
            .DropDownArrowStyle = DropDownArrowStyle.None

            Dim activateButton As New ButtonTool(ContextMenuToolKey.ActivateModConfig.ToString)
            activateButton.SharedProps.Caption = ModuleConfigFormStrings.Activate_CtxtMnu_Caption
            activateButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.ActivateModConfig.ToBitmap

            Dim addNewButton As New ButtonTool(ContextMenuToolKey.AddNewModConfig.ToString)
            addNewButton.SharedProps.Caption = ModuleConfigFormStrings.AddNew_CtxtMnu_Caption
            addNewButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.AddNewModConfig.ToBitmap

            Dim copyButton As New ButtonTool(ContextMenuToolKey.CopyModConfig.ToString)
            copyButton.SharedProps.Caption = ModuleConfigFormStrings.Copy_CtxtMnu_Caption
            copyButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.CopyModConfig.ToBitmap

            Dim deleteButton As New ButtonTool(ContextMenuToolKey.DeleteModConfig.ToString)
            deleteButton.SharedProps.Caption = ModuleConfigFormStrings.Delete_CtxtMnu_Caption
            deleteButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.DeleteModConfig.ToBitmap

            Dim deselectAllButton As New ButtonTool(ContextMenuToolKey.DeselectAllModules.ToString)
            deselectAllButton.SharedProps.Caption = ModuleConfigFormStrings.DeselectAll_CtxtMnu_Caption
            deselectAllButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.UncheckAll.ToBitmap

            Dim pasteFromClipboardButton As New ButtonTool(ContextMenuToolKey.PasteFromClipboard.ToString)
            pasteFromClipboardButton.SharedProps.Caption = ModuleConfigFormStrings.Paste_CtxtMnu_Caption
            pasteFromClipboardButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.PasteFromClipboard.ToBitmap

            Dim renameButton As New ButtonTool(ContextMenuToolKey.RenameModConfig.ToString)
            renameButton.SharedProps.Caption = ModuleConfigFormStrings.Rename_CtxtMnu_Caption
            renameButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.RenameModConfig.ToBitmap

            Dim selectAllButton As New ButtonTool(ContextMenuToolKey.SelectAllModules.ToString)
            selectAllButton.SharedProps.Caption = ModuleConfigFormStrings.SelectAll_CtxtMnu_Caption
            selectAllButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.CheckAll.ToBitmap

            Dim showHideModConfigButton As New ButtonTool(ContextMenuToolKey.ShowHideModConfig.ToString)
            showHideModConfigButton.SharedProps.Caption = ModuleConfigFormStrings.ToggleVisibility_CtxtMnu_Caption
            showHideModConfigButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.ShowHideModConfig.ToBitmap

            Me.utmModuleConfig.Tools.AddRange(New ToolBase() {_contextMenu, addNewButton, activateButton, copyButton, deleteButton, deselectAllButton, pasteFromClipboardButton, renameButton, selectAllButton, showHideModConfigButton})

            .Tools.AddTool(addNewButton.Key)
            .Tools.AddTool(activateButton.Key)
            .Tools.AddTool(copyButton.Key)
            .Tools.AddTool(renameButton.Key)
            .Tools.AddTool(deleteButton.Key)
            .Tools.AddTool(pasteFromClipboardButton.Key)
            .Tools.AddTool(selectAllButton.Key)
            .Tools.AddTool(deselectAllButton.Key)
            .Tools.AddTool(showHideModConfigButton.Key)

            .Tools(pasteFromClipboardButton.Key).InstanceProps.IsFirstInGroup = True
            .Tools(selectAllButton.Key).InstanceProps.IsFirstInGroup = True
            .Tools(showHideModConfigButton.Key).InstanceProps.IsFirstInGroup = True
        End With

        With _contextMenuExport
            .DropDownArrowStyle = DropDownArrowStyle.None

            Dim exportActiveButton As New ButtonTool(ContextMenuToolKey.ExportActive.ToString)
            exportActiveButton.SharedProps.Caption = ModuleConfigFormStrings.ActiveConfig_CtxtMnu_Caption

            Dim exportAllButton As New ButtonTool(ContextMenuToolKey.ExportAll.ToString)
            exportAllButton.SharedProps.Caption = ModuleConfigFormStrings.AllConfig_CtxtMnu_Caption

            Dim exportUserDefinedButton As New ButtonTool(ContextMenuToolKey.ExportUserDefined.ToString)
            exportUserDefinedButton.SharedProps.Caption = ModuleConfigFormStrings.UserConfig_CtxtMnu_Caption

            Me.utmModuleConfig.Tools.AddRange(New ToolBase() {_contextMenuExport, exportActiveButton, exportAllButton, exportUserDefinedButton})

            .Tools.AddTool(exportAllButton.Key)
            .Tools.AddTool(exportActiveButton.Key)
            .Tools.AddTool(exportUserDefinedButton.Key)
        End With

        Me.uddbEditActiveConfig.PopupItem = _contextMenu
        Me.uddbExport.PopupItem = _contextMenuExport
    End Sub

    Private Sub InitializeGridDataSource()
        With Me.udsModuleConfig
            With .Band
                .Key = "ModuleConfigData"

                .Columns.Add(ModuleConfigFormStrings.Id_ColCaption)
                .Columns.Add(ModuleConfigFormStrings.PartNumber_ColCaption)
                .Columns.Add(ModuleConfigFormStrings.Abbreviation_ColCaption)
                .Columns.Add(ModuleConfigFormStrings.Description_ColCaption)
                .Columns.Add(ModuleConfigFormStrings.OfFamily_ColCaption)

                If (_harnessModuleConfigurations.Where(Function(harnessModuleConfig) harnessModuleConfig.ConfigurationType = HarnessModuleConfigurationType.Custom).Any()) Then
                    .Columns.Add(KblObjectType.Custom.ToLocalizedString).Tag = _harnessModuleConfigurations.Where(Function(harnessModuleConfig) harnessModuleConfig.ConfigurationType = HarnessModuleConfigurationType.Custom).FirstOrDefault
                End If

                For Each harnessModuleConfig As HarnessModuleConfiguration In _harnessModuleConfigurations
                    If (harnessModuleConfig.ConfigurationType = HarnessModuleConfigurationType.FromKBL) Then
                        If (Regex.Replace(harnessModuleConfig.HarnessConfiguration.Part_number, "\s", String.Empty) <> String.Empty) AndAlso (Not .Columns.Exists(harnessModuleConfig.HarnessConfiguration.Part_number)) Then
                            .Columns.Add(harnessModuleConfig.HarnessConfiguration.Part_number).Tag = harnessModuleConfig
                        ElseIf (Regex.Replace(harnessModuleConfig.HarnessConfiguration.Abbreviation, "\s", String.Empty) <> String.Empty) AndAlso (Not .Columns.Exists(harnessModuleConfig.HarnessConfiguration.Abbreviation)) Then
                            .Columns.Add(harnessModuleConfig.HarnessConfiguration.Abbreviation).Tag = harnessModuleConfig
                        ElseIf (Regex.Replace(harnessModuleConfig.HarnessConfiguration.Description, "\s", String.Empty) <> String.Empty) AndAlso (Not .Columns.Exists(harnessModuleConfig.HarnessConfiguration.Description)) Then
                            .Columns.Add(harnessModuleConfig.HarnessConfiguration.Description).Tag = harnessModuleConfig
                        End If
                    End If
                Next

                For Each harnessModuleConfig As HarnessModuleConfiguration In _harnessModuleConfigurations
                    If (harnessModuleConfig.ConfigurationType = HarnessModuleConfigurationType.UserDefined) Then
                        .Columns.Add(harnessModuleConfig.HarnessConfiguration.Part_number).Tag = harnessModuleConfig
                    End If
                Next
            End With

            For Each [module] As [Module] In _kblMapper.GetModules
                With [module]
                    Dim row As UltraDataRow = Me.udsModuleConfig.Rows.Add
                    row.SetCellValue(ModuleConfigFormStrings.Id_ColCaption, .SystemId)
                    row.SetCellValue(ModuleConfigFormStrings.PartNumber_ColCaption, .Part_number)
                    row.SetCellValue(ModuleConfigFormStrings.Abbreviation_ColCaption, .Abbreviation)
                    row.SetCellValue(ModuleConfigFormStrings.Description_ColCaption, .Description)

                    Dim moduleFamily As String = String.Empty

                    If ([module].Of_family IsNot Nothing) AndAlso (_kblMapper.KBLOccurrenceMapper.ContainsKey([module].Of_family)) Then moduleFamily = DirectCast(_kblMapper.KBLOccurrenceMapper([module].Of_family), Module_family).Id

                    row.SetCellValue(ModuleConfigFormStrings.OfFamily_ColCaption, moduleFamily)
                    row.Tag = [module]
                End With
            Next

            For Each column As UltraDataColumn In Me.udsModuleConfig.Band.Columns
                If (column.Tag IsNot Nothing) Then
                    Dim moduleIds As List(Of String) = DirectCast(column.Tag, HarnessModuleConfiguration).HarnessConfiguration.Modules.SplitSpace.ToList

                    For Each row As UltraDataRow In Me.udsModuleConfig.Rows
                        row.SetCellValue(column, moduleIds.Contains(row(ModuleConfigFormStrings.Id_ColCaption).ToString))
                    Next
                End If
            Next
        End With
    End Sub

    Private Sub InitializeGridLayout(layout As UltraGridLayout)
        With layout
            .CaptionVisible = Infragistics.Win.DefaultableBoolean.False
            .GroupByBox.Hidden = True

            With .Override
                .AllowAddNew = AllowAddNew.No
                .AllowColMoving = AllowColMoving.NotAllowed
                .AllowDelete = Infragistics.Win.DefaultableBoolean.False
                .FixedCellSeparatorColor = Color.Red
                .FixedHeaderIndicator = FixedHeaderIndicator.None
                .RowSelectors = Infragistics.Win.DefaultableBoolean.False
                .SelectTypeCell = SelectType.Single
                .SelectTypeRow = SelectType.Single
            End With

            .UseFixedHeaders = True

            For Each band As UltraGridBand In .Bands
                With band
                    For Each column As UltraGridColumn In .Columns
                        If (Not column.Hidden) Then
                            With column
                                .CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center
                                .CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle
                                .Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center
                                .Header.Appearance.TextVAlign = Infragistics.Win.VAlign.Middle
                                .MinWidth = 75

                                If (.Index = 0) Then
                                    .Hidden = True
                                End If

                                If (.Index = 1) Then
                                    .SortIndicator = SortIndicator.Ascending
                                End If

                                If (.Index = 2) Then
                                    .MinWidth = 120
                                End If

                                If (.Index = 3) Then
                                    .Width = 400
                                End If

                                If (.Index > 4) Then
                                    .Style = ColumnStyle.CheckBox
                                Else
                                    .CellActivation = Activation.NoEdit
                                    .Header.Fixed = True
                                End If
                            End With
                        End If
                    Next
                End With
            Next
        End With
    End Sub


    Private Sub ModuleConfigForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If (e.KeyCode = Keys.Escape) Then
            Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        End If
    End Sub

    Private Sub ModuleConfigForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        _systemChange = True

        InitializeContextMenus()

        Dim activeItem As Infragistics.Win.ValueListItem = Nothing

        For Each harnessModuleConfig As HarnessModuleConfiguration In _harnessModuleConfigurations
            If Not harnessModuleConfig.HarnessConfiguration.Part_number.RegExEmptyReplace("\s").IsNullOrEmpty Then
                Me.uceActiveConfig.Items.Add(harnessModuleConfig, If(harnessModuleConfig.HarnessConfiguration.Abbreviation IsNot Nothing AndAlso harnessModuleConfig.HarnessConfiguration.Abbreviation <> String.Empty, String.Format("{0} [{1}]", harnessModuleConfig.HarnessConfiguration.Abbreviation, harnessModuleConfig.HarnessConfiguration.Part_number), harnessModuleConfig.HarnessConfiguration.Part_number))
                If (harnessModuleConfig.IsActive) Then
                    activeItem = Me.uceActiveConfig.Items(Me.uceActiveConfig.Items.Count - 1)
                End If
            ElseIf Not harnessModuleConfig.HarnessConfiguration.Abbreviation.RegExEmptyReplace("\s").IsNullOrEmpty Then
                Me.uceActiveConfig.Items.Add(harnessModuleConfig, harnessModuleConfig.HarnessConfiguration.Abbreviation)
                If (harnessModuleConfig.IsActive) Then
                    activeItem = Me.uceActiveConfig.Items(Me.uceActiveConfig.Items.Count - 1)
                End If
            ElseIf Not harnessModuleConfig.HarnessConfiguration.Description.RegExEmptyReplace("\s").IsNullOrEmpty Then
                Me.uceActiveConfig.Items.Add(harnessModuleConfig, harnessModuleConfig.HarnessConfiguration.Description)
                If (harnessModuleConfig.IsActive) Then
                    activeItem = Me.uceActiveConfig.Items(Me.uceActiveConfig.Items.Count - 1)
                End If
            End If
        Next

        Me.uceActiveConfig.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending
        Me.uceActiveConfig.SelectedItem = activeItem

        InitializeGridDataSource()

        Me.ugModuleConfig.SyncWithCurrencyManager = False
        Me.ugModuleConfig.DataSource = Me.udsModuleConfig

        _systemChange = False
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
    End Sub

    Private Sub btnImport_Click(sender As Object, e As EventArgs) Handles btnImport.Click
        Using ofdModuleConfig As New OpenFileDialog
            With ofdModuleConfig
                .DefaultExt = "hmcfg"
                .FileName = String.Empty
                .Filter = "Harness module configuration files (*.hmcfg)|*.hmcfg"
                .Title = ModuleConfigFormStrings.ImportConfigFile_Title

                If .ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then
                    Try
                        Dim kblContainer_imported As KBL_container = KBL_container.Read(.FileName)

                        If (kblContainer_imported.Harness.Part_number = _kblMapper.HarnessPartNumber AndAlso kblContainer_imported.Harness.Version = _kblMapper.GetHarness.Version) OrElse (MessageBoxEx.ShowQuestion(ModuleConfigFormStrings.ImportConfig_Msg) = System.Windows.Forms.DialogResult.Yes) Then
                            Dim overrideExistingConfigurations As Boolean = False
                            Dim dialogResult As DialogResult = MessageBoxEx.ShowQuestion(ModuleConfigFormStrings.OverrideNames_Msg, MessageBoxButtons.YesNoCancel)

                            If (dialogResult = System.Windows.Forms.DialogResult.Yes) Then
                                overrideExistingConfigurations = True
                            ElseIf (dialogResult = System.Windows.Forms.DialogResult.Cancel) Then
                                Return
                            End If

                            RaiseEvent LogMessage(Me, New LogEventArgs(LogEventArgs.LoggingLevel.Information, String.Format(ModuleConfigFormStrings.LoadConfigFile_LogMsg, .FileName, DateTime.Now, vbCrLf)))

                            For Each harnessConfig_imported As Harness_configuration In kblContainer_imported.Harness.Harness_configuration.OrEmpty
                                Dim moduleIds_imported As List(Of String) = harnessConfig_imported.Modules?.Trim.SplitSpace.ToList
                                Dim modulePartNumbers_imported As New List(Of String)

                                For Each moduleId As String In moduleIds_imported
                                    modulePartNumbers_imported.Add(kblContainer_imported.Harness.Module.Single(Function([module]) [module].SystemId = moduleId).Part_number)
                                Next

                                harnessConfig_imported.Modules = String.Empty

                                For Each modulePartNumber As String In modulePartNumbers_imported
                                    Dim m_pn As [Module] = Nothing
                                    If _kblMapper.TryGetModuleByPartNr(modulePartNumber, m_pn) Then
                                        If harnessConfig_imported.Modules.IsNullOrEmpty Then
                                            harnessConfig_imported.Modules = _kblMapper.GetModuleByPartNr(modulePartNumber)?.SystemId
                                        Else
                                            harnessConfig_imported.Modules = String.Format("{0} {1}", harnessConfig_imported.Modules, _kblMapper.GetModuleByPartNr(modulePartNumber).SystemId)
                                        End If
                                    Else
                                        RaiseEvent LogMessage(Me, New LogEventArgs(LogEventArgs.LoggingLevel.Warning, String.Format(ModuleConfigFormStrings.ModuleNotExist_LogMsg, modulePartNumber, vbCrLf)))
                                    End If
                                Next

                                If Me.ugModuleConfig.DisplayLayout.Bands(0).Columns.Exists(harnessConfig_imported.Part_number) Then
                                    If (overrideExistingConfigurations) AndAlso (DirectCast(Me.ugModuleConfig.DisplayLayout.Bands(0).Columns(harnessConfig_imported.Part_number).Tag, HarnessModuleConfiguration).ConfigurationType <> HarnessModuleConfigurationType.FromKBL) Then
                                        DirectCast(Me.ugModuleConfig.DisplayLayout.Bands(0).Columns(harnessConfig_imported.Part_number).Tag, HarnessModuleConfiguration).HarnessConfiguration = harnessConfig_imported

                                        For Each row As UltraDataRow In Me.udsModuleConfig.Rows
                                            row.SetCellValue(harnessConfig_imported.Part_number, modulePartNumbers_imported.Contains(row.GetCellValue(ModuleConfigFormStrings.PartNumber_ColCaption).ToString))
                                        Next
                                    ElseIf (overrideExistingConfigurations) Then
                                        AddOrCopyHarnessModuleConfiguration(String.Format(ModuleConfigFormStrings.Copy2_String, harnessConfig_imported.Part_number), Nothing, harnessConfig_imported)
                                    End If
                                Else
                                    AddOrCopyHarnessModuleConfiguration(harnessConfig_imported.Part_number, Nothing, harnessConfig_imported)
                                End If
                            Next

                            _isDirty = True

                            MessageBox.Show(ModuleConfigFormStrings.ImportConfigSuccess_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
                        End If
                    Catch ex As Exception
#If DEBUG Or CONFIG = "Debug" Then
                        Throw
#Else
                        MessageBoxEx.ShowError(String.Format(ModuleConfigFormStrings.ErrorImportConfig_Msg, vbCrLf, ex.Message))
#End If
                    End Try
                End If
            End With
        End Using
    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        If _isDirty Then
            _harnessModuleConfigurations.Clear()

            For Each column As UltraGridColumn In Me.ugModuleConfig.DisplayLayout.Bands(0).Columns
                If (column.Tag IsNot Nothing) Then
                    _harnessModuleConfigurations.Add(DirectCast(column.Tag, HarnessModuleConfiguration))
                End If
            Next
        End If

        Me.DialogResult = System.Windows.Forms.DialogResult.OK
    End Sub

    Private Sub uceActiveConfig_SelectionChanged(sender As Object, e As EventArgs) Handles uceActiveConfig.SelectionChanged
        If (Me.uceActiveConfig.SelectedItem Is Nothing) Then
            Return
        End If

        Me.ugModuleConfig.BeginUpdate()

        For Each column As UltraGridColumn In Me.ugModuleConfig.DisplayLayout.Bands(0).Columns
            If (column.Tag IsNot Nothing) Then
                Dim harnessModuleConfig As HarnessModuleConfiguration = DirectCast(column.Tag, HarnessModuleConfiguration)

                If (harnessModuleConfig.Equals(Me.uceActiveConfig.SelectedItem.DataValue)) Then
                    column.CellAppearance.BackColor = Color.CornflowerBlue
                    harnessModuleConfig.IsActive = True

                    Me.ugModuleConfig.ActiveColScrollRegion.ScrollColIntoView(column)
                Else
                    If (harnessModuleConfig.ConfigurationType = HarnessModuleConfigurationType.FromKBL) Then
                        column.CellAppearance.BackColor = Color.LightGray
                    Else
                        column.CellAppearance.BackColor = Color.White
                    End If

                    harnessModuleConfig.IsActive = False
                End If
            End If
        Next

        If (Not _systemChange) Then
            _isDirty = True
        End If

        Me.ugModuleConfig.EndUpdate()
    End Sub

    Private Sub uddbEditActiveConfig_DroppingDown(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles uddbEditActiveConfig.DroppingDown
        For Each tool As ButtonTool In _contextMenu.Tools
            If (tool.Key = ContextMenuToolKey.ActivateModConfig.ToString) Then
                tool.SharedProps.Visible = False
            ElseIf (tool.Key = ContextMenuToolKey.PasteFromClipboard.ToString) Then
                tool.SharedProps.Visible = Clipboard.ContainsText AndAlso Clipboard.GetText <> String.Empty AndAlso Clipboard.GetText <> " "
            Else
                tool.SharedProps.Visible = True
            End If
        Next
    End Sub

    Private Sub ugModuleConfig_AfterCellUpdate(sender As Object, e As CellEventArgs) Handles ugModuleConfig.AfterCellUpdate
        If (e.Cell.Column.Tag IsNot Nothing) Then
            With DirectCast(e.Cell.Column.Tag, HarnessModuleConfiguration).HarnessConfiguration
                Dim moduleIds As List(Of String) = .Modules.SplitSpace.ToList

                If (CBool(e.Cell.Value)) AndAlso (Not moduleIds.Contains(e.Cell.Row.Cells(ModuleConfigFormStrings.Id_ColCaption).Value.ToString)) Then
                    moduleIds.Add(e.Cell.Row.Cells(ModuleConfigFormStrings.Id_ColCaption).Value.ToString)
                ElseIf (Not CBool(e.Cell.Value)) AndAlso (moduleIds.Contains(e.Cell.Row.Cells(ModuleConfigFormStrings.Id_ColCaption).Value.ToString)) Then
                    moduleIds.Remove(e.Cell.Row.Cells(ModuleConfigFormStrings.Id_ColCaption).Value.ToString)
                End If

                .Modules = String.Empty

                For Each moduleId As String In moduleIds
                    If (.Modules = String.Empty) Then
                        .Modules = moduleId
                    Else
                        .Modules = String.Format("{0} {1}", .Modules, moduleId)
                    End If
                Next
            End With

            _isDirty = True
        End If
    End Sub

    Private Sub ugModuleConfig_BeforeCellActivate(sender As Object, e As CancelableCellEventArgs) Handles ugModuleConfig.BeforeCellActivate
        If (e.Cell.Column.Tag Is Nothing) OrElse (DirectCast(e.Cell.Column.Tag, HarnessModuleConfiguration).ConfigurationType = HarnessModuleConfigurationType.FromKBL) Then e.Cancel = True
    End Sub

    Private Sub ugModuleConfig_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugModuleConfig.InitializeLayout
        Me.ugModuleConfig.BeginUpdate()

        InitializeGridLayout(e.Layout)

        If (Me.ugModuleConfig.Rows.Count <> 0) Then
            For Each column As UltraDataColumn In DirectCast(Me.ugModuleConfig.Rows(0).ListObject, UltraDataRow).Band.Columns
                If (column.Tag IsNot Nothing) Then
                    Me.ugModuleConfig.Rows.Band.Columns(column.Key).Tag = column.Tag

                    column.Tag = Nothing
                End If
            Next

            For Each column As UltraGridColumn In Me.ugModuleConfig.Rows.Band.Columns
                If (column.Tag IsNot Nothing) Then
                    Dim harnessModuleConfig As HarnessModuleConfiguration = DirectCast(column.Tag, HarnessModuleConfiguration)

                    If (Regex.Replace(harnessModuleConfig.HarnessConfiguration.Part_number, "\s", String.Empty) <> String.Empty) Then column.Header.Caption = If(harnessModuleConfig.HarnessConfiguration.Abbreviation IsNot Nothing AndAlso harnessModuleConfig.HarnessConfiguration.Abbreviation <> String.Empty, String.Format("{0} [{1}]", harnessModuleConfig.HarnessConfiguration.Abbreviation, harnessModuleConfig.HarnessConfiguration.Part_number), harnessModuleConfig.HarnessConfiguration.Part_number)

                    Select Case harnessModuleConfig.ConfigurationType
                        Case HarnessModuleConfigurationType.Custom
                            column.Header.ToolTipText = ModuleConfigFormStrings.TempDefConfig_Tooltip
                        Case HarnessModuleConfigurationType.FromKBL
                            column.CellAppearance.BackColor = Color.LightGray
                            column.Header.ToolTipText = ModuleConfigFormStrings.ReadOnlyConfig_Tooltip
                        Case HarnessModuleConfigurationType.UserDefined
                            column.Header.ToolTipText = ModuleConfigFormStrings.UserDefConfig_Tooltip
                    End Select

                    If (harnessModuleConfig.IsActive) Then column.CellAppearance.BackColor = Color.CornflowerBlue
                End If
            Next
        End If

        Me.ugModuleConfig.EndUpdate()
    End Sub

    Private Sub ugModuleConfig_InitializeRow(sender As Object, e As InitializeRowEventArgs) Handles ugModuleConfig.InitializeRow
        e.Row.Tag = DirectCast(e.Row.ListObject, UltraDataRow).Tag
    End Sub

    Private Sub ugModuleConfig_MouseClick(sender As Object, e As MouseEventArgs) Handles ugModuleConfig.MouseClick
        If (e.Button = System.Windows.Forms.MouseButtons.Right) Then
            Dim element As Infragistics.Win.UIElement = Me.ugModuleConfig.DisplayLayout.UIElement.LastElementEntered
            Dim header As Infragistics.Win.UltraWinGrid.ColumnHeader = TryCast(element.GetContext(GetType(Infragistics.Win.UltraWinGrid.ColumnHeader)), ColumnHeader)
            If (header IsNot Nothing) AndAlso (header.Column.Tag IsNot Nothing) Then
                _clickedColumn = header.Column

                _contextMenu.Tools(ContextMenuToolKey.ActivateModConfig.ToString).SharedProps.Visible = Not DirectCast(_clickedColumn.Tag, HarnessModuleConfiguration).IsActive

                Select Case DirectCast(_clickedColumn.Tag, HarnessModuleConfiguration).ConfigurationType
                    Case HarnessModuleConfigurationType.Custom
                        _contextMenu.Tools(ContextMenuToolKey.DeleteModConfig.ToString).SharedProps.Visible = False
                        _contextMenu.Tools(ContextMenuToolKey.DeselectAllModules.ToString).SharedProps.Visible = True
                        _contextMenu.Tools(ContextMenuToolKey.PasteFromClipboard.ToString).SharedProps.Visible = Clipboard.ContainsText AndAlso Clipboard.GetText <> String.Empty AndAlso Clipboard.GetText <> " "
                        _contextMenu.Tools(ContextMenuToolKey.RenameModConfig.ToString).SharedProps.Visible = False
                        _contextMenu.Tools(ContextMenuToolKey.SelectAllModules.ToString).SharedProps.Visible = True
                    Case HarnessModuleConfigurationType.FromKBL
                        _contextMenu.Tools(ContextMenuToolKey.DeleteModConfig.ToString).SharedProps.Visible = False
                        _contextMenu.Tools(ContextMenuToolKey.DeselectAllModules.ToString).SharedProps.Visible = False
                        _contextMenu.Tools(ContextMenuToolKey.PasteFromClipboard.ToString).SharedProps.Visible = False
                        _contextMenu.Tools(ContextMenuToolKey.RenameModConfig.ToString).SharedProps.Visible = False
                        _contextMenu.Tools(ContextMenuToolKey.SelectAllModules.ToString).SharedProps.Visible = False
                    Case HarnessModuleConfigurationType.UserDefined
                        _contextMenu.Tools(ContextMenuToolKey.DeleteModConfig.ToString).SharedProps.Visible = True
                        _contextMenu.Tools(ContextMenuToolKey.DeselectAllModules.ToString).SharedProps.Visible = True
                        _contextMenu.Tools(ContextMenuToolKey.PasteFromClipboard.ToString).SharedProps.Visible = Clipboard.ContainsText AndAlso Clipboard.GetText <> String.Empty AndAlso Clipboard.GetText <> " "
                        _contextMenu.Tools(ContextMenuToolKey.RenameModConfig.ToString).SharedProps.Visible = True
                        _contextMenu.Tools(ContextMenuToolKey.SelectAllModules.ToString).SharedProps.Visible = True
                End Select

                _contextMenu.ShowPopup()
            End If
        End If
    End Sub

    Private Sub utmModuleConfig_ToolClick(sender As Object, e As ToolClickEventArgs) Handles utmModuleConfig.ToolClick
        Select Case e.Tool.Key
            Case ContextMenuToolKey.ActivateModConfig.ToString
                If (_clickedColumn IsNot Nothing) Then
                    For Each item As Infragistics.Win.ValueListItem In Me.uceActiveConfig.Items
                        If (item.DataValue.Equals(_clickedColumn.Tag)) Then
                            Me.uceActiveConfig.SelectedItem = item

                            Exit For
                        End If
                    Next
                End If
            Case ContextMenuToolKey.AddNewModConfig.ToString
                Using inputForm As New InputForm(ModuleConfigFormStrings.ChooseConfigName_InputBoxPrompt, ModuleConfigFormStrings.AddNewConfigName_InputBoxTitle, ModuleConfigFormStrings.NewConfig_InputBoxDefResp)
                    If (inputForm.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then
                        Dim harnessModuleConfigName As String = inputForm.Response.Trim

                        If harnessModuleConfigName.IsNullOrEmpty Then
                            MessageBoxEx.ShowWarning(ModuleConfigFormStrings.SelConfigNameIsEmpty_Msg)
                        ElseIf (harnessModuleConfigName.Length > 50) Then
                            MessageBoxEx.ShowWarning(ModuleConfigFormStrings.SelConfigNameTooLong_Msg)
                        ElseIf (Me.udsModuleConfig.Band.Columns.Exists(harnessModuleConfigName)) Then
                            MessageBoxEx.ShowWarning(ModuleConfigFormStrings.SelConfigNameExists_Msg)
                        Else
                            AddOrCopyHarnessModuleConfiguration(harnessModuleConfigName)
                        End If
                    End If
                End Using
            Case ContextMenuToolKey.CopyModConfig.ToString
                If (_clickedColumn IsNot Nothing) Then
                    AddOrCopyHarnessModuleConfiguration(String.Format(ModuleConfigFormStrings.Copy2_String, _clickedColumn.Key), DirectCast(_clickedColumn.Tag, HarnessModuleConfiguration))
                Else
                    AddOrCopyHarnessModuleConfiguration(String.Format(ModuleConfigFormStrings.Copy2_String, DirectCast(Me.uceActiveConfig.SelectedItem.DataValue, HarnessModuleConfiguration).HarnessConfiguration.Part_number), DirectCast(Me.uceActiveConfig.SelectedItem.DataValue, HarnessModuleConfiguration))
                End If
            Case ContextMenuToolKey.DeleteModConfig.ToString
                If (_clickedColumn IsNot Nothing) AndAlso (_clickedColumn.Key <> KblObjectType.Custom.ToLocalizedString) Then
                    If (DirectCast(_clickedColumn.Tag, HarnessModuleConfiguration).ConfigurationType = HarnessModuleConfigurationType.FromKBL) Then
                        MessageBoxEx.ShowWarning(ModuleConfigFormStrings.ConfigCannotBeDel_Msg)
                    ElseIf (MessageBoxEx.ShowQuestion(ModuleConfigFormStrings.DelSelConfig_Msg) = System.Windows.Forms.DialogResult.Yes) Then
                        Me.udsModuleConfig.Band.Columns.Remove(_clickedColumn.Key)

                        For Each item As Infragistics.Win.ValueListItem In Me.uceActiveConfig.Items
                            If (item.DisplayText = _clickedColumn.Key) Then
                                Me.uceActiveConfig.Items.Remove(item)

                                Exit For
                            End If
                        Next

                        If (Me.uceActiveConfig.SelectedItem Is Nothing) Then
                            Me.uceActiveConfig.Text = KblObjectType.Custom.ToLocalizedString
                        End If

                        _isDirty = True
                    End If
                ElseIf (_clickedColumn Is Nothing) AndAlso (Me.uceActiveConfig.Text <> KblObjectType.Custom.ToLocalizedString) Then
                    If (DirectCast(Me.uceActiveConfig.SelectedItem.DataValue, HarnessModuleConfiguration).ConfigurationType = HarnessModuleConfigurationType.FromKBL) Then
                        MessageBoxEx.ShowWarning(ModuleConfigFormStrings.ConfigCannotBeDel_Msg)
                    ElseIf (MessageBox.Show(ModuleConfigFormStrings.DelActConfig_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = System.Windows.Forms.DialogResult.Yes) Then
                        Me.udsModuleConfig.Band.Columns.Remove(DirectCast(Me.uceActiveConfig.SelectedItem.DataValue, HarnessModuleConfiguration).HarnessConfiguration.Part_number)
                        Me.uceActiveConfig.Items.Remove(Me.uceActiveConfig.SelectedItem)
                        Me.uceActiveConfig.Text = KblObjectType.Custom.ToLocalizedString

                        _isDirty = True
                    End If
                Else
                    MessageBoxEx.ShowWarning(ModuleConfigFormStrings.CustomConfigCannotBeDel_Msg)
                End If
            Case ContextMenuToolKey.DeselectAllModules.ToString, ContextMenuToolKey.SelectAllModules.ToString
                Dim columnKey As String = DirectCast(Me.uceActiveConfig.SelectedItem.DataValue, HarnessModuleConfiguration).HarnessConfiguration.Part_number
                Dim isReadOnly As Boolean = DirectCast(Me.uceActiveConfig.SelectedItem.DataValue, HarnessModuleConfiguration).ConfigurationType = HarnessModuleConfigurationType.FromKBL

                If (_clickedColumn IsNot Nothing) Then
                    columnKey = _clickedColumn.Key
                    isReadOnly = (DirectCast(_clickedColumn.Tag, HarnessModuleConfiguration).ConfigurationType = HarnessModuleConfigurationType.FromKBL)
                End If

                If (isReadOnly) Then
                    MessageBoxEx.ShowWarning(ModuleConfigFormStrings.ConfigCannotBeMod_Msg)
                Else
                    Me.ugModuleConfig.BeginUpdate()

                    For Each row As UltraGridRow In Me.ugModuleConfig.Rows
                        row.Cells(columnKey).Value = (e.Tool.Key = ContextMenuToolKey.SelectAllModules.ToString)
                    Next

                    Me.ugModuleConfig.EndUpdate()
                End If
            Case ContextMenuToolKey.ExportActive.ToString, ContextMenuToolKey.ExportAll.ToString, ContextMenuToolKey.ExportUserDefined.ToString
                ExportConfigurations(e.Tool.Key)
            Case ContextMenuToolKey.PasteFromClipboard.ToString
                Dim columnKey As String = DirectCast(Me.uceActiveConfig.SelectedItem.DataValue, HarnessModuleConfiguration).HarnessConfiguration.Part_number
                Dim isReadOnly As Boolean = DirectCast(Me.uceActiveConfig.SelectedItem.DataValue, HarnessModuleConfiguration).ConfigurationType = HarnessModuleConfigurationType.FromKBL

                If (_clickedColumn IsNot Nothing) Then
                    columnKey = _clickedColumn.Key
                    isReadOnly = (DirectCast(_clickedColumn.Tag, HarnessModuleConfiguration).ConfigurationType = HarnessModuleConfigurationType.FromKBL)
                End If

                If (isReadOnly) Then
                    MessageBoxEx.ShowWarning(ModuleConfigFormStrings.ConfigCannotBeMod_Msg)
                Else
                    Me.ugModuleConfig.BeginUpdate()

                    Dim clipboardRegex As Regex = Nothing

                    Try
                        clipboardRegex = New Regex(_clipboardModulePartNumberRegEx)
                    Catch ex As Exception
                        ex.ShowMessageBox(ModuleConfigFormStrings.RegExInvalid_Msg)
                        clipboardRegex = New Regex("\t|\r|\v|\n|[.,;|]")
                    End Try

                    Dim pastedStrings As New List(Of String)
                    Dim pastedText As String = Clipboard.GetText

                    For Each subString As String In clipboardRegex.Split(pastedText)
                        subString = Regex.Replace(subString, "\s", String.Empty)

                        If (subString <> String.Empty) AndAlso (Not pastedStrings.Contains(subString)) Then
                            pastedStrings.Add(subString)
                        End If
                    Next

                    Dim modules As List(Of [Module]) = _kblMapper.GetModules.ToList
                    Dim notMatchingStrings As New List(Of String)

                    For Each pastedString As String In pastedStrings
                        If (Not modules.Where(Function(m) m.Part_number.RegExEmptyReplace("\s") = pastedString).Any) AndAlso (Not notMatchingStrings.Contains(pastedString)) Then
                            notMatchingStrings.Add(pastedString)
                        End If
                    Next

                    Dim dialogResult As DialogResult = DialogResult.OK

                    If notMatchingStrings.Count > 0 Then
                        Using pasteResultForm As New PasteResultForm(notMatchingStrings)
                            dialogResult = pasteResultForm.ShowDialog(Me)
                        End Using
                    End If

                    If dialogResult = System.Windows.Forms.DialogResult.OK Then
                        For Each row As UltraGridRow In Me.ugModuleConfig.Rows
                            row.Cells(columnKey).Value = pastedStrings.Contains(row.Cells(ModuleConfigFormStrings.PartNumber_ColCaption).Value.ToString.RegExEmptyReplace("\s"))
                        Next
                    End If

                    Me.ugModuleConfig.EndUpdate()

                    ' HINT: There is a bug in Windows 7 (x64) systems when Clipboard.Clear() method will be used to delete all clipboard content.
                    '       This leads to some random crashes in other currently opened applications!
                    Clipboard.SetText(" ")
                End If
            Case ContextMenuToolKey.RenameModConfig.ToString
                If (_clickedColumn IsNot Nothing AndAlso DirectCast(_clickedColumn.Tag, HarnessModuleConfiguration).ConfigurationType <> HarnessModuleConfigurationType.UserDefined) OrElse (_clickedColumn Is Nothing AndAlso DirectCast(Me.uceActiveConfig.SelectedItem.DataValue, HarnessModuleConfiguration).ConfigurationType <> HarnessModuleConfigurationType.UserDefined) Then
                    MessageBoxEx.ShowWarning(ModuleConfigFormStrings.ConfigCannotBeRen_Msg)
                Else
                    Dim columnKey As String = DirectCast(Me.uceActiveConfig.SelectedItem.DataValue, HarnessModuleConfiguration).HarnessConfiguration.Part_number

                    If (_clickedColumn IsNot Nothing) Then
                        columnKey = _clickedColumn.Key
                    End If

                    Using inputForm As New InputForm(ModuleConfigFormStrings.ChooseConfigName_InputBoxPrompt, ModuleConfigFormStrings.RenameConfig_InputBoxTitle, columnKey)
                        If (inputForm.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then
                            Dim harnessModuleConfigName As String = inputForm.Response.Trim

                            If harnessModuleConfigName.IsNullOrEmpty Then
                                MessageBoxEx.ShowWarning(ModuleConfigFormStrings.SelConfigNameIsEmpty_Msg)
                            ElseIf (harnessModuleConfigName.Length > 50) Then
                                MessageBoxEx.ShowWarning(ModuleConfigFormStrings.SelConfigNameTooLong_Msg)
                            ElseIf (Me.udsModuleConfig.Band.Columns.Exists(harnessModuleConfigName)) Then
                                MessageBoxEx.ShowWarning(ModuleConfigFormStrings.SelConfigNameExists_Msg)
                            ElseIf (_clickedColumn IsNot Nothing) Then
                                Dim oldHarnessModuleConfigName As String = _clickedColumn.Key

                                Me.udsModuleConfig.Band.Columns(_clickedColumn.Key).Key = harnessModuleConfigName

                                _clickedColumn.Header.Caption = harnessModuleConfigName

                                DirectCast(_clickedColumn.Tag, HarnessModuleConfiguration).HarnessConfiguration.Part_number = harnessModuleConfigName

                                For Each item As Infragistics.Win.ValueListItem In Me.uceActiveConfig.Items
                                    If (item.DisplayText = oldHarnessModuleConfigName) Then
                                        DirectCast(item.DataValue, HarnessModuleConfiguration).HarnessConfiguration.Part_number = harnessModuleConfigName

                                        item.DisplayText = harnessModuleConfigName

                                        Exit For
                                    End If
                                Next

                                _isDirty = True
                            Else
                                Dim oldHarnessModuleConfigName As String = Me.uceActiveConfig.SelectedItem.DisplayText

                                Me.uceActiveConfig.SelectedItem.DisplayText = harnessModuleConfigName
                                Me.uceActiveConfig.Text = harnessModuleConfigName

                                DirectCast(Me.uceActiveConfig.SelectedItem.DataValue, HarnessModuleConfiguration).HarnessConfiguration.Part_number = harnessModuleConfigName

                                DirectCast(Me.ugModuleConfig.DisplayLayout.Bands(0).Columns(oldHarnessModuleConfigName).Tag, HarnessModuleConfiguration).HarnessConfiguration.Part_number = harnessModuleConfigName

                                Me.udsModuleConfig.Band.Columns(oldHarnessModuleConfigName).Key = harnessModuleConfigName

                                Me.ugModuleConfig.DisplayLayout.Bands(0).Columns(harnessModuleConfigName).Header.Caption = harnessModuleConfigName

                                _isDirty = True
                            End If
                        End If
                    End Using
                End If
            Case ContextMenuToolKey.ShowHideModConfig.ToString
                Me.ugModuleConfig.BeginUpdate()

                For Each column As UltraGridColumn In Me.ugModuleConfig.DisplayLayout.Bands(0).Columns
                    If (column.Tag IsNot Nothing) AndAlso (DirectCast(column.Tag, HarnessModuleConfiguration).ConfigurationType = HarnessModuleConfigurationType.FromKBL) Then
                        column.Hidden = Not column.Hidden
                    End If
                Next

                Me.ugModuleConfig.EndUpdate()
        End Select

        _clickedColumn = Nothing
    End Sub


    Friend ReadOnly Property IsDirty() As Boolean
        Get
            Return _isDirty
        End Get
    End Property

End Class