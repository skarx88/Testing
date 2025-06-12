Namespace Schematics.Identifier.Component

    Public Class SuffixMatch
        Private _index As Integer
        Private _suffix As String

        Public Sub New(index As Integer, suffix As String)
            _index = index
            _suffix = suffix
        End Sub

        ReadOnly Property Index As Integer
            Get
                Return _index
            End Get
        End Property

        ReadOnly Property Suffix As String
            Get
                Return _suffix
            End Get
        End Property
    End Class

End Namespace