Imports System.Reflection

Namespace Schematics.Converter

    <ObfuscationAttribute(Feature:="renaming", ApplyToMembers:=True)>
    Public Class ConverterEventArgs
        Inherits EventArgs

        Private _converterId As Guid

        Public Sub New(converterId As Guid)
            _converterId = converterId
        End Sub

        ReadOnly Property ConverterId As Guid
            Get
                Return _converterId
            End Get
        End Property

    End Class

End Namespace