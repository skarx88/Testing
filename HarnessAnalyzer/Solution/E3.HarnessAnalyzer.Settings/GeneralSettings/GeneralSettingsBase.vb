Imports System.ComponentModel
Imports System.IO
Imports System.Reflection
Imports System.Xml.Serialization
Imports Zuken.E3.HarnessAnalyzer.Shared

<ObfuscationAttribute(Feature:="renaming", ApplyToMembers:=False)>
<Serializable()>
<XmlInclude(GetType(BundleDiameterRange))>
<XmlInclude(GetType(InlinerIdentifier))>
<XmlInclude(GetType(RecentFile))>
<XmlInclude(GetType(WireColorCode))>
Public MustInherit Class GeneralSettingsBase

    Public Event Modified(sender As Object, e As EventArgs)

    Private _bundleDiameterRanges As New BundleDiameterRangeList
    Private _tutorials As New TutorialList
    Private _carModelDirectory As String = String.Empty
    Private _connectorFacesDirectory As String = String.Empty
    Private _fullName As String = String.Empty
    Private _inlinerPairCheckClassifications As New InlinerPairCheckClassificationList
    Private _recentFiles As New RecentFileList
    Private _wireColorCodeMapper As New WireColorCodeList
    Private _componentIdentifierGroups As Schematics.Identifier.Component.IdentifierGroupCollectionBase
    Private _inlinerIdentifiers As New InlinerIdentifierCollection
    Private _ultrasonicSpliceIdentifiers As New UltrasonicSpliceIdentifierCollection
    Private _spliceIdentifiers As New SpliceIdentifierCollection
    Private _eyeletIdentifiers As New EyeletIdentifierCollection
    Private _ecuConnectorIdentifier As EcuConnectorIdentifier

    Private Shared _loadError As Exception = Nothing

    Public Sub New()
        MyBase.New()
        _componentIdentifierGroups = CreateComponentIdentifierGroupsInstance()
    End Sub

    Protected MustOverride Function CreateComponentIdentifierGroupsInstance() As Schematics.Identifier.Component.IdentifierGroupCollectionBase

    Public Sub ResetToDefaults()
        With Me
            .Version = E3.Lib.DotNet.Expansions.Devices.My.Application.Info.Version.ToString
            .AutoZoomSelectionSchematics = False
            .AutoSync3DSelection = False
            .AutoSyncCavityChecksSelection = False
            .UseDynamicBundles = True
            .UseJTColors = False
            .ViewSplicesInCavityChecks = True
            .BundleDiameterRanges.CreateDefaultBundleDiameterRanges()
            .Tutorials.CreateDefaultTutorialList()
            .CarModelDirectory = FileIO.SpecialDirectories.MyDocuments
            .ClipboardModulePartNumberRegEx = "\t|\r|\v|\n|[.,;|]"
            .ConnectorFacesDirectory = FileIO.SpecialDirectories.MyDocuments
            .DefaultCableLengthType = String.Empty
            .DefaultWireLengthType = String.Empty
            .E3ApplicationHighlightHooksEnabled = False
            .HideNavigatorHubOnLoad = False
            .InlinerIdentifiers.CreateDefaultIdentifiers()
            .SpliceIdentifiers.CreateDefaultIdentifiers()
            .EyeletIdentifiers.CreateDefaultIdentifiers()
            .UltrasonicSpliceIdentifiers.CreateDefaultIdentifiers()
            .EcuConnectorIdentifier = EcuConnectorIdentifier.CreateDefaultIdentifier
            .InlinerPairCheckClassifications.CreateDefaultInlinerPairCheckClassifications()
            .LastChangedByEditable = True
            .LastOpenedDirectory = String.Empty
            .MoveDistanceToleranceForGraphicalCompare = 10
            .MoveDistanceToleranceMaxForGraphicalCompare = 1000
            .NavigatorObjectCountThreshold = Defaults.DEFAULT_NAVIGATOR_OBJECT_COUNT_THRESHOLD
            .RedliningStampIndicatorScaleFactor = 1.0
            .ResistivityAluminium = 2.6548
            .ResistivityCopper = 1.678
            .TemperatureCoefficientAluminium = 4.0
            .TemperatureCoefficientCopper = 3.93
            .ThresholdNofInstancesWithIdenticalOffsetValue = 0.75
            .TouchEnabledUI = False
            .UILanguage = If(Globalization.CultureInfo.CurrentCulture.Name = "de-DE", "de-DE", "en-US")
            .WireColorCodes.CreateDefaultColorCodeMapping()
            .ValidateKBLAfterLoad = False
