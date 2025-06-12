Imports System.IO
Imports devDept.Eyeshot
Imports devDept.Eyeshot.Entities

Namespace D3D.Consolidated.Designs

    Public MustInherit Class BaseFileLoader
        Implements IDisposable

        Public Event LoadFinished(sender As Object, e As EventArgs)
        Public Event ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs)

        Protected _entities As New List(Of Entity)
        Protected _layer As Layer
        Protected _model As devDept.Eyeshot.Design
        Protected _loading As Boolean = False
        Protected _materials As New MaterialKeyedCollection

        Public Sub New(model As devDept.Eyeshot.Design, Optional layerName As String = "")
            _model = model

            If (Not String.IsNullOrEmpty(layerName)) Then
                _layer = model.Layers.Where(Function(l) l.Name.ToLower = layerName.ToLower).FirstOrDefault()

                If (_layer Is Nothing) Then
                    _layer = New Layer(layerName)

                    model.Layers.Add(_layer)
                End If
            End If
        End Sub

        Public MustOverride Sub Load(fileName As String)
        Public MustOverride Async Function LoadAsync(fileName As String, Optional cancel As Consolidated.Controls.D3DCancellationTokenSource = Nothing) As Task

        Public Shared Function GetExtensionFromFilter(fileFilter As String) As String
            Return fileFilter.Split("|"c).Last.TrimStart("*"c).TrimStart("."c)
        End Function


        Protected Sub OnLoadFinished(sender As Object, e As EventArgs)
            RaiseEvent LoadFinished(sender, e)
        End Sub

        Protected Sub OnProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs)
            RaiseEvent ProgressChanged(sender, e)
        End Sub

        Public ReadOnly Property Entities As List(Of Entity)
            Get
                Return _entities
            End Get
        End Property

        Public ReadOnly Property Layer As Layer
            Get
                Return _layer
            End Get
        End Property

        Public ReadOnly Property Materials As MaterialKeyedCollection
            Get
                Return _materials
            End Get
        End Property

        Public Shared Function Create(fullPath As String, model As devDept.Eyeshot.Design) As BaseFileLoader
            Select Case IO.Path.GetExtension(fullPath).ToLower
                Case KnownFile.JT
                    Throw New NotSupportedException($"JT format no longer supported here, use '{GetType(Translators.ReadJT2).FullName}' instead!")
                Case Else
                    Return New ObjectFileLoader(model, IO.Path.GetFileNameWithoutExtension(fullPath).Replace(" "c, "_"c))
            End Select
        End Function

#Region "IDisposable Support"
        Protected _disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not _disposedValue Then
                _entities = Nothing
                _model = Nothing
                _materials = Nothing
                _entities = Nothing
                _layer = Nothing
            End If

            _disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            ' TODO: uncomment the following line if Finalize() is overridden above.
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class

End Namespace