Imports System.ComponentModel
Imports System.Runtime.Serialization

Namespace IssueReporting

    <DataContract(Namespace:="Zuken.E3.HarnessAnalyzer.IssueReporting")>
    Public Class Issue

        <DataMember> Private _id As String = String.Empty
        <DataMember> Private _noOfOccurrences As UInteger = 0
        Private _dateOfConfirmation As Nullable(Of Date)
        Private _issueTag As String
        Private _confirmedBy As String
        Private _objectReference As String
        Private _confirmationComment As String = String.Empty
        Private _description As String = String.Empty
        Private _hasChanges As Boolean = False
        Private _vdIssues As List(Of VdIssue)

        Public Sub New(id As String, nOfOccurences As UInteger)
            _id = id
            _noOfOccurrences = nOfOccurences
        End Sub

        ReadOnly Property Id As String
            Get
                Return _id
            End Get
        End Property

        ' <Browsable(False)>
        <DataMember> Property ObjectReference As String
            Get
                Return _objectReference
            End Get
            Friend Set(value As String)
                If _objectReference <> value Then
                    If Collection IsNot Nothing Then
                        Throw New NotSupportedException(String.Format("Can't change ObjectReference after item was added to {0}", GetType(IssueCollection).Name))
                    End If
                    If value Is Nothing Then
                        value = String.Empty
                    End If
                    _objectReference = value
                    _hasChanges = True
                End If
            End Set
        End Property

        <DataMember> Property Description As String
            Get
                Return _description
            End Get
            Friend Set(value As String)
                If _description <> value Then
                    _description = value
                    _hasChanges = True
                End If
            End Set
        End Property

        <DataMember> Property IssueTag As String
            Get
                Return _issueTag
            End Get
            Friend Set(value As String)
                If _issueTag <> value Then
                    _issueTag = value
                    _hasChanges = True
                End If
            End Set
        End Property

        ReadOnly Property NofOccurrences As UInteger
            Get
                Return _noOfOccurrences
            End Get
        End Property

        <Browsable(False)>
        Property Collection As IssueCollection

        <DataMember> Property ConfirmedBy As String
            Get
                Return _confirmedBy
            End Get
            Set(value As String)
                If value <> _confirmedBy Then
                    _confirmedBy = value
                    _hasChanges = True
                End If
            End Set
        End Property

        <DataMember> Property DateOfConfirmation As Nullable(Of DateTime)
            Get
                Return _dateOfConfirmation
            End Get
            Friend Set(value As Nullable(Of DateTime))
                If (value.HasValue AndAlso Not value.Equals(_dateOfConfirmation)) OrElse (Not value.HasValue AndAlso _dateOfConfirmation.HasValue) Then
                    _dateOfConfirmation = value
                    _hasChanges = True
                End If
            End Set
        End Property

        <DataMember> Property ConfirmationComment As String
            Get
                Return _confirmationComment
            End Get
            Set(value As String)
                If value <> _confirmationComment Then
                    _confirmationComment = value

                End If
            End Set
        End Property

        <Browsable(False)>
        ReadOnly Property vdIssues As List(Of VdIssue)
            Get
                Return _vdIssues
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return "Id: " & _id.ToString & "ObjectReference: " & ObjectReference '& str.ToString
        End Function

        <OnDeserialized()>
        Private Sub OnDeserialized(context As StreamingContext)
            _vdIssues = New List(Of VdIssue)
        End Sub

        ReadOnly Property HasChanges As Boolean
            Get
                Return _hasChanges
            End Get
        End Property

        Public Sub AcceptChanges()
            _hasChanges = False
        End Sub

    End Class


End Namespace