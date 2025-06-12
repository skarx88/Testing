<Serializable()>
Public Class CheckedCompareResultList
    Inherits System.Collections.ObjectModel.Collection(Of CheckedCompareResult)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function AddOrGet(compareDrawingName As String, referenceDrawingName As String) As CheckedCompareResult
        Dim result As CheckedCompareResult = FindCheckedCompareResult(compareDrawingName, referenceDrawingName)
        If result Is Nothing Then
            result = AddCheckedCompareResult(compareDrawingName, referenceDrawingName)
        Else
            If String.IsNullOrEmpty(result.CompareDrawingName) Then
                result.CompareDrawingName = compareDrawingName
            End If
            If String.IsNullOrEmpty(result.ReferenceDrawingName) Then
                result.ReferenceDrawingName = referenceDrawingName
            End If
        End If
        Return result
    End Function

    Public Function AddCheckedCompareResult(compareDrawingName As String, referenceDrawingName As String) As CheckedCompareResult
        Dim checkedCompareResult As CheckedCompareResult = Nothing

        If (Not ContainsCheckedCompareResult(compareDrawingName, referenceDrawingName)) Then
            checkedCompareResult = New CheckedCompareResult(compareDrawingName, referenceDrawingName)

            Me.Add(checkedCompareResult)
        End If

        Return checkedCompareResult
    End Function

    Public Function ContainsCheckedCompareResult(compareDrawingName As String, referenceDrawingName As String) As Boolean
        For Each checkedCompareResult As CheckedCompareResult In Me
            If (checkedCompareResult.CompareDrawingName.ToLower = compareDrawingName.ToLower) AndAlso (checkedCompareResult.ReferenceDrawingName.ToLower = referenceDrawingName.ToLower) Then
                Return True
            End If
        Next

        Return False
    End Function

    Public Function FindCheckedCompareResult(compareDrawingName As String, referenceDrawingName As String) As CheckedCompareResult
        For Each checkedCompareResult As CheckedCompareResult In Me
            If (String.IsNullOrEmpty(checkedCompareResult.CompareDrawingName) OrElse checkedCompareResult.CompareDrawingName.ToLower = compareDrawingName.ToLower) AndAlso (String.IsNullOrEmpty(checkedCompareResult.ReferenceDrawingName) OrElse checkedCompareResult.ReferenceDrawingName.ToLower = referenceDrawingName.ToLower) Then ' HINT: the empty check is a emergency fallback, when nothing was saved we are interpreting the empty name as an "ALL-Documents-Value"
                Return checkedCompareResult
            End If
        Next

        Return Nothing
    End Function

End Class