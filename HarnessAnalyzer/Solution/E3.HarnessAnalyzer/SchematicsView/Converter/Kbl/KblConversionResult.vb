Imports Zuken.E3.App.Controls

Namespace Schematics.Converter.Kbl

    Public Class KblConversionResult
        Inherits ConversionResult

        Private _modulesSet As New Dictionary(Of String, String)
        Private _functionSet As New Dictionary(Of String, String)
        Private _readonlymodulesSet As New System.Collections.ObjectModel.ReadOnlyDictionary(Of String, String)(_modulesSet)
        Private _readonlyfunctionSet As New System.Collections.ObjectModel.ReadOnlyDictionary(Of String, String)(_functionSet)

        Public Sub New(converterId As Guid)
            MyBase.New(converterId)
        End Sub

        ReadOnly Property ModulesSet As System.Collections.ObjectModel.ReadOnlyDictionary(Of String, String)
            Get
                Return _readonlymodulesSet
            End Get
        End Property

        ReadOnly Property FunctionSet As System.Collections.ObjectModel.ReadOnlyDictionary(Of String, String)
            Get
                Return _readonlyfunctionSet
            End Get
        End Property

        Public Function GetModelItems(edbObjectId As String) As Connectivity.Model.BaseItem()
            If Not String.IsNullOrEmpty(edbObjectId) Then
                If Me.IsFinishedSuccessfully Then
                    Dim entities As List(Of EdbConversionEntity) = GetEntities(edbObjectId)
                    Return GetModelItems(entities).ToArray
                End If
            End If
            Return Array.Empty(Of Connectivity.Model.BaseItem)()
        End Function

        Public Function TryGetFunctionName(edbObjectId As String, ByRef functionName As String) As Boolean
            Return _functionSet.TryGetValue(edbObjectId, functionName)
        End Function

        Public Function TryGetModuleName(edbObjectId As String, ByRef moduleName As String) As Boolean
            Return _modulesSet.TryGetValue(edbObjectId, moduleName)
        End Function

        Public Function IsAndHasFunction(edbObjectId As String) As Boolean
            Return _functionSet.ContainsKey(edbObjectId)
        End Function

        Public Function IsAndHasModule(edbObjectId As String) As Boolean
            Return _modulesSet.ContainsKey(edbObjectId)
        End Function

        Public Function GetEntities(edbObjectId As String) As List(Of EdbConversionEntity)
            Dim entitiesList As New List(Of EdbConversionEntity)
            Dim edbIds As New List(Of String)

            For Each block As EdbConversionDocument In Me.ConvertedDocuments
                If block.Contains(edbObjectId) Then
                    entitiesList.Add(block(edbObjectId))
                End If
            Next
            Return entitiesList
        End Function

        Private Function GetModelItems(entities As IEnumerable(Of EdbConversionEntity)) As List(Of Connectivity.Model.BaseItem)
            Dim list As New List(Of Connectivity.Model.BaseItem)
            For Each ent As EdbConversionEntity In entities
                If TypeOf ent Is EdbConversionCavityGroupEntity Then
                    For Each edbId As String In CType(ent, EdbConversionCavityGroupEntity)
                        list.Add(Me.Model.GetItemById(edbId))
                    Next
                Else
                    list.Add(Me.Model.GetItemById(ent.Id))
                End If
            Next
            Return list
        End Function

        Public Function Contains(edbObjectId As String) As Boolean
            Return IsAndHasFunction(edbObjectId) OrElse IsAndHasModule(edbObjectId) OrElse GetEntities(edbObjectId).Count > 0
        End Function

        Protected Overrides Sub Dispose(disposing As Boolean)
            _functionSet = Nothing
            _modulesSet = Nothing
            MyBase.Dispose(disposing)
        End Sub

        Public Class KblConversionAdapter
            Inherits ConversionResultAdapter

            Public Sub New(result As KblConversionResult)
                MyBase.New(result)
            End Sub

            Public Overrides Sub AttachTo(converter As IKblConverter)
                MyBase.AttachTo(converter)

                With CType(converter, Converter.Kbl.KblEdbConverter)
                    AddHandler .AfterFunctionSet, AddressOf _kblConverter_AfterFunctionSet
                    AddHandler .AfterModulesSet, AddressOf _kblConverter_AfterModuleSet
                End With
            End Sub

            Public Overrides Sub Detach()
                If Converter IsNot Nothing Then
                    With Me.Converter
                        RemoveHandler .AfterFunctionSet, AddressOf _kblConverter_AfterFunctionSet
                        RemoveHandler .AfterModulesSet, AddressOf _kblConverter_AfterModuleSet
                    End With
                End If
                MyBase.Detach()
            End Sub

            Private Sub _kblConverter_AfterFunctionSet(sender As Object, e As FunctionSetEventArgs)
                Result._functionSet.TryAdd(e.FunctionId, e.FunctionName)
            End Sub

            Private Sub _kblConverter_AfterModuleSet(sender As Object, e As ModuleSetEventArgs)
                For Each mi As ModuleInfo In e.Modules
                    Result._modulesSet.TryAdd(mi.Id, mi.Name)
                Next
            End Sub

            Protected Overrides Sub OnAfterConversionFinished(e As FinishedEventArgs)
                MyBase.OnAfterConversionFinished(e)
            End Sub

            Protected Overrides Sub OnBeforeConversionStart(e As System.ComponentModel.CancelEventArgs)
                MyBase.OnBeforeConversionStart(e)
                With Result
                    ._functionSet.Clear()
                    ._modulesSet.Clear()
                End With
            End Sub

            Private ReadOnly Property Result As Kbl.KblConversionResult
                Get
                    Return DirectCast(_result, Kbl.KblConversionResult)
                End Get
            End Property

            Private ReadOnly Property Converter As KblEdbConverter
                Get
                    Return DirectCast(_converter, Kbl.KblEdbConverter)
                End Get
            End Property

            Public Shared Shadows Function AttachNewResultTo(converter As IKblConverter) As KblConversionAdapter
                Return ConversionResultAdapter.AttachNewResultTo(Of KblConversionAdapter, KblConversionResult)(converter)
            End Function

        End Class

    End Class

End Namespace
