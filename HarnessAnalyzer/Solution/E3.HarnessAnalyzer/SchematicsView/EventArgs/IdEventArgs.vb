Namespace Schematics.Converter

    Public Class IdEventArgs
        Inherits ConverterEventArgs

        Private _documentId As String
        Private _id As String

        Public Sub New(conversionId As Guid, documentId As String, id As String)
            MyBase.New(conversionId)
            _documentId = documentId
            _id = id
        End Sub

        ReadOnly Property documentId As String
            Get
                Return _documentId
            End Get
        End Property

        ReadOnly Property Id As String
            Get
                Return _id
            End Get
        End Property

        Property EdbId As String = Nothing

    End Class

End Namespace
