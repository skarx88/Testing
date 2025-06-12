Imports devDept.Eyeshot
Imports Zuken.E3.Lib.Eyeshot.Model

Namespace D3D.Consolidated.Controls

    Partial Public Class Consolidated3DControl

        ReadOnly Property IsSelecting As Boolean

        Public ReadOnly Property SelectedEntities As IEESelectedEntitiesCollection
            Get
                Return Design3D.SelectedEntities
            End Get
        End Property

        ReadOnly Property Entities As EEEntityCollection
            Get
                Return Design3D.Entities
            End Get
        End Property

        ReadOnly Property IsInitialized As Boolean
            Get
                Return _initialized > 0
            End Get
        End Property

        ReadOnly Property Initialized As InitializedType
            Get
                Return _initialized
            End Get
        End Property

        Property EventsEnabled As Boolean
            Get
                Return _eventsEnabled
            End Get
            Set(value As Boolean)
                If _eventsEnabled <> value Then
                    _previousEventsEnabled = _eventsEnabled
                    _eventsEnabled = value
                End If
            End Set
        End Property

        Property KeyRotateEnabled As Boolean
            Get
                Return _keyRotateEnabled
            End Get
            Set(value As Boolean)
                If value <> _keyRotateEnabled Then
                    _keyRotateEnabled = value
                    If value = True Then
                        _keyStepsBefore.SetViewportKeySteps(Of ViewportRotateStep)(Design3D)
                    Else
                        For Each rotStep As ViewportRotateStep In _keyStepsBefore.OfType(Of ViewportRotateStep).ToArray
                            _keyStepsBefore.Replace(rotStep, New ViewportRotateStep(rotStep.ViewportIndex, 0))
                        Next
                    End If
                End If
            End Set
        End Property

        Property KeyPanEnabled As Boolean
            Get
                Return _keyPanEnabled
            End Get
            Set(value As Boolean)
                If value <> _keyPanEnabled Then
                    _keyPanEnabled = value
                    If value = True Then
                        _keyStepsBefore.SetViewportKeySteps(Of ViewportPanStep)(Design3D)
                    Else
                        For Each panStep As ViewportPanStep In _keyStepsBefore.OfType(Of ViewportPanStep).ToArray
                            _keyStepsBefore.Replace(panStep, New ViewportPanStep(panStep.ViewportIndex, 0))
                        Next
                    End If
                End If
            End Set
        End Property

        Property CurrentTrans As Settings.CarTransformationSetting
            Get
                Return _carStateMachine.Transformation
            End Get
            Set(value As Settings.CarTransformationSetting)
                _carStateMachine.Transformation = value
            End Set
        End Property

        ReadOnly Property HasCar As Boolean
            Get
                Return _carStateMachine.HasCar
            End Get
        End Property

        ReadOnly Property HasChanges As Boolean
            Get
                Return (_carStateMachine?.HasChanges).GetValueOrDefault
            End Get
        End Property

        Property CarModelsDirectory As String
            Get
                Return CarModelsViewControl.Directory
            End Get
            Set(value As String)
                CarModelsViewControl.Directory = value
            End Set
        End Property

        ReadOnly Property IsUpdating As Boolean
            Get
                Return _isUpdating OrElse _isUpdatingViews
            End Get
        End Property

        Public ReadOnly Property AdjustMode As Boolean
            Get
                Return _adjustModeStateMachine.AdjustMode
            End Get
        End Property

        Public Sub DisableAdjustMode(type As ObjectManipulatorChangeType)
            _adjustModeStateMachine.Disable(type)
        End Sub

        Public Shadows ReadOnly Property Visible As Boolean
            Get
                Return _isDisplayed
            End Get
        End Property

    End Class

End Namespace