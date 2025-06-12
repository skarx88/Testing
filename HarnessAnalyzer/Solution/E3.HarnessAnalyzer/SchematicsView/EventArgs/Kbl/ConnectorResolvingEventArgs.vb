Namespace Schematics.Converter.Kbl

    Public Class ConnectorResolvingEventArgs
        Inherits ConverterEventArgs

        Private _connector As ConnectorInfo

        Public Sub New(conversionId As Guid, connector As ConnectorInfo)
            MyBase.New(conversionId)
            _connector = connector
        End Sub

        ReadOnly Property Connector As ConnectorInfo
            Get
                Return _connector
            End Get
        End Property

    End Class

End Namespace
