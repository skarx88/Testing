Imports System.Runtime.CompilerServices

Friend Module CompareExtensions

    <Extension>
    Public Function GetLocalizedDiffType(signature As CompareSignatures) As String
        Select Case signature.DiffType
            Case NameOf(GraphicalCompareFormStrings.Added)
                Return GraphicalCompareFormStrings.Added
            Case NameOf(GraphicalCompareFormStrings.Modified)
                Return GraphicalCompareFormStrings.Modified
            Case NameOf(GraphicalCompareFormStrings.Deleted)
                Return GraphicalCompareFormStrings.Deleted
            Case Else
                Return GraphicalCompareFormStrings.ResourceManager.GetString(signature.DiffType)
        End Select
    End Function

    <Extension>
    Public Function GetLocalizedEntitySignatureType(signature As CompareSignatures) As String
        Select Case signature.EntitySignature
            Case NameOf(GraphicalCompareFormStrings.Added)
                Return GraphicalCompareFormStrings.Added
            Case NameOf(GraphicalCompareFormStrings.Modified)
                Return GraphicalCompareFormStrings.Modified
            Case NameOf(GraphicalCompareFormStrings.Deleted)
                Return GraphicalCompareFormStrings.Deleted
            Case Else
                Return signature.EntitySignature
        End Select
    End Function

End Module

Public Class GCompareUtils

    Public Shared Function GetLocalizedString(input As String) As String
        Return GraphicalCompareFormStrings.ResourceManager.GetString(input)
    End Function

End Class