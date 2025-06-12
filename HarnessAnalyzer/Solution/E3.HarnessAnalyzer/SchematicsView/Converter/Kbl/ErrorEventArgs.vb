Namespace Schematics.Converter.Kbl

    Public Class ErrorEventArgs
        Inherits EventArgs

        Private _errorType As EdbConverterErrorType
        Private _object As Object
        Private _documentId As String
        Private _message As String

        Public Sub New(type As EdbConverterErrorType, documentId As String, [object] As Object, message As String)
            _errorType = type
            _documentId = documentId
            _object = [object]
            _message = message
        End Sub

        ReadOnly Property ErrorType As EdbConverterErrorType
            Get
                Return _errorType
            End Get
        End Property

        ReadOnly Property [Object] As Object
            Get
                Return _object
            End Get
        End Property

        ReadOnly Property DocumentId As String
            Get
                Return _documentId
            End Get
        End Property

        ReadOnly Property Message As String
            Get
                Return _message
            End Get
        End Property

        Public Enum EdbConverterErrorType
            Unknown = 0
            InitEntityAlreadyExistsError = 1
        End Enum

    End Class


End Namespace