Imports System.Text.RegularExpressions

Public Class CavitySorter
    Implements IComparer(Of ContactingViewer.BaseCavity)

    Private Shared ReadOnly _re As Regex = New Regex("(?<=\D)(?=\d)|(?<=\d)(?=\D)", RegexOptions.IgnoreCase)

    Public Function Compare(ByVal x As ContactingViewer.BaseCavity, ByVal y As ContactingViewer.BaseCavity) As Integer Implements IComparer(Of ContactingViewer.BaseCavity).Compare
        If x IsNot Nothing And y IsNot Nothing Then
            Dim l As String = String.Empty
            Dim m As String = String.Empty

            If x Is y Then 'So we avoid comparing same thing
                Return 0
            End If

            If Not String.IsNullOrEmpty(x.Name) Then
                l = CStr(x.Name)
            End If
            If Not String.IsNullOrEmpty(y.Name) Then
                m = CStr(y.Name)
            End If

            If String.Compare(l, 0, m, 0, Math.Min(l.Length, m.Length)) = 0 Then
                If l.Length = m.Length Then Return 0
                Return If(l.Length < m.Length, -1, 1)
            End If

            Dim a As String() = _re.Split(l) 'string array containg  the number(s) or string(s) or character(s) seperately.
            Dim b As String() = _re.Split(m)
            Dim i As Integer = 0
            Dim r As Integer

            If a.Length = 2 And b.Length = 2 Then ' Specially for Fakra
                Dim result As Integer
                If Regex.IsMatch(a(0), "^x", RegexOptions.IgnoreCase) And Regex.IsMatch(b(0), "^s", RegexOptions.IgnoreCase) Then
                    result = If(a(1) <= b(1), -1, 1) 'e.g x1 s1 , x1 s2 --> -1 and x2 s1 --> 1 
                    Return result
                ElseIf Regex.IsMatch(a(0), "^s", RegexOptions.IgnoreCase) And Regex.IsMatch(b(0), "^x", RegexOptions.IgnoreCase) Then
                    result = If(a(1) >= b(1), 1, -1) ' e.g s1 x1 ,s2 x1 and s2 x2 -->1 and s1 x2 --> -1 
                    Return result
                End If

            ElseIf a.Length = 1 And b.Length = 1 Then
                If Regex.IsMatch(a(0), "^x", RegexOptions.IgnoreCase) And Regex.IsMatch(b(0), "^s", RegexOptions.IgnoreCase) Then
                    Return -1
                ElseIf Regex.IsMatch(a(0), "^s", RegexOptions.IgnoreCase) And Regex.IsMatch(b(0), "^x", RegexOptions.IgnoreCase) Then
                    Return 1
                End If
            End If

            If a IsNot Nothing And b IsNot Nothing Then
                While True
                    If i < a.Length Then
                        r = PartCompare(a(i), b(i)) ' part to part is checked,and decide whether it is greater,equal or less
                        If r <> 0 Then Return r
                        i += 1
                    Else
                        Return 0
                    End If

                End While
            End If


            Return r
        Else
            Return 0
        End If

    End Function

    Private Shared Function PartCompare(ByVal x As String, ByVal y As String) As Integer
        Dim a, b As Integer
        If Integer.TryParse(x, a) AndAlso Integer.TryParse(y, b) Then Return a.CompareTo(b) ' less<0, equal 0, greater >0
        Return x.CompareTo(y)
    End Function


End Class

