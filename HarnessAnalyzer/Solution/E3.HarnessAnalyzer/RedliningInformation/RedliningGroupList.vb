Imports System.ComponentModel

Public Class RedliningGroupList
    Inherits BindingList(Of RedliningGroup)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub AddRedliningGroup(redliningGroup As RedliningGroup)
        If (Not ContainsRedliningGroup(redliningGroup)) Then
            Me.Add(redliningGroup)
        End If
    End Sub

    Public Function ContainsRedliningGroup(redliningGroup As RedliningGroup) As Boolean
        If (Where(Function(rl_group) rl_group.ID = redliningGroup.ID).Any()) Then
            Return True
        End If

        Return False
    End Function

    Public Sub DeleteRedliningGroup(redliningGroup As RedliningGroup)
        Me.Remove(redliningGroup)
    End Sub

    Public Function FindRedliningGroups(changeTag As String) As IEnumerable(Of RedliningGroup)
        Return Me.Where(Function(rl_group) rl_group.ChangeTag = changeTag)
    End Function

End Class