Imports Zuken.E3.HarnessAnalyzer.Schematics.Converter

Namespace Schematics.Controls

    Public Class EntityEventArgs
        Inherits IdEventArgs

        Private _entity As EdbConversionEntity

        Public Sub New(conversionId As Guid, entity As EdbConversionEntity)
            MyBase.New(conversionId, entity.DocumentId, entity.Id)
            _entity = entity
        End Sub

        ReadOnly Property Entity As EdbConversionEntity
            Get
                Return _entity
            End Get
        End Property

    End Class

End Namespace
