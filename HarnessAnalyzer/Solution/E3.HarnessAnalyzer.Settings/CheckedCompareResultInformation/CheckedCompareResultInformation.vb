Imports System.IO
Imports System.Runtime.Serialization
Imports Zuken.E3.HarnessAnalyzer.Compatibility
Imports Zuken.E3.Lib.IO.Files.Hcv

<DataContract>
<KnownType(GetType(CheckedCompareResultList))>
Public Class CheckedCompareResultInformation

    <DataMember> Private _checkedCompareResults As New CheckedCompareResultList
    <IgnoreDataMember> Private _fullName As String = String.Empty
    <IgnoreDataMember> Private _ccType As KnownContainerFileFlags

    Private Sub New()
        MyBase.New()
    End Sub

    <DataMember> Public Property CreatedBy As String
    <DataMember> Public Property CreatedOn As Date
    <DataMember> Public Property HarnessPartNumber As String
    <DataMember> Public Property HarnessVersion As String
    <DataMember> Public Property Name As String

    Public ReadOnly Property CheckedCompareResults() As CheckedCompareResultList
        Get
            Return _checkedCompareResults
        End Get
    End Property

    Public Property Type As KnownContainerFileFlags
        Get
            Return _ccType
        End Get
        Set
            ThrowKnownContainerFileNotSupported(Value)
            _ccType = Value
        End Set
    End Property

    Public Shared Function LoadFromFile(fullName As String) As CheckedCompareResultInformation
        Using fs As New FileStream(fullName, FileMode.Open)
            Dim result As CheckedCompareResultInformation = Load(fs)
            result.Type = CType(KnownFile.GetFileType(fullName), KnownContainerFileFlags)
            Return result
        End Using
    End Function

    Private Shared Sub ThrowKnownContainerFileNotSupported(type As KnownContainerFileFlags)
        Select Case type
            Case KnownContainerFileFlags.Unspecified, KnownContainerFileFlags.GCRI, KnownContainerFileFlags.TCRI
                Return
            Case Else
                Throw New NotSupportedException($"File type ""{type.ToString}"" not supported for ""{GetType(CheckedCompareResultInformation).FullName}""")
        End Select
    End Sub

    Public Shared Function Load(s As System.IO.Stream, Optional useFileName As String = Nothing) As CheckedCompareResultInformation
        If String.IsNullOrEmpty(useFileName) Then
            useFileName = TryCast(s, System.IO.FileStream)?.Name
        End If

        Dim checkedCompareResults As CheckedCompareResultInformation = Nothing
        Try
            Dim [error] As Exception = Nothing
            If BinaryFile.IsBinFormatted(s) Then
                'HINT: we have the old format, load this over the compatibility-layer and convert it to V2024+
                Dim result As CheckedCompareResultInformation_2023 = CheckedCompareResultInformation_2023.Load(s)
                checkedCompareResults = CheckedCompareResultInformation.CreateFromV2023(result)
            ElseIf XmlFile.IsXml(s) Then
                checkedCompareResults = DataContractXmlFile.Load(Of CheckedCompareResultInformation)(s, [error])
                If checkedCompareResults.CreatedBy Is Nothing Then ' HINT: this file was serialized using the <Serializable>-Attribute -> we changed to DataContract so deserialization is not working anymore but we don't get an exception for this case, switch to alternative serialization here
                    If s.CanSeek Then
                        s.Position = 0
                    End If
                    Dim xml As XDocument = XDocument.Load(s)
                    checkedCompareResults = xml.AsObject(Of CheckedCompareResultInformation)(SerializerType.LimitedReflection) ' HINT: use limited reflection to deserialize more loose (not so type strict) into object
                End If
            End If

            If [error] Is Nothing Then
                checkedCompareResults.Name = IO.Path.GetFileName(useFileName)
                checkedCompareResults._fullName = useFileName
            Else
                Throw New Exception([error].Message, [error])
            End If
        Catch e As Runtime.Serialization.SerializationException
            Throw New Runtime.Serialization.SerializationException(String.Format("Failed to load checked compare result information.{0} Reason: {1}", vbCrLf, e.Message))
        End Try

        checkedCompareResults.Type = CType(KnownContainerFile.GetFileType(useFileName), KnownContainerFileFlags)

        Return checkedCompareResults
    End Function

    Public Sub Save(s As System.IO.Stream)
        Try
            Dim [error] As Exception = Nothing
            If Not DataContractXmlFile.Save(Me, s, [error]) AndAlso [error] IsNot Nothing Then 'HINT: new format is now always safed in XML-Style because BinaryFormatter will be obsolete in the future
                Throw New Exception([error].Message, [error])
            End If
        Catch e As Runtime.Serialization.SerializationException
            Throw New Runtime.Serialization.SerializationException(String.Format("Failed to save checked compare result information.{0}Reason: {1}", vbCrLf, e.Message))
        End Try
    End Sub

    Public Sub Save()
        If (String.IsNullOrEmpty(_fullName)) Then
            Throw New ArgumentNullException("Save of checked compare result information failed, path is not set!")
        End If

        Save(_fullName)
    End Sub

    Public Sub Save(ByVal fullName As String)
        Using fileStream As New FileStream(fullName, FileMode.Create)
            Me.Save(fileStream)
        End Using
    End Sub

    Public Function AsStream() As System.IO.Stream
        Dim ms As New MemoryStream
        Me.Save(ms)
        ms.Position = 0
        Return ms
    End Function


    Public Shared Function CreateNew(type As KnownContainerFileFlags) As CheckedCompareResultInformation
        Dim info As New CheckedCompareResultInformation()
#If WINDOWS Or NETFRAMEWORK Then
        info.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent.Name
#Else
        info.CreatedBy = System.Runtime.InteropServices.RuntimeInformation.OSDescription
#End If
        info.CreatedOn = Now
        info.Type = type
        Return info
    End Function

    Private Shared Function CreateFromV2023(ByVal result As CheckedCompareResultInformation_2023) As CheckedCompareResultInformation
        Dim result2024 As New CheckedCompareResultInformation()
        With result2024
            .CreatedBy = result.CreatedBy
            .CreatedOn = result.CreatedOn
            .HarnessPartNumber = result.HarnessPartNumber
            .HarnessVersion = result.HarnessVersion
            .Name = result.Name
        End With

        For Each chkres As CheckedCompareResult_2023 In result.CheckedCompareResults.Cast(Of CheckedCompareResult_2023)
            Dim chkRes2024 As New CheckedCompareResult
            chkRes2024.ReferenceDrawingName = chkres.ReferenceDrawingName
            chkRes2024.CompareDrawingName = chkres.CompareDrawingName
            For Each entry_2023 As CheckedCompareResultEntry_2023 In chkres.CheckedCompareResultEntries.OfType(Of CheckedCompareResultEntry_2023)
                Dim entry_2024 As New CheckedCompareResultEntry()
                With entry_2024
                    .Comment = entry_2023.Comment
                    .CompareSignature = entry_2023.CompareSignature
                    .ReferenceSignature = entry_2023.ReferenceSignature
                    .ToBeChanged = entry_2023.ToBeChanged
                End With
                chkRes2024.CheckedCompareResultEntries.Add(entry_2024)
            Next
            result2024.CheckedCompareResults.Add(chkRes2024)
        Next
        Return result2024
    End Function


End Class