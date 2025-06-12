Imports Zuken.E3.Lib.Model
Imports Zuken.E3.Lib.Model.Utils

Public Class Utils

    Public Shared Function GetContainerTextWithNames(containerId As ContainerId, ParamArray names() As String) As String
        Dim textCase As TextCase = TextCase.ProperCase
        If names.Length > 1 Then
            textCase = textCase Or TextCase.Multiple
        End If
        Return GetContainerTextWithNames(GetContainerIdText(containerId, textCase), names)
    End Function

    Public Shared Function GetContainerTextWithNames(containerIdText As String, ParamArray names() As String) As String
        Return $"{containerIdText}: '{String.Join(",", names)}'"
    End Function

End Class