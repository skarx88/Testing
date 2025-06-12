Imports Zuken.E3.App.Controls

Namespace Schematics.Converter

    Public Class FinishedEventArgs
        Inherits ConverterEventArgs

        Private _failed As Boolean
        Private _cancelled As Boolean
        Private _edbModel As Connectivity.Model.EdbModel

        Public Sub New(conversionId As Guid, failed As Boolean, cancelled As Boolean, edbModel As Connectivity.Model.EdbModel)
            MyBase.New(conversionId)
            _failed = failed
            _cancelled = cancelled
            _edbModel = edbModel
        End Sub

        ReadOnly Property Failed As Boolean
            Get
                Return _failed
            End Get
        End Property

        ReadOnly Property Cancelled As Boolean
            Get
                Return _cancelled
            End Get
        End Property

        ReadOnly Property EdbModel As Connectivity.Model.EdbModel
            Get
                Return _edbModel
            End Get
        End Property

    End Class

End Namespace
