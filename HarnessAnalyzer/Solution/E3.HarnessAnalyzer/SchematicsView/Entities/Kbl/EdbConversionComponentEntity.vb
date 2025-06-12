Imports Zuken.E3.App.Controls

Namespace Schematics.Converter.Kbl

    Public Class EdbConversionComponentEntity
        Inherits BaseChildrensEntity(Of Connectivity.Model.Component, EdbConversionConnectorEntity)

        Private _componentType As Connectivity.Model.ComponentType

        Public Sub New(blockId As String, originalSystemId As String, edbComponent As Connectivity.Model.Component, componentType As Connectivity.Model.ComponentType)
            MyBase.New(blockId, originalSystemId, edbComponent)
            _componentType = componentType
        End Sub

        Protected Overrides Sub OnAfterChildrenCollectionItemsAdded(newItems As IEnumerable(Of EdbConversionConnectorEntity))
            'do nothing here because the AddNew in ConvertedSysIdConnectorsCollection already added the item to the EdbItem
        End Sub

        Public ReadOnly Property Connectors As ConvertedSysIdConnectorsCollection
            Get
                Return CType(MyBase.Children, ConvertedSysIdConnectorsCollection)
            End Get
        End Property

        Public Sub UpdateIsActive(model As EdbConversionDocument)
            Dim myIsActive As Boolean = Me.Connectors.Count = 0

            If Me.Connectors.Count > 0 Then
                For Each connector As EdbConversionConnectorEntity In Me.Connectors.Entities
                    Dim entities As IList(Of EdbConversionEntity) = Nothing
                    If model.TryGetEntitiesByOriginalId(connector.OriginalIds.Single, entities) Then
                        Dim foundConn As EdbConversionConnectorEntity = entities.OfType(Of EdbConversionConnectorEntity).SingleOrDefault
                        If foundConn IsNot Nothing AndAlso foundConn.IsActive Then
                            myIsActive = True
                            Exit For
                        End If
                    End If
                Next
            End If

            Me.IsActive = myIsActive
        End Sub

        ReadOnly Property ComponentType As Connectivity.Model.ComponentType
            Get
                Return _componentType
            End Get
        End Property

        Protected Overrides Function GetNewChildrens() As BaseChildrensEntity(Of Connectivity.Model.Component, EdbConversionConnectorEntity).IdCollection(Of EdbConversionConnectorEntity)
            Return New ConvertedSysIdConnectorsCollection(Me)
        End Function

        Public Class ConvertedSysIdConnectorsCollection
            Inherits IdCollection(Of EdbConversionConnectorEntity)

            Private _connectors As New List(Of EdbConversionConnectorEntity)
            Private _parentComponent As EdbConversionComponentEntity

            Public Sub New(parentComponent As EdbConversionComponentEntity)
                _parentComponent = parentComponent
            End Sub

            Public Shadows Function AddNew(blockId As String, originalSysId As String, sysId As String, shortName As String, connectorType As Connectivity.Model.ConnectorType) As EdbConversionConnectorEntity
                Dim edbConnector As Connectivity.Model.Connector = _parentComponent.EdbItem.AddNewConnector(sysId, shortName, connectorType)
                Dim newConnector As New EdbConversionConnectorEntity(blockId, originalSysId, edbConnector, connectorType)
                newConnector.OwnerComponent = _parentComponent
                If Me.TryAdd(newConnector) Then
                    _connectors.Add(newConnector)
                End If
                Return newConnector
            End Function

            ReadOnly Property Entities As IEnumerable(Of EdbConversionConnectorEntity)
                Get
                    Return _connectors
                End Get
            End Property


            Public Overrides Sub Clear()
                MyBase.Clear()
                _connectors.Clear()
            End Sub

        End Class

    End Class

End Namespace
