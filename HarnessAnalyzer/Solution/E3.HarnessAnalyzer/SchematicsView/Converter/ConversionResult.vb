Imports System.ComponentModel
Imports Zuken.E3.App.Controls
Imports Zuken.E3.HarnessAnalyzer.Schematics.Controls
Imports Zuken.E3.HarnessAnalyzer.Schematics.Converter.Kbl

Namespace Schematics.Converter

    Public Class ConversionResult
        Implements IDisposable

        Private _edbModel As Connectivity.Model.EdbModel
        Private _convertedDocuments As New DocumentsCollection
        Private _finished As FinishedType = FinishedType.Unfinished
        Private _converterId As Guid

        Public Sub New(converterId As Guid)
            _converterId = converterId
        End Sub

        Public ReadOnly Property ConverterId As Guid
            Get
                Return _converterId
            End Get
        End Property

        Public ReadOnly Property Model As Connectivity.Model.EdbModel
            Get
                Return _edbModel
            End Get
        End Property

        Public ReadOnly Property ConvertedDocuments As DocumentsCollection
            Get
                Return _convertedDocuments
            End Get
        End Property

        Public ReadOnly Property Finished As FinishedType
            Get
                Return _finished
            End Get
        End Property

        Public Function IsFinishedSuccessfully() As Boolean
            Return Me.Finished = FinishedType.Finished
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' So ermitteln Sie überflüssige Aufrufe

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                _convertedDocuments = Nothing
                _edbModel = Nothing
            End If
            Me.disposedValue = True
        End Sub

        ' Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Ändern Sie diesen Code nicht. Fügen Sie oben in Dispose(disposing As Boolean) Bereinigungscode ein.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

        Public Class ConversionResultAdapter
            Implements IDisposable

            Protected WithEvents _converter As IKblConverter
            Protected _result As ConversionResult

            Public Sub New(resultToFill As ConversionResult)
                _result = resultToFill
            End Sub

            Private Sub _converter_AfterConversionFinished(sender As Object, e As FinishedEventArgs) Handles _converter.IKblConverter_AfterConversionFinished
                OnAfterConversionFinished(e)
            End Sub

            Private Sub _converter_AfterEdbEntityCreated(sender As Object, e As EntityEventArgs) Handles _converter.AfterEdbEntityCreated
                AfterEdbEntitiyCreated(_result.ConvertedDocuments, e.Entity)
            End Sub

            Public Shared Sub AfterEdbEntitiyCreated(entities As DocumentsCollection, entity As EdbConversionEntity)
                If entities IsNot Nothing AndAlso entity IsNot Nothing Then
                    'HINT: objects are converted per Document. But in XHCV we can have the same objects across multiple documents (seperation points). To solve this problem they way of conversion must be changed to a single converter among the whole xhcv (which is currently not possible because of the way loading documents seperately)
                    ' -> As a result the objects are made "double" from each converter, but the converter can't detect that the current conversion is already done by another converter. So we are getting this behavior of double keys with different objects.
                    ' -> to solve this the last converted object (with same content) is added (replaced) to this collection (-> which is no problem because resolving entities from the added collection is always done by Id)
                    If Not entities.AddNewOrGet(entity.DocumentId).TryAdd(entity) Then
                        entities(entity.DocumentId).Remove(entity.Id)
                        entities(entity.DocumentId).Add(entity)
                    End If
                End If
            End Sub

            Private Sub _converter_BeforeConversionStart(sender As Object, e As CancelEventArgs) Handles _converter.IKblConverter_BeforeConversionStart
                OnBeforeConversionStart(e)
            End Sub

            Protected Overridable Sub OnBeforeConversionStart(e As System.ComponentModel.CancelEventArgs)
                _result._finished = FinishedType.Unfinished
            End Sub

            Protected Overridable Sub OnAfterConversionFinished(e As FinishedEventArgs)
                If Not e.Cancelled Then
                    _result._finished = FinishedType.Finished
                    If e.Failed Then
                        _result._finished = _result._finished Or FinishedType.Failure
                    Else
                        _result._edbModel = e.EdbModel
                    End If
                End If
            End Sub

            Public Overridable Sub AttachTo(converter As Kbl.IKblConverter)
                _converter = converter
            End Sub

            Public Overridable Sub Detach()
                _converter = Nothing
            End Sub

            Public ReadOnly Property IsAttached As Boolean
                Get
                    Return _converter IsNot Nothing
                End Get
            End Property

            Public ReadOnly Property CurrentResult As ConversionResult
                Get
                    Return _result
                End Get
            End Property

            Public Shared Function AttachNewResultTo(converter As Kbl.IKblConverter) As ConversionResultAdapter
                Return AttachNewResultTo(Of ConversionResultAdapter, ConversionResult)(converter)
            End Function

            Protected Shared Function AttachNewResultTo(Of TResultAdaptor As ConversionResultAdapter, TNewResult As ConversionResult)(converter As Kbl.IKblConverter) As TResultAdaptor
                Dim newResult As TNewResult = CType(Activator.CreateInstance(GetType(TNewResult), converter.Id), TNewResult)
                Dim adaptor As TResultAdaptor = CType(Activator.CreateInstance(GetType(TResultAdaptor), newResult), TResultAdaptor)
                adaptor.AttachTo(converter)
                Return adaptor
            End Function

            Public Shared Function AttachNew(Of T As Kbl.IKblConverter)() As ConversionResultAdapter
                Return ConversionResultAdapter.AttachNew(GetType(T))
            End Function

            Public Shared Function AttachNew(converterType As Type) As ConversionResultAdapter
                If Not GetType(Kbl.IKblConverter).IsAssignableFrom(converterType) Then
                    Throw New ArgumentException(String.Format("Incorrect converter type! Does not implement ""{0}""-Interface", GetType(Kbl.IKblConverter).Name), NameOf(converterType))
                End If
                Dim converter As Kbl.IKblConverter = CType(Activator.CreateInstance(converterType, True), IKblConverter)
                Return ConversionResultAdapter.AttachNewResultTo(converter)
            End Function

#Region "IDisposable Support"
            Private disposedValue As Boolean ' So ermitteln Sie überflüssige Aufrufe

            ' IDisposable
            Protected Overridable Sub Dispose(disposing As Boolean)
                If Not Me.disposedValue Then
                    If disposing Then
                        Detach()
                    End If
                    _result = Nothing
                End If
                Me.disposedValue = True
            End Sub

            Public Sub Dispose() Implements IDisposable.Dispose
                ' Ändern Sie diesen Code nicht. Fügen Sie oben in Dispose(disposing As Boolean) Bereinigungscode ein.
                Dispose(True)
                GC.SuppressFinalize(Me)
            End Sub
#End Region

        End Class

    End Class

End Namespace