Imports VectorDraw.Geometry

Public Class BSpline

    Private _controlPoints As New gPoints
    Private _degree As Integer = 2
    Private _nodeVector() As Integer

    Public Sub New(ctrlPoints As gPoints, degree As Integer)
        _controlPoints = ctrlPoints
        _degree = degree

        CreateNodeVector()
    End Sub

    Public Function Evaluate(Optional intervall As Single = 0.1) As gPoints
        Dim points As New gPoints
        points.Add(_controlPoints(0))

        For i As Single = intervall To _nodeVector(_controlPoints.Count + _degree) Step intervall
            Dim pt As gPoint = Nothing

            For j As Integer = 0 To _controlPoints.Count - 1
                If (i >= j) Then pt = DeBoor(_degree, j, i)
            Next

            If (pt IsNot Nothing) Then points.Add(pt)
        Next

        points.Add(_controlPoints(_controlPoints.Count - 1))

        Return points
    End Function


    Private Sub CreateNodeVector()
        Dim knots As Integer = 0

        _nodeVector = New Integer(_controlPoints.Count + 1 + _degree) {}

        For i As Integer = 0 To _controlPoints.Count + 1 + _degree
            If (i > _degree) Then
                If (i <= _controlPoints.Count) Then knots += 1

                _nodeVector(i) = knots
            Else
                _nodeVector(i) = knots
            End If
        Next
    End Sub

    Private Function DeBoor(r As Integer, i As Integer, u As Single) As gPoint
        If (r = 0) Then
            Return _controlPoints(i)
        Else
            Try
                Dim pre As Single = ((u - _nodeVector((i + r))) / (_nodeVector((i + (_degree + 1))) - _nodeVector((i + r))))

                Return ((DeBoor((r - 1), i, u) * (1 - pre)) + (DeBoor((r - 1), (i + 1), u) * pre))
            Catch ex As Exception
                Return Nothing
            End Try
        End If
    End Function

End Class