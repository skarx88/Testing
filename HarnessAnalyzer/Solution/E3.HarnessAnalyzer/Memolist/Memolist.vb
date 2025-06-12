Imports System.ComponentModel
Imports System.IO
Imports System.Xml.Serialization

<Serializable()>
<XmlInclude(GetType(MemoItem))>
Public Class Memolist
    Public Event Modified()

    Private _fullName As String = String.Empty
    Private _memoItems As New MemoItemList
    Private Shared _loadError As Exception = Nothing

    Public Sub New()
        MyBase.New()
    End Sub

    Public Property CreatedBy As String
    Public Property CreatedOn As Date
    Public Property HarnessPartNumber As String
    Public Property HarnessVersion As String
    Public Property Name As String

    Public Shared Function Load(s As System.IO.Stream, Optional useFileName As String = Nothing) As Memolist
        If String.IsNullOrEmpty(useFileName) Then
            useFileName = TryCast(s, System.IO.FileStream).Name
        End If

        Try
            Dim serializer As New XmlSerializer(GetType(Memolist))
            Dim memolist As Memolist = DirectCast(serializer.Deserialize(s), Memolist)
            memolist._fullName = useFileName
            Return memolist
        Catch ex As Exception
            _loadError = ex

            Return Nothing
        End Try
    End Function

    Public Shared Function LoadFromFile(fullName As String) As Memolist
        Using fs As New FileStream(fullName, FileMode.Open, FileAccess.Read)
            Return Load(fs)
        End Using
    End Function

    Public Sub NotifyModification()
        RaiseEvent Modified()
    End Sub

    Public Sub Save()
        If String.IsNullOrEmpty(_fullName) Then
            Throw New ArgumentNullException("Save of memolist failed, path is not set!")
        End If

        Save(_fullName)
    End Sub

    Public Sub Save(ByVal fullName As String)
        Using fs As New FileStream(fullName, FileMode.Create)
            Save(fs)
        End Using
    End Sub

    Public Sub Save(s As System.IO.Stream)
        Dim serializer As New XmlSerializer(GetType(Memolist))
        Dim xs As New XmlSerializerNamespaces()

        Using writer As New StreamWriter(s, leaveOpen:=True)
            If TypeOf s Is FileStream Then
                _fullName = CType(s, System.IO.FileStream)?.Name
            End If
            xs.Add(HarnessAnalyzer.[Shared].COMPANY_FOLDER, HarnessAnalyzer.[Shared].PRODUCT_FOLDER)
            serializer.Serialize(writer, Me, xs)
        End Using
    End Sub

    <XmlIgnore>
    Public Shared ReadOnly Property LoadError As Exception
        Get
            Return _loadError
        End Get
    End Property

    <Category("MemoItems")>
    Public ReadOnly Property MemoItems() As MemoItemList
        Get
            Return _memoItems
        End Get
    End Property

End Class