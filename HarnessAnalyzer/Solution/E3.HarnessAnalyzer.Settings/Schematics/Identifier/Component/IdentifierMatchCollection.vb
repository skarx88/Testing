Namespace Schematics.Identifier.Component

    Public Class IdentifierMatchCollection
        Implements IEnumerable(Of IdentifierMatch)

        Private _matchesList As New List(Of IdentifierMatch)

        Public Sub New()
        End Sub

        Public Sub New(results As IEnumerable(Of IdentifierMatch))
            Me.New()
            If results IsNot Nothing AndAlso results.Count > 0 Then
                _matchesList = New List(Of IdentifierMatch)(results)
            End If
        End Sub

        ReadOnly Property Count As Integer
            Get
                Return _matchesList.Count
            End Get
        End Property

        Default ReadOnly Property Item(index As Integer) As IdentifierMatch
            Get
                Return _matchesList(index)
            End Get
        End Property

        Public Function GetEnumerator() As IEnumerator(Of IdentifierMatch) Implements IEnumerable(Of IdentifierMatch).GetEnumerator
            Return _matchesList.GetEnumerator
        End Function

        Private Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Return _matchesList.GetEnumerator
        End Function

    End Class

End Namespace