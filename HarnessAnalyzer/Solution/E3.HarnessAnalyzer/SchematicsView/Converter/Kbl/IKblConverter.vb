Namespace Schematics.Converter.Kbl

    Public Interface IKblConverter
        Inherits IEdbConverter

        Event ConnectorResolving(sender As Object, e As ConverterEventArgs)

    End Interface

End Namespace