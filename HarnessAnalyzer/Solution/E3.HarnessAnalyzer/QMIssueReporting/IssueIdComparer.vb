Imports System.Text.RegularExpressions
Imports Infragistics.Win.UltraWinGrid

Namespace IssueReporting
    Public Class IssueIdComparer
        Implements IComparer
        Implements IDisposable

        Private _table As New Dictionary(Of String, String())

        'HINT: Natural compare implementation (f.e.: 23,23a,23b,24,usw.)
        Public Function Compare(x As Object, y As Object) As Integer Implements IComparer.Compare
            Dim CellX As UltraGridCell = DirectCast(x, UltraGridCell)
            Dim CellY As UltraGridCell = DirectCast(y, UltraGridCell)

            If (CellX.Value IsNot Nothing AndAlso CellX.Value.Equals(CellY.Value)) OrElse (CellX.Value Is Nothing AndAlso CellY.Value Is Nothing) Then
                Return 0
            End If
            Dim x1 As String() = Nothing
            Dim y1 As String() = Nothing

            If Not _table.TryGetValue(CellX.Value.ToString, x1) Then
                x1 = Regex.Split(CellX.Value.ToString.Replace(" ", String.Empty), "([0-9]+)")
                _table.Add(CellX.Value.ToString, x1)
            End If
            If Not _table.TryGetValue(CellY.Value.ToString, y1) Then
                y1 = Regex.Split(CellY.Value.ToString.Replace(" ", String.Empty), "([0-9]+)")
                _table.Add(CellY.Value.ToString, y1)
            End If

            Dim i As Integer = 0
            While i < x1.Length AndAlso i < y1.Length
                If x1(i) <> y1(i) Then
                    Return PartCompare(x1(i), y1(i))
                End If
                i += 1
            End While
            If y1.Length > x1.Length Then
                Return 1
            ElseIf x1.Length > y1.Length Then
                Return -1
            Else
                Return 0
            End If
        End Function

        Private Shared Function PartCompare(left As String, right As String) As Integer
            Dim x As Integer, y As Integer
            If Not Integer.TryParse(left, x) Then
                Return left.CompareTo(right)
            End If

            If Not Integer.TryParse(right, y) Then
                Return left.CompareTo(right)
            End If

            Return x.CompareTo(y)
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' So ermitteln Sie überflüssige Aufrufe

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then

                End If

                _table.Clear()
                _table = Nothing
            End If
            Me.disposedValue = True
        End Sub

        ' Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Ändern Sie diesen Code nicht. Fügen Sie oben in Dispose(disposing As Boolean) Bereinigungscode ein.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace