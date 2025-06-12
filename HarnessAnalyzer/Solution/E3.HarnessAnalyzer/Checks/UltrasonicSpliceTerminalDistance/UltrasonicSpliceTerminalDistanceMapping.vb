Imports System.ComponentModel
Imports System.IO
Imports System.Xml.Serialization

<Serializable()>
<XmlInclude(GetType(UltrasonicSpliceTerminalDistanceMap))>
<XmlInclude(GetType(Settings.GenericDiameterFormulaParameters))>
Public Class UltrasonicSpliceTerminalDistanceMapping
    Public Event Modified()

    Public Property Name As String
    Public Property Version As String

    Private _fullName As String = String.Empty
    Private _ultrasonicSpliceTerminalDistanceMaps As New UltrasonicSpliceTerminalDistanceMapList

    Private Shared _loadError As Exception = Nothing

    Public Sub New()
        MyBase.New()
    End Sub

    Public Shared Function LoadFromFile(fullName As String) As UltrasonicSpliceTerminalDistanceMapping
        Try
            Dim serializer As New XmlSerializer(GetType(UltrasonicSpliceTerminalDistanceMapping))

            Using fileStream As New FileStream(fullName, FileMode.Open, FileAccess.Read, FileShare.Read)
                Dim ultrasonicSpliceTerminalDistanceMapping As UltrasonicSpliceTerminalDistanceMapping = DirectCast(serializer.Deserialize(fileStream), UltrasonicSpliceTerminalDistanceMapping)
                ultrasonicSpliceTerminalDistanceMapping._fullName = fullName

                Return ultrasonicSpliceTerminalDistanceMapping
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
            Throw New ArgumentNullException("Save of ultrasonic splice - terminal distance mapping failed, path is not set!")
        End If

        Save(_fullName)
    End Sub

    Public Sub Save(ByVal fullName As String)
        Dim serializer As New XmlSerializer(GetType(UltrasonicSpliceTerminalDistanceMapping))
        Dim xs As New XmlSerializerNamespaces()

        Using writer As New StreamWriter(fullName)
            _fullName = fullName

            xs.Add(HarnessAnalyzer.Shared.COMPANY_FOLDER, HarnessAnalyzer.Shared.PRODUCT_FOLDER)

            serializer.Serialize(writer, Me, xs)
        End Using
    End Sub


    <XmlIgnore>
    Public Shared ReadOnly Property LoadError As Exception
        Get
            Return _loadError
        End Get
    End Property

    <Category("UltrasonicSpliceTerminalDistanceMaps")>
    Public ReadOnly Property UltrasonicSpliceTerminalDistanceMaps() As UltrasonicSpliceTerminalDistanceMapList
        Get
            Return _ultrasonicSpliceTerminalDistanceMaps
        End Get
    End Property

End Class