Imports System.ComponentModel
Imports System.IO
Imports System.Xml.Serialization

<Serializable()>
<XmlInclude(GetType(GridColumn))> _
Public Class GridTable

    Public Event Modified(sender As Object, e As EventArgs)

    Private _gridColumns As New GridColumnList(Me)
    Private _fullName As String = String.Empty

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(tabletype As KblObjectType, tableDescription As String)
        Type = tabletype
        Description = tableDescription
        Version = My.Application.Info.Version.ToString(2)
    End Sub

    <Category("Type")>
    Public Property Type As KblObjectType

    <Category("Description")>
    Public Property Description As String           'TODO: is serialized as localized string: needs re-evaluting. Maybe this property can completely removed by resolving on runtime

    <Category("Version")>
    Public Property Version As String

    <Category("GridSubTable")>
    Public Property GridSubTable As GridTable

    Public Shared Function LoadFromFile(ByVal fullName As String) As GridTable
        Using fs As New FileStream(fullName, FileMode.Open, FileAccess.Read, FileShare.Read)
            If fs.Length > 0 Then
                Dim gt As GridTable = LoadFromStream(fs)
                If gt IsNot Nothing Then
                    gt._fullName = fullName
                End If
                Return gt
            End If
        End Using
        Return Nothing
    End Function

    Public Shared Function LoadFromStream(stream As Stream) As GridTable
        Try
            Dim serializer As New XmlSerializer(GetType(GridTable), GridAppearance.AllTypes)
            Dim table As GridTable = DirectCast(serializer.Deserialize(stream), GridTable)
            Return table
        Catch ex As Exception
#If DEBUG Then
            'HINT: watch was done incorrect here on deserialize?? In release this will be ignored
            Throw New Exception($"Only debug exception: check deserialize method in GridTable ({NameOf(LoadFromStream)}), error: {ex.Message}")
#End If
            Return Nothing
        End Try
    End Function

    Public Function HasOlderVersion(currentVersion As Version) As Boolean
        Return Version Is Nothing OrElse New Version(Version) < currentVersion
    End Function

    Public Sub NotifyModification()
        RaiseEvent Modified(Me, New EventArgs)
    End Sub

    Public Sub Save()
        If String.IsNullOrEmpty(_fullName) Then
            Throw New ArgumentNullException("Save of grid table appearance configuration failed, path is not set!")
        End If

        Save(_fullName)
    End Sub

    Public Sub Save(ByVal fullName As String)
        Using fs As New FileStream(fullName, FileMode.Create, FileAccess.Write, FileShare.None)
            SaveToStream(fs)
            _fullName = fullName
        End Using
    End Sub

    Public Sub SaveToStream(s As Stream)
        Dim serializer As New XmlSerializer(GetType(GridTable), GridAppearance.AllTypes)
        Dim xs As New XmlSerializerNamespaces()

        Dim writer As New StreamWriter(s)
        xs.Add(HarnessAnalyzer.[Shared].COMPANY_FOLDER, HarnessAnalyzer.[Shared].PRODUCT_FOLDER)
        serializer.Serialize(writer, Me, xs)
    End Sub

    <Category("GridColumns")> _
    Public ReadOnly Property GridColumns() As GridColumnList
        Get
            Return _gridColumns
        End Get
    End Property

End Class