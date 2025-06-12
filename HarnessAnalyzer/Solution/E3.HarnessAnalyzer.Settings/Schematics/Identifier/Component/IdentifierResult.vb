Imports System.Collections.ObjectModel
Namespace Schematics.Identifier.Component

    Public Class IdentifierResult

        Public Sub New(connectorShortName As String, match As IdentifierMatch)
            Me.ComponentName = connectorShortName
            Me.Connector = New ConnectorInfo(connectorShortName)
            Me.ComponentType = match.Group.ComponentType
            If match.Identifier IsNot Nothing Then
                With match.Identifier
                    Select Case .Condition.Type
                        Case ConditionType.Contains, ConditionType.StartsWith, ConditionType.EndsWith
                            'TODO: this does not work if other than StartsWith is used!!!! needs still to be fixed GEM-1525 
                            Dim idx As Integer = connectorShortName.IndexOf(.ConditionValue)

                            If idx > -1 AndAlso idx < connectorShortName.Length Then
                                Dim afterIdentifierStr As String = connectorShortName.Substring(idx)
                                Dim suffixIdx As Integer = If(Not String.IsNullOrEmpty(match.Group.ComponentSuffix), GetSuffixIndex(afterIdentifierStr, match.Group.ComponentSuffix, idx), afterIdentifierStr.Length)
                                Me.Connector = GetConnectorInfo(afterIdentifierStr, suffixIdx, match)
                                If suffixIdx > 0 Then
                                    Me.ComponentName = afterIdentifierStr.Substring(0, suffixIdx)
                                ElseIf Me.Connector.Inliners IsNot Nothing AndAlso Me.Connector.Inliners.Count > 0 Then
                                    Me.ComponentName = Me.Connector.Inliners.First.Name
                                End If
                            End If
                        Case ConditionType.DoesNotContain, ConditionType.DoesNotEndWith, ConditionType.DoesNotStartWith, ConditionType.DoesNotMatchRegEx, ConditionType.Equals, ConditionType.DoesNotEqual, ConditionType.NotLike
                            Dim suffixIdx As Integer = If(Not String.IsNullOrEmpty(match.Group.ComponentSuffix), connectorShortName.IndexOf(match.Group.ComponentSuffix), connectorShortName.Length)
                            Me.Connector = GetConnectorInfo(connectorShortName, suffixIdx, match)
                            If suffixIdx > -1 Then
                                Me.ComponentName = connectorShortName.Substring(0, suffixIdx)
                            ElseIf Me.Connector.Inliners IsNot Nothing AndAlso Me.Connector.Inliners.Count > 0 Then
                                Me.ComponentName = Me.Connector.Inliners.First.Name
                            End If
                        Case ConditionType.Like, ConditionType.MatchRegEx
                            Dim suffixIdx As Integer = If(Not String.IsNullOrEmpty(match.Group.ComponentSuffix), connectorShortName.IndexOf(match.Group.ComponentSuffix), connectorShortName.Length)
                            Me.Connector = GetConnectorInfo(connectorShortName, suffixIdx, match)
                            If suffixIdx > 0 Then
                                Me.ComponentName = connectorShortName.Substring(0, suffixIdx)
                            ElseIf Me.Connector.Inliners IsNot Nothing AndAlso Me.Connector.Inliners.Count > 0 Then
                                Me.ComponentName = Me.Connector.Inliners.First.Name
                            End If
                        Case Else
                            Throw New NotImplementedException(String.Format("Unimplemented Identifier-Type: ""{0}"" detected!", .Condition.Type))
                    End Select
                End With
            ElseIf match.Group.IsDefault Then
                Dim idx As Integer = If(Not String.IsNullOrEmpty(match.Group.ComponentSuffix), connectorShortName.IndexOf(match.Group.ComponentSuffix), -1)
                Me.Connector = GetConnectorInfo(connectorShortName, idx, match)
                If idx > 0 Then
                    Me.ComponentName = connectorShortName.Substring(0, idx)
                ElseIf Me.Connector.Inliners IsNot Nothing AndAlso Me.Connector.Inliners.Count > 0 Then
                    Me.ComponentName = Me.Connector.Inliners.First.Name
                End If
            End If

            If String.IsNullOrEmpty(match.Group.ComponentSuffix) AndAlso Connector.Inliners IsNot Nothing AndAlso Connector.Inliners.Count > 0 Then
                Me.ComponentName = Me.Connector.Inliners.First.Name
            End If
        End Sub

        Private Function GetSuffixIndex(str As String, suffix As String, Optional startAtIndex As Nullable(Of Integer) = Nothing) As Integer
            'TODO this is to be reworked as we are talking about suffixes but use the first hit with index of? 
            If Not String.IsNullOrEmpty(suffix) Then
                If startAtIndex.HasValue Then
                    If (startAtIndex.Value < str.Length) Then
                        Return str.IndexOf(suffix, startAtIndex.Value)
                    Else
                        Return 0
                    End If
                Else
                    Return str.IndexOf(suffix)
                End If
            End If
            Return 0
        End Function

        Private Function GetConnectorInfo(str As String, suffixIndex As Integer, match As IdentifierMatch) As ConnectorInfo
            Return New ConnectorInfo(If(suffixIndex > -1 AndAlso suffixIndex < str.Length, str.Substring(suffixIndex + 1), str), match)
        End Function

        Property ComponentType As Integer
        Property ComponentName As String
        Property Connector As ConnectorInfo

        Public Class ConnectorInfo
            Private _shortName As String
            Private _inliners As ReadOnlyCollection(Of InlinerInfo)
            Private _isInliner As Boolean

            Public Sub New(connectorShortName As String)
                _shortName = connectorShortName
            End Sub

            Public Sub New(connectorShortName As String, match As IdentifierMatch)
                Me.New(connectorShortName)
                _isInliner = match.Group.IsInlinerType

                If _isInliner Then
                    Dim inlinersList As New List(Of InlinerInfo)
                    For Each suffixMatch As SuffixMatch In match.Group.ConnectorSuffixes.GetMatches(connectorShortName)
                        Dim inlinerNumber As String = connectorShortName.Substring(0, suffixMatch.Index)
                        Dim suffix As String = connectorShortName.Substring(suffixMatch.Index)
                        inlinersList.Add(New InlinerInfo(inlinerNumber, suffix))
                    Next
                    _inliners = New ObjectModel.ReadOnlyCollection(Of InlinerInfo)(inlinersList)
                End If
            End Sub

            ReadOnly Property ShortName As String
                Get
                    Return _shortName
                End Get
            End Property

            ReadOnly Property IsInliner As Boolean
                Get
                    Return _isInliner
                End Get
            End Property

            ReadOnly Property Inliners As ReadOnlyCollection(Of InlinerInfo)
                Get
                    Return _inliners
                End Get
            End Property

            Public Overrides Function ToString() As String
                Return String.Format("{0}, Inliner: {1}", Me.ShortName, Me.IsInliner)
            End Function

            Public Class InlinerInfo

                Private _name As String
                Private _suffix As String

                Public Sub New(name As String, suffix As String)
                    _name = name
                    _suffix = suffix
                End Sub

                ReadOnly Property Name As String
                    Get
                        Return _name
                    End Get
                End Property

                ReadOnly Property Suffix As String
                    Get
                        Return _suffix
                    End Get
                End Property

                Public Overrides Function ToString() As String
                    Return String.Format("{0}:{1}", Me.Name, Me.Suffix)
                End Function

            End Class

        End Class

    End Class

End Namespace