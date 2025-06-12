
Public Class NumericStringFilterComparer
    Implements IComparer

    Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare
        Dim value1 As String = String.Empty
        Dim value2 As String = String.Empty

        If (TypeOf x Is String) Then
            If (x.ToString <> String.Empty) Then
                value1 = x.ToString.SplitSpace(0)
            End If
        End If

        If (TypeOf y Is String) Then
            If (y.ToString <> String.Empty) Then
                value2 = y.ToString.SplitSpace(0)
            End If
        End If

        Return Compare(value1, value2)
    End Function

    Private Function Compare(ByVal x As String, ByVal y As String) As Integer
        Dim isNumX As Boolean = IsNumeric(x)
        Dim isNumY As Boolean = IsNumeric(y)

        If (isNumX AndAlso isNumY) Then
            Return CSng(x).CompareTo(CSng(y))
        End If
        If (isNumX AndAlso Not isNumY) Then
            Return -1
        End If
        If (Not isNumX AndAlso isNumY) Then
            Return 1
        End If

        Return [String].Compare(x.ToString, y.ToString, True)
    End Function
End Class