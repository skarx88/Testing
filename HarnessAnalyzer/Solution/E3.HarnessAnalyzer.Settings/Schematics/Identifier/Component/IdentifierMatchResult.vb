Namespace Schematics.Identifier.Component

    Public Class IdentifierMatchResult
        Inherits MatchResult

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(matchResult As MatchResult, identifier As Identifier)
            MyBase.New()
            With matchResult
                Me.Success = .Success
                Me.RegExMatch = .RegExMatch
            End With
            Me.Identifier = identifier
        End Sub

        Property Identifier As Identifier

    End Class

End Namespace