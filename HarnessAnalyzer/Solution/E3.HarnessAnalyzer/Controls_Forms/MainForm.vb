Imports System.ComponentModel
Imports System.IO
Imports System.IO.Compression
Imports System.Reflection
Imports Infragistics.Win.UltraWinDock
Imports Infragistics.Win.UltraWinTabbedMdi
Imports Infragistics.Win.UltraWinToolbars
Imports Zuken.E3.HarnessAnalyzer.Schematics.Converter.Kbl
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.HarnessAnalyzer.Settings.QualityStamping.Specification
Imports Zuken.E3.HarnessAnalyzer.Shared
Imports Zuken.E3.Lib.IO.E3XML
Imports Zuken.E3.Lib.IO.Files.Hcv
Imports Zuken.E3.Lib.Schema.Kbl.Properties

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class MainForm
    Implements IMessageFilter
    Implements IHarnessAnalyzerSettingsProvider

    <ObfuscationAttribute(Feature:="renaming")>
    Public Event DocumentOpenTotalFinished(sender As Object, e As DocumentOpenFinshedEventArgs) ' hint event raised when document open + additional async processes are finished

    Private _currentMaximizePane As DockableControlPane
    Private _diameterSettings As DiameterSettings

    Private _entireRoutingPath As Dictionary(Of String, RoutingPath)
    Private _fontAndShapeFilesDir As String
    Private _generalSettings As GeneralSettings
    Private _isOverMaximize As Boolean
    Private _qmStampSpecifications As QMStampSpecifications
    Private _weightSettings As WeightSettings
    Private _e3xmlSettings As E3XmlMapper
    Private _activeProgressId As Guid = Guid.Empty
    Private _topoHubWasShown As Boolean = False

    ' HINT: Sent when the contents of the clipboard have changed
    Private Const WM_CLIPBOARDUPDATE As Integer = &H31D

    Private WithEvents _activeDocument As DocumentForm
    Private WithEvents _checkTimer As Timer

    Private WithEvents _toastManager As LogHubToastManager
    Private WithEvents _mainStateMachine As MainStateMachine
    Private WithEvents _searchMachine As SearchMachine
    Private WithEvents _topologyHub As TopologyHub
    Private WithEvents _schematicsView As Schematics.Controls.ViewControl
    Private WithEvents _cavityCheckForm As Checks.Cavities.CavityCheckForm
    Private WithEvents _documentViews As New Checks.Cavities.Views.Document.DocumentViewCollection
    Private WithEvents MainFormController As D3D.MainFormController

    Private _panes As TabToolPanesCollection

    Private _project As New HarnessAnalyzerProject ' HINT: future structure preperation (this structure is currently only used for newly introduced 3D -documents) to (maybe) get rid of the document GUI-access calls (tabs, forms) 

    Public Sub New()
        InitializeComponent()

        ' Add clipboard listener to MainForm
        AttachToClipboard()
        Application.AddMessageFilter(Me)
        _project.SynchronizationContext = Me
    End Sub

    Private Sub Initialize()
#If DEBUG Or CONFIG = "Debug" Then
        If My.Settings.LastWindowPosition <> System.Drawing.Point.Empty Then
            If Screen.AllScreens.Any(Function(s) s.WorkingArea.Contains(My.Settings.LastWindowPosition)) Then
                Me.Location = My.Settings.LastWindowPosition
            End If
        End If
#End If

        Me.Icon = My.Resources.E3_HarnessAnalyzer
        Me.Text = String.Format("{0} 20{1}", My.Application.Info.Title, My.Application.Info.Version.ToString(2))
        _toastManager = New LogHubToastManager(Me) ' HINT: create first to ensure that the popup warnings will be visible if next things need them

        Dim ressSearchForm As New Global.System.Resources.ResourceManager("Zuken.E3.HarnessAnalyzer.SearchForm", Me.GetType.Assembly)
        Dim ressCavityCheckForm As New Global.System.Resources.ResourceManager("Zuken.E3.HarnessAnalyzer.Checks.Cavities.CavityCheckForm", Me.GetType.Assembly)

        _panes = New TabToolPanesCollection(udmMain) ' HINT: needed before initializing settings because the collection is needed for creation of dockable control panes
        _Tools = New UtMainTools(utmMain)

        CheckVersion()

        Dim retryInit As Boolean
        Do
            retryInit = False
            Try
                ExecWithStat(AddressOf InitializeDiameterSettings)
                ExecWithStat(AddressOf InitializeWeightSettings)
                ExecWithStat(AddressOf InitializeE3XmlSettings)
                ExecWithStat(AddressOf InitializeFontAndShapeFiles)
                _generalSettings = Settings.GeneralSettings.Current.GeneralSettings
                ExecWithStat(AddressOf InitializeGridAppearances)
                ExecWithStat(AddressOf InitializeStampSpecifications)
                ExecWithStat(AddressOf InitializeTopologyHub)
                ExecWithStat(AddressOf InitializeSchematicsView)
            Catch ex As Exception
#If DEBUG Or CONFIG = "Debug" Then
                If ex.ShowMessageBox(Me, $"Error initializing detected! {vbCrLf}Possible fix: delete Application settings-folder's and retry ? {vbCrLf}(this message is debug-only!)", MessageBoxButtons.YesNo) = DialogResult.Yes Then
                    DirectoryEx.TryDelete(Application.UserAppDataPath, True)
                    DirectoryEx.TryDelete(Application.CommonAppDataPath, True)
                    If Not Directory.Exists([Shared].Utilities.GetApplicationSettingsPath) OrElse Not DirectoryEx.TryDelete([Shared].Utilities.GetApplicationSettingsPath, True) Then
                        If Not Directory.Exists([Shared].Utilities.GetApplicationSettingsPath) Then
                            MessageBoxEx.ShowError(Me, $"Error: directory ""{[Shared].Utilities.GetApplicationSettingsPath}"" does not exist!. Aborting load application now...")
                        Else
                            MessageBoxEx.ShowError(Me, $"Failed: failed to delete folder: {[Shared].Utilities.GetApplicationSettingsPath}. Aborting load application now...")
                        End If
                        Environment.Exit(-1)
                        Return
                    Else
                        retryInit = True
                    End If
                Else
                    Environment.Exit(-1)
                End If
#End If
            End Try
        Loop Until Not retryInit

        CType(My.Application.SplashScreen, SplashScreen).SetMessageSuffix($"Initializing ""{ressCavityCheckForm.GetString("$this.Text")}""")
        _cavityCheckForm = New Checks.Cavities.CavityCheckForm(_documentViews, Me)

        CType(My.Application.SplashScreen, SplashScreen).SetMessageSuffix($"Initializing ""{ressSearchForm.GetString("$this.Text")}""")
        _searchMachine = New SearchMachine(Me)

        CType(My.Application.SplashScreen, SplashScreen).SetMessageSuffix("Initializing ""[Main]""")
        _mainStateMachine = New MainStateMachine(Me)

        MainFormController = New D3D.MainFormController(Me) ' HINT: must be initialized after mainstatemachine and Main.Project was set

        _checkTimer = New Timer()
    End Sub

    Private Sub ExecWithStat(action As Action)
        If My.Application.SplashScreen?.Visible Then
            CType(My.Application.SplashScreen, SplashScreen).SetMessageSuffix($"""{action.Method.Name.Replace("Initialize", "Initializing ")}""")
        End If
        action.Invoke
    End Sub

    Private Sub FixMySettings()
        If My.Settings.LastTransparency > 100 Then
            My.Settings.LastTransparency = 100
        End If
        If My.Settings.LastTransparencyJT > 100 Then
            My.Settings.LastTransparencyJT = 100
        End If
    End Sub

    <Obfuscation(Feature:="renaming", Exclude:=True)> 'HINT: this is needed because the init-text is extracted from the method name -> this could be done better by replacing it with seperate resource-strings
    Private Sub InitializeDiameterSettings()
        Dim diameterSettingsConfigFile As String = GetDiameterSettingsConfigurationFile()
        Dim errorOnLoad As Boolean = False

        If (IO.File.Exists(diameterSettingsConfigFile)) Then
            _diameterSettings = DiameterSettings.LoadFromFile(diameterSettingsConfigFile)

            If (_diameterSettings Is Nothing) Then
                AppInitManager.ErrorManager.ShowMsgBoxErrorOrWriteConsole(String.Format(MainFormStrings.ErrorLoadDiaSettings_Msg, vbCrLf, DiameterSettings.LoadError.Message))
                errorOnLoad = True
            End If
        End If

        If (_diameterSettings Is Nothing) Then
            ' HINT: Check for a default settings file in case this was supplied by the installation package
            Dim defaultFullPath As String = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), DEFAULT_DIAMETER_SETTINGS_FILE)
            If (IO.File.Exists(defaultFullPath)) Then
                IO.File.Copy(defaultFullPath, diameterSettingsConfigFile, True)
                _diameterSettings = DiameterSettings.LoadFromFile(diameterSettingsConfigFile)

                If (_diameterSettings Is Nothing) Then
                    errorOnLoad = True
                End If
            End If

            If (_diameterSettings Is Nothing) Then
                _diameterSettings = New DiameterSettings
                _diameterSettings.ResetToDefaults()

                If (Not errorOnLoad) Then
                    _diameterSettings.Save(diameterSettingsConfigFile)
                End If

            End If
        ElseIf (_diameterSettings.GenericDiameterFormulaParameters IsNot Nothing) Then
            With _diameterSettings.GenericDiameterFormulaParameters
                If (.BDL_Coeff1 = 0) Then .BDL_Coeff1 = 1.425
                If (.BDL_Corr = 0) Then .BDL_Corr = 1.0
                If (.BDL_Exp = 0) Then .BDL_Exp = 0.5
            End With
        End If
    End Sub

    <Obfuscation(Feature:="renaming", Exclude:=True)> 'HINT: this is needed because the init-text is extracted from the method name -> this could be done better by replacing it with separate resource-strings
    Public Sub InitializeE3XmlSettings()
        Dim errorOnLoad As Boolean = False
        Dim mappedPropertiesFromE3toModelConfigFile As String = GetMappedPropertiesFromE3toModelConfigurationFile()

        If (IO.File.Exists(mappedPropertiesFromE3toModelConfigFile)) Then
            Dim fi As New FileInfo(mappedPropertiesFromE3toModelConfigFile)
            If (fi.Length > 0) Then
                _e3xmlSettings = E3XmlMapper.LoadFromFile(mappedPropertiesFromE3toModelConfigFile)
                If (_e3xmlSettings Is Nothing) Then
                    AppInitManager.ErrorManager.ShowMsgBoxErrorOrWriteConsole(String.Format(MainFormStrings.ErrorLoadE3XmlSettings_Msg, vbCrLf, E3XmlMapper.LoadError.Message))
                    errorOnLoad = True
                End If
            End If
        End If

        If (_e3xmlSettings Is Nothing) Then
            ' HINT: Check for a default settings file in case this was supplied by the installation package
            Dim defaultFullPath As String = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), DEFAULT_E3XML_SETTINGS_FILE)
            If (IO.File.Exists(defaultFullPath)) Then
                IO.File.Copy(defaultFullPath, mappedPropertiesFromE3toModelConfigFile, True)
                _e3xmlSettings = E3XmlMapper.LoadFromFile(mappedPropertiesFromE3toModelConfigFile)

                If (_e3xmlSettings Is Nothing) Then
                    errorOnLoad = True
                End If
            End If

            If (_e3xmlSettings Is Nothing) Then
                _e3xmlSettings = E3XmlMapper.InitializeMapperToDefault()

                If (Not errorOnLoad) Then
                    _e3xmlSettings.SaveToFile(mappedPropertiesFromE3toModelConfigFile)
                End If
            End If
        End If

        E3XmltoModelResolver.SetMapper(_e3xmlSettings)
    End Sub

    <Obfuscation(Feature:="renaming", Exclude:=True)> 'HINT: this is needed because the init-text is extracted from the method name -> this could be done better by replacing it with separate resource-strings
    Private Sub InitializeWeightSettings()
        Dim errorOnLoad As Boolean = False
        Dim weightSettingsConfigFile As String = GetWeightSettingsConfigurationFile()

        If (IO.File.Exists(weightSettingsConfigFile)) Then
            Dim fi As New FileInfo(weightSettingsConfigFile)
            If (fi.Length > 0) Then
                _weightSettings = WeightSettings.LoadFromFile(weightSettingsConfigFile)
                If (_weightSettings Is Nothing) Then
                    AppInitManager.ErrorManager.ShowMsgBoxErrorOrWriteConsole(String.Format(MainFormStrings.ErrorLoadWeightSettings_Msg, vbCrLf, WeightSettings.LoadError.Message))
                    errorOnLoad = True
                End If
            End If
        End If

        If (_weightSettings Is Nothing) Then
            ' HINT: Check for a default settings file in case this was supplied by the installation package
            Dim defaultFullPath As String = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), DEFAULT_WEIGHT_SETTINGS_FILE)
            If (IO.File.Exists(defaultFullPath)) Then
                IO.File.Copy(defaultFullPath, weightSettingsConfigFile, True)
                _weightSettings = WeightSettings.LoadFromFile(weightSettingsConfigFile)

                If (_weightSettings Is Nothing) Then
                    errorOnLoad = True
                End If
            End If

            If (_weightSettings Is Nothing) Then
                _weightSettings = New WeightSettings

                If (Not errorOnLoad) Then
                    _weightSettings.SaveTo(weightSettingsConfigFile)
                End If
            End If
        End If
    End Sub

    Public ReadOnly Property Project As HarnessAnalyzerProject
        Get
            Return _project
        End Get
    End Property

    <Obfuscation(Feature:="renaming", Exclude:=True)> 'HINT: this is needed because the init-text is extracted from the method name -> this could be done better by replacing it with separate resource-strings
    Private Sub InitializeFontAndShapeFiles()
        _fontAndShapeFilesDir = [Shared].Utilities.GetOrCreateApplicationDirectoryName(E3.Lib.Converter.Svg.Defaults.FONT_AND_SHAPE_DIR_NAME)

