Imports System.Runtime.Serialization

Namespace Schematics.Identifier.Component

    <Serializable>
    Public Class Identifier

        Public Sub New()
        End Sub

        Public Sub New(type As Condition, value As String)
            Me.Condition = type
            Me.ConditionValue = value
        End Sub

        Public Sub New(typeDef As ConditionType, value As String)
            Me.New(New Condition(typeDef), value)
        End Sub

        <DataMember> Property Condition As New Condition
        <DataMember> Property ConditionValue As String

        Public Function IsMatch(inValue As String) As MatchResult
            If Not String.IsNullOrEmpty(inValue) AndAlso Not String.IsNullOrEmpty(ConditionValue) Then
                Return Me.Condition.IsMatch(inValue, ConditionValue)
            End If
            Return New MatchResult(False)
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("[{0}] [{1}] [{2}]", Condition.ToString, ConditionValue.ToString)
        End Function

    End Class

End Namespace