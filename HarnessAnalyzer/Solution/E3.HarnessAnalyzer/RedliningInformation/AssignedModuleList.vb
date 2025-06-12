Imports System.ComponentModel

Public Class AssignedModuleList
    Inherits BindingList(Of AssignedModule)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function FindModuleFromAbbreviation(abbreviation As String) As AssignedModule
        For Each assModule As AssignedModule In Me
            If (assModule.Abbreviation = abbreviation) Then
                Return assModule
            End If
        Next

        Return Nothing
    End Function

    Public Function FindModuleFromPartNumber(partNumber As String) As AssignedModule
        For Each assModule As AssignedModule In Me
            If (assModule.PartNumber = partNumber) Then
                Return assModule
            End If
        Next

        Return Nothing
    End Function

End Class