#If DEBUG Or CONFIG = "Debug" Then 'HINT: in release this resource is not included here, because we are not allowed to distribute to Autodesk font files
        Dim dir As New DirectoryInfo(_fontAndShapeFilesDir)
        Dim shxFiles As FileInfo() = dir.GetFiles("*" + KnownFile.SHX, SearchOption.TopDirectoryOnly)
        If shxFiles.Length = 0 Then
            'We have no shx files in the dir => we apply the default font files from our resources
            Dim s As System.IO.Stream = Assembly.GetEntryAssembly.GetManifestResourceStream(GetType(MainForm).Namespace + ".DefaultFonts.zip")
            If s IsNot Nothing Then
                Using s
                    Dim fontFilesZa As New ZipArchive(s)
                    For Each entry As ZipArchiveEntry In fontFilesZa.Entries
                        Using fs As New FileStream(IO.Path.Combine(_fontAndShapeFilesDir, entry.Name), FileMode.Create)
                            entry.Open.CopyTo(fs)
                        End Using
                    Next
                End Using
            End If
        End If
#End If

    End Sub

    <Obfuscation(Feature:="renaming", Exclude:=True)> 'HINT: this is needed because the init-text is extracted from the method name -> this could be done better by replacing it with separate resource-strings
    Private Sub InitializeGridAppearances()
        InitializeGridAppearancesCore()

        Dim currentApplicationVersion As Version = My.Application.Info.Version

        Dim accessoryGridAppearance As AccessoryGridAppearance = GridAppearance.All.OfType(Of AccessoryGridAppearance).Single
        With accessoryGridAppearance
            If (.GridTable?.HasOlderVersion(currentApplicationVersion)).GetValueOrDefault Then
                If (String.IsNullOrEmpty(.GridTable.Version)) Then
                    .GridTable.Description = KblObjectType.Accessory_occurrence.ToString

                    If (.GridTable.GridColumns.FindGridColumn(AccessoryPropertyName.Installation_Information).Count > 0) Then
                        .GridTable.GridColumns.FindGridColumn(AccessoryPropertyName.Installation_Information).Single.KBLPropertyName = AccessoryPropertyName.Installation_Information.ToString
                    End If
                    If (.GridTable.GridColumns.FindGridColumn(AccessoryPropertyName.Alias_id).Count > 0) Then
                        .GridTable.GridColumns.FindGridColumn(AccessoryPropertyName.Alias_id).Single.KBLPropertyName = AccessoryPropertyName.Alias_id.ToString
                    End If
                    If (.GridTable.GridColumns.FindGridColumn(PartPropertyName.Part_number_type).Count > 0) Then
                        .GridTable.GridColumns.FindGridColumn(PartPropertyName.Part_alias_ids).Single.KBLPropertyName = PartPropertyName.Part_alias_ids.ToString
                    End If
                    If (.GridTable.GridColumns.FindGridColumn(PartPropertyName.External_references).Count > 0) Then
                        .GridTable.GridColumns.FindGridColumn(PartPropertyName.External_references).Single.KBLPropertyName = PartPropertyName.External_references.ToString
                    End If
                    If (.GridTable.GridColumns.FindGridColumn(PartPropertyName.Change).Count > 0) Then
                        .GridTable.GridColumns.FindGridColumn(PartPropertyName.Change).Single.KBLPropertyName = PartPropertyName.Change.ToString
                    End If
                End If

                If (.GridTable.HasOlderVersion(New Version(20, 1))) Then
                    If (.GridTable.GridColumns.FindGridColumn(PartPropertyName.Part_number_type).Count = 0) Then
                        .GridTable.GridColumns.AddNew(accessoryGridAppearance, ObjectPropertyNameStrings.PartNumberType, 20, True, True, PartPropertyName.Part_number_type.ToString)
                    End If
                End If

                If (.GridTable.HasOlderVersion(New Version(23, 1))) Then
                    If (.GridTable.GridColumns.FindGridColumn(SegmentPropertyName.Start_node).Count = 0) Then
                        .GridTable.GridColumns.AddNew(accessoryGridAppearance, ObjectPropertyNameStrings.StartVertex, 21, True, True, SegmentPropertyName.Start_node.ToString)
                    End If
                    If (.GridTable.GridColumns.FindGridColumn(AccessoryPropertyName.SegmentLocation).Count = 0) Then
                        .GridTable.GridColumns.AddNew(accessoryGridAppearance, ObjectPropertyNameStrings.SegmentLocation, 22, True, True, AccessoryPropertyName.SegmentLocation.ToString)
                    End If
                    If (.GridTable.GridColumns.FindGridColumn(AccessoryPropertyName.SegmentAbsolute_location).Count = 0) Then
                        .GridTable.GridColumns.AddNew(accessoryGridAppearance, ObjectPropertyNameStrings.LocationAbsolute, 23, True, True, AccessoryPropertyName.SegmentAbsolute_location.ToString)
                    End If
                End If

                .GridTable.Version = currentApplicationVersion.ToString(2)
                .Save()
            End If
        End With

        Dim approvalGridAppearance As ApprovalGridAppearance = GridAppearance.All.OfType(Of ApprovalGridAppearance).Single
        With approvalGridAppearance
            If (.GridTable?.HasOlderVersion(currentApplicationVersion)).GetValueOrDefault Then
                ' TODO: Further updates

                .GridTable.Version = currentApplicationVersion.ToString(2)
                .Save()
            End If
        End With

        Dim assemblyPartGridAppearance As AssemblyPartGridAppearance = GridAppearance.All.OfType(Of AssemblyPartGridAppearance).Single
        With assemblyPartGridAppearance
            If (.GridTable?.HasOlderVersion(currentApplicationVersion)).GetValueOrDefault Then
                If (.GridTable.HasOlderVersion(New Version(20, 1))) Then
                    If (.GridTable.GridColumns.FindGridColumn(PartPropertyName.Part_number_type).Count = 0) Then
                        .GridTable.GridColumns.AddNew(assemblyPartGridAppearance, ObjectPropertyNameStrings.PartNumberType, 19, True, True, PartPropertyName.Part_number_type.ToString)
                    End If
                End If

                .GridTable.Version = currentApplicationVersion.ToString(2)
                .Save()
            End If
        End With

        Dim cableGridAppearance As CableGridAppearance = GridAppearance.All.OfType(Of CableGridAppearance).Single
        With cableGridAppearance
            If (.GridTable?.HasOlderVersion(currentApplicationVersion)).GetValueOrDefault Then
                If (String.IsNullOrEmpty(.GridTable.Version)) Then
                    .GridTable.Description = KblObjectType.Special_wire_occurrence.ToString

                    If (.GridTable.GridColumns.FindGridColumn(CablePropertyName.CoverColours).Count > 0) Then
                        .GridTable.GridColumns.FindGridColumn(CablePropertyName.CoverColours).Single.KBLPropertyName = CablePropertyName.CoverColours.ToString
                    End If
                    If (.GridTable.GridColumns.FindGridColumn(CablePropertyName.Length_Information).Count > 0) Then
                        .GridTable.GridColumns.FindGridColumn(CablePropertyName.Length_Information).Single.KBLPropertyName = CablePropertyName.Length_Information.ToString
                    End If
                    If (.GridTable.GridColumns.FindGridColumn(PartPropertyName.Part_alias_ids).Count > 0) Then
                        .GridTable.GridColumns.FindGridColumn(PartPropertyName.Part_alias_ids).Single.KBLPropertyName = PartPropertyName.Part_alias_ids.ToString
                    End If
                    If (.GridTable.GridColumns.FindGridColumn(PartPropertyName.External_references).Count > 0) Then
                        .GridTable.GridColumns.FindGridColumn(PartPropertyName.External_references).Single.KBLPropertyName = PartPropertyName.External_references.ToString
                    End If
                    If (.GridTable.GridColumns.FindGridColumn(PartPropertyName.Change).Count > 0) Then
                        .GridTable.GridColumns.FindGridColumn(PartPropertyName.Change).Single.KBLPropertyName = PartPropertyName.Change.ToString
                    End If
                    If (.GridTable.GridColumns.FindGridColumn(CablePropertyName.Installation_Information).Count > 0) Then
                        .GridTable.GridColumns.FindGridColumn(CablePropertyName.Installation_Information).Single.KBLPropertyName = CablePropertyName.Installation_Information.ToString
                    End If

                    .GridTable.GridSubTable.Description = KblObjectType.Core_occurrence.ToString

                    If (.GridTable.GridSubTable.GridColumns.FindGridColumn(WirePropertyName.Core_Colour).Count > 0) Then
                        .GridTable.GridSubTable.GridColumns.FindGridColumn(WirePropertyName.Core_Colour).Single.KBLPropertyName = WirePropertyName.Core_Colour.ToString
                    End If
                    If (.GridTable.GridSubTable.GridColumns.FindGridColumn(WirePropertyName.Length_information).Count > 0) Then
                        .GridTable.GridSubTable.GridColumns.FindGridColumn(WirePropertyName.Length_information).Single.KBLPropertyName = WirePropertyName.Length_information.ToString
                    End If
                End If

                If (.GridTable.HasOlderVersion(New Version(20, 1))) Then
                    If (.GridTable.GridColumns.FindGridColumn(PartPropertyName.Part_number_type).Count = 0) Then
                        .GridTable.GridColumns.AddNew(cableGridAppearance, ObjectPropertyNameStrings.PartNumberType, 24, True, True, PartPropertyName.Part_number_type.ToString)
                    End If
                End If

                .GridTable.Version = currentApplicationVersion.ToString(2)
                .Save()
            End If
        End With

        Dim changeDescriptionGridAppearance As ChangeDescriptionGridAppearance = GridAppearance.All.OfType(Of ChangeDescriptionGridAppearance).Single
        With changeDescriptionGridAppearance
            If (.GridTable?.HasOlderVersion(currentApplicationVersion)).GetValueOrDefault Then
                If (.GridTable.HasOlderVersion(New Version(20, 1))) Then
                    If (.GridTable.GridColumns.FindGridColumn(ChangeDescriptionPropertyName.External_references).Count = 0) Then
                        .GridTable.GridColumns.AddNew(changeDescriptionGridAppearance, ObjectPropertyNameStrings.ExternalReferences, 10, True, True, ChangeDescriptionPropertyName.External_references.ToString)
                    End If
                End If

                .GridTable.Version = currentApplicationVersion.ToString(2)
                .Save()
            End If
        End With

        Dim componentBoxGridAppearance As ComponentBoxGridAppearance = GridAppearance.All.OfType(Of ComponentBoxGridAppearance).Single
        With componentBoxGridAppearance
            If (.GridTable?.HasOlderVersion(currentApplicationVersion)).GetValueOrDefault Then
                If (.GridTable.HasOlderVersion(New Version(20, 1))) Then
                    If (.GridTable.GridColumns.FindGridColumn(PartPropertyName.Part_number_type).Count = 0) Then
                        .GridTable.GridColumns.AddNew(componentBoxGridAppearance, ObjectPropertyNameStrings.PartNumberType, 19, True, True, PartPropertyName.Part_number_type.ToString)
                    End If
                End If

                .GridTable.Version = currentApplicationVersion.ToString(2)
                .Save()
            End If
        End With

        Dim componentGridAppearance As ComponentGridAppearance = GridAppearance.All.OfType(Of ComponentGridAppearance).Single
        With componentGridAppearance
            If (.GridTable?.HasOlderVersion(currentApplicationVersion)).GetValueOrDefault Then
                If (String.IsNullOrEmpty(.GridTable.Version)) Then
                    .GridTable.Description = KblObjectType.Component_occurrence.ToString

                    If (.GridTable.GridColumns.FindGridColumn(PartPropertyName.External_references).Count > 0) Then
                        .GridTable.GridColumns.FindGridColumn(PartPropertyName.External_references).Single.KBLPropertyName = PartPropertyName.External_references.ToString
                    End If
                    If (.GridTable.GridColumns.FindGridColumn(ComponentPropertyName.Alias_id).Count > 0) Then
                        .GridTable.GridColumns.FindGridColumn(ComponentPropertyName.Alias_id).Single.KBLPropertyName = ComponentPropertyName.Alias_id.ToString
                    End If
                    If (.GridTable.GridColumns.FindGridColumn(PartPropertyName.Part_alias_ids).Count > 0) Then
                        .GridTable.GridColumns.FindGridColumn(PartPropertyName.Part_alias_ids).Single.KBLPropertyName = PartPropertyName.Part_alias_ids.ToString
                    End If
                    If (.GridTable.GridColumns.FindGridColumn(PartPropertyName.Change).Count > 0) Then
                        .GridTable.GridColumns.FindGridColumn(PartPropertyName.Change).Single.KBLPropertyName = PartPropertyName.Change.ToString
                    End If
                End If

                If (.GridTable.HasOlderVersion(New Version(20, 1))) Then
                    If (.GridTable.GridColumns.FindGridColumn(ComponentPropertyName.ComponentPinMaps).Count = 0) Then
                        .GridTable.GridColumns.AddNew(componentGridAppearance, ObjectPropertyNameStrings.ComponentPinMaps, 18, True, True, ComponentPropertyName.ComponentPinMaps.ToString)
                    End If
                    If (.GridTable.GridColumns.FindGridColumn(ComponentPropertyName.Installation_Information).Count = 0) Then
                        .GridTable.GridColumns.AddNew(componentGridAppearance, ObjectPropertyNameStrings.InstallationInformation, 19, True, True, ComponentPropertyName.Installation_Information.ToString)
                    End If
                    If (.GridTable.GridColumns.FindGridColumn(PartPropertyName.Part_number_type).Count = 0) Then
                        .GridTable.GridColumns.AddNew(componentGridAppearance, ObjectPropertyNameStrings.PartNumberType, 20, True, True, PartPropertyName.Part_number_type.ToString)
                    End If
                End If

                .GridTable.Version = currentApplicationVersion.ToString(2)
                .Save()
            End If
        End With

        Dim connectorGridAppearance As ConnectorGridAppearance = GridAppearance.All.OfType(Of ConnectorGridAppearance).Single
        With connectorGridAppearance
            If (.GridTable?.HasOlderVersion(currentApplicationVersion)).GetValueOrDefault Then
                If (String.IsNullOrEmpty(.GridTable.Version)) Then
                    .GridTable.Description = KblObjectType.Connector_occurrence.ToString

                    If (.GridTable.GridColumns.FindGridColumn(ConnectorPropertyName.Installation_Information).Count > 0) Then
                        .GridTable.GridColumns.FindGridColumn(ConnectorPropertyName.Installation_Information).Single.KBLPropertyName = ConnectorPropertyName.Installation_Information.ToString
                    End If
                    If (.GridTable.GridColumns.FindGridColumn(ConnectorPropertyName.Alias_id).Count > 0) Then
                        .GridTable.GridColumns.FindGridColumn(ConnectorPropertyName.Alias_id).Single.KBLPropertyName = ConnectorPropertyName.Alias_id.ToString
                    End If
                    If (.GridTable.GridColumns.FindGridColumn(PartPropertyName.Part_alias_ids).Count > 0) Then
                        .GridTable.GridColumns.FindGridColumn(PartPropertyName.Part_alias_ids).Single.KBLPropertyName = PartPropertyName.Part_alias_ids.ToString
                    End If
                    If (.GridTable.GridColumns.FindGridColumn(PartPropertyName.External_references).Count > 0) Then
                        .GridTable.GridColumns.FindGridColumn(PartPropertyName.External_references).Single.KBLPropertyName = PartPropertyName.External_references.ToString
                    End If
                    If (.GridTable.GridColumns.FindGridColumn(PartPropertyName.Change).Count > 0) Then
                        .GridTable.GridColumns.FindGridColumn(PartPropertyName.Change).Single.KBLPropertyName = PartPropertyName.Change.ToString
                    End If

                    .GridTable.GridSubTable.Description = KblObjectType.Cavity_occurrence.ToString
                End If

                If (.GridTable.HasOlderVersion(New Version(18, 1))) Then
                    If (.GridTable.GridSubTable.GridColumns.FindGridColumn(ConnectorPropertyName.Plating).Count = 0) Then
                        .GridTable.GridSubTable.GridColumns.AddNew(connectorGridAppearance, ObjectPropertyNameStrings.Plating, 9, True, True, ConnectorPropertyName.Plating.ToString)
                    End If
                End If

                If (.GridTable.HasOlderVersion(New Version(20, 1))) Then
                    If (.GridTable.GridColumns.FindGridColumn(PartPropertyName.Part_number_type).Count = 0) Then
                        .GridTable.GridColumns.AddNew(connectorGridAppearance, ObjectPropertyNameStrings.PartNumberType, 24, True, True, PartPropertyName.Part_number_type.ToString)
                    End If
                End If

                .GridTable.Version = currentApplicationVersion.ToString(2)
                .Save()
            End If
        End With

        Dim coPackGridAppearance As CoPackGridAppearance = GridAppearance.All.OfType(Of CoPackGridAppearance).Single
        With coPackGridAppearance
            If (.GridTable?.HasOlderVersion(currentApplicationVersion)).GetValueOrDefault Then
                If (.GridTable.HasOlderVersion(New Version(20, 1))) Then
                    If (.GridTable.GridColumns.FindGridColumn(PartPropertyName.Part_number_type).Count = 0) Then
                        .GridTable.GridColumns.AddNew(coPackGridAppearance, ObjectPropertyNameStrings.PartNumberType, 19, True, True, PartPropertyName.Part_number_type.ToString)
                    End If
                End If

                .GridTable.Version = currentApplicationVersion.ToString(2)
                .Save()
            End If
        End With

        Dim defDimSpecGridAppearance As DefaultDimSpecGridAppearance = GridAppearance.All.OfType(Of DefaultDimSpecGridAppearance).Single
        With defDimSpecGridAppearance
            If (.GridTable?.HasOlderVersion(currentApplicationVersion)).GetValueOrDefault Then
                ' TODO: Further updates

                .GridTable.Version = currentApplicationVersion.ToString(2)
                .Save()
            End If
        End With

        Dim dimSpecGridAppearance As DimSpecGridAppearance = GridAppearance.All.OfType(Of DimSpecGridAppearance).Single
        With dimSpecGridAppearance
            If (.GridTable?.HasOlderVersion(currentApplicationVersion)).GetValueOrDefault Then
                If (.GridTable.HasOlderVersion(New Version(20, 1))) Then
                    If (.GridTable.GridColumns.FindGridColumn(DimensionSpecificationPropertyName.Alias_Id).Count = 0) Then
                        .GridTable.GridColumns.AddNew(dimSpecGridAppearance, ObjectPropertyNameStrings.AliasId, 7, True, True, DimensionSpecificationPropertyName.Alias_Id.ToString)
                    End If
                End If

                .GridTable.Version = currentApplicationVersion.ToString(2)
                .Save()
            End If
        End With

        Dim fixingGridAppearance As FixingGridAppearance = GridAppearance.All.OfType(Of FixingGridAppearance).Single
        With fixingGridAppearance
            If (.GridTable?.HasOlderVersion(currentApplicationVersion)).GetValueOrDefault Then
                If (String.IsNullOrEmpty(.GridTable.Version)) Then
                    .GridTable.Description = KblObjectType.Fixing_occurrence.ToString

                    If (.GridTable.GridColumns.FindGridColumn(FixingPropertyName.Alias_Id).Count > 0) Then
                        .GridTable.GridColumns.FindGridColumn(FixingPropertyName.Alias_Id).Single.KBLPropertyName = FixingPropertyName.Alias_Id.ToString
                    End If
                    If (.GridTable.GridColumns.FindGridColumn(PartPropertyName.External_references).Count > 0) Then
                        .GridTable.GridColumns.FindGridColumn(PartPropertyName.External_references).Single.KBLPropertyName = PartPropertyName.External_references.ToString
                    End If
                    If (.GridTable.GridColumns.FindGridColumn(FixingPropertyName.Installation_Information).Count > 0) Then
                        .GridTable.GridColumns.FindGridColumn(FixingPropertyName.Installation_Information).Single.KBLPropertyName = FixingPropertyName.Installation_Information.ToString
                    End If
                    If (.GridTable.GridColumns.FindGridColumn(PartPropertyName.Part_alias_ids).Count > 0) Then
                        .GridTable.GridColumns.FindGridColumn(PartPropertyName.Part_alias_ids).Single.KBLPropertyName = PartPropertyName.Part_alias_ids.ToString
                    End If
                    If (.GridTable.GridColumns.FindGridColumn(PartPropertyName.Change).Count > 0) Then
                        .GridTable.GridColumns.FindGridColumn(PartPropertyName.Change).Single.KBLPropertyName = PartPropertyName.Change.ToString
                    End If

                    If (.GridTable.GridColumns.FindGridColumn(FixingPropertyName.SegmentLocation).Count = 0) Then
                        .GridTable.GridColumns.AddNew(fixingGridAppearance, ObjectPropertyNameStrings.SegmentLocation, 19, True, True, FixingPropertyName.SegmentLocation.ToString)
                    End If
                    If (.GridTable.GridColumns.FindGridColumn(FixingPropertyName.SegmentAbsolute_location).Count = 0) Then
                        .GridTable.GridColumns.AddNew(fixingGridAppearance, ObjectPropertyNameStrings.LocationAbsolute, 20, True, True, FixingPropertyName.SegmentAbsolute_location.ToString)
                    End If
                End If

                If (.GridTable.HasOlderVersion(New Version(20, 1))) Then
                    If (.GridTable.GridColumns.FindGridColumn(PartPropertyName.Part_number_type).Count = 0) Then
                        .GridTable.GridColumns.AddNew(fixingGridAppearance, ObjectPropertyNameStrings.PartNumberType, 21, True, True, PartPropertyName.Part_number_type.ToString)
                    End If
                End If

                .GridTable.Version = currentApplicationVersion.ToString(2)
                .Save()
            End If
        End With

        Dim moduleGridAppearance As ModuleGridAppearance = GridAppearance.All.OfType(Of ModuleGridAppearance).Single
        With moduleGridAppearance
            If (.GridTable?.HasOlderVersion(currentApplicationVersion)).GetValueOrDefault Then
                If (String.IsNullOrEmpty(.GridTable.Version)) Then
                    .GridTable.Description = KblObjectType.Module.ToString

                    If (.GridTable.GridColumns.FindGridColumn(PartPropertyName.External_references).Count > 0) Then
                        .GridTable.GridColumns.FindGridColumn(PartPropertyName.External_references).Single.KBLPropertyName = PartPropertyName.External_references.ToString
                    End If
                    If (.GridTable.GridColumns.FindGridColumn(PartPropertyName.Part_alias_ids).Count > 0) Then
                        .GridTable.GridColumns.FindGridColumn(PartPropertyName.Part_alias_ids).Single.KBLPropertyName = PartPropertyName.Part_alias_ids.ToString
                    End If

                    .GridTable.GridSubTable.Description = MainFormStrings.ModuleChanges_GridTableDescription
                End If

                If (.GridTable.HasOlderVersion(New Version(20, 1))) Then
                    If (.GridTable.GridColumns.FindGridColumn(PartPropertyName.Part_number_type).Count = 0) Then
                        .GridTable.GridColumns.AddNew(moduleGridAppearance, ObjectPropertyNameStrings.PartNumberType, 24, True, True, PartPropertyName.Part_number_type.ToString)
                    End If
                End If

                .GridTable.Version = currentApplicationVersion.ToString(2)
                .Save()
            End If
        End With

        Dim netGridAppearance As NetGridAppearance = GridAppearance.All.OfType(Of NetGridAppearance).Single
        With netGridAppearance
            If (.GridTable?.HasOlderVersion(currentApplicationVersion)).GetValueOrDefault Then
                If (String.IsNullOrEmpty(.GridTable.Version)) Then
                    .GridTable.Description = KblObjectType.Net.ToString

                    .GridTable.GridSubTable.Description = MainFormStrings.NetConns_GridTableDescription

                    If (.GridTable.GridSubTable.GridColumns.FindGridColumn(PartPropertyName.External_references).Count > 0) Then
                        .GridTable.GridSubTable.GridColumns.FindGridColumn(PartPropertyName.External_references).Single.KBLPropertyName = ConnectionPropertyName.External_references.ToString
                    End If
                    If (.GridTable.GridSubTable.GridColumns.FindGridColumn(ConnectionPropertyName.Installation_Information).Count > 0) Then
                        .GridTable.GridSubTable.GridColumns.FindGridColumn(ConnectionPropertyName.Installation_Information).Single.KBLPropertyName = ConnectionPropertyName.Installation_Information.ToString
                    End If
                    If (.GridTable.GridSubTable.GridColumns.FindGridColumn(ConnectionPropertyName.Processing_Information).Count > 0) Then
                        .GridTable.GridSubTable.GridColumns.FindGridColumn(ConnectionPropertyName.Processing_Information).Single.KBLPropertyName = ConnectionPropertyName.Processing_Information.ToString
                    End If
                End If

                If (.GridTable.GridColumns.FindGridColumn(ConnectionPropertyName.Signal_type).Count = 0) Then
                    .GridTable.GridColumns.AddNew(netGridAppearance, ObjectPropertyNameStrings.SignalType, 1, True, True, ConnectionPropertyName.Signal_type.ToString)
                End If
                If (.GridTable.GridSubTable.GridColumns.FindGridColumn(ConnectionPropertyName.Nominal_voltage).Count = 0) Then
                    .GridTable.GridSubTable.GridColumns.AddNew(netGridAppearance, ObjectPropertyNameStrings.NominalVoltage, 6, True, True, ConnectionPropertyName.Nominal_voltage.ToString)
                End If

                .GridTable.Version = currentApplicationVersion.ToString(2)
                .Save()
            End If
        End With

        Dim segmentGridAppearance As SegmentGridAppearance = GridAppearance.All.OfType(Of SegmentGridAppearance).Single
        With segmentGridAppearance
            If (.GridTable?.HasOlderVersion(currentApplicationVersion)).GetValueOrDefault Then
                If (String.IsNullOrEmpty(.GridTable.Version)) Then
                    .GridTable.Description = KblObjectType.Segment.ToString

                    If (.GridTable.GridColumns.FindGridColumn(SegmentPropertyName.Cross_Section_Area_information).Count > 0) Then
                        .GridTable.GridColumns.FindGridColumn(SegmentPropertyName.Cross_Section_Area_information).Single.KBLPropertyName = SegmentPropertyName.Cross_Section_Area_information.ToString
                    End If
                    If (.GridTable.GridColumns.FindGridColumn(SegmentPropertyName.Alias_id).Count > 0) Then
                        .GridTable.GridColumns.FindGridColumn(SegmentPropertyName.Alias_id).Single.KBLPropertyName = SegmentPropertyName.Alias_id.ToString
                    End If
                    If (.GridTable.GridColumns.FindGridColumn(SegmentPropertyName.Fixing_Assignment).Count > 0) Then
                        .GridTable.GridColumns.FindGridColumn(SegmentPropertyName.Fixing_Assignment).Single.KBLPropertyName = SegmentPropertyName.Fixing_Assignment.ToString
                    End If

                    .GridTable.GridSubTable.Description = MainFormStrings.SegProts_GridTableDescription

                    If (.GridTable.GridSubTable.GridColumns.FindGridColumn(WireProtectionPropertyName.Alias_id).Count > 0) Then
                        .GridTable.GridSubTable.GridColumns.FindGridColumn(WireProtectionPropertyName.Alias_id).Single.KBLPropertyName = WireProtectionPropertyName.Alias_id.ToString
                    End If
                    If (.GridTable.GridSubTable.GridColumns.FindGridColumn(ProtectionAreaPropertyName.Processing_information).Count > 0) Then
                        .GridTable.GridSubTable.GridColumns.FindGridColumn(ProtectionAreaPropertyName.Processing_information).Single.KBLPropertyName = ProtectionAreaPropertyName.Processing_information.ToString
                    End If
                    If (.GridTable.GridSubTable.GridColumns.FindGridColumn(PartPropertyName.Part_alias_ids).Count > 0) Then
                        .GridTable.GridSubTable.GridColumns.FindGridColumn(PartPropertyName.Part_alias_ids).Single.KBLPropertyName = PartPropertyName.Part_alias_ids.ToString
                    End If
                    If (.GridTable.GridSubTable.GridColumns.FindGridColumn(PartPropertyName.External_references).Count > 0) Then
                        .GridTable.GridSubTable.GridColumns.FindGridColumn(PartPropertyName.External_references).Single.KBLPropertyName = PartPropertyName.External_references.ToString
                    End If
                    If (.GridTable.GridSubTable.GridColumns.FindGridColumn(PartPropertyName.Change).Count > 0) Then
                        .GridTable.GridSubTable.GridColumns.FindGridColumn(PartPropertyName.Change).Single.KBLPropertyName = PartPropertyName.Change.ToString
                    End If
                    If (.GridTable.GridSubTable.GridColumns.FindGridColumn(WireProtectionPropertyName.Installation_Information).Count > 0) Then
                        .GridTable.GridSubTable.GridColumns.FindGridColumn(WireProtectionPropertyName.Installation_Information).Single.KBLPropertyName = WireProtectionPropertyName.Installation_Information.ToString
                    End If
                End If

                If (.GridTable.GridColumns.FindGridColumn(SegmentPropertyName.Processing_information).Count = 0) Then
                    .GridTable.GridColumns.AddNew(segmentGridAppearance, ObjectPropertyNameStrings.ProcessingInformation, 9, True, True, SegmentPropertyName.Processing_information.ToString)
                End If

                If (.GridTable.HasOlderVersion(New Version(20, 1))) Then
                    If (.GridTable.GridSubTable.GridColumns.FindGridColumn(ProtectionAreaPropertyName.Is_on_top_of).Count = 0) Then
                        .GridTable.GridSubTable.GridColumns.AddNew(segmentGridAppearance, ObjectPropertyNameStrings.IsOnTopOf, 25, True, True, ProtectionAreaPropertyName.Is_on_top_of.ToString)
                    End If
                    If (.GridTable.GridSubTable.GridColumns.FindGridColumn(WireProtectionPropertyName.Winding_type).Count = 0) Then
                        .GridTable.GridSubTable.GridColumns.AddNew(segmentGridAppearance, ObjectPropertyNameStrings.WindingType, 26, True, True, WireProtectionPropertyName.Winding_type.ToString)
                    End If
                    If (.GridTable.GridSubTable.GridColumns.FindGridColumn(WireProtectionPropertyName.Winding_firmness).Count = 0) Then
                        .GridTable.GridSubTable.GridColumns.AddNew(segmentGridAppearance, ObjectPropertyNameStrings.WindingFirmness, 27, True, True, WireProtectionPropertyName.Winding_firmness.ToString)
                    End If
                    If (.GridTable.GridSubTable.GridColumns.FindGridColumn(PartPropertyName.Part_number_type).Count = 0) Then
                        .GridTable.GridSubTable.GridColumns.AddNew(segmentGridAppearance, ObjectPropertyNameStrings.PartNumberType, 28, True, True, PartPropertyName.Part_number_type.ToString)
                    End If
                End If

                .GridTable.Version = currentApplicationVersion.ToString(2)
                .Save()
            End If
        End With

        Dim vertexGridAppearance As VertexGridAppearance = GridAppearance.All.OfType(Of VertexGridAppearance).Single
        With vertexGridAppearance
            If (.GridTable?.HasOlderVersion(currentApplicationVersion)).GetValueOrDefault Then
                If (.GridTable.HasOlderVersion(New Version(20, 1))) Then
                    If (.GridTable.GridColumns.FindGridColumn(VertexPropertyName.Folding_direction).Count = 0) Then
                        .GridTable.GridColumns.AddNew(vertexGridAppearance, ObjectPropertyNameStrings.FoldingDirection, 8, True, True, VertexPropertyName.Folding_direction.ToString)
                    End If

                    If (.GridTable.GridSubTable.GridColumns.FindGridColumn(WireProtectionPropertyName.Winding_type).Count = 0) Then
                        .GridTable.GridSubTable.GridColumns.AddNew(vertexGridAppearance, ObjectPropertyNameStrings.WindingType, 21, True, True, WireProtectionPropertyName.Winding_type.ToString)
                    End If
                    If (.GridTable.GridSubTable.GridColumns.FindGridColumn(WireProtectionPropertyName.Winding_firmness).Count = 0) Then
                        .GridTable.GridSubTable.GridColumns.AddNew(vertexGridAppearance, ObjectPropertyNameStrings.WindingFirmness, 22, True, True, WireProtectionPropertyName.Winding_firmness.ToString)
                    End If
                    If (.GridTable.GridSubTable.GridColumns.FindGridColumn(PartPropertyName.Part_number_type).Count = 0) Then
                        .GridTable.GridSubTable.GridColumns.AddNew(vertexGridAppearance, ObjectPropertyNameStrings.PartNumberType, 23, True, True, PartPropertyName.Part_number_type.ToString)
                    End If
                End If

                .GridTable.Version = currentApplicationVersion.ToString(2)
                .Save()
            End If
        End With

        Dim wireGridAppearance As WireGridAppearance = GridAppearance.All.OfType(Of WireGridAppearance).Single
        With wireGridAppearance
            If (.GridTable?.HasOlderVersion(currentApplicationVersion)).GetValueOrDefault Then
                If (String.IsNullOrEmpty(.GridTable.Version)) Then
                    .GridTable.Description = KblObjectType.Wire_occurrence.ToString

                    If (.GridTable.GridColumns.FindGridColumn(WirePropertyName.Core_Colour).Count > 0) Then
                        .GridTable.GridColumns.FindGridColumn(WirePropertyName.Core_Colour).Single.KBLPropertyName = WirePropertyName.Core_Colour.ToString
                    End If
                    If (.GridTable.GridColumns.FindGridColumn(WirePropertyName.Length_information).Count > 0) Then
                        .GridTable.GridColumns.FindGridColumn(WirePropertyName.Length_information).Single.KBLPropertyName = WirePropertyName.Length_information.ToString
                    End If
                    If (.GridTable.GridColumns.FindGridColumn(PartPropertyName.Part_alias_ids).Count > 0) Then
                        .GridTable.GridColumns.FindGridColumn(PartPropertyName.Part_alias_ids).Single.KBLPropertyName = PartPropertyName.Part_alias_ids.ToString
                    End If
                    If (.GridTable.GridColumns.FindGridColumn(PartPropertyName.External_references).Count > 0) Then
                        .GridTable.GridColumns.FindGridColumn(PartPropertyName.External_references).Single.KBLPropertyName = PartPropertyName.External_references.ToString
                    End If
                    If (.GridTable.GridColumns.FindGridColumn(PartPropertyName.Change).Count > 0) Then
                        .GridTable.GridColumns.FindGridColumn(PartPropertyName.Change).Single.KBLPropertyName = PartPropertyName.Change.ToString
                    End If
                    If (.GridTable.GridColumns.FindGridColumn(WirePropertyName.Installation_Information).Count > 0) Then
                        .GridTable.GridColumns.FindGridColumn(WirePropertyName.Installation_Information).Single.KBLPropertyName = WirePropertyName.Installation_Information.ToString
                    End If

                    If (.GridTable.GridColumns.FindGridColumn(WirePropertyName.AdditionalExtremities).Count = 0) Then
                        .GridTable.GridColumns.AddNew(wireGridAppearance, ObjectPropertyNameStrings.AdditionalExtrimities, 30, True, True, WirePropertyName.AdditionalExtremities.ToString)
                    End If
                End If

                If (.GridTable.HasOlderVersion(New Version(20, 1))) Then
                    If (.GridTable.GridColumns.FindGridColumn(PartPropertyName.Part_number_type).Count = 0) Then
                        .GridTable.GridColumns.AddNew(wireGridAppearance, ObjectPropertyNameStrings.PartNumberType, 31, True, True, PartPropertyName.Part_number_type.ToString)
                    End If
                End If

                .GridTable.Version = currentApplicationVersion.ToString(2)
                .Save()
            End If
        End With
    End Sub

    Private Sub InitializeGridAppearancesCore() ' HINT: load or copy all default appearances
        Dim errors As New List(Of Exception)
        For Each gridApp As GridAppearance In GridAppearance.All
            gridApp.TryCopyFromDefaultDir()
            Try
                gridApp.LoadOrCreateDefaultTable()
                If Not gridApp.FileExists Then
                    gridApp.Save()
                End If
            Catch ex As Exception
