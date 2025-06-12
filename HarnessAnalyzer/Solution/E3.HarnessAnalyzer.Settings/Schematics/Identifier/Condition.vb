Imports System.Drawing
Imports System.Runtime.Serialization
Imports System.Text.RegularExpressions
Imports Zuken.E3.HarnessAnalyzer.Shared.My.Resources

Namespace Schematics.Identifier

    <Serializable>
    Public Class Condition

        Public Sub New(conditionType As ConditionType)
            Me.Type = conditionType
        End Sub

        Public Sub New()
        End Sub

        <DataMember> Property Type As ConditionType

        Public Function IsMatch(inValue As String, valueToMatch As String) As MatchResult
            Select Case Type
                Case ConditionType.Contains
                    Return New MatchResult(inValue.Contains(valueToMatch))
                Case ConditionType.DoesNotContain
                    Return New MatchResult(Not inValue.Contains(valueToMatch))
                Case ConditionType.EndsWith
                    Return New MatchResult(inValue.EndsWith(valueToMatch))
                Case ConditionType.DoesNotEndWith
                    Return New MatchResult(Not inValue.EndsWith(valueToMatch))
                Case ConditionType.Equals
                    Return New MatchResult(inValue.Equals(valueToMatch))
                Case ConditionType.DoesNotEqual
                    Return New MatchResult(Not inValue.Equals(valueToMatch))
                Case ConditionType.Like
                    If inValue Like valueToMatch Then Return GetNewWildCardMatchResult(inValue, valueToMatch)
                    Return New MatchResult(False)
                Case ConditionType.NotLike
                    If Not inValue Like valueToMatch Then Return New MatchResult(True)
                    Return New MatchResult(False)
                Case ConditionType.StartsWith
                    Return New MatchResult(inValue.StartsWith(valueToMatch))
                Case ConditionType.DoesNotStartWith
                    Return New MatchResult(Not inValue.StartsWith(valueToMatch))
                Case ConditionType.MatchRegEx
                    Return New MatchResult(System.Text.RegularExpressions.Regex.Match(inValue, valueToMatch))
                Case ConditionType.DoesNotMatchRegEx
                    Dim match As Match = System.Text.RegularExpressions.Regex.Match(inValue, valueToMatch)
                    If Not match.Success Then
                        Return New MatchResult(True) With {.RegExMatch = match}
                    End If
                    Return New MatchResult(False)
                Case Else
                    Throw New NotImplementedException(String.Format("Unimplemented identifier type ""{0}""", Me.Type.ToString))
            End Select
        End Function

        Private Function GetNewWildCardMatchResult(inValue As String, valueToMatch As String) As MatchResult
            Dim wildCardRegEx As New Regex(WildCardToRegEx(valueToMatch))
            Return New MatchResult(wildCardRegEx.Match(inValue))
        End Function

        Private Function WildCardToRegEx(pattern As String) As String
            Return String.Format("^{0}$", System.Text.RegularExpressions.Regex.Escape(pattern).Replace("\*", ".*").Replace("\?", "."))
        End Function

#If NETFRAMEWORK Or WINDOWS Then
        ReadOnly Property Image As Bitmap
            Get
                Select Case Type
                    Case ConditionType.Contains
                        Return E3.HarnessAnalyzer.Shared.My.Resources.Contains
                    Case ConditionType.DoesNotContain
                        Return E3.HarnessAnalyzer.Shared.My.Resources.DoesNotContain
                    Case ConditionType.EndsWith
                        Return E3.HarnessAnalyzer.Shared.My.Resources.EndsWith
                    Case ConditionType.DoesNotEndWith
                        Return E3.HarnessAnalyzer.Shared.My.Resources.DoesNotEndWith
                    Case ConditionType.Equals
                        Return E3.HarnessAnalyzer.Shared.My.Resources._equals
                    Case ConditionType.DoesNotEqual
                        Return E3.HarnessAnalyzer.Shared.My.Resources.UnEquals
                    Case ConditionType.Like
                        Return E3.HarnessAnalyzer.Shared.My.Resources._Like
                    Case ConditionType.NotLike
                        Return E3.HarnessAnalyzer.Shared.My.Resources.NotLike
                    Case ConditionType.StartsWith
                        Return E3.HarnessAnalyzer.Shared.My.Resources.StartsWith
                    Case ConditionType.DoesNotStartWith
                        Return E3.HarnessAnalyzer.Shared.My.Resources.DoesNotStartWith
                    Case ConditionType.MatchRegEx
                        Return E3.HarnessAnalyzer.Shared.My.Resources.RegExMatch
                    Case ConditionType.DoesNotMatchRegEx
                        Return E3.HarnessAnalyzer.Shared.My.Resources.NoRegExMatch
                    Case Else
                        Throw New NotImplementedException(String.Format("Unimplemented identifier type ""{0}""", Me.Type.ToString))
                End Select
            End Get
        End Property
