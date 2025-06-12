
Imports System.Text.RegularExpressions
Imports Zuken.E3.HarnessAnalyzer.ContactingViewer

Public Class InsertSorter
    Implements IComparer(Of Insert)
    Private Shared ReadOnly _re As Regex = New Regex("(?<=\D)(?=\d)|(?<=\d)(?=\D)", RegexOptions.IgnoreCase)

    Public Function Compare(ByVal x As Insert, ByVal y As Insert) As Integer Implements IComparer(Of Insert).Compare
        If x IsNot Nothing And y IsNot Nothing Then
            Dim l As String
            Dim m As String
            Dim p As New List(Of String)
            Dim q As New List(Of String)

            If x Is y Then 'So we avoid comparing same thing
                Return 0
            End If

            If x.Cavities.Count > 0 Then
                x.Cavities.Sort(New CavitySorter)
            End If

            If y.Cavities.Count > 0 Then
                y.Cavities.Sort(New CavitySorter)
            End If


            For Each cav As ContactingViewer.BaseCavity In x.Cavities
                p.Add(cav.Name)
            Next

            For Each cav As ContactingViewer.BaseCavity In y.Cavities
                q.Add(cav.Name)
            Next


            If p.Count = 1 And q.Count = 1 Then
                l = p.FirstOrDefault
                m = q.FirstOrDefault
            Else
                Dim alphanum1 As String = String.Empty
                Dim alphanum2 As String = String.Empty
                Dim num1 As Double
                Dim num2 As Double

                For Each value As String In p
                    If IsNumeric(p) Then
                        num1 += CDbl(value)
                    Else
                        alphanum1 += value
                    End If
                Next
                l = CStr(num1) + alphanum1
                For Each value As String In q
                    If IsNumeric(p) Then
                        num2 += CDbl(value)
                    Else
                        alphanum2 += value
                    End If
                Next
                m = CStr(num2) + alphanum2
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
        Dim p As String = x.ToLower
        Dim q As String = y.ToLower

        If Integer.TryParse(p, a) AndAlso Integer.TryParse(q, b) Then Return a.CompareTo(b) ' less<0, equal 0, greater >0
        Return p.CompareTo(q)
    End Function


End Class
