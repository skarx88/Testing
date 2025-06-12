Namespace Schematics.Converter.Kbl

    Public Class KblDocumentData

        Private _documentId As String
        Private _kbl As KBLMapper

        Public Sub New(documentId As String, kblData As KBLMapper)
            _documentId = documentId
            _kbl = kblData
        End Sub

        ReadOnly Property DocumentId As String
            Get
                Return _documentId
            End Get
        End Property

        ReadOnly Property Kbl As KBLMapper
            Get
                Return _kbl
            End Get
        End Property

    End Class

End Namespace
