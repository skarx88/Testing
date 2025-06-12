Imports System.Runtime.Serialization

Namespace IssueReporting

    <CollectionDataContract(Namespace:="Zuken.E3.HarnessAnalyzer.IssueReporting")>
    Public Class IssueCollection
        Inherits System.Collections.ObjectModel.KeyedCollection(Of String, Issue)

        Private _objectReferenceMapper As New Dictionary(Of String, List(Of Issue))

        Public Sub New()
        End Sub

        Public Function AddNew(id As String, objectReference As String, Optional description As String = "", Optional issueTag As String = "", Optional NofOccurences As UInteger = 0,
                               Optional confirmedBy As String = "", Optional dateOfConfirmation As Nullable(Of DateTime) = Nothing, Optional confirmationComment As String = "") As Issue
            Dim newStamp As New Issue(id, NofOccurences)
            With newStamp
                .ObjectReference = objectReference
                .Description = description
                .IssueTag = issueTag
                .ConfirmedBy = confirmedBy
                .DateOfConfirmation = dateOfConfirmation
                .ConfirmationComment = confirmationComment
            End With
            Me.Add(newStamp)
            Return newStamp
        End Function

        Protected Overrides Sub InsertItem(index As Integer, item As Issue)
            MyBase.InsertItem(index, item)
            item.Collection = Me
            _objectReferenceMapper.AddOrUpdate(item.ObjectReference, item)
        End Sub

        Protected Overrides Sub RemoveItem(index As Integer)
            Dim item As Issue = Me(index)
            MyBase.RemoveItem(index)
            item.Collection = Nothing
            _objectReferenceMapper.TryRemove(Item.ObjectReference, Item)
        End Sub

        Protected Overrides Sub ClearItems()
            Dim items As Issue() = Me.ToArray
            MyBase.ClearItems()
            _objectReferenceMapper.Clear()
            For Each Item As Issue In items
                Item.Collection = Nothing
            Next
        End Sub

        Public Function FindOfKblId(KblId As String) As List(Of Issue)
            If _objectReferenceMapper.ContainsKey(KblId) Then
                Return _objectReferenceMapper(KblId)
            End If
            Return New List(Of Issue)
        End Function

        Protected Overrides Function GetKeyForItem(item As Issue) As String
            Return item.Id
        End Function

    End Class

End Namespace