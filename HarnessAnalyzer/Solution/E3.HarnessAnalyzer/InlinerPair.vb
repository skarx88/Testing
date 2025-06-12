Imports Zuken.E3.HarnessAnalyzer.Project.Documents

Public MustInherit Class InlinerPair

    Private _id As String
    Private _connectorFacesA As DictionaryKblIds(Of VdSVGGroup)
    Private _connectorFacesB As DictionaryKblIds(Of VdSVGGroup)
    Private _inactiveObjectsA As ITypeGroupedKblIds
    Private _inactiveObjectsB As ITypeGroupedKblIds
    Private _inlinerIdA As String
    Private _inlinerIdB As String
    'HINT this was added as there were nonsense searches via kbl ids implemented in the Inliner Details (should be restructured...) 
    Private _documentA As HcvDocument
    Private _documentB As HcvDocument

    Public Sub New()
        _connectorFacesA = New DictionaryKblIds(Of VdSVGGroup)
        _connectorFacesB = New DictionaryKblIds(Of VdSVGGroup)
    End Sub

    Public Sub New(id As String)
        _connectorFacesA = New DictionaryKblIds(Of VdSVGGroup)
        _connectorFacesB = New DictionaryKblIds(Of VdSVGGroup)
        _id = id
    End Sub

    ReadOnly Property ConnectorFacesA As DictionaryKblIds(Of VdSVGGroup)
        Get
            Return _connectorFacesA
        End Get
    End Property

    ReadOnly Property ConnectorFacesB As DictionaryKblIds(Of VdSVGGroup)
        Get
            Return _connectorFacesB
        End Get
    End Property

    Property Id As String
        Get
            Return _id
        End Get
        Set(value As String)
            _id = value
        End Set
    End Property

    Property InactiveObjectsA As ITypeGroupedKblIds
        Get
            Return _inactiveObjectsA
        End Get
        Set(value As ITypeGroupedKblIds)
            _inactiveObjectsA = value
        End Set
    End Property

    Property InactiveObjectsB As ITypeGroupedKblIds
        Get
            Return _inactiveObjectsB
        End Get
        Set(value As ITypeGroupedKblIds)
            _inactiveObjectsB = value
        End Set
    End Property

    Property InlinerIdA As String
        Get
            Return _inlinerIdA
        End Get
        Set(value As String)
            _inlinerIdA = value
        End Set
    End Property

    Property InlinerIdB As String
        Get
            Return _inlinerIdB
        End Get
        Set(value As String)
            _inlinerIdB = value
        End Set
    End Property

    Property DocumentA As HcvDocument
        Get
            Return _documentA
        End Get
        Set(value As HcvDocument)
            _documentA = value
        End Set
    End Property

    Property DocumentB As HcvDocument
        Get
            Return _documentB
        End Get
        Set(value As HcvDocument)
            _documentB = value
        End Set
    End Property

    Public Shared ReadOnly Property [Empty] As InlinerPair
        Get
            Static instance As New EmptyInlinerPair
            Return instance
        End Get
    End Property

    Private Class EmptyInlinerPair
        Inherits InlinerPair

        Public Sub New()
        End Sub

        Public Sub New(id As String)
            MyBase.New(id)
        End Sub
    End Class

End Class