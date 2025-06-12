Imports System.ComponentModel
Imports System.IO
Imports System.Runtime.Serialization

Namespace QualityStamping

    <DataContract(Namespace:="Zuken.E3.HarnessAnalyzer.QualityStamping")>
    <KnownType(GetType(QMStamp))>
    Public Class QMStamps

        <DataMember> Public Property Version As String
        <DataMember> Public Property CreatedBy As String
        <DataMember> Public Property CreatedOn As Date
        <DataMember> Public Property HarnessPartNumber As String
        <DataMember> Public Property HarnessVersion As String

        <IgnoreDataMember> Private _fullName As String = String.Empty
        <DataMember> Private _qmStamps As New QMStampList

        Public Property Name As String

        Private Shared _loadError As Exception = Nothing

        Public Sub New(fullName As String)
            Me.New()
            File.Create(fullName).Close()
            _fullName = fullName
        End Sub

        Public Sub New()
            MyBase.New()

            HarnessPartNumber = String.Empty
            _qmStamps.QMStamps = Me
        End Sub

        Public Shared Function Load(s As System.IO.Stream, Optional useFileName As String = Nothing) As QMStamps
            If String.IsNullOrEmpty(useFileName) Then
                useFileName = TryCast(s, System.IO.FileStream)?.Name
            End If

            Try
                Dim qmStamps As QMStamps = LoadCore(s)
                qmStamps._fullName = useFileName
                Return qmStamps
            Catch ex1 As System.Runtime.Serialization.SerializationException
                If ex1.Message.Contains("'StampFile'") Then ' Hint: this is only a minimal - compatibility fallback to make reading still possible -> BUT: it's not guaranteed that all data can be deserialized -> partial data is better than nothing
                    Try
                        s.Seek(0, SeekOrigin.Begin)
                        Dim namespace_attr As DataContractAttribute = CType(GetType(QMStamps).GetCustomAttributes(GetType(DataContractAttribute), False).SingleOrDefault, DataContractAttribute)
                        Dim qmStamps As QMStamps = LoadCore(s, "StampFile", namespace_attr.Namespace)
                        qmStamps._fullName = useFileName
                        Return qmStamps
                    Catch ex_1 As Exception
#If DEBUG Or CONFIG = "Debug" Then
                        Throw
#End If
                        _loadError = ex_1
                    End Try
                Else
                    Throw
                End If
            Catch ex As Exception
#If DEBUG Or CONFIG = "Debug" Then
                Throw
#End If
                _loadError = ex
            End Try
            Return Nothing
        End Function

        Private Shared Function LoadCore(s As System.IO.Stream, Optional rootName As String = Nothing, Optional rootNamespace As String = Nothing) As QMStamps
            Dim qmStamps As QMStamps
            qmStamps = HarnessAnalyzer.Shared.Utilities.ReadXml(Of QMStamps)(s, rootName, rootNamespace)
            Return qmStamps
        End Function

        Public Shared Function LoadOrEmpty(s As System.IO.Stream, Optional useFileName As String = Nothing) As QMStamps
            Dim qmStamps As QMStamps = Load(s, useFileName)
            If qmStamps IsNot Nothing Then
                Return qmStamps
            End If
            Return New QMStamps() With {._fullName = useFileName}
        End Function

        Public Shared Function LoadFromFile(fullName As String) As QMStamps
            Using fs As New FileStream(fullName, FileMode.Open, FileAccess.Read)
                Return Load(fs)
            End Using
        End Function

        Public Sub Save()
            If String.IsNullOrEmpty(_fullName) Then
                Throw New ArgumentNullException("Save of QM stamp information failed, path is not set!")
            End If

            Save(_fullName)
        End Sub

        Public Sub Save(ByVal fullName As String)
            Using fs As New FileStream(fullName, FileMode.Create, FileAccess.Write, FileShare.None)
                Me.Save(fs)
            End Using
        End Sub

        Public Sub Save(ByVal s As System.IO.Stream)
            HarnessAnalyzer.Shared.Utilities.WriteXml(Me, s, , Me.GetType.Namespace)
        End Sub

        <Category("Stamps")> _
        Public ReadOnly Property Stamps() As QMStampList
            Get
                If (_qmStamps Is Nothing) Then
                    _qmStamps = New QMStampList
                    _qmStamps.QMStamps = Me
                End If

                Return _qmStamps
            End Get
        End Property

        <OnDeserialized>
        Private Sub OnDeserialized(ctx As StreamingContext)
            _qmStamps.QMStamps = Me
        End Sub

        <IgnoreDataMember>
        Public Shared ReadOnly Property LoadError As Exception
            Get
                Return _loadError
            End Get
        End Property

    End Class

End Namespace