Imports System.Text.RegularExpressions

Public MustInherit Class IdentifierBase
    Protected _kblPropertyName As String
    Protected _identificationCriteria As String
    Protected _regEx As Regex = Nothing

    Public Sub New()
        _kblPropertyName = String.Empty
        _identificationCriteria = String.Empty
    End Sub

    Public Sub New(kblPropName As String, criteria As String)
        _kblPropertyName = kblPropName
        _identificationCriteria = criteria
        SetRegex(_identificationCriteria)
    End Sub

    Public Property KBLPropertyName As String
        Get
            Return _kblPropertyName
        End Get
        Set(value As String)
            _kblPropertyName = value
        End Set
    End Property

    Public Property IdentificationCriteria As String
        Get
            Return _identificationCriteria
        End Get
        Set(value As String)
            _identificationCriteria = value
            SetRegex(_identificationCriteria)
        End Set
    End Property

    Public Function IsMatch(value As String) As Boolean
        If _regEx IsNot Nothing Then
            Dim m As Match = _regEx.Match(value)
            Return CBool(m IsNot Nothing AndAlso m.Success)
        End If
        Return False
    End Function

    Private Sub SetRegex(expr As String)
        If expr.StartsWith("Regex:") Then
            Try
                _regEx = New Regex(expr.Substring("Regex:".Length))
            Catch ex As Exception
                _regEx = Nothing
            End Try

        Else
            BuildRegex(expr)
        End If
    End Sub

    Protected Function GetMatchValue(value As String, captureGroup As String) As String
        If _regEx IsNot Nothing Then
            Dim m As Match = _regEx.Match(value)
            If m IsNot Nothing AndAlso m.Success Then
                For Each g As Group In m.Groups
                    If g.Name = captureGroup Then
                        Return g.Value
                    End If
                Next
            End If
        End If
        Return String.Empty
    End Function

    Protected Function GetEscapedExpression(exp As String, escapeAsterix As Boolean) As String
        Dim escapedExpr As String = exp
        For Each c As Char In "\.^$*+?()|".ToCharArray
            If c = "*"c AndAlso Not escapeAsterix Then
                escapedExpr = escapedExpr.Replace(c, String.Format(".{0}", c))
            Else
                escapedExpr = escapedExpr.Replace(c, String.Format("\{0}", c))
            End If
        Next
        Return escapedExpr
    End Function

    Protected Overridable Sub BuildRegex(expr As String)
        _regEx = New Regex(String.Format("^{0}", GetEscapedExpression(expr, False)))
    End Sub

End Class
