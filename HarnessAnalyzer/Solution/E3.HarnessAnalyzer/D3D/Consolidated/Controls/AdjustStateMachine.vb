Imports devDept.Eyeshot

Namespace D3D.Consolidated.Controls

    Public Class AdjustStateMachine
        Implements IDisposable

        Public Event Changed(sender As Object, e As AdjustModeChangedEventArgs)

        Private WithEvents _model As D3D.Consolidated.Designs.ConsolidatedDesign
        Private WithEvents _carStateMachine As CarStateMachine
        Private _adjustSettings As AdjustCarSettings
        Private _disposedValue As Boolean

        Private Sub New(model As D3D.Consolidated.Designs.ConsolidatedDesign, settings As AdjustCarSettings)
            _model = model
            _adjustSettings = settings
        End Sub

        Public ReadOnly Property AdjustMode As Boolean

        Public Sub Toggle(changeType As ObjectManipulatorChangeType)
            _AdjustMode = Not AdjustMode
            Me.UpdateAdjustMode()
            OnAdjustModeChanged(New AdjustModeChangedEventArgs(changeType))
        End Sub

        Public Sub Disable(changeType As ObjectManipulatorChangeType)
            If AdjustMode Then
                _AdjustMode = False
                UpdateAdjustMode()
                OnAdjustModeChanged(New AdjustModeChangedEventArgs(changeType))
            End If
        End Sub

        Friend Sub UpdateAdjustMode()
            With _model
                Dim isDisposed As Boolean = .IsDisposed

                If Not isDisposed Then
                    .SuspendUpdate(True)

                    .Viewports.Clear()
                    .Entities.Regen()

                    If AdjustMode Then
                        _adjustSettings.Layout4.InitModel(_model)
                    Else
                        .Viewports.Add(_adjustSettings.Layout4.Main)
                        .LayoutMode = viewportLayoutType.SingleViewport
                    End If

                    .Invalidate()

                    .SuspendUpdate(False)
                    .UpdateVisibleSelection()
                Else
                    Throw New ObjectDisposedException(NameOf(devDept.Eyeshot.Design))
                End If
            End With
        End Sub

        Private Sub _model_HandleCreated(sender As Object, e As EventArgs) Handles _model.HandleCreated
            UpdateAdjustMode()
        End Sub

        Protected Overridable Sub OnAdjustModeChanged(e As AdjustModeChangedEventArgs)
            RaiseEvent Changed(Me, e)
        End Sub

        Private Sub _carStateMachine_Initialized(sender As Object, e As EventArgs) Handles _carStateMachine.Initialized
            If AdjustMode Then
                Me._adjustSettings.ObjectManipulator.Enable(_carStateMachine.GetEntities)
            End If
        End Sub

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not _disposedValue Then
                If disposing Then
                End If

                _model = Nothing
                _adjustSettings = Nothing
                _carStateMachine = Nothing
                _disposedValue = True
            End If
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub

        Public Shared Function Create(model As D3D.Consolidated.Designs.ConsolidatedDesign, settings As AdjustCarSettings) As CreationResult
            Dim adjustMachine As New AdjustStateMachine(model, settings)
            Dim carmachine As New CarStateMachine(model, settings, adjustMachine)
            adjustMachine._carStateMachine = carmachine
            Return New CreationResult(adjustMachine, carmachine)
        End Function

        Public Class CreationResult
            Public Sub New(adjustStateMachine As AdjustStateMachine, carStateMachine As CarStateMachine)
                Me.AdjustStateMachine = adjustStateMachine
                Me.CarStateMachine = carStateMachine
            End Sub

            Public ReadOnly Property AdjustStateMachine As AdjustStateMachine
            Public ReadOnly Property CarStateMachine As CarStateMachine
        End Class


    End Class

End Namespace