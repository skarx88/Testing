Imports System.Runtime.Serialization
Imports System.Xml.Serialization

Namespace Checks.Cavities.Files

    <CollectionDataContract(Name:="Connectors", ItemName:="Connector", [Namespace]:=CavityChecksFile.Namespace)>
    <XmlType(TypeName:="Connectors")> ' for schema generation
    <KnownType(GetType(ObjectModel.KeyedCollection(Of String, ConnectorEntry)))>
    Public Class ConnectorEntryCollection
        Inherits ObjectModel.KeyedCollection(Of String, ConnectorEntry)

        Friend Sub New()
        End Sub

        Friend Sub New(file As CavityChecksFile)
            Me.File = file
        End Sub

        Public Sub UpdateAll()
            Me.Clear()
            For Each cv As Views.Model.ConnectorView In _File.Document.Model.Connectors.Where(Function(c) _File.Document.Model.Settings.Connectors.Contains(c.KblId))
                If Not Me.Contains(cv.KblId) Then
                    AddNewConnectorView(cv)
                End If
            Next
        End Sub

        Protected Overrides Function GetKeyForItem(item As ConnectorEntry) As String
            Return item.KblId
        End Function

        Public Function AddNewOrGetConnectorEntry(connectorKblId As String) As ConnectorEntry
            Dim entry As ConnectorEntry = Nothing
            If Me.Dictionary IsNot Nothing Then
                Me.Dictionary.TryGetValue(connectorKblId, entry)
            End If

            If entry Is Nothing Then
                Dim conn As Views.Model.ConnectorView = _File.Document.Model.Connectors.Where(Function(c) c.KblId = connectorKblId).SingleOrDefault
                If conn IsNot Nothing Then
                    entry = AddNewConnectorView(conn)
                End If
            End If
            Return entry
        End Function

        Private Function AddNewConnectorView(connView As Views.Model.ConnectorView) As ConnectorEntry
            Dim entry As New ConnectorEntry(connView)
            Me.Add(entry)
            Return entry
        End Function

        Friend Property File As CavityChecksFile

    End Class


End Namespace
