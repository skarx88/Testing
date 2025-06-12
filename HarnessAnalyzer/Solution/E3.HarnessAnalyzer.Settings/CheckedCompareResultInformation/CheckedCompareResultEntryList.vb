<Serializable()>
Public Class CheckedCompareResultEntryList
    Inherits System.Collections.ObjectModel.Collection(Of CheckedCompareResultEntry)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function AddCheckedCompareResultEntry(compareSignature As String, referenceSignature As String) As CheckedCompareResultEntry
        Dim checkedCompareResultEntry As CheckedCompareResultEntry = Nothing

        If (Not ContainsCheckedCompareResultEntry(compareSignature, referenceSignature)) Then
            checkedCompareResultEntry = New CheckedCompareResultEntry(compareSignature, referenceSignature)

            Me.Add(checkedCompareResultEntry)
        End If

        Return checkedCompareResultEntry
    End Function

    Public Function AddNew(compRefSig As String, Optional comment As String = "", Optional toBeChanged As Boolean = False) As CheckedCompareResultEntry
        Dim newEntry As CheckedCompareResultEntry = AddCheckedCompareResultEntry(compRefSig, compRefSig)
        With newEntry
            .Comment = comment
            .ToBeChanged = toBeChanged
        End With
        Return newEntry
    End Function

    Public Function SetOrAddNew(compRefSig As String, Optional comment As String = "", Optional toBeChanged As Boolean = False) As CheckedCompareResultEntry
        If Not Me.ContainsCheckedCompareResultEntry(compRefSig, compRefSig) Then
            Return Me.AddNew(compRefSig, comment, toBeChanged)
        End If

        Dim entry As CheckedCompareResultEntry = Me.GetEntry(compRefSig)
        entry.Comment = comment
        entry.ToBeChanged = toBeChanged
        Return entry
    End Function

    Public Overloads Function Remove(compRefSig As String) As Boolean
        Dim entry As CheckedCompareResultEntry = Me.GetEntry(compRefSig)
        If entry IsNot Nothing Then
            Me.DeleteCheckedCompareResultEntry(entry)
            Return True
        End If
        Return False
    End Function

    Public Function ContainsCheckedCompareResultEntry(compareSignature As String, referenceSignature As String) As Boolean
        For Each checkedCompareResultEntry As CheckedCompareResultEntry In Me
            If (checkedCompareResultEntry.CompareSignature = compareSignature) AndAlso (checkedCompareResultEntry.ReferenceSignature = referenceSignature) Then
                Return True
            End If
        Next

        Return False
    End Function

    Public Function FindCheckedCompareResultEntry(compareSignature As String, referenceSignature As String) As CheckedCompareResultEntry
        For Each checkedCompareResultEntry As CheckedCompareResultEntry In Me
            If (checkedCompareResultEntry.CompareSignature = compareSignature) AndAlso (checkedCompareResultEntry.ReferenceSignature = referenceSignature) Then
                Return checkedCompareResultEntry
            End If
        Next

        Return Nothing
    End Function

    Public Function GetEntries(ParamArray compRefSigs() As String) As List(Of CheckedCompareResultEntry)
        Dim resultEntries As New List(Of CheckedCompareResultEntry)
        Dim hashSigs As New HashSet(Of String)(compRefSigs)
        For Each entry As CheckedCompareResultEntry In Me
            If entry.CompareSignature = entry.ReferenceSignature AndAlso hashSigs.Contains(entry.CompareSignature) Then
                resultEntries.Add(entry)
            End If
        Next
        Return resultEntries
    End Function

    Public Function GetEntry(compRefSig As String) As CheckedCompareResultEntry
        Return FindCheckedCompareResultEntry(compRefSig, compRefSig)
    End Function

    Public Sub DeleteCheckedCompareResultEntry(checkedCompareResultEntry As CheckedCompareResultEntry)
        Me.Remove(checkedCompareResultEntry)
    End Sub

End Class