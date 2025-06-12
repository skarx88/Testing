Imports Zuken.E3.HarnessAnalyzer.Schematics.Converter

Namespace Schematics.Controls

    Public Class EntityMouseEventArgs
        Inherits EntityEventArgs

        Private _mouseLoc As Point

        Public Sub New(conversionId As Guid, entity As EdbConversionEntity, mouseLocation As Point)
            MyBase.New(conversionId, entity)
            _mouseLoc = mouseLocation
        End Sub

        ReadOnly Property MouseLocation As Point
            Get
                Return _mouseLoc
            End Get
        End Property

    End Class

End Namespace
