Imports System.ComponentModel
Imports System.Xml.Serialization
Imports Zuken.E3.HarnessAnalyzer.Shared

<Serializable()>
<XmlInclude(GetType(AssignedModule))> _
Public Class Redlining

    Private _assignedModules As New AssignedModuleList
    Private _Classification As RedliningClassification
    Private _classificationValues As RedliningClassification() = [Enum].GetValues(Of RedliningClassification) ' for safety checking within on setting the property

    Public Sub New()
        Classification = RedliningClassification.Confirmation
        Comment = String.Empty
        LastChangedBy = System.Security.Principal.WindowsIdentity.GetCurrent.Name
        LastChangedOn = Now
        ID = Guid.NewGuid.ToString
        IsVisible = True
        ObjectId = String.Empty
        ObjectName = String.Empty
        ObjectType = KblObjectType.Undefined
    End Sub

    Public Sub New(objectKblId As String, objectKblName As String, objectKblType As KblObjectType, redliningClassification As RedliningClassification, redliningComment As String, isRedliningVisible As Boolean)
        Classification = redliningClassification
        Comment = redliningComment
        LastChangedBy = System.Security.Principal.WindowsIdentity.GetCurrent.Name
        LastChangedOn = Now
        ID = Guid.NewGuid.ToString
        IsVisible = isRedliningVisible
        ObjectId = objectKblId
        ObjectName = objectKblName
        ObjectType = objectKblType
    End Sub

    <Category("AssignedModules")>
    Public Property AssignedModules() As AssignedModuleList
        Get
            Return _assignedModules
        End Get
        Set(value As AssignedModuleList)
            _assignedModules = value
        End Set
    End Property

    Public Property ID As String
    Public Property ObjectId As String
    Public Property ObjectName As String
    <XmlIgnore()>
    Public Property ObjectType As KblObjectType

    Public Property Classification As RedliningClassification
        Get
            Return _Classification
        End Get
        Set
            If _classificationValues.Contains(Value) Then
                _Classification = Value
            Else
                Throw New InvalidEnumArgumentException(NameOf(Classification), Value, GetType(RedliningClassification))
            End If
        End Set
    End Property

    Public Property Comment As String
    Public Property IsVisible As Boolean
    Public Property LastChangedBy As String
    Public Property LastChangedOn As Date
    Public Property OnGroup As String

    Public Function GetLocalizedClassificationString() As String
        Return Redlining.GetLocalizedClassificationString(Me.Classification)
    End Function

    Public Shared Function GetLocalizedClassificationString(classification As RedliningClassification) As String
        Select Case classification
            Case RedliningClassification.Confirmation
                Return InformationHubStrings.Confirmation_RedliningType
            Case RedliningClassification.Error
                Return InformationHubStrings.Error_RedliningType
            Case RedliningClassification.GraphicalComment
                Return InformationHubStrings.Graphical_RedliningType
            Case RedliningClassification.Information
                Return InformationHubStrings.Info_RedliningType
            Case RedliningClassification.LengthComment
                Return InformationHubStrings.Length_RedliningType
            Case RedliningClassification.Question
                Return InformationHubStrings.Question_RedliningType
            Case Else
                Return classification.ToString
        End Select
    End Function

    Public Shared Function ParseFromLocalizedClassificationString(value As String) As RedliningClassification
        Select Case value
            Case InformationHubStrings.Confirmation_RedliningType
                Return RedliningClassification.Confirmation
            Case InformationHubStrings.Error_RedliningType
                Return RedliningClassification.Error
            Case InformationHubStrings.Graphical_RedliningType
                Return RedliningClassification.GraphicalComment
            Case InformationHubStrings.Info_RedliningType
                Return RedliningClassification.Information
            Case InformationHubStrings.Length_RedliningType
                Return RedliningClassification.LengthComment
            Case InformationHubStrings.Question_RedliningType
                Return RedliningClassification.Question
            Case Else
                Throw New NotImplementedException($"Localized value for parsing to ""{NameOf(RedliningClassification)}"" not implemented!")
        End Select
    End Function

    Public Function GetClassificationImage() As Bitmap
        Return GetClassificationImage(Me)
    End Function

    Public Shared Function GetClassificationImage(r As Redlining) As Bitmap
        Dim bmp As New Bitmap(My.Resources.RedliningConfirmation)
        Select Case r.Classification
            Case RedliningClassification.Confirmation
                Return My.Resources.RedliningConfirmationPen
            Case RedliningClassification.Error
                Return My.Resources.RedliningErrorPen
            Case RedliningClassification.LengthComment
                Return My.Resources.RedliningLengthCommentPen
            Case RedliningClassification.Question
                Return My.Resources.RedliningQuestionPen
            Case RedliningClassification.Information
                Return My.Resources.RedliningInformationPen
        End Select
        Return bmp
    End Function

End Class