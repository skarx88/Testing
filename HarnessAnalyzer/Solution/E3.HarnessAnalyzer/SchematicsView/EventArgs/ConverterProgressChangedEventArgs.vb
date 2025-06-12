Namespace Schematics.Converter

    Public Class ConverterProgressChangedEventArgs
        Inherits Schematics.Converter.ConverterEventArgs

        Private _progressPercentage As Integer

        Public Sub New(progressPercentage As Integer, converterId As Guid)
            MyBase.New(converterId)
            _progressPercentage = progressPercentage
        End Sub

        ReadOnly Property ProgressPercentage As Integer
            Get
                Return _progressPercentage
            End Get
        End Property

    End Class

End Namespace