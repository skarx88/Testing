Imports System.IO
Imports Infragistics.Win
Imports Infragistics.Win.UltraWinEditors
Imports Infragistics.Win.UltraWinGrid
Imports Infragistics.Win.UltraWinTabControl
Imports Infragistics.Win.UltraWinToolTip
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.HarnessAnalyzer.Shared
Imports Zuken.E3.Lib.Schema.Kbl.Properties

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class GeneralSettingsForm

    Private _contextMenuStrip As ContextMenuStrip
    Private _defaultLengthTypeChanged As Boolean
    Private _directoryInvalid As Boolean = False
    Private _generalSettings As GeneralSettings
    Private _isInitializing As Boolean
    Private _lastValidValues As New Dictionary(Of UltraTextEditor, String)
    Private _needsReloadMessage As Boolean = False
    Private _recentFileListCleared As Boolean
    Private _activeTabOnShow As String = Nothing

    Public Const TAB_3D As String = "tab3D"
    Public Const TAB_CONNECTORS As String = "tabConn"
    Public Const TAB_BUNDLE_DIAMETER_RANGES As String = "tabBundleDiameterRanges"
    Private Const TAB_KEY_EXPERIMENTAL As String = "tabExperimental"

    Public Sub New(generalSettings As GeneralSettings, cableLengthTypes As List(Of String), wireLengthTypes As List(Of String))
        InitializeComponent()

        _generalSettings = generalSettings

        Initialize(cableLengthTypes, wireLengthTypes)
    End Sub

    Private Function CheckDirectoryValid(directoryPath As String) As Boolean
        If (Not String.IsNullOrWhiteSpace(directoryPath)) Then
            Try
                Dim dir As New DirectoryInfo(directoryPath)

                Return dir.Exists
            Catch ex As Exception
                Return False
            End Try
        End If

        Return True
    End Function

    Protected Overrides Sub OnLoad(e As EventArgs)
#If CONFIG = "Debug" Or DEBUG Then
        Dim tabPreviewFeatures As UltraTab = Me.utcSettings.Tabs.Item(TAB_KEY_EXPERIMENTAL)
        tabPreviewFeatures.Visible = True
        Me.chkFilePreviewAddon.Checked = [Shared].Utilities.IsFilePreviewerRegistered
        Me.utcSettings.Tabs(0).Active = True
