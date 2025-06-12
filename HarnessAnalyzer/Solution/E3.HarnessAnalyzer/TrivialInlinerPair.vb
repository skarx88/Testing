Public Class TrivialInlinerPair
    Inherits InlinerPair

    Private _connectorOccA As Connector_occurrence
    Private _connectorOccB As Connector_occurrence
    Private _kblMapperA As KblMapper
    Private _kblMapperB As KblMapper

    Public Sub New(connectorFaceA As VdSVGGroup, connectorOccA As Connector_occurrence, inactiveObjectsA As ITypeGroupedKblIds, inlinerIdA As String, kblMapperA As KblMapper)
        MyBase.New()

        _connectorOccA = connectorOccA
        _kblMapperA = kblMapperA

        Me.ConnectorFacesA.Add(_connectorOccA.Id, _connectorOccA.SystemId, connectorFaceA)
        Me.InactiveObjectsA = inactiveObjectsA
        Me.InlinerIdA = inlinerIdA
    End Sub

    ReadOnly Property ConnectorOccA As Connector_occurrence
        Get
            Return _connectorOccA
        End Get
    End Property

    Property ConnectorOccB As Connector_occurrence
        Get
            Return _connectorOccB
        End Get
        Set(value As Connector_occurrence)
            _connectorOccB = value
        End Set
    End Property

    ReadOnly Property KblMapperA As KblMapper
        Get
            Return _kblMapperA
        End Get
    End Property

    Property KblMapperB As KblMapper
        Get
            Return _kblMapperB
        End Get
        Set(value As KblMapper)
            _kblMapperB = value
        End Set
    End Property

End Class