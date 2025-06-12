Imports System.Text.RegularExpressions

Namespace Schematics.Identifier.Component

    Public Class IdentifierMatch

        Private _identifier As Identifier
        Private _group As IdentifierGroupBase
        Private _regExMatch As System.Text.RegularExpressions.Match

        Public Sub New(group As IdentifierGroupBase, identifier As Identifier, regExMatch As Match)
            _group = group
            _identifier = identifier
            _regExMatch = regExMatch
        End Sub

        ReadOnly Property Group As IdentifierGroupBase
            Get
                Return _group
            End Get
        End Property

        ReadOnly Property Identifier As Identifier
            Get
                Return _identifier
            End Get
        End Property

        ReadOnly Property RegExMatch As Match
            Get
                Return _regExMatch
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("Identifier: {0}, Group: {1}", If(Me.Identifier IsNot Nothing, Me.Identifier.ToString, "<Empty>"), If(Me.Group IsNot Nothing, Me.Group.ToString, "<Empty>"))
        End Function

    End Class

End Namespace