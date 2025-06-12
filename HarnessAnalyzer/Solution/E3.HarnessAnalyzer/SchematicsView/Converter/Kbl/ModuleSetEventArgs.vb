Namespace Schematics.Converter.Kbl

    Public Class ModuleSetEventArgs
        Inherits EventArgs

        Private _modules As IEnumerable(Of ModuleInfo)
        Private _edbObjectId As String

        Public Sub New(modules As IEnumerable(Of ModuleInfo), edbObjectId As String)
            MyBase.New()
            _modules = modules
            _edbObjectId = edbObjectId
        End Sub

        ReadOnly Property EdbObjectId As String
            Get
                Return _edbObjectId
            End Get
        End Property

        ReadOnly Property Modules As IEnumerable(Of ModuleInfo)
            Get
                Return _modules
            End Get
        End Property

    End Class

End Namespace