#End If

        Public Overrides Function ToString() As String
            Select Case Type
                Case ConditionType.Contains
                    Return ComponentIdentifierTypeStrings.Contains
                Case ConditionType.DoesNotContain
                    Return ComponentIdentifierTypeStrings.DoesNotContain
                Case ConditionType.EndsWith
                    Return ComponentIdentifierTypeStrings.EndsWith
                Case ConditionType.DoesNotEndWith
                    Return ComponentIdentifierTypeStrings.DoesNotEndWith
                Case ConditionType.Equals
                    Return ComponentIdentifierTypeStrings._Equals
                Case ConditionType.DoesNotEqual
                    Return ComponentIdentifierTypeStrings.DoesNotEqual
                Case ConditionType.Like
                    Return ComponentIdentifierTypeStrings._Like
                Case ConditionType.NotLike
                    Return ComponentIdentifierTypeStrings.NotLike
                Case ConditionType.StartsWith
                    Return ComponentIdentifierTypeStrings.StartsWith
                Case ConditionType.DoesNotStartWith
                    Return ComponentIdentifierTypeStrings.DoesNotStartWith
                Case ConditionType.MatchRegEx
                    Return ComponentIdentifierTypeStrings.MatchesRegEx
                Case ConditionType.DoesNotMatchRegEx
                    Return ComponentIdentifierTypeStrings.DoesNotMatchRegEx
                Case Else
                    Throw New NotImplementedException(String.Format("Unimplemented identifier type ""{0}""", Me.Type.ToString))
            End Select
        End Function


        Public Shared ReadOnly Property Contains As Condition
            Get
                Return New Condition With {.Type = ConditionType.Contains}
            End Get
        End Property

        Public Shared ReadOnly Property DoesNotContain As Condition
            Get
                Return New Condition With {.Type = ConditionType.DoesNotContain}
            End Get
        End Property

        Public Shared ReadOnly Property EndsWith As Condition
            Get
                Return New Condition With {.Type = ConditionType.EndsWith}
            End Get
        End Property

        Public Shared ReadOnly Property DoesNotEndWith As Condition
            Get
                Return New Condition With {.Type = ConditionType.DoesNotEndWith}
            End Get
        End Property

        Public Shared Shadows ReadOnly Property [Equals] As Condition
            Get
                Return New Condition With {.Type = ConditionType.Equals}
            End Get
        End Property

        Public Shared Shadows ReadOnly Property DoesNotEqual As Condition
            Get
                Return New Condition With {.Type = ConditionType.DoesNotEqual}
            End Get
        End Property

        Public Shared Shadows ReadOnly Property [Like] As Condition
            Get
                Return New Condition With {.Type = ConditionType.Like}
            End Get
        End Property

        Public Shared Shadows ReadOnly Property NotLike As Condition
            Get
                Return New Condition With {.Type = ConditionType.Like}
            End Get
        End Property

        Public Shared Shadows ReadOnly Property [StartsWith] As Condition
            Get
                Return New Condition With {.Type = ConditionType.StartsWith}
            End Get
        End Property

        Public Shared Shadows ReadOnly Property NotStartsWith As Condition
            Get
                Return New Condition With {.Type = ConditionType.StartsWith}
            End Get
        End Property

        Public Shared Shadows ReadOnly Property [RegEx] As Condition
            Get
                Return New Condition With {.Type = ConditionType.MatchRegEx}
            End Get
        End Property

        Public Shared Shadows ReadOnly Property DoesNotMatchRegEx As Condition
            Get
                Return New Condition With {.Type = ConditionType.DoesNotMatchRegEx}
            End Get
        End Property

    End Class

End Namespace