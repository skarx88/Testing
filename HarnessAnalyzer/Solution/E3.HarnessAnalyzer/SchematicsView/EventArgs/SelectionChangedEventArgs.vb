Imports Zuken.E3.HarnessAnalyzer.Schematics.Converter

Namespace Schematics

    Public Class SelectionChangedEventArgs
        Inherits EventArgs

        Private _selected As EdbConversionEntityInfo()

        Public Sub New(ParamArray selected As EdbConversionEntityInfo())
            _selected = selected
        End Sub

        ReadOnly Property Selected As EdbConversionEntityInfo()
            Get
                Return _selected
            End Get
        End Property

    End Class

End Namespace
