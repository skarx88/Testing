Imports System.Text.RegularExpressions

Namespace Schematics.Identifier

    Public Class MatchResult

        Protected Sub New()
        End Sub

        Public Sub New(success As Boolean)
            Me.New()
            Me.Success = success
        End Sub

        Public Sub New(regExMatch As Match)
            Me.New(regExMatch.Success)
            Me.RegExMatch = regExMatch
        End Sub

        Property Success As Boolean
        Property RegExMatch As Match

    End Class

End Namespace