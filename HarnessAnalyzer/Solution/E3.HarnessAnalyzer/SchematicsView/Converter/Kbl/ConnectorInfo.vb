Imports Zuken.E3.App.Controls

Namespace Schematics.Converter.Kbl

    Public Class ConnectorInfo
        Inherits EdbConversionEntityInfo

        Private _component As ComponentInfo

        Friend Sub New(documentId As String, connectorId As String, originalId As String, shortName As String, inlinerName As String)
            MyBase.New(documentId, connectorId, originalId, shortName, Connectivity.Model.ObjType.Connector)
            Me.ShortName = shortName
            Me.InlinerName = inlinerName
            _component = New ComponentInfo(shortName, Me)
        End Sub

        Property InlinerName As String
        Property IsSplice As Boolean
        Property IsEyelet As Boolean
        Property Description As String = ""
        Property AliasIds As New List(Of String)
        Property InstallationInstructions As New List(Of String)

        Property Component As ComponentInfo
            Get
                Return _component
            End Get
            Set(value As ComponentInfo)
                If _component IsNot value Then
                    _component?.Connectors?.Remove(Me)
                    _component = value
                    _component?.Connectors?.TryAdd(Me)
                End If
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0}_{1}", DocumentId, Me.Id)
        End Function

    End Class

End Namespace
