Imports System.Reflection
Imports Zuken.E3.HarnessAnalyzer.Settings

Public MustInherit Class ComparisonMapper
    Protected _currentActiveObjects As ICollection(Of String)
    Protected _compareActiveObjects As ICollection(Of String)

    Protected _currentContainer As IKblContainer
    Protected _compareContainer As IKblContainer

    Protected _currentKBLMapper As KBLMapper
    Protected _compareKBLMapper As KBLMapper

    Protected _currentObject As Object
    Protected _compareObject As Object

    Protected _currentOccurrenceMapper As IDictionary(Of String, IKblOccurrence)
    Protected _compareOccurrenceMapper As IDictionary(Of String, IKblOccurrence)

    Protected _listConvertToDictionary As ListConvertToDictionary

    Private _owner As ComparisonMapper
    Private _generalSettings As GeneralSettingsBase

    Public MustOverride Sub CompareObjects()

    Public Sub New(activeContainer As KblMapper, compareContainer As KblMapper, currentActiveObjects As ICollection(Of String), compareActiveObjects As ICollection(Of String), settings As GeneralSettingsBase)
        Me.New(Nothing, activeContainer, compareContainer, currentActiveObjects, compareActiveObjects, settings)
    End Sub

    Protected Sub New(owner As ComparisonMapper, listConvertToDictionary As ListConvertToDictionary, settings As GeneralSettingsBase)
        Me.New(owner, listConvertToDictionary, Nothing, Nothing, settings)
    End Sub

    Protected Sub New(owner As ComparisonMapper, listConvertToDictionary As ListConvertToDictionary, currentActiveObjects As ICollection(Of String), compareActiveObjects As ICollection(Of String), settings As GeneralSettingsBase)
        Me.New(owner, Nothing, Nothing, Nothing, Nothing, currentActiveObjects, compareActiveObjects, listConvertToDictionary, settings)
    End Sub

    Protected Sub New(owner As ComparisonMapper, currentObject As Object, compareObject As Object, currentKBLMapper As KblMapper, compareKBLMapper As KblMapper, settings As GeneralSettingsBase)
        Me.New(owner, currentObject, compareObject, currentKBLMapper, compareKBLMapper, Nothing, Nothing, Nothing, settings)
    End Sub

    Protected Sub New(owner As ComparisonMapper, currentKBLMapper As KblMapper, compareKBLMapper As KblMapper, currentActiveObjects As ICollection(Of String), compareActiveObjects As ICollection(Of String), settings As GeneralSettingsBase)
        Me.New(owner, Nothing, Nothing, currentKBLMapper, compareKBLMapper, currentActiveObjects, compareActiveObjects, Nothing, settings)
    End Sub

    Protected Sub New(owner As ComparisonMapper, currentKBLMapper As KblMapper, compareKBLMapper As KblMapper, currentActiveObjects As ICollection(Of String), compareActiveObjects As ICollection(Of String), listConvertToDictionary As ListConvertToDictionary, settings As GeneralSettingsBase)
        Me.New(owner, Nothing, Nothing, currentKBLMapper, compareKBLMapper, currentActiveObjects, compareActiveObjects, listConvertToDictionary, settings)
    End Sub

    Private Sub New(owner As ComparisonMapper, currentObject As Object, compareObject As Object, currentKBLMapper As KblMapper, compareKBLMapper As KblMapper, currentActiveObjects As ICollection(Of String), compareActiveObjects As ICollection(Of String), listConvertToDictionary As ListConvertToDictionary, settings As GeneralSettingsBase)
        _owner = owner
        _currentObject = currentObject
        _compareObject = compareObject
        _currentContainer = currentKBLMapper
        _compareContainer = compareKBLMapper
        _currentOccurrenceMapper = currentKBLMapper?.KBLOccurrenceMapper
        _compareOccurrenceMapper = compareKBLMapper?.KBLOccurrenceMapper
        _currentKBLMapper = currentKBLMapper
        _compareKBLMapper = compareKBLMapper
        _currentActiveObjects = currentActiveObjects
        _compareActiveObjects = compareActiveObjects
        _listConvertToDictionary = listConvertToDictionary
        _generalSettings = settings
    End Sub

    Public Property ExcludedProperties As New ExcludedProperties

    Public ReadOnly Property Settings As GeneralSettingsBase
        Get
            Return _generalSettings
        End Get
    End Property

    Protected ReadOnly Property ResolvedExcludedProperties As List(Of String)
        Get
            If _owner Is Nothing Then
                Return Me.ExcludedProperties.TopLevelExcludedProperties
            Else
                Return _owner.ExcludedProperties.SubLevelExcludedProperties
            End If
        End Get
    End Property

    Protected Function CompareProperties(Of T As ChangedProperty)(currentObject As Object, compareObject As Object) As T
        Dim changedProp As T
        Dim constrInfos As ConstructorInfo() = GetType(T).GetConstructors()
        If constrInfos.Where(Function(ci) ci.GetParameters.Length = 6).Any() Then
            changedProp = CType(Activator.CreateInstance(GetType(T), Me, _currentKBLMapper, _compareKBLMapper, _currentActiveObjects, _compareActiveObjects, Me.Settings), T)
        ElseIf constrInfos.Where(Function(ci) ci.GetParameters.Length = 4).Any() Then
            changedProp = CType(Activator.CreateInstance(GetType(T), Me, _currentKBLMapper, _compareKBLMapper, Me.Settings), T)
        Else
            changedProp = CType(Activator.CreateInstance(GetType(T), Me, Me.Settings), T)
        End If
        changedProp.CompareObjectProperties(currentObject, compareObject, ResolvedExcludedProperties)
        Return changedProp
    End Function

    Public Shared ReadOnly Property All As TypeWithACollection(Of KblObjectTypeAttribute)
        Get
            Static myCompMapperTypes As TypeWithACollection(Of KblObjectTypeAttribute) = GetType(ComparisonMapper).Assembly.GetTypes(Of ComparisonMapper).Instanceables.WithAttribute(Of KblObjectTypeAttribute)
            Return myCompMapperTypes
        End Get
    End Property

    Public ReadOnly Property HasChanges As Boolean
        Get
            Return Me.Changes IsNot Nothing AndAlso Me.Changes.Count > 0
        End Get
    End Property

    Public Property Changes As New CompareChangesCollection(Of Object, Object, ChangedProperty)

    Protected Function AddOrReplaceChangeWithInverse(key As String, item As Object, changeType As CompareChangeType) As ChangedItem
        If _generalSettings.InverseCompare Then
            Select Case changeType
                Case CompareChangeType.Deleted
                    Return Me.Changes.AddOrReplaceNew(key, item)
                Case CompareChangeType.[New]
                    Return Me.Changes.AddOrReplaceNewDeleted(key, item)
            End Select
        End If

        Return Me.Changes.AddOrReplaceNew(key, item, changeType)
    End Function

    Protected Function AddOrReplaceChangeWithInverse(key As String, kblIdRef As String, kblIdComp As String, item As Object, changeType As CompareChangeType) As ChangedItem
        If _generalSettings.InverseCompare Then
            Select Case changeType
                Case CompareChangeType.Deleted
                    Return Me.Changes.AddOrReplaceNew(key, kblIdRef, kblIdComp, item)
                Case CompareChangeType.[New]
                    Return Me.Changes.AddOrReplaceNewDeleted(key, kblIdRef, kblIdComp, item)
            End Select
        End If

        Return Me.Changes.AddOrReplaceNew(key, kblIdRef, kblIdComp, item, changeType)
    End Function

    Public ReadOnly Property DefaultWireLengthType As String
        Get
            Return _generalSettings.DefaultWireLengthType
        End Get
    End Property
    Public ReadOnly Property DefaultCableLengthType As String
        Get
            Return _generalSettings.DefaultCableLengthType
        End Get
    End Property

End Class
