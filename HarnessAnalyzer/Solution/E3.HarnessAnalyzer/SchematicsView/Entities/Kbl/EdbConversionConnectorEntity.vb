Imports Zuken.E3.App.Controls

Namespace Schematics.Converter.Kbl

    Public Class EdbConversionConnectorEntity
        Inherits BaseChildrensEntity(Of Connectivity.Model.Connector, EdbConversionConnectorEntity)

        Private _connectorType As Connectivity.Model.ConnectorType
        Private _cavityEdbSysIdsAdded As New HashSet(Of String)
        Private _matingConnectorIds As New HashSet(Of String)

        Public Sub New(info As ConnectorInfo, edbConnector As Connectivity.Model.Connector, connectorType As Connectivity.Model.ConnectorType)
            Me.New(info.DocumentId, info.OriginalIds.SingleOrDefault, edbConnector, connectorType)
        End Sub

        Public Sub New(documentId As String, originalSystemId As String, edbConnector As Connectivity.Model.Connector, connectorType As Connectivity.Model.ConnectorType)
            MyBase.New(documentId, originalSystemId, edbConnector)
            _connectorType = connectorType
        End Sub

        Public ReadOnly Property MatingConnectorIds As HashSet(Of String)
            Get
                Return _matingConnectorIds
            End Get
        End Property

        Public Sub AddMatingConnector(conn As EdbConversionConnectorEntity)
            Me.EdbItem.AddMatingConnector(conn.EdbItem)
            _matingConnectorIds.Add(conn.Id)
        End Sub

        Protected Overrides Sub OnAfterChildrenCollectionItemsAdded(newItems As IEnumerable(Of EdbConversionConnectorEntity))
            For Each item As EdbConversionConnectorEntity In newItems
                Me.EdbItem.AddMatingConnector(item.EdbItem)
            Next
        End Sub

        ReadOnly Property ConnectorType As Connectivity.Model.ConnectorType
            Get
                Return _connectorType
            End Get
        End Property

        Property OwnerComponent As EdbConversionComponentEntity

        Public Function AddNewCavity(Of T As EdbConversionCavityEntity)(documentId As String, edbSysId As String, originalIds() As String, name As String, Optional addToModel As Boolean = True) As T
            If _cavityEdbSysIdsAdded.Add(edbSysId) Then
                Dim modelCavity As Connectivity.Model.Cavity = If(addToModel, Me.EdbItem.AddNewCavity(edbSysId, name), Nothing)
                Return CType(Activator.CreateInstance(GetType(T), New Object() {documentId, originalIds, edbSysId, name, modelCavity, Me.Id}), T)
            End If
            Throw New ArgumentException(String.Format("Cavity with id ""{0}"" already added to connector ""{1}"" ({2})", edbSysId, Me.Id, Me.ShortName))
        End Function

        ReadOnly Property Cavities As HashSet(Of String)
            Get
                Return _cavityEdbSysIdsAdded
            End Get
        End Property

    End Class

End Namespace

