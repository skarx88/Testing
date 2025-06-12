Imports System.ComponentModel

Public Class RedliningList
    Inherits BindingList(Of Redlining)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function TryAdd(redlining As Redlining) As Boolean
        If Not Contains(redlining) Then
            Me.Add(redlining)
            Return True
        End If
        Return False
    End Function

    Public Overloads Function Contains(redlining As Redlining) As Boolean
        If Me.GetById(redlining.ID).Any Then
            Return True
        End If

        Return False
    End Function

    Public Sub DeleteRedlining(redlining As Redlining)
        Me.Remove(redlining)
    End Sub

    Public Function GetByClassification(classification As [Shared].RedliningClassification) As IEnumerable(Of Redlining)
        Return Me.Where(Function(rl) rl.Classification = classification)
    End Function

    Public Function GetByObjectId(objectId As String) As IEnumerable(Of Redlining)
        Return Me.Where(Function(rl) rl.ObjectId = objectId)
    End Function

    Public Function GetById(id As String) As IEnumerable(Of Redlining)
        Return Me.Where(Function(rl) rl.ID = id)
    End Function

    Public Function GetIds() As IEnumerable(Of String)
        Return Me.Select(Function(rl) rl.ID)
    End Function

    Public Function HasGraphicalComments() As Boolean
        Return GetByClassification([Shared].RedliningClassification.GraphicalComment).Any
    End Function

End Class