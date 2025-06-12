Namespace Schematics.Identifier.Component

    Public Class IdentifierResultCollection
        Inherits List(Of IdentifierResult)

        Public Sub New()
        End Sub

        Public Sub New(connectorShortName As String, searchInGroups As IdentifierGroupCollectionBase)
            Dim matches As IdentifierMatchCollection = searchInGroups.TryGetMatches(connectorShortName)
            Dim componentTypesAndNames As New List(Of Tuple(Of Integer, String))
            If matches.Count > 0 Then
                For Each match As IdentifierMatch In matches
                    Me.Add(New IdentifierResult(connectorShortName, match))
                Next
            ElseIf searchInGroups.DefaultGroup IsNot Nothing Then
                Me.Add(New IdentifierResult(connectorShortName, New IdentifierMatch(searchInGroups.DefaultGroup, Nothing, Nothing)))
            End If
        End Sub

    End Class

End Namespace