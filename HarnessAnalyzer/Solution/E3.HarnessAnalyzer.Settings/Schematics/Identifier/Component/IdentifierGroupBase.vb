Imports System.ComponentModel
Imports System.Runtime.Serialization
Imports Zuken.E3.HarnessAnalyzer.Shared.My.Resources

Namespace Schematics.Identifier.Component

    <Serializable>
    Public MustInherit Class IdentifierGroupBase
        Implements IDisposable
        Implements INotifyPropertyChanged

        Private WithEvents _connectorSuffixes As New SuffixCollection(Me)
        Private WithEvents _identifiers As New IdentifierCollection()

        Private _componentType As Integer
        Private _componentSuffix As String
        Private _isDefault As Boolean = False

        Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

        Protected Friend Sub New()
        End Sub

        Protected Sub New(componentSuffix As String)
            _componentSuffix = componentSuffix
        End Sub

        Protected Friend Sub New(componentTypeJT As Integer, Optional componentSuffix As String = "")
            _componentType = Me.ComponentType
            _componentSuffix = componentSuffix
        End Sub

        <DataMember>
        ReadOnly Property Identifiers As IdentifierCollection
            Get
                Return _identifiers
            End Get
        End Property

        <DataMember>
        Protected Friend Overridable Property ComponentType As Integer
            Get
                Return _componentType
            End Get
            Set(value As Integer)
                If _componentType <> value Then
                    If _componentType = GetInlinerComponetType() Then 'HINT: clear all identifier when changing from Type Inliner to other type
                        Me.ConnectorSuffixes.Clear()
                    End If

                    If value = GetEyeletComponetType() Or value = GetSpliceComponetType() Then
                        Me._componentSuffix = String.Empty
                    End If

                    _componentType = value

                    OnPropertyChanged(NameOf(ComponentType))
                End If
            End Set
        End Property

        Protected Friend MustOverride Function GetSpliceComponetType() As Integer
        Protected Friend MustOverride Function GetEyeletComponetType() As Integer
        Protected Friend MustOverride Function GetInlinerComponetType() As Integer
        Protected Friend MustOverride Function GetEcuComponetType() As Integer
        Protected Friend MustOverride Function GetSvgComponetType() As Integer
        Protected Friend MustOverride Function GetUndefinedComponetType() As Integer

        Friend ReadOnly Property IsInlinerType As Boolean
            Get
                Return Me.ComponentType = Me.GetInlinerComponetType
            End Get
        End Property

        Friend ReadOnly Property IsEcuType As Boolean
            Get
                Return Me.ComponentType = Me.GetEcuComponetType
            End Get
        End Property

        Friend ReadOnly Property IsEyeletType As Boolean
            Get
                Return Me.ComponentType = Me.GetEyeletComponetType
            End Get
        End Property

        Friend ReadOnly Property IsSpliceType As Boolean
            Get
                Return Me.ComponentType = Me.GetSpliceComponetType
            End Get
        End Property

        Friend ReadOnly Property IsSvgType As Boolean
            Get
                Return Me.ComponentType = Me.GetSvgComponetType
            End Get
        End Property

        Friend ReadOnly Property IsUndefinedType As Boolean
            Get
                Return Me.ComponentType = Me.GetUndefinedComponetType
            End Get
        End Property

        <DataMember>
        Property IsDefault As Boolean
            Get
                Return _isDefault
            End Get
            Set(value As Boolean)
                If _isDefault <> value Then
                    _isDefault = value
                    If _isDefault Then
                        Me.Identifiers.Clear()
                    End If
                    Me.Identifiers.AllowNew = Not _isDefault
                    OnPropertyChanged(NameOf(IsDefault))
                End If
            End Set
        End Property

        <DataMember>
        Property ComponentSuffix As String ' and when this componentName ends
            Get
                Return _componentSuffix
            End Get
            Set(value As String)
                If _componentSuffix <> value Then
                    _componentSuffix = value
                    OnPropertyChanged(NameOf(ComponentSuffix))
                End If
            End Set
        End Property

        <DataMember>
        <Browsable(False)>
        ReadOnly Property ConnectorSuffixes As SuffixCollection
            Get
                Return _connectorSuffixes
            End Get
        End Property

        ReadOnly Property ConnectorSuffixesValue As String
            Get
                Return ConnectorSuffixes.ToString
            End Get
        End Property

        Public Function TryGetIdentifierMatchResults(inValue As String) As IdentifierMatchResult()
            Dim results As New List(Of IdentifierMatchResult)
            If Me.Identifiers.Count > 0 Then
                For Each ident As Identifier In Me.Identifiers
                    Dim result As MatchResult = ident.IsMatch(inValue)
                    results.Add(New IdentifierMatchResult(result, ident))
                Next
            End If
            Return results.ToArray
        End Function

        Protected Overridable Sub OnPropertyChanged(e As PropertyChangedEventArgs)
            RaiseEvent PropertyChanged(Me, e)
        End Sub

        Private Sub OnPropertyChanged(propertyName As String)
            OnPropertyChanged(New PropertyChangedEventArgs(propertyName))
        End Sub

        Private Sub _connectorSuffixes_CollectionChanged(sender As Object, e As System.Collections.Specialized.NotifyCollectionChangedEventArgs) Handles _connectorSuffixes.CollectionChanged
            OnPropertyChanged(NameOf(ConnectorSuffixesValue))
        End Sub

#If NETFRAMEWORK Or WINDOWS Then
        Protected Friend Shared Function GetComponentTypeString(componentGroup As IdentifierGroupBase) As String
            If componentGroup.IsInlinerType Then
                Return ComponentTypeStrings.Inliner
            ElseIf componentGroup.IsEyeletType Then
                Return ComponentTypeStrings.Eyelet
            ElseIf componentGroup.IsEcuType Then
                Return ComponentTypeStrings.Ecu
            ElseIf componentGroup.IsSpliceType Then
                Return ComponentTypeStrings.Splice
            ElseIf componentGroup.IsSvgType Then
                Return ComponentTypeStrings.SVG
            ElseIf componentGroup.IsUndefinedType Then
                Return ComponentTypeStrings.Undefined
            Else
                Throw New NotImplementedException(String.Format("ComponentType ""{0}"" not implemented", componentGroup.ComponentType))
            End If
        End Function

        Public Overrides Function ToString() As String
            Return GetComponentTypeString(Me)
        End Function
#End If

    End Class

End Namespace
