
Imports Zuken.E3.HarnessAnalyzer.D3D.Consolidated.Controls
Imports Zuken.E3.HarnessAnalyzer.Project.Documents
Imports Zuken.E3.Lib.Eyeshot.Model

Namespace D3D.Document.Controls

    Public Class DocumentClone
        Implements IDisposable

        Private _entities As DocumentEntitiesCollection
        Private _addedAndInitialized As New HashSet(Of D3D.Document.DocumentDesign)
        Private _disposedValue As Boolean

        Public Event BeforeInitialize(sender As Object, e As BeforeInitializeEventArgs)
        Public Event AfterInitialize(sender As Object, e As AfterInitializeEventArgs)

        Public Sub New(document As HcvDocument)
            Me.Document = document
            Me.EEModel = document.Model
            _entities = New DocumentEntitiesCollection(document)

            For Each en As IBaseModelEntityEx In document.Entities
                Me._entities.Add(CType(en.Clone, IBaseModelEntityEx))
            Next
        End Sub

        Property Document As HcvDocument
        Property Owner As ClonedDocuments

        ReadOnly Property EEModel As E3.Lib.Model.EESystemModel
        ReadOnly Property AddedEnvironment As D3D.Document.DocumentDesign

        ReadOnly Property IsAdded As Boolean
            Get
                Return Me.AddedEnvironment IsNot Nothing
            End Get
        End Property

        ReadOnly Property Entities As DocumentEntitiesCollection
            Get
                Return _entities
            End Get
        End Property

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not _disposedValue Then
                If disposing Then
                    Me.Owner = Nothing
                End If
                _entities.Clear()
                _addedAndInitialized.Clear()
                _Document = Nothing
                _AddedEnvironment = Nothing
                _EEModel = Nothing
                _disposedValue = True
            End If
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub

    End Class
End Namespace