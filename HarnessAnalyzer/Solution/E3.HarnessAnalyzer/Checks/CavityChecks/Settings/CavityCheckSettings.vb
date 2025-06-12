Imports System.IO
Imports System.Runtime.Serialization
Imports System.Xml
Imports Zuken.E3.HarnessAnalyzer.Checks.Cavities.Views.Model
Imports Zuken.E3.HarnessAnalyzer.Compatibility

Namespace Checks.Cavities.Settings

    <DataContract()>
    <KnownType(GetType(ConnectorSettingCollection))>
    <KnownType(GetType(ModuleSettingCollection))>
    Public Class CavityCheckSettings
        Public Event Updated(sender As Object, e As EventArgs)

        Public Const FILENAME_DEFAULT As String = "CavityCheckSettings.xml"
        Private Const KBL_MODEL_FIELD_NAME As String = "_kbl"

        <DataMember> Private _activeHarnessConfigurationId As String = String.Empty

        Public Sub New(oldsettings As CavityCheckSettings_2023)
            _activeHarnessConfigurationId = oldsettings.ActiveHarnessConfigurationId
            For Each conn As ConnectorSetting_2023 In oldsettings.Connectors
                Me.Connectors.Add(New ConnectorSetting(conn))
            Next
            For Each oldModuleSetting As ModuleSetting_2023 In oldsettings.ActiveModules
                Me.ActiveModules.Add(New ModuleSetting(oldModuleSetting))
            Next
        End Sub

        Public Sub New(model As Views.Model.ModelView)
            Me.Model = model
        End Sub

        Private ReadOnly Property KblFromModel As KblMapper
            Get
                If Model IsNot Nothing Then
                    Return Reflection.UtilsEx.GetFieldValues(Of KblMapper)(Model).FirstOrDefault
                End If
                Return Nothing
            End Get
        End Property

        <IgnoreDataMember>
        Property Model As Views.Model.ModelView

        <DataMember>
        Property Connectors As New ConnectorSettingCollection

        <DataMember>
        Property ActiveModules As New ModuleSettingCollection

        <IgnoreDataMember>
        Property ActiveHarnessConfigurationId As String
            Get
                Return _activeHarnessConfigurationId
            End Get
            Set(value As String)
                If _activeHarnessConfigurationId <> value Then
                    _activeHarnessConfigurationId = value
                    _IsDirty = True
                End If
            End Set
        End Property

        ReadOnly Property IsDirty As Boolean

        Public Sub Update()
            UpdateFromModelCore()
        End Sub

        Private Sub UpdateFromModelCore()
            If Model Is Nothing Then
                Throw New ArgumentNullException($"Property ""{NameOf(Me.Model)}"" can't be empty for an update!", NameOf(Model))
            End If

            Dim kbl As KblMapper = KblFromModel

            If kbl Is Nothing Then
                Throw New ArgumentNullException($"KblContainer in property ""{NameOf(Me.Model)}"" can't be empty for an update!", NameOf(Model) & "." & KBL_MODEL_FIELD_NAME)
            End If

            Dim oldConnectorIds As List(Of String) = Connectors.Select(Function(conn) conn.ConnectorId).ToList
            Dim changedWireIds As List(Of String) = Connectors.SelectMany(Function(c) c.Cavities).Where(Function(cav) cav.Checked <> CheckState.Indeterminate).Select(Function(cav) cav.WireId).ToList
            Dim oldModuleIds As List(Of String) = ActiveModules.Select(Function(m) m.SystemId).ToList

            Me.ActiveModules.Clear()
            Me.Connectors.Clear()
            Dim newChangedWireIds As New List(Of String)

            Try
                For Each m As [Lib].Schema.Kbl.[Module] In kbl.GetModules
                    If Not kbl.InactiveModules.ContainsKey(m.SystemId) Then
                        Dim ms As New Settings.ModuleSetting(m)
                        ActiveModules.Add(ms)
                    End If
                Next

                For Each conn As ConnectorView In Model.Connectors.Where(Function(c) c.Visible) 'HINT: only connectors available in selected modules
                    Dim connSetting As New ConnectorSetting(conn.KblId, conn.Name)
                    With connSetting
                        For Each cavWire As Views.Model.CavityWireView In conn.CavWires ' HINT: here we need ALL cavWires because the user can also edit the deactivated (which are not included in the selected modules)
                            Dim cavSetting As New CavityWireSetting(cavWire.KblCavityId, cavWire.KblWireId, cavWire.CheckState)
                            With cavSetting
                                .CavityName = cavWire.CavityName
                                .WireNumber = cavWire.WireName
                                If .Checked <> CheckState.Indeterminate Then
                                    newChangedWireIds.Add(.WireId)
                                End If
                            End With
                            .Cavities.Add(cavSetting)
                        Next
                    End With
                    Connectors.Add(connSetting)
                Next
            Finally
                _IsDirty = Not Me.Connectors.Select(Function(c) c.ConnectorId).SequenceEqual(oldConnectorIds) OrElse Not Me.ActiveModules.Select(Function(m) m.SystemId).SequenceEqual(oldModuleIds) OrElse Not changedWireIds.SequenceEqual(newChangedWireIds)
                OnUpdated(Me, New EventArgs)
            End Try
        End Sub

        Public Sub SaveToFile(filePath As String)
            Using fs As New FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None)
                Save(fs)
            End Using
        End Sub

        Public Sub Save(s As System.IO.Stream)
            Dim settings As New XmlWriterSettings
            settings.Indent = True
            settings.ConformanceLevel = ConformanceLevel.Auto

            Using xmlWriter As XmlWriter = XmlWriter.Create(s, settings)
                Dim ser As New DataContractSerializer(GetType(Settings.CavityCheckSettings))
                ser.WriteObject(xmlWriter, Me)
            End Using
        End Sub

        Public Sub ApplyToModel()
            If Model Is Nothing Then
                Throw New ArgumentNullException($"Property ""{NameOf(Me.Model)}"" must be set before {NameOf(ApplyToModel)}!", NameOf(Model))
            End If
            Me.ApplyTo(Me.Model)
        End Sub

        Public Sub ApplyTo(model As Views.Model.ModelView)
            Dim modelConns As Dictionary(Of String, ConnectorView) = model.Connectors.ToDictionary(Function(c) c.KblId, Function(c) c)

            For Each connKv As ConnectorSetting In Me.Connectors
                Dim modelConn As Views.Model.ConnectorView = Nothing
                If modelConns.ContainsKey(connKv.ConnectorId) Then
                    modelConn = modelConns(connKv.ConnectorId)
                End If

                If modelConn IsNot Nothing Then
                    connKv.ApplyTo(modelConn)
                End If
            Next

            If Me.Model Is model Then
                _IsDirty = False
            End If
        End Sub

        Public Shared Function GetSettingsFilePath(directory As String) As String
            Return Path.Combine(If(directory, String.Empty), FILENAME_DEFAULT)
        End Function

        Public Shared Function LoadFromFile(filePath As String) As CavityCheckSettings
            Using fs As New FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)
                Return Load(fs)
            End Using
        End Function

        Public Shared Function Load(s As System.IO.Stream) As CavityCheckSettings
            If CavityCheckSettings_2023.Is2023Xml(s) Then
                Dim compSettings2023 As CavityCheckSettings_2023 = Compatibility.CavityCheckSettings_2023.Load(s)
                Return New CavityCheckSettings(compSettings2023)
            Else
                Dim settings As New XmlReaderSettings()
                settings.CloseInput = False
                Using reader As XmlReader = XmlReader.Create(s, settings)
                    Dim ser As New DataContractSerializer(GetType(CavityCheckSettings))
                    Return CType(ser.ReadObject(reader), CavityCheckSettings)
                End Using
            End If
        End Function

        Protected Overridable Sub OnUpdated(sender As Object, e As EventArgs)
            RaiseEvent Updated(sender, e)
        End Sub

    End Class

    <DataContract>
    Public Class CavityWireSetting
        <DataMember> Private _connectorId As String
        <DataMember> Private _cavityId As String
        <DataMember> Private _wireId As String

        Public Sub New(oldSetting As CavityWireSetting_2023)
            Me.New(oldSetting.CavityId, oldSetting.WireId, CType(oldSetting.Checked, CheckState))
            _connectorId = oldSetting.ConnectorId
            _WireNumber = oldSetting.WireNumber
            _CavityName = oldSetting.CavityName
        End Sub

        Friend Sub New()
        End Sub

        Public Sub New(cavityId As String, wireId As String, Optional checked As CheckState = CheckState.Indeterminate)
            _cavityId = cavityId
            _wireId = wireId
            Me.Checked = checked
        End Sub

        <DataMember> Property CavityName As String
        <DataMember> Property WireNumber As String
        <DataMember> Property Checked As CheckState

        ReadOnly Property ConnectorId As String
            Get
                Return _connectorId
            End Get
        End Property

        ReadOnly Property CavityId As String
            Get
                Return _cavityId
            End Get
        End Property

        ReadOnly Property WireId As String
            Get
                Return _wireId
            End Get
        End Property

        Friend Sub ApplyTo(cv As CavityWireView)
            With cv
                .CheckState = Me.Checked
            End With
        End Sub

    End Class

    <DataContract>
    <KnownType(GetType(CavityWireSetting))>
    Public Class ConnectorSetting

        <DataMember> Private _name As String
        <DataMember> Private _connectorId As String

        Public Sub New(oldSetting As ConnectorSetting_2023)
            Me.New(oldSetting.ConnectorId, oldSetting.Name)
            For Each oldCavSetting As CavityWireSetting_2023 In oldSetting.Cavities
                Me.Cavities.Add(New CavityWireSetting(oldCavSetting))
            Next
        End Sub

        Public Sub New(connectorId As String, name As String)
            _connectorId = connectorId
            _name = name
        End Sub

        ReadOnly Property ConnectorId As String
            Get
                Return _connectorId
            End Get
        End Property

        ReadOnly Property Name As String
            Get
                Return _name
            End Get
        End Property

        <DataMember>
        Property Cavities As New List(Of CavityWireSetting)

        Friend Sub ApplyTo(conn As Views.Model.ConnectorView)
            Dim cavWireSets As Dictionary(Of String, List(Of CavityWireSetting)) = Me.Cavities.GroupBy(Function(cav) cav.WireId).ToDictionary(Function(grp) grp.Key, Function(grp) grp.ToList)
            For Each cv As Views.Model.CavityWireView In conn.CavWires
                If cavWireSets.ContainsKey(cv.KblWireId) Then
                    Dim cavWireSet As List(Of CavityWireSetting) = cavWireSets(cv.KblWireId)
                    For Each cavWSet As CavityWireSetting In cavWireSet
                        cavWSet.ApplyTo(cv)
                    Next
                End If
            Next
        End Sub

    End Class

    <DataContract>
    Public Class ModuleSetting

        Public Sub New(oldSetting As ModuleSetting_2023)
            Me.SystemId = oldSetting.SystemId
            Me.Part_number = oldSetting.Part_number
            Me.Company_name = oldSetting.Company_name
            Me.Version = oldSetting.Version
            Me.Abbreviation = oldSetting.Abbreviation
            Me.Description = oldSetting.Description
            Me.Car_classification_level_2 = oldSetting.Car_classification_level_2
            Me.Model_year = oldSetting.Model_year
            Me.Content = CType(oldSetting.Content, Module_content)
        End Sub

        Public Sub New(m As [Lib].Schema.Kbl.[Module])
            Me.SystemId = m.SystemId
            Me.Part_number = m.Part_number
            Me.Company_name = m.Company_name
            Me.Version = m.Version
            Me.Abbreviation = m.Abbreviation
            Me.Description = m.Description
            Me.Car_classification_level_2 = m.Car_classification_level_2
            Me.Model_year = m.Model_year
            Me.Content = m.Content
        End Sub

        <DataMember> Property SystemId As String
        <DataMember> Property Part_number As String
        <DataMember> Property Company_name As String
        <DataMember> Property Version As String
        <DataMember> Property Abbreviation As String
        <DataMember> Property Description As String
        <DataMember> Property Car_classification_level_2 As String
        <DataMember> Property Model_year As String
        <DataMember> Property Content As Module_content

        Public Overrides Function ToString() As String
            Return String.Format("{0}; Part number: {1}", Me.Abbreviation, Me.Part_number.ToString)
        End Function

    End Class

End Namespace