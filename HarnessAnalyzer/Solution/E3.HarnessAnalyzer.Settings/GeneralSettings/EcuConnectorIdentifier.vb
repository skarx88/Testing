Imports System.Text.RegularExpressions
Imports Zuken.E3.Lib.Schema.Kbl.Properties

<Serializable()>
Public Class EcuConnectorIdentifier
    Inherits IdentifierBase
    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(kblPropName As String, criteria As String)
        MyBase.New(kblPropName, criteria)
    End Sub

    Protected Overrides Sub Buildregex(expr As String)
        _regEx = Nothing
        If Not String.IsNullOrEmpty(expr) Then
            _regEx = New Regex(String.Format("^(?'component'.*)(?={0})(?'separator'{0})(?'connector'.*)", GetEscapedExpression(expr, True)))
        End If
    End Sub

    Public Function GetConnectorRecognizer(value As String) As String
        Return GetMatchValue(value, "connector")
    End Function

    Public Function GetComponentRecognizer(value As String) As String
        Return GetMatchValue(value, "component")
    End Function

    Public Shared Function CreateDefaultIdentifier() As EcuConnectorIdentifier
        Return New EcuConnectorIdentifier(ConnectorPropertyName.Id.ToString, "*")
    End Function

End Class
