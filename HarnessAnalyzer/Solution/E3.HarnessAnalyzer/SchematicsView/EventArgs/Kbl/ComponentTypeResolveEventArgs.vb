Imports Zuken.E3.App.Controls

Namespace Schematics.Converter.Kbl

    Public Class ComponentTypeResolveEventArgs
        Inherits ConverterEventArgs

        Private _componentShortName As String

        Public Sub New(componentShortName As String, conversionId As Guid)
            MyBase.New(conversionId)
            _componentShortName = componentShortName
        End Sub

        ReadOnly Property ComponentShortName As String
            Get
                Return _componentShortName
            End Get
        End Property

        Property ComponentType As Nullable(Of Connectivity.Model.ComponentType) = Nothing

    End Class

End Namespace
