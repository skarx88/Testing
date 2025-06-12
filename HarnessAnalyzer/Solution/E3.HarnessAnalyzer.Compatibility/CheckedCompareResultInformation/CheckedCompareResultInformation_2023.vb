Imports System.IO
Imports System.Runtime.Serialization

'HINT: these are are skeleton (no further BL) classes to only hook into serialization process of the old version, making it possible retrieve the data (f.e. CollectionBase is no longer deserializable)
'      a custom binder is used to map the type-names on deserialization (because the names of the types are without the 2023-suffix) to this classes here
<Serializable()>
Public Class CheckedCompareResultInformation_2023

    Private _checkedCompareResults As New CheckedCompareResultList_2023
    Private _fullName As String = String.Empty

    Private Sub New()
        MyBase.New()
    End Sub

    Public Property CreatedBy As String
    Public Property CreatedOn As Date
    Public Property HarnessPartNumber As String
    Public Property HarnessVersion As String
    Public Property Name As String

    Public Shared Function LoadFromFile(fullName As String) As CheckedCompareResultInformation_2023
        Dim fileStream As New FileStream(fullName, FileMode.Open, FileAccess.Read)
        Try
            Return Load(fileStream)
        Finally
            fileStream.Close()
        End Try
    End Function

    Public Shared Function Load(s As System.IO.Stream) As CheckedCompareResultInformation_2023
        Dim checkedCompareResults As CheckedCompareResultInformation_2023 = Nothing
        Try
            Dim binaryFormatter As New Runtime.Serialization.Formatters.Binary.BinaryFormatter
            If s.CanSeek Then
                s.Position = 0
            End If
            binaryFormatter.Binder = New TypeBinderTo2023
            binaryFormatter.AssemblyFormat = Formatters.FormatterAssemblyStyle.Simple
            checkedCompareResults = DirectCast(binaryFormatter.Deserialize(s), CheckedCompareResultInformation_2023)
            checkedCompareResults._fullName = TryCast(s, System.IO.FileStream)?.Name
        Catch e As Runtime.Serialization.SerializationException
            Throw New Runtime.Serialization.SerializationException(String.Format("Failed to load checked compare result information.{0}Reason: {1}", vbCrLf, e.Message))
        End Try

        Return checkedCompareResults
    End Function

    Private Class TypeBinderTo2023
        Inherits SerializationBinder

        Private Const V2023String As String = "2023"

        Public Overrides Function BindToType(assemblyName As String, typeName As String) As Type
            Dim name As String = typeName.Split(Type.Delimiter).Last
            If NameOf(CheckedCompareResultInformation_2023).TrimEnd(V2023String.ToCharArray).Trim("_"c) = name Then
                Return GetType(CheckedCompareResultInformation_2023)
            ElseIf NameOf(CheckedCompareResultList_2023).TrimEnd(V2023String.ToCharArray).Trim("_"c) = name Then
                Return GetType(CheckedCompareResultList_2023)
            ElseIf NameOf(CheckedCompareResult_2023).TrimEnd(V2023String.ToCharArray).Trim("_"c) = name Then
                Return GetType(CheckedCompareResult_2023)
            ElseIf NameOf(CheckedCompareResultEntryList_2023).TrimEnd(V2023String.ToCharArray).Trim("_"c) = name Then
                Return GetType(CheckedCompareResultEntryList_2023)
            ElseIf NameOf(CheckedCompareResultEntry_2023).TrimEnd(V2023String.ToCharArray).Trim("_"c) = name Then
                Return GetType(CheckedCompareResultEntry_2023)
            Else
                Return Type.GetType(typeName)
            End If
        End Function
    End Class

    Public ReadOnly Property CheckedCompareResults() As CheckedCompareResultList_2023
        Get
            Return _checkedCompareResults
        End Get
    End Property

End Class