#If DEBUG Or CONFIG = "Debug" Then
                Throw
#End If
                errors.Add(ex)
            End Try
        Next

        If errors.Count > 0 Then
            MessageBoxEx.ShowWarning(Me, ErrorStrings.ErrorInitCreatingAppearances)
        End If
    End Sub

    <Obfuscation(Feature:="renaming", Exclude:=True)> 'HINT: this is needed because the init-text is extracted from the method name -> this could be done better by replacing it with separate resource-strings
    Private Sub InitializeStampSpecifications()
        Dim errorOnLoad As Boolean = False
        Dim qmStampSpecificationFile As String = GetQMStampSpecificationsFile()

        If IO.File.Exists(qmStampSpecificationFile) Then
            Dim fi As New FileInfo(qmStampSpecificationFile)
            If (fi.Length > 0) Then
                _qmStampSpecifications = QMStampSpecifications.LoadFromFile(qmStampSpecificationFile)
                If (_qmStampSpecifications Is Nothing) Then
                    AppInitManager.ErrorManager.ShowMsgBoxErrorOrWriteConsole(String.Format(MainFormStrings.ErrorLoadStampSpec_Msg, vbCrLf, QMStampSpecifications.LoadError.Message))
                    errorOnLoad = True
                End If
            End If
        End If

        If (_qmStampSpecifications Is Nothing) Then
            _qmStampSpecifications = New QMStampSpecifications
            _qmStampSpecifications.ResetToDefaults()

            If (Not errorOnLoad) Then
                _qmStampSpecifications.SaveTo(qmStampSpecificationFile)
            End If
        End If
    End Sub

    <Obfuscation(Feature:="renaming", Exclude:=True)> 'HINT: this is needed because the init-text is extracted from the method name -> this could be done better by replacing it with separate resource-strings
    Private Sub InitializeSchematicsView()
        If (HasSchematicsFeature) Then

            Dim schematicsViewPane As DockableControlPane = Me.Panes.AddNew(Of Schematics.Controls.ViewControl)(HomeTabToolKey.DisplaySchematicsView, UIStrings.SchematicsViewPane_Caption, DockedSide.Bottom, My.Settings.AdvConnFloatSize, True)
            With schematicsViewPane
                .Settings.CanDisplayAsMdiChild = Infragistics.Win.DefaultableBoolean.False
                .Settings.AllowDockAsTab = Infragistics.Win.DefaultableBoolean.False
                .Settings.DoubleClickAction = PaneDoubleClickAction.None
            End With

            _schematicsView = CType(schematicsViewPane.Control, Schematics.Controls.ViewControl)
            _schematicsView.SetLicense()
            _schematicsView.AutoZoomSelection = _generalSettings.AutoZoomSelectionSchematics
            Me.udmMain.EndUpdate()
        End If
    End Sub

    Private Sub ValidateAssignedCoresWiresOnContactPoint(coreWireIds As IEnumerable(Of String), documentForm As DocumentForm, inliner As Connector_occurrence, isPredecessor As Boolean, prevRoutingConnection As RoutingConnection)
        Dim docName As String = documentForm.File.FullName
        Dim kblMapper As KblMapper = documentForm.KBL

        For Each coreWireId As String In coreWireIds
            If documentForm.InactiveObjects.ContainsValue(KblObjectType.Core_occurrence, coreWireId) OrElse (documentForm.InactiveObjects.ContainsValue(KblObjectType.Wire_occurrence, coreWireId)) Then
                Continue For
            End If

            Dim cable As Special_wire_occurrence = kblMapper.GetCableOfWireOrCore(coreWireId)
            If cable IsNot Nothing AndAlso (Not _entireRoutingPath.Values.Where(Function(routingPath) routingPath.Connections.ContainsKey(coreWireId)).Any()) Then
                Dim routingConnection As New RoutingConnection(docName)
                With routingConnection
                    .Cable = cable
                    .Core = kblMapper.GetOccurrenceObject(Of Core_occurrence)(coreWireId)

                    If (.Core IsNot Nothing) Then
                        .CrossSectionArea = kblMapper.GetPart(Of Core)(.Core.Part)?.Cross_section_area
                    End If

                    If (_entireRoutingPath(docName).Connections.Count = 0) Then
                        .IsActiveConnection = True
                    End If

                    If (isPredecessor) Then
                        prevRoutingConnection.PredecessorConnection = routingConnection
                        .SuccessorConnection = prevRoutingConnection
                    Else
                        prevRoutingConnection.SuccessorConnection = routingConnection
                        .PredecessorConnection = prevRoutingConnection
                    End If

                    .Wire = kblMapper.GetOccurrenceObject(Of Wire_occurrence)(coreWireId)

                    If (.Wire IsNot Nothing) Then
                        .CrossSectionArea = kblMapper.GetPart(Of General_wire)(.Wire.Part)?.Cross_section_area
                    End If
                End With

                _entireRoutingPath(docName).Connections.Add(coreWireId, routingConnection)

                ValidateRoutingConnection_Recursively(documentForm, inliner, isPredecessor, _entireRoutingPath(docName).Connections.Last, _entireRoutingPath(docName))
            End If
        Next
    End Sub

    Private Sub ValidateCounterInliner(cavityId As String, cavityNumber As String, documentForm As DocumentForm, inliner As Connector_occurrence, isPredecessor As Boolean, prevRoutingConnection As RoutingConnection)
        If (_mainStateMachine.OverallConnectivity IsNot Nothing) Then
            Dim id As String = HarnessConnectivity.GetUniqueId(documentForm.KBL.HarnessPartNumber, cavityId)
            If (_mainStateMachine.OverallConnectivity.InlinerCavityPairs.ContainsKey(id)) Then
                Dim counterCavityId As String = HarnessConnectivity.GetKblIdFromUniqueId(_mainStateMachine.OverallConnectivity.InlinerCavityPairs(id))
                Dim counterHarnessPartNumber As String = HarnessConnectivity.GetHarnessFromUniqueId(_mainStateMachine.OverallConnectivity.InlinerCavityPairs(id))
                For Each tabGroup As MdiTabGroup In Me.utmmMain.TabGroups
                    For Each tab As MdiTab In tabGroup.Tabs
                        Dim docForm As DocumentForm = DirectCast(tab.Form, DocumentForm)

                        If (Not docForm.Equals(documentForm)) AndAlso (docForm.KBL.HarnessPartNumber = counterHarnessPartNumber) Then
                            If (Not _entireRoutingPath.ContainsKey(docForm.File.FullName)) Then _entireRoutingPath.Add(docForm.File.FullName, New RoutingPath(False, docForm.KBL.GetHarness))

                            If (docForm.KBL.KBLCavityContactPointMapper.ContainsKey(counterCavityId)) Then
                                For Each contactPoint As Contact_point In docForm.KBL.KBLCavityContactPointMapper(counterCavityId)
                                    If (docForm.KBL.KBLContactPointWireMapper.ContainsKey(contactPoint.SystemId)) Then
                                        ValidateAssignedCoresWiresOnContactPoint(docForm.KBL.KBLContactPointWireMapper(contactPoint.SystemId), docForm, inliner, isPredecessor, prevRoutingConnection)
                                    End If
                                Next
                            End If
                        End If
                    Next
                Next
            End If
        Else
            Dim counterInliner As Connector_occurrence = documentForm.Inliners(inliner)

            For Each tabGroup As MdiTabGroup In Me.utmmMain.TabGroups
                For Each tab As MdiTab In tabGroup.Tabs
                    Dim docForm As DocumentForm = DirectCast(tab.Form, DocumentForm)

                    If (Not docForm.Equals(documentForm)) AndAlso (docForm.Inliners.ContainsKey(counterInliner)) Then
                        If (Not _entireRoutingPath.ContainsKey(docForm.File.FullName)) Then
                            _entireRoutingPath.Add(docForm.File.FullName, New RoutingPath(False, docForm.KBL.GetHarness))
                        End If

                        If (counterInliner.Slots IsNot Nothing) AndAlso (counterInliner.Slots.Length <> 0) Then
                            For Each cavity As Cavity_occurrence In counterInliner.Slots(0).Cavities.Where(Function(cav) docForm.KBL.KBLPartMapper.ContainsKey(cav.Part) AndAlso DirectCast(docForm.KBL.KBLPartMapper(cav.Part), Cavity).Cavity_number = cavityNumber)
                                If (docForm.KBL.KBLCavityContactPointMapper.ContainsKey(cavity.SystemId)) Then
                                    For Each contactPoint As Contact_point In docForm.KBL.KBLCavityContactPointMapper(cavity.SystemId)
                                        If (docForm.KBL.KBLContactPointWireMapper.ContainsKey(contactPoint.SystemId)) Then
                                            ValidateAssignedCoresWiresOnContactPoint(docForm.KBL.KBLContactPointWireMapper(contactPoint.SystemId), docForm, inliner, isPredecessor, prevRoutingConnection)
                                        End If
                                    Next
                                End If
                            Next
                        End If
                    End If
                Next
            Next
        End If
    End Sub

    Private Sub ValidateRoutingConnection_Recursively(documentForm As DocumentForm, inliner As Connector_occurrence, isPredecessor As Boolean, routingConnection As KeyValuePair(Of String, RoutingConnection), routingPath As RoutingPath)
        For Each connection As Connection In documentForm.KBL.GetConnections.Where(Function(conn) conn.Wire = routingConnection.Key)
            Dim cavityIdA As String = String.Empty
            Dim cavityIdB As String = String.Empty
            Dim cavityNumberA As String = String.Empty
            Dim cavityNumberB As String = String.Empty
            Dim connectorA As Connector_occurrence = Nothing
            Dim connectorB As Connector_occurrence = Nothing
            Dim contactPointA As String = connection.GetStartContactPointId
            Dim contactPointB As String = connection.GetEndContactPointId
            Dim generalTerminalA As General_terminal = Nothing
            Dim generalTerminalB As General_terminal = Nothing
            Dim signalName As String = String.Empty

            If (connection.Signal_name IsNot Nothing) Then
                signalName = connection.Signal_name
            End If

            routingPath.SignalName = signalName
            connectorA = documentForm.KBL.GetConnectorOfContactPoint(contactPointA)
            If connectorA IsNot Nothing Then
                cavityIdA = documentForm.KBL.GetCavityOccurrenceOfContactPointId(contactPointA)?.SystemId
                cavityNumberA = documentForm.KBL.GetCavityOfContactPointId(contactPointA)?.Cavity_number

                Dim contactPointA_occ As Contact_point = documentForm.KBL.GetOccurrenceObject(Of Contact_point)(contactPointA)
                For Each associatedPart As String In contactPointA_occ?.Associated_parts.OrEmpty.SplitSpace
                    Dim occ As IKblOccurrence = documentForm.KBL.GetOccurrenceObjectUntyped(associatedPart)
                    If occ IsNot Nothing Then
                        Select Case occ.ObjectType
                            Case KblObjectType.Special_terminal_occurrence, KblObjectType.Terminal_occurrence
                                generalTerminalA = documentForm.KBL.GetPart(Of General_terminal)(occ.Part)
                                Exit For
                        End Select
                    End If
                Next
            End If


            If (contactPointA <> contactPointB) Then
                connectorB = documentForm.KBL.GetConnectorOfContactPoint(contactPointB)
                If connectorB IsNot Nothing Then
                    cavityIdB = documentForm.KBL.GetCavityOccurrenceOfContactPointId(contactPointB)?.SystemId
                    cavityNumberB = documentForm.KBL.GetCavityOfContactPointId(contactPointB)?.Cavity_number

                    Dim contactPointB_occ As Contact_point = documentForm.KBL.GetOccurrenceObject(Of Contact_point)(contactPointB)
                    For Each associatedPart As String In contactPointB_occ?.Associated_parts.OrEmpty.SplitSpace
                        Dim occ As IKblOccurrence = documentForm.KBL.GetOccurrenceObjectUntyped(associatedPart)
                        If occ IsNot Nothing Then
                            Select Case occ.ObjectType
                                Case KblObjectType.Special_terminal_occurrence, KblObjectType.Terminal_occurrence
                                    generalTerminalB = documentForm.KBL.GetPart(Of General_terminal)(occ.Part)
                                    Exit For
                            End Select
                        End If
                    Next
                End If
            End If

            If (routingPath.IsOriginHarness) Then
                With routingConnection.Value
                    .CavityNumberA = cavityNumberA
                    .CavityNumberB = cavityNumberB
                    .ConnectorA = connectorA
                    .ConnectorB = connectorB
                    .GeneralTerminalA = generalTerminalA
                    .GeneralTerminalB = generalTerminalB
                End With

                If (connectorA IsNot Nothing) AndAlso (documentForm.Inliners.ContainsKey(connectorA)) AndAlso (documentForm.Inliners(connectorA) IsNot Nothing) Then
                    ValidateCounterInliner(cavityIdA, cavityNumberA, documentForm, connectorA, True, routingConnection.Value)
                End If
                If (connectorB IsNot Nothing) AndAlso (documentForm.Inliners.ContainsKey(connectorB)) AndAlso (documentForm.Inliners(connectorB) IsNot Nothing) Then
                    ValidateCounterInliner(cavityIdB, cavityNumberB, documentForm, connectorB, False, routingConnection.Value)
                End If
            Else
                If (isPredecessor AndAlso documentForm.Inliners.ContainsKey(connectorA) AndAlso documentForm.Inliners(connectorA) IsNot Nothing AndAlso documentForm.Inliners(connectorA).Equals(inliner)) OrElse (Not isPredecessor AndAlso connectorB IsNot Nothing AndAlso documentForm.Inliners.ContainsKey(connectorB) AndAlso documentForm.Inliners(connectorB) IsNot Nothing AndAlso documentForm.Inliners(connectorB).Equals(inliner)) Then
                    Dim prevCavityNumberB As String = cavityNumberB
                    Dim prevConnectorB As Connector_occurrence = connectorB
                    Dim prevGeneralTerminalB As General_terminal = generalTerminalB
                    Dim prevCavityIdB As String = cavityIdB

                    cavityIdB = cavityIdA
                    cavityIdA = prevCavityIdB
                    cavityNumberB = cavityNumberA
                    cavityNumberA = prevCavityNumberB
                    connectorB = connectorA
                    connectorA = prevConnectorB
                    generalTerminalB = generalTerminalA
                    generalTerminalA = prevGeneralTerminalB
                End If

                With routingConnection.Value
                    .CavityNumberA = cavityNumberA
                    .CavityNumberB = cavityNumberB
                    .ConnectorA = connectorA
                    .ConnectorB = connectorB
                    .GeneralTerminalA = generalTerminalA
                    .GeneralTerminalB = generalTerminalB
                End With

                If (isPredecessor) Then
                    If (documentForm.Inliners.ContainsKey(connectorA)) AndAlso (documentForm.Inliners(connectorA) IsNot Nothing) Then
                        ValidateCounterInliner(cavityIdA, cavityNumberA, documentForm, connectorA, isPredecessor, routingConnection.Value)
                    End If
                Else
                    If (connectorB IsNot Nothing) AndAlso (documentForm.Inliners.ContainsKey(connectorB)) AndAlso (documentForm.Inliners(connectorB) IsNot Nothing) Then
                        ValidateCounterInliner(cavityIdB, cavityNumberB, documentForm, connectorB, isPredecessor, routingConnection.Value)
                    End If
                End If
            End If
        Next
    End Sub

    Private Sub ValidateRoutingOfInitialCoresWires(e As HighlightEntireRoutingPathEventArgs)
        For Each coreWireId As String In e.CoreWireIds
            If ActiveDocument.InactiveObjects.ContainsValue(KblObjectType.Core_occurrence, coreWireId) OrElse ActiveDocument.InactiveObjects.ContainsValue(KblObjectType.Wire_occurrence, coreWireId) Then
                Continue For
            End If

            Dim routingConnection As New RoutingConnection(ActiveDocument.File.FullName)
            routingConnection.Cable = ActiveDocument.KBL.GetCableOfWireOrCore(coreWireId)

            routingConnection.Core = ActiveDocument.KBL.GetOccurrenceObject(Of Core_occurrence)(coreWireId) ' HINT: just try out if something will be found with that type
            routingConnection.Wire = ActiveDocument.KBL.GetOccurrenceObject(Of Wire_occurrence)(coreWireId) ' HINT: just try out if something will be found with that type

            If (routingConnection.Core IsNot Nothing) Then
                routingConnection.CrossSectionArea = ActiveDocument.KBL.GetPart(Of Core)(routingConnection.Core.Part)?.Cross_section_area
            End If

            If (routingConnection.Wire IsNot Nothing) Then
                routingConnection.CrossSectionArea = ActiveDocument.KBL.GetPart(Of General_wire)(routingConnection.Wire.Part)?.Cross_section_area
            End If

            If (_entireRoutingPath(ActiveDocument.File.FullName).Connections.Count = 0) Then
                routingConnection.IsActiveConnection = True
            End If

            _entireRoutingPath(ActiveDocument.File.FullName).Connections.Add(coreWireId, routingConnection)

            ValidateRoutingConnection_Recursively(ActiveDocument, Nothing, False, _entireRoutingPath(ActiveDocument.File.FullName).Connections.Last, _entireRoutingPath(ActiveDocument.File.FullName))
        Next
    End Sub

    <Obfuscation(Feature:="renaming")>
    Private Function GetDiameterSettingsConfigurationFile() As String
        Dim e3HarnessAnalyzerDir As String = [Shared].Utilities.GetApplicationSettingsPath

        If (Not IO.Directory.Exists(e3HarnessAnalyzerDir)) Then
            IO.Directory.CreateDirectory(e3HarnessAnalyzerDir)
        End If

        Return IO.Path.Combine(e3HarnessAnalyzerDir, "DiameterSettings.xml")
    End Function

    <Obfuscation(Feature:="renaming")>
    Private Function GetQMStampSpecificationsFile() As String
        Dim e3HarnessAnalyzerDir As String = [Shared].Utilities.GetApplicationSettingsPath

        If (Not IO.Directory.Exists(e3HarnessAnalyzerDir)) Then
            IO.Directory.CreateDirectory(e3HarnessAnalyzerDir)
        End If

        Return IO.Path.Combine(e3HarnessAnalyzerDir, "QMStampSpecifications.xml")
    End Function

    <Obfuscation(Feature:="renaming")>
    Private Function GetWeightSettingsConfigurationFile() As String
        Dim e3HarnessAnalyzerDir As String = [Shared].Utilities.GetApplicationSettingsPath

        If (Not IO.Directory.Exists(e3HarnessAnalyzerDir)) Then
            IO.Directory.CreateDirectory(e3HarnessAnalyzerDir)
        End If

        Return IO.Path.Combine(e3HarnessAnalyzerDir, "WeightSettings.xml")
    End Function

    <Obfuscation(Feature:="renaming")>
    Private Function GetMappedPropertiesFromE3toModelConfigurationFile() As String
        Dim e3HarnessAnalyzerDir As String = [Shared].Utilities.GetApplicationSettingsPath

        If (Not IO.Directory.Exists(e3HarnessAnalyzerDir)) Then
            IO.Directory.CreateDirectory(e3HarnessAnalyzerDir)
        End If

        Return IO.Path.Combine(e3HarnessAnalyzerDir, "E3XmlSettings.xml")
    End Function

    Private Function IsNet45OrNewer() As Boolean
        Return Type.GetType("System.Reflection.ReflectionContext", False) IsNot Nothing
    End Function

    Private Function IsTouchEnabled() As Boolean
        Return CBool(System.Windows.Native.GetSystemMetrics(System.Windows.Native.SM_DIGITIZER) And System.Windows.Native.NID_MULTI_INPUT)
    End Function

    Private Sub CheckVersion(Optional generalSettingsConfigFile As String = Nothing)
        If (String.IsNullOrEmpty(generalSettingsConfigFile)) Then
            generalSettingsConfigFile = HarnessAnalyzer.D3D.MainFormController.GetGeneralSettingsConfigurationFile()
        End If

        Dim settings As GeneralSettings = GeneralSettings.LoadFromFile(Of GeneralSettings)(generalSettingsConfigFile)
        If settings IsNot Nothing Then
            Try
                Dim MainVersion As String = Split(settings.Version, ".").First
                Dim AppVersion As String = Split(My.Application.Info.Version.ToString, ".").First
                If MainVersion <> AppVersion Then
                    Dim fInf As New IO.FileInfo(generalSettingsConfigFile)
                    Dim folder As String = fInf.DirectoryName
                    Dim zipFileName As String = "ProfileBackup_" + settings.Version + ".zip"
                    Dim tmpFile As String = Path.Combine(Path.GetTempPath, zipFileName)
                    Dim oldProfile As MemoryStream = GetOldProfileAsZip(folder, tmpFile)
                    IO.Directory.Delete(folder, True)
                    Dim dirInfo As New DirectoryInfo(folder)
                    If Not dirInfo.Exists Then
                        dirInfo.Create()
                    End If
                    SaveOldProfileAsZip(oldProfile, folder, zipFileName)
                End If
            Catch ex As Exception
                AppInitManager.ErrorManager.WriteConsoleError(ex.Message)
            End Try
        End If

    End Sub

    Private Sub SaveOldProfileAsZip(profile As MemoryStream, folder As String, filename As String)

        Using file As New FileStream(Path.Combine(folder, filename), FileMode.Create, FileAccess.Write)
            profile.WriteTo(file)
            file.Close()
        End Using
        profile.Close()
    End Sub

    Private Function GetOldProfileAsZip(ProfileFolder As String, ProfileTempName As String) As MemoryStream
        Dim tmpfile As New IO.FileInfo(ProfileTempName)
        If tmpfile.Exists Then
            tmpfile.Delete()
        End If

        Dim oldZipInfo As IO.FileInfo = Nothing
        For Each item As String In IO.Directory.GetFiles(ProfileFolder)
            Dim testInfo As New IO.FileInfo(item)
            If testInfo.Name.StartsWith("ProfileBackup_") AndAlso testInfo.Extension = ".zip" Then
                oldZipInfo = testInfo
                Exit For
            End If
        Next

        If oldZipInfo IsNot Nothing Then
            oldZipInfo.Delete()
        End If

        ZipFile.CreateFromDirectory(ProfileFolder, ProfileTempName, CompressionLevel.Optimal, False)

        Dim bData As Byte()
        Dim br As BinaryReader = New BinaryReader(System.IO.File.OpenRead(ProfileTempName))
        Dim length As Integer = CInt(br.BaseStream.Length)
        bData = br.ReadBytes(CInt(br.BaseStream.Length))
        Dim ms As MemoryStream = New MemoryStream(bData, 0, bData.Length)
        ms.Write(bData, 0, bData.Length)
        br.Close()

        tmpfile = New FileInfo(ProfileTempName)
        If tmpfile.Exists Then
            tmpfile.Delete()
        End If

        Return ms
    End Function

    ' Override WndProc to get messages
    <DebuggerStepThrough>
    Protected Overrides Sub WndProc(ByRef m As Message)
        If (m.Msg = WM_CLIPBOARDUPDATE) AndAlso (Me.utmmMain.TabGroups.Count <> 0) Then
            For Each tabGroup As MdiTabGroup In Me.utmmMain.TabGroups
                For Each documentTab As MdiTab In tabGroup.Tabs
                    DirectCast(documentTab.Form, DocumentForm).Tools.Home.PasteFromClipboardBtn.SharedProps.Enabled = Clipboard.ContainsText AndAlso Not String.IsNullOrWhiteSpace(Clipboard.GetText)
                Next
            Next
        End If

        MyBase.WndProc(m)
    End Sub

    <Obfuscation(Feature:="renaming", Exclude:=True)> 'HINT: this is needed because the init-text is extracted from the method name -> this could be done better by replacing it with separate resource-strings
    Friend Sub InitializeTopologyHub()
        Me.udmMain.BeginUpdate()
        Dim topologyHubPane As DockableControlPane = Me.Panes.AddNew(Of TopologyHub)(PaneKeys.TopologyHub, UIStrings.CarTopoPane_Caption, DockedSide.Bottom, New Drawing.Size(500, 250), True)
        With topologyHubPane
            .Settings.CanDisplayAsMdiChild = Infragistics.Win.DefaultableBoolean.False
            .Settings.AllowPin = Infragistics.Win.DefaultableBoolean.False
            .Settings.AllowMaximize = Infragistics.Win.DefaultableBoolean.False
            .Settings.DoubleClickAction = PaneDoubleClickAction.None
        End With
        _topologyHub = CType(topologyHubPane.Control, TopologyHub)
        Me.udmMain.EndUpdate()
    End Sub

    Friend Async Function OpenDocumentFromCommandLine() As Task(Of System.ComponentModel.IResult)
        Dim commandLine As CommandLineArgsProcessor = AppInitManager.CommandLine

        Select Case commandLine.FileArgStatus
            Case CommandLineArgsProcessor.FileState.HasAnotherFile
                Dim msg As String = String.Format(MainFormStrings.NotValidParameter_Msg, commandLine.File)
                AppInitManager.ErrorManager.ShowMsgBoxErrorOrWriteConsole(msg, ErrorCodes.ERROR_INVALID_PARAMETER)
                Return ComponentModel.Result.Faulted(msg)
            Case CommandLineArgsProcessor.FileState.HasDSI, CommandLineArgsProcessor.FileState.HasHCV, CommandLineArgsProcessor.FileState.HasKBL, CommandLineArgsProcessor.FileState.HasXHCV
                Dim fileInfo As IO.FileInfo = commandLine.TryGetAsFileInfo
                If (fileInfo IsNot Nothing) Then
                    Await _mainStateMachine.OpenDocument(New ListToolItem(fileInfo.FullName))
                Else
                    Dim msg As String = MainFormStrings.ErrorOpenFile_Msg
                    AppInitManager.ErrorManager.ShowMsgBoxErrorOrWriteConsole(msg, ErrorCodes.ERROR_FILE_NOT_FOUND)
                    Return ComponentModel.Result.Faulted(msg)
                End If
            Case CommandLineArgsProcessor.FileState.HasNonExistingFile
                Dim msg As String = String.Format(MainFormStrings.ErrorFindParameter_Msg, commandLine.File)
                AppInitManager.ErrorManager.ShowMsgBoxErrorOrWriteConsole(msg, ErrorCodes.ERROR_INVALID_PARAMETER)
                Return ComponentModel.Result.Faulted(msg)
        End Select
        Return ComponentModel.Result.Success
    End Function

    Friend Sub UpdateAllEntitiesVisibleWithoutModules()
        Using Me.EnableWaitCursor
            For Each doc As DocumentForm In GetAllDocuments.Values
                If doc.ToggleVisibleOfEntitiesWithNoModules(NoModulesVisible) Then
                    doc.ActiveDrawingCanvas.vdCanvas.BaseControl.ActiveDocument.Invalidate()
                End If
            Next
        End Using
    End Sub

    Friend Sub DetachFromClipboard()
        System.Windows.Native.RemoveClipboardFormatListener(Me.Handle)
    End Sub

    Friend Sub AttachToClipboard()
        System.Windows.Native.AddClipboardFormatListener(Me.Handle)
    End Sub

    Private Async Sub MainForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If (e.Control) AndAlso (e.KeyCode = Keys.O) Then
            Await _mainStateMachine.OpenDocument()
        End If
    End Sub

    Private Async Sub MainForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            Initialize() ' HINT: here can be files created to the disk! Moved out from the constructor to avoid disk writing on creating the mainform constructor
        Catch ex As Exception
            If ex.OnDebugCheckForVectorDrawEvaluationError Then
                Me.Close()
                Return
            ElseIf Not DebugEx.IsDebug Then
                'in release show a full message
                ex.ShowAppCrashMessage(False) 'HINT: These exception in the load-event are suppressed by the framework (therefore show them always in release and deubg)
            Else
                'in debug show a short/fast message, must be done here because Load-Event is suppressing execptions (under some circumstances) that happening within
                ex.ShowAppCrashMessage(False, False)
            End If
        End Try

        Dim res As ComponentModel.IResult = Await OpenDocumentFromCommandLine()
        If res.IsSuccess Then
            If (_checkTimer IsNot Nothing) Then
                _checkTimer.Start()
            End If
        Else
            Me.Close()
        End If
    End Sub

    Private Sub MainForm_ResizeBegin(sender As Object, e As EventArgs) Handles Me.ResizeBegin
        If (ActiveDocument IsNot Nothing) AndAlso (ActiveDocument.ActiveDrawingCanvas IsNot Nothing) AndAlso (ActiveDocument.ActiveDrawingCanvas.vdCanvas IsNot Nothing) AndAlso (ActiveDocument.ActiveDrawingCanvas.vdCanvas.BaseControl.ActiveDocument IsNot Nothing) Then
            ActiveDocument.ActiveDrawingCanvas.vdCanvas.BaseControl.ActiveDocument.DisableRedraw = True
        End If
    End Sub

    Private Sub MainForm_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        If (ActiveDocument IsNot Nothing) AndAlso (ActiveDocument.ActiveDrawingCanvas IsNot Nothing) AndAlso (ActiveDocument.ActiveDrawingCanvas.vdCanvas IsNot Nothing) AndAlso (ActiveDocument.ActiveDrawingCanvas.vdCanvas.BaseControl.ActiveDocument IsNot Nothing) Then
            ActiveDocument.ActiveDrawingCanvas.vdCanvas.BaseControl.ActiveDocument.DisableRedraw = False
        End If
    End Sub

    Private Sub ActiveDocument_HighlightEntireRoutingPath(sender As InformationHub, e As HighlightEntireRoutingPathEventArgs) Handles _activeDocument.HighlightEntireRoutingPath
        _entireRoutingPath.NewOrClear
        _entireRoutingPath.Add(ActiveDocument.File.FullName, New RoutingPath(True, ActiveDocument.KBL.GetHarness))

        ValidateRoutingOfInitialCoresWires(e)

        If (e.SelectRowsInGrids) Then
            If (Not _entireRoutingPath.Values.Where(Function(routingPath) Not routingPath.IsOriginHarness).Any()) Then
                ActiveDocument.SelectRowsInGrids(e.CoreWireIds, True, True, False) 'HINT Highlight my own path in case selected from 2D drawing
                MessageBox.Show(MainFormStrings.NoRoutingPathFound_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                For Each kv As KeyValuePair(Of DocumentForm, List(Of String)) In GetRoutingPathsPerDocument(e.IsCable, _entireRoutingPath)
                    If kv.Key IsNot ActiveDocument Then
                        kv.Key.SelectRowsInGrids(kv.Value, True, True, False)
                    End If
                Next
            End If
        End If

        Dim harnessPartNumbers As New List(Of String)
        Dim inlinerNames As New List(Of String)

        For Each routingPath As RoutingPath In _entireRoutingPath.Values
            harnessPartNumbers.Add(routingPath.Harness.Part_number)

            For Each routingConnection As RoutingConnection In routingPath.Connections.Values
                If (routingConnection.ConnectorA IsNot Nothing) Then
                    inlinerNames.TryAdd(routingConnection.ConnectorA.Id)
                End If
                If (routingConnection.ConnectorB IsNot Nothing) Then
                    inlinerNames.TryAdd(routingConnection.ConnectorB.Id)
                End If
            Next
        Next

        _topologyHub.SelectCompartments(harnessPartNumbers, inlinerNames)
    End Sub

    Private Function GetRoutingPathsPerDocument(isCable As Boolean, ByRef entireRoutingPath As Dictionary(Of String, RoutingPath)) As Dictionary(Of DocumentForm, List(Of String))
        Dim kblIdsPerDocument As New Dictionary(Of DocumentForm, List(Of String))
        For Each tabGroup As MdiTabGroup In Me.utmmMain.TabGroups
            For Each tab As MdiTab In tabGroup.Tabs
                Dim docForm As DocumentForm = DirectCast(tab.Form, DocumentForm)
                Dim harness As Zuken.E3.Lib.Schema.Kbl.Harness = docForm.KBL.GetHarness

                For Each routingPath As RoutingPath In entireRoutingPath.Values.Where(Function(path) path.Harness.Equals(harness))
                    For Each routingConnection As RoutingConnection In routingPath.Connections.Values
                        If (routingConnection.Core IsNot Nothing) Then
                            If (isCable) AndAlso (docForm.KBL.KBLCoreCableMapper.ContainsKey(routingConnection.Core.SystemId)) AndAlso (Not kblIdsPerDocument.ContainsKey(docForm) OrElse Not kblIdsPerDocument(docForm).Contains(docForm.KBL.KBLCoreCableMapper(routingConnection.Core.SystemId))) Then
                                Dim cableId As String = docForm.KBL.KBLCoreCableMapper(routingConnection.Core.SystemId)
                                kblIdsPerDocument.AddOrUpdate(docForm, cableId)
                            ElseIf (Not kblIdsPerDocument.ContainsKey(docForm) OrElse Not kblIdsPerDocument(docForm).Contains(routingConnection.Core.SystemId)) Then
                                kblIdsPerDocument.AddOrUpdate(docForm, routingConnection.Core.SystemId)
                            End If
                        ElseIf (Not kblIdsPerDocument.ContainsKey(docForm) OrElse kblIdsPerDocument(docForm).Contains(routingConnection.Wire.SystemId)) Then
                            kblIdsPerDocument.AddOrUpdate(docForm, routingConnection.Wire.SystemId)
                        End If
                    Next
                Next
            Next
        Next
        Return kblIdsPerDocument
    End Function

    Private Function GetCursorPositionText(x As Double, y As Double, Optional visX As Boolean = True, Optional visY As Boolean = True) As String
        Dim xStr As String = If(Not Double.IsNaN(x) AndAlso Not Double.IsInfinity(x) AndAlso visX, String.Format("X: {0:0.####}", x), String.Empty)
        Dim yStr As String = If(Not Double.IsNaN(y) AndAlso Not Double.IsInfinity(y) AndAlso visY, String.Format("Y: {0:0.####}", y), String.Empty)
        Return If(String.IsNullOrEmpty(xStr) AndAlso String.IsNullOrEmpty(yStr), String.Empty, String.Format("{0}, {1}", xStr, yStr))
    End Function

    Private Sub _activeDocument_Message(sender As Object, e As MessageEventArgs) Handles _activeDocument.Message
        Me.InvokeOrDefault(Sub() ProcessDocumentMessage(DirectCast(sender, DocumentForm), e))
    End Sub

    Public Sub ProcessDocumentMessage(document As DocumentForm, e As MessageEventArgs)
        Static lastCursorPositionX As Double = Double.NaN
        Static lastCursorPositionY As Double = Double.NaN
        Static lastMessage As String = Nothing
        Static lastProgress As Integer = -1

        With Me.usbMain
            .BeginUpdate()

            Select Case e.MessageType
                Case MessageEventArgs.MsgType.ShowCoordinates
                    If (Not Double.IsNaN(e.XPosition)) Then
                        lastCursorPositionX = e.XPosition
                    End If

                    If (Not Double.IsNaN(e.YPosition)) Then
                        lastCursorPositionY = e.YPosition
                    End If

                    .Panels("panCursorPosition").Text = GetCursorPositionText(lastCursorPositionX, lastCursorPositionY, Not Double.IsInfinity(e.XPosition), Not Double.IsInfinity(e.YPosition))

                Case MessageEventArgs.MsgType.ShowMessage
                    If e.StatusMessage IsNot Nothing Then
                        .Panels("panMessage").Text = e.StatusMessage
                    Else
                        .Panels("panMessage").Text = String.Empty
                    End If

                    .Panels("panMessage").Visible = True
                    .Panels("panCursorPosition").Text = String.Empty

                Case MessageEventArgs.MsgType.ShowProgressAndMessage

                    If _activeProgressId = Guid.Empty Then
                        _activeProgressId = e.ProgressId
                    End If

                    If _activeProgressId = e.ProgressId Then
                        If (e.StatusMessage IsNot Nothing) Then
                            lastMessage = e.StatusMessage
                        End If

                        If (e.ProgressPercentage >= 0 AndAlso e.ProgressPercentage > lastProgress) Then
                            lastProgress = e.ProgressPercentage
                        End If

                        .Panels("panMessage").Text = lastMessage

                        If (e.ProgressPercentage = 0) Then
                            .Panels("panProgress").Visible = False
                            If Not String.IsNullOrEmpty(lastMessage) Then
                                .Panels("panMessage").Visible = False
                            End If
                        Else
                            If Not usbMain.Visible Then
                                usbMain.Visible = True
                            End If
                            .Panels("panProgress").Visible = True
                            If Not String.IsNullOrEmpty(lastMessage) Then
                                .Panels("panMessage").Visible = True
                            End If
                        End If


                        If (lastProgress < 100) AndAlso lastProgress > -1 Then
                            .Panels("panProgress").ProgressBarInfo.Value = lastProgress
                        End If

                        If lastProgress = -1 OrElse lastProgress >= 100 Then
                            _activeProgressId = Guid.Empty
                            e.ProgressId = Guid.Empty
                            .Panels("panMessage").Text = String.Empty
                            .Panels("PanProgress").Visible = False
                            lastProgress = -1
                        End If

                    Else
                        If e.ProgressPercentage = 100 Then
                            e.ProgressId = Guid.Empty
                        End If
                    End If
                Case MessageEventArgs.MsgType.HideProgress
                    e.ProgressId = Guid.Empty
                    .Panels("panProgress").Visible = False
            End Select

            .EndUpdate()
            .Update()
        End With
    End Sub

    Private Sub _checkTimer_Tick(sender As Object, e As EventArgs) Handles _checkTimer.Tick
        _checkTimer.Stop()
        _checkTimer.Dispose()

        If Not IsNet45OrNewer() Then
            MessageBoxEx.ShowError(String.Format(MainFormStrings.MissingFramework_Msg, vbCrLf))
            Me.Close()
        ElseIf (My.Application.CommandLineArgs.Count = 0) Then
            Me.utmMain.Ribbon.ApplicationMenu2010.DropDown(Me.Tools.Application.RecentDocuments)
        End If
    End Sub

    Private Sub _mainStateMachine_DocumentOpenFinished(sender As Object, e As DocumentOpenFinshedEventArgs) Handles _mainStateMachine.DocumentOpenFinished
        With Me.utmMain
            Me.Tools.Application.CloseDocument.SharedProps.Enabled = True
            Me.Tools.Application.CloseDocuments.SharedProps.Enabled = Me.utmmMain.TabGroups.Count <> 0 AndAlso Me.utmmMain.TabGroups(0).Tabs.Count > 1
            Me.Tools.Application.Export.SharedProps.Enabled = True

            Me.Tools.Home.CompareData.SharedProps.Enabled = True
            Me.Tools.Home.CompareGraphic.SharedProps.Enabled = True
            Me.Tools.Home.Inliners.SharedProps.Enabled = e.Document.IsExtendedHCV
            Me.Tools.Home.Search.SharedProps.Enabled = True
            Me.Tools.Home.LoadCarTransformation.SharedProps.Visible = Not _mainStateMachine.IsXhcv
            Me.Tools.Home.SaveCarTransformation.SharedProps.Visible = Not _mainStateMachine.IsXhcv

            Me.Tools.Home.HideNoModuleEntities.SharedProps.Enabled = True
            Me.Tools.Home.BundleCalculator.SharedProps.Enabled = True
        End With

        If e.Document.KBL IsNot Nothing Then
            Me.utmMain.Ribbon.Tabs(NameOf(MenuButtonStrings.Home)).Groups(NameOf(MenuButtonStrings._3D)).Visible = HasView3DFeature
            'HINT: consolidated 3dview loading is done by the attachment of document to consolidated3dControl (see DocumentControl3D.FileViewAdapter)

            If HasSchematicsFeature Then
                Me.AddDocumentToAdvConnectivityView(e.Document, Sub() RaiseEvent DocumentOpenTotalFinished(Me, e)) 'HINT: this method is async but not awaitable and therefore it comes at the end of the document open, to not overlap the progressbars (would result in 2x progress parallel)
            Else
                RaiseEvent DocumentOpenTotalFinished(Me, e) ' HINT: for automatic tests to get the end of the whole process more safer
            End If
        Else
            RaiseEvent DocumentOpenTotalFinished(Me, e) ' HINT: for automatic tests to get the end of the whole process more safer
        End If
    End Sub

    Friend Function RemoveDocumentFromConnectivityView(docId As String) As Boolean
        If HasSchematicsFeature AndAlso Me.SchematicsView IsNot Nothing Then
            Dim kblblocks As KblDocumentDataCollection = Me.SchematicsView.KblBlocks
            If kblblocks IsNot Nothing Then
                Return kblblocks.Remove(docId.ToString)
            End If
        End If
        Return False
    End Function

    Private Sub AddDocumentToAdvConnectivityView(document As DocumentForm, Optional finishedProcessing As Action = Nothing)
        If HasSchematicsFeature AndAlso Me.SchematicsView IsNot Nothing Then
            Dim kblblocks As KblDocumentDataCollection = Me.SchematicsView.KblBlocks
            If kblblocks IsNot Nothing Then
                Dim genFinished As KblDocumentDataCollection.GenerationFinishedEventHandler = Nothing
                genFinished = Sub()
                                  RemoveHandler kblblocks.GenerationFinished, genFinished
                                  If finishedProcessing IsNot Nothing Then
                                      finishedProcessing.Invoke
                                  End If
                              End Sub

                If kblblocks.AutoGenerate = KblDocumentDataCollection.AutoGenerateType.Async Then
                    AddHandler kblblocks.GenerationFinished, genFinished
                End If

                kblblocks.AddNew(document.Id.ToString, document.KBL)

                If kblblocks.AutoGenerate = KblDocumentDataCollection.AutoGenerateType.Default Then
                    genFinished.Invoke(Nothing, Nothing)
                End If
            ElseIf finishedProcessing IsNot Nothing Then
                finishedProcessing.Invoke
            End If
        ElseIf finishedProcessing IsNot Nothing Then
            finishedProcessing.Invoke
        End If
    End Sub

    Private Sub _searchMachine_SearchEvent(sender As Object, kblMapperId As String, objectType As String, objectId As String) Handles _searchMachine.SearchEvent
        Dim kblIds As New List(Of String)
        kblIds.Add(objectId)

        If (ActiveDocument.KBL.Id = kblMapperId) Then
            ActiveDocument.SelectRowsInGrids(kblIds, True, True)
        Else
            For Each tabGroup As MdiTabGroup In Me.utmmMain.TabGroups
                For Each documentTab As MdiTab In tabGroup.Tabs
                    Dim documentForm As DocumentForm = DirectCast(documentTab.Form, DocumentForm)

                    If (documentForm.KBL.Id = kblMapperId) Then
                        documentTab.Activate()
                        documentForm.SelectRowsInGrids(kblIds, True, True)
                    End If
                Next
            Next
        End If
    End Sub

    Private Sub _topologyHub_CompartmentClicked(sender As TopologyHub, e As String) Handles _topologyHub.CompartmentClicked
        For Each tabGroup As MdiTabGroup In Me.utmmMain.TabGroups
            For Each documentTab As MdiTab In tabGroup.Tabs
                Dim documentForm As DocumentForm = DirectCast(documentTab.Form, DocumentForm)
                If (documentForm.Tag.ToString = e) Then
                    documentTab.Activate()
                End If
            Next
        Next
    End Sub

    Private Sub udmMain_MouseEnterElement(sender As Object, e As Infragistics.Win.UIElementEventArgs) Handles udmMain.MouseEnterElement
        If (TypeOf e.Element Is PaneCaptionButtonUIElement) AndAlso (CType(e.Element, PaneCaptionButtonUIElement).PaneButtonType = PaneButton.Maximize) Then
            _isOverMaximize = True
            _currentMaximizePane = CType(e.Element.GetContext(GetType(DockableControlPane)), DockableControlPane)
        End If
    End Sub

    Private Sub udmMain_MouseLeaveElement(sender As Object, e As Infragistics.Win.UIElementEventArgs) Handles udmMain.MouseLeaveElement
        If (TypeOf e.Element Is PaneCaptionButtonUIElement) AndAlso (CType(e.Element, PaneCaptionButtonUIElement).PaneButtonType = PaneButton.Maximize) Then
            _isOverMaximize = False
            _currentMaximizePane = Nothing
        End If
    End Sub

    Private Sub udmMain_PaneHidden(sender As Object, e As PaneHiddenEventArgs) Handles udmMain.PaneHidden
        If TypeOf D3D Is D3D.MainFormController Then
            CType(D3D, D3D.MainFormController).OnAfterPaneHidden(e.Pane)
        End If
    End Sub

    Private Sub udmMain_PaneDisplayed(sender As Object, e As PaneDisplayedEventArgs) Handles udmMain.PaneDisplayed
        If TypeOf D3D Is D3D.MainFormController Then
            CType(D3D, D3D.MainFormController).OnAfterPaneDisplayed(e.Pane)
        End If
    End Sub

    Private Sub utmmBrowser_TabActivated(sender As Object, e As MdiTabEventArgs) Handles utmmMain.TabActivated
        ActiveDocument = TryCast(e.Tab.Form, DocumentForm)

        If ActiveDocument IsNot Nothing AndAlso _documentViews?.Contains(ActiveDocument.Id) Then
            _documentViews(ActiveDocument.Id).Active = True
            _topologyHub.SelectActiveCompartments()

            Me.Tools.Application.SaveDocumentAs.SharedProps.Enabled = KnownFile.IsHcv(ActiveDocument.File.FullName)
            Me.Tools.Application.SaveDocument.SharedProps.Enabled = ActiveDocument.IsDirty
        End If
    End Sub

    Private Sub utmmBrowser_TabClosed(sender As Object, e As MdiTabEventArgs) Handles utmmMain.TabClosed
        Dim doc As DocumentForm = TryCast((e.Tab?.Form), DocumentForm)
        If (Me.utmmMain.ActiveTab Is Nothing) AndAlso ActiveDocument IsNot Nothing Then
            _documentsDic.Remove(ActiveDocument.Id.ToString)
            _documentsOfLastConverter.Remove(ActiveDocument.Id.ToString)
            ActiveDocument = Nothing
        End If
    End Sub

    Private Sub _schematicsView_ActiveIdChanged(sender As Object, e As EventArgs) Handles _schematicsView.ActiveEntitiesChanged
        Dim viewConnPane As Infragistics.Win.UltraWinDock.DockableControlPane = Me.udmMain.PaneFromControl(_schematicsView)
        viewConnPane.Text = String.Format("{0}: {1}", UIStrings.SchematicsViewPane_Caption, String.Join(";", _schematicsView.ActiveEntities.GroupBy(Function(ent) ent.Type).Select(Function(grp) String.Format("{0} ({1})", String.Join(",", grp.Select(Function(ent) ent.Name)), grp.FirstOrDefault?.TypRessourceString))))
    End Sub

    Friend Sub ShowActiveCavityChecker()
        If _cavityCheckForm IsNot Nothing Then
            If Not _cavityCheckForm.Visible Then
                _cavityCheckForm.Show(Me)
            End If
        Else
            Debug.Fail(NameOf(_cavityCheckForm) & " is null!")
        End If
    End Sub

    Friend Sub InvalidateActiveCavityChecker()
        If ActiveDocument IsNot Nothing Then
            If _documentViews.Contains(ActiveDocument.Id) Then
                _documentViews(ActiveDocument.Id).Invalidate()
            End If
        End If
    End Sub

    Friend Property ActiveDocument() As DocumentForm
        Get
            Return _activeDocument
        End Get
        Set(value As DocumentForm)
            _activeDocument = value
        End Set
    End Property

    Friend ReadOnly Property DiameterSettings() As DiameterSettings Implements IHarnessAnalyzerSettingsProvider.DiameterSettings
        Get
            Return _diameterSettings
        End Get
    End Property

    Friend ReadOnly Property WeightSettings As WeightSettings
        Get
            Return _weightSettings
        End Get
    End Property

    Friend ReadOnly Property EntireRoutingPath() As Dictionary(Of String, RoutingPath)
        Get
            Return _entireRoutingPath
        End Get
    End Property

    Friend ReadOnly Property FontAndShapeFilesDir() As String
        Get
            Return _fontAndShapeFilesDir
        End Get
    End Property

    <ObfuscationAttribute(Feature:="renaming")>
    Friend ReadOnly Property GeneralSettings() As GeneralSettings
        Get
            Return _generalSettings
        End Get
    End Property

    <Obfuscation(Feature:="renaming")>
    Friend ReadOnly Property HasView3DFeature As Boolean Implements IHarnessAnalyzerSettingsProvider.HasView3DFeature
        Get
            Return AppInitManager.LicensedFeatures.HasFlag(ApplicationFeature.E3HarnessAnalyzer3D)
        End Get
    End Property

    <Obfuscation(Feature:="renaming")>
    Friend ReadOnly Property HasTopoCompareFeature As Boolean Implements IHarnessAnalyzerSettingsProvider.HasTopoCompareFeature
        Get
            Return AppInitManager.LicensedFeatures.HasFlag(ApplicationFeature.E3HarnessAnalyzerTopoCompare)
        End Get
    End Property

    <Obfuscation(Feature:="renaming")>
    Friend ReadOnly Property HasSchematicsFeature As Boolean Implements IHarnessAnalyzerSettingsProvider.HasSchematicsFeature
        Get
            Return AppInitManager.LicensedFeatures.HasFlag(ApplicationFeature.E3HarnessAnalyzerConBrow)
        End Get
    End Property

    Public ReadOnly Property AppInitManager As InitManager
        Get
            'HINT: this is needed here, because calling  My.Application from another thread will give another Application context. To get always to main application context this is done over the mainform-invoke-call
            If Me.InvokeRequired Then
                Return DirectCast(Me.Invoke(Function() My.Application.InitManager), InitManager)
            Else
                Return My.Application.InitManager
            End If
        End Get
    End Property

    Friend ReadOnly Property MainStateMachine() As MainStateMachine
        Get
            Return _mainStateMachine
        End Get
    End Property

    Public Property NoModulesVisible As Boolean
        Get
            Return Not Me.Tools.Home.HideNoModuleEntities.Checked
        End Get
        Set(value As Boolean)
            Me.Tools.Home.HideNoModuleEntities.Checked = Not value
        End Set
    End Property

    Friend ReadOnly Property QMStampSpecifications() As QMStampSpecifications Implements IHarnessAnalyzerSettingsProvider.QMStampSpecifications
        Get
            Return _qmStampSpecifications
        End Get
    End Property

    Friend ReadOnly Property SearchMachine() As SearchMachine
        Get
            Return _searchMachine
        End Get
    End Property

    Friend ReadOnly Property TopologyHub() As TopologyHub
        Get
            Return _topologyHub
        End Get
    End Property

    Public ReadOnly Property D3D As D3D.ID3DAccessor
        Get
            Return MainFormController
        End Get
    End Property

    Friend ReadOnly Property ToastManager As LogHubToastManager
        Get
            Return _toastManager
        End Get
    End Property

    Friend ReadOnly Property SchematicsView As Schematics.Controls.ViewControl
        Get
            Return _schematicsView
        End Get
    End Property

    Public ReadOnly Property DocumentViews As Checks.Cavities.Views.Document.DocumentViewCollection
        Get
            Return _documentViews
        End Get
    End Property

    Friend Function ShowTopologyHub(xhcv As XhcvFile) As Boolean
        If (xhcv.TopologyView IsNot Nothing) AndAlso (TopologyHub IsNot Nothing) Then
            If Not _mainStateMachine.IsLoadingXhcv OrElse Not _topoHubWasShown Then
                If Me.Panes.TopologyHubPane Is Nothing Then
                    InitializeTopologyHub()
                End If

                If Not Me.Panes.TopologyHubPane.IsVisible Then
                    TopologyHub.Initialize(xhcv.TopologyView)
                    TopologyHub.ToggleActiveHarnesses(utmmMain)

                    udmMain.BeginUpdate()
                    Me.Panes.TopologyHubPane.Show()
                    Me.Panes.TopologyHubPane.Float(True, New System.Drawing.Point(225, 225))
                    udmMain.EndUpdate()
                    TopologyHub.SelectActiveCompartments()
                Else
                    TopologyHub.ToggleActiveHarnesses(utmmMain)
                End If

                _topoHubWasShown = True
                Return True
            End If
        End If
        Return False
    End Function

    Friend Sub OnStartLoadingXhcv()
        _topoHubWasShown = False
    End Sub

    <DebuggerStepThrough>
    Private Function PreFilterMessage(ByRef m As Message) As Boolean Implements IMessageFilter.PreFilterMessage
        Static WM_LBUTTONDOWN As Integer = &H201
        Static WM_LBUTTONUP As Integer = &H202
        Static mouseWasDown As Boolean = False
        If m.Msg = WM_LBUTTONDOWN Then
            If _isOverMaximize Then
                mouseWasDown = True
            End If
        ElseIf m.Msg = WM_LBUTTONUP Then
            If mouseWasDown AndAlso _isOverMaximize Then
                Dim currRect As New Rectangle(_currentMaximizePane.DockAreaPane.FloatingLocation, _currentMaximizePane.DockAreaPane.Size)
                Dim workingArea As Rectangle = Screen.FromPoint(System.Windows.Forms.Form.MousePosition).WorkingArea
                workingArea.Width = workingArea.Width - 6
                workingArea.Height = workingArea.Height - 6
                Dim IsTopologyHub As Boolean = _currentMaximizePane.Key = PaneKeys.TopologyHub.ToString

                Dim previousRect As System.Drawing.Rectangle
                If IsTopologyHub Then
                    previousRect = My.Settings.PreviousTopologyRect
                Else
                    previousRect = My.Settings.PreviousD3DRect
                End If

                If Not currRect.Equals(workingArea) Then
                    _currentMaximizePane.Float(False, workingArea)
                ElseIf previousRect <> Rectangle.Empty Then
                    _currentMaximizePane.Float(False, previousRect)
                Else
                    _currentMaximizePane.Float(False, New Rectangle(Me.Location.X + CInt((Me.Width / 2)), Me.Location.Y + CInt((Me.Height / 2)), CInt(Me.Width / 2), CInt(Me.Height / 2)))
                End If

                If IsTopologyHub Then
                    My.Settings.PreviousTopologyRect = currRect
                Else
                    My.Settings.PreviousD3DRect = currRect
                End If
            End If

            mouseWasDown = False
        End If
        Return False
    End Function

    Private Sub _d3dController_Progress(sender As Object, e As DocumentFormMessageEventArgs) Handles MainFormController.Progress
        ProcessDocumentMessage(e.DocumentFrm, e)
    End Sub

    Private Sub MainForm_HandleCreated(sender As Object, e As EventArgs) Handles Me.HandleCreated
        'HINT: set window handle to mainform for making screenshots for unhandled exception debug package creation
        For Each sEntry As Exceptions.Issues.Entries.ScreenshotEntry In Exceptions.Issues.ExceptionReportSetting.Current.Entries.OfType(Of Exceptions.Issues.Entries.ScreenshotEntry)
            sEntry.WindowHwnd = Me.Handle
        Next
    End Sub

    Public ReadOnly Property Panes As TabToolPanesCollection
        Get
            Return _panes
        End Get
    End Property

    Public ReadOnly Property Tools As UtMainTools

    Private ReadOnly Property IHarnessAnalyzerSettingsProvider_GeneralSettings As GeneralSettingsBase Implements IHarnessAnalyzerSettingsProvider.GeneralSettings
        Get
            Return Me.GeneralSettings
        End Get
    End Property

    Protected Overrides Sub OnClosing(e As CancelEventArgs)
        MyBase.OnClosing(e)
        If Not e.Cancel Then
            My.Settings.Save()
        End If
    End Sub

    Friend Sub SaveConsolidated3DSettings()
        MainFormController?.SaveConsolidated3DSettings()
    End Sub

    Friend Sub SaveAdvConnectivitySettings()
        MainFormController?.SaveAdvConnectivitySettings()
    End Sub


End Class