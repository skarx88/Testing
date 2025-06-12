Namespace Schematics.Converter.Kbl

    Partial Public Class KblDocumentDataCollection

        Public Class GenerationEventArgs
            Inherits Converter.ConverterEventArgs

            Private _startedDocumentId As String
            Private _isAutoGenerating As Boolean

            Public Sub New(startedDocumentId As String, isAutoGenerating As Boolean, generateId As Guid)
                MyBase.New(generateId)
                _startedDocumentId = startedDocumentId
                _isAutoGenerating = isAutoGenerating
            End Sub

            Public Sub New(startedAtdocumentId As String, isAutoGenerating As Boolean)
                Me.New(startedAtdocumentId, isAutoGenerating, Guid.NewGuid)
            End Sub

            ReadOnly Property StartedDocumentId As String
                Get
                    Return _startedDocumentId
                End Get
            End Property

            ReadOnly Property IsAutoGenerating As Boolean
                Get
                    Return _isAutoGenerating
                End Get
            End Property

        End Class

        Public Class GenerationFinishedEventArgs
            Inherits GenerationEventArgs

            Private _cancelled As Boolean
            Private _result As KblConversionResult

            Public Sub New(startedDocumentId As String, cancelled As Boolean, isAutoGenerating As Boolean, generateId As Guid, result As KblConversionResult)
                MyBase.New(startedDocumentId, isAutoGenerating, generateId)
                _cancelled = cancelled
                _result = result
            End Sub

            ReadOnly Property Cancelled As Boolean
                Get
                    Return _cancelled
                End Get
            End Property

            ReadOnly Property Result As KblConversionResult
                Get
                    Return _result
                End Get
            End Property

        End Class

    End Class

End Namespace