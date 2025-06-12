Imports System.Collections.ObjectModel
Imports System.Reflection
Imports Zuken.E3.HarnessAnalyzer.Schematics.Controls

Namespace Schematics.Converter.Kbl

    <ObfuscationAttribute(Feature:="renaming", ApplyToMembers:=False)>
    Public Class KblDocumentDataCollection
        Inherits KeyedCollection(Of String, KblDocumentData)
        Implements IDisposable

        Friend Event [Error](sender As Object, e As ErrorEventArgs)
        Friend Event ConnectorResolving(sender As Object, e As ConnectorResolvingEventArgs)
        Friend Event ResolveComponentType(sender As Object, e As ComponentTypeResolveEventArgs)
        Friend Event ResolveEntityId(sender As Object, e As IdEventArgs)
        Friend Event AfterEntityCreated(sender As Object, e As EntityEventArgs)
        Friend Event AfterModulesSet(sender As Object, e As ModuleSetEventArgs)
        Friend Event AfterFunctionSet(sender As Object, e As FunctionSetEventArgs)

        Public Event ProgressChanged(sender As Object, e As Converter.ConverterProgressChangedEventArgs)

        Private _lastConversion As KblConversionResult
        Private _cancelTokenSource As New Threading.CancellationTokenSource
        Private _cancelled As Boolean = False
        Private _isGenerating As Boolean = False
        Private _lock As New System.Threading.LockStateMachine
        Private _syncCtx As System.Threading.SynchronizationContext = System.Threading.SynchronizationContext.Current

        Private Const CHANGE_LOCKING_NAME As String = "#Changing"

        <Obfuscation(Feature:="renaming", ApplyToMembers:=True)>
        Public Event BeforeGenerationStart(sender As Object, e As GenerationEventArgs)
        <Obfuscation(Feature:="renaming", ApplyToMembers:=True)>
        Public Event GenerationFinished(sender As Object, e As GenerationFinishedEventArgs)

        Protected Overrides Sub ClearItems()
            Using _lock.BeginWait(CHANGE_LOCKING_NAME)
                MyBase.ClearItems()

                If AutoGenerate <> AutoGenerateType.Default AndAlso _lastConversion IsNot Nothing Then
                    DisposeLastConversionAndModel()
                    _lastConversion = Nothing
                End If
            End Using
        End Sub

        Private Sub DisposeLastConversionAndModel()
            If _lastConversion IsNot Nothing Then
                If _lastConversion.Model IsNot Nothing Then
                    _lastConversion.Model.Dispose()
                End If
                _lastConversion.Dispose()
            End If
        End Sub

        Protected Overrides Sub InsertItem(index As Integer, item As KblDocumentData)
            Using _lock.BeginWait(CHANGE_LOCKING_NAME) ' wait if async generation is finished (copies f.e. this collection for generating)
                MyBase.InsertItem(index, item)
            End Using
            ProcessAutoGeneration(item.DocumentId)
        End Sub

        Protected Overrides Sub RemoveItem(index As Integer)
            Dim item As KblDocumentData = MyBase.Item(index)
            Using _lock.BeginWait(CHANGE_LOCKING_NAME) ' wait if async generation is finished (copies f.e. this collection for generating)
                MyBase.RemoveItem(index)
            End Using
            ProcessAutoGeneration(item.DocumentId)
        End Sub

        Protected Overrides Sub SetItem(index As Integer, item As KblDocumentData)
            Using _lock.BeginWait(CHANGE_LOCKING_NAME) ' wait if async generation is finished (copies f.e. this collection for generating)
                MyBase.SetItem(index, item)
            End Using
            ProcessAutoGeneration(item.DocumentId)
        End Sub

        Public Function AddNew(documentId As String, kbl As KBLMapper) As KblDocumentData
            Dim newBlockData As New KblDocumentData(documentId, kbl)
            Me.Add(newBlockData)
            Return newBlockData
        End Function

        Public Function TryGetDocumentData(documentId As String, ByRef item As KblDocumentData) As Boolean
            If Me.Dictionary IsNot Nothing Then
                Return Me.Dictionary.TryGetValue(documentId, item)
            End If
            Return False
        End Function

        Public Function TryGetKblData(documentId As String) As KBLMapper
            Dim blockData As KblDocumentData = Nothing
            If TryGetDocumentData(documentId, blockData) Then
                Return blockData.Kbl
            End If
            Return Nothing
        End Function

        Public Function TryGetOccurrence(Of T As {IKblOccurrence, New})(documentId As String, itemKey As String) As T
            Dim kblMapper As KblMapper = TryGetKblData(documentId)
            If kblMapper IsNot Nothing Then
                Return kblMapper.GetOccurrenceObject(Of T)(itemKey)
            End If
            Return Nothing
        End Function

        Public Function TryGetModuleIds(documentId As String, objectId As String) As IEnumerable(Of String)
            Dim kblMapper As KblMapper = TryGetKblData(documentId)
            If kblMapper IsNot Nothing Then
                Return kblMapper.KBLObjectModuleMapper.TryGetOrDefault(objectId)
            End If
            Return New HashSet(Of String)
        End Function

        Protected Overrides Function GetKeyForItem(item As KblDocumentData) As String
            Return item.DocumentId
        End Function

    End Class

End Namespace