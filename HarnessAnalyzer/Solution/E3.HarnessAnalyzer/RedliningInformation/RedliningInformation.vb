Imports System.ComponentModel
Imports System.IO
Imports System.Runtime.Serialization
Imports System.Xml.Serialization

<Serializable()>
<XmlInclude(GetType(Redlining))>
<XmlInclude(GetType(RedliningGroup))>
Public Class RedliningInformation

    Public Event Modified(sender As Object, e As EventArgs)

    Private _fullName As String = String.Empty
    Private _redlinings As New RedliningList
    Private _redliningGroups As New RedliningGroupList

    Private Shared _loadError As Exception = Nothing

    Public Sub New()
        MyBase.New()
    End Sub

    Public Shared Function LoadFromFile(fullName As String) As RedliningInformation
        Using fs As New FileStream(fullName, FileMode.Open, FileAccess.Read)
            Return Load(fs)
        End Using
    End Function

    Public Shared Function Load(s As System.IO.Stream, Optional useFileName As String = Nothing) As RedliningInformation
        If String.IsNullOrEmpty(useFileName) Then
            useFileName = TryCast(s, System.IO.FileStream).Name
        End If

        Try
            Dim redliningInformation As RedliningInformation = XmlFile.LoadFrom(Of RedliningInformation)(s)
            redliningInformation._fullName = useFileName
            Return redliningInformation
        Catch ex As Exception
#If DEBUG Or CONFIG = "Debug" Then
            Throw
#End If
            _loadError = ex

            Return Nothing
        End Try
    End Function

    Public Sub OnModified(e As EventArgs)
        RaiseEvent Modified(Me, e)
    End Sub

    Public Sub Save()
        If (String.IsNullOrEmpty(_fullName)) Then
            Throw New ArgumentNullException("Save of redlining information failed, path is not set!")
        End If

        Save(_fullName)
    End Sub

    Public Sub Save(fullName As String)
        Using fs As New FileStream(fullName, FileMode.Create)
            SaveRedliningInfo(fs, Me)
        End Using
    End Sub

    Public Sub Save(s As System.IO.Stream)
        SaveRedliningInfo(s, Me)
    End Sub

    Public Shared Sub SaveRedliningInfo(s As System.IO.Stream, info As RedliningInformation)
        Dim xs As New XmlSerializerNamespaces()

        If TypeOf s Is FileStream Then
            info._fullName = CType(s, FileStream).Name
        End If

        xs.Add(HarnessAnalyzer.[Shared].COMPANY_FOLDER, HarnessAnalyzer.[Shared].PRODUCT_FOLDER)
        s.SerializeXml(info, xs)
    End Sub

    Public Property CreatedBy As String
    Public Property CreatedOn As Date
    Public Property HarnessPartNumber As String = String.Empty
    Public Property HarnessVersion As String
    Public Property Name As String

    <XmlIgnore>
    Public Shared ReadOnly Property LoadError As Exception
        Get
            Return _loadError
        End Get
    End Property

    <Category("Redlinings")> _
    Public ReadOnly Property Redlinings() As RedliningList
        Get
            Return _redlinings
        End Get
    End Property

    <Category("RedliningGroups")>
    Public Property RedliningGroups() As RedliningGroupList
        Get
            Return _redliningGroups
        End Get
        Set(value As RedliningGroupList)
            _redliningGroups = value
        End Set
    End Property

End Class