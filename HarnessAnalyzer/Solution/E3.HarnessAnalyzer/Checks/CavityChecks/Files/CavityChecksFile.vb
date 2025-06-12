Imports System.IO
Imports System.Runtime.Serialization
Imports System.Xml
Imports System.Xml.Serialization

Namespace Checks.Cavities.Files

    <XmlRoot(DataType:="CavityAssignmentList", ElementName:="CavityAssignmentList", IsNullable:=False, [Namespace]:=CavityChecksFile.Namespace)> ' for xsd schema generation
    <XmlType(TypeName:="CavityAssignmentList", [Namespace]:=CavityChecksFile.Namespace)> ' for xsd schema generation
    <KnownType(GetType(ModuleEntryCollection))>
    <KnownType(GetType(ConnectorEntryCollection))>
    <DataContract(Name:="CavityAssignmentList")>
    Public Class CavityChecksFile
        Private _docView As Views.Document.DocumentView

        Friend Const [Namespace] As String = "http://www.zuken.com/E3.HarnessAnalyzer/CavityAssignment"

        Public Sub New()
        End Sub

        Public Sub New(docView As Views.Document.DocumentView)
            _docView = docView
        End Sub

        <DataMember(Order:=0)>
        <XmlElement(IsNullable:=True, Order:=0)>
        Property Header As HeaderDataEntry = HeaderDataEntry.New10Version

        <DataMember(Order:=1)>
        <XmlElement(IsNullable:=False, Order:=1)>
        ReadOnly Property Connectors As New ConnectorEntryCollection(Me)

        <DataMember(Order:=2)>
        <XmlElement(IsNullable:=False, Order:=2)>
        ReadOnly Property Modules As New ModuleEntryCollection(Me)

        <IgnoreDataMember>
        <XmlIgnore> ' for schema generation
        Property Document As Views.Document.DocumentView
            Get
                Return _docView
            End Get
            Set(value As Views.Document.DocumentView)
                If _docView IsNot value Then
                    If _docView IsNot Nothing Then
                        Clear()
                    End If
                    _docView = value
                End If
            End Set
        End Property

        Public Sub UpdateAll()
            Me.Modules.UpdateAll()
            Me.Connectors.UpdateAll()

            If Me.Header Is Nothing Then
                _Header = HeaderDataEntry.New10Version
            End If

            Me.Header.HarnessPartNumber = _docView.GetHarnessPartNumber
            Me.Header.HarnessProject = _docView.GetHarnessProjectNumber
            Me.Header.HarnessVersion = _docView.GetHarnessVersion
        End Sub

        Public Sub Clear()
            Modules.Clear()
            Connectors.Clear()
        End Sub

        Public Sub SaveTo(filePath As String)
            Using fs As New FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None)
                Save(fs)
            End Using
        End Sub

        Public Sub Save(stream As IO.Stream)
            Dim xmlSettings As New XmlWriterSettings()
            With xmlSettings
                .Indent = True
            End With
            Using w As XmlWriter = XmlWriter.Create(stream, xmlSettings)
                Dim serializer As New XmlSerializer(Me.GetType)
                serializer.Serialize(w, Me)
            End Using
        End Sub

        Public Shared Function ReadFrom(filePath As String) As CavityChecksFile
            Using fs As New FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)
                Return Read(fs)
            End Using
        End Function

        Public Shared Function Read(stream As IO.Stream) As CavityChecksFile
            Dim s As New DataContractSerializer(GetType(CavityChecksFile))
            Return CType(s.ReadObject(stream), CavityChecksFile)
        End Function

        <OnDeserialized>
        Private Sub OnDeserialized(ctx As StreamingContext)
            Me.Modules.File = Me
            Me.Connectors.File = Me
        End Sub

        Public Shared ReadOnly Property FileFilter As String
            Get
                Return $"{My.Resources.CavityChecksStrings.FileFilterDescription} (*.{FileExtension})|*.{FileExtension}"
            End Get
        End Property

        Public Shared ReadOnly Property FileExtension As String
            Get
                Return "calst"
            End Get
        End Property

    End Class

End Namespace