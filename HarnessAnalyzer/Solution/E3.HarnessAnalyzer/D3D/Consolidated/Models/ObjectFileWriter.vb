Imports System.IO
Imports devDept
Imports devDept.Eyeshot
Imports devDept.Eyeshot.Entities
Imports devDept.Eyeshot.Translators
Imports devDept.WorkUnit

Namespace D3D.Consolidated.Designs

    Public Class ObjectFileWriter
        Implements IDisposable

        Public Event ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs)
        Public Event Finished(sender As Object, e As EventArgs)

        Private _writing As Boolean = False
        Private _layer As Layer
        Private _entities As New List(Of Entity)
        Private _model As devDept.Eyeshot.Design

        Sub New(model As devDept.Eyeshot.Design)
            _model = model
        End Sub

        Public ReadOnly Property Layer As Layer
            Get
                Return _layer
            End Get
        End Property

        ReadOnly Property Entities As List(Of Entity)
            Get
                Return _entities
            End Get
        End Property

        Public Sub Save(fileName As String)
            AddHandler _model.ProgressChanged, AddressOf ViewPortLayoutWorkerProgressChanged
            Dim writer As WriteFileAsync = GetWriter(_model, Me.Entities, fileName)
            _writing = True
            writer.DoWork()
            ViewportLayoutOnReaderWorkCompleted(_model, New WorkCompletedEventArgs(writer))
        End Sub

        Private Shared Function GetFileStream(fileName As String) As IO.FileStream
            Return New IO.FileStream(fileName, IO.FileMode.Create, IO.FileAccess.Write, IO.FileShare.None)
        End Function

        Public Shared Function GetWriter(vp As devDept.Eyeshot.Design, entities As IList(Of Entity), fileName As String) As WriteFileAsync
            Dim ext As String = IO.Path.GetExtension(fileName).TrimStart("."c).ToLower
            Select Case ext
                Case GetExtensionFromFilter(FileExtensionFilters.STLbinary).ToLower
                    Return New WriteSTL(New WriteParams(entities, vp.Layers, vp.Blocks), fileName) ' GetFileStream(fileName)) ' HINT: GetFileStream: when using the FileName (string) directly the writer creates an additional empty directory off the given file name - when using the stream the writer writes only to the stream
                Case GetExtensionFromFilter(FileExtensionFilters.Wavefront).ToLower
                    Dim writeParams As New WriteParamsWithMaterials(entities, vp.Layers, vp.Blocks, New MaterialKeyedCollection(entities.Where(Function(e) Not String.IsNullOrEmpty(e.MaterialName)).Select(Function(e) vp.Materials(e.MaterialName)).Distinct()))
                    Return New WriteOBJ(writeParams, fileName) ') GetFileStream(fileName))                                     ' HINT: GetFileStream: when using the FileName (string) directly the writer creates an additional empty directory off the given file name - when using the stream the writer writes only to the stream
                Case Else
                    Throw New NotImplementedException(String.Format("File extension ""{0}"" not implemented!", ext))
            End Select
        End Function

        Public Async Function SaveAsync(fileName As String, Optional cancel As Consolidated.Controls.D3DCancellationTokenSource = Nothing) As Task
            AddHandler _model.WorkCompleted, AddressOf ViewportLayoutOnReaderWorkCompleted
            AddHandler _model.ProgressChanged, AddressOf ViewPortLayoutWorkerProgressChanged
            Dim writer As WriteFileAsync = Nothing

            Try
                writer = GetWriter(_model, Me.Entities, fileName)

                _writing = True
                _model.ObjectManipulator.Cancel()
                _model.StartWork(writer)

                Await TaskEx.WaitUntil(
                    Function()
                        If (cancel IsNot Nothing AndAlso cancel.IsCancellationRequested) Then
                            _model.CancelWork()
                            Return True
                        End If

                        If Not _writing Then
                            Return True
                        End If
                        Return False
                    End Function)
            Finally
                If writer IsNot Nothing AndAlso writer.Stream IsNot Nothing Then
                    writer.Stream.Flush()
                    writer.Stream.Close()
                    writer.Stream.Dispose()
                End If

                Dim fi As New FileInfo(fileName)
                If fi.Exists Then
                    Dim dir As New DirectoryInfo(IO.Path.Combine(fi.DirectoryName, IO.Path.GetFileNameWithoutExtension(fi.Name)))
                    If dir.Exists Then
                        dir.Delete(True)
                    End If
                End If

            End Try
        End Function

        Private Sub ViewPortLayoutWorkerProgressChanged(sender As Object, e As EventArgs)
            Dim progress As Integer = If(TypeOf e Is ProgressChangedEventArgs, CType(e, ProgressChangedEventArgs).Progress, CType(e, System.ComponentModel.ProgressChangedEventArgs).ProgressPercentage)
            Dim text As String = If(TypeOf e Is ProgressChangedEventArgs, CType(e, ProgressChangedEventArgs).Text, CStr(CType(e, System.ComponentModel.ProgressChangedEventArgs).UserState))
            RaiseEvent ProgressChanged(Me, New System.ComponentModel.ProgressChangedEventArgs(progress, text))
        End Sub

        Private Sub ViewportLayoutOnReaderWorkCompleted(sender As Object, workCompletedEventArgs As WorkCompletedEventArgs)
            RemoveHandler _model.WorkCompleted, AddressOf ViewportLayoutOnReaderWorkCompleted
            RemoveHandler _model.ProgressChanged, AddressOf ViewPortLayoutWorkerProgressChanged
            _writing = False
            RaiseEvent Finished(Me, New EventArgs)
        End Sub

        Shared ReadOnly Property SupportedFileFilters As String()
            Get
                Return New String() {FileExtensionFilters.STLbinary, FileExtensionFilters.Wavefront}
            End Get
        End Property

        Public Shared Function GetExtensionFromFilter(fileFilter As String) As String
            Return fileFilter.Split("|"c).Last.TrimStart("*"c).TrimStart("."c)
        End Function

        Public Shared Function GetSupportedFileExtensions() As String()
            Return SupportedFileFilters.Select(Function(ff) GetExtensionFromFilter(ff)).ToArray
        End Function


#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    RemoveHandler _model.WorkCompleted, AddressOf ViewportLayoutOnReaderWorkCompleted
                    _writing = False
                End If

                _model = Nothing
                _entities = Nothing
                _layer = Nothing
            End If
            Me.disposedValue = True
        End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class

End Namespace