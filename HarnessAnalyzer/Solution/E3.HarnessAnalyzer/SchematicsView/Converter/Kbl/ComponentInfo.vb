Imports Zuken.E3.App.Controls

Namespace Schematics.Converter.Kbl

    ''' <summary>
    ''' Information how to create a component in the model (mostly virtual components)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ComponentInfo
        Inherits EdbConversionEntityInfo

        Private WithEvents _connectors As New ConnectorInfoCollection()

        Friend Sub New(shortName As String, initialConnector As ConnectorInfo)
            Me.New(shortName)
            _connectors = New ConnectorInfoCollection(initialConnector)
        End Sub

        Public Sub New(shortName As String, Optional type As Nullable(Of Connectivity.Model.ComponentType) = Nothing)
            Me.New(Guid.NewGuid.ToString, shortName, type)
        End Sub

        Public Sub New(id As String, shortName As String, Optional type As Nullable(Of Connectivity.Model.ComponentType) = Nothing)
            Me.New(id, id, shortName, type)
        End Sub

        Public Sub New(id As String, originalId As String, shortName As String, Optional type As Nullable(Of Connectivity.Model.ComponentType) = Nothing)
            MyBase.New(String.Empty, id, originalId, shortName, Connectivity.Model.ObjType.Component)
            Me.Type = type
            Me.ShortName = shortName
        End Sub

        Property Type As Nullable(Of Connectivity.Model.ComponentType)

        ReadOnly Property Connectors As ConnectorInfoCollection
            Get
                Return _connectors
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("ShortName: {0}, Connectors: {1}", Me.ShortName, Me.Connectors.Count)
        End Function

        Private Sub _connectors_CollectionChanged(sender As Object, e As Specialized.NotifyCollectionChangedEventArgs) Handles _connectors.CollectionChanged
            Select Case e.Action
                Case Specialized.NotifyCollectionChangedAction.Add
                    For Each item As ConnectorInfo In e.NewItems
                        item.Component = Me
                    Next
                Case Specialized.NotifyCollectionChangedAction.Remove
                    For Each item As ConnectorInfo In e.OldItems
                        item.Component = Nothing
                    Next
                Case Specialized.NotifyCollectionChangedAction.Replace
                    For Each item As ConnectorInfo In e.OldItems
                        item.Component = Nothing
                    Next
                    For Each item As ConnectorInfo In e.NewItems
                        item.Component = Me
                    Next
                Case Else
                    Throw New NotSupportedException(String.Format("Not supported collection change: {0}", e.Action.ToString))
            End Select
        End Sub
    End Class

End Namespace