Namespace Schematics.Converter.Kbl

    Partial Public Class KblDocumentDataCollection

        Property AutoGenerate As AutoGenerateType = AutoGenerateType.Async

        <System.Reflection.ObfuscationAttribute(Feature:="renaming", ApplyToMembers:=True)>
        ReadOnly Property IsGenerating As Boolean
            Get
                Return _isGenerating
            End Get
        End Property

    End Class

End Namespace