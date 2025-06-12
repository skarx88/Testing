Imports System.ComponentModel

Namespace Schematics.Identifier.Component

    <Serializable>
    Public MustInherit Class IdentifierGroupCollectionBase
        Inherits BindingList(Of IdentifierGroupBase)

        Private _default As IdentifierGroupBase = Nothing
        Private _isDefaultChanging As Boolean = False

        Protected Sub New()
            MyBase.New()
        End Sub

        Protected Sub New(list As IList(Of IdentifierGroupBase))
            MyBase.New(list)
        End Sub

        Public Sub CreateDefault()
            TryAddSpliceDefault()
            TryAddEyeletDefault()
            TryAddInlinerDefault()
            TryAddEcuDefault()
        End Sub

        Public Sub AddMissingDefaultEntries()
            If Not Me.Any(Function(grp) grp.IsSpliceType) Then
                TryAddSpliceDefault()
            End If

            If Not Me.Any(Function(grp) grp.IsEyeletType) Then
                TryAddEyeletDefault()
            End If

            If Not Me.Any(Function(grp) grp.IsInlinerType) Then
                TryAddInlinerDefault()
            End If

            If Not Me.Any(Function(grp) grp.IsEcuType) Then
                TryAddEcuDefault()
            End If
        End Sub

        Friend Overloads Function CreateNewInstance() As IdentifierGroupBase
            Dim newGroup As IdentifierGroupBase = CreateNewInstance(String.Empty)
            Return newGroup
        End Function

        Protected MustOverride Overloads Function CreateNewInstance(componentSuffix As String) As IdentifierGroupBase

        Protected Overridable Function TryAddSpliceDefault() As Boolean
            Dim newGroup As IdentifierGroupBase = CreateNewInstance()
            If newGroup IsNot Nothing Then
                Dim spliceCompType As Integer = newGroup.GetSpliceComponetType
                newGroup.ComponentType = spliceCompType
                newGroup.Identifiers.AddNew(ConditionType.StartsWith, "Z")
                Me.Add(newGroup)
                Return True
            End If
            Return False
        End Function

        Private Function TryAddEyeletDefault() As Boolean
            Dim newGroup As IdentifierGroupBase = CreateNewInstance()
            If newGroup IsNot Nothing Then
                Dim eyeletCompType As Integer = newGroup.GetEyeletComponetType
                newGroup.ComponentType = eyeletCompType
                newGroup.Identifiers.AddNew(ConditionType.StartsWith, "W")
                Me.Add(newGroup)
                Return True
            End If
            Return False
        End Function

        Private Function TryAddInlinerDefault() As Boolean
            Dim newGroup As IdentifierGroupBase = CreateNewInstance("*")
            If newGroup IsNot Nothing Then
                Dim inlinerCompType As Integer = newGroup.GetInlinerComponetType
                newGroup.ComponentType = inlinerCompType
                newGroup.ConnectorSuffixes.Add("-B")
                newGroup.ConnectorSuffixes.Add("-S")
                newGroup.Identifiers.AddNew(ConditionType.StartsWith, "X")
                Me.Add(newGroup)
                Return True
            End If
            Return False
        End Function

        Private Function TryAddEcuDefault() As Boolean
            Dim newGroup As IdentifierGroupBase = CreateNewInstance("*")
            If newGroup IsNot Nothing Then
                Dim ecuCompType As Integer = newGroup.GetEcuComponetType
                newGroup.ComponentType = ecuCompType
                newGroup.IsDefault = True
                Me.Add(newGroup)
                Return True
            End If
            Return False
        End Function

        ReadOnly Property DefaultGroup As IdentifierGroupBase
            Get
                Return _default
            End Get
        End Property

        Protected Overrides Sub InsertItem(index As Integer, item As IdentifierGroupBase)
            MyBase.InsertItem(index, item)
            If item.IsDefault Then
                SetDefault(item)
            End If
            AddGroupHandler(item)
        End Sub

        Protected Overrides Sub RemoveItem(index As Integer)
            Dim item As IdentifierGroupBase = Me(index)
            RemoveGroupHandler(item)
            MyBase.RemoveItem(index)
            If _default Is item Then
                _default = Nothing
            End If
        End Sub

        Protected Overrides Sub ClearItems()
            For Each item As IdentifierGroupBase In Me.ToArray
                Me.Remove(item)
            Next
            MyBase.ClearItems() ' HINT: call just for the BL orthogonalization but it shoudn't do nothing here because the collection shoud be already empty
            _default = Nothing
        End Sub

        Public Function TryGetMatches(value As String) As IdentifierMatchCollection
            Dim matchResults As New List(Of IdentifierMatch)
            For Each idGrop As IdentifierGroupBase In Me
                Dim componentName As String = Nothing
                Dim results As IdentifierMatchResult() = idGrop.TryGetIdentifierMatchResults(value)
                If results.Length > 0 AndAlso results.All(Function(rst) rst.Success) Then
                    matchResults.Add(New IdentifierMatch(idGrop, results.First.Identifier, results.First.RegExMatch))
                End If
            Next
            Return New IdentifierMatchCollection(matchResults)
        End Function

        Private Sub AddGroupHandler(grp As IdentifierGroupBase)
            AddHandler grp.PropertyChanged, AddressOf _group_propertyChanged
        End Sub

        Private Sub RemoveGroupHandler(grp As IdentifierGroupBase)
            RemoveHandler grp.PropertyChanged, AddressOf _group_propertyChanged
        End Sub

        Private Sub _group_propertyChanged(sender As Object, e As PropertyChangedEventArgs)
            Dim grp As IdentifierGroupBase = DirectCast(sender, IdentifierGroupBase)
            If e.PropertyName = NameOf(IdentifierGroupBase.IsDefault) AndAlso grp.IsDefault Then
                SetDefault(grp)
            End If
        End Sub

        Private Sub SetDefault(grp As IdentifierGroupBase)
            If Not _isDefaultChanging Then
                _isDefaultChanging = True
                _default = grp
                For Each otherGrp As Component.IdentifierGroupBase In Me
                    If Not otherGrp Is grp Then
                        otherGrp.IsDefault = False
                    End If
                Next
                _isDefaultChanging = False
            End If
        End Sub

    End Class

End Namespace