#If WINDOWS Or NETFRAMEWORK Then
            .ToastMessageVerbosity = ToastMessageVerbosity.Warnings Or ToastMessageVerbosity.Errors
#End If
            .MagnifierSize = 300
            .MagnifierZoom = 12
            .UseSelectionCenterForRotation = False
            .AutomaticTooltips = False
            .Mark3DConnectorsOnWireModification = False
            .LengthClassDocument3D = E3.Lib.Model.LengthClass.Nominal
            .UseKblAbsoluteLocations = True

            .ProtectionWeightCalculationMinWeight = 1.0

            .ProtectionWeightCalculationMin_mm2 = 5 * 10 ^ -5
            .ProtectionWeightCalculationMax_mm2 = 10 ^ -3

            .ProtectionWeightCalculationMin_mm = CSng(1 / 50 * .ProtectionWeightCalculationMin_mm2)
            .ProtectionWeightCalculationMax_mm = CSng(1 / 50 * .ProtectionWeightCalculationMax_mm2)

            .ProtectionWeightCalculationTapeList = "tape;avbandagierung;spiraltape;overlaptape"
            .ProtectionWeightCalculationTubeList = "tube;schlauch"
            .ProtectionWeightCalculationTapePattern = "G[0-9]{1,2}|P[0-9]{1,2}"
            .ProtectionWeightCalculationTubePattern = "B[0-9]{0,2}|S[0-9]{1,2}|T[0-9]{1,2}|U[0-9]{0,2}|W[0-9]{0,2}|X[0-9]{1,2}"




        End With
    End Sub

    Public Shared Function LoadFromFile(Of T As GeneralSettingsBase)(fullName As String) As T
        Try
            Dim serializer As New XmlSerializer(GetType(T))

            Using fileStream As New FileStream(fullName, FileMode.Open, FileAccess.Read, FileShare.Read)
                Dim generalSettings As T = DirectCast(serializer.Deserialize(fileStream), T)
                generalSettings._fullName = fullName

                Return generalSettings
            End Using
        Catch ex As Exception
            _loadError = ex

            Return Nothing
        End Try
    End Function

    Public Sub OnModified()
        RaiseEvent Modified(Me, New EventArgs)
    End Sub

    Public Sub Save()
        If (String.IsNullOrEmpty(_fullName)) Then
            Throw New ArgumentNullException("Save of general settings configuration failed, path is not set!")
        End If

#If WINDOWS Or NETFRAMEWORK Then
        Save(_fullName)
#Else
        Save(_fullName, E3.Lib.DotNet.Expansions.Devices.My.Application.Info.CompanyName, E3.Lib.DotNet.Expansions.Devices.My.Application.Info.ProductName)
#End If
    End Sub

    Public Sub Save(fullName As String, companyName As String, productName As String)
        Dim serializer As New XmlSerializer(Me.GetType)
        Dim xs As New XmlSerializerNamespaces()

        Using writer As New StreamWriter(fullName)
            _fullName = fullName
            xs.Add(companyName, productName)
            serializer.Serialize(writer, Me, xs)
        End Using
    End Sub

