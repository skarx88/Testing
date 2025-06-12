Imports System.IO
Imports System.Runtime.Serialization

Namespace IssueReporting

    <DataContract(Namespace:="Zuken.E3.HarnessAnalyzer.IssueReporting")>
    Public Class ReportFile

        Private _fileName As String
        Private _createdBy As String = Environment.UserName
        Private _version As String = "1.0"
        Private _dateOfCreation As Nullable(Of DateTime) = Now
        Private _hasChanges As Boolean = False
        Private _authoringSystem As String = HarnessAnalyzer.[Shared].PRODUCT_FOLDER
        Private _beginOfObservation As Nullable(Of DateTime)
        Private _endOfObservation As Nullable(Of DateTime)
        Private _harnessPartNumber As String = String.Empty

        Public Sub New()
        End Sub

        <DataMember(Order:=0)> Property Version As String
            Get
                Return _version
            End Get
            Set(value As String)
                If _version <> value Then
                    _version = value
                    SetAuthoringSystem()
                    _hasChanges = True
                End If
            End Set
        End Property

        <DataMember(Order:=1)> Property CreatedBy As String
            Get
                Return _createdBy
            End Get
            Set(value As String)
                If _createdBy <> value Then
                    _createdBy = value
                    SetAuthoringSystem()
                    _hasChanges = True
                End If
            End Set
        End Property

        Private Sub SetAuthoringSystem()
            Me.AuthoringSystem = HarnessAnalyzer.[Shared].PRODUCT_FOLDER
        End Sub

        <DataMember(Order:=2)> Property DateOfCreation As Nullable(Of DateTime)
            Get
                Return _dateOfCreation
            End Get
            Set(value As Nullable(Of DateTime))
                If Not _dateOfCreation.HasValue OrElse _dateOfCreation <> value Then
                    _dateOfCreation = value
                    _hasChanges = True
                End If
            End Set
        End Property

        <DataMember(Order:=3)> Property AuthoringSystem As String
            Get
                Return _authoringSystem
            End Get
            Set(value As String)
                If _authoringSystem <> value Then
                    _authoringSystem = value
                    _hasChanges = True
                End If
            End Set
        End Property

        <DataMember(Order:=4)> Property BeginOfObservation As Nullable(Of DateTime)
            Get
                Return _beginOfObservation
            End Get
            Set(value As Nullable(Of DateTime))
                If Not _beginOfObservation.HasValue OrElse value <> _beginOfObservation Then
                    _beginOfObservation = value
                    _hasChanges = True
                End If
            End Set
        End Property

        <DataMember(Order:=5)> Property EndOfObservation As Nullable(Of DateTime)
            Get
                Return _endOfObservation
            End Get
            Set(value As Nullable(Of DateTime))
                If Not _endOfObservation.HasValue OrElse _endOfObservation <> value Then
                    _endOfObservation = value
                    _hasChanges = True
                End If
            End Set
        End Property

        <DataMember(Order:=6)> Property HarnessPartNumber As String
            Get
                Return _harnessPartNumber
            End Get
            Set(value As String)
                If _harnessPartNumber <> value Then
                    _harnessPartNumber = value
                    _hasChanges = True
                End If
            End Set
        End Property

        <DataMember(Order:=7)> Property Issues As New IssueReporting.IssueCollection

        ReadOnly Property FileName As String
            Get
                Return _fileName
            End Get
        End Property

        Public Shared Function LoadFrom(s As Stream) As ReportFile
            Dim rf As ReportFile = HarnessAnalyzer.Shared.Utilities.ReadXml(Of ReportFile)(s)
            rf._hasChanges = False
            Return rf
        End Function

        Public Shared Function LoadFrom(fullFilePath As String) As ReportFile
            Dim rf As ReportFile = HarnessAnalyzer.Shared.Utilities.ReadXml(Of ReportFile)(fullFilePath)
            rf._fileName = fullFilePath
            rf._hasChanges = False
            Return rf
        End Function

        Public Sub SaveAs(fullFilePath As String)
            Using fs As New FileStream(fullFilePath, FileMode.Create, FileAccess.Write, FileShare.None)
                Me.Save(fs)
                _fileName = fullFilePath
                fs.Flush()
            End Using
        End Sub

        Public Sub Save(s As Stream)
            HarnessAnalyzer.Shared.Utilities.WriteXml(Me, s)
            Me._hasChanges = False
        End Sub

        Public Sub Save()
            If Not String.IsNullOrEmpty(_fileName) Then
                Me.SaveAs(Me.FileName)
            Else
                Throw New Exception(String.Format("Save not possible because {0} was not loaded from a file", Me.GetType.Name))
            End If
        End Sub

        Public Function HasChanges() As Boolean
            Return _hasChanges OrElse Issues.Any(Function(iss) iss.HasChanges)
        End Function

        Public Sub AcceptChanges()
            _hasChanges = False
            For Each iss As Issue In Me.Issues
                iss.acceptChanges()
            Next
        End Sub

    End Class

End Namespace