#End If

        If Not String.IsNullOrEmpty(_activeTabOnShow) Then
            If utcSettings.Tabs.Exists(_activeTabOnShow) Then
                Dim tab As UltraWinTabControl.UltraTab = utcSettings.Tabs.Item(_activeTabOnShow)
                tab.Active = True
                tab.Selected = True
            End If
        End If
        MyBase.OnLoad(e)
    End Sub

    Public Shadows Function ShowDialog(owner As IWin32Window) As DialogResult
        _activeTabOnShow = Nothing
        Return MyBase.ShowDialog(owner)
    End Function

    Public Shadows Function ShowDialog(owner As IWin32Window, activeTab As String) As DialogResult
        _activeTabOnShow = activeTab
        Return MyBase.ShowDialog(owner)
    End Function

    Private Sub Initialize(cableLengthTypes As List(Of String), wireLengthTypes As List(Of String))
        Me.BackColor = Color.White
        Me.Icon = My.Resources.GeneralSettings

        _isInitializing = True

        Me.uceLanguage.Items.Add("en-US", GeneralSettingsFormStrings.EnglishLang_ListItem)
        Me.uceLanguage.Items.Add("de-DE", GeneralSettingsFormStrings.GermanLang_ListItem)

        Me.uckTouchSupport.Checked = _generalSettings.TouchEnabledUI
        Me.uckDataValidation.Checked = _generalSettings.ValidateKBLAfterLoad

        Me.uceLanguage.SelectedText = Globalization.CultureInfo.CurrentUICulture.Name 'HINT: reflects the current ui culture and not the information serialized in the settings, this is more accurate to the user experience

        If (_generalSettings.DefaultCableLengthType IsNot Nothing) AndAlso (_generalSettings.DefaultCableLengthType <> String.Empty) Then
            Me.uceLengthTypeCables.Items.Add(_generalSettings.DefaultCableLengthType, _generalSettings.DefaultCableLengthType)
        End If
        If (_generalSettings.DefaultWireLengthType IsNot Nothing) AndAlso (_generalSettings.DefaultWireLengthType <> String.Empty) Then
            Me.uceLengthTypeWires.Items.Add(_generalSettings.DefaultWireLengthType, _generalSettings.DefaultWireLengthType)
        End If

        If (cableLengthTypes IsNot Nothing) Then
            For Each cableLengthType As String In cableLengthTypes
                If (cableLengthType <> _generalSettings.DefaultCableLengthType) Then
                    Me.uceLengthTypeCables.Items.Add(cableLengthType, cableLengthType)
                End If
            Next

            Me.lblSelectableLengthTypeCables.Visible = False
            Me.uceLengthTypeCables.ReadOnly = False
        End If

        If (wireLengthTypes IsNot Nothing) Then
            For Each wireLengthType As String In wireLengthTypes
                If (wireLengthType <> _generalSettings.DefaultWireLengthType) Then
                    Me.uceLengthTypeWires.Items.Add(wireLengthType, wireLengthType)
                End If
            Next

            Me.lblSelectableLengthTypeWires.Visible = False
            Me.uceLengthTypeWires.ReadOnly = False
        End If

        _contextMenuStrip = New ContextMenuStrip
        _contextMenuStrip.Items.Add(GeneralSettingsFormStrings.Delete_CtxtMnu_Caption, My.Resources.Delete)

        AddHandler _contextMenuStrip.ItemClicked, AddressOf OnContextMenuItemClicked

        Me.uceLengthTypeCables.SortStyle = ValueListSortStyle.Ascending
        Me.uceLengthTypeCables.Text = _generalSettings.DefaultCableLengthType

        Me.uceLengthTypeWires.SortStyle = ValueListSortStyle.Ascending
        Me.uceLengthTypeWires.Text = _generalSettings.DefaultWireLengthType

        Me.uckHideNavigatorHub.Checked = _generalSettings.HideNavigatorHubOnLoad

        Me.ugBundleDiameterRanges.SyncWithCurrencyManager = False
        Me.ugBundleDiameterRanges.DataSource = _generalSettings.BundleDiameterRanges

        Me.ugInlinerIdentifiers.SyncWithCurrencyManager = False
        Me.ugInlinerIdentifiers.DataSource = _generalSettings.InlinerIdentifiers

        Me.ugUSSpliceIdentifiers.SyncWithCurrencyManager = False
        Me.ugUSSpliceIdentifiers.DataSource = _generalSettings.UltrasonicSpliceIdentifiers

        Me.ugEyeletIdentifiers.SyncWithCurrencyManager = False
        Me.ugEyeletIdentifiers.DataSource = _generalSettings.EyeletIdentifiers
        Me.ugSpliceIdentifiers.SyncWithCurrencyManager = False
        Me.ugSpliceIdentifiers.DataSource = _generalSettings.SpliceIdentifiers

        Me.utEcuConnectorSeparator.Text = _generalSettings.EcuConnectorIdentifier.IdentificationCriteria

        Me.ugWireColorCodes.SyncWithCurrencyManager = False
        Me.ugWireColorCodes.DataSource = _generalSettings.WireColorCodes

        Me.uneMoveDistTolerance.Value = _generalSettings.MoveDistanceToleranceForGraphicalCompare
        Me.uneMoveDistTolerance.MaxValue = _generalSettings.MoveDistanceToleranceMaxForGraphicalCompare
        Me.uneScaleFactorRedliningIndicator.Value = _generalSettings.RedliningStampIndicatorScaleFactor

        Me.lblMoveDistToleranceInfo.Text = String.Format(GeneralSettingsFormStrings.MaxMoveTolerance, _generalSettings.MoveDistanceToleranceMaxForGraphicalCompare.ToString)

        Select Case _generalSettings.ObjectTypeDependentSelection
            Case False
                Me.uosDrawingSelectionMode.CheckedIndex = 0
            Case True
                Me.uosDrawingSelectionMode.CheckedIndex = 1
        End Select

        Select Case _generalSettings.ShowFullScreenAxisCursor
            Case False
                Me.uosSelectCursor.CheckedIndex = 0
            Case True
                Me.uosSelectCursor.CheckedIndex = 1
        End Select

        Me.utcSettings.Tabs(TAB_CONNECTORS).Visible = My.Application.MainForm.HasSchematicsFeature
        Me.utcSettings.Tabs(TAB_3D).Visible = My.Application.MainForm.HasView3DFeature
        Me.utcSettings.Tabs(TAB_BUNDLE_DIAMETER_RANGES).Visible = False ' TODO: Temporary invisible until feature will be released

        Me.txtCarModelDirectory.Text = _generalSettings.CarModelDirectory
        Me.txtConnectorFacesDirectory.Text = _generalSettings.ConnectorFacesDirectory
        Me.uckAutoSyncCavityChecks.Checked = _generalSettings.AutoSyncCavityChecksSelection
        Me.uckAutoSync3D.Checked = _generalSettings.AutoSync3DSelection
        Me.uckSchematicsAutoZoom.Checked = _generalSettings.AutoZoomSelectionSchematics
        Me.uckRotateAroundSelection.Checked = _generalSettings.UseSelectionCenterForRotation
        Me.uckRotateAroundSelection.Text = Document3DStrings.uck_RotateSelection
        Me.uckAutomaticTooltips.Checked = _generalSettings.AutomaticTooltips
        Me.uckAutomaticTooltips.Text = GeneralSettingsFormStrings.uckAutomaticTooltips


        _lastValidValues.Add(txtCarModelDirectory, txtCarModelDirectory.Text)
        _lastValidValues.Add(txtConnectorFacesDirectory, txtConnectorFacesDirectory.Text)

        uce_LengthClass.Items.Clear()
        uce_LengthClass.Items.Add(E3.Lib.Model.LengthClass.DMU, GeneralSettingsFormStrings.VirtualLength)
        uce_LengthClass.Items.Add(E3.Lib.Model.LengthClass.Nominal, GeneralSettingsFormStrings.PhysicalLength)

        With uceLogMsgVerbosity
            .Items.Clear()
            .Items.Add(ToastMessageVerbosity.Silent, GeneralSettingsFormStrings.LogVerbosity_Silent)
            .Items.Add(ToastMessageVerbosity.Errors, GeneralSettingsFormStrings.LogVerbosity_ErrorsOnly)
            .Items.Add(ToastMessageVerbosity.Warnings Or ToastMessageVerbosity.Errors, GeneralSettingsFormStrings.LogVerbosity_WarningsAndErrors)
            .Value = _generalSettings.ToastMessageVerbosity
        End With

        If _generalSettings.LengthClassDocument3D = 0 Then
            uce_LengthClass.Value = E3.Lib.Model.LengthClass.DMU
        Else
            uce_LengthClass.Value = _generalSettings.LengthClassDocument3D
        End If

        chkUseDynamicBundles.Checked = _generalSettings.UseDynamicBundles
        chkUseJTColors.Checked = _generalSettings.UseJTColors

        _isInitializing = False
    End Sub

    Private Sub InitializeGridLayout(layout As UltraGridLayout)
        With layout
            .AutoFitStyle = AutoFitStyle.ResizeAllColumns
            .CaptionVisible = Infragistics.Win.DefaultableBoolean.True
            .GroupByBox.Hidden = True

            With .Override
                .AllowColMoving = AllowColMoving.NotAllowed
                .RowSelectors = Infragistics.Win.DefaultableBoolean.True
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

                                If (column.Index = 0) Then
                                    .SortIndicator = SortIndicator.Ascending
                                End If
                            End With
                        End If
                    Next
                End With
            Next
        End With
    End Sub

    Private Sub OnContextMenuItemClicked(sender As Object, e As ToolStripItemClickedEventArgs)
        With DirectCast(DirectCast(sender, ContextMenuStrip).Tag, UltraGrid)
            .ActiveRow.Delete()

            For Each row As UltraGridRow In .Selected.Rows
                row.Delete()
            Next
        End With
    End Sub

    Private Sub RestoreLastValidValue(editor As UltraTextEditor)
        If (_directoryInvalid) Then
            Dim restoreDirValid As Boolean = CheckDirectoryValid(_lastValidValues(editor))
            editor.Text = If(restoreDirValid, _lastValidValues(editor), String.Empty)

            If (editor.Text.Length > 1) Then
                editor.SelectionStart = editor.Text.Length - 1
            End If

            _directoryInvalid = False
        End If
    End Sub

    Private Sub GeneralSettingsForm_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        If (_needsReloadMessage) Then
            MessageBox.Show(Me, GeneralSettingsFormStrings.PleaseCloseAllOpenedDocumentsToApplyChange_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

        _generalSettings.CarModelDirectory = Me.txtCarModelDirectory.Text
        _generalSettings.ConnectorFacesDirectory = Me.txtConnectorFacesDirectory.Text
        _generalSettings.AutoSync3DSelection = Me.uckAutoSync3D.Checked
        _generalSettings.AutoZoomSelectionSchematics = Me.uckSchematicsAutoZoom.Checked
        _generalSettings.AutoSyncCavityChecksSelection = Me.uckAutoSyncCavityChecks.Checked
        _generalSettings.UseSelectionCenterForRotation = Me.uckRotateAroundSelection.Checked
        _generalSettings.AutomaticTooltips = Me.uckAutomaticTooltips.Checked
    End Sub

    Private Sub GeneralSettingsForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        RemoveHandler _contextMenuStrip.ItemClicked, AddressOf OnContextMenuItemClicked
    End Sub

    Private Sub GeneralSettingsForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If (e.KeyCode = Keys.Escape AndAlso Not _directoryInvalid) Then
            Me.Close()
        End If
    End Sub

    Private Sub btnAddBDR_Click(sender As Object, e As EventArgs) Handles btnAddBDR.Click
        Using inputForm As New InputForm(GeneralSettingsFormStrings.EnterDiaRange_InputBoxPrompt, GeneralSettingsFormStrings.AddDiaRange_InputBoxTitle, GeneralSettingsFormStrings.MinMaxDia_InputBoxDefResp)
            If (inputForm.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then
                Dim bundleDiameterRange As String = inputForm.Response.Trim
                If (bundleDiameterRange <> String.Empty) Then
                    Dim minDiameter As Integer = Integer.MinValue
                    Dim maxDiameter As Integer = Integer.MaxValue

                    For Each diameterValue As String In bundleDiameterRange.Split("-".ToCharArray, StringSplitOptions.RemoveEmptyEntries)
                        If (IsNumeric(diameterValue)) Then
                            If (minDiameter = Integer.MinValue) Then
                                minDiameter = CInt(diameterValue)
                            ElseIf (maxDiameter = Integer.MaxValue) Then
                                maxDiameter = CInt(diameterValue)
                            End If
                        End If
                    Next

                    If (minDiameter <> Integer.MinValue) AndAlso (maxDiameter <> Integer.MaxValue) AndAlso (_generalSettings.BundleDiameterRanges.FindBundleDiameterRange(minDiameter, maxDiameter) Is Nothing) Then
                        Me.ugBundleDiameterRanges.BeginUpdate()

                        _generalSettings.BundleDiameterRanges.Add(New BundleDiameterRange(minDiameter, maxDiameter, Color.Black.ToArgb))

                        Me.ugBundleDiameterRanges.EndUpdate()
                    Else
                        MessageBoxEx.ShowError(GeneralSettingsFormStrings.DiaHasInvalidFormat_Msg)
                    End If
                End If
            End If
        End Using
    End Sub

    Private Sub btnAddInlIdentifier_Click(sender As Object, e As EventArgs) Handles btnAddInlIdentifier.Click
        Dim kblPropertyValueList As New ValueList

        If (Not ugInlinerIdentifiers.Rows.Where(Function(row) row.Cells("KBLPropertyName").Value.ToString = ObjectPropertyNameStrings.Id).Any()) Then
            kblPropertyValueList.ValueListItems.Add(ConnectorPropertyName.Id.ToString, ObjectPropertyNameStrings.Id)
        End If

        If (Not ugInlinerIdentifiers.Rows.Where(Function(row) row.Cells("KBLPropertyName").Value.ToString = ObjectPropertyNameStrings.AliasId).Any()) Then
            kblPropertyValueList.ValueListItems.Add(ConnectorPropertyName.Alias_id.ToString, ObjectPropertyNameStrings.AliasId)
        End If

        If (Not ugInlinerIdentifiers.Rows.Where(Function(row) row.Cells("KBLPropertyName").Value.ToString = ObjectPropertyNameStrings.Description).Any()) Then
            kblPropertyValueList.ValueListItems.Add(ConnectorPropertyName.Description.ToString, ObjectPropertyNameStrings.Description)
        End If

        If (Not ugInlinerIdentifiers.Rows.Where(Function(row) row.Cells("KBLPropertyName").Value.ToString = ObjectPropertyNameStrings.ReferenceElement).Any()) Then
            kblPropertyValueList.ValueListItems.Add(ConnectorPropertyName.Reference_element.ToString, ObjectPropertyNameStrings.ReferenceElement)
        End If

        If (Not ugInlinerIdentifiers.Rows.Where(Function(row) row.Cells("KBLPropertyName").Value.ToString = ObjectPropertyNameStrings.InstallationInformation).Any()) Then
            kblPropertyValueList.ValueListItems.Add(ConnectorPropertyName.Installation_Information.ToString, ObjectPropertyNameStrings.InstallationInformation)
        End If

        If (kblPropertyValueList.ValueListItems.Count > 0) Then
            Using inputForm As New InputForm(GeneralSettingsFormStrings.AddInlIdent_InputBoxPrompt, GeneralSettingsFormStrings.SelKBLConn_InputBoxTitle, String.Empty, kblPropertyValueList)
                If (inputForm.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then
                    ' User entered a new (not existing) inliner identifier
                    Me.ugInlinerIdentifiers.BeginUpdate()
                    _generalSettings.InlinerIdentifiers.Add(New InlinerIdentifier(inputForm.Response, "*"))
                    Me.ugInlinerIdentifiers.EndUpdate()
                    _needsReloadMessage = True
                End If
            End Using
        Else
            MessageBox.Show(GeneralSettingsFormStrings.NoFurtherConnPropAvailable_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub btnAddUSSpliceIdentifier_Click(sender As Object, e As EventArgs) Handles btnAddUSSpliceIdentifier.Click
        Dim kblPropertyValueList As New ValueList

        If (Not ugUSSpliceIdentifiers.Rows.Where(Function(row) row.Cells("KBLPropertyName").Value.ToString = ObjectPropertyNameStrings.Id.ToString).Any()) Then
            kblPropertyValueList.ValueListItems.Add(ConnectorPropertyName.Id.ToString, ObjectPropertyNameStrings.Id)
        End If

        If (Not ugUSSpliceIdentifiers.Rows.Where(Function(row) row.Cells("KBLPropertyName").Value.ToString = ObjectPropertyNameStrings.AliasId).Any()) Then
            kblPropertyValueList.ValueListItems.Add(ConnectorPropertyName.Alias_id.ToString, ObjectPropertyNameStrings.AliasId)
        End If

        If (Not ugUSSpliceIdentifiers.Rows.Where(Function(row) row.Cells("KBLPropertyName").Value.ToString = ObjectPropertyNameStrings.Description).Any()) Then
            kblPropertyValueList.ValueListItems.Add(ConnectorPropertyName.Description.ToString, ObjectPropertyNameStrings.Description)
        End If

        If (Not ugUSSpliceIdentifiers.Rows.Where(Function(row) row.Cells("KBLPropertyName").Value.ToString = ObjectPropertyNameStrings.InstallationInformation).Any()) Then
            kblPropertyValueList.ValueListItems.Add(ConnectorPropertyName.Installation_Information.ToString, ObjectPropertyNameStrings.InstallationInformation)
        End If

        If (Not ugUSSpliceIdentifiers.Rows.Where(Function(row) row.Cells("KBLPropertyName").Value.ToString = ObjectPropertyNameStrings.PartNumber).Any()) Then
            kblPropertyValueList.ValueListItems.Add(PartPropertyName.Part_number.ToString, ObjectPropertyNameStrings.PartNumber)
        End If

        If (Not ugUSSpliceIdentifiers.Rows.Where(Function(row) row.Cells("KBLPropertyName").Value.ToString = ObjectPropertyNameStrings.PartDescription).Any()) Then
            kblPropertyValueList.ValueListItems.Add(PartPropertyName.Part_description.ToString, ObjectPropertyNameStrings.PartDescription)
        End If

        If (Not ugUSSpliceIdentifiers.Rows.Where(Function(row) row.Cells("KBLPropertyName").Value.ToString = ObjectPropertyNameStrings.PartProcessingInformation).Any()) Then
            kblPropertyValueList.ValueListItems.Add(PartPropertyName.Part_processing_information.ToString, ObjectPropertyNameStrings.PartProcessingInformation)
        End If

        If (kblPropertyValueList.ValueListItems.Count > 0) Then
            Using inputForm As New InputForm(GeneralSettingsFormStrings.AddUSSpliceIdent_InputBoxPrompt, GeneralSettingsFormStrings.SelKBLConn_InputBoxTitle, String.Empty, kblPropertyValueList)
                If (inputForm.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then
                    ' User entered a new (not existing) ultrasonic splice identifier
                    Me.ugUSSpliceIdentifiers.BeginUpdate()
                    _generalSettings.UltrasonicSpliceIdentifiers.Add(New UltrasonicSpliceIdentifier(inputForm.Response, "*"))
                    Me.ugUSSpliceIdentifiers.EndUpdate()
                    _needsReloadMessage = True
                End If
            End Using
        Else
            MessageBox.Show(GeneralSettingsFormStrings.NoFurtherConnPropAvailable_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub btnAddWCC_Click(sender As Object, e As EventArgs) Handles btnAddWCC.Click
        Using inputForm As New InputForm(GeneralSettingsFormStrings.EnterColor_InputBoxPrompt, GeneralSettingsFormStrings.AddColor_InputBoxTitle)
            If (inputForm.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then
                Dim wireColorCode As String = inputForm.Response.Trim
                If (wireColorCode = String.Empty) Then
                    ' User entered nothing or pressed cancel
                    ' Nothing to do
                ElseIf (_generalSettings.WireColorCodes.FindWireColorCode(wireColorCode) Is Nothing) Then
                    ' User entered a new (not existing) color code
                    Me.ugWireColorCodes.BeginUpdate()
                    _generalSettings.WireColorCodes.Add(New WireColorCode(wireColorCode, Color.Black.ToArgb))
                    Me.ugWireColorCodes.EndUpdate()
                    _needsReloadMessage = True
                Else
                    ' User entered an already known color code
                    MessageBoxEx.ShowWarning(GeneralSettingsFormStrings.SelColorExists_Msg)
                End If
            End If
        End Using
    End Sub

    Private Sub btnClearRecentFiles_Click(sender As Object, e As EventArgs) Handles btnClearRecentFiles.Click
        _generalSettings.RecentFiles.Clear()
        _recentFileListCleared = True
        MessageBox.Show(GeneralSettingsFormStrings.FileListCleared_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
    End Sub

    Private Sub btnResetBDR_Click(sender As Object, e As EventArgs) Handles btnResetBDR.Click
        If (MessageBox.Show(GeneralSettingsFormStrings.ResetDiaRanges_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = MsgBoxResult.Ok) Then
            Me.ugBundleDiameterRanges.BeginUpdate()

            _generalSettings.BundleDiameterRanges.Clear()
            _generalSettings.BundleDiameterRanges.CreateDefaultBundleDiameterRanges()

            Me.ugBundleDiameterRanges.EndUpdate()
        End If
    End Sub

    Private Sub btnResetConnectivityDefaults_Click(sender As Object, e As EventArgs) Handles btnResetConnectivityDefaults.Click
        If (MessageBox.Show(GeneralSettingsFormStrings.ResetComponentIdent_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = MsgBoxResult.Ok) Then
            Me.ugSpliceIdentifiers.BeginUpdate()
            _generalSettings.SpliceIdentifiers.Clear()
            _generalSettings.SpliceIdentifiers.CreateDefaultIdentifiers()
            Me.ugSpliceIdentifiers.EndUpdate()

            Me.ugEyeletIdentifiers.BeginUpdate()
            _generalSettings.EyeletIdentifiers.Clear()
            _generalSettings.EyeletIdentifiers.CreateDefaultIdentifiers()
            Me.ugEyeletIdentifiers.EndUpdate()

            _generalSettings.EcuConnectorIdentifier = EcuConnectorIdentifier.CreateDefaultIdentifier

            _needsReloadMessage = True
        End If
    End Sub

    Private Sub btnResetInlIdentifiers_Click(sender As Object, e As EventArgs) Handles btnResetInlIdentifiers.Click
        If (MessageBox.Show(GeneralSettingsFormStrings.ResetInlIdent_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = MsgBoxResult.Ok) Then
            Me.ugInlinerIdentifiers.BeginUpdate()

            _generalSettings.InlinerIdentifiers.Clear()
            _generalSettings.InlinerIdentifiers.CreateDefaultIdentifiers()

            Me.ugInlinerIdentifiers.EndUpdate()

            _needsReloadMessage = True
        End If
    End Sub

    Private Sub btnResetUSSpliceIdentifiers_Click(sender As Object, e As EventArgs) Handles btnResetUSSpliceIdentifiers.Click
        If MessageBox.Show(GeneralSettingsFormStrings.ResetUSSpliceIdent_Msg, MSG_BOX_TITLE, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = MsgBoxResult.Ok Then
            ugUSSpliceIdentifiers.BeginUpdate()

            _generalSettings.UltrasonicSpliceIdentifiers.Clear()
            _generalSettings.UltrasonicSpliceIdentifiers.CreateDefaultIdentifiers()

            ugUSSpliceIdentifiers.EndUpdate()

            _needsReloadMessage = True
        End If
    End Sub

    Private Sub btnResetWCC_Click(sender As Object, e As EventArgs) Handles btnResetWCC.Click
        If (MessageBox.Show(GeneralSettingsFormStrings.ResetColorCodes_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = MsgBoxResult.Ok) Then
            Me.ugWireColorCodes.BeginUpdate()

            _generalSettings.WireColorCodes.Clear()
            _generalSettings.WireColorCodes.CreateDefaultColorCodeMapping()

            Me.ugWireColorCodes.EndUpdate()

            _needsReloadMessage = True
        End If
    End Sub

    Private Sub txtDirectories_EditorButtonClick(sender As Object, e As EditorButtonEventArgs) Handles txtCarModelDirectory.EditorButtonClick, txtConnectorFacesDirectory.EditorButtonClick
        Dim sourceEditor As UltraTextEditor = DirectCast(sender, UltraTextEditor)

        Using selectDirDlg As New FolderBrowserDialog
            Dim dir As New DirectoryInfo(If(String.IsNullOrEmpty(sourceEditor.Text), E3.Lib.DotNet.Expansions.Devices.My.Computer.FileSystem.CurrentDirectory, sourceEditor.Text))
            selectDirDlg.InitialDirectory = If(dir.Exists, dir.FullName, String.Empty)

            If selectDirDlg.ShowDialog = DialogResult.OK Then
                sourceEditor.Text = selectDirDlg.SelectedPath
            End If
        End Using
    End Sub

    Private Sub txtDirectories_KeyUp(sender As Object, e As KeyEventArgs) Handles txtCarModelDirectory.KeyUp, txtConnectorFacesDirectory.KeyUp
        If (e.KeyCode = Keys.Escape) Then
            Dim sourceEditor As UltraTextEditor = DirectCast(sender, UltraTextEditor)

            RestoreLastValidValue(sourceEditor)
        End If
    End Sub

    Private Sub txtDirectories_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles txtCarModelDirectory.Validating, txtConnectorFacesDirectory.Validating
        Dim sourceEditor As UltraTextEditor = DirectCast(sender, UltraTextEditor)
        _directoryInvalid = Not CheckDirectoryValid(sourceEditor.Text)
        e.Cancel = _directoryInvalid

        If (e.Cancel) Then
            Dim ttInfo As New UltraToolTipInfo()
            ttInfo.ToolTipText = String.Format(ErrorStrings.GeneralSettings_DirectoryNotFound, sourceEditor.Text)

            Me.uttmSettings.SetUltraToolTip(sourceEditor, ttInfo)
            Me.uttmSettings.ShowToolTip(sourceEditor, sourceEditor.Parent.PointToScreen(New Point(sourceEditor.Location.X + sourceEditor.Size.Width, sourceEditor.Location.Y)))
        End If

        If (Not e.Cancel) Then
            _lastValidValues.Remove(sourceEditor)
            _lastValidValues.Add(sourceEditor, sourceEditor.Text)
        End If
    End Sub

    Private Sub uceLanguage_ValueChanged(sender As Object, e As EventArgs) Handles uceLanguage.ValueChanged
        If (Not _isInitializing) Then
            _generalSettings.UILanguage = Me.uceLanguage.SelectedItem.DataValue.ToString

            If (MessageBoxEx.ShowQuestion(GeneralSettingsFormStrings.ResetDataTableSettings_Msg) = System.Windows.Forms.DialogResult.Yes) Then
                My.Settings.ResetDataTableSettings = True
            End If

            MessageBox.Show(GeneralSettingsFormStrings.RestartApp_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub uceLengthTypeCables_ValueChanged(sender As Object, e As EventArgs) Handles uceLengthTypeCables.ValueChanged
        If (Not _isInitializing) Then
            _defaultLengthTypeChanged = True
            _generalSettings.DefaultCableLengthType = Me.uceLengthTypeCables.SelectedItem.DisplayText
        End If
    End Sub

    Private Sub uceLengthTypeWires_ValueChanged(sender As Object, e As EventArgs) Handles uceLengthTypeWires.ValueChanged
        If (Not _isInitializing) Then
            _defaultLengthTypeChanged = True
            _generalSettings.DefaultWireLengthType = Me.uceLengthTypeWires.SelectedItem.DisplayText
        End If
    End Sub

    Private Sub uckDataValidation_CheckedValueChanged(sender As Object, e As EventArgs) Handles uckDataValidation.CheckedValueChanged
        _generalSettings.ValidateKBLAfterLoad = Me.uckDataValidation.Checked
    End Sub

    Private Sub uckHideNavigatorHub_CheckedValueChanged(sender As Object, e As EventArgs) Handles uckHideNavigatorHub.CheckedValueChanged
        _generalSettings.HideNavigatorHubOnLoad = Me.uckHideNavigatorHub.Checked
    End Sub

    Private Sub uckTouchSupport_CheckedValueChanged(sender As Object, e As EventArgs) Handles uckTouchSupport.CheckedValueChanged
        _generalSettings.TouchEnabledUI = Me.uckTouchSupport.Checked
    End Sub

    Private Sub ugBundleDiameterRanges_BeforeRowsDeleted(sender As Object, e As BeforeRowsDeletedEventArgs) Handles ugBundleDiameterRanges.BeforeRowsDeleted
        e.DisplayPromptMsg = False

        For Each row As UltraGridRow In e.Rows
            If (MessageBoxEx.ShowQuestion(String.Format(GeneralSettingsFormStrings.DelBundleDiaRange_Msg, row.Cells("MinDiameter").Value, row.Cells("MaxDiameter").Value)) = MsgBoxResult.No) Then
                e.Cancel = True
                Exit For
            End If
        Next
    End Sub

    Private Sub ugBundleDiameterRanges_CellDataError(sender As Object, e As CellDataErrorEventArgs) Handles ugBundleDiameterRanges.CellDataError
        e.RaiseErrorEvent = False
        e.RestoreOriginalValue = True
    End Sub

    Private Sub ugBundleDiameterRanges_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugBundleDiameterRanges.InitializeLayout
        Me.ugBundleDiameterRanges.BeginUpdate()

        InitializeGridLayout(e.Layout)

        With e.Layout.Bands(0)
            .Columns("MinDiameter").Header.Caption = GeneralSettingsFormStrings.MinDia_ColCaption
            .Columns("MaxDiameter").Header.Caption = GeneralSettingsFormStrings.MaxDia_ColCaption
            .Columns("ColorRGB").Hidden = True
            .Columns("Color").Style = ColumnStyle.Color
        End With

        Me.ugBundleDiameterRanges.EndUpdate()
    End Sub

    Private Sub ugInlinerIdentifiers_AfterCellUpdate(sender As Object, e As CellEventArgs) Handles ugInlinerIdentifiers.AfterCellUpdate
        _needsReloadMessage = True
    End Sub

    Private Sub ugInlinerIdentifiers_AfterRowsDeleted(sender As Object, e As EventArgs) Handles ugInlinerIdentifiers.AfterRowsDeleted
        _needsReloadMessage = True
    End Sub

    Private Sub ugInlinerIdentifiers_BeforeCellUpdate(sender As Object, e As BeforeCellUpdateEventArgs) Handles ugInlinerIdentifiers.BeforeCellUpdate
        If (e.Cell.Column.Key = "IdentificationCriteria") AndAlso (e.NewValue.ToString = String.Empty) Then
            e.Cancel = True
        End If
        Try
            Dim id As New InlinerIdentifier("", e.NewValue.ToString)
        Catch ex As Exception
            e.Cancel = True
            ex.ShowMessageBox(Me, String.Format(GeneralSettingsFormStrings.RegExInvalid, e.NewValue.ToString))
        End Try
    End Sub

    Private Sub ugInlinerIdentifiers_BeforeRowsDeleted(sender As Object, e As BeforeRowsDeletedEventArgs) Handles ugInlinerIdentifiers.BeforeRowsDeleted
        e.DisplayPromptMsg = False

        For Each row As UltraGridRow In e.Rows
            If (MessageBoxEx.ShowQuestion(String.Format(GeneralSettingsFormStrings.DelInlIdent_Msg, row.Cells("KBLPropertyName").Value)) = MsgBoxResult.No) Then
                e.Cancel = True
                Exit For
            End If
        Next
    End Sub

    Private Sub ugInlinerIdentifiers_CellDataError(sender As Object, e As CellDataErrorEventArgs) Handles ugInlinerIdentifiers.CellDataError
        e.RaiseErrorEvent = False
        e.RestoreOriginalValue = True
        e.StayInEditMode = False
    End Sub

    Private Sub ugInlinerIdentifiers_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugInlinerIdentifiers.InitializeLayout
        Me.ugInlinerIdentifiers.BeginUpdate()

        InitializeGridLayout(e.Layout)

        With e.Layout.Bands(0)
            .Columns("KBLPropertyName").CellActivation = Activation.NoEdit
            .Columns("KBLPropertyName").Header.Caption = GeneralSettingsFormStrings.KBLPropName_ColCaption
            .Columns("IdentificationCriteria").Header.Caption = GeneralSettingsFormStrings.IdentCriteria_ColCaption

            .Override.RowSelectorAppearance.Image = My.Resources.Ampersand.ToBitmap
        End With

        Me.ugInlinerIdentifiers.EndUpdate()
    End Sub

    Private Sub ugInlinerIdentifiers_MouseDown(sender As Object, e As MouseEventArgs) Handles ugInlinerIdentifiers.MouseDown
        Me.ugInlinerIdentifiers.ContextMenuStrip = Nothing

        If (e.Button = System.Windows.Forms.MouseButtons.Right) Then
            Dim element As Infragistics.Win.UIElement = Me.ugInlinerIdentifiers.DisplayLayout.UIElement.LastElementEntered
            Dim row As UltraGridRow = TryCast(element.GetContext(GetType(UltraGridRow)), UltraGridRow)

            Me.ugInlinerIdentifiers.ContextMenuStrip = If(row IsNot Nothing AndAlso (row.IsActiveRow OrElse row.Selected), _contextMenuStrip, Nothing)

            If (Me.ugInlinerIdentifiers.ContextMenuStrip IsNot Nothing) Then
                Me.ugInlinerIdentifiers.ContextMenuStrip.Tag = Me.ugInlinerIdentifiers
            End If
        End If
    End Sub

    Private Sub ugUSSpliceIdentifiers_AfterCellUpdate(sender As Object, e As CellEventArgs) Handles ugUSSpliceIdentifiers.AfterCellUpdate
        _needsReloadMessage = True
    End Sub

    Private Sub ugUSSpliceIdentifiers_AfterRowsDeleted(sender As Object, e As EventArgs) Handles ugUSSpliceIdentifiers.AfterRowsDeleted
        _needsReloadMessage = True
    End Sub

    Private Sub ugUSSpliceIdentifiers_BeforeCellUpdate(sender As Object, e As BeforeCellUpdateEventArgs) Handles ugUSSpliceIdentifiers.BeforeCellUpdate
        If (e.Cell.Column.Key = "IdentificationCriteria") AndAlso (e.NewValue.ToString = String.Empty) Then
            e.Cancel = True
        End If

        Try
            Dim id As New UltrasonicSpliceIdentifier("", e.NewValue.ToString)
        Catch ex As Exception
            e.Cancel = True
            ex.ShowMessageBox(Me, String.Format(GeneralSettingsFormStrings.RegExInvalid, e.NewValue.ToString))
        End Try
    End Sub

    Private Sub ugUSSpliceIdentifiers_BeforeRowsDeleted(sender As Object, e As BeforeRowsDeletedEventArgs) Handles ugUSSpliceIdentifiers.BeforeRowsDeleted
        e.DisplayPromptMsg = False

        For Each row As UltraGridRow In e.Rows
            If (MessageBoxEx.ShowQuestion(String.Format(GeneralSettingsFormStrings.DelUSSpliceIdent_Msg, row.Cells("KBLPropertyName").Value)) = MsgBoxResult.No) Then
                e.Cancel = True
                Exit For
            End If
        Next
    End Sub

    Private Sub ugUSSpliceIdentifiers_CellDataError(sender As Object, e As CellDataErrorEventArgs) Handles ugUSSpliceIdentifiers.CellDataError
        e.RaiseErrorEvent = False
        e.RestoreOriginalValue = True
        e.StayInEditMode = False
    End Sub

    Private Sub ugUSSpliceIdentifiers_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugUSSpliceIdentifiers.InitializeLayout
        Me.ugUSSpliceIdentifiers.BeginUpdate()

        InitializeGridLayout(e.Layout)

        With e.Layout.Bands(0)
            .Columns("KBLPropertyName").CellActivation = Activation.NoEdit
            .Columns("KBLPropertyName").Header.Caption = GeneralSettingsFormStrings.KBLPropName_ColCaption
            .Columns("IdentificationCriteria").Header.Caption = GeneralSettingsFormStrings.IdentCriteria_ColCaption

            .Override.RowSelectorAppearance.Image = My.Resources.Ampersand.ToBitmap
        End With

        Me.ugUSSpliceIdentifiers.EndUpdate()
    End Sub

    Private Sub ugUSSpliceIdentifiers_MouseDown(sender As Object, e As MouseEventArgs) Handles ugUSSpliceIdentifiers.MouseDown
        Me.ugUSSpliceIdentifiers.ContextMenuStrip = Nothing

        If (e.Button = System.Windows.Forms.MouseButtons.Right) Then
            Dim element As UIElement = Me.ugUSSpliceIdentifiers.DisplayLayout.UIElement.LastElementEntered
            Dim row As UltraGridRow = TryCast(element.GetContext(GetType(UltraGridRow)), UltraGridRow)

            Me.ugUSSpliceIdentifiers.ContextMenuStrip = If(row IsNot Nothing AndAlso (row.IsActiveRow OrElse row.Selected), _contextMenuStrip, Nothing)

            If (Me.ugUSSpliceIdentifiers.ContextMenuStrip IsNot Nothing) Then
                Me.ugUSSpliceIdentifiers.ContextMenuStrip.Tag = Me.ugUSSpliceIdentifiers
            End If
        End If
    End Sub

    Private Sub ugWireColorCodes_AfterCellUpdate(sender As Object, e As CellEventArgs) Handles ugWireColorCodes.AfterCellUpdate
        _needsReloadMessage = True
    End Sub

    Private Sub ugWireColorCodes_AfterRowsDeleted(sender As Object, e As EventArgs) Handles ugWireColorCodes.AfterRowsDeleted
        _needsReloadMessage = True
    End Sub

    Private Sub ugWireColorCodes_BeforeCellUpdate(sender As Object, e As BeforeCellUpdateEventArgs) Handles ugWireColorCodes.BeforeCellUpdate
        If (e.NewValue.ToString = String.Empty) OrElse (_generalSettings.WireColorCodes.FindWireColorCode(e.NewValue.ToString) IsNot Nothing) Then
            e.Cancel = True
        End If
    End Sub

    Private Sub ugWireColorCodes_BeforeRowsDeleted(sender As Object, e As BeforeRowsDeletedEventArgs) Handles ugWireColorCodes.BeforeRowsDeleted
        e.DisplayPromptMsg = False

        For Each row As UltraGridRow In e.Rows
            If MessageBoxEx.ShowQuestion(String.Format(GeneralSettingsFormStrings.DelColorCode_Msg, row.Cells("ColorCode").Value)) = DialogResult.No Then
                e.Cancel = True
                Exit For
            End If
        Next
    End Sub

    Private Sub ugWireColorCodes_CellDataError(sender As Object, e As CellDataErrorEventArgs) Handles ugWireColorCodes.CellDataError
        e.RaiseErrorEvent = False
        e.RestoreOriginalValue = True
    End Sub

    Private Sub ugWireColorCodes_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugWireColorCodes.InitializeLayout
        Me.ugWireColorCodes.BeginUpdate()

        InitializeGridLayout(e.Layout)

        With e.Layout.Bands(0)
            .Columns("ColorCode").Header.Caption = GeneralSettingsFormStrings.ColorCode_ColCaption
            .Columns("ColorRGB").Hidden = True
            .Columns("Color").Style = ColumnStyle.Color
        End With

        Me.ugWireColorCodes.EndUpdate()
    End Sub

    Private Sub ugWireColorCodes_MouseDown(sender As Object, e As MouseEventArgs) Handles ugWireColorCodes.MouseDown
        Me.ugWireColorCodes.ContextMenuStrip = Nothing

        If (e.Button = System.Windows.Forms.MouseButtons.Right) Then
            Dim element As Infragistics.Win.UIElement = Me.ugWireColorCodes.DisplayLayout.UIElement.LastElementEntered
            Dim row As UltraGridRow = TryCast(element.GetContext(GetType(UltraGridRow)), UltraGridRow)

            Me.ugWireColorCodes.ContextMenuStrip = If(row IsNot Nothing AndAlso (row.IsActiveRow OrElse row.Selected), _contextMenuStrip, Nothing)

            If (Me.ugWireColorCodes.ContextMenuStrip IsNot Nothing) Then
                Me.ugWireColorCodes.ContextMenuStrip.Tag = Me.ugWireColorCodes
            End If
        End If
    End Sub

    Private Sub uneScaleFactorRedliningIndicator_ValueChanged(sender As Object, e As EventArgs) Handles uneScaleFactorRedliningIndicator.ValueChanged
        _generalSettings.RedliningStampIndicatorScaleFactor = CSng(Me.uneScaleFactorRedliningIndicator.Value)
    End Sub

    Private Sub uneToleranceFactor_ValueChanged(sender As Object, e As EventArgs) Handles uneMoveDistTolerance.ValueChanged
        _generalSettings.MoveDistanceToleranceForGraphicalCompare = CInt(Me.uneMoveDistTolerance.Value)
    End Sub

    Private Sub uosDrawingSelectionMode_ValueChanged(sender As Object, e As EventArgs) Handles uosDrawingSelectionMode.ValueChanged
        Select Case uosDrawingSelectionMode.CheckedIndex
            Case 0
                _generalSettings.ObjectTypeDependentSelection = False
            Case 1
                _generalSettings.ObjectTypeDependentSelection = True
        End Select
    End Sub

    Private Sub uosSelectCursor_ValueChanged(sender As Object, e As EventArgs) Handles uosSelectCursor.ValueChanged
        Select Case uosSelectCursor.CheckedIndex
            Case 0
                _generalSettings.ShowFullScreenAxisCursor = False
            Case 1
                _generalSettings.ShowFullScreenAxisCursor = True
        End Select
    End Sub

    Private Sub uce_LengthClass_ValueChanged(sender As Object, e As EventArgs) Handles uce_LengthClass.ValueChanged
        If _generalSettings.LengthClassDocument3D <> Me.CurrentLengthClassDocument3D Then
            _generalSettings.LengthClassDocument3D = Me.CurrentLengthClassDocument3D
            _needsReloadMessage = True
        End If
    End Sub

    Private Sub uceLogMsgVerbosity_ValueChanged(sender As Object, e As EventArgs) Handles uceLogMsgVerbosity.ValueChanged
        _generalSettings.ToastMessageVerbosity = CType(uceLogMsgVerbosity.Value, ToastMessageVerbosity)
    End Sub

    Private Sub chkUseDynamicBundles_CheckedChanged(sender As Object, e As EventArgs) Handles chkUseDynamicBundles.CheckedChanged
        _generalSettings.UseDynamicBundles = chkUseDynamicBundles.Checked
    End Sub

    Private Sub chkUseJTColors_CheckedChanged(sender As Object, e As EventArgs) Handles chkUseJTColors.CheckedChanged
        _generalSettings.UseJTColors = chkUseJTColors.Checked
    End Sub

    Private Sub ugEyeletIdentifiers_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugEyeletIdentifiers.InitializeLayout
        Me.ugUSSpliceIdentifiers.BeginUpdate()

        InitializeGridLayout(e.Layout)

        With e.Layout.Bands(0)
            .Columns("KBLPropertyName").CellActivation = Activation.NoEdit
            .Columns("KBLPropertyName").Header.Caption = GeneralSettingsFormStrings.KBLPropName_ColCaption
            .Columns("IdentificationCriteria").Header.Caption = GeneralSettingsFormStrings.IdentCriteria_ColCaption

            .Override.RowSelectorAppearance.Image = My.Resources.Ampersand.ToBitmap
        End With

        Me.ugUSSpliceIdentifiers.EndUpdate()
    End Sub

    Private Sub ugSpliceIdentifiers_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugSpliceIdentifiers.InitializeLayout
        Me.ugUSSpliceIdentifiers.BeginUpdate()

        InitializeGridLayout(e.Layout)

        With e.Layout.Bands(0)
            .Columns("KBLPropertyName").CellActivation = Activation.NoEdit
            .Columns("KBLPropertyName").Header.Caption = GeneralSettingsFormStrings.KBLPropName_ColCaption
            .Columns("IdentificationCriteria").Header.Caption = GeneralSettingsFormStrings.IdentCriteria_ColCaption

            .Override.RowSelectorAppearance.Image = My.Resources.Ampersand.ToBitmap
        End With

        Me.ugUSSpliceIdentifiers.EndUpdate()
    End Sub

    Friend ReadOnly Property DefaultLengthTypeChanged() As Boolean
        Get
            Return _defaultLengthTypeChanged
        End Get
    End Property

    Friend ReadOnly Property RecentFileListCleared() As Boolean
        Get
            Return _recentFileListCleared
        End Get
    End Property

    Public ReadOnly Property CurrentLengthClassDocument3D As E3.Lib.Model.LengthClass
        Get
            Return CType(uce_LengthClass.SelectedItem.DataValue, E3.Lib.Model.LengthClass)
        End Get
    End Property

    Private Sub ubtnAddSpliceIdent_Click(sender As Object, e As EventArgs) Handles ubtnAddSpliceIdent.Click
        Dim kblPropertyValueList As New ValueList

        If (Not ugInlinerIdentifiers.Rows.Where(Function(row) row.Cells("KBLPropertyName").Value.ToString = ObjectPropertyNameStrings.Id).Any()) Then
            kblPropertyValueList.ValueListItems.Add(ConnectorPropertyName.Id.ToString, ObjectPropertyNameStrings.Id)
        End If
        If (Not ugInlinerIdentifiers.Rows.Where(Function(row) row.Cells("KBLPropertyName").Value.ToString = ObjectPropertyNameStrings.AliasId).Any()) Then
            kblPropertyValueList.ValueListItems.Add(ConnectorPropertyName.Alias_id.ToString, ObjectPropertyNameStrings.AliasId)
        End If
        If (Not ugInlinerIdentifiers.Rows.Where(Function(row) row.Cells("KBLPropertyName").Value.ToString = ObjectPropertyNameStrings.Description).Any()) Then
            kblPropertyValueList.ValueListItems.Add(ConnectorPropertyName.Description.ToString, ObjectPropertyNameStrings.Description)
        End If
        If (Not ugInlinerIdentifiers.Rows.Where(Function(row) row.Cells("KBLPropertyName").Value.ToString = ObjectPropertyNameStrings.InstallationInformation).Any()) Then
            kblPropertyValueList.ValueListItems.Add(ConnectorPropertyName.Installation_Information.ToString, ObjectPropertyNameStrings.InstallationInformation)
        End If

        If (kblPropertyValueList.ValueListItems.Count <> 0) Then
            Using inputForm As New InputForm(GeneralSettingsFormStrings.AddSpliceIdent_InputBoxPrompt, GeneralSettingsFormStrings.SelKBLConn_InputBoxTitle, String.Empty, kblPropertyValueList)
                If (inputForm.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then

                    Me.ugInlinerIdentifiers.BeginUpdate()
                    _generalSettings.SpliceIdentifiers.Add(New SpliceIdentifier(inputForm.Response, "*"))
                    Me.ugInlinerIdentifiers.EndUpdate()
                    _needsReloadMessage = True
                End If
            End Using
        Else
            MessageBoxEx.ShowWarning(GeneralSettingsFormStrings.NoFurtherConnPropAvailable_Msg)
        End If
    End Sub

    Private Sub ubtnAddEyeletIdent_Click(sender As Object, e As EventArgs) Handles ubtnAddEyeletIdent.Click
        Dim kblPropertyValueList As New ValueList

        If (Not ugInlinerIdentifiers.Rows.Where(Function(row) row.Cells("KBLPropertyName").Value.ToString = ObjectPropertyNameStrings.Id).Any()) Then
            kblPropertyValueList.ValueListItems.Add(ConnectorPropertyName.Id.ToString, ObjectPropertyNameStrings.Id)
        End If
        If (Not ugInlinerIdentifiers.Rows.Where(Function(row) row.Cells("KBLPropertyName").Value.ToString = ObjectPropertyNameStrings.AliasId).Any()) Then
            kblPropertyValueList.ValueListItems.Add(ConnectorPropertyName.Alias_id.ToString, ObjectPropertyNameStrings.AliasId)
        End If
        If (Not ugInlinerIdentifiers.Rows.Where(Function(row) row.Cells("KBLPropertyName").Value.ToString = ObjectPropertyNameStrings.Description).Any()) Then
            kblPropertyValueList.ValueListItems.Add(ConnectorPropertyName.Description.ToString, ObjectPropertyNameStrings.Description)
        End If
        If (Not ugInlinerIdentifiers.Rows.Where(Function(row) row.Cells("KBLPropertyName").Value.ToString = ObjectPropertyNameStrings.InstallationInformation).Any()) Then
            kblPropertyValueList.ValueListItems.Add(ConnectorPropertyName.Installation_Information.ToString, ObjectPropertyNameStrings.InstallationInformation)
        End If

        If (kblPropertyValueList.ValueListItems.Count <> 0) Then
            Using inputForm As New InputForm(GeneralSettingsFormStrings.AddEyeletIdent_InputBoxPrompt, GeneralSettingsFormStrings.SelKBLConn_InputBoxTitle, String.Empty, kblPropertyValueList)
                If (inputForm.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then
                    Me.ugInlinerIdentifiers.BeginUpdate()
                    _generalSettings.EyeletIdentifiers.Add(New EyeletIdentifier(inputForm.Response, "*"))
                    Me.ugInlinerIdentifiers.EndUpdate()
                    _needsReloadMessage = True
                End If
            End Using
        Else
            MessageBox.Show(GeneralSettingsFormStrings.NoFurtherConnPropAvailable_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub ugSpliceIdentifiers_AfterCellUpdate(sender As Object, e As CellEventArgs) Handles ugSpliceIdentifiers.AfterCellUpdate
        _needsReloadMessage = True
    End Sub

    Private Sub ugEyeletIdentifiers_AfterCellUpdate(sender As Object, e As CellEventArgs) Handles ugEyeletIdentifiers.AfterCellUpdate
        _needsReloadMessage = True
    End Sub

    Private Sub ugEyeletIdentifiers_AfterRowsDeleted(sender As Object, e As EventArgs) Handles ugEyeletIdentifiers.AfterRowsDeleted
        _needsReloadMessage = True
    End Sub

    Private Sub ugSpliceIdentifiers_AfterRowsDeleted(sender As Object, e As EventArgs) Handles ugSpliceIdentifiers.AfterRowsDeleted
        _needsReloadMessage = True
    End Sub

    Private Sub ugEyeletIdentifiers_BeforeCellUpdate(sender As Object, e As BeforeCellUpdateEventArgs) Handles ugEyeletIdentifiers.BeforeCellUpdate
        If (e.Cell.Column.Key = "IdentificationCriteria") AndAlso (e.NewValue.ToString = String.Empty) Then
            e.Cancel = True
        End If

        Try
            Dim i As New EyeletIdentifier("", e.NewValue.ToString)
        Catch ex As Exception
            e.Cancel = True
            ex.ShowMessageBox(Me, String.Format(GeneralSettingsFormStrings.RegExInvalid, e.NewValue.ToString))
        End Try

    End Sub

    Private Sub ugSpliceIdentifiers_BeforeCellUpdate(sender As Object, e As BeforeCellUpdateEventArgs) Handles ugSpliceIdentifiers.BeforeCellUpdate
        If (e.Cell.Column.Key = "IdentificationCriteria") AndAlso (e.NewValue.ToString = String.Empty) Then
            e.Cancel = True
        End If

        Try
            Dim id As New SpliceIdentifier("", e.NewValue.ToString)
        Catch ex As Exception
            e.Cancel = True
            ex.ShowMessageBox(Me, String.Format(GeneralSettingsFormStrings.RegExInvalid, e.NewValue.ToString))

        End Try
    End Sub

    Private Sub ugEyeletIdentifiers_CellDataError(sender As Object, e As CellDataErrorEventArgs) Handles ugEyeletIdentifiers.CellDataError
        e.RaiseErrorEvent = False
        e.RestoreOriginalValue = True
        e.StayInEditMode = False
    End Sub

    Private Sub ugSpliceIdentifiers_CellDataError(sender As Object, e As CellDataErrorEventArgs) Handles ugSpliceIdentifiers.CellDataError
        e.RaiseErrorEvent = False
        e.RestoreOriginalValue = True
        e.StayInEditMode = False
    End Sub

    Private Sub ugSpliceIdentifiers_MouseDown(sender As Object, e As MouseEventArgs) Handles ugSpliceIdentifiers.MouseDown
        Me.ugSpliceIdentifiers.ContextMenuStrip = Nothing

        If (e.Button = System.Windows.Forms.MouseButtons.Right) Then
            Dim element As UIElement = Me.ugSpliceIdentifiers.DisplayLayout.UIElement.LastElementEntered
            Dim row As UltraGridRow = TryCast(element.GetContext(GetType(UltraGridRow)), UltraGridRow)

            Me.ugSpliceIdentifiers.ContextMenuStrip = If(row IsNot Nothing AndAlso (row.IsActiveRow OrElse row.Selected), _contextMenuStrip, Nothing)

            If (Me.ugSpliceIdentifiers.ContextMenuStrip IsNot Nothing) Then
                Me.ugSpliceIdentifiers.ContextMenuStrip.Tag = Me.ugSpliceIdentifiers
            End If
        End If
    End Sub

    Private Sub ugEyeletIdentifiers_MouseDown(sender As Object, e As MouseEventArgs) Handles ugEyeletIdentifiers.MouseDown
        Me.ugEyeletIdentifiers.ContextMenuStrip = Nothing

        If (e.Button = System.Windows.Forms.MouseButtons.Right) Then
            Dim element As UIElement = Me.ugEyeletIdentifiers.DisplayLayout.UIElement.LastElementEntered
            Dim row As UltraGridRow = TryCast(element.GetContext(GetType(UltraGridRow)), UltraGridRow)

            Me.ugEyeletIdentifiers.ContextMenuStrip = If(row IsNot Nothing AndAlso (row.IsActiveRow OrElse row.Selected), _contextMenuStrip, Nothing)

            If (Me.ugEyeletIdentifiers.ContextMenuStrip IsNot Nothing) Then
                Me.ugEyeletIdentifiers.ContextMenuStrip.Tag = Me.ugEyeletIdentifiers
            End If
        End If
    End Sub

    Private Sub ugSpliceIdentifiers_BeforeRowsDeleted(sender As Object, e As BeforeRowsDeletedEventArgs) Handles ugSpliceIdentifiers.BeforeRowsDeleted
        e.DisplayPromptMsg = False

        For Each row As UltraGridRow In e.Rows
            If (MessageBox.Show(String.Format(GeneralSettingsFormStrings.DelSpliceIdent_Msg, row.Cells("KBLPropertyName").Value), [Shared].MSG_BOX_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = MsgBoxResult.No) Then
                e.Cancel = True
                Exit For
            End If
        Next
    End Sub

    Private Sub ugEyeletIdentifiers_BeforeRowsDeleted(sender As Object, e As BeforeRowsDeletedEventArgs) Handles ugEyeletIdentifiers.BeforeRowsDeleted
        e.DisplayPromptMsg = False

        For Each row As UltraGridRow In e.Rows
            If (MessageBoxEx.ShowQuestion(String.Format(GeneralSettingsFormStrings.DelEyeletIdent_Msg, row.Cells("KBLPropertyName").Value)) = MsgBoxResult.No) Then
                e.Cancel = True
                Exit For
            End If
        Next
    End Sub

    Private Sub utEcuConnectorSeparator_AfterExitEditMode(sender As Object, e As EventArgs) Handles utEcuConnectorSeparator.AfterExitEditMode
        _generalSettings.EcuConnectorIdentifier = New EcuConnectorIdentifier(ConnectorPropertyName.Id, utEcuConnectorSeparator.Text)
        _needsReloadMessage = True
    End Sub

    Private Sub utEcuConnectorSeparator_BeforeExitEditMode(sender As Object, e As Infragistics.Win.BeforeExitEditModeEventArgs) Handles utEcuConnectorSeparator.BeforeExitEditMode
        Try
            Dim i As New EcuConnectorIdentifier("", utEcuConnectorSeparator.Text)
        Catch ex As Exception
            e.Cancel = True
            ex.ShowMessageBox(Me, String.Format(GeneralSettingsFormStrings.RegExInvalid, utEcuConnectorSeparator.Text))
        End Try
    End Sub

    Public ReadOnly Property FilePreviewAddonsChecked As Boolean
        Get
            Return (Me.chkFilePreviewAddon?.Checked).GetValueOrDefault
        End Get
    End Property

End Class