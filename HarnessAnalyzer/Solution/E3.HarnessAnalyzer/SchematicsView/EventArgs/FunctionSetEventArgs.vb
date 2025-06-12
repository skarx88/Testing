Namespace Schematics.Converter

    Public Class FunctionSetEventArgs
        Inherits EventArgs

        Private _functionId As String
        Private _functionName As String
        Private _edbObjectId As String

        Public Sub New(functionId As String, edbObjectId As String, Optional functionName As String = "")
            MyBase.New()
            _functionName = functionName
            _functionId = functionId
            _edbObjectId = edbObjectId
        End Sub

        ReadOnly Property EdbObjectId As String
            Get
                Return _edbObjectId
            End Get
        End Property

        ReadOnly Property FunctionId As String
            Get
                Return _functionId
            End Get
        End Property

        ReadOnly Property FunctionName As String
            Get
                Return _functionName
            End Get
        End Property

    End Class

End Namespace