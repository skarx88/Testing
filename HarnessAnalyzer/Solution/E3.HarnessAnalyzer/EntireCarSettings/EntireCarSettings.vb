Imports System.ComponentModel
Imports System.IO
Imports System.Xml.Serialization
Imports Zuken.E3.Lib.IO.Files.Hcv

<Serializable()>
<XmlInclude(GetType(EntireCarHarness))>
Public Class EntireCarSettings
    Public Event Modified()

    Public Property CreatedBy As String
    Public Property CreatedOn As Date
    Public Property Name As String

    Private _harnesses As New EntireCarHarnessList
    Private _fullName As String = String.Empty

    Private Shared _loadError As Exception = Nothing

    Public Sub New()
        MyBase.New()
    End Sub

    Public Shared Function LoadFromFile(fullName As String) As EntireCarSettings
        Using fs As New FileStream(fullName, FileMode.Open, FileAccess.Read, FileShare.Read)
            Return Load(fs, fullName)
        End Using
    End Function

    Public Shared Function LoadFromContainerFile(container As IDataContainerFile) As EntireCarSettings
        If container IsNot Nothing Then
            Using s As Stream = container.GetDataStream
                If s IsNot Nothing Then
                    Return EntireCarSettings.Load(s)
                End If
            End Using
        End If
        Return Nothing
    End Function

    Public Shared Function Load(s As System.IO.Stream, Optional useFullName As String = Nothing) As EntireCarSettings
        If String.IsNullOrEmpty(useFullName) Then
            useFullName = TryCast(s, System.IO.FileStream)?.Name
        End If

        Try
            Dim serializer As New XmlSerializer(GetType(EntireCarSettings))
            Dim activeCarModules As EntireCarSettings = DirectCast(serializer.Deserialize(s), EntireCarSettings)
            activeCarModules._fullName = useFullName

            Return activeCarModules
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
            Throw New ArgumentNullException("Save of entire car settings failed, path is not set!")
        End If

        Save(_fullName)
    End Sub

    Public Sub Save(ByVal fullName As String)
        Dim serializer As New XmlSerializer(GetType(EntireCarSettings))
        Dim xs As New XmlSerializerNamespaces()

        Using writer As New StreamWriter(fullName)
            _fullName = fullName

            xs.Add(HarnessAnalyzer.[Shared].COMPANY_FOLDER, HarnessAnalyzer.[Shared].PRODUCT_FOLDER)

            serializer.Serialize(writer, Me, xs)
        End Using
    End Sub


    <Category("Harnesses")> _
    Public ReadOnly Property Harnesses() As EntireCarHarnessList
        Get
            Return _harnesses
        End Get
    End Property

    <XmlIgnore>
    Public Shared ReadOnly Property LoadError As Exception
        Get
            Return _loadError
        End Get
    End Property

End Class