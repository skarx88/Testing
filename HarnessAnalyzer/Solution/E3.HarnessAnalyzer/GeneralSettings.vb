Imports System.ComponentModel
Imports System.IO
Imports System.Reflection
Imports System.Xml.Serialization
Imports Zuken.E3.HarnessAnalyzer.Schematics.Identifier.Component
Imports Zuken.E3.HarnessAnalyzer.Settings.Schematics.Identifier.Component

Namespace Settings

    <ObfuscationAttribute(Feature:="renaming", ApplyToMembers:=False)>
    <XmlInclude(GetType(BundleDiameterRange))>
    <XmlInclude(GetType(InlinerIdentifier))>
    <XmlInclude(GetType(RecentFile))>
    <XmlInclude(GetType(WireColorCode))>
    <Serializable()>
    Public Class GeneralSettings
        Inherits GeneralSettingsBase

        Private Shared _defaultSettings As GeneralSettingsResult

        Public Sub New()
            MyBase.New()
        End Sub

        Protected Overrides Function CreateComponentIdentifierGroupsInstance() As IdentifierGroupCollectionBase
            Return New IdentifierGroupCollection()
        End Function

        Public Shared ReadOnly Property Current As GeneralSettingsResult
            Get
                If _defaultSettings Is Nothing Then
                    Dim generalSettingsConfigFile As String = HarnessAnalyzer.D3D.MainFormController.GetGeneralSettingsConfigurationFile()
                    If IO.File.Exists(generalSettingsConfigFile) Then
                        _defaultSettings = LoadGeneralSettingsFile(generalSettingsConfigFile)
                    Else
                        _defaultSettings = New GeneralSettingsResult(ResultState.Faulted, $"File ""{generalSettingsConfigFile}"" not found!")
                    End If

                    If _defaultSettings.IsFaulted Then
                        Dim defaultFullPath As String = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), E3.HarnessAnalyzer.Shared.DEFAULT_GENERAL_SETTINGS_FILE)
                        If (IO.File.Exists(defaultFullPath)) Then
                            IO.File.Copy(defaultFullPath, generalSettingsConfigFile, True)

                            _defaultSettings = LoadGeneralSettingsFile(generalSettingsConfigFile)

                            If (_defaultSettings.IsFaulted) Then
                                CreateDefaultGeneralSettings(generalSettingsConfigFile)
                            End If
                        Else
                            CreateDefaultGeneralSettings(generalSettingsConfigFile)
                        End If
                    Else
                        If (_defaultSettings.GeneralSettings.InlinerPairCheckClassifications.Count = 0) Then
                            _defaultSettings.GeneralSettings.InlinerPairCheckClassifications.CreateDefaultInlinerPairCheckClassifications()
                        End If

                        If (_defaultSettings.GeneralSettings.MoveDistanceToleranceForGraphicalCompare = 0) Then
                            _defaultSettings.GeneralSettings.MoveDistanceToleranceForGraphicalCompare = 10
                            _defaultSettings.GeneralSettings.Save(generalSettingsConfigFile)
                        End If

                        If (_defaultSettings.GeneralSettings.MoveDistanceToleranceMaxForGraphicalCompare = 0) Then
                            _defaultSettings.GeneralSettings.MoveDistanceToleranceMaxForGraphicalCompare = 100
                            _defaultSettings.GeneralSettings.Save(generalSettingsConfigFile)
                        End If

                        If (_defaultSettings.GeneralSettings.CarModelDirectory = String.Empty) Then
                            _defaultSettings.GeneralSettings.CarModelDirectory = FileIO.SpecialDirectories.MyDocuments
                        End If
                        If (_defaultSettings.GeneralSettings.ConnectorFacesDirectory = String.Empty) Then
                            _defaultSettings.GeneralSettings.ConnectorFacesDirectory = FileIO.SpecialDirectories.MyDocuments
                        End If
                        If (_defaultSettings.GeneralSettings.RedliningStampIndicatorScaleFactor = -1) Then
                            _defaultSettings.GeneralSettings.RedliningStampIndicatorScaleFactor = 1.0
                        End If
                        If (_defaultSettings.GeneralSettings.ResistivityAluminium = -1) Then
                            _defaultSettings.GeneralSettings.ResistivityAluminium = 2.6548
                        End If
                        If (_defaultSettings.GeneralSettings.ResistivityCopper = -1) Then
                            _defaultSettings.GeneralSettings.ResistivityCopper = 1.678
                        End If
                        If (_defaultSettings.GeneralSettings.TemperatureCoefficientAluminium = -1) Then
                            _defaultSettings.GeneralSettings.TemperatureCoefficientAluminium = 4.0
                        End If
                        If (_defaultSettings.GeneralSettings.TemperatureCoefficientCopper = -1) Then
                            _defaultSettings.GeneralSettings.TemperatureCoefficientCopper = 3.93
                        End If
                        If (_defaultSettings.GeneralSettings.ThresholdNofInstancesWithIdenticalOffsetValue = -1) Then
                            _defaultSettings.GeneralSettings.ThresholdNofInstancesWithIdenticalOffsetValue = 0.75
                        End If
                        If String.IsNullOrEmpty(_defaultSettings.GeneralSettings.UILanguage) Then
                            _defaultSettings.GeneralSettings.UILanguage = If(Globalization.CultureInfo.CurrentCulture.Name = "de-DE", "de-DE", "en-US")
                        End If
                        If (_defaultSettings.GeneralSettings.UltrasonicSpliceIdentifiers.Count = 0) Then
                            _defaultSettings.GeneralSettings.UltrasonicSpliceIdentifiers.CreateDefaultIdentifiers()
                        End If
                    End If
                End If
                Return _defaultSettings
            End Get
        End Property

        Private Shared Sub CreateDefaultGeneralSettings(Optional createFilePath As String = Nothing)
            _defaultSettings = New GeneralSettingsResult(New GeneralSettings)
            _defaultSettings.GeneralSettings.ResetToDefaults()

            If (Not String.IsNullOrEmpty(createFilePath)) Then
                _defaultSettings.GeneralSettings.Save(createFilePath)
            End If
        End Sub

        Private Shared Function LoadGeneralSettingsFile(Optional generalSettingsConfigFile As String = Nothing) As GeneralSettingsResult
            If (String.IsNullOrEmpty(generalSettingsConfigFile)) Then
                generalSettingsConfigFile = HarnessAnalyzer.D3D.MainFormController.GetGeneralSettingsConfigurationFile()
            End If

            Dim mySettings As GeneralSettings = GeneralSettings.LoadFromFile(Of GeneralSettings)(generalSettingsConfigFile)

            If (mySettings Is Nothing) Then
                Return New GeneralSettingsResult(ResultState.Faulted, String.Format(MainFormStrings.ErrorLoadAppSettings_Msg, vbCrLf, GeneralSettings.LoadError.Message))
            End If

            Return New GeneralSettingsResult(ResultState.Success, mySettings)
        End Function

        Public Class GeneralSettingsResult
            Inherits System.ComponentModel.Result(Of GeneralSettings)

            Public Sub New(data As GeneralSettings)
                MyBase.New(data)
            End Sub

            Public Sub New(result As System.ComponentModel.ResultState, Optional data As GeneralSettings = Nothing)
                MyBase.New(result, data)
            End Sub

            Public Sub New(exception As Exception, Optional data As GeneralSettings = Nothing)
                MyBase.New(exception, data)
            End Sub

            Public Sub New(result As System.ComponentModel.ResultState, message As String, Optional data As GeneralSettings = Nothing)
                MyBase.New(result, message, data)
            End Sub

            Protected Sub New()
            End Sub

            Protected Friend Sub New(other As System.ComponentModel.IResult)
                MyBase.New(other)
            End Sub

            ReadOnly Property GeneralSettings As GeneralSettings
                Get
                    Return MyBase.Data
                End Get
            End Property
        End Class

    End Class

End Namespace
