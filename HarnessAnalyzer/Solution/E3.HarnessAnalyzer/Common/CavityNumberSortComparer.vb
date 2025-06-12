Public Class CavityNumberSortComparer
    Implements IComparer(Of String)

    Public Sub New()
    End Sub

    Public Function Compare(xCavityNumber As String, yCavityNumber As String) As Integer Implements IComparer(Of String).Compare
        If (Not String.IsNullOrWhiteSpace(xCavityNumber)) AndAlso (Not String.IsNullOrWhiteSpace(yCavityNumber)) Then
            If (IsNumeric(xCavityNumber)) Then
                If (IsNumeric(yCavityNumber)) Then
                    Return Comparer.Default.Compare(CInt(xCavityNumber), CInt(yCavityNumber))
                Else
                    Return -1
                End If
            Else
                If (IsNumeric(yCavityNumber)) Then
                    Return 1
                Else
                    Return Comparer.Default.Compare(xCavityNumber, yCavityNumber)
                End If
            End If
        Else
            Return -1
        End If
    End Function
End Class