#If WINDOWS Or NETFRAMEWORK Then
    Public Sub Save(fullName As String)
        Me.Save(fullName, HarnessAnalyzer.[Shared].COMPANY_FOLDER, HarnessAnalyzer.[Shared].PRODUCT_FOLDER)
    End Sub
#End If

    Public Property Version As String
    Public Property AutoZoomSelectionSchematics As Boolean
    Public Property AutoSync3DSelection As Boolean
    Public Property AutoSyncCavityChecksSelection As Boolean
    ''' <summary>
    ''' Bundles are variable in Document 3D if true. Static if false which means that they don't recalculate and overwritten from JT-Data (if JT
    ''' </summary>
    ''' <returns></returns>
    Public Property UseDynamicBundles As Boolean = True
    ''' <summary>
    ''' When true, use colors saved in JT file when overwriting/linking 3D-entity from each JT-object. When false the default defined colors will be used for all 3D entities
    ''' </summary>
    ''' <returns></returns>
    Public Property UseJTColors As Boolean = False
    ''' <summary>
    ''' Hide/Show connector splices (connector usage splices) in cavity checks view
    ''' </summary>
    Public Property ViewSplicesInCavityChecks As Boolean
    Public Property ClipboardModulePartNumberRegEx As String

    <ObfuscationAttribute(Feature:="renaming")>
    Public Property DefaultCableLengthType As String
    <ObfuscationAttribute(Feature:="renaming")>
    Public Property DefaultWireLengthType As String
    Public Property DisplaySVGValidationMessage As Boolean = True
    ''' <summary>
    '''  Selection to E3 and backwards (polling, every 3 s for backwards selection from E3) enabled
    ''' </summary>
    ''' <returns></returns>
    Public Property E3ApplicationHighlightHooksEnabled As Boolean
    Public Property HideNavigatorHubOnLoad As Boolean
    Public Property LastChangedByEditable As Boolean
    Public Property LastOpenedDirectory As String
    ''' <summary>
    ''' Factor registers tolreance of graphical compare move (more differnces and changes when lower)
    ''' </summary>
    ''' <returns></returns>
    Public Property MoveDistanceToleranceForGraphicalCompare As Integer
    ''' <summary>
    ''' Allowed maximum value for move tolerance
    ''' </summary>
    ''' <returns></returns>
    Public Property MoveDistanceToleranceMaxForGraphicalCompare As Integer
    ''' <summary>
    ''' Limit for navigator shows objects (performance)
    ''' </summary>
    ''' <returns></returns>
    Public Property NavigatorObjectCountThreshold As Integer
    ''' <summary>
    ''' Can only select by object type when enabled
    ''' </summary>
    ''' <returns></returns>
    Public Property ObjectTypeDependentSelection As Boolean
    Public Property RedliningStampIndicatorScaleFactor As Double = -1
    Public Property ResistivityAluminium As Single = -1
    Public Property ResistivityCopper As Single = -1
    ''' <summary>
    ''' Lines of cursor-cross over the whole screen enabled
    ''' </summary>
    ''' <returns></returns>
    Public Property ShowFullScreenAxisCursor As Boolean
    Public Property TemperatureCoefficientAluminium As Single = -1
    Public Property TemperatureCoefficientCopper As Single = -1

    Public Property ThresholdNofInstancesWithIdenticalOffsetValue As Single = -1
    Public Property TouchEnabledUI As Boolean
    Public Property UILanguage As String
    Public Property ValidateKBLAfterLoad As Boolean
    Public Property InverseCompare As Boolean = False 'HINT: Inverse logic of added and deleted-entries in compare (technical and graphical)
    Public Property LengthClassDocument3D As E3.Lib.Model.LengthClass = E3.Lib.Model.LengthClass.DMU

    Public Property MagnifierSize As Integer = 250
    Public Property MagnifierZoom As Integer = 3

    Public Property UseSelectionCenterForRotation As Boolean
    Public Property AutomaticTooltips As Boolean
    Public Property Mark3DConnectorsOnWireModification As Boolean
    Public Property UseKblAbsoluteLocations As Boolean

    Public Property ProtectionWeightCalculationMinWeight As Single

    Public Property ProtectionWeightCalculationMin_mm As Single
    Public Property ProtectionWeightCalculationMax_mm As Single

    Public Property ProtectionWeightCalculationMin_mm2 As Single
    Public Property ProtectionWeightCalculationMax_mm2 As Single

    Public Property ProtectionWeightCalculationTubePattern As String
    Public Property ProtectionWeightCalculationTapePattern As String

    Public Property ProtectionWeightCalculationTubeList As String
    Public Property ProtectionWeightCalculationTapeList As String

    <Category("BundleDiameterRanges")>
    Public ReadOnly Property BundleDiameterRanges() As BundleDiameterRangeList
        Get
            Return _bundleDiameterRanges
        End Get
    End Property

    <Category("Tutorials")>
    Public ReadOnly Property Tutorials() As TutorialList
        Get
            Return _tutorials
        End Get
    End Property

    Public Property CarModelDirectory As String
        Get
            Return _carModelDirectory
        End Get
        Set(value As String)
            If (value <> _carModelDirectory) Then _carModelDirectory = value
        End Set
    End Property

    Public Property ConnectorFacesDirectory As String
        Get
            Return _connectorFacesDirectory
        End Get
        Set(value As String)
            If (value <> _connectorFacesDirectory) Then _connectorFacesDirectory = value
        End Set
    End Property

    <Category("InlinerIdentifiers")>
    Public ReadOnly Property InlinerIdentifiers() As InlinerIdentifierCollection
        Get
            Return _inlinerIdentifiers
        End Get
    End Property

    <Category("SpliceIdentifiers")>
    Public ReadOnly Property SpliceIdentifiers() As SpliceIdentifierCollection
        Get
            Return _spliceIdentifiers
        End Get
    End Property

    <Category("EyeletIdentifiers")>
    Public ReadOnly Property EyeletIdentifiers() As EyeletIdentifierCollection
        Get
            Return _eyeletIdentifiers
        End Get
    End Property

    <Category("EcuConnectorIdentifier")>
    Public Property EcuConnectorIdentifier() As EcuConnectorIdentifier
        Get
            Return _ecuConnectorIdentifier
        End Get
        Set(value As EcuConnectorIdentifier)
            _ecuConnectorIdentifier = value
        End Set
    End Property

    <Category("InlinerPairCheckClassifications")>
    Public ReadOnly Property InlinerPairCheckClassifications() As InlinerPairCheckClassificationList
        Get
            Return _inlinerPairCheckClassifications
        End Get
    End Property

    <Category("RecentFiles")>
    Public ReadOnly Property RecentFiles() As RecentFileList
        Get
            Return _recentFiles
        End Get
    End Property

    <Category("WireColorCodes")>
    Public ReadOnly Property WireColorCodes() As WireColorCodeList
        Get
            Return _wireColorCodeMapper
        End Get
    End Property

    <Category("ComponentIdentifiers")>
    Public ReadOnly Property ComponentIdentifierGroups As Schematics.Identifier.Component.IdentifierGroupCollectionBase
        Get
            Return _componentIdentifierGroups
        End Get
    End Property

    <Category("UltrasonicSpliceIdentifiers")>
    Public ReadOnly Property UltrasonicSpliceIdentifiers() As UltrasonicSpliceIdentifierCollection
        Get
            Return _ultrasonicSpliceIdentifiers
        End Get
    End Property

    <XmlIgnore>
    Public Shared ReadOnly Property LoadError As Exception
        Get
            Return _loadError
        End Get
    End Property

#If NETFRAMEWORK Or WINDOWS Then
    Public Property ToastMessageVerbosity As ToastMessageVerbosity = ToastMessageVerbosity.Warnings Or ToastMessageVerbosity.Errors
#End If

End Class