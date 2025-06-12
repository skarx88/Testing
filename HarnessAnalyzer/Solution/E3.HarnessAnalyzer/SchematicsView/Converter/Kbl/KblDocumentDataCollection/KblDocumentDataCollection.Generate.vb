Namespace Schematics.Converter.Kbl

    Partial Public Class KblDocumentDataCollection

        Public Function CancelGeneration() As Boolean
            If _cancelTokenSource IsNot Nothing AndAlso _cancelTokenSource.Token.CanBeCanceled Then
                _cancelTokenSource.Cancel()
                Return True
            End If
            Return False
        End Function

        Private Async Sub ProcessAutoGeneration(documentId As String)
            Select Case AutoGenerate
                Case AutoGenerateType.Async
                    _lastConversion = Await GenerateNewEdbAsyncInternal(documentId, True)
            End Select
        End Sub

        Private Function GenerateNewEdb(startDocumentId As String, isAutoGenerating As Boolean) As KblConversionResult
            If IsGenerating Then
                CancelGeneration()
            End If

            SyncLock Me ' allow only one gerate at once at the same class
                _isGenerating = True
                _cancelled = False
                _cancelTokenSource = New Threading.CancellationTokenSource

                Using kblEdbConverter As New KblEdbConverter()
                    AddHandler kblEdbConverter.ErrorInitialize, AddressOf converter_InitializeError
                    Try
                        kblEdbConverter.Init(Me.ToArray, _cancelTokenSource.Token)
                        If Not (_cancelTokenSource?.IsCancellationRequested).GetValueOrDefault Then
                            AddConverterEvents(kblEdbConverter)
                            OnBeforeGenerationStart(New GenerationEventArgs(startDocumentId, isAutoGenerating, kblEdbConverter.Id))
                            If _cancelTokenSource IsNot Nothing Then
                                _lastConversion = kblEdbConverter.CreateEdb(_cancelTokenSource.Token)
                            Else
                                Throw New System.OperationCanceledException() ' HINT: this is the case when the collection was already (or is) disposed -> we simply cancel the generation
                            End If
                        End If
                    Catch ex As TaskCanceledException 'HINT: DISCONNECTED-CONTEXT. This means there is not enough memory available to generated the EDB model!
                        _cancelled = True
                        Throw
                    Catch ex As System.OperationCanceledException 'HINT: When Operation/TaskCancelledCancelledException is thrown here, the Generation was cancelled correctly - the code can proceed normal after this interruption. This exception will always hit while in DEBUG! In RELEASE this behavior will not be recognizable.
                        _cancelled = True
                        Throw
                    Finally
                        If (_cancelTokenSource?.IsCancellationRequested).GetValueOrDefault Then
                            _cancelled = True
                        End If
                        RemoveHandler kblEdbConverter.ErrorInitialize, AddressOf converter_InitializeError
                        RemoveConverterEvents(kblEdbConverter)
                        _isGenerating = False
                        OnGenerationFinished(New GenerationFinishedEventArgs(startDocumentId, _cancelled, isAutoGenerating, kblEdbConverter.Id, _lastConversion))
                    End Try
                End Using
                Return _lastConversion
            End SyncLock
        End Function

        Private Sub converter_InitializeError(sender As Object, e As KblEdbConverter.InitializeErrorEventArgs)
            Dim args As New ErrorEventArgs(ErrorEventArgs.EdbConverterErrorType.Unknown, e.DocumentId, e.Object, e.Message)

            Select Case e.Type
                Case KblEdbConverter.InitializeErrorEventArgs.ErrorType.EntityAlreadyExists
                    args = New ErrorEventArgs(ErrorEventArgs.EdbConverterErrorType.InitEntityAlreadyExistsError, e.DocumentId, e.Object, String.Format("{0}: ""{1}""", SchematicsViewStrings.Converter_Log_Caption, e.Message))
            End Select

            SendOnSyncContext(Sub() RaiseEvent Error(Me, args))
        End Sub

        Private Sub AddConverterEvents(converter As Kbl.KblEdbConverter)
            AddHandler converter.ConnectorResolving, AddressOf _currentConverter_ConnectorResolving
            AddHandler converter.ResolveComponentType, AddressOf _currentConverter_ResolveComponentType
            AddHandler converter.ResolveEntityId, AddressOf _currentConverter_ResolveEntityId
            AddHandler converter.AfterEntityCreated, AddressOf _currentConverter_AfterEntityCreated
            AddHandler converter.ProgressChanged, AddressOf _currentConverter_ProgressChanged
            AddHandler converter.AfterFunctionSet, AddressOf _currentConverter_AfterFunctionSet
            AddHandler converter.AfterModulesSet, AddressOf _currentConverter_AfterModuleSet
        End Sub

        Private Sub RemoveConverterEvents(converter As Kbl.KblEdbConverter)
            RemoveHandler converter.ConnectorResolving, AddressOf _currentConverter_ConnectorResolving
            RemoveHandler converter.ResolveComponentType, AddressOf _currentConverter_ResolveComponentType
            RemoveHandler converter.ResolveEntityId, AddressOf _currentConverter_ResolveEntityId
            RemoveHandler converter.AfterEntityCreated, AddressOf _currentConverter_AfterEntityCreated
            RemoveHandler converter.ProgressChanged, AddressOf _currentConverter_ProgressChanged
            RemoveHandler converter.AfterFunctionSet, AddressOf _currentConverter_AfterFunctionSet
            RemoveHandler converter.AfterModulesSet, AddressOf _currentConverter_AfterModuleSet
        End Sub

        Public Async Function GenerateNewEdbAsync(documentId As String) As Task(Of KblConversionResult)
            Return Await GenerateNewEdbAsyncInternal(documentId, False)
        End Function

        Private Async Function GenerateNewEdbAsyncInternal(documentId As String, isAutoGenerating As Boolean) As Task(Of KblConversionResult)
            Using Await _lock.BeginWaitAsync(Nothing)
                Try
                    Return Await Task.Factory.StartNew(Function() GenerateNewEdb(documentId, isAutoGenerating))
                Catch ex As TaskCanceledException
                    Return Nothing
                Catch ex As System.OperationCanceledException
                    Return Nothing
                End Try
            End Using
        End Function

        Protected Overridable Sub OnBeforeGenerationStart(e As GenerationEventArgs)
            SendOnSyncContext(Sub() RaiseEvent BeforeGenerationStart(Me, e))
        End Sub

        Protected Overridable Sub OnProgressChanged(e As Converter.ConverterProgressChangedEventArgs)
            SendOnSyncContext(Sub() RaiseEvent ProgressChanged(Me, e))
        End Sub

        Protected Overridable Sub OnGenerationFinished(e As GenerationFinishedEventArgs)
            SendOnSyncContext(Sub() RaiseEvent GenerationFinished(Me, e))
        End Sub

        Private Sub _currentConverter_ConnectorResolving(sender As Object, e As Converter.Kbl.ConnectorResolvingEventArgs)
            SendOnSyncContext(Sub() RaiseEvent ConnectorResolving(Me, e))
        End Sub

        Private Sub _currentConverter_ResolveComponentType(sender As Object, e As Converter.Kbl.ComponentTypeResolveEventArgs)
            SendOnSyncContext(Sub() RaiseEvent ResolveComponentType(Me, e))
        End Sub

        Private Sub _currentConverter_ResolveEntityId(sender As Object, e As Converter.IdEventArgs)
            SendOnSyncContext(Sub() RaiseEvent ResolveEntityId(Me, e))
        End Sub

        Private Sub _currentConverter_AfterEntityCreated(sender As Object, e As Controls.EntityEventArgs)
            SendOnSyncContext(Sub() RaiseEvent AfterEntityCreated(Me, e))
        End Sub

        Private Sub _currentConverter_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs)
            SendOnSyncContext(Sub() RaiseEvent ProgressChanged(Me, New Converter.ConverterProgressChangedEventArgs(e.ProgressPercentage, DirectCast(sender, Converter.Kbl.KblEdbConverter).Id)))
        End Sub

        Private Sub _currentConverter_AfterFunctionSet(sender As Object, e As FunctionSetEventArgs)
            SendOnSyncContext(Sub() RaiseEvent AfterFunctionSet(Me, e))
        End Sub

        Private Sub _currentConverter_AfterModuleSet(sender As Object, e As ModuleSetEventArgs)
            SendOnSyncContext(Sub() RaiseEvent AfterModulesSet(Me, e))
        End Sub

        Private Sub SendOnSyncContext(action As Action)
            Try
                If _syncCtx IsNot Nothing Then
                    _syncCtx.Send(New System.Threading.SendOrPostCallback(Sub() action.Invoke()), Nothing)
                End If
            Catch ex As System.ComponentModel.InvalidAsynchronousStateException
                'ignore, means that the syncContext is no longer available
            End Try
        End Sub

        Public Enum AutoGenerateType
            [Default] = 0
            Async = 1
        End Enum

    End Class

End Namespace