Imports System.ComponentModel

<Serializable()> _
Public Class ActiveModule

    Public Property Abbreviation As String
    Public Property PartNumber As String

    Public Sub New()
        Abbreviation = String.Empty
        PartNumber = String.Empty
    End Sub

End Class


Public Class ActiveModuleList
    Inherits BindingList(Of ActiveModule)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function FindModuleFromAbbreviation(abbreviation As String) As ActiveModule
        For Each actModule As ActiveModule In Me
            If actModule.Abbreviation = abbreviation Then Return actModule
        Next

        Return Nothing
    End Function

    Public Function FindModuleFromPartNumber(partNumber As String) As ActiveModule
        For Each actModule As ActiveModule In Me
            If actModule.PartNumber.Trim.Replace(" ", String.Empty) = partNumber.Trim.Replace(" ", String.Empty) Then
                Return actModule
            End If
        Next

        Return Nothing
    End Function

End Class