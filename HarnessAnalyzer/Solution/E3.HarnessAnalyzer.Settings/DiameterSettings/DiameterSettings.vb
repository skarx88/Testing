Imports System.ComponentModel
Imports System.IO
Imports System.Xml.Serialization

<Serializable()>
<XmlInclude(GetType(Diameter))>
<XmlInclude(GetType(GenericDiameterFormulaParameters))>
Public Class DiameterSettings
    Public Event Modified()

    Public Property Name As String

    <Category("Version")>
    Public Property Version As String

    Public Property IsAddOnToleranceOnArea As Boolean
    Public Property RawBundleInstallationAddOnTolerance As Double
    Public Property RawBundleProvisioningAddOnTolerance As Double

    Private _diameters As New DiameterList
    Private _fullName As String = String.Empty
    Private _genericDiameterFormulaParameters As New GenericDiameterFormulaParameters
    Private Shared _loadError As Exception = Nothing

    Public Sub New()
        MyBase.New
    End Sub

    Public Sub ResetToDefaults()
        Me.Diameters.CreateDefaultDiameters()

        Me.Version = E3.Lib.DotNet.Expansions.Devices.My.Application.Info.Version.ToString
        Me.Name = "Diameter Settings"
        Me.IsAddOnToleranceOnArea = False
        Me.RawBundleInstallationAddOnTolerance = 0.1
        Me.RawBundleProvisioningAddOnTolerance = 0.1
    End Sub

    Public Shared Function LoadFromFile(fullName As String) As DiameterSettings
        Try
            Dim serializer As New XmlSerializer(GetType(DiameterSettings))
            Using fileStream As New FileStream(fullName, FileMode.Open, FileAccess.Read, FileShare.Read)
                Dim diameterSettings As DiameterSettings = DirectCast(serializer.Deserialize(fileStream), DiameterSettings)
                diameterSettings._fullName = fullName

                Return diameterSettings
            End Using
        Catch ex As Exception
            _loadError = ex

            Return Nothing
        End Try
    End Function

    Public Sub NotifyModification()
        RaiseEvent Modified()
    End Sub

    Public Sub Save()
        If (String.IsNullOrEmpty(_fullName)) Then
            Throw New ArgumentNullException("Save of diameter settings configuration failed, path is not set!")
        End If

        Save(_fullName)
    End Sub

    Public Sub Save(ByVal fullName As String)
        Dim serializer As New XmlSerializer(GetType(DiameterSettings))
        Dim xs As New XmlSerializerNamespaces()

        Using writer As New StreamWriter(fullName)
            _fullName = fullName

            xs.Add(E3.HarnessAnalyzer.Shared.COMPANY_FOLDER, E3.HarnessAnalyzer.Shared.PRODUCT_FOLDER)

            serializer.Serialize(writer, Me, xs)
        End Using
    End Sub


    <Category("Diameters")> _
    Public ReadOnly Property Diameters() As DiameterList
        Get
            Return _diameters
        End Get
    End Property

    <Category("GenericDiameterFormulaParameters")> _
    Public Property GenericDiameterFormulaParameters() As GenericDiameterFormulaParameters
        Get
            Return _genericDiameterFormulaParameters
        End Get
        Set(value As GenericDiameterFormulaParameters)
            _genericDiameterFormulaParameters = value
        End Set
    End Property

    <XmlIgnore>
    Public Shared ReadOnly Property LoadError As Exception
        Get
            Return _loadError
        End Get
    End Property

End Class