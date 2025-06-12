Imports devDept
Imports devDept.Eyeshot
Imports devDept.Eyeshot.Translators

Namespace D3D.Consolidated.Designs

    Public Class ObjectFileLoader
        Inherits BaseFileLoader

        Public Sub New(model As devDept.Eyeshot.Design, Optional layerName As String = "")
            MyBase.New(model, layerName)
        End Sub

        Public Overrides Sub Load(fileName As String)
            AddHandler _model.ProgressChanged, AddressOf ViewportLayoutWorkerProgressChanged

            Dim reader As ReadFileAsync = GetReader(fileName)

            _loading = True

            reader.DoWork()

            ViewportLayoutOnReaderWorkCompleted(_model, New WorkCompletedEventArgs(reader))
        End Sub

        Public Overrides Async Function LoadAsync(fileName As String, Optional cancel As Consolidated.Controls.D3DCancellationTokenSource = Nothing) As Task
            AddHandler _model.WorkCompleted, AddressOf ViewportLayoutOnReaderWorkCompleted
            AddHandler _model.ProgressChanged, AddressOf ViewportLayoutWorkerProgressChanged

            Dim reader As ReadFileAsync = GetReader(fileName)

            _loading = True
            _model.ObjectManipulator.Cancel()
            _model.StartWork(reader)

            Await Task.Factory.StartNew(Sub() Threading.SpinWait.SpinUntil(Function()
                                                                               If (cancel IsNot Nothing AndAlso cancel.IsCancellationRequested) Then
                                                                                   _model.CancelWork()
                                                                                   Return True
                                                                               End If

                                                                               If (Not _loading) Then Return True

                                                                               Return False
                                                                           End Function))
        End Function


        Public Shared Function GetReader(fileName As String) As ReadFileAsync
            Dim ext As String = IO.Path.GetExtension(fileName).TrimStart("."c).ToLower

            Select Case ext
                Case GetExtensionFromFilter(FileExtensionFilters.STLbinary).ToLower
                    Return New ReadSTL(fileName)
                Case GetExtensionFromFilter(FileExtensionFilters.Wavefront).ToLower
                    Return New ReadOBJ(fileName)
                Case Else
                    Throw New NotImplementedException(String.Format("File extension ({0}) not implemented!", ext))
            End Select
        End Function

        Public Shared Function GetSupportedFileExtensions() As String()
            Return SupportedFileFilters.Select(Function(ff) GetExtensionFromFilter(ff)).ToArray
        End Function

        Public Shared Function LoadFromFile(fileName As String) As ReadFileAsync
            Dim reader As ReadFileAsync = GetReader(fileName)
            reader.DoWork()

            Return reader
        End Function


        Private Sub ViewportLayoutOnReaderWorkCompleted(sender As Object, workCompletedEventArgs As WorkCompletedEventArgs)
            Dim reader As ReadFileAsync = DirectCast(workCompletedEventArgs.WorkUnit, ReadFileAsync)

            _entities = New List(Of Entities.Entity)
            _entities.AddRange(reader.Entities)

            If (TypeOf reader Is ReadOBJ) Then _materials = DirectCast(reader, ReadOBJ).Materials

            RemoveHandler _model.WorkCompleted, AddressOf ViewportLayoutOnReaderWorkCompleted
            RemoveHandler _model.ProgressChanged, AddressOf ViewportLayoutWorkerProgressChanged

            _loading = False

            OnLoadFinished(Me, New EventArgs)
        End Sub

        Private Sub ViewportLayoutWorkerProgressChanged(sender As Object, e As WorkUnit.ProgressChangedEventArgs)
            OnProgressChanged(Me, New System.ComponentModel.ProgressChangedEventArgs(e.Progress, e.Text))
        End Sub

        Public Shared ReadOnly Property SupportedFileFilters As String()
            Get
                Return New String() {FileExtensionFilters.STLbinary, FileExtensionFilters.Wavefront}
            End Get
        End Property


#Region "IDisposable Support"

        ' IDisposable
        Protected Overrides Sub Dispose(disposing As Boolean)
            If Not _disposedValue Then
                If disposing Then
                    RemoveHandler _model.WorkCompleted, AddressOf ViewportLayoutOnReaderWorkCompleted
                End If
            End If

            MyBase.Dispose(disposing)

            _disposedValue = True
        End Sub

#End Region

    End Class

End Namespace