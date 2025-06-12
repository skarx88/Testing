Imports System.ComponentModel
Imports System.Text.RegularExpressions

<Serializable()>
Public Class InlinerIdentifier
    Inherits IdentifierBase

    Private _pairId1 As String
    Private _pairId2 As String

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(kblPropName As String, criteria As String)
        MyBase.New(kblPropName, criteria)
    End Sub

    Protected Overrides Sub Buildregex(expr As String)

        Dim criteriaRegEx As New Regex("(?'beginning'.*){(?'pair1'.+),(?'pair2'.+)}(?'ending'.*)")
        Dim m As Match = criteriaRegEx.Match(expr)
        Dim beginnig As String = String.Empty
        Dim ending As String = String.Empty
        Dim finalExpression As String = String.Empty

        _pairId1 = String.Empty
        _pairId2 = String.Empty

        If m IsNot Nothing AndAlso m.Success Then
            For Each g As Group In m.Groups
                Select Case g.Name
                    Case "beginning"
                        beginnig = g.Value
                    Case "pair1"
                        _pairId1 = g.Value
                    Case "pair2"
                        _pairId2 = g.Value
                    Case "ending"
                        ending = g.Value
                End Select
            Next

            Dim expression As New System.Text.StringBuilder
            expression.Append("^")

            If String.IsNullOrEmpty(beginnig) Then
                expression.Append(String.Format("(?'pair'{0}|{1})", GetEscapedExpression(_pairId1, True), GetEscapedExpression(_pairId2, True)))
                expression.Append(String.Format("(?'connector'{0})", GetEscapedExpression(ending, False)))
            Else
                expression.Append(String.Format("(?'connector'{0})", GetEscapedExpression(beginnig, False)))
                expression.Append(String.Format("(?={0}|{1})", GetEscapedExpression(_pairId1, True), GetEscapedExpression(_pairId2, True)))
                expression.Append(String.Format("(?'pair'{0}|{1})", GetEscapedExpression(_pairId1, True), GetEscapedExpression(_pairId2, True)))
                expression.Append(GetEscapedExpression(ending, False))
            End If
            finalExpression = expression.ToString

        Else
            finalExpression = GetEscapedExpression(ending, False)
        End If


        _regEx = New Regex(finalExpression)

    End Sub

    <Browsable(False)>
    Public ReadOnly Property PairIdentifiers As Tuple(Of String, String)
        Get
            Return New Tuple(Of String, String)(_pairId1, _pairId2)
        End Get
    End Property

    Public Function GetConnectorRecognizer(value As String) As String
        Return GetMatchValue(value, "connector")
    End Function

    Public Function GetPairRecognizer(value As String) As String
        Return GetMatchValue(value, "pair")
    End Function

End Class